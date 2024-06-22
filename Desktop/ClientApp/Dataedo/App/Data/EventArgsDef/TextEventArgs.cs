using System;

namespace Dataedo.App.Data.EventArgsDef;

public class TextEventArgs : EventArgs
{
	public string Text { get; set; }

	public TextEventArgs()
	{
	}

	public TextEventArgs(string text)
	{
		Text = text;
	}
}
