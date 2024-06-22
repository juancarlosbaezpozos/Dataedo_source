using System;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.EventArgsDef;

public class ProfilingVisibilityEventArgs : EventArgs
{
	public bool SingleTableButtonsVisible { get; set; }

	public SharedObjectTypeEnum.ObjectType? SingleObjectType { get; set; }

	public bool DataProfilingButtonVisible { get; set; }

	public ProfilingVisibilityEventArgs(bool singleObjectButtonsVisible, SharedObjectTypeEnum.ObjectType? singleObjectType, bool dataProfilingButtonVisible)
	{
		SingleTableButtonsVisible = singleObjectButtonsVisible;
		SingleObjectType = singleObjectType;
		DataProfilingButtonVisible = dataProfilingButtonVisible;
	}
}
