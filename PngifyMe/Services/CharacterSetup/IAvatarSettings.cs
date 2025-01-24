using PngifyMe.Services.CharacterSetup.Basic;
using System.Collections.Generic;

namespace PngifyMe.Services.CharacterSetup
{
    public interface IAvatarSettings
    {
        List<CharacterState> States { get; set; }
    }
}