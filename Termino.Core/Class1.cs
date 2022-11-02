namespace Termino.Core
{
    public interface IUndoStack
    {
        void Push(Command command);

        CommandExecution Pop();

        IReadOnlyCollection<CommandExecution> PopAll();
    }

    public class UndoStack : IUndoStack
    {
        public virtual void Push(Command command) 
        {
            _stack.Push(new CommandExecution(command));
        }

        public virtual CommandExecution Pop() 
        {
            return _stack.Pop();
        }

        public virtual IReadOnlyCollection<CommandExecution> PopAll() 
        {
            var commands = _stack.ToArray();
            _stack.Clear();
            return commands;
        }

        private readonly Stack<CommandExecution> _stack = new Stack<CommandExecution>();
    }

    /// <summary>
    /// Combines a command and a timestamp to represent the execution of a given command against the settings file.
    /// </summary>
    public class CommandExecution
    { 
        internal CommandExecution(Command command) 
        {
            Command = command;
            Timestamp = DateTime.UtcNow;
        }

        public Command Command { get; }

        public DateTime Timestamp { get; }
    }

    public interface IExecutable
    {
        void Execute();

        void Unexecute();
    }



    public abstract class Command
    {
        public string Name { get; init; }

        public string Description { get; init; }
    }

    public class SetForegroundCommand : Command
    {

    }

    public class TransformCommand : Command
    {

    }

    public class CompositeCommand : Command
    { }

    public interface ISettingsTransformer
    {

    }

    public class SettingsTransformer : ISettingsTransformer
    {
        
    }

    public interface ISettingsModel
    {

    }

    public class InMemorySettingsModel : ISettingsModel
    {

    }

    public interface ISettingsReader
    {

    }

    public abstract class SettingsReader : ISettingsReader
    { 
    }

    public class FileSettingsReader : SettingsReader
    { }

    public class StandardFileSettingsReader : FileSettingsReader
    { }

    public class MemorySettingsReader : SettingsReader
    { }

}