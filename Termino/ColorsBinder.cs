using System.CommandLine.Binding;
using System.CommandLine.Parsing;
using System.Drawing;

namespace Termino;

internal sealed class ColorsBinder : BinderBase<Colors>
{
    protected override Colors GetBoundValue(BindingContext bindingContext) => Colors;

    public void ValidateColor(SymbolResult r)
    {
        try
        {
            var value = ColorTranslator.FromHtml(r.Tokens[0].Value);
            if (value == Color.Empty)
            {
                r.ErrorMessage = "Color was not specified";
                return;
            }

            switch (r.Symbol.Name)
            {
                case "tabColor":
                    Colors.TabColor = value;
                    return;
                case "background":
                    Colors.Background = value;
                    return;
                case "foreground":
                    Colors.Foreground = value;
                    return;
            }

            r.ErrorMessage = $"The symbol '{r.Symbol.Name}' cannot be parsed";

        }
        catch (FormatException)
        {
            r.ErrorMessage = r.Symbol.Name switch
            {
                "tabColor" => $"The tab color '{r.Tokens[0].Value}' appears to be a hexadecimal specification, but there are invalid digits.",
                "background" => $"The background color '{r.Tokens[0].Value}' appears to be a hexadecimal specification, but there are invalid digits.",
                "foreground" => $"The foreground color '{r.Tokens[0].Value}' appears to be a hexadecimal specification, but there are invalid digits.",
                _ => "An error occurred while trying to set the color"
            };
        }
        catch (ArgumentException e) when (e.Message.Contains("is not a valid value"))
        {
            r.ErrorMessage = r.Symbol.Name switch
            {
                "tabColor" => $"Cannot set an unknown color named '{r.Tokens[0].Value}' as the tab color",
                "background" => $"Cannot set an unknown color named '{r.Tokens[0].Value}' as the background color",
                "foreground" => $"Cannot set an unknown color named '{r.Tokens[0].Value}' as the foreground color",
                _ => "An error occurred while trying to set the color"
            };
        }
        catch
        {
            r.ErrorMessage = r.Symbol.Name switch
            {
                "tabColor" => "An error occurred while trying to determine the tab color",
                "background" => "An error occurred while trying to determine the background color",
                "foreground" => "An error occurred while trying to determine the foreground color",
                _ => "An error occurred while trying to set the color"
            };
        }
    }

    public Colors Colors { get; } = new();
}