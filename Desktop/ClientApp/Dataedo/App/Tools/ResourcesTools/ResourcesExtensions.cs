using System.IO;
using DevExpress.Utils.Svg;

namespace Dataedo.App.Tools.ResourcesTools;

public static class ResourcesExtensions
{
	public static SvgImage AsSvg(this byte[] svgAsBytes)
	{
		using MemoryStream stream = new MemoryStream(svgAsBytes);
		return new SvgImage(stream);
	}
}
