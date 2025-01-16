using BarRaider.SdTools;
using NetCoreServer;
using System;
using System.Net.Sockets;
using System.Threading;

namespace me.Pngify.Streamdeck
{
    /// <summary>
    /// https://github.com/chronoxor/NetCoreServer?tab=readme-ov-file#example-websocket-chat-client
    /// </summary>
    public static class WSClient
    {
        private static Websocket client;

        public static void Start()
        {
            string address = "127.0.0.1";           
            int port = 7666;           

            // Create a new TCP chat client
            client = new Websocket(address, port);

            // Connect the client
            client.Connect();
        }

        public static void Stop()
        {
            client.DisconnectAndStop();
        }

        public static void Send(string input)
        {
            client.SendTextAsync(input);
        }
    }

    class Websocket : WsClient
    {
        public Websocket(string address, int port) : base(address, port) { }

        public void DisconnectAndStop()
        {
            _stop = true;
            CloseAsync(1000);
            while (IsConnected)
                Thread.Yield();
        }

        public override void OnWsConnecting(HttpRequest request)
        {
            request.SetBegin("GET", "/");
            request.SetHeader("Host", "localhost");
            request.SetHeader("Origin", "http://localhost");
            request.SetHeader("Upgrade", "websocket");
            request.SetHeader("Connection", "Upgrade");
            request.SetHeader("Sec-WebSocket-Key", Convert.ToBase64String(WsNonce));
            request.SetHeader("Sec-WebSocket-Version", "13");
            request.SetBody();
        }

        public override void OnWsConnected(HttpResponse response)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"WebSocket client connected a new session with Id {Id}");
        }

        public override void OnWsDisconnected()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"WebSocket client disconnected a session with Id {Id}");
        }

        public override void OnWsReceived(byte[] buffer, long offset, long size)
        {
            //Console.WriteLine($"Incoming: {Encoding.UTF8.GetString(buffer, (int)offset, (int)size)}");
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();

            Logger.Instance.LogMessage(TracingLevel.INFO, $"Chat WebSocket client disconnected a session with Id {Id}");

            // Wait for a while...
            Thread.Sleep(1000);

            // Try to connect again
            if (!_stop)
                ConnectAsync();
        }

        protected override void OnError(SocketError error)
        {
            Logger.Instance.LogMessage(TracingLevel.ERROR, $"Chat WebSocket client caught an error with code {error}");
        }

        private bool _stop;
    }

    public class WebsocketCommand
    {
        public string Command { get; set; } = "SwitchLayer";
        public string Parameter { get; set; }
    }

}