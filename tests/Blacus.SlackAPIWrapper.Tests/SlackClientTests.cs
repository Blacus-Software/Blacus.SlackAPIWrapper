using Blacus.SlackAPIWrapper;
using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Methods;
using Blacus.SlackAPIWrapper.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Blacus.SlackAPIWrapper.Tests
{

    [Collection("SlackClientStaticCollection")]
    public class SlackClientTests
    {

        private class MockHttpMessageHandler : HttpMessageHandler
        {
            public HttpRequestMessage? LastRequest { get; private set; }
            public string? LastRequestBody { get; private set; }
            public Func<HttpRequestMessage, HttpResponseMessage>? ResponseFactory { get; set; }

            public HttpResponseMessage ResponseToReturn { get; set; } = new(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"ok\": true}")
            };

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                cancellationToken.ThrowIfCancellationRequested();
                LastRequest = request;
                if (request.Content != null)
                {
                    LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
                }
                return ResponseFactory != null ? ResponseFactory(request) : ResponseToReturn;
            }
        }

        [SlackMethod("test.method", "POST", SlackMethodContentType.Json)]
        private class TestJsonMethod : SlackMethod
        {
            public class OptionalArguments : SlackOptionalArguments
            {
                [JsonPropertyName("foo")]
                public string? Foo { get; set; }
            }
        }

        [SlackMethod("test.form", "POST", SlackMethodContentType.FormUrlEncoded)]
        private class TestFormMethod : SlackMethod
        {
            public class OptionalArguments : SlackOptionalArguments
            {
                [JsonPropertyName("bar")]
                public string? Bar { get; set; }
            }
        }

        private class MethodWithoutAttribute : SlackMethod
        {
        }

        [SlackLimiterTier(LimiterTier.WebAPITier4)]
        [SlackMethod("test.limited", "POST", SlackMethodContentType.Json)]
        private class TestLimitedMethod : SlackMethod
        {
        }

        [Fact]
        public async Task SendAsync_NonSuccessStatusCode_ThrowsHttpRequestException()
        {
            var handler = new MockHttpMessageHandler
            {
                ResponseToReturn = new HttpResponseMessage(HttpStatusCode.BadRequest)
            };
            var client = new SlackClient(new HttpClient(handler));

            await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await client.SendAsync<TestJsonMethod>("token");
            });
        }

        [Fact]
        public async Task SendAsync_StreamJsonFalse_ReturnsDeserializedModel()
        {
            var handler = new MockHttpMessageHandler
            {
                ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"ok\": true, \"error\": null}")
                }
            };
            var client = new SlackClient(new HttpClient(handler));

            var result = await client.SendAsync<TestJsonMethod>("token", streamJson: false);
            Assert.NotNull(result);
            Assert.True(result.Ok);
        }

        [Fact]
        public async Task SendAsync_WithLimiterTier_Succeeds()
        {
            var handler = new MockHttpMessageHandler
            {
                ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"ok\": true}")
                }
            };
            var client = new SlackClient(new HttpClient(handler));

            var result = await client.SendAsync<TestLimitedMethod>("token");
            Assert.NotNull(result);
            Assert.True(result.Ok);
        }

        [Fact]
        public void SlackClient_Constructor_ThrowsArgumentNullException_ForNullFactory()
        {
            IHttpClientFactory nullFactory = null!;
            Assert.Throws<ArgumentNullException>(() => new SlackClient(nullFactory));
        }

        [Fact]
        public async Task HttpRequestAsync_StaticMethods_Succeed()
        {
            var originalClient = SlackClient.HttpClient;
            var handler = new MockHttpMessageHandler
            {
                ResponseFactory = _ => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"ok\": true}")
                }
            };
            SlackClient.HttpClient = new HttpClient(handler);

            try
            {
                var result1 = await SlackClient.HttpRequestAsync<TestJsonMethod>("test-token");
                Assert.NotNull(result1);
                Assert.True(result1.Ok);

                var result2 = await SlackClient.HttpRequestAsync<TestJsonMethod>(new AuthenticationHeaderValue("Bearer", "test-token"));
                Assert.NotNull(result2);
                Assert.True(result2.Ok);
            }
            finally
            {
                SlackClient.HttpClient = originalClient;
            }
        }

        [Fact]
        public async Task SendAsync_JsonMethod_SendsCorrectHeadersAndPayload()
        {
            var handler = new MockHttpMessageHandler
            {
                ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"ok\": true}")
                }
            };

            var httpClient = new HttpClient(handler);
            var client = new SlackClient(httpClient);

            var args = new TestJsonMethod.OptionalArguments { Foo = "baz" };
            var result = await client.SendAsync<TestJsonMethod>("xoxb-test-token", args);

            Assert.NotNull(result);
            Assert.True(result.Ok);
            Assert.NotNull(handler.LastRequest);
            Assert.Equal("https://slack.com/api/test.method", handler.LastRequest.RequestUri?.ToString());
            Assert.Equal("Bearer", handler.LastRequest.Headers.Authorization?.Scheme);
            Assert.Equal("xoxb-test-token", handler.LastRequest.Headers.Authorization?.Parameter);
            Assert.Contains("\"foo\":\"baz\"", handler.LastRequestBody);
        }

        [Fact]
        public async Task SendAsync_FormMethod_SendsUrlEncodedContent()
        {
            var handler = new MockHttpMessageHandler
            {
                ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"ok\": true}")
                }
            };

            var httpClient = new HttpClient(handler);
            var client = new SlackClient(httpClient);

            var args = new TestFormMethod.OptionalArguments { Bar = "qux" };
            var result = await client.SendAsync<TestFormMethod>("xoxb-test-token", args);

            Assert.NotNull(result);
            Assert.True(result.Ok);
            Assert.NotNull(handler.LastRequest);
            Assert.Equal("https://slack.com/api/test.form", handler.LastRequest.RequestUri?.ToString());
            Assert.Equal("bar=qux", handler.LastRequestBody);
        }

        [Fact]
        public async Task SendAsync_MissingAttribute_ThrowsInvalidOperationException()
        {
            var client = new SlackClient(new HttpClient(new MockHttpMessageHandler()));
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await client.SendAsync<MethodWithoutAttribute>("token");
            });
        }

        [Fact]
        public async Task SendAsync_PropagatesCancellationToken()
        {
            var handler = new MockHttpMessageHandler();
            var client = new SlackClient(new HttpClient(handler));

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
            {
                await client.SendAsync<TestJsonMethod>("token", cancellationToken: cts.Token);
            });
        }

    }

}
