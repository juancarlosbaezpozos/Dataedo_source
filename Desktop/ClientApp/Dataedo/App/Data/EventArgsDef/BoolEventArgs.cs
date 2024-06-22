using System;

namespace Dataedo.App.Data.EventArgsDef;

public class BoolEventArgs : EventArgs
{
	public bool Value { get; set; }

	public BoolEventArgs(bool value)
	{
		Value = value;
	}
}
