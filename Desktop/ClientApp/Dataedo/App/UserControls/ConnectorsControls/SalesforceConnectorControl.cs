using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classes;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class SalesforceConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl salesforceLayoutControl;

	private CheckEdit salesforceSavePasswordCheckEdit;

	private TextEdit passwordTextEdit;

	private ComboBoxEdit salesforceComboBoxEdit;

	private LayoutControlGroup layoutControlGroup4;

	private LayoutControlItem passwordLayoutControlItem;

	private LayoutControlItem savePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem8;

	private EmptySpaceItem emptySpaceItem9;

	private EmptySpaceItem salesforceTimeoutEmptySpaceItem;

	private LayoutControlItem sqlServerHostLayoutControlItem;

	private EmptySpaceItem emptySpaceItem4;

	private TextEdit consumerKeyTextEdit;

	private TextEdit userTextEdit;

	private LayoutControlItem consumerKeyLayoutControlItem;

	private LayoutControlItem userLayoutControlItem;

	private TextEdit consumerSecretTextEdit;

	private LayoutControlItem consumerSecretLayoutControlItem;

	private CheckEdit isSandboxInstanceCheckEdit;

	private LayoutControlItem isSandboxInstanceLayoutControlItem;

	private RadioGroup connectionTypeRadioGroup;

	private LayoutControlItem connectionTypeRadioGroupLayoutControlItem;

	private EmptySpaceItem connectionTypeRadioGroupEmptySpaceItem;

	private EmptySpaceItem emptySpaceItem5;

	private ToolTipController toolTipController;

	private string providedSalesforceHost => splittedHost?.Host ?? salesforceComboBoxEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Salesforce;

	private bool? IsInteractiveConnectionMode
	{
		get
		{
			SalesforceConnectionTypeEnum.SalesforceConnectionType? salesforceConnectionType = connectionTypeRadioGroup.EditValue as SalesforceConnectionTypeEnum.SalesforceConnectionType?;
			if (salesforceConnectionType.HasValue && salesforceConnectionType != SalesforceConnectionTypeEnum.SalesforceConnectionType.None)
			{
				return salesforceConnectionType == SalesforceConnectionTypeEnum.SalesforceConnectionType.Interactive;
			}
			return null;
		}
	}

	protected override bool ShouldShowWaitingPanelWhileConnect
	{
		get
		{
			if (IsInteractiveConnectionMode != true)
			{
				return true;
			}
			return false;
		}
	}

	protected override TextEdit HostTextEdit => salesforceComboBoxEdit;

	protected override ComboBoxEdit HostComboBoxEdit => salesforceComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => salesforceSavePasswordCheckEdit;

	public SalesforceConnectorControl()
	{
		InitializeComponent();
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		if (SkinsManager.IsCurrentSkinDark)
		{
			connectionTypeRadioGroup.BackColor = BackColor;
		}
		SetConnectionTypes();
		SetConnectionTypeFields();
		if (!databaseId.HasValue)
		{
			connectionTypeRadioGroup.EditValue = SalesforceConnectionTypeEnum.SalesforceConnectionType.Interactive;
		}
		if (ConfigHelper.GetSalesforceClientIdConfigValue() == null)
		{
			connectionTypeRadioGroup.EditValue = SalesforceConnectionTypeEnum.SalesforceConnectionType.Values;
			LayoutVisibility layoutVisibility3 = (connectionTypeRadioGroupLayoutControlItem.Visibility = (connectionTypeRadioGroupEmptySpaceItem.Visibility = LayoutVisibility.Never));
		}
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? documentationTitle : base.DatabaseRow?.Name, documentationTitle, providedSalesforceHost, userTextEdit.Text, passwordTextEdit.Text, windows_authentication: false, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, null, null);
		if (IsInteractiveConnectionMode != true)
		{
			SimpleAES simpleAES = new SimpleAES();
			base.DatabaseRow.Param1 = simpleAES.EncryptToString(consumerKeyTextEdit.Text);
			base.DatabaseRow.Param2 = simpleAES.EncryptToString(consumerSecretTextEdit.Text);
		}
		else
		{
			base.DatabaseRow.Param3 = SalesforceConnectionTypeEnum.TypeToString(connectionTypeRadioGroup.EditValue as SalesforceConnectionTypeEnum.SalesforceConnectionType?);
			base.DatabaseRow.Param4 = (isSandboxInstanceCheckEdit.Checked ? "true" : "false");
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		if (IsInteractiveConnectionMode == true)
		{
			return flag;
		}
		flag &= ValidateSalesforceHost();
		flag &= ValidateSalesforceUser();
		flag &= ValidateSalesforcePassword();
		flag &= ValidateSalesforceConsumerKey();
		return flag & ValidateSalesforceConsumerSecret();
	}

	private void SetConnectionTypes()
	{
		Dictionary<SalesforceConnectionTypeEnum.SalesforceConnectionType, string> salesforceConnectionTypes = SalesforceConnectionTypeEnum.GetSalesforceConnectionTypes();
		connectionTypeRadioGroup.Properties.Items.Clear();
		foreach (KeyValuePair<SalesforceConnectionTypeEnum.SalesforceConnectionType, string> item in salesforceConnectionTypes)
		{
			connectionTypeRadioGroup.Properties.Items.Add(new RadioGroupItem(item.Key, item.Value, enabled: true, item.Key));
		}
	}

	private bool ValidateSalesforceHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(salesforceComboBoxEdit, addDBErrorProvider, "Server name", acceptEmptyValue);
	}

	private bool ValidateSalesforceUser(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(userTextEdit, addDBErrorProvider, "User:", acceptEmptyValue);
	}

	private bool ValidateSalesforcePassword(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(passwordTextEdit, addDBErrorProvider, "Password:", acceptEmptyValue);
	}

	private bool ValidateSalesforceConsumerKey(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(consumerKeyTextEdit, addDBErrorProvider, "Consumer Key:", acceptEmptyValue);
	}

	private bool ValidateSalesforceConsumerSecret(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(consumerSecretTextEdit, addDBErrorProvider, "Consumer Secret:", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		if (base.DatabaseRow.Param3 == SalesforceConnectionTypeEnum.TypeToString(SalesforceConnectionTypeEnum.SalesforceConnectionType.Interactive))
		{
			connectionTypeRadioGroup.EditValue = SalesforceConnectionTypeEnum.SalesforceConnectionType.Interactive;
			isSandboxInstanceCheckEdit.Checked = base.DatabaseRow.Param4 == "true";
		}
		else if (!string.IsNullOrWhiteSpace(base.DatabaseRow?.Param1))
		{
			connectionTypeRadioGroup.EditValue = SalesforceConnectionTypeEnum.SalesforceConnectionType.Values;
		}
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		salesforceComboBoxEdit.Text = base.DatabaseRow.Host;
		passwordTextEdit.Text = value;
		userTextEdit.Text = base.DatabaseRow.User;
		SimpleAES simpleAES = new SimpleAES();
		if (!string.IsNullOrWhiteSpace(base.DatabaseRow?.Param1))
		{
			consumerKeyTextEdit.Text = simpleAES.DecryptString(base.DatabaseRow.Param1);
		}
		if (!string.IsNullOrWhiteSpace(base.DatabaseRow?.Param2))
		{
			consumerSecretTextEdit.Text = simpleAES.DecryptString(base.DatabaseRow.Param2);
		}
		salesforceSavePasswordCheckEdit.Checked = !string.IsNullOrEmpty(value);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return providedSalesforceHost ?? "";
	}

	private void SalesforcePasswordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSalesforcePassword(acceptEmptyValue: true);
	}

	private new void HostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		base.HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void HostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	private void ConnectionTypeRadioGroup_EditValueChanged(object sender, EventArgs e)
	{
		SetConnectionTypeFields();
	}

	private void ToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		if (e.SelectedControl != connectionTypeRadioGroup)
		{
			return;
		}
		for (int i = 0; i < connectionTypeRadioGroup.Properties.Items.Count; i++)
		{
			if (connectionTypeRadioGroup.GetItemRectangle(i).Contains(e.ControlMousePosition) && connectionTypeRadioGroup.Properties.Items[i].Tag is SalesforceConnectionTypeEnum.SalesforceConnectionType value)
			{
				e.Info = new ToolTipControlInfo(i, SalesforceConnectionTypeEnum.GetSalesforceConnectionTypeDescriptions(value));
				break;
			}
		}
	}

	protected override void ClearPanelLoginAndPassword()
	{
		userTextEdit.Text = string.Empty;
		passwordTextEdit.Text = string.Empty;
		consumerKeyTextEdit.Text = string.Empty;
		consumerSecretTextEdit.Text = string.Empty;
	}

	private void SetConnectionTypeFields()
	{
		bool? isInteractiveConnectionMode = IsInteractiveConnectionMode;
		LayoutControlItem layoutControlItem = sqlServerHostLayoutControlItem;
		LayoutControlItem layoutControlItem2 = userLayoutControlItem;
		LayoutControlItem layoutControlItem3 = passwordLayoutControlItem;
		LayoutControlItem layoutControlItem4 = savePasswordLayoutControlItem;
		LayoutControlItem layoutControlItem5 = consumerKeyLayoutControlItem;
		LayoutVisibility layoutVisibility2 = (consumerSecretLayoutControlItem.Visibility = ((isInteractiveConnectionMode == true) ? LayoutVisibility.Never : LayoutVisibility.Always));
		LayoutVisibility layoutVisibility4 = (layoutControlItem5.Visibility = layoutVisibility2);
		LayoutVisibility layoutVisibility6 = (layoutControlItem4.Visibility = layoutVisibility4);
		LayoutVisibility layoutVisibility8 = (layoutControlItem3.Visibility = layoutVisibility6);
		LayoutVisibility layoutVisibility11 = (layoutControlItem.Visibility = (layoutControlItem2.Visibility = layoutVisibility8));
		isSandboxInstanceLayoutControlItem.Visibility = ((isInteractiveConnectionMode != true) ? LayoutVisibility.Never : LayoutVisibility.Always);
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
		this.salesforceLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.salesforceSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.passwordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.salesforceComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.consumerKeyTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.userTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.consumerSecretTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.isSandboxInstanceCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.connectionTypeRadioGroup = new DevExpress.XtraEditors.RadioGroup();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.passwordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem9 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.salesforceTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.sqlServerHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.consumerKeyLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.userLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.consumerSecretLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.savePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.isSandboxInstanceLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.connectionTypeRadioGroupLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.connectionTypeRadioGroupEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.salesforceLayoutControl).BeginInit();
		this.salesforceLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.salesforceSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.salesforceComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.consumerKeyTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.userTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.consumerSecretTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.isSandboxInstanceCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectionTypeRadioGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.salesforceTimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.consumerKeyLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.userLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.consumerSecretLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.isSandboxInstanceLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectionTypeRadioGroupLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.connectionTypeRadioGroupEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		base.SuspendLayout();
		this.salesforceLayoutControl.AllowCustomization = false;
		this.salesforceLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.salesforceLayoutControl.Controls.Add(this.salesforceSavePasswordCheckEdit);
		this.salesforceLayoutControl.Controls.Add(this.passwordTextEdit);
		this.salesforceLayoutControl.Controls.Add(this.salesforceComboBoxEdit);
		this.salesforceLayoutControl.Controls.Add(this.consumerKeyTextEdit);
		this.salesforceLayoutControl.Controls.Add(this.userTextEdit);
		this.salesforceLayoutControl.Controls.Add(this.consumerSecretTextEdit);
		this.salesforceLayoutControl.Controls.Add(this.isSandboxInstanceCheckEdit);
		this.salesforceLayoutControl.Controls.Add(this.connectionTypeRadioGroup);
		this.salesforceLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.salesforceLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.salesforceLayoutControl.Name = "salesforceLayoutControl";
		this.salesforceLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2719, 179, 279, 548);
		this.salesforceLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.salesforceLayoutControl.Root = this.layoutControlGroup4;
		this.salesforceLayoutControl.Size = new System.Drawing.Size(499, 322);
		this.salesforceLayoutControl.TabIndex = 3;
		this.salesforceLayoutControl.Text = "layoutControl1";
		this.salesforceSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 148);
		this.salesforceSavePasswordCheckEdit.Name = "salesforceSavePasswordCheckEdit";
		this.salesforceSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.salesforceSavePasswordCheckEdit.Size = new System.Drawing.Size(230, 20);
		this.salesforceSavePasswordCheckEdit.StyleController = this.salesforceLayoutControl;
		this.salesforceSavePasswordCheckEdit.TabIndex = 3;
		this.passwordTextEdit.Location = new System.Drawing.Point(105, 124);
		this.passwordTextEdit.Name = "passwordTextEdit";
		this.passwordTextEdit.Properties.UseSystemPasswordChar = true;
		this.passwordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.passwordTextEdit.StyleController = this.salesforceLayoutControl;
		this.passwordTextEdit.TabIndex = 2;
		this.passwordTextEdit.EditValueChanged += new System.EventHandler(SalesforcePasswordTextEdit_EditValueChanged);
		this.salesforceComboBoxEdit.Location = new System.Drawing.Point(105, 76);
		this.salesforceComboBoxEdit.Margin = new System.Windows.Forms.Padding(2);
		this.salesforceComboBoxEdit.Name = "salesforceComboBoxEdit";
		this.salesforceComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.salesforceComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.salesforceComboBoxEdit.StyleController = this.salesforceLayoutControl;
		this.salesforceComboBoxEdit.TabIndex = 1;
		this.salesforceComboBoxEdit.EditValueChanged += new System.EventHandler(HostComboBoxEdit_EditValueChanged);
		this.salesforceComboBoxEdit.Leave += new System.EventHandler(HostComboBoxEdit_Leave);
		this.consumerKeyTextEdit.Location = new System.Drawing.Point(105, 172);
		this.consumerKeyTextEdit.Name = "consumerKeyTextEdit";
		this.consumerKeyTextEdit.Properties.UseSystemPasswordChar = true;
		this.consumerKeyTextEdit.Size = new System.Drawing.Size(230, 20);
		this.consumerKeyTextEdit.StyleController = this.salesforceLayoutControl;
		this.consumerKeyTextEdit.TabIndex = 2;
		this.userTextEdit.Location = new System.Drawing.Point(105, 100);
		this.userTextEdit.Name = "userTextEdit";
		this.userTextEdit.Size = new System.Drawing.Size(230, 20);
		this.userTextEdit.StyleController = this.salesforceLayoutControl;
		this.userTextEdit.TabIndex = 2;
		this.consumerSecretTextEdit.Location = new System.Drawing.Point(105, 196);
		this.consumerSecretTextEdit.Name = "consumerSecretTextEdit";
		this.consumerSecretTextEdit.Properties.UseSystemPasswordChar = true;
		this.consumerSecretTextEdit.Size = new System.Drawing.Size(230, 20);
		this.consumerSecretTextEdit.StyleController = this.salesforceLayoutControl;
		this.consumerSecretTextEdit.TabIndex = 2;
		this.isSandboxInstanceCheckEdit.Location = new System.Drawing.Point(105, 52);
		this.isSandboxInstanceCheckEdit.Name = "isSandboxInstanceCheckEdit";
		this.isSandboxInstanceCheckEdit.Properties.Caption = "Is sandbox instance";
		this.isSandboxInstanceCheckEdit.Size = new System.Drawing.Size(230, 20);
		this.isSandboxInstanceCheckEdit.StyleController = this.salesforceLayoutControl;
		this.isSandboxInstanceCheckEdit.TabIndex = 3;
		this.connectionTypeRadioGroup.Location = new System.Drawing.Point(105, 0);
		this.connectionTypeRadioGroup.Name = "connectionTypeRadioGroup";
		this.connectionTypeRadioGroup.Properties.AllowFocused = false;
		this.connectionTypeRadioGroup.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.connectionTypeRadioGroup.Properties.ColumnIndent = 0;
		this.connectionTypeRadioGroup.Properties.Columns = 1;
		this.connectionTypeRadioGroup.Size = new System.Drawing.Size(230, 52);
		this.connectionTypeRadioGroup.StyleController = this.salesforceLayoutControl;
		this.connectionTypeRadioGroup.TabIndex = 4;
		this.connectionTypeRadioGroup.ToolTipController = this.toolTipController;
		this.connectionTypeRadioGroup.EditValueChanged += new System.EventHandler(ConnectionTypeRadioGroup_EditValueChanged);
		this.toolTipController.Rounded = true;
		this.toolTipController.ToolTipAnchor = DevExpress.Utils.ToolTipAnchor.Object;
		this.toolTipController.ToolTipLocation = DevExpress.Utils.ToolTipLocation.TopRight;
		this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(ToolTipController_GetActiveObjectInfo);
		this.layoutControlGroup4.CustomizationFormText = "Root";
		this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup4.GroupBordersVisible = false;
		this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[15]
		{
			this.passwordLayoutControlItem, this.emptySpaceItem1, this.emptySpaceItem8, this.emptySpaceItem9, this.salesforceTimeoutEmptySpaceItem, this.sqlServerHostLayoutControlItem, this.emptySpaceItem4, this.consumerKeyLayoutControlItem, this.userLayoutControlItem, this.consumerSecretLayoutControlItem,
			this.savePasswordLayoutControlItem, this.isSandboxInstanceLayoutControlItem, this.connectionTypeRadioGroupLayoutControlItem, this.connectionTypeRadioGroupEmptySpaceItem, this.emptySpaceItem5
		});
		this.layoutControlGroup4.Name = "Root";
		this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup4.Size = new System.Drawing.Size(499, 322);
		this.layoutControlGroup4.TextVisible = false;
		this.passwordLayoutControlItem.Control = this.passwordTextEdit;
		this.passwordLayoutControlItem.CustomizationFormText = "Password";
		this.passwordLayoutControlItem.Location = new System.Drawing.Point(0, 124);
		this.passwordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.passwordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.passwordLayoutControlItem.Name = "passwordLayoutControlItem";
		this.passwordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.passwordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.passwordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.passwordLayoutControlItem.Text = "Password:";
		this.passwordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.passwordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.passwordLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem6";
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 257);
		this.emptySpaceItem1.Name = "emptySpaceItem6";
		this.emptySpaceItem1.Size = new System.Drawing.Size(499, 65);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem8.AllowHotTrack = false;
		this.emptySpaceItem8.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem8.Location = new System.Drawing.Point(335, 100);
		this.emptySpaceItem8.Name = "emptySpaceItem22";
		this.emptySpaceItem8.Size = new System.Drawing.Size(164, 43);
		this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem9.AllowHotTrack = false;
		this.emptySpaceItem9.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem9.Location = new System.Drawing.Point(335, 183);
		this.emptySpaceItem9.Name = "emptySpaceItem28";
		this.emptySpaceItem9.Size = new System.Drawing.Size(164, 37);
		this.emptySpaceItem9.Text = "emptySpaceItem22";
		this.emptySpaceItem9.TextSize = new System.Drawing.Size(0, 0);
		this.salesforceTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.salesforceTimeoutEmptySpaceItem.CustomizationFormText = "emptySpaceItem12";
		this.salesforceTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 220);
		this.salesforceTimeoutEmptySpaceItem.Name = "sqlServerTimeoutEmptySpaceItem";
		this.salesforceTimeoutEmptySpaceItem.Size = new System.Drawing.Size(499, 37);
		this.salesforceTimeoutEmptySpaceItem.Text = "emptySpaceItem12";
		this.salesforceTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.sqlServerHostLayoutControlItem.Control = this.salesforceComboBoxEdit;
		this.sqlServerHostLayoutControlItem.CustomizationFormText = "Server name:";
		this.sqlServerHostLayoutControlItem.Location = new System.Drawing.Point(0, 76);
		this.sqlServerHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerHostLayoutControlItem.Name = "sqlServerHostLayoutControlItem";
		this.sqlServerHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sqlServerHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerHostLayoutControlItem.Text = "Server URL:";
		this.sqlServerHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerHostLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(335, 143);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(164, 40);
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.consumerKeyLayoutControlItem.Control = this.consumerKeyTextEdit;
		this.consumerKeyLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.consumerKeyLayoutControlItem.CustomizationFormText = "Consumer Key";
		this.consumerKeyLayoutControlItem.Location = new System.Drawing.Point(0, 172);
		this.consumerKeyLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.consumerKeyLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.consumerKeyLayoutControlItem.Name = "consumerKeyLayoutControlItem";
		this.consumerKeyLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.consumerKeyLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.consumerKeyLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.consumerKeyLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.consumerKeyLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.consumerKeyLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.consumerKeyLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.consumerKeyLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.consumerKeyLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.consumerKeyLayoutControlItem.Text = "Consumer Key:";
		this.consumerKeyLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.consumerKeyLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.consumerKeyLayoutControlItem.TextToControlDistance = 5;
		this.userLayoutControlItem.Control = this.userTextEdit;
		this.userLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.userLayoutControlItem.CustomizationFormText = "User";
		this.userLayoutControlItem.Location = new System.Drawing.Point(0, 100);
		this.userLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.userLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.userLayoutControlItem.Name = "userLayoutControlItem";
		this.userLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.userLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.userLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.userLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.userLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.userLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.userLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.userLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.userLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.userLayoutControlItem.Text = "User:";
		this.userLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.userLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.userLayoutControlItem.TextToControlDistance = 5;
		this.consumerSecretLayoutControlItem.Control = this.consumerSecretTextEdit;
		this.consumerSecretLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.consumerSecretLayoutControlItem.CustomizationFormText = "Consumer Secret:";
		this.consumerSecretLayoutControlItem.Location = new System.Drawing.Point(0, 196);
		this.consumerSecretLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.consumerSecretLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.consumerSecretLayoutControlItem.Name = "consumerSecretLayoutControlItem";
		this.consumerSecretLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.consumerSecretLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.consumerSecretLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.consumerSecretLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.consumerSecretLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.consumerSecretLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.consumerSecretLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.consumerSecretLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.consumerSecretLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.consumerSecretLayoutControlItem.Text = "Consumer Secret:";
		this.consumerSecretLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.consumerSecretLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.consumerSecretLayoutControlItem.TextToControlDistance = 5;
		this.savePasswordLayoutControlItem.Control = this.salesforceSavePasswordCheckEdit;
		this.savePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem2";
		this.savePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 148);
		this.savePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.savePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.savePasswordLayoutControlItem.Name = "sqlServerSavePasswordLayoutControlItem";
		this.savePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.savePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.savePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.savePasswordLayoutControlItem.Text = " ";
		this.savePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.savePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.savePasswordLayoutControlItem.TextToControlDistance = 5;
		this.isSandboxInstanceLayoutControlItem.Control = this.isSandboxInstanceCheckEdit;
		this.isSandboxInstanceLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.isSandboxInstanceLayoutControlItem.CustomizationFormText = "layoutControlItem2";
		this.isSandboxInstanceLayoutControlItem.Location = new System.Drawing.Point(0, 52);
		this.isSandboxInstanceLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.isSandboxInstanceLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.isSandboxInstanceLayoutControlItem.Name = "isSandboxInstanceLayoutControlItem";
		this.isSandboxInstanceLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.isSandboxInstanceLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.isSandboxInstanceLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.isSandboxInstanceLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.isSandboxInstanceLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.isSandboxInstanceLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.isSandboxInstanceLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.isSandboxInstanceLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.isSandboxInstanceLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.isSandboxInstanceLayoutControlItem.Text = " ";
		this.isSandboxInstanceLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.isSandboxInstanceLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.isSandboxInstanceLayoutControlItem.TextToControlDistance = 5;
		this.connectionTypeRadioGroupLayoutControlItem.Control = this.connectionTypeRadioGroup;
		this.connectionTypeRadioGroupLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.connectionTypeRadioGroupLayoutControlItem.MaxSize = new System.Drawing.Size(0, 52);
		this.connectionTypeRadioGroupLayoutControlItem.MinSize = new System.Drawing.Size(155, 52);
		this.connectionTypeRadioGroupLayoutControlItem.Name = "connectionTypeRadioGroupLayoutControlItem";
		this.connectionTypeRadioGroupLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.connectionTypeRadioGroupLayoutControlItem.Size = new System.Drawing.Size(335, 52);
		this.connectionTypeRadioGroupLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.connectionTypeRadioGroupLayoutControlItem.Text = "Connection type:";
		this.connectionTypeRadioGroupLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.connectionTypeRadioGroupLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.connectionTypeRadioGroupLayoutControlItem.TextToControlDistance = 5;
		this.connectionTypeRadioGroupEmptySpaceItem.AllowHotTrack = false;
		this.connectionTypeRadioGroupEmptySpaceItem.Location = new System.Drawing.Point(335, 0);
		this.connectionTypeRadioGroupEmptySpaceItem.Name = "emptySpaceItem2";
		this.connectionTypeRadioGroupEmptySpaceItem.Size = new System.Drawing.Size(164, 52);
		this.connectionTypeRadioGroupEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.Location = new System.Drawing.Point(335, 52);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(164, 48);
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.salesforceLayoutControl);
		base.Name = "SalesforceConnectorControl";
		base.Size = new System.Drawing.Size(499, 322);
		((System.ComponentModel.ISupportInitialize)this.salesforceLayoutControl).EndInit();
		this.salesforceLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.salesforceSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.salesforceComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.consumerKeyTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.userTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.consumerSecretTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.isSandboxInstanceCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectionTypeRadioGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.salesforceTimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.consumerKeyLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.userLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.consumerSecretLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.isSandboxInstanceLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectionTypeRadioGroupLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.connectionTypeRadioGroupEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		base.ResumeLayout(false);
	}
}
