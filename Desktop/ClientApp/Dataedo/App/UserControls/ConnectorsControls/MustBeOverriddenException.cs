using System;

namespace Dataedo.App.UserControls.ConnectorsControls;

[Serializable]
internal class MustBeOverriddenException : Exception
{
	public MustBeOverriddenException()
	{
	}

	public MustBeOverriddenException(string methodName)
		: base("Method '" + methodName + "' must be overridden")
	{
	}
}
