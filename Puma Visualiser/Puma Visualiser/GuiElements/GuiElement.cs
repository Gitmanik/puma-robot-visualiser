﻿using Raylib_CsLo;

namespace Puma_Visualiser.GuiElements;

public abstract class GuiElement
{
    protected Rectangle Bounds;
    private Rectangle _relativeBounds;

    public GuiElement(Rectangle bounds)
    {
        _relativeBounds = bounds;
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
        Bounds = _relativeBounds;

        /* SCALING BASED ON BASE WIDTH AND HEIGHT */
        //_bounds.x = _bounds.x * ((float) Raylib.GetScreenWidth() / Visualiser.windowSize.Width);
        //_bounds.y = _bounds.y * ((float) Raylib.GetScreenHeight() / Visualiser.windowSize.Height);
        //_bounds.width = _bounds.width * ((float)Raylib.GetScreenWidth() / Visualiser.windowSize.Width);
        //_bounds.height = _bounds.height * ((float) Raylib.GetScreenHeight() / Visualiser.windowSize.Height);


        /* DPI SCALING */
        //_bounds.X = _bounds.X * Raylib.GetWindowScaleDPI().X;
        //_bounds.Y = _bounds.Y * Raylib.GetWindowScaleDPI().Y;
        //_bounds.width = _bounds.width * Raylib.GetWindowScaleDPI().X;
        //_bounds.height = _bounds.height * Raylib.GetWindowScaleDPI().Y;

        if (_relativeBounds.x < 0)
        {
            Bounds.x = Raylib.GetScreenWidth() + _relativeBounds.x;
        }

        if (_relativeBounds.y < 0)
        {
            Bounds.y = Raylib.GetScreenHeight() + _relativeBounds.y;
        }

        if (_relativeBounds.width < 0)
        {
            Bounds.width = Raylib.GetScreenWidth() + _relativeBounds.width;
        }

        if (_relativeBounds.height < 0)
        {
            Bounds.height = Raylib.GetScreenHeight() + _relativeBounds.height;
        }

        Console.WriteLine(
            $"Recalculated bounds for {this}: {_relativeBounds.x} {_relativeBounds.y} -> {Bounds.x} {Bounds.y}");
    }
}