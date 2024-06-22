using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Template.PdfTemplate.Model;
using Dataedo.App.Documentation.Tools;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Model.Data.Common.CustomFields;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class GlossaryEntryDoc : ObjectDoc
{
	public BindingList<RelatedTermDoc> RelatedTerms { get; set; }

	public BindingList<DataLinkDoc> DataLinks { get; set; }

	public override string CustomFieldsStringValuesSeparator => "<br/>";

	public GlossaryEntryDoc(DatabaseDoc database, TermDocObject row, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, int order, SharedDatabaseTypeEnum.DatabaseType? databaseType, bool useModuleString, Form owner = null)
		: base(docGeneratingOptions, database, row, docHeaders, order, databaseType, database.UseSchema, SharedObjectTypeEnum.ObjectType.Term, SharedObjectSubtypeEnum.StringToType(SharedObjectTypeEnum.ObjectType.Term, row.Subtype), (docGeneratingOptions?.Template as PdfTemplateModel)?.Customization?.Localization?.Headings, useModuleString, row.GlossaryTermTypeTitle)
	{
		base.IdString = PdfLinksSupport.CreateIdString(row.DatabaseId, row.Id);
		RelatedTerms = RelatedTermDoc.GetRelatedTerms(docGeneratingOptions, base.Id.Value, owner);
		DataLinks = DataLinkDoc.GetDataLinks(docGeneratingOptions, base.Id.Value, owner);
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

	public static BindingList<GlossaryEntryDoc> GetGlossaryEntries(IEnumerable<TermDocObject> glossaryEntries, DatabaseDoc database, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		try
		{
			return LoadDataToList(database, glossaryEntries, docGeneratingOptions, docHeaders, databaseType, owner);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting terms from the database.", owner);
			return null;
		}
	}

	private static BindingList<GlossaryEntryDoc> LoadDataToList(DatabaseDoc database, IEnumerable<TermDocObject> termsDataView, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner = null)
	{
		int tableOrder = 1;
		return new BindingList<GlossaryEntryDoc>(new List<GlossaryEntryDoc>(termsDataView.Select((TermDocObject x) => new GlossaryEntryDoc(database, x, docGeneratingOptions, docHeaders, tableOrder++, databaseType, useModuleString: false, owner))));
	}
}
