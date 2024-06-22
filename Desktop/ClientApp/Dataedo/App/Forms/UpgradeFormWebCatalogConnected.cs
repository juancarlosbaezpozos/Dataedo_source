using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.UI;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class UpgradeFormWebCatalogConnected : BaseXtraForm
{
	private string database;

	private IContainer components;

	private NonCustomizableLayoutControl buttonsLayoutControl;

	private PanelControl messagePanelControl;

	private NonCustomizableLayoutControl messageLayoutControl;

	private LabelControl messageLabel;

	private LayoutControlGroup messageLayoutControlGroup;

	private LayoutControlItem messageLayoutControlItem;

	private SimpleButton okSimpleButton;

	private LayoutControlGroup buttonsLayoutControlGroup;

	private LayoutControlItem buttonsLayoutControlItem3;

	private LayoutControlItem layoutControlItem1;

	private EmptySpaceItem buttonsEmptySpaceItem;

	private SimpleButton detachWebCatalogButton;

	private LayoutControlItem layoutControlItem2;

	public UpgradeFormWebCatalogConnected(string database)
	{
		Application.EnableVisualStyles();
		Font = SystemFonts.MessageBoxFont;
		ForeColor = SystemColors.WindowText;
		this.database = database;
		InitializeComponent();
		string text = "Repository " + this.database + " is connected to Dataedo Web." + Environment.NewLine + "To upgrade it, use Dataedo Web or contact your administrator." + Environment.NewLine + Environment.NewLine + "If you don’t use Web Catalog, you can detach it from the repository.";
		messageLabel.Text = text;
		messageLayoutControlItem.Image = SystemIcons.Warning.ToBitmap();
		okSimpleButton.Select();
		messageLabel.ForeColor = SkinColors.ControlForeColorFromSystemColors;
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

	private void okSimpleButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.Cancel;
		Close();
	}

	private void UpgradeForm_Load(object sender, EventArgs e)
	{
		BringToFront();
	}

	private void detachWebCatalogButton_Click(object sender, EventArgs e)
	{
		Hide();
		if (new DetachWebCatalogForm().ShowDialog(this) == DialogResult.Yes)
		{
			base.DialogResult = DialogResult.Yes;
		}
		else
		{
			base.DialogResult = DialogResult.Cancel;
		}
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
		this.buttonsLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.detachWebCatalogButton = new DevExpress.XtraEditors.SimpleButton();
		this.messagePanelControl = new DevExpress.XtraEditors.PanelControl();
		this.messageLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.messageLabel = new DevExpress.XtraEditors.LabelControl();
		this.messageLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.messageLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.okSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.buttonsLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.buttonsLayoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.buttonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.buttonsLayoutControl).BeginInit();
		this.buttonsLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.messagePanelControl).BeginInit();
		this.messagePanelControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.messageLayoutControl).BeginInit();
		this.messageLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.messageLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.messageLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsLayoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		base.SuspendLayout();
		this.buttonsLayoutControl.AllowCustomization = false;
		this.buttonsLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.buttonsLayoutControl.Controls.Add(this.detachWebCatalogButton);
		this.buttonsLayoutControl.Controls.Add(this.messagePanelControl);
		this.buttonsLayoutControl.Controls.Add(this.okSimpleButton);
		this.buttonsLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.buttonsLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.buttonsLayoutControl.Name = "buttonsLayoutControl";
		this.buttonsLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2310, 292, 820, 522);
		this.buttonsLayoutControl.Root = this.buttonsLayoutControlGroup;
		this.buttonsLayoutControl.Size = new System.Drawing.Size(453, 118);
		this.buttonsLayoutControl.TabIndex = 5;
		this.buttonsLayoutControl.Text = "layoutControl2";
		this.detachWebCatalogButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.detachWebCatalogButton.Location = new System.Drawing.Point(217, 88);
		this.detachWebCatalogButton.Name = "detachWebCatalogButton";
		this.detachWebCatalogButton.Size = new System.Drawing.Size(141, 25);
		this.detachWebCatalogButton.StyleController = this.buttonsLayoutControl;
		this.detachWebCatalogButton.TabIndex = 4;
		this.detachWebCatalogButton.Text = "Detach Web Catalog";
		this.detachWebCatalogButton.Click += new System.EventHandler(detachWebCatalogButton_Click);
		this.messagePanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.messagePanelControl.Controls.Add(this.messageLayoutControl);
		this.messagePanelControl.Location = new System.Drawing.Point(2, 2);
		this.messagePanelControl.Name = "messagePanelControl";
		this.messagePanelControl.Size = new System.Drawing.Size(449, 79);
		this.messagePanelControl.TabIndex = 3;
		this.messageLayoutControl.AllowCustomization = false;
		this.messageLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.messageLayoutControl.Controls.Add(this.messageLabel);
		this.messageLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.messageLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.messageLayoutControl.Name = "messageLayoutControl";
		this.messageLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2264, 217, 250, 350);
		this.messageLayoutControl.Root = this.messageLayoutControlGroup;
		this.messageLayoutControl.Size = new System.Drawing.Size(449, 79);
		this.messageLayoutControl.TabIndex = 2;
		this.messageLayoutControl.Text = "layoutControl1";
		this.messageLabel.AllowHtmlString = true;
		this.messageLabel.Appearance.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
		this.messageLabel.Appearance.Options.UseImageAlign = true;
		this.messageLabel.Appearance.Options.UseTextOptions = true;
		this.messageLabel.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
		this.messageLabel.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.messageLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.messageLabel.Location = new System.Drawing.Point(47, 4);
		this.messageLabel.Margin = new System.Windows.Forms.Padding(0);
		this.messageLabel.Name = "messageLabel";
		this.messageLabel.Padding = new System.Windows.Forms.Padding(2);
		this.messageLabel.Size = new System.Drawing.Size(398, 71);
		this.messageLabel.StyleController = this.messageLayoutControl;
		this.messageLabel.TabIndex = 4;
		this.messageLabel.Text = "labelControl1";
		this.messageLayoutControlGroup.CustomizationFormText = "Root";
		this.messageLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.messageLayoutControlGroup.GroupBordersVisible = false;
		this.messageLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.messageLayoutControlItem });
		this.messageLayoutControlGroup.Name = "Root";
		this.messageLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.messageLayoutControlGroup.Size = new System.Drawing.Size(449, 79);
		this.messageLayoutControlGroup.TextVisible = false;
		this.messageLayoutControlItem.Control = this.messageLabel;
		this.messageLayoutControlItem.CustomizationFormText = " ";
		this.messageLayoutControlItem.ImageOptions.Alignment = System.Drawing.ContentAlignment.TopLeft;
		this.messageLayoutControlItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_top_green_32;
		this.messageLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.messageLayoutControlItem.MinSize = new System.Drawing.Size(58, 36);
		this.messageLayoutControlItem.Name = "messageLayoutControlItem";
		this.messageLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(4, 4, 4, 4);
		this.messageLayoutControlItem.Size = new System.Drawing.Size(449, 79);
		this.messageLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.messageLayoutControlItem.Text = " ";
		this.messageLayoutControlItem.TextSize = new System.Drawing.Size(40, 45);
		this.okSimpleButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.okSimpleButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.okSimpleButton.Location = new System.Drawing.Point(368, 88);
		this.okSimpleButton.Name = "okSimpleButton";
		this.okSimpleButton.Size = new System.Drawing.Size(80, 25);
		this.okSimpleButton.StyleController = this.buttonsLayoutControl;
		this.okSimpleButton.TabIndex = 1;
		this.okSimpleButton.Text = "OK";
		this.okSimpleButton.Click += new System.EventHandler(okSimpleButton_Click);
		this.buttonsLayoutControlGroup.CustomizationFormText = "Root";
		this.buttonsLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.buttonsLayoutControlGroup.GroupBordersVisible = false;
		this.buttonsLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.buttonsLayoutControlItem3, this.layoutControlItem1, this.buttonsEmptySpaceItem, this.layoutControlItem2 });
		this.buttonsLayoutControlGroup.Name = "Root";
		this.buttonsLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.buttonsLayoutControlGroup.Size = new System.Drawing.Size(453, 118);
		this.buttonsLayoutControlGroup.TextVisible = false;
		this.buttonsLayoutControlItem3.Control = this.okSimpleButton;
		this.buttonsLayoutControlItem3.CustomizationFormText = "buttonsLayoutControlItem3";
		this.buttonsLayoutControlItem3.Location = new System.Drawing.Point(363, 83);
		this.buttonsLayoutControlItem3.MaxSize = new System.Drawing.Size(90, 35);
		this.buttonsLayoutControlItem3.MinSize = new System.Drawing.Size(90, 35);
		this.buttonsLayoutControlItem3.Name = "buttonsLayoutControlItem3";
		this.buttonsLayoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
		this.buttonsLayoutControlItem3.Size = new System.Drawing.Size(90, 35);
		this.buttonsLayoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.buttonsLayoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.buttonsLayoutControlItem3.TextVisible = false;
		this.layoutControlItem1.Control = this.messagePanelControl;
		this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(104, 24);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(453, 83);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.buttonsEmptySpaceItem.AllowHotTrack = false;
		this.buttonsEmptySpaceItem.CustomizationFormText = "buttonsEmptySpaceItem";
		this.buttonsEmptySpaceItem.Location = new System.Drawing.Point(0, 83);
		this.buttonsEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 35);
		this.buttonsEmptySpaceItem.MinSize = new System.Drawing.Size(10, 35);
		this.buttonsEmptySpaceItem.Name = "buttonsEmptySpaceItem";
		this.buttonsEmptySpaceItem.Size = new System.Drawing.Size(212, 35);
		this.buttonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.buttonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.Control = this.detachWebCatalogButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(212, 83);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(151, 35);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(151, 35);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 5, 5, 5);
		this.layoutControlItem2.Size = new System.Drawing.Size(151, 35);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
		base.ClientSize = new System.Drawing.Size(453, 118);
		base.Controls.Add(this.buttonsLayoutControl);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.IconOptions.ShowIcon = false;
		base.KeyPreview = true;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "UpgradeFormWebCatalogConnected";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Upgrade";
		base.Load += new System.EventHandler(UpgradeForm_Load);
		((System.ComponentModel.ISupportInitialize)this.buttonsLayoutControl).EndInit();
		this.buttonsLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.messagePanelControl).EndInit();
		this.messagePanelControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.messageLayoutControl).EndInit();
		this.messageLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.messageLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.messageLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsLayoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		base.ResumeLayout(false);
	}
}
