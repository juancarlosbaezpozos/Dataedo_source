using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Tools.Exceptions;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class DetachWebCatalogForm : BaseXtraForm
{
	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private LayoutControlGroup Root;

	private LabelControl messageLabel;

	private LayoutControlItem messageLayoutControlItem;

	private SimpleButton okSimpleButton;

	private LayoutControlItem layoutControlItem1;

	private EmptySpaceItem emptySpaceItem1;

	private SimpleButton cancelButton;

	private LayoutControlItem layoutControlItem2;

	public DetachWebCatalogForm()
	{
		InitializeComponent();
		messageLayoutControlItem.Image = SystemIcons.Warning.ToBitmap();
	}

	private void okSimpleButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Yes;
		Close();
	}

	private void DetachWebCatalogForm_Load(object sender, EventArgs e)
	{
		BringToFront();
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if (keyData == Keys.Escape)
			{
				base.DialogResult = DialogResult.Cancel;
				Close();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, this);
		}
		return base.ProcessCmdKey(ref msg, keyData);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.DetachWebCatalogForm));
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
		this.okSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.messageLabel = new DevExpress.XtraEditors.LabelControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.messageLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.messageLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl1.Controls.Add(this.cancelButton);
		this.nonCustomizableLayoutControl1.Controls.Add(this.okSimpleButton);
		this.nonCustomizableLayoutControl1.Controls.Add(this.messageLabel);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.Root = this.Root;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(408, 112);
		this.nonCustomizableLayoutControl1.TabIndex = 0;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelButton.Location = new System.Drawing.Point(306, 73);
		this.cancelButton.Name = "cancelButton";
		this.cancelButton.Size = new System.Drawing.Size(87, 24);
		this.cancelButton.StyleController = this.nonCustomizableLayoutControl1;
		this.cancelButton.TabIndex = 6;
		this.cancelButton.Text = "Cancel";
		this.okSimpleButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.okSimpleButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.okSimpleButton.Location = new System.Drawing.Point(147, 73);
		this.okSimpleButton.Name = "okSimpleButton";
		this.okSimpleButton.Size = new System.Drawing.Size(149, 24);
		this.okSimpleButton.StyleController = this.nonCustomizableLayoutControl1;
		this.okSimpleButton.TabIndex = 5;
		this.okSimpleButton.Text = "Yes, detach Web Catalog";
		this.okSimpleButton.Click += new System.EventHandler(okSimpleButton_Click);
		this.messageLabel.AllowHtmlString = true;
		this.messageLabel.Appearance.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
		this.messageLabel.Appearance.Options.UseImageAlign = true;
		this.messageLabel.Appearance.Options.UseTextOptions = true;
		this.messageLabel.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
		this.messageLabel.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.messageLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.messageLabel.Location = new System.Drawing.Point(57, 14);
		this.messageLabel.Margin = new System.Windows.Forms.Padding(0);
		this.messageLabel.Name = "messageLabel";
		this.messageLabel.Padding = new System.Windows.Forms.Padding(2);
		this.messageLabel.Size = new System.Drawing.Size(337, 50);
		this.messageLabel.StyleController = this.nonCustomizableLayoutControl1;
		this.messageLabel.TabIndex = 4;
		this.messageLabel.Text = "You are about to detach the Web Catalog from the Repository.  \r\nThat is a <b>permanent action, and can’t be reverted!</b>";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.messageLayoutControlItem, this.layoutControlItem1, this.emptySpaceItem1, this.layoutControlItem2 });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(408, 112);
		this.Root.TextVisible = false;
		this.messageLayoutControlItem.Control = this.messageLabel;
		this.messageLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.messageLayoutControlItem.CustomizationFormText = " ";
		this.messageLayoutControlItem.ImageOptions.Alignment = System.Drawing.ContentAlignment.TopLeft;
		this.messageLayoutControlItem.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("messageLayoutControlItem.ImageOptions.Image");
		this.messageLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.messageLayoutControlItem.MinSize = new System.Drawing.Size(58, 36);
		this.messageLayoutControlItem.Name = "messageLayoutControlItem";
		this.messageLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.messageLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.messageLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.messageLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.messageLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.messageLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.messageLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 4, 4, 4);
		this.messageLayoutControlItem.Size = new System.Drawing.Size(388, 58);
		this.messageLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.messageLayoutControlItem.Text = " ";
		this.messageLayoutControlItem.TextSize = new System.Drawing.Size(40, 45);
		this.layoutControlItem1.Control = this.okSimpleButton;
		this.layoutControlItem1.Location = new System.Drawing.Point(132, 58);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(159, 34);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(159, 34);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
		this.layoutControlItem1.Size = new System.Drawing.Size(159, 34);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 58);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(132, 34);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.Control = this.cancelButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(291, 58);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(97, 34);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(97, 34);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
		this.layoutControlItem2.Size = new System.Drawing.Size(97, 34);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(408, 112);
		base.Controls.Add(this.nonCustomizableLayoutControl1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.IconOptions.ShowIcon = false;
		base.Name = "DetachWebCatalogForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Heads up!";
		base.Load += new System.EventHandler(DetachWebCatalogForm_Load);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.messageLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		base.ResumeLayout(false);
	}
}
