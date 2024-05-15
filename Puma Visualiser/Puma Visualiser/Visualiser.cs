using Raylib_CsLo;

public static class Visualiser
{
	private enum AppState
	{
		INTRO,
		PROGRAM
	}

	private static AppState _state = AppState.INTRO;

	public static async Task Main(string[] args)
	{
		Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
		Raylib.InitWindow(1280, 720, "PUMA Robot Visualiser");
		Raylib.SetTargetFPS(60);

		while (!Raylib.WindowShouldClose())
		{
			Raylib.BeginDrawing();

			switch (_state)
			{
				case AppState.INTRO:
					DrawIntro();
					break;

				case AppState.PROGRAM:
					RunApp();
					break;

				default:
					throw new NotImplementedException("Unsupported AppState!");
			}
			Raylib.EndDrawing();
		}
		Raylib.CloseWindow();
	}

	private static void DrawIntro()
	{
		Raylib.ClearBackground(Raylib.LIGHTGRAY);
		Raylib.DrawText("PUMA Robot Visualiser", 640 - Raylib.MeasureText("PUMA Robot Visualiser", 50) / 2, 360 - 25, 50, Raylib.RED);

		if (Raylib.GetTime() > 1)
		{
			_state = AppState.PROGRAM;
		}
	}

	private static bool test = false;
	private static string x = "";

	private static void RunApp()
	{
		Raylib.ClearBackground(Raylib.LIGHTGRAY);
		test = RayGui.GuiCheckBox(new Rectangle(10, 10, 20, 20), "test ok?", test);

		int RightPanelStart = Raylib.GetScreenWidth() - 250;

		Raylib.DrawLine(RightPanelStart, 0, RightPanelStart, Raylib.GetScreenHeight(), Raylib.BLACK);

		char cha = (char)Raylib.GetCharPressed();
		int key = Raylib.GetKeyPressed();
		if (cha != 0)
		{
			Console.WriteLine($"Char Pressed : {cha}");
			x += cha;
		}
		else if (key != 0)
		{
			Console.WriteLine($"Pressed : {key}");
			if (key == 259)
			{
				x = x.Remove(x.Length - 1);
			}
		}

		RayGui.GuiTextBox(new Rectangle(RightPanelStart, 10, 230, 30), x, 20, true);
	}
}