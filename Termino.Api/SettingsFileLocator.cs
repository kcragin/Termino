namespace Termino.Api;

/// <summary>
/// Provides the location of a settings file.
/// </summary>
public class SettingsFileLocator
{
    /// <summary>
    /// The sole instance of the <see cref="SettingsFileLocator"/>.
    /// </summary>
    public static readonly SettingsFileLocator Instance = new();

    /// <summary>
    /// Gets the logical locations of the settings file based on https://learn.microsoft.com/en-us/windows/terminal/install.
    /// </summary>
    /// <remarks>
    /// The returned value is the location of the settings file without any environment variables or special path segments expanded.
    /// </remarks>
    /// <param name="installationType">The type of Windows Terminal installation whose settings file must be retrieved.</param>
    /// <param name="hintPath">An optional path that hints where the file may actually be. This is mainly used for a <see cref="WindowsTerminalInstallType.Unspecified" /> installationType.</param>
    /// <returns>A string representing the full logical path to the settings file based on the type of install.</returns>
    /// <exception cref="ArgumentOutOfRangeException">The installation type is out of range.</exception>
    public string GetSettingsFileLogicalPath(WindowsTerminalInstallType installationType, string? hintPath = null)
    {
        return installationType switch
        {
            WindowsTerminalInstallType.Stable => @"%LOCALAPPDATA%\Packages\Microsoft.WindowsTerminal_8wekyb3d8bbwe\LocalState\settings.json",
            WindowsTerminalInstallType.Preview => @"%LOCALAPPDATA%\Packages\Microsoft.WindowsTerminalPreview_8wekyb3d8bbwe\LocalState\settings.json",
            WindowsTerminalInstallType.Unpackaged => @"%LOCALAPPDATA%\Microsoft\Windows Terminal\settings.json",
            WindowsTerminalInstallType.Unspecified => hintPath ?? throw new ArgumentException("The hint path must be specified for an unspecified installation type", nameof(hintPath)),
            _ => throw new ArgumentOutOfRangeException(nameof(installationType), "The installation type specified is out of range of the valid installation types")
        };
    }

    public FileInfo GetSettingsFilePath(WindowsTerminalInstallType installationType, string? hintPath = null)
    {
        var x = Environment.ExpandEnvironmentVariables(GetSettingsFileLogicalPath(installationType, hintPath));
        return new FileInfo(x);
    }
}