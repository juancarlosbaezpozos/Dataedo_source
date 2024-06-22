using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.LoginFormTools.Tools.Recent;
using Dataedo.App.LoginFormTools.Tools.Repository;
using Dataedo.App.LoginFormTools.Tools.Repository.RepositoryCreator;
using Dataedo.App.LoginFormTools.Tools.Repository.RepositoryCreator.Data;
using Dataedo.App.LoginFormTools.Tools.Repository.SqlServer;
using Dataedo.App.LoginFormTools.Tools.ScriptsSupport;
using Dataedo.App.LoginFormTools.UserControls.Base;
using Dataedo.App.LoginFormTools.UserControls.Subcontrols.Interfaces;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using Dataedo.Data.Commands.Enums;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.LoginFormTools.UserControls.Subcontrols;

public class UpgradeSqlServerRepositoryUserControl : PageWithLoaderUserControl, IRepositoryConnection, IRecentProjectData, IConnectionData
{
	private const string ScriptsCatalog = "Scripts";

	private string lastProvidedLogin = string.Empty;

	private bool lastProvidedSavePassword;

	private string lastProvidedPassword;

	private SqlConnectionStringBuilder serverConnectionStringBuilder;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private TextEdit serverNameTextEdit;

	private TextEdit portTextEdit;

	private LookUpEdit authenticationLookUpEdit;

	private TextEdit loginTextEdit;

	private TextEdit passwordTextEdit;

	private CheckEdit passwordCheckEdit;

	private ButtonEdit databaseButtonEdit;

	private LookUpEdit connectionModeLookUpEdit;

	private DXErrorProvider dxErrorProvider;

	private ToolTipController toolTipController;

	private LayoutControlItem serverNameTextEditLayoutControlItem;

	private LayoutControlItem authenticationLookUpEditLayoutControlItem;

	private LayoutControlItem loginTextEditLayoutControlItem;

	private LayoutControlItem portTextEditLayoutControlItem;

	private SimpleLabelItem portSimpleLabelItem;

	private SimpleLabelItem authenticationSimpleLabelItem;

	private LayoutControlItem databaseButtonEditLayoutControlItem;

	private LayoutControlItem connectionModeLookUpEditLayoutControlItem;

	private LayoutControlItem passwordTextEditLayoutControlItem;

	private LayoutControlItem passwordCheckEditLayoutControlItem;

	private SimpleLabelItem loginSimpleLabelItem;

	private SimpleLabelItem passwordSimpleLabelItem;

	private SimpleLabelItem connectionModeSimpleLabelItem;

	private SimpleLabelItem databaseSimpleLabelItem;

	private HelpIconUserControl portHelpIconUserControl;

	private LayoutControlItem portHelpIconUserControlLayoutControlItem;

	private HelpIconUserControl databaseHelpIconUserControl;

	private HelpIconUserControl loginHelpIconUserControl;

	private HelpIconUserControl helpIconUserControl;

	private LayoutControlItem loginHelpIconUserControlLayoutControlItem;

	private LayoutControlItem databaseHelpIconUserControlLayoutControlItem;

	private LayoutControlItem serverNameHelpIconUserControlLayoutControlItem;

	private SimpleLabelItem serverNameSimpleLabelItem;

	public string PreparedConnectionString => serverConnectionStringBuilder.ConnectionString;

	public List<Instruction> Instructions { get; private set; }

	public string Database => databaseButtonEdit.Text;

	public DatabaseType RepositoryType => DatabaseType.SqlServer;

	public string ServerName => serverNameTextEdit.Text;

	public string Login => loginTextEdit.Text;

	public string Password => passwordTextEdit.Text;

	public bool SavePassword => passwordCheckEdit.Enabled = passwordCheckEdit.Checked;

	public AuthenticationType.AuthenticationTypeEnum AuthenticationType => Dataedo.Shared.Enums.AuthenticationType.GetTypeByIndex(Authentication.GetIndex(authenticationLookUpEdit));

	public string AuthenticationTypeString => Dataedo.Shared.Enums.AuthenticationType.TypeToString(Dataedo.Shared.Enums.AuthenticationType.GetTypeByIndex(Authentication.GetIndex(authenticationLookUpEdit)));

	public int? Port => PrepareValue.ToInt(portTextEdit.Text);

	public SqlServerConnectionModeEnum.SqlServerConnectionMode? SqlServerConnectionMode => SqlServerConnectionModeEnum.StringToTypeOrDefault(connectionModeLookUpEdit.EditValue as string);

	public string ServerConnectionString => serverConnectionStringBuilder?.ConnectionString;

	[EditorBrowsable(EditorBrowsableState.Always)]
	[Browsable(true)]
	public event EventHandler DetailsVisibilityChanged;

	public UpgradeSqlServerRepositoryUserControl()
	{
		InitializeComponent();
		Dataedo.App.Data.MetadataServer.SqlServerConnectionMode.SetSqlServerConnectionMode(connectionModeLookUpEdit);
		connectionModeLookUpEdit.EditValue = SqlServerConnectionModeEnum.DefaultModeString;
		Authentication.CreateAuthenticationDataSource();
		Authentication.SetAuthenticationDataSource(authenticationLookUpEdit);
	}

	internal void SetRecentData(RecentItemModel recentItemModel)
	{
		serverNameTextEdit.Text = recentItemModel.Data.Server;
		databaseButtonEdit.Text = recentItemModel.Data.Database;
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
		ValidateRequiredFields();
	}

	internal void PrepareServerConnectionStringBuilder()
	{
		serverConnectionStringBuilder = SqlServerConnectionStringTools.GetConnectionStringBuilder(this, FindForm(), withDatabase: true, null);
	}

	internal bool PrepareInstructions(bool detachWebCatalog = false)
	{
		Instructions = new List<Instruction>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("USE [" + Database + "]");
		stringBuilder.AppendLine();
		RepositoryVersion targetRepositoryVersion = new RepositoryVersion
		{
			Version = ProgramVersion.Major,
			Update = ProgramVersion.Minor,
			Release = ProgramVersion.Build
		};
		try
		{
			RepositoryVersion repositoryVersion = DatabaseHelper.GetRepositoryVersion(PreparedConnectionString, FindForm());
			if (repositoryVersion == null)
			{
				return false;
			}
			List<ScriptFile> upgradeScriptFiles = GetUpgradeScriptFiles(targetRepositoryVersion, repositoryVersion, base.ParentForm);
			if (upgradeScriptFiles == null)
			{
				return false;
			}
			if (upgradeScriptFiles.Count == 0)
			{
				GeneralMessageBoxesHandling.Show("Upgrade error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, "Could not find required script files.", 1, FindForm());
				return false;
			}
			if (detachWebCatalog)
			{
				Instructions.Add(new Instruction("UPDATE [guid] SET [is_web_portal_connected] = 0"));
			}
			foreach (ScriptFile file in upgradeScriptFiles)
			{
				if (!File.Exists(file.FilePath))
				{
					string details = "Could not find script file:" + Environment.NewLine + file.FilePath + ".";
					GeneralMessageBoxesHandling.Show("Upgrade error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, details);
					return false;
				}
				Instruction[] collection = (from x in CommonScriptsHelper.SplitScript(file.FilePath)
					select new Instruction(x, file.RepositoryVersion)).ToArray();
				Instructions.AddRange(collection);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to parse script files.", FindForm());
			return false;
		}
		return true;
	}

	public static List<ScriptFile> GetUpgradeScriptFiles(RepositoryVersion targetRepositoryVersion, RepositoryVersion actualRepositoryVersion, Form owner = null, string sriptFolderPath = null)
	{
		List<ScriptFile> list = new List<ScriptFile>();
		try
		{
			foreach (string item in (from directoryPath in Directory.GetDirectories(sriptFolderPath ?? Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Scripts"))
				orderby Version.Parse(new DirectoryInfo(directoryPath).Name.Replace('_', '.'))
				select directoryPath).ToList())
			{
				string[] array = new DirectoryInfo(item).Name.Split('_', '.');
				RepositoryVersion repositoryVersion = new RepositoryVersion
				{
					Version = int.Parse(array[0]),
					Update = int.Parse(array[1])
				};
				try
				{
					repositoryVersion.Release = int.Parse(array[2]);
				}
				catch
				{
					repositoryVersion.Release = 0;
				}
				if ((!(repositoryVersion > actualRepositoryVersion) && (!ConfigHelper.GetForceRepoUpgradeConfigValue() || !(repositoryVersion >= targetRepositoryVersion))) || !(repositoryVersion <= targetRepositoryVersion))
				{
					continue;
				}
				IOrderedEnumerable<string> orderedEnumerable = from d in Directory.GetFiles(item)
					orderby d
					select d;
				if (!orderedEnumerable.Any())
				{
					continue;
				}
				foreach (string item2 in orderedEnumerable)
				{
					list.Add(new ScriptFile
					{
						FilePath = item2,
						RepositoryVersion = repositoryVersion
					});
				}
			}
			return list;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to prepare script files.", owner);
			return null;
		}
	}

	internal bool CheckConnectionToServer()
	{
		return DatabaseHelper.TryConnection(ServerConnectionString, showException: true, FindForm());
	}

	internal bool ValidateRequiredFields()
	{
		bool flag = true;
		if (!Authentication.IsWindowsAuthentication(authenticationLookUpEdit) && !Authentication.IsActiveDirectoryIntegrated(authenticationLookUpEdit))
		{
			flag &= ValidateLogin(flag);
		}
		return flag;
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
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

	private bool ValidateLogin(bool setFocusIfNotValid)
	{
		bool num = ValidateFields.ValidateEdit(loginTextEdit, dxErrorProvider, "login");
		if (!num && setFocusIfNotValid)
		{
			loginTextEdit.Focus();
		}
		return num;
	}

	private void ConnectToSqlServerRepositoryUserControl_Load(object sender, EventArgs e)
	{
		base.LoaderContext = base.Parent;
	}

	private void AuthenticationLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		bool flag = Authentication.IsWindowsAuthentication(authenticationLookUpEdit) || Authentication.IsActiveDirectoryIntegrated(authenticationLookUpEdit);
		LayoutControlItem layoutControlItem = loginTextEditLayoutControlItem;
		LayoutControlItem layoutControlItem2 = passwordTextEditLayoutControlItem;
		bool flag3 = (passwordCheckEdit.Enabled = !flag);
		bool enabled = (layoutControlItem2.Enabled = flag3);
		layoutControlItem.Enabled = enabled;
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
		else
		{
			loginTextEdit.Text = lastProvidedLogin;
			passwordCheckEdit.Enabled = true;
			passwordCheckEdit.Checked = lastProvidedSavePassword;
			passwordTextEdit.Text = lastProvidedPassword;
		}
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		DevExpress.XtraLayout.ColumnDefinition columnDefinition = new DevExpress.XtraLayout.ColumnDefinition();
		DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
		DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
		DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
		DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
		DevExpress.XtraLayout.RowDefinition rowDefinition = new DevExpress.XtraLayout.RowDefinition();
		DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
		DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
		DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
		DevExpress.XtraLayout.RowDefinition rowDefinition5 = new DevExpress.XtraLayout.RowDefinition();
		DevExpress.XtraLayout.RowDefinition rowDefinition6 = new DevExpress.XtraLayout.RowDefinition();
		DevExpress.XtraLayout.RowDefinition rowDefinition7 = new DevExpress.XtraLayout.RowDefinition();
		DevExpress.XtraLayout.RowDefinition rowDefinition8 = new DevExpress.XtraLayout.RowDefinition();
		DevExpress.XtraLayout.RowDefinition rowDefinition9 = new DevExpress.XtraLayout.RowDefinition();
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.databaseHelpIconUserControl = new Dataedo.App.UserControls.HelpIconUserControl();
		this.loginHelpIconUserControl = new Dataedo.App.UserControls.HelpIconUserControl();
		this.helpIconUserControl = new Dataedo.App.UserControls.HelpIconUserControl();
		this.portHelpIconUserControl = new Dataedo.App.UserControls.HelpIconUserControl();
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
		this.authenticationLookUpEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.loginTextEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.portTextEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.portSimpleLabelItem = new DevExpress.XtraLayout.SimpleLabelItem();
		this.authenticationSimpleLabelItem = new DevExpress.XtraLayout.SimpleLabelItem();
		this.databaseButtonEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.connectionModeLookUpEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.passwordTextEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.passwordCheckEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.loginSimpleLabelItem = new DevExpress.XtraLayout.SimpleLabelItem();
		this.passwordSimpleLabelItem = new DevExpress.XtraLayout.SimpleLabelItem();
		this.connectionModeSimpleLabelItem = new DevExpress.XtraLayout.SimpleLabelItem();
		this.databaseSimpleLabelItem = new DevExpress.XtraLayout.SimpleLabelItem();
		this.portHelpIconUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.loginHelpIconUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.databaseHelpIconUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.serverNameHelpIconUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.serverNameSimpleLabelItem = new DevExpress.XtraLayout.SimpleLabelItem();
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
		((System.ComponentModel.ISupportInitialize)this.authenticationLookUpEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.loginTextEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.portTextEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.portSimpleLabelItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.authenticationSimpleLabelItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseButtonEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectionModeLookUpEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordCheckEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.loginSimpleLabelItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordSimpleLabelItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectionModeSimpleLabelItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseSimpleLabelItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.portHelpIconUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.loginHelpIconUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseHelpIconUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.serverNameHelpIconUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.serverNameSimpleLabelItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dxErrorProvider).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.Controls.Add(this.databaseHelpIconUserControl);
		this.mainLayoutControl.Controls.Add(this.loginHelpIconUserControl);
		this.mainLayoutControl.Controls.Add(this.helpIconUserControl);
		this.mainLayoutControl.Controls.Add(this.portHelpIconUserControl);
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
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3611, 271, 733, 650);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "mainLayoutControl";
		this.mainLayoutControl.ToolTipController = this.toolTipController;
		this.databaseHelpIconUserControl.AutoPopDelay = 5000;
		this.databaseHelpIconUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.databaseHelpIconUserControl.KeepWhileHovered = false;
		this.databaseHelpIconUserControl.Location = new System.Drawing.Point(125, 191);
		this.databaseHelpIconUserControl.MaximumSize = new System.Drawing.Size(20, 20);
		this.databaseHelpIconUserControl.MaxToolTipWidth = 500;
		this.databaseHelpIconUserControl.MinimumSize = new System.Drawing.Size(20, 20);
		this.databaseHelpIconUserControl.Name = "databaseHelpIconUserControl";
		this.databaseHelpIconUserControl.Size = new System.Drawing.Size(20, 20);
		this.databaseHelpIconUserControl.TabIndex = 51;
		this.databaseHelpIconUserControl.ToolTipHeader = null;
		this.databaseHelpIconUserControl.ToolTipText = "Name of the database that will servers as Dataedo repository.";
		this.loginHelpIconUserControl.AutoPopDelay = 5000;
		this.loginHelpIconUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.loginHelpIconUserControl.KeepWhileHovered = false;
		this.loginHelpIconUserControl.Location = new System.Drawing.Point(125, 83);
		this.loginHelpIconUserControl.MaximumSize = new System.Drawing.Size(20, 20);
		this.loginHelpIconUserControl.MaxToolTipWidth = 500;
		this.loginHelpIconUserControl.MinimumSize = new System.Drawing.Size(20, 20);
		this.loginHelpIconUserControl.Name = "loginHelpIconUserControl";
		this.loginHelpIconUserControl.Size = new System.Drawing.Size(20, 20);
		this.loginHelpIconUserControl.TabIndex = 50;
		this.loginHelpIconUserControl.ToolTipHeader = null;
		this.loginHelpIconUserControl.ToolTipText = "To create repository please use account with <b>CREATE ANY DATABASE</b> permission on the server.";
		this.helpIconUserControl.AutoPopDelay = 5000;
		this.helpIconUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.helpIconUserControl.KeepWhileHovered = false;
		this.helpIconUserControl.Location = new System.Drawing.Point(125, 2);
		this.helpIconUserControl.MaximumSize = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.MaxToolTipWidth = 500;
		this.helpIconUserControl.MinimumSize = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.Name = "helpIconUserControl";
		this.helpIconUserControl.Size = new System.Drawing.Size(20, 20);
		this.helpIconUserControl.TabIndex = 49;
		this.helpIconUserControl.ToolTipHeader = null;
		this.helpIconUserControl.ToolTipText = "Host name of your SQL Server.";
		this.portHelpIconUserControl.AutoPopDelay = 5000;
		this.portHelpIconUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.portHelpIconUserControl.KeepWhileHovered = false;
		this.portHelpIconUserControl.Location = new System.Drawing.Point(125, 29);
		this.portHelpIconUserControl.MaximumSize = new System.Drawing.Size(20, 20);
		this.portHelpIconUserControl.MaxToolTipWidth = 500;
		this.portHelpIconUserControl.MinimumSize = new System.Drawing.Size(20, 20);
		this.portHelpIconUserControl.Name = "portHelpIconUserControl";
		this.portHelpIconUserControl.Size = new System.Drawing.Size(20, 20);
		this.portHelpIconUserControl.TabIndex = 48;
		this.portHelpIconUserControl.ToolTipHeader = null;
		this.portHelpIconUserControl.ToolTipText = "Port of the database that serves as Dataedo repository.";
		this.connectionModeLookUpEdit.Location = new System.Drawing.Point(151, 164);
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
		this.connectionModeLookUpEdit.Size = new System.Drawing.Size(296, 20);
		this.connectionModeLookUpEdit.StyleController = this.mainLayoutControl;
		this.connectionModeLookUpEdit.TabIndex = 45;
		this.connectionModeLookUpEdit.ToolTipController = this.toolTipController;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.databaseButtonEdit.Enabled = false;
		this.databaseButtonEdit.Location = new System.Drawing.Point(151, 191);
		this.databaseButtonEdit.MaximumSize = new System.Drawing.Size(300, 20);
		this.databaseButtonEdit.Name = "databaseButtonEdit";
		this.databaseButtonEdit.Size = new System.Drawing.Size(296, 20);
		this.databaseButtonEdit.StyleController = this.mainLayoutControl;
		this.databaseButtonEdit.TabIndex = 44;
		this.databaseButtonEdit.ToolTipController = this.toolTipController;
		this.passwordCheckEdit.Location = new System.Drawing.Point(151, 137);
		this.passwordCheckEdit.Margin = new System.Windows.Forms.Padding(0);
		this.passwordCheckEdit.MaximumSize = new System.Drawing.Size(100, 0);
		this.passwordCheckEdit.Name = "passwordCheckEdit";
		this.passwordCheckEdit.Properties.Caption = "Save password";
		this.passwordCheckEdit.Size = new System.Drawing.Size(100, 20);
		this.passwordCheckEdit.StyleController = this.mainLayoutControl;
		this.passwordCheckEdit.TabIndex = 43;
		this.passwordTextEdit.Location = new System.Drawing.Point(151, 110);
		this.passwordTextEdit.MaximumSize = new System.Drawing.Size(300, 20);
		this.passwordTextEdit.Name = "passwordTextEdit";
		this.passwordTextEdit.Properties.UseSystemPasswordChar = true;
		this.passwordTextEdit.Size = new System.Drawing.Size(296, 20);
		this.passwordTextEdit.StyleController = this.mainLayoutControl;
		this.passwordTextEdit.TabIndex = 42;
		this.passwordTextEdit.ToolTipController = this.toolTipController;
		this.loginTextEdit.Location = new System.Drawing.Point(151, 83);
		this.loginTextEdit.MaximumSize = new System.Drawing.Size(300, 20);
		this.loginTextEdit.Name = "loginTextEdit";
		this.loginTextEdit.Size = new System.Drawing.Size(296, 20);
		this.loginTextEdit.StyleController = this.mainLayoutControl;
		this.loginTextEdit.TabIndex = 41;
		this.loginTextEdit.ToolTipController = this.toolTipController;
		this.authenticationLookUpEdit.Location = new System.Drawing.Point(151, 56);
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
		this.authenticationLookUpEdit.Size = new System.Drawing.Size(296, 20);
		this.authenticationLookUpEdit.StyleController = this.mainLayoutControl;
		this.authenticationLookUpEdit.TabIndex = 40;
		this.authenticationLookUpEdit.ToolTipController = this.toolTipController;
		this.authenticationLookUpEdit.EditValueChanged += new System.EventHandler(AuthenticationLookUpEdit_EditValueChanged);
		this.portTextEdit.Enabled = false;
		this.portTextEdit.Location = new System.Drawing.Point(151, 29);
		this.portTextEdit.MaximumSize = new System.Drawing.Size(300, 20);
		this.portTextEdit.Name = "portTextEdit";
		this.portTextEdit.Properties.Mask.EditMask = "\\d+";
		this.portTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.portTextEdit.Properties.Mask.ShowPlaceHolders = false;
		this.portTextEdit.Properties.MaxLength = 5;
		this.portTextEdit.Size = new System.Drawing.Size(296, 20);
		this.portTextEdit.StyleController = this.mainLayoutControl;
		this.portTextEdit.TabIndex = 5;
		this.portTextEdit.ToolTipController = this.toolTipController;
		this.serverNameTextEdit.Enabled = false;
		this.serverNameTextEdit.Location = new System.Drawing.Point(151, 2);
		this.serverNameTextEdit.MaximumSize = new System.Drawing.Size(300, 20);
		this.serverNameTextEdit.Name = "serverNameTextEdit";
		this.serverNameTextEdit.Size = new System.Drawing.Size(296, 20);
		this.serverNameTextEdit.StyleController = this.mainLayoutControl;
		this.serverNameTextEdit.TabIndex = 4;
		this.serverNameTextEdit.ToolTipController = this.toolTipController;
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[19]
		{
			this.serverNameTextEditLayoutControlItem, this.authenticationLookUpEditLayoutControlItem, this.loginTextEditLayoutControlItem, this.portTextEditLayoutControlItem, this.portSimpleLabelItem, this.authenticationSimpleLabelItem, this.databaseButtonEditLayoutControlItem, this.connectionModeLookUpEditLayoutControlItem, this.passwordTextEditLayoutControlItem, this.passwordCheckEditLayoutControlItem,
			this.loginSimpleLabelItem, this.passwordSimpleLabelItem, this.connectionModeSimpleLabelItem, this.databaseSimpleLabelItem, this.portHelpIconUserControlLayoutControlItem, this.loginHelpIconUserControlLayoutControlItem, this.databaseHelpIconUserControlLayoutControlItem, this.serverNameHelpIconUserControlLayoutControlItem, this.serverNameSimpleLabelItem
		});
		this.mainLayoutControlGroup.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.OptionsItemText.TextToControlDistance = 10;
		columnDefinition.SizeType = System.Windows.Forms.SizeType.Absolute;
		columnDefinition.Width = 123.0;
		columnDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
		columnDefinition2.Width = 26.0;
		columnDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
		columnDefinition3.Width = 300.0;
		columnDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
		columnDefinition4.Width = 20.0;
		columnDefinition5.SizeType = System.Windows.Forms.SizeType.AutoSize;
		columnDefinition5.Width = 231.0;
		this.mainLayoutControlGroup.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[5] { columnDefinition, columnDefinition2, columnDefinition3, columnDefinition4, columnDefinition5 });
		rowDefinition.Height = 27.0;
		rowDefinition.SizeType = System.Windows.Forms.SizeType.AutoSize;
		rowDefinition2.Height = 27.0;
		rowDefinition2.SizeType = System.Windows.Forms.SizeType.AutoSize;
		rowDefinition3.Height = 27.0;
		rowDefinition3.SizeType = System.Windows.Forms.SizeType.AutoSize;
		rowDefinition4.Height = 27.0;
		rowDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
		rowDefinition5.Height = 27.0;
		rowDefinition5.SizeType = System.Windows.Forms.SizeType.AutoSize;
		rowDefinition6.Height = 27.0;
		rowDefinition6.SizeType = System.Windows.Forms.SizeType.AutoSize;
		rowDefinition7.Height = 27.0;
		rowDefinition7.SizeType = System.Windows.Forms.SizeType.AutoSize;
		rowDefinition8.Height = 27.0;
		rowDefinition8.SizeType = System.Windows.Forms.SizeType.AutoSize;
		rowDefinition9.Height = 254.0;
		rowDefinition9.SizeType = System.Windows.Forms.SizeType.AutoSize;
		this.mainLayoutControlGroup.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[9] { rowDefinition, rowDefinition2, rowDefinition3, rowDefinition4, rowDefinition5, rowDefinition6, rowDefinition7, rowDefinition8, rowDefinition9 });
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(700, 470);
		this.mainLayoutControlGroup.TextVisible = false;
		this.serverNameTextEditLayoutControlItem.Control = this.serverNameTextEdit;
		this.serverNameTextEditLayoutControlItem.Location = new System.Drawing.Point(149, 0);
		this.serverNameTextEditLayoutControlItem.Name = "serverNameTextEditLayoutControlItem";
		this.serverNameTextEditLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 2;
		this.serverNameTextEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.serverNameTextEditLayoutControlItem.Size = new System.Drawing.Size(300, 27);
		this.serverNameTextEditLayoutControlItem.Text = "Server name:";
		this.serverNameTextEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.serverNameTextEditLayoutControlItem.TextVisible = false;
		this.authenticationLookUpEditLayoutControlItem.Control = this.authenticationLookUpEdit;
		this.authenticationLookUpEditLayoutControlItem.Location = new System.Drawing.Point(149, 54);
		this.authenticationLookUpEditLayoutControlItem.Name = "authenticationLookUpEditLayoutControlItem";
		this.authenticationLookUpEditLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 2;
		this.authenticationLookUpEditLayoutControlItem.OptionsTableLayoutItem.RowIndex = 2;
		this.authenticationLookUpEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.authenticationLookUpEditLayoutControlItem.Size = new System.Drawing.Size(300, 27);
		this.authenticationLookUpEditLayoutControlItem.Text = "Authentication:";
		this.authenticationLookUpEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.authenticationLookUpEditLayoutControlItem.TextVisible = false;
		this.loginTextEditLayoutControlItem.Control = this.loginTextEdit;
		this.loginTextEditLayoutControlItem.Location = new System.Drawing.Point(149, 81);
		this.loginTextEditLayoutControlItem.Name = "loginTextEditLayoutControlItem";
		this.loginTextEditLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 2;
		this.loginTextEditLayoutControlItem.OptionsTableLayoutItem.RowIndex = 3;
		this.loginTextEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.loginTextEditLayoutControlItem.Size = new System.Drawing.Size(300, 27);
		this.loginTextEditLayoutControlItem.Text = "Login:";
		this.loginTextEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.loginTextEditLayoutControlItem.TextVisible = false;
		this.portTextEditLayoutControlItem.Control = this.portTextEdit;
		this.portTextEditLayoutControlItem.Location = new System.Drawing.Point(149, 27);
		this.portTextEditLayoutControlItem.Name = "portTextEditLayoutControlItem";
		this.portTextEditLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 2;
		this.portTextEditLayoutControlItem.OptionsTableLayoutItem.RowIndex = 1;
		this.portTextEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.portTextEditLayoutControlItem.Size = new System.Drawing.Size(300, 27);
		this.portTextEditLayoutControlItem.Text = "Port:";
		this.portTextEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.portTextEditLayoutControlItem.TextVisible = false;
		this.portSimpleLabelItem.AllowHotTrack = false;
		this.portSimpleLabelItem.Location = new System.Drawing.Point(0, 27);
		this.portSimpleLabelItem.Name = "portSimpleLabelItem";
		this.portSimpleLabelItem.OptionsTableLayoutItem.RowIndex = 1;
		this.portSimpleLabelItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.portSimpleLabelItem.Size = new System.Drawing.Size(123, 27);
		this.portSimpleLabelItem.Text = "Port:";
		this.portSimpleLabelItem.TextSize = new System.Drawing.Size(99, 13);
		this.authenticationSimpleLabelItem.AllowHotTrack = false;
		this.authenticationSimpleLabelItem.Location = new System.Drawing.Point(0, 54);
		this.authenticationSimpleLabelItem.Name = "authenticationSimpleLabelItem";
		this.authenticationSimpleLabelItem.OptionsTableLayoutItem.RowIndex = 2;
		this.authenticationSimpleLabelItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.authenticationSimpleLabelItem.Size = new System.Drawing.Size(123, 27);
		this.authenticationSimpleLabelItem.Text = "Authentication type:";
		this.authenticationSimpleLabelItem.TextSize = new System.Drawing.Size(99, 13);
		this.databaseButtonEditLayoutControlItem.Control = this.databaseButtonEdit;
		this.databaseButtonEditLayoutControlItem.Location = new System.Drawing.Point(149, 189);
		this.databaseButtonEditLayoutControlItem.Name = "databaseButtonEditLayoutControlItem";
		this.databaseButtonEditLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 2;
		this.databaseButtonEditLayoutControlItem.OptionsTableLayoutItem.RowIndex = 7;
		this.databaseButtonEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.databaseButtonEditLayoutControlItem.Size = new System.Drawing.Size(300, 27);
		this.databaseButtonEditLayoutControlItem.Text = "Database:";
		this.databaseButtonEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.databaseButtonEditLayoutControlItem.TextVisible = false;
		this.connectionModeLookUpEditLayoutControlItem.Control = this.connectionModeLookUpEdit;
		this.connectionModeLookUpEditLayoutControlItem.Location = new System.Drawing.Point(149, 162);
		this.connectionModeLookUpEditLayoutControlItem.Name = "connectionModeLookUpEditLayoutControlItem";
		this.connectionModeLookUpEditLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 2;
		this.connectionModeLookUpEditLayoutControlItem.OptionsTableLayoutItem.RowIndex = 6;
		this.connectionModeLookUpEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.connectionModeLookUpEditLayoutControlItem.Size = new System.Drawing.Size(300, 27);
		this.connectionModeLookUpEditLayoutControlItem.Text = "Connection mode:";
		this.connectionModeLookUpEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.connectionModeLookUpEditLayoutControlItem.TextVisible = false;
		this.passwordTextEditLayoutControlItem.Control = this.passwordTextEdit;
		this.passwordTextEditLayoutControlItem.Location = new System.Drawing.Point(149, 108);
		this.passwordTextEditLayoutControlItem.Name = "passwordTextEditLayoutControlItem";
		this.passwordTextEditLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 2;
		this.passwordTextEditLayoutControlItem.OptionsTableLayoutItem.RowIndex = 4;
		this.passwordTextEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.passwordTextEditLayoutControlItem.Size = new System.Drawing.Size(300, 27);
		this.passwordTextEditLayoutControlItem.Text = "Password:";
		this.passwordTextEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.passwordTextEditLayoutControlItem.TextVisible = false;
		this.passwordCheckEditLayoutControlItem.Control = this.passwordCheckEdit;
		this.passwordCheckEditLayoutControlItem.CustomizationFormText = "Save password:";
		this.passwordCheckEditLayoutControlItem.Location = new System.Drawing.Point(149, 135);
		this.passwordCheckEditLayoutControlItem.Name = "passwordCheckEditLayoutControlItem";
		this.passwordCheckEditLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 2;
		this.passwordCheckEditLayoutControlItem.OptionsTableLayoutItem.RowIndex = 5;
		this.passwordCheckEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.passwordCheckEditLayoutControlItem.Size = new System.Drawing.Size(300, 27);
		this.passwordCheckEditLayoutControlItem.Text = " ";
		this.passwordCheckEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.passwordCheckEditLayoutControlItem.TextVisible = false;
		this.loginSimpleLabelItem.AllowHotTrack = false;
		this.loginSimpleLabelItem.Location = new System.Drawing.Point(0, 81);
		this.loginSimpleLabelItem.Name = "loginSimpleLabelItem";
		this.loginSimpleLabelItem.OptionsTableLayoutItem.RowIndex = 3;
		this.loginSimpleLabelItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.loginSimpleLabelItem.Size = new System.Drawing.Size(123, 27);
		this.loginSimpleLabelItem.Text = "Login:";
		this.loginSimpleLabelItem.TextSize = new System.Drawing.Size(99, 13);
		this.passwordSimpleLabelItem.AllowHotTrack = false;
		this.passwordSimpleLabelItem.Location = new System.Drawing.Point(0, 108);
		this.passwordSimpleLabelItem.Name = "passwordSimpleLabelItem";
		this.passwordSimpleLabelItem.OptionsTableLayoutItem.RowIndex = 4;
		this.passwordSimpleLabelItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.passwordSimpleLabelItem.Size = new System.Drawing.Size(123, 27);
		this.passwordSimpleLabelItem.Text = "Password:";
		this.passwordSimpleLabelItem.TextSize = new System.Drawing.Size(99, 13);
		this.connectionModeSimpleLabelItem.AllowHotTrack = false;
		this.connectionModeSimpleLabelItem.Location = new System.Drawing.Point(0, 162);
		this.connectionModeSimpleLabelItem.Name = "connectionModeSimpleLabelItem";
		this.connectionModeSimpleLabelItem.OptionsTableLayoutItem.RowIndex = 6;
		this.connectionModeSimpleLabelItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.connectionModeSimpleLabelItem.Size = new System.Drawing.Size(123, 27);
		this.connectionModeSimpleLabelItem.Text = "Connection mode:";
		this.connectionModeSimpleLabelItem.TextSize = new System.Drawing.Size(99, 13);
		this.databaseSimpleLabelItem.AllowHotTrack = false;
		this.databaseSimpleLabelItem.Location = new System.Drawing.Point(0, 189);
		this.databaseSimpleLabelItem.Name = "databaseSimpleLabelItem";
		this.databaseSimpleLabelItem.OptionsTableLayoutItem.RowIndex = 7;
		this.databaseSimpleLabelItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.databaseSimpleLabelItem.Size = new System.Drawing.Size(123, 27);
		this.databaseSimpleLabelItem.Text = "Database:";
		this.databaseSimpleLabelItem.TextSize = new System.Drawing.Size(99, 13);
		this.portHelpIconUserControlLayoutControlItem.Control = this.portHelpIconUserControl;
		this.portHelpIconUserControlLayoutControlItem.Location = new System.Drawing.Point(123, 27);
		this.portHelpIconUserControlLayoutControlItem.Name = "portHelpIconUserControlLayoutControlItem";
		this.portHelpIconUserControlLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 1;
		this.portHelpIconUserControlLayoutControlItem.OptionsTableLayoutItem.RowIndex = 1;
		this.portHelpIconUserControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.portHelpIconUserControlLayoutControlItem.Size = new System.Drawing.Size(26, 27);
		this.portHelpIconUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.portHelpIconUserControlLayoutControlItem.TextVisible = false;
		this.loginHelpIconUserControlLayoutControlItem.Control = this.loginHelpIconUserControl;
		this.loginHelpIconUserControlLayoutControlItem.Location = new System.Drawing.Point(123, 81);
		this.loginHelpIconUserControlLayoutControlItem.Name = "loginHelpIconUserControlLayoutControlItem";
		this.loginHelpIconUserControlLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 1;
		this.loginHelpIconUserControlLayoutControlItem.OptionsTableLayoutItem.RowIndex = 3;
		this.loginHelpIconUserControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.loginHelpIconUserControlLayoutControlItem.Size = new System.Drawing.Size(26, 27);
		this.loginHelpIconUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.loginHelpIconUserControlLayoutControlItem.TextVisible = false;
		this.databaseHelpIconUserControlLayoutControlItem.Control = this.databaseHelpIconUserControl;
		this.databaseHelpIconUserControlLayoutControlItem.Location = new System.Drawing.Point(123, 189);
		this.databaseHelpIconUserControlLayoutControlItem.Name = "databaseHelpIconUserControlLayoutControlItem";
		this.databaseHelpIconUserControlLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 1;
		this.databaseHelpIconUserControlLayoutControlItem.OptionsTableLayoutItem.RowIndex = 7;
		this.databaseHelpIconUserControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.databaseHelpIconUserControlLayoutControlItem.Size = new System.Drawing.Size(26, 27);
		this.databaseHelpIconUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.databaseHelpIconUserControlLayoutControlItem.TextVisible = false;
		this.serverNameHelpIconUserControlLayoutControlItem.Control = this.helpIconUserControl;
		this.serverNameHelpIconUserControlLayoutControlItem.Location = new System.Drawing.Point(123, 0);
		this.serverNameHelpIconUserControlLayoutControlItem.Name = "serverNameHelpIconUserControlLayoutControlItem";
		this.serverNameHelpIconUserControlLayoutControlItem.OptionsTableLayoutItem.ColumnIndex = 1;
		this.serverNameHelpIconUserControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.serverNameHelpIconUserControlLayoutControlItem.Size = new System.Drawing.Size(26, 27);
		this.serverNameHelpIconUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.serverNameHelpIconUserControlLayoutControlItem.TextVisible = false;
		this.serverNameSimpleLabelItem.AllowHotTrack = false;
		this.serverNameSimpleLabelItem.Location = new System.Drawing.Point(0, 0);
		this.serverNameSimpleLabelItem.Name = "serverNameSimpleLabelItem";
		this.serverNameSimpleLabelItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 5);
		this.serverNameSimpleLabelItem.Size = new System.Drawing.Size(123, 27);
		this.serverNameSimpleLabelItem.Text = "Server name:";
		this.serverNameSimpleLabelItem.TextSize = new System.Drawing.Size(99, 13);
		this.dxErrorProvider.ContainerControl = this;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Name = "UpgradeSqlServerRepositoryUserControl";
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
		((System.ComponentModel.ISupportInitialize)this.authenticationLookUpEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.loginTextEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.portTextEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.portSimpleLabelItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.authenticationSimpleLabelItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseButtonEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectionModeLookUpEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordCheckEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.loginSimpleLabelItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordSimpleLabelItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectionModeSimpleLabelItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseSimpleLabelItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.portHelpIconUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.loginHelpIconUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseHelpIconUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.serverNameHelpIconUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.serverNameSimpleLabelItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dxErrorProvider).EndInit();
		base.ResumeLayout(false);
	}
}
