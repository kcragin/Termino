using System.Drawing;

namespace Termino;

internal record Colors
{
    public Color? Foreground { get; set; }
    public Color? Background { get; set; }
    public Color? TabColor { get; set; }
}