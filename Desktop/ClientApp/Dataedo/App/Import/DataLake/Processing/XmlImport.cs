using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.Exceptions;
using Dataedo.Model.Data.MongoDB;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.DataLake.Processing;

public class XmlImport : IDataLakeImport
{
	public SharedObjectTypeEnum.ObjectType ObjectType { get; protected set; }

	public DataLakeTypeEnum.DataLakeType DataLakeType => DataLakeTypeEnum.DataLakeType.XML;

	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype => DataLakeTypeEnum.GetObjectSubtype(ObjectType, DataLakeType);

	public string DefaultExtension => ".xml";

	public IEnumerable<string> Extensions => new string[1] { DefaultExtension };

	public bool DetermineByExtensionPriority => true;

	public string DocumentationLink => "https://dataedo.com/docs/xml?utm_source=App&utm_medium=App";

	public XmlImport(SharedObjectTypeEnum.ObjectType objectType)
	{
		ObjectType = objectType;
	}

	public IEnumerable<ObjectModel> GetObjectsFromData(string data)
	{
		return GetObjectsFromStream(new StringReader(data?.Trim()), null);
	}

	public IEnumerable<ObjectModel> GetObjectsFromFile(string path)
	{
		using StreamReader textReader = new StreamReader(path);
		return GetObjectsFromStream(textReader, path);
	}

	public bool IsValidData(string data)
	{
		try
		{
			new XDocument();
			XDocument.Parse(data?.Trim());
			return true;
		}
		catch (XmlException)
		{
			return false;
		}
		catch (Exception ex2)
		{
			throw ex2;
		}
	}

	public bool IsValidFile(string path)
	{
		if (!File.Exists(path))
		{
			return false;
		}
		try
		{
			new XmlDocument().Load(new StreamReader(path));
			return true;
		}
		catch
		{
			return false;
		}
	}

	private IEnumerable<ObjectModel> GetObjectsFromStream(TextReader textReader, string path)
	{
		try
		{
			BsonTable bsonTable = new BsonTable(string.Empty, string.Empty);
			new XmlDocument();
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.IgnoreComments = true;
			xmlReaderSettings.DtdProcessing = DtdProcessing.Ignore;
			using (XmlReader reader = XmlReader.Create(textReader, xmlReaderSettings))
			{
				bsonTable.AddXmlDocumentFromFile(reader);
			}
			XmlObjectModel xmlObjectModel = new XmlObjectModel(string.IsNullOrEmpty(path) ? string.Empty : Path.GetFileName(path), path, ObjectType, DataLakeType, bsonTable);
			return new XmlObjectModel[1] { xmlObjectModel };
		}
		catch (XmlException innerException)
		{
			throw new InvalidDataProvidedException("Unable to load XML data." + Environment.NewLine + "XML data is invalid.", innerException);
		}
		catch (Exception)
		{
			throw;
		}
	}
}
