using PngifyMe.Layers.Helper;
using PngifyMe.Services.CharacterSetup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PngifyMe.Helpers;

public static class JsonSerializeHelper
{
    public static JsonSerializerOptions GetDefault()
    {
        var options = new JsonSerializerOptions();
        options.Converters.Add(new BaseLayerJsonConverter());
        options.Converters.Add(new TriggerJsonConverter());
        options.Converters.Add(new CharacterSetupJsonConv());
        options.Converters.Add(new CharacterSettingJsonConv());
#if DEBUG
        options.WriteIndented = true;
#endif
        return options;
    }
}
