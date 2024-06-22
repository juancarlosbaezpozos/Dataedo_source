using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Data.Commands;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.DataProcessing.MetadataServer;
using Dataedo.DataProcessing.Synchronize;

namespace Dataedo.App.Data.MetadataServer;

public abstract class CommonDBSupport : BaseCommonDBSupport
{
	protected new CommandsSetBase commands;

	public CommonDBSupport()
	{
	}

	protected new string GetNotStatusValue(bool notDeletedOnly)
	{
		string result = null;
		if (notDeletedOnly)
		{
			result = "D";
		}
		return result;
	}

	protected new void SetCustomFields(BaseWithCustomFields destination, BaseRow row)
	{
		Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> customFields = row.CustomFields?.CustomFieldsData?.Where((CustomFieldDefinition x) => x?.CustomField != null)?.ToDictionary((CustomFieldDefinition x) => x.CustomField.FieldName, (CustomFieldDefinition x) => x.CustomField.GetCustomFieldWithValue(x.FieldValue));
		destination.SetCustomFields(customFields);
	}

	protected void SetCustomFields(BaseWithCustomFieldsForSynchronization destination, ObjectRow row)
	{
		Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> customFields = row.CustomFields?.CustomFieldsData?.Where((CustomFieldDefinition x) => x?.CustomField != null)?.ToDictionary((CustomFieldDefinition x) => x.CustomField.FieldName, (CustomFieldDefinition x) => x.CustomField.GetCustomFieldWithValue(x.FieldValue));
		destination.SetCustomFields(customFields);
	}

	protected new void SetCustomFields(BaseWithCustomFieldsForSynchronization destination, BaseRow row)
	{
		Dictionary<string, BaseWithCustomFields.CustomFieldWithValue> customFields = row.CustomFields?.CustomFieldsData?.Where((CustomFieldDefinition x) => x?.CustomField != null)?.ToDictionary((CustomFieldDefinition x) => x.CustomField.FieldName, (CustomFieldDefinition x) => x.CustomField.GetCustomFieldWithValue(x.FieldValue));
		destination.SetCustomFields(customFields);
	}
}
