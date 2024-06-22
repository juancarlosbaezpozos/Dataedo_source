using System;

namespace Dataedo.App.Tools.Export.Universal.Exceptions;

internal class UnrecognizedTemplateTransformerException : Exception
{
	public UnrecognizedTemplateTransformerException(string msg)
		: base(msg)
	{
	}
}
