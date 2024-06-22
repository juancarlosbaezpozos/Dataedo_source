using System;

namespace Dataedo.App.Drivers.ODBC.Repositories.Exceptions;

internal class DriverSerializationException : Exception
{
	public DriverSerializationException(string msg)
		: base(msg)
	{
	}
}
