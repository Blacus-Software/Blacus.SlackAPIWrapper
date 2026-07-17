using Blacus.SlackAPIWrapper.Methods;
using Blacus.SlackAPIWrapper.Models;
using Blacus.SlackAPIWrapper.Models.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper
{
    public class SlackWebSocketClient : IDisposable, IAsyncDisposable
    {

        public int ReceiveBufferSize { get; } = 8192;

        public void Dispose()
        {
            try
            {
                DisconnectAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                logger.LogError($"Error during dispose: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync();
        }

        private ClientWebSocket WS;
        private CancellationTokenSource CTS;

        public required Action<ISlackWebSocketBase> OnMessageReceived { get; set; }
        private ILogger logger { get; }

        public async Task SendMessageAsync(string text) =>
            await this.WS.SendAsync(Encoding.UTF8.GetBytes(text), WebSocketMessageType.Text, true, CTS.Token);

        private Task taskReceiveLoop;

        public SlackWebSocketClient(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task ConnectAsync(string appToken)
        {

            var openResponse = await AppsConnectionsOpen.ExecuteAsync(appToken);
            var url = openResponse.Url;

            logger.LogTrace($"Connecting to WebSocket at {url}");

            if (WS != null)
                WS.Dispose();

            WS = new ClientWebSocket();

            if (CTS != null)
                CTS.Dispose();

            CTS = new CancellationTokenSource();

            await WS.ConnectAsync(new Uri(url), CTS.Token);

            taskReceiveLoop = Task.Run(ReceiveLoop, CTS.Token);

        }

        public async Task DisconnectAsync()
        {

            if (WS is null)
                return;

            if (WS.State == WebSocketState.Open)
            {

                CTS.CancelAfter(TimeSpan.FromSeconds(2));
                await WS.CloseOutputAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
                await WS.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);

            }

            WS.Dispose();
            WS = null;
            CTS.Dispose();
            CTS = null;

            logger.LogInformation($"WebSocket Disconnected");

        }

        private async Task ReceiveLoop()
        {

            var loopToken = CTS.Token;
            WebSocketReceiveResult receiveResult = null;

            var buffer = new byte[ReceiveBufferSize];

            try
            {
                while (!loopToken.IsCancellationRequested)
                {

                    using (var outputStream = new MemoryStream(ReceiveBufferSize))
                    {

                        do
                        {

                            receiveResult = await WS.ReceiveAsync(buffer, CTS.Token);

                            if (receiveResult.MessageType != WebSocketMessageType.Close)
                                outputStream.Write(buffer, 0, receiveResult.Count);

                        }
                        while (!receiveResult.EndOfMessage);

                        if (receiveResult.MessageType == WebSocketMessageType.Close)
                            break;

                        outputStream.Position = 0;

                        await ResponseReceived(outputStream);

                    }

                }
            }
            catch (TaskCanceledException) { }
            catch (WebSocketException ex)
            {
                logger.LogError($"WebSocket error: {ex.Message}");
                await DisconnectAsync();
            }

        } 

        private async Task ResponseReceived(Stream inputStream)
        {

            bool stream = true;
            ISlackWebSocketBase slackWebSocketTransmission = null; 

            if (stream)
            {
                slackWebSocketTransmission = ISlackModel.FromJson<ISlackWebSocketBase>(inputStream);
            }
            else
            {
                string json = null;
                try
                {
                    using (StreamReader reader = new StreamReader(inputStream, Encoding.UTF8))
                    {
                        json = await reader.ReadToEndAsync();
                        slackWebSocketTransmission = ISlackModel.FromJson<ISlackWebSocketBase>(json);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }

            slackWebSocketTransmission.Client = this;
            slackWebSocketTransmission.AcknowledgePayload = new() { EnvelopeId = slackWebSocketTransmission.EnvelopeId, };

            switch (slackWebSocketTransmission)
            {
                case SlackWebSocketTransmissionHello:
                    logger.LogInformation($"WebSocket Hello");
                    break;

                default:
                    OnMessageReceived(slackWebSocketTransmission);
                    string ackPayload = slackWebSocketTransmission.AcknowledgePayload.ToJson();
                    await SendMessageAsync(ackPayload);
                    break;

            }

        }

    }

}
