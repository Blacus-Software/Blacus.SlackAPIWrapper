using Blacus.SlackAPIWrapper.JsonConverters;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Blacus.SlackAPIWrapper.Tests
{

    public class JsonConvertersTests
    {

        private class BoolTestModel
        {
            [JsonConverter(typeof(BoolConverter))]
            public bool Value { get; set; }
        }

        private class DateOnlyTestModel
        {
            [JsonConverter(typeof(DateOnlyConverter))]
            public DateOnly Date { get; set; }
        }

        private class TimeOnlyTestModel
        {
            [JsonConverter(typeof(TimeOnlyConverter))]
            public TimeOnly Time { get; set; }
        }

        private class DateTimeOffsetTestModel
        {
            [JsonConverter(typeof(DateTimeOffsetAsIntConverter))]
            public DateTimeOffset Timestamp { get; set; }
        }

        [Theory]
        [InlineData("{\"Value\": true}", true)]
        [InlineData("{\"Value\": false}", false)]
        [InlineData("{\"Value\": \"true\"}", true)]
        [InlineData("{\"Value\": \"false\"}", false)]
        [InlineData("{\"Value\": \"1\"}", true)]
        [InlineData("{\"Value\": \"0\"}", false)]
        [InlineData("{\"Value\": 1}", true)]
        [InlineData("{\"Value\": 0}", false)]
        public void BoolConverter_Deserializes_Correctly(string json, bool expected)
        {
            var result = JsonSerializer.Deserialize<BoolTestModel>(json, JsonConverterSettings.Singleton);
            Assert.NotNull(result);
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public void BoolConverter_Serializes_Correctly()
        {
            var model = new BoolTestModel { Value = true };
            var json = JsonSerializer.Serialize(model, JsonConverterSettings.Singleton);
            Assert.Contains("\"Value\":true", json);
        }

        [Fact]
        public void DateOnlyConverter_CanConvert_ChecksDateOnly()
        {
            var converter = new DateOnlyConverter();
            Assert.True(converter.CanConvert(typeof(DateOnly)));
            Assert.False(converter.CanConvert(typeof(TimeOnly)));
        }

        [Fact]
        public void DateOnlyConverter_Deserializes_And_Serializes()
        {
            string json = "{\"Date\": \"2026-08-22\"}";
            var model = JsonSerializer.Deserialize<DateOnlyTestModel>(json, JsonConverterSettings.Singleton);
            Assert.NotNull(model);
            Assert.Equal(new DateOnly(2026, 8, 22), model.Date);

            var serialized = JsonSerializer.Serialize(model, JsonConverterSettings.Singleton);
            Assert.Contains("\"Date\":\"2026-08-22\"", serialized);
        }

        [Fact]
        public void TimeOnlyConverter_CanConvert_ChecksTimeOnly()
        {
            var converter = new TimeOnlyConverter();
            Assert.True(converter.CanConvert(typeof(TimeOnly)));
            Assert.False(converter.CanConvert(typeof(DateOnly)));
        }

        [Fact]
        public void TimeOnlyConverter_Deserializes_And_Serializes()
        {
            string json = "{\"Time\": \"14:30\"}";
            var model = JsonSerializer.Deserialize<TimeOnlyTestModel>(json, JsonConverterSettings.Singleton);
            Assert.NotNull(model);
            Assert.Equal(new TimeOnly(14, 30), model.Time);

            var serialized = JsonSerializer.Serialize(model, JsonConverterSettings.Singleton);
            Assert.Contains("\"Time\":\"14:30\"", serialized);
        }

        [Fact]
        public void DateTimeOffsetAsIntConverter_Deserializes_NumericAndString()
        {
            string jsonNumber = "{\"Timestamp\": 1700000000}";
            var modelNumber = JsonSerializer.Deserialize<DateTimeOffsetTestModel>(jsonNumber, JsonConverterSettings.Singleton);
            Assert.NotNull(modelNumber);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1700000000), modelNumber.Timestamp);

            string jsonString = "{\"Timestamp\": \"1700000000.0002\"}";
            var modelString = JsonSerializer.Deserialize<DateTimeOffsetTestModel>(jsonString, JsonConverterSettings.Singleton);
            Assert.NotNull(modelString);
            Assert.Equal(DateTimeOffset.FromUnixTimeSeconds(1700000000), modelString.Timestamp);
        }

        [Fact]
        public void DateTimeHelpers_ToDateTimeFromUnixTimeStamp_ConvertsCorrectly()
        {
            long unixTimestamp = 1700000000;
            var expectedUtc = DateTime.UnixEpoch.AddSeconds(unixTimestamp);
            var result = Helpers.DateTimeHelpers.ToDateTimeFromUnixTimeStamp(unixTimestamp);
            Assert.Equal(expectedUtc.ToLocalTime(), result);
        }

    }

}
