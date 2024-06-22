using System.ComponentModel;
using System.Windows.Forms;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Tools;
using Dataedo.App.Tools;
using Dataedo.Model.Data.Common.CustomFields;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class TableViewDoc : DependencyObjectDoc
{
	public BindingList<ColumnDoc> Columns { get; set; }

	public BindingList<UniqueContraintDoc> UniqueConstraints { get; set; }

	public BindingList<RelationsDoc> PKRelations { get; set; }

	public BindingList<RelationsDoc> FKRelations { get; set; }

	public BindingList<TriggerDoc> Triggers { get; set; }

	public override string CustomFieldsStringValuesSeparator => "<br/>";

	public TableViewDoc(DatabaseDoc database, ObjectDocObject row, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, int order, SharedDatabaseTypeEnum.DatabaseType? databaseType, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype, bool useModuleString, Form owner = null)
		: base(database, row, docGeneratingOptions, docHeaders, order, databaseType, objectType, useModuleString, owner)
	{
		base.IdString = PdfLinksSupport.CreateIdString(row);
	}

	protected override void RetrieveDescription(CustomFieldsData row, bool retrieveCustomFields = true)
	{
		RetrieveCustomFieldsAsHtmlWithDescription(row, retrieveCustomFields);
	}

	protected override void LoadStandardData(DatabaseDoc database, ObjectDocObject row)
	{
		base.Id = row.Id;
		base.DocumentationId = row.DatabaseId;
		base.DocumentationTitle = row.DatabaseTitle;
		base.HasDatabaseMultipleSchemas = row.DatabaseMultipleSchemas;
		base.ShowSchema = row.DatabaseDatabaseShowSchema;
		base.ShowSchemaOverride = row.DatabaseShowSchemaOverride;
		base.IsFromAnotherDocumentation = database.Id != base.DocumentationId;
	}
}
