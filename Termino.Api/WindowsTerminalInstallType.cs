namespace Termino.Api;

/// <summary>
/// Specifies the type of Windows Terminal installation.
/// </summary>
/// <remarks>
/// Note that this is not the same thing as a version. For example, there can be multiple
/// stable versions and multiple preview versions.
/// </remarks>
public enum WindowsTerminalInstallType
{
    /// <summary>
    /// Indicates the stable (generally available) version of Windows Terminal installed.
    /// </summary>
    Stable = 0,

    /// <summary>
    /// Indicates the preview version of Windows Terminal installed.
    /// </summary>
    Preview = 1,

    /// <summary>
    /// Indicates a version of Windows Terminal that has not yet been packaged.
    /// </summary>
    Unpackaged = 2,

    /// <summary>
    /// No known version is installed. Use this when specifying an explicit location of the file (e.g. for
    /// testing).
    /// </summary>
    Unspecified = 3,

    /// <summary>
    /// Indicates that, where used, the API should attempt to determine what type of installation is installed
    /// by searching the various locations a settings file may be located. Probing starts with <see cref="Stable"/>
    /// and works it's way through the other places in the order of the fields in <see cref="WindowsTerminalInstallType"/>.
    /// </summary>
    Probe = 4
}