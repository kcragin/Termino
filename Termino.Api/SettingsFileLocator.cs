using static Termino.Api.WindowsTerminalInstallType;

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
    /// Gets the logical location of the settings file based on https://learn.microsoft.com/en-us/windows/terminal/install.
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
            Stable => @"%LOCALAPPDATA%\Packages\Microsoft.WindowsTerminal_8wekyb3d8bbwe\LocalState\settings.json",
            Preview => @"%LOCALAPPDATA%\Packages\Microsoft.WindowsTerminalPreview_8wekyb3d8bbwe\LocalState\settings.json",
            Unpackaged => @"%LOCALAPPDATA%\Microsoft\Windows Terminal\settings.json",
            Unspecified => hintPath ?? throw new ArgumentException("The hint path must be specified for an unspecified installation type", nameof(hintPath)),
            WindowsTerminalInstallType.Probe => Probe(),
            _ => throw new ArgumentOutOfRangeException(nameof(installationType), "The installation type specified is out of range of the valid installation types")
        };

        string Probe()
        {
            var path = GetSettingsFileLogicalPath(Stable);
            if (File.Exists(path)) return path;
            path = GetSettingsFileLogicalPath(Preview);
            if (File.Exists(path)) return path;
            path = GetSettingsFileLogicalPath(Unpackaged);
            if (File.Exists(path)) return path;
            path = GetSettingsFileLogicalPath(Unspecified, hintPath);
            if (File.Exists(path)) return path;
            throw new FileNotFoundException("Cannot find the location of the settings file");
        }
    }

    /// <summary>
    /// Gets the actual file information about the settings file with all environment variables and special path segments expanded.
    /// </summary>
    /// <param name="installationType"></param>
    /// <param name="hintPath"></param>
    /// <returns></returns>
    public FileInfo GetSettingsFilePath(WindowsTerminalInstallType installationType, string? hintPath = null)
    {
        var path = Environment.ExpandEnvironmentVariables(GetSettingsFileLogicalPath(installationType, hintPath));
        var fi =  new FileInfo(path);
        return fi.Exists ? fi : throw new FileNotFoundException("The settings file does not exist", path);
    }
}