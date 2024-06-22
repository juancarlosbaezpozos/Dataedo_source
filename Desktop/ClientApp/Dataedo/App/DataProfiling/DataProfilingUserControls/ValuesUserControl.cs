using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DataProfiling.Enums;
using Dataedo.App.DataProfiling.Models;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.Tools.UI;
using Dataedo.App.Tools.UI.Skins;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.DataSources.Base.DataProfiling.Model;
using Dataedo.DataSources.Enums;
using Dataedo.Model.Data.DataProfiling;
using Dataedo.Shared.Enums;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Dataedo.App.DataProfiling.DataProfilingUserControls;

public class ValuesUserControl : BaseUserControl
{
	private ColumnNavigationObject FocusedColumnNavigationObject;

	private SplashScreenManager splashScreenManager;

	private CancellationTokenSource valuesCancellationTokenSource;

	private ProfilingDatabaseTypeEnum.ProfilingDatabaseType? profilingDatabaseType;

	private LayoutControlItem infoPanelLayoutItem;

	private InfoUserControl infoUserControl;

	private readonly List<int> listOfChangedColumnIdsTopAllRandom;

	private IContainer components;

	private NonCustomizableLayoutControl rightSectionLayoutControl;

	private LabelControl noRowsLabelControl;

	private LabelControl rightSectionSupportLabelControl;

	private SimpleButton allValuesBtn;

	private SimpleButton randomValuesSimpleButton;

	private DropDownButton topDropDownButton;

	private TreeList valuesTreeList;

	private TreeListColumn valueTreeListColumn;

	private TreeListColumn countTreeListColumn;

	private TreeListColumn percentTreeListColumn;

	private RepositoryItemProgressBar topAllValuesRepositoryItemProgressBar;

	private LabelControl listOfValuesTitleLabelControl;

	private LayoutControlGroup rightSectionLayoutControlGroup;

	private LayoutControlItem ListOfValuesLayoutControlItem;

	private LayoutControlItem valuesTreeListLayoutControlItem;

	private LayoutControlItem topDropDownButtonLayoutControlItem;

	private LayoutControlItem randomValuesSimpleButtonLayoutControlItem;

	private LayoutControlItem allSimpleButtonLayoutControlItem;

	private EmptySpaceItem emptySpaceItem10;

	private LayoutControlItem rightSectionSupportLabelControlLayoutControlItem;

	private LayoutControlItem noRowsLabelControlLayoutControlItem;

	private PopupMenu topValuesPopupMenu;

	private BarManager barManager1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarButtonItem top10BarButtonItem;

	private BarButtonItem top100BarButtonItem;

	private BarButtonItem top1000BarButtonItem;

	public List<int> IdsOfColumnsWithChangedValues => listOfChangedColumnIdsTopAllRandom;

	public bool NoChangesInValues => listOfChangedColumnIdsTopAllRandom.Count == 0;

	private DataProfilingForm DataProfilingForm => FindForm() as DataProfilingForm;

	public ValuesUserControl()
	{
		InitializeComponent();
		listOfChangedColumnIdsTopAllRandom = new List<int>();
	}

	public void Init(SplashScreenManager splashScreenManager, InfoUserControl infoUserControl, LayoutControlItem infoPanelLayoutItem)
	{
		this.splashScreenManager = splashScreenManager;
		this.infoUserControl = infoUserControl;
		this.infoPanelLayoutItem = infoPanelLayoutItem;
		ChangeApperanceOfTreeListCaption();
		SetValuesTreeListFieldNames();
	}

	private void ChangeApperanceOfTreeListCaption()
	{
		if (SkinsManager.CurrentSkin is OfficeWhite)
		{
			valuesTreeList.Appearance.Caption.ForeColor = Color.Black;
		}
		else if (SkinsManager.CurrentSkin is VsDark)
		{
			valuesTreeList.Appearance.Caption.ForeColor = Color.White;
		}
	}

	private void SetValuesTreeListFieldNames()
	{
		valuesTreeList.KeyFieldName = "Id";
		valuesTreeList.ParentFieldName = "NavigationId";
	}

	public void SetProfilingDatabaseType(SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		profilingDatabaseType = EnumToEnumChanger.GetProfilingDatabaseTypeEnum(databaseType);
	}

	public void SetFocusedNavObject(INavigationObject navObj)
	{
		FocusedColumnNavigationObject = navObj as ColumnNavigationObject;
	}

	public void SetColumnValuesClearedOrSaved(int columnId)
	{
		listOfChangedColumnIdsTopAllRandom.Remove(columnId);
	}

	public void ClearAllProfiling()
	{
		listOfChangedColumnIdsTopAllRandom.Clear();
	}

	public void SetPresentationFocusOnSpecificColumn()
	{
		try
		{
			ColumnNavigationObject focusedColumnNavigationObject = FocusedColumnNavigationObject;
			if (focusedColumnNavigationObject != null && focusedColumnNavigationObject != null)
			{
				ColumnNavigationObject columnNavigationObject = focusedColumnNavigationObject;
				if (columnNavigationObject.Column != null && columnNavigationObject.Column.ProfilingDate.HasValue)
				{
					HideInfoElement();
					HideNoRowsElement();
					ClearValuesTreeListDataSource();
					SetMaxResultsOfTopAllDataWhenChangingSelection();
					LoadNavColumnValuesIntoValuesTreeList(columnNavigationObject);
				}
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	private void SetMaxResultsOfTopAllDataWhenChangingSelection()
	{
		int? valuesListRowsCount = FocusedColumnNavigationObject.ValuesListRowsCount;
		string valuesListMode = FocusedColumnNavigationObject.ValuesListMode;
		if (valuesListRowsCount == 10)
		{
			topDropDownButton.Text = "Top 10";
		}
		else if (valuesListRowsCount == 100)
		{
			topDropDownButton.Text = "Top 100";
		}
		else if (valuesListRowsCount == 1000)
		{
			if (valuesListMode == "R")
			{
				topDropDownButton.Text = "Top 10";
			}
			else if (valuesListMode == "T")
			{
				topDropDownButton.Text = "Top 1 000";
			}
			else
			{
				topDropDownButton.Text = "Top 10";
			}
		}
	}

	private void ClearValuesTreeListDataSource()
	{
		valuesTreeList.BeginUpdate();
		valuesTreeList.DataSource = null;
		valuesTreeList.EndUpdate();
	}

	private async void AllValues_Click(object sender, EventArgs e)
	{
		await GetFocusedNavColumnValuesAsync(TopAllRandomEnum.TopAllRandom.All);
	}

	private async void RandomValues_Click(object sender, EventArgs e)
	{
		await GetFocusedNavColumnValuesAsync(TopAllRandomEnum.TopAllRandom.Random);
	}

	private async void Top10BarButtonItem1_ItemClick(object sender, ItemClickEventArgs e)
	{
		UpdateDropDownButton(e.Item);
		await GetFocusedNavColumnValuesAsync(TopAllRandomEnum.TopAllRandom.Top);
	}

	private async void Top100BarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		UpdateDropDownButton(e.Item);
		await GetFocusedNavColumnValuesAsync(TopAllRandomEnum.TopAllRandom.Top);
	}

	private async void Top1000BarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		UpdateDropDownButton(e.Item);
		await GetFocusedNavColumnValuesAsync(TopAllRandomEnum.TopAllRandom.Top);
	}

	private void UpdateDropDownButton(BarItem submenuItem)
	{
		topDropDownButton.Text = submenuItem.Caption;
		topDropDownButton.Tag = submenuItem.Tag;
	}

	private async void TopDropDownButton_Click(object sender, EventArgs e)
	{
		await GetFocusedNavColumnValuesAsync(TopAllRandomEnum.TopAllRandom.Top);
	}

	private async Task GetFocusedNavColumnValuesAsync(TopAllRandomEnum.TopAllRandom typeOfValues)
	{
		ColumnNavigationObject navColumn = FocusedColumnNavigationObject;
		if (navColumn == null || navColumn.Column == null)
		{
			return;
		}
		ColumnProfiledDataObject column = navColumn.Column;
		CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
		string text2 = (column.ValuesListMode = (navColumn.ValuesListMode = TopAllRandomEnum.GetStringValue(typeOfValues)));
		SetNavColumnMaxValuesNumber(navColumn, typeOfValues);
		DatabaseRow connectedDatabaseRow = DataProfilingForm.ReturnConnectedDatabaseRow();
		if (connectedDatabaseRow == null)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			return;
		}
		valuesCancellationTokenSource = new CancellationTokenSource();
		ColumnProfiledDataObject columnProfiledDataObject = column;
		columnProfiledDataObject.RowCount = await DataProfiler.GetTableRowCounterAsync(navColumn.TableName, navColumn.TableSchema, connectedDatabaseRow, valuesCancellationTokenSource.Token);
		long uniqueValuesCounter = await DataProfiler.ReturnColumnUniqueValuesCounterAsync(navColumn, connectedDatabaseRow, valuesCancellationTokenSource);
		switch (typeOfValues)
		{
		case TopAllRandomEnum.TopAllRandom.Random:
			navColumn.ObjectSampleData = await DataProfiler.ReturnColumnRandomValuesAsync(navColumn, connectedDatabaseRow, valuesCancellationTokenSource, column.RowCount.GetValueOrDefault(), returnOnlyNotNullValues: true);
			break;
		case TopAllRandomEnum.TopAllRandom.Top:
		case TopAllRandomEnum.TopAllRandom.All:
			navColumn.FieldMostCommonlyUsedValues = await DataProfiler.ReturnColumnMostCommonlyUsedValuesAsync(navColumn, connectedDatabaseRow, valuesCancellationTokenSource);
			break;
		}
		column.ValuesUniqueValues = uniqueValuesCounter;
		LoadNavColumnValuesIntoValuesTreeList(navColumn);
		CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		int columnId = navColumn.ColumnId;
		if (!listOfChangedColumnIdsTopAllRandom.Contains(columnId))
		{
			listOfChangedColumnIdsTopAllRandom.Add(columnId);
		}
		DataProfilingForm.RefreshNavigationTreeList();
		DataProfilingForm.RefreshRibbonButtonsEnability();
	}

	private void SetNavColumnMaxValuesNumber(ColumnNavigationObject navColumn, TopAllRandomEnum.TopAllRandom typeOfValues)
	{
		switch (typeOfValues)
		{
		case TopAllRandomEnum.TopAllRandom.All:
			navColumn.ValuesListRowsCount = 1000;
			navColumn.Column.ValuesListRowsCount = 1000;
			break;
		case TopAllRandomEnum.TopAllRandom.Random:
			navColumn.ValuesListRowsCount = 10;
			navColumn.Column.ValuesListRowsCount = 10;
			break;
		case TopAllRandomEnum.TopAllRandom.Top:
			switch (topDropDownButton.Text)
			{
			case "Top 10":
				navColumn.ValuesListRowsCount = 10;
				navColumn.Column.ValuesListRowsCount = 10;
				break;
			case "Top 100":
				navColumn.ValuesListRowsCount = 100;
				navColumn.Column.ValuesListRowsCount = 100;
				break;
			case "Top 1 000":
				navColumn.ValuesListRowsCount = 1000;
				navColumn.Column.ValuesListRowsCount = 1000;
				break;
			}
			break;
		}
	}

	private void LoadNavColumnValuesIntoValuesTreeList(ColumnNavigationObject navColumn)
	{
		if (navColumn == null)
		{
			return;
		}
		ClearValuesTreeListDataSource();
		ColumnProfiledDataObject column = navColumn.Column;
		ListOfValuesLayoutControlItem.Visibility = LayoutVisibility.Always;
		if (DataTypeChecker.TypeIsNotSupportedForProfiling(column.ProfilingDataType, profilingDatabaseType) || DataTypeChecker.IsStringCoreType(column.ProfilingDataType))
		{
			HideValuesTreeList();
			HideValuesButtons();
			HideNoRowsElement();
			ShowInfoElement("This data type is not supported");
			return;
		}
		if (navColumn.IsEncrypted)
		{
			HideValuesTreeList();
			HideValuesButtons();
			HideNoRowsElement();
			ShowInfoElement("Encrypted columns are not supported");
			return;
		}
		HideInfoElement();
		ShowValuesButtons();
		switch (TopAllRandomEnum.GetEnumValue(column.ValuesListMode))
		{
		case TopAllRandomEnum.TopAllRandom.Random:
			HideValuesTreeListRowsRelatedColumns();
			LoadNavColumnRandomSampleValuesIntoValuesTreeList(navColumn);
			break;
		default:
			ShowValuesTreeListRowsRelatedColumns();
			LoadNavColumnRandomTopValuesIntoValuesTreeList(navColumn);
			break;
		}
	}

	private void LoadNavColumnRandomTopValuesIntoValuesTreeList(ColumnNavigationObject navColumn)
	{
		if (navColumn == null)
		{
			return;
		}
		string displayText = TopAllRandomEnum.GetDisplayText(navColumn.ValuesListMode);
		valuesTreeList.Caption = displayText ?? "";
		IEnumerable<StringValueWithCount> fieldMostCommonlyUsedValues = navColumn.FieldMostCommonlyUsedValues;
		if (fieldMostCommonlyUsedValues != null)
		{
			if (!fieldMostCommonlyUsedValues.Any() || fieldMostCommonlyUsedValues.Sum((StringValueWithCount x) => x.Count) == 0)
			{
				ShowColumnHasNoValues(displayText);
				return;
			}
			BindingList<ColumnTopValues> dataSource = ValuesTreeListPresenter.PopulateTopAllValuesList(navColumn);
			SetValuesTreeListDataSource(displayText, dataSource);
			return;
		}
		if (navColumn.ListOfValues == null)
		{
			navColumn.GetValuesFromRepository(FindForm());
		}
		if (navColumn.ListOfValues != null)
		{
			if (!navColumn.ListOfValues.Any() || navColumn.ListOfValues.Sum((ColumnValuesDataObject x) => x.RowCount) == 0)
			{
				ShowColumnHasNoValues(displayText);
				return;
			}
			BindingList<ColumnTopValues> dataSource2 = ValuesTreeListPresenter.ShowSavedInRepositoryValuesWhenListOfValuesIsNull(navColumn);
			SetValuesTreeListDataSource(displayText, dataSource2);
		}
	}

	private void LoadNavColumnRandomSampleValuesIntoValuesTreeList(ColumnNavigationObject navColumn)
	{
		if (navColumn == null)
		{
			return;
		}
		string displayText = TopAllRandomEnum.GetDisplayText(navColumn.ValuesListMode);
		valuesTreeList.Caption = displayText ?? "";
		if (navColumn.ObjectSampleData != null)
		{
			PopulateRandomValuesInValuesTreeList(navColumn, displayText);
			return;
		}
		ProfiledDataModel columnProfileModel = DataProfilingForm.GetColumnProfileModel(navColumn);
		if (columnProfileModel != null && columnProfileModel.ObjectSampleData != null)
		{
			navColumn.ObjectSampleData = columnProfileModel.ObjectSampleData;
			PopulateRandomValuesInValuesTreeList(navColumn, displayText);
			return;
		}
		if (navColumn.ListOfValues == null)
		{
			navColumn.GetValuesFromRepository(FindForm());
		}
		if (navColumn.ListOfValues != null)
		{
			if (!navColumn.ListOfValues.Any())
			{
				ShowColumnHasNoValues(displayText);
				return;
			}
			BindingList<ColumnTopValues> dataSource = ValuesTreeListPresenter.ShowSampleSavedValues(navColumn);
			SetValuesTreeListDataSource(displayText, dataSource);
		}
	}

	private void PopulateRandomValuesInValuesTreeList(ColumnNavigationObject navColumn, string textToDisplay)
	{
		if (!navColumn.ObjectSampleData.Values.Any())
		{
			ShowColumnHasNoValues(textToDisplay);
			return;
		}
		BindingList<ColumnTopValues> dataSource = ValuesTreeListPresenter.LoadValuesFromSampleData(navColumn);
		SetValuesTreeListDataSource(textToDisplay, dataSource);
	}

	private void SetValuesTreeListDataSource(string caption, BindingList<ColumnTopValues> dataSource)
	{
		valuesTreeList.BeginUpdate();
		valuesTreeList.Caption = caption;
		valuesTreeList.DataSource = dataSource;
		valuesTreeList.ExpandAll();
		valuesTreeList.EndUpdate();
		ShowValuesTreeList();
	}

	private void ShowValuesTreeListRowsRelatedColumns()
	{
		valuesTreeList.BeginUpdate();
		countTreeListColumn.VisibleIndex = 1;
		percentTreeListColumn.VisibleIndex = 2;
		valuesTreeList.EndUpdate();
	}

	private void HideValuesTreeListRowsRelatedColumns()
	{
		valuesTreeList.BeginUpdate();
		countTreeListColumn.Visible = false;
		percentTreeListColumn.Visible = false;
		valuesTreeList.EndUpdate();
	}

	private void ShowValuesTreeList()
	{
		valuesTreeListLayoutControlItem.Visibility = LayoutVisibility.Always;
	}

	private void HideValuesTreeList()
	{
		valuesTreeListLayoutControlItem.Visibility = LayoutVisibility.Never;
	}

	private void ShowValuesButtons()
	{
		topDropDownButtonLayoutControlItem.Visibility = LayoutVisibility.Always;
		allSimpleButtonLayoutControlItem.Visibility = LayoutVisibility.Always;
		randomValuesSimpleButtonLayoutControlItem.Visibility = LayoutVisibility.Always;
	}

	private void HideValuesButtons()
	{
		topDropDownButtonLayoutControlItem.Visibility = LayoutVisibility.Never;
		allSimpleButtonLayoutControlItem.Visibility = LayoutVisibility.Never;
		randomValuesSimpleButtonLayoutControlItem.Visibility = LayoutVisibility.Never;
	}

	private void ShowColumnHasNoValues(string textToDisplayOnList)
	{
		valuesTreeList.Caption = textToDisplayOnList ?? "";
		ShowNoRowsElement();
		HideValuesTreeList();
		ClearValuesTreeListDataSource();
	}

	private void HideNoRowsElement()
	{
		noRowsLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
	}

	private void ShowNoRowsElement()
	{
		noRowsLabelControl.Text = "Column has no values";
		noRowsLabelControlLayoutControlItem.Visibility = LayoutVisibility.Always;
	}

	private void HideInfoElement()
	{
		LayoutVisibility layoutVisibility3 = (rightSectionSupportLabelControlLayoutControlItem.Visibility = (infoPanelLayoutItem.Visibility = LayoutVisibility.Never));
	}

	private void ShowInfoElement(string text)
	{
		string text4 = (rightSectionSupportLabelControl.Text = (infoUserControl.Description = text));
		LayoutVisibility layoutVisibility3 = (rightSectionSupportLabelControlLayoutControlItem.Visibility = (infoPanelLayoutItem.Visibility = LayoutVisibility.Always));
	}

	private void ValuesTreeList_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		Font font = new Font("Tahoma", 8.25f, FontStyle.Bold);
		if (e.Node.Level == 0)
		{
			e.Appearance.Font = font;
		}
	}

	private void ValuesTreeList_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
	{
		e.Allow = false;
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
		this.rightSectionLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.noRowsLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.rightSectionSupportLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.allValuesBtn = new DevExpress.XtraEditors.SimpleButton();
		this.randomValuesSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.topDropDownButton = new DevExpress.XtraEditors.DropDownButton();
		this.topValuesPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.top10BarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.top100BarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.top1000BarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.valuesTreeList = new DevExpress.XtraTreeList.TreeList();
		this.valueTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.countTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.percentTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.topAllValuesRepositoryItemProgressBar = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.listOfValuesTitleLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.rightSectionLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.ListOfValuesLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.valuesTreeListLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.topDropDownButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.randomValuesSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.allSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem10 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.rightSectionSupportLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.noRowsLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.rightSectionLayoutControl).BeginInit();
		this.rightSectionLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.topValuesPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valuesTreeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.topAllValuesRepositoryItemProgressBar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rightSectionLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ListOfValuesLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valuesTreeListLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.topDropDownButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.randomValuesSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rightSectionSupportLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.noRowsLabelControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.rightSectionLayoutControl.AllowCustomization = false;
		this.rightSectionLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.rightSectionLayoutControl.Controls.Add(this.noRowsLabelControl);
		this.rightSectionLayoutControl.Controls.Add(this.rightSectionSupportLabelControl);
		this.rightSectionLayoutControl.Controls.Add(this.allValuesBtn);
		this.rightSectionLayoutControl.Controls.Add(this.randomValuesSimpleButton);
		this.rightSectionLayoutControl.Controls.Add(this.topDropDownButton);
		this.rightSectionLayoutControl.Controls.Add(this.valuesTreeList);
		this.rightSectionLayoutControl.Controls.Add(this.listOfValuesTitleLabelControl);
		this.rightSectionLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.rightSectionLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.rightSectionLayoutControl.Name = "rightSectionLayoutControl";
		this.rightSectionLayoutControl.Root = this.rightSectionLayoutControlGroup;
		this.rightSectionLayoutControl.Size = new System.Drawing.Size(300, 479);
		this.rightSectionLayoutControl.TabIndex = 11;
		this.rightSectionLayoutControl.Text = "layoutControl3";
		this.noRowsLabelControl.Location = new System.Drawing.Point(2, 464);
		this.noRowsLabelControl.Name = "noRowsLabelControl";
		this.noRowsLabelControl.Size = new System.Drawing.Size(296, 13);
		this.noRowsLabelControl.StyleController = this.rightSectionLayoutControl;
		this.noRowsLabelControl.TabIndex = 23;
		this.noRowsLabelControl.Text = "No rows";
		this.rightSectionSupportLabelControl.Location = new System.Drawing.Point(2, 55);
		this.rightSectionSupportLabelControl.Name = "rightSectionSupportLabelControl";
		this.rightSectionSupportLabelControl.Size = new System.Drawing.Size(296, 13);
		this.rightSectionSupportLabelControl.StyleController = this.rightSectionLayoutControl;
		this.rightSectionSupportLabelControl.TabIndex = 21;
		this.rightSectionSupportLabelControl.Text = "Column has no values";
		this.allValuesBtn.Location = new System.Drawing.Point(139, 72);
		this.allValuesBtn.Name = "allValuesBtn";
		this.allValuesBtn.Size = new System.Drawing.Size(57, 22);
		this.allValuesBtn.StyleController = this.rightSectionLayoutControl;
		this.allValuesBtn.TabIndex = 20;
		this.allValuesBtn.Text = "All values";
		this.allValuesBtn.Click += new System.EventHandler(AllValues_Click);
		this.randomValuesSimpleButton.Location = new System.Drawing.Point(200, 72);
		this.randomValuesSimpleButton.Name = "randomValuesSimpleButton";
		this.randomValuesSimpleButton.Size = new System.Drawing.Size(88, 22);
		this.randomValuesSimpleButton.StyleController = this.rightSectionLayoutControl;
		this.randomValuesSimpleButton.TabIndex = 19;
		this.randomValuesSimpleButton.Text = "Random values";
		this.randomValuesSimpleButton.Click += new System.EventHandler(RandomValues_Click);
		this.topDropDownButton.DropDownControl = this.topValuesPopupMenu;
		this.topDropDownButton.Location = new System.Drawing.Point(2, 72);
		this.topDropDownButton.Name = "topDropDownButton";
		this.topDropDownButton.Size = new System.Drawing.Size(133, 22);
		this.topDropDownButton.StyleController = this.rightSectionLayoutControl;
		this.topDropDownButton.TabIndex = 18;
		this.topDropDownButton.Text = "Top 10";
		this.topDropDownButton.Click += new System.EventHandler(TopDropDownButton_Click);
		this.topValuesPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.top10BarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.top100BarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.top1000BarButtonItem)
		});
		this.topValuesPopupMenu.Manager = this.barManager1;
		this.topValuesPopupMenu.Name = "topValuesPopupMenu";
		this.top10BarButtonItem.Caption = "Top 10";
		this.top10BarButtonItem.Id = 0;
		this.top10BarButtonItem.Name = "top10BarButtonItem";
		this.top10BarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(Top10BarButtonItem1_ItemClick);
		this.top100BarButtonItem.Caption = "Top 100";
		this.top100BarButtonItem.Id = 1;
		this.top100BarButtonItem.Name = "top100BarButtonItem";
		this.top100BarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(Top100BarButtonItem_ItemClick);
		this.top1000BarButtonItem.Caption = "Top 1 000";
		this.top1000BarButtonItem.Id = 2;
		this.top1000BarButtonItem.Name = "top1000BarButtonItem";
		this.top1000BarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(Top1000BarButtonItem_ItemClick);
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[3] { this.top10BarButtonItem, this.top100BarButtonItem, this.top1000BarButtonItem });
		this.barManager1.MaxItemId = 3;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(300, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 479);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(300, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 479);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(300, 0);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 479);
		this.valuesTreeList.Appearance.Caption.Font = new System.Drawing.Font("Tahoma", 9.25f, System.Drawing.FontStyle.Bold);
		this.valuesTreeList.Appearance.Caption.Options.UseFont = true;
		this.valuesTreeList.Caption = "Values List";
		this.valuesTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[3] { this.valueTreeListColumn, this.countTreeListColumn, this.percentTreeListColumn });
		this.valuesTreeList.Font = new System.Drawing.Font("Tahoma", 8.25f);
		this.valuesTreeList.Location = new System.Drawing.Point(2, 98);
		this.valuesTreeList.Name = "valuesTreeList";
		this.valuesTreeList.OptionsCustomization.AllowFilter = false;
		this.valuesTreeList.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFocus;
		this.valuesTreeList.OptionsView.ShowBandsMode = DevExpress.Utils.DefaultBoolean.False;
		this.valuesTreeList.OptionsView.ShowCaption = true;
		this.valuesTreeList.OptionsView.ShowHorzLines = false;
		this.valuesTreeList.OptionsView.ShowIndicator = false;
		this.valuesTreeList.OptionsView.ShowVertLines = false;
		this.valuesTreeList.OptionsView.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.None;
		this.valuesTreeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.topAllValuesRepositoryItemProgressBar });
		this.valuesTreeList.Size = new System.Drawing.Size(296, 362);
		this.valuesTreeList.TabIndex = 17;
		this.valuesTreeList.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(ValuesTreeList_NodeCellStyle);
		this.valuesTreeList.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(ValuesTreeList_PopupMenuShowing);
		this.valueTreeListColumn.Caption = " ";
		this.valueTreeListColumn.FieldName = "Value";
		this.valueTreeListColumn.MinWidth = 90;
		this.valueTreeListColumn.Name = "valueTreeListColumn";
		this.valueTreeListColumn.OptionsColumn.AllowEdit = false;
		this.valueTreeListColumn.OptionsColumn.AllowSort = false;
		this.valueTreeListColumn.OptionsColumn.ReadOnly = true;
		this.valueTreeListColumn.Visible = true;
		this.valueTreeListColumn.VisibleIndex = 0;
		this.valueTreeListColumn.Width = 121;
		this.countTreeListColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.countTreeListColumn.AppearanceHeader.Options.UseFont = true;
		this.countTreeListColumn.Caption = "Rows";
		this.countTreeListColumn.FieldName = "Rows";
		this.countTreeListColumn.MaxWidth = 80;
		this.countTreeListColumn.MinWidth = 80;
		this.countTreeListColumn.Name = "countTreeListColumn";
		this.countTreeListColumn.OptionsColumn.AllowEdit = false;
		this.countTreeListColumn.OptionsColumn.AllowSort = false;
		this.countTreeListColumn.OptionsColumn.ReadOnly = true;
		this.countTreeListColumn.OptionsFilter.AllowFilter = false;
		this.countTreeListColumn.Visible = true;
		this.countTreeListColumn.VisibleIndex = 1;
		this.countTreeListColumn.Width = 80;
		this.percentTreeListColumn.AppearanceHeader.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.percentTreeListColumn.AppearanceHeader.Options.UseFont = true;
		this.percentTreeListColumn.Caption = "% Rows";
		this.percentTreeListColumn.ColumnEdit = this.topAllValuesRepositoryItemProgressBar;
		this.percentTreeListColumn.FieldName = "Percents";
		this.percentTreeListColumn.MaxWidth = 80;
		this.percentTreeListColumn.MinWidth = 80;
		this.percentTreeListColumn.Name = "percentTreeListColumn";
		this.percentTreeListColumn.OptionsColumn.AllowEdit = false;
		this.percentTreeListColumn.OptionsColumn.AllowSort = false;
		this.percentTreeListColumn.OptionsColumn.ReadOnly = true;
		this.percentTreeListColumn.OptionsFilter.AllowFilter = false;
		this.percentTreeListColumn.Visible = true;
		this.percentTreeListColumn.VisibleIndex = 2;
		this.percentTreeListColumn.Width = 80;
		this.topAllValuesRepositoryItemProgressBar.DisplayFormat.FormatString = "{0}%";
		this.topAllValuesRepositoryItemProgressBar.EndColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.topAllValuesRepositoryItemProgressBar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
		this.topAllValuesRepositoryItemProgressBar.LookAndFeel.UseDefaultLookAndFeel = false;
		this.topAllValuesRepositoryItemProgressBar.Name = "topAllValuesRepositoryItemProgressBar";
		this.topAllValuesRepositoryItemProgressBar.PercentView = false;
		this.topAllValuesRepositoryItemProgressBar.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.topAllValuesRepositoryItemProgressBar.ReadOnly = true;
		this.topAllValuesRepositoryItemProgressBar.ShowTitle = true;
		this.topAllValuesRepositoryItemProgressBar.StartColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.listOfValuesTitleLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold);
		this.listOfValuesTitleLabelControl.Appearance.Options.UseFont = true;
		this.listOfValuesTitleLabelControl.AutoEllipsis = true;
		this.listOfValuesTitleLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.listOfValuesTitleLabelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.listOfValuesTitleLabelControl.Location = new System.Drawing.Point(2, 2);
		this.listOfValuesTitleLabelControl.Name = "listOfValuesTitleLabelControl";
		this.listOfValuesTitleLabelControl.Size = new System.Drawing.Size(96, 49);
		this.listOfValuesTitleLabelControl.StyleController = this.rightSectionLayoutControl;
		this.listOfValuesTitleLabelControl.TabIndex = 16;
		this.listOfValuesTitleLabelControl.Text = "List of values";
		this.rightSectionLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.rightSectionLayoutControlGroup.GroupBordersVisible = false;
		this.rightSectionLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[8] { this.ListOfValuesLayoutControlItem, this.valuesTreeListLayoutControlItem, this.topDropDownButtonLayoutControlItem, this.randomValuesSimpleButtonLayoutControlItem, this.allSimpleButtonLayoutControlItem, this.emptySpaceItem10, this.rightSectionSupportLabelControlLayoutControlItem, this.noRowsLabelControlLayoutControlItem });
		this.rightSectionLayoutControlGroup.Name = "rightSectionLayoutControlGroup";
		this.rightSectionLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.rightSectionLayoutControlGroup.Size = new System.Drawing.Size(300, 479);
		this.rightSectionLayoutControlGroup.TextVisible = false;
		this.ListOfValuesLayoutControlItem.Control = this.listOfValuesTitleLabelControl;
		this.ListOfValuesLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.ListOfValuesLayoutControlItem.MaxSize = new System.Drawing.Size(100, 53);
		this.ListOfValuesLayoutControlItem.MinSize = new System.Drawing.Size(100, 53);
		this.ListOfValuesLayoutControlItem.Name = "ListOfValuesLayoutControlItem";
		this.ListOfValuesLayoutControlItem.Size = new System.Drawing.Size(300, 53);
		this.ListOfValuesLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.ListOfValuesLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.ListOfValuesLayoutControlItem.TextVisible = false;
		this.ListOfValuesLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.valuesTreeListLayoutControlItem.Control = this.valuesTreeList;
		this.valuesTreeListLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.valuesTreeListLayoutControlItem.Name = "valuesTreeListLayoutControlItem";
		this.valuesTreeListLayoutControlItem.Size = new System.Drawing.Size(300, 366);
		this.valuesTreeListLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.valuesTreeListLayoutControlItem.TextVisible = false;
		this.valuesTreeListLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.topDropDownButtonLayoutControlItem.Control = this.topDropDownButton;
		this.topDropDownButtonLayoutControlItem.Location = new System.Drawing.Point(0, 70);
		this.topDropDownButtonLayoutControlItem.MaxSize = new System.Drawing.Size(137, 26);
		this.topDropDownButtonLayoutControlItem.MinSize = new System.Drawing.Size(137, 26);
		this.topDropDownButtonLayoutControlItem.Name = "topDropDownButtonLayoutControlItem";
		this.topDropDownButtonLayoutControlItem.Size = new System.Drawing.Size(137, 26);
		this.topDropDownButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.topDropDownButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.topDropDownButtonLayoutControlItem.TextVisible = false;
		this.topDropDownButtonLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.randomValuesSimpleButtonLayoutControlItem.Control = this.randomValuesSimpleButton;
		this.randomValuesSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(198, 70);
		this.randomValuesSimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(137, 26);
		this.randomValuesSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(85, 26);
		this.randomValuesSimpleButtonLayoutControlItem.Name = "randomValuesSimpleButtonLayoutControlItem";
		this.randomValuesSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(92, 26);
		this.randomValuesSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.randomValuesSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.randomValuesSimpleButtonLayoutControlItem.TextVisible = false;
		this.randomValuesSimpleButtonLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.allSimpleButtonLayoutControlItem.Control = this.allValuesBtn;
		this.allSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(137, 70);
		this.allSimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(137, 26);
		this.allSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(57, 26);
		this.allSimpleButtonLayoutControlItem.Name = "allSimpleButtonLayoutControlItem";
		this.allSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(61, 26);
		this.allSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.allSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.allSimpleButtonLayoutControlItem.TextVisible = false;
		this.allSimpleButtonLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.emptySpaceItem10.AllowHotTrack = false;
		this.emptySpaceItem10.Location = new System.Drawing.Point(290, 70);
		this.emptySpaceItem10.Name = "emptySpaceItem10";
		this.emptySpaceItem10.Size = new System.Drawing.Size(10, 26);
		this.emptySpaceItem10.TextSize = new System.Drawing.Size(0, 0);
		this.rightSectionSupportLabelControlLayoutControlItem.Control = this.rightSectionSupportLabelControl;
		this.rightSectionSupportLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 53);
		this.rightSectionSupportLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(108, 17);
		this.rightSectionSupportLabelControlLayoutControlItem.Name = "rightSectionSupportLabelControlLayoutControlItem";
		this.rightSectionSupportLabelControlLayoutControlItem.Size = new System.Drawing.Size(300, 17);
		this.rightSectionSupportLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.rightSectionSupportLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.rightSectionSupportLabelControlLayoutControlItem.TextVisible = false;
		this.rightSectionSupportLabelControlLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.noRowsLabelControlLayoutControlItem.Control = this.noRowsLabelControl;
		this.noRowsLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 462);
		this.noRowsLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 17);
		this.noRowsLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(108, 17);
		this.noRowsLabelControlLayoutControlItem.Name = "noRowsLabelControlLayoutControlItem";
		this.noRowsLabelControlLayoutControlItem.Size = new System.Drawing.Size(300, 17);
		this.noRowsLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.noRowsLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.noRowsLabelControlLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.rightSectionLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		this.MinimumSize = new System.Drawing.Size(300, 0);
		base.Name = "ValuesUserControl";
		base.Size = new System.Drawing.Size(300, 479);
		((System.ComponentModel.ISupportInitialize)this.rightSectionLayoutControl).EndInit();
		this.rightSectionLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.topValuesPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valuesTreeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.topAllValuesRepositoryItemProgressBar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rightSectionLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ListOfValuesLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valuesTreeListLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.topDropDownButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.randomValuesSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rightSectionSupportLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.noRowsLabelControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
