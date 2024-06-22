using System;
using System.Windows.Forms;
using Dataedo.App.Tools.Exceptions;
using Dataedo.CustomMessageBox;

namespace Dataedo.App.Forms.Tools;

public static class FormExtensions
{
	public static DialogResult ShowDialog(this Form form, Form caller, bool setCustomMessageDefaultOwner, bool showWithOwner)
	{
		DialogResult result = DialogResult.None;
		try
		{
			if (setCustomMessageDefaultOwner)
			{
				CustomMessageBoxForm.DefaultOwner = form;
			}
			result = ((!showWithOwner) ? form.ShowDialog() : form.ShowDialog(caller));
			if (setCustomMessageDefaultOwner)
			{
				CustomMessageBoxForm.DefaultOwner = caller;
				return result;
			}
			return result;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, caller);
			return result;
		}
	}

	public static DialogResult ShowDialog(this Form form, Form caller, bool setCustomMessageDefaultOwner)
	{
		return form.ShowDialog(caller, setCustomMessageDefaultOwner, showWithOwner: true);
	}

	public static DialogResult ShowDialog(this Form form, Control caller, bool setCustomMessageDefaultOwner)
	{
		return form.ShowDialog(caller?.FindForm(), setCustomMessageDefaultOwner);
	}
}
