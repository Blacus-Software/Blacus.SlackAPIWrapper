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
    using System.Web;

    public interface ISlackASPTransmission
    {

        public ISlackPayload Payload { get; set; }
        public SlackCredential DefaultSlackCredential { get; set; }

        public string Code { get; set; }

    }

    public class SlackASPTransmission : ISlackASPTransmission
    {

        private IHttpContextAccessor httpContextAccessor;
        public HttpContext HttpContext { get => httpContextAccessor.HttpContext; }

        public ISlackPayload Payload { get; set; }
        public SlackCredential DefaultSlackCredential { get; set; }
        public string Code { get; set; }

        public SlackASPTransmission(IHttpContextAccessor httpContextAccessor, SlackCredential slackCredential)
        {

            this.httpContextAccessor = httpContextAccessor;
            DefaultSlackCredential = slackCredential;

            string bodyAsString = null;

            using StreamReader reader = new(HttpContext.Request.Body, leaveOpen: true);
            bodyAsString = reader.ReadToEndAsync().Result;

            Payload = PayloadFromASP(HttpContext.Request.ContentType, bodyAsString);

            if (Payload == null)
                return;

            if (Payload.Type == PayloadType.UrlVerification)
            {
                this.HttpContext.Response.WriteAsync(new UrlVerificationResponse(((SlackPayload_UrlResponse)Payload).Challenge).ToJson()).Wait();
                return;
            }

            var headers = HttpContext.Request.Headers;
            var verified = new RequestVerification()
            {
                RequestBody = bodyAsString,
                SigningSecret = DefaultSlackCredential.SigningSecret,
                SlackRequestTimestamp = headers["X-Slack-Request-Timestamp"].FirstOrDefault(),
                SlackSignature = headers["X-Slack-Signature"].FirstOrDefault()
            };

            if (!verified.Verified)
            {
                this.HttpContext.Response.StatusCode = 401;
                return;
            }

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
                    toReturn = ISlackModel.FromJson<SlackPayload>(json);
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
                var toReturn = ISlackModel.FromJson<SlackPayload>(json);
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

                SlackPayload_Command toReturn = new()
                {
                    TypeString = "slash_command",
                    Token = HttpUtility.UrlDecode(values["token"]),
                    TeamID = HttpUtility.UrlDecode(values["team_id"]),
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