using System.Drawing;

namespace Termino.Api;

public interface ICommand
{
}

public record SetTabColorCommand(string Name, string Description, Color Color) : ICommand;

public record SetForegroundColorCommand(string Name, string Description, Color Color) : ICommand;

public record SetBackgroundColorCommand(string Name, string Description, Color Color) : ICommand;

public record UndoCommand(string Name, string Description, int NumberOfCommands) : ICommand;

public class State
{

}