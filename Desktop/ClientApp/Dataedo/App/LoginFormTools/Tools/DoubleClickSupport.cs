using System;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraGrid.Views.Base;

namespace Dataedo.App.LoginFormTools.Tools;

internal class DoubleClickSupport
{
	private readonly Timer clickTimer;

	private DateTime lastClick;

	private bool inDoubleClick;

	private Rectangle doubleClickArea;

	private TimeSpan doubleClickMaxTime;

	public event EventHandler DoubleClick;

	public DoubleClickSupport(BaseView baseView)
	{
		baseView.MouseUp += BaseView_MouseUp;
		clickTimer = new Timer();
		clickTimer.Tick += ClickTimer_Tick;
	}

	internal void SetParameters()
	{
		inDoubleClick = false;
		doubleClickMaxTime = TimeSpan.FromMilliseconds(SystemInformation.DoubleClickTime);
		clickTimer.Interval = SystemInformation.DoubleClickTime;
	}

	private void BaseView_MouseUp(object sender, MouseEventArgs e)
	{
		if (inDoubleClick)
		{
			inDoubleClick = false;
			TimeSpan timeSpan = DateTime.Now - lastClick;
			if (doubleClickArea.Contains(e.Location) && timeSpan < doubleClickMaxTime)
			{
				clickTimer.Stop();
				this.DoubleClick?.Invoke(sender, EventArgs.Empty);
			}
		}
		else
		{
			clickTimer.Stop();
			clickTimer.Start();
			lastClick = DateTime.Now;
			inDoubleClick = true;
			Point location = e.Location - new Size(SystemInformation.DoubleClickSize.Width / 2, SystemInformation.DoubleClickSize.Height / 2);
			doubleClickArea = new Rectangle(location, SystemInformation.DoubleClickSize);
		}
	}

	private void ClickTimer_Tick(object sender, EventArgs e)
	{
		inDoubleClick = false;
		clickTimer.Stop();
	}
}
