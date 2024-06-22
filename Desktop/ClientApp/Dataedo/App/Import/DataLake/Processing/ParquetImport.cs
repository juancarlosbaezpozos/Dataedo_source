using System;
using System.Collections.Generic;
using System.IO;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.Exceptions;
using Dataedo.Shared.Enums;
using Parquet;
using Parquet.Data;

namespace Dataedo.App.Import.DataLake.Processing;

internal class ParquetImport : IDataLakeImport, IStreamableDataLakeImport
{
	public SharedObjectTypeEnum.ObjectType ObjectType { get; }

	public virtual DataLakeTypeEnum.DataLakeType DataLakeType => DataLakeTypeEnum.DataLakeType.PARQUET;

	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype => DataLakeTypeEnum.GetObjectSubtype(ObjectType, DataLakeType);

	public virtual string DefaultExtension => ".parquet";

	public virtual IEnumerable<string> Extensions => new string[1] { DefaultExtension };

	public bool DetermineByExtensionPriority => true;

	public string DocumentationLink => "https://dataedo.com/docs/apache-parquet?utm_source=App&utm_medium=App";

	public ParquetImport(SharedObjectTypeEnum.ObjectType objectType)
	{
		ObjectType = objectType;
	}

	public IEnumerable<ObjectModel> GetObjectsFromData(string data)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<ObjectModel> GetObjectsFromFile(string path)
	{
		using FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
		return GetObjectsFromStream(fileStream, path);
	}

	public bool IsValidData(string data)
	{
		throw new NotImplementedException();
	}

	public bool IsValidFile(string path)
	{
		if (!File.Exists(path))
		{
			return false;
		}
		return IsValidStream(new FileStream(path, FileMode.Open));
	}

	private bool IsValidStream(FileStream fileStream)
	{
		try
		{
			using (new ParquetReader(fileStream))
			{
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	public IEnumerable<ObjectModel> GetObjectsFromStream(Stream stream)
	{
		return GetObjectsFromStream(stream, null);
	}

	public virtual IEnumerable<ObjectModel> GetObjectsFromStream(Stream fileStream, string path)
	{
		try
		{
			ObjectModel objectModel = new ObjectModel(Path.GetFileName(path), path, DataLakeType, ObjectType, ObjectSubtype);
			using (ParquetReader parquetReader = new ParquetReader(fileStream))
			{
				int num = 1;
				foreach (Field field in parquetReader.Schema.Fields)
				{
					AddFieldsToModel(objectModel, field, null, num, 1);
					num++;
				}
			}
			return new ObjectModel[1] { objectModel };
		}
		catch (ParquetException innerException)
		{
			throw new InvalidDataProvidedException("Unable to load parquet data." + Environment.NewLine + "Parquet data is invalid.", innerException);
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	protected void AddFieldsToModel(ObjectModel objectModel, Field parquetField, FieldModel parentField, int position, int level, bool omitField = false)
	{
		FieldModel parentField2 = parentField;
		if (!omitField)
		{
			ParquetFieldModel parquetFieldModel = ParquetFieldModel.ParquetFieldModelCreator(parquetField, parentField, position, level);
			objectModel.Fields.Add(parquetFieldModel);
			parentField2 = parquetFieldModel;
		}
		switch (parquetField.SchemaType)
		{
		case SchemaType.Struct:
		{
			level++;
			int position2 = 1;
			{
				foreach (Field field in ((StructField)parquetField).Fields)
				{
					AddFieldsToModel(objectModel, field, parentField2, position2, level);
					position2++;
				}
				break;
			}
		}
		case SchemaType.List:
		{
			Field item = ((ListField)parquetField).Item;
			bool omitField2 = true;
			if (isComplexType(item))
			{
				level++;
				omitField2 = false;
			}
			int position2 = 1;
			AddFieldsToModel(objectModel, item, parentField2, position2, level, omitField2);
			break;
		}
		case SchemaType.Map:
			level++;
			AddFieldsToModel(objectModel, ((MapField)parquetField).Key, parentField2, position = 1, level);
			AddFieldsToModel(objectModel, ((MapField)parquetField).Value, parentField2, position = 2, level);
			break;
		}
	}

	protected bool isComplexType(Field parquetField)
	{
		SchemaType schemaType = parquetField.SchemaType;
		if (schemaType != 0 && (uint)(schemaType - 1) <= 2u)
		{
			return true;
		}
		return false;
	}
}
