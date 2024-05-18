using Raylib_CsLo;

namespace Puma_Visualiser.GuiElements;

public class Timeline : GuiElement
{
    //TODO: Move time system to MainView (or somewhere else)
    private float time = 2;
    private float timeScale = 1;
    private float timeStart = 0;
    private float timeEnd = 5;

    private readonly int _margin = 10;

    private readonly int _trackButtonClickMargin = 10;
    private readonly Color _trackColor = Raylib.GREEN;

    private Dictionary<int, List<Button>> trackButtons = new();

    public Timeline(Rectangle _bounds) : base(_bounds)
    {
        for (int idx = 0; idx < 3; idx++)
            trackButtons[idx] = new List<Button>();
    }

    public override void Draw()
    {
        Raylib.DrawRectangle(0, (int)_bounds.y, (int)_bounds.width, (int)_bounds.height, Raylib.WHITE);

        float xx = (_bounds.height - 2 * _margin) / 3;

        Raylib.DrawRectangle((int)_bounds.x + _margin, (int)_bounds.y + _margin, (int)_bounds.width - 2 * _margin,
            (int)_bounds.height - 2 * _margin, Raylib.BLACK); // TODO: Change color

        // TODO: Register which button was clicked and act
        bool trackButtonClicked = false;
        for (int idx = 0; idx < trackButtons.Count; idx++)
            foreach (var button in trackButtons[idx])
            {
                button.Draw();
                if (button.IsClicked())
                    trackButtonClicked = true;
            }

        for (int idx = 1; idx <= 3; idx++)
        {
            int y = (int)(_bounds.y + _margin + idx * xx - .5 * xx);
            Raylib.DrawLine((int)_bounds.x + _margin, y, (int)_bounds.width - _margin, y, _trackColor);

            if (!trackButtonClicked && Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
            {
                var mouse = Raylib.GetMousePosition();
                if (mouse.X > _bounds.X + _margin && mouse.X < _bounds.width - _margin &&
                    mouse.Y > y - _trackButtonClickMargin && mouse.Y < y + _trackButtonClickMargin)
                {
                    Console.WriteLine($"C on track {idx}, x:{mouse.X}");
                    trackButtons[idx - 1].Add(new Button(new Rectangle(mouse.X - 5, y - 5, 10, 10), ""));
                    Console.WriteLine(trackButtons[idx - 1].Count);
                }
            }
        }

        int timeX = (int)Map(time, timeStart, timeEnd, _bounds.x + _margin, _bounds.x + _bounds.width - _margin);

        Raylib.DrawLine(timeX, (int)_bounds.Y + _margin, timeX,
            (int)_bounds.y + _margin + (int)_bounds.height - _margin, Raylib.RED);

        // TODO: Move time system to MainView (or somewhere else)
        time += Raylib.GetFrameTime();
        if (time > timeEnd) time = timeStart;
    }

    private float Map(float x, float inMin, float inMax, float outMin, float outMax) =>
        (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
}