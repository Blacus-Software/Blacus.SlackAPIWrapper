using Blacus.SlackAPIWrapper.Models;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.Json;


namespace Blacus.SlackAPIWrapper
{

    public class JsonExtensionDataHelper
    {

        public string ClassName { get; private set; }
        public ISlackModel slackModel { get; set; }
        public Dictionary<string, JsonElement>? ModelExtensionData { get => slackModel.ExtensionData; }
        public ItemHandler[] Items { get; set; }

        public Dictionary<string, string> Classes { get; set; } = new();

        public JsonExtensionDataHelper(ISlackModel slackModel, string className)
        {

            this.ClassName = className;
            this.slackModel = slackModel;

            Items = new ItemHandler[ModelExtensionData.Count];

            var modelExtensionDataArray = ModelExtensionData.ToArray();

            for (int i = 0; i < ModelExtensionData.Count; i++)
            {
                Items[i] = new ItemHandler(modelExtensionDataArray[i]);
            }

            AddClass(this, Classes);

        }

        private static void AddClass(JsonExtensionDataHelper helper, Dictionary<string, string> classes)
        {

            classes.TryAdd(helper.ClassName, helper.GetClassString(helper.ClassName));

            foreach (var item in helper.Items.Where(itm => itm.IsObject))
            {

                var extJamd = new JsonExtensionDataHelper(item.Model, item.ObjectName);

                classes.TryAdd(item.ObjectName, extJamd.GetClassString(item.ObjectName));

                AddClass(new(item.Model, item.ObjectName), classes);

            }

        }

        public string GetAllClassStrings()
        {

            return string.Join("\\n", Classes.Select(cls => cls.Value));

        }

        public string GetClassString(string? className = "Root")
        {

            string properties = string.Join("\n", Items.Select(im => im.PropertyDefinition));

            return @$"public class {className}
{{

{properties}
}}
";

        }

    }

    public class ItemHandler
    {

        public override string ToString() => $"{ObjectName}";

        public static readonly TextInfo myTI = new CultureInfo("en-US", false).TextInfo;

        public string Key { get; private set; }
        public JsonElement Element { get; private set; }

        public bool IsObject => Element.ValueKind == JsonValueKind.Object;

        public ItemHandler(string key, JsonElement item)
        {
            this.Key = key;
            this.Element = item;
        }

        public ItemHandler(KeyValuePair<string, JsonElement> modelExtensionData) :
            this(modelExtensionData.Key, modelExtensionData.Value)
        { }

        public string ObjectName { get => myTI.ToTitleCase(Key).Replace("_", string.Empty); }

        public string ObjectType
        {
            get
            {
                switch (Element.ValueKind)
                {
                    default:
                        return "unknown";

                    case JsonValueKind.Null:
                    case JsonValueKind.Object:
                        return ObjectName;

                    case JsonValueKind.String:
                        return "string";

                    case JsonValueKind.Number:
                        return "long";

                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        return "bool";

                    case JsonValueKind.Array:
                        return "object[]";

                }
            }
        }

        public string PropertyDefinition
        {

            get => $@"[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
[JsonPropertyName(""{Key}"")]
public {ObjectType}? {ObjectName} {{ get; set; }}        
";
        }

        public string PropertyJson => Element.ToString();

        public ISlackModel Model
        {
            get
            {
                if (IsObject) return ISlackModel.FromJson(PropertyJson);

                return null;
            }

        }

    }

}