using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Dataedo.App.UserControls.DataLineage;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;

namespace Dataedo.App.UserControls.OverriddenControls;

public class TileViewWithInfoText : TileView
{
	private string infoText;

	protected override void OnLoaded()
	{
		base.OnLoaded();
		base.GridControl.PaintEx += delegate(object sender, PaintExEventArgs e)
		{
			try
			{
				if (!string.IsNullOrWhiteSpace(infoText))
				{
					Dictionary<int, TileViewItem> dictionary = (((ITileControl)GetViewInfo())?.ViewInfo as TileViewInfoCore)?.VisibleItems;
					if (dictionary == null || !dictionary.Any())
					{
						ControlsUtils.AddInfoText(e.Cache, ViewRect.TopBoundRect(0), infoText, 40);
					}
					else
					{
						TileViewItem tileViewItem = (from x in dictionary
							orderby x.Key
							select x.Value).Last();
						if (tileViewItem != null)
						{
							Rectangle bounds = tileViewItem.ItemInfo.Bounds;
							ControlsUtils.AddInfoText(e.Cache, bounds.WithY(bounds.Y + bounds.Height), infoText, tileViewItem.ItemInfo.Size.Height);
						}
					}
				}
			}
			catch
			{
			}
		};
	}

	public void SetInfoText(string text)
	{
		infoText = text;
	}
}
