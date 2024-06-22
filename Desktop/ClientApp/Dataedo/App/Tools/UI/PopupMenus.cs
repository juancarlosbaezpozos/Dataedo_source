using System.Drawing;
using System.Linq;
using DevExpress.XtraBars;

namespace Dataedo.App.Tools.UI;

internal static class PopupMenus
{
	public static void StartGroupBeforeLastItem(this PopupMenu popupMenu)
	{
		if (popupMenu.ItemLinks.Count > 0 && popupMenu.ItemLinks.LastOrDefault().Links?[popupMenu.ItemLinks.Count - 1] != null)
		{
			popupMenu.ItemLinks.LastOrDefault().Links[popupMenu.ItemLinks.Count - 1].BeginGroup = true;
		}
	}

	public static void StartGroupBeforeLastItem(this PopupMenu popupMenu, ref bool condition)
	{
		if (!condition)
		{
			popupMenu.StartGroupBeforeLastItem();
			condition = true;
		}
	}

	public static void StartGroupBeforeLastItem(this BarSubItem barSubItem)
	{
		if (barSubItem.ItemLinks.Count > 0 && barSubItem.ItemLinks.LastOrDefault().Links?[barSubItem.ItemLinks.Count - 1] != null)
		{
			barSubItem.ItemLinks.LastOrDefault().Links[barSubItem.ItemLinks.Count - 1].BeginGroup = true;
		}
	}

	public static void AddItem(this PopupMenu popupMenu, string caption, Image image, ItemClickEventHandler itemClickEventHandler, bool isEnabled)
	{
		BarButtonItem barButtonItem = CreateBarButtonItem(caption, image, itemClickEventHandler);
		barButtonItem.Enabled = isEnabled;
		popupMenu.AddItem(barButtonItem);
	}

	public static void AddItem(this PopupMenu popupMenu, string caption, Image image, ItemClickEventHandler itemClickEventHandler)
	{
		popupMenu.AddItem(CreateBarButtonItem(caption, image, itemClickEventHandler));
	}

	public static BarSubItem AddSubItem(this PopupMenu popupMenu, string caption, Image image)
	{
		BarSubItem barSubItem = new BarSubItem();
		barSubItem.Caption = caption;
		barSubItem.ImageOptions.Image = image;
		popupMenu.AddItem(barSubItem);
		return barSubItem;
	}

	public static BarSubItem AddSubItem(this PopupMenu popupMenu, string caption)
	{
		return popupMenu.AddSubItem(caption, null);
	}

	public static void AddItem(this BarSubItem barSubItem, string caption, Image image, ItemClickEventHandler itemClickEventHandler)
	{
		barSubItem.AddItem(CreateBarButtonItem(caption, image, itemClickEventHandler));
	}

	private static BarButtonItem CreateBarButtonItem(string caption, Image image, ItemClickEventHandler itemClickEventHandler)
	{
		BarButtonItem barButtonItem = new BarButtonItem();
		barButtonItem.Caption = caption;
		barButtonItem.ImageOptions.Image = image;
		barButtonItem.ItemClick += itemClickEventHandler;
		return barButtonItem;
	}
}
