using System;
using System.Drawing;
using Dataedo.App.Tools.ERD.Canvas;

namespace Dataedo.App.Tools.ERD.Diagram;

public class Box
{
	public Point StartPoint = new Point(int.MaxValue, int.MaxValue);

	public Point EndPoint = new Point(int.MinValue, int.MinValue);

	public Box()
	{
		StartPoint = new Point(int.MaxValue, int.MaxValue);
		EndPoint = new Point(int.MinValue, int.MinValue);
	}

	public void UpdateMinMax(RectangularObject element)
	{
		if (element != null)
		{
			StartPoint.X = Math.Min(StartPoint.X, element.Left);
			StartPoint.Y = Math.Min(StartPoint.Y, element.Top);
			EndPoint.X = Math.Max(EndPoint.X, element.Right);
			EndPoint.Y = Math.Max(EndPoint.Y, element.Bottom);
		}
	}

	public void UpdateMinMax(Box box)
	{
		if (box != null)
		{
			StartPoint.X = Math.Min(StartPoint.X, box.StartPoint.X);
			StartPoint.Y = Math.Min(StartPoint.Y, box.StartPoint.Y);
			EndPoint.X = Math.Max(EndPoint.X, box.EndPoint.X);
			EndPoint.Y = Math.Max(EndPoint.Y, box.EndPoint.Y);
		}
	}
}
