using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Utils.Helpers;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.SelectingDataControls.Base.HierarchyModel;

public abstract class Item
{
	public static Size ItemSize { get; private set; } = new Size(16, 16);


	public string FullName { get; private set; }

	public string Name { get; private set; }

	public string DisplayName { get; private set; }

	public virtual Image Image { get; protected set; }

	public Item(string fullName)
	{
		FullName = fullName ?? throw new ArgumentNullException("fullName");
		Name = GetName(fullName);
		DisplayName = GetDisplayName(fullName);
		Image = GetImage(fullName);
	}

	public abstract List<Item> GetChildItems();

	protected virtual string GetDisplayName(string fullName)
	{
		return Name;
	}

	protected virtual string GetName(string fullName)
	{
		return fullName;
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
