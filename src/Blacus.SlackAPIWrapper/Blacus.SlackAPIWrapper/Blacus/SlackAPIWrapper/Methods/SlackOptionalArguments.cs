using System.Collections.Generic;
using System.Reflection;
using System.Text.Json.Serialization;

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

    }

    public abstract class SlackOptionalArguments : ISlackOptionalArguments
    {

    }

}