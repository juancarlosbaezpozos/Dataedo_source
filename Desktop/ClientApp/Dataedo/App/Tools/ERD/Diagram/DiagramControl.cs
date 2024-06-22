using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Classes.Synchronize.Tools;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Helpers.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.ERD.Canvas;
using Dataedo.App.Tools.ERD.Canvas.CanvasEventHandlers;
using Dataedo.App.Tools.ERD.Extensions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.UI;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;

namespace Dataedo.App.Tools.ERD.Diagram;

public class DiagramControl : CanvasControl
{
	public delegate void NodeEventHandler(Node node);

	public delegate bool NodeRemoveFromSAEventHandler(Node node);

	public delegate void PostItEventHandler(PostIt postIt);

	public delegate void MouseUpRelatedNodes(List<Node> selectedNodes);

	public delegate void AddNodeEventHandler(int databaseId, Point position, SharedObjectTypeEnum.ObjectType objectType);

	public delegate void AddPostItEventHandler(Point position);

	public delegate void LinkEventHandler(Link link);

	public delegate void SelectionChangedHandler(CanvasObject[] objects);

	public delegate void CMouseEventHandler(Box box, Point point);

	public delegate void LinkStyleEventHandler(LinkStyleEnum.LinkStyle style);

	public delegate void DisplayDocumentationNameModeEventHandler(DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode mode);

	public delegate void ShowColumnOptionEventHandler(bool show);

	private class NodePositions
	{
		public Point Position { get; set; }

		public Point Center { get; set; }

		public NodePositions(Point position, Point center)
		{
			Position = position;
			Center = center;
		}
	}

	public const int HORIZONTAL_MARGIN = 25;

	public const int VERTICAL_MARGIN = 25;

	public static readonly int MaxControlSize = 25000;

	public static readonly int NodesPositionLeftMargin = -MaxControlSize;

	public static readonly int NodesPositionRightMargin = MaxControlSize;

	public static readonly int NodesPositionTopMargin = -MaxControlSize;

	public static readonly int NodesPositionBottomMargin = MaxControlSize;

	public Diagram Diagram = new Diagram();

	public const int GRID_SIZE = 50;

	public const int GRID_ATTRACTION = 10;

	private BarManager barManager;

	private PopupMenu popupMenu = new PopupMenu();

	private ToolTipController toolTipController;

	private SuperToolTip superToolTip;

	public bool ToolTipVisible;

	private bool isDiagramInAddRelationMode;

	private Node addRelationEndNode;

	private ISet<RectangularObjectKey> SelectedElements = new HashSet<RectangularObjectKey>();

	private Link selectedLink;

	private bool activeNodeResizing;

	private bool activeNodeDrag;

	private int resizedNode;

	private EdgeToResize resizedEdge;

	private bool MouseMoveWhileNodeDrag;

	private Point StartNodeDragPoint;

	private Dictionary<int, NodePositions> StartNodeDragPosition;

	private int ClickedOnNodeDrag;

	private MemoEdit postItEditor;

	private Box box;

	private bool SelectionActive;

	private Point SelectionStartPoint;

	private Point SelectionEndPoint;

	private IContainer components;

	public int CurrentDatabaseId { get; set; }

	public bool ContextShowSchema { get; set; }

	public bool ShowColumnTypes { get; set; }

	public bool ShowColumnNullable { get; set; }

	public LinkStyleEnum.LinkStyle? LinkStyle { get; set; } = LinkStyleEnum.LinkStyle.Straight;


	public CustomFieldsSupport CustomFieldsSupport { get; set; }

	public DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode DisplayDocumentationNameMode { get; set; }

	public event NodeEventHandler NodeDblClicked;

	public event PostItEventHandler PostItDblClicked;

	public event NodeEventHandler GoToNodeClicked;

	public event NodeEventHandler NodeDeleted;

	public event NodeEventHandler NewNodeDeleted;

	public event NodeEventHandler ColumnsChanged;

	public event NodeEventHandler AddRelatedTables;

	public event NodeEventHandler NodeAddedToSubjectArea;

	public event NodeRemoveFromSAEventHandler NodeRemovedFromSubjectArea;

	public event MouseUpRelatedNodes LoadRelatedNodes;

	public event AddNodeEventHandler AddNode;

	public event AddPostItEventHandler AddPostIt;

	public event LinkEventHandler LinkDblClicked;

	public event LinkEventHandler LinkAdded;

	public event LinkEventHandler LinkDeleted;

	public event SelectionChangedHandler SelectionChanged;

	public event CMouseEventHandler DragMouseUp;

	public event LinkStyleEventHandler LinkStyleChanged;

	public event DisplayDocumentationNameModeEventHandler DisplayDocumentationNameModeChanged;

	public event ShowColumnOptionEventHandler ShowColumnTypesChanged;

	public event ShowColumnOptionEventHandler ShowColumnNullableChanged;

	public event EventHandler RefreshIgnoredNodes;

	public DiagramControl()
	{
		barManager = new BarManager
		{
			Form = this
		};
		popupMenu = new PopupMenu(barManager);
		UseLayers(Diagram.Elements);
		Diagram.Changed += OnDiagramChanged;
		Diagram.NodeContainerChanged += OnNodeContainerChanged;
		base.KeyDown += OnKeyDown;
		base.MouseDown += OnMouseDown;
		base.MouseMove += OnMouseMove;
		base.MouseUp += OnMouseUp;
		base.MouseDoubleClick += OnMouseDoubleClick;
		base.MouseLeave += OnMouseLeave;
		base.Click += OnClick;
		toolTipController = new ToolTipController
		{
			KeepWhileHovered = true,
			InitialDelay = 0,
			ReshowDelay = 0,
			AutoPopDelay = int.MaxValue,
			AllowHtmlText = true
		};
		superToolTip = new SuperToolTip
		{
			MaxWidth = 768
		};
		popupMenu.Tag = false;
		popupMenu.CloseUp += OnContextMenuClosing;
		isDiagramInAddRelationMode = false;
		InitPostItEditor();
	}

	private void InitPostItEditor()
	{
		postItEditor = new MemoEdit();
		postItEditor.Leave += PostItEditor_Leave;
		postItEditor.Properties.AcceptsReturn = true;
		postItEditor.Properties.MaxLength = 1000;
		postItEditor.Properties.ScrollBars = ScrollBars.None;
		postItEditor.Hide();
		base.Controls.Add(postItEditor);
	}

	public void EditERDLink(Link link, int databaseId, Control caller = null, bool setCustomMessageDefaultOwner = true)
	{
		RelationForm relationForm = new RelationForm(link, databaseId, ContextShowSchema, editMode: true, Elements.Nodes);
		if (caller == null)
		{
			relationForm.ShowDialog();
		}
		else
		{
			relationForm.ShowDialog(caller, setCustomMessageDefaultOwner);
		}
		if (relationForm.DialogResult == DialogResult.OK)
		{
			if (link.FromNode == null)
			{
				Diagram.RemoveLink(link);
				return;
			}
			link.ToNode.SetColumnsAfterEdition(null, FindForm());
			link.ToNode.RefreshColumnsListString();
			AddLinkInformationToColumns(link);
		}
	}

	public static void AddLinkInformationToColumns(Link link)
	{
		foreach (RelationColumnRow linkColumn in link.Columns)
		{
			foreach (NodeColumnDB item in link.ToNode.Columns.Where((NodeColumnDB x) => x.ColumnId == linkColumn.ColumnFkId))
			{
				item.UniqueConstraintsDataContainer.Data.Add(new ColumnUniqueConstraintWithFkData(linkColumn.ColumnPkId));
			}
		}
	}

	public void RemoveLinkInformationFromToNodeColumns(Link link)
	{
		foreach (RelationColumnRow linkColumn in link.Columns)
		{
			ColumnUniqueConstraintWithFkDataContainer columnUniqueConstraintWithFkDataContainer = (from x in link.ToNode.Columns
				where x.ColumnId == linkColumn.ColumnFkId && x.UniqueConstraintsDataContainer.CastedData.Any((ColumnUniqueConstraintWithFkData y) => y.RelationSource == UserTypeEnum.UserType.USER && y.PkColumnID == linkColumn.ColumnPkId)
				select x.UniqueConstraintsDataContainer).FirstOrDefault();
			ColumnUniqueConstraintWithFkData columnUniqueConstraintWithFkData = columnUniqueConstraintWithFkDataContainer?.CastedData.Where((ColumnUniqueConstraintWithFkData x) => x.RelationSource == UserTypeEnum.UserType.USER && x.PkColumnID == linkColumn.ColumnPkId).FirstOrDefault();
			if (columnUniqueConstraintWithFkData != null)
			{
				if (columnUniqueConstraintWithFkData.ConstraintType == UniqueConstraintType.UniqueConstraintTypeEnum.NotSet)
				{
					columnUniqueConstraintWithFkDataContainer.Data.Remove(columnUniqueConstraintWithFkData);
					continue;
				}
				columnUniqueConstraintWithFkData.RelationSource = UserTypeEnum.UserType.NotSet;
				columnUniqueConstraintWithFkData.PkColumnID = null;
			}
		}
	}

	public void RefreshNode(Node node)
	{
		this.ColumnsChanged(node);
	}

	private void OnClick(object sender, EventArgs e)
	{
		Focus();
	}

	private void OnContextMenuClosing(object sender, EventArgs e)
	{
		(sender as PopupMenu).Tag = false;
		PickingMouseOverElement();
	}

	private void InitializeDiagramContexMenu()
	{
		HideHint();
		popupMenu.ItemLinks.Clear();
		popupMenu.AddItem("Select all", Resources.select_all_16, ContextSelectAll);
		popupMenu.AddItem("Copy to clipboard", Resources.copy_16, ContextCopyToClipboard);
		popupMenu.AddItem("Add new table", Resources.table_add_16, ContextAddNewTable);
		popupMenu.StartGroupBeforeLastItem();
		popupMenu.AddItem("Add new view", Resources.view_new_16, ContextAddNewView);
		popupMenu.AddItem("Add new structure", Resources.object_add_16, ContextAddNewStructure);
		popupMenu.AddItem("Add new post-it", Resources.post_it_add_16, ContextAddNewPostIt);
		BarSubItem barSubItem = popupMenu.AddSubItem("Show columns", Resources.columns_show_all_16);
		barSubItem.AddItem("Show all columns", Resources.columns_show_all_16, ContextShowAllColumns);
		barSubItem.AddItem("Show key columns only", Resources.columns_show_key_16, ContextShowKeyColumns);
		barSubItem.AddItem("Hide all columns", Resources.columns_hide_all_16, ContextHideAllColumns);
		popupMenu.StartGroupBeforeLastItem();
		if (ShowColumnTypes)
		{
			popupMenu.AddItem("Hide column types", Resources.hide_types_16, ContextShowColumnTypes);
		}
		else
		{
			popupMenu.AddItem("Show column types", Resources.show_types_16, ContextShowColumnTypes);
		}
		if (ShowColumnNullable)
		{
			popupMenu.AddItem("Hide nullability", Resources.hide_nullable_16, ContextShowColumnNullability);
		}
		else
		{
			popupMenu.AddItem("Show nullability", Resources.nullable_16, ContextShowColumnNullability);
		}
		BarSubItem barSubItem2 = popupMenu.AddSubItem("Display doc name");
		barSubItem2.AddItem(DisplayDocumentationNameModeEnum.ExternalEntitiesOnlyModeDisplayString, Resources.database_16, ContextChangeGlobalDisplayDocumentationNameMode);
		barSubItem2.AddItem(DisplayDocumentationNameModeEnum.AllEntitiesModeDisplayString, Resources.database_16, ContextChangeGlobalDisplayDocumentationNameMode);
		barSubItem2.AddItem(DisplayDocumentationNameModeEnum.NoEntitiesModeDisplayString, Resources.database_16, ContextChangeGlobalDisplayDocumentationNameMode);
		if (DisplayDocumentationNameMode == DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode.AllEntities)
		{
			barSubItem2.ImageOptions.Image = Resources.database_16;
			(barSubItem2.ItemLinks[1].Item as BarButtonItem).ButtonStyle = BarButtonStyle.Check;
			(barSubItem2.ItemLinks[1].Item as BarButtonItem).Down = true;
		}
		else if (DisplayDocumentationNameMode == DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode.NoEntities)
		{
			barSubItem2.ImageOptions.Image = Resources.database_16;
			(barSubItem2.ItemLinks[2].Item as BarButtonItem).ButtonStyle = BarButtonStyle.Check;
			(barSubItem2.ItemLinks[2].Item as BarButtonItem).Down = true;
		}
		else
		{
			barSubItem2.ImageOptions.Image = Resources.database_16;
			(barSubItem2.ItemLinks[0].Item as BarButtonItem).ButtonStyle = BarButtonStyle.Check;
			(barSubItem2.ItemLinks[0].Item as BarButtonItem).Down = true;
		}
		BarSubItem barSubItem3 = popupMenu.AddSubItem("Link style");
		barSubItem3.AddItem(LinkStyleEnum.StraightLinkStyleDisplayString, Resources.link_straight, ContextChangeGlobalLinkStyle);
		barSubItem3.AddItem(LinkStyleEnum.OrthogonalLinkStyleDisplayString, Resources.link_orthogonal, ContextChangeGlobalLinkStyle);
		if (LinkStyle == LinkStyleEnum.LinkStyle.Orthogonal)
		{
			barSubItem3.ImageOptions.Image = Resources.link_orthogonal;
			(barSubItem3.ItemLinks[1].Item as BarButtonItem).ButtonStyle = BarButtonStyle.Check;
			(barSubItem3.ItemLinks[1].Item as BarButtonItem).Down = true;
		}
		else
		{
			barSubItem3.ImageOptions.Image = Resources.link_straight;
			(barSubItem3.ItemLinks[0].Item as BarButtonItem).ButtonStyle = BarButtonStyle.Check;
			(barSubItem3.ItemLinks[0].Item as BarButtonItem).Down = true;
		}
		popupMenu.StartGroupBeforeLastItem();
	}

	private void ContextCopyToClipboard(object sender, EventArgs e)
	{
		ClipboardSupport.SetImage(Diagram.ToImage(Color.White, applyResources: true));
	}

	private void ContextAddNewTable(object sender, EventArgs e)
	{
		this.AddNode?.Invoke(CurrentDatabaseId, PointToClient(Cursor.Position), SharedObjectTypeEnum.ObjectType.Table);
	}

	private void ContextAddNewView(object sender, EventArgs e)
	{
		this.AddNode?.Invoke(CurrentDatabaseId, PointToClient(Cursor.Position), SharedObjectTypeEnum.ObjectType.View);
	}

	private void ContextAddNewStructure(object sender, EventArgs e)
	{
		this.AddNode?.Invoke(CurrentDatabaseId, PointToClient(Cursor.Position), SharedObjectTypeEnum.ObjectType.Structure);
	}

	private void ContextSelectAll(object sender, EventArgs e)
	{
		SelectAllNodes();
	}

	private void ContextChangeGlobalLinkStyle(object sender, ItemClickEventArgs e)
	{
		LinkStyleEnum.LinkStyle linkStyle = LinkStyleEnum.ObjectToType((e.Item as BarButtonItem).Caption);
		LinkStyle = linkStyle;
		this.LinkStyleChanged?.Invoke(linkStyle);
	}

	private void ContextChangeGlobalDisplayDocumentationNameMode(object sender, ItemClickEventArgs e)
	{
		DisplayDocumentationNameModeEnum.DisplayDocumentationNameMode mode = (DisplayDocumentationNameMode = DisplayDocumentationNameModeEnum.ObjectToType((e.Item as BarButtonItem).Caption));
		this.DisplayDocumentationNameModeChanged?.Invoke(mode);
	}

	private void ContextShowColumnTypes(object sender, EventArgs e)
	{
		ShowColumnTypes = !ShowColumnTypes;
		this.ShowColumnTypesChanged?.Invoke(ShowColumnTypes);
	}

	private void ContextShowColumnNullability(object sender, EventArgs e)
	{
		ShowColumnNullable = !ShowColumnNullable;
		this.ShowColumnNullableChanged?.Invoke(ShowColumnNullable);
	}

	private void ContextAddRelatedTables(object sender, EventArgs e)
	{
		if (SelectedElements.Count == 1)
		{
			Node node = Elements.GetNode(SelectedElements.First((RectangularObjectKey x) => x.Type == RectangularObjectType.Node).Key);
			this.AddRelatedTables?.Invoke(node);
		}
	}

	private void InitializeLinkContextMenu()
	{
		HideHint();
		popupMenu.ItemLinks.Clear();
		if (selectedLink != null)
		{
			popupMenu.AddItem("Edit relationship", Resources.edit_16, ContextEditRelation);
			if (selectedLink.ShowTitle == true)
			{
				popupMenu.AddItem("Hide title", Resources.hide_label_16, ContextShowTitle);
			}
			else
			{
				popupMenu.AddItem("Show title", Resources.show_label_16, ContextShowTitle);
			}
			popupMenu.StartGroupBeforeLastItem();
			if (selectedLink.ShowJoinCondition == true)
			{
				popupMenu.AddItem("Hide join condition", Resources.hide_join_16, ContextShowJoinCondition);
			}
			else
			{
				popupMenu.AddItem("Show join condition", Resources.show_join_16, ContextShowJoinCondition);
			}
			if (selectedLink.Hidden == true)
			{
				popupMenu.AddItem("Show relationship", Resources.show_link_16, ContextShowRelation);
			}
			else
			{
				popupMenu.AddItem("Hide relationship", Resources.hide_link_16, ContextShowRelation);
			}
			BarSubItem barSubItem = popupMenu.AddSubItem("Link style", Resources.colors_16);
			barSubItem.AddItem(LinkStyleEnum.StraightLinkStyleDisplayString, Resources.link_straight, ContextChangeLinkStyle);
			barSubItem.AddItem(LinkStyleEnum.OrthogonalLinkStyleDisplayString, Resources.link_orthogonal, ContextChangeLinkStyle);
			Link link = selectedLink;
			if (link != null && link.LinkStyle == LinkStyleEnum.LinkStyle.Orthogonal)
			{
				barSubItem.ImageOptions.Image = Resources.link_orthogonal;
				(barSubItem.ItemLinks[1].Item as BarButtonItem).ButtonStyle = BarButtonStyle.Check;
				(barSubItem.ItemLinks[1].Item as BarButtonItem).Down = true;
			}
			else
			{
				barSubItem.ImageOptions.Image = Resources.link_straight;
				(barSubItem.ItemLinks[0].Item as BarButtonItem).ButtonStyle = BarButtonStyle.Check;
				(barSubItem.ItemLinks[0].Item as BarButtonItem).Down = true;
			}
			if (selectedLink.UserRelation)
			{
				popupMenu.AddItem("Remove from repository", Resources.relation_delete_16, ContextDelete);
				popupMenu.StartGroupBeforeLastItem();
			}
		}
	}

	private void ContextShowAllColumns(object sender, EventArgs e)
	{
		if (GeneralMessageBoxesHandling.Show("Do you want to show all columns?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, null, 1, FindForm()).DialogResult != DialogResult.Yes)
		{
			return;
		}
		foreach (Node node in Diagram.Elements.Nodes)
		{
			node.ColumnsChanged = true;
			node.ShowColumns(show: true);
			this.ColumnsChanged(node);
		}
	}

	private void ContextShowKeyColumns(object sender, EventArgs e)
	{
		if (GeneralMessageBoxesHandling.Show("Do you want to show key columns only?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, null, 1, FindForm()).DialogResult != DialogResult.Yes)
		{
			return;
		}
		foreach (Node node in Diagram.Elements.Nodes)
		{
			node.ShowKeyColumns();
			node.ColumnsChanged = true;
			this.ColumnsChanged(node);
		}
	}

	private void ContextHideAllColumns(object sender, EventArgs e)
	{
		if (GeneralMessageBoxesHandling.Show("Do you want to hide all columns?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, null, 1, FindForm()).DialogResult != DialogResult.Yes)
		{
			return;
		}
		foreach (Node node in Diagram.Elements.Nodes)
		{
			node.ColumnsChanged = true;
			node.ShowColumns(show: false);
			this.ColumnsChanged(node);
		}
	}

	private void ContextEditRelation(object sender, EventArgs e)
	{
		EditERDLink(selectedLink, selectedLink.FromNode.DatabaseId);
	}

	private void ContextShowTitle(object sender, EventArgs e)
	{
		selectedLink.IsModifiedInRelationForm = false;
		selectedLink.ToggleShowTitle();
	}

	private void ContextShowJoinCondition(object sender, EventArgs e)
	{
		selectedLink.IsModifiedInRelationForm = false;
		selectedLink.ToggleShowJoinCondition();
	}

	private void ContextShowRelation(object sender, EventArgs e)
	{
		selectedLink.IsModifiedInRelationForm = false;
		selectedLink.ToggleShowRelation();
	}

	private void ContextChangeLinkStyle(object sender, ItemClickEventArgs e)
	{
		BarButtonItem barButtonItem = e.Item as BarButtonItem;
		selectedLink.IsModifiedInRelationForm = false;
		selectedLink.LinkStyle = LinkStyleEnum.ObjectToType(barButtonItem.Caption);
	}

	private void ContextDelete(object sender, EventArgs e)
	{
		DeleteSelectedObjects();
	}

	private void InitializeNodeContexMenu()
	{
		popupMenu.ItemLinks.Clear();
		RectangularObject element = Elements.GetElement(SelectedElements.First().Key);
		if (SelectedElements.Count == 1)
		{
			if (element is Node node)
			{
				InitializeOneNodeContextMenu(node);
			}
			else if (element is PostIt postIt)
			{
				InitializeOnePostItContexMenu(postIt);
			}
		}
		else if (SelectedElements.Count > 1)
		{
			if (ElementsContainer.HasCollectionMultipleTypes(SelectedElements))
			{
				InitializeMultiElementContextMenu();
			}
			else if (element is Node)
			{
				InitializeMultiNodeContextMenu();
			}
			else if (element is PostIt)
			{
				InitializeMultiPostItContextMenu();
			}
		}
	}

	private void InitializeOneNodeContextMenu(Node node)
	{
		popupMenu.AddItem("Add relationship", Resources.relation_mx_1x_user_16, ContextAddRelation);
		this.LoadRelatedNodes?.Invoke(Diagram.Elements.Nodes.Where((Node x) => x.Selected).ToList());
		popupMenu.AddItem("Add related objects", Resources.related_tables_16, ContextAddRelatedTables, node.RelatedNodes.Count > 0);
		if (node.IsInSubjectArea)
		{
			popupMenu.AddItem("Remove from Subject Area", Resources.delete_16, ContextRemoveFromSubjectArea);
		}
		else
		{
			popupMenu.AddItem("Add to Subject Area", Resources.module_16, ContextAddToSubjectArea);
		}
		popupMenu.AddItem("Edit display options", Resources.edit_16, ContextEditNode);
		popupMenu.StartGroupBeforeLastItem();
		AddSelectedNodesContextItem();
		AddChangeColorContextItem(node.Color);
		popupMenu.AddItem($"Design {node.Type}", IconsForButtonsFinder.ReturnImageForDesignButtonItem16(node.Type), ContextDesignTable, node.Columns.All((NodeColumnDB x) => !x.ParentId.HasValue));
		popupMenu.StartGroupBeforeLastItem();
		popupMenu.AddItem("Remove from diagram", Resources.delete_16, ContextDelete);
		Image nodeIcon = node.NodeIcon;
		popupMenu.AddItem("Go to " + node.DisplayNameUiEscaped, nodeIcon, GoToNode);
	}

	private void InitializeMultiElementContextMenu()
	{
		AddChangeColorContextItem(Color.Empty);
		popupMenu.AddItem("Remove from diagram", Resources.delete_16, ContextDelete);
		popupMenu.StartGroupBeforeLastItem();
	}

	private void InitializeMultiNodeContextMenu()
	{
		AddSelectedNodesContextItem();
		AddChangeColorContextItem(Color.Empty);
		popupMenu.AddItem("Remove from diagram", Resources.delete_16, ContextDelete);
		popupMenu.StartGroupBeforeLastItem();
	}

	private void InitializeOnePostItContexMenu(PostIt postIt)
	{
		popupMenu.AddItem("Edit text", Resources.edit_16, ContextEditPostIt);
		popupMenu.StartGroupBeforeLastItem();
		popupMenu.AddItem("Bring to front", Resources.bring_to_front_16, ContextBringToFront);
		popupMenu.AddItem("Send to back", Resources.send_to_back_16, ContextSendToBack);
		AddChangeColorContextItem(postIt.Color);
		popupMenu.StartGroupBeforeLastItem();
		popupMenu.AddItem("Remove from diagram", Resources.delete_16, ContextDelete);
	}

	private void InitializeMultiPostItContextMenu()
	{
		popupMenu.AddItem("Bring to front", Resources.bring_to_front_16, ContextBringToFront);
		popupMenu.AddItem("Send to back", Resources.send_to_back_16, ContextSendToBack);
		AddChangeColorContextItem(Color.Empty);
		popupMenu.StartGroupBeforeLastItem();
		popupMenu.AddItem("Remove from diagram", Resources.delete_16, ContextDelete);
	}

	private void ContextDesignTable(object sender, EventArgs e)
	{
		Node node = Diagram.Elements.Nodes.FirstOrDefault((Node x) => x.Selected);
		DesignSelectedObject();
		foreach (Link link2 in node.inLinks)
		{
			int j;
			for (j = 0; j < link2.Columns.Count; j++)
			{
				link2.Columns[j].ColumnFkName = node.Columns.FirstOrDefault((NodeColumnDB x) => x.ColumnId == link2.Columns[j].ColumnFkId)?.Name;
			}
			link2.RefreshUserRelationHint();
		}
		foreach (Link link in node.outLinks)
		{
			int i;
			for (i = 0; i < link.Columns.Count; i++)
			{
				link.Columns[i].ColumnPkName = node.Columns.FirstOrDefault((NodeColumnDB x) => x.ColumnId == link.Columns[i].ColumnPkId)?.Name;
			}
			link.RefreshUserRelationHint();
		}
	}

	private void AddSelectedNodesContextItem()
	{
		BarSubItem barSubItem = popupMenu.AddSubItem("Show columns", Resources.columns_show_all_16);
		barSubItem.AddItem("Show all columns", Resources.columns_show_all_16, ContextShowSelectedNodesColumns);
		barSubItem.AddItem("Show key columns only", Resources.columns_show_key_16, ContextShowSelectedNodesKeyColumns);
		barSubItem.AddItem("Hide all columns", Resources.columns_hide_all_16, ContextHideSelectedNodesColumns);
	}

	private void AddChangeColorContextItem(Color selectedColor)
	{
		BarSubItem barSubItem = popupMenu.AddSubItem("Color", Resources.colors_16);
		barSubItem.AddItem("Default", Resources.color_gray, ContextChangeColor);
		barSubItem.AddItem("Blue", Resources.color_blue, ContextChangeColor);
		barSubItem.StartGroupBeforeLastItem();
		barSubItem.AddItem("Green", Resources.color_green, ContextChangeColor);
		barSubItem.AddItem("Red", Resources.color_red, ContextChangeColor);
		barSubItem.AddItem("Yellow", Resources.color_yellow, ContextChangeColor);
		barSubItem.AddItem("Purple", Resources.color_purple, ContextChangeColor);
		barSubItem.AddItem("Orange", Resources.color_orange, ContextChangeColor);
		barSubItem.AddItem("Cyan", Resources.color_cyan, ContextChangeColor);
		barSubItem.AddItem("Lime", Resources.color_lime, ContextChangeColor);
		if (!(selectedColor != Color.Empty))
		{
			return;
		}
		IEnumerator enumerator = barSubItem.ItemLinks.GetEnumerator();
		while (enumerator.MoveNext())
		{
			BarButtonItem barButtonItem = (enumerator.Current as BarItemLink)?.Item as BarButtonItem;
			if (barButtonItem?.Caption == NodeColors.GetNodeColorString(selectedColor))
			{
				barButtonItem.ButtonStyle = BarButtonStyle.Check;
				barButtonItem.Down = true;
				break;
			}
		}
	}

	private void ContextShowSelectedNodesColumns(object sender, EventArgs e)
	{
		foreach (RectangularObjectKey selectedElement in SelectedElements)
		{
			Node node = Elements.GetNode(selectedElement.Key);
			node.ColumnsChanged = true;
			node.ShowColumns(show: true);
			this.ColumnsChanged(node);
		}
	}

	private void ContextShowSelectedNodesKeyColumns(object sender, EventArgs e)
	{
		foreach (RectangularObjectKey selectedElement in SelectedElements)
		{
			Node node = Elements.GetNode(selectedElement.Key);
			node.ColumnsChanged = true;
			node.ShowColumns(show: false);
			node.ShowKeyColumns();
			this.ColumnsChanged(node);
		}
	}

	private void ContextHideSelectedNodesColumns(object sender, EventArgs e)
	{
		foreach (RectangularObjectKey selectedElement in SelectedElements)
		{
			Node node = Elements.GetNode(selectedElement.Key);
			node.ColumnsChanged = true;
			node.ShowColumns(show: false);
			this.ColumnsChanged(node);
		}
	}

	private void ContextEditNode(object sender, EventArgs e)
	{
		int key = SelectedElements.FirstOrDefault().Key;
		Node node = Elements.GetNode(key);
		new ERDNodeForm(node).ShowDialog(this);
		RefreshNode(node);
	}

	private void ContextChangeColor(object sender, ItemClickEventArgs e)
	{
		BarButtonItem barButtonItem = e.Item as BarButtonItem;
		foreach (RectangularObjectKey selectedElement in SelectedElements)
		{
			Elements.GetElement(selectedElement.Key).Color = NodeColors.GetNodeColor(barButtonItem.Caption);
		}
	}

	private void ContextAddRelation(object sender, EventArgs e)
	{
		isDiagramInAddRelationMode = true;
		addRelationEndNode = Elements.GetNode(SelectedElements.First().Key);
	}

	private void ContextAddToSubjectArea(object sender, EventArgs e)
	{
		RectangularObjectKey rectangularObjectKey = SelectedElements.FirstOrDefault();
		Node node = Elements.GetNode(rectangularObjectKey.Key);
		AddNodeToSubjectArea(node);
	}

	private void ContextRemoveFromSubjectArea(object sender, EventArgs e)
	{
		RectangularObjectKey rectangularObjectKey = SelectedElements.FirstOrDefault();
		Node node = Elements.GetNode(rectangularObjectKey.Key);
		RemoveNodeFromSubjectArea(node);
	}

	private void ContextBringToFront(object sender, EventArgs e)
	{
		foreach (RectangularObjectKey selectedElement in SelectedElements)
		{
			Elements.GetPostIt(selectedElement.Key)?.BringToFront();
		}
	}

	private void ContextSendToBack(object sender, EventArgs e)
	{
		foreach (RectangularObjectKey selectedElement in SelectedElements)
		{
			Elements.GetPostIt(selectedElement.Key)?.SendToBack();
		}
	}

	private void ContextAddNewPostIt(object sender, EventArgs e)
	{
		this.AddPostIt?.Invoke(PointToClient(Cursor.Position));
	}

	private void ContextEditPostIt(object sender, EventArgs e)
	{
		RectangularObjectKey rectangularObjectKey = SelectedElements.FirstOrDefault();
		PostIt postIt = Elements.GetPostIt(rectangularObjectKey.Key);
		EditPostIt(postIt);
	}

	public void EditPostIt(PostIt postIt)
	{
		UnselectAllNodes();
		postItEditor.EditValue = postIt.Text;
		postItEditor.Font = postIt.Font;
		postItEditor.Location = postIt.EditorPosition(base.StartPoint);
		postItEditor.Properties.Appearance.TextOptions.VAlignment = ((postIt.TextPosition == PostItTextPositionEnum.PostItTextPosition.TopLeft) ? VertAlignment.Top : VertAlignment.Center);
		postItEditor.Properties.Appearance.TextOptions.HAlignment = ((postIt.TextPosition == PostItTextPositionEnum.PostItTextPosition.TopLeft) ? HorzAlignment.Near : HorzAlignment.Center);
		postItEditor.Size = postIt.EditorSize;
		postItEditor.Tag = postIt;
		postItEditor.Show();
		postItEditor.Focus();
		postItEditor.BeginInvoke((MethodInvoker)delegate
		{
			postItEditor.SelectionLength = 0;
			postItEditor.SelectionStart = postItEditor.Text.Length;
		});
	}

	private void PostItEditor_Leave(object sender, EventArgs e)
	{
		ClosePostItEditor();
	}

	public void ClosePostItEditor()
	{
		if (postItEditor.Tag is PostIt postIt)
		{
			string text = postItEditor.Text;
			if (!text.Equals(postIt.Text))
			{
				postIt.Text = text;
				postIt.ImportantChange?.Invoke(null, null);
				postIt.FitSizeToText();
			}
		}
		postItEditor.Hide();
	}

	private void GoToNode(object sender, EventArgs e)
	{
		Node node = Elements.GetNode(SelectedElements.First().Key);
		this.GoToNodeClicked?.Invoke(node);
	}

	private void OnMouseDoubleClick(object sender, MouseEventArgs e)
	{
		if (!(bool)popupMenu.Tag && e.Button == MouseButtons.Left)
		{
			Node node = base.MouseOverElement as Node;
			PostIt postIt = base.MouseOverElement as PostIt;
			Link link = base.MouseOverElement as Link;
			if (Control.ModifierKeys == Keys.None && node != null && this.NodeDblClicked != null && !SelectionActive)
			{
				HideHint();
				this.NodeDblClicked(node);
			}
			if (Control.ModifierKeys == Keys.None && postIt != null && this.PostItDblClicked != null && !SelectionActive)
			{
				HideHint();
				this.PostItDblClicked(postIt);
			}
			if (Control.ModifierKeys == Keys.Shift && node != null)
			{
				SelectNodesGroup(node);
			}
			if (Control.ModifierKeys == Keys.None && link != null && this.LinkDblClicked != null && !SelectionActive)
			{
				HideHint();
				this.LinkDblClicked(link);
			}
		}
	}

	private void SelectNodesGroup(Node mouseOverNode)
	{
		UnselectAllNodes();
		Stack<int> stack = new Stack<int>();
		stack.Push(mouseOverNode.Key);
		while (stack.Count > 0)
		{
			int key = stack.Pop();
			Node node = Elements.GetNode(key);
			SelectNode(key);
			foreach (Link outLink in node.outLinks)
			{
				if (!IsNodeSelected(outLink.ToNode.Key, RectangularObjectType.Node))
				{
					stack.Push(outLink.ToNode.Key);
				}
			}
			foreach (Link inLink in node.inLinks)
			{
				if (!IsNodeSelected(inLink.FromNode.Key, RectangularObjectType.Node))
				{
					stack.Push(inLink.FromNode.Key);
				}
			}
		}
	}

	private void OnMouseUp(object sender, MouseEventArgs e)
	{
		this.LoadRelatedNodes?.Invoke(Diagram.Elements.Nodes.Where((Node x) => x.Selected).ToList());
		Box box = this.box;
		StopNodeDrag(new Point(e.X, e.Y));
		if (!(bool)popupMenu.Tag)
		{
			AcceptSelection();
			if (e.Button == MouseButtons.Left && activeNodeResizing)
			{
				activeNodeResizing = false;
				this.DragMouseUp?.Invoke(box, new Point(e.X, e.Y));
			}
		}
	}

	private void OnMouseMove(object sender, MouseEventArgs e)
	{
		if (!(bool)popupMenu.Tag)
		{
			SelectNodesWhileSelectionActive();
			UpdateSelection(e);
			SetCursorOnResizeMode(e);
			ResizeElement();
			Invalidate();
		}
	}

	private void SetCursorOnResizeMode(MouseEventArgs e)
	{
		if (base.MouseOverElementToResize != null)
		{
			Point cursorPos = ToLocal(e.X, e.Y).Subtract(base.StartPoint);
			Cursor = base.MouseOverElementToResize.GetCursor(cursorPos);
		}
		else if (activeNodeResizing)
		{
			Cursor = RectangularObject.GetCursor(resizedEdge);
		}
		else
		{
			Cursor = Cursors.Default;
		}
	}

	private void ResizeElement()
	{
		if (activeNodeResizing)
		{
			box = new Box();
			Point point = PointToClient(Control.MousePosition);
			Point cursorPos = ToLocal(point.X, point.Y).Subtract(base.StartPoint);
			RectangularObject element = Elements.GetElement(resizedNode);
			element?.Resize(cursorPos, Diagram.LastDiagramBox, resizedEdge);
			box.UpdateMinMax(element);
		}
	}

	private void OnMouseLeave(object sender, EventArgs e)
	{
		HideHint();
	}

	private void OnMouseDown(object sender, MouseEventArgs e)
	{
		if ((bool)popupMenu.Tag)
		{
			return;
		}
		RectangularObject rectangularObject = base.MouseOverElement as RectangularObject;
		Link link = base.MouseOverElement as Link;
		if (base.MouseOverElement == null && !Control.ModifierKeys.HasFlag(Keys.Control) && e.Button == MouseButtons.Left && base.MouseOverElementToResize == null)
		{
			if (Control.ModifierKeys != Keys.Shift)
			{
				UnselectAllNodes();
				isDiagramInAddRelationMode = false;
			}
			StartSelection(e);
		}
		else
		{
			DiscardSelection();
		}
		if (e.Button == MouseButtons.Left && !Control.ModifierKeys.HasFlag(Keys.Control) && link != null)
		{
			UnselectAllNodes();
			SelectLink(link.Key);
			this.SelectionChanged?.Invoke(new CanvasObject[1] { link });
		}
		if (e.Button == MouseButtons.Right)
		{
			if (rectangularObject != null)
			{
				if (!IsNodeSelected(rectangularObject.Key, rectangularObject.RectangularObjectType))
				{
					UnselectAllNodes();
					SelectNode(rectangularObject.Key);
				}
				InitializeNodeContexMenu();
				popupMenu.Tag = true;
				popupMenu.ShowPopup(PointToScreen(new Point(e.Location.X, e.Location.Y)));
			}
			else if (link != null)
			{
				UnselectAllNodes();
				SelectLink(link.Key);
				InitializeLinkContextMenu();
				popupMenu.Tag = true;
				popupMenu.ShowPopup(PointToScreen(new Point(e.Location.X, e.Location.Y)));
			}
			else
			{
				UnselectAllNodes();
				InitializeDiagramContexMenu();
				popupMenu.Tag = true;
				popupMenu.ShowPopup(PointToScreen(new Point(e.Location.X, e.Location.Y)));
			}
		}
		if (e.Button == MouseButtons.Left)
		{
			if (base.MouseOverElementToResize != null)
			{
				Point cursorPos = ToLocal(e.X, e.Y).Subtract(base.StartPoint);
				resizedEdge = base.MouseOverElementToResize.GetEdge(cursorPos);
				activeNodeResizing = true;
				resizedNode = base.MouseOverElementToResize.Key;
			}
			else
			{
				resizedNode = int.MinValue;
				resizedEdge = EdgeToResize.None;
			}
		}
	}

	private void OnKeyDown(object sender, KeyEventArgs e)
	{
		if (!(bool)popupMenu.Tag)
		{
			if (e.KeyCode == Keys.A && e.Modifiers == Keys.Control)
			{
				SelectAllNodes();
			}
			if (e.KeyCode == Keys.Escape)
			{
				UnselectAllNodes();
			}
			if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
			{
				SkinsManager.SetResources(SkinsManager.DefaultSkin.ResourceManager);
				ClipboardSupport.SetImage(Diagram.ToImage(Color.White, applyResources: true));
			}
			if (e.KeyCode == Keys.Delete)
			{
				DeleteSelectedObjects();
			}
		}
	}

	public void DeleteSelectedObjects()
	{
		bool flag = false;
		Link link = selectedLink;
		if (link != null && link.UserRelation)
		{
			flag = DeleteSelectedLink();
		}
		else if (SelectedElements.Count > 0)
		{
			flag = DeleteSelectedNodes(deletingByUser: true);
		}
		if (flag)
		{
			this.SelectionChanged?.Invoke(null);
			this.RefreshIgnoredNodes?.Invoke(null, null);
		}
	}

	public void DesignSelectedObject()
	{
		Node node = Diagram.Elements.Nodes.FirstOrDefault((Node x) => x.Selected);
		if (node == null)
		{
			return;
		}
		DesignTableForm designTableForm = new DesignTableForm(node.DatabaseId, node.TableId, node.Schema, node.Label, node.Title, node.TableSource, node.TableType, node.TableSubtype, node.DatabaseName, CustomFieldsSupport);
		if (designTableForm.ShowDialog(FindForm()) == DialogResult.OK)
		{
			node.Schema = designTableForm.TableDesigner.Schema;
			node.Label = designTableForm.TableDesigner.Name;
			node.Title = designTableForm.TableDesigner.Title;
			node.TableSubtype = designTableForm.TableDesigner.Type;
			node.TableSource = designTableForm.TableDesigner.Source;
			node.SetColumnsAfterEdition(designTableForm.TableDesigner.InsertedColumns?.Select((ColumnRow x) => x.Name), FindForm());
			node.RefreshColumnsListString();
			RemoveColumns(designTableForm.TableDesigner.ColumnsToRemove.Select((ColumnRow x) => x.Id).ToList());
		}
	}

	private void RemoveColumns(List<int> removedColumns)
	{
		if (removedColumns == null || removedColumns.Count() == 0)
		{
			return;
		}
		foreach (Link link in Elements.Links)
		{
			foreach (RelationColumnRow item in link.Columns.Where((RelationColumnRow x) => removedColumns.Contains(x.ColumnFkId) || removedColumns.Contains(x.ColumnPkId)).ToList())
			{
				link.Columns.Remove(item);
			}
		}
	}

	public void EditSelectedERDNode()
	{
		new ERDNodeForm(Diagram.Elements.Nodes.FirstOrDefault((Node x) => x.Selected)).ShowDialog();
	}

	public void EditSelectedERDLink()
	{
		EditERDLink(selectedLink, CurrentDatabaseId);
	}

	public bool DeleteSelectedNodes(bool deletingByUser = false)
	{
		int count = SelectedElements.Count;
		if (count == 0)
		{
			return false;
		}
		if (deletingByUser && ((count != 1) ? (!CommonFunctionsDatabase.AskIfDeleting(count, FindForm())) : ((!(Diagram.Elements.GetFirstSelectedObject() is Node node)) ? (!CommonFunctionsDatabase.AskIfDeleting(count, FindForm())) : (!CommonFunctionsDatabase.AskIfDeleting(node, fromSubjectArea: false, null, FindForm())))))
		{
			return false;
		}
		while (SelectedElements.Count > 0)
		{
			RectangularObjectKey rectangularObjectKey = SelectedElements.First();
			if (Elements.GetElement(rectangularObjectKey.Key) is Node node2)
			{
				this.NewNodeDeleted?.Invoke(node2);
			}
			Diagram.RemoveNode(rectangularObjectKey.Key, deletingByUser);
		}
		if (count > 0)
		{
			this.NodeDeleted?.Invoke(null);
		}
		PickingMouseOverElement();
		return true;
	}

	private bool DeleteSelectedLink()
	{
		Link link = selectedLink;
		if ((link != null && !link.UserRelation) || !CommonFunctionsDatabase.AskIfDeleting(selectedLink, FindForm()))
		{
			return false;
		}
		this.LinkDeleted?.Invoke(selectedLink);
		Diagram.RemoveLink(selectedLink);
		selectedLink = null;
		PickingMouseOverElement();
		return true;
	}

	private bool AddNodeToSubjectArea(Node node)
	{
		if (node == null)
		{
			return false;
		}
		node.IsInSubjectArea = true;
		this.NodeAddedToSubjectArea?.Invoke(node);
		return true;
	}

	private bool RemoveNodeFromSubjectArea(Node node)
	{
		if (node != null)
		{
			return this.NodeRemovedFromSubjectArea?.Invoke(node) ?? false;
		}
		return false;
	}

	private void OnNodeContainerChanged(object sender, RectangularObject node, bool isNew, bool byUser = false)
	{
		if (isNew)
		{
			node.MouseDown += OnNodeMouseDown;
			node.MouseUp += OnNodeMouseUp;
			node.MouseMove += OnNodeMouseMove;
			node.MouseClick += OnNodeMouseClick;
		}
		else
		{
			UnselectNode(node.Key);
		}
	}

	private void StartSelection(MouseEventArgs e)
	{
		SelectionActive = true;
		SelectionStartPoint = ToLocal(e.X, e.Y);
		SelectionEndPoint = SelectionStartPoint;
	}

	private void UpdateSelection(MouseEventArgs e)
	{
		if (SelectionActive)
		{
			SelectionEndPoint = ToLocal(e.X, e.Y);
			Invalidate();
		}
	}

	private void AcceptSelection()
	{
		if (SelectionActive)
		{
			UnselectAllNodes();
			SelectionActive = false;
			Rectangle rect = CalcSelectionRect();
			rect.Offset(base.StartPoint.GetReversed());
			SelectNodes(rect);
		}
	}

	private void SelectNodesWhileSelectionActive()
	{
		HashSet<RectangularObjectKey> hashSet = new HashSet<RectangularObjectKey>();
		if (!SelectionActive)
		{
			return;
		}
		Rectangle rect = CalcSelectionRect();
		rect.Offset(base.StartPoint.GetReversed());
		foreach (RectangularObjectKey selectedElement in SelectedElements)
		{
			RectangularObject element = Elements.GetElement(selectedElement.Key);
			element.Selected = rect.IntersectsWith(element.BoxToIntersect);
			if (!element.Selected)
			{
				hashSet.Add(new RectangularObjectKey(element.Key, element.RectangularObjectType));
			}
		}
		SelectedElements.ExceptWith(hashSet);
		SelectNodes(rect);
	}

	private void SelectNodes(Rectangle rect)
	{
		foreach (RectangularObject rectangularObject in Elements.RectangularObjects)
		{
			if (rect.IntersectsWith(rectangularObject.Box))
			{
				SelectNode(rectangularObject.Key);
			}
		}
		Invalidate();
	}

	private void DiscardSelection()
	{
		SelectionActive = false;
		Invalidate();
	}

	private Rectangle CalcSelectionRect()
	{
		Point point = new Point(Math.Min(SelectionStartPoint.X, SelectionEndPoint.X), Math.Min(SelectionStartPoint.Y, SelectionEndPoint.Y));
		Point point2 = new Point(Math.Max(SelectionStartPoint.X, SelectionEndPoint.X), Math.Max(SelectionStartPoint.Y, SelectionEndPoint.Y));
		return new Rectangle(point.X, point.Y, point2.X - point.X, point2.Y - point.Y);
	}

	private void RenderSelection(Graphics g)
	{
		if (SelectionActive)
		{
			Color color = Color.FromArgb(160, SkinsManager.CurrentSkin.ErdSelectedNodeColor);
			Pen pen = new Pen(SkinsManager.CurrentSkin.ErdSelectedNodeBorderColor, 1f);
			SolidBrush solidBrush = new SolidBrush(color);
			Rectangle rect = CalcSelectionRect();
			g.FillRectangle(solidBrush, rect);
			g.DrawRectangle(pen, rect);
			solidBrush.Dispose();
			pen.Dispose();
		}
	}

	private void RenderTooltips(Graphics g)
	{
		if (toolTipController == null || superToolTip == null)
		{
			return;
		}
		if (!(base.MouseOverElement is Link link))
		{
			if (ToolTipVisible)
			{
				HideHint();
			}
		}
		else if (!popupMenu.Visible && !ToolTipVisible)
		{
			Point position = Cursor.Position;
			position.X += 20;
			superToolTip.Items.Clear();
			if (!string.IsNullOrWhiteSpace(link.Title))
			{
				ToolTipTitleItem toolTipTitleItem = new ToolTipTitleItem();
				toolTipTitleItem.Text = link.Title;
				superToolTip.Items.Add(toolTipTitleItem);
				superToolTip.Items.Add(new ToolTipSeparatorItem());
			}
			if (!string.IsNullOrWhiteSpace(link.JoinCondition))
			{
				ToolTipTitleItem toolTipTitleItem2 = new ToolTipTitleItem();
				toolTipTitleItem2.Text = link.JoinCondition;
				superToolTip.Items.Add(toolTipTitleItem2);
			}
			if (!string.IsNullOrWhiteSpace(link.Description))
			{
				ToolTipItem toolTipItem = new ToolTipItem();
				toolTipItem.Text = link.Description;
				superToolTip.Items.Add(toolTipItem);
			}
			ToolTipControllerShowEventArgs toolTipControllerShowEventArgs = new ToolTipControllerShowEventArgs();
			toolTipControllerShowEventArgs.ToolTipType = ToolTipType.SuperTip;
			toolTipControllerShowEventArgs.SuperTip = superToolTip;
			toolTipController.ShowHint(toolTipControllerShowEventArgs, position);
			ToolTipVisible = true;
		}
	}

	private void HideHint()
	{
		toolTipController.HideHint();
		ToolTipVisible = false;
	}

	private void StartNodeDrag(RectangularObject clickedOn, CMouseEventArgs e)
	{
		activeNodeDrag = true;
		StartNodeDragPoint = new Point(e.X, e.Y);
		ClickedOnNodeDrag = clickedOn.Key;
		MouseMoveWhileNodeDrag = false;
		StartNodeDragPosition = new Dictionary<int, NodePositions>();
		foreach (RectangularObject item in Elements.RectangularObjects.Where((RectangularObject x) => x.Selected))
		{
			StartNodeDragPosition.Add(item.Key, new NodePositions(item.Position, item.Center));
		}
		Diagram.CalcBBox();
	}

	private void MoveNodeDrag(CMouseEventArgs e)
	{
		if (!activeNodeDrag)
		{
			return;
		}
		box = new Box();
		NodePositions nodePositions = StartNodeDragPosition[ClickedOnNodeDrag];
		Point point = new Point(nodePositions.Position.X + base.StartPoint.X + e.X - StartNodeDragPoint.X - base.GridStartPoint.X, nodePositions.Position.Y + base.StartPoint.Y + e.Y - StartNodeDragPoint.Y - base.GridStartPoint.Y);
		Point point2 = new Point(nodePositions.Center.X + base.StartPoint.X + e.X - StartNodeDragPoint.X - base.GridStartPoint.X, nodePositions.Center.Y + base.StartPoint.Y + e.Y - StartNodeDragPoint.Y - base.GridStartPoint.Y);
		int num = 0;
		int num2 = 0;
		int num3 = (int)Math.Round((float)point.X / 50f);
		int num4 = (int)Math.Round((float)point2.X / 50f);
		if (Math.Abs(point.X - num3 * 50) <= 10)
		{
			num = num3 * 50 - point.X;
		}
		else if (Math.Abs(point2.X - num4 * 50) <= 10)
		{
			num = num4 * 50 - point2.X;
		}
		int num5 = (int)Math.Round((float)point.Y / 50f);
		int num6 = (int)Math.Round((float)point2.Y / 50f);
		if (Math.Abs(point.Y - num5 * 50) <= 10)
		{
			num2 = num5 * 50 - point.Y;
		}
		else if (Math.Abs(point2.Y - num6 * 50) <= 10)
		{
			num2 = num6 * 50 - point2.Y;
		}
		foreach (RectangularObjectKey selectedElement in SelectedElements)
		{
			int key = selectedElement.Key;
			RectangularObject element = Elements.GetElement(key);
			element.UpdatePosition(new Point(StartNodeDragPosition[key].Position.X + e.X - StartNodeDragPoint.X + num, StartNodeDragPosition[key].Position.Y + e.Y - StartNodeDragPoint.Y + num2), Diagram.LastDiagramBox);
			box.UpdateMinMax(element);
		}
		MouseMoveWhileNodeDrag = true;
	}

	private void StopNodeDrag(Point point)
	{
		if (activeNodeDrag && box != null)
		{
			this.DragMouseUp?.Invoke(box, StartNodeDragPoint.Subtract(point));
		}
		activeNodeDrag = false;
		MouseMoveWhileNodeDrag = false;
		box = null;
	}

	private void OnNodeMouseMove(object sender, CMouseEventArgs e)
	{
		if (!(bool)popupMenu.Tag)
		{
			MoveNodeDrag(e);
		}
	}

	private void OnNodeMouseUp(object sender, CMouseEventArgs e)
	{
		if (!(bool)popupMenu.Tag)
		{
			RectangularObject rectangularObject = sender as RectangularObject;
			if (e.Button == MouseButtons.Left && e.Ctrl && selectedLink == null)
			{
				ToggleNodeSelection(rectangularObject.Key, rectangularObject.RectangularObjectType);
			}
			StopNodeDrag(new Point(e.X, e.Y));
		}
	}

	private void OnNodeMouseDown(object sender, CMouseEventArgs e)
	{
		if ((bool)popupMenu.Tag)
		{
			return;
		}
		RectangularObject rectangularObject = sender as RectangularObject;
		if (e.Button == MouseButtons.Left && !rectangularObject.Selected && !e.Ctrl)
		{
			UnselectAllNodes();
			SelectNode(rectangularObject.Key);
		}
		if (e.Button == MouseButtons.Left && isDiagramInAddRelationMode && !e.Ctrl && rectangularObject is Node node)
		{
			isDiagramInAddRelationMode = false;
			Link link = Diagram.CreateLink(node, addRelationEndNode, Diagram.NextLinkRelationId, isUserRelation: true);
			link.SubjecrAreaId = rectangularObject.SubjectAreaId;
			link.LinkStyle = LinkStyle;
			if (new RelationForm(link, node.DatabaseId, ContextShowSchema, editMode: false, Elements.Nodes).ShowDialog(this, setCustomMessageDefaultOwner: true) != DialogResult.OK)
			{
				Diagram.RemoveLink(link);
				return;
			}
			Elements.AddLink(link);
			link.RefreshUserRelationHint();
			if (this.LinkAdded != null && !SelectionActive)
			{
				this.LinkAdded(link);
			}
			if (link.FromNode == null)
			{
				Diagram.RemoveLink(link);
			}
		}
		else if (e.Button == MouseButtons.Left && !e.Ctrl)
		{
			StartNodeDrag(rectangularObject, e);
		}
		else
		{
			StopNodeDrag(new Point(e.X, e.Y));
		}
	}

	private void OnNodeMouseClick(object sender, CMouseEventArgs e)
	{
		if (!(bool)popupMenu.Tag)
		{
			RectangularObject rectangularObject = sender as RectangularObject;
			if (e.Button == MouseButtons.Left && !MouseMoveWhileNodeDrag && !e.Ctrl)
			{
				UnselectAllNodes();
				SelectNode(rectangularObject.Key);
			}
		}
	}

	public void SelectNode(int key)
	{
		RectangularObject element = Elements.GetElement(key);
		if (element.Selected)
		{
			return;
		}
		selectedLink = null;
		element.Selected = true;
		SelectedElements.Add(new RectangularObjectKey(key, element.RectangularObjectType));
		SelectionChangedHandler selectionChanged = this.SelectionChanged;
		if (selectionChanged != null)
		{
			CanvasObject[] objects = SelectedElements.Select((RectangularObjectKey x) => Elements.GetElement(x.Key)).ToArray();
			selectionChanged(objects);
		}
	}

	public bool IsNodeSelected(int key, RectangularObjectType type)
	{
		return SelectedElements.Contains(new RectangularObjectKey(key, type));
	}

	public void UnselectNode(int key)
	{
		RectangularObject element = Elements.GetElement(key);
		if (!element.Selected)
		{
			return;
		}
		element.Selected = false;
		SelectedElements.Remove(new RectangularObjectKey(key, element.RectangularObjectType));
		SelectionChangedHandler selectionChanged = this.SelectionChanged;
		if (selectionChanged != null)
		{
			CanvasObject[] objects = SelectedElements.Select((RectangularObjectKey x) => Elements.GetElement(x.Key)).ToArray();
			selectionChanged(objects);
		}
	}

	public void ToggleNodeSelection(int key, RectangularObjectType type)
	{
		if (IsNodeSelected(key, type))
		{
			UnselectNode(key);
		}
		else
		{
			SelectNode(key);
		}
	}

	public void SelectAllNodes()
	{
		foreach (Node node in Elements.Nodes)
		{
			SelectNode(node.Key);
		}
	}

	public void UnselectAllNodes()
	{
		foreach (RectangularObjectKey selectedElement in SelectedElements)
		{
			Elements.GetElement(selectedElement.Key).Selected = false;
		}
		foreach (Link link in Elements.Links)
		{
			UnselectLink(link.Key);
		}
		SelectedElements.Clear();
		SelectionChangedHandler selectionChanged = this.SelectionChanged;
		if (selectionChanged != null)
		{
			CanvasObject[] objects = SelectedElements.Select((RectangularObjectKey x) => Elements.GetElement(x.Key)).ToArray();
			selectionChanged(objects);
		}
	}

	public void SelectLink(int key)
	{
		Link link = Elements.GetLink(key);
		if (!link.Selected)
		{
			link.Selected = true;
			selectedLink = link;
			SelectionChangedHandler selectionChanged = this.SelectionChanged;
			if (selectionChanged != null)
			{
				CanvasObject[] objects = new Link[1] { link };
				selectionChanged(objects);
			}
		}
	}

	public void UnselectLink(int key)
	{
		Link link = Elements.GetLink(key);
		if (link.Selected)
		{
			link.Selected = false;
			SelectionChangedHandler selectionChanged = this.SelectionChanged;
			if (selectionChanged != null)
			{
				CanvasObject[] objects = new Link[1] { link };
				selectionChanged(objects);
			}
		}
	}

	public void ToggleLinkSelection(int key)
	{
		if (Elements.GetLink(key).Selected)
		{
			UnselectLink(key);
		}
		else
		{
			SelectLink(key);
		}
	}

	private void OnDiagramChanged(object sender, EventArgs e)
	{
		Invalidate();
	}

	public override Point ToLocal(int x, int y)
	{
		return new Point(x, y);
	}

	public override void Render(Graphics g, Point startPoint)
	{
		int num = Math.Max(base.Width / 50, base.Height / 50) + 3;
		using (Pen pen = new Pen(SkinsManager.CurrentSkin.ErdERDGridColor, 1f))
		{
			for (int i = 0; i < num; i++)
			{
				g.DrawLine(pen, new Point(50 * i + base.GridStartPoint.X, base.GridStartPoint.Y), new Point(50 * i + base.GridStartPoint.X, 50 * num + base.GridStartPoint.Y));
				g.DrawLine(pen, new Point(base.GridStartPoint.X, 50 * i + base.GridStartPoint.Y), new Point(50 * num + base.GridStartPoint.X, 50 * i + base.GridStartPoint.Y));
			}
		}
		DrawEmptyERDMessage(g);
		Diagram.Render(g, startPoint, CanvasObject.Output.Control, RenderAddNewRelation);
		RenderSelection(g);
		RenderTooltips(g);
	}

	private void DrawEmptyERDMessage(Graphics g)
	{
		if (!Diagram.HasAnyNodes)
		{
			string s = "Add entity by drag & drop or double click elements" + Environment.NewLine + "from the list on the right";
			Font font = new Font("Tahoma", 12f, FontStyle.Regular);
			SizeF sizeF = g.MeasureString(s, font);
			g.DrawString(s, font, Brushes.Gray, (float)(base.ClientSize.Width / 2) - sizeF.Width / 2f, (float)(base.ClientSize.Height / 2) - sizeF.Height / 2f);
		}
	}

	public void RefreshControlSize(bool calculateDiagramBox, Point startPoint)
	{
		if (calculateDiagramBox)
		{
			Diagram.CalcBBox();
		}
		Rectangle lastDiagramBox = Diagram.LastDiagramBox;
		Size clientSize = base.Parent.ClientSize;
		int num = lastDiagramBox.Width + lastDiagramBox.X + startPoint.X + 25;
		int num2 = lastDiagramBox.Height + lastDiagramBox.Y + startPoint.Y + 25;
		base.Width = Math.Min(MaxControlSize, (clientSize.Width > num) ? clientSize.Width : num);
		base.Height = Math.Min(MaxControlSize, (clientSize.Height > num2) ? clientSize.Height : num2);
	}

	public static int GetDiagramGridReminder(int value, bool toAbsoluteFloor = true)
	{
		if (toAbsoluteFloor)
		{
			return value % 50;
		}
		return -(50 - value % 50);
	}

	public static int GetEntireGridCellsSize(int baseValue, bool toAbsoluteFloor)
	{
		int value = Math.Abs(baseValue);
		if (toAbsoluteFloor)
		{
			return baseValue - GetDiagramGridReminder(baseValue, toAbsoluteFloor);
		}
		if (baseValue >= 0)
		{
			return baseValue - GetDiagramGridReminder(value, toAbsoluteFloor);
		}
		return baseValue + GetDiagramGridReminder(value, toAbsoluteFloor);
	}

	private void RenderAddNewRelation(Graphics g)
	{
		if (isDiagramInAddRelationMode)
		{
			_ = addRelationEndNode.Center;
			g.SmoothingMode = SmoothingMode.AntiAlias;
			Point point = PointToClient(Cursor.Position);
			Pen pen = Styles.LinkBorderPen(selected: false, userRelation: true, false, CanvasObject.Output.Control);
			g.DrawLine(pen, ToLocal(point.X, point.Y), addRelationEndNode.Center.Add(base.StartPoint));
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
	}
}
