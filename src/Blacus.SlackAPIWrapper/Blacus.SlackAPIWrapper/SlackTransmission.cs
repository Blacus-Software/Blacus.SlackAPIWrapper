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

    public interface ISlackTransmission
    {

        public ISlackPayload? Payload { get; }

        public SlackCredential DefaultSlackCredential { get; set; }

        public bool Verified { get; }

        public bool ShortCircuited { get; }

        public Task PrepareAsync();

    }

    public class SlackTransmission : ISlackTransmission
    {

        private readonly IHttpContextAccessor httpContextAccessor;
        private ISlackPayload? payload;
        private bool prepared;

        public HttpContext? HttpContext { get => httpContextAccessor.HttpContext; }

        public ISlackPayload? Payload
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

        public bool Verified { get; private set; }
        public bool ShortCircuited { get; private set; }

        public SlackTransmission(IHttpContextAccessor httpContextAccessor, SlackCredential slackCredential)
        {

            this.httpContextAccessor = httpContextAccessor;
            DefaultSlackCredential = slackCredential;

        }

        public async Task PrepareAsync()
        {

            if (prepared)
                return;

            if (HttpContext == null)
                throw new InvalidOperationException("HttpContext is not available.");

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

            if (payload is SlackUrlResponsePayload urlPayload)
            {

                string response = new UrlVerificationResponse(urlPayload.Challenge ?? string.Empty).ToJson();
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


        public static ISlackPayload? PayloadFromASP(string? contentType, string? bodyString)
        {

            if (string.IsNullOrWhiteSpace(bodyString))
                return null;

            ISlackPayload? toReturn = null;
            string mediaType = contentType?.Split(';')[0].Trim().ToLowerInvariant() ?? string.Empty;

            switch (mediaType)
            {
                case "application/x-www-form-urlencoded":
                    toReturn = processFormData(bodyString);
                    break;

                case "application/json":
                    toReturn = ISlackModel.FromJson<ISlackPayload>(bodyString);
                    break;

                default:
                    // Unknown or unsupported content type
                    break;
            }

            return toReturn;

        }

        private static ISlackPayload? processFormData(string bodyString)
        {

            if (string.IsNullOrWhiteSpace(bodyString))
                return null;

            var parsed = HttpUtility.ParseQueryString(bodyString);

            var payloadJson = parsed["payload"];
            if (!string.IsNullOrEmpty(payloadJson))
            {
                return ISlackModel.FromJson<ISlackPayload>(payloadJson);
            }

            Uri? responseUrl = null;
            var responseUrlString = parsed["response_url"];
            if (!string.IsNullOrEmpty(responseUrlString) && Uri.TryCreate(responseUrlString, UriKind.Absolute, out var uri))
            {
                responseUrl = uri;
            }

            SlackSlashCommandPayload toReturn = new()
            {
                Token = parsed["token"],
                TeamId = parsed["team_id"],
                TeamDomain = parsed["team_domain"],
                ResponseUrl = responseUrl,
                TriggerId = parsed["trigger_id"],
                ChannelId = parsed["channel_id"],
                ChannelName = parsed["channel_name"],
                UserId = parsed["user_id"],
                UserName = parsed["user_name"],
                Command = parsed["command"],
                Text = parsed["text"],
                ApiAppId = parsed["api_app_id"],
                IsEnterpriseInstall = bool.TryParse(parsed["is_enterprise_install"], out var isEnterprise) ? isEnterprise : null,
            };

            return toReturn;

        }

    }

}
