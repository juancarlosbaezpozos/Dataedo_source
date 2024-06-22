using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml.Serialization;

namespace Dataedo.App.Onboarding.Home.Model;

public class LinkWithImageModel : LinkModel
{
	[XmlElement]
	public string Base64Image { get; set; }

	[XmlIgnore]
	public Image Image
	{
		get
		{
			try
			{
				byte[] array = Convert.FromBase64String(Base64Image.Replace("data:image/png;base64,", ""));
				using MemoryStream stream = new MemoryStream(array, 0, array.Length);
				return Image.FromStream(stream, useEmbeddedColorManagement: true);
			}
			catch (Exception)
			{
				return null;
			}
		}
		set
		{
			using MemoryStream memoryStream = new MemoryStream();
			value.Save(memoryStream, ImageFormat.Png);
			Base64Image = Convert.ToBase64String(memoryStream.ToArray());
		}
	}

	public LinkWithImageModel()
	{
	}

	public LinkWithImageModel(string text, string link, string base64Image)
		: base(text, link)
	{
		Base64Image = base64Image;
	}

	public LinkWithImageModel(string text, string link, string base64Image, ToolTipModel toolTip)
		: base(text, link, toolTip)
	{
		Base64Image = base64Image;
	}
}
