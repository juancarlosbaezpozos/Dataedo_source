using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Helpers.CloudStorage.AzureStorage;
using Dataedo.App.Helpers.CloudStorage.AzureStorage.Connections;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using Microsoft.WindowsAzure.Storage;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class AzureStorageConnectorControl : ConnectorControlBase
{
	private Dictionary<AzureStorageAuthentication.AzureStorageAuthenticationEnum, IList<LayoutControlItem>> _controlsToShow;

	private Dictionary<AzureStorageAuthentication.AzureStorageAuthenticationEnum, IList<BaseEdit>> _editorsToCleanAfterChange;

	private List<LayoutControlItem> _hideableControls;

	private IContainer components;

	private NonCustomizableLayoutControl azureStorageLayoutControl;

	private TextEdit accessKeyTextEdit;

	private CheckEdit savePasswordCheckEdit;

	private ComboBoxEdit accountNameComboBoxEdit;

	private LayoutControlGroup layoutControlGroup6;

	private LayoutControlItem accessKeyLayoutControlItem;

	private LayoutControlItem savePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem34;

	private LayoutControlItem accountNameLayoutControlItem;

	private LayoutControlItem authenticationLayoutControlItem;

	private ImageComboBoxEdit authenticationImageComboBoxEdit;

	private EmptySpaceItem emptySpaceItem5;

	private TextEdit sharedAccessSignatureTextEdit;

	private LayoutControlItem sharedAccessSignatureLayoutControlItem;

	private TextEdit connectionStringTextEdit;

	private LayoutControlItem connectionStringLayoutControlItem;

	private TextEdit containerNameTextEdit;

	private LayoutControlItem containerNameLayoutControlItem;

	private TextEdit pathTextEdit;

	private LayoutControlItem pathLayoutControlItem;

	private TextEdit sharedAccessSignatureURLTextEdit;

	private LayoutControlItem sharedAccessSignatureURLLayoutControlItem;

	private CheckEdit overrideCredentialsCheckEdit;

	private LayoutControlItem overrideCredentialsLayoutControlItem;

	protected override ComboBoxEdit HostComboBoxEdit => accountNameComboBoxEdit;

	protected override TextEdit HostTextEdit => accountNameComboBoxEdit;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.AzureStorage;

	protected override CheckEdit SavePasswordCheckEdit => savePasswordCheckEdit;

	private string ProvidedAccountName => accountNameComboBoxEdit.Text;

	private string ProvidedPathOrContainer
	{
		get
		{
			if (ProvidedAzureStorageAuthentication != AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureDirectory)
			{
				return containerNameTextEdit.Text;
			}
			return pathTextEdit.Text;
		}
	}

	private AzureStorageAuthentication.AzureStorageAuthenticationEnum? ProvidedAzureStorageAuthentication => authenticationImageComboBoxEdit.EditValue as AzureStorageAuthentication.AzureStorageAuthenticationEnum?;

	private string ProvidedPassword => GetPasswordTextEdit()?.Text;

	public bool HasChanges { get; private set; }

	public bool ValuesFromDBFilled { get; private set; }

	public AzureStorageConnectorControl()
	{
		InitializeComponent();
		InitControlsDictionary();
		InitEvents();
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
		DatabaseRow databaseRow = base.DatabaseRow;
		if (databaseRow != null && databaseRow.Id.HasValue)
		{
			savePasswordLayoutControlItem.HideToCustomization();
			base.HideSavingCredentials = true;
		}
		else
		{
			base.HideSavingCredentials = false;
		}
		HideOverrideEdit();
		SetDbmsControlTextWidth(150);
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		azureStorageLayoutControl.Root.Remove(timeoutLayoutControlItem);
		_ = base.SelectedDatabaseType.HasValue;
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
			try
			{
				SetNewDBRowValues();
			}
			catch (SystemException ex)
			{
				GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
				return false;
			}
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

	public override void PageCommited()
	{
		if (HasChanges && overrideCredentialsCheckEdit.Checked)
		{
			parentControl.Save();
		}
	}

	protected override void ClearPanelLoginAndPassword()
	{
		accessKeyTextEdit.Text = string.Empty;
		accountNameComboBoxEdit.Text = string.Empty;
		connectionStringTextEdit.Text = string.Empty;
		containerNameTextEdit.Text = string.Empty;
		pathTextEdit.Text = string.Empty;
		sharedAccessSignatureTextEdit.Text = string.Empty;
		sharedAccessSignatureURLTextEdit.Text = string.Empty;
	}

	protected override string GetPanelDocumentationTitle()
	{
		switch (ProvidedAzureStorageAuthentication)
		{
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.AccessKey:
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.AzureADInteractive:
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.PublicContainer:
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureAccount:
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureDirectory:
			return AzureStorageConnection.GetBlobServiceUriString(ProvidedAccountName);
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureURL:
			return new Uri(sharedAccessSignatureURLTextEdit.Text).GetLeftPart(UriPartial.Path);
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.ConnectionString:
			return CloudStorageAccount.Parse(connectionStringTextEdit.Text).BlobEndpoint.ToString();
		default:
			return null;
		}
	}

	protected override void ReadPanelValues()
	{
		ValuesFromDBFilled = false;
		HasChanges = false;
		HideOverrideEdit();
		AzureStorageAuthentication.AzureStorageAuthenticationEnum azureStorageAuthenticationEnum = AzureStorageAuthentication.FromString(base.DatabaseRow.Param1) ?? AzureStorageAuthentication.AzureStorageAuthenticationEnum.AccessKey;
		authenticationImageComboBoxEdit.EditValue = azureStorageAuthenticationEnum;
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		switch (azureStorageAuthenticationEnum)
		{
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.AccessKey:
			accountNameComboBoxEdit.Text = base.DatabaseRow.Host;
			accessKeyTextEdit.Text = value;
			break;
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.AzureADInteractive:
			accountNameComboBoxEdit.Text = base.DatabaseRow.Host;
			break;
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.ConnectionString:
			connectionStringTextEdit.Text = value;
			break;
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.PublicContainer:
			accountNameComboBoxEdit.Text = base.DatabaseRow.Host;
			containerNameTextEdit.Text = base.DatabaseRow.Param2;
			break;
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureAccount:
			accountNameComboBoxEdit.Text = base.DatabaseRow.Host;
			sharedAccessSignatureTextEdit.Text = value;
			break;
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureDirectory:
			accountNameComboBoxEdit.Text = base.DatabaseRow.Host;
			sharedAccessSignatureTextEdit.Text = value;
			pathTextEdit.Text = base.DatabaseRow.Param2;
			break;
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureURL:
			sharedAccessSignatureURLTextEdit.Text = value;
			break;
		}
		SetSavePasswordText();
		savePasswordCheckEdit.Checked = !string.IsNullOrEmpty(value);
		ValuesFromDBFilled = true;
	}

	protected override void SetAuthenticationDataSource()
	{
		authenticationImageComboBoxEdit.Properties.Items.Clear();
		authenticationImageComboBoxEdit.Properties.Items.AddEnum<AzureStorageAuthentication.AzureStorageAuthenticationEnum>(AzureStorageAuthentication.ToDisplayString);
		if (base.SelectedDatabaseType == SharedDatabaseTypeEnum.DatabaseType.AzureBlobStorage)
		{
			authenticationImageComboBoxEdit.Properties.Items.Remove(authenticationImageComboBoxEdit.Properties.Items.GetItem(AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureDirectory));
		}
		authenticationImageComboBoxEdit.EditValue = AzureStorageAuthentication.AzureStorageAuthenticationEnum.AccessKey;
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow
		{
			Type = base.SelectedDatabaseType,
			Title = documentationTitle,
			Host = ProvidedAccountName,
			Password = ProvidedPassword,
			Id = base.DatabaseRow.Id,
			Name = ProvidedAccountName,
			Param1 = AzureStorageAuthentication.ToString(ProvidedAzureStorageAuthentication),
			Param2 = ProvidedPathOrContainer
		};
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		return true & ValidateEditIfVisible(accountNameComboBoxEdit, addDBErrorProvider, "accountName") & ValidateEditIfVisible(accessKeyTextEdit, addDBErrorProvider, "accessKey") & ValidateEditIfVisible(connectionStringTextEdit, addDBErrorProvider, "connectionString") & ValidateEditIfVisible(containerNameTextEdit, addDBErrorProvider, "containerName") & ValidateEditIfVisible(sharedAccessSignatureTextEdit, addDBErrorProvider, "sharedAccessSignature") & ValidateEditIfVisible(pathTextEdit, addDBErrorProvider, "path") & ValidateEditIfVisible(sharedAccessSignatureURLTextEdit, addDBErrorProvider, "sharedAccessSignatureURL");
	}

	private TextEdit GetPasswordTextEdit()
	{
		return ProvidedAzureStorageAuthentication switch
		{
			AzureStorageAuthentication.AzureStorageAuthenticationEnum.AccessKey => accessKeyTextEdit, 
			AzureStorageAuthentication.AzureStorageAuthenticationEnum.ConnectionString => connectionStringTextEdit, 
			AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureAccount => sharedAccessSignatureTextEdit, 
			AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureDirectory => sharedAccessSignatureTextEdit, 
			AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureURL => sharedAccessSignatureURLTextEdit, 
			_ => null, 
		};
	}

	private string GetSavePasswordText(AzureStorageAuthentication.AzureStorageAuthenticationEnum? authentication)
	{
		switch (authentication)
		{
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.AccessKey:
			return "Save Access Key";
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.ConnectionString:
			return "Save Connection String";
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureAccount:
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureDirectory:
			return "Save Signature";
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureURL:
			return "Save Signature URL";
		default:
			return null;
		}
	}

	private void InitControlsDictionary()
	{
		Dictionary<AzureStorageAuthentication.AzureStorageAuthenticationEnum, IList<LayoutControlItem>> dictionary = new Dictionary<AzureStorageAuthentication.AzureStorageAuthenticationEnum, IList<LayoutControlItem>>();
		dictionary[AzureStorageAuthentication.AzureStorageAuthenticationEnum.AccessKey] = new LayoutControlItem[2] { accountNameLayoutControlItem, accessKeyLayoutControlItem };
		dictionary[AzureStorageAuthentication.AzureStorageAuthenticationEnum.AzureADInteractive] = new LayoutControlItem[1] { accountNameLayoutControlItem };
		dictionary[AzureStorageAuthentication.AzureStorageAuthenticationEnum.ConnectionString] = new LayoutControlItem[1] { connectionStringLayoutControlItem };
		dictionary[AzureStorageAuthentication.AzureStorageAuthenticationEnum.PublicContainer] = new LayoutControlItem[2] { accountNameLayoutControlItem, containerNameLayoutControlItem };
		dictionary[AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureAccount] = new LayoutControlItem[2] { accountNameLayoutControlItem, sharedAccessSignatureLayoutControlItem };
		dictionary[AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureDirectory] = new LayoutControlItem[3] { accountNameLayoutControlItem, pathLayoutControlItem, sharedAccessSignatureLayoutControlItem };
		dictionary[AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureURL] = new LayoutControlItem[1] { sharedAccessSignatureURLLayoutControlItem };
		Dictionary<AzureStorageAuthentication.AzureStorageAuthenticationEnum, IList<LayoutControlItem>> dictionary2 = dictionary;
		Dictionary<AzureStorageAuthentication.AzureStorageAuthenticationEnum, IList<BaseEdit>> dictionary3 = new Dictionary<AzureStorageAuthentication.AzureStorageAuthenticationEnum, IList<BaseEdit>>();
		dictionary3[AzureStorageAuthentication.AzureStorageAuthenticationEnum.AccessKey] = new TextEdit[1] { accessKeyTextEdit };
		dictionary3[AzureStorageAuthentication.AzureStorageAuthenticationEnum.AzureADInteractive] = Array.Empty<BaseEdit>();
		dictionary3[AzureStorageAuthentication.AzureStorageAuthenticationEnum.ConnectionString] = new TextEdit[1] { connectionStringTextEdit };
		dictionary3[AzureStorageAuthentication.AzureStorageAuthenticationEnum.PublicContainer] = Array.Empty<BaseEdit>();
		dictionary3[AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureAccount] = new TextEdit[1] { sharedAccessSignatureTextEdit };
		dictionary3[AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureDirectory] = new TextEdit[1] { sharedAccessSignatureTextEdit };
		dictionary3[AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureURL] = new TextEdit[1] { sharedAccessSignatureURLTextEdit };
		Dictionary<AzureStorageAuthentication.AzureStorageAuthenticationEnum, IList<BaseEdit>> editorsToCleanAfterChange = dictionary3;
		_controlsToShow = dictionary2;
		_editorsToCleanAfterChange = editorsToCleanAfterChange;
		_hideableControls = dictionary2.SelectMany((KeyValuePair<AzureStorageAuthentication.AzureStorageAuthenticationEnum, IList<LayoutControlItem>> x) => x.Value).Distinct().ToList();
	}

	private void InitEvents()
	{
		accountNameComboBoxEdit.EditValueChanged += AccountNameComboBoxEdit_EditValueChanged;
		accountNameComboBoxEdit.Leave += AccountNameComboBoxEdit_Leave;
		authenticationImageComboBoxEdit.EditValueChanged += AuthenticationImageComboBoxEdit_EditValueChanged;
		foreach (LayoutControlItem hideableControl in _hideableControls)
		{
			if (hideableControl.Control is BaseEdit baseEdit)
			{
				baseEdit.EditValueChanged += BaseEdit_EditValueChanged;
			}
		}
		authenticationImageComboBoxEdit.EditValueChanged += BaseEdit_EditValueChanged;
	}

	private void SetSavePasswordText()
	{
		string savePasswordText = GetSavePasswordText(ProvidedAzureStorageAuthentication);
		if (!string.IsNullOrEmpty(savePasswordText))
		{
			savePasswordCheckEdit.Text = savePasswordText;
			savePasswordLayoutControlItem.Visibility = LayoutVisibility.Always;
		}
		else
		{
			savePasswordCheckEdit.Text = string.Empty;
			savePasswordLayoutControlItem.Visibility = LayoutVisibility.Never;
		}
	}

	private void ShowOverrideEdit()
	{
		overrideCredentialsLayoutControlItem.Visibility = LayoutVisibility.Always;
	}

	private void HideOverrideEdit()
	{
		overrideCredentialsCheckEdit.Checked = false;
		overrideCredentialsLayoutControlItem.Visibility = LayoutVisibility.Never;
	}

	private static bool ValidateEditIfVisible(TextEdit textEdit, DXErrorProvider errorProvider, string communicate)
	{
		bool acceptEmptyValue = !textEdit.Visible;
		return ValidateFields.ValidateEdit(textEdit, errorProvider, communicate, acceptEmptyValue);
	}

	private void AccountNameComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void AccountNameComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	private void AuthenticationImageComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		AzureStorageAuthentication.AzureStorageAuthenticationEnum key = (AzureStorageAuthentication.AzureStorageAuthenticationEnum)authenticationImageComboBoxEdit.EditValue;
		IList<LayoutControlItem> list = _controlsToShow[key];
		foreach (LayoutControlItem hideableControl in _hideableControls)
		{
			if (list.Contains(hideableControl))
			{
				hideableControl.Visibility = LayoutVisibility.Always;
			}
			else
			{
				hideableControl.Visibility = LayoutVisibility.Never;
			}
		}
		foreach (BaseEdit item in _editorsToCleanAfterChange[key])
		{
			item.ResetText();
		}
		SetSavePasswordText();
	}

	private void BaseEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (ValuesFromDBFilled)
		{
			HasChanges = true;
			if (!base.IsDBAdded)
			{
				ShowOverrideEdit();
			}
		}
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
		this.azureStorageLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.accessKeyTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.savePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.accountNameComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.authenticationImageComboBoxEdit = new DevExpress.XtraEditors.ImageComboBoxEdit();
		this.sharedAccessSignatureTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.connectionStringTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.containerNameTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.pathTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.sharedAccessSignatureURLTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem34 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.savePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.accessKeyLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.accountNameLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.connectionStringLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.containerNameLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.authenticationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sharedAccessSignatureURLLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.pathLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sharedAccessSignatureLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.overrideCredentialsCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.overrideCredentialsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.azureStorageLayoutControl).BeginInit();
		this.azureStorageLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.accessKeyTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.accountNameComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.authenticationImageComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sharedAccessSignatureTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectionStringTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.containerNameTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pathTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sharedAccessSignatureURLTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem34).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.accessKeyLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.accountNameLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectionStringLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.containerNameLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.authenticationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sharedAccessSignatureURLLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pathLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sharedAccessSignatureLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.overrideCredentialsCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.overrideCredentialsLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.azureStorageLayoutControl.AllowCustomization = false;
		this.azureStorageLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.azureStorageLayoutControl.Controls.Add(this.accessKeyTextEdit);
		this.azureStorageLayoutControl.Controls.Add(this.savePasswordCheckEdit);
		this.azureStorageLayoutControl.Controls.Add(this.accountNameComboBoxEdit);
		this.azureStorageLayoutControl.Controls.Add(this.authenticationImageComboBoxEdit);
		this.azureStorageLayoutControl.Controls.Add(this.sharedAccessSignatureTextEdit);
		this.azureStorageLayoutControl.Controls.Add(this.connectionStringTextEdit);
		this.azureStorageLayoutControl.Controls.Add(this.containerNameTextEdit);
		this.azureStorageLayoutControl.Controls.Add(this.pathTextEdit);
		this.azureStorageLayoutControl.Controls.Add(this.sharedAccessSignatureURLTextEdit);
		this.azureStorageLayoutControl.Controls.Add(this.overrideCredentialsCheckEdit);
		this.azureStorageLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.azureStorageLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.azureStorageLayoutControl.Name = "azureStorageLayoutControl";
		this.azureStorageLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(807, 202, 695, 525);
		this.azureStorageLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.azureStorageLayoutControl.Root = this.layoutControlGroup6;
		this.azureStorageLayoutControl.Size = new System.Drawing.Size(492, 340);
		this.azureStorageLayoutControl.TabIndex = 3;
		this.azureStorageLayoutControl.Text = "layoutControl1";
		this.accessKeyTextEdit.Location = new System.Drawing.Point(155, 44);
		this.accessKeyTextEdit.Name = "accessKeyTextEdit";
		this.accessKeyTextEdit.Properties.UseSystemPasswordChar = true;
		this.accessKeyTextEdit.Size = new System.Drawing.Size(245, 20);
		this.accessKeyTextEdit.StyleController = this.azureStorageLayoutControl;
		this.accessKeyTextEdit.TabIndex = 2;
		this.savePasswordCheckEdit.Location = new System.Drawing.Point(155, 176);
		this.savePasswordCheckEdit.Name = "savePasswordCheckEdit";
		this.savePasswordCheckEdit.Properties.Caption = "Save access key";
		this.savePasswordCheckEdit.Size = new System.Drawing.Size(245, 20);
		this.savePasswordCheckEdit.StyleController = this.azureStorageLayoutControl;
		this.savePasswordCheckEdit.TabIndex = 3;
		this.accountNameComboBoxEdit.Location = new System.Drawing.Point(155, 22);
		this.accountNameComboBoxEdit.Margin = new System.Windows.Forms.Padding(2);
		this.accountNameComboBoxEdit.Name = "accountNameComboBoxEdit";
		this.accountNameComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.accountNameComboBoxEdit.Size = new System.Drawing.Size(245, 20);
		this.accountNameComboBoxEdit.StyleController = this.azureStorageLayoutControl;
		this.accountNameComboBoxEdit.TabIndex = 1;
		this.authenticationImageComboBoxEdit.Location = new System.Drawing.Point(155, 0);
		this.authenticationImageComboBoxEdit.Name = "authenticationImageComboBoxEdit";
		this.authenticationImageComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.authenticationImageComboBoxEdit.Size = new System.Drawing.Size(245, 20);
		this.authenticationImageComboBoxEdit.StyleController = this.azureStorageLayoutControl;
		this.authenticationImageComboBoxEdit.TabIndex = 0;
		this.sharedAccessSignatureTextEdit.Location = new System.Drawing.Point(155, 132);
		this.sharedAccessSignatureTextEdit.Name = "sharedAccessSignatureTextEdit";
		this.sharedAccessSignatureTextEdit.Properties.UseSystemPasswordChar = true;
		this.sharedAccessSignatureTextEdit.Size = new System.Drawing.Size(245, 20);
		this.sharedAccessSignatureTextEdit.StyleController = this.azureStorageLayoutControl;
		this.sharedAccessSignatureTextEdit.TabIndex = 2;
		this.connectionStringTextEdit.Location = new System.Drawing.Point(155, 66);
		this.connectionStringTextEdit.Name = "connectionStringTextEdit";
		this.connectionStringTextEdit.Properties.UseSystemPasswordChar = true;
		this.connectionStringTextEdit.Size = new System.Drawing.Size(245, 20);
		this.connectionStringTextEdit.StyleController = this.azureStorageLayoutControl;
		this.connectionStringTextEdit.TabIndex = 2;
		this.containerNameTextEdit.Location = new System.Drawing.Point(155, 88);
		this.containerNameTextEdit.Name = "containerNameTextEdit";
		this.containerNameTextEdit.Size = new System.Drawing.Size(245, 20);
		this.containerNameTextEdit.StyleController = this.azureStorageLayoutControl;
		this.containerNameTextEdit.TabIndex = 2;
		this.pathTextEdit.Location = new System.Drawing.Point(155, 110);
		this.pathTextEdit.Name = "pathTextEdit";
		this.pathTextEdit.Size = new System.Drawing.Size(245, 20);
		this.pathTextEdit.StyleController = this.azureStorageLayoutControl;
		this.pathTextEdit.TabIndex = 2;
		this.sharedAccessSignatureURLTextEdit.Location = new System.Drawing.Point(155, 154);
		this.sharedAccessSignatureURLTextEdit.Name = "sharedAccessSignatureURLTextEdit";
		this.sharedAccessSignatureURLTextEdit.Properties.UseSystemPasswordChar = true;
		this.sharedAccessSignatureURLTextEdit.Size = new System.Drawing.Size(245, 20);
		this.sharedAccessSignatureURLTextEdit.StyleController = this.azureStorageLayoutControl;
		this.sharedAccessSignatureURLTextEdit.TabIndex = 2;
		this.layoutControlGroup6.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup6.GroupBordersVisible = false;
		this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[12]
		{
			this.emptySpaceItem34, this.emptySpaceItem5, this.accessKeyLayoutControlItem, this.accountNameLayoutControlItem, this.connectionStringLayoutControlItem, this.containerNameLayoutControlItem, this.authenticationLayoutControlItem, this.sharedAccessSignatureURLLayoutControlItem, this.pathLayoutControlItem, this.sharedAccessSignatureLayoutControlItem,
			this.overrideCredentialsLayoutControlItem, this.savePasswordLayoutControlItem
		});
		this.layoutControlGroup6.Name = "Root";
		this.layoutControlGroup6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup6.Size = new System.Drawing.Size(492, 340);
		this.layoutControlGroup6.TextVisible = false;
		this.emptySpaceItem34.AllowHotTrack = false;
		this.emptySpaceItem34.Location = new System.Drawing.Point(0, 220);
		this.emptySpaceItem34.Name = "emptySpaceItem3";
		this.emptySpaceItem34.Size = new System.Drawing.Size(492, 120);
		this.emptySpaceItem34.TextSize = new System.Drawing.Size(0, 0);
		this.savePasswordLayoutControlItem.Control = this.savePasswordCheckEdit;
		this.savePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem18";
		this.savePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 176);
		this.savePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(400, 22);
		this.savePasswordLayoutControlItem.MinSize = new System.Drawing.Size(400, 22);
		this.savePasswordLayoutControlItem.Name = "savePasswordLayoutControlItem";
		this.savePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.savePasswordLayoutControlItem.Size = new System.Drawing.Size(400, 22);
		this.savePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.savePasswordLayoutControlItem.Text = " ";
		this.savePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.savePasswordLayoutControlItem.TextSize = new System.Drawing.Size(150, 13);
		this.savePasswordLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.Location = new System.Drawing.Point(400, 0);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(92, 220);
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.accessKeyLayoutControlItem.Control = this.accessKeyTextEdit;
		this.accessKeyLayoutControlItem.CustomizationFormText = "Password";
		this.accessKeyLayoutControlItem.Location = new System.Drawing.Point(0, 44);
		this.accessKeyLayoutControlItem.MaxSize = new System.Drawing.Size(400, 22);
		this.accessKeyLayoutControlItem.MinSize = new System.Drawing.Size(400, 22);
		this.accessKeyLayoutControlItem.Name = "accessKeyLayoutControlItem";
		this.accessKeyLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.accessKeyLayoutControlItem.Size = new System.Drawing.Size(400, 22);
		this.accessKeyLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.accessKeyLayoutControlItem.Text = "Access Key:";
		this.accessKeyLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.accessKeyLayoutControlItem.TextSize = new System.Drawing.Size(150, 13);
		this.accessKeyLayoutControlItem.TextToControlDistance = 5;
		this.accountNameLayoutControlItem.Control = this.accountNameComboBoxEdit;
		this.accountNameLayoutControlItem.CustomizationFormText = "Host:";
		this.accountNameLayoutControlItem.Location = new System.Drawing.Point(0, 22);
		this.accountNameLayoutControlItem.MaxSize = new System.Drawing.Size(400, 22);
		this.accountNameLayoutControlItem.MinSize = new System.Drawing.Size(400, 22);
		this.accountNameLayoutControlItem.Name = "accountNameLayoutControlItem";
		this.accountNameLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.accountNameLayoutControlItem.Size = new System.Drawing.Size(400, 22);
		this.accountNameLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.accountNameLayoutControlItem.Text = "Account Name:";
		this.accountNameLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.accountNameLayoutControlItem.TextSize = new System.Drawing.Size(150, 13);
		this.accountNameLayoutControlItem.TextToControlDistance = 5;
		this.connectionStringLayoutControlItem.Control = this.connectionStringTextEdit;
		this.connectionStringLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.connectionStringLayoutControlItem.CustomizationFormText = "Password";
		this.connectionStringLayoutControlItem.Location = new System.Drawing.Point(0, 66);
		this.connectionStringLayoutControlItem.MaxSize = new System.Drawing.Size(400, 22);
		this.connectionStringLayoutControlItem.MinSize = new System.Drawing.Size(400, 22);
		this.connectionStringLayoutControlItem.Name = "connectionStringLayoutControlItem";
		this.connectionStringLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.connectionStringLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.connectionStringLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.connectionStringLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.connectionStringLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.connectionStringLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.connectionStringLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.connectionStringLayoutControlItem.Size = new System.Drawing.Size(400, 22);
		this.connectionStringLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.connectionStringLayoutControlItem.Text = "Connection String:";
		this.connectionStringLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.connectionStringLayoutControlItem.TextSize = new System.Drawing.Size(150, 13);
		this.connectionStringLayoutControlItem.TextToControlDistance = 5;
		this.containerNameLayoutControlItem.Control = this.containerNameTextEdit;
		this.containerNameLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.containerNameLayoutControlItem.CustomizationFormText = "Password";
		this.containerNameLayoutControlItem.Location = new System.Drawing.Point(0, 88);
		this.containerNameLayoutControlItem.MaxSize = new System.Drawing.Size(400, 22);
		this.containerNameLayoutControlItem.MinSize = new System.Drawing.Size(400, 22);
		this.containerNameLayoutControlItem.Name = "containerNameLayoutControlItem";
		this.containerNameLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.containerNameLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.containerNameLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.containerNameLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.containerNameLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.containerNameLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.containerNameLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.containerNameLayoutControlItem.Size = new System.Drawing.Size(400, 22);
		this.containerNameLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.containerNameLayoutControlItem.Text = "Container Name:";
		this.containerNameLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.containerNameLayoutControlItem.TextSize = new System.Drawing.Size(150, 13);
		this.containerNameLayoutControlItem.TextToControlDistance = 5;
		this.authenticationLayoutControlItem.Control = this.authenticationImageComboBoxEdit;
		this.authenticationLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.authenticationLayoutControlItem.CustomizationFormText = "Authentication:";
		this.authenticationLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.authenticationLayoutControlItem.MaxSize = new System.Drawing.Size(400, 22);
		this.authenticationLayoutControlItem.MinSize = new System.Drawing.Size(400, 22);
		this.authenticationLayoutControlItem.Name = "authenticationLayoutControlItem";
		this.authenticationLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.authenticationLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.authenticationLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.authenticationLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.authenticationLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.authenticationLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.authenticationLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.authenticationLayoutControlItem.Size = new System.Drawing.Size(400, 22);
		this.authenticationLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.authenticationLayoutControlItem.Text = "Authentication:";
		this.authenticationLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.authenticationLayoutControlItem.TextSize = new System.Drawing.Size(150, 13);
		this.authenticationLayoutControlItem.TextToControlDistance = 5;
		this.sharedAccessSignatureURLLayoutControlItem.Control = this.sharedAccessSignatureURLTextEdit;
		this.sharedAccessSignatureURLLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.sharedAccessSignatureURLLayoutControlItem.CustomizationFormText = "Password";
		this.sharedAccessSignatureURLLayoutControlItem.Location = new System.Drawing.Point(0, 154);
		this.sharedAccessSignatureURLLayoutControlItem.MaxSize = new System.Drawing.Size(400, 22);
		this.sharedAccessSignatureURLLayoutControlItem.MinSize = new System.Drawing.Size(400, 22);
		this.sharedAccessSignatureURLLayoutControlItem.Name = "sharedAccessSignatureURLLayoutControlItem";
		this.sharedAccessSignatureURLLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sharedAccessSignatureURLLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.sharedAccessSignatureURLLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sharedAccessSignatureURLLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.sharedAccessSignatureURLLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sharedAccessSignatureURLLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.sharedAccessSignatureURLLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sharedAccessSignatureURLLayoutControlItem.Size = new System.Drawing.Size(400, 22);
		this.sharedAccessSignatureURLLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sharedAccessSignatureURLLayoutControlItem.Text = "Shared Access Signature URL:";
		this.sharedAccessSignatureURLLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sharedAccessSignatureURLLayoutControlItem.TextSize = new System.Drawing.Size(150, 13);
		this.sharedAccessSignatureURLLayoutControlItem.TextToControlDistance = 5;
		this.pathLayoutControlItem.Control = this.pathTextEdit;
		this.pathLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.pathLayoutControlItem.CustomizationFormText = "Password";
		this.pathLayoutControlItem.Location = new System.Drawing.Point(0, 110);
		this.pathLayoutControlItem.MaxSize = new System.Drawing.Size(400, 22);
		this.pathLayoutControlItem.MinSize = new System.Drawing.Size(400, 22);
		this.pathLayoutControlItem.Name = "pathLayoutControlItem";
		this.pathLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.pathLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.pathLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.pathLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.pathLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.pathLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.pathLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.pathLayoutControlItem.Size = new System.Drawing.Size(400, 22);
		this.pathLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.pathLayoutControlItem.Text = "Path:";
		this.pathLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.pathLayoutControlItem.TextSize = new System.Drawing.Size(150, 13);
		this.pathLayoutControlItem.TextToControlDistance = 5;
		this.sharedAccessSignatureLayoutControlItem.Control = this.sharedAccessSignatureTextEdit;
		this.sharedAccessSignatureLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.sharedAccessSignatureLayoutControlItem.CustomizationFormText = "Password";
		this.sharedAccessSignatureLayoutControlItem.Location = new System.Drawing.Point(0, 132);
		this.sharedAccessSignatureLayoutControlItem.MaxSize = new System.Drawing.Size(400, 22);
		this.sharedAccessSignatureLayoutControlItem.MinSize = new System.Drawing.Size(400, 22);
		this.sharedAccessSignatureLayoutControlItem.Name = "sharedAccessSignatureLayoutControlItem";
		this.sharedAccessSignatureLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sharedAccessSignatureLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.sharedAccessSignatureLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sharedAccessSignatureLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.sharedAccessSignatureLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sharedAccessSignatureLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.sharedAccessSignatureLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sharedAccessSignatureLayoutControlItem.Size = new System.Drawing.Size(400, 22);
		this.sharedAccessSignatureLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sharedAccessSignatureLayoutControlItem.Text = "Shared Access Signature:";
		this.sharedAccessSignatureLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sharedAccessSignatureLayoutControlItem.TextSize = new System.Drawing.Size(150, 13);
		this.sharedAccessSignatureLayoutControlItem.TextToControlDistance = 5;
		this.overrideCredentialsCheckEdit.Location = new System.Drawing.Point(155, 198);
		this.overrideCredentialsCheckEdit.Name = "overrideCredentialsCheckEdit";
		this.overrideCredentialsCheckEdit.Properties.Caption = "Override credentials";
		this.overrideCredentialsCheckEdit.Size = new System.Drawing.Size(245, 20);
		this.overrideCredentialsCheckEdit.StyleController = this.azureStorageLayoutControl;
		this.overrideCredentialsCheckEdit.TabIndex = 3;
		this.overrideCredentialsLayoutControlItem.Control = this.overrideCredentialsCheckEdit;
		this.overrideCredentialsLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.overrideCredentialsLayoutControlItem.CustomizationFormText = "layoutControlItem18";
		this.overrideCredentialsLayoutControlItem.Location = new System.Drawing.Point(0, 198);
		this.overrideCredentialsLayoutControlItem.MaxSize = new System.Drawing.Size(400, 22);
		this.overrideCredentialsLayoutControlItem.MinSize = new System.Drawing.Size(400, 22);
		this.overrideCredentialsLayoutControlItem.Name = "overrideCredentialsLayoutControlItem";
		this.overrideCredentialsLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.overrideCredentialsLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.overrideCredentialsLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.overrideCredentialsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.overrideCredentialsLayoutControlItem.Size = new System.Drawing.Size(400, 22);
		this.overrideCredentialsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.overrideCredentialsLayoutControlItem.Text = " ";
		this.overrideCredentialsLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.overrideCredentialsLayoutControlItem.TextSize = new System.Drawing.Size(150, 13);
		this.overrideCredentialsLayoutControlItem.TextToControlDistance = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.azureStorageLayoutControl);
		base.Name = "AzureStorageConnectorControl";
		base.Size = new System.Drawing.Size(492, 340);
		((System.ComponentModel.ISupportInitialize)this.azureStorageLayoutControl).EndInit();
		this.azureStorageLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.accessKeyTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.accountNameComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.authenticationImageComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sharedAccessSignatureTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectionStringTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.containerNameTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pathTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sharedAccessSignatureURLTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem34).EndInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.accessKeyLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.accountNameLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectionStringLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.containerNameLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.authenticationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sharedAccessSignatureURLLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pathLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sharedAccessSignatureLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.overrideCredentialsCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.overrideCredentialsLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
