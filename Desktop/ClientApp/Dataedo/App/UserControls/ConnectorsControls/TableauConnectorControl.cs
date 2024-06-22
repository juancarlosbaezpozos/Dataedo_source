using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class TableauConnectorControl : ConnectorControlBase
{
	protected string lastProvidedToken = string.Empty;

	private IContainer components;

	private NonCustomizableLayoutControl tableauLayoutControl;

	private ComboBoxEdit tableauHostComboBoxEdit;

	private TextEdit tableauLoginTextEdit;

	private TextEdit tableauPasswordTextEdit;

	private CheckEdit tableauSavePasswordCheckEdit;

	private LayoutControlGroup layoutControlGroup10;

	private LayoutControlItem tableauHostLayoutControlItem;

	private LayoutControlItem tableauUserLayoutControlItem;

	private LayoutControlItem tableauPasswordLayoutControlItem;

	private LayoutControlItem tableauSavePasswordLayoutControlItem;

	private EmptySpaceItem emptySpaceItem70;

	private EmptySpaceItem emptySpaceItem71;

	private EmptySpaceItem emptySpaceItem73;

	private EmptySpaceItem emptySpaceItem74;

	private EmptySpaceItem emptySpaceItem76;

	private EmptySpaceItem tableauTimeoutEmptySpaceItem;

	private ButtonEdit tableauSiteButtonEdit;

	private LayoutControlItem tableauSiteLayoutControlItem;

	private LookUpEdit tableauAuthenticationLookUpEdit;

	private LayoutControlItem tableauAuthenticationLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem2;

	private LookUpEdit tableauProductLookUpEdit;

	private LayoutControlItem tableauProductLayoutControlItem;

	private string providedTableauHost => splittedHost?.Host ?? tableauHostComboBoxEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Tableau;

	protected override TextEdit HostTextEdit => tableauHostComboBoxEdit;

	protected override ComboBoxEdit HostComboBoxEdit => tableauHostComboBoxEdit;

	protected override CheckEdit SavePasswordCheckEdit => tableauSavePasswordCheckEdit;

	public TableauConnectorControl()
	{
		InitializeComponent();
	}

	protected override void SetAuthenticationDataSource()
	{
		AuthenticationTableau.SetAuthenticationDataSource(tableauAuthenticationLookUpEdit);
		Dataedo.App.Classes.TableauProduct.SetTableauProductDataSource(tableauProductLookUpEdit);
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? tableauSiteButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedTableauHost, tableauLoginTextEdit.Text, tableauPasswordTextEdit.Text, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion, AuthenticationType.GetTypeByIndex(AuthenticationTableau.GetIndex(tableauAuthenticationLookUpEdit)), Dataedo.Shared.Enums.TableauProduct.GetTypeByIndex(Dataedo.App.Classes.TableauProduct.GetIndex(tableauProductLookUpEdit)));
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		tableauLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			tableauLayoutControl.Root.AddItem(timeoutLayoutControlItem, tableauTimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		bool flag = true;
		flag &= ValidateTableauHost();
		flag &= ValidateTableauProduct();
		flag &= ValidateTableauAuthenticationType();
		flag &= ValidateTableauLogin();
		flag &= ValidateTableauPassword();
		if (0.Equals(tableauProductLookUpEdit.EditValue))
		{
			flag &= ValidateTableauSite();
		}
		return flag;
	}

	private bool ValidateTableauHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(tableauHostComboBoxEdit, addDBErrorProvider, "host", acceptEmptyValue);
	}

	private bool ValidateTableauAuthenticationType(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(tableauAuthenticationLookUpEdit, addDBErrorProvider, "authentication type", acceptEmptyValue);
	}

	private bool ValidateTableauProduct(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(tableauProductLookUpEdit, addDBErrorProvider, "cproduct", acceptEmptyValue);
	}

	private bool ValidateTableauPassword(bool acceptEmptyValue = true)
	{
		if (AuthenticationTableau.IsTokenAuthentication(tableauAuthenticationLookUpEdit))
		{
			return ValidateFields.ValidateEdit(tableauPasswordTextEdit, addDBErrorProvider, "token secret", acceptEmptyValue);
		}
		return ValidateFields.ValidateEdit(tableauPasswordTextEdit, addDBErrorProvider, "password", acceptEmptyValue);
	}

	private bool ValidateTableauLogin(bool acceptEmptyValue = false)
	{
		if (AuthenticationTableau.IsTokenAuthentication(tableauAuthenticationLookUpEdit))
		{
			return ValidateFields.ValidateEdit(tableauLoginTextEdit, addDBErrorProvider, "token name", acceptEmptyValue);
		}
		return ValidateFields.ValidateEdit(tableauLoginTextEdit, addDBErrorProvider, "username", acceptEmptyValue);
	}

	private bool ValidateTableauSite(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(tableauSiteButtonEdit, addDBErrorProvider, "site", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		tableauHostComboBoxEdit.Text = base.DatabaseRow.Host;
		tableauSiteButtonEdit.EditValue = base.DatabaseRow.Name;
		tableauProductLookUpEdit.EditValue = Dataedo.Shared.Enums.TableauProduct.GetLookupIndex(Dataedo.Shared.Enums.TableauProduct.StringToType(base.DatabaseRow.Param1));
		tableauAuthenticationLookUpEdit.EditValue = AuthenticationType.GetLookupIndex(base.DatabaseRow.SelectedAuthenticationType);
		if (base.DatabaseRow.SelectedAuthenticationType == AuthenticationType.AuthenticationTypeEnum.Token)
		{
			tableauLoginTextEdit.Text = base.DatabaseRow.ServiceName;
		}
		else
		{
			tableauLoginTextEdit.Text = base.DatabaseRow.User;
		}
		tableauPasswordTextEdit.Text = base.DatabaseRow.Password;
		tableauSavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return tableauSiteButtonEdit.Text + "@" + providedTableauHost;
	}

	private void tableauHostComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		HostComboBoxEdit_EditValueChanged(sender, e);
	}

	private void tableauHostComboBoxEdit_Leave(object sender, EventArgs e)
	{
		hostComboBoxEdit_Leave(sender, e);
	}

	protected override void ClearPanelLoginAndPassword()
	{
		tableauLoginTextEdit.Text = string.Empty;
		tableauPasswordTextEdit.Text = string.Empty;
	}

	private bool CheckUriScheme()
	{
		Uri.TryCreate(HostComboBoxEdit.Text, UriKind.RelativeOrAbsolute, out var result);
		try
		{
			if (!Uri.CheckSchemeName(result.Scheme))
			{
				SetUriError();
				return false;
			}
		}
		catch (Exception)
		{
			SetUriError();
			return false;
		}
		return true;
	}

	private void SetUriError()
	{
		string text = "Invalid URI: Missing protocol in the URI. Usually, URI starts with https://...";
		addDBErrorProvider.SetError(HostComboBoxEdit, text);
		GeneralMessageBoxesHandling.Show(text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
	}

	private void tableauSiteButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		CheckUriScheme();
		DatabaseButtonEdit_ButtonClick(sender, e);
	}

	private void tableauAuthenticationLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (AuthenticationTableau.IsTokenAuthentication(tableauAuthenticationLookUpEdit))
		{
			tableauUserLayoutControlItem.Text = "Token name:";
			tableauPasswordLayoutControlItem.Text = "Token secret:";
		}
		else
		{
			tableauUserLayoutControlItem.Text = "Username:";
			tableauPasswordLayoutControlItem.Text = "Password:";
		}
		ValidateTableauAuthenticationType(acceptEmptyValue: true);
	}

	private void tableauAuthenticationLookUpEdit_EditValueChanging(object sender, ChangingEventArgs e)
	{
		AuthenticationType.AuthenticationTypeEnum? authenticationTypeEnum = ((e.NewValue == null) ? null : new AuthenticationType.AuthenticationTypeEnum?(AuthenticationType.GetTypeByIndex((int)e.NewValue)));
		AuthenticationType.AuthenticationTypeEnum? authenticationTypeEnum2 = ((e.OldValue == null) ? null : new AuthenticationType.AuthenticationTypeEnum?(AuthenticationType.GetTypeByIndex((int)e.OldValue)));
		if (authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.Token && authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.StandardAuthentication)
		{
			lastProvidedToken = tableauLoginTextEdit.Text;
			tableauLoginTextEdit.Text = lastProvidedLogin;
		}
		else if (authenticationTypeEnum == AuthenticationType.AuthenticationTypeEnum.Token && authenticationTypeEnum2 == AuthenticationType.AuthenticationTypeEnum.StandardAuthentication)
		{
			lastProvidedLogin = tableauLoginTextEdit.Text;
			tableauLoginTextEdit.Text = lastProvidedToken;
		}
		tableauPasswordTextEdit.EditValue = string.Empty;
	}

	private void tableauProductLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		tableauSiteButtonEdit.Properties.Buttons[0].Enabled = Dataedo.App.Classes.TableauProduct.IsServerProduct(tableauProductLookUpEdit);
		tableauSiteButtonEdit.Properties.Buttons[0].ToolTip = (Dataedo.App.Classes.TableauProduct.IsServerProduct(tableauProductLookUpEdit) ? string.Empty : "List of sites is not available in Tableau Online");
		ValidateTableauAuthenticationType(acceptEmptyValue: true);
	}

	public override bool TestConnection(bool testForGettingDatabasesList)
	{
		if (CheckUriScheme())
		{
			return base.TestConnection(testForGettingDatabasesList);
		}
		return false;
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
		this.tableauLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.tableauProductLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.tableauSiteButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.tableauHostComboBoxEdit = new DevExpress.XtraEditors.ComboBoxEdit();
		this.tableauLoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.tableauPasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.tableauSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.tableauAuthenticationLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.layoutControlGroup10 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.tableauHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.tableauUserLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.tableauPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.tableauSavePasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem70 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem71 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem73 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem74 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem76 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.tableauTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.tableauSiteLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.tableauAuthenticationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.tableauProductLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.tableauLayoutControl).BeginInit();
		this.tableauLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tableauProductLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableauSiteButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableauHostComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableauLoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableauPasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableauSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableauAuthenticationLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableauHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableauUserLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableauPasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableauSavePasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem70).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem71).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem73).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem74).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem76).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableauTimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableauSiteLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableauAuthenticationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableauProductLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		base.SuspendLayout();
		this.tableauLayoutControl.AllowCustomization = false;
		this.tableauLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.tableauLayoutControl.Controls.Add(this.tableauProductLookUpEdit);
		this.tableauLayoutControl.Controls.Add(this.tableauSiteButtonEdit);
		this.tableauLayoutControl.Controls.Add(this.tableauHostComboBoxEdit);
		this.tableauLayoutControl.Controls.Add(this.tableauLoginTextEdit);
		this.tableauLayoutControl.Controls.Add(this.tableauPasswordTextEdit);
		this.tableauLayoutControl.Controls.Add(this.tableauSavePasswordCheckEdit);
		this.tableauLayoutControl.Controls.Add(this.tableauAuthenticationLookUpEdit);
		this.tableauLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableauLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.tableauLayoutControl.Name = "tableauLayoutControl";
		this.tableauLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(872, 130, 832, 925);
		this.tableauLayoutControl.Root = this.layoutControlGroup10;
		this.tableauLayoutControl.Size = new System.Drawing.Size(471, 407);
		this.tableauLayoutControl.TabIndex = 2;
		this.tableauLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.tableauProductLookUpEdit.Location = new System.Drawing.Point(105, 0);
		this.tableauProductLookUpEdit.Name = "tableauProductLookUpEdit";
		this.tableauProductLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.tableauProductLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("type", "Type")
		});
		this.tableauProductLookUpEdit.Properties.DisplayMember = "type";
		this.tableauProductLookUpEdit.Properties.NullText = "";
		this.tableauProductLookUpEdit.Properties.ShowFooter = false;
		this.tableauProductLookUpEdit.Properties.ShowHeader = false;
		this.tableauProductLookUpEdit.Properties.ShowLines = false;
		this.tableauProductLookUpEdit.Properties.ValueMember = "id";
		this.tableauProductLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.tableauProductLookUpEdit.StyleController = this.tableauLayoutControl;
		this.tableauProductLookUpEdit.TabIndex = 10;
		this.tableauProductLookUpEdit.EditValueChanged += new System.EventHandler(tableauProductLookUpEdit_EditValueChanged);
		this.tableauSiteButtonEdit.Location = new System.Drawing.Point(105, 144);
		this.tableauSiteButtonEdit.Name = "tableauSiteButtonEdit";
		this.tableauSiteButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.tableauSiteButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.tableauSiteButtonEdit.StyleController = this.tableauLayoutControl;
		this.tableauSiteButtonEdit.TabIndex = 9;
		this.tableauSiteButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(tableauSiteButtonEdit_ButtonClick);
		this.tableauHostComboBoxEdit.Location = new System.Drawing.Point(105, 24);
		this.tableauHostComboBoxEdit.Margin = new System.Windows.Forms.Padding(2);
		this.tableauHostComboBoxEdit.Name = "tableauHostComboBoxEdit";
		this.tableauHostComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.tableauHostComboBoxEdit.Size = new System.Drawing.Size(230, 20);
		this.tableauHostComboBoxEdit.StyleController = this.tableauLayoutControl;
		this.tableauHostComboBoxEdit.TabIndex = 4;
		this.tableauHostComboBoxEdit.EditValueChanged += new System.EventHandler(tableauHostComboBoxEdit_EditValueChanged);
		this.tableauHostComboBoxEdit.Leave += new System.EventHandler(tableauHostComboBoxEdit_Leave);
		this.tableauLoginTextEdit.Location = new System.Drawing.Point(105, 72);
		this.tableauLoginTextEdit.Margin = new System.Windows.Forms.Padding(2);
		this.tableauLoginTextEdit.Name = "tableauLoginTextEdit";
		this.tableauLoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.tableauLoginTextEdit.StyleController = this.tableauLayoutControl;
		this.tableauLoginTextEdit.TabIndex = 7;
		this.tableauPasswordTextEdit.Location = new System.Drawing.Point(105, 96);
		this.tableauPasswordTextEdit.Margin = new System.Windows.Forms.Padding(2);
		this.tableauPasswordTextEdit.Name = "tableauPasswordTextEdit";
		this.tableauPasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.tableauPasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.tableauPasswordTextEdit.StyleController = this.tableauLayoutControl;
		this.tableauPasswordTextEdit.TabIndex = 8;
		this.tableauSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 120);
		this.tableauSavePasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.tableauSavePasswordCheckEdit.Name = "tableauSavePasswordCheckEdit";
		this.tableauSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.tableauSavePasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.tableauSavePasswordCheckEdit.StyleController = this.tableauLayoutControl;
		this.tableauSavePasswordCheckEdit.TabIndex = 3;
		this.tableauAuthenticationLookUpEdit.EditValue = 2;
		this.tableauAuthenticationLookUpEdit.Location = new System.Drawing.Point(105, 48);
		this.tableauAuthenticationLookUpEdit.Name = "tableauAuthenticationLookUpEdit";
		this.tableauAuthenticationLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.tableauAuthenticationLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("type", "Type")
		});
		this.tableauAuthenticationLookUpEdit.Properties.DisplayMember = "type";
		this.tableauAuthenticationLookUpEdit.Properties.NullText = "";
		this.tableauAuthenticationLookUpEdit.Properties.ShowFooter = false;
		this.tableauAuthenticationLookUpEdit.Properties.ShowHeader = false;
		this.tableauAuthenticationLookUpEdit.Properties.ShowLines = false;
		this.tableauAuthenticationLookUpEdit.Properties.ValueMember = "id";
		this.tableauAuthenticationLookUpEdit.Size = new System.Drawing.Size(230, 20);
		this.tableauAuthenticationLookUpEdit.StyleController = this.tableauLayoutControl;
		this.tableauAuthenticationLookUpEdit.TabIndex = 0;
		this.tableauAuthenticationLookUpEdit.EditValueChanged += new System.EventHandler(tableauAuthenticationLookUpEdit_EditValueChanged);
		this.tableauAuthenticationLookUpEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(tableauAuthenticationLookUpEdit_EditValueChanging);
		this.layoutControlGroup10.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup10.GroupBordersVisible = false;
		this.layoutControlGroup10.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[15]
		{
			this.tableauHostLayoutControlItem, this.tableauUserLayoutControlItem, this.tableauPasswordLayoutControlItem, this.tableauSavePasswordLayoutControlItem, this.emptySpaceItem70, this.emptySpaceItem71, this.emptySpaceItem73, this.emptySpaceItem74, this.emptySpaceItem76, this.tableauTimeoutEmptySpaceItem,
			this.tableauSiteLayoutControlItem, this.tableauAuthenticationLayoutControlItem, this.emptySpaceItem1, this.tableauProductLayoutControlItem, this.emptySpaceItem2
		});
		this.layoutControlGroup10.Name = "Root";
		this.layoutControlGroup10.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup10.Size = new System.Drawing.Size(471, 407);
		this.layoutControlGroup10.TextVisible = false;
		this.tableauHostLayoutControlItem.Control = this.tableauHostComboBoxEdit;
		this.tableauHostLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.tableauHostLayoutControlItem.CustomizationFormText = "Host";
		this.tableauHostLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.tableauHostLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.tableauHostLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.tableauHostLayoutControlItem.Name = "tableauHostLayoutControlItem";
		this.tableauHostLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.tableauHostLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.tableauHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.tableauHostLayoutControlItem.Text = "Host:";
		this.tableauHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.tableauHostLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.tableauHostLayoutControlItem.TextToControlDistance = 5;
		this.tableauUserLayoutControlItem.Control = this.tableauLoginTextEdit;
		this.tableauUserLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.tableauUserLayoutControlItem.CustomizationFormText = "User";
		this.tableauUserLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.tableauUserLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.tableauUserLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.tableauUserLayoutControlItem.Name = "tableauUserLayoutControlItem";
		this.tableauUserLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.tableauUserLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.tableauUserLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.tableauUserLayoutControlItem.Text = "Username:";
		this.tableauUserLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.tableauUserLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.tableauUserLayoutControlItem.TextToControlDistance = 5;
		this.tableauPasswordLayoutControlItem.Control = this.tableauPasswordTextEdit;
		this.tableauPasswordLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.tableauPasswordLayoutControlItem.CustomizationFormText = "Password";
		this.tableauPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.tableauPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.tableauPasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.tableauPasswordLayoutControlItem.Name = "tableauPasswordLayoutControlItem";
		this.tableauPasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.tableauPasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.tableauPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.tableauPasswordLayoutControlItem.Text = "Password:";
		this.tableauPasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.tableauPasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.tableauPasswordLayoutControlItem.TextToControlDistance = 5;
		this.tableauSavePasswordLayoutControlItem.Control = this.tableauSavePasswordCheckEdit;
		this.tableauSavePasswordLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.tableauSavePasswordLayoutControlItem.CustomizationFormText = "layoutControlItem18";
		this.tableauSavePasswordLayoutControlItem.Location = new System.Drawing.Point(0, 120);
		this.tableauSavePasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.tableauSavePasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.tableauSavePasswordLayoutControlItem.Name = "tableauSavePasswordLayoutControlItem";
		this.tableauSavePasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.tableauSavePasswordLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.tableauSavePasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.tableauSavePasswordLayoutControlItem.Text = " ";
		this.tableauSavePasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.tableauSavePasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.tableauSavePasswordLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem70.AllowHotTrack = false;
		this.emptySpaceItem70.Location = new System.Drawing.Point(0, 192);
		this.emptySpaceItem70.Name = "emptySpaceItem70";
		this.emptySpaceItem70.Size = new System.Drawing.Size(471, 215);
		this.emptySpaceItem70.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem71.AllowHotTrack = false;
		this.emptySpaceItem71.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem71.Name = "emptySpaceItem71";
		this.emptySpaceItem71.Size = new System.Drawing.Size(136, 24);
		this.emptySpaceItem71.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem73.AllowHotTrack = false;
		this.emptySpaceItem73.Location = new System.Drawing.Point(335, 72);
		this.emptySpaceItem73.Name = "emptySpaceItem73";
		this.emptySpaceItem73.Size = new System.Drawing.Size(136, 24);
		this.emptySpaceItem73.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem74.AllowHotTrack = false;
		this.emptySpaceItem74.Location = new System.Drawing.Point(335, 96);
		this.emptySpaceItem74.Name = "emptySpaceItem74";
		this.emptySpaceItem74.Size = new System.Drawing.Size(136, 24);
		this.emptySpaceItem74.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem76.AllowHotTrack = false;
		this.emptySpaceItem76.Location = new System.Drawing.Point(335, 120);
		this.emptySpaceItem76.Name = "emptySpaceItem76";
		this.emptySpaceItem76.Size = new System.Drawing.Size(136, 24);
		this.emptySpaceItem76.TextSize = new System.Drawing.Size(0, 0);
		this.tableauTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.tableauTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 168);
		this.tableauTimeoutEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 24);
		this.tableauTimeoutEmptySpaceItem.MinSize = new System.Drawing.Size(104, 24);
		this.tableauTimeoutEmptySpaceItem.Name = "tableauTimeoutEmptySpaceItem";
		this.tableauTimeoutEmptySpaceItem.Size = new System.Drawing.Size(471, 24);
		this.tableauTimeoutEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.tableauTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.tableauSiteLayoutControlItem.Control = this.tableauSiteButtonEdit;
		this.tableauSiteLayoutControlItem.Location = new System.Drawing.Point(0, 144);
		this.tableauSiteLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.tableauSiteLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.tableauSiteLayoutControlItem.Name = "tableauSiteLayoutControlItem";
		this.tableauSiteLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.tableauSiteLayoutControlItem.Size = new System.Drawing.Size(471, 24);
		this.tableauSiteLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.tableauSiteLayoutControlItem.Text = "Site:";
		this.tableauSiteLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.tableauSiteLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.tableauSiteLayoutControlItem.TextToControlDistance = 5;
		this.tableauAuthenticationLayoutControlItem.Control = this.tableauAuthenticationLookUpEdit;
		this.tableauAuthenticationLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.tableauAuthenticationLayoutControlItem.CustomizationFormText = "Authentication:";
		this.tableauAuthenticationLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.tableauAuthenticationLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.tableauAuthenticationLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.tableauAuthenticationLayoutControlItem.Name = "tableauAuthenticationLayoutControlItem";
		this.tableauAuthenticationLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.tableauAuthenticationLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.tableauAuthenticationLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.tableauAuthenticationLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.tableauAuthenticationLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.tableauAuthenticationLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.tableauAuthenticationLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.tableauAuthenticationLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.tableauAuthenticationLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.tableauAuthenticationLayoutControlItem.Text = "Authentication:";
		this.tableauAuthenticationLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.tableauAuthenticationLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.tableauAuthenticationLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(335, 48);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(136, 24);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.tableauProductLayoutControlItem.Control = this.tableauProductLookUpEdit;
		this.tableauProductLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.tableauProductLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.tableauProductLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.tableauProductLayoutControlItem.Name = "tableauProductLayoutControlItem";
		this.tableauProductLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.tableauProductLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.tableauProductLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.tableauProductLayoutControlItem.Text = "Product:";
		this.tableauProductLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.tableauProductLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.tableauProductLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(335, 24);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(136, 24);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.tableauLayoutControl);
		base.Name = "TableauConnectorControl";
		base.Size = new System.Drawing.Size(471, 407);
		((System.ComponentModel.ISupportInitialize)this.tableauLayoutControl).EndInit();
		this.tableauLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tableauProductLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableauSiteButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableauHostComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableauLoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableauPasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableauSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableauAuthenticationLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableauHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableauUserLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableauPasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableauSavePasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem70).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem71).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem73).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem74).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem76).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableauTimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableauSiteLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableauAuthenticationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableauProductLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		base.ResumeLayout(false);
	}
}
