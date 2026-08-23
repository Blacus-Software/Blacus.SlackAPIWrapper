namespace Blacus.SlackAPIWrapper
{
    using Blacus.SlackAPIWrapper.Authentication;
    using Blacus.SlackAPIWrapper.Models;
    using Blacus.SlackAPIWrapper.Models.SlackPayloads;
    using Microsoft.AspNetCore.Http;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    public interface ISlackASPTransmission
    {

        public ISlackPayload Payload { get; }

        public SlackCredential DefaultSlackCredential { get; set; }

        public string Code { get; set; }

        public bool Verified { get; }

        public bool ShortCircuited { get; }

        public Task PrepareAsync();

    }

    public class SlackASPTransmission : ISlackASPTransmission
    {

        private readonly IHttpContextAccessor httpContextAccessor;
        private ISlackPayload payload;
        private bool prepared;

        public HttpContext HttpContext { get => httpContextAccessor.HttpContext; }

        public ISlackPayload Payload
        {

            get
            {

                if (!prepared)
                    throw new InvalidOperationException("The transmission has not been prepared. Call PrepareAsync() before accessing the payload.");

                return payload;

            }

            internal set => payload = value;

        }

        public SlackCredential DefaultSlackCredential { get; set; }
        public string Code { get; set; }

        public bool Verified { get; private set; }
        public bool ShortCircuited { get; private set; }

        public SlackASPTransmission(IHttpContextAccessor httpContextAccessor, SlackCredential slackCredential)
        {

            this.httpContextAccessor = httpContextAccessor;
            DefaultSlackCredential = slackCredential;

        }

        public async Task PrepareAsync()
        {

            if (prepared)
                return;

            string bodyAsString;
            using (StreamReader reader = new(HttpContext.Request.Body, encoding: Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 1024, leaveOpen: true))
            {
                bodyAsString = await reader.ReadToEndAsync();
            }

            payload = PayloadFromASP(HttpContext.Request.ContentType, bodyAsString);

            if (payload == null)
            {
                prepared = true;
                return;
            }

            if (payload.GetType() == typeof(SlackUrlResponsePayload))
            {

                string response = new UrlVerificationResponse(((SlackUrlResponsePayload)payload).Challenge).ToJson();
                HttpContext.Response.ContentType = "application/json";
                await HttpContext.Response.WriteAsync(response);

                prepared = true;
                ShortCircuited = true;
                return;

            }

            var verified = new RequestVerification()
            {
                RequestBody = bodyAsString,
                SigningSecret = DefaultSlackCredential.SigningSecret,
                SlackRequestTimestamp = HttpContext.Request.Headers["X-Slack-Request-Timestamp"].FirstOrDefault(),
                SlackSignature = HttpContext.Request.Headers["X-Slack-Signature"].FirstOrDefault()
            };

            prepared = true;

            if (!verified.Verified)
            {
                HttpContext.Response.StatusCode = 401;
                ShortCircuited = true;
                return;
            }

            Verified = true;

        }


        public static ISlackPayload PayloadFromASP(string contentType, string bodyString)
        {

            ISlackPayload? toReturn = null;
            string json = bodyString;

            switch (contentType)
            {
                case "application/x-www-form-urlencoded":
                    toReturn = processFormData(bodyString);
                    break;

                case "application/json":
                    toReturn = ISlackModel.FromJson<ISlackPayload>(json);
                    break;

                default:
                    Console.WriteLine($"ContentType not found: {contentType}");
                    break;
            }

            return toReturn;

        }

        private static ISlackPayload processFormData(string bodyString)
        {

            if (bodyString.StartsWith("payload="))
            {
                var json = HttpUtility.UrlDecode(bodyString.Substring(8));
                var toReturn = ISlackModel.FromJson<ISlackPayload>(json);
                return toReturn;
            }
            else
            {

                Dictionary<string, string> values = new();
                var formData = bodyString.Split("&");

                foreach (var item in formData)
                {
                    var split = item.Split("=");
                    values.Add(split[0], split[1]);
                }

                SlackSlashCommandPayload toReturn = new()
                {
                    Token = HttpUtility.UrlDecode(values["token"]),
                    TeamId = HttpUtility.UrlDecode(values["team_id"]),
                    // EnterpriseId = HttpUtility.UrlDecode(values["enterprise_id"]),
                    ResponseUrl = new Uri(HttpUtility.UrlDecode(values["response_url"])),
                    TriggerId = HttpUtility.UrlDecode(values["trigger_id"]),
                    ChannelId = HttpUtility.UrlDecode(values["channel_id"]),
                    UserId = HttpUtility.UrlDecode(values["user_id"]),
                    UserName = HttpUtility.UrlDecode(values["user_name"]),
                    Command = HttpUtility.UrlDecode(values["command"]),
                    Text = HttpUtility.UrlDecode(values["text"]),
                    ApiAppId = HttpUtility.UrlDecode(values["api_app_id"]),
                };

                return toReturn;

            }

        }

    }

}
