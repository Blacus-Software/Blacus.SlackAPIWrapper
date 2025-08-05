using System;
using System.Security.Cryptography;
using System.Text;

namespace Blacus.SlackAPIWrapper.Authentication
{

    public class RequestVerification
    {

        private const int timeStampValidForSeconds = 10;
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

                    double currentTime = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                    double delta = currentTime - SlackRequestTimestampInt;

                    if (delta > timeStampValidForSeconds)
                    {
                        return false;
                    }

                }

                if (_verified == null)
                {
                    StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                    _verified = comparer.Compare(SlackSignature, Hash) == 0;
                }

                return (bool)_verified;

            }

        }

        private static string getHash(string signingSecret, string input)
        {

            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] code = encoder.GetBytes(signingSecret);
            HashAlgorithm hashAlgorithm = new HMACSHA256(code);

            // Convert the input string to a byte array and compute the hash.
            byte[] data = hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();

        }

    }

}