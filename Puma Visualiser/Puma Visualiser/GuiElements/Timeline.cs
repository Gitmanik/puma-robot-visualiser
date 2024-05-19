using Raylib_CsLo;

namespace Puma_Visualiser.GuiElements;

public class Timeline : GuiElement
{
    private readonly int _margin;
    private readonly int _trackButtonClickMargin;
    private readonly Color _trackColor;
    private readonly Color _backgroundColor;
    private readonly Color _playheadColor;
    private readonly Color _trackBackgroundColor;

    private float _time;
    private float _timeStart;
    private float _timeEnd;

    private Dictionary<int, List<Button>> _trackButtons = new();

    public Timeline(Rectangle bounds, Color backgroundColor, Color trackBackgroundColor, Color trackColor,
        Color playheadColor, int margin, int trackButtonClickMargin) : base(bounds)
    {
        _backgroundColor = backgroundColor;
        _trackColor = trackColor;
        _trackBackgroundColor = trackBackgroundColor;
        _playheadColor = playheadColor;
        _margin = margin;
        _trackButtonClickMargin = trackButtonClickMargin;

        for (int idx = 0; idx < 3; idx++)
            _trackButtons[idx] = new List<Button>();
    }

    public override void Draw()
    {
        base.Draw();
        Raylib.DrawRectangle(0, (int)Bounds.y, (int)Bounds.width, (int)Bounds.height, _backgroundColor);

        float xx = (Bounds.height - 2 * _margin) / 3;

        Raylib.DrawRectangle((int)Bounds.x + _margin, (int)Bounds.y + _margin, (int)Bounds.width - 2 * _margin,
            (int)Bounds.height - 2 * _margin, _trackBackgroundColor);

        // Draw track lines
        for (int idx = 1; idx <= 3; idx++)
        {
            int y = (int)(Bounds.y + _margin + idx * xx - .5 * xx);
            Raylib.DrawLine((int)Bounds.x + _margin, y, (int)Bounds.width - _margin, y, _trackColor);
        }

        // Draw track points
        // TODO: Register which button was clicked and act
        bool trackButtonClicked = false;
        for (int idx = 0; idx < _trackButtons.Count; idx++)
            foreach (var button in _trackButtons[idx])
            {
                button.Draw();
                if (button.IsClicked())
                    trackButtonClicked = true;
            }

        // Add new track points
        if (!trackButtonClicked && Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
        {
            for (int idx = 1; idx <= 3; idx++)
            {
                int y = (int)(Bounds.y + _margin + idx * xx - .5 * xx);
                var mouse = Raylib.GetMousePosition();
                if (mouse.X > Bounds.X + _margin && mouse.X < Bounds.width - _margin &&
                    mouse.Y > y - _trackButtonClickMargin && mouse.Y < y + _trackButtonClickMargin)
                {
                    Console.WriteLine($"Click on track {idx}, x:{mouse.X}");
                    _trackButtons[idx - 1].Add(new Button(new Rectangle(mouse.X - 5, y - 5, 10, 10), ""));
                    Console.WriteLine(_trackButtons[idx - 1].Count);
                }
            }
        }

        // Draw playhead
        int timeX = (int)Map(_time, _timeStart, _timeEnd, Bounds.x + _margin, Bounds.x + Bounds.width - _margin);
        Raylib.DrawLine(timeX, (int)Bounds.Y + _margin, timeX,
            (int)Bounds.y + (int)Bounds.height - _margin, _playheadColor);
    }

    public void SetTimeData(float time, float timeStart, float timeEnd)
    {
        _time = time;
        _timeStart = timeStart;
        _timeEnd = timeEnd;
    }

    private static float Map(float x, float inMin, float inMax, float outMin, float outMax) =>
        (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
}