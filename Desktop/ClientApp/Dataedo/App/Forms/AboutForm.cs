using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class AboutForm : BaseXtraForm
{
	private readonly string CHANGE_KEY = "Change key";

	private readonly string ENTER_KEY = "Enter key";

	private readonly string linksSeparator = "    ";

	private IContainer components;

	private PictureEdit ddLogoPictureEdit;

	private NonCustomizableLayoutControl layoutControl1;

	private LabelControl copyrightLabelControl;

	private LabelControl versionLabel;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem versionLayoutControlItem;

	private LayoutControlItem layoutControlItem3;

	private SimpleButton closeButton;

	private LayoutControlItem layoutControlItem1;

	private EmptySpaceItem emptySpaceItem4;

	private EmptySpaceItem emptySpaceItem6;

	private LabelControl versionTitleLabelControl;

	private LayoutControlItem versionTitleLayoutControlItem;

	private LabelControl licenseLabelControl;

	private EmptySpaceItem emptySpaceItem2;

	private LayoutControlItem layoutControlItem4;

	private LabelControl productNameLabelControl;

	private LayoutControlItem layoutControlItem5;

	public AboutForm()
	{
		InitializeComponent();
		CompleteProgramInfoPanel();
		closeButton.Select();
	}

	public void CompleteProgramInfoPanel()
	{
		versionLabel.Text = ProgramVersion.VersionWithBitVersion ?? "";
	}

	private void AboutForm_Load(object sender, EventArgs e)
	{
		Text = $"About Dataedo {ProgramVersion.Major}";
		copyrightLabelControl.Text = ProgramVersion.Copyright;
		licenseLabelControl.Text = "Dataedo and Dataedo logo are properties of Dataedo sp. z o.o.<br>All other trademarks, logos and copyrights are the property of their respective owners.<br><href>License agreement</href>";
	}

	private void closeButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if (keyData == Keys.Escape)
			{
				Close();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, this);
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void ddLogoPictureEdit_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			Links.OpenLink(Links.Dataedo, this);
		}
	}

	private void LicenseLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		string text = Path.Combine(Environment.CurrentDirectory, "license.txt");
		if (File.Exists(text))
		{
			Process.Start(text);
		}
		else
		{
			GeneralMessageBoxesHandling.Show("License file missing from installation folder" + Environment.NewLine + "To read the license please reinstall Dataedo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this);
		}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.AboutForm));
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.productNameLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.licenseLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.versionTitleLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.closeButton = new DevExpress.XtraEditors.SimpleButton();
		this.copyrightLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.versionLabel = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.versionLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.versionTitleLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.ddLogoPictureEdit = new DevExpress.XtraEditors.PictureEdit();
		this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.versionLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.versionTitleLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ddLogoPictureEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl1.Controls.Add(this.productNameLabelControl);
		this.layoutControl1.Controls.Add(this.licenseLabelControl);
		this.layoutControl1.Controls.Add(this.versionTitleLabelControl);
		this.layoutControl1.Controls.Add(this.closeButton);
		this.layoutControl1.Controls.Add(this.copyrightLabelControl);
		this.layoutControl1.Controls.Add(this.versionLabel);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 150);
		this.layoutControl1.MinimumSize = new System.Drawing.Size(0, 140);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3599, 185, 660, 649);
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(453, 225);
		this.layoutControl1.TabIndex = 1;
		this.layoutControl1.Text = "layoutControl1";
		this.productNameLabelControl.AllowHtmlString = true;
		this.productNameLabelControl.Location = new System.Drawing.Point(13, 13);
		this.productNameLabelControl.Name = "productNameLabelControl";
		this.productNameLabelControl.Size = new System.Drawing.Size(427, 13);
		this.productNameLabelControl.StyleController = this.layoutControl1;
		this.productNameLabelControl.TabIndex = 15;
		this.productNameLabelControl.Text = "<b>Dataedo Desktop</b>";
		this.licenseLabelControl.AllowHtmlString = true;
		this.licenseLabelControl.Appearance.Options.UseTextOptions = true;
		this.licenseLabelControl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.licenseLabelControl.Location = new System.Drawing.Point(13, 102);
		this.licenseLabelControl.Name = "licenseLabelControl";
		this.licenseLabelControl.Size = new System.Drawing.Size(427, 47);
		this.licenseLabelControl.StyleController = this.layoutControl1;
		this.licenseLabelControl.TabIndex = 13;
		this.licenseLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(LicenseLabelControl_HyperlinkClick);
		this.versionTitleLabelControl.Location = new System.Drawing.Point(13, 30);
		this.versionTitleLabelControl.Name = "versionTitleLabelControl";
		this.versionTitleLabelControl.Size = new System.Drawing.Size(39, 13);
		this.versionTitleLabelControl.StyleController = this.layoutControl1;
		this.versionTitleLabelControl.TabIndex = 12;
		this.versionTitleLabelControl.Text = "Version:";
		this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.closeButton.Location = new System.Drawing.Point(353, 190);
		this.closeButton.MaximumSize = new System.Drawing.Size(80, 22);
		this.closeButton.MinimumSize = new System.Drawing.Size(80, 22);
		this.closeButton.Name = "closeButton";
		this.closeButton.Size = new System.Drawing.Size(80, 22);
		this.closeButton.StyleController = this.layoutControl1;
		this.closeButton.TabIndex = 7;
		this.closeButton.Text = "Close";
		this.closeButton.Click += new System.EventHandler(closeButton_Click);
		this.copyrightLabelControl.Location = new System.Drawing.Point(13, 47);
		this.copyrightLabelControl.Name = "copyrightLabelControl";
		this.copyrightLabelControl.Size = new System.Drawing.Size(427, 13);
		this.copyrightLabelControl.StyleController = this.layoutControl1;
		this.copyrightLabelControl.TabIndex = 6;
		this.copyrightLabelControl.Text = "Copyright";
		this.copyrightLabelControl.UseMnemonic = false;
		this.versionLabel.Location = new System.Drawing.Point(56, 30);
		this.versionLabel.Name = "versionLabel";
		this.versionLabel.Size = new System.Drawing.Size(384, 13);
		this.versionLabel.StyleController = this.layoutControl1;
		this.versionLabel.TabIndex = 4;
		this.versionLabel.Text = " ";
		this.versionLabel.UseMnemonic = false;
		this.layoutControlGroup1.CustomizationFormText = "Root";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[8] { this.versionLayoutControlItem, this.layoutControlItem3, this.layoutControlItem1, this.emptySpaceItem2, this.emptySpaceItem4, this.versionTitleLayoutControlItem, this.layoutControlItem4, this.layoutControlItem5 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(11, 11, 11, 11);
		this.layoutControlGroup1.Size = new System.Drawing.Size(453, 225);
		this.layoutControlGroup1.TextVisible = false;
		this.versionLayoutControlItem.Control = this.versionLabel;
		this.versionLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.MiddleLeft;
		this.versionLayoutControlItem.CustomizationFormText = "Version:";
		this.versionLayoutControlItem.Location = new System.Drawing.Point(43, 17);
		this.versionLayoutControlItem.MinSize = new System.Drawing.Size(7, 17);
		this.versionLayoutControlItem.Name = "versionLayoutControlItem";
		this.versionLayoutControlItem.Size = new System.Drawing.Size(388, 17);
		this.versionLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.versionLayoutControlItem.Text = "Version:";
		this.versionLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.versionLayoutControlItem.TextVisible = false;
		this.layoutControlItem3.Control = this.copyrightLabelControl;
		this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 34);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(0, 17);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(424, 17);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(431, 17);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.layoutControlItem1.Control = this.closeButton;
		this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
		this.layoutControlItem1.Location = new System.Drawing.Point(340, 177);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(91, 26);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(91, 26);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(91, 26);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 140);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(104, 10);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(431, 37);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.CustomizationFormText = "emptySpaceItem4";
		this.emptySpaceItem4.Location = new System.Drawing.Point(0, 177);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(340, 26);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(1, 26);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(340, 26);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.versionTitleLayoutControlItem.Control = this.versionTitleLabelControl;
		this.versionTitleLayoutControlItem.Location = new System.Drawing.Point(0, 17);
		this.versionTitleLayoutControlItem.MaxSize = new System.Drawing.Size(0, 17);
		this.versionTitleLayoutControlItem.MinSize = new System.Drawing.Size(43, 17);
		this.versionTitleLayoutControlItem.Name = "versionTitleLayoutControlItem";
		this.versionTitleLayoutControlItem.Size = new System.Drawing.Size(43, 17);
		this.versionTitleLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.versionTitleLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.versionTitleLayoutControlItem.TextVisible = false;
		this.layoutControlItem4.Control = this.licenseLabelControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 51);
		this.layoutControlItem4.MaxSize = new System.Drawing.Size(431, 89);
		this.layoutControlItem4.MinSize = new System.Drawing.Size(431, 89);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 40, 2);
		this.layoutControlItem4.Size = new System.Drawing.Size(431, 89);
		this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem5.Control = this.productNameLabelControl;
		this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem5.MaxSize = new System.Drawing.Size(0, 17);
		this.layoutControlItem5.MinSize = new System.Drawing.Size(102, 17);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(431, 17);
		this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.ddLogoPictureEdit.Cursor = System.Windows.Forms.Cursors.Hand;
		this.ddLogoPictureEdit.Dock = System.Windows.Forms.DockStyle.Top;
		this.ddLogoPictureEdit.EditValue = Dataedo.App.Properties.Resources.login;
		this.ddLogoPictureEdit.Location = new System.Drawing.Point(0, 0);
		this.ddLogoPictureEdit.MaximumSize = new System.Drawing.Size(453, 150);
		this.ddLogoPictureEdit.MinimumSize = new System.Drawing.Size(453, 150);
		this.ddLogoPictureEdit.Name = "ddLogoPictureEdit";
		this.ddLogoPictureEdit.Properties.AllowFocused = false;
		this.ddLogoPictureEdit.Properties.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.ddLogoPictureEdit.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.ddLogoPictureEdit.Properties.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.ddLogoPictureEdit.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(59, 59, 59);
		this.ddLogoPictureEdit.Properties.Appearance.Options.UseBackColor = true;
		this.ddLogoPictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.ddLogoPictureEdit.Properties.Padding = new System.Windows.Forms.Padding(11);
		this.ddLogoPictureEdit.Properties.ShowMenu = false;
		this.ddLogoPictureEdit.Size = new System.Drawing.Size(453, 150);
		this.ddLogoPictureEdit.TabIndex = 0;
		this.ddLogoPictureEdit.MouseClick += new System.Windows.Forms.MouseEventHandler(ddLogoPictureEdit_MouseClick);
		this.emptySpaceItem6.AllowHotTrack = false;
		this.emptySpaceItem6.CustomizationFormText = "emptySpaceItem5";
		this.emptySpaceItem6.Location = new System.Drawing.Point(0, 0);
		this.emptySpaceItem6.MaxSize = new System.Drawing.Size(12, 26);
		this.emptySpaceItem6.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem6.Name = "emptySpaceItem5";
		this.emptySpaceItem6.Size = new System.Drawing.Size(12, 140);
		this.emptySpaceItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.closeButton;
		base.ClientSize = new System.Drawing.Size(453, 375);
		base.Controls.Add(this.layoutControl1);
		base.Controls.Add(this.ddLogoPictureEdit);
		base.FormBorderEffect = DevExpress.XtraEditors.FormBorderEffect.Shadow;
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("AboutForm.IconOptions.Icon");
		base.Name = "AboutForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		base.Load += new System.EventHandler(AboutForm_Load);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.versionLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.versionTitleLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ddLogoPictureEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).EndInit();
		base.ResumeLayout(false);
	}
}
