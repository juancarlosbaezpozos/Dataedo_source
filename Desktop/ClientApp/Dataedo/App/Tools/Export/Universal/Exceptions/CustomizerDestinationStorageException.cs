using System;

namespace Dataedo.App.Tools.Export.Universal.Exceptions;

internal class CustomizerDestinationStorageException : Exception
{
	public CustomizerDestinationStorageException(string msg)
		: base(msg)
	{
	}
}
