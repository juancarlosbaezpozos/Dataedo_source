using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classes;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class AthenaConnectorControl : ConnectorControlBase
{
	private bool testForGettingDataCatalog;

	private bool testForGettingWorkgroup;

	private IContainer components;

	private NonCustomizableLayoutControl sqlServerLayoutControl;

	private ButtonEdit sqlServerDatabaseButtonEdit;

	private CheckEdit sqlServerSavePasswordCheckEdit;

	private TextEdit loginTextEdit;

	private TextEdit sqlServerPasswordTextEdit;

	private ComboBoxEdit sqlServerHostComboBoxEdit;

	private LayoutControlGroup layoutControlGroup4;

	private LayoutControlItem sqlServerPasswordLayoutControlItem;

	private LayoutControlItem loginLayoutControlItem;

	private LayoutControlItem sqlServerSavePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem5;

	private EmptySpaceItem emptySpaceItem8;

	private EmptySpaceItem emptySpaceItem9;

	private LayoutControlItem sqlServerDatabaseLayoutControlItem;

	private LayoutControlItem hostLayoutControlItem;

	private EmptySpaceItem emptySpaceItem3;

	private EmptySpaceItem emptySpaceItem4;

	private TextEdit s3BucketTextEdit;

	private LayoutControlItem sqlServerPasswordLayoutControlItem1;

	private EmptySpaceItem sqlServerTimeoutEmptySpaceItem;

	private TextEdit workbookTextEdit;

	private LayoutControlItem sqlServerPasswordLayoutControlItem2;

	private TextEdit dataCatalogTextEdit;

	private LayoutControlItem layoutControlItem1;

	private ButtonEdit workgroupButtonEdit;

	private LayoutControlItem sqlServerDatabaseLayoutControlItem1;

	private ButtonEdit dataCatalogButtonEdit;

	private LayoutControlItem sqlServerDatabaseLayoutControlItem2;

	private string providedSqlServerHost => splittedHost?.Host ?? sqlServerHostComboBoxEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Athena;

	protected override TextEdit HostTextEdit => sqlServerHostComboBoxEdit;

	protected override ComboBoxEdit HostComboBoxEdit => sqlServerHostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => sqlServerSavePasswordCheckEdit;

	public AthenaConnectorControl()
	{
		InitializeComponent();
	}

	public override void SetParameters(int? databaseId = null, bool? isCopyingConnection = false, bool isExporting = false)
	{
		base.SetParameters(databaseId, isCopyingConnection, isExporting);
		foreach (string item in new List<string>
		{
			"us-east-2", "us-east-1", "us-west-1", "us-west-2", "af-south-1", "ap-east-1", "ap-southeast-3", "ap-south-1", "ap-northeast-3", "ap-northeast-2",
			"ap-southeast-1", "ap-southeast-2", "ap-northeast-1", "ca-central-1", "eu-central-1", "eu-west-1", "eu-west-2", "eu-south-1", "eu-west-3", "eu-north-1",
			"me-south-1", "sa-east-1"
		})
		{
			AddHostToItems(item);
		}
	}

	private void AddHostToItems(string host)
	{
		if (!sqlServerHostComboBoxEdit.Properties.Items.Contains(host))
		{
			sqlServerHostComboBoxEdit.Properties.Items.Add(host);
		}
	}

	public override void SetSwitchDatabaseAvailability(bool isAvailable)
	{
		sqlServerDatabaseButtonEdit.Enabled = isAvailable;
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? sqlServerDatabaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedSqlServerHost, loginTextEdit.Text, sqlServerPasswordTextEdit.Text, false, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, null);
		base.DatabaseRow.Param1 = workgroupButtonEdit.Text;
		base.DatabaseRow.Param2 = dataCatalogButtonEdit.Text;
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		sqlServerLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			sqlServerLayoutControl.Root.AddItem(timeoutLayoutControlItem, sqlServerTimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateAthenaHost();
		flag &= ValidateAthenaLogin();
		flag &= ValidateAthenaPassword(acceptEmptyValue: false);
		if (!testForGettingDataCatalog && !testForGettingWorkgroup && !testForGettingDatabasesList)
		{
			flag &= ValidateDataCatalog();
		}
		if (!testForGettingWorkgroup && !testForGettingDatabasesList && !testForGettingDataCatalog)
		{
			flag &= ValidateWorkgroup();
		}
		if (!testForGettingDatabasesList && !testForGettingWorkgroup && !testForGettingDataCatalog)
		{
			flag &= ValidateAthenaDatabase();
		}
		return flag;
	}

	private bool ValidateAthenaHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(sqlServerHostComboBoxEdit, addDBErrorProvider, "aws region", acceptEmptyValue);
	}

	private bool ValidateAthenaLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(loginTextEdit, addDBErrorProvider, "access token", acceptEmptyValue);
	}

	private bool ValidateAthenaPassword(bool acceptEmptyValue = true)
	{
		return ValidateFields.ValidateEdit(sqlServerPasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateAthenaDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(sqlServerDatabaseButtonEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	private bool ValidateWorkgroup(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(workgroupButtonEdit, addDBErrorProvider, "workgroup", acceptEmptyValue);
	}

	private bool ValidateDataCatalog(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(dataCatalogButtonEdit, addDBErrorProvider, "data catalog", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		sqlServerHostComboBoxEdit.Text = base.DatabaseRow.Host;
		sqlServerDatabaseButtonEdit.EditValue = base.DatabaseRow.Name;
		loginTextEdit.Text = base.DatabaseRow.User;
		sqlServerPasswordTextEdit.Text = value;
		workgroupButtonEdit.EditValue = base.DatabaseRow.Param1;
		dataCatalogButtonEdit.EditValue = base.DatabaseRow.Param2;
		sqlServerSavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return sqlServerDatabaseButtonEdit.Text + "@" + providedSqlServerHost;
	}

	private void sqlServerLoginTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateAthenaLogin(acceptEmptyValue: true);
	}

	private void sqlServerDatabaseButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateAthenaDatabase(acceptEmptyValue: true);
	}

	private void sqlServerPasswordTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateAthenaPassword();
	}

	private void sqlServerDatabaseButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		DatabaseButtonEdit_ButtonClick(sender, e);
	}

	private void sqlServerHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void sqlServerHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	private void sqlServerPortTextEdit_Leave(object sender, EventArgs e)
	{
		PortTextEdit_Leave(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		loginTextEdit.Text = string.Empty;
		sqlServerPasswordTextEdit.Text = string.Empty;
	}

	private void workgroupButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		string param = string.Empty;
		string text = workgroupButtonEdit.Text;
		workgroupButtonEdit.Text = string.Empty;
		try
		{
			testForGettingWorkgroup = true;
			if (!ValidateRequiredFields(testForGettingDatabasesList: false))
			{
				GeneralMessageBoxesHandling.Show("Required fields are not filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
				return;
			}
			SetNewDBRowValues(forGettingDatabasesList: true);
			if (base.SelectedDatabaseType.HasValue && DatabaseSupportFactory.GetDatabaseSupport(base.SelectedDatabaseType) is AthenaSupport athenaSupport)
			{
				ConnectionResult connectionResult = base.DatabaseRow.TryConnection(useOnlyRequiredFields: true);
				if (connectionResult.Exception != null)
				{
					throw connectionResult.Exception;
				}
				base.DatabaseRow.Connection = connectionResult.Connection;
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
				List<string> workgroupNames = athenaSupport.GetWorkgroupNames(base.DatabaseRow.ConnectionString, base.DatabaseRow.Connection, workgroupButtonEdit.Text, base.ParentForm);
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
				string title = "Workgroup list";
				ListForm listForm = new ListForm(workgroupNames, title);
				if (listForm.ShowDialog(this, setCustomMessageDefaultOwner: true) != DialogResult.OK)
				{
					workgroupButtonEdit.Text = text;
					return;
				}
				(sender as ButtonEdit).EditValue = listForm.SelectedValue;
				param = listForm.SelectedValue;
			}
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
		}
		finally
		{
			base.DatabaseRow.Param1 = param;
			testForGettingWorkgroup = false;
		}
	}

	private void dataCatalogButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		string param = string.Empty;
		string text = dataCatalogButtonEdit.Text;
		dataCatalogButtonEdit.Text = string.Empty;
		try
		{
			testForGettingDataCatalog = true;
			if (!ValidateRequiredFields(testForGettingDatabasesList: false))
			{
				GeneralMessageBoxesHandling.Show("Required fields are not filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
				return;
			}
			SetNewDBRowValues(forGettingDatabasesList: true);
			if (base.SelectedDatabaseType.HasValue && DatabaseSupportFactory.GetDatabaseSupport(base.SelectedDatabaseType) is AthenaSupport athenaSupport)
			{
				ConnectionResult connectionResult = base.DatabaseRow.TryConnection(useOnlyRequiredFields: true);
				if (connectionResult.Exception != null)
				{
					throw connectionResult.Exception;
				}
				base.DatabaseRow.Connection = connectionResult.Connection;
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
				List<string> dataCatalogNames = athenaSupport.GetDataCatalogNames(base.DatabaseRow.ConnectionString, base.DatabaseRow.Connection, workgroupButtonEdit.Text, base.ParentForm);
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
				string title = "Data catalogs list";
				ListForm listForm = new ListForm(dataCatalogNames, title);
				if (listForm.ShowDialog(this, setCustomMessageDefaultOwner: true) != DialogResult.OK)
				{
					dataCatalogButtonEdit.Text = text;
					return;
				}
				(sender as ButtonEdit).EditValue = listForm.SelectedValue;
				param = listForm.SelectedValue;
			}
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
		}
		finally
		{
			base.DatabaseRow.Param2 = param;
			testForGettingDataCatalog = false;
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
		this.sqlServerLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.sqlServerDatabaseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.sqlServerSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.loginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.sqlServerPasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.sqlServerHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.s3BucketTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.workbookTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.dataCatalogTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.workgroupButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.dataCatalogButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.sqlServerPasswordLayoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.sqlServerPasswordLayoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.sqlServerPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.loginLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sqlServerSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem9 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.sqlServerDatabaseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sqlServerTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.hostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.sqlServerDatabaseLayoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.sqlServerDatabaseLayoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.sqlServerLayoutControl).BeginInit();
		this.sqlServerLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.sqlServerDatabaseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.loginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.s3BucketTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.workbookTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataCatalogTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.workgroupButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataCatalogButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPasswordLayoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPasswordLayoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.loginLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerDatabaseLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerTimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerDatabaseLayoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerDatabaseLayoutControlItem1).BeginInit();
		base.SuspendLayout();
		this.sqlServerLayoutControl.AllowCustomization = false;
		this.sqlServerLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.sqlServerLayoutControl.Controls.Add(this.sqlServerDatabaseButtonEdit);
		this.sqlServerLayoutControl.Controls.Add(this.sqlServerSavePasswordCheckEdit);
		this.sqlServerLayoutControl.Controls.Add(this.loginTextEdit);
		this.sqlServerLayoutControl.Controls.Add(this.sqlServerPasswordTextEdit);
		this.sqlServerLayoutControl.Controls.Add(this.sqlServerHostComboBoxEdit);
		this.sqlServerLayoutControl.Controls.Add(this.s3BucketTextEdit);
		this.sqlServerLayoutControl.Controls.Add(this.workbookTextEdit);
		this.sqlServerLayoutControl.Controls.Add(this.dataCatalogTextEdit);
		this.sqlServerLayoutControl.Controls.Add(this.workgroupButtonEdit);
		this.sqlServerLayoutControl.Controls.Add(this.dataCatalogButtonEdit);
		this.sqlServerLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.sqlServerLayoutControl.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.sqlServerPasswordLayoutControlItem1, this.sqlServerPasswordLayoutControlItem2, this.layoutControlItem1 });
		this.sqlServerLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.sqlServerLayoutControl.Name = "sqlServerLayoutControl";
		this.sqlServerLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1179, 179, 279, 548);
		this.sqlServerLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.sqlServerLayoutControl.Root = this.layoutControlGroup4;
		this.sqlServerLayoutControl.Size = new System.Drawing.Size(499, 322);
		this.sqlServerLayoutControl.TabIndex = 3;
		this.sqlServerLayoutControl.Text = "layoutControl1";
		this.sqlServerDatabaseButtonEdit.Location = new System.Drawing.Point(105, 144);
		this.sqlServerDatabaseButtonEdit.Name = "sqlServerDatabaseButtonEdit";
		this.sqlServerDatabaseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.sqlServerDatabaseButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.sqlServerDatabaseButtonEdit.StyleController = this.sqlServerLayoutControl;
		this.sqlServerDatabaseButtonEdit.TabIndex = 4;
		this.sqlServerDatabaseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(sqlServerDatabaseButtonEdit_ButtonClick);
		this.sqlServerDatabaseButtonEdit.EditValueChanged += new System.EventHandler(sqlServerDatabaseButtonEdit_EditValueChanged);
		this.sqlServerSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 72);
		this.sqlServerSavePasswordCheckEdit.Name = "sqlServerSavePasswordCheckEdit";
		this.sqlServerSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.sqlServerSavePasswordCheckEdit.Size = new System.Drawing.Size(230, 20);
		this.sqlServerSavePasswordCheckEdit.StyleController = this.sqlServerLayoutControl;
		this.sqlServerSavePasswordCheckEdit.TabIndex = 3;
		this.loginTextEdit.Location = new System.Drawing.Point(105, 24);
		this.loginTextEdit.Name = "loginTextEdit";
		this.loginTextEdit.Properties.AllowFocused = false;
		this.loginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.loginTextEdit.StyleController = this.sqlServerLayoutControl;
		this.loginTextEdit.TabIndex = 1;
		this.loginTextEdit.EditValueChanged += new System.EventHandler(sqlServerLoginTextEdit_EditValueChanged);
		this.sqlServerPasswordTextEdit.Location = new System.Drawing.Point(105, 48);
		this.sqlServerPasswordTextEdit.Name = "sqlServerPasswordTextEdit";
		this.sqlServerPasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.sqlServerPasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.sqlServerPasswordTextEdit.StyleController = this.sqlServerLayoutControl;
		this.sqlServerPasswordTextEdit.TabIndex = 2;
		this.sqlServerPasswordTextEdit.EditValueChanged += new System.EventHandler(sqlServerPasswordTextEdit_EditValueChanged);
		this.sqlServerHostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.sqlServerHostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.sqlServerHostComboBoxEdit.Name = "sqlServerHostComboBoxEdit";
		this.sqlServerHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.sqlServerHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.sqlServerHostComboBoxEdit.StyleController = this.sqlServerLayoutControl;
		this.sqlServerHostComboBoxEdit.TabIndex = 1;
		this.sqlServerHostComboBoxEdit.EditValueChanged += new System.EventHandler(sqlServerHostComboBoxEdit_EditValueChanged);
		this.sqlServerHostComboBoxEdit.Leave += new System.EventHandler(sqlServerHostComboBoxEdit_Leave);
		this.s3BucketTextEdit.Location = new System.Drawing.Point(104, 24);
		this.s3BucketTextEdit.Name = "s3BucketTextEdit";
		this.s3BucketTextEdit.Size = new System.Drawing.Size(232, 20);
		this.s3BucketTextEdit.StyleController = this.sqlServerLayoutControl;
		this.s3BucketTextEdit.TabIndex = 2;
		this.workbookTextEdit.Location = new System.Drawing.Point(104, 24);
		this.workbookTextEdit.Name = "workbookTextEdit";
		this.workbookTextEdit.Size = new System.Drawing.Size(232, 20);
		this.workbookTextEdit.StyleController = this.sqlServerLayoutControl;
		this.workbookTextEdit.TabIndex = 2;
		this.dataCatalogTextEdit.Location = new System.Drawing.Point(104, 73);
		this.dataCatalogTextEdit.Name = "dataCatalogTextEdit";
		this.dataCatalogTextEdit.Size = new System.Drawing.Size(232, 20);
		this.dataCatalogTextEdit.StyleController = this.sqlServerLayoutControl;
		this.dataCatalogTextEdit.TabIndex = 2;
		this.workgroupButtonEdit.Location = new System.Drawing.Point(105, 120);
		this.workgroupButtonEdit.Name = "workgroupButtonEdit";
		this.workgroupButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.workgroupButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.workgroupButtonEdit.StyleController = this.sqlServerLayoutControl;
		this.workgroupButtonEdit.TabIndex = 4;
		this.workgroupButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(workgroupButtonEdit_ButtonClick);
		this.dataCatalogButtonEdit.Location = new System.Drawing.Point(105, 96);
		this.dataCatalogButtonEdit.Name = "dataCatalogButtonEdit";
		this.dataCatalogButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.dataCatalogButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.dataCatalogButtonEdit.StyleController = this.sqlServerLayoutControl;
		this.dataCatalogButtonEdit.TabIndex = 4;
		this.dataCatalogButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(dataCatalogButtonEdit_ButtonClick);
		this.sqlServerPasswordLayoutControlItem1.Control = this.s3BucketTextEdit;
		this.sqlServerPasswordLayoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.sqlServerPasswordLayoutControlItem1.CustomizationFormText = "S3 bucket";
		this.sqlServerPasswordLayoutControlItem1.Location = new System.Drawing.Point(0, 30);
		this.sqlServerPasswordLayoutControlItem1.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerPasswordLayoutControlItem1.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerPasswordLayoutControlItem1.Name = "sqlServerPasswordLayoutControlItem1";
		this.sqlServerPasswordLayoutControlItem1.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.sqlServerPasswordLayoutControlItem1.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.sqlServerPasswordLayoutControlItem1.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.sqlServerPasswordLayoutControlItem1.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.sqlServerPasswordLayoutControlItem1.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.sqlServerPasswordLayoutControlItem1.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.sqlServerPasswordLayoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerPasswordLayoutControlItem1.Size = new System.Drawing.Size(665, 30);
		this.sqlServerPasswordLayoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerPasswordLayoutControlItem1.Text = "S3 bucket:";
		this.sqlServerPasswordLayoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerPasswordLayoutControlItem1.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerPasswordLayoutControlItem1.TextToControlDistance = 5;
		this.sqlServerPasswordLayoutControlItem2.Control = this.workbookTextEdit;
		this.sqlServerPasswordLayoutControlItem2.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.sqlServerPasswordLayoutControlItem2.CustomizationFormText = "S3 bucket";
		this.sqlServerPasswordLayoutControlItem2.Location = new System.Drawing.Point(0, 30);
		this.sqlServerPasswordLayoutControlItem2.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerPasswordLayoutControlItem2.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerPasswordLayoutControlItem2.Name = "sqlServerPasswordLayoutControlItem2";
		this.sqlServerPasswordLayoutControlItem2.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.sqlServerPasswordLayoutControlItem2.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.sqlServerPasswordLayoutControlItem2.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.sqlServerPasswordLayoutControlItem2.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.sqlServerPasswordLayoutControlItem2.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.sqlServerPasswordLayoutControlItem2.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.sqlServerPasswordLayoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerPasswordLayoutControlItem2.Size = new System.Drawing.Size(665, 30);
		this.sqlServerPasswordLayoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerPasswordLayoutControlItem2.Text = "Workgroup:";
		this.sqlServerPasswordLayoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerPasswordLayoutControlItem2.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerPasswordLayoutControlItem2.TextToControlDistance = 5;
		this.layoutControlItem1.Control = this.dataCatalogTextEdit;
		this.layoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.layoutControlItem1.CustomizationFormText = "Data catalog:";
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 90);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(335, 24);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.layoutControlItem1.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.layoutControlItem1.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.layoutControlItem1.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.layoutControlItem1.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.layoutControlItem1.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem1.Size = new System.Drawing.Size(665, 30);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.Text = "Data catalog:";
		this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem1.TextToControlDistance = 5;
		this.layoutControlGroup4.CustomizationFormText = "Root";
		this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup4.GroupBordersVisible = false;
		this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[14]
		{
			this.sqlServerPasswordLayoutControlItem, this.loginLayoutControlItem, this.sqlServerSavePasswordLayoutControlItem, this.emptySpaceItem1, this.emptySpaceItem5, this.emptySpaceItem8, this.emptySpaceItem9, this.sqlServerDatabaseLayoutControlItem, this.sqlServerTimeoutEmptySpaceItem, this.hostLayoutControlItem,
			this.emptySpaceItem3, this.emptySpaceItem4, this.sqlServerDatabaseLayoutControlItem2, this.sqlServerDatabaseLayoutControlItem1
		});
		this.layoutControlGroup4.Name = "Root";
		this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup4.Size = new System.Drawing.Size(499, 322);
		this.layoutControlGroup4.TextVisible = false;
		this.sqlServerPasswordLayoutControlItem.Control = this.sqlServerPasswordTextEdit;
		this.sqlServerPasswordLayoutControlItem.CustomizationFormText = "Secret token";
		this.sqlServerPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.sqlServerPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerPasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerPasswordLayoutControlItem.Name = "sqlServerPasswordLayoutControlItem";
		this.sqlServerPasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerPasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sqlServerPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerPasswordLayoutControlItem.Text = "Secret token:";
		this.sqlServerPasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerPasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerPasswordLayoutControlItem.TextToControlDistance = 5;
		this.loginLayoutControlItem.Control = this.loginTextEdit;
		this.loginLayoutControlItem.CustomizationFormText = "Access token:";
		this.loginLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.loginLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.loginLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.loginLayoutControlItem.Name = "loginLayoutControlItem";
		this.loginLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.loginLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.loginLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.loginLayoutControlItem.Text = "Access token:";
		this.loginLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.loginLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.loginLayoutControlItem.TextToControlDistance = 5;
		this.sqlServerSavePasswordLayoutControlItem.Control = this.sqlServerSavePasswordCheckEdit;
		this.sqlServerSavePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem2";
		this.sqlServerSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.sqlServerSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerSavePasswordLayoutControlItem.Name = "sqlServerSavePasswordLayoutControlItem";
		this.sqlServerSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerSavePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sqlServerSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerSavePasswordLayoutControlItem.Text = " ";
		this.sqlServerSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem6";
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 209);
		this.emptySpaceItem1.Name = "emptySpaceItem6";
		this.emptySpaceItem1.Size = new System.Drawing.Size(499, 113);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.CustomizationFormText = "emptySpaceItem21";
		this.emptySpaceItem5.Location = new System.Drawing.Point(335, 24);
		this.emptySpaceItem5.Name = "emptySpaceItem21";
		this.emptySpaceItem5.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem8.AllowHotTrack = false;
		this.emptySpaceItem8.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem8.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem8.Name = "emptySpaceItem22";
		this.emptySpaceItem8.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem9.AllowHotTrack = false;
		this.emptySpaceItem9.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem9.Location = new System.Drawing.Point(335, 139);
		this.emptySpaceItem9.Name = "emptySpaceItem28";
		this.emptySpaceItem9.Size = new System.Drawing.Size(164, 29);
		this.emptySpaceItem9.Text = "emptySpaceItem22";
		this.emptySpaceItem9.TextSize = new System.Drawing.Size(0, 0);
		this.sqlServerDatabaseLayoutControlItem.Control = this.sqlServerDatabaseButtonEdit;
		this.sqlServerDatabaseLayoutControlItem.Location = new System.Drawing.Point(0, 144);
		this.sqlServerDatabaseLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerDatabaseLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerDatabaseLayoutControlItem.Name = "sqlServerDatabaseLayoutControlItem";
		this.sqlServerDatabaseLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerDatabaseLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.sqlServerDatabaseLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerDatabaseLayoutControlItem.Text = "Database:";
		this.sqlServerDatabaseLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerDatabaseLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerDatabaseLayoutControlItem.TextToControlDistance = 5;
		this.sqlServerTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.sqlServerTimeoutEmptySpaceItem.CustomizationFormText = "emptySpaceItem12";
		this.sqlServerTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 168);
		this.sqlServerTimeoutEmptySpaceItem.Name = "sqlServerTimeoutEmptySpaceItem";
		this.sqlServerTimeoutEmptySpaceItem.Size = new System.Drawing.Size(499, 41);
		this.sqlServerTimeoutEmptySpaceItem.Text = "emptySpaceItem12";
		this.sqlServerTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.hostLayoutControlItem.Control = this.sqlServerHostComboBoxEdit;
		this.hostLayoutControlItem.CustomizationFormText = "AWS region:";
		this.hostLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.hostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.hostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.hostLayoutControlItem.Name = "hostLayoutControlItem";
		this.hostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.hostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.hostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.hostLayoutControlItem.Text = "AWS region:";
		this.hostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.hostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.hostLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(164, 24);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(335, 72);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(164, 67);
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.sqlServerDatabaseLayoutControlItem2.Control = this.dataCatalogButtonEdit;
		this.sqlServerDatabaseLayoutControlItem2.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.sqlServerDatabaseLayoutControlItem2.CustomizationFormText = "Data catalog:";
		this.sqlServerDatabaseLayoutControlItem2.Location = new System.Drawing.Point(0, 96);
		this.sqlServerDatabaseLayoutControlItem2.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerDatabaseLayoutControlItem2.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerDatabaseLayoutControlItem2.Name = "sqlServerDatabaseLayoutControlItem2";
		this.sqlServerDatabaseLayoutControlItem2.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.sqlServerDatabaseLayoutControlItem2.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.sqlServerDatabaseLayoutControlItem2.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.sqlServerDatabaseLayoutControlItem2.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.sqlServerDatabaseLayoutControlItem2.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.sqlServerDatabaseLayoutControlItem2.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.sqlServerDatabaseLayoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerDatabaseLayoutControlItem2.Size = new System.Drawing.Size(335, 24);
		this.sqlServerDatabaseLayoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerDatabaseLayoutControlItem2.Text = "Data catalog:";
		this.sqlServerDatabaseLayoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerDatabaseLayoutControlItem2.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerDatabaseLayoutControlItem2.TextToControlDistance = 5;
		this.sqlServerDatabaseLayoutControlItem1.Control = this.workgroupButtonEdit;
		this.sqlServerDatabaseLayoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.sqlServerDatabaseLayoutControlItem1.CustomizationFormText = "Workgroup:";
		this.sqlServerDatabaseLayoutControlItem1.Location = new System.Drawing.Point(0, 120);
		this.sqlServerDatabaseLayoutControlItem1.MaxSize = new System.Drawing.Size(335, 24);
		this.sqlServerDatabaseLayoutControlItem1.MinSize = new System.Drawing.Size(335, 24);
		this.sqlServerDatabaseLayoutControlItem1.Name = "sqlServerDatabaseLayoutControlItem1";
		this.sqlServerDatabaseLayoutControlItem1.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.sqlServerDatabaseLayoutControlItem1.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.sqlServerDatabaseLayoutControlItem1.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.sqlServerDatabaseLayoutControlItem1.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.sqlServerDatabaseLayoutControlItem1.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8f);
		this.sqlServerDatabaseLayoutControlItem1.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.sqlServerDatabaseLayoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.sqlServerDatabaseLayoutControlItem1.Size = new System.Drawing.Size(335, 24);
		this.sqlServerDatabaseLayoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sqlServerDatabaseLayoutControlItem1.Text = "Workgroup:";
		this.sqlServerDatabaseLayoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.sqlServerDatabaseLayoutControlItem1.TextSize = new System.Drawing.Size(100, 13);
		this.sqlServerDatabaseLayoutControlItem1.TextToControlDistance = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.sqlServerLayoutControl);
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "AthenaConnectorControl";
		base.Size = new System.Drawing.Size(499, 322);
		((System.ComponentModel.ISupportInitialize)this.sqlServerLayoutControl).EndInit();
		this.sqlServerLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.sqlServerDatabaseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.loginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.s3BucketTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.workbookTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataCatalogTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.workgroupButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataCatalogButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPasswordLayoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPasswordLayoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerPasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.loginLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerDatabaseLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerTimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerDatabaseLayoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sqlServerDatabaseLayoutControlItem1).EndInit();
		base.ResumeLayout(false);
	}
}
