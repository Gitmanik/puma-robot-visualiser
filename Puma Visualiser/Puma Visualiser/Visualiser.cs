﻿using Puma_Visualiser.Views;
using Raylib_CsLo;

namespace Puma_Visualiser
{
	public static class Visualiser
	{
		private static IView? CurrentView;

		public static readonly System.Drawing.Size windowSize = new(1280, 720);

		public static async Task Main(string[] args)
		{
			Raylib.SetConfigFlags(ConfigFlags.FLAG_WINDOW_RESIZABLE);
			Raylib.InitWindow(windowSize.Width, windowSize.Height, "PUMA Robot Visualiser");
			Raylib.SetTargetFPS(60);

			CurrentView = new IntroView();

			while (!Raylib.WindowShouldClose())
			{
				Raylib.BeginDrawing();

				CurrentView.Draw();

				Raylib.EndDrawing();
			}
			Raylib.CloseWindow();
		}

		public static void ChangeView(IView newView)
		{
			Console.WriteLine($"View changed to {newView}");
			CurrentView = newView;
		}
	}
}