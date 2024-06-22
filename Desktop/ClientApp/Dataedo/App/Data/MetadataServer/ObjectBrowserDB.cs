using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.UserControls.ObjectBrowser;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer;

public class ObjectBrowserDB : CommonDBSupport
{
	public ObjectBrowserDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public List<ObjectBrowserItem> GetObjectsForObjectBrowser(int? databaseId = null, Form owner = null)
	{
		try
		{
			return (from x in commands.Select.ObjectBrowser.GetObjectsByDatabaseId(databaseId)
				select new ObjectBrowserItem(x)).ToList();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting objects for object browser:", owner);
			return null;
		}
	}

	public List<ObjectBrowserItem> GetObjectsForObjectBrowser(List<int> databaseIDs, Form owner = null)
	{
		try
		{
			if (databaseIDs == null || !databaseIDs.Any())
			{
				return null;
			}
			return (from x in commands.Select.ObjectBrowser.GetObjectsByDatabaseIDs(databaseIDs)
				select new ObjectBrowserItem(x)).ToList();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting objects for object browser:", owner);
			return null;
		}
	}

	public List<ObjectIdWithType> GetObjectRelatedTableIDs(int objectId, SharedObjectTypeEnum.ObjectType objectType, Form owner = null)
	{
		try
		{
			switch (objectType)
			{
			case SharedObjectTypeEnum.ObjectType.View:
				return commands.Select.ObjectBrowser.GetObjectRelatedObjectsByViewId(objectId);
			case SharedObjectTypeEnum.ObjectType.Function:
			case SharedObjectTypeEnum.ObjectType.Procedure:
				return commands.Select.ObjectBrowser.GetObjectRelatedObjectsByProcedureId(objectId);
			default:
				return new List<ObjectIdWithType>();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting related tables for object browser:", owner);
			return null;
		}
	}

	public List<ObjectIdWithType> GetObjectRelatedTableIDsByProcessScript(int processId, Form owner = null)
	{
		try
		{
			return commands.Select.ObjectBrowser.GetObjectRelatedTableIDsByProcessScript(processId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting related tables by process for object browser:", owner);
			return null;
		}
	}
}
