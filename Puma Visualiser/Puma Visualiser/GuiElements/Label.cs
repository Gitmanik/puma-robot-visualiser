using Raylib_CsLo;

namespace Puma_Visualiser.GuiElements;

public class Label : GuiElement
{
    private string _text;
    private Color _color;
    private int _fontSize;

    public Label(Rectangle bounds, string text, int fontSize, Color color) : base(bounds)
    {
        _text = text;
        _fontSize = fontSize;
        _color = color;
    }

    public override void Draw()
    {
        base.Draw();
        Raylib.DrawText(_text, _bounds.x, _bounds.y, _fontSize, _color);
    }
}