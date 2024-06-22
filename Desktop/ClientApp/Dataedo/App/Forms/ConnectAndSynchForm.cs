using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Enums;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Helpers.CloudStorage.AmazonS3;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Connections;
using Dataedo.App.Import.CloudStorage;
using Dataedo.App.Import.DataLake;
using Dataedo.App.Licences;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Synchronization_Tools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CommandLine;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.ImportCommand;
using Dataedo.App.Tools.CommandLine.Xml;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.Tools.Tracking.Builders;
using Dataedo.App.Tools.Tracking.Enums;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.Tools.Tracking.Models;
using Dataedo.App.Tools.Tracking.Services;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.ConnectorsControls;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.App.UserControls.Interfaces;
using Dataedo.App.UserControls.InterfaceTables;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Model.Data.InterfaceTables;
using Dataedo.Model.Extensions;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraWizard;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.BigQuery.V2;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.Forms;

public class ConnectAndSynchForm : BaseXtraForm
{
	private DocObjectsManagement addingDocObjects;

	private TreeList metadataTreeList;

	private MetadataEditorUserControl mainControl;

	private bool isSynchRequired;

	private SynchDatabase synchronize;

	private ConnectToDatabase connectToDatabase;

	private bool canCloseWindow;

	private DXErrorProvider customFieldsErrorProvider;

	private List<ImportExtendedProperyRow> customFields;

	private List<DocumentationCustomFieldRow> docCustomFields;

	private int? databaseId;

	private bool? isCopyingConnection;

	private IContainer components;

	private SplashScreenManager splashScreenManager;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem8;

	private CheckEdit checkEdit1;

	private PictureEdit pictureEdit2;

	private ToolTipControllerUserControl toolTipController;

	private DevExpress.XtraWizard.WizardPage customFieldsWizardPage;

	private NonCustomizableLayoutControl customFieldsLayoutControl;

	private SimpleButton defineCustomFieldsSimpleButton;

	private InfoUserControl extendedPropertiesInfoUserControl;

	private GridControl customFieldsGridControl;

	private CustomGridUserControl customFieldsGridView;

	private GridColumn selectedCustomFieldsGridColumn;

	private RepositoryItemCheckEdit selectedCustomFieldsRepositoryItemCheckEdit;

	private GridColumn titleCustomFieldsGridColumn;

	private GridColumn extendedPropertyCustomFieldsGridColumn;

	private RepositoryItemTextEdit extendedPropertyRepositoryItemTextEdit;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem extendedPropertiesInfoLayoutControlItem;

	private EmptySpaceItem emptySpaceItem3;

	private LayoutControlItem defineCustomFieldLayoutControlItem;

	private EmptySpaceItem emptySpaceItem4;

	private GridView gridView1;

	private DevExpress.XtraWizard.WizardPage progressWizardPage;

	private NonCustomizableLayoutControl readingDatabaseLayoutControl;

	private LabelControl messageLabel;

	private LabelControl objectLabel;

	private ProgressBarControl readingObjectsProgressBar;

	private LayoutControlGroup layoutControlGroup9;

	private LayoutControlItem layoutControlItem8;

	private LayoutControlItem layoutControlItem13;

	private LayoutControlItem readingObjectsProgressBarLayoutControlItem;

	private DevExpress.XtraWizard.WizardPage documentationTitleWizardPage;

	private NonCustomizableLayoutControl documentationTitleLayoutControl;

	private TextEdit documentationTitleTextEdit;

	private LayoutControlGroup layoutControlGroup10;

	private LayoutControlItem documentationTitleLayoutControlItem;

	private DevExpress.XtraWizard.WizardPage selectObjectsSynchWizardPage;

	private GridControl synchronizeGrid;

	private CustomGridUserControl synchronizeGridView;

	private GridColumn synchronizeObjectGridColumn;

	private RepositoryItemCheckEdit synchronizeTableRepositoryItemCheckEdit;

	private GridColumn statusObjectGridColumn;

	private GridColumn classSynchronizeGridColumn;

	private GridColumn typeSynchronizeGridColumn;

	private GridColumn schemaSynchronizeGridColumn;

	private GridColumn nameSynchronizeGridColumn;

	private NonCustomizableLayoutControl layout;

	private LabelControl saveImportCommandLabelControl;

	private LabelControl updatedObjectCounterlabel;

	private LabelControl newObjectCounterlabel;

	private LabelControl selectedObjectCounterlabel;

	private LayoutControlGroup layoutControlGroup7;

	private LayoutControlItem layoutControlItem20;

	private LayoutControlItem layoutControlItem23;

	private LayoutControlItem layoutControlItem24;

	private LayoutControlItem saveImportCommandLayoutControlItem;

	private NonCustomizableLayoutControl objectsStatusesLayoutControl;

	private LabelControl selectObjectsLabel;

	private LabelControl labelControl2;

	private HyperLinkEdit checkNoneHyperLinkEdit;

	private HyperLinkEdit checkAllHyperLinkEdit;

	private InfoUserControl synchStateCounterInfoUserControl;

	private LayoutControlGroup layoutControlGroup5;

	private LayoutControlGroup statusLayoutControlGroup;

	private LayoutControlItem layoutControlItem6;

	private LayoutControlGroup layoutControlGroup6;

	private LayoutControlItem checkAllLayoutControlItem;

	private LayoutControlItem checkNoneLayoutControlItem;

	private EmptySpaceItem checkEmptySpaceItem;

	private LayoutControlItem layoutControlItem19;

	private LayoutControlItem selectObjectsLayoutControlItem;

	private GridView gridView2;

	private DevExpress.XtraWizard.WizardPage filterWizardPage;

	private NonCustomizableLayoutControl generatingDocLayoutControl;

	private ImportFilterControl importFilterControl;

	private LayoutControlGroup layoutControlGroup4;

	private LayoutControlItem filterLayoutControlItem;

	private CompletionWizardPage completionWizardPage;

	private NonCustomizableLayoutControl synchronizationLayoutControl;

	private ProgressBarControl synchronizeProgressBar;

	private LabelControl objectNameLabel;

	private LabelControl synchronizeObjectLabel;

	private LayoutControlGroup layoutControlGroup8;

	private LayoutControlItem synchStateLayoutControlItem;

	private LayoutControlItem synchObjectlayoutControlItem;

	private LayoutControlItem progressLayoutControlItem;

	private EmptySpaceItem emptySpaceItem5;

	private EmptySpaceItem emptySpaceItem10;

	private DevExpress.XtraWizard.WizardPage connectionWizardPage;

	private NonCustomizableLayoutControl layoutControl1;

	private HelpIconUserControl helpIconUserControl;

	private ComboBoxEdit saveAsComboBoxEdit;

	private InfoUserControl filterInfoUserControl;

	private CheckEdit advancedCheckEdit;

	private DbConnectUserControlNew dbConnectUserControl;

	private LayoutControlGroup layoutControlGroup11;

	private EmptySpaceItem advancedSettingsEmptySpaceItem;

	private LayoutControlItem advancedSettingsLayoutControlItem;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem saveAsLayoutControlItem;

	private LayoutControlItem filterInfoLayoutControlItem;

	private LayoutControlItem helpIconLayoutControlItem;

	private WizardControl connectAndSynchWizardControl;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private GridView repositoryItemGridLookUpEdit1View;

	private RepositoryItemLookUpEdit customFieldRepositoryItemLookUpEdit;

	private DevExpress.XtraWizard.WizardPage dbmsPickerWizardPage;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private LayoutControlGroup Root;

	private LabelControl saveUpdateCommandLabelControl;

	private DevExpress.XtraWizard.WizardPage dbmsAdditionalPickerWizardPage;

	private DBMSPickerUserControl databaseSubtypesDbmsPickerUserControl;

	private ImportSelectionUserControl importSelectionUserControl;

	private LayoutControlItem layoutControlItem4;

	private DevExpress.XtraWizard.WizardPage fileImportDestinationWizardPage;

	private ChooseDocumentationObjectTypeUserControl chooseDocumentationObjectTypeControl;

	private InfoUserControl learnMoreInfoUserControl;

	private LayoutControlItem learnMoreInfoLayoutControlItem;

	private DevExpress.XtraWizard.WizardPage selectNewDatabaseTitleWizardPage;

	private TextEdit newDatabaseTitleTextEdit;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl2;

	private LayoutControlGroup layoutControlGroup2;

	private LayoutControlItem newDatabaseTitleLayoutControlItem;

	private EmptySpaceItem emptySpaceItem2;

	private DevExpress.XtraWizard.WizardPage interfaceTablesValidationWizardPage;

	private InterfaceTablesImportErrorsUserControl interfaceTablesImportErrorsUserControl;

	private CheckEdit importDependenciesCheckEdit;

	private LayoutControlItem importDependenciesLayoutControlItem;

	private HelpIconUserControl helpIconImportDependencies;

	private EmptySpaceItem importDependenciesEmptySpaceItem;

	private LayoutControlItem importDependenciesHelpIconLayoutControlItem;

	private EmptySpaceItem emptySpaceItem6;

	private MarqueeProgressBarControl readingObjectsMarqueeProgressBarControl;

	private LayoutControlItem readingObjectsMarqueeProgressBarControlLayoutControlItem;

	private bool ForceFullReimport
	{
		get
		{
			if (!DatabaseSupportFactory.GetDatabaseSupport(dbConnectUserControl.DatabaseRow?.Type).ShouldForceFullReimport)
			{
				return importDependenciesCheckEdit.Checked;
			}
			return true;
		}
	}

	public SharedDatabaseTypeEnum.DatabaseType DatabaseType => dbConnectUserControl.DatabaseRow?.Type ?? SharedDatabaseTypeEnum.DatabaseType.Manual;

	public bool HasFinished { get; private set; }

	public ButtonInfo NextButton { get; private set; }

	public event EventHandler AddDatabaseEvent;

	public event EventHandler SynchFinishedEvent;

	public ConnectAndSynchForm(MetadataEditorUserControl mainControl, int? databaseId = null, TreeList metadataTreeList = null, bool? isCopyingConnection = false)
	{
		this.databaseId = databaseId;
		this.mainControl = mainControl;
		synchStateCounterInfoUserControl = new InfoUserControl();
		InitializeComponent();
		connectAndSynchWizardControl.CustomizeCommandButtons += ConnectAndSynchWizardControl_CustomizeCommandButtons;
		importSelectionUserControl.FocusedDatabaseTypeChanged += ImportSelectionUserControl_FocusedDatabaseTypeChanged;
		importSelectionUserControl.DbmsGridViewDoubleClick += ImportSelectionUserControl_DbmsGridViewDoubleClick;
		databaseSubtypesDbmsPickerUserControl.FocusedDatabaseTypeChanged += DatabaseSubtypesDbmsPickerUserControl_FocusedDatabaseTypeChanged;
		databaseSubtypesDbmsPickerUserControl.DbmsGridViewDoubleClick += DatabaseSubtypesDbmsPickerUserControl_DbmsGridViewDoubleClick;
		ChooseDocumentationObjectTypeUserControl chooseDocumentationObjectTypeUserControl = chooseDocumentationObjectTypeControl;
		chooseDocumentationObjectTypeUserControl.SelectionChanged = (EventHandler)Delegate.Combine(chooseDocumentationObjectTypeUserControl.SelectionChanged, new EventHandler(ChooseDocumentationObjectTypeControl_SelectionChanged));
		ChooseDocumentationObjectTypeUserControl chooseDocumentationObjectTypeUserControl2 = chooseDocumentationObjectTypeControl;
		chooseDocumentationObjectTypeUserControl2.DoubleClick = (EventHandler)Delegate.Combine(chooseDocumentationObjectTypeUserControl2.DoubleClick, new EventHandler(ChooseDocumentationObjectTypeControl_DoubleClick));
		customFields = new List<ImportExtendedProperyRow>();
		docCustomFields = new List<DocumentationCustomFieldRow>();
		this.isCopyingConnection = isCopyingConnection;
		LengthValidation.SetExtendedPropertiesLimit(extendedPropertyRepositoryItemTextEdit);
		customFieldsErrorProvider = new DXErrorProvider();
		LengthValidation.SetTitleOrNameLengthLimit(documentationTitleTextEdit);
		connectToDatabase = new ConnectToDatabase();
		connectToDatabase.UpdateProgressEvent += connectionToDB_UpdateProgressEvent;
		connectToDatabase.FinishedEvent += connectionToDB_ConnectionFinishedEvent;
		synchronize = new SynchDatabase();
		synchronize.UpdateProgressEvent += synchronize_UpdateProgressEvent;
		synchronize.SynchronizationFinishedEvent += synchronize_SynchronizationFinishedEvent;
		this.metadataTreeList = metadataTreeList;
		if (this.databaseId.HasValue)
		{
			dbConnectUserControl.InitializeDatabaseRow(this.databaseId);
			TrackingRunner.Track(delegate
			{
				TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilder(new TrackingUserParameters(), new TrackingDataedoParameters()), (this.isCopyingConnection == true) ? TrackingEventEnum.ImportShowForm : TrackingEventEnum.UpdateShowForm);
			});
		}
		importSelectionUserControl.SetParameters(databaseId, dbConnectUserControl.DatabaseRow?.Type);
		dbConnectUserControl.SelectedDatabaseTypeChanged += DbConnectUserControl_SelectedDatabaseTypeChanged;
		databaseSubtypesDbmsPickerUserControl.SetParameters(databaseId, dbConnectUserControl.DatabaseRow?.Type, showOnlyDatabaseSubtypesItems: true);
		SharedDatabaseTypeEnum.DatabaseType? databaseType = dbConnectUserControl.DatabaseRow?.Type;
		if (databaseType.HasValue)
		{
			if (DatabaseSupportFactory.IsAdditionalSelectionType(databaseType.Value))
			{
				databaseSubtypesDbmsPickerUserControl.SelectedDatabaseType = databaseType;
			}
			else
			{
				importSelectionUserControl.SelectedDatabaseType = databaseType;
			}
		}
		saveImportCommandLayoutControlItem.ContentVisible = (databaseId.HasValue ? true : false);
		if (!databaseId.HasValue || !databaseType.HasValue || !DatabaseSupportFactory.GetDatabaseSupport(databaseType).CanCreateImportCommand || isCopyingConnection.GetValueOrDefault())
		{
			saveImportCommandLayoutControlItem.ContentVisible = false;
			extendedPropertiesInfoLayoutControlItem.Visibility = LayoutVisibility.Never;
			TrackingRunner.Track(delegate
			{
				TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilder(new TrackingUserParameters(), new TrackingDataedoParameters()), TrackingEventEnum.ImportShowForm);
			});
		}
		else
		{
			saveImportCommandLayoutControlItem.ContentVisible = true;
		}
		Text = ((this.databaseId.HasValue && isCopyingConnection == false) ? "Update documentation" : "Add documentation");
		addingDocObjects = new DocObjectsManagement(synchStateCounterInfoUserControl, synchronizeGridView, selectedObjectCounterlabel, newObjectCounterlabel, updatedObjectCounterlabel, checkAllLayoutControlItem, checkNoneLayoutControlItem, checkEmptySpaceItem, synchronizeTableRepositoryItemCheckEdit, checkAllHyperLinkEdit, checkNoneHyperLinkEdit, synchronizeObjectGridColumn, selectObjectsLayoutControlItem);
		addingDocObjects.UpdateSynchRequiredButtons += addingDocObjects_UpdateSynchRequiredButtons;
		readingObjectsProgressBar.Properties.LookAndFeel.UseDefaultLookAndFeel = (synchronizeProgressBar.Properties.LookAndFeel.UseDefaultLookAndFeel = true);
		readingObjectsProgressBar.Properties.LookAndFeel.Style = (synchronizeProgressBar.Properties.LookAndFeel.Style = LookAndFeelStyle.Flat);
		isSynchRequired = true;
		canCloseWindow = true;
		saveAsComboBoxEdit.Properties.Items.Add(UserSettingsEnum.GetSettingsString(UserSettingsType.Public));
		saveAsComboBoxEdit.Properties.Items.Add(UserSettingsEnum.GetSettingsString(UserSettingsType.Personal));
		saveAsComboBoxEdit.SelectedIndex = 0;
		dbConnectUserControl.DisplayFormSaveAsLayoutControlItem += DisplaySaveAsLayoutControlItem;
		dbConnectUserControl.DisplayFilterInfo += DisplayFilterInfo;
		if (GetConnectionPageCondition())
		{
			SetConnectionPageParameters(DatabaseSupportFactory.IsAdditionalSelectionType(databaseType));
			DatabaseRow databaseRow = dbConnectUserControl.DatabaseRow;
			if (databaseRow != null && databaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Manual)
			{
				SetDBMSPickerPage();
			}
			else
			{
				connectAndSynchWizardControl.SelectedPage = connectionWizardPage;
			}
		}
		SetFunctionality();
	}

	private void DatabaseSubtypesDbmsPickerUserControl_DbmsGridViewDoubleClick(object sender, EventArgs e)
	{
		NextButtonPerformClick();
	}

	private void ChooseDocumentationObjectTypeControl_SelectionChanged(object sender, EventArgs e)
	{
		if (NextButton != null)
		{
			if (e is BoolEventArgs boolEventArgs && boolEventArgs.Value)
			{
				NextButton.Button.Enabled = true;
			}
			else
			{
				NextButton.Button.Enabled = false;
			}
		}
	}

	private void ChooseDocumentationObjectTypeControl_DoubleClick(object sender, EventArgs e)
	{
		NextButtonPerformClick();
	}

	private void ImportSelectionUserControl_DbmsGridViewDoubleClick(object sender, EventArgs e)
	{
		NextButtonPerformClick();
	}

	private void NextButtonPerformClick()
	{
		ButtonInfo nextButton = NextButton;
		if (nextButton != null && nextButton.Button?.Enabled == true)
		{
			NextButton.Button.PerformClick();
		}
	}

	private void ImportSelectionUserControl_FocusedDatabaseTypeChanged(object sender, EventArgs e)
	{
		SetNextButtonAvailability(importSelectionUserControl);
	}

	private void DatabaseSubtypesDbmsPickerUserControl_FocusedDatabaseTypeChanged(object sender, EventArgs e)
	{
		SetNextButtonAvailability(databaseSubtypesDbmsPickerUserControl);
	}

	private void SetNextButtonAvailability(ISelectImportControl selectImportControl)
	{
		if (NextButton != null)
		{
			DBMSGridModel dBMSGridModel = selectImportControl?.GetFocusedDBMSGridModel();
			if (dBMSGridModel == null || string.IsNullOrEmpty(dBMSGridModel.Type))
			{
				NextButton.Button.Enabled = false;
			}
			else if (dBMSGridModel.IsConnectorInLicense)
			{
				NextButton.Button.Enabled = true;
			}
			else
			{
				NextButton.Button.Enabled = false;
			}
		}
	}

	private void ConnectAndSynchWizardControl_CustomizeCommandButtons(object sender, CustomizeCommandButtonsEventArgs e)
	{
		NextButton = e.NextButton;
		if (e.CancelButton?.Button != null)
		{
			e.CancelButton.Button.AllowFocus = false;
		}
		e.PrevButton.Visible = true;
		if (e.Page == dbmsPickerWizardPage)
		{
			SetNextButtonAvailability(importSelectionUserControl);
			e.PrevButton.Visible = false;
		}
		else if (e.Page == dbmsAdditionalPickerWizardPage)
		{
			SetNextButtonAvailability(databaseSubtypesDbmsPickerUserControl);
		}
	}

	public void SetManualDocumentationItemsVisibility()
	{
		if (dbConnectUserControl.DatabaseRow.Id.HasValue)
		{
			DisplaySaveAsLayoutControlItem();
			dbConnectUserControl.SetSaveAsLayoutControlItemVisibility(DatabaseType != SharedDatabaseTypeEnum.DatabaseType.Manual && !StaticData.IsProjectFile);
		}
	}

	private void DisplayFilterInfo()
	{
		filterInfoLayoutControlItem.Visibility = (dbConnectUserControl.DatabaseRow.Filter.IsTransparent ? LayoutVisibility.Never : LayoutVisibility.Always);
	}

	private void DisplaySaveAsLayoutControlItem()
	{
		saveAsLayoutControlItem.Visibility = ((!dbConnectUserControl.SelectedDatabaseType.HasValue || StaticData.IsProjectFile || (dbConnectUserControl.DatabaseRow.Id.HasValue && (dbConnectUserControl.DatabaseRow.Type != SharedDatabaseTypeEnum.DatabaseType.Manual || dbConnectUserControl.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.Manual))) ? LayoutVisibility.Never : LayoutVisibility.Always);
	}

	private void DbConnectUserControl_SelectedDatabaseTypeChanged(object sender, EventArgs e)
	{
		SharedDatabaseTypeEnum.DatabaseType? selectedDatabaseType = dbConnectUserControl.SelectedDatabaseType;
		bool num = DatabaseSupportFactory.SupportsFilters(selectedDatabaseType, dbConnectUserControl?.DatabaseRow);
		LayoutVisibility visibility = ((!num) ? LayoutVisibility.Never : LayoutVisibility.Always);
		LayoutVisibility visibility2 = ((!num || dbConnectUserControl.DatabaseRow == null || dbConnectUserControl.DatabaseRow.Filter.IsTransparent) ? LayoutVisibility.Never : LayoutVisibility.Always);
		filterInfoLayoutControlItem.Visibility = visibility2;
		advancedSettingsLayoutControlItem.Visibility = visibility;
		helpIconLayoutControlItem.Visibility = visibility;
		advancedSettingsEmptySpaceItem.Visibility = visibility;
		EmptySpaceItem emptySpaceItem = importDependenciesEmptySpaceItem;
		LayoutControlItem layoutControlItem = importDependenciesHelpIconLayoutControlItem;
		LayoutControlItem layoutControlItem2 = importDependenciesLayoutControlItem;
		IDatabaseSupport databaseSupport = DatabaseSupportFactory.GetDatabaseSupport(selectedDatabaseType);
		LayoutVisibility layoutVisibility2 = (layoutControlItem2.Visibility = ((databaseSupport == null || !databaseSupport.CanImportDependencies) ? LayoutVisibility.Never : LayoutVisibility.Always));
		LayoutVisibility layoutVisibility5 = (emptySpaceItem.Visibility = (layoutControlItem.Visibility = layoutVisibility2));
		dbConnectUserControl.HideOtherTypeFields();
	}

	private void SetFunctionality()
	{
		defineCustomFieldsSimpleButton.Enabled = Functionalities.HasFunctionality(FunctionalityEnum.Functionality.CustomFields);
		defineCustomFieldsSimpleButton.SuperTip = Functionalities.GetUnavailableActionToolTip(FunctionalityEnum.Functionality.CustomFields);
		saveImportCommandLabelControl.Text = "<href>Save import command</href>";
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.Escape:
			if (!addingDocObjects.IsFilterPopupMenuShown)
			{
				EscPressed();
			}
			else
			{
				addingDocObjects.IsFilterPopupMenuShown = false;
			}
			break;
		case Keys.Return:
			if (dbConnectUserControl.ContainsFocus)
			{
				dbConnectUserControl.SetPortFromHost();
			}
			base.ProcessCmdKey(ref msg, Keys.Return);
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void EscPressed()
	{
		bool num = connectAndSynchWizardControl.SelectedPage.Equals(filterWizardPage) && (connectToDatabase?.IsBusy ?? false);
		bool flag = connectAndSynchWizardControl.SelectedPage.Equals(completionWizardPage) && (synchronize?.IsBusy ?? false);
		if (!num && !flag)
		{
			Close();
		}
	}

	private void connectionToDB_UpdateProgressEvent(object sender, EventArgs e)
	{
		BackgroundWorkerProgressEventArgs backgroundWorkerProgressEventArgs = e as BackgroundWorkerProgressEventArgs;
		readingObjectsProgressBar.Position = backgroundWorkerProgressEventArgs.Progress;
		messageLabel.Text = backgroundWorkerProgressEventArgs.Messages[0];
		objectLabel.Text = ((backgroundWorkerProgressEventArgs.Messages.Count == 2) ? backgroundWorkerProgressEventArgs.Messages[1] : null);
		objectLabel.Refresh();
	}

	private void connectionToDB_ConnectionFinishedEvent(object sender, EventArgs e)
	{
		canCloseWindow = true;
		if (filterWizardPage.AllowCancel)
		{
			if ((e as ConnectionStatusEventArgs).IsSuccessful)
			{
				addingDocObjects.SetDatabase(dbConnectUserControl.DatabaseRow);
				addingDocObjects.SetSynchronizedObjectWithCounter();
				connectAndSynchWizardControl.SetNextPage();
				SetBackNextButtonsEnability(filterWizardPage, enabled: true);
				int objectsCount = dbConnectUserControl.DatabaseRow.tableRows.Count((ObjectRow x) => x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.New || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Ignored || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Unsynchronized || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted);
				TrackingRunner.Track(delegate
				{
					TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoConnectionBuilderEventSpecificObjectsCount(new TrackingConnectionParameters(dbConnectUserControl.DatabaseRow, dbConnectUserControl.SelectedDatabaseType, SSLTypeHelper.GetSelectedSSLType(dbConnectUserControl.SelectedDatabaseType, dbConnectUserControl.GetSSLTypeValue(), dbConnectUserControl.DatabaseRow.SSLSettings), dbConnectUserControl.GetConnectionType()), new TrackingUserParameters(), new TrackingDataedoParameters(), databaseId.HasValue ? objectsCount.ToString() : dbConnectUserControl.DatabaseRow.tableRows.Count().ToString()), databaseId.HasValue ? TrackingEventEnum.UpdateList : TrackingEventEnum.ImportList);
				});
				if (!isSynchRequired)
				{
					TrackingRunner.Track(delegate
					{
						TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoConnectionBuilder(new TrackingConnectionParameters(dbConnectUserControl.DatabaseRow, dbConnectUserControl.SelectedDatabaseType, SSLTypeHelper.GetSelectedSSLType(dbConnectUserControl.SelectedDatabaseType, dbConnectUserControl.GetSSLTypeValue(), dbConnectUserControl.DatabaseRow.SSLSettings), dbConnectUserControl.GetConnectionType()), new TrackingUserParameters(), new TrackingDataedoParameters()), TrackingEventEnum.UpdateNotRequired);
					});
				}
			}
			else
			{
				SynchronizationFinishedEventAction(e);
				connectAndSynchWizardControl.SelectedPage = completionWizardPage;
			}
		}
		else
		{
			Close();
		}
	}

	private void addingDocObjects_UpdateSynchRequiredButtons(object sender, EventArgs e)
	{
		isSynchRequired = (e as ObjectTypeEventArgs).IsButtonEnabled;
		connectAndSynchWizardControl.NextText = ((e as ObjectTypeEventArgs).IsButtonEnabled ? "Import" : "Finish");
	}

	private void synchronize_SynchronizationFinishedEvent(object sender, EventArgs e)
	{
		SynchronizationFinishedEventAction(e);
	}

	private void SynchronizationFinishedEventAction(EventArgs e)
	{
		try
		{
			if (metadataTreeList == null)
			{
				int? id = dbConnectUserControl.DatabaseRow.Id;
				if (id.HasValue)
				{
					this.AddDatabaseEvent?.Invoke(null, new IdEventArgs(id.Value));
				}
			}
			else
			{
				this.SynchFinishedEvent?.Invoke(null, e);
				metadataTreeList.EndUpdate();
			}
		}
		catch (Exception)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("Import is successful but an error occurred while refreshing view. Do you want to refresh view now?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, this);
			if (handlingDialogResult != null && handlingDialogResult.DialogResult == DialogResult.Yes)
			{
				mainControl.RefreshTree(showWaitForm: true, mainControl.ShowProgress);
			}
		}
		if ((e as ConnectionStatusEventArgs).IsSuccessful)
		{
			synchronizeObjectLabel.Text = "<b>Import succeeded!</b>";
			synchStateLayoutControlItem.Image = Resources.ok_16;
			HasFinished = true;
		}
		else
		{
			synchronizeObjectLabel.Text = ((dbConnectUserControl.DatabaseRow.ConnectAndSynchronizeState == SynchConnectStateEnum.SynchConnectStateType.Error) ? "<b>Import failed!</b>" : "<b>Import canceled!</b>");
			synchStateLayoutControlItem.Image = Resources.error_16;
		}
		synchStateLayoutControlItem.TextVisible = true;
		synchStateLayoutControlItem.TextAlignMode = TextAlignModeItem.CustomSize;
		synchStateLayoutControlItem.TextSize = new Size(20, 16);
		LayoutVisibility layoutVisibility3 = (synchObjectlayoutControlItem.Visibility = (progressLayoutControlItem.Visibility = LayoutVisibility.Never));
		CompletionWizardPage obj = completionWizardPage;
		bool allowBack = (completionWizardPage.AllowCancel = false);
		obj.AllowBack = allowBack;
		completionWizardPage.AllowFinish = true;
		canCloseWindow = true;
		if (DatabaseSupportFactory.GetDatabaseSupport(dbConnectUserControl.DatabaseRow.Type).CanCreateImportCommand)
		{
			saveUpdateCommandLabelControl.Visible = true;
		}
		else
		{
			saveUpdateCommandLabelControl.Visible = false;
		}
	}

	private void synchronize_UpdateProgressEvent(object sender, EventArgs e)
	{
		BackgroundWorkerProgressEventArgs backgroundWorkerProgressEventArgs = e as BackgroundWorkerProgressEventArgs;
		synchronizeProgressBar.Position = backgroundWorkerProgressEventArgs.Progress;
		if (backgroundWorkerProgressEventArgs.Messages != null && backgroundWorkerProgressEventArgs.Messages.Count > 0)
		{
			synchronizeObjectLabel.Text = backgroundWorkerProgressEventArgs.Messages[0];
			objectNameLabel.Text = ((backgroundWorkerProgressEventArgs.Messages.Count >= 2) ? backgroundWorkerProgressEventArgs.Messages[1] : null);
		}
	}

	private void connectAndSynchWizardControl_CustomizeCommandButtons(object sender, CustomizeCommandButtonsEventArgs e)
	{
		if (e.Page.Equals(connectionWizardPage))
		{
			if (dbConnectUserControl.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.DdlScript)
			{
				e.NextButton.Text = "Parse";
			}
			else
			{
				e.NextButton.Text = "Connect";
			}
			e.NextButton.Image = Resources.connect_16;
			e.NextButton.Button.Select();
			if (GetConnectionPageCondition())
			{
				e.PrevButton.Visible = false;
			}
		}
		else if (e.Page.Equals(filterWizardPage))
		{
			e.NextButton.Text = "Read";
			e.NextButton.Image = Resources.connect_16;
			e.NextButton.Button.Select();
		}
		else if ((e.Page.Equals(selectObjectsSynchWizardPage) && !dbConnectUserControl.IsDBAdded && isCopyingConnection != true) || (e.Page.Equals(documentationTitleWizardPage) && (dbConnectUserControl.IsDBAdded || isCopyingConnection == true)))
		{
			if (isSynchRequired)
			{
				e.NextButton.Text = "Import";
				e.NextButton.Image = Resources.synchronize_16;
				e.NextButton.Button.Select();
			}
			else
			{
				e.NextButton.Text = "Finish";
				e.NextButton.Image = Resources.finish_16;
				e.NextButton.Button.Select();
			}
		}
		else if (e.Page.Equals(completionWizardPage))
		{
			e.FinishButton.Image = Resources.finish_16;
			e.FinishButton.Button.Select();
		}
		else if (e.Page.Equals(interfaceTablesValidationWizardPage))
		{
			e.NextButton.Text = "Ignore >";
			e.NextButton.Image = null;
			e.NextButton.Button.Select();
		}
		else
		{
			e.NextButton.Text = "Next >";
			e.NextButton.Image = null;
			e.NextButton.Button.Select();
		}
	}

	private bool GetConnectionPageCondition()
	{
		if (!databaseId.HasValue)
		{
			return isCopyingConnection == true;
		}
		return true;
	}

	private void SetBackNextButtonsEnability(DevExpress.XtraWizard.WizardPage wizardPage, bool enabled)
	{
		bool allowBack = (wizardPage.AllowNext = enabled);
		wizardPage.AllowBack = allowBack;
	}

	private void SetBackNextButtonsEnability(CompletionWizardPage wizardPage, bool enabled)
	{
		bool allowBack = (wizardPage.AllowFinish = enabled);
		wizardPage.AllowBack = allowBack;
	}

	private void connectAndSynchWizardControl_SelectedPageChanged(object sender, WizardPageChangedEventArgs e)
	{
		if (e.Page.Equals(progressWizardPage))
		{
			if (importFilterControl.IsFilterControlTransparent == true)
			{
				importFilterControl.InvokeClearFilter();
			}
			dbConnectUserControl.DatabaseRow.Filter = importFilterControl.GetRulesCollection();
			SetBackNextButtonsEnability(progressWizardPage, enabled: false);
			canCloseWindow = false;
			DatabaseRow databaseRow = dbConnectUserControl.DatabaseRow;
			ImportExtendedProperyRow[] source = GetNormalizeCustomFields().ToArray();
			connectToDatabase.TryConnectAndRead(ref databaseRow, importFilterControl.IsFullReimport, updateEntireDocumentation: false, source.Select((ImportExtendedProperyRow x) => x.SelectedCustomField).ToArray(), dbConnectUserControl.IsDBAdded || isCopyingConnection == true, this);
		}
	}

	private void connectAndSynchWizardControl_NextClick(object sender, WizardCommandButtonClickEventArgs e)
	{
		if (e.Page.Equals(dbmsPickerWizardPage))
		{
			DbmsPickerPageNextClick(e);
		}
		else if (e.Page.Equals(dbmsAdditionalPickerWizardPage))
		{
			DbmsAdditionalPickerPageNextClick(e);
		}
		if (e.Page.Equals(connectionWizardPage))
		{
			ConnectionPageNextClick(e);
		}
		else if (e.Page.Equals(filterWizardPage))
		{
			FilterPageNextClick(e);
		}
		else if (e.Page.Equals(customFieldsWizardPage))
		{
			CustomFieldsPageNextClick(e);
		}
		else if (e.Page.Equals(selectObjectsSynchWizardPage) && !dbConnectUserControl.IsDBAdded && isCopyingConnection != true)
		{
			StartSynchronizing();
			connectAndSynchWizardControl.SelectedPage = completionWizardPage;
		}
		else if (e.Page.Equals(documentationTitleWizardPage) && (dbConnectUserControl.IsDBAdded || isCopyingConnection == true))
		{
			dbConnectUserControl.DatabaseRow.Title = documentationTitleTextEdit.Text;
			StartSynchronizing();
		}
		else if (e.Page.Equals(fileImportDestinationWizardPage))
		{
			FileImportDestinationWizardPageNextClick(e);
		}
		else if (e.Page.Equals(selectNewDatabaseTitleWizardPage))
		{
			SelectNewDatabaseNameWizardPageNextClick(e);
		}
		else if (e.Page.Equals(interfaceTablesValidationWizardPage))
		{
			InterfaceTablesValidationWizardPageNextClick(e);
		}
	}

	private void InterfaceTablesValidationWizardPageNextClick(WizardCommandButtonClickEventArgs e)
	{
		if (advancedCheckEdit.Checked)
		{
			TrackImportUpdateAdvanced();
			connectAndSynchWizardControl.SelectedPage = customFieldsWizardPage;
		}
		else
		{
			SelectProgressWizardPage();
		}
		e.Handled = true;
	}

	private void SelectProgressWizardPage()
	{
		messageLabel.Text = null;
		objectLabel.Text = null;
		readingObjectsProgressBar.EditValue = 0;
		DbConnectUserControlNew dbConnectUserControlNew = dbConnectUserControl;
		if (dbConnectUserControlNew != null && dbConnectUserControlNew.DatabaseRow?.Type == SharedDatabaseTypeEnum.DatabaseType.DdlScript)
		{
			readingObjectsProgressBarLayoutControlItem.Visibility = LayoutVisibility.Never;
			readingObjectsMarqueeProgressBarControlLayoutControlItem.Visibility = LayoutVisibility.Always;
		}
		else
		{
			readingObjectsProgressBarLayoutControlItem.Visibility = LayoutVisibility.Always;
			readingObjectsMarqueeProgressBarControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		}
		connectAndSynchWizardControl.SelectedPage = progressWizardPage;
	}

	private void SelectNewDatabaseNameWizardPageNextClick(WizardCommandButtonClickEventArgs e)
	{
		if (string.IsNullOrWhiteSpace(newDatabaseTitleTextEdit.Text))
		{
			GeneralMessageBoxesHandling.Show("Database title cannot be empty.", "Empty title", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this);
			e.Handled = true;
			return;
		}
		if (DB.Database.CheckIfDatabaseTitleExists(newDatabaseTitleTextEdit.Text))
		{
			GeneralMessageBoxesHandling.Show("This database title is already used. Please choose another one.", "Title already used", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
			e.Handled = true;
			return;
		}
		DBMSGridModel focusedDBMSGridModel = importSelectionUserControl.GetFocusedDBMSGridModel();
		if (focusedDBMSGridModel == null)
		{
			e.Handled = true;
			return;
		}
		int? num;
		if (focusedDBMSGridModel.IsCloudStorage)
		{
			dbConnectUserControl.DatabaseRow.Title = newDatabaseTitleTextEdit.Text;
			StartSynchronizing();
			num = dbConnectUserControl.DatabaseRow?.Id;
		}
		else
		{
			num = DB.Database.InsertManualDatabase(newDatabaseTitleTextEdit.Text, this);
			DB.History.InsertHistoryRow(num, num, newDatabaseTitleTextEdit.Text, null, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType.Database), saveTitle: true, saveDescription: false, SharedObjectTypeEnum.ObjectType.Database);
			WorkWithDataedoTrackingHelper.TrackNewManualDatabaseAdd();
		}
		TreeListNode treeListNode = null;
		if (databaseId.HasValue)
		{
			treeListNode = mainControl?.SearchTreeNodeOperation?.FindNode(metadataTreeList?.Nodes, databaseId.Value);
		}
		if (treeListNode == null)
		{
			mainControl.AddDatabaseNode(num);
		}
		else
		{
			mainControl.TreeListHelpers.GetDBTreeNode(treeListNode).Title = newDatabaseTitleTextEdit.Text;
			mainControl.RefreshTree(showWaitForm: true);
		}
		chooseDocumentationObjectTypeControl.SelectedDatabaseId = num;
		OpenFileImportWindow(focusedDBMSGridModel);
		e.Handled = true;
	}

	private void FileImportDestinationWizardPageNextClick(WizardCommandButtonClickEventArgs e)
	{
		DBMSGridModel focusedDBMSGridModel = importSelectionUserControl.GetFocusedDBMSGridModel();
		if (focusedDBMSGridModel == null)
		{
			e.Handled = true;
			return;
		}
		if (chooseDocumentationObjectTypeControl.IsNewDatabase)
		{
			GoToNewDatabaseTitlePage();
		}
		else if (chooseDocumentationObjectTypeControl.SelectedDatabaseId.HasValue)
		{
			OpenFileImportWindow(focusedDBMSGridModel);
		}
		e.Handled = true;
	}

	private string NewDatabaseTitle()
	{
		switch (dbConnectUserControl.DatabaseRow?.Type)
		{
		case SharedDatabaseTypeEnum.DatabaseType.AmazonS3:
			return AmazonS3Connection.GetBucketName(dbConnectUserControl.DatabaseRow.Host) + "@AmazonS3";
		case SharedDatabaseTypeEnum.DatabaseType.AzureBlobStorage:
		case SharedDatabaseTypeEnum.DatabaseType.AzureDataLakeStorage:
			return AzureStorageConnection.GetAzureStorageDatabaseTitle(dbConnectUserControl.DatabaseRow);
		default:
			return "New database";
		}
	}

	private void GoToNewDatabaseTitlePage()
	{
		string text = NewDatabaseTitle();
		string newName = FindingNewName.GetNewName(!DB.Database.GetDatabasesNewDatabaseTitle(text), text, DB.Database.GetDatabaseNewTitle(text));
		newDatabaseTitleTextEdit.Text = newName;
		connectAndSynchWizardControl.SelectedPage = selectNewDatabaseTitleWizardPage;
	}

	private void OpenFileImportWindow(DBMSGridModel dbmsGridModel)
	{
		if (dbmsGridModel != null && chooseDocumentationObjectTypeControl.SelectedDatabaseId.HasValue)
		{
			if (dbmsGridModel.IsCloudStorage)
			{
				CloudStorageTypeEnum.CloudStorageType value = CloudStorageTypeEnum.StringToType(dbmsGridModel.Type);
				int? num = chooseDocumentationObjectTypeControl.SelectedDatabaseId.Value;
				SharedObjectTypeEnum.ObjectType selectedObjectType = chooseDocumentationObjectTypeControl.SelectedObjectType;
				CustomFieldsSupport customFieldsSupport = mainControl.CustomFieldsSupport;
				CloudStorageTypeEnum.CloudStorageType? cloudStorageType = value;
				DatabaseRow databaseRow = dbConnectUserControl.DatabaseRow;
				new ImportFromCloudStorageForm(this, num, selectedObjectType, customFieldsSupport, null, cloudStorageType, databaseRow, importData: true).ShowDialog(this);
			}
			else
			{
				new ImportFromFileForm(this, chooseDocumentationObjectTypeControl.SelectedDatabaseId.Value, chooseDocumentationObjectTypeControl.SelectedObjectType, mainControl.CustomFieldsSupport, DataLakeTypeEnum.StringToType(dbmsGridModel.Type), isOpenFromMainImportWindow: true).ShowDialog(this);
			}
		}
	}

	private void DbmsPickerPageNextClick(WizardCommandButtonClickEventArgs e)
	{
		DBMSGridModel focusedDBMSGridModel = importSelectionUserControl.GetFocusedDBMSGridModel();
		if (focusedDBMSGridModel.IsCloudStorage)
		{
			if (!SetConnectionPageParameters())
			{
				e.Handled = true;
				return;
			}
			connectAndSynchWizardControl.SelectedPage = connectionWizardPage;
			e.Handled = true;
		}
		else if (!focusedDBMSGridModel.IsDatabase)
		{
			DataLakeTypeEnum.DataLakeType dataLakeType = DataLakeTypeEnum.StringToType(focusedDBMSGridModel.Type);
			chooseDocumentationObjectTypeControl.SetParameters(dataLakeType);
			connectAndSynchWizardControl.SelectedPage = fileImportDestinationWizardPage;
			if (dataLakeType == DataLakeTypeEnum.DataLakeType.JSON)
			{
				NextButton.Button.Enabled = chooseDocumentationObjectTypeControl.IsObjectTypeSelected();
			}
			else
			{
				NextButton.Button.Enabled = true;
			}
			e.Handled = true;
		}
		else if (DatabaseSupportFactory.IsTypeWithSubtypes(importSelectionUserControl.SelectedDatabaseType))
		{
			TrackImportConnectorSelected(importSelectionUserControl.SelectedDatabaseType);
			databaseSubtypesDbmsPickerUserControl.SetParameters(databaseId, importSelectionUserControl.SelectedDatabaseType, showOnlyDatabaseSubtypesItems: true);
			connectAndSynchWizardControl.SelectedPage = dbmsAdditionalPickerWizardPage;
			SetNextButtonAvailability(databaseSubtypesDbmsPickerUserControl);
			e.Handled = true;
		}
		else if (!SetConnectionPageParameters())
		{
			e.Handled = true;
		}
		else
		{
			connectAndSynchWizardControl.SelectedPage = connectionWizardPage;
			e.Handled = true;
		}
	}

	private void DbmsAdditionalPickerPageNextClick(WizardCommandButtonClickEventArgs e)
	{
		if (!SetConnectionPageParameters(useDatabaseSubtypesDbmsPickerUserControl: true))
		{
			e.Handled = true;
			return;
		}
		connectAndSynchWizardControl.SelectedPage = connectionWizardPage;
		e.Handled = true;
	}

	private bool SetConnectionPageParameters(bool useDatabaseSubtypesDbmsPickerUserControl = false)
	{
		ISelectImportControl selectImportControl = null;
		selectImportControl = ((!useDatabaseSubtypesDbmsPickerUserControl) ? ((ISelectImportControl)importSelectionUserControl) : ((ISelectImportControl)databaseSubtypesDbmsPickerUserControl));
		DBMSGridModel focusedDBMSGridModel = selectImportControl.GetFocusedDBMSGridModel();
		if (focusedDBMSGridModel != null && !focusedDBMSGridModel.IsActive)
		{
			ShowNotSupportedYetInfomationPopup(selectImportControl.SelectedDatabaseType);
			return false;
		}
		dbConnectUserControl.SetParameters(databaseId, selectImportControl.IsDBAdded, selectImportControl.SelectedDatabaseType, isCopyingConnection, selectImportControl.DBMSGridModel);
		dbConnectUserControl.Clear();
		SetManualDocumentationItemsVisibility();
		if (isCopyingConnection == true)
		{
			databaseId = null;
		}
		if (!databaseId.HasValue)
		{
			connectionWizardPage.Text = "Add documentation";
			if (selectImportControl.DBMSGridModel.Type == SharedDatabaseTypeEnum.TypeToString(SharedDatabaseTypeEnum.DatabaseType.InterfaceTables))
			{
				connectionWizardPage.DescriptionText = "Choose which database from Interface Tables (tables starting with \"import_\" in the Dataedo repository) you would like to add.";
			}
			else
			{
				connectionWizardPage.DescriptionText = "Add new documentation to your repository. Please provide connection details to the database you would like to document";
			}
			if (isCopyingConnection == true)
			{
				DisplayFilterInfo();
			}
			else
			{
				filterInfoLayoutControlItem.Visibility = LayoutVisibility.Never;
			}
			TrackImportConnectorSelected(selectImportControl.SelectedDatabaseType);
		}
		else
		{
			DevExpress.XtraWizard.WizardPage wizardPage = connectionWizardPage;
			string descriptionText;
			if (dbConnectUserControl.DatabaseRow.Type != SharedDatabaseTypeEnum.DatabaseType.Manual)
			{
				string text2 = (connectionWizardPage.DescriptionText = "Update documentation by reimporting objects from database. All descriptions will remain unchanged.");
				descriptionText = text2;
			}
			else
			{
				string text2 = (connectionWizardPage.DescriptionText = "Connect to existing database.");
				descriptionText = text2;
			}
			wizardPage.DescriptionText = descriptionText;
			Text = ((DatabaseType == SharedDatabaseTypeEnum.DatabaseType.Manual) ? "Connect to database" : "Update documentation");
			DisplayFilterInfo();
		}
		return true;
	}

	private void TrackImportConnectorSelected(SharedDatabaseTypeEnum.DatabaseType? selectedDatabaseType)
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoBuilderEventSpecificConnector(new TrackingUserParameters(), new TrackingDataedoParameters(), SharedDatabaseTypeEnum.TypeToString(selectedDatabaseType)), TrackingEventEnum.ImportConnectorSelected);
		});
	}

	private void ShowNotSupportedYetInfomationPopup(SharedDatabaseTypeEnum.DatabaseType? selectedDatabaseType)
	{
		GeneralMessageBoxesHandling.Show(selectedDatabaseType.HasValue ? ("The " + SharedDatabaseTypeEnum.TypeToStringForDisplay(selectedDatabaseType) + " connector is not supported yet.") : "This connector is not supported yet.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
		TrackImportConnectorSelected(selectedDatabaseType);
	}

	private void ConnectionPageNextClick(WizardCommandButtonClickEventArgs e)
	{
		if (!CheckSnowflakeRoleForDependencies())
		{
			e.Handled = true;
			return;
		}
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoConnectionBuilder(new TrackingConnectionParameters(dbConnectUserControl.DatabaseRow, dbConnectUserControl.SelectedDatabaseType, SSLTypeHelper.GetSelectedSSLType(dbConnectUserControl.SelectedDatabaseType, dbConnectUserControl.GetSSLTypeValue(), dbConnectUserControl.DatabaseRow.SSLSettings), dbConnectUserControl.GetConnectionType()), new TrackingUserParameters(), new TrackingDataedoParameters()), databaseId.HasValue ? TrackingEventEnum.UpdateTryConnect : TrackingEventEnum.ImportTryConnect);
		});
		dbConnectUserControl.SetImportCommandsTimeout();
		dbConnectUserControl?.DatabaseRow?.CloseConnection();
		bool connected = dbConnectUserControl.TestConnection(testForGettingDatabasesList: false);
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoConnectionBuilder(new TrackingConnectionParameters(dbConnectUserControl.DatabaseRow, dbConnectUserControl.SelectedDatabaseType, SSLTypeHelper.GetSelectedSSLType(dbConnectUserControl.SelectedDatabaseType, dbConnectUserControl.GetSSLTypeValue(), dbConnectUserControl.DatabaseRow.SSLSettings), dbConnectUserControl.GetConnectionType()), new TrackingUserParameters(), new TrackingDataedoParameters()), (!databaseId.HasValue) ? (connected ? TrackingEventEnum.ImportConnected : TrackingEventEnum.ImportConnectionFailed) : (connected ? TrackingEventEnum.UpdateConnected : TrackingEventEnum.UpdateConnectionFailed));
		});
		if (!connected)
		{
			e.Handled = true;
			return;
		}
		if (DatabaseType == SharedDatabaseTypeEnum.DatabaseType.Odbc || DatabaseType == SharedDatabaseTypeEnum.DatabaseType.IBMDb2BigQuery)
		{
			DisplayBetaWarning(DatabaseTypeEnum.TypeToStringForDisplay(DatabaseType));
		}
		string dbName = DatabaseTypeEnum.TypeToStringForDisplay(dbConnectUserControl.DatabaseRow?.Type);
		importFilterControl.SetFullReimportSwitchable(ForceFullReimport, dbName);
		if (!dbConnectUserControl.PrepareSchemas())
		{
			e.Handled = true;
			return;
		}
		if (dbConnectUserControl.DatabaseRow.HasMultipleSchemas != true || dbConnectUserControl.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.GoogleBigQuery)
		{
			documentationTitleTextEdit.Text = dbConnectUserControl.DatabaseRow.Title;
		}
		else
		{
			documentationTitleTextEdit.Text = dbConnectUserControl.DatabaseRow.User + "@" + dbConnectUserControl.DatabaseRow.Host;
		}
		importFilterControl.FullReimportVisible = !dbConnectUserControl.IsDBAdded && isCopyingConnection != true;
		importFilterControl.DatabaseType = dbConnectUserControl.DatabaseRow.Type;
		importFilterControl.SetRulesCollection(dbConnectUserControl.DatabaseRow.Filter);
		bool hasImportUsingCustomFields = DatabaseSupportFactory.GetDatabaseSupport(dbConnectUserControl.DatabaseRow.Type).HasImportUsingCustomFields;
		if (hasImportUsingCustomFields)
		{
			RefreshCustomFieldsGrid();
		}
		CloudStorageTypeEnum.CloudStorageType? cloudStorageType = CloudStorageTypeEnum.FromDatabaseType(dbConnectUserControl.DatabaseRow.Type);
		if (cloudStorageType.HasValue)
		{
			CloudStorageTypeEnum.CloudStorageType valueOrDefault = cloudStorageType.GetValueOrDefault();
			chooseDocumentationObjectTypeControl.SetParameters(valueOrDefault);
			if (!databaseId.HasValue)
			{
				GoToNewDatabaseTitlePage();
			}
			else
			{
				connectAndSynchWizardControl.SelectedPage = fileImportDestinationWizardPage;
			}
			e.Handled = true;
			return;
		}
		if (DatabaseType == SharedDatabaseTypeEnum.DatabaseType.InterfaceTables)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			if (!DB.InterfaceTables.ValidateAllImportTables(dbConnectUserControl.DatabaseRow.Param1))
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
				e.Handled = true;
				return;
			}
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			List<InterfaceTableErrorObject> allImportErrors = DB.InterfaceTables.GetAllImportErrors(dbConnectUserControl.DatabaseRow.Param1);
			if (allImportErrors != null && allImportErrors.Count > 0)
			{
				interfaceTablesImportErrorsUserControl.SetParameters(allImportErrors);
				connectAndSynchWizardControl.SelectedPage = interfaceTablesValidationWizardPage;
				e.Handled = true;
				return;
			}
		}
		if (advancedCheckEdit.Checked)
		{
			TrackImportUpdateAdvanced();
			if (hasImportUsingCustomFields)
			{
				connectAndSynchWizardControl.SelectedPage = customFieldsWizardPage;
			}
			else
			{
				connectAndSynchWizardControl.SelectedPage = filterWizardPage;
			}
			e.Handled = true;
		}
		else
		{
			filterInfoUserControl.SaveFilter = isCopyingConnection == true;
			SelectProgressWizardPage();
			e.Handled = true;
		}
	}

	private void TrackImportUpdateAdvanced()
	{
		TrackingRunner.Track(delegate
		{
			TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoConnectionBuilder(new TrackingConnectionParameters(dbConnectUserControl.DatabaseRow, dbConnectUserControl.SelectedDatabaseType, SSLTypeHelper.GetSelectedSSLType(dbConnectUserControl.SelectedDatabaseType, dbConnectUserControl.GetSSLTypeValue(), dbConnectUserControl.DatabaseRow.SSLSettings), dbConnectUserControl.GetConnectionType()), new TrackingUserParameters(), new TrackingDataedoParameters()), databaseId.HasValue ? TrackingEventEnum.UpdateAdvanced : TrackingEventEnum.ImportAdvanced);
		});
	}

	private void DisplayBetaWarning(string databaseType)
	{
		GeneralMessageBoxesHandling.Show(databaseType + " connection is a beta feature." + Environment.NewLine + "Import process may be unstable, but there's no risk to your other documentations.", databaseType + " connection", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, this);
	}

	private void FilterPageNextClick(WizardCommandButtonClickEventArgs e)
	{
		if (importFilterControl.SaveChangesVisible)
		{
			string message = (dbConnectUserControl.IsDBAdded ? "Do you want to <b>save filter</b>?" : "Do you want to <b>save filter</b> changes?");
			string title = (dbConnectUserControl.IsDBAdded ? "Save filter" : "Save filter changes");
			if (saveAsComboBoxEdit.SelectedIndex == 1 || dbConnectUserControl.HasPersonalSettingsLoaded)
			{
				DialogResult? dialogResult = GeneralMessageBoxesHandling.Show(message, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, null, 1, this)?.DialogResult;
				if (dialogResult == DialogResult.Yes)
				{
					filterInfoUserControl.SaveFilter = true;
				}
				else if (dialogResult == DialogResult.No)
				{
					if (isCopyingConnection == true)
					{
						importFilterControl.SetRulesCollection(dbConnectUserControl.DatabaseRow.Filter);
					}
					filterInfoUserControl.SaveFilter = false;
				}
				else if (dialogResult != DialogResult.No)
				{
					e.Handled = true;
					return;
				}
			}
			else
			{
				DialogResult? dialogResult2 = GeneralMessageBoxesHandling.Show(message, title, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, null, 1, this)?.DialogResult;
				if (dialogResult2 == DialogResult.Yes)
				{
					filterInfoUserControl.SaveFilter = true;
					DB.Database.UpdateFilter(dbConnectUserControl.DatabaseRow.Id, importFilterControl.GetRulesCollection().GetRulesXml(), this);
				}
				else if (dialogResult2 == DialogResult.No && isCopyingConnection == true)
				{
					importFilterControl.SetRulesCollection(dbConnectUserControl.DatabaseRow.Filter);
				}
				else if (dialogResult2 != DialogResult.No)
				{
					e.Handled = true;
					return;
				}
			}
		}
		if (isCopyingConnection == true)
		{
			filterInfoUserControl.SaveFilter = true;
		}
		SelectProgressWizardPage();
		e.Handled = true;
	}

	private void CustomFieldsPageNextClick(WizardCommandButtonClickEventArgs e)
	{
		if (DatabaseType == SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse && !ValidateExtendedPropertiesNumberForAzureDWH())
		{
			GeneralMessageBoxesHandling.Show("Due to Azure Data Warehouse limitations, only 20 extended properties can be imported at the same time." + Environment.NewLine + "If you want to import more extended properties, run an additional update selecting those unchecked now.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
			connectAndSynchWizardControl.SelectedPage = customFieldsWizardPage;
			e.Handled = true;
		}
		customFieldsGridView.ClearColumnErrors();
		bool flag = ValidateSelectedCustomFieldsEmptyValues();
		bool flag2 = ValidateMultipleBinding();
		customFieldsGridView.RefreshData();
		string text = (flag ? string.Empty : "Custom field must not be empty.");
		text += ((!flag && !flag2) ? Environment.NewLine : string.Empty);
		text += (flag2 ? string.Empty : "Custom field can not be used in multiple extended properties.");
		if (!flag || !flag2)
		{
			GeneralMessageBoxesHandling.Show(text, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
			connectAndSynchWizardControl.SelectedPage = customFieldsWizardPage;
			e.Handled = true;
		}
	}

	private bool ValidateExtendedPropertiesNumberForAzureDWH()
	{
		if (customFieldsGridView.DataRowCount < 20)
		{
			return true;
		}
		if (!(customFieldsGridControl.DataSource is List<ImportExtendedProperyRow> source))
		{
			return true;
		}
		return source.Count((ImportExtendedProperyRow x) => x.IsSelected) < 20;
	}

	private bool ValidateSelectedCustomFieldsEmptyValues()
	{
		bool result = true;
		int dataRowCount = customFieldsGridView.DataRowCount;
		for (int i = 0; i < dataRowCount; i++)
		{
			int rowHandle = customFieldsGridView.GetRowHandle(i);
			ImportExtendedProperyRow importExtendedProperyRow = customFieldsGridView.GetRow(rowHandle) as ImportExtendedProperyRow;
			if (importExtendedProperyRow.IsSelected && importExtendedProperyRow.SelectedCustomField == null)
			{
				importExtendedProperyRow.IsCustomFieldEmpty = true;
				result = false;
			}
			else
			{
				importExtendedProperyRow.IsCustomFieldEmpty = false;
			}
		}
		return result;
	}

	private bool ValidateMultipleBinding()
	{
		bool result = true;
		int dataRowCount = customFieldsGridView.DataRowCount;
		for (int i = 0; i < dataRowCount; i++)
		{
			int rowHandle = customFieldsGridView.GetRowHandle(i);
			ImportExtendedProperyRow row = customFieldsGridView.GetRow(rowHandle) as ImportExtendedProperyRow;
			if (row.SelectedCustomField != null && row.IsSelected && customFields.Count((ImportExtendedProperyRow x) => x.SelectedCustomField?.CustomFieldId == row.SelectedCustomField.CustomFieldId && x.IsSelected) > 1)
			{
				row.AlreadyInUse = true;
				result = false;
			}
			else
			{
				row.AlreadyInUse = false;
			}
		}
		return result;
	}

	private void RefreshCustomFieldsGridOnCustomFieldsChange()
	{
		_ = customFields;
		customFields = new List<ImportExtendedProperyRow>();
		SetCustomFieldsDataSource();
		List<string> list = new List<string>();
		if (dbConnectUserControl.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.SqlServer)
		{
			using (SqlCommand sqlCommand = new SqlCommand("SELECT distinct name FROM sys.extended_properties", dbConnectUserControl.DatabaseRow.Connection as SqlConnection))
			{
				using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
				while (sqlDataReader.Read())
				{
					list.Add(sqlDataReader["name"]?.ToString());
				}
				return;
			}
		}
		if (dbConnectUserControl.DatabaseRow.Type != SharedDatabaseTypeEnum.DatabaseType.GoogleBigQuery)
		{
			return;
		}
		GoogleCredential credential = GoogleCredential.FromFile(dbConnectUserControl.DatabaseRow.GetConnectionString());
		using BigQueryClient bigQueryClient = BigQueryClient.Create(dbConnectUserControl.DatabaseRow.Host, credential);
		foreach (string schema in dbConnectUserControl.DatabaseRow.Schemas)
		{
			string sql = "SELECT tsl.option_value\r\n                        FROM `" + dbConnectUserControl.DatabaseRow.Host + "`.`" + schema.Trim() + "`.INFORMATION_SCHEMA.TABLE_OPTIONS as tsl\r\n                        WHERE option_name = 'labels'";
			foreach (BigQueryRow item in bigQueryClient.ExecuteQuery(sql, null))
			{
				string text = item.Field<string>("option_value");
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				string text2 = text.Replace("STRUCT", "").Replace("[", "").Replace("]", "")
					.Replace(" ", "");
				foreach (KeyValuePair<string, string> item2 in (from value in text2.Substring(0, text2.LastIndexOf(")")).Replace("),", ")").Split(')')
					select value.Replace("\"", "").Replace("(", "").Replace(")", "") into value
					select value.Split(',')).ToDictionary((string[] pair) => pair[0], (string[] pair) => pair[1]))
				{
					if (!list.Contains(item2.Key))
					{
						list.Add(item2.Key);
					}
				}
			}
		}
	}

	private void RefreshCustomFieldsGrid()
	{
		customFields = new List<ImportExtendedProperyRow>();
		SetCustomFieldsDataSource();
		foreach (string extendedProperty in GetExtendedProperties())
		{
			ImportExtendedProperyRow importExtendedProperyRow = new ImportExtendedProperyRow();
			importExtendedProperyRow.ExtendedProperty = extendedProperty;
			DocumentationCustomFieldRow documentationCustomFieldRow = docCustomFields.FirstOrDefault((DocumentationCustomFieldRow x) => x.ExtendedProperty?.Equals(extendedProperty) ?? false);
			if (documentationCustomFieldRow != null)
			{
				importExtendedProperyRow.IsSelected = true;
				importExtendedProperyRow.CustomFieldId = documentationCustomFieldRow.CustomFieldId;
				importExtendedProperyRow.SelectedCustomField = documentationCustomFieldRow;
			}
			customFields.Add(importExtendedProperyRow);
		}
		customFieldsGridControl.DataSource = customFields;
	}

	private List<string> GetExtendedProperties()
	{
		return dbConnectUserControl.DatabaseRow.GetExtendedProperties();
	}

	private void SetCustomFieldsDataSource()
	{
		docCustomFields = new List<DocumentationCustomFieldRow>();
		DocumentationCustomFieldRow documentationCustomFieldRow = new DocumentationCustomFieldRow();
		documentationCustomFieldRow.Title = "<i>Add new custom field...</i>";
		documentationCustomFieldRow.CustomFieldId = -1;
		docCustomFields.AddRange(DB.CustomField.GetDocumentationCustomFields(dbConnectUserControl.DatabaseRow?.Id));
		docCustomFields.Add(documentationCustomFieldRow);
		customFieldRepositoryItemLookUpEdit.DataSource = docCustomFields;
	}

	private IEnumerable<ImportExtendedProperyRow> GetAllFields()
	{
		for (int i = 0; i < customFieldsGridView.RowCount; i++)
		{
			int visibleRowHandle = customFieldsGridView.GetVisibleRowHandle(i);
			if (customFieldsGridView.IsDataRow(visibleRowHandle))
			{
				ImportExtendedProperyRow importExtendedProperyRow;
				try
				{
					importExtendedProperyRow = customFieldsGridView.GetRow(visibleRowHandle) as ImportExtendedProperyRow;
				}
				catch (Exception)
				{
					continue;
				}
				if (importExtendedProperyRow.IsSelected)
				{
					yield return importExtendedProperyRow;
				}
			}
		}
	}

	private IEnumerable<ImportExtendedProperyRow> GetNormalizeCustomFields()
	{
		IEnumerable<ImportExtendedProperyRow> allFields = GetAllFields();
		foreach (ImportExtendedProperyRow item in allFields)
		{
			if (!item.IsSelected)
			{
				item.ExtendedProperty = null;
			}
			yield return item;
		}
	}

	private void StartSynchronizing()
	{
		IEnumerable<ImportExtendedProperyRow> normalizeCustomFields = GetNormalizeCustomFields();
		if (isSynchRequired)
		{
			TrackingRunner.Track(delegate
			{
				TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoConnectionBuilder(new TrackingConnectionParameters(dbConnectUserControl.DatabaseRow, dbConnectUserControl.SelectedDatabaseType, SSLTypeHelper.GetSelectedSSLType(dbConnectUserControl.SelectedDatabaseType, dbConnectUserControl.GetSSLTypeValue(), dbConnectUserControl.DatabaseRow.SSLSettings), dbConnectUserControl.GetConnectionType()), new TrackingUserParameters(), new TrackingDataedoParameters()), databaseId.HasValue ? TrackingEventEnum.UpdateRun : TrackingEventEnum.ImportRun);
			});
			DateTime utcNow = DateTime.UtcNow;
			SetBackNextButtonsEnability(completionWizardPage, enabled: false);
			canCloseWindow = false;
			metadataTreeList?.BeginUpdate();
			DbConnectUserControlNew dbConnectUserControlNew = dbConnectUserControl;
			IEnumerable<DocumentationCustomFieldRow> enumerable = from x in normalizeCustomFields
				where x.IsSelected
				select x.SelectedCustomField;
			bool saveFilter = filterInfoUserControl.SaveFilter;
			bool isPersonalSettingsInsert = saveAsComboBoxEdit.SelectedIndex == 1 || (isCopyingConnection == true && dbConnectUserControl.SaveProfileSelectedIndex == 1);
			DBTreeNode focusedNode = mainControl.GetFocusedNode();
			dbConnectUserControlNew.Save(enumerable, saveFilter, isPersonalSettingsInsert, focusedNode != null && focusedNode.DatabaseType == SharedDatabaseTypeEnum.DatabaseType.Manual);
			DatabaseRow databaseRow = dbConnectUserControl.DatabaseRow;
			if (!databaseRow.Id.HasValue)
			{
				metadataTreeList?.EndUpdate();
				canCloseWindow = true;
				completionWizardPage.Visible = false;
				GeneralMessageBoxesHandling.Show("Inserting of a new database to the repository was unsuccessful." + Environment.NewLine + "Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this);
				DateTime utcNow2 = DateTime.UtcNow;
				string duration = (utcNow2 - utcNow).TotalSeconds.ToString();
				TrackingRunner.Track(delegate
				{
					TrackingService.MakeAsyncRequest(new ParametersWithUserDataedoConnectionBuilderEventSpecificTime(new TrackingConnectionParameters(databaseRow, databaseRow.Type, SSLTypeHelper.GetSelectedSSLType(databaseRow), ConnectionTypeService.GetConnectionType(databaseRow)), new TrackingUserParameters(), new TrackingDataedoParameters(), duration), databaseId.HasValue ? TrackingEventEnum.ImportFailed : TrackingEventEnum.UpdateFailed);
				});
				Close();
				return;
			}
			int updateId = 0;
			if (!StaticData.IsProjectFile)
			{
				updateId = DB.SchemaImportsAndChanges.InsertSchemaUpdateRow(dbConnectUserControl.DatabaseRow, (!dbConnectUserControl.IsDBAdded) ? SchemaUpdateTypeEnum.SchemaUpdateType.Update : SchemaUpdateTypeEnum.SchemaUpdateType.Import, this);
			}
			bool flag = DatabaseSupportFactory.GetDatabaseSupport(DatabaseType)?.CanImportDependencies ?? false;
			synchronize.Synchronize(dbConnectUserControl.DatabaseRow, importFilterControl.IsFullReimport, updateEntireDocumentation: false, dbConnectUserControl.IsDBAdded || isCopyingConnection == true, (from x in normalizeCustomFields
				where x.IsSelected
				select x.SelectedCustomField).ToArray(), updateId, null, flag && importDependenciesCheckEdit.Checked);
			metadataTreeList?.EndUpdate();
		}
		else
		{
			DbConnectUserControlNew dbConnectUserControlNew2 = dbConnectUserControl;
			IEnumerable<DocumentationCustomFieldRow> enumerable2 = from x in normalizeCustomFields
				where x.IsSelected
				select x.SelectedCustomField;
			bool saveFilter2 = filterInfoUserControl.SaveFilter;
			bool isPersonalSettingsInsert2 = saveAsComboBoxEdit.SelectedIndex == 1;
			DBTreeNode focusedNode2 = mainControl.GetFocusedNode();
			dbConnectUserControlNew2.Save(enumerable2, saveFilter2, isPersonalSettingsInsert2, focusedNode2 != null && focusedNode2.DatabaseType == SharedDatabaseTypeEnum.DatabaseType.Manual);
			if (metadataTreeList != null)
			{
				metadataTreeList.BeginUpdate();
				this.SynchFinishedEvent?.Invoke(null, null);
				metadataTreeList.EndUpdate();
			}
			if (!StaticData.IsProjectFile)
			{
				DB.SchemaImportsAndChanges.InsertSchemaUpdateRow(dbConnectUserControl.DatabaseRow, (!dbConnectUserControl.IsDBAdded) ? SchemaUpdateTypeEnum.SchemaUpdateType.Update : SchemaUpdateTypeEnum.SchemaUpdateType.Import, this);
			}
			completionWizardPage.Visible = false;
			Close();
		}
	}

	private void SetDBMSPickerPage()
	{
		connectAndSynchWizardControl.SelectedPage = dbmsPickerWizardPage;
	}

	private void connectAndSynchWizardControl_PrevClick(object sender, WizardCommandButtonClickEventArgs e)
	{
		if (e.Page.Equals(selectObjectsSynchWizardPage))
		{
			connectAndSynchWizardControl.SelectedPage = (advancedCheckEdit.Checked ? filterWizardPage : ((dbConnectUserControl.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.InterfaceTables && interfaceTablesImportErrorsUserControl.HasAnyErrorsOrWarnings()) ? interfaceTablesValidationWizardPage : connectionWizardPage));
			e.Handled = true;
		}
		else if (e.Page.Equals(filterWizardPage))
		{
			bool canImportToCustomFields = DatabaseSupportFactory.GetDatabaseSupport(dbConnectUserControl.DatabaseRow.Type).CanImportToCustomFields;
			connectAndSynchWizardControl.SelectedPage = (canImportToCustomFields ? customFieldsWizardPage : connectionWizardPage);
			e.Handled = true;
		}
		else if (e.Page.Equals(connectionWizardPage))
		{
			if (DatabaseSupportFactory.IsAdditionalSelectionType(dbConnectUserControl.SelectedDatabaseType))
			{
				connectAndSynchWizardControl.SelectedPage = dbmsAdditionalPickerWizardPage;
			}
			else
			{
				connectAndSynchWizardControl.SelectedPage = dbmsPickerWizardPage;
			}
			e.Handled = true;
		}
		else if (e.Page.Equals(fileImportDestinationWizardPage))
		{
			connectAndSynchWizardControl.SelectedPage = dbmsPickerWizardPage;
			e.Handled = true;
		}
		else if (e.Page.Equals(customFieldsWizardPage))
		{
			connectAndSynchWizardControl.SelectedPage = ((dbConnectUserControl.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.InterfaceTables && interfaceTablesImportErrorsUserControl.HasAnyErrorsOrWarnings()) ? interfaceTablesValidationWizardPage : connectionWizardPage);
			e.Handled = true;
		}
		else if (e.Page.Equals(interfaceTablesValidationWizardPage))
		{
			connectAndSynchWizardControl.SelectedPage = connectionWizardPage;
			e.Handled = true;
		}
	}

	private void connectAndSynchWizardControl_CancelClick(object sender, CancelEventArgs e)
	{
		if (connectAndSynchWizardControl.SelectedPage.Equals(filterWizardPage))
		{
			connectToDatabase.Cancel();
			filterWizardPage.AllowCancel = false;
		}
		else if (connectAndSynchWizardControl.SelectedPage.Equals(completionWizardPage))
		{
			synchronize.Cancel();
			completionWizardPage.AllowCancel = false;
		}
	}

	private void ConnectAndSynchForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (metadataTreeList == null && !isSynchRequired && dbConnectUserControl.DatabaseRow.Id.HasValue)
		{
			this.AddDatabaseEvent?.Invoke(null, new IdEventArgs(dbConnectUserControl.DatabaseRow.Id.Value));
		}
		if (canCloseWindow)
		{
			e.Cancel = false;
			dbConnectUserControl?.DatabaseRow?.CloseConnection();
		}
		else
		{
			e.Cancel = true;
		}
	}

	private void buttonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
		dbConnectUserControl.SetNewDBRowValues(forGettingDatabasesList: true);
		List<string> databases = dbConnectUserControl.DatabaseRow.GetDatabases();
		CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		if (databases == null)
		{
			GeneralMessageBoxesHandling.Show("Unable to connect to server", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this);
			return;
		}
		string title = ((dbConnectUserControl.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Oracle) ? "Schemas list" : "Databases list");
		ListForm listForm = new ListForm(databases, title);
		if (listForm.ShowDialog(this, setCustomMessageDefaultOwner: true) == DialogResult.OK)
		{
			(sender as ButtonEdit).EditValue = listForm.SelectedValue;
		}
	}

	private void saveImportCommandLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		DataedoCommandsVersion1 dataedoCommandsVersion = new DataedoCommandsVersion1();
		Dataedo.App.Tools.CommandLine.Xml.Import import = ImportCommandBuilder.CreateImportCommandObject(StaticData.RepositoryType, LastConnectionInfo.LOGIN_INFO, dbConnectUserControl.DatabaseRow, CommandsWithTimeout.Timeout, importFilterControl.IsFullReimport, updateEntireDocumentation: false, importFilterControl.GetRulesCollection(), (from x in customFields
			where x.SelectedCustomField != null
			select x.SelectedCustomField).ToList());
		dataedoCommandsVersion.Commands.Add(import);
		SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = "Dataedo commands file (*." + CommandBuilderBase.CommandsFileExtension + ")|*." + CommandBuilderBase.CommandsFileExtension;
		string text = null;
		text = ((!dbConnectUserControl.DatabaseRow.Id.HasValue) ? "Import from" : "Update from");
		string text2 = import.SourceDatabaseConnection.GetDatabaseFull();
		if (text2.Length > 200)
		{
			text2 = import.SourceDatabaseConnection.GetHost();
		}
		string path = "Dataedo " + Paths.RemoveInvalidFilePathCharacters(text + " " + text2 + " ", "_", removeInvalidFileNameCharsOnly: true) + "{DateTime:yyyy-MM-dd}.log";
		string fileName = text + Paths.RemoveInvalidFilePathCharacters(" " + text2 + "." + CommandBuilderBase.CommandsFileExtension, "_", removeInvalidFileNameCharsOnly: true);
		dataedoCommandsVersion.Settings.LogFile.Path = path;
		dataedoCommandsVersion.Settings.LogFile.PathAlternative = new XmlCommentObject("<Path>{MyDocuments}\\\\Dataedo\\\\Logs\\\\" + dataedoCommandsVersion.Settings.LogFile.Path + "</Path>");
		saveFileDialog.FileName = fileName;
		if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
		{
			dataedoCommandsVersion.SaveCommandsXml(saveFileDialog.FileName, FileMode.Create);
		}
	}

	private void selectedCustomFieldsRepositoryItemCheckEdit_EditValueChanged(object sender, EventArgs e)
	{
		customFieldsGridView.CloseEditor();
	}

	private void customFieldsGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (customFieldsGridView.GetRow(e.RowHandle) is ImportExtendedProperyRow importExtendedProperyRow)
		{
			if (importExtendedProperyRow.IsSelected)
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.GridGridRowBackColor;
				e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridGridRowForeColor;
			}
			else
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
				e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridDisabledGridRowForeColor;
			}
		}
	}

	private void customFieldsGridView_ShowingEditor(object sender, CancelEventArgs e)
	{
		ImportExtendedProperyRow importExtendedProperyRow = customFieldsGridView.GetRow(customFieldsGridView.FocusedRowHandle) as ImportExtendedProperyRow;
		GridColumn focusedColumn = customFieldsGridView.FocusedColumn;
		if (titleCustomFieldsGridColumn.Equals(focusedColumn) && !importExtendedProperyRow.IsSelected)
		{
			e.Cancel = true;
		}
	}

	private void defineCustomFieldsSimpleButton_Click(object sender, EventArgs e)
	{
		DefineCustomFields();
	}

	private void DefineCustomFields()
	{
		try
		{
			if (mainControl.ContinueAfterPossibleChanges())
			{
				mainControl.EditCustomFields();
				RefreshCustomFieldsGridOnCustomFieldsChange();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, this);
		}
	}

	private void customFieldsGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
	}

	private void customFieldRepositoryItemLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void customFieldsGridView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		ImportExtendedProperyRow row = customFieldsGridView.GetRow(e.RowHandle) as ImportExtendedProperyRow;
		if (e.Column.Equals(titleCustomFieldsGridColumn))
		{
			row.SelectedCustomField = docCustomFields.FirstOrDefault((DocumentationCustomFieldRow x) => x.CustomFieldId == row.CustomFieldId)?.GetCopy();
		}
	}

	private void customFieldsGridView_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		if (e.Column.Equals(titleCustomFieldsGridColumn))
		{
			customFieldsGridView.GetRow(e.RowHandle);
			if ((int)e.Value < 0)
			{
				DefineCustomFields();
			}
		}
	}

	private void customFieldRepositoryItemLookUpEdit_CustomDrawCell(object sender, LookUpCustomDrawCellArgs e)
	{
		e.DrawHtmlText(e.DisplayText);
		e.Handled = true;
	}

	private void connectAndSynchWizardControl_SelectedPageChanging(object sender, WizardPageChangingEventArgs e)
	{
		if (e.PrevPage.Equals(selectNewDatabaseTitleWizardPage) && e.Direction == Direction.Forward)
		{
			e.Cancel = true;
		}
		else if (e.Page.Equals(connectionWizardPage))
		{
			StaticData.CrashedDatabaseType = dbConnectUserControl.SelectedDatabaseType;
			SetConnectionInfoText();
		}
		else if (e.Page.Equals(customFieldsWizardPage))
		{
			SetCustomFieldsWizardPageTexts();
		}
		else if (e.Page.Equals(fileImportDestinationWizardPage) && e.Direction == Direction.Forward)
		{
			if (DataLakeTypeEnum.StringToType(importSelectionUserControl.GetFocusedDBMSGridModel()?.Type) == DataLakeTypeEnum.DataLakeType.JSON)
			{
				fileImportDestinationWizardPage.Text = "Select documentation and folder";
			}
			else
			{
				fileImportDestinationWizardPage.Text = "Select documentation";
			}
		}
		else if (e.PrevPage.Equals(selectNewDatabaseTitleWizardPage) && e.Direction == Direction.Backward && SharedDatabaseTypeEnum.IsCloudStorage(dbConnectUserControl?.DatabaseRow?.Type))
		{
			e.Page = connectionWizardPage;
		}
	}

	private void SetCustomFieldsWizardPageTexts()
	{
		if (importSelectionUserControl.GetFocusedDBMSGridModel().IsDatabase && importSelectionUserControl.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.Tableau)
		{
			customFieldsWizardPage.DescriptionText = "Define attributes you want to import to Dataedo custom fields.";
			customFieldsWizardPage.Text = "Attributes";
			extendedPropertyCustomFieldsGridColumn.Caption = "Attribute";
			extendedPropertiesInfoUserControl.Description = "Select \"reimport all objects\" on the next page to make sure to import all attributes.";
			extendedPropertiesInfoLayoutControlItem.Visibility = LayoutVisibility.Always;
		}
		else if (DatabaseType == SharedDatabaseTypeEnum.DatabaseType.InterfaceTables)
		{
			customFieldsWizardPage.Text = "Custom fields";
			customFieldsWizardPage.DescriptionText = "Please map fields from Interface Tables to Dataedo custom fields.";
			extendedPropertyCustomFieldsGridColumn.Caption = "Interface table field";
			extendedPropertiesInfoLayoutControlItem.Visibility = LayoutVisibility.Never;
		}
		else
		{
			customFieldsWizardPage.DescriptionText = "Define extended properties you want to import to Dataedo custom fields.";
			customFieldsWizardPage.Text = "Extended properties";
			extendedPropertyCustomFieldsGridColumn.Caption = "Extended property";
			extendedPropertiesInfoUserControl.Description = "Select \"reimport all objects\" on the next page to make sure to import all extended properties.";
			extendedPropertiesInfoLayoutControlItem.Visibility = LayoutVisibility.Always;
		}
	}

	private void SetConnectionInfoText()
	{
		if (!dbConnectUserControl.SelectedDatabaseType.HasValue)
		{
			learnMoreInfoLayoutControlItem.Visibility = LayoutVisibility.Never;
			return;
		}
		SharedDatabaseTypeEnum.DatabaseType? parentDatabaseType = databaseSubtypesDbmsPickerUserControl.ParentDatabaseType;
		SharedDatabaseTypeEnum.DatabaseType? databaseType;
		if (parentDatabaseType.HasValue)
		{
			SharedDatabaseTypeEnum.DatabaseType valueOrDefault = parentDatabaseType.GetValueOrDefault();
			if ((uint)(valueOrDefault - 49) <= 2u)
			{
				databaseType = databaseSubtypesDbmsPickerUserControl.ParentDatabaseType;
				goto IL_0064;
			}
		}
		databaseType = dbConnectUserControl.SelectedDatabaseType;
		goto IL_0064;
		IL_0064:
		string text = DatabaseSupportFactory.GetDatabaseSupport(databaseType)?.DocumentationLink;
		if (string.IsNullOrEmpty(text))
		{
			learnMoreInfoLayoutControlItem.Visibility = LayoutVisibility.Never;
		}
		else
		{
			learnMoreInfoLayoutControlItem.Visibility = LayoutVisibility.Always;
		}
		string connectionAdditionalText = dbConnectUserControl.GetConnectionAdditionalText();
		string text2 = ((dbConnectUserControl.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.InterfaceTables) ? ("<href=" + text + ">Learn more</href> about importing from Interface Tables.") : ("<href=" + text + ">Learn more</href> about supported versions and <href=" + text + ">how to connect</href>."));
		if (string.IsNullOrEmpty(connectionAdditionalText))
		{
			learnMoreInfoUserControl.Description = text2;
			return;
		}
		learnMoreInfoUserControl.Description = string.Join(" ", dbConnectUserControl.GetConnectionAdditionalText(), text2).Trim();
	}

	internal void SetPageBackFromImportFile(int databaseId, SharedObjectTypeEnum.ObjectType objectType)
	{
		DBMSGridModel focusedDBMSGridModel = importSelectionUserControl.GetFocusedDBMSGridModel();
		if (focusedDBMSGridModel != null)
		{
			chooseDocumentationObjectTypeControl.SetParameters(DataLakeTypeEnum.StringToType(focusedDBMSGridModel.Type), databaseId, objectType);
			connectAndSynchWizardControl.SelectedPage = fileImportDestinationWizardPage;
		}
		else
		{
			connectAndSynchWizardControl.SelectedPage = dbmsPickerWizardPage;
		}
	}

	private bool CheckSnowflakeRoleForDependencies()
	{
		if (dbConnectUserControl.SelectedDatabaseType != SharedDatabaseTypeEnum.DatabaseType.Snowflake || !importDependenciesCheckEdit.Checked)
		{
			return true;
		}
		if (dbConnectUserControl.ConnectorControl is SnowflakeConnectorControl snowflakeConnectorControl && !snowflakeConnectorControl.IsAccountAdminRoleSelected() && GeneralMessageBoxesHandling.Show("To import dependencies you need to select role ACCOUNTADMIN or have access to SNOWFLAKE.ACCOUNT_USAGE.OBJECT_DEPENDENCIES view. " + Environment.NewLine + "Otherwise dependencies will not be imported. " + Environment.NewLine + Environment.NewLine + "Do you want to continue with the currently selected role?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 2, this).DialogResult != DialogResult.Yes)
		{
			return false;
		}
		return true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.ConnectAndSynchForm));
		this.toolTipController = new Dataedo.App.UserControls.ToolTipControllerUserControl(this.components);
		this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true);
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.checkEdit1 = new DevExpress.XtraEditors.CheckEdit();
		this.pictureEdit2 = new DevExpress.XtraEditors.PictureEdit();
		this.customFieldsWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.customFieldsLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.defineCustomFieldsSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.extendedPropertiesInfoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.customFieldsGridControl = new DevExpress.XtraGrid.GridControl();
		this.customFieldsGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.selectedCustomFieldsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.selectedCustomFieldsRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.titleCustomFieldsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.customFieldRepositoryItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.extendedPropertyCustomFieldsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.extendedPropertyRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.extendedPropertiesInfoLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.defineCustomFieldLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.repositoryItemGridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.progressWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.readingDatabaseLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.messageLabel = new DevExpress.XtraEditors.LabelControl();
		this.objectLabel = new DevExpress.XtraEditors.LabelControl();
		this.readingObjectsProgressBar = new DevExpress.XtraEditors.ProgressBarControl();
		this.layoutControlGroup9 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
		this.readingObjectsProgressBarLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.documentationTitleWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.documentationTitleLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.documentationTitleTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControlGroup10 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.documentationTitleLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.selectObjectsSynchWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.synchronizeGrid = new DevExpress.XtraGrid.GridControl();
		this.synchronizeGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.synchronizeObjectGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.synchronizeTableRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.statusObjectGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.classSynchronizeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.typeSynchronizeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.schemaSynchronizeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameSynchronizeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.gridView2 = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.layout = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.saveImportCommandLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.updatedObjectCounterlabel = new DevExpress.XtraEditors.LabelControl();
		this.newObjectCounterlabel = new DevExpress.XtraEditors.LabelControl();
		this.selectedObjectCounterlabel = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup7 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem20 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem23 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem24 = new DevExpress.XtraLayout.LayoutControlItem();
		this.saveImportCommandLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.objectsStatusesLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.selectObjectsLabel = new DevExpress.XtraEditors.LabelControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.checkNoneHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.checkAllHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.synchStateCounterInfoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.statusLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.checkAllLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.checkNoneLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.checkEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
		this.selectObjectsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.filterWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.generatingDocLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.importFilterControl = new Dataedo.App.UserControls.ImportFilter.ImportFilterControl();
		this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.filterLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.completionWizardPage = new DevExpress.XtraWizard.CompletionWizardPage();
		this.saveUpdateCommandLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.synchronizationLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.synchronizeProgressBar = new DevExpress.XtraEditors.ProgressBarControl();
		this.objectNameLabel = new DevExpress.XtraEditors.LabelControl();
		this.synchronizeObjectLabel = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup8 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.synchStateLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.synchObjectlayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.progressLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem10 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.connectionWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.helpIconImportDependencies = new Dataedo.App.UserControls.HelpIconUserControl();
		this.importDependenciesCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.learnMoreInfoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.helpIconUserControl = new Dataedo.App.UserControls.HelpIconUserControl();
		this.saveAsComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.filterInfoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.advancedCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.dbConnectUserControl = new Dataedo.App.UserControls.ConnectorsControls.DbConnectUserControlNew();
		this.layoutControlGroup11 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.advancedSettingsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.advancedSettingsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.helpIconLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.learnMoreInfoLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.importDependenciesLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.filterInfoLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.saveAsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.importDependenciesEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.importDependenciesHelpIconLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.connectAndSynchWizardControl = new DevExpress.XtraWizard.WizardControl();
		this.dbmsPickerWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.importSelectionUserControl = new Dataedo.App.UserControls.ImportSelectionUserControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.dbmsAdditionalPickerWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.databaseSubtypesDbmsPickerUserControl = new Dataedo.App.UserControls.DBMSPickerUserControl();
		this.fileImportDestinationWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.chooseDocumentationObjectTypeControl = new Dataedo.App.UserControls.ChooseDocumentationObjectTypeUserControl();
		this.selectNewDatabaseTitleWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.nonCustomizableLayoutControl2 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.newDatabaseTitleTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.newDatabaseTitleLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.interfaceTablesValidationWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.interfaceTablesImportErrorsUserControl = new Dataedo.App.UserControls.InterfaceTables.InterfaceTablesImportErrorsUserControl();
		this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.readingObjectsMarqueeProgressBarControl = new DevExpress.XtraEditors.MarqueeProgressBarControl();
		this.readingObjectsMarqueeProgressBarControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pictureEdit2.Properties).BeginInit();
		this.customFieldsWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.customFieldsLayoutControl).BeginInit();
		this.customFieldsLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.customFieldsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.selectedCustomFieldsRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldRepositoryItemLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.extendedPropertyRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.extendedPropertiesInfoLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.defineCustomFieldLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEdit1View).BeginInit();
		this.progressWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.readingDatabaseLayoutControl).BeginInit();
		this.readingDatabaseLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.readingObjectsProgressBar.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.readingObjectsProgressBarLayoutControlItem).BeginInit();
		this.documentationTitleWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.documentationTitleLayoutControl).BeginInit();
		this.documentationTitleLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.documentationTitleTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.documentationTitleLayoutControlItem).BeginInit();
		this.selectObjectsSynchWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.synchronizeGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.synchronizeGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.synchronizeTableRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout).BeginInit();
		this.layout.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem20).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem23).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveImportCommandLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectsStatusesLayoutControl).BeginInit();
		this.objectsStatusesLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.checkNoneHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkAllHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.statusLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkAllLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkNoneLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem19).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.selectObjectsLayoutControlItem).BeginInit();
		this.filterWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.generatingDocLayoutControl).BeginInit();
		this.generatingDocLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.filterLayoutControlItem).BeginInit();
		this.completionWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.synchronizationLayoutControl).BeginInit();
		this.synchronizationLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.synchronizeProgressBar.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.synchStateLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.synchObjectlayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem10).BeginInit();
		this.connectionWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.importDependenciesCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveAsComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.advancedCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.advancedSettingsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.advancedSettingsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.helpIconLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.learnMoreInfoLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.importDependenciesLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.filterInfoLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveAsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.importDependenciesEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.importDependenciesHelpIconLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectAndSynchWizardControl).BeginInit();
		this.connectAndSynchWizardControl.SuspendLayout();
		this.dbmsPickerWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		this.dbmsAdditionalPickerWizardPage.SuspendLayout();
		this.fileImportDestinationWizardPage.SuspendLayout();
		this.selectNewDatabaseTitleWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl2).BeginInit();
		this.nonCustomizableLayoutControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.newDatabaseTitleTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.newDatabaseTitleLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		this.interfaceTablesValidationWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.readingObjectsMarqueeProgressBarControl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.readingObjectsMarqueeProgressBarControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.toolTipController.ReshowDelay = 10000;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 0);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(582, 397);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem8.AllowHotTrack = false;
		this.emptySpaceItem8.Location = new System.Drawing.Point(0, 375);
		this.emptySpaceItem8.Name = "emptySpaceItem8";
		this.emptySpaceItem8.Size = new System.Drawing.Size(582, 22);
		this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
		this.checkEdit1.Location = new System.Drawing.Point(12, 385);
		this.checkEdit1.Name = "checkEdit1";
		this.checkEdit1.Properties.Caption = "Advanced settings";
		this.checkEdit1.Size = new System.Drawing.Size(111, 20);
		this.checkEdit1.TabIndex = 24;
		this.pictureEdit2.Location = new System.Drawing.Point(127, 385);
		this.pictureEdit2.Name = "pictureEdit2";
		this.pictureEdit2.Properties.AllowFocused = false;
		this.pictureEdit2.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.pictureEdit2.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.pictureEdit2.Properties.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.pictureEdit2.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.pictureEdit2.Properties.ShowMenu = false;
		this.pictureEdit2.Size = new System.Drawing.Size(20, 20);
		this.pictureEdit2.TabIndex = 26;
		this.pictureEdit2.ToolTip = "Checking this option will allow you to set timeout, change filter settings and force full reimport of data structure.";
		this.pictureEdit2.ToolTipController = this.toolTipController;
		this.customFieldsWizardPage.Controls.Add(this.customFieldsLayoutControl);
		this.customFieldsWizardPage.DescriptionText = "Define extended properties you want to import to Dataedo custom fields.";
		this.customFieldsWizardPage.Name = "customFieldsWizardPage";
		this.customFieldsWizardPage.Size = new System.Drawing.Size(690, 512);
		this.customFieldsWizardPage.Text = "Extended properties";
		this.customFieldsLayoutControl.AllowCustomization = false;
		this.customFieldsLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.customFieldsLayoutControl.Controls.Add(this.defineCustomFieldsSimpleButton);
		this.customFieldsLayoutControl.Controls.Add(this.extendedPropertiesInfoUserControl);
		this.customFieldsLayoutControl.Controls.Add(this.customFieldsGridControl);
		this.customFieldsLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.customFieldsLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.customFieldsLayoutControl.Name = "customFieldsLayoutControl";
		this.customFieldsLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(4172, 333, 250, 350);
		this.customFieldsLayoutControl.OptionsView.UseDefaultDragAndDropRendering = false;
		this.customFieldsLayoutControl.Root = this.layoutControlGroup1;
		this.customFieldsLayoutControl.Size = new System.Drawing.Size(690, 512);
		this.customFieldsLayoutControl.TabIndex = 32;
		this.customFieldsLayoutControl.Text = "layoutControl2";
		this.defineCustomFieldsSimpleButton.Location = new System.Drawing.Point(2, 2);
		this.defineCustomFieldsSimpleButton.Name = "defineCustomFieldsSimpleButton";
		this.defineCustomFieldsSimpleButton.Size = new System.Drawing.Size(107, 22);
		this.defineCustomFieldsSimpleButton.StyleController = this.customFieldsLayoutControl;
		this.defineCustomFieldsSimpleButton.TabIndex = 32;
		this.defineCustomFieldsSimpleButton.Text = "Define custom fields";
		this.defineCustomFieldsSimpleButton.Click += new System.EventHandler(defineCustomFieldsSimpleButton_Click);
		this.extendedPropertiesInfoUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.extendedPropertiesInfoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.extendedPropertiesInfoUserControl.Description = "Select \"reimport all objects\" on the next page to make sure to import all extended properties.";
		this.extendedPropertiesInfoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.extendedPropertiesInfoUserControl.Image = Dataedo.App.Properties.Resources.about_16;
		this.extendedPropertiesInfoUserControl.Location = new System.Drawing.Point(2, 478);
		this.extendedPropertiesInfoUserControl.MaximumSize = new System.Drawing.Size(0, 32);
		this.extendedPropertiesInfoUserControl.MinimumSize = new System.Drawing.Size(564, 32);
		this.extendedPropertiesInfoUserControl.Name = "extendedPropertiesInfoUserControl";
		this.extendedPropertiesInfoUserControl.Size = new System.Drawing.Size(686, 32);
		this.extendedPropertiesInfoUserControl.TabIndex = 31;
		this.customFieldsGridControl.Location = new System.Drawing.Point(2, 47);
		this.customFieldsGridControl.MainView = this.customFieldsGridView;
		this.customFieldsGridControl.Name = "customFieldsGridControl";
		this.customFieldsGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.selectedCustomFieldsRepositoryItemCheckEdit, this.extendedPropertyRepositoryItemTextEdit, this.customFieldRepositoryItemLookUpEdit });
		this.customFieldsGridControl.Size = new System.Drawing.Size(686, 427);
		this.customFieldsGridControl.TabIndex = 4;
		this.customFieldsGridControl.ToolTipController = this.toolTipController;
		this.customFieldsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.customFieldsGridView, this.gridView1 });
		this.customFieldsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.selectedCustomFieldsGridColumn, this.titleCustomFieldsGridColumn, this.extendedPropertyCustomFieldsGridColumn });
		this.customFieldsGridView.GridControl = this.customFieldsGridControl;
		this.customFieldsGridView.Name = "customFieldsGridView";
		this.customFieldsGridView.OptionsFilter.AllowFilterEditor = false;
		this.customFieldsGridView.OptionsView.ShowGroupPanel = false;
		this.customFieldsGridView.OptionsView.ShowIndicator = false;
		this.customFieldsGridView.RowHighlightingIsEnabled = true;
		this.customFieldsGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(customFieldsGridView_CustomDrawCell);
		this.customFieldsGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(customFieldsGridView_PopupMenuShowing);
		this.customFieldsGridView.ShowingEditor += new System.ComponentModel.CancelEventHandler(customFieldsGridView_ShowingEditor);
		this.customFieldsGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(customFieldsGridView_CellValueChanged);
		this.customFieldsGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(customFieldsGridView_CellValueChanging);
		this.selectedCustomFieldsGridColumn.Caption = " ";
		this.selectedCustomFieldsGridColumn.ColumnEdit = this.selectedCustomFieldsRepositoryItemCheckEdit;
		this.selectedCustomFieldsGridColumn.FieldName = "IsSelected";
		this.selectedCustomFieldsGridColumn.MaxWidth = 20;
		this.selectedCustomFieldsGridColumn.Name = "selectedCustomFieldsGridColumn";
		this.selectedCustomFieldsGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.selectedCustomFieldsGridColumn.OptionsFilter.AllowFilter = false;
		this.selectedCustomFieldsGridColumn.Visible = true;
		this.selectedCustomFieldsGridColumn.VisibleIndex = 0;
		this.selectedCustomFieldsGridColumn.Width = 20;
		this.selectedCustomFieldsRepositoryItemCheckEdit.AutoHeight = false;
		this.selectedCustomFieldsRepositoryItemCheckEdit.Name = "selectedCustomFieldsRepositoryItemCheckEdit";
		this.selectedCustomFieldsRepositoryItemCheckEdit.EditValueChanged += new System.EventHandler(selectedCustomFieldsRepositoryItemCheckEdit_EditValueChanged);
		this.titleCustomFieldsGridColumn.Caption = "Custom field";
		this.titleCustomFieldsGridColumn.ColumnEdit = this.customFieldRepositoryItemLookUpEdit;
		this.titleCustomFieldsGridColumn.FieldName = "CustomFieldId";
		this.titleCustomFieldsGridColumn.Name = "titleCustomFieldsGridColumn";
		this.titleCustomFieldsGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.titleCustomFieldsGridColumn.OptionsFilter.AllowFilter = false;
		this.titleCustomFieldsGridColumn.Visible = true;
		this.titleCustomFieldsGridColumn.VisibleIndex = 2;
		this.customFieldRepositoryItemLookUpEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.customFieldRepositoryItemLookUpEdit.AutoHeight = false;
		this.customFieldRepositoryItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.customFieldRepositoryItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Title", "Title")
		});
		this.customFieldRepositoryItemLookUpEdit.DisplayMember = "Title";
		this.customFieldRepositoryItemLookUpEdit.Name = "customFieldRepositoryItemLookUpEdit";
		this.customFieldRepositoryItemLookUpEdit.NullText = "";
		this.customFieldRepositoryItemLookUpEdit.ShowFooter = false;
		this.customFieldRepositoryItemLookUpEdit.ShowHeader = false;
		this.customFieldRepositoryItemLookUpEdit.ShowLines = false;
		this.customFieldRepositoryItemLookUpEdit.ValueMember = "CustomFieldId";
		this.customFieldRepositoryItemLookUpEdit.CustomDrawCell += new DevExpress.XtraEditors.Popup.LookUpCustomDrawCellEventHandler(customFieldRepositoryItemLookUpEdit_CustomDrawCell);
		this.customFieldRepositoryItemLookUpEdit.EditValueChanged += new System.EventHandler(customFieldRepositoryItemLookUpEdit_EditValueChanged);
		this.extendedPropertyCustomFieldsGridColumn.Caption = "Extended property";
		this.extendedPropertyCustomFieldsGridColumn.ColumnEdit = this.extendedPropertyRepositoryItemTextEdit;
		this.extendedPropertyCustomFieldsGridColumn.FieldName = "ExtendedProperty";
		this.extendedPropertyCustomFieldsGridColumn.Name = "extendedPropertyCustomFieldsGridColumn";
		this.extendedPropertyCustomFieldsGridColumn.OptionsColumn.AllowEdit = false;
		this.extendedPropertyCustomFieldsGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.extendedPropertyCustomFieldsGridColumn.OptionsColumn.ReadOnly = true;
		this.extendedPropertyCustomFieldsGridColumn.OptionsFilter.AllowFilter = false;
		this.extendedPropertyCustomFieldsGridColumn.Visible = true;
		this.extendedPropertyCustomFieldsGridColumn.VisibleIndex = 1;
		this.extendedPropertyRepositoryItemTextEdit.AutoHeight = false;
		this.extendedPropertyRepositoryItemTextEdit.Name = "extendedPropertyRepositoryItemTextEdit";
		this.gridView1.GridControl = this.customFieldsGridControl;
		this.gridView1.Name = "gridView1";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.layoutControlItem2, this.extendedPropertiesInfoLayoutControlItem, this.emptySpaceItem3, this.defineCustomFieldLayoutControlItem, this.emptySpaceItem4 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(690, 512);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem2.Control = this.customFieldsGridControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 45);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(690, 431);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.extendedPropertiesInfoLayoutControlItem.Control = this.extendedPropertiesInfoUserControl;
		this.extendedPropertiesInfoLayoutControlItem.Location = new System.Drawing.Point(0, 476);
		this.extendedPropertiesInfoLayoutControlItem.Name = "extendedPropertiesInfoLayoutControlItem";
		this.extendedPropertiesInfoLayoutControlItem.Size = new System.Drawing.Size(690, 36);
		this.extendedPropertiesInfoLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.extendedPropertiesInfoLayoutControlItem.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(111, 0);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(0, 26);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(579, 26);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.defineCustomFieldLayoutControlItem.Control = this.defineCustomFieldsSimpleButton;
		this.defineCustomFieldLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.defineCustomFieldLayoutControlItem.MaxSize = new System.Drawing.Size(111, 26);
		this.defineCustomFieldLayoutControlItem.MinSize = new System.Drawing.Size(111, 26);
		this.defineCustomFieldLayoutControlItem.Name = "defineCustomFieldLayoutControlItem";
		this.defineCustomFieldLayoutControlItem.Size = new System.Drawing.Size(111, 26);
		this.defineCustomFieldLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.defineCustomFieldLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.defineCustomFieldLayoutControlItem.TextVisible = false;
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(0, 26);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(690, 19);
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.repositoryItemGridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.repositoryItemGridLookUpEdit1View.Name = "repositoryItemGridLookUpEdit1View";
		this.repositoryItemGridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.repositoryItemGridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
		this.progressWizardPage.Controls.Add(this.readingDatabaseLayoutControl);
		this.progressWizardPage.DescriptionText = "Reading database ";
		this.progressWizardPage.Name = "progressWizardPage";
		this.progressWizardPage.Size = new System.Drawing.Size(690, 512);
		this.progressWizardPage.Text = "Reading database";
		this.readingDatabaseLayoutControl.AllowCustomization = false;
		this.readingDatabaseLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.readingDatabaseLayoutControl.Controls.Add(this.readingObjectsMarqueeProgressBarControl);
		this.readingDatabaseLayoutControl.Controls.Add(this.messageLabel);
		this.readingDatabaseLayoutControl.Controls.Add(this.objectLabel);
		this.readingDatabaseLayoutControl.Controls.Add(this.readingObjectsProgressBar);
		this.readingDatabaseLayoutControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.readingDatabaseLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.readingDatabaseLayoutControl.Name = "readingDatabaseLayoutControl";
		this.readingDatabaseLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2516, 182, 250, 350);
		this.readingDatabaseLayoutControl.OptionsView.ShareLookAndFeelWithChildren = false;
		this.readingDatabaseLayoutControl.Root = this.layoutControlGroup9;
		this.readingDatabaseLayoutControl.Size = new System.Drawing.Size(690, 82);
		this.readingDatabaseLayoutControl.TabIndex = 17;
		this.readingDatabaseLayoutControl.TabStop = false;
		this.readingDatabaseLayoutControl.Text = "layoutControl1";
		this.messageLabel.Location = new System.Drawing.Point(2, 2);
		this.messageLabel.Name = "messageLabel";
		this.messageLabel.Size = new System.Drawing.Size(686, 13);
		this.messageLabel.TabIndex = 13;
		this.messageLabel.UseMnemonic = false;
		this.objectLabel.Location = new System.Drawing.Point(2, 19);
		this.objectLabel.Name = "objectLabel";
		this.objectLabel.Size = new System.Drawing.Size(686, 13);
		this.objectLabel.TabIndex = 14;
		this.objectLabel.UseMnemonic = false;
		this.readingObjectsProgressBar.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.readingObjectsProgressBar.Location = new System.Drawing.Point(2, 36);
		this.readingObjectsProgressBar.Name = "readingObjectsProgressBar";
		this.readingObjectsProgressBar.Properties.ShowTitle = true;
		this.readingObjectsProgressBar.Properties.Step = 1;
		this.readingObjectsProgressBar.Size = new System.Drawing.Size(686, 18);
		this.readingObjectsProgressBar.TabIndex = 10;
		this.layoutControlGroup9.CustomizationFormText = "Root";
		this.layoutControlGroup9.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup9.GroupBordersVisible = false;
		this.layoutControlGroup9.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.layoutControlItem8, this.layoutControlItem13, this.readingObjectsProgressBarLayoutControlItem, this.readingObjectsMarqueeProgressBarControlLayoutControlItem });
		this.layoutControlGroup9.Name = "Root";
		this.layoutControlGroup9.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup9.Size = new System.Drawing.Size(690, 82);
		this.layoutControlGroup9.TextVisible = false;
		this.layoutControlItem8.Control = this.messageLabel;
		this.layoutControlItem8.CustomizationFormText = "layoutControlItem8";
		this.layoutControlItem8.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem8.Name = "layoutControlItem8";
		this.layoutControlItem8.Size = new System.Drawing.Size(690, 17);
		this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.TextVisible = false;
		this.layoutControlItem13.Control = this.objectLabel;
		this.layoutControlItem13.CustomizationFormText = "layoutControlItem13";
		this.layoutControlItem13.Location = new System.Drawing.Point(0, 17);
		this.layoutControlItem13.Name = "layoutControlItem13";
		this.layoutControlItem13.Size = new System.Drawing.Size(690, 17);
		this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem13.TextVisible = false;
		this.readingObjectsProgressBarLayoutControlItem.Control = this.readingObjectsProgressBar;
		this.readingObjectsProgressBarLayoutControlItem.Location = new System.Drawing.Point(0, 34);
		this.readingObjectsProgressBarLayoutControlItem.Name = "readingObjectsProgressBarLayoutControlItem";
		this.readingObjectsProgressBarLayoutControlItem.Size = new System.Drawing.Size(690, 22);
		this.readingObjectsProgressBarLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.readingObjectsProgressBarLayoutControlItem.TextVisible = false;
		this.documentationTitleWizardPage.Controls.Add(this.documentationTitleLayoutControl);
		this.documentationTitleWizardPage.DescriptionText = "Provide title for your documentation. It can be changed later.";
		this.documentationTitleWizardPage.Name = "documentationTitleWizardPage";
		this.documentationTitleWizardPage.Size = new System.Drawing.Size(690, 512);
		this.documentationTitleWizardPage.Text = "Documentation title";
		this.documentationTitleLayoutControl.AllowCustomization = false;
		this.documentationTitleLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.documentationTitleLayoutControl.Controls.Add(this.documentationTitleTextEdit);
		this.documentationTitleLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.documentationTitleLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.documentationTitleLayoutControl.Name = "documentationTitleLayoutControl";
		this.documentationTitleLayoutControl.Root = this.layoutControlGroup10;
		this.documentationTitleLayoutControl.Size = new System.Drawing.Size(690, 512);
		this.documentationTitleLayoutControl.TabIndex = 0;
		this.documentationTitleLayoutControl.Text = "layoutControl1";
		this.documentationTitleTextEdit.Location = new System.Drawing.Point(106, 12);
		this.documentationTitleTextEdit.Name = "documentationTitleTextEdit";
		this.documentationTitleTextEdit.Size = new System.Drawing.Size(572, 20);
		this.documentationTitleTextEdit.StyleController = this.documentationTitleLayoutControl;
		this.documentationTitleTextEdit.TabIndex = 4;
		this.layoutControlGroup10.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup10.GroupBordersVisible = false;
		this.layoutControlGroup10.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.documentationTitleLayoutControlItem });
		this.layoutControlGroup10.Name = "layoutControlGroup10";
		this.layoutControlGroup10.Size = new System.Drawing.Size(690, 512);
		this.layoutControlGroup10.TextVisible = false;
		this.documentationTitleLayoutControlItem.Control = this.documentationTitleTextEdit;
		this.documentationTitleLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.documentationTitleLayoutControlItem.Name = "documentationTitleLayoutControlItem";
		this.documentationTitleLayoutControlItem.Size = new System.Drawing.Size(670, 492);
		this.documentationTitleLayoutControlItem.Text = "Documentation title";
		this.documentationTitleLayoutControlItem.TextSize = new System.Drawing.Size(91, 13);
		this.selectObjectsSynchWizardPage.Controls.Add(this.synchronizeGrid);
		this.selectObjectsSynchWizardPage.Controls.Add(this.layout);
		this.selectObjectsSynchWizardPage.Controls.Add(this.objectsStatusesLayoutControl);
		this.selectObjectsSynchWizardPage.DescriptionText = "Select objects";
		this.selectObjectsSynchWizardPage.Name = "selectObjectsSynchWizardPage";
		this.selectObjectsSynchWizardPage.Size = new System.Drawing.Size(690, 512);
		this.selectObjectsSynchWizardPage.Text = "Select objects to import";
		this.synchronizeGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.synchronizeGrid.Dock = System.Windows.Forms.DockStyle.Fill;
		this.synchronizeGrid.Location = new System.Drawing.Point(0, 142);
		this.synchronizeGrid.MainView = this.synchronizeGridView;
		this.synchronizeGrid.MenuManager = this.barManager;
		this.synchronizeGrid.Name = "synchronizeGrid";
		this.synchronizeGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.synchronizeTableRepositoryItemCheckEdit });
		this.synchronizeGrid.Size = new System.Drawing.Size(690, 302);
		this.synchronizeGrid.TabIndex = 6;
		this.synchronizeGrid.ToolTipController = this.toolTipController;
		this.synchronizeGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.synchronizeGridView, this.gridView2 });
		this.synchronizeGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.synchronizeObjectGridColumn, this.statusObjectGridColumn, this.classSynchronizeGridColumn, this.typeSynchronizeGridColumn, this.schemaSynchronizeGridColumn, this.nameSynchronizeGridColumn });
		this.synchronizeGridView.GridControl = this.synchronizeGrid;
		this.synchronizeGridView.Name = "synchronizeGridView";
		this.synchronizeGridView.OptionsView.ColumnAutoWidth = false;
		this.synchronizeGridView.OptionsView.ShowAutoFilterRow = true;
		this.synchronizeGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.synchronizeGridView.OptionsView.ShowGroupPanel = false;
		this.synchronizeGridView.OptionsView.ShowIndicator = false;
		this.synchronizeGridView.RowHighlightingIsEnabled = true;
		this.synchronizeObjectGridColumn.Caption = " ";
		this.synchronizeObjectGridColumn.ColumnEdit = this.synchronizeTableRepositoryItemCheckEdit;
		this.synchronizeObjectGridColumn.FieldName = "ToSynchronize";
		this.synchronizeObjectGridColumn.Name = "synchronizeObjectGridColumn";
		this.synchronizeObjectGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.synchronizeObjectGridColumn.OptionsFilter.AllowAutoFilter = false;
		this.synchronizeObjectGridColumn.OptionsFilter.AllowFilter = false;
		this.synchronizeObjectGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.synchronizeObjectGridColumn.Visible = true;
		this.synchronizeObjectGridColumn.VisibleIndex = 0;
		this.synchronizeObjectGridColumn.Width = 26;
		this.synchronizeTableRepositoryItemCheckEdit.AutoHeight = false;
		this.synchronizeTableRepositoryItemCheckEdit.Caption = "Check";
		this.synchronizeTableRepositoryItemCheckEdit.Name = "synchronizeTableRepositoryItemCheckEdit";
		this.statusObjectGridColumn.Caption = "Status";
		this.statusObjectGridColumn.FieldName = "StateToStringForDescription";
		this.statusObjectGridColumn.Name = "statusObjectGridColumn";
		this.statusObjectGridColumn.OptionsColumn.AllowEdit = false;
		this.statusObjectGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.statusObjectGridColumn.OptionsColumn.ReadOnly = true;
		this.statusObjectGridColumn.OptionsFilter.AllowFilter = false;
		this.statusObjectGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.statusObjectGridColumn.Visible = true;
		this.statusObjectGridColumn.VisibleIndex = 1;
		this.statusObjectGridColumn.Width = 100;
		this.classSynchronizeGridColumn.AppearanceCell.Options.UseTextOptions = true;
		this.classSynchronizeGridColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.classSynchronizeGridColumn.Caption = "Class";
		this.classSynchronizeGridColumn.FieldName = "TypeDisplayString";
		this.classSynchronizeGridColumn.Name = "classSynchronizeGridColumn";
		this.classSynchronizeGridColumn.OptionsColumn.AllowEdit = false;
		this.classSynchronizeGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.classSynchronizeGridColumn.OptionsColumn.ReadOnly = true;
		this.classSynchronizeGridColumn.OptionsFilter.AllowFilter = false;
		this.classSynchronizeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.classSynchronizeGridColumn.Visible = true;
		this.classSynchronizeGridColumn.VisibleIndex = 2;
		this.typeSynchronizeGridColumn.AppearanceCell.Options.UseTextOptions = true;
		this.typeSynchronizeGridColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.typeSynchronizeGridColumn.Caption = "Type";
		this.typeSynchronizeGridColumn.FieldName = "SubtypeDisplayString";
		this.typeSynchronizeGridColumn.Name = "typeSynchronizeGridColumn";
		this.typeSynchronizeGridColumn.OptionsColumn.AllowEdit = false;
		this.typeSynchronizeGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.typeSynchronizeGridColumn.OptionsColumn.ReadOnly = true;
		this.typeSynchronizeGridColumn.OptionsFilter.AllowFilter = false;
		this.typeSynchronizeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.typeSynchronizeGridColumn.Visible = true;
		this.typeSynchronizeGridColumn.VisibleIndex = 3;
		this.schemaSynchronizeGridColumn.Caption = "Schema";
		this.schemaSynchronizeGridColumn.FieldName = "Schema";
		this.schemaSynchronizeGridColumn.Name = "schemaSynchronizeGridColumn";
		this.schemaSynchronizeGridColumn.OptionsColumn.AllowEdit = false;
		this.schemaSynchronizeGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.schemaSynchronizeGridColumn.OptionsColumn.ReadOnly = true;
		this.schemaSynchronizeGridColumn.OptionsFilter.AllowFilter = false;
		this.schemaSynchronizeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.schemaSynchronizeGridColumn.Visible = true;
		this.schemaSynchronizeGridColumn.VisibleIndex = 4;
		this.nameSynchronizeGridColumn.Caption = "Name";
		this.nameSynchronizeGridColumn.FieldName = "Name";
		this.nameSynchronizeGridColumn.Name = "nameSynchronizeGridColumn";
		this.nameSynchronizeGridColumn.OptionsColumn.AllowEdit = false;
		this.nameSynchronizeGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.nameSynchronizeGridColumn.OptionsColumn.ReadOnly = true;
		this.nameSynchronizeGridColumn.OptionsFilter.AllowFilter = false;
		this.nameSynchronizeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.nameSynchronizeGridColumn.Visible = true;
		this.nameSynchronizeGridColumn.VisibleIndex = 5;
		this.nameSynchronizeGridColumn.Width = 190;
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(722, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 655);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(722, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 655);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(722, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 655);
		this.gridView2.GridControl = this.synchronizeGrid;
		this.gridView2.Name = "gridView2";
		this.layout.AllowCustomization = false;
		this.layout.BackColor = System.Drawing.Color.Transparent;
		this.layout.Controls.Add(this.saveImportCommandLabelControl);
		this.layout.Controls.Add(this.updatedObjectCounterlabel);
		this.layout.Controls.Add(this.newObjectCounterlabel);
		this.layout.Controls.Add(this.selectedObjectCounterlabel);
		this.layout.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.layout.Location = new System.Drawing.Point(0, 444);
		this.layout.Name = "layout";
		this.layout.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(912, 222, 663, 350);
		this.layout.Root = this.layoutControlGroup7;
		this.layout.Size = new System.Drawing.Size(690, 68);
		this.layout.TabIndex = 5;
		this.layout.Text = "layoutControl2";
		this.saveImportCommandLabelControl.AllowHtmlString = true;
		this.saveImportCommandLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.saveImportCommandLabelControl.Location = new System.Drawing.Point(583, 38);
		this.saveImportCommandLabelControl.Name = "saveImportCommandLabelControl";
		this.saveImportCommandLabelControl.Size = new System.Drawing.Size(105, 13);
		this.saveImportCommandLabelControl.StyleController = this.layout;
		this.saveImportCommandLabelControl.TabIndex = 12;
		this.saveImportCommandLabelControl.Text = "<href>Save import command</href>";
		this.saveImportCommandLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(saveImportCommandLabelControl_HyperlinkClick);
		this.updatedObjectCounterlabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.updatedObjectCounterlabel.Location = new System.Drawing.Point(0, 36);
		this.updatedObjectCounterlabel.Name = "updatedObjectCounterlabel";
		this.updatedObjectCounterlabel.Size = new System.Drawing.Size(581, 13);
		this.updatedObjectCounterlabel.StyleController = this.layout;
		this.updatedObjectCounterlabel.TabIndex = 6;
		this.updatedObjectCounterlabel.Text = "Objects that will be updated in repository: ";
		this.newObjectCounterlabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.newObjectCounterlabel.Location = new System.Drawing.Point(0, 23);
		this.newObjectCounterlabel.Name = "newObjectCounterlabel";
		this.newObjectCounterlabel.Size = new System.Drawing.Size(690, 13);
		this.newObjectCounterlabel.StyleController = this.layout;
		this.newObjectCounterlabel.TabIndex = 5;
		this.newObjectCounterlabel.Text = "New objects that will be ignored: ";
		this.selectedObjectCounterlabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.selectedObjectCounterlabel.Location = new System.Drawing.Point(0, 10);
		this.selectedObjectCounterlabel.Name = "selectedObjectCounterlabel";
		this.selectedObjectCounterlabel.Size = new System.Drawing.Size(690, 13);
		this.selectedObjectCounterlabel.StyleController = this.layout;
		this.selectedObjectCounterlabel.TabIndex = 4;
		this.selectedObjectCounterlabel.Text = "Objects that will be added to repository:";
		this.layoutControlGroup7.CustomizationFormText = "layoutControlGroup2";
		this.layoutControlGroup7.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup7.GroupBordersVisible = false;
		this.layoutControlGroup7.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.layoutControlItem20, this.layoutControlItem23, this.layoutControlItem24, this.saveImportCommandLayoutControlItem });
		this.layoutControlGroup7.Name = "Root";
		this.layoutControlGroup7.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 10, 10);
		this.layoutControlGroup7.Size = new System.Drawing.Size(690, 68);
		this.layoutControlGroup7.TextVisible = false;
		this.layoutControlItem20.Control = this.selectedObjectCounterlabel;
		this.layoutControlItem20.CustomizationFormText = "layoutControlItem6";
		this.layoutControlItem20.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem20.Name = "layoutControlItem6";
		this.layoutControlItem20.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem20.Size = new System.Drawing.Size(690, 13);
		this.layoutControlItem20.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem20.TextVisible = false;
		this.layoutControlItem23.Control = this.newObjectCounterlabel;
		this.layoutControlItem23.Location = new System.Drawing.Point(0, 13);
		this.layoutControlItem23.Name = "layoutControlItem23";
		this.layoutControlItem23.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem23.Size = new System.Drawing.Size(690, 13);
		this.layoutControlItem23.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem23.TextVisible = false;
		this.layoutControlItem24.Control = this.updatedObjectCounterlabel;
		this.layoutControlItem24.Location = new System.Drawing.Point(0, 26);
		this.layoutControlItem24.Name = "layoutControlItem24";
		this.layoutControlItem24.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem24.Size = new System.Drawing.Size(581, 22);
		this.layoutControlItem24.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem24.TextVisible = false;
		this.saveImportCommandLayoutControlItem.Control = this.saveImportCommandLabelControl;
		this.saveImportCommandLayoutControlItem.Location = new System.Drawing.Point(581, 26);
		this.saveImportCommandLayoutControlItem.Name = "saveImportCommandLayoutControlItem";
		this.saveImportCommandLayoutControlItem.Size = new System.Drawing.Size(109, 22);
		this.saveImportCommandLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveImportCommandLayoutControlItem.TextVisible = false;
		this.objectsStatusesLayoutControl.AllowCustomization = false;
		this.objectsStatusesLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.objectsStatusesLayoutControl.Controls.Add(this.selectObjectsLabel);
		this.objectsStatusesLayoutControl.Controls.Add(this.labelControl2);
		this.objectsStatusesLayoutControl.Controls.Add(this.checkNoneHyperLinkEdit);
		this.objectsStatusesLayoutControl.Controls.Add(this.checkAllHyperLinkEdit);
		this.objectsStatusesLayoutControl.Controls.Add(this.synchStateCounterInfoUserControl);
		this.objectsStatusesLayoutControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.objectsStatusesLayoutControl.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.layoutControlGroup5, this.statusLayoutControlGroup, this.layoutControlItem6 });
		this.objectsStatusesLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.objectsStatusesLayoutControl.Name = "objectsStatusesLayoutControl";
		this.objectsStatusesLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(761, 281, 387, 448);
		this.objectsStatusesLayoutControl.Root = this.layoutControlGroup6;
		this.objectsStatusesLayoutControl.Size = new System.Drawing.Size(690, 142);
		this.objectsStatusesLayoutControl.TabIndex = 4;
		this.objectsStatusesLayoutControl.Text = "layoutControl1";
		this.selectObjectsLabel.Location = new System.Drawing.Point(2, 97);
		this.selectObjectsLabel.Name = "selectObjectsLabel";
		this.selectObjectsLabel.Size = new System.Drawing.Size(598, 19);
		this.selectObjectsLabel.StyleController = this.objectsStatusesLayoutControl;
		this.selectObjectsLabel.TabIndex = 10;
		this.selectObjectsLabel.Text = "Select objects you would like to import from database. Uncheck objects you would like to ignore in your documentation.";
		this.labelControl2.Location = new System.Drawing.Point(2, 15);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(63, 13);
		this.labelControl2.StyleController = this.objectsStatusesLayoutControl;
		this.labelControl2.TabIndex = 5;
		this.labelControl2.Text = "labelControl2";
		this.checkNoneHyperLinkEdit.EditValue = "None";
		this.checkNoneHyperLinkEdit.Location = new System.Drawing.Point(27, 120);
		this.checkNoneHyperLinkEdit.MaximumSize = new System.Drawing.Size(40, 0);
		this.checkNoneHyperLinkEdit.MinimumSize = new System.Drawing.Size(40, 0);
		this.checkNoneHyperLinkEdit.Name = "checkNoneHyperLinkEdit";
		this.checkNoneHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.checkNoneHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.checkNoneHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.checkNoneHyperLinkEdit.Size = new System.Drawing.Size(40, 18);
		this.checkNoneHyperLinkEdit.StyleController = this.objectsStatusesLayoutControl;
		this.checkNoneHyperLinkEdit.TabIndex = 7;
		this.checkAllHyperLinkEdit.EditValue = "All";
		this.checkAllHyperLinkEdit.Location = new System.Drawing.Point(2, 120);
		this.checkAllHyperLinkEdit.MaximumSize = new System.Drawing.Size(25, 0);
		this.checkAllHyperLinkEdit.MinimumSize = new System.Drawing.Size(25, 0);
		this.checkAllHyperLinkEdit.Name = "checkAllHyperLinkEdit";
		this.checkAllHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.checkAllHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.checkAllHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.checkAllHyperLinkEdit.Size = new System.Drawing.Size(25, 18);
		this.checkAllHyperLinkEdit.StyleController = this.objectsStatusesLayoutControl;
		this.checkAllHyperLinkEdit.TabIndex = 6;
		this.synchStateCounterInfoUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.synchStateCounterInfoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.synchStateCounterInfoUserControl.Description = "";
		this.synchStateCounterInfoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.synchStateCounterInfoUserControl.Image = (System.Drawing.Image)resources.GetObject("synchStateCounterInfoUserControl.Image");
		this.synchStateCounterInfoUserControl.Location = new System.Drawing.Point(2, 2);
		this.synchStateCounterInfoUserControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.synchStateCounterInfoUserControl.Name = "synchStateCounterInfoUserControl";
		this.synchStateCounterInfoUserControl.Size = new System.Drawing.Size(686, 91);
		this.synchStateCounterInfoUserControl.TabIndex = 9;
		this.layoutControlGroup5.CustomizationFormText = "layoutControlGroup5";
		this.layoutControlGroup5.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup5.Name = "layoutControlGroup5";
		this.layoutControlGroup5.Size = new System.Drawing.Size(662, 43);
		this.statusLayoutControlGroup.CustomizationFormText = "Synchronization state";
		this.statusLayoutControlGroup.Location = new System.Drawing.Point(0, 0);
		this.statusLayoutControlGroup.Name = "statusLayoutControlGroup";
		this.statusLayoutControlGroup.Size = new System.Drawing.Size(703, 56);
		this.statusLayoutControlGroup.Text = "Synchronization state";
		this.layoutControlItem6.Control = this.labelControl2;
		this.layoutControlItem6.CustomizationFormText = "layoutControlItem2";
		this.layoutControlItem6.Location = new System.Drawing.Point(0, 13);
		this.layoutControlItem6.Name = "layoutControlItem2";
		this.layoutControlItem6.Size = new System.Drawing.Size(703, 43);
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.layoutControlGroup6.CustomizationFormText = "Root";
		this.layoutControlGroup6.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup6.GroupBordersVisible = false;
		this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.checkAllLayoutControlItem, this.checkNoneLayoutControlItem, this.checkEmptySpaceItem, this.layoutControlItem19, this.selectObjectsLayoutControlItem });
		this.layoutControlGroup6.Name = "Root";
		this.layoutControlGroup6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup6.Size = new System.Drawing.Size(690, 142);
		this.layoutControlGroup6.TextVisible = false;
		this.checkAllLayoutControlItem.Control = this.checkAllHyperLinkEdit;
		this.checkAllLayoutControlItem.CustomizationFormText = "checkAllLayoutControlItem";
		this.checkAllLayoutControlItem.Location = new System.Drawing.Point(0, 118);
		this.checkAllLayoutControlItem.MaxSize = new System.Drawing.Size(25, 24);
		this.checkAllLayoutControlItem.MinSize = new System.Drawing.Size(25, 24);
		this.checkAllLayoutControlItem.Name = "checkAllLayoutControlItem";
		this.checkAllLayoutControlItem.Size = new System.Drawing.Size(25, 24);
		this.checkAllLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.checkAllLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.checkAllLayoutControlItem.TextVisible = false;
		this.checkNoneLayoutControlItem.Control = this.checkNoneHyperLinkEdit;
		this.checkNoneLayoutControlItem.CustomizationFormText = "checkNoneLayoutControlItem";
		this.checkNoneLayoutControlItem.Location = new System.Drawing.Point(25, 118);
		this.checkNoneLayoutControlItem.MaxSize = new System.Drawing.Size(45, 24);
		this.checkNoneLayoutControlItem.MinSize = new System.Drawing.Size(45, 24);
		this.checkNoneLayoutControlItem.Name = "checkNoneLayoutControlItem";
		this.checkNoneLayoutControlItem.Size = new System.Drawing.Size(45, 24);
		this.checkNoneLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.checkNoneLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.checkNoneLayoutControlItem.TextVisible = false;
		this.checkEmptySpaceItem.AllowHotTrack = false;
		this.checkEmptySpaceItem.CustomizationFormText = "checkEmptySpaceItem";
		this.checkEmptySpaceItem.Location = new System.Drawing.Point(70, 118);
		this.checkEmptySpaceItem.MinSize = new System.Drawing.Size(104, 24);
		this.checkEmptySpaceItem.Name = "checkEmptySpaceItem";
		this.checkEmptySpaceItem.Size = new System.Drawing.Size(620, 24);
		this.checkEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.checkEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem19.Control = this.synchStateCounterInfoUserControl;
		this.layoutControlItem19.CustomizationFormText = " ";
		this.layoutControlItem19.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem19.MinSize = new System.Drawing.Size(104, 24);
		this.layoutControlItem19.Name = "layoutControlItem3";
		this.layoutControlItem19.Size = new System.Drawing.Size(690, 95);
		this.layoutControlItem19.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem19.Text = " ";
		this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem19.TextVisible = false;
		this.selectObjectsLayoutControlItem.Control = this.selectObjectsLabel;
		this.selectObjectsLayoutControlItem.CustomizationFormText = "selectObjectsLayoutControlItem";
		this.selectObjectsLayoutControlItem.Location = new System.Drawing.Point(0, 95);
		this.selectObjectsLayoutControlItem.MaxSize = new System.Drawing.Size(602, 23);
		this.selectObjectsLayoutControlItem.MinSize = new System.Drawing.Size(602, 23);
		this.selectObjectsLayoutControlItem.Name = "selectObjectsLayoutControlItem";
		this.selectObjectsLayoutControlItem.Size = new System.Drawing.Size(690, 23);
		this.selectObjectsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.selectObjectsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.selectObjectsLayoutControlItem.TextVisible = false;
		this.filterWizardPage.Controls.Add(this.generatingDocLayoutControl);
		this.filterWizardPage.DescriptionText = "Choose which objects are read from database";
		this.filterWizardPage.Name = "filterWizardPage";
		this.filterWizardPage.Size = new System.Drawing.Size(690, 512);
		this.filterWizardPage.Text = "Advanced settings";
		this.generatingDocLayoutControl.AllowCustomization = false;
		this.generatingDocLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.generatingDocLayoutControl.Controls.Add(this.importFilterControl);
		this.generatingDocLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.generatingDocLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.generatingDocLayoutControl.Name = "generatingDocLayoutControl";
		this.generatingDocLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(934, 191, 250, 350);
		this.generatingDocLayoutControl.OptionsFocus.EnableAutoTabOrder = false;
		this.generatingDocLayoutControl.Root = this.layoutControlGroup4;
		this.generatingDocLayoutControl.Size = new System.Drawing.Size(690, 512);
		this.generatingDocLayoutControl.TabIndex = 15;
		this.generatingDocLayoutControl.Text = "layoutControl1";
		this.importFilterControl.BackColor = System.Drawing.Color.Transparent;
		this.importFilterControl.DatabaseType = null;
		this.importFilterControl.Enabled = false;
		this.importFilterControl.FullReimportVisible = false;
		this.importFilterControl.Location = new System.Drawing.Point(11, 11);
		this.importFilterControl.Name = "importFilterControl";
		this.importFilterControl.SaveChangesVisible = false;
		this.importFilterControl.Size = new System.Drawing.Size(668, 490);
		this.importFilterControl.TabIndex = 2;
		this.layoutControlGroup4.CustomizationFormText = "Root";
		this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup4.GroupBordersVisible = false;
		this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.filterLayoutControlItem });
		this.layoutControlGroup4.Name = "Root";
		this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(11, 11, 11, 11);
		this.layoutControlGroup4.Size = new System.Drawing.Size(690, 512);
		this.layoutControlGroup4.TextVisible = false;
		this.filterLayoutControlItem.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.filterLayoutControlItem.AppearanceItemCaption.Options.UseFont = true;
		this.filterLayoutControlItem.Control = this.importFilterControl;
		this.filterLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.filterLayoutControlItem.Name = "filterLayoutControlItem";
		this.filterLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.filterLayoutControlItem.Size = new System.Drawing.Size(668, 490);
		this.filterLayoutControlItem.TextLocation = DevExpress.Utils.Locations.Top;
		this.filterLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.filterLayoutControlItem.TextVisible = false;
		this.completionWizardPage.Controls.Add(this.saveUpdateCommandLabelControl);
		this.completionWizardPage.Controls.Add(this.synchronizationLayoutControl);
		this.completionWizardPage.FinishText = "";
		this.completionWizardPage.Name = "completionWizardPage";
		this.completionWizardPage.ProceedText = "";
		this.completionWizardPage.Size = new System.Drawing.Size(689, 523);
		this.completionWizardPage.Text = "Importing";
		this.saveUpdateCommandLabelControl.AllowHtmlString = true;
		this.saveUpdateCommandLabelControl.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.saveUpdateCommandLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.saveUpdateCommandLabelControl.Location = new System.Drawing.Point(579, 496);
		this.saveUpdateCommandLabelControl.Name = "saveUpdateCommandLabelControl";
		this.saveUpdateCommandLabelControl.Size = new System.Drawing.Size(110, 13);
		this.saveUpdateCommandLabelControl.StyleController = this.layout;
		this.saveUpdateCommandLabelControl.TabIndex = 18;
		this.saveUpdateCommandLabelControl.Text = "<href>Save update command</href>";
		this.saveUpdateCommandLabelControl.Visible = false;
		this.saveUpdateCommandLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(saveImportCommandLabelControl_HyperlinkClick);
		this.synchronizationLayoutControl.AllowCustomization = false;
		this.synchronizationLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.synchronizationLayoutControl.Controls.Add(this.synchronizeProgressBar);
		this.synchronizationLayoutControl.Controls.Add(this.objectNameLabel);
		this.synchronizationLayoutControl.Controls.Add(this.synchronizeObjectLabel);
		this.synchronizationLayoutControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.synchronizationLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.synchronizationLayoutControl.Name = "synchronizationLayoutControl";
		this.synchronizationLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(972, 304, 250, 350);
		this.synchronizationLayoutControl.OptionsView.ShareLookAndFeelWithChildren = false;
		this.synchronizationLayoutControl.Root = this.layoutControlGroup8;
		this.synchronizationLayoutControl.Size = new System.Drawing.Size(689, 115);
		this.synchronizationLayoutControl.TabIndex = 17;
		this.synchronizationLayoutControl.Text = "layoutControl1";
		this.synchronizeProgressBar.Location = new System.Drawing.Point(2, 60);
		this.synchronizeProgressBar.Name = "synchronizeProgressBar";
		this.synchronizeProgressBar.Properties.ShowTitle = true;
		this.synchronizeProgressBar.Properties.Step = 1;
		this.synchronizeProgressBar.Size = new System.Drawing.Size(685, 18);
		this.synchronizeProgressBar.TabIndex = 16;
		this.objectNameLabel.Location = new System.Drawing.Point(2, 19);
		this.objectNameLabel.Name = "objectNameLabel";
		this.objectNameLabel.Size = new System.Drawing.Size(413, 13);
		this.objectNameLabel.TabIndex = 6;
		this.objectNameLabel.Text = "Object: name";
		this.objectNameLabel.UseMnemonic = false;
		this.synchronizeObjectLabel.AllowHtmlString = true;
		this.synchronizeObjectLabel.Location = new System.Drawing.Point(2, 2);
		this.synchronizeObjectLabel.Name = "synchronizeObjectLabel";
		this.synchronizeObjectLabel.Size = new System.Drawing.Size(413, 13);
		this.synchronizeObjectLabel.TabIndex = 5;
		this.synchronizeObjectLabel.Text = "Synchronizing object";
		this.synchronizeObjectLabel.UseMnemonic = false;
		this.layoutControlGroup8.CustomizationFormText = "Root";
		this.layoutControlGroup8.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup8.GroupBordersVisible = false;
		this.layoutControlGroup8.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.synchStateLayoutControlItem, this.synchObjectlayoutControlItem, this.progressLayoutControlItem, this.emptySpaceItem5, this.emptySpaceItem10 });
		this.layoutControlGroup8.Name = "Root";
		this.layoutControlGroup8.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup8.Size = new System.Drawing.Size(689, 115);
		this.layoutControlGroup8.TextVisible = false;
		this.synchStateLayoutControlItem.Control = this.synchronizeObjectLabel;
		this.synchStateLayoutControlItem.CustomizationFormText = "layoutControlItem5";
		this.synchStateLayoutControlItem.ImageOptions.Image = Dataedo.App.Properties.Resources.ok_16;
		this.synchStateLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.synchStateLayoutControlItem.MaxSize = new System.Drawing.Size(417, 17);
		this.synchStateLayoutControlItem.MinSize = new System.Drawing.Size(417, 17);
		this.synchStateLayoutControlItem.Name = "synchStateLayoutControlItem";
		this.synchStateLayoutControlItem.Size = new System.Drawing.Size(689, 17);
		this.synchStateLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.synchStateLayoutControlItem.Text = " ";
		this.synchStateLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.synchStateLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.synchStateLayoutControlItem.TextToControlDistance = 0;
		this.synchStateLayoutControlItem.TextVisible = false;
		this.synchObjectlayoutControlItem.Control = this.objectNameLabel;
		this.synchObjectlayoutControlItem.CustomizationFormText = "layoutControlItem6";
		this.synchObjectlayoutControlItem.Location = new System.Drawing.Point(0, 17);
		this.synchObjectlayoutControlItem.MaxSize = new System.Drawing.Size(417, 17);
		this.synchObjectlayoutControlItem.MinSize = new System.Drawing.Size(417, 17);
		this.synchObjectlayoutControlItem.Name = "layoutControlItem6";
		this.synchObjectlayoutControlItem.Size = new System.Drawing.Size(689, 17);
		this.synchObjectlayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.synchObjectlayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.synchObjectlayoutControlItem.TextVisible = false;
		this.progressLayoutControlItem.Control = this.synchronizeProgressBar;
		this.progressLayoutControlItem.CustomizationFormText = "layoutControlItem24";
		this.progressLayoutControlItem.Location = new System.Drawing.Point(0, 58);
		this.progressLayoutControlItem.MaxSize = new System.Drawing.Size(0, 22);
		this.progressLayoutControlItem.MinSize = new System.Drawing.Size(417, 22);
		this.progressLayoutControlItem.Name = "layoutControlItem24";
		this.progressLayoutControlItem.Size = new System.Drawing.Size(689, 22);
		this.progressLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.progressLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.progressLayoutControlItem.TextVisible = false;
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.CustomizationFormText = "emptySpaceItem5";
		this.emptySpaceItem5.Location = new System.Drawing.Point(0, 34);
		this.emptySpaceItem5.MaxSize = new System.Drawing.Size(417, 24);
		this.emptySpaceItem5.MinSize = new System.Drawing.Size(417, 24);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(689, 24);
		this.emptySpaceItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem10.AllowHotTrack = false;
		this.emptySpaceItem10.Location = new System.Drawing.Point(0, 80);
		this.emptySpaceItem10.MaxSize = new System.Drawing.Size(417, 35);
		this.emptySpaceItem10.MinSize = new System.Drawing.Size(417, 35);
		this.emptySpaceItem10.Name = "emptySpaceItem10";
		this.emptySpaceItem10.Size = new System.Drawing.Size(689, 35);
		this.emptySpaceItem10.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem10.TextSize = new System.Drawing.Size(0, 0);
		this.connectionWizardPage.Controls.Add(this.layoutControl1);
		this.connectionWizardPage.DescriptionText = "Please provide connection details to database you want to document";
		this.connectionWizardPage.Name = "connectionWizardPage";
		this.connectionWizardPage.Size = new System.Drawing.Size(690, 512);
		this.connectionWizardPage.Text = "Connection";
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl1.Controls.Add(this.helpIconImportDependencies);
		this.layoutControl1.Controls.Add(this.importDependenciesCheckEdit);
		this.layoutControl1.Controls.Add(this.learnMoreInfoUserControl);
		this.layoutControl1.Controls.Add(this.helpIconUserControl);
		this.layoutControl1.Controls.Add(this.saveAsComboBoxEdit);
		this.layoutControl1.Controls.Add(this.filterInfoUserControl);
		this.layoutControl1.Controls.Add(this.advancedCheckEdit);
		this.layoutControl1.Controls.Add(this.dbConnectUserControl);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1326, 272, 847, 609);
		this.layoutControl1.Root = this.layoutControlGroup11;
		this.layoutControl1.Size = new System.Drawing.Size(690, 512);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.helpIconImportDependencies.AutoPopDelay = 5000;
		this.helpIconImportDependencies.BackColor = System.Drawing.Color.Transparent;
		this.helpIconImportDependencies.KeepWhileHovered = false;
		this.helpIconImportDependencies.Location = new System.Drawing.Point(138, 456);
		this.helpIconImportDependencies.MaximumSize = new System.Drawing.Size(20, 20);
		this.helpIconImportDependencies.MaxToolTipWidth = 500;
		this.helpIconImportDependencies.MinimumSize = new System.Drawing.Size(20, 20);
		this.helpIconImportDependencies.Name = "helpIconImportDependencies";
		this.helpIconImportDependencies.Size = new System.Drawing.Size(20, 20);
		this.helpIconImportDependencies.TabIndex = 37;
		this.helpIconImportDependencies.ToolTipHeader = null;
		this.helpIconImportDependencies.ToolTipText = "Selecting this option may increase the import time as dependencies must be imported for all objects.";
		this.importDependenciesCheckEdit.Location = new System.Drawing.Point(14, 456);
		this.importDependenciesCheckEdit.Name = "importDependenciesCheckEdit";
		this.importDependenciesCheckEdit.Properties.Caption = "Import dependencies";
		this.importDependenciesCheckEdit.Size = new System.Drawing.Size(120, 20);
		this.importDependenciesCheckEdit.StyleController = this.layoutControl1;
		this.importDependenciesCheckEdit.TabIndex = 36;
		this.learnMoreInfoUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.learnMoreInfoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.learnMoreInfoUserControl.Description = "<href=>Learn more</href> about supported versions and how to connect.";
		this.learnMoreInfoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.learnMoreInfoUserControl.Image = (System.Drawing.Image)resources.GetObject("learnMoreInfoUserControl.Image");
		this.learnMoreInfoUserControl.Location = new System.Drawing.Point(12, 12);
		this.learnMoreInfoUserControl.Name = "learnMoreInfoUserControl";
		this.learnMoreInfoUserControl.Size = new System.Drawing.Size(666, 31);
		this.learnMoreInfoUserControl.TabIndex = 35;
		this.helpIconUserControl.AutoPopDelay = 5000;
		this.helpIconUserControl.BackColor = System.Drawing.Color.Transparent;
		this.helpIconUserControl.KeepWhileHovered = false;
		this.helpIconUserControl.Location = new System.Drawing.Point(127, 480);
		this.helpIconUserControl.MaximumSize = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.MaxToolTipWidth = 500;
		this.helpIconUserControl.MinimumSize = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.Name = "helpIconUserControl";
		this.helpIconUserControl.Size = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.TabIndex = 34;
		this.helpIconUserControl.ToolTipHeader = null;
		this.helpIconUserControl.ToolTipText = "Advanced settings allows you to define import filter, import extended properties and force full reimport of database schema.";
		this.saveAsComboBoxEdit.Location = new System.Drawing.Point(117, 396);
		this.saveAsComboBoxEdit.Name = "saveAsComboBoxEdit";
		this.saveAsComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.saveAsComboBoxEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.saveAsComboBoxEdit.Size = new System.Drawing.Size(226, 20);
		this.saveAsComboBoxEdit.StyleController = this.layoutControl1;
		this.saveAsComboBoxEdit.TabIndex = 33;
		this.filterInfoUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.filterInfoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.filterInfoUserControl.Description = "There is a filter specified for importing objects. To view or change it check the advanced settings box below.";
		this.filterInfoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.filterInfoUserControl.Image = Dataedo.App.Properties.Resources.about_16;
		this.filterInfoUserControl.Location = new System.Drawing.Point(12, 420);
		this.filterInfoUserControl.MaximumSize = new System.Drawing.Size(0, 32);
		this.filterInfoUserControl.MinimumSize = new System.Drawing.Size(564, 32);
		this.filterInfoUserControl.Name = "filterInfoUserControl";
		this.filterInfoUserControl.Size = new System.Drawing.Size(666, 32);
		this.filterInfoUserControl.TabIndex = 30;
		this.advancedCheckEdit.Location = new System.Drawing.Point(14, 480);
		this.advancedCheckEdit.Name = "advancedCheckEdit";
		this.advancedCheckEdit.Properties.Caption = "Advanced settings";
		this.advancedCheckEdit.Size = new System.Drawing.Size(109, 20);
		this.advancedCheckEdit.StyleController = this.layoutControl1;
		this.advancedCheckEdit.TabIndex = 28;
		this.dbConnectUserControl.BackColor = System.Drawing.Color.Transparent;
		this.dbConnectUserControl.DatabaseRow = null;
		this.dbConnectUserControl.IsDBAdded = false;
		this.dbConnectUserControl.IsExporting = false;
		this.dbConnectUserControl.Location = new System.Drawing.Point(12, 47);
		this.dbConnectUserControl.Name = "dbConnectUserControl";
		this.dbConnectUserControl.SelectedDatabaseType = null;
		this.dbConnectUserControl.Size = new System.Drawing.Size(666, 345);
		this.dbConnectUserControl.TabIndex = 31;
		this.layoutControlGroup11.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup11.GroupBordersVisible = false;
		this.layoutControlGroup11.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[10] { this.advancedSettingsEmptySpaceItem, this.advancedSettingsLayoutControlItem, this.layoutControlItem1, this.helpIconLayoutControlItem, this.learnMoreInfoLayoutControlItem, this.importDependenciesLayoutControlItem, this.filterInfoLayoutControlItem, this.saveAsLayoutControlItem, this.importDependenciesEmptySpaceItem, this.importDependenciesHelpIconLayoutControlItem });
		this.layoutControlGroup11.Name = "Root";
		this.layoutControlGroup11.Size = new System.Drawing.Size(690, 512);
		this.layoutControlGroup11.TextVisible = false;
		this.advancedSettingsEmptySpaceItem.AllowHotTrack = false;
		this.advancedSettingsEmptySpaceItem.CustomizationFormText = "emptySpaceItem13";
		this.advancedSettingsEmptySpaceItem.Location = new System.Drawing.Point(139, 468);
		this.advancedSettingsEmptySpaceItem.MinSize = new System.Drawing.Size(104, 24);
		this.advancedSettingsEmptySpaceItem.Name = "advancedSettingsEmptySpaceItem";
		this.advancedSettingsEmptySpaceItem.Size = new System.Drawing.Size(531, 24);
		this.advancedSettingsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.advancedSettingsEmptySpaceItem.Text = "emptySpaceItem13";
		this.advancedSettingsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.advancedSettingsLayoutControlItem.Control = this.advancedCheckEdit;
		this.advancedSettingsLayoutControlItem.Location = new System.Drawing.Point(0, 468);
		this.advancedSettingsLayoutControlItem.MaxSize = new System.Drawing.Size(115, 24);
		this.advancedSettingsLayoutControlItem.MinSize = new System.Drawing.Size(115, 24);
		this.advancedSettingsLayoutControlItem.Name = "advancedSettingsLayoutControlItem";
		this.advancedSettingsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 2, 2, 2);
		this.advancedSettingsLayoutControlItem.Size = new System.Drawing.Size(115, 24);
		this.advancedSettingsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.advancedSettingsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.advancedSettingsLayoutControlItem.TextVisible = false;
		this.layoutControlItem1.Control = this.dbConnectUserControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 35);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(104, 24);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(670, 349);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Bottom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.helpIconLayoutControlItem.Control = this.helpIconUserControl;
		this.helpIconLayoutControlItem.Location = new System.Drawing.Point(115, 468);
		this.helpIconLayoutControlItem.Name = "helpIconLayoutControlItem";
		this.helpIconLayoutControlItem.Size = new System.Drawing.Size(24, 24);
		this.helpIconLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.helpIconLayoutControlItem.TextVisible = false;
		this.learnMoreInfoLayoutControlItem.Control = this.learnMoreInfoUserControl;
		this.learnMoreInfoLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.learnMoreInfoLayoutControlItem.MaxSize = new System.Drawing.Size(0, 35);
		this.learnMoreInfoLayoutControlItem.MinSize = new System.Drawing.Size(104, 35);
		this.learnMoreInfoLayoutControlItem.Name = "learnMoreInfoLayoutControlItem";
		this.learnMoreInfoLayoutControlItem.Size = new System.Drawing.Size(670, 35);
		this.learnMoreInfoLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.learnMoreInfoLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.learnMoreInfoLayoutControlItem.TextVisible = false;
		this.importDependenciesLayoutControlItem.Control = this.importDependenciesCheckEdit;
		this.importDependenciesLayoutControlItem.Location = new System.Drawing.Point(0, 444);
		this.importDependenciesLayoutControlItem.MaxSize = new System.Drawing.Size(126, 24);
		this.importDependenciesLayoutControlItem.MinSize = new System.Drawing.Size(126, 24);
		this.importDependenciesLayoutControlItem.Name = "layoutControlItem3";
		this.importDependenciesLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 2, 2, 2);
		this.importDependenciesLayoutControlItem.Size = new System.Drawing.Size(126, 24);
		this.importDependenciesLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.importDependenciesLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.importDependenciesLayoutControlItem.TextVisible = false;
		this.filterInfoLayoutControlItem.Control = this.filterInfoUserControl;
		this.filterInfoLayoutControlItem.Location = new System.Drawing.Point(0, 408);
		this.filterInfoLayoutControlItem.MinSize = new System.Drawing.Size(568, 36);
		this.filterInfoLayoutControlItem.Name = "filterInfoLayoutControlItem";
		this.filterInfoLayoutControlItem.Size = new System.Drawing.Size(670, 36);
		this.filterInfoLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.filterInfoLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.filterInfoLayoutControlItem.TextVisible = false;
		this.saveAsLayoutControlItem.Control = this.saveAsComboBoxEdit;
		this.saveAsLayoutControlItem.Location = new System.Drawing.Point(0, 384);
		this.saveAsLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.saveAsLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.saveAsLayoutControlItem.Name = "saveAsLayoutControlItem";
		this.saveAsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 2, 2, 2);
		this.saveAsLayoutControlItem.Size = new System.Drawing.Size(670, 24);
		this.saveAsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.saveAsLayoutControlItem.Text = "Save as profile:";
		this.saveAsLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.saveAsLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.saveAsLayoutControlItem.TextToControlDistance = 3;
		this.saveAsLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.importDependenciesEmptySpaceItem.AllowHotTrack = false;
		this.importDependenciesEmptySpaceItem.Location = new System.Drawing.Point(150, 444);
		this.importDependenciesEmptySpaceItem.Name = "importDependenciesEmptySpaceItem";
		this.importDependenciesEmptySpaceItem.Size = new System.Drawing.Size(520, 24);
		this.importDependenciesEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.importDependenciesHelpIconLayoutControlItem.Control = this.helpIconImportDependencies;
		this.importDependenciesHelpIconLayoutControlItem.Location = new System.Drawing.Point(126, 444);
		this.importDependenciesHelpIconLayoutControlItem.Name = "importDependenciesHelpIconLayoutControlItem";
		this.importDependenciesHelpIconLayoutControlItem.Size = new System.Drawing.Size(24, 24);
		this.importDependenciesHelpIconLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.importDependenciesHelpIconLayoutControlItem.TextVisible = false;
		this.connectAndSynchWizardControl.Controls.Add(this.connectionWizardPage);
		this.connectAndSynchWizardControl.Controls.Add(this.completionWizardPage);
		this.connectAndSynchWizardControl.Controls.Add(this.filterWizardPage);
		this.connectAndSynchWizardControl.Controls.Add(this.selectObjectsSynchWizardPage);
		this.connectAndSynchWizardControl.Controls.Add(this.documentationTitleWizardPage);
		this.connectAndSynchWizardControl.Controls.Add(this.progressWizardPage);
		this.connectAndSynchWizardControl.Controls.Add(this.customFieldsWizardPage);
		this.connectAndSynchWizardControl.Controls.Add(this.dbmsPickerWizardPage);
		this.connectAndSynchWizardControl.Controls.Add(this.dbmsAdditionalPickerWizardPage);
		this.connectAndSynchWizardControl.Controls.Add(this.fileImportDestinationWizardPage);
		this.connectAndSynchWizardControl.Controls.Add(this.selectNewDatabaseTitleWizardPage);
		this.connectAndSynchWizardControl.Controls.Add(this.interfaceTablesValidationWizardPage);
		this.connectAndSynchWizardControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.connectAndSynchWizardControl.Image = Dataedo.App.Properties.Resources.empty_image;
		this.connectAndSynchWizardControl.ImageLayout = System.Windows.Forms.ImageLayout.None;
		this.connectAndSynchWizardControl.ImageWidth = 1;
		this.connectAndSynchWizardControl.Name = "connectAndSynchWizardControl";
		this.connectAndSynchWizardControl.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[12]
		{
			this.dbmsPickerWizardPage, this.dbmsAdditionalPickerWizardPage, this.fileImportDestinationWizardPage, this.selectNewDatabaseTitleWizardPage, this.connectionWizardPage, this.customFieldsWizardPage, this.filterWizardPage, this.interfaceTablesValidationWizardPage, this.progressWizardPage, this.selectObjectsSynchWizardPage,
			this.documentationTitleWizardPage, this.completionWizardPage
		});
		this.connectAndSynchWizardControl.Size = new System.Drawing.Size(722, 655);
		this.connectAndSynchWizardControl.UseCancelButton = false;
		this.connectAndSynchWizardControl.SelectedPageChanged += new DevExpress.XtraWizard.WizardPageChangedEventHandler(connectAndSynchWizardControl_SelectedPageChanged);
		this.connectAndSynchWizardControl.SelectedPageChanging += new DevExpress.XtraWizard.WizardPageChangingEventHandler(connectAndSynchWizardControl_SelectedPageChanging);
		this.connectAndSynchWizardControl.CancelClick += new System.ComponentModel.CancelEventHandler(connectAndSynchWizardControl_CancelClick);
		this.connectAndSynchWizardControl.NextClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(connectAndSynchWizardControl_NextClick);
		this.connectAndSynchWizardControl.PrevClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(connectAndSynchWizardControl_PrevClick);
		this.connectAndSynchWizardControl.CustomizeCommandButtons += new DevExpress.XtraWizard.WizardCustomizeCommandButtonsEventHandler(connectAndSynchWizardControl_CustomizeCommandButtons);
		this.dbmsPickerWizardPage.Controls.Add(this.nonCustomizableLayoutControl1);
		this.dbmsPickerWizardPage.DescriptionText = "Choose one of available native connectors below or a user ODBC connector to connect to any source.";
		this.dbmsPickerWizardPage.Name = "dbmsPickerWizardPage";
		this.dbmsPickerWizardPage.Size = new System.Drawing.Size(690, 512);
		this.dbmsPickerWizardPage.Text = "Choose your data source";
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl1.Controls.Add(this.importSelectionUserControl);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.Root = this.Root;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(690, 512);
		this.nonCustomizableLayoutControl1.TabIndex = 0;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.importSelectionUserControl.BackColor = System.Drawing.Color.Transparent;
		this.importSelectionUserControl.Location = new System.Drawing.Point(12, 12);
		this.importSelectionUserControl.Margin = new System.Windows.Forms.Padding(1);
		this.importSelectionUserControl.Name = "importSelectionUserControl";
		this.importSelectionUserControl.SelectedDatabaseType = null;
		this.importSelectionUserControl.Size = new System.Drawing.Size(666, 488);
		this.importSelectionUserControl.TabIndex = 4;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem4 });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(690, 512);
		this.Root.TextVisible = false;
		this.layoutControlItem4.Control = this.importSelectionUserControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(670, 492);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.dbmsAdditionalPickerWizardPage.Controls.Add(this.databaseSubtypesDbmsPickerUserControl);
		this.dbmsAdditionalPickerWizardPage.DescriptionText = "Choose one of available engines for the selected connector.";
		this.dbmsAdditionalPickerWizardPage.Name = "dbmsAdditionalPickerWizardPage";
		this.dbmsAdditionalPickerWizardPage.Size = new System.Drawing.Size(690, 512);
		this.dbmsAdditionalPickerWizardPage.Text = "Choose engine";
		this.databaseSubtypesDbmsPickerUserControl.Location = new System.Drawing.Point(0, 0);
		this.databaseSubtypesDbmsPickerUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.databaseSubtypesDbmsPickerUserControl.Name = "databaseSubtypesDbmsPickerUserControl";
		this.databaseSubtypesDbmsPickerUserControl.ParentDatabaseType = null;
		this.databaseSubtypesDbmsPickerUserControl.SelectedDatabaseType = null;
		this.databaseSubtypesDbmsPickerUserControl.Size = new System.Drawing.Size(690, 512);
		this.databaseSubtypesDbmsPickerUserControl.TabIndex = 0;
		this.fileImportDestinationWizardPage.Controls.Add(this.chooseDocumentationObjectTypeControl);
		this.fileImportDestinationWizardPage.DescriptionText = "Select existing database and import file into it, or add a new database.";
		this.fileImportDestinationWizardPage.Name = "fileImportDestinationWizardPage";
		this.fileImportDestinationWizardPage.Size = new System.Drawing.Size(690, 512);
		this.fileImportDestinationWizardPage.Text = "Select documentation and folder";
		this.chooseDocumentationObjectTypeControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.chooseDocumentationObjectTypeControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.chooseDocumentationObjectTypeControl.Location = new System.Drawing.Point(0, 0);
		this.chooseDocumentationObjectTypeControl.Name = "chooseDocumentationObjectTypeControl";
		this.chooseDocumentationObjectTypeControl.SelectedDatabaseId = null;
		this.chooseDocumentationObjectTypeControl.Size = new System.Drawing.Size(690, 512);
		this.chooseDocumentationObjectTypeControl.TabIndex = 0;
		this.selectNewDatabaseTitleWizardPage.Controls.Add(this.nonCustomizableLayoutControl2);
		this.selectNewDatabaseTitleWizardPage.DescriptionText = "Please enter the title of the new database.";
		this.selectNewDatabaseTitleWizardPage.Name = "selectNewDatabaseTitleWizardPage";
		this.selectNewDatabaseTitleWizardPage.Size = new System.Drawing.Size(690, 512);
		this.selectNewDatabaseTitleWizardPage.Text = "Select the title of the new database";
		this.nonCustomizableLayoutControl2.AllowCustomization = false;
		this.nonCustomizableLayoutControl2.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl2.Controls.Add(this.newDatabaseTitleTextEdit);
		this.nonCustomizableLayoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl2.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl2.Name = "nonCustomizableLayoutControl2";
		this.nonCustomizableLayoutControl2.Root = this.layoutControlGroup2;
		this.nonCustomizableLayoutControl2.Size = new System.Drawing.Size(690, 512);
		this.nonCustomizableLayoutControl2.TabIndex = 1;
		this.nonCustomizableLayoutControl2.Text = "nonCustomizableLayoutControl2";
		this.newDatabaseTitleTextEdit.Location = new System.Drawing.Point(35, 12);
		this.newDatabaseTitleTextEdit.MenuManager = this.barManager;
		this.newDatabaseTitleTextEdit.Name = "newDatabaseTitleTextEdit";
		this.newDatabaseTitleTextEdit.Size = new System.Drawing.Size(643, 20);
		this.newDatabaseTitleTextEdit.StyleController = this.nonCustomizableLayoutControl2;
		this.newDatabaseTitleTextEdit.TabIndex = 0;
		this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup2.GroupBordersVisible = false;
		this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.newDatabaseTitleLayoutControlItem, this.emptySpaceItem2 });
		this.layoutControlGroup2.Name = "layoutControlGroup2";
		this.layoutControlGroup2.Size = new System.Drawing.Size(690, 512);
		this.layoutControlGroup2.TextVisible = false;
		this.newDatabaseTitleLayoutControlItem.Control = this.newDatabaseTitleTextEdit;
		this.newDatabaseTitleLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.newDatabaseTitleLayoutControlItem.Name = "newDatabaseTitleLayoutControlItem";
		this.newDatabaseTitleLayoutControlItem.Size = new System.Drawing.Size(670, 24);
		this.newDatabaseTitleLayoutControlItem.Text = "Title";
		this.newDatabaseTitleLayoutControlItem.TextSize = new System.Drawing.Size(20, 13);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 24);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(670, 468);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.interfaceTablesValidationWizardPage.Controls.Add(this.interfaceTablesImportErrorsUserControl);
		this.interfaceTablesValidationWizardPage.DescriptionText = "Please fix data in Interface Tables and try again.";
		this.interfaceTablesValidationWizardPage.Name = "interfaceTablesValidationWizardPage";
		this.interfaceTablesValidationWizardPage.Size = new System.Drawing.Size(690, 512);
		this.interfaceTablesValidationWizardPage.Text = "Data validation errors";
		this.interfaceTablesImportErrorsUserControl.BackColor = System.Drawing.Color.Transparent;
		this.interfaceTablesImportErrorsUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.interfaceTablesImportErrorsUserControl.Location = new System.Drawing.Point(0, 0);
		this.interfaceTablesImportErrorsUserControl.Name = "interfaceTablesImportErrorsUserControl";
		this.interfaceTablesImportErrorsUserControl.Size = new System.Drawing.Size(690, 512);
		this.interfaceTablesImportErrorsUserControl.TabIndex = 0;
		this.emptySpaceItem6.AllowHotTrack = false;
		this.emptySpaceItem6.Location = new System.Drawing.Point(164, 444);
		this.emptySpaceItem6.Name = "emptySpaceItem6";
		this.emptySpaceItem6.Size = new System.Drawing.Size(506, 24);
		this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		this.readingObjectsMarqueeProgressBarControl.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.readingObjectsMarqueeProgressBarControl.Location = new System.Drawing.Point(2, 58);
		this.readingObjectsMarqueeProgressBarControl.MenuManager = this.barManager;
		this.readingObjectsMarqueeProgressBarControl.Name = "readingObjectsMarqueeProgressBarControl";
		this.readingObjectsMarqueeProgressBarControl.Size = new System.Drawing.Size(686, 18);
		this.readingObjectsMarqueeProgressBarControl.TabIndex = 15;
		this.readingObjectsMarqueeProgressBarControlLayoutControlItem.Control = this.readingObjectsMarqueeProgressBarControl;
		this.readingObjectsMarqueeProgressBarControlLayoutControlItem.Location = new System.Drawing.Point(0, 56);
		this.readingObjectsMarqueeProgressBarControlLayoutControlItem.Name = "readingObjectsMarqueeProgressBarControlLayoutControlItem";
		this.readingObjectsMarqueeProgressBarControlLayoutControlItem.Size = new System.Drawing.Size(690, 26);
		this.readingObjectsMarqueeProgressBarControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.readingObjectsMarqueeProgressBarControlLayoutControlItem.TextVisible = false;
		this.readingObjectsMarqueeProgressBarControlLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(722, 655);
		base.Controls.Add(this.connectAndSynchWizardControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("ConnectAndSynchForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "ConnectAndSynchForm";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ConnectAndSynchForm_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pictureEdit2.Properties).EndInit();
		this.customFieldsWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.customFieldsLayoutControl).EndInit();
		this.customFieldsLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.customFieldsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.selectedCustomFieldsRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldRepositoryItemLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.extendedPropertyRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.extendedPropertiesInfoLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.defineCustomFieldLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEdit1View).EndInit();
		this.progressWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.readingDatabaseLayoutControl).EndInit();
		this.readingDatabaseLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.readingObjectsProgressBar.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.readingObjectsProgressBarLayoutControlItem).EndInit();
		this.documentationTitleWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.documentationTitleLayoutControl).EndInit();
		this.documentationTitleLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.documentationTitleTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.documentationTitleLayoutControlItem).EndInit();
		this.selectObjectsSynchWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.synchronizeGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.synchronizeGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.synchronizeTableRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout).EndInit();
		this.layout.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem20).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem23).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveImportCommandLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectsStatusesLayoutControl).EndInit();
		this.objectsStatusesLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.checkNoneHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkAllHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.statusLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkAllLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkNoneLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem19).EndInit();
		((System.ComponentModel.ISupportInitialize)this.selectObjectsLayoutControlItem).EndInit();
		this.filterWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.generatingDocLayoutControl).EndInit();
		this.generatingDocLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.filterLayoutControlItem).EndInit();
		this.completionWizardPage.ResumeLayout(false);
		this.completionWizardPage.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.synchronizationLayoutControl).EndInit();
		this.synchronizationLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.synchronizeProgressBar.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.synchStateLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.synchObjectlayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.progressLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem10).EndInit();
		this.connectionWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.importDependenciesCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveAsComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.advancedCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.advancedSettingsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.advancedSettingsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.helpIconLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.learnMoreInfoLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.importDependenciesLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.filterInfoLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveAsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.importDependenciesEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.importDependenciesHelpIconLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectAndSynchWizardControl).EndInit();
		this.connectAndSynchWizardControl.ResumeLayout(false);
		this.dbmsPickerWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		this.dbmsAdditionalPickerWizardPage.ResumeLayout(false);
		this.fileImportDestinationWizardPage.ResumeLayout(false);
		this.selectNewDatabaseTitleWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl2).EndInit();
		this.nonCustomizableLayoutControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.newDatabaseTitleTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.newDatabaseTitleLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		this.interfaceTablesValidationWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.readingObjectsMarqueeProgressBarControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.readingObjectsMarqueeProgressBarControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
