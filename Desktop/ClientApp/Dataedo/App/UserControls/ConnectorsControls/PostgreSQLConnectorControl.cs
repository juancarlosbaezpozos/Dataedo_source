using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Classes.Synchronize.DatabaseRows;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.DatabasesSupport;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class PostgreSQLConnectorControl : ConnectorControlBase
{
	private IDatabaseSupport _databaseSupport;

	private IContainer components;

	protected NonCustomizableLayoutControl postgresLayoutControl;

	protected LookUpEdit postgreSqlSSLModeLookUpEdit;

	private LabelControl postgreSqlDefaultPortLabelControl;

	protected ButtonEdit postgreSqlDatabaseButtonEdit;

	private CheckEdit postgreSqlSavePasswordCheckEdit;

	protected TextEdit postgreSqlLoginTextEdit;

	protected TextEdit postgreSqlPasswordTextEdit;

	private TextEdit postgreSqlPortTextEdit;

	private ComboBoxEdit postgreSqlHostComboBoxEdit;

	private LayoutControlGroup layoutControlGroup3;

	private LayoutControlItem postgreSqlPasswordLayoutControlItem;

	private LayoutControlItem postgreSqlLoginLayoutControlItem;

	private LayoutControlItem postgreSqlSavePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem25;

	private EmptySpaceItem emptySpaceItem26;

	private EmptySpaceItem emptySpaceItem27;

	private EmptySpaceItem emptySpaceItem28;

	protected LayoutControlItem postgreSqlDatabaseLayoutControlItem;

	private EmptySpaceItem postgreSqlTimeoutEmptySpaceItem;

	private LayoutControlItem postgreSqlPortLayoutControlItem;

	private LayoutControlItem layoutControlItem27;

	private EmptySpaceItem emptySpaceItem49;

	private LayoutControlItem postgreSqlHostLayoutControlItem;

	private EmptySpaceItem emptySpaceItem51;

	private LayoutControlItem postgreSqlSSLModeLayoutControlItem;

	private EmptySpaceItem emptySpaceItem46;

	private SimpleButton configureSimpleButton;

	private LayoutControlItem configureSSLLayoutControlItem;

	protected string providedPostgreSqlHost => splittedHost?.Host ?? postgreSqlHostComboBoxEdit.Text;

	protected string providedPostgresPort => postgreSqlPortTextEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.PostgreSQL;

	private SSLSettings SSLSettings { get; set; }

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
		set
		{
			_databaseSupport = value;
		}
	}

	protected override TextEdit HostTextEdit => postgreSqlHostComboBoxEdit;

	protected override TextEdit PortTextEdit => postgreSqlPortTextEdit;

	protected override ComboBoxEdit HostComboBoxEdit => postgreSqlHostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => postgreSqlSavePasswordCheckEdit;

	public PostgreSQLConnectorControl()
	{
		InitializeComponent();
		HideSSLConfigureButton();
	}

	protected override void SetAuthenticationDataSource()
	{
		Dictionary<SSLTypeEnum.SSLType, string> dictionary = (DatabaseSupport.HasSslSettings ? SSLTypeEnum.GetPostgreSqlSSLTypes() : SSLTypeEnum.GetBasicSSLTypes());
		postgreSqlSSLModeLookUpEdit.Properties.DataSource = dictionary;
		postgreSqlSSLModeLookUpEdit.Properties.DropDownRows = dictionary.Count;
		if (postgreSqlSSLModeLookUpEdit.EditValue == null)
		{
			postgreSqlSSLModeLookUpEdit.EditValue = SSLTypeEnum.SSLType.Prefer;
		}
		if (DatabaseSupportFactoryShared.CheckIfTypeIsSupported(base.SelectedDatabaseType))
		{
			IDatabaseSupport databaseSupport = DatabaseSupportFactory.GetDatabaseSupport(base.SelectedDatabaseType);
			postgreSqlDatabaseButtonEdit.Properties.Buttons[0].Visible = databaseSupport.CanGetDatabases;
		}
	}

	public override void SetSwitchDatabaseAvailability(bool isAvailable)
	{
		postgreSqlDatabaseButtonEdit.Enabled = isAvailable;
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		if (DatabaseSupportFactory.GetDatabaseSupport(base.SelectedDatabaseType).HasSslSettings)
		{
			base.DatabaseRow = new PostgresDatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? postgreSqlDatabaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedPostgreSqlHost, postgreSqlLoginTextEdit.Text, postgreSqlPasswordTextEdit.Text, providedPostgresPort, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, SSLTypeEnum.TypeToString((SSLTypeEnum.SSLType)postgreSqlSSLModeLookUpEdit.EditValue), base.DatabaseRow.SSLSettings);
		}
		else
		{
			base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? postgreSqlDatabaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedPostgreSqlHost, postgreSqlLoginTextEdit.Text, postgreSqlPasswordTextEdit.Text, providedPostgresPort, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, SSLTypeEnum.TypeToString((SSLTypeEnum.SSLType)postgreSqlSSLModeLookUpEdit.EditValue));
		}
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		postgresLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			postgresLayoutControl.Root.AddItem(timeoutLayoutControlItem, postgreSqlTimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidatePostgreSqlHost();
		flag &= ValidatePostgreSqlLogin();
		flag &= ValidatePostgreSqlPort();
		flag &= ValidatePostgreSqlPassword();
		flag &= ValidatePostgreSqlSSLMode();
		if (!testForGettingDatabasesList)
		{
			flag &= ValidatePostgreSqlDatabase();
		}
		return flag;
	}

	private bool ValidatePostgreSqlHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(postgreSqlHostComboBoxEdit, addDBErrorProvider, "host", acceptEmptyValue);
	}

	private bool ValidatePostgreSqlPassword(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(postgreSqlPasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidatePostgreSqlLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(postgreSqlLoginTextEdit, addDBErrorProvider, "user", acceptEmptyValue);
	}

	private bool ValidatePostgreSqlPort(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(postgreSqlPortTextEdit, addDBErrorProvider, "port", acceptEmptyValue);
	}

	private bool ValidatePostgreSqlDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(postgreSqlDatabaseButtonEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	private bool ValidatePostgreSqlSSLMode(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(postgreSqlSSLModeLookUpEdit, addDBErrorProvider, "SSL Mode", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		postgreSqlHostComboBoxEdit.Text = base.DatabaseRow.Host;
		postgreSqlDatabaseButtonEdit.EditValue = base.DatabaseRow.Name;
		postgreSqlSSLModeLookUpEdit.EditValue = SSLTypeEnum.StringToType(base.DatabaseRow.SSLType);
		postgreSqlPortTextEdit.Text = PrepareValue.ToString(base.DatabaseRow.Port) ?? DatabaseTypeEnum.GetDefaultPort(base.DatabaseRow.Type);
		postgreSqlLoginTextEdit.Text = base.DatabaseRow.User;
		postgreSqlPasswordTextEdit.Text = base.DatabaseRow.Password;
		postgreSqlSavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
		if (DatabaseSupport.HasSslSettings)
		{
			SSLSettings = base.DatabaseRow.SSLSettings;
			SSLSettings.SetEncryptedPassword(base.DatabaseRow.Param1);
		}
	}

	protected override string GetPanelDocumentationTitle()
	{
		return postgreSqlDatabaseButtonEdit.Text + "@" + providedPostgreSqlHost;
	}

	private void postgresLoginTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidatePostgreSqlLogin(acceptEmptyValue: true);
	}

	private void postgresPortTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidatePostgreSqlPort(acceptEmptyValue: true);
		PortTextEdit_EditValueChanged(sender, e);
	}

	private void postgresDatabaseButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidatePostgreSqlDatabase(acceptEmptyValue: true);
	}

	private void postgreSqlSSLModeLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidatePostgreSqlSSLMode();
	}

	private void postgresDefaultPortLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			postgreSqlPortTextEdit.Text = DatabaseTypeEnum.GetDefaultPort(SharedDatabaseTypeEnum.DatabaseType.PostgreSQL);
		}
	}

	private void postgreSqlPasswordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidatePostgreSqlPassword();
	}

	private void postgreSqlDatabaseButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		DatabaseButtonEdit_ButtonClick(sender, e);
	}

	private void postgreSqlHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void postgreSqlHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		postgreSqlLoginTextEdit.Text = string.Empty;
		postgreSqlPasswordTextEdit.Text = string.Empty;
	}

	private void configureSimpleButton_Click(object sender, EventArgs e)
	{
		SSLForm sSLForm = new SSLForm(base.DatabaseRow.SSLSettings, SSLSettingsType.ClientPfxWithPassword);
		sSLForm.ShowDialog();
		if (sSLForm.DialogResult == DialogResult.OK)
		{
			base.DatabaseRow.SSLSettings = sSLForm.SSL;
		}
	}

	protected bool HideSSLConfigureButton()
	{
		configureSSLLayoutControlItem.Visibility = LayoutVisibility.Never;
		return true;
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
		this.postgresLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.postgreSqlSSLModeLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.postgreSqlDefaultPortLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.postgreSqlDatabaseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.postgreSqlSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.postgreSqlLoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.postgreSqlPasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.postgreSqlPortTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.postgreSqlHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.configureSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.postgreSqlPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.postgreSqlLoginLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.postgreSqlSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem25 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem26 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem27 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem28 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.postgreSqlTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.postgreSqlPortLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem27 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem49 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.postgreSqlHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem51 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.postgreSqlSSLModeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem46 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.postgreSqlDatabaseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.configureSSLLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.postgresLayoutControl).BeginInit();
		this.postgresLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlSSLModeLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlDatabaseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlLoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlPasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlPortTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlPasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlLoginLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem25).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem26).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem27).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem28).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlTimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlPortLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem27).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem49).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem51).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlSSLModeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem46).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlDatabaseLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.configureSSLLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.postgresLayoutControl.AllowCustomization = false;
		this.postgresLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.postgresLayoutControl.Controls.Add(this.postgreSqlSSLModeLookUpEdit);
		this.postgresLayoutControl.Controls.Add(this.postgreSqlDefaultPortLabelControl);
		this.postgresLayoutControl.Controls.Add(this.postgreSqlDatabaseButtonEdit);
		this.postgresLayoutControl.Controls.Add(this.postgreSqlSavePasswordCheckEdit);
		this.postgresLayoutControl.Controls.Add(this.postgreSqlLoginTextEdit);
		this.postgresLayoutControl.Controls.Add(this.postgreSqlPasswordTextEdit);
		this.postgresLayoutControl.Controls.Add(this.postgreSqlPortTextEdit);
		this.postgresLayoutControl.Controls.Add(this.postgreSqlHostComboBoxEdit);
		this.postgresLayoutControl.Controls.Add(this.configureSimpleButton);
		this.postgresLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.postgresLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.postgresLayoutControl.Name = "postgresLayoutControl";
		this.postgresLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(603, 123, 657, 726);
		this.postgresLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.postgresLayoutControl.Root = this.layoutControlGroup3;
		this.postgresLayoutControl.Size = new System.Drawing.Size(464, 251);
		this.postgresLayoutControl.TabIndex = 29;
		this.postgresLayoutControl.Text = "layoutControl1";
		this.postgreSqlSSLModeLookUpEdit.Location = new System.Drawing.Point(105, 120);
		this.postgreSqlSSLModeLookUpEdit.Name = "postgreSqlSSLModeLookUpEdit";
		this.postgreSqlSSLModeLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.postgreSqlSSLModeLookUpEdit.Properties.DisplayMember = "Value";
		this.postgreSqlSSLModeLookUpEdit.Properties.NullText = "";
		this.postgreSqlSSLModeLookUpEdit.Properties.ShowFooter = false;
		this.postgreSqlSSLModeLookUpEdit.Properties.ShowHeader = false;
		this.postgreSqlSSLModeLookUpEdit.Properties.ShowLines = false;
		this.postgreSqlSSLModeLookUpEdit.Properties.ValueMember = "Key";
		this.postgreSqlSSLModeLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.postgreSqlSSLModeLookUpEdit.StyleController = this.postgresLayoutControl;
		this.postgreSqlSSLModeLookUpEdit.TabIndex = 33;
		this.postgreSqlSSLModeLookUpEdit.EditValueChanged += new System.EventHandler(postgreSqlSSLModeLookUpEdit_EditValueChanged);
		this.postgreSqlDefaultPortLabelControl.AllowHtmlString = true;
		this.postgreSqlDefaultPortLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.postgreSqlDefaultPortLabelControl.Location = new System.Drawing.Point(347, 26);
		this.postgreSqlDefaultPortLabelControl.MinimumSize = new System.Drawing.Size(34, 20);
		this.postgreSqlDefaultPortLabelControl.Name = "postgreSqlDefaultPortLabelControl";
		this.postgreSqlDefaultPortLabelControl.Size = new System.Drawing.Size(34, 20);
		this.postgreSqlDefaultPortLabelControl.StyleController = this.postgresLayoutControl;
		this.postgreSqlDefaultPortLabelControl.TabIndex = 31;
		this.postgreSqlDefaultPortLabelControl.Text = "<href>default</href>";
		this.postgreSqlDefaultPortLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(postgresDefaultPortLabelControl_HyperlinkClick);
		this.postgreSqlDatabaseButtonEdit.Location = new System.Drawing.Point(105, 168);
		this.postgreSqlDatabaseButtonEdit.Name = "postgreSqlDatabaseButtonEdit";
		this.postgreSqlDatabaseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.postgreSqlDatabaseButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.postgreSqlDatabaseButtonEdit.StyleController = this.postgresLayoutControl;
		this.postgreSqlDatabaseButtonEdit.TabIndex = 4;
		this.postgreSqlDatabaseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(postgreSqlDatabaseButtonEdit_ButtonClick);
		this.postgreSqlDatabaseButtonEdit.EditValueChanged += new System.EventHandler(postgresDatabaseButtonEdit_EditValueChanged);
		this.postgreSqlSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 96);
		this.postgreSqlSavePasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.postgreSqlSavePasswordCheckEdit.Name = "postgreSqlSavePasswordCheckEdit";
		this.postgreSqlSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.postgreSqlSavePasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.postgreSqlSavePasswordCheckEdit.StyleController = this.postgresLayoutControl;
		this.postgreSqlSavePasswordCheckEdit.TabIndex = 3;
		this.postgreSqlLoginTextEdit.Location = new System.Drawing.Point(105, 48);
		this.postgreSqlLoginTextEdit.Name = "postgreSqlLoginTextEdit";
		this.postgreSqlLoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.postgreSqlLoginTextEdit.StyleController = this.postgresLayoutControl;
		this.postgreSqlLoginTextEdit.TabIndex = 1;
		this.postgreSqlLoginTextEdit.EditValueChanged += new System.EventHandler(postgresLoginTextEdit_EditValueChanged);
		this.postgreSqlPasswordTextEdit.Location = new System.Drawing.Point(105, 72);
		this.postgreSqlPasswordTextEdit.Name = "postgreSqlPasswordTextEdit";
		this.postgreSqlPasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.postgreSqlPasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.postgreSqlPasswordTextEdit.StyleController = this.postgresLayoutControl;
		this.postgreSqlPasswordTextEdit.TabIndex = 2;
		this.postgreSqlPasswordTextEdit.EditValueChanged += new System.EventHandler(postgreSqlPasswordTextEdit_EditValueChanged);
		this.postgreSqlPortTextEdit.EditValue = "5432";
		this.postgreSqlPortTextEdit.Location = new System.Drawing.Point(105, 24);
		this.postgreSqlPortTextEdit.Margin = new System.Windows.Forms.Padding(2);
		this.postgreSqlPortTextEdit.Name = "postgreSqlPortTextEdit";
		this.postgreSqlPortTextEdit.Properties.Mask.EditMask = "\\d+";
		this.postgreSqlPortTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.postgreSqlPortTextEdit.Properties.Mask.ShowPlaceHolders = false;
		this.postgreSqlPortTextEdit.Properties.MaxLength = 5;
		this.postgreSqlPortTextEdit.Size = new System.Drawing.Size(230, 20);
		this.postgreSqlPortTextEdit.StyleController = this.postgresLayoutControl;
		this.postgreSqlPortTextEdit.TabIndex = 0;
		this.postgreSqlPortTextEdit.EditValueChanged += new System.EventHandler(postgresPortTextEdit_EditValueChanged);
		this.postgreSqlHostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.postgreSqlHostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2);
		this.postgreSqlHostComboBoxEdit.Name = "postgreSqlHostComboBoxEdit";
		this.postgreSqlHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.postgreSqlHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.postgreSqlHostComboBoxEdit.StyleController = this.postgresLayoutControl;
		this.postgreSqlHostComboBoxEdit.TabIndex = 1;
		this.postgreSqlHostComboBoxEdit.EditValueChanged += new System.EventHandler(postgreSqlHostComboBoxEdit_EditValueChanged);
		this.postgreSqlHostComboBoxEdit.Leave += new System.EventHandler(postgreSqlHostComboBoxEdit_Leave);
		this.configureSimpleButton.Location = new System.Drawing.Point(105, 144);
		this.configureSimpleButton.Margin = new System.Windows.Forms.Padding(0);
		this.configureSimpleButton.Name = "configureSimpleButton";
		this.configureSimpleButton.Size = new System.Drawing.Size(78, 19);
		this.configureSimpleButton.StyleController = this.postgresLayoutControl;
		this.configureSimpleButton.TabIndex = 29;
		this.configureSimpleButton.Text = "Configure";
		this.configureSimpleButton.Click += new System.EventHandler(configureSimpleButton_Click);
		this.layoutControlGroup3.CustomizationFormText = "Root";
		this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup3.GroupBordersVisible = false;
		this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[17]
		{
			this.postgreSqlPasswordLayoutControlItem, this.postgreSqlLoginLayoutControlItem, this.postgreSqlSavePasswordLayoutControlItem, this.emptySpaceItem25, this.emptySpaceItem26, this.emptySpaceItem27, this.emptySpaceItem28, this.postgreSqlTimeoutEmptySpaceItem, this.postgreSqlPortLayoutControlItem, this.layoutControlItem27,
			this.emptySpaceItem49, this.postgreSqlHostLayoutControlItem, this.emptySpaceItem51, this.postgreSqlSSLModeLayoutControlItem, this.emptySpaceItem46, this.postgreSqlDatabaseLayoutControlItem, this.configureSSLLayoutControlItem
		});
		this.layoutControlGroup3.Name = "Root";
		this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup3.Size = new System.Drawing.Size(464, 251);
		this.layoutControlGroup3.TextVisible = false;
		this.postgreSqlPasswordLayoutControlItem.Control = this.postgreSqlPasswordTextEdit;
		this.postgreSqlPasswordLayoutControlItem.CustomizationFormText = "Password";
		this.postgreSqlPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.postgreSqlPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.postgreSqlPasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.postgreSqlPasswordLayoutControlItem.Name = "postgreSqlPasswordLayoutControlItem";
		this.postgreSqlPasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.postgreSqlPasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.postgreSqlPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.postgreSqlPasswordLayoutControlItem.Text = "Password:";
		this.postgreSqlPasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.postgreSqlPasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.postgreSqlPasswordLayoutControlItem.TextToControlDistance = 5;
		this.postgreSqlLoginLayoutControlItem.Control = this.postgreSqlLoginTextEdit;
		this.postgreSqlLoginLayoutControlItem.CustomizationFormText = "User:";
		this.postgreSqlLoginLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.postgreSqlLoginLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.postgreSqlLoginLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.postgreSqlLoginLayoutControlItem.Name = "postgreSqlLoginLayoutControlItem";
		this.postgreSqlLoginLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.postgreSqlLoginLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.postgreSqlLoginLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.postgreSqlLoginLayoutControlItem.Text = "User:";
		this.postgreSqlLoginLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.postgreSqlLoginLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.postgreSqlLoginLayoutControlItem.TextToControlDistance = 5;
		this.postgreSqlSavePasswordLayoutControlItem.Control = this.postgreSqlSavePasswordCheckEdit;
		this.postgreSqlSavePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem2";
		this.postgreSqlSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.postgreSqlSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.postgreSqlSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.postgreSqlSavePasswordLayoutControlItem.Name = "postgreSqlSavePasswordLayoutControlItem";
		this.postgreSqlSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.postgreSqlSavePasswordLayoutControlItem.Size = new System.Drawing.Size(464, 24);
		this.postgreSqlSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.postgreSqlSavePasswordLayoutControlItem.Text = " ";
		this.postgreSqlSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.postgreSqlSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.postgreSqlSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem25.AllowHotTrack = false;
		this.emptySpaceItem25.CustomizationFormText = "emptySpaceItem6";
		this.emptySpaceItem25.Location = new System.Drawing.Point(0, 216);
		this.emptySpaceItem25.Name = "emptySpaceItem6";
		this.emptySpaceItem25.Size = new System.Drawing.Size(464, 35);
		this.emptySpaceItem25.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem26.AllowHotTrack = false;
		this.emptySpaceItem26.CustomizationFormText = "emptySpaceItem20";
		this.emptySpaceItem26.Location = new System.Drawing.Point(383, 24);
		this.emptySpaceItem26.Name = "emptySpaceItem20";
		this.emptySpaceItem26.Size = new System.Drawing.Size(81, 24);
		this.emptySpaceItem26.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem27.AllowHotTrack = false;
		this.emptySpaceItem27.CustomizationFormText = "emptySpaceItem21";
		this.emptySpaceItem27.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem27.Name = "emptySpaceItem21";
		this.emptySpaceItem27.Size = new System.Drawing.Size(129, 24);
		this.emptySpaceItem27.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem28.AllowHotTrack = false;
		this.emptySpaceItem28.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem28.Location = new System.Drawing.Point(335, 72);
		this.emptySpaceItem28.Name = "emptySpaceItem22";
		this.emptySpaceItem28.Size = new System.Drawing.Size(129, 24);
		this.emptySpaceItem28.TextSize = new System.Drawing.Size(0, 0);
		this.postgreSqlTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.postgreSqlTimeoutEmptySpaceItem.CustomizationFormText = "emptySpaceItem12";
		this.postgreSqlTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 192);
		this.postgreSqlTimeoutEmptySpaceItem.MaxSize = new System.Drawing.Size(405, 24);
		this.postgreSqlTimeoutEmptySpaceItem.MinSize = new System.Drawing.Size(405, 24);
		this.postgreSqlTimeoutEmptySpaceItem.Name = "postgreSqlTimeoutEmptySpaceItem";
		this.postgreSqlTimeoutEmptySpaceItem.Size = new System.Drawing.Size(464, 24);
		this.postgreSqlTimeoutEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.postgreSqlTimeoutEmptySpaceItem.Text = "emptySpaceItem12";
		this.postgreSqlTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.postgreSqlPortLayoutControlItem.Control = this.postgreSqlPortTextEdit;
		this.postgreSqlPortLayoutControlItem.CustomizationFormText = "Port:";
		this.postgreSqlPortLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.postgreSqlPortLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.postgreSqlPortLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.postgreSqlPortLayoutControlItem.Name = "postgreSqlPortLayoutControlItem";
		this.postgreSqlPortLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.postgreSqlPortLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.postgreSqlPortLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.postgreSqlPortLayoutControlItem.Text = "Port:";
		this.postgreSqlPortLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.postgreSqlPortLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.postgreSqlPortLayoutControlItem.TextToControlDistance = 5;
		this.layoutControlItem27.Control = this.postgreSqlDefaultPortLabelControl;
		this.layoutControlItem27.Location = new System.Drawing.Point(345, 24);
		this.layoutControlItem27.Name = "layoutControlItem27";
		this.layoutControlItem27.Size = new System.Drawing.Size(38, 24);
		this.layoutControlItem27.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem27.TextVisible = false;
		this.emptySpaceItem49.AllowHotTrack = false;
		this.emptySpaceItem49.Location = new System.Drawing.Point(335, 24);
		this.emptySpaceItem49.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem49.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem49.Name = "emptySpaceItem49";
		this.emptySpaceItem49.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem49.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem49.TextSize = new System.Drawing.Size(0, 0);
		this.postgreSqlHostLayoutControlItem.Control = this.postgreSqlHostComboBoxEdit;
		this.postgreSqlHostLayoutControlItem.CustomizationFormText = "Host:";
		this.postgreSqlHostLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.postgreSqlHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.postgreSqlHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.postgreSqlHostLayoutControlItem.Name = "postgreSqlHostLayoutControlItem";
		this.postgreSqlHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.postgreSqlHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.postgreSqlHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.postgreSqlHostLayoutControlItem.Text = "Host:";
		this.postgreSqlHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.postgreSqlHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.postgreSqlHostLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem51.AllowHotTrack = false;
		this.emptySpaceItem51.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem51.Name = "emptySpaceItem51";
		this.emptySpaceItem51.Size = new System.Drawing.Size(129, 24);
		this.emptySpaceItem51.TextSize = new System.Drawing.Size(0, 0);
		this.postgreSqlSSLModeLayoutControlItem.Control = this.postgreSqlSSLModeLookUpEdit;
		this.postgreSqlSSLModeLayoutControlItem.Location = new System.Drawing.Point(0, 120);
		this.postgreSqlSSLModeLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.postgreSqlSSLModeLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.postgreSqlSSLModeLayoutControlItem.Name = "postgreSqlSSLModeLayoutControlItem";
		this.postgreSqlSSLModeLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.postgreSqlSSLModeLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.postgreSqlSSLModeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.postgreSqlSSLModeLayoutControlItem.Text = "SSL mode:";
		this.postgreSqlSSLModeLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.postgreSqlSSLModeLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.postgreSqlSSLModeLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem46.AllowHotTrack = false;
		this.emptySpaceItem46.Location = new System.Drawing.Point(335, 120);
		this.emptySpaceItem46.Name = "emptySpaceItem46";
		this.emptySpaceItem46.Size = new System.Drawing.Size(129, 24);
		this.emptySpaceItem46.TextSize = new System.Drawing.Size(0, 0);
		this.postgreSqlDatabaseLayoutControlItem.Control = this.postgreSqlDatabaseButtonEdit;
		this.postgreSqlDatabaseLayoutControlItem.Location = new System.Drawing.Point(0, 168);
		this.postgreSqlDatabaseLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.postgreSqlDatabaseLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.postgreSqlDatabaseLayoutControlItem.Name = "postgreSqlDatabaseLayoutControlItem";
		this.postgreSqlDatabaseLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.postgreSqlDatabaseLayoutControlItem.Size = new System.Drawing.Size(464, 24);
		this.postgreSqlDatabaseLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.postgreSqlDatabaseLayoutControlItem.Text = "Database:";
		this.postgreSqlDatabaseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.postgreSqlDatabaseLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.postgreSqlDatabaseLayoutControlItem.TextToControlDistance = 5;
		this.configureSSLLayoutControlItem.Control = this.configureSimpleButton;
		this.configureSSLLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.configureSSLLayoutControlItem.CustomizationFormText = "Configure SSL:";
		this.configureSSLLayoutControlItem.Location = new System.Drawing.Point(0, 144);
		this.configureSSLLayoutControlItem.MaxSize = new System.Drawing.Size(183, 24);
		this.configureSSLLayoutControlItem.MinSize = new System.Drawing.Size(183, 24);
		this.configureSSLLayoutControlItem.Name = "configureSSLLayoutControlItem";
		this.configureSSLLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.configureSSLLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.configureSSLLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.configureSSLLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.configureSSLLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.configureSSLLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.configureSSLLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 5);
		this.configureSSLLayoutControlItem.Size = new System.Drawing.Size(464, 24);
		this.configureSSLLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.configureSSLLayoutControlItem.Text = "Configure SSL:";
		this.configureSSLLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.configureSSLLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.configureSSLLayoutControlItem.TextToControlDistance = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.postgresLayoutControl);
		base.Name = "PostgreSQLConnectorControl";
		base.Size = new System.Drawing.Size(464, 251);
		((System.ComponentModel.ISupportInitialize)this.postgresLayoutControl).EndInit();
		this.postgresLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.postgreSqlSSLModeLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlDatabaseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlLoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlPasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlPortTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlPasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlLoginLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem25).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem26).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem27).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem28).EndInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlTimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlPortLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem27).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem49).EndInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem51).EndInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlSSLModeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem46).EndInit();
		((System.ComponentModel.ISupportInitialize)this.postgreSqlDatabaseLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.configureSSLLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
