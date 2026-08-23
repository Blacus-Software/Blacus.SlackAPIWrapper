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
