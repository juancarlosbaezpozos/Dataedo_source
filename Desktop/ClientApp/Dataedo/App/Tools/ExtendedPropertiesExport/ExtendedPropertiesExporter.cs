using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Tools.Export;
using Dataedo.Model.Data.ExtendedProperties;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.Tools.ExtendedPropertiesExport;

internal class ExtendedPropertiesExporter : DBDescriptionExporter<SqlConnection>
{
	private List<DocumentationCustomFieldRow> customFields;

	private List<DBDescription> customFieldDescriptionObjects;

	public override string DescriptionLabel => "extended properties";

	public ExtendedPropertiesExporter(BackgroundProcessingWorker exportWorker, string connectionString, int databaseId, List<int> modulesId, List<DocumentationCustomFieldRow> customFields)
		: base(exportWorker, connectionString, databaseId, modulesId)
	{
		this.customFields = customFields;
		customFieldDescriptionObjects = new List<DBDescription>();
	}

	protected override List<DBDescription> GetDescriptionObjects()
	{
		List<DBDescription> list = new List<DBDescription>();
		if (!string.IsNullOrEmpty(types))
		{
			foreach (DbDescriptionObject item in DB.Database.UpdateExtendedPropertiesCommand(databaseId, modulesId, types))
			{
				list.Add(new DBDescription(item));
			}
			return list;
		}
		return list;
	}

	public void GetCustomFieldsDescriptionObjects(DocumentationCustomFieldRow customField)
	{
		foreach (DbDescriptionObject item in DB.Database.UpdateCustomFieldsExtendedPropertiesCommand(databaseId, modulesId, types, customField.FieldName, customField.CustomFieldId))
		{
			customFieldDescriptionObjects.Add(new DBDescription(item));
		}
	}

	public override void ExportDescription(bool exportDescriptions = true, Form owner = null)
	{
		InitializeExportObjects(exportDescriptions);
		base.ExportDescription(exportDescriptions: true, owner);
	}

	public override void InitializeExportObjects(bool exportDescriptions = true)
	{
		if (exportDescriptions)
		{
			base.DescriptionObjects = GetDescriptionObjects();
			AddDocumentationCustomFields();
		}
		else
		{
			base.DescriptionObjects = new List<DBDescription>();
			AddDocumentationCustomFields();
		}
	}

	protected override void HandleExceptions(Action action, List<DBDescription> objectsFailureList, DBDescription description)
	{
		try
		{
			action();
		}
		catch (SqlException ex) when (ex != null && (ex.Number == 15135 || ex.Number == 12320 || ex.Number == 3906 || ex.Number == 1088))
		{
			if (ex.Number == 3906)
			{
				throw;
			}
			objectsFailureList.Add(description);
		}
	}

	private void AddDocumentationCustomFields()
	{
		if (customFields == null)
		{
			return;
		}
		foreach (DocumentationCustomFieldRow customField in customFields)
		{
			GetCustomFieldsDescriptionObjects(customField);
		}
		base.DescriptionObjects.AddRange(customFieldDescriptionObjects);
	}
}
