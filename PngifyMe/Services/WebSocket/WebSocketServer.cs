using NetCoreServer;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Serilog;
using System.Linq;
using PngifyMe.Layers;
using System.Text.Json;
using System;
using PngifyMe.Services.Settings;
using PngifyMe.Services.CharacterSetup.Basic;

namespace PngifyMe.Services.WebSocket;

public static class WebSocketServer
{
    public static Server Server { get; private set; }

    public static void Start()
    {
        Server = new Server(IPAddress.Loopback, 7666);
        Server.Start();
    }
}

/// <summary>
/// https://github.com/chronoxor/NetCoreServer?tab=readme-ov-file#example-websocket-chat-server
/// </summary>
public class Server : WsServer
{
    public Server(IPAddress address, int port) : base(address, port)
    {
        LayerManager.CharacterStateHandler.StateChanged += MicStateChanged;
        LayerManager.LayerTriggered += LayerTriggered;
    }

    private void LayerTriggered(object? sender, Layersetting e)
    {
        MulticastText(JsonSerializer.Serialize(new WebSocketStatus()
        {
            Type = "LayerTrigger",
            Status = e.Name,
        }));
    }

    private void MicStateChanged(object? sender, CharacterState e)
    {
        MulticastText(JsonSerializer.Serialize(new WebSocketStatus()
        {
            Type = "MicState",
            Status = e.Name,
        }));
    }

    protected override TcpSession CreateSession() { return new Session(this); }

    protected override void OnError(SocketError error)
    {
        Log.Debug($"WebSocket server caught an error with code {error}");
    }
}

public class Session : WsSession
{
    public Session(WsServer server) : base(server) { }

    public override void OnWsConnected(HttpRequest request)
    {
        Log.Debug($"WebSocket session with Id {Id} connected!");
    }

    public override void OnWsDisconnected()
    {
        Log.Debug($"WebSocket session with Id {Id} disconnected!");
    }

    public override void OnWsReceived(byte[] buffer, long offset, long size)
    {
        string message = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
        Log.Debug("Incoming: " + message);
        try
        {
            var data = JsonSerializer.Deserialize<WebsocketCommand>(message);
            switch (data?.Command)
            {
                case "SwitchLayer":
                    //check layer name, call this if match:
                    SettingsManager.Current.LayerSetup.Layers
                        .FirstOrDefault(x => string.Equals(x.Name, data.Parameter, StringComparison.OrdinalIgnoreCase))?.AddLayers();
                    break;
                case "SwitchState":
                    //check mic states, add those if match:
                    var match = SettingsManager.Current.Profile.Active.CharacterSetup.States
                                .FirstOrDefault(x => string.Equals(x.Name, data.Parameter, StringComparison.OrdinalIgnoreCase));
                    if (match != null)
                        LayerManager.CharacterStateHandler.ToggleState(match);
                    break;
                default:
                    break;
            }
        }
        catch (Exception e)
        {
            Log.Error(e, "Websocket error");
        }
    }

    protected override void OnError(SocketError error)
    {
        Log.Debug($"WebSocket session caught an error with code {error}");
    }
}

public class WebsocketCommand
{
    public string Command { get; set; } = "SwitchLayer";
    public string Parameter { get; set; }
}

public class WebSocketStatus
{
    public string Type { get; set; } = "SwitchLayer";
    public string Status { get; set; }
}
