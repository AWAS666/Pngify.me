using Serilog.Core;
using Serilog.Events;
using System;
using Ursa.Controls;

namespace PngifyMe.Helpers
{
    public class EventSink : ILogEventSink
    {
        public WindowNotificationManager NotificationManager { get; private set; }
        public bool ShowErrors { get; private set; }

        public void Emit(LogEvent logEvent)
        {
            if (!ShowErrors) return;
            NotificationManager?.Show(
                new Notification(null, logEvent.MessageTemplate.Text),
                showIcon: true,
                showClose: true,
                type: Convert(logEvent.Level),
                classes: ["Light"]
                );
        }

        public void SetNotificationHandler(WindowNotificationManager handler)
        {
            NotificationManager = handler;
            NotificationManager.Position = Avalonia.Controls.Notifications.NotificationPosition.BottomLeft;
        }

        public Avalonia.Controls.Notifications.NotificationType Convert(LogEventLevel level)
        {
            switch (level)
            {
                case LogEventLevel.Verbose:
                case LogEventLevel.Debug:
                case LogEventLevel.Information:
                    return Avalonia.Controls.Notifications.NotificationType.Information;
                case LogEventLevel.Warning:
                    return Avalonia.Controls.Notifications.NotificationType.Warning;
                case LogEventLevel.Error:
                case LogEventLevel.Fatal:
                    return Avalonia.Controls.Notifications.NotificationType.Error;
                default:
                    return Avalonia.Controls.Notifications.NotificationType.Information;
            }
        }

        internal void SetActive(bool isVisible)
        {
            ShowErrors = isVisible;
        }
    }

    public static class ErrorForwarder
    {
        public static EventSink Sink = new EventSink();
    }
}
