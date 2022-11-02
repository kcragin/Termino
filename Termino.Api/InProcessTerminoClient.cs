namespace Termino.Api;

/// <summary>
/// A termino client that executes commands sequentially within the containing
/// process space. The commands are executed against Windows Terminal's settings
/// file synchronously.
/// </summary>
public sealed class InProcessTerminoClient : ITerminoClient
{
    public InProcessTerminoClient(string settingsFilePath)
    {
        
    }

    public void SendCommand(ICommand command)
    {
    }
}