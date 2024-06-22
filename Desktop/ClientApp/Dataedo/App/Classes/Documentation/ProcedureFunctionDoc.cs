using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Documentation.Common;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Tools;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Tools;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Common.CustomFields;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class ProcedureFunctionDoc : DependencyObjectDoc
{
	public BindingList<ParameterDoc> Parameters { get; set; }

	public override string CustomFieldsStringValuesSeparator => "<br/>";

	public List<DefinitionDoc> Definition { get; protected set; }

	public ProcedureFunctionDoc(DatabaseDoc database, ObjectDocObject row, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, int order, SharedDatabaseTypeEnum.DatabaseType? databaseType, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype subtype, bool useModuleString, Form owner = null)
		: base(database, row, docGeneratingOptions, docHeaders, order, databaseType, objectType, useModuleString, owner)
	{
		base.IdString = PdfLinksSupport.CreateIdString(row);
		if ((this is ProcedureDoc && !docGeneratingOptions.ExcludedObjects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Procedure, SharedObjectTypeEnum.ObjectType.Script))) || (this is FunctionDoc && !docGeneratingOptions.ExcludedObjects.Any((ObjectTypeHierarchy x) => x.IsType(SharedObjectTypeEnum.ObjectType.Function, SharedObjectTypeEnum.ObjectType.Script))))
		{
			string text = PrepareValue.ToString(row.Definition);
			if (!string.IsNullOrEmpty(text))
			{
				Definition = new List<DefinitionDoc>
				{
					new DefinitionDoc(text, "SQL", docGeneratingOptions)
				};
			}
		}
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
