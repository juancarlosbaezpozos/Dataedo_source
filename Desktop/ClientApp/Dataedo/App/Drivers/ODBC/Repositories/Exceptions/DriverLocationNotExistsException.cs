using System;

namespace Dataedo.App.Drivers.ODBC.Repositories.Exceptions;

internal class DriverLocationNotExistsException : Exception
{
	public DriverLocationNotExistsException(string msg)
		: base(msg)
	{
	}
}
