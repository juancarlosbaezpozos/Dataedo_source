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

public class VerticaConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl verticaLayoutControl;

	private ComboBoxEdit verticaHostComboBoxEdit;

	private TextEdit verticaPortTextEdit;

	private TextEdit verticaLoginTextEdit;

	private TextEdit verticaPasswordTextEdit;

	private TextEdit verticaDatabaseTextEdit;

	private CheckEdit verticaSavePasswordCheckEdit;

	private LabelControl verticaDefaultPortLabelControl;

	private LayoutControlGroup layoutControlGroup10;

	private LayoutControlItem verticaHostLayoutControlItem;

	private LayoutControlItem verticaPortLayoutControlItem;

	private LayoutControlItem verticaUserLayoutControlItem;

	private LayoutControlItem verticaPasswordLayoutControlItem;

	private LayoutControlItem verticaDatabaseLayoutControlItem;

	private LayoutControlItem verticaSavePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem70;

	private EmptySpaceItem emptySpaceItem71;

	private EmptySpaceItem emptySpaceItem72;

	private EmptySpaceItem emptySpaceItem73;

	private EmptySpaceItem emptySpaceItem74;

	private EmptySpaceItem emptySpaceItem76;

	private EmptySpaceItem emptySpaceItem77;

	private LayoutControlItem layoutControlItem32;

	private EmptySpaceItem verticaTimeoutEmptySpaceItem;

	private string providedVerticaHost => splittedHost?.Host ?? verticaHostComboBoxEdit.Text;

	private string providedVerticaPort => verticaPortTextEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Vertica;

	protected override TextEdit HostTextEdit => verticaHostComboBoxEdit;

	protected override TextEdit PortTextEdit => verticaPortTextEdit;

	protected override ComboBoxEdit HostComboBoxEdit => verticaHostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => verticaSavePasswordCheckEdit;

	public VerticaConnectorControl()
	{
		InitializeComponent();
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? verticaDatabaseTextEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedVerticaHost, verticaLoginTextEdit.Text, verticaPasswordTextEdit.Text, providedVerticaPort, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion);
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		verticaLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			verticaLayoutControl.Root.AddItem(timeoutLayoutControlItem, verticaTimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateVerticaHost();
		flag &= ValidateVerticaLogin();
		flag &= ValidateVerticaPort();
		flag &= ValidateVerticaPassword();
		if (!testForGettingDatabasesList)
		{
			flag &= ValidateVerticaDatabase();
		}
		return flag;
	}

	private bool ValidateVerticaHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(verticaHostComboBoxEdit, addDBErrorProvider, "host", acceptEmptyValue);
	}

	private bool ValidateVerticaPassword(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(verticaPasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateVerticaLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(verticaLoginTextEdit, addDBErrorProvider, "user", acceptEmptyValue);
	}

	private bool ValidateVerticaPort(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(verticaPortTextEdit, addDBErrorProvider, "port", acceptEmptyValue);
	}

	private bool ValidateVerticaDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(verticaDatabaseTextEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		verticaHostComboBoxEdit.Text = base.DatabaseRow.Host;
		verticaDatabaseTextEdit.EditValue = base.DatabaseRow.Name;
		verticaPortTextEdit.Text = PrepareValue.ToString(base.DatabaseRow.Port) ?? DatabaseTypeEnum.GetDefaultPort(base.DatabaseRow.Type);
		verticaLoginTextEdit.Text = base.DatabaseRow.User;
		verticaPasswordTextEdit.Text = base.DatabaseRow.Password;
		verticaSavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return verticaDatabaseTextEdit.Text + "@" + providedVerticaHost;
	}

	private void verticaDefaultPortLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			verticaPortTextEdit.Text = DatabaseTypeEnum.GetDefaultPort(SharedDatabaseTypeEnum.DatabaseType.Vertica);
		}
	}

	private void verticaHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void verticaHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	private void verticaPortTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		PortTextEdit_EditValueChanged(sender, e);
	}

	private void verticaPortTextEdit_Leave(object sender, EventArgs e)
	{
		PortTextEdit_Leave(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		verticaLoginTextEdit.Text = string.Empty;
		verticaPasswordTextEdit.Text = string.Empty;
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
		this.verticaLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.verticaHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.verticaPortTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.verticaLoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.verticaPasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.verticaDatabaseTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.verticaSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.verticaDefaultPortLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup10 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.verticaHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.verticaPortLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.verticaUserLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.verticaPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.verticaDatabaseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.verticaSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem70 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem71 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem72 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem73 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem74 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem76 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem77 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem32 = new DevExpress.XtraLayout.LayoutControlItem();
		this.verticaTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.verticaLayoutControl).BeginInit();
		this.verticaLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.verticaHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.verticaPortTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.verticaLoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.verticaPasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.verticaDatabaseTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.verticaSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.verticaHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.verticaPortLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.verticaUserLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.verticaPasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.verticaDatabaseLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.verticaSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem70).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem71).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem72).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem73).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem74).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem76).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem77).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem32).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.verticaTimeoutEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.verticaLayoutControl.AllowCustomization = false;
		this.verticaLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.verticaLayoutControl.Controls.Add(this.verticaHostComboBoxEdit);
		this.verticaLayoutControl.Controls.Add(this.verticaPortTextEdit);
		this.verticaLayoutControl.Controls.Add(this.verticaLoginTextEdit);
		this.verticaLayoutControl.Controls.Add(this.verticaPasswordTextEdit);
		this.verticaLayoutControl.Controls.Add(this.verticaDatabaseTextEdit);
		this.verticaLayoutControl.Controls.Add(this.verticaSavePasswordCheckEdit);
		this.verticaLayoutControl.Controls.Add(this.verticaDefaultPortLabelControl);
		this.verticaLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.verticaLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.verticaLayoutControl.Name = "verticaLayoutControl";
		this.verticaLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(872, 130, 832, 925);
		this.verticaLayoutControl.Root = this.layoutControlGroup10;
		this.verticaLayoutControl.Size = new System.Drawing.Size(471, 407);
		this.verticaLayoutControl.TabIndex = 2;
		this.verticaLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.verticaHostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.verticaHostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.verticaHostComboBoxEdit.Name = "verticaHostComboBoxEdit";
		this.verticaHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.verticaHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.verticaHostComboBoxEdit.StyleController = this.verticaLayoutControl;
		this.verticaHostComboBoxEdit.TabIndex = 4;
		this.verticaHostComboBoxEdit.EditValueChanged += new System.EventHandler(verticaHostComboBoxEdit_EditValueChanged);
		this.verticaHostComboBoxEdit.Leave += new System.EventHandler(verticaHostComboBoxEdit_Leave);
		this.verticaPortTextEdit.EditValue = "5433";
		this.verticaPortTextEdit.Location = new System.Drawing.Point(105, 24);
		this.verticaPortTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.verticaPortTextEdit.Name = "verticaPortTextEdit";
		this.verticaPortTextEdit.Properties.Mask.EditMask = "\\d+";
		this.verticaPortTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.verticaPortTextEdit.Properties.Mask.ShowPlaceHolders = false;
		this.verticaPortTextEdit.Properties.MaxLength = 5;
		this.verticaPortTextEdit.Size = new System.Drawing.Size(230, 20);
		this.verticaPortTextEdit.StyleController = this.verticaLayoutControl;
		this.verticaPortTextEdit.TabIndex = 5;
		this.verticaPortTextEdit.EditValueChanged += new System.EventHandler(verticaPortTextEdit_EditValueChanged);
		this.verticaPortTextEdit.Leave += new System.EventHandler(verticaPortTextEdit_Leave);
		this.verticaLoginTextEdit.Location = new System.Drawing.Point(105, 48);
		this.verticaLoginTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.verticaLoginTextEdit.Name = "verticaLoginTextEdit";
		this.verticaLoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.verticaLoginTextEdit.StyleController = this.verticaLayoutControl;
		this.verticaLoginTextEdit.TabIndex = 7;
		this.verticaPasswordTextEdit.Location = new System.Drawing.Point(105, 72);
		this.verticaPasswordTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.verticaPasswordTextEdit.Name = "verticaPasswordTextEdit";
		this.verticaPasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.verticaPasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.verticaPasswordTextEdit.StyleController = this.verticaLayoutControl;
		this.verticaPasswordTextEdit.TabIndex = 8;
		this.verticaDatabaseTextEdit.Location = new System.Drawing.Point(105, 120);
		this.verticaDatabaseTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.verticaDatabaseTextEdit.Name = "verticaDatabaseTextEdit";
		this.verticaDatabaseTextEdit.Size = new System.Drawing.Size(230, 20);
		this.verticaDatabaseTextEdit.StyleController = this.verticaLayoutControl;
		this.verticaDatabaseTextEdit.TabIndex = 6;
		this.verticaSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 96);
		this.verticaSavePasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.verticaSavePasswordCheckEdit.Name = "verticaSavePasswordCheckEdit";
		this.verticaSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.verticaSavePasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.verticaSavePasswordCheckEdit.StyleController = this.verticaLayoutControl;
		this.verticaSavePasswordCheckEdit.TabIndex = 3;
		this.verticaDefaultPortLabelControl.AllowHtmlString = true;
		this.verticaDefaultPortLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.verticaDefaultPortLabelControl.Location = new System.Drawing.Point(346, 26);
		this.verticaDefaultPortLabelControl.Name = "verticaDefaultPortLabelControl";
		this.verticaDefaultPortLabelControl.Size = new System.Drawing.Size(36, 20);
		this.verticaDefaultPortLabelControl.StyleController = this.verticaLayoutControl;
		this.verticaDefaultPortLabelControl.TabIndex = 27;
		this.verticaDefaultPortLabelControl.Text = "<href>default</href>";
		this.verticaDefaultPortLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(verticaDefaultPortLabelControl_HyperlinkClick);
		this.layoutControlGroup10.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup10.GroupBordersVisible = false;
		this.layoutControlGroup10.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[15]
		{
			this.verticaHostLayoutControlItem, this.verticaPortLayoutControlItem, this.verticaUserLayoutControlItem, this.verticaPasswordLayoutControlItem, this.verticaDatabaseLayoutControlItem, this.verticaSavePasswordLayoutControlItem, this.emptySpaceItem70, this.emptySpaceItem71, this.emptySpaceItem72, this.emptySpaceItem73,
			this.emptySpaceItem74, this.emptySpaceItem76, this.emptySpaceItem77, this.layoutControlItem32, this.verticaTimeoutEmptySpaceItem
		});
		this.layoutControlGroup10.Name = "Root";
		this.layoutControlGroup10.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup10.Size = new System.Drawing.Size(471, 407);
		this.layoutControlGroup10.TextVisible = false;
		this.verticaHostLayoutControlItem.Control = this.verticaHostComboBoxEdit;
		this.verticaHostLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.verticaHostLayoutControlItem.CustomizationFormText = "Host";
		this.verticaHostLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.verticaHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.verticaHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.verticaHostLayoutControlItem.Name = "verticaHostLayoutControlItem";
		this.verticaHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.verticaHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.verticaHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.verticaHostLayoutControlItem.Text = "Host:";
		this.verticaHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.verticaHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.verticaHostLayoutControlItem.TextToControlDistance = 5;
		this.verticaPortLayoutControlItem.Control = this.verticaPortTextEdit;
		this.verticaPortLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.verticaPortLayoutControlItem.CustomizationFormText = "Port";
		this.verticaPortLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.verticaPortLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.verticaPortLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.verticaPortLayoutControlItem.Name = "verticaPortLayoutControlItem";
		this.verticaPortLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.verticaPortLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.verticaPortLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.verticaPortLayoutControlItem.Text = "Port:";
		this.verticaPortLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.verticaPortLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.verticaPortLayoutControlItem.TextToControlDistance = 5;
		this.verticaUserLayoutControlItem.Control = this.verticaLoginTextEdit;
		this.verticaUserLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.verticaUserLayoutControlItem.CustomizationFormText = "User";
		this.verticaUserLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.verticaUserLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.verticaUserLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.verticaUserLayoutControlItem.Name = "verticaUserLayoutControlItem";
		this.verticaUserLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.verticaUserLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.verticaUserLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.verticaUserLayoutControlItem.Text = "User:";
		this.verticaUserLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.verticaUserLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.verticaUserLayoutControlItem.TextToControlDistance = 5;
		this.verticaPasswordLayoutControlItem.Control = this.verticaPasswordTextEdit;
		this.verticaPasswordLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.verticaPasswordLayoutControlItem.CustomizationFormText = "Password";
		this.verticaPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.verticaPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.verticaPasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.verticaPasswordLayoutControlItem.Name = "verticaPasswordLayoutControlItem";
		this.verticaPasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.verticaPasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.verticaPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.verticaPasswordLayoutControlItem.Text = "Password:";
		this.verticaPasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.verticaPasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.verticaPasswordLayoutControlItem.TextToControlDistance = 5;
		this.verticaDatabaseLayoutControlItem.Control = this.verticaDatabaseTextEdit;
		this.verticaDatabaseLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.verticaDatabaseLayoutControlItem.CustomizationFormText = "Database";
		this.verticaDatabaseLayoutControlItem.Location = new System.Drawing.Point(0, 120);
		this.verticaDatabaseLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.verticaDatabaseLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.verticaDatabaseLayoutControlItem.Name = "verticaDatabaseLayoutControlItem";
		this.verticaDatabaseLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.verticaDatabaseLayoutControlItem.Size = new System.Drawing.Size(471, 24);
		this.verticaDatabaseLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.verticaDatabaseLayoutControlItem.Text = "Database:";
		this.verticaDatabaseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.verticaDatabaseLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.verticaDatabaseLayoutControlItem.TextToControlDistance = 5;
		this.verticaSavePasswordLayoutControlItem.Control = this.verticaSavePasswordCheckEdit;
		this.verticaSavePasswordLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.verticaSavePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem18";
		this.verticaSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.verticaSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.verticaSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.verticaSavePasswordLayoutControlItem.Name = "verticaSavePasswordLayoutControlItem";
		this.verticaSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.verticaSavePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.verticaSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.verticaSavePasswordLayoutControlItem.Text = " ";
		this.verticaSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.verticaSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.verticaSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem70.AllowHotTrack = false;
		this.emptySpaceItem70.Location = new System.Drawing.Point(0, 168);
		this.emptySpaceItem70.Name = "emptySpaceItem70";
		this.emptySpaceItem70.Size = new System.Drawing.Size(471, 239);
		this.emptySpaceItem70.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem71.AllowHotTrack = false;
		this.emptySpaceItem71.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem71.Name = "emptySpaceItem71";
		this.emptySpaceItem71.Size = new System.Drawing.Size(136, 24);
		this.emptySpaceItem71.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem72.AllowHotTrack = false;
		this.emptySpaceItem72.Location = new System.Drawing.Point(383, 24);
		this.emptySpaceItem72.Name = "emptySpaceItem72";
		this.emptySpaceItem72.Size = new System.Drawing.Size(88, 24);
		this.emptySpaceItem72.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem73.AllowHotTrack = false;
		this.emptySpaceItem73.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem73.Name = "emptySpaceItem73";
		this.emptySpaceItem73.Size = new System.Drawing.Size(136, 24);
		this.emptySpaceItem73.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem74.AllowHotTrack = false;
		this.emptySpaceItem74.Location = new System.Drawing.Point(335, 72);
		this.emptySpaceItem74.Name = "emptySpaceItem74";
		this.emptySpaceItem74.Size = new System.Drawing.Size(136, 24);
		this.emptySpaceItem74.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem76.AllowHotTrack = false;
		this.emptySpaceItem76.Location = new System.Drawing.Point(335, 96);
		this.emptySpaceItem76.Name = "emptySpaceItem76";
		this.emptySpaceItem76.Size = new System.Drawing.Size(136, 24);
		this.emptySpaceItem76.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem77.AllowHotTrack = false;
		this.emptySpaceItem77.Location = new System.Drawing.Point(335, 24);
		this.emptySpaceItem77.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem77.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem77.Name = "emptySpaceItem77";
		this.emptySpaceItem77.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem77.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem77.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem32.Control = this.verticaDefaultPortLabelControl;
		this.layoutControlItem32.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.layoutControlItem32.CustomizationFormText = "layoutControlItem32";
		this.layoutControlItem32.Location = new System.Drawing.Point(345, 24);
		this.layoutControlItem32.MaxSize = new System.Drawing.Size(38, 24);
		this.layoutControlItem32.MinSize = new System.Drawing.Size(38, 24);
		this.layoutControlItem32.Name = "layoutControlItem32";
		this.layoutControlItem32.Size = new System.Drawing.Size(38, 24);
		this.layoutControlItem32.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem32.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem32.TextVisible = false;
		this.verticaTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.verticaTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 144);
		this.verticaTimeoutEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 24);
		this.verticaTimeoutEmptySpaceItem.MinSize = new System.Drawing.Size(104, 24);
		this.verticaTimeoutEmptySpaceItem.Name = "verticaTimeoutEmptySpaceItem";
		this.verticaTimeoutEmptySpaceItem.Size = new System.Drawing.Size(471, 24);
		this.verticaTimeoutEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.verticaTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.verticaLayoutControl);
		base.Name = "VerticaConnectorControl";
		base.Size = new System.Drawing.Size(471, 407);
		((System.ComponentModel.ISupportInitialize)this.verticaLayoutControl).EndInit();
		this.verticaLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.verticaHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.verticaPortTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.verticaLoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.verticaPasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.verticaDatabaseTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.verticaSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.verticaHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.verticaPortLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.verticaUserLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.verticaPasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.verticaDatabaseLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.verticaSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem70).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem71).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem72).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem73).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem74).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem76).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem77).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem32).EndInit();
		((System.ComponentModel.ISupportInitialize)this.verticaTimeoutEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
	}
}
