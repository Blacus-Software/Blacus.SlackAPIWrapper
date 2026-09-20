using Blacus.SlackAPIWrapper;
using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Methods;
using Blacus.SlackAPIWrapper.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Blacus.SlackAPIWrapper.Tests
{

    [Collection("SlackClientStaticCollection")]
    public class SlackMethodsTests
    {

        private class MockHandler : HttpMessageHandler
        {
            public Func<HttpRequestMessage, HttpResponseMessage> HandlerFunc { get; set; } =
                _ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{\"ok\": true}") };

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return Task.FromResult(HandlerFunc(request));
            }
        }

        private class UnannotatedMethod : SlackMethod
        {
        }

        [Fact]
        public void SlackMethodAttribute_GetSlackMethodAttribute_ReturnsCorrectMetadata()
        {
            var chatPostAttr = SlackMethodAttribute.GetSlackMethodAttribute<ChatPostMessage>();
            Assert.NotNull(chatPostAttr);
            Assert.Equal("chat.postMessage", chatPostAttr.Path);
            Assert.Equal(HttpMethod.Post, chatPostAttr.HttpMethod);

            var unannotatedAttr = SlackMethodAttribute.GetSlackMethodAttribute<UnannotatedMethod>();
            Assert.Null(unannotatedAttr);
        }

        [Fact]
        public void SlackLimiterTierAttribute_GetSlackLimiterAttribute_ReturnsNullWhenNotAnnotated()
        {
            var limiterAttr = SlackLimiterTierAttribute.GetSlackLimiterAttribute<UnannotatedMethod>();
            Assert.Null(limiterAttr);
        }

        private class SampleArguments : SlackOptionalArguments
        {
            [JsonPropertyName("channel_id")]
            public string? ChannelId { get; set; }

            [JsonPropertyName("is_active")]
            public bool? IsActive { get; set; }

            public string? NormalProperty { get; set; }
        }

        [Fact]
        public void SlackOptionalArguments_GetFormUrlEncode_FormatsCorrectly()
        {
            ISlackOptionalArguments args = new SampleArguments
            {
                ChannelId = "C12345",
                IsActive = true,
                NormalProperty = "custom_val"
            };

            var encoded = args.GetFormUrlEncode();

            Assert.Contains(encoded, kv => kv.Key == "channel_id" && kv.Value == "C12345");
            Assert.Contains(encoded, kv => kv.Key == "is_active" && kv.Value == "true");
            Assert.Contains(encoded, kv => kv.Key == "NormalProperty" && kv.Value == "custom_val");
        }

        [Fact]
        public void SlackOptionalArguments_GetRequestQuery_FormatsCorrectly()
        {
            ISlackOptionalArguments args = new SampleArguments
            {
                ChannelId = "C12345",
                IsActive = false
            };

            var query = args.GetRequestQuery();

            Assert.Equal("C12345", query["channel_id"]);
            Assert.Equal("false", query["is_active"]);
        }

        [Fact]
        public void SlackOptionalArguments_ToJson_SerializesCorrectly()
        {
            ISlackOptionalArguments args = new SampleArguments
            {
                ChannelId = "C999",
                IsActive = true
            };

            string json = args.ToJson();

            Assert.Contains("\"channel_id\":\"C999\"", json);
            Assert.Contains("\"is_active\":true", json);
        }

        [Fact]
        public async Task ConversationsHistory_ExecuteAsync_CallsSlackClient()
        {
            var originalClient = SlackClient.HttpClient;
            var handler = new MockHandler
            {
                HandlerFunc = _ => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"ok\": true, \"messages\": [{\"text\": \"hello\"}]}")
                }
            };
            SlackClient.HttpClient = new HttpClient(handler);

            try
            {
                var result = await ConversationsHistory.ExecuteAsync("token-123", new ConversationsHistory.OptionalArguments
                {
                    Channel = "C123"
                });

                Assert.NotNull(result);
                Assert.True(result.Ok);
                Assert.NotNull(result.Messages);
                Assert.Single(result.Messages);
                Assert.Equal("hello", result.Messages[0].Text);
            }
            finally
            {
                SlackClient.HttpClient = originalClient;
            }
        }

        [Fact]
        public async Task ConversationsOpen_ExecuteAsync_CallsSlackClient()
        {
            var originalClient = SlackClient.HttpClient;
            var handler = new MockHandler
            {
                HandlerFunc = _ => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"ok\": true, \"no_op\": true}")
                }
            };
            SlackClient.HttpClient = new HttpClient(handler);

            try
            {
                var result = await ConversationsOpen.ExecuteAsync("token-123", new ConversationsOpen.OptionalArguments
                {
                    Users = "U123"
                });

                Assert.NotNull(result);
                Assert.True(result.Ok);
                Assert.True(result.NoOp);
            }
            finally
            {
                SlackClient.HttpClient = originalClient;
            }
        }

        [Fact]
        public async Task ConversationsReplies_ExecuteAsync_CallsSlackClient()
        {
            var originalClient = SlackClient.HttpClient;
            var handler = new MockHandler
            {
                HandlerFunc = _ => new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{\"ok\": true, \"has_more\": false}")
                }
            };
            SlackClient.HttpClient = new HttpClient(handler);

            try
            {
                var result = await ConversationsReplies.ExecuteAsync("token-123", new ConversationsReplies.OptionalArguments
                {
                    Channel = "C123"
                });

                Assert.NotNull(result);
                Assert.True(result.Ok);
                Assert.False(result.HasMore);
            }
            finally
            {
                SlackClient.HttpClient = originalClient;
            }
        }

        [Fact]
        public void OAuthV2Access_OptionalArguments_FormUrlEncode_ProducesCorrectSnakeCaseKeys()
        {
            var args = new OAuthV2Access.OptionalArguments
            {
                ClientId = "test-client-id",
                ClientSecret = "test-client-secret",
                Code = "test-code",
                CodeVerifier = "test-code-verifier",
                RedirectUri = "https://example.com/oauth/callback",
                GrantType = "authorization_code",
                RefreshToken = "test-refresh-token"
            };

            var encoded = args.GetFormUrlEncode();

            Assert.Contains(encoded, kv => kv.Key == "client_id" && kv.Value == "test-client-id");
            Assert.Contains(encoded, kv => kv.Key == "client_secret" && kv.Value == "test-client-secret");
            Assert.Contains(encoded, kv => kv.Key == "code" && kv.Value == "test-code");
            Assert.Contains(encoded, kv => kv.Key == "code_verifier" && kv.Value == "test-code-verifier");
            Assert.Contains(encoded, kv => kv.Key == "redirect_uri" && kv.Value == "https://example.com/oauth/callback");
            Assert.Contains(encoded, kv => kv.Key == "grant_type" && kv.Value == "authorization_code");
            Assert.Contains(encoded, kv => kv.Key == "refresh_token" && kv.Value == "test-refresh-token");
        }

        [Fact]
        public void OAuthV2Access_BackwardCompatibilityAliases_WorkCorrectly()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            var args = new OAuthV2Access.OptionalArguments
            {
                client_id = "old-client-id",
                client_secret = "old-client-secret",
                code = "old-code",
                grant_type = "refresh_token",
                redirect_uri = "https://example.com/cb",
                refresh_token = "old-ref-token"
            };

            Assert.Equal("old-client-id", args.ClientId);
            Assert.Equal("old-client-secret", args.ClientSecret);
            Assert.Equal("old-code", args.Code);
            Assert.Equal("refresh_token", args.GrantType);
            Assert.Equal("https://example.com/cb", args.RedirectUri);
            Assert.Equal("old-ref-token", args.RefreshToken);

            // Test top-level obsolete type alias
            Blacus.SlackAPIWrapper.Methods.OptionalArguments topLevelArgs = new()
            {
                ClientId = "alias-id"
            };
            Assert.Equal("alias-id", topLevelArgs.ClientId);
#pragma warning restore CS0618
        }

        [Fact]
        public async Task OAuthV2Access_ExecuteAsync_UsesBasicAuthHeader()
        {
            var originalClient = SlackClient.HttpClient;
            HttpRequestMessage? capturedRequest = null;
            var handler = new MockHandler
            {
                HandlerFunc = req =>
                {
                    capturedRequest = req;
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent("{\"ok\": true, \"access_token\": \"xoxb-new\"}")
                    };
                }
            };
            SlackClient.HttpClient = new HttpClient(handler);

            try
            {
                var result = await OAuthV2Access.ExecuteAsync(new OAuthV2Access.OptionalArguments
                {
                    ClientId = "my-client",
                    ClientSecret = "my-secret",
                    Code = "auth-code"
                });

                Assert.NotNull(result);
                Assert.True(result.Ok);
                Assert.Equal("xoxb-new", result.AccessToken);
                Assert.NotNull(capturedRequest);
                Assert.Equal("Basic", capturedRequest.Headers.Authorization?.Scheme);

                string expectedAuthParam = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes("my-client:my-secret"));
                Assert.Equal(expectedAuthParam, capturedRequest.Headers.Authorization?.Parameter);
            }
            finally
            {
                SlackClient.HttpClient = originalClient;
            }
        }

        [Fact]
        public async Task Limiter_TryGetAndDisposeAll_BehavesCorrectly()
        {
            string limiterId = "limiter_test_" + Guid.NewGuid().ToString();
            var created = Limiter.LimiterFactory(limiterId, 5, TimeSpan.FromSeconds(1));

            Assert.True(Limiter.TryGet(limiterId, out var retrieved));
            Assert.Same(created, retrieved);

            Assert.False(Limiter.TryGet("non_existent_limiter", out _));

            await Limiter.DisposeAllAsync();
            Assert.False(Limiter.TryGet(limiterId, out _));
        }

    }

}
