using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Licences;
using Dataedo.App.MenuTree;
using Dataedo.App.Onboarding;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.ObjectBrowser;
using Dataedo.App.UserControls.OverriddenControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTab;

namespace Dataedo.App.UserControls.DataLineage;

public class DataLineageUserControl : BaseUserControl
{
	public delegate void DataLineageEditedHandler();

	private const string SettingsViewTrackingCaption = "SETTINGS";

	private const string DiagramViewTrackingCaption = "DIAGRAM";

	private const string DisabledColumnsTooltip = "You can access column-level flows only from the processors (procedures, functions, or views) or on the diagram tab.";

	private DBTreeNode currentObjectNode;

	private MetadataEditorUserControl metadataEditorUserControl;

	private UpgradeDataLineageControl upgradeDataLineageControl;

	private bool isSetParametersInProcess;

	private bool wasDiagramTabOpenedForCurrentItem;

	private bool wasConfigurationTabOpenedForCurrentItem;

	private IContainer components;

	private NonCustomizableLayoutControl dataLineageNonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private DataLineageProcessesUserControl dataLineageProcessesUserControl;

	private LayoutControlItem processesLayoutControlItem;

	private DataLineageFlowsUserControl dataLineageFlowsUserControl;

	private ExpandableLayoutControlGroup processesLayoutControlGroup;

	private SplitterItem splitterItem1;

	private DataLineageObjectBrowserUserControl objectBrowserUserControl;

	private LayoutControlItem objectBrawserLayoutControlItem;

	private ExpandableLayoutControlGroup objectBrowserLayoutControlGroup;

	private SplitterItem splitterItem2;

	private ToggleSwitch showColumnsControlToggleSwitch;

	private LayoutControlItem layoutControlItem2;

	private XtraTabControl pageTabsXtraTabControl;

	private XtraTabPage diagramXtraTabPage;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	private XtraTabPage configurationXtraTabPage;

	private LayoutControlItem tabsLayoutControlItem;

	private NonCustomizableLayoutControl configurationNonCustomizableLayoutControl;

	private LayoutControlGroup layoutControlGroup2;

	private LayoutControlItem inflowsAndOutflowsLayoutControlItem;

	private DataLineageDiagramUserControl dataLineageDiagramUserControl;

	private LayoutControlItem diagramLayoutControlItem;

	private DataLineageColumnsUserControl dataLineageColumnsUserControl;

	private LayoutControlItem columnsToggleSwitchLayoutControlItem;

	private ExpandableLayoutControlGroup columnsLayoutControlGroup;

	private LayoutControlItem columnsControlLayoutControlItem;

	private SplitterItem splitterItem;

	private ToolTipController toolTipController;

	private SplitterItem columnsSplitterItem;

	private bool ShowColumnsControl => showColumnsControlToggleSwitch?.IsOn ?? false;

	public static bool ColumnsExpanded { get; private set; }

	public static bool IsDiagramTabSelected { get; private set; }

	public bool IsInitialized { get; set; }

	public event DataLineageEditedHandler DataLineageEdited
	{
		add
		{
			dataLineageProcessesUserControl.DataLineageEdited += value;
			dataLineageFlowsUserControl.DataLineageEdited += value;
			dataLineageColumnsUserControl.DataLineageEdited += value;
		}
		remove
		{
			dataLineageProcessesUserControl.DataLineageEdited -= value;
			dataLineageFlowsUserControl.DataLineageEdited -= value;
			dataLineageColumnsUserControl.DataLineageEdited -= value;
		}
	}

	public DataLineageUserControl()
	{
		InitializeComponent();
		SkinsManager.SetToggleSwitchTheme(showColumnsControlToggleSwitch);
		DataLineageProcessesUserControl obj = dataLineageProcessesUserControl;
		obj.FocusedProcessChanged = (EventHandler)Delegate.Combine(obj.FocusedProcessChanged, (EventHandler)delegate(object selectedData, EventArgs e)
		{
			SelectedProcessChanged(selectedData);
		});
		objectBrowserUserControl.SetManager(new DataLineageObjectBrowserManager());
		DataLineageFlowsUserControl obj2 = dataLineageFlowsUserControl;
		obj2.FocusedDataFlowChanged = (EventHandler)Delegate.Combine(obj2.FocusedDataFlowChanged, new EventHandler(DataLineageFlowsUserControl_FocusedDataFlowChanged));
		dataLineageFlowsUserControl.DataLineageEdited += DataLineageFlowsUserControl_DataLienageEdited;
		ExpandableLayoutControlGroup expandableLayoutControlGroup = columnsLayoutControlGroup;
		expandableLayoutControlGroup.GroupExpandChanged = (EventHandler)Delegate.Combine(expandableLayoutControlGroup.GroupExpandChanged, new EventHandler(ColumnsLayoutControlGroup_ExpandChanged));
		columnsLayoutControlGroup.CustomButtonClick += ColumnsLayoutControlGroup_CustomButtonClick;
	}

	private bool SetFunctionalityBasedOnLicense()
	{
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataLineage))
		{
			if (upgradeDataLineageControl == null)
			{
				upgradeDataLineageControl = new UpgradeDataLineageControl
				{
					Dock = DockStyle.Fill
				};
				base.Controls.Add(upgradeDataLineageControl);
			}
			dataLineageNonCustomizableLayoutControl.Visible = false;
			upgradeDataLineageControl.Visible = true;
			return false;
		}
		dataLineageNonCustomizableLayoutControl.Visible = true;
		if (upgradeDataLineageControl != null)
		{
			upgradeDataLineageControl.Visible = false;
		}
		return true;
	}

	public void SetParameters(MetadataEditorUserControl metadataEditorUserControl, DBTreeNode currentObjectNode)
	{
		if (!SetFunctionalityBasedOnLicense())
		{
			return;
		}
		try
		{
			isSetParametersInProcess = true;
			wasDiagramTabOpenedForCurrentItem = false;
			wasConfigurationTabOpenedForCurrentItem = false;
			this.metadataEditorUserControl = metadataEditorUserControl;
			this.currentObjectNode = currentObjectNode;
			dataLineageProcessesUserControl.SetParameters(this.currentObjectNode);
			ClearDeletedObjectLists();
			List<int> processesIDs = dataLineageProcessesUserControl.GetDataProcessesCollection()?.AllDataProcesses?.Select((DataProcessRow x) => x.Id)?.ToList();
			objectBrowserUserControl.SetParameters(this.currentObjectNode, processesIDs);
			if (IsDiagramTabSelected)
			{
				pageTabsXtraTabControl.SelectedTabPage = diagramXtraTabPage;
			}
			else
			{
				pageTabsXtraTabControl.SelectedTabPage = configurationXtraTabPage;
			}
			SetColumnsFunctionality();
			bool flag = (this.currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || this.currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure) && pageTabsXtraTabControl.SelectedTabPage == configurationXtraTabPage;
			this.metadataEditorUserControl.DataLineageTabVisibilityChanged(!flag);
			TrackDataLineageShown();
			ShowOnboarding();
		}
		finally
		{
			isSetParametersInProcess = false;
		}
	}

	private void TrackDataLineageShown()
	{
		if (pageTabsXtraTabControl.SelectedTabPage == diagramXtraTabPage)
		{
			if (wasDiagramTabOpenedForCurrentItem)
			{
				return;
			}
			wasDiagramTabOpenedForCurrentItem = true;
		}
		else if (pageTabsXtraTabControl.SelectedTabPage == configurationXtraTabPage)
		{
			if (wasConfigurationTabOpenedForCurrentItem)
			{
				return;
			}
			wasConfigurationTabOpenedForCurrentItem = true;
		}
		int? flowsCount = DB.DataFlows.CountFlowsForObject(currentObjectNode.Id, SharedObjectTypeEnum.TypeToString(currentObjectNode.ObjectType));
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificObjectTypeViewFlowsCount(new TrackingUserParameters(), new TrackingDataedoParameters(), SharedObjectTypeEnum.TypeToString(currentObjectNode.ObjectType), (pageTabsXtraTabControl.SelectedTabPage == configurationXtraTabPage) ? "SETTINGS" : "DIAGRAM", flowsCount?.ToString()), TrackingEventEnum.DataLineageShow);
		});
		ShowOnboarding();
		SetColumnsFunctionality();
		IsInitialized = true;
	}

	private void SetColumnsFunctionality()
	{
		if (currentObjectNode == null)
		{
			showColumnsControlToggleSwitch.IsOn = false;
			showColumnsControlToggleSwitch.ReadOnly = true;
			columnsLayoutControlGroup.IsExpandDisabled = true;
			string text3 = (columnsLayoutControlGroup.OptionsToolTip.ToolTip = (showColumnsControlToggleSwitch.ToolTip = "You can access column-level flows only from the processors (procedures, functions, or views) or on the diagram tab."));
			columnsLayoutControlGroup.SetExpandButtonTooltip("You can access column-level flows only from the processors (procedures, functions, or views) or on the diagram tab.");
		}
		else
		{
			showColumnsControlToggleSwitch.IsOn = ColumnsExpanded;
			showColumnsControlToggleSwitch.ReadOnly = false;
			string text3 = (columnsLayoutControlGroup.OptionsToolTip.ToolTip = (showColumnsControlToggleSwitch.ToolTip = null));
			columnsLayoutControlGroup.SetExpandButtonTooltip(null);
			if (currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || currentObjectNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure)
			{
				if (pageTabsXtraTabControl.SelectedTabPage == configurationXtraTabPage)
				{
					columnsLayoutControlGroup.Collapse();
				}
				columnsLayoutControlGroup.IsExpandDisabled = true;
				columnsLayoutControlGroup.OptionsToolTip.ToolTip = "You can access column-level flows only from the processors (procedures, functions, or views) or on the diagram tab.";
				columnsLayoutControlGroup.SetExpandButtonTooltip("You can access column-level flows only from the processors (procedures, functions, or views) or on the diagram tab.");
			}
			else
			{
				columnsLayoutControlGroup.IsExpandDisabled = false;
				if (pageTabsXtraTabControl.SelectedTabPage == configurationXtraTabPage)
				{
					ChangeColumnsVisibility(ColumnsExpanded);
				}
			}
		}
		metadataEditorUserControl.DataLineageModeChanged(pageTabsXtraTabControl.SelectedTabPage == diagramXtraTabPage);
	}

	private void ShowOnboarding()
	{
		Point point = showColumnsControlToggleSwitch.PointToScreen(Point.Empty);
		Rectangle bounds = new Rectangle(point.X + 50, point.Y, showColumnsControlToggleSwitch.Width, showColumnsControlToggleSwitch.Height);
		OnboardingSupport.ShowPanel(OnboardingSupport.OnboardingMessages.DataLineageShown, this, () => bounds);
	}

	public void SelectDataProcess(int? processId)
	{
		if (processId.HasValue)
		{
			dataLineageProcessesUserControl.SelectDataProcess(processId.Value);
		}
	}

	private void SelectedProcessChanged(object selectedData)
	{
		if (selectedData is AllDataFlowsContainer allDataFlowsContainer)
		{
			dataLineageFlowsUserControl.SetParameters(metadataEditorUserControl, objectBrowserUserControl, null, currentObjectNode, allDataFlowsContainer);
			if (!isSetParametersInProcess)
			{
				objectBrowserUserControl.UpdateSuggestionsForProcess(null);
			}
		}
		else if (selectedData is DataProcessRow dataProcessRow)
		{
			dataLineageFlowsUserControl.SetParameters(metadataEditorUserControl, objectBrowserUserControl, dataProcessRow, currentObjectNode);
			if (!isSetParametersInProcess)
			{
				objectBrowserUserControl.UpdateSuggestionsForProcess(dataProcessRow.Id);
			}
		}
		else
		{
			dataLineageFlowsUserControl.SetParameters(metadataEditorUserControl, objectBrowserUserControl, null, currentObjectNode);
			if (!isSetParametersInProcess)
			{
				objectBrowserUserControl.UpdateSuggestionsForProcess(null);
			}
		}
		ReloadDataFlowsColumns();
		RefreshDiagramControl();
	}

	public bool CheckIfObjectEquals(int? objectId, SharedObjectTypeEnum.ObjectType? objectType)
	{
		DBTreeNode dBTreeNode = currentObjectNode;
		if ((dBTreeNode != null) ? (dBTreeNode.Id == objectId) : (!objectId.HasValue))
		{
			DBTreeNode dBTreeNode2 = currentObjectNode;
			if (dBTreeNode2 == null)
			{
				return !objectType.HasValue;
			}
			return dBTreeNode2.ObjectType == objectType;
		}
		return false;
	}

	public bool SaveChanges()
	{
		try
		{
			DataProcessesCollection dataProcessesCollection = dataLineageProcessesUserControl.GetDataProcessesCollection();
			if (dataProcessesCollection == null)
			{
				return false;
			}
			int num = dataLineageProcessesUserControl.GetDataProcessesCollection().GetAllModifiedColumnsCount();
			if (dataLineageColumnsUserControl.DeletedColumnRows != null)
			{
				num += dataLineageColumnsUserControl.DeletedColumnRows.Count;
			}
			bool num2 = dataProcessesCollection.SaveData(dataLineageFlowsUserControl.DeletedFlows, dataLineageColumnsUserControl.DeletedColumnRows, base.ParentForm);
			if (num2)
			{
				if (num > 0)
				{
					DataFlowHelper.TrackColumnsCount(num, currentObjectNode.ObjectType);
				}
				ClearDeletedObjectLists();
			}
			return num2;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating data lineage", FindForm());
			return false;
		}
	}

	private void ClearDeletedObjectLists()
	{
		dataLineageFlowsUserControl.DeletedFlows = new List<DataFlowRow>();
		dataLineageColumnsUserControl.DeletedColumnRows = new List<DataLineageColumnsFlowRow>();
	}

	private void ShowColumnsColntrolToggleSwitch_Toggled(object sender, EventArgs e)
	{
		if (pageTabsXtraTabControl.SelectedTabPage == diagramXtraTabPage)
		{
			HideColumnsControl();
			RefreshDiagramControl();
		}
		else if (ShowColumnsControl && currentObjectNode != null && currentObjectNode.ObjectType != SharedObjectTypeEnum.ObjectType.Table && currentObjectNode.ObjectType != SharedObjectTypeEnum.ObjectType.Structure)
		{
			columnsLayoutControlGroup.Expand();
			LayoutVisibility layoutVisibility3 = (columnsControlLayoutControlItem.Visibility = (columnsSplitterItem.Visibility = LayoutVisibility.Always));
			ReloadDataFlowsColumns();
		}
		else
		{
			HideColumnsControl();
		}
		metadataEditorUserControl?.DataLineageColumnsVisibilityChanged(ShowColumnsControl);
		ColumnsExpanded = ShowColumnsControl;
	}

	private void HideColumnsControl()
	{
		columnsLayoutControlGroup.Collapse();
		LayoutVisibility layoutVisibility3 = (columnsControlLayoutControlItem.Visibility = (columnsSplitterItem.Visibility = LayoutVisibility.Never));
	}

	private void DataLineageFlowsUserControl_FocusedDataFlowChanged(object sender, EventArgs e)
	{
		if (e is GenericEventArgs<DataFlowRow> genericEventArgs)
		{
			dataLineageColumnsUserControl.FilterColumns(genericEventArgs.Value);
		}
	}

	private void ReloadDataFlowsColumns()
	{
		if (ShowColumnsControl)
		{
			dataLineageColumnsUserControl.SetParameters(dataLineageProcessesUserControl.GetFocusedRow(), dataLineageProcessesUserControl.GetAllDataFlowsContainer());
		}
	}

	private void DataLineageFlowsUserControl_DataLienageEdited()
	{
		ReloadDataFlowsColumns();
	}

	private void ColumnsLayoutControlGroup_ExpandChanged(object sender, EventArgs e)
	{
		if (e is BoolEventArgs boolEventArgs && pageTabsXtraTabControl.SelectedTabPage != diagramXtraTabPage)
		{
			ChangeColumnsVisibility(boolEventArgs.Value);
		}
	}

	public void ChangeColumnsVisibility(bool? visible)
	{
		if (!visible.HasValue)
		{
			return;
		}
		if (visible.Value)
		{
			if (!ShowColumnsControl)
			{
				showColumnsControlToggleSwitch.Toggle();
			}
		}
		else if (ShowColumnsControl)
		{
			showColumnsControlToggleSwitch.Toggle();
		}
	}

	private void PageTabsXtraTabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		IsDiagramTabSelected = false;
		if (e.Page == diagramXtraTabPage)
		{
			IsDiagramTabSelected = true;
			RefreshDiagramControl();
			metadataEditorUserControl?.DataLineageModeChanged(isDiagramVisible: true);
			metadataEditorUserControl?.DataLineageTabVisibilityChanged(isDataLineageTabVisible: true);
		}
		else if (e.Page == configurationXtraTabPage)
		{
			DBTreeNode dBTreeNode = currentObjectNode;
			if (dBTreeNode == null || dBTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Table)
			{
				DBTreeNode dBTreeNode2 = currentObjectNode;
				if (dBTreeNode2 == null || dBTreeNode2.ObjectType != SharedObjectTypeEnum.ObjectType.Structure)
				{
					columnsLayoutControlGroup.Expanded = ShowColumnsControl;
					goto IL_00bc;
				}
			}
			metadataEditorUserControl?.DataLineageTabVisibilityChanged(isDataLineageTabVisible: false);
			columnsLayoutControlGroup.Expanded = false;
			goto IL_00bc;
		}
		goto IL_00e8;
		IL_00bc:
		LayoutVisibility layoutVisibility3 = (columnsControlLayoutControlItem.Visibility = (columnsSplitterItem.Visibility = LayoutVisibility.Always));
		metadataEditorUserControl?.DataLineageModeChanged(isDiagramVisible: false);
		goto IL_00e8;
		IL_00e8:
		TrackDataLineageShown();
	}

	private void RefreshDiagramControl()
	{
		if (pageTabsXtraTabControl.SelectedTabPage == diagramXtraTabPage)
		{
			bool showColumnsControl = ShowColumnsControl;
			List<DataFlowRow> allInflowRows = dataLineageFlowsUserControl.GetAllInflowRows();
			List<DataFlowRow> allOutflowRows = dataLineageFlowsUserControl.GetAllOutflowRows();
			dataLineageDiagramUserControl.SetParameters(metadataEditorUserControl, allInflowRows, allOutflowRows, currentObjectNode, showColumnsControl);
		}
	}

	public void SelectDiagramTab()
	{
		pageTabsXtraTabControl.SelectedTabPage = diagramXtraTabPage;
	}

	public void ChangeDiagramZoom(bool zoomIn)
	{
		dataLineageDiagramUserControl.ChangeDiagramZoom(zoomIn);
	}

	private void ColumnsLayoutControlGroup_CustomButtonClick(object sender, BaseButtonEventArgs e)
	{
		ShowExpandButtonTooltip();
	}

	private void ShowExpandButtonTooltip()
	{
		DBTreeNode dBTreeNode = currentObjectNode;
		if (dBTreeNode == null || dBTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Table)
		{
			DBTreeNode dBTreeNode2 = currentObjectNode;
			if (dBTreeNode2 == null || dBTreeNode2.ObjectType != SharedObjectTypeEnum.ObjectType.Structure)
			{
				return;
			}
		}
		if (pageTabsXtraTabControl.SelectedTabPage == configurationXtraTabPage)
		{
			toolTipController.ShowHint("You can access column-level flows only from the processors (procedures, functions, or views) or on the diagram tab.");
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
		this.dataLineageNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.pageTabsXtraTabControl = new DevExpress.XtraTab.XtraTabControl();
		this.configurationXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.configurationNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.dataLineageColumnsUserControl = new Dataedo.App.UserControls.DataLineage.DataLineageColumnsUserControl();
		this.dataLineageFlowsUserControl = new Dataedo.App.UserControls.DataLineage.DataLineageFlowsUserControl();
		this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.inflowsAndOutflowsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.columnsLayoutControlGroup = new Dataedo.App.UserControls.OverriddenControls.ExpandableLayoutControlGroup();
		this.columnsControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.columnsSplitterItem = new DevExpress.XtraLayout.SplitterItem();
		this.diagramXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.dataLineageDiagramUserControl = new Dataedo.App.UserControls.DataLineage.DataLineageDiagramUserControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.diagramLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.showColumnsControlToggleSwitch = new DevExpress.XtraEditors.ToggleSwitch();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.objectBrowserUserControl = new Dataedo.App.UserControls.ObjectBrowser.DataLineageObjectBrowserUserControl();
		this.dataLineageProcessesUserControl = new Dataedo.App.UserControls.DataLineage.DataLineageProcessesUserControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.processesLayoutControlGroup = new Dataedo.App.UserControls.OverriddenControls.ExpandableLayoutControlGroup();
		this.processesLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.columnsToggleSwitchLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
		this.objectBrowserLayoutControlGroup = new Dataedo.App.UserControls.OverriddenControls.ExpandableLayoutControlGroup();
		this.objectBrawserLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.splitterItem2 = new DevExpress.XtraLayout.SplitterItem();
		this.tabsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.splitterItem = new DevExpress.XtraLayout.SplitterItem();
		((System.ComponentModel.ISupportInitialize)this.dataLineageNonCustomizableLayoutControl).BeginInit();
		this.dataLineageNonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pageTabsXtraTabControl).BeginInit();
		this.pageTabsXtraTabControl.SuspendLayout();
		this.configurationXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.configurationNonCustomizableLayoutControl).BeginInit();
		this.configurationNonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.inflowsAndOutflowsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnsLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnsControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnsSplitterItem).BeginInit();
		this.diagramXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.diagramLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.showColumnsControlToggleSwitch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.processesLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.processesLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnsToggleSwitchLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitterItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectBrowserLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectBrawserLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitterItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitterItem).BeginInit();
		base.SuspendLayout();
		this.dataLineageNonCustomizableLayoutControl.AllowCustomization = false;
		this.dataLineageNonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.dataLineageNonCustomizableLayoutControl.Controls.Add(this.pageTabsXtraTabControl);
		this.dataLineageNonCustomizableLayoutControl.Controls.Add(this.showColumnsControlToggleSwitch);
		this.dataLineageNonCustomizableLayoutControl.Controls.Add(this.objectBrowserUserControl);
		this.dataLineageNonCustomizableLayoutControl.Controls.Add(this.dataLineageProcessesUserControl);
		this.dataLineageNonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dataLineageNonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.dataLineageNonCustomizableLayoutControl.Margin = new System.Windows.Forms.Padding(4);
		this.dataLineageNonCustomizableLayoutControl.Name = "dataLineageNonCustomizableLayoutControl";
		this.dataLineageNonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(971, 355, 812, 500);
		this.dataLineageNonCustomizableLayoutControl.Root = this.Root;
		this.dataLineageNonCustomizableLayoutControl.Size = new System.Drawing.Size(1672, 886);
		this.dataLineageNonCustomizableLayoutControl.TabIndex = 0;
		this.dataLineageNonCustomizableLayoutControl.Text = "dataLineageNonCustomizableLayoutControl";
		this.pageTabsXtraTabControl.Location = new System.Drawing.Point(282, 12);
		this.pageTabsXtraTabControl.Name = "pageTabsXtraTabControl";
		this.pageTabsXtraTabControl.SelectedTabPage = this.configurationXtraTabPage;
		this.pageTabsXtraTabControl.Size = new System.Drawing.Size(961, 862);
		this.pageTabsXtraTabControl.TabIndex = 9;
		this.pageTabsXtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.diagramXtraTabPage, this.configurationXtraTabPage });
		this.pageTabsXtraTabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(PageTabsXtraTabControl_SelectedPageChanged);
		this.configurationXtraTabPage.Controls.Add(this.configurationNonCustomizableLayoutControl);
		this.configurationXtraTabPage.Name = "configurationXtraTabPage";
		this.configurationXtraTabPage.Size = new System.Drawing.Size(959, 833);
		this.configurationXtraTabPage.Text = "Configuration";
		this.configurationNonCustomizableLayoutControl.AllowCustomization = false;
		this.configurationNonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.configurationNonCustomizableLayoutControl.Controls.Add(this.dataLineageColumnsUserControl);
		this.configurationNonCustomizableLayoutControl.Controls.Add(this.dataLineageFlowsUserControl);
		this.configurationNonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.configurationNonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.configurationNonCustomizableLayoutControl.Name = "configurationNonCustomizableLayoutControl";
		this.configurationNonCustomizableLayoutControl.Root = this.layoutControlGroup2;
		this.configurationNonCustomizableLayoutControl.Size = new System.Drawing.Size(959, 833);
		this.configurationNonCustomizableLayoutControl.TabIndex = 0;
		this.configurationNonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl2";
		this.dataLineageColumnsUserControl.BackColor = System.Drawing.Color.Transparent;
		this.dataLineageColumnsUserControl.Location = new System.Drawing.Point(5, 516);
		this.dataLineageColumnsUserControl.MinimumSize = new System.Drawing.Size(0, 250);
		this.dataLineageColumnsUserControl.Name = "dataLineageColumnsUserControl";
		this.dataLineageColumnsUserControl.Size = new System.Drawing.Size(949, 312);
		this.dataLineageColumnsUserControl.TabIndex = 6;
		this.dataLineageFlowsUserControl.BackColor = System.Drawing.Color.Transparent;
		this.dataLineageFlowsUserControl.Location = new System.Drawing.Point(2, 2);
		this.dataLineageFlowsUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.dataLineageFlowsUserControl.Name = "dataLineageFlowsUserControl";
		this.dataLineageFlowsUserControl.Size = new System.Drawing.Size(955, 472);
		this.dataLineageFlowsUserControl.TabIndex = 5;
		this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup2.GroupBordersVisible = false;
		this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.inflowsAndOutflowsLayoutControlItem, this.columnsLayoutControlGroup, this.columnsSplitterItem });
		this.layoutControlGroup2.Name = "layoutControlGroup2";
		this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup2.Size = new System.Drawing.Size(959, 833);
		this.layoutControlGroup2.TextVisible = false;
		this.inflowsAndOutflowsLayoutControlItem.Control = this.dataLineageFlowsUserControl;
		this.inflowsAndOutflowsLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.inflowsAndOutflowsLayoutControlItem.Name = "inflowsAndOutflowsLayoutControlItem";
		this.inflowsAndOutflowsLayoutControlItem.Size = new System.Drawing.Size(959, 476);
		this.inflowsAndOutflowsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.inflowsAndOutflowsLayoutControlItem.TextVisible = false;
		this.columnsLayoutControlGroup.ExpandDirection = System.Windows.Forms.AnchorStyles.Top;
		this.columnsLayoutControlGroup.IsExpandDisabled = false;
		this.columnsLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.columnsControlLayoutControlItem });
		this.columnsLayoutControlGroup.Location = new System.Drawing.Point(0, 486);
		this.columnsLayoutControlGroup.Name = "columnsExpandableLayoutControlGroup";
		this.columnsLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.columnsLayoutControlGroup.Size = new System.Drawing.Size(959, 347);
		this.columnsLayoutControlGroup.Text = "Columns";
		this.columnsControlLayoutControlItem.Control = this.dataLineageColumnsUserControl;
		this.columnsControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.columnsControlLayoutControlItem.Name = "columnsLayoutControlItem";
		this.columnsControlLayoutControlItem.Size = new System.Drawing.Size(953, 316);
		this.columnsControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.columnsControlLayoutControlItem.TextVisible = false;
		this.columnsSplitterItem.AllowHotTrack = true;
		this.columnsSplitterItem.Location = new System.Drawing.Point(0, 476);
		this.columnsSplitterItem.Name = "columnsSplitterItem";
		this.columnsSplitterItem.Size = new System.Drawing.Size(959, 10);
		this.diagramXtraTabPage.Controls.Add(this.nonCustomizableLayoutControl1);
		this.diagramXtraTabPage.Name = "diagramXtraTabPage";
		this.diagramXtraTabPage.Size = new System.Drawing.Size(960, 833);
		this.diagramXtraTabPage.Text = "Diagram";
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl1.Controls.Add(this.dataLineageDiagramUserControl);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.BackColor = System.Drawing.Color.LightGray;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25f);
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.nonCustomizableLayoutControl1.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.nonCustomizableLayoutControl1.Root = this.layoutControlGroup1;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(960, 833);
		this.nonCustomizableLayoutControl1.TabIndex = 0;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.dataLineageDiagramUserControl.Location = new System.Drawing.Point(0, 0);
		this.dataLineageDiagramUserControl.Name = "dataLineageDiagramUserControl";
		this.dataLineageDiagramUserControl.Size = new System.Drawing.Size(960, 833);
		this.dataLineageDiagramUserControl.TabIndex = 4;
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.diagramLayoutControlItem });
		this.layoutControlGroup1.Name = "layoutControlGroup1";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(960, 833);
		this.layoutControlGroup1.TextVisible = false;
		this.diagramLayoutControlItem.Control = this.dataLineageDiagramUserControl;
		this.diagramLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.diagramLayoutControlItem.Name = "diagramLayoutControlItem";
		this.diagramLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.diagramLayoutControlItem.Size = new System.Drawing.Size(960, 833);
		this.diagramLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.diagramLayoutControlItem.TextVisible = false;
		this.showColumnsControlToggleSwitch.Location = new System.Drawing.Point(13, 852);
		this.showColumnsControlToggleSwitch.Name = "showColumnsControlToggleSwitch";
		this.showColumnsControlToggleSwitch.Properties.AllowFocused = false;
		this.showColumnsControlToggleSwitch.Properties.OffText = " Columns level flows";
		this.showColumnsControlToggleSwitch.Properties.OnText = " Columns level flows";
		this.showColumnsControlToggleSwitch.Size = new System.Drawing.Size(254, 19);
		this.showColumnsControlToggleSwitch.StyleController = this.dataLineageNonCustomizableLayoutControl;
		this.showColumnsControlToggleSwitch.TabIndex = 8;
		this.showColumnsControlToggleSwitch.ToolTipController = this.toolTipController;
		this.showColumnsControlToggleSwitch.Toggled += new System.EventHandler(ShowColumnsColntrolToggleSwitch_Toggled);
		this.objectBrowserUserControl.BackColor = System.Drawing.Color.Transparent;
		this.objectBrowserUserControl.Location = new System.Drawing.Point(1256, 40);
		this.objectBrowserUserControl.Margin = new System.Windows.Forms.Padding(2);
		this.objectBrowserUserControl.MinimumSize = new System.Drawing.Size(218, 0);
		this.objectBrowserUserControl.Name = "objectBrowserUserControl";
		this.objectBrowserUserControl.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
		this.objectBrowserUserControl.Size = new System.Drawing.Size(405, 831);
		this.objectBrowserUserControl.TabIndex = 6;
		this.dataLineageProcessesUserControl.BackColor = System.Drawing.Color.Transparent;
		this.dataLineageProcessesUserControl.Location = new System.Drawing.Point(11, 40);
		this.dataLineageProcessesUserControl.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
		this.dataLineageProcessesUserControl.MinimumSize = new System.Drawing.Size(188, 0);
		this.dataLineageProcessesUserControl.Name = "dataLineageProcessesUserControl";
		this.dataLineageProcessesUserControl.Size = new System.Drawing.Size(258, 806);
		this.dataLineageProcessesUserControl.TabIndex = 4;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.processesLayoutControlGroup, this.splitterItem1, this.objectBrowserLayoutControlGroup, this.tabsLayoutControlItem, this.splitterItem2 });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(1672, 886);
		this.Root.TextVisible = false;
		this.processesLayoutControlGroup.ExpandDirection = System.Windows.Forms.AnchorStyles.Left;
		this.processesLayoutControlGroup.IsExpandDisabled = false;
		this.processesLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.processesLayoutControlItem, this.columnsToggleSwitchLayoutControlItem });
		this.processesLayoutControlGroup.Location = new System.Drawing.Point(0, 0);
		this.processesLayoutControlGroup.Name = "processesLayoutControlGroup";
		this.processesLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.processesLayoutControlGroup.Size = new System.Drawing.Size(260, 866);
		this.processesLayoutControlGroup.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 2, 2);
		this.processesLayoutControlGroup.Text = "Processes";
		this.processesLayoutControlItem.Control = this.dataLineageProcessesUserControl;
		this.processesLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.processesLayoutControlItem.Name = "processesLayoutControlItem";
		this.processesLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 2, 4);
		this.processesLayoutControlItem.Size = new System.Drawing.Size(258, 812);
		this.processesLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.processesLayoutControlItem.TextVisible = false;
		this.columnsToggleSwitchLayoutControlItem.Control = this.showColumnsControlToggleSwitch;
		this.columnsToggleSwitchLayoutControlItem.Location = new System.Drawing.Point(0, 812);
		this.columnsToggleSwitchLayoutControlItem.Name = "columnsToggleSwitchLayoutControlItem";
		this.columnsToggleSwitchLayoutControlItem.Size = new System.Drawing.Size(258, 23);
		this.columnsToggleSwitchLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.columnsToggleSwitchLayoutControlItem.TextVisible = false;
		this.splitterItem1.AllowHotTrack = true;
		this.splitterItem1.Location = new System.Drawing.Point(260, 0);
		this.splitterItem1.Name = "splitterItem1";
		this.splitterItem1.Size = new System.Drawing.Size(10, 866);
		this.objectBrowserLayoutControlGroup.ExpandDirection = System.Windows.Forms.AnchorStyles.Right;
		this.objectBrowserLayoutControlGroup.IsExpandDisabled = false;
		this.objectBrowserLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.objectBrawserLayoutControlItem });
		this.objectBrowserLayoutControlGroup.Location = new System.Drawing.Point(1245, 0);
		this.objectBrowserLayoutControlGroup.Name = "objectBrowserLayoutControlGroup";
		this.objectBrowserLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.objectBrowserLayoutControlGroup.Size = new System.Drawing.Size(407, 866);
		this.objectBrowserLayoutControlGroup.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 2, 2);
		this.objectBrowserLayoutControlGroup.Text = "Object browser";
		this.objectBrawserLayoutControlItem.Control = this.objectBrowserUserControl;
		this.objectBrawserLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.objectBrawserLayoutControlItem.Name = "objectBrawserLayoutControlItem";
		this.objectBrawserLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 2, 2);
		this.objectBrawserLayoutControlItem.Size = new System.Drawing.Size(405, 835);
		this.objectBrawserLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.objectBrawserLayoutControlItem.TextVisible = false;
		this.splitterItem2.AllowHotTrack = true;
		this.splitterItem2.Location = new System.Drawing.Point(1235, 0);
		this.splitterItem2.Name = "splitterItem2";
		this.splitterItem2.Size = new System.Drawing.Size(10, 866);
		this.tabsLayoutControlItem.Control = this.pageTabsXtraTabControl;
		this.tabsLayoutControlItem.Location = new System.Drawing.Point(270, 0);
		this.tabsLayoutControlItem.Name = "tabsLayoutControlItem";
		this.tabsLayoutControlItem.Size = new System.Drawing.Size(965, 866);
		this.tabsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.tabsLayoutControlItem.TextVisible = false;
		this.splitterItem.AllowHotTrack = true;
		this.splitterItem.Location = new System.Drawing.Point(0, 0);
		this.splitterItem.Name = "splitterItem";
		this.splitterItem.Size = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.dataLineageNonCustomizableLayoutControl);
		base.Name = "DataLineageUserControl";
		base.Size = new System.Drawing.Size(1672, 886);
		((System.ComponentModel.ISupportInitialize)this.dataLineageNonCustomizableLayoutControl).EndInit();
		this.dataLineageNonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pageTabsXtraTabControl).EndInit();
		this.pageTabsXtraTabControl.ResumeLayout(false);
		this.configurationXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.configurationNonCustomizableLayoutControl).EndInit();
		this.configurationNonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.inflowsAndOutflowsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnsLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnsControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnsSplitterItem).EndInit();
		this.diagramXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.diagramLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.showColumnsControlToggleSwitch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.processesLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.processesLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnsToggleSwitchLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitterItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectBrowserLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectBrawserLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitterItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitterItem).EndInit();
		base.ResumeLayout(false);
	}
}
