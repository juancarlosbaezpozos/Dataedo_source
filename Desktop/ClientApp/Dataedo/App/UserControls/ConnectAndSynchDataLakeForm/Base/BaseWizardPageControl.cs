using System;
using System.Drawing;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Base.EventArgs;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Base;

public class BaseWizardPageControl : BaseUserControl
{
	private bool allowContinue;

	public bool AllowContinue
	{
		get
		{
			return allowContinue;
		}
		protected set
		{
			if (allowContinue != value)
			{
				allowContinue = value;
				OnAllowContinueChanged(new NavigationEventArgs(allowContinue));
			}
		}
	}

	public event EventHandler<NavigationEventArgs> AllowContinueChanged;

	public event EventHandler Continue;

	public BaseWizardPageControl()
	{
		BackColor = Color.Transparent;
	}

	protected virtual void OnAllowContinueChanged(NavigationEventArgs e)
	{
		this.AllowContinueChanged?.Invoke(this, e);
	}

	protected virtual void OnContinue()
	{
		this.Continue?.Invoke(this, new System.EventArgs());
	}
}
