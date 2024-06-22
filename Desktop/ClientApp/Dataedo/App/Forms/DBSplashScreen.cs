using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.UI;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.Forms;

public class DBSplashScreen : SplashScreen
{
	private const int WS_EX_TOOLWINDOW = 128;

	private IContainer components;

	private MarqueeProgressBarControl marqueeProgressBarControl1;

	private LabelControl copyrightLabelControl;

	private LabelControl labelControl2;

	private PictureEdit logoPictureEdit;

	private LabelControl versionLabel;

	private LabelControl productNameLabelControl;

	protected override CreateParams CreateParams
	{
		get
		{
			CreateParams obj = base.CreateParams;
			obj.ExStyle &= -129;
			return obj;
		}
	}

	public DBSplashScreen()
	{
		InitializeComponent();
		versionLabel.Text = "Version " + ProgramVersion.VersionWithBitVersion;
		copyrightLabelControl.Text = ProgramVersion.Copyright;
		BackColor = Dataedo.App.Tools.UI.SkinColors.ControlColorFromSystemColorsStored;
		labelControl2.ForeColor = Dataedo.App.Tools.UI.SkinColors.ControlForeColorFromSystemColorsStored;
		versionLabel.ForeColor = Dataedo.App.Tools.UI.SkinColors.ControlForeColorFromSystemColorsStored;
		copyrightLabelControl.ForeColor = Dataedo.App.Tools.UI.SkinColors.ControlForeColorFromSystemColorsStored;
		productNameLabelControl.ForeColor = Dataedo.App.Tools.UI.SkinColors.ControlForeColorFromSystemColorsStored;
	}

	public override void ProcessCommand(Enum cmd, object arg)
	{
		base.ProcessCommand(cmd, arg);
	}

	protected override void DrawBackground(PaintEventArgs e)
	{
	}

	protected override void DrawContent(GraphicsCache graphicsCache, Skin skin)
	{
		DrawCustomBackground(graphicsCache);
	}

	protected override void DrawTopElement(GraphicsCache graphicsCache, Skin skin)
	{
		DrawCustomBackground(graphicsCache);
	}

	private void DrawCustomBackground(GraphicsCache graphicsCache)
	{
		graphicsCache.DrawRectangle(new Pen(BackColor), base.ClientRectangle);
	}

	private void DBSplashScreen_Shown(object sender, EventArgs e)
	{
		ScreenHelper.CenterInCurrentScreen(this);
	}

	private void LogoPictureEdit_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			Links.OpenLink(Links.Dataedo, this);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.DBSplashScreen));
		this.marqueeProgressBarControl1 = new DevExpress.XtraEditors.MarqueeProgressBarControl();
		this.copyrightLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.logoPictureEdit = new DevExpress.XtraEditors.PictureEdit();
		this.versionLabel = new DevExpress.XtraEditors.LabelControl();
		this.productNameLabelControl = new DevExpress.XtraEditors.LabelControl();
		((System.ComponentModel.ISupportInitialize)this.marqueeProgressBarControl1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEdit.Properties).BeginInit();
		base.SuspendLayout();
		this.marqueeProgressBarControl1.EditValue = 0;
		this.marqueeProgressBarControl1.Location = new System.Drawing.Point(23, 231);
		this.marqueeProgressBarControl1.Name = "marqueeProgressBarControl1";
		this.marqueeProgressBarControl1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.marqueeProgressBarControl1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.marqueeProgressBarControl1.Properties.EndColor = System.Drawing.Color.FromArgb(70, 121, 198);
		this.marqueeProgressBarControl1.Properties.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.marqueeProgressBarControl1.Properties.LookAndFeel.UseDefaultLookAndFeel = false;
		this.marqueeProgressBarControl1.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.marqueeProgressBarControl1.Properties.StartColor = System.Drawing.Color.FromArgb(70, 121, 198);
		this.marqueeProgressBarControl1.Size = new System.Drawing.Size(404, 12);
		this.marqueeProgressBarControl1.TabIndex = 5;
		this.copyrightLabelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.copyrightLabelControl.Location = new System.Drawing.Point(23, 305);
		this.copyrightLabelControl.Name = "copyrightLabelControl";
		this.copyrightLabelControl.Size = new System.Drawing.Size(47, 13);
		this.copyrightLabelControl.TabIndex = 6;
		this.copyrightLabelControl.Text = "Copyright";
		this.copyrightLabelControl.UseMnemonic = false;
		this.labelControl2.Location = new System.Drawing.Point(23, 206);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(50, 13);
		this.labelControl2.TabIndex = 7;
		this.labelControl2.Text = "Starting...";
		this.logoPictureEdit.Cursor = System.Windows.Forms.Cursors.Hand;
		this.logoPictureEdit.Dock = System.Windows.Forms.DockStyle.Top;
		this.logoPictureEdit.EditValue = Dataedo.App.Properties.Resources.splash;
		this.logoPictureEdit.Location = new System.Drawing.Point(0, 0);
		this.logoPictureEdit.Name = "logoPictureEdit";
		this.logoPictureEdit.Properties.AllowFocused = false;
		this.logoPictureEdit.Properties.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.logoPictureEdit.Properties.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.logoPictureEdit.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(59, 59, 59);
		this.logoPictureEdit.Properties.Appearance.Options.UseBackColor = true;
		this.logoPictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.logoPictureEdit.Properties.ShowMenu = false;
		this.logoPictureEdit.Size = new System.Drawing.Size(450, 189);
		this.logoPictureEdit.TabIndex = 9;
		this.logoPictureEdit.MouseClick += new System.Windows.Forms.MouseEventHandler(LogoPictureEdit_MouseClick);
		this.versionLabel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.versionLabel.Location = new System.Drawing.Point(23, 267);
		this.versionLabel.Name = "versionLabel";
		this.versionLabel.Size = new System.Drawing.Size(35, 13);
		this.versionLabel.TabIndex = 10;
		this.versionLabel.Text = "Version";
		this.versionLabel.UseMnemonic = false;
		this.productNameLabelControl.AllowHtmlString = true;
		this.productNameLabelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.productNameLabelControl.Location = new System.Drawing.Point(23, 248);
		this.productNameLabelControl.Name = "productNameLabelControl";
		this.productNameLabelControl.Size = new System.Drawing.Size(98, 13);
		this.productNameLabelControl.TabIndex = 11;
		this.productNameLabelControl.Text = "<b>Dataedo Desktop</b>";
		this.productNameLabelControl.UseMnemonic = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(450, 330);
		base.Controls.Add(this.productNameLabelControl);
		base.Controls.Add(this.versionLabel);
		base.Controls.Add(this.logoPictureEdit);
		base.Controls.Add(this.labelControl2);
		base.Controls.Add(this.copyrightLabelControl);
		base.Controls.Add(this.marqueeProgressBarControl1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "DBSplashScreen";
		base.ShowInTaskbar = true;
		this.Text = "Dataedo";
		base.Shown += new System.EventHandler(DBSplashScreen_Shown);
		((System.ComponentModel.ISupportInitialize)this.marqueeProgressBarControl1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.logoPictureEdit.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
