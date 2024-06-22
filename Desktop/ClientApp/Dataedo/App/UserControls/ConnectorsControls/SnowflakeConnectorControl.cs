using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Odbc;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.StaticData;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Helpers.Extensions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class SnowflakeConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl snowflakeLayoutControl;

	private ButtonEdit snowflakeDatabaseButtonEdit;

	private CheckEdit snowflakeSavePasswordCheckEdit;

	private TextEdit snowflakeLoginTextEdit;

	private TextEdit snowflakePasswordTextEdit;

	private ButtonEdit snowflakeWarehouseButtonEdit;

	private ComboBoxEdit snowflakeHostComboBoxEdit;

	private LookUpEdit snowflakeAuthenticationLookUpEdit;

	private ButtonEdit snowflakePrivateKeyButtonEdit;

	private ButtonEdit snowflakeRoleButtonEdit;

	private LayoutControlGroup snowflakeLayoutControlGroup;

	private LayoutControlItem snowflakePasswordLayoutControlItem;

	private LayoutControlItem snowflakeLoginLayoutControlItem;

	private LayoutControlItem snowflakeSavePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem41;

	private EmptySpaceItem emptySpaceItem43;

	private EmptySpaceItem emptySpaceItem44;

	private LayoutControlItem snowflakeDatabaseLayoutControlItem;

	private EmptySpaceItem snowflakeTimeoutEmptySpaceItem;

	private LayoutControlItem snowflakeWarehouseLayoutControlItem;

	private LayoutControlItem snowFlakeHostLayoutControlItem;

	private EmptySpaceItem emptySpaceItem53;

	private EmptySpaceItem emptySpaceItem64;

	private EmptySpaceItem emptySpaceItem65;

	private LayoutControlItem layoutControlItem23;

	private LayoutControlItem snowflakePrivateKeyRow;

	private LayoutControlItem snowflakeRoleLayoutControlItem1;

	private string ProvidedSnowflakeHost => splittedHost?.Host ?? snowflakeHostComboBoxEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Snowflake;

	protected override ComboBoxEdit HostComboBoxEdit => snowflakeHostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => snowflakeSavePasswordCheckEdit;

	public SnowflakeConnectorControl()
	{
		InitializeComponent();
	}

	protected override void SetAuthenticationDataSource()
	{
		SnowflakeAuth.SetAuthenticationDataSource(snowflakeAuthenticationLookUpEdit);
	}

	public override void SetSwitchDatabaseAvailability(bool isAvailable)
	{
		snowflakeDatabaseButtonEdit.Enabled = isAvailable;
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? snowflakeDatabaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, ProvidedSnowflakeHost, snowflakeLoginTextEdit.Text, snowflakePasswordTextEdit.Text, null, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, sslType: SnowflakeAuthMethod.TypeToString(SnowflakeAuthMethod.GetTypeByIndex((int)snowflakeAuthenticationLookUpEdit.EditValue)), sslSettings: new SSLSettings
		{
			KeyPath = (string)snowflakePrivateKeyButtonEdit.EditValue
		}, connectionRole: snowflakeRoleButtonEdit.Text, serviceName: snowflakeWarehouseButtonEdit.Text);
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		snowflakeLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			snowflakeLayoutControl.Root.AddItem(timeoutLayoutControlItem, snowflakeTimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateSnowflakeHost();
		flag &= ValidateSnowflakePrivateKey();
		flag &= ValidateSnowflakeLogin();
		flag &= ValidateSnowflakePassword();
		if (!testForGettingWarehousesList)
		{
			flag &= ValidateSnowflakeWarehouse();
		}
		if (!testForGettingDatabasesList && !testForGettingWarehousesList)
		{
			flag &= ValidateSnowflakeDatabase();
		}
		return flag;
	}

	private bool ValidateSnowflakeHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(snowflakeHostComboBoxEdit, addDBErrorProvider, "host", acceptEmptyValue);
	}

	private bool ValidateSnowflakeLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(snowflakeLoginTextEdit, addDBErrorProvider, "user", acceptEmptyValue);
	}

	private bool ValidateSnowflakePassword(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(snowflakePasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateSnowflakePrivateKey(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(snowflakePrivateKeyButtonEdit, addDBErrorProvider, "privateKey", acceptEmptyValue);
	}

	private bool ValidateSnowflakeDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(snowflakeDatabaseButtonEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	private bool ValidateSnowflakeWarehouse(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(snowflakeWarehouseButtonEdit, addDBErrorProvider, "warehouse", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		snowflakeHostComboBoxEdit.Text = base.DatabaseRow.Host;
		snowflakeAuthenticationLookUpEdit.EditValue = SnowflakeAuthMethod.GetLookupIndex(SnowflakeAuthMethod.StringToType(base.DatabaseRow.SSLType));
		snowflakeDatabaseButtonEdit.EditValue = base.DatabaseRow.Name;
		snowflakeWarehouseButtonEdit.EditValue = base.DatabaseRow.ServiceName;
		snowflakeRoleButtonEdit.EditValue = base.DatabaseRow.ConnectionRole;
		snowflakeLoginTextEdit.Text = base.DatabaseRow.User;
		snowflakePasswordTextEdit.Text = base.DatabaseRow.Password;
		snowflakePrivateKeyButtonEdit.EditValue = base.DatabaseRow.SSLSettings?.KeyPath;
		snowflakeSavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return snowflakeDatabaseButtonEdit.Text + "@" + ProvidedSnowflakeHost;
	}

	private void snowflakeLoginTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSnowflakeLogin(acceptEmptyValue: true);
	}

	private void snowflakePasswordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSnowflakePassword();
	}

	private void snowflakeDatabaseButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSnowflakeDatabase(acceptEmptyValue: true);
	}

	private void snowflakeWarehouseButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSnowflakeWarehouse(acceptEmptyValue: true);
	}

	private void SnowflakeDatabaseSplashScreenManager_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		if (!ValidateRequiredFields(testForGettingDatabasesList: true))
		{
			GeneralMessageBoxesHandling.Show("Required fields are not filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
			return;
		}
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
		SetNewDBRowValues(forGettingDatabasesList: true);
		ConnectionResult connectionResult = base.DatabaseRow.TryConnection();
		base.DatabaseRow.CloseConnection();
		if (connectionResult.Exception != null)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
			if (connectionResult.Exception is OdbcException ex && ex.Errors[0].SQLState.Equals("IM002"))
			{
				string text = (Environment.Is64BitProcess ? "32" : "64");
				GeneralMessageBoxesHandling.Show("Data source name not found and no default driver specified.\nInstall Snowflake ODBC driver, see <href=https://docs.snowflake.net/manuals/user-guide/odbc-download.html>official Snowflake guide</href> for more information.\nIf you have installed the driver, try running Dataedo in " + text + "-bit mode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
			}
			else
			{
				GeneralMessageBoxesHandling.Show(connectionResult.Exception.GetInnerExceptionsMessages(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			return;
		}
		List<string> databases = base.DatabaseRow.GetDatabases(base.SplashScreenManager);
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
		if (databases != null)
		{
			base.DatabaseRow.Name = string.Empty;
			ListForm listForm = new ListForm(databases, "Databases list");
			if (listForm.ShowDialog(this, setCustomMessageDefaultOwner: true) == DialogResult.OK)
			{
				(sender as ButtonEdit).EditValue = listForm.SelectedValue;
			}
		}
	}

	private void OpenWarehouseSplashScreenManager_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		if (!ValidateRequiredFields(testForGettingDatabasesList: false, testForGettingWarehousesList: true))
		{
			GeneralMessageBoxesHandling.Show("Required fields are not filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
			return;
		}
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
		SetNewDBRowValues(forGettingDatabasesList: true);
		ConnectionResult connectionResult = base.DatabaseRow.TryConnection(useOnlyRequiredFields: true);
		if (connectionResult.Exception != null)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
			GeneralMessageBoxesHandling.Show(connectionResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
			return;
		}
		List<string> warehouses = base.DatabaseRow.GetWarehouses(base.SplashScreenManager, FindForm());
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
		if (warehouses != null)
		{
			string title = "Warehouses list";
			ListForm listForm = new ListForm(warehouses, title);
			if (listForm.ShowDialog(this, setCustomMessageDefaultOwner: true) == DialogResult.OK)
			{
				(sender as ButtonEdit).EditValue = listForm.SelectedValue;
			}
		}
	}

	private void snowflakeAuthLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		switch (SnowflakeAuthMethod.GetTypeByIndex((int)snowflakeAuthenticationLookUpEdit.EditValue))
		{
		case SnowflakeAuthMethod.SnowflakeAuthMethodEnum.LoginPassword:
		{
			LayoutControlItem layoutControlItem5 = snowflakePasswordLayoutControlItem;
			EmptySpaceItem emptySpaceItem4 = emptySpaceItem44;
			LayoutControlItem layoutControlItem6 = snowflakeSavePasswordLayoutControlItem;
			LayoutVisibility layoutVisibility2 = (emptySpaceItem64.Visibility = LayoutVisibility.Always);
			LayoutVisibility layoutVisibility4 = (layoutControlItem6.Visibility = layoutVisibility2);
			LayoutVisibility layoutVisibility7 = (layoutControlItem5.Visibility = (emptySpaceItem4.Visibility = layoutVisibility4));
			snowflakePrivateKeyRow.Visibility = LayoutVisibility.Never;
			snowflakePasswordLayoutControlItem.Text = "Password:";
			break;
		}
		case SnowflakeAuthMethod.SnowflakeAuthMethodEnum.SSO_ExternalBrowser:
		{
			LayoutControlItem layoutControlItem3 = snowflakePasswordLayoutControlItem;
			EmptySpaceItem emptySpaceItem2 = emptySpaceItem44;
			LayoutControlItem layoutControlItem4 = snowflakeSavePasswordLayoutControlItem;
			EmptySpaceItem emptySpaceItem3 = emptySpaceItem64;
			LayoutVisibility layoutVisibility9 = (snowflakePrivateKeyRow.Visibility = LayoutVisibility.Never);
			LayoutVisibility layoutVisibility2 = (emptySpaceItem3.Visibility = layoutVisibility9);
			LayoutVisibility layoutVisibility4 = (layoutControlItem4.Visibility = layoutVisibility2);
			LayoutVisibility layoutVisibility7 = (layoutControlItem3.Visibility = (emptySpaceItem2.Visibility = layoutVisibility4));
			break;
		}
		case SnowflakeAuthMethod.SnowflakeAuthMethodEnum.JWT_PrivateKey:
		{
			LayoutControlItem layoutControlItem = snowflakePasswordLayoutControlItem;
			EmptySpaceItem emptySpaceItem = emptySpaceItem44;
			LayoutControlItem layoutControlItem2 = snowflakeSavePasswordLayoutControlItem;
			LayoutVisibility layoutVisibility2 = (emptySpaceItem64.Visibility = LayoutVisibility.Always);
			LayoutVisibility layoutVisibility4 = (layoutControlItem2.Visibility = layoutVisibility2);
			LayoutVisibility layoutVisibility7 = (layoutControlItem.Visibility = (emptySpaceItem.Visibility = layoutVisibility4));
			snowflakePrivateKeyRow.Visibility = LayoutVisibility.Always;
			snowflakePasswordLayoutControlItem.Text = "Pass phrase:";
			break;
		}
		}
	}

	private void SelectPrivateKeyFile_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "Private key (*.p8;*.key;*.pkey)|*.p8;*.key;*.pkey|All files (*.*)|*.*";
		openFileDialog.RestoreDirectory = true;
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			string fileName = openFileDialog.FileName;
			snowflakePrivateKeyButtonEdit.EditValue = fileName;
		}
	}

	private void SnowflakeHostValueChanged(object sender, EventArgs e)
	{
		AutofixSnowflakeHostFieldValue();
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void OpenSnowflakeRoleManager_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		if (!ValidateRequiredFields(testForGettingDatabasesList: false, testForGettingWarehousesList: true))
		{
			GeneralMessageBoxesHandling.Show("Required fields are not filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
		SetNewDBRowValues(forGettingDatabasesList: true);
		ConnectionResult connectionResult = base.DatabaseRow.TryConnection(useOnlyRequiredFields: true);
		if (connectionResult.Exception != null)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
			GeneralMessageBoxesHandling.Show(connectionResult.Exception.GetInnerExceptionsMessages(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		List<string> roles = base.DatabaseRow.GetRoles(base.SplashScreenManager);
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
		if (roles != null)
		{
			string title = "Roles list";
			ListForm listForm = new ListForm(roles, title);
			if (listForm.ShowDialog(this, setCustomMessageDefaultOwner: true) == DialogResult.OK)
			{
				(sender as ButtonEdit).EditValue = listForm.SelectedValue;
			}
		}
	}

	private void snowflakeHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		snowflakeLoginTextEdit.Text = string.Empty;
		snowflakePasswordTextEdit.Text = string.Empty;
	}

	private void AutofixSnowflakeHostFieldValue()
	{
		try
		{
			Uri uri = new Uri(snowflakeHostComboBoxEdit.Text);
			snowflakeHostComboBoxEdit.Text = uri.Host;
		}
		catch (Exception)
		{
		}
	}

	public bool IsAccountAdminRoleSelected()
	{
		return string.Equals(snowflakeRoleButtonEdit?.Text, "accountadmin", StringComparison.OrdinalIgnoreCase);
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
		this.snowflakeLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.snowflakeDatabaseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.snowflakeSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.snowflakeLoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.snowflakePasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.snowflakeWarehouseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.snowflakeHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.snowflakeAuthenticationLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.snowflakePrivateKeyButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.snowflakeRoleButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.snowflakeLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.snowflakePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.snowflakeLoginLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.snowflakeSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem41 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem43 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem44 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.snowflakeDatabaseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.snowflakeTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.snowflakeWarehouseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.snowFlakeHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem53 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem64 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem65 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem23 = new DevExpress.XtraLayout.LayoutControlItem();
		this.snowflakePrivateKeyRow = new DevExpress.XtraLayout.LayoutControlItem();
		this.snowflakeRoleLayoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.snowflakeLayoutControl).BeginInit();
		this.snowflakeLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.snowflakeDatabaseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeLoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakePasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeWarehouseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeAuthenticationLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakePrivateKeyButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeRoleButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeLoginLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem41).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem43).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem44).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeDatabaseLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeTimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeWarehouseLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowFlakeHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem53).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem64).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem65).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem23).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakePrivateKeyRow).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeRoleLayoutControlItem1).BeginInit();
		base.SuspendLayout();
		this.snowflakeLayoutControl.AllowCustomization = false;
		this.snowflakeLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.snowflakeLayoutControl.Controls.Add(this.snowflakeDatabaseButtonEdit);
		this.snowflakeLayoutControl.Controls.Add(this.snowflakeSavePasswordCheckEdit);
		this.snowflakeLayoutControl.Controls.Add(this.snowflakeLoginTextEdit);
		this.snowflakeLayoutControl.Controls.Add(this.snowflakePasswordTextEdit);
		this.snowflakeLayoutControl.Controls.Add(this.snowflakeWarehouseButtonEdit);
		this.snowflakeLayoutControl.Controls.Add(this.snowflakeHostComboBoxEdit);
		this.snowflakeLayoutControl.Controls.Add(this.snowflakeAuthenticationLookUpEdit);
		this.snowflakeLayoutControl.Controls.Add(this.snowflakePrivateKeyButtonEdit);
		this.snowflakeLayoutControl.Controls.Add(this.snowflakeRoleButtonEdit);
		this.snowflakeLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.snowflakeLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.snowflakeLayoutControl.Name = "snowflakeLayoutControl";
		this.snowflakeLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1026, 170, 279, 548);
		this.snowflakeLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.snowflakeLayoutControl.Root = this.snowflakeLayoutControlGroup;
		this.snowflakeLayoutControl.Size = new System.Drawing.Size(493, 344);
		this.snowflakeLayoutControl.TabIndex = 30;
		this.snowflakeLayoutControl.Text = "layoutControl1";
		this.snowflakeDatabaseButtonEdit.Location = new System.Drawing.Point(105, 192);
		this.snowflakeDatabaseButtonEdit.Name = "snowflakeDatabaseButtonEdit";
		this.snowflakeDatabaseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.snowflakeDatabaseButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.snowflakeDatabaseButtonEdit.StyleController = this.snowflakeLayoutControl;
		this.snowflakeDatabaseButtonEdit.TabIndex = 4;
		this.snowflakeDatabaseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(SnowflakeDatabaseSplashScreenManager_ButtonClick);
		this.snowflakeDatabaseButtonEdit.EditValueChanged += new System.EventHandler(snowflakeDatabaseButtonEdit_EditValueChanged);
		this.snowflakeSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 120);
		this.snowflakeSavePasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.snowflakeSavePasswordCheckEdit.Name = "snowflakeSavePasswordCheckEdit";
		this.snowflakeSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.snowflakeSavePasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.snowflakeSavePasswordCheckEdit.StyleController = this.snowflakeLayoutControl;
		this.snowflakeSavePasswordCheckEdit.TabIndex = 3;
		this.snowflakeLoginTextEdit.Location = new System.Drawing.Point(105, 48);
		this.snowflakeLoginTextEdit.Name = "snowflakeLoginTextEdit";
		this.snowflakeLoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.snowflakeLoginTextEdit.StyleController = this.snowflakeLayoutControl;
		this.snowflakeLoginTextEdit.TabIndex = 1;
		this.snowflakeLoginTextEdit.EditValueChanged += new System.EventHandler(snowflakeLoginTextEdit_EditValueChanged);
		this.snowflakePasswordTextEdit.Location = new System.Drawing.Point(105, 96);
		this.snowflakePasswordTextEdit.Name = "snowflakePasswordTextEdit";
		this.snowflakePasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.snowflakePasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.snowflakePasswordTextEdit.StyleController = this.snowflakeLayoutControl;
		this.snowflakePasswordTextEdit.TabIndex = 2;
		this.snowflakePasswordTextEdit.EditValueChanged += new System.EventHandler(snowflakePasswordTextEdit_EditValueChanged);
		this.snowflakeWarehouseButtonEdit.Location = new System.Drawing.Point(105, 168);
		this.snowflakeWarehouseButtonEdit.Name = "snowflakeWarehouseButtonEdit";
		this.snowflakeWarehouseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.snowflakeWarehouseButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.snowflakeWarehouseButtonEdit.StyleController = this.snowflakeLayoutControl;
		this.snowflakeWarehouseButtonEdit.TabIndex = 4;
		this.snowflakeWarehouseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(OpenWarehouseSplashScreenManager_ButtonClick);
		this.snowflakeWarehouseButtonEdit.EnabledChanged += new System.EventHandler(snowflakeWarehouseButtonEdit_EditValueChanged);
		this.snowflakeHostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.snowflakeHostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.snowflakeHostComboBoxEdit.Name = "snowflakeHostComboBoxEdit";
		this.snowflakeHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.snowflakeHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.snowflakeHostComboBoxEdit.StyleController = this.snowflakeLayoutControl;
		this.snowflakeHostComboBoxEdit.TabIndex = 1;
		this.snowflakeHostComboBoxEdit.EditValueChanged += new System.EventHandler(SnowflakeHostValueChanged);
		this.snowflakeHostComboBoxEdit.Leave += new System.EventHandler(snowflakeHostComboBoxEdit_Leave);
		this.snowflakeAuthenticationLookUpEdit.Location = new System.Drawing.Point(105, 24);
		this.snowflakeAuthenticationLookUpEdit.Name = "snowflakeAuthenticationLookUpEdit";
		this.snowflakeAuthenticationLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.snowflakeAuthenticationLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("type", "Type")
		});
		this.snowflakeAuthenticationLookUpEdit.Properties.DisplayMember = "type";
		this.snowflakeAuthenticationLookUpEdit.Properties.NullText = "";
		this.snowflakeAuthenticationLookUpEdit.Properties.ShowFooter = false;
		this.snowflakeAuthenticationLookUpEdit.Properties.ShowHeader = false;
		this.snowflakeAuthenticationLookUpEdit.Properties.ShowLines = false;
		this.snowflakeAuthenticationLookUpEdit.Properties.ValueMember = "id";
		this.snowflakeAuthenticationLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.snowflakeAuthenticationLookUpEdit.StyleController = this.snowflakeLayoutControl;
		this.snowflakeAuthenticationLookUpEdit.TabIndex = 0;
		this.snowflakeAuthenticationLookUpEdit.EditValueChanged += new System.EventHandler(snowflakeAuthLookUpEdit_EditValueChanged);
		this.snowflakePrivateKeyButtonEdit.Location = new System.Drawing.Point(105, 72);
		this.snowflakePrivateKeyButtonEdit.Name = "snowflakePrivateKeyButtonEdit";
		this.snowflakePrivateKeyButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.snowflakePrivateKeyButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.snowflakePrivateKeyButtonEdit.StyleController = this.snowflakeLayoutControl;
		this.snowflakePrivateKeyButtonEdit.TabIndex = 4;
		this.snowflakePrivateKeyButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(SelectPrivateKeyFile_ButtonClick);
		this.snowflakeRoleButtonEdit.Location = new System.Drawing.Point(105, 144);
		this.snowflakeRoleButtonEdit.Name = "snowflakeRoleButtonEdit";
		this.snowflakeRoleButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.snowflakeRoleButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.snowflakeRoleButtonEdit.StyleController = this.snowflakeLayoutControl;
		this.snowflakeRoleButtonEdit.TabIndex = 4;
		this.snowflakeRoleButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(OpenSnowflakeRoleManager_ButtonClick);
		this.snowflakeLayoutControlGroup.CustomizationFormText = "Root";
		this.snowflakeLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.snowflakeLayoutControlGroup.GroupBordersVisible = false;
		this.snowflakeLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[16]
		{
			this.snowflakePasswordLayoutControlItem, this.snowflakeLoginLayoutControlItem, this.snowflakeSavePasswordLayoutControlItem, this.emptySpaceItem41, this.emptySpaceItem43, this.emptySpaceItem44, this.snowflakeTimeoutEmptySpaceItem, this.snowflakeWarehouseLayoutControlItem, this.snowFlakeHostLayoutControlItem, this.emptySpaceItem53,
			this.emptySpaceItem64, this.emptySpaceItem65, this.layoutControlItem23, this.snowflakePrivateKeyRow, this.snowflakeRoleLayoutControlItem1, this.snowflakeDatabaseLayoutControlItem
		});
		this.snowflakeLayoutControlGroup.Name = "Root";
		this.snowflakeLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.snowflakeLayoutControlGroup.Size = new System.Drawing.Size(493, 344);
		this.snowflakeLayoutControlGroup.TextVisible = false;
		this.snowflakePasswordLayoutControlItem.Control = this.snowflakePasswordTextEdit;
		this.snowflakePasswordLayoutControlItem.CustomizationFormText = "Password";
		this.snowflakePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.snowflakePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.snowflakePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.snowflakePasswordLayoutControlItem.Name = "snowflakePasswordLayoutControlItem";
		this.snowflakePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.snowflakePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.snowflakePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.snowflakePasswordLayoutControlItem.Text = "Password:";
		this.snowflakePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.snowflakePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.snowflakePasswordLayoutControlItem.TextToControlDistance = 5;
		this.snowflakeLoginLayoutControlItem.Control = this.snowflakeLoginTextEdit;
		this.snowflakeLoginLayoutControlItem.CustomizationFormText = "User:";
		this.snowflakeLoginLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.snowflakeLoginLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.snowflakeLoginLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.snowflakeLoginLayoutControlItem.Name = "snowflakeLoginLayoutControlItem";
		this.snowflakeLoginLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.snowflakeLoginLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.snowflakeLoginLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.snowflakeLoginLayoutControlItem.Text = "User:";
		this.snowflakeLoginLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.snowflakeLoginLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.snowflakeLoginLayoutControlItem.TextToControlDistance = 5;
		this.snowflakeSavePasswordLayoutControlItem.Control = this.snowflakeSavePasswordCheckEdit;
		this.snowflakeSavePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem2";
		this.snowflakeSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 120);
		this.snowflakeSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.snowflakeSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.snowflakeSavePasswordLayoutControlItem.Name = "snowflakeSavePasswordLayoutControlItem";
		this.snowflakeSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.snowflakeSavePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.snowflakeSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.snowflakeSavePasswordLayoutControlItem.Text = " ";
		this.snowflakeSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.snowflakeSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.snowflakeSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem41.AllowHotTrack = false;
		this.emptySpaceItem41.CustomizationFormText = "emptySpaceItem6";
		this.emptySpaceItem41.Location = new System.Drawing.Point(0, 240);
		this.emptySpaceItem41.Name = "emptySpaceItem6";
		this.emptySpaceItem41.Size = new System.Drawing.Size(493, 104);
		this.emptySpaceItem41.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem43.AllowHotTrack = false;
		this.emptySpaceItem43.CustomizationFormText = "emptySpaceItem21";
		this.emptySpaceItem43.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem43.Name = "emptySpaceItem21";
		this.emptySpaceItem43.Size = new System.Drawing.Size(158, 24);
		this.emptySpaceItem43.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem44.AllowHotTrack = false;
		this.emptySpaceItem44.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem44.Location = new System.Drawing.Point(335, 96);
		this.emptySpaceItem44.Name = "emptySpaceItem22";
		this.emptySpaceItem44.Size = new System.Drawing.Size(158, 24);
		this.emptySpaceItem44.TextSize = new System.Drawing.Size(0, 0);
		this.snowflakeDatabaseLayoutControlItem.Control = this.snowflakeDatabaseButtonEdit;
		this.snowflakeDatabaseLayoutControlItem.Location = new System.Drawing.Point(0, 192);
		this.snowflakeDatabaseLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.snowflakeDatabaseLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.snowflakeDatabaseLayoutControlItem.Name = "snowflakeDatabaseLayoutControlItem";
		this.snowflakeDatabaseLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.snowflakeDatabaseLayoutControlItem.Size = new System.Drawing.Size(493, 24);
		this.snowflakeDatabaseLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.snowflakeDatabaseLayoutControlItem.Text = "Database:";
		this.snowflakeDatabaseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.snowflakeDatabaseLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.snowflakeDatabaseLayoutControlItem.TextToControlDistance = 5;
		this.snowflakeTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.snowflakeTimeoutEmptySpaceItem.CustomizationFormText = "emptySpaceItem12";
		this.snowflakeTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 216);
		this.snowflakeTimeoutEmptySpaceItem.MaxSize = new System.Drawing.Size(405, 24);
		this.snowflakeTimeoutEmptySpaceItem.MinSize = new System.Drawing.Size(405, 24);
		this.snowflakeTimeoutEmptySpaceItem.Name = "snowflakeTimeoutEmptySpaceItem";
		this.snowflakeTimeoutEmptySpaceItem.Size = new System.Drawing.Size(493, 24);
		this.snowflakeTimeoutEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.snowflakeTimeoutEmptySpaceItem.Text = "emptySpaceItem12";
		this.snowflakeTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.snowflakeWarehouseLayoutControlItem.Control = this.snowflakeWarehouseButtonEdit;
		this.snowflakeWarehouseLayoutControlItem.CustomizationFormText = "Database:";
		this.snowflakeWarehouseLayoutControlItem.Location = new System.Drawing.Point(0, 168);
		this.snowflakeWarehouseLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.snowflakeWarehouseLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.snowflakeWarehouseLayoutControlItem.Name = "snowflakeWarehouseLayoutControlItem";
		this.snowflakeWarehouseLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.snowflakeWarehouseLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.snowflakeWarehouseLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.snowflakeWarehouseLayoutControlItem.Text = "Warehouse:";
		this.snowflakeWarehouseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.snowflakeWarehouseLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.snowflakeWarehouseLayoutControlItem.TextToControlDistance = 5;
		this.snowFlakeHostLayoutControlItem.Control = this.snowflakeHostComboBoxEdit;
		this.snowFlakeHostLayoutControlItem.CustomizationFormText = "Host:";
		this.snowFlakeHostLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.snowFlakeHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.snowFlakeHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.snowFlakeHostLayoutControlItem.Name = "snowFlakeHostLayoutControlItem";
		this.snowFlakeHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.snowFlakeHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.snowFlakeHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.snowFlakeHostLayoutControlItem.Text = "Host:";
		this.snowFlakeHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.snowFlakeHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.snowFlakeHostLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem53.AllowHotTrack = false;
		this.emptySpaceItem53.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem53.Name = "emptySpaceItem53";
		this.emptySpaceItem53.Size = new System.Drawing.Size(158, 24);
		this.emptySpaceItem53.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem64.AllowHotTrack = false;
		this.emptySpaceItem64.Location = new System.Drawing.Point(335, 120);
		this.emptySpaceItem64.MaxSize = new System.Drawing.Size(0, 24);
		this.emptySpaceItem64.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem64.Name = "emptySpaceItem64";
		this.emptySpaceItem64.Size = new System.Drawing.Size(158, 24);
		this.emptySpaceItem64.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem64.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem65.AllowHotTrack = false;
		this.emptySpaceItem65.Location = new System.Drawing.Point(335, 168);
		this.emptySpaceItem65.MaxSize = new System.Drawing.Size(0, 24);
		this.emptySpaceItem65.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem65.Name = "emptySpaceItem65";
		this.emptySpaceItem65.Size = new System.Drawing.Size(158, 24);
		this.emptySpaceItem65.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem65.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem23.Control = this.snowflakeAuthenticationLookUpEdit;
		this.layoutControlItem23.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.layoutControlItem23.CustomizationFormText = "Authentication:";
		this.layoutControlItem23.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem23.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem23.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem23.Name = "layoutControlItem23";
		this.layoutControlItem23.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem23.Size = new System.Drawing.Size(493, 24);
		this.layoutControlItem23.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem23.Text = "Authentication:";
		this.layoutControlItem23.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem23.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem23.TextToControlDistance = 5;
		this.snowflakePrivateKeyRow.Control = this.snowflakePrivateKeyButtonEdit;
		this.snowflakePrivateKeyRow.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.snowflakePrivateKeyRow.CustomizationFormText = "Private key:";
		this.snowflakePrivateKeyRow.Location = new System.Drawing.Point(0, 72);
		this.snowflakePrivateKeyRow.MaxSize = new System.Drawing.Size(335, 24);
		this.snowflakePrivateKeyRow.MinSize = new System.Drawing.Size(335, 24);
		this.snowflakePrivateKeyRow.Name = "snowflakePrivateKeyRow";
		this.snowflakePrivateKeyRow.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.snowflakePrivateKeyRow.Size = new System.Drawing.Size(493, 24);
		this.snowflakePrivateKeyRow.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.snowflakePrivateKeyRow.Text = "Private key:";
		this.snowflakePrivateKeyRow.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.snowflakePrivateKeyRow.TextSize = new System.Drawing.Size(100, 13);
		this.snowflakePrivateKeyRow.TextToControlDistance = 5;
		this.snowflakeRoleLayoutControlItem1.Control = this.snowflakeRoleButtonEdit;
		this.snowflakeRoleLayoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.snowflakeRoleLayoutControlItem1.CustomizationFormText = "Role:";
		this.snowflakeRoleLayoutControlItem1.Location = new System.Drawing.Point(0, 144);
		this.snowflakeRoleLayoutControlItem1.MaxSize = new System.Drawing.Size(335, 24);
		this.snowflakeRoleLayoutControlItem1.MinSize = new System.Drawing.Size(335, 24);
		this.snowflakeRoleLayoutControlItem1.Name = "snowflakeRoleLayoutControlItem1";
		this.snowflakeRoleLayoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.snowflakeRoleLayoutControlItem1.Size = new System.Drawing.Size(493, 24);
		this.snowflakeRoleLayoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.snowflakeRoleLayoutControlItem1.Text = "Role:";
		this.snowflakeRoleLayoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.snowflakeRoleLayoutControlItem1.TextSize = new System.Drawing.Size(100, 13);
		this.snowflakeRoleLayoutControlItem1.TextToControlDistance = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.snowflakeLayoutControl);
		base.Name = "SnowflakeConnectorControl";
		base.Size = new System.Drawing.Size(493, 344);
		((System.ComponentModel.ISupportInitialize)this.snowflakeLayoutControl).EndInit();
		this.snowflakeLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.snowflakeDatabaseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeLoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakePasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeWarehouseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeAuthenticationLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakePrivateKeyButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeRoleButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeLoginLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem41).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem43).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem44).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeDatabaseLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeTimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeWarehouseLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowFlakeHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem53).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem64).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem65).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem23).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakePrivateKeyRow).EndInit();
		((System.ComponentModel.ISupportInitialize)this.snowflakeRoleLayoutControlItem1).EndInit();
		base.ResumeLayout(false);
	}
}
