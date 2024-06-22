using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using DevExpress.Utils.Helpers;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base.ObjectModel;

public class ObjectItem
{
	public static Size ItemSize { get; private set; } = new Size(16, 16);


	public string FullName { get; private set; }

	public string Name { get; private set; }

	public string DisplayName { get; private set; }

	public virtual Image Image { get; private set; }

	public string TypeDisplayName { get; set; }

	public ObjectItem(FileInfo fileInfo)
	{
		FullName = fileInfo.FullName;
		Name = fileInfo.Name;
		DisplayName = fileInfo.Name;
		Image = GetImage(FullName);
	}

	protected Image GetImage(string fullName)
	{
		Image image = FileSystemHelper.GetImage(fullName, IconSizeType.Small, ItemSize);
		if (image.Size != ItemSize)
		{
			Bitmap bitmap = new Bitmap(ItemSize.Width, ItemSize.Height);
			using Graphics graphics = Graphics.FromImage(bitmap);
			graphics.InterpolationMode = InterpolationMode.High;
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.DrawImage(image, 0, 0, ItemSize.Width, ItemSize.Height);
			return bitmap;
		}
		return image;
	}
}
