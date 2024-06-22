using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Tools.Pannels;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classes;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ConnectorsControls;

public class Neo4jConnectorControl : ConnectorControlBase
{
	private const string neo4jDefaultPort = "7687";

	private IContainer components;

	private NonCustomizableLayoutControl neo4JNonCustomizableLayoutControl;

	private LabelControl neo4jDefaultConnectionURLLabelControl;

	private CheckEdit neo4jSavePasswordCheckEdit;

	private TextEdit neo4jLoginTextEdit;

	private TextEdit neo4jPasswordTextEdit;

	private TextEdit neo4jHostTextEdit;

	private ButtonEdit neo4jDatabaseButtonEdit;

	private LayoutControlItem layoutControlItem24;

	private LayoutControlGroup layoutControlGroup11;

	private LayoutControlItem neo4jPasswordTypeLayoutControlItem;

	private LayoutControlItem neo4jUserTypeLayoutControlItem;

	private LayoutControlItem neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem neo4jUserTypeLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem neo4jPasswordTypeLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem neo4jEmptySpaceItem;

	private LayoutControlItem neo4jHostTypeLayoutControlItem;

	private EmptySpaceItem neo4jPortTypeLayoutControlItemEmptySpaceItem;

	private LayoutControlItem layoutControlItem28;

	private EmptySpaceItem emptySpaceItem75;

	private LayoutControlItem neo4jDatabaseTypeLayoutControlItemEmptySpaceItem;

	private EmptySpaceItem emptySpaceItem1;

	private string providedNeo4jHost => splittedHost?.Host ?? neo4jHostTextEdit.Text;

	protected override PanelTypeEnum.PanelType PanelType => PanelTypeEnum.PanelType.Neo4j;

	protected override TextEdit HostTextEdit => neo4jHostTextEdit;

	protected override CheckEdit SavePasswordCheckEdit => neo4jSavePasswordCheckEdit;

	public Neo4jConnectorControl()
	{
		InitializeComponent();
	}

	protected override void SetPanelNewDBRowValues(bool forGettingDatabasesList = false)
	{
		string documentationTitle = GetDocumentationTitle();
		base.DatabaseRow = new DatabaseRow(base.SelectedDatabaseType, (!forGettingDatabasesList) ? neo4jDatabaseButtonEdit.Text : base.DatabaseRow?.Name, documentationTitle, providedNeo4jHost, neo4jLoginTextEdit.Text, neo4jPasswordTextEdit.Text, null, base.DatabaseRow.Id, base.DatabaseRow.Filter.GetRulesXml(), base.DatabaseRow.DbmsVersion);
	}

	protected override bool ValidatePanelRequiredFields(bool testForGettingDatabasesList, bool testForGettingWarehousesList = false, bool testForGettingPerspectiveList = false)
	{
		return true & ValidateNeo4jHost();
	}

	private bool ValidateNeo4jHost(bool acceptEmptyValue = false)
	{
		return ValidateFields.ValidateEdit(neo4jHostTextEdit, addDBErrorProvider, "Host", acceptEmptyValue);
	}

	protected override void ReadPanelValues()
	{
		string value = PrepareValue.ToString(base.DatabaseRow.Password);
		neo4jHostTextEdit.Text = base.DatabaseRow.Host;
		neo4jDatabaseButtonEdit.EditValue = base.DatabaseRow.Name;
		neo4jLoginTextEdit.Text = base.DatabaseRow.User;
		neo4jPasswordTextEdit.Text = base.DatabaseRow.Password;
		neo4jSavePasswordCheckEdit.Checked = ((!string.IsNullOrEmpty(value)) ? true : false);
	}

	protected override string GetPanelDocumentationTitle()
	{
		return neo4jDatabaseButtonEdit.Text + "@" + neo4jHostTextEdit.Text;
	}

	private void neo4jDefaultConnectionURLLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		neo4jHostTextEdit.Text = "bolt://localhost:7687";
	}

	private void neo4jDatabaseButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		DatabaseButtonEdit_ButtonClick(sender, e);
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
		this.neo4JNonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.neo4jDefaultConnectionURLLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.neo4jSavePasswordCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.neo4jLoginTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.neo4jPasswordTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.neo4jHostTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.neo4jDatabaseButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.layoutControlItem24 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlGroup11 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.neo4jPasswordTypeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.neo4jUserTypeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.neo4jUserTypeLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.neo4jPasswordTypeLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.neo4jEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.neo4jHostTypeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.neo4jPortTypeLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem28 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem75 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.neo4JNonCustomizableLayoutControl).BeginInit();
		this.neo4JNonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.neo4jSavePasswordCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jLoginTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jPasswordTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jHostTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jDatabaseButtonEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jPasswordTypeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jUserTypeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jUserTypeLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jPasswordTypeLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jHostTypeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jPortTypeLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem28).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem75).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		base.SuspendLayout();
		this.neo4JNonCustomizableLayoutControl.AllowCustomization = false;
		this.neo4JNonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.neo4JNonCustomizableLayoutControl.Controls.Add(this.neo4jDefaultConnectionURLLabelControl);
		this.neo4JNonCustomizableLayoutControl.Controls.Add(this.neo4jSavePasswordCheckEdit);
		this.neo4JNonCustomizableLayoutControl.Controls.Add(this.neo4jLoginTextEdit);
		this.neo4JNonCustomizableLayoutControl.Controls.Add(this.neo4jPasswordTextEdit);
		this.neo4JNonCustomizableLayoutControl.Controls.Add(this.neo4jHostTextEdit);
		this.neo4JNonCustomizableLayoutControl.Controls.Add(this.neo4jDatabaseButtonEdit);
		this.neo4JNonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.neo4JNonCustomizableLayoutControl.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem24 });
		this.neo4JNonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.neo4JNonCustomizableLayoutControl.Name = "neo4JNonCustomizableLayoutControl";
		this.neo4JNonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(766, 290, 698, 726);
		this.neo4JNonCustomizableLayoutControl.OptionsFocus.ActivateSelectedControlOnGotFocus = false;
		this.neo4JNonCustomizableLayoutControl.Root = this.layoutControlGroup11;
		this.neo4JNonCustomizableLayoutControl.Size = new System.Drawing.Size(568, 387);
		this.neo4JNonCustomizableLayoutControl.TabIndex = 30;
		this.neo4JNonCustomizableLayoutControl.Text = "layoutControl1";
		this.neo4jDefaultConnectionURLLabelControl.AllowHtmlString = true;
		this.neo4jDefaultConnectionURLLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.neo4jDefaultConnectionURLLabelControl.Location = new System.Drawing.Point(346, 2);
		this.neo4jDefaultConnectionURLLabelControl.MinimumSize = new System.Drawing.Size(34, 20);
		this.neo4jDefaultConnectionURLLabelControl.Name = "neo4jDefaultConnectionURLLabelControl";
		this.neo4jDefaultConnectionURLLabelControl.Size = new System.Drawing.Size(34, 20);
		this.neo4jDefaultConnectionURLLabelControl.StyleController = this.neo4JNonCustomizableLayoutControl;
		this.neo4jDefaultConnectionURLLabelControl.TabIndex = 31;
		this.neo4jDefaultConnectionURLLabelControl.Text = "<href>default</href>";
		this.neo4jDefaultConnectionURLLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(neo4jDefaultConnectionURLLabelControl_HyperlinkClick);
		this.neo4jSavePasswordCheckEdit.Location = new System.Drawing.Point(105, 72);
		this.neo4jSavePasswordCheckEdit.MaximumSize = new System.Drawing.Size(95, 0);
		this.neo4jSavePasswordCheckEdit.Name = "neo4jSavePasswordCheckEdit";
		this.neo4jSavePasswordCheckEdit.Properties.Caption = "Save password";
		this.neo4jSavePasswordCheckEdit.Size = new System.Drawing.Size(95, 20);
		this.neo4jSavePasswordCheckEdit.StyleController = this.neo4JNonCustomizableLayoutControl;
		this.neo4jSavePasswordCheckEdit.TabIndex = 3;
		this.neo4jLoginTextEdit.Location = new System.Drawing.Point(105, 24);
		this.neo4jLoginTextEdit.Name = "neo4jLoginTextEdit";
		this.neo4jLoginTextEdit.Size = new System.Drawing.Size(230, 20);
		this.neo4jLoginTextEdit.StyleController = this.neo4JNonCustomizableLayoutControl;
		this.neo4jLoginTextEdit.TabIndex = 1;
		this.neo4jPasswordTextEdit.Location = new System.Drawing.Point(105, 48);
		this.neo4jPasswordTextEdit.Name = "neo4jPasswordTextEdit";
		this.neo4jPasswordTextEdit.Properties.UseSystemPasswordChar = true;
		this.neo4jPasswordTextEdit.Size = new System.Drawing.Size(230, 20);
		this.neo4jPasswordTextEdit.StyleController = this.neo4JNonCustomizableLayoutControl;
		this.neo4jPasswordTextEdit.TabIndex = 2;
		this.neo4jHostTextEdit.EditValue = "bolt://localhost:7687";
		this.neo4jHostTextEdit.Location = new System.Drawing.Point(105, 0);
		this.neo4jHostTextEdit.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.neo4jHostTextEdit.Name = "neo4jHostTextEdit";
		this.neo4jHostTextEdit.Size = new System.Drawing.Size(230, 20);
		this.neo4jHostTextEdit.StyleController = this.neo4JNonCustomizableLayoutControl;
		this.neo4jHostTextEdit.TabIndex = 1;
		this.neo4jDatabaseButtonEdit.Location = new System.Drawing.Point(105, 96);
		this.neo4jDatabaseButtonEdit.Name = "neo4jDatabaseButtonEdit";
		this.neo4jDatabaseButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.neo4jDatabaseButtonEdit.Size = new System.Drawing.Size(230, 20);
		this.neo4jDatabaseButtonEdit.StyleController = this.neo4JNonCustomizableLayoutControl;
		this.neo4jDatabaseButtonEdit.TabIndex = 4;
		this.neo4jDatabaseButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(neo4jDatabaseButtonEdit_ButtonClick);
		this.layoutControlItem24.Location = new System.Drawing.Point(541, 120);
		this.layoutControlItem24.Name = "layoutControlItem24";
		this.layoutControlItem24.Size = new System.Drawing.Size(104, 24);
		this.layoutControlItem24.Text = " ";
		this.layoutControlItem24.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem24.TextVisible = false;
		this.layoutControlGroup11.CustomizationFormText = "Root";
		this.layoutControlGroup11.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup11.GroupBordersVisible = false;
		this.layoutControlGroup11.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[12]
		{
			this.neo4jPasswordTypeLayoutControlItem, this.neo4jUserTypeLayoutControlItem, this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem, this.neo4jUserTypeLayoutControlItemEmptySpaceItem, this.neo4jPasswordTypeLayoutControlItemEmptySpaceItem, this.neo4jEmptySpaceItem, this.neo4jHostTypeLayoutControlItem, this.neo4jPortTypeLayoutControlItemEmptySpaceItem, this.layoutControlItem28, this.emptySpaceItem75,
			this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem, this.emptySpaceItem1
		});
		this.layoutControlGroup11.Name = "Root";
		this.layoutControlGroup11.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup11.Size = new System.Drawing.Size(568, 387);
		this.layoutControlGroup11.TextVisible = false;
		this.neo4jPasswordTypeLayoutControlItem.Control = this.neo4jPasswordTextEdit;
		this.neo4jPasswordTypeLayoutControlItem.CustomizationFormText = "Password";
		this.neo4jPasswordTypeLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.neo4jPasswordTypeLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.neo4jPasswordTypeLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.neo4jPasswordTypeLayoutControlItem.Name = "neo4jPasswordTypeLayoutControlItem";
		this.neo4jPasswordTypeLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.neo4jPasswordTypeLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.neo4jPasswordTypeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.neo4jPasswordTypeLayoutControlItem.Text = "Password:";
		this.neo4jPasswordTypeLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.neo4jPasswordTypeLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.neo4jPasswordTypeLayoutControlItem.TextToControlDistance = 5;
		this.neo4jUserTypeLayoutControlItem.Control = this.neo4jLoginTextEdit;
		this.neo4jUserTypeLayoutControlItem.CustomizationFormText = "User:";
		this.neo4jUserTypeLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.neo4jUserTypeLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.neo4jUserTypeLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.neo4jUserTypeLayoutControlItem.Name = "neo4jUserTypeLayoutControlItem";
		this.neo4jUserTypeLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.neo4jUserTypeLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.neo4jUserTypeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.neo4jUserTypeLayoutControlItem.Text = "User:";
		this.neo4jUserTypeLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.neo4jUserTypeLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.neo4jUserTypeLayoutControlItem.TextToControlDistance = 5;
		this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem.Control = this.neo4jSavePasswordCheckEdit;
		this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem.CustomizationFormText = "layoutControlItem2";
		this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(0, 72);
		this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem.MaxSize = new System.Drawing.Size(335, 24);
		this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem.MinSize = new System.Drawing.Size(335, 24);
		this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem.Name = "neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem";
		this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(568, 24);
		this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem.Text = " ";
		this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(100, 13);
		this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem.TextToControlDistance = 5;
		this.neo4jUserTypeLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.neo4jUserTypeLayoutControlItemEmptySpaceItem.CustomizationFormText = "emptySpaceItem21";
		this.neo4jUserTypeLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 24);
		this.neo4jUserTypeLayoutControlItemEmptySpaceItem.Name = "neo4jUserTypeLayoutControlItemEmptySpaceItem";
		this.neo4jUserTypeLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(233, 24);
		this.neo4jUserTypeLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.neo4jPasswordTypeLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.neo4jPasswordTypeLayoutControlItemEmptySpaceItem.CustomizationFormText = "emptySpaceItem22";
		this.neo4jPasswordTypeLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(335, 48);
		this.neo4jPasswordTypeLayoutControlItemEmptySpaceItem.Name = "neo4jPasswordTypeLayoutControlItemEmptySpaceItem";
		this.neo4jPasswordTypeLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(233, 24);
		this.neo4jPasswordTypeLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.neo4jEmptySpaceItem.AllowHotTrack = false;
		this.neo4jEmptySpaceItem.CustomizationFormText = "emptySpaceItem12";
		this.neo4jEmptySpaceItem.Location = new System.Drawing.Point(0, 120);
		this.neo4jEmptySpaceItem.MaxSize = new System.Drawing.Size(405, 24);
		this.neo4jEmptySpaceItem.MinSize = new System.Drawing.Size(405, 24);
		this.neo4jEmptySpaceItem.Name = "neo4jEmptySpaceItem";
		this.neo4jEmptySpaceItem.Size = new System.Drawing.Size(568, 24);
		this.neo4jEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.neo4jEmptySpaceItem.Text = "emptySpaceItem12";
		this.neo4jEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.neo4jHostTypeLayoutControlItem.Control = this.neo4jHostTextEdit;
		this.neo4jHostTypeLayoutControlItem.CustomizationFormText = "Connection URL:";
		this.neo4jHostTypeLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.neo4jHostTypeLayoutControlItem.MaxSize = new System.Drawing.Size(335, 24);
		this.neo4jHostTypeLayoutControlItem.MinSize = new System.Drawing.Size(335, 24);
		this.neo4jHostTypeLayoutControlItem.Name = "neo4jHostTypeLayoutControlItem";
		this.neo4jHostTypeLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.neo4jHostTypeLayoutControlItem.Size = new System.Drawing.Size(335, 24);
		this.neo4jHostTypeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.neo4jHostTypeLayoutControlItem.Text = "Connection URL:";
		this.neo4jHostTypeLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.neo4jHostTypeLayoutControlItem.TextSize = new System.Drawing.Size(100, 13);
		this.neo4jHostTypeLayoutControlItem.TextToControlDistance = 5;
		this.neo4jPortTypeLayoutControlItemEmptySpaceItem.AllowHotTrack = false;
		this.neo4jPortTypeLayoutControlItemEmptySpaceItem.CustomizationFormText = "emptySpaceItem20";
		this.neo4jPortTypeLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(381, 0);
		this.neo4jPortTypeLayoutControlItemEmptySpaceItem.Name = "neo4jPortTypeLayoutControlItemEmptySpaceItem";
		this.neo4jPortTypeLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(187, 24);
		this.neo4jPortTypeLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem28.Control = this.neo4jDefaultConnectionURLLabelControl;
		this.layoutControlItem28.Location = new System.Drawing.Point(345, 0);
		this.layoutControlItem28.Name = "layoutControlItem27";
		this.layoutControlItem28.Size = new System.Drawing.Size(36, 24);
		this.layoutControlItem28.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem28.TextVisible = false;
		this.emptySpaceItem75.AllowHotTrack = false;
		this.emptySpaceItem75.Location = new System.Drawing.Point(0, 144);
		this.emptySpaceItem75.Name = "emptySpaceItem75";
		this.emptySpaceItem75.Size = new System.Drawing.Size(568, 243);
		this.emptySpaceItem75.TextSize = new System.Drawing.Size(0, 0);
		this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem.Control = this.neo4jDatabaseButtonEdit;
		this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem.Location = new System.Drawing.Point(0, 96);
		this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem.MaxSize = new System.Drawing.Size(335, 24);
		this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem.MinSize = new System.Drawing.Size(335, 24);
		this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem.Name = "neo4jDatabaseTypeLayoutControlItemEmptySpaceItem";
		this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem.Size = new System.Drawing.Size(568, 24);
		this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem.Text = "Database:";
		this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem.TextSize = new System.Drawing.Size(100, 13);
		this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem.TextToControlDistance = 5;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(335, 0);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.neo4JNonCustomizableLayoutControl);
		base.Name = "Neo4jConnectorControl";
		base.Size = new System.Drawing.Size(568, 387);
		((System.ComponentModel.ISupportInitialize)this.neo4JNonCustomizableLayoutControl).EndInit();
		this.neo4JNonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.neo4jSavePasswordCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jLoginTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jPasswordTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jHostTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jDatabaseButtonEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jPasswordTypeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jUserTypeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jSavePasswordTypeLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jUserTypeLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jPasswordTypeLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jHostTypeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jPortTypeLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem28).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem75).EndInit();
		((System.ComponentModel.ISupportInitialize)this.neo4jDatabaseTypeLayoutControlItemEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		base.ResumeLayout(false);
	}
}
