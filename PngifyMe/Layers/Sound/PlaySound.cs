using Avalonia.Platform;
using PngifyMe.Layers.Helper;
using PngifyMe.Layers.Movements;
using PngifyMe.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.Layers.Sound
{
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
                //todo: raise error here
                IsExiting = true;
                return;
            }
            var audio = File.OpenRead(FilePath);

            _ = Task.Run(async () =>
            {
                await AudioService.PlaySoundWav(audio, 1, true);
                audio.Dispose();
                IsExiting = true;
            });
            base.OnEnter();
        }

        public override void OnCalculateParameters(float dt, ref LayerValues values)
        {
            // do nothing
        }
    }
}
