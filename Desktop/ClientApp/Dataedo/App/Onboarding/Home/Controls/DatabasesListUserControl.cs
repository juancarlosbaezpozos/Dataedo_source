using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.MenuTree;
using Dataedo.App.Onboarding.Home.Support;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Model.Data.Documentations;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Behaviors;
using DevExpress.Utils.Layout;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.Onboarding.Home.Controls;

public class DatabasesListUserControl : UserControl
{
	private const string ArrowDown = "↓";

	private const string ArrowUp = "↑";

	public static readonly CultureInfo ObjectsCountCulture = CultureInfo.GetCultureInfo("en-US");

	private List<DocumentationWithObjectsCountObject> documentations;

	private MetadataEditorUserControl metadataEditorUserControl;

	private TablePanel tablePanel;

	private int rowsCount;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private LabelControl headerLabelControl;

	private LayoutControlItem headerLabelControlLayoutControlItem;

	private SimpleButton exportDocumentationButtonSimpleButton;

	private LayoutControlItem exportDocumentationButtonSimpleButtonLayoutControlItem;

	private GridControl databasesGridControl;

	private CustomGridUserControl databasesGridView;

	private GridColumn dataSourceGridColumn;

	private GridColumn objectCountGridColumn;

	private GridColumn exportGridColumn;

	private LayoutControlItem databasesGridControlLayoutControlItem;

	private RepositoryItemHyperLinkEdit exportRepositoryItemHyperLinkEdit;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ToolTipController toolTipController;

	private EmptySpaceItem bottomEmptySpaceItem;

	private GridColumn lastImportedGridColumn;

	private GridColumn importGridColumn;

	private BehaviorManager behaviorManager1;

	private RepositoryItemHyperLinkEdit importRepositoryItemHyperLinkEdit;

	private SimpleButton expandSimpleButton;

	private LayoutControlItem expandSimpleButtonLayoutControlItem;

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
	public event EventHandler ExportDocumentationsButtonClick
	{
		add
		{
			exportDocumentationButtonSimpleButton.Click += value;
		}
		remove
		{
			exportDocumentationButtonSimpleButton.Click -= value;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public event EventHandler<int> ExportDocumentationButtonClick;

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public event EventHandler<int> ImportChangesButtonClick;

	public DatabasesListUserControl(MetadataEditorUserControl metadataEditorUserControl, TablePanel tablePanel)
	{
		InitializeComponent();
		headerLabelControl.Appearance.Options.UseFont = true;
		databasesGridView.Appearance.Row.BackColor = SkinColors.ControlColorFromSystemColors;
		databasesGridView.Appearance.Empty.BackColor = SkinColors.ControlColorFromSystemColors;
		databasesGridView.Appearance.SelectedRow.BackColor = SkinsManager.CurrentSkin.GridSelectionBackColor;
		databasesGridView.Appearance.SelectedRow.Options.UseBackColor = true;
		this.metadataEditorUserControl = metadataEditorUserControl;
		this.tablePanel = tablePanel;
		UnfocusRowOnStart();
		databasesGridControl.MouseWheel += DatabasesGridView_MouseWheel;
		SetObjectCountColumnFormat();
	}

	private void SetObjectCountColumnFormat()
	{
		((ISupportInitialize)databasesGridControl).BeginInit();
		objectCountGridColumn.DisplayFormat.Format = ObjectsCountCulture;
		objectCountGridColumn.DisplayFormat.FormatString = "N0";
		objectCountGridColumn.DisplayFormat.FormatType = FormatType.Numeric;
		((ISupportInitialize)databasesGridControl).EndInit();
	}

	private void DatabasesGridView_MouseWheel(object sender, MouseEventArgs e)
	{
		ScrollHandler.HandleMouseWheel(e, databasesGridControl, tablePanel);
	}

	private void UnfocusRowOnStart()
	{
		CustomGridUserControl customGridUserControl = databasesGridView;
		customGridUserControl.BeginUpdate();
		bool showAutoFilterRow = customGridUserControl.OptionsView.ShowAutoFilterRow;
		if (!showAutoFilterRow)
		{
			customGridUserControl.OptionsView.ShowAutoFilterRow = true;
		}
		customGridUserControl.FocusedRowHandle = -2147483646;
		if (!showAutoFilterRow)
		{
			customGridUserControl.OptionsView.ShowAutoFilterRow = false;
		}
		customGridUserControl.EndUpdate();
	}

	public void SetParameters(int? rowsCount)
	{
		documentations = DB.Database.GetDocumentationsWithObjectsCount();
		IEnumerable<DatabaseWithCountModel> dataSource = documentations.Select((DocumentationWithObjectsCountObject x) => new DatabaseWithCountModel(x.DatabaseId, x.Class, x.Title, x.Count, x.LastImportDate));
		this.rowsCount = rowsCount ?? int.MaxValue;
		databasesGridView.BeginDataUpdate();
		databasesGridControl.DataSource = dataSource;
		databasesGridView.EndDataUpdate();
		databasesGridControl.ForceInitialize();
		UpdateSizes();
	}

	public void RefreshDatabases()
	{
		SetParameters(rowsCount);
	}

	private void DatabasesListUserControl_Load(object sender, EventArgs e)
	{
		UpdateSizes();
	}

	private void DatabasesGridControl_DataSourceChanged(object sender, EventArgs e)
	{
		UpdateSizes();
	}

	private void UpdateSizes()
	{
		List<DocumentationWithObjectsCountObject> list = documentations;
		if (list != null && list.Count > 0)
		{
			bool databasesGridIsExpanded = expandSimpleButton.Text == "↑";
			LayoutControlItem[] otherControls = new LayoutControlItem[2] { headerLabelControlLayoutControlItem, exportDocumentationButtonSimpleButtonLayoutControlItem };
			LayoutSupport.SetSizes(this, databasesGridControlLayoutControlItem, databasesGridView, documentations?.Count ?? 0, rowsCount, otherControls, databasesGridIsExpanded);
		}
	}

	private void ExpandSimpleButton_Click(object sender, EventArgs e)
	{
		if (expandSimpleButton.Text == "↓")
		{
			expandSimpleButton.Text = "↑";
		}
		else if (expandSimpleButton.Text == "↑")
		{
			expandSimpleButton.Text = "↓";
		}
		UpdateSizes();
	}

	private void DatabasesGridView_RowCellClick(object sender, RowCellClickEventArgs e)
	{
		GridHitInfo gridHitInfo = databasesGridView.CalcHitInfo(databasesGridView.GridControl.PointToClient(Control.MousePosition));
		if (!gridHitInfo.InRowCell)
		{
			return;
		}
		int rowHandle = gridHitInfo.RowHandle;
		if (e.Clicks > 1)
		{
			DatabaseWithCountModel data = databasesGridView.GetRow(rowHandle) as DatabaseWithCountModel;
			TreeListNode treeListNode = metadataEditorUserControl?.MetadataTreeList?.Nodes?[0].Nodes?.Where((TreeListNode x) => ((DBTreeNode)metadataEditorUserControl.MetadataTreeList.GetDataRecordByNode(x)).Id == data.DatabaseId).FirstOrDefault();
			if (treeListNode != null)
			{
				metadataEditorUserControl.SelectDBTreeNode(treeListNode);
			}
		}
		else if (gridHitInfo.Column.FieldName == "Import")
		{
			if (databasesGridView.GetRow(rowHandle) is DatabaseWithCountModel databaseWithCountModel)
			{
				this.ImportChangesButtonClick?.Invoke(sender, databaseWithCountModel.DatabaseId);
			}
		}
		else if (gridHitInfo.Column.FieldName == "Export" && databasesGridView.GetRow(rowHandle) is DatabaseWithCountModel databaseWithCountModel2)
		{
			this.ExportDocumentationButtonClick?.Invoke(sender, databaseWithCountModel2.DatabaseId);
		}
	}

	private void DatabasesGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (sender is GridView gridView)
		{
			int[] selectedRows = gridView.GetSelectedRows();
			int focusedRowHandle = gridView.FocusedRowHandle;
			if (selectedRows.Length != 0 && e.RowHandle == focusedRowHandle)
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.GridHighlightRowBackColor;
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
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.expandSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.databasesGridControl = new DevExpress.XtraGrid.GridControl();
		this.databasesGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.dataSourceGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.objectCountGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastImportedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.importGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.importRepositoryItemHyperLinkEdit = new DevExpress.XtraEditors.Repository.RepositoryItemHyperLinkEdit();
		this.exportGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.exportRepositoryItemHyperLinkEdit = new DevExpress.XtraEditors.Repository.RepositoryItemHyperLinkEdit();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.exportDocumentationButtonSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.headerLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.exportDocumentationButtonSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.databasesGridControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.expandSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.databasesGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databasesGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.importRepositoryItemHyperLinkEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.exportRepositoryItemHyperLinkEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.exportDocumentationButtonSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databasesGridControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.expandSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.behaviorManager1).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.expandSimpleButton);
		this.mainLayoutControl.Controls.Add(this.databasesGridControl);
		this.mainLayoutControl.Controls.Add(this.exportDocumentationButtonSimpleButton);
		this.mainLayoutControl.Controls.Add(this.headerLabelControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.mainLayoutControl.MenuManager = this.barManager;
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2788, 555, 970, 449);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(1540, 232);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.expandSimpleButton.Location = new System.Drawing.Point(159, 205);
		this.expandSimpleButton.MaximumSize = new System.Drawing.Size(30, 0);
		this.expandSimpleButton.MinimumSize = new System.Drawing.Size(30, 0);
		this.expandSimpleButton.Name = "expandSimpleButton";
		this.expandSimpleButton.Size = new System.Drawing.Size(30, 24);
		this.expandSimpleButton.StyleController = this.mainLayoutControl;
		this.expandSimpleButton.TabIndex = 9;
		this.expandSimpleButton.Text = "↓";
		this.expandSimpleButton.Click += new System.EventHandler(ExpandSimpleButton_Click);
		this.databasesGridControl.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.databasesGridControl.Location = new System.Drawing.Point(2, 38);
		this.databasesGridControl.MainView = this.databasesGridView;
		this.databasesGridControl.MenuManager = this.barManager;
		this.databasesGridControl.Name = "databasesGridControl";
		this.databasesGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.exportRepositoryItemHyperLinkEdit, this.importRepositoryItemHyperLinkEdit });
		this.databasesGridControl.ShowOnlyPredefinedDetails = true;
		this.databasesGridControl.Size = new System.Drawing.Size(1536, 155);
		this.databasesGridControl.TabIndex = 8;
		this.databasesGridControl.ToolTipController = this.toolTipController;
		this.databasesGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.databasesGridView });
		this.databasesGridControl.DataSourceChanged += new System.EventHandler(DatabasesGridControl_DataSourceChanged);
		this.databasesGridView.Appearance.Row.Font = new System.Drawing.Font("Tahoma", 10f);
		this.databasesGridView.Appearance.Row.Options.UseFont = true;
		this.databasesGridView.Appearance.SelectedRow.Options.UseBackColor = true;
		this.databasesGridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.UltraFlat;
		this.databasesGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.dataSourceGridColumn, this.objectCountGridColumn, this.lastImportedGridColumn, this.importGridColumn, this.exportGridColumn });
		this.databasesGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.databasesGridView.GridControl = this.databasesGridControl;
		this.databasesGridView.Name = "databasesGridView";
		this.databasesGridView.OptionsBehavior.ReadOnly = true;
		this.databasesGridView.OptionsFilter.AllowFilterEditor = false;
		this.databasesGridView.OptionsSelection.MultiSelect = true;
		this.databasesGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.databasesGridView.OptionsView.ColumnAutoWidth = false;
		this.databasesGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.databasesGridView.OptionsView.ShowGroupPanel = false;
		this.databasesGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.databasesGridView.OptionsView.ShowIndicator = false;
		this.databasesGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.databasesGridView.RowHighlightingIsEnabled = false;
		this.databasesGridView.RowCellClick += new DevExpress.XtraGrid.Views.Grid.RowCellClickEventHandler(DatabasesGridView_RowCellClick);
		this.databasesGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(DatabasesGridView_CustomDrawCell);
		this.dataSourceGridColumn.Caption = "Data source";
		this.dataSourceGridColumn.FieldName = "Title";
		this.dataSourceGridColumn.MinWidth = 100;
		this.dataSourceGridColumn.Name = "dataSourceGridColumn";
		this.dataSourceGridColumn.OptionsColumn.AllowEdit = false;
		this.dataSourceGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.dataSourceGridColumn.OptionsFilter.AllowFilter = false;
		this.dataSourceGridColumn.Visible = true;
		this.dataSourceGridColumn.VisibleIndex = 0;
		this.dataSourceGridColumn.Width = 150;
		this.objectCountGridColumn.Caption = "Objects";
		this.objectCountGridColumn.FieldName = "Count";
		this.objectCountGridColumn.MinWidth = 80;
		this.objectCountGridColumn.Name = "objectCountGridColumn";
		this.objectCountGridColumn.OptionsColumn.AllowEdit = false;
		this.objectCountGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.objectCountGridColumn.OptionsFilter.AllowFilter = false;
		this.objectCountGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
		this.objectCountGridColumn.Visible = true;
		this.objectCountGridColumn.VisibleIndex = 1;
		this.objectCountGridColumn.Width = 80;
		this.lastImportedGridColumn.Caption = "Last imported";
		this.lastImportedGridColumn.FieldName = "LastImported";
		this.lastImportedGridColumn.MinWidth = 100;
		this.lastImportedGridColumn.Name = "lastImportedGridColumn";
		this.lastImportedGridColumn.OptionsColumn.AllowEdit = false;
		this.lastImportedGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.lastImportedGridColumn.OptionsFilter.AllowFilter = false;
		this.lastImportedGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.DateTime;
		this.lastImportedGridColumn.Visible = true;
		this.lastImportedGridColumn.VisibleIndex = 2;
		this.lastImportedGridColumn.Width = 100;
		this.importGridColumn.ColumnEdit = this.importRepositoryItemHyperLinkEdit;
		this.importGridColumn.FieldName = "Import";
		this.importGridColumn.MaxWidth = 50;
		this.importGridColumn.MinWidth = 50;
		this.importGridColumn.Name = "importGridColumn";
		this.importGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.importGridColumn.OptionsFilter.AllowFilter = false;
		this.importGridColumn.Visible = true;
		this.importGridColumn.VisibleIndex = 3;
		this.importGridColumn.Width = 50;
		this.importRepositoryItemHyperLinkEdit.AllowFocused = false;
		this.importRepositoryItemHyperLinkEdit.AutoHeight = false;
		this.importRepositoryItemHyperLinkEdit.Name = "importRepositoryItemHyperLinkEdit";
		this.importRepositoryItemHyperLinkEdit.SingleClick = true;
		this.importRepositoryItemHyperLinkEdit.StartLinkOnClickingEmptySpace = false;
		this.exportGridColumn.ColumnEdit = this.exportRepositoryItemHyperLinkEdit;
		this.exportGridColumn.FieldName = "Export";
		this.exportGridColumn.MaxWidth = 50;
		this.exportGridColumn.MinWidth = 50;
		this.exportGridColumn.Name = "exportGridColumn";
		this.exportGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.exportGridColumn.OptionsFilter.AllowFilter = false;
		this.exportGridColumn.Visible = true;
		this.exportGridColumn.VisibleIndex = 4;
		this.exportGridColumn.Width = 50;
		this.exportRepositoryItemHyperLinkEdit.AllowFocused = false;
		this.exportRepositoryItemHyperLinkEdit.AutoHeight = false;
		this.exportRepositoryItemHyperLinkEdit.Name = "exportRepositoryItemHyperLinkEdit";
		this.exportRepositoryItemHyperLinkEdit.SingleClick = true;
		this.exportRepositoryItemHyperLinkEdit.StartLinkOnClickingEmptySpace = false;
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(1540, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 232);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(1540, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 232);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1540, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 232);
		this.toolTipController.AllowHtmlText = true;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.exportDocumentationButtonSimpleButton.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.exportDocumentationButtonSimpleButton.Appearance.Options.UseFont = true;
		this.exportDocumentationButtonSimpleButton.Location = new System.Drawing.Point(2, 205);
		this.exportDocumentationButtonSimpleButton.MaximumSize = new System.Drawing.Size(150, 0);
		this.exportDocumentationButtonSimpleButton.Name = "exportDocumentationButtonSimpleButton";
		this.exportDocumentationButtonSimpleButton.Size = new System.Drawing.Size(150, 24);
		this.exportDocumentationButtonSimpleButton.StyleController = this.mainLayoutControl;
		this.exportDocumentationButtonSimpleButton.TabIndex = 6;
		this.exportDocumentationButtonSimpleButton.Text = "Export";
		this.headerLabelControl.AllowHtmlString = true;
		this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 14f, System.Drawing.FontStyle.Bold);
		this.headerLabelControl.Appearance.Options.UseFont = true;
		this.headerLabelControl.Location = new System.Drawing.Point(2, 2);
		this.headerLabelControl.Name = "headerLabelControl";
		this.headerLabelControl.Size = new System.Drawing.Size(1536, 24);
		this.headerLabelControl.StyleController = this.mainLayoutControl;
		this.headerLabelControl.TabIndex = 4;
		this.headerLabelControl.Text = "Your Catalog";
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.headerLabelControlLayoutControlItem, this.exportDocumentationButtonSimpleButtonLayoutControlItem, this.databasesGridControlLayoutControlItem, this.bottomEmptySpaceItem, this.expandSimpleButtonLayoutControlItem });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(1540, 232);
		this.mainLayoutControlGroup.TextVisible = false;
		this.headerLabelControlLayoutControlItem.Control = this.headerLabelControl;
		this.headerLabelControlLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.headerLabelControlLayoutControlItem.CustomizationFormText = "headerLabelControlLayoutControlItem";
		this.headerLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.headerLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 36);
		this.headerLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(1, 36);
		this.headerLabelControlLayoutControlItem.Name = "headerLabelControlLayoutControlItem";
		this.headerLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 10);
		this.headerLabelControlLayoutControlItem.Size = new System.Drawing.Size(1540, 36);
		this.headerLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.headerLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.headerLabelControlLayoutControlItem.TextVisible = false;
		this.exportDocumentationButtonSimpleButtonLayoutControlItem.Control = this.exportDocumentationButtonSimpleButton;
		this.exportDocumentationButtonSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(0, 195);
		this.exportDocumentationButtonSimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(154, 36);
		this.exportDocumentationButtonSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(117, 36);
		this.exportDocumentationButtonSimpleButtonLayoutControlItem.Name = "exportDocumentationButtonSimpleButtonLayoutControlItem";
		this.exportDocumentationButtonSimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 10, 2);
		this.exportDocumentationButtonSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(154, 36);
		this.exportDocumentationButtonSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.exportDocumentationButtonSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.exportDocumentationButtonSimpleButtonLayoutControlItem.TextVisible = false;
		this.databasesGridControlLayoutControlItem.Control = this.databasesGridControl;
		this.databasesGridControlLayoutControlItem.Location = new System.Drawing.Point(0, 36);
		this.databasesGridControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 159);
		this.databasesGridControlLayoutControlItem.MinSize = new System.Drawing.Size(104, 159);
		this.databasesGridControlLayoutControlItem.Name = "databasesGridControlLayoutControlItem";
		this.databasesGridControlLayoutControlItem.Size = new System.Drawing.Size(1540, 159);
		this.databasesGridControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.databasesGridControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.databasesGridControlLayoutControlItem.TextVisible = false;
		this.bottomEmptySpaceItem.AllowHotTrack = false;
		this.bottomEmptySpaceItem.Location = new System.Drawing.Point(0, 231);
		this.bottomEmptySpaceItem.MinSize = new System.Drawing.Size(104, 1);
		this.bottomEmptySpaceItem.Name = "bottomEmptySpaceItem";
		this.bottomEmptySpaceItem.Size = new System.Drawing.Size(1540, 1);
		this.bottomEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.bottomEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.expandSimpleButtonLayoutControlItem.Control = this.expandSimpleButton;
		this.expandSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(154, 195);
		this.expandSimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(37, 36);
		this.expandSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(37, 36);
		this.expandSimpleButtonLayoutControlItem.Name = "expandSimpleButtonLayoutControlItem";
		this.expandSimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 2, 10, 2);
		this.expandSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(1386, 36);
		this.expandSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.expandSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.expandSimpleButtonLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		this.DoubleBuffered = true;
		base.Name = "DatabasesListUserControl";
		base.Size = new System.Drawing.Size(1540, 232);
		base.Load += new System.EventHandler(DatabasesListUserControl_Load);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.databasesGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databasesGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.importRepositoryItemHyperLinkEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.exportRepositoryItemHyperLinkEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.headerLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.exportDocumentationButtonSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databasesGridControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.expandSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.behaviorManager1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
