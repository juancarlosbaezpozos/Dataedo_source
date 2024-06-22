using System;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;

namespace Dataedo.App.UserControls.PanelControls.CommonHelpers;

internal class ScrollableControlHelper
{
	private XtraScrollableControl control;

	public int ScrollValue { get; set; } = 60;


	public ScrollableControlHelper(XtraScrollableControl control)
	{
		this.control = control;
	}

	public void EnableMouseWheel()
	{
		control.VisibleChanged += Control_VisibleChanged;
	}

	public void DisableMouseWheel()
	{
		control.VisibleChanged -= Control_VisibleChanged;
	}

	private void Control_VisibleChanged(object sender, EventArgs e)
	{
		control.Select();
		RemoveMouseWheelHandler(control.Controls);
		AddMouseWheelHandler(control.Controls);
	}

	private void AddMouseWheelHandler(Control.ControlCollection controls)
	{
		foreach (Control control in controls)
		{
			control.MouseWheel += Control_MouseWheel;
			AddMouseWheelHandler(control.Controls);
		}
	}

	private void RemoveMouseWheelHandler(Control.ControlCollection controls)
	{
		foreach (Control control in controls)
		{
			control.MouseWheel -= Control_MouseWheel;
			RemoveMouseWheelHandler(control.Controls);
		}
	}

	private void Control_MouseWheel(object sender, MouseEventArgs e)
	{
		DXMouseEventArgs.GetMouseArgs(e).Handled = true;
		int value = control.VerticalScroll.Value;
		if (e.Delta < 0)
		{
			control.VerticalScroll.Value += ScrollValue;
		}
		else
		{
			if (control.VerticalScroll.Value < ScrollValue)
			{
				control.VerticalScroll.Value = 0;
			}
			control.VerticalScroll.Value -= ScrollValue;
		}
		if (control.VerticalScroll.Visible || control.VerticalScroll.Value != value)
		{
			return;
		}
		if (e.Delta < 0)
		{
			control.HorizontalScroll.Value += ScrollValue;
			return;
		}
		if (control.HorizontalScroll.Value < ScrollValue)
		{
			control.HorizontalScroll.Value = 0;
		}
		control.HorizontalScroll.Value -= ScrollValue;
	}
}
