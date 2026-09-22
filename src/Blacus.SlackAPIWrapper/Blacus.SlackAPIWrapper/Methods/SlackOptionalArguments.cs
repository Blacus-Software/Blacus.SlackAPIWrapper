using Blacus.SlackAPIWrapper.JsonConverters;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;

namespace Blacus.SlackAPIWrapper.Methods
{

    public interface ISlackOptionalArguments
    {

        public string ToJson() =>
            JsonSerializer.Serialize<object>(this, JsonConverterSettings.Singleton);

        public List<KeyValuePair<string, string>> GetFormUrlEncode() =>
            SlackOptionalArgumentsHelper.GetFormUrlEncode(this);

        public NameValueCollection GetRequestQuery() =>
            SlackOptionalArgumentsHelper.GetRequestQuery(this);

    }

    internal static class SlackOptionalArgumentsHelper
    {

        public static List<KeyValuePair<string, string>> GetFormUrlEncode(ISlackOptionalArguments instance)
        {
            var toReturn = new List<KeyValuePair<string, string>>();

            PropertyInfo[] properties = instance.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttribute<JsonIgnoreAttribute>()?.Condition == JsonIgnoreCondition.Always)
                    continue;

                var propertyValue = property.GetValue(instance);

                if (propertyValue != null)
                {
                    var jsonName = property.GetCustomAttribute<JsonPropertyNameAttribute>();
                    string stringValue = propertyValue switch
                    {
                        bool b => b ? "true" : "false",
                        _ => propertyValue.ToString() ?? string.Empty
                    };

                    if (jsonName == null)
                    {
                        toReturn.Add(new(property.Name, stringValue));
                    }
                    else
                    {
                        toReturn.Add(new(jsonName.Name, stringValue));
                    }
                }
            }

            return toReturn;
        }

        public static NameValueCollection GetRequestQuery(ISlackOptionalArguments instance)
        {
            var toReturn = HttpUtility.ParseQueryString(string.Empty);

            PropertyInfo[] properties = instance.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttribute<JsonIgnoreAttribute>()?.Condition == JsonIgnoreCondition.Always)
                    continue;

                var propertyValue = property.GetValue(instance);

                if (propertyValue != null)
                {
                    var jsonName = property.GetCustomAttribute<JsonPropertyNameAttribute>();
                    string stringValue = propertyValue switch
                    {
                        bool b => b ? "true" : "false",
                        _ => propertyValue.ToString() ?? string.Empty
                    };

                    if (jsonName == null)
                    {
                        toReturn.Add(property.Name, stringValue);
                    }
                    else
                    {
                        toReturn.Add(jsonName.Name, stringValue);
                    }
                }
            }

            return toReturn;
        }

    }

    public abstract class SlackOptionalArguments : ISlackOptionalArguments
    {

        public virtual string ToJson() =>
            JsonSerializer.Serialize<object>(this, JsonConverterSettings.Singleton);

        public virtual List<KeyValuePair<string, string>> GetFormUrlEncode() =>
            SlackOptionalArgumentsHelper.GetFormUrlEncode(this);

        public virtual NameValueCollection GetRequestQuery() =>
            SlackOptionalArgumentsHelper.GetRequestQuery(this);

    }

}