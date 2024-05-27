using System.Diagnostics;
using System.Numerics;
using Puma_Visualiser.GuiElements;
using Raylib_CsLo;

namespace Puma_Visualiser.Views;

internal class MainView : IView
{
    private int RightPanelStart = -250;
    private int margin = 10;
    private int RightPanelInputStartY = 100;

    private List<GuiElement> _textBoxes = new List<GuiElement>();

    private Timeline _timeline;

    private RenderTexture? robotTexture;
    private Camera3D robotCamera;

    private float _time;
    private float _timeEnd = 5;
    private float _timeStart = 0;
    private float _timeScale = 1;

    public MainView()
    {
        _timeline = new Timeline(new Rectangle(0, -250, -250, 250), Raylib.GRAY, Raylib.LIGHTGRAY, Raylib.DARKGRAY,
            Raylib.RED, 10, 10);
        _textBoxes.Add(new TextBox(
            new Rectangle(RightPanelStart + margin + 40, RightPanelInputStartY + 10, 200 - 2 * margin, 30), 20));
        _textBoxes.Add(new TextBox(
            new Rectangle(RightPanelStart + margin + 40, RightPanelInputStartY + 50, 200 - 2 * margin, 30), 20));
        _textBoxes.Add(new TextBox(
            new Rectangle(RightPanelStart + margin + 40, RightPanelInputStartY + 90, 200 - 2 * margin, 30), 20));
        _textBoxes.Add(new TextBox(
            new Rectangle(RightPanelStart + margin + 40, RightPanelInputStartY + 130, 200 - 2 * margin, 30), 20));

        _textBoxes.Add(new Label(new Rectangle(RightPanelStart + margin + 10, RightPanelInputStartY + 10 + 5, 0, 0),
            "X", 20,
            Raylib.BLACK));
        _textBoxes.Add(new Label(new Rectangle(RightPanelStart + margin + 10, RightPanelInputStartY + 50 + 5, 0, 0),
            "Y", 20,
            Raylib.BLACK));
        _textBoxes.Add(new Label(new Rectangle(RightPanelStart + margin + 10, RightPanelInputStartY + 90 + 5, 0, 0),
            "Z", 20,
            Raylib.BLACK));
        _textBoxes.Add(new Label(new Rectangle(RightPanelStart + margin + 10, RightPanelInputStartY + 130 + 5, 0, 0),
            "t", 20,
            Raylib.BLACK));

        InitializeRobot();
    }

    public void Draw()
    {
        if (Raylib.IsWindowResized())
            InitializeRobot();
        Debug.Assert(robotTexture != null, nameof(robotTexture) + " != null");

        // 3D DRAWING

        Raylib.BeginTextureMode(robotTexture.Value);
        Raylib.ClearBackground(Raylib.LIGHTGRAY);
        Raylib.BeginMode3D(robotCamera);

        Raylib.DrawGrid(10, 1.0f);

        Raylib.DrawCube(new Vector3(0, 0.5f, 0), 1, 1, 1, Raylib.GOLD);

        Raylib.EndMode3D();
        Raylib.EndTextureMode();


        // GUI DRAWING

        Raylib.BeginDrawing();

        Raylib.ClearBackground(Raylib.GRAY);

        Raylib.DrawRectangle(Raylib.GetScreenWidth() + RightPanelStart, 0, 250, Raylib.GetScreenHeight(), Raylib.GRAY);
        Raylib.DrawRectangle(Raylib.GetScreenWidth() + RightPanelStart + 10, 10, 230, Raylib.GetScreenHeight() - 20,
            Raylib.LIGHTGRAY);

        foreach (var item in _textBoxes)
            item.Draw();

        // Time
        _timeline.SetTimeData(_time, _timeStart, _timeEnd);
        _timeline.Draw();

        _time += Raylib.GetFrameTime() * _timeScale;
        if (_time > _timeEnd) _time = _timeStart;

        Raylib.DrawTexture(robotTexture.Value.texture, margin, margin, Raylib.WHITE);
        Raylib.EndDrawing();
    }

    private void InitializeRobot()
    {
        if (robotTexture.HasValue)
            Raylib.UnloadRenderTexture(robotTexture.Value);

        // Robot render
        robotTexture = Raylib.LoadRenderTexture(Raylib.GetScreenWidth() + RightPanelStart - 2 * margin,
            Raylib.GetScreenHeight() - 250 - 2 * margin);
        robotCamera = new Camera3D(new Vector3(5, 5, 5), Vector3.Zero, -Vector3.UnitY, 45f,
            CameraProjection.CAMERA_PERSPECTIVE);
    }
}