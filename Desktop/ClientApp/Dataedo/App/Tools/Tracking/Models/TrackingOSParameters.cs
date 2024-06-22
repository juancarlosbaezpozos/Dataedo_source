using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Dataedo.App.Tools.Tracking.Models;

public class TrackingOSParameters
{
	public enum DeviceCap
	{
		VERTRES = 10,
		DESKTOPVERTRES = 117,
		DESKTOPHORZRES = 118
	}

	private const string windows = "Microsoft Windows";

	public string OS => Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion", "ProductName", string.Empty).ToString();

	public string OSVersion
	{
		get
		{
			if (!RuntimeInformation.OSDescription.Contains("Microsoft Windows"))
			{
				return RuntimeInformation.OSDescription;
			}
			return RuntimeInformation.OSDescription.Substring("Microsoft Windows".Length)?.Trim();
		}
	}

	public string DisplaySize => GetResolution();

	public string DisplayScaling => $"{GetScalingFactor()}%";

	public string DeviceName
	{
		get
		{
			try
			{
				return Registry.GetValue("HKEY_LOCAL_MACHINE\\HARDWARE\\DESCRIPTION\\System\\BIOS", "SystemVersion", string.Empty).ToString();
			}
			catch
			{
				return string.Empty;
			}
		}
	}

	public string DeviceId
	{
		get
		{
			try
			{
				return Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Cryptography", "MachineGuid", string.Empty).ToString();
			}
			catch
			{
				return string.Empty;
			}
		}
	}

	public float GetScalingFactor()
	{
		IntPtr hdc = Graphics.FromHwnd(IntPtr.Zero).GetHdc();
		int deviceCaps = GetDeviceCaps(hdc, 10);
		return (int)((float)GetDeviceCaps(hdc, 117) / (float)deviceCaps * 100f);
	}

	public string GetResolution()
	{
		IntPtr hdc = Graphics.FromHwnd(IntPtr.Zero).GetHdc();
		int deviceCaps = GetDeviceCaps(hdc, 117);
		int deviceCaps2 = GetDeviceCaps(hdc, 118);
		return $"{deviceCaps2}x{deviceCaps}";
	}

	[DllImport("gdi32.dll")]
	private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
}
