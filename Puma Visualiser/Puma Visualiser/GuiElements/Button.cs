using Raylib_CsLo;

namespace Puma_Visualiser.GuiElements;

public class Button : GuiElement
{
    private string _text;
    private bool _state;
    public Button(Rectangle _bounds, string text) : base(_bounds)
    {
        _text = text;
    }

    public override void Draw()
    {
        base.Draw();
        _state = RayGui.GuiButton(_bounds, _text);
    }

    public bool IsClicked() => _state;
}