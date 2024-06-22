using System;
using System.Collections.Generic;

namespace Dataedo.App.MenuTree;

public static class Extensions
{
	public static IEnumerable<DBTreeNode> Flatten<DBTreeNode>(this IEnumerable<DBTreeNode> sequence, Func<DBTreeNode, IEnumerable<DBTreeNode>> childFetcher)
	{
		Queue<DBTreeNode> itemsToYield = new Queue<DBTreeNode>(sequence);
		while (itemsToYield.Count > 0)
		{
			DBTreeNode item = itemsToYield.Dequeue();
			yield return item;
			IEnumerable<DBTreeNode> enumerable = childFetcher(item);
			if (enumerable == null)
			{
				continue;
			}
			foreach (DBTreeNode item2 in enumerable)
			{
				itemsToYield.Enqueue(item2);
			}
		}
	}
}
