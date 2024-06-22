using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Base;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomMessageBox;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Shared.DatabasesSupport;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class ConnectorControlBase : BaseUserControl
{
	protected SpinEdit timeoutSpinEdit;

	protected LayoutControlItem timeoutLayoutControlItem;

	protected DXErrorProvider addDBErrorProvider;

	protected HostAndPort splittedHost;

	protected bool isDatabaseUpdating;

	protected bool clearLoginAndPassword;

	protected IEnumerable<string> schemas = new List<string>();

	protected string lastProvidedLogin = string.Empty;

	protected DbConnectUserControlNew parentControl;

	private IContainer components;

	private SplashScreenManager splashScreenManager1;

	public virtual ConnectionTypeEnum.ConnectionType ConnectionType => ConnectionTypeEnum.ConnectionType.Direct;

	public virtual GeneralConnectionTypeEnum.GeneralConnectionType GeneralConnectionType => GeneralConnectionTypeEnum.GeneralConnectionType.None;

	public bool HideSavingCredentials { get; protected set; }

	protected virtual PanelTypeEnum.PanelType PanelType
	{
		get
		{
			throw new MustBeOverriddenException(MethodBase.GetCurrentMethod().Name);
		}
	}

	protected SplashScreenManager SplashScreenManager => splashScreenManager1;

	protected bool IsDBAdded => parentControl.IsDBAdded;

	protected bool IsCopyingConnection => parentControl.isCopyingConnection == true;

	protected virtual bool ShouldShowWaitingPanelWhileConnect => true;

	protected SharedDatabaseTypeEnum.DatabaseType? SelectedDatabaseType
	{
		get
		{
			return parentControl.SelectedDatabaseType;
		}
		set
		{
			parentControl.SelectedDatabaseType = value;
		}
	}

	protected DatabaseRow DatabaseRow
	{
		get
		{
			return parentControl.DatabaseRow;
		}
		set
		{
			parentControl.DatabaseRow = value;
		}
	}

	protected virtual TextEdit HostTextEdit => null;

	protected virtual TextEdit PortTextEdit => null;

	protected virtual ComboBoxEdit HostComboBoxEdit => null;

	protected virtual CheckEdit SavePasswordCheckEdit => null;

	protected virtual string ValidationErrorMessage => "Required fields are not filled in.";

	public virtual string PreLearnMoreText => null;

	public ConnectorControlBase()
	{
		InitializeComponent();
	}

	public virtual bool TestConnection(bool testForGettingDatabasesList)
	{
		try
		{
			if (!ValidateRequiredFields(testForGettingDatabasesList))
			{
				GeneralMessageBoxesHandling.Show(ValidationErrorMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
				return false;
			}
			SetNewDBRowValues();
			if (ShouldShowWaitingPanelWhileConnect)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: true);
			}
			ConnectionResult connectionResult = DatabaseRow.TryConnection();
			if (connectionResult.IsSuccess)
			{
				DatabaseRow.Connection = connectionResult.Connection;
				SetDBMSVersion();
				CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: false);
				SharedDatabaseTypeEnum.DatabaseType? expectedDatabaseType = GetExpectedDatabaseType();
				if (ShouldShowWaitingPanelWhileConnect)
				{
					CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: true);
				}
				if (!expectedDatabaseType.HasValue)
				{
					return false;
				}
				if (expectedDatabaseType != DatabaseRow.Type)
				{
					DatabaseRow.Type = expectedDatabaseType;
					SelectedDatabaseType = expectedDatabaseType;
				}
				return true;
			}
			CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: false);
			GeneralMessageBoxesHandling.Show(connectionResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, connectionResult.Details, 1, messageSimple: connectionResult.MessageSimple, owner: FindForm());
			return false;
		}
		catch
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: false);
			throw;
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: false);
		}
	}

	protected SharedDatabaseTypeEnum.DatabaseType? GetExpectedDatabaseType()
	{
		SharedDatabaseTypeEnum.DatabaseType? version = DatabaseRow.GetVersion(FindForm());
		if (version.HasValue)
		{
			StaticData.CrashedDatabaseType = version;
		}
		return version;
	}

	protected void SetDBMSVersion()
	{
		DatabaseRow.DbmsVersion = DatabaseRow.GetDbmsVersion(FindForm());
		StaticData.CrashedDBMSVersion = DatabaseRow.DbmsVersion;
	}

	public virtual void SetSwitchDatabaseAvailability(bool isAvailable)
	{
	}

	public virtual string GetSSLTypeValue()
	{
		return string.Empty;
	}

	public virtual void AfterChangingUsedProfileType()
	{
	}

	public virtual void PreparePasswordForPersonalSettings()
	{
	}

	public virtual void AdditionalUserPersonalSettingsPreparation(ref UserPersonalSettings settings)
	{
	}

	public virtual int? GetUserPersonalSettingsPort()
	{
		return DatabaseRow.Port ?? 1433;
	}

	public virtual void SetTimeoutControlPosition()
	{
	}

	public virtual void HideOtherTypeFields()
	{
	}

	public virtual void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		addDBErrorProvider = new DXErrorProvider();
		SetAuthenticationDataSource();
		if (databaseId.HasValue)
		{
			ReadValues();
		}
		if (databaseId.HasValue && isCopyingConnection == false)
		{
			isDatabaseUpdating = true;
		}
		SetTimeoutControlPosition();
		if (SelectedDatabaseType.HasValue)
		{
			SetHostComboBox();
			SetPortFromHost();
		}
	}

	public void ReadValues()
	{
		if (DatabaseRow != null)
		{
			PanelTypeEnum.PanelType panelType = PanelTypeEnum.PanelType.None;
			if (DatabaseSupportFactoryShared.CheckIfTypeIsSupported(DatabaseRow.Type))
			{
				panelType = DatabaseSupportFactory.GetDatabaseSupport(DatabaseRow.Type).PanelType;
			}
			if (panelType == PanelType)
			{
				ReadPanelValues();
			}
		}
	}

	public string GetDocumentationTitle()
	{
		if (!IsDBAdded && parentControl.isCopyingConnection != true)
		{
			return DatabaseRow.Title;
		}
		return GetPanelDocumentationTitle();
	}

	public void SetParentControl(DbConnectUserControlNew parentControl)
	{
		this.parentControl = parentControl;
	}

	public void SetPortFromHost()
	{
		if (SelectedDatabaseType.HasValue && HostTextEdit != null)
		{
			TextEdit hostTextEdit = HostTextEdit;
			splittedHost = ConnectionHelper.GetSplittedHost(hostTextEdit?.Text);
			if (splittedHost != null && PortTextEdit != null)
			{
				PortTextEdit.Text = splittedHost.Port;
				hostTextEdit.Text = splittedHost.Host;
			}
		}
	}

	public bool ValidateRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		ClearErrorProvider();
		if (!SelectedDatabaseType.HasValue)
		{
			return false;
		}
		return ValidatePanelRequiredFields(testForGettingDatabasesList, testForGettingWarehousesList, testForGettingPerspectiveList);
	}

	public void ClearErrorProvider()
	{
		addDBErrorProvider.ClearErrors();
	}

	public string GetPasswordForUserPersonalSettings()
	{
		if (!GetSavedPassword())
		{
			return string.Empty;
		}
		return DatabaseRow.PasswordEncrypted;
	}

	public bool PrepareSchemas()
	{
		DatabaseRow.Schemas = DatabaseRow.Schemas?.GroupBy((string x) => x)?.Select((IGrouping<string, string> x) => x.Key)?.ToList();
		bool num = DatabaseSupportFactory.GetDatabaseSupport(DatabaseRow.Type).PrepareElements(DatabaseRow, ref schemas, GetButtonEditForSchemasPreparation(), GetLabelControlForSchemasPreparation(), FindForm());
		if (!num)
		{
			ValidateRequiredFields(testForGettingDatabasesList: false);
		}
		return num;
	}

	public void SetNewDBRowValues(bool forGettingDatabasesList = false)
	{
		if (!IsDBAdded)
		{
			DatabaseRow.ConnectAndSynchronizeState = SynchConnectStateEnum.SynchConnectStateType.New;
		}
		SetPanelNewDBRowValues(forGettingDatabasesList);
	}

	public bool DoNotSavePassword()
	{
		return !GetSavePasswordCheckEditState();
	}

	public void SetImportCommandsTimeout()
	{
		int timeout = 0;
		try
		{
			if (timeoutSpinEdit != null)
			{
				timeout = decimal.ToInt32(timeoutSpinEdit.Value);
			}
		}
		catch (Exception)
		{
			CommandsWithTimeout.SetDefaultTimeout();
		}
		if (timeoutSpinEdit != null && timeoutSpinEdit.Visible)
		{
			CommandsWithTimeout.Timeout = timeout;
			return;
		}
		CommandsWithTimeout.SetDefaultTimeout();
		CommandsWithTimeout.SetNotToUseTimeout();
	}

	protected static void SetDropDownSize(LookUpEdit lookUpEdit, int count)
	{
		lookUpEdit.Properties.DropDownRows = count;
	}

	protected static IEnumerable<string> GetPrintedSchemas(ButtonEdit buttonEdit)
	{
		IEnumerable<string> result = new List<string>();
		string[] array = buttonEdit.EditValue?.ToString().Split(',');
		if (array != null)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
			}
			result = array;
		}
		return result;
	}

	protected static Func<IEnumerable<string>, bool> GetValidateValueFunction(string objectName)
	{
		if (!StaticData.IsProjectFile)
		{
			return null;
		}
		return delegate(IEnumerable<string> x)
		{
			if (DatabaseRow.PrepareSchemasList(x).Length > 4000)
			{
				CustomMessageBoxForm.Show("Too many " + objectName + " selected to correctly import to a file repository." + Environment.NewLine + Environment.NewLine + "Deselect some " + objectName + " to continue, or use <href=" + Links.CreatingServerRepository + ">server repository</href> to import this many " + objectName + ".", "Too many " + objectName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return false;
			}
			return true;
		};
	}

	protected static bool CheckSchemaExistence(ButtonEdit buttonEdit, List<string> source)
	{
		string[] array = buttonEdit.EditValue?.ToString().Split(',');
		foreach (string text in array)
		{
			string schemaTest = text.Trim().ToLower();
			if (!source.Any((string x) => x.ToLower().Equals(schemaTest)))
			{
				return false;
			}
		}
		return true;
	}

	protected virtual void ReadPanelValues()
	{
		throw new MustBeOverriddenException(MethodBase.GetCurrentMethod().Name);
	}

	protected virtual bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		throw new MustBeOverriddenException(MethodBase.GetCurrentMethod().Name);
	}

	protected virtual bool GetSavedPassword()
	{
		return GetSavePasswordCheckEditState();
	}

	protected virtual string GetPanelDocumentationTitle()
	{
		throw new MustBeOverriddenException(MethodBase.GetCurrentMethod().Name);
	}

	protected virtual ButtonEdit GetButtonEditForSchemasPreparation()
	{
		return null;
	}

	protected virtual LabelControl GetLabelControlForSchemasPreparation()
	{
		return null;
	}

	protected virtual void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		throw new MustBeOverriddenException(MethodBase.GetCurrentMethod().Name);
	}

	protected virtual void SetAuthenticationDataSource()
	{
	}

	protected virtual void ClearLoginAndPassword()
	{
		if (isDatabaseUpdating && clearLoginAndPassword)
		{
			ClearPanelLoginAndPassword();
			clearLoginAndPassword = false;
		}
	}

	protected virtual void ClearPanelLoginAndPassword()
	{
	}

	protected void SetHostComboBox()
	{
		List<string> savedHostsForDatabaseType = DB.Database.GetSavedHostsForDatabaseType(DatabaseTypeEnum.TypeToString(SelectedDatabaseType), FindForm());
		if (savedHostsForDatabaseType != null && HostComboBoxEdit != null)
		{
			HostComboBoxEdit.Properties.Items.Clear();
			HostComboBoxEdit.Properties.Items.AddRange(savedHostsForDatabaseType);
		}
	}

	protected void SetTimeoutSpinEdit()
	{
		timeoutSpinEdit = new SpinEdit
		{
			EditValue = CommandsWithTimeout.Timeout,
			Margin = new System.Windows.Forms.Padding(4),
			MaximumSize = new Size(60, 0),
			Name = "timeoutSpinEdit"
		};
		timeoutSpinEdit.Properties.Increment = new decimal(new int[4] { 10, 0, 0, 0 });
		timeoutSpinEdit.Properties.IsFloatValue = false;
		timeoutSpinEdit.Properties.Mask.EditMask = "N00";
		timeoutSpinEdit.Properties.MaxValue = new decimal(new int[4] { 600, 0, 0, 0 });
		timeoutSpinEdit.Properties.MinValue = new decimal(new int[4] { 1, 0, 0, 0 });
		timeoutSpinEdit.Size = new Size(80, 24);
		timeoutLayoutControlItem = new LayoutControlItem
		{
			Control = timeoutSpinEdit,
			CustomizationFormText = "Timeout (s):",
			Name = "timeoutLayoutControlItem",
			Padding = new DevExpress.XtraLayout.Utils.Padding(0, 3, 2, 2),
			Text = "Timeout (s):",
			TextAlignMode = TextAlignModeItem.CustomSize,
			TextSize = new Size(100, 13),
			TextToControlDistance = 5
		};
	}

	protected bool GetSavePasswordCheckEditState()
	{
		bool result = false;
		if (SavePasswordCheckEdit != null)
		{
			result = SavePasswordCheckEdit.Checked;
		}
		return result;
	}

	protected void PortTextEdit_Leave(object sender, EventArgs e)
	{
		ClearLoginAndPassword();
	}

	protected void hostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		SetPortFromHost();
	}

	protected void HostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (isDatabaseUpdating && sender is TextEdit textEdit && textEdit.ContainsFocus)
		{
			clearLoginAndPassword = true;
		}
	}

	protected void PortTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (isDatabaseUpdating && sender is TextEdit textEdit && textEdit.ContainsFocus)
		{
			clearLoginAndPassword = true;
		}
	}

	protected void SetSchemasButtonEdit(ButtonEdit schemasButtonEdit)
	{
		DatabaseSupportBase.SetElementsButtonEdit(schemasButtonEdit, schemas);
	}

	protected void SetElementsLabelControl(LabelControl schemasLabelControl, string elementsName = "schemas")
	{
		DatabaseSupportBase.SetElementsLabelControl(schemasLabelControl, schemas, elementsName);
	}

	protected void HandleSchemaButtonEdit(ButtonEdit buttonEdit, bool testConnection, string objectName, LabelControl labelControl = null, string dialogTitle = null)
	{
		if (!ValidateRequiredFields(testForGettingDatabasesList: true))
		{
			GeneralMessageBoxesHandling.Show("Required fields are not filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
		}
		else
		{
			if (testConnection && !TestConnection(testForGettingDatabasesList: true))
			{
				return;
			}
			CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: true);
			SetNewDBRowValues(forGettingDatabasesList: true);
			List<string> databases = DatabaseRow.GetDatabases(SplashScreenManager, forceStandardConnection: false, FindForm());
			CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: false);
			if (databases == null)
			{
				return;
			}
			try
			{
				ConnectionResult connectionResult = DatabaseRow.TryConnection();
				if (connectionResult.Exception != null)
				{
					throw connectionResult.Exception;
				}
				IEnumerable<string> printedSchemas = GetPrintedSchemas(buttonEdit);
				CheckedListForm checkedListForm = new CheckedListForm(databases, printedSchemas, string.IsNullOrWhiteSpace(dialogTitle) ? (objectName + " list") : dialogTitle, GetValidateValueFunction(objectName.ToLower()));
				if (checkedListForm.ShowDialog(this, setCustomMessageDefaultOwner: true) != DialogResult.OK)
				{
					schemas = printedSchemas;
					return;
				}
				schemas = checkedListForm.CheckedValues;
				SetSchemasButtonEdit(buttonEdit);
				if (labelControl != null)
				{
					SetElementsLabelControl(labelControl, objectName);
				}
				CheckSchemaExistence(buttonEdit, databases);
			}
			catch (Exception ex)
			{
				GeneralMessageBoxesHandling.Show(DatabaseRow.GetOracleExceptionMessage(ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
			}
		}
	}

	protected virtual void ClearDatabaseData()
	{
		DatabaseRow.Name = string.Empty;
	}

	protected void DatabaseButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		ButtonEdit buttonEdit = sender as ButtonEdit;
		if (!ValidateRequiredFields(testForGettingDatabasesList: true))
		{
			GeneralMessageBoxesHandling.Show("Required fields are not filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
			return;
		}
		CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: true);
		SetNewDBRowValues(forGettingDatabasesList: true);
		List<string> databases = DatabaseRow.GetDatabases(SplashScreenManager, forceStandardConnection: false, FindForm());
		CommonFunctionsDatabase.SetWaitFormVisibility(SplashScreenManager, show: false);
		if (databases == null)
		{
			return;
		}
		string empty = string.Empty;
		try
		{
			ClearDatabaseData();
			ConnectionResult connectionResult = DatabaseRow.TryConnection(useOnlyRequiredFields: true);
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
			DatabaseSupportFactory.GetDatabaseSupport(DatabaseRow.Type).ProcessException(ex, DatabaseRow.Name, DatabaseRow.ServiceName, FindForm());
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
		}
		finally
		{
			DatabaseRow.Name = empty;
		}
	}

	public void SetTheme()
	{
		ApplyStyle();
	}

	protected void SetDbmsControlTextWidth(int textWidth)
	{
		parentControl.SetDbmsControlTextWidth(textWidth);
	}

	public virtual void PageCommited()
	{
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
		this.splashScreenManager1 = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true, typeof(System.Windows.Forms.UserControl));
		base.SuspendLayout();
		this.splashScreenManager1.ClosingDelay = 500;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		base.Name = "ConnectorControlBase";
		base.Size = new System.Drawing.Size(153, 128);
		base.ResumeLayout(false);
	}
}
