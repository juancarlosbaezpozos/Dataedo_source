using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Windows.Forms;
using Dataedo.App.Tools.Export;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

internal class OdbcCommentsExporter : DBDescriptionExporter<OdbcConnection>
{
	private CommentsExportHelper helper;

	private readonly string viewType;

	public override string DescriptionLabel => "comments";

	public OdbcCommentsExporter(BackgroundProcessingWorker exportWorker, string connectionString, int databaseId, List<int> modulesId, string viewType)
		: base(exportWorker, connectionString, databaseId, modulesId)
	{
		helper = new CommentsExportHelper(new OdbcCommentsExportExceptionHandler());
		this.viewType = viewType;
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
