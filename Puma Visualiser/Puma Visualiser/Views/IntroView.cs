using Raylib_CsLo;

namespace Puma_Visualiser.Views;

internal class IntroView : IView
{
    public void Draw()
    {
        Raylib.ClearBackground(Raylib.LIGHTGRAY);
        Raylib.DrawText("PUMA Robot Visualiser", 640 - Raylib.MeasureText("PUMA Robot Visualiser", 50) / 2, 360 - 25,
            50, Raylib.RED);

        if (Raylib.GetTime() > 1)
        {
            Visualiser.ChangeView(new MainView());
        }
    }
}