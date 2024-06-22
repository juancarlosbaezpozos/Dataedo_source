using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Tools.Search;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.CustomFields;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls.Search;

public class CustomFieldSearchControl : BaseUserControl
{
	public static int BaseHeight = 24;

	private CustomFieldRowExtended customField;

	private IContainer components;

	private TextEdit valueTextEdit;

	private LabelControl removeLabelControl;

	private NonCustomizableLayoutControl layoutControl;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem valueLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	public CustomFieldRowExtended CustomField
	{
		get
		{
			return customField;
		}
		set
		{
			customField = value;
			Title = value?.ToString();
		}
	}

	public string Title
	{
		get
		{
			return valueLayoutControlItem.Text;
		}
		set
		{
			valueLayoutControlItem.Text = value;
		}
	}

	public string Value
	{
		get
		{
			return valueTextEdit.Text;
		}
		set
		{
			valueTextEdit.Text = value;
		}
	}

	public event Action<CustomFieldSearchControl> CustomFieldRemoved;

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public event PreviewKeyDownEventHandler FieldPreviewKeyDown
	{
		add
		{
			valueTextEdit.PreviewKeyDown += value;
		}
		remove
		{
			valueTextEdit.PreviewKeyDown -= value;
		}
	}

	[Browsable(true)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
	public event KeyPressEventHandler FieldKeyPress
	{
		add
		{
			valueTextEdit.KeyPress += value;
		}
		remove
		{
			valueTextEdit.KeyPress -= value;
		}
	}

	public CustomFieldSearchItem GetSearchItem()
	{
		return new CustomFieldSearchItem(CustomField, Value);
	}

	public CustomFieldSearchControl(CustomFieldRowExtended customField)
	{
		InitializeComponent();
		CustomField = customField;
	}

	private void removeLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		this.CustomFieldRemoved?.Invoke(this);
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
		this.valueTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.removeLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.valueLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.valueTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valueLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		base.SuspendLayout();
		this.valueTextEdit.Location = new System.Drawing.Point(105, 0);
		this.valueTextEdit.Name = "valueTextEdit";
		this.valueTextEdit.Properties.NullValuePrompt = "Enter text to search...";
		this.valueTextEdit.Size = new System.Drawing.Size(277, 20);
		this.valueTextEdit.StyleController = this.layoutControl;
		this.valueTextEdit.TabIndex = 1;
		this.layoutControl.Controls.Add(this.valueTextEdit);
		this.layoutControl.Controls.Add(this.removeLabelControl);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3443, 280, 250, 350);
		this.layoutControl.Root = this.layoutControlGroup1;
		this.layoutControl.Size = new System.Drawing.Size(432, 24);
		this.layoutControl.TabIndex = 3;
		this.layoutControl.Text = "layoutControl1";
		this.removeLabelControl.AllowHtmlString = true;
		this.removeLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.removeLabelControl.Location = new System.Drawing.Point(394, 2);
		this.removeLabelControl.Name = "removeLabelControl";
		this.removeLabelControl.Size = new System.Drawing.Size(36, 18);
		this.removeLabelControl.StyleController = this.layoutControl;
		this.removeLabelControl.TabIndex = 2;
		this.removeLabelControl.Text = "<href>remove</href>";
		this.removeLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(removeLabelControl_HyperlinkClick);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.layoutControlItem1, this.valueLayoutControlItem, this.emptySpaceItem1 });
		this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(432, 24);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.Control = this.removeLabelControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(392, 0);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(40, 22);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(40, 22);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(40, 24);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.valueLayoutControlItem.Control = this.valueTextEdit;
		this.valueLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.valueLayoutControlItem.MaxSize = new System.Drawing.Size(0, 22);
		this.valueLayoutControlItem.MinSize = new System.Drawing.Size(155, 22);
		this.valueLayoutControlItem.Name = "valueLayoutControlItem";
		this.valueLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.valueLayoutControlItem.Size = new System.Drawing.Size(382, 24);
		this.valueLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.valueLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.valueLayoutControlItem.TextSize = new System.Drawing.Size(100, 0);
		this.valueLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(382, 0);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 24);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(10, 24);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.layoutControl);
		base.Margin = new System.Windows.Forms.Padding(0);
		base.Name = "CustomFieldSearchControl";
		base.Size = new System.Drawing.Size(432, 24);
		((System.ComponentModel.ISupportInitialize)this.valueTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valueLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		base.ResumeLayout(false);
	}
}
