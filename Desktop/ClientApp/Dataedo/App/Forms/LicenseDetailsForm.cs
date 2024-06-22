using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.API.Models;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Properties;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using DevExpress.Utils;
using DevExpress.XtraLayout;

namespace Dataedo.App.Forms;

public class LicenseDetailsForm : BaseXtraForm
{
	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private LicenseDetailsUserControl licenseDetailsUserControl;

	private LayoutControlItem layoutControlItem;

	public LicenseDetailsForm()
	{
		InitializeComponent();
		if (string.IsNullOrEmpty(StaticData.License?.PackageName))
		{
			Text = "License details";
		}
		else
		{
			Text = StaticData.License?.PackageName + " - license details";
		}
	}

	public void SetWindowLocation(Point point)
	{
		base.Location = point;
		ScreenHelper.Center(this);
	}

	public void SetParameters(AppLicense appLicense)
	{
		licenseDetailsUserControl.SetParameters(appLicense);
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Escape)
		{
			Close();
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	public static void ShowInParentForm(Form parent)
	{
		LicenseDetailsForm licenseDetailsForm = new LicenseDetailsForm();
		licenseDetailsForm.SetParameters(StaticData.License);
		licenseDetailsForm.StartPosition = FormStartPosition.CenterParent;
		licenseDetailsForm.ShowDialog(parent);
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
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.licenseDetailsUserControl = new Dataedo.App.UserControls.LicenseDetailsUserControl();
		this.layoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.licenseDetailsUserControl);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(445, 450);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(445, 450);
		this.Root.TextVisible = false;
		this.licenseDetailsUserControl.Location = new System.Drawing.Point(12, 12);
		this.licenseDetailsUserControl.Name = "licenseDetailsUserControl";
		this.licenseDetailsUserControl.Size = new System.Drawing.Size(421, 426);
		this.licenseDetailsUserControl.TabIndex = 4;
		this.layoutControlItem.Control = this.licenseDetailsUserControl;
		this.layoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem.Name = "layoutControlItem";
		this.layoutControlItem.Size = new System.Drawing.Size(425, 430);
		this.layoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(445, 450);
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_32;
		base.Name = "LicenseDetailsForm";
		this.Text = "License details";
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
