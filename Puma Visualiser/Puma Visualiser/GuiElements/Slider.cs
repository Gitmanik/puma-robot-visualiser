using Raylib_CsLo;

namespace Puma_Visualiser.GuiElements;

public class Slider : GuiElement
{
    private float _value;

    private readonly float _minValue, _maxValue;
    private readonly float _height;
    private readonly string _minText, _maxText;
    private readonly int _textPadding;
    private readonly int _minTextWidth;
    private readonly int _maxTextWidth;

    public Slider(Rectangle bounds, string minText, string maxText, float minValue, float maxValue, float value,
        float height) : base(bounds)
    {
        _minText = minText;
        _maxText = maxText;
        _minValue = minValue;
        _maxValue = maxValue;
        _height = height;
        _value = value;

        int defaultFontSize = RayGui.GuiGetStyle(0, 16);
        _textPadding = RayGui.GuiGetStyle(4, 13);
        _minTextWidth = Raylib.MeasureText(_minText, defaultFontSize);
        _maxTextWidth = Raylib.MeasureText(_maxText, defaultFontSize);
    }

    public void Draw()
    {
        base.Draw();
        
        _value = RayGui.GuiSlider(
            new Rectangle(Bounds.x + _minTextWidth + _textPadding, Bounds.y, Bounds.width - _minTextWidth - _maxTextWidth - _textPadding*2,_height), _minText, _maxText, _value, _minValue, _maxValue);
    }

    public float GetValue() => _value;
}