namespace Termino.Api;

public interface ITerminoClient
{
    void SendCommand(ICommand command);
}