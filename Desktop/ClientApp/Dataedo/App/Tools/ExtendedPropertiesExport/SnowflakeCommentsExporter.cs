using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.Tools.Export;
using Snowflake.Data.Client;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

internal class SnowflakeCommentsExporter : DBDescriptionExporter<SnowflakeDbConnection>
{
	private readonly string viewType = "VIEW";

	private CommentsExportHelper helper;

	public override string DescriptionLabel => "comments";

	public SnowflakeCommentsExporter(BackgroundProcessingWorker exportWorker, string connectionString, int databaseId, List<int> modulesId, ICommentsExportExceptionHanlder exceptionHandler)
		: base(exportWorker, connectionString, databaseId, modulesId)
	{
		helper = new CommentsExportHelper(exceptionHandler);
	}

	protected override List<DBDescription> GetDescriptionObjects()
	{
		return helper.GetDescriptionObjects(databaseId, modulesId, types, viewType);
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
