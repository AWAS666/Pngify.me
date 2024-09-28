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
    public class PlayRandomSound : PermaLayer
    {
        [Unit("Path")]
        [FolderPicker]
        public string Folder { get; set; } = string.Empty;

        public PlayRandomSound()
        {
            EnterTime = 0f;
            ExitTime = 0f;
        }

        public override void OnEnter()
        {
            if (!Directory.Exists(Folder))
            {
                //todo: raise error here
                IsExiting = true;
                return;
            }
            var files = Directory.GetFiles(Folder).Where(x => x.EndsWith(".wav"));

            if (files.Count() < 1)
            {
                //todo: raise error here
                IsExiting = true;
                return;
            }

            var audio = File.OpenRead(files.ElementAt(Random.Shared.Next(0, files.Count())));

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
