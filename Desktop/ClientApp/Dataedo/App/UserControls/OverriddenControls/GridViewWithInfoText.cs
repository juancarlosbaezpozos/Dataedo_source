using System.Drawing;
using System.Linq;
using Dataedo.App.UserControls.DataLineage;
using DevExpress.Utils.Extensions;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Dataedo.App.UserControls.OverriddenControls;

public class GridViewWithInfoText : GridView
{
	private string infoText;

	protected override void OnLoaded()
	{
		base.OnLoaded();
		base.GridControl.PaintEx += delegate(object sender, PaintExEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(infoText) && GetViewInfo() is GridViewInfo gridViewInfo)
			{
				if (gridViewInfo.RowsInfo.Count == 0)
				{
					ControlsUtils.AddInfoText(e.Cache, ViewRect.TopBoundRect(0), infoText, 80);
				}
				else
				{
					GridRowInfo gridRowInfo = gridViewInfo.RowsInfo.Last();
					if (gridRowInfo != null)
					{
						Rectangle bounds = gridRowInfo.Bounds;
						ControlsUtils.AddInfoText(e.Cache, bounds.WithY(bounds.Y + bounds.Height), infoText, gridRowInfo.ViewInfo?.ColumnRowHeight ?? 0);
					}
				}
			}
		};
	}

	public void SetInfoText(string text)
	{
		infoText = text;
	}
}
