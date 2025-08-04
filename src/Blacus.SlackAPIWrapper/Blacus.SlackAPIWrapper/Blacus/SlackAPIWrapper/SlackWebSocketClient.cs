using Blacus.SlackAPIWrapper.Methods;
using Blacus.SlackAPIWrapper.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable CS8603, CS8600, CS8602, CS1998, CS8625, CS8618, CS8604

namespace Blacus.SlackAPIWrapper
{

    public class SlackWebSocketClient : IDisposable
    {

        public int ReceiveBufferSize { get; set; } = 8192;

        public List<Task<SlackWebSocketPayload>> Tasks = new();

        public void Dispose() => DisconnectAsync().Wait();

        private ClientWebSocket WS;
        private CancellationTokenSource CTS;

        public required Action<ISlackWebSocketPayload> OnMessageReceived { get; set; }

        private async Task SendMessageAsync(string text) =>
            await this.WS.SendAsync(Encoding.UTF8.GetBytes(text), WebSocketMessageType.Text, true, CTS.Token);

        private Task taskCleanUp;
        private Task taskReceiveLoop;

        public async Task ConnectAsync(string appToken)
        {

            var openResponse = AppsConnectionsOpen.ExecuteAsync(appToken).Result;
            var url = openResponse.Url;

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
                    Console.WriteLine($"WebSocket Error: {ex.Message}");
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
                Console.WriteLine($"Websocket error: {ex.Message}");
                DisconnectAsync().Wait();
            }
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

                    var toReturn = SlackWebSocketPayload.FromJson(text);

                    switch (toReturn.TypeString)
                    {
                        case "hello":
                            break;

                        default:
                            await SendMessageAsync(new SlackWebSocketAcknowledge() { EnvelopeId = toReturn.EnvelopeId, }.ToJson());
                            OnMessageReceived(toReturn);
                            break;

                    }

                    return toReturn;

                }
                ));

            }

        }

    }

}
