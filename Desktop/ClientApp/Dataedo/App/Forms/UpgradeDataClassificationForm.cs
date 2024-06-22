using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;

namespace Dataedo.App.Forms;

public class UpgradeDataClassificationForm : BaseXtraForm
{
	private IContainer components;

	private UpgradeDataClassificationControl upgradeDataClassificationControl1;

	public UpgradeDataClassificationForm()
	{
		InitializeComponent();
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
		this.upgradeDataClassificationControl1 = new Dataedo.App.UserControls.UpgradeDataClassificationControl();
		base.SuspendLayout();
		this.upgradeDataClassificationControl1.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.upgradeDataClassificationControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.upgradeDataClassificationControl1.ForeColor = System.Drawing.Color.Black;
		this.upgradeDataClassificationControl1.Location = new System.Drawing.Point(0, 0);
		this.upgradeDataClassificationControl1.Name = "upgradeDataClassificationControl1";
		this.upgradeDataClassificationControl1.Size = new System.Drawing.Size(1139, 624);
		this.upgradeDataClassificationControl1.TabIndex = 0;
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1139, 624);
		base.Controls.Add(this.upgradeDataClassificationControl1);
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_16;
		base.Name = "UpgradeDataClassificationForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Data Discovery & Classification";
		base.ResumeLayout(false);
	}
}
