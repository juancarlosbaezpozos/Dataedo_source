using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.MenuTree;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Shared.Enums;

namespace Dataedo.App.UserControls.ObjectBrowser;

public class DataLineageObjectBrowserUserControl : ObjectBrowserUserControl
{
	private Dictionary<int, List<ObjectIdWithType>> processesSuggestedObjects;

	private int? selectedProcessId;

	public void SetParameters(DBTreeNode dbTreeNode, List<int> processesIDs)
	{
		selectedProcessId = null;
		processesSuggestedObjects = new Dictionary<int, List<ObjectIdWithType>>();
		if (processesIDs != null && processesIDs.Any())
		{
			foreach (int processesID in processesIDs)
			{
				processesSuggestedObjects.Add(processesID, null);
			}
		}
		SetParameters(dbTreeNode);
	}

	protected override void SetAdditionalSuggestions(List<ObjectBrowserItem> nodes, ref bool wasSuggestionFound)
	{
		if (!selectedProcessId.HasValue || !processesSuggestedObjects.ContainsKey(selectedProcessId.Value))
		{
			return;
		}
		List<ObjectIdWithType> suggestions = processesSuggestedObjects[selectedProcessId.Value];
		List<ObjectIdWithType> list = suggestions;
		if (list == null || !list.Any())
		{
			return;
		}
		foreach (ObjectBrowserItem item in nodes.Where((ObjectBrowserItem x) => suggestions.Any((ObjectIdWithType r) => r.ObjectId == x.Id && r.ObjectType == SharedObjectTypeEnum.TypeToString(x.ObjectType))))
		{
			item.IsSuggested = true;
			wasSuggestionFound = true;
		}
	}

	public void UpdateSuggestionsForProcess(int? processId)
	{
		if (!processId.HasValue || !processesSuggestedObjects.ContainsKey(processId.Value))
		{
			selectedProcessId = null;
			SetDataSource();
			return;
		}
		selectedProcessId = processId;
		if (processesSuggestedObjects[processId.Value] == null)
		{
			GetSuggestedTableIDsByProcessScript(processId.Value);
		}
		SetDataSource();
	}

	private void GetSuggestedTableIDsByProcessScript(int processId)
	{
		processesSuggestedObjects[processId] = DB.ObjectBrowser.GetObjectRelatedTableIDsByProcessScript(processId);
	}
}
