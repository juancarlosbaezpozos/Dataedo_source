using System;
using System.Linq;
using Dataedo.DataProcessing.CustomFields;

namespace Dataedo.App.Tools.Search;

public class CustomFieldSearchItem
{
	public CustomFieldRowExtended CustomField { get; set; }

	public string SearchText { get; set; }

	public string[] SearchWords { get; set; }

	public CustomFieldSearchItem(CustomFieldRowExtended customField, string searchText)
	{
		CustomField = customField;
		SearchText = searchText;
		SearchWords = searchText.Split(new string[1] { " " }, StringSplitOptions.RemoveEmptyEntries).Distinct().ToArray();
	}
}
