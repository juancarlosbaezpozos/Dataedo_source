using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class NetSuiteConnectorControl : ConnectorControlBase
{
	private const string ServerDataSource = "NetSuite2.com";

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LabelControl portlabelControl;

	private CheckEdit savePasswordCheckEdit;

	private TextEdit loginTextEdit;

	private TextEdit passwordTextEdit;

	private ComboBoxEdit hostComboBoxEdit;

	private TextEdit portTextEdit;

	private LayoutControlGroup layoutControlGroup4;

	private LayoutControlItem passwordLayoutControlItem;

	private LayoutControlItem loginLayoutControlItem;

	private LayoutControlItem savePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem5;

	private EmptySpaceItem emptySpaceItem8;

	private EmptySpaceItem timeoutEmptySpaceItem;

	private LayoutControlItem hostLayoutControlItem;

	private EmptySpaceItem emptySpaceItem3;

	private EmptySpaceItem emptySpaceItem4;

	private LayoutControlItem portLayoutControlItem;

	private EmptySpaceItem emptySpaceItem57;

	private LayoutControlItem layoutControlItem16;

	private EmptySpaceItem emptySpaceItem60;

	private TextEdit accountIdTextEdit;

	private TextEdit roleIdTextEdit;

	private LayoutControlItem accountIdLayoutControlItem;

	private LayoutControlItem roleIdLayoutControlItem;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.NetSuite;

	protected override TextEdit HostTextEdit => hostComboBoxEdit;

	protected override TextEdit PortTextEdit => portTextEdit;

	protected override ComboBoxEdit HostComboBoxEdit => hostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => savePasswordCheckEdit;

	public NetSuiteConnectorControl()
	{
		InitializeComponent();
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? documentationTitle : base.DatabaseRow?.Name, documentationTitle, hostComboBoxEdit.Text, loginTextEdit.Text, passwordTextEdit.Text, windows_authentication: false, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, portTextEdit.Text, null)
		{
			Param1 = "NetSuite2.com",
			Param2 = accountIdTextEdit.Text,
			Param3 = roleIdTextEdit.Text
		};
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		mainLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			mainLayoutControl.Root.AddItem(timeoutLayoutControlItem, timeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		return true & ValidateHost() & ValidatePort() & ValidateLogin() & ValidatePassword() & ValidateAccountId() & ValidateRoleId();
	}

	private bool ValidateHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(hostComboBoxEdit, addDBErrorProvider, "server name", acceptEmptyValue);
	}

	private bool ValidatePort(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(portTextEdit, addDBErrorProvider, "port", acceptEmptyValue);
	}

	private bool ValidateLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(loginTextEdit, addDBErrorProvider, "user", acceptEmptyValue);
	}

	private bool ValidatePassword(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(passwordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateAccountId(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(accountIdTextEdit, addDBErrorProvider, "Account Id", acceptEmptyValue);
	}

	private bool ValidateRoleId(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(roleIdTextEdit, addDBErrorProvider, "Role Id", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		hostComboBoxEdit.Text = base.DatabaseRow.Host;
		loginTextEdit.Text = base.DatabaseRow.User;
		passwordTextEdit.Text = value;
		portTextEdit.Text = PrepareValue.ToString(base.DatabaseRow.Port) ?? DatabaseTypeEnum.GetDefaultPort(SharedDatabaseTypeEnum.DatabaseType.NetSuite);
		accountIdTextEdit.Text = base.DatabaseRow.Param2;
		roleIdTextEdit.Text = base.DatabaseRow.Param3;
		savePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return "NetSuite";
	}

	private void NetSuitePortTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidatePort(acceptEmptyValue: true);
		PortTextEdit_EditValueChanged(sender, e);
	}

	private void NetSuiteHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateHost(acceptEmptyValue: true);
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void LoginTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateLogin(acceptEmptyValue: true);
	}

	private void PasswordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidatePassword(acceptEmptyValue: true);
	}

	private void RoleIdTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateRoleId(acceptEmptyValue: true);
	}

	private void AccountIdTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateAccountId(acceptEmptyValue: true);
	}

	private void PortlabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			portTextEdit.Text = DatabaseTypeEnum.GetDefaultPort(SharedDatabaseTypeEnum.DatabaseType.NetSuite);
		}
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
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.portlabelControl = new DevExpress.XtraEditors.LabelControl();
		this.savePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.loginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.passwordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.hostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.portTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.passwordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.loginLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.savePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.timeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.hostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.portLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem57 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem60 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.roleIdTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.roleIdLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.accountIdLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.accountIdTextEdit = new DevExpress.XtraEditors.TextEdit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.savePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.loginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.portTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.loginLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.timeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.portLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem57).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem60).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.roleIdTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.roleIdLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.accountIdLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.accountIdTextEdit.Properties).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.portlabelControl);
		this.mainLayoutControl.Controls.Add(this.savePasswordCheckEdit);
		this.mainLayoutControl.Controls.Add(this.loginTextEdit);
		this.mainLayoutControl.Controls.Add(this.passwordTextEdit);
		this.mainLayoutControl.Controls.Add(this.hostComboBoxEdit);
		this.mainLayoutControl.Controls.Add(this.portTextEdit);
		this.mainLayoutControl.Controls.Add(this.accountIdTextEdit);
		this.mainLayoutControl.Controls.Add(this.roleIdTextEdit);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2719, 179, 279, 548);
		this.mainLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.mainLayoutControl.Root = this.layoutControlGroup4;
		this.mainLayoutControl.Size = new System.Drawing.Size(499, 322);
		this.mainLayoutControl.TabIndex = 3;
		this.mainLayoutControl.Text = "layoutControl1";
		this.portlabelControl.AllowHtmlString = true;
		this.portlabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.portlabelControl.Location = new System.Drawing.Point(347, 26);
		this.portlabelControl.Name = "portlabelControl";
		this.portlabelControl.Size = new System.Drawing.Size(32, 13);
		this.portlabelControl.StyleController = this.mainLayoutControl;
		this.portlabelControl.TabIndex = 28;
		this.portlabelControl.Text = "<href>default</href>";
		this.portlabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(PortlabelControl_HyperlinkClick);
		this.savePasswordCheckEdit.Location = new System.Drawing.Point(105, 96);
		this.savePasswordCheckEdit.Name = "savePasswordCheckEdit";
		this.savePasswordCheckEdit.Properties.Caption = "Save password";
		this.savePasswordCheckEdit.Size = new System.Drawing.Size(230, 20);
		this.savePasswordCheckEdit.StyleController = this.mainLayoutControl;
		this.savePasswordCheckEdit.TabIndex = 3;
		this.loginTextEdit.Location = new System.Drawing.Point(105, 48);
		this.loginTextEdit.Name = "loginTextEdit";
		this.loginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.loginTextEdit.StyleController = this.mainLayoutControl;
		this.loginTextEdit.TabIndex = 1;
		this.loginTextEdit.EditValueChanged += new System.EventHandler(LoginTextEdit_EditValueChanged);
		this.passwordTextEdit.Location = new System.Drawing.Point(105, 72);
		this.passwordTextEdit.Name = "passwordTextEdit";
		this.passwordTextEdit.Properties.UseSystemPasswordChar = true;
		this.passwordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.passwordTextEdit.StyleController = this.mainLayoutControl;
		this.passwordTextEdit.TabIndex = 2;
		this.passwordTextEdit.EditValueChanged += new System.EventHandler(PasswordTextEdit_EditValueChanged);
		this.hostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.hostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2);
		this.hostComboBoxEdit.Name = "hostComboBoxEdit";
		this.hostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.hostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.hostComboBoxEdit.StyleController = this.mainLayoutControl;
		this.hostComboBoxEdit.TabIndex = 1;
		this.hostComboBoxEdit.EditValueChanged += new System.EventHandler(NetSuiteHostComboBoxEdit_EditValueChanged);
		this.hostComboBoxEdit.Leave += new System.EventHandler(base.hostComboBoxEdit_Leave);
		this.portTextEdit.EditValue = "1708";
		this.portTextEdit.Location = new System.Drawing.Point(105, 24);
		this.portTextEdit.Margin = new System.Windows.Forms.Padding(2);
		this.portTextEdit.Name = "portTextEdit";
		this.portTextEdit.Properties.Mask.EditMask = "\\d+";
		this.portTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.portTextEdit.Properties.Mask.ShowPlaceHolders = false;
		this.portTextEdit.Properties.MaxLength = 5;
		this.portTextEdit.Size = new System.Drawing.Size(230, 20);
		this.portTextEdit.StyleController = this.mainLayoutControl;
		this.portTextEdit.TabIndex = 1;
		this.portTextEdit.EditValueChanged += new System.EventHandler(NetSuitePortTextEdit_EditValueChanged);
		this.portTextEdit.Leave += new System.EventHandler(base.PortTextEdit_Leave);
		this.layoutControlGroup4.CustomizationFormText = "Root";
		this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup4.GroupBordersVisible = false;
		this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[16]
		{
			this.passwordLayoutControlItem, this.loginLayoutControlItem, this.savePasswordLayoutControlItem, this.emptySpaceItem1, this.emptySpaceItem5, this.emptySpaceItem8, this.timeoutEmptySpaceItem, this.hostLayoutControlItem, this.emptySpaceItem3, this.emptySpaceItem4,
			this.portLayoutControlItem, this.emptySpaceItem57, this.layoutControlItem16, this.emptySpaceItem60, this.accountIdLayoutControlItem, this.roleIdLayoutControlItem
		});
		this.layoutControlGroup4.Name = "Root";
		this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup4.Size = new System.Drawing.Size(499, 322);
		this.layoutControlGroup4.TextVisible = false;
		this.passwordLayoutControlItem.Control = this.passwordTextEdit;
		this.passwordLayoutControlItem.CustomizationFormText = "Password";
		this.passwordLayoutControlItem.Location = new System.Drawing.Point(0, 72);
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
		this.loginLayoutControlItem.Location = new System.Drawing.Point(0, 48);
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
		this.savePasswordLayoutControlItem.Control = this.savePasswordCheckEdit;
		this.savePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem2";
		this.savePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.savePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.savePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.savePasswordLayoutControlItem.Name = "savePasswordLayoutControlItem";
		this.savePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.savePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.savePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.savePasswordLayoutControlItem.Text = " ";
		this.savePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.savePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.savePasswordLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem6";
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 196);
		this.emptySpaceItem1.Name = "emptySpaceItem6";
		this.emptySpaceItem1.Size = new System.Drawing.Size(499, 126);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.CustomizationFormText = "emptySpaceItem21";
		this.emptySpaceItem5.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem5.Name = "emptySpaceItem21";
		this.emptySpaceItem5.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem8.AllowHotTrack = false;
		this.emptySpaceItem8.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem8.Location = new System.Drawing.Point(335, 72);
		this.emptySpaceItem8.Name = "emptySpaceItem22";
		this.emptySpaceItem8.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
		this.timeoutEmptySpaceItem.AllowHotTrack = false;
		this.timeoutEmptySpaceItem.CustomizationFormText = "emptySpaceItem12";
		this.timeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 168);
		this.timeoutEmptySpaceItem.Name = "sqlServerTimeoutEmptySpaceItem";
		this.timeoutEmptySpaceItem.Size = new System.Drawing.Size(499, 28);
		this.timeoutEmptySpaceItem.Text = "emptySpaceItem12";
		this.timeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.hostLayoutControlItem.Control = this.hostComboBoxEdit;
		this.hostLayoutControlItem.CustomizationFormText = "Server name:";
		this.hostLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.hostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.hostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.hostLayoutControlItem.Name = "hostLayoutControlItem";
		this.hostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.hostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.hostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.hostLayoutControlItem.Text = "Server name:";
		this.hostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.hostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.hostLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(335, 96);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.portLayoutControlItem.Control = this.portTextEdit;
		this.portLayoutControlItem.CustomizationFormText = "Port:";
		this.portLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.portLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.portLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.portLayoutControlItem.Name = "portLayoutControlItem";
		this.portLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.portLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.portLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.portLayoutControlItem.Text = "Port:";
		this.portLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.portLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.portLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem57.AllowHotTrack = false;
		this.emptySpaceItem57.CustomizationFormText = "emptySpaceItem3";
		this.emptySpaceItem57.Location = new System.Drawing.Point(381, 24);
		this.emptySpaceItem57.Name = "emptySpaceItem57";
		this.emptySpaceItem57.Size = new System.Drawing.Size(118, 24);
		this.emptySpaceItem57.Text = "emptySpaceItem3";
		this.emptySpaceItem57.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem16.Control = this.portlabelControl;
		this.layoutControlItem16.Location = new System.Drawing.Point(345, 24);
		this.layoutControlItem16.Name = "layoutControlItem16";
		this.layoutControlItem16.Size = new System.Drawing.Size(36, 24);
		this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem16.TextVisible = false;
		this.emptySpaceItem60.AllowHotTrack = false;
		this.emptySpaceItem60.Location = new System.Drawing.Point(335, 24);
		this.emptySpaceItem60.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem60.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem60.Name = "emptySpaceItem60";
		this.emptySpaceItem60.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem60.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem60.TextSize = new System.Drawing.Size(0, 0);
		this.roleIdTextEdit.Location = new System.Drawing.Point(105, 144);
		this.roleIdTextEdit.Name = "roleIdTextEdit";
		this.roleIdTextEdit.Size = new System.Drawing.Size(230, 20);
		this.roleIdTextEdit.StyleController = this.mainLayoutControl;
		this.roleIdTextEdit.TabIndex = 1;
		this.roleIdTextEdit.EditValueChanged += new System.EventHandler(RoleIdTextEdit_EditValueChanged);
		this.roleIdLayoutControlItem.Control = this.roleIdTextEdit;
		this.roleIdLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.roleIdLayoutControlItem.CustomizationFormText = "Role Id:";
		this.roleIdLayoutControlItem.Location = new System.Drawing.Point(0, 144);
		this.roleIdLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.roleIdLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.roleIdLayoutControlItem.Name = "roleIdLayoutControlItem";
		this.roleIdLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.roleIdLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.roleIdLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.roleIdLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.roleIdLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.roleIdLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.roleIdLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.roleIdLayoutControlItem.Size = new System.Drawing.Size(499, 24);
		this.roleIdLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.roleIdLayoutControlItem.Text = "Role Id:";
		this.roleIdLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.roleIdLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.roleIdLayoutControlItem.TextToControlDistance = 5;
		this.accountIdLayoutControlItem.Control = this.accountIdTextEdit;
		this.accountIdLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.accountIdLayoutControlItem.CustomizationFormText = "Account Id:";
		this.accountIdLayoutControlItem.Location = new System.Drawing.Point(0, 120);
		this.accountIdLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.accountIdLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.accountIdLayoutControlItem.Name = "accountIdLayoutControlItem";
		this.accountIdLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.accountIdLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.accountIdLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.accountIdLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.accountIdLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.accountIdLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.accountIdLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.accountIdLayoutControlItem.Size = new System.Drawing.Size(499, 24);
		this.accountIdLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.accountIdLayoutControlItem.Text = "Account Id:";
		this.accountIdLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.accountIdLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.accountIdLayoutControlItem.TextToControlDistance = 5;
		this.accountIdTextEdit.Location = new System.Drawing.Point(105, 120);
		this.accountIdTextEdit.Name = "accountIdTextEdit";
		this.accountIdTextEdit.Size = new System.Drawing.Size(230, 20);
		this.accountIdTextEdit.StyleController = this.mainLayoutControl;
		this.accountIdTextEdit.TabIndex = 1;
		this.accountIdTextEdit.EditValueChanged += new System.EventHandler(AccountIdTextEdit_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Name = "NetSuiteConnectorControl";
		base.Size = new System.Drawing.Size(499, 322);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		this.mainLayoutControl.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.savePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.loginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.portTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.loginLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.timeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.portLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem57).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem60).EndInit();
		((System.ComponentModel.ISupportInitialize)this.roleIdTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.roleIdLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.accountIdLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.accountIdTextEdit.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
