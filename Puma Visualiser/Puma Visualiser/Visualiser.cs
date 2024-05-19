using Puma_Visualiser.Views;
using Raylib_CsLo;

namespace Puma_Visualiser;

public static class Visualiser
{
    public static readonly System.Drawing.Size WindowSize = new(1280, 720);
    private static IView? _currentView;

    public static async Task Main(string[] args)
    {
        Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
        Raylib.InitWindow(WindowSize.Width, WindowSize.Height, "PUMA Robot Visualiser");
        Raylib.SetTargetFPS(60);

        _currentView = new IntroView();

        while (!Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();

            _currentView.Draw();

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    public static void ChangeView(IView newView)
    {
        Console.WriteLine($"View changed to {newView}");
        _currentView = newView;
    }
}