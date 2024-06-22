using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class SSISConnectorControl : ConnectorControlBase
{
	private SSISPackageFileHelper fileHelper;

	private IContainer components;

	private NonCustomizableLayoutControl ssisLayoutControl;

	private TextEdit ssisPasswordTextEdit;

	private TextEdit ssisLoginTextEdit;

	private ComboBoxEdit ssisHostComboBoxEdit;

	private ButtonEdit ssisDatabaseImportButtonEdit;

	private LookUpEdit ssisConnectionTypeLookUpEdit;

	private LayoutControlGroup Root;

	private LayoutControlItem ssisHostLayoutControlItem;

	private LayoutControlItem ssisLoginLayoutControlItem;

	private LayoutControlItem ssisPasswordLayoutControlItem;

	private EmptySpaceItem ssisEmptySpaceItem2;

	private LayoutControlItem ssisSavePasswordLayoutControlItem;

	private EmptySpaceItem ssisLoginLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem ssisPasswordLayoutControlItemEmptySpaceItem;

	private LayoutControlItem ssisImportDatabaseLayoutControlItem;

	private EmptySpaceItem ssisImportDatabaseLayoutControlItemEmptySpaceItem2;

	private LayoutControlItem ssisConnectionTypeLayoutControlItem;

	private EmptySpaceItem ssisConnectionTypeLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem ssisHostLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem ssisEmptySpaceItem;

	private EmptySpaceItem ssisSavePasswordLayoutControlItemEmptySpaceItem;

	private CheckEdit ssisPasswordCheckEdit;

	private CustomTextEdit packageFileCustomTextEdit;

	private LayoutControlItem packageFileLayoutControlItem;

	private SimpleButton packageFileSimpleButton;

	private LayoutControlItem layoutControlItem1;

	private string host
	{
		get
		{
			if (string.IsNullOrEmpty(ssisHostComboBoxEdit.Text))
			{
				return "SSIS";
			}
			return ssisHostComboBoxEdit.Text;
		}
	}

	private string databaseName
	{
		get
		{
			if (string.IsNullOrEmpty(ssisDatabaseImportButtonEdit.Text))
			{
				return Path.GetFileName(packageFileCustomTextEdit.Text);
			}
			return ssisDatabaseImportButtonEdit.Text;
		}
	}

	private SSISConnectionTypeEnum.SSISConnectionType ssisConnectionType => (SSISConnectionTypeEnum.SSISConnectionType)ssisConnectionTypeLookUpEdit.EditValue;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.SSIS;

	protected override TextEdit HostTextEdit => ssisHostComboBoxEdit;

	protected override ComboBoxEdit HostComboBoxEdit => ssisHostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => ssisPasswordCheckEdit;

	public SSISConnectorControl()
	{
		InitializeComponent();
		fileHelper = new SSISPackageFileHelper();
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		SetSSISConnectionTypes();
		SetSSISConnectionTypeFields();
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
	}

	private void SetSSISConnectionTypeFields()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		object editValue = ssisConnectionTypeLookUpEdit.EditValue;
		if (editValue is SSISConnectionTypeEnum.SSISConnectionType)
		{
			SSISConnectionTypeEnum.SSISConnectionType sSISConnectionType = (SSISConnectionTypeEnum.SSISConnectionType)editValue;
			if (sSISConnectionType == SSISConnectionTypeEnum.SSISConnectionType.DTSX || sSISConnectionType == SSISConnectionTypeEnum.SSISConnectionType.ISPAC)
			{
				LayoutControlItem layoutControlItem = ssisHostLayoutControlItem;
				LayoutControlItem layoutControlItem2 = ssisImportDatabaseLayoutControlItem;
				EmptySpaceItem emptySpaceItem = ssisImportDatabaseLayoutControlItemEmptySpaceItem2;
				LayoutControlItem layoutControlItem3 = ssisHostLayoutControlItem;
				EmptySpaceItem emptySpaceItem2 = ssisHostLayoutControlItemEmptySpaceItem;
				LayoutControlItem layoutControlItem4 = ssisLoginLayoutControlItem;
				EmptySpaceItem emptySpaceItem3 = ssisLoginLayoutControlItemEmptySpaceItem;
				LayoutControlItem layoutControlItem5 = ssisPasswordLayoutControlItem;
				EmptySpaceItem emptySpaceItem4 = ssisPasswordLayoutControlItemEmptySpaceItem;
				LayoutControlItem layoutControlItem6 = ssisSavePasswordLayoutControlItem;
				EmptySpaceItem emptySpaceItem5 = ssisSavePasswordLayoutControlItemEmptySpaceItem;
				LayoutVisibility layoutVisibility2 = (timeoutLayoutControlItem.Visibility = LayoutVisibility.Never);
				LayoutVisibility layoutVisibility4 = (emptySpaceItem5.Visibility = layoutVisibility2);
				LayoutVisibility layoutVisibility6 = (layoutControlItem6.Visibility = layoutVisibility4);
				LayoutVisibility layoutVisibility8 = (emptySpaceItem4.Visibility = layoutVisibility6);
				LayoutVisibility layoutVisibility10 = (layoutControlItem5.Visibility = layoutVisibility8);
				LayoutVisibility layoutVisibility12 = (emptySpaceItem3.Visibility = layoutVisibility10);
				LayoutVisibility layoutVisibility14 = (layoutControlItem4.Visibility = layoutVisibility12);
				LayoutVisibility layoutVisibility16 = (emptySpaceItem2.Visibility = layoutVisibility14);
				LayoutVisibility layoutVisibility18 = (layoutControlItem3.Visibility = layoutVisibility16);
				LayoutVisibility layoutVisibility20 = (emptySpaceItem.Visibility = layoutVisibility18);
				LayoutVisibility layoutVisibility23 = (layoutControlItem.Visibility = (layoutControlItem2.Visibility = layoutVisibility20));
				packageFileLayoutControlItem.Visibility = LayoutVisibility.Always;
			}
		}
	}

	private void SetSSISConnectionTypes()
	{
		Dictionary<SSISConnectionTypeEnum.SSISConnectionType, string> sSISConnectionTypes = SSISConnectionTypeEnum.GetSSISConnectionTypes();
		ConnectorControlBase.SetDropDownSize(ssisConnectionTypeLookUpEdit, sSISConnectionTypes.Count);
		ssisConnectionTypeLookUpEdit.Properties.DataSource = sSISConnectionTypes;
		ssisConnectionTypeLookUpEdit.EditValue = ((ssisConnectionTypeLookUpEdit.EditValue == null) ? ((object)SSISConnectionTypeEnum.SSISConnectionType.ISPAC) : ssisConnectionTypeLookUpEdit.EditValue);
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, databaseName, documentationTitle, host, null, null, null, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion)
		{
			Param1 = ((SSISConnectionTypeEnum.SSISConnectionType)ssisConnectionTypeLookUpEdit.EditValue).ToString(),
			Param2 = packageFileCustomTextEdit.Text
		};
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		return true & ValidateSSISConnectionType() & ValidatePackagePath();
	}

	private bool ValidateSSISConnectionType(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(ssisConnectionTypeLookUpEdit, addDBErrorProvider, "connection type", acceptEmptyValue);
	}

	private bool ValidateSSISHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(ssisHostComboBoxEdit, addDBErrorProvider, "host", acceptEmptyValue);
	}

	private bool ValidateSSISUsername(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(ssisLoginTextEdit, addDBErrorProvider, "username", acceptEmptyValue);
	}

	private bool ValidateSSISPassword(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(ssisPasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateSSISDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(ssisDatabaseImportButtonEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	private bool ValidatePackagePath(bool acceptEmtyValue = false)
	{
		return ValidateFields.ValidateEdit(packageFileCustomTextEdit, addDBErrorProvider, "path", acceptEmtyValue);
	}

	protected override bool GetSavedPassword()
	{
		return GetSavePasswordCheckEditState();
	}

	protected override void ReadPanelValues()
	{
		if (!string.IsNullOrEmpty(base.DatabaseRow.Param1) && Enum.TryParse<SSISConnectionTypeEnum.SSISConnectionType>(base.DatabaseRow.Param1, out var result))
		{
			ssisConnectionTypeLookUpEdit.EditValue = result;
		}
		packageFileCustomTextEdit.Text = base.DatabaseRow.Param2;
	}

	protected override string GetPanelDocumentationTitle()
	{
		if (!string.IsNullOrEmpty(databaseName))
		{
			return databaseName + "@" + host;
		}
		return host ?? "";
	}

	private void ssisConnectionTypeLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		SetSSISConnectionTypeFields();
	}

	private void ssisDatabaseImportButtonEdit_Leave(object sender, EventArgs e)
	{
	}

	private void ssisDatabaseImportButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		schemas = ConnectorControlBase.GetPrintedSchemas(sender as ButtonEdit);
	}

	private void ssisDatabaseImportButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		HandleSchemaButtonEdit(sender as ButtonEdit, testConnection: true, "Databases");
	}

	private void ssisHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void ssisHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	public override void AfterChangingUsedProfileType()
	{
		SetSSISConnectionTypeFields();
	}

	protected override void ClearPanelLoginAndPassword()
	{
		ssisLoginTextEdit.Text = string.Empty;
		ssisPasswordTextEdit.Text = string.Empty;
	}

	private void packageFileSimpleButton_Click(object sender, EventArgs e)
	{
		packageFileCustomTextEdit.Text = fileHelper.GetFilePathWithPrefix((SSISConnectionTypeEnum.SSISConnectionType)ssisConnectionTypeLookUpEdit.EditValue);
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
		this.ssisLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.packageFileSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.ssisPasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.ssisLoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.ssisHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.ssisPasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.ssisDatabaseImportButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.ssisConnectionTypeLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.packageFileCustomTextEdit = new Dataedo.App.UserControls.CustomTextEdit();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.ssisHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.ssisLoginLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.ssisPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.ssisEmptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.ssisSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.ssisLoginLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.ssisPasswordLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.ssisImportDatabaseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.ssisImportDatabaseLayoutControlItemEmptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.ssisConnectionTypeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.ssisConnectionTypeLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.ssisHostLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.ssisEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.ssisSavePasswordLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.packageFileLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.ssisLayoutControl).BeginInit();
		this.ssisLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ssisPasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisLoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisPasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisDatabaseImportButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisConnectionTypeLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.packageFileCustomTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisLoginLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisPasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisEmptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisLoginLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisPasswordLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisImportDatabaseLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisImportDatabaseLayoutControlItemEmptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisConnectionTypeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisConnectionTypeLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisHostLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ssisSavePasswordLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.packageFileLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		base.SuspendLayout();
		this.ssisLayoutControl.AllowCustomization = false;
		this.ssisLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.ssisLayoutControl.Controls.Add(this.packageFileSimpleButton);
		this.ssisLayoutControl.Controls.Add(this.ssisPasswordTextEdit);
		this.ssisLayoutControl.Controls.Add(this.ssisLoginTextEdit);
		this.ssisLayoutControl.Controls.Add(this.ssisHostComboBoxEdit);
		this.ssisLayoutControl.Controls.Add(this.ssisPasswordCheckEdit);
		this.ssisLayoutControl.Controls.Add(this.ssisDatabaseImportButtonEdit);
		this.ssisLayoutControl.Controls.Add(this.ssisConnectionTypeLookUpEdit);
		this.ssisLayoutControl.Controls.Add(this.packageFileCustomTextEdit);
		this.ssisLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ssisLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.ssisLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.ssisLayoutControl.Name = "ssisLayoutControl";
		this.ssisLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3376, 367, 937, 686);
		this.ssisLayoutControl.Root = this.Root;
		this.ssisLayoutControl.Size = new System.Drawing.Size(624, 531);
		this.ssisLayoutControl.TabIndex = 1;
		this.ssisLayoutControl.Text = "layoutControl3";
		this.packageFileSimpleButton.Location = new System.Drawing.Point(537, 146);
		this.packageFileSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.packageFileSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.packageFileSimpleButton.Name = "packageFileSimpleButton";
		this.packageFileSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.packageFileSimpleButton.StyleController = this.ssisLayoutControl;
		this.packageFileSimpleButton.TabIndex = 12;
		this.packageFileSimpleButton.Text = "Browse";
		this.packageFileSimpleButton.Click += new System.EventHandler(packageFileSimpleButton_Click);
		this.ssisPasswordTextEdit.Location = new System.Drawing.Point(105, 96);
		this.ssisPasswordTextEdit.Name = "ssisPasswordTextEdit";
		this.ssisPasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.ssisPasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.ssisPasswordTextEdit.StyleController = this.ssisLayoutControl;
		this.ssisPasswordTextEdit.TabIndex = 7;
		this.ssisLoginTextEdit.Location = new System.Drawing.Point(105, 72);
		this.ssisLoginTextEdit.Name = "ssisLoginTextEdit";
		this.ssisLoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.ssisLoginTextEdit.StyleController = this.ssisLayoutControl;
		this.ssisLoginTextEdit.TabIndex = 6;
		this.ssisHostComboBoxEdit.Location = new System.Drawing.Point(105, 24);
		this.ssisHostComboBoxEdit.Name = "ssisHostComboBoxEdit";
		this.ssisHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ssisHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.ssisHostComboBoxEdit.StyleController = this.ssisLayoutControl;
		this.ssisHostComboBoxEdit.TabIndex = 4;
		this.ssisHostComboBoxEdit.EditValueChanged += new System.EventHandler(ssisHostComboBoxEdit_EditValueChanged);
		this.ssisHostComboBoxEdit.Leave += new System.EventHandler(ssisHostComboBoxEdit_Leave);
		this.ssisPasswordCheckEdit.Location = new System.Drawing.Point(105, 120);
		this.ssisPasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.ssisPasswordCheckEdit.Name = "ssisPasswordCheckEdit";
		this.ssisPasswordCheckEdit.Properties.Caption = "Save password";
		this.ssisPasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.ssisPasswordCheckEdit.StyleController = this.ssisLayoutControl;
		this.ssisPasswordCheckEdit.TabIndex = 3;
		this.ssisDatabaseImportButtonEdit.Location = new System.Drawing.Point(105, 48);
		this.ssisDatabaseImportButtonEdit.Name = "ssisDatabaseImportButtonEdit";
		this.ssisDatabaseImportButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.ssisDatabaseImportButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.ssisDatabaseImportButtonEdit.StyleController = this.ssisLayoutControl;
		this.ssisDatabaseImportButtonEdit.TabIndex = 4;
		this.ssisDatabaseImportButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(ssisDatabaseImportButtonEdit_ButtonClick);
		this.ssisDatabaseImportButtonEdit.EditValueChanged += new System.EventHandler(ssisDatabaseImportButtonEdit_EditValueChanged);
		this.ssisDatabaseImportButtonEdit.Leave += new System.EventHandler(ssisDatabaseImportButtonEdit_Leave);
		this.ssisConnectionTypeLookUpEdit.Location = new System.Drawing.Point(105, 0);
		this.ssisConnectionTypeLookUpEdit.Name = "ssisConnectionTypeLookUpEdit";
		this.ssisConnectionTypeLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ssisConnectionTypeLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name")
		});
		this.ssisConnectionTypeLookUpEdit.Properties.DisplayMember = "Value";
		this.ssisConnectionTypeLookUpEdit.Properties.NullText = "";
		this.ssisConnectionTypeLookUpEdit.Properties.ShowFooter = false;
		this.ssisConnectionTypeLookUpEdit.Properties.ShowHeader = false;
		this.ssisConnectionTypeLookUpEdit.Properties.ShowLines = false;
		this.ssisConnectionTypeLookUpEdit.Properties.ValueMember = "Key";
		this.ssisConnectionTypeLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.ssisConnectionTypeLookUpEdit.StyleController = this.ssisLayoutControl;
		this.ssisConnectionTypeLookUpEdit.TabIndex = 0;
		this.ssisConnectionTypeLookUpEdit.EditValueChanged += new System.EventHandler(ssisConnectionTypeLookUpEdit_EditValueChanged);
		this.packageFileCustomTextEdit.Location = new System.Drawing.Point(105, 147);
		this.packageFileCustomTextEdit.Name = "packageFileCustomTextEdit";
		this.packageFileCustomTextEdit.Size = new System.Drawing.Size(430, 20);
		this.packageFileCustomTextEdit.StyleController = this.ssisLayoutControl;
		this.packageFileCustomTextEdit.TabIndex = 8;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[16]
		{
			this.ssisHostLayoutControlItem, this.ssisLoginLayoutControlItem, this.ssisPasswordLayoutControlItem, this.ssisEmptySpaceItem2, this.ssisSavePasswordLayoutControlItem, this.ssisLoginLayoutControlItemEmptySpaceItem, this.ssisPasswordLayoutControlItemEmptySpaceItem, this.ssisImportDatabaseLayoutControlItem, this.ssisImportDatabaseLayoutControlItemEmptySpaceItem2, this.ssisConnectionTypeLayoutControlItem,
			this.ssisConnectionTypeLayoutControlItemEmptySpaceItem, this.ssisHostLayoutControlItemEmptySpaceItem, this.ssisEmptySpaceItem, this.ssisSavePasswordLayoutControlItemEmptySpaceItem, this.packageFileLayoutControlItem, this.layoutControlItem1
		});
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(624, 531);
		this.Root.TextVisible = false;
		this.ssisHostLayoutControlItem.Control = this.ssisHostComboBoxEdit;
		this.ssisHostLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.ssisHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.ssisHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.ssisHostLayoutControlItem.Name = "ssisHostLayoutControlItem";
		this.ssisHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.ssisHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.ssisHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.ssisHostLayoutControlItem.Text = "Host:";
		this.ssisHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.ssisHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.ssisHostLayoutControlItem.TextToControlDistance = 5;
		this.ssisLoginLayoutControlItem.Control = this.ssisLoginTextEdit;
		this.ssisLoginLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.ssisLoginLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.ssisLoginLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.ssisLoginLayoutControlItem.Name = "ssisLoginLayoutControlItem";
		this.ssisLoginLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.ssisLoginLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.ssisLoginLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.ssisLoginLayoutControlItem.Text = "Username:";
		this.ssisLoginLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.ssisLoginLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.ssisLoginLayoutControlItem.TextToControlDistance = 5;
		this.ssisPasswordLayoutControlItem.Control = this.ssisPasswordTextEdit;
		this.ssisPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.ssisPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.ssisPasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.ssisPasswordLayoutControlItem.Name = "ssisPasswordLayoutControlItem";
		this.ssisPasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.ssisPasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.ssisPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.ssisPasswordLayoutControlItem.Text = "Password:";
		this.ssisPasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.ssisPasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.ssisPasswordLayoutControlItem.TextToControlDistance = 5;
		this.ssisEmptySpaceItem2.AllowHotTrack = false;
		this.ssisEmptySpaceItem2.Location = new System.Drawing.Point(0, 521);
		this.ssisEmptySpaceItem2.MaxSize = new System.Drawing.Size(0, 10);
		this.ssisEmptySpaceItem2.MinSize = new System.Drawing.Size(104, 10);
		this.ssisEmptySpaceItem2.Name = "ssisEmptySpaceItem2";
		this.ssisEmptySpaceItem2.Size = new System.Drawing.Size(624, 10);
		this.ssisEmptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.ssisEmptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.ssisSavePasswordLayoutControlItem.Control = this.ssisPasswordCheckEdit;
		this.ssisSavePasswordLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.ssisSavePasswordLayoutControlItem.CustomizationFormText = "mongoDbSavePasswordLayoutControlItem";
		this.ssisSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 120);
		this.ssisSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.ssisSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.ssisSavePasswordLayoutControlItem.Name = "ssisSavePasswordLayoutControlItem";
		this.ssisSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.ssisSavePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.ssisSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.ssisSavePasswordLayoutControlItem.Text = " ";
		this.ssisSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.ssisSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.ssisSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.ssisLoginLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.ssisLoginLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 72);
		this.ssisLoginLayoutControlItemEmptySpaceItem.Name = "ssisLoginLayoutControlItemEmptySpaceItem";
		this.ssisLoginLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(289, 24);
		this.ssisLoginLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.ssisPasswordLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.ssisPasswordLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 96);
		this.ssisPasswordLayoutControlItemEmptySpaceItem.Name = "ssisPasswordLayoutControlItemEmptySpaceItem";
		this.ssisPasswordLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(289, 24);
		this.ssisPasswordLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.ssisImportDatabaseLayoutControlItem.Control = this.ssisDatabaseImportButtonEdit;
		this.ssisImportDatabaseLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.ssisImportDatabaseLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.ssisImportDatabaseLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.ssisImportDatabaseLayoutControlItem.Name = "ssisImportDatabaseLayoutControlItem";
		this.ssisImportDatabaseLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.ssisImportDatabaseLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.ssisImportDatabaseLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.ssisImportDatabaseLayoutControlItem.Text = "Database:";
		this.ssisImportDatabaseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.ssisImportDatabaseLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.ssisImportDatabaseLayoutControlItem.TextToControlDistance = 5;
		this.ssisImportDatabaseLayoutControlItemEmptySpaceItem2.AllowHotTrack = false;
		this.ssisImportDatabaseLayoutControlItemEmptySpaceItem2.Location = new System.Drawing.Point(335, 48);
		this.ssisImportDatabaseLayoutControlItemEmptySpaceItem2.Name = "ssisImportDatabaseLayoutControlItemEmptySpaceItem2";
		this.ssisImportDatabaseLayoutControlItemEmptySpaceItem2.Size = new System.Drawing.Size(289, 24);
		this.ssisImportDatabaseLayoutControlItemEmptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.ssisConnectionTypeLayoutControlItem.Control = this.ssisConnectionTypeLookUpEdit;
		this.ssisConnectionTypeLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.ssisConnectionTypeLayoutControlItem.CustomizationFormText = "Import type:";
		this.ssisConnectionTypeLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.ssisConnectionTypeLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.ssisConnectionTypeLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.ssisConnectionTypeLayoutControlItem.Name = "ssisConnectionTypeLayoutControlItem";
		this.ssisConnectionTypeLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.ssisConnectionTypeLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.ssisConnectionTypeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.ssisConnectionTypeLayoutControlItem.Text = "Import type:";
		this.ssisConnectionTypeLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.ssisConnectionTypeLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.ssisConnectionTypeLayoutControlItem.TextToControlDistance = 5;
		this.ssisConnectionTypeLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.ssisConnectionTypeLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 0);
		this.ssisConnectionTypeLayoutControlItemEmptySpaceItem.MinSize = new System.Drawing.Size(104, 24);
		this.ssisConnectionTypeLayoutControlItemEmptySpaceItem.Name = "ssisConnectionTypeLayoutControlItemEmptySpaceItem";
		this.ssisConnectionTypeLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(289, 24);
		this.ssisConnectionTypeLayoutControlItemEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.ssisConnectionTypeLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.ssisHostLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.ssisHostLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 24);
		this.ssisHostLayoutControlItemEmptySpaceItem.Name = "ssisHostLayoutControlItemEmptySpaceItem";
		this.ssisHostLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(289, 24);
		this.ssisHostLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.ssisEmptySpaceItem.AllowHotTrack = false;
		this.ssisEmptySpaceItem.Location = new System.Drawing.Point(0, 170);
		this.ssisEmptySpaceItem.MinSize = new System.Drawing.Size(104, 10);
		this.ssisEmptySpaceItem.Name = "ssisEmptySpaceItem";
		this.ssisEmptySpaceItem.Size = new System.Drawing.Size(624, 351);
		this.ssisEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.ssisEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.ssisSavePasswordLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.ssisSavePasswordLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 120);
		this.ssisSavePasswordLayoutControlItemEmptySpaceItem.Name = "ssisSavePasswordLayoutControlItemEmptySpaceItem";
		this.ssisSavePasswordLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(289, 24);
		this.ssisSavePasswordLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.packageFileLayoutControlItem.Control = this.packageFileCustomTextEdit;
		this.packageFileLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.packageFileLayoutControlItem.CustomizationFormText = "File path:";
		this.packageFileLayoutControlItem.Location = new System.Drawing.Point(0, 144);
		this.packageFileLayoutControlItem.MaxSize = new System.Drawing.Size(0, 26);
		this.packageFileLayoutControlItem.MinSize = new System.Drawing.Size(126, 26);
		this.packageFileLayoutControlItem.Name = "packageFileLayoutControlItem";
		this.packageFileLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.packageFileLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.packageFileLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.packageFileLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.packageFileLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.packageFileLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.packageFileLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 3, 0);
		this.packageFileLayoutControlItem.Size = new System.Drawing.Size(535, 26);
		this.packageFileLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.packageFileLayoutControlItem.Text = "File path:";
		this.packageFileLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.packageFileLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.packageFileLayoutControlItem.TextToControlDistance = 5;
		this.layoutControlItem1.Control = this.packageFileSimpleButton;
		this.layoutControlItem1.Location = new System.Drawing.Point(535, 144);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(89, 26);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.ssisLayoutControl);
		base.Name = "SSISConnectorControl";
		base.Size = new System.Drawing.Size(624, 531);
		((System.ComponentModel.ISupportInitialize)this.ssisLayoutControl).EndInit();
		this.ssisLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ssisPasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisLoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisPasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisDatabaseImportButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisConnectionTypeLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.packageFileCustomTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisLoginLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisPasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisEmptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisLoginLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisPasswordLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisImportDatabaseLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisImportDatabaseLayoutControlItemEmptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisConnectionTypeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisConnectionTypeLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisHostLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ssisSavePasswordLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.packageFileLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		base.ResumeLayout(false);
	}
}
