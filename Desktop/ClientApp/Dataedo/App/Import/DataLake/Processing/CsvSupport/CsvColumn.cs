using System.Collections.Generic;

namespace Dataedo.App.Import.DataLake.Processing.CsvSupport;

public class CsvColumn
{
	public string HeaderName { get; set; }

	public HashSet<CsvDataType> FieldDataTypes { get; set; }

	public CsvDataType ColumnDataType { get; set; }

	public int FieldsCheckedCount { get; set; }

	public int Position { get; set; }

	public CsvColumn(string header, int position)
	{
		HeaderName = header;
		FieldDataTypes = new HashSet<CsvDataType>();
		Position = position;
	}

	public void AddFieldDataType(CsvDataType type)
	{
		FieldDataTypes.Add(type);
		FieldsCheckedCount++;
	}

	public void DetermineColumnType()
	{
		ColumnDataType = CsvDataTypeSupport.DetermineColumnType(FieldDataTypes);
	}
}
