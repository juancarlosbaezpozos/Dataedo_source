using System;
using System.ComponentModel;
using System.Drawing;
using System.Security.Principal;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
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

public class SQLServerConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl sqlServerLayoutControl;

	private LabelControl sqlServerPortlabelControl;

	private CheckEdit sqlServerSavePasswordCheckEdit;

	private TextEdit sqlServerLoginTextEdit;

	private TextEdit sqlServerPasswordTextEdit;

	private LookUpEdit sqlServerAuthenticationLookUpEdit;

	private ComboBoxEdit sqlServerHostComboBoxEdit;

	private TextEdit sqlServerPortTextEdit;

	private LayoutControlGroup layoutControlGroup4;

	private LayoutControlItem sqlServerPasswordLayoutControlItem;

	private LayoutControlItem sqlServerAuthenticationLayoutControlItem;

	private LayoutControlItem sqlServerLoginLayoutControlItem;

	private LayoutControlItem sqlServerSavePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem5;

	private EmptySpaceItem emptySpaceItem8;

	private EmptySpaceItem emptySpaceItem9;

	private LayoutControlItem sqlServerDatabaseLayoutControlItem;

	private EmptySpaceItem sqlServerTimeoutEmptySpaceItem;

	private LayoutControlItem sqlServerHostLayoutControlItem;

	private EmptySpaceItem emptySpaceItem2;

	private EmptySpaceItem emptySpaceItem3;

	private EmptySpaceItem emptySpaceItem4;

	private LayoutControlItem sqlServerPortLayoutControlItem;

	private EmptySpaceItem emptySpaceItem57;

	private LayoutControlItem layoutControlItem16;

	private EmptySpaceItem emptySpaceItem60;

	private LookUpEdit connectionModeLookUpEdit;

	private LayoutControlItem connectionModeLayoutControlItem;

	protected ButtonEdit sqlServerDatabaseButtonEdit;

	protected NonCustomizableLayoutControl layoutControl => sqlServerLayoutControl;

	protected LayoutControlItem databaseLayoutItem => sqlServerDatabaseLayoutControlItem;

	protected string providedSqlServerHost => splittedHost?.Host ?? sqlServerHostComboBoxEdit.Text;

	protected string providedSqlServerPort => sqlServerPortTextEdit.Text;

	protected string providedSQLServerDatabase => sqlServerDatabaseButtonEdit.Text;

	protected string providedSQLServerLogin => sqlServerLoginTextEdit.Text;

	protected string providedSQLServerPassword => sqlServerPasswordTextEdit.Text;

	protected LookUpEdit providedSqlServerAuthenticationLookUpEdit => sqlServerAuthenticationLookUpEdit;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.SqlServer;

	protected SqlServerConnectionModeEnum.SqlServerConnectionMode SqlServerConnectionMode => SqlServerConnectionModeEnum.StringToTypeOrDefault(connectionModeLookUpEdit.EditValue as string);

	protected override TextEdit HostTextEdit => sqlServerHostComboBoxEdit;

	protected override TextEdit PortTextEdit => sqlServerPortTextEdit;

	protected override ComboBoxEdit HostComboBoxEdit => sqlServerHostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => sqlServerSavePasswordCheckEdit;

	public SQLServerConnectorControl()
	{
		InitializeComponent();
	}

	protected override void SetAuthenticationDataSource()
	{
		Authentication.SetAuthenticationDataSource(sqlServerAuthenticationLookUpEdit);
		SetConnectionMode(null);
	}

	public override void SetSwitchDatabaseAvailability(bool isAvailable)
	{
		sqlServerDatabaseButtonEdit.Enabled = isAvailable;
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? sqlServerDatabaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedSqlServerHost, sqlServerLoginTextEdit.Text, sqlServerPasswordTextEdit.Text, Authentication.IsWindowsAuthentication(sqlServerAuthenticationLookUpEdit), base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, providedSqlServerPort, AuthenticationType.GetTypeByIndex(Authentication.GetIndex(sqlServerAuthenticationLookUpEdit)))
		{
			Param1 = SqlServerConnectionModeEnum.TypeToString(SqlServerConnectionMode)
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
			sqlServerLayoutControl.Root.AddItem(timeoutLayoutControlItem, sqlServerTimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateSqlServerHost();
		flag &= ValidateSqlServerAuthenticationType();
		if (!Authentication.IsWindowsAuthentication(sqlServerAuthenticationLookUpEdit) && !Authentication.IsActiveDirectoryInteractive(sqlServerAuthenticationLookUpEdit))
		{
			flag &= ValidateSqlServerLogin();
			flag &= ValidateSqlServerPassword();
		}
		if (!testForGettingDatabasesList)
		{
			flag &= ValidateSqlServerDatabase();
		}
		return flag & ValidatePort();
	}

	private bool ValidatePort(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(sqlServerPortTextEdit, addDBErrorProvider, "port", acceptEmptyValue);
	}

	private bool ValidateSqlServerHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(sqlServerHostComboBoxEdit, addDBErrorProvider, "Server name", acceptEmptyValue);
	}

	private bool ValidateSqlServerAuthenticationType(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(sqlServerAuthenticationLookUpEdit, addDBErrorProvider, "authentication type", acceptEmptyValue);
	}

	private bool ValidateSqlServerLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(sqlServerLoginTextEdit, addDBErrorProvider, "user", acceptEmptyValue);
	}

	private bool ValidateSqlServerPassword(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(sqlServerPasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateSqlServerDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(sqlServerDatabaseButtonEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		sqlServerHostComboBoxEdit.Text = base.DatabaseRow.Host;
		sqlServerDatabaseButtonEdit.EditValue = base.DatabaseRow.Name;
		sqlServerAuthenticationLookUpEdit.EditValue = AuthenticationType.GetLookupIndex(base.DatabaseRow.SelectedAuthenticationType);
		sqlServerLoginTextEdit.Text = base.DatabaseRow.User;
		sqlServerPasswordTextEdit.Text = value;
		sqlServerPortTextEdit.Text = PrepareValue.ToString(base.DatabaseRow.Port) ?? DatabaseTypeEnum.GetDefaultPort(SharedDatabaseTypeEnum.DatabaseType.SqlServer);
		sqlServerSavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
		SetConnectionMode(base.DatabaseRow.Param1);
	}

	protected void SetConnectionMode(string connectionMode)
	{
		Dataedo.App.Data.MetadataServer.SqlServerConnectionMode.SetSqlServerConnectionMode(connectionModeLookUpEdit);
		connectionModeLookUpEdit.EditValue = connectionMode ?? SqlServerConnectionModeEnum.DefaultModeString;
	}

	protected override string GetPanelDocumentationTitle()
	{
		return sqlServerDatabaseButtonEdit.Text + "@" + providedSqlServerHost;
	}

	private void authenticationLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		sqlServerDatabaseButtonEdit.Properties.Buttons[0].Visible = true;
		if (Authentication.IsWindowsAuthentication(sqlServerAuthenticationLookUpEdit) || Authentication.IsActiveDirectoryIntegrated(sqlServerAuthenticationLookUpEdit))
		{
			sqlServerLoginTextEdit.Text = WindowsIdentity.GetCurrent().Name;
			TextEdit textEdit = sqlServerLoginTextEdit;
			TextEdit textEdit2 = sqlServerPasswordTextEdit;
			bool flag2 = (sqlServerSavePasswordCheckEdit.Enabled = false);
			bool enabled = (textEdit2.Enabled = flag2);
			textEdit.Enabled = enabled;
			sqlServerPasswordTextEdit.Text = string.Empty;
		}
		else if (Authentication.IsActiveDirectoryInteractive(sqlServerAuthenticationLookUpEdit))
		{
			sqlServerLoginTextEdit.Enabled = true;
			TextEdit textEdit3 = sqlServerPasswordTextEdit;
			bool enabled = (sqlServerSavePasswordCheckEdit.Enabled = false);
			textEdit3.Enabled = enabled;
			sqlServerDatabaseButtonEdit.Properties.Buttons[0].Visible = false;
		}
		else
		{
			TextEdit textEdit4 = sqlServerLoginTextEdit;
			TextEdit textEdit5 = sqlServerPasswordTextEdit;
			bool flag2 = (sqlServerSavePasswordCheckEdit.Enabled = true);
			bool enabled = (textEdit5.Enabled = flag2);
			textEdit4.Enabled = enabled;
		}
		ValidateSqlServerAuthenticationType(acceptEmptyValue: true);
	}

	private void sqlServerAuthenticationLookUpEdit_EditValueChanging(object sender, ChangingEventArgs e)
	{
		AuthenticationType.AuthenticationTypeEnum? authenticationTypeEnum = ((e.NewValue == null) ? null : new AuthenticationType.AuthenticationTypeEnum?(AuthenticationType.GetTypeByIndex((int)e.NewValue)));
		AuthenticationType.AuthenticationTypeEnum? authenticationTypeEnum2 = ((e.OldValue == null) ? null : new AuthenticationType.AuthenticationTypeEnum?(AuthenticationType.GetTypeByIndex((int)e.OldValue)));
		if ((authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.WindowsAuthentication || authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryIntegrated) && (authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.StandardAuthentication || authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryPassword || authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryInteractive))
		{
			sqlServerLoginTextEdit.Text = lastProvidedLogin;
		}
		else if ((authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.WindowsAuthentication || authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryIntegrated) && (authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.StandardAuthentication || authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryPassword || authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryInteractive))
		{
			lastProvidedLogin = sqlServerLoginTextEdit.Text;
		}
	}

	private void sqlServerLoginTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSqlServerLogin(acceptEmptyValue: true);
	}

	private void sqlServerDatabaseButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSqlServerDatabase(acceptEmptyValue: true);
	}

	private void sqlServerPortlabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			sqlServerPortTextEdit.Text = DatabaseTypeEnum.GetDefaultPort(SharedDatabaseTypeEnum.DatabaseType.SqlServer);
		}
	}

	private void sqlServerPortTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		PortTextEdit_EditValueChanged(sender, e);
	}

	private void sqlServerPasswordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSqlServerPassword();
	}

	private void sqlServerDatabaseButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		DatabaseButtonEdit_ButtonClick(sender, e);
	}

	private void sqlServerHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void sqlServerHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	private void sqlServerPortTextEdit_Leave(object sender, EventArgs e)
	{
		PortTextEdit_Leave(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		sqlServerLoginTextEdit.Text = string.Empty;
		sqlServerPasswordTextEdit.Text = string.Empty;
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
		this.sqlServerLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.sqlServerPortlabelControl = new DevExpress.XtraEditors.LabelControl();
		this.sqlServerDatabaseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.sqlServerSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.sqlServerLoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.sqlServerPasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.sqlServerAuthenticationLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.sqlServerHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.sqlServerPortTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.connectionModeLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.sqlServerPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sqlServerAuthenticationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sqlServerLoginLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sqlServerSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem9 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.sqlServerDatabaseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sqlServerTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.sqlServerHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.sqlServerPortLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem57 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem60 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.connectionModeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.sqlServerLayoutControl).BeginInit();
		this.sqlServerLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.sqlServerDatabaseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerLoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerAuthenticationLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPortTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectionModeLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerAuthenticationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerLoginLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerDatabaseLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerTimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPortLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem57).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem60).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectionModeLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.sqlServerLayoutControl.AllowCustomization = false;
		this.sqlServerLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.sqlServerLayoutControl.Controls.Add(this.sqlServerPortlabelControl);
		this.sqlServerLayoutControl.Controls.Add(this.sqlServerDatabaseButtonEdit);
		this.sqlServerLayoutControl.Controls.Add(this.sqlServerSavePasswordCheckEdit);
		this.sqlServerLayoutControl.Controls.Add(this.sqlServerLoginTextEdit);
		this.sqlServerLayoutControl.Controls.Add(this.sqlServerPasswordTextEdit);
		this.sqlServerLayoutControl.Controls.Add(this.sqlServerAuthenticationLookUpEdit);
		this.sqlServerLayoutControl.Controls.Add(this.sqlServerHostComboBoxEdit);
		this.sqlServerLayoutControl.Controls.Add(this.sqlServerPortTextEdit);
		this.sqlServerLayoutControl.Controls.Add(this.connectionModeLookUpEdit);
		this.sqlServerLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.sqlServerLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.sqlServerLayoutControl.Name = "sqlServerLayoutControl";
		this.sqlServerLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2719, 179, 279, 548);
		this.sqlServerLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.sqlServerLayoutControl.Root = this.layoutControlGroup4;
		this.sqlServerLayoutControl.Size = new System.Drawing.Size(499, 322);
		this.sqlServerLayoutControl.TabIndex = 3;
		this.sqlServerLayoutControl.Text = "layoutControl1";
		this.sqlServerPortlabelControl.AllowHtmlString = true;
		this.sqlServerPortlabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.sqlServerPortlabelControl.Location = new System.Drawing.Point(347, 26);
		this.sqlServerPortlabelControl.Name = "sqlServerPortlabelControl";
		this.sqlServerPortlabelControl.Size = new System.Drawing.Size(32, 13);
		this.sqlServerPortlabelControl.StyleController = this.sqlServerLayoutControl;
		this.sqlServerPortlabelControl.TabIndex = 28;
		this.sqlServerPortlabelControl.Text = "<href>default</href>";
		this.sqlServerPortlabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(sqlServerPortlabelControl_HyperlinkClick);
		this.sqlServerDatabaseButtonEdit.Location = new System.Drawing.Point(105, 168);
		this.sqlServerDatabaseButtonEdit.Name = "sqlServerDatabaseButtonEdit";
		this.sqlServerDatabaseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.sqlServerDatabaseButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.sqlServerDatabaseButtonEdit.StyleController = this.sqlServerLayoutControl;
		this.sqlServerDatabaseButtonEdit.TabIndex = 4;
		this.sqlServerDatabaseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(sqlServerDatabaseButtonEdit_ButtonClick);
		this.sqlServerDatabaseButtonEdit.EditValueChanged += new System.EventHandler(sqlServerDatabaseButtonEdit_EditValueChanged);
		this.sqlServerSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 120);
		this.sqlServerSavePasswordCheckEdit.Name = "sqlServerSavePasswordCheckEdit";
		this.sqlServerSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.sqlServerSavePasswordCheckEdit.Size = new System.Drawing.Size(230, 20);
		this.sqlServerSavePasswordCheckEdit.StyleController = this.sqlServerLayoutControl;
		this.sqlServerSavePasswordCheckEdit.TabIndex = 3;
		this.sqlServerLoginTextEdit.Location = new System.Drawing.Point(105, 72);
		this.sqlServerLoginTextEdit.Name = "sqlServerLoginTextEdit";
		this.sqlServerLoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.sqlServerLoginTextEdit.StyleController = this.sqlServerLayoutControl;
		this.sqlServerLoginTextEdit.TabIndex = 1;
		this.sqlServerLoginTextEdit.EditValueChanged += new System.EventHandler(sqlServerLoginTextEdit_EditValueChanged);
		this.sqlServerPasswordTextEdit.Location = new System.Drawing.Point(105, 96);
		this.sqlServerPasswordTextEdit.Name = "sqlServerPasswordTextEdit";
		this.sqlServerPasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.sqlServerPasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.sqlServerPasswordTextEdit.StyleController = this.sqlServerLayoutControl;
		this.sqlServerPasswordTextEdit.TabIndex = 2;
		this.sqlServerPasswordTextEdit.EditValueChanged += new System.EventHandler(sqlServerPasswordTextEdit_EditValueChanged);
		this.sqlServerAuthenticationLookUpEdit.Location = new System.Drawing.Point(105, 48);
		this.sqlServerAuthenticationLookUpEdit.Name = "sqlServerAuthenticationLookUpEdit";
		this.sqlServerAuthenticationLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.sqlServerAuthenticationLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("type", "Type")
		});
		this.sqlServerAuthenticationLookUpEdit.Properties.DisplayMember = "type";
		this.sqlServerAuthenticationLookUpEdit.Properties.NullText = "";
		this.sqlServerAuthenticationLookUpEdit.Properties.ShowFooter = false;
		this.sqlServerAuthenticationLookUpEdit.Properties.ShowHeader = false;
		this.sqlServerAuthenticationLookUpEdit.Properties.ShowLines = false;
		this.sqlServerAuthenticationLookUpEdit.Properties.ValueMember = "id";
		this.sqlServerAuthenticationLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.sqlServerAuthenticationLookUpEdit.StyleController = this.sqlServerLayoutControl;
		this.sqlServerAuthenticationLookUpEdit.TabIndex = 0;
		this.sqlServerAuthenticationLookUpEdit.EditValueChanged += new System.EventHandler(authenticationLookUpEdit_EditValueChanged);
		this.sqlServerAuthenticationLookUpEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(sqlServerAuthenticationLookUpEdit_EditValueChanging);
		this.sqlServerHostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.sqlServerHostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2);
		this.sqlServerHostComboBoxEdit.Name = "sqlServerHostComboBoxEdit";
		this.sqlServerHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.sqlServerHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.sqlServerHostComboBoxEdit.StyleController = this.sqlServerLayoutControl;
		this.sqlServerHostComboBoxEdit.TabIndex = 1;
		this.sqlServerHostComboBoxEdit.EditValueChanged += new System.EventHandler(sqlServerHostComboBoxEdit_EditValueChanged);
		this.sqlServerHostComboBoxEdit.Leave += new System.EventHandler(sqlServerHostComboBoxEdit_Leave);
		this.sqlServerPortTextEdit.EditValue = "1433";
		this.sqlServerPortTextEdit.Location = new System.Drawing.Point(105, 24);
		this.sqlServerPortTextEdit.Margin = new System.Windows.Forms.Padding(2);
		this.sqlServerPortTextEdit.Name = "sqlServerPortTextEdit";
		this.sqlServerPortTextEdit.Properties.Mask.EditMask = "\\d+";
		this.sqlServerPortTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.sqlServerPortTextEdit.Properties.Mask.ShowPlaceHolders = false;
		this.sqlServerPortTextEdit.Properties.MaxLength = 5;
		this.sqlServerPortTextEdit.Size = new System.Drawing.Size(230, 20);
		this.sqlServerPortTextEdit.StyleController = this.sqlServerLayoutControl;
		this.sqlServerPortTextEdit.TabIndex = 1;
		this.sqlServerPortTextEdit.EditValueChanged += new System.EventHandler(sqlServerPortTextEdit_EditValueChanged);
		this.sqlServerPortTextEdit.Leave += new System.EventHandler(sqlServerPortTextEdit_Leave);
		this.connectionModeLookUpEdit.Location = new System.Drawing.Point(105, 144);
		this.connectionModeLookUpEdit.Name = "connectionModeLookUpEdit";
		this.connectionModeLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.connectionModeLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("name", "Name")
		});
		this.connectionModeLookUpEdit.Properties.DisplayMember = "name";
		this.connectionModeLookUpEdit.Properties.NullText = "";
		this.connectionModeLookUpEdit.Properties.ShowFooter = false;
		this.connectionModeLookUpEdit.Properties.ShowHeader = false;
		this.connectionModeLookUpEdit.Properties.ShowLines = false;
		this.connectionModeLookUpEdit.Properties.ValueMember = "id";
		this.connectionModeLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.connectionModeLookUpEdit.StyleController = this.sqlServerLayoutControl;
		this.connectionModeLookUpEdit.TabIndex = 0;
		this.layoutControlGroup4.CustomizationFormText = "Root";
		this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup4.GroupBordersVisible = false;
		this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[19]
		{
			this.sqlServerPasswordLayoutControlItem, this.sqlServerAuthenticationLayoutControlItem, this.sqlServerLoginLayoutControlItem, this.sqlServerSavePasswordLayoutControlItem, this.emptySpaceItem1, this.emptySpaceItem5, this.emptySpaceItem8, this.emptySpaceItem9, this.sqlServerDatabaseLayoutControlItem, this.sqlServerTimeoutEmptySpaceItem,
			this.sqlServerHostLayoutControlItem, this.emptySpaceItem2, this.emptySpaceItem3, this.emptySpaceItem4, this.sqlServerPortLayoutControlItem, this.emptySpaceItem57, this.layoutControlItem16, this.emptySpaceItem60, this.connectionModeLayoutControlItem
		});
		this.layoutControlGroup4.Name = "Root";
		this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup4.Size = new System.Drawing.Size(499, 322);
		this.layoutControlGroup4.TextVisible = false;
		this.sqlServerPasswordLayoutControlItem.Control = this.sqlServerPasswordTextEdit;
		this.sqlServerPasswordLayoutControlItem.CustomizationFormText = "Password";
		this.sqlServerPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.sqlServerPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerPasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerPasswordLayoutControlItem.Name = "sqlServerPasswordLayoutControlItem";
		this.sqlServerPasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerPasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sqlServerPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerPasswordLayoutControlItem.Text = "Password:";
		this.sqlServerPasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerPasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerPasswordLayoutControlItem.TextToControlDistance = 5;
		this.sqlServerAuthenticationLayoutControlItem.Control = this.sqlServerAuthenticationLookUpEdit;
		this.sqlServerAuthenticationLayoutControlItem.CustomizationFormText = "Authentication:";
		this.sqlServerAuthenticationLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.sqlServerAuthenticationLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerAuthenticationLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerAuthenticationLayoutControlItem.Name = "sqlServerAuthenticationLayoutControlItem";
		this.sqlServerAuthenticationLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerAuthenticationLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sqlServerAuthenticationLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerAuthenticationLayoutControlItem.Text = "Authentication:";
		this.sqlServerAuthenticationLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerAuthenticationLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerAuthenticationLayoutControlItem.TextToControlDistance = 5;
		this.sqlServerLoginLayoutControlItem.Control = this.sqlServerLoginTextEdit;
		this.sqlServerLoginLayoutControlItem.CustomizationFormText = "User:";
		this.sqlServerLoginLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.sqlServerLoginLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerLoginLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerLoginLayoutControlItem.Name = "sqlServerLoginLayoutControlItem";
		this.sqlServerLoginLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerLoginLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sqlServerLoginLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerLoginLayoutControlItem.Text = "User:";
		this.sqlServerLoginLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerLoginLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerLoginLayoutControlItem.TextToControlDistance = 5;
		this.sqlServerSavePasswordLayoutControlItem.Control = this.sqlServerSavePasswordCheckEdit;
		this.sqlServerSavePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem2";
		this.sqlServerSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 120);
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
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 229);
		this.emptySpaceItem1.Name = "emptySpaceItem6";
		this.emptySpaceItem1.Size = new System.Drawing.Size(499, 93);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.CustomizationFormText = "emptySpaceItem21";
		this.emptySpaceItem5.Location = new System.Drawing.Point(335, 72);
		this.emptySpaceItem5.Name = "emptySpaceItem21";
		this.emptySpaceItem5.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem8.AllowHotTrack = false;
		this.emptySpaceItem8.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem8.Location = new System.Drawing.Point(335, 96);
		this.emptySpaceItem8.Name = "emptySpaceItem22";
		this.emptySpaceItem8.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem9.AllowHotTrack = false;
		this.emptySpaceItem9.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem9.Location = new System.Drawing.Point(335, 135);
		this.emptySpaceItem9.Name = "emptySpaceItem28";
		this.emptySpaceItem9.Size = new System.Drawing.Size(164, 57);
		this.emptySpaceItem9.Text = "emptySpaceItem22";
		this.emptySpaceItem9.TextSize = new System.Drawing.Size(0, 0);
		this.sqlServerDatabaseLayoutControlItem.Control = this.sqlServerDatabaseButtonEdit;
		this.sqlServerDatabaseLayoutControlItem.Location = new System.Drawing.Point(0, 168);
		this.sqlServerDatabaseLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerDatabaseLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerDatabaseLayoutControlItem.Name = "sqlServerDatabaseLayoutControlItem";
		this.sqlServerDatabaseLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerDatabaseLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sqlServerDatabaseLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerDatabaseLayoutControlItem.Text = "Database:";
		this.sqlServerDatabaseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerDatabaseLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerDatabaseLayoutControlItem.TextToControlDistance = 5;
		this.sqlServerTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.sqlServerTimeoutEmptySpaceItem.CustomizationFormText = "emptySpaceItem12";
		this.sqlServerTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 192);
		this.sqlServerTimeoutEmptySpaceItem.Name = "sqlServerTimeoutEmptySpaceItem";
		this.sqlServerTimeoutEmptySpaceItem.Size = new System.Drawing.Size(499, 37);
		this.sqlServerTimeoutEmptySpaceItem.Text = "emptySpaceItem12";
		this.sqlServerTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.sqlServerHostLayoutControlItem.Control = this.sqlServerHostComboBoxEdit;
		this.sqlServerHostLayoutControlItem.CustomizationFormText = "Server name:";
		this.sqlServerHostLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.sqlServerHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerHostLayoutControlItem.Name = "sqlServerHostLayoutControlItem";
		this.sqlServerHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sqlServerHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerHostLayoutControlItem.Text = "Server name:";
		this.sqlServerHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerHostLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(335, 120);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(164, 15);
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.sqlServerPortLayoutControlItem.Control = this.sqlServerPortTextEdit;
		this.sqlServerPortLayoutControlItem.CustomizationFormText = "Port:";
		this.sqlServerPortLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.sqlServerPortLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerPortLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerPortLayoutControlItem.Name = "sqlServerPortLayoutControlItem";
		this.sqlServerPortLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerPortLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sqlServerPortLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerPortLayoutControlItem.Text = "Port:";
		this.sqlServerPortLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerPortLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerPortLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem57.AllowHotTrack = false;
		this.emptySpaceItem57.CustomizationFormText = "emptySpaceItem3";
		this.emptySpaceItem57.Location = new System.Drawing.Point(381, 24);
		this.emptySpaceItem57.Name = "emptySpaceItem57";
		this.emptySpaceItem57.Size = new System.Drawing.Size(118, 24);
		this.emptySpaceItem57.Text = "emptySpaceItem3";
		this.emptySpaceItem57.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem16.Control = this.sqlServerPortlabelControl;
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
		this.connectionModeLayoutControlItem.Control = this.connectionModeLookUpEdit;
		this.connectionModeLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.connectionModeLayoutControlItem.CustomizationFormText = "Connection mode:";
		this.connectionModeLayoutControlItem.Location = new System.Drawing.Point(0, 144);
		this.connectionModeLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.connectionModeLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.connectionModeLayoutControlItem.Name = "connectionModeLayoutControlItem";
		this.connectionModeLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.connectionModeLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.connectionModeLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.connectionModeLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.connectionModeLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.connectionModeLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.connectionModeLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.connectionModeLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.connectionModeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.connectionModeLayoutControlItem.Text = "Connection mode:";
		this.connectionModeLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.connectionModeLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.connectionModeLayoutControlItem.TextToControlDistance = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.sqlServerLayoutControl);
		base.Name = "SQLServerConnectorControl";
		base.Size = new System.Drawing.Size(499, 322);
		((System.ComponentModel.ISupportInitialize)this.sqlServerLayoutControl).EndInit();
		this.sqlServerLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.sqlServerDatabaseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerLoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerAuthenticationLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPortTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectionModeLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerAuthenticationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerLoginLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerDatabaseLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerTimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPortLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem57).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem60).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectionModeLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
