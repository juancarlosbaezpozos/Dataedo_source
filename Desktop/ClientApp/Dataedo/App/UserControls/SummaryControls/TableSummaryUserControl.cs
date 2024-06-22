using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.CommonFunctionsForPanels;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Helpers.Forms;
using Dataedo.App.History;
using Dataedo.App.Licences;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Helpers;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Columns;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.App.UserControls.PanelControls.TableUserControlHelpers;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.DataProfiling;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.UserControls.SummaryControls;

public class TableSummaryUserControl : BaseSummaryUserControl
{
	private const int ColumnPickerBarButtonIndex = 4;

	private SharedObjectTypeEnum.ObjectType listObjectsType;

	private readonly TableDragDropManager tableDragDropManager;

	private int databaseId;

	private List<ObjectWithModulesObject> dataSource;

	private DBTreeNode folderNode;

	private IContainer components;

	private LabelControl allDatabaseTablesLabel;

	private GridControl allDatabaseTablesGrid;

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

	private ToolTipController tablesToolTipController;

	private RepositoryItemCustomTextEdit titleRepositoryItemCustomTextEdit;

	private BulkCopyGridUserControl allDatabaseTablesGridView;

	private GridColumn documentationGridColumn;

	private GridColumn typeTableGridColumn;

	private GridPanelUserControl gridPanelUserControl;

	private GridColumn dbmsCreatedGridColumn;

	private GridColumn createdGridColumn;

	private GridColumn createdByGridColumn;

	private GridColumn lastImportedByGridColumn;

	private GridColumn lastUpdatedByGridColumn;

	private GridColumn lastUpdatedGridColumn;

	private PopupMenu allTablesPopupMenu;

	private BarButtonItem addTableBarButtonItem;

	private BarButtonItem addToNewModuleBarButtonItem;

	private BarButtonItem assignModuleBarButtonItem;

	private BarButtonItem addLinkedTermBarButtonItem;

	private BarButtonItem designTableBarButtonItem;

	private BarButtonItem deleteBarButtonItem;

	private BarManager allTablesBarManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private SplashScreenManager splashScreenManager;

	private GridColumn locationGridColumn;

	private RepositoryItemCustomTextEdit locationRepositoryItemCustomTextEdit;

	private BarButtonItem profileTableBarButtonItem;

	private GridColumn SparkLineRowsGridColumn;

	private RepositoryItemProgressBar sparkLineRowsRepositoryItemProgressBar;

	private BarButtonItem clearAllProfilingDataBarButtonItem;

	private BarButtonItem previewSampleDataBarButtonItem;

	private BarButtonItem addTableGridBarButtonItem;

	private BarButtonItem viewHistoryBarButtonItem;

	private BarButtonItem dataProfilingBarButtonItem;

	public int? NumberOfSelectedTablesInTheGrid => allDatabaseTablesGridView?.SelectedRowsCount;

	public event EventHandler ShowTableControl;

	public TableSummaryUserControl(SharedObjectTypeEnum.ObjectType listObjectsType)
	{
		InitializeComponent();
		this.listObjectsType = listObjectsType;
		base.BulkCopy = new SummaryBulkCopy(this.listObjectsType, this);
		allDatabaseTablesGridView.Copy = base.BulkCopy;
		allDatabaseTablesGridView.SplashScreenManager = splashScreenManager;
		MetadataToolTip.SetColumnToolTip(dbmsCreatedGridColumn, "dbms_created");
		MetadataToolTip.SetColumnToolTip(dbmsLastModificationDateTableGridColumn, "dbms_last_updated");
		MetadataToolTip.SetColumnToolTip(createdGridColumn, "first_imported");
		MetadataToolTip.SetColumnToolTip(createdByGridColumn, "first_imported_by");
		MetadataToolTip.SetColumnToolTip(synchronizationDateTableGridColumn, "last_imported");
		MetadataToolTip.SetColumnToolTip(lastImportedByGridColumn, "last_imported_by");
		MetadataToolTip.SetColumnToolTip(lastUpdatedGridColumn, "last_updated");
		MetadataToolTip.SetColumnToolTip(lastUpdatedByGridColumn, "last_updated_by");
		ObjectEventArgs = new ObjectEventArgs();
		LengthValidation.SetTitleOrNameLengthLimit(titleRepositoryItemCustomTextEdit);
		tableDragDropManager = new TableDragDropManager();
		tableDragDropManager.AddEvents(allDatabaseTablesGrid);
		gridPanelUserControl.InsertAdditionalButton(addTableGridBarButtonItem, 4);
	}

	private void TableSummaryUserControl_Load(object sender, EventArgs e)
	{
		AddEvents();
		WorkWithDataedoTrackingHelper.TrackFirstInSessionObjectsListView();
	}

	public override void RefreshData()
	{
		allDatabaseTablesGridView.RefreshData();
	}

	protected override void AddEvents()
	{
		CommonFunctionsPanels.AddEventsForSummaryTable(allDatabaseTablesGridView, listObjectsType, allTablesPopupMenu, allTablesBarManager, this.ShowTableControl, iconTableGridColumn, deleteBarButtonItem, ObjectEventArgs, base.TreeMenu, moduleRepositoryItemCheckedComboBoxEdit, this, gridPanelUserControl, null, null, null, null, null, FindForm());
		addToNewModuleBarButtonItem.ItemClick += delegate
		{
			AddToNewModule();
		};
		if (listObjectsType == SharedObjectTypeEnum.ObjectType.Table || listObjectsType == SharedObjectTypeEnum.ObjectType.Structure || listObjectsType == SharedObjectTypeEnum.ObjectType.View)
		{
			addTableBarButtonItem.Glyph = IconsForButtonsFinder.ReturnImageForAddButtonItem16(listObjectsType);
			designTableBarButtonItem.Glyph = IconsForButtonsFinder.ReturnImageForDesignButtonItem16(listObjectsType);
			ItemClickEventHandler value = delegate
			{
				CommonFunctionsDatabase.AddNewTable(allDatabaseTablesGridView, databaseId, folderNode, base.CustomFieldsSupport);
			};
			addTableBarButtonItem.Caption = "Add " + SharedObjectTypeEnum.TypeToStringForSingle(listObjectsType);
			designTableBarButtonItem.Caption = "Design " + SharedObjectTypeEnum.TypeToStringForSingle(listObjectsType);
			addTableBarButtonItem.ItemClick += value;
			addTableGridBarButtonItem.Hint = "Add " + SharedObjectTypeEnum.TypeToStringForSingle(listObjectsType);
			addTableGridBarButtonItem.Caption = string.Empty;
			addTableGridBarButtonItem.Glyph = addTableBarButtonItem.Glyph;
			addTableGridBarButtonItem.ItemClick += value;
			designTableBarButtonItem.ItemClick += delegate
			{
				CommonFunctionsDatabase.DesignTable(allDatabaseTablesGridView, dataSource, base.CustomFieldsSupport);
			};
			profileTableBarButtonItem.ItemClick += delegate
			{
				ProfileSelectedTables();
			};
			previewSampleDataBarButtonItem.ItemClick += delegate
			{
				PreviewSampleData();
			};
			clearAllProfilingDataBarButtonItem.ItemClick += delegate
			{
				ClearAllProfilingData();
			};
			dataProfilingBarButtonItem.ItemClick += delegate
			{
				ProfileSelectedTables();
			};
			addTableGridBarButtonItem.Visibility = BarItemVisibility.Always;
			addTableBarButtonItem.Visibility = BarItemVisibility.Always;
			designTableBarButtonItem.Visibility = BarItemVisibility.Always;
		}
		else
		{
			addTableBarButtonItem.Visibility = BarItemVisibility.Never;
			designTableBarButtonItem.Visibility = BarItemVisibility.Never;
		}
		GridColumn gridColumn = schemaTableGridColumn;
		bool visible = (schemaTableGridColumn.OptionsColumn.ShowInCustomizationForm = listObjectsType != SharedObjectTypeEnum.ObjectType.Structure);
		gridColumn.Visible = visible;
		GridColumn gridColumn2 = locationGridColumn;
		visible = (locationGridColumn.OptionsColumn.ShowInCustomizationForm = listObjectsType == SharedObjectTypeEnum.ObjectType.Structure);
		gridColumn2.Visible = visible;
		base.DragRows = new DragRowsBase<ObjectWithModulesObject>(allDatabaseTablesGridView);
		base.DragRows.AddEvents();
		List<ToolTipData> list = new List<ToolTipData>();
		list.Add(new ToolTipData(allDatabaseTablesGrid, listObjectsType, iconTableGridColumn.VisibleIndex));
		CommonFunctionsPanels.AddEventsForToolTips(tablesToolTipController, list);
		CommonFunctionsPanels.AddEventForAutoFilterRow(allDatabaseTablesGridView);
	}

	public void SaveColumns()
	{
		if (base.ObjectType.HasValue && listObjectsType != SharedObjectTypeEnum.ObjectType.UnresolvedEntity)
		{
			UserViewData.SaveColumns(UserViewData.GetViewName(this, allDatabaseTablesGridView, listObjectsType), allDatabaseTablesGridView);
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
		allDatabaseTablesGridView.SetRowCellValue(-2147483646, "iconTableGridColumn", Resources.blank_16);
		allDatabaseTablesGrid.DataSource = null;
		new CustomFieldsCellsTypesSupport(isForSummaryTable: true).SetCustomColumns(allDatabaseTablesGridView, customFieldsSupport, listObjectsType, customFieldsAsArray: true);
		List<ObjectWithModulesObject> data = null;
		bool flag2 = false;
		CommonFunctionsPanels.FillObjectEventArgs(ObjectEventArgs, node);
		if (objectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			allDatabaseTablesGridView.Columns.ColumnByFieldName("DatabaseTitle").Visible = true;
			if (listObjectsType == SharedObjectTypeEnum.ObjectType.Table || listObjectsType == SharedObjectTypeEnum.ObjectType.View || listObjectsType == SharedObjectTypeEnum.ObjectType.Structure)
			{
				data = DB.Table.GetObjectsByModuleWithoutDescription(node.Id, listObjectsType);
				if (DataProfilingUtils.ObjectCanBeProfilled(listObjectsType))
				{
					flag2 = SetSparklinesAndRefreshGridForOneTableByTableIdOrAllTables(data, flag2, databaseId);
				}
			}
			CommonFunctionsPanels.SetModuleColumnVisibility(moduleTableGridColumn, isInDatabase: false);
			deleteBarButtonItem.Caption = "Remove from Subject Area";
		}
		else
		{
			allDatabaseTablesGridView.Columns.ColumnByFieldName("DatabaseTitle").Visible = false;
			if (listObjectsType == SharedObjectTypeEnum.ObjectType.Table || listObjectsType == SharedObjectTypeEnum.ObjectType.View || listObjectsType == SharedObjectTypeEnum.ObjectType.Structure)
			{
				data = DB.Table.GetObjectsByDatabaseWithoutDescription(node.Id, listObjectsType);
				if (DataProfilingUtils.ObjectCanBeProfilled(listObjectsType))
				{
					flag2 = SetSparklinesAndRefreshGridForOneTableByTableIdOrAllTables(data, flag2, node.DatabaseId);
				}
			}
			CommonFunctionsPanels.SetModuleColumnVisibility(moduleTableGridColumn, isInDatabase: true);
			ObjectEventArgs.ModuleId = null;
			deleteBarButtonItem.Caption = "Remove from repository";
		}
		dataSource = data;
		allDatabaseTablesGrid.DataSource = dataSource;
		base.GridView = allDatabaseTablesGridView;
		CommonFunctionsDatabase.SetModuleWithDataSource(moduleRepositoryItemCheckedComboBoxEdit, node.DatabaseId);
		base.Modules = moduleRepositoryItemCheckedComboBoxEdit.DataSource as List<DropdownModuleModel>;
		CommonFunctionsPanels.AddSubtypeDisplayText(dataSource, listObjectsType);
		databaseId = node.DatabaseId;
		folderNode = node;
		CommonFunctionsPanels.SetSummaryObjectTitle(allDatabaseTablesLabel, objectType, SharedObjectTypeEnum.TypeToStringForMenu(listObjectsType), node.ParentNode.Title);
		if (DataProfilingUtils.ObjectCanBeProfilled(listObjectsType))
		{
			if (DB.DataProfiling.IsDataProfilingDisabled() || !Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
			{
				SparkLineRowsGridColumn.Visible = false;
				SparkLineRowsGridColumn.OptionsColumn.ShowInCustomizationForm = false;
			}
			else
			{
				SparkLineRowsGridColumn.Visible = flag2;
				SparkLineRowsGridColumn.OptionsColumn.ShowInCustomizationForm = true;
			}
		}
		else
		{
			SparkLineRowsGridColumn.Visible = false;
		}
		gridPanelUserControl.Initialize(SharedObjectTypeEnum.TypeToStringForMenu(listObjectsType), objectType == SharedObjectTypeEnum.ObjectType.Module);
		if (flag && !UserViewData.LoadColumns(UserViewData.GetViewName(this, allDatabaseTablesGridView, listObjectsType), allDatabaseTablesGridView))
		{
			CustomFieldsCellsTypesSupport.SortCustomColumns(allDatabaseTablesGridView);
			CommonFunctionsPanels.SetBestFitForColumns(allDatabaseTablesGridView);
		}
		SaveOldTitleAndCustomFieldsHistory(data);
		SetProfileTableButonVisibility(base.GridView);
		SetClearAllProfilingDataButtonVisibility();
	}

	public override void SetTooltips()
	{
		typeTableGridColumn.ToolTip = CommonTooltips.GetType(base.Node.ContainedObjectsObjectType);
		schemaTableGridColumn.ToolTip = CommonTooltips.GetSchema(base.Node.ContainedObjectsObjectType);
		nameTableGridColumn.ToolTip = CommonTooltips.GetName(base.Node.ContainedObjectsObjectType);
		titleTableGridColumn.ToolTip = CommonTooltips.GetTitle(base.Node.ContainedObjectsObjectType);
		moduleTableGridColumn.ToolTip = CommonTooltips.GetModule(base.Node.ContainedObjectsObjectType);
	}

	public override void ClearData()
	{
		allDatabaseTablesGrid.BeginUpdate();
		allDatabaseTablesGrid.DataSource = null;
		allDatabaseTablesGrid.EndUpdate();
	}

	private void allDatabaseTablesGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		gridPanelUserControl.SetRemoveButtonVisibility(allDatabaseTablesGridView.SelectedRowsCount > 0);
		if (sender is GridView profileTableButonVisibility)
		{
			SetProfileTableButonVisibility(profileTableButonVisibility);
		}
	}

	private void SetProfileTableButonVisibility(GridView gridView)
	{
		if (gridView == null || DB.DataProfiling.IsDataProfilingDisabled() || !DataProfilingUtils.ObjectCanBeProfilled(listObjectsType))
		{
			base.MainControl.SetProfileTableButtonVisibility(new ProfilingVisibilityEventArgs(singleObjectButtonsVisible: false, null, dataProfilingButtonVisible: false));
			return;
		}
		int[] selectedRows = gridView.GetSelectedRows();
		if (selectedRows == null || !selectedRows.Any())
		{
			base.MainControl.SetProfileTableButtonVisibility(new ProfilingVisibilityEventArgs(singleObjectButtonsVisible: false, null, dataProfilingButtonVisible: true));
		}
		else if (selectedRows.Length > 1)
		{
			bool dataProfilingButtonVisible = (from x in selectedRows.Select((int x) => gridView.GetRow(x)).OfType<ObjectWithModulesObject>()
				where DataProfilingUtils.ObjectCanBeProfilled(x)
				select x).Count() == selectedRows.Count();
			base.MainControl.SetProfileTableButtonVisibility(new ProfilingVisibilityEventArgs(singleObjectButtonsVisible: false, null, dataProfilingButtonVisible));
		}
		else if (selectedRows.Length == 1 && gridView.GetRow(selectedRows.FirstOrDefault()) is ObjectWithModulesObject objectWithModules && DataProfilingUtils.ObjectCanBeProfilled(objectWithModules))
		{
			base.MainControl.SetProfileTableButtonVisibility(new ProfilingVisibilityEventArgs(singleObjectButtonsVisible: true, listObjectsType, dataProfilingButtonVisible: true));
		}
	}

	private void SetClearAllProfilingDataButtonVisibility()
	{
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
		{
			clearAllProfilingDataBarButtonItem.Visibility = BarItemVisibility.Never;
		}
		else
		{
			clearAllProfilingDataBarButtonItem.Visibility = BarItemVisibility.Always;
		}
	}

	private void TableSummaryUserControl_Leave(object sender, EventArgs e)
	{
		allDatabaseTablesGridView.HideCustomization();
	}

	private void allTablesBarManager_HighlightedLinkChanged(object sender, HighlightedLinkChangedEventArgs e)
	{
		if (e.Link is BarCustomContainerItemLink barCustomContainerItemLink && barCustomContainerItemLink.ScreenBounds.Contains(Control.MousePosition))
		{
			barCustomContainerItemLink.OpenMenu();
		}
	}

	private void AllDatabaseTablesGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (e.Column != iconTableGridColumn && GridColumnsHelper.ShouldColumnBeGrayOut(e.Column) && (!allDatabaseTablesGridView.IsFocusedView || !allDatabaseTablesGridView.GetSelectedRows().Contains(e.RowHandle)))
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridNonEditableColumnsBackColor;
		}
	}

	private bool SetSparklinesAndRefreshGridForOneTableByTableIdOrAllTables(List<ObjectWithModulesObject> data, bool sparklineColumnHaveNonEmptyRows, int databaseId, int? tableId = null)
	{
		if (tableId.HasValue)
		{
			ObjectWithModulesObject objectWithModulesObject = data.FirstOrDefault((ObjectWithModulesObject t) => t.Id == tableId.Value);
			if (objectWithModulesObject == null)
			{
				return sparklineColumnHaveNonEmptyRows;
			}
			long? tablesStatsCount = DB.DataProfiling.SelectTableStats(tableId.Value)?.RowCount;
			sparklineColumnHaveNonEmptyRows = SetSparklineValue(sparklineColumnHaveNonEmptyRows, objectWithModulesObject, tablesStatsCount);
		}
		else
		{
			List<TableStatsRowCountDataObject> tablesStatsForDatabase = DB.DataProfiling.GetTablesStatsForDatabase(databaseId);
			foreach (ObjectWithModulesObject table in data)
			{
				long? tablesStatsCount2 = tablesStatsForDatabase.Where((TableStatsRowCountDataObject ts) => ts.TableId == table.Id).FirstOrDefault()?.RowCount;
				sparklineColumnHaveNonEmptyRows = SetSparklineValue(sparklineColumnHaveNonEmptyRows, table, tablesStatsCount2);
			}
		}
		RefreshData();
		return sparklineColumnHaveNonEmptyRows;
	}

	private bool SetSparklineValue(bool sparklineColumnHaveNonEmptyRows, ObjectWithModulesObject table, long? tablesStatsCount)
	{
		if (!tablesStatsCount.HasValue)
		{
			table.SparkLineRowsCount = -1.0;
		}
		else if (tablesStatsCount == 0)
		{
			if (!sparklineColumnHaveNonEmptyRows)
			{
				sparklineColumnHaveNonEmptyRows = true;
			}
			table.SparkLineRowsCount = null;
		}
		else
		{
			if (!sparklineColumnHaveNonEmptyRows)
			{
				sparklineColumnHaveNonEmptyRows = true;
			}
			double value = Math.Log(tablesStatsCount.Value, 1.33);
			table.SparkLineRowsCount = value;
		}
		return sparklineColumnHaveNonEmptyRows;
	}

	private void sparkLineRowsRepositoryItemProgressBar_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		if (!(e.DisplayText == "EMPTY") && e.Value != null)
		{
			double num = ((!(e.Value is IConvertible convertible)) ? 0.0 : convertible.ToDouble(null));
			int num2 = -1;
			if (num == (double)num2)
			{
				e.DisplayText = string.Empty;
			}
			else
			{
				string text2 = (e.DisplayText = DataProfilingStringFormatter.Format2FloatInteligentValues(Math.Pow(1.33, num)));
			}
		}
	}

	private void ClearAllProfilingData()
	{
		int[] selectedRows = allDatabaseTablesGridView.GetSelectedRows();
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
		{
			return;
		}
		List<ObjectWithModulesObject> list = (from x in selectedRows
			select allDatabaseTablesGridView.GetRow(x) as ObjectWithModulesObject into x
			where DataProfilingUtils.ObjectCanBeProfilled(x)
			select x).ToList();
		if (list.Count() != selectedRows.Count() || GeneralMessageBoxesHandling.Show("Do you want to delete all profiling data from " + ((list.Count() == 1) ? ("the <b>" + list.First().Name + "</b> " + listObjectsType.ToString().ToLower() + "?") : ($"all {list.Count()} selected " + SharedObjectTypeEnum.TypeToStringForMenu(listObjectsType).ToLower() + "?")), "Clear all Profiling Data", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, null, 1, FindForm()).DialogResult != DialogResult.OK)
		{
			return;
		}
		DateTime utcNow = DateTime.UtcNow;
		foreach (ObjectWithModulesObject item in list)
		{
			DB.DataProfiling.RemoveAllProfilingForSingleTable(item.Id, splashScreenManager, base.ParentForm);
			SetSparklinesAndRefreshGridForOneTableByTableIdOrAllTables(dataSource, sparklineColumnHaveNonEmptyRows: false, item.DatabaseId, item.Id);
		}
		string duration = ((int)(DateTime.UtcNow - utcNow).TotalSeconds).ToString();
		DataProfilingTrackingHelper.TrackDataProfilingCleared("ALL", duration);
	}

	public void ProfileSelectedTables()
	{
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
		{
			using (UpgradeDataProfilingForm upgradeDataProfilingForm = new UpgradeDataProfilingForm())
			{
				upgradeDataProfilingForm.ShowDialog();
				return;
			}
		}
		CommonFunctionsDatabase.ProfileSelectedTables(allDatabaseTablesGridView, base.MainControl);
	}

	public void PreviewSampleData()
	{
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
		{
			using (UpgradeDataProfilingForm upgradeDataProfilingForm = new UpgradeDataProfilingForm())
			{
				upgradeDataProfilingForm.ShowDialog();
				return;
			}
		}
		CommonFunctionsDatabase.PreviewSampleData(allDatabaseTablesGridView, base.MainControl);
	}

	internal void RefreshTableProfiling(int? tableId)
	{
		if (dataSource != null)
		{
			SetSparklinesAndRefreshGridForOneTableByTableIdOrAllTables(dataSource, sparklineColumnHaveNonEmptyRows: false, databaseId, tableId);
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
			historyForm.CustomFieldCaption = allDatabaseTablesGridView?.Columns?.Where((GridColumn x) => x.FieldName.ToLower() == field)?.FirstOrDefault()?.Caption;
			historyForm.SetParameters(objectWithModulesObject.Id, field, objectWithModulesObject.Name, objectWithModulesObject.Schema, objectWithModulesObject.DatabaseDatabaseShowSchema, objectWithModulesObject.DatabaseShowSchemaOverride, objectWithModulesObject.Title, "tables", objectWithModulesObject.ObjectType, objectWithModulesObject.Subtype, objectWithModulesObject.Source, null);
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			historyForm.ShowDialog(FindForm());
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void allDatabaseTablesGridView_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		if (e.Column == null || e.Column != SparkLineRowsGridColumn)
		{
			return;
		}
		e.Handled = true;
		double num;
		if (e.Value1 == null)
		{
			num = 0.0;
		}
		else
		{
			num = Convert.ToDouble(e.Value1);
			if (num != -1.0)
			{
				num = Math.Pow(1.33, num);
			}
		}
		double num2;
		if (e.Value2 == null)
		{
			num2 = 0.0;
		}
		else
		{
			num2 = Convert.ToDouble(e.Value2);
			if (num2 != -1.0)
			{
				num2 = Math.Pow(1.33, num2);
			}
		}
		e.Result = Comparer.Default.Compare(num, num2);
	}

	private void SaveOldTitleAndCustomFieldsHistory(List<ObjectWithModulesObject> data)
	{
		CommonFunctionsPanels.customFieldsForHistory = new Dictionary<int, Dictionary<string, BaseWithCustomFields.CustomFieldWithValue>>();
		CommonFunctionsPanels.summaryObjectTitleHistory = new Dictionary<int, string>();
		if (data == null || allDatabaseTablesGridView == null)
		{
			return;
		}
		IEnumerable<GridColumn> source = allDatabaseTablesGridView.Columns.Where((GridColumn x) => x.FieldName.Contains("Field"));
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
		DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem = new DevExpress.Utils.ToolTipItem();
		this.allDatabaseTablesLabel = new DevExpress.XtraEditors.LabelControl();
		this.allDatabaseTablesGrid = new DevExpress.XtraGrid.GridControl();
		this.allDatabaseTablesGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.iconTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.typeTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.documentationGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.schemaTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.titleTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.titleRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.locationGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.locationRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.moduleTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.moduleRepositoryItemCheckedComboBoxEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
		this.dbmsCreatedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dbmsLastModificationDateTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.synchronizationDateTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastImportedByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.SparkLineRowsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.sparkLineRowsRepositoryItemProgressBar = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.moduleRepositoryItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.typeRepositoryItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tablesToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.gridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.allTablesPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.addTableBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.addToNewModuleBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.assignModuleBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.addLinkedTermBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.designTableBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.dataProfilingBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.profileTableBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.previewSampleDataBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.clearAllProfilingDataBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.deleteBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.viewHistoryBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.allTablesBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.addTableGridBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true, typeof(System.Windows.Forms.UserControl));
		((System.ComponentModel.ISupportInitialize)this.allDatabaseTablesGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allDatabaseTablesGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.locationRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.moduleRepositoryItemCheckedComboBoxEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sparkLineRowsRepositoryItemProgressBar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.moduleRepositoryItemLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeRepositoryItemLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allTablesPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allTablesBarManager).BeginInit();
		base.SuspendLayout();
		this.allDatabaseTablesLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.allDatabaseTablesLabel.Appearance.Options.UseFont = true;
		this.allDatabaseTablesLabel.Appearance.Options.UseTextOptions = true;
		this.allDatabaseTablesLabel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.allDatabaseTablesLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.allDatabaseTablesLabel.Dock = System.Windows.Forms.DockStyle.Top;
		this.allDatabaseTablesLabel.Location = new System.Drawing.Point(0, 0);
		this.allDatabaseTablesLabel.Name = "allDatabaseTablesLabel";
		this.allDatabaseTablesLabel.Padding = new System.Windows.Forms.Padding(0, 4, 4, 4);
		this.allDatabaseTablesLabel.Size = new System.Drawing.Size(1246, 19);
		this.allDatabaseTablesLabel.TabIndex = 8;
		this.allDatabaseTablesLabel.Text = "Tables in the database:";
		this.allDatabaseTablesLabel.UseMnemonic = false;
		this.allDatabaseTablesGrid.AllowDrop = true;
		this.allDatabaseTablesGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.allDatabaseTablesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
		this.allDatabaseTablesGrid.Location = new System.Drawing.Point(0, 47);
		this.allDatabaseTablesGrid.MainView = this.allDatabaseTablesGridView;
		this.allDatabaseTablesGrid.Name = "allDatabaseTablesGrid";
		this.allDatabaseTablesGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.moduleRepositoryItemLookUpEdit, this.typeRepositoryItemLookUpEdit, this.iconRepositoryItemPictureEdit, this.moduleRepositoryItemCheckedComboBoxEdit, this.titleRepositoryItemCustomTextEdit, this.locationRepositoryItemCustomTextEdit, this.sparkLineRowsRepositoryItemProgressBar });
		this.allDatabaseTablesGrid.Size = new System.Drawing.Size(1246, 520);
		this.allDatabaseTablesGrid.TabIndex = 9;
		this.allDatabaseTablesGrid.ToolTipController = this.tablesToolTipController;
		this.allDatabaseTablesGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.allDatabaseTablesGridView });
		this.allDatabaseTablesGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[17]
		{
			this.iconTableGridColumn, this.typeTableGridColumn, this.documentationGridColumn, this.schemaTableGridColumn, this.nameTableGridColumn, this.titleTableGridColumn, this.locationGridColumn, this.moduleTableGridColumn, this.dbmsCreatedGridColumn, this.dbmsLastModificationDateTableGridColumn,
			this.createdGridColumn, this.createdByGridColumn, this.synchronizationDateTableGridColumn, this.lastImportedByGridColumn, this.lastUpdatedGridColumn, this.lastUpdatedByGridColumn, this.SparkLineRowsGridColumn
		});
		defaultBulkCopy.IsCopying = false;
		this.allDatabaseTablesGridView.Copy = defaultBulkCopy;
		this.allDatabaseTablesGridView.GridControl = this.allDatabaseTablesGrid;
		this.allDatabaseTablesGridView.Name = "allDatabaseTablesGridView";
		this.allDatabaseTablesGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.allDatabaseTablesGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.allDatabaseTablesGridView.OptionsSelection.MultiSelect = true;
		this.allDatabaseTablesGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.allDatabaseTablesGridView.OptionsView.ColumnAutoWidth = false;
		this.allDatabaseTablesGridView.OptionsView.RowAutoHeight = true;
		this.allDatabaseTablesGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.allDatabaseTablesGridView.OptionsView.ShowGroupPanel = false;
		this.allDatabaseTablesGridView.OptionsView.ShowIndicator = false;
		this.allDatabaseTablesGridView.RowHighlightingIsEnabled = true;
		this.allDatabaseTablesGridView.SplashScreenManager = null;
		this.allDatabaseTablesGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(AllDatabaseTablesGridView_CustomDrawCell);
		this.allDatabaseTablesGridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(allDatabaseTablesGridView_SelectionChanged);
		this.allDatabaseTablesGridView.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(allDatabaseTablesGridView_CustomColumnSort);
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
		this.documentationGridColumn.VisibleIndex = 7;
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
		this.nameTableGridColumn.OptionsColumn.ReadOnly = true;
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
		this.locationGridColumn.Caption = "Location";
		this.locationGridColumn.ColumnEdit = this.locationRepositoryItemCustomTextEdit;
		this.locationGridColumn.FieldName = "Location";
		this.locationGridColumn.Name = "locationGridColumn";
		this.locationGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.locationGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.locationGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.locationGridColumn.Tag = "FIT_WIDTH";
		this.locationGridColumn.Visible = true;
		this.locationGridColumn.VisibleIndex = 5;
		this.locationRepositoryItemCustomTextEdit.AutoHeight = false;
		this.locationRepositoryItemCustomTextEdit.Name = "locationRepositoryItemCustomTextEdit";
		this.moduleTableGridColumn.Caption = "Subject Area";
		this.moduleTableGridColumn.ColumnEdit = this.moduleRepositoryItemCheckedComboBoxEdit;
		this.moduleTableGridColumn.FieldName = "ModulesId";
		this.moduleTableGridColumn.Name = "moduleTableGridColumn";
		this.moduleTableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.moduleTableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.moduleTableGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.moduleTableGridColumn.Tag = "FIT_WIDTH";
		this.moduleTableGridColumn.Visible = true;
		this.moduleTableGridColumn.VisibleIndex = 6;
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
		this.dbmsCreatedGridColumn.Width = 100;
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
		this.createdGridColumn.Width = 150;
		this.createdByGridColumn.Caption = "Created/first imported by";
		this.createdByGridColumn.FieldName = "CreatedBy";
		this.createdByGridColumn.Name = "createdByGridColumn";
		this.createdByGridColumn.OptionsColumn.AllowEdit = false;
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
		this.lastImportedByGridColumn.Caption = "Last imported by";
		this.lastImportedByGridColumn.FieldName = "SynchronizedBy";
		this.lastImportedByGridColumn.Name = "lastImportedByGridColumn";
		this.lastImportedByGridColumn.OptionsColumn.AllowEdit = false;
		this.lastImportedByGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.lastImportedByGridColumn.Width = 100;
		this.lastUpdatedGridColumn.Caption = "Last updated";
		this.lastUpdatedGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.lastUpdatedGridColumn.FieldName = "LastModificationDate";
		this.lastUpdatedGridColumn.Name = "lastUpdatedGridColumn";
		this.lastUpdatedGridColumn.OptionsColumn.AllowEdit = false;
		this.lastUpdatedGridColumn.Width = 100;
		this.lastUpdatedByGridColumn.Caption = "Last updated by";
		this.lastUpdatedByGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.lastUpdatedByGridColumn.FieldName = "ModifiedBy";
		this.lastUpdatedByGridColumn.Name = "lastUpdatedByGridColumn";
		this.lastUpdatedByGridColumn.OptionsColumn.AllowEdit = false;
		this.lastUpdatedByGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.lastUpdatedByGridColumn.Width = 100;
		this.SparkLineRowsGridColumn.AppearanceCell.Options.UseTextOptions = true;
		this.SparkLineRowsGridColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.SparkLineRowsGridColumn.Caption = "Rows";
		this.SparkLineRowsGridColumn.ColumnEdit = this.sparkLineRowsRepositoryItemProgressBar;
		this.SparkLineRowsGridColumn.FieldName = "SparkLineRowsCount";
		this.SparkLineRowsGridColumn.MaxWidth = 104;
		this.SparkLineRowsGridColumn.MinWidth = 104;
		this.SparkLineRowsGridColumn.Name = "SparkLineRowsGridColumn";
		this.SparkLineRowsGridColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.SparkLineRowsGridColumn.Visible = true;
		this.SparkLineRowsGridColumn.VisibleIndex = 8;
		this.SparkLineRowsGridColumn.Width = 104;
		this.sparkLineRowsRepositoryItemProgressBar.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.sparkLineRowsRepositoryItemProgressBar.EndColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.sparkLineRowsRepositoryItemProgressBar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
		this.sparkLineRowsRepositoryItemProgressBar.LookAndFeel.UseDefaultLookAndFeel = false;
		this.sparkLineRowsRepositoryItemProgressBar.Minimum = -1;
		this.sparkLineRowsRepositoryItemProgressBar.Name = "sparkLineRowsRepositoryItemProgressBar";
		this.sparkLineRowsRepositoryItemProgressBar.NullText = "EMPTY";
		this.sparkLineRowsRepositoryItemProgressBar.PercentView = false;
		this.sparkLineRowsRepositoryItemProgressBar.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.sparkLineRowsRepositoryItemProgressBar.ShowTitle = true;
		this.sparkLineRowsRepositoryItemProgressBar.StartColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.sparkLineRowsRepositoryItemProgressBar.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(sparkLineRowsRepositoryItemProgressBar_CustomDisplayText);
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
		this.tablesToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.gridPanelUserControl.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.gridPanelUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.gridPanelUserControl.GridView = this.allDatabaseTablesGridView;
		this.gridPanelUserControl.Location = new System.Drawing.Point(0, 19);
		this.gridPanelUserControl.Name = "gridPanelUserControl";
		this.gridPanelUserControl.Size = new System.Drawing.Size(1246, 28);
		this.gridPanelUserControl.TabIndex = 10;
		this.allTablesPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[11]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.addTableBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.addToNewModuleBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.assignModuleBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.addLinkedTermBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.designTableBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.dataProfilingBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.profileTableBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.previewSampleDataBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.clearAllProfilingDataBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.deleteBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.viewHistoryBarButtonItem)
		});
		this.allTablesPopupMenu.Manager = this.allTablesBarManager;
		this.allTablesPopupMenu.Name = "allTablesPopupMenu";
		this.addTableBarButtonItem.Caption = "Add Table";
		this.addTableBarButtonItem.Id = 0;
		this.addTableBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.table_add_16;
		this.addTableBarButtonItem.Name = "addTableBarButtonItem";
		this.addToNewModuleBarButtonItem.Caption = "Add to new subject area";
		this.addToNewModuleBarButtonItem.Id = 1;
		this.addToNewModuleBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.module_16;
		this.addToNewModuleBarButtonItem.Name = "addToNewModuleBarButtonItem";
		this.addToNewModuleBarButtonItem.Tag = "addToNewModule";
		this.assignModuleBarButtonItem.Caption = "Assign subject area";
		this.assignModuleBarButtonItem.Id = 2;
		this.assignModuleBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.module_16;
		this.assignModuleBarButtonItem.Name = "assignModuleBarButtonItem";
		this.assignModuleBarButtonItem.Tag = "modules";
		this.addLinkedTermBarButtonItem.Caption = "Add new linked Business Glossary term";
		this.addLinkedTermBarButtonItem.Id = 3;
		this.addLinkedTermBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.term_add_16;
		this.addLinkedTermBarButtonItem.Name = "addLinkedTermBarButtonItem";
		this.addLinkedTermBarButtonItem.Tag = "addToBusinessGlossaryTerm";
		this.designTableBarButtonItem.Caption = "Design table";
		this.designTableBarButtonItem.Id = 4;
		this.designTableBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
		this.designTableBarButtonItem.Name = "designTableBarButtonItem";
		this.designTableBarButtonItem.Tag = "design";
		this.dataProfilingBarButtonItem.Caption = "Data Profiling";
		this.dataProfilingBarButtonItem.Id = 10;
		this.dataProfilingBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.profile_database_16;
		this.dataProfilingBarButtonItem.Name = "dataProfilingBarButtonItem";
		this.dataProfilingBarButtonItem.Tag = "dataProfiling";
		this.profileTableBarButtonItem.Caption = "Profile Table";
		this.profileTableBarButtonItem.Id = 6;
		this.profileTableBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.profile_table_16;
		this.profileTableBarButtonItem.Name = "profileTableBarButtonItem";
		this.profileTableBarButtonItem.Tag = "profileTable";
		this.previewSampleDataBarButtonItem.Caption = "Preview Sample Data";
		this.previewSampleDataBarButtonItem.Id = 8;
		this.previewSampleDataBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.sample_data_16;
		this.previewSampleDataBarButtonItem.Name = "previewSampleDataBarButtonItem";
		this.previewSampleDataBarButtonItem.Tag = "previewSampleData";
		this.clearAllProfilingDataBarButtonItem.Caption = "Clear all Profiling Data";
		this.clearAllProfilingDataBarButtonItem.Id = 7;
		this.clearAllProfilingDataBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.all_data_deleted_16;
		this.clearAllProfilingDataBarButtonItem.Name = "clearAllProfilingDataBarButtonItem";
		toolTipItem.Text = "Removes all profiling data for the selected table";
		superToolTip.Items.Add(toolTipItem);
		this.clearAllProfilingDataBarButtonItem.SuperTip = superToolTip;
		this.clearAllProfilingDataBarButtonItem.Tag = "clearAllProfilingData";
		this.deleteBarButtonItem.Caption = "Delete";
		this.deleteBarButtonItem.Id = 5;
		this.deleteBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.deleteBarButtonItem.Name = "deleteBarButtonItem";
		this.viewHistoryBarButtonItem.Caption = "View History";
		this.viewHistoryBarButtonItem.Id = 9;
		this.viewHistoryBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.search_16;
		this.viewHistoryBarButtonItem.Name = "viewHistoryBarButtonItem";
		this.viewHistoryBarButtonItem.Tag = "viewHistory";
		this.viewHistoryBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ViewHistoryBarButtonItem_ItemClick);
		this.allTablesBarManager.DockControls.Add(this.barDockControlTop);
		this.allTablesBarManager.DockControls.Add(this.barDockControlBottom);
		this.allTablesBarManager.DockControls.Add(this.barDockControlLeft);
		this.allTablesBarManager.DockControls.Add(this.barDockControlRight);
		this.allTablesBarManager.Form = this;
		this.allTablesBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[12]
		{
			this.addTableBarButtonItem, this.addToNewModuleBarButtonItem, this.assignModuleBarButtonItem, this.addLinkedTermBarButtonItem, this.designTableBarButtonItem, this.deleteBarButtonItem, this.profileTableBarButtonItem, this.clearAllProfilingDataBarButtonItem, this.previewSampleDataBarButtonItem, this.addTableGridBarButtonItem,
			this.viewHistoryBarButtonItem, this.dataProfilingBarButtonItem
		});
		this.allTablesBarManager.MaxItemId = 11;
		this.allTablesBarManager.HighlightedLinkChanged += new DevExpress.XtraBars.HighlightedLinkChangedEventHandler(allTablesBarManager_HighlightedLinkChanged);
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.allTablesBarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(1246, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 567);
		this.barDockControlBottom.Manager = this.allTablesBarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(1246, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.allTablesBarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 567);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1246, 0);
		this.barDockControlRight.Manager = this.allTablesBarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 567);
		this.addTableGridBarButtonItem.Caption = "Add Table";
		this.addTableGridBarButtonItem.Hint = "Add Table";
		this.addTableGridBarButtonItem.Id = 8;
		this.addTableGridBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.table_add_16;
		this.addTableGridBarButtonItem.Name = "addTableGridBarButtonItem";
		this.splashScreenManager.ClosingDelay = 500;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.allDatabaseTablesGrid);
		base.Controls.Add(this.gridPanelUserControl);
		base.Controls.Add(this.allDatabaseTablesLabel);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "TableSummaryUserControl";
		base.Size = new System.Drawing.Size(1246, 567);
		base.Load += new System.EventHandler(TableSummaryUserControl_Load);
		base.Leave += new System.EventHandler(TableSummaryUserControl_Leave);
		((System.ComponentModel.ISupportInitialize)this.allDatabaseTablesGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allDatabaseTablesGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.locationRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.moduleRepositoryItemCheckedComboBoxEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sparkLineRowsRepositoryItemProgressBar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.moduleRepositoryItemLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeRepositoryItemLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allTablesPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allTablesBarManager).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
