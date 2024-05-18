using Puma_Visualiser.GuiElements;
using Raylib_CsLo;

namespace Puma_Visualiser.Views
{
    internal class MainView : IView
	{
		private bool test = false;

		private int RightPanelStart = -250;
 
		private List<GuiElement> _textBoxes = new List<GuiElement>();

		public MainView()
		{
			_textBoxes.Add(new TextBox(new Rectangle(RightPanelStart + 40, 10, 200, 30), 20));
			_textBoxes.Add(new TextBox(new Rectangle(RightPanelStart + 40, 50, 200, 30), 20));
			_textBoxes.Add(new TextBox(new Rectangle(RightPanelStart + 40, 90, 200, 30), 20));

			_textBoxes.Add(new Label(new Rectangle(RightPanelStart + 10, 10 + 5, 0, 0), "X", 20, Raylib.BLACK));
			_textBoxes.Add(new Label(new Rectangle(RightPanelStart + 10, 50 + 5, 0, 0), "Y", 20, Raylib.BLACK));
			_textBoxes.Add(new Label(new Rectangle(RightPanelStart + 10, 90 + 5, 0, 0), "Z", 20, Raylib.BLACK));
		}

		public void Draw()
		{
			Raylib.ClearBackground(Raylib.LIGHTGRAY);
			test = RayGui.GuiCheckBox(new Rectangle(10, 10, 20, 20), "test ok?", test);

			Raylib.DrawRectangle(Raylib.GetScreenWidth() + RightPanelStart, 0, 250, Raylib.GetScreenHeight(), Raylib.WHITE);

			foreach (var item in _textBoxes)
				item.Draw();
		}
	}
}