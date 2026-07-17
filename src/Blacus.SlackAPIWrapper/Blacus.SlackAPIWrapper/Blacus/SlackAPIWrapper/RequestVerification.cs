using System;
using System.Security.Cryptography;
using System.Text;

namespace Blacus.SlackAPIWrapper.Authentication
{

    public class RequestVerification
    {

        private const int timeStampValidForSeconds = 300;
        private const string slackVersion = "v0";

        public string SlackSignature { get; set; }
        public string SlackRequestTimestamp { get; set; }
        public string RequestBody { get; set; }
        public string SigningSecret { get; set; }

        public bool IgnoreTimestamp { get; set; } = false;

        public DateTime SlackRequestTimestampUTC { get => DateTime.UnixEpoch.AddSeconds(int.Parse(SlackRequestTimestamp)); }

        public int SlackRequestTimestampInt { get => int.Parse(SlackRequestTimestamp); }

        public string Hash { get => "v0=" + getHash(SigningSecret, SignatureBaseString); }
        public string SignatureBaseString { get => $"{slackVersion}:{SlackRequestTimestamp}:{RequestBody}"; }

        private bool? _verified = null;
        public bool Verified
        {

            get
            {

                if (!IgnoreTimestamp)
                {

                    double currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    double delta = Math.Abs(currentTime - SlackRequestTimestampInt);

                    if (delta > timeStampValidForSeconds)
                    {
                        return false;
                    }

                }

                if (_verified == null)
                {
                    byte[] signatureBytes = Encoding.UTF8.GetBytes(SlackSignature ?? string.Empty);
                    byte[] hashBytes = Encoding.UTF8.GetBytes(Hash ?? string.Empty);
                    _verified = CryptographicOperations.FixedTimeEquals(signatureBytes, hashBytes);
                }

                return (bool)_verified;

            }

        }

        private static string getHash(string signingSecret, string input)
        {

            byte[] key = Encoding.UTF8.GetBytes(signingSecret ?? string.Empty);
            byte[] sourceBytes = Encoding.UTF8.GetBytes(input ?? string.Empty);
            byte[] hashBytes = HMACSHA256.HashData(key, sourceBytes);

            return Convert.ToHexString(hashBytes).ToLowerInvariant();

        }

    }

}