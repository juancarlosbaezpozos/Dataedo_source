using System.Drawing;

namespace Dataedo.App.Classes;

public class LegendDocRow
{
	public Bitmap Icon { get; set; }

	public string Description { get; set; }

	public LegendDocRow(Bitmap icon, string description)
	{
		Icon = icon;
		Description = description;
	}
}
