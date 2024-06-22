using System;
using System.Collections.Generic;
using System.Data.Odbc;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

public class OdbcCommentsExportExceptionHandler : ICommentsExportExceptionHanlder
{
	public void HandleExceptions(Action action, List<DBDescription> objectsFailureList, DBDescription description)
	{
		try
		{
			action();
		}
		catch (OdbcException ex)
		{
			OdbcError odbcError = ex.Errors[0];
			if (odbcError == null)
			{
				throw;
			}
			if (odbcError.SQLState.Equals("42S02") || odbcError.SQLState.Equals("42704"))
			{
				objectsFailureList.Add(description);
				return;
			}
			throw;
		}
	}
}
