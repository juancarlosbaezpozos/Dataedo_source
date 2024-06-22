using System;
using System.Xml;
using System.Xml.Serialization;
using Dataedo.App.Tools.TemplateEditor;

namespace Dataedo.App.Documentation.Template;

[XmlRoot("Template")]
public class BaseTemplateModel : Element, IBaseTemplateModel
{
	[XmlElement(IsNullable = true)]
	public string Name { get; set; }

	[XmlIgnore]
	public string Description { get; set; }

	[XmlElement("Description", IsNullable = true)]
	public XmlCDataSection DescriptionCDATA
	{
		get
		{
			return new XmlDocument().CreateCDataSection(Description);
		}
		set
		{
			Description = value.Value;
		}
	}

	[XmlElement]
	public TemplateTypeEnum.TemplateType Type { get; set; }

	[XmlElement(IsNullable = true)]
	public bool? IsCustomizable { get; set; }

	[XmlIgnore]
	public Exception ExceptionValue { get; set; }

	[XmlIgnore]
	public string CoreTemplateFilePath => DocTemplateFile.GetCoreTemplateFilePath(this);
}
