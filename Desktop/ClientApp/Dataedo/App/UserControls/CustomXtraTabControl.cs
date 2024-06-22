using System.ComponentModel;
using System.Windows.Forms;
using Dataedo.App.Tools.UI;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;

namespace Dataedo.App.UserControls;

public class CustomXtraTabControl : XtraTabControl
{
	private IContainer components;

	public CustomXtraTabControl()
		: this(noBorders: true)
	{
	}

	public CustomXtraTabControl(bool noBorders = true)
	{
		InitializeComponent();
		if (noBorders)
		{
			BorderStyle = BorderStyles.NoBorder;
			BorderStylePage = BorderStyles.NoBorder;
			LookAndFeel.Style = LookAndFeelStyle.Flat;
			ShowTabHeader = DefaultBoolean.True;
			BackColor = SkinColors.ControlColorFromSystemColors;
			Appearance.Options.UseBackColor = true;
			ForeColor = SkinColors.ControlForeColorFromSystemColors;
			Appearance.Options.UseForeColor = true;
			LookAndFeel.UseDefaultLookAndFeel = false;
		}
	}

	private void CustomXtraTabControl_KeyDown(object sender, KeyEventArgs e)
	{
		if ((e.Control && e.KeyCode == Keys.Tab) || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
		{
			e.Handled = true;
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
		((System.ComponentModel.ISupportInitialize)this).BeginInit();
		base.SuspendLayout();
		base.KeyDown += new System.Windows.Forms.KeyEventHandler(CustomXtraTabControl_KeyDown);
		((System.ComponentModel.ISupportInitialize)this).EndInit();
		base.ResumeLayout(false);
	}
}
