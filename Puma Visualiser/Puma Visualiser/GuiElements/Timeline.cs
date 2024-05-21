using Puma_Visualiser.Objects;
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

    private readonly float _timestampSize = 15;

    private float _time;
    private float _timeStart;
    private float _timeEnd;

    private readonly List<Timestamp> _timestamps = new();

    public Timeline(Rectangle bounds, Color backgroundColor, Color trackBackgroundColor, Color trackColor,
        Color playheadColor, int margin, int trackButtonClickMargin) : base(bounds)
    {
        _backgroundColor = backgroundColor;
        _trackColor = trackColor;
        _trackBackgroundColor = trackBackgroundColor;
        _playheadColor = playheadColor;
        _margin = margin;
        _trackButtonClickMargin = trackButtonClickMargin;
    }

    public override void Draw()
    {
        base.Draw();
        Raylib.DrawRectangle(0, (int)Bounds.y, (int)Bounds.width, (int)Bounds.height, _backgroundColor);

        Raylib.DrawRectangle((int)Bounds.x + _margin, (int)Bounds.y + _margin, (int)Bounds.width - 2 * _margin,
            (int)Bounds.height - 2 * _margin, _trackBackgroundColor);

        // Draw track lines
        for (int trackNumber = 1; trackNumber <= 3; trackNumber++)
        {
            int y = CalculateTrackY(trackNumber);
            Raylib.DrawLine((int)Bounds.x + _margin, y, (int) Bounds.x + (int)Bounds.width - _margin, y, _trackColor);
        }

        // Draw track points
        Timestamp? timestampClicked = null;
        foreach (var timestamp in _timestamps)
        {
            int timestampX = TimeToX(timestamp.Time);
            int timestampY = CalculateTrackY(timestamp.TrackNumber);
            
            bool val = RayGui.GuiButton(new Rectangle(timestampX - _timestampSize/2, timestampY - _timestampSize/2, _timestampSize, _timestampSize), "");
            if (val)
            {
                Console.WriteLine($"{timestamp} clicked!");
                timestampClicked = timestamp;
            }
        }

        // Add new track points
        if (timestampClicked == null && Raylib.IsMouseButtonReleased(MouseButton.MOUSE_BUTTON_LEFT))
        {
            for (int idx = 1; idx <= 3; idx++)
            {
                int y = CalculateTrackY(idx);
                var mouse = Raylib.GetMousePosition();
                
                if (mouse.X > Bounds.X + _margin && mouse.X < Bounds.width - _margin &&
                    mouse.Y > y - _trackButtonClickMargin && mouse.Y < y + _trackButtonClickMargin)
                {
                   _timestamps.Add(new Timestamp(XToTime(mouse.X), idx, 0)); // TODO: Insert current value here
                }
            }
        }

        // Draw playhead
        Raylib.DrawLine(TimeToX(_time), (int)Bounds.Y + _margin, TimeToX(_time),
            (int)Bounds.y + (int)Bounds.height - _margin, _playheadColor);
    }

    private int TimeToX(float time)
    {
        return (int)Map(time, _timeStart, _timeEnd,Bounds.x + _margin, Bounds.x + Bounds.width - _margin);
    }

    private float XToTime(float x)
    {
        float rescaledX = x - Bounds.X - _margin;
        return Map(rescaledX , 0, Bounds.width - 2* _margin, _timeStart, _timeEnd);
    }
    
    private int CalculateTrackY(int trackNumber)
    {
        float trackYdistance = (Bounds.height - 2 * _margin) / 3;
        return (int) (Bounds.y + _margin + trackNumber * trackYdistance - .5 * trackYdistance);
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