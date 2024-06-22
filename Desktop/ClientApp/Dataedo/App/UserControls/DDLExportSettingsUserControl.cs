using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Tools.DDLGenerating;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class DDLExportSettingsUserControl : BaseUserControl
{
	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private RadioGroup schemaRadioGroup;

	private RadioGroup escapeRadioGroup;

	private LabelControl keysLabelControl;

	private LabelControl additionalLabelControl;

	private LayoutControlItem additionalLayoutControlItem;

	private LayoutControlItem keysLayoutControlItem;

	private CheckEdit foreignKeysCheckEdit;

	private CheckEdit uniqueKeysCheckEdit;

	private CheckEdit primaryKeysCheckEdit;

	private CheckEdit defaultValuesCheckEdit;

	private CheckEdit nullabilityCheckEdit;

	private LayoutControlItem nullabilityLayoutControlItem;

	private LayoutControlItem defaultValuesLayoutControlItem;

	private LayoutControlItem primaryKeysLayoutControlItem;

	private LayoutControlItem uniqueKeysLayoutControlItem;

	private LayoutControlItem foreignKeysLayoutControlItem;

	private EmptySpaceItem emptySpaceItem2;

	private EmptySpaceItem emptySpaceItem3;

	private PanelControl schemaPanelControl;

	private GrayLabelControl grayLabelControl2;

	private GrayLabelControl grayLabelControl1;

	private LayoutControlItem layoutControlItem1;

	private PanelControl escapePanelControl;

	private GrayLabelControl grayLabelControl8;

	private GrayLabelControl grayLabelControl7;

	private GrayLabelControl grayLabelControl6;

	private GrayLabelControl grayLabelControl5;

	private GrayLabelControl grayLabelControl4;

	private GrayLabelControl grayLabelControl3;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private LayoutControlItem layoutControlItem4;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem4;

	public DDLExportSettingsUserControl()
	{
		InitializeComponent();
	}

	public DDLGeneratorSettings GetSettings()
	{
		return new DDLGeneratorSettings((escapeRadioGroup.EditValue as string) switch
		{
			"none" => EscapeCharacterModeEnum.None, 
			"sql_server" => EscapeCharacterModeEnum.SQLServerLike, 
			"mysql" => EscapeCharacterModeEnum.MySQLLike, 
			"oracle" => EscapeCharacterModeEnum.OracleLike, 
			_ => EscapeCharacterModeEnum.None, 
		}, schemaRadioGroup.EditValue as bool? == true, nullabilityCheckEdit.Checked, defaultValuesCheckEdit.Checked, primaryKeysCheckEdit.Checked, uniqueKeysCheckEdit.Checked, foreignKeysCheckEdit.Checked);
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
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.escapePanelControl = new DevExpress.XtraEditors.PanelControl();
		this.escapeRadioGroup = new DevExpress.XtraEditors.RadioGroup();
		this.schemaPanelControl = new DevExpress.XtraEditors.PanelControl();
		this.schemaRadioGroup = new DevExpress.XtraEditors.RadioGroup();
		this.foreignKeysCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.uniqueKeysCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.primaryKeysCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.defaultValuesCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.nullabilityCheckEdit = new DevExpress.XtraEditors.CheckEdit();
		this.keysLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.additionalLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.additionalLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.keysLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.nullabilityLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.defaultValuesLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.primaryKeysLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.uniqueKeysLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.foreignKeysLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.grayLabelControl7 = new Dataedo.App.UserControls.GrayLabelControl();
		this.grayLabelControl8 = new Dataedo.App.UserControls.GrayLabelControl();
		this.grayLabelControl3 = new Dataedo.App.UserControls.GrayLabelControl();
		this.grayLabelControl5 = new Dataedo.App.UserControls.GrayLabelControl();
		this.grayLabelControl4 = new Dataedo.App.UserControls.GrayLabelControl();
		this.grayLabelControl6 = new Dataedo.App.UserControls.GrayLabelControl();
		this.grayLabelControl2 = new Dataedo.App.UserControls.GrayLabelControl();
		this.grayLabelControl1 = new Dataedo.App.UserControls.GrayLabelControl();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.escapePanelControl).BeginInit();
		this.escapePanelControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.escapeRadioGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.schemaPanelControl).BeginInit();
		this.schemaPanelControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.schemaRadioGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.foreignKeysCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.uniqueKeysCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.primaryKeysCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.defaultValuesCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nullabilityCheckEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.additionalLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.keysLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nullabilityLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.defaultValuesLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.primaryKeysLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.uniqueKeysLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.foreignKeysLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.grayLabelControl7);
		this.nonCustomizableLayoutControl.Controls.Add(this.escapePanelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.grayLabelControl6);
		this.nonCustomizableLayoutControl.Controls.Add(this.schemaPanelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.foreignKeysCheckEdit);
		this.nonCustomizableLayoutControl.Controls.Add(this.uniqueKeysCheckEdit);
		this.nonCustomizableLayoutControl.Controls.Add(this.primaryKeysCheckEdit);
		this.nonCustomizableLayoutControl.Controls.Add(this.defaultValuesCheckEdit);
		this.nonCustomizableLayoutControl.Controls.Add(this.nullabilityCheckEdit);
		this.nonCustomizableLayoutControl.Controls.Add(this.keysLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.additionalLabelControl);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Margin = new System.Windows.Forms.Padding(2);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(723, 253, 650, 400);
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(517, 457);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.escapePanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.escapePanelControl.Controls.Add(this.grayLabelControl8);
		this.escapePanelControl.Controls.Add(this.grayLabelControl3);
		this.escapePanelControl.Controls.Add(this.grayLabelControl5);
		this.escapePanelControl.Controls.Add(this.grayLabelControl4);
		this.escapePanelControl.Controls.Add(this.escapeRadioGroup);
		this.escapePanelControl.Location = new System.Drawing.Point(12, 113);
		this.escapePanelControl.Name = "escapePanelControl";
		this.escapePanelControl.Size = new System.Drawing.Size(493, 146);
		this.escapePanelControl.TabIndex = 9;
		this.escapeRadioGroup.Dock = System.Windows.Forms.DockStyle.Fill;
		this.escapeRadioGroup.EditValue = "none";
		this.escapeRadioGroup.Location = new System.Drawing.Point(0, 0);
		this.escapeRadioGroup.Margin = new System.Windows.Forms.Padding(2);
		this.escapeRadioGroup.Name = "escapeRadioGroup";
		this.escapeRadioGroup.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.escapeRadioGroup.Properties.Appearance.Options.UseBackColor = true;
		this.escapeRadioGroup.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.escapeRadioGroup.Properties.Columns = 1;
		this.escapeRadioGroup.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[4]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("none", "none"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("sql_server", "SQL Server like"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("mysql", "MySQL like"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("oracle", "Oracle like")
		});
		this.escapeRadioGroup.Size = new System.Drawing.Size(493, 146);
		this.escapeRadioGroup.TabIndex = 1;
		this.schemaPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.schemaPanelControl.Controls.Add(this.grayLabelControl2);
		this.schemaPanelControl.Controls.Add(this.grayLabelControl1);
		this.schemaPanelControl.Controls.Add(this.schemaRadioGroup);
		this.schemaPanelControl.Location = new System.Drawing.Point(12, 27);
		this.schemaPanelControl.Margin = new System.Windows.Forms.Padding(2);
		this.schemaPanelControl.Name = "schemaPanelControl";
		this.schemaPanelControl.Size = new System.Drawing.Size(493, 67);
		this.schemaPanelControl.TabIndex = 8;
		this.schemaRadioGroup.Dock = System.Windows.Forms.DockStyle.Fill;
		this.schemaRadioGroup.EditValue = true;
		this.schemaRadioGroup.Location = new System.Drawing.Point(0, 0);
		this.schemaRadioGroup.Margin = new System.Windows.Forms.Padding(2);
		this.schemaRadioGroup.Name = "schemaRadioGroup";
		this.schemaRadioGroup.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.schemaRadioGroup.Properties.Appearance.Options.UseBackColor = true;
		this.schemaRadioGroup.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.schemaRadioGroup.Properties.Columns = 1;
		this.schemaRadioGroup.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(true, "Include schema"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(false, "Don't include schema")
		});
		this.schemaRadioGroup.Size = new System.Drawing.Size(493, 67);
		this.schemaRadioGroup.TabIndex = 0;
		this.foreignKeysCheckEdit.EditValue = true;
		this.foreignKeysCheckEdit.Location = new System.Drawing.Point(12, 399);
		this.foreignKeysCheckEdit.Margin = new System.Windows.Forms.Padding(2);
		this.foreignKeysCheckEdit.Name = "foreignKeysCheckEdit";
		this.foreignKeysCheckEdit.Properties.Caption = "Create foreign keys";
		this.foreignKeysCheckEdit.Size = new System.Drawing.Size(493, 20);
		this.foreignKeysCheckEdit.StyleController = this.nonCustomizableLayoutControl;
		this.foreignKeysCheckEdit.TabIndex = 6;
		this.uniqueKeysCheckEdit.EditValue = true;
		this.uniqueKeysCheckEdit.Location = new System.Drawing.Point(12, 375);
		this.uniqueKeysCheckEdit.Margin = new System.Windows.Forms.Padding(2);
		this.uniqueKeysCheckEdit.Name = "uniqueKeysCheckEdit";
		this.uniqueKeysCheckEdit.Properties.Caption = "Create unique keys";
		this.uniqueKeysCheckEdit.Size = new System.Drawing.Size(493, 20);
		this.uniqueKeysCheckEdit.StyleController = this.nonCustomizableLayoutControl;
		this.uniqueKeysCheckEdit.TabIndex = 5;
		this.primaryKeysCheckEdit.EditValue = true;
		this.primaryKeysCheckEdit.Location = new System.Drawing.Point(12, 351);
		this.primaryKeysCheckEdit.Margin = new System.Windows.Forms.Padding(2);
		this.primaryKeysCheckEdit.Name = "primaryKeysCheckEdit";
		this.primaryKeysCheckEdit.Properties.Caption = "Create primary keys";
		this.primaryKeysCheckEdit.Size = new System.Drawing.Size(493, 20);
		this.primaryKeysCheckEdit.StyleController = this.nonCustomizableLayoutControl;
		this.primaryKeysCheckEdit.TabIndex = 4;
		this.defaultValuesCheckEdit.EditValue = true;
		this.defaultValuesCheckEdit.Location = new System.Drawing.Point(12, 303);
		this.defaultValuesCheckEdit.Margin = new System.Windows.Forms.Padding(2);
		this.defaultValuesCheckEdit.Name = "defaultValuesCheckEdit";
		this.defaultValuesCheckEdit.Properties.Caption = "Set default values";
		this.defaultValuesCheckEdit.Size = new System.Drawing.Size(115, 20);
		this.defaultValuesCheckEdit.StyleController = this.nonCustomizableLayoutControl;
		this.defaultValuesCheckEdit.TabIndex = 3;
		this.nullabilityCheckEdit.EditValue = true;
		this.nullabilityCheckEdit.Location = new System.Drawing.Point(12, 279);
		this.nullabilityCheckEdit.Margin = new System.Windows.Forms.Padding(2);
		this.nullabilityCheckEdit.Name = "nullabilityCheckEdit";
		this.nullabilityCheckEdit.Properties.Caption = "Set nullability";
		this.nullabilityCheckEdit.Size = new System.Drawing.Size(113, 20);
		this.nullabilityCheckEdit.StyleController = this.nonCustomizableLayoutControl;
		this.nullabilityCheckEdit.TabIndex = 2;
		this.keysLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 7.8f, System.Drawing.FontStyle.Bold);
		this.keysLabelControl.Appearance.Options.UseFont = true;
		this.keysLabelControl.Location = new System.Drawing.Point(12, 335);
		this.keysLabelControl.Margin = new System.Windows.Forms.Padding(2);
		this.keysLabelControl.Name = "keysLabelControl";
		this.keysLabelControl.Size = new System.Drawing.Size(493, 12);
		this.keysLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.keysLabelControl.TabIndex = 7;
		this.keysLabelControl.Text = "Keys";
		this.additionalLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 7.8f, System.Drawing.FontStyle.Bold);
		this.additionalLabelControl.Appearance.Options.UseFont = true;
		this.additionalLabelControl.Location = new System.Drawing.Point(12, 263);
		this.additionalLabelControl.Margin = new System.Windows.Forms.Padding(2);
		this.additionalLabelControl.Name = "additionalLabelControl";
		this.additionalLabelControl.Size = new System.Drawing.Size(493, 12);
		this.additionalLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.additionalLabelControl.TabIndex = 6;
		this.additionalLabelControl.Text = "Additional options";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[15]
		{
			this.additionalLayoutControlItem, this.keysLayoutControlItem, this.nullabilityLayoutControlItem, this.defaultValuesLayoutControlItem, this.primaryKeysLayoutControlItem, this.uniqueKeysLayoutControlItem, this.foreignKeysLayoutControlItem, this.emptySpaceItem2, this.emptySpaceItem3, this.layoutControlItem1,
			this.layoutControlItem2, this.layoutControlItem3, this.layoutControlItem4, this.emptySpaceItem1, this.emptySpaceItem4
		});
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(517, 457);
		this.Root.TextVisible = false;
		this.additionalLayoutControlItem.Control = this.additionalLabelControl;
		this.additionalLayoutControlItem.Location = new System.Drawing.Point(0, 251);
		this.additionalLayoutControlItem.MaxSize = new System.Drawing.Size(632, 16);
		this.additionalLayoutControlItem.MinSize = new System.Drawing.Size(1, 16);
		this.additionalLayoutControlItem.Name = "additionalLayoutControlItem";
		this.additionalLayoutControlItem.Size = new System.Drawing.Size(497, 16);
		this.additionalLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.additionalLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.additionalLayoutControlItem.TextVisible = false;
		this.keysLayoutControlItem.Control = this.keysLabelControl;
		this.keysLayoutControlItem.Location = new System.Drawing.Point(0, 323);
		this.keysLayoutControlItem.MaxSize = new System.Drawing.Size(632, 16);
		this.keysLayoutControlItem.MinSize = new System.Drawing.Size(1, 16);
		this.keysLayoutControlItem.Name = "keysLayoutControlItem";
		this.keysLayoutControlItem.Size = new System.Drawing.Size(497, 16);
		this.keysLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.keysLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.keysLayoutControlItem.TextVisible = false;
		this.nullabilityLayoutControlItem.Control = this.nullabilityCheckEdit;
		this.nullabilityLayoutControlItem.Location = new System.Drawing.Point(0, 267);
		this.nullabilityLayoutControlItem.MaxSize = new System.Drawing.Size(117, 24);
		this.nullabilityLayoutControlItem.MinSize = new System.Drawing.Size(117, 24);
		this.nullabilityLayoutControlItem.Name = "nullabilityLayoutControlItem";
		this.nullabilityLayoutControlItem.Size = new System.Drawing.Size(117, 24);
		this.nullabilityLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.nullabilityLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.nullabilityLayoutControlItem.TextVisible = false;
		this.defaultValuesLayoutControlItem.Control = this.defaultValuesCheckEdit;
		this.defaultValuesLayoutControlItem.Location = new System.Drawing.Point(0, 291);
		this.defaultValuesLayoutControlItem.MaxSize = new System.Drawing.Size(117, 24);
		this.defaultValuesLayoutControlItem.MinSize = new System.Drawing.Size(117, 24);
		this.defaultValuesLayoutControlItem.Name = "defaultValuesLayoutControlItem";
		this.defaultValuesLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
		this.defaultValuesLayoutControlItem.Size = new System.Drawing.Size(117, 24);
		this.defaultValuesLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.defaultValuesLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.defaultValuesLayoutControlItem.TextVisible = false;
		this.primaryKeysLayoutControlItem.Control = this.primaryKeysCheckEdit;
		this.primaryKeysLayoutControlItem.Location = new System.Drawing.Point(0, 339);
		this.primaryKeysLayoutControlItem.MaxSize = new System.Drawing.Size(632, 24);
		this.primaryKeysLayoutControlItem.MinSize = new System.Drawing.Size(1, 24);
		this.primaryKeysLayoutControlItem.Name = "primaryKeysLayoutControlItem";
		this.primaryKeysLayoutControlItem.Size = new System.Drawing.Size(497, 24);
		this.primaryKeysLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.primaryKeysLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.primaryKeysLayoutControlItem.TextVisible = false;
		this.uniqueKeysLayoutControlItem.Control = this.uniqueKeysCheckEdit;
		this.uniqueKeysLayoutControlItem.Location = new System.Drawing.Point(0, 363);
		this.uniqueKeysLayoutControlItem.MaxSize = new System.Drawing.Size(632, 24);
		this.uniqueKeysLayoutControlItem.MinSize = new System.Drawing.Size(1, 24);
		this.uniqueKeysLayoutControlItem.Name = "uniqueKeysLayoutControlItem";
		this.uniqueKeysLayoutControlItem.Size = new System.Drawing.Size(497, 24);
		this.uniqueKeysLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.uniqueKeysLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.uniqueKeysLayoutControlItem.TextVisible = false;
		this.foreignKeysLayoutControlItem.Control = this.foreignKeysCheckEdit;
		this.foreignKeysLayoutControlItem.Location = new System.Drawing.Point(0, 387);
		this.foreignKeysLayoutControlItem.MaxSize = new System.Drawing.Size(632, 24);
		this.foreignKeysLayoutControlItem.MinSize = new System.Drawing.Size(1, 24);
		this.foreignKeysLayoutControlItem.Name = "foreignKeysLayoutControlItem";
		this.foreignKeysLayoutControlItem.Size = new System.Drawing.Size(497, 24);
		this.foreignKeysLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.foreignKeysLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.foreignKeysLayoutControlItem.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 315);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(632, 8);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(1, 8);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(497, 8);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 411);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(497, 26);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 7.8f, System.Drawing.FontStyle.Bold);
		this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem1.Control = this.schemaPanelControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(632, 86);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(1, 86);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(497, 86);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.Text = "Schema";
		this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(130, 12);
		this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 7.8f, System.Drawing.FontStyle.Bold);
		this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem2.Control = this.escapePanelControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 86);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(632, 165);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(1, 165);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(497, 165);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.Text = "Identifier quote character";
		this.layoutControlItem2.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(130, 12);
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(393, 267);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(104, 24);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(273, 291);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(224, 24);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(224, 24);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(224, 24);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.grayLabelControl7.AllowHtmlString = true;
		this.grayLabelControl7.Appearance.Font = new System.Drawing.Font("Courier New", 8.25f);
		this.grayLabelControl7.Appearance.Options.UseFont = true;
		this.grayLabelControl7.Location = new System.Drawing.Point(127, 303);
		this.grayLabelControl7.Margin = new System.Windows.Forms.Padding(2);
		this.grayLabelControl7.MaximumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl7.MinimumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl7.Name = "grayLabelControl7";
		this.grayLabelControl7.Padding = new System.Windows.Forms.Padding(4);
		this.grayLabelControl7.Size = new System.Drawing.Size(154, 20);
		this.grayLabelControl7.StyleController = this.nonCustomizableLayoutControl;
		this.grayLabelControl7.TabIndex = 7;
		this.grayLabelControl7.Text = "column1 int <b>DEFAULT 1</b>";
		this.grayLabelControl8.AllowHtmlString = true;
		this.grayLabelControl8.Appearance.Font = new System.Drawing.Font("Courier New", 8.25f);
		this.grayLabelControl8.Appearance.Options.UseFont = true;
		this.grayLabelControl8.Location = new System.Drawing.Point(61, 11);
		this.grayLabelControl8.Margin = new System.Windows.Forms.Padding(2);
		this.grayLabelControl8.MaximumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl8.MinimumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl8.Name = "grayLabelControl8";
		this.grayLabelControl8.Padding = new System.Windows.Forms.Padding(4);
		this.grayLabelControl8.Size = new System.Drawing.Size(141, 20);
		this.grayLabelControl8.TabIndex = 8;
		this.grayLabelControl8.Text = "CREATE TABLE table1";
		this.grayLabelControl3.AllowHtmlString = true;
		this.grayLabelControl3.Appearance.Font = new System.Drawing.Font("Courier New", 8.25f);
		this.grayLabelControl3.Appearance.Options.UseFont = true;
		this.grayLabelControl3.Location = new System.Drawing.Point(109, 45);
		this.grayLabelControl3.Margin = new System.Windows.Forms.Padding(2);
		this.grayLabelControl3.MaximumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl3.MinimumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl3.Name = "grayLabelControl3";
		this.grayLabelControl3.Padding = new System.Windows.Forms.Padding(4);
		this.grayLabelControl3.Size = new System.Drawing.Size(155, 20);
		this.grayLabelControl3.TabIndex = 3;
		this.grayLabelControl3.Text = "CREATE TABLE [table1]";
		this.grayLabelControl5.AllowHtmlString = true;
		this.grayLabelControl5.Appearance.Font = new System.Drawing.Font("Courier New", 8.25f);
		this.grayLabelControl5.Appearance.Options.UseFont = true;
		this.grayLabelControl5.Location = new System.Drawing.Point(86, 113);
		this.grayLabelControl5.Margin = new System.Windows.Forms.Padding(2);
		this.grayLabelControl5.MaximumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl5.MinimumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl5.Name = "grayLabelControl5";
		this.grayLabelControl5.Padding = new System.Windows.Forms.Padding(4);
		this.grayLabelControl5.Size = new System.Drawing.Size(155, 20);
		this.grayLabelControl5.TabIndex = 5;
		this.grayLabelControl5.Text = "CREATE TABLE \"table1\"";
		this.grayLabelControl4.AllowHtmlString = true;
		this.grayLabelControl4.Appearance.Font = new System.Drawing.Font("Courier New", 8.25f);
		this.grayLabelControl4.Appearance.Options.UseFont = true;
		this.grayLabelControl4.Location = new System.Drawing.Point(88, 78);
		this.grayLabelControl4.Margin = new System.Windows.Forms.Padding(2);
		this.grayLabelControl4.MaximumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl4.MinimumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl4.Name = "grayLabelControl4";
		this.grayLabelControl4.Padding = new System.Windows.Forms.Padding(4);
		this.grayLabelControl4.Size = new System.Drawing.Size(155, 20);
		this.grayLabelControl4.TabIndex = 4;
		this.grayLabelControl4.Text = "CREATE TABLE `table1`";
		this.grayLabelControl6.AllowHtmlString = true;
		this.grayLabelControl6.Appearance.Font = new System.Drawing.Font("Courier New", 8.25f);
		this.grayLabelControl6.Appearance.Options.UseFont = true;
		this.grayLabelControl6.Location = new System.Drawing.Point(127, 279);
		this.grayLabelControl6.Margin = new System.Windows.Forms.Padding(2);
		this.grayLabelControl6.MaximumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl6.MinimumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl6.Name = "grayLabelControl6";
		this.grayLabelControl6.Padding = new System.Windows.Forms.Padding(4);
		this.grayLabelControl6.Size = new System.Drawing.Size(274, 20);
		this.grayLabelControl6.StyleController = this.nonCustomizableLayoutControl;
		this.grayLabelControl6.TabIndex = 6;
		this.grayLabelControl6.Text = "column1 int <b>NULL</b>, column2 int <b>NOT NULL</b>";
		this.grayLabelControl2.AllowHtmlString = true;
		this.grayLabelControl2.Appearance.Font = new System.Drawing.Font("Courier New", 8.25f);
		this.grayLabelControl2.Appearance.Options.UseFont = true;
		this.grayLabelControl2.Location = new System.Drawing.Point(137, 37);
		this.grayLabelControl2.Margin = new System.Windows.Forms.Padding(2);
		this.grayLabelControl2.MaximumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl2.MinimumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl2.Name = "grayLabelControl2";
		this.grayLabelControl2.Padding = new System.Windows.Forms.Padding(4);
		this.grayLabelControl2.Size = new System.Drawing.Size(141, 20);
		this.grayLabelControl2.TabIndex = 2;
		this.grayLabelControl2.Text = "CREATE TABLE table1";
		this.grayLabelControl1.AllowHtmlString = true;
		this.grayLabelControl1.Appearance.Font = new System.Drawing.Font("Courier New", 8.25f);
		this.grayLabelControl1.Appearance.Options.UseFont = true;
		this.grayLabelControl1.Location = new System.Drawing.Point(111, 8);
		this.grayLabelControl1.Margin = new System.Windows.Forms.Padding(2);
		this.grayLabelControl1.MaximumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl1.MinimumSize = new System.Drawing.Size(0, 20);
		this.grayLabelControl1.Name = "grayLabelControl1";
		this.grayLabelControl1.Padding = new System.Windows.Forms.Padding(4);
		this.grayLabelControl1.Size = new System.Drawing.Size(169, 20);
		this.grayLabelControl1.TabIndex = 1;
		this.grayLabelControl1.Text = "CREATE TABLE dbo.table1";
		this.layoutControlItem3.Control = this.grayLabelControl6;
		this.layoutControlItem3.Location = new System.Drawing.Point(117, 267);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(276, 24);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(276, 24);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
		this.layoutControlItem3.Size = new System.Drawing.Size(276, 24);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.layoutControlItem4.Control = this.grayLabelControl7;
		this.layoutControlItem4.Location = new System.Drawing.Point(117, 291);
		this.layoutControlItem4.MaxSize = new System.Drawing.Size(156, 24);
		this.layoutControlItem4.MinSize = new System.Drawing.Size(156, 24);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
		this.layoutControlItem4.Size = new System.Drawing.Size(156, 24);
		this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.Margin = new System.Windows.Forms.Padding(2);
		base.Name = "DDLExportSettingsUserControl";
		base.Size = new System.Drawing.Size(517, 457);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.escapePanelControl).EndInit();
		this.escapePanelControl.ResumeLayout(false);
		this.escapePanelControl.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.escapeRadioGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.schemaPanelControl).EndInit();
		this.schemaPanelControl.ResumeLayout(false);
		this.schemaPanelControl.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.schemaRadioGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.foreignKeysCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.uniqueKeysCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.primaryKeysCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.defaultValuesCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nullabilityCheckEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.additionalLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.keysLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nullabilityLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.defaultValuesLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.primaryKeysLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.uniqueKeysLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.foreignKeysLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		base.ResumeLayout(false);
	}
}
