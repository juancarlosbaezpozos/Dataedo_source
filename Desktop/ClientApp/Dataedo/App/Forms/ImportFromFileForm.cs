using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Helpers.FileImport;
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

public class ImportFromFileForm : BaseXtraForm, IDataModel
{
	private Form parentForm;

	private int databaseId;

	private readonly CustomFieldsSupport customFieldsSupport;

	private bool getDataOnly;

	private DialogResult dialogResult = DialogResult.Cancel;

	private DevExpress.XtraWizard.WizardButton finishButton;

	private ButtonInfo nextButton;

	private bool isOpenFromMainImportWindow;

	private Action _browseButtonClick;

	private BaseWizardPage _firstPage;

	private IContainer components;

	private OpenFileDialog openFileDialog;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ToolTipController toolTipController;

	private SplashScreenManager splashScreenManager;

	private WizardControl importFromFileWizard;

	private DevExpress.XtraWizard.WizardPage selectFormatWizardPage;

	private DevExpress.XtraWizard.WizardPage SelectFilePathPage;

	private Panel selectFilePanel;

	private NonCustomizableLayoutControl mainLayoutControl;

	private SimpleButton browseSimpleButton;

	private TextEdit pathTextEdit;

	private LayoutControlGroup mainLayoutControlGroup;

	private LayoutControlItem pathTextEditLayoutControlItem;

	private LayoutControlItem browseSimpleButtonLayoutControlItem;

	private EmptySpaceItem browseEmptySpaceItem;

	private EmptySpaceItem emptySpaceItem1;

	private GridControl fileFormatGridControl;

	private GridView fileFormatGridView;

	private GridColumn iconGridColumn;

	private GridColumn nameGridColumn;

	private GridColumn urlGridColumn;

	private RepositoryItemButtonEdit UrlRepositoryItemButtonEdit;

	private BehaviorManager behaviorManager1;

	private GridColumn emptyGridColumn;

	private ImageListBoxControl fileFormatSelectionImageList;

	private XtraFolderBrowserDialog folderBrowserDialog;

	private InfoUserControl connectionInfoUserControl;

	private LayoutControlItem connectionInfoLayoutControlItem;

	private DevExpress.XtraWizard.WizardPage selectProviderWizardPage;

	private ChooseFileProviderUserControl chooseFileProviderUserControl1;

	public DataLakeTypeEnum.DataLakeType? DataLakeType { get; private set; }

	public ICloudStorageResultObject CloudStorageResultObject { get; private set; }

	private string Path
	{
		get
		{
			return pathTextEdit.Text.Trim('"');
		}
		set
		{
			pathTextEdit.Text = value;
		}
	}

	private bool IsPathProvided => !string.IsNullOrWhiteSpace(pathTextEdit.Text);

	public int DatabaseId => databaseId;

	public IEnumerable<ObjectModel> ObjectModels { get; private set; }

	public SharedObjectTypeEnum.ObjectType ObjectType { get; private set; }

	public bool FileExists
	{
		get
		{
			if (!File.Exists(Path))
			{
				if (Paths.Length > 1)
				{
					return Paths.Any((string x) => File.Exists(x));
				}
				return false;
			}
			return true;
		}
	}

	private bool HasMultiplePaths => Paths.Length > 1;

	private string[] Paths
	{
		get
		{
			if (CloudStorageResultObject != null)
			{
				return CloudStorageResultObject.CloudStorageItems().ToArray();
			}
			if (DataLakeType == DataLakeTypeEnum.DataLakeType.DELTALAKE)
			{
				return new string[1] { pathTextEdit.Text };
			}
			if (!openFileDialog.Multiselect || !Path.Contains("\""))
			{
				return new string[1] { Path };
			}
			return (from x in Path.Split(new string[1] { "\"" }, StringSplitOptions.RemoveEmptyEntries)
				select x.Trim('"', ' ') into x
				where !string.IsNullOrWhiteSpace(x)
				select x).ToArray();
		}
	}

	public FileImportTypeObject SelectedProvider { get; private set; }

	public ImportFromFileForm(Form parentForm, int databaseId, SharedObjectTypeEnum.ObjectType objectType, CustomFieldsSupport customFieldsSupport)
	{
		this.parentForm = parentForm;
		InitializeComponent();
		this.databaseId = databaseId;
		ObjectType = objectType;
		this.customFieldsSupport = customFieldsSupport;
		importFromFileWizard.SelectedPageChanging += ImportFromFileWizard_SelectedPageChanging;
		importFromFileWizard.CustomizeCommandButtons += ImportFromFileWizard_CustomizeCommandButtons;
		importFromFileWizard.FinishClick += ImportFromFileWizard_FinishClick;
		importFromFileWizard.CancelClick += ImportFromFileWizard_CancelClick;
		base.FormClosing += ImportFromFileForm_FormClosing;
		SetFormatListItems();
		SetFunctionality();
		ImportFileTrackingHelper.TrackImportFileShowForm();
		_browseButtonClick = DefaultBrowseButtonClick;
		_firstPage = importFromFileWizard.Pages[0];
		pathTextEdit.TextChanged += PathTextEdit_TextChanged;
	}

	public ImportFromFileForm(Form parentForm, int databaseId, SharedObjectTypeEnum.ObjectType objectType, CustomFieldsSupport customFieldsSupport, DataLakeTypeEnum.DataLakeType dataLakeType, bool isOpenFromMainImportWindow)
		: this(parentForm, databaseId, objectType, customFieldsSupport)
	{
		SelectDataLakeTypeAndChangePage(dataLakeType);
		this.isOpenFromMainImportWindow = isOpenFromMainImportWindow;
		SetFunctionality();
	}

	private void SelectDataLakeTypeAndChangePage(DataLakeTypeEnum.DataLakeType dataLakeType)
	{
		DataLakeType = dataLakeType;
		int focusedRowHandle = fileFormatGridView.LocateByValue("Value", DataLakeType);
		fileFormatGridView.FocusedRowHandle = focusedRowHandle;
		importFromFileWizard.SelectedPage = SelectFilePathPage;
	}

	private void ImportFromFileForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		base.DialogResult = dialogResult;
	}

	private void ImportFromFileWizard_CustomizeCommandButtons(object sender, CustomizeCommandButtonsEventArgs e)
	{
		e.FinishButton.Text = "Import";
		finishButton = e.FinishButton.Button;
		nextButton = e.NextButton;
		SetImportButtonAvailability();
		ChangeNextButtonAvailability();
		if (ObjectType == SharedObjectTypeEnum.ObjectType.Table && !isOpenFromMainImportWindow)
		{
			e.PrevButton.Visible = false;
		}
		else if (e.Page == _firstPage)
		{
			e.PrevButton.Visible = false;
		}
		else
		{
			e.PrevButton.Visible = true;
		}
	}

	private void SetFunctionality()
	{
		if (ObjectType == SharedObjectTypeEnum.ObjectType.Table)
		{
			if (!isOpenFromMainImportWindow)
			{
				SelectFilePathPage.AllowBack = false;
			}
			else
			{
				SelectFilePathPage.AllowBack = true;
			}
			int focusedRowHandle = fileFormatGridView.LocateByValue("Value", DataLakeTypeEnum.DataLakeType.JSON);
			fileFormatGridView.FocusedRowHandle = focusedRowHandle;
			importFromFileWizard.SelectedPage = SelectFilePathPage;
		}
	}

	private void ImportFromFileWizard_SelectedPageChanging(object sender, WizardPageChangingEventArgs e)
	{
		if (e.Page == SelectFilePathPage)
		{
			SelectedProvider = chooseFileProviderUserControl1.GetSelectedObject();
			if (SelectedProvider != null)
			{
				ImportFileTrackingHelper.TrackImportFileConnectorSelected(SelectedProvider.Value);
			}
			SetSelectFilePathPageOptions();
			base.Size = new Size(MinimumSize.Width, MinimumSize.Height);
			base.FormBorderStyle = FormBorderStyle.FixedDialog;
			pathTextEdit.Text = string.Empty;
			if (fileFormatGridView.GetFocusedRow() is DataLakeTypeObject dataLakeTypeObject)
			{
				DataLakeType = dataLakeTypeObject.Value;
				SelectFilePathPage.Text = "Select " + dataLakeTypeObject.DisplayName + " files";
				ImportFileTrackingHelper.TrackImportFileConnectorSelected(DataLakeType);
			}
			SetConnectionInfoText();
			SetImportButtonAvailability();
		}
		else if (e.Page == selectProviderWizardPage)
		{
			if (e.Direction == Direction.Backward && isOpenFromMainImportWindow)
			{
				if (parentForm is ConnectAndSynchForm connectAndSynchForm)
				{
					connectAndSynchForm.SetPageBackFromImportFile(databaseId, ObjectType);
					connectAndSynchForm.Show();
				}
				Hide();
				Close();
			}
			base.Size = new Size(base.Size.Width, 600);
			base.FormBorderStyle = FormBorderStyle.Sizable;
			chooseFileProviderUserControl1.Init();
		}
		else
		{
			base.Size = new Size(base.Size.Width, 600);
			base.FormBorderStyle = FormBorderStyle.Sizable;
		}
	}

	private void SetSelectFilePathPageOptions()
	{
		FileImportTypeEnum.FileImportType? fileImportType = SelectedProvider?.Value;
		if (fileImportType.HasValue)
		{
			FileImportTypeEnum.FileImportType valueOrDefault = fileImportType.GetValueOrDefault();
			if ((uint)(valueOrDefault - 1) <= 2u)
			{
				browseSimpleButton.Text = "Connect";
				_browseButtonClick = CloudStorageClick;
				pathTextEdit.ReadOnly = true;
				return;
			}
		}
		browseSimpleButton.Text = "Browse";
		_browseButtonClick = DefaultBrowseButtonClick;
		pathTextEdit.ReadOnly = false;
	}

	public ImportFromFileForm(Form parentForm, int databaseId, bool getDataOnly, SharedObjectTypeEnum.ObjectType objectType, CustomFieldsSupport customFieldsSupport)
		: this(parentForm, databaseId, objectType, customFieldsSupport)
	{
		this.getDataOnly = getDataOnly;
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Escape)
		{
			Close();
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void BrowseSimpleButton_Click(object sender, EventArgs e)
	{
		if (_browseButtonClick == null)
		{
			DefaultBrowseButtonClick();
		}
		else
		{
			_browseButtonClick();
		}
		SetImportButtonAvailability();
	}

	private void DefaultBrowseButtonClick()
	{
		CloudStorageResultObject = null;
		if (DataLakeType == DataLakeTypeEnum.DataLakeType.DELTALAKE)
		{
			if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
			{
				pathTextEdit.Text = folderBrowserDialog.SelectedPath;
			}
			return;
		}
		openFileDialog.FileName = pathTextEdit.Text;
		if (DataLakeType.HasValue)
		{
			openFileDialog.Filter = DataLakeTypeEnum.GetTypeFilesFilter(DataLakeType.Value) + "|All files (*.*)|*.*";
		}
		if (openFileDialog.ShowDialog(this) == DialogResult.OK)
		{
			if (openFileDialog.FileNames.Length == 1)
			{
				pathTextEdit.Text = openFileDialog.FileName;
			}
			else
			{
				pathTextEdit.Text = "\"" + string.Join("\" \"", openFileDialog.FileNames) + "\"";
			}
		}
	}

	private void ImportFromFileWizard_FinishClick(object sender, CancelEventArgs e)
	{
		if (!SetImportButtonAvailability())
		{
			e.Cancel = true;
		}
		else
		{
			if (!DataLakeType.HasValue)
			{
				return;
			}
			List<ImportItem> list;
			try
			{
				list = ((CloudStorageResultObject != null) ? CloudStorageResultObject.GetStreamableImportItems() : ((DataLakeType != DataLakeTypeEnum.DataLakeType.DELTALAKE) ? ((IEnumerable<ImportItem>)Paths.Select((string x) => new LocalFileImportItem(x))).ToList() : ((IEnumerable<ImportItem>)Paths.Select((string x) => new LocalFolderImportItem(x))).ToList()));
			}
			catch (IOException ex)
			{
				e.Cancel = true;
				GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
				return;
			}
			ImportResult importResult = FilesImporter.ImportItems(list, DataLakeType, ObjectType, splashScreenManager, this);
			if (importResult.CancelEvent)
			{
				e.Cancel = true;
			}
			if (!importResult.Success)
			{
				foreach (ImportItem item in list)
				{
					if (item.DeleteFileAfterImport)
					{
						item.DeleteTemporaryFiles();
					}
				}
				return;
			}
			if (getDataOnly)
			{
				ObjectModels = importResult.ObjectModels;
				dialogResult = DialogResult.OK;
				return;
			}
			DataLakeImportProcessor dataLakeImportProcessor = new DataLakeImportProcessor(databaseId, ObjectType, this, splashScreenManager, customFieldsSupport);
			if (!dataLakeImportProcessor.ProcessImport(DataLakeType, importResult.ObjectModels))
			{
				dialogResult = DialogResult.Cancel;
				{
					foreach (ObjectModel objectModel in importResult.ObjectModels)
					{
						if (objectModel.ImportItem != null && objectModel.ImportItem.DeleteFileAfterImport)
						{
							objectModel.ImportItem.DeleteTemporaryFiles();
						}
					}
					return;
				}
			}
			ObjectModels = dataLakeImportProcessor.ObjectModels;
			foreach (ObjectModel item2 in dataLakeImportProcessor.AllModels.Concat(importResult.ObjectModels))
			{
				if (item2.ImportItem != null && item2.ImportItem.DeleteFileAfterImport)
				{
					item2.ImportItem.DeleteTemporaryFiles();
				}
			}
			dialogResult = DialogResult.OK;
		}
	}

	private void ImportFromFileWizard_CancelClick(object sender, EventArgs e)
	{
		dialogResult = DialogResult.Cancel;
	}

	private bool SetImportButtonAvailability()
	{
		if (finishButton == null)
		{
			return false;
		}
		pathTextEdit.ErrorText = null;
		FileImportTypeEnum.FileImportType? fileImportType = SelectedProvider?.Value;
		if (fileImportType.HasValue)
		{
			FileImportTypeEnum.FileImportType valueOrDefault = fileImportType.GetValueOrDefault();
			if ((uint)(valueOrDefault - 1) <= 2u)
			{
				finishButton.Enabled = IsPathProvided || HasMultiplePaths;
				return finishButton.Enabled;
			}
		}
		if (string.IsNullOrWhiteSpace(Path))
		{
			finishButton.Enabled = false;
			return finishButton.Enabled;
		}
		if (DataLakeType == DataLakeTypeEnum.DataLakeType.DELTALAKE)
		{
			if (Directory.Exists(Path))
			{
				finishButton.Enabled = true;
			}
			else
			{
				finishButton.Enabled = false;
				pathTextEdit.ErrorText = "Provided directory does not exist";
			}
		}
		else if (FileExists)
		{
			finishButton.Enabled = true;
		}
		else
		{
			finishButton.Enabled = false;
			pathTextEdit.ErrorText = "Provided file(s) does not exist";
		}
		return finishButton.Enabled;
	}

	private void ImportFromFileForm_Shown(object sender, EventArgs e)
	{
		if (parentForm != null)
		{
			parentForm.Visible = false;
		}
	}

	private void SetFormatListItems()
	{
		fileFormatGridControl.DataSource = DataLakeTypeEnum.GetDataLakeTypeObjects();
		fileFormatGridView.FocusedRowChanged += FileFormatGridView_FocusedRowChanged;
		fileFormatGridView.MouseMove += FileFormatGridView_MouseMove;
		fileFormatGridView.MouseDown += FileFormatGridView_MouseDown;
		fileFormatGridView.RowStyle += FileFormatGridView_RowStyle;
	}

	private void FileFormatGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		ChangeNextButtonAvailability();
	}

	private void ChangeNextButtonAvailability()
	{
		if (fileFormatGridView.GetFocusedRow() is DataLakeTypeObject dataLakeTypeObject && nextButton != null)
		{
			if (!Connectors.HasDataLakeTypeConnector(dataLakeTypeObject.Value))
			{
				nextButton.Button.Enabled = false;
			}
			else
			{
				nextButton.Button.Enabled = true;
			}
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
		if (!Connectors.HasAllConnectors() && e is DXMouseEventArgs && sender is GridView gridView)
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

	private void CloudStorageClick()
	{
		CloudStorageTypeEnum.CloudStorageType? cloudStorageType = FileImportTypeEnum.ToCloudStorageType(SelectedProvider.Value);
		Form form = parentForm;
		int? num = databaseId;
		SharedObjectTypeEnum.ObjectType objectType = ObjectType;
		CustomFieldsSupport obj = customFieldsSupport;
		CloudStorageTypeEnum.CloudStorageType? cloudStorageType2 = cloudStorageType;
		using ImportFromCloudStorageForm importFromCloudStorageForm = new ImportFromCloudStorageForm(form, num, objectType, obj, DataLakeType, cloudStorageType2, null, importData: false);
		if (importFromCloudStorageForm.ShowDialog(this) == DialogResult.OK)
		{
			CloudStorageResultObject = importFromCloudStorageForm.CloudStorageResultObject;
			Path = string.Join(" ", from x in CloudStorageResultObject.CloudStorageItems()
				select "\"" + x + "\"");
		}
	}

	private void SetConnectionInfoText()
	{
		if (!DataLakeType.HasValue)
		{
			connectionInfoLayoutControlItem.Visibility = LayoutVisibility.Never;
			return;
		}
		string text = DataLakeImportFactory.GetDataLakeImport(ObjectType, DataLakeType.Value)?.DocumentationLink;
		if (string.IsNullOrEmpty(text))
		{
			connectionInfoLayoutControlItem.Visibility = LayoutVisibility.Never;
		}
		else
		{
			connectionInfoLayoutControlItem.Visibility = LayoutVisibility.Always;
		}
		connectionInfoUserControl.Description = "<href=" + text + ">Learn more</href> about supported versions and <href=" + text + ">how to connect</href>.";
	}

	private void PathTextEdit_TextChanged(object sender, EventArgs e)
	{
		SetImportButtonAvailability();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.ImportFromFileForm));
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
		this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true);
		this.importFromFileWizard = new DevExpress.XtraWizard.WizardControl();
		this.selectFormatWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.fileFormatGridControl = new DevExpress.XtraGrid.GridControl();
		this.fileFormatGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.iconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.urlGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.UrlRepositoryItemButtonEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.emptyGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.SelectFilePathPage = new DevExpress.XtraWizard.WizardPage();
		this.selectFilePanel = new System.Windows.Forms.Panel();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.connectionInfoUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.browseSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.pathTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.pathTextEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.browseSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.browseEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.connectionInfoLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.selectProviderWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.chooseFileProviderUserControl1 = new Dataedo.App.UserControls.ChooseFileProviderUserControl();
		this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
		this.folderBrowserDialog = new DevExpress.XtraEditors.XtraFolderBrowserDialog(this.components);
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.importFromFileWizard).BeginInit();
		this.importFromFileWizard.SuspendLayout();
		this.selectFormatWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.fileFormatGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fileFormatGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.UrlRepositoryItemButtonEdit).BeginInit();
		this.SelectFilePathPage.SuspendLayout();
		this.selectFilePanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pathTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pathTextEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.browseSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.browseEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectionInfoLayoutControlItem).BeginInit();
		this.selectProviderWizardPage.SuspendLayout();
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
		this.importFromFileWizard.Controls.Add(this.selectFormatWizardPage);
		this.importFromFileWizard.Controls.Add(this.SelectFilePathPage);
		this.importFromFileWizard.Controls.Add(this.selectProviderWizardPage);
		this.importFromFileWizard.Dock = System.Windows.Forms.DockStyle.Fill;
		this.importFromFileWizard.Name = "importFromFileWizard";
		this.importFromFileWizard.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[3] { this.selectFormatWizardPage, this.selectProviderWizardPage, this.SelectFilePathPage });
		this.importFromFileWizard.Size = new System.Drawing.Size(671, 570);
		this.selectFormatWizardPage.Controls.Add(this.fileFormatGridControl);
		this.selectFormatWizardPage.DescriptionText = "";
		this.selectFormatWizardPage.Name = "selectFormatWizardPage";
		this.selectFormatWizardPage.Size = new System.Drawing.Size(639, 427);
		this.selectFormatWizardPage.Text = "Select format";
		this.fileFormatGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.fileFormatGridControl.Location = new System.Drawing.Point(0, 0);
		this.fileFormatGridControl.MainView = this.fileFormatGridView;
		this.fileFormatGridControl.MenuManager = this.barManager;
		this.fileFormatGridControl.Name = "fileFormatGridControl";
		this.fileFormatGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.UrlRepositoryItemButtonEdit });
		this.fileFormatGridControl.Size = new System.Drawing.Size(639, 427);
		this.fileFormatGridControl.TabIndex = 1;
		this.fileFormatGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.fileFormatGridView });
		this.fileFormatGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.iconGridColumn, this.nameGridColumn, this.urlGridColumn, this.emptyGridColumn });
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
		this.iconGridColumn.Caption = " ";
		this.iconGridColumn.FieldName = "Image";
		this.iconGridColumn.MaxWidth = 30;
		this.iconGridColumn.Name = "iconGridColumn";
		this.iconGridColumn.OptionsColumn.ShowCaption = false;
		this.iconGridColumn.Visible = true;
		this.iconGridColumn.VisibleIndex = 0;
		this.iconGridColumn.Width = 20;
		this.nameGridColumn.Caption = " ";
		this.nameGridColumn.FieldName = "DisplayName";
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
		this.SelectFilePathPage.Controls.Add(this.selectFilePanel);
		this.SelectFilePathPage.DescriptionText = "";
		this.SelectFilePathPage.Name = "SelectFilePathPage";
		this.SelectFilePathPage.Size = new System.Drawing.Size(639, 427);
		this.SelectFilePathPage.Text = "Select files";
		this.selectFilePanel.Controls.Add(this.mainLayoutControl);
		this.selectFilePanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.selectFilePanel.Location = new System.Drawing.Point(0, 0);
		this.selectFilePanel.Name = "selectFilePanel";
		this.selectFilePanel.Size = new System.Drawing.Size(639, 427);
		this.selectFilePanel.TabIndex = 8;
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.connectionInfoUserControl);
		this.mainLayoutControl.Controls.Add(this.browseSimpleButton);
		this.mainLayoutControl.Controls.Add(this.pathTextEdit);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.MenuManager = this.barManager;
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1340, 528, 839, 627);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(639, 427);
		this.mainLayoutControl.TabIndex = 1;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.mainLayoutControl.ToolTipController = this.toolTipController;
		this.connectionInfoUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.connectionInfoUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.connectionInfoUserControl.Description = "<href=>Learn more</href> about supported versions and how to connect.";
		this.connectionInfoUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.connectionInfoUserControl.Image = (System.Drawing.Image)resources.GetObject("connectionInfoUserControl.Image");
		this.connectionInfoUserControl.Location = new System.Drawing.Point(2, 2);
		this.connectionInfoUserControl.Name = "connectionInfoUserControl";
		this.connectionInfoUserControl.Size = new System.Drawing.Size(635, 35);
		this.connectionInfoUserControl.TabIndex = 16;
		this.browseSimpleButton.Location = new System.Drawing.Point(552, 41);
		this.browseSimpleButton.Name = "browseSimpleButton";
		this.browseSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.browseSimpleButton.StyleController = this.mainLayoutControl;
		this.browseSimpleButton.TabIndex = 15;
		this.browseSimpleButton.Text = "Browse";
		this.browseSimpleButton.Click += new System.EventHandler(BrowseSimpleButton_Click);
		this.pathTextEdit.Location = new System.Drawing.Point(2, 42);
		this.pathTextEdit.Name = "pathTextEdit";
		this.pathTextEdit.Properties.EditValueChangedDelay = 1000;
		this.pathTextEdit.Properties.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Buffered;
		this.pathTextEdit.Size = new System.Drawing.Size(533, 20);
		this.pathTextEdit.StyleController = this.mainLayoutControl;
		this.pathTextEdit.TabIndex = 14;
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.pathTextEditLayoutControlItem, this.browseSimpleButtonLayoutControlItem, this.browseEmptySpaceItem, this.emptySpaceItem1, this.connectionInfoLayoutControlItem });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(639, 427);
		this.mainLayoutControlGroup.TextVisible = false;
		this.pathTextEditLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.pathTextEditLayoutControlItem.Control = this.pathTextEdit;
		this.pathTextEditLayoutControlItem.Location = new System.Drawing.Point(0, 39);
		this.pathTextEditLayoutControlItem.Name = "pathTextEditLayoutControlItem";
		this.pathTextEditLayoutControlItem.Size = new System.Drawing.Size(537, 26);
		this.pathTextEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.pathTextEditLayoutControlItem.TextVisible = false;
		this.browseSimpleButtonLayoutControlItem.Control = this.browseSimpleButton;
		this.browseSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(550, 39);
		this.browseSimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(89, 26);
		this.browseSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(89, 26);
		this.browseSimpleButtonLayoutControlItem.Name = "browseSimpleButtonLayoutControlItem";
		this.browseSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(89, 26);
		this.browseSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.browseSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.browseSimpleButtonLayoutControlItem.TextVisible = false;
		this.browseEmptySpaceItem.AllowHotTrack = false;
		this.browseEmptySpaceItem.Location = new System.Drawing.Point(537, 39);
		this.browseEmptySpaceItem.MaxSize = new System.Drawing.Size(13, 26);
		this.browseEmptySpaceItem.MinSize = new System.Drawing.Size(13, 26);
		this.browseEmptySpaceItem.Name = "browseEmptySpaceItem";
		this.browseEmptySpaceItem.Size = new System.Drawing.Size(13, 26);
		this.browseEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.browseEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 65);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(639, 362);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.connectionInfoLayoutControlItem.Control = this.connectionInfoUserControl;
		this.connectionInfoLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.connectionInfoLayoutControlItem.MaxSize = new System.Drawing.Size(0, 39);
		this.connectionInfoLayoutControlItem.MinSize = new System.Drawing.Size(104, 39);
		this.connectionInfoLayoutControlItem.Name = "connectionInfoLayoutControlItem";
		this.connectionInfoLayoutControlItem.Size = new System.Drawing.Size(639, 39);
		this.connectionInfoLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.connectionInfoLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.connectionInfoLayoutControlItem.TextVisible = false;
		this.selectProviderWizardPage.Controls.Add(this.chooseFileProviderUserControl1);
		this.selectProviderWizardPage.DescriptionText = "";
		this.selectProviderWizardPage.Name = "selectProviderWizardPage";
		this.selectProviderWizardPage.Size = new System.Drawing.Size(639, 427);
		this.selectProviderWizardPage.Text = "Select Provider";
		this.chooseFileProviderUserControl1.BackColor = System.Drawing.Color.Transparent;
		this.chooseFileProviderUserControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.chooseFileProviderUserControl1.Location = new System.Drawing.Point(0, 0);
		this.chooseFileProviderUserControl1.Name = "chooseFileProviderUserControl1";
		this.chooseFileProviderUserControl1.Size = new System.Drawing.Size(639, 427);
		this.chooseFileProviderUserControl1.TabIndex = 0;
		this.folderBrowserDialog.SelectedPath = "folderBrowserDialog";
		this.folderBrowserDialog.Title = "Please select Delta Lake folder";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(671, 570);
		base.Controls.Add(this.importFromFileWizard);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_16;
		base.MaximizeBox = false;
		this.MaximumSize = new System.Drawing.Size(10240, 800);
		base.MinimizeBox = false;
		this.MinimumSize = new System.Drawing.Size(650, 280);
		base.Name = "ImportFromFileForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Import from file";
		base.Shown += new System.EventHandler(ImportFromFileForm_Shown);
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.importFromFileWizard).EndInit();
		this.importFromFileWizard.ResumeLayout(false);
		this.selectFormatWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.fileFormatGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fileFormatGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.UrlRepositoryItemButtonEdit).EndInit();
		this.SelectFilePathPage.ResumeLayout(false);
		this.selectFilePanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pathTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pathTextEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.browseSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.browseEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectionInfoLayoutControlItem).EndInit();
		this.selectProviderWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.behaviorManager1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
