using PngifyMe.Settings;
using System.Text.Json.Serialization;

namespace PngifyMe.Services.Settings;

public class AppSettings
{
    public GeneralSettings General { get; set; } = new();
    public BackgroundSettings Background { get; set; } = new();
    public LayerSetup LayerSetup { get; set; } = new();
    public TwitchSettings Twitch { get; set; } = new();
    public TitsSettings Tits { get; set; } = new();
    public ProfileSettings Profile { get; set; } = new();
    public LLMSettings LLM { get; set; } = new();
}
