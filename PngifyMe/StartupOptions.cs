namespace PngifyMe;

/// <summary>
/// Startup options set from command-line arguments before the application runs.
/// </summary>
public static class StartupOptions
{
    /// <summary>
    /// Profile name to activate at startup (e.g. from --profile &lt;name&gt;). Null or empty = use default.
    /// Cleared by SettingsManager after applying once.
    /// </summary>
    public static string? ProfileName { get; set; }
}
