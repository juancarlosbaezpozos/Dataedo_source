using System.Drawing;
using System.Xml.Serialization;
using Dataedo.App.Tools.TemplateEditor;

namespace Dataedo.App.Documentation.Template.PdfTemplate.Model.Common;

public class ImageModel : Element
{
	[XmlElement(IsNullable = true)]
	public string Name { get; set; }

	public Image ImageValue { get; set; }

	[XmlElement(IsNullable = true)]
	public string Link { get; set; }

	public override void Initialize(ref int id, Element parent, string name = null)
	{
		base.Initialize(ref id, parent, name);
		Add(ref id, "Image", Name, delegate(object x)
		{
			Name = x?.ToString();
		}, ElementTypeEnum.ElementType.Path);
		Add(ref id, "Link", Link, delegate(object x)
		{
			Link = x?.ToString();
		}, ElementTypeEnum.ElementType.Link);
	}
}
