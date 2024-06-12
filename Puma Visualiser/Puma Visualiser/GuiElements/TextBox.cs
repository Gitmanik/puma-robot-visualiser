using Raylib_CsLo;

namespace Puma_Visualiser.GuiElements;

public class TextBox : GuiElement
{
    public string Text { get; set; }
    private int _textSize;
    private bool _active;

    public TextBox(Rectangle bounds, int textSize, string text = "") : base(bounds)
    {
        Text = text;
        _textSize = textSize;
    }

    public override void Draw()
    {
        base.Draw();

        if (Raylib.IsMouseButtonReleased(Raylib.MOUSE_LEFT_BUTTON))
        {
            var mouse = Raylib.GetMousePosition();
            if (mouse.X > Bounds.X && mouse.X < Bounds.X + Bounds.width &&
                mouse.Y > Bounds.Y && mouse.Y < Bounds.Y + Bounds.height)
                _active = true;
            else
                _active = false;
        }

        if (_active)
        {
            char cha = (char)Raylib.GetCharPressed();
            int key = Raylib.GetKeyPressed();
            if (cha != 0)
            {
                Text += cha;
            }
            else if (key != 0)
            {
                if (key == 259)
                {
                    if (Text.Length > 1)
                        Text = Text.Remove(Text.Length - 1);
                }
            }
        }

        RayGui.GuiTextBox(Bounds, Text, _textSize, _active);
    }
}