using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Base;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Base.EventArgs;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Support;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm;

public class ConnectionTypeUserControl : BaseWizardPageControl
{
	private IEnumerable<ConnectionTypeItem> datasource;

	private IContainer components;

	private GridControl gridControl;

	private TileView tileView;

	private BindingSource connectionTypeItemBindingSource;

	private TileViewColumn iconGridColumn;

	private TileViewColumn nameGridColumn;

	private NonCustomizableLayoutControl mainNonCustomizableLayoutControl;

	private SearchControl searchControl;

	private LayoutControlGroup Root;

	private LayoutControlItem mainLayoutControlItem;

	private LayoutControlItem searchLayoutControlItem;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	public ConnectionTypeUserControl()
	{
		InitializeComponent();
		SetConnectionTypes();
	}

	protected void OnFocusedRowChanged(TileView tileView)
	{
		base.AllowContinue = tileView?.GetFocusedRow() != null;
	}

	private void ConnectionTypeUserControl_Load(object sender, EventArgs e)
	{
		base.OnAllowContinueChanged(new NavigationEventArgs(isAllowed: false));
	}

	private void SetConnectionTypes()
	{
		datasource = ConnectionTypes.GetConnectionTypes();
		gridControl.BeginUpdate();
		gridControl.DataSource = datasource;
		gridControl.EndUpdate();
		tileView.FocusedRowHandle = int.MinValue;
	}

	private void tileView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		OnFocusedRowChanged(sender as TileView);
	}

	private void gridControl_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left && ((sender as GridControl)?.FocusedView as TileView)?.GetFocusedRow() != null)
		{
			OnContinue();
		}
	}

	private void tileView_Click(object sender, EventArgs e)
	{
		OnFocusedRowChanged(sender as TileView);
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
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition obj = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj3 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj4 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj5 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition obj6 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		this.iconGridColumn = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.nameGridColumn = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.gridControl = new DevExpress.XtraGrid.GridControl();
		this.connectionTypeItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
		this.tileView = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.mainNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.searchControl = new DevExpress.XtraEditors.SearchControl();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.mainLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.searchLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.gridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectionTypeItemBindingSource).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).BeginInit();
		this.mainNonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.searchControl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.searchLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.iconGridColumn.FieldName = "Icon";
		this.iconGridColumn.Name = "iconGridColumn";
		this.iconGridColumn.Visible = true;
		this.iconGridColumn.VisibleIndex = 0;
		this.nameGridColumn.FieldName = "Name";
		this.nameGridColumn.Name = "nameGridColumn";
		this.nameGridColumn.Visible = true;
		this.nameGridColumn.VisibleIndex = 1;
		this.gridControl.DataSource = this.connectionTypeItemBindingSource;
		this.gridControl.Location = new System.Drawing.Point(2, 29);
		this.gridControl.MainView = this.tileView;
		this.gridControl.Name = "gridControl";
		this.gridControl.Size = new System.Drawing.Size(556, 442);
		this.gridControl.TabIndex = 0;
		this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileView });
		this.gridControl.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(gridControl_MouseDoubleClick);
		this.connectionTypeItemBindingSource.DataSource = typeof(Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Support.ConnectionTypeItem);
		this.tileView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.iconGridColumn, this.nameGridColumn });
		this.tileView.GridControl = this.gridControl;
		this.tileView.Name = "tileView";
		this.tileView.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileView.OptionsTiles.RowCount = 0;
		this.tileView.TileColumns.Add(obj);
		this.tileView.TileRows.Add(obj2);
		this.tileView.TileRows.Add(obj3);
		this.tileView.TileRows.Add(obj4);
		this.tileView.TileRows.Add(obj5);
		this.tileView.TileRows.Add(obj6);
		tileViewItemElement.Column = this.iconGridColumn;
		tileViewItemElement.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement.RowIndex = 1;
		tileViewItemElement.Text = "iconGridColumn";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.Column = this.nameGridColumn;
		tileViewItemElement2.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement2.RowIndex = 3;
		tileViewItemElement2.Text = "nameGridColumn";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.tileView.TileTemplate.Add(tileViewItemElement);
		this.tileView.TileTemplate.Add(tileViewItemElement2);
		this.tileView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(tileView_FocusedRowChanged);
		this.tileView.Click += new System.EventHandler(tileView_Click);
		this.mainNonCustomizableLayoutControl.AllowCustomization = false;
		this.mainNonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainNonCustomizableLayoutControl.Controls.Add(this.searchControl);
		this.mainNonCustomizableLayoutControl.Controls.Add(this.gridControl);
		this.mainNonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainNonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainNonCustomizableLayoutControl.MenuManager = this.barManager;
		this.mainNonCustomizableLayoutControl.Name = "mainNonCustomizableLayoutControl";
		this.mainNonCustomizableLayoutControl.Root = this.Root;
		this.mainNonCustomizableLayoutControl.Size = new System.Drawing.Size(560, 473);
		this.mainNonCustomizableLayoutControl.TabIndex = 1;
		this.mainNonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.searchControl.Client = this.gridControl;
		this.searchControl.Location = new System.Drawing.Point(20, 2);
		this.searchControl.MaximumSize = new System.Drawing.Size(420, 0);
		this.searchControl.Name = "searchControl";
		this.searchControl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Repository.ClearButton(),
			new DevExpress.XtraEditors.Repository.SearchButton()
		});
		this.searchControl.Properties.Client = this.gridControl;
		this.searchControl.Size = new System.Drawing.Size(420, 20);
		this.searchControl.StyleController = this.mainNonCustomizableLayoutControl;
		this.searchControl.TabIndex = 4;
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(560, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 473);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(560, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 473);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(560, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 473);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.mainLayoutControlItem, this.searchLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(560, 473);
		this.Root.TextVisible = false;
		this.mainLayoutControlItem.Control = this.gridControl;
		this.mainLayoutControlItem.Location = new System.Drawing.Point(0, 27);
		this.mainLayoutControlItem.Name = "mainLayoutControlItem";
		this.mainLayoutControlItem.Size = new System.Drawing.Size(560, 446);
		this.mainLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.mainLayoutControlItem.TextVisible = false;
		this.searchLayoutControlItem.Control = this.searchControl;
		this.searchLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.searchLayoutControlItem.Name = "searchLayoutControlItem";
		this.searchLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(20, 2, 2, 5);
		this.searchLayoutControlItem.Size = new System.Drawing.Size(560, 27);
		this.searchLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.searchLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		base.Controls.Add(this.mainNonCustomizableLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "ConnectionTypeUserControl";
		base.Size = new System.Drawing.Size(560, 473);
		base.Load += new System.EventHandler(ConnectionTypeUserControl_Load);
		((System.ComponentModel.ISupportInitialize)this.gridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectionTypeItemBindingSource).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).EndInit();
		this.mainNonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.searchControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.searchLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
