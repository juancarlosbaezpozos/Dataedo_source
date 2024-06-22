using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class DataverseConnectorControl : ConnectorControlBase
{
	private const string DefaultMsAppId = "51f81489-12ee-4a9e-aaae-a2591f45987d";

	private const string DefaultMsRedirectUri = "app://58145B91-0C36-4500-8554-080854F2AC97";

	private IContainer components;

	private NonCustomizableLayoutControl sqlServerLayoutControl;

	private CheckEdit savePasswordCheckEdit;

	private TextEdit loginTextEdit;

	private TextEdit passwordTextEdit;

	private ComboBoxEdit hostComboBoxEdit;

	private LayoutControlGroup layoutControlGroup4;

	private LayoutControlItem passwordLayoutControlItem;

	private LayoutControlItem loginLayoutControlItem;

	private LayoutControlItem sqlServerSavePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem5;

	private EmptySpaceItem emptySpaceItem8;

	private EmptySpaceItem emptySpaceItem9;

	private EmptySpaceItem timeoutEmptySpaceItem;

	private LayoutControlItem hostLayoutControlItem;

	private EmptySpaceItem emptySpaceItem3;

	private EmptySpaceItem emptySpaceItem4;

	private LookUpEdit langaugeLookUpEdit;

	private LayoutControlItem langaugeLayoutControlItem;

	private TextEdit appIdTextEdit;

	private TextEdit redirectUriTextEdit;

	private LayoutControlItem appIdLayoutControlItem;

	private LayoutControlItem redirectUriLayoutControlItem;

	private PictureEdit pictureEdit1;

	private ToolTipController toolTipController;

	private LabelControl labelControl1;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem redirectImageLayoutControlItem;

	private EmptySpaceItem emptySpaceItem2;

	private PictureEdit pictureEdit2;

	private LabelControl labelControl2;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem appIdPictureLayoutControlItem;

	protected string providedHost => splittedHost?.Host ?? hostComboBoxEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Dataverse;

	protected override bool ShouldShowWaitingPanelWhileConnect => false;

	protected override TextEdit HostTextEdit => hostComboBoxEdit;

	protected override ComboBoxEdit HostComboBoxEdit => hostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => savePasswordCheckEdit;

	public DataverseConnectorControl()
	{
		InitializeComponent();
	}

	private static DataTable GetLanguageDataTabel()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("id", typeof(int));
		dataTable.Columns.Add("type", typeof(string));
		dataTable.Rows.Add(1, "User localized language");
		dataTable.Rows.Add(2, "Organization base language");
		return dataTable;
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
		DataTable languageDataTabel = GetLanguageDataTabel();
		langaugeLookUpEdit.Properties.DataSource = languageDataTabel;
		langaugeLookUpEdit.Properties.DropDownRows = languageDataTabel.Rows.Count;
		if (string.IsNullOrEmpty(base.DatabaseRow.Param3))
		{
			appIdTextEdit.Text = "51f81489-12ee-4a9e-aaae-a2591f45987d";
		}
		if (string.IsNullOrEmpty(base.DatabaseRow.Param4))
		{
			redirectUriTextEdit.Text = "app://58145B91-0C36-4500-8554-080854F2AC97";
		}
		if (string.IsNullOrEmpty(base.DatabaseRow.Param2))
		{
			langaugeLookUpEdit.EditValue = 1;
		}
	}

	private static int GetLookupIndex(LookUpEdit lookupEdit)
	{
		return Convert.ToInt32(lookupEdit.EditValue);
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, providedHost, documentationTitle, providedHost, loginTextEdit.Text, passwordTextEdit.Text, windows_authentication: false, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, null, null)
		{
			Param2 = LanguageType.TypeToString(LanguageType.GetTypeByIndex(GetLookupIndex(langaugeLookUpEdit))),
			Param3 = appIdTextEdit.Text,
			Param4 = redirectUriTextEdit.Text
		};
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		sqlServerLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			sqlServerLayoutControl.Root.AddItem(timeoutLayoutControlItem, timeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		return true & ValidateHost() & ValidateLogin() & ValidatePassword() & ValidateAppId() & ValidateRedirectUri();
	}

	private bool ValidateHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(hostComboBoxEdit, addDBErrorProvider, "Server name", acceptEmptyValue);
	}

	private bool ValidateLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(loginTextEdit, addDBErrorProvider, "user", acceptEmptyValue);
	}

	private bool ValidatePassword(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(passwordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateAppId(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(appIdTextEdit, addDBErrorProvider, "App Id", acceptEmptyValue);
	}

	private bool ValidateRedirectUri(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(redirectUriTextEdit, addDBErrorProvider, "Redirect Uri", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		hostComboBoxEdit.Text = base.DatabaseRow.Host;
		loginTextEdit.Text = base.DatabaseRow.User;
		passwordTextEdit.Text = value;
		appIdTextEdit.Text = base.DatabaseRow.Param3;
		redirectUriTextEdit.Text = base.DatabaseRow.Param4;
		savePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
		langaugeLookUpEdit.EditValue = LanguageType.GetIndexByType(LanguageType.StringToType(base.DatabaseRow.Param2));
	}

	protected override string GetPanelDocumentationTitle()
	{
		return providedHost;
	}

	private void dataverseLoginTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateLogin(acceptEmptyValue: true);
	}

	private void dataversePasswordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidatePassword(acceptEmptyValue: true);
	}

	private void dataverseHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateHost(acceptEmptyValue: true);
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void dataverseHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	private void ToolTipController_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		Links.OpenLink(e.Link);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		loginTextEdit.Text = string.Empty;
		passwordTextEdit.Text = string.Empty;
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
		DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem = new DevExpress.Utils.ToolTipItem();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.ConnectorsControls.DataverseConnectorControl));
		DevExpress.Utils.SuperToolTip superToolTip2 = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem2 = new DevExpress.Utils.ToolTipItem();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.sqlServerLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.pictureEdit2 = new DevExpress.XtraEditors.PictureEdit();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.savePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.loginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.passwordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.hostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.langaugeLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.appIdTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.redirectUriTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.passwordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.loginLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sqlServerSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem9 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.timeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.hostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.langaugeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.appIdLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.redirectUriLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.redirectImageLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.appIdPictureLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.sqlServerLayoutControl).BeginInit();
		this.sqlServerLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureEdit2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pictureEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.loginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.langaugeLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.appIdTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redirectUriTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.loginLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.timeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.langaugeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.appIdLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redirectUriLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redirectImageLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.appIdPictureLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		base.SuspendLayout();
		this.toolTipController.AllowHtmlText = true;
		this.toolTipController.KeepWhileHovered = true;
		this.toolTipController.Rounded = true;
		this.toolTipController.RoundRadius = 10;
		this.toolTipController.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(ToolTipController_HyperlinkClick);
		this.sqlServerLayoutControl.AllowCustomization = false;
		this.sqlServerLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.sqlServerLayoutControl.Controls.Add(this.pictureEdit2);
		this.sqlServerLayoutControl.Controls.Add(this.labelControl2);
		this.sqlServerLayoutControl.Controls.Add(this.pictureEdit1);
		this.sqlServerLayoutControl.Controls.Add(this.labelControl1);
		this.sqlServerLayoutControl.Controls.Add(this.savePasswordCheckEdit);
		this.sqlServerLayoutControl.Controls.Add(this.loginTextEdit);
		this.sqlServerLayoutControl.Controls.Add(this.passwordTextEdit);
		this.sqlServerLayoutControl.Controls.Add(this.hostComboBoxEdit);
		this.sqlServerLayoutControl.Controls.Add(this.langaugeLookUpEdit);
		this.sqlServerLayoutControl.Controls.Add(this.appIdTextEdit);
		this.sqlServerLayoutControl.Controls.Add(this.redirectUriTextEdit);
		this.sqlServerLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.sqlServerLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.sqlServerLayoutControl.Name = "sqlServerLayoutControl";
		this.sqlServerLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2719, 179, 279, 548);
		this.sqlServerLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.sqlServerLayoutControl.Root = this.layoutControlGroup4;
		this.sqlServerLayoutControl.Size = new System.Drawing.Size(499, 322);
		this.sqlServerLayoutControl.TabIndex = 3;
		this.sqlServerLayoutControl.Text = "layoutControl1";
		this.pictureEdit2.AllowHtmlTextInToolTip = DevExpress.Utils.DefaultBoolean.True;
		this.pictureEdit2.EditValue = Dataedo.App.Properties.Resources.question_16;
		this.pictureEdit2.Location = new System.Drawing.Point(38, 96);
		this.pictureEdit2.Name = "pictureEdit2";
		this.pictureEdit2.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.pictureEdit2.Properties.Appearance.Options.UseBackColor = true;
		this.pictureEdit2.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.pictureEdit2.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
		this.pictureEdit2.Size = new System.Drawing.Size(24, 24);
		this.pictureEdit2.StyleController = this.sqlServerLayoutControl;
		toolTipItem.Text = resources.GetString("toolTipItem1.Text");
		superToolTip.Items.Add(toolTipItem);
		this.pictureEdit2.SuperTip = superToolTip;
		this.pictureEdit2.TabIndex = 7;
		this.pictureEdit2.ToolTipController = this.toolTipController;
		this.labelControl2.Location = new System.Drawing.Point(0, 99);
		this.labelControl2.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(36, 19);
		this.labelControl2.StyleController = this.sqlServerLayoutControl;
		this.labelControl2.TabIndex = 6;
		this.labelControl2.Text = "App Id:";
		this.pictureEdit1.EditValue = Dataedo.App.Properties.Resources.question_16;
		this.pictureEdit1.Location = new System.Drawing.Point(63, 120);
		this.pictureEdit1.Name = "pictureEdit1";
		this.pictureEdit1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.pictureEdit1.Properties.Appearance.Options.UseBackColor = true;
		this.pictureEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.pictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
		this.pictureEdit1.Size = new System.Drawing.Size(24, 24);
		this.pictureEdit1.StyleController = this.sqlServerLayoutControl;
		toolTipItem2.Text = resources.GetString("toolTipItem2.Text");
		superToolTip2.Items.Add(toolTipItem2);
		this.pictureEdit1.SuperTip = superToolTip2;
		this.pictureEdit1.TabIndex = 5;
		this.pictureEdit1.ToolTipController = this.toolTipController;
		this.labelControl1.Location = new System.Drawing.Point(0, 126);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(61, 16);
		this.labelControl1.StyleController = this.sqlServerLayoutControl;
		this.labelControl1.TabIndex = 4;
		this.labelControl1.Text = "Redirect Uri:";
		this.savePasswordCheckEdit.Location = new System.Drawing.Point(105, 72);
		this.savePasswordCheckEdit.Name = "savePasswordCheckEdit";
		this.savePasswordCheckEdit.Properties.Caption = "Save password";
		this.savePasswordCheckEdit.Size = new System.Drawing.Size(230, 20);
		this.savePasswordCheckEdit.StyleController = this.sqlServerLayoutControl;
		this.savePasswordCheckEdit.TabIndex = 3;
		this.loginTextEdit.Location = new System.Drawing.Point(105, 24);
		this.loginTextEdit.Name = "loginTextEdit";
		this.loginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.loginTextEdit.StyleController = this.sqlServerLayoutControl;
		this.loginTextEdit.TabIndex = 1;
		this.loginTextEdit.EditValueChanged += new System.EventHandler(dataverseLoginTextEdit_EditValueChanged);
		this.passwordTextEdit.Location = new System.Drawing.Point(105, 48);
		this.passwordTextEdit.Name = "passwordTextEdit";
		this.passwordTextEdit.Properties.UseSystemPasswordChar = true;
		this.passwordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.passwordTextEdit.StyleController = this.sqlServerLayoutControl;
		this.passwordTextEdit.TabIndex = 2;
		this.passwordTextEdit.EditValueChanged += new System.EventHandler(dataversePasswordTextEdit_EditValueChanged);
		this.hostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.hostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2);
		this.hostComboBoxEdit.Name = "hostComboBoxEdit";
		this.hostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.hostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.hostComboBoxEdit.StyleController = this.sqlServerLayoutControl;
		this.hostComboBoxEdit.TabIndex = 1;
		this.hostComboBoxEdit.EditValueChanged += new System.EventHandler(dataverseHostComboBoxEdit_EditValueChanged);
		this.hostComboBoxEdit.Leave += new System.EventHandler(dataverseHostComboBoxEdit_Leave);
		this.langaugeLookUpEdit.Location = new System.Drawing.Point(105, 144);
		this.langaugeLookUpEdit.Name = "langaugeLookUpEdit";
		this.langaugeLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.langaugeLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("type", "Type")
		});
		this.langaugeLookUpEdit.Properties.DisplayMember = "type";
		this.langaugeLookUpEdit.Properties.NullText = "";
		this.langaugeLookUpEdit.Properties.ShowFooter = false;
		this.langaugeLookUpEdit.Properties.ShowHeader = false;
		this.langaugeLookUpEdit.Properties.ShowLines = false;
		this.langaugeLookUpEdit.Properties.ValueMember = "id";
		this.langaugeLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.langaugeLookUpEdit.StyleController = this.sqlServerLayoutControl;
		this.langaugeLookUpEdit.TabIndex = 0;
		this.appIdTextEdit.Location = new System.Drawing.Point(106, 96);
		this.appIdTextEdit.Name = "appIdTextEdit";
		this.appIdTextEdit.Size = new System.Drawing.Size(229, 20);
		this.appIdTextEdit.StyleController = this.sqlServerLayoutControl;
		this.appIdTextEdit.TabIndex = 1;
		this.redirectUriTextEdit.Location = new System.Drawing.Point(105, 120);
		this.redirectUriTextEdit.Name = "redirectUriTextEdit";
		this.redirectUriTextEdit.Size = new System.Drawing.Size(230, 20);
		this.redirectUriTextEdit.StyleController = this.sqlServerLayoutControl;
		this.redirectUriTextEdit.TabIndex = 1;
		this.layoutControlGroup4.CustomizationFormText = "Root";
		this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup4.GroupBordersVisible = false;
		this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[19]
		{
			this.passwordLayoutControlItem, this.loginLayoutControlItem, this.sqlServerSavePasswordLayoutControlItem, this.emptySpaceItem1, this.emptySpaceItem5, this.emptySpaceItem8, this.emptySpaceItem9, this.timeoutEmptySpaceItem, this.hostLayoutControlItem, this.emptySpaceItem3,
			this.langaugeLayoutControlItem, this.appIdLayoutControlItem, this.redirectUriLayoutControlItem, this.layoutControlItem1, this.redirectImageLayoutControlItem, this.emptySpaceItem2, this.layoutControlItem2, this.appIdPictureLayoutControlItem, this.emptySpaceItem4
		});
		this.layoutControlGroup4.Name = "Root";
		this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup4.Size = new System.Drawing.Size(499, 322);
		this.layoutControlGroup4.TextVisible = false;
		this.passwordLayoutControlItem.Control = this.passwordTextEdit;
		this.passwordLayoutControlItem.CustomizationFormText = "Password";
		this.passwordLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.passwordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.passwordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.passwordLayoutControlItem.Name = "passwordLayoutControlItem";
		this.passwordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.passwordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.passwordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.passwordLayoutControlItem.Text = "Password:";
		this.passwordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.passwordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.passwordLayoutControlItem.TextToControlDistance = 5;
		this.loginLayoutControlItem.Control = this.loginTextEdit;
		this.loginLayoutControlItem.CustomizationFormText = "User:";
		this.loginLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.loginLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.loginLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.loginLayoutControlItem.Name = "loginLayoutControlItem";
		this.loginLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.loginLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.loginLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.loginLayoutControlItem.Text = "User:";
		this.loginLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.loginLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.loginLayoutControlItem.TextToControlDistance = 5;
		this.sqlServerSavePasswordLayoutControlItem.Control = this.savePasswordCheckEdit;
		this.sqlServerSavePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem2";
		this.sqlServerSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.sqlServerSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerSavePasswordLayoutControlItem.Name = "sqlServerSavePasswordLayoutControlItem";
		this.sqlServerSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerSavePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sqlServerSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerSavePasswordLayoutControlItem.Text = " ";
		this.sqlServerSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem6";
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 286);
		this.emptySpaceItem1.Name = "emptySpaceItem6";
		this.emptySpaceItem1.Size = new System.Drawing.Size(499, 36);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.CustomizationFormText = "emptySpaceItem21";
		this.emptySpaceItem5.Location = new System.Drawing.Point(335, 24);
		this.emptySpaceItem5.Name = "emptySpaceItem21";
		this.emptySpaceItem5.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem8.AllowHotTrack = false;
		this.emptySpaceItem8.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem8.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem8.Name = "emptySpaceItem22";
		this.emptySpaceItem8.Size = new System.Drawing.Size(164, 36);
		this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem9.AllowHotTrack = false;
		this.emptySpaceItem9.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem9.Location = new System.Drawing.Point(335, 84);
		this.emptySpaceItem9.Name = "emptySpaceItem28";
		this.emptySpaceItem9.Size = new System.Drawing.Size(164, 36);
		this.emptySpaceItem9.Text = "emptySpaceItem22";
		this.emptySpaceItem9.TextSize = new System.Drawing.Size(0, 0);
		this.timeoutEmptySpaceItem.AllowHotTrack = false;
		this.timeoutEmptySpaceItem.CustomizationFormText = "emptySpaceItem12";
		this.timeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 168);
		this.timeoutEmptySpaceItem.Name = "timeoutEmptySpaceItem";
		this.timeoutEmptySpaceItem.Size = new System.Drawing.Size(499, 118);
		this.timeoutEmptySpaceItem.Text = "emptySpaceItem12";
		this.timeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.hostLayoutControlItem.Control = this.hostComboBoxEdit;
		this.hostLayoutControlItem.CustomizationFormText = "Environment URL:";
		this.hostLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.hostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.hostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.hostLayoutControlItem.Name = "hostLayoutControlItem";
		this.hostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.hostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.hostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.hostLayoutControlItem.Text = "Environment URL:";
		this.hostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.hostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.hostLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.langaugeLayoutControlItem.Control = this.langaugeLookUpEdit;
		this.langaugeLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.langaugeLayoutControlItem.CustomizationFormText = "Language:";
		this.langaugeLayoutControlItem.Location = new System.Drawing.Point(0, 144);
		this.langaugeLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.langaugeLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.langaugeLayoutControlItem.Name = "langaugeLayoutControlItem";
		this.langaugeLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.langaugeLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.langaugeLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.langaugeLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.langaugeLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.langaugeLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.langaugeLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.langaugeLayoutControlItem.Size = new System.Drawing.Size(499, 24);
		this.langaugeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.langaugeLayoutControlItem.Text = "Language:";
		this.langaugeLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.langaugeLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.langaugeLayoutControlItem.TextToControlDistance = 5;
		this.appIdLayoutControlItem.Control = this.appIdTextEdit;
		this.appIdLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.appIdLayoutControlItem.CustomizationFormText = "App Id";
		this.appIdLayoutControlItem.Location = new System.Drawing.Point(106, 96);
		this.appIdLayoutControlItem.MaxSize = new System.Drawing.Size(229, 24);
		this.appIdLayoutControlItem.MinSize = new System.Drawing.Size(229, 24);
		this.appIdLayoutControlItem.Name = "appIdLayoutControlItem";
		this.appIdLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.appIdLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.appIdLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.appIdLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.appIdLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.appIdLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.appIdLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.appIdLayoutControlItem.Size = new System.Drawing.Size(229, 24);
		this.appIdLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.appIdLayoutControlItem.Text = "App Id:";
		this.appIdLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.appIdLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.appIdLayoutControlItem.TextToControlDistance = 0;
		this.appIdLayoutControlItem.TextVisible = false;
		this.redirectUriLayoutControlItem.Control = this.redirectUriTextEdit;
		this.redirectUriLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.redirectUriLayoutControlItem.CustomizationFormText = "Redirect Uri";
		this.redirectUriLayoutControlItem.Location = new System.Drawing.Point(87, 120);
		this.redirectUriLayoutControlItem.MinSize = new System.Drawing.Size(155, 20);
		this.redirectUriLayoutControlItem.Name = "redirectUriLayoutControlItem";
		this.redirectUriLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.redirectUriLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.redirectUriLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.redirectUriLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.redirectUriLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.redirectUriLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.redirectUriLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.redirectUriLayoutControlItem.Size = new System.Drawing.Size(248, 24);
		this.redirectUriLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.redirectUriLayoutControlItem.Text = " ";
		this.redirectUriLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.redirectUriLayoutControlItem.TextSize = new System.Drawing.Size(13, 13);
		this.redirectUriLayoutControlItem.TextToControlDistance = 5;
		this.layoutControlItem1.Control = this.labelControl1;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 120);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(63, 24);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(63, 24);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 6, 2);
		this.layoutControlItem1.Size = new System.Drawing.Size(63, 24);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.redirectImageLayoutControlItem.Control = this.pictureEdit1;
		this.redirectImageLayoutControlItem.Location = new System.Drawing.Point(63, 120);
		this.redirectImageLayoutControlItem.MaxSize = new System.Drawing.Size(24, 24);
		this.redirectImageLayoutControlItem.MinSize = new System.Drawing.Size(24, 24);
		this.redirectImageLayoutControlItem.Name = "redirectImageLayoutControlItem";
		this.redirectImageLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.redirectImageLayoutControlItem.Size = new System.Drawing.Size(24, 24);
		this.redirectImageLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.redirectImageLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.redirectImageLayoutControlItem.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(335, 120);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(164, 24);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(164, 24);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.Control = this.labelControl2;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 96);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(38, 24);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(38, 24);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 3, 2);
		this.layoutControlItem2.Size = new System.Drawing.Size(38, 24);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.appIdPictureLayoutControlItem.Control = this.pictureEdit2;
		this.appIdPictureLayoutControlItem.Location = new System.Drawing.Point(38, 96);
		this.appIdPictureLayoutControlItem.MaxSize = new System.Drawing.Size(24, 24);
		this.appIdPictureLayoutControlItem.MinSize = new System.Drawing.Size(24, 24);
		this.appIdPictureLayoutControlItem.Name = "appIdPictureLayoutControlItem";
		this.appIdPictureLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.appIdPictureLayoutControlItem.Size = new System.Drawing.Size(24, 24);
		this.appIdPictureLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.appIdPictureLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.appIdPictureLayoutControlItem.TextVisible = false;
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(62, 96);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(44, 24);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(44, 24);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(44, 24);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.sqlServerLayoutControl);
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "DataverseConnectorControl";
		base.Size = new System.Drawing.Size(499, 322);
		((System.ComponentModel.ISupportInitialize)this.sqlServerLayoutControl).EndInit();
		this.sqlServerLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pictureEdit2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pictureEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.loginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.langaugeLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.appIdTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redirectUriTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.loginLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.timeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.langaugeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.appIdLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redirectUriLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redirectImageLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.appIdPictureLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		base.ResumeLayout(false);
	}
}
