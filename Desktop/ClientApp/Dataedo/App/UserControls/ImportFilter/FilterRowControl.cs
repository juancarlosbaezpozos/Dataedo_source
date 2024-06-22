using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Tools;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.ImportFilter;

public class FilterRowControl : BaseUserControl
{
	private SharedDatabaseTypeEnum.DatabaseType? databaseType;

	private IContainer components;

	private NonCustomizableLayoutControl layoutControl;

	private LayoutControlGroup layoutControlGroup1;

	private LookUpEdit objectTypeLookUpEdit;

	private TextEdit valueTextEdit;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem layoutControlItem2;

	private EmptySpaceItem emptySpaceItem2;

	private TextEdit schemaTextEdit;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem1;

	private LabelControl removeLabelControl;

	private LayoutControlItem layoutControlItem4;

	private EmptySpaceItem emptySpaceItem3;

	public FilterRuleType RuleType { get; set; }

	public FilterObjectTypeEnum.FilterObjectType ObjectType => (FilterObjectTypeEnum.FilterObjectType)objectTypeLookUpEdit.EditValue;

	public string SchemaFilter => schemaTextEdit.Text;

	public string ValueFilter => valueTextEdit.Text;

	public event Action FilterChangedByUserEvent;

	public event HyperlinkClickEventHandler RemoveClick
	{
		add
		{
			removeLabelControl.HyperlinkClick += value;
		}
		remove
		{
			removeLabelControl.HyperlinkClick -= value;
		}
	}

	public FilterRowControl(SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		InitializeComponent();
		this.databaseType = databaseType;
		objectTypeLookUpEdit.EditValue = FilterObjectTypeEnum.FilterObjectType.Any;
		LayoutVisibility schemaControlsVisibility = CheckSchemaControlsVisibility(databaseType);
		SetSchemaControlsVisibility(schemaControlsVisibility);
		LengthValidation.SetFilterConditionLengthLimit(valueTextEdit);
		LengthValidation.SetFilterConditionLengthLimit(schemaTextEdit);
	}

	private LayoutVisibility CheckSchemaControlsVisibility(SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		if (!DatabaseSupportFactory.GetDatabaseSupport(databaseType).CanFilterBySchema)
		{
			return LayoutVisibility.Never;
		}
		return LayoutVisibility.Always;
	}

	private void SetSchemaControlsVisibility(LayoutVisibility visibility)
	{
		LayoutVisibility layoutVisibility3 = (layoutControlItem3.Visibility = (emptySpaceItem1.Visibility = visibility));
		switch (visibility)
		{
		case LayoutVisibility.Always:
			base.Width = 476;
			break;
		case LayoutVisibility.Never:
			base.Width = 304;
			break;
		}
	}

	public FilterRowControl(FilterRule rule, SharedDatabaseTypeEnum.DatabaseType? databaseType)
		: this(databaseType)
	{
		RuleType = rule.RuleType;
		objectTypeLookUpEdit.EditValue = rule.ObjectType;
		schemaTextEdit.Text = rule.Schema;
		valueTextEdit.Text = rule.Name;
	}

	public FilterRowControl(FilterRuleType ruleType, SharedDatabaseTypeEnum.DatabaseType? databaseType)
		: this(databaseType)
	{
		RuleType = ruleType;
	}

	private void FilterRowControl_Load(object sender, EventArgs e)
	{
		Dictionary<FilterObjectTypeEnum.FilterObjectType, string> dictionary = ((databaseType != SharedDatabaseTypeEnum.DatabaseType.MongoDB && databaseType != SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) ? Enum.GetValues(typeof(FilterObjectTypeEnum.FilterObjectType)).Cast<FilterObjectTypeEnum.FilterObjectType>().ToDictionary((FilterObjectTypeEnum.FilterObjectType x) => x, (FilterObjectTypeEnum.FilterObjectType x) => x.GetAttributeDescription()) : new Dictionary<FilterObjectTypeEnum.FilterObjectType, string> { 
		{
			FilterObjectTypeEnum.FilterObjectType.Any,
			FilterObjectTypeEnum.FilterObjectType.Any.GetAttributeDescription()
		} });
		objectTypeLookUpEdit.Properties.Columns.Add(new LookUpColumnInfo("Value"));
		objectTypeLookUpEdit.Properties.DropDownRows = dictionary.Count;
		objectTypeLookUpEdit.Properties.ValueMember = "Key";
		objectTypeLookUpEdit.Properties.DisplayMember = "Value";
		objectTypeLookUpEdit.Properties.DataSource = dictionary;
	}

	private void objectTypeLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		this.FilterChangedByUserEvent?.Invoke();
	}

	private void removeLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		this.FilterChangedByUserEvent?.Invoke();
	}

	private void schemaTextEdit_KeyUp(object sender, KeyEventArgs e)
	{
		this.FilterChangedByUserEvent?.Invoke();
	}

	private void valueTextEdit_KeyUp(object sender, KeyEventArgs e)
	{
		this.FilterChangedByUserEvent?.Invoke();
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
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.removeLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.schemaTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.objectTypeLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.valueTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.schemaTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectTypeLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valueTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		base.SuspendLayout();
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.Controls.Add(this.removeLabelControl);
		this.layoutControl.Controls.Add(this.schemaTextEdit);
		this.layoutControl.Controls.Add(this.objectTypeLookUpEdit);
		this.layoutControl.Controls.Add(this.valueTextEdit);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(788, 330, 250, 350);
		this.layoutControl.Root = this.layoutControlGroup1;
		this.layoutControl.Size = new System.Drawing.Size(476, 24);
		this.layoutControl.TabIndex = 0;
		this.layoutControl.Text = "layoutControl1";
		this.removeLabelControl.AllowHtmlString = true;
		this.removeLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.removeLabelControl.Location = new System.Drawing.Point(438, 2);
		this.removeLabelControl.Name = "removeLabelControl";
		this.removeLabelControl.Size = new System.Drawing.Size(36, 13);
		this.removeLabelControl.StyleController = this.layoutControl;
		this.removeLabelControl.TabIndex = 3;
		this.removeLabelControl.Text = "<href>remove</href>";
		this.removeLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(removeLabelControl_HyperlinkClick);
		this.schemaTextEdit.EditValue = "";
		this.schemaTextEdit.Location = new System.Drawing.Point(3, 0);
		this.schemaTextEdit.Name = "schemaTextEdit";
		this.schemaTextEdit.Properties.NullValuePrompt = "Schema pattern";
		this.schemaTextEdit.Properties.NullValuePromptShowForEmptyValue = true;
		this.schemaTextEdit.Size = new System.Drawing.Size(163, 20);
		this.schemaTextEdit.StyleController = this.layoutControl;
		this.schemaTextEdit.TabIndex = 0;
		this.schemaTextEdit.KeyUp += new System.Windows.Forms.KeyEventHandler(schemaTextEdit_KeyUp);
		this.objectTypeLookUpEdit.Location = new System.Drawing.Point(349, 0);
		this.objectTypeLookUpEdit.Name = "objectTypeLookUpEdit";
		this.objectTypeLookUpEdit.Properties.AllowMouseWheel = false;
		this.objectTypeLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.objectTypeLookUpEdit.Properties.DropDownRows = 5;
		this.objectTypeLookUpEdit.Properties.NullText = "All classes";
		this.objectTypeLookUpEdit.Properties.ShowFooter = false;
		this.objectTypeLookUpEdit.Properties.ShowHeader = false;
		this.objectTypeLookUpEdit.Size = new System.Drawing.Size(77, 20);
		this.objectTypeLookUpEdit.StyleController = this.layoutControl;
		this.objectTypeLookUpEdit.TabIndex = 2;
		this.objectTypeLookUpEdit.EditValueChanged += new System.EventHandler(objectTypeLookUpEdit_EditValueChanged);
		this.valueTextEdit.EditValue = "";
		this.valueTextEdit.Location = new System.Drawing.Point(176, 0);
		this.valueTextEdit.Name = "valueTextEdit";
		this.valueTextEdit.Properties.NullValuePrompt = "Name pattern";
		this.valueTextEdit.Properties.NullValuePromptShowForEmptyValue = true;
		this.valueTextEdit.Size = new System.Drawing.Size(163, 20);
		this.valueTextEdit.StyleController = this.layoutControl;
		this.valueTextEdit.TabIndex = 1;
		this.valueTextEdit.KeyUp += new System.Windows.Forms.KeyEventHandler(valueTextEdit_KeyUp);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[7] { this.layoutControlItem1, this.layoutControlItem2, this.emptySpaceItem2, this.layoutControlItem3, this.emptySpaceItem1, this.layoutControlItem4, this.emptySpaceItem3 });
		this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(476, 24);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.Control = this.valueTextEdit;
		this.layoutControlItem1.Location = new System.Drawing.Point(176, 0);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(163, 24);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(163, 24);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem1.Size = new System.Drawing.Size(163, 24);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem2.Control = this.objectTypeLookUpEdit;
		this.layoutControlItem2.Location = new System.Drawing.Point(349, 0);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(0, 24);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(1, 24);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem2.Size = new System.Drawing.Size(77, 24);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(339, 0);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.emptySpaceItem2.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.Control = this.schemaTextEdit;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(166, 24);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(166, 24);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 0, 0, 0);
		this.layoutControlItem3.Size = new System.Drawing.Size(166, 24);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(166, 0);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.emptySpaceItem1.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.Control = this.removeLabelControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(436, 0);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(40, 24);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(426, 0);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.emptySpaceItem3.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.layoutControl);
		base.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
		base.Name = "FilterRowControl";
		base.Size = new System.Drawing.Size(476, 24);
		base.Load += new System.EventHandler(FilterRowControl_Load);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.schemaTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectTypeLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valueTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		base.ResumeLayout(false);
	}
}
