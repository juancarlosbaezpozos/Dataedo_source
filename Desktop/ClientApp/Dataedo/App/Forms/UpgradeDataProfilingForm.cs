using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Properties;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;

namespace Dataedo.App.Forms;

public class UpgradeDataProfilingForm : BaseXtraForm
{
	private IContainer components;

	private UpgradeDataProfilingControl upgradeDataProfilingControl1;

	public UpgradeDataProfilingForm()
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
		this.upgradeDataProfilingControl1 = new Dataedo.App.UserControls.UpgradeDataProfilingControl();
		base.SuspendLayout();
		this.upgradeDataProfilingControl1.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.upgradeDataProfilingControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.upgradeDataProfilingControl1.ForeColor = System.Drawing.Color.Black;
		this.upgradeDataProfilingControl1.Location = new System.Drawing.Point(0, 0);
		this.upgradeDataProfilingControl1.Name = "upgradeDataClassificationControl1";
		this.upgradeDataProfilingControl1.Size = new System.Drawing.Size(1139, 624);
		this.upgradeDataProfilingControl1.TabIndex = 0;
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1139, 624);
		base.Controls.Add(this.upgradeDataProfilingControl1);
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon_16;
		base.Name = "UpgradeDataProfilingForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Data Profiling";
		base.ResumeLayout(false);
	}
}
