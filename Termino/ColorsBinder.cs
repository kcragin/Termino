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

            r.ErrorMessage = $"{r.Symbol.Name} is an unknown symbol";

        }
        catch
        {
            r.ErrorMessage = r.Symbol.Name switch
            {
                "tabColor" => "An error occurred while trying to determine the tab color",
                "background" => "An error occurred while trying to determine the background color",
                "foreground" => "An error occurred while trying to determine the foreground color",
                _ => r.ErrorMessage
            };
        }
    }

    public Colors Colors { get; } = new();
}