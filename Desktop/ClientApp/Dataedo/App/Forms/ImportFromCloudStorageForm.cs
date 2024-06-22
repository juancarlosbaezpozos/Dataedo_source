using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Enums;
using Dataedo.App.Helpers.CloudStorage;
using Dataedo.App.Helpers.CloudStorage.AmazonS3;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Connections;
using Dataedo.App.Helpers.Controls;
using Dataedo.App.Helpers.Files;
using Dataedo.App.Import.CloudStorage;
using Dataedo.App.Import.DataLake;
using Dataedo.App.Import.DataLake.Interfaces;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.CloudStorageBrowserUserControls;
using Dataedo.App.UserControls.ConnectorsControls;
using Dataedo.App.UserControls.Interfaces;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.Utils.Behaviors;
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
using DevExpress.XtraSplashScreen;
using DevExpress.XtraWizard;

namespace Dataedo.App.Forms;

public class ImportFromCloudStorageForm : BaseXtraForm, IDataModel
{
	private readonly Form _parentForm;

	private readonly int? _databaseId;

	private readonly CustomFieldsSupport _customFieldsSupport;

	private readonly bool _importData;

	private BaseWizardPage _firstPage;

	private ICloudStorageBrowserUserControl _cloudStorageBrowserUserControl;

	private DBMSGridModel _dbmsGridModel;

	private IContainer components;

	private OpenFileDialog openFileDialog;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ToolTipController toolTipController;

	private SplashScreenManager splashScreenManager;

	private WizardControl importFromCloudStorageWizard;

	private DevExpress.XtraWizard.WizardPage selectCloudStorageProviderWizardPage;

	private DevExpress.XtraWizard.WizardPage selectFilesPage;

	private GridControl cloudStorageProviderGridControl;

	private GridView cloudStorageProviderGridView;

	private GridColumn iconGridColumn;

	private GridColumn nameGridColumn;

	private GridColumn urlGridColumn;

	private RepositoryItemButtonEdit UrlRepositoryItemButtonEdit;

	private BehaviorManager behaviorManager1;

	private GridColumn emptyGridColumn;

	private ImageListBoxControl fileFormatSelectionImageList;

	private XtraFolderBrowserDialog folderBrowserDialog;

	private DevExpress.XtraWizard.WizardPage connectWizardPage;

	private NonCustomizableLayoutControl layoutControl1;

	private InfoUserControl learnMoreInfoUserControl;

	private HelpIconUserControl helpIconUserControl;

	private ComboBoxEdit saveAsComboBoxEdit;

	private InfoUserControl filterInfoUserControl;

	private CheckEdit advancedCheckEdit;

	private DbConnectUserControlNew dbConnectUserControl;

	private LayoutControlGroup layoutControlGroup11;

	private EmptySpaceItem emptySpaceItem9;

	private LayoutControlItem advancedCheckEditLayoutControlItem;

	private EmptySpaceItem emptySpaceItem24;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem saveAsLayoutControlItem;

	private EmptySpaceItem emptySpaceItem6;

	private EmptySpaceItem emptySpaceItem7;

	private LayoutControlItem filterInfoLayoutControlItem;

	private EmptySpaceItem emptySpaceItem30;

	private LayoutControlItem helpIconLayoutControlItem;

	private LayoutControlItem learnMoreInfoLayoutControlItem;

	private DevExpress.XtraWizard.WizardPage selectFormatPage;

	private GridControl fileFormatGridControl;

	private GridView fileFormatGridView;

	private GridColumn gridColumn1;

	private GridColumn gridColumn2;

	private GridColumn gridColumn3;

	private RepositoryItemButtonEdit repositoryItemButtonEdit1;

	private GridColumn gridColumn4;

	public DataLakeTypeEnum.DataLakeType? DataLakeType { get; private set; }

	public CloudStorageTypeEnum.CloudStorageType? CloudStorageType { get; private set; }

	public IEnumerable<ObjectModel> ObjectModels { get; private set; }

	public SharedObjectTypeEnum.ObjectType ObjectType { get; private set; }

	public ICloudStorageResultObject CloudStorageResultObject { get; private set; }

	public DatabaseRow DatabaseRow { get; private set; }

	public ImportFromCloudStorageForm(Form parentForm, int? databaseId, SharedObjectTypeEnum.ObjectType objectType, CustomFieldsSupport customFieldsSupport, DataLakeTypeEnum.DataLakeType? dataLakeType, CloudStorageTypeEnum.CloudStorageType? cloudStorageType, DatabaseRow databaseRow, bool importData)
	{
		_parentForm = parentForm;
		InitializeComponent();
		WizardPageInfo.InitWizardPages(importFromCloudStorageWizard);
		_databaseId = databaseId;
		_customFieldsSupport = customFieldsSupport;
		_importData = importData;
		DataLakeType = dataLakeType;
		ObjectType = objectType;
		CloudStorageType = cloudStorageType;
		DatabaseRow = databaseRow;
		if (DataLakeType.HasValue)
		{
			WizardPageInfo.GetOrCreate(selectFormatPage).SkipPage = true;
		}
		if (CloudStorageType.HasValue)
		{
			WizardPageInfo.GetOrCreate(selectCloudStorageProviderWizardPage).SkipPage = true;
		}
		if (DatabaseRow != null)
		{
			WizardPageInfo.GetOrCreate(connectWizardPage).SkipPage = true;
		}
		InitEvents();
		if (!WizardPageInfo.GetOrCreate(selectFormatPage).SkipPage)
		{
			fileFormatGridControl.DataSource = DataLakeTypeEnum.GetDataLakeTypeObjects();
		}
		if (!WizardPageInfo.GetOrCreate(connectWizardPage).SkipPage)
		{
			cloudStorageProviderGridControl.DataSource = CloudStorageTypeEnum.GetCloudStorageTypeObjects().Select(CreateDBMSModel).ToList();
		}
		selectFilesPage.AllowFinish = false;
		dbConnectUserControl.InitializeDatabaseRow(_databaseId);
		HideAdvancedSettings();
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Escape)
		{
			Close();
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void HideAdvancedSettings()
	{
		advancedCheckEditLayoutControlItem.HideToCustomization();
		filterInfoLayoutControlItem.HideToCustomization();
		helpIconLayoutControlItem.HideToCustomization();
	}

	private void InitEvents()
	{
		base.Load += ImportFromCloudStorageForm_Load;
		base.Shown += ImportFromFileForm_Shown;
		importFromCloudStorageWizard.SelectedPageChanging += ImportFromCloudStorageWizard_SelectedPageChanging;
		importFromCloudStorageWizard.CustomizeCommandButtons += ImportFromCloudStorageWizard_CustomizeCommandButtons;
		importFromCloudStorageWizard.FinishClick += ImportFromCloudStorageWizard_FinishClick;
		importFromCloudStorageWizard.CancelClick += ImportFromCloudStorageWizard_CancelClick;
		if (!WizardPageInfo.GetOrCreate(selectCloudStorageProviderWizardPage).SkipPage)
		{
			selectCloudStorageProviderWizardPage.PageInit += SelectCloudStorageProviderWizardPage_PageInit;
			selectCloudStorageProviderWizardPage.PageCommit += SelectCloudStorageProviderWizardPage_PageCommit;
			cloudStorageProviderGridView.FocusedRowChanged += CloudStorageGridView_FocusedRowChanged;
			cloudStorageProviderGridView.RowStyle += CloudStorageGridView_RowStyle;
		}
		if (!WizardPageInfo.GetOrCreate(connectWizardPage).SkipPage)
		{
			connectWizardPage.PageInit += ConnectWizardPage_PageInit;
			connectWizardPage.PageValidating += ConnectWizardPage_PageValidating;
			connectWizardPage.PageCommit += ConnectWizardPage_PageCommit;
		}
		if (!WizardPageInfo.GetOrCreate(selectFormatPage).SkipPage)
		{
			selectFormatPage.PageCommit += SelectFormatPage_PageCommit;
			fileFormatGridView.FocusedRowChanged += FileFormatGridView_FocusedRowChanged;
			fileFormatGridView.MouseMove += FileFormatGridView_MouseMove;
			fileFormatGridView.MouseDown += FileFormatGridView_MouseDown;
			fileFormatGridView.RowStyle += FileFormatGridView_RowStyle;
		}
		selectFilesPage.PageInit += SelectFilesPage_PageInit;
		selectFilesPage.PageRollback += SelectFilesPage_PageRollback;
	}

	private void InitCloudStorageBrowser()
	{
		if (DatabaseRow == null)
		{
			return;
		}
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			object obj = DatabaseRow?.Connection;
			if (!(obj is AmazonS3Connection browserUserControl))
			{
				if (obj is AzureStorageConnection browserUserControl2)
				{
					SetBrowserUserControl<AzureStorageBrowserUserControl, AzureStorageConnection>(browserUserControl2);
				}
			}
			else
			{
				SetBrowserUserControl<AmazonS3BrowserUserControl, AmazonS3Connection>(browserUserControl);
			}
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private void SetBrowserUserControl<TBrowserUserControl, TConnection>(TConnection cloudStorageConnection) where TBrowserUserControl : Control, ICloudStorageBrowserUserControl<TConnection>, new()
	{
		TBrowserUserControl val = _cloudStorageBrowserUserControl as TBrowserUserControl;
		if (val == null)
		{
			selectFilesPage.Controls.Clear();
			_cloudStorageBrowserUserControl?.Dispose();
			_cloudStorageBrowserUserControl = null;
			val = new TBrowserUserControl();
			selectFilesPage.Controls.Add(val);
			val.Dock = DockStyle.Fill;
			val.Location = new Point(0, 0);
			val.Name = "_cloudStorageBrowserUserControl";
			val.SelectedItemsChanged += CloudBrowserUserControl_SelectedItemsChanged;
			_cloudStorageBrowserUserControl = val;
		}
		CloudStorageBrowserInfo browserInfo = new CloudStorageBrowserInfo
		{
			DataLakeType = DataLakeType
		};
		val.Init(cloudStorageConnection, DatabaseRow, browserInfo);
	}

	private void SetFinishButtonAvailability(ICloudStorageBrowserUserControl control)
	{
		if (control != null && control.IsSelectedItem)
		{
			selectFilesPage.AllowFinish = true;
		}
		else
		{
			selectFilesPage.AllowFinish = false;
		}
	}

	private void InitStartPage()
	{
		_firstPage = importFromCloudStorageWizard.Pages[0];
		for (int i = 0; i < importFromCloudStorageWizard.Pages.Count; i++)
		{
			BaseWizardPage baseWizardPage = importFromCloudStorageWizard.Pages[i];
			if (!WizardPageInfo.GetOrCreate(baseWizardPage).SkipPage)
			{
				_firstPage = baseWizardPage;
				break;
			}
		}
		if (WizardPageInfo.GetOrCreate(selectCloudStorageProviderWizardPage).SkipPage)
		{
			_dbmsGridModel = CreateDBMSModel(new CloudStorageTypeObject(CloudStorageType.Value));
		}
		importFromCloudStorageWizard.SelectedPage = _firstPage;
	}

	private bool InitConnectPage()
	{
		SharedDatabaseTypeEnum.DatabaseType? databaseType = DatabaseTypeEnum.StringToType(_dbmsGridModel.Type);
		dbConnectUserControl.SetParameters(_databaseId, selectedDatabaseType: databaseType, dbmsGridModel: _dbmsGridModel, isDbAdded: false, isCopyingConnection: false);
		dbConnectUserControl.Clear();
		return true;
	}

	private void SelectFilesPageInit()
	{
		SetFinishButtonAvailability(_cloudStorageBrowserUserControl);
		if (DataLakeType.HasValue)
		{
			if (DataLakeType == DataLakeTypeEnum.DataLakeType.DELTALAKE)
			{
				selectFilesPage.Text = "Select Delta Lake folder";
			}
			else
			{
				selectFilesPage.Text = "Select " + DataLakeTypeEnum.GetDisplayName(DataLakeType.Value) + " file(s)";
			}
		}
		InitCloudStorageBrowser();
		SetFinishButtonAvailability(_cloudStorageBrowserUserControl);
	}

	private void SelectFilesPageRollback()
	{
		_cloudStorageBrowserUserControl.ClearNodes();
	}

	private void FinishClick(CancelEventArgs e)
	{
		CloudStorageResultObject = GetResult();
		if (!string.IsNullOrEmpty(CloudStorageResultObject.WarningMessage) && GeneralMessageBoxesHandling.Show(CloudStorageResultObject.WarningMessage, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, this).DialogResult != DialogResult.Yes)
		{
			e.Cancel = true;
		}
		else if (!_importData)
		{
			base.DialogResult = DialogResult.OK;
		}
		else
		{
			if (!DataLakeType.HasValue)
			{
				return;
			}
			ImportResult importResult = FilesImporter.ImportItems(CloudStorageResultObject.GetStreamableImportItems(), DataLakeType, ObjectType, splashScreenManager, this);
			if (importResult.CancelEvent)
			{
				e.Cancel = true;
			}
			if (!importResult.Success)
			{
				return;
			}
			DataLakeImportProcessor dataLakeImportProcessor = new DataLakeImportProcessor(_databaseId.Value, ObjectType, this, splashScreenManager, _customFieldsSupport);
			if (dataLakeImportProcessor.ProcessImport(DataLakeType, importResult.ObjectModels))
			{
				ObjectModels = dataLakeImportProcessor.ObjectModels;
				base.DialogResult = DialogResult.OK;
			}
			else
			{
				base.DialogResult = DialogResult.Cancel;
			}
			foreach (ObjectModel item in importResult.ObjectModels.Concat(dataLakeImportProcessor.AllModels ?? Enumerable.Empty<ObjectModel>()))
			{
				if (item?.ImportItem != null && item.ImportItem.DeleteFileAfterImport)
				{
					item.ImportItem.DeleteTemporaryFiles();
				}
			}
		}
	}

	private ICloudStorageResultObject GetResult()
	{
		return _cloudStorageBrowserUserControl?.GetResult();
	}

	private static DBMSGridModel CreateDBMSModel(CloudStorageTypeObject cloudStorageTypeObject)
	{
		IDatabaseSupport databaseSupport = DatabaseSupportFactory.GetDatabaseSupport(cloudStorageTypeObject.DatabaseType);
		bool flag = Connectors.HasDatabaseTypeConnector(databaseSupport.SupportedDatabaseType);
		return new DBMSGridModel(databaseSupport.TypeValue, databaseSupport.GetFriendlyDisplayNameForImport(isLite: false), flag ? databaseSupport.TypeImage : ToolStripRenderer.CreateDisabledImage(databaseSupport.TypeImage), flag ? "" : ("<href=" + Links.ManageAccounts + ">(upgrade to connect)</href>"), databaseSupport.ImportFolders, isDatabase: true, isActive: true, flag);
	}

	private void ChangeNextButtonAvailabilityCloudStorage()
	{
		if (cloudStorageProviderGridView.GetFocusedRow() is DBMSGridModel dBMSGridModel)
		{
			if (!Connectors.HasCloudStorageTypeConnector(CloudStorageTypeEnum.StringToType(dBMSGridModel.Type)))
			{
				selectCloudStorageProviderWizardPage.AllowNext = false;
			}
			else
			{
				selectCloudStorageProviderWizardPage.AllowNext = true;
			}
		}
	}

	private void ChangeNextButtonAvailabilityFileFormat()
	{
		if (fileFormatGridView.GetFocusedRow() is DataLakeTypeObject dataLakeTypeObject)
		{
			if (!Connectors.HasDataLakeTypeConnector(dataLakeTypeObject.Value))
			{
				selectFormatPage.AllowNext = false;
			}
			else
			{
				selectFormatPage.AllowNext = true;
			}
		}
	}

	private void SelectCloudStorageProviderWizardPage_PageInit(object sender, EventArgs e)
	{
		ChangeNextButtonAvailabilityCloudStorage();
	}

	private void ImportFromCloudStorageWizard_CancelClick(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
	}

	private void ImportFromFileForm_Shown(object sender, EventArgs e)
	{
		if (_parentForm != null)
		{
			_parentForm.Visible = false;
		}
	}

	private void ImportFromCloudStorageWizard_FinishClick(object sender, CancelEventArgs e)
	{
		FinishClick(e);
	}

	private void SelectFormatPage_PageCommit(object sender, EventArgs e)
	{
		if (fileFormatGridView.GetFocusedRow() is DataLakeTypeObject dataLakeTypeObject)
		{
			DataLakeType = dataLakeTypeObject.Value;
			ImportFileTrackingHelper.TrackImportFileConnectorSelected(DataLakeType);
		}
	}

	private void ImportFromCloudStorageForm_Load(object sender, EventArgs e)
	{
		InitStartPage();
	}

	private void SelectCloudStorageProviderWizardPage_PageCommit(object sender, EventArgs e)
	{
		_dbmsGridModel = cloudStorageProviderGridView.GetFocusedRow() as DBMSGridModel;
		ImportFileTrackingHelper.TrackImportFileConnectorSelected(DatabaseTypeEnum.StringToType(_dbmsGridModel.Type));
	}

	private void SelectFilesPage_PageInit(object sender, EventArgs e)
	{
		SelectFilesPageInit();
	}

	private void SelectFilesPage_PageRollback(object sender, EventArgs e)
	{
		SelectFilesPageRollback();
	}

	private void ConnectWizardPage_PageInit(object sender, EventArgs e)
	{
		InitConnectPage();
	}

	private void ConnectWizardPage_PageValidating(object sender, WizardPageValidatingEventArgs e)
	{
		dbConnectUserControl.SetNewDBRowValues();
		dbConnectUserControl.SetImportCommandsTimeout();
		dbConnectUserControl.DatabaseRow?.CloseConnection();
		if (!dbConnectUserControl.TestConnection(testForGettingDatabasesList: false))
		{
			e.Valid = false;
		}
	}

	private void ConnectWizardPage_PageCommit(object sender, EventArgs e)
	{
		dbConnectUserControl.PageCommited();
		DatabaseRow = dbConnectUserControl.DatabaseRow;
	}

	private void ImportFromCloudStorageWizard_CustomizeCommandButtons(object sender, CustomizeCommandButtonsEventArgs e)
	{
		e.FinishButton.Text = "Import";
		if (e.Page == _firstPage)
		{
			e.PrevButton.Visible = false;
		}
		else
		{
			e.PrevButton.Visible = true;
		}
	}

	private void ImportFromCloudStorageWizard_SelectedPageChanging(object sender, WizardPageChangingEventArgs e)
	{
		WizardPageInfo orCreate = WizardPageInfo.GetOrCreate(e.Page);
		if (orCreate.SkipPage)
		{
			if (e.Direction == Direction.Backward && orCreate.PrevPage != null)
			{
				e.Page = orCreate.PrevPage;
			}
			else if (e.Direction == Direction.Forward && orCreate.NextPage != null)
			{
				e.Page = orCreate.NextPage;
			}
		}
	}

	private void CloudStorageGridView_RowStyle(object sender, RowStyleEventArgs e)
	{
		if (!Connectors.HasAllConnectors() && e.RowHandle >= 0 && sender is GridView gridView && gridView.GetRow(e.RowHandle) is CloudStorageTypeObject cloudStorageTypeObject && !Connectors.HasCloudStorageTypeConnector(cloudStorageTypeObject.Value))
		{
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridDisabledGridRowForeColor;
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
		}
	}

	private void FileFormatGridView_RowStyle(object sender, RowStyleEventArgs e)
	{
		if (!Connectors.HasAllConnectors() && e.RowHandle >= 0 && sender is GridView gridView && gridView.GetRow(e.RowHandle) is DataLakeTypeObject dataLakeTypeObject && !Connectors.HasDataLakeTypeConnector(dataLakeTypeObject.Value))
		{
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridDisabledGridRowForeColor;
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
		}
	}

	private void FileFormatGridView_MouseDown(object sender, MouseEventArgs e)
	{
		if (!Connectors.HasAllConnectors() && sender is GridView gridView)
		{
			GridHitInfo gridHitInfo = gridView.CalcHitInfo(e.Location);
			if (gridHitInfo != null && gridHitInfo.RowHandle >= 0 && gridView.GetRow(gridHitInfo.RowHandle) is DataLakeTypeObject dataLakeTypeObject && gridHitInfo.Column.FieldName == "URL" && !Connectors.HasDataLakeTypeConnector(dataLakeTypeObject.Value))
			{
				Links.OpenLink(Links.ManageAccounts);
			}
		}
	}

	private void FileFormatGridView_MouseMove(object sender, MouseEventArgs e)
	{
		if (sender is GridView gridView && !Connectors.HasAllConnectors())
		{
			if (gridView.CalcHitInfo(e.Location)?.Column == gridView.Columns["URL"])
			{
				gridView.GridControl.Cursor = Cursors.Hand;
			}
			else
			{
				gridView.GridControl.Cursor = Cursors.Default;
			}
		}
	}

	private void FileFormatGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		if (fileFormatGridView.GetFocusedRow() is DataLakeTypeObject dataLakeTypeObject)
		{
			DataLakeType = dataLakeTypeObject.Value;
		}
		ChangeNextButtonAvailabilityFileFormat();
	}

	private void CloudStorageGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		ChangeNextButtonAvailabilityCloudStorage();
	}

	private void CloudBrowserUserControl_SelectedItemsChanged(object sender, EventArgs e)
	{
		SetFinishButtonAvailability(sender as ICloudStorageBrowserUserControl);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.ImportFromCloudStorageForm));
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
		this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true);
		this.importFromCloudStorageWizard = new DevExpress.XtraWizard.WizardControl();
		this.selectCloudStorageProviderWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.cloudStorageProviderGridControl = new DevExpress.XtraGrid.GridControl();
		this.cloudStorageProviderGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.iconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.urlGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.UrlRepositoryItemButtonEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.emptyGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.selectFilesPage = new DevExpress.XtraWizard.WizardPage();
		this.connectWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.learnMoreInfoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.helpIconUserControl = new Dataedo.App.UserControls.HelpIconUserControl();
		this.saveAsComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.filterInfoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.advancedCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.dbConnectUserControl = new Dataedo.App.UserControls.ConnectorsControls.DbConnectUserControlNew();
		this.layoutControlGroup11 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem9 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.advancedCheckEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem24 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.saveAsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem7 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.filterInfoLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem30 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.helpIconLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.learnMoreInfoLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.selectFormatPage = new DevExpress.XtraWizard.WizardPage();
		this.fileFormatGridControl = new DevExpress.XtraGrid.GridControl();
		this.fileFormatGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn4 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
		this.folderBrowserDialog = new DevExpress.XtraEditors.XtraFolderBrowserDialog(this.components);
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.importFromCloudStorageWizard).BeginInit();
		this.importFromCloudStorageWizard.SuspendLayout();
		this.selectCloudStorageProviderWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cloudStorageProviderGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cloudStorageProviderGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.UrlRepositoryItemButtonEdit).BeginInit();
		this.connectWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.saveAsComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.advancedCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.advancedCheckEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveAsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.filterInfoLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem30).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.helpIconLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.learnMoreInfoLayoutControlItem).BeginInit();
		this.selectFormatPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.fileFormatGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fileFormatGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.behaviorManager1).BeginInit();
		base.SuspendLayout();
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(671, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 570);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(671, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 570);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(671, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 570);
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.openFileDialog.Multiselect = true;
		this.splashScreenManager.ClosingDelay = 500;
		this.importFromCloudStorageWizard.Controls.Add(this.selectCloudStorageProviderWizardPage);
		this.importFromCloudStorageWizard.Controls.Add(this.selectFilesPage);
		this.importFromCloudStorageWizard.Controls.Add(this.connectWizardPage);
		this.importFromCloudStorageWizard.Controls.Add(this.selectFormatPage);
		this.importFromCloudStorageWizard.Dock = System.Windows.Forms.DockStyle.Fill;
		this.importFromCloudStorageWizard.Name = "importFromCloudStorageWizard";
		this.importFromCloudStorageWizard.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[4] { this.selectCloudStorageProviderWizardPage, this.connectWizardPage, this.selectFormatPage, this.selectFilesPage });
		this.importFromCloudStorageWizard.Size = new System.Drawing.Size(671, 570);
		this.selectCloudStorageProviderWizardPage.Controls.Add(this.cloudStorageProviderGridControl);
		this.selectCloudStorageProviderWizardPage.DescriptionText = "";
		this.selectCloudStorageProviderWizardPage.Name = "selectCloudStorageProviderWizardPage";
		this.selectCloudStorageProviderWizardPage.Size = new System.Drawing.Size(639, 427);
		this.selectCloudStorageProviderWizardPage.Text = "Select provider";
		this.cloudStorageProviderGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.cloudStorageProviderGridControl.Location = new System.Drawing.Point(0, 0);
		this.cloudStorageProviderGridControl.MainView = this.cloudStorageProviderGridView;
		this.cloudStorageProviderGridControl.MenuManager = this.barManager;
		this.cloudStorageProviderGridControl.Name = "cloudStorageProviderGridControl";
		this.cloudStorageProviderGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.UrlRepositoryItemButtonEdit });
		this.cloudStorageProviderGridControl.Size = new System.Drawing.Size(639, 427);
		this.cloudStorageProviderGridControl.TabIndex = 1;
		this.cloudStorageProviderGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.cloudStorageProviderGridView });
		this.cloudStorageProviderGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.iconGridColumn, this.nameGridColumn, this.urlGridColumn, this.emptyGridColumn });
		this.cloudStorageProviderGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.cloudStorageProviderGridView.GridControl = this.cloudStorageProviderGridControl;
		this.cloudStorageProviderGridView.Name = "cloudStorageProviderGridView";
		this.cloudStorageProviderGridView.OptionsBehavior.AutoPopulateColumns = false;
		this.cloudStorageProviderGridView.OptionsBehavior.Editable = false;
		this.cloudStorageProviderGridView.OptionsBehavior.ReadOnly = true;
		this.cloudStorageProviderGridView.OptionsCustomization.AllowFilter = false;
		this.cloudStorageProviderGridView.OptionsDetail.EnableMasterViewMode = false;
		this.cloudStorageProviderGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.cloudStorageProviderGridView.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Fast;
		this.cloudStorageProviderGridView.OptionsView.ShowColumnHeaders = false;
		this.cloudStorageProviderGridView.OptionsView.ShowGroupPanel = false;
		this.cloudStorageProviderGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.cloudStorageProviderGridView.OptionsView.ShowIndicator = false;
		this.cloudStorageProviderGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.iconGridColumn.Caption = " ";
		this.iconGridColumn.FieldName = "Image";
		this.iconGridColumn.MaxWidth = 30;
		this.iconGridColumn.Name = "iconGridColumn";
		this.iconGridColumn.OptionsColumn.ShowCaption = false;
		this.iconGridColumn.Visible = true;
		this.iconGridColumn.VisibleIndex = 0;
		this.iconGridColumn.Width = 20;
		this.nameGridColumn.Caption = " ";
		this.nameGridColumn.FieldName = "Name";
		this.nameGridColumn.MinWidth = 100;
		this.nameGridColumn.Name = "nameGridColumn";
		this.nameGridColumn.OptionsColumn.ShowCaption = false;
		this.nameGridColumn.Visible = true;
		this.nameGridColumn.VisibleIndex = 1;
		this.nameGridColumn.Width = 100;
		this.urlGridColumn.Caption = " ";
		this.urlGridColumn.ColumnEdit = this.UrlRepositoryItemButtonEdit;
		this.urlGridColumn.FieldName = "URL";
		this.urlGridColumn.MaxWidth = 120;
		this.urlGridColumn.MinWidth = 120;
		this.urlGridColumn.Name = "urlGridColumn";
		this.urlGridColumn.OptionsColumn.ShowCaption = false;
		this.urlGridColumn.Visible = true;
		this.urlGridColumn.VisibleIndex = 2;
		this.urlGridColumn.Width = 120;
		this.UrlRepositoryItemButtonEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.UrlRepositoryItemButtonEdit.AutoHeight = false;
		this.UrlRepositoryItemButtonEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.UrlRepositoryItemButtonEdit.Name = "UrlRepositoryItemButtonEdit";
		this.emptyGridColumn.Caption = "gridColumn1";
		this.emptyGridColumn.Name = "emptyGridColumn";
		this.emptyGridColumn.Visible = true;
		this.emptyGridColumn.VisibleIndex = 3;
		this.emptyGridColumn.Width = 429;
		this.selectFilesPage.DescriptionText = "";
		this.selectFilesPage.Name = "selectFilesPage";
		this.selectFilesPage.Size = new System.Drawing.Size(639, 427);
		this.selectFilesPage.Text = "Select files";
		this.connectWizardPage.Controls.Add(this.layoutControl1);
		this.connectWizardPage.DescriptionText = "";
		this.connectWizardPage.Name = "connectWizardPage";
		this.connectWizardPage.Size = new System.Drawing.Size(639, 427);
		this.connectWizardPage.Text = "Connection Page";
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.BackColor = System.Drawing.Color.Transparent;
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
		this.layoutControl1.Size = new System.Drawing.Size(639, 427);
		this.layoutControl1.TabIndex = 1;
		this.layoutControl1.Text = "layoutControl1";
		this.learnMoreInfoUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.learnMoreInfoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.learnMoreInfoUserControl.Description = "<href=>Learn more</href> about supported versions and how to connect.";
		this.learnMoreInfoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.learnMoreInfoUserControl.Image = (System.Drawing.Image)resources.GetObject("learnMoreInfoUserControl.Image");
		this.learnMoreInfoUserControl.Location = new System.Drawing.Point(12, 12);
		this.learnMoreInfoUserControl.Name = "learnMoreInfoUserControl";
		this.learnMoreInfoUserControl.Size = new System.Drawing.Size(615, 31);
		this.learnMoreInfoUserControl.TabIndex = 35;
		this.helpIconUserControl.AutoPopDelay = 5000;
		this.helpIconUserControl.BackColor = System.Drawing.Color.Transparent;
		this.helpIconUserControl.KeepWhileHovered = false;
		this.helpIconUserControl.Location = new System.Drawing.Point(141, 395);
		this.helpIconUserControl.MaximumSize = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.MaxToolTipWidth = 500;
		this.helpIconUserControl.MinimumSize = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.Name = "helpIconUserControl";
		this.helpIconUserControl.Size = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.TabIndex = 34;
		this.helpIconUserControl.ToolTipHeader = null;
		this.helpIconUserControl.ToolTipText = "Advanced settings allows you to define import filter, import extended properties and force full reimport of database schema.";
		this.saveAsComboBoxEdit.Location = new System.Drawing.Point(129, 335);
		this.saveAsComboBoxEdit.Name = "saveAsComboBoxEdit";
		this.saveAsComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.saveAsComboBoxEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.saveAsComboBoxEdit.Size = new System.Drawing.Size(228, 20);
		this.saveAsComboBoxEdit.StyleController = this.layoutControl1;
		this.saveAsComboBoxEdit.TabIndex = 33;
		this.filterInfoUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.filterInfoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.filterInfoUserControl.Description = "There is a filter specified for importing objects. To view or change it check the advanced settings box below.";
		this.filterInfoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.filterInfoUserControl.Image = Dataedo.App.Properties.Resources.about_16;
		this.filterInfoUserControl.Location = new System.Drawing.Point(26, 359);
		this.filterInfoUserControl.MaximumSize = new System.Drawing.Size(0, 32);
		this.filterInfoUserControl.MinimumSize = new System.Drawing.Size(564, 32);
		this.filterInfoUserControl.Name = "filterInfoUserControl";
		this.filterInfoUserControl.Size = new System.Drawing.Size(601, 32);
		this.filterInfoUserControl.TabIndex = 30;
		this.advancedCheckEdit.Location = new System.Drawing.Point(26, 395);
		this.advancedCheckEdit.Name = "advancedCheckEdit";
		this.advancedCheckEdit.Properties.Caption = "Advanced settings";
		this.advancedCheckEdit.Size = new System.Drawing.Size(111, 20);
		this.advancedCheckEdit.StyleController = this.layoutControl1;
		this.advancedCheckEdit.TabIndex = 28;
		this.dbConnectUserControl.BackColor = System.Drawing.Color.Transparent;
		this.dbConnectUserControl.DatabaseRow = null;
		this.dbConnectUserControl.IsDBAdded = false;
		this.dbConnectUserControl.IsExporting = false;
		this.dbConnectUserControl.Location = new System.Drawing.Point(12, 47);
		this.dbConnectUserControl.Name = "dbConnectUserControl";
		this.dbConnectUserControl.SelectedDatabaseType = null;
		this.dbConnectUserControl.Size = new System.Drawing.Size(615, 284);
		this.dbConnectUserControl.TabIndex = 31;
		this.layoutControlGroup11.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup11.GroupBordersVisible = false;
		this.layoutControlGroup11.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[11]
		{
			this.emptySpaceItem9, this.advancedCheckEditLayoutControlItem, this.emptySpaceItem24, this.layoutControlItem2, this.saveAsLayoutControlItem, this.emptySpaceItem6, this.emptySpaceItem7, this.filterInfoLayoutControlItem, this.emptySpaceItem30, this.helpIconLayoutControlItem,
			this.learnMoreInfoLayoutControlItem
		});
		this.layoutControlGroup11.Name = "Root";
		this.layoutControlGroup11.Size = new System.Drawing.Size(639, 427);
		this.layoutControlGroup11.TextVisible = false;
		this.emptySpaceItem9.AllowHotTrack = false;
		this.emptySpaceItem9.CustomizationFormText = "emptySpaceItem13";
		this.emptySpaceItem9.Location = new System.Drawing.Point(153, 383);
		this.emptySpaceItem9.MinSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem9.Name = "emptySpaceItem9";
		this.emptySpaceItem9.Size = new System.Drawing.Size(466, 24);
		this.emptySpaceItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem9.Text = "emptySpaceItem13";
		this.emptySpaceItem9.TextSize = new System.Drawing.Size(0, 0);
		this.advancedCheckEditLayoutControlItem.Control = this.advancedCheckEdit;
		this.advancedCheckEditLayoutControlItem.Location = new System.Drawing.Point(14, 383);
		this.advancedCheckEditLayoutControlItem.MaxSize = new System.Drawing.Size(115, 24);
		this.advancedCheckEditLayoutControlItem.MinSize = new System.Drawing.Size(115, 24);
		this.advancedCheckEditLayoutControlItem.Name = "advancedCheckEditLayoutControlItem";
		this.advancedCheckEditLayoutControlItem.Size = new System.Drawing.Size(115, 24);
		this.advancedCheckEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.advancedCheckEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.advancedCheckEditLayoutControlItem.TextVisible = false;
		this.emptySpaceItem24.AllowHotTrack = false;
		this.emptySpaceItem24.CustomizationFormText = "emptySpaceItem11";
		this.emptySpaceItem24.Location = new System.Drawing.Point(0, 383);
		this.emptySpaceItem24.MaxSize = new System.Drawing.Size(14, 24);
		this.emptySpaceItem24.MinSize = new System.Drawing.Size(14, 24);
		this.emptySpaceItem24.Name = "emptySpaceItem24";
		this.emptySpaceItem24.Size = new System.Drawing.Size(14, 24);
		this.emptySpaceItem24.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem24.Text = "emptySpaceItem11";
		this.emptySpaceItem24.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.Control = this.dbConnectUserControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 35);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(104, 24);
		this.layoutControlItem2.Name = "layoutControlItem1";
		this.layoutControlItem2.Size = new System.Drawing.Size(619, 288);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextLocation = DevExpress.Utils.Locations.Bottom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.saveAsLayoutControlItem.Control = this.saveAsComboBoxEdit;
		this.saveAsLayoutControlItem.Location = new System.Drawing.Point(14, 323);
		this.saveAsLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.saveAsLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.saveAsLayoutControlItem.Name = "saveAsLayoutControlItem";
		this.saveAsLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.saveAsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.saveAsLayoutControlItem.Text = "Save as profile:";
		this.saveAsLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.saveAsLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.saveAsLayoutControlItem.TextToControlDistance = 3;
		this.saveAsLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.emptySpaceItem6.AllowHotTrack = false;
		this.emptySpaceItem6.Location = new System.Drawing.Point(0, 323);
		this.emptySpaceItem6.MaxSize = new System.Drawing.Size(14, 24);
		this.emptySpaceItem6.MinSize = new System.Drawing.Size(14, 24);
		this.emptySpaceItem6.Name = "emptySpaceItem6";
		this.emptySpaceItem6.Size = new System.Drawing.Size(14, 24);
		this.emptySpaceItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem7.AllowHotTrack = false;
		this.emptySpaceItem7.Location = new System.Drawing.Point(349, 323);
		this.emptySpaceItem7.Name = "emptySpaceItem7";
		this.emptySpaceItem7.Size = new System.Drawing.Size(270, 24);
		this.emptySpaceItem7.TextSize = new System.Drawing.Size(0, 0);
		this.filterInfoLayoutControlItem.Control = this.filterInfoUserControl;
		this.filterInfoLayoutControlItem.Location = new System.Drawing.Point(14, 347);
		this.filterInfoLayoutControlItem.MinSize = new System.Drawing.Size(568, 36);
		this.filterInfoLayoutControlItem.Name = "filterInfoLayoutControlItem";
		this.filterInfoLayoutControlItem.Size = new System.Drawing.Size(605, 36);
		this.filterInfoLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.filterInfoLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.filterInfoLayoutControlItem.TextVisible = false;
		this.emptySpaceItem30.AllowHotTrack = false;
		this.emptySpaceItem30.CustomizationFormText = "emptySpaceItem11";
		this.emptySpaceItem30.Location = new System.Drawing.Point(0, 347);
		this.emptySpaceItem30.MaxSize = new System.Drawing.Size(14, 36);
		this.emptySpaceItem30.MinSize = new System.Drawing.Size(14, 36);
		this.emptySpaceItem30.Name = "emptySpaceItem30";
		this.emptySpaceItem30.Size = new System.Drawing.Size(14, 36);
		this.emptySpaceItem30.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem30.Text = "emptySpaceItem11";
		this.emptySpaceItem30.TextSize = new System.Drawing.Size(0, 0);
		this.helpIconLayoutControlItem.Control = this.helpIconUserControl;
		this.helpIconLayoutControlItem.Location = new System.Drawing.Point(129, 383);
		this.helpIconLayoutControlItem.Name = "helpIconLayoutControlItem";
		this.helpIconLayoutControlItem.Size = new System.Drawing.Size(24, 24);
		this.helpIconLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.helpIconLayoutControlItem.TextVisible = false;
		this.learnMoreInfoLayoutControlItem.Control = this.learnMoreInfoUserControl;
		this.learnMoreInfoLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.learnMoreInfoLayoutControlItem.MaxSize = new System.Drawing.Size(0, 35);
		this.learnMoreInfoLayoutControlItem.MinSize = new System.Drawing.Size(104, 35);
		this.learnMoreInfoLayoutControlItem.Name = "learnMoreInfoLayoutControlItem";
		this.learnMoreInfoLayoutControlItem.Size = new System.Drawing.Size(619, 35);
		this.learnMoreInfoLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.learnMoreInfoLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.learnMoreInfoLayoutControlItem.TextVisible = false;
		this.selectFormatPage.Controls.Add(this.fileFormatGridControl);
		this.selectFormatPage.DescriptionText = "";
		this.selectFormatPage.Name = "selectFormatPage";
		this.selectFormatPage.Size = new System.Drawing.Size(639, 427);
		this.selectFormatPage.Text = "Select Format";
		this.fileFormatGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.fileFormatGridControl.Location = new System.Drawing.Point(0, 0);
		this.fileFormatGridControl.MainView = this.fileFormatGridView;
		this.fileFormatGridControl.MenuManager = this.barManager;
		this.fileFormatGridControl.Name = "fileFormatGridControl";
		this.fileFormatGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemButtonEdit1 });
		this.fileFormatGridControl.Size = new System.Drawing.Size(639, 427);
		this.fileFormatGridControl.TabIndex = 2;
		this.fileFormatGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.fileFormatGridView });
		this.fileFormatGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4 });
		this.fileFormatGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.fileFormatGridView.GridControl = this.fileFormatGridControl;
		this.fileFormatGridView.Name = "fileFormatGridView";
		this.fileFormatGridView.OptionsBehavior.AutoPopulateColumns = false;
		this.fileFormatGridView.OptionsBehavior.Editable = false;
		this.fileFormatGridView.OptionsBehavior.ReadOnly = true;
		this.fileFormatGridView.OptionsCustomization.AllowFilter = false;
		this.fileFormatGridView.OptionsDetail.EnableMasterViewMode = false;
		this.fileFormatGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.fileFormatGridView.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Fast;
		this.fileFormatGridView.OptionsView.ShowColumnHeaders = false;
		this.fileFormatGridView.OptionsView.ShowGroupPanel = false;
		this.fileFormatGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.fileFormatGridView.OptionsView.ShowIndicator = false;
		this.fileFormatGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn1.Caption = " ";
		this.gridColumn1.FieldName = "Image";
		this.gridColumn1.MaxWidth = 30;
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ShowCaption = false;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 20;
		this.gridColumn2.Caption = " ";
		this.gridColumn2.FieldName = "DisplayName";
		this.gridColumn2.MinWidth = 100;
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ShowCaption = false;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 100;
		this.gridColumn3.Caption = " ";
		this.gridColumn3.ColumnEdit = this.repositoryItemButtonEdit1;
		this.gridColumn3.FieldName = "URL";
		this.gridColumn3.MaxWidth = 120;
		this.gridColumn3.MinWidth = 120;
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.ShowCaption = false;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn3.Width = 120;
		this.repositoryItemButtonEdit1.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.repositoryItemButtonEdit1.AutoHeight = false;
		this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
		this.gridColumn4.Caption = "gridColumn1";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.gridColumn4.Width = 429;
		this.folderBrowserDialog.SelectedPath = "folderBrowserDialog";
		this.folderBrowserDialog.Title = "Please select Delta Lake folder";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(671, 570);
		base.Controls.Add(this.importFromCloudStorageWizard);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_16;
		base.MaximizeBox = false;
		this.MaximumSize = new System.Drawing.Size(10240, 800);
		base.MinimizeBox = false;
		this.MinimumSize = new System.Drawing.Size(650, 280);
		base.Name = "ImportFromCloudStorageForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Import from cloud storage";
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.importFromCloudStorageWizard).EndInit();
		this.importFromCloudStorageWizard.ResumeLayout(false);
		this.selectCloudStorageProviderWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cloudStorageProviderGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cloudStorageProviderGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.UrlRepositoryItemButtonEdit).EndInit();
		this.connectWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.saveAsComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.advancedCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.advancedCheckEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveAsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.filterInfoLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem30).EndInit();
		((System.ComponentModel.ISupportInitialize)this.helpIconLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.learnMoreInfoLayoutControlItem).EndInit();
		this.selectFormatPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.fileFormatGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fileFormatGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.behaviorManager1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
