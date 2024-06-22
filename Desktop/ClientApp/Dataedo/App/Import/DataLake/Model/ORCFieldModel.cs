using Dataedo.App.Import.DataLake.Processing.ORCSupport;
using Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.DataLake.Model;

internal class ORCFieldModel : FieldModel
{
	public ColumnTypeKind ColumnTypeKind { get; set; }

	public static ORCFieldModel CreateORCFieldModel(string fieldName, ColumnType column, int level, int position, FieldModel parentField, FileTail fileTail)
	{
		return new ORCFieldModel(fieldName, level, position, parentField)
		{
			DataType = GetDataType(column, fileTail),
			Path = parentField?.FullName,
			ColumnTypeKind = column.Kind,
			ObjectSubtype = GetSubtype(column)
		};
	}

	private ORCFieldModel(string fieldName, int level, int position, FieldModel parentField)
	{
		base.Name = fieldName;
		base.ParentField = parentField;
		base.Level = level;
		base.Position = position;
		base.Nullable = true;
	}

	private static SharedObjectSubtypeEnum.ObjectSubtype GetSubtype(ColumnType column)
	{
		return column.Kind switch
		{
			ColumnTypeKind.Struct => SharedObjectSubtypeEnum.ObjectSubtype.Object, 
			ColumnTypeKind.List => SharedObjectSubtypeEnum.ObjectSubtype.Array, 
			ColumnTypeKind.Map => SharedObjectSubtypeEnum.ObjectSubtype.Map, 
			_ => SharedObjectSubtypeEnum.ObjectSubtype.Field, 
		};
	}

	private static string GetDataType(ColumnType column, FileTail fileTail)
	{
		switch (column.Kind)
		{
		case ColumnTypeKind.Struct:
			return "Struct";
		case ColumnTypeKind.List:
		{
			int index3 = (int)column.SubTypes[0];
			ColumnTypeKind kind = fileTail.GetFooter().Types[index3].Kind;
			return $"{kind}[]";
		}
		case ColumnTypeKind.Map:
		{
			int index = (int)column.SubTypes[0];
			int index2 = (int)column.SubTypes[1];
			ColumnType column2 = fileTail.GetFooter().Types[index];
			ColumnType column3 = fileTail.GetFooter().Types[index2];
			return "Map<" + GetDataType(column2, fileTail) + ", " + GetDataType(column3, fileTail) + ">";
		}
		default:
			return $"{column.Kind}";
		}
	}
}
