using Raylib_CsLo;

namespace Puma_Visualiser.GuiElements;

public class Button : GuiElement
{
    private string _text;
    private bool _state;

    public Button(Rectangle bounds, string text) : base(bounds)
    {
        _text = text;
    }

    public override void Draw()
    {
        base.Draw();
        _state = RayGui.GuiButton(Bounds, _text);
    }

    public bool IsClicked() => _state;
}