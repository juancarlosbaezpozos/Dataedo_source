using DevExpress.XtraRichEdit.API.Native;

namespace Dataedo.App.Tools.UI;

public static class DocumentPropertiesExtensions
{
	public static void InvertColors(this CharacterPropertiesBase properties)
	{
		properties.HighlightColor = properties.HighlightColor.Invert();
		properties.BackColor = properties.BackColor.Invert();
		properties.ForeColor = properties.ForeColor.Invert();
		properties.UnderlineColor = properties.UnderlineColor.Invert();
	}

	public static void InvertTextColors(this ParagraphProperties properties)
	{
		properties.BackColor = properties.BackColor.Invert();
	}
}
