using System;
using System.Collections.Generic;

namespace Dataedo.App.Import.DataLake.Processing.CsvSupport;

internal class CsvDataTypeSupport
{
	public static CsvDataType DetermineColumnType(HashSet<CsvDataType> column)
	{
		if (column.Count == 0)
		{
			return CsvDataType.Null;
		}
		if (column.Contains(CsvDataType.String))
		{
			return CsvDataType.String;
		}
		if (column.Contains(CsvDataType.DateTime) && !column.Contains(CsvDataType.Float) && !column.Contains(CsvDataType.Int) && !column.Contains(CsvDataType.Bool))
		{
			return CsvDataType.DateTime;
		}
		if (column.Contains(CsvDataType.Float) && !column.Contains(CsvDataType.DateTime))
		{
			return CsvDataType.Float;
		}
		if (column.Contains(CsvDataType.Int) && !column.Contains(CsvDataType.DateTime))
		{
			return CsvDataType.Int;
		}
		if (column.Contains(CsvDataType.Bool) && !column.Contains(CsvDataType.DateTime))
		{
			return CsvDataType.Bool;
		}
		if (column.Contains(CsvDataType.Null) && column.Count == 1)
		{
			return CsvDataType.Null;
		}
		return CsvDataType.String;
	}

	public static CsvDataType DetermineFieldType(string field)
	{
		if (string.IsNullOrWhiteSpace(field))
		{
			return CsvDataType.Null;
		}
		if (field == "1" || field == "0")
		{
			return CsvDataType.Bool;
		}
		if (int.TryParse(field, out var _))
		{
			return CsvDataType.Int;
		}
		if (double.TryParse(field, out var _))
		{
			return CsvDataType.Float;
		}
		if (DateTime.TryParse(field, out var _))
		{
			return CsvDataType.DateTime;
		}
		return CsvDataType.String;
	}
}
