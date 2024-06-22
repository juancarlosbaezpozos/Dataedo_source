using System;
using System.Collections.Generic;
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

public class RedshiftConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl redshiftLayoutControl;

	private LookUpEdit redshiftSSLModeLookUpEdit;

	private LabelControl redshiftDefaultPortLabelControl;

	private CheckEdit redshiftSavePasswordCheckEdit;

	private TextEdit redshiftLoginTextEdit;

	private TextEdit redshiftPasswordTextEdit;

	private TextEdit redshiftPortTextEdit;

	private TextEdit redshiftDatabaseTextEdit;

	private ComboBoxEdit redshiftHostComboBoxEdit;

	private LayoutControlGroup redshiftLayoutControlGroup;

	private LayoutControlItem redshiftPasswordLayoutControlItem;

	private LayoutControlItem redshiftLoginLayoutControlItem;

	private LayoutControlItem redshiftSavePasswordLayoutControlItem;

	private EmptySpaceItem redshiftEmptySpaceItem6;

	private EmptySpaceItem redshiftEmptySpaceItem1;

	private EmptySpaceItem redshiftEmptySpaceItem2;

	private EmptySpaceItem redshiftEmptySpaceItem3;

	private EmptySpaceItem redshiftTimeoutEmptySpaceItem;

	private LayoutControlItem redshiftPortLayoutControlItem;

	private LayoutControlItem redshiftDefaultPortLayoutControlItem;

	private LayoutControlItem redshiftDatabaseLayoutControlItem;

	private EmptySpaceItem emptySpaceItem50;

	private LayoutControlItem redshiftHostLayoutControlItem;

	private EmptySpaceItem emptySpaceItem52;

	private EmptySpaceItem emptySpaceItem39;

	private LayoutControlItem redshiftSSLModeLayoutControlItem;

	private string providedRedshiftHost => splittedHost?.Host ?? redshiftHostComboBoxEdit.Text;

	private string providedRedshiftPort => redshiftPortTextEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Redshift;

	protected override TextEdit HostTextEdit => redshiftHostComboBoxEdit;

	protected override TextEdit PortTextEdit => redshiftPortTextEdit;

	protected override ComboBoxEdit HostComboBoxEdit => redshiftHostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => redshiftSavePasswordCheckEdit;

	public RedshiftConnectorControl()
	{
		InitializeComponent();
	}

	protected override void SetAuthenticationDataSource()
	{
		Dictionary<SSLTypeEnum.SSLType, string> redshiftSSLTypes = SSLTypeEnum.GetRedshiftSSLTypes();
		redshiftSSLModeLookUpEdit.Properties.DataSource = redshiftSSLTypes;
		redshiftSSLModeLookUpEdit.Properties.DropDownRows = redshiftSSLTypes.Count;
		if (redshiftSSLModeLookUpEdit.EditValue == null)
		{
			redshiftSSLModeLookUpEdit.EditValue = SSLTypeEnum.SSLType.Require;
		}
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? redshiftDatabaseTextEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedRedshiftHost, redshiftLoginTextEdit.Text, redshiftPasswordTextEdit.Text, providedRedshiftPort, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, SSLTypeEnum.TypeToString((SSLTypeEnum.SSLType)redshiftSSLModeLookUpEdit.EditValue));
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		redshiftLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			redshiftLayoutControl.Root.AddItem(timeoutLayoutControlItem, redshiftTimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateRedshiftHost();
		flag &= ValidateRedshiftLogin();
		flag &= ValidateRedshiftPort();
		flag &= ValidateRedshiftPassword();
		flag &= ValidateRedshiftSSLMode();
		if (!testForGettingDatabasesList)
		{
			flag &= ValidateRedshiftDatabase();
		}
		return flag;
	}

	private bool ValidateRedshiftHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(redshiftHostComboBoxEdit, addDBErrorProvider, "host", acceptEmptyValue);
	}

	private bool ValidateRedshiftPassword(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(redshiftPasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateRedshiftLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(redshiftLoginTextEdit, addDBErrorProvider, "user", acceptEmptyValue);
	}

	private bool ValidateRedshiftPort(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(redshiftPortTextEdit, addDBErrorProvider, "port", acceptEmptyValue);
	}

	private bool ValidateRedshiftDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(redshiftDatabaseTextEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	private bool ValidateRedshiftSSLMode(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(redshiftSSLModeLookUpEdit, addDBErrorProvider, "SSL Mode", acceptEmptyValue);
	}

	public override string GetSSLTypeValue()
	{
		return redshiftSSLModeLookUpEdit.EditValue.ToString();
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		redshiftHostComboBoxEdit.Text = base.DatabaseRow.Host;
		redshiftDatabaseTextEdit.EditValue = base.DatabaseRow.Name;
		redshiftSSLModeLookUpEdit.EditValue = SSLTypeEnum.StringToType(base.DatabaseRow.SSLType);
		redshiftPortTextEdit.Text = PrepareValue.ToString(base.DatabaseRow.Port) ?? DatabaseTypeEnum.GetDefaultPort(base.DatabaseRow.Type);
		redshiftLoginTextEdit.Text = base.DatabaseRow.User;
		redshiftPasswordTextEdit.Text = base.DatabaseRow.Password;
		redshiftSavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return redshiftDatabaseTextEdit.Text + "@" + providedRedshiftHost;
	}

	private void redshiftPortTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateRedshiftPort(acceptEmptyValue: true);
		PortTextEdit_EditValueChanged(sender, e);
	}

	private void redshiftLoginTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateRedshiftLogin(acceptEmptyValue: true);
	}

	private void redshiftDatabaseTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateRedshiftDatabase(acceptEmptyValue: true);
	}

	private void redshiftDefaultPortLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			redshiftPortTextEdit.Text = DatabaseTypeEnum.GetDefaultPort(SharedDatabaseTypeEnum.DatabaseType.Redshift);
		}
	}

	private void redshiftSSLModeLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateRedshiftSSLMode();
	}

	private void redshiftPasswordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateRedshiftPassword();
	}

	private void redshiftHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void redshiftHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	private void redshiftPortTextEdit_Leave(object sender, EventArgs e)
	{
		PortTextEdit_Leave(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		redshiftLoginTextEdit.Text = string.Empty;
		redshiftPasswordTextEdit.Text = string.Empty;
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
		this.redshiftLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.redshiftSSLModeLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.redshiftDefaultPortLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.redshiftSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.redshiftLoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.redshiftPasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.redshiftPortTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.redshiftDatabaseTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.redshiftHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.redshiftLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.redshiftPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.redshiftLoginLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.redshiftSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.redshiftEmptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.redshiftEmptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.redshiftEmptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.redshiftEmptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.redshiftTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.redshiftPortLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.redshiftDefaultPortLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.redshiftDatabaseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem50 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.redshiftHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem52 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem39 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.redshiftSSLModeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.redshiftLayoutControl).BeginInit();
		this.redshiftLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.redshiftSSLModeLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftLoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftPasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftPortTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftDatabaseTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftPasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftLoginLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftEmptySpaceItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftEmptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftEmptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftEmptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftTimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftPortLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftDefaultPortLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftDatabaseLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem50).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem52).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem39).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftSSLModeLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.redshiftLayoutControl.AllowCustomization = false;
		this.redshiftLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.redshiftLayoutControl.Controls.Add(this.redshiftSSLModeLookUpEdit);
		this.redshiftLayoutControl.Controls.Add(this.redshiftDefaultPortLabelControl);
		this.redshiftLayoutControl.Controls.Add(this.redshiftSavePasswordCheckEdit);
		this.redshiftLayoutControl.Controls.Add(this.redshiftLoginTextEdit);
		this.redshiftLayoutControl.Controls.Add(this.redshiftPasswordTextEdit);
		this.redshiftLayoutControl.Controls.Add(this.redshiftPortTextEdit);
		this.redshiftLayoutControl.Controls.Add(this.redshiftDatabaseTextEdit);
		this.redshiftLayoutControl.Controls.Add(this.redshiftHostComboBoxEdit);
		this.redshiftLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.redshiftLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.redshiftLayoutControl.Name = "redshiftLayoutControl";
		this.redshiftLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2269, 179, 729, 548);
		this.redshiftLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.redshiftLayoutControl.Root = this.redshiftLayoutControlGroup;
		this.redshiftLayoutControl.Size = new System.Drawing.Size(543, 387);
		this.redshiftLayoutControl.TabIndex = 30;
		this.redshiftLayoutControl.Text = "layoutControl1";
		this.redshiftSSLModeLookUpEdit.Location = new System.Drawing.Point(105, 120);
		this.redshiftSSLModeLookUpEdit.Name = "redshiftSSLModeLookUpEdit";
		this.redshiftSSLModeLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.redshiftSSLModeLookUpEdit.Properties.DisplayMember = "Value";
		this.redshiftSSLModeLookUpEdit.Properties.NullText = "";
		this.redshiftSSLModeLookUpEdit.Properties.ShowFooter = false;
		this.redshiftSSLModeLookUpEdit.Properties.ShowHeader = false;
		this.redshiftSSLModeLookUpEdit.Properties.ShowLines = false;
		this.redshiftSSLModeLookUpEdit.Properties.ValueMember = "Key";
		this.redshiftSSLModeLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.redshiftSSLModeLookUpEdit.StyleController = this.redshiftLayoutControl;
		this.redshiftSSLModeLookUpEdit.TabIndex = 32;
		this.redshiftSSLModeLookUpEdit.EditValueChanged += new System.EventHandler(redshiftSSLModeLookUpEdit_EditValueChanged);
		this.redshiftDefaultPortLabelControl.AllowHtmlString = true;
		this.redshiftDefaultPortLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.redshiftDefaultPortLabelControl.Location = new System.Drawing.Point(346, 26);
		this.redshiftDefaultPortLabelControl.MinimumSize = new System.Drawing.Size(34, 20);
		this.redshiftDefaultPortLabelControl.Name = "redshiftDefaultPortLabelControl";
		this.redshiftDefaultPortLabelControl.Size = new System.Drawing.Size(34, 20);
		this.redshiftDefaultPortLabelControl.StyleController = this.redshiftLayoutControl;
		this.redshiftDefaultPortLabelControl.TabIndex = 31;
		this.redshiftDefaultPortLabelControl.Text = "<href>default</href>";
		this.redshiftDefaultPortLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(redshiftDefaultPortLabelControl_HyperlinkClick);
		this.redshiftSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 96);
		this.redshiftSavePasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.redshiftSavePasswordCheckEdit.Name = "redshiftSavePasswordCheckEdit";
		this.redshiftSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.redshiftSavePasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.redshiftSavePasswordCheckEdit.StyleController = this.redshiftLayoutControl;
		this.redshiftSavePasswordCheckEdit.TabIndex = 3;
		this.redshiftLoginTextEdit.Location = new System.Drawing.Point(105, 48);
		this.redshiftLoginTextEdit.Name = "redshiftLoginTextEdit";
		this.redshiftLoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.redshiftLoginTextEdit.StyleController = this.redshiftLayoutControl;
		this.redshiftLoginTextEdit.TabIndex = 1;
		this.redshiftLoginTextEdit.EditValueChanged += new System.EventHandler(redshiftLoginTextEdit_EditValueChanged);
		this.redshiftPasswordTextEdit.Location = new System.Drawing.Point(105, 72);
		this.redshiftPasswordTextEdit.Name = "redshiftPasswordTextEdit";
		this.redshiftPasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.redshiftPasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.redshiftPasswordTextEdit.StyleController = this.redshiftLayoutControl;
		this.redshiftPasswordTextEdit.TabIndex = 2;
		this.redshiftPasswordTextEdit.EditValueChanged += new System.EventHandler(redshiftPasswordTextEdit_EditValueChanged);
		this.redshiftPortTextEdit.EditValue = "5439";
		this.redshiftPortTextEdit.Location = new System.Drawing.Point(105, 24);
		this.redshiftPortTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.redshiftPortTextEdit.Name = "redshiftPortTextEdit";
		this.redshiftPortTextEdit.Properties.Mask.EditMask = "\\d+";
		this.redshiftPortTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.redshiftPortTextEdit.Properties.Mask.ShowPlaceHolders = false;
		this.redshiftPortTextEdit.Properties.MaxLength = 5;
		this.redshiftPortTextEdit.Size = new System.Drawing.Size(230, 20);
		this.redshiftPortTextEdit.StyleController = this.redshiftLayoutControl;
		this.redshiftPortTextEdit.TabIndex = 0;
		this.redshiftPortTextEdit.EditValueChanged += new System.EventHandler(redshiftPortTextEdit_EditValueChanged);
		this.redshiftPortTextEdit.Leave += new System.EventHandler(redshiftPortTextEdit_Leave);
		this.redshiftDatabaseTextEdit.Location = new System.Drawing.Point(105, 144);
		this.redshiftDatabaseTextEdit.Name = "redshiftDatabaseTextEdit";
		this.redshiftDatabaseTextEdit.Size = new System.Drawing.Size(230, 20);
		this.redshiftDatabaseTextEdit.StyleController = this.redshiftLayoutControl;
		this.redshiftDatabaseTextEdit.TabIndex = 1;
		this.redshiftDatabaseTextEdit.EditValueChanged += new System.EventHandler(redshiftDatabaseTextEdit_EditValueChanged);
		this.redshiftHostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.redshiftHostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.redshiftHostComboBoxEdit.Name = "redshiftHostComboBoxEdit";
		this.redshiftHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.redshiftHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.redshiftHostComboBoxEdit.StyleController = this.redshiftLayoutControl;
		this.redshiftHostComboBoxEdit.TabIndex = 1;
		this.redshiftHostComboBoxEdit.EditValueChanged += new System.EventHandler(redshiftHostComboBoxEdit_EditValueChanged);
		this.redshiftHostComboBoxEdit.Leave += new System.EventHandler(redshiftHostComboBoxEdit_Leave);
		this.redshiftLayoutControlGroup.CustomizationFormText = "Root";
		this.redshiftLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.redshiftLayoutControlGroup.GroupBordersVisible = false;
		this.redshiftLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[16]
		{
			this.redshiftPasswordLayoutControlItem, this.redshiftLoginLayoutControlItem, this.redshiftSavePasswordLayoutControlItem, this.redshiftEmptySpaceItem6, this.redshiftEmptySpaceItem1, this.redshiftEmptySpaceItem2, this.redshiftEmptySpaceItem3, this.redshiftTimeoutEmptySpaceItem, this.redshiftPortLayoutControlItem, this.redshiftDefaultPortLayoutControlItem,
			this.emptySpaceItem50, this.redshiftHostLayoutControlItem, this.emptySpaceItem52, this.emptySpaceItem39, this.redshiftSSLModeLayoutControlItem, this.redshiftDatabaseLayoutControlItem
		});
		this.redshiftLayoutControlGroup.Name = "Root";
		this.redshiftLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.redshiftLayoutControlGroup.Size = new System.Drawing.Size(543, 387);
		this.redshiftLayoutControlGroup.TextVisible = false;
		this.redshiftPasswordLayoutControlItem.Control = this.redshiftPasswordTextEdit;
		this.redshiftPasswordLayoutControlItem.CustomizationFormText = "Password";
		this.redshiftPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.redshiftPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.redshiftPasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.redshiftPasswordLayoutControlItem.Name = "redshiftPasswordLayoutControlItem";
		this.redshiftPasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.redshiftPasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.redshiftPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.redshiftPasswordLayoutControlItem.Text = "Password:";
		this.redshiftPasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.redshiftPasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.redshiftPasswordLayoutControlItem.TextToControlDistance = 5;
		this.redshiftLoginLayoutControlItem.Control = this.redshiftLoginTextEdit;
		this.redshiftLoginLayoutControlItem.CustomizationFormText = "User:";
		this.redshiftLoginLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.redshiftLoginLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.redshiftLoginLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.redshiftLoginLayoutControlItem.Name = "redshiftLoginLayoutControlItem";
		this.redshiftLoginLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.redshiftLoginLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.redshiftLoginLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.redshiftLoginLayoutControlItem.Text = "User:";
		this.redshiftLoginLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.redshiftLoginLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.redshiftLoginLayoutControlItem.TextToControlDistance = 5;
		this.redshiftSavePasswordLayoutControlItem.Control = this.redshiftSavePasswordCheckEdit;
		this.redshiftSavePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem2";
		this.redshiftSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.redshiftSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.redshiftSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.redshiftSavePasswordLayoutControlItem.Name = "redshiftSavePasswordLayoutControlItem";
		this.redshiftSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.redshiftSavePasswordLayoutControlItem.Size = new System.Drawing.Size(543, 24);
		this.redshiftSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.redshiftSavePasswordLayoutControlItem.Text = " ";
		this.redshiftSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.redshiftSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.redshiftSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.redshiftEmptySpaceItem6.AllowHotTrack = false;
		this.redshiftEmptySpaceItem6.CustomizationFormText = "emptySpaceItem6";
		this.redshiftEmptySpaceItem6.Location = new System.Drawing.Point(0, 192);
		this.redshiftEmptySpaceItem6.Name = "redshiftEmptySpaceItem6";
		this.redshiftEmptySpaceItem6.Size = new System.Drawing.Size(543, 195);
		this.redshiftEmptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		this.redshiftEmptySpaceItem1.AllowHotTrack = false;
		this.redshiftEmptySpaceItem1.CustomizationFormText = "emptySpaceItem20";
		this.redshiftEmptySpaceItem1.Location = new System.Drawing.Point(381, 24);
		this.redshiftEmptySpaceItem1.Name = "redshiftEmptySpaceItem1";
		this.redshiftEmptySpaceItem1.Size = new System.Drawing.Size(162, 24);
		this.redshiftEmptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.redshiftEmptySpaceItem2.AllowHotTrack = false;
		this.redshiftEmptySpaceItem2.CustomizationFormText = "emptySpaceItem21";
		this.redshiftEmptySpaceItem2.Location = new System.Drawing.Point(335, 48);
		this.redshiftEmptySpaceItem2.Name = "redshiftEmptySpaceItem2";
		this.redshiftEmptySpaceItem2.Size = new System.Drawing.Size(208, 24);
		this.redshiftEmptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.redshiftEmptySpaceItem3.AllowHotTrack = false;
		this.redshiftEmptySpaceItem3.CustomizationFormText = "emptySpaceItem22";
		this.redshiftEmptySpaceItem3.Location = new System.Drawing.Point(335, 72);
		this.redshiftEmptySpaceItem3.Name = "redshiftEmptySpaceItem3";
		this.redshiftEmptySpaceItem3.Size = new System.Drawing.Size(208, 24);
		this.redshiftEmptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.redshiftTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.redshiftTimeoutEmptySpaceItem.CustomizationFormText = "emptySpaceItem12";
		this.redshiftTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 168);
		this.redshiftTimeoutEmptySpaceItem.MaxSize = new System.Drawing.Size(405, 24);
		this.redshiftTimeoutEmptySpaceItem.MinSize = new System.Drawing.Size(405, 24);
		this.redshiftTimeoutEmptySpaceItem.Name = "redshiftTimeoutEmptySpaceItem";
		this.redshiftTimeoutEmptySpaceItem.Size = new System.Drawing.Size(543, 24);
		this.redshiftTimeoutEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.redshiftTimeoutEmptySpaceItem.Text = "emptySpaceItem12";
		this.redshiftTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.redshiftPortLayoutControlItem.Control = this.redshiftPortTextEdit;
		this.redshiftPortLayoutControlItem.CustomizationFormText = "Port:";
		this.redshiftPortLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.redshiftPortLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.redshiftPortLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.redshiftPortLayoutControlItem.Name = "redshiftPortLayoutControlItem";
		this.redshiftPortLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.redshiftPortLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.redshiftPortLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.redshiftPortLayoutControlItem.Text = "Port:";
		this.redshiftPortLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.redshiftPortLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.redshiftPortLayoutControlItem.TextToControlDistance = 5;
		this.redshiftDefaultPortLayoutControlItem.Control = this.redshiftDefaultPortLabelControl;
		this.redshiftDefaultPortLayoutControlItem.Location = new System.Drawing.Point(345, 24);
		this.redshiftDefaultPortLayoutControlItem.Name = "redshiftDefaultPortLayoutControlItem";
		this.redshiftDefaultPortLayoutControlItem.Size = new System.Drawing.Size(36, 24);
		this.redshiftDefaultPortLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.redshiftDefaultPortLayoutControlItem.TextVisible = false;
		this.redshiftDatabaseLayoutControlItem.Control = this.redshiftDatabaseTextEdit;
		this.redshiftDatabaseLayoutControlItem.CustomizationFormText = "Database:";
		this.redshiftDatabaseLayoutControlItem.Location = new System.Drawing.Point(0, 144);
		this.redshiftDatabaseLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.redshiftDatabaseLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.redshiftDatabaseLayoutControlItem.Name = "redshiftDatabaseLayoutControlItem";
		this.redshiftDatabaseLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.redshiftDatabaseLayoutControlItem.Size = new System.Drawing.Size(543, 24);
		this.redshiftDatabaseLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.redshiftDatabaseLayoutControlItem.Text = "Database:";
		this.redshiftDatabaseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.redshiftDatabaseLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.redshiftDatabaseLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem50.AllowHotTrack = false;
		this.emptySpaceItem50.Location = new System.Drawing.Point(335, 24);
		this.emptySpaceItem50.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem50.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem50.Name = "emptySpaceItem50";
		this.emptySpaceItem50.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem50.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem50.TextSize = new System.Drawing.Size(0, 0);
		this.redshiftHostLayoutControlItem.Control = this.redshiftHostComboBoxEdit;
		this.redshiftHostLayoutControlItem.CustomizationFormText = "Host:";
		this.redshiftHostLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.redshiftHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.redshiftHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.redshiftHostLayoutControlItem.Name = "redshiftHostLayoutControlItem";
		this.redshiftHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.redshiftHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.redshiftHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.redshiftHostLayoutControlItem.Text = "Host:";
		this.redshiftHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.redshiftHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.redshiftHostLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem52.AllowHotTrack = false;
		this.emptySpaceItem52.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem52.Name = "emptySpaceItem52";
		this.emptySpaceItem52.Size = new System.Drawing.Size(208, 24);
		this.emptySpaceItem52.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem39.AllowHotTrack = false;
		this.emptySpaceItem39.Location = new System.Drawing.Point(335, 120);
		this.emptySpaceItem39.Name = "emptySpaceItem39";
		this.emptySpaceItem39.Size = new System.Drawing.Size(208, 24);
		this.emptySpaceItem39.TextSize = new System.Drawing.Size(0, 0);
		this.redshiftSSLModeLayoutControlItem.Control = this.redshiftSSLModeLookUpEdit;
		this.redshiftSSLModeLayoutControlItem.Location = new System.Drawing.Point(0, 120);
		this.redshiftSSLModeLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.redshiftSSLModeLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.redshiftSSLModeLayoutControlItem.Name = "redshiftSSLModeLayoutControlItem";
		this.redshiftSSLModeLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.redshiftSSLModeLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.redshiftSSLModeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.redshiftSSLModeLayoutControlItem.Text = "SSL mode:";
		this.redshiftSSLModeLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.redshiftSSLModeLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.redshiftSSLModeLayoutControlItem.TextToControlDistance = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.redshiftLayoutControl);
		base.Name = "RedshiftConnectorControl";
		base.Size = new System.Drawing.Size(543, 387);
		((System.ComponentModel.ISupportInitialize)this.redshiftLayoutControl).EndInit();
		this.redshiftLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.redshiftSSLModeLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftLoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftPasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftPortTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftDatabaseTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftPasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftLoginLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftEmptySpaceItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftEmptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftEmptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftEmptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftTimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftPortLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftDefaultPortLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftDatabaseLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem50).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem52).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem39).EndInit();
		((System.ComponentModel.ISupportInitialize)this.redshiftSSLModeLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
