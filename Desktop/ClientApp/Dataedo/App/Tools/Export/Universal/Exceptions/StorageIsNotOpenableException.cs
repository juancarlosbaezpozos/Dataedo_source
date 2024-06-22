using System;

namespace Dataedo.App.Tools.Export.Universal.Exceptions;

internal class StorageIsNotOpenableException : Exception
{
	public StorageIsNotOpenableException(string msg)
		: base(msg)
	{
	}
}
