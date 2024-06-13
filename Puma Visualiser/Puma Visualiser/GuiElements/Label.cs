using Raylib_CsLo;

namespace Puma_Visualiser.GuiElements;

public class Label : GuiElement
{
    public string Text { get; set; }
    private Color _color;
    private int _fontSize;

    public Label(Rectangle bounds, string text, int fontSize, Color color) : base(bounds)
    {
        Text = text;
        _fontSize = fontSize;
        _color = color;
    }

    public override void Draw()
    {
        base.Draw();
        Raylib.DrawText(Text, Bounds.x, Bounds.y, _fontSize, _color);
    }
}