using System.Drawing;
using DevExpress.Utils.Helpers;
using DevExpress.XtraGrid.Views.WinExplorer;

namespace Dataedo.App.UserControls.ConnectAndSynchDataLakeForm.DataSourceControls.Tools;

internal static class WinExplorerViewTools
{
	public static Size GetItemSize(WinExplorerViewStyle viewStyle)
	{
		return viewStyle switch
		{
			WinExplorerViewStyle.ExtraLarge => new Size(256, 256), 
			WinExplorerViewStyle.Large => new Size(96, 96), 
			WinExplorerViewStyle.Content => new Size(32, 32), 
			WinExplorerViewStyle.Small => new Size(16, 16), 
			_ => new Size(96, 96), 
		};
	}

	public static IconSizeType GetIconSizeType(WinExplorerViewStyle viewStyle)
	{
		switch (viewStyle)
		{
		case WinExplorerViewStyle.ExtraLarge:
		case WinExplorerViewStyle.Large:
			return IconSizeType.ExtraLarge;
		case WinExplorerViewStyle.Small:
		case WinExplorerViewStyle.List:
			return IconSizeType.Small;
		case WinExplorerViewStyle.Medium:
		case WinExplorerViewStyle.Tiles:
		case WinExplorerViewStyle.Content:
			return IconSizeType.Large;
		default:
			return IconSizeType.ExtraLarge;
		}
	}
}
