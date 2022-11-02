using Termino.Api;

namespace Termino;

internal record CliUndoCommand(string Name, string Description, Mode Mode, int NumberOfCommands) : UndoCommand(Name, Description, NumberOfCommands), ICliCommand
{
    public void Execute()
    {
        if (Mode == Mode.InProc)
        {
            // store command to state
        }
        else
        {
            // send command to oop
        }
    }
}