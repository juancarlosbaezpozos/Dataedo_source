using System;
using System.Collections.Generic;
using Npgsql;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

public class NpgSqlCommentsExportExceptionHandler : ICommentsExportExceptionHanlder
{
	public void HandleExceptions(Action action, List<DBDescription> objectsFailureList, DBDescription description)
	{
		try
		{
			action();
		}
		catch (PostgresException ex)
		{
			if (ex.SqlState.Equals("42P01"))
			{
				objectsFailureList.Add(description);
				return;
			}
			throw;
		}
	}
}
