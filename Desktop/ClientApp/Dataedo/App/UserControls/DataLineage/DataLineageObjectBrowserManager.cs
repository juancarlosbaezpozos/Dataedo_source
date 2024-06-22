using Dataedo.App.UserControls.ObjectBrowser;

namespace Dataedo.App.UserControls.DataLineage;

public class DataLineageObjectBrowserManager : ObjectBrowserManager
{
	protected override bool IsObjectAllowedAsProposed(ObjectBrowserItem objectBrowserItem)
	{
		string tooltipText = null;
		return DataFlowHelper.CanBeDropped(objectBrowserItem.ObjectType, currentObjectTreeNode.ObjectType, null, null, ref tooltipText);
	}
}
