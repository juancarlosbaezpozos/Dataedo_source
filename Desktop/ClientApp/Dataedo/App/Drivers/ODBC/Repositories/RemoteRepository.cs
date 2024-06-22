using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Dataedo.App.Drivers.ODBC.Repositories.Exceptions;
using Dataedo.App.Drivers.ODBC.ValueObjects;
using Dataedo.App.Tools;
using DevExpress.Compression;
using Newtonsoft.Json.Linq;

namespace Dataedo.App.Drivers.ODBC.Repositories;

internal class RemoteRepository : IRepository
{
	public const int Version = 1;

	public string BaseUrl { get; protected set; }

	public RemoteRepository(string baseUrl)
	{
		BaseUrl = baseUrl;
	}

	public bool Has(string uid)
	{
		throw new NotImplementedException();
	}

	public bool Has(Driver driver)
	{
		throw new NotImplementedException();
	}

	public IEnumerable<DriverMetaFile> List()
	{
		List<DriverMetaFile> list = new List<DriverMetaFile>();
		using WebClient webClient = new WebClient();
		try
		{
			foreach (JToken item2 in JArray.Parse(webClient.DownloadString($"{BaseUrl}/api/v{1}/drivers/odbc/{ProgramVersion.VersionWithBuildForUrl}")))
			{
				DriverMetaFileJson driverMetaFileJson = (DriverMetaFileJson)item2.ToObject(typeof(DriverMetaFileJson));
				DriverMetaFile item = new DriverMetaFile(driverMetaFileJson.UID, driverMetaFileJson.Name, driverMetaFileJson.Version);
				list.Add(item);
			}
			return list;
		}
		catch (WebException ex)
		{
			throw ex;
		}
		catch (Exception)
		{
			throw new DriverSerializationException("Unrecognized drivers list response from the remote repository ('" + BaseUrl + "').");
		}
	}

	public Driver Load(string uid)
	{
		using WebClient webClient = new WebClient();
		try
		{
			ZipArchive zip = ZipArchive.Read(new MemoryStream(webClient.DownloadData($"{BaseUrl}/api/v{1}/drivers/odbc/{ProgramVersion.VersionWithBuildForUrl}/{uid}/download")));
			DriverMetaFile metaFile = LoadMetaFile(uid, zip);
			DriverQueries queries = LoadQueries(zip);
			return new Driver(metaFile, queries);
		}
		catch (WebException ex)
		{
			throw ex;
		}
		catch (Exception)
		{
			throw new DriverSerializationException("Unrecognized driver response from the remote repository ('" + BaseUrl + "').");
		}
	}

	private DriverMetaFile LoadMetaFile(string uid, ZipArchive zip)
	{
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(DriverMetaFile));
		Stream stream = zip["driver.xml"].Open();
		DriverMetaFile obj = (DriverMetaFile)xmlSerializer.Deserialize(stream);
		obj.UID = uid;
		stream.Close();
		return obj;
	}

	private DriverQueries LoadQueries(ZipArchive zip)
	{
		DriverQueries driverQueries = new DriverQueries();
		PropertyInfo[] properties = driverQueries.GetType().GetProperties();
		foreach (PropertyInfo propertyInfo in properties)
		{
			string name = CamelCaseToSnakeCase(propertyInfo.Name) + ".sql";
			Stream stream = zip[name].Open();
			string value = new StreamReader(stream).ReadToEnd();
			propertyInfo.SetValue(driverQueries, value, null);
			stream.Close();
		}
		return driverQueries;
	}

	public Driver Load(DriverMetaFile meta)
	{
		return Load(meta.UID);
	}

	public void Store(Driver driver)
	{
		throw new NotImplementedException();
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
