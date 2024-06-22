using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Security.Principal;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
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

public class SSASConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl ssasTabularNonCustomizableLayoutControl;

	private ButtonEdit ssasTabularDatabaseButtonEdit;

	private CheckEdit ssasTabularSavePasswordCheckEdit;

	private TextEdit ssasTabularLoginTextEdit;

	private TextEdit ssasTabularPasswordTextEdit;

	private ComboBoxEdit ssasTabularHostComboBoxEdit;

	private LookUpEdit ssasTabularAuthenticationLookUpEdit;

	private ButtonEdit ssasTabularPerspectiveButtonEdit;

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

	private LayoutControlItem sqlServerDatabaseLayoutControlItem1;

	private string providedSsasTabularHost => splittedHost?.Host ?? ssasTabularHostComboBoxEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.SsasTabular;

	protected override TextEdit HostTextEdit => ssasTabularHostComboBoxEdit;

	protected override ComboBoxEdit HostComboBoxEdit => ssasTabularHostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => ssasTabularSavePasswordCheckEdit;

	public SSASConnectorControl()
	{
		InitializeComponent();
	}

	protected override void SetAuthenticationDataSource()
	{
		AuthenticationSSAS.SetAuthenticationDataSource(ssasTabularAuthenticationLookUpEdit);
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
			SetNewDBRowValues();
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
			ConnectionResult connectionResult = base.DatabaseRow.TryConnection();
			if (connectionResult.IsSuccess)
			{
				if (!testForGettingDatabasesList && DatabaseSupportFactory.GetDatabaseSupport(base.SelectedDatabaseType) is SsasTabularSupport ssasTabularSupport)
				{
					List<string> perspectiveNames = ssasTabularSupport.GetPerspectiveNames(base.DatabaseRow.ConnectionString, base.DatabaseRow.Connection, ssasTabularDatabaseButtonEdit.Text, base.ParentForm);
					if (perspectiveNames == null)
					{
						return false;
					}
					if (!string.IsNullOrEmpty(ssasTabularPerspectiveButtonEdit.Text) && !perspectiveNames.Contains(ssasTabularPerspectiveButtonEdit.Text))
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
		ssasTabularDatabaseButtonEdit.Enabled = isAvailable;
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? ssasTabularDatabaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedSsasTabularHost, ssasTabularLoginTextEdit.Text, ssasTabularPasswordTextEdit.Text, AuthenticationSSAS.IsWindowsAuthentication(ssasTabularAuthenticationLookUpEdit), base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, null, AuthenticationType.GetTypeByIndex(AuthenticationSSAS.GetIndex(ssasTabularAuthenticationLookUpEdit)), ssasTabularPerspectiveButtonEdit.Text);
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateSsasTabularHost();
		flag &= ValidateSsasTabularAuthenticationType();
		if (!AuthenticationSSAS.IsWindowsAuthentication(ssasTabularAuthenticationLookUpEdit))
		{
			flag &= ValidateSsasTabularLogin();
			flag &= ValidateSsasTabularPassword();
		}
		if (!testForGettingDatabasesList)
		{
			flag &= ValidateSsasTabularDatabase();
		}
		if (!testForGettingDatabasesList && !testForGettingPerspectiveList)
		{
			flag &= ValidateSsasTabularPerspective();
		}
		return flag;
	}

	private bool ValidateSsasTabularHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(ssasTabularHostComboBoxEdit, addDBErrorProvider, "server name", acceptEmptyValue);
	}

	private bool ValidateSsasTabularLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(ssasTabularLoginTextEdit, addDBErrorProvider, "user", acceptEmptyValue);
	}

	private bool ValidateSsasTabularAuthenticationType(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(ssasTabularAuthenticationLookUpEdit, addDBErrorProvider, "authentication type", acceptEmptyValue);
	}

	private bool ValidateSsasTabularPassword(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(ssasTabularPasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateSsasTabularDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(ssasTabularDatabaseButtonEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	private bool ValidateSsasTabularPerspective(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(ssasTabularPerspectiveButtonEdit, addDBErrorProvider, "perspective", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		ssasTabularDatabaseButtonEdit.EditValue = base.DatabaseRow.Name;
		ssasTabularHostComboBoxEdit.Text = base.DatabaseRow.Host;
		ssasTabularPasswordTextEdit.Text = base.DatabaseRow.Password;
		ssasTabularLoginTextEdit.Text = base.DatabaseRow.User;
		ssasTabularAuthenticationLookUpEdit.EditValue = AuthenticationType.GetLookupIndex(base.DatabaseRow.SelectedAuthenticationType);
		ssasTabularSavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
		ssasTabularPerspectiveButtonEdit.Text = base.DatabaseRow.Perspective?.Trim();
	}

	protected override string GetPanelDocumentationTitle()
	{
		return ssasTabularDatabaseButtonEdit.Text + "@" + ssasTabularHostComboBoxEdit.Text;
	}

	private void ssasTabularLoginTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSsasTabularLogin(acceptEmptyValue: true);
	}

	private void ssasTabularPasswordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSsasTabularPassword();
	}

	private void SsasTabularDatabaseButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSsasTabularDatabase(acceptEmptyValue: true);
		if (!(sender is ButtonEdit buttonEdit) || buttonEdit.ContainsFocus)
		{
			ssasTabularPerspectiveButtonEdit.EditValue = null;
		}
	}

	private void ssasTabularAuthenticationLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (AuthenticationSSAS.IsWindowsAuthentication(ssasTabularAuthenticationLookUpEdit) || AuthenticationSSAS.IsActiveDirectoryIntegrated(ssasTabularAuthenticationLookUpEdit))
		{
			ssasTabularLoginTextEdit.Text = WindowsIdentity.GetCurrent().Name;
			TextEdit textEdit = ssasTabularLoginTextEdit;
			TextEdit textEdit2 = ssasTabularPasswordTextEdit;
			bool flag2 = (ssasTabularSavePasswordCheckEdit.Enabled = false);
			bool enabled = (textEdit2.Enabled = flag2);
			textEdit.Enabled = enabled;
			ssasTabularPasswordTextEdit.Text = string.Empty;
		}
		else
		{
			TextEdit textEdit3 = ssasTabularLoginTextEdit;
			TextEdit textEdit4 = ssasTabularPasswordTextEdit;
			bool flag2 = (ssasTabularSavePasswordCheckEdit.Enabled = true);
			bool enabled = (textEdit4.Enabled = flag2);
			textEdit3.Enabled = enabled;
		}
		ValidateSsasTabularAuthenticationType(acceptEmptyValue: true);
	}

	private void ssasTabularAuthenticationLookUpEdit_EditValueChanging(object sender, ChangingEventArgs e)
	{
		AuthenticationType.AuthenticationTypeEnum? authenticationTypeEnum = ((e.NewValue == null) ? null : new AuthenticationType.AuthenticationTypeEnum?(AuthenticationType.GetTypeByIndex((int)e.NewValue)));
		AuthenticationType.AuthenticationTypeEnum? authenticationTypeEnum2 = ((e.OldValue == null) ? null : new AuthenticationType.AuthenticationTypeEnum?(AuthenticationType.GetTypeByIndex((int)e.OldValue)));
		if ((authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.WindowsAuthentication || authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryIntegrated) && (authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.StandardAuthentication || authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryPassword))
		{
			ssasTabularLoginTextEdit.Text = lastProvidedLogin;
		}
		else if ((authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.WindowsAuthentication || authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryIntegrated) && (authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.StandardAuthentication || authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.ActiveDirectoryPassword))
		{
			lastProvidedLogin = ssasTabularLoginTextEdit.Text;
		}
	}

	private void ssasTabularPerspectiveButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		string perspective = string.Empty;
		try
		{
			if (!ValidateRequiredFields(testForGettingDatabasesList: false, testForGettingWarehousesList: false, testForGettingPerspectiveList: true))
			{
				GeneralMessageBoxesHandling.Show("Required fields are not filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
			}
			else if (base.SelectedDatabaseType.HasValue && DatabaseSupportFactory.GetDatabaseSupport(base.SelectedDatabaseType) is SsasTabularSupport ssasTabularSupport)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
				List<string> databases = base.DatabaseRow.GetDatabases(base.SplashScreenManager);
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
				if (databases == null)
				{
					throw new Exception("Could not find any databases on the server.");
				}
				if (!databases.Contains(ssasTabularDatabaseButtonEdit.Text))
				{
					addDBErrorProvider.SetError(ssasTabularDatabaseButtonEdit, "Could not find provided database name on the server.");
					throw new Exception("Could not find provided database name on the server.");
				}
				ConnectionResult connectionResult = base.DatabaseRow.TryConnection(useOnlyRequiredFields: true);
				if (connectionResult.Exception != null)
				{
					throw connectionResult.Exception;
				}
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
				List<string> perspectiveNames = ssasTabularSupport.GetPerspectiveNames(base.DatabaseRow.ConnectionString, base.DatabaseRow.Connection, ssasTabularDatabaseButtonEdit.Text, base.ParentForm);
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
				string title = "Perspective list";
				ListForm listForm = new ListForm(perspectiveNames, title);
				if (listForm.ShowDialog(this, setCustomMessageDefaultOwner: true) == DialogResult.OK)
				{
					(sender as ButtonEdit).EditValue = listForm.SelectedValue;
					perspective = listForm.SelectedValue;
				}
			}
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
		}
		finally
		{
			base.DatabaseRow.Perspective = perspective;
		}
	}

	private void ssasTabularPerspectiveButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(ssasTabularDatabaseButtonEdit.Text))
		{
			ValidateSsasTabularPerspective();
		}
	}

	private void OpenSplashScreenManager_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
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

	private void ssasTabularHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		ssasTabularHostComboBoxEdit.Text = string.Empty;
		ssasTabularPasswordTextEdit.Text = string.Empty;
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
		this.ssasTabularNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.ssasTabularDatabaseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.ssasTabularSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.ssasTabularLoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.ssasTabularPasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.ssasTabularHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.ssasTabularAuthenticationLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.ssasTabularPerspectiveButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
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
		this.sqlServerDatabaseLayoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.ssasTabularNonCustomizableLayoutControl).BeginInit();
		this.ssasTabularNonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ssasTabularDatabaseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssasTabularSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssasTabularLoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssasTabularPasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssasTabularHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssasTabularAuthenticationLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssasTabularPerspectiveButtonEdit.Properties).BeginInit();
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
		((System.ComponentModel.ISupportInitialize)this.sqlServerDatabaseLayoutControlItem1).BeginInit();
		base.SuspendLayout();
		this.ssasTabularNonCustomizableLayoutControl.AllowCustomization = false;
		this.ssasTabularNonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.ssasTabularNonCustomizableLayoutControl.Controls.Add(this.ssasTabularDatabaseButtonEdit);
		this.ssasTabularNonCustomizableLayoutControl.Controls.Add(this.ssasTabularSavePasswordCheckEdit);
		this.ssasTabularNonCustomizableLayoutControl.Controls.Add(this.ssasTabularLoginTextEdit);
		this.ssasTabularNonCustomizableLayoutControl.Controls.Add(this.ssasTabularPasswordTextEdit);
		this.ssasTabularNonCustomizableLayoutControl.Controls.Add(this.ssasTabularHostComboBoxEdit);
		this.ssasTabularNonCustomizableLayoutControl.Controls.Add(this.ssasTabularAuthenticationLookUpEdit);
		this.ssasTabularNonCustomizableLayoutControl.Controls.Add(this.ssasTabularPerspectiveButtonEdit);
		this.ssasTabularNonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ssasTabularNonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.ssasTabularNonCustomizableLayoutControl.Name = "ssasTabularNonCustomizableLayoutControl";
		this.ssasTabularNonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2719, 179, 279, 548);
		this.ssasTabularNonCustomizableLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.ssasTabularNonCustomizableLayoutControl.Root = this.layoutControlGroup12;
		this.ssasTabularNonCustomizableLayoutControl.Size = new System.Drawing.Size(484, 422);
		this.ssasTabularNonCustomizableLayoutControl.TabIndex = 4;
		this.ssasTabularNonCustomizableLayoutControl.Text = "layoutControl1";
		this.ssasTabularDatabaseButtonEdit.Location = new System.Drawing.Point(105, 120);
		this.ssasTabularDatabaseButtonEdit.Name = "ssasTabularDatabaseButtonEdit";
		this.ssasTabularDatabaseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.ssasTabularDatabaseButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.ssasTabularDatabaseButtonEdit.StyleController = this.ssasTabularNonCustomizableLayoutControl;
		this.ssasTabularDatabaseButtonEdit.TabIndex = 4;
		this.ssasTabularDatabaseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(OpenSplashScreenManager_ButtonClick);
		this.ssasTabularDatabaseButtonEdit.EditValueChanged += new System.EventHandler(SsasTabularDatabaseButtonEdit_EditValueChanged);
		this.ssasTabularSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 96);
		this.ssasTabularSavePasswordCheckEdit.Name = "ssasTabularSavePasswordCheckEdit";
		this.ssasTabularSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.ssasTabularSavePasswordCheckEdit.Size = new System.Drawing.Size(230, 20);
		this.ssasTabularSavePasswordCheckEdit.StyleController = this.ssasTabularNonCustomizableLayoutControl;
		this.ssasTabularSavePasswordCheckEdit.TabIndex = 3;
		this.ssasTabularLoginTextEdit.Location = new System.Drawing.Point(105, 48);
		this.ssasTabularLoginTextEdit.Name = "ssasTabularLoginTextEdit";
		this.ssasTabularLoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.ssasTabularLoginTextEdit.StyleController = this.ssasTabularNonCustomizableLayoutControl;
		this.ssasTabularLoginTextEdit.TabIndex = 1;
		this.ssasTabularLoginTextEdit.EditValueChanged += new System.EventHandler(ssasTabularLoginTextEdit_EditValueChanged);
		this.ssasTabularPasswordTextEdit.Location = new System.Drawing.Point(105, 72);
		this.ssasTabularPasswordTextEdit.Name = "ssasTabularPasswordTextEdit";
		this.ssasTabularPasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.ssasTabularPasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.ssasTabularPasswordTextEdit.StyleController = this.ssasTabularNonCustomizableLayoutControl;
		this.ssasTabularPasswordTextEdit.TabIndex = 2;
		this.ssasTabularPasswordTextEdit.EditValueChanged += new System.EventHandler(ssasTabularPasswordTextEdit_EditValueChanged);
		this.ssasTabularHostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.ssasTabularHostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.ssasTabularHostComboBoxEdit.Name = "ssasTabularHostComboBoxEdit";
		this.ssasTabularHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ssasTabularHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.ssasTabularHostComboBoxEdit.StyleController = this.ssasTabularNonCustomizableLayoutControl;
		this.ssasTabularHostComboBoxEdit.TabIndex = 1;
		this.ssasTabularHostComboBoxEdit.EditValueChanged += new System.EventHandler(ssasTabularHostComboBoxEdit_EditValueChanged);
		this.ssasTabularAuthenticationLookUpEdit.Location = new System.Drawing.Point(105, 24);
		this.ssasTabularAuthenticationLookUpEdit.Name = "ssasTabularAuthenticationLookUpEdit";
		this.ssasTabularAuthenticationLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ssasTabularAuthenticationLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("type", "Type")
		});
		this.ssasTabularAuthenticationLookUpEdit.Properties.DisplayMember = "type";
		this.ssasTabularAuthenticationLookUpEdit.Properties.NullText = "";
		this.ssasTabularAuthenticationLookUpEdit.Properties.ShowFooter = false;
		this.ssasTabularAuthenticationLookUpEdit.Properties.ShowHeader = false;
		this.ssasTabularAuthenticationLookUpEdit.Properties.ShowLines = false;
		this.ssasTabularAuthenticationLookUpEdit.Properties.ValueMember = "id";
		this.ssasTabularAuthenticationLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.ssasTabularAuthenticationLookUpEdit.StyleController = this.ssasTabularNonCustomizableLayoutControl;
		this.ssasTabularAuthenticationLookUpEdit.TabIndex = 0;
		this.ssasTabularAuthenticationLookUpEdit.EditValueChanged += new System.EventHandler(ssasTabularAuthenticationLookUpEdit_EditValueChanged);
		this.ssasTabularAuthenticationLookUpEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(ssasTabularAuthenticationLookUpEdit_EditValueChanging);
		this.ssasTabularPerspectiveButtonEdit.Location = new System.Drawing.Point(105, 144);
		this.ssasTabularPerspectiveButtonEdit.Name = "ssasTabularPerspectiveButtonEdit";
		this.ssasTabularPerspectiveButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.ssasTabularPerspectiveButtonEdit.Properties.MaxLength = 250;
		this.ssasTabularPerspectiveButtonEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.ssasTabularPerspectiveButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.ssasTabularPerspectiveButtonEdit.StyleController = this.ssasTabularNonCustomizableLayoutControl;
		this.ssasTabularPerspectiveButtonEdit.TabIndex = 4;
		this.ssasTabularPerspectiveButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ssasTabularPerspectiveButtonEdit_ButtonClick);
		this.ssasTabularPerspectiveButtonEdit.EditValueChanged += new System.EventHandler(ssasTabularPerspectiveButtonEdit_EditValueChanged);
		this.layoutControlGroup12.CustomizationFormText = "Root";
		this.layoutControlGroup12.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup12.GroupBordersVisible = false;
		this.layoutControlGroup12.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[14]
		{
			this.layoutControlItem25, this.layoutControlItem29, this.layoutControlItem30, this.emptySpaceItem78, this.emptySpaceItem79, this.emptySpaceItem80, this.emptySpaceItem81, this.layoutControlItem31, this.emptySpaceItem82, this.layoutControlItem34,
			this.emptySpaceItem84, this.emptySpaceItem85, this.layoutControlItem26, this.sqlServerDatabaseLayoutControlItem1
		});
		this.layoutControlGroup12.Name = "Root";
		this.layoutControlGroup12.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup12.Size = new System.Drawing.Size(484, 422);
		this.layoutControlGroup12.TextVisible = false;
		this.layoutControlItem25.Control = this.ssasTabularPasswordTextEdit;
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
		this.layoutControlItem29.Control = this.ssasTabularLoginTextEdit;
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
		this.layoutControlItem30.Control = this.ssasTabularSavePasswordCheckEdit;
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
		this.layoutControlItem31.Control = this.ssasTabularDatabaseButtonEdit;
		this.layoutControlItem31.Location = new System.Drawing.Point(0, 120);
		this.layoutControlItem31.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem31.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem31.Name = "sqlServerDatabaseLayoutControlItem";
		this.layoutControlItem31.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem31.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem31.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem31.Text = "Database:";
		this.layoutControlItem31.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem31.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem31.TextToControlDistance = 5;
		this.emptySpaceItem82.AllowHotTrack = false;
		this.emptySpaceItem82.CustomizationFormText = "emptySpaceItem12";
		this.emptySpaceItem82.Location = new System.Drawing.Point(0, 168);
		this.emptySpaceItem82.Name = "sqlServerTimeoutEmptySpaceItem";
		this.emptySpaceItem82.Size = new System.Drawing.Size(484, 60);
		this.emptySpaceItem82.Text = "emptySpaceItem12";
		this.emptySpaceItem82.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem34.Control = this.ssasTabularHostComboBoxEdit;
		this.layoutControlItem34.CustomizationFormText = "Server name:";
		this.layoutControlItem34.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem34.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem34.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem34.Name = "sqlServerHostLayoutControlItem";
		this.layoutControlItem34.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem34.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem34.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem34.Text = "Server name:";
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
		this.layoutControlItem26.Control = this.ssasTabularAuthenticationLookUpEdit;
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
		this.sqlServerDatabaseLayoutControlItem1.Control = this.ssasTabularPerspectiveButtonEdit;
		this.sqlServerDatabaseLayoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.sqlServerDatabaseLayoutControlItem1.CustomizationFormText = "Perspective:";
		this.sqlServerDatabaseLayoutControlItem1.Location = new System.Drawing.Point(0, 144);
		this.sqlServerDatabaseLayoutControlItem1.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerDatabaseLayoutControlItem1.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerDatabaseLayoutControlItem1.Name = "sqlServerDatabaseLayoutControlItem1";
		this.sqlServerDatabaseLayoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerDatabaseLayoutControlItem1.Size = new System.Drawing.Size(484, 24);
		this.sqlServerDatabaseLayoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerDatabaseLayoutControlItem1.Text = "Perspective:";
		this.sqlServerDatabaseLayoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerDatabaseLayoutControlItem1.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerDatabaseLayoutControlItem1.TextToControlDistance = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.ssasTabularNonCustomizableLayoutControl);
		base.Name = "SSASConnectorControl";
		base.Size = new System.Drawing.Size(484, 422);
		((System.ComponentModel.ISupportInitialize)this.ssasTabularNonCustomizableLayoutControl).EndInit();
		this.ssasTabularNonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ssasTabularDatabaseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssasTabularSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssasTabularLoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssasTabularPasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssasTabularHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssasTabularAuthenticationLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssasTabularPerspectiveButtonEdit.Properties).EndInit();
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
		((System.ComponentModel.ISupportInitialize)this.sqlServerDatabaseLayoutControlItem1).EndInit();
		base.ResumeLayout(false);
	}
}
