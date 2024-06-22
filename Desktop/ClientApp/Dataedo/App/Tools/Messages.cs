using System;
using System.Windows.Forms;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;

namespace Dataedo.App.Tools;

public class Messages
{
	public static void CheckAndShowErrorMessage(Exception ex, string message, int? result, Form owner = null)
	{
		if (result != 0)
		{
			if (ex != null)
			{
				GeneralExceptionHandling.Handle(ex, message, "Error", owner);
			}
			else
			{
				GeneralMessageBoxesHandling.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			}
		}
	}

	public static void ShowErrorMessage(string message, Exception ex, Form owner = null)
	{
		GeneralExceptionHandling.Handle(ex, message, owner);
	}
}
