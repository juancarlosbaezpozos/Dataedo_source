using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.Enums;
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
using MongoDB.Driver;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class MongoDbConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl mongoDbLayoutControl;

	private MemoEdit mongoDBConnectionStringMemoEdit;

	private CheckEdit mongoDBSslTypeCheckEdit;

	private TextEdit mongoDBReplicaSetTextEdit;

	private TextEdit mongoDBDefaultAuthDBTextEdit;

	private TextEdit mongoDBPasswordTextEdit;

	private TextEdit mongoDBLoginTextEdit;

	private ComboBoxEdit mongoDBHostComboBoxEdit;

	private CheckEdit mongoDbPasswordCheckEdit;

	private CheckEdit mongoDBSrvCheckEdit;

	private ButtonEdit mongoDBDatabaseImportButtonEdit;

	private LabelControl mongoDBDatabasesCountLabelControl;

	private LookUpEdit mongoDBConnectionTypeLookUpEdit;

	private LayoutControlGroup Root;

	private LayoutControlItem mongoDBHostLayoutControlItem;

	private LayoutControlItem mongoDBLoginLayoutControlItem;

	private LayoutControlItem mongoDBPasswordLayoutControlItem;

	private EmptySpaceItem mongoDbEmptySpaceItem2;

	private LayoutControlItem mongoDbSavePasswordLayoutControlItem;

	private EmptySpaceItem mongoDBLoginLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem mongoDBPasswordLayoutControlItemEmptySpaceItem;

	private LayoutControlItem mongoDBImportDatabaseLayoutControlItem;

	private EmptySpaceItem mongoDBImportDatabaseLayoutControlItemEmptySpaceItem2;

	private LayoutControlItem mongoDBSrvLayoutControlItem;

	private EmptySpaceItem mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1;

	private LayoutControlItem mongoDBDatabasesCountLayoutControlItem;

	private LayoutControlItem mongoDBConnectionTypeLayoutControlItem;

	private EmptySpaceItem mongoDBConnectionTypeLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem mongoDBHostLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem mongoDbEmptySpaceItem;

	private EmptySpaceItem mongoDBSrvLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem mongoDBConnectionStringLayoutControlItemEmptySpaceItem;

	private LayoutControlItem mongoDBDefaultAuthDBLayoutControl;

	private LayoutControlItem mongoDBReplicaSetLayoutControl;

	private EmptySpaceItem mongoDBDefaultAuthDBLayoutControlEmptySpaceItem;

	private EmptySpaceItem mongoDbSavePasswordLayoutControlItemEmptySpaceItem;

	private LayoutControlItem mongoDBSslTypeLayoutControlItem;

	private EmptySpaceItem mongoDBReplicaSetLayoutControlEmptySpaceItem;

	private LayoutControlItem mongoDBConnectionStringLayoutControlItem;

	private string providedMongoDBHost => mongoDBHostComboBoxEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.MongoDB;

	public override GeneralConnectionTypeEnum.GeneralConnectionType GeneralConnectionType => GeneralConnectionTypeEnum.ParamStringToType(mongoDBConnectionTypeLookUpEdit.EditValue?.ToString());

	private bool? IsMongoDbConnectionStringMode
	{
		get
		{
			GeneralConnectionTypeEnum.GeneralConnectionType? generalConnectionType = mongoDBConnectionTypeLookUpEdit.EditValue as GeneralConnectionTypeEnum.GeneralConnectionType?;
			if (generalConnectionType.HasValue && generalConnectionType != GeneralConnectionTypeEnum.GeneralConnectionType.None)
			{
				return generalConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString;
			}
			return null;
		}
	}

	protected override TextEdit HostTextEdit => mongoDBHostComboBoxEdit;

	protected override ComboBoxEdit HostComboBoxEdit => mongoDBHostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => mongoDbPasswordCheckEdit;

	public MongoDbConnectorControl()
	{
		InitializeComponent();
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
		if (base.SelectedDatabaseType.HasValue)
		{
			SetGeneralConnectionTypes();
			SetMongoDBConnectionTypeFields();
		}
	}

	private void SetMongoDBConnectionTypeFields()
	{
		if (base.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.MongoDB || base.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB)
		{
			if (timeoutLayoutControlItem == null)
			{
				SetTimeoutSpinEdit();
			}
			bool? isMongoDbConnectionStringMode = IsMongoDbConnectionStringMode;
			LayoutControlItem layoutControlItem = mongoDBHostLayoutControlItem;
			LayoutControlItem layoutControlItem2 = mongoDBImportDatabaseLayoutControlItem;
			EmptySpaceItem emptySpaceItem = mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1;
			LayoutControlItem layoutControlItem3 = mongoDBDatabasesCountLayoutControlItem;
			LayoutVisibility layoutVisibility2 = (mongoDBImportDatabaseLayoutControlItemEmptySpaceItem2.Visibility = ((!isMongoDbConnectionStringMode.HasValue) ? LayoutVisibility.Never : LayoutVisibility.Always));
			LayoutVisibility layoutVisibility4 = (layoutControlItem3.Visibility = layoutVisibility2);
			LayoutVisibility layoutVisibility6 = (emptySpaceItem.Visibility = layoutVisibility4);
			LayoutVisibility layoutVisibility9 = (layoutControlItem.Visibility = (layoutControlItem2.Visibility = layoutVisibility6));
			LayoutControlItem layoutControlItem4 = mongoDBHostLayoutControlItem;
			EmptySpaceItem emptySpaceItem2 = mongoDBHostLayoutControlItemEmptySpaceItem;
			LayoutControlItem layoutControlItem5 = mongoDBSrvLayoutControlItem;
			EmptySpaceItem emptySpaceItem3 = mongoDBSrvLayoutControlItemEmptySpaceItem;
			LayoutControlItem layoutControlItem6 = mongoDBLoginLayoutControlItem;
			EmptySpaceItem emptySpaceItem4 = mongoDBLoginLayoutControlItemEmptySpaceItem;
			LayoutControlItem layoutControlItem7 = mongoDBPasswordLayoutControlItem;
			EmptySpaceItem emptySpaceItem5 = mongoDBPasswordLayoutControlItemEmptySpaceItem;
			LayoutControlItem layoutControlItem8 = mongoDbSavePasswordLayoutControlItem;
			EmptySpaceItem emptySpaceItem6 = mongoDbSavePasswordLayoutControlItemEmptySpaceItem;
			LayoutControlItem layoutControlItem9 = mongoDBDefaultAuthDBLayoutControl;
			EmptySpaceItem emptySpaceItem7 = mongoDBDefaultAuthDBLayoutControlEmptySpaceItem;
			LayoutControlItem layoutControlItem10 = mongoDBReplicaSetLayoutControl;
			EmptySpaceItem emptySpaceItem8 = mongoDBReplicaSetLayoutControlEmptySpaceItem;
			LayoutControlItem layoutControlItem11 = mongoDBSslTypeLayoutControlItem;
			LayoutVisibility layoutVisibility11 = (timeoutLayoutControlItem.Visibility = ((isMongoDbConnectionStringMode == true || !isMongoDbConnectionStringMode.HasValue) ? LayoutVisibility.Never : LayoutVisibility.Always));
			LayoutVisibility layoutVisibility13 = (layoutControlItem11.Visibility = layoutVisibility11);
			LayoutVisibility layoutVisibility15 = (emptySpaceItem8.Visibility = layoutVisibility13);
			LayoutVisibility layoutVisibility17 = (layoutControlItem10.Visibility = layoutVisibility15);
			LayoutVisibility layoutVisibility19 = (emptySpaceItem7.Visibility = layoutVisibility17);
			LayoutVisibility layoutVisibility21 = (layoutControlItem9.Visibility = layoutVisibility19);
			LayoutVisibility layoutVisibility23 = (emptySpaceItem6.Visibility = layoutVisibility21);
			LayoutVisibility layoutVisibility25 = (layoutControlItem8.Visibility = layoutVisibility23);
			LayoutVisibility layoutVisibility27 = (emptySpaceItem5.Visibility = layoutVisibility25);
			LayoutVisibility layoutVisibility29 = (layoutControlItem7.Visibility = layoutVisibility27);
			LayoutVisibility layoutVisibility31 = (emptySpaceItem4.Visibility = layoutVisibility29);
			layoutVisibility2 = (layoutControlItem6.Visibility = layoutVisibility31);
			layoutVisibility4 = (emptySpaceItem3.Visibility = layoutVisibility2);
			layoutVisibility6 = (layoutControlItem5.Visibility = layoutVisibility4);
			layoutVisibility9 = (layoutControlItem4.Visibility = (emptySpaceItem2.Visibility = layoutVisibility6));
			layoutVisibility9 = (mongoDBConnectionStringLayoutControlItem.Visibility = (mongoDBConnectionStringLayoutControlItemEmptySpaceItem.Visibility = ((isMongoDbConnectionStringMode != true) ? LayoutVisibility.Never : LayoutVisibility.Always)));
		}
	}

	private void SetGeneralConnectionTypes()
	{
		if (base.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.MongoDB || base.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB)
		{
			Dictionary<GeneralConnectionTypeEnum.GeneralConnectionType, string> generalConnectionTypes = GeneralConnectionTypeEnum.GetGeneralConnectionTypes();
			ConnectorControlBase.SetDropDownSize(mongoDBConnectionTypeLookUpEdit, generalConnectionTypes.Count);
			mongoDBConnectionTypeLookUpEdit.Properties.DataSource = generalConnectionTypes;
		}
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
			if ((base.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.MongoDB || base.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) && mongoDBConnectionTypeLookUpEdit.EditValue.Equals(GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString))
			{
				try
				{
					new MongoUrlBuilder(mongoDBConnectionStringMemoEdit.Text);
				}
				catch (MongoConfigurationException ex)
				{
					string text = " If the username or password includes the following characters:: / ? # [ ] @ those characters must be converted using percent encoding";
					GeneralMessageBoxesHandling.Show(ex.Message + text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
					return false;
				}
			}
			SetNewDBRowValues();
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
			ConnectionResult connectionResult = base.DatabaseRow.TryConnection();
			if (connectionResult.IsSuccess)
			{
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

	public override void PreparePasswordForPersonalSettings()
	{
		if ((base.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.MongoDB || base.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) && base.DatabaseRow.GeneralConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString)
		{
			base.DatabaseRow.Password = base.DatabaseRow.UserProvidedConnectionString;
		}
	}

	public override void AdditionalUserPersonalSettingsPreparation(ref UserPersonalSettings settings)
	{
		settings.ConnectionType = GeneralConnectionTypeEnum.TypeToString(base.DatabaseRow.GeneralConnectionType);
		if (base.DatabaseRow.GeneralConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString)
		{
			settings.User = null;
			settings.Host = null;
			settings.InstanceIdentifier = null;
			settings.Port = null;
		}
		else
		{
			settings.InstanceIdentifier = (base.DatabaseRow.MongoDBIsSrv ? SrvEnum.TypeToString(SrvEnum.Srv.SRV) : null);
		}
	}

	public override int? GetUserPersonalSettingsPort()
	{
		int? port = base.DatabaseRow.Port;
		if (!port.HasValue)
		{
			if ((base.DatabaseRow.Type != SharedDatabaseTypeEnum.DatabaseType.MongoDB && base.SelectedDatabaseType != SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) || base.DatabaseRow.GeneralConnectionType != GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString)
			{
				return 1433;
			}
			return null;
		}
		return port;
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		string name = DatabaseRow.PrepareSchemasList(schemas.ToList());
		if (IsMongoDbConnectionStringMode == true)
		{
			base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, name, documentationTitle, mongoDBConnectionStringMemoEdit.Text, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, string.IsNullOrEmpty(mongoDBDatabaseImportButtonEdit.Text), schemas.Count() > 1, null, null, schemas.ToList());
		}
		else
		{
			base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, name, documentationTitle, providedMongoDBHost, mongoDBLoginTextEdit.Text, mongoDBPasswordTextEdit.Text, null, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, mongoDBSrvCheckEdit.Checked, string.IsNullOrEmpty(mongoDBDatabaseImportButtonEdit.Text), schemas.Count() > 1, schemas.ToList(), mongoDBDefaultAuthDBTextEdit.Text, mongoDBReplicaSetTextEdit.Text, GetSSLTypeValue());
		}
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		mongoDbLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			mongoDbLayoutControl.Root.AddItem(timeoutLayoutControlItem, mongoDbEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateMongoDBConnectionType();
		if (IsMongoDbConnectionStringMode == false)
		{
			flag &= ValidateMongoDBHost();
		}
		else if (IsMongoDbConnectionStringMode == true)
		{
			flag &= ValidateMongoDBConnectionString();
		}
		return flag;
	}

	private bool ValidateMongoDBConnectionType(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(mongoDBConnectionTypeLookUpEdit, addDBErrorProvider, "connection type", acceptEmptyValue);
	}

	private bool ValidateMongoDBConnectionString(bool acceptEmptyValue = false)
	{
		if (!ValidateFields.ValidateEdit(mongoDBConnectionStringMemoEdit, addDBErrorProvider, "connection string", acceptEmptyValue))
		{
			return false;
		}
		try
		{
			new MongoUrlBuilder(mongoDBConnectionStringMemoEdit.Text);
		}
		catch (MongoConfigurationException)
		{
			string text = " If the username or password includes the following characters:: / ? # [ ] @ those characters must be converted using percent encoding";
			addDBErrorProvider.SetError(mongoDBConnectionStringMemoEdit, "The connection string is invalid!" + text);
		}
		return true;
	}

	private bool ValidateMongoDBHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(mongoDBHostComboBoxEdit, addDBErrorProvider, "host", acceptEmptyValue);
	}

	private bool ValidateMongoDBUsername(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(mongoDBLoginTextEdit, addDBErrorProvider, "username", acceptEmptyValue);
	}

	private bool ValidateMongoDBPassword(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(mongoDBPasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateMongoDBDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(mongoDBDatabaseImportButtonEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	protected override bool GetSavedPassword()
	{
		if (!GetSavePasswordCheckEditState())
		{
			if (base.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.MongoDB || base.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB)
			{
				return !string.IsNullOrEmpty(base.DatabaseRow.PasswordEncrypted);
			}
			return false;
		}
		return true;
	}

	public override string GetSSLTypeValue()
	{
		return mongoDBSslTypeCheckEdit.Checked.ToString();
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		mongoDBConnectionTypeLookUpEdit.EditValue = base.DatabaseRow.GeneralConnectionType;
		if (base.DatabaseRow.GeneralConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString)
		{
			mongoDBConnectionStringMemoEdit.Text = base.DatabaseRow.UserProvidedConnectionString;
		}
		else
		{
			mongoDBHostComboBoxEdit.Text = base.DatabaseRow.Host;
			mongoDBSrvCheckEdit.Checked = base.DatabaseRow.MongoDBIsSrv;
			mongoDBLoginTextEdit.Text = base.DatabaseRow.User;
			mongoDBPasswordTextEdit.Text = base.DatabaseRow.Password;
			mongoDbPasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
			mongoDBDefaultAuthDBTextEdit.Text = base.DatabaseRow.MongoDBAuthenticationDatabase;
			mongoDBReplicaSetTextEdit.Text = base.DatabaseRow.MongoDBReplicaSet;
			mongoDBHostComboBoxEdit.Text = base.DatabaseRow.MultiHost;
			mongoDBSslTypeCheckEdit.Checked = Convert.ToBoolean(base.DatabaseRow.SSLType);
		}
		if (base.DatabaseRow.ImportAllSchemas)
		{
			base.DatabaseRow.Name = null;
		}
		if (base.DatabaseRow.GeneralConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.None)
		{
			mongoDBConnectionStringMemoEdit.Text = null;
			mongoDBDatabaseImportButtonEdit.EditValue = base.DatabaseRow.Name;
		}
		schemas = new List<string>(base.DatabaseRow.Schemas ?? new List<string>());
		SetSchemasButtonEdit(mongoDBDatabaseImportButtonEdit);
		SetElementsLabelControl(mongoDBDatabasesCountLabelControl);
	}

	protected override string GetPanelDocumentationTitle()
	{
		string text = mongoDBDatabaseImportButtonEdit.Text;
		string text2 = null;
		if (IsMongoDbConnectionStringMode == true)
		{
			new MongoDBSupport();
			text2 = MongoDBSupport.GetHostFromConnectionString(mongoDBConnectionStringMemoEdit.Text);
		}
		else
		{
			text2 = providedMongoDBHost;
		}
		if (!string.IsNullOrEmpty(text))
		{
			return text + "@" + text2;
		}
		return text2 ?? "";
	}

	private void mongoDBConnectionTypeLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		SetMongoDBConnectionTypeFields();
	}

	private void mongoDBDatabaseImportButtonEdit_Leave(object sender, EventArgs e)
	{
		SetElementsLabelControl(mongoDBDatabasesCountLabelControl, "databases");
	}

	private void mongoDBDatabaseImportButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		schemas = ConnectorControlBase.GetPrintedSchemas(sender as ButtonEdit);
	}

	private void mongoDBDatabaseImportButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		HandleSchemaButtonEdit(sender as ButtonEdit, testConnection: true, "Databases");
	}

	private void mongoDBHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void mongoDBHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	public override void AfterChangingUsedProfileType()
	{
		SetMongoDBConnectionTypeFields();
	}

	protected override ButtonEdit GetButtonEditForSchemasPreparation()
	{
		if (base.DatabaseRow.Type != SharedDatabaseTypeEnum.DatabaseType.MongoDB && base.SelectedDatabaseType != SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB)
		{
			return null;
		}
		return mongoDBDatabaseImportButtonEdit;
	}

	protected override LabelControl GetLabelControlForSchemasPreparation()
	{
		if (base.DatabaseRow.Type != SharedDatabaseTypeEnum.DatabaseType.MongoDB && base.SelectedDatabaseType != SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB)
		{
			return null;
		}
		return mongoDBDatabasesCountLabelControl;
	}

	protected override void ClearPanelLoginAndPassword()
	{
		mongoDBLoginTextEdit.Text = string.Empty;
		mongoDBPasswordTextEdit.Text = string.Empty;
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
		this.mongoDbLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.mongoDBConnectionStringMemoEdit = new DevExpress.XtraEditors.MemoEdit();
		this.mongoDBSslTypeCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.mongoDBReplicaSetTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.mongoDBDefaultAuthDBTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.mongoDBPasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.mongoDBLoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.mongoDBHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.mongoDbPasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.mongoDBSrvCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.mongoDBDatabaseImportButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.mongoDBDatabasesCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.mongoDBConnectionTypeLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.mongoDBHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.mongoDBLoginLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.mongoDBPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.mongoDbEmptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mongoDbSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.mongoDBLoginLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mongoDBPasswordLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mongoDBImportDatabaseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mongoDBSrvLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mongoDBDatabasesCountLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.mongoDBConnectionTypeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.mongoDBConnectionTypeLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mongoDBHostLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mongoDbEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mongoDBSrvLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mongoDBConnectionStringLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mongoDBDefaultAuthDBLayoutControl = new DevExpress.XtraLayout.LayoutControlItem();
		this.mongoDBReplicaSetLayoutControl = new DevExpress.XtraLayout.LayoutControlItem();
		this.mongoDBDefaultAuthDBLayoutControlEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mongoDbSavePasswordLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mongoDBSslTypeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.mongoDBReplicaSetLayoutControlEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mongoDBConnectionStringLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mongoDbLayoutControl).BeginInit();
		this.mongoDbLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.mongoDBConnectionStringMemoEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBSslTypeCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBReplicaSetTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBDefaultAuthDBTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBPasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBLoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDbPasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBSrvCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBDatabaseImportButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBConnectionTypeLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBLoginLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBPasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDbEmptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDbSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBLoginLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBPasswordLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBImportDatabaseLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBSrvLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBDatabasesCountLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBConnectionTypeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBConnectionTypeLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBHostLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDbEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBSrvLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBConnectionStringLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBDefaultAuthDBLayoutControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBReplicaSetLayoutControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBDefaultAuthDBLayoutControlEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDbSavePasswordLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBSslTypeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBReplicaSetLayoutControlEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBConnectionStringLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.mongoDbLayoutControl.AllowCustomization = false;
		this.mongoDbLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mongoDbLayoutControl.Controls.Add(this.mongoDBConnectionStringMemoEdit);
		this.mongoDbLayoutControl.Controls.Add(this.mongoDBSslTypeCheckEdit);
		this.mongoDbLayoutControl.Controls.Add(this.mongoDBReplicaSetTextEdit);
		this.mongoDbLayoutControl.Controls.Add(this.mongoDBDefaultAuthDBTextEdit);
		this.mongoDbLayoutControl.Controls.Add(this.mongoDBPasswordTextEdit);
		this.mongoDbLayoutControl.Controls.Add(this.mongoDBLoginTextEdit);
		this.mongoDbLayoutControl.Controls.Add(this.mongoDBHostComboBoxEdit);
		this.mongoDbLayoutControl.Controls.Add(this.mongoDbPasswordCheckEdit);
		this.mongoDbLayoutControl.Controls.Add(this.mongoDBSrvCheckEdit);
		this.mongoDbLayoutControl.Controls.Add(this.mongoDBDatabaseImportButtonEdit);
		this.mongoDbLayoutControl.Controls.Add(this.mongoDBDatabasesCountLabelControl);
		this.mongoDbLayoutControl.Controls.Add(this.mongoDBConnectionTypeLookUpEdit);
		this.mongoDbLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mongoDbLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mongoDbLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.mongoDbLayoutControl.Name = "mongoDbLayoutControl";
		this.mongoDbLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3376, 367, 937, 686);
		this.mongoDbLayoutControl.Root = this.Root;
		this.mongoDbLayoutControl.Size = new System.Drawing.Size(624, 531);
		this.mongoDbLayoutControl.TabIndex = 1;
		this.mongoDbLayoutControl.Text = "layoutControl3";
		this.mongoDBConnectionStringMemoEdit.EditValue = "";
		this.mongoDBConnectionStringMemoEdit.Location = new System.Drawing.Point(105, 51);
		this.mongoDBConnectionStringMemoEdit.Name = "mongoDBConnectionStringMemoEdit";
		this.mongoDBConnectionStringMemoEdit.Size = new System.Drawing.Size(230, 82);
		this.mongoDBConnectionStringMemoEdit.StyleController = this.mongoDbLayoutControl;
		this.mongoDBConnectionStringMemoEdit.TabIndex = 37;
		this.mongoDBSslTypeCheckEdit.Location = new System.Drawing.Point(105, 304);
		this.mongoDBSslTypeCheckEdit.MaximumSize = new System.Drawing.Size(60, 24);
		this.mongoDBSslTypeCheckEdit.MinimumSize = new System.Drawing.Size(60, 0);
		this.mongoDBSslTypeCheckEdit.Name = "mongoDBSslTypeCheckEdit";
		this.mongoDBSslTypeCheckEdit.Properties.Caption = "Use tls";
		this.mongoDBSslTypeCheckEdit.Size = new System.Drawing.Size(60, 20);
		this.mongoDBSslTypeCheckEdit.StyleController = this.mongoDbLayoutControl;
		this.mongoDBSslTypeCheckEdit.TabIndex = 36;
		this.mongoDBReplicaSetTextEdit.Location = new System.Drawing.Point(105, 280);
		this.mongoDBReplicaSetTextEdit.Name = "mongoDBReplicaSetTextEdit";
		this.mongoDBReplicaSetTextEdit.Size = new System.Drawing.Size(230, 20);
		this.mongoDBReplicaSetTextEdit.StyleController = this.mongoDbLayoutControl;
		this.mongoDBReplicaSetTextEdit.TabIndex = 34;
		this.mongoDBDefaultAuthDBTextEdit.Location = new System.Drawing.Point(105, 256);
		this.mongoDBDefaultAuthDBTextEdit.Name = "mongoDBDefaultAuthDBTextEdit";
		this.mongoDBDefaultAuthDBTextEdit.Size = new System.Drawing.Size(230, 20);
		this.mongoDBDefaultAuthDBTextEdit.StyleController = this.mongoDbLayoutControl;
		this.mongoDBDefaultAuthDBTextEdit.TabIndex = 33;
		this.mongoDBPasswordTextEdit.Location = new System.Drawing.Point(105, 208);
		this.mongoDBPasswordTextEdit.Name = "mongoDBPasswordTextEdit";
		this.mongoDBPasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.mongoDBPasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.mongoDBPasswordTextEdit.StyleController = this.mongoDbLayoutControl;
		this.mongoDBPasswordTextEdit.TabIndex = 7;
		this.mongoDBLoginTextEdit.Location = new System.Drawing.Point(105, 184);
		this.mongoDBLoginTextEdit.Name = "mongoDBLoginTextEdit";
		this.mongoDBLoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.mongoDBLoginTextEdit.StyleController = this.mongoDbLayoutControl;
		this.mongoDBLoginTextEdit.TabIndex = 6;
		this.mongoDBHostComboBoxEdit.Location = new System.Drawing.Point(105, 24);
		this.mongoDBHostComboBoxEdit.Name = "mongoDBHostComboBoxEdit";
		this.mongoDBHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.mongoDBHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.mongoDBHostComboBoxEdit.StyleController = this.mongoDbLayoutControl;
		this.mongoDBHostComboBoxEdit.TabIndex = 4;
		this.mongoDBHostComboBoxEdit.EditValueChanged += new System.EventHandler(mongoDBHostComboBoxEdit_EditValueChanged);
		this.mongoDBHostComboBoxEdit.Leave += new System.EventHandler(mongoDBHostComboBoxEdit_Leave);
		this.mongoDbPasswordCheckEdit.Location = new System.Drawing.Point(105, 232);
		this.mongoDbPasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.mongoDbPasswordCheckEdit.Name = "mongoDbPasswordCheckEdit";
		this.mongoDbPasswordCheckEdit.Properties.Caption = "Save password";
		this.mongoDbPasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.mongoDbPasswordCheckEdit.StyleController = this.mongoDbLayoutControl;
		this.mongoDbPasswordCheckEdit.TabIndex = 3;
		this.mongoDBSrvCheckEdit.Location = new System.Drawing.Point(105, 160);
		this.mongoDBSrvCheckEdit.MaximumSize = new System.Drawing.Size(40, 19);
		this.mongoDBSrvCheckEdit.Name = "mongoDBSrvCheckEdit";
		this.mongoDBSrvCheckEdit.Properties.Caption = "SRV";
		this.mongoDBSrvCheckEdit.Size = new System.Drawing.Size(40, 19);
		this.mongoDBSrvCheckEdit.StyleController = this.mongoDbLayoutControl;
		this.mongoDBSrvCheckEdit.TabIndex = 3;
		this.mongoDBDatabaseImportButtonEdit.Location = new System.Drawing.Point(105, 136);
		this.mongoDBDatabaseImportButtonEdit.Name = "mongoDBDatabaseImportButtonEdit";
		this.mongoDBDatabaseImportButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.mongoDBDatabaseImportButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.mongoDBDatabaseImportButtonEdit.StyleController = this.mongoDbLayoutControl;
		this.mongoDBDatabaseImportButtonEdit.TabIndex = 4;
		this.mongoDBDatabaseImportButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(mongoDBDatabaseImportButtonEdit_ButtonClick);
		this.mongoDBDatabaseImportButtonEdit.EditValueChanged += new System.EventHandler(mongoDBDatabaseImportButtonEdit_EditValueChanged);
		this.mongoDBDatabaseImportButtonEdit.Leave += new System.EventHandler(mongoDBDatabaseImportButtonEdit_Leave);
		this.mongoDBDatabasesCountLabelControl.Location = new System.Drawing.Point(346, 138);
		this.mongoDBDatabasesCountLabelControl.Name = "mongoDBDatabasesCountLabelControl";
		this.mongoDBDatabasesCountLabelControl.Size = new System.Drawing.Size(137, 13);
		this.mongoDBDatabasesCountLabelControl.StyleController = this.mongoDbLayoutControl;
		this.mongoDBDatabasesCountLabelControl.TabIndex = 27;
		this.mongoDBConnectionTypeLookUpEdit.Location = new System.Drawing.Point(105, 0);
		this.mongoDBConnectionTypeLookUpEdit.Name = "mongoDBConnectionTypeLookUpEdit";
		this.mongoDBConnectionTypeLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.mongoDBConnectionTypeLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name")
		});
		this.mongoDBConnectionTypeLookUpEdit.Properties.DisplayMember = "Value";
		this.mongoDBConnectionTypeLookUpEdit.Properties.NullText = "";
		this.mongoDBConnectionTypeLookUpEdit.Properties.ShowFooter = false;
		this.mongoDBConnectionTypeLookUpEdit.Properties.ShowHeader = false;
		this.mongoDBConnectionTypeLookUpEdit.Properties.ShowLines = false;
		this.mongoDBConnectionTypeLookUpEdit.Properties.ValueMember = "Key";
		this.mongoDBConnectionTypeLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.mongoDBConnectionTypeLookUpEdit.StyleController = this.mongoDbLayoutControl;
		this.mongoDBConnectionTypeLookUpEdit.TabIndex = 0;
		this.mongoDBConnectionTypeLookUpEdit.EditValueChanged += new System.EventHandler(mongoDBConnectionTypeLookUpEdit_EditValueChanged);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[25]
		{
			this.mongoDBHostLayoutControlItem, this.mongoDBLoginLayoutControlItem, this.mongoDBPasswordLayoutControlItem, this.mongoDbEmptySpaceItem2, this.mongoDbSavePasswordLayoutControlItem, this.mongoDBLoginLayoutControlItemEmptySpaceItem, this.mongoDBPasswordLayoutControlItemEmptySpaceItem, this.mongoDBImportDatabaseLayoutControlItem, this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem2, this.mongoDBSrvLayoutControlItem,
			this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1, this.mongoDBDatabasesCountLayoutControlItem, this.mongoDBConnectionTypeLayoutControlItem, this.mongoDBConnectionTypeLayoutControlItemEmptySpaceItem, this.mongoDBHostLayoutControlItemEmptySpaceItem, this.mongoDbEmptySpaceItem, this.mongoDBSrvLayoutControlItemEmptySpaceItem, this.mongoDBConnectionStringLayoutControlItemEmptySpaceItem, this.mongoDBDefaultAuthDBLayoutControl, this.mongoDBReplicaSetLayoutControl,
			this.mongoDBDefaultAuthDBLayoutControlEmptySpaceItem, this.mongoDbSavePasswordLayoutControlItemEmptySpaceItem, this.mongoDBSslTypeLayoutControlItem, this.mongoDBReplicaSetLayoutControlEmptySpaceItem, this.mongoDBConnectionStringLayoutControlItem
		});
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(624, 531);
		this.Root.TextVisible = false;
		this.mongoDBHostLayoutControlItem.Control = this.mongoDBHostComboBoxEdit;
		this.mongoDBHostLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.mongoDBHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.mongoDBHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.mongoDBHostLayoutControlItem.Name = "mongoDBHostLayoutControlItem";
		this.mongoDBHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mongoDBHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.mongoDBHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDBHostLayoutControlItem.Text = "Host:";
		this.mongoDBHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mongoDBHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.mongoDBHostLayoutControlItem.TextToControlDistance = 5;
		this.mongoDBLoginLayoutControlItem.Control = this.mongoDBLoginTextEdit;
		this.mongoDBLoginLayoutControlItem.Location = new System.Drawing.Point(0, 184);
		this.mongoDBLoginLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.mongoDBLoginLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.mongoDBLoginLayoutControlItem.Name = "mongoDBLoginLayoutControlItem";
		this.mongoDBLoginLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mongoDBLoginLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.mongoDBLoginLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDBLoginLayoutControlItem.Text = "Username:";
		this.mongoDBLoginLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mongoDBLoginLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.mongoDBLoginLayoutControlItem.TextToControlDistance = 5;
		this.mongoDBPasswordLayoutControlItem.Control = this.mongoDBPasswordTextEdit;
		this.mongoDBPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 208);
		this.mongoDBPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.mongoDBPasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.mongoDBPasswordLayoutControlItem.Name = "mongoDBPasswordLayoutControlItem";
		this.mongoDBPasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mongoDBPasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.mongoDBPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDBPasswordLayoutControlItem.Text = "Password:";
		this.mongoDBPasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mongoDBPasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.mongoDBPasswordLayoutControlItem.TextToControlDistance = 5;
		this.mongoDbEmptySpaceItem2.AllowHotTrack = false;
		this.mongoDbEmptySpaceItem2.Location = new System.Drawing.Point(0, 521);
		this.mongoDbEmptySpaceItem2.MaxSize = new System.Drawing.Size(0, 10);
		this.mongoDbEmptySpaceItem2.MinSize = new System.Drawing.Size(104, 10);
		this.mongoDbEmptySpaceItem2.Name = "mongoDbEmptySpaceItem2";
		this.mongoDbEmptySpaceItem2.Size = new System.Drawing.Size(624, 10);
		this.mongoDbEmptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDbEmptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.mongoDbSavePasswordLayoutControlItem.Control = this.mongoDbPasswordCheckEdit;
		this.mongoDbSavePasswordLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.mongoDbSavePasswordLayoutControlItem.CustomizationFormText = "mongoDbSavePasswordLayoutControlItem";
		this.mongoDbSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 232);
		this.mongoDbSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.mongoDbSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.mongoDbSavePasswordLayoutControlItem.Name = "mongoDbSavePasswordLayoutControlItem";
		this.mongoDbSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mongoDbSavePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.mongoDbSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDbSavePasswordLayoutControlItem.Text = " ";
		this.mongoDbSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mongoDbSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.mongoDbSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.mongoDBLoginLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.mongoDBLoginLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 184);
		this.mongoDBLoginLayoutControlItemEmptySpaceItem.Name = "mongoDBLoginLayoutControlItemEmptySpaceItem";
		this.mongoDBLoginLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(289, 24);
		this.mongoDBLoginLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.mongoDBPasswordLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.mongoDBPasswordLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 208);
		this.mongoDBPasswordLayoutControlItemEmptySpaceItem.Name = "mongoDBPasswordLayoutControlItemEmptySpaceItem";
		this.mongoDBPasswordLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(289, 24);
		this.mongoDBPasswordLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.mongoDBImportDatabaseLayoutControlItem.Control = this.mongoDBDatabaseImportButtonEdit;
		this.mongoDBImportDatabaseLayoutControlItem.Location = new System.Drawing.Point(0, 136);
		this.mongoDBImportDatabaseLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.mongoDBImportDatabaseLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.mongoDBImportDatabaseLayoutControlItem.Name = "mongoDBImportDatabaseLayoutControlItem";
		this.mongoDBImportDatabaseLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mongoDBImportDatabaseLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.mongoDBImportDatabaseLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDBImportDatabaseLayoutControlItem.Text = "Database:";
		this.mongoDBImportDatabaseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mongoDBImportDatabaseLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.mongoDBImportDatabaseLayoutControlItem.TextToControlDistance = 5;
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem2.AllowHotTrack = false;
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem2.Location = new System.Drawing.Point(484, 136);
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem2.Name = "mongoDBImportDatabaseLayoutControlItemEmptySpaceItem2";
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem2.Size = new System.Drawing.Size(140, 24);
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.mongoDBSrvLayoutControlItem.Control = this.mongoDBSrvCheckEdit;
		this.mongoDBSrvLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.mongoDBSrvLayoutControlItem.CustomizationFormText = "mongoDBSrvLayoutControlItem";
		this.mongoDBSrvLayoutControlItem.Location = new System.Drawing.Point(0, 160);
		this.mongoDBSrvLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.mongoDBSrvLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.mongoDBSrvLayoutControlItem.Name = "mongoDBSrvLayoutControlItem";
		this.mongoDBSrvLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mongoDBSrvLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.mongoDBSrvLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDBSrvLayoutControlItem.Text = " ";
		this.mongoDBSrvLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mongoDBSrvLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.mongoDBSrvLayoutControlItem.TextToControlDistance = 5;
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1.AllowHotTrack = false;
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1.Location = new System.Drawing.Point(335, 136);
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1.MaxSize = new System.Drawing.Size(10, 24);
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1.MinSize = new System.Drawing.Size(10, 24);
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1.Name = "mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1";
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1.Size = new System.Drawing.Size(10, 24);
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.mongoDBDatabasesCountLayoutControlItem.Control = this.mongoDBDatabasesCountLabelControl;
		this.mongoDBDatabasesCountLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.mongoDBDatabasesCountLayoutControlItem.CustomizationFormText = "schemasCountLayoutControlItem";
		this.mongoDBDatabasesCountLayoutControlItem.Location = new System.Drawing.Point(345, 136);
		this.mongoDBDatabasesCountLayoutControlItem.Name = "mongoDBDatabasesCountLayoutControlItem";
		this.mongoDBDatabasesCountLayoutControlItem.Size = new System.Drawing.Size(139, 24);
		this.mongoDBDatabasesCountLayoutControlItem.Text = "schemasCountLayoutControlItem";
		this.mongoDBDatabasesCountLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.mongoDBDatabasesCountLayoutControlItem.TextVisible = false;
		this.mongoDBConnectionTypeLayoutControlItem.Control = this.mongoDBConnectionTypeLookUpEdit;
		this.mongoDBConnectionTypeLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.mongoDBConnectionTypeLayoutControlItem.CustomizationFormText = "Connection type:";
		this.mongoDBConnectionTypeLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.mongoDBConnectionTypeLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.mongoDBConnectionTypeLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.mongoDBConnectionTypeLayoutControlItem.Name = "mongoDBConnectionTypeLayoutControlItem";
		this.mongoDBConnectionTypeLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mongoDBConnectionTypeLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.mongoDBConnectionTypeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDBConnectionTypeLayoutControlItem.Text = "Connection type:";
		this.mongoDBConnectionTypeLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mongoDBConnectionTypeLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.mongoDBConnectionTypeLayoutControlItem.TextToControlDistance = 5;
		this.mongoDBConnectionTypeLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.mongoDBConnectionTypeLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 0);
		this.mongoDBConnectionTypeLayoutControlItemEmptySpaceItem.MinSize = new System.Drawing.Size(104, 24);
		this.mongoDBConnectionTypeLayoutControlItemEmptySpaceItem.Name = "mongoDBConnectionTypeLayoutControlItemEmptySpaceItem";
		this.mongoDBConnectionTypeLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(289, 24);
		this.mongoDBConnectionTypeLayoutControlItemEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDBConnectionTypeLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.mongoDBHostLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.mongoDBHostLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 24);
		this.mongoDBHostLayoutControlItemEmptySpaceItem.Name = "mongoDBHostLayoutControlItemEmptySpaceItem";
		this.mongoDBHostLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(289, 24);
		this.mongoDBHostLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.mongoDbEmptySpaceItem.AllowHotTrack = false;
		this.mongoDbEmptySpaceItem.Location = new System.Drawing.Point(0, 328);
		this.mongoDbEmptySpaceItem.MinSize = new System.Drawing.Size(104, 10);
		this.mongoDbEmptySpaceItem.Name = "mongoDbEmptySpaceItem";
		this.mongoDbEmptySpaceItem.Size = new System.Drawing.Size(624, 193);
		this.mongoDbEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDbEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.mongoDBSrvLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.mongoDBSrvLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 160);
		this.mongoDBSrvLayoutControlItemEmptySpaceItem.Name = "mongoDBSrvLayoutControlItemEmptySpaceItem";
		this.mongoDBSrvLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(289, 24);
		this.mongoDBSrvLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.mongoDBConnectionStringLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.mongoDBConnectionStringLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 48);
		this.mongoDBConnectionStringLayoutControlItemEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 88);
		this.mongoDBConnectionStringLayoutControlItemEmptySpaceItem.MinSize = new System.Drawing.Size(104, 88);
		this.mongoDBConnectionStringLayoutControlItemEmptySpaceItem.Name = "mongoDBConnectionStringLayoutControlItemEmptySpaceItem";
		this.mongoDBConnectionStringLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(289, 88);
		this.mongoDBConnectionStringLayoutControlItemEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDBConnectionStringLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.mongoDBDefaultAuthDBLayoutControl.Control = this.mongoDBDefaultAuthDBTextEdit;
		this.mongoDBDefaultAuthDBLayoutControl.Location = new System.Drawing.Point(0, 256);
		this.mongoDBDefaultAuthDBLayoutControl.MaxSize = new System.Drawing.Size(335, 24);
		this.mongoDBDefaultAuthDBLayoutControl.MinSize = new System.Drawing.Size(335, 24);
		this.mongoDBDefaultAuthDBLayoutControl.Name = "mongoDBDefaultAuthDBLayoutControl";
		this.mongoDBDefaultAuthDBLayoutControl.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mongoDBDefaultAuthDBLayoutControl.Size = new System.Drawing.Size(335, 24);
		this.mongoDBDefaultAuthDBLayoutControl.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDBDefaultAuthDBLayoutControl.Text = "Auth database:";
		this.mongoDBDefaultAuthDBLayoutControl.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mongoDBDefaultAuthDBLayoutControl.TextSize = new System.Drawing.Size(100, 13);
		this.mongoDBDefaultAuthDBLayoutControl.TextToControlDistance = 5;
		this.mongoDBReplicaSetLayoutControl.Control = this.mongoDBReplicaSetTextEdit;
		this.mongoDBReplicaSetLayoutControl.Location = new System.Drawing.Point(0, 280);
		this.mongoDBReplicaSetLayoutControl.MaxSize = new System.Drawing.Size(335, 24);
		this.mongoDBReplicaSetLayoutControl.MinSize = new System.Drawing.Size(335, 24);
		this.mongoDBReplicaSetLayoutControl.Name = "mongoDBReplicaSetLayoutControl";
		this.mongoDBReplicaSetLayoutControl.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mongoDBReplicaSetLayoutControl.Size = new System.Drawing.Size(335, 24);
		this.mongoDBReplicaSetLayoutControl.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDBReplicaSetLayoutControl.Text = "Replica set:";
		this.mongoDBReplicaSetLayoutControl.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mongoDBReplicaSetLayoutControl.TextSize = new System.Drawing.Size(100, 13);
		this.mongoDBReplicaSetLayoutControl.TextToControlDistance = 5;
		this.mongoDBDefaultAuthDBLayoutControlEmptySpaceItem.AllowHotTrack = false;
		this.mongoDBDefaultAuthDBLayoutControlEmptySpaceItem.Location = new System.Drawing.Point(335, 256);
		this.mongoDBDefaultAuthDBLayoutControlEmptySpaceItem.Name = "mongoDBDefaultAuthDBLayoutControlEmptySpaceItem";
		this.mongoDBDefaultAuthDBLayoutControlEmptySpaceItem.Size = new System.Drawing.Size(289, 24);
		this.mongoDBDefaultAuthDBLayoutControlEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.mongoDbSavePasswordLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.mongoDbSavePasswordLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 232);
		this.mongoDbSavePasswordLayoutControlItemEmptySpaceItem.Name = "mongoDbSavePasswordLayoutControlItemEmptySpaceItem";
		this.mongoDbSavePasswordLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(289, 24);
		this.mongoDbSavePasswordLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.mongoDBSslTypeLayoutControlItem.Control = this.mongoDBSslTypeCheckEdit;
		this.mongoDBSslTypeLayoutControlItem.Location = new System.Drawing.Point(0, 304);
		this.mongoDBSslTypeLayoutControlItem.MaxSize = new System.Drawing.Size(60, 24);
		this.mongoDBSslTypeLayoutControlItem.MinSize = new System.Drawing.Size(55, 20);
		this.mongoDBSslTypeLayoutControlItem.Name = "mongoDBSslTypeLayoutControlItem";
		this.mongoDBSslTypeLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mongoDBSslTypeLayoutControlItem.Size = new System.Drawing.Size(624, 24);
		this.mongoDBSslTypeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDBSslTypeLayoutControlItem.Text = " ";
		this.mongoDBSslTypeLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mongoDBSslTypeLayoutControlItem.TextSize = new System.Drawing.Size(100, 0);
		this.mongoDBSslTypeLayoutControlItem.TextToControlDistance = 5;
		this.mongoDBReplicaSetLayoutControlEmptySpaceItem.AllowHotTrack = false;
		this.mongoDBReplicaSetLayoutControlEmptySpaceItem.Location = new System.Drawing.Point(335, 280);
		this.mongoDBReplicaSetLayoutControlEmptySpaceItem.Name = "mongoDBReplicaSetLayoutControlEmptySpaceItem";
		this.mongoDBReplicaSetLayoutControlEmptySpaceItem.Size = new System.Drawing.Size(289, 24);
		this.mongoDBReplicaSetLayoutControlEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.mongoDBConnectionStringLayoutControlItem.Control = this.mongoDBConnectionStringMemoEdit;
		this.mongoDBConnectionStringLayoutControlItem.CustomizationFormText = "Connection string:";
		this.mongoDBConnectionStringLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.mongoDBConnectionStringLayoutControlItem.MaxSize = new System.Drawing.Size(335, 88);
		this.mongoDBConnectionStringLayoutControlItem.MinSize = new System.Drawing.Size(335, 88);
		this.mongoDBConnectionStringLayoutControlItem.Name = "mongoDBConnectionStringLayoutControlItem";
		this.mongoDBConnectionStringLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 3, 3);
		this.mongoDBConnectionStringLayoutControlItem.Size = new System.Drawing.Size(335, 88);
		this.mongoDBConnectionStringLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mongoDBConnectionStringLayoutControlItem.Text = "Connection string:";
		this.mongoDBConnectionStringLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mongoDBConnectionStringLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.mongoDBConnectionStringLayoutControlItem.TextToControlDistance = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mongoDbLayoutControl);
		base.Name = "MongoDbConnectorControl";
		base.Size = new System.Drawing.Size(624, 531);
		((System.ComponentModel.ISupportInitialize)this.mongoDbLayoutControl).EndInit();
		this.mongoDbLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.mongoDBConnectionStringMemoEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBSslTypeCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBReplicaSetTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBDefaultAuthDBTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBPasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBLoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDbPasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBSrvCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBDatabaseImportButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBConnectionTypeLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBLoginLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBPasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDbEmptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDbSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBLoginLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBPasswordLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBImportDatabaseLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBSrvLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBImportDatabaseLayoutControlItemEmptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBDatabasesCountLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBConnectionTypeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBConnectionTypeLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBHostLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDbEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBSrvLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBConnectionStringLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBDefaultAuthDBLayoutControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBReplicaSetLayoutControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBDefaultAuthDBLayoutControlEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDbSavePasswordLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBSslTypeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBReplicaSetLayoutControlEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mongoDBConnectionStringLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
