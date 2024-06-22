using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Dataedo.App.Tools;

public static class RunExeHelper
{
	private static void RunExe(string path)
	{
		Process process = new Process();
		process.StartInfo.FileName = path;
		process.Start();
		IntPtr handle = process.Handle;
		while (handle == IntPtr.Zero)
		{
		}
		SetForegroundWindow(handle);
	}

	[DllImport("USER32.DLL")]
	private static extern bool SetForegroundWindow(IntPtr hWnd);
}
