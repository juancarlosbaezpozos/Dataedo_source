using System;
using Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base.HierarchyModel;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.Base.EventArgs;

public class SelectedItemChangedEventArgs : System.EventArgs
{
	public Item Item { get; }

	public SelectedItemChangedEventArgs(Item item)
	{
		Item = item;
	}
}
