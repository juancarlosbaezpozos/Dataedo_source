using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class UpgradeDataClassificationControl : BaseUserControl
{
	private IContainer components;

	private LabelControl upgradeAccountLinkLabelControl;

	private NonCustomizableLayoutControl layoutControl;

	private LayoutControlGroup layoutControlGroup1;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem2;

	private EmptySpaceItem emptySpaceItem3;

	private LayoutControlItem firstlinkLayoutControlItem;

	private LabelControl mainTitleLabelControl;

	private LabelControl viewDocumentationLabelControl;

	private LayoutControlItem firstlinkLayoutControlItem1;

	private EmptySpaceItem emptySpaceItem5;

	private LabelControl firstPointLabelControl;

	private LabelControl thirdPointLabelControl;

	private LabelControl secondPointLabelControl;

	private EmptySpaceItem emptySpaceItem9;

	private EmptySpaceItem emptySpaceItem8;

	private EmptySpaceItem emptySpaceItem15;

	private EmptySpaceItem emptySpaceItem16;

	private Panel panel1;

	private HyperlinkLabelControl hyperlinkLabelControl1;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem7;

	private LayoutControlItem layoutControlItem5;

	private EmptySpaceItem emptySpaceItem11;

	private EmptySpaceItem emptySpaceItem13;

	private LayoutControlItem layoutControlItem6;

	private EmptySpaceItem emptySpaceItem10;

	private LayoutControlItem layoutControlItem4;

	private EmptySpaceItem emptySpaceItem12;

	private EmptySpaceItem emptySpaceItem6;

	private LayoutControlItem layoutControlItem8;

	private PictureBox pictureBox1;

	private EmptySpaceItem emptySpaceItem4;

	private LayoutControlItem layoutControlItem9;

	private LayoutControlItem layoutControlItem1;

	private EmptySpaceItem emptySpaceItem14;

	private EmptySpaceItem emptySpaceItem17;

	private EmptySpaceItem emptySpaceItem18;

	public UpgradeDataClassificationControl()
	{
		InitializeComponent();
		upgradeAccountLinkLabelControl.Appearance.Options.UseFont = (viewDocumentationLabelControl.Appearance.Options.UseFont = true);
		SetTheme();
	}

	private void SetTheme()
	{
		Color color2 = (BackColor = (layoutControl.BackColor = SkinsManager.CurrentSkin.ControlBackColor));
		NonCustomizableLayoutControl nonCustomizableLayoutControl = layoutControl;
		Color color3 = (mainTitleLabelControl.ForeColor = SkinsManager.CurrentSkin.ControlForeColor);
		color2 = (ForeColor = (nonCustomizableLayoutControl.ForeColor = color3));
		LabelControl labelControl = mainTitleLabelControl;
		LabelControl labelControl2 = firstPointLabelControl;
		LabelControl labelControl3 = secondPointLabelControl;
		Color color6 = (thirdPointLabelControl.ForeColor = SkinsManager.CurrentSkin.TextFieldForeColor);
		color3 = (labelControl3.ForeColor = color6);
		color2 = (labelControl.ForeColor = (labelControl2.ForeColor = color3));
	}

	private void upgradeAccountLinkLabelControl_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			Links.OpenLink(Links.ManageAccounts, FindForm());
		}
	}

	private void viewDocumentationLabelControl_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			Links.OpenLink(Links.DataClassificationDocumentation, FindForm());
		}
	}

	private void hyperlinkLabelControl1_Click(object sender, EventArgs e)
	{
		Links.OpenLink(Links.SalesContact, FindForm());
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
		this.upgradeAccountLinkLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.mainTitleLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.pictureBox1 = new System.Windows.Forms.PictureBox();
		this.hyperlinkLabelControl1 = new DevExpress.XtraEditors.HyperlinkLabelControl();
		this.panel1 = new System.Windows.Forms.Panel();
		this.firstPointLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.thirdPointLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.secondPointLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.viewDocumentationLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.firstlinkLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.firstlinkLayoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem7 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem11 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem13 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem10 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem12 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem9 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem15 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem16 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem14 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem17 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem18 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.firstlinkLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.firstlinkLayoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem15).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem14).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem17).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).BeginInit();
		base.SuspendLayout();
		this.upgradeAccountLinkLabelControl.Appearance.BackColor = System.Drawing.Color.FromArgb(58, 91, 167);
		this.upgradeAccountLinkLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 10f, System.Drawing.FontStyle.Bold);
		this.upgradeAccountLinkLabelControl.Appearance.ForeColor = System.Drawing.SystemColors.Control;
		this.upgradeAccountLinkLabelControl.Appearance.Options.UseBackColor = true;
		this.upgradeAccountLinkLabelControl.Appearance.Options.UseFont = true;
		this.upgradeAccountLinkLabelControl.Appearance.Options.UseForeColor = true;
		this.upgradeAccountLinkLabelControl.Appearance.Options.UseTextOptions = true;
		this.upgradeAccountLinkLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.upgradeAccountLinkLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.upgradeAccountLinkLabelControl.Location = new System.Drawing.Point(347, 464);
		this.upgradeAccountLinkLabelControl.MaximumSize = new System.Drawing.Size(150, 30);
		this.upgradeAccountLinkLabelControl.MinimumSize = new System.Drawing.Size(150, 30);
		this.upgradeAccountLinkLabelControl.Name = "upgradeAccountLinkLabelControl";
		this.upgradeAccountLinkLabelControl.Size = new System.Drawing.Size(150, 30);
		this.upgradeAccountLinkLabelControl.StyleController = this.layoutControl;
		this.upgradeAccountLinkLabelControl.TabIndex = 0;
		this.upgradeAccountLinkLabelControl.Text = "Change plan";
		this.upgradeAccountLinkLabelControl.MouseClick += new System.Windows.Forms.MouseEventHandler(upgradeAccountLinkLabelControl_MouseClick);
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl.Controls.Add(this.mainTitleLabelControl);
		this.layoutControl.Controls.Add(this.pictureBox1);
		this.layoutControl.Controls.Add(this.hyperlinkLabelControl1);
		this.layoutControl.Controls.Add(this.panel1);
		this.layoutControl.Controls.Add(this.firstPointLabelControl);
		this.layoutControl.Controls.Add(this.thirdPointLabelControl);
		this.layoutControl.Controls.Add(this.secondPointLabelControl);
		this.layoutControl.Controls.Add(this.upgradeAccountLinkLabelControl);
		this.layoutControl.Controls.Add(this.viewDocumentationLabelControl);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2272, 242, 250, 350);
		this.layoutControl.Root = this.layoutControlGroup1;
		this.layoutControl.Size = new System.Drawing.Size(1135, 632);
		this.layoutControl.TabIndex = 2;
		this.layoutControl.Text = "layoutControl1";
		this.mainTitleLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 16f, System.Drawing.FontStyle.Bold);
		this.mainTitleLabelControl.Appearance.ForeColor = System.Drawing.Color.Black;
		this.mainTitleLabelControl.Appearance.Options.UseFont = true;
		this.mainTitleLabelControl.Appearance.Options.UseForeColor = true;
		this.mainTitleLabelControl.Appearance.Options.UseTextOptions = true;
		this.mainTitleLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.mainTitleLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.mainTitleLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.mainTitleLabelControl.Location = new System.Drawing.Point(124, 47);
		this.mainTitleLabelControl.Name = "mainTitleLabelControl";
		this.mainTitleLabelControl.Size = new System.Drawing.Size(875, 50);
		this.mainTitleLabelControl.StyleController = this.layoutControl;
		this.mainTitleLabelControl.TabIndex = 7;
		this.mainTitleLabelControl.Text = "Find and tag sensitive data in your databases\r\n";
		this.pictureBox1.Image = Dataedo.App.Properties.Resources.data_Classification_Upgrade;
		this.pictureBox1.Location = new System.Drawing.Point(124, 126);
		this.pictureBox1.Name = "pictureBox1";
		this.pictureBox1.Size = new System.Drawing.Size(875, 189);
		this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.pictureBox1.TabIndex = 15;
		this.pictureBox1.TabStop = false;
		this.hyperlinkLabelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 10f);
		this.hyperlinkLabelControl1.Appearance.Options.UseFont = true;
		this.hyperlinkLabelControl1.Appearance.Options.UseTextOptions = true;
		this.hyperlinkLabelControl1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.hyperlinkLabelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.hyperlinkLabelControl1.Location = new System.Drawing.Point(330, 512);
		this.hyperlinkLabelControl1.Name = "hyperlinkLabelControl1";
		this.hyperlinkLabelControl1.Size = new System.Drawing.Size(381, 16);
		this.hyperlinkLabelControl1.StyleController = this.layoutControl;
		this.hyperlinkLabelControl1.TabIndex = 0;
		this.hyperlinkLabelControl1.Text = "Contact sales";
		this.hyperlinkLabelControl1.Click += new System.EventHandler(hyperlinkLabelControl1_Click);
		this.panel1.Location = new System.Drawing.Point(2, 542);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(1131, 60);
		this.panel1.TabIndex = 13;
		this.firstPointLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 10f);
		this.firstPointLabelControl.Appearance.Options.UseFont = true;
		this.firstPointLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
		this.firstPointLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
		this.firstPointLabelControl.ImageOptions.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
		this.firstPointLabelControl.ImageOptions.Image = Dataedo.App.Properties.Resources.check_16;
		this.firstPointLabelControl.IndentBetweenImageAndText = 20;
		this.firstPointLabelControl.Location = new System.Drawing.Point(330, 343);
		this.firstPointLabelControl.Name = "firstPointLabelControl";
		this.firstPointLabelControl.Size = new System.Drawing.Size(485, 20);
		this.firstPointLabelControl.StyleController = this.layoutControl;
		this.firstPointLabelControl.TabIndex = 12;
		this.firstPointLabelControl.Text = "Find sensitive data automatically with built in built in classifications (GDPR, PII)";
		this.thirdPointLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 10f);
		this.thirdPointLabelControl.Appearance.Options.UseFont = true;
		this.thirdPointLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
		this.thirdPointLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
		this.thirdPointLabelControl.ImageOptions.Image = Dataedo.App.Properties.Resources.check_16;
		this.thirdPointLabelControl.IndentBetweenImageAndText = 20;
		this.thirdPointLabelControl.Location = new System.Drawing.Point(330, 411);
		this.thirdPointLabelControl.Name = "thirdPointLabelControl";
		this.thirdPointLabelControl.Size = new System.Drawing.Size(407, 20);
		this.thirdPointLabelControl.StyleController = this.layoutControl;
		this.thirdPointLabelControl.TabIndex = 11;
		this.thirdPointLabelControl.Text = "Store and share classification within your data dictionary/catalog";
		this.secondPointLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 10f);
		this.secondPointLabelControl.Appearance.Options.UseFont = true;
		this.secondPointLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Horizontal;
		this.secondPointLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftCenter;
		this.secondPointLabelControl.ImageOptions.Image = Dataedo.App.Properties.Resources.check_16;
		this.secondPointLabelControl.IndentBetweenImageAndText = 20;
		this.secondPointLabelControl.Location = new System.Drawing.Point(330, 377);
		this.secondPointLabelControl.Name = "secondPointLabelControl";
		this.secondPointLabelControl.Size = new System.Drawing.Size(345, 20);
		this.secondPointLabelControl.StyleController = this.layoutControl;
		this.secondPointLabelControl.TabIndex = 10;
		this.secondPointLabelControl.Text = "Enable manual collaborative curation of classifications";
		this.viewDocumentationLabelControl.Appearance.BackColor = System.Drawing.Color.FromArgb(58, 91, 167);
		this.viewDocumentationLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 10f, System.Drawing.FontStyle.Bold);
		this.viewDocumentationLabelControl.Appearance.ForeColor = System.Drawing.SystemColors.Control;
		this.viewDocumentationLabelControl.Appearance.Options.UseBackColor = true;
		this.viewDocumentationLabelControl.Appearance.Options.UseFont = true;
		this.viewDocumentationLabelControl.Appearance.Options.UseForeColor = true;
		this.viewDocumentationLabelControl.Appearance.Options.UseTextOptions = true;
		this.viewDocumentationLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.viewDocumentationLabelControl.Cursor = System.Windows.Forms.Cursors.Hand;
		this.viewDocumentationLabelControl.Location = new System.Drawing.Point(539, 464);
		this.viewDocumentationLabelControl.MaximumSize = new System.Drawing.Size(150, 30);
		this.viewDocumentationLabelControl.MinimumSize = new System.Drawing.Size(150, 30);
		this.viewDocumentationLabelControl.Name = "viewDocumentationLabelControl";
		this.viewDocumentationLabelControl.Size = new System.Drawing.Size(150, 30);
		this.viewDocumentationLabelControl.StyleController = this.layoutControl;
		this.viewDocumentationLabelControl.TabIndex = 0;
		this.viewDocumentationLabelControl.Text = "View documentation";
		this.viewDocumentationLabelControl.MouseClick += new System.Windows.Forms.MouseEventHandler(viewDocumentationLabelControl_MouseClick);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[26]
		{
			this.emptySpaceItem1, this.emptySpaceItem2, this.emptySpaceItem3, this.emptySpaceItem4, this.firstlinkLayoutControlItem, this.firstlinkLayoutControlItem1, this.emptySpaceItem5, this.emptySpaceItem7, this.layoutControlItem5, this.emptySpaceItem11,
			this.emptySpaceItem13, this.layoutControlItem6, this.emptySpaceItem10, this.layoutControlItem4, this.emptySpaceItem12, this.emptySpaceItem9, this.emptySpaceItem15, this.emptySpaceItem16, this.layoutControlItem3, this.emptySpaceItem6,
			this.layoutControlItem9, this.layoutControlItem1, this.emptySpaceItem14, this.emptySpaceItem17, this.emptySpaceItem18, this.layoutControlItem8
		});
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(1135, 632);
		this.layoutControlGroup1.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 627);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 5);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(1, 5);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.emptySpaceItem1.Size = new System.Drawing.Size(1135, 5);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 0);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(0, 5);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(1, 5);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(1135, 5);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 443);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(328, 97);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(328, 97);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(328, 97);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(713, 443);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(0, 67);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(10, 67);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(422, 97);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.firstlinkLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.firstlinkLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.firstlinkLayoutControlItem.Control = this.upgradeAccountLinkLabelControl;
		this.firstlinkLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopCenter;
		this.firstlinkLayoutControlItem.Location = new System.Drawing.Point(328, 443);
		this.firstlinkLayoutControlItem.MaxSize = new System.Drawing.Size(188, 67);
		this.firstlinkLayoutControlItem.MinSize = new System.Drawing.Size(188, 67);
		this.firstlinkLayoutControlItem.Name = "firstlinkLayoutControlItem";
		this.firstlinkLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 11, 6);
		this.firstlinkLayoutControlItem.Size = new System.Drawing.Size(188, 67);
		this.firstlinkLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.firstlinkLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.firstlinkLayoutControlItem.TextVisible = false;
		this.firstlinkLayoutControlItem1.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.firstlinkLayoutControlItem1.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.firstlinkLayoutControlItem1.Control = this.viewDocumentationLabelControl;
		this.firstlinkLayoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.TopCenter;
		this.firstlinkLayoutControlItem1.CustomizationFormText = "firstlinkLayoutControlItem";
		this.firstlinkLayoutControlItem1.Location = new System.Drawing.Point(516, 443);
		this.firstlinkLayoutControlItem1.MaxSize = new System.Drawing.Size(197, 67);
		this.firstlinkLayoutControlItem1.MinSize = new System.Drawing.Size(197, 67);
		this.firstlinkLayoutControlItem1.Name = "firstlinkLayoutControlItem1";
		this.firstlinkLayoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 11, 6);
		this.firstlinkLayoutControlItem1.Size = new System.Drawing.Size(197, 67);
		this.firstlinkLayoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.firstlinkLayoutControlItem1.Text = "firstlinkLayoutControlItem";
		this.firstlinkLayoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.firstlinkLayoutControlItem1.TextVisible = false;
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.Location = new System.Drawing.Point(0, 604);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(1135, 10);
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem7.AllowHotTrack = false;
		this.emptySpaceItem7.Location = new System.Drawing.Point(0, 341);
		this.emptySpaceItem7.MaxSize = new System.Drawing.Size(328, 102);
		this.emptySpaceItem7.MinSize = new System.Drawing.Size(328, 102);
		this.emptySpaceItem7.Name = "emptySpaceItem7";
		this.emptySpaceItem7.OptionsPrint.AppearanceItem.BackColor = System.Drawing.Color.Transparent;
		this.emptySpaceItem7.OptionsPrint.AppearanceItem.Options.UseBackColor = true;
		this.emptySpaceItem7.Size = new System.Drawing.Size(328, 102);
		this.emptySpaceItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.Control = this.secondPointLabelControl;
		this.layoutControlItem5.Location = new System.Drawing.Point(328, 375);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(489, 24);
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.emptySpaceItem11.AllowHotTrack = false;
		this.emptySpaceItem11.Location = new System.Drawing.Point(328, 399);
		this.emptySpaceItem11.MaxSize = new System.Drawing.Size(374, 10);
		this.emptySpaceItem11.MinSize = new System.Drawing.Size(374, 10);
		this.emptySpaceItem11.Name = "emptySpaceItem11";
		this.emptySpaceItem11.Size = new System.Drawing.Size(489, 10);
		this.emptySpaceItem11.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem11.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem13.AllowHotTrack = false;
		this.emptySpaceItem13.Location = new System.Drawing.Point(328, 433);
		this.emptySpaceItem13.MaxSize = new System.Drawing.Size(374, 10);
		this.emptySpaceItem13.MinSize = new System.Drawing.Size(374, 10);
		this.emptySpaceItem13.Name = "emptySpaceItem13";
		this.emptySpaceItem13.Size = new System.Drawing.Size(489, 10);
		this.emptySpaceItem13.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem13.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.Control = this.thirdPointLabelControl;
		this.layoutControlItem6.Location = new System.Drawing.Point(328, 409);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.Size = new System.Drawing.Size(489, 24);
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.emptySpaceItem10.AllowHotTrack = false;
		this.emptySpaceItem10.Location = new System.Drawing.Point(328, 365);
		this.emptySpaceItem10.MaxSize = new System.Drawing.Size(374, 10);
		this.emptySpaceItem10.MinSize = new System.Drawing.Size(374, 10);
		this.emptySpaceItem10.Name = "emptySpaceItem10";
		this.emptySpaceItem10.Size = new System.Drawing.Size(489, 10);
		this.emptySpaceItem10.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem10.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.Control = this.firstPointLabelControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(328, 341);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(489, 24);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.emptySpaceItem12.AllowHotTrack = false;
		this.emptySpaceItem12.Location = new System.Drawing.Point(817, 341);
		this.emptySpaceItem12.MaxSize = new System.Drawing.Size(0, 102);
		this.emptySpaceItem12.MinSize = new System.Drawing.Size(104, 102);
		this.emptySpaceItem12.Name = "emptySpaceItem12";
		this.emptySpaceItem12.OptionsPrint.AppearanceItem.BackColor = System.Drawing.Color.Transparent;
		this.emptySpaceItem12.OptionsPrint.AppearanceItem.Options.UseBackColor = true;
		this.emptySpaceItem12.Size = new System.Drawing.Size(318, 102);
		this.emptySpaceItem12.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem12.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem9.AllowHotTrack = false;
		this.emptySpaceItem9.Location = new System.Drawing.Point(1001, 5);
		this.emptySpaceItem9.MinSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem9.Name = "emptySpaceItem9";
		this.emptySpaceItem9.Size = new System.Drawing.Size(134, 312);
		this.emptySpaceItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem9.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem15.AllowHotTrack = false;
		this.emptySpaceItem15.Location = new System.Drawing.Point(0, 5);
		this.emptySpaceItem15.MaxSize = new System.Drawing.Size(122, 312);
		this.emptySpaceItem15.MinSize = new System.Drawing.Size(122, 312);
		this.emptySpaceItem15.Name = "emptySpaceItem15";
		this.emptySpaceItem15.Size = new System.Drawing.Size(122, 312);
		this.emptySpaceItem15.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem15.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem16.AllowHotTrack = false;
		this.emptySpaceItem16.Location = new System.Drawing.Point(0, 614);
		this.emptySpaceItem16.Name = "emptySpaceItem16";
		this.emptySpaceItem16.Size = new System.Drawing.Size(1135, 13);
		this.emptySpaceItem16.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.Control = this.panel1;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 540);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(1135, 64);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem6.AllowHotTrack = false;
		this.emptySpaceItem6.Location = new System.Drawing.Point(0, 317);
		this.emptySpaceItem6.MaxSize = new System.Drawing.Size(1135, 24);
		this.emptySpaceItem6.MinSize = new System.Drawing.Size(1135, 24);
		this.emptySpaceItem6.Name = "emptySpaceItem6";
		this.emptySpaceItem6.Size = new System.Drawing.Size(1135, 24);
		this.emptySpaceItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem9.Control = this.pictureBox1;
		this.layoutControlItem9.Location = new System.Drawing.Point(122, 124);
		this.layoutControlItem9.MaxSize = new System.Drawing.Size(879, 193);
		this.layoutControlItem9.MinSize = new System.Drawing.Size(879, 193);
		this.layoutControlItem9.Name = "layoutControlItem9";
		this.layoutControlItem9.Size = new System.Drawing.Size(879, 193);
		this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem9.TextVisible = false;
		this.layoutControlItem1.Control = this.mainTitleLabelControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(122, 45);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(879, 54);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(879, 54);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(879, 54);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.emptySpaceItem14.AllowHotTrack = false;
		this.emptySpaceItem14.Location = new System.Drawing.Point(122, 99);
		this.emptySpaceItem14.MaxSize = new System.Drawing.Size(879, 25);
		this.emptySpaceItem14.MinSize = new System.Drawing.Size(879, 25);
		this.emptySpaceItem14.Name = "emptySpaceItem14";
		this.emptySpaceItem14.Size = new System.Drawing.Size(879, 25);
		this.emptySpaceItem14.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem14.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem17.AllowHotTrack = false;
		this.emptySpaceItem17.Location = new System.Drawing.Point(122, 5);
		this.emptySpaceItem17.MaxSize = new System.Drawing.Size(879, 40);
		this.emptySpaceItem17.MinSize = new System.Drawing.Size(879, 40);
		this.emptySpaceItem17.Name = "emptySpaceItem17";
		this.emptySpaceItem17.Size = new System.Drawing.Size(879, 40);
		this.emptySpaceItem17.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem17.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem18.AllowHotTrack = false;
		this.emptySpaceItem18.Location = new System.Drawing.Point(328, 530);
		this.emptySpaceItem18.Name = "emptySpaceItem18";
		this.emptySpaceItem18.Size = new System.Drawing.Size(385, 10);
		this.emptySpaceItem18.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.Control = this.hyperlinkLabelControl1;
		this.layoutControlItem8.Location = new System.Drawing.Point(328, 510);
		this.layoutControlItem8.MaxSize = new System.Drawing.Size(385, 20);
		this.layoutControlItem8.MinSize = new System.Drawing.Size(385, 20);
		this.layoutControlItem8.Name = "layoutControlItem8";
		this.layoutControlItem8.Size = new System.Drawing.Size(385, 20);
		this.layoutControlItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.TextVisible = false;
		this.emptySpaceItem8.AllowHotTrack = false;
		this.emptySpaceItem8.Location = new System.Drawing.Point(0, 0);
		this.emptySpaceItem8.Name = "emptySpaceItem8";
		this.emptySpaceItem8.Size = new System.Drawing.Size(0, 0);
		this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		base.Controls.Add(this.layoutControl);
		this.ForeColor = System.Drawing.Color.Black;
		base.Name = "UpgradeDataClassificationControl";
		base.Size = new System.Drawing.Size(1135, 632);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pictureBox1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.firstlinkLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.firstlinkLayoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem15).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem14).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem17).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).EndInit();
		base.ResumeLayout(false);
	}
}
