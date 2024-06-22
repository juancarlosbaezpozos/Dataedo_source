using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base.ObjectModel;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base;

public class AvailableObjectsUserControl : BaseUserControl
{
	private IContainer components;

	private NonCustomizableLayoutControl mainNonCustomizableLayoutControl;

	private TreeList treeList;

	private LabelControl titleLabelControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private LayoutControlItem titleLabelControlLayoutControlItem;

	private LayoutControlItem treeListLayoutControlItem;

	private SearchControl searchControl;

	private LayoutControlItem searchControlLayoutControlItem;

	private SvgImageCollection svgImageCollection;

	private TreeListColumn displayNameTreeListColumn;

	private TreeListColumn typeTreeListColumn;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ToolTipController toolTipController;

	public AvailableObjectsUserControl()
	{
		InitializeComponent();
		Initialize();
	}

	public void SetSource(string path)
	{
		List<ObjectItem> dataSource = new List<ObjectItem>();
		try
		{
			dataSource = new List<ObjectItem>(from x in new DirectoryInfo(path).GetFiles()
				where (x.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden
				select new ObjectItem(x));
		}
		catch (Exception)
		{
		}
		treeList.DataSource = dataSource;
		treeList.ForceInitialize();
		treeList.BestFitColumns();
	}

	private void Initialize()
	{
	}

	private void treeList_CustomDrawNodeImages(object sender, CustomDrawNodeImagesEventArgs e)
	{
		ObjectItem objectItem = (ObjectItem)(((TreeList)sender)?.GetRow(e.Node.Id));
		if (objectItem.Image != null)
		{
			e.Cache.DrawImage(objectItem.Image, e.SelectImageLocation);
			e.Handled = true;
		}
	}

	private void treeList_GetSelectImage(object sender, GetSelectImageEventArgs e)
	{
		e.NodeImageIndex = 0;
	}

	private void treeList_MouseDoubleClick(object sender, MouseEventArgs e)
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
		this.searchControl = new DevExpress.XtraEditors.SearchControl();
		this.treeList = new DevExpress.XtraTreeList.TreeList();
		this.displayNameTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.typeTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
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
		this.searchControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).BeginInit();
		this.mainNonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.searchControl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.treeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.svgImageCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.treeListLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.searchControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.mainNonCustomizableLayoutControl.AllowCustomization = false;
		this.mainNonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainNonCustomizableLayoutControl.Controls.Add(this.searchControl);
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
		this.searchControl.Client = this.treeList;
		this.searchControl.Location = new System.Drawing.Point(2, 19);
		this.searchControl.Name = "searchControl";
		this.searchControl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()
		});
		this.searchControl.Properties.Client = this.treeList;
		this.searchControl.Size = new System.Drawing.Size(485, 20);
		this.searchControl.StyleController = this.mainNonCustomizableLayoutControl;
		this.searchControl.TabIndex = 13;
		this.treeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[2] { this.displayNameTreeListColumn, this.typeTreeListColumn });
		this.treeList.Location = new System.Drawing.Point(2, 43);
		this.treeList.MinimumSize = new System.Drawing.Size(100, 0);
		this.treeList.Name = "treeList";
		this.treeList.OptionsBehavior.Editable = false;
		this.treeList.OptionsFind.AllowFindPanel = false;
		this.treeList.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.treeList.OptionsSelection.MultiSelect = true;
		this.treeList.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.None;
		this.treeList.OptionsView.ShowColumns = false;
		this.treeList.OptionsView.ShowHorzLines = false;
		this.treeList.OptionsView.ShowIndentAsRowStyle = true;
		this.treeList.OptionsView.ShowIndicator = false;
		this.treeList.OptionsView.ShowRoot = false;
		this.treeList.OptionsView.ShowVertLines = false;
		this.treeList.RowHeight = 22;
		this.treeList.SelectImageList = this.svgImageCollection;
		this.treeList.Size = new System.Drawing.Size(485, 384);
		this.treeList.TabIndex = 12;
		this.treeList.ToolTipController = this.toolTipController;
		this.treeList.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(treeList_GetSelectImage);
		this.treeList.CustomDrawNodeImages += new DevExpress.XtraTreeList.CustomDrawNodeImagesEventHandler(treeList_CustomDrawNodeImages);
		this.treeList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(treeList_MouseDoubleClick);
		this.displayNameTreeListColumn.Caption = "DisplayName";
		this.displayNameTreeListColumn.FieldName = "DisplayName";
		this.displayNameTreeListColumn.Name = "displayNameTreeListColumn";
		this.displayNameTreeListColumn.Visible = true;
		this.displayNameTreeListColumn.VisibleIndex = 0;
		this.typeTreeListColumn.Caption = "Type";
		this.typeTreeListColumn.FieldName = "TypeDisplayName";
		this.typeTreeListColumn.MinWidth = 40;
		this.typeTreeListColumn.Name = "typeTreeListColumn";
		this.typeTreeListColumn.Visible = true;
		this.typeTreeListColumn.VisibleIndex = 1;
		this.svgImageCollection.Add("bo_list", "image://svgimages/business objects/bo_list.svg");
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.titleLabelControl.Location = new System.Drawing.Point(2, 2);
		this.titleLabelControl.Name = "titleLabelControl";
		this.titleLabelControl.Size = new System.Drawing.Size(81, 13);
		this.titleLabelControl.StyleController = this.mainNonCustomizableLayoutControl;
		this.titleLabelControl.TabIndex = 4;
		this.titleLabelControl.Text = "Available objects";
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
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.titleLabelControlLayoutControlItem, this.treeListLayoutControlItem, this.searchControlLayoutControlItem });
		this.mainLayoutControlGroup.Name = "hierarchyLayoutControlGroup";
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
		this.treeListLayoutControlItem.Location = new System.Drawing.Point(0, 41);
		this.treeListLayoutControlItem.Name = "treeListLayoutControlItem";
		this.treeListLayoutControlItem.Size = new System.Drawing.Size(489, 388);
		this.treeListLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.treeListLayoutControlItem.TextVisible = false;
		this.searchControlLayoutControlItem.Control = this.searchControl;
		this.searchControlLayoutControlItem.Location = new System.Drawing.Point(0, 17);
		this.searchControlLayoutControlItem.Name = "searchControlLayoutControlItem";
		this.searchControlLayoutControlItem.Size = new System.Drawing.Size(489, 24);
		this.searchControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.searchControlLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainNonCustomizableLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "AvailableObjectsUserControl";
		base.Size = new System.Drawing.Size(489, 429);
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).EndInit();
		this.mainNonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.searchControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.treeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.svgImageCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.treeListLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.searchControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
