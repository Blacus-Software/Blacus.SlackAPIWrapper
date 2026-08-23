using Blacus.SlackAPIWrapper.Authentication;
using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace Blacus.SlackAPIWrapper.Tests
{

    public class RequestVerificationTests
    {

        private static string ComputeSlackSignature(string secret, string timestamp, string body)
        {
            string baseString = $"v0:{timestamp}:{body}";
            byte[] key = Encoding.UTF8.GetBytes(secret);
            byte[] sourceBytes = Encoding.UTF8.GetBytes(baseString);
            byte[] hash = HMACSHA256.HashData(key, sourceBytes);
            return "v0=" + Convert.ToHexString(hash).ToLowerInvariant();
        }

        [Fact]
        public void Verified_ReturnsTrue_ForValidSignatureAndCurrentTimestamp()
        {
            string secret = "8f742231b10e8888abcd99yyzz";
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string body = "command=%2Ftest&text=hello";
            string signature = ComputeSlackSignature(secret, timestamp, body);

            var verifier = new RequestVerification
            {
                SigningSecret = secret,
                SlackRequestTimestamp = timestamp,
                RequestBody = body,
                SlackSignature = signature
            };

            Assert.True(verifier.Verified);
            Assert.True(verifier.HasValidTimestamp);
        }

        [Fact]
        public void Verified_ReturnsFalse_WhenSignatureDoesNotMatch()
        {
            string secret = "secret123";
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string body = "command=%2Ftest";
            string signature = "v0=0000000000000000000000000000000000000000000000000000000000000000";

            var verifier = new RequestVerification
            {
                SigningSecret = secret,
                SlackRequestTimestamp = timestamp,
                RequestBody = body,
                SlackSignature = signature
            };

            Assert.False(verifier.Verified);
        }

        [Fact]
        public void Verified_ReturnsFalse_WhenTimestampIsExpired()
        {
            string secret = "secret123";
            // 10 minutes ago (> 300 seconds)
            string timestamp = (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 600).ToString();
            string body = "command=%2Ftest";
            string signature = ComputeSlackSignature(secret, timestamp, body);

            var verifier = new RequestVerification
            {
                SigningSecret = secret,
                SlackRequestTimestamp = timestamp,
                RequestBody = body,
                SlackSignature = signature
            };

            Assert.False(verifier.Verified);

            // If timestamp is ignored, it should pass
            verifier.IgnoreTimestamp = true;
            Assert.True(verifier.Verified);
        }

        [Fact]
        public void Verified_ReturnsFalse_WhenSecretOrSignatureMissing()
        {
            var verifier1 = new RequestVerification
            {
                SigningSecret = null,
                SlackRequestTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                RequestBody = "test",
                SlackSignature = "v0=abc"
            };
            Assert.False(verifier1.Verified);

            var verifier2 = new RequestVerification
            {
                SigningSecret = "secret",
                SlackRequestTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                RequestBody = "test",
                SlackSignature = null
            };
            Assert.False(verifier2.Verified);
        }

    }

}
