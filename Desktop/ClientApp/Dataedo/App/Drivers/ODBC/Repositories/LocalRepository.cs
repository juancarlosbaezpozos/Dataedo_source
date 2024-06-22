using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Dataedo.App.Drivers.ODBC.Repositories.Exceptions;
using Dataedo.App.Drivers.ODBC.ValueObjects;

namespace Dataedo.App.Drivers.ODBC.Repositories;

internal class LocalRepository : IRepository
{
	public string BasePath { get; protected set; }

	public LocalRepository(string basePath)
	{
		BasePath = basePath;
	}

	public bool Has(string uid)
	{
		return Directory.Exists(GetDriverPath(uid));
	}

	public bool Has(Driver driver)
	{
		return Has(driver.MetaFile.UID);
	}

	public IEnumerable<DriverMetaFile> List()
	{
		List<DriverMetaFile> list = new List<DriverMetaFile>();
		if (Directory.Exists(BasePath))
		{
			string[] directories = Directory.GetDirectories(BasePath, "*", SearchOption.TopDirectoryOnly);
			for (int i = 0; i < directories.Length; i++)
			{
				string fileName = Path.GetFileName(directories[i]);
				list.Add(LoadMetaFile(fileName));
			}
		}
		return list;
	}

	public Driver Load(string uid)
	{
		string driverPath = GetDriverPath(uid);
		DriverMetaFile metaFile = LoadMetaFile(uid);
		DriverQueries driverQueries = new DriverQueries();
		PropertyInfo[] properties = driverQueries.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			string path = CamelCaseToSnakeCase(propertyInfo.Name) + ".sql";
			string value = File.ReadAllText(Path.Combine(driverPath, path));
			propertyInfo.SetValue(driverQueries, value, null);
		}
		return new Driver(metaFile, driverQueries);
	}

	public Driver Load(DriverMetaFile meta)
	{
		return Load(meta.UID);
	}

	private DriverMetaFile LoadMetaFile(string uid)
	{
		string driverPath = GetDriverPath(uid);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(DriverMetaFileXml));
		StreamReader streamReader = new StreamReader(Path.Combine(driverPath, "driver.xml"));
		DriverMetaFileXml driverMetaFileXml = (DriverMetaFileXml)xmlSerializer.Deserialize(streamReader);
		DriverMetaFile result = new DriverMetaFile(uid, driverMetaFileXml.Name, driverMetaFileXml.Version);
		streamReader.Close();
		return result;
	}

	public void Store(Driver driver)
	{
		try
		{
			string driverPath = GetDriverPath(driver);
			Directory.CreateDirectory(driverPath);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(DriverMetaFileXml));
			TextWriter textWriter = new StreamWriter(Path.Combine(driverPath, "driver.xml"));
			xmlSerializer.Serialize(textWriter, new DriverMetaFileXml
			{
				Name = driver.MetaFile.Name,
				Version = driver.MetaFile.Version
			});
			textWriter.Close();
			PropertyInfo[] properties = driver.Queries.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				string path = CamelCaseToSnakeCase(propertyInfo.Name) + ".sql";
				string contents = propertyInfo.GetValue(driver.Queries, null) as string;
				File.WriteAllText(Path.Combine(driverPath, path), contents);
			}
		}
		catch (Exception)
		{
			throw new DriverSerializationException("Failed at storing the '" + driver.MetaFile.Name + "' (directory name: '" + driver.MetaFile.UID + "') driver on disk.");
		}
	}

	protected string GetDriverPath(string uid)
	{
		return Path.Combine(BasePath, uid);
	}

	protected string GetDriverPath(Driver driver)
	{
		return GetDriverPath(driver.MetaFile.UID);
	}

	protected string CamelCaseToSnakeCase(string value)
	{
		if (string.IsNullOrEmpty(value))
		{
			return value;
		}
		return Regex.Replace(value, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", "_$1", RegexOptions.Compiled).Trim().ToLower();
	}
}
