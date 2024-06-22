using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Onboarding.Home.Model;
using Dataedo.App.Onboarding.Home.Support;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.Utils.Layout;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Onboarding.Home.Controls;

public class LinksUserControl : UserControl
{
	private List<LinkModel> links;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private LabelControl headerLabelControl;

	private LayoutControlItem headerLabelControlLayoutControlItem;

	private GridControl linksGridControl;

	private CustomGridUserControl linksGridView;

	private GridColumn linkGridColumn;

	private RepositoryItemHyperLinkEdit repositoryItemHyperLinkEdit;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ToolTipController toolTipController;

	private LayoutControlItem linksGridControlLayoutControlItem;

	private EmptySpaceItem bottomEmptySpaceItem;

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

	public LinksUserControl()
	{
		InitializeComponent();
		headerLabelControl.Appearance.Options.UseFont = true;
		linksGridView.Appearance.Row.BackColor = SkinColors.ControlColorFromSystemColors;
		linksGridView.Appearance.Empty.BackColor = SkinColors.ControlColorFromSystemColors;
	}

	public void SetParameters(List<LinkModel> links)
	{
		this.links = links;
		base.Padding = new System.Windows.Forms.Padding(0, 0, 0, 0);
		linksGridView.BeginDataUpdate();
		linksGridControl.DataSource = this.links;
		linksGridView.EndDataUpdate();
		linksGridControl.ForceInitialize();
		UpdateSizes();
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
		List<LinkModel> list = links;
		if (list != null && list.Count > 0)
		{
			SuspendLayout();
			LayoutSupport.SetSizes(this, linksGridControlLayoutControlItem, linksGridView, links?.Count ?? 0, 10, new LayoutControlItem[1] { headerLabelControlLayoutControlItem });
			ResumeLayout();
		}
	}

	private void repositoryItemHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		LinkModel rowDataUnderCursor = ActionsSupport.GetRowDataUnderCursor<LinkModel>(linksGridView);
		if (!string.IsNullOrEmpty(rowDataUnderCursor?.Link))
		{
			this.LinkClick?.Invoke(sender, rowDataUnderCursor.Link);
		}
	}

	private void toolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		ActionsSupport.GetActiveObjectInfo<LinkModel>(linksGridControl, linksGridView, linkGridColumn, e);
	}

	private void linksGridView_MouseWheel(object sender, MouseEventArgs e)
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
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.linksGridControl = new DevExpress.XtraGrid.GridControl();
		this.linksGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.linkGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemHyperLinkEdit = new DevExpress.XtraEditors.Repository.RepositoryItemHyperLinkEdit();
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
		((System.ComponentModel.ISupportInitialize)this.linksGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHyperLinkEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.linksGridControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.headerLabelControl);
		this.mainLayoutControl.Controls.Add(this.linksGridControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2890, 620, 932, 570);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(556, 266);
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
		this.linksGridControl.MainView = this.linksGridView;
		this.linksGridControl.MenuManager = this.barManager;
		this.linksGridControl.Name = "linksGridControl";
		this.linksGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemHyperLinkEdit });
		this.linksGridControl.ShowOnlyPredefinedDetails = true;
		this.linksGridControl.Size = new System.Drawing.Size(552, 229);
		this.linksGridControl.TabIndex = 8;
		this.linksGridControl.ToolTipController = this.toolTipController;
		this.linksGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.linksGridView });
		this.linksGridControl.DataSourceChanged += new System.EventHandler(linksGridControl_DataSourceChanged);
		this.linksGridView.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.linksGridView.Appearance.Row.Options.UseFont = true;
		this.linksGridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.linksGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.linkGridColumn });
		this.linksGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
		this.linksGridView.GridControl = this.linksGridControl;
		this.linksGridView.Name = "linksGridView";
		this.linksGridView.OptionsBehavior.ReadOnly = true;
		this.linksGridView.OptionsFilter.AllowFilterEditor = false;
		this.linksGridView.OptionsNavigation.AutoMoveRowFocus = false;
		this.linksGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.linksGridView.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.linksGridView.OptionsSelection.EnableAppearanceHideSelection = false;
		this.linksGridView.OptionsView.ColumnAutoWidth = false;
		this.linksGridView.OptionsView.ShowColumnHeaders = false;
		this.linksGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.linksGridView.OptionsView.ShowGroupPanel = false;
		this.linksGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.linksGridView.OptionsView.ShowIndicator = false;
		this.linksGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.linksGridView.RowHighlightingIsEnabled = false;
		this.linksGridView.MouseWheel += new System.Windows.Forms.MouseEventHandler(linksGridView_MouseWheel);
		this.linkGridColumn.Caption = "Link";
		this.linkGridColumn.ColumnEdit = this.repositoryItemHyperLinkEdit;
		this.linkGridColumn.FieldName = "Text";
		this.linkGridColumn.MinWidth = 50;
		this.linkGridColumn.Name = "linkGridColumn";
		this.linkGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.linkGridColumn.OptionsFilter.AllowFilter = false;
		this.linkGridColumn.Visible = true;
		this.linkGridColumn.VisibleIndex = 0;
		this.linkGridColumn.Width = 50;
		this.repositoryItemHyperLinkEdit.AutoHeight = false;
		this.repositoryItemHyperLinkEdit.Name = "repositoryItemHyperLinkEdit";
		this.repositoryItemHyperLinkEdit.Padding = new System.Windows.Forms.Padding(-2, 0, 0, 0);
		this.repositoryItemHyperLinkEdit.SingleClick = true;
		this.repositoryItemHyperLinkEdit.StartLinkOnClickingEmptySpace = false;
		this.repositoryItemHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(repositoryItemHyperLinkEdit_OpenLink);
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
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 266);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(556, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 266);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(556, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 266);
		this.toolTipController.AllowHtmlText = true;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(toolTipController_GetActiveObjectInfo);
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.headerLabelControlLayoutControlItem, this.linksGridControlLayoutControlItem, this.bottomEmptySpaceItem });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(556, 266);
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
		this.linksGridControlLayoutControlItem.MinSize = new System.Drawing.Size(50, 1);
		this.linksGridControlLayoutControlItem.Name = "linksGridControlLayoutControlItem";
		this.linksGridControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 0, 0);
		this.linksGridControlLayoutControlItem.Size = new System.Drawing.Size(556, 229);
		this.linksGridControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.linksGridControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.linksGridControlLayoutControlItem.TextVisible = false;
		this.bottomEmptySpaceItem.AllowHotTrack = false;
		this.bottomEmptySpaceItem.Location = new System.Drawing.Point(0, 265);
		this.bottomEmptySpaceItem.MinSize = new System.Drawing.Size(104, 1);
		this.bottomEmptySpaceItem.Name = "bottomEmptySpaceItem";
		this.bottomEmptySpaceItem.Size = new System.Drawing.Size(556, 1);
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
		base.Name = "LinksUserControl";
		base.Size = new System.Drawing.Size(556, 266);
		base.Load += new System.EventHandler(LinksUserControl_Load);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.linksGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.linksGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHyperLinkEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.linksGridControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
