using System.Drawing;
using Termino.Api;

namespace Termino;

internal record CliSetBackgroundColorCommand(string Name, string Description, Color Color, Mode Mode) : SetBackgroundColorCommand(Name, Description, Color), ICliCommand
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