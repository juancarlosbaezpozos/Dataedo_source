using DevExpress.XtraGrid.Views.BandedGrid;

public class ClassificatorBandedGridCreator : BandedGridCreator
{
    public override GridBand GetBand(string title)
    {
        GridBand gridBand = new GridBand();
        gridBand.Caption = title;
        gridBand.Visible = true;
        gridBand.Tag = title;
        gridBand.OptionsBand.AllowMove = false;
        return gridBand;
    }
}
