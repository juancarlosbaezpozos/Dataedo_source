using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;

namespace Dataedo.App.Overriden_controls;

public class CheckedComboBoxEditWithButtons : CheckedComboBoxEdit
{
	private SimpleButton extraButton;

	public string ExtraButtonCaption { get; set; }

	public event EventHandler ExtraButtonEvent;

	public CheckedComboBoxEditWithButtons()
	{
		Popup += CheckedComboBoxEditWithButtons_Popup;
	}

	private void CheckedComboBoxEditWithButtons_Popup(object sender, EventArgs e)
	{
		if (extraButton == null || extraButton.IsDisposed)
		{
			extraButton = new SimpleButton();
			extraButton.Text = ExtraButtonCaption;
			extraButton.Width = 70;
			extraButton.Click += extraButton_Click;
		}
		ControlCollection controls = (sender as IPopupControl).PopupWindow.Controls;
		if (!controls.Contains(extraButton))
		{
			controls.Add(extraButton);
		}
		Control control = controls[0];
		int num = control.Location.X - (PopupForm.Width - control.Width - 10);
		int num2 = control.Location.Y;
		extraButton.Location = new Point(num, num2);
	}

	private void extraButton_Click(object sender, EventArgs e)
	{
		ClosePopup();
		if (this.ExtraButtonEvent != null)
		{
			this.ExtraButtonEvent(this, null);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && extraButton != null)
		{
			extraButton.Click -= extraButton_Click;
		}
		Popup -= CheckedComboBoxEditWithButtons_Popup;
		base.Dispose(disposing);
	}
}
