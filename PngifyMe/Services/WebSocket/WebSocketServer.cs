﻿using NetCoreServer;
using System.Net.Sockets;
using System.Net;
using System.Text;
using Serilog;
using System.Linq;
using PngifyMe.Layers;
using System.Text.Json;
using System;

namespace PngifyMe.Services.WebSocket;

public static class WebSocketServer
{
    public static Server Server { get; private set; }

    public static void Start()
    {
        Server = new Server(IPAddress.Any, 6666);
    }
}

/// <summary>
/// https://github.com/chronoxor/NetCoreServer?tab=readme-ov-file#example-websocket-chat-server
/// </summary>
public class Server : WsServer
{
    public Server(IPAddress address, int port) : base(address, port) { }

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
        var data = JsonSerializer.Deserialize<WebsocketCommand>(message);
        switch (data.Command)
        {
            case "SwitchLayer":
                //check layer name, call this if match:
                SettingsManager.Current.LayerSetup.Layers
                    .FirstOrDefault(x => string.Equals(x.Name, data.Parameter, StringComparison.OrdinalIgnoreCase))?.AddLayers();
                break;
            case "SwitchState":
                //check mic states, add those if match:
                var match = SettingsManager.Current.Profile.Active.MicroPhone.States
                            .FirstOrDefault(x => string.Equals(x.Name, data.Parameter, StringComparison.OrdinalIgnoreCase));
                if (match != null)
                    LayerManager.MicroPhoneStateLayer.ToggleState(match);
                break;
            default:
                break;
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
