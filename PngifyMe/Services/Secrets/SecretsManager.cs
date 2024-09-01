using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace PngifyMe.Services.Secrets
{
    public static class SecretsManager
    {
        private static Dictionary<string, string>? keys;

        public static string TwitchClientId => GetSecret("TwitchClientId");

        private static string GetSecret(string v)
        {
            return keys[v];
        }

        static SecretsManager()
        {
            var file = LoadFile("secrets.json");
            keys = JsonSerializer.Deserialize<Dictionary<string, string>>(file);
        }

        public static string LoadFile(string embeddedFileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = GetFullyQualifiedResourceName(assembly, embeddedFileName);

            if (string.IsNullOrEmpty(resourceName))
            {
                throw new InvalidOperationException($"Resource '{embeddedFileName}' not found in the assembly.");
            }

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private static string GetFullyQualifiedResourceName(Assembly assembly, string embeddedFileName)
        {
            // Get all resource names and find the matching one
            var resourceNames = assembly.GetManifestResourceNames();
            foreach (var resourceName in resourceNames)
            {
                if (resourceName.EndsWith(embeddedFileName, StringComparison.OrdinalIgnoreCase))
                {
                    return resourceName;
                }
            }

            return null; // Resource not found
        }
    }
}

