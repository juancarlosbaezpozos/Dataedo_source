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

public class AstraConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl cassandraLayoutControl;

	private TextEdit passwordTextEdit;

	private TextEdit userTextEdit;

	private CheckEdit passwordCheckEdit;

	private LayoutControlGroup cassandraLayoutControlGroup;

	private EmptySpaceItem cassandraEmptySpaceItem;

	private LayoutControlItem cassandraUserLayoutControlItem;

	private LayoutControlItem cassandraPasswordLayoutControlItem;

	private EmptySpaceItem cassandraHostLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem cassandraUserLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem cassandraPasswordLayoutControlItemEmptySpaceItem;

	private LayoutControlItem cassandraSavePasswordLayoutControlItem;

	private EmptySpaceItem cassandraSavePasswordLayoutControlItemEmptySpaceItem;

	private ButtonEdit fileButtonEdit;

	private LayoutControlItem fileLayoutControl;

	private ButtonEdit databaseButtonEdit;

	private LayoutControlItem cassandraDatabaseLayoutControlItem;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Astra;

	protected override CheckEdit SavePasswordCheckEdit => passwordCheckEdit;

	public AstraConnectorControl()
	{
		InitializeComponent();
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? databaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, fileButtonEdit.Text, userTextEdit.Text, passwordTextEdit.Text, null, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion);
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		cassandraLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			cassandraLayoutControl.Root.AddItem(timeoutLayoutControlItem, cassandraEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateAstraUser();
		flag &= ValidateAstraPassword();
		flag &= ValidateAstraSecureBundle();
		if (!testForGettingDatabasesList)
		{
			flag &= ValidateAstraDatabase();
		}
		return flag;
	}

	private bool ValidateAstraSecureBundle(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(fileButtonEdit, addDBErrorProvider, "secrure connection bundle", acceptEmptyValue);
	}

	private bool ValidateAstraDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(databaseButtonEdit, addDBErrorProvider, "keyspace", acceptEmptyValue);
	}

	private bool ValidateAstraUser(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(userTextEdit, addDBErrorProvider, "client key", acceptEmptyValue);
	}

	private bool ValidateAstraPassword(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(passwordTextEdit, addDBErrorProvider, "secret key", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		databaseButtonEdit.Text = base.DatabaseRow.Name;
		userTextEdit.Text = base.DatabaseRow.User;
		fileButtonEdit.Text = base.DatabaseRow.Host;
		passwordTextEdit.Text = base.DatabaseRow.Password;
		passwordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return databaseButtonEdit.Text + "@" + fileButtonEdit.Text;
	}

	protected override void ClearPanelLoginAndPassword()
	{
		userTextEdit.Text = string.Empty;
		passwordTextEdit.Text = string.Empty;
	}

	private void fileButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		string empty = string.Empty;
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "zip files (*.zip)|*.zip|All files (*.*)|*.*";
		openFileDialog.FilterIndex = 1;
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			empty = openFileDialog.FileName;
			fileButtonEdit.Text = empty;
		}
	}

	private void databaseButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		DatabaseButtonEdit_ButtonClick(sender, e);
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
		this.cassandraLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.passwordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.userTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.passwordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.fileButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.databaseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.cassandraLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.cassandraEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.cassandraUserLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cassandraPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cassandraHostLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.cassandraUserLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.cassandraPasswordLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.cassandraSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cassandraSavePasswordLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.fileLayoutControl = new DevExpress.XtraLayout.LayoutControlItem();
		this.cassandraDatabaseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.cassandraLayoutControl).BeginInit();
		this.cassandraLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.passwordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.userTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.passwordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fileButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraUserLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraHostLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraUserLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPasswordLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraSavePasswordLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fileLayoutControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraDatabaseLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.cassandraLayoutControl.AllowCustomization = false;
		this.cassandraLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.cassandraLayoutControl.Controls.Add(this.passwordTextEdit);
		this.cassandraLayoutControl.Controls.Add(this.userTextEdit);
		this.cassandraLayoutControl.Controls.Add(this.passwordCheckEdit);
		this.cassandraLayoutControl.Controls.Add(this.fileButtonEdit);
		this.cassandraLayoutControl.Controls.Add(this.databaseButtonEdit);
		this.cassandraLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.cassandraLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.cassandraLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.cassandraLayoutControl.Name = "cassandraLayoutControl";
		this.cassandraLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3451, 236, 800, 771);
		this.cassandraLayoutControl.Root = this.cassandraLayoutControlGroup;
		this.cassandraLayoutControl.Size = new System.Drawing.Size(492, 239);
		this.cassandraLayoutControl.TabIndex = 3;
		this.cassandraLayoutControl.Text = "layoutControl3";
		this.passwordTextEdit.Location = new System.Drawing.Point(105, 48);
		this.passwordTextEdit.Margin = new System.Windows.Forms.Padding(2);
		this.passwordTextEdit.Name = "passwordTextEdit";
		this.passwordTextEdit.Properties.UseSystemPasswordChar = true;
		this.passwordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.passwordTextEdit.StyleController = this.cassandraLayoutControl;
		this.passwordTextEdit.TabIndex = 6;
		this.userTextEdit.Location = new System.Drawing.Point(105, 24);
		this.userTextEdit.Margin = new System.Windows.Forms.Padding(2);
		this.userTextEdit.Name = "userTextEdit";
		this.userTextEdit.Size = new System.Drawing.Size(230, 20);
		this.userTextEdit.StyleController = this.cassandraLayoutControl;
		this.userTextEdit.TabIndex = 5;
		this.passwordCheckEdit.Location = new System.Drawing.Point(105, 72);
		this.passwordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.passwordCheckEdit.Name = "passwordCheckEdit";
		this.passwordCheckEdit.Properties.Caption = "Save password";
		this.passwordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.passwordCheckEdit.StyleController = this.cassandraLayoutControl;
		this.passwordCheckEdit.TabIndex = 3;
		this.fileButtonEdit.Location = new System.Drawing.Point(105, 0);
		this.fileButtonEdit.Name = "fileButtonEdit";
		this.fileButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.fileButtonEdit.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(fileButtonEdit_ButtonClick);
		this.fileButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.fileButtonEdit.StyleController = this.cassandraLayoutControl;
		this.fileButtonEdit.TabIndex = 9;
		this.databaseButtonEdit.Location = new System.Drawing.Point(105, 96);
		this.databaseButtonEdit.Name = "databaseButtonEdit";
		this.databaseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.databaseButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.databaseButtonEdit.StyleController = this.cassandraLayoutControl;
		this.databaseButtonEdit.TabIndex = 10;
		this.databaseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(databaseButtonEdit_ButtonClick);
		this.cassandraLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.cassandraLayoutControlGroup.GroupBordersVisible = false;
		this.cassandraLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[10] { this.cassandraEmptySpaceItem, this.cassandraUserLayoutControlItem, this.cassandraPasswordLayoutControlItem, this.cassandraHostLayoutControlItemEmptySpaceItem, this.cassandraUserLayoutControlItemEmptySpaceItem, this.cassandraPasswordLayoutControlItemEmptySpaceItem, this.cassandraSavePasswordLayoutControlItem, this.cassandraSavePasswordLayoutControlItemEmptySpaceItem, this.fileLayoutControl, this.cassandraDatabaseLayoutControlItem });
		this.cassandraLayoutControlGroup.Name = "Root";
		this.cassandraLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.cassandraLayoutControlGroup.Size = new System.Drawing.Size(492, 239);
		this.cassandraLayoutControlGroup.TextVisible = false;
		this.cassandraEmptySpaceItem.AllowHotTrack = false;
		this.cassandraEmptySpaceItem.Location = new System.Drawing.Point(0, 120);
		this.cassandraEmptySpaceItem.Name = "cassandraEmptySpaceItem";
		this.cassandraEmptySpaceItem.Size = new System.Drawing.Size(492, 119);
		this.cassandraEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.cassandraUserLayoutControlItem.Control = this.userTextEdit;
		this.cassandraUserLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.cassandraUserLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.cassandraUserLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.cassandraUserLayoutControlItem.Name = "cassandraUserLayoutControlItem";
		this.cassandraUserLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.cassandraUserLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.cassandraUserLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cassandraUserLayoutControlItem.Text = "Client key:";
		this.cassandraUserLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.cassandraUserLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.cassandraUserLayoutControlItem.TextToControlDistance = 5;
		this.cassandraPasswordLayoutControlItem.Control = this.passwordTextEdit;
		this.cassandraPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.cassandraPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.cassandraPasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.cassandraPasswordLayoutControlItem.Name = "cassandraPasswordLayoutControlItem";
		this.cassandraPasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.cassandraPasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.cassandraPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cassandraPasswordLayoutControlItem.Text = "Secret key:";
		this.cassandraPasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.cassandraPasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.cassandraPasswordLayoutControlItem.TextToControlDistance = 5;
		this.cassandraHostLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.cassandraHostLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 0);
		this.cassandraHostLayoutControlItemEmptySpaceItem.Name = "cassandraHostLayoutControlItemEmptySpaceItem";
		this.cassandraHostLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(157, 24);
		this.cassandraHostLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.cassandraUserLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.cassandraUserLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 24);
		this.cassandraUserLayoutControlItemEmptySpaceItem.Name = "cassandraUserLayoutControlItemEmptySpaceItem";
		this.cassandraUserLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(157, 24);
		this.cassandraUserLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.cassandraPasswordLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.cassandraPasswordLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 48);
		this.cassandraPasswordLayoutControlItemEmptySpaceItem.Name = "cassandraPasswordLayoutControlItemEmptySpaceItem";
		this.cassandraPasswordLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(157, 24);
		this.cassandraPasswordLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.cassandraSavePasswordLayoutControlItem.Control = this.passwordCheckEdit;
		this.cassandraSavePasswordLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.cassandraSavePasswordLayoutControlItem.CustomizationFormText = "mongoDbSavePasswordLayoutControlItem";
		this.cassandraSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.cassandraSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.cassandraSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.cassandraSavePasswordLayoutControlItem.Name = "cassandraSavePasswordLayoutControlItem";
		this.cassandraSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.cassandraSavePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.cassandraSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cassandraSavePasswordLayoutControlItem.Text = " ";
		this.cassandraSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.cassandraSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.cassandraSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.cassandraSavePasswordLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.cassandraSavePasswordLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 72);
		this.cassandraSavePasswordLayoutControlItemEmptySpaceItem.Name = "cassandraSavePasswordLayoutControlItemEmptySpaceItem";
		this.cassandraSavePasswordLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(157, 24);
		this.cassandraSavePasswordLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.fileLayoutControl.Control = this.fileButtonEdit;
		this.fileLayoutControl.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.fileLayoutControl.CustomizationFormText = "Connection bundle:";
		this.fileLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.fileLayoutControl.MaxSize = new System.Drawing.Size(335, 24);
		this.fileLayoutControl.MinSize = new System.Drawing.Size(335, 24);
		this.fileLayoutControl.Name = "fileLayoutControl";
		this.fileLayoutControl.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.fileLayoutControl.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.fileLayoutControl.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.fileLayoutControl.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.fileLayoutControl.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.fileLayoutControl.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.fileLayoutControl.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.fileLayoutControl.Size = new System.Drawing.Size(335, 24);
		this.fileLayoutControl.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.fileLayoutControl.Text = "Connection bundle:";
		this.fileLayoutControl.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.fileLayoutControl.TextSize = new System.Drawing.Size(100, 13);
		this.fileLayoutControl.TextToControlDistance = 5;
		this.cassandraDatabaseLayoutControlItem.Control = this.databaseButtonEdit;
		this.cassandraDatabaseLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.cassandraDatabaseLayoutControlItem.CustomizationFormText = "Keyspace";
		this.cassandraDatabaseLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.cassandraDatabaseLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.cassandraDatabaseLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.cassandraDatabaseLayoutControlItem.Name = "cassandraDatabaseLayoutControlItem";
		this.cassandraDatabaseLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.cassandraDatabaseLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.cassandraDatabaseLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.cassandraDatabaseLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.cassandraDatabaseLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.cassandraDatabaseLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.cassandraDatabaseLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.cassandraDatabaseLayoutControlItem.Size = new System.Drawing.Size(492, 24);
		this.cassandraDatabaseLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cassandraDatabaseLayoutControlItem.Text = "Keyspace";
		this.cassandraDatabaseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.cassandraDatabaseLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.cassandraDatabaseLayoutControlItem.TextToControlDistance = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.cassandraLayoutControl);
		base.Name = "AstraConnectorControl";
		base.Size = new System.Drawing.Size(492, 239);
		((System.ComponentModel.ISupportInitialize)this.cassandraLayoutControl).EndInit();
		this.cassandraLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.passwordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.userTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.passwordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fileButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraUserLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraHostLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraUserLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPasswordLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraSavePasswordLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fileLayoutControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraDatabaseLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
