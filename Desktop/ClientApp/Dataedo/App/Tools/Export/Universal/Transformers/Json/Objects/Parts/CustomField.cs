using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

internal class CustomField
{
	[JsonProperty("value")]
	public string Value;

	[JsonProperty("type")]
	public string Type;

	public CustomField(ICustomField customField)
	{
		Value = customField.Value;
		Type = CustomFieldTypeEnum.TypeToString(customField.Type);
	}

	public static bool HasValue(ICustomField customField)
	{
		if (customField.Type == CustomFieldTypeEnum.CustomFieldType.Checkbox)
		{
			if (!CustomFieldRowExtended.AcceptableCheckboxYesValues.Contains(customField.Value))
			{
				return CustomFieldRowExtended.AcceptableCheckboxNoValues.Contains(customField.Value);
			}
			return true;
		}
		return !string.IsNullOrEmpty(customField.Value);
	}

	public static IList<string> GetUniqueNamesWithValue(IEnumerable<object> items, Func<object, IList<ICustomField>> getCustomFields)
	{
		if (items == null || items.Count() == 0)
		{
			return new List<string>();
		}
		List<string> list = (from x in getCustomFields(items.First())
			select x.Name).ToList();
		Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
		foreach (string item in list)
		{
			dictionary.Add(item, value: false);
		}
		foreach (object item2 in items)
		{
			foreach (string item3 in from x in getCustomFields(item2)
				where HasValue(x)
				select x.Name)
			{
				dictionary[item3] = true;
			}
		}
		List<string> list2 = new List<string>();
		foreach (string item4 in list)
		{
			if (dictionary[item4])
			{
				list2.Add(item4);
			}
		}
		return list2;
	}
}
