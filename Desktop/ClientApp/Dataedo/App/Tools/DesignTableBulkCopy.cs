using DevExpress.XtraGrid.Columns;

namespace Dataedo.App.Tools;

public class DesignTableBulkCopy : BaseBulkCopy, IBulkCopy
{
	public override object GetProperValue(string value, GridColumn column)
	{
		if (string.IsNullOrEmpty(value))
		{
			if (column.RealColumnEdit.EditorTypeName.Equals("CheckEdit"))
			{
				return false;
			}
			return string.Empty;
		}
		if (column.RealColumnEdit.EditorTypeName.Equals("TextEdit"))
		{
			if (column.FieldName.ToLower().Equals("title") && value.Length > 80)
			{
				return GetShortenedValue(value, 79);
			}
			if (column.FieldName.Equals("DataLength") && value.Length > 50)
			{
				return GetShortenedValue(value, 49);
			}
			if (column.FieldName.Equals("DataType") && value.Length > 250)
			{
				return GetShortenedValue(value, 249);
			}
			if (column.FieldName.Equals("Name") && value.Length > 80)
			{
				return GetShortenedValue(value, 79);
			}
			return GetMultilineValues(value);
		}
		if (column.RealColumnEdit.EditorTypeName.Equals("CheckEdit"))
		{
			return CheckboxValues.IsPositive(value);
		}
		return value;
	}
}
