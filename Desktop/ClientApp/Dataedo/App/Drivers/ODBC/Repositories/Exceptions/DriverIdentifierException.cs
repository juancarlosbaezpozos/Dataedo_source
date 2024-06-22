using System;

namespace Dataedo.App.Drivers.ODBC.Repositories.Exceptions;

internal class DriverIdentifierException : Exception
{
	public DriverIdentifierException(string msg)
		: base(msg)
	{
	}
}
