using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Template.PdfTemplate.Model;
using Dataedo.App.Enums;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Tools;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class DependencyObjectDoc : ObjectDoc
{
	public string ObjectSource { get; set; }

	public BindingList<DependencyDoc> UsesDependencies { get; set; }

	public BindingList<DependencyDoc> TriggerDependencies { get; set; }

	public BindingList<DependencyDoc> UsedByDependencies { get; set; }

	public DependencyObjectDoc()
	{
	}

	public DependencyObjectDoc(DatabaseDoc database, ObjectDocObject row, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, int order, SharedDatabaseTypeEnum.DatabaseType? databaseType, SharedObjectTypeEnum.ObjectType objectType, bool useModuleString, Form owner = null)
		: base(docGeneratingOptions, database, row, docHeaders, order, databaseType, database.UseSchema, objectType, SharedObjectSubtypeEnum.StringToType(objectType, row.Subtype), (docGeneratingOptions?.Template as PdfTemplateModel)?.Customization?.Localization?.Headings, useModuleString)
	{
		base.Id = row.Id;
		base.Schema = row.Schema;
		base.ObjectName = row.Name;
		base.ObjectTitle = row.Title;
		int databaseId = row.DatabaseId;
		SharedDatabaseTypeEnum.DatabaseType? databaseType3 = (base.CurrentObjectDocumentationType = DatabaseTypeEnum.StringToType(row.DatabaseType));
		string databaseHost = row.DatabaseHost;
		string databaseName = row.DatabaseName;
		string databaseTitle = row.DatabaseTitle;
		bool? databaseMultipleSchemas = row.DatabaseMultipleSchemas;
		if (objectType == SharedObjectTypeEnum.ObjectType.Table || objectType == SharedObjectTypeEnum.ObjectType.View || objectType == SharedObjectTypeEnum.ObjectType.Structure)
		{
			ObjectSource = row.Source;
		}
		if (docGeneratingOptions.LoadDependencies && !docGeneratingOptions.ExcludedObjects.Any((ObjectTypeHierarchy x) => x.ObjectType == objectType && x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Dependency))
		{
			DatabaseInfo currentDatabase = new DatabaseInfo(database.Id.Value, database.Type, database.Server, database.Database, database.Title, database.HasMultipleSchemas, database.ShowSchema, database.ShowSchemaOverride, base.Schema);
			UsesDependencies = DependencyDoc.GetUsesDependencies(databaseId, databaseType3, databaseHost, databaseName, databaseTitle, databaseMultipleSchemas, row.DatabaseDatabaseShowSchema, row.DatabaseShowSchemaOverride, DatabaseRow.GetShowSchema(row.DatabaseDatabaseShowSchema, row.DatabaseShowSchemaOverride), base.Schema, base.ObjectName, base.ObjectTitle, ObjectSource, objectType, base.ObjectSubtype, "DBMS", currentDatabase, base.Id.Value, owner);
			UsedByDependencies = DependencyDoc.GetUsedByDependencies(databaseId, databaseType3, databaseHost, databaseName, databaseTitle, databaseMultipleSchemas, row.DatabaseDatabaseShowSchema, row.DatabaseShowSchemaOverride, DatabaseRow.GetShowSchema(row.DatabaseDatabaseShowSchema, row.DatabaseShowSchemaOverride), base.Schema, base.ObjectName, base.ObjectTitle, ObjectSource, objectType, base.ObjectSubtype, "DBMS", currentDatabase, base.Id.Value, owner);
		}
	}
}
