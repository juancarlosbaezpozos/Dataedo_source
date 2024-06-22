using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.LoginFormTools.Tools.Common;
using Dataedo.App.Properties;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.LoginFormTools.UserControls.Subcontrols;

public class NoLicensesUserControl : XtraUserControl
{
	private string email;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private LabelControl headerLabelControl;

	private LayoutControlItem layoutControlItem1;

	private LabelControl headerLabelControl1;

	private LayoutControlItem layoutControlItem3;

	private LabelControl secondRowLabelControl;

	private EmptySpaceItem emptySpaceItem5;

	private EmptySpaceItem emptySpaceItem6;

	private LayoutControlItem layoutControlItem2;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem3;

	private PictureBox pictureBox1;

	private LayoutControlItem layoutControlItem4;

	public NoLicensesUserControl()
	{
		InitializeComponent();
	}

	public void SetEmail(string email)
	{
		this.email = email;
	}

	private void HeaderLabelControl1_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		OpeningLinks.OpenLinkWithOptionalEmail(e, email, '&');
	}

	private void secondRowLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		OpeningLinks.OpenLink(e);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.LoginFormTools.UserControls.Subcontrols.NoLicensesUserControl));
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.headerLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.headerLabelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.secondRowLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.pictureBox1);
		this.mainLayoutControl.Controls.Add(this.secondRowLabelControl);
		this.mainLayoutControl.Controls.Add(this.headerLabelControl);
		this.mainLayoutControl.Controls.Add(this.headerLabelControl1);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2674, 456, 650, 400);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(620, 398);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.headerLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f);
		this.headerLabelControl.Appearance.Options.UseFont = true;
		this.headerLabelControl.Location = new System.Drawing.Point(2, 42);
		this.headerLabelControl.Name = "headerLabelControl";
		this.headerLabelControl.Size = new System.Drawing.Size(328, 16);
		this.headerLabelControl.StyleController = this.mainLayoutControl;
		this.headerLabelControl.TabIndex = 4;
		this.headerLabelControl.Text = "We couldn't find any active license assigned to this email.";
		this.headerLabelControl1.AllowHtmlString = true;
		this.headerLabelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f);
		this.headerLabelControl1.Appearance.Options.UseFont = true;
		this.headerLabelControl1.Appearance.Options.UseTextOptions = true;
		this.headerLabelControl1.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.headerLabelControl1.Location = new System.Drawing.Point(2, 178);
		this.headerLabelControl1.Name = "headerLabelControl1";
		this.headerLabelControl1.Size = new System.Drawing.Size(239, 32);
		this.headerLabelControl1.StyleController = this.mainLayoutControl;
		this.headerLabelControl1.TabIndex = 4;
		this.headerLabelControl1.Text = resources.GetString("headerLabelControl1.Text");
		this.headerLabelControl1.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(HeaderLabelControl1_HyperlinkClick);
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[8] { this.emptySpaceItem1, this.layoutControlItem1, this.emptySpaceItem5, this.layoutControlItem2, this.emptySpaceItem6, this.layoutControlItem3, this.emptySpaceItem3, this.layoutControlItem4 });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(620, 398);
		this.mainLayoutControlGroup.TextVisible = false;
		this.layoutControlItem1.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.layoutControlItem1.Control = this.headerLabelControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 40);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(332, 38);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(332, 38);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 20);
		this.layoutControlItem1.Size = new System.Drawing.Size(332, 38);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem3.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.layoutControlItem3.Control = this.headerLabelControl1;
		this.layoutControlItem3.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.layoutControlItem3.CustomizationFormText = "layoutControlItem1";
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 176);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(332, 36);
		this.layoutControlItem3.Text = "layoutControlItem1";
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.Location = new System.Drawing.Point(0, 78);
		this.emptySpaceItem5.MinSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(332, 30);
		this.emptySpaceItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem6.AllowHotTrack = false;
		this.emptySpaceItem6.Location = new System.Drawing.Point(0, 144);
		this.emptySpaceItem6.MinSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem6.Name = "emptySpaceItem6";
		this.emptySpaceItem6.Size = new System.Drawing.Size(332, 32);
		this.emptySpaceItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		this.secondRowLabelControl.AllowHtmlString = true;
		this.secondRowLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f);
		this.secondRowLabelControl.Appearance.Options.UseFont = true;
		this.secondRowLabelControl.Appearance.Options.UseTextOptions = true;
		this.secondRowLabelControl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.secondRowLabelControl.Location = new System.Drawing.Point(2, 110);
		this.secondRowLabelControl.Name = "secondRowLabelControl";
		this.secondRowLabelControl.Size = new System.Drawing.Size(303, 32);
		this.secondRowLabelControl.StyleController = this.mainLayoutControl;
		this.secondRowLabelControl.TabIndex = 5;
		this.secondRowLabelControl.Text = "Not sure if your license should be there? Read about \r\n<href=https://dataedo.com/docs/access-to-licenses?utm_source=App&utm_medium=App>how to access an existing license</href>";
		this.secondRowLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(secondRowLabelControl_HyperlinkClick);
		this.layoutControlItem2.Control = this.secondRowLabelControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 108);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(332, 36);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 212);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(332, 186);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 0);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(332, 0);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(332, 24);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(332, 40);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pictureBox1.Image = Dataedo.App.Properties.Resources.no_license;
		this.pictureBox1.Location = new System.Drawing.Point(334, 2);
		this.pictureBox1.Margin = new System.Windows.Forms.Padding(0);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(284, 394);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pictureBox1.TabIndex = 8;
		this.pictureBox1.TabStop = false;
		this.layoutControlItem4.Control = this.pictureBox1;
		this.layoutControlItem4.Location = new System.Drawing.Point(332, 0);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(288, 398);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Name = "NoLicensesUserControl";
		base.Size = new System.Drawing.Size(620, 398);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		base.ResumeLayout(false);
	}
}
