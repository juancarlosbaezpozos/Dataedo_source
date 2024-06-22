using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.DataLake.Processing.CsvSupport;
using Dataedo.App.Import.Exceptions;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.DataLake.Processing;

public class CsvImport : IDataLakeImport, IStreamableDataLakeImport
{
	public SharedObjectTypeEnum.ObjectType ObjectType { get; protected set; }

	public DataLakeTypeEnum.DataLakeType DataLakeType => DataLakeTypeEnum.DataLakeType.CSV;

	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype => DataLakeTypeEnum.GetObjectSubtype(ObjectType, DataLakeType);

	public string DefaultExtension => ".csv";

	public IEnumerable<string> Extensions => new string[1] { DefaultExtension };

	public bool DetermineByExtensionPriority => false;

	public string DocumentationLink => "https://dataedo.com/docs/csv?utm_source=App&utm_medium=App";

	public CsvImport(SharedObjectTypeEnum.ObjectType objectType)
	{
		ObjectType = objectType;
	}

	public IEnumerable<ObjectModel> GetObjectsFromData(string data)
	{
		int value = data.Split('\n').Length;
		return GetObjectsFromStream(new StringReader(data), null, value);
	}

	public IEnumerable<ObjectModel> GetObjectsFromFile(string path)
	{
		int value = File.ReadLines(path).Count();
		return GetObjectsFromStream(new StreamReader(path), path, value);
	}

	public bool IsValidData(string data)
	{
		int value = data.Split('\n').Length;
		return IsValidData(new StringReader(data), value);
	}

	public bool IsValidFile(string path)
	{
		if (!File.Exists(path))
		{
			return false;
		}
		string text = null;
		try
		{
			text = Path.GetExtension(path);
		}
		catch (Exception)
		{
		}
		return text?.ToLower() == DefaultExtension;
	}

	private bool IsValidData(TextReader stream, int? lineCount = null)
	{
		if (lineCount < 2)
		{
			return false;
		}
		try
		{
			using CsvReader csvReader = new CsvReader(stream, CultureInfo.InvariantCulture);
			csvReader.Configuration.IgnoreBlankLines = true;
			csvReader.Configuration.IgnoreQuotes = false;
			csvReader.Configuration.BadDataFound = delegate
			{
			};
			csvReader.Configuration.MissingFieldFound = delegate
			{
			};
			csvReader.Read();
			csvReader.ReadHeader();
			int num = csvReader.Context.HeaderRecord.Count();
			int num2 = 0;
			int num3 = 1000;
			using (CsvDataReader csvDataReader = new CsvDataReader(csvReader))
			{
				do
				{
					if (csvDataReader.FieldCount >= num)
					{
						string[] array = new string[num];
						object[] values = array;
						csvDataReader.GetValues(values);
						num2++;
					}
				}
				while (csvDataReader.Read() && num2 < num3);
			}
			if (num2 == 0)
			{
				return false;
			}
		}
		catch
		{
			return false;
		}
		return true;
	}

	public IEnumerable<ObjectModel> GetObjectsFromStream(Stream stream)
	{
		using StreamReader textReader = new StreamReader(stream);
		return GetObjectsFromStream(textReader, null);
	}

	private IEnumerable<ObjectModel> GetObjectsFromStream(TextReader textReader, string path, int? lineCount = null)
	{
		try
		{
			CsvTable bsonTable = new CsvTable(textReader, lineCount);
			CsvObjectModel csvObjectModel = new CsvObjectModel(string.IsNullOrEmpty(path) ? string.Empty : Path.GetFileName(path), path, ObjectType, DataLakeType, bsonTable);
			return new CsvObjectModel[1] { csvObjectModel };
		}
		catch (BadDataException innerException)
		{
			throw new InvalidDataProvidedException("Unable to load CSV data." + Environment.NewLine + "CSV data is invalid.", innerException);
		}
		catch (ReaderException innerException2)
		{
			throw new InvalidDataProvidedException("Unable to load CSV data." + Environment.NewLine + "CSV data is invalid.", innerException2);
		}
		catch (Exception ex)
		{
			if (ex.InnerException is InvalidDataProvidedException ex2)
			{
				throw ex2;
			}
			throw;
		}
	}
}
