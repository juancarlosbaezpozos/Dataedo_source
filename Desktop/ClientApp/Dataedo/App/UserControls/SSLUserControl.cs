using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Enums;
using Dataedo.App.Tools;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.UserControls;

public class SSLUserControl : BaseUserControl
{
	private SSLFileHelper fileHelper;

	private SSLSettingsType sslSettingsType;

	private IContainer components;

	private LayoutControlGroup layoutControlGroup1;

	private CustomTextEdit sslKeyFileCustomTextEdit;

	private LayoutControlItem sslKeyFileLayoutControlItem;

	private CustomTextEdit sslCaFileCustomTextEdit;

	private CustomTextEdit sslCertFileCustomTextEdit;

	private LayoutControlItem sslCertFileLayoutControlItem;

	private LayoutControlItem sslCAFileLayoutControlItem;

	private SimpleButton sslKeyFileSimpleButton;

	private LayoutControlItem sslKeyFileSimpleButtonLayoutControlItem;

	private SimpleButton sslCertFileSimpleButton;

	private LayoutControlItem sslCertFileSimpleButtonLayoutControlItem;

	private SimpleButton sslCaFileSimpleButton;

	private LayoutControlItem sslCaFileSimpleButtonLayoutControlItem;

	private CustomTextEdit cipherCustomTextEdit;

	private LayoutControlItem sslCipherLayoutControlItem;

	private NonCustomizableLayoutControl layoutControl1;

	private CustomTextEdit sslCertPasswordCustomTextEdit;

	private LayoutControlItem sslCertPasswordLayoutControlItem;

	public string KeyPath { get; private set; }

	public string CertPath { get; private set; }

	public string CAPath { get; private set; }

	public string Cipher { get; private set; }

	public string CertPassword { get; private set; }

	public SSLUserControl()
	{
		InitializeComponent();
		fileHelper = new SSLFileHelper();
		sslCipherLayoutControlItem.Visibility = LayoutVisibility.Never;
		sslCertPasswordLayoutControlItem.Visibility = LayoutVisibility.Never;
	}

	public void Configure(SSLSettingsType sslSettingsType)
	{
		this.sslSettingsType = sslSettingsType;
		if (this.sslSettingsType == SSLSettingsType.ClientPfxWithPassword)
		{
			LayoutControlItem layoutControlItem = sslCAFileLayoutControlItem;
			LayoutControlItem layoutControlItem2 = sslCaFileSimpleButtonLayoutControlItem;
			LayoutControlItem layoutControlItem3 = sslKeyFileLayoutControlItem;
			LayoutControlItem layoutControlItem4 = sslKeyFileSimpleButtonLayoutControlItem;
			LayoutVisibility layoutVisibility2 = (sslCipherLayoutControlItem.Visibility = LayoutVisibility.Never);
			LayoutVisibility layoutVisibility4 = (layoutControlItem4.Visibility = layoutVisibility2);
			LayoutVisibility layoutVisibility6 = (layoutControlItem3.Visibility = layoutVisibility4);
			LayoutVisibility layoutVisibility9 = (layoutControlItem.Visibility = (layoutControlItem2.Visibility = layoutVisibility6));
			sslCertFileLayoutControlItem.Visibility = LayoutVisibility.Always;
			sslCertFileLayoutControlItem.Text = "Client Certificate (.pfx):";
			sslCertPasswordLayoutControlItem.Visibility = LayoutVisibility.Always;
			sslCertPasswordCustomTextEdit.Properties.UseSystemPasswordChar = true;
		}
	}

	public void SetParameters(SSLSettings settings)
	{
		sslKeyFileCustomTextEdit.EditValue = settings?.KeyPath;
		sslCertFileCustomTextEdit.EditValue = settings?.CertPath;
		sslCaFileCustomTextEdit.EditValue = settings?.CAPath;
		cipherCustomTextEdit.EditValue = settings?.Cipher;
		sslCertPasswordCustomTextEdit.EditValue = SSLSettings.DecryptCertPassword(settings?.CertPassword);
	}

	private void sslKeyFileCustomTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		KeyPath = sslKeyFileCustomTextEdit.Text;
	}

	private void sslCertFileCustomTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		CertPath = sslCertFileCustomTextEdit.Text;
	}

	private void sslCaFileCustomTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		CAPath = sslCaFileCustomTextEdit.Text;
	}

	private void cipherCustomTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		Cipher = cipherCustomTextEdit.Text;
	}

	private void sslCertPasswordCustomTextEditCustomTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		CertPassword = sslCertPasswordCustomTextEdit.Text;
	}

	private void sslKeyFileSimpleButton_Click(object sender, EventArgs e)
	{
		sslKeyFileCustomTextEdit.Text = fileHelper.GetFilePathWithPrefix();
	}

	private void sslCertFileSimpleButton_Click(object sender, EventArgs e)
	{
		if (sslSettingsType == SSLSettingsType.ClientPfxWithPassword)
		{
			sslCertFileCustomTextEdit.Text = fileHelper.GetFilePathWithPrefix("pfx");
		}
		else
		{
			sslCertFileCustomTextEdit.Text = fileHelper.GetFilePathWithPrefix();
		}
	}

	private void sslCaFileSimpleButton_Click(object sender, EventArgs e)
	{
		sslCaFileCustomTextEdit.Text = fileHelper.GetFilePathWithPrefix();
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
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.sslCaFileSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.sslCertFileSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.sslKeyFileSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.sslCaFileCustomTextEdit = new Dataedo.App.UserControls.CustomTextEdit();
		this.sslCertFileCustomTextEdit = new Dataedo.App.UserControls.CustomTextEdit();
		this.sslKeyFileCustomTextEdit = new Dataedo.App.UserControls.CustomTextEdit();
		this.cipherCustomTextEdit = new Dataedo.App.UserControls.CustomTextEdit();
		this.sslCertPasswordCustomTextEdit = new Dataedo.App.UserControls.CustomTextEdit();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.sslKeyFileLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sslCertFileLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sslCAFileLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sslKeyFileSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sslCertFileSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sslCaFileSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sslCipherLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sslCertPasswordLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.sslCaFileCustomTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sslCertFileCustomTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sslKeyFileCustomTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cipherCustomTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sslCertPasswordCustomTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sslKeyFileLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sslCertFileLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sslCAFileLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sslKeyFileSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sslCertFileSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sslCaFileSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sslCipherLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sslCertPasswordLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl1.Controls.Add(this.sslCaFileSimpleButton);
		this.layoutControl1.Controls.Add(this.sslCertFileSimpleButton);
		this.layoutControl1.Controls.Add(this.sslKeyFileSimpleButton);
		this.layoutControl1.Controls.Add(this.sslCaFileCustomTextEdit);
		this.layoutControl1.Controls.Add(this.sslCertFileCustomTextEdit);
		this.layoutControl1.Controls.Add(this.sslKeyFileCustomTextEdit);
		this.layoutControl1.Controls.Add(this.cipherCustomTextEdit);
		this.layoutControl1.Controls.Add(this.sslCertPasswordCustomTextEdit);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(394, 153);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.sslCaFileSimpleButton.Location = new System.Drawing.Point(294, 64);
		this.sslCaFileSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.sslCaFileSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.sslCaFileSimpleButton.Name = "sslCaFileSimpleButton";
		this.sslCaFileSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.sslCaFileSimpleButton.StyleController = this.layoutControl1;
		this.sslCaFileSimpleButton.TabIndex = 13;
		this.sslCaFileSimpleButton.Text = "Browse";
		this.sslCaFileSimpleButton.Click += new System.EventHandler(sslCaFileSimpleButton_Click);
		this.sslCertFileSimpleButton.Location = new System.Drawing.Point(294, 38);
		this.sslCertFileSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.sslCertFileSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.sslCertFileSimpleButton.Name = "sslCertFileSimpleButton";
		this.sslCertFileSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.sslCertFileSimpleButton.StyleController = this.layoutControl1;
		this.sslCertFileSimpleButton.TabIndex = 12;
		this.sslCertFileSimpleButton.Text = "Browse";
		this.sslCertFileSimpleButton.Click += new System.EventHandler(sslCertFileSimpleButton_Click);
		this.sslKeyFileSimpleButton.Location = new System.Drawing.Point(294, 12);
		this.sslKeyFileSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.sslKeyFileSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.sslKeyFileSimpleButton.Name = "sslKeyFileSimpleButton";
		this.sslKeyFileSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.sslKeyFileSimpleButton.StyleController = this.layoutControl1;
		this.sslKeyFileSimpleButton.TabIndex = 11;
		this.sslKeyFileSimpleButton.Text = "Browse";
		this.sslKeyFileSimpleButton.Click += new System.EventHandler(sslKeyFileSimpleButton_Click);
		this.sslCaFileCustomTextEdit.Location = new System.Drawing.Point(114, 64);
		this.sslCaFileCustomTextEdit.Name = "sslCaFileCustomTextEdit";
		this.sslCaFileCustomTextEdit.Size = new System.Drawing.Size(176, 20);
		this.sslCaFileCustomTextEdit.StyleController = this.layoutControl1;
		this.sslCaFileCustomTextEdit.TabIndex = 10;
		this.sslCaFileCustomTextEdit.EditValueChanged += new System.EventHandler(sslCaFileCustomTextEdit_EditValueChanged);
		this.sslCertFileCustomTextEdit.Location = new System.Drawing.Point(114, 38);
		this.sslCertFileCustomTextEdit.Name = "sslCertFileCustomTextEdit";
		this.sslCertFileCustomTextEdit.Size = new System.Drawing.Size(176, 20);
		this.sslCertFileCustomTextEdit.StyleController = this.layoutControl1;
		this.sslCertFileCustomTextEdit.TabIndex = 9;
		this.sslCertFileCustomTextEdit.EditValueChanged += new System.EventHandler(sslCertFileCustomTextEdit_EditValueChanged);
		this.sslKeyFileCustomTextEdit.Location = new System.Drawing.Point(114, 13);
		this.sslKeyFileCustomTextEdit.Name = "sslKeyFileCustomTextEdit";
		this.sslKeyFileCustomTextEdit.Size = new System.Drawing.Size(176, 20);
		this.sslKeyFileCustomTextEdit.StyleController = this.layoutControl1;
		this.sslKeyFileCustomTextEdit.TabIndex = 8;
		this.sslKeyFileCustomTextEdit.EditValueChanged += new System.EventHandler(sslKeyFileCustomTextEdit_EditValueChanged);
		this.cipherCustomTextEdit.Location = new System.Drawing.Point(114, 90);
		this.cipherCustomTextEdit.Name = "cipherCustomTextEdit";
		this.cipherCustomTextEdit.Size = new System.Drawing.Size(265, 20);
		this.cipherCustomTextEdit.StyleController = this.layoutControl1;
		this.cipherCustomTextEdit.TabIndex = 10;
		this.cipherCustomTextEdit.EditValueChanged += new System.EventHandler(cipherCustomTextEdit_EditValueChanged);
		this.sslCertPasswordCustomTextEdit.Location = new System.Drawing.Point(114, 116);
		this.sslCertPasswordCustomTextEdit.Name = "sslCertPasswordCustomTextEdit";
		this.sslCertPasswordCustomTextEdit.Size = new System.Drawing.Size(265, 20);
		this.sslCertPasswordCustomTextEdit.StyleController = this.layoutControl1;
		this.sslCertPasswordCustomTextEdit.TabIndex = 10;
		this.sslCertPasswordCustomTextEdit.EditValueChanged += new System.EventHandler(sslCertPasswordCustomTextEditCustomTextEdit_EditValueChanged);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[8] { this.sslKeyFileLayoutControlItem, this.sslCertFileLayoutControlItem, this.sslCAFileLayoutControlItem, this.sslKeyFileSimpleButtonLayoutControlItem, this.sslCertFileSimpleButtonLayoutControlItem, this.sslCaFileSimpleButtonLayoutControlItem, this.sslCipherLayoutControlItem, this.sslCertPasswordLayoutControlItem });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 13, 10, 10);
		this.layoutControlGroup1.Size = new System.Drawing.Size(394, 153);
		this.layoutControlGroup1.TextVisible = false;
		this.sslKeyFileLayoutControlItem.Control = this.sslKeyFileCustomTextEdit;
		this.sslKeyFileLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.sslKeyFileLayoutControlItem.MaxSize = new System.Drawing.Size(0, 26);
		this.sslKeyFileLayoutControlItem.MinSize = new System.Drawing.Size(126, 26);
		this.sslKeyFileLayoutControlItem.Name = "sslKeyFileLayoutControlItem";
		this.sslKeyFileLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 3, 2);
		this.sslKeyFileLayoutControlItem.Size = new System.Drawing.Size(282, 26);
		this.sslKeyFileLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sslKeyFileLayoutControlItem.Text = "Client Private Key:";
		this.sslKeyFileLayoutControlItem.TextSize = new System.Drawing.Size(99, 13);
		this.sslCertFileLayoutControlItem.Control = this.sslCertFileCustomTextEdit;
		this.sslCertFileLayoutControlItem.Location = new System.Drawing.Point(0, 26);
		this.sslCertFileLayoutControlItem.MaxSize = new System.Drawing.Size(0, 26);
		this.sslCertFileLayoutControlItem.MinSize = new System.Drawing.Size(126, 26);
		this.sslCertFileLayoutControlItem.Name = "sslCertFileLayoutControlItem";
		this.sslCertFileLayoutControlItem.Size = new System.Drawing.Size(282, 26);
		this.sslCertFileLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sslCertFileLayoutControlItem.Text = "Client Certificate:";
		this.sslCertFileLayoutControlItem.TextSize = new System.Drawing.Size(99, 13);
		this.sslCAFileLayoutControlItem.Control = this.sslCaFileCustomTextEdit;
		this.sslCAFileLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopCenter;
		this.sslCAFileLayoutControlItem.Location = new System.Drawing.Point(0, 52);
		this.sslCAFileLayoutControlItem.MaxSize = new System.Drawing.Size(0, 26);
		this.sslCAFileLayoutControlItem.MinSize = new System.Drawing.Size(126, 26);
		this.sslCAFileLayoutControlItem.Name = "sslCAFileLayoutControlItem";
		this.sslCAFileLayoutControlItem.Size = new System.Drawing.Size(282, 26);
		this.sslCAFileLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sslCAFileLayoutControlItem.Text = "CA Certificate:";
		this.sslCAFileLayoutControlItem.TextSize = new System.Drawing.Size(99, 13);
		this.sslKeyFileSimpleButtonLayoutControlItem.Control = this.sslKeyFileSimpleButton;
		this.sslKeyFileSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(282, 0);
		this.sslKeyFileSimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(89, 26);
		this.sslKeyFileSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(89, 26);
		this.sslKeyFileSimpleButtonLayoutControlItem.Name = "sslKeyFileSimpleButtonLayoutControlItem";
		this.sslKeyFileSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(89, 26);
		this.sslKeyFileSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sslKeyFileSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.sslKeyFileSimpleButtonLayoutControlItem.TextVisible = false;
		this.sslCertFileSimpleButtonLayoutControlItem.Control = this.sslCertFileSimpleButton;
		this.sslCertFileSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(282, 26);
		this.sslCertFileSimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(89, 26);
		this.sslCertFileSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(89, 26);
		this.sslCertFileSimpleButtonLayoutControlItem.Name = "sslCertFileSimpleButtonLayoutControlItem";
		this.sslCertFileSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(89, 26);
		this.sslCertFileSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sslCertFileSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.sslCertFileSimpleButtonLayoutControlItem.TextVisible = false;
		this.sslCaFileSimpleButtonLayoutControlItem.Control = this.sslCaFileSimpleButton;
		this.sslCaFileSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(282, 52);
		this.sslCaFileSimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(89, 26);
		this.sslCaFileSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(89, 26);
		this.sslCaFileSimpleButtonLayoutControlItem.Name = "sslCaFileSimpleButtonLayoutControlItem";
		this.sslCaFileSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(89, 26);
		this.sslCaFileSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sslCaFileSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.sslCaFileSimpleButtonLayoutControlItem.TextVisible = false;
		this.sslCipherLayoutControlItem.Control = this.cipherCustomTextEdit;
		this.sslCipherLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopCenter;
		this.sslCipherLayoutControlItem.CustomizationFormText = "SSL CA File:";
		this.sslCipherLayoutControlItem.Location = new System.Drawing.Point(0, 78);
		this.sslCipherLayoutControlItem.MaxSize = new System.Drawing.Size(0, 26);
		this.sslCipherLayoutControlItem.MinSize = new System.Drawing.Size(126, 26);
		this.sslCipherLayoutControlItem.Name = "sslCipherLayoutControlItem";
		this.sslCipherLayoutControlItem.Size = new System.Drawing.Size(371, 26);
		this.sslCipherLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sslCipherLayoutControlItem.Text = "Cipher:";
		this.sslCipherLayoutControlItem.TextSize = new System.Drawing.Size(99, 13);
		this.sslCertPasswordLayoutControlItem.Control = this.sslCertPasswordCustomTextEdit;
		this.sslCertPasswordLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopCenter;
		this.sslCertPasswordLayoutControlItem.CustomizationFormText = "SSL CA File:";
		this.sslCertPasswordLayoutControlItem.Location = new System.Drawing.Point(0, 104);
		this.sslCertPasswordLayoutControlItem.MaxSize = new System.Drawing.Size(0, 26);
		this.sslCertPasswordLayoutControlItem.MinSize = new System.Drawing.Size(126, 26);
		this.sslCertPasswordLayoutControlItem.Name = "sslCertPasswordLayoutControlItem";
		this.sslCertPasswordLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sslCertPasswordLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.sslCertPasswordLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sslCertPasswordLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.sslCertPasswordLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.sslCertPasswordLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.sslCertPasswordLayoutControlItem.Size = new System.Drawing.Size(371, 29);
		this.sslCertPasswordLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sslCertPasswordLayoutControlItem.Text = "Certificate Password:";
		this.sslCertPasswordLayoutControlItem.TextSize = new System.Drawing.Size(99, 13);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.layoutControl1);
		base.Name = "SSLUserControl";
		base.Size = new System.Drawing.Size(394, 153);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.sslCaFileCustomTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sslCertFileCustomTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sslKeyFileCustomTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cipherCustomTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sslCertPasswordCustomTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sslKeyFileLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sslCertFileLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sslCAFileLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sslKeyFileSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sslCertFileSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sslCaFileSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sslCipherLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sslCertPasswordLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
