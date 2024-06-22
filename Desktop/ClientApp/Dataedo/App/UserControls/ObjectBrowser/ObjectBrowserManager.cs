using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.MenuTree;

namespace Dataedo.App.UserControls.ObjectBrowser;

public class ObjectBrowserManager
{
	protected DBTreeNode currentObjectTreeNode;

	internal List<ObjectBrowserItem> ProposedObjects { get; private set; }

	public bool ShowObjectsFromAllDbs { get; set; }

	public ObjectBrowserManager()
	{
		ProposedObjects = new List<ObjectBrowserItem>();
	}

	public void SetParameters(DBTreeNode currentObjectTreeNode, List<int> databaseIDs)
	{
		this.currentObjectTreeNode = currentObjectTreeNode;
		ReloadObjects(databaseIDs);
	}

	internal void ReloadObjects(List<int> databaseIDs)
	{
		ProposedObjects = new List<ObjectBrowserItem>();
		if (currentObjectTreeNode == null)
		{
			return;
		}
		IEnumerable<ObjectBrowserItem> enumerable = null;
		enumerable = ((!ShowObjectsFromAllDbs) ? DB.ObjectBrowser.GetObjectsForObjectBrowser(databaseIDs) : DB.ObjectBrowser.GetObjectsForObjectBrowser());
		if (enumerable != null)
		{
			ProposedObjects.AddRange(enumerable.Where((ObjectBrowserItem x) => IsObjectAllowedAsProposed(x)));
		}
		ProposedObjects.ForEach(delegate(ObjectBrowserItem x)
		{
			x.SetParentDatabase(currentObjectTreeNode?.DatabaseId);
		});
	}

	protected virtual bool IsObjectAllowedAsProposed(ObjectBrowserItem objectBrowserItem)
	{
		return true;
	}
}
