using Blacus.SlackAPIWrapper;
using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Blacus.SlackAPIWrapper.Tests
{

    public class SlackTransmissionHandlerTests
    {

        private class MockHttpContextAccessor : IHttpContextAccessor
        {
            public HttpContext? HttpContext { get; set; }
        }

        private class MockSlackASPTransmission : ISlackASPTransmission
        {
            public bool PrepareAsyncCalled { get; private set; }
            public ISlackPayload? Payload { get; set; }
            public SlackCredential DefaultSlackCredential { get; set; } = new();
            public bool Verified { get; set; }
            public bool ShortCircuited { get; set; }

            public Task PrepareAsync()
            {
                PrepareAsyncCalled = true;
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task HandleAsync_NullTransmission_ThrowsArgumentNullException()
        {
            await Assert.ThrowsAsync<ArgumentNullException>("transmission", () =>
                SlackTransmissionHandler.HandleAsync(null!, _ => Task.FromResult<string?>("ok")));
        }

        [Fact]
        public async Task HandleAsync_NullHandler_ThrowsArgumentNullException()
        {
            var transmission = new MockSlackASPTransmission();
            await Assert.ThrowsAsync<ArgumentNullException>("handler", () =>
                SlackTransmissionHandler.HandleAsync(transmission, null!));
        }

        [Fact]
        public async Task HandleAsync_CallsPrepareAsyncAndReturnsHandlerResult()
        {
            var transmission = new MockSlackASPTransmission();
            bool wasPreparedWhenHandlerRan = false;

            var result = await SlackTransmissionHandler.HandleAsync(transmission, t =>
            {
                wasPreparedWhenHandlerRan = ((MockSlackASPTransmission)t).PrepareAsyncCalled;
                return Task.FromResult<string?>("custom_response");
            });

            Assert.True(transmission.PrepareAsyncCalled);
            Assert.True(wasPreparedWhenHandlerRan);
            Assert.Equal("custom_response", result);
        }

        [Fact]
        public async Task HandleAsync_WithSlackASPTransmission_PreparesPayloadBeforeHandler()
        {
            string contentType = "application/x-www-form-urlencoded; charset=UTF-8";
            string body = "command=%2Fhello&text=world";

            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = contentType;
            httpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));

            var accessor = new MockHttpContextAccessor { HttpContext = httpContext };
            var credential = new SlackCredential { SigningSecret = "secret" };
            var transmission = new SlackTransmission(accessor, credential);

            string? handlerResult = await SlackTransmissionHandler.HandleAsync(transmission, async t =>
            {
                // If PrepareAsync was not called, t.Payload throws InvalidOperationException
                Assert.NotNull(t.Payload);
                Assert.IsType<SlackSlashCommandPayload>(t.Payload);
                var slashCommand = (SlackSlashCommandPayload)t.Payload;
                await Task.Yield();
                return $"Processed {slashCommand.Command} with {slashCommand.Text}";
            });

            Assert.Equal("Processed /hello with world", handlerResult);
        }

    }

}
