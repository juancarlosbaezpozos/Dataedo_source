using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classes;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class CosmosDbConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private ButtonEdit databaseButtonEdit;

	private CheckEdit savePasswordCheckEdit;

	private TextEdit passwordTextEdit;

	private ComboBoxEdit hostComboBoxEdit;

	private LayoutControlGroup layoutControlGroup4;

	private LayoutControlItem passwordLayoutControlItem;

	private LayoutControlItem savePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem8;

	private EmptySpaceItem emptySpaceItem9;

	private LayoutControlItem databaseLayoutControlItem;

	private LayoutControlItem layoutControlItem;

	private EmptySpaceItem emptySpaceItem3;

	private EmptySpaceItem emptySpaceItem4;

	private string providedCosmosDbHost => splittedHost?.Host ?? hostComboBoxEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.CosmosDBSqlAPI;

	protected override TextEdit HostTextEdit => hostComboBoxEdit;

	protected override ComboBoxEdit HostComboBoxEdit => hostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => savePasswordCheckEdit;

	public CosmosDbConnectorControl()
	{
		InitializeComponent();
	}

	public override void SetSwitchDatabaseAvailability(bool isAvailable)
	{
		databaseButtonEdit.Enabled = isAvailable;
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? databaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedCosmosDbHost, null, passwordTextEdit.Text, false, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, null);
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateCosmosDbHost();
		flag &= ValidateCosmosDbPassword();
		if (!testForGettingDatabasesList)
		{
			flag &= ValidateCosmosDbDatabase();
		}
		return flag;
	}

	private bool ValidateCosmosDbHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(hostComboBoxEdit, addDBErrorProvider, "Server name", acceptEmptyValue);
	}

	private bool ValidateCosmosDbPassword(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(passwordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateCosmosDbDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(databaseButtonEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		hostComboBoxEdit.Text = base.DatabaseRow.Host;
		databaseButtonEdit.EditValue = base.DatabaseRow.Name;
		passwordTextEdit.Text = value;
		savePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return databaseButtonEdit.Text + "@" + providedCosmosDbHost;
	}

	private void cosmosDbDatabaseButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateCosmosDbDatabase(acceptEmptyValue: true);
	}

	private void cosmosDbPasswordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateCosmosDbPassword();
	}

	private new void DatabaseButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		base.DatabaseButtonEdit_ButtonClick(sender, e);
	}

	private new void HostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		base.HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void HostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		passwordTextEdit.Text = string.Empty;
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
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.databaseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.savePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.passwordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.hostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.passwordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.savePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem9 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.databaseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.databaseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.databaseButtonEdit);
		this.mainLayoutControl.Controls.Add(this.savePasswordCheckEdit);
		this.mainLayoutControl.Controls.Add(this.passwordTextEdit);
		this.mainLayoutControl.Controls.Add(this.hostComboBoxEdit);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2719, 179, 279, 548);
		this.mainLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.mainLayoutControl.Root = this.layoutControlGroup4;
		this.mainLayoutControl.Size = new System.Drawing.Size(499, 322);
		this.mainLayoutControl.TabIndex = 3;
		this.mainLayoutControl.Text = "layoutControl1";
		this.databaseButtonEdit.Location = new System.Drawing.Point(105, 72);
		this.databaseButtonEdit.Name = "cosmosDbDatabaseButtonEdit";
		this.databaseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.databaseButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.databaseButtonEdit.StyleController = this.mainLayoutControl;
		this.databaseButtonEdit.TabIndex = 4;
		this.databaseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(DatabaseButtonEdit_ButtonClick);
		this.databaseButtonEdit.EditValueChanged += new System.EventHandler(cosmosDbDatabaseButtonEdit_EditValueChanged);
		this.savePasswordCheckEdit.Location = new System.Drawing.Point(105, 48);
		this.savePasswordCheckEdit.Name = "cosmosDbSavePasswordCheckEdit";
		this.savePasswordCheckEdit.Properties.Caption = "Save password";
		this.savePasswordCheckEdit.Size = new System.Drawing.Size(230, 20);
		this.savePasswordCheckEdit.StyleController = this.mainLayoutControl;
		this.savePasswordCheckEdit.TabIndex = 3;
		this.passwordTextEdit.Location = new System.Drawing.Point(105, 24);
		this.passwordTextEdit.Name = "cosmosDbPasswordTextEdit";
		this.passwordTextEdit.Properties.UseSystemPasswordChar = true;
		this.passwordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.passwordTextEdit.StyleController = this.mainLayoutControl;
		this.passwordTextEdit.TabIndex = 2;
		this.passwordTextEdit.EditValueChanged += new System.EventHandler(cosmosDbPasswordTextEdit_EditValueChanged);
		this.hostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.hostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2);
		this.hostComboBoxEdit.Name = "cosmosDbHostComboBoxEdit";
		this.hostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.hostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.hostComboBoxEdit.StyleController = this.mainLayoutControl;
		this.hostComboBoxEdit.TabIndex = 1;
		this.hostComboBoxEdit.EditValueChanged += new System.EventHandler(HostComboBoxEdit_EditValueChanged);
		this.hostComboBoxEdit.Leave += new System.EventHandler(HostComboBoxEdit_Leave);
		this.layoutControlGroup4.CustomizationFormText = "Root";
		this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup4.GroupBordersVisible = false;
		this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[9] { this.passwordLayoutControlItem, this.savePasswordLayoutControlItem, this.emptySpaceItem1, this.emptySpaceItem8, this.emptySpaceItem9, this.databaseLayoutControlItem, this.layoutControlItem, this.emptySpaceItem3, this.emptySpaceItem4 });
		this.layoutControlGroup4.Name = "Root";
		this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup4.Size = new System.Drawing.Size(499, 322);
		this.layoutControlGroup4.TextVisible = false;
		this.passwordLayoutControlItem.Control = this.passwordTextEdit;
		this.passwordLayoutControlItem.CustomizationFormText = "Primary Key:";
		this.passwordLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.passwordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.passwordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.passwordLayoutControlItem.Name = "cosmosDbPasswordLayoutControlItem";
		this.passwordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.passwordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.passwordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.passwordLayoutControlItem.Text = "Primary Key:";
		this.passwordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.passwordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.passwordLayoutControlItem.TextToControlDistance = 5;
		this.savePasswordLayoutControlItem.Control = this.savePasswordCheckEdit;
		this.savePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem2";
		this.savePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.savePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.savePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.savePasswordLayoutControlItem.Name = "savePasswordLayoutControlItem";
		this.savePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.savePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.savePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.savePasswordLayoutControlItem.Text = " ";
		this.savePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.savePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.savePasswordLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem6";
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 216);
		this.emptySpaceItem1.Name = "emptySpaceItem6";
		this.emptySpaceItem1.Size = new System.Drawing.Size(499, 106);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem8.AllowHotTrack = false;
		this.emptySpaceItem8.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem8.Location = new System.Drawing.Point(335, 24);
		this.emptySpaceItem8.Name = "emptySpaceItem22";
		this.emptySpaceItem8.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem9.AllowHotTrack = false;
		this.emptySpaceItem9.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem9.Location = new System.Drawing.Point(335, 79);
		this.emptySpaceItem9.Name = "emptySpaceItem28";
		this.emptySpaceItem9.Size = new System.Drawing.Size(164, 17);
		this.emptySpaceItem9.Text = "emptySpaceItem22";
		this.emptySpaceItem9.TextSize = new System.Drawing.Size(0, 0);
		this.databaseLayoutControlItem.Control = this.databaseButtonEdit;
		this.databaseLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.databaseLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.databaseLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.databaseLayoutControlItem.Name = "databaseLayoutControlItem";
		this.databaseLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.databaseLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.databaseLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.databaseLayoutControlItem.Text = "Database:";
		this.databaseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.databaseLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.databaseLayoutControlItem.TextToControlDistance = 5;
		this.layoutControlItem.Control = this.hostComboBoxEdit;
		this.layoutControlItem.CustomizationFormText = "URI:";
		this.layoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem.Name = "cosmosDbLayoutControlItem";
		this.layoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem.Text = "URI:";
		this.layoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(164, 31);
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Margin = new System.Windows.Forms.Padding(2);
		base.Name = "CosmosDbConnectorControl";
		base.Size = new System.Drawing.Size(499, 322);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.databaseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.savePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		base.ResumeLayout(false);
	}
}
