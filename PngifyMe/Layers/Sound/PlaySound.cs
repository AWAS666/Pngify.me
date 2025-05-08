using PngifyMe.Layers.Helper;
using PngifyMe.Services;
using Serilog;
using System.IO;
using System.Threading.Tasks;

namespace PngifyMe.Layers.Sound;

[LayerDescription("PlayOneSound")]
public class PlaySound : PermaLayer
{
    [Unit("Path")]
    [WavPicker]
    public string FilePath { get; set; } = string.Empty;

    public PlaySound()
    {
        EnterTime = 0f;
        ExitTime = 0f;
    }

    public override void OnEnter()
    {
        if (!File.Exists(FilePath))
        {
            Log.Error($"PlaySound: File {FilePath} not found");
            IsExiting = true;
            return;
        }
        var audio = File.OpenRead(FilePath);

        _ = Task.Run(async () =>
        {
            await AudioService.PlaySoundWav(audio, 1);
            IsExiting = true;
        });
        base.OnEnter();
    }

    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        // do nothing
    }
}
