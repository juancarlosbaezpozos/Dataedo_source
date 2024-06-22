using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.UserControls;

public class DesignTableUserControl : BaseUserControl
{
	private class ColumnType
	{
		public SharedObjectSubtypeEnum.ObjectSubtype ObjectType { get; set; }

		public UserTypeEnum.UserType Source { get; set; }

		public string DisplayName => SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Column, ObjectType);

		public Bitmap Icon => IconsSupport.GetObjectIcon(SharedObjectTypeEnum.ObjectType.Column, ObjectType, Source);

		public ColumnType(SharedObjectSubtypeEnum.ObjectSubtype objectType, UserTypeEnum.UserType source = UserTypeEnum.UserType.USER)
		{
			ObjectType = objectType;
			Source = source;
		}
	}

	private TableDesigner tableDesigner;

	private CustomFieldsCellsTypesSupport customFieldsCellsTypesSupport;

	private List<ColumnType> columnTypes;

	private Action setRemoveButtonAvailability;

	private IContainer components;

	private ToolTipController toolTipController;

	private TreeList designerTreeList;

	private TreeListColumn nameTreeListColumn;

	private TreeListColumn dataTypeTreeListColumn;

	private TreeListColumn sizeTreeListColumn;

	private TreeListColumn nullableTreeListColumn;

	private TreeListColumn titleTreeListColumn;

	private TreeListColumn descriptionTreeListColumn;

	private TreeListColumn typeTreeListColumn;

	private RepositoryItemImageComboBox typeNormalTreeListRepositoryItemImageComboBox;

	private RepositoryItemImageComboBox typeUserDefinedTreeListRepositoryItemImageComboBox;

	private TreeListColumn ordinalPositonTreeListColumn;

	private TreeListColumn defaultValueTreeListColumn;

	private TreeListColumn computedFormulaTreeListColumn;

	private TreeListColumn identityTreeListColumn;

	private RepositoryItemTextEdit nameRepositoryItemTextEdit;

	private RepositoryItemTextEdit dataTypeRepositoryItemTextEdit;

	private RepositoryItemTextEdit sizeRepositoryItemTextEdit;

	private RepositoryItemTextEdit titleRepositoryItemTextEdit;

	private RepositoryItemCheckEdit nullableRepositoryItemCheckEdit;

	public ColumnRow FocusedRow => designerTreeList.GetFocusedRow() as ColumnRow;

	public event Action SetRemoveButtonAvailability
	{
		add
		{
			if (setRemoveButtonAvailability == null || !setRemoveButtonAvailability.GetInvocationList().Contains(value))
			{
				setRemoveButtonAvailability = (Action)Delegate.Combine(setRemoveButtonAvailability, value);
			}
		}
		remove
		{
			setRemoveButtonAvailability = (Action)Delegate.Remove(setRemoveButtonAvailability, value);
		}
	}

	public DesignTableUserControl()
	{
		InitializeComponent();
		customFieldsCellsTypesSupport = new CustomFieldsCellsTypesSupport(isForSummaryTable: false);
		LengthValidation.SetTitleOrNameLengthLimit(titleRepositoryItemTextEdit);
		LengthValidation.SetTitleOrNameLengthLimit(nameRepositoryItemTextEdit);
		LengthValidation.SetDataTypeLength(dataTypeRepositoryItemTextEdit);
		LengthValidation.SetDataSize(sizeRepositoryItemTextEdit);
		ImageCollection normalIconsImageCollection = new ImageCollection();
		ImageCollection userDefinedIconsImageCollection = new ImageCollection();
		ImageComboBoxItem[] items = SharedObjectSubtypeEnum.ColumnSubtypes.OrderBy((SharedObjectSubtypeEnum.ObjectSubtype x) => SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Column, x)).Select(delegate(SharedObjectSubtypeEnum.ObjectSubtype x)
		{
			normalIconsImageCollection.AddImage(IconsSupport.GetObjectIcon(SharedObjectTypeEnum.ObjectType.Column, x, UserTypeEnum.UserType.DBMS));
			userDefinedIconsImageCollection.AddImage(IconsSupport.GetObjectIcon(SharedObjectTypeEnum.ObjectType.Column, x, UserTypeEnum.UserType.USER));
			return new ImageComboBoxItem(SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Column, x), SharedObjectSubtypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Column, x), normalIconsImageCollection.Images.Count - 1);
		}).ToArray();
		typeNormalTreeListRepositoryItemImageComboBox.SmallImages = normalIconsImageCollection;
		typeNormalTreeListRepositoryItemImageComboBox.Items.AddRange(items);
		typeUserDefinedTreeListRepositoryItemImageComboBox.SmallImages = userDefinedIconsImageCollection;
		typeUserDefinedTreeListRepositoryItemImageComboBox.Items.AddRange(items);
		ImageCollection allIconsImageCollection = new ImageCollection();
		columnTypes = new List<ColumnType>(SharedObjectSubtypeEnum.ColumnSubtypes.Count() * 2);
		columnTypes.AddRange(from x in SharedObjectSubtypeEnum.ColumnSubtypes
			orderby SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Column, x)
			select new ColumnType(x, UserTypeEnum.UserType.DBMS));
		columnTypes.AddRange(from x in SharedObjectSubtypeEnum.ColumnSubtypes
			orderby SharedObjectSubtypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.ObjectType.Column, x)
			select new ColumnType(x));
		columnTypes.ForEach(delegate(ColumnType x)
		{
			allIconsImageCollection.AddImage(x.Icon);
		});
		designerTreeList.StateImageList = allIconsImageCollection;
	}

	public void Init(TableDesigner tableDesigner)
	{
		this.tableDesigner = tableDesigner;
		if (tableDesigner.ObjectTypeValue == SharedObjectTypeEnum.ObjectType.Structure)
		{
			TreeListColumn treeListColumn = ordinalPositonTreeListColumn;
			bool visible = (ordinalPositonTreeListColumn.OptionsColumn.ShowInCustomizationForm = false);
			treeListColumn.Visible = visible;
			TreeListColumn treeListColumn2 = defaultValueTreeListColumn;
			TreeListOptionsColumn optionsColumn = defaultValueTreeListColumn.OptionsColumn;
			TreeListColumn treeListColumn3 = computedFormulaTreeListColumn;
			TreeListOptionsColumn optionsColumn2 = computedFormulaTreeListColumn.OptionsColumn;
			TreeListColumn treeListColumn4 = identityTreeListColumn;
			bool flag3 = (identityTreeListColumn.OptionsColumn.ShowInCustomizationForm = false);
			bool flag5 = (treeListColumn4.Visible = flag3);
			bool flag7 = (optionsColumn2.ShowInCustomizationForm = flag5);
			bool flag9 = (treeListColumn3.Visible = flag7);
			visible = (optionsColumn.ShowInCustomizationForm = flag9);
			treeListColumn2.Visible = visible;
		}
		customFieldsCellsTypesSupport.SetCustomColumns(designerTreeList, this.tableDesigner.CustomFieldsSupport.GetVisibleFields(this.tableDesigner.ObjectTypeValue));
		RefreshDataSource();
		SetNodeLevelPositions(checkIfChanged: false);
		if (((from x in tableDesigner.DataSourceColumns
			orderby x.Level, x.Sort
			select x).FirstOrDefault()?.UniqueId).HasValue)
		{
			designerTreeList.TopVisibleNodeIndex = 0;
		}
	}

	public void RefreshDataSource(bool setColumnsWidth = true)
	{
		designerTreeList.BeginUpdate();
		designerTreeList.BeginUnboundLoad();
		designerTreeList.DataSource = tableDesigner.DataSourceColumns;
		designerTreeList.EndUnboundLoad();
		designerTreeList.EndUpdate();
		designerTreeList.RefreshDataSource();
		if (setColumnsWidth)
		{
			SetColumnsWidth();
		}
	}

	private void SetColumnsWidth()
	{
		foreach (TreeListColumn column in designerTreeList.Columns)
		{
			if (!column.FieldName.Equals("iconGridColumn") && !column.FieldName.Equals("IsIdentity") && !column.FieldName.Equals("Nullable"))
			{
				column.BestFit();
				if (column.Width > 400)
				{
					column.Width = 400;
				}
			}
		}
		designerTreeList.ExpandAll();
		designerTreeList.BestFitColumns();
	}

	public void AddColumn()
	{
		TreeListNode treeListNode = null;
		IEnumerable<IGrouping<TreeListNode, TreeListNode>> source = from x in designerTreeList.Selection
			group x by x.ParentNode;
		if (source.Count() == 1)
		{
			treeListNode = source.First().First().ParentNode;
		}
		CloseEditor();
		ColumnRow columnRow = tableDesigner.AddColumnAndGetColumnRow((treeListNode == null) ? null : (designerTreeList.GetDataRecordByNode(treeListNode) as ColumnRow));
		columnRow.NodeLevelPosition = ((treeListNode == null) ? (designerTreeList.AllNodesCount + 1) : (treeListNode?.Nodes?.Count ?? 1));
		RefreshDataSource();
		TreeListNode[] array = designerTreeList.Selection.ToArray();
		foreach (TreeListNode node in array)
		{
			designerTreeList.UnselectNode(node);
		}
		TreeListNode treeListNode2 = designerTreeList.FindNodeByKeyID(columnRow.UniqueId);
		if (treeListNode2.PrevVisibleNode != null)
		{
			ColumnRow columnRow2 = designerTreeList.GetDataRecordByNode(treeListNode2.PrevVisibleNode) as ColumnRow;
			columnRow.Type = columnRow2.Type;
		}
		designerTreeList.SelectCell(treeListNode2, designerTreeList.VisibleColumns[0]);
		designerTreeList.FocusedColumn = designerTreeList.VisibleColumns[0];
		designerTreeList.FocusedNode = treeListNode2;
		designerTreeList.ShowEditor();
	}

	public void RemoveColumn()
	{
		Remove();
		RefreshDataSource();
		designerTreeList.RefreshDataSource();
	}

	public bool IsNotValid()
	{
		designerTreeList.ClearColumnErrors();
		designerTreeList.RefreshDataSource();
		return tableDesigner.HasDuplicatesOrEmptyNames();
	}

	public bool MoveUp()
	{
		return Move(moveUp: true);
	}

	public bool MoveDown()
	{
		return Move(moveUp: false);
	}

	public new bool Move(bool moveUp)
	{
		HashSet<TreeListNode> hashSet = new HashSet<TreeListNode>();
		try
		{
			designerTreeList.BeginUpdate();
			CloseEditor();
			TreeListNode[] array = designerTreeList.Selection.OrderBy(delegate(TreeListNode x)
			{
				ColumnRow item = designerTreeList.GetDataRecordByNode(x) as ColumnRow;
				return tableDesigner.DataSourceColumns.IndexOf(item);
			}).ToArray();
			ColumnRow[] array2 = array.Select((TreeListNode x) => designerTreeList.GetDataRecordByNode(x) as ColumnRow).ToArray();
			for (int i = 0; i < array2.Length; i++)
			{
				designerTreeList.UnselectNode(array[i]);
				if (MoveColumns(array2[i], moveUp, array2))
				{
					hashSet.Add(array[i].ParentNode);
				}
			}
			designerTreeList.RefreshDataSource();
			designerTreeList.ClearSelection();
			designerTreeList.Selection.SelectCells(array2.Select((ColumnRow x) => designerTreeList.FindNodeByKeyID(x.UniqueId)), nameTreeListColumn);
			if (hashSet.Count > 0)
			{
				foreach (TreeListNode item2 in hashSet)
				{
					SetNodesPositions(item2?.Nodes ?? designerTreeList.Nodes);
				}
			}
		}
		finally
		{
			designerTreeList.EndUpdate();
		}
		return hashSet.Count > 0;
	}

	public void MoveToBottom()
	{
		try
		{
			designerTreeList.BeginUpdate();
			while (Move(moveUp: false))
			{
			}
		}
		finally
		{
			designerTreeList.EndUpdate();
		}
	}

	public void MoveToTop()
	{
		try
		{
			designerTreeList.BeginUpdate();
			while (Move(moveUp: true))
			{
			}
		}
		finally
		{
			designerTreeList.EndUpdate();
		}
	}

	public void UpdateCustomFields()
	{
		foreach (CustomFieldRowExtended field in tableDesigner.CustomFieldsSupport.GetVisibleFields(tableDesigner.ObjectTypeValue))
		{
			CustomFieldsRepositoryItems.RefreshEditOpenValues(designerTreeList.Columns.FirstOrDefault((TreeListColumn x) => x.FieldName.Equals(field.FieldName)).ColumnEdit, field);
		}
	}

	private bool MoveColumns(ColumnRow node, bool moveUp, IEnumerable<ColumnRow> selectedNodes)
	{
		int num = ((!moveUp) ? 1 : (-1));
		ColumnRow currentColumn = node;
		int num2 = tableDesigner.DataSourceColumns.IndexOf(currentColumn);
		ColumnRow columnRow = null;
		if (num < 0)
		{
			columnRow = tableDesigner.DataSourceColumns.Take(num2).LastOrDefault((ColumnRow x) => x.UniqueParentId == currentColumn.UniqueParentId && x.Level == currentColumn.Level);
		}
		else if (num > 0)
		{
			columnRow = tableDesigner.DataSourceColumns.Skip(num2 + 1).FirstOrDefault((ColumnRow x) => x.UniqueParentId == currentColumn.UniqueParentId && x.Level == currentColumn.Level);
		}
		if (selectedNodes.Contains(columnRow))
		{
			columnRow = null;
		}
		tableDesigner.DataSourceColumns.IndexOf(columnRow);
		if (currentColumn == null || columnRow == null)
		{
			return false;
		}
		tableDesigner.SwapColumns(currentColumn, columnRow);
		designerTreeList.FocusedNode = null;
		RefreshDataSource();
		designerTreeList.FocusedNode = designerTreeList.FindNodeByKeyID(currentColumn.UniqueId);
		tableDesigner.IsChanged = true;
		ColumnRow columnRow2 = currentColumn;
		bool isSortChanged = (columnRow.IsSortChanged = true);
		columnRow2.IsSortChanged = isSortChanged;
		return true;
	}

	private void designerTreeList_DragDrop(object sender, DragEventArgs e)
	{
		TreeList obj = sender as TreeList;
		DXDragEventArgs dXDragEventArgs = obj.GetDXDragEventArgs(e);
		TreeListNode node = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
		TreeListNode node2 = obj.CalcHitInfo(obj.PointToClient(new Point(e.X, e.Y))).Node;
		DragDrop(node, node2, dXDragEventArgs.DragInsertPosition);
	}

	private new void DragDrop(TreeListNode node, TreeListNode destinationNode, DragInsertPosition dragInsertPosition)
	{
		ColumnRow columnRow = designerTreeList.GetDataRecordByNode(node) as ColumnRow;
		ColumnRow columnRow2 = designerTreeList.GetDataRecordByNode(destinationNode) as ColumnRow;
		switch (dragInsertPosition)
		{
		case DragInsertPosition.AsChild:
			columnRow.ParentColumn = columnRow2;
			columnRow.ParentId = columnRow2?.Id;
			columnRow.Level = (columnRow2?.Level ?? 0) + 1;
			columnRow.Path = columnRow2?.FullName;
			columnRow.IsSortChanged = true;
			if (columnRow.RowState != ManagingRowsEnum.ManagingRows.ForAdding)
			{
				columnRow.SetModified();
			}
			tableDesigner.UpdatePaths(columnRow);
			break;
		case DragInsertPosition.Before:
		case DragInsertPosition.After:
			columnRow.ParentColumn = columnRow2?.ParentColumn;
			columnRow.ParentId = columnRow2?.ParentId;
			columnRow.Level = columnRow2?.Level ?? 1;
			columnRow.Path = columnRow2?.Path;
			columnRow.IsSortChanged = true;
			if (columnRow.RowState != ManagingRowsEnum.ManagingRows.ForAdding)
			{
				columnRow.SetModified();
			}
			break;
		}
	}

	private void designerTreeList_AfterDropNode(object sender, AfterDropNodeEventArgs e)
	{
		SetRecordsPositions(e.Node, e.DestinationNode);
	}

	private void SetRecordsPositions(TreeListNode node, TreeListNode destinationNode)
	{
		SetLevelNodesPositions(node);
		SetLevelNodesPositions(destinationNode);
	}

	private void SetLevelNodesPositions(TreeListNode node)
	{
		if (node != null)
		{
			TreeListNodes nodesPositions = node.ParentNode?.Nodes ?? node.TreeList.Nodes;
			SetNodesPositions(nodesPositions);
		}
	}

	private void SetNodesPositions(IEnumerable<TreeListNode> nodes)
	{
		for (int i = 0; i < nodes?.Count(); i++)
		{
			if (designerTreeList.GetDataRecordByNode(nodes.ElementAt(i)) is ColumnRow columnRow && columnRow.NodeLevelPosition != i)
			{
				columnRow.NodeLevelPosition = i;
				columnRow.IsSortChanged = true;
			}
		}
	}

	public void CloseEditor()
	{
		designerTreeList.CloseEditor();
	}

	public void ResetSort()
	{
		tableDesigner.ResetSort();
		RefreshDataSource();
		SetNodeLevelPositions(checkIfChanged: true);
	}

	public void SetNodeLevelPositions(bool checkIfChanged)
	{
		tableDesigner.DataSourceColumns.ForEach(delegate(ColumnRow x)
		{
			int nodeIndex = designerTreeList.GetNodeIndex(designerTreeList.FindNodeByKeyID(x.UniqueId));
			if (checkIfChanged && x.NodeLevelPosition != nodeIndex)
			{
				x.IsSortChanged = true;
			}
			x.NodeLevelPosition = nodeIndex;
		});
	}

	private void MoveOutsideSelection(TreeListNode node, List<TreeListNode> selection)
	{
		if (selection.Contains(node))
		{
			TreeListNode treeListNode = node.ParentNode ?? node.PrevNode;
			while (selection.Contains(treeListNode))
			{
				treeListNode = treeListNode.ParentNode ?? treeListNode.PrevNode;
			}
			DragDrop(node, treeListNode, DragInsertPosition.After);
			selection.Remove(node);
		}
	}

	private void Remove()
	{
		List<TreeListNode> list = designerTreeList.Selection.Select((TreeListNode x) => x).ToList();
		List<TreeListNode> nodesToBeSelected = list.ToList();
		designerTreeList.ClearSelection();
		list.ForEach(delegate(TreeListNode x)
		{
			SetChildrenToBeSelected(x, nodesToBeSelected);
		});
		nodesToBeSelected = (from x in nodesToBeSelected
			orderby (designerTreeList.GetDataRecordByNode(x) as ColumnRow).Level
			orderby (designerTreeList.GetDataRecordByNode(x) as ColumnRow).NodeLevelPosition
			select x).ToList();
		nodesToBeSelected.Where((TreeListNode x) => (designerTreeList.GetDataRecordByNode(x) as ColumnRow).Source == UserTypeEnum.UserType.DBMS).ToList().ForEach(delegate(TreeListNode x)
		{
			MoveOutsideSelection(x, nodesToBeSelected);
		});
		designerTreeList.Selection.SelectCells(nodesToBeSelected, nameTreeListColumn);
		ColumnRow[] array = (from x in nodesToBeSelected
			select designerTreeList.GetRow(x.Id) as ColumnRow into x
			where x != null && x.Source == UserTypeEnum.UserType.USER
			select x).ToArray();
		IEnumerable<int?> enumerable = from x in array
			select x.UniqueParentId into x
			group x by x into x
			select x.Key;
		tableDesigner.Remove(array);
		designerTreeList.RefreshDataSource();
		foreach (int? item in enumerable)
		{
			SetNodesPositions(((!item.HasValue) ? null : designerTreeList.FindNodeByKeyID(item))?.Nodes ?? designerTreeList.Nodes);
		}
		designerTreeList.ClearSelection();
	}

	private void SetChildrenToBeSelected(TreeListNode node, List<TreeListNode> nodesToBeSelected)
	{
		IEnumerator enumerator = node.Nodes.GetEnumerator();
		while (enumerator.MoveNext())
		{
			TreeListNode treeListNode = (TreeListNode)enumerator.Current;
			nodesToBeSelected.Add(treeListNode);
			if (treeListNode.HasChildren)
			{
				SetChildrenToBeSelected(treeListNode, nodesToBeSelected);
			}
		}
	}

	private void designerTreeList_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		ColumnRow columnAsModified = designerTreeList.GetFocusedRow() as ColumnRow;
		tableDesigner.SetColumnAsModified(columnAsModified);
	}

	private void designerTreeList_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
	{
		ColumnRow row = designerTreeList.GetRow(e.Node.Id) as ColumnRow;
		if (tableDesigner.ObjectTypeValue != SharedObjectTypeEnum.ObjectType.Structure && ColumnsHelper.IsCellBlocked(row, e.Column.FieldName) && !designerTreeList.GetSelectedCells(e.Node).Any((TreeListColumn x) => x == e.Column))
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridDisabledGridRowForeColor;
		}
	}

	private void designerTreeList_ShowingEditor(object sender, CancelEventArgs e)
	{
		ColumnRow row = designerTreeList.GetFocusedRow() as ColumnRow;
		if (tableDesigner.ObjectTypeValue != SharedObjectTypeEnum.ObjectType.Structure && ColumnsHelper.IsCellBlocked(row, designerTreeList.FocusedColumn.FieldName))
		{
			e.Cancel = true;
		}
	}

	private void designerTreeList_ShownEditor(object sender, EventArgs e)
	{
		TextBoxMaskBox textBoxMaskBox = ((sender as TreeList)?.ActiveEditor as TextEdit)?.MaskBox;
		if (textBoxMaskBox == null)
		{
			return;
		}
		if (designerTreeList.FocusedColumn.Equals(dataTypeTreeListColumn))
		{
			textBoxMaskBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
			textBoxMaskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
			textBoxMaskBox.AutoCompleteCustomSource = ColumnsHelper.DataTypes;
			if (designerTreeList.Width < 80)
			{
				textBoxMaskBox.Width = 80;
			}
		}
		else
		{
			textBoxMaskBox.AutoCompleteMode = AutoCompleteMode.None;
		}
	}

	private void designerTreeList_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		TreeListHitInfo treeListHitInfo = (sender as TreeList).CalcHitInfo(e.Point);
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e, treeListHitInfo);
	}

	private void designerTreeList_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		setRemoveButtonAvailability?.Invoke();
	}

	private void designerTreeList_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (e.Column != null && e.Column.FieldName.Equals("Name"))
		{
			IsNotValid();
			if (designerTreeList.GetRow(e.Node.Id) is ColumnRow columnRow)
			{
				tableDesigner.UpdatePaths(columnRow);
			}
		}
	}

	private void designerTreeList_CustomNodeCellEditForEditing(object sender, GetCustomNodeCellEditEventArgs e)
	{
		if (sender is TreeList treeList && e.Column == typeTreeListColumn)
		{
			_ = e.RepositoryItem;
			ColumnRow columnRow = treeList.GetRow(e.Node.Id) as ColumnRow;
			e.RepositoryItem = ((columnRow.Source == UserTypeEnum.UserType.DBMS) ? typeNormalTreeListRepositoryItemImageComboBox : typeUserDefinedTreeListRepositoryItemImageComboBox);
		}
	}

	private void designerTreeList_GetStateImage(object sender, GetStateImageEventArgs e)
	{
		ColumnRow column = (sender as TreeList).GetDataRecordByNode(e.Node) as ColumnRow;
		if (column != null)
		{
			e.NodeImageIndex = columnTypes.IndexOf(columnTypes.FirstOrDefault((ColumnType x) => x.ObjectType == column.ObjectSubtype && x.Source == column.Source) ?? columnTypes.FirstOrDefault((ColumnType x) => x.ObjectType == SharedObjectSubtypeEnum.ObjectSubtype.Column && x.Source == column.Source));
		}
	}

	private void designerTreeList_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
	{
		if (sender is TreeList treeList && e.Column == typeTreeListColumn)
		{
			_ = e.RepositoryItem;
			if (treeList.GetDataRecordByNode(e.Node) is ColumnRow columnRow)
			{
				e.RepositoryItem = ((columnRow.Source == UserTypeEnum.UserType.DBMS) ? typeNormalTreeListRepositoryItemImageComboBox : typeUserDefinedTreeListRepositoryItemImageComboBox);
			}
		}
	}

	private void typeNormalTreeListRepositoryItemImageComboBox_SelectedValueChanged(object sender, EventArgs e)
	{
		designerTreeList.PostEditor();
	}

	private void typeUserDefinedTreeListRepositoryItemImageComboBox_SelectedValueChanged(object sender, EventArgs e)
	{
		designerTreeList.PostEditor();
	}

	private void nullableRepositoryItemCheckEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (sender is TreeList treeList)
		{
			treeList.CloseEditor();
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
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.designerTreeList = new DevExpress.XtraTreeList.TreeList();
		this.nameTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.nameRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.ordinalPositonTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.typeTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.typeNormalTreeListRepositoryItemImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
		this.dataTypeTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.dataTypeRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.sizeTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.sizeRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.nullableTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.nullableRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.defaultValueTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.computedFormulaTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.identityTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.titleTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.titleRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.descriptionTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.typeUserDefinedTreeListRepositoryItemImageComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
		((System.ComponentModel.ISupportInitialize)this.designerTreeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeNormalTreeListRepositoryItemImageComboBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataTypeRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sizeRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nullableRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeUserDefinedTreeListRepositoryItemImageComboBox).BeginInit();
		base.SuspendLayout();
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.designerTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[11]
		{
			this.nameTreeListColumn, this.ordinalPositonTreeListColumn, this.typeTreeListColumn, this.dataTypeTreeListColumn, this.sizeTreeListColumn, this.nullableTreeListColumn, this.defaultValueTreeListColumn, this.computedFormulaTreeListColumn, this.identityTreeListColumn, this.titleTreeListColumn,
			this.descriptionTreeListColumn
		});
		this.designerTreeList.Dock = System.Windows.Forms.DockStyle.Fill;
		this.designerTreeList.Font = new System.Drawing.Font("Tahoma", 8.25f);
		this.designerTreeList.KeyFieldName = "UniqueId";
		this.designerTreeList.Location = new System.Drawing.Point(0, 0);
		this.designerTreeList.Margin = new System.Windows.Forms.Padding(0);
		this.designerTreeList.Name = "designerTreeList";
		this.designerTreeList.OptionsBehavior.EditorShowMode = DevExpress.XtraTreeList.TreeListEditorShowMode.MouseDownFocused;
		this.designerTreeList.OptionsCustomization.AllowSort = false;
		this.designerTreeList.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Single;
		this.designerTreeList.OptionsSelection.MultiSelect = true;
		this.designerTreeList.OptionsSelection.MultiSelectMode = DevExpress.XtraTreeList.TreeListMultiSelectMode.CellSelect;
		this.designerTreeList.OptionsView.AutoWidth = false;
		this.designerTreeList.OptionsView.ShowHorzLines = false;
		this.designerTreeList.OptionsView.ShowIndicator = false;
		this.designerTreeList.OptionsView.ShowVertLines = false;
		this.designerTreeList.ParentFieldName = "UniqueParentId";
		this.designerTreeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.typeNormalTreeListRepositoryItemImageComboBox, this.typeUserDefinedTreeListRepositoryItemImageComboBox, this.nameRepositoryItemTextEdit, this.titleRepositoryItemTextEdit, this.dataTypeRepositoryItemTextEdit, this.sizeRepositoryItemTextEdit, this.nullableRepositoryItemCheckEdit });
		this.designerTreeList.Size = new System.Drawing.Size(926, 360);
		this.designerTreeList.TabIndex = 1;
		this.designerTreeList.UseDirectXPaint = DevExpress.Utils.DefaultBoolean.False;
		this.designerTreeList.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(designerTreeList_GetStateImage);
		this.designerTreeList.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(designerTreeList_CustomNodeCellEdit);
		this.designerTreeList.CustomNodeCellEditForEditing += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(designerTreeList_CustomNodeCellEditForEditing);
		this.designerTreeList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(designerTreeList_FocusedNodeChanged);
		this.designerTreeList.AfterDropNode += new DevExpress.XtraTreeList.AfterDropNodeEventHandler(designerTreeList_AfterDropNode);
		this.designerTreeList.ShownEditor += new System.EventHandler(designerTreeList_ShownEditor);
		this.designerTreeList.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(designerTreeList_CustomDrawNodeCell);
		this.designerTreeList.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(designerTreeList_PopupMenuShowing);
		this.designerTreeList.CellValueChanging += new DevExpress.XtraTreeList.CellValueChangedEventHandler(designerTreeList_CellValueChanging);
		this.designerTreeList.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(designerTreeList_CellValueChanged);
		this.designerTreeList.ShowingEditor += new System.ComponentModel.CancelEventHandler(designerTreeList_ShowingEditor);
		this.designerTreeList.DragDrop += new System.Windows.Forms.DragEventHandler(designerTreeList_DragDrop);
		this.nameTreeListColumn.Caption = "Name";
		this.nameTreeListColumn.ColumnEdit = this.nameRepositoryItemTextEdit;
		this.nameTreeListColumn.FieldName = "Name";
		this.nameTreeListColumn.MinWidth = 97;
		this.nameTreeListColumn.Name = "nameTreeListColumn";
		this.nameTreeListColumn.OptionsColumn.AllowSort = false;
		this.nameTreeListColumn.Visible = true;
		this.nameTreeListColumn.VisibleIndex = 0;
		this.nameTreeListColumn.Width = 97;
		this.nameRepositoryItemTextEdit.AutoHeight = false;
		this.nameRepositoryItemTextEdit.Name = "nameRepositoryItemTextEdit";
		this.ordinalPositonTreeListColumn.Caption = "#";
		this.ordinalPositonTreeListColumn.FieldName = "DisplayPosition";
		this.ordinalPositonTreeListColumn.MinWidth = 30;
		this.ordinalPositonTreeListColumn.Name = "ordinalPositonTreeListColumn";
		this.ordinalPositonTreeListColumn.OptionsColumn.AllowEdit = false;
		this.ordinalPositonTreeListColumn.OptionsColumn.AllowSort = false;
		this.ordinalPositonTreeListColumn.Visible = true;
		this.ordinalPositonTreeListColumn.VisibleIndex = 1;
		this.typeTreeListColumn.Caption = "Type";
		this.typeTreeListColumn.ColumnEdit = this.typeNormalTreeListRepositoryItemImageComboBox;
		this.typeTreeListColumn.FieldName = "Type";
		this.typeTreeListColumn.Name = "typeTreeListColumn";
		this.typeTreeListColumn.OptionsColumn.AllowSort = false;
		this.typeTreeListColumn.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.typeTreeListColumn.Visible = true;
		this.typeTreeListColumn.VisibleIndex = 2;
		this.typeNormalTreeListRepositoryItemImageComboBox.AutoHeight = false;
		this.typeNormalTreeListRepositoryItemImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.typeNormalTreeListRepositoryItemImageComboBox.Name = "typeNormalTreeListRepositoryItemImageComboBox";
		this.typeNormalTreeListRepositoryItemImageComboBox.SelectedValueChanged += new System.EventHandler(typeNormalTreeListRepositoryItemImageComboBox_SelectedValueChanged);
		this.dataTypeTreeListColumn.Caption = "Data type";
		this.dataTypeTreeListColumn.ColumnEdit = this.dataTypeRepositoryItemTextEdit;
		this.dataTypeTreeListColumn.FieldName = "DataTypeWithoutLength";
		this.dataTypeTreeListColumn.MinWidth = 70;
		this.dataTypeTreeListColumn.Name = "dataTypeTreeListColumn";
		this.dataTypeTreeListColumn.OptionsColumn.AllowSort = false;
		this.dataTypeTreeListColumn.Visible = true;
		this.dataTypeTreeListColumn.VisibleIndex = 3;
		this.dataTypeTreeListColumn.Width = 70;
		this.dataTypeRepositoryItemTextEdit.AutoHeight = false;
		this.dataTypeRepositoryItemTextEdit.Name = "dataTypeRepositoryItemTextEdit";
		this.sizeTreeListColumn.Caption = "Size";
		this.sizeTreeListColumn.ColumnEdit = this.sizeRepositoryItemTextEdit;
		this.sizeTreeListColumn.FieldName = "DataLength";
		this.sizeTreeListColumn.MinWidth = 55;
		this.sizeTreeListColumn.Name = "sizeTreeListColumn";
		this.sizeTreeListColumn.OptionsColumn.AllowSort = false;
		this.sizeTreeListColumn.Visible = true;
		this.sizeTreeListColumn.VisibleIndex = 4;
		this.sizeTreeListColumn.Width = 55;
		this.sizeRepositoryItemTextEdit.AutoHeight = false;
		this.sizeRepositoryItemTextEdit.Name = "sizeRepositoryItemTextEdit";
		this.nullableTreeListColumn.Caption = "Nullable";
		this.nullableTreeListColumn.ColumnEdit = this.nullableRepositoryItemCheckEdit;
		this.nullableTreeListColumn.FieldName = "Nullable";
		this.nullableTreeListColumn.MinWidth = 46;
		this.nullableTreeListColumn.Name = "nullableTreeListColumn";
		this.nullableTreeListColumn.OptionsColumn.AllowSort = false;
		this.nullableTreeListColumn.Visible = true;
		this.nullableTreeListColumn.VisibleIndex = 5;
		this.nullableTreeListColumn.Width = 46;
		this.nullableRepositoryItemCheckEdit.AutoHeight = false;
		this.nullableRepositoryItemCheckEdit.Name = "nullableRepositoryItemCheckEdit";
		this.nullableRepositoryItemCheckEdit.EditValueChanged += new System.EventHandler(nullableRepositoryItemCheckEdit_EditValueChanged);
		this.defaultValueTreeListColumn.Caption = "Default value";
		this.defaultValueTreeListColumn.FieldName = "DefaultValue";
		this.defaultValueTreeListColumn.MinWidth = 102;
		this.defaultValueTreeListColumn.Name = "defaultValueTreeListColumn";
		this.defaultValueTreeListColumn.OptionsColumn.AllowSort = false;
		this.defaultValueTreeListColumn.Visible = true;
		this.defaultValueTreeListColumn.VisibleIndex = 6;
		this.defaultValueTreeListColumn.Width = 102;
		this.computedFormulaTreeListColumn.Caption = "Computed formula";
		this.computedFormulaTreeListColumn.FieldName = "ComputedFormula";
		this.computedFormulaTreeListColumn.MinWidth = 60;
		this.computedFormulaTreeListColumn.Name = "computedFormulaTreeListColumn";
		this.computedFormulaTreeListColumn.OptionsColumn.AllowSort = false;
		this.computedFormulaTreeListColumn.Visible = true;
		this.computedFormulaTreeListColumn.VisibleIndex = 7;
		this.computedFormulaTreeListColumn.Width = 60;
		this.identityTreeListColumn.Caption = "Identity";
		this.identityTreeListColumn.FieldName = "IsIdentity";
		this.identityTreeListColumn.MinWidth = 60;
		this.identityTreeListColumn.Name = "identityTreeListColumn";
		this.identityTreeListColumn.OptionsColumn.AllowSort = false;
		this.identityTreeListColumn.Visible = true;
		this.identityTreeListColumn.VisibleIndex = 8;
		this.identityTreeListColumn.Width = 60;
		this.titleTreeListColumn.Caption = "Title";
		this.titleTreeListColumn.ColumnEdit = this.titleRepositoryItemTextEdit;
		this.titleTreeListColumn.FieldName = "Title";
		this.titleTreeListColumn.MinWidth = 64;
		this.titleTreeListColumn.Name = "titleTreeListColumn";
		this.titleTreeListColumn.OptionsColumn.AllowSort = false;
		this.titleTreeListColumn.Visible = true;
		this.titleTreeListColumn.VisibleIndex = 9;
		this.titleTreeListColumn.Width = 64;
		this.titleRepositoryItemTextEdit.AutoHeight = false;
		this.titleRepositoryItemTextEdit.Name = "titleRepositoryItemTextEdit";
		this.descriptionTreeListColumn.Caption = "Description";
		this.descriptionTreeListColumn.FieldName = "Description";
		this.descriptionTreeListColumn.Name = "descriptionTreeListColumn";
		this.descriptionTreeListColumn.OptionsColumn.AllowSort = false;
		this.descriptionTreeListColumn.Visible = true;
		this.descriptionTreeListColumn.VisibleIndex = 10;
		this.typeUserDefinedTreeListRepositoryItemImageComboBox.AutoHeight = false;
		this.typeUserDefinedTreeListRepositoryItemImageComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.typeUserDefinedTreeListRepositoryItemImageComboBox.Name = "typeUserDefinedTreeListRepositoryItemImageComboBox";
		this.typeUserDefinedTreeListRepositoryItemImageComboBox.SelectedValueChanged += new System.EventHandler(typeUserDefinedTreeListRepositoryItemImageComboBox_SelectedValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.designerTreeList);
		base.Name = "DesignTableUserControl";
		base.Size = new System.Drawing.Size(926, 360);
		((System.ComponentModel.ISupportInitialize)this.designerTreeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeNormalTreeListRepositoryItemImageComboBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataTypeRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sizeRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nullableRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeUserDefinedTreeListRepositoryItemImageComboBox).EndInit();
		base.ResumeLayout(false);
	}
}
