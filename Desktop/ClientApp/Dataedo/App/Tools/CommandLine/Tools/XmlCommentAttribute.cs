using System;

namespace Dataedo.App.Tools.CommandLine.Tools;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class XmlCommentAttribute : Attribute
{
	public string Value { get; set; }

	public XmlCommentAttribute(string value)
	{
		Value = value;
	}
}
