using Godot;
using System;

public static partial class Trace2D
{
	public static void TraceLine(Vector2I start, Vector2I dir, int length, Action<Vector2I> action)
	{
		Vector2I curCoords = start;

		for (int i = 0; i < length; i++)
		{
			action.Invoke(curCoords);
			curCoords += dir;
		}
	}

	public static void TraceRect(Vector2I start, Vector2I stop, Action<Vector2I> action)
	{
		Vector2I dir = (stop - start).Clamp(-1, 1);

		for (int x = start.X; x != stop.X; x += dir.X)
		{
			for (int y = start.Y; y != stop.Y; y += dir.Y)
			{
				Vector2I coords = new(x, y);
				action.Invoke(coords);
			}
		}
	}

	public static void TraceRectCells(Vector2I start, Vector2I stop, Action<Vector2I> action)
	{
		Vector2I offset = (stop - start).Clamp(-1, 0);
		TraceRect(start + offset, stop + offset, action);
	}

	public static void TraceHollowRect(Vector2I start, Vector2I stop, Action<Vector2I> action)
	{
		int horzSteps = Mathf.Abs(stop.X - start.X);
		int vertSteps = Mathf.Abs(stop.Y - start.Y) - 2;
		Vector2I dirX = new(Mathf.Clamp(stop.X - start.X, -1, 1), 0);
		Vector2I dirY = new(0, Mathf.Clamp(stop.Y - start.Y, -1, 1));
		Vector2I botLeft = new(start.X, stop.Y);
		Vector2I topRight = new(stop.X, start.Y);

		TraceLine(start, dirX, horzSteps, action);
		TraceLine(start + dirY, dirY, vertSteps, action);
		TraceLine(topRight + dirY - dirX, dirY, vertSteps, action);
		TraceLine(botLeft - dirY, dirX, horzSteps, action);
	}

	public static void TraceHollowRectCells(Vector2I start, Vector2I stop, Action<Vector2I> action)
	{
		Vector2I offset = (stop - start).Clamp(-1, 0);
		TraceHollowRect(start + offset, stop + offset, action);
	}
}
