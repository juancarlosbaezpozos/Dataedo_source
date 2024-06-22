using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;

namespace Dataedo.App.Forms;

public class SSLForm : BaseXtraForm
{
	private readonly SSLSettingsType sslSettingsType;

	private IContainer components;

	private LayoutControlGroup layoutControlGroup1;

	private SSLUserControl sslUserControl;

	private LayoutControlItem layoutControlItem1;

	private NonCustomizableLayoutControl layoutControl1;

	private SimpleButton saveSimpleButton;

	private SimpleButton cancelSimpleButton;

	private LayoutControlItem layoutControlItem2;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem2;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem3;

	public SSLSettings SSL { get; private set; }

	public SSLForm()
	{
		InitializeComponent();
	}

	public SSLForm(SSLSettings settings, SSLSettingsType sslSettingsType)
		: this()
	{
		SSL = settings;
		this.sslSettingsType = sslSettingsType;
		sslUserControl.SetParameters(SSL);
		sslUserControl.Configure(this.sslSettingsType);
	}

	private void saveSimpleButton_Click(object sender, EventArgs e)
	{
		if (SSL == null)
		{
			SSL = new SSLSettings();
		}
		SSL.KeyPath = sslUserControl.KeyPath;
		SSL.CertPath = sslUserControl.CertPath;
		SSL.CAPath = sslUserControl.CAPath;
		SSL.Cipher = sslUserControl.Cipher;
		SSL.CertPassword = sslUserControl.CertPassword;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.SSLForm));
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.saveSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.sslUserControl = new Dataedo.App.UserControls.SSLUserControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl1.Controls.Add(this.saveSimpleButton);
		this.layoutControl1.Controls.Add(this.cancelSimpleButton);
		this.layoutControl1.Controls.Add(this.sslUserControl);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(551, 154);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.saveSimpleButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.saveSimpleButton.Location = new System.Drawing.Point(335, 120);
		this.saveSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.saveSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.saveSimpleButton.Name = "saveSimpleButton";
		this.saveSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.saveSimpleButton.StyleController = this.layoutControl1;
		this.saveSimpleButton.TabIndex = 6;
		this.saveSimpleButton.Text = "Save";
		this.saveSimpleButton.Click += new System.EventHandler(saveSimpleButton_Click);
		this.cancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelSimpleButton.Location = new System.Drawing.Point(439, 120);
		this.cancelSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.StyleController = this.layoutControl1;
		this.cancelSimpleButton.TabIndex = 5;
		this.cancelSimpleButton.Text = "Cancel";
		this.sslUserControl.BackColor = System.Drawing.Color.Transparent;
		this.sslUserControl.Location = new System.Drawing.Point(12, 12);
		this.sslUserControl.Name = "sslUserControl";
		this.sslUserControl.Size = new System.Drawing.Size(527, 104);
		this.sslUserControl.TabIndex = 4;
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.layoutControlItem1, this.layoutControlItem2, this.emptySpaceItem1, this.emptySpaceItem2, this.layoutControlItem3, this.emptySpaceItem3 });
		this.layoutControlGroup1.Name = "layoutControlGroup1";
		this.layoutControlGroup1.Size = new System.Drawing.Size(551, 154);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.Control = this.sslUserControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(531, 108);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem2.Control = this.cancelSimpleButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(427, 108);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(89, 26);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 108);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(323, 26);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(516, 108);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(15, 26);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(15, 26);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(15, 26);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.Control = this.saveSimpleButton;
		this.layoutControlItem3.Location = new System.Drawing.Point(323, 108);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(89, 26);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(412, 108);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(15, 26);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(15, 26);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(15, 26);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(551, 154);
		base.Controls.Add(this.layoutControl1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("SSLForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.MaximizeBox = false;
		base.Name = "SSLForm";
		this.Text = "SSL settings";
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		base.ResumeLayout(false);
	}
}
