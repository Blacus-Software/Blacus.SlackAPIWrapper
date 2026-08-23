using Blacus.SlackAPIWrapper.Models;
using Blacus.SlackAPIWrapper.Models.WebSocket;
using System;
using System.IO;
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

    }

}
