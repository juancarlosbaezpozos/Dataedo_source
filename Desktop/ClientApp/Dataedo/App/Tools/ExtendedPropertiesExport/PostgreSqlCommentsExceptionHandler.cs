using System;
using System.Collections.Generic;
using Npgsql;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

public class PostgreSqlCommentsExceptionHandler : ICommentsExportExceptionHanlder
{
	public void HandleExceptions(Action action, List<DBDescription> objectsFailureList, DBDescription description)
	{
		try
		{
			action();
		}
		catch (PostgresException ex)
		{
			if (ex.Message.Contains("does not exist"))
			{
				objectsFailureList.Add(description);
				return;
			}
			throw;
		}
	}
}
