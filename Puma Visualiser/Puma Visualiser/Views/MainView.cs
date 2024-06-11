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

    private Model p1, p2, p3, p4, p5, p6;
    private Vector3 modelScale = new Vector3(1f, 1f, 1f);

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
        
        DrawRobot();
        
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
        
        //loading .obj files
        p1 = Raylib.LoadModel("resources\\p0.obj");
        p2 = Raylib.LoadModel("resources\\p1.obj");
        p3 = Raylib.LoadModel("resources\\p2.obj");
        p4 = Raylib.LoadModel("resources\\p3.obj");
        p5 = Raylib.LoadModel("resources\\p4.obj");
        p6 = Raylib.LoadModel("resources\\p5.obj");
    }

    private float theta1 = 0.0f, theta2 = 0.0f, theta3 = 0.0f, theta4 = 0.0f, theta5 = 0.0f, theta6 = 0.0f;
    private void DrawRobot()
    {
        //translation of DH position
 
        float pos03x = 41.917f * MathF.Cos(RayMath.DEG2RAD * theta1) * MathF.Cos(RayMath.DEG2RAD * theta2) - 12.92f * MathF.Sin(RayMath.DEG2RAD * theta1);
        float pos03y = 41.917f * MathF.Sin(RayMath.DEG2RAD * theta1) * MathF.Cos(RayMath.DEG2RAD * theta2) + 12.92f * MathF.Cos(RayMath.DEG2RAD * theta1);
        float pos03z = 61.722f + 41.917f * MathF.Sin(RayMath.DEG2RAD * theta2);

               /* prawie git
        float pos06x = pos03x + 43.215f * (MathF.Cos(RayMath.DEG2RAD * theta1) * MathF.Cos(RayMath.DEG2RAD * theta2) * MathF.Sin(RayMath.DEG2RAD * theta3) + MathF.Cos(RayMath.DEG2RAD * theta1) * MathF.Sin(RayMath.DEG2RAD * theta2) * MathF.Cos(RayMath.DEG2RAD * theta3));
        float pos06y = pos03y + 43.215f * (MathF.Sin(RayMath.DEG2RAD * theta1) * MathF.Cos(RayMath.DEG2RAD * theta2) * MathF.Sin(RayMath.DEG2RAD * theta3) + MathF.Sin(RayMath.DEG2RAD * theta1) * MathF.Sin(RayMath.DEG2RAD * theta2) * MathF.Cos(RayMath.DEG2RAD * theta3));
        float pos06z = pos03z + 43.215f * (MathF.Sin(RayMath.DEG2RAD * theta2) * MathF.Sin(RayMath.DEG2RAD * theta3) - MathF.Cos(RayMath.DEG2RAD * theta2) * MathF.Cos(RayMath.DEG2RAD * theta3));
       */

        float pos06x = pos03x + 43.215f * (MathF.Cos(RayMath.DEG2RAD * theta1) * MathF.Cos(RayMath.DEG2RAD * theta2) * MathF.Sin(RayMath.DEG2RAD * theta3) + MathF.Cos(RayMath.DEG2RAD * theta1) * MathF.Sin(RayMath.DEG2RAD * theta2) * MathF.Cos(RayMath.DEG2RAD * theta3));
        float pos06y = pos03y + 43.215f * (MathF.Sin(RayMath.DEG2RAD * theta1) * MathF.Cos(RayMath.DEG2RAD * theta2) * MathF.Sin(RayMath.DEG2RAD * theta3) + MathF.Sin(RayMath.DEG2RAD * theta1) * MathF.Sin(RayMath.DEG2RAD * theta2) * MathF.Cos(RayMath.DEG2RAD * theta3));
        float pos06z = pos03z + 43.215f * (MathF.Sin(RayMath.DEG2RAD * theta2) * MathF.Sin(RayMath.DEG2RAD * theta3) - MathF.Cos(RayMath.DEG2RAD * theta2) * MathF.Cos(RayMath.DEG2RAD * theta3));

        //rotating
        rotate();

        //drawing 
        Raylib.DrawModelEx(p1, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0f, modelScale, Raylib.BLACK);
        Raylib.DrawModelEx(p2, new Vector3(0, 61.722f, 0), new Vector3(0, 1, 0), 90f, modelScale, Raylib.GREEN);
        Raylib.DrawModelEx(p3, new Vector3(0, 61.722f, 0), new Vector3(0, 1, 0), 90f, modelScale, Raylib.BLACK);
        Raylib.DrawModelEx(p4, new Vector3(pos03x, pos03z, pos03y), new Vector3(0, 1, 0), 90f, modelScale, Raylib.GREEN);
        Raylib.DrawModelEx(p5, new Vector3(pos06x, pos06z, pos06y), new Vector3(0, 1, 0), 90f, modelScale, Raylib.BLACK);
        Raylib.DrawModelEx(p6, new Vector3(pos06x, pos06z, pos06y), new Vector3(0, 1, 0), 90f, modelScale, Raylib.GREEN);
    }

    
    private void rotate()
    {
        if (Raylib.IsKeyDown(KeyboardKey.KEY_S)) theta1 -= 10.0f;
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_A)) theta1 += 10.0f;
        
        if (Raylib.IsKeyDown(KeyboardKey.KEY_Z)) theta2 -= 10.0f;
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_X)) theta2 += 10.0f;

        if (Raylib.IsKeyDown(KeyboardKey.KEY_C)) theta3 -= 10.0f;
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_V)) theta3 += 10.0f;

        if (Raylib.IsKeyDown(KeyboardKey.KEY_D)) theta4 -= 10.0f;
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_F)) theta4 += 10.0f;

        if (Raylib.IsKeyDown(KeyboardKey.KEY_B)) theta5 -= 10.0f;
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_N)) theta5 += 10.0f;

        if (Raylib.IsKeyDown(KeyboardKey.KEY_G)) theta6 -= 10.0f;
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_H)) theta6 += 10.0f;

        //axis rotation points
        /*p3.transform = RayMath.MatrixTranslate(0, 0, 0);
        p4.transform = RayMath.MatrixTranslate(0, 0, 0);
        p5.transform = RayMath.MatrixTranslate(0, 0, 0);
        p6.transform = RayMath.MatrixTranslate(0, 0, 0);*/

        p2.transform = RayMath.MatrixRotateXYZ(new Vector3(0f, RayMath.DEG2RAD * theta1, 0f));
        p3.transform = RayMath.MatrixRotateXYZ(new Vector3(RayMath.DEG2RAD * theta2, RayMath.DEG2RAD * theta1, 0f));
        p4.transform = RayMath.MatrixRotateXYZ(new Vector3(RayMath.DEG2RAD * theta2 , RayMath.DEG2RAD * theta1 + RayMath.DEG2RAD * theta3, 0f));
        p5.transform = RayMath.MatrixRotateXYZ(new Vector3(RayMath.DEG2RAD * theta2 + RayMath.DEG2RAD * theta3, RayMath.DEG2RAD * theta1 + RayMath.DEG2RAD * theta4, 0f));
        p6.transform = RayMath.MatrixRotateXYZ(new Vector3(RayMath.DEG2RAD * theta2 + RayMath.DEG2RAD * theta3 + RayMath.DEG2RAD * theta5, RayMath.DEG2RAD * theta1 + RayMath.DEG2RAD * theta4, 0f));
        //if(isitemgrabbed(true)) item.transform = RayMath.MatrixRotateXYZ(new Vector3(RayMath.DEG2RAD * theta2 + RayMath.DEG2RAD * theta3 RayMath.DEG2RAD * theta5, RayMath.DEG2RAD * theta1 + RayMath.DEG2RAD * theta4RayMath.DEG2RAD * theta6, 0f));


    }
}