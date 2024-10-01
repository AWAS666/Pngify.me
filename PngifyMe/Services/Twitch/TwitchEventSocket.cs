using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api.Core.Enums;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Stream;

namespace PngifyMe.Services.Twitch
{
    public static class TwitchEventSocket
    {
        public static EventHandler<string> RedeemUsed;
        public static EventHandler<ChannelPointsCustomRewardRedemption> RedeemFull;
        public static EventHandler<ChannelCheer> BitsUsed;
        public static EventHandler<string> NewFollower;
        public static EventHandler<TwitchAuth> Authenticated;
        public static EventHandler<ChannelChatMessage> NewChat;

        private static EventSubWebsocketClient _eventSubWebsocketClient;

        private static string ws = "wss://eventsub.wss.twitch.tv/ws";
        public static TwitchApi Api { get; private set; }

        public static async Task Start()
        {
            _eventSubWebsocketClient = new EventSubWebsocketClient();
            _eventSubWebsocketClient.WebsocketConnected += WebsocketConnected;
            _eventSubWebsocketClient.WebsocketDisconnected += OnWebsocketDisconnected;

            _eventSubWebsocketClient.ChannelFollow += OnChannelFollow;
            _eventSubWebsocketClient.StreamOnline += OnStreamOnline;
            _eventSubWebsocketClient.StreamOffline += OnStreamOffline;

            _eventSubWebsocketClient.ChannelRaid += OnChannelRaid;
            _eventSubWebsocketClient.ChannelHypeTrainBegin += HypeTrainStart;
            _eventSubWebsocketClient.ChannelHypeTrainEnd += HypeTrainEnd;

            _eventSubWebsocketClient.ChannelPointsCustomRewardRedemptionAdd += OnChannelPointsRedeemed;
            _eventSubWebsocketClient.ChannelCheer += Cheer;
            _eventSubWebsocketClient.ChannelChatMessage += ChatMessage;

            await _eventSubWebsocketClient.ConnectAsync(new Uri(ws));
        }

        private static async Task WebsocketConnected(object sender, TwitchLib.EventSub.Websockets.Core.EventArgs.WebsocketConnectedArgs args)
        {
            try
            {
                Api = new TwitchApi();
                await Api.Connect();

                await Api.Api.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.update", "1",
                        new Dictionary<string, string>() { { "broadcaster_user_id", Api.UserId } },
                        EventSubTransportMethod.Websocket, _eventSubWebsocketClient.SessionId);

                //https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelfollow
                await Api.Api.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.follow", "2",
                    new Dictionary<string, string>() { { "broadcaster_user_id", Api.UserId }, { "moderator_user_id", Api.UserId } },
                    EventSubTransportMethod.Websocket, _eventSubWebsocketClient.SessionId);

                await Api.Api.Helix.EventSub.CreateEventSubSubscriptionAsync("stream.online", "1",
                    new Dictionary<string, string>() { { "broadcaster_user_id", Api.UserId }, },
                    EventSubTransportMethod.Websocket, _eventSubWebsocketClient.SessionId);


                await Api.Api.Helix.EventSub.CreateEventSubSubscriptionAsync("stream.offline", "1",
                    new Dictionary<string, string>() { { "broadcaster_user_id", Api.UserId }, },
                    EventSubTransportMethod.Websocket, _eventSubWebsocketClient.SessionId);

                await Api.Api.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.raid", "1",
                      new Dictionary<string, string>() { { "to_broadcaster_user_id", Api.UserId } },
                      EventSubTransportMethod.Websocket, _eventSubWebsocketClient.SessionId);

                //https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelhype_trainend
                await Api.Api.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.hype_train.begin", "1",
                     new Dictionary<string, string>() { { "broadcaster_user_id", Api.UserId } },
                     EventSubTransportMethod.Websocket, _eventSubWebsocketClient.SessionId);

                await Api.Api.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.hype_train.end", "1",
                    new Dictionary<string, string>() { { "broadcaster_user_id", Api.UserId } },
                    EventSubTransportMethod.Websocket, _eventSubWebsocketClient.SessionId);

                //https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelchannel_points_custom_reward_redemptionadd
                await Api.Api.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.channel_points_custom_reward_redemption.add", "1",
                    new Dictionary<string, string>() { { "broadcaster_user_id", Api.UserId } },
                    EventSubTransportMethod.Websocket, _eventSubWebsocketClient.SessionId);

                //https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelcheer
                await Api.Api.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.cheer", "1",
                    new Dictionary<string, string>() { { "broadcaster_user_id", Api.UserId } },
                    EventSubTransportMethod.Websocket, _eventSubWebsocketClient.SessionId);

                //https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelchatmessage
                await Api.Api.Helix.EventSub.CreateEventSubSubscriptionAsync("channel.chat.message", "1",
                   new Dictionary<string, string>() { { "broadcaster_user_id", Api.UserId },
                       { "user_id", Api.UserId } },
                   EventSubTransportMethod.Websocket, _eventSubWebsocketClient.SessionId);

                Authenticated?.Invoke(Api, Api.Auth);
            }
            catch (Exception e)
            {
                Log.Error($"Error in twitch auth: " + e.Message, e);
            }
        }

        private static async Task OnWebsocketDisconnected(object? sender, EventArgs e)
        {
            Log.Debug($"Websocket {_eventSubWebsocketClient.SessionId} disconnected!");

            // Don't do this in production. You should implement a better reconnect strategy with exponential backoff
            int count = 0;
            while (!await _eventSubWebsocketClient.ReconnectAsync() && count < 100)
            {
                Log.Debug("Websocket reconnect failed!");
                await Task.Delay(500 * count);
                count++;
            }
        }


        private async static Task OnChannelFollow(object? sender, ChannelFollowArgs e)
        {
            var eventData = e.Notification.Payload.Event;
            NewFollower?.Invoke(null, eventData.UserName);
        }

        private static async Task OnChannelPointsRedeemed(object? sender, ChannelPointsCustomRewardRedemptionArgs e)
        {
            ChannelPointsCustomRewardRedemption eventData = e.Notification.Payload.Event;
            RedeemUsed?.Invoke(null, eventData.Reward.Title);
            RedeemFull?.Invoke(null, eventData);
        }

        private static async Task Cheer(object sender, ChannelCheerArgs e)
        {
            var eventData = e.Notification.Payload.Event;
            BitsUsed?.Invoke(null, eventData);
        }

        private static async Task HypeTrainStart(object? sender, ChannelHypeTrainBeginArgs e)
        {
            var eventData = e.Notification.Payload.Event;
        }

        private static async Task HypeTrainEnd(object? sender, ChannelHypeTrainEndArgs e)
        {
            var eventData = e.Notification.Payload.Event;
        }


        private static async Task OnChannelRaid(object? sender, ChannelRaidArgs e)
        {
            var eventData = e.Notification.Payload.Event;
        }

        private static async Task OnStreamOffline(object? sender, StreamOfflineArgs e)
        {
            var eventData = e.Notification.Payload.Event;
        }

        private static async Task OnStreamOnline(object? sender, StreamOnlineArgs e)
        {
            var eventData = e.Notification.Payload.Event;
        }

        private static async Task ChatMessage(object sender, ChannelChatMessageArgs e)
        {
            var eventData = e.Notification.Payload.Event;
            NewChat?.Invoke(null, eventData);
        }
    }
}
