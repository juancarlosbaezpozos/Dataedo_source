using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Model.Data.PersonalSettings;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class DbConnectUserControlNew : BaseUserControl
{
	private Action displayFormSaveAsLayoutControlItem;

	private Action displayFilterInfo;

	public bool? isCopyingConnection;

	private bool isSettingParameters;

	private DBMSGridModel dbmsGridModel;

	private const int DBMS_LAYOUT_CONTROL_ORIGINAL_TEXT_SIZE = 100;

	private DatabaseRow databaseRow;

	private SharedDatabaseTypeEnum.DatabaseType? initDatabaseType;

	private SharedDatabaseTypeEnum.DatabaseType? selectedDatabaseType;

	private IContainer components;

	private NonCustomizableLayoutControl loginLayoutControl;

	private LabelControl dbcconnectionLabelControl;

	private LayoutControlGroup layoutControlGroup7;

	private SplashScreenManager splashScreenManager1;

	private ComboBoxEdit saveAsComboBoxEdit;

	private LayoutControlItem saveAsLayoutControlItem;

	private EmptySpaceItem saveAsEmptySpaceItem;

	private DefaultToolTipController defaultToolTipController;

	private LabelControl dbmsLabelControl;

	private LayoutControlItem dbmsLayoutControlItem;

	private EmptySpaceItem emptySpaceItem69;

	private EmptySpaceItem emptySpaceItem42;

	private LayoutControlItem connectorLayoutControlItem;

	private SQLServerConnectorControl sqlServerConnectorControl;

	public ConnectorControlBase ConnectorControl { get; private set; }

	public int SaveProfileSelectedIndex => saveAsComboBoxEdit.SelectedIndex;

	public bool IsExporting { get; set; }

	public bool HasPersonalSettingsLoaded { get; private set; }

	public bool IsDBAdded { get; set; }

	public DatabaseRow DatabaseRow
	{
		get
		{
			return databaseRow;
		}
		set
		{
			databaseRow = value;
		}
	}

	public SharedDatabaseTypeEnum.DatabaseType? SelectedDatabaseType
	{
		get
		{
			return selectedDatabaseType;
		}
		set
		{
			selectedDatabaseType = value;
			this.SelectedDatabaseTypeChanged?.Invoke(this, null);
		}
	}

	public event Action DisplayFormSaveAsLayoutControlItem
	{
		add
		{
			if (displayFormSaveAsLayoutControlItem == null || !displayFormSaveAsLayoutControlItem.GetInvocationList().Contains(value))
			{
				displayFormSaveAsLayoutControlItem = (Action)Delegate.Combine(displayFormSaveAsLayoutControlItem, value);
			}
		}
		remove
		{
			displayFormSaveAsLayoutControlItem = (Action)Delegate.Remove(displayFormSaveAsLayoutControlItem, value);
		}
	}

	public event Action DisplayFilterInfo
	{
		add
		{
			if (displayFilterInfo == null || !displayFilterInfo.GetInvocationList().Contains(value))
			{
				displayFilterInfo = (Action)Delegate.Combine(displayFilterInfo, value);
			}
		}
		remove
		{
			displayFilterInfo = (Action)Delegate.Remove(displayFilterInfo, value);
		}
	}

	public event EventHandler SelectedDatabaseTypeChanged;

	public DbConnectUserControlNew()
	{
		InitializeComponent();
		ConnectorControl = sqlServerConnectorControl;
		sqlServerConnectorControl.SetParentControl(this);
		sqlServerConnectorControl.SetTheme();
	}

	private void SetConnectorControl()
	{
		if (SelectedDatabaseType.HasValue)
		{
			switch (DatabaseSupportFactory.GetDatabaseSupport(SelectedDatabaseType).PanelType)
			{
			case PanelTypeEnum.PanelType.MySQL:
				SetNewConnectorControl<MySQLConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.SqlServer:
				SetNewConnectorControl<SQLServerConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Oracle:
				SetNewConnectorControl<OracleConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.SsasTabular:
				SetNewConnectorControl<SSASConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Odbc:
				SetNewConnectorControl<ODBCConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.MongoDB:
				SetNewConnectorControl<MongoDbConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Redshift:
				SetNewConnectorControl<RedshiftConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Neo4j:
				SetNewConnectorControl<Neo4jConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Elasticsearch:
				SetNewConnectorControl<ElasticsearchConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.GoogleBigQuery:
				SetNewConnectorControl<GoogleBigQueryConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Cassandra:
				SetNewConnectorControl<CassandraConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Teradata:
				SetNewConnectorControl<TeradataConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Vertica:
				SetNewConnectorControl<VerticaConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Db2:
				SetNewConnectorControl<Db2ConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Snowflake:
				SetNewConnectorControl<SnowflakeConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.PostgreSQL:
				SetNewConnectorControl<PostgreSQLConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.PowerBiDataset:
				SetNewConnectorControl<PowerBiDatasetConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.SapAse:
				SetNewConnectorControl<SapAseConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.None:
				ClearConnectorControl();
				break;
			case PanelTypeEnum.PanelType.CosmosDBSqlAPI:
				SetNewConnectorControl<CosmosDbConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.HiveMetastoreMySQL:
				SetNewConnectorControl<HiveMetastoreMySQLConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.HiveMetastoreSQLServer:
				SetNewConnectorControl<HiveMetastoreSQLServerConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.HiveMetastorePostgreSQL:
				SetNewConnectorControl<HiveMetastorePostgreSQLConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Salesforce:
				SetNewConnectorControl<SalesforceConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.DdlScript:
				SetNewConnectorControl<DdlScriptConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Athena:
				SetNewConnectorControl<AthenaConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Tableau:
				SetNewConnectorControl<TableauConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.AmazonS3:
				SetNewConnectorControl<AmazonS3ConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.InterfaceTables:
				SetNewConnectorControl<InterfaceTablesConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Dataverse:
				SetNewConnectorControl<DataverseConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.DynamoDB:
				SetNewConnectorControl<DynamoDBConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.SapHana:
				SetNewConnectorControl<SapHanaConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.AzureStorage:
				SetNewConnectorControl<AzureStorageConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.Astra:
				SetNewConnectorControl<AstraConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.NetSuite:
				SetNewConnectorControl<NetSuiteConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.DBT:
				SetNewConnectorControl<DBTConnectorControl>();
				break;
			case PanelTypeEnum.PanelType.SSIS:
				SetNewConnectorControl<SSISConnectorControl>();
				break;
			}
		}
	}

	private void SetNewConnectorControl<T>() where T : ConnectorControlBase, new()
	{
		if (ConnectorControl?.GetType() != typeof(T))
		{
			ConnectorControl = new T();
			ConnectorControl.SetParentControl(this);
			loginLayoutControl.BeginInit();
			connectorLayoutControlItem.BeginInit();
			Control control = connectorLayoutControlItem.Control;
			connectorLayoutControlItem.Control = ConnectorControl;
			if (control != null)
			{
				control.Parent = null;
			}
			connectorLayoutControlItem.EndInit();
			loginLayoutControl.EndInit();
		}
	}

	private void ClearConnectorControl()
	{
		ConnectorControl = null;
		loginLayoutControl.BeginInit();
		connectorLayoutControlItem.BeginInit();
		Control control = connectorLayoutControlItem.Control;
		connectorLayoutControlItem.Control = null;
		if (control != null)
		{
			control.Parent = null;
		}
		connectorLayoutControlItem.EndInit();
		loginLayoutControl.EndInit();
	}

	public void SetParameters(int? databaseId = null, bool? isDbAdded = false, SharedDatabaseTypeEnum.DatabaseType? selectedDatabaseType = null, bool? isCopyingConnection = false, DBMSGridModel dbmsGridModel = null)
	{
		this.dbmsGridModel = dbmsGridModel;
		dbmsLabelControl.ImageOptions.Image = GetImportIcon(selectedDatabaseType, dbmsGridModel);
		dbmsLabelControl.Text = this.dbmsGridModel?.Name;
		this.isCopyingConnection = isCopyingConnection;
		if (saveAsComboBoxEdit.Properties.Items.Count == 0)
		{
			saveAsComboBoxEdit.Properties.Items.Add(UserSettingsEnum.GetSettingsString(UserSettingsType.Public));
			saveAsComboBoxEdit.Properties.Items.Add(UserSettingsEnum.GetSettingsString(UserSettingsType.Personal));
		}
		if (!databaseId.HasValue && isCopyingConnection == false)
		{
			SelectedDatabaseType = selectedDatabaseType;
			IsDBAdded = isDbAdded.GetValueOrDefault();
			DatabaseRow = new DatabaseRow();
		}
		else
		{
			IsDBAdded = false;
			if (isCopyingConnection == true)
			{
				DatabaseRow.Id = null;
			}
			SelectedDatabaseType = selectedDatabaseType;
		}
		SetConnectorControl();
		SetDbmsControlTextWidth(100);
		ConnectorControl?.SetParameters(databaseId, isCopyingConnection, IsExporting);
		if ((ConnectorControl?.HideSavingCredentials).GetValueOrDefault())
		{
			saveAsLayoutControlItem.HideToCustomization();
			saveAsEmptySpaceItem.HideToCustomization();
		}
		else
		{
			if (saveAsLayoutControlItem.IsHidden)
			{
				saveAsLayoutControlItem.RestoreFromCustomization();
			}
			if (saveAsEmptySpaceItem.IsHidden)
			{
				saveAsEmptySpaceItem.RestoreFromCustomization();
			}
		}
		if (DatabaseRow == null || !DatabaseRow.Id.HasValue || StaticData.IsProjectFile)
		{
			SetSaveAsLayoutControlItemVisibility(visible: false);
		}
		isSettingParameters = true;
		saveAsComboBoxEdit.SelectedIndex = ((HasPersonalSettingsLoaded && !StaticData.IsProjectFile) ? 1 : 0);
		isSettingParameters = false;
		initDatabaseType = this?.DatabaseRow?.Type;
		displayFormSaveAsLayoutControlItem?.Invoke();
	}

	private Image GetImportIcon(SharedDatabaseTypeEnum.DatabaseType? selectedDatabaseType, DBMSGridModel dbmsGridModel)
	{
		if (selectedDatabaseType.HasValue && selectedDatabaseType.GetValueOrDefault() == SharedDatabaseTypeEnum.DatabaseType.DdlScript)
		{
			return Resources.ddl_script_16;
		}
		return dbmsGridModel?.Image;
	}

	public void SetSaveAsLayoutControlItemVisibility(bool visible)
	{
		LayoutVisibility layoutVisibility3 = (saveAsLayoutControlItem.Visibility = (saveAsEmptySpaceItem.Visibility = ((!visible) ? LayoutVisibility.Never : LayoutVisibility.Always)));
	}

	public void InitializeDatabaseRow(int? databaseId)
	{
		if (IsExporting)
		{
			DatabaseRow = new DatabaseRow(DB.Database.GetDataById(databaseId.Value));
		}
		else if (StaticData.IsProjectFile)
		{
			if (!databaseId.HasValue)
			{
				DatabaseRow = new DatabaseRow();
			}
			else
			{
				GetPublicSettings(databaseId);
			}
			HasPersonalSettingsLoaded = false;
		}
		else
		{
			LoadSettings(databaseId);
		}
	}

	public void LoadSettings(int? databaseId)
	{
		PersonalSettingsObject personalSettingsDB = GetPersonalSettingsDB(databaseId);
		if (!databaseId.HasValue)
		{
			DatabaseRow = new DatabaseRow();
			HasPersonalSettingsLoaded = false;
		}
		else if (personalSettingsDB == null)
		{
			GetPublicSettings(databaseId);
			HasPersonalSettingsLoaded = false;
		}
		else
		{
			GetPersonalSettings(databaseId, personalSettingsDB);
			HasPersonalSettingsLoaded = true;
		}
	}

	private void ChangeDatabaseRow(int? databaseId)
	{
		if (StaticData.IsProjectFile)
		{
			if (!databaseId.HasValue)
			{
				DatabaseRow = new DatabaseRow();
			}
			else
			{
				GetPublicSettings(databaseId);
			}
			HasPersonalSettingsLoaded = false;
		}
		else
		{
			ChangeServerDatabaseRow(databaseId);
		}
	}

	private void ChangeServerDatabaseRow(int? databaseId)
	{
		if (!databaseId.HasValue)
		{
			DatabaseRow = new DatabaseRow();
			HasPersonalSettingsLoaded = false;
		}
		else if (saveAsComboBoxEdit.SelectedIndex == 0)
		{
			GetPublicSettings(databaseId);
			HasPersonalSettingsLoaded = false;
		}
		else
		{
			PersonalSettingsObject personalSettingsDB = GetPersonalSettingsDB(databaseId);
			GetPersonalSettings(databaseId, personalSettingsDB);
			HasPersonalSettingsLoaded = true;
		}
	}

	private PersonalSettingsObject GetPersonalSettingsDB(int? databaseId)
	{
		return DB.UserPersonalSettings.GetPersonalSettings(StaticData.CurrentLicenseId, databaseId ?? (-1));
	}

	private void GetPublicSettings(int? databaseId)
	{
		DatabaseRow = new DatabaseRow(DB.Database.GetDataById(databaseId.Value));
	}

	private void GetPersonalSettings(int? databaseId, PersonalSettingsObject data)
	{
		if (data == null)
		{
			SharedDatabaseTypeEnum.DatabaseType? type = DatabaseRow.Type;
			string name = DatabaseRow.Name;
			string title = DatabaseRow.Title;
			string serviceName = DatabaseRow.ServiceName;
			DatabaseRow = new DatabaseRow
			{
				Id = databaseId,
				Type = type,
				SSLType = DatabaseRow.SSLType,
				SSLSettings = DatabaseRow.SSLSettings
			};
			if (IsExporting && string.IsNullOrEmpty(DatabaseRow.Name))
			{
				DatabaseRow.Name = name;
				DatabaseRow.Title = title;
			}
			DatabaseRow.ServiceName = serviceName;
		}
		else
		{
			DatabaseRow = new DatabaseRow(data, getSSLSettings: true);
		}
	}

	public void Clear()
	{
		ConnectorControl?.ClearErrorProvider();
	}

	public bool TestConnection(bool testForGettingDatabasesList)
	{
		return ConnectorControl.TestConnection(testForGettingDatabasesList);
	}

	public void Save(IEnumerable<DocumentationCustomFieldRow> customFields = null, bool saveInsertFilter = false, bool isPersonalSettingsInsert = false, bool isManualDocumentaion = false)
	{
		if (isPersonalSettingsInsert)
		{
			if (IsDBAdded || isCopyingConnection == true)
			{
				DB.Database.InsertBaseInformation(DatabaseRow, FindForm());
				DB.History.InsertHistoryRow(DatabaseRow?.Id, DatabaseRow?.Id, DatabaseRow?.Title, null, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType.Database), saveTitle: true, saveDescription: false, SharedObjectTypeEnum.ObjectType.Database);
			}
			else if (isManualDocumentaion)
			{
				DB.Database.UpdateManualDocumentationBaseInfo(DatabaseRow, FindForm());
			}
			DB.UserPersonalSettings.Insert(GetPersonalSettings(saveInsertFilter), saveInsertFilter, FindForm());
		}
		else if (HasPersonalSettingsLoaded)
		{
			InsertOrUpdatePersonalSettings(saveInsertFilter);
		}
		else
		{
			SaveDatabase(customFields, saveInsertFilter);
		}
	}

	private void InsertOrUpdatePersonalSettings(bool saveInsertFilter)
	{
		if (GetPersonalSettingsDB(DatabaseRow.Id) == null)
		{
			DB.UserPersonalSettings.Insert(GetPersonalSettings(saveInsertFilter), saveInsertFilter, FindForm());
		}
		else
		{
			DB.UserPersonalSettings.Update(GetPersonalSettings(saveInsertFilter), saveInsertFilter, FindForm());
		}
		if (SelectedDatabaseType != initDatabaseType)
		{
			DB.Database.UpdateType(DatabaseRow.Id, DatabaseTypeEnum.TypeToString(SelectedDatabaseType), FindForm());
		}
	}

	public string GetConnectionType()
	{
		return ConnectionTypeService.GetConnectionType(SelectedDatabaseType, ConnectorControl.ConnectionType, DatabaseRow.SelectedAuthenticationType, ConnectorControl.GeneralConnectionType);
	}

	private UserPersonalSettings GetPersonalSettings(bool saveInsertFilter = false)
	{
		UserPersonalSettings settings = new UserPersonalSettings
		{
			LicenseId = StaticData.CurrentLicenseId,
			DatabaseId = (DatabaseRow.Id ?? (-1)),
			DatabaseName = DatabaseRow.Name,
			User = DatabaseRow.User
		};
		ConnectorControl.PreparePasswordForPersonalSettings();
		settings.Password = ConnectorControl.GetPasswordForUserPersonalSettings();
		settings.WindowsAuthentication = DatabaseRow.WindowsAutentication;
		settings.MultipleSchemas = DatabaseRow.HasMultipleSchemas.GetValueOrDefault();
		settings.DifferentSchema = DatabaseRow.UseDifferentSchema.GetValueOrDefault();
		settings.ConnectionType = ConnectionTypeService.GetConnectionType(DatabaseRow.Type, DatabaseRow.ConnectionType, DatabaseRow.SelectedAuthenticationType, ConnectorControl.GeneralConnectionType);
		settings.Host = DatabaseRow.Host;
		settings.Port = ConnectorControl.GetUserPersonalSettingsPort();
		settings.ServiceName = DatabaseRow.ServiceName;
		settings.OracleSid = DatabaseRow.OracleSid;
		settings.InstanceIdentifier = DatabaseRow.InstanceIdentifierString;
		settings.Filter = DatabaseRow.GetFilter(saveInsertFilter);
		settings.CertPath = DatabaseRow.SSLSettings?.CertPath;
		settings.CAPath = DatabaseRow.SSLSettings?.CAPath;
		settings.KeyPath = DatabaseRow.SSLSettings?.KeyPath;
		settings.Cipher = DatabaseRow.SSLSettings?.Cipher;
		settings.SSLType = DatabaseRow.SSLType;
		settings.ConnectionRole = DatabaseRow.ConnectionRole;
		settings.Perspective = DatabaseRow.Perspective;
		settings.Param1 = DatabaseRow.Param1;
		settings.Param2 = DatabaseRow.Param2;
		settings.Param3 = DatabaseRow.Param3;
		settings.Param4 = DatabaseRow.Param4;
		settings.Param5 = DatabaseRow.Param5;
		settings.Param6 = DatabaseRow.Param6;
		settings.Param7 = DatabaseRow.Param7;
		settings.Param8 = DatabaseRow.Param8;
		settings.Param9 = DatabaseRow.Param9;
		settings.Param10 = DatabaseRow.Param10;
		ConnectorControl.AdditionalUserPersonalSettingsPreparation(ref settings);
		return settings;
	}

	public void SaveDatabase(IEnumerable<DocumentationCustomFieldRow> customFields = null, bool saveInsertFilter = false)
	{
		string password = DatabaseRow.Password;
		bool flag = false;
		if (databaseRow.Type != SharedDatabaseTypeEnum.DatabaseType.Odbc && ConnectorControl.DoNotSavePassword())
		{
			DatabaseRow.Password = null;
			flag = true;
		}
		if (IsDBAdded || isCopyingConnection == true)
		{
			DB.Database.Insert(DatabaseRow, saveInsertFilter, FindForm());
			DB.History.InsertHistoryRow(DatabaseRow?.Id, DatabaseRow?.Id, DatabaseRow?.Title, null, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType.Database), saveTitle: true, saveDescription: false, SharedObjectTypeEnum.ObjectType.Database);
		}
		else if (StaticData.IsProjectFile)
		{
			DB.Database.UpdateCE(DatabaseRow, FindForm());
		}
		else
		{
			DB.Database.Update(DatabaseRow, FindForm());
		}
		if (flag)
		{
			DatabaseRow.Password = password;
		}
		if (customFields != null)
		{
			UpdateCustomFields(customFields);
		}
	}

	private void UpdateCustomFields(IEnumerable<DocumentationCustomFieldRow> customFields)
	{
		DB.CustomField.InsertOrUpdateDocumentationCustomFields(customFields, DatabaseRow.IdValue);
	}

	public void SetSwitchDatabaseAvailability(bool isAvailable)
	{
		ConnectorControl.SetSwitchDatabaseAvailability(isAvailable);
	}

	public void SetNewDBRowValues(bool forGettingDatabasesList = false)
	{
		ConnectorControl.SetNewDBRowValues(forGettingDatabasesList);
	}

	public void SetImportCommandsTimeout()
	{
		ConnectorControl.SetImportCommandsTimeout();
	}

	public string GetSSLTypeValue()
	{
		return ConnectorControl.GetSSLTypeValue();
	}

	public void HideOtherTypeFields()
	{
		SetConnectorControl();
		ConnectorControl?.HideOtherTypeFields();
	}

	public void SetPortFromHost()
	{
		ConnectorControl.SetPortFromHost();
	}

	private void DbConnect_Load(object sender, EventArgs e)
	{
		CommandsWithTimeout.SetDefaultTimeout();
	}

	private void saveAsComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (saveAsComboBoxEdit.SelectedIndex >= 0 && !isSettingParameters)
		{
			ChangeDatabaseRow(DatabaseRow.Id);
			ConnectorControl.ReadValues();
			HasPersonalSettingsLoaded = saveAsComboBoxEdit.SelectedIndex == 1;
		}
		displayFilterInfo?.Invoke();
		ConnectorControl?.AfterChangingUsedProfileType();
	}

	public bool PrepareSchemas()
	{
		return ConnectorControl.PrepareSchemas();
	}

	public string GetConnectionAdditionalText()
	{
		return ConnectorControl?.PreLearnMoreText;
	}

	public void SetDbmsControlTextWidth(int textWidth)
	{
		Size textSize = dbmsLayoutControlItem.TextSize;
		textSize.Width = textWidth;
		dbmsLayoutControlItem.TextSize = textSize;
	}

	public void PageCommited()
	{
		ConnectorControl?.PageCommited();
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
		this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true, typeof(System.Windows.Forms.UserControl));
		this.loginLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.sqlServerConnectorControl = new Dataedo.App.UserControls.ConnectorsControls.SQLServerConnectorControl();
		this.dbmsLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.saveAsComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.layoutControlGroup7 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.saveAsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.saveAsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.dbmsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem42 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem69 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.connectorLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.dbcconnectionLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.defaultToolTipController = new DevExpress.Utils.DefaultToolTipController(this.components);
		((System.ComponentModel.ISupportInitialize)this.loginLayoutControl).BeginInit();
		this.loginLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.saveAsComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveAsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveAsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem42).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem69).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectorLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.loginLayoutControl.AllowCustomization = false;
		this.loginLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.loginLayoutControl.Controls.Add(this.sqlServerConnectorControl);
		this.loginLayoutControl.Controls.Add(this.dbmsLabelControl);
		this.loginLayoutControl.Controls.Add(this.saveAsComboBoxEdit);
		this.loginLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.loginLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.loginLayoutControl.Margin = new System.Windows.Forms.Padding(2);
		this.loginLayoutControl.Name = "loginLayoutControl";
		this.loginLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(712, 331, 994, 806);
		this.loginLayoutControl.Root = this.layoutControlGroup7;
		this.loginLayoutControl.Size = new System.Drawing.Size(644, 468);
		this.loginLayoutControl.TabIndex = 0;
		this.loginLayoutControl.Text = "layoutControl1";
		this.defaultToolTipController.SetAllowHtmlText(this.sqlServerConnectorControl, DevExpress.Utils.DefaultBoolean.Default);
		this.sqlServerConnectorControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.sqlServerConnectorControl.Location = new System.Drawing.Point(2, 50);
		this.sqlServerConnectorControl.Margin = new System.Windows.Forms.Padding(2);
		this.sqlServerConnectorControl.Name = "sqlServerConnectorControl";
		this.sqlServerConnectorControl.Size = new System.Drawing.Size(640, 416);
		this.sqlServerConnectorControl.TabIndex = 30;
		this.dbmsLabelControl.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.dbmsLabelControl.Appearance.Options.UseBackColor = true;
		this.dbmsLabelControl.AutoEllipsis = true;
		this.dbmsLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.dbmsLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
		this.dbmsLabelControl.ImageOptions.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
		this.dbmsLabelControl.Location = new System.Drawing.Point(107, 2);
		this.dbmsLabelControl.Name = "dbmsLabelControl";
		this.dbmsLabelControl.Size = new System.Drawing.Size(229, 20);
		this.dbmsLabelControl.StyleController = this.loginLayoutControl;
		this.dbmsLabelControl.TabIndex = 27;
		this.dbmsLabelControl.Text = "labelControl";
		this.saveAsComboBoxEdit.Location = new System.Drawing.Point(107, 26);
		this.saveAsComboBoxEdit.Name = "saveAsComboBoxEdit";
		this.saveAsComboBoxEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.saveAsComboBoxEdit.Properties.Appearance.Options.UseBackColor = true;
		this.saveAsComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.saveAsComboBoxEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.saveAsComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.saveAsComboBoxEdit.StyleController = this.loginLayoutControl;
		this.saveAsComboBoxEdit.TabIndex = 26;
		this.saveAsComboBoxEdit.EditValueChanged += new System.EventHandler(saveAsComboBoxEdit_EditValueChanged);
		this.layoutControlGroup7.CustomizationFormText = "Root";
		this.layoutControlGroup7.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup7.GroupBordersVisible = false;
		this.layoutControlGroup7.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.saveAsLayoutControlItem, this.saveAsEmptySpaceItem, this.dbmsLayoutControlItem, this.emptySpaceItem42, this.emptySpaceItem69, this.connectorLayoutControlItem });
		this.layoutControlGroup7.Name = "Root";
		this.layoutControlGroup7.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup7.Size = new System.Drawing.Size(644, 468);
		this.layoutControlGroup7.TextVisible = false;
		this.saveAsLayoutControlItem.Control = this.saveAsComboBoxEdit;
		this.saveAsLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.saveAsLayoutControlItem.MaxSize = new System.Drawing.Size(338, 24);
		this.saveAsLayoutControlItem.MinSize = new System.Drawing.Size(338, 24);
		this.saveAsLayoutControlItem.Name = "saveAsLayoutControlItem";
		this.saveAsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 1, 2, 2);
		this.saveAsLayoutControlItem.Size = new System.Drawing.Size(338, 24);
		this.saveAsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.saveAsLayoutControlItem.Text = "Use profile:";
		this.saveAsLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.saveAsLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.saveAsLayoutControlItem.TextToControlDistance = 5;
		this.saveAsEmptySpaceItem.AllowHotTrack = false;
		this.saveAsEmptySpaceItem.Location = new System.Drawing.Point(338, 24);
		this.saveAsEmptySpaceItem.MinSize = new System.Drawing.Size(1, 1);
		this.saveAsEmptySpaceItem.Name = "saveAsEmptySpaceItem";
		this.saveAsEmptySpaceItem.Size = new System.Drawing.Size(306, 24);
		this.saveAsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.saveAsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.dbmsLayoutControlItem.Control = this.dbmsLabelControl;
		this.dbmsLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.dbmsLayoutControlItem.MaxSize = new System.Drawing.Size(338, 24);
		this.dbmsLayoutControlItem.MinSize = new System.Drawing.Size(338, 24);
		this.dbmsLayoutControlItem.Name = "dbmsLayoutControlItem";
		this.dbmsLayoutControlItem.Size = new System.Drawing.Size(338, 24);
		this.dbmsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.dbmsLayoutControlItem.Text = "DBMS:";
		this.dbmsLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.dbmsLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.dbmsLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem42.AllowHotTrack = false;
		this.emptySpaceItem42.Location = new System.Drawing.Point(338, 0);
		this.emptySpaceItem42.MaxSize = new System.Drawing.Size(8, 20);
		this.emptySpaceItem42.MinSize = new System.Drawing.Size(8, 20);
		this.emptySpaceItem42.Name = "emptySpaceItem42";
		this.emptySpaceItem42.Size = new System.Drawing.Size(8, 24);
		this.emptySpaceItem42.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem42.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem69.AllowHotTrack = false;
		this.emptySpaceItem69.Location = new System.Drawing.Point(346, 0);
		this.emptySpaceItem69.Name = "emptySpaceItem69";
		this.emptySpaceItem69.Size = new System.Drawing.Size(298, 24);
		this.emptySpaceItem69.TextSize = new System.Drawing.Size(0, 0);
		this.connectorLayoutControlItem.Control = this.sqlServerConnectorControl;
		this.connectorLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.connectorLayoutControlItem.Name = "connectorLayoutControlItem";
		this.connectorLayoutControlItem.Size = new System.Drawing.Size(644, 420);
		this.connectorLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.connectorLayoutControlItem.TextVisible = false;
		this.dbcconnectionLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.dbcconnectionLabelControl.Appearance.Options.UseFont = true;
		this.dbcconnectionLabelControl.Location = new System.Drawing.Point(12, 12);
		this.dbcconnectionLabelControl.Name = "dbcconnectionLabelControl";
		this.dbcconnectionLabelControl.Size = new System.Drawing.Size(620, 15);
		this.dbcconnectionLabelControl.StyleController = this.loginLayoutControl;
		this.dbcconnectionLabelControl.TabIndex = 10;
		this.dbcconnectionLabelControl.Text = "Connection";
		this.defaultToolTipController.DefaultController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.defaultToolTipController.SetAllowHtmlText(this, DevExpress.Utils.DefaultBoolean.Default);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		base.Controls.Add(this.loginLayoutControl);
		base.Name = "DbConnectUserControlNew";
		base.Size = new System.Drawing.Size(644, 468);
		base.Load += new System.EventHandler(DbConnect_Load);
		((System.ComponentModel.ISupportInitialize)this.loginLayoutControl).EndInit();
		this.loginLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.saveAsComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveAsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveAsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem42).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem69).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectorLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
