using System;
using System.Windows.Forms;

namespace Dataedo.App.Tools.FormsSupport;

public static class DrawingHelpers
{
	public static void SuspendDrawing(Control control)
	{
		Message m = Message.Create(control.Handle, 11, IntPtr.Zero, IntPtr.Zero);
		NativeWindow.FromHandle(control.Handle)?.DefWndProc(ref m);
	}

	public static void ResumeDrawing(Control control)
	{
		Message m = Message.Create(control.Handle, 11, new IntPtr(1), IntPtr.Zero);
		NativeWindow.FromHandle(control.Handle)?.DefWndProc(ref m);
		control.Refresh();
	}
}
