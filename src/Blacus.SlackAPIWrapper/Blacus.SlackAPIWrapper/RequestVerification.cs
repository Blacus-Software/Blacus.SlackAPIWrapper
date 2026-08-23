using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Blacus.SlackAPIWrapper.Authentication
{

    public class RequestVerification
    {

        private const int timeStampValidForSeconds = 300;
        private const string slackVersion = "v0";

        public string? SlackSignature { get; set; }
        public string? SlackRequestTimestamp { get; set; }
        public string? RequestBody { get; set; }
        public string? SigningSecret { get; set; }

        public bool IgnoreTimestamp { get; set; } = false;

        public long SlackRequestTimestampLong
        {
            get
            {
                if (long.TryParse(SlackRequestTimestamp ?? string.Empty, NumberStyles.Integer, CultureInfo.InvariantCulture, out long timestamp))
                    return timestamp;

                return long.MinValue;
            }
        }

        public int SlackRequestTimestampInt => (int)Math.Clamp(SlackRequestTimestampLong, int.MinValue, int.MaxValue);

        public bool HasValidTimestamp => long.TryParse(SlackRequestTimestamp ?? string.Empty, NumberStyles.Integer, CultureInfo.InvariantCulture, out _);

        public DateTime SlackRequestTimestampUTC => HasValidTimestamp ? DateTime.UnixEpoch.AddSeconds(SlackRequestTimestampLong) : DateTime.MinValue;

        public string Hash => "v0=" + getHash(SigningSecret, SignatureBaseString);
        public string SignatureBaseString => $"{slackVersion}:{SlackRequestTimestamp}:{RequestBody}";

        public bool Verified
        {

            get
            {

                if (string.IsNullOrWhiteSpace(SigningSecret) || string.IsNullOrWhiteSpace(SlackSignature))
                    return false;

                if (!IgnoreTimestamp)
                {

                    if (!HasValidTimestamp)
                    {
                        return false;
                    }

                    long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    long delta = Math.Abs(currentTime - SlackRequestTimestampLong);

                    if (delta > timeStampValidForSeconds)
                    {
                        return false;
                    }

                }

                byte[] signatureBytes = Encoding.UTF8.GetBytes(SlackSignature);
                byte[] hashBytes = Encoding.UTF8.GetBytes(Hash);
                return CryptographicOperations.FixedTimeEquals(signatureBytes, hashBytes);

            }

        }

        private static string getHash(string? signingSecret, string? input)
        {

            byte[] key = Encoding.UTF8.GetBytes(signingSecret ?? string.Empty);
            byte[] sourceBytes = Encoding.UTF8.GetBytes(input ?? string.Empty);
            byte[] hashBytes = HMACSHA256.HashData(key, sourceBytes);

#if NET9_0_OR_GREATER
            return Convert.ToHexStringLower(hashBytes);
#else
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
#endif

        }

    }

}