using System.Drawing;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;

namespace Dataedo.App.Helpers.Controls;

public class BandedGridViewHelpers
{
	public static void DrawVerticalLinesBetweenBands(BandedGridView bandedGridView, PaintExEventArgs e)
	{
		try
		{
			GridBandInfoCollection gridBandInfoCollection = (bandedGridView?.GetViewInfo() as BandedGridViewInfo)?.BandsInfo;
			if (gridBandInfoCollection != null)
			{
				for (int i = 1; i < gridBandInfoCollection.Count; i++)
				{
					GridBandInfoArgs gridBandInfoArgs = gridBandInfoCollection[i];
					e.Cache.DrawLine(new Point(gridBandInfoArgs.Bounds.X - 1, gridBandInfoArgs.Bounds.Y), new Point(gridBandInfoArgs.Bounds.X - 1, gridBandInfoArgs.Bounds.Y + bandedGridView.GridControl.Height), bandedGridView.PaintAppearance.VertLine.BackColor, 1);
				}
			}
		}
		catch
		{
		}
	}
}
