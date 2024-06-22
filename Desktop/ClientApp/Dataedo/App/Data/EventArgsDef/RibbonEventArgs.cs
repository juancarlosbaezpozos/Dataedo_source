using System;
using Dataedo.App.MenuTree;

namespace Dataedo.App.Data.EventArgsDef;

internal class RibbonEventArgs : EventArgs
{
	public bool? Value { get; set; }

	public DBTreeNode Node { get; }

	public RibbonEventArgs(bool value, DBTreeNode node)
	{
		Value = value;
		Node = node;
	}
}
