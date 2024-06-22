using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class MySQLConnectorControl : ConnectorControlBase
{
	private IDatabaseSupport _databaseSupport;

	private IContainer components;

	private NonCustomizableLayoutControl mysqlLayoutControl;

	private SimpleButton configureSimpleButton;

	private LabelControl mySqlDefaultPortLabelControl;

	private TextEdit mySqlPortTextEdit;

	private TextEdit mySqlLoginTextEdit;

	private TextEdit mySqlPasswordTextEdit;

	private CheckEdit mySqlSavePasswordCheckEdit;

	private ComboBoxEdit mySqlHostComboBoxEdit;

	private LayoutControlGroup layoutControlGroup6;

	private LayoutControlItem mySqlPortLayoutControlItem;

	private LayoutControlItem mySqlLoginLayoutControlItem;

	private LayoutControlItem mySqlPasswordLayoutControlItem;

	private LayoutControlItem mySqlSavePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem34;

	private EmptySpaceItem emptySpaceItem35;

	private EmptySpaceItem emptySpaceItem36;

	private EmptySpaceItem emptySpaceItem37;

	private EmptySpaceItem emptySpaceItem38;

	private LayoutControlItem mySqlDatabaseLayoutControlItem;

	private LayoutControlItem layoutControlItem44;

	private EmptySpaceItem mySqlTimeoutEmptySpaceItem;

	private EmptySpaceItem emptySpaceItem48;

	private LayoutControlItem mySqlHostLayoutControlItem;

	private EmptySpaceItem emptySpaceItem45;

	private LayoutControlItem configureSSLLayoutControlItem;

	private EmptySpaceItem configureEmptySpaceItem;

	protected ButtonEdit mySqlDatabaseButtonEdit;

	private LookUpEdit sslModeLookUpEdit;

	private LayoutControlItem sslModeLayoutControlItem;

	protected NonCustomizableLayoutControl layoutControl => mysqlLayoutControl;

	protected LayoutControlItem databaseLayoutItem => mySqlDatabaseLayoutControlItem;

	protected string providedMySQLHost => splittedHost?.Host ?? mySqlHostComboBoxEdit.Text;

	protected string providedMySQLPort => mySqlPortTextEdit.Text;

	protected string providedMySQLLogin => mySqlLoginTextEdit.Text;

	protected string providedMySQLPassword => mySqlPasswordTextEdit.Text;

	protected string providedMySQLDatabase => mySqlDatabaseButtonEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.MySQL;

	private IDatabaseSupport DatabaseSupport
	{
		get
		{
			if (_databaseSupport == null)
			{
				_databaseSupport = DatabaseSupportFactory.GetDatabaseSupport(base.SelectedDatabaseType);
			}
			return _databaseSupport;
		}
	}

	protected override TextEdit HostTextEdit => mySqlHostComboBoxEdit;

	protected override TextEdit PortTextEdit => mySqlPortTextEdit;

	protected override ComboBoxEdit HostComboBoxEdit => mySqlHostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => mySqlSavePasswordCheckEdit;

	public MySQLConnectorControl()
	{
		InitializeComponent();
	}

	protected override void SetAuthenticationDataSource()
	{
		SetConfigureButtonVisibility(MySqlSupport.IsDatabaseTypeMySqlFork(base.SelectedDatabaseType));
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		SetSslTypes();
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
	}

	private void SetSslTypes()
	{
		if (DatabaseSupport.HasSslSettings)
		{
			Dictionary<SSLTypeEnum.SSLType, string> mySqlSSLTypes = SSLTypeEnum.GetMySqlSSLTypes();
			sslModeLookUpEdit.Properties.DataSource = mySqlSSLTypes;
			sslModeLookUpEdit.Properties.DropDownRows = mySqlSSLTypes.Count;
			sslModeLayoutControlItem.Visibility = LayoutVisibility.Always;
			if (sslModeLookUpEdit.EditValue == null)
			{
				sslModeLookUpEdit.EditValue = SSLTypeEnum.SSLType.Prefer;
			}
		}
		else
		{
			sslModeLayoutControlItem.Visibility = LayoutVisibility.Never;
		}
	}

	private void SetConfigureButtonVisibility(bool condition)
	{
		LayoutVisibility layoutVisibility3 = (configureSSLLayoutControlItem.Visibility = (configureEmptySpaceItem.Visibility = ((!condition) ? LayoutVisibility.Never : LayoutVisibility.Always)));
	}

	public override void SetSwitchDatabaseAvailability(bool isAvailable)
	{
		mySqlDatabaseButtonEdit.Enabled = isAvailable;
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		if (DatabaseSupportFactory.GetDatabaseSupport(base.SelectedDatabaseType).HasSslSettings)
		{
			base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? mySqlDatabaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedMySQLHost, mySqlLoginTextEdit.Text, mySqlPasswordTextEdit.Text, providedMySQLPort, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, null, null, base.DatabaseRow.SSLSettings)
			{
				SSLType = SSLTypeEnum.TypeToString((SSLTypeEnum.SSLType)sslModeLookUpEdit.EditValue)
			};
		}
		else
		{
			base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? mySqlDatabaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedMySQLHost, mySqlLoginTextEdit.Text, mySqlPasswordTextEdit.Text, providedMySQLPort, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion);
		}
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		mysqlLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			mysqlLayoutControl.Root.AddItem(timeoutLayoutControlItem, mySqlTimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateMySqlHost();
		flag &= ValidateMySqlLogin();
		flag &= ValidateMySqlPort();
		flag &= ValidateMySqlPassword();
		if (!testForGettingDatabasesList)
		{
			flag &= ValidateMySqlDatabase();
		}
		return flag;
	}

	private bool ValidateMySqlHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(mySqlHostComboBoxEdit, addDBErrorProvider, "host", acceptEmptyValue);
	}

	private bool ValidateMySqlPassword(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(mySqlPasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateMySqlLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(mySqlLoginTextEdit, addDBErrorProvider, "user", acceptEmptyValue);
	}

	private bool ValidateMySqlPort(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(mySqlPortTextEdit, addDBErrorProvider, "port", acceptEmptyValue);
	}

	private bool ValidateMySqlDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(mySqlDatabaseButtonEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		mySqlHostComboBoxEdit.Text = base.DatabaseRow.Host;
		mySqlDatabaseButtonEdit.EditValue = base.DatabaseRow.Name;
		mySqlPortTextEdit.Text = PrepareValue.ToString(base.DatabaseRow.Port) ?? DatabaseTypeEnum.GetDefaultPort(base.DatabaseRow.Type);
		mySqlLoginTextEdit.Text = base.DatabaseRow.User;
		mySqlPasswordTextEdit.Text = base.DatabaseRow.Password;
		mySqlSavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
		if (DatabaseSupport.HasSslSettings)
		{
			sslModeLookUpEdit.EditValue = ((!string.IsNullOrEmpty(base.DatabaseRow.SSLType)) ? SSLTypeEnum.StringToType(base.DatabaseRow.SSLType) : SSLTypeEnum.SSLType.Prefer);
		}
	}

	protected override string GetPanelDocumentationTitle()
	{
		return mySqlDatabaseButtonEdit.Text + "@" + providedMySQLHost;
	}

	private void mysqlLoginTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateMySqlLogin(acceptEmptyValue: true);
	}

	private void mysqlPortTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateMySqlPort(acceptEmptyValue: true);
		PortTextEdit_EditValueChanged(sender, e);
	}

	private void mysqlDatabaseButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateMySqlDatabase(acceptEmptyValue: true);
	}

	private void mySqlDefaultPortLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			mySqlPortTextEdit.Text = DatabaseTypeEnum.GetDefaultPort(SharedDatabaseTypeEnum.DatabaseType.MySQL);
		}
	}

	private void mySqlPasswordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateMySqlPassword();
	}

	private void configureSimpleButton_Click(object sender, EventArgs e)
	{
		SSLForm sSLForm = new SSLForm(base.DatabaseRow.SSLSettings, SSLSettingsType.Default);
		sSLForm.ShowDialog();
		if (sSLForm.DialogResult == DialogResult.OK)
		{
			SSLSettings sSLSettings2 = (base.DatabaseRow.SSLSettings = (base.DatabaseRow.SSLSettings = sSLForm.SSL));
		}
	}

	private void mySqlDatabaseButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		DatabaseButtonEdit_ButtonClick(sender, e);
	}

	private void mySqlHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void mySqlHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	private void mySqlPortTextEdit_Leave(object sender, EventArgs e)
	{
		PortTextEdit_Leave(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		mySqlLoginTextEdit.Text = string.Empty;
		mySqlPasswordTextEdit.Text = string.Empty;
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
		this.mysqlLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.configureSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.mySqlDefaultPortLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.mySqlDatabaseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.mySqlPortTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.mySqlLoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.mySqlPasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.mySqlSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.mySqlHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.mySqlPortLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.mySqlLoginLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.mySqlPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.mySqlSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem34 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem35 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem36 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem37 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem38 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mySqlDatabaseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem44 = new DevExpress.XtraLayout.LayoutControlItem();
		this.mySqlTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem48 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mySqlHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem45 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.configureSSLLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.configureEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.sslModeLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.sslModeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mysqlLayoutControl).BeginInit();
		this.mysqlLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.mySqlDatabaseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlPortTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlLoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlPasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlPortLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlLoginLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlPasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem34).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem35).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem36).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem37).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem38).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlDatabaseLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem44).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlTimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem48).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem45).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.configureSSLLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.configureEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sslModeLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sslModeLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.mysqlLayoutControl.AllowCustomization = false;
		this.mysqlLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mysqlLayoutControl.Controls.Add(this.configureSimpleButton);
		this.mysqlLayoutControl.Controls.Add(this.mySqlDefaultPortLabelControl);
		this.mysqlLayoutControl.Controls.Add(this.mySqlDatabaseButtonEdit);
		this.mysqlLayoutControl.Controls.Add(this.mySqlPortTextEdit);
		this.mysqlLayoutControl.Controls.Add(this.mySqlLoginTextEdit);
		this.mysqlLayoutControl.Controls.Add(this.mySqlPasswordTextEdit);
		this.mysqlLayoutControl.Controls.Add(this.mySqlSavePasswordCheckEdit);
		this.mysqlLayoutControl.Controls.Add(this.mySqlHostComboBoxEdit);
		this.mysqlLayoutControl.Controls.Add(this.sslModeLookUpEdit);
		this.mysqlLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mysqlLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mysqlLayoutControl.Name = "mysqlLayoutControl";
		this.mysqlLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2548, 234, 360, 525);
		this.mysqlLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.mysqlLayoutControl.Root = this.layoutControlGroup6;
		this.mysqlLayoutControl.Size = new System.Drawing.Size(492, 340);
		this.mysqlLayoutControl.TabIndex = 3;
		this.mysqlLayoutControl.Text = "layoutControl1";
		this.configureSimpleButton.Location = new System.Drawing.Point(105, 144);
		this.configureSimpleButton.Margin = new System.Windows.Forms.Padding(0);
		this.configureSimpleButton.Name = "configureSimpleButton";
		this.configureSimpleButton.Size = new System.Drawing.Size(78, 19);
		this.configureSimpleButton.StyleController = this.mysqlLayoutControl;
		this.configureSimpleButton.TabIndex = 29;
		this.configureSimpleButton.Text = "Configure";
		this.configureSimpleButton.Click += new System.EventHandler(configureSimpleButton_Click);
		this.mySqlDefaultPortLabelControl.AllowHtmlString = true;
		this.mySqlDefaultPortLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.mySqlDefaultPortLabelControl.Location = new System.Drawing.Point(347, 26);
		this.mySqlDefaultPortLabelControl.Name = "mySqlDefaultPortLabelControl";
		this.mySqlDefaultPortLabelControl.Size = new System.Drawing.Size(34, 20);
		this.mySqlDefaultPortLabelControl.StyleController = this.mysqlLayoutControl;
		this.mySqlDefaultPortLabelControl.TabIndex = 27;
		this.mySqlDefaultPortLabelControl.Text = "<href>default</href>";
		this.mySqlDefaultPortLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(mySqlDefaultPortLabelControl_HyperlinkClick);
		this.mySqlDatabaseButtonEdit.Location = new System.Drawing.Point(105, 168);
		this.mySqlDatabaseButtonEdit.Name = "mySqlDatabaseButtonEdit";
		this.mySqlDatabaseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.mySqlDatabaseButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.mySqlDatabaseButtonEdit.StyleController = this.mysqlLayoutControl;
		this.mySqlDatabaseButtonEdit.TabIndex = 4;
		this.mySqlDatabaseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(mySqlDatabaseButtonEdit_ButtonClick);
		this.mySqlDatabaseButtonEdit.EditValueChanged += new System.EventHandler(mysqlDatabaseButtonEdit_EditValueChanged);
		this.mySqlPortTextEdit.EditValue = "3306";
		this.mySqlPortTextEdit.Location = new System.Drawing.Point(105, 24);
		this.mySqlPortTextEdit.Margin = new System.Windows.Forms.Padding(2);
		this.mySqlPortTextEdit.Name = "mySqlPortTextEdit";
		this.mySqlPortTextEdit.Properties.Mask.EditMask = "\\d+";
		this.mySqlPortTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.mySqlPortTextEdit.Properties.Mask.ShowPlaceHolders = false;
		this.mySqlPortTextEdit.Properties.MaxLength = 5;
		this.mySqlPortTextEdit.Size = new System.Drawing.Size(230, 20);
		this.mySqlPortTextEdit.StyleController = this.mysqlLayoutControl;
		this.mySqlPortTextEdit.TabIndex = 0;
		this.mySqlPortTextEdit.EditValueChanged += new System.EventHandler(mysqlPortTextEdit_EditValueChanged);
		this.mySqlPortTextEdit.Leave += new System.EventHandler(mySqlPortTextEdit_Leave);
		this.mySqlLoginTextEdit.Location = new System.Drawing.Point(105, 48);
		this.mySqlLoginTextEdit.Name = "mySqlLoginTextEdit";
		this.mySqlLoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.mySqlLoginTextEdit.StyleController = this.mysqlLayoutControl;
		this.mySqlLoginTextEdit.TabIndex = 1;
		this.mySqlLoginTextEdit.EditValueChanged += new System.EventHandler(mysqlLoginTextEdit_EditValueChanged);
		this.mySqlPasswordTextEdit.Location = new System.Drawing.Point(105, 72);
		this.mySqlPasswordTextEdit.Name = "mySqlPasswordTextEdit";
		this.mySqlPasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.mySqlPasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.mySqlPasswordTextEdit.StyleController = this.mysqlLayoutControl;
		this.mySqlPasswordTextEdit.TabIndex = 2;
		this.mySqlPasswordTextEdit.EditValueChanged += new System.EventHandler(mySqlPasswordTextEdit_EditValueChanged);
		this.mySqlSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 96);
		this.mySqlSavePasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.mySqlSavePasswordCheckEdit.Name = "mySqlSavePasswordCheckEdit";
		this.mySqlSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.mySqlSavePasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.mySqlSavePasswordCheckEdit.StyleController = this.mysqlLayoutControl;
		this.mySqlSavePasswordCheckEdit.TabIndex = 3;
		this.mySqlHostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.mySqlHostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2);
		this.mySqlHostComboBoxEdit.Name = "mySqlHostComboBoxEdit";
		this.mySqlHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.mySqlHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.mySqlHostComboBoxEdit.StyleController = this.mysqlLayoutControl;
		this.mySqlHostComboBoxEdit.TabIndex = 1;
		this.mySqlHostComboBoxEdit.EditValueChanged += new System.EventHandler(mySqlHostComboBoxEdit_EditValueChanged);
		this.mySqlHostComboBoxEdit.Leave += new System.EventHandler(mySqlHostComboBoxEdit_Leave);
		this.layoutControlGroup6.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup6.GroupBordersVisible = false;
		this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[18]
		{
			this.mySqlPortLayoutControlItem, this.mySqlLoginLayoutControlItem, this.mySqlPasswordLayoutControlItem, this.mySqlSavePasswordLayoutControlItem, this.emptySpaceItem34, this.emptySpaceItem35, this.emptySpaceItem36, this.emptySpaceItem37, this.emptySpaceItem38, this.mySqlDatabaseLayoutControlItem,
			this.layoutControlItem44, this.mySqlTimeoutEmptySpaceItem, this.emptySpaceItem48, this.mySqlHostLayoutControlItem, this.emptySpaceItem45, this.configureSSLLayoutControlItem, this.configureEmptySpaceItem, this.sslModeLayoutControlItem
		});
		this.layoutControlGroup6.Name = "Root";
		this.layoutControlGroup6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup6.Size = new System.Drawing.Size(492, 340);
		this.layoutControlGroup6.TextVisible = false;
		this.mySqlPortLayoutControlItem.Control = this.mySqlPortTextEdit;
		this.mySqlPortLayoutControlItem.CustomizationFormText = "Port:";
		this.mySqlPortLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.mySqlPortLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.mySqlPortLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.mySqlPortLayoutControlItem.Name = "mySqlPortLayoutControlItem";
		this.mySqlPortLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mySqlPortLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.mySqlPortLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mySqlPortLayoutControlItem.Text = "Port:";
		this.mySqlPortLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mySqlPortLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.mySqlPortLayoutControlItem.TextToControlDistance = 5;
		this.mySqlLoginLayoutControlItem.Control = this.mySqlLoginTextEdit;
		this.mySqlLoginLayoutControlItem.CustomizationFormText = "User:";
		this.mySqlLoginLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.mySqlLoginLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.mySqlLoginLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.mySqlLoginLayoutControlItem.Name = "mySqlLoginLayoutControlItem";
		this.mySqlLoginLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mySqlLoginLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.mySqlLoginLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mySqlLoginLayoutControlItem.Text = "User:";
		this.mySqlLoginLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mySqlLoginLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.mySqlLoginLayoutControlItem.TextToControlDistance = 5;
		this.mySqlPasswordLayoutControlItem.Control = this.mySqlPasswordTextEdit;
		this.mySqlPasswordLayoutControlItem.CustomizationFormText = "Password";
		this.mySqlPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.mySqlPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.mySqlPasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.mySqlPasswordLayoutControlItem.Name = "mySqlPasswordLayoutControlItem";
		this.mySqlPasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mySqlPasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.mySqlPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mySqlPasswordLayoutControlItem.Text = "Password:";
		this.mySqlPasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mySqlPasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.mySqlPasswordLayoutControlItem.TextToControlDistance = 5;
		this.mySqlSavePasswordLayoutControlItem.Control = this.mySqlSavePasswordCheckEdit;
		this.mySqlSavePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem18";
		this.mySqlSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.mySqlSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.mySqlSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.mySqlSavePasswordLayoutControlItem.Name = "mySqlSavePasswordLayoutControlItem";
		this.mySqlSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mySqlSavePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.mySqlSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mySqlSavePasswordLayoutControlItem.Text = " ";
		this.mySqlSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mySqlSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.mySqlSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem34.AllowHotTrack = false;
		this.emptySpaceItem34.Location = new System.Drawing.Point(0, 214);
		this.emptySpaceItem34.Name = "emptySpaceItem3";
		this.emptySpaceItem34.Size = new System.Drawing.Size(492, 126);
		this.emptySpaceItem34.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem35.AllowHotTrack = false;
		this.emptySpaceItem35.CustomizationFormText = "emptySpaceItem13";
		this.emptySpaceItem35.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem35.Name = "emptySpaceItem19";
		this.emptySpaceItem35.Size = new System.Drawing.Size(157, 24);
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
		this.emptySpaceItem37.Location = new System.Drawing.Point(335, 120);
		this.emptySpaceItem37.Name = "emptySpaceItem26";
		this.emptySpaceItem37.Size = new System.Drawing.Size(157, 72);
		this.emptySpaceItem37.Text = "emptySpaceItem13";
		this.emptySpaceItem37.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem38.AllowHotTrack = false;
		this.emptySpaceItem38.CustomizationFormText = "emptySpaceItem13";
		this.emptySpaceItem38.Location = new System.Drawing.Point(335, 72);
		this.emptySpaceItem38.Name = "emptySpaceItem27";
		this.emptySpaceItem38.Size = new System.Drawing.Size(157, 24);
		this.emptySpaceItem38.Text = "emptySpaceItem13";
		this.emptySpaceItem38.TextSize = new System.Drawing.Size(0, 0);
		this.mySqlDatabaseLayoutControlItem.Control = this.mySqlDatabaseButtonEdit;
		this.mySqlDatabaseLayoutControlItem.Location = new System.Drawing.Point(0, 168);
		this.mySqlDatabaseLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.mySqlDatabaseLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.mySqlDatabaseLayoutControlItem.Name = "mySqlDatabaseLayoutControlItem";
		this.mySqlDatabaseLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mySqlDatabaseLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.mySqlDatabaseLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mySqlDatabaseLayoutControlItem.Text = "Database:";
		this.mySqlDatabaseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mySqlDatabaseLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.mySqlDatabaseLayoutControlItem.TextToControlDistance = 5;
		this.layoutControlItem44.Control = this.mySqlDefaultPortLabelControl;
		this.layoutControlItem44.Location = new System.Drawing.Point(345, 24);
		this.layoutControlItem44.MaxSize = new System.Drawing.Size(38, 24);
		this.layoutControlItem44.MinSize = new System.Drawing.Size(38, 24);
		this.layoutControlItem44.Name = "layoutControlItem32";
		this.layoutControlItem44.Size = new System.Drawing.Size(38, 24);
		this.layoutControlItem44.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem44.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem44.TextVisible = false;
		this.mySqlTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.mySqlTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 192);
		this.mySqlTimeoutEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 22);
		this.mySqlTimeoutEmptySpaceItem.MinSize = new System.Drawing.Size(10, 22);
		this.mySqlTimeoutEmptySpaceItem.Name = "mySqlTimeoutEmptySpaceItem";
		this.mySqlTimeoutEmptySpaceItem.Size = new System.Drawing.Size(492, 22);
		this.mySqlTimeoutEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mySqlTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem48.AllowHotTrack = false;
		this.emptySpaceItem48.Location = new System.Drawing.Point(335, 24);
		this.emptySpaceItem48.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem48.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem48.Name = "emptySpaceItem48";
		this.emptySpaceItem48.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem48.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem48.TextSize = new System.Drawing.Size(0, 0);
		this.mySqlHostLayoutControlItem.Control = this.mySqlHostComboBoxEdit;
		this.mySqlHostLayoutControlItem.CustomizationFormText = "Host:";
		this.mySqlHostLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.mySqlHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.mySqlHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.mySqlHostLayoutControlItem.Name = "mySqlHostLayoutControlItem";
		this.mySqlHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mySqlHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.mySqlHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.mySqlHostLayoutControlItem.Text = "Host:";
		this.mySqlHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.mySqlHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.mySqlHostLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem45.AllowHotTrack = false;
		this.emptySpaceItem45.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem45.Name = "emptySpaceItem45";
		this.emptySpaceItem45.Size = new System.Drawing.Size(157, 24);
		this.emptySpaceItem45.TextSize = new System.Drawing.Size(0, 0);
		this.configureSSLLayoutControlItem.Control = this.configureSimpleButton;
		this.configureSSLLayoutControlItem.Location = new System.Drawing.Point(0, 144);
		this.configureSSLLayoutControlItem.MaxSize = new System.Drawing.Size(183, 24);
		this.configureSSLLayoutControlItem.MinSize = new System.Drawing.Size(183, 24);
		this.configureSSLLayoutControlItem.Name = "configureSSLLayoutControlItem";
		this.configureSSLLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 5);
		this.configureSSLLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.configureSSLLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.configureSSLLayoutControlItem.Text = "Configure SSL:";
		this.configureSSLLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.configureSSLLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.configureSSLLayoutControlItem.TextToControlDistance = 5;
		this.configureEmptySpaceItem.AllowHotTrack = false;
		this.configureEmptySpaceItem.Location = new System.Drawing.Point(335, 96);
		this.configureEmptySpaceItem.Name = "configureEmptySpaceItem";
		this.configureEmptySpaceItem.Size = new System.Drawing.Size(157, 24);
		this.configureEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.sslModeLookUpEdit.Location = new System.Drawing.Point(105, 120);
		this.sslModeLookUpEdit.Name = "sslModeLookUpEdit";
		this.sslModeLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.sslModeLookUpEdit.Properties.DisplayMember = "Value";
		this.sslModeLookUpEdit.Properties.NullText = "";
		this.sslModeLookUpEdit.Properties.ShowFooter = false;
		this.sslModeLookUpEdit.Properties.ShowHeader = false;
		this.sslModeLookUpEdit.Properties.ShowLines = false;
		this.sslModeLookUpEdit.Properties.ValueMember = "Key";
		this.sslModeLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.sslModeLookUpEdit.StyleController = this.mysqlLayoutControl;
		this.sslModeLookUpEdit.TabIndex = 33;
		this.sslModeLayoutControlItem.Control = this.sslModeLookUpEdit;
		this.sslModeLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.sslModeLayoutControlItem.CustomizationFormText = "SSL mode:";
		this.sslModeLayoutControlItem.Location = new System.Drawing.Point(0, 120);
		this.sslModeLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sslModeLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sslModeLayoutControlItem.Name = "sslModeLayoutControlItem";
		this.sslModeLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sslModeLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sslModeLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sslModeLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sslModeLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sslModeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sslModeLayoutControlItem.Text = "SSL mode:";
		this.sslModeLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sslModeLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sslModeLayoutControlItem.TextToControlDistance = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mysqlLayoutControl);
		base.Name = "MySQLConnectorControl";
		base.Size = new System.Drawing.Size(492, 340);
		((System.ComponentModel.ISupportInitialize)this.mysqlLayoutControl).EndInit();
		this.mysqlLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.mySqlDatabaseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlPortTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlLoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlPasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlPortLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlLoginLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlPasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem34).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem35).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem36).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem37).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem38).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlDatabaseLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem44).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlTimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem48).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mySqlHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem45).EndInit();
		((System.ComponentModel.ISupportInitialize)this.configureSSLLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.configureEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sslModeLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sslModeLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
