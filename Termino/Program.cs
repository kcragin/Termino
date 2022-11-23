using System.Collections.Concurrent;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using System.Diagnostics;
using Termino;
using Termino.Api;

var colorsBinder = new ColorsBinder();
var commandQueue = new BlockingCollection<ICliCommand>();

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
var getCommand = new Command("get", "Gets the content of the Windows Terminal settings file. The mode option is ignored.")
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

var settingsFileLocationOption = new Option<WindowsTerminalInstallType>(
    new[] { "-sfl", "--settingsfilelocation" },
    () => WindowsTerminalInstallType.Stable,
    "The logical location of the settings file, which is based on the release of Windows Terminal that you have installed. Can be one of Stable, Preview, Unpackaged or Unspecified");

var globalOptions = new Option[] { modeOption, settingsFileLocationOption };

#endregion

#region Profile Related Options

var profileNameOption = new Option<string>(
    new[] { "-n", "--name" },
    "References the first profile profile with the specified name");

var profileGuidOption = new Option<string>(
    new[] { "-g", "--guid" },
    "References the profile identified by the specified guid");

var profilePositionOption = new Option<int>(
    new[] { "-p", "--position" },
    "References the profile at the specified, zero-indexed position")
{
    ArgumentHelpName = "pos",
};

var profileRelatedOptions = globalOptions
    .Concat(new Option[] { profileGuidOption, profileNameOption, profilePositionOption })
    .ToArray();

#endregion

#region Profile Get Command

var profileGetCommand = new Command("get", "Gets the content of the specified profile. The mode option is ignored.");
profileRelatedOptions.ForEach(profileGetCommand.AddOption);

profileGetCommand.SetHandler(file =>
{
});

#endregion

#region Profile Set Command

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

var profileSetCommand = new Command("set", "Sets the specified Windows Terminal element to the specified value")
{
    tabColorOption,
    backgroundOption,
    foregroundOption
};

profileRelatedOptions.ForEach(profileSetCommand.AddOption);

profileSetCommand.AddValidator(cr =>
{
    if (!cr.Children.Any())
        cr.ErrorMessage = "At least one option must be supplied";
});

profileSetCommand.SetHandler(
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

var profileSubCommand = new Command("profile", "Gets information about a profile or changes settings related to a profile")
{
    profileGetCommand,
    profileSetCommand,
};

var rootCommand = new RootCommand("Termino - Control how your Windows Terminal appears and works via the command line")
{
    modeOption,
    settingsFileLocationOption,
    getCommand,
    undoCommand,
    profileSubCommand,
};

var parseResult = new CommandLineBuilder(rootCommand)
    .UseHelp(context =>
    {
        // add a general section about colors for commands that require a color argument

        switch (context.Command)
        {
            case var x when x == profileSetCommand:
                context.HelpBuilder.CustomizeLayout(
                    _ => HelpBuilder.Default
                        .GetLayout()
                        .Append(hc =>
                        {
                            Console.WriteLine("Colors:");
                            var colorParams = new[] { new TwoColumnHelpRow("<color>", "Should be a named color (see https://www.w3schools.com/colors/colors_names.asp) or have the hexadecimal format, #RRGGBB (see https://www.w3schools.com/colors/colors_hexadecimal.asp)") };
                            hc.HelpBuilder.WriteColumns(colorParams, hc);
                        }));
                break;
        }
    })
    .Build()
    .Parse(args);

if (parseResult.Errors.Any())
{
    foreach (var e in parseResult.Errors)
    {
        Console.Error.WriteLine(e);
        Console.Error.WriteLine();
    }

    Environment.Exit(1);
}

var exitCode = await parseResult.InvokeAsync();

if (exitCode == 0)
    // after parsing, we want to run a background task that dequeues the
    // commands sent from the handlers assigned to the parsed commands/options/arguments.
    // this task will then play these commands directly against the settings file or
    // push them to a daemon for playing out-of-process from the CLI
    await Task.Run(() =>
    {
        foreach (var command in commandQueue.GetConsumingEnumerable())
        {
            // if outofproc requeue
            // if inproc exec
        }
    });

return exitCode;
