using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.CommonFunctionsForPanels;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.History;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Helpers;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Columns;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.UserControls.SummaryControls;

public class ProcedureSummaryUserControl : BaseSummaryUserControl
{
	private const int AddNewModuleBarButtonIndex = 4;

	private SharedObjectTypeEnum.ObjectType listObjectsType;

	private int databaseId;

	private DBTreeNode folderNode;

	private IContainer components;

	private LabelControl allDatabaseProceduresLabel;

	private GridControl allDatabaseProceduresGrid;

	private GridColumn schemaTableGridColumn;

	private GridColumn nameTableGridColumn;

	private GridColumn titleTableGridColumn;

	private RepositoryItemLookUpEdit typeRepositoryItemLookUpEdit;

	private GridColumn moduleTableGridColumn;

	private RepositoryItemLookUpEdit moduleRepositoryItemLookUpEdit;

	private GridColumn iconTableGridColumn;

	private RepositoryItemPictureEdit iconRepositoryItemPictureEdit;

	private RepositoryItemCheckedComboBoxEdit moduleRepositoryItemCheckedComboBoxEdit;

	private GridColumn dbmsLastModificationDateTableGridColumn;

	private GridColumn synchronizationDateTableGridColumn;

	private ToolTipController proceduresToolTipController;

	private RepositoryItemCustomTextEdit titleRepositoryItemCustomTextEdit;

	private GridColumn documentationGridColumn;

	private BulkCopyGridUserControl allDatabaseProceduresGridView;

	private GridColumn typeTableGridColumn;

	private GridPanelUserControl gridPanelUserControl;

	private GridColumn dbmsCreatedGridColumn;

	private GridColumn createdGridColumn;

	private GridColumn createdByGridColumn;

	private GridColumn lastImportedByGidColumn;

	private GridColumn lastUpdatedGridColumn;

	private GridColumn lastUpdatedByGridColumn;

	private PopupMenu allProceduresPopupMenu;

	private BarManager allProceduresBarManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarButtonItem addToNewModuleBarButtonItem;

	private BarButtonItem assignModuleBarButtonItem;

	private BarButtonItem deleteBarButtonItem;

	private SplashScreenManager splashScreenManager;

	private BarButtonItem designBarButtonItem;

	private BarButtonItem addProcedureBarButtonItem;

	private BarButtonItem viewHistoryBarButtonItem;

	public List<ObjectWithModulesObject> DataSource { get; private set; }

	public event EventHandler ShowProcedureControl;

	public ProcedureSummaryUserControl(SharedObjectTypeEnum.ObjectType listObjectsType)
	{
		InitializeComponent();
		this.listObjectsType = listObjectsType;
		base.BulkCopy = new SummaryBulkCopy(this.listObjectsType, this);
		allDatabaseProceduresGridView.Copy = base.BulkCopy;
		allDatabaseProceduresGridView.SplashScreenManager = splashScreenManager;
		MetadataToolTip.SetColumnToolTip(dbmsCreatedGridColumn, "dbms_created");
		MetadataToolTip.SetColumnToolTip(dbmsLastModificationDateTableGridColumn, "dbms_last_updated");
		MetadataToolTip.SetColumnToolTip(createdGridColumn, "first_imported");
		MetadataToolTip.SetColumnToolTip(createdByGridColumn, "first_imported_by");
		MetadataToolTip.SetColumnToolTip(synchronizationDateTableGridColumn, "last_imported");
		MetadataToolTip.SetColumnToolTip(lastImportedByGidColumn, "last_imported_by");
		MetadataToolTip.SetColumnToolTip(lastUpdatedGridColumn, "last_updated");
		MetadataToolTip.SetColumnToolTip(lastUpdatedByGridColumn, "last_updated_by");
		ObjectEventArgs = new ObjectEventArgs();
		LengthValidation.SetTitleOrNameLengthLimit(titleRepositoryItemCustomTextEdit);
		gridPanelUserControl.InsertAdditionalButton(addProcedureBarButtonItem, 4);
	}

	public override void SetTooltips()
	{
		typeTableGridColumn.ToolTip = CommonTooltips.GetType(base.Node.ContainedObjectsObjectType);
		schemaTableGridColumn.ToolTip = CommonTooltips.GetSchema(base.Node.ContainedObjectsObjectType);
		nameTableGridColumn.ToolTip = CommonTooltips.GetName(base.Node.ContainedObjectsObjectType);
		titleTableGridColumn.ToolTip = CommonTooltips.GetTitle(base.Node.ContainedObjectsObjectType);
		moduleTableGridColumn.ToolTip = CommonTooltips.GetModule(base.Node.ContainedObjectsObjectType);
	}

	private void ProcedureSummaryUserControl_Load(object sender, EventArgs e)
	{
		AddEvents();
		WorkWithDataedoTrackingHelper.TrackFirstInSessionObjectsListView();
	}

	public override void RefreshData()
	{
		allDatabaseProceduresGridView.RefreshData();
	}

	protected override void AddEvents()
	{
		CommonFunctionsPanels.AddEventsForSummaryTable(allDatabaseProceduresGridView, listObjectsType, allProceduresPopupMenu, allProceduresBarManager, this.ShowProcedureControl, iconTableGridColumn, deleteBarButtonItem, ObjectEventArgs, base.TreeMenu, moduleRepositoryItemCheckedComboBoxEdit, this, gridPanelUserControl, null, null, null, null, null, FindForm());
		base.DragRows = new DragRowsBase<ObjectWithModulesObject>(allDatabaseProceduresGridView);
		base.DragRows.AddEvents();
		addToNewModuleBarButtonItem.ItemClick += delegate
		{
			AddToNewModule();
		};
		List<ToolTipData> list = new List<ToolTipData>();
		list.Add(new ToolTipData(allDatabaseProceduresGrid, listObjectsType, iconTableGridColumn.VisibleIndex));
		CommonFunctionsPanels.AddEventsForToolTips(proceduresToolTipController, list);
		CommonFunctionsPanels.AddEventForAutoFilterRow(allDatabaseProceduresGridView);
		if (listObjectsType == SharedObjectTypeEnum.ObjectType.Procedure || listObjectsType == SharedObjectTypeEnum.ObjectType.Function)
		{
			designBarButtonItem.Caption = "Design " + SharedObjectTypeEnum.TypeToStringForSingle(listObjectsType);
			designBarButtonItem.ItemClick += delegate
			{
				CommonFunctionsDatabase.DesignProcedureOrFunction(allDatabaseProceduresGridView, DataSource, base.CustomFieldsSupport);
			};
			if (listObjectsType == SharedObjectTypeEnum.ObjectType.Procedure)
			{
				addProcedureBarButtonItem.Glyph = Resources.procedure_new_16;
			}
			else if (listObjectsType == SharedObjectTypeEnum.ObjectType.Function)
			{
				addProcedureBarButtonItem.Glyph = Resources.function_new_16;
			}
			addProcedureBarButtonItem.Hint = "Add " + SharedObjectTypeEnum.TypeToStringForSingle(listObjectsType);
			addProcedureBarButtonItem.ItemClick += delegate
			{
				CommonFunctionsDatabase.AddNewProcedure(allDatabaseProceduresGridView, databaseId, folderNode, base.CustomFieldsSupport);
			};
		}
		else
		{
			BarItemVisibility barItemVisibility3 = (addProcedureBarButtonItem.Visibility = (designBarButtonItem.Visibility = BarItemVisibility.Never));
		}
	}

	public void SaveColumns()
	{
		if (base.ObjectType.HasValue && listObjectsType != SharedObjectTypeEnum.ObjectType.UnresolvedEntity)
		{
			UserViewData.SaveColumns(UserViewData.GetViewName(this, allDatabaseProceduresGridView, listObjectsType), allDatabaseProceduresGridView);
		}
	}

	public override void SetParameters(DBTreeNode node, CustomFieldsSupport customFieldsSupport, SharedObjectTypeEnum.ObjectType? objectType)
	{
		gridPanelUserControl.SetRemoveButtonVisibility(value: true);
		gridPanelUserControl.SetDefaultLockButtonVisibility(value: true);
		gridPanelUserControl.CustomFields += base.MainControl.EditCustomFields;
		bool flag = false;
		if (!base.ObjectType.HasValue)
		{
			flag = true;
		}
		base.SetParameters(node, customFieldsSupport, objectType);
		allDatabaseProceduresGridView.SetRowCellValue(-2147483646, "iconTableGridColumn", Resources.blank_16);
		allDatabaseProceduresGrid.DataSource = null;
		databaseId = node.DatabaseId;
		folderNode = node;
		CustomFieldsCellsTypesSupport customFieldsCellsTypesSupport = new CustomFieldsCellsTypesSupport(isForSummaryTable: true);
		if (objectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			customFieldsCellsTypesSupport.SetCustomColumns(allDatabaseProceduresGridView, customFieldsSupport, listObjectsType, customFieldsAsArray: true);
			documentationGridColumn.Visible = true;
			List<ObjectWithModulesObject> list = null;
			if (listObjectsType == SharedObjectTypeEnum.ObjectType.Procedure)
			{
				list = DB.Procedure.GetProceduresByModuleWithoutDescription(node.Id);
			}
			else if (listObjectsType == SharedObjectTypeEnum.ObjectType.Function)
			{
				list = DB.Procedure.GetFunctionsByModuleWithoutDescription(node.Id);
			}
			DataSource = list;
			allDatabaseProceduresGrid.DataSource = list;
			CommonFunctionsPanels.SetModuleColumnVisibility(moduleTableGridColumn, isInDatabase: false);
			CommonFunctionsPanels.FillObjectEventArgs(ObjectEventArgs, node);
			deleteBarButtonItem.Caption = "Remove from Subject Area";
			CommonFunctionsPanels.AddSubtypeDisplayText(list);
		}
		else
		{
			customFieldsCellsTypesSupport.SetCustomColumns(allDatabaseProceduresGridView, customFieldsSupport, listObjectsType, customFieldsAsArray: true);
			documentationGridColumn.Visible = false;
			List<ObjectWithModulesObject> list2 = null;
			if (listObjectsType == SharedObjectTypeEnum.ObjectType.Procedure)
			{
				list2 = DB.Procedure.GetProceduresByDatabaseWithoutDescription(node.Id);
			}
			else if (listObjectsType == SharedObjectTypeEnum.ObjectType.Function)
			{
				list2 = DB.Procedure.GetFunctionsByDatabaseWithoutDescription(node.Id);
			}
			DataSource = list2;
			allDatabaseProceduresGrid.DataSource = list2;
			CommonFunctionsPanels.SetModuleColumnVisibility(moduleTableGridColumn, isInDatabase: true);
			CommonFunctionsDatabase.SetModuleWithDataSource(moduleRepositoryItemCheckedComboBoxEdit, node.Id);
			ObjectEventArgs.ModuleId = null;
			deleteBarButtonItem.Caption = "Remove from repository";
			CommonFunctionsPanels.AddSubtypeDisplayText(list2);
		}
		base.GridView = allDatabaseProceduresGridView;
		CommonFunctionsDatabase.SetModuleWithDataSource(moduleRepositoryItemCheckedComboBoxEdit, node.Id);
		base.Modules = moduleRepositoryItemCheckedComboBoxEdit.DataSource as List<DropdownModuleModel>;
		CommonFunctionsPanels.SetSummaryObjectTitle(allDatabaseProceduresLabel, objectType, SharedObjectTypeEnum.TypeToStringForMenu(listObjectsType), node.ParentNode.Title);
		gridPanelUserControl.Initialize(SharedObjectTypeEnum.TypeToStringForMenu(listObjectsType), objectType == SharedObjectTypeEnum.ObjectType.Module);
		if (flag && !UserViewData.LoadColumns(UserViewData.GetViewName(this, allDatabaseProceduresGridView, listObjectsType), allDatabaseProceduresGridView))
		{
			CustomFieldsCellsTypesSupport.SortCustomColumns(allDatabaseProceduresGridView);
			CommonFunctionsPanels.SetBestFitForColumns(allDatabaseProceduresGridView);
		}
		SaveOldTitleAndCustomFieldsHistory(DataSource);
	}

	public override void ClearData()
	{
		allDatabaseProceduresGrid.BeginUpdate();
		allDatabaseProceduresGrid.DataSource = null;
		allDatabaseProceduresGrid.EndUpdate();
	}

	private void allDatabaseProceduresGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		gridPanelUserControl.SetRemoveButtonVisibility(allDatabaseProceduresGridView.SelectedRowsCount > 0);
	}

	private void ProcedureSummaryUserControl_Leave(object sender, EventArgs e)
	{
		allDatabaseProceduresGridView.HideCustomization();
	}

	private void allDatabaseProceduresGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (e.Column != iconTableGridColumn && GridColumnsHelper.ShouldColumnBeGrayOut(e.Column) && (!allDatabaseProceduresGridView.IsFocusedView || !allDatabaseProceduresGridView.GetSelectedRows().Contains(e.RowHandle)))
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridNonEditableColumnsBackColor;
		}
	}

	private void ViewHistoryBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ObjectWithModulesObject objectWithModulesObject = base.GridView?.GetFocusedRow() as ObjectWithModulesObject;
		string field = base.GridView?.FocusedColumn?.FieldName?.ToLower();
		if (objectWithModulesObject == null || (field != "description" && field != "title" && base.CustomFieldsSupport.GetField(field) == null))
		{
			return;
		}
		try
		{
			using HistoryForm historyForm = new HistoryForm();
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			historyForm.CustomFieldCaption = allDatabaseProceduresGridView?.Columns?.Where((GridColumn x) => x.FieldName.ToLower() == field)?.FirstOrDefault()?.Caption;
			historyForm.SetParameters(objectWithModulesObject.Id, field, objectWithModulesObject.Name, objectWithModulesObject.Schema, objectWithModulesObject.DatabaseDatabaseShowSchema, objectWithModulesObject.DatabaseShowSchemaOverride, objectWithModulesObject.Title, "procedures", objectWithModulesObject.ObjectType, objectWithModulesObject.Subtype, objectWithModulesObject.Source, null);
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			historyForm.ShowDialog(FindForm());
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void SaveOldTitleAndCustomFieldsHistory(List<ObjectWithModulesObject> data)
	{
		CommonFunctionsPanels.customFieldsForHistory = new Dictionary<int, Dictionary<string, BaseWithCustomFields.CustomFieldWithValue>>();
		CommonFunctionsPanels.summaryObjectTitleHistory = new Dictionary<int, string>();
		if (data == null || allDatabaseProceduresGridView == null)
		{
			return;
		}
		IEnumerable<GridColumn> source = allDatabaseProceduresGridView.Columns.Where((GridColumn x) => x.FieldName.Contains("Field"));
		foreach (ObjectWithModulesObject objectWithModules in data)
		{
			CommonFunctionsPanels.customFieldsForHistory.Add(objectWithModules.Id, source.ToDictionary((GridColumn x) => x.FieldName, (GridColumn y) => BaseCustomFieldDB.GetCustomFieldWithValue(base.CustomFieldsSupport.GetField(y.FieldName), objectWithModules.GetField(y.FieldName)?.ToString())));
			CommonFunctionsPanels.summaryObjectTitleHistory.Add(objectWithModules.Id, objectWithModules.Title);
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
		Dataedo.App.Tools.DefaultBulkCopy defaultBulkCopy = new Dataedo.App.Tools.DefaultBulkCopy();
		this.allDatabaseProceduresLabel = new DevExpress.XtraEditors.LabelControl();
		this.allDatabaseProceduresGrid = new DevExpress.XtraGrid.GridControl();
		this.allDatabaseProceduresGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.iconTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.typeTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.documentationGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.schemaTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.titleTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.titleRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.moduleTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.moduleRepositoryItemCheckedComboBoxEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
		this.dbmsCreatedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dbmsLastModificationDateTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.synchronizationDateTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastImportedByGidColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.moduleRepositoryItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.typeRepositoryItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.proceduresToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.gridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.allProceduresPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.addToNewModuleBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.assignModuleBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.designBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.deleteBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.viewHistoryBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.allProceduresBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.addProcedureBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true, typeof(System.Windows.Forms.UserControl));
		((System.ComponentModel.ISupportInitialize)this.allDatabaseProceduresGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allDatabaseProceduresGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.moduleRepositoryItemCheckedComboBoxEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.moduleRepositoryItemLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeRepositoryItemLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allProceduresPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allProceduresBarManager).BeginInit();
		base.SuspendLayout();
		this.allDatabaseProceduresLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.allDatabaseProceduresLabel.Appearance.Options.UseFont = true;
		this.allDatabaseProceduresLabel.Appearance.Options.UseTextOptions = true;
		this.allDatabaseProceduresLabel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.allDatabaseProceduresLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.allDatabaseProceduresLabel.Dock = System.Windows.Forms.DockStyle.Top;
		this.allDatabaseProceduresLabel.Location = new System.Drawing.Point(0, 0);
		this.allDatabaseProceduresLabel.Name = "allDatabaseProceduresLabel";
		this.allDatabaseProceduresLabel.Padding = new System.Windows.Forms.Padding(0, 4, 4, 4);
		this.allDatabaseProceduresLabel.Size = new System.Drawing.Size(1009, 19);
		this.allDatabaseProceduresLabel.TabIndex = 9;
		this.allDatabaseProceduresLabel.Text = "Procedures in the database:";
		this.allDatabaseProceduresLabel.UseMnemonic = false;
		this.allDatabaseProceduresGrid.AllowDrop = true;
		this.allDatabaseProceduresGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.allDatabaseProceduresGrid.Dock = System.Windows.Forms.DockStyle.Fill;
		this.allDatabaseProceduresGrid.Location = new System.Drawing.Point(0, 47);
		this.allDatabaseProceduresGrid.MainView = this.allDatabaseProceduresGridView;
		this.allDatabaseProceduresGrid.Name = "allDatabaseProceduresGrid";
		this.allDatabaseProceduresGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.moduleRepositoryItemLookUpEdit, this.typeRepositoryItemLookUpEdit, this.iconRepositoryItemPictureEdit, this.moduleRepositoryItemCheckedComboBoxEdit, this.titleRepositoryItemCustomTextEdit });
		this.allDatabaseProceduresGrid.Size = new System.Drawing.Size(1009, 271);
		this.allDatabaseProceduresGrid.TabIndex = 10;
		this.allDatabaseProceduresGrid.ToolTipController = this.proceduresToolTipController;
		this.allDatabaseProceduresGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.allDatabaseProceduresGridView });
		this.allDatabaseProceduresGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[15]
		{
			this.iconTableGridColumn, this.typeTableGridColumn, this.documentationGridColumn, this.schemaTableGridColumn, this.nameTableGridColumn, this.titleTableGridColumn, this.moduleTableGridColumn, this.dbmsCreatedGridColumn, this.dbmsLastModificationDateTableGridColumn, this.createdGridColumn,
			this.createdByGridColumn, this.synchronizationDateTableGridColumn, this.lastImportedByGidColumn, this.lastUpdatedGridColumn, this.lastUpdatedByGridColumn
		});
		defaultBulkCopy.IsCopying = false;
		this.allDatabaseProceduresGridView.Copy = defaultBulkCopy;
		this.allDatabaseProceduresGridView.GridControl = this.allDatabaseProceduresGrid;
		this.allDatabaseProceduresGridView.Name = "allDatabaseProceduresGridView";
		this.allDatabaseProceduresGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.allDatabaseProceduresGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.allDatabaseProceduresGridView.OptionsSelection.MultiSelect = true;
		this.allDatabaseProceduresGridView.OptionsView.ColumnAutoWidth = false;
		this.allDatabaseProceduresGridView.OptionsView.RowAutoHeight = true;
		this.allDatabaseProceduresGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.allDatabaseProceduresGridView.OptionsView.ShowGroupPanel = false;
		this.allDatabaseProceduresGridView.OptionsView.ShowIndicator = false;
		this.allDatabaseProceduresGridView.RowHighlightingIsEnabled = true;
		this.allDatabaseProceduresGridView.SplashScreenManager = null;
		this.allDatabaseProceduresGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(allDatabaseProceduresGridView_CustomDrawCell);
		this.allDatabaseProceduresGridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(allDatabaseProceduresGridView_SelectionChanged);
		this.iconTableGridColumn.Caption = " ";
		this.iconTableGridColumn.ColumnEdit = this.iconRepositoryItemPictureEdit;
		this.iconTableGridColumn.FieldName = "iconTableGridColumn";
		this.iconTableGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.iconTableGridColumn.MaxWidth = 21;
		this.iconTableGridColumn.MinWidth = 21;
		this.iconTableGridColumn.Name = "iconTableGridColumn";
		this.iconTableGridColumn.OptionsColumn.AllowEdit = false;
		this.iconTableGridColumn.OptionsFilter.AllowFilter = false;
		this.iconTableGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.iconTableGridColumn.Visible = true;
		this.iconTableGridColumn.VisibleIndex = 0;
		this.iconTableGridColumn.Width = 21;
		this.iconRepositoryItemPictureEdit.AllowFocused = false;
		this.iconRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.Name = "iconRepositoryItemPictureEdit";
		this.iconRepositoryItemPictureEdit.ShowMenu = false;
		this.typeTableGridColumn.Caption = "Type";
		this.typeTableGridColumn.FieldName = "SubtypeDisplayText";
		this.typeTableGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.typeTableGridColumn.Name = "typeTableGridColumn";
		this.typeTableGridColumn.OptionsColumn.AllowEdit = false;
		this.typeTableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.typeTableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.typeTableGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.typeTableGridColumn.Tag = "FIT_WIDTH";
		this.typeTableGridColumn.Visible = true;
		this.typeTableGridColumn.VisibleIndex = 1;
		this.documentationGridColumn.Caption = "Documentation";
		this.documentationGridColumn.FieldName = "DatabaseTitle";
		this.documentationGridColumn.Name = "documentationGridColumn";
		this.documentationGridColumn.OptionsColumn.AllowEdit = false;
		this.documentationGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.documentationGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.documentationGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.documentationGridColumn.Tag = "FIT_WIDTH";
		this.documentationGridColumn.Visible = true;
		this.documentationGridColumn.VisibleIndex = 6;
		this.schemaTableGridColumn.Caption = "Schema";
		this.schemaTableGridColumn.FieldName = "Schema";
		this.schemaTableGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.schemaTableGridColumn.Name = "schemaTableGridColumn";
		this.schemaTableGridColumn.OptionsColumn.AllowEdit = false;
		this.schemaTableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.schemaTableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.schemaTableGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.schemaTableGridColumn.Tag = "FIT_WIDTH";
		this.schemaTableGridColumn.Visible = true;
		this.schemaTableGridColumn.VisibleIndex = 2;
		this.schemaTableGridColumn.Width = 60;
		this.nameTableGridColumn.Caption = "Name";
		this.nameTableGridColumn.FieldName = "Name";
		this.nameTableGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.nameTableGridColumn.Name = "nameTableGridColumn";
		this.nameTableGridColumn.OptionsColumn.AllowEdit = false;
		this.nameTableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.nameTableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.nameTableGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.nameTableGridColumn.Tag = "FIT_WIDTH";
		this.nameTableGridColumn.Visible = true;
		this.nameTableGridColumn.VisibleIndex = 3;
		this.nameTableGridColumn.Width = 140;
		this.titleTableGridColumn.Caption = "Title";
		this.titleTableGridColumn.ColumnEdit = this.titleRepositoryItemCustomTextEdit;
		this.titleTableGridColumn.FieldName = "Title";
		this.titleTableGridColumn.Name = "titleTableGridColumn";
		this.titleTableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.titleTableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.titleTableGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.titleTableGridColumn.Tag = "FIT_WIDTH";
		this.titleTableGridColumn.Visible = true;
		this.titleTableGridColumn.VisibleIndex = 4;
		this.titleTableGridColumn.Width = 140;
		this.titleRepositoryItemCustomTextEdit.AutoHeight = false;
		this.titleRepositoryItemCustomTextEdit.Name = "titleRepositoryItemCustomTextEdit";
		this.moduleTableGridColumn.Caption = "Subject Area";
		this.moduleTableGridColumn.ColumnEdit = this.moduleRepositoryItemCheckedComboBoxEdit;
		this.moduleTableGridColumn.FieldName = "ModulesId";
		this.moduleTableGridColumn.Name = "moduleTableGridColumn";
		this.moduleTableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.moduleTableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.moduleTableGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.moduleTableGridColumn.Tag = "FIT_WIDTH";
		this.moduleTableGridColumn.Visible = true;
		this.moduleTableGridColumn.VisibleIndex = 5;
		this.moduleTableGridColumn.Width = 250;
		this.moduleRepositoryItemCheckedComboBoxEdit.AutoHeight = false;
		this.moduleRepositoryItemCheckedComboBoxEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.moduleRepositoryItemCheckedComboBoxEdit.DisplayMember = "DisplayName";
		this.moduleRepositoryItemCheckedComboBoxEdit.Name = "moduleRepositoryItemCheckedComboBoxEdit";
		this.moduleRepositoryItemCheckedComboBoxEdit.PopupSizeable = false;
		this.moduleRepositoryItemCheckedComboBoxEdit.SelectAllItemVisible = false;
		this.moduleRepositoryItemCheckedComboBoxEdit.ShowButtons = false;
		this.moduleRepositoryItemCheckedComboBoxEdit.ShowPopupCloseButton = false;
		this.moduleRepositoryItemCheckedComboBoxEdit.ValueMember = "ModuleId";
		this.dbmsCreatedGridColumn.Caption = "Source created";
		this.dbmsCreatedGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.dbmsCreatedGridColumn.FieldName = "DbmsCreationDate";
		this.dbmsCreatedGridColumn.Name = "dbmsCreatedGridColumn";
		this.dbmsCreatedGridColumn.OptionsColumn.AllowEdit = false;
		this.dbmsCreatedGridColumn.OptionsColumn.AllowFocus = false;
		this.dbmsLastModificationDateTableGridColumn.Caption = "Source last updated";
		this.dbmsLastModificationDateTableGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.dbmsLastModificationDateTableGridColumn.FieldName = "DbmsLastModificationDate";
		this.dbmsLastModificationDateTableGridColumn.Name = "dbmsLastModificationDateTableGridColumn";
		this.dbmsLastModificationDateTableGridColumn.OptionsColumn.AllowEdit = false;
		this.dbmsLastModificationDateTableGridColumn.OptionsColumn.ReadOnly = true;
		this.dbmsLastModificationDateTableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.dbmsLastModificationDateTableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.dbmsLastModificationDateTableGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.dbmsLastModificationDateTableGridColumn.Width = 100;
		this.createdGridColumn.Caption = "Created/first imported";
		this.createdGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.createdGridColumn.FieldName = "CreationDate";
		this.createdGridColumn.Name = "createdGridColumn";
		this.createdGridColumn.OptionsColumn.AllowEdit = false;
		this.createdGridColumn.OptionsColumn.AllowFocus = false;
		this.createdGridColumn.Width = 120;
		this.createdByGridColumn.Caption = "Created/first imported by";
		this.createdByGridColumn.FieldName = "CreatedBy";
		this.createdByGridColumn.Name = "createdByGridColumn";
		this.createdByGridColumn.OptionsColumn.AllowEdit = false;
		this.createdByGridColumn.OptionsColumn.AllowFocus = false;
		this.createdByGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.createdByGridColumn.Width = 150;
		this.synchronizationDateTableGridColumn.Caption = "Last imported";
		this.synchronizationDateTableGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.synchronizationDateTableGridColumn.FieldName = "SynchronizationDate";
		this.synchronizationDateTableGridColumn.Name = "synchronizationDateTableGridColumn";
		this.synchronizationDateTableGridColumn.OptionsColumn.AllowEdit = false;
		this.synchronizationDateTableGridColumn.OptionsColumn.ReadOnly = true;
		this.synchronizationDateTableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.synchronizationDateTableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.synchronizationDateTableGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.synchronizationDateTableGridColumn.Width = 100;
		this.lastImportedByGidColumn.Caption = "Last imported by";
		this.lastImportedByGidColumn.FieldName = "SynchronizedBy";
		this.lastImportedByGidColumn.Name = "lastImportedByGidColumn";
		this.lastImportedByGidColumn.OptionsColumn.AllowEdit = false;
		this.lastImportedByGidColumn.OptionsColumn.AllowFocus = false;
		this.lastImportedByGidColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.lastImportedByGidColumn.Width = 100;
		this.lastUpdatedGridColumn.Caption = "Last updated";
		this.lastUpdatedGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.lastUpdatedGridColumn.FieldName = "LastModificationDate";
		this.lastUpdatedGridColumn.Name = "lastUpdatedGridColumn";
		this.lastUpdatedGridColumn.OptionsColumn.AllowEdit = false;
		this.lastUpdatedGridColumn.OptionsColumn.AllowFocus = false;
		this.lastUpdatedGridColumn.Width = 100;
		this.lastUpdatedByGridColumn.Caption = "Last updated by";
		this.lastUpdatedByGridColumn.FieldName = "ModifiedBy";
		this.lastUpdatedByGridColumn.Name = "lastUpdatedByGridColumn";
		this.lastUpdatedByGridColumn.OptionsColumn.AllowEdit = false;
		this.lastUpdatedByGridColumn.OptionsColumn.AllowFocus = false;
		this.lastUpdatedByGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.lastUpdatedByGridColumn.Width = 100;
		this.moduleRepositoryItemLookUpEdit.AutoHeight = false;
		this.moduleRepositoryItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.moduleRepositoryItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("title", "Title")
		});
		this.moduleRepositoryItemLookUpEdit.DisplayMember = "title";
		this.moduleRepositoryItemLookUpEdit.Name = "moduleRepositoryItemLookUpEdit";
		this.moduleRepositoryItemLookUpEdit.NullText = "";
		this.moduleRepositoryItemLookUpEdit.ShowFooter = false;
		this.moduleRepositoryItemLookUpEdit.ShowHeader = false;
		this.moduleRepositoryItemLookUpEdit.ShowLines = false;
		this.moduleRepositoryItemLookUpEdit.ValueMember = "module_id";
		this.typeRepositoryItemLookUpEdit.AutoHeight = false;
		this.typeRepositoryItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.typeRepositoryItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("title", "Title")
		});
		this.typeRepositoryItemLookUpEdit.DisplayMember = "title";
		this.typeRepositoryItemLookUpEdit.Name = "typeRepositoryItemLookUpEdit";
		this.typeRepositoryItemLookUpEdit.NullText = "";
		this.typeRepositoryItemLookUpEdit.ShowFooter = false;
		this.typeRepositoryItemLookUpEdit.ShowHeader = false;
		this.typeRepositoryItemLookUpEdit.ShowLines = false;
		this.typeRepositoryItemLookUpEdit.ValueMember = "name";
		this.proceduresToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.gridPanelUserControl.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.gridPanelUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.gridPanelUserControl.GridView = this.allDatabaseProceduresGridView;
		this.gridPanelUserControl.Location = new System.Drawing.Point(0, 19);
		this.gridPanelUserControl.Name = "gridPanelUserControl";
		this.gridPanelUserControl.Size = new System.Drawing.Size(1009, 28);
		this.gridPanelUserControl.TabIndex = 11;
		this.allProceduresPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.addToNewModuleBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.assignModuleBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.designBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.deleteBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.viewHistoryBarButtonItem)
		});
		this.allProceduresPopupMenu.Manager = this.allProceduresBarManager;
		this.allProceduresPopupMenu.Name = "allProceduresPopupMenu";
		this.addToNewModuleBarButtonItem.Caption = "Add to subject area";
		this.addToNewModuleBarButtonItem.Id = 1;
		this.addToNewModuleBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.module_16;
		this.addToNewModuleBarButtonItem.Name = "addToNewModuleBarButtonItem";
		this.addToNewModuleBarButtonItem.Tag = "addToNewModule";
		this.assignModuleBarButtonItem.Caption = "Assign subject area";
		this.assignModuleBarButtonItem.Id = 2;
		this.assignModuleBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.module_16;
		this.assignModuleBarButtonItem.Name = "assignModuleBarButtonItem";
		this.assignModuleBarButtonItem.Tag = "modules";
		this.designBarButtonItem.Caption = "Design Procedure";
		this.designBarButtonItem.Id = 3;
		this.designBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
		this.designBarButtonItem.Name = "designBarButtonItem";
		this.designBarButtonItem.Tag = "design";
		this.deleteBarButtonItem.Caption = "Delete";
		this.deleteBarButtonItem.Id = 5;
		this.deleteBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.deleteBarButtonItem.Name = "deleteBarButtonItem";
		this.viewHistoryBarButtonItem.Caption = "View History";
		this.viewHistoryBarButtonItem.Id = 6;
		this.viewHistoryBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.search_16;
		this.viewHistoryBarButtonItem.Name = "viewHistoryBarButtonItem";
		this.viewHistoryBarButtonItem.Tag = "viewHistory";
		this.viewHistoryBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ViewHistoryBarButtonItem_ItemClick);
		this.allProceduresBarManager.DockControls.Add(this.barDockControlTop);
		this.allProceduresBarManager.DockControls.Add(this.barDockControlBottom);
		this.allProceduresBarManager.DockControls.Add(this.barDockControlLeft);
		this.allProceduresBarManager.DockControls.Add(this.barDockControlRight);
		this.allProceduresBarManager.Form = this;
		this.allProceduresBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[6] { this.addToNewModuleBarButtonItem, this.assignModuleBarButtonItem, this.deleteBarButtonItem, this.designBarButtonItem, this.addProcedureBarButtonItem, this.viewHistoryBarButtonItem });
		this.allProceduresBarManager.MaxItemId = 7;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.allProceduresBarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(1009, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 318);
		this.barDockControlBottom.Manager = this.allProceduresBarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(1009, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.allProceduresBarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 318);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1009, 0);
		this.barDockControlRight.Manager = this.allProceduresBarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 318);
		this.addProcedureBarButtonItem.Hint = "Add Procedure";
		this.addProcedureBarButtonItem.Id = 5;
		this.addProcedureBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.procedure_new_16;
		this.addProcedureBarButtonItem.Name = "addProcedureBarButtonItem";
		this.splashScreenManager.ClosingDelay = 500;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.allDatabaseProceduresGrid);
		base.Controls.Add(this.gridPanelUserControl);
		base.Controls.Add(this.allDatabaseProceduresLabel);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "ProcedureSummaryUserControl";
		base.Size = new System.Drawing.Size(1009, 318);
		base.Load += new System.EventHandler(ProcedureSummaryUserControl_Load);
		base.Leave += new System.EventHandler(ProcedureSummaryUserControl_Leave);
		((System.ComponentModel.ISupportInitialize)this.allDatabaseProceduresGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allDatabaseProceduresGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.moduleRepositoryItemCheckedComboBoxEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.moduleRepositoryItemLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeRepositoryItemLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allProceduresPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allProceduresBarManager).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
