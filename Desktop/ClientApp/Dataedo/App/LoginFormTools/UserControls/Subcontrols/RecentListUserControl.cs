using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.API.Models;
using Dataedo.App.LoginFormTools.Tools.Licenses;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Newtonsoft.Json;

namespace Dataedo.App.LoginFormTools.UserControls.Subcontrols;

public class RecentListUserControl : XtraUserControl
{
	private int lastFocusedRowHandle;

	private object lastLicenses;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private GridControl gridControl;

	private TileView tileView;

	private TileViewColumn packageNameTileViewColumn;

	private LayoutControlItem gridControlLayoutControlItem;

	private ToolTipController toolTipController;

	private TileViewColumn organizationTileViewColumn;

	private TileViewColumn expiresOnTileViewColumn;

	public GridControl GridControl => gridControl;

	public TileView TileView => tileView;

	public event KeyEventHandler TileViewKeyDown;

	public event FocusedRowChangedEventHandler FocusedRowChanged;

	public RecentListUserControl()
	{
		InitializeComponent();
		tileView.KeyDown += TileView_KeyDown;
		tileView.FocusedRowChanged += TileView_FocusedRowChanged;
	}

	public void SetParameters()
	{
		string a = JsonConvert.SerializeObject(lastLicenses);
		string b = JsonConvert.SerializeObject(gridControl.DataSource);
		if (!string.Equals(a, b, StringComparison.OrdinalIgnoreCase) || gridControl.DataSource == null)
		{
			lastFocusedRowHandle = 0;
			lastLicenses = gridControl.DataSource;
		}
		tileView.FocusedRowHandle = lastFocusedRowHandle;
	}

	public void SetLastFocusedRowHandle()
	{
		lastFocusedRowHandle = tileView.FocusedRowHandle;
	}

	private void TileView_KeyDown(object sender, KeyEventArgs e)
	{
		this.TileViewKeyDown?.Invoke(sender, e);
	}

	private void TileView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		this.FocusedRowChanged?.Invoke(sender, e);
	}

	private void GridControl_Load(object sender, EventArgs e)
	{
		if (!base.DesignMode && base.ParentForm != null)
		{
			tileView.Appearance.EmptySpace.BackColor = base.ParentForm.BackColor;
		}
	}

	private void TileView_ItemCustomize(object sender, TileViewItemCustomizeEventArgs e)
	{
		if (base.ParentForm != null)
		{
			e.Item.AppearanceItem.Normal.BackColor = base.ParentForm.BackColor;
		}
		e.Item.AppearanceItem.Normal.Font = new Font(e.Item.AppearanceItem.Normal.Font.FontFamily, 10f);
		e.Item.AppearanceItem.Focused.BackColor = SkinsManager.CurrentSkin.LoginFormFocusedItemBackColor;
		TileView tileView = sender as TileView;
		LicenseDataResult obj = tileView?.GetRow(e.RowHandle) as LicenseDataResult;
		if (obj == null || obj.IsValid)
		{
			FileLicenseData obj2 = tileView?.GetRow(e.RowHandle) as FileLicenseData;
			if (obj2 == null || obj2.IsValid)
			{
				if (base.ParentForm != null)
				{
					e.Item.AppearanceItem.Normal.ForeColor = base.ParentForm.ForeColor;
				}
				return;
			}
		}
		e.Item.AppearanceItem.Normal.ForeColor = SkinsManager.CurrentSkin.DisabledForeColor;
		e.Item.AppearanceItem.Focused.BackColor = e.Item.AppearanceItem.Normal.BackColor;
	}

	private void toolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		if (e.SelectedControl != gridControl)
		{
			return;
		}
		TileViewHitInfo tileViewHitInfo = tileView.CalcHitInfo(e.ControlMousePosition);
		if (tileViewHitInfo.HitTest != TileControlHitTest.Item)
		{
			return;
		}
		foreach (TileViewElementInfo element in tileViewHitInfo.ItemInfo.Elements)
		{
			if (element.TextBounds.Contains(e.ControlMousePosition))
			{
				e.Info = new ToolTipControlInfo(element, element.Text);
				break;
			}
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
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition2 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition3 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement3 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		this.packageNameTileViewColumn = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.organizationTileViewColumn = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.expiresOnTileViewColumn = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.gridControl = new DevExpress.XtraGrid.GridControl();
		this.tileView = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.gridControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tileView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.packageNameTileViewColumn.Caption = "Package";
		this.packageNameTileViewColumn.FieldName = "PackageName";
		this.packageNameTileViewColumn.Name = "packageNameTileViewColumn";
		this.packageNameTileViewColumn.Visible = true;
		this.packageNameTileViewColumn.VisibleIndex = 0;
		this.organizationTileViewColumn.Caption = "Organization";
		this.organizationTileViewColumn.FieldName = "Organization";
		this.organizationTileViewColumn.Name = "organizationTileViewColumn";
		this.organizationTileViewColumn.Visible = true;
		this.organizationTileViewColumn.VisibleIndex = 1;
		this.expiresOnTileViewColumn.Caption = "Expires on";
		this.expiresOnTileViewColumn.FieldName = "ExpiresOn";
		this.expiresOnTileViewColumn.Name = "expiresOnTileViewColumn";
		this.expiresOnTileViewColumn.Visible = true;
		this.expiresOnTileViewColumn.VisibleIndex = 2;
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.gridControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(764, 508);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.gridControl.Location = new System.Drawing.Point(2, 2);
		this.gridControl.MainView = this.tileView;
		this.gridControl.Name = "gridControl";
		this.gridControl.Size = new System.Drawing.Size(760, 504);
		this.gridControl.TabIndex = 24;
		this.gridControl.ToolTipController = this.toolTipController;
		this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tileView });
		this.gridControl.Load += new System.EventHandler(GridControl_Load);
		this.tileView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.packageNameTileViewColumn, this.organizationTileViewColumn, this.expiresOnTileViewColumn });
		this.tileView.GridControl = this.gridControl;
		this.tileView.Name = "tileView";
		this.tileView.OptionsList.DrawItemSeparators = DevExpress.XtraGrid.Views.Tile.DrawItemSeparatorsMode.None;
		this.tileView.OptionsTiles.GroupTextPadding = new System.Windows.Forms.Padding(13, 8, 12, 8);
		this.tileView.OptionsTiles.IndentBetweenGroups = 0;
		this.tileView.OptionsTiles.IndentBetweenItems = 0;
		this.tileView.OptionsTiles.ItemPadding = new System.Windows.Forms.Padding(8, 4, 8, 4);
		this.tileView.OptionsTiles.ItemSize = new System.Drawing.Size(300, 40);
		this.tileView.OptionsTiles.LayoutMode = DevExpress.XtraGrid.Views.Tile.TileViewLayoutMode.List;
		this.tileView.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.tileView.OptionsTiles.Padding = new System.Windows.Forms.Padding(0);
		this.tileView.OptionsTiles.RowCount = 0;
		tableColumnDefinition.Length.Value = 105.0;
		tableColumnDefinition2.Length.Value = 114.0;
		tableColumnDefinition3.Length.Value = 65.0;
		this.tileView.TileColumns.Add(tableColumnDefinition);
		this.tileView.TileColumns.Add(tableColumnDefinition2);
		this.tileView.TileColumns.Add(tableColumnDefinition3);
		tableRowDefinition.Length.Type = DevExpress.XtraEditors.TableLayout.TableDefinitionLengthType.Pixel;
		tableRowDefinition.Length.Value = 32.0;
		this.tileView.TileRows.Add(tableRowDefinition);
		tileViewItemElement.Column = this.packageNameTileViewColumn;
		tileViewItemElement.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement.Text = "packageNameTileViewColumn";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement2.Column = this.organizationTileViewColumn;
		tileViewItemElement2.ColumnIndex = 1;
		tileViewItemElement2.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement2.Text = "organizationTileViewColumn";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleLeft;
		tileViewItemElement3.Column = this.expiresOnTileViewColumn;
		tileViewItemElement3.ColumnIndex = 2;
		tileViewItemElement3.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement3.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement3.Text = "expiresOnTileViewColumn";
		tileViewItemElement3.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleRight;
		this.tileView.TileTemplate.Add(tileViewItemElement);
		this.tileView.TileTemplate.Add(tileViewItemElement2);
		this.tileView.TileTemplate.Add(tileViewItemElement3);
		this.tileView.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(TileView_ItemCustomize);
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(toolTipController_GetActiveObjectInfo);
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.gridControlLayoutControlItem });
		this.mainLayoutControlGroup.Name = "mainLayoutControlGroup";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(764, 508);
		this.mainLayoutControlGroup.TextVisible = false;
		this.gridControlLayoutControlItem.Control = this.gridControl;
		this.gridControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.gridControlLayoutControlItem.Name = "gridControlLayoutControlItem";
		this.gridControlLayoutControlItem.Size = new System.Drawing.Size(764, 508);
		this.gridControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.gridControlLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Name = "RecentListUserControl";
		base.Size = new System.Drawing.Size(764, 508);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tileView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
