using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;

namespace Dataedo.App.Forms;

public class BusinessGlossaryExportForm : BaseXtraForm
{
	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	private SimpleButton closeSimpleButton;

	private EmptySpaceItem emptySpaceItem2;

	private LayoutControlItem closeLayoutControlItem;

	private LabelControl labelControl;

	private EmptySpaceItem emptySpaceItem1;

	private LayoutControlItem labelControlLayoutControlItem;

	public BusinessGlossaryExportForm()
	{
		InitializeComponent();
	}

	private void labelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			Links.OpenLink(e.Link, this);
		}
	}

	private void closeSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.BusinessGlossaryExportForm));
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.labelControl = new DevExpress.XtraEditors.LabelControl();
		this.closeSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.closeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.labelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.closeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.labelControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.Controls.Add(this.labelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.closeSimpleButton);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2545, 257, 250, 350);
		this.nonCustomizableLayoutControl1.Root = this.layoutControlGroup1;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(460, 120);
		this.nonCustomizableLayoutControl1.TabIndex = 0;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.labelControl.AllowHtmlString = true;
		this.labelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.labelControl.Location = new System.Drawing.Point(12, 12);
		this.labelControl.Name = "labelControl";
		this.labelControl.Size = new System.Drawing.Size(436, 60);
		this.labelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.labelControl.TabIndex = 6;
		this.labelControl.Text = "Users of <b>HTML Plus</b> export should have <b>Enterprise/Data Catalog</b> (any) subscription. Use of <b>HTML Basic</b> export is free. <href=https://dataedo.com/pricing>Learn more</href>";
		this.labelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(labelControl_HyperlinkClick);
		this.closeSimpleButton.Location = new System.Drawing.Point(368, 86);
		this.closeSimpleButton.Name = "closeSimpleButton";
		this.closeSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.closeSimpleButton.StyleController = this.nonCustomizableLayoutControl1;
		this.closeSimpleButton.TabIndex = 5;
		this.closeSimpleButton.Text = "OK";
		this.closeSimpleButton.Click += new System.EventHandler(closeSimpleButton_Click);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.emptySpaceItem1, this.emptySpaceItem2, this.closeLayoutControlItem, this.labelControlLayoutControlItem });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Size = new System.Drawing.Size(460, 120);
		this.layoutControlGroup1.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 64);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(385, 10);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(385, 10);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(440, 10);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 74);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(356, 26);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.closeLayoutControlItem.Control = this.closeSimpleButton;
		this.closeLayoutControlItem.Location = new System.Drawing.Point(356, 74);
		this.closeLayoutControlItem.MaxSize = new System.Drawing.Size(84, 26);
		this.closeLayoutControlItem.MinSize = new System.Drawing.Size(84, 26);
		this.closeLayoutControlItem.Name = "closeLayoutControlItem";
		this.closeLayoutControlItem.Size = new System.Drawing.Size(84, 26);
		this.closeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.closeLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.closeLayoutControlItem.TextVisible = false;
		this.labelControlLayoutControlItem.Control = this.labelControl;
		this.labelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.labelControlLayoutControlItem.MinSize = new System.Drawing.Size(385, 17);
		this.labelControlLayoutControlItem.Name = "labelControlLayoutControlItem";
		this.labelControlLayoutControlItem.Size = new System.Drawing.Size(440, 64);
		this.labelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.labelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.labelControlLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(460, 120);
		base.Controls.Add(this.nonCustomizableLayoutControl1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("BusinessGlossaryExportForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.MaximizeBox = false;
		base.Name = "BusinessGlossaryExportForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "HTML Plus";
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.closeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.labelControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
