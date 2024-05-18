using Raylib_CsLo;

namespace Puma_Visualiser.GuiElements
{
	public abstract class GuiElement
	{
		protected Rectangle _bounds;
		private Rectangle _relativeBounds;

		public GuiElement(Rectangle _bounds)
		{
			_relativeBounds = _bounds;
			CalculateBounds();
		}

		public virtual void Draw()
		{
			if (Raylib.IsWindowResized())
			{
				CalculateBounds();
			}
		}

		private void CalculateBounds()
		{
			_bounds = _relativeBounds;


			if (_relativeBounds.x < 0)
			{
				_bounds.x = Raylib.GetScreenWidth() + _relativeBounds.x;
			}

			if (_relativeBounds.y < 0)
			{
				_bounds.y = Raylib.GetScreenHeight() + _relativeBounds.y;
			}
		}
	}
}