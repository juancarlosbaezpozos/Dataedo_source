using System.Windows.Forms;
using System.Xml.Serialization;

namespace Dataedo.App.Onboarding.Home.Model;

public class PaddingModel
{
	[XmlElement]
	public int Bottom { get; set; }

	[XmlElement]
	public int Left { get; set; }

	[XmlElement]
	public int Right { get; set; }

	[XmlElement]
	public int Top { get; set; }

	public PaddingModel()
	{
	}

	public PaddingModel(int bottom, int left, int right, int top)
	{
		Bottom = bottom;
		Left = left;
		Right = right;
		Top = top;
	}

	public Padding ToPadding()
	{
		return new Padding(Left, Top, Right, Bottom);
	}
}
