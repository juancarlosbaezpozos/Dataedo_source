using System.Xml.Serialization;
using Dataedo.App.Tools.TemplateEditor;

namespace Dataedo.App.Documentation.Template.PdfTemplate.Model.ChildModels;

public class LocalizationModel : Element
{
	[XmlElement]
	public HeadingsNamesModel Headings { get; set; } = new HeadingsNamesModel();


	[XmlElement]
	public LegendNamesModel Legend { get; set; } = new LegendNamesModel();


	[XmlElement]
	public DataNamesModel Data { get; set; } = new DataNamesModel();


	public override void Initialize(ref int id, Element parent, string name = null)
	{
		base.Initialize(ref id, parent, name);
		Add(ref id, Headings, "Headings");
		Add(ref id, Legend, "Legend");
		Add(ref id, Data, "Data");
	}
}
