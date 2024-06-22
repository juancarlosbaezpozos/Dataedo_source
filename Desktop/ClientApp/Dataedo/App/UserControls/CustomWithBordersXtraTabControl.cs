using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraTab;

namespace Dataedo.App.UserControls;

public class CustomWithBordersXtraTabControl : XtraTabControl
{
	private IContainer components;

	public CustomWithBordersXtraTabControl()
	{
		InitializeComponent();
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
