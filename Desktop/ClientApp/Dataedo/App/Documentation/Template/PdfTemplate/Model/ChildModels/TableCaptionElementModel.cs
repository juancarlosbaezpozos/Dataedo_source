using System.Xml.Serialization;
using Dataedo.App.Tools.TemplateEditor;

namespace Dataedo.App.Documentation.Template.PdfTemplate.Model.ChildModels;

public class TableCaptionElementModel : Element
{
	[XmlElement]
	public string Caption { get; set; }

	public TableCaptionElementModel()
	{
	}

	public TableCaptionElementModel(string caption)
	{
		Caption = caption;
	}

	public override void Initialize(ref int id, Element parent, string name = null)
	{
		base.Initialize(ref id, parent, name);
		Add(ref id, "Caption", Caption, delegate(object x)
		{
			Caption = x?.ToString();
		});
	}
}
