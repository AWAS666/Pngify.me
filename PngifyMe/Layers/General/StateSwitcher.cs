using PngifyMe.Helpers;
using PngifyMe.Layers.Helper;
using PngifyMe.Layers.Movements;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup.Basic;
using PngifyMe.Views.Avatar;
using System;
using System.Linq;

namespace PngifyMe.Layers;

[LayerDescription("StateSwitcher")]
public class StateSwitcher : PermaLayer
{
    private float lastupdate;

    [Unit("seconds")]
    public float TimeToSwitch { get; set; } = 30;

    public override void OnEnter()
    {
        lastupdate = CurrentTime;
        base.OnEnter();
    }

    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        if (lastupdate + TimeToSwitch > CurrentTime)
        {
            lastupdate = CurrentTime;
            // only works on basic
            if (LayerManager.CharacterStateHandler.CharacterSetup is BasicCharacterSetup setup)
            {
                var set = (BasicCharSettings)setup.Settings;
                // needs at least two states
                var target = set.States.Shuffle().FirstOrDefault(x => x != setup.CurrentState);
                if (target != null)
                    setup.SwitchState(target);
            }
        }
    }
}
