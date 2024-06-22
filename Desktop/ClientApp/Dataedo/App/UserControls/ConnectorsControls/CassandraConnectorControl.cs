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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class CassandraConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl cassandraLayoutControl;

	private LabelControl cassandraDefaultPortLabelControl;

	private ButtonEdit cassandraDatabaseButtonEdit;

	private TextEdit cassandraPortTextEdit;

	private ComboBoxEdit cassandraHostComboBoxEdit;

	private TextEdit cassandraPasswordTextEdit;

	private TextEdit cassandraUserTextEdit;

	private CheckEdit cassandraPasswordCheckEdit;

	private LayoutControlGroup cassandraLayoutControlGroup;

	private EmptySpaceItem cassandraEmptySpaceItem;

	private LayoutControlItem cassandraUserLayoutControlItem;

	private LayoutControlItem cassandraPasswordLayoutControlItem;

	private LayoutControlItem cassandraHostLayoutControlItem;

	private LayoutControlItem cassandraPortLayoutControlItem;

	private EmptySpaceItem cassandraHostLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem cassandraUserLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem cassandraPasswordLayoutControlItemEmptySpaceItem;

	private LayoutControlItem cassandraSavePasswordLayoutControlItem;

	private EmptySpaceItem cassandraSavePasswordLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem cassandraPortLayoutControlItemEmptySpaceItem;

	private LayoutControlItem cassandraDatabaseLayoutControlItem;

	private EmptySpaceItem cassandraDatabaseLayoutControlItemEmptySpaceItem;

	private LayoutControlItem cassandraDefaultPortLabelControlLayoutControlItem;

	private EmptySpaceItem cassandraDefaultPortEmptySpaceItem;

	private string providedCassandraHost => splittedHost?.Host ?? cassandraHostComboBoxEdit.Text;

	private string providedCassandraPort => cassandraPortTextEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Cassandra;

	protected override CheckEdit SavePasswordCheckEdit => cassandraPasswordCheckEdit;

	protected override TextEdit HostTextEdit => cassandraHostComboBoxEdit;

	protected override TextEdit PortTextEdit => cassandraPortTextEdit;

	protected override ComboBoxEdit HostComboBoxEdit => cassandraHostComboBoxEdit;

	public CassandraConnectorControl()
	{
		InitializeComponent();
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
		if (!isDatabaseUpdating && !base.IsCopyingConnection)
		{
			cassandraPortTextEdit.Text = DatabaseTypeEnum.GetDefaultPort(base.SelectedDatabaseType);
		}
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? cassandraDatabaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedCassandraHost, cassandraUserTextEdit.Text, cassandraPasswordTextEdit.Text, providedCassandraPort, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion);
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
		flag &= ValidateCassandraHost();
		flag &= ValidateCassandraPort();
		if (!testForGettingDatabasesList)
		{
			flag &= ValidateCassandraDatabase();
		}
		return flag;
	}

	private bool ValidateCassandraHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(cassandraHostComboBoxEdit, addDBErrorProvider, "host", acceptEmptyValue);
	}

	private bool ValidateCassandraPort(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(cassandraPortTextEdit, addDBErrorProvider, "port", acceptEmptyValue);
	}

	private bool ValidateCassandraDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(cassandraDatabaseButtonEdit, addDBErrorProvider, "keyspace", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		cassandraDatabaseButtonEdit.Text = base.DatabaseRow.Name;
		cassandraHostComboBoxEdit.Text = base.DatabaseRow.Host;
		cassandraPortTextEdit.Text = PrepareValue.ToString(base.DatabaseRow.Port) ?? DatabaseTypeEnum.GetDefaultPort(base.DatabaseRow.Type);
		cassandraUserTextEdit.Text = base.DatabaseRow.User;
		cassandraPasswordTextEdit.Text = base.DatabaseRow.Password;
		cassandraPasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return cassandraDatabaseButtonEdit.Text + "@" + cassandraHostComboBoxEdit.Text;
	}

	private void cassandraDefaultPortLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			cassandraPortTextEdit.Text = DatabaseTypeEnum.GetDefaultPort(base.SelectedDatabaseType);
		}
	}

	private void cassandraDatabaseButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		DatabaseButtonEdit_ButtonClick(sender, e);
	}

	private void cassandraHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void cassandraHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	private void cassandraPortTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		PortTextEdit_EditValueChanged(sender, e);
	}

	private void cassandraPortTextEdit_Leave(object sender, EventArgs e)
	{
		PortTextEdit_Leave(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		cassandraUserTextEdit.Text = string.Empty;
		cassandraPasswordTextEdit.Text = string.Empty;
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
		this.cassandraDefaultPortLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.cassandraDatabaseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.cassandraPortTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.cassandraHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.cassandraPasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.cassandraUserTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.cassandraPasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.cassandraLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.cassandraEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.cassandraUserLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cassandraPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cassandraHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cassandraPortLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cassandraHostLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.cassandraUserLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.cassandraPasswordLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.cassandraSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cassandraSavePasswordLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.cassandraPortLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.cassandraDatabaseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cassandraDatabaseLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.cassandraDefaultPortLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cassandraDefaultPortEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.cassandraLayoutControl).BeginInit();
		this.cassandraLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cassandraDatabaseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPortTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraUserTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraUserLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPortLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraHostLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraUserLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPasswordLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraSavePasswordLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPortLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraDatabaseLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraDatabaseLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraDefaultPortLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraDefaultPortEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.cassandraLayoutControl.AllowCustomization = false;
		this.cassandraLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.cassandraLayoutControl.Controls.Add(this.cassandraDefaultPortLabelControl);
		this.cassandraLayoutControl.Controls.Add(this.cassandraDatabaseButtonEdit);
		this.cassandraLayoutControl.Controls.Add(this.cassandraPortTextEdit);
		this.cassandraLayoutControl.Controls.Add(this.cassandraHostComboBoxEdit);
		this.cassandraLayoutControl.Controls.Add(this.cassandraPasswordTextEdit);
		this.cassandraLayoutControl.Controls.Add(this.cassandraUserTextEdit);
		this.cassandraLayoutControl.Controls.Add(this.cassandraPasswordCheckEdit);
		this.cassandraLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.cassandraLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.cassandraLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.cassandraLayoutControl.Name = "cassandraLayoutControl";
		this.cassandraLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3451, 236, 800, 771);
		this.cassandraLayoutControl.Root = this.cassandraLayoutControlGroup;
		this.cassandraLayoutControl.Size = new System.Drawing.Size(492, 239);
		this.cassandraLayoutControl.TabIndex = 3;
		this.cassandraLayoutControl.Text = "layoutControl3";
		this.cassandraDefaultPortLabelControl.AllowHtmlString = true;
		this.cassandraDefaultPortLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.cassandraDefaultPortLabelControl.Location = new System.Drawing.Point(346, 26);
		this.cassandraDefaultPortLabelControl.MinimumSize = new System.Drawing.Size(34, 20);
		this.cassandraDefaultPortLabelControl.Name = "cassandraDefaultPortLabelControl";
		this.cassandraDefaultPortLabelControl.Size = new System.Drawing.Size(34, 20);
		this.cassandraDefaultPortLabelControl.StyleController = this.cassandraLayoutControl;
		this.cassandraDefaultPortLabelControl.TabIndex = 33;
		this.cassandraDefaultPortLabelControl.Text = "<href>default</href>";
		this.cassandraDefaultPortLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(cassandraDefaultPortLabelControl_HyperlinkClick);
		this.cassandraDatabaseButtonEdit.Location = new System.Drawing.Point(105, 120);
		this.cassandraDatabaseButtonEdit.Name = "cassandraDatabaseButtonEdit";
		this.cassandraDatabaseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.cassandraDatabaseButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.cassandraDatabaseButtonEdit.StyleController = this.cassandraLayoutControl;
		this.cassandraDatabaseButtonEdit.TabIndex = 10;
		this.cassandraDatabaseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(cassandraDatabaseButtonEdit_ButtonClick);
		this.cassandraPortTextEdit.EditValue = "9042";
		this.cassandraPortTextEdit.Location = new System.Drawing.Point(105, 24);
		this.cassandraPortTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.cassandraPortTextEdit.Name = "cassandraPortTextEdit";
		this.cassandraPortTextEdit.Properties.Mask.EditMask = "\\d+";
		this.cassandraPortTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.cassandraPortTextEdit.Properties.Mask.ShowPlaceHolders = false;
		this.cassandraPortTextEdit.Properties.MaxLength = 5;
		this.cassandraPortTextEdit.Size = new System.Drawing.Size(230, 20);
		this.cassandraPortTextEdit.StyleController = this.cassandraLayoutControl;
		this.cassandraPortTextEdit.TabIndex = 8;
		this.cassandraPortTextEdit.EditValueChanged += new System.EventHandler(cassandraPortTextEdit_EditValueChanged);
		this.cassandraPortTextEdit.Leave += new System.EventHandler(cassandraPortTextEdit_Leave);
		this.cassandraHostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.cassandraHostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.cassandraHostComboBoxEdit.Name = "cassandraHostComboBoxEdit";
		this.cassandraHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cassandraHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.cassandraHostComboBoxEdit.StyleController = this.cassandraLayoutControl;
		this.cassandraHostComboBoxEdit.TabIndex = 7;
		this.cassandraHostComboBoxEdit.EditValueChanged += new System.EventHandler(cassandraHostComboBoxEdit_EditValueChanged);
		this.cassandraHostComboBoxEdit.Leave += new System.EventHandler(cassandraHostComboBoxEdit_Leave);
		this.cassandraPasswordTextEdit.Location = new System.Drawing.Point(105, 72);
		this.cassandraPasswordTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.cassandraPasswordTextEdit.Name = "cassandraPasswordTextEdit";
		this.cassandraPasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.cassandraPasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.cassandraPasswordTextEdit.StyleController = this.cassandraLayoutControl;
		this.cassandraPasswordTextEdit.TabIndex = 6;
		this.cassandraUserTextEdit.Location = new System.Drawing.Point(105, 48);
		this.cassandraUserTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.cassandraUserTextEdit.Name = "cassandraUserTextEdit";
		this.cassandraUserTextEdit.Size = new System.Drawing.Size(230, 20);
		this.cassandraUserTextEdit.StyleController = this.cassandraLayoutControl;
		this.cassandraUserTextEdit.TabIndex = 5;
		this.cassandraPasswordCheckEdit.Location = new System.Drawing.Point(105, 96);
		this.cassandraPasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.cassandraPasswordCheckEdit.Name = "cassandraPasswordCheckEdit";
		this.cassandraPasswordCheckEdit.Properties.Caption = "Save password";
		this.cassandraPasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.cassandraPasswordCheckEdit.StyleController = this.cassandraLayoutControl;
		this.cassandraPasswordCheckEdit.TabIndex = 3;
		this.cassandraLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.cassandraLayoutControlGroup.GroupBordersVisible = false;
		this.cassandraLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[15]
		{
			this.cassandraEmptySpaceItem, this.cassandraUserLayoutControlItem, this.cassandraPasswordLayoutControlItem, this.cassandraHostLayoutControlItem, this.cassandraPortLayoutControlItem, this.cassandraHostLayoutControlItemEmptySpaceItem, this.cassandraUserLayoutControlItemEmptySpaceItem, this.cassandraPasswordLayoutControlItemEmptySpaceItem, this.cassandraSavePasswordLayoutControlItem, this.cassandraSavePasswordLayoutControlItemEmptySpaceItem,
			this.cassandraPortLayoutControlItemEmptySpaceItem, this.cassandraDatabaseLayoutControlItem, this.cassandraDatabaseLayoutControlItemEmptySpaceItem, this.cassandraDefaultPortLabelControlLayoutControlItem, this.cassandraDefaultPortEmptySpaceItem
		});
		this.cassandraLayoutControlGroup.Name = "Root";
		this.cassandraLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.cassandraLayoutControlGroup.Size = new System.Drawing.Size(492, 239);
		this.cassandraLayoutControlGroup.TextVisible = false;
		this.cassandraEmptySpaceItem.AllowHotTrack = false;
		this.cassandraEmptySpaceItem.Location = new System.Drawing.Point(0, 144);
		this.cassandraEmptySpaceItem.Name = "cassandraEmptySpaceItem";
		this.cassandraEmptySpaceItem.Size = new System.Drawing.Size(492, 95);
		this.cassandraEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.cassandraUserLayoutControlItem.Control = this.cassandraUserTextEdit;
		this.cassandraUserLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.cassandraUserLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.cassandraUserLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.cassandraUserLayoutControlItem.Name = "cassandraUserLayoutControlItem";
		this.cassandraUserLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.cassandraUserLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.cassandraUserLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cassandraUserLayoutControlItem.Text = "User";
		this.cassandraUserLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.cassandraUserLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.cassandraUserLayoutControlItem.TextToControlDistance = 5;
		this.cassandraPasswordLayoutControlItem.Control = this.cassandraPasswordTextEdit;
		this.cassandraPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.cassandraPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.cassandraPasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.cassandraPasswordLayoutControlItem.Name = "cassandraPasswordLayoutControlItem";
		this.cassandraPasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.cassandraPasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.cassandraPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cassandraPasswordLayoutControlItem.Text = "Password";
		this.cassandraPasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.cassandraPasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.cassandraPasswordLayoutControlItem.TextToControlDistance = 5;
		this.cassandraHostLayoutControlItem.Control = this.cassandraHostComboBoxEdit;
		this.cassandraHostLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.cassandraHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.cassandraHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.cassandraHostLayoutControlItem.Name = "cassandraHostLayoutControlItem";
		this.cassandraHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.cassandraHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.cassandraHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cassandraHostLayoutControlItem.Text = "Host";
		this.cassandraHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.cassandraHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.cassandraHostLayoutControlItem.TextToControlDistance = 5;
		this.cassandraPortLayoutControlItem.Control = this.cassandraPortTextEdit;
		this.cassandraPortLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.cassandraPortLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.cassandraPortLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.cassandraPortLayoutControlItem.Name = "cassandraPortLayoutControlItem";
		this.cassandraPortLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.cassandraPortLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.cassandraPortLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cassandraPortLayoutControlItem.Text = "Port";
		this.cassandraPortLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.cassandraPortLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.cassandraPortLayoutControlItem.TextToControlDistance = 5;
		this.cassandraHostLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.cassandraHostLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 0);
		this.cassandraHostLayoutControlItemEmptySpaceItem.Name = "cassandraHostLayoutControlItemEmptySpaceItem";
		this.cassandraHostLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(157, 24);
		this.cassandraHostLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.cassandraUserLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.cassandraUserLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 48);
		this.cassandraUserLayoutControlItemEmptySpaceItem.Name = "cassandraUserLayoutControlItemEmptySpaceItem";
		this.cassandraUserLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(157, 24);
		this.cassandraUserLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.cassandraPasswordLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.cassandraPasswordLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 72);
		this.cassandraPasswordLayoutControlItemEmptySpaceItem.Name = "cassandraPasswordLayoutControlItemEmptySpaceItem";
		this.cassandraPasswordLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(157, 24);
		this.cassandraPasswordLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.cassandraSavePasswordLayoutControlItem.Control = this.cassandraPasswordCheckEdit;
		this.cassandraSavePasswordLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.cassandraSavePasswordLayoutControlItem.CustomizationFormText = "mongoDbSavePasswordLayoutControlItem";
		this.cassandraSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 96);
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
		this.cassandraSavePasswordLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 96);
		this.cassandraSavePasswordLayoutControlItemEmptySpaceItem.Name = "cassandraSavePasswordLayoutControlItemEmptySpaceItem";
		this.cassandraSavePasswordLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(157, 24);
		this.cassandraSavePasswordLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.cassandraPortLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.cassandraPortLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 24);
		this.cassandraPortLayoutControlItemEmptySpaceItem.MaxSize = new System.Drawing.Size(10, 24);
		this.cassandraPortLayoutControlItemEmptySpaceItem.MinSize = new System.Drawing.Size(10, 24);
		this.cassandraPortLayoutControlItemEmptySpaceItem.Name = "cassandraPortLayoutControlItemEmptySpaceItem";
		this.cassandraPortLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(10, 24);
		this.cassandraPortLayoutControlItemEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cassandraPortLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.cassandraDatabaseLayoutControlItem.Control = this.cassandraDatabaseButtonEdit;
		this.cassandraDatabaseLayoutControlItem.Location = new System.Drawing.Point(0, 120);
		this.cassandraDatabaseLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.cassandraDatabaseLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.cassandraDatabaseLayoutControlItem.Name = "cassandraDatabaseLayoutControlItem";
		this.cassandraDatabaseLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.cassandraDatabaseLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.cassandraDatabaseLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cassandraDatabaseLayoutControlItem.Text = "Keyspace";
		this.cassandraDatabaseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.cassandraDatabaseLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.cassandraDatabaseLayoutControlItem.TextToControlDistance = 5;
		this.cassandraDatabaseLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.cassandraDatabaseLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 120);
		this.cassandraDatabaseLayoutControlItemEmptySpaceItem.Name = "cassandraDatabaseLayoutControlItemEmptySpaceItem";
		this.cassandraDatabaseLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(157, 24);
		this.cassandraDatabaseLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.cassandraDefaultPortLabelControlLayoutControlItem.Control = this.cassandraDefaultPortLabelControl;
		this.cassandraDefaultPortLabelControlLayoutControlItem.Location = new System.Drawing.Point(345, 24);
		this.cassandraDefaultPortLabelControlLayoutControlItem.Name = "cassandraDefaultPortLabelControlLayoutControlItem";
		this.cassandraDefaultPortLabelControlLayoutControlItem.Size = new System.Drawing.Size(36, 24);
		this.cassandraDefaultPortLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.cassandraDefaultPortLabelControlLayoutControlItem.TextVisible = false;
		this.cassandraDefaultPortEmptySpaceItem.AllowHotTrack = false;
		this.cassandraDefaultPortEmptySpaceItem.Location = new System.Drawing.Point(381, 24);
		this.cassandraDefaultPortEmptySpaceItem.Name = "cassandraDefaultPortEmptySpaceItem";
		this.cassandraDefaultPortEmptySpaceItem.Size = new System.Drawing.Size(111, 24);
		this.cassandraDefaultPortEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.cassandraLayoutControl);
		base.Name = "CassandraConnectorControl";
		base.Size = new System.Drawing.Size(492, 239);
		((System.ComponentModel.ISupportInitialize)this.cassandraLayoutControl).EndInit();
		this.cassandraLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cassandraDatabaseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPortTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraUserTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraUserLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPortLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraHostLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraUserLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPasswordLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraSavePasswordLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraPortLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraDatabaseLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraDatabaseLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraDefaultPortLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cassandraDefaultPortEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
	}
}
