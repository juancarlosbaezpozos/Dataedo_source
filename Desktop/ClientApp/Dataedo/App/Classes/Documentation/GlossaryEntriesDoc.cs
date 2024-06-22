using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class GlossaryEntriesDoc
{
	public string Header { get; set; }

	public BindingList<GlossaryEntryDoc> GlossaryEntries { get; set; }

	public static BindingList<GlossaryEntriesDoc> GetGlossaryEntries(DatabaseDoc database, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, List<ObjectTypeHierarchy> excludedObjects, Form owner = null)
	{
		try
		{
			List<GlossaryEntriesDoc> list = new List<GlossaryEntriesDoc>();
			int index = 0;
			SharedTermTypeEnum.TermType[] allTermTypeCodes = SharedTermTypeEnum.GetTermTypes();
			IEnumerable<SharedTermTypeEnum.TermType> enumerable = allTermTypeCodes.Where((SharedTermTypeEnum.TermType x) => !excludedObjects.Any((ObjectTypeHierarchy y) => SharedTermTypeEnum.IsEqualTo(x, y.ObjectSubtype)));
			List<TermDocObject> glossaryEntriesDoc = DB.BusinessGlossary.GetGlossaryEntriesDoc(database.Id.Value, allTermTypeCodes.Except(enumerable).ToArray(), !excludedObjects.Any((ObjectTypeHierarchy x) => x.ObjectSubtype == SharedObjectTypeEnum.ObjectType.Other));
			DocHeaders localDocHeaders = new DocHeaders(index, database.NumberingPrefix);
			foreach (SharedTermTypeEnum.TermType termTypeCode in enumerable)
			{
				IEnumerable<TermDocObject> glossaryEntries = glossaryEntriesDoc.Where((TermDocObject x) => x.GlossaryTermTypeCode == SharedTermTypeEnum.TypeToString(termTypeCode));
				AddGlossaryEntries(database, docGeneratingOptions, docHeaders, localDocHeaders, databaseType, SharedTermTypeEnum.TypeToStringForMenu(termTypeCode), glossaryEntries, ref index, list, owner);
			}
			IEnumerable<TermDocObject> glossaryEntries2 = glossaryEntriesDoc.Where((TermDocObject x) => !allTermTypeCodes.Any((SharedTermTypeEnum.TermType y) => x.GlossaryTermTypeCode == SharedTermTypeEnum.TypeToString(y)));
			AddGlossaryEntries(database, docGeneratingOptions, docHeaders, localDocHeaders, databaseType, "Other", glossaryEntries2, ref index, list, owner);
			return new BindingList<GlossaryEntriesDoc>(list);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting terms from the database.", owner);
			return null;
		}
	}

	private static void AddGlossaryEntries(DatabaseDoc database, DocGeneratingOptions docGeneratingOptions, DocHeaders docHeaders, DocHeaders localDocHeaders, SharedDatabaseTypeEnum.DatabaseType? databaseType, string headerName, IEnumerable<TermDocObject> glossaryEntries, ref int index, List<GlossaryEntriesDoc> result, Form owner = null)
	{
		if (glossaryEntries.Count() != 0)
		{
			index++;
			result.Add(new GlossaryEntriesDoc
			{
				Header = docHeaders.CreateListNameWithOrder(headerName, headerName),
				GlossaryEntries = GlossaryEntryDoc.GetGlossaryEntries(glossaryEntries, database, docGeneratingOptions, localDocHeaders, databaseType, owner)
			});
		}
	}
}
