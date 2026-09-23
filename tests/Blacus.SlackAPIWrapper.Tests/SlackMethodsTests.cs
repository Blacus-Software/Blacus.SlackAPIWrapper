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

        [Fact]
        public void UsersInfo_Metadata_HasCorrectAttributes()
        {
            var methodAttr = SlackMethodAttribute.GetSlackMethodAttribute<UsersInfo>();
            Assert.NotNull(methodAttr);
            Assert.Equal("users.info", methodAttr.Path);
            Assert.Equal(HttpMethod.Post, methodAttr.HttpMethod);
            Assert.Equal(SlackMethodContentType.FormUrlEncoded, methodAttr.ContentType);

            var limiterAttr = SlackLimiterTierAttribute.GetSlackLimiterAttribute<UsersInfo>();
            Assert.NotNull(limiterAttr);
            Assert.Equal(LimiterTier.WebAPITier4, limiterAttr.LimiterTier);
        }

        [Fact]
        public void UsersInfo_OptionalArguments_Serialization_BehavesCorrectly()
        {
            var args = new UsersInfo.OptionalArguments
            {
                User = "W012A3CDE",
                IncludeLocale = true
            };

            string json = args.ToJson();
            Assert.Contains("\"user\":\"W012A3CDE\"", json);
            Assert.Contains("\"include_locale\":true", json);

            var encoded = args.GetFormUrlEncode();
            Assert.Contains(encoded, kv => kv.Key == "user" && kv.Value == "W012A3CDE");
            Assert.Contains(encoded, kv => kv.Key == "include_locale" && kv.Value == "true");

            // Test UserId alias maps to User and does not duplicate in form url encoding
            var aliasArgs = new UsersInfo.OptionalArguments
            {
                UserId = "U98765"
            };
            Assert.Equal("U98765", aliasArgs.User);
            Assert.Equal("U98765", aliasArgs.UserId);

            var aliasEncoded = aliasArgs.GetFormUrlEncode();
            Assert.Contains(aliasEncoded, kv => kv.Key == "user" && kv.Value == "U98765");
            Assert.DoesNotContain(aliasEncoded, kv => kv.Key == "UserId");
        }

        [Fact]
        public async Task UsersInfo_ExecuteAsync_DeserializesResponseCorrectly()
        {
            var originalClient = SlackClient.HttpClient;
            HttpRequestMessage? capturedRequest = null;
            string sampleJsonResponse = """
            {
              "ok": true,
              "user": {
                "id": "W012A3CDE",
                "team_id": "T012AB3C4",
                "name": "spengler",
                "deleted": false,
                "color": "9f69e7",
                "real_name": "Egon Spengler",
                "tz": "America/Los_Angeles",
                "tz_label": "Pacific Daylight Time",
                "tz_offset": -25200,
                "profile": {
                  "avatar_hash": "ge3b51ca72de",
                  "status_text": "Print is dead",
                  "status_emoji": ":books:",
                  "real_name": "Egon Spengler",
                  "display_name": "spengler",
                  "real_name_normalized": "Egon Spengler",
                  "display_name_normalized": "spengler",
                  "email": "spengler@ghostbusters.example.com",
                  "image_original": "https://example.com/avatar/orig.jpg",
                  "image_24": "https://example.com/avatar/24.jpg",
                  "image_32": "https://example.com/avatar/32.jpg",
                  "image_48": "https://example.com/avatar/48.jpg",
                  "image_72": "https://example.com/avatar/72.jpg",
                  "image_192": "https://example.com/avatar/192.jpg",
                  "image_512": "https://example.com/avatar/512.jpg",
                  "team": "T012AB3C4"
                },
                "is_admin": true,
                "is_owner": false,
                "is_primary_owner": false,
                "is_restricted": false,
                "is_ultra_restricted": false,
                "is_bot": false,
                "updated": 1502138686,
                "is_app_user": false,
                "has_2fa": false,
                "locale": "en-US"
              }
            }
            """;

            var handler = new MockHandler
            {
                HandlerFunc = req =>
                {
                    capturedRequest = req;
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(sampleJsonResponse, System.Text.Encoding.UTF8, "application/json")
                    };
                }
            };
            SlackClient.HttpClient = new HttpClient(handler);

            try
            {
                var result = await UsersInfo.ExecuteAsync("test-user-token", new UsersInfo.OptionalArguments
                {
                    User = "W012A3CDE",
                    IncludeLocale = true
                });

                Assert.NotNull(result);
                Assert.True(result.Ok);
                Assert.NotNull(result.User);
                Assert.Equal("W012A3CDE", result.User.Id);
                Assert.Equal("T012AB3C4", result.User.TeamId);
                Assert.Equal("spengler", result.User.Name);
                Assert.Equal("Egon Spengler", result.User.RealName);
                Assert.False(result.User.Deleted);
                Assert.True(result.User.IsAdmin);
                Assert.False(result.User.IsOwner);
                Assert.False(result.User.IsBot);
                Assert.False(result.User.Has2Fa);
                Assert.Equal("en-US", result.User.Locale);
                Assert.Equal(1502138686, result.User.Updated);

                Assert.NotNull(result.User.Profile);
                Assert.Equal("Print is dead", result.User.Profile.StatusText);
                Assert.Equal(":books:", result.User.Profile.StatusEmoji);
                Assert.Equal("spengler@ghostbusters.example.com", result.User.Profile.Email);

                Assert.NotNull(capturedRequest);
                Assert.Equal(HttpMethod.Post, capturedRequest.Method);
                Assert.Equal("https://slack.com/api/users.info", capturedRequest.RequestUri?.ToString());
                Assert.Equal("Bearer", capturedRequest.Headers.Authorization?.Scheme);
                Assert.Equal("test-user-token", capturedRequest.Headers.Authorization?.Parameter);
            }
            finally
            {
                SlackClient.HttpClient = originalClient;
            }
        }

    }

}
