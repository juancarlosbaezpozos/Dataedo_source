using System;

namespace Dataedo.App.Data.EventArgsDef;

internal class GenericEventArgs<T> : EventArgs
{
	public T Value { get; set; }

	public GenericEventArgs(T value)
	{
		Value = value;
	}
}
