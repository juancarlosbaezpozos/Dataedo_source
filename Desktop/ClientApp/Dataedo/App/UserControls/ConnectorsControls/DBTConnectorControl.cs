using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.Model.Data.Documentations;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class DBTConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl dbtLayoutControl;

	private LayoutControlGroup layoutControlGroup6;

	private EmptySpaceItem emptySpaceItem34;

	private LayoutControlItem pathLayoutControlItem;

	private ButtonEdit buttonPathTextEdit;

	private EmptySpaceItem emptySpaceItem1;

	private LayoutControlItem databaseLayoutControlItem;

	private LookUpEdit databaseLookUpEdit;

	protected override ComboBoxEdit HostComboBoxEdit => null;

	protected override TextEdit HostTextEdit => buttonPathTextEdit;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.DBT;

	protected override CheckEdit SavePasswordCheckEdit => null;

	private string ProvidedPath => buttonPathTextEdit.Text;

	public DBTConnectorControl()
	{
		InitializeComponent();
		InitEvents();
		SetDatabasesComboBox();
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
	}

	private void SetDatabasesComboBox()
	{
		List<DocumentationForMenuObject> dataForMenu = DB.Database.GetDataForMenu();
		databaseLookUpEdit.Properties.DataSource = dataForMenu;
		databaseLookUpEdit.Properties.DisplayMember = "Title";
		databaseLookUpEdit.Properties.Columns.Clear();
		databaseLookUpEdit.Properties.Columns.Add(new LookUpColumnInfo("Title"));
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		dbtLayoutControl.Root.Remove(timeoutLayoutControlItem);
		_ = base.SelectedDatabaseType.HasValue;
	}

	public override bool TestConnection(bool testForGettingDatabasesList)
	{
		try
		{
			if (!ValidateRequiredFields(testForGettingDatabasesList))
			{
				GeneralMessageBoxesHandling.Show("Required fields are not filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
				return false;
			}
			try
			{
				SetNewDBRowValues();
			}
			catch (SystemException ex)
			{
				GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
				return false;
			}
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
			ConnectionResult connectionResult = base.DatabaseRow.TryConnection();
			if (connectionResult.IsWarning)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
				if (GeneralMessageBoxesHandling.Show(connectionResult.Message, "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation).DialogResult.Value != DialogResult.Yes)
				{
					return false;
				}
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
			}
			if (connectionResult.IsSuccess)
			{
				base.DatabaseRow.Connection = connectionResult.Connection;
				base.DatabaseRow.DbmsVersion = base.DatabaseRow.GetDbmsVersion(FindForm());
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
				SharedDatabaseTypeEnum.DatabaseType? version = base.DatabaseRow.GetVersion(FindForm());
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
				if (!version.HasValue)
				{
					return false;
				}
				if (version != base.DatabaseRow.Type)
				{
					base.DatabaseRow.Type = version;
					base.SelectedDatabaseType = version;
				}
				return true;
			}
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
			GeneralMessageBoxesHandling.Show(connectionResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, connectionResult.Details, 1, messageSimple: connectionResult.MessageSimple, owner: FindForm());
			return false;
		}
		catch
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
			throw;
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
		}
	}

	protected override void ClearPanelLoginAndPassword()
	{
		buttonPathTextEdit.Text = string.Empty;
	}

	protected override string GetPanelDocumentationTitle()
	{
		try
		{
			return new DirectoryInfo(buttonPathTextEdit.Text).Name;
		}
		catch (Exception ex) when (ex is IOException || ex is ArgumentException || ex is SecurityException)
		{
			return "New dbt Database";
		}
	}

	protected override void OnLoad(EventArgs e)
	{
		base.OnLoad(e);
	}

	protected override void ReadPanelValues()
	{
		buttonPathTextEdit.Text = base.DatabaseRow.Host;
		if (int.TryParse(base.DatabaseRow.Param1, out var databaseId))
		{
			DocumentationForMenuObject editValue = (databaseLookUpEdit.Properties.DataSource as IEnumerable<DocumentationForMenuObject>)?.Where((DocumentationForMenuObject x) => x.DatabaseId == databaseId)?.FirstOrDefault();
			databaseLookUpEdit.EditValue = editValue;
		}
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		DocumentationForMenuObject documentationForMenuObject = databaseLookUpEdit.EditValue as DocumentationForMenuObject;
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, ProvidedPath, documentationTitle, ProvidedPath, null, null, null, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion)
		{
			Param1 = (documentationForMenuObject?.DatabaseId)?.ToString()
		};
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		return ValidateFields.ValidateEdit(buttonPathTextEdit, addDBErrorProvider, "path");
	}

	private void InitEvents()
	{
		buttonPathTextEdit.AllowDrop = true;
		buttonPathTextEdit.DragEnter += ButtonPathTextEdit_DragEnter;
		buttonPathTextEdit.DragDrop += ButtonPathTextEdit_DragDrop;
		buttonPathTextEdit.ButtonClick += ButtonPathTextEdit_ButtonClick;
	}

	private void ButtonPathTextEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		using FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
		folderBrowserDialog.SelectedPath = buttonPathTextEdit.Text;
		if (folderBrowserDialog.ShowDialog(this) == DialogResult.OK)
		{
			buttonPathTextEdit.Text = folderBrowserDialog.SelectedPath;
		}
	}

	private void ButtonPathTextEdit_DragDrop(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			string text = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
			buttonPathTextEdit.Text = text;
		}
	}

	private void ButtonPathTextEdit_DragEnter(object sender, DragEventArgs e)
	{
		DragDropEffects effect = DragDropEffects.None;
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			string[] array = (string[])e.Data.GetData(DataFormats.FileDrop);
			if (array.Length == 1 && Directory.Exists(array[0]))
			{
				effect = DragDropEffects.Copy;
			}
		}
		e.Effect = effect;
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
		this.dbtLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.buttonPathTextEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.databaseLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.pathLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem34 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.databaseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.dbtLayoutControl).BeginInit();
		this.dbtLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.buttonPathTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pathLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem34).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.dbtLayoutControl.AllowCustomization = false;
		this.dbtLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.dbtLayoutControl.Controls.Add(this.buttonPathTextEdit);
		this.dbtLayoutControl.Controls.Add(this.databaseLookUpEdit);
		this.dbtLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dbtLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.dbtLayoutControl.Name = "dbtLayoutControl";
		this.dbtLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(807, 202, 695, 525);
		this.dbtLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.dbtLayoutControl.Root = this.layoutControlGroup6;
		this.dbtLayoutControl.Size = new System.Drawing.Size(492, 340);
		this.dbtLayoutControl.TabIndex = 3;
		this.dbtLayoutControl.Text = "layoutControl1";
		this.buttonPathTextEdit.Location = new System.Drawing.Point(107, 2);
		this.buttonPathTextEdit.Name = "buttonPathTextEdit";
		this.buttonPathTextEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.buttonPathTextEdit.Size = new System.Drawing.Size(255, 20);
		this.buttonPathTextEdit.StyleController = this.dbtLayoutControl;
		this.buttonPathTextEdit.TabIndex = 5;
		this.databaseLookUpEdit.Location = new System.Drawing.Point(107, 26);
		this.databaseLookUpEdit.Name = "databaseLookUpEdit";
		this.databaseLookUpEdit.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.True;
		this.databaseLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.databaseLookUpEdit.Properties.NullText = "Not selected";
		this.databaseLookUpEdit.Properties.NullValuePrompt = "Not selected";
		this.databaseLookUpEdit.Properties.PopupSizeable = false;
		this.databaseLookUpEdit.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.AutoComplete;
		this.databaseLookUpEdit.Properties.ShowFooter = false;
		this.databaseLookUpEdit.Properties.ShowHeader = false;
		this.databaseLookUpEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.databaseLookUpEdit.Size = new System.Drawing.Size(255, 20);
		this.databaseLookUpEdit.StyleController = this.dbtLayoutControl;
		this.databaseLookUpEdit.TabIndex = 7;
		this.layoutControlGroup6.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup6.GroupBordersVisible = false;
		this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.pathLayoutControlItem, this.emptySpaceItem1, this.emptySpaceItem34, this.databaseLayoutControlItem });
		this.layoutControlGroup6.Name = "Root";
		this.layoutControlGroup6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup6.Size = new System.Drawing.Size(492, 340);
		this.layoutControlGroup6.TextVisible = false;
		this.pathLayoutControlItem.Control = this.buttonPathTextEdit;
		this.pathLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.pathLayoutControlItem.Name = "pathLayoutControlItem";
		this.pathLayoutControlItem.Size = new System.Drawing.Size(364, 24);
		this.pathLayoutControlItem.Text = "Path: ";
		this.pathLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.pathLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.pathLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 48);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(364, 292);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem34.AllowHotTrack = false;
		this.emptySpaceItem34.Location = new System.Drawing.Point(364, 0);
		this.emptySpaceItem34.Name = "emptySpaceItem34";
		this.emptySpaceItem34.Size = new System.Drawing.Size(128, 340);
		this.emptySpaceItem34.TextSize = new System.Drawing.Size(0, 0);
		this.databaseLayoutControlItem.Control = this.databaseLookUpEdit;
		this.databaseLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.databaseLayoutControlItem.Name = "databaseLayoutControlItem";
		this.databaseLayoutControlItem.Size = new System.Drawing.Size(364, 24);
		this.databaseLayoutControlItem.Text = "Database (optional): ";
		this.databaseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.databaseLayoutControlItem.TextSize = new System.Drawing.Size(100, 20);
		this.databaseLayoutControlItem.TextToControlDistance = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.dbtLayoutControl);
		base.Name = "DBTConnectorControl";
		base.Size = new System.Drawing.Size(492, 340);
		((System.ComponentModel.ISupportInitialize)this.dbtLayoutControl).EndInit();
		this.dbtLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.buttonPathTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pathLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem34).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
