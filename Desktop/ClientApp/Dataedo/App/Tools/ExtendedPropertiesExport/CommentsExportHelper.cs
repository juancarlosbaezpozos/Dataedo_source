using System;
using System.Collections.Generic;
using Dataedo.App.Data.MetadataServer;
using Dataedo.Model.Data.ExtendedProperties;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

public class CommentsExportHelper
{
	private ICommentsExportExceptionHanlder exceptionHandler;

	public CommentsExportHelper(ICommentsExportExceptionHanlder exceptionHandler)
	{
		this.exceptionHandler = exceptionHandler;
	}

	public List<DBDescription> GetDescriptionObjects(int databaseId, List<int> modulesId, string types, string viewType)
	{
		List<DBDescription> list = new List<DBDescription>();
		foreach (DbDescriptionObject item in DB.Database.UpdateCommentsCommand(databaseId, modulesId, types, viewType))
		{
			list.Add(new DBDescription(item));
		}
		return list;
	}

	public void HandleExceptions(Action action, List<DBDescription> objectsFailureList, DBDescription description)
	{
		exceptionHandler.HandleExceptions(action, objectsFailureList, description);
	}
}
