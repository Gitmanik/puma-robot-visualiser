using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using Puma_Visualiser.GuiElements;
using Raylib_CsLo;

namespace Puma_Visualiser.Views;

internal class MainView : IView
{
    private int RightPanelStart = -250;
    private int GuiMargin = 10;
    private int RightPanelInputStartY = 100;

    private readonly List<GuiElement> _guiElements = new List<GuiElement>();

    private readonly TextBox _currentXTextBox, _currentYTextBox, _currentZTextBox, _currentTimeTextBox;
    private readonly TextBox _targetXTextBox, _targetYTextBox, _targetZTextBox, _targetTimeTextBox;
    
    private readonly Button _startButton, _stopButton;
    private readonly Slider _timeSlider;

    private readonly Timeline _timeline;

    private RenderTexture? robotTexture;
    private Camera3D robotCamera;

    private float _time;
    private float _timeEnd = 5;
    private float _timeStart = 0;
    private float _timeScale = 1;
    private float _minTimeScale = 0, _maxTimeScale = 5;
    private bool _timeStopped = false;

    private Model p1, p2, p3, p4, p5, p6, cubeModel;
    private Texture p1t, p2t, p3t, p4t, p5t, p6t;
    private Vector3 modelScale = new Vector3(1f, 1f, 1f);

    public MainView()
    {
        _timeline = new Timeline(new Rectangle(0, -250, -250, 250), Raylib.GRAY, Raylib.LIGHTGRAY, Raylib.DARKGRAY,
            Raylib.RED, 10, 10);
        _guiElements.Add(_timeline);
        
        
        _guiElements.Add(new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY + 130 + 20 + 40, -RightPanelInputStartY, 40), "Timestamp", 40, Raylib.BLACK));
        _guiElements.Add(new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY + 10 + 5 + 130 + 20 + 40 + 40, 0, 0),
            "X", 20,
            Raylib.BLACK));
        _guiElements.Add(new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY + 50 + 5 + 130 + 20 + 40 + 40, 0, 0),
            "Y", 20,
            Raylib.BLACK));
        _guiElements.Add(new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY + 90 + 5 + 130 + 20 + 40 + 40, 0, 0),
            "Z", 20,
            Raylib.BLACK));
        _guiElements.Add(new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY + 130 + 5 + 130 + 20 + 40 + 40, 0, 0),
            "t", 20,
            Raylib.BLACK));

        /* Current values Gui Elements */
        
        _guiElements.Add(new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY - 40, -RightPanelInputStartY, 40), "Current", 40, Raylib.BLACK));
        _guiElements.Add(new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY + 10 + 5, 0, 0),
            "X", 20,
            Raylib.BLACK));
        _guiElements.Add(new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY + 50 + 5, 0, 0),
            "Y", 20,
            Raylib.BLACK));
        _guiElements.Add(new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY + 90 + 5, 0, 0),
            "Z", 20,
            Raylib.BLACK));
        _guiElements.Add(new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY + 130 + 5, 0, 0),
            "t", 20,
            Raylib.BLACK));
        
        _currentXTextBox = new TextBox(
            new Rectangle(RightPanelStart + GuiMargin + 40, RightPanelInputStartY + 10, 200 - 2 * GuiMargin, 30), 20);
        _guiElements.Add(_currentXTextBox);
        
        _currentYTextBox = new  TextBox(
            new Rectangle(RightPanelStart + GuiMargin + 40, RightPanelInputStartY + 50, 200 - 2 * GuiMargin, 30), 20);
        _guiElements.Add(_currentYTextBox);
        
        _currentZTextBox = new TextBox(
            new Rectangle(RightPanelStart + GuiMargin + 40, RightPanelInputStartY + 90, 200 - 2 * GuiMargin, 30), 20);
        _guiElements.Add(_currentZTextBox);
        
        _currentTimeTextBox = new TextBox(
            new Rectangle(RightPanelStart + GuiMargin + 40, RightPanelInputStartY + 130, 200 - 2 * GuiMargin, 30), 20);
        _guiElements.Add(_currentTimeTextBox);
        
        
        /* Target values Gui Elements */
        
        _targetXTextBox = new TextBox(
            new Rectangle(RightPanelStart + GuiMargin + 40, RightPanelInputStartY + 10 + 130 + 20 + 40 + 40, 200 - 2 * GuiMargin, 30), 20);
        _guiElements.Add(_targetXTextBox);
        
        _targetYTextBox = new TextBox(
            new Rectangle(RightPanelStart + GuiMargin + 40, RightPanelInputStartY + 50 + 130 + 20 + 40 + 40, 200 - 2 * GuiMargin, 30), 20);
        _guiElements.Add(_targetYTextBox);
        
        _targetZTextBox = new TextBox(
            new Rectangle(RightPanelStart + GuiMargin + 40, RightPanelInputStartY + 90 + 130 + 20 + 40 + 40, 200 - 2 * GuiMargin, 30), 20);
        _guiElements.Add(_targetZTextBox);
        
        _targetTimeTextBox = new  TextBox(
            new Rectangle(RightPanelStart + GuiMargin + 40, RightPanelInputStartY + 130 + 130 + 20 + 40 + 40, 200 - 2 * GuiMargin, 30), 20);
        _guiElements.Add(_targetTimeTextBox);

        
        _startButton = new Button(new Rectangle(RightPanelStart + GuiMargin + 10, -10 - GuiMargin - 40 - 10 - 40, -RightPanelStart - 3 * GuiMargin - 10, 40), "START");
        _guiElements.Add(_startButton);
        
        _stopButton = new Button(new Rectangle(RightPanelStart + GuiMargin + 10, -10 - GuiMargin - 40, -RightPanelStart - 3 * GuiMargin - 10, 40), "STOP");
        _guiElements.Add(_stopButton);
        
        _timeSlider = new Slider(
            new Rectangle(RightPanelStart + GuiMargin + 10, -10 - GuiMargin - 40 - 40 - 30 - 10,
                -RightPanelStart - 3 * GuiMargin - 10, 20), _minTimeScale.ToString(CultureInfo.InvariantCulture),
            _maxTimeScale.ToString(CultureInfo.InvariantCulture), _minTimeScale, _maxTimeScale, _timeScale, 20);
        _guiElements.Add(_timeSlider);
        
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
        
        ManualControl();

        float xTarget = 0, yTarget = 0, zTarget = 0;
        if (!string.IsNullOrWhiteSpace(_targetXTextBox.Text))
        {
            float.TryParse(_targetXTextBox.Text, out xTarget);
        }        
        if (!string.IsNullOrWhiteSpace(_targetYTextBox.Text))
        {
            float.TryParse(_targetYTextBox.Text, out yTarget);
        }        
        if (!string.IsNullOrWhiteSpace(_targetZTextBox.Text))
        {
            float.TryParse(_targetZTextBox.Text, out zTarget);
        }

        Raylib.DrawCube(new Vector3(xTarget, yTarget, zTarget), 10,10,10, Raylib.GOLD);
        
        DrawRobot();
        
        Raylib.EndMode3D();
        Raylib.EndTextureMode();


        // TIME
        
        _timeScale = _timeSlider.GetValue();
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
        
        // GUI DRAWING

        Raylib.BeginDrawing();

        Raylib.ClearBackground(Raylib.GRAY);

        Raylib.DrawRectangle(Raylib.GetScreenWidth() + RightPanelStart, 0, 250, Raylib.GetScreenHeight(), Raylib.GRAY);
        Raylib.DrawRectangle(Raylib.GetScreenWidth() + RightPanelStart + 10, 10, 230, Raylib.GetScreenHeight() - 20,
            Raylib.LIGHTGRAY);
        
        foreach (var item in _guiElements)
            item.Draw();
        
        Raylib.DrawTexture(robotTexture.Value.texture, GuiMargin, GuiMargin, Raylib.WHITE);
        
        Raylib.EndDrawing();
    }

    private void InitializeRobot()
    {
        if (robotTexture.HasValue)
            Raylib.UnloadRenderTexture(robotTexture.Value);

        // Robot render 
        robotTexture = Raylib.LoadRenderTexture(Raylib.GetScreenWidth() + RightPanelStart - 2 * GuiMargin,
            Raylib.GetScreenHeight() - 250 - 2 * GuiMargin);
        robotCamera = new Camera3D(new Vector3(200, 200, 200), Vector3.Zero, -Vector3.UnitY, 45f,
            CameraProjection.CAMERA_PERSPECTIVE);
        
        //loading .obj files
        p1 = Raylib.LoadModel("resources\\p0.obj");
        p2 = Raylib.LoadModel("resources\\p1.obj");
        p3 = Raylib.LoadModel("resources\\p2.obj");
        p4 = Raylib.LoadModel("resources\\p3.obj");
        p5 = Raylib.LoadModel("resources\\p4.obj");
        p6 = Raylib.LoadModel("resources\\p5.obj");
        cubeModel = Raylib.LoadModel("resources\\kostka.obj");
        
        p1t = Raylib.LoadTexture("resources\\p1.png");
        Raylib.SetTextureWrap(p1t, TextureWrap.TEXTURE_WRAP_CLAMP);
        
        unsafe
        {
            p1.materials[0].maps[0].texture = p1t;
        }
    }

    private float _theta1 = 0.0f, _theta2 = 0.0f, _theta3 = 0.0f, _theta4 = 0.0f, _theta5 = 0.0f, _theta6 = 0.0f;
    private bool _cubeGrabbed = true;

    private void DrawRobot()
    {
        //translation of DH position
        float pos03x = 41.917f * MathF.Cos(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) - 12.92f * MathF.Sin(RayMath.DEG2RAD * _theta1);
        float pos03y = 41.917f * MathF.Sin(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) + 12.92f * MathF.Cos(RayMath.DEG2RAD * _theta1);
        float pos03z = 61.722f + 41.917f * MathF.Sin(RayMath.DEG2RAD * _theta2);

        float pos06x = pos03x + 43.215f * (MathF.Cos(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) * MathF.Sin(RayMath.DEG2RAD * _theta3) + MathF.Cos(RayMath.DEG2RAD * _theta1) * MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3));
        float pos06y = pos03y + 43.215f * (MathF.Sin(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) * MathF.Sin(RayMath.DEG2RAD * _theta3) + MathF.Sin(RayMath.DEG2RAD * _theta1) * MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3));
        float pos06z = pos03z + 43.215f * (MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Sin(RayMath.DEG2RAD * _theta3) - MathF.Cos(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3));
        
        float pos7x = pos06x + MathF.Cos(RayMath.DEG2RAD * _theta5) * 5.334f * (MathF.Cos(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) * MathF.Sin(RayMath.DEG2RAD * _theta3) + MathF.Cos(RayMath.DEG2RAD * _theta1) * MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3));
        float pos7y = pos06y + MathF.Cos(RayMath.DEG2RAD * _theta5) * 5.334f * (MathF.Sin(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) * MathF.Sin(RayMath.DEG2RAD * _theta3) + MathF.Sin(RayMath.DEG2RAD * _theta1) * MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3));
        float pos7z = pos06z + MathF.Cos(RayMath.DEG2RAD * _theta5) * 5.334f * (MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Sin(RayMath.DEG2RAD * _theta3) - MathF.Cos(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3));
       
        float pos07x = pos7x + 5.334f * MathF.Cos(RayMath.DEG2RAD * _theta4) * MathF.Sin(RayMath.DEG2RAD * _theta5) * (MathF.Cos(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3) + MathF.Cos(RayMath.DEG2RAD * _theta1) * MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Sin(RayMath.DEG2RAD * _theta3)) - 5.334f * MathF.Sin(RayMath.DEG2RAD * _theta4) * MathF.Sin(RayMath.DEG2RAD * _theta5) * MathF.Sin(RayMath.DEG2RAD * _theta1);
        float pos07y = pos7y + 5.334f * MathF.Cos(RayMath.DEG2RAD * _theta4) * MathF.Sin(RayMath.DEG2RAD * _theta5) * (MathF.Sin(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3) - MathF.Sin(RayMath.DEG2RAD * _theta1) * MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Sin(RayMath.DEG2RAD * _theta3)) + 5.334f * MathF.Sin(RayMath.DEG2RAD * _theta4) * MathF.Sin(RayMath.DEG2RAD * _theta5) * MathF.Cos(RayMath.DEG2RAD * _theta1);
        float pos07z = pos7z + 5.334f * MathF.Cos(RayMath.DEG2RAD * _theta4) * MathF.Sin(RayMath.DEG2RAD * _theta5) * (MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3) + MathF.Cos(RayMath.DEG2RAD * _theta2) * MathF.Sin(RayMath.DEG2RAD * _theta3));
        
        Vector3 pos03 = new Vector3(pos03x, pos03z, pos03y);
        Vector3 pos06 = new Vector3(pos06x, pos06z, pos06y);
        Vector3 pos07 = new Vector3(pos07x, pos07z, pos07y); // wspolrzedne koncowki do obrotu bryla przenoszona 

        _currentXTextBox.Text = "" + pos06x;
        _currentYTextBox.Text = "" + pos06y;
        _currentZTextBox.Text = "" + pos06z;
        
        //rotating
        RotateRobot();

        Raylib.DrawModelEx(p1, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0f, modelScale, Raylib.WHITE);
        Raylib.DrawModelEx(p2, new Vector3(0, 61.722f, 0), new Vector3(0, 1, 0), 90f, modelScale, Raylib.GREEN);
        Raylib.DrawModelEx(p3, new Vector3(0, 61.722f, 0), new Vector3(0, 1, 0), 90f, modelScale, Raylib.BLACK);
        Raylib.DrawModelEx(p4, pos03, new Vector3(0, 1, 0), 90f, modelScale, Raylib.GREEN);
        Raylib.DrawModelEx(p5, pos06, new Vector3(0, 1, 0), -90f, modelScale, Raylib.BLACK);
        Raylib.DrawModelEx(p6, pos06, new Vector3(0, 1, 0), 90f, modelScale, Raylib.GREEN);
        Raylib.DrawModelEx(cubeModel, pos07, new Vector3(0, 1, 0), 90f, new Vector3(5, 5, 5), Raylib.GREEN); //bryla do poprawnego ustawienia obrotu bryly przenoszonej
    }

    
    private void RotateRobot()
    {
        p2.transform = RayMath.MatrixRotateXYZ(new Vector3(0f, RayMath.DEG2RAD * _theta1, 0f));
        p3.transform = RayMath.MatrixRotateXYZ(new Vector3(RayMath.DEG2RAD * _theta2, RayMath.DEG2RAD * _theta1, 0f));
        p4.transform = RayMath.MatrixRotateXYZ(new Vector3(RayMath.DEG2RAD * _theta2 + RayMath.DEG2RAD * _theta3, RayMath.DEG2RAD * _theta1, 0f));

        Matrix4x4 p5previousTransform = RayMath.MatrixRotateXYZ(new Vector3(-RayMath.DEG2RAD * _theta2 - RayMath.DEG2RAD * _theta3, RayMath.DEG2RAD * _theta1, 0f));
        Matrix4x4 p5localTransform = RayMath.MatrixRotateY(RayMath.DEG2RAD * _theta4);
        p5.transform = RayMath.MatrixMultiply(p5previousTransform, p5localTransform);

        Matrix4x4 p6previousTransform = RayMath.MatrixRotateXYZ(new Vector3(RayMath.DEG2RAD * _theta2 + RayMath.DEG2RAD * _theta3, RayMath.DEG2RAD * _theta1, 0f));
        Matrix4x4 p6additionalTransform = RayMath.MatrixRotateX(RayMath.DEG2RAD * _theta5);
        Matrix4x4 p6LocalTransform = RayMath.MatrixMultiply(p5localTransform, p6additionalTransform);
        p6.transform = RayMath.MatrixMultiply(p6previousTransform, p6LocalTransform);

        if (_cubeGrabbed)
        {
            Matrix4x4 p7additionalTransform = RayMath.MatrixRotateY(RayMath.DEG2RAD * _theta6);
            Matrix4x4 p7LocalTransform = RayMath.MatrixMultiply(p6LocalTransform, p7additionalTransform);
            cubeModel.transform = RayMath.MatrixMultiply(p6previousTransform, p7LocalTransform);
        }
    }
    
    private void ManualControl()
    {
        if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
            _theta1 -= _timeScale;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
            _theta1 += _timeScale;

        if (Raylib.IsKeyDown(KeyboardKey.KEY_Z))
            _theta2 -= _timeScale;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_X))
            _theta2 += _timeScale;

        if (Raylib.IsKeyDown(KeyboardKey.KEY_C))
            _theta3 -= _timeScale;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_V))
            _theta3 += _timeScale;

        if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
            _theta4 -= _timeScale;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_F))
            _theta4 += _timeScale;

        if (Raylib.IsKeyDown(KeyboardKey.KEY_B))
            _theta5 -= _timeScale;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_N))
            _theta5 += _timeScale;

        if (Raylib.IsKeyDown(KeyboardKey.KEY_G))
            _theta6 -= _timeScale;
        if (Raylib.IsKeyDown(KeyboardKey.KEY_H))
            _theta6 += _timeScale;
        
        _theta1 = Math.Clamp(_theta1, -90, 150);
        _theta2 = Math.Clamp(_theta2, -85, 120);
        _theta3 = Math.Clamp(_theta3, -30, 120);
        _theta4 = Math.Clamp(_theta4, -45, 45);
        _theta5 = Math.Clamp(_theta5, -45, 45);
        _theta6 = Math.Clamp(_theta6, -150, 150);
    }
}
