using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

#pragma warning disable CS8601, CS8603, CS8600, CS8602, CS1998, CS8625, CS8618, CS8604

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("chat.postMessage", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class ChatPostMessage : SlackMethod
    {

        public static async Task<ChatPostMessage> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
        {
            if (slackOptionalArguments != null && !string.IsNullOrEmpty(slackOptionalArguments.Channel))
            {
                await Limiter.LimiterFactory(slackOptionalArguments.Channel, 1, new System.TimeSpan(0, 0, 1)).WaitAsync();
            }
            return await SlackClient.HttpRequestAsync<ChatPostMessage>(token, slackOptionalArguments);
        }

        public static async Task<string> TransmitAsync(string token, Uri uri, OptionalArguments slackOptionalArguments)
        {

            var client = SlackClient.HttpClient;
            var request = new HttpRequestMessage(HttpMethod.Post, uri);

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string json = System.Text.Json.JsonSerializer.Serialize<ChatPostMessage.OptionalArguments>(slackOptionalArguments);

            var content = new StringContent(json, null, "application/json");

            request.Content = content;

            var response = await client.SendAsync(request);

            string toReturn = await response.Content.ReadAsStringAsync();

            return toReturn;

        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("channel")]
        public string? Channel { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("ts")]
        public string? Ts { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("message")]
        public Message? Message { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("message")]
            public string? Message { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("channel")]
            public string? Channel { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("blocks")]
            public string? Blocks { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("fileIds")]
            public string? FileIds { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("link_names")]
            public string? LinkNames { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("metadata")]
            public string? Metadata { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("parse")]
            public string? Parse { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("text")]
            public string? Text { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("username")]
            public string? Username { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("icon_url")]
            public string? IconUrl { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("thread_ts")]
            public string? ThreadTs { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("attachments")]
            public string? Attachments { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("markdown_text")]
            public string? MarkdownText { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("reply_broadcast")]
            public bool? ReplyBroadcast { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("as_user")]
            public bool? AsUser { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("mrkdwn")]
            public bool? Mrkdwn { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("response_type")]
            public string? ResponseType { get; set; }

        }

    }

}
