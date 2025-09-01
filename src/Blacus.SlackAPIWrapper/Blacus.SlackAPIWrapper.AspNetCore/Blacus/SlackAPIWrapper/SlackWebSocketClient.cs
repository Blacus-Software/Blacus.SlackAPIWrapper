using Blacus.SlackAPIWrapper.Methods;
using Blacus.SlackAPIWrapper.Models;
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

    public class SlackWebSocketClient : IDisposable
    {

        public int ReceiveBufferSize { get; } = 8192;

        public List<Task<SlackWebSocketTransmission>> Tasks = new();

        public void Dispose() => DisconnectAsync().Wait();

        private ClientWebSocket WS;
        private CancellationTokenSource CTS;

        public required Action<SlackWebSocketTransmission> OnMessageReceived { get; set; }
        private ILogger logger { get; }

        public async Task SendMessageAsync(string text) =>
            await this.WS.SendAsync(Encoding.UTF8.GetBytes(text), WebSocketMessageType.Text, true, CTS.Token);

        private Task taskCleanUp;
        private Task taskReceiveLoop;

        public SlackWebSocketClient(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task ConnectAsync(string appToken)
        {

            var openResponse = AppsConnectionsOpen.ExecuteAsync(appToken).Result;
            var url = openResponse.Url;

            logger.LogTrace($"Connecting to WebSocket at {url}");

            if (WS != null)
                WS.Dispose();

            WS = new ClientWebSocket();

            if (CTS != null)
                CTS.Dispose();

            CTS = new CancellationTokenSource();

            await WS.ConnectAsync(new Uri(url), CTS.Token);

            if (taskCleanUp?.Status != TaskStatus.Running)
                taskCleanUp = Task.Run(() => cleanUp());

            taskReceiveLoop = await Task.Factory.StartNew(ReceiveLoop, CTS.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            taskReceiveLoop.Wait();

        }

        private void cleanUp()
        {

            var loopToken = CTS.Token;

            while (!loopToken.IsCancellationRequested)
            {

                try
                {

                    if (Tasks.Count == 0)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {

                        var taskArray = Tasks.ToArray();

                        int taskId = Task.WaitAny(taskArray, new TimeSpan(0, 0, 1));

                        var task = taskArray[taskId];

                        lock (Tasks)
                        {
                            Tasks.Remove(task);
                        }

                        if (task.Exception != null)
                        {
                            foreach (var ex in task.Exception.InnerExceptions)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                        }

                    }

                }
                catch (Exception ex)
                {
                    logger.LogError($"WebSocket Error: {ex.Message}");
                }

            }

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

                    if (receiveResult.MessageType == WebSocketMessageType.Close)
                        break;

                    outputStream.Position = 0;

                    await ResponseReceived(outputStream);

                }
            }
            catch (TaskCanceledException) { }
            catch (WebSocketException ex)
            {
                logger.LogError($"WebSocket error: {ex.Message}");
                DisconnectAsync().Wait();
            }
            finally
            {
                outputStream?.Dispose();
            }

        }

        private async Task ResponseReceived(Stream inputStream)
        {

            bool stream = true;
            SlackWebSocketTransmission slackWebSocketTransmission = null;

            if (stream)
            {
                slackWebSocketTransmission = ISlackModel.FromJson<SlackWebSocketTransmission>(inputStream);
            }
            else
            {
                string json = null;
                try
                {
                    using (StreamReader reader = new StreamReader(inputStream, Encoding.UTF8))
                    {
                        json = await reader.ReadToEndAsync();
                        slackWebSocketTransmission = ISlackModel.FromJson<SlackWebSocketTransmission>(json);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            slackWebSocketTransmission.Client = this;
            slackWebSocketTransmission.AcknowledgePayload = new() { EnvelopeId = slackWebSocketTransmission.EnvelopeId, };

            switch (slackWebSocketTransmission.TypeString)
            {
                case "hello":
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
