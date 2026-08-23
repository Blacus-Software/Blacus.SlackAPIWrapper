using Blacus.SlackAPIWrapper;
using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using System.Text.Json;
using Xunit;

namespace Blacus.SlackAPIWrapper.Tests
{

    public class SlackASPTransmissionTests
    {

        [Fact]
        public void PayloadFromASP_FormUrlEncoded_ParsesSlashCommand()
        {
            string contentType = "application/x-www-form-urlencoded; charset=UTF-8";
            string body = "token=gIkuvaNzQIHg97ATvDxqgjtO&team_id=T0001&channel_id=C2147483705&channel_name=test&user_id=U2147483697&user_name=Steve&command=%2Fweather&text=94070&response_url=https%3A%2F%2Fhooks.slack.com%2Fcommands%2F1234%2F5678&trigger_id=13345224609.738474920.8088930838d88f008e0";

            var payload = SlackASPTransmission.PayloadFromASP(contentType, body);

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

            var payload = SlackASPTransmission.PayloadFromASP(contentType, body);

            Assert.NotNull(payload);
            Assert.IsType<SlackViewSubmissionPayload>(payload);
            Assert.Equal("trig_123", payload?.TriggerId);
        }

        [Fact]
        public void PayloadFromASP_Json_ParsesDirectly()
        {
            string contentType = "application/json";
            string body = "{\"type\": \"url_verification\", \"challenge\": \"test_challenge_code\"}";

            var payload = SlackASPTransmission.PayloadFromASP(contentType, body);

            Assert.NotNull(payload);
            Assert.IsType<SlackUrlResponsePayload>(payload);
            Assert.Equal("test_challenge_code", ((SlackUrlResponsePayload)payload!).Challenge);
        }

        [Fact]
        public void PayloadFromASP_EmptyBody_ReturnsNull()
        {
            var payload = SlackASPTransmission.PayloadFromASP("application/json", "");
            Assert.Null(payload);
        }

    }

}
