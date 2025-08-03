using System;
using System.Linq;
using System.Net.Http;
using System.Reflection;

namespace Blacus.SlackAPIWrapper.Attributes
{

    public class SlackMethodAttribute : Attribute
    {

        public string Path { get; }
        public HttpMethod HttpMethod { get; }
        public SlackMethodContentType ContentType { get; }

        public SlackMethodAttribute(string path, string httpMethod, SlackMethodContentType contentType)
        {
            Path = path;
            HttpMethod = HttpMethod.Parse(httpMethod);
            ContentType = contentType;
        }

        public static SlackMethodAttribute GetSlackMethodAttribute<T>()
        {

            var type = typeof(T);
            TypeInfo info = type.GetTypeInfo();
            var slackMethodAttribute = info.GetCustomAttributes<SlackMethodAttribute>().FirstOrDefault();
            return slackMethodAttribute;

        }

    }

    public enum SlackMethodContentType
    {
        Json,
        FormUrlEncoded,
        MultipartFormData
    }

}