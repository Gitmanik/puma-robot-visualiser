using Raylib_CsLo;

namespace Puma_Visualiser.Views;

internal class IntroView : IView
{
    public void Draw()
    {
        Raylib.BeginDrawing();
            
        Raylib.ClearBackground(Raylib.LIGHTGRAY);
        Raylib.DrawText("PUMA Robot Visualiser", 640 - Raylib.MeasureText("PUMA Robot Visualiser", 50) / 2, 360 - 25 - 10,
            50, Raylib.RED);
        
        Raylib.DrawText("Michal Sojka, Pawel Reich", 640 - Raylib.MeasureText("Michal Sojka, Pawel Reich", 30) / 2, 360 - 15 + 25 + 10,
            30, Raylib.RED);

        if (Raylib.GetTime() > 1)
        {
            Visualiser.ChangeView(new MainView());
        }
        
        Raylib.EndDrawing();
    }
}