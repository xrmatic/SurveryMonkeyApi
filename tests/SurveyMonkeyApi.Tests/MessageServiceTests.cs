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
    public class MessageServiceTests
    {
        private const string BaseUrl = "https://api.surveymonkey.com/v3";
        private const string Token = "test-token";

        private static (Mock<HttpMessageHandler> handler, MessageService service)
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

            return (handler, new MessageService(httpClient, throttler.Object, BaseUrl, Token));
        }

        [Fact]
        public async Task ListAsync_CallsCorrectEndpoint()
        {
            var (handler, service) = BuildService(HttpStatusCode.OK,
                new PagedResponse<Message> { Data = new() { new Message { Id = "m1" } } });

            await service.ListAsync("c1");

            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.RequestUri!.PathAndQuery.Contains("collectors/c1/messages")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetAsync_CallsCorrectEndpoint()
        {
            var (handler, service) = BuildService(HttpStatusCode.OK, new Message { Id = "m1" });

            await service.GetAsync("c1", "m1");

            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.RequestUri!.AbsolutePath.EndsWith("/collectors/c1/messages/m1")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task CreateAsync_PostsToCorrectEndpoint()
        {
            var (handler, service) = BuildService(HttpStatusCode.Created, new Message { Id = "m2" });

            var result = await service.CreateAsync("c1", new MessageRequest { Type = "invite" });

            Assert.Equal("m2", result.Id);
            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Post &&
                    r.RequestUri!.AbsolutePath.EndsWith("/collectors/c1/messages")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAsync_PatchesMessage()
        {
            var (handler, service) = BuildService(HttpStatusCode.OK, new Message { Id = "m1" });

            await service.UpdateAsync("c1", "m1", new MessageRequest { Subject = "Hi" });

            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Patch &&
                    r.RequestUri!.AbsolutePath.EndsWith("/collectors/c1/messages/m1")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task DeleteAsync_DeletesMessage()
        {
            var (handler, service) = BuildService(HttpStatusCode.NoContent, new { });

            await service.DeleteAsync("c1", "m1");

            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Delete &&
                    r.RequestUri!.AbsolutePath.EndsWith("/collectors/c1/messages/m1")),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
