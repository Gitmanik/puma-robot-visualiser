using Puma_Visualiser.GuiElements;
using Raylib_CsLo;

namespace Puma_Visualiser.Views;

internal class MainView : IView
{
    private int RightPanelStart = -250;
    private int RightPanelInputStartY = 100;

    private List<GuiElement> _textBoxes = new List<GuiElement>();

    private Timeline _timeline;

    public MainView()
    {
        _timeline = new Timeline(new Rectangle(0, -250, -250, 250));
        _textBoxes.Add(new TextBox(new Rectangle(RightPanelStart + 40, RightPanelInputStartY + 10, 200, 30), 20));
        _textBoxes.Add(new TextBox(new Rectangle(RightPanelStart + 40, RightPanelInputStartY + 50, 200, 30), 20));
        _textBoxes.Add(new TextBox(new Rectangle(RightPanelStart + 40, RightPanelInputStartY + 90, 200, 30), 20));
        _textBoxes.Add(new TextBox(new Rectangle(RightPanelStart + 40, RightPanelInputStartY + 130, 200, 30), 20));

        _textBoxes.Add(new Label(new Rectangle(RightPanelStart + 10, RightPanelInputStartY + 10 + 5, 0, 0), "X", 20,
            Raylib.BLACK));
        _textBoxes.Add(new Label(new Rectangle(RightPanelStart + 10, RightPanelInputStartY + 50 + 5, 0, 0), "Y", 20,
            Raylib.BLACK));
        _textBoxes.Add(new Label(new Rectangle(RightPanelStart + 10, RightPanelInputStartY + 90 + 5, 0, 0), "Z", 20,
            Raylib.BLACK));
        _textBoxes.Add(new Label(new Rectangle(RightPanelStart + 10, RightPanelInputStartY + 130 + 5, 0, 0), "t", 20,
            Raylib.BLACK));
    }

    public void Draw()
    {
        Raylib.ClearBackground(Raylib.LIGHTGRAY);

        Raylib.DrawRectangle(Raylib.GetScreenWidth() + RightPanelStart, 0, 250, Raylib.GetScreenHeight(), Raylib.WHITE);

        foreach (var item in _textBoxes)
            item.Draw();

        _timeline.Draw();
    }
}