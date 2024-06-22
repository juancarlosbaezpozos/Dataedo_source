using System;
using System.Collections.Generic;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

internal class SnowflakeCommentsExceptionHandler : ICommentsExportExceptionHanlder
{
	public void HandleExceptions(Action action, List<DBDescription> objectsFailureList, DBDescription description)
	{
		try
		{
			action();
		}
		catch (Exception)
		{
			objectsFailureList.Add(description);
		}
	}
}
