using Raylib_CsLo;

namespace Puma_Visualiser.Views
{
	internal class MainView : IView
	{
		private bool test = false;

		private int RightPanelStart => Raylib.GetScreenWidth() - 250;

		private List<TextBox> _textBoxes = new List<TextBox>();

		public MainView()
		{
			_textBoxes.Add(new TextBox(new Rectangle(RightPanelStart + 40, 10, 200, 30), 20));
			_textBoxes.Add(new TextBox(new Rectangle(RightPanelStart + 40, 50, 200, 30), 20));
			_textBoxes.Add(new TextBox(new Rectangle(RightPanelStart + 40, 90, 200, 30), 20));
		}

		public void Draw()
		{
			Raylib.ClearBackground(Raylib.LIGHTGRAY);
			test = RayGui.GuiCheckBox(new Rectangle(10, 10, 20, 20), "test ok?", test);

			Raylib.DrawText("X", RightPanelStart + 10, 10 + 5, 20, Raylib.BLACK);
			Raylib.DrawText("Y", RightPanelStart + 10, 50 + 5, 20, Raylib.BLACK);
			Raylib.DrawText("Z", RightPanelStart + 10, 90 + 5, 20, Raylib.BLACK);


			Raylib.DrawLine(RightPanelStart, 0, RightPanelStart, Raylib.GetScreenHeight(), Raylib.BLACK);

			foreach (var item in _textBoxes)
				item.Tick();
		}
	}
}