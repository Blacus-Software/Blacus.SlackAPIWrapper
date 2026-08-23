using Blacus.SlackAPIWrapper;
using Blacus.SlackAPIWrapper.Models;
using Blacus.SlackAPIWrapper.Models.BlockElements;
using Blacus.SlackAPIWrapper.Models.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Blacus.SlackAPIWrapper.Tests
{

    public class ModelParsingExceptionTests
    {

        [Fact]
        public void FromJson_ThrowsArgumentException_WhenJsonIsNullEmptyOrWhitespace()
        {
            Assert.Throws<ArgumentException>(() => ISlackModel.FromJson<SlackModel>(""));
            Assert.Throws<ArgumentException>(() => ISlackModel.FromJson<SlackModel>("   "));
            Assert.Throws<ArgumentException>(() => SlackWebSocketTransmission.FromJson(""));
        }

        [Fact]
        public void FromJson_ThrowsArgumentNullException_WhenStreamIsNull()
        {
            Stream nullStream = null!;
            Assert.Throws<ArgumentNullException>(() => ISlackModel.FromJson<SlackModel>(nullStream));
        }

        [Fact]
        public async Task FromJsonAsync_ThrowsArgumentNullException_WhenStreamIsNull()
        {
            Stream nullStream = null!;
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await ISlackModel.FromJsonAsync<SlackModel>(nullStream);
            });
        }

        [Fact]
        public void FromJson_CapturesSourceJson()
        {
            string json = "{\"ok\": true}";
            var model = ISlackModel.FromJson<SlackModel>(json);
            Assert.NotNull(model);
            Assert.Equal(json, model.SourceJson);
        }

        [Fact]
        public void TextObject_TextGetter_NullValueWithMaxLength_ReturnsNull()
        {
            var textObj = new TextObject
            {
                MaxTextLength = 100,
                Text = null
            };

            Assert.Null(textObj.Text);
        }

        [Fact]
        public void TextObject_TextGetter_TruncatesWhenExceedingMaxLength()
        {
            var textObj = new TextObject
            {
                MaxTextLength = 5,
                Text = "Hello World"
            };

            Assert.Equal("Hello", textObj.Text);
        }

        [Fact]
        public void ConfirmationDialogObject_NullText_DoesNotThrow()
        {
            var dialog = new ConfirmationDialogObject();
            dialog.Text = null;
            Assert.Null(dialog.Text);
        }

        [Fact]
        public void DatePickerElement_NullPlaceholder_DoesNotThrow()
        {
            var datePicker = new DatePickerElement("date_action");
            datePicker.Placeholder = null;
            Assert.Null(datePicker.Placeholder);
        }

        [Fact]
        public void BlockOptionObject_NullText_DoesNotThrow()
        {
            var option = new BlockOptionObject();
            option.Text = null;
            Assert.Null(option.Text);
        }

        [Fact]
        public void JsonExtensionDataHelper_NullExtensionData_DoesNotThrow()
        {
            var model = new SlackModel();
            var helper = new JsonExtensionDataHelper(model, "TestClass");

            Assert.NotNull(helper);
            Assert.Empty(helper.Items);
            Assert.NotNull(helper.GetClassString());
        }

        [Fact]
        public void JsonExtensionDataHelper_WithProperties_GeneratesClasses()
        {
            var model = new SlackModel
            {
                ExtensionData = new Dictionary<string, JsonElement>
                {
                    ["user_name"] = JsonDocument.Parse("\"alice\"").RootElement,
                    ["is_admin"] = JsonDocument.Parse("true").RootElement
                }
            };

            var helper = new JsonExtensionDataHelper(model, "CustomClass");

            Assert.Equal(2, helper.Items.Length);
            string classString = helper.GetClassString("CustomClass");
            Assert.Contains("public string? UserName { get; set; }", classString);
            Assert.Contains("public bool? IsAdmin { get; set; }", classString);
        }

        [Fact]
        public void SlackWebSocketTransmission_FromJson_PolymorphicallyDeserializesHello()
        {
            string helloJson = "{\"type\": \"hello\", \"num_connections\": 4}";
            var transmission = SlackWebSocketTransmission.FromJson(helloJson);

            Assert.NotNull(transmission);
            Assert.IsType<SlackWebSocketTransmissionHello>(transmission);
            Assert.Equal(4, ((SlackWebSocketTransmissionHello)transmission).NumConnections);
        }

    }

}
