using Blacus.SlackAPIWrapper.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Methods
{

    public interface ISlackMethod;

    public abstract class SlackMethod : ISlackMethod
    {

        /// <summary>
        /// Should always be checked before trying to process a response.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }

        /// <summary>
        /// if ok is false, then this is the reason-code
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("error")]
        public string? Error { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object>? ExtensionData { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? SourceJson { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("response_metadata")]
        public ResponseMetaData? ResponseMetadata { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("needed")]
        public string? Needed { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("provided")]
        public string? Provided { get; set; }

    }

}