using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using Puma_Visualiser.GuiElements;
using Raylib_CsLo;

namespace Puma_Visualiser.Views;

internal class MainView : IView
{
    private int RightPanelStart = -250;
    private int margin = 10;
    private int RightPanelInputStartY = 100;

    private List<GuiElement> _textBoxes = new List<GuiElement>();

    private Button _startButton;
    private Button _stopButton;
    private Slider _timeSlider;

    private Timeline _timeline;

    private RenderTexture? robotTexture;
    private Camera3D robotCamera;

    private float _time;
    private float _timeEnd = 5;
    private float _timeStart = 0;
    private float _timeScale = 1;
    private float _minTimeScale = 0, _maxTimeScale = 5;
    private bool _timeStopped = false;

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

        _textBoxes.Add(new Label(new Rectangle(RightPanelStart + margin + 10, RightPanelInputStartY - 40, -RightPanelInputStartY, 40), "Current", 40, Raylib.BLACK));
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

        _textBoxes.Add(new Label(new Rectangle(RightPanelStart + margin + 10, RightPanelInputStartY + 130 + 20 + 40, -RightPanelInputStartY, 40), "Timestamp", 40, Raylib.BLACK));
        _textBoxes.Add(new Label(new Rectangle(RightPanelStart + margin + 10, RightPanelInputStartY + 10 + 5 + 130 + 20 + 40 + 40, 0, 0),
            "X", 20,
            Raylib.BLACK));
        _textBoxes.Add(new Label(new Rectangle(RightPanelStart + margin + 10, RightPanelInputStartY + 50 + 5 + 130 + 20 + 40 + 40, 0, 0),
            "Y", 20,
            Raylib.BLACK));
        _textBoxes.Add(new Label(new Rectangle(RightPanelStart + margin + 10, RightPanelInputStartY + 90 + 5 + 130 + 20 + 40 + 40, 0, 0),
            "Z", 20,
            Raylib.BLACK));
        _textBoxes.Add(new Label(new Rectangle(RightPanelStart + margin + 10, RightPanelInputStartY + 130 + 5 + 130 + 20 + 40 + 40, 0, 0),
            "t", 20,
            Raylib.BLACK));

        _textBoxes.Add(new TextBox(
            new Rectangle(RightPanelStart + margin + 40, RightPanelInputStartY + 10 + 130 + 20 + 40 + 40, 200 - 2 * margin, 30), 20));
        _textBoxes.Add(new TextBox(
            new Rectangle(RightPanelStart + margin + 40, RightPanelInputStartY + 50 + 130 + 20 + 40 + 40, 200 - 2 * margin, 30), 20));
        _textBoxes.Add(new TextBox(
            new Rectangle(RightPanelStart + margin + 40, RightPanelInputStartY + 90 + 130 + 20 + 40 + 40, 200 - 2 * margin, 30), 20));
        _textBoxes.Add(new TextBox(
            new Rectangle(RightPanelStart + margin + 40, RightPanelInputStartY + 130 + 130 + 20 + 40 + 40, 200 - 2 * margin, 30), 20));

        _startButton = new Button(new Rectangle(RightPanelStart + margin + 10, -10 - margin - 40 - 10 - 40, -RightPanelStart - 3 * margin - 10, 40), "START");
        _stopButton = new Button(new Rectangle(RightPanelStart + margin + 10, -10 - margin - 40, -RightPanelStart - 3 * margin - 10, 40), "STOP");
        _timeSlider = new Slider(
            new Rectangle(RightPanelStart + margin + 10, -10 - margin - 40 - 40 - 30 - 10,
                -RightPanelStart - 3 * margin - 10, 20), _minTimeScale.ToString(CultureInfo.InvariantCulture),
            _maxTimeScale.ToString(CultureInfo.InvariantCulture), _minTimeScale, _maxTimeScale, _timeScale, 20);

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

        Raylib.DrawGrid(250, 10f);

        //Raylib.DrawCube(new Vector3(0, 0.5f, 0), 1, 1, 1, Raylib.GOLD);
        
        InitializeModel();
        
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

        _startButton.Draw();
        _stopButton.Draw();

        if (_startButton.IsClicked())
            _timeStopped = false;
        if (_stopButton.IsClicked())
            _timeStopped = true;

        if (!_timeStopped)
            _time += Raylib.GetFrameTime() * _timeScale;
        if (_time > _timeEnd) _time = _timeStart;

        Raylib.DrawTexture(robotTexture.Value.texture, margin, margin, Raylib.WHITE);

        _timeSlider.Draw();
        _timeScale = _timeSlider.GetValue();

        Raylib.EndDrawing();
    }

    private void InitializeRobot()
    {
        if (robotTexture.HasValue)
            Raylib.UnloadRenderTexture(robotTexture.Value);

        // Robot render
        robotTexture = Raylib.LoadRenderTexture(Raylib.GetScreenWidth() + RightPanelStart - 2 * margin,
            Raylib.GetScreenHeight() - 250 - 2 * margin);
        robotCamera = new Camera3D(new Vector3(200, 200, 200), Vector3.Zero, -Vector3.UnitY, 45f,
            CameraProjection.CAMERA_PERSPECTIVE);
    }
    private void InitializeModel()
    {
        //loading .obj files
        Model p1 = Raylib.LoadModel("resources\\no_Part1.obj");
        Model p2 = Raylib.LoadModel("resources\\no_Part2.obj");
        Model p3 = Raylib.LoadModel("resources\\no_Part3.obj");
        Model p4 = Raylib.LoadModel("resources\\no_Part4.obj");
        Model p5 = Raylib.LoadModel("resources\\no_Part5.obj");
        Model p6 = Raylib.LoadModel("resources\\no_Part6.obj");

        

            //rotating
            rotate1(p2, p3, p4, p5, p6);

            //drawing 
            Raylib.DrawModelEx(p1, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0f, new Vector3(0.1f, 0.1f, 0.1f), Raylib.BLACK);
            Raylib.DrawModelEx(p2, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0f, new Vector3(0.1f, 0.1f, 0.1f), Raylib.GREEN);
            Raylib.DrawModelEx(p3, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0f, new Vector3(0.1f, 0.1f, 0.1f), Raylib.BLACK);
            Raylib.DrawModelEx(p4, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0f, new Vector3(0.1f, 0.1f, 0.1f), Raylib.GREEN);
            Raylib.DrawModelEx(p5, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0f, new Vector3(0.1f, 0.1f, 0.1f), Raylib.BLACK);
            Raylib.DrawModelEx(p6, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0f, new Vector3(0.1f, 0.1f, 0.1f), Raylib.GREEN);


        /* old model (xy!=0)
        Model part1 = Raylib.LoadModel("resources\\Part1.obj");
        Raylib.DrawModel(part1, new Vector3(0.56025f, 0, -0.142875f), 0.0025f, Raylib.BLACK);

        Model part2 = Raylib.LoadModel("resources\\Part2.obj");
        Raylib.DrawModel(part2, new Vector3(0.56025f, 0, -0.142875f), 0.0025f, Raylib.GREEN);

        Model part3 = Raylib.LoadModel("resources\\Part3.obj");
        Raylib.DrawModel(part3, new Vector3(0.56025f, 0, -0.142875f), 0.0025f, Raylib.BLACK);

        Model part4 = Raylib.LoadModel("resources\\Part4.obj");
        Raylib.DrawModel(part4, new Vector3(0.56025f, 0, -0.142875f), 0.0025f, Raylib.GREEN);

        Model part5 = Raylib.LoadModel("resources\\Part5.obj");
        Raylib.DrawModel(part5, new Vector3(0.56025f, 0, -0.142875f), 0.0025f, Raylib.BLACK);

        Model part6 = Raylib.LoadModel("resources\\Part6.obj");
        Raylib.DrawModel(part6, new Vector3(0.56025f, 0, -0.142875f), 0.0025f, Raylib.GREEN);
        */
    }
    private void rotate1(Model p2, Model p3, Model p4, Model p5, Model p6)
    {
        float yaw = 0.0f;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) yaw -= 1.0f;
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) yaw += 1.0f;

        //axis rotation points
        p2.transform = RayMath.MatrixTranslate(0, 0, 0);
        p3.transform = RayMath.MatrixTranslate(0, 0, 0);
        p4.transform = RayMath.MatrixTranslate(0, 0, 0);
        p5.transform = RayMath.MatrixTranslate(0, 0, 0);
        p6.transform = RayMath.MatrixTranslate(0, 0, 0);

        p2.transform = RayMath.MatrixRotateXYZ(new Vector3(0f, RayMath.DEG2RAD * yaw, 0f));
        p2.transform = RayMath.MatrixRotateXYZ(new Vector3(0f, RayMath.DEG2RAD * yaw, 0f));
        p2.transform = RayMath.MatrixRotateXYZ(new Vector3(0f, RayMath.DEG2RAD * yaw, 0f));
        p2.transform = RayMath.MatrixRotateXYZ(new Vector3(0f, RayMath.DEG2RAD * yaw, 0f));
        p2.transform = RayMath.MatrixRotateXYZ(new Vector3(0f, RayMath.DEG2RAD * yaw, 0f));

    }
}