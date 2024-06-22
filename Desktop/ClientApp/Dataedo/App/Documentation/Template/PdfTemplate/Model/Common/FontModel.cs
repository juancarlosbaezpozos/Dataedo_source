using System.Drawing;
using System.Xml.Serialization;
using Dataedo.App.Tools.TemplateEditor;

namespace Dataedo.App.Documentation.Template.PdfTemplate.Model.Common;

public class FontModel : Element
{
	[XmlElement(IsNullable = true)]
	public string FontFamily { get; set; }

	[XmlElement(IsNullable = true)]
	public decimal? Size { get; set; }

	[XmlElement(IsNullable = true)]
	public bool? Bold { get; set; }

	[XmlElement(IsNullable = true)]
	public bool? Italic { get; set; }

	[XmlElement(IsNullable = true)]
	public bool? Underline { get; set; }

	[XmlElement(IsNullable = true)]
	public bool? Strikethrough { get; set; }

	[XmlElement(IsNullable = true)]
	public ArgbColorModel Color { get; set; } = new ArgbColorModel();


	public FontFamily FamilyValue
	{
		get
		{
			if (!string.IsNullOrEmpty(FontFamily))
			{
				return new FontFamily(FontFamily);
			}
			return null;
		}
	}

	public override void Initialize(ref int id, Element parent, string name = null)
	{
		base.Initialize(ref id, parent, name);
		Add(ref id, "Font family", FontFamily, delegate(object x)
		{
			FontFamily = x?.ToString();
		}, ElementTypeEnum.ElementType.Font);
		Add(ref id, "Size", Size, delegate(object x)
		{
			Size = x as decimal?;
		}, ElementTypeEnum.ElementType.Decimal);
		Add(ref id, "Bold", Bold, delegate(object x)
		{
			Bold = x as bool?;
		}, ElementTypeEnum.ElementType.Bool);
		Add(ref id, "Italic", Italic, delegate(object x)
		{
			Italic = x as bool?;
		}, ElementTypeEnum.ElementType.Bool);
		Add(ref id, "Underline", Underline, delegate(object x)
		{
			Underline = x as bool?;
		}, ElementTypeEnum.ElementType.Bool);
		Add(ref id, "Strikethrough", Strikethrough, delegate(object x)
		{
			Strikethrough = x as bool?;
		}, ElementTypeEnum.ElementType.Bool);
		Add(ref id, "Color", (Color ?? new ArgbColorModel()).Color, delegate(object x)
		{
			if (x is Color)
			{
				Color color = (Color)x;
				Color = new ArgbColorModel
				{
					A = color.A,
					R = color.R,
					G = color.G,
					B = color.B
				};
			}
		}, ElementTypeEnum.ElementType.Color);
	}

	public override string ToString()
	{
		return (FontFamily ?? "NULL") + "; " + (Size?.ToString() ?? "NULL ") + "; " + (this?.Color?.ToString() ?? "NULL");
	}
}
