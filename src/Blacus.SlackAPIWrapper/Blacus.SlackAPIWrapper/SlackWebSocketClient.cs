using Blacus.SlackAPIWrapper.Methods;
using Blacus.SlackAPIWrapper.Models;
using Blacus.SlackAPIWrapper.Models.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper;

public sealed class SlackWebSocketClient(ILogger logger) : IAsyncDisposable, IDisposable
{

    private readonly SemaphoreSlim _sendLock = new(1, 1);
    private readonly SemaphoreSlim _lifecycleLock = new(1, 1);

    private ClientWebSocket? _webSocket;
    private CancellationTokenSource? _connectionCts;
    private Task? _receiveLoopTask;
    private Channel<string>? _rawChannel;
    private bool _disposed;

    public int ReceiveBufferSize { get; init; } = 8192;
    public bool IsConnected => _webSocket?.State == WebSocketState.Open;

    /// <summary>
    /// Optional callback for raw JSON strings received from Slack.
    /// </summary>
    public Action<string>? OnRawMessageReceived { get; set; }

    /// <summary>
    /// Optional callback for deserialized Slack models.
    /// </summary>
    public Action<ISlackWebSocketBase>? OnMessageReceived { get; set; }

    /// <summary>
    /// Connects to Slack Socket Mode. Safe to call again to reconnect.
    /// </summary>
    public async Task ConnectAsync(string appToken, CancellationToken cancellationToken = default)
    {

        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentException.ThrowIfNullOrWhiteSpace(appToken);

        await _lifecycleLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        
        try
        {

            await DisconnectInternalAsync().ConfigureAwait(false);

            var openResponse = await AppsConnectionsOpen.ExecuteAsync(appToken).ConfigureAwait(false);
            if (!openResponse.Ok || string.IsNullOrWhiteSpace(openResponse.Url))
            {
                var error = openResponse.Error ?? "Failed to obtain WebSocket connection URL.";
                logger.LogError("AppsConnectionsOpen failed: {Error}", error);
                throw new InvalidOperationException($"Failed to open WebSocket connection to Slack: {error}");
            }

            logger.LogTrace("Connecting to Slack WebSocket at {Url}", openResponse.Url);

            _connectionCts = new CancellationTokenSource();
            _webSocket = new ClientWebSocket();
            _rawChannel = System.Threading.Channels.Channel.CreateUnbounded<string>(new UnboundedChannelOptions
            {
                SingleWriter = true,
                SingleReader = false // Allows multiple consumers if needed
            });

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _connectionCts.Token);
            await _webSocket.ConnectAsync(new Uri(openResponse.Url), linkedCts.Token).ConfigureAwait(false);

            // Capture local references so the background task never races with reconnect/dispose
            var ws = _webSocket;
            var writer = _rawChannel.Writer;
            var token = _connectionCts.Token;

            _receiveLoopTask = Task.Run(() => ReceiveLoopAsync(ws, writer, token), CancellationToken.None);

        }
        finally
        {
            _lifecycleLock.Release();
        }

    }

    /// <summary>
    /// Streams raw JSON transmissions from the WebSocket as an IAsyncEnumerable&lt;string&gt;.
    /// </summary>
    public async IAsyncEnumerable<string> ReadRawMessagesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var channel = _rawChannel ?? throw new InvalidOperationException("Call ConnectAsync before reading messages.");

        await foreach (var json in channel.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        {
            yield return json;
        }
    }

    /// <summary>
    /// Streams deserialized Slack models as an IAsyncEnumerable&lt;ISlackWebSocketBase&gt;.
    /// </summary>
    public async IAsyncEnumerable<ISlackWebSocketBase> ReadMessagesAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var json in ReadRawMessagesAsync(cancellationToken).ConfigureAwait(false))
        {
            var model = DeserializeAndBind(json);
            if (model is not null and not SlackWebSocketTransmissionHello)
            {
                yield return model;
            }
        }
    }

    /// <summary>
    /// Thread-safe method to send a UTF-8 text payload over the active WebSocket.
    /// </summary>
    public async ValueTask SendMessageAsync(string text, CancellationToken cancellationToken = default)
    {

        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(text);

        var ws = _webSocket;
        var cts = _connectionCts;
        if (ws is not { State: WebSocketState.Open } || cts is null)
            return;

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, cts.Token);

        int maxByteCount = Encoding.UTF8.GetMaxByteCount(text.Length);
        byte[] rentedBuffer = ArrayPool<byte>.Shared.Rent(maxByteCount);

        try
        {
            int actualBytes = Encoding.UTF8.GetBytes(text, rentedBuffer);

            await _sendLock.WaitAsync(linkedCts.Token).ConfigureAwait(false);
            try
            {
                if (ws.State == WebSocketState.Open)
                {
                    await ws.SendAsync(
                        rentedBuffer.AsMemory(0, actualBytes),
                        WebSocketMessageType.Text,
                        endOfMessage: true,
                        linkedCts.Token).ConfigureAwait(false);
                }
            }
            finally
            {
                _sendLock.Release();
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation on send during shutdown
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rentedBuffer);
        }

    }

    private async Task ReceiveLoopAsync(ClientWebSocket ws, ChannelWriter<string> writer, CancellationToken token)
    {

        byte[] buffer = ArrayPool<byte>.Shared.Rent(ReceiveBufferSize);
        using var messageBuffer = new MemoryStream(ReceiveBufferSize);

        try
        {
            while (!token.IsCancellationRequested && ws.State == WebSocketState.Open)
            {

                messageBuffer.SetLength(0);
                ValueWebSocketReceiveResult result;

                do
                {
                    result = await ws.ReceiveAsync(buffer.AsMemory(), token).ConfigureAwait(false);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        logger.LogInformation("Slack server initiated WebSocket close.");
                        writer.TryComplete();
                        return;
                    }

                    messageBuffer.Write(buffer, 0, result.Count);
                }
                while (!result.EndOfMessage);

                if (result.MessageType != WebSocketMessageType.Text)
                    continue;

                // Zero-copy UTF-8 decode directly from MemoryStream's underlying buffer
                if (!messageBuffer.TryGetBuffer(out ArraySegment<byte> segment))
                    continue;

                string json = Encoding.UTF8.GetString(segment.AsSpan());
                await ProcessIncomingJsonAsync(json, writer, token).ConfigureAwait(false);

            }

            writer.TryComplete();

        }
        catch (OperationCanceledException)
        {
            writer.TryComplete();
        }
        catch (WebSocketException ex)
        {
            logger.LogError(ex, "WebSocket connection dropped unexpectedly.");
            writer.TryComplete(ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error in WebSocket receive loop.");
            writer.TryComplete(ex);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }

    }

    private async ValueTask ProcessIncomingJsonAsync(string json, ChannelWriter<string> writer, CancellationToken token)
    {

        try
        {
            var model = DeserializeAndBind(json);

            if (model is SlackWebSocketTransmissionHello)
            {
                logger.LogInformation("Slack WebSocket Hello received.");
            }
            else if (model is not null)
            {
                // Always ACK Slack envelopes immediately so slow consumers never cause 3s retries
                if (model.EnvelopeId != Guid.Empty)
                {
                    string ackJson = model.AcknowledgePayload.ToJson();
                    await SendMessageAsync(ackJson, token).ConfigureAwait(false);
                }

                // Invoke typed Action callback if subscribed
                OnMessageReceived?.Invoke(model);
            }

            // Invoke raw string Action callback if subscribed
            OnRawMessageReceived?.Invoke(json);

            // Push raw string to IAsyncEnumerable<string> channel only if someone might read it,
            // or keep unbounded buffer ready. Note: if ONLY using Actions, consider setting EnableStreaming = false
            // if running indefinitely to avoid buffering unread strings in memory.
            if (_rawChannel is not null)
            {
                writer.TryWrite(json);
            }

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process incoming Slack WebSocket transmission.");
        }

    }

    private ISlackWebSocketBase? DeserializeAndBind(string json)
    {
        
        var model = ISlackModel.FromJson<ISlackWebSocketBase>(json);
        
        if (model is null)
            return null;

        model.Client = this;
        model.AcknowledgePayload = new() { EnvelopeId = model.EnvelopeId };
        
        return model;

    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (_disposed) return;

        await _lifecycleLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await DisconnectInternalAsync().ConfigureAwait(false);
        }
        finally
        {
            _lifecycleLock.Release();
        }
    }

    private async Task DisconnectInternalAsync()
    {

        var ws = _webSocket;
        var cts = _connectionCts;
        var loopTask = _receiveLoopTask;
        var channel = _rawChannel;

        _webSocket = null;
        _connectionCts = null;
        _receiveLoopTask = null;
        _rawChannel = null;

        channel?.Writer.TryComplete();

        if (ws is not null)
        {
            try
            {
                if (ws.State is WebSocketState.Open or WebSocketState.CloseReceived)
                {
                    using var closeTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
                    await _sendLock.WaitAsync(closeTimeout.Token).ConfigureAwait(false);
                    try
                    {
                        await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Disconnecting", closeTimeout.Token).ConfigureAwait(false);
                    }
                    finally
                    {
                        _sendLock.Release();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogTrace(ex, "Non-fatal exception while closing WebSocket.");
            }
        }

        if (cts is not null)
        {
            await cts.CancelAsync().ConfigureAwait(false);
        }

        if (loopTask is not null)
        {
            try
            {
                await loopTask.ConfigureAwait(false);
            }
            catch
            {
                // Background loop exceptions are already logged and forwarded to channel
            }
        }

        ws?.Dispose();
        cts?.Dispose();
        logger.LogInformation("Slack WebSocket disconnected.");

    }

    public async ValueTask DisposeAsync()
    {

        if (_disposed) return;
        _disposed = true;

        await _lifecycleLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await DisconnectInternalAsync().ConfigureAwait(false);
        }
        finally
        {
            _lifecycleLock.Release();
        }

        _sendLock.Dispose();
        _lifecycleLock.Dispose();

    }

    public void Dispose()
    {
        
        if (_disposed) return;
        _disposed = true;

        _rawChannel?.Writer.TryComplete();
        _connectionCts?.Cancel();
        _webSocket?.Dispose();
        _connectionCts?.Dispose();
        _sendLock.Dispose();
        _lifecycleLock.Dispose();

    }

}