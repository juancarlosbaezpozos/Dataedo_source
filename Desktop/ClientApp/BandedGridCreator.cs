using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;

public class BandedGridCreator
{
    public virtual GridBand GetBand(string title)
    {
        GridBand gridBand = new GridBand();
        gridBand.Caption = title;
        gridBand.Visible = true;
        gridBand.Tag = title;
        gridBand.OptionsBand.AllowMove = false;
        return gridBand;
    }

    public BandedGridColumn GetColumn(string caption, string fieldName)
    {
        BandedGridColumn bandedGridColumn = new BandedGridColumn();
        bandedGridColumn.Caption = caption;
        bandedGridColumn.FieldName = fieldName;
        bandedGridColumn.Visible = true;
        bandedGridColumn.OptionsColumn.AllowMove = false;
        return bandedGridColumn;
    }

    public BandedGridColumn GetReadonlyColumn(string caption, string fieldName)
    {
        BandedGridColumn bandedGridColumn = new BandedGridColumn();
        bandedGridColumn.Caption = caption;
        bandedGridColumn.FieldName = fieldName;
        bandedGridColumn.Visible = true;
        bandedGridColumn.OptionsColumn.ReadOnly = true;
        bandedGridColumn.OptionsColumn.AllowMove = false;
        bandedGridColumn.OptionsFilter.AutoFilterCondition = AutoFilterCondition.Contains;
        return bandedGridColumn;
    }
}
