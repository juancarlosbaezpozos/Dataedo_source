using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class AmazonS3ConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl amazonS3LayoutControl;

	private TextEdit amazonS3AccesKeyTextEdit;

	private TextEdit amazonS3SecretKeyTextEdit;

	private CheckEdit amazonS3SaveSecretKeyCheckEdit;

	private ComboBoxEdit amazonS3ArnComboBoxEdit;

	private LayoutControlGroup layoutControlGroup6;

	private LayoutControlItem amazonS3AccessKeyLayoutControlItem;

	private LayoutControlItem amazonS3SecretKeyLayoutControlItem;

	private LayoutControlItem amazonS3SaveSecretKeyLayoutControlItem;

	private EmptySpaceItem emptySpaceItem34;

	private EmptySpaceItem emptySpaceItem35;

	private EmptySpaceItem emptySpaceItem38;

	private LayoutControlItem amazonS3ArnLayoutControlItem;

	private EmptySpaceItem emptySpaceItem45;

	private EmptySpaceItem configureEmptySpaceItem;

	private string providedArn => splittedHost?.Host ?? amazonS3ArnComboBoxEdit.Text;

	private string providedAccessKey => amazonS3AccesKeyTextEdit.Text;

	private string providedSecretKey => amazonS3SecretKeyTextEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.AmazonS3;

	protected override TextEdit HostTextEdit => amazonS3ArnComboBoxEdit;

	protected override ComboBoxEdit HostComboBoxEdit => amazonS3ArnComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => amazonS3SaveSecretKeyCheckEdit;

	public AmazonS3ConnectorControl()
	{
		InitializeComponent();
		InitEvents();
	}

	private void InitEvents()
	{
		amazonS3AccesKeyTextEdit.EditValueChanged += amazonS3AccesKeyTextEdit_EditValueChanged;
		amazonS3SecretKeyTextEdit.EditValueChanged += amazonS3SecretKeyTextEdit_EditValueChanged;
		amazonS3ArnComboBoxEdit.EditValueChanged += amazonS3ArnComboBoxEdit_EditValueChanged;
		amazonS3ArnComboBoxEdit.Leave += amazonS3ArnComboBoxEdit_Leave;
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
		DatabaseRow databaseRow = base.DatabaseRow;
		if (databaseRow != null && databaseRow.Type == SharedDatabaseTypeEnum.DatabaseType.AmazonS3)
		{
			ReadPanelValues();
		}
		DatabaseRow databaseRow2 = base.DatabaseRow;
		if (databaseRow2 != null && databaseRow2.Id.HasValue)
		{
			amazonS3SaveSecretKeyLayoutControlItem.HideToCustomization();
			base.HideSavingCredentials = true;
		}
		else
		{
			base.HideSavingCredentials = false;
		}
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow
		{
			Type = base.SelectedDatabaseType,
			Title = documentationTitle,
			Host = providedArn,
			User = providedAccessKey,
			Password = providedSecretKey,
			Id = base.DatabaseRow.Id,
			Name = providedArn
		};
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		amazonS3LayoutControl.Root.Remove(timeoutLayoutControlItem);
		_ = base.SelectedDatabaseType.HasValue;
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		return true & ValidateArn() & ValidateAccessKey() & ValidateSecretKey();
	}

	private bool ValidateArn(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(amazonS3ArnComboBoxEdit, addDBErrorProvider, "arn", acceptEmptyValue);
	}

	private bool ValidateSecretKey(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(amazonS3SecretKeyTextEdit, addDBErrorProvider, "secretKey", acceptEmptyValue);
	}

	private bool ValidateAccessKey(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(amazonS3AccesKeyTextEdit, addDBErrorProvider, "accesKey", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		amazonS3ArnComboBoxEdit.Text = base.DatabaseRow.Host;
		amazonS3AccesKeyTextEdit.Text = base.DatabaseRow.User;
		amazonS3SecretKeyTextEdit.Text = base.DatabaseRow.Password;
		amazonS3SaveSecretKeyCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return providedArn + "@AmazonS3";
	}

	private void amazonS3AccesKeyTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateAccessKey(acceptEmptyValue: true);
	}

	private void amazonS3SecretKeyTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSecretKey();
	}

	private void amazonS3ArnComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void amazonS3ArnComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		amazonS3AccesKeyTextEdit.Text = string.Empty;
		amazonS3SecretKeyTextEdit.Text = string.Empty;
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
		this.amazonS3LayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.amazonS3AccesKeyTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.amazonS3SecretKeyTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.amazonS3SaveSecretKeyCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.amazonS3ArnComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.amazonS3AccessKeyLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.amazonS3SecretKeyLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.amazonS3SaveSecretKeyLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem34 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem35 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem38 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.amazonS3ArnLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem45 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.configureEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.amazonS3LayoutControl).BeginInit();
		this.amazonS3LayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.amazonS3AccesKeyTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.amazonS3SecretKeyTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.amazonS3SaveSecretKeyCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.amazonS3ArnComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.amazonS3AccessKeyLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.amazonS3SecretKeyLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.amazonS3SaveSecretKeyLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem34).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem35).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem38).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.amazonS3ArnLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem45).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.configureEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.amazonS3LayoutControl.AllowCustomization = false;
		this.amazonS3LayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.amazonS3LayoutControl.Controls.Add(this.amazonS3AccesKeyTextEdit);
		this.amazonS3LayoutControl.Controls.Add(this.amazonS3SecretKeyTextEdit);
		this.amazonS3LayoutControl.Controls.Add(this.amazonS3SaveSecretKeyCheckEdit);
		this.amazonS3LayoutControl.Controls.Add(this.amazonS3ArnComboBoxEdit);
		this.amazonS3LayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.amazonS3LayoutControl.Location = new System.Drawing.Point(0, 0);
		this.amazonS3LayoutControl.Name = "amazonS3LayoutControl";
		this.amazonS3LayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2548, 234, 360, 525);
		this.amazonS3LayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.amazonS3LayoutControl.Root = this.layoutControlGroup6;
		this.amazonS3LayoutControl.Size = new System.Drawing.Size(492, 340);
		this.amazonS3LayoutControl.TabIndex = 3;
		this.amazonS3LayoutControl.Text = "layoutControl1";
		this.amazonS3AccesKeyTextEdit.Location = new System.Drawing.Point(105, 24);
		this.amazonS3AccesKeyTextEdit.Name = "amazonS3AccesKeyTextEdit";
		this.amazonS3AccesKeyTextEdit.Size = new System.Drawing.Size(230, 20);
		this.amazonS3AccesKeyTextEdit.StyleController = this.amazonS3LayoutControl;
		this.amazonS3AccesKeyTextEdit.TabIndex = 1;
		this.amazonS3SecretKeyTextEdit.Location = new System.Drawing.Point(105, 48);
		this.amazonS3SecretKeyTextEdit.Name = "amazonS3SecretKeyTextEdit";
		this.amazonS3SecretKeyTextEdit.Properties.UseSystemPasswordChar = true;
		this.amazonS3SecretKeyTextEdit.Size = new System.Drawing.Size(230, 20);
		this.amazonS3SecretKeyTextEdit.StyleController = this.amazonS3LayoutControl;
		this.amazonS3SecretKeyTextEdit.TabIndex = 2;
		this.amazonS3SaveSecretKeyCheckEdit.Location = new System.Drawing.Point(105, 72);
		this.amazonS3SaveSecretKeyCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.amazonS3SaveSecretKeyCheckEdit.Name = "amazonS3SaveSecretKeyCheckEdit";
		this.amazonS3SaveSecretKeyCheckEdit.Properties.Caption = "Save password";
		this.amazonS3SaveSecretKeyCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.amazonS3SaveSecretKeyCheckEdit.StyleController = this.amazonS3LayoutControl;
		this.amazonS3SaveSecretKeyCheckEdit.TabIndex = 3;
		this.amazonS3ArnComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.amazonS3ArnComboBoxEdit.Margin = new System.Windows.Forms.Padding(2);
		this.amazonS3ArnComboBoxEdit.Name = "amazonS3ArnComboBoxEdit";
		this.amazonS3ArnComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.amazonS3ArnComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.amazonS3ArnComboBoxEdit.StyleController = this.amazonS3LayoutControl;
		this.amazonS3ArnComboBoxEdit.TabIndex = 1;
		this.layoutControlGroup6.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup6.GroupBordersVisible = false;
		this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[9] { this.amazonS3AccessKeyLayoutControlItem, this.amazonS3SecretKeyLayoutControlItem, this.amazonS3SaveSecretKeyLayoutControlItem, this.emptySpaceItem34, this.emptySpaceItem35, this.emptySpaceItem38, this.amazonS3ArnLayoutControlItem, this.emptySpaceItem45, this.configureEmptySpaceItem });
		this.layoutControlGroup6.Name = "Root";
		this.layoutControlGroup6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup6.Size = new System.Drawing.Size(492, 340);
		this.layoutControlGroup6.TextVisible = false;
		this.amazonS3AccessKeyLayoutControlItem.Control = this.amazonS3AccesKeyTextEdit;
		this.amazonS3AccessKeyLayoutControlItem.CustomizationFormText = "User:";
		this.amazonS3AccessKeyLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.amazonS3AccessKeyLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.amazonS3AccessKeyLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.amazonS3AccessKeyLayoutControlItem.Name = "amazonS3AccessKeyLayoutControlItem";
		this.amazonS3AccessKeyLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.amazonS3AccessKeyLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.amazonS3AccessKeyLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.amazonS3AccessKeyLayoutControlItem.Text = "Access Key:";
		this.amazonS3AccessKeyLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.amazonS3AccessKeyLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.amazonS3AccessKeyLayoutControlItem.TextToControlDistance = 5;
		this.amazonS3SecretKeyLayoutControlItem.Control = this.amazonS3SecretKeyTextEdit;
		this.amazonS3SecretKeyLayoutControlItem.CustomizationFormText = "Password";
		this.amazonS3SecretKeyLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.amazonS3SecretKeyLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.amazonS3SecretKeyLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.amazonS3SecretKeyLayoutControlItem.Name = "amazonS3SecretKeyLayoutControlItem";
		this.amazonS3SecretKeyLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.amazonS3SecretKeyLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.amazonS3SecretKeyLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.amazonS3SecretKeyLayoutControlItem.Text = "Secret Key:";
		this.amazonS3SecretKeyLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.amazonS3SecretKeyLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.amazonS3SecretKeyLayoutControlItem.TextToControlDistance = 5;
		this.amazonS3SaveSecretKeyLayoutControlItem.Control = this.amazonS3SaveSecretKeyCheckEdit;
		this.amazonS3SaveSecretKeyLayoutControlItem.CustomizationFormText = "layoutControlItem18";
		this.amazonS3SaveSecretKeyLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.amazonS3SaveSecretKeyLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.amazonS3SaveSecretKeyLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.amazonS3SaveSecretKeyLayoutControlItem.Name = "amazonS3SaveSecretKeyLayoutControlItem";
		this.amazonS3SaveSecretKeyLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.amazonS3SaveSecretKeyLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.amazonS3SaveSecretKeyLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.amazonS3SaveSecretKeyLayoutControlItem.Text = " ";
		this.amazonS3SaveSecretKeyLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.amazonS3SaveSecretKeyLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.amazonS3SaveSecretKeyLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem34.AllowHotTrack = false;
		this.emptySpaceItem34.Location = new System.Drawing.Point(0, 96);
		this.emptySpaceItem34.Name = "emptySpaceItem3";
		this.emptySpaceItem34.Size = new System.Drawing.Size(492, 244);
		this.emptySpaceItem34.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem35.AllowHotTrack = false;
		this.emptySpaceItem35.CustomizationFormText = "emptySpaceItem13";
		this.emptySpaceItem35.Location = new System.Drawing.Point(335, 24);
		this.emptySpaceItem35.Name = "emptySpaceItem19";
		this.emptySpaceItem35.Size = new System.Drawing.Size(157, 24);
		this.emptySpaceItem35.Text = "emptySpaceItem13";
		this.emptySpaceItem35.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem38.AllowHotTrack = false;
		this.emptySpaceItem38.CustomizationFormText = "emptySpaceItem13";
		this.emptySpaceItem38.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem38.Name = "emptySpaceItem27";
		this.emptySpaceItem38.Size = new System.Drawing.Size(157, 24);
		this.emptySpaceItem38.Text = "emptySpaceItem13";
		this.emptySpaceItem38.TextSize = new System.Drawing.Size(0, 0);
		this.amazonS3ArnLayoutControlItem.Control = this.amazonS3ArnComboBoxEdit;
		this.amazonS3ArnLayoutControlItem.CustomizationFormText = "Host:";
		this.amazonS3ArnLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.amazonS3ArnLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.amazonS3ArnLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.amazonS3ArnLayoutControlItem.Name = "amazonS3ArnLayoutControlItem";
		this.amazonS3ArnLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.amazonS3ArnLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.amazonS3ArnLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.amazonS3ArnLayoutControlItem.Text = "ARN:";
		this.amazonS3ArnLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.amazonS3ArnLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.amazonS3ArnLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem45.AllowHotTrack = false;
		this.emptySpaceItem45.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem45.Name = "emptySpaceItem45";
		this.emptySpaceItem45.Size = new System.Drawing.Size(157, 24);
		this.emptySpaceItem45.TextSize = new System.Drawing.Size(0, 0);
		this.configureEmptySpaceItem.AllowHotTrack = false;
		this.configureEmptySpaceItem.Location = new System.Drawing.Point(335, 72);
		this.configureEmptySpaceItem.Name = "configureEmptySpaceItem";
		this.configureEmptySpaceItem.Size = new System.Drawing.Size(157, 24);
		this.configureEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.amazonS3LayoutControl);
		base.Name = "AmazonS3ConnectorControl";
		base.Size = new System.Drawing.Size(492, 340);
		((System.ComponentModel.ISupportInitialize)this.amazonS3LayoutControl).EndInit();
		this.amazonS3LayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.amazonS3AccesKeyTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.amazonS3SecretKeyTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.amazonS3SaveSecretKeyCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.amazonS3ArnComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.amazonS3AccessKeyLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.amazonS3SecretKeyLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.amazonS3SaveSecretKeyLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem34).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem35).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem38).EndInit();
		((System.ComponentModel.ISupportInitialize)this.amazonS3ArnLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem45).EndInit();
		((System.ComponentModel.ISupportInitialize)this.configureEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
	}
}
