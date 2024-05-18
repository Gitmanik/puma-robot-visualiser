using Raylib_CsLo;

namespace Puma_Visualiser.GuiElements
{
    public class TextBox : GuiElement
    {
        private string _text;
        private int _textSize;
        private bool _active;

        public TextBox(Rectangle bounds, int textSize, string text = "") : base(bounds)
        {
            _text = text;
            _textSize = textSize;
        }

		public override void Draw()
        {
            base.Draw();

            if (Raylib.IsMouseButtonReleased(Raylib.MOUSE_LEFT_BUTTON))
            {
                var mouse = Raylib.GetMousePosition();
                if (mouse.X > _bounds.X && mouse.X < _bounds.X + _bounds.width &&
                    mouse.Y > _bounds.Y && mouse.Y < _bounds.Y + _bounds.height)
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
                    _text += cha;
                }
                else if (key != 0)
                {
                    if (key == 259)
                    {
                        _text = _text.Remove(_text.Length - 1);
                    }
                }
            }

            RayGui.GuiTextBox(_bounds, _text, _textSize, _active);
        }
    }
}