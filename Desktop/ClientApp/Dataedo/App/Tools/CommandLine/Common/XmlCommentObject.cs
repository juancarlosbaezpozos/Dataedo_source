using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Tools;

namespace Dataedo.App.Tools.CommandLine.Common;

[XmlObjectComment]
[XmlRoot]
public class XmlCommentObject : IXmlSerializable
{
	[XmlIgnore]
	public string Comment { get; set; }

	public XmlCommentObject()
	{
	}

	public XmlCommentObject(string comment)
	{
		Comment = comment;
	}

	public XmlSchema GetSchema()
	{
		return null;
	}

	public void ReadXml(XmlReader reader)
	{
	}

	public void WriteXml(XmlWriter writer)
	{
		writer.WriteComment(" " + Regex.Replace(Comment, "-+", "-") + " ");
	}
}
