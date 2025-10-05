using PngifyMe.Helpers;
using PngifyMe.Layers.Helper;
using PngifyMe.Layers.Movements;
using PngifyMe.Services;
using PngifyMe.Services.CharacterSetup.Basic;
using PngifyMe.Views.Avatar;
using System;
using System.Linq;

namespace PngifyMe.Layers;

[LayerDescription("InactiveStateSwitcher")]
public class InactiveStateSwitcher : PermaLayer
{
    private float lastupdate;

    [Unit("seconds")]
    public float InActiveTime { get; set; } = 30;

    [Unit("Name of Layer")]
    public string LayerName { get; set; } = string.Empty;

    public override void OnEnter()
    {
        lastupdate = CurrentTime;
        base.OnEnter();
    }

    public override void OnCalculateParameters(float dt, ref LayerValues values)
    {
        if (AudioService.Talking)
            lastupdate = CurrentTime;

        if (lastupdate + InActiveTime > CurrentTime)
        {
            // only works on basic -> might be able to do this on advanced too
            if (LayerManager.CharacterStateHandler.CharacterSetup is BasicCharacterSetup setup)
            {
                var set = (BasicCharSettings)setup.Settings;
                var target = set.States.FirstOrDefault(x => string.Compare(x.Name, LayerName, true) == 0);
                if (target != null)
                    setup.SwitchState(target);
            }
        }
    }
}
