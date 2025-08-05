using Blacus.SlackAPIWrapper.Models;
using System.IO;

namespace Blacus.SlackAPIWrapper
{

    public class UrlVerificationResponse : SlackModel
    {

        public string Challenge { get; set; }

        public UrlVerificationResponse(string challenge) =>
            Challenge = challenge;

    }

}