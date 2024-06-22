using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Tools.UI;
using DevExpress.XtraEditors;

namespace Dataedo.App.UserControls;

public class GrayLabelControl : LabelControl
{
	public GrayLabelControl()
	{
		InitializeComponent();
		SetColors();
	}

	private void SetColors()
	{
		BackColor = SkinsManager.CurrentSkin.DDLExampleBackColor;
		ForeColor = SkinsManager.CurrentSkin.DDLExampleForeColor;
	}

	private void InitializeComponent()
	{
		base.SuspendLayout();
		this.AllowHtmlString = true;
		base.Appearance.BackColor = System.Drawing.Color.FromArgb(229, 229, 229);
		base.Appearance.Font = new System.Drawing.Font("Courier New", 8.25f);
		base.Appearance.ForeColor = System.Drawing.Color.FromArgb(57, 57, 57);
		base.Appearance.Options.UseBackColor = true;
		base.Appearance.Options.UseFont = true;
		base.Appearance.Options.UseForeColor = true;
		this.MaximumSize = new System.Drawing.Size(0, 24);
		this.MinimumSize = new System.Drawing.Size(0, 24);
		base.Padding = new System.Windows.Forms.Padding(4);
		base.Size = new System.Drawing.Size(0, 24);
		base.ResumeLayout(false);
	}
}
