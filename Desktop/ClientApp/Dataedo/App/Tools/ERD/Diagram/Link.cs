using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Enums;
using Dataedo.App.Tools.ERD.Canvas;
using Dataedo.App.Tools.ERD.Extensions;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.UI;
using Dataedo.Model.Data.Erd;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.Tools.ERD.Diagram;

public class Link : CanvasObject, IEqualityComparer<Link>
{
	private const int LABEL_WIDTH = 180;

	public EventHandler ImportantChange;

	public int MouseOverSqrBorder = 100;

	private string name = "";

	private string title = "";

	private string description = string.Empty;

	private bool selected;

	private bool? showTitle = false;

	private bool? showJoinCondition = false;

	private bool? hidden = false;

	private LinkStyleEnum.LinkStyle? linkStyle;

	private int joinConditionMaxLines
	{
		get
		{
			if (ShowTitle != true || string.IsNullOrWhiteSpace(Title))
			{
				return 3;
			}
			return 2;
		}
	}

	public bool IsModifiedInRelationForm { get; set; }

	public int? Id { get; set; }

	public Node FromNode { get; private set; }

	public Node ToNode { get; private set; }

	public CardinalityTypeEnum.CardinalityType? FromNodeCardinality { get; set; }

	public CardinalityTypeEnum.CardinalityType? ToNodeCardinality { get; set; }

	public string JoinCondition { get; set; }

	public string JoinConditionPlain => JoinCondition;

	public string Name
	{
		get
		{
			return name;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			if (value != name)
			{
				name = value;
				NotifyChange();
				NotifyImportantChange();
			}
		}
	}

	public string Title
	{
		get
		{
			return title;
		}
		set
		{
			if (value != title)
			{
				title = value;
				NotifyChange();
				NotifyImportantChange();
			}
		}
	}

	public string LabelToShow
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (isTitleRendered)
			{
				stringBuilder.AppendLine(Title);
			}
			if (ShowJoinCondition == true && !string.IsNullOrEmpty(JoinCondition))
			{
				stringBuilder.Append(JoinCondition);
			}
			string text = stringBuilder.ToString();
			if (!string.IsNullOrWhiteSpace(text))
			{
				return text;
			}
			return null;
		}
	}

	public string LabelToShowInTooltip
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(Title))
			{
				stringBuilder.AppendLine(Title);
			}
			if (!string.IsNullOrEmpty(JoinCondition))
			{
				stringBuilder.AppendLine(JoinCondition);
			}
			if (!string.IsNullOrEmpty(Description))
			{
				stringBuilder.AppendLine(Description);
			}
			string text = stringBuilder.ToString();
			if (!string.IsNullOrWhiteSpace(text))
			{
				return text;
			}
			return null;
		}
	}

	public string Description
	{
		get
		{
			return description;
		}
		set
		{
			if (value != description && value != null)
			{
				description = value;
				NotifyChange();
				NotifyImportantChange();
			}
		}
	}

	public bool UserRelation { get; set; }

	public bool Selected
	{
		get
		{
			return selected;
		}
		set
		{
			if (value != selected)
			{
				selected = value;
				IEnumerable<int> columnIds = Columns.Select((RelationColumnRow x) => x.ColumnPkId);
				IEnumerable<int> columnIds2 = Columns.Select((RelationColumnRow x) => x.ColumnFkId);
				FromNode?.SetActiveColumn(columnIds, Selected);
				ToNode?.SetActiveColumn(columnIds2, Selected);
				NotifyChange();
			}
		}
	}

	public bool? ShowTitle
	{
		get
		{
			return showTitle;
		}
		set
		{
			if (value != showTitle)
			{
				showTitle = value;
				NotifyChange();
				NotifyImportantChange();
			}
		}
	}

	private bool isTitleRendered
	{
		get
		{
			if (ShowTitle == true)
			{
				return !string.IsNullOrWhiteSpace(Title);
			}
			return false;
		}
	}

	public bool? ShowJoinCondition
	{
		get
		{
			return showJoinCondition;
		}
		set
		{
			if (value != showJoinCondition)
			{
				showJoinCondition = value;
				NotifyChange();
				NotifyImportantChange();
			}
		}
	}

	public bool? Hidden
	{
		get
		{
			return hidden;
		}
		set
		{
			if (value != hidden)
			{
				hidden = value;
				NotifyChange();
				NotifyImportantChange();
			}
		}
	}

	public LinkStyleEnum.LinkStyle? LinkStyle
	{
		get
		{
			return linkStyle;
		}
		set
		{
			if (value != linkStyle)
			{
				linkStyle = value;
				NotifyChange();
				NotifyImportantChange();
			}
		}
	}

	public LinkArrow FromNodeArrow { get; set; }

	public LinkArrow ToNodeArrow { get; set; }

	public int RelationId { get; set; }

	public int SubjecrAreaId { get; set; }

	public List<RelationColumnRow> Columns { get; set; }

	public void SetFromNode(Node node)
	{
		FromNode = node;
	}

	internal Link(Node fromNode, Node toNode, int relationId, bool isUserRelation = false, string pkType = "ONE", string fkType = "MANY")
	{
		base.MouseEnter += base.OnMouseEnter;
		base.MouseLeave += base.OnMouseLeave;
		ToNodeCardinality = CardinalityTypeEnum.StringToType(fkType);
		FromNodeCardinality = CardinalityTypeEnum.StringToType(pkType);
		FromNodeArrow = new LinkArrow(LinkSideEnum.From, FromNodeCardinality == CardinalityTypeEnum.CardinalityType.Many);
		ToNodeArrow = new LinkArrow(LinkSideEnum.To, ToNodeCardinality == CardinalityTypeEnum.CardinalityType.Many);
		FromNode = fromNode;
		ToNode = toNode;
		RelationId = relationId;
		base.Key = relationId;
		UserRelation = isUserRelation;
		Columns = new List<RelationColumnRow>();
	}

	public void SetKey(int value)
	{
		base.Key = value;
	}

	public void ToggleShowTitle()
	{
		ShowTitle = !ShowTitle.GetValueOrDefault();
	}

	public void ToggleShowJoinCondition()
	{
		ShowJoinCondition = !ShowJoinCondition.GetValueOrDefault();
	}

	public void ToggleShowRelation()
	{
		Hidden = !Hidden.GetValueOrDefault();
	}

	public void GetDataFromRelationRow(RelationLinkObject relation, int moduleId, bool formatted, bool forHtml, OtherFieldsSupport otherFieldsSupport)
	{
		SubjecrAreaId = moduleId;
		if (otherFieldsSupport == null || otherFieldsSupport.IsSelected(OtherFieldEnum.OtherField.Description))
		{
			Description = relation.Description;
		}
		Name = relation.Name;
		if (otherFieldsSupport == null || otherFieldsSupport.IsSelected(OtherFieldEnum.OtherField.Title))
		{
			Title = relation.Title;
		}
		ShowTitle = relation.ShowLabel ?? true;
		ShowJoinCondition = relation.ShowJoinCondition;
		Hidden = relation.Hidden;
		LinkStyle = LinkStyleEnum.ObjectToType(relation.LinkStyle);
		Id = relation.LinkId;
		FromNodeCardinality = CardinalityTypeEnum.StringToType(relation.PkType);
		ToNodeCardinality = CardinalityTypeEnum.StringToType(relation.FkType);
		JoinCondition = LinkJoins.GetJoin(formatted, forHtml, ToNode, FromNode, Columns);
	}

	public void RefreshUserRelationHint()
	{
		string join = LinkJoins.GetJoin(formatted: false, forHtml: true, ToNode, FromNode, Columns);
		if (!join.Equals(JoinCondition))
		{
			JoinCondition = join;
		}
	}

	protected void NotifyImportantChange()
	{
		if (!IsModifiedInRelationForm)
		{
			ImportantChange?.Invoke(this, EventArgs.Empty);
		}
	}

	public override void Render(Graphics g, Point startPoint, Output dest = Output.Control)
	{
		if (dest == Output.Image && (FromNode.Deleted || ToNode.Deleted || Hidden == true || FromNode == null))
		{
			return;
		}
		Pen linePen = Styles.LinkBorderPen(Selected, userRelation: false, Hidden, dest);
		g.SmoothingMode = SmoothingMode.AntiAlias;
		FromNodeArrow.RedrawArrow(g, startPoint, linePen, FromNode, ToNode, CardinalityTypeEnum.CardinalityType.Many == FromNodeCardinality);
		ToNodeArrow.RedrawArrow(g, startPoint, linePen, FromNode, ToNode, CardinalityTypeEnum.CardinalityType.Many == ToNodeCardinality);
		RenderLine(g, startPoint, dest);
		if (isTitleRendered || (ShowJoinCondition == true && !string.IsNullOrEmpty(JoinCondition)))
		{
			float x = (FromNodeArrow.UpperPoint.X + ToNodeArrow.UpperPoint.X) / 2;
			float y = (FromNodeArrow.UpperPoint.Y + ToNodeArrow.UpperPoint.Y) / 2;
			if (FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Down && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Down)
			{
				y = Math.Max(FromNodeArrow.UpperPoint.Y, ToNodeArrow.UpperPoint.Y);
			}
			PointF pointF = new PointF(x, y);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < joinConditionMaxLines; i++)
			{
				stringBuilder.AppendLine();
			}
			int num = TextRenderer.MeasureText(Environment.NewLine + Environment.NewLine, Styles.LinkFont, default(Size), Styles.LinkFormatFlags).Height / 2;
			int height = TextRenderer.MeasureText(stringBuilder.ToString(), Styles.LinkFont, default(Size), Styles.LinkFormatFlags).Height;
			int labelShiftForHorizontalLines = GetLabelShiftForHorizontalLines(g);
			float num2 = (float)startPoint.X + pointF.X - 90f;
			float num3 = (float)startPoint.Y + pointF.Y - (float)(height / 2) - (float)labelShiftForHorizontalLines;
			new SolidBrush((dest == Output.Control) ? SkinsManager.CurrentSkin.ErdLinkColor : SkinsManager.DefaultSkin.ErdLinkColor);
			Color foreColor = ((dest == Output.Control) ? SkinsManager.CurrentSkin.ErdLinkTextForeColor : SkinsManager.DefaultSkin.ErdLinkTextForeColor);
			if (isTitleRendered)
			{
				int num4 = ((ShowJoinCondition == true && !string.IsNullOrEmpty(JoinCondition)) ? num : 0);
				TextRenderer.DrawText(bounds: new Rectangle((int)num2, (int)(num3 - (float)num4), 180, num), dc: g, text: Title, font: Styles.LinkFont, foreColor: foreColor, flags: Styles.LinkFormatFlags);
			}
			if (ShowJoinCondition == true && !string.IsNullOrEmpty(JoinConditionPlain))
			{
				TextRenderer.DrawText(bounds: new Rectangle((int)num2, (int)num3, 180, height), dc: g, text: JoinConditionPlain, font: Styles.LinkFont, foreColor: foreColor, flags: Styles.LinkFormatFlags);
			}
		}
	}

	private void DrawText(Graphics g, Rectangle rectangle, Color linkTextColor)
	{
		string[] array = JoinConditionPlain.Split(new string[1] { " " }, StringSplitOptions.None);
		StringBuilder stringBuilder = new StringBuilder();
		List<string> list = new List<string>();
		for (int i = 0; i < array.Length; i++)
		{
			string text = ((i != 0) ? (" " + array[i]) : array[i]);
			if (TextRenderer.MeasureText(stringBuilder?.ToString() + text, Styles.LinkFont, default(Size), TextFormatFlags.Default).Width < rectangle.Width)
			{
				stringBuilder.Append(text);
			}
			else if (stringBuilder.Length > 0)
			{
				list.Add(stringBuilder.ToString());
				stringBuilder.Clear();
				stringBuilder.Append(text);
			}
		}
		if (stringBuilder.Length > 0)
		{
			list.Add(stringBuilder.ToString());
		}
		CharacterRange[] array2 = new CharacterRange[4]
		{
			new CharacterRange(0, 5),
			new CharacterRange(5, 5),
			new CharacterRange(10, 5),
			new CharacterRange(15, JoinConditionPlain.Length - 15)
		};
		StringFormat stringFormat = new StringFormat();
		stringFormat.Alignment = StringAlignment.Center;
		stringFormat.LineAlignment = StringAlignment.Center;
		stringFormat.SetMeasurableCharacterRanges(array2);
		Region[] array3 = g.MeasureCharacterRanges(JoinCondition, Styles.LinkFont, rectangle, stringFormat);
		array3.Select((Region x) => Rectangle.Round(x.GetBounds(g)));
		for (int j = 0; j < array3.Length; j++)
		{
			Rectangle bounds = Rectangle.Round(array3[j].GetBounds(g));
			string text2 = JoinCondition.Substring(array2[j].First, array2[j].Length);
			TextRenderer.DrawText(g, text2, Styles.LinkFont, bounds, (j % 2 == 0) ? Color.Red : linkTextColor, Styles.LinkFormatFlags);
		}
	}

	private void RenderLine(Graphics g, Point startPoint, Output dest)
	{
		Pen pen = Styles.LinkBorderPen(Selected, UserRelation, Hidden, dest);
		Point point = FromNodeArrow.UpperPoint.Add(startPoint);
		Point point2 = ToNodeArrow.UpperPoint.Add(startPoint);
		bool flag = point.X == point2.X || point.Y == point2.Y;
		if (LinkStyle == LinkStyleEnum.LinkStyle.Orthogonal && !flag)
		{
			DrawLineWithSubpoints(g, pen, point, point2, GetLinkSubpoints(point, point2));
		}
		else
		{
			g.DrawLine(pen, point, point2);
		}
		pen.Dispose();
	}

	private Point[] GetLinkSubpoints(Point fromPoint, Point toPoint)
	{
		Point[] result = new Point[0];
		Point point = new Point((fromPoint.X + toPoint.X) / 2, (fromPoint.Y + toPoint.Y) / 2);
		bool num = (FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Down && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Up) || (FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Up && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Down);
		bool flag = (FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Left && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Right) || (FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Right && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Left);
		bool flag2 = FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Down && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Down;
		bool flag3 = FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Up && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Up;
		bool flag4 = FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Left && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Left;
		bool flag5 = FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Right && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Right;
		bool flag6 = (FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Up && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Left) || (FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Left && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Up);
		bool flag7 = (FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Up && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Right) || (FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Right && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Up);
		bool flag8 = (FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Left && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Down) || (FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Down && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Left);
		bool flag9 = (FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Right && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Down) || (FromNodeArrow.ArrowDirection == ArrowDirectionEnum.Down && ToNodeArrow.ArrowDirection == ArrowDirectionEnum.Right);
		if (num)
		{
			result = new Point[2]
			{
				new Point(fromPoint.X, point.Y),
				new Point(toPoint.X, point.Y)
			};
		}
		else if (flag)
		{
			result = new Point[2]
			{
				new Point(point.X, fromPoint.Y),
				new Point(point.X, toPoint.Y)
			};
		}
		else if (flag2)
		{
			int y = ((fromPoint.Y > toPoint.Y) ? fromPoint.Y : toPoint.Y);
			result = new Point[2]
			{
				new Point(fromPoint.X, y),
				new Point(toPoint.X, y)
			};
		}
		else if (flag3)
		{
			int y2 = ((fromPoint.Y < toPoint.Y) ? fromPoint.Y : toPoint.Y);
			result = new Point[2]
			{
				new Point(fromPoint.X, y2),
				new Point(toPoint.X, y2)
			};
		}
		else if (flag4)
		{
			int x = ((fromPoint.X < toPoint.X) ? fromPoint.X : toPoint.X);
			result = new Point[2]
			{
				new Point(x, fromPoint.Y),
				new Point(x, toPoint.Y)
			};
		}
		else if (flag5)
		{
			int x2 = ((fromPoint.X > toPoint.X) ? fromPoint.X : toPoint.X);
			result = new Point[2]
			{
				new Point(x2, fromPoint.Y),
				new Point(x2, toPoint.Y)
			};
		}
		else if (flag6 || flag7)
		{
			result = new Point[1] { (fromPoint.Y < toPoint.Y) ? new Point(toPoint.X, fromPoint.Y) : new Point(fromPoint.X, toPoint.Y) };
		}
		else if (flag8 || flag9)
		{
			result = new Point[1] { (fromPoint.Y > toPoint.Y) ? new Point(toPoint.X, fromPoint.Y) : new Point(fromPoint.X, toPoint.Y) };
		}
		return result;
	}

	private void DrawLineWithSubpoints(Graphics g, Pen linePen, Point from, Point to, Point[] subpoints)
	{
		int num = subpoints.Length;
		if (num == 0)
		{
			g.DrawLine(linePen, from, to);
			return;
		}
		g.DrawLine(linePen, from, subpoints.First());
		g.DrawLine(linePen, subpoints.Last(), to);
		if (num > 1)
		{
			for (int i = 0; i < num - 1; i++)
			{
				g.DrawLine(linePen, subpoints[i], subpoints[i + 1]);
			}
		}
	}

	public static bool IsInPolygon(Point[] poly, Point point)
	{
		List<int> list = poly.Skip(1).Select((Point p, int i) => (point.Y - poly[i].Y) * (p.X - poly[i].X) - (point.X - poly[i].X) * (p.Y - poly[i].Y)).ToList();
		if (list.Any((int p) => p == 0))
		{
			return true;
		}
		for (int j = 1; j < list.Count(); j++)
		{
			if (list[j] * list[j - 1] < 0)
			{
				return false;
			}
		}
		return true;
	}

	private int GetLabelShiftForHorizontalLines(Graphics g)
	{
		if (string.IsNullOrWhiteSpace(LabelToShow))
		{
			return 0;
		}
		int height = TextRenderer.MeasureText(g, JoinCondition, Styles.LinkFont, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.SingleLine).Height;
		int height2 = TextRenderer.MeasureText(g, JoinCondition, Styles.LinkFont, new Size(180, int.MaxValue), Styles.LinkFormatFlags).Height;
		if (height == 0)
		{
			return 0;
		}
		int num = height2 / height;
		if (num > joinConditionMaxLines)
		{
			num = joinConditionMaxLines;
		}
		if (Math.Abs(FromNodeArrow.MiddlePoint.Y - ToNodeArrow.MiddlePoint.Y) <= height2 && num % 2 == 1)
		{
			return height / 2;
		}
		return 0;
	}

	public override bool Contains(Point p)
	{
		bool flag = ((!FromNodeArrow.IsFullArrow) ? (SqrDistance(FromNodeArrow.UpperPoint, FromNodeArrow.MiddlePoint, p) < (double)MouseOverSqrBorder) : (SqrDistance(FromNodeArrow.UpperPoint, FromNodeArrow.MiddlePoint, p) < (double)MouseOverSqrBorder || SqrDistance(FromNodeArrow.LowerLeftPoint, FromNodeArrow.LowerCenterPoint, p) < (double)MouseOverSqrBorder || SqrDistance(FromNodeArrow.LowerRightPoint, FromNodeArrow.LowerCenterPoint, p) < (double)MouseOverSqrBorder));
		bool flag2 = ((!ToNodeArrow.IsFullArrow) ? (SqrDistance(ToNodeArrow.UpperPoint, ToNodeArrow.MiddlePoint, p) < (double)MouseOverSqrBorder) : (SqrDistance(ToNodeArrow.UpperPoint, ToNodeArrow.MiddlePoint, p) < (double)MouseOverSqrBorder || SqrDistance(ToNodeArrow.LowerLeftPoint, ToNodeArrow.LowerCenterPoint, p) < (double)MouseOverSqrBorder || SqrDistance(ToNodeArrow.LowerRightPoint, ToNodeArrow.LowerCenterPoint, p) < (double)MouseOverSqrBorder));
		return LinkLineContains(p) || flag || flag2;
	}

	private bool LinkLineContains(Point p)
	{
		if (LinkStyle == LinkStyleEnum.LinkStyle.Orthogonal)
		{
			return OrthogonalLineContains(p);
		}
		return SqrDistance(FromNodeArrow.UpperPoint, ToNodeArrow.UpperPoint, p) < (double)MouseOverSqrBorder;
	}

	private bool OrthogonalLineContains(Point p)
	{
		Point upperPoint = FromNodeArrow.UpperPoint;
		Point upperPoint2 = ToNodeArrow.UpperPoint;
		Point[] linkSubpoints = GetLinkSubpoints(upperPoint, upperPoint2);
		int num = linkSubpoints.Length;
		if (num == 0)
		{
			return SqrDistance(upperPoint, upperPoint2, p) < (double)MouseOverSqrBorder;
		}
		if (SqrDistance(upperPoint, linkSubpoints.First(), p) < (double)MouseOverSqrBorder)
		{
			return true;
		}
		if (SqrDistance(linkSubpoints.Last(), upperPoint2, p) < (double)MouseOverSqrBorder)
		{
			return true;
		}
		if (num > 1)
		{
			for (int i = 0; i < num - 1; i++)
			{
				if (SqrDistance(linkSubpoints[i], linkSubpoints[i + 1], p) < (double)MouseOverSqrBorder)
				{
					return true;
				}
			}
		}
		return false;
	}

	private double SqrDistance(PointF v, PointF w, PointF p)
	{
		double num = w.X - v.X;
		double num2 = w.Y - v.Y;
		double num3 = num * num + num2 * num2;
		double val = ((double)(p.X - v.X) * num + (double)(p.Y - v.Y) * num2) / num3;
		val = Math.Max(0.0, Math.Min(1.0, val));
		double num4 = (double)v.X + val * num;
		double num5 = (double)v.Y + val * num2;
		double num6 = num4 - (double)p.X;
		double num7 = num5 - (double)p.Y;
		return num6 * num6 + num7 * num7;
	}

	public override XmlElement ToSvg(XmlDocument xml, Output destination)
	{
		XmlElement xmlElement = xml.CreateElement("g");
		if (FromNode.Deleted || ToNode.Deleted || Hidden == true)
		{
			return xmlElement;
		}
		FromNodeArrow.RecalculatePosition(FromNode, ToNode);
		ToNodeArrow.RecalculatePosition(FromNode, ToNode);
		string text = string.Empty;
		if (!string.IsNullOrEmpty(Title))
		{
			text = text + "<div style=\"margin-bottom: 8px;\"><b>" + Title + "</b></div>";
		}
		text = (string.IsNullOrEmpty(Description) ? (text + JoinCondition) : (text + "<div style=\"margin-bottom: 8px;\">" + JoinCondition + "</div>" + Description.Replace(Environment.NewLine, "<br/>")));
		Point upperPoint = FromNodeArrow.UpperPoint;
		Point upperPoint2 = ToNodeArrow.UpperPoint;
		bool flag = upperPoint.X == upperPoint2.X || upperPoint.Y == upperPoint2.Y;
		if (LinkStyle == LinkStyleEnum.LinkStyle.Orthogonal)
		{
			_ = !flag;
		}
		else
			_ = 0;
		List<Point> list = new List<Point>(new Point[2] { FromNodeArrow.LowerCenterPoint, FromNodeArrow.UpperPoint });
		if (LinkStyle == LinkStyleEnum.LinkStyle.Orthogonal && !flag)
		{
			list.AddRange(GetLinkSubpoints(upperPoint, upperPoint2));
		}
		list.Add(ToNodeArrow.UpperPoint);
		list.Add(ToNodeArrow.LowerCenterPoint);
		DrawSvgHintLineWithHint(xml, xmlElement, text, destination, list.ToArray());
		if (FromNodeArrow.IsFullArrow)
		{
			DrawSvgLine(xml, xmlElement, destination, FromNodeArrow.LowerLeftPoint, FromNodeArrow.MiddlePoint, FromNodeArrow.LowerRightPoint);
		}
		if (ToNodeArrow.IsFullArrow)
		{
			DrawSvgLine(xml, xmlElement, destination, ToNodeArrow.LowerLeftPoint, ToNodeArrow.MiddlePoint, ToNodeArrow.LowerRightPoint);
		}
		return xmlElement;
	}

	private void DrawSvgLine(XmlDocument xml, XmlElement g, Output destination, params Point[] points)
	{
		XmlElement xmlElement = xml.CreateElement("polyline");
		xmlElement.SetAttribute("stroke", ColorTranslator.ToHtml((destination == Output.Control) ? SkinsManager.CurrentSkin.ErdGrayDark : SkinsManager.DefaultSkin.ErdGrayDark));
		xmlElement.SetAttribute("fill", "transparent");
		xmlElement.SetAttribute("points", string.Join(" ", points.Select((Point x) => $"{x.X}, {x.Y}")) ?? "");
		g.AppendChild(xmlElement);
	}

	private void DrawSvgHintLine(XmlDocument xml, XmlElement g, string hint, params Point[] points)
	{
		XmlElement xmlElement = xml.CreateElement("polyline");
		xmlElement.SetAttribute("data-msg", hint);
		xmlElement.SetAttribute("data-relation", null);
		xmlElement.SetAttribute("data-relation-pk-table-id", FromNode.TableId.ToString());
		xmlElement.SetAttribute("data-relation-fk-table-id", ToNode.TableId.ToString());
		xmlElement.SetAttribute("data-relation-pk-column-ids", string.Join(",", Columns.Select((RelationColumnRow x) => x.ColumnPkId)));
		xmlElement.SetAttribute("data-relation-fk-column-ids", string.Join(",", Columns.Select((RelationColumnRow x) => x.ColumnFkId)));
		xmlElement.SetAttribute("stroke", "transparent");
		xmlElement.SetAttribute("stroke-width", "10");
		xmlElement.SetAttribute("fill", "transparent");
		xmlElement.SetAttribute("points", string.Join(" ", points.Select((Point x) => $"{x.X}, {x.Y}")) ?? "");
		g.AppendChild(xmlElement);
	}

	private void DrawSvgHintLineWithHint(XmlDocument xml, XmlElement g, string hint, Output destination, params Point[] points)
	{
		DrawSvgLine(xml, g, destination, points);
		DrawSvgHintLine(xml, g, hint, points);
	}

	public bool Equals(Link x, Link y)
	{
		return x.Key == y.Key;
	}

	public int GetHashCode(Link obj)
	{
		return base.Key.GetHashCode();
	}
}
