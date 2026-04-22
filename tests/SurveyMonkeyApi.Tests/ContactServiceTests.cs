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
    public class ContactServiceTests
    {
        private const string BaseUrl = "https://api.surveymonkey.com/v3";
        private const string Token = "test-token";

        private static (Mock<HttpMessageHandler> handler, ContactService service)
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

            return (handler, new ContactService(httpClient, throttler.Object, BaseUrl, Token));
        }

        [Fact]
        public async Task ListAsync_CallsContactsEndpoint()
        {
            var (handler, service) = BuildService(HttpStatusCode.OK,
                new PagedResponse<Contact> { Data = new() { new Contact { Id = "ct1" } } });

            await service.ListAsync();

            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.RequestUri!.PathAndQuery.Contains("contacts")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task GetAsync_CallsContactByIdEndpoint()
        {
            var (handler, service) = BuildService(HttpStatusCode.OK, new Contact { Id = "ct1" });

            await service.GetAsync("ct1");

            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.RequestUri!.AbsolutePath.EndsWith("/contacts/ct1")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task CreateAsync_PostsContact()
        {
            var (handler, service) = BuildService(HttpStatusCode.Created, new Contact { Id = "ct2" });

            var result = await service.CreateAsync(new ContactRequest { Email = "a@b.com" });

            Assert.Equal("ct2", result.Id);
            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Post &&
                    r.RequestUri!.AbsolutePath.EndsWith("/contacts")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAsync_PatchesContact()
        {
            var (handler, service) = BuildService(HttpStatusCode.OK, new Contact { Id = "ct1" });

            await service.UpdateAsync("ct1", new ContactRequest { FirstName = "Jane" });

            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Patch &&
                    r.RequestUri!.AbsolutePath.EndsWith("/contacts/ct1")),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task DeleteAsync_DeletesContact()
        {
            var (handler, service) = BuildService(HttpStatusCode.NoContent, new { });

            await service.DeleteAsync("ct1");

            handler.Protected().Verify("SendAsync", Times.Once(),
                ItExpr.Is<HttpRequestMessage>(r =>
                    r.Method == HttpMethod.Delete &&
                    r.RequestUri!.AbsolutePath.EndsWith("/contacts/ct1")),
                ItExpr.IsAny<CancellationToken>());
        }
    }
}
