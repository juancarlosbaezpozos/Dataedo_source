using System.Xml.Serialization;
using Dataedo.App.Tools.TemplateEditor;

namespace Dataedo.App.Documentation.Template.PdfTemplate.Model.Common;

public class TextModel : SimpleFontModel
{
	[XmlElement(IsNullable = true)]
	public string Text { get; set; }

	[XmlElement(IsNullable = true)]
	public string Link { get; set; }

	public override void Initialize(ref int id, Element parent, string name = null)
	{
		base.Initialize(ref id, parent, name);
		Add(ref id, "Text", Text, delegate(object x)
		{
			Text = x?.ToString();
		});
		Add(ref id, "Link", Link, delegate(object x)
		{
			Link = x?.ToString();
		}, ElementTypeEnum.ElementType.Link);
	}
}
