using System;
using System.Collections.Generic;
using Devart.Data.Oracle;
using Devart.Data.Universal;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

public class OracleCommentsExceptionHandler : ICommentsExportExceptionHanlder
{
	public void HandleExceptions(Action action, List<DBDescription> objectsFailureList, DBDescription description)
	{
		try
		{
			action();
		}
		catch (UniException ex)
		{
			if (!(ex.InnerException is OracleException ex2))
			{
				throw;
			}
			if (ex2.Code == 942 || ex2.Code == 904 || ex2.Code == 1031)
			{
				objectsFailureList.Add(description);
				return;
			}
			throw;
		}
	}
}
