using System.Drawing;
using Dataedo.App.Tools.ERD.Extensions;

namespace Dataedo.App.Tools.ERD.Diagram;

public class LinkArrow
{
	public const int ARROW_LINE_LENGTH = 10;

	public const int HALF_ARROW_LINE_LENGTH = 5;

	private int VerticalShift;

	private int HorizontalShift;

	public Point LowerLeftPoint { get; set; }

	public Point LowerCenterPoint { get; set; }

	public Point LowerRightPoint { get; set; }

	public Point MiddlePoint { get; set; }

	public Point UpperPoint { get; set; }

	public ArrowDirectionEnum ArrowDirection { get; set; }

	public LinkSideEnum LinkSide { get; set; }

	public bool IsFullArrow { get; set; }

	public void SetShift(int value, bool vertical)
	{
		if (vertical)
		{
			VerticalShift = value;
			HorizontalShift = 0;
		}
		else
		{
			HorizontalShift = value;
			VerticalShift = 0;
		}
	}

	public void ClearShift()
	{
		VerticalShift = (HorizontalShift = 0);
	}

	public LinkArrow(LinkSideEnum linkSide, bool isFullArrow)
	{
		LinkSide = linkSide;
		IsFullArrow = isFullArrow;
	}

	public void RedrawArrow(Graphics g, Point startPoint, Pen linePen, Node fromNode, Node toNode, bool isFullArrow)
	{
		RecalculatePosition(fromNode, toNode);
		g.DrawLine(linePen, LowerCenterPoint.Add(startPoint), UpperPoint.Add(startPoint));
		IsFullArrow = isFullArrow;
		if (IsFullArrow)
		{
			g.DrawLine(linePen, LowerLeftPoint.Add(startPoint), MiddlePoint.Add(startPoint));
			g.DrawLine(linePen, LowerRightPoint.Add(startPoint), MiddlePoint.Add(startPoint));
		}
	}

	public void RecalculatePosition(Node fromNode, Node toNode)
	{
		if (LinkSide == LinkSideEnum.To && toNode != null && fromNode != null)
		{
			ArrowDirection = GetNodeDirection(fromNode, toNode);
			RecalculatePoints(toNode, ArrowDirection);
		}
		else if (fromNode != null && toNode != null)
		{
			ArrowDirection = GetNodeDirection(toNode, fromNode);
			RecalculatePoints(fromNode, ArrowDirection);
		}
	}

	private ArrowDirectionEnum GetNodeDirection(Node farNode, Node nearNode)
	{
		int x = farNode.Center.X;
		int y = farNode.Center.Y;
		int x2 = nearNode.Center.X;
		int y2 = nearNode.Center.Y;
		Point point = new Point(x, farNode.Top - 20);
		Point point2 = new Point(x, farNode.Bottom + 20);
		Point point3 = new Point(farNode.Left - 20, y);
		Point point4 = new Point(farNode.Right + 20, y);
		Point point5 = new Point(x2, nearNode.Top - 20);
		Point point6 = new Point(x2, nearNode.Bottom + 20);
		Point point7 = new Point(nearNode.Left - 20, y2);
		Point point8 = new Point(nearNode.Right + 20, y2);
		if (point2.Y <= point5.Y)
		{
			return ArrowDirectionEnum.Up;
		}
		if (point.Y > point6.Y)
		{
			return ArrowDirectionEnum.Down;
		}
		if (point3.X > point8.X)
		{
			return ArrowDirectionEnum.Right;
		}
		if (point4.X <= point7.X)
		{
			return ArrowDirectionEnum.Left;
		}
		if (x < x2 && y <= y2)
		{
			if (point4.X > point7.X)
			{
				if (point2.Y > y2)
				{
					return ArrowDirectionEnum.Down;
				}
				if (point.Y > y2)
				{
					return ArrowDirectionEnum.Up;
				}
			}
			return ArrowDirectionEnum.Left;
		}
		if (x < x2 && y > y2)
		{
			if (point4.X > x2)
			{
				return ArrowDirectionEnum.Right;
			}
			return ArrowDirectionEnum.Down;
		}
		if (x > x2 && y <= y2)
		{
			if (point4.X > point7.X)
			{
				if (point2.Y > y2)
				{
					return ArrowDirectionEnum.Down;
				}
				if (point.Y > y2)
				{
					return ArrowDirectionEnum.Up;
				}
			}
			return ArrowDirectionEnum.Right;
		}
		if (point3.X <= x2)
		{
			return ArrowDirectionEnum.Left;
		}
		return ArrowDirectionEnum.Down;
	}

	private void RecalculatePoints(Node nearNode, ArrowDirectionEnum nearNodeDirection)
	{
		Point point = (LowerCenterPoint = GetCenterPoint(nearNode, nearNodeDirection));
		switch (nearNodeDirection)
		{
		case ArrowDirectionEnum.Up:
			UpperPoint = new Point(point.X, point.Y - 20);
			LowerLeftPoint = new Point(point.X - 5, point.Y);
			LowerRightPoint = new Point(point.X + 5, point.Y);
			MiddlePoint = new Point(point.X, point.Y - 10);
			break;
		case ArrowDirectionEnum.Right:
			UpperPoint = new Point(point.X + 20, point.Y);
			LowerLeftPoint = new Point(point.X, point.Y - 5);
			LowerRightPoint = new Point(point.X, point.Y + 5);
			MiddlePoint = new Point(point.X + 10, point.Y);
			break;
		case ArrowDirectionEnum.Down:
			UpperPoint = new Point(point.X, point.Y + 20);
			LowerLeftPoint = new Point(point.X - 5, point.Y);
			LowerRightPoint = new Point(point.X + 5, point.Y);
			MiddlePoint = new Point(point.X, point.Y + 10);
			break;
		case ArrowDirectionEnum.Left:
			UpperPoint = new Point(point.X - 20, point.Y);
			LowerLeftPoint = new Point(point.X, point.Y - 5);
			LowerRightPoint = new Point(point.X, point.Y + 5);
			MiddlePoint = new Point(point.X - 10, point.Y);
			break;
		}
	}

	private Point GetCenterPoint(Node nearNode, ArrowDirectionEnum nearNodeDirection)
	{
		int x = nearNode.Center.X;
		int y = nearNode.Center.Y;
		Point result = nearNodeDirection switch
		{
			ArrowDirectionEnum.Up => new Point(x, nearNode.Top), 
			ArrowDirectionEnum.Right => new Point(nearNode.Right, y), 
			ArrowDirectionEnum.Down => new Point(x, nearNode.Bottom), 
			ArrowDirectionEnum.Left => new Point(nearNode.Left, y), 
			_ => new Point(x, nearNode.Top), 
		};
		result.Offset(HorizontalShift, VerticalShift);
		return result;
	}
}
