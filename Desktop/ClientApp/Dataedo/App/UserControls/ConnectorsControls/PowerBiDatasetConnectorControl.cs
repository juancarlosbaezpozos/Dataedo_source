using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools.MessageBoxes;
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

public class PowerBiDatasetConnectorControl : ConnectorControlBase
{
	private const string DefaultPerspectiveName = "Model";

	private string powerBiPerspectiveName;

	private IContainer components;

	private NonCustomizableLayoutControl mainNonCustomizableLayoutControl;

	private ButtonEdit databaseButtonEdit;

	private CheckEdit savePasswordCheckEdit;

	private TextEdit loginTextEdit;

	private TextEdit passwordTextEdit;

	private ComboBoxEdit hostComboBoxEdit;

	private LookUpEdit authenticationLookUpEdit;

	private LayoutControlGroup layoutControlGroup12;

	private LayoutControlItem layoutControlItem25;

	private LayoutControlItem layoutControlItem29;

	private LayoutControlItem layoutControlItem30;

	private EmptySpaceItem emptySpaceItem78;

	private EmptySpaceItem emptySpaceItem79;

	private EmptySpaceItem emptySpaceItem80;

	private EmptySpaceItem emptySpaceItem81;

	private LayoutControlItem layoutControlItem31;

	private EmptySpaceItem emptySpaceItem82;

	private LayoutControlItem layoutControlItem34;

	private EmptySpaceItem emptySpaceItem84;

	private EmptySpaceItem emptySpaceItem85;

	private LayoutControlItem layoutControlItem26;

	private string providedPowerBiHost => splittedHost?.Host ?? hostComboBoxEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.PowerBiDataset;

	protected override TextEdit HostTextEdit => hostComboBoxEdit;

	protected override ComboBoxEdit HostComboBoxEdit => hostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => savePasswordCheckEdit;

	public PowerBiDatasetConnectorControl()
	{
		InitializeComponent();
	}

	protected override void SetAuthenticationDataSource()
	{
		AuthenticationSSAS.SetAuthenticationDataSource(authenticationLookUpEdit);
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
			if (!CheckAuthenticationType())
			{
				return false;
			}
			SetNewDBRowValues();
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
			ConnectionResult connectionResult = base.DatabaseRow.TryConnection();
			if (connectionResult.IsSuccess)
			{
				if (!testForGettingDatabasesList && DatabaseSupportFactory.GetDatabaseSupport(base.SelectedDatabaseType) is IPerspectiveDatabase perspectiveDatabase)
				{
					List<string> perspectiveNames = perspectiveDatabase.GetPerspectiveNames(base.DatabaseRow.ConnectionString, base.DatabaseRow.Connection, databaseButtonEdit.Text, base.ParentForm);
					if (perspectiveNames == null)
					{
						return false;
					}
					if (string.IsNullOrEmpty(powerBiPerspectiveName))
					{
						powerBiPerspectiveName = "Model";
					}
					if (!perspectiveNames.Contains(powerBiPerspectiveName))
					{
						CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
						GeneralMessageBoxesHandling.Show("Specified perspective not found. Check the perspective name and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
						return false;
					}
				}
				base.DatabaseRow.Connection = connectionResult.Connection;
				SetDBMSVersion();
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
				SharedDatabaseTypeEnum.DatabaseType? expectedDatabaseType = GetExpectedDatabaseType();
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
				if (!expectedDatabaseType.HasValue)
				{
					return false;
				}
				if (expectedDatabaseType != base.DatabaseRow.Type)
				{
					base.DatabaseRow.Type = expectedDatabaseType;
					base.SelectedDatabaseType = expectedDatabaseType;
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

	public override void SetSwitchDatabaseAvailability(bool isAvailable)
	{
		databaseButtonEdit.Enabled = isAvailable;
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? databaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedPowerBiHost, loginTextEdit.Text, passwordTextEdit.Text, AuthenticationSSAS.IsWindowsAuthentication(authenticationLookUpEdit), base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, null, AuthenticationType.GetTypeByIndex(AuthenticationSSAS.GetIndex(authenticationLookUpEdit)), powerBiPerspectiveName);
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateHost();
		flag &= ValidateAuthenticationType();
		if (!AuthenticationSSAS.IsWindowsAuthentication(authenticationLookUpEdit))
		{
			flag &= ValidateLogin();
			flag &= ValidatePassword();
		}
		if (!testForGettingDatabasesList)
		{
			flag &= ValidateDatabase();
		}
		return flag;
	}

	private bool ValidateHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(hostComboBoxEdit, addDBErrorProvider, "server name", acceptEmptyValue);
	}

	private bool ValidateLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(loginTextEdit, addDBErrorProvider, "user", acceptEmptyValue);
	}

	private bool ValidateAuthenticationType(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(authenticationLookUpEdit, addDBErrorProvider, "authentication type", acceptEmptyValue);
	}

	private bool ValidatePassword(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(passwordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(databaseButtonEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		databaseButtonEdit.EditValue = base.DatabaseRow.Name;
		hostComboBoxEdit.Text = base.DatabaseRow.Host;
		passwordTextEdit.Text = base.DatabaseRow.Password;
		loginTextEdit.Text = base.DatabaseRow.User;
		authenticationLookUpEdit.EditValue = AuthenticationType.GetLookupIndex(base.DatabaseRow.SelectedAuthenticationType);
		savePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
		powerBiPerspectiveName = base.DatabaseRow.Perspective?.Trim();
	}

	protected override string GetPanelDocumentationTitle()
	{
		return databaseButtonEdit.Text + "@" + hostComboBoxEdit.Text;
	}

	private void loginTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateLogin(acceptEmptyValue: true);
	}

	private void passwordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidatePassword();
	}

	private void DatabaseButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateDatabase(acceptEmptyValue: true);
		if ((!(sender is ButtonEdit buttonEdit) || buttonEdit.ContainsFocus) && base.SelectedDatabaseType.HasValue && base.DatabaseRow.Type.HasValue && DatabaseSupportFactory.GetDatabaseSupport(base.SelectedDatabaseType) is IPerspectiveDatabase perspectiveDatabase)
		{
			powerBiPerspectiveName = perspectiveDatabase.GetPerspectiveNames(base.DatabaseRow.ConnectionString, base.DatabaseRow.Connection, databaseButtonEdit.Text, base.ParentForm)?.FirstOrDefault();
		}
	}

	private void authenticationLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (AuthenticationSSAS.IsWindowsAuthentication(authenticationLookUpEdit) || AuthenticationSSAS.IsActiveDirectoryIntegrated(authenticationLookUpEdit))
		{
			loginTextEdit.Text = WindowsIdentity.GetCurrent().Name;
			TextEdit textEdit = loginTextEdit;
			TextEdit textEdit2 = passwordTextEdit;
			bool flag2 = (savePasswordCheckEdit.Enabled = false);
			bool enabled = (textEdit2.Enabled = flag2);
			textEdit.Enabled = enabled;
			passwordTextEdit.Text = string.Empty;
		}
		else
		{
			TextEdit textEdit3 = loginTextEdit;
			TextEdit textEdit4 = passwordTextEdit;
			bool flag2 = (savePasswordCheckEdit.Enabled = true);
			bool enabled = (textEdit4.Enabled = flag2);
			textEdit3.Enabled = enabled;
		}
		ValidateAuthenticationType(acceptEmptyValue: true);
	}

	private void authenticationLookUpEdit_EditValueChanging(object sender, ChangingEventArgs e)
	{
		AuthenticationType.AuthenticationTypeEnum? authenticationTypeEnum = ((e.NewValue == null) ? null : new AuthenticationType.AuthenticationTypeEnum?(AuthenticationType.GetTypeByIndex((int)e.NewValue)));
		AuthenticationType.AuthenticationTypeEnum? authenticationTypeEnum2 = ((e.OldValue == null) ? null : new AuthenticationType.AuthenticationTypeEnum?(AuthenticationType.GetTypeByIndex((int)e.OldValue)));
		if ((authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.WindowsAuthentication || authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryIntegrated) && (authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.StandardAuthentication || authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryPassword))
		{
			loginTextEdit.Text = lastProvidedLogin;
		}
		else if ((authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.WindowsAuthentication || authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryIntegrated) && (authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.StandardAuthentication || authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryPassword))
		{
			lastProvidedLogin = loginTextEdit.Text;
		}
	}

	private void OpenSplashScreenManager_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		if (!CheckAuthenticationType())
		{
			return;
		}
		ButtonEdit buttonEdit = sender as ButtonEdit;
		if (!ValidateRequiredFields(testForGettingDatabasesList: true))
		{
			GeneralMessageBoxesHandling.Show("Required fields are not filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
			return;
		}
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
		SetNewDBRowValues(forGettingDatabasesList: true);
		List<string> databases = base.DatabaseRow.GetDatabases(base.SplashScreenManager, forceStandardConnection: false, FindForm());
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
		if (databases == null)
		{
			return;
		}
		string empty = string.Empty;
		try
		{
			base.DatabaseRow.Name = string.Empty;
			ConnectionResult connectionResult = base.DatabaseRow.TryConnection(useOnlyRequiredFields: true);
			if (connectionResult.Exception != null)
			{
				throw connectionResult.Exception;
			}
			ListForm listForm = new ListForm(databases, "Databases list");
			if (listForm.ShowDialog(this, setCustomMessageDefaultOwner: true) == DialogResult.OK)
			{
				buttonEdit.EditValue = listForm.SelectedValue;
			}
		}
		catch (Exception ex)
		{
			DatabaseSupportFactory.GetDatabaseSupport(base.DatabaseRow.Type).ProcessException(ex, base.DatabaseRow.Name, base.DatabaseRow.ServiceName, FindForm());
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
		}
		finally
		{
			base.DatabaseRow.Name = empty;
		}
	}

	private void hostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		hostComboBoxEdit.Text = string.Empty;
		passwordTextEdit.Text = string.Empty;
	}

	private bool CheckAuthenticationType()
	{
		if (AuthenticationSSAS.IsWindowsAuthentication(authenticationLookUpEdit))
		{
			GeneralMessageBoxesHandling.Show("This authentication option is not supported by this type of connection." + Environment.NewLine + Environment.NewLine + "Please choose a different authentication type.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, FindForm());
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
		this.mainNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.databaseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.savePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.loginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.passwordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.hostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.authenticationLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.layoutControlGroup12 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem25 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem29 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem30 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem78 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem79 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem80 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem81 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem31 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem82 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem34 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem84 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem85 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem26 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).BeginInit();
		this.mainNonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.databaseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.loginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.authenticationLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem25).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem29).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem30).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem78).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem79).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem80).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem81).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem31).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem82).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem34).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem84).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem85).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem26).BeginInit();
		base.SuspendLayout();
		this.mainNonCustomizableLayoutControl.AllowCustomization = false;
		this.mainNonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainNonCustomizableLayoutControl.Controls.Add(this.databaseButtonEdit);
		this.mainNonCustomizableLayoutControl.Controls.Add(this.savePasswordCheckEdit);
		this.mainNonCustomizableLayoutControl.Controls.Add(this.loginTextEdit);
		this.mainNonCustomizableLayoutControl.Controls.Add(this.passwordTextEdit);
		this.mainNonCustomizableLayoutControl.Controls.Add(this.hostComboBoxEdit);
		this.mainNonCustomizableLayoutControl.Controls.Add(this.authenticationLookUpEdit);
		this.mainNonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainNonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainNonCustomizableLayoutControl.Name = "ssasTabularNonCustomizableLayoutControl";
		this.mainNonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2719, 179, 279, 548);
		this.mainNonCustomizableLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.mainNonCustomizableLayoutControl.Root = this.layoutControlGroup12;
		this.mainNonCustomizableLayoutControl.Size = new System.Drawing.Size(484, 422);
		this.mainNonCustomizableLayoutControl.TabIndex = 4;
		this.mainNonCustomizableLayoutControl.Text = "layoutControl1";
		this.databaseButtonEdit.Location = new System.Drawing.Point(105, 120);
		this.databaseButtonEdit.Name = "ssasTabularDatabaseButtonEdit";
		this.databaseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.databaseButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.databaseButtonEdit.StyleController = this.mainNonCustomizableLayoutControl;
		this.databaseButtonEdit.TabIndex = 4;
		this.databaseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(OpenSplashScreenManager_ButtonClick);
		this.databaseButtonEdit.EditValueChanged += new System.EventHandler(DatabaseButtonEdit_EditValueChanged);
		this.savePasswordCheckEdit.Location = new System.Drawing.Point(105, 96);
		this.savePasswordCheckEdit.Name = "ssasTabularSavePasswordCheckEdit";
		this.savePasswordCheckEdit.Properties.Caption = "Save password";
		this.savePasswordCheckEdit.Size = new System.Drawing.Size(230, 20);
		this.savePasswordCheckEdit.StyleController = this.mainNonCustomizableLayoutControl;
		this.savePasswordCheckEdit.TabIndex = 3;
		this.loginTextEdit.Location = new System.Drawing.Point(105, 48);
		this.loginTextEdit.Name = "ssasTabularLoginTextEdit";
		this.loginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.loginTextEdit.StyleController = this.mainNonCustomizableLayoutControl;
		this.loginTextEdit.TabIndex = 1;
		this.loginTextEdit.EditValueChanged += new System.EventHandler(loginTextEdit_EditValueChanged);
		this.passwordTextEdit.Location = new System.Drawing.Point(105, 72);
		this.passwordTextEdit.Name = "ssasTabularPasswordTextEdit";
		this.passwordTextEdit.Properties.UseSystemPasswordChar = true;
		this.passwordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.passwordTextEdit.StyleController = this.mainNonCustomizableLayoutControl;
		this.passwordTextEdit.TabIndex = 2;
		this.passwordTextEdit.EditValueChanged += new System.EventHandler(passwordTextEdit_EditValueChanged);
		this.hostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.hostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2);
		this.hostComboBoxEdit.Name = "ssasTabularHostComboBoxEdit";
		this.hostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.hostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.hostComboBoxEdit.StyleController = this.mainNonCustomizableLayoutControl;
		this.hostComboBoxEdit.TabIndex = 1;
		this.hostComboBoxEdit.EditValueChanged += new System.EventHandler(hostComboBoxEdit_EditValueChanged);
		this.authenticationLookUpEdit.Location = new System.Drawing.Point(105, 24);
		this.authenticationLookUpEdit.Name = "ssasTabularAuthenticationLookUpEdit";
		this.authenticationLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.authenticationLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("type", "Type")
		});
		this.authenticationLookUpEdit.Properties.DisplayMember = "type";
		this.authenticationLookUpEdit.Properties.NullText = "";
		this.authenticationLookUpEdit.Properties.ShowFooter = false;
		this.authenticationLookUpEdit.Properties.ShowHeader = false;
		this.authenticationLookUpEdit.Properties.ShowLines = false;
		this.authenticationLookUpEdit.Properties.ValueMember = "id";
		this.authenticationLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.authenticationLookUpEdit.StyleController = this.mainNonCustomizableLayoutControl;
		this.authenticationLookUpEdit.TabIndex = 0;
		this.authenticationLookUpEdit.EditValueChanged += new System.EventHandler(authenticationLookUpEdit_EditValueChanged);
		this.authenticationLookUpEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(authenticationLookUpEdit_EditValueChanging);
		this.layoutControlGroup12.CustomizationFormText = "Root";
		this.layoutControlGroup12.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup12.GroupBordersVisible = false;
		this.layoutControlGroup12.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[13]
		{
			this.layoutControlItem25, this.layoutControlItem29, this.layoutControlItem30, this.emptySpaceItem78, this.emptySpaceItem79, this.emptySpaceItem80, this.emptySpaceItem81, this.layoutControlItem31, this.emptySpaceItem82, this.layoutControlItem34,
			this.emptySpaceItem84, this.emptySpaceItem85, this.layoutControlItem26
		});
		this.layoutControlGroup12.Name = "Root";
		this.layoutControlGroup12.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup12.Size = new System.Drawing.Size(484, 422);
		this.layoutControlGroup12.TextVisible = false;
		this.layoutControlItem25.Control = this.passwordTextEdit;
		this.layoutControlItem25.CustomizationFormText = "Password";
		this.layoutControlItem25.Location = new System.Drawing.Point(0, 72);
		this.layoutControlItem25.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem25.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem25.Name = "sqlServerPasswordLayoutControlItem";
		this.layoutControlItem25.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem25.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem25.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem25.Text = "Password:";
		this.layoutControlItem25.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem25.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem25.TextToControlDistance = 5;
		this.layoutControlItem29.Control = this.loginTextEdit;
		this.layoutControlItem29.CustomizationFormText = "User:";
		this.layoutControlItem29.Location = new System.Drawing.Point(0, 48);
		this.layoutControlItem29.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem29.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem29.Name = "sqlServerLoginLayoutControlItem";
		this.layoutControlItem29.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem29.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem29.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem29.Text = "User:";
		this.layoutControlItem29.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem29.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem29.TextToControlDistance = 5;
		this.layoutControlItem30.Control = this.savePasswordCheckEdit;
		this.layoutControlItem30.CustomizationFormText = "layoutControlItem2";
		this.layoutControlItem30.Location = new System.Drawing.Point(0, 96);
		this.layoutControlItem30.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem30.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem30.Name = "sqlServerSavePasswordLayoutControlItem";
		this.layoutControlItem30.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem30.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem30.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem30.Text = " ";
		this.layoutControlItem30.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem30.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem30.TextToControlDistance = 5;
		this.emptySpaceItem78.AllowHotTrack = false;
		this.emptySpaceItem78.CustomizationFormText = "emptySpaceItem6";
		this.emptySpaceItem78.Location = new System.Drawing.Point(0, 228);
		this.emptySpaceItem78.Name = "emptySpaceItem6";
		this.emptySpaceItem78.Size = new System.Drawing.Size(484, 194);
		this.emptySpaceItem78.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem79.AllowHotTrack = false;
		this.emptySpaceItem79.CustomizationFormText = "emptySpaceItem21";
		this.emptySpaceItem79.Location = new System.Drawing.Point(335, 24);
		this.emptySpaceItem79.Name = "emptySpaceItem21";
		this.emptySpaceItem79.Size = new System.Drawing.Size(149, 48);
		this.emptySpaceItem79.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem80.AllowHotTrack = false;
		this.emptySpaceItem80.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem80.Location = new System.Drawing.Point(335, 72);
		this.emptySpaceItem80.Name = "emptySpaceItem22";
		this.emptySpaceItem80.Size = new System.Drawing.Size(149, 24);
		this.emptySpaceItem80.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem81.AllowHotTrack = false;
		this.emptySpaceItem81.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem81.Location = new System.Drawing.Point(335, 127);
		this.emptySpaceItem81.Name = "emptySpaceItem28";
		this.emptySpaceItem81.Size = new System.Drawing.Size(149, 17);
		this.emptySpaceItem81.Text = "emptySpaceItem22";
		this.emptySpaceItem81.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem31.Control = this.databaseButtonEdit;
		this.layoutControlItem31.Location = new System.Drawing.Point(0, 120);
		this.layoutControlItem31.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem31.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem31.Name = "sqlServerDatabaseLayoutControlItem";
		this.layoutControlItem31.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem31.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem31.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem31.Text = "Dataset:";
		this.layoutControlItem31.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem31.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem31.TextToControlDistance = 5;
		this.emptySpaceItem82.AllowHotTrack = false;
		this.emptySpaceItem82.CustomizationFormText = "emptySpaceItem12";
		this.emptySpaceItem82.Location = new System.Drawing.Point(0, 144);
		this.emptySpaceItem82.Name = "sqlServerTimeoutEmptySpaceItem";
		this.emptySpaceItem82.Size = new System.Drawing.Size(484, 84);
		this.emptySpaceItem82.Text = "emptySpaceItem12";
		this.emptySpaceItem82.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem34.Control = this.hostComboBoxEdit;
		this.layoutControlItem34.CustomizationFormText = "XMLA endpoint:";
		this.layoutControlItem34.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem34.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem34.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem34.Name = "sqlServerHostLayoutControlItem";
		this.layoutControlItem34.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem34.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem34.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem34.Text = "XMLA endpoint:";
		this.layoutControlItem34.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem34.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem34.TextToControlDistance = 5;
		this.emptySpaceItem84.AllowHotTrack = false;
		this.emptySpaceItem84.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem84.Name = "emptySpaceItem3";
		this.emptySpaceItem84.Size = new System.Drawing.Size(149, 24);
		this.emptySpaceItem84.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem85.AllowHotTrack = false;
		this.emptySpaceItem85.Location = new System.Drawing.Point(335, 96);
		this.emptySpaceItem85.Name = "emptySpaceItem4";
		this.emptySpaceItem85.Size = new System.Drawing.Size(149, 31);
		this.emptySpaceItem85.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem26.Control = this.authenticationLookUpEdit;
		this.layoutControlItem26.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.layoutControlItem26.CustomizationFormText = "Authentication:";
		this.layoutControlItem26.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem26.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem26.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem26.Name = "layoutControlItem26";
		this.layoutControlItem26.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem26.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem26.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem26.Text = "Authentication:";
		this.layoutControlItem26.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem26.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem26.TextToControlDistance = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainNonCustomizableLayoutControl);
		base.Name = "PowerBiDatasetConnectorControl";
		base.Size = new System.Drawing.Size(484, 422);
		((System.ComponentModel.ISupportInitialize)this.mainNonCustomizableLayoutControl).EndInit();
		this.mainNonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.databaseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.loginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.authenticationLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem25).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem29).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem30).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem78).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem79).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem80).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem81).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem31).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem82).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem34).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem84).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem85).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem26).EndInit();
		base.ResumeLayout(false);
	}
}
