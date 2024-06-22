using System.Drawing;
using System.Xml.Serialization;
using Dataedo.App.Documentation.Template.PdfTemplate.Model.ChildModels;
using Dataedo.App.Documentation.Template.PdfTemplate.Model.Common;
using Dataedo.App.Tools.TemplateEditor;

namespace Dataedo.App.Documentation.Template.PdfTemplate.Model;

[XmlRoot("Template")]
public class PdfTemplateModel : BaseTemplateModel
{
	public class CustomizationModel : Element
	{
		public class GeneralModel : Element
		{
			[XmlElement(IsNullable = true)]
			public string FontFamily { get; set; }

			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, "Font family", FontFamily, delegate(object x)
				{
					FontFamily = x?.ToString();
				}, ElementTypeEnum.ElementType.Font);
			}
		}

		public class TitlePageModel : Element
		{
			[XmlElement]
			public TextModel Header { get; set; } = new TextModel();


			[XmlElement]
			public ImageModel Image { get; set; } = new ImageModel();


			[XmlElement]
			public TextModel Title { get; set; } = new TextModel();


			[XmlElement]
			public TextModel Subtitle { get; set; } = new TextModel();


			[XmlElement]
			public SimpleFontModelWithShow Date { get; set; } = new SimpleFontModelWithShow();


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, Header, "Header");
				Add(ref id, Image, "Image");
				Add(ref id, Title, "Title");
				Add(ref id, Subtitle, "Subtitle");
				Add(ref id, Date, "Date");
			}
		}

		public class TableOfContentsModel : Element
		{
			public class LevelModel : Element
			{
				[XmlElement]
				public FontModel Font { get; set; } = new FontModel();


				public override void Initialize(ref int id, Element parent, string name = null)
				{
					base.Initialize(ref id, parent, name);
					Add(ref id, Font, "Font");
				}
			}

			[XmlElement]
			public LevelModel Title { get; set; } = new LevelModel();


			[XmlElement]
			public LevelModel Level1 { get; set; } = new LevelModel();


			[XmlElement]
			public LevelModel Level2 { get; set; } = new LevelModel();


			[XmlElement]
			public LevelModel Level3 { get; set; } = new LevelModel();


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, Title, "Title");
				Add(ref id, Level1, "Level 1");
				Add(ref id, Level2, "Level 2");
				Add(ref id, Level3, "Level 3");
			}
		}

		public class LegendModel : Element
		{
			[XmlElement]
			public FontModel Font { get; set; } = new FontModel();


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, Font, "Font");
			}
		}

		public class FooterModel : Element
		{
			[XmlElement]
			public ComplexFooterElementModel Left { get; set; } = new ComplexFooterElementModel();


			[XmlElement]
			public ComplexFooterElementModel Center { get; set; } = new ComplexFooterElementModel();


			[XmlElement]
			public ComplexFooterElementModel Right { get; set; } = new ComplexFooterElementModel();


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, Left, "Left");
				Add(ref id, Center, "Center");
				Add(ref id, Right, "Right");
			}
		}

		public class TablesModel : Element
		{
			public class CellModel : Element
			{
				[XmlElement(IsNullable = true)]
				public ArgbColorModel BackgroundColor { get; set; } = new ArgbColorModel();


				[XmlElement(IsNullable = true)]
				public ArgbColorModel BorderColor { get; set; } = new ArgbColorModel();


				[XmlElement]
				public FontModel Font { get; set; } = new FontModel();


				public override void Initialize(ref int id, Element parent, string name = null)
				{
					base.Initialize(ref id, parent, name);
					Add(ref id, "Background color", (BackgroundColor ?? new ArgbColorModel()).Color, delegate(object x)
					{
						if (x is Color)
						{
							Color color2 = (Color)x;
							BackgroundColor = new ArgbColorModel
							{
								A = color2.A,
								R = color2.R,
								G = color2.G,
								B = color2.B
							};
						}
					}, ElementTypeEnum.ElementType.Color);
					Add(ref id, "Border color", (BorderColor ?? new ArgbColorModel()).Color, delegate(object x)
					{
						if (x is Color)
						{
							Color color = (Color)x;
							BorderColor = new ArgbColorModel
							{
								A = color.A,
								R = color.R,
								G = color.G,
								B = color.B
							};
						}
					}, ElementTypeEnum.ElementType.Color);
					Add(ref id, Font, "Font");
				}
			}

			[XmlElement]
			public CellModel Header { get; set; } = new CellModel();


			[XmlElement]
			public CellModel Body { get; set; } = new CellModel();


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, Header, "Header");
				Add(ref id, Body, "Body");
			}
		}

		public class HeadersModel : Element
		{
			public class HeaderModel : Element
			{
				[XmlElement]
				public FontModel Font { get; set; } = new FontModel();


				public override void Initialize(ref int id, Element parent, string name = null)
				{
					base.Initialize(ref id, parent, name);
					Add(ref id, Font, "Font");
				}
			}

			[XmlElement]
			public HeaderModel Heading1 { get; set; } = new HeaderModel();


			[XmlElement]
			public HeaderModel Heading2 { get; set; } = new HeaderModel();


			[XmlElement]
			public HeaderModel Heading3 { get; set; } = new HeaderModel();


			[XmlElement]
			public HeaderModel Heading4 { get; set; } = new HeaderModel();


			public override void Initialize(ref int id, Element parent, string name = null)
			{
				base.Initialize(ref id, parent, name);
				Add(ref id, Heading1, "Heading 1");
				Add(ref id, Heading2, "Heading 2");
				Add(ref id, Heading3, "Heading 3");
				Add(ref id, Heading4, "Heading 4");
			}
		}

		[XmlElement]
		public GeneralModel General { get; set; } = new GeneralModel();


		[XmlElement]
		public TitlePageModel TitlePage { get; set; } = new TitlePageModel();


		[XmlElement]
		public TableOfContentsModel TableOfContents { get; set; } = new TableOfContentsModel();


		[XmlElement]
		public LegendModel Legend { get; set; } = new LegendModel();


		[XmlElement]
		public FooterModel Footer { get; set; } = new FooterModel();


		[XmlElement]
		public TablesModel Tables { get; set; } = new TablesModel();


		[XmlElement]
		public HeadersModel Headings { get; set; } = new HeadersModel();


		[XmlIgnore]
		public LocalizationModel Localization { get; set; } = new LocalizationModel();


		public override void Initialize(ref int id, Element parent, string name = null)
		{
			base.Initialize(ref id, parent, name);
			Add(ref id, General, "General");
			Add(ref id, Legend, "Legend");
			Add(ref id, TitlePage, "Title page");
			Add(ref id, TableOfContents, "Table of contents");
			Add(ref id, Footer, "Footer");
			Add(ref id, Tables, "Tables");
			Add(ref id, Headings, "Headings");
		}
	}

	[XmlElement]
	public TemplatePDFSubtypeEnum.TemplatePDFSubtype Subtype { get; set; }

	[XmlElement]
	public CustomizationModel Customization { get; set; } = new CustomizationModel();


	public override void Initialize(ref int id, Element parent, string name = null)
	{
		base.Initialize(ref id, null, name);
		Add(ref id, "Template description", base.Description, delegate(object x)
		{
			base.Description = x?.ToString();
		});
		Add(ref id, "Type", (base.Type == TemplateTypeEnum.TemplateType.Undefined) ? TemplateTypeEnum.TemplateType.PDF : base.Type, delegate(object x)
		{
			base.Type = TemplateTypeEnum.StringToType(x?.ToString());
		}, ElementTypeEnum.ElementType.ReadOnly);
		Add(ref id, "Subtype", (Subtype == TemplatePDFSubtypeEnum.TemplatePDFSubtype.Undefined) ? TemplatePDFSubtypeEnum.TemplatePDFSubtype.Detailed : Subtype, delegate(object x)
		{
			Subtype = TemplatePDFSubtypeEnum.StringToType(x?.ToString());
		}, ElementTypeEnum.ElementType.ReadOnly);
		Add(ref id, Customization, "Customization");
	}
}
