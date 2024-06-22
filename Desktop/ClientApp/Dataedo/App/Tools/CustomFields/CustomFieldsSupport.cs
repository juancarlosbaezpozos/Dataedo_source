using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.CustomFields;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.CustomFields;

public class CustomFieldsSupport : BaseCustomFieldsSupport
{
	public List<DocumentationCustomFieldRow> DocumentationCustomFields { get; private set; }

	public override void LoadCustomFields(int customFieldsLimit, IEnumerable<int> idsToLoad = null)
	{
		IEnumerable<CustomFieldRowExtended> source = from x in DB.CustomField.GetCustomFields(customFieldsLimit, false)
			select new CustomFieldRowExtended(x);
		if (idsToLoad == null)
		{
			base.Fields = source.ToList();
			return;
		}
		base.Fields = source.Where((CustomFieldRowExtended x) => idsToLoad?.Contains(x.CustomFieldId) ?? false).ToList();
	}

	public void LoadDocumentationCustomFields(int? databaseId)
	{
		DocumentationCustomFields = DB.CustomField.GetDocumentationCustomFields(databaseId);
	}

	public override void LoadDistinctValues(SharedObjectTypeEnum.ObjectType? objectType)
	{
		if (base.Fields == null)
		{
			return;
		}
		foreach (CustomFieldRowExtended item in base.Fields.Where((CustomFieldRowExtended x) => x.Type != CustomFieldTypeEnum.CustomFieldType.Text && (!objectType.HasValue || x.IsFieldVisible(objectType, visibleIfSelectedOnly: false))))
		{
			IEnumerable<string> enumerable = new List<string>();
			if (item.IsDefinitionType && !string.IsNullOrEmpty(item.Definition))
			{
				enumerable = item.Definition?.Split(BaseCustomFieldsSupport.ListSeparators, StringSplitOptions.RemoveEmptyEntries)?.Select((string y) => y.Trim())?.Where((string x) => !string.IsNullOrEmpty(x));
			}
			if (item.IsOpenDefinitionType)
			{
				enumerable = enumerable.Concat(from x in DB.CustomField.GetCustomFieldDistinctValues(item.CustomFieldId)
					where !string.IsNullOrEmpty(x)
					select x);
			}
			item.SetDefinitionValues(enumerable);
		}
	}

	public void SelectAllDocumentationCustomFields()
	{
		foreach (DocumentationCustomFieldRow documentationCustomField in DocumentationCustomFields)
		{
			documentationCustomField.IsSelected = true;
		}
	}

	public void UnselectAllDocumentationCustomFields()
	{
		foreach (DocumentationCustomFieldRow documentationCustomField in DocumentationCustomFields)
		{
			documentationCustomField.IsSelected = false;
		}
	}
}
