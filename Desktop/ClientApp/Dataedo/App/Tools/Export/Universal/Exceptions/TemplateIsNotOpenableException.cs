using System;

namespace Dataedo.App.Tools.Export.Universal.Exceptions;

internal class TemplateIsNotOpenableException : Exception
{
	public TemplateIsNotOpenableException(string msg)
		: base(msg)
	{
	}
}
