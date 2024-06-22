using System;

namespace Dataedo.App.Data.EventArgsDef;

public class ConnectionStatusEventArgs : EventArgs
{
	public bool IsSuccessful { get; set; }

	public ConnectionStatusEventArgs(bool isSuccessful)
	{
		IsSuccessful = isSuccessful;
	}
}
