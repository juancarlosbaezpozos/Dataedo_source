using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.Enums;
using Dataedo.App.Tools.MessageBoxes;
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

public class TeradataConnectorControl : ConnectorControlBase
{
	private bool? _isQVCIEnabled;

	private IContainer components;

	private NonCustomizableLayoutControl teradataLayoutControl;

	private LabelControl teradataDatabasesCountLabelControl;

	private ButtonEdit teradataDatabaseButtonEdit;

	private LabelControl teradataSqlDefaultPortLabelControl;

	private TextEdit teradataPortTextEdit;

	private TextEdit teradataPasswordTextEdit;

	private TextEdit teradataLoginTextEdit;

	private ComboBoxEdit teradataHostComboBoxEdit;

	private CheckEdit teradataSavePasswordCheckEdit;

	private LayoutControlGroup teradataRootLayoutControlGroup;

	private LayoutControlItem teradataHostLayoutControlItem;

	private LayoutControlItem teradataLoginLayoutControlItem;

	private LayoutControlItem teradataPasswordLayoutControlItem;

	private EmptySpaceItem teradataBottomEmptySpaceItem;

	private LayoutControlItem teradataPortLayoutControlItem;

	private EmptySpaceItem teradataEmptySpaceItem;

	private EmptySpaceItem teradataDefaultPortEmptySpaceItem;

	private EmptySpaceItem teradataLoginEmptySpaceItem;

	private EmptySpaceItem teradataPortEmptySpaceItem;

	private LayoutControlItem taradataDefaultPortLayoutControlItem;

	private EmptySpaceItem teradataPasswordEmptySpaceItem;

	private EmptySpaceItem teradataSavePasswordEmptySpaceItem;

	private LayoutControlItem teradataSavePasswordLayoutControlItem;

	private LayoutControlItem teradataDatabaseLayoutControl;

	private EmptySpaceItem teradataTimeoutEmptySpaceItem;

	private EmptySpaceItem teradataDatabaseEmptySpaceItem1;

	private EmptySpaceItem teradataDatabaseEmptySpaceItem2;

	private LayoutControlItem teradataDatabasesCountLabelControlLayoutControlItem;

	private string providedTeradataHost => splittedHost?.Host ?? teradataHostComboBoxEdit.Text;

	private string providedTeradataPort => teradataPortTextEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Teradata;

	private bool? IsQVCIEnabled
	{
		get
		{
			if (!_isQVCIEnabled.HasValue && DatabaseSupportFactory.GetDatabaseSupport(base.DatabaseRow.Type) is TeradataSupport teradataSupport)
			{
				_isQVCIEnabled = teradataSupport.CheckIfQVCIEnabled(() => base.DatabaseRow.ConnectionString);
			}
			return _isQVCIEnabled;
		}
		set
		{
			_isQVCIEnabled = value;
		}
	}

	protected override TextEdit HostTextEdit => teradataHostComboBoxEdit;

	protected override TextEdit PortTextEdit => teradataPortTextEdit;

	protected override ComboBoxEdit HostComboBoxEdit => teradataHostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => teradataSavePasswordCheckEdit;

	public TeradataConnectorControl()
	{
		InitializeComponent();
	}

	public override bool TestConnection(bool testForGettingDatabasesList)
	{
		try
		{
			if (!ValidateRequiredFields(testForGettingDatabasesList))
			{
				GeneralMessageBoxesHandling.Show("Required fields are not filled in.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
				return false;
			}
			SetNewDBRowValues();
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
			ConnectionResult connectionResult = base.DatabaseRow.TryConnection();
			if (connectionResult.IsSuccess)
			{
				base.DatabaseRow.Connection = connectionResult.Connection;
				SetDBMSVersion();
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
				SharedDatabaseTypeEnum.DatabaseType? expectedDatabaseType = GetExpectedDatabaseType();
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
				if (!expectedDatabaseType.HasValue)
				{
					return false;
				}
				if (expectedDatabaseType != base.DatabaseRow.Type)
				{
					base.DatabaseRow.Type = expectedDatabaseType;
					base.SelectedDatabaseType = expectedDatabaseType;
				}
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
				if (IsQVCIEnabled != true)
				{
					GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("Due to your Teradata configuration, column data type information for views will be missing. To import these details enable QVCI feature and retry." + Environment.NewLine + Environment.NewLine + "Do you want to contiue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, FindForm());
					if (handlingDialogResult == null || handlingDialogResult.DialogResult != DialogResult.Yes)
					{
						IsQVCIEnabled = null;
						return false;
					}
				}
				base.DatabaseRow.Param1 = IsQVCIEnabled.ToString();
				CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: true);
				return true;
			}
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
			GeneralMessageBoxesHandling.Show(connectionResult.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, connectionResult.Details, 1, messageSimple: connectionResult.MessageSimple, owner: FindForm());
			return false;
		}
		catch
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
			throw;
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(base.SplashScreenManager, show: false);
		}
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? teradataDatabaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedTeradataHost, teradataLoginTextEdit.Text, teradataPasswordTextEdit.Text, providedTeradataPort, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, null, null, null, teradataDatabaseButtonEdit.Text.Split(',').ToList());
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		teradataLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			teradataLayoutControl.Root.AddItem(timeoutLayoutControlItem, teradataTimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateTeradataHost();
		flag &= ValidateTeradataLogin();
		flag &= ValidateTeradataPort();
		flag &= ValidateTeradataPassword();
		if (!testForGettingDatabasesList)
		{
			flag &= ValidateTeradataDatabase();
		}
		return flag;
	}

	private bool ValidateTeradataHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(teradataHostComboBoxEdit, addDBErrorProvider, "host", acceptEmptyValue);
	}

	private bool ValidateTeradataPassword(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(teradataPasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateTeradataLogin(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(teradataLoginTextEdit, addDBErrorProvider, "user", acceptEmptyValue);
	}

	private bool ValidateTeradataPort(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(teradataPortTextEdit, addDBErrorProvider, "port", acceptEmptyValue);
	}

	private bool ValidateTeradataDatabase(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(teradataDatabaseButtonEdit, addDBErrorProvider, "database", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		teradataHostComboBoxEdit.Text = base.DatabaseRow.Host;
		teradataDatabaseButtonEdit.EditValue = base.DatabaseRow.Name;
		teradataPortTextEdit.Text = PrepareValue.ToString(base.DatabaseRow.Port) ?? DatabaseTypeEnum.GetDefaultPort(base.DatabaseRow.Type);
		teradataLoginTextEdit.Text = base.DatabaseRow.User;
		teradataPasswordTextEdit.Text = base.DatabaseRow.Password;
		teradataSavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
		schemas = new List<string>(base.DatabaseRow.Schemas ?? new List<string>());
		SetSchemasButtonEdit(teradataDatabaseButtonEdit);
		SetElementsLabelControl(teradataDatabasesCountLabelControl, "databases");
	}

	protected override string GetPanelDocumentationTitle()
	{
		return teradataDatabaseButtonEdit.Text + "@" + teradataHostComboBoxEdit.Text;
	}

	private void teradataSqlDefaultPortLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			teradataPortTextEdit.Text = DatabaseTypeEnum.GetDefaultPort(SharedDatabaseTypeEnum.DatabaseType.Teradata);
		}
	}

	private void teradataDatabaseButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		HandleSchemaButtonEdit(sender as ButtonEdit, testConnection: false, "Databases", teradataDatabasesCountLabelControl);
	}

	private void teradataDatabaseButtonEdit_Leave(object sender, EventArgs e)
	{
		SetElementsLabelControl(teradataDatabasesCountLabelControl, "databases");
	}

	private void teradataDatabaseButtonEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateTeradataDatabase(acceptEmptyValue: true);
		schemas = ConnectorControlBase.GetPrintedSchemas(teradataDatabaseButtonEdit);
	}

	private void teradataHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void teradataHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	private void teradataPortTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		PortTextEdit_EditValueChanged(sender, e);
	}

	private void teradataPortTextEdit_Leave(object sender, EventArgs e)
	{
		PortTextEdit_Leave(sender, e);
	}

	protected override ButtonEdit GetButtonEditForSchemasPreparation()
	{
		if (base.DatabaseRow.Type != SharedDatabaseTypeEnum.DatabaseType.Teradata)
		{
			return null;
		}
		return teradataDatabaseButtonEdit;
	}

	protected override LabelControl GetLabelControlForSchemasPreparation()
	{
		if (base.DatabaseRow.Type != SharedDatabaseTypeEnum.DatabaseType.Teradata)
		{
			return null;
		}
		return teradataDatabasesCountLabelControl;
	}

	protected override void ClearPanelLoginAndPassword()
	{
		teradataLoginTextEdit.Text = string.Empty;
		teradataPasswordTextEdit.Text = string.Empty;
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
		this.teradataLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.teradataDatabasesCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.teradataDatabaseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.teradataSqlDefaultPortLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.teradataPortTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.teradataPasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.teradataLoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.teradataHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.teradataSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.teradataRootLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.teradataHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.teradataLoginLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.teradataPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.teradataBottomEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.teradataPortLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.teradataEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.teradataDefaultPortEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.teradataLoginEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.teradataPortEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.taradataDefaultPortLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.teradataPasswordEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.teradataSavePasswordEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.teradataSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.teradataDatabaseLayoutControl = new DevExpress.XtraLayout.LayoutControlItem();
		this.teradataTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.teradataDatabaseEmptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.teradataDatabaseEmptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.teradataDatabasesCountLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.teradataLayoutControl).BeginInit();
		this.teradataLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.teradataDatabaseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataPortTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataPasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataLoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataRootLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataLoginLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataPasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataBottomEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataPortLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataDefaultPortEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataLoginEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataPortEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.taradataDefaultPortLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataPasswordEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataSavePasswordEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataDatabaseLayoutControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataTimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataDatabaseEmptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataDatabaseEmptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.teradataDatabasesCountLabelControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.teradataLayoutControl.AllowCustomization = false;
		this.teradataLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.teradataLayoutControl.Controls.Add(this.teradataDatabasesCountLabelControl);
		this.teradataLayoutControl.Controls.Add(this.teradataDatabaseButtonEdit);
		this.teradataLayoutControl.Controls.Add(this.teradataSqlDefaultPortLabelControl);
		this.teradataLayoutControl.Controls.Add(this.teradataPortTextEdit);
		this.teradataLayoutControl.Controls.Add(this.teradataPasswordTextEdit);
		this.teradataLayoutControl.Controls.Add(this.teradataLoginTextEdit);
		this.teradataLayoutControl.Controls.Add(this.teradataHostComboBoxEdit);
		this.teradataLayoutControl.Controls.Add(this.teradataSavePasswordCheckEdit);
		this.teradataLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.teradataLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.teradataLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.teradataLayoutControl.Name = "teradataLayoutControl";
		this.teradataLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(666, 137, 884, 864);
		this.teradataLayoutControl.Root = this.teradataRootLayoutControlGroup;
		this.teradataLayoutControl.Size = new System.Drawing.Size(508, 306);
		this.teradataLayoutControl.TabIndex = 3;
		this.teradataLayoutControl.Text = "layoutControl3";
		this.teradataDatabasesCountLabelControl.Location = new System.Drawing.Point(346, 122);
		this.teradataDatabasesCountLabelControl.Name = "teradataDatabasesCountLabelControl";
		this.teradataDatabasesCountLabelControl.Size = new System.Drawing.Size(74, 13);
		this.teradataDatabasesCountLabelControl.StyleController = this.teradataLayoutControl;
		this.teradataDatabasesCountLabelControl.TabIndex = 34;
		this.teradataDatabaseButtonEdit.Location = new System.Drawing.Point(105, 120);
		this.teradataDatabaseButtonEdit.Name = "teradataDatabaseButtonEdit";
		this.teradataDatabaseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.teradataDatabaseButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.teradataDatabaseButtonEdit.StyleController = this.teradataLayoutControl;
		this.teradataDatabaseButtonEdit.TabIndex = 33;
		this.teradataDatabaseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(teradataDatabaseButtonEdit_ButtonClick);
		this.teradataDatabaseButtonEdit.EditValueChanged += new System.EventHandler(teradataDatabaseButtonEdit_EditValueChanged);
		this.teradataDatabaseButtonEdit.Leave += new System.EventHandler(teradataDatabaseButtonEdit_Leave);
		this.teradataSqlDefaultPortLabelControl.AllowHtmlString = true;
		this.teradataSqlDefaultPortLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.teradataSqlDefaultPortLabelControl.Location = new System.Drawing.Point(346, 26);
		this.teradataSqlDefaultPortLabelControl.MinimumSize = new System.Drawing.Size(34, 20);
		this.teradataSqlDefaultPortLabelControl.Name = "teradataSqlDefaultPortLabelControl";
		this.teradataSqlDefaultPortLabelControl.Size = new System.Drawing.Size(34, 20);
		this.teradataSqlDefaultPortLabelControl.StyleController = this.teradataLayoutControl;
		this.teradataSqlDefaultPortLabelControl.TabIndex = 32;
		this.teradataSqlDefaultPortLabelControl.Text = "<href>default</href>";
		this.teradataSqlDefaultPortLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(teradataSqlDefaultPortLabelControl_HyperlinkClick);
		this.teradataPortTextEdit.EditValue = "1025";
		this.teradataPortTextEdit.Location = new System.Drawing.Point(105, 24);
		this.teradataPortTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.teradataPortTextEdit.Name = "teradataPortTextEdit";
		this.teradataPortTextEdit.Properties.Mask.EditMask = "\\d+";
		this.teradataPortTextEdit.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.teradataPortTextEdit.Properties.Mask.ShowPlaceHolders = false;
		this.teradataPortTextEdit.Properties.MaxLength = 5;
		this.teradataPortTextEdit.Size = new System.Drawing.Size(230, 20);
		this.teradataPortTextEdit.StyleController = this.teradataLayoutControl;
		this.teradataPortTextEdit.TabIndex = 5;
		this.teradataPortTextEdit.EditValueChanged += new System.EventHandler(teradataPortTextEdit_EditValueChanged);
		this.teradataPortTextEdit.Leave += new System.EventHandler(teradataPortTextEdit_Leave);
		this.teradataPasswordTextEdit.Location = new System.Drawing.Point(105, 72);
		this.teradataPasswordTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.teradataPasswordTextEdit.Name = "teradataPasswordTextEdit";
		this.teradataPasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.teradataPasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.teradataPasswordTextEdit.StyleController = this.teradataLayoutControl;
		this.teradataPasswordTextEdit.TabIndex = 6;
		this.teradataLoginTextEdit.Location = new System.Drawing.Point(105, 48);
		this.teradataLoginTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.teradataLoginTextEdit.Name = "teradataLoginTextEdit";
		this.teradataLoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.teradataLoginTextEdit.StyleController = this.teradataLayoutControl;
		this.teradataLoginTextEdit.TabIndex = 5;
		this.teradataHostComboBoxEdit.Location = new System.Drawing.Point(105, 0);
		this.teradataHostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.teradataHostComboBoxEdit.Name = "teradataHostComboBoxEdit";
		this.teradataHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.teradataHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.teradataHostComboBoxEdit.StyleController = this.teradataLayoutControl;
		this.teradataHostComboBoxEdit.TabIndex = 4;
		this.teradataHostComboBoxEdit.EditValueChanged += new System.EventHandler(teradataHostComboBoxEdit_EditValueChanged);
		this.teradataHostComboBoxEdit.Leave += new System.EventHandler(teradataHostComboBoxEdit_Leave);
		this.teradataSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 96);
		this.teradataSavePasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 20);
		this.teradataSavePasswordCheckEdit.MinimumSize = new System.Drawing.Size(95, 20);
		this.teradataSavePasswordCheckEdit.Name = "teradataSavePasswordCheckEdit";
		this.teradataSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.teradataSavePasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.teradataSavePasswordCheckEdit.StyleController = this.teradataLayoutControl;
		this.teradataSavePasswordCheckEdit.TabIndex = 3;
		this.teradataRootLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.teradataRootLayoutControlGroup.GroupBordersVisible = false;
		this.teradataRootLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[18]
		{
			this.teradataHostLayoutControlItem, this.teradataLoginLayoutControlItem, this.teradataPasswordLayoutControlItem, this.teradataBottomEmptySpaceItem, this.teradataPortLayoutControlItem, this.teradataEmptySpaceItem, this.teradataDefaultPortEmptySpaceItem, this.teradataLoginEmptySpaceItem, this.teradataPortEmptySpaceItem, this.taradataDefaultPortLayoutControlItem,
			this.teradataPasswordEmptySpaceItem, this.teradataSavePasswordEmptySpaceItem, this.teradataSavePasswordLayoutControlItem, this.teradataDatabaseLayoutControl, this.teradataTimeoutEmptySpaceItem, this.teradataDatabaseEmptySpaceItem1, this.teradataDatabaseEmptySpaceItem2, this.teradataDatabasesCountLabelControlLayoutControlItem
		});
		this.teradataRootLayoutControlGroup.Name = "Root";
		this.teradataRootLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.teradataRootLayoutControlGroup.Size = new System.Drawing.Size(508, 306);
		this.teradataRootLayoutControlGroup.TextVisible = false;
		this.teradataHostLayoutControlItem.Control = this.teradataHostComboBoxEdit;
		this.teradataHostLayoutControlItem.CustomizationFormText = "Host";
		this.teradataHostLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.teradataHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.teradataHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.teradataHostLayoutControlItem.Name = "teradataHostLayoutControlItem";
		this.teradataHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.teradataHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.teradataHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.teradataHostLayoutControlItem.Text = "Host:";
		this.teradataHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.teradataHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.teradataHostLayoutControlItem.TextToControlDistance = 5;
		this.teradataLoginLayoutControlItem.Control = this.teradataLoginTextEdit;
		this.teradataLoginLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.teradataLoginLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.teradataLoginLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.teradataLoginLayoutControlItem.Name = "teradataLoginLayoutControlItem";
		this.teradataLoginLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.teradataLoginLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.teradataLoginLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.teradataLoginLayoutControlItem.Text = "User:";
		this.teradataLoginLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.teradataLoginLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.teradataLoginLayoutControlItem.TextToControlDistance = 5;
		this.teradataPasswordLayoutControlItem.Control = this.teradataPasswordTextEdit;
		this.teradataPasswordLayoutControlItem.CustomizationFormText = "Password:";
		this.teradataPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.teradataPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.teradataPasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.teradataPasswordLayoutControlItem.Name = "teradataPasswordLayoutControlItem";
		this.teradataPasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.teradataPasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.teradataPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.teradataPasswordLayoutControlItem.Text = "Password:";
		this.teradataPasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.teradataPasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.teradataPasswordLayoutControlItem.TextToControlDistance = 5;
		this.teradataBottomEmptySpaceItem.AllowHotTrack = false;
		this.teradataBottomEmptySpaceItem.Location = new System.Drawing.Point(0, 168);
		this.teradataBottomEmptySpaceItem.Name = "teradataBottomEmptySpaceItem";
		this.teradataBottomEmptySpaceItem.Size = new System.Drawing.Size(508, 138);
		this.teradataBottomEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.teradataPortLayoutControlItem.Control = this.teradataPortTextEdit;
		this.teradataPortLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.teradataPortLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.teradataPortLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.teradataPortLayoutControlItem.Name = "teradataPortLayoutControlItem";
		this.teradataPortLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.teradataPortLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.teradataPortLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.teradataPortLayoutControlItem.Text = "Port:";
		this.teradataPortLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.teradataPortLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.teradataPortLayoutControlItem.TextToControlDistance = 5;
		this.teradataEmptySpaceItem.AllowHotTrack = false;
		this.teradataEmptySpaceItem.Location = new System.Drawing.Point(335, 0);
		this.teradataEmptySpaceItem.Name = "teradataEmptySpaceItem";
		this.teradataEmptySpaceItem.Size = new System.Drawing.Size(173, 24);
		this.teradataEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.teradataDefaultPortEmptySpaceItem.AllowHotTrack = false;
		this.teradataDefaultPortEmptySpaceItem.Location = new System.Drawing.Point(381, 24);
		this.teradataDefaultPortEmptySpaceItem.Name = "teradataDefaultPortEmptySpaceItem";
		this.teradataDefaultPortEmptySpaceItem.Size = new System.Drawing.Size(127, 24);
		this.teradataDefaultPortEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.teradataLoginEmptySpaceItem.AllowHotTrack = false;
		this.teradataLoginEmptySpaceItem.Location = new System.Drawing.Point(335, 48);
		this.teradataLoginEmptySpaceItem.Name = "teradataLoginEmptySpaceItem";
		this.teradataLoginEmptySpaceItem.Size = new System.Drawing.Size(173, 24);
		this.teradataLoginEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.teradataPortEmptySpaceItem.AllowHotTrack = false;
		this.teradataPortEmptySpaceItem.Location = new System.Drawing.Point(335, 24);
		this.teradataPortEmptySpaceItem.MaxSize = new System.Drawing.Size(10, 24);
		this.teradataPortEmptySpaceItem.MinSize = new System.Drawing.Size(10, 24);
		this.teradataPortEmptySpaceItem.Name = "teradataPortEmptySpaceItem";
		this.teradataPortEmptySpaceItem.Size = new System.Drawing.Size(10, 24);
		this.teradataPortEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.teradataPortEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.taradataDefaultPortLayoutControlItem.Control = this.teradataSqlDefaultPortLabelControl;
		this.taradataDefaultPortLayoutControlItem.Location = new System.Drawing.Point(345, 24);
		this.taradataDefaultPortLayoutControlItem.Name = "taradataDefaultPortLayoutControlItem";
		this.taradataDefaultPortLayoutControlItem.Size = new System.Drawing.Size(36, 24);
		this.taradataDefaultPortLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.taradataDefaultPortLayoutControlItem.TextVisible = false;
		this.teradataPasswordEmptySpaceItem.AllowHotTrack = false;
		this.teradataPasswordEmptySpaceItem.Location = new System.Drawing.Point(335, 72);
		this.teradataPasswordEmptySpaceItem.Name = "teradataPasswordEmptySpaceItem";
		this.teradataPasswordEmptySpaceItem.Size = new System.Drawing.Size(173, 24);
		this.teradataPasswordEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.teradataSavePasswordEmptySpaceItem.AllowHotTrack = false;
		this.teradataSavePasswordEmptySpaceItem.Location = new System.Drawing.Point(335, 96);
		this.teradataSavePasswordEmptySpaceItem.Name = "teradataSavePasswordEmptySpaceItem";
		this.teradataSavePasswordEmptySpaceItem.Size = new System.Drawing.Size(173, 24);
		this.teradataSavePasswordEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.teradataSavePasswordLayoutControlItem.Control = this.teradataSavePasswordCheckEdit;
		this.teradataSavePasswordLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.teradataSavePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem2";
		this.teradataSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.teradataSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.teradataSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.teradataSavePasswordLayoutControlItem.Name = "teradataSavePasswordLayoutControlItem";
		this.teradataSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.teradataSavePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.teradataSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.teradataSavePasswordLayoutControlItem.Text = " ";
		this.teradataSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.teradataSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.teradataSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.teradataDatabaseLayoutControl.Control = this.teradataDatabaseButtonEdit;
		this.teradataDatabaseLayoutControl.Location = new System.Drawing.Point(0, 120);
		this.teradataDatabaseLayoutControl.MaxSize = new System.Drawing.Size(335, 24);
		this.teradataDatabaseLayoutControl.MinSize = new System.Drawing.Size(335, 24);
		this.teradataDatabaseLayoutControl.Name = "teradataDatabaseLayoutControl";
		this.teradataDatabaseLayoutControl.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.teradataDatabaseLayoutControl.Size = new System.Drawing.Size(335, 24);
		this.teradataDatabaseLayoutControl.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.teradataDatabaseLayoutControl.Text = "Database:";
		this.teradataDatabaseLayoutControl.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.teradataDatabaseLayoutControl.TextSize = new System.Drawing.Size(100, 13);
		this.teradataDatabaseLayoutControl.TextToControlDistance = 5;
		this.teradataTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.teradataTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 144);
		this.teradataTimeoutEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 24);
		this.teradataTimeoutEmptySpaceItem.MinSize = new System.Drawing.Size(104, 24);
		this.teradataTimeoutEmptySpaceItem.Name = "teradataTimeoutEmptySpaceItem";
		this.teradataTimeoutEmptySpaceItem.Size = new System.Drawing.Size(508, 24);
		this.teradataTimeoutEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.teradataTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.teradataDatabaseEmptySpaceItem1.AllowHotTrack = false;
		this.teradataDatabaseEmptySpaceItem1.Location = new System.Drawing.Point(335, 120);
		this.teradataDatabaseEmptySpaceItem1.MaxSize = new System.Drawing.Size(10, 24);
		this.teradataDatabaseEmptySpaceItem1.MinSize = new System.Drawing.Size(10, 24);
		this.teradataDatabaseEmptySpaceItem1.Name = "teradataDatabaseEmptySpaceItem1";
		this.teradataDatabaseEmptySpaceItem1.Size = new System.Drawing.Size(10, 24);
		this.teradataDatabaseEmptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.teradataDatabaseEmptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.teradataDatabaseEmptySpaceItem2.AllowHotTrack = false;
		this.teradataDatabaseEmptySpaceItem2.Location = new System.Drawing.Point(421, 120);
		this.teradataDatabaseEmptySpaceItem2.Name = "teradataDatabaseEmptySpaceItem2";
		this.teradataDatabaseEmptySpaceItem2.Size = new System.Drawing.Size(87, 24);
		this.teradataDatabaseEmptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.teradataDatabasesCountLabelControlLayoutControlItem.Control = this.teradataDatabasesCountLabelControl;
		this.teradataDatabasesCountLabelControlLayoutControlItem.Location = new System.Drawing.Point(345, 120);
		this.teradataDatabasesCountLabelControlLayoutControlItem.Name = "teradataDatabasesCountLabelControlLayoutControlItem";
		this.teradataDatabasesCountLabelControlLayoutControlItem.Size = new System.Drawing.Size(76, 24);
		this.teradataDatabasesCountLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.teradataDatabasesCountLabelControlLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.teradataLayoutControl);
		base.Name = "TeradataConnectorControl";
		base.Size = new System.Drawing.Size(508, 306);
		((System.ComponentModel.ISupportInitialize)this.teradataLayoutControl).EndInit();
		this.teradataLayoutControl.ResumeLayout(false);
		this.teradataLayoutControl.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.teradataDatabaseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataPortTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataPasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataLoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataRootLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataLoginLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataPasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataBottomEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataPortLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataDefaultPortEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataLoginEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataPortEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.taradataDefaultPortLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataPasswordEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataSavePasswordEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataDatabaseLayoutControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataTimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataDatabaseEmptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataDatabaseEmptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.teradataDatabasesCountLabelControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
