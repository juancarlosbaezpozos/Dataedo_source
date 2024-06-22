using System.ComponentModel;
using System.Windows.Forms;
using Dataedo.App.Tools.UI;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;

namespace Dataedo.App.UserControls.Base;

public class BaseXtraUserControl : XtraUserControl
{
	private IContainer components;

	public BaseXtraUserControl()
	{
		InitializeComponent();
		SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, value: true);
		ApplyStyle();
		UserLookAndFeel.Default.StyleChanged += delegate
		{
			ApplyStyle();
		};
	}

	private void ApplyStyle()
	{
		BackColor = SkinColors.ControlColorFromSystemColors;
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
		this.components = new System.ComponentModel.Container();
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
	}
}
