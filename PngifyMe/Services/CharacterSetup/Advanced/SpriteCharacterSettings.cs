using CommunityToolkit.Mvvm.ComponentModel;
using PngifyMe.Services.CharacterSetup.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PngifyMe.Services.CharacterSetup.Advanced;
public partial class SpriteCharacterSettings : ObservableObject, IAvatarSettings
{
    // todo remove this
    public List<CharacterState> States { get; set; } = new();

    public SpriteImage Parent { get; set; } = new();
    public double BlinkTime { get; set; } = 0.25f;
    public double BlinkInterval { get; set; } = 3f;
}
