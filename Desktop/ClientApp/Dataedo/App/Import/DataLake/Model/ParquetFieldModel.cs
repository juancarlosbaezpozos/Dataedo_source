using Dataedo.Shared.Enums;
using Parquet.Data;

namespace Dataedo.App.Import.DataLake.Model;

internal class ParquetFieldModel : FieldModel
{
	private const string arrayName = "list";

	private const string keyValueName = "key_value";

	public static ParquetFieldModel ParquetFieldModelCreator(Field parquetField, FieldModel parentFieldModel, int position, int level)
	{
		ParquetFieldModel parquetFieldModel = new ParquetFieldModel(parquetField, position, level);
		parquetFieldModel.DataType = GetDataType(parquetField);
		parquetFieldModel.ObjectSubtype = GetSubtype(parquetField);
		parquetFieldModel.Nullable = IsNullable(parquetField);
		if (parentFieldModel != null)
		{
			parquetFieldModel.ParentField = parentFieldModel;
			parquetFieldModel.Path = parentFieldModel.FullName;
		}
		else
		{
			parquetFieldModel.Path = ParseParquetFieldPath(parquetField.Name, parquetField.Path, '.');
		}
		return parquetFieldModel;
	}

	private ParquetFieldModel(Field parquetField, int position, int level)
	{
		base.Name = parquetField.Name;
		base.Level = level;
		base.Position = position;
	}

	private static string ParseParquetFieldPath(string fieldName, string fieldPath, char pathSeparator)
	{
		int num = fieldPath.IndexOf(pathSeparator + "list");
		if (num != -1)
		{
			fieldPath = fieldPath.Remove(num, "list".Length + 1);
		}
		int num2 = fieldPath.IndexOf(pathSeparator + "key_value");
		if (num2 != -1)
		{
			fieldPath = fieldPath.Remove(num2, "key_value".Length + 1);
		}
		int num3 = fieldPath.IndexOf(pathSeparator + fieldName);
		if (num3 != -1)
		{
			return fieldPath.Remove(num3);
		}
		return string.Empty;
	}

	private static SharedObjectSubtypeEnum.ObjectSubtype GetSubtype(Field field)
	{
		return field.SchemaType switch
		{
			SchemaType.Struct => SharedObjectSubtypeEnum.ObjectSubtype.Object, 
			SchemaType.List => SharedObjectSubtypeEnum.ObjectSubtype.Array, 
			SchemaType.Map => SharedObjectSubtypeEnum.ObjectSubtype.Map, 
			_ => SharedObjectSubtypeEnum.ObjectSubtype.Field, 
		};
	}

	private static string GetDataType(Field field)
	{
		switch (field.SchemaType)
		{
		case SchemaType.Struct:
			return "Struct";
		case SchemaType.List:
			return GetNestedType(((ListField)field).Item) + "[]";
		case SchemaType.Map:
		{
			string dataType = GetDataType(((MapField)field).Key);
			string dataType2 = GetDataType(((MapField)field).Value);
			return "Map<" + dataType + ", " + dataType2 + ">";
		}
		default:
			return ((DataField)field).DataType.ToString();
		}
	}

	private static string GetNestedType(Field field)
	{
		return field.SchemaType switch
		{
			SchemaType.List => "List", 
			SchemaType.Map => "Map", 
			_ => GetDataType(field), 
		};
	}

	private static bool IsNullable(Field parquetField)
	{
		SchemaType schemaType = parquetField.SchemaType;
		if (schemaType != 0 && (uint)(schemaType - 1) <= 2u)
		{
			return true;
		}
		return ((DataField)parquetField).HasNulls;
	}
}
