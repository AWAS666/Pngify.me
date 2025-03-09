using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;
using Steamworks;

namespace PngifyMe.Steam;

public static class SteamLocalization
{
    public static bool UsingSteam { get; private set; }

    static SteamLocalization()
    {
        try
        {
            var success = SteamAPI.Init();
            if (!success)
            {
                Log.Error($"Steam init failed");
            }
            UsingSteam = success;
        }
        catch (Exception e)
        {
            Log.Error(e, $"Steam init failed: {e.Message}");
        }
    }
    private static CultureInfo MapLocale(string steamLocale) => steamLocale switch
    {
        SteamLocales.English => new CultureInfo("en-US"),
        SteamLocales.SimplifiedChinese => new CultureInfo("zh-CN"),
        SteamLocales.TraditionalChinese => new CultureInfo("zh-CN"),
        SteamLocales.Russian => new CultureInfo("ru-RU"),
        SteamLocales.German => new CultureInfo("de-DE"),
        SteamLocales.French => new CultureInfo("fr-FR"),
        SteamLocales.Brazilian => new CultureInfo("pt-BR"),
        _ => new CultureInfo("en-US")
    };

    public static CultureInfo GetStartUpCulture()
    {
        if (UsingSteam)
        {
            return MapLocale(SteamApps.GetCurrentGameLanguage());
        }
        else
        {
            return new CultureInfo("en-US");
        }
    }
}

public static class SteamLocales
{
    public const string English = "english";
    public const string SimplifiedChinese = "schinese";
    public const string TraditionalChinese = "tchinese";
    public const string Russian = "russian";
    public const string German = "german";
    public const string French = "french";
    public const string Brazilian = "brazilian";
}