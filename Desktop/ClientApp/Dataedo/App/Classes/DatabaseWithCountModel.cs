using System;
using Dataedo.App.Data.MetadataServer;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes;

public class DatabaseWithCountModel
{
	private readonly string title;

	private readonly int count;

	private readonly DateTime? lastImported;

	public string Title => title;

	public DateTime? LastImported => lastImported;

	public int? Count => count;

	public string Import { get; set; }

	public string Export { get; set; }

	public int DatabaseId { get; set; }

	public string Class { get; set; }

	public DatabaseWithCountModel(int databaseId, string databaseClass, string title, int count, DateTime? lastImported)
	{
		DatabaseId = databaseId;
		Class = databaseClass;
		this.title = title;
		this.count = count;
		this.lastImported = lastImported;
		SetImportText(databaseId);
	}

	private void SetImportText(int databaseId)
	{
		SharedDatabaseTypeEnum.DatabaseType? databaseType = SharedDatabaseTypeEnum.StringToType(DB.Database.GetDatabaseTypeById(databaseId));
		if (!databaseType.HasValue)
		{
			Import = null;
		}
		else if (databaseType == SharedDatabaseTypeEnum.DatabaseType.Manual)
		{
			Import = "import";
		}
		else
		{
			Import = "import";
		}
		Export = "export";
	}
}
