using System.Collections.Generic;
using Avro;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.DataLake.Model;

internal class AvroFieldModel : FieldModel
{
	public Schema Schema { get; }

	public string Namespace { get; set; }

	public static AvroFieldModel CreateAvroFieldModel(Schema schema, int level, int position, AvroFieldModel parentField)
	{
		AvroFieldModel avroFieldModel = new AvroFieldModel(schema, level, position, parentField);
		avroFieldModel.DataType = GetDataType(avroFieldModel.Schema);
		avroFieldModel.ObjectSubtype = GetObjectSubtypeEnum(schema);
		avroFieldModel.Nullable = IsNullable(schema);
		avroFieldModel.Path = ((avroFieldModel.ParentField != null) ? avroFieldModel.ParentField.FullName : null);
		if (schema.GetType().BaseType == typeof(NamedSchema))
		{
			avroFieldModel.Description = ((NamedSchema)schema).Documentation;
		}
		return avroFieldModel;
	}

	private AvroFieldModel(Schema schema, int level, int position, AvroFieldModel parentField)
	{
		Schema = schema;
		base.Name = schema.Name;
		base.Level = level;
		base.Position = position;
		base.ParentField = parentField;
	}

	private static bool IsNullable(Schema schema)
	{
		if (schema.GetType() == typeof(UnionSchema))
		{
			foreach (Schema schema2 in ((UnionSchema)schema).Schemas)
			{
				if (schema2.Tag == Schema.Type.Null)
				{
					return true;
				}
			}
		}
		return false;
	}

	private static SharedObjectSubtypeEnum.ObjectSubtype GetObjectSubtypeEnum(Schema schema)
	{
		return schema.Tag switch
		{
			Schema.Type.Record => SharedObjectSubtypeEnum.ObjectSubtype.Object, 
			Schema.Type.Array => SharedObjectSubtypeEnum.ObjectSubtype.Array, 
			Schema.Type.Map => SharedObjectSubtypeEnum.ObjectSubtype.Map, 
			_ => SharedObjectSubtypeEnum.ObjectSubtype.Field, 
		};
	}

	private static string GetDataType(Schema schema)
	{
		if (schema.GetType() == typeof(RecordSchema))
		{
			string fullname = ((RecordSchema)schema).Fullname;
			return "record: " + fullname;
		}
		if (schema.GetType() == typeof(ArraySchema))
		{
			return Schema.GetTypeString(((ArraySchema)schema).ItemSchema.Tag) + "[]";
		}
		if (schema.GetType() == typeof(MapSchema))
		{
			string text = "string";
			Schema.Type tag = ((MapSchema)schema).ValueSchema.Tag;
			return "map<" + text + ", " + Schema.GetTypeString(tag) + ">";
		}
		if (schema.GetType() == typeof(EnumSchema))
		{
			IList<string> symbols = ((EnumSchema)schema).Symbols;
			string fullname2 = schema.Fullname;
			return "enum: " + fullname2 + "[" + string.Join(", ", symbols) + "]";
		}
		if (schema.GetType() == typeof(FixedSchema))
		{
			int size = ((FixedSchema)schema).Size;
			string fullname3 = schema.Fullname;
			return $"fixed: {fullname3}({size})";
		}
		if (schema.GetType() == typeof(LogicalSchema))
		{
			return ((LogicalSchema)schema).LogicalTypeName;
		}
		if (schema.GetType() == typeof(UnionSchema))
		{
			List<string> list = new List<string>();
			foreach (Schema schema2 in ((UnionSchema)schema).Schemas)
			{
				list.Add(GetDataType(schema2));
			}
			return "[" + string.Join(", ", list) + "]";
		}
		return Schema.GetTypeString(schema.Tag);
	}

	public static bool HasComplexFields(Schema schema)
	{
		if (schema.GetType() == typeof(RecordSchema))
		{
			return true;
		}
		if (schema.GetType() == typeof(UnionSchema))
		{
			foreach (Schema schema2 in ((UnionSchema)schema).Schemas)
			{
				if (HasComplexFields(schema2))
				{
					return true;
				}
			}
			return false;
		}
		if (schema.GetType() == typeof(ArraySchema))
		{
			return IsComplexSchema(((ArraySchema)schema).ItemSchema);
		}
		if (schema.GetType() == typeof(MapSchema))
		{
			return IsComplexSchema(((MapSchema)schema).ValueSchema);
		}
		return false;
	}

	public static bool IsComplexSchema(Schema schema)
	{
		if (schema.GetType() == typeof(RecordSchema) || schema.GetType() == typeof(MapSchema) || schema.GetType() == typeof(ArraySchema))
		{
			return true;
		}
		return false;
	}
}
