using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class OracleConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl oracleLayoutControl;

	private LookUpEdit instanceIdentifierLookUpEdit;

	private LabelControl schemasCountLabelControl;

	private CheckEdit oracleUseDifferentSchemaCheckEdit;

	private LabelControl oracleDefaultPortLabelControl;

	private ButtonEdit oracleSchemaButtonEdit;

	private TextEdit oraclePasswordTextEdit;

	private TextEdit oracleLoginTextEdit;

	private TextEdit oraclePortTextEdit;

	private TextEdit oracleServiceNameTextEdit;

	private CheckEdit oracleSavePasswordCheckEdit;

	private LookUpEdit oracleConnectionTypeLookUpEdit;

	private ComboBoxEdit oracleHostComboBoxEdit;

	private LayoutControlGroup layoutControlGroup5;

	private LayoutControlItem oracleServiceNameLayoutControlItem;

	private LayoutControlItem oraclePortLayoutControlItem;

	private LayoutControlItem oracleLoginLayoutControlItem;

	private LayoutControlItem oraclePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem10;

	private EmptySpaceItem emptySpaceItem11;

	private EmptySpaceItem emptySpaceItem12;

	private EmptySpaceItem emptySpaceItem24;

	private LayoutControlItem oracleSavePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem30;

	private EmptySpaceItem emptySpaceItem31;

	private EmptySpaceItem emptySpaceItem32;

	private LayoutControlItem oracleSchemaLayoutControlItem;

	private EmptySpaceItem emptySpaceItem33;

	private LayoutControlItem layoutControlItem38;

	private EmptySpaceItem oracleTimeoutEmptySpaceItem;

	private LayoutControlItem oracleUseDifferentSchemaLayoutControlItem;

	private EmptySpaceItem emptySpaceItem47;

	private LayoutControlItem oracleConnectionTypeLayoutControlItem;

	private LayoutControlItem oracleHostLayoutControlItem;

	private EmptySpaceItem emptySpaceItem6;

	private EmptySpaceItem emptySpaceItem40;

	private LayoutControlItem schemasCountLayoutControlItem;

	private LayoutControlItem layoutControlItem22;

	private EmptySpaceItem emptySpaceItem63;

	private string providedOracleHost => splittedHost?.Host ?? oracleHostComboBoxEdit.Text;

	private string providedOraclePort => oraclePortTextEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Oracle;

	public override ConnectionTypeEnum.ConnectionType ConnectionType => ConnectionTypeEnum.StringToType(oracleConnectionTypeLookUpEdit.EditValue?.ToString());

	protected override TextEdit HostTextEdit => oracleHostComboBoxEdit;

	protected override TextEdit PortTextEdit => oraclePortTextEdit;

	protected override ComboBoxEdit HostComboBoxEdit => oracleHostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => oracleSavePasswordCheckEdit;

	public OracleConnectorControl()
	{
		InitializeComponent();
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
		if (isExporting && base.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Oracle)
		{
			LayoutControlItem layoutControlItem = oracleUseDifferentSchemaLayoutControlItem;
			EmptySpaceItem emptySpaceItem = emptySpaceItem31;
			LayoutControlItem layoutControlItem2 = oracleSchemaLayoutControlItem;
			EmptySpaceItem emptySpaceItem2 = emptySpaceItem63;
			LayoutControlItem layoutControlItem3 = schemasCountLayoutControlItem;
			LayoutVisibility layoutVisibility2 = (emptySpaceItem32.Visibility = LayoutVisibility.Never);
			LayoutVisibility layoutVisibility4 = (layoutControlItem3.Visibility = layoutVisibility2);
			LayoutVisibility layoutVisibility6 = (emptySpaceItem2.Visibility = layoutVisibility4);
			LayoutVisibility layoutVisibility8 = (layoutControlItem2.Visibility = layoutVisibility6);
			LayoutVisibility layoutVisibility11 = (layoutControlItem.Visibility = (emptySpaceItem.Visibility = layoutVisibility8));
		}
		if (base.SelectedDatabaseType.HasValue)
		{
			SetInstanceIdentifiers();
		}
	}

	private void SetInstanceIdentifiers()
	{
		Dictionary<InstanceIdentifierEnum.InstanceIdentifier, string> instanceIdentifiers = InstanceIdentifierEnum.GetInstanceIdentifiers();
		ConnectorControlBase.SetDropDownSize(instanceIdentifierLookUpEdit, instanceIdentifiers.Count);
		instanceIdentifierLookUpEdit.Properties.DataSource = instanceIdentifiers;
		instanceIdentifierLookUpEdit.EditValue = base.DatabaseRow.InstanceIdentifier;
	}

	protected override void SetAuthenticationDataSource()
	{
		oracleConnectionTypeLookUpEdit.Properties.DataSource = OracleConnectionTypes.GetOracleConnectionTypesDataSource();
		oracleConnectionTypeLookUpEdit.Properties.DropDownRows = (oracleConnectionTypeLookUpEdit.Properties.DataSource as DataTable).Rows.Count;
		oracleConnectionTypeLookUpEdit.EditValue = ConnectionTypeEnum.TypeToString(ConnectionTypeEnum.ConnectionType.Direct);
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
			if (!testForGettingDatabasesList && base.DatabaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.Oracle)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
				SetNewDBRowValues(forGettingDatabasesList: true);
				List<string> databases = base.DatabaseRow.GetDatabases(base.SplashScreenManager);
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
				if (databases == null)
				{
					return false;
				}
				Func<IEnumerable<string>, bool> validateValueFunction = ConnectorControlBase.GetValidateValueFunction("schemas");
				if (validateValueFunction != null && !validateValueFunction(schemas))
				{
					return false;
				}
				if (!ConnectorControlBase.CheckSchemaExistence(oracleSchemaButtonEdit, databases))
				{
					CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
					GeneralMessageBoxesHandling.Show("Specified schema not found. Check the schema name and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
					return false;
				}
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

	public override void SetSwitchDatabaseAvailability(bool isAvailable)
	{
		oracleUseDifferentSchemaCheckEdit.Enabled = isAvailable;
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		string name = DatabaseRow.PrepareSchemasList(schemas.ToList());
		InstanceIdentifierEnum.InstanceIdentifier instanceIdentifier = base.DatabaseRow.InstanceIdentifier;
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, name, documentationTitle, ConnectionTypeEnum.StringToType(oracleConnectionTypeLookUpEdit.EditValue as string), providedOracleHost, oracleLoginTextEdit.Text, oraclePasswordTextEdit.Text, providedOraclePort, (instanceIdentifier == InstanceIdentifierEnum.InstanceIdentifier.ServiceName) ? oracleServiceNameTextEdit.Text : null, (instanceIdentifier == InstanceIdentifierEnum.InstanceIdentifier.SID) ? oracleServiceNameTextEdit.Text : null, base.DatabaseRow.InstanceIdentifier, base.DatabaseRow.Id, oracleUseDifferentSchemaCheckEdit.Checked, oracleUseDifferentSchemaCheckEdit.Checked && schemas.Count() > 1, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, schemas.ToList());
		base.DatabaseRow.InstanceIdentifier = instanceIdentifier;
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		oracleLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			oracleLayoutControl.Root.AddItem(timeoutLayoutControlItem, oracleTimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateOracleHost();
		flag &= ValidateOracleServiceName();
		flag &= ValidateOraclePort();
		flag &= ValidateOracleLogin();
		flag &= ValidateOraclePassword();
		if (!testForGettingDatabasesList)
		{
			flag &= ValidateOracleSchema();
		}
		return flag;
	}

	private bool ValidateOracleHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(oracleHostComboBoxEdit, addDBErrorProvider, "host", acceptEmptyValue);
	}

	private bool ValidateOracleServiceName(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(oracleServiceNameTextEdit, addDBErrorProvider, "service name", acceptEmptyValue);
	}

	private bool ValidateOraclePort(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(oraclePortTextEdit, addDBErrorProvider, "port", acceptEmptyValue);
	}

	private bool ValidateOracleLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(oracleLoginTextEdit, addDBErrorProvider, "user", acceptEmptyValue);
	}

	private bool ValidateOraclePassword(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(oraclePasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateOracleSchema(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(oracleSchemaButtonEdit, addDBErrorProvider, "schema", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		oracleHostComboBoxEdit.Text = base.DatabaseRow.Host;
		oracleUseDifferentSchemaCheckEdit.Checked = base.DatabaseRow.UseDifferentSchema.GetValueOrDefault();
		instanceIdentifierLookUpEdit.EditValue = base.DatabaseRow.InstanceIdentifier;
		oracleServiceNameTextEdit.Text = ((base.DatabaseRow.InstanceIdentifier == InstanceIdentifierEnum.InstanceIdentifier.SID) ? base.DatabaseRow.OracleSid : base.DatabaseRow.ServiceName);
		oraclePortTextEdit.Text = PrepareValue.ToString(base.DatabaseRow.Port) ?? DatabaseTypeEnum.GetDefaultPort(SharedDatabaseTypeEnum.DatabaseType.Oracle);
		oracleLoginTextEdit.Text = base.DatabaseRow.User;
		oraclePasswordTextEdit.Text = base.DatabaseRow.Password;
		oracleSavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
		schemas = new List<string>(base.DatabaseRow.Schemas ?? new List<string>());
		SetSchemasButtonEdit(oracleSchemaButtonEdit);
		SetElementsLabelControl(schemasCountLabelControl);
		oracleConnectionTypeLookUpEdit.EditValue = ConnectionTypeEnum.TypeToString(base.DatabaseRow.ConnectionType);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return $"{oracleSchemaButtonEdit.EditValue}@{providedOracleHost}";
	}

	private void serviceNameTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateOracleServiceName(acceptEmptyValue: true);
	}

	private void oraclePortTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateOraclePort(acceptEmptyValue: true);
		PortTextEdit_Leave(sender, e);
	}

	private void oracleLoginTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (!oracleUseDifferentSchemaCheckEdit.Checked)
		{
			oracleSchemaButtonEdit.EditValue = (sender as TextEdit).EditValue;
			schemas = new List<string> { oracleSchemaButtonEdit.Text.ToUpper() };
		}
		ValidateOracleLogin(acceptEmptyValue: true);
		ValidateOracleSchema(acceptEmptyValue: true);
	}

	private void useDifferentSchemaCheckEdit_CheckedChanged(object sender, EventArgs e)
	{
		bool @checked = (sender as CheckEdit).Checked;
		if (!@checked)
		{
			oracleSchemaButtonEdit.EditValue = oracleLoginTextEdit.EditValue;
		}
		schemas = new List<string> { oracleSchemaButtonEdit.Text.ToUpper() };
		oracleSchemaButtonEdit.Properties.ReadOnly = !@checked;
		if (oracleSchemaButtonEdit.Properties.Buttons.Any())
		{
			oracleSchemaButtonEdit.Properties.Buttons[0].Enabled = @checked;
		}
		ValidateOracleSchema(acceptEmptyValue: true);
	}

	private void schemaDatabaseButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateOracleSchema(acceptEmptyValue: true);
		schemas = ConnectorControlBase.GetPrintedSchemas(sender as ButtonEdit);
	}

	private void oracleDefaultPortLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			oraclePortTextEdit.Text = DatabaseTypeEnum.GetDefaultPort(SharedDatabaseTypeEnum.DatabaseType.Oracle);
		}
	}

	private void instanceIdentifierLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		base.DatabaseRow.InstanceIdentifier = (InstanceIdentifierEnum.InstanceIdentifier)instanceIdentifierLookUpEdit.EditValue;
	}

	private void oraclePasswordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateOraclePassword();
	}

	private void oracleSchemaButtonEdit_Leave(object sender, EventArgs e)
	{
		SetElementsLabelControl(schemasCountLabelControl);
	}

	private void oracleSchemaButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		HandleSchemaButtonEdit(sender as ButtonEdit, testConnection: true, "Schemas", schemasCountLabelControl);
	}

	private void oracleHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void oracleHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	private void oraclePortTextEdit_Leave(object sender, EventArgs e)
	{
		PortTextEdit_Leave(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		oracleLoginTextEdit.Text = string.Empty;
		oraclePasswordTextEdit.Text = string.Empty;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.oracleLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.instanceIdentifierLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.schemasCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.oracleUseDifferentSchemaCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.oracleDefaultPortLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.oracleSchemaButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.oraclePasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.oracleLoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.oraclePortTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.oracleServiceNameTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.oracleSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.oracleConnectionTypeLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.oracleHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.oracleServiceNameLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.oraclePortLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.oracleLoginLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.oraclePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem10 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem11 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem12 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem24 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.oracleSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem30 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem31 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem32 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.oracleSchemaLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem33 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem38 = new DevExpress.XtraLayout.LayoutControlItem();
		this.oracleTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.oracleUseDifferentSchemaLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem47 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.oracleConnectionTypeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.oracleHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem40 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.schemasCountLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem22 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem63 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.oracleLayoutControl).BeginInit();
		this.oracleLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.instanceIdentifierLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleUseDifferentSchemaCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleSchemaButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oraclePasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleLoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oraclePortTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleServiceNameTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleConnectionTypeLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleServiceNameLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oraclePortLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleLoginLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oraclePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem30).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem31).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem32).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleSchemaLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem33).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem38).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleTimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleUseDifferentSchemaLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem47).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleConnectionTypeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.oracleHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem40).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.schemasCountLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem22).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem63).BeginInit();
		base.SuspendLayout();
		this.oracleLayoutControl.AllowCustomization = false;
		this.oracleLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.oracleLayoutControl.Controls.Add(this.instanceIdentifierLookUpEdit);
		this.oracleLayoutControl.Controls.Add(this.schemasCountLabelControl);
		this.oracleLayoutControl.Controls.Add(this.oracleUseDifferentSchemaCheckEdit);
		this.oracleLayoutControl.Controls.Add(this.oracleDefaultPortLabelControl);
		this.oracleLayoutControl.Controls.Add(this.oracleSchemaButtonEdit);
		this.oracleLayoutControl.Controls.Add(this.oraclePasswordTextEdit);
		this.oracleLayoutControl.Controls.Add(this.oracleLoginTextEdit);
		this.oracleLayoutControl.Controls.Add(this.oraclePortTextEdit);
		this.oracleLayoutControl.Controls.Add(this.oracleServiceNameTextEdit);
		this.oracleLayoutControl.Controls.Add(this.oracleSavePasswordCheckEdit);
		this.oracleLayoutControl.Controls.Add(this.oracleConnectionTypeLookUpEdit);
		this.oracleLayoutControl.Controls.Add(this.oracleHostComboBoxEdit);
		this.oracleLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.oracleLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.oracleLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.oracleLayoutControl.Name = "oracleLayoutControl";
		this.oracleLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3130, 661, 517, 550);
		this.oracleLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.oracleLayoutControl.OptionsFocus.AllowFocusReadonlyEditors = false;
		this.oracleLayoutControl.Root = this.layoutControlGroup5;
		this.oracleLayoutControl.Size = new System.Drawing.Size(600, 366);
		this.oracleLayoutControl.TabIndex = 3;
		this.oracleLayoutControl.Text = "layoutControl1";
		this.instanceIdentifierLookUpEdit.Location = new System.Drawing.Point(105, 48);
		this.instanceIdentifierLookUpEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.instanceIdentifierLookUpEdit.Name = "instanceIdentifierLookUpEdit";
		this.instanceIdentifierLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.instanceIdentifierLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name1")
		});
		this.instanceIdentifierLookUpEdit.Properties.DisplayMember = "Value";
		this.instanceIdentifierLookUpEdit.Properties.NullText = "";
		this.instanceIdentifierLookUpEdit.Properties.ShowFooter = false;
		this.instanceIdentifierLookUpEdit.Properties.ShowHeader = false;
		this.instanceIdentifierLookUpEdit.Properties.ShowLines = false;
		this.instanceIdentifierLookUpEdit.Properties.ValueMember = "Key";
		this.instanceIdentifierLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.instanceIdentifierLookUpEdit.StyleController = this.oracleLayoutControl;
		this.instanceIdentifierLookUpEdit.TabIndex = 28;
		this.instanceIdentifierLookUpEdit.EditValueChanged += new System.EventHandler(instanceIdentifierLookUpEdit_EditValueChanged);
		this.schemasCountLabelControl.Location = new System.Drawing.Point(346, 218);
		this.schemasCountLabelControl.Name = "schemasCountLabelControl";
		this.schemasCountLabelControl.Size = new System.Drawing.Size(144, 20);
		this.schemasCountLabelControl.StyleController = this.oracleLayoutControl;
		this.schemasCountLabelControl.TabIndex = 27;
		this.oracleUseDifferentSchemaCheckEdit.Location = new System.Drawing.Point(105, 192);
		this.oracleUseDifferentSchemaCheckEdit.MaximumSize = new System.Drawing.Size(130, 0);
		this.oracleUseDifferentSchemaCheckEdit.Name = "oracleUseDifferentSchemaCheckEdit";
		this.oracleUseDifferentSchemaCheckEdit.Properties.Caption = "Use different schema";
		this.oracleUseDifferentSchemaCheckEdit.Size = new System.Drawing.Size(130, 20);
		this.oracleUseDifferentSchemaCheckEdit.StyleController = this.oracleLayoutControl;
		this.oracleUseDifferentSchemaCheckEdit.TabIndex = 5;
		this.oracleUseDifferentSchemaCheckEdit.CheckedChanged += new System.EventHandler(useDifferentSchemaCheckEdit_CheckedChanged);
		this.oracleDefaultPortLabelControl.AllowHtmlString = true;
		this.oracleDefaultPortLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.oracleDefaultPortLabelControl.Location = new System.Drawing.Point(346, 98);
		this.oracleDefaultPortLabelControl.Name = "oracleDefaultPortLabelControl";
		this.oracleDefaultPortLabelControl.Size = new System.Drawing.Size(36, 20);
		this.oracleDefaultPortLabelControl.StyleController = this.oracleLayoutControl;
		this.oracleDefaultPortLabelControl.TabIndex = 26;
		this.oracleDefaultPortLabelControl.Text = "<href>default</href>";
		this.oracleDefaultPortLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(oracleDefaultPortLabelControl_HyperlinkClick);
		this.oracleSchemaButtonEdit.Location = new System.Drawing.Point(105, 216);
		this.oracleSchemaButtonEdit.Name = "oracleSchemaButtonEdit";
		this.oracleSchemaButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, false, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.oracleSchemaButtonEdit.Properties.ReadOnly = true;
		this.oracleSchemaButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.oracleSchemaButtonEdit.StyleController = this.oracleLayoutControl;
		this.oracleSchemaButtonEdit.TabIndex = 6;
		this.oracleSchemaButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(oracleSchemaButtonEdit_ButtonClick);
		this.oracleSchemaButtonEdit.EditValueChanged += new System.EventHandler(schemaDatabaseButtonEdit_EditValueChanged);
		this.oracleSchemaButtonEdit.Leave += new System.EventHandler(oracleSchemaButtonEdit_Leave);
		this.oraclePasswordTextEdit.Location = new System.Drawing.Point(105, 144);
		this.oraclePasswordTextEdit.Name = "oraclePasswordTextEdit";
		this.oraclePasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.oraclePasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.oraclePasswordTextEdit.StyleController = this.oracleLayoutControl;
		this.oraclePasswordTextEdit.TabIndex = 3;
		this.oraclePasswordTextEdit.EditValueChanged += new System.EventHandler(oraclePasswordTextEdit_EditValueChanged);
		this.oracleLoginTextEdit.Location = new System.Drawing.Point(105, 120);
		this.oracleLoginTextEdit.Name = "oracleLoginTextEdit";
		this.oracleLoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.oracleLoginTextEdit.StyleController = this.oracleLayoutControl;
		this.oracleLoginTextEdit.TabIndex = 2;
		this.oracleLoginTextEdit.EditValueChanged += new System.EventHandler(oracleLoginTextEdit_EditValueChanged);
		this.oraclePortTextEdit.EditValue = "1521";
		this.oraclePortTextEdit.Location = new System.Drawing.Point(105, 96);
		this.oraclePortTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.oraclePortTextEdit.Name = "oraclePortTextEdit";
		this.oraclePortTextEdit.Properties.Mask.EditMask = "\\d+";
		this.oraclePortTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.oraclePortTextEdit.Properties.Mask.ShowPlaceHolders = false;
		this.oraclePortTextEdit.Properties.MaxLength = 5;
		this.oraclePortTextEdit.Size = new System.Drawing.Size(230, 20);
		this.oraclePortTextEdit.StyleController = this.oracleLayoutControl;
		this.oraclePortTextEdit.TabIndex = 1;
		this.oraclePortTextEdit.EditValueChanged += new System.EventHandler(oraclePortTextEdit_EditValueChanged);
		this.oraclePortTextEdit.Leave += new System.EventHandler(oraclePortTextEdit_Leave);
		this.oracleServiceNameTextEdit.EditValue = "";
		this.oracleServiceNameTextEdit.Location = new System.Drawing.Point(105, 72);
		this.oracleServiceNameTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.oracleServiceNameTextEdit.Name = "oracleServiceNameTextEdit";
		this.oracleServiceNameTextEdit.Size = new System.Drawing.Size(230, 20);
		this.oracleServiceNameTextEdit.StyleController = this.oracleLayoutControl;
		this.oracleServiceNameTextEdit.TabIndex = 0;
		this.oracleServiceNameTextEdit.EditValueChanged += new System.EventHandler(serviceNameTextEdit_EditValueChanged);
		this.oracleSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 168);
		this.oracleSavePasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.oracleSavePasswordCheckEdit.Name = "oracleSavePasswordCheckEdit";
		this.oracleSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.oracleSavePasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.oracleSavePasswordCheckEdit.StyleController = this.oracleLayoutControl;
		this.oracleSavePasswordCheckEdit.TabIndex = 4;
		this.oracleConnectionTypeLookUpEdit.Location = new System.Drawing.Point(105, 0);
		this.oracleConnectionTypeLookUpEdit.Name = "oracleConnectionTypeLookUpEdit";
		this.oracleConnectionTypeLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.oracleConnectionTypeLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("name", "Name")
		});
		this.oracleConnectionTypeLookUpEdit.Properties.DisplayMember = "name";
		this.oracleConnectionTypeLookUpEdit.Properties.NullText = "";
		this.oracleConnectionTypeLookUpEdit.Properties.ShowFooter = false;
		this.oracleConnectionTypeLookUpEdit.Properties.ShowHeader = false;
		this.oracleConnectionTypeLookUpEdit.Properties.ShowLines = false;
		this.oracleConnectionTypeLookUpEdit.Properties.ValueMember = "type";
		this.oracleConnectionTypeLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.oracleConnectionTypeLookUpEdit.StyleController = this.oracleLayoutControl;
		this.oracleConnectionTypeLookUpEdit.TabIndex = 0;
		this.oracleHostComboBoxEdit.Location = new System.Drawing.Point(105, 24);
		this.oracleHostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.oracleHostComboBoxEdit.Name = "oracleHostComboBoxEdit";
		this.oracleHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.oracleHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.oracleHostComboBoxEdit.StyleController = this.oracleLayoutControl;
		this.oracleHostComboBoxEdit.TabIndex = 1;
		this.oracleHostComboBoxEdit.EditValueChanged += new System.EventHandler(oracleHostComboBoxEdit_EditValueChanged);
		this.oracleHostComboBoxEdit.Leave += new System.EventHandler(oracleHostComboBoxEdit_Leave);
		this.layoutControlGroup5.CustomizationFormText = "Root";
		this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup5.GroupBordersVisible = false;
		this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[25]
		{
			this.oracleServiceNameLayoutControlItem, this.oraclePortLayoutControlItem, this.oracleLoginLayoutControlItem, this.oraclePasswordLayoutControlItem, this.emptySpaceItem10, this.emptySpaceItem11, this.emptySpaceItem12, this.emptySpaceItem24, this.oracleSavePasswordLayoutControlItem, this.emptySpaceItem30,
			this.emptySpaceItem31, this.emptySpaceItem32, this.oracleSchemaLayoutControlItem, this.emptySpaceItem33, this.layoutControlItem38, this.oracleTimeoutEmptySpaceItem, this.oracleUseDifferentSchemaLayoutControlItem, this.emptySpaceItem47, this.oracleConnectionTypeLayoutControlItem, this.oracleHostLayoutControlItem,
			this.emptySpaceItem6, this.emptySpaceItem40, this.schemasCountLayoutControlItem, this.layoutControlItem22, this.emptySpaceItem63
		});
		this.layoutControlGroup5.Name = "Root";
		this.layoutControlGroup5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup5.Size = new System.Drawing.Size(600, 366);
		this.layoutControlGroup5.TextVisible = false;
		this.oracleServiceNameLayoutControlItem.Control = this.oracleServiceNameTextEdit;
		this.oracleServiceNameLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.oracleServiceNameLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.oracleServiceNameLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.oracleServiceNameLayoutControlItem.Name = "oracleServiceNameLayoutControlItem";
		this.oracleServiceNameLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.oracleServiceNameLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.oracleServiceNameLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.oracleServiceNameLayoutControlItem.Text = " ";
		this.oracleServiceNameLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.oracleServiceNameLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.oracleServiceNameLayoutControlItem.TextToControlDistance = 5;
		this.oraclePortLayoutControlItem.Control = this.oraclePortTextEdit;
		this.oraclePortLayoutControlItem.CustomizationFormText = "Port:";
		this.oraclePortLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.oraclePortLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.oraclePortLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.oraclePortLayoutControlItem.Name = "oraclePortLayoutControlItem";
		this.oraclePortLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.oraclePortLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.oraclePortLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.oraclePortLayoutControlItem.Text = "Port:";
		this.oraclePortLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.oraclePortLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.oraclePortLayoutControlItem.TextToControlDistance = 5;
		this.oracleLoginLayoutControlItem.Control = this.oracleLoginTextEdit;
		this.oracleLoginLayoutControlItem.CustomizationFormText = "User:";
		this.oracleLoginLayoutControlItem.Location = new System.Drawing.Point(0, 120);
		this.oracleLoginLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.oracleLoginLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.oracleLoginLayoutControlItem.Name = "oracleLoginLayoutControlItem";
		this.oracleLoginLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.oracleLoginLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.oracleLoginLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.oracleLoginLayoutControlItem.Text = "User:";
		this.oracleLoginLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.oracleLoginLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.oracleLoginLayoutControlItem.TextToControlDistance = 5;
		this.oraclePasswordLayoutControlItem.Control = this.oraclePasswordTextEdit;
		this.oraclePasswordLayoutControlItem.CustomizationFormText = "Password";
		this.oraclePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 144);
		this.oraclePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.oraclePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.oraclePasswordLayoutControlItem.Name = "oraclePasswordLayoutControlItem";
		this.oraclePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.oraclePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.oraclePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.oraclePasswordLayoutControlItem.Text = "Password:";
		this.oraclePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.oraclePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.oraclePasswordLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem10.AllowHotTrack = false;
		this.emptySpaceItem10.CustomizationFormText = "emptySpaceItem14";
		this.emptySpaceItem10.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem10.Name = "emptySpaceItem14";
		this.emptySpaceItem10.Size = new System.Drawing.Size(265, 48);
		this.emptySpaceItem10.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem11.AllowHotTrack = false;
		this.emptySpaceItem11.CustomizationFormText = "emptySpaceItem15";
		this.emptySpaceItem11.Location = new System.Drawing.Point(383, 96);
		this.emptySpaceItem11.Name = "emptySpaceItem15";
		this.emptySpaceItem11.Size = new System.Drawing.Size(217, 24);
		this.emptySpaceItem11.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem12.AllowHotTrack = false;
		this.emptySpaceItem12.CustomizationFormText = "emptySpaceItem17";
		this.emptySpaceItem12.Location = new System.Drawing.Point(335, 120);
		this.emptySpaceItem12.Name = "emptySpaceItem17";
		this.emptySpaceItem12.Size = new System.Drawing.Size(265, 24);
		this.emptySpaceItem12.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem24.AllowHotTrack = false;
		this.emptySpaceItem24.CustomizationFormText = "emptySpaceItem18";
		this.emptySpaceItem24.Location = new System.Drawing.Point(335, 144);
		this.emptySpaceItem24.Name = "emptySpaceItem18";
		this.emptySpaceItem24.Size = new System.Drawing.Size(265, 24);
		this.emptySpaceItem24.TextSize = new System.Drawing.Size(0, 0);
		this.oracleSavePasswordLayoutControlItem.Control = this.oracleSavePasswordCheckEdit;
		this.oracleSavePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem18";
		this.oracleSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 168);
		this.oracleSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.oracleSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.oracleSavePasswordLayoutControlItem.Name = "oracleSavePasswordLayoutControlItem";
		this.oracleSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.oracleSavePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.oracleSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.oracleSavePasswordLayoutControlItem.Text = " ";
		this.oracleSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.oracleSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.oracleSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem30.AllowHotTrack = false;
		this.emptySpaceItem30.Location = new System.Drawing.Point(0, 259);
		this.emptySpaceItem30.Name = "emptySpaceItem29";
		this.emptySpaceItem30.Size = new System.Drawing.Size(600, 107);
		this.emptySpaceItem30.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem31.AllowHotTrack = false;
		this.emptySpaceItem31.Location = new System.Drawing.Point(335, 192);
		this.emptySpaceItem31.Name = "emptySpaceItem23";
		this.emptySpaceItem31.Size = new System.Drawing.Size(265, 24);
		this.emptySpaceItem31.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem32.AllowHotTrack = false;
		this.emptySpaceItem32.CustomizationFormText = "emptySpaceItem16";
		this.emptySpaceItem32.Location = new System.Drawing.Point(491, 216);
		this.emptySpaceItem32.Name = "emptySpaceItem16";
		this.emptySpaceItem32.Size = new System.Drawing.Size(109, 24);
		this.emptySpaceItem32.TextSize = new System.Drawing.Size(0, 0);
		this.oracleSchemaLayoutControlItem.Control = this.oracleSchemaButtonEdit;
		this.oracleSchemaLayoutControlItem.Location = new System.Drawing.Point(0, 216);
		this.oracleSchemaLayoutControlItem.Name = "oracleSchemaLayoutControlItem";
		this.oracleSchemaLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.oracleSchemaLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.oracleSchemaLayoutControlItem.Text = "Schema:";
		this.oracleSchemaLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.oracleSchemaLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.oracleSchemaLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem33.AllowHotTrack = false;
		this.emptySpaceItem33.Location = new System.Drawing.Point(335, 168);
		this.emptySpaceItem33.Name = "emptySpaceItem4";
		this.emptySpaceItem33.Size = new System.Drawing.Size(265, 24);
		this.emptySpaceItem33.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem38.Control = this.oracleDefaultPortLabelControl;
		this.layoutControlItem38.Location = new System.Drawing.Point(345, 96);
		this.layoutControlItem38.MaxSize = new System.Drawing.Size(38, 24);
		this.layoutControlItem38.MinSize = new System.Drawing.Size(38, 24);
		this.layoutControlItem38.Name = "layoutControlItem31";
		this.layoutControlItem38.Size = new System.Drawing.Size(38, 24);
		this.layoutControlItem38.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem38.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem38.TextVisible = false;
		this.oracleTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.oracleTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 240);
		this.oracleTimeoutEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 19);
		this.oracleTimeoutEmptySpaceItem.MinSize = new System.Drawing.Size(10, 19);
		this.oracleTimeoutEmptySpaceItem.Name = "oracleTimeoutEmptySpaceItem";
		this.oracleTimeoutEmptySpaceItem.Size = new System.Drawing.Size(600, 19);
		this.oracleTimeoutEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.oracleTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.oracleUseDifferentSchemaLayoutControlItem.Control = this.oracleUseDifferentSchemaCheckEdit;
		this.oracleUseDifferentSchemaLayoutControlItem.Location = new System.Drawing.Point(0, 192);
		this.oracleUseDifferentSchemaLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.oracleUseDifferentSchemaLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.oracleUseDifferentSchemaLayoutControlItem.Name = "oracleUseDifferentSchemaLayoutControlItem";
		this.oracleUseDifferentSchemaLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.oracleUseDifferentSchemaLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.oracleUseDifferentSchemaLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.oracleUseDifferentSchemaLayoutControlItem.Text = " ";
		this.oracleUseDifferentSchemaLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.oracleUseDifferentSchemaLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.oracleUseDifferentSchemaLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem47.AllowHotTrack = false;
		this.emptySpaceItem47.Location = new System.Drawing.Point(335, 96);
		this.emptySpaceItem47.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem47.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem47.Name = "emptySpaceItem47";
		this.emptySpaceItem47.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem47.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem47.TextSize = new System.Drawing.Size(0, 0);
		this.oracleConnectionTypeLayoutControlItem.Control = this.oracleConnectionTypeLookUpEdit;
		this.oracleConnectionTypeLayoutControlItem.CustomizationFormText = "Connection type:";
		this.oracleConnectionTypeLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.oracleConnectionTypeLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.oracleConnectionTypeLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.oracleConnectionTypeLayoutControlItem.Name = "oracleConnectionTypeLayoutControlItem";
		this.oracleConnectionTypeLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.oracleConnectionTypeLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.oracleConnectionTypeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.oracleConnectionTypeLayoutControlItem.Text = "Connection type:";
		this.oracleConnectionTypeLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.oracleConnectionTypeLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.oracleConnectionTypeLayoutControlItem.TextToControlDistance = 5;
		this.oracleHostLayoutControlItem.Control = this.oracleHostComboBoxEdit;
		this.oracleHostLayoutControlItem.CustomizationFormText = "Host:";
		this.oracleHostLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.oracleHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.oracleHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.oracleHostLayoutControlItem.Name = "oracleHostLayoutControlItem";
		this.oracleHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.oracleHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.oracleHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.oracleHostLayoutControlItem.Text = "Host:";
		this.oracleHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.oracleHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.oracleHostLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem6.AllowHotTrack = false;
		this.emptySpaceItem6.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem6.Name = "emptySpaceItem6";
		this.emptySpaceItem6.Size = new System.Drawing.Size(265, 24);
		this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem40.AllowHotTrack = false;
		this.emptySpaceItem40.Location = new System.Drawing.Point(335, 24);
		this.emptySpaceItem40.Name = "emptySpaceItem40";
		this.emptySpaceItem40.Size = new System.Drawing.Size(265, 24);
		this.emptySpaceItem40.TextSize = new System.Drawing.Size(0, 0);
		this.schemasCountLayoutControlItem.Control = this.schemasCountLabelControl;
		this.schemasCountLayoutControlItem.Location = new System.Drawing.Point(345, 216);
		this.schemasCountLayoutControlItem.MinSize = new System.Drawing.Size(80, 17);
		this.schemasCountLayoutControlItem.Name = "schemasCountLayoutControlItem";
		this.schemasCountLayoutControlItem.Size = new System.Drawing.Size(146, 24);
		this.schemasCountLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.schemasCountLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.schemasCountLayoutControlItem.TextVisible = false;
		this.layoutControlItem22.Control = this.instanceIdentifierLookUpEdit;
		this.layoutControlItem22.Location = new System.Drawing.Point(0, 48);
		this.layoutControlItem22.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem22.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem22.Name = "layoutControlItem22";
		this.layoutControlItem22.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem22.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem22.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem22.Text = "Instance identifier";
		this.layoutControlItem22.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem22.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem22.TextToControlDistance = 5;
		this.emptySpaceItem63.AllowHotTrack = false;
		this.emptySpaceItem63.Location = new System.Drawing.Point(335, 216);
		this.emptySpaceItem63.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem63.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem63.Name = "emptySpaceItem63";
		this.emptySpaceItem63.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem63.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem63.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.oracleLayoutControl);
		base.Name = "OracleConnectorControl";
		base.Size = new System.Drawing.Size(600, 366);
		((System.ComponentModel.ISupportInitialize)this.oracleLayoutControl).EndInit();
		this.oracleLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.instanceIdentifierLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleUseDifferentSchemaCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleSchemaButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oraclePasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleLoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oraclePortTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleServiceNameTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleConnectionTypeLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleServiceNameLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oraclePortLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleLoginLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oraclePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem30).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem31).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem32).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleSchemaLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem33).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem38).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleTimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleUseDifferentSchemaLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem47).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleConnectionTypeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.oracleHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem40).EndInit();
		((System.ComponentModel.ISupportInitialize)this.schemasCountLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem22).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem63).EndInit();
		base.ResumeLayout(false);
	}
}
