using System.Collections.Generic;

namespace Dataedo.App.Tools.ClassificationExplorer;

public class ClassificationExplorerCustomFieldsItem
{
	public int ClassificatorId { get; private set; }

	public string DisplayName { get; set; }

	public IEnumerable<int?> CustomFields { get; set; }

	public ClassificationExplorerCustomFieldsItem(int classificatorId, string displayName, IEnumerable<int?> customFields)
	{
		ClassificatorId = classificatorId;
		DisplayName = displayName;
		CustomFields = customFields;
	}
}
