using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PngifyMe.Services.Secrets;

public static class SecretsManager
{
    public static string TwitchClientId => GetSecret("TWITCHCLIENTID");

    private static readonly Dictionary<string, string> EnvVariables = new();

    static SecretsManager()
    {
        LoadEmbeddedEnvFile();
    }

    private static void LoadEmbeddedEnvFile()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = assembly.GetManifestResourceNames().FirstOrDefault(name => name.EndsWith(".env"));

        if (resourceName == null)
        {
            // No embedded .env resource found
            Log.Fatal("No embedded .env file found.");
            return;
        }

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            Log.Fatal("Failed to load embedded .env file.");
            return;
        }

        using var reader = new StreamReader(stream);
        var lines = reader.ReadToEnd()
            .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(line => !string.IsNullOrWhiteSpace(line) && !line.TrimStart().StartsWith("#"));

        foreach (var line in lines)
        {
            var parts = line.Split('=', 2, StringSplitOptions.TrimEntries);
            if (parts.Length == 2)
            {
                EnvVariables[parts[0]] = parts[1];
            }
        }
    }

    public static string GetSecret(string key, string defaultValue = null)
    {
        return EnvVariables.GetValueOrDefault(key, defaultValue);
    }
}
