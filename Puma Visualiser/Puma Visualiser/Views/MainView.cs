using System.Diagnostics;
using System.Globalization;
using System.Numerics;
using System.Runtime.InteropServices.JavaScript;
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
    private readonly Label _targetTitle, _targetXLabel, _targetYLabel, _targetZLabel, _targetTimeLabel;
    
    private readonly Button _startButton, _stopButton;
    private readonly Button _recordButton, _playButton;
    private readonly Slider _timeSlider;

    private readonly Label _statusLabel;
    
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

    private Vector3 _cubePos = new(50, 50, 0);
    private Matrix4x4 cubeRot = new Matrix4x4();

    private enum RobotState
    {
        MANUAL,
        PLAYING,
        RECORDING,
        IK
    }

    private RobotState _robotState;
    
    private record Thetas
    {
        public readonly float Time;
        public readonly bool CubeGrabbed;
        public readonly float Theta1, Theta2, Theta3, Theta4, Theta5, Theta6;

        public Thetas(float theta1, float theta2, float theta3, float theta4, float theta5, float theta6, bool cubeGrabbed, float time)
        {
            Theta1 = theta1;
            Theta2 = theta2;
            Theta3 = theta3;
            Theta4 = theta4;
            Theta5 = theta5;
            Theta6 = theta6;
            CubeGrabbed = cubeGrabbed;
            Time = time;
        }
    }
    private float _recordingDelta;
    private float _playDelta = 0;
    private int _playRecordingIdx = 0;
    private List<Thetas> _recording = new List<Thetas>();

    private Vector3 _cubeStartRecording;

    public MainView()
    {
        _timeline = new Timeline(new Rectangle(0, -250, -250, 250), Raylib.GRAY, Raylib.LIGHTGRAY, Raylib.DARKGRAY,
            Raylib.RED, 10, 10);
        
        _targetTitle = new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY + 130 + 20 + 40, -RightPanelInputStartY, 40), "Timestamp", 40, Raylib.BLACK);
        _targetXLabel = new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY + 10 + 5 + 130 + 20 + 40 + 40, 0, 0),
            "X", 20,
            Raylib.BLACK);
        _targetYLabel = new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY + 50 + 5 + 130 + 20 + 40 + 40, 0, 0),
            "Y", 20,
            Raylib.BLACK);
        _targetZLabel = new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY + 90 + 5 + 130 + 20 + 40 + 40, 0, 0),
            "Z", 20,
            Raylib.BLACK);
        _targetTimeLabel = new Label(new Rectangle(RightPanelStart + GuiMargin + 10, RightPanelInputStartY + 130 + 5 + 130 + 20 + 40 + 40, 0, 0),
            "t", 20,
            Raylib.BLACK);

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
        
        _targetYTextBox = new TextBox(
            new Rectangle(RightPanelStart + GuiMargin + 40, RightPanelInputStartY + 50 + 130 + 20 + 40 + 40, 200 - 2 * GuiMargin, 30), 20);
        
        _targetZTextBox = new TextBox(
            new Rectangle(RightPanelStart + GuiMargin + 40, RightPanelInputStartY + 90 + 130 + 20 + 40 + 40, 200 - 2 * GuiMargin, 30), 20);
        
        _targetTimeTextBox = new  TextBox(
            new Rectangle(RightPanelStart + GuiMargin + 40, RightPanelInputStartY + 130 + 130 + 20 + 40 + 40, 200 - 2 * GuiMargin, 30), 20);

        _guiElements.Add(new Label(new Rectangle(RightPanelStart + GuiMargin + 10, -10 - GuiMargin - 40 - 10 - 30 - 40 - 10 - 40 - 10 - 30, -RightPanelInputStartY, 40), "Status", 40, Raylib.BLACK));
        _statusLabel = new Label(new Rectangle(RightPanelStart + GuiMargin + 10, -10 - GuiMargin - 40 - 10 - 30 - 40 - 10 - 40, RightPanelInputStartY, 40), "Manual", 30, Raylib.BLACK);
        _guiElements.Add(_statusLabel);

        _startButton = new Button(new Rectangle(RightPanelStart + GuiMargin + 10, -10 - GuiMargin - 40 - 10 - 40, -RightPanelStart - 3 * GuiMargin - 10, 40), "START");
        _stopButton = new Button(new Rectangle(RightPanelStart + GuiMargin + 10, -10 - GuiMargin - 40, -RightPanelStart - 3 * GuiMargin - 10, 40), "STOP");
        
        _recordButton = new Button(new Rectangle(RightPanelStart + GuiMargin + 10, -10 - GuiMargin - 40 - 10 - 40, -RightPanelStart - 3 * GuiMargin - 10, 40), "RECORD");
        _playButton = new Button(new Rectangle(RightPanelStart + GuiMargin + 10, -10 - GuiMargin - 40, -RightPanelStart - 3 * GuiMargin - 10, 40), "PLAY");
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

        // SolveIK(xTarget, yTarget, zTarget, out _theta1, out _theta2, out _theta3);
        
        // Raylib.DrawCube(new Vector3(xTarget, yTarget, zTarget), 10,10,10, Raylib.GOLD);
        
        DrawRobot();
        
        Raylib.EndMode3D();
        Raylib.EndTextureMode();

        // RECORDING
        
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_R) || _recordButton.IsClicked())
        {
            if (_robotState == RobotState.RECORDING)
            {
                _robotState = RobotState.MANUAL;
            }
            else
            {
                _robotState = RobotState.RECORDING;
                _recordingDelta = 0;
                _recording.Clear();
                _recording.Add(new Thetas(_theta1, _theta2, _theta3, _theta4, _theta5, _theta6, _cubeGrabbed, _recordingDelta));
                _cubeStartRecording = _cubePos;
            }
        }
        
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_P) || _playButton.IsClicked())
        {
            if (_robotState == RobotState.PLAYING)
            {
                _robotState = RobotState.MANUAL;
            }
            else
            {
                _robotState = RobotState.PLAYING;
                _playDelta = 0;
                _playRecordingIdx = 0;
                _cubePos = _cubeStartRecording;
            }
        }

        if (_robotState == RobotState.RECORDING)
        {
            _recordingDelta += Raylib.GetFrameTime() * _timeScale;
            _currentTimeTextBox.Text = _recordingDelta.ToString("0.00");
            int l = _recording.Count - 1;
            var th = _recording[l];
            if (th.Theta1 != _theta1 ||
                th.Theta2 != _theta2 ||
                th.Theta3 != _theta3 ||
                th.Theta4 != _theta4 ||
                th.Theta5 != _theta5 ||
                th.Theta6 != _theta6 ||
                th.CubeGrabbed != _cubeGrabbed)
            {
                // Console.WriteLine($"Currently Recording idx: {_recording.Count}");
                _recording.Add(new Thetas(_theta1, _theta2, _theta3, _theta4, _theta5, _theta6, _cubeGrabbed, _recordingDelta));
            }
            
        }

        if (_robotState == RobotState.PLAYING)
        {
            // Console.WriteLine($"Currently Playing {_playRecordingIdx}/{_recording.Count}, Play Delta: {_playDelta}");
            _playDelta += Raylib.GetFrameTime() * _timeScale;
            _currentTimeTextBox.Text = _playDelta.ToString("0.00");
            while (true)
            {
                if (_playRecordingIdx >= _recording.Count)
                {
                    // Console.WriteLine("Playing finished.");
                    _robotState = RobotState.MANUAL;
                    break;
                }
                
                var th = _recording[_playRecordingIdx];
                if (th.Time > _playDelta)
                    break;
                _theta1 = th.Theta1;
                _theta2 = th.Theta2;
                _theta3 = th.Theta3;
                _theta4 = th.Theta4;
                _theta5 = th.Theta5;
                _theta6 = th.Theta6;
                _cubeGrabbed = th.CubeGrabbed;
                _playRecordingIdx++;
                // Console.WriteLine("Changed idx");
            }
        }

        if (_robotState == RobotState.IK)
        {
            // TIME
            _currentTimeTextBox.Text = _time.ToString("0.00");
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
        }

        _timeScale = _timeSlider.GetValue();
        
        
        // GUI DRAWING

        Raylib.BeginDrawing();

        Raylib.ClearBackground(Raylib.GRAY);

        Raylib.DrawRectangle(Raylib.GetScreenWidth() + RightPanelStart, 0, 250, Raylib.GetScreenHeight(), Raylib.GRAY);
        Raylib.DrawRectangle(Raylib.GetScreenWidth() + RightPanelStart + 10, 10, 230, Raylib.GetScreenHeight() - 20,
            Raylib.LIGHTGRAY);

        _statusLabel.Text = _robotState switch
        {
            RobotState.MANUAL => "MANUAL",
            RobotState.PLAYING => "PLAYING",
            RobotState.RECORDING => "RECORDING",
            RobotState.IK => "IK",
            _ => throw new ArgumentOutOfRangeException()
        };

        foreach (var item in _guiElements)
            item.Draw();
        
        
        if (_robotState == RobotState.MANUAL ||
            _robotState == RobotState.PLAYING ||
            _robotState == RobotState.RECORDING)
        {
            _recordButton.Draw();
            _playButton.Draw();
        }

        if (_robotState == RobotState.IK)
        {
            _timeline.Draw();
            _targetXTextBox.Draw();
            _targetYTextBox.Draw();
            _targetZTextBox.Draw();
            _targetTimeTextBox.Draw();
            _targetXLabel.Draw();
            _targetYLabel.Draw();
            _targetZLabel.Draw();
            _targetTimeLabel.Draw();
            _targetTitle.Draw();
        }
        
        Raylib.DrawTexture(robotTexture.Value.texture, GuiMargin, GuiMargin, Raylib.WHITE);
        
        Raylib.EndDrawing();
    }

    private void InitializeRobot()
    {
        if (robotTexture.HasValue)
            Raylib.UnloadRenderTexture(robotTexture.Value);

        // Robot render 
        if (_robotState == RobotState.IK)
        {
            robotTexture = Raylib.LoadRenderTexture(Raylib.GetScreenWidth() + RightPanelStart - 2 * GuiMargin,
                Raylib.GetScreenHeight() - 250 - 2 * GuiMargin);
        }
        else
        {
            robotTexture = Raylib.LoadRenderTexture(Raylib.GetScreenWidth() + RightPanelStart - 2 * GuiMargin,
                Raylib.GetScreenHeight() - 2 * GuiMargin);
        }
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
        p2t = Raylib.LoadTexture("resources\\p2.png");
        p3t = Raylib.LoadTexture("resources\\p3.png");
        p4t = Raylib.LoadTexture("resources\\p4.png");
        p5t = Raylib.LoadTexture("resources\\p5.png");
        p6t = Raylib.LoadTexture("resources\\p6.png");
        Raylib.SetTextureWrap(p1t, TextureWrap.TEXTURE_WRAP_CLAMP);
        Raylib.SetTextureWrap(p2t, TextureWrap.TEXTURE_WRAP_CLAMP);
        Raylib.SetTextureWrap(p3t, TextureWrap.TEXTURE_WRAP_CLAMP);
        Raylib.SetTextureWrap(p4t, TextureWrap.TEXTURE_WRAP_CLAMP);
        Raylib.SetTextureWrap(p5t, TextureWrap.TEXTURE_WRAP_CLAMP);
        Raylib.SetTextureWrap(p6t, TextureWrap.TEXTURE_WRAP_CLAMP);
        
        unsafe
        {
            p1.materials[0].maps[0].texture = p1t;
            p2.materials[0].maps[0].texture = p2t;
            p3.materials[0].maps[0].texture = p3t;
            p4.materials[0].maps[0].texture = p4t;
            // p5.materials[0].maps[0].texture = p5t;
            // p6.materials[0].maps[0].texture = p6t;
        }
    }

    private float _theta1 = 0.0f, _theta2 = 0.0f, _theta3 = 0.0f, _theta4 = 0.0f, _theta5 = 0.0f, _theta6 = 0.0f;
    private bool _cubeGrabbed = true;
    


    public void SolveIK(float xTarget, float yTarget, float zTarget, out float theta1, out float theta2, out float theta3)
    {
        float d0 = 61.722f;
        float a1 = 41.917f;
        float a3 = 43.215f;
        float d2 = 12.92f;

        // Calculate theta1 using x and y coordinates
        theta1 = MathF.Atan2(yTarget, xTarget);

        // Calculate the horizontal distance from the base to the target point (in xy-plane)
        float r = MathF.Sqrt(xTarget * xTarget + yTarget * yTarget) - d2;
        float s = zTarget - d0;

        // Calculate theta3 using the cosine law
        float D = (r * r + s * s - a1 * a1 - a3 * a3) / (2 * a1 * a3);
        theta3 = MathF.Atan2(MathF.Sqrt(1 - D * D), D);

        // Calculate theta2
        float phi2 = MathF.Atan2(s, r);
        float phi1 = MathF.Atan2(a3 * MathF.Sin(theta3), a1 + a3 * MathF.Cos(theta3));
        theta2 = phi2 - phi1;
        
        // Convert angles to degrees
        theta1 = RayMath.RAD2DEG * theta1;
        theta2 = RayMath.RAD2DEG * theta2;
        theta3 = RayMath.RAD2DEG * theta3;
    }

    private void DrawRobot()
    {
        //translation of DH position
        float pos03x = 41.917f * MathF.Cos(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) -
                       12.92f * MathF.Sin(RayMath.DEG2RAD * _theta1);
        float pos03y = 41.917f * MathF.Sin(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) +
                       12.92f * MathF.Cos(RayMath.DEG2RAD * _theta1);
        float pos03z = 61.722f + 41.917f * MathF.Sin(RayMath.DEG2RAD * _theta2);

        float pos06x = pos03x + 43.215f *
            (MathF.Cos(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) *
                MathF.Sin(RayMath.DEG2RAD * _theta3) + MathF.Cos(RayMath.DEG2RAD * _theta1) *
                MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3));
        float pos06y = pos03y + 43.215f *
            (MathF.Sin(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) *
                MathF.Sin(RayMath.DEG2RAD * _theta3) + MathF.Sin(RayMath.DEG2RAD * _theta1) *
                MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3));
        float pos06z = pos03z + 43.215f * (MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Sin(RayMath.DEG2RAD * _theta3) -
                                           MathF.Cos(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3));

        float pos7x = pos06x + MathF.Cos(RayMath.DEG2RAD * _theta5) * 5.334f *
            (MathF.Cos(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) *
                MathF.Sin(RayMath.DEG2RAD * _theta3) + MathF.Cos(RayMath.DEG2RAD * _theta1) *
                MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3));
        float pos7y = pos06y + MathF.Cos(RayMath.DEG2RAD * _theta5) * 5.334f *
            (MathF.Sin(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) *
                MathF.Sin(RayMath.DEG2RAD * _theta3) + MathF.Sin(RayMath.DEG2RAD * _theta1) *
                MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3));
        float pos7z = pos06z + MathF.Cos(RayMath.DEG2RAD * _theta5) * 5.334f *
            (MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Sin(RayMath.DEG2RAD * _theta3) -
             MathF.Cos(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3));

        float pos07x = pos7x +
            5.334f * MathF.Cos(RayMath.DEG2RAD * _theta4) * MathF.Sin(RayMath.DEG2RAD * _theta5) *
            (MathF.Cos(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) *
             MathF.Cos(RayMath.DEG2RAD * _theta3) +
             MathF.Cos(RayMath.DEG2RAD * _theta1) * MathF.Sin(RayMath.DEG2RAD * _theta2) *
             MathF.Sin(RayMath.DEG2RAD * _theta3)) - 5.334f * MathF.Sin(RayMath.DEG2RAD * _theta4) *
            MathF.Sin(RayMath.DEG2RAD * _theta5) * MathF.Sin(RayMath.DEG2RAD * _theta1);
        float pos07y = pos7y +
                       5.334f * MathF.Cos(RayMath.DEG2RAD * _theta4) * MathF.Sin(RayMath.DEG2RAD * _theta5) *
                       (MathF.Sin(RayMath.DEG2RAD * _theta1) * MathF.Cos(RayMath.DEG2RAD * _theta2) *
                        MathF.Cos(RayMath.DEG2RAD * _theta3) -
                        MathF.Sin(RayMath.DEG2RAD * _theta1) * MathF.Sin(RayMath.DEG2RAD * _theta2) *
                        MathF.Sin(RayMath.DEG2RAD * _theta3)) + 5.334f * MathF.Sin(RayMath.DEG2RAD * _theta4) *
                       MathF.Sin(RayMath.DEG2RAD * _theta5) * MathF.Cos(RayMath.DEG2RAD * _theta1);
        float pos07z = pos7z + 5.334f * MathF.Cos(RayMath.DEG2RAD * _theta4) * MathF.Sin(RayMath.DEG2RAD * _theta5) *
            (MathF.Sin(RayMath.DEG2RAD * _theta2) * MathF.Cos(RayMath.DEG2RAD * _theta3) +
             MathF.Cos(RayMath.DEG2RAD * _theta2) * MathF.Sin(RayMath.DEG2RAD * _theta3));

        Vector3 pos03 = new Vector3(pos03x, pos03z, pos03y);
        Vector3 pos06 = new Vector3(pos06x, pos06z, pos06y);
        Vector3 pos07 = new Vector3(pos07x, pos07z, pos07y); // wspolrzedne koncowki do obrotu bryla przenoszona 

        _currentXTextBox.Text = "" + pos07x;
        _currentYTextBox.Text = "" + pos07y;
        _currentZTextBox.Text = "" + pos07z;

        //rotating
        RotateRobot();

        Console.WriteLine($"{_cubePos} {pos07}");

    if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
        {
            if (_cubeGrabbed)
            {
                _cubeGrabbed = false;
            }
            else if (_cubePos.X > _cubePos.X - 5 && pos07.X < _cubePos.X + 5 &&
                pos07.Y > _cubePos.Y - 5 && pos07.Y < _cubePos.Y + 5 &&
                pos07.Z > _cubePos.Z - 5 && pos07.Z < _cubePos.Z + 5)
            {
                _cubeGrabbed = true;
            }
        }
        
        Console.WriteLine(_cubeGrabbed);

        var pos = _cubeGrabbed ? pos07 : _cubePos;

        if (_cubeGrabbed)
            _cubePos = pos07;
        
        Raylib.DrawModelEx(p1, new Vector3(0, 0, 0), new Vector3(0, 0, 0), 0f, modelScale, Raylib.WHITE);
        Raylib.DrawModelEx(p2, new Vector3(0, 61.722f, 0), new Vector3(0, 1, 0), 90f, modelScale, Raylib.WHITE);
        Raylib.DrawModelEx(p3, new Vector3(0, 61.722f, 0), new Vector3(0, 1, 0), 90f, modelScale, Raylib.WHITE);
        Raylib.DrawModelEx(p4, pos03, new Vector3(0, 1, 0), 90f, modelScale, Raylib.WHITE);
        Raylib.DrawModelEx(p5, pos06, new Vector3(0, 1, 0), -90f, modelScale, Raylib.DARKGRAY);
        Raylib.DrawModelEx(p6, pos06, new Vector3(0, 1, 0), 90f, modelScale, Raylib.GREEN);
        
        Raylib.DrawModelEx(cubeModel, pos, new Vector3(0, 1, 0), 90f, new Vector3(5, 5, 5), Raylib.DARKBLUE);
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
