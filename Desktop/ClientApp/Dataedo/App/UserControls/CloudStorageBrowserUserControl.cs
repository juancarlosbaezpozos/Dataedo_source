using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Helpers.CloudStorage;
using Dataedo.App.Helpers.Controls;
using Dataedo.App.Import.DataLake;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.UserControls;

public class CloudStorageBrowserUserControl : UserControl
{
	public const int WarningSizeInBytes = 104857600;

	private const string PossibleDirectoryToolTipInfoText = "This item can be either 0-byte file, or directory You don't have access to";

	protected CloudStorageBrowserInfo _browserInfo;

	private Cursor _expandCursor;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private TreeList cloudStorageBrowserTreeList;

	private LayoutControlItem cloudStorageBrowserLayoutControlItem;

	private TextEdit searchTextEdit;

	private LayoutControlItem searchLayoutControlItem;

	private ToolTipController toolTipController1;

	public bool SquishFirstLevelIfOneItem { get; protected set; }

	public bool ExpandOneItemLevels { get; protected set; }

	public int StartingExpandLevel { get; protected set; }

	public bool IsDynamicLoading { get; protected set; }

	public bool AllowDirectorySelect
	{
		get
		{
			CloudStorageBrowserInfo browserInfo = _browserInfo;
			if (browserInfo == null)
			{
				return false;
			}
			return browserInfo.DataLakeType == DataLakeTypeEnum.DataLakeType.DELTALAKE;
		}
	}

	public bool AllowFileSelect
	{
		get
		{
			CloudStorageBrowserInfo browserInfo = _browserInfo;
			if (browserInfo == null)
			{
				return true;
			}
			return browserInfo.DataLakeType != DataLakeTypeEnum.DataLakeType.DELTALAKE;
		}
	}

	public int MaxSelectedItems
	{
		get
		{
			CloudStorageBrowserInfo browserInfo = _browserInfo;
			if (browserInfo != null && browserInfo.DataLakeType == DataLakeTypeEnum.DataLakeType.DELTALAKE)
			{
				return 1;
			}
			return -1;
		}
	}

	public event EventHandler SelectedItemsChanged;

	public CloudStorageBrowserUserControl()
	{
		InitializeComponent();
		InitColumns();
		cloudStorageBrowserTreeList.OptionsView.CheckBoxStyle = DefaultNodeCheckBoxStyle.Check;
		cloudStorageBrowserTreeList.CustomDrawNodeCheckBox += CloudStorageBrowserTreeList_CustomDrawNodeCheckBox;
		DataLakeImageCollection dataLakeImageCollection = new DataLakeImageCollection();
		dataLakeImageCollection.Init();
		cloudStorageBrowserTreeList.StateImageList = dataLakeImageCollection;
		cloudStorageBrowserTreeList.GetStateImage += CloudStorageBrowserTreeListTreeList_GetStateImage;
		searchTextEdit.EditValueChanged += SearchTextEdit_EditValueChanged;
		cloudStorageBrowserTreeList.AfterCheckNode += CloudStorageBrowserTreeList_AfterCheckNode;
		cloudStorageBrowserTreeList.BeforeCheckNode += CloudStorageBrowserTreeList_BeforeCheckNode;
		cloudStorageBrowserTreeList.CustomColumnSort += CloudStorageBrowserTreeList_CustomColumnSort;
		cloudStorageBrowserTreeList.VirtualTreeGetChildNodes += CloudStorageBrowserTreeList_VirtualTreeGetChildNodes;
		cloudStorageBrowserTreeList.VirtualTreeGetCellValue += CloudStorageBrowserTreeList_VirtualTreeGetCellValue;
		cloudStorageBrowserTreeList.BeforeExpand += CloudStorageBrowserTreeList_BeforeExpand;
		cloudStorageBrowserTreeList.AfterExpand += CloudStorageBrowserTreeList_AfterExpand;
		toolTipController1.GetActiveObjectInfo += ToolTipController1_GetActiveObjectInfo;
		StartingExpandLevel = 0;
		IsDynamicLoading = false;
		ExpandOneItemLevels = true;
	}

	public static FileLikeNode GetNode(TreeListNode treeNode)
	{
		if (treeNode.Tag is FileLikeNode result)
		{
			return result;
		}
		return (FileLikeNode)treeNode.TreeList.GetDataRecordByNode(treeNode);
	}

	public IEnumerable<TNode> GetCheckedObjects<TNode>() where TNode : FileLikeNode
	{
		return (from x in cloudStorageBrowserTreeList.GetAllCheckedNodes()
			select GetNode(x) as TNode).ToList();
	}

	public static List<ColumnCreatorData<FileLikeNode>> GetColumnCreators()
	{
		return new List<ColumnCreatorData<FileLikeNode>>
		{
			new ColumnCreatorData<FileLikeNode>
			{
				Caption = "Name",
				FieldName = "Name",
				GetValue = (FileLikeNode x) => x.Name
			},
			new ColumnCreatorData<FileLikeNode>
			{
				Caption = "Last Modified",
				FieldName = "LastModified",
				GetValue = (FileLikeNode x) => x.LastModified
			},
			new ColumnCreatorData<FileLikeNode>
			{
				Caption = "Size",
				FieldName = "ReadableSize",
				GetValue = (FileLikeNode x) => x.ReadableSize,
				CustomSort = true
			}
		};
	}

	private void InitColumns()
	{
		List<ColumnCreatorData<FileLikeNode>> columnCreators = GetColumnCreators();
		for (int i = 0; i < columnCreators.Count; i++)
		{
			TreeListColumn column = columnCreators[i].CreateColumn(i);
			cloudStorageBrowserTreeList.Columns.Add(column);
		}
	}

	public void ClearNodes()
	{
		cloudStorageBrowserTreeList.ClearNodes();
		cloudStorageBrowserTreeList.Refresh();
	}

	protected void InitTreeView(IReadOnlyList<FileLikeNode> objects, bool dynamicLoading)
	{
		ClearNodes();
		if (dynamicLoading)
		{
			cloudStorageBrowserTreeList.EnableDynamicLoading = true;
			cloudStorageBrowserTreeList.DataSource = new FileLikeNodeDynamicRootNode(objects);
		}
		else
		{
			IReadOnlyList<FileLikeNode> readOnlyList = objects;
			if (SquishFirstLevelIfOneItem && readOnlyList.Count == 1)
			{
				readOnlyList = readOnlyList[0].Children;
			}
			cloudStorageBrowserTreeList.ChildListFieldName = "Children";
			List<FileLikeNode> dataSource = readOnlyList.ToList();
			cloudStorageBrowserTreeList.DataSource = dataSource;
		}
		cloudStorageBrowserTreeList.ExpandToLevel(StartingExpandLevel);
		SetDynamicLoadingOptions(dynamicLoading);
	}

	private void SetDynamicLoadingOptions(bool dynamicLoading)
	{
		IsDynamicLoading = dynamicLoading;
		cloudStorageBrowserTreeList.OptionsMenu.ShowExpandCollapseItems = !dynamicLoading;
	}

	private static int GetImageIndex(TreeListNode treeListNode, DataLakeImageCollection imageCollection)
	{
		if (treeListNode != null)
		{
			FileLikeNode node = GetNode(treeListNode);
			if (node != null)
			{
				if (treeListNode.Nodes.Count > 0 || node.IsDirectoryLike)
				{
					return imageCollection.FolderImageIndex;
				}
				Exception exception;
				DataLakeTypeEnum.DataLakeType? dataLakeType = DataLakeTypeDeterminer.DetermineTypeByFileExtension(node.Name, null, out exception);
				if (!dataLakeType.HasValue)
				{
					return imageCollection.UnknownFileImageIndex;
				}
				return imageCollection.GetIndex(dataLakeType.Value);
			}
		}
		return imageCollection.UnknownFileImageIndex;
	}

	private void SearchTextEdit_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void CloudStorageBrowserTreeList_CustomDrawNodeCheckBox(object sender, CustomDrawNodeCheckBoxEventArgs e)
	{
		if (!CanDrawCheckBox(e.Node))
		{
			e.Handled = true;
			e.ObjectArgs.State = ObjectState.Disabled;
		}
		else if (MaxSelectedItems >= 0 && cloudStorageBrowserTreeList.GetAllCheckedNodes().Count >= MaxSelectedItems && !e.Node.Checked)
		{
			e.ObjectArgs.State = ObjectState.Disabled;
		}
	}

	private bool CanDrawCheckBox(TreeListNode node)
	{
		if (node.Nodes.Count > 0)
		{
			if (!AllowDirectorySelect)
			{
				return false;
			}
		}
		else
		{
			FileLikeNode node2 = GetNode(node);
			if (node2 != null && node2.IsDirectoryLike)
			{
				if (!AllowDirectorySelect)
				{
					return false;
				}
			}
			else
			{
				if (GetNode(node) == null)
				{
					return false;
				}
				if (!AllowFileSelect)
				{
					return false;
				}
			}
		}
		return true;
	}

	private void CloudStorageBrowserTreeListTreeList_GetStateImage(object sender, GetStateImageEventArgs e)
	{
		if (sender is TreeList treeList && treeList.StateImageList is DataLakeImageCollection imageCollection)
		{
			e.NodeImageIndex = GetImageIndex(e.Node, imageCollection);
		}
	}

	private void CloudStorageBrowserTreeList_AfterCheckNode(object sender, NodeEventArgs e)
	{
		this.SelectedItemsChanged?.Invoke(this, EventArgs.Empty);
	}

	private void CloudStorageBrowserTreeList_BeforeCheckNode(object sender, CheckNodeEventArgs e)
	{
		if (!CanDrawCheckBox(e.Node))
		{
			e.CanCheck = false;
		}
		else
		{
			e.CanCheck = e.State == CheckState.Unchecked || MaxSelectedItems < 0 || cloudStorageBrowserTreeList.GetAllCheckedNodes().Count < MaxSelectedItems;
		}
	}

	private void CloudStorageBrowserTreeList_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		if (!(e.Column.FieldName == "ReadableSize"))
		{
			return;
		}
		FileLikeNode node = GetNode(e.Node1);
		if (node == null)
		{
			return;
		}
		FileLikeNode node2 = GetNode(e.Node2);
		if (node2 != null)
		{
			if (node.Size > node2.Size)
			{
				e.Result = 1;
			}
			else if (node.Size < node2.Size)
			{
				e.Result = -1;
			}
			else
			{
				e.Result = 0;
			}
		}
	}

	private void ToolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		if (e.SelectedControl != cloudStorageBrowserTreeList)
		{
			return;
		}
		TreeListHitInfo treeListHitInfo = cloudStorageBrowserTreeList.CalcHitInfo(e.ControlMousePosition);
		if (treeListHitInfo.InRowCell)
		{
			FileLikeNode node = GetNode(treeListHitInfo.Node);
			if (node.Size == 0 && !node.IsDirectoryLike)
			{
				ToolTipControlInfo toolTipControlInfo2 = (e.Info = new ToolTipControlInfo(node, "This item can be either 0-byte file, or directory You don't have access to"));
			}
		}
	}

	private void CloudStorageBrowserTreeList_VirtualTreeGetCellValue(object sender, VirtualTreeGetCellValueInfo e)
	{
		if (e.Node is FileLikeNode fileLikeNode)
		{
			switch (e.Column.FieldName)
			{
			case "Name":
				e.CellData = fileLikeNode.Name;
				break;
			case "ReadableSize":
				e.CellData = fileLikeNode.ReadableSize;
				break;
			case "LastModified":
				e.CellData = fileLikeNode.LastModified;
				break;
			}
		}
	}

	private void CloudStorageBrowserTreeList_VirtualTreeGetChildNodes(object sender, VirtualTreeGetChildNodesInfo e)
	{
		if (e.Node is FileLikeNode fileLikeNode)
		{
			e.Children = fileLikeNode.Children.ToList();
		}
	}

	private void CloudStorageBrowserTreeList_AfterExpand(object sender, NodeEventArgs e)
	{
		if (ExpandOneItemLevels && e.Node.Nodes.Count == 1)
		{
			e.Node.Nodes[0].Expand();
		}
		else if (_expandCursor != null)
		{
			Cursor.Current = _expandCursor;
			_expandCursor = null;
		}
	}

	private void CloudStorageBrowserTreeList_BeforeExpand(object sender, BeforeExpandEventArgs e)
	{
		if (IsDynamicLoading && _expandCursor == null)
		{
			_expandCursor = Cursor.Current;
			Cursor.Current = Cursors.WaitCursor;
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
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.searchTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.cloudStorageBrowserTreeList = new DevExpress.XtraTreeList.TreeList();
		this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.cloudStorageBrowserLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.searchLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.searchTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cloudStorageBrowserTreeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cloudStorageBrowserLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.searchLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.searchTextEdit);
		this.nonCustomizableLayoutControl.Controls.Add(this.cloudStorageBrowserTreeList);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(519, 413);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl";
		this.searchTextEdit.Location = new System.Drawing.Point(55, 12);
		this.searchTextEdit.Name = "searchTextEdit";
		this.searchTextEdit.Size = new System.Drawing.Size(452, 20);
		this.searchTextEdit.StyleController = this.nonCustomizableLayoutControl;
		this.searchTextEdit.TabIndex = 29;
		this.cloudStorageBrowserTreeList.CustomizationFormBounds = new System.Drawing.Rectangle(314, 335, 252, 234);
		this.cloudStorageBrowserTreeList.Location = new System.Drawing.Point(12, 36);
		this.cloudStorageBrowserTreeList.Name = "cloudStorageBrowserTreeList";
		this.cloudStorageBrowserTreeList.OptionsBehavior.Editable = false;
		this.cloudStorageBrowserTreeList.OptionsSelection.MultiSelect = true;
		this.cloudStorageBrowserTreeList.OptionsView.ShowIndicator = false;
		this.cloudStorageBrowserTreeList.Size = new System.Drawing.Size(495, 365);
		this.cloudStorageBrowserTreeList.TabIndex = 28;
		this.cloudStorageBrowserTreeList.ToolTipController = this.toolTipController1;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.cloudStorageBrowserLayoutControlItem, this.searchLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(519, 413);
		this.Root.TextVisible = false;
		this.cloudStorageBrowserLayoutControlItem.Control = this.cloudStorageBrowserTreeList;
		this.cloudStorageBrowserLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.cloudStorageBrowserLayoutControlItem.Name = "cloudStorageBrowserLayoutControlItem";
		this.cloudStorageBrowserLayoutControlItem.Size = new System.Drawing.Size(499, 369);
		this.cloudStorageBrowserLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.cloudStorageBrowserLayoutControlItem.TextVisible = false;
		this.searchLayoutControlItem.Control = this.searchTextEdit;
		this.searchLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.searchLayoutControlItem.Name = "searchLayoutControlItem";
		this.searchLayoutControlItem.Size = new System.Drawing.Size(499, 24);
		this.searchLayoutControlItem.Text = "Search: ";
		this.searchLayoutControlItem.TextSize = new System.Drawing.Size(40, 13);
		this.searchLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.Name = "CloudStorageBrowserUserControl";
		base.Size = new System.Drawing.Size(519, 413);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.searchTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cloudStorageBrowserTreeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cloudStorageBrowserLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.searchLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
