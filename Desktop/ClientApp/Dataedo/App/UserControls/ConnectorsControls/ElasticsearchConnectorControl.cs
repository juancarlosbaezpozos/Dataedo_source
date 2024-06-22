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
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class ElasticsearchConnectorControl : ConnectorControlBase
{
	private IContainer components;

	private NonCustomizableLayoutControl elasticsearchLayoutControl;

	private TextEdit elasticsearchPortTextEdit;

	private TextEdit elasticsearchHostTextEdit;

	private TextEdit elasticsearchUserTextEdit;

	private TextEdit elasticsearchPasswordTextEdit;

	private CheckEdit elasticsearchSavePasswordCheckEdit;

	private LayoutControlGroup elasticsearchRoot;

	private LayoutControlItem elasticsearchHostLayoutControlItem;

	private EmptySpaceItem elasticsearchEmptySpaceItem;

	private LayoutControlItem elasticsearchPortLayoutControlItem;

	private EmptySpaceItem elasticsearchTimeoutEmptySpaceItem;

	private EmptySpaceItem elasticSearchPortEmptySpaceItem;

	private EmptySpaceItem elasticsearchEHostmptySpaceItem;

	private LayoutControlItem elasticsearchUserLayoutControlItem;

	private LayoutControlItem elasticsearchPasswordLayoutControlItem;

	private EmptySpaceItem elasticSearchUserEmptySpaceItem;

	private EmptySpaceItem elasticsearchPasswordEmptySpaceItem;

	private LayoutControlItem elasticSearchCheckPasswordLayoutControlItem;

	private string providedElasticsearchHost => splittedHost?.Host ?? elasticsearchHostTextEdit.Text;

	private string providedElasticsearchPort => elasticsearchPortTextEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Elasticsearch;

	protected override TextEdit HostTextEdit => elasticsearchHostTextEdit;

	protected override TextEdit PortTextEdit => elasticsearchPortTextEdit;

	protected override CheckEdit SavePasswordCheckEdit => elasticsearchSavePasswordCheckEdit;

	public ElasticsearchConnectorControl()
	{
		InitializeComponent();
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? providedElasticsearchHost : base.DatabaseRow?.Name, documentationTitle, providedElasticsearchHost, elasticsearchUserTextEdit.Text, elasticsearchPasswordTextEdit.Text, providedElasticsearchPort, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion);
	}

	public override void SetTimeoutControlPosition()
	{
		if (timeoutLayoutControlItem == null)
		{
			SetTimeoutSpinEdit();
		}
		timeoutLayoutControlItem.Visibility = LayoutVisibility.Always;
		elasticsearchLayoutControl.Root.Remove(timeoutLayoutControlItem);
		if (base.SelectedDatabaseType.HasValue)
		{
			elasticsearchLayoutControl.Root.AddItem(timeoutLayoutControlItem, elasticsearchTimeoutEmptySpaceItem, InsertType.Top);
		}
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		return true & ValidateElasticsearchHost() & ValidateElasticsearchPort();
	}

	private bool ValidateElasticsearchHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(elasticsearchHostTextEdit, addDBErrorProvider, "host", acceptEmptyValue);
	}

	private bool ValidateElasticsearchPort(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(elasticsearchPortTextEdit, addDBErrorProvider, "port", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		elasticsearchHostTextEdit.Text = base.DatabaseRow.Host;
		elasticsearchPortTextEdit.Text = PrepareValue.ToString(base.DatabaseRow.Port) ?? DatabaseTypeEnum.GetDefaultPort(base.DatabaseRow.Type);
		elasticsearchPasswordTextEdit.Text = base.DatabaseRow.Password;
		elasticsearchUserTextEdit.Text = base.DatabaseRow.User;
		elasticsearchSavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return elasticsearchHostTextEdit.Text ?? "";
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
		this.elasticsearchLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.elasticsearchPortTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.elasticsearchHostTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.elasticsearchUserTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.elasticsearchPasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.elasticsearchSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.elasticsearchRoot = new DevExpress.XtraLayout.LayoutControlGroup();
		this.elasticsearchHostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.elasticsearchEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.elasticsearchPortLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.elasticsearchTimeoutEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.elasticSearchPortEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.elasticsearchEHostmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.elasticsearchUserLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.elasticsearchPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.elasticSearchUserEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.elasticsearchPasswordEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.elasticSearchCheckPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchLayoutControl).BeginInit();
		this.elasticsearchLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchPortTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchHostTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchUserTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchPasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchRoot).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchHostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchPortLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchTimeoutEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticSearchPortEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchEHostmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchUserLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchPasswordLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticSearchUserEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchPasswordEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.elasticSearchCheckPasswordLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.elasticsearchLayoutControl.AllowCustomization = false;
		this.elasticsearchLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.elasticsearchLayoutControl.Controls.Add(this.elasticsearchPortTextEdit);
		this.elasticsearchLayoutControl.Controls.Add(this.elasticsearchHostTextEdit);
		this.elasticsearchLayoutControl.Controls.Add(this.elasticsearchUserTextEdit);
		this.elasticsearchLayoutControl.Controls.Add(this.elasticsearchPasswordTextEdit);
		this.elasticsearchLayoutControl.Controls.Add(this.elasticsearchSavePasswordCheckEdit);
		this.elasticsearchLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.elasticsearchLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.elasticsearchLayoutControl.Name = "elasticsearchLayoutControl";
		this.elasticsearchLayoutControl.Root = this.elasticsearchRoot;
		this.elasticsearchLayoutControl.Size = new System.Drawing.Size(438, 377);
		this.elasticsearchLayoutControl.TabIndex = 1;
		this.elasticsearchLayoutControl.Text = "layoutControl3";
		this.elasticsearchPortTextEdit.EditValue = "9200";
		this.elasticsearchPortTextEdit.Location = new System.Drawing.Point(104, 26);
		this.elasticsearchPortTextEdit.MinimumSize = new System.Drawing.Size(229, 20);
		this.elasticsearchPortTextEdit.Name = "elasticsearchPortTextEdit";
		this.elasticsearchPortTextEdit.Size = new System.Drawing.Size(231, 20);
		this.elasticsearchPortTextEdit.StyleController = this.elasticsearchLayoutControl;
		this.elasticsearchPortTextEdit.TabIndex = 5;
		this.elasticsearchHostTextEdit.EditValue = "http://";
		this.elasticsearchHostTextEdit.Location = new System.Drawing.Point(104, 2);
		this.elasticsearchHostTextEdit.MinimumSize = new System.Drawing.Size(229, 20);
		this.elasticsearchHostTextEdit.Name = "elasticsearchHostTextEdit";
		this.elasticsearchHostTextEdit.Size = new System.Drawing.Size(231, 20);
		this.elasticsearchHostTextEdit.StyleController = this.elasticsearchLayoutControl;
		this.elasticsearchHostTextEdit.TabIndex = 4;
		this.elasticsearchUserTextEdit.EditValue = "";
		this.elasticsearchUserTextEdit.Location = new System.Drawing.Point(104, 50);
		this.elasticsearchUserTextEdit.MinimumSize = new System.Drawing.Size(229, 20);
		this.elasticsearchUserTextEdit.Name = "elasticsearchUserTextEdit";
		this.elasticsearchUserTextEdit.Size = new System.Drawing.Size(231, 20);
		this.elasticsearchUserTextEdit.StyleController = this.elasticsearchLayoutControl;
		this.elasticsearchUserTextEdit.TabIndex = 5;
		this.elasticsearchPasswordTextEdit.EditValue = "";
		this.elasticsearchPasswordTextEdit.Location = new System.Drawing.Point(104, 74);
		this.elasticsearchPasswordTextEdit.MinimumSize = new System.Drawing.Size(229, 20);
		this.elasticsearchPasswordTextEdit.Name = "elasticsearchPasswordTextEdit";
		this.elasticsearchPasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.elasticsearchPasswordTextEdit.Size = new System.Drawing.Size(231, 20);
		this.elasticsearchPasswordTextEdit.StyleController = this.elasticsearchLayoutControl;
		this.elasticsearchPasswordTextEdit.TabIndex = 5;
		this.elasticsearchSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 96);
		this.elasticsearchSavePasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.elasticsearchSavePasswordCheckEdit.Name = "elasticsearchSavePasswordCheckEdit";
		this.elasticsearchSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.elasticsearchSavePasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.elasticsearchSavePasswordCheckEdit.StyleController = this.elasticsearchLayoutControl;
		this.elasticsearchSavePasswordCheckEdit.TabIndex = 3;
		this.elasticsearchRoot.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.elasticsearchRoot.GroupBordersVisible = false;
		this.elasticsearchRoot.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[11]
		{
			this.elasticsearchHostLayoutControlItem, this.elasticsearchPortLayoutControlItem, this.elasticsearchTimeoutEmptySpaceItem, this.elasticSearchPortEmptySpaceItem, this.elasticsearchEHostmptySpaceItem, this.elasticsearchUserLayoutControlItem, this.elasticsearchPasswordLayoutControlItem, this.elasticSearchUserEmptySpaceItem, this.elasticsearchPasswordEmptySpaceItem, this.elasticSearchCheckPasswordLayoutControlItem,
			this.elasticsearchEmptySpaceItem
		});
		this.elasticsearchRoot.Name = "Root";
		this.elasticsearchRoot.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.elasticsearchRoot.Size = new System.Drawing.Size(438, 377);
		this.elasticsearchRoot.TextVisible = false;
		this.elasticsearchHostLayoutControlItem.Control = this.elasticsearchHostTextEdit;
		this.elasticsearchHostLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.elasticsearchHostLayoutControlItem.MaxSize = new System.Drawing.Size(336, 24);
		this.elasticsearchHostLayoutControlItem.MinSize = new System.Drawing.Size(336, 24);
		this.elasticsearchHostLayoutControlItem.Name = "elasticsearchHostLayoutControlItem";
		this.elasticsearchHostLayoutControlItem.Size = new System.Drawing.Size(336, 24);
		this.elasticsearchHostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.elasticsearchHostLayoutControlItem.Text = "Host:";
		this.elasticsearchHostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.elasticsearchHostLayoutControlItem.TextSize = new System.Drawing.Size(98, 13);
		this.elasticsearchHostLayoutControlItem.TextToControlDistance = 5;
		this.elasticsearchEmptySpaceItem.AllowHotTrack = false;
		this.elasticsearchEmptySpaceItem.Location = new System.Drawing.Point(0, 144);
		this.elasticsearchEmptySpaceItem.Name = "elasticsearchEmptySpaceItem";
		this.elasticsearchEmptySpaceItem.Size = new System.Drawing.Size(438, 233);
		this.elasticsearchEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.elasticsearchPortLayoutControlItem.Control = this.elasticsearchPortTextEdit;
		this.elasticsearchPortLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.elasticsearchPortLayoutControlItem.MaxSize = new System.Drawing.Size(336, 24);
		this.elasticsearchPortLayoutControlItem.MinSize = new System.Drawing.Size(336, 24);
		this.elasticsearchPortLayoutControlItem.Name = "elasticsearchPortLayoutControlItem";
		this.elasticsearchPortLayoutControlItem.Size = new System.Drawing.Size(336, 24);
		this.elasticsearchPortLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.elasticsearchPortLayoutControlItem.Text = "Port:";
		this.elasticsearchPortLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.elasticsearchPortLayoutControlItem.TextSize = new System.Drawing.Size(98, 13);
		this.elasticsearchPortLayoutControlItem.TextToControlDistance = 5;
		this.elasticsearchTimeoutEmptySpaceItem.AllowHotTrack = false;
		this.elasticsearchTimeoutEmptySpaceItem.Location = new System.Drawing.Point(0, 120);
		this.elasticsearchTimeoutEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 24);
		this.elasticsearchTimeoutEmptySpaceItem.MinSize = new System.Drawing.Size(104, 24);
		this.elasticsearchTimeoutEmptySpaceItem.Name = "elasticsearchTimeoutEmptySpaceItem";
		this.elasticsearchTimeoutEmptySpaceItem.Size = new System.Drawing.Size(438, 24);
		this.elasticsearchTimeoutEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.elasticsearchTimeoutEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.elasticSearchPortEmptySpaceItem.AllowHotTrack = false;
		this.elasticSearchPortEmptySpaceItem.Location = new System.Drawing.Point(336, 24);
		this.elasticSearchPortEmptySpaceItem.Name = "elasticSearchPortEmptySpaceItem";
		this.elasticSearchPortEmptySpaceItem.Size = new System.Drawing.Size(102, 24);
		this.elasticSearchPortEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.elasticsearchEHostmptySpaceItem.AllowHotTrack = false;
		this.elasticsearchEHostmptySpaceItem.Location = new System.Drawing.Point(336, 0);
		this.elasticsearchEHostmptySpaceItem.Name = "elasticsearchEHostmptySpaceItem";
		this.elasticsearchEHostmptySpaceItem.Size = new System.Drawing.Size(102, 24);
		this.elasticsearchEHostmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.elasticsearchUserLayoutControlItem.Control = this.elasticsearchUserTextEdit;
		this.elasticsearchUserLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.elasticsearchUserLayoutControlItem.CustomizationFormText = "Port:";
		this.elasticsearchUserLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.elasticsearchUserLayoutControlItem.MaxSize = new System.Drawing.Size(336, 24);
		this.elasticsearchUserLayoutControlItem.MinSize = new System.Drawing.Size(336, 24);
		this.elasticsearchUserLayoutControlItem.Name = "elasticsearchUserLayoutControlItem";
		this.elasticsearchUserLayoutControlItem.Size = new System.Drawing.Size(336, 24);
		this.elasticsearchUserLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.elasticsearchUserLayoutControlItem.Text = "User:";
		this.elasticsearchUserLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.elasticsearchUserLayoutControlItem.TextSize = new System.Drawing.Size(98, 13);
		this.elasticsearchUserLayoutControlItem.TextToControlDistance = 5;
		this.elasticsearchPasswordLayoutControlItem.Control = this.elasticsearchPasswordTextEdit;
		this.elasticsearchPasswordLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.elasticsearchPasswordLayoutControlItem.CustomizationFormText = "Port:";
		this.elasticsearchPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.elasticsearchPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(336, 24);
		this.elasticsearchPasswordLayoutControlItem.MinSize = new System.Drawing.Size(336, 24);
		this.elasticsearchPasswordLayoutControlItem.Name = "elasticsearchPasswordLayoutControlItem";
		this.elasticsearchPasswordLayoutControlItem.Size = new System.Drawing.Size(336, 24);
		this.elasticsearchPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.elasticsearchPasswordLayoutControlItem.Text = "Password:";
		this.elasticsearchPasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.elasticsearchPasswordLayoutControlItem.TextSize = new System.Drawing.Size(98, 13);
		this.elasticsearchPasswordLayoutControlItem.TextToControlDistance = 5;
		this.elasticSearchUserEmptySpaceItem.AllowHotTrack = false;
		this.elasticSearchUserEmptySpaceItem.Location = new System.Drawing.Point(336, 48);
		this.elasticSearchUserEmptySpaceItem.Name = "elasticSearchUserEmptySpaceItem";
		this.elasticSearchUserEmptySpaceItem.Size = new System.Drawing.Size(102, 24);
		this.elasticSearchUserEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.elasticsearchPasswordEmptySpaceItem.AllowHotTrack = false;
		this.elasticsearchPasswordEmptySpaceItem.Location = new System.Drawing.Point(336, 72);
		this.elasticsearchPasswordEmptySpaceItem.Name = "elasticsearchPasswordEmptySpaceItem";
		this.elasticsearchPasswordEmptySpaceItem.Size = new System.Drawing.Size(102, 24);
		this.elasticsearchPasswordEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.elasticSearchCheckPasswordLayoutControlItem.Control = this.elasticsearchSavePasswordCheckEdit;
		this.elasticSearchCheckPasswordLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.elasticSearchCheckPasswordLayoutControlItem.CustomizationFormText = "layoutControlItem18";
		this.elasticSearchCheckPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.elasticSearchCheckPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.elasticSearchCheckPasswordLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.elasticSearchCheckPasswordLayoutControlItem.Name = "elasticSearchCheckPasswordLayoutControlItem";
		this.elasticSearchCheckPasswordLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.elasticSearchCheckPasswordLayoutControlItem.Size = new System.Drawing.Size(438, 24);
		this.elasticSearchCheckPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.elasticSearchCheckPasswordLayoutControlItem.Text = " ";
		this.elasticSearchCheckPasswordLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.elasticSearchCheckPasswordLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.elasticSearchCheckPasswordLayoutControlItem.TextToControlDistance = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.elasticsearchLayoutControl);
		base.Name = "ElasticsearchConnectorControl";
		base.Size = new System.Drawing.Size(438, 377);
		((System.ComponentModel.ISupportInitialize)this.elasticsearchLayoutControl).EndInit();
		this.elasticsearchLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.elasticsearchPortTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchHostTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchUserTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchPasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchRoot).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchHostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchPortLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchTimeoutEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticSearchPortEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchEHostmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchUserLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchPasswordLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticSearchUserEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticsearchPasswordEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.elasticSearchCheckPasswordLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
