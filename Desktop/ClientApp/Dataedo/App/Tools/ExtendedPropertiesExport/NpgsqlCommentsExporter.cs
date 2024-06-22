using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.Tools.Export;
using Npgsql;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

public class NpgsqlCommentsExporter : DBDescriptionExporter<NpgsqlConnection>
{
	private CommentsExportHelper helper;

	public override string DescriptionLabel => "comments";

	public NpgsqlCommentsExporter(BackgroundProcessingWorker exportWorker, string connectionString, int databaseId, List<int> modulesId)
		: base(exportWorker, connectionString, databaseId, modulesId)
	{
		helper = new CommentsExportHelper(new NpgSqlCommentsExportExceptionHandler());
	}

	protected override List<DBDescription> GetDescriptionObjects()
	{
		return helper.GetDescriptionObjects(databaseId, modulesId, types, "VIEW");
	}

	public override void ExportDescription(bool exportDescriptions = true, Form owner = null)
	{
		base.DescriptionObjects = GetDescriptionObjects();
		base.ExportDescription(exportDescriptions: true, owner);
	}

	protected override void HandleExceptions(Action action, List<DBDescription> objectsFailureList, DBDescription description)
	{
		helper.HandleExceptions(action, objectsFailureList, description);
	}

	public override void InitializeExportObjects(bool exportDescriptions = true)
	{
		base.DescriptionObjects = GetDescriptionObjects();
	}
}
