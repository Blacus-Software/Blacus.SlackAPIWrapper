using Blacus.SlackAPIWrapper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper
{

    public class SlackWebSocketClient : IDisposable
    {

        public SlackCredential SlackCredential { get; }

        public int ReceiveBufferSize { get; set; } = 8192;

        public List<Task<SlackWebSocketPayload>> Tasks = new();

        public void Dispose() => DisconnectAsync().Wait();

        private ClientWebSocket WS;
        private CancellationTokenSource CTS;

        public SlackWebSocketClient(SlackCredential slackCredential)
        {
            SlackCredential = slackCredential;
        }

        private async Task SendMessageAsync(string text) =>
            await this.WS.SendAsync(Encoding.UTF8.GetBytes(text), WebSocketMessageType.Text, true, default);

        public async Task ConnectAsync(string url)
        {
            if (WS != null)
            {
                if (WS.State == WebSocketState.Open) return;
                else WS.Dispose();
            }
            WS = new ClientWebSocket();
            if (CTS != null) CTS.Dispose();
            CTS = new CancellationTokenSource();
            await WS.ConnectAsync(new Uri(url), CTS.Token);
            await Task.Factory.StartNew(ReceiveLoop, CTS.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        public async Task DisconnectAsync()
        {
            if (WS is null) return;
            // TODO: requests cleanup code, sub-protocol dependent.
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
        }

        private async Task ReceiveLoop()
        {
            var loopToken = CTS.Token;
            MemoryStream outputStream = null;
            WebSocketReceiveResult receiveResult = null;
            var buffer = new byte[ReceiveBufferSize];
            try
            {
                while (!loopToken.IsCancellationRequested)
                {
                    outputStream = new MemoryStream(ReceiveBufferSize);
                    do
                    {
                        receiveResult = await WS.ReceiveAsync(buffer, CTS.Token);
                        if (receiveResult.MessageType != WebSocketMessageType.Close)
                            outputStream.Write(buffer, 0, receiveResult.Count);
                    }
                    while (!receiveResult.EndOfMessage);
                    if (receiveResult.MessageType == WebSocketMessageType.Close) break;
                    outputStream.Position = 0;
                    await ResponseReceived(outputStream);
                }
            }
            catch (TaskCanceledException) { }
            finally
            {
                outputStream?.Dispose();
            }
        }

        private async Task ResponseReceived(Stream inputStream)
        {

            // convert stream to string
            StreamReader reader = new StreamReader(inputStream);
            string text = reader.ReadToEnd();

            lock (Tasks)
            {
                Tasks.Add(Task.Run(async () =>
                {

                    var toReturn = FromJson(text).Result;

                    switch (toReturn.Type)
                    {
                        case "hello":
                            break;

                        default:
                            await SendMessageAsync(new SlackWebSocketAcknowledge() { EnvelopeId = toReturn.EnvelopeId, }.ToJson());
                            Processor(toReturn);
                            break;

                    }

                    return toReturn;

                }
                ));
            }

        }

        public required Action<ISlackWebSocketPayload> Processor { get; set; }

        private static async Task<SlackWebSocketPayload> FromJson(string json)
        {

            SlackWebSocketPayload? toReturn = null;

            try
            {


                toReturn = System.Text.Json.JsonSerializer.Deserialize<SlackWebSocketPayload>(json, JsonConverterSettings.Singleton);

                toReturn.SourceJson = json;

            }
            catch (Exception ex)
            {

                throw;
            }

            return toReturn;

        }

    }

}
