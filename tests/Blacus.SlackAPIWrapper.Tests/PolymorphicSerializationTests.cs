using Blacus.SlackAPIWrapper.JsonConverters;
using Blacus.SlackAPIWrapper.Models.BlockElements;
using Blacus.SlackAPIWrapper.Models.Blocks;
using Blacus.SlackAPIWrapper.Models.SlackActions;
using Blacus.SlackAPIWrapper.Models.SlackEvents;
using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using Blacus.SlackAPIWrapper.Models.WebSocket;
using System.Text.Json;
using Xunit;

namespace Blacus.SlackAPIWrapper.Tests
{

    public class PolymorphicSerializationTests
    {

        [Fact]
        public void SlackBlock_PolymorphicDeserialization()
        {
            string sectionJson = "{\"type\": \"section\", \"block_id\": \"b1\"}";
            var block = JsonSerializer.Deserialize<SlackBlock>(sectionJson, JsonConverterSettings.Singleton);
            Assert.IsType<SectionBlock>(block);
            Assert.Equal("b1", block?.BlockId);

            string dividerJson = "{\"type\": \"divider\", \"block_id\": \"b2\"}";
            var divider = JsonSerializer.Deserialize<SlackBlock>(dividerJson, JsonConverterSettings.Singleton);
            Assert.IsType<DividerBlock>(divider);
        }

        [Fact]
        public void BlockElement_PolymorphicDeserialization()
        {
            string buttonJson = "{\"type\": \"button\", \"action_id\": \"btn1\"}";
            var elem = JsonSerializer.Deserialize<BlockElement>(buttonJson, JsonConverterSettings.Singleton);
            Assert.IsType<ButtonElement>(elem);
            Assert.Equal("btn1", elem?.ActionId);

            string datePickerJson = "{\"type\": \"datepicker\", \"action_id\": \"dp1\"}";
            var datePicker = JsonSerializer.Deserialize<BlockElement>(datePickerJson, JsonConverterSettings.Singleton);
            Assert.IsType<DatePickerElement>(datePicker);
        }

        [Fact]
        public void SlackPayloadEvent_PolymorphicDeserialization()
        {
            string appMentionJson = "{\"type\": \"app_mention\", \"user\": \"U12345\"}";
            var evt = JsonSerializer.Deserialize<ISlackPayloadEvent>(appMentionJson, JsonConverterSettings.Singleton);
            Assert.IsType<EventAppMention>(evt);

            string reactionJson = "{\"type\": \"reaction_added\", \"reaction\": \"thumbsup\"}";
            var reactionEvt = JsonSerializer.Deserialize<ISlackPayloadEvent>(reactionJson, JsonConverterSettings.Singleton);
            Assert.IsType<EventReactionAdded>(reactionEvt);
        }

        [Fact]
        public void SlackAction_PolymorphicDeserialization()
        {
            string actionJson = "{\"type\": \"button\", \"action_id\": \"btn_click\"}";
            var action = JsonSerializer.Deserialize<ISlackAction>(actionJson, JsonConverterSettings.Singleton);
            Assert.IsType<ButtonAction>(action);
            Assert.Equal("btn_click", action?.ActionId);
        }

        [Fact]
        public void SlackWebSocket_PolymorphicDeserialization()
        {
            string slashJson = "{\"type\": \"slash_commands\", \"envelope_id\": \"3fa85f64-5717-4562-b3fc-2c963f66afa6\"}";
            var wsTransmission = JsonSerializer.Deserialize<ISlackWebSocketBase>(slashJson, JsonConverterSettings.Singleton);
            Assert.IsType<SlackWebSocketTransmissionSlashCommand>(wsTransmission);

            string helloJson = "{\"type\": \"hello\", \"num_connections\": 2}";
            var hello = JsonSerializer.Deserialize<ISlackWebSocketBase>(helloJson, JsonConverterSettings.Singleton);
            Assert.IsType<SlackWebSocketTransmissionHello>(hello);
            Assert.Equal(2, ((SlackWebSocketTransmissionHello)hello!).NumConnections);
        }

        [Fact]
        public void SlackPayload_SlashCommand_Deserialization()
        {
            string payloadJson = "{\"type\": \"slash_commands\", \"command\": \"/report\", \"text\": \"weekly status\"}";
            var payload = JsonSerializer.Deserialize<ISlackPayload>(payloadJson, JsonConverterSettings.Singleton);
            Assert.IsType<SlackSlashCommandPayload>(payload);
            var slashPayload = (SlackSlashCommandPayload)payload!;
            Assert.Equal("/report", slashPayload.Command);
            Assert.Equal("weekly status", slashPayload.Text);
        }

    }

}
