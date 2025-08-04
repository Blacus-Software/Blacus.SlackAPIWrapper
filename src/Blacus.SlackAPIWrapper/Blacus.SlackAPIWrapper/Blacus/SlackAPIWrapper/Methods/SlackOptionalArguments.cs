using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Web;

#pragma warning disable CS8601, CS8603, CS8600, CS8602, CS1998, CS8625, CS8618, CS8604

namespace Blacus.SlackAPIWrapper.Methods
{

    public interface ISlackOptionalArguments
    {

        public List<KeyValuePair<string, string>> GetFormUrlEncode()
        {

            var toReturn = new List<KeyValuePair<string, string>>();

            PropertyInfo[] properties = GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {

                var propertyValue = property.GetValue(this);

                if (propertyValue != null)
                {

                    var jsonName = property.GetCustomAttribute<JsonPropertyNameAttribute>();

                    if (jsonName == null)
                    {
                        toReturn.Add(new(property.Name, propertyValue.ToString()));
                    }
                    else
                    {
                        toReturn.Add(new(jsonName.Name, propertyValue.ToString()));
                    }

                }

            }

            return toReturn;

        }

        public NameValueCollection GetRequestQuery()
        {

            var toReturn = HttpUtility.ParseQueryString(string.Empty);

            PropertyInfo[] properties = this.GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {

                var propertyValue = property.GetValue(this);

                if (propertyValue != null)
                {

                    var jsonName = property.GetCustomAttribute<JsonPropertyNameAttribute>();

                    if (jsonName == null)
                    {
                        toReturn.Add(property.Name, propertyValue.ToString());
                    }
                    else
                    {
                        toReturn.Add(jsonName.Name, propertyValue.ToString());
                    }

                }

            }

            return toReturn;

        }

    }

    public abstract class SlackOptionalArguments : ISlackOptionalArguments
    {

    }

}