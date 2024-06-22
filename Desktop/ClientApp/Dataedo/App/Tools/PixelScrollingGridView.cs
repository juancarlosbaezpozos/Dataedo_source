using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Tools;

internal class PixelScrollingGridView : GridView
{
	protected override bool IsAllowPixelScrollingAutoRowHeight => true;

	public PixelScrollingGridView()
		: base(null)
	{
		base.OptionsBehavior.AllowPixelScrolling = DefaultBoolean.True;
	}

	public PixelScrollingGridView(GridControl grid)
		: base(grid)
	{
		base.OptionsBehavior.AllowPixelScrolling = DefaultBoolean.True;
	}
}
