using System.Drawing;

namespace Dataedo.App.UserControls.OverriddenControls;

public class ImageData
{
	public Image Image { get; set; }

	public string ToolTip { get; set; }

	public ImageData(Image image, string tooltip)
	{
		Image = image;
		ToolTip = tooltip;
	}
}
