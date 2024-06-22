using System.Collections.Generic;
using System.Linq;
using Dataedo.DataProcessing.Synchronize.Classes;

namespace Dataedo.App.Tools.ClassificationSummary;

public class ClasificatorDataService
{
	private static List<string> SetClassificatorCustomFieldDistinctValues(string propertyName, IList<ClassificatorDataModel> dataSource)
	{
		List<string> list = new List<string>();
		foreach (ClassificatorDataModel item in dataSource)
		{
			string text = item.GetType().GetProperty(propertyName).GetValue(item, null)?.ToString();
			if (!list.Contains(text) && !string.IsNullOrEmpty(text))
			{
				list.Add(text);
			}
		}
		return list.Distinct().ToList();
	}

	public void InitializeDistinctValuesDictionary(Dictionary<string, IList<string>> dictionary, List<CustomFieldRow> filteredCustomFields, IList<ClassificatorDataModel> dataSource)
	{
		for (int i = 0; i < 5; i++)
		{
			if (filteredCustomFields.Count > i)
			{
				dictionary.Add(filteredCustomFields[i].Title, SetClassificatorCustomFieldDistinctValues($"Field{i + 1}Update", dataSource));
				AddDefinition(dictionary, filteredCustomFields[i]);
			}
		}
	}

	private void AddDefinition(Dictionary<string, IList<string>> dictionary, CustomFieldRow customField)
	{
		if (!string.IsNullOrEmpty(customField.Definition) && customField.CustomFieldId <= 0)
		{
			string[] array = customField.Definition.Split(',');
			foreach (string text in array)
			{
				dictionary[customField.Title].Add(text.Trim());
			}
		}
	}
}
