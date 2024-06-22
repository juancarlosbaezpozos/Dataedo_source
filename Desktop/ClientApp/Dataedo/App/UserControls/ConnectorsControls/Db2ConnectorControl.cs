using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.Data.Odbc;
using Dataedo.DataProcessing.Classes;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class Db2ConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl db2LayoutControl;

	private LabelControl labelControl2;

	private TextEdit db2PortTextEdit;

	private TextEdit db2LoginTextEdit;

	private TextEdit db2PasswordTextEdit;

	private CheckEdit db2SavePasswordCheckEdit;

	private ComboBoxEdit db2HostComboBoxEdit;

	private ButtonEdit db2InstanceButtonEdit;

	private TextEdit db2DatabaseTextEdit;

	private LayoutControlGroup layoutControlGroup8;

	private LayoutControlItem layoutControlItem6;

	private LayoutControlItem layoutControlItem7;

	private LayoutControlItem layoutControlItem8;

	private LayoutControlItem layoutControlItem13;

	private EmptySpaceItem emptySpaceItem54;

	private EmptySpaceItem emptySpaceItem55;

	private EmptySpaceItem emptySpaceItem56;

	private EmptySpaceItem emptySpaceItem58;

	private LayoutControlItem layoutControlItem18;

	private EmptySpaceItem emptySpaceItem61;

	private LayoutControlItem layoutControlItem19;

	private EmptySpaceItem emptySpaceItem62;

	private LayoutControlItem mySqlDatabaseLayoutControlItem1;

	private LayoutControlItem mySqlPasswordLayoutControlItem1;

	private EmptySpaceItem db2TimeoutEmptySpaceItem;

	private EmptySpaceItem emptySpaceItem66;

	private EmptySpaceItem emptySpaceItem68;

	private string providedDb2Host => splittedHost?.Host ?? db2HostComboBoxEdit.Text;

	private string providedDb2Port => db2PortTextEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Db2;

	protected override TextEdit HostTextEdit => db2HostComboBoxEdit;

	protected override TextEdit PortTextEdit => db2PortTextEdit;

	protected override ComboBoxEdit HostComboBoxEdit => db2HostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => db2SavePasswordCheckEdit;

	public Db2ConnectorControl()
	{
		InitializeComponent();
	}

	protected override void SetAuthenticationDataSource()
	{
		if (string.IsNullOrEmpty(db2InstanceButtonEdit.Text))
		{
			db2InstanceButtonEdit.Text = GetDb2Instances().FirstOrDefault();
		}
	}

	public override void SetSwitchDatabaseAvailability(bool isAvailable)
	{
		db2DatabaseTextEdit.Enabled = isAvailable;
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? db2DatabaseTextEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedDb2Host, db2LoginTextEdit.Text, db2PasswordTextEdit.Text, providedDb2Port, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, db2InstanceButtonEdit.Text, null, null, null, null);
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		db2LayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			db2LayoutControl.Root.AddItem(timeoutLayoutControlItem, db2TimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateDb2Instance();
		flag &= ValidateDb2Login();
		flag &= ValidateDb2Port();
		flag &= ValidateDb2Password();
		if (!testForGettingDatabasesList)
		{
			flag &= ValidateDb2Database();
		}
		return flag;
	}

	private bool ValidateDb2Instance(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(db2InstanceButtonEdit, addDBErrorProvider, "instance", acceptEmptyValue);
	}

	private bool ValidateDb2Password(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(db2PasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateDb2Login(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(db2LoginTextEdit, addDBErrorProvider, "user", acceptEmptyValue);
	}

	private bool ValidateDb2Port(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(db2PortTextEdit, addDBErrorProvider, "port", acceptEmptyValue);
	}

	private bool ValidateDb2Database(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(db2DatabaseTextEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		db2HostComboBoxEdit.Text = base.DatabaseRow.Host;
		db2DatabaseTextEdit.EditValue = base.DatabaseRow.Name;
		db2PortTextEdit.Text = PrepareValue.ToString(base.DatabaseRow.Port) ?? DatabaseTypeEnum.GetDefaultPort(base.DatabaseRow.Type);
		db2LoginTextEdit.Text = base.DatabaseRow.User;
		db2PasswordTextEdit.Text = base.DatabaseRow.Password;
		db2SavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
		db2InstanceButtonEdit.Text = base.DatabaseRow.ServiceName;
	}

	protected override string GetPanelDocumentationTitle()
	{
		return db2DatabaseTextEdit.Text + "@" + providedDb2Host;
	}

	private void db2PortTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateDb2Port(acceptEmptyValue: true);
		PortTextEdit_EditValueChanged(sender, e);
	}

	private void db2LoginTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateDb2Login(acceptEmptyValue: true);
	}

	private void db2DatabaseTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateDb2Database(acceptEmptyValue: true);
	}

	private void db2DefaultPortLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			db2PortTextEdit.Text = DatabaseTypeEnum.GetDefaultPort(base.DatabaseRow.Type);
		}
	}

	private void db2InstanceButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
		SetNewDBRowValues(forGettingDatabasesList: true);
		IEnumerable<string> db2Instances = GetDb2Instances();
		CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
		if (db2Instances == null || db2Instances.Count() == 0)
		{
			GeneralMessageBoxesHandling.Show("64-bit IBM Db2 ODBC driver not found." + Environment.NewLine + "To connect to Db2, install the 64-bit <href=https://www-01.ibm.com/support/docview.wss?uid=swg21385217>IBM Data Server Driver Package</href>, or run Dataedo 32-bit if you have the 32-bit ODBC driver installed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
			return;
		}
		string title = "IBM DB2 Drivers";
		ListForm listForm = new ListForm(db2Instances, title);
		if (listForm.ShowDialog(this, setCustomMessageDefaultOwner: true) == DialogResult.OK)
		{
			(sender as ButtonEdit).EditValue = listForm.SelectedValue;
		}
	}

	private void db2PasswordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateDb2Password();
	}

	private void db2HostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void db2HostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		db2LoginTextEdit.Text = string.Empty;
		db2PasswordTextEdit.Text = string.Empty;
	}

	private IEnumerable<string> GetDb2Instances()
	{
		return (from x in Dataedo.Data.Odbc.DataSources.GetDrivers()
			where x.StartsWith("IBM DB2 ODBC DRIVER")
			select x).ToArray();
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
		this.db2LayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.db2PortTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.db2LoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.db2PasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.db2SavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.db2HostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.db2InstanceButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.db2DatabaseTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControlGroup8 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem54 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem55 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem56 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem58 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem61 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem19 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem62 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mySqlDatabaseLayoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.mySqlPasswordLayoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.db2TimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem66 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem68 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.db2LayoutControl).BeginInit();
		this.db2LayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.db2PortTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.db2LoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.db2PasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.db2SavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.db2HostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.db2InstanceButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.db2DatabaseTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem54).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem55).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem56).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem58).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem61).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem19).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem62).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlDatabaseLayoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlPasswordLayoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.db2TimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem66).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem68).BeginInit();
		base.SuspendLayout();
		this.db2LayoutControl.AllowCustomization = false;
		this.db2LayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.db2LayoutControl.Controls.Add(this.labelControl2);
		this.db2LayoutControl.Controls.Add(this.db2PortTextEdit);
		this.db2LayoutControl.Controls.Add(this.db2LoginTextEdit);
		this.db2LayoutControl.Controls.Add(this.db2PasswordTextEdit);
		this.db2LayoutControl.Controls.Add(this.db2SavePasswordCheckEdit);
		this.db2LayoutControl.Controls.Add(this.db2HostComboBoxEdit);
		this.db2LayoutControl.Controls.Add(this.db2InstanceButtonEdit);
		this.db2LayoutControl.Controls.Add(this.db2DatabaseTextEdit);
		this.db2LayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.db2LayoutControl.Location = new System.Drawing.Point(0, 0);
		this.db2LayoutControl.Name = "db2LayoutControl";
		this.db2LayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2548, 234, 360, 525);
		this.db2LayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.db2LayoutControl.Root = this.layoutControlGroup8;
		this.db2LayoutControl.Size = new System.Drawing.Size(448, 353);
		this.db2LayoutControl.TabIndex = 4;
		this.db2LayoutControl.Text = "layoutControl1";
		this.labelControl2.AllowHtmlString = true;
		this.labelControl2.Cursor = System.Windows.Forms.Cursors.Hand;
		this.labelControl2.Location = new System.Drawing.Point(346, 26);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(36, 20);
		this.labelControl2.StyleController = this.db2LayoutControl;
		this.labelControl2.TabIndex = 27;
		this.labelControl2.Text = "<href>default</href>";
		this.labelControl2.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(db2DefaultPortLabelControl_HyperlinkClick);
		this.db2PortTextEdit.EditValue = "50000";
		this.db2PortTextEdit.Location = new System.Drawing.Point(105, 24);
		this.db2PortTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.db2PortTextEdit.Name = "db2PortTextEdit";
		this.db2PortTextEdit.Properties.Mask.EditMask = "\\d+";
		this.db2PortTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.db2PortTextEdit.Properties.Mask.ShowPlaceHolders = false;
		this.db2PortTextEdit.Properties.MaxLength = 5;
		this.db2PortTextEdit.Size = new System.Drawing.Size(230, 20);
		this.db2PortTextEdit.StyleController = this.db2LayoutControl;
		this.db2PortTextEdit.TabIndex = 0;
		this.db2PortTextEdit.EditValueChanged += new System.EventHandler(db2PortTextEdit_EditValueChanged);
		this.db2LoginTextEdit.Location = new System.Drawing.Point(105, 72);
		this.db2LoginTextEdit.Name = "db2LoginTextEdit";
		this.db2LoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.db2LoginTextEdit.StyleController = this.db2LayoutControl;
		this.db2LoginTextEdit.TabIndex = 1;
		this.db2LoginTextEdit.EditValueChanged += new System.EventHandler(db2LoginTextEdit_EditValueChanged);
		this.db2PasswordTextEdit.Location = new System.Drawing.Point(105, 96);
		this.db2PasswordTextEdit.Name = "db2PasswordTextEdit";
		this.db2PasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.db2PasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.db2PasswordTextEdit.StyleController = this.db2LayoutControl;
		this.db2PasswordTextEdit.TabIndex = 2;
		this.db2PasswordTextEdit.EditValueChanged += new System.EventHandler(db2PasswordTextEdit_EditValueChanged);
		this.db2SavePasswordCheckEdit.Location = new System.Drawing.Point(105, 120);
		this.db2SavePasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.db2SavePasswordCheckEdit.Name = "db2SavePasswordCheckEdit";
		this.db2SavePasswordCheckEdit.Properties.Caption = "Save password";
		this.db2SavePasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.db2SavePasswordCheckEdit.StyleController = this.db2LayoutControl;
		this.db2SavePasswordCheckEdit.TabIndex = 3;
		this.db2HostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.db2HostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.db2HostComboBoxEdit.Name = "db2HostComboBoxEdit";
		this.db2HostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.db2HostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.db2HostComboBoxEdit.StyleController = this.db2LayoutControl;
		this.db2HostComboBoxEdit.TabIndex = 1;
		this.db2HostComboBoxEdit.EditValueChanged += new System.EventHandler(db2HostComboBoxEdit_EditValueChanged);
		this.db2HostComboBoxEdit.Leave += new System.EventHandler(db2HostComboBoxEdit_Leave);
		this.db2InstanceButtonEdit.Location = new System.Drawing.Point(105, 48);
		this.db2InstanceButtonEdit.Name = "db2InstanceButtonEdit";
		this.db2InstanceButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.db2InstanceButtonEdit.Properties.ReadOnly = true;
		this.db2InstanceButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.db2InstanceButtonEdit.StyleController = this.db2LayoutControl;
		this.db2InstanceButtonEdit.TabIndex = 4;
		this.db2InstanceButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(db2InstanceButtonEdit_ButtonClick);
		this.db2DatabaseTextEdit.Location = new System.Drawing.Point(105, 144);
		this.db2DatabaseTextEdit.Name = "db2DatabaseTextEdit";
		this.db2DatabaseTextEdit.Size = new System.Drawing.Size(230, 20);
		this.db2DatabaseTextEdit.StyleController = this.db2LayoutControl;
		this.db2DatabaseTextEdit.TabIndex = 2;
		this.db2DatabaseTextEdit.EditValueChanged += new System.EventHandler(db2DatabaseTextEdit_EditValueChanged);
		this.layoutControlGroup8.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup8.GroupBordersVisible = false;
		this.layoutControlGroup8.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[17]
		{
			this.layoutControlItem6, this.layoutControlItem7, this.layoutControlItem8, this.layoutControlItem13, this.emptySpaceItem54, this.emptySpaceItem55, this.emptySpaceItem56, this.emptySpaceItem58, this.layoutControlItem18, this.emptySpaceItem61,
			this.layoutControlItem19, this.emptySpaceItem62, this.mySqlDatabaseLayoutControlItem1, this.mySqlPasswordLayoutControlItem1, this.db2TimeoutEmptySpaceItem, this.emptySpaceItem66, this.emptySpaceItem68
		});
		this.layoutControlGroup8.Name = "Root";
		this.layoutControlGroup8.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup8.Size = new System.Drawing.Size(448, 353);
		this.layoutControlGroup8.TextVisible = false;
		this.layoutControlItem6.Control = this.db2PortTextEdit;
		this.layoutControlItem6.CustomizationFormText = "Port:";
		this.layoutControlItem6.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem6.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem6.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem6.Name = "mySqlPortLayoutControlItem";
		this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem6.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem6.Text = "Port:";
		this.layoutControlItem6.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem6.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem6.TextToControlDistance = 5;
		this.layoutControlItem7.Control = this.db2LoginTextEdit;
		this.layoutControlItem7.CustomizationFormText = "User:";
		this.layoutControlItem7.Location = new System.Drawing.Point(0, 72);
		this.layoutControlItem7.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem7.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem7.Name = "mySqlLoginLayoutControlItem";
		this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem7.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem7.Text = "User:";
		this.layoutControlItem7.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem7.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem7.TextToControlDistance = 5;
		this.layoutControlItem8.Control = this.db2PasswordTextEdit;
		this.layoutControlItem8.CustomizationFormText = "Password";
		this.layoutControlItem8.Location = new System.Drawing.Point(0, 96);
		this.layoutControlItem8.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem8.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem8.Name = "mySqlPasswordLayoutControlItem";
		this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem8.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem8.Text = "Password:";
		this.layoutControlItem8.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem8.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem8.TextToControlDistance = 5;
		this.layoutControlItem13.Control = this.db2SavePasswordCheckEdit;
		this.layoutControlItem13.CustomizationFormText = "layoutControlItem18";
		this.layoutControlItem13.Location = new System.Drawing.Point(0, 120);
		this.layoutControlItem13.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem13.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem13.Name = "mySqlSavePasswordLayoutControlItem";
		this.layoutControlItem13.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem13.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem13.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem13.Text = " ";
		this.layoutControlItem13.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem13.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem13.TextToControlDistance = 5;
		this.emptySpaceItem54.AllowHotTrack = false;
		this.emptySpaceItem54.Location = new System.Drawing.Point(0, 192);
		this.emptySpaceItem54.Name = "emptySpaceItem3";
		this.emptySpaceItem54.Size = new System.Drawing.Size(448, 161);
		this.emptySpaceItem54.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem55.AllowHotTrack = false;
		this.emptySpaceItem55.CustomizationFormText = "emptySpaceItem13";
		this.emptySpaceItem55.Location = new System.Drawing.Point(335, 72);
		this.emptySpaceItem55.Name = "emptySpaceItem19";
		this.emptySpaceItem55.Size = new System.Drawing.Size(113, 24);
		this.emptySpaceItem55.Text = "emptySpaceItem13";
		this.emptySpaceItem55.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem56.AllowHotTrack = false;
		this.emptySpaceItem56.CustomizationFormText = "emptySpaceItem13";
		this.emptySpaceItem56.Location = new System.Drawing.Point(383, 24);
		this.emptySpaceItem56.Name = "emptySpaceItem25";
		this.emptySpaceItem56.Size = new System.Drawing.Size(65, 24);
		this.emptySpaceItem56.Text = "emptySpaceItem13";
		this.emptySpaceItem56.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem58.AllowHotTrack = false;
		this.emptySpaceItem58.CustomizationFormText = "emptySpaceItem13";
		this.emptySpaceItem58.Location = new System.Drawing.Point(335, 96);
		this.emptySpaceItem58.Name = "emptySpaceItem27";
		this.emptySpaceItem58.Size = new System.Drawing.Size(113, 24);
		this.emptySpaceItem58.Text = "emptySpaceItem13";
		this.emptySpaceItem58.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem18.Control = this.labelControl2;
		this.layoutControlItem18.Location = new System.Drawing.Point(345, 24);
		this.layoutControlItem18.MaxSize = new System.Drawing.Size(38, 24);
		this.layoutControlItem18.MinSize = new System.Drawing.Size(38, 24);
		this.layoutControlItem18.Name = "layoutControlItem32";
		this.layoutControlItem18.Size = new System.Drawing.Size(38, 24);
		this.layoutControlItem18.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem18.TextVisible = false;
		this.emptySpaceItem61.AllowHotTrack = false;
		this.emptySpaceItem61.Location = new System.Drawing.Point(335, 24);
		this.emptySpaceItem61.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem61.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem61.Name = "emptySpaceItem48";
		this.emptySpaceItem61.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem61.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem61.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem19.Control = this.db2HostComboBoxEdit;
		this.layoutControlItem19.CustomizationFormText = "Host:";
		this.layoutControlItem19.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem19.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem19.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem19.Name = "mySqlHostLayoutControlItem";
		this.layoutControlItem19.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem19.Size = new System.Drawing.Size(335, 24);
		this.layoutControlItem19.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem19.Text = "Host:";
		this.layoutControlItem19.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem19.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem19.TextToControlDistance = 5;
		this.emptySpaceItem62.AllowHotTrack = false;
		this.emptySpaceItem62.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem62.Name = "emptySpaceItem45";
		this.emptySpaceItem62.Size = new System.Drawing.Size(113, 24);
		this.emptySpaceItem62.TextSize = new System.Drawing.Size(0, 0);
		this.mySqlDatabaseLayoutControlItem1.Control = this.db2InstanceButtonEdit;
		this.mySqlDatabaseLayoutControlItem1.CustomizationFormText = "Database:";
		this.mySqlDatabaseLayoutControlItem1.Location = new System.Drawing.Point(0, 48);
		this.mySqlDatabaseLayoutControlItem1.MaxSize = new System.Drawing.Size(335, 24);
		this.mySqlDatabaseLayoutControlItem1.MinSize = new System.Drawing.Size(335, 24);
		this.mySqlDatabaseLayoutControlItem1.Name = "mySqlDatabaseLayoutControlItem1";
		this.mySqlDatabaseLayoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mySqlDatabaseLayoutControlItem1.Size = new System.Drawing.Size(335, 24);
		this.mySqlDatabaseLayoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mySqlDatabaseLayoutControlItem1.Text = "Driver";
		this.mySqlDatabaseLayoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mySqlDatabaseLayoutControlItem1.TextSize = new System.Drawing.Size(100, 13);
		this.mySqlDatabaseLayoutControlItem1.TextToControlDistance = 5;
		this.mySqlPasswordLayoutControlItem1.Control = this.db2DatabaseTextEdit;
		this.mySqlPasswordLayoutControlItem1.CustomizationFormText = "Password";
		this.mySqlPasswordLayoutControlItem1.Location = new System.Drawing.Point(0, 144);
		this.mySqlPasswordLayoutControlItem1.MaxSize = new System.Drawing.Size(335, 24);
		this.mySqlPasswordLayoutControlItem1.MinSize = new System.Drawing.Size(335, 24);
		this.mySqlPasswordLayoutControlItem1.Name = "mySqlPasswordLayoutControlItem1";
		this.mySqlPasswordLayoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mySqlPasswordLayoutControlItem1.Size = new System.Drawing.Size(448, 24);
		this.mySqlPasswordLayoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mySqlPasswordLayoutControlItem1.Text = "Database:";
		this.mySqlPasswordLayoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mySqlPasswordLayoutControlItem1.TextSize = new System.Drawing.Size(100, 13);
		this.mySqlPasswordLayoutControlItem1.TextToControlDistance = 5;
		this.db2TimeoutEmptySpaceItem.AllowHotTrack = false;
		this.db2TimeoutEmptySpaceItem.CustomizationFormText = "emptySpaceItem12";
		this.db2TimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 168);
		this.db2TimeoutEmptySpaceItem.MaxSize = new System.Drawing.Size(405, 24);
		this.db2TimeoutEmptySpaceItem.MinSize = new System.Drawing.Size(405, 24);
		this.db2TimeoutEmptySpaceItem.Name = "db2TimeoutEmptySpaceItem";
		this.db2TimeoutEmptySpaceItem.Size = new System.Drawing.Size(448, 24);
		this.db2TimeoutEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.db2TimeoutEmptySpaceItem.Text = "emptySpaceItem12";
		this.db2TimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem66.AllowHotTrack = false;
		this.emptySpaceItem66.Location = new System.Drawing.Point(335, 120);
		this.emptySpaceItem66.Name = "emptySpaceItem66";
		this.emptySpaceItem66.Size = new System.Drawing.Size(113, 24);
		this.emptySpaceItem66.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem68.AllowHotTrack = false;
		this.emptySpaceItem68.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem68.Name = "emptySpaceItem68";
		this.emptySpaceItem68.Size = new System.Drawing.Size(113, 24);
		this.emptySpaceItem68.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.db2LayoutControl);
		base.Name = "Db2ConnectorControl";
		base.Size = new System.Drawing.Size(448, 353);
		((System.ComponentModel.ISupportInitialize)this.db2LayoutControl).EndInit();
		this.db2LayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.db2PortTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.db2LoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.db2PasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.db2SavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.db2HostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.db2InstanceButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.db2DatabaseTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem54).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem55).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem56).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem58).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem61).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem19).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem62).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlDatabaseLayoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlPasswordLayoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.db2TimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem66).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem68).EndInit();
		base.ResumeLayout(false);
	}
}
