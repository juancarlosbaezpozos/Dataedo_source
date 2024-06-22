using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.Exceptions;
using Dataedo.Model.Data.MongoDB;
using Dataedo.Shared.Enums;
using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dataedo.App.Import.DataLake.Processing;

public class JsonImport : IDataLakeImport, IStreamableDataLakeImport
{
	private int numberOfFieldsToImport = 1000;

	public SharedObjectTypeEnum.ObjectType ObjectType { get; protected set; }

	public DataLakeTypeEnum.DataLakeType DataLakeType => DataLakeTypeEnum.DataLakeType.JSON;

	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype => DataLakeTypeEnum.GetObjectSubtype(ObjectType, DataLakeType);

	public string DefaultExtension => ".json";

	public IEnumerable<string> Extensions => new string[1] { DefaultExtension };

	public bool DetermineByExtensionPriority => true;

	public string DocumentationLink => "https://dataedo.com/docs/json?utm_source=App&utm_medium=App";

	public JsonImport(SharedObjectTypeEnum.ObjectType objectType)
	{
		ObjectType = objectType;
	}

	public IEnumerable<ObjectModel> GetObjectsFromData(string data)
	{
		return GetObjectsFromStream(new StringReader(data), null);
	}

	public IEnumerable<ObjectModel> GetObjectsFromFile(string path)
	{
		return GetObjectsFromStream(new StreamReader(path), path);
	}

	public bool IsValidData(string data)
	{
		return IsValidStream(new StringReader(data));
	}

	public bool IsValidFile(string path)
	{
		if (!File.Exists(path))
		{
			return false;
		}
		return IsValidStream(new StreamReader(path));
	}

	private bool IsValidStream(TextReader textReader)
	{
		try
		{
			using (JsonTextReader jsonTextReader = new JsonTextReader(textReader))
			{
				int num = 0;
				jsonTextReader.SupportMultipleContent = true;
				while (jsonTextReader.Read())
				{
					num++;
					if (jsonTextReader.TokenType == JsonToken.StartObject)
					{
						JObject.Load(jsonTextReader);
					}
					if (num > numberOfFieldsToImport)
					{
						break;
					}
				}
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
		using StreamReader textReader = new StreamReader(stream);
		return GetObjectsFromStream(textReader, null);
	}

	private IEnumerable<ObjectModel> GetObjectsFromStream(TextReader textReader, string path)
	{
		BsonTable bsonTable = new BsonTable(string.Empty, string.Empty);
		try
		{
			string text = textReader.ReadToEnd();
			using (StringReader textReader2 = new StringReader(text))
			{
				if (IsValidStream(textReader2))
				{
					using StringReader reader = new StringReader(text);
					using JsonTextReader jsonTextReader = new JsonTextReader(reader);
					int num = 0;
					jsonTextReader.SupportMultipleContent = true;
					while (jsonTextReader.Read())
					{
						num++;
						if (jsonTextReader.TokenType == JsonToken.StartObject)
						{
							bsonTable.AddDocument(JObject.Load(jsonTextReader));
						}
						if (num > numberOfFieldsToImport)
						{
							break;
						}
					}
				}
				else
				{
					try
					{
						BsonDocument document = BsonDocument.Parse(text);
						bsonTable.AddDocument(document);
					}
					catch
					{
						throw new JsonReaderException();
					}
				}
			}
			JsonObjectModel jsonObjectModel = new JsonObjectModel(string.IsNullOrEmpty(path) ? string.Empty : Path.GetFileName(path), path, ObjectType, DataLakeType, bsonTable);
			return new JsonObjectModel[1] { jsonObjectModel };
		}
		catch (JsonReaderException innerException)
		{
			if (bsonTable.Columns.Count() > 0)
			{
				JsonObjectModel jsonObjectModel2 = new JsonObjectModel(string.IsNullOrEmpty(path) ? null : Path.GetFileName(path), path, ObjectType, DataLakeType, bsonTable);
				return new JsonObjectModel[1] { jsonObjectModel2 };
			}
			throw new InvalidDataProvidedException("Unable to load JSON data." + Environment.NewLine + "JSON data is invalid.", innerException);
		}
		catch (Exception)
		{
			throw;
		}
	}
}
