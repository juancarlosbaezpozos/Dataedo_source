using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Dataedo.App.Tools.UI;

public static class DrawingControl
{
	private const int WM_SETREDRAW = 11;

	[DllImport("user32.dll")]
	public static extern int SendMessage(IntPtr hWnd, int wMsg, bool wParam, int lParam);

	public static void SuspendDrawing(Control control)
	{
		if (control != null && !control.IsDisposed)
		{
			SendMessage(control.Handle, 11, wParam: false, 0);
		}
	}

	public static void ResumeDrawing(Control control)
	{
		if (control != null && !control.IsDisposed)
		{
			SendMessage(control.Handle, 11, wParam: true, 0);
			control.Refresh();
		}
	}
}
