using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.App.Tools.ERD.Canvas;
using Dataedo.App.Tools.Export;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.UI;
using Dataedo.Model.Data.Erd;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.Tools.ERD.Diagram;

[DebuggerDisplay("{DisplayName}")]
public class Node : RectangularObject, INode
{
	public readonly Color PATH_COLOR = Color.FromArgb(150, 150, 150);

	private Point _Position = Point.Empty;

	private string _Label = "";

	private NodeTypeEnum.NodeType _Type;

	public bool _Deleted;

	private bool _Selected;

	private Color _Color = SkinsManager.DefaultSkin.ErdNodeDefault;

	private bool columnsChanged;

	private string title;

	private int width = 220;

	private bool isInSubjectArea;

	internal List<Link> inLinks = new List<Link>();

	internal List<Link> outLinks = new List<Link>();

	private Font defaultFont;

	private StringFormat labelFormat;

	private StringFormat nodeColumnFormat;

	private StringFormat nodeColumnDataTypeFormat;

	private int nodeTextHeight;

	public override IEnumerable<EdgeToResize> AvailableResizeEdges => new HashSet<EdgeToResize>
	{
		EdgeToResize.None,
		EdgeToResize.Left,
		EdgeToResize.Right
	};

	public bool? IsMultipleSchemasDatabase { get; set; }

	public bool? DatabaseShowSchema { get; set; }

	public bool? DatabaseShowSchemaOverride { get; set; }

	public bool ShowSchema
	{
		get
		{
			if (DatabaseShowSchemaOverride != true)
			{
				if (!DatabaseShowSchemaOverride.HasValue)
				{
					return DatabaseShowSchema == true;
				}
				return false;
			}
			return true;
		}
	}

	public string ObjectIdString => Paths.EncodeInvalidPathCharacters(ShowSchema ? (Schema + "_" + Label) : Label);

	public string IdStringForPath => $"{ObjectIdString}_{TableId}";

	public SharedObjectTypeEnum.ObjectType ObjectType
	{
		get
		{
			SharedObjectTypeEnum.ObjectType result = SharedObjectTypeEnum.ObjectType.Structure;
			if (Type == NodeTypeEnum.NodeType.Table)
			{
				result = SharedObjectTypeEnum.ObjectType.Table;
			}
			else if (Type == NodeTypeEnum.NodeType.View)
			{
				result = SharedObjectTypeEnum.ObjectType.View;
			}
			return result;
		}
	}

	public override Point Position
	{
		get
		{
			return _Position;
		}
		set
		{
			if (Visible)
			{
				if (value == _Position)
				{
					RecalcLabelBox();
					RecalcColumnsBox();
					return;
				}
				_Position = value;
				RecalcLabelBox();
				RecalcColumnsBox();
				NotifyChange();
				NotifyImportantChange();
			}
		}
	}

	private int labelBoxHeight
	{
		get
		{
			if ((!string.IsNullOrWhiteSpace(Title) && !showDatabaseName) || (string.IsNullOrWhiteSpace(Title) && showDatabaseName))
			{
				return 38;
			}
			if (!string.IsNullOrWhiteSpace(Title) && showDatabaseName)
			{
				return 52;
			}
			return 24;
		}
	}

	public override Size Size => new Size(Width, labelBoxHeight);

	public Rectangle LabelBox { get; private set; }

	public override Rectangle BoxToIntersect => LabelBox;

	public string Label
	{
		get
		{
			return _Label;
		}
		set
		{
			if (value == null)
			{
				throw new ArgumentNullException();
			}
			_Label = value;
			if (Visible)
			{
				NotifyChange();
			}
		}
	}

	public NodeTypeEnum.NodeType Type
	{
		get
		{
			return _Type;
		}
		set
		{
			if (value != _Type)
			{
				_Type = value;
				if (Visible)
				{
					NotifyChange();
					NotifyImportantChange();
				}
			}
		}
	}

	public bool Visible { get; set; }

	public bool IsTableAddedToDatabase { get; set; }

	public bool Deleted
	{
		get
		{
			return _Deleted;
		}
		set
		{
			if (value != _Deleted)
			{
				_Deleted = value;
				if (Visible)
				{
					NotifyChange();
					NotifyImportantChange();
				}
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
			if (Visible)
			{
				NotifyChange();
			}
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
				if (Visible)
				{
					NotifyChange();
					NotifyImportantChange();
				}
			}
		}
	}

	public bool ColumnsChanged
	{
		get
		{
			return columnsChanged;
		}
		set
		{
			columnsChanged = value;
			NotifyChange();
			NotifyImportantChange();
		}
	}

	public int ImageIndex
	{
		get
		{
			switch (Type)
			{
			case NodeTypeEnum.NodeType.Table:
				if (!Deleted)
				{
					return 0;
				}
				return 2;
			case NodeTypeEnum.NodeType.View:
				if (!Deleted)
				{
					return 1;
				}
				return 3;
			default:
				throw new Exception("Unknown node type (cannot get icon)");
			}
		}
	}

	public Image NodeIcon
	{
		get
		{
			switch (Type)
			{
			case NodeTypeEnum.NodeType.Table:
				if (!Deleted)
				{
					return IconsSupport.GetObjectIcon(ObjectType, TableSubtype, TableSource, SynchronizeStateEnum.SynchronizeState.Synchronized);
				}
				return Resources.table_delete_16;
			case NodeTypeEnum.NodeType.View:
				if (!Deleted)
				{
					return IconsSupport.GetObjectIcon(ObjectType, TableSubtype, TableSource, SynchronizeStateEnum.SynchronizeState.Synchronized);
				}
				return Resources.view_delete_16;
			case NodeTypeEnum.NodeType.Structure:
				if (!Deleted)
				{
					return IconsSupport.GetObjectIcon(ObjectType, TableSubtype, TableSource, SynchronizeStateEnum.SynchronizeState.Synchronized);
				}
				return Resources.object_deleted_16;
			default:
				throw new Exception("Unknown node type (cannot get icon)");
			}
		}
	}

	public List<Node> RelatedNodes { get; set; }

	public int CurrentDiagramDatabaseId { get; set; }

	public bool CurrentDiagramDatabaseShowSchema { get; set; }

	public string Schema { get; set; }

	public int TableId { get; set; }

	public int DatabaseId { get; set; }

	public SharedDatabaseTypeEnum.DatabaseType? DatabaseType { get; set; }

	public string DatabaseName { get; set; }

	public ERDNodeSource NodeSource { get; set; }

	public DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode DisplayDocumentationNameMode { get; set; }

	public bool IsFromCurrentDatabase => CurrentDiagramDatabaseId == DatabaseId;

	private bool showDatabaseName
	{
		get
		{
			if (DisplayDocumentationNameMode == DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode.AllEntities || (DisplayDocumentationNameMode == DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode.ExternalEntitiesOnly && !IsFromCurrentDatabase))
			{
				return true;
			}
			return false;
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
			}
			RecalcLabelBox();
		}
	}

	public string Href { get; set; }

	public string DisplayNameWithoutTitle => ObjectNames.GetTableObjectName(null, DatabaseId, string.Empty, ShowSchema, TableId, Schema, Label, CurrentDiagramDatabaseId, CurrentDiagramDatabaseShowSchema, useDatabaseName: true);

	public string DisplayName
	{
		get
		{
			StringBuilder stringBuilder = new StringBuilder(ObjectNames.GetTableObjectName(null, DatabaseId, DatabaseName, ShowSchema, TableId, Schema, Label, CurrentDiagramDatabaseId, CurrentDiagramDatabaseShowSchema, useDatabaseName: true));
			if (ValidateFields.IsFieldNotEmpty(Title))
			{
				stringBuilder.Append(" (" + Title + ")");
			}
			return stringBuilder.ToString();
		}
	}

	public string DisplayNameUiEscaped => Escaping.EscapeTextForUI(DisplayName);

	public bool ShowColumnsDataTypes { get; set; }

	public bool ShowNullable { get; set; }

	public List<NodeColumnDB> Columns { get; set; }

	public bool IsRelatedToSelected { get; set; }

	public bool IsSuggested { get; set; }

	public List<NodeRelationContainer> RelationsPkTables { get; set; }

	public List<NodeRelationContainer> RelationsFkTables { get; set; }

	public UserTypeEnum.UserType TableSource { get; set; }

	public SharedObjectTypeEnum.ObjectType TableType => ObjectType;

	public SharedObjectSubtypeEnum.ObjectSubtype TableSubtype { get; set; }

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
				RecalcLabelBox();
			}
		}
	}

	public override int Height
	{
		get
		{
			return base.Box.Height;
		}
		set
		{
		}
	}

	public override RectangularObjectType RectangularObjectType => RectangularObjectType.Node;

	public bool IsInSubjectArea
	{
		get
		{
			return isInSubjectArea;
		}
		set
		{
			if (value != isInSubjectArea)
			{
				isInSubjectArea = value;
				if (Visible)
				{
					NotifyChange();
					NotifyImportantChange();
				}
			}
		}
	}

	private Rectangle ColumnsBox { get; set; }

	private IEnumerable<NodeColumnDB> selectedColumns => Columns?.Where((NodeColumnDB x) => x.Selected);

	internal void SetActiveColumn(IEnumerable<int> columnIds, bool focused)
	{
		foreach (int columnId in columnIds)
		{
			NodeColumnDB nodeColumnDB = Columns.FirstOrDefault((NodeColumnDB x) => x.ColumnId == columnId);
			if (nodeColumnDB != null)
			{
				nodeColumnDB.Focused = focused;
			}
		}
	}

	public Node(int currentDiagramDatabaseId, bool currentDiagramDatabaseShowSchema, int databaseId, bool? isMultipleSchemasDatabase, bool? databaseShowSchema, bool? databaseShowSchemaOverride, int key, string label, Point position, NodeTypeEnum.NodeType type, bool deleted, bool visible, UserTypeEnum.UserType source = UserTypeEnum.UserType.DBMS, SharedObjectSubtypeEnum.ObjectSubtype subtype = SharedObjectSubtypeEnum.ObjectSubtype.Table, bool isTableAddedToDatabase = true)
	{
		IsTableAddedToDatabase = isTableAddedToDatabase;
		CurrentDiagramDatabaseId = currentDiagramDatabaseId;
		CurrentDiagramDatabaseShowSchema = currentDiagramDatabaseShowSchema;
		IsMultipleSchemasDatabase = isMultipleSchemasDatabase;
		DatabaseShowSchema = databaseShowSchema;
		DatabaseShowSchemaOverride = databaseShowSchemaOverride;
		DatabaseId = databaseId;
		base.MouseEnter += base.OnMouseEnter;
		base.MouseLeave += base.OnMouseLeave;
		Visible = visible;
		base.Key = key;
		Label = label;
		Position = position;
		Type = type;
		Deleted = deleted;
		RelationsFkTables = new List<NodeRelationContainer>();
		RelationsPkTables = new List<NodeRelationContainer>();
		RelatedNodes = new List<Node>();
		TableSource = source;
		TableSubtype = subtype;
		Columns = new List<NodeColumnDB>();
		ColumnsBox = default(Rectangle);
		defaultFont = new Font("Arial", Styles.NodeTextSize);
		nodeTextHeight = (int)(defaultFont.Size + (float)Styles.NodeVerticalPadding);
		labelFormat = new StringFormat
		{
			Alignment = StringAlignment.Near,
			LineAlignment = StringAlignment.Center,
			FormatFlags = StringFormatFlags.NoWrap,
			Trimming = StringTrimming.EllipsisCharacter
		};
		nodeColumnFormat = new StringFormat
		{
			FormatFlags = StringFormatFlags.NoWrap,
			Trimming = StringTrimming.None
		};
		nodeColumnDataTypeFormat = new StringFormat
		{
			Alignment = StringAlignment.Far,
			FormatFlags = StringFormatFlags.NoWrap,
			Trimming = StringTrimming.None
		};
	}

	public void ShowColumns(bool show)
	{
		foreach (NodeColumnDB column in Columns)
		{
			column.Selected = show;
		}
	}

	public void ShowKeyColumns()
	{
		Columns.ForEach(delegate(NodeColumnDB x)
		{
			x.Selected = false;
		});
		foreach (NodeColumnDB item in Columns.Where((NodeColumnDB x) => x.UniqueConstraintsDataContainer.IsKey))
		{
			item.Selected = item.UniqueConstraintsDataContainer.IsKey;
			ColumnsSelector.SelectParentColumns(item, Columns);
		}
	}

	public void ShowPrimaryKeyColumns()
	{
		foreach (NodeColumnDB column in Columns)
		{
			if (column.PrimaryKey)
			{
				column.Selected = true;
			}
		}
	}

	public void SetColumnsAfterEdition(IEnumerable<string> insertedColumns = null, Form owner = null)
	{
		IEnumerable<NodeColumnDB> enumerable = from x in DB.NodeColumn.GetErdColumns(base.SubjectAreaId, TableId)
			group x by x.ColumnId into x
			select x.FirstOrDefault() into x
			select new NodeColumnDB(x, base.SubjectAreaId);
		Columns = PrepareColumns(enumerable.ToList());
		RemoveColumnsFromERD(enumerable);
		AddNewColumnsToRemove(insertedColumns, Columns);
		List<int> list = (from x in Columns
			where x.Selected
			select x.ColumnId).ToList();
		foreach (NodeColumnDB column in Columns)
		{
			if (list.Contains(column.ColumnId))
			{
				column.Selected = true;
			}
		}
		if (Id.HasValue)
		{
			DB.NodeColumn.InsertOrUpdateErdNodeColumns(Columns, owner);
		}
	}

	private List<NodeColumnDB> PrepareColumns(List<NodeColumnDB> columns)
	{
		foreach (NodeColumnDB column in columns)
		{
			column.Selected = Columns.FirstOrDefault((NodeColumnDB x) => x.ColumnId == column.ColumnId)?.Selected ?? false;
		}
		return columns;
	}

	private void AddNewColumnsToRemove(IEnumerable<string> insertedColumns, IEnumerable<NodeColumnDB> columns)
	{
		if (insertedColumns == null)
		{
			return;
		}
		foreach (NodeColumnDB item in columns.Where((NodeColumnDB x) => insertedColumns.Any((string y) => y.Equals(x.Name))).ToList())
		{
			item.Selected = true;
		}
	}

	private void RemoveColumnsFromERD(IEnumerable<NodeColumnDB> columns)
	{
		NodeColumnDB[] array = Columns.Where((NodeColumnDB x) => !columns.Any((NodeColumnDB y) => y.ColumnId == x.ColumnId)).ToArray();
		for (int num = array.Length - 1; num >= 0; num--)
		{
			Columns.Remove(array[num]);
		}
		NodeColumnDB[] array2 = array;
		foreach (NodeColumnDB item in array2)
		{
			Columns.Remove(item);
		}
	}

	private void AppendChildColumns(IEnumerable<NodeColumnDB> specificLevelSubColumns, List<NodeColumnDB> allColumns, IList<NodeColumnDB> result, OtherFieldsSupport otherFieldsSupport)
	{
		if (specificLevelSubColumns == null)
		{
			return;
		}
		foreach (NodeColumnDB column in specificLevelSubColumns?.OrderBy((NodeColumnDB x) => x.Sort)?.ThenBy((NodeColumnDB x) => x.OrdinalPosition ?? 99999))
		{
			result.Add(column);
			if (otherFieldsSupport != null && !otherFieldsSupport.IsSelected(OtherFieldEnum.OtherField.Title))
			{
				column.Title = null;
			}
			AppendChildColumns(allColumns.Where((NodeColumnDB x) => x.ParentId == column.ColumnId).ToList(), allColumns, result, otherFieldsSupport);
		}
	}

	internal void SetColumns(bool selectPrimaryKeyColumns = false, bool selectUniqueKeyColumns = false, bool selectFkColumns = false, OtherFieldsSupport otherFieldsSupport = null)
	{
		IEnumerable<NodeColumnDB> source = (from x in DB.NodeColumn.GetErdColumns(base.SubjectAreaId, TableId)
			select new NodeColumnDB(x, base.SubjectAreaId) into x
			group x by x.ColumnId).Select(delegate(IGrouping<int, NodeColumnDB> x)
		{
			NodeColumnDB nodeColumnDB = x.First();
			nodeColumnDB.UniqueConstraintsDataContainer.Data = x.SelectMany((NodeColumnDB y) => y.UniqueConstraintsDataContainer.Data).ToList();
			return nodeColumnDB;
		});
		IOrderedEnumerable<NodeColumnDB> specificLevelSubColumns = from x in source
			where !x.ParentId.HasValue
			orderby x.Sort, x.OrdinalPosition ?? 99999
			select x;
		List<NodeColumnDB> list = new List<NodeColumnDB>();
		AppendChildColumns(specificLevelSubColumns, source.ToList(), list, otherFieldsSupport);
		Columns.AddRange(list);
		if (!IsTableAddedToDatabase)
		{
			Columns.ForEach(delegate(NodeColumnDB x)
			{
				ColumnsSelector.SelectParentColumns(x, Columns);
				x.Selected = true;
			});
		}
		else if (selectPrimaryKeyColumns || selectUniqueKeyColumns)
		{
			foreach (NodeColumnDB column in Columns)
			{
				if ((selectPrimaryKeyColumns && column.UniqueConstraintsDataContainer.IsAnyActivePk) || (selectUniqueKeyColumns && column.UniqueConstraintsDataContainer.IsAnyActiveUk) || (selectFkColumns && column.UniqueConstraintsDataContainer.IsAnyFk))
				{
					column.Selected = true;
					ColumnsSelector.SelectParentColumns(column, Columns);
				}
			}
		}
		RefreshColumnsListString();
	}

	protected void NotifyImportantChange()
	{
		ImportantChange?.Invoke(this, EventArgs.Empty);
	}

	public void Translate(int x, int y)
	{
		Position = new Point(Position.X + x, Position.Y + y);
	}

	private void RecalcLabelBox()
	{
		LabelBox = new Rectangle(Position.X - Size.Width / 2, Position.Y - labelBoxHeight / 2, Size.Width, labelBoxHeight);
		RecalcBox();
	}

	private void RecalcColumnsBox()
	{
		if (selectedColumns != null)
		{
			int num = selectedColumns.Count();
			if (num > 0)
			{
				ColumnsBox = new Rectangle(LabelBox.X, LabelBox.Y + LabelBox.Height, LabelBox.Width, num * nodeTextHeight + 2 * Styles.NodeVerticalPadding);
			}
			else
			{
				ColumnsBox = default(Rectangle);
			}
			RecalcBox();
		}
	}

	private void RecalcBox()
	{
		base.Box = new Rectangle(LabelBox.X, LabelBox.Y, LabelBox.Width, LabelBox.Height + ColumnsBox.Height);
	}

	public override Point GetNodePositionFromStartPoint(int x, int y)
	{
		return new Point(x + base.Box.Width / 2, y + LabelBox.Height / 2);
	}

	public override Point GetNodePositionFromEndPoint(int x, int y)
	{
		return new Point(x - base.Box.Width / 2, y - ColumnsBox.Height - LabelBox.Height / 2);
	}

	public void RefreshColumnsListString(bool withRecalcColumnsBox = true)
	{
		if (withRecalcColumnsBox)
		{
			RecalcColumnsBox();
		}
	}

	public void RecalcLinksArrows()
	{
		ResetLinksArrows();
		RecalcLinkArrows(ArrowDirectionEnum.Up);
		RecalcLinkArrows(ArrowDirectionEnum.Down);
		RecalcLinkArrows(ArrowDirectionEnum.Left);
		RecalcLinkArrows(ArrowDirectionEnum.Right);
	}

	private void ResetLinksArrows()
	{
		foreach (NodeAndArrow item in inLinks.Select((Link x) => new NodeAndArrow(x.FromNode, x.ToNodeArrow, x.RelationId)).Union(outLinks.Select((Link x) => new NodeAndArrow(x.ToNode, x.FromNodeArrow, x.RelationId))))
		{
			item.Arrow.ClearShift();
		}
	}

	private void RecalcLinkArrows(ArrowDirectionEnum direction)
	{
		IEnumerable<NodeAndArrow> enumerable = GetNodesOfDirection(direction);
		if (enumerable.Any((NodeAndArrow x) => x.NodeValue == null))
		{
			return;
		}
		int num = enumerable.Count();
		if (num == 0)
		{
			return;
		}
		bool vertical = direction == ArrowDirectionEnum.Left || direction == ArrowDirectionEnum.Right;
		if (direction == ArrowDirectionEnum.Up || direction == ArrowDirectionEnum.Left)
		{
			enumerable = enumerable.Reverse();
		}
		List<NodeAndArrow> list = NodeAndArrow.OrderByArrowsPositions(enumerable, direction).ToList();
		int num2 = num / 2;
		bool num3 = num % 2 == 1;
		int num4 = ((direction == ArrowDirectionEnum.Up || direction == ArrowDirectionEnum.Down) ? (Width / (num * 2)) : (Height / (num * 2)));
		num4 *= 2;
		if (num3)
		{
			for (int i = 1; i <= num2; i++)
			{
				list[num2 + i].Arrow.SetShift(i * num4, vertical);
				list[num2 - i].Arrow.SetShift(-i * num4, vertical);
			}
		}
		else
		{
			for (int j = 0; j < num2; j++)
			{
				list[j].Arrow.SetShift(-(num2 - j - 1) * num4 - num4 / 2, vertical);
				list[j + num2].Arrow.SetShift(j * num4 + num4 / 2, vertical);
			}
		}
	}

	private IEnumerable<NodeAndArrow> GetNodesOfDirection(ArrowDirectionEnum direction)
	{
		return (from x in inLinks
			where x.ToNodeArrow.ArrowDirection == direction
			orderby x.Key
			select new NodeAndArrow(x.FromNode, x.ToNodeArrow, x.RelationId)).Union(from x in outLinks
			where x.FromNodeArrow.ArrowDirection == direction
			orderby x.Key
			select new NodeAndArrow(x.ToNode, x.FromNodeArrow, x.RelationId));
	}

	public override bool IsOnEdge(Point cursorPos)
	{
		return IsOnVerticalEdge(cursorPos);
	}

	public override void Render(Graphics g, Point startPoint, Output dest = Output.Control)
	{
		if (dest == Output.Image && Deleted)
		{
			return;
		}
		g.SmoothingMode = SmoothingMode.HighSpeed;
		Pen pen = SetBrush(dest);
		SolidBrush solidBrush = new SolidBrush(Styles.NodeBackgroundColor(this, dest));
		SolidBrush solidBrush2 = new SolidBrush((dest == Output.Control) ? SkinsManager.CurrentSkin.ErdNodeForeColor : SkinsManager.DefaultSkin.ErdNodeForeColor);
		if (dest != 0)
		{
			_ = SkinsManager.DefaultSkin.ErdNodeDefault;
		}
		else
		{
			_ = SkinsManager.CurrentSkin.ErdNodeDefault;
		}
		Color color = ((dest == Output.Control) ? SkinsManager.CurrentSkin.ErdNodeBorderColor : SkinsManager.DefaultSkin.ErdNodeBorderColor);
		Color color2 = ((dest == Output.Control) ? SkinsManager.CurrentSkin.ErdNodeBackColor : SkinsManager.DefaultSkin.ErdNodeBackColor);
		Color color3 = ((dest == Output.Control) ? SkinsManager.CurrentSkin.ErdFocusedColumnBackColor : SkinsManager.DefaultSkin.ErdFocusedColumnBackColor);
		Color foreColor = ((dest == Output.Control) ? SkinsManager.CurrentSkin.ErdNodeForeColor : SkinsManager.DefaultSkin.ErdNodeForeColor);
		if (Deleted)
		{
			pen.DashPattern = new float[2] { 4f, 3f };
		}
		Rectangle rectangle = new Rectangle(startPoint.X + LabelBox.X + Styles.NodeIconSize + Styles.NodeHorizontalPadding, startPoint.Y + LabelBox.Y, LabelBox.Width - Styles.NodeIconSize - 2 * Styles.NodeHorizontalPadding, LabelBox.Height);
		if (selectedColumns.Count() > 0)
		{
			using (Pen pen2 = new Pen(color))
			{
				g.FillRectangle(new SolidBrush(color2), startPoint.X + ColumnsBox.X, startPoint.Y + ColumnsBox.Y, ColumnsBox.Width, ColumnsBox.Height);
				g.DrawRectangle(pen2, startPoint.X + ColumnsBox.X, startPoint.Y + ColumnsBox.Y, ColumnsBox.Width, ColumnsBox.Height);
			}
			int num = startPoint.Y + ColumnsBox.Top + Styles.NodeVerticalPadding;
			int num2 = (ShowNullable ? (Styles.NodeIconSize + Styles.NodeHorizontalPadding) : 0);
			foreach (NodeColumnDB selectedColumn in selectedColumns)
			{
				Bitmap image = (selectedColumn.IsNullable ? Resources.nullable_16 : Resources.empty_image);
				g.DrawImage(selectedColumn.UniqueConstraintIcon, new Rectangle(startPoint.X + LabelBox.X + Styles.NodeHorizontalPadding, num + 2, Styles.NodeIconSize, Styles.NodeIconSize));
				if (ShowNullable)
				{
					g.DrawImage(image, new Rectangle(rectangle.X + rectangle.Width - num2, num + 2, Styles.NodeIconSize, Styles.NodeIconSize));
				}
				if (ShowColumnsDataTypes)
				{
					int num3 = (int)g.MeasureString(selectedColumn.DataType, defaultFont).Width + 10 + num2;
					g.DrawString(selectedColumn.DisplayPath, defaultFont, new SolidBrush(PATH_COLOR), new Rectangle(rectangle.X, num, rectangle.Width - num3, nodeTextHeight), nodeColumnFormat);
					if (rectangle.Width - num3 - GetPathHorizontalTranslation(selectedColumn, g) > 0)
					{
						g.DrawString(selectedColumn.DisplayName, defaultFont, solidBrush2, new Rectangle(rectangle.X + GetPathHorizontalTranslation(selectedColumn, g), num, rectangle.Width - num3 - GetPathHorizontalTranslation(selectedColumn, g), nodeTextHeight), nodeColumnFormat);
					}
					g.DrawString(selectedColumn.DataType, defaultFont, solidBrush2, new Rectangle(rectangle.X + rectangle.Width - num3 - num2, num, num3, nodeTextHeight), nodeColumnDataTypeFormat);
				}
				else
				{
					if (selectedColumn.Focused)
					{
						SolidBrush brush = new SolidBrush(color3);
						g.FillRectangle(brush, new Rectangle(rectangle.X, num, rectangle.Width, nodeTextHeight));
					}
					g.DrawString(selectedColumn.DisplayPath, defaultFont, new SolidBrush(PATH_COLOR), new Rectangle(rectangle.X, num, rectangle.Width, nodeTextHeight), nodeColumnFormat);
					int num4 = rectangle.Width - GetPathHorizontalTranslation(selectedColumn, g) - 10 - num2;
					if (num4 > 0)
					{
						g.DrawString(selectedColumn.DisplayName, defaultFont, solidBrush2, new Rectangle(rectangle.X + GetPathHorizontalTranslation(selectedColumn, g), num, num4, nodeTextHeight), nodeColumnFormat);
					}
				}
				num += nodeTextHeight;
			}
		}
		DrawBackgroundBox(startPoint, pen, solidBrush, g);
		g.DrawImage(NodeIcon, new RectangleF(startPoint.X + LabelBox.X + Styles.NodeHorizontalPadding, startPoint.Y + LabelBox.Y + LabelBox.Height / 2 - Styles.NodeIconSize / 2, Styles.NodeIconSize, Styles.NodeIconSize));
		if (IsInSubjectArea)
		{
			g.DrawImage(Resources.module_16, new RectangleF(startPoint.X + LabelBox.X + LabelBox.Width - Styles.NodeHorizontalPadding - Styles.NodeIconSize, startPoint.Y + LabelBox.Y + LabelBox.Height / 2 - Styles.NodeIconSize / 2, Styles.NodeIconSize, Styles.NodeIconSize));
		}
		Rectangle bounds = new Rectangle(rectangle.X, rectangle.Y, rectangle.Width - Styles.NodeHorizontalPadding - Styles.NodeIconSize, 24);
		if (showDatabaseName)
		{
			TextRenderer.DrawText(g, "[" + DatabaseName + "]", defaultFont, bounds, foreColor, Styles.NodeFormatFlags);
			bounds.Y += 14;
		}
		TextRenderer.DrawText(g, DisplayNameWithoutTitle, defaultFont, bounds, foreColor, Styles.NodeFormatFlags);
		bounds.Y += 14;
		if (!string.IsNullOrWhiteSpace(Title))
		{
			TextRenderer.DrawText(g, "(" + Title + ")", defaultFont, bounds, foreColor, Styles.NodeFormatFlags);
		}
		solidBrush.Dispose();
		pen.Dispose();
		solidBrush2.Dispose();
	}

	protected override bool IsSizeValid(int width, int height)
	{
		return width >= 220;
	}

	public override bool Contains(Point p)
	{
		return base.Box.Contains(p);
	}

	public override XmlElement ToSvg(XmlDocument xml, Output destination)
	{
		XmlElement xmlElement = xml.CreateElement("g");
		xmlElement.SetAttribute("data-table", null);
		xmlElement.SetAttribute("data-table-id", TableId.ToString());
		if (!string.IsNullOrEmpty(Href))
		{
			xmlElement.SetAttribute("href", Href);
			xmlElement.SetAttribute("data-target", "#right");
		}
		xmlElement.SetAttribute("class", "node");
		if (Deleted)
		{
			return xmlElement;
		}
		xmlElement.SetAttribute("transform", $"translate({LabelBox.X},{LabelBox.Y})");
		if (!string.IsNullOrEmpty(Title))
		{
			xmlElement.SetAttribute("data-msg", "<div style=\"margin-bottom: 8px;\">" + DisplayNameWithoutTitle + "</div><strong>" + Title + "</strong>");
		}
		else
		{
			xmlElement.SetAttribute("data-msg", "<div>" + DisplayNameWithoutTitle + "</div>");
		}
		XmlElement xmlElement2 = xml.CreateElement("rect");
		xmlElement2.SetAttribute("width", LabelBox.Width.ToString());
		xmlElement2.SetAttribute("height", LabelBox.Height.ToString());
		xmlElement2.SetAttribute("stroke-width", "1");
		xmlElement2.SetAttribute("stroke-dasharray", Deleted ? "4,3" : "1,0");
		xmlElement2.SetAttribute("fill", ColorTranslator.ToHtml(Styles.NodeBackgroundColor(this, destination)));
		xmlElement2.SetAttribute("stroke", ColorTranslator.ToHtml(Styles.NodeBorderColor(this, destination)));
		xmlElement2.SetAttribute("class", "hoverable");
		XmlElement newChild = CreateIconElement(xml, NodeIcon, Styles.NodeHorizontalPadding - 1, LabelBox.Height / 2 - Styles.NodeIconSize / 2, Styles.NodeIconSize, Styles.NodeIconSize);
		string text = $"node-text-mask-{base.Key}";
		CreateMask(xml, text, Styles.NodeHorizontalPadding + Styles.NodeIconSize, Styles.NodeVerticalPadding, Size.Width - Styles.NodeIconSize - 2 * Styles.NodeHorizontalPadding, labelBoxHeight - 2 * Styles.NodeVerticalPadding);
		int num = Size.Width - (Styles.NodeHorizontalPadding + Styles.NodeIconSize) - Styles.NodeHorizontalPadding / 2;
		int x = Styles.NodeHorizontalPadding + Styles.NodeIconSize;
		int num2 = 0;
		XmlElement xmlElement3 = null;
		if (showDatabaseName)
		{
			xmlElement3 = CanvasObject.CreateTextElement(xml, x, num2, num, 24, "[" + DatabaseName + "]", null, text);
			num2 += 14;
		}
		XmlElement newChild2 = CanvasObject.CreateTextElement(xml, x, num2, num, 24, DisplayNameWithoutTitle, null, text);
		num2 += 14;
		XmlElement xmlElement4 = null;
		if (!string.IsNullOrEmpty(title))
		{
			xmlElement4 = CanvasObject.CreateTextElement(xml, x, num2, num, 24, "(" + Title + ")", null, text);
		}
		if (selectedColumns.Count() > 0)
		{
			xmlElement.AppendChild(CreateColumnsElement(xml, xmlElement, destination));
		}
		xmlElement.AppendChild(xmlElement2);
		xmlElement.AppendChild(newChild);
		xmlElement.AppendChild(newChild2);
		if (xmlElement3 != null)
		{
			xmlElement.AppendChild(xmlElement3);
		}
		if (xmlElement4 != null)
		{
			xmlElement.AppendChild(xmlElement4);
		}
		return xmlElement;
	}

	private int GetPathHorizontalTranslation(NodeColumnDB column, Graphics g)
	{
		float num = g.MeasureString(column.DisplayPath, defaultFont).Width;
		if (!(num > 0f))
		{
			return 0;
		}
		return (int)num;
	}

	private XmlElement CreateColumnsElement(XmlDocument xml, XmlElement g, Output destination)
	{
		XmlElement xmlElement = xml.CreateElement("g");
		if (selectedColumns.Count() > 0)
		{
			xmlElement.SetAttribute("transform", $"translate(0,{LabelBox.Height})");
			XmlElement xmlElement2 = xml.CreateElement("rect");
			xmlElement2.SetAttribute("width", ColumnsBox.Width.ToString());
			xmlElement2.SetAttribute("height", ColumnsBox.Height.ToString());
			xmlElement2.SetAttribute("stroke-width", "1");
			xmlElement2.SetAttribute("fill", ColorTranslator.ToHtml((destination == Output.Control) ? SkinsManager.CurrentSkin.ErdNodeBackColor : SkinsManager.DefaultSkin.ErdNodeBackColor));
			xmlElement2.SetAttribute("stroke", ColorTranslator.ToHtml((destination == Output.Control) ? SkinsManager.CurrentSkin.ErdNodeDefault : SkinsManager.DefaultSkin.ErdNodeDefault));
			xmlElement.AppendChild(xmlElement2);
			Rectangle rectangle = new Rectangle(Styles.NodeIconSize + Styles.NodeHorizontalPadding, 0, LabelBox.Width - Styles.NodeIconSize - 2 * Styles.NodeHorizontalPadding, LabelBox.Height);
			int num = Styles.NodeVerticalPadding;
			foreach (NodeColumnDB selectedColumn in selectedColumns)
			{
				XmlElement xmlElement3 = xml.CreateElement("g");
				xmlElement3.SetAttribute("data-column", null);
				xmlElement3.SetAttribute("data-column-id", selectedColumn.ColumnId.ToString());
				xmlElement3.AppendChild(CreateIconElement(xml, selectedColumn.UniqueConstraintIcon, Styles.NodeHorizontalPadding - 1, num, Styles.NodeIconSize, Styles.NodeIconSize));
				int num2 = (ShowNullable ? (Styles.NodeIconSize + Styles.NodeHorizontalPadding) : 0);
				if (ShowColumnsDataTypes)
				{
					string text = $"node-column-name-mask-{base.Key}-{selectedColumn.ColumnId}";
					CreateMask(xml, text, rectangle.X, num - 3, rectangle.Width / 2, nodeTextHeight);
					xmlElement3.AppendChild(CanvasObject.CreateTextElement(xml, rectangle.X, num - 3, rectangle.Width / 2, nodeTextHeight, selectedColumn.DisplayNameForHtml, null, text, setXml: true));
					text = $"node-column-data-type-mask-{base.Key}-{selectedColumn.ColumnId}";
					CreateMask(xml, text, rectangle.X + rectangle.Width / 2, num - 3, rectangle.Width / 2 - num2, nodeTextHeight);
					xmlElement3.AppendChild(CanvasObject.CreateTextElement(xml, rectangle.X + rectangle.Width / 2, num - 3, rectangle.Width / 2 - num2, nodeTextHeight, selectedColumn.DataType, "right"));
				}
				else
				{
					xmlElement3.AppendChild(CanvasObject.CreateTextElement(xml, rectangle.X, num - 3, rectangle.Width - num2, nodeTextHeight, selectedColumn.DisplayNameForHtml, null, null, setXml: true));
				}
				Bitmap iconImage = (selectedColumn.IsNullable ? Resources.nullable_16 : Resources.empty_image);
				if (ShowNullable)
				{
					xmlElement3.AppendChild(CreateIconElement(xml, iconImage, rectangle.X + rectangle.Width - num2, num, Styles.NodeIconSize, Styles.NodeIconSize));
				}
				num += nodeTextHeight;
				xmlElement.AppendChild(xmlElement3);
			}
			g.AppendChild(xmlElement);
		}
		return xmlElement;
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

	protected override void DrawBackgroundBox(Point startPoint, Pen borderPen, SolidBrush backgroundBrush, Graphics g)
	{
		if (Selected || base.MouseOver)
		{
			g.FillRectangle(backgroundBrush, startPoint.X + LabelBox.X + 1, startPoint.Y + LabelBox.Y, LabelBox.Width - 1, LabelBox.Height);
			g.DrawRectangle(borderPen, startPoint.X + base.Box.X + 1, startPoint.Y + base.Box.Y, base.Box.Width - 1, base.Box.Height);
			if (!SkinsManager.CurrentSkin.ErdUseNodeColorForHeaderBorder)
			{
				g.DrawLine(borderPen, startPoint.X + LabelBox.X, startPoint.Y + LabelBox.Y + LabelBox.Height, startPoint.X + LabelBox.X + LabelBox.Width - 1, startPoint.Y + LabelBox.Y + LabelBox.Height);
			}
		}
		else
		{
			g.FillRectangle(backgroundBrush, startPoint.X + LabelBox.X + 1, startPoint.Y + LabelBox.Y + 1, LabelBox.Width - 1, LabelBox.Height - 1);
			g.DrawRectangle(borderPen, startPoint.X + LabelBox.X, startPoint.Y + LabelBox.Y, LabelBox.Width, LabelBox.Height);
		}
	}

	public override Rectangle GetBoxForNewPosition(int x, int y)
	{
		return new Rectangle(x - Size.Width / 2, y - labelBoxHeight / 2, base.Box.Width, base.Box.Height);
	}
}
