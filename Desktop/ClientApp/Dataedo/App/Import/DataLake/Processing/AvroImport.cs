using System;
using System.Collections.Generic;
using System.IO;
using Avro;
using Avro.File;
using Avro.Generic;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.Exceptions;
using Dataedo.Shared.Enums;
using Newtonsoft.Json;

namespace Dataedo.App.Import.DataLake.Processing;

internal class AvroImport : IDataLakeImport, IStreamableDataLakeImport
{
	private Dictionary<string, ObjectModel> recordObjectModelsDict;

	public SharedObjectTypeEnum.ObjectType ObjectType { get; protected set; }

	public DataLakeTypeEnum.DataLakeType DataLakeType => DataLakeTypeEnum.DataLakeType.AVRO;

	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype => DataLakeTypeEnum.GetObjectSubtype(ObjectType, DataLakeType);

	public string DefaultExtension => ".avro";

	public IEnumerable<string> Extensions => new string[2] { DefaultExtension, ".avsc" };

	public bool DetermineByExtensionPriority => true;

	public string DocumentationLink => "https://dataedo.com/docs/apache-avro?utm_source=App&utm_medium=App";

	public AvroImport(SharedObjectTypeEnum.ObjectType objectType)
	{
		recordObjectModelsDict = new Dictionary<string, ObjectModel>();
		ObjectType = objectType;
	}

	public IEnumerable<ObjectModel> GetObjectsFromData(string data)
	{
		return GetObjectsFromData(data, null);
	}

	public IEnumerable<ObjectModel> GetObjectsFromFile(string path)
	{
		string text = Path.GetExtension(path).ToLower();
		if (!(text == ".avsc"))
		{
			if (!(text == ".avro"))
			{
			}
			using FileStream avroStream = new FileStream(path, FileMode.Open, FileAccess.Read);
			return GetObjectsFromAvroStream(avroStream, path);
		}
		string jsonData = File.ReadAllText(path);
		return GetObjectsFromData(jsonData, path);
	}

	public bool IsValidData(string data)
	{
		try
		{
			Schema.Parse(data);
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public bool IsValidFile(string path)
	{
		if (!File.Exists(path))
		{
			return false;
		}
		using IFileReader<GenericRecord> fileReader = DataFileReader<GenericRecord>.OpenReader(path);
		return IsValidStream(fileReader);
	}

	private bool IsValidStream(IFileReader<GenericRecord> fileReader)
	{
		try
		{
			fileReader.GetSchema();
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private IEnumerable<ObjectModel> GetObjectsFromData(string jsonData, string path)
	{
		try
		{
			Schema schema = Schema.Parse(jsonData);
			return CreateObjectModelsFromSchema(schema, path, DataLakeType, ObjectType);
		}
		catch (Exception ex)
		{
			if (ex is AvroException || ex is JsonException || ex is ArgumentNullException)
			{
				throw new InvalidDataProvidedException("Unable to load Avro data." + Environment.NewLine + "Avro data is invalid.", ex);
			}
			throw ex;
		}
	}

	public IEnumerable<ObjectModel> GetObjectsFromStream(Stream stream)
	{
		return GetObjectsFromAvroStream(stream, null);
	}

	public IEnumerable<ObjectModel> GetObjectsFromAvroOrAvscStream(Stream stream, string fileName)
	{
		string text = Path.GetExtension(fileName).ToLower();
		if (!(text == ".avsc"))
		{
			if (!(text == ".avro"))
			{
			}
			return GetObjectsFromAvroStream(stream, null);
		}
		IEnumerable<ObjectModel> objectsFromStream = new JsonImport(ObjectType).GetObjectsFromStream(stream);
		foreach (ObjectModel item in objectsFromStream)
		{
			item.DataLakeType = DataLakeTypeEnum.DataLakeType.AVRO;
		}
		return objectsFromStream;
	}

	private IEnumerable<ObjectModel> GetObjectsFromAvroStream(Stream avroStream, string path)
	{
		try
		{
			using IFileReader<GenericRecord> fileReader = DataFileReader<GenericRecord>.OpenReader(avroStream);
			Schema schema = fileReader.GetSchema();
			return CreateObjectModelsFromSchema(schema, path, DataLakeType, ObjectType);
		}
		catch (AvroException innerException)
		{
			throw new InvalidDataProvidedException("Unable to load Avro data." + Environment.NewLine + "Avro data is invalid.", innerException);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	private IEnumerable<ObjectModel> CreateObjectModelsFromSchema(Schema schema, string path, DataLakeTypeEnum.DataLakeType dataLakeType, SharedObjectTypeEnum.ObjectType objectType)
	{
		List<ObjectModel> list = new List<ObjectModel>(1);
		int index = 0;
		int num = 1;
		if (schema.GetType() == typeof(RecordSchema))
		{
			list.Add(CreateRootRecordObjectModel(schema, path, dataLakeType, objectType));
			return list;
		}
		list.Add(new ObjectModel(string.IsNullOrEmpty(path) ? "structure" : (Path.GetFileName(path) ?? ""), path, dataLakeType, objectType, SharedObjectSubtypeEnum.ObjectSubtype.Structure));
		if (schema.GetType() == typeof(UnionSchema))
		{
			foreach (Schema schema2 in ((UnionSchema)schema).Schemas)
			{
				if (schema2.GetType() == typeof(RecordSchema))
				{
					AvroFieldModel item = AvroFieldModel.CreateAvroFieldModel(schema2, 1, num, null);
					list[index].Fields.Add(item);
					list.Add(CreateRootRecordObjectModel(schema2, path, dataLakeType, objectType));
				}
				else
				{
					AddFieldToModel(schema2, list[index], 1, num, null);
				}
				num++;
			}
			return list;
		}
		AddFieldToModel(schema, list[index], 1, num, null);
		return list;
	}

	private ObjectModel CreateRootRecordObjectModel(Schema schema, string path, DataLakeTypeEnum.DataLakeType dataLakeType, SharedObjectTypeEnum.ObjectType objectType)
	{
		ObjectModel objectModel = new ObjectModel(string.IsNullOrEmpty(path) ? (schema.Fullname ?? "") : (Path.GetFileName(path) + ".[" + schema.Fullname + "]"), path, dataLakeType, objectType, SharedObjectSubtypeEnum.ObjectSubtype.AvroRecord);
		ParseRecordSchema((RecordSchema)schema, objectModel, 0, null);
		return objectModel;
	}

	private void AddFieldToModel(Schema schema, ObjectModel objectModel, int level, int position, AvroFieldModel parentField)
	{
		AddFieldToModel(schema, objectModel, level, position, parentField, null, null);
	}

	private void AddFieldToModel(Schema schema, ObjectModel objectModel, int level, int position, AvroFieldModel parentField, string name, string description)
	{
		AvroFieldModel avroFieldModel = AvroFieldModel.CreateAvroFieldModel(schema, level, position, parentField);
		avroFieldModel.Description = ((!string.IsNullOrEmpty(description)) ? description : avroFieldModel.Description);
		avroFieldModel.Name = ((!string.IsNullOrEmpty(name)) ? name : avroFieldModel.Name);
		objectModel.Fields.Add(avroFieldModel);
		if (schema.GetType() == typeof(RecordSchema))
		{
			ParseRecordSchema((RecordSchema)schema, objectModel, level, avroFieldModel);
		}
		else if (schema.GetType() == typeof(ArraySchema) && AvroFieldModel.HasComplexFields(schema))
		{
			ParseArraySchema((ArraySchema)schema, objectModel, level, avroFieldModel);
		}
		else if (schema.GetType() == typeof(MapSchema))
		{
			ParseMapSchema((MapSchema)schema, objectModel, level, avroFieldModel);
		}
		else if (schema.GetType() == typeof(UnionSchema) && AvroFieldModel.HasComplexFields(schema))
		{
			ParseUnionSchema((UnionSchema)schema, objectModel, level, avroFieldModel);
		}
	}

	private void ParseRecordSchema(RecordSchema recordSchema, ObjectModel objectModel, int level, AvroFieldModel parentField)
	{
		if (recordObjectModelsDict.ContainsKey(recordSchema.Fullname))
		{
			return;
		}
		recordObjectModelsDict.Add(recordSchema.Fullname, objectModel);
		int level2 = level + 1;
		foreach (Field field in recordSchema.Fields)
		{
			string name = field.Name;
			string documentation = field.Documentation;
			int position = field.Pos + 1;
			AddFieldToModel(field.Schema, objectModel, level2, position, parentField, name, documentation);
		}
	}

	private void ParseArraySchema(ArraySchema arraySchema, ObjectModel objectModel, int level, AvroFieldModel parentField)
	{
		int level2 = level + 1;
		AddFieldToModel(arraySchema.ItemSchema, objectModel, level2, 1, parentField);
	}

	private void ParseMapSchema(MapSchema mapSchema, ObjectModel objectModel, int level, AvroFieldModel parentField)
	{
		int level2 = level + 1;
		int position = 1;
		int position2 = 2;
		AddFieldToModel(PrimitiveSchema.NewInstance("string"), objectModel, level2, position, parentField, "key", null);
		AddFieldToModel(mapSchema.ValueSchema, objectModel, level2, position2, parentField, "value", null);
	}

	private void ParseUnionSchema(UnionSchema unionSchema, ObjectModel objectModel, int level, AvroFieldModel parentField)
	{
		int num = 1;
		foreach (Schema schema in unionSchema.Schemas)
		{
			AddFieldToModel(schema, objectModel, level + 1, num, parentField);
			num++;
		}
	}
}
