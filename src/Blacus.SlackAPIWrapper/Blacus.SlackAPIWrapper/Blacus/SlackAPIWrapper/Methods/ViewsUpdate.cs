using Blacus.SlackAPIWrapper.Attributes;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("views.update", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class ViewsUpdate : SlackMethod
    {

        public static async Task<ViewsUpdate> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
              => await SlackClient.HttpRequestAsync<ViewsUpdate>(token, slackOptionalArguments);

        public string? external_id { get; set; }
        public string? view_id { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {

            public string? view { get; set; }
            public string? external_id { get; set; }
            public string? view_id { get; set; }

        }

    }

}