using BarRaider.SdTools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace me.Pngify.Streamdeck
{
    [PluginActionId("me.pngify.streamdeck.switchlayeraction")]
    public class SwitchLayerAction : KeypadBase
    {
        private class PluginSettings
        {
            public static PluginSettings CreateDefaultSettings()
            {
                PluginSettings instance = new PluginSettings
                {
                    LayerName = string.Empty
                };
                return instance;
            }
            [JsonProperty(PropertyName = "parameter")]
            public string LayerName { get; set; }
        }

        #region Private Members

        private PluginSettings settings;

        #endregion
        public SwitchLayerAction(SDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (payload.Settings == null || payload.Settings.Count == 0)
            {
                this.settings = PluginSettings.CreateDefaultSettings();
                SaveSettings();
            }
            else
            {
                this.settings = payload.Settings.ToObject<PluginSettings>();
            }
        }      

        public override void Dispose()
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, $"Destructor called");
        }

        public override void KeyPressed(KeyPayload payload)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Key Pressed");
            var obj = new WebsocketCommand
            {
                Command = "SwitchLayer",
                Parameter = this.settings.LayerName,
            };
            WSClient.Send(JsonConvert.SerializeObject(obj));
        }

        public override void KeyReleased(KeyPayload payload) { }

        public async override void OnTick()
        {
           
        }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            Tools.AutoPopulateSettings(settings, payload.Settings);
            SaveSettings();
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        #region Private Methods

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(settings));
        }

        #endregion
    }
}