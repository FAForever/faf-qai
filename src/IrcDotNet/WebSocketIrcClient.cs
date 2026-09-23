using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace IrcDotNet
{
    /// <summary>
    ///     Represents an IRC client that talks to the server over a WebSocket connection, as served by the
    ///     websocket listener of modern ircds such as Ergo, instead of a raw TCP socket.
    /// </summary>
    /// <remarks>
    ///     Each IRC message is sent as its own text frame. Received frames may contain one or more lines.
    /// </remarks>
    public class WebSocketIrcClient : IrcClient
    {
        // The subprotocol used by ircds that serve IRC over websockets; servers that don't know it simply
        // don't negotiate one, in which case text frames are used anyway.
        private const string TextSubProtocol = "text.ircv3.net";

        // Minimum duration of time to wait between sending successive raw messages.
        private const long minimumSendWaitTime = 50;

        private const int receiveBufferSize = 0xFFFF;

        // Give up on a handshake that never completes, so that a server which accepts the connection
        // but never upgrades it doesn't leave us connecting forever.
        private static readonly TimeSpan connectTimeout = TimeSpan.FromSeconds(30);

        // Drop the connection when the server stops answering the WebSocket keep-alive pings. Without
        // this the socket keeps looking open on a network path that silently went away.
        private static readonly TimeSpan keepAliveTimeout = TimeSpan.FromSeconds(20);

        // Queue of pending messages and their tokens to be sent when ready. Messages are enqueued from
        // whichever thread sends them (including the receive loop, which answers PING and CAP) while the
        // send timer dequeues them, so this has to be thread-safe.
        private readonly ConcurrentQueue<Tuple<string, object>> messageSendQueue = new();

        private readonly SemaphoreSlim sendLock = new(1, 1);

        private readonly ManualResetEventSlim disconnectedEvent = new(false);

        // Set once disconnection has been handled, so that it is only ever signalled once per connection.
        private int disconnectedHandled;

        private Timer sendTimer;

        private ClientWebSocket webSocket;
        private CancellationTokenSource cancellationTokenSource;
        private Uri serverUri;

        public WebSocketIrcClient()
        {
            sendTimer = new Timer(WritePendingMessages, null, Timeout.Infinite, Timeout.Infinite);
        }

        public override bool IsConnected
        {
            get
            {
                CheckDisposed();
                return webSocket != null && webSocket.State == WebSocketState.Open;
            }
        }

        /// <summary>
        ///     Connects asynchronously to the specified server.
        /// </summary>
        /// <param name="url">The <c>ws</c> or <c>wss</c> URL of the server to which to connect.</param>
        /// <param name="registrationInfo">
        ///     The information used for registering the client.
        ///     The type of the object may be either <see cref="IrcUserRegistrationInfo" /> or
        ///     <see cref="IrcServiceRegistrationInfo" />.
        /// </param>
        public void Connect(Uri url, IrcRegistrationInfo registrationInfo)
        {
            CheckDisposed();

            ArgumentNullException.ThrowIfNull(url);
            ArgumentNullException.ThrowIfNull(registrationInfo);

            if (url.Scheme != "ws" && url.Scheme != "wss")
                throw new ArgumentException($"The URL scheme '{url.Scheme}' is not a WebSocket scheme.", nameof(url));

            // Validates the registration info and resets the state of the client.
            Connect(registrationInfo);

            serverUri = url;

            HandleClientConnecting();

            _ = ConnectAsync(url, registrationInfo);
        }

        /// <inheritdoc cref="Connect(Uri, IrcRegistrationInfo)" />
        public void Connect(string url, IrcRegistrationInfo registrationInfo)
        {
            Connect(new Uri(url), registrationInfo);
        }

        public override void Disconnect()
        {
            base.Disconnect();

            _ = DisconnectAsync();
        }

        public override void Quit(int timeout, string comment)
        {
            base.Quit(timeout, comment);
            if (!disconnectedEvent.Wait(timeout))
                Disconnect();
        }

        protected override void WriteMessage(string line, object token)
        {
            // Add message line to send queue.
            messageSendQueue.Enqueue(Tuple.Create(line, token));
        }

        protected override void ResetState()
        {
            base.ResetState();

            disconnectedHandled = 0;
            disconnectedEvent.Reset();
        }

        protected override void HandleClientDisconnected()
        {
            // Ensure that client has not already handled disconnection.
            if (Interlocked.Exchange(ref disconnectedHandled, 1) != 0)
                return;

            DebugUtilities.WriteEvent("Disconnected from server.");

            // Stop sending messages immediately.
            sendTimer?.Change(Timeout.Infinite, Timeout.Infinite);

            // Make sure the transport is really gone. The receive loop may have died on its own, for
            // instance on a line the parser chokes on, and a socket left open would keep IsConnected
            // true so that nothing ever reconnects.
            try
            {
                webSocket?.Abort();
            }
            catch (ObjectDisposedException)
            {
                // Ignore.
            }

            // Set that client has disconnected.
            disconnectedEvent.Set();

            base.HandleClientDisconnected();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing)
                return;

            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
            if (webSocket != null)
            {
                webSocket.Dispose();
                webSocket = null;

                HandleClientDisconnected();
            }
            if (sendTimer != null)
            {
                sendTimer.Dispose();
                sendTimer = null;
            }
            disconnectedEvent.Dispose();
            sendLock.Dispose();
        }

        private async Task ConnectAsync(Uri url, IrcRegistrationInfo registrationInfo)
        {
            var cancellation = new CancellationTokenSource();
            cancellationTokenSource = cancellation;

            using var connectCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellation.Token);
            connectCancellation.CancelAfter(connectTimeout);

            try
            {
                var socket = new ClientWebSocket();
                socket.Options.AddSubProtocol(TextSubProtocol);
                socket.Options.KeepAliveTimeout = keepAliveTimeout;
                webSocket = socket;

                await socket.ConnectAsync(url, connectCancellation.Token);

                DebugUtilities.WriteEvent("Connected to server at '{0}'.", url);

                // Start sending and receiving data to/from server.
                sendTimer.Change(0, Timeout.Infinite);
                _ = Task.Run(() => ReceiveAsync(socket, cancellation.Token), CancellationToken.None);

                HandleClientConnectedNew(registrationInfo);
            }
            catch (Exception ex)
            {
                // The client was disposed or disconnected while connecting; nothing to report.
                if (cancellation.IsCancellationRequested)
                    return;

                OnConnectFailed(new IrcErrorEventArgs(connectCancellation.IsCancellationRequested
                    ? new TimeoutException($"Timed out connecting to '{url}'.")
                    : ex));
            }
        }

        private async Task ReceiveAsync(ClientWebSocket socket, CancellationToken cancellationToken)
        {
            var buffer = new byte[receiveBufferSize];
            // A single IRC message may be split across multiple websocket frames.
            var messageStream = new MemoryStream();

            try
            {
                while (socket.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null,
                            CancellationToken.None);
                        break;
                    }

                    messageStream.Write(buffer, 0, result.Count);

                    if (!result.EndOfMessage)
                        continue;

                    var message = TextEncoding.GetString(messageStream.GetBuffer(), 0, (int) messageStream.Length);
                    messageStream.SetLength(0);

                    // A frame usually holds a single message, but nothing stops a server from batching them.
                    foreach (var line in message.Split('\n'))
                    {
                        var trimmedLine = line.TrimEnd('\r');
                        if (trimmedLine.Length != 0)
                            ParseMessage(trimmedLine);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore.
            }
            catch (ObjectDisposedException)
            {
                // Ignore.
            }
            catch (WebSocketException ex)
            {
                // The server hung up, which is what it does after a QUIT; this is not an error.
                DebugUtilities.WriteEvent("WebSocket closed: {0}", ex.Message);
            }
            catch (Exception ex)
            {
                OnError(new IrcErrorEventArgs(ex));
            }
            finally
            {
                messageStream.Dispose();
                HandleClientDisconnected();
            }
        }

        private void WritePendingMessages(object state)
        {
            try
            {
                // Send pending messages in queue until flood preventer indicates to stop.
                long sendDelay = 0;

                while (!messageSendQueue.IsEmpty)
                {
                    // Check that flood preventer currently permits sending of messages.
                    if (FloodPreventer != null)
                    {
                        sendDelay = FloodPreventer.GetSendDelay();
                        if (sendDelay > 0)
                            break;
                    }

                    // Send next message in queue.
                    if (!messageSendQueue.TryDequeue(out var message))
                        break;

                    _ = SendAsync(message.Item1, message.Item2);

                    // Tell flood preventer mechanism that message has just been sent.
                    FloodPreventer?.HandleMessageSent();
                }

                // Make timer fire when next message in send queue should be written.
                sendTimer.Change((int) Math.Max(sendDelay, minimumSendWaitTime), Timeout.Infinite);
            }
            catch (ObjectDisposedException)
            {
                // Ignore.
            }
            catch (Exception ex)
            {
                OnError(new IrcErrorEventArgs(ex));
            }
        }

        private async Task SendAsync(string line, object token)
        {
            // Only a single send may be in flight on a websocket at any time.
            await sendLock.WaitAsync();
            try
            {
                var socket = webSocket;
                if (socket == null)
                    return;

                var cancellationToken = cancellationTokenSource?.Token ?? CancellationToken.None;
                var buffer = TextEncoding.GetBytes(line + "\r\n");
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true,
                    cancellationToken);

                if (token is IrcRawMessageEventArgs messageSentEventArgs)
                {
                    OnRawMessageSent(messageSentEventArgs);

                    DebugUtilities.WriteIrcRawLine(this, "<<< " + messageSentEventArgs.RawContent);
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore.
            }
            catch (ObjectDisposedException)
            {
                // Ignore.
            }
            catch (WebSocketException)
            {
                HandleClientDisconnected();
            }
            catch (Exception ex)
            {
                OnError(new IrcErrorEventArgs(ex));
            }
            finally
            {
                sendLock.Release();
            }
        }

        private async Task DisconnectAsync()
        {
            try
            {
                var socket = webSocket;
                if (socket is { State: WebSocketState.Open })
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
            catch (Exception)
            {
                // The connection is going away either way.
            }
            finally
            {
                cancellationTokenSource?.Cancel();
                HandleClientDisconnected();
            }
        }

        /// <summary>
        ///     Returns a string representation of this instance.
        /// </summary>
        /// <returns>A string that represents this instance.</returns>
        public override string ToString()
        {
            if (IsDisposed) return "(Disposed)";
            if (!IsConnected) return "(Not connected)";

            var username = LocalUser?.UserName ?? "Unknown";
            var serverInfo = ServerName ?? serverUri?.ToString() ?? "Unknown";

            return $"{username}@{serverInfo}";
        }
    }
}
