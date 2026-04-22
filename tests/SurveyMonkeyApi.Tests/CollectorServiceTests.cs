using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using SurveyMonkeyApi.Models;
using SurveyMonkeyApi.Services;
using SurveyMonkeyApi.Throttling;
using Xunit;

namespace SurveyMonkeyApi.Tests
{
    public class CollectorServiceTests
    {
        private const string BaseUrl = "https://api.surveymonkey.com/v3";
        private const string Token = "test-token";

        private static (Mock<HttpMessageHandler> handler, CollectorService service)
            BuildService(HttpStatusCode status, object responseBody)
        {
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(status)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(responseBody),
                        Encoding.UTF8,
                        "application/json")
                });

            var httpClient = new HttpClient(handler.Object);
            var throttler = new Mock<IRequestThrottler>();
            throttler.Setup(t => t.WaitAsync(It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            var service = new CollectorService(httpClient, throttler.Object, BaseUrl, Token);
            return (handler, service);
        }

        [Fact]
        public async Task ListAsync_CallsCorrectEndpointForSurvey()
        {
            var expected = new PagedResponse<Collector>
            {
                Data = new() { new Collector { Id = "c1" } }
            };
            var (handler, service) = BuildService(HttpStatusCode.OK, expected);

            var result = await service.ListAsync("s1");

            Assert.Single(result.Data);
            handler.Protected().Verify(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.RequestUri!.PathAndQuery.Contains("surveys/s1/collectors")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetAsync_CallsCollectorsEndpoint()
        {
            var (handler, service) = BuildService(HttpStatusCode.OK, new Collector { Id = "c1" });

            await service.GetAsync("c1");

            handler.Protected().Verify(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.RequestUri!.AbsolutePath.EndsWith("/collectors/c1")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task CreateAsync_PostsToSurveyCollectors()
        {
            var (handler, service) = BuildService(HttpStatusCode.Created, new Collector { Id = "c2" });

            var result = await service.CreateAsync("s1", new CollectorRequest { Type = "weblink" });

            Assert.Equal("c2", result.Id);
            handler.Protected().Verify(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Post &&
                    r.RequestUri!.AbsolutePath.EndsWith("/surveys/s1/collectors")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAsync_PatchesCollector()
        {
            var (handler, service) = BuildService(HttpStatusCode.OK, new Collector { Id = "c1" });

            await service.UpdateAsync("c1", new CollectorRequest { Name = "Updated" });

            handler.Protected().Verify(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Patch &&
                    r.RequestUri!.AbsolutePath.EndsWith("/collectors/c1")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task DeleteAsync_DeletesCollector()
        {
            var (handler, service) = BuildService(HttpStatusCode.NoContent, new { });

            await service.DeleteAsync("c1");

            handler.Protected().Verify(
                "SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Delete &&
                    r.RequestUri!.AbsolutePath.EndsWith("/collectors/c1")),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
