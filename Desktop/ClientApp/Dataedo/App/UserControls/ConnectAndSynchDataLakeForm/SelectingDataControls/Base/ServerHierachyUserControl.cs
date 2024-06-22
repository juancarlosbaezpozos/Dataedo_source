using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Base.EventArgs;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base.HierarchyModel;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base;

public class ServerHierachyUserControl : BaseUserControl
{
	private IContainer components;

	private NonCustomizableLayoutControl mainNonCustomizableLayoutControl;

	private TreeList treeList;

	private LabelControl titleLabelControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private LayoutControlItem titleLabelControlLayoutControlItem;

	private LayoutControlItem treeListLayoutControlItem;

	private TreeListColumn displayNameTreeListColumn;

	private SvgImageCollection svgImageCollection;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ToolTipController toolTipController;

	public Item SelectedItem { get; private set; }

	public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

	public ServerHierachyUserControl()
	{
		InitializeComponent();
		Initialize();
	}

	private void Initialize()
	{
		treeList.DataSource = new RootItem();
		treeList.ForceInitialize();
		treeList.Nodes[0].Expand();
	}

	protected virtual void OnSelectedItemChanged(SelectedItemChangedEventArgs e)
	{
		this.SelectedItemChanged?.Invoke(this, e);
	}

	private void treeList_VirtualTreeGetCellValue(object sender, VirtualTreeGetCellValueInfo e)
	{
		e.CellData = ((Item)e.Node).DisplayName;
	}

	private void treeList_VirtualTreeGetChildNodes(object sender, VirtualTreeGetChildNodesInfo e)
	{
		Cursor current = Cursor.Current;
		Cursor.Current = Cursors.WaitCursor;
		e.Children = ((Item)e.Node).GetChildItems();
		Cursor.Current = current;
	}

	private void treeList_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		Item item2 = (SelectedItem = (Item)(((TreeList)sender)?.GetRow(e.Node.Id)));
		OnSelectedItemChanged(new SelectedItemChangedEventArgs(item2));
	}

	private void treeList_CustomDrawNodeImages(object sender, CustomDrawNodeImagesEventArgs e)
	{
		Item item = (Item)(((TreeList)sender)?.GetRow(e.Node.Id));
		if (item.Image != null)
		{
			e.Cache.DrawImage(item.Image, e.SelectImageLocation);
			e.Handled = true;
		}
	}

	private void treeList_GetSelectImage(object sender, GetSelectImageEventArgs e)
	{
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
		this.mainNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.treeList = new DevExpress.XtraTreeList.TreeList();
		this.displayNameTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.svgImageCollection = new DevExpress.Utils.SvgImageCollection(this.components);
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.titleLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.titleLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.treeListLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).BeginInit();
		this.mainNonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.treeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.svgImageCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.treeListLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.mainNonCustomizableLayoutControl.AllowCustomization = false;
		this.mainNonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainNonCustomizableLayoutControl.Controls.Add(this.treeList);
		this.mainNonCustomizableLayoutControl.Controls.Add(this.titleLabelControl);
		this.mainNonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainNonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainNonCustomizableLayoutControl.MenuManager = this.barManager;
		this.mainNonCustomizableLayoutControl.Name = "mainNonCustomizableLayoutControl";
		this.mainNonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2678, 739, 650, 400);
		this.mainNonCustomizableLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainNonCustomizableLayoutControl.Size = new System.Drawing.Size(489, 429);
		this.mainNonCustomizableLayoutControl.TabIndex = 17;
		this.mainNonCustomizableLayoutControl.Text = "mainNonCustomizableLayoutControl";
		this.treeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[1] { this.displayNameTreeListColumn });
		this.treeList.Location = new System.Drawing.Point(2, 19);
		this.treeList.MinimumSize = new System.Drawing.Size(100, 0);
		this.treeList.Name = "treeList";
		this.treeList.OptionsBehavior.Editable = false;
		this.treeList.OptionsFind.AllowFindPanel = false;
		this.treeList.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.treeList.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.None;
		this.treeList.OptionsView.ShowColumns = false;
		this.treeList.OptionsView.ShowHorzLines = false;
		this.treeList.OptionsView.ShowIndentAsRowStyle = true;
		this.treeList.OptionsView.ShowIndicator = false;
		this.treeList.OptionsView.ShowVertLines = false;
		this.treeList.RowHeight = 22;
		this.treeList.SelectImageList = this.svgImageCollection;
		this.treeList.Size = new System.Drawing.Size(485, 408);
		this.treeList.TabIndex = 12;
		this.treeList.ToolTipController = this.toolTipController;
		this.treeList.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(treeList_GetSelectImage);
		this.treeList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(treeList_FocusedNodeChanged);
		this.treeList.CustomDrawNodeImages += new DevExpress.XtraTreeList.CustomDrawNodeImagesEventHandler(treeList_CustomDrawNodeImages);
		this.treeList.VirtualTreeGetChildNodes += new DevExpress.XtraTreeList.VirtualTreeGetChildNodesEventHandler(treeList_VirtualTreeGetChildNodes);
		this.treeList.VirtualTreeGetCellValue += new DevExpress.XtraTreeList.VirtualTreeGetCellValueEventHandler(treeList_VirtualTreeGetCellValue);
		this.displayNameTreeListColumn.Caption = "DisplayName";
		this.displayNameTreeListColumn.FieldName = "DisplayName";
		this.displayNameTreeListColumn.Name = "displayNameTreeListColumn";
		this.displayNameTreeListColumn.Visible = true;
		this.displayNameTreeListColumn.VisibleIndex = 0;
		this.svgImageCollection.Add("electronics_desktopwindows", "image://svgimages/icon builder/electronics_desktopwindows.svg");
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.titleLabelControl.Location = new System.Drawing.Point(2, 2);
		this.titleLabelControl.Name = "titleLabelControl";
		this.titleLabelControl.Size = new System.Drawing.Size(81, 13);
		this.titleLabelControl.StyleController = this.mainNonCustomizableLayoutControl;
		this.titleLabelControl.TabIndex = 4;
		this.titleLabelControl.Text = "Server Hierarchy";
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(489, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 429);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(489, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 429);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(489, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 429);
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.titleLabelControlLayoutControlItem, this.treeListLayoutControlItem });
		this.mainLayoutControlGroup.Name = "mainLayoutControlGroup";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(489, 429);
		this.mainLayoutControlGroup.TextVisible = false;
		this.titleLabelControlLayoutControlItem.Control = this.titleLabelControl;
		this.titleLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.titleLabelControlLayoutControlItem.Name = "titleLabelControlLayoutControlItem";
		this.titleLabelControlLayoutControlItem.Size = new System.Drawing.Size(489, 17);
		this.titleLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.titleLabelControlLayoutControlItem.TextVisible = false;
		this.treeListLayoutControlItem.Control = this.treeList;
		this.treeListLayoutControlItem.Location = new System.Drawing.Point(0, 17);
		this.treeListLayoutControlItem.Name = "treeListLayoutControlItem";
		this.treeListLayoutControlItem.Size = new System.Drawing.Size(489, 412);
		this.treeListLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.treeListLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainNonCustomizableLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "ServerHierachyUserControl";
		base.Size = new System.Drawing.Size(489, 429);
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).EndInit();
		this.mainNonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.treeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.svgImageCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.treeListLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
