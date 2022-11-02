namespace Termino;

internal interface ICliCommand
{
    Mode Mode { get; init; }

    void Execute();
}