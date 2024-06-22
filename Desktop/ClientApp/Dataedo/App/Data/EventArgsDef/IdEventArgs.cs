using System;

namespace Dataedo.App.Data.EventArgsDef;

public class IdEventArgs : EventArgs
{
	public int? DatabaseId { get; set; }

	public IdEventArgs()
	{
	}

	public IdEventArgs(int databaseId)
	{
		DatabaseId = databaseId;
	}
}
