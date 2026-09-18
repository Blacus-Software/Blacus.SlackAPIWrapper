using Blacus.SlackAPIWrapper.Methods;
using Blacus.SlackAPIWrapper.Models;
using Blacus.SlackAPIWrapper.Models.WebSocket;
using Microsoft.Extensions.Logging;
using System;
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
                CTS?.Cancel();
                WS?.Dispose();
                CTS?.Dispose();
                _sendLock.Dispose();
                WS = null;
                CTS = null;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error during dispose: {ex.Message}");
            }
        }

        public async ValueTask DisposeAsync()
        {
            await DisconnectAsync().ConfigureAwait(false);
        }

        private ClientWebSocket? WS;
        private CancellationTokenSource? CTS;
        private readonly SemaphoreSlim _sendLock = new(1, 1);

        public required Action<ISlackWebSocketBase> OnMessageReceived { get; set; }
        private ILogger logger { get; }

        public async Task SendMessageAsync(string text)
        {
            if (WS == null || WS.State != WebSocketState.Open || CTS == null)
                return;

            byte[] bytes = Encoding.UTF8.GetBytes(text);
            try
            {
                await _sendLock.WaitAsync(CTS.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            try
            {
                if (WS != null && WS.State == WebSocketState.Open && CTS != null)
                {
                    await WS.SendAsync(bytes, WebSocketMessageType.Text, true, CTS.Token).ConfigureAwait(false);
                }
            }
            finally
            {
                _sendLock.Release();
            }
        }

        private Task? taskReceiveLoop;

        public SlackWebSocketClient(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ConnectAsync(string appToken)
        {

            if (string.IsNullOrWhiteSpace(appToken))
                throw new ArgumentNullException(nameof(appToken));

            var openResponse = await AppsConnectionsOpen.ExecuteAsync(appToken).ConfigureAwait(false);

            if (!openResponse.Ok || string.IsNullOrEmpty(openResponse.Url))
            {
                string error = openResponse.Error ?? "Failed to obtain WebSocket connection URL.";
                logger.LogError($"AppsConnectionsOpen failed: {error}");
                throw new InvalidOperationException($"Failed to open WebSocket connection to Slack: {error}");
            }

            var url = openResponse.Url;

            logger.LogTrace($"Connecting to WebSocket at {url}");

            if (WS != null)
                WS.Dispose();

            WS = new ClientWebSocket();

            if (CTS != null)
                CTS.Dispose();

            CTS = new CancellationTokenSource();

            await WS.ConnectAsync(new Uri(url), CTS.Token).ConfigureAwait(false);

            taskReceiveLoop = Task.Run(ReceiveLoop, CTS.Token);

        }

        public async Task DisconnectAsync()
        {

            if (WS is null)
                return;

            var ws = WS;
            var cts = CTS;
            WS = null;
            CTS = null;

            try
            {
                if (ws.State == WebSocketState.Open || ws.State == WebSocketState.CloseReceived)
                {
                    using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                    try
                    {
                        await _sendLock.WaitAsync(timeoutCts.Token).ConfigureAwait(false);
                        try
                        {
                            await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Client disconnecting", timeoutCts.Token).ConfigureAwait(false);
                        }
                        finally
                        {
                            _sendLock.Release();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Send lock timed out during disconnect
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogTrace($"Exception during WebSocket close: {ex.Message}");
            }
            finally
            {
                cts?.Cancel();
                ws?.Dispose();
                cts?.Dispose();
            }

            if (taskReceiveLoop != null)
            {
                try
                {
                    await taskReceiveLoop.ConfigureAwait(false);
                }
                catch
                {
                    // Ignore background cancellation during disconnect
                }
            }

            logger.LogInformation($"WebSocket Disconnected");

        }

        private async Task ReceiveLoop()
        {

            if (CTS == null || WS == null)
                return;

            var loopToken = CTS.Token;
            WebSocketReceiveResult? receiveResult = null;

            var buffer = new byte[ReceiveBufferSize];

            try
            {
                while (!loopToken.IsCancellationRequested && WS != null && WS.State == WebSocketState.Open)
                {

                    using (var outputStream = new MemoryStream(ReceiveBufferSize))
                    {

                        do
                        {

                            receiveResult = await WS.ReceiveAsync(buffer, loopToken).ConfigureAwait(false);

                            if (receiveResult.MessageType != WebSocketMessageType.Close)
                                outputStream.Write(buffer, 0, receiveResult.Count);

                        }
                        while (!receiveResult.EndOfMessage);

                        if (receiveResult.MessageType == WebSocketMessageType.Close)
                            break;

                        outputStream.Position = 0;

                        await ResponseReceived(outputStream).ConfigureAwait(false);

                    }

                }

                logger.LogInformation("Socket Closed");

            }
            catch (OperationCanceledException) { }
            catch (WebSocketException ex)
            {
                logger.LogError($"WebSocket error: {ex.Message}");
                await DisconnectAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unexpected error in WebSocket receive loop: {ex.Message}");
                await DisconnectAsync().ConfigureAwait(false);
            }

        } 

        private async Task ResponseReceived(Stream inputStream)
        {

            ISlackWebSocketBase? slackWebSocketTransmission;
            string json;

            try
            {

                using var reader = new StreamReader(inputStream);
                json = await reader.ReadToEndAsync();

                // ISlackWebSocketBase? slackWebSocketTransmission = ISlackModel.FromJson<ISlackWebSocketBase>(inputStream);
                slackWebSocketTransmission = ISlackModel.FromJson<ISlackWebSocketBase>(json);

                if (slackWebSocketTransmission == null)
                    return;

                slackWebSocketTransmission.Client = this;
                slackWebSocketTransmission.AcknowledgePayload = new() { EnvelopeId = slackWebSocketTransmission.EnvelopeId };

                switch (slackWebSocketTransmission)
                {
                    case SlackWebSocketTransmissionHello:
                        logger.LogInformation($"WebSocket Hello");
                        break;

                    default:
                        OnMessageReceived?.Invoke(slackWebSocketTransmission);
                        string ackPayload = slackWebSocketTransmission.AcknowledgePayload.ToJson();
                        await SendMessageAsync(ackPayload).ConfigureAwait(false);
                        break;

                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Failed to process received WebSocket message: {ex.Message}");
            }

        }

    }

}
