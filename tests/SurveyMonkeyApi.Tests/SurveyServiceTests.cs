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
    /// <summary>
    /// Tests for <see cref="SurveyService"/> verifying correct endpoint construction,
    /// Bearer token attachment, and throttler integration.
    /// </summary>
    public class SurveyServiceTests
    {
        private const string BaseUrl = "https://api.surveymonkey.com/v3";
        private const string Token = "test-token";

        private static (Mock<HttpMessageHandler> handler, SurveyService service)
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

            var service = new SurveyService(httpClient, throttler.Object, BaseUrl, Token);
            return (handler, service);
        }

        [Fact]
        public async Task ListAsync_CallsCorrectEndpoint()
        {
            var expected = new PagedResponse<Survey>
            {
                Data = new() { new Survey { Id = "1", Title = "Test" } },
                Total = 1
            };
            var (handler, service) = BuildService(HttpStatusCode.OK, expected);

            var result = await service.ListAsync(page: 1, perPage: 10);

            Assert.Single(result.Data);
            Assert.Equal("1", result.Data[0].Id);

            handler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Get &&
                    r.RequestUri!.PathAndQuery.Contains("surveys") &&
                    r.RequestUri.PathAndQuery.Contains("page=1") &&
                    r.Headers.Authorization!.Scheme == "Bearer" &&
                    r.Headers.Authorization.Parameter == Token),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetAsync_CallsCorrectEndpoint()
        {
            var expected = new Survey { Id = "42", Title = "My Survey" };
            var (handler, service) = BuildService(HttpStatusCode.OK, expected);

            var result = await service.GetAsync("42");

            Assert.Equal("42", result.Id);
            handler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Get &&
                    r.RequestUri!.AbsolutePath.EndsWith("/surveys/42")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task CreateAsync_PostsToCorrectEndpoint()
        {
            var expected = new Survey { Id = "99", Title = "New Survey" };
            var (handler, service) = BuildService(HttpStatusCode.Created, expected);

            var result = await service.CreateAsync(new SurveyRequest { Title = "New Survey" });

            Assert.Equal("99", result.Id);
            handler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Post &&
                    r.RequestUri!.AbsolutePath.EndsWith("/surveys")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAsync_PatchesToCorrectEndpoint()
        {
            var expected = new Survey { Id = "99", Title = "Updated" };
            var (handler, service) = BuildService(HttpStatusCode.OK, expected);

            var result = await service.UpdateAsync("99", new SurveyRequest { Title = "Updated" });

            Assert.Equal("99", result.Id);
            handler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Patch &&
                    r.RequestUri!.AbsolutePath.EndsWith("/surveys/99")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task DeleteAsync_SendsDeleteToCorrectEndpoint()
        {
            var (handler, service) = BuildService(HttpStatusCode.NoContent, new { });

            await service.DeleteAsync("99");

            handler.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Delete &&
                    r.RequestUri!.AbsolutePath.EndsWith("/surveys/99")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public void GetAsync_ThrowsOnNullOrWhiteSpaceId()
        {
            var (_, service) = BuildService(HttpStatusCode.OK, new Survey());
            var ex = Record.Exception(() => { _ = service.GetAsync(""); });
            Assert.IsType<ArgumentException>(ex);
        }

        [Fact]
        public void CreateAsync_ThrowsOnNullRequest()
        {
            var (_, service) = BuildService(HttpStatusCode.OK, new Survey());
            var ex = Record.Exception(() => { _ = service.CreateAsync(null!); });
            Assert.IsType<ArgumentNullException>(ex);
        }

        [Fact]
        public async Task WaitAsync_IsCalledBeforeEachRequest()
        {
            var expected = new Survey { Id = "1" };
            var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            handler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(() => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(expected), Encoding.UTF8, "application/json")
                }));

            var throttler = new Mock<IRequestThrottler>();
            throttler.Setup(t => t.WaitAsync(It.IsAny<CancellationToken>()))
                     .Returns(Task.CompletedTask);

            var service = new SurveyService(new HttpClient(handler.Object), throttler.Object, BaseUrl, Token);

            await service.GetAsync("1");
            await service.GetAsync("1");

            throttler.Verify(t => t.WaitAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
    }
}
