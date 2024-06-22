using System;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Base.EventArgs;

public class NavigationEventArgs : System.EventArgs
{
	public bool IsAllowed { get; }

	public NavigationEventArgs(bool isAllowed)
	{
		IsAllowed = isAllowed;
	}
}
