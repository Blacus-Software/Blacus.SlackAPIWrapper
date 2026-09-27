using Blacus.SlackAPIWrapper;
using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blacus.SlackAPIWrapper.Tests
{

    public class SlackASPTransmissionTests
    {

        private class MockHttpContextAccessor : IHttpContextAccessor
        {
            public HttpContext? HttpContext { get; set; }
        }

        private static string ComputeSignature(string secret, string timestamp, string body)
        {
            string baseString = $"v0:{timestamp}:{body}";
            byte[] key = Encoding.UTF8.GetBytes(secret);
            byte[] sourceBytes = Encoding.UTF8.GetBytes(baseString);
            byte[] hash = HMACSHA256.HashData(key, sourceBytes);
            return "v0=" + Convert.ToHexString(hash).ToLowerInvariant();
        }

        [Fact]
        public void Payload_BeforePrepareAsync_ThrowsInvalidOperationException()
        {
            var httpContext = new DefaultHttpContext();
            var accessor = new MockHttpContextAccessor { HttpContext = httpContext };
            var credential = new SlackCredential { SigningSecret = "secret" };
            var transmission = new SlackTransmission(accessor, credential);

            Assert.Throws<InvalidOperationException>(() => transmission.Payload);
        }

        [Fact]
        public async Task PrepareAsync_UrlVerification_WritesChallengeAndShortCircuits()
        {
            var httpContext = new DefaultHttpContext();
            string jsonBody = "{\"type\": \"url_verification\", \"challenge\": \"test_challenge_123\"}";
            httpContext.Request.ContentType = "application/json";
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(jsonBody));
            httpContext.Response.Body = new MemoryStream();

            var accessor = new MockHttpContextAccessor { HttpContext = httpContext };
            var credential = new SlackCredential { SigningSecret = "secret" };
            var transmission = new SlackTransmission(accessor, credential);

            await transmission.PrepareAsync();

            Assert.True(transmission.ShortCircuited);
            Assert.NotNull(transmission.Payload);
            Assert.IsType<SlackUrlResponsePayload>(transmission.Payload);

            httpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(httpContext.Response.Body);
            string responseText = await reader.ReadToEndAsync();
            Assert.Contains("test_challenge_123", responseText);
        }

        [Fact]
        public async Task PrepareAsync_ValidSignature_SetsVerifiedTrue()
        {
            string secret = "signing_secret_xyz";
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string jsonBody = "{\"type\": \"view_submission\", \"trigger_id\": \"trig_999\"}";
            string signature = ComputeSignature(secret, timestamp, jsonBody);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = "application/json";
            httpContext.Request.Headers["X-Slack-Request-Timestamp"] = new StringValues(timestamp);
            httpContext.Request.Headers["X-Slack-Signature"] = new StringValues(signature);
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(jsonBody));
            httpContext.Response.Body = new MemoryStream();

            var accessor = new MockHttpContextAccessor { HttpContext = httpContext };
            var credential = new SlackCredential { SigningSecret = secret };
            var transmission = new SlackTransmission(accessor, credential);

            await transmission.PrepareAsync();

            Assert.True(transmission.Verified);
            Assert.False(transmission.ShortCircuited);
            Assert.NotNull(transmission.Payload);
            Assert.IsType<SlackViewSubmissionPayload>(transmission.Payload);
        }

        [Fact]
        public async Task PrepareAsync_InvalidSignature_Sets401AndShortCircuits()
        {
            string secret = "signing_secret_xyz";
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string jsonBody = "{\"type\": \"view_submission\", \"trigger_id\": \"trig_999\"}";

            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = "application/json";
            httpContext.Request.Headers["X-Slack-Request-Timestamp"] = new StringValues(timestamp);
            httpContext.Request.Headers["X-Slack-Signature"] = new StringValues("v0=invalid_signature");
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(jsonBody));
            httpContext.Response.Body = new MemoryStream();

            var accessor = new MockHttpContextAccessor { HttpContext = httpContext };
            var credential = new SlackCredential { SigningSecret = secret };
            var transmission = new SlackTransmission(accessor, credential);

            await transmission.PrepareAsync();

            Assert.False(transmission.Verified);
            Assert.True(transmission.ShortCircuited);
            Assert.Equal(401, httpContext.Response.StatusCode);
        }

        [Fact]
        public async Task PrepareAsync_NullPayload_MarksPreparedSafely()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = "text/plain";
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("unknown format"));

            var accessor = new MockHttpContextAccessor { HttpContext = httpContext };
            var credential = new SlackCredential { SigningSecret = "secret" };
            var transmission = new SlackTransmission(accessor, credential);

            await transmission.PrepareAsync();

            Assert.Null(transmission.Payload);
            Assert.False(transmission.Verified);
            Assert.False(transmission.ShortCircuited);
        }

        [Fact]
        public void PayloadFromASP_FormUrlEncoded_ParsesSlashCommand()
        {
            string contentType = "application/x-www-form-urlencoded; charset=UTF-8";
            string body = "token=gIkuvaNzQIHg97ATvDxqgjtO&team_id=T0001&channel_id=C2147483705&channel_name=test&user_id=U2147483697&user_name=Steve&command=%2Fweather&text=94070&response_url=https%3A%2F%2Fhooks.slack.com%2Fcommands%2F1234%2F5678&trigger_id=13345224609.738474920.8088930838d88f008e0";

            var payload = SlackTransmission.PayloadFromASP(contentType, body);

            Assert.NotNull(payload);
            Assert.IsType<SlackSlashCommandPayload>(payload);
            var slashPayload = (SlackSlashCommandPayload)payload!;

            Assert.Equal("/weather", slashPayload.Command);
            Assert.Equal("94070", slashPayload.Text);
            Assert.Equal("Steve", slashPayload.UserName);
            Assert.Equal("U2147483697", slashPayload.UserId);
            Assert.Equal("https://hooks.slack.com/commands/1234/5678", slashPayload.ResponseUrl?.ToString());
        }

        [Fact]
        public void PayloadFromASP_FormUrlEncoded_WithPayloadField_ParsesInnerJson()
        {
            string contentType = "application/x-www-form-urlencoded";
            string innerJson = "{\"type\": \"view_submission\", \"trigger_id\": \"trig_123\"}";
            string body = "payload=" + System.Web.HttpUtility.UrlEncode(innerJson);

            var payload = SlackTransmission.PayloadFromASP(contentType, body);

            Assert.NotNull(payload);
            Assert.IsType<SlackViewSubmissionPayload>(payload);
            Assert.Equal("trig_123", payload?.TriggerId);
        }

        [Fact]
        public void PayloadFromASP_Json_ParsesDirectly()
        {
            string contentType = "application/json";
            string body = "{\"type\": \"url_verification\", \"challenge\": \"test_challenge_code\"}";

            var payload = SlackTransmission.PayloadFromASP(contentType, body);

            Assert.NotNull(payload);
            Assert.IsType<SlackUrlResponsePayload>(payload);
            Assert.Equal("test_challenge_code", ((SlackUrlResponsePayload)payload!).Challenge);
        }

        [Fact]
        public void PayloadFromASP_EmptyBody_ReturnsNull()
        {
            var payload = SlackTransmission.PayloadFromASP("application/json", "");
            Assert.Null(payload);
        }

    }

}
