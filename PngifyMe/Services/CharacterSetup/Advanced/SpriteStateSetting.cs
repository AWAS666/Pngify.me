﻿using System;

namespace PngifyMe.Services.CharacterSetup.Advanced;

public class SpriteStateSetting
{
    public int Index { get; set; }
    public bool Flag { get; set; } = true;

    public SpriteStateSetting Clone()
    {
        return (SpriteStateSetting)this.MemberwiseClone();
    }
}