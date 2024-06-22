using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DataProfiling.DataProfilingForms;
using Dataedo.App.DataProfiling.DataProfilingUserControls;
using Dataedo.App.DataProfiling.Enums;
using Dataedo.App.DataProfiling.Models;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.Forms;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.OverriddenControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.DataSources.Commands;
using Dataedo.DataSources.Enums;
using Dataedo.DataSources.Factories;
using Dataedo.Log.Error;
using Dataedo.Model.Data.DataProfiling;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Behaviors;
using DevExpress.Utils.Extensions;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Localization;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.DataProfiling;

public class DataProfilingForm : BaseXtraForm
{
	private readonly Image cancelProfilingImage = Resources.stop_16;

	private readonly Image cancelProfilingLargeImage = Resources.stop_32;

	private const string cancelProfilingCaption = "Cancel profiling";

	private readonly Image fullProfilingImage = Resources.all_data_play_16;

	private readonly Image fullProfilingLargeImage = Resources.all_data_play_32;

	private const string fullProfilingCaption = "Full Profiling (selected objects)";

	private readonly Image profileDistributionImage = Resources.distribution_play_16;

	private readonly Image profileDistributionLargeImage = Resources.distribution_play_32;

	private const string profileDistributionCaption = "Profile Distribution (selected objects)";

	private readonly Image profileValuesImage = Resources.values_play_16;

	private readonly Image profileValuesLargeImage = Resources.values_play_32;

	private const string profileValuesCaption = "Profile Values (selected objects)";

	private readonly Dictionary<int, List<ProfiledDataModel>> profiledDataModels;

	private INavigationObject FocusedNavigationObject;

	private List<TableSimpleData> tables;

	private int databaseId;

	private readonly Dictionary<int, List<ColumnProfiledDataObject>> columns;

	private CancellationTokenSource profilingCancellationTokenSource;

	private bool formIsClosing;

	private bool formIsClosed;

	private SharedDatabaseTypeEnum.DatabaseType? databaseTypeOriginal;

	private MetadataEditorUserControl metadataEditorUserControl;

	private IContainer components;

	private SplashScreenManager splashScreenManager;

	private NonCustomizableLayoutControl wholeWindowLayoutControl;

	private LayoutControlGroup rootLayoutControlGroup;

	private RibbonControlWithGroupIcons aboveRibbonControl;

	private RibbonPage ribbonPage1;

	private BarButtonItem fullProfileBarButtonItem;

	private BehaviorManager behaviorManager1;

	private ProgressBarControl profileprogressBarControl;

	private SimpleButton saveAllSimpleButton;

	private SimpleButton quitWithoutSavingSimpleButton;

	private EmptySpaceItem emptySpaceItem3;

	private EmptySpaceItem emptySpaceItem4;

	private EmptySpaceItem emptySpaceItem2;

	private LayoutControlItem profileprogressBarLayoutControlItem;

	private LayoutControlItem saveAllSimpleButtonLayoutControlItem;

	private LayoutControlItem quitWithoutSavingSimpleButtonLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private LayoutControlItem aboveRibbondLayoutControlItem;

	private EmptySpaceItem emptySpaceItem5;

	private RibbonPageGroup profilingRibbonPageGroup;

	private EmptySpaceItem emptySpaceItem15;

	private EmptySpaceItem emptySpaceItem33;

	private BarManager barManager1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private RibbonPageGroup clearRibbonPageGroup;

	private RibbonPageGroup saveRibbonPageGroup;

	private BarButtonItem clearAllDataBarButtonItem;

	private BarButtonItem clearDistributionBarButtonItem;

	private BarButtonItem clearValuesBarButtonItem;

	private BarButtonItem clearAllProfilingBarButtonItem;

	private BarButtonItem saveAllDatabarButtonItem;

	private BarButtonItem saveDistributionBarButtonItem;

	private BarButtonItem saveValuesBarButtonItem;

	private BarButtonItem profileDistributionBarButtonItem;

	private BarButtonItem profileValuesBarButtonItem;

	private SplitterItem LeftCenterSplitterItem;

	private SplitterItem centerRightSplitterItem;

	private BarStaticItem savingIsDisabledBarStaticItem;

	private BarStaticItem learnMoreBarStaticItem;

	private ToolTipController ribbonToolTipController;

	private ValuesUserControl valuesUserControl;

	private LayoutControlItem rightSectionLayoutControlItem;

	private CenterSectionUserControl centerSectionUserControl;

	private LayoutControlItem centerSectionLayoutControlItem;

	private NavigationUserControl navigationUserControl;

	private LayoutControlItem leftSectionNavigationLayoutControlItem;

	private InfoUserControl infoUserControl;

	private LayoutControlItem infoPanelLayoutControlItem;

	public DatabaseRow DatabaseRow { get; set; }

	private void RefreshViewIfFocusedObject(INavigationObject navObj)
	{
		if (navObj == FocusedNavigationObject)
		{
			SetPresentationFocusOnSpecificColumn();
		}
	}

	private void SetPresentationFocusOnSpecificColumn()
	{
		try
		{
			INavigationObject focusedNavigationObject = FocusedNavigationObject;
			valuesUserControl.SetPresentationFocusOnSpecificColumn();
			centerSectionUserControl.SetPresentationFocusOnSpecificColumn();
			if (focusedNavigationObject == null)
			{
				return;
			}
			if (focusedNavigationObject is ColumnNavigationObject columnNavigationObject && columnNavigationObject.Column != null)
			{
				if (!columnNavigationObject.Column.ProfilingDate.HasValue)
				{
					HideRightSection();
				}
				else
				{
					ShowRightSection();
				}
			}
			else if (focusedNavigationObject is TableNavigationObject)
			{
				HideRightSection();
				infoPanelLayoutControlItem.Visibility = LayoutVisibility.Never;
			}
		}
		catch (Exception ex)
		{
			if (!formIsClosed)
			{
				DataProfiler.ShowErrorMessageBox(ex, this);
			}
		}
	}

	private void HideRightSection()
	{
		rightSectionLayoutControlItem.Visibility = LayoutVisibility.Never;
	}

	private void ShowRightSection()
	{
		rightSectionLayoutControlItem.Visibility = LayoutVisibility.Always;
	}

	private TableNavigationObject GetTableNavigationObject(int tableId)
	{
		return navigationUserControl.GetTableNavigationObject(tableId);
	}

	private ColumnNavigationObject GetColumnNavigationObject(int columnId)
	{
		return navigationUserControl.GetColumnNavigationObject(columnId);
	}

	public bool IsTableInCurrentProfilingSession(int tableId)
	{
		return navigationUserControl.IsTableInCurrentProfilingSession(tableId);
	}

	public void RefreshNavigationTreeList()
	{
		navigationUserControl.RefreshNavigationTreeList();
	}

	private void SaveDistributionAllColumnsRibbonBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (IsSavingPossible())
		{
			SaveDistributionForAllColumns();
			RefreshRibbonButtonsEnability();
			RefreshNavigationTreeList();
		}
	}

	private void SaveValuesSectionAllColumnsRibbonBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (IsSavingPossible())
		{
			SaveValuesForAllColumns();
			RefreshRibbonButtonsEnability();
			RefreshNavigationTreeList();
		}
	}

	private void SaveAllDataAllColumnsRibbonBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (IsSavingPossible())
		{
			SaveAllProfilingForAllColumns();
			RefreshRibbonButtonsEnability();
			RefreshNavigationTreeList();
		}
	}

	private void SaveAllSimpleButton_Click(object sender, EventArgs e)
	{
		if (IsSavingPossible())
		{
			SaveRowOrValuesForm saveRowOrValuesForm = new SaveRowOrValuesForm();
			if (saveRowOrValuesForm.ShowDialog(this) == DialogResult.OK)
			{
				SaveUsingRowOrValuesForm(saveRowOrValuesForm.SaveRow, saveRowOrValuesForm.SaveValues);
				RefreshRibbonButtonsEnability();
				RefreshNavigationTreeList();
			}
		}
	}

	private void SaveUsingRowOrValuesForm(bool saveRow, bool saveValues)
	{
		if (saveRow || saveValues)
		{
			if (saveRow && !saveValues)
			{
				SaveDistributionForAllColumns();
			}
			else if (!saveRow && saveValues)
			{
				SaveValuesForAllColumns();
			}
			else if (saveRow && saveValues)
			{
				SaveAllProfilingForAllColumns();
			}
		}
	}

	private void SaveDistributionForAllColumns()
	{
		try
		{
			DateTime utcNow = DateTime.UtcNow;
			navigationUserControl.PostEditor();
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			SaveRowCountForAllTables();
			List<int> list = (from x in profiledDataModels.SelectMany((KeyValuePair<int, List<ProfiledDataModel>> x) => x.Value)
				where x.IsRowDistributionProfiling
				select x.ColumnId).ToList();
			if (!columns.Any())
			{
				return;
			}
			foreach (int item in list)
			{
				ColumnNavigationObject columnNavigationObject = GetColumnNavigationObject(item);
				SaveNavColumnProfilingDistributionSection(columnNavigationObject);
			}
			string duration = ((int)(DateTime.UtcNow - utcNow).TotalSeconds).ToString();
			DataProfilingTrackingHelper.TrackDataProfilingSaved("DISTRIBUTION", duration);
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			ShowSuccessSaveMessageBox();
		}
		catch (Exception ex)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			DataProfiler.ShowErrorMessageBox(ex, this);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private void SaveValuesForAllColumns()
	{
		try
		{
			DateTime utcNow = DateTime.UtcNow;
			navigationUserControl.PostEditor();
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			SaveRowCountForAllTables();
			List<int> list = (from x in profiledDataModels.SelectMany((KeyValuePair<int, List<ProfiledDataModel>> x) => x.Value)
				where x.IsValuesProfiling
				select x.ColumnId).ToList();
			List<int> list2 = list.Union(valuesUserControl.IdsOfColumnsWithChangedValues).ToList();
			if (!list.Any() && !list2.Any())
			{
				return;
			}
			foreach (int item in list)
			{
				ColumnNavigationObject columnNavigationObject = GetColumnNavigationObject(item);
				SaveNavColumnProfilingValueSection(columnNavigationObject);
			}
			foreach (int item2 in list2)
			{
				ColumnNavigationObject columnNavigationObject2 = GetColumnNavigationObject(item2);
				SaveNavColumnValues(columnNavigationObject2);
			}
			string duration = ((int)(DateTime.UtcNow - utcNow).TotalSeconds).ToString();
			DataProfilingTrackingHelper.TrackDataProfilingSaved("VALUES", duration);
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			ShowSuccessSaveMessageBox();
		}
		catch (Exception ex)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			DataProfiler.ShowErrorMessageBox(ex, this);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private void SaveAllProfilingForAllColumns()
	{
		try
		{
			DateTime utcNow = DateTime.UtcNow;
			navigationUserControl.PostEditor();
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			SaveRowCountForAllTables();
			List<int> list = (from x in profiledDataModels.SelectMany((KeyValuePair<int, List<ProfiledDataModel>> x) => x.Value)
				where x.IsValuesProfiling || x.IsRowDistributionProfiling
				select x.ColumnId).ToList();
			List<int> list2 = (from x in profiledDataModels.SelectMany((KeyValuePair<int, List<ProfiledDataModel>> x) => x.Value)
				where x.IsValuesProfiling
				select x.ColumnId).Union(valuesUserControl.IdsOfColumnsWithChangedValues).Distinct().ToList();
			if (!list.Any() && !list2.Any())
			{
				return;
			}
			foreach (int item in list)
			{
				ColumnNavigationObject columnNavigationObject = GetColumnNavigationObject(item);
				SaveNavColumnAllProfiling(columnNavigationObject);
			}
			foreach (int item2 in list2)
			{
				ColumnNavigationObject columnNavigationObject2 = GetColumnNavigationObject(item2);
				SaveNavColumnValues(columnNavigationObject2);
			}
			string duration = ((int)(DateTime.UtcNow - utcNow).TotalSeconds).ToString();
			DataProfilingTrackingHelper.TrackDataProfilingSaved("ALL", duration);
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			ShowSuccessSaveMessageBox();
		}
		catch (Exception ex)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			DataProfiler.ShowErrorMessageBox(ex, this);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private void SaveRowCountForAllTables()
	{
		if (!IsSavingPossible())
		{
			return;
		}
		foreach (TableNavigationObject item in navigationUserControl.AllNavTables.Where((TableNavigationObject x) => (from z in profiledDataModels.SelectMany((KeyValuePair<int, List<ProfiledDataModel>> y) => y.Value)
			select z.TableId).Contains(x.TableId)).ToList())
		{
			SaveRowCountToRepository(item.TableId, item.RowsCount);
		}
	}

	private void SaveRowCountToRepository(int tableId, long? rowsCount)
	{
		if (!IsSavingPossible())
		{
			return;
		}
		try
		{
			DB.DataProfiling.SaveTableRowCountToRepository(tableId, rowsCount);
		}
		catch (Exception ex)
		{
			DataProfiler.ShowErrorMessageBox(ex, this);
		}
	}

	private bool IsSavingPossible()
	{
		SetButtonsVisibilityBasedOnKillerSwitch();
		if (DB.DataProfiling.ProfilingNoSave)
		{
			return false;
		}
		if (DB.DataProfiling.IsDataProfilingDisabled())
		{
			Close();
			return false;
		}
		return true;
	}

	private void ShowSuccessSaveMessageBox()
	{
		GeneralMessageBoxesHandling.Show("All your profile data has been successfully saved", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
	}

	private void SaveNavColumnProfilingDistributionSection(ColumnNavigationObject navColumn)
	{
		if (navColumn == null)
		{
			return;
		}
		int tableId = navColumn.TableId;
		int columnId = navColumn.ColumnId;
		if (!profiledDataModels.ContainsKey(tableId) || !columns.ContainsKey(tableId))
		{
			return;
		}
		ProfiledDataModel profiledDataModel = profiledDataModels[tableId].Where((ProfiledDataModel x) => x.ColumnId == columnId).FirstOrDefault();
		ColumnProfiledDataObject columnProfiledDataObject = columns[tableId].Where((ColumnProfiledDataObject x) => x.ColumnId == columnId).FirstOrDefault();
		if (profiledDataModel != null && columnProfiledDataObject != null)
		{
			DB.DataProfiling.SaveColumnProfilingRowDistribution(columnProfiledDataObject);
			profiledDataModel.IsRowDistributionProfiling = false;
			if (!profiledDataModel.IsValuesProfiling && !profiledDataModel.IsRowDistributionProfiling)
			{
				profiledDataModels[tableId].Remove(profiledDataModel);
			}
		}
	}

	private void SaveNavColumnProfilingValueSection(ColumnNavigationObject navColumn)
	{
		if (navColumn == null)
		{
			return;
		}
		int tableId = navColumn.TableId;
		int columnId = navColumn.ColumnId;
		if (!profiledDataModels.ContainsKey(tableId) || !columns.ContainsKey(tableId))
		{
			return;
		}
		ProfiledDataModel profiledDataModel = profiledDataModels[tableId].Where((ProfiledDataModel x) => x.ColumnId == columnId).FirstOrDefault();
		ColumnProfiledDataObject columnProfiledDataObject = columns[tableId].Where((ColumnProfiledDataObject x) => x.ColumnId == columnId).FirstOrDefault();
		if (profiledDataModel != null && columnProfiledDataObject != null)
		{
			DB.DataProfiling.SaveColumnProfilingValueSection(columnProfiledDataObject);
			profiledDataModel.IsValuesProfiling = false;
			if (!profiledDataModel.IsValuesProfiling && !profiledDataModel.IsRowDistributionProfiling)
			{
				profiledDataModels[tableId].Remove(profiledDataModel);
			}
		}
	}

	private void SaveNavColumnAllProfiling(ColumnNavigationObject navColumn)
	{
		if (navColumn == null)
		{
			return;
		}
		int tableId = navColumn.TableId;
		int columnId = navColumn.ColumnId;
		if (profiledDataModels.ContainsKey(tableId) && columns.ContainsKey(tableId))
		{
			ProfiledDataModel profiledDataModel = profiledDataModels[tableId].Where((ProfiledDataModel x) => x.ColumnId == columnId).FirstOrDefault();
			ColumnProfiledDataObject columnProfiledDataObject = columns[tableId].Where((ColumnProfiledDataObject x) => x.ColumnId == columnId).FirstOrDefault();
			if (profiledDataModel != null && columnProfiledDataObject != null)
			{
				DB.DataProfiling.SaveColumnAllProfiling(columnProfiledDataObject);
				profiledDataModel.IsValuesProfiling = false;
				profiledDataModel.IsRowDistributionProfiling = false;
				profiledDataModels[tableId].Remove(profiledDataModel);
			}
		}
	}

	private void SaveNavColumnValues(ColumnNavigationObject navColumn)
	{
		if (navColumn != null)
		{
			DB.DataProfiling.SaveColumnValues(navColumn);
			valuesUserControl.SetColumnValuesClearedOrSaved(navColumn.ColumnId);
		}
	}

	private void SaveNavColumnAfterProfiling(ColumnNavigationObject navColumn, ProfilingType profilingType)
	{
		switch (profilingType)
		{
		case ProfilingType.Values:
			SaveNavColumnProfilingValueSection(navColumn);
			SaveNavColumnValues(navColumn);
			break;
		case ProfilingType.Distribution:
			SaveNavColumnProfilingDistributionSection(navColumn);
			break;
		case ProfilingType.Full:
			SaveNavColumnAllProfiling(navColumn);
			SaveNavColumnValues(navColumn);
			break;
		}
	}

	public bool ColumnRequiresSaving(int tableId, int columnId)
	{
		if (valuesUserControl.IdsOfColumnsWithChangedValues.Contains(columnId) || profiledDataModels[tableId].Where((ProfiledDataModel x) => x.ColumnId == columnId).Any())
		{
			return true;
		}
		return false;
	}

	private async void FullProfilingSelectedObjectsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		BarButtonItem barButtonItem = e.Item as BarButtonItem;
		if (ButtonHasProfilingCaption(barButtonItem))
		{
			await ProfileSelectedObjectsButtonClickedAsync(ProfilingType.Full);
			return;
		}
		RestoreButtonProfilingCaption(barButtonItem);
		profilingCancellationTokenSource.Cancel();
	}

	private async void ProfileDistributionSelectedObjectsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		BarButtonItem barButtonItem = e.Item as BarButtonItem;
		if (ButtonHasProfilingCaption(barButtonItem))
		{
			await ProfileSelectedObjectsButtonClickedAsync(ProfilingType.Distribution);
			return;
		}
		RestoreButtonProfilingCaption(barButtonItem);
		profilingCancellationTokenSource.Cancel();
	}

	private async void ProfileValuesSelectedObjectsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		BarButtonItem barButtonItem = e.Item as BarButtonItem;
		if (ButtonHasProfilingCaption(barButtonItem))
		{
			await ProfileSelectedObjectsButtonClickedAsync(ProfilingType.Values);
			return;
		}
		RestoreButtonProfilingCaption(barButtonItem);
		profilingCancellationTokenSource.Cancel();
	}

	public async Task PreviewSampleDataForSingleNavTableAsync(TableNavigationObject navTable)
	{
		if (navTable == null)
		{
			return;
		}
		ProfilingType profilingType = ProfilingType.Full;
		if (!DataProfilingUtils.IsConnectorInLicense(databaseTypeOriginal, this))
		{
			return;
		}
		profilingCancellationTokenSource = new CancellationTokenSource();
		SetProfilingStartVisibility(profilingType);
		DatabaseRow connectedDatabaseRow = ReturnConnectedDatabaseRow();
		if (connectedDatabaseRow == null)
		{
			SetProfilingEndVisibility(profilingType);
			return;
		}
		SetButtonsVisibilityBasedOnKillerSwitch();
		centerSectionUserControl.DisableCentralProfilingButton();
		ProfilingProgress progressBarToTrackWholeProfiling = CreateProfilingProgress();
		try
		{
			navTable.RowsCount = await DataProfiler.GetTableRowCounterAsync(navTable.TableName, navTable.TableSchema, connectedDatabaseRow, profilingCancellationTokenSource.Token);
			CommandsSet dbConnectionCommands = CommandsFactory.GetDbConnectionCommands(EnumToEnumChanger.GetProfilingDatabaseTypeEnum(connectedDatabaseRow.Type), connectedDatabaseRow.ConnectionString, CommandsWithTimeout.Timeout);
			if (navTable.RowsCount.HasValue && navTable.RowsCount > 0)
			{
				await ProfileNavTableAsync(navTable, ProfilingType.Values, dbConnectionCommands, progressBarToTrackWholeProfiling);
				DataProfilingTrackingHelper.TrackSampleData(connectedDatabaseRow);
			}
			if (!formIsClosing)
			{
				SetProfilingEndVisibility(profilingType);
			}
			RefreshViewIfFocusedObject(navTable);
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception)
		{
			navTable.ErrorOccurred = true;
		}
	}

	private async Task ProfileSelectedObjectsButtonClickedAsync(ProfilingType profilingType)
	{
		navigationUserControl.PostEditor();
		if (navigationUserControl.NoObjectsSelectedForProfiling)
		{
			GeneralMessageBoxesHandling.Show("There are no objects selected for profiling", "No objects selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
			return;
		}
		await ProfileSelectedObjectsAsync(profilingType, (TableNavigationObject t) => t.ProfileCheckbox, (ColumnNavigationObject c) => c.ProfileCheckbox);
	}

	public async Task FullProfileSingleNavTableAsync(int tableId)
	{
		if (GetTableNavigationObject(tableId) != null)
		{
			await ProfileSelectedObjectsAsync(ProfilingType.Full, (TableNavigationObject t) => t.TableId == tableId, (ColumnNavigationObject c) => c.TableId == tableId);
		}
	}

	public async Task FullProfileSingleNavColumnAsync(int columnId)
	{
		if (GetColumnNavigationObject(columnId) != null)
		{
			await ProfileSelectedObjectsAsync(ProfilingType.Full, (TableNavigationObject t) => false, (ColumnNavigationObject c) => c.ColumnId == columnId);
		}
	}

	private async Task ProfileSelectedObjectsAsync(ProfilingType profilingType, Func<TableNavigationObject, bool> tablesSelector, Func<ColumnNavigationObject, bool> columnsSelector)
	{
		_ = string.Empty;
		if (!DataProfilingUtils.IsConnectorInLicense(databaseTypeOriginal, this))
		{
			return;
		}
		navigationUserControl.PostEditor();
		profileprogressBarControl.EditValue = 0;
		SetProfilingStartVisibility(profilingType);
		profilingCancellationTokenSource = new CancellationTokenSource();
		DatabaseRow connectedDatabaseRow = ReturnConnectedDatabaseRow();
		if (connectedDatabaseRow == null)
		{
			SetProfilingEndVisibility(profilingType);
			return;
		}
		List<TableNavigationObject> source = navigationUserControl.AllNavTables.Where((TableNavigationObject c) => tablesSelector(c)).ToList();
		List<ColumnNavigationObject> columnsToProfile = navigationUserControl.AllNavColumns.Where((ColumnNavigationObject c) => columnsSelector(c)).ToList();
		IEnumerable<int> enumerable = source.Select((TableNavigationObject x) => x.TableId).Union(columnsToProfile.Select((ColumnNavigationObject x) => x.TableId)).Distinct();
		int tablesToProfileCounter = columnsToProfile.Select((ColumnNavigationObject x) => x.TableId).Distinct().Count();
		bool autoSaveProfiling = false;
		if (enumerable.Count() > 1)
		{
			autoSaveProfiling = UseAutoSaveDuringProfiling();
		}
		DataProfilingTrackingHelper.TrackDataProfilingStarted(connectedDatabaseRow, DataProfilingTrackingHelper.GetProfilingScope(profilingType), tablesToProfileCounter, columnsToProfile.Count());
		DateTime startTime = DateTime.UtcNow;
		SetButtonsVisibilityBasedOnKillerSwitch();
		ProfilingProgress profilingProgress = CreateProfilingProgress();
		profilingProgress.Init(source.Count(), columnsToProfile.Count());
		ProfilingDatabaseTypeEnum.ProfilingDatabaseType databaseTypeEnum = EnumToEnumChanger.GetProfilingDatabaseTypeEnum(connectedDatabaseRow.Type);
		CommandsSet commandsSet = CommandsFactory.GetDbConnectionCommands(databaseTypeEnum, connectedDatabaseRow.ConnectionString, CommandsWithTimeout.Timeout);
		foreach (int item in enumerable)
		{
			if (profilingCancellationTokenSource.IsCancellationRequested)
			{
				break;
			}
			TableNavigationObject navTable = GetTableNavigationObject(item);
			if (navTable == null)
			{
				continue;
			}
			try
			{
				TableNavigationObject tableNavigationObject = navTable;
				tableNavigationObject.RowsCount = await DataProfiler.GetTableRowCounterAsync(navTable.TableName, navTable.TableSchema, connectedDatabaseRow, profilingCancellationTokenSource.Token);
				foreach (ColumnNavigationObject column in navTable.Columns)
				{
					column.RowsCount = navTable.RowsCount;
				}
				bool tableShouldBeProfiled = tablesSelector(navTable);
				IEnumerable<ColumnNavigationObject> tableColumnsToBeProfiled = navTable.Columns.Where((ColumnNavigationObject c) => columnsSelector(c));
				if (autoSaveProfiling && tableShouldBeProfiled)
				{
					await Task.Run(delegate
					{
						SaveRowCountToRepository(navTable.TableId, navTable.RowsCount);
					});
				}
				if (!navTable.RowsCount.HasValue || navTable.RowsCount == 0)
				{
					if (tableShouldBeProfiled)
					{
						profilingProgress.SkipTableProfiling();
					}
					if (tableColumnsToBeProfiled.Any())
					{
						profilingProgress.SkipColumnsProfiling(tableColumnsToBeProfiled.Count());
					}
					continue;
				}
				if (tableShouldBeProfiled)
				{
					await ProfileNavTableAsync(navTable, profilingType, commandsSet, profilingProgress);
				}
				foreach (ColumnNavigationObject navColumn in tableColumnsToBeProfiled)
				{
					if (profilingCancellationTokenSource.IsCancellationRequested)
					{
						break;
					}
					await ProfileNavColumnAsync(navColumn, profilingType, commandsSet, databaseTypeEnum, profilingProgress);
					if (autoSaveProfiling)
					{
						await Task.Run(delegate
						{
							SaveNavColumnAfterProfiling(navColumn, profilingType);
						});
					}
				}
			}
			catch (OperationCanceledException)
			{
			}
			catch (Exception exception)
			{
				StaticData.CrashedDatabaseType = DatabaseRow?.Type;
				StaticData.CrashedDBMSVersion = DatabaseRow?.DbmsVersion;
				GeneralExceptionHandling.Handle(exception, HandlingMethodEnumeration.HandlingMethod.LogInErrorLog, this);
				StaticData.ClearDatabaseInfoForCrashes();
			}
		}
		int successfullyProfiledColumns = columnsToProfile.Where((ColumnNavigationObject x) => !x.ErrorOccurred && x.ExactCompletion > 0.0).Count();
		int num = (from x in navigationUserControl.AllNavTables
			where x.Columns.Where((ColumnNavigationObject c) => c.ProfileCheckbox).Any()
			where x.Columns.Any((ColumnNavigationObject c) => !c.ErrorOccurred && c.ExactCompletion > 0.0)
			select x).Count();
		string status = (profilingCancellationTokenSource.IsCancellationRequested ? "CANCELED" : ((num <= 0) ? "FAILED" : "FINISHED"));
		string duration = ((int)(DateTime.UtcNow - startTime).TotalSeconds).ToString();
		DataProfilingTrackingHelper.TrackDataProfilingFinished(status, tablesToProfileCounter, columnsToProfile.Count(), num, successfullyProfiledColumns, DataProfilingTrackingHelper.GetProfliledColumnsDatatypesJson(columnsToProfile), duration);
		SetProfilingEndVisibility(profilingType);
		if (!formIsClosed)
		{
			if (!formIsClosing)
			{
				string message = (profilingCancellationTokenSource.IsCancellationRequested ? "Cancelling profiling ended successfully" : ((num <= 0) ? "Profiling ended unsuccessfully" : "Profiling ended successfully"));
				GeneralMessageBoxesHandling.Show(message, "Profiling ended", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
			}
			profileprogressBarControl.EditValue = 0;
			RefreshNavigationTreeList();
		}
	}

	private bool UseAutoSaveDuringProfiling()
	{
		if (!IsSavingPossible())
		{
			return false;
		}
		return GeneralMessageBoxesHandling.Show("Bulk Profiling of multiple objects can take a while." + Environment.NewLine + "Would you like to automatically save data as you go?", "Bulk Profiling", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, this).DialogResult == DialogResult.Yes;
	}

	public async Task ProfileNavTableAsync(TableNavigationObject navTable, ProfilingType profilingType, CommandsSet commandsSet, ProfilingProgress profilingProgress)
	{
		try
		{
			profilingProgress.ObjectProfilingStarted(navTable);
			if (profilingType == ProfilingType.Full || profilingType == ProfilingType.Values)
			{
				await DataProfiler.ProfileTableAsync(navTable, profilingCancellationTokenSource, commandsSet);
				profilingProgress.ObjectProfilingStepEnded();
				RefreshViewIfFocusedObject(navTable);
			}
		}
		catch (AggregateException)
		{
			navTable.ErrorOccurred = false;
		}
		catch (OperationCanceledException)
		{
			navTable.ErrorOccurred = false;
		}
		catch (Exception ex3)
		{
			ProcessException(ex3);
		}
		finally
		{
			profilingProgress.ObjectProfilingEnded();
		}
	}

	public async Task ProfileNavColumnAsync(ColumnNavigationObject navColumn, ProfilingType profilingType, CommandsSet commandsSet, ProfilingDatabaseTypeEnum.ProfilingDatabaseType databaseType, ProfilingProgress profilingProgress)
	{
		int tabledId = navColumn.TableId;
		double rememberedCompletion = navColumn.ExactCompletion;
		try
		{
			profilingProgress.ObjectProfilingStarted(navColumn);
			ProfiledDataModel profiledDataModel = await DataProfiler.GetColumnProfiledDataModelAsync(navColumn, commandsSet, databaseType, profilingCancellationTokenSource, profilingProgress, profilingType);
			if (profiledDataModel != null)
			{
				ProfiledDataModel profiledDataModel2 = profiledDataModels[tabledId].Where((ProfiledDataModel r) => r.ColumnId == navColumn.ColumnId).FirstOrDefault();
				if (profiledDataModel2 == null)
				{
					profiledDataModels[tabledId].Add(profiledDataModel);
				}
				else
				{
					profiledDataModel2.UpdateProfilingModel(profiledDataModel);
					profiledDataModel = profiledDataModel2;
				}
				navColumn.UpdateProfilingData(profiledDataModel);
				if (string.IsNullOrEmpty(DataTypeChecker.GetProfilingDataType(navColumn.DataType, databaseType)))
				{
					DataProfiler.GuessNavColumnProfilingDataType(profiledDataModel, navColumn);
				}
				navColumn.Column.SetTextForSparkLine();
				navColumn.TextForSparkline = navColumn?.Column?.TextForSparkLine;
				navColumn.SetValuesListRowsCount();
				RefreshViewIfFocusedObject(navColumn);
			}
		}
		catch (AggregateException)
		{
			navColumn.ExactCompletion = rememberedCompletion;
			navColumn.ErrorOccurred = false;
		}
		catch (OperationCanceledException)
		{
			navColumn.ExactCompletion = rememberedCompletion;
			navColumn.ErrorOccurred = false;
		}
		catch (Exception ex3)
		{
			ProcessException(ex3);
			navColumn.ExactCompletion = rememberedCompletion;
		}
		finally
		{
			profilingProgress.ObjectProfilingEnded();
		}
	}

	private void ProcessException(Exception ex)
	{
		StaticData.CrashedDatabaseType = DatabaseRow?.Type;
		StaticData.CrashedDBMSVersion = DatabaseRow?.DbmsVersion;
		GeneralExceptionHandling.Handle(ex, HandlingMethodEnumeration.HandlingMethod.LogInErrorLog);
		StaticData.ClearDatabaseInfoForCrashes();
	}

	private ProfilingProgress CreateProfilingProgress()
	{
		ProfilingProgress profilingProgress = new ProfilingProgress();
		profileprogressBarControl.EditValue = 0;
		profilingProgress.ProgressChanged += delegate(object _, (double, INavigationObject) progress)
		{
			try
			{
				if (!formIsClosed)
				{
					navigationUserControl.RefreshNode(progress.Item2);
					profileprogressBarControl.EditValue = progress.Item1;
				}
			}
			catch (Exception)
			{
			}
		};
		return profilingProgress;
	}

	private bool AreThereAnyChangesMadeInThisProfilingSession()
	{
		bool noChangesInValues = valuesUserControl.NoChangesInValues;
		return profiledDataModels.Sum((KeyValuePair<int, List<ProfiledDataModel>> x) => x.Value.Count) == 0 && noChangesInValues;
	}

	private async void ClearAllDataSelectedColumnsRibbonBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			DateTime startTime = DateTime.UtcNow;
			navigationUserControl.PostEditor();
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			foreach (INavigationObject item in navigationUserControl.AllNavObjects.Where((INavigationObject x) => x.ProfileCheckbox))
			{
				if (item is ColumnNavigationObject navColumn)
				{
					ClearNavColumnAllProfiling(navColumn);
				}
				else if (item is TableNavigationObject navTable)
				{
					ClearNavTableAllProfiling(navTable);
				}
			}
			await RefreshAfterClearingAsync();
			string duration = ((int)(DateTime.UtcNow - startTime).TotalSeconds).ToString();
			DataProfilingTrackingHelper.TrackDataProfilingCleared("ALL", duration);
		}
		catch (Exception ex)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			DataProfiler.ShowErrorMessageBox(ex, this);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private async void ClearDistributionSelectedColumnsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			DateTime startTime = DateTime.UtcNow;
			navigationUserControl.PostEditor();
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			foreach (ColumnNavigationObject item in navigationUserControl.AllNavColumns.Where((ColumnNavigationObject x) => x.ProfileCheckbox))
			{
				ClearNavColumnDistribution(item);
			}
			await RefreshAfterClearingAsync();
			string duration = ((int)(DateTime.UtcNow - startTime).TotalSeconds).ToString();
			DataProfilingTrackingHelper.TrackDataProfilingCleared("DISTRIBUTION", duration);
		}
		catch (Exception ex)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			DataProfiler.ShowErrorMessageBox(ex, this);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private async void ClearValuesSelectedColumnsRibbonBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			DateTime startTime = DateTime.UtcNow;
			navigationUserControl.PostEditor();
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			foreach (INavigationObject item in navigationUserControl.AllNavObjects.Where((INavigationObject x) => x.ProfileCheckbox))
			{
				if (item is ColumnNavigationObject navColumn)
				{
					ClearNavColumnValueSection(navColumn);
				}
				else if (item is TableNavigationObject tableNavigationObject)
				{
					tableNavigationObject.ClearValueProfiling();
				}
			}
			await RefreshAfterClearingAsync();
			string duration = ((int)(DateTime.UtcNow - startTime).TotalSeconds).ToString();
			DataProfilingTrackingHelper.TrackDataProfilingCleared("VALUES", duration);
		}
		catch (Exception ex)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			DataProfiler.ShowErrorMessageBox(ex, this);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private async void ClearAllProfilingAllRibbonBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (GeneralMessageBoxesHandling.Show("You are about to delete all Data Profiling data for all objects in Tree View, regardless of selection. Are you sure?", "Heads up!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, this).DialogResult != DialogResult.Yes)
		{
			return;
		}
		try
		{
			DateTime startTime = DateTime.UtcNow;
			navigationUserControl.PostEditor();
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			foreach (INavigationObject allNavObject in navigationUserControl.AllNavObjects)
			{
				if (allNavObject is ColumnNavigationObject navColumn)
				{
					ClearNavColumnAllProfiling(navColumn);
				}
				else if (allNavObject is TableNavigationObject navTable)
				{
					ClearNavTableAllProfiling(navTable);
				}
			}
			profiledDataModels.ForEach(delegate(KeyValuePair<int, List<ProfiledDataModel>> x)
			{
				x.Value.Clear();
			});
			valuesUserControl.ClearAllProfiling();
			await RefreshAfterClearingAsync();
			string duration = ((int)(DateTime.UtcNow - startTime).TotalSeconds).ToString();
			DataProfilingTrackingHelper.TrackDataProfilingCleared("ALL", duration);
		}
		catch (Exception ex)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			DataProfiler.ShowErrorMessageBox(ex, this);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	public async Task<bool> ClearAllTableProfilingAsync(int tableId)
	{
		TableNavigationObject tableNavigationObject = GetTableNavigationObject(tableId);
		if (tableNavigationObject == null)
		{
			return false;
		}
		await ClearAllTableProfilingAsync(tableNavigationObject);
		return true;
	}

	public async Task ClearAllTableProfilingAsync(TableNavigationObject navTable)
	{
		try
		{
			DateTime startTime = DateTime.UtcNow;
			navigationUserControl.PostEditor();
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			ClearNavTableAllProfiling(navTable);
			foreach (ColumnNavigationObject column in navTable.Columns)
			{
				ClearNavColumnAllProfiling(column);
			}
			profiledDataModels[navTable.TableId].Clear();
			await RefreshAfterClearingAsync();
			string duration = ((int)(DateTime.UtcNow - startTime).TotalSeconds).ToString();
			DataProfilingTrackingHelper.TrackDataProfilingCleared("ALL", duration);
		}
		catch (Exception ex)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			DataProfiler.ShowErrorMessageBox(ex, this);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	public async Task<bool> ClearAllColumnProfilingAsync(int columnId)
	{
		ColumnNavigationObject columnNavigationObject = GetColumnNavigationObject(columnId);
		if (columnNavigationObject == null)
		{
			return false;
		}
		await ClearAllColumnProfilingAsync(columnNavigationObject);
		return true;
	}

	public async Task ClearAllColumnProfilingAsync(ColumnNavigationObject navColumn)
	{
		try
		{
			DateTime startTime = DateTime.UtcNow;
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			ClearNavColumnAllProfiling(navColumn);
			await RefreshAfterClearingAsync();
			string duration = ((int)(DateTime.UtcNow - startTime).TotalSeconds).ToString();
			DataProfilingTrackingHelper.TrackDataProfilingCleared("ALL", duration);
		}
		catch (Exception ex)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			DataProfiler.ShowErrorMessageBox(ex, this);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private void ClearNavColumnAllProfiling(ColumnNavigationObject navColumn)
	{
		if (navColumn == null || navColumn.Column == null)
		{
			return;
		}
		DB.DataProfiling.RemoveAllProfilingForSingleColumn(navColumn.Column);
		navColumn.ClearAllProfiling();
		valuesUserControl.SetColumnValuesClearedOrSaved(navColumn.ColumnId);
		if (profiledDataModels.ContainsKey(navColumn.TableId))
		{
			List<ProfiledDataModel> list = profiledDataModels[navColumn.TableId];
			ProfiledDataModel profiledDataModel = list.Where((ProfiledDataModel p) => p.ColumnId == navColumn.ColumnId).FirstOrDefault();
			if (profiledDataModel != null)
			{
				list.Remove(profiledDataModel);
			}
		}
	}

	private void ClearNavColumnDistribution(ColumnNavigationObject navColumn)
	{
		if (navColumn == null || navColumn.Column == null)
		{
			return;
		}
		DB.DataProfiling.RemoveDistributionProfilingForSingleColumn(navColumn.Column);
		navColumn.ClearDistiributionProfiling();
		if (!profiledDataModels.ContainsKey(navColumn.TableId))
		{
			return;
		}
		List<ProfiledDataModel> list = profiledDataModels[navColumn.TableId];
		ProfiledDataModel profiledDataModel = list.Where((ProfiledDataModel p) => p.ColumnId == navColumn.ColumnId).FirstOrDefault();
		if (profiledDataModel != null)
		{
			profiledDataModel.IsRowDistributionProfiling = false;
			if (!profiledDataModel.IsValuesProfiling)
			{
				list.Remove(profiledDataModel);
			}
		}
	}

	private void ClearNavColumnValueSection(ColumnNavigationObject navColumn)
	{
		if (navColumn == null || navColumn.Column == null)
		{
			return;
		}
		DB.DataProfiling.RemoveValueSectionProfilingForSingleColumn(navColumn.Column);
		navColumn.ClearValueProfiling();
		valuesUserControl.SetColumnValuesClearedOrSaved(navColumn.ColumnId);
		if (!profiledDataModels.ContainsKey(navColumn.TableId))
		{
			return;
		}
		List<ProfiledDataModel> list = profiledDataModels[navColumn.TableId];
		ProfiledDataModel profiledDataModel = list.Where((ProfiledDataModel p) => p.ColumnId == navColumn.ColumnId).FirstOrDefault();
		if (profiledDataModel != null)
		{
			profiledDataModel.IsValuesProfiling = false;
			if (!profiledDataModel.IsRowDistributionProfiling)
			{
				list.Remove(profiledDataModel);
			}
		}
	}

	private void ClearNavTableAllProfiling(TableNavigationObject navTable)
	{
		DB.DataProfiling.RemoveTableRowCountFromRepository(navTable.TableId);
		navTable.ClearAllProfiling();
	}

	private async Task RefreshAfterClearingAsync()
	{
		RefreshNavigationTreeList();
		SetPresentationFocusOnSpecificColumn();
		RefreshRibbonButtonsEnability();
	}

	private void AddInfoImageToProfilingGroup()
	{
		aboveRibbonControl.AddImageToGroupCaption(profilingRibbonPageGroup.Name, Resources.question_16, "Profiling can cause usage of your database");
	}

	private void SetButtonsVisibilityBasedOnKillerSwitch()
	{
		DB.DataProfiling.SetSelectKillerSwitchMode();
		if (DB.DataProfiling.ProfilingEnabled)
		{
			saveAllDatabarButtonItem.Visibility = BarItemVisibility.Always;
			saveDistributionBarButtonItem.Visibility = BarItemVisibility.Always;
			saveValuesBarButtonItem.Visibility = BarItemVisibility.Always;
			saveAllSimpleButtonLayoutControlItem.Visibility = LayoutVisibility.Always;
			learnMoreBarStaticItem.Visibility = BarItemVisibility.Never;
			savingIsDisabledBarStaticItem.Visibility = BarItemVisibility.Never;
		}
		else if (DB.DataProfiling.ProfilingNoSave)
		{
			saveAllDatabarButtonItem.Visibility = BarItemVisibility.Never;
			saveDistributionBarButtonItem.Visibility = BarItemVisibility.Never;
			saveValuesBarButtonItem.Visibility = BarItemVisibility.Never;
			saveAllSimpleButtonLayoutControlItem.Visibility = LayoutVisibility.Never;
			learnMoreBarStaticItem.Visibility = BarItemVisibility.Always;
			learnMoreBarStaticItem.Caption = "<href=" + Links.DataProfilingConfiguration + ">" + $"<color={SkinsManager.CurrentSkin.DataedoColor.R}," + $"{SkinsManager.CurrentSkin.DataedoColor.G}," + $"{SkinsManager.CurrentSkin.DataedoColor.B}>" + "Learn more</color></href> about enabling the save option.";
			savingIsDisabledBarStaticItem.Visibility = BarItemVisibility.Always;
		}
	}

	public void RefreshRibbonButtonsEnability()
	{
		bool isThereAnyProfilingData = navigationUserControl.IsThereAnyProfilingData;
		fullProfileBarButtonItem.Enabled = true;
		profileDistributionBarButtonItem.Enabled = true;
		profileValuesBarButtonItem.Enabled = true;
		if (isThereAnyProfilingData)
		{
			clearAllDataBarButtonItem.Enabled = true;
			clearDistributionBarButtonItem.Enabled = true;
			clearValuesBarButtonItem.Enabled = true;
			clearAllProfilingBarButtonItem.Enabled = true;
			if (AreThereAnyChangesMadeInThisProfilingSession())
			{
				saveAllDatabarButtonItem.Enabled = false;
				saveDistributionBarButtonItem.Enabled = false;
				saveValuesBarButtonItem.Enabled = false;
			}
			else
			{
				saveAllDatabarButtonItem.Enabled = true;
				saveDistributionBarButtonItem.Enabled = true;
				saveValuesBarButtonItem.Enabled = true;
			}
		}
		else
		{
			clearAllDataBarButtonItem.Enabled = false;
			clearDistributionBarButtonItem.Enabled = false;
			clearValuesBarButtonItem.Enabled = false;
			clearAllProfilingBarButtonItem.Enabled = false;
			saveAllDatabarButtonItem.Enabled = false;
			saveDistributionBarButtonItem.Enabled = false;
			saveValuesBarButtonItem.Enabled = false;
		}
	}

	private bool ButtonHasProfilingCaption(BarButtonItem barButtonItem)
	{
		if (barButtonItem == fullProfileBarButtonItem)
		{
			return barButtonItem.Caption == "Full Profiling (selected objects)";
		}
		if (barButtonItem == profileDistributionBarButtonItem)
		{
			return barButtonItem.Caption == "Profile Distribution (selected objects)";
		}
		if (barButtonItem == profileValuesBarButtonItem)
		{
			return barButtonItem.Caption == "Profile Values (selected objects)";
		}
		return false;
	}

	private void RestoreButtonProfilingCaption(BarButtonItem barButtonItem)
	{
		if (barButtonItem == fullProfileBarButtonItem)
		{
			fullProfileBarButtonItem.ImageOptions.Image = fullProfilingImage;
			fullProfileBarButtonItem.ImageOptions.LargeImage = fullProfilingLargeImage;
			fullProfileBarButtonItem.Caption = "Full Profiling (selected objects)";
		}
		else if (barButtonItem == profileDistributionBarButtonItem)
		{
			profileDistributionBarButtonItem.ImageOptions.Image = profileDistributionImage;
			profileDistributionBarButtonItem.ImageOptions.LargeImage = profileDistributionLargeImage;
			profileDistributionBarButtonItem.Caption = "Profile Distribution (selected objects)";
		}
		else if (barButtonItem == profileValuesBarButtonItem)
		{
			profileValuesBarButtonItem.ImageOptions.Image = profileValuesImage;
			profileValuesBarButtonItem.ImageOptions.LargeImage = profileValuesLargeImage;
			profileValuesBarButtonItem.Caption = "Profile Values (selected objects)";
		}
	}

	private void ChangeProfilingButtonToCancel(BarButtonItem barButtonItem)
	{
		barButtonItem.ImageOptions.Image = cancelProfilingImage;
		barButtonItem.ImageOptions.LargeImage = cancelProfilingLargeImage;
		barButtonItem.Caption = "Cancel profiling";
	}

	private BarButtonItem GetProfilingRibbonButton(ProfilingType profilingType)
	{
		return profilingType switch
		{
			ProfilingType.Full => fullProfileBarButtonItem, 
			ProfilingType.Distribution => profileDistributionBarButtonItem, 
			ProfilingType.Values => profileValuesBarButtonItem, 
			_ => null, 
		};
	}

	private void SetProfilingStartVisibility(ProfilingType profilingType)
	{
		navigationUserControl.DisablePopupMenuButtons();
		centerSectionUserControl.DisableCentralProfilingButton();
		fullProfileBarButtonItem.Enabled = false;
		profileDistributionBarButtonItem.Enabled = false;
		profileValuesBarButtonItem.Enabled = false;
		GetProfilingRibbonButton(profilingType).Enabled = true;
		saveAllSimpleButtonLayoutControlItem.Enabled = false;
		profileprogressBarLayoutControlItem.Visibility = LayoutVisibility.Always;
		clearAllDataBarButtonItem.Enabled = false;
		clearDistributionBarButtonItem.Enabled = false;
		clearValuesBarButtonItem.Enabled = false;
		clearAllProfilingBarButtonItem.Enabled = false;
		saveAllDatabarButtonItem.Enabled = false;
		saveDistributionBarButtonItem.Enabled = false;
		saveValuesBarButtonItem.Enabled = false;
		ChangeProfilingButtonToCancel(GetProfilingRibbonButton(profilingType));
	}

	private void SetProfilingEndVisibility(ProfilingType profilingType)
	{
		navigationUserControl.EnablePopupMenuButtons();
		centerSectionUserControl.EnableCentralProfilingButton();
		saveAllSimpleButtonLayoutControlItem.Enabled = true;
		profileprogressBarLayoutControlItem.Visibility = LayoutVisibility.Never;
		RestoreButtonProfilingCaption(GetProfilingRibbonButton(profilingType));
		RefreshRibbonButtonsEnability();
	}

	private void LearnMoreBarStaticItem_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		Links.OpenLink(Links.DataProfilingConfiguration);
	}

	private void AboveRibbonControl_ShowCustomizationMenu(object sender, RibbonCustomizationMenuEventArgs e)
	{
		foreach (BarItemLink itemLink in e.CustomizationMenu.ItemLinks)
		{
			if (itemLink.Caption.Equals(BarLocalizer.Active.GetLocalizedString(BarString.RibbonToolbarMinimizeRibbon), StringComparison.CurrentCulture))
			{
				itemLink.Visible = false;
				break;
			}
		}
	}

	private void ribbonToolTipController_BeforeShow(object sender, ToolTipControllerShowEventArgs e)
	{
		if (e.SelectedObject == learnMoreBarStaticItem.Links[0] || e.SelectedObject == savingIsDisabledBarStaticItem.Links[0])
		{
			e.Show = false;
		}
	}

	public DataProfilingForm()
	{
		InitializeComponent();
		profiledDataModels = new Dictionary<int, List<ProfiledDataModel>>();
		columns = new Dictionary<int, List<ColumnProfiledDataObject>>();
		profilingCancellationTokenSource = new CancellationTokenSource();
		Init();
	}

	private void Init()
	{
		SetProgressBarStyle();
		valuesUserControl.Init(splashScreenManager, infoUserControl, infoPanelLayoutControlItem);
		navigationUserControl.Init();
		SetCurrentDataProfilingFormForApplication();
		AddInfoImageToProfilingGroup();
		navigationUserControl.FocusedNavigationObjectChanged += NavigationUserControl_FocusedNavigationObjectChanged;
	}

	private void NavigationUserControl_FocusedNavigationObjectChanged(object sender, INavigationObject e)
	{
		FocusedNavigationObject = e;
		valuesUserControl.SetFocusedNavObject(e);
		centerSectionUserControl.SetFocusedNavObject(e);
		SetPresentationFocusOnSpecificColumn();
	}

	private void SetProgressBarStyle()
	{
		profileprogressBarControl.Properties.LookAndFeel.SetStyle(LookAndFeelStyle.Style3D, useWindowsXPTheme: false, useDefaultLookAndFeel: false);
	}

	private void SetCurrentDataProfilingFormForApplication()
	{
		DB.DataProfiling.DataProfiling = this;
	}

	public void SetParameters(IEnumerable<TableSimpleData> tables, int databaseId, string databaseTitle, MetadataEditorUserControl metadataEditorUserControl, SharedDatabaseTypeEnum.DatabaseType? databaseType, int? columnId = null)
	{
		if (!DataProfilingUtils.CanViewDataProfilingForms(databaseType, this))
		{
			Close();
			return;
		}
		SetButtonsVisibilityBasedOnKillerSwitch();
		SetArguments(tables, databaseId, databaseType, metadataEditorUserControl);
		LoadColumnsFromRepository();
		this.tables.RemoveAll((TableSimpleData x) => !columns.ContainsKey(x.TableId));
		if (columns.Count == 0)
		{
			Close();
			return;
		}
		foreach (TableSimpleData table in this.tables)
		{
			profiledDataModels.Add(table.TableId, new List<ProfiledDataModel>());
		}
		navigationUserControl.LoadNavigationData(this.tables);
		if (columnId.HasValue)
		{
			navigationUserControl.SelectOnlyOneColumnForProfiling(columnId.Value);
			navigationUserControl.SetFocusOnSpecificColumn(columnId.Value);
		}
		RefreshRibbonButtonsEnability();
		SetButtonsVisibilityBasedOnKillerSwitch();
		if (this.tables.Count == 1)
		{
			DataProfilingTrackingHelper.TrackDataProfilingSingleTableOpen();
		}
		else if (this.tables.Count > 1)
		{
			DataProfilingTrackingHelper.TrackDataProfilingMultipleTableOpen(this.tables.Count);
		}
		navigationUserControl.SetDatabaseTitleAndImage(databaseTitle, databaseType);
		Show();
	}

	private void SetArguments(IEnumerable<TableSimpleData> tables, int databaseId, SharedDatabaseTypeEnum.DatabaseType? databaseType, MetadataEditorUserControl metadataEditorUserControl)
	{
		this.tables = tables.ToList();
		this.databaseId = databaseId;
		centerSectionUserControl.SetProfilingDatabaseType(databaseType);
		valuesUserControl.SetProfilingDatabaseType(databaseType);
		databaseTypeOriginal = databaseType;
		this.metadataEditorUserControl = metadataEditorUserControl;
	}

	private void LoadColumnsFromRepository()
	{
		List<TableSimpleData> list = new List<TableSimpleData>();
		foreach (TableSimpleData table in tables)
		{
			try
			{
				List<ColumnProfiledDataObject> list2 = DB.DataProfiling.SelectColumnsProfilingData(table.TableId);
				if (list2.Count == 0)
				{
					list.Add(table);
				}
				else
				{
					columns.Add(table.TableId, list2);
				}
			}
			catch (Exception ex)
			{
				DataProfiler.ShowErrorMessageBox(ex, this);
			}
		}
		if (list.Any())
		{
			string message;
			if (list.Count == 1)
			{
				TableSimpleData tableSimpleData = list.First();
				message = "The <i>" + tableSimpleData.TableName + "</i> " + tableSimpleData.ObjectType.ToString().ToLower() + " has no valid columns. It will be removed from this profiling session.";
			}
			else
			{
				message = $"{list.Count} out of {tables.Count} " + "objects have no valid columns. They will be removed from this profiling session.";
			}
			GeneralMessageBoxesHandling.Show(message, "No valid columns", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
		}
	}

	public DatabaseRow ReturnConnectedDatabaseRow()
	{
		DatabaseRow = DataProfiler.ReturnConnectedDatabaseRow(DatabaseRow, databaseId, databaseTypeOriginal, this, splashScreenManager);
		DataProfiler.DatabaseRow = DatabaseRow;
		return DatabaseRow;
	}

	public ProfiledDataModel GetColumnProfileModel(ColumnNavigationObject navColumn)
	{
		return profiledDataModels[navColumn.TableId].Where((ProfiledDataModel c) => c.ColumnId == navColumn.ColumnId).FirstOrDefault();
	}

	public List<ColumnProfiledDataObject> GetTableAllColumnsProfileDataObjects(int tableId)
	{
		return columns[tableId];
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Escape)
		{
			Close();
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void ProfileProgressBarControl_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		string text2 = (e.DisplayText = DataProfilingStringFormatter.FormatFloat1Values(Convert.ToDouble(e.Value, CultureInfo.InvariantCulture)) + "%");
	}

	private void DataProfilingForm_Shown(object sender, EventArgs e)
	{
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
		{
			Hide();
			using (UpgradeDataProfilingForm upgradeDataProfilingForm = new UpgradeDataProfilingForm())
			{
				upgradeDataProfilingForm.ShowDialog();
			}
			Close();
		}
		SetLeftSplitterForNoNavigationScroll();
	}

	private void SetLeftSplitterForNoNavigationScroll()
	{
		leftSectionNavigationLayoutControlItem.Width = navigationUserControl.CalculatedWidth;
		int num = navigationUserControl.WidthWithoutHorizontalScroll + leftSectionNavigationLayoutControlItem.Padding.Width;
		leftSectionNavigationLayoutControlItem.Width = num;
	}

	private void QuitWithoutSavingSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void DataProfilingForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		bool num = AreThereAnyChangesMadeInThisProfilingSession();
		bool flag = true;
		if (num || DB.DataProfiling.ProfilingNoSave)
		{
			flag = false;
		}
		if (flag)
		{
			DialogResult? dialogResult = GeneralMessageBoxesHandling.Show("You will lose your profiling data. Do you want to close the window?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
			if (dialogResult == DialogResult.None)
			{
				dialogResult = DialogResult.No;
			}
			e.Cancel = dialogResult == DialogResult.No;
			if (dialogResult == DialogResult.Yes)
			{
				formIsClosing = true;
				profilingCancellationTokenSource.Cancel();
			}
		}
		profilingCancellationTokenSource?.Cancel();
		foreach (TableSimpleData table in tables)
		{
			metadataEditorUserControl?.RefreshProfilingInVisibleTableSummaryControl(table.TableId);
		}
	}

	private void DataProfilingForm_FormClosed(object sender, FormClosedEventArgs e)
	{
		formIsClosed = true;
		metadataEditorUserControl.RefreshSparklinesForDataProfiling();
		DB.DataProfiling.DataProfiling = null;
		DataProfiler.DatabaseRow = null;
		ErrorLog.FlushBuffer();
	}

	private void LeftCenterSplitterItem_DoubleClick(object sender, EventArgs e)
	{
		SetLeftSplitterForNoNavigationScroll();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.DataProfiling.DataProfilingForm));
		this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true);
		this.wholeWindowLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.infoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.navigationUserControl = new Dataedo.App.DataProfiling.DataProfilingUserControls.NavigationUserControl();
		this.centerSectionUserControl = new Dataedo.App.DataProfiling.DataProfilingUserControls.CenterSectionUserControl();
		this.valuesUserControl = new Dataedo.App.DataProfiling.DataProfilingUserControls.ValuesUserControl();
		this.quitWithoutSavingSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveAllSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.profileprogressBarControl = new DevExpress.XtraEditors.ProgressBarControl();
		this.aboveRibbonControl = new Dataedo.App.UserControls.OverriddenControls.RibbonControlWithGroupIcons();
		this.fullProfileBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.clearAllDataBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.clearDistributionBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.clearValuesBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.clearAllProfilingBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.saveAllDatabarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.saveDistributionBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.saveValuesBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.profileDistributionBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.profileValuesBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.savingIsDisabledBarStaticItem = new DevExpress.XtraBars.BarStaticItem();
		this.learnMoreBarStaticItem = new DevExpress.XtraBars.BarStaticItem();
		this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
		this.profilingRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.clearRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.saveRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.ribbonToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.rootLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.aboveRibbondLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.profileprogressBarLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.saveAllSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.quitWithoutSavingSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem15 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem33 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.LeftCenterSplitterItem = new DevExpress.XtraLayout.SplitterItem();
		this.centerRightSplitterItem = new DevExpress.XtraLayout.SplitterItem();
		this.rightSectionLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.centerSectionLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.leftSectionNavigationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.infoPanelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		((System.ComponentModel.ISupportInitialize)this.wholeWindowLayoutControl).BeginInit();
		this.wholeWindowLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.profileprogressBarControl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.aboveRibbonControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rootLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.aboveRibbondLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.profileprogressBarLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveAllSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.quitWithoutSavingSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem15).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem33).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.LeftCenterSplitterItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.centerRightSplitterItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rightSectionLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.centerSectionLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.leftSectionNavigationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.infoPanelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.behaviorManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		base.SuspendLayout();
		this.wholeWindowLayoutControl.AllowCustomization = false;
		this.wholeWindowLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.wholeWindowLayoutControl.Controls.Add(this.infoUserControl);
		this.wholeWindowLayoutControl.Controls.Add(this.navigationUserControl);
		this.wholeWindowLayoutControl.Controls.Add(this.centerSectionUserControl);
		this.wholeWindowLayoutControl.Controls.Add(this.valuesUserControl);
		this.wholeWindowLayoutControl.Controls.Add(this.quitWithoutSavingSimpleButton);
		this.wholeWindowLayoutControl.Controls.Add(this.saveAllSimpleButton);
		this.wholeWindowLayoutControl.Controls.Add(this.profileprogressBarControl);
		this.wholeWindowLayoutControl.Controls.Add(this.aboveRibbonControl);
		this.wholeWindowLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.wholeWindowLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.wholeWindowLayoutControl.Name = "wholeWindowLayoutControl";
		this.wholeWindowLayoutControl.Padding = new System.Windows.Forms.Padding(2);
		this.wholeWindowLayoutControl.Root = this.rootLayoutControlGroup;
		this.wholeWindowLayoutControl.Size = new System.Drawing.Size(1311, 740);
		this.wholeWindowLayoutControl.TabIndex = 5;
		this.wholeWindowLayoutControl.Text = "layoutControl1";
		this.infoUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.infoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.infoUserControl.Description = null;
		this.infoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.infoUserControl.Image = (System.Drawing.Image)resources.GetObject("infoUserControl.Image");
		this.infoUserControl.Location = new System.Drawing.Point(316, 120);
		this.infoUserControl.MaximumSize = new System.Drawing.Size(0, 32);
		this.infoUserControl.MinimumSize = new System.Drawing.Size(0, 32);
		this.infoUserControl.Name = "infoUserControl";
		this.infoUserControl.Size = new System.Drawing.Size(985, 32);
		this.infoUserControl.TabIndex = 23;
		this.navigationUserControl.BackColor = System.Drawing.Color.Transparent;
		this.navigationUserControl.Location = new System.Drawing.Point(2, 112);
		this.navigationUserControl.MinimumSize = new System.Drawing.Size(300, 0);
		this.navigationUserControl.Name = "navigationUserControl";
		this.navigationUserControl.Size = new System.Drawing.Size(300, 562);
		this.navigationUserControl.TabIndex = 21;
		this.centerSectionUserControl.BackColor = System.Drawing.Color.Transparent;
		this.centerSectionUserControl.Location = new System.Drawing.Point(316, 156);
		this.centerSectionUserControl.Name = "centerSectionUserControl";
		this.centerSectionUserControl.Size = new System.Drawing.Size(629, 518);
		this.centerSectionUserControl.TabIndex = 20;
		this.valuesUserControl.BackColor = System.Drawing.Color.Transparent;
		this.valuesUserControl.Location = new System.Drawing.Point(959, 156);
		this.valuesUserControl.MinimumSize = new System.Drawing.Size(350, 0);
		this.valuesUserControl.Name = "valuesUserControl";
		this.valuesUserControl.Size = new System.Drawing.Size(350, 518);
		this.valuesUserControl.TabIndex = 19;
		this.quitWithoutSavingSimpleButton.Location = new System.Drawing.Point(1213, 706);
		this.quitWithoutSavingSimpleButton.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
		this.quitWithoutSavingSimpleButton.MaximumSize = new System.Drawing.Size(150, 22);
		this.quitWithoutSavingSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.quitWithoutSavingSimpleButton.Name = "quitWithoutSavingSimpleButton";
		this.quitWithoutSavingSimpleButton.Size = new System.Drawing.Size(86, 22);
		this.quitWithoutSavingSimpleButton.StyleController = this.wholeWindowLayoutControl;
		this.quitWithoutSavingSimpleButton.TabIndex = 15;
		this.quitWithoutSavingSimpleButton.Text = "Close";
		this.quitWithoutSavingSimpleButton.Click += new System.EventHandler(QuitWithoutSavingSimpleButton_Click);
		this.saveAllSimpleButton.Location = new System.Drawing.Point(1123, 706);
		this.saveAllSimpleButton.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
		this.saveAllSimpleButton.MaximumSize = new System.Drawing.Size(150, 22);
		this.saveAllSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.saveAllSimpleButton.Name = "saveAllSimpleButton";
		this.saveAllSimpleButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
		this.saveAllSimpleButton.Size = new System.Drawing.Size(86, 22);
		this.saveAllSimpleButton.StyleController = this.wholeWindowLayoutControl;
		this.saveAllSimpleButton.TabIndex = 17;
		this.saveAllSimpleButton.Text = "Save";
		this.saveAllSimpleButton.Click += new System.EventHandler(SaveAllSimpleButton_Click);
		this.profileprogressBarControl.Location = new System.Drawing.Point(2, 688);
		this.profileprogressBarControl.MenuManager = this.aboveRibbonControl;
		this.profileprogressBarControl.Name = "profileprogressBarControl";
		this.profileprogressBarControl.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(148, 183, 227);
		this.profileprogressBarControl.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.profileprogressBarControl.Properties.EndColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.profileprogressBarControl.Properties.PercentView = false;
		this.profileprogressBarControl.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.profileprogressBarControl.Properties.ShowTitle = true;
		this.profileprogressBarControl.Properties.StartColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.profileprogressBarControl.Properties.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(ProfileProgressBarControl_Properties_CustomDisplayText);
		this.profileprogressBarControl.ShowProgressInTaskBar = true;
		this.profileprogressBarControl.Size = new System.Drawing.Size(1307, 14);
		this.profileprogressBarControl.StyleController = this.wholeWindowLayoutControl;
		this.profileprogressBarControl.TabIndex = 14;
		this.aboveRibbonControl.Dock = System.Windows.Forms.DockStyle.None;
		this.aboveRibbonControl.ExpandCollapseItem.Id = 0;
		this.aboveRibbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[14]
		{
			this.aboveRibbonControl.ExpandCollapseItem,
			this.aboveRibbonControl.SearchEditItem,
			this.fullProfileBarButtonItem,
			this.clearAllDataBarButtonItem,
			this.clearDistributionBarButtonItem,
			this.clearValuesBarButtonItem,
			this.clearAllProfilingBarButtonItem,
			this.saveAllDatabarButtonItem,
			this.saveDistributionBarButtonItem,
			this.saveValuesBarButtonItem,
			this.profileDistributionBarButtonItem,
			this.profileValuesBarButtonItem,
			this.savingIsDisabledBarStaticItem,
			this.learnMoreBarStaticItem
		});
		this.aboveRibbonControl.Location = new System.Drawing.Point(2, 2);
		this.aboveRibbonControl.Margin = new System.Windows.Forms.Padding(0);
		this.aboveRibbonControl.MaxItemId = 30;
		this.aboveRibbonControl.Name = "aboveRibbonControl";
		this.aboveRibbonControl.OptionsPageCategories.ShowCaptions = false;
		this.aboveRibbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.ribbonPage1 });
		this.aboveRibbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
		this.aboveRibbonControl.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
		this.aboveRibbonControl.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
		this.aboveRibbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
		this.aboveRibbonControl.ShowToolbarCustomizeItem = false;
		this.aboveRibbonControl.Size = new System.Drawing.Size(1307, 102);
		this.aboveRibbonControl.Toolbar.ShowCustomizeItem = false;
		this.aboveRibbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
		this.aboveRibbonControl.ToolTipController = this.ribbonToolTipController;
		this.aboveRibbonControl.ShowCustomizationMenu += new DevExpress.XtraBars.Ribbon.RibbonCustomizationMenuEventHandler(AboveRibbonControl_ShowCustomizationMenu);
		this.fullProfileBarButtonItem.Caption = "Full Profiling (selected objects)";
		this.fullProfileBarButtonItem.Id = 1;
		this.fullProfileBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.all_data_play_16;
		this.fullProfileBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.all_data_play_32;
		this.fullProfileBarButtonItem.LargeWidth = 100;
		this.fullProfileBarButtonItem.Name = "fullProfileBarButtonItem";
		this.fullProfileBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(FullProfilingSelectedObjectsBarButtonItem_ItemClick);
		this.clearAllDataBarButtonItem.Caption = "Clear All Data (selected objects)";
		this.clearAllDataBarButtonItem.Id = 16;
		this.clearAllDataBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.all_data_deleted_16;
		this.clearAllDataBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.all_data_deleted_32;
		this.clearAllDataBarButtonItem.Name = "clearAllDataBarButtonItem";
		this.clearAllDataBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ClearAllDataSelectedColumnsRibbonBarButtonItem_ItemClick);
		this.clearDistributionBarButtonItem.Caption = "Clear Distribution (selected objects)";
		this.clearDistributionBarButtonItem.Id = 17;
		this.clearDistributionBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.distribution_deleted_16;
		this.clearDistributionBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.distribution_deleted_32;
		this.clearDistributionBarButtonItem.Name = "clearDistributionBarButtonItem";
		this.clearDistributionBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ClearDistributionSelectedColumnsBarButtonItem_ItemClick);
		this.clearValuesBarButtonItem.Caption = "Clear Values (selected objects)";
		this.clearValuesBarButtonItem.Id = 18;
		this.clearValuesBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.values_deleted_16;
		this.clearValuesBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.values_deleted_32;
		this.clearValuesBarButtonItem.Name = "clearValuesBarButtonItem";
		this.clearValuesBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ClearValuesSelectedColumnsRibbonBarButtonItem_ItemClick);
		this.clearAllProfilingBarButtonItem.Caption = "Clear whole Profiling";
		this.clearAllProfilingBarButtonItem.Id = 19;
		this.clearAllProfilingBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.clearAllProfilingBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.delete_32;
		this.clearAllProfilingBarButtonItem.Name = "clearAllProfilingBarButtonItem";
		this.clearAllProfilingBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ClearAllProfilingAllRibbonBarButtonItem_ItemClick);
		this.saveAllDatabarButtonItem.Caption = "Save All Data (all objects)";
		this.saveAllDatabarButtonItem.Id = 20;
		this.saveAllDatabarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.all_data_save_16;
		this.saveAllDatabarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.all_data_save_32;
		this.saveAllDatabarButtonItem.Name = "saveAllDatabarButtonItem";
		this.saveAllDatabarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(SaveAllDataAllColumnsRibbonBarButtonItem_ItemClick);
		this.saveDistributionBarButtonItem.Caption = "Save Distribution (all objects)";
		this.saveDistributionBarButtonItem.Id = 21;
		this.saveDistributionBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.distribution_save_16;
		this.saveDistributionBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.distribution_save_32;
		this.saveDistributionBarButtonItem.Name = "saveDistributionBarButtonItem";
		this.saveDistributionBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(SaveDistributionAllColumnsRibbonBarButtonItem_ItemClick);
		this.saveValuesBarButtonItem.Caption = "Save Values (all objects)";
		this.saveValuesBarButtonItem.Id = 22;
		this.saveValuesBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.values_save_16;
		this.saveValuesBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.values_save_32;
		this.saveValuesBarButtonItem.Name = "saveValuesBarButtonItem";
		this.saveValuesBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(SaveValuesSectionAllColumnsRibbonBarButtonItem_ItemClick);
		this.profileDistributionBarButtonItem.Caption = "Profile Distribution (selected objects)";
		this.profileDistributionBarButtonItem.Id = 23;
		this.profileDistributionBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.distribution_play_16;
		this.profileDistributionBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.distribution_play_32;
		this.profileDistributionBarButtonItem.LargeWidth = 100;
		this.profileDistributionBarButtonItem.Name = "profileDistributionBarButtonItem";
		this.profileDistributionBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ProfileDistributionSelectedObjectsBarButtonItem_ItemClick);
		this.profileValuesBarButtonItem.Caption = "Profile Values (selected objects)";
		this.profileValuesBarButtonItem.Id = 24;
		this.profileValuesBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.values_play_16;
		this.profileValuesBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.values_play_32;
		this.profileValuesBarButtonItem.LargeWidth = 100;
		this.profileValuesBarButtonItem.Name = "profileValuesBarButtonItem";
		this.profileValuesBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ProfileValuesSelectedObjectsBarButtonItem_ItemClick);
		this.savingIsDisabledBarStaticItem.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
		this.savingIsDisabledBarStaticItem.AllowRightClickInMenu = false;
		this.savingIsDisabledBarStaticItem.Caption = "Saving is disabled in the repository configuration. ";
		this.savingIsDisabledBarStaticItem.Id = 28;
		this.savingIsDisabledBarStaticItem.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Gray;
		this.savingIsDisabledBarStaticItem.ItemAppearance.Normal.Options.UseForeColor = true;
		this.savingIsDisabledBarStaticItem.Name = "savingIsDisabledBarStaticItem";
		this.learnMoreBarStaticItem.AllowHtmlText = DevExpress.Utils.DefaultBoolean.True;
		this.learnMoreBarStaticItem.AllowRightClickInMenu = false;
		this.learnMoreBarStaticItem.Caption = "<href=https://www.dataedo.com><color=65,180,244>Learn more</color></href> about enabling the save option.";
		this.learnMoreBarStaticItem.Id = 29;
		this.learnMoreBarStaticItem.ItemAppearance.Normal.ForeColor = System.Drawing.Color.Gray;
		this.learnMoreBarStaticItem.ItemAppearance.Normal.Options.UseForeColor = true;
		this.learnMoreBarStaticItem.Name = "learnMoreBarStaticItem";
		this.learnMoreBarStaticItem.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(LearnMoreBarStaticItem_HyperlinkClick);
		this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[3] { this.profilingRibbonPageGroup, this.clearRibbonPageGroup, this.saveRibbonPageGroup });
		this.ribbonPage1.Name = "ribbonPage1";
		this.ribbonPage1.Text = "ribbonPage1";
		this.profilingRibbonPageGroup.ItemLinks.Add(this.fullProfileBarButtonItem);
		this.profilingRibbonPageGroup.ItemLinks.Add(this.profileDistributionBarButtonItem);
		this.profilingRibbonPageGroup.ItemLinks.Add(this.profileValuesBarButtonItem);
		this.profilingRibbonPageGroup.Name = "profilingRibbonPageGroup";
		this.profilingRibbonPageGroup.Text = "Profiling";
		this.clearRibbonPageGroup.ItemLinks.Add(this.clearAllDataBarButtonItem);
		this.clearRibbonPageGroup.ItemLinks.Add(this.clearDistributionBarButtonItem);
		this.clearRibbonPageGroup.ItemLinks.Add(this.clearValuesBarButtonItem);
		this.clearRibbonPageGroup.ItemLinks.Add(this.clearAllProfilingBarButtonItem);
		this.clearRibbonPageGroup.Name = "clearRibbonPageGroup";
		this.clearRibbonPageGroup.Text = "Clear";
		this.saveRibbonPageGroup.ItemLinks.Add(this.saveAllDatabarButtonItem);
		this.saveRibbonPageGroup.ItemLinks.Add(this.saveDistributionBarButtonItem);
		this.saveRibbonPageGroup.ItemLinks.Add(this.saveValuesBarButtonItem);
		this.saveRibbonPageGroup.ItemLinks.Add(this.savingIsDisabledBarStaticItem);
		this.saveRibbonPageGroup.ItemLinks.Add(this.learnMoreBarStaticItem);
		this.saveRibbonPageGroup.Name = "saveRibbonPageGroup";
		this.saveRibbonPageGroup.Text = "Save";
		this.ribbonToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.ribbonToolTipController.BeforeShow += new DevExpress.Utils.ToolTipControllerBeforeShowEventHandler(ribbonToolTipController_BeforeShow);
		this.rootLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.rootLayoutControlGroup.GroupBordersVisible = false;
		this.rootLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[14]
		{
			this.aboveRibbondLayoutControlItem, this.emptySpaceItem2, this.profileprogressBarLayoutControlItem, this.saveAllSimpleButtonLayoutControlItem, this.quitWithoutSavingSimpleButtonLayoutControlItem, this.emptySpaceItem1, this.emptySpaceItem15, this.emptySpaceItem33, this.LeftCenterSplitterItem, this.centerRightSplitterItem,
			this.rightSectionLayoutControlItem, this.centerSectionLayoutControlItem, this.leftSectionNavigationLayoutControlItem, this.infoPanelLayoutControlItem
		});
		this.rootLayoutControlGroup.Name = "rootLayoutControlGroup";
		this.rootLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.rootLayoutControlGroup.Size = new System.Drawing.Size(1311, 740);
		this.rootLayoutControlGroup.TextVisible = false;
		this.aboveRibbondLayoutControlItem.Control = this.aboveRibbonControl;
		this.aboveRibbondLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.aboveRibbondLayoutControlItem.MaxSize = new System.Drawing.Size(0, 110);
		this.aboveRibbondLayoutControlItem.MinSize = new System.Drawing.Size(1, 110);
		this.aboveRibbondLayoutControlItem.Name = "aboveRibbondLayoutControlItem";
		this.aboveRibbondLayoutControlItem.Size = new System.Drawing.Size(1311, 110);
		this.aboveRibbondLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.aboveRibbondLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.aboveRibbondLayoutControlItem.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 676);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(0, 10);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(10, 10);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(1311, 10);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.profileprogressBarLayoutControlItem.Control = this.profileprogressBarControl;
		this.profileprogressBarLayoutControlItem.Location = new System.Drawing.Point(0, 686);
		this.profileprogressBarLayoutControlItem.Name = "profileprogressBarLayoutControlItem";
		this.profileprogressBarLayoutControlItem.Size = new System.Drawing.Size(1311, 18);
		this.profileprogressBarLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.profileprogressBarLayoutControlItem.TextVisible = false;
		this.profileprogressBarLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.saveAllSimpleButtonLayoutControlItem.Control = this.saveAllSimpleButton;
		this.saveAllSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(1121, 704);
		this.saveAllSimpleButtonLayoutControlItem.Name = "saveAllSimpleButtonLayoutControlItem";
		this.saveAllSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(90, 26);
		this.saveAllSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveAllSimpleButtonLayoutControlItem.TextVisible = false;
		this.quitWithoutSavingSimpleButtonLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.quitWithoutSavingSimpleButtonLayoutControlItem.Control = this.quitWithoutSavingSimpleButton;
		this.quitWithoutSavingSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(1211, 704);
		this.quitWithoutSavingSimpleButtonLayoutControlItem.Name = "quitWithoutSavingSimpleButtonLayoutControlItem";
		this.quitWithoutSavingSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(90, 26);
		this.quitWithoutSavingSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.quitWithoutSavingSimpleButtonLayoutControlItem.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 704);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(1121, 26);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem15.AllowHotTrack = false;
		this.emptySpaceItem15.Location = new System.Drawing.Point(0, 730);
		this.emptySpaceItem15.MaxSize = new System.Drawing.Size(0, 10);
		this.emptySpaceItem15.MinSize = new System.Drawing.Size(10, 10);
		this.emptySpaceItem15.Name = "emptySpaceItem15";
		this.emptySpaceItem15.Size = new System.Drawing.Size(1311, 10);
		this.emptySpaceItem15.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem15.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem33.AllowHotTrack = false;
		this.emptySpaceItem33.Location = new System.Drawing.Point(1301, 704);
		this.emptySpaceItem33.MaxSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem33.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem33.Name = "emptySpaceItem33";
		this.emptySpaceItem33.Size = new System.Drawing.Size(10, 26);
		this.emptySpaceItem33.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem33.TextSize = new System.Drawing.Size(0, 0);
		this.LeftCenterSplitterItem.AllowHotTrack = true;
		this.LeftCenterSplitterItem.IsCollapsible = DevExpress.Utils.DefaultBoolean.True;
		this.LeftCenterSplitterItem.Location = new System.Drawing.Point(304, 110);
		this.LeftCenterSplitterItem.Name = "LeftCenterSplitterItem";
		this.LeftCenterSplitterItem.Size = new System.Drawing.Size(10, 566);
		this.LeftCenterSplitterItem.DoubleClick += new System.EventHandler(LeftCenterSplitterItem_DoubleClick);
		this.centerRightSplitterItem.AllowHotTrack = true;
		this.centerRightSplitterItem.Inverted = true;
		this.centerRightSplitterItem.IsCollapsible = DevExpress.Utils.DefaultBoolean.True;
		this.centerRightSplitterItem.Location = new System.Drawing.Point(947, 154);
		this.centerRightSplitterItem.Name = "centerRightSplitterItem";
		this.centerRightSplitterItem.Size = new System.Drawing.Size(10, 522);
		this.rightSectionLayoutControlItem.Control = this.valuesUserControl;
		this.rightSectionLayoutControlItem.Location = new System.Drawing.Point(957, 154);
		this.rightSectionLayoutControlItem.Name = "rightSectionLayoutControlItem";
		this.rightSectionLayoutControlItem.Size = new System.Drawing.Size(354, 522);
		this.rightSectionLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.rightSectionLayoutControlItem.TextVisible = false;
		this.centerSectionLayoutControlItem.Control = this.centerSectionUserControl;
		this.centerSectionLayoutControlItem.Location = new System.Drawing.Point(314, 154);
		this.centerSectionLayoutControlItem.Name = "centerSectionLayoutControlItem";
		this.centerSectionLayoutControlItem.Size = new System.Drawing.Size(633, 522);
		this.centerSectionLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.centerSectionLayoutControlItem.TextVisible = false;
		this.leftSectionNavigationLayoutControlItem.Control = this.navigationUserControl;
		this.leftSectionNavigationLayoutControlItem.Location = new System.Drawing.Point(0, 110);
		this.leftSectionNavigationLayoutControlItem.Name = "leftSectionNavigationLayoutControlItem";
		this.leftSectionNavigationLayoutControlItem.Size = new System.Drawing.Size(304, 566);
		this.leftSectionNavigationLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.leftSectionNavigationLayoutControlItem.TextVisible = false;
		this.infoPanelLayoutControlItem.Control = this.infoUserControl;
		this.infoPanelLayoutControlItem.Location = new System.Drawing.Point(314, 110);
		this.infoPanelLayoutControlItem.Name = "infoPanelLayoutControlItem";
		this.infoPanelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 10, 10, 2);
		this.infoPanelLayoutControlItem.Size = new System.Drawing.Size(997, 44);
		this.infoPanelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.infoPanelLayoutControlItem.TextVisible = false;
		this.infoPanelLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(975, 22);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(203, 60);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(0, 72);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(1267, 10);
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.Location = new System.Drawing.Point(0, 863);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(1291, 39);
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(1311, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 740);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(1311, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 740);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1311, 0);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 740);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1311, 740);
		base.Controls.Add(this.wholeWindowLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("DataProfilingForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "DataProfilingForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Data Profiling";
		base.WindowState = System.Windows.Forms.FormWindowState.Maximized;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(DataProfilingForm_FormClosing);
		base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(DataProfilingForm_FormClosed);
		base.Shown += new System.EventHandler(DataProfilingForm_Shown);
		((System.ComponentModel.ISupportInitialize)this.wholeWindowLayoutControl).EndInit();
		this.wholeWindowLayoutControl.ResumeLayout(false);
		this.wholeWindowLayoutControl.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.profileprogressBarControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.aboveRibbonControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rootLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.aboveRibbondLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.profileprogressBarLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveAllSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.quitWithoutSavingSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem15).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem33).EndInit();
		((System.ComponentModel.ISupportInitialize)this.LeftCenterSplitterItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.centerRightSplitterItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rightSectionLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.centerSectionLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.leftSectionNavigationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.infoPanelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.behaviorManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
