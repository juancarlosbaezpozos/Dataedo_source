using System.Xml.Serialization;
using Dataedo.App.Tools.TemplateEditor;

namespace Dataedo.App.Documentation.Template.PdfTemplate.Model.Common;

public class SimpleFontModel : Element
{
	[XmlElement]
	public FontModel Font { get; set; } = new FontModel();


	public override void Initialize(ref int id, Element parent, string name = null)
	{
		base.Initialize(ref id, parent, name);
		Add(ref id, Font, "Font");
	}
}
