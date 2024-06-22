using DevExpress.XtraGrid.Views.BandedGrid;

namespace Dataedo.App.Tools;

public class ColumnDefaultValues
{
	public int Position { get; set; }

	public int Width { get; set; }

	public GridBand Band { get; set; }

	public ColumnDefaultValues(int position, int width)
	{
		Position = position;
		Width = width;
	}
}
