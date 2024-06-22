using System.Drawing;
using System.Xml.Serialization;

namespace Dataedo.App.Documentation.Template.PdfTemplate.Model.Common;

public class ArgbColorModel
{
	[XmlElement]
	public int? A { get; set; }

	[XmlElement]
	public int? R { get; set; }

	[XmlElement]
	public int? G { get; set; }

	[XmlElement]
	public int? B { get; set; }

	[XmlIgnore]
	public Color? Color
	{
		get
		{
			if (!A.HasValue || !R.HasValue || !G.HasValue || !B.HasValue)
			{
				return null;
			}
			return System.Drawing.Color.FromArgb(A.Value, R.Value, G.Value, B.Value);
		}
	}

	public override string ToString()
	{
		return $"{A};{R};{G};{B}";
	}
}
