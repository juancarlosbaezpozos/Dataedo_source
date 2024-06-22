using System.Collections.Generic;
using System.Windows.Forms;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Forms.Tools;

public static class ColumnsHelper
{
	public static readonly int MaxColumnsCount;

	public static readonly int MaxColumnsSort;

	private static readonly List<string> columnsToBlock;

	private static readonly string[] dataTypesStrings;

	public static AutoCompleteStringCollection DataTypes { get; }

	static ColumnsHelper()
	{
		MaxColumnsCount = 1000;
		MaxColumnsSort = 99999;
		columnsToBlock = new List<string>
		{
			"DisplayPosition", "Type", "Name", "DataTypeForDisplay", "Size", "Nullable", "DefaultValue", "ComputedFormula", "IsIdentity", "DataLength",
			"DataType", "DataTypeWithoutLength", "Mode"
		};
		dataTypesStrings = new string[17]
		{
			"varchar", "char", "text", "date", "datetime", "double", "bit", "int", "long", "decimal",
			"float", "real", "money", "blob", "xml", "time", "timestamp"
		};
		DataTypes = new AutoCompleteStringCollection();
		DataTypes.AddRange(dataTypesStrings);
	}

	public static bool IsCellBlocked(BaseRow row, string columnName)
	{
		if (row != null && row.Source == UserTypeEnum.UserType.DBMS)
		{
			return columnsToBlock.Contains(columnName);
		}
		return false;
	}

	public static string GetFullTableName(TableByDatabaseIdObject row, string schemaColumnName, string tableColumnName)
	{
		string schema = row.GetType()?.GetProperty(schemaColumnName)?.GetValue(row, null)?.ToString();
		string name = row.GetType()?.GetProperty(tableColumnName)?.GetValue(row, null)?.ToString();
		return GetFullTableName(schema, name);
	}

	public static string GetFullTableName(string schema, string name)
	{
		return schema?.ToLower() + "." + name?.ToLower();
	}
}
