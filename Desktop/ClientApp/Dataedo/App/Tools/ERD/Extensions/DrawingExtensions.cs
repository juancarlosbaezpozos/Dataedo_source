using System.Drawing;

namespace Dataedo.App.Tools.ERD.Extensions;

public static class DrawingExtensions
{
	public static Point Add(this Point current, Point other)
	{
		return new Point(current.X + other.X, current.Y + other.Y);
	}

	public static Point AddX(this Point current, int value)
	{
		return new Point(current.X + value, current.Y);
	}

	public static Point AddY(this Point current, int value)
	{
		return new Point(current.X, current.Y + value);
	}

	public static Point Subtract(this Point current, Point other)
	{
		return new Point(current.X - other.X, current.Y - other.Y);
	}

	public static Point SubtractX(this Point current, int value)
	{
		return new Point(current.X - value, current.Y);
	}

	public static Point SubtractY(this Point current, int value)
	{
		return new Point(current.X, current.Y - value);
	}

	public static Point ZeroX(this Point current)
	{
		return new Point(0, current.Y);
	}

	public static Point ZeroY(this Point current)
	{
		return new Point(current.X, 0);
	}

	public static Point ChangeX(this Point current, int value)
	{
		return new Point(value, current.Y);
	}

	public static Point ChangeY(this Point current, int value)
	{
		return new Point(current.X, value);
	}

	public static Point GetReversed(this Point current)
	{
		return new Point(-current.X, -current.Y);
	}

	public static Point GetNotNegative(this Point current)
	{
		if (current.X >= 0 && current.Y >= 0)
		{
			return current;
		}
		return new Point((current.X >= 0) ? current.X : 0, (current.Y >= 0) ? current.Y : 0);
	}

	public static Point GetNotLessThan(this Point current, int value)
	{
		if (current.X >= value && current.Y >= value)
		{
			return current;
		}
		return new Point((current.X >= value) ? current.X : value, (current.Y >= value) ? current.Y : value);
	}

	public static Point GetNotLessThan(this Point current, int x, int y)
	{
		if (current.X >= x && current.Y >= y)
		{
			return current;
		}
		return new Point((current.X >= x) ? current.X : x, (current.Y >= y) ? current.Y : y);
	}

	public static Point GetNotPositive(this Point current)
	{
		if (current.X <= 0 && current.Y <= 0)
		{
			return current;
		}
		return new Point((current.X <= 0) ? current.X : 0, (current.Y <= 0) ? current.Y : 0);
	}

	public static int GetNotPositiveX(this Point current)
	{
		if (current.X > 0)
		{
			return 0;
		}
		return current.X;
	}

	public static int GetNotPositiveY(this Point current)
	{
		if (current.Y > 0)
		{
			return 0;
		}
		return current.Y;
	}
}
