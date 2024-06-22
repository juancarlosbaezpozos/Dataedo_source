using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
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

public class SapAseConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl sapAseLayoutControl;

	private LabelControl sapAseDefaultPortLabelControl;

	private ButtonEdit sapAseDatabaseButtonEdit;

	private TextEdit sapAsePortTextEdit;

	private TextEdit sapAseLoginTextEdit;

	private TextEdit sapAsePasswordTextEdit;

	private CheckEdit sapAseSavePasswordCheckEdit;

	private ComboBoxEdit sapAseHostComboBoxEdit;

	private LayoutControlGroup layoutControlGroup6;

	private LayoutControlItem sapAsePortLayoutControlItem;

	private LayoutControlItem sapAseLoginLayoutControlItem;

	private LayoutControlItem sapAsePasswordLayoutControlItem;

	private LayoutControlItem sapAseSavePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem34;

	private EmptySpaceItem emptySpaceItem35;

	private EmptySpaceItem emptySpaceItem36;

	private EmptySpaceItem emptySpaceItem37;

	private EmptySpaceItem emptySpaceItem38;

	private LayoutControlItem sapAseDatabaseLayoutControlItem;

	private LayoutControlItem layoutControlItem44;

	private EmptySpaceItem sapAseTimeoutEmptySpaceItem;

	private EmptySpaceItem emptySpaceItem48;

	private LayoutControlItem sapAseHostLayoutControlItem;

	private EmptySpaceItem emptySpaceItem45;

	private EmptySpaceItem configureEmptySpaceItem;

	private TextEdit sapAseCharsetTextEdit;

	private LayoutControlItem sapAseCharsetLayoutControlItem;

	private string providedSapAseHost => splittedHost?.Host ?? sapAseHostComboBoxEdit.Text;

	private string providedSapAsePort => sapAsePortTextEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.SapAse;

	protected override TextEdit HostTextEdit => sapAseHostComboBoxEdit;

	protected override TextEdit PortTextEdit => sapAsePortTextEdit;

	protected override ComboBoxEdit HostComboBoxEdit => sapAseHostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => sapAseSavePasswordCheckEdit;

	public SapAseConnectorControl()
	{
		InitializeComponent();
	}

	public override void SetSwitchDatabaseAvailability(bool isAvailable)
	{
		sapAseDatabaseButtonEdit.Enabled = isAvailable;
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new SapAseDatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? sapAseDatabaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedSapAseHost, sapAseLoginTextEdit.Text, sapAsePasswordTextEdit.Text, providedSapAsePort, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, sapAseCharsetTextEdit.Text);
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		sapAseLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			sapAseLayoutControl.Root.AddItem(timeoutLayoutControlItem, sapAseTimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateSapAseHost();
		flag &= ValidateSapAsePort();
		flag &= ValidateSapAseCharset();
		flag &= ValidateSapAseLogin();
		flag &= ValidateSapAsePassword();
		if (!testForGettingDatabasesList)
		{
			flag &= ValidateSapAseDatabase();
		}
		return flag;
	}

	private bool ValidateSapAseHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(sapAseHostComboBoxEdit, addDBErrorProvider, "host", acceptEmptyValue);
	}

	private bool ValidateSapAsePassword(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(sapAsePasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateSapAseLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(sapAseLoginTextEdit, addDBErrorProvider, "user", acceptEmptyValue);
	}

	private bool ValidateSapAsePort(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(sapAsePortTextEdit, addDBErrorProvider, "port", acceptEmptyValue);
	}

	private bool ValidateSapAseDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(sapAseDatabaseButtonEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	private bool ValidateSapAseCharset(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(sapAseCharsetTextEdit, addDBErrorProvider, "charset", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		sapAseHostComboBoxEdit.Text = base.DatabaseRow.Host;
		sapAseDatabaseButtonEdit.EditValue = base.DatabaseRow.Name;
		sapAsePortTextEdit.Text = PrepareValue.ToString(base.DatabaseRow.Port) ?? DatabaseTypeEnum.GetDefaultPort(base.DatabaseRow.Type);
		sapAseLoginTextEdit.Text = base.DatabaseRow.User;
		sapAsePasswordTextEdit.Text = base.DatabaseRow.Password;
		sapAseSavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
		sapAseCharsetTextEdit.Text = base.DatabaseRow.Param1;
	}

	protected override string GetPanelDocumentationTitle()
	{
		return sapAseDatabaseButtonEdit.Text + "@" + providedSapAseHost;
	}

	private void sapAseLoginTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSapAseLogin(acceptEmptyValue: true);
	}

	private void sapAsePortTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSapAsePort(acceptEmptyValue: true);
		PortTextEdit_EditValueChanged(sender, e);
	}

	private void sapAseDatabaseButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSapAseDatabase(acceptEmptyValue: true);
	}

	private void sapAseDefaultPortLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			sapAsePortTextEdit.Text = DatabaseTypeEnum.GetDefaultPort(SharedDatabaseTypeEnum.DatabaseType.SapAse);
		}
	}

	private void sapAsePasswordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateSapAsePassword();
	}

	private void sapAseDatabaseButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		base.DatabaseRow.Name = string.Empty;
		DatabaseButtonEdit_ButtonClick(sender, e);
	}

	private void sapAseHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void sapAseHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	private void sapAsePortTextEdit_Leave(object sender, EventArgs e)
	{
		PortTextEdit_Leave(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		sapAseLoginTextEdit.Text = string.Empty;
		sapAsePasswordTextEdit.Text = string.Empty;
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
		this.sapAseLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.sapAseDefaultPortLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.sapAseDatabaseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.sapAsePortTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.sapAseLoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.sapAsePasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.sapAseSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.sapAseHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.sapAseCharsetTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.sapAsePortLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sapAseLoginLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sapAsePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sapAseSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem34 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem35 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem36 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem37 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem38 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.sapAseDatabaseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem44 = new DevExpress.XtraLayout.LayoutControlItem();
		this.sapAseTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem48 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.sapAseHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem45 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.configureEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.sapAseCharsetLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.sapAseLayoutControl).BeginInit();
		this.sapAseLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.sapAseDatabaseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sapAsePortTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseLoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sapAsePasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseCharsetTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sapAsePortLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseLoginLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sapAsePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem34).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem35).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem36).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem37).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem38).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseDatabaseLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem44).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseTimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem48).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem45).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.configureEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseCharsetLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.sapAseLayoutControl.AllowCustomization = false;
		this.sapAseLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.sapAseLayoutControl.Controls.Add(this.sapAseDefaultPortLabelControl);
		this.sapAseLayoutControl.Controls.Add(this.sapAseDatabaseButtonEdit);
		this.sapAseLayoutControl.Controls.Add(this.sapAsePortTextEdit);
		this.sapAseLayoutControl.Controls.Add(this.sapAseLoginTextEdit);
		this.sapAseLayoutControl.Controls.Add(this.sapAsePasswordTextEdit);
		this.sapAseLayoutControl.Controls.Add(this.sapAseSavePasswordCheckEdit);
		this.sapAseLayoutControl.Controls.Add(this.sapAseHostComboBoxEdit);
		this.sapAseLayoutControl.Controls.Add(this.sapAseCharsetTextEdit);
		this.sapAseLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.sapAseLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.sapAseLayoutControl.Name = "sapAseLayoutControl";
		this.sapAseLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2548, 234, 360, 525);
		this.sapAseLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.sapAseLayoutControl.Root = this.layoutControlGroup6;
		this.sapAseLayoutControl.Size = new System.Drawing.Size(492, 340);
		this.sapAseLayoutControl.TabIndex = 3;
		this.sapAseLayoutControl.Text = "layoutControl1";
		this.sapAseDefaultPortLabelControl.AllowHtmlString = true;
		this.sapAseDefaultPortLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.sapAseDefaultPortLabelControl.Location = new System.Drawing.Point(347, 26);
		this.sapAseDefaultPortLabelControl.Name = "sapAseDefaultPortLabelControl";
		this.sapAseDefaultPortLabelControl.Size = new System.Drawing.Size(34, 20);
		this.sapAseDefaultPortLabelControl.StyleController = this.sapAseLayoutControl;
		this.sapAseDefaultPortLabelControl.TabIndex = 27;
		this.sapAseDefaultPortLabelControl.Text = "<href>default</href>";
		this.sapAseDefaultPortLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(sapAseDefaultPortLabelControl_HyperlinkClick);
		this.sapAseDatabaseButtonEdit.Location = new System.Drawing.Point(105, 144);
		this.sapAseDatabaseButtonEdit.Name = "sapAseDatabaseButtonEdit";
		this.sapAseDatabaseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.sapAseDatabaseButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.sapAseDatabaseButtonEdit.StyleController = this.sapAseLayoutControl;
		this.sapAseDatabaseButtonEdit.TabIndex = 4;
		this.sapAseDatabaseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(sapAseDatabaseButtonEdit_ButtonClick);
		this.sapAseDatabaseButtonEdit.EditValueChanged += new System.EventHandler(sapAseDatabaseButtonEdit_EditValueChanged);
		this.sapAsePortTextEdit.EditValue = "5000";
		this.sapAsePortTextEdit.Location = new System.Drawing.Point(105, 24);
		this.sapAsePortTextEdit.Margin = new System.Windows.Forms.Padding(2);
		this.sapAsePortTextEdit.Name = "sapAsePortTextEdit";
		this.sapAsePortTextEdit.Properties.Mask.EditMask = "\\d+";
		this.sapAsePortTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.sapAsePortTextEdit.Properties.Mask.ShowPlaceHolders = false;
		this.sapAsePortTextEdit.Properties.MaxLength = 5;
		this.sapAsePortTextEdit.Size = new System.Drawing.Size(230, 20);
		this.sapAsePortTextEdit.StyleController = this.sapAseLayoutControl;
		this.sapAsePortTextEdit.TabIndex = 0;
		this.sapAsePortTextEdit.EditValueChanged += new System.EventHandler(sapAsePortTextEdit_EditValueChanged);
		this.sapAsePortTextEdit.Leave += new System.EventHandler(sapAsePortTextEdit_Leave);
		this.sapAseLoginTextEdit.Location = new System.Drawing.Point(105, 72);
		this.sapAseLoginTextEdit.Name = "sapAseLoginTextEdit";
		this.sapAseLoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.sapAseLoginTextEdit.StyleController = this.sapAseLayoutControl;
		this.sapAseLoginTextEdit.TabIndex = 1;
		this.sapAseLoginTextEdit.EditValueChanged += new System.EventHandler(sapAseLoginTextEdit_EditValueChanged);
		this.sapAsePasswordTextEdit.Location = new System.Drawing.Point(105, 96);
		this.sapAsePasswordTextEdit.Name = "sapAsePasswordTextEdit";
		this.sapAsePasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.sapAsePasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.sapAsePasswordTextEdit.StyleController = this.sapAseLayoutControl;
		this.sapAsePasswordTextEdit.TabIndex = 2;
		this.sapAsePasswordTextEdit.EditValueChanged += new System.EventHandler(sapAsePasswordTextEdit_EditValueChanged);
		this.sapAseSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 120);
		this.sapAseSavePasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.sapAseSavePasswordCheckEdit.Name = "sapAseSavePasswordCheckEdit";
		this.sapAseSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.sapAseSavePasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.sapAseSavePasswordCheckEdit.StyleController = this.sapAseLayoutControl;
		this.sapAseSavePasswordCheckEdit.TabIndex = 3;
		this.sapAseHostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.sapAseHostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2);
		this.sapAseHostComboBoxEdit.Name = "sapAseHostComboBoxEdit";
		this.sapAseHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.sapAseHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.sapAseHostComboBoxEdit.StyleController = this.sapAseLayoutControl;
		this.sapAseHostComboBoxEdit.TabIndex = 1;
		this.sapAseHostComboBoxEdit.EditValueChanged += new System.EventHandler(sapAseHostComboBoxEdit_EditValueChanged);
		this.sapAseHostComboBoxEdit.Leave += new System.EventHandler(sapAseHostComboBoxEdit_Leave);
		this.sapAseCharsetTextEdit.EditValue = "iso_1";
		this.sapAseCharsetTextEdit.Location = new System.Drawing.Point(105, 48);
		this.sapAseCharsetTextEdit.Name = "sapAseCharsetTextEdit";
		this.sapAseCharsetTextEdit.Size = new System.Drawing.Size(230, 20);
		this.sapAseCharsetTextEdit.StyleController = this.sapAseLayoutControl;
		this.sapAseCharsetTextEdit.TabIndex = 1;
		this.layoutControlGroup6.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup6.GroupBordersVisible = false;
		this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[17]
		{
			this.sapAsePortLayoutControlItem, this.sapAseLoginLayoutControlItem, this.sapAsePasswordLayoutControlItem, this.sapAseSavePasswordLayoutControlItem, this.emptySpaceItem34, this.emptySpaceItem35, this.emptySpaceItem36, this.emptySpaceItem37, this.emptySpaceItem38, this.sapAseDatabaseLayoutControlItem,
			this.layoutControlItem44, this.sapAseTimeoutEmptySpaceItem, this.emptySpaceItem48, this.sapAseHostLayoutControlItem, this.emptySpaceItem45, this.configureEmptySpaceItem, this.sapAseCharsetLayoutControlItem
		});
		this.layoutControlGroup6.Name = "Root";
		this.layoutControlGroup6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup6.Size = new System.Drawing.Size(492, 340);
		this.layoutControlGroup6.TextVisible = false;
		this.sapAsePortLayoutControlItem.Control = this.sapAsePortTextEdit;
		this.sapAsePortLayoutControlItem.CustomizationFormText = "Port:";
		this.sapAsePortLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.sapAsePortLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sapAsePortLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sapAsePortLayoutControlItem.Name = "sapAsePortLayoutControlItem";
		this.sapAsePortLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sapAsePortLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sapAsePortLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sapAsePortLayoutControlItem.Text = "Port:";
		this.sapAsePortLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sapAsePortLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sapAsePortLayoutControlItem.TextToControlDistance = 5;
		this.sapAseLoginLayoutControlItem.Control = this.sapAseLoginTextEdit;
		this.sapAseLoginLayoutControlItem.CustomizationFormText = "User:";
		this.sapAseLoginLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.sapAseLoginLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sapAseLoginLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sapAseLoginLayoutControlItem.Name = "sapAseLoginLayoutControlItem";
		this.sapAseLoginLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sapAseLoginLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sapAseLoginLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sapAseLoginLayoutControlItem.Text = "User:";
		this.sapAseLoginLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sapAseLoginLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sapAseLoginLayoutControlItem.TextToControlDistance = 5;
		this.sapAsePasswordLayoutControlItem.Control = this.sapAsePasswordTextEdit;
		this.sapAsePasswordLayoutControlItem.CustomizationFormText = "Password";
		this.sapAsePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.sapAsePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sapAsePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sapAsePasswordLayoutControlItem.Name = "sapAsePasswordLayoutControlItem";
		this.sapAsePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sapAsePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sapAsePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sapAsePasswordLayoutControlItem.Text = "Password:";
		this.sapAsePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sapAsePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sapAsePasswordLayoutControlItem.TextToControlDistance = 5;
		this.sapAseSavePasswordLayoutControlItem.Control = this.sapAseSavePasswordCheckEdit;
		this.sapAseSavePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem18";
		this.sapAseSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 120);
		this.sapAseSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sapAseSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sapAseSavePasswordLayoutControlItem.Name = "sapAseSavePasswordLayoutControlItem";
		this.sapAseSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sapAseSavePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sapAseSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sapAseSavePasswordLayoutControlItem.Text = " ";
		this.sapAseSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sapAseSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sapAseSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem34.AllowHotTrack = false;
		this.emptySpaceItem34.Location = new System.Drawing.Point(0, 190);
		this.emptySpaceItem34.Name = "emptySpaceItem3";
		this.emptySpaceItem34.Size = new System.Drawing.Size(492, 150);
		this.emptySpaceItem34.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem35.AllowHotTrack = false;
		this.emptySpaceItem35.CustomizationFormText = "emptySpaceItem13";
		this.emptySpaceItem35.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem35.Name = "emptySpaceItem19";
		this.emptySpaceItem35.Size = new System.Drawing.Size(157, 48);
		this.emptySpaceItem35.Text = "emptySpaceItem13";
		this.emptySpaceItem35.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem36.AllowHotTrack = false;
		this.emptySpaceItem36.CustomizationFormText = "emptySpaceItem13";
		this.emptySpaceItem36.Location = new System.Drawing.Point(383, 24);
		this.emptySpaceItem36.Name = "emptySpaceItem25";
		this.emptySpaceItem36.Size = new System.Drawing.Size(109, 24);
		this.emptySpaceItem36.Text = "emptySpaceItem13";
		this.emptySpaceItem36.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem37.AllowHotTrack = false;
		this.emptySpaceItem37.CustomizationFormText = "emptySpaceItem13";
		this.emptySpaceItem37.Location = new System.Drawing.Point(335, 144);
		this.emptySpaceItem37.Name = "emptySpaceItem26";
		this.emptySpaceItem37.Size = new System.Drawing.Size(157, 24);
		this.emptySpaceItem37.Text = "emptySpaceItem13";
		this.emptySpaceItem37.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem38.AllowHotTrack = false;
		this.emptySpaceItem38.CustomizationFormText = "emptySpaceItem13";
		this.emptySpaceItem38.Location = new System.Drawing.Point(335, 96);
		this.emptySpaceItem38.Name = "emptySpaceItem27";
		this.emptySpaceItem38.Size = new System.Drawing.Size(157, 24);
		this.emptySpaceItem38.Text = "emptySpaceItem13";
		this.emptySpaceItem38.TextSize = new System.Drawing.Size(0, 0);
		this.sapAseDatabaseLayoutControlItem.Control = this.sapAseDatabaseButtonEdit;
		this.sapAseDatabaseLayoutControlItem.Location = new System.Drawing.Point(0, 144);
		this.sapAseDatabaseLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sapAseDatabaseLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sapAseDatabaseLayoutControlItem.Name = "sapAseDatabaseLayoutControlItem";
		this.sapAseDatabaseLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sapAseDatabaseLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sapAseDatabaseLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sapAseDatabaseLayoutControlItem.Text = "Database:";
		this.sapAseDatabaseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sapAseDatabaseLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sapAseDatabaseLayoutControlItem.TextToControlDistance = 5;
		this.layoutControlItem44.Control = this.sapAseDefaultPortLabelControl;
		this.layoutControlItem44.Location = new System.Drawing.Point(345, 24);
		this.layoutControlItem44.MaxSize = new System.Drawing.Size(38, 24);
		this.layoutControlItem44.MinSize = new System.Drawing.Size(38, 24);
		this.layoutControlItem44.Name = "layoutControlItem32";
		this.layoutControlItem44.Size = new System.Drawing.Size(38, 24);
		this.layoutControlItem44.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem44.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem44.TextVisible = false;
		this.sapAseTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.sapAseTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 168);
		this.sapAseTimeoutEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 22);
		this.sapAseTimeoutEmptySpaceItem.MinSize = new System.Drawing.Size(10, 22);
		this.sapAseTimeoutEmptySpaceItem.Name = "sapAseTimeoutEmptySpaceItem";
		this.sapAseTimeoutEmptySpaceItem.Size = new System.Drawing.Size(492, 22);
		this.sapAseTimeoutEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sapAseTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem48.AllowHotTrack = false;
		this.emptySpaceItem48.Location = new System.Drawing.Point(335, 24);
		this.emptySpaceItem48.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem48.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem48.Name = "emptySpaceItem48";
		this.emptySpaceItem48.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem48.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem48.TextSize = new System.Drawing.Size(0, 0);
		this.sapAseHostLayoutControlItem.Control = this.sapAseHostComboBoxEdit;
		this.sapAseHostLayoutControlItem.CustomizationFormText = "Host:";
		this.sapAseHostLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.sapAseHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sapAseHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sapAseHostLayoutControlItem.Name = "sapAseHostLayoutControlItem";
		this.sapAseHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sapAseHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sapAseHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sapAseHostLayoutControlItem.Text = "Host:";
		this.sapAseHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sapAseHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sapAseHostLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem45.AllowHotTrack = false;
		this.emptySpaceItem45.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem45.Name = "emptySpaceItem45";
		this.emptySpaceItem45.Size = new System.Drawing.Size(157, 24);
		this.emptySpaceItem45.TextSize = new System.Drawing.Size(0, 0);
		this.configureEmptySpaceItem.AllowHotTrack = false;
		this.configureEmptySpaceItem.Location = new System.Drawing.Point(335, 120);
		this.configureEmptySpaceItem.Name = "configureEmptySpaceItem";
		this.configureEmptySpaceItem.Size = new System.Drawing.Size(157, 24);
		this.configureEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.sapAseCharsetLayoutControlItem.Control = this.sapAseCharsetTextEdit;
		this.sapAseCharsetLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.sapAseCharsetLayoutControlItem.CustomizationFormText = "User:";
		this.sapAseCharsetLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.sapAseCharsetLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sapAseCharsetLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sapAseCharsetLayoutControlItem.Name = "sapAseCharsetLayoutControlItem";
		this.sapAseCharsetLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sapAseCharsetLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.sapAseCharsetLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sapAseCharsetLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.sapAseCharsetLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sapAseCharsetLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.sapAseCharsetLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sapAseCharsetLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sapAseCharsetLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sapAseCharsetLayoutControlItem.Text = "Charset:";
		this.sapAseCharsetLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sapAseCharsetLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sapAseCharsetLayoutControlItem.TextToControlDistance = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.sapAseLayoutControl);
		base.Name = "SapAseConnectorControl";
		base.Size = new System.Drawing.Size(492, 340);
		((System.ComponentModel.ISupportInitialize)this.sapAseLayoutControl).EndInit();
		this.sapAseLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.sapAseDatabaseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sapAsePortTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseLoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sapAsePasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseCharsetTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sapAsePortLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseLoginLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sapAsePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem34).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem35).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem36).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem37).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem38).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseDatabaseLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem44).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseTimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem48).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem45).EndInit();
		((System.ComponentModel.ISupportInitialize)this.configureEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sapAseCharsetLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
