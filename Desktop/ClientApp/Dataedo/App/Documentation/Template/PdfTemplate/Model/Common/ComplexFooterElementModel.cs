using System.Xml.Serialization;
using Dataedo.App.Tools.TemplateEditor;

namespace Dataedo.App.Documentation.Template.PdfTemplate.Model.Common;

public class ComplexFooterElementModel : Element
{
	public class PageNumberModel : Element
	{
		[XmlElement(IsNullable = true)]
		public bool? Show { get; set; }

		[XmlElement]
		public FontModel Font { get; set; } = new FontModel();


		public override void Initialize(ref int id, Element parent, string name = null)
		{
			base.Initialize(ref id, parent, name);
			Add(ref id, "Show", Show, delegate(object x)
			{
				Show = x as bool?;
			}, ElementTypeEnum.ElementType.Bool);
			Add(ref id, Font, "Font");
		}
	}

	[XmlElement]
	public ImageModel Image { get; set; } = new ImageModel();


	[XmlElement]
	public TextModel Text { get; set; } = new TextModel();


	[XmlElement]
	public PageNumberModel PageNumber { get; set; } = new PageNumberModel();


	public override void Initialize(ref int id, Element parent, string name = null)
	{
		base.Initialize(ref id, parent, name);
		Add(ref id, Image, "Image");
		Add(ref id, Text, "Text");
		Add(ref id, PageNumber, "Page number");
	}
}
