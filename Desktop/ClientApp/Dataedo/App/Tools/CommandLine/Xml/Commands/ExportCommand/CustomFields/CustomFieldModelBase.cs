using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ObjectTypes.Base;

namespace Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.CustomFields;

public class CustomFieldModelBase : ExportModelBase
{
	[XmlAnyElement(Name = "CustomFieldIdComment")]
	public XmlCommentObject CustomFieldIdComment { get; set; }

	[XmlElement]
	public int CustomFieldId { get; set; }

	public int CustomFieldIdValue => CustomFieldId;

	public CustomFieldModelBase()
	{
	}

	public CustomFieldModelBase(int id, bool export)
	{
		CustomFieldId = id;
		base.Export = export;
	}

	public CustomFieldModelBase(int id, bool export, string name)
	{
		CustomFieldId = id;
		base.Export = export;
		CustomFieldIdComment = new XmlCommentObject(name);
	}
}
