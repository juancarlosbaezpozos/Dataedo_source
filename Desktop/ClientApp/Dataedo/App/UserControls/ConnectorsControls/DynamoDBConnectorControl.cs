using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Amazon;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classes;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class DynamoDBConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl dynamoDBLayoutControl;

	private LayoutControlGroup Root;

	private TextEdit dynamoDBAccessKeyTextEdit;

	private LayoutControlItem dynamoDBAccessKeyLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem2;

	private TextEdit dynamoDBSecretKeyTextEdit;

	private LayoutControlItem dynamoDBSecretKeyLayoutControlItem;

	private CheckEdit dynamoDBSavePasswordCheckEdit;

	private LayoutControlItem dynamoDBSavePasswordLayoutControlItem;

	private TextEdit dynamoDBSampleSizeTextEdit;

	private LayoutControlItem dynamoDBSampleSizeLayoutControlItem;

	private ComboBoxEdit dynamoDBAWSRegionComboBoxEdit;

	private LayoutControlItem dynamoDBAWSRegionLayoutControlItem;

	private EmptySpaceItem emptySpaceItem3;

	private string providedAccessKey => dynamoDBAccessKeyTextEdit.Text;

	private string providedSecretKey => dynamoDBSecretKeyTextEdit.Text;

	private string providedSampleSize => dynamoDBSampleSizeTextEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.DynamoDB;

	protected override CheckEdit SavePasswordCheckEdit => dynamoDBSavePasswordCheckEdit;

	public DynamoDBConnectorControl()
	{
		InitializeComponent();
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		SetAWSRegionsLookupEdit();
		dynamoDBSampleSizeTextEdit.Text = "500";
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
	}

	private void SetAWSRegionsLookupEdit()
	{
		foreach (RegionEndpoint enumerableAllRegion in RegionEndpoint.EnumerableAllRegions)
		{
			dynamoDBAWSRegionComboBoxEdit.Properties.Items.Add(enumerableAllRegion);
		}
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, ((RegionEndpoint)dynamoDBAWSRegionComboBoxEdit.EditValue).SystemName, documentationTitle, ((RegionEndpoint)dynamoDBAWSRegionComboBoxEdit.EditValue).SystemName, providedAccessKey, providedSecretKey, null, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion)
		{
			Param1 = providedSampleSize
		};
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		return true & ValidateAccessKey() & ValidateSecretKey() & ValidateSampleSize() & ValidateAWSRegion();
	}

	private bool ValidateAccessKey(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(dynamoDBAccessKeyTextEdit, addDBErrorProvider, "access key", acceptEmptyValue);
	}

	private bool ValidateSecretKey(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(dynamoDBSecretKeyTextEdit, addDBErrorProvider, "secret key", acceptEmptyValue);
	}

	private bool ValidateSampleSize(bool acceptEmptyValue = false)
	{
		string text = dynamoDBSampleSizeTextEdit.Text;
		string text2 = "sample size";
		if (!acceptEmptyValue && text == null && string.IsNullOrEmpty(text.Trim()))
		{
			addDBErrorProvider.SetError(dynamoDBSampleSizeTextEdit, "The " + text2 + " is required!");
			return false;
		}
		if (!int.TryParse(text, out var result))
		{
			addDBErrorProvider.SetError(dynamoDBSampleSizeTextEdit, "The " + text2 + " must be a numeric value!");
			return false;
		}
		if (result <= 0)
		{
			addDBErrorProvider.SetError(dynamoDBSampleSizeTextEdit, "The " + text2 + " must be a numeric value bigger than 0!");
			return false;
		}
		return true;
	}

	private bool ValidateAWSRegion(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(dynamoDBAWSRegionComboBoxEdit, addDBErrorProvider, "AWS region", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		dynamoDBAWSRegionComboBoxEdit.SelectedIndex = GetRegionEndpointIndex(base.DatabaseRow.Host);
		dynamoDBAccessKeyTextEdit.Text = base.DatabaseRow.User;
		dynamoDBSecretKeyTextEdit.Text = base.DatabaseRow.Password;
		dynamoDBSavePasswordCheckEdit.Checked = !string.IsNullOrEmpty(value);
		dynamoDBSampleSizeTextEdit.Text = base.DatabaseRow.Param1;
	}

	protected override string GetPanelDocumentationTitle()
	{
		return ((RegionEndpoint)dynamoDBAWSRegionComboBoxEdit.EditValue).SystemName + "@DynamoDB";
	}

	private void dynamoDBAccessKeyTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateAccessKey(acceptEmptyValue: true);
	}

	private void dynamoDBSecretKeyTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSecretKey(acceptEmptyValue: true);
	}

	private void dynamoDBSampleSizeTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSampleSize(acceptEmptyValue: true);
	}

	private void dynamoDBAWSRegionComboBoxEdit_SelectedIndexChanged(object sender, EventArgs e)
	{
		ValidateAWSRegion(acceptEmptyValue: true);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		dynamoDBAccessKeyTextEdit.Text = string.Empty;
		dynamoDBSecretKeyTextEdit.Text = string.Empty;
	}

	private int GetRegionEndpointIndex(string regionEndpointSystemName)
	{
		if (string.IsNullOrEmpty(regionEndpointSystemName))
		{
			return -1;
		}
		return dynamoDBAWSRegionComboBoxEdit.Properties.Items.IndexOf(RegionEndpoint.GetBySystemName(base.DatabaseRow.Host));
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
		this.dynamoDBLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.dynamoDBAWSRegionComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.dynamoDBAccessKeyTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.dynamoDBSecretKeyTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.dynamoDBSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.dynamoDBSampleSizeTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.dynamoDBAccessKeyLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.dynamoDBSecretKeyLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.dynamoDBSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.dynamoDBSampleSizeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.dynamoDBAWSRegionLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBLayoutControl).BeginInit();
		this.dynamoDBLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBAWSRegionComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBAccessKeyTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBSecretKeyTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBSampleSizeTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBAccessKeyLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBSecretKeyLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBSampleSizeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBAWSRegionLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		base.SuspendLayout();
		this.dynamoDBLayoutControl.AllowCustomization = false;
		this.dynamoDBLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.dynamoDBLayoutControl.Controls.Add(this.dynamoDBAWSRegionComboBoxEdit);
		this.dynamoDBLayoutControl.Controls.Add(this.dynamoDBAccessKeyTextEdit);
		this.dynamoDBLayoutControl.Controls.Add(this.dynamoDBSecretKeyTextEdit);
		this.dynamoDBLayoutControl.Controls.Add(this.dynamoDBSavePasswordCheckEdit);
		this.dynamoDBLayoutControl.Controls.Add(this.dynamoDBSampleSizeTextEdit);
		this.dynamoDBLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dynamoDBLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.dynamoDBLayoutControl.Name = "dynamoDBLayoutControl";
		this.dynamoDBLayoutControl.OptionsPrint.AppearanceGroupCaption.BackColor = System.Drawing.Color.LightGray;
		this.dynamoDBLayoutControl.OptionsPrint.AppearanceGroupCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25f);
		this.dynamoDBLayoutControl.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
		this.dynamoDBLayoutControl.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
		this.dynamoDBLayoutControl.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
		this.dynamoDBLayoutControl.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.dynamoDBLayoutControl.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.dynamoDBLayoutControl.Root = this.Root;
		this.dynamoDBLayoutControl.Size = new System.Drawing.Size(490, 340);
		this.dynamoDBLayoutControl.TabIndex = 0;
		this.dynamoDBLayoutControl.Text = "dynamoDBLayoutControl";
		this.dynamoDBAWSRegionComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.dynamoDBAWSRegionComboBoxEdit.Name = "dynamoDBAWSRegionComboBoxEdit";
		this.dynamoDBAWSRegionComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dynamoDBAWSRegionComboBoxEdit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.dynamoDBAWSRegionComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.dynamoDBAWSRegionComboBoxEdit.StyleController = this.dynamoDBLayoutControl;
		this.dynamoDBAWSRegionComboBoxEdit.TabIndex = 6;
		this.dynamoDBAWSRegionComboBoxEdit.SelectedIndexChanged += new System.EventHandler(dynamoDBAWSRegionComboBoxEdit_SelectedIndexChanged);
		this.dynamoDBAccessKeyTextEdit.Location = new System.Drawing.Point(105, 24);
		this.dynamoDBAccessKeyTextEdit.Margin = new System.Windows.Forms.Padding(2);
		this.dynamoDBAccessKeyTextEdit.Name = "dynamoDBAccessKeyTextEdit";
		this.dynamoDBAccessKeyTextEdit.Size = new System.Drawing.Size(230, 20);
		this.dynamoDBAccessKeyTextEdit.StyleController = this.dynamoDBLayoutControl;
		this.dynamoDBAccessKeyTextEdit.TabIndex = 1;
		this.dynamoDBAccessKeyTextEdit.EditValueChanged += new System.EventHandler(dynamoDBAccessKeyTextEdit_EditValueChanged);
		this.dynamoDBSecretKeyTextEdit.Location = new System.Drawing.Point(105, 48);
		this.dynamoDBSecretKeyTextEdit.Name = "dynamoDBSecretKeyTextEdit";
		this.dynamoDBSecretKeyTextEdit.Properties.UseSystemPasswordChar = true;
		this.dynamoDBSecretKeyTextEdit.Size = new System.Drawing.Size(230, 20);
		this.dynamoDBSecretKeyTextEdit.StyleController = this.dynamoDBLayoutControl;
		this.dynamoDBSecretKeyTextEdit.TabIndex = 4;
		this.dynamoDBSecretKeyTextEdit.EditValueChanged += new System.EventHandler(dynamoDBSecretKeyTextEdit_EditValueChanged);
		this.dynamoDBSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 72);
		this.dynamoDBSavePasswordCheckEdit.Name = "dynamoDBSavePasswordCheckEdit";
		this.dynamoDBSavePasswordCheckEdit.Properties.Caption = "Save secret key";
		this.dynamoDBSavePasswordCheckEdit.Size = new System.Drawing.Size(230, 20);
		this.dynamoDBSavePasswordCheckEdit.StyleController = this.dynamoDBLayoutControl;
		this.dynamoDBSavePasswordCheckEdit.TabIndex = 3;
		this.dynamoDBSampleSizeTextEdit.Location = new System.Drawing.Point(105, 96);
		this.dynamoDBSampleSizeTextEdit.Name = "dynamoDBSampleSizeTextEdit";
		this.dynamoDBSampleSizeTextEdit.Properties.Mask.EditMask = "d";
		this.dynamoDBSampleSizeTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
		this.dynamoDBSampleSizeTextEdit.Size = new System.Drawing.Size(45, 20);
		this.dynamoDBSampleSizeTextEdit.StyleController = this.dynamoDBLayoutControl;
		this.dynamoDBSampleSizeTextEdit.TabIndex = 5;
		this.dynamoDBSampleSizeTextEdit.EditValueChanged += new System.EventHandler(dynamoDBSampleSizeTextEdit_EditValueChanged);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[8] { this.dynamoDBAccessKeyLayoutControlItem, this.emptySpaceItem1, this.emptySpaceItem2, this.dynamoDBSecretKeyLayoutControlItem, this.dynamoDBSavePasswordLayoutControlItem, this.dynamoDBSampleSizeLayoutControlItem, this.dynamoDBAWSRegionLayoutControlItem, this.emptySpaceItem3 });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(490, 340);
		this.Root.TextVisible = false;
		this.dynamoDBAccessKeyLayoutControlItem.Control = this.dynamoDBAccessKeyTextEdit;
		this.dynamoDBAccessKeyLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.dynamoDBAccessKeyLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.dynamoDBAccessKeyLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.dynamoDBAccessKeyLayoutControlItem.Name = "dynamoDBAccessKeyLayoutControlItem";
		this.dynamoDBAccessKeyLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.dynamoDBAccessKeyLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.dynamoDBAccessKeyLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.dynamoDBAccessKeyLayoutControlItem.Text = "Access Key:";
		this.dynamoDBAccessKeyLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.dynamoDBAccessKeyLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.dynamoDBAccessKeyLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 120);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(490, 220);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(155, 120);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.dynamoDBSecretKeyLayoutControlItem.Control = this.dynamoDBSecretKeyTextEdit;
		this.dynamoDBSecretKeyLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.dynamoDBSecretKeyLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.dynamoDBSecretKeyLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.dynamoDBSecretKeyLayoutControlItem.Name = "dynamoDBSecretKeyLayoutControlItem";
		this.dynamoDBSecretKeyLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.dynamoDBSecretKeyLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.dynamoDBSecretKeyLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.dynamoDBSecretKeyLayoutControlItem.Text = "Secret Key:";
		this.dynamoDBSecretKeyLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.dynamoDBSecretKeyLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.dynamoDBSecretKeyLayoutControlItem.TextToControlDistance = 5;
		this.dynamoDBSavePasswordLayoutControlItem.Control = this.dynamoDBSavePasswordCheckEdit;
		this.dynamoDBSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.dynamoDBSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.dynamoDBSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.dynamoDBSavePasswordLayoutControlItem.Name = "dynamoDBSavePasswordLayoutControlItem";
		this.dynamoDBSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.dynamoDBSavePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.dynamoDBSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.dynamoDBSavePasswordLayoutControlItem.Text = " ";
		this.dynamoDBSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.dynamoDBSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.dynamoDBSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.dynamoDBSampleSizeLayoutControlItem.Control = this.dynamoDBSampleSizeTextEdit;
		this.dynamoDBSampleSizeLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.dynamoDBSampleSizeLayoutControlItem.MaxSize = new System.Drawing.Size(150, 24);
		this.dynamoDBSampleSizeLayoutControlItem.MinSize = new System.Drawing.Size(150, 24);
		this.dynamoDBSampleSizeLayoutControlItem.Name = "dynamoDBSampleSizeLayoutControlItem";
		this.dynamoDBSampleSizeLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.dynamoDBSampleSizeLayoutControlItem.Size = new System.Drawing.Size(150, 24);
		this.dynamoDBSampleSizeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.dynamoDBSampleSizeLayoutControlItem.Text = "Sample size:";
		this.dynamoDBSampleSizeLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.dynamoDBSampleSizeLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.dynamoDBSampleSizeLayoutControlItem.TextToControlDistance = 5;
		this.dynamoDBAWSRegionLayoutControlItem.Control = this.dynamoDBAWSRegionComboBoxEdit;
		this.dynamoDBAWSRegionLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.dynamoDBAWSRegionLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.dynamoDBAWSRegionLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.dynamoDBAWSRegionLayoutControlItem.Name = "dynamoDBAWSRegionLayoutControlItem";
		this.dynamoDBAWSRegionLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.dynamoDBAWSRegionLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.dynamoDBAWSRegionLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.dynamoDBAWSRegionLayoutControlItem.Text = "AWS Region:";
		this.dynamoDBAWSRegionLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.dynamoDBAWSRegionLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.dynamoDBAWSRegionLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(150, 96);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(185, 24);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.dynamoDBLayoutControl);
		base.Margin = new System.Windows.Forms.Padding(0);
		base.Name = "DynamoDBConnectorControl";
		base.Size = new System.Drawing.Size(490, 340);
		((System.ComponentModel.ISupportInitialize)this.dynamoDBLayoutControl).EndInit();
		this.dynamoDBLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dynamoDBAWSRegionComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBAccessKeyTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBSecretKeyTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBSampleSizeTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBAccessKeyLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBSecretKeyLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBSampleSizeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dynamoDBAWSRegionLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		base.ResumeLayout(false);
	}
}
