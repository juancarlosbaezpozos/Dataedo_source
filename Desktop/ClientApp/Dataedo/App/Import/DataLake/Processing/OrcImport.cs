using System;
using System.Collections.Generic;
using System.IO;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.DataLake.Processing.ORCSupport;
using Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;
using Dataedo.App.Import.Exceptions;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.DataLake.Processing;

internal class OrcImport : IDataLakeImport, IStreamableDataLakeImport
{
	public SharedObjectTypeEnum.ObjectType ObjectType { get; protected set; }

	public DataLakeTypeEnum.DataLakeType DataLakeType => DataLakeTypeEnum.DataLakeType.ORC;

	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype => DataLakeTypeEnum.GetObjectSubtype(ObjectType, DataLakeType);

	public string DefaultExtension => ".orc";

	public IEnumerable<string> Extensions => new string[1] { DefaultExtension };

	public bool DetermineByExtensionPriority => true;

	public string DocumentationLink => "https://dataedo.com/docs/apache-orc?utm_source=App&utm_medium=App";

	public OrcImport(SharedObjectTypeEnum.ObjectType objectType)
	{
		ObjectType = objectType;
	}

	public IEnumerable<ObjectModel> GetObjectsFromData(string data)
	{
		throw new NotSupportedException();
	}

	public IEnumerable<ObjectModel> GetObjectsFromFile(string path)
	{
		using FileStream inputStream = new FileStream(path, FileMode.Open, FileAccess.Read);
		return GetObjectsFromStream(inputStream, path);
	}

	public bool IsValidData(string data)
	{
		throw new NotSupportedException();
	}

	public bool IsValidFile(string path)
	{
		if (!File.Exists(path))
		{
			return false;
		}
		using FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
		return IsValidStream(stream);
	}

	private bool IsValidStream(Stream stream)
	{
		try
		{
			new FileTail(stream).GetFooter();
			return true;
		}
		catch (InvalidDataException)
		{
			return false;
		}
	}

	public IEnumerable<ObjectModel> GetObjectsFromStream(Stream stream)
	{
		return GetObjectsFromStream(stream, null);
	}

	private IEnumerable<ObjectModel> GetObjectsFromStream(Stream inputStream, string path)
	{
		try
		{
			ObjectModel objectModel = new ObjectModel(Path.GetFileName(path), path, DataLakeType, ObjectType, ObjectSubtype);
			FileTail fileTail = new FileTail(inputStream);
			int index = 0;
			int level = 0;
			AddStructFieldsToModel(fileTail.GetFooter().Types[index], objectModel, fileTail, level, null);
			return new ObjectModel[1] { objectModel };
		}
		catch (Exception ex)
		{
			if (ex is InvalidDataException || ex is NotSupportedException || ex is ArithmeticException)
			{
				throw new InvalidDataProvidedException("Unable to load ORC data." + Environment.NewLine + "ORC data is invalid.", ex);
			}
			throw ex;
		}
	}

	private static ORCFieldModel AddFieldsToModel(string fieldName, ColumnType column, ObjectModel objectModel, FileTail fileTail, int level, int position, FieldModel parentField, bool omitField = false)
	{
		ORCFieldModel oRCFieldModel = ORCFieldModel.CreateORCFieldModel(fieldName, column, level, position, parentField, fileTail);
		FieldModel parentField2 = parentField;
		if (!omitField)
		{
			objectModel.Fields.Add(oRCFieldModel);
			parentField2 = oRCFieldModel;
		}
		switch (column.Kind)
		{
		case ColumnTypeKind.Struct:
			AddStructFieldsToModel(column, objectModel, fileTail, level, parentField2);
			break;
		case ColumnTypeKind.List:
			AddListFieldsToModel(column, objectModel, fileTail, level, parentField2);
			break;
		case ColumnTypeKind.Map:
			AddMapFieldsToModel(column, objectModel, fileTail, level, parentField2);
			break;
		case ColumnTypeKind.Union:
			AddUnionFieldsToModel(column, objectModel, fileTail, level, parentField2);
			break;
		}
		return oRCFieldModel;
	}

	private static void AddStructFieldsToModel(ColumnType column, ObjectModel objectModel, FileTail fileTail, int level, FieldModel parentField)
	{
		level++;
		for (int i = 0; i < column.FieldNames.Count; i++)
		{
			string fieldName = column.FieldNames[i];
			int index = (int)column.SubTypes[i];
			ColumnType column2 = fileTail.GetFooter().Types[index];
			AddFieldsToModel(fieldName, column2, objectModel, fileTail, level, i + 1, parentField);
		}
	}

	private static void AddListFieldsToModel(ColumnType column, ObjectModel objectModel, FileTail fileTail, int level, FieldModel parentField)
	{
		bool omitField = true;
		int index = (int)column.SubTypes[0];
		ColumnType column2 = fileTail.GetFooter().Types[index];
		if (IsComplexField(column2))
		{
			omitField = false;
			level++;
		}
		AddFieldsToModel("element", column2, objectModel, fileTail, level, 1, parentField, omitField);
	}

	private static void AddMapFieldsToModel(ColumnType column, ObjectModel objectModel, FileTail fileTail, int level, FieldModel parentField)
	{
		level++;
		string fieldName = "value";
		int position = 1;
		int position2 = 2;
		int index = (int)column.SubTypes[0];
		int index2 = (int)column.SubTypes[1];
		ColumnType column2 = fileTail.GetFooter().Types[index];
		ColumnType column3 = fileTail.GetFooter().Types[index2];
		AddFieldsToModel("key", column2, objectModel, fileTail, level, position, parentField);
		AddFieldsToModel(fieldName, column3, objectModel, fileTail, level, position2, parentField);
	}

	private static void AddUnionFieldsToModel(ColumnType column, ObjectModel objectModel, FileTail fileTail, int level, FieldModel parentField)
	{
		level++;
		for (int i = 0; i < column.SubTypes.Count; i++)
		{
			int index = (int)column.SubTypes[i];
			ColumnType columnType = fileTail.GetFooter().Types[index];
			AddFieldsToModel(columnType.Kind.ToString(), columnType, objectModel, fileTail, level, i + 1, parentField);
		}
	}

	private static bool IsComplexField(ColumnType column)
	{
		ColumnTypeKind kind = column.Kind;
		if ((uint)(kind - 10) <= 3u)
		{
			return true;
		}
		return false;
	}
}
