using System;

namespace Dataedo.App.Data.EventArgsDef;

public class ObjectEventArgs : EventArgs
{
	public int DatabaseId { get; set; }

	public int ObjectId { get; set; }

	public int? ModuleId { get; set; }

	public string ModuleName { get; set; }

	public ObjectEventArgs()
	{
	}

	public ObjectEventArgs(int databaseId, int objectId, int? moduleId = -1, string moduleName = null)
	{
		DatabaseId = databaseId;
		ObjectId = objectId;
		ModuleId = moduleId;
		ModuleName = moduleName;
	}
}
