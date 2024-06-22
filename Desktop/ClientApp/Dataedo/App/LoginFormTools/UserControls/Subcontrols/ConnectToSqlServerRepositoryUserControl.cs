using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.LoginFormTools.Tools.Recent;
using Dataedo.App.LoginFormTools.Tools.Repository;
using Dataedo.App.LoginFormTools.Tools.Repository.SqlServer;
using Dataedo.App.LoginFormTools.UserControls.Base;
using Dataedo.App.LoginFormTools.UserControls.Subcontrols.Interfaces;
using Dataedo.App.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.Data.Commands.Enums;
using Dataedo.DataProcessing.Classes;
using Dataedo.LicenseHelperLibrary.Repository;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraWaitForm;
using Microsoft.Data.SqlClient;
using RecentProjectsLibrary;

namespace Dataedo.App.LoginFormTools.UserControls.Subcontrols;

public class ConnectToSqlServerRepositoryUserControl : PageWithLoaderUserControl, IRepositoryConnection, IRecentProjectData
{
	private bool areDetailsShown = true;

	private Size baseSize;

	private string lastProvidedLogin = string.Empty;

	private bool lastProvidedSavePassword;

	private string lastProvidedPassword;

	private CancellationTokenSource gettingDatabasesCancellationTokenSource;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private TextEdit serverNameTextEdit;

	private LayoutControlItem serverNameTextEditLayoutControlItem;

	private EmptySpaceItem bottomEmptySpaceItem;

	private TextEdit portTextEdit;

	private LayoutControlItem portTextEditLayoutControlItem;

	private LookUpEdit authenticationLookUpEdit;

	private LayoutControlItem authenticationLookUpEditLayoutControlItem;

	private TextEdit loginTextEdit;

	private LayoutControlItem loginTextEditLayoutControlItem;

	private TextEdit passwordTextEdit;

	private LayoutControlItem passwordTextEditLayoutControlItem;

	private CheckEdit passwordCheckEdit;

	private LayoutControlItem passwordCheckEditLayoutControlItem;

	private ButtonEdit databaseButtonEdit;

	private LayoutControlItem databaseButtonEditLayoutControlItem;

	private LookUpEdit connectionModeLookUpEdit;

	private LayoutControlItem connectionModeLookUpEditLayoutControlItem;

	private LabelControl defaultPortLabelControl;

	private LayoutControlItem defaultPortLabelControlLayoutControlItem;

	private LabelControl detailsLabelControl;

	private LayoutControlItem detailsLabelControlLayoutControlItem;

	private DXErrorProvider dxErrorProvider;

	private ToolTipController toolTipController;

	private ProgressPanel progressPanel;

	private LayoutControlItem progressPanelLayoutControlItem;

	private SimpleButton cancelGettingDatabasesSimpleButton;

	private LayoutControlItem cancelGettingDatabasesSimpleButtonLayoutControlItem;

	private LabelControl repositoryValueLabelControl;

	private LabelControl repositoryHeaderLabelControl;

	private LayoutControlItem repositoryHeaderLayoutControlItem;

	private LayoutControlItem repositoryValueLayoutControlItem;

	public string PreparedConnectionString { get; private set; }

	public string Database => databaseButtonEdit.Text;

	public DatabaseType RepositoryType => DatabaseType.SqlServer;

	public string ServerName => serverNameTextEdit.Text;

	public string Login => loginTextEdit.Text;

	public string Password => passwordTextEdit.Text;

	public bool SavePassword => passwordCheckEdit.Enabled = passwordCheckEdit.Checked;

	public AuthenticationType.AuthenticationTypeEnum AuthenticationType => Dataedo.Shared.Enums.AuthenticationType.GetTypeByIndex(Authentication.GetIndex(authenticationLookUpEdit));

	public int? Port => PrepareValue.ToInt(portTextEdit.Text);

	public SqlServerConnectionModeEnum.SqlServerConnectionMode? SqlServerConnectionMode => SqlServerConnectionModeEnum.StringToTypeOrDefault(connectionModeLookUpEdit.EditValue as string);

	public bool HasStandardFieldsCompleted
	{
		get
		{
			if (!string.IsNullOrWhiteSpace(ServerName))
			{
				return !string.IsNullOrWhiteSpace(Database);
			}
			return false;
		}
	}

	public bool HasLoginFieldsCompleted
	{
		get
		{
			if (!string.IsNullOrEmpty(Login))
			{
				if (!Dataedo.Shared.Enums.AuthenticationType.IsNoPasswordAuthentication(AuthenticationType))
				{
					return !string.IsNullOrEmpty(Password);
				}
				return true;
			}
			return false;
		}
	}

	public bool AreDetailsShown
	{
		get
		{
			return areDetailsShown;
		}
		protected set
		{
			if (value != areDetailsShown)
			{
				areDetailsShown = value;
				this.DetailsVisibilityChanged?.Invoke(this, EventArgs.Empty);
			}
		}
	}

	public bool IsRepositoryLocationValid
	{
		get
		{
			if (!string.IsNullOrEmpty(databaseButtonEdit.Text))
			{
				return !string.IsNullOrEmpty(serverNameTextEdit.Text);
			}
			return false;
		}
	}

	public string RepositoryLocation => databaseButtonEdit.Text + "@" + serverNameTextEdit.Text;

	public bool AllowEnterNextAction
	{
		get
		{
			if (!serverNameTextEdit.ContainsFocus && !portTextEdit.ContainsFocus && !loginTextEdit.ContainsFocus && !passwordTextEdit.ContainsFocus)
			{
				return databaseButtonEdit.ContainsFocus;
			}
			return true;
		}
	}

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public event EventHandler DetailsVisibilityChanged;

	public ConnectToSqlServerRepositoryUserControl()
	{
		InitializeComponent();
		baseSize = base.Size;
		Dataedo.App.Data.MetadataServer.SqlServerConnectionMode.SetSqlServerConnectionMode(connectionModeLookUpEdit);
		connectionModeLookUpEdit.EditValue = SqlServerConnectionModeEnum.DefaultModeString;
		Authentication.CreateAuthenticationDataSource();
		Authentication.SetAuthenticationDataSource(authenticationLookUpEdit);
	}

	internal void SetParameter()
	{
		ClearData();
		ShowHideConnectionDetails(visible: true);
	}

	internal void PrepareConnectionString(out RepositoryExistsStatusEnum? connectionResult, out Exception exception)
	{
		PreparedConnectionString = GetConnectionString(out connectionResult, out exception);
		if (connectionResult == RepositoryExistsStatusEnum.NotExists)
		{
			ShowHideConnectionDetails(visible: true);
		}
	}

	internal string GetConnectionString(out RepositoryExistsStatusEnum? connectionResult, out Exception exception)
	{
		return SqlServerConnectionStringTools.GetConnectionStringForConnection(Database, RepositoryType, ServerName, Login, Password, AuthenticationType, Port, SqlServerConnectionMode, out connectionResult, out exception, showMessages: false);
	}

	internal RepositoryExistsStatusEnum CheckIfRepositoryExists(ref Exception exception)
	{
		return LoginHelper.CheckIfRepositoryExists(PreparedConnectionString, Database, ref exception);
	}

	internal bool CheckIfRepositoryIsValid()
	{
		return LicenseHelper.CheckIfRepositoryIsValid(PreparedConnectionString, Database);
	}

	internal void AddRecentRecord()
	{
		LastConnectionInfo.SetValues(ServerName, Database, Login, AuthenticationType, SqlServerConnectionModeEnum.TypeToString(SqlServerConnectionMode), Port);
		LastConnectionInfo.Password = passwordTextEdit.Text;
		RecentProjectsHelper.Add(RecentSupport.GetRecentProject(this));
	}

	internal void SetRecentData(RecentItemModel recentItemModel, bool validate)
	{
		if (recentItemModel?.Data != null)
		{
			dxErrorProvider.ClearErrors();
			serverNameTextEdit.Text = recentItemModel.Data.Server;
			databaseButtonEdit.Text = recentItemModel.Data.Database;
			repositoryValueLabelControl.Text = recentItemModel.DisplayName;
			portTextEdit.Text = (string.IsNullOrEmpty(recentItemModel.Data.Port.ToString()) ? DatabaseTypeEnum.GetDefaultPort(SharedDatabaseTypeEnum.DatabaseType.SqlServer) : recentItemModel.Data.Port.ToString());
			authenticationLookUpEdit.EditValue = Dataedo.Shared.Enums.AuthenticationType.GetLookupIndex(Dataedo.Shared.Enums.AuthenticationType.StringToType(recentItemModel.Data.AuthenticationType));
			if (Authentication.IsWindowsAuthentication(recentItemModel.Data.AuthenticationType) || Authentication.IsActiveDirectoryIntegrated(recentItemModel.Data.AuthenticationType) || Authentication.IsActiveDirectoryInteractive(recentItemModel.Data.AuthenticationType))
			{
				CheckEdit checkEdit = passwordCheckEdit;
				bool enabled = (passwordCheckEdit.Checked = false);
				checkEdit.Enabled = enabled;
			}
			else
			{
				loginTextEdit.Text = recentItemModel.Data.Login;
				SetPassword(recentItemModel.Data.Password);
				if (string.IsNullOrEmpty(passwordTextEdit.Text))
				{
					passwordTextEdit.Focus();
				}
				passwordCheckEdit.Checked = recentItemModel.Data.IsPasswordSaved;
				passwordCheckEdit.Enabled = !Authentication.IsWindowsAuthentication(authenticationLookUpEdit) && !Authentication.IsActiveDirectoryIntegrated(authenticationLookUpEdit);
			}
			connectionModeLookUpEdit.EditValue = SqlServerConnectionModeEnum.StringParsedOrDefault(recentItemModel.Data.ConnectionMode);
			if (validate)
			{
				ValidateRequiredFields();
			}
			else
			{
				SetFocus();
			}
		}
		else
		{
			ClearData();
			SetFocus();
		}
	}

	internal bool SetFocus()
	{
		if (!ValidateFields.FocusIfIsStringNullOrEmpty(serverNameTextEdit) && !ValidateFields.FocusIfIsStringNullOrEmpty(portTextEdit) && !ValidateFields.FocusIfIsStringNullOrEmpty(authenticationLookUpEdit) && !ValidateFields.FocusIfIsStringNullOrEmpty(loginTextEdit) && !ValidateFields.FocusIfIsStringNullOrEmpty(passwordTextEdit) && !ValidateFields.FocusIfIsStringNullOrEmpty(databaseButtonEdit))
		{
			return ValidateFields.FocusIfIsStringNullOrEmpty(connectionModeLookUpEdit);
		}
		return true;
	}

	internal bool ValidateRequiredFields()
	{
		return ValidateRequiredFields(validateDatabase: true);
	}

	internal void ShowHideConnectionDetails()
	{
		if (HasStandardFieldsCompleted)
		{
			ShowHideConnectionDetails(visible: false);
		}
	}

	internal void CancelAsyncOperations()
	{
		CancelGettingDatabases();
	}

	protected override void Dispose(bool disposing)
	{
		gettingDatabasesCancellationTokenSource?.Dispose();
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void CancelGettingDatabases()
	{
		try
		{
			gettingDatabasesCancellationTokenSource?.Cancel();
		}
		catch (Exception)
		{
		}
	}

	private bool ValidateRequiredFields(bool validateDatabase)
	{
		bool flag = true;
		flag &= ValidateServerName(flag);
		if (validateDatabase)
		{
			flag &= ValidateDatabase(flag);
		}
		if (!Authentication.IsWindowsAuthentication(authenticationLookUpEdit) && !Authentication.IsActiveDirectoryIntegrated(authenticationLookUpEdit) && !Authentication.IsActiveDirectoryInteractive(authenticationLookUpEdit))
		{
			flag &= ValidateLogin(flag);
		}
		return flag & ValidatePort(flag);
	}

	private void ClearData()
	{
		dxErrorProvider.ClearErrors();
		serverNameTextEdit.Text = null;
		databaseButtonEdit.Text = null;
		portTextEdit.Text = null;
		authenticationLookUpEdit.EditValue = Dataedo.Shared.Enums.AuthenticationType.GetLookupIndex(Dataedo.Shared.Enums.AuthenticationType.AuthenticationTypeEnum.StandardAuthentication);
		passwordCheckEdit.Checked = false;
		loginTextEdit.Text = null;
		SetPassword(null);
		connectionModeLookUpEdit.EditValue = SqlServerConnectionModeEnum.StringParsedOrDefault(SqlServerConnectionModeEnum.DefaultModeString);
		ShowHideConnectionDetails(visible: true);
	}

	private bool ValidateServerName(bool setFocusIfNotValid)
	{
		bool num = ValidateFields.ValidateEdit(serverNameTextEdit, dxErrorProvider, "host");
		if (!num)
		{
			ShowHideConnectionDetails(visible: true);
			if (setFocusIfNotValid)
			{
				serverNameTextEdit.Focus();
			}
		}
		return num;
	}

	private bool ValidateDatabase(bool setFocusIfNotValid)
	{
		bool num = ValidateFields.ValidateEdit(databaseButtonEdit, dxErrorProvider, "database");
		if (!num)
		{
			ShowHideConnectionDetails(visible: true);
			if (setFocusIfNotValid)
			{
				databaseButtonEdit.Focus();
			}
		}
		return num;
	}

	private bool ValidateLogin(bool setFocusIfNotValid)
	{
		bool num = ValidateFields.ValidateEdit(loginTextEdit, dxErrorProvider, "login");
		if (!num && setFocusIfNotValid)
		{
			loginTextEdit.Focus();
		}
		return num;
	}

	private bool ValidatePort(bool setFocusIfNotValid)
	{
		bool num = ValidateFields.ValidateEdit(portTextEdit, dxErrorProvider, "port");
		if (!num)
		{
			ShowHideConnectionDetails(visible: true);
			if (setFocusIfNotValid)
			{
				portTextEdit.Focus();
			}
		}
		return num;
	}

	private void ConnectToSqlServerRepositoryUserControl_Load(object sender, EventArgs e)
	{
		base.LoaderContext = base.Parent;
	}

	private void DefaultPortLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		portTextEdit.Text = DatabaseTypeEnum.GetDefaultPort(SharedDatabaseTypeEnum.DatabaseType.SqlServer);
	}

	private void SetPassword(string password)
	{
		try
		{
			passwordTextEdit.Text = new SimpleAES().DecryptString(password);
		}
		catch
		{
			passwordTextEdit.Text = string.Empty;
		}
	}

	private void DetailsLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		ShowHideConnectionDetails(!AreDetailsShown);
	}

	private void ShowHideConnectionDetails(bool visible)
	{
		AreDetailsShown = visible;
		try
		{
			SuspendLayout();
			LayoutVisibility visibility = ((!visible) ? LayoutVisibility.Never : LayoutVisibility.Always);
			serverNameTextEditLayoutControlItem.Visibility = visibility;
			portTextEditLayoutControlItem.Visibility = visibility;
			defaultPortLabelControlLayoutControlItem.Visibility = visibility;
			authenticationLookUpEditLayoutControlItem.Visibility = visibility;
			databaseButtonEditLayoutControlItem.Visibility = visibility;
			connectionModeLookUpEditLayoutControlItem.Visibility = visibility;
			detailsLabelControl.Text = (visible ? "<href>Hide details</href>" : "<href>Show details</href>");
			base.Size = (visible ? baseSize : new Size(baseSize.Width, 200));
		}
		finally
		{
			ResumeLayout(performLayout: false);
		}
	}

	private void AuthenticationLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		bool flag = Authentication.IsWindowsAuthentication(authenticationLookUpEdit) || Authentication.IsActiveDirectoryIntegrated(authenticationLookUpEdit);
		LayoutControlItem layoutControlItem = loginTextEditLayoutControlItem;
		LayoutControlItem layoutControlItem2 = passwordTextEditLayoutControlItem;
		bool flag3 = (passwordCheckEdit.Enabled = !flag);
		bool enabled = (layoutControlItem2.Enabled = flag3);
		layoutControlItem.Enabled = enabled;
		if (databaseButtonEdit.Properties.Buttons.Any())
		{
			databaseButtonEdit.Properties.Buttons[0].Visible = true;
		}
		if (flag)
		{
			lastProvidedLogin = loginTextEdit.Text;
			loginTextEdit.Text = WindowsIdentity.GetCurrent().Name;
			lastProvidedSavePassword = passwordCheckEdit.Checked;
			lastProvidedPassword = passwordTextEdit.Text;
			CheckEdit checkEdit = passwordCheckEdit;
			enabled = (passwordCheckEdit.Enabled = false);
			checkEdit.Checked = enabled;
			passwordTextEdit.Text = string.Empty;
		}
		else if (Authentication.IsActiveDirectoryInteractive(authenticationLookUpEdit))
		{
			lastProvidedSavePassword = passwordCheckEdit.Checked;
			lastProvidedPassword = passwordTextEdit.Text;
			CheckEdit checkEdit2 = passwordCheckEdit;
			CheckEdit checkEdit3 = passwordCheckEdit;
			flag3 = (passwordTextEditLayoutControlItem.Enabled = false);
			enabled = (checkEdit3.Enabled = flag3);
			checkEdit2.Checked = enabled;
			passwordTextEdit.Text = string.Empty;
			if (databaseButtonEdit.Properties.Buttons.Any())
			{
				databaseButtonEdit.Properties.Buttons[0].Visible = false;
			}
		}
		else
		{
			loginTextEdit.Text = lastProvidedLogin;
			passwordCheckEdit.Enabled = true;
			passwordCheckEdit.Checked = lastProvidedSavePassword;
			passwordTextEdit.Text = lastProvidedPassword;
		}
	}

	private async void DatabaseButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		dxErrorProvider.ClearErrors();
		if (!ValidateRequiredFields(validateDatabase: false))
		{
			return;
		}
		try
		{
			using (gettingDatabasesCancellationTokenSource = new CancellationTokenSource())
			{
				databaseButtonEdit.Enabled = false;
				progressPanelLayoutControlItem.Visibility = LayoutVisibility.Always;
				cancelGettingDatabasesSimpleButtonLayoutControlItem.Visibility = LayoutVisibility.Always;
				IEnumerable<string> enumerable = await GetRepositoriesAsync(await SqlServerConnectionStringTools.GetConnectionStringForConnectionAsync(Database, RepositoryType, ServerName, Login, Password, AuthenticationType, Port, SqlServerConnectionMode, showMessages: false, gettingDatabasesCancellationTokenSource.Token), gettingDatabasesCancellationTokenSource.Token);
				if (enumerable == null)
				{
					return;
				}
				using ListForm listForm = new ListForm(enumerable.ToList(), "Available databases");
				databaseButtonEdit.Enabled = true;
				cancelGettingDatabasesSimpleButtonLayoutControlItem.Visibility = LayoutVisibility.Never;
				progressPanelLayoutControlItem.Visibility = LayoutVisibility.Never;
				if (listForm.ShowDialog(this, setCustomMessageDefaultOwner: true) != DialogResult.OK)
				{
					return;
				}
				databaseButtonEdit.Text = listForm.SelectedValue;
			}
		}
		finally
		{
			databaseButtonEdit.Enabled = true;
			cancelGettingDatabasesSimpleButtonLayoutControlItem.Visibility = LayoutVisibility.Never;
			progressPanelLayoutControlItem.Visibility = LayoutVisibility.Never;
		}
	}

	private async Task<IEnumerable<string>> GetRepositoriesAsync(string connectionString, CancellationToken cancellationToken)
	{
		_ = 1;
		try
		{
			List<string> result = new List<string>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				await connection.OpenAsync(cancellationToken);
				string cmdText = "SELECT [name]\r\n                                    FROM [sys].[databases]\r\n                                    WHERE [state] = 0\r\n                                        AND is_read_only = 0\r\n                                        AND [database_id] > 4\r\n                                    ORDER BY [name];";
				using SqlCommand command = new SqlCommand(cmdText, connection);
				using SqlDataReader sqlDataReader = await (command?.ExecuteReaderAsync(cancellationToken));
				while (sqlDataReader != null && sqlDataReader.Read())
				{
					if (sqlDataReader[0] != null && !(sqlDataReader[0] is DBNull))
					{
						result.Add(sqlDataReader[0]?.ToString());
					}
				}
			}
			return result;
		}
		catch (TaskCanceledException)
		{
			return null;
		}
		catch (Exception ex2)
		{
			GeneralMessageBoxesHandling.Show(ex2.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, base.ParentForm);
			return null;
		}
	}

	private void CancelGettingDatabasesSimpleButton_Click(object sender, EventArgs e)
	{
		CancelGettingDatabases();
	}

	internal void SetRepositoryLayoutItemVisibility(bool visible)
	{
		LayoutVisibility layoutVisibility3 = (repositoryHeaderLayoutControlItem.Visibility = (repositoryValueLayoutControlItem.Visibility = ((!visible) ? LayoutVisibility.Never : LayoutVisibility.Always)));
		if (!visible)
		{
			repositoryValueLabelControl.Text = string.Empty;
		}
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.repositoryValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.repositoryHeaderLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.cancelGettingDatabasesSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.progressPanel = new DevExpress.XtraWaitForm.ProgressPanel();
		this.detailsLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.defaultPortLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.connectionModeLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.databaseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.passwordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.passwordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.loginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.authenticationLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.portTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.serverNameTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.serverNameTextEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.bottomEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.portTextEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.authenticationLookUpEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.loginTextEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.passwordTextEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.passwordCheckEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.databaseButtonEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.connectionModeLookUpEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.defaultPortLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.detailsLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.progressPanelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cancelGettingDatabasesSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.repositoryHeaderLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.repositoryValueLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.dxErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.connectionModeLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.loginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.authenticationLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.portTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.serverNameTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.serverNameTextEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.portTextEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.authenticationLookUpEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.loginTextEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordCheckEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseButtonEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectionModeLookUpEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.defaultPortLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.detailsLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.progressPanelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cancelGettingDatabasesSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryHeaderLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryValueLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dxErrorProvider).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.repositoryValueLabelControl);
		this.mainLayoutControl.Controls.Add(this.repositoryHeaderLabelControl);
		this.mainLayoutControl.Controls.Add(this.cancelGettingDatabasesSimpleButton);
		this.mainLayoutControl.Controls.Add(this.progressPanel);
		this.mainLayoutControl.Controls.Add(this.detailsLabelControl);
		this.mainLayoutControl.Controls.Add(this.defaultPortLabelControl);
		this.mainLayoutControl.Controls.Add(this.connectionModeLookUpEdit);
		this.mainLayoutControl.Controls.Add(this.databaseButtonEdit);
		this.mainLayoutControl.Controls.Add(this.passwordCheckEdit);
		this.mainLayoutControl.Controls.Add(this.passwordTextEdit);
		this.mainLayoutControl.Controls.Add(this.loginTextEdit);
		this.mainLayoutControl.Controls.Add(this.authenticationLookUpEdit);
		this.mainLayoutControl.Controls.Add(this.portTextEdit);
		this.mainLayoutControl.Controls.Add(this.serverNameTextEdit);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2931, 511, 733, 650);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "mainLayoutControl";
		this.mainLayoutControl.ToolTipController = this.toolTipController;
		this.repositoryValueLabelControl.Location = new System.Drawing.Point(102, 2);
		this.repositoryValueLabelControl.Name = "repositoryValueLabelControl";
		this.repositoryValueLabelControl.Padding = new System.Windows.Forms.Padding(2, 0, 0, 0);
		this.repositoryValueLabelControl.Size = new System.Drawing.Size(596, 23);
		this.repositoryValueLabelControl.StyleController = this.mainLayoutControl;
		this.repositoryValueLabelControl.TabIndex = 53;
		this.repositoryHeaderLabelControl.Location = new System.Drawing.Point(2, 2);
		this.repositoryHeaderLabelControl.MaximumSize = new System.Drawing.Size(98, 0);
		this.repositoryHeaderLabelControl.MinimumSize = new System.Drawing.Size(98, 0);
		this.repositoryHeaderLabelControl.Name = "repositoryHeaderLabelControl";
		this.repositoryHeaderLabelControl.Size = new System.Drawing.Size(98, 23);
		this.repositoryHeaderLabelControl.StyleController = this.mainLayoutControl;
		this.repositoryHeaderLabelControl.TabIndex = 52;
		this.repositoryHeaderLabelControl.Text = "Repository:";
		this.cancelGettingDatabasesSimpleButton.Location = new System.Drawing.Point(442, 191);
		this.cancelGettingDatabasesSimpleButton.MaximumSize = new System.Drawing.Size(50, 0);
		this.cancelGettingDatabasesSimpleButton.Name = "cancelGettingDatabasesSimpleButton";
		this.cancelGettingDatabasesSimpleButton.Size = new System.Drawing.Size(50, 20);
		this.cancelGettingDatabasesSimpleButton.StyleController = this.mainLayoutControl;
		this.cancelGettingDatabasesSimpleButton.TabIndex = 49;
		this.cancelGettingDatabasesSimpleButton.Text = "Cancel";
		this.cancelGettingDatabasesSimpleButton.Click += new System.EventHandler(CancelGettingDatabasesSimpleButton_Click);
		this.progressPanel.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.progressPanel.Appearance.Options.UseBackColor = true;
		this.progressPanel.Location = new System.Drawing.Point(413, 193);
		this.progressPanel.Margin = new System.Windows.Forms.Padding(0);
		this.progressPanel.MaximumSize = new System.Drawing.Size(25, 16);
		this.progressPanel.MinimumSize = new System.Drawing.Size(25, 16);
		this.progressPanel.Name = "progressPanel";
		this.progressPanel.ShowCaption = false;
		this.progressPanel.ShowDescription = false;
		this.progressPanel.Size = new System.Drawing.Size(25, 16);
		this.progressPanel.StyleController = this.mainLayoutControl;
		this.progressPanel.TabIndex = 48;
		this.progressPanel.Visible = false;
		this.detailsLabelControl.AllowHtmlString = true;
		this.detailsLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
		this.detailsLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.detailsLabelControl.Location = new System.Drawing.Point(99, 253);
		this.detailsLabelControl.Name = "detailsLabelControl";
		this.detailsLabelControl.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
		this.detailsLabelControl.Size = new System.Drawing.Size(65, 13);
		this.detailsLabelControl.StyleController = this.mainLayoutControl;
		this.detailsLabelControl.TabIndex = 47;
		this.detailsLabelControl.Text = "<href>Hide details</href>";
		this.detailsLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(DetailsLabelControl_HyperlinkClick);
		this.defaultPortLabelControl.AllowHtmlString = true;
		this.defaultPortLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.defaultPortLabelControl.Location = new System.Drawing.Point(413, 59);
		this.defaultPortLabelControl.Name = "defaultPortLabelControl";
		this.defaultPortLabelControl.Size = new System.Drawing.Size(35, 13);
		this.defaultPortLabelControl.StyleController = this.mainLayoutControl;
		this.defaultPortLabelControl.TabIndex = 46;
		this.defaultPortLabelControl.Text = "<href>Default</href>";
		this.defaultPortLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(DefaultPortLabelControl_HyperlinkClick);
		this.connectionModeLookUpEdit.Location = new System.Drawing.Point(99, 218);
		this.connectionModeLookUpEdit.MaximumSize = new System.Drawing.Size(300, 20);
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
		this.connectionModeLookUpEdit.Size = new System.Drawing.Size(300, 20);
		this.connectionModeLookUpEdit.StyleController = this.mainLayoutControl;
		this.connectionModeLookUpEdit.TabIndex = 45;
		this.connectionModeLookUpEdit.ToolTipController = this.toolTipController;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.databaseButtonEdit.Location = new System.Drawing.Point(99, 191);
		this.databaseButtonEdit.MaximumSize = new System.Drawing.Size(300, 20);
		this.databaseButtonEdit.Name = "databaseButtonEdit";
		this.databaseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.databaseButtonEdit.Size = new System.Drawing.Size(300, 20);
		this.databaseButtonEdit.StyleController = this.mainLayoutControl;
		this.databaseButtonEdit.TabIndex = 44;
		this.databaseButtonEdit.ToolTipController = this.toolTipController;
		this.databaseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(DatabaseButtonEdit_ButtonClick);
		this.passwordCheckEdit.Location = new System.Drawing.Point(99, 164);
		this.passwordCheckEdit.Margin = new System.Windows.Forms.Padding(0);
		this.passwordCheckEdit.MaximumSize = new System.Drawing.Size(100, 0);
		this.passwordCheckEdit.Name = "passwordCheckEdit";
		this.passwordCheckEdit.Properties.Caption = "Save password";
		this.passwordCheckEdit.Size = new System.Drawing.Size(100, 20);
		this.passwordCheckEdit.StyleController = this.mainLayoutControl;
		this.passwordCheckEdit.TabIndex = 43;
		this.passwordTextEdit.Location = new System.Drawing.Point(99, 137);
		this.passwordTextEdit.MaximumSize = new System.Drawing.Size(300, 20);
		this.passwordTextEdit.Name = "passwordTextEdit";
		this.passwordTextEdit.Properties.UseSystemPasswordChar = true;
		this.passwordTextEdit.Size = new System.Drawing.Size(300, 20);
		this.passwordTextEdit.StyleController = this.mainLayoutControl;
		this.passwordTextEdit.TabIndex = 42;
		this.passwordTextEdit.ToolTipController = this.toolTipController;
		this.loginTextEdit.Location = new System.Drawing.Point(99, 110);
		this.loginTextEdit.MaximumSize = new System.Drawing.Size(300, 20);
		this.loginTextEdit.Name = "loginTextEdit";
		this.loginTextEdit.Size = new System.Drawing.Size(300, 20);
		this.loginTextEdit.StyleController = this.mainLayoutControl;
		this.loginTextEdit.TabIndex = 41;
		this.loginTextEdit.ToolTipController = this.toolTipController;
		this.authenticationLookUpEdit.Location = new System.Drawing.Point(99, 83);
		this.authenticationLookUpEdit.MaximumSize = new System.Drawing.Size(300, 20);
		this.authenticationLookUpEdit.Name = "authenticationLookUpEdit";
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
		this.authenticationLookUpEdit.Size = new System.Drawing.Size(300, 20);
		this.authenticationLookUpEdit.StyleController = this.mainLayoutControl;
		this.authenticationLookUpEdit.TabIndex = 40;
		this.authenticationLookUpEdit.ToolTipController = this.toolTipController;
		this.authenticationLookUpEdit.EditValueChanged += new System.EventHandler(AuthenticationLookUpEdit_EditValueChanged);
		this.portTextEdit.Location = new System.Drawing.Point(99, 56);
		this.portTextEdit.MaximumSize = new System.Drawing.Size(300, 20);
		this.portTextEdit.MinimumSize = new System.Drawing.Size(300, 20);
		this.portTextEdit.Name = "portTextEdit";
		this.portTextEdit.Properties.Mask.EditMask = "\\d+";
		this.portTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.portTextEdit.Properties.Mask.ShowPlaceHolders = false;
		this.portTextEdit.Properties.MaxLength = 5;
		this.portTextEdit.Size = new System.Drawing.Size(300, 20);
		this.portTextEdit.StyleController = this.mainLayoutControl;
		this.portTextEdit.TabIndex = 5;
		this.portTextEdit.ToolTipController = this.toolTipController;
		this.serverNameTextEdit.Location = new System.Drawing.Point(99, 29);
		this.serverNameTextEdit.MaximumSize = new System.Drawing.Size(300, 20);
		this.serverNameTextEdit.Name = "serverNameTextEdit";
		this.serverNameTextEdit.Size = new System.Drawing.Size(300, 20);
		this.serverNameTextEdit.StyleController = this.mainLayoutControl;
		this.serverNameTextEdit.TabIndex = 4;
		this.serverNameTextEdit.ToolTipController = this.toolTipController;
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[15]
		{
			this.serverNameTextEditLayoutControlItem, this.bottomEmptySpaceItem, this.portTextEditLayoutControlItem, this.authenticationLookUpEditLayoutControlItem, this.loginTextEditLayoutControlItem, this.passwordTextEditLayoutControlItem, this.passwordCheckEditLayoutControlItem, this.databaseButtonEditLayoutControlItem, this.connectionModeLookUpEditLayoutControlItem, this.defaultPortLabelControlLayoutControlItem,
			this.detailsLabelControlLayoutControlItem, this.progressPanelLayoutControlItem, this.cancelGettingDatabasesSimpleButtonLayoutControlItem, this.repositoryHeaderLayoutControlItem, this.repositoryValueLayoutControlItem
		});
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.OptionsItemText.TextToControlDistance = 10;
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControlGroup.TextVisible = false;
		this.serverNameTextEditLayoutControlItem.Control = this.serverNameTextEdit;
		this.serverNameTextEditLayoutControlItem.Location = new System.Drawing.Point(0, 27);
		this.serverNameTextEditLayoutControlItem.Name = "serverNameTextEditLayoutControlItem";
		this.serverNameTextEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.serverNameTextEditLayoutControlItem.Size = new System.Drawing.Size(700, 27);
		this.serverNameTextEditLayoutControlItem.Text = "Server name:";
		this.serverNameTextEditLayoutControlItem.TextSize = new System.Drawing.Size(87, 13);
		this.bottomEmptySpaceItem.AllowHotTrack = false;
		this.bottomEmptySpaceItem.Location = new System.Drawing.Point(0, 271);
		this.bottomEmptySpaceItem.MinSize = new System.Drawing.Size(10, 1);
		this.bottomEmptySpaceItem.Name = "bottomEmptySpaceItem";
		this.bottomEmptySpaceItem.Size = new System.Drawing.Size(700, 199);
		this.bottomEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.bottomEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.portTextEditLayoutControlItem.Control = this.portTextEdit;
		this.portTextEditLayoutControlItem.Location = new System.Drawing.Point(0, 54);
		this.portTextEditLayoutControlItem.Name = "portTextEditLayoutControlItem";
		this.portTextEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.portTextEditLayoutControlItem.Size = new System.Drawing.Size(401, 27);
		this.portTextEditLayoutControlItem.Text = "Port:";
		this.portTextEditLayoutControlItem.TextSize = new System.Drawing.Size(87, 13);
		this.authenticationLookUpEditLayoutControlItem.Control = this.authenticationLookUpEdit;
		this.authenticationLookUpEditLayoutControlItem.Location = new System.Drawing.Point(0, 81);
		this.authenticationLookUpEditLayoutControlItem.Name = "authenticationLookUpEditLayoutControlItem";
		this.authenticationLookUpEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.authenticationLookUpEditLayoutControlItem.Size = new System.Drawing.Size(700, 27);
		this.authenticationLookUpEditLayoutControlItem.Text = "Authentication:";
		this.authenticationLookUpEditLayoutControlItem.TextSize = new System.Drawing.Size(87, 13);
		this.loginTextEditLayoutControlItem.Control = this.loginTextEdit;
		this.loginTextEditLayoutControlItem.Location = new System.Drawing.Point(0, 108);
		this.loginTextEditLayoutControlItem.Name = "loginTextEditLayoutControlItem";
		this.loginTextEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.loginTextEditLayoutControlItem.Size = new System.Drawing.Size(700, 27);
		this.loginTextEditLayoutControlItem.Text = "Login:";
		this.loginTextEditLayoutControlItem.TextSize = new System.Drawing.Size(87, 13);
		this.passwordTextEditLayoutControlItem.Control = this.passwordTextEdit;
		this.passwordTextEditLayoutControlItem.Location = new System.Drawing.Point(0, 135);
		this.passwordTextEditLayoutControlItem.Name = "passwordTextEditLayoutControlItem";
		this.passwordTextEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.passwordTextEditLayoutControlItem.Size = new System.Drawing.Size(700, 27);
		this.passwordTextEditLayoutControlItem.Text = "Password:";
		this.passwordTextEditLayoutControlItem.TextSize = new System.Drawing.Size(87, 13);
		this.passwordCheckEditLayoutControlItem.Control = this.passwordCheckEdit;
		this.passwordCheckEditLayoutControlItem.CustomizationFormText = "Save password:";
		this.passwordCheckEditLayoutControlItem.Location = new System.Drawing.Point(0, 162);
		this.passwordCheckEditLayoutControlItem.Name = "passwordCheckEditLayoutControlItem";
		this.passwordCheckEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.passwordCheckEditLayoutControlItem.Size = new System.Drawing.Size(700, 27);
		this.passwordCheckEditLayoutControlItem.Text = " ";
		this.passwordCheckEditLayoutControlItem.TextSize = new System.Drawing.Size(87, 13);
		this.databaseButtonEditLayoutControlItem.Control = this.databaseButtonEdit;
		this.databaseButtonEditLayoutControlItem.Location = new System.Drawing.Point(0, 189);
		this.databaseButtonEditLayoutControlItem.Name = "databaseButtonEditLayoutControlItem";
		this.databaseButtonEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.databaseButtonEditLayoutControlItem.Size = new System.Drawing.Size(401, 27);
		this.databaseButtonEditLayoutControlItem.Text = "Database:";
		this.databaseButtonEditLayoutControlItem.TextSize = new System.Drawing.Size(87, 13);
		this.connectionModeLookUpEditLayoutControlItem.Control = this.connectionModeLookUpEdit;
		this.connectionModeLookUpEditLayoutControlItem.Location = new System.Drawing.Point(0, 216);
		this.connectionModeLookUpEditLayoutControlItem.Name = "connectionModeLookUpEditLayoutControlItem";
		this.connectionModeLookUpEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.connectionModeLookUpEditLayoutControlItem.Size = new System.Drawing.Size(700, 27);
		this.connectionModeLookUpEditLayoutControlItem.Text = "Connection mode:";
		this.connectionModeLookUpEditLayoutControlItem.TextSize = new System.Drawing.Size(87, 13);
		this.defaultPortLabelControlLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.defaultPortLabelControlLayoutControlItem.Control = this.defaultPortLabelControl;
		this.defaultPortLabelControlLayoutControlItem.Location = new System.Drawing.Point(401, 54);
		this.defaultPortLabelControlLayoutControlItem.Name = "defaultPortLabelControlLayoutControlItem";
		this.defaultPortLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 2, 2, 5);
		this.defaultPortLabelControlLayoutControlItem.Size = new System.Drawing.Size(299, 27);
		this.defaultPortLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.defaultPortLabelControlLayoutControlItem.TextVisible = false;
		this.detailsLabelControlLayoutControlItem.Control = this.detailsLabelControl;
		this.detailsLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 243);
		this.detailsLabelControlLayoutControlItem.Name = "detailsLabelControlLayoutControlItem";
		this.detailsLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 10, 5);
		this.detailsLabelControlLayoutControlItem.Size = new System.Drawing.Size(700, 28);
		this.detailsLabelControlLayoutControlItem.Text = " ";
		this.detailsLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(87, 13);
		this.progressPanelLayoutControlItem.Control = this.progressPanel;
		this.progressPanelLayoutControlItem.CustomizationFormText = "progressPanelLayoutControlItem";
		this.progressPanelLayoutControlItem.Location = new System.Drawing.Point(401, 189);
		this.progressPanelLayoutControlItem.MinSize = new System.Drawing.Size(39, 25);
		this.progressPanelLayoutControlItem.Name = "progressPanelLayoutControlItem";
		this.progressPanelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 12, 4, 5);
		this.progressPanelLayoutControlItem.Size = new System.Drawing.Size(39, 27);
		this.progressPanelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.progressPanelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.progressPanelLayoutControlItem.TextVisible = false;
		this.progressPanelLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.cancelGettingDatabasesSimpleButtonLayoutControlItem.Control = this.cancelGettingDatabasesSimpleButton;
		this.cancelGettingDatabasesSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(440, 189);
		this.cancelGettingDatabasesSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(78, 26);
		this.cancelGettingDatabasesSimpleButtonLayoutControlItem.Name = "cancelGettingDatabasesSimpleButtonLayoutControlItem";
		this.cancelGettingDatabasesSimpleButtonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.cancelGettingDatabasesSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(260, 27);
		this.cancelGettingDatabasesSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cancelGettingDatabasesSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.cancelGettingDatabasesSimpleButtonLayoutControlItem.TextVisible = false;
		this.cancelGettingDatabasesSimpleButtonLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.repositoryHeaderLayoutControlItem.Control = this.repositoryHeaderLabelControl;
		this.repositoryHeaderLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.repositoryHeaderLayoutControlItem.MaxSize = new System.Drawing.Size(100, 27);
		this.repositoryHeaderLayoutControlItem.MinSize = new System.Drawing.Size(100, 27);
		this.repositoryHeaderLayoutControlItem.Name = "repositoryHeaderLayoutControlItem";
		this.repositoryHeaderLayoutControlItem.Size = new System.Drawing.Size(100, 27);
		this.repositoryHeaderLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.repositoryHeaderLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.repositoryHeaderLayoutControlItem.TextVisible = false;
		this.repositoryValueLayoutControlItem.Control = this.repositoryValueLabelControl;
		this.repositoryValueLayoutControlItem.Location = new System.Drawing.Point(100, 0);
		this.repositoryValueLayoutControlItem.MaxSize = new System.Drawing.Size(0, 27);
		this.repositoryValueLayoutControlItem.MinSize = new System.Drawing.Size(1, 27);
		this.repositoryValueLayoutControlItem.Name = "repositoryValueLayoutControlItem";
		this.repositoryValueLayoutControlItem.Size = new System.Drawing.Size(600, 27);
		this.repositoryValueLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.repositoryValueLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.repositoryValueLayoutControlItem.TextVisible = false;
		this.dxErrorProvider.ContainerControl = this;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Name = "ConnectToSqlServerRepositoryUserControl";
		base.Load += new System.EventHandler(ConnectToSqlServerRepositoryUserControl_Load);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.connectionModeLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.loginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.authenticationLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.portTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.serverNameTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.serverNameTextEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.portTextEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.authenticationLookUpEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.loginTextEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordCheckEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseButtonEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectionModeLookUpEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.defaultPortLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.detailsLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.progressPanelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cancelGettingDatabasesSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryHeaderLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryValueLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dxErrorProvider).EndInit();
		base.ResumeLayout(false);
	}
}
