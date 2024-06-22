using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;

namespace Dataedo.App.Data.MetadataServer;

internal class IgnoredObjects : CommonDBSupport
{
	public IgnoredObjects()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public bool Insert(List<IgnoredObject> objects, Form owner = null)
	{
		try
		{
			commands.Manipulation.Objects.InsertIgnoredObjects(objects.ToArray());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting to the ignored objects table:", owner);
			return false;
		}
		return true;
	}

	public bool Delete(List<IgnoredObject> objects, Form owner = null)
	{
		try
		{
			commands.Manipulation.Objects.DeleteIgnoredObjects(objects.ToArray());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting from the ignored objects table: ", owner);
			return false;
		}
		return true;
	}
}
