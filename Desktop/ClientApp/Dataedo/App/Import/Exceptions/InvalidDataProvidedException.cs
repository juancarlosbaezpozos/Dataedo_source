using System;

namespace Dataedo.App.Import.Exceptions;

internal class InvalidDataProvidedException : Exception
{
	public InvalidDataProvidedException()
	{
	}

	public InvalidDataProvidedException(string message)
		: base(message)
	{
	}

	public InvalidDataProvidedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
