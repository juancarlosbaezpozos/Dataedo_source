using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Dataedo.App.Tools.ERD.Canvas;
using Dataedo.App.Tools.UI;

namespace Dataedo.App.Tools.ERD.Diagram;

public class PostIt : RectangularObject
{
	public const int MAX_DEFAULT_WIDTH = 500;

	public const int MAX_DEFAULT_HEIGHT = 500;

	private Point _Position = Point.Empty;

	private int width = 220;

	private int height = 52;

	private PostItLayerEnum.PostItLayer layer;

	private bool _Selected;

	private Color _Color = SkinsManager.DefaultSkin.ErdNodeDefault;

	public override IEnumerable<EdgeToResize> AvailableResizeEdges => Enum.GetValues(typeof(EdgeToResize)).Cast<EdgeToResize>();

	public override Point Position
	{
		get
		{
			return _Position;
		}
		set
		{
			if (value == _Position)
			{
				RecalcBox();
				return;
			}
			_Position = value;
			RecalcBox();
			NotifyChange();
			NotifyImportantChange();
		}
	}

	public override Size Size => new Size(Width, Height);

	public override int Width
	{
		get
		{
			return width;
		}
		set
		{
			if (value != Width)
			{
				width = value;
				RecalcBox();
			}
		}
	}

	public override int Height
	{
		get
		{
			return height;
		}
		set
		{
			if (value != Height)
			{
				height = value;
				RecalcBox();
			}
		}
	}

	public PostItLayerEnum.PostItLayer Layer
	{
		get
		{
			return layer;
		}
		set
		{
			if (value != layer)
			{
				layer = value;
				if (value != 0 && value == PostItLayerEnum.PostItLayer.Back)
				{
					TextPosition = PostItTextPositionEnum.PostItTextPosition.TopLeft;
				}
				else
				{
					TextPosition = PostItTextPositionEnum.PostItTextPosition.CenterCenter;
				}
				NotifyChange();
				NotifyImportantChange();
			}
		}
	}

	public override bool Selected
	{
		get
		{
			return _Selected;
		}
		internal set
		{
			_Selected = value;
			NotifyChange();
		}
	}

	public override Color Color
	{
		get
		{
			return _Color;
		}
		set
		{
			if (!(value == _Color))
			{
				_Color = value;
				NotifyChange();
				NotifyImportantChange();
			}
		}
	}

	public Font Font { get; set; } = new Font("Arial", Styles.NodeTextSize);


	public string Text { get; set; }

	public PostItTextPositionEnum.PostItTextPosition TextPosition { get; private set; } = PostItTextPositionEnum.PostItTextPosition.CenterCenter;


	public override Rectangle BoxToIntersect => base.Box;

	public override RectangularObjectType RectangularObjectType => RectangularObjectType.PostIt;

	public Size EditorSize => new Size(Math.Max(Width, 220) + 1, Math.Max(height, 52) + 2);

	public bool IsInAddingMode { get; set; }

	public Point EditorPosition(Point startPoint)
	{
		return new Point(startPoint.X + base.Box.X, startPoint.Y + base.Box.Y - 1);
	}

	public PostIt(int key, Point position, PostItLayerEnum.PostItLayer layer, string text)
	{
		base.MouseEnter += base.OnMouseEnter;
		base.MouseLeave += base.OnMouseLeave;
		Position = position;
		BringToFront();
		Id = (base.Key = key);
		Position = position;
		Layer = layer;
		Text = text;
		RecalcBox();
	}

	protected void NotifyImportantChange()
	{
		ImportantChange?.Invoke(this, EventArgs.Empty);
	}

	public override bool Contains(Point p)
	{
		return base.Box.Contains(p);
	}

	protected override Pen SetBrush(Output dest)
	{
		Color color = Styles.NodeBorderColor(this, dest);
		if (!Selected && !base.MouseOver)
		{
			return new Pen(color);
		}
		return new Pen(color, 2f);
	}

	private void RecalcBox()
	{
		base.Box = new Rectangle(Position.X - Size.Width / 2, Position.Y - Size.Height / 2, Size.Width, Size.Height);
	}

	public void FitSizeToText()
	{
		Size size = TextRenderer.MeasureText(Text, Font, new Size(Math.Max(Width, 500), Math.Max(Height, 500)), GetTextFormatFlags());
		size.Width += 2 * Styles.PostItHorizontalPadding;
		size.Height += 2 * Styles.PostItVerticalPadding;
		int val = ((size.Width % 2 == 0) ? size.Width : (size.Width + 1));
		int val2 = ((size.Height % 2 == 0) ? size.Height : (size.Height + 1));
		int num = 0;
		int num2 = 0;
		val = Math.Min(val, 500);
		val2 = Math.Min(val2, 500);
		val = Math.Max(val, 220);
		val2 = Math.Max(val2, 24);
		if (IsInAddingMode || val > Width)
		{
			num = (val - Width) / 2;
			Width = val;
		}
		if (IsInAddingMode || val2 > Height)
		{
			num2 = (val2 - Height) / 2;
			Height = val2;
		}
		Position = new Point(Position.X + num, Position.Y + num2);
		if (IsInAddingMode)
		{
			IsInAddingMode = false;
		}
	}

	public override void Render(Graphics g, Point startPoint, Output dest = Output.Control)
	{
		g.SmoothingMode = SmoothingMode.HighSpeed;
		Pen pen = SetBrush(dest);
		SolidBrush solidBrush = new SolidBrush(Styles.NodeBackgroundColor(this, dest));
		SolidBrush solidBrush2 = new SolidBrush((dest == Output.Control) ? SkinsManager.CurrentSkin.ErdNodeForeColor : SkinsManager.DefaultSkin.ErdNodeForeColor);
		Color foreColor = ((dest == Output.Control) ? SkinsManager.CurrentSkin.ErdNodeForeColor : SkinsManager.DefaultSkin.ErdNodeForeColor);
		DrawBackgroundBox(startPoint, pen, solidBrush, g);
		TextFormatFlags textFormatFlags = GetTextFormatFlags();
		Size textRectSize = GetTextRectSize();
		Rectangle bounds = new Rectangle(startPoint.X + base.Box.X + Styles.PostItHorizontalPadding, startPoint.Y + base.Box.Y + Styles.PostItVerticalPadding, textRectSize.Width - 2 * Styles.PostItHorizontalPadding, textRectSize.Height - 2 * Styles.PostItVerticalPadding);
		TextRenderer.DrawText(g, Text, Font, bounds, foreColor, textFormatFlags);
		solidBrush.Dispose();
		pen.Dispose();
		solidBrush2.Dispose();
	}

	private Size GetTextRectSize()
	{
		return new Size(base.Box.Width, base.Box.Height);
	}

	private TextFormatFlags GetTextFormatFlags()
	{
		TextFormatFlags postItFormatFlags = Styles.PostItFormatFlags;
		return TextPosition switch
		{
			PostItTextPositionEnum.PostItTextPosition.TopLeft => postItFormatFlags | TextFormatFlags.Default | TextFormatFlags.Default, 
			_ => postItFormatFlags | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter, 
		};
	}

	protected override void DrawBackgroundBox(Point startPoint, Pen borderPen, SolidBrush backgroundBrush, Graphics g)
	{
		if (Selected || base.MouseOver)
		{
			g.FillRectangle(backgroundBrush, startPoint.X + base.Box.X + 1, startPoint.Y + base.Box.Y, base.Box.Width - 1, base.Box.Height);
			g.DrawRectangle(borderPen, startPoint.X + base.Box.X + 1, startPoint.Y + base.Box.Y, base.Box.Width - 1, base.Box.Height);
			if (!SkinsManager.CurrentSkin.ErdUseNodeColorForHeaderBorder)
			{
				g.DrawLine(borderPen, startPoint.X + base.Box.X, startPoint.Y + base.Box.Y + base.Box.Height, startPoint.X + base.Box.X + base.Box.Width - 1, startPoint.Y + base.Box.Y + base.Box.Height);
			}
		}
		else
		{
			g.FillRectangle(backgroundBrush, startPoint.X + base.Box.X + 1, startPoint.Y + base.Box.Y + 1, base.Box.Width - 1, base.Box.Height - 1);
			g.DrawRectangle(borderPen, startPoint.X + base.Box.X, startPoint.Y + base.Box.Y, base.Box.Width, base.Box.Height);
		}
	}

	public override Rectangle GetBoxForNewPosition(int x, int y)
	{
		return new Rectangle(x - Size.Width / 2, y - Size.Height / 2, base.Box.Width, base.Box.Height);
	}

	public override Point GetNodePositionFromEndPoint(int x, int y)
	{
		return new Point(x - base.Box.Width / 2, y - base.Box.Height / 2);
	}

	public override Point GetNodePositionFromStartPoint(int x, int y)
	{
		return new Point(x + base.Box.Width / 2, y + base.Box.Height / 2);
	}

	public void BringToFront()
	{
		Layer = PostItLayerEnum.PostItLayer.Front;
	}

	public void SendToBack()
	{
		Layer = PostItLayerEnum.PostItLayer.Back;
	}

	protected override bool IsSizeValid(int width, int height)
	{
		if (width > 0)
		{
			return height >= 24;
		}
		return false;
	}

	public override bool IsOnEdge(Point cursorPos)
	{
		if (!IsOnVerticalEdge(cursorPos))
		{
			return IsOnHorizontalEdge(cursorPos);
		}
		return true;
	}

	public override XmlElement ToSvg(XmlDocument xml, Output destination)
	{
		XmlElement xmlElement = xml.CreateElement("g");
		xmlElement.SetAttribute("class", "post-it");
		xmlElement.SetAttribute("transform", $"translate({base.Box.X},{base.Box.Y})");
		XmlElement xmlElement2 = xml.CreateElement("rect");
		xmlElement2.SetAttribute("width", base.Box.Width.ToString());
		xmlElement2.SetAttribute("height", base.Box.Height.ToString());
		xmlElement2.SetAttribute("stroke-width", "1");
		xmlElement2.SetAttribute("stroke-dasharray", "1,0");
		xmlElement2.SetAttribute("fill", ColorTranslator.ToHtml(Styles.NodeBackgroundColor(this, destination)));
		xmlElement2.SetAttribute("stroke", ColorTranslator.ToHtml(Styles.NodeBorderColor(this, destination)));
		string text = $"post-it-text-mask-{base.Key}";
		CreateMask(xml, text, Styles.NodeHorizontalPadding, Styles.NodeVerticalPadding, Size.Width - 2 * Styles.PostItHorizontalPadding, Size.Height - 2 * Styles.PostItVerticalPadding);
		int num = Size.Width - 2 * Styles.PostItHorizontalPadding;
		int num2 = Size.Height - 2 * Styles.PostItVerticalPadding;
		int postItHorizontalPadding = Styles.PostItHorizontalPadding;
		int postItVerticalPadding = Styles.PostItVerticalPadding;
		string horzAlign;
		string vertAlign;
		switch (TextPosition)
		{
		case PostItTextPositionEnum.PostItTextPosition.TopLeft:
			horzAlign = "left";
			vertAlign = "text-top";
			break;
		default:
			horzAlign = "center";
			vertAlign = "middle";
			break;
		}
		XmlElement newChild = CanvasObject.CreateTextElement(xml, postItHorizontalPadding, postItVerticalPadding, num, num2, Text, horzAlign, text, setXml: false, vertAlign);
		xmlElement.AppendChild(xmlElement2);
		xmlElement.AppendChild(newChild);
		return xmlElement;
	}
}
