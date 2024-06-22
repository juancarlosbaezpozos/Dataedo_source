using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Import.DataLake;
using Dataedo.App.Import.DataLake.Interfaces;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.Exceptions;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
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

public class ImportPasteDocumentForm : BaseXtraForm, IDataModel
{
	private Form parentForm;

	private int databaseId;

	private readonly CustomFieldsSupport customFieldsSupport;

	private ButtonInfo nextButton;

	private IContainer components;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ToolTipController toolTipController;

	private SplashScreenManager splashScreenManager;

	private WizardControl pasteDocumentWizard;

	private DevExpress.XtraWizard.WizardPage selectFormatWizardPage;

	private DevExpress.XtraWizard.WizardPage pasteDocumentWizardPage;

	private Panel pasteDocumentPanel;

	private NonCustomizableLayoutControl mainLayoutControl;

	private MemoEdit documentMemoEdit;

	private LayoutControlGroup mainLayoutControlGroup;

	private LayoutControlItem documentMemoEditLayoutControlItem;

	private GridControl fileFormatGridControl;

	private GridView fileFormatGridView;

	private GridColumn iconGridColumn;

	private GridColumn nameGridColumn;

	private GridColumn urlGridColumn;

	private RepositoryItemButtonEdit UrlRepositoryItemButtonEdit;

	private GridColumn emptyGridColumn;

	private string Document
	{
		get
		{
			return documentMemoEdit.Text;
		}
		set
		{
			documentMemoEdit.Text = value;
		}
	}

	private DataLakeTypeEnum.DataLakeType? DataLakeType { get; set; }

	public IEnumerable<ObjectModel> ObjectModels { get; private set; }

	public SharedObjectTypeEnum.ObjectType ObjectType { get; private set; }

	public ImportPasteDocumentForm(Form parentForm, int databaseId, SharedObjectTypeEnum.ObjectType objectType, CustomFieldsSupport customFieldsSupport)
	{
		this.parentForm = parentForm;
		InitializeComponent();
		this.databaseId = databaseId;
		ObjectType = objectType;
		this.customFieldsSupport = customFieldsSupport;
		pasteDocumentWizard.SelectedPageChanging += PasteDocumentWizard_SelectedPageChanging;
		pasteDocumentWizard.CustomizeCommandButtons += PasteDocumentWizard_CustomizeCommandButtons;
		pasteDocumentWizard.CancelClick += ImportFromFileWizard_CancelClick;
		SetFormatListItems();
		SetFunctionality();
		ImportFileTrackingHelper.TrackImportFileShowForm();
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.S | Keys.Control:
			if (DataLakeType.HasValue)
			{
				base.DialogResult = DialogResult.OK;
				Close();
			}
			break;
		case Keys.Escape:
			Close();
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void Save()
	{
		if (!DataLakeType.HasValue)
		{
			return;
		}
		IEnumerable<ObjectModel> enumerable = null;
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			enumerable = DataLakeImportFactory.GetDataLakeImport(ObjectType, DataLakeType.Value).GetObjectsFromData(Document);
			ImportFileTrackingHelper.TrackImportFileReadSuccess(enumerable.SelectMany((ObjectModel o) => o.Fields).Count(), "0", DataLakeType);
		}
		catch (InvalidDataProvidedException ex)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, ex?.InnerException?.Message, 1, this);
			base.DialogResult = DialogResult.Cancel;
			ImportFileTrackingHelper.TrackImportFileReadFailed("0", DataLakeType);
			return;
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
		DataLakeImportProcessor dataLakeImportProcessor = new DataLakeImportProcessor(databaseId, ObjectType, this, splashScreenManager, customFieldsSupport);
		if (dataLakeImportProcessor.ProcessImport(DataLakeType.Value, enumerable))
		{
			ObjectModels = dataLakeImportProcessor.ObjectModels;
			base.DialogResult = DialogResult.OK;
		}
		else
		{
			base.DialogResult = DialogResult.Cancel;
		}
	}

	private void ImportFromFileWizard_CancelClick(object sender, EventArgs e)
	{
		Close();
	}

	private void ImportPasteDocumentForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (base.DialogResult == DialogResult.OK)
		{
			Save();
			if (base.DialogResult == DialogResult.Cancel)
			{
				e.Cancel = true;
			}
		}
		else
		{
			if (string.IsNullOrEmpty(documentMemoEdit.Text))
			{
				return;
			}
			DialogResult? dialogResult = GeneralMessageBoxesHandling.Show("Data has been changed, would you like to save these changes?", "Data has been changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
			if (dialogResult == DialogResult.Yes)
			{
				if (!DataLakeType.HasValue)
				{
					e.Cancel = true;
				}
				else
				{
					Save();
				}
			}
			else if (dialogResult != DialogResult.No)
			{
				base.DialogResult = DialogResult.Cancel;
				e.Cancel = true;
			}
		}
	}

	private void ImportPasteDocumentForm_Shown(object sender, EventArgs e)
	{
		if (parentForm != null)
		{
			parentForm.Visible = false;
		}
	}

	private void SetFormatListItems()
	{
		fileFormatGridControl.DataSource = DataLakeTypeEnum.GetDataLakePasteTypeObjects();
		fileFormatGridView.FocusedRowChanged += FileFormatGridView_FocusedRowChanged;
		fileFormatGridView.MouseMove += FileFormatGridView_MouseMove;
		fileFormatGridView.MouseDown += FileFormatGridView_MouseDown;
		fileFormatGridView.RowStyle += FileFormatGridView_RowStyle;
	}

	private void SetFunctionality()
	{
		if (ObjectType == SharedObjectTypeEnum.ObjectType.Table)
		{
			pasteDocumentWizardPage.AllowBack = false;
			int focusedRowHandle = fileFormatGridView.LocateByValue("Value", DataLakeTypeEnum.DataLakeType.JSON);
			fileFormatGridView.FocusedRowHandle = focusedRowHandle;
			pasteDocumentWizard.SelectedPage = pasteDocumentWizardPage;
		}
	}

	private void PasteDocumentWizard_SelectedPageChanging(object sender, WizardPageChangingEventArgs e)
	{
		if (e.Page == pasteDocumentWizardPage)
		{
			documentMemoEdit.Text = string.Empty;
			if (fileFormatGridView.GetFocusedRow() is DataLakeTypeObject dataLakeTypeObject)
			{
				DataLakeType = dataLakeTypeObject.Value;
				pasteDocumentWizardPage.Text = "Paste " + dataLakeTypeObject.DisplayName + " document";
				ImportFileTrackingHelper.TrackImportFileConnectorSelected(DataLakeType);
			}
		}
	}

	private void PasteDocumentWizard_CustomizeCommandButtons(object sender, CustomizeCommandButtonsEventArgs e)
	{
		e.FinishButton.Text = "Import";
		nextButton = e.NextButton;
		ChangeNextButtonAvailability();
		if (ObjectType == SharedObjectTypeEnum.ObjectType.Table)
		{
			e.PrevButton.Visible = false;
		}
		else if (e.Page == selectFormatWizardPage)
		{
			e.PrevButton.Visible = false;
		}
		else
		{
			e.PrevButton.Visible = true;
		}
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
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true);
		this.pasteDocumentWizard = new DevExpress.XtraWizard.WizardControl();
		this.selectFormatWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.fileFormatGridControl = new DevExpress.XtraGrid.GridControl();
		this.fileFormatGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.iconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.urlGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.UrlRepositoryItemButtonEdit = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.emptyGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.pasteDocumentWizardPage = new DevExpress.XtraWizard.WizardPage();
		this.pasteDocumentPanel = new System.Windows.Forms.Panel();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.documentMemoEdit = new DevExpress.XtraEditors.MemoEdit();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.documentMemoEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pasteDocumentWizard).BeginInit();
		this.pasteDocumentWizard.SuspendLayout();
		this.selectFormatWizardPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.fileFormatGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fileFormatGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.UrlRepositoryItemButtonEdit).BeginInit();
		this.pasteDocumentWizardPage.SuspendLayout();
		this.pasteDocumentPanel.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.documentMemoEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.documentMemoEditLayoutControlItem).BeginInit();
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
		this.barDockControlTop.Size = new System.Drawing.Size(760, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 526);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(760, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 526);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(760, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 526);
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.splashScreenManager.ClosingDelay = 500;
		this.pasteDocumentWizard.Controls.Add(this.selectFormatWizardPage);
		this.pasteDocumentWizard.Controls.Add(this.pasteDocumentWizardPage);
		this.pasteDocumentWizard.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pasteDocumentWizard.Name = "pasteDocumentWizard";
		this.pasteDocumentWizard.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[2] { this.selectFormatWizardPage, this.pasteDocumentWizardPage });
		this.pasteDocumentWizard.Size = new System.Drawing.Size(760, 526);
		this.selectFormatWizardPage.Controls.Add(this.fileFormatGridControl);
		this.selectFormatWizardPage.DescriptionText = "";
		this.selectFormatWizardPage.Name = "selectFormatWizardPage";
		this.selectFormatWizardPage.Size = new System.Drawing.Size(728, 383);
		this.selectFormatWizardPage.Text = "Select format";
		this.fileFormatGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.fileFormatGridControl.Location = new System.Drawing.Point(0, 0);
		this.fileFormatGridControl.MainView = this.fileFormatGridView;
		this.fileFormatGridControl.MenuManager = this.barManager;
		this.fileFormatGridControl.Name = "fileFormatGridControl";
		this.fileFormatGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.UrlRepositoryItemButtonEdit });
		this.fileFormatGridControl.Size = new System.Drawing.Size(728, 383);
		this.fileFormatGridControl.TabIndex = 2;
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
		this.nameGridColumn.MinWidth = 40;
		this.nameGridColumn.Name = "nameGridColumn";
		this.nameGridColumn.OptionsColumn.ShowCaption = false;
		this.nameGridColumn.Visible = true;
		this.nameGridColumn.VisibleIndex = 1;
		this.nameGridColumn.Width = 40;
		this.urlGridColumn.Caption = " ";
		this.urlGridColumn.ColumnEdit = this.UrlRepositoryItemButtonEdit;
		this.urlGridColumn.FieldName = "URL";
		this.urlGridColumn.MaxWidth = 120;
		this.urlGridColumn.MinWidth = 10;
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
		this.pasteDocumentWizardPage.Controls.Add(this.pasteDocumentPanel);
		this.pasteDocumentWizardPage.DescriptionText = "";
		this.pasteDocumentWizardPage.Name = "pasteDocumentWizardPage";
		this.pasteDocumentWizardPage.Size = new System.Drawing.Size(728, 383);
		this.pasteDocumentWizardPage.Text = "Paste document";
		this.pasteDocumentPanel.Controls.Add(this.mainLayoutControl);
		this.pasteDocumentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pasteDocumentPanel.Location = new System.Drawing.Point(0, 0);
		this.pasteDocumentPanel.Name = "pasteDocumentPanel";
		this.pasteDocumentPanel.Size = new System.Drawing.Size(728, 383);
		this.pasteDocumentPanel.TabIndex = 9;
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.documentMemoEdit);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.MenuManager = this.barManager;
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3160, 704, 892, 496);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(728, 383);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "mainLayoutControl";
		this.mainLayoutControl.ToolTipController = this.toolTipController;
		this.documentMemoEdit.Location = new System.Drawing.Point(5, 5);
		this.documentMemoEdit.MenuManager = this.barManager;
		this.documentMemoEdit.Name = "documentMemoEdit";
		this.documentMemoEdit.Size = new System.Drawing.Size(718, 373);
		this.documentMemoEdit.StyleController = this.mainLayoutControl;
		this.documentMemoEdit.TabIndex = 12;
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.documentMemoEditLayoutControlItem });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(728, 383);
		this.mainLayoutControlGroup.TextVisible = false;
		this.documentMemoEditLayoutControlItem.Control = this.documentMemoEdit;
		this.documentMemoEditLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.documentMemoEditLayoutControlItem.Name = "documentMemoEditLayoutControlItem";
		this.documentMemoEditLayoutControlItem.Size = new System.Drawing.Size(722, 377);
		this.documentMemoEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.documentMemoEditLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(760, 526);
		base.Controls.Add(this.pasteDocumentWizard);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_16;
		this.MinimumSize = new System.Drawing.Size(510, 350);
		base.Name = "ImportPasteDocumentForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Paste Document";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ImportPasteDocumentForm_FormClosing);
		base.Shown += new System.EventHandler(ImportPasteDocumentForm_Shown);
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pasteDocumentWizard).EndInit();
		this.pasteDocumentWizard.ResumeLayout(false);
		this.selectFormatWizardPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.fileFormatGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fileFormatGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.UrlRepositoryItemButtonEdit).EndInit();
		this.pasteDocumentWizardPage.ResumeLayout(false);
		this.pasteDocumentPanel.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.documentMemoEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.documentMemoEditLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
