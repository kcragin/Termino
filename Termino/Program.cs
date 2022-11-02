
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Parsing;
using System.Drawing;
using Termino;
using Termino.Api;

var colorsBinder = new ColorsBinder();

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

var undoArgument = new Argument<int>("N", () => 1, "The number of commands to undo");
var undoCommand = new Command("undo", "Undoes the previous N commands. If N is not specified, only the most recent command is undone.")
{
    undoArgument,
};

var rootCommand = new RootCommand("Termino - Control how your Windows Terminal appears and works via the command line")
{
    setCommand,
    undoCommand
};

setCommand.SetHandler(
    colors =>
    {
       if (colors.TabColor.HasValue)
            Console.WriteLine(new SetTabColorCommand(
                tabColorOption.Name,
                tabColorOption.Description ?? "",
                colors.TabColor.Value));
       if (colors.Background.HasValue)
            Console.WriteLine(new SetBackgroundColorCommand(
                backgroundOption.Name,
                backgroundOption.Description ?? "",
                colors.Background.Value));
       if (colors.Foreground.HasValue)
            Console.WriteLine(new SetForegroundColorCommand(
                foregroundOption.Name,
                foregroundOption.Description ?? "",
                colors.Foreground.Value));
    },
    colorsBinder);
    
undoCommand.SetHandler(
    numberOfCommandsToUndo =>
    {
        Console.WriteLine(new UndoCommand(
            undoArgument.Name,
            undoArgument.Description ?? "",
            numberOfCommandsToUndo));
    },
    undoArgument);

var parser = new CommandLineBuilder(rootCommand)
    .UseHelp(context =>
    {
        if (context.Command == setCommand)
            context.HelpBuilder.CustomizeLayout(
                _ => HelpBuilder
                    .Default
                    .GetLayout()
                    .Append(_ => Console.WriteLine("\nColors:\n  <color> A named color (https://www.w3schools.com/colors/colors_names.asp) or have the hexadecimal format, #RRGGBB (https://www.w3schools.com/colors/colors_hexadecimal.asp)")));
    })
    .Build();

var parseResult = parser.Parse(args);
if (parseResult.Errors.Any())
{
    foreach (var e in parseResult.Errors)
        Console.Error.WriteLine(e);
    Environment.Exit(1);
}

var x = await parseResult.InvokeAsync();
return x;
