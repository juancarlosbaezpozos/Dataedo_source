using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Onboarding.Home.Model;
using Dataedo.App.Onboarding.Home.Support;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Onboarding.Home.Controls;

public class LinksWithImageUserControl : UserControl
{
	private List<LinkWithImageModel> links;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private LabelControl headerLabelControl;

	private LayoutControlItem headerLabelControlLayoutControlItem;

	private GridControl linksGridControl;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ToolTipController toolTipController;

	private LayoutControlItem linksGridControlLayoutControlItem;

	private EmptySpaceItem bottomEmptySpaceItem;

	private TileView linksTileView;

	private TileViewColumn linkGridColumn;

	private TileViewColumn imageGridColumn;

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public TablePanel TablePanel { get; set; }

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public string Title
	{
		get
		{
			return headerLabelControl.Text;
		}
		set
		{
			headerLabelControl.Text = value;
			headerLabelControlLayoutControlItem.Visibility = ((value == null) ? LayoutVisibility.Never : LayoutVisibility.Always);
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public event EventHandler<string> LinkClick;

	public LinksWithImageUserControl()
	{
		InitializeComponent();
		headerLabelControl.Appearance.Options.UseFont = true;
	}

	public void SetParameters(Size tileSize, Size imageSize, List<LinkWithImageModel> links)
	{
		linksTileView.OptionsTiles.ItemSize = tileSize;
		base.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
		int index = 0;
		linksTileView.TileTemplate[index].ImageOptions.ImageSize = imageSize;
		linksTileView.TileRows[index].Length.Value = imageSize.Height;
		this.links = links;
		linksTileView.BeginDataUpdate();
		linksGridControl.DataSource = this.links;
		linksTileView.EndDataUpdate();
		linksGridControl.ForceInitialize();
		UpdateSizes();
	}

	public void Redraw()
	{
		linksTileView.LayoutChanged();
	}

	private void LinksUserControl_Load(object sender, EventArgs e)
	{
		UpdateSizes();
	}

	private void linksGridControl_DataSourceChanged(object sender, EventArgs e)
	{
		UpdateSizes();
	}

	private void UpdateSizes()
	{
		List<LinkWithImageModel> list = links;
		if (list != null && list.Count > 0)
		{
			SuspendLayout();
			Size itemSize = linksTileView.OptionsTiles.ItemSize;
			int num = Math.Max(1, linksGridControl.Width / (itemSize.Width + linksTileView.OptionsTiles.Padding.Top));
			int num2 = links.Count / num + ((links.Count % num > 0) ? 1 : 0);
			int num3 = (itemSize.Height + linksTileView.OptionsTiles.Padding.Top) * num2 + linksTileView.OptionsTiles.Padding.Top + linksGridControlLayoutControlItem.Padding.Height + 15;
			Size size3 = (linksGridControlLayoutControlItem.MaxSize = (linksGridControlLayoutControlItem.MinSize = new Size(0, num3)));
			base.Height = linksGridControlLayoutControlItem.Height + headerLabelControlLayoutControlItem.Height + 16;
			ResumeLayout();
		}
	}

	private void linksGridControl_Click(object sender, EventArgs e)
	{
		LinkWithImageModel rowDataUnderCursor = ActionsSupport.GetRowDataUnderCursor<LinkWithImageModel>(linksTileView);
		if (!string.IsNullOrEmpty(rowDataUnderCursor?.Link))
		{
			this.LinkClick?.Invoke(sender, rowDataUnderCursor.Link);
		}
	}

	private void linksTileView_ItemCustomize(object sender, TileViewItemCustomizeEventArgs e)
	{
		e.Item.AllowHtmlText = DefaultBoolean.True;
		e.Item.AppearanceItem.Normal.BackColor = SkinColors.ControlColorFromSystemColors;
		e.Item.AppearanceItem.Normal.BorderColor = Color.Transparent;
		foreach (TileItemElement element in e.Item.Elements)
		{
			element.Text = element.Text ?? "";
		}
	}

	private void linksTileView_MouseMove(object sender, MouseEventArgs e)
	{
		if (sender is TileView tileView)
		{
			TileViewHitInfo tileViewHitInfo = tileView.CalcHitInfo(e.Location);
			if (tileViewHitInfo.HitTest == TileControlHitTest.Item && !string.IsNullOrEmpty((linksTileView.GetRow(tileViewHitInfo.RowHandle) as LinkWithImageModel)?.Link))
			{
				tileView.GridControl.Cursor = Cursors.Hand;
			}
			else
			{
				tileView.GridControl.Cursor = Cursors.Default;
			}
		}
	}

	private void LinksWithImageUserControl_Resize(object sender, EventArgs e)
	{
		UpdateSizes();
	}

	private void toolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		if (e.SelectedControl != linksGridControl)
		{
			return;
		}
		TileViewHitInfo tileViewHitInfo = linksTileView.CalcHitInfo(e.ControlMousePosition);
		if (tileViewHitInfo.HitTest != TileControlHitTest.Item)
		{
			return;
		}
		foreach (TileViewElementInfo element in tileViewHitInfo.ItemInfo.Elements)
		{
			if (element.ItemContentBounds.Contains(e.ControlMousePosition))
			{
				LinkWithImageModel linkWithImageModel = linksTileView.GetRow(tileViewHitInfo.RowHandle) as LinkWithImageModel;
				if (linkWithImageModel?.ToolTip != null)
				{
					e.Info = new ToolTipControlInfo(element, linkWithImageModel.ToolTip.Text, linkWithImageModel.ToolTip.Title);
				}
				break;
			}
		}
	}

	private void linksTileView_MouseWheel(object sender, MouseEventArgs e)
	{
		ScrollHandler.HandleMouseWheel(e, linksGridControl, TablePanel);
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
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition obj = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition tableColumnDefinition2 = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		this.linkGridColumn = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.imageGridColumn = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.linksGridControl = new DevExpress.XtraGrid.GridControl();
		this.linksTileView = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.headerLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.linksGridControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.linksGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.linksTileView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.linksGridControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.linkGridColumn.Caption = "Link";
		this.linkGridColumn.FieldName = "Text";
		this.linkGridColumn.MinWidth = 10;
		this.linkGridColumn.Name = "linkGridColumn";
		this.linkGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.linkGridColumn.OptionsFilter.AllowFilter = false;
		this.linkGridColumn.Visible = true;
		this.linkGridColumn.VisibleIndex = 0;
		this.linkGridColumn.Width = 10;
		this.imageGridColumn.Caption = "Image";
		this.imageGridColumn.FieldName = "Image";
		this.imageGridColumn.MinWidth = 10;
		this.imageGridColumn.Name = "imageGridColumn";
		this.imageGridColumn.Visible = true;
		this.imageGridColumn.VisibleIndex = 1;
		this.imageGridColumn.Width = 10;
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.headerLabelControl);
		this.mainLayoutControl.Controls.Add(this.linksGridControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(831, 435, 932, 570);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(556, 489);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.headerLabelControl.AllowHtmlString = true;
		this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 14f, System.Drawing.FontStyle.Bold);
		this.headerLabelControl.Appearance.Options.UseFont = true;
		this.headerLabelControl.Location = new System.Drawing.Point(2, 2);
		this.headerLabelControl.Name = "headerLabelControl";
		this.headerLabelControl.Size = new System.Drawing.Size(552, 24);
		this.headerLabelControl.StyleController = this.mainLayoutControl;
		this.headerLabelControl.TabIndex = 4;
		this.headerLabelControl.Text = "Links";
		this.linksGridControl.Location = new System.Drawing.Point(2, 36);
		this.linksGridControl.MainView = this.linksTileView;
		this.linksGridControl.MenuManager = this.barManager;
		this.linksGridControl.Name = "linksGridControl";
		this.linksGridControl.ShowOnlyPredefinedDetails = true;
		this.linksGridControl.Size = new System.Drawing.Size(552, 451);
		this.linksGridControl.TabIndex = 8;
		this.linksGridControl.ToolTipController = this.toolTipController;
		this.linksGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.linksTileView });
		this.linksGridControl.DataSourceChanged += new System.EventHandler(linksGridControl_DataSourceChanged);
		this.linksGridControl.Click += new System.EventHandler(linksGridControl_Click);
		this.linksTileView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.linksTileView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.linkGridColumn, this.imageGridColumn });
		this.linksTileView.DetailHeight = 320;
		this.linksTileView.GridControl = this.linksGridControl;
		this.linksTileView.Name = "linksTileView";
		this.linksTileView.OptionsBehavior.ReadOnly = true;
		this.linksTileView.OptionsTiles.HighlightFocusedTileStyle = DevExpress.XtraGrid.Views.Tile.HighlightFocusedTileStyle.None;
		this.linksTileView.OptionsTiles.HorizontalContentAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.linksTileView.OptionsTiles.ItemPadding = new System.Windows.Forms.Padding(1);
		this.linksTileView.OptionsTiles.ItemSize = new System.Drawing.Size(320, 320);
		this.linksTileView.OptionsTiles.Padding = new System.Windows.Forms.Padding(0, 10, 10, 10);
		this.linksTileView.OptionsTiles.RowCount = 0;
		this.linksTileView.OptionsTiles.VerticalContentAlignment = DevExpress.Utils.VertAlignment.Top;
		tableColumnDefinition.Length.Type = DevExpress.XtraEditors.TableLayout.TableDefinitionLengthType.Pixel;
		tableColumnDefinition.Length.Value = 0.0;
		tableColumnDefinition2.Length.Type = DevExpress.XtraEditors.TableLayout.TableDefinitionLengthType.Pixel;
		tableColumnDefinition2.Length.Value = 10.0;
		this.linksTileView.TileColumns.Add(tableColumnDefinition);
		this.linksTileView.TileColumns.Add(obj);
		this.linksTileView.TileColumns.Add(tableColumnDefinition2);
		tableRowDefinition.Length.Type = DevExpress.XtraEditors.TableLayout.TableDefinitionLengthType.Pixel;
		tableRowDefinition.Length.Value = 220.0;
		tableRowDefinition2.Length.Value = 10.0;
		tableRowDefinition2.PaddingBottom = 10;
		tableRowDefinition2.PaddingTop = 10;
		this.linksTileView.TileRows.Add(tableRowDefinition);
		this.linksTileView.TileRows.Add(tableRowDefinition2);
		tableSpan.ColumnSpan = 3;
		this.linksTileView.TileSpans.Add(tableSpan);
		tileViewItemElement.Appearance.Normal.Font = new System.Drawing.Font("Tahoma", 14f);
		tileViewItemElement.Appearance.Normal.Options.UseFont = true;
		tileViewItemElement.Column = this.linkGridColumn;
		tileViewItemElement.ColumnIndex = 1;
		tileViewItemElement.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement.RowIndex = 1;
		tileViewItemElement.Text = "linkGridColumn";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.TopLeft;
		tileViewItemElement2.Column = this.imageGridColumn;
		tileViewItemElement2.ImageOptions.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.ImageOptions.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement2.Text = "imageGridColumn";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.linksTileView.TileTemplate.Add(tileViewItemElement);
		this.linksTileView.TileTemplate.Add(tileViewItemElement2);
		this.linksTileView.ItemCustomize += new DevExpress.XtraGrid.Views.Tile.TileViewItemCustomizeEventHandler(linksTileView_ItemCustomize);
		this.linksTileView.MouseWheel += new System.Windows.Forms.MouseEventHandler(linksTileView_MouseWheel);
		this.linksTileView.MouseMove += new System.Windows.Forms.MouseEventHandler(linksTileView_MouseMove);
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(556, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 489);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(556, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 489);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(556, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 489);
		this.toolTipController.AllowHtmlText = true;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(toolTipController_GetActiveObjectInfo);
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.headerLabelControlLayoutControlItem, this.linksGridControlLayoutControlItem, this.bottomEmptySpaceItem });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(556, 489);
		this.mainLayoutControlGroup.TextVisible = false;
		this.headerLabelControlLayoutControlItem.Control = this.headerLabelControl;
		this.headerLabelControlLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.headerLabelControlLayoutControlItem.CustomizationFormText = "headerLabelControlLayoutControlItem";
		this.headerLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.headerLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 36);
		this.headerLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(40, 36);
		this.headerLabelControlLayoutControlItem.Name = "headerLabelControlLayoutControlItem";
		this.headerLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 10);
		this.headerLabelControlLayoutControlItem.Size = new System.Drawing.Size(556, 36);
		this.headerLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.headerLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.headerLabelControlLayoutControlItem.TextVisible = false;
		this.linksGridControlLayoutControlItem.Control = this.linksGridControl;
		this.linksGridControlLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.linksGridControlLayoutControlItem.CustomizationFormText = "linksGridControlLayoutControlItem";
		this.linksGridControlLayoutControlItem.Location = new System.Drawing.Point(0, 36);
		this.linksGridControlLayoutControlItem.MinSize = new System.Drawing.Size(104, 32);
		this.linksGridControlLayoutControlItem.Name = "linksGridControlLayoutControlItem";
		this.linksGridControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 0);
		this.linksGridControlLayoutControlItem.Size = new System.Drawing.Size(556, 451);
		this.linksGridControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.linksGridControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.linksGridControlLayoutControlItem.TextVisible = false;
		this.bottomEmptySpaceItem.AllowHotTrack = false;
		this.bottomEmptySpaceItem.Location = new System.Drawing.Point(0, 487);
		this.bottomEmptySpaceItem.MinSize = new System.Drawing.Size(1, 1);
		this.bottomEmptySpaceItem.Name = "bottomEmptySpaceItem";
		this.bottomEmptySpaceItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.bottomEmptySpaceItem.Size = new System.Drawing.Size(556, 2);
		this.bottomEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.bottomEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		this.DoubleBuffered = true;
		base.Name = "LinksWithImageUserControl";
		base.Size = new System.Drawing.Size(556, 489);
		base.Load += new System.EventHandler(LinksUserControl_Load);
		base.Resize += new System.EventHandler(LinksWithImageUserControl_Resize);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.linksGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.linksTileView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.linksGridControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
