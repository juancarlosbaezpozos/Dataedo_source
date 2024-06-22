using System;
using System.IO;
using Dataedo.App.Drivers.ODBC.Repositories;
using Dataedo.App.Drivers.ODBC.ValueObjects;

namespace Dataedo.App.Drivers.ODBC;

internal static class Factory
{
	private static IRepository remoteRepository;

	private static IRepository localRepository;

	public static IRepository GetRemoteRepository()
	{
		if (remoteRepository != null)
		{
			return remoteRepository;
		}
		remoteRepository = new RemoteRepository("https://dataedo.com");
		return remoteRepository;
	}

	public static IRepository GetLocalRepository()
	{
		if (localRepository != null)
		{
			return localRepository;
		}
		return new LocalRepository(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Dataedo", "Drivers", "ODBC"));
	}

	public static Driver MakeEmptyDriver()
	{
		return new Driver(new DriverMetaFile("universal", "Universal (tables, views, procedures)"), new DriverQueries());
	}
}
