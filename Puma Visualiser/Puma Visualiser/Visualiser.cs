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
		Raylib.InitWindow(1280, 720, "PUMA Robot Visualiser");
		Raylib.SetTargetFPS(60);

		while (!Raylib.WindowShouldClose()) // Detect window close button or ESC key
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

	private static void RunApp()
	{
		Raylib.ClearBackground(Raylib.LIGHTGRAY);
	}
}