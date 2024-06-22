using System.Xml.Serialization;
using Dataedo.App.Tools.TemplateEditor;

namespace Dataedo.App.Documentation.Template.PdfTemplate.Model.Common;

public class SimpleFontModelWithShow : SimpleFontModel
{
	[XmlElement(IsNullable = true)]
	public bool? Show { get; set; }

	public override void Initialize(ref int id, Element parent, string name = null)
	{
		base.Initialize(ref id, parent, name);
		Add(ref id, "Show", Show, delegate(object x)
		{
			Show = x as bool?;
		}, ElementTypeEnum.ElementType.Bool);
	}
}
