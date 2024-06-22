using System;
using Dataedo.App.LoginFormTools.Tools.Enums;

namespace Dataedo.App.LoginFormTools.Tools.CustomEventArgs;

public class ActionEventArgs : EventArgs
{
	public new static readonly ActionEventArgs Empty = new ActionEventArgs(ActionResultEnum.ActionResult.None);

	public ActionResultEnum.ActionResult ActionResult { get; set; }

	public object Parameter { get; set; }

	public ActionEventArgs(ActionResultEnum.ActionResult actionResult)
	{
		ActionResult = actionResult;
	}

	public ActionEventArgs(ActionResultEnum.ActionResult actionResult, object parameter)
	{
		ActionResult = actionResult;
		Parameter = parameter;
	}
}
