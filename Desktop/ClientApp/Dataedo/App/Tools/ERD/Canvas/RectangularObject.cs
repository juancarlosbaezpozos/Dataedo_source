using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Tools.ERD.Diagram;

namespace Dataedo.App.Tools.ERD.Canvas;

public abstract class RectangularObject : CanvasObject
{
	public const int DEFAULT_ONE_LINE_HEIGHT = 24;

	public const int DEFAULT_TWO_LINE_HEIGHT = 38;

	public const int DEFAULT_THREE_LINE_HEIGHT = 52;

	public const int DEFAULT_WIDTH = 220;

	protected const int RESIZE_ARROW_SHIFT = 3;

	public int? Id;

	public EventHandler ImportantChange;

	public int SubjectAreaId { get; set; }

	public Rectangle Box { get; set; }

	public abstract Rectangle BoxToIntersect { get; }

	public Point Center => new Point(Box.X + Box.Width / 2, Box.Y + Box.Height / 2);

	public int Top => Box.Top;

	public int Bottom => Box.Bottom;

	public int Left => Box.Left;

	public int Right => Box.Right;

	public abstract Point Position { get; set; }

	public abstract Size Size { get; }

	public abstract int Width { get; set; }

	public abstract int Height { get; set; }

	public abstract bool Selected { get; internal set; }

	public abstract Color Color { get; set; }

	public abstract RectangularObjectType RectangularObjectType { get; }

	public abstract IEnumerable<EdgeToResize> AvailableResizeEdges { get; }

	protected abstract Pen SetBrush(Output dest);

	public abstract bool IsOnEdge(Point cursorPos);

	public bool IsOnVerticalEdge(Point p)
	{
		if (IsVerticallyOnNode(p.Y))
		{
			if (!IsOnLeftEdge(p.X))
			{
				return IsOnRightEdge(p.X);
			}
			return true;
		}
		return false;
	}

	public bool IsOnLeftEdge(Point p)
	{
		if (IsVerticallyOnNode(p.Y))
		{
			return IsOnLeftEdge(p.X);
		}
		return false;
	}

	public bool IsOnRightEdge(Point p)
	{
		if (IsVerticallyOnNode(p.Y))
		{
			return IsOnRightEdge(p.X);
		}
		return false;
	}

	private bool IsVerticallyOnNode(int y)
	{
		if (y > Box.Top)
		{
			return y < Box.Bottom;
		}
		return false;
	}

	private bool IsOnLeftEdge(int x)
	{
		if (x > Box.Left - 3)
		{
			return x < Box.Left + 3;
		}
		return false;
	}

	private bool IsOnRightEdge(int x)
	{
		if (x > Box.Right - 3)
		{
			return x < Box.Right + 3;
		}
		return false;
	}

	public bool IsOnHorizontalEdge(Point p)
	{
		if (IsHorizontallyOnNode(p.X))
		{
			if (!IsOnTopEdge(p.Y))
			{
				return IsOnBottomEdge(p.Y);
			}
			return true;
		}
		return false;
	}

	public bool IsOnTopEdge(Point p)
	{
		if (IsHorizontallyOnNode(p.X))
		{
			return IsOnTopEdge(p.Y);
		}
		return false;
	}

	public bool IsOnBottomEdge(Point p)
	{
		if (IsHorizontallyOnNode(p.X))
		{
			return IsOnBottomEdge(p.Y);
		}
		return false;
	}

	private bool IsHorizontallyOnNode(int x)
	{
		if (x > Box.Left)
		{
			return x < Box.Right;
		}
		return false;
	}

	private bool IsOnTopEdge(int y)
	{
		if (y > Box.Top - 3)
		{
			return y < Box.Top + 3;
		}
		return false;
	}

	private bool IsOnBottomEdge(int y)
	{
		if (y > Box.Bottom - 3)
		{
			return y < Box.Bottom + 3;
		}
		return false;
	}

	public abstract Rectangle GetBoxForNewPosition(int x, int y);

	public abstract Point GetNodePositionFromEndPoint(int x, int y);

	public abstract Point GetNodePositionFromStartPoint(int x, int y);

	protected abstract void DrawBackgroundBox(Point startPoint, Pen borderPen, SolidBrush backgroundBrush, Graphics g);

	public Point GetValidNodePosition(Point position, Rectangle lastDiagramBox)
	{
		Point point = new Point(position.X, position.Y);
		int nodesPositionRightMargin = DiagramControl.NodesPositionRightMargin;
		int nodesPositionBottomMargin = DiagramControl.NodesPositionBottomMargin;
		Rectangle boxForNewPosition = GetBoxForNewPosition(point.X, point.Y);
		point.X = ((boxForNewPosition.Right <= nodesPositionRightMargin) ? boxForNewPosition.Right : nodesPositionRightMargin);
		point.Y = ((boxForNewPosition.Bottom <= nodesPositionBottomMargin) ? boxForNewPosition.Bottom : nodesPositionBottomMargin);
		point = GetNodePositionFromEndPoint((boxForNewPosition.Right < nodesPositionRightMargin) ? boxForNewPosition.Right : nodesPositionRightMargin, (boxForNewPosition.Bottom <= nodesPositionBottomMargin) ? boxForNewPosition.Bottom : nodesPositionBottomMargin);
		nodesPositionRightMargin = DiagramControl.NodesPositionLeftMargin + lastDiagramBox.Width;
		nodesPositionBottomMargin = DiagramControl.NodesPositionTopMargin + lastDiagramBox.Height;
		boxForNewPosition = GetBoxForNewPosition(point.X, point.Y);
		point.X = ((boxForNewPosition.Left >= nodesPositionRightMargin) ? boxForNewPosition.Left : nodesPositionRightMargin);
		point.Y = ((boxForNewPosition.Top >= nodesPositionBottomMargin) ? boxForNewPosition.Top : nodesPositionBottomMargin);
		return GetNodePositionFromStartPoint((boxForNewPosition.Left >= nodesPositionRightMargin) ? boxForNewPosition.Left : nodesPositionRightMargin, (boxForNewPosition.Top >= nodesPositionBottomMargin) ? boxForNewPosition.Top : nodesPositionBottomMargin);
	}

	public void UpdatePosition(Point position, Rectangle lastDiagramBox)
	{
		Point validNodePosition = GetValidNodePosition(position, lastDiagramBox);
		if (!Position.Equals(validNodePosition))
		{
			Position = validNodePosition;
		}
	}

	public static Cursor GetCursor(EdgeToResize edge)
	{
		switch (edge)
		{
		case EdgeToResize.Left:
		case EdgeToResize.Right:
			return Cursors.SizeWE;
		case EdgeToResize.Top:
		case EdgeToResize.Bottom:
			return Cursors.SizeNS;
		case EdgeToResize.TopLeft:
		case EdgeToResize.BottomRight:
			return Cursors.SizeNWSE;
		case EdgeToResize.TopRight:
		case EdgeToResize.BottomLeft:
			return Cursors.SizeNESW;
		default:
			return Cursors.Default;
		}
	}

	public Cursor GetCursor(Point cursorPos)
	{
		return GetCursor(GetEdge(cursorPos));
	}

	public void Resize(Point cursorPos, Rectangle diagramBox, EdgeToResize edge)
	{
		if (!AvailableResizeEdges.Contains(edge))
		{
			return;
		}
		ResizeParameters resizeParameters = GetResizeParameters(cursorPos, edge);
		if (IsSizeValid(resizeParameters.NewWidth, resizeParameters.NewHeight))
		{
			Point validNodePosition = GetValidNodePosition(new Point(Position.X - resizeParameters.XPositionShift, Position.Y - resizeParameters.YPositionShift), diagramBox);
			if (validNodePosition.X + resizeParameters.XPositionShift == Position.X && validNodePosition.Y + resizeParameters.YPositionShift == Position.Y)
			{
				Width = resizeParameters.NewWidth;
				Height = resizeParameters.NewHeight;
				Position = new Point(Position.X - resizeParameters.XPositionShift, Position.Y - resizeParameters.YPositionShift);
			}
		}
	}

	protected ResizeParameters GetResizeParameters(Point newEdgePosition, EdgeToResize edge)
	{
		ResizeParameters resizeParameters = new ResizeParameters();
		resizeParameters.NewWidth = Width;
		resizeParameters.NewHeight = Height;
		switch (edge)
		{
		case EdgeToResize.Left:
		{
			int num2 = Left - newEdgePosition.X;
			resizeParameters.XPositionShift = num2 - num2 / 2;
			resizeParameters.NewWidth += resizeParameters.XPositionShift * 2;
			break;
		}
		case EdgeToResize.Right:
		{
			int num2 = Right - newEdgePosition.X;
			resizeParameters.XPositionShift = num2 - num2 / 2;
			resizeParameters.NewWidth -= resizeParameters.XPositionShift * 2;
			break;
		}
		case EdgeToResize.Top:
		{
			int num = Top - newEdgePosition.Y;
			resizeParameters.YPositionShift = num - num / 2;
			resizeParameters.NewHeight += resizeParameters.YPositionShift * 2;
			break;
		}
		case EdgeToResize.Bottom:
		{
			int num = Bottom - newEdgePosition.Y;
			resizeParameters.YPositionShift = num - num / 2;
			resizeParameters.NewHeight -= resizeParameters.YPositionShift * 2;
			break;
		}
		case EdgeToResize.TopLeft:
		{
			int num = Top - newEdgePosition.Y;
			resizeParameters.YPositionShift = num - num / 2;
			resizeParameters.NewHeight += resizeParameters.YPositionShift * 2;
			int num2 = Left - newEdgePosition.X;
			resizeParameters.XPositionShift = num2 - num2 / 2;
			resizeParameters.NewWidth += resizeParameters.XPositionShift * 2;
			break;
		}
		case EdgeToResize.TopRight:
		{
			int num = Top - newEdgePosition.Y;
			resizeParameters.YPositionShift = num - num / 2;
			resizeParameters.NewHeight += resizeParameters.YPositionShift * 2;
			int num2 = Right - newEdgePosition.X;
			resizeParameters.XPositionShift = num2 - num2 / 2;
			resizeParameters.NewWidth -= resizeParameters.XPositionShift * 2;
			break;
		}
		case EdgeToResize.BottomLeft:
		{
			int num = Bottom - newEdgePosition.Y;
			resizeParameters.YPositionShift = num - num / 2;
			resizeParameters.NewHeight -= resizeParameters.YPositionShift * 2;
			int num2 = Left - newEdgePosition.X;
			resizeParameters.XPositionShift = num2 - num2 / 2;
			resizeParameters.NewWidth += resizeParameters.XPositionShift * 2;
			break;
		}
		case EdgeToResize.BottomRight:
		{
			int num = Bottom - newEdgePosition.Y;
			resizeParameters.YPositionShift = num - num / 2;
			resizeParameters.NewHeight -= resizeParameters.YPositionShift * 2;
			int num2 = Right - newEdgePosition.X;
			resizeParameters.XPositionShift = num2 - num2 / 2;
			resizeParameters.NewWidth -= resizeParameters.XPositionShift * 2;
			break;
		}
		}
		return resizeParameters;
	}

	public EdgeToResize GetEdge(Point cursorPos)
	{
		EdgeToResize edgeToResize = ((IsOnTopEdge(cursorPos) && IsOnLeftEdge(cursorPos)) ? EdgeToResize.TopLeft : ((IsOnTopEdge(cursorPos) && IsOnRightEdge(cursorPos)) ? EdgeToResize.TopRight : ((IsOnBottomEdge(cursorPos) && IsOnLeftEdge(cursorPos)) ? EdgeToResize.BottomLeft : ((IsOnBottomEdge(cursorPos) && IsOnRightEdge(cursorPos)) ? EdgeToResize.BottomRight : (IsOnLeftEdge(cursorPos) ? EdgeToResize.Left : (IsOnRightEdge(cursorPos) ? EdgeToResize.Right : (IsOnTopEdge(cursorPos) ? EdgeToResize.Top : (IsOnBottomEdge(cursorPos) ? EdgeToResize.Bottom : EdgeToResize.None))))))));
		if (!AvailableResizeEdges.Contains(edgeToResize))
		{
			return EdgeToResize.None;
		}
		return edgeToResize;
	}

	protected abstract bool IsSizeValid(int width, int height);

	public bool Intersects(Rectangle r)
	{
		return Box.IntersectsWith(r);
	}
}
