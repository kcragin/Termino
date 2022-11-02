using System.Collections.Concurrent;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using Termino;

var colorsBinder = new ColorsBinder();
var commandQueue = new BlockingCollection<ICliCommand>();

#region Set Command

var tabColorOption = new Option<string>("--tabColor", "Sets the tab color");
tabColorOption.AddAlias("-tc");
tabColorOption.ArgumentHelpName = "color";
tabColorOption.AddValidator(colorsBinder.ValidateColor);

var backgroundOption = new Option<string>("--background", "Sets the background color of the window or pane");
backgroundOption.AddAlias("-b");
backgroundOption.ArgumentHelpName = "color";
backgroundOption.AddValidator(colorsBinder.ValidateColor);

var foregroundOption = new Option<string>("--foreground", "Sets foreground color of the window or pane");
foregroundOption.AddAlias("-f");
foregroundOption.ArgumentHelpName = "color";
foregroundOption.AddValidator(colorsBinder.ValidateColor);

var setCommand = new Command("set", "Sets the specified Windows Terminal element to the specified value")
{
    tabColorOption,
    backgroundOption,
    foregroundOption,
};

setCommand.AddValidator(cr =>
{
    if (!cr.Children.Any())
        cr.ErrorMessage = "At least one option must be supplied";
});

setCommand.SetHandler(
    colors =>
    {
        if (colors.TabColor.HasValue)
            commandQueue.Add(new CliSetTabColorCommand(
                tabColorOption.Name,
                tabColorOption.Description ?? "",
                colors.TabColor.Value,
                Mode.InProc));
        if (colors.Background.HasValue)
            commandQueue.Add(new CliSetBackgroundColorCommand(
                backgroundOption.Name,
                backgroundOption.Description ?? "",
                colors.Background.Value,
                Mode.InProc));
        if (colors.Foreground.HasValue)
            commandQueue.Add(new CliSetForegroundColorCommand(
                foregroundOption.Name,
                foregroundOption.Description ?? "",
                colors.Foreground.Value,
                Mode.InProc));
    },
    colorsBinder);

#endregion

#region Undo Command

var undoArgument = new Argument<int>(
    "N",
    () => 1,
    "The number of commands to undo");

var undoCommand = new Command("undo", "Undoes the previous N commands. If N is not specified, only the most recent command is undone.")
{
    undoArgument,
};

undoCommand.SetHandler(
    numberOfCommandsToUndo =>
    {
        commandQueue.Add(new CliUndoCommand(
            undoArgument.Name,
            undoArgument.Description ?? "",
            Mode.InProc,
            numberOfCommandsToUndo));
    },
    undoArgument);

#endregion

#region Get Command
var getCommand = new Command("get", "Gets the content of the Windows Terminal settings file. The mode is ignored; settings will be retrieved and displayed synchronously")
{
};

getCommand.SetHandler(file =>
{
});

#endregion

#region Global Options

var modeOption = new Option<Mode>(
    new[] { "-m", "--mode" },
    () => Mode.InProc,
    "The mode in which commands will be executed against the profile. Can be one of InProc or OutOfProc");

var settingsFileLocationOption = new Option<SettingsFileLocation>(
    new[] { "-sfl", "--settingsfilelocation" },
    () => SettingsFileLocation.Stable,
    "The logical location of the settings file, which is based on the release of Windows Terminal that you have installed. Can be one of Stable, Preview, Unpackaged or Named");

#endregion

var rootCommand = new RootCommand("Termino - Control how your Windows Terminal appears and works via the command line")
{
    getCommand,
    setCommand,
    undoCommand,
    modeOption,
    settingsFileLocationOption
};

var parser = new CommandLineBuilder(rootCommand)
    .UseHelp(context =>
    {
        // add a general section about colors for commands that require a color argument
        if (context.Command == setCommand)
            context.HelpBuilder.CustomizeLayout(
                _ => HelpBuilder.Default
                    .GetLayout()
                    .Append(hc =>
                    {
                        Console.WriteLine("Colors:");
                        var colorParams = new[] { new TwoColumnHelpRow("<color>", "Should be a named color (see https://www.w3schools.com/colors/colors_names.asp) or have the hexadecimal format, #RRGGBB (see https://www.w3schools.com/colors/colors_hexadecimal.asp)")};
                        hc.HelpBuilder.WriteColumns(colorParams, hc);
                    }));
    })
    .Build();

var parseResult = parser.Parse(args);
if (parseResult.Errors.Any())
{
    foreach (var e in parseResult.Errors)
    {
        Console.Error.WriteLine(e);
        Console.Error.WriteLine();
    }

    Environment.Exit(1);
}

var x = await parseResult.InvokeAsync();

// after parsing, we want to run a background task that dequeues the
// commands sent from the handlers assigned to the parsed commands/options/arguments.
// this task will then play these commands directly against the settings file or
// push them to a daemon for playing out-of-process from the CLI
await Task.Run(() =>
{
    foreach (var command in commandQueue.GetConsumingEnumerable())
    {
        // if outproc requeue
        // if inproc exec
    }
});

return x;
