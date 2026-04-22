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
    public class ResponseServiceTests
    {
        private const string BaseUrl = "https://api.surveymonkey.com/v3";
        private const string Token = "test-token";

        private static (Mock<HttpMessageHandler> handler, ResponseService service)
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
                        JsonSerializer.Serialize(responseBody), Encoding.UTF8, "application/json")
                });

            var httpClient = new HttpClient(handler.Object);
            var throttler = new Mock<IRequestThrottler>();
            throttler.Setup(t => t.WaitAsync(It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            return (handler, new ResponseService(httpClient, throttler.Object, BaseUrl, Token));
        }

        [Fact]
        public async Task ListAsync_CallsCorrectEndpoint()
        {
            var (handler, service) = BuildService(HttpStatusCode.OK,
                new PagedResponse<SurveyResponse>
                {
                    Data = new() { new SurveyResponse { Id = "r1" } }
                });

            var result = await service.ListAsync("s1");

            Assert.Single(result.Data);
            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.RequestUri!.PathAndQuery.Contains("surveys/s1/responses")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetAsync_CallsCorrectEndpoint()
        {
            var (handler, service) = BuildService(HttpStatusCode.OK,
                new SurveyResponse { Id = "r1" });

            await service.GetAsync("s1", "r1");

            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.RequestUri!.AbsolutePath.EndsWith("/surveys/s1/responses/r1")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task CreateAsync_PostsResponse()
        {
            var (handler, service) = BuildService(HttpStatusCode.Created,
                new SurveyResponse { Id = "r2" });

            var result = await service.CreateAsync("s1", new SurveyResponseRequest());

            Assert.Equal("r2", result.Id);
            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Post &&
                    r.RequestUri!.AbsolutePath.EndsWith("/surveys/s1/responses")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAsync_PatchesResponse()
        {
            var (handler, service) = BuildService(HttpStatusCode.OK,
                new SurveyResponse { Id = "r1" });

            await service.UpdateAsync("s1", "r1", new SurveyResponseRequest());

            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Patch &&
                    r.RequestUri!.AbsolutePath.EndsWith("/surveys/s1/responses/r1")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task DeleteAsync_DeletesResponse()
        {
            var (handler, service) = BuildService(HttpStatusCode.NoContent, new { });

            await service.DeleteAsync("s1", "r1");

            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Delete &&
                    r.RequestUri!.AbsolutePath.EndsWith("/surveys/s1/responses/r1")),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
