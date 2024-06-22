using System;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Tools.MessageBoxes;

namespace Dataedo.App.Tools.UI;

public static class ClipboardSupport
{
	private static readonly int retryTimes = 5;

	private static readonly int retryDelay = 200;

	public static void SetDataObject(object data, Form owner = null)
	{
		try
		{
			Clipboard.SetDataObject(data, copy: false, retryTimes, retryDelay);
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to copy to clipboard.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, owner);
		}
	}

	public static void SetText(string data, Form owner = null)
	{
		SetDataObject(data, owner);
	}

	internal static void SetImage(Image data)
	{
		SetDataObject(data);
	}
}
