using System.Xml.Serialization;

namespace Dataedo.App.Tools.CommandLine.Common;

public class XmlTextObject
{
	[XmlText]
	public string Value { get; set; }

	[XmlAttribute]
	public bool IsEncrypted { get; set; }

	public XmlTextObject()
	{
	}

	public XmlTextObject(string value, bool isEncrypted)
	{
		Value = value;
		IsEncrypted = isEncrypted;
	}
}
