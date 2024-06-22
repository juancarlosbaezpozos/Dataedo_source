using System;
using System.Collections.Generic;
using System.Linq;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Files.DataLake;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Helpers.Extensions;
using Microsoft.WindowsAzure.Storage;

namespace Dataedo.App.Helpers.CloudStorage.AzureStorage.Connections;

public abstract class AzureStorageConnection
{
	public const string DefaultDelimiter = "/";

	public abstract Uri Uri { get; }

	public static AzureStorageConnection CreateConnectionFromSASToken(string sasToken, string accountName, string path)
	{
		string[] array = path?.Split(new string[1] { "/" }, StringSplitOptions.RemoveEmptyEntries);
		if (array == null || !array.Any())
		{
			return new AzureStorageBlobServiceConnection(new AzureSasCredential(sasToken), accountName);
		}
		ParsedSASQuery parsedSASQuery = new ParsedSASQuery(sasToken);
		string text = array.FirstOrDefault();
		List<string> list = array.Skip(1).ToList();
		if (parsedSASQuery.IsSASForContainer)
		{
			return new AzureStorageBlobContainerConnection(new AzureSasCredential(sasToken), accountName, text, list);
		}
		if (parsedSASQuery.IsSASForDirectory)
		{
			return new AzureStorageDataLakeConnection(new DataLakeDirectoryClient(new DataLakeUriBuilder(new Uri(GetBlobServiceUriString(accountName)))
			{
				FileSystemName = text,
				DirectoryOrFilePath = string.Join("/", list),
				Query = sasToken.DeletePrefixIfExists("?")
			}.ToUri()));
		}
		throw new ArgumentException("Unknown SAS type");
	}

	public static AzureStorageConnection CreateConnectionFromSASUri(Uri sasUri)
	{
		if (sasUri == null)
		{
			throw new ArgumentNullException("sasUri");
		}
		ParsedSASQuery parsedSASQuery = new ParsedSASQuery(sasUri.Query);
		if (parsedSASQuery.IsSASForContainer)
		{
			return new AzureStorageBlobContainerConnection(sasUri);
		}
		if (parsedSASQuery.IsSASForDirectory)
		{
			return new AzureStorageDataLakeConnection(new DataLakeDirectoryClient(sasUri));
		}
		if (parsedSASQuery.IsSASForService)
		{
			return new AzureStorageBlobServiceConnection(new BlobServiceClient(sasUri));
		}
		throw new ArgumentException("Unknown SAS type");
	}

	public static string GetAzureStorageDatabaseTitle(DatabaseRow databaseRow)
	{
		switch (AzureStorageAuthentication.FromString(databaseRow.Param1))
		{
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.AccessKey:
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.AzureADInteractive:
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureAccount:
			return GetBlobServiceUriString(databaseRow.Host);
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.PublicContainer:
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureDirectory:
			return GetBlobServiceUriString(databaseRow.Host) + "/" + databaseRow.Param2;
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.SharedAccessSignatureURL:
			return new Uri(databaseRow.Password).GetLeftPart(UriPartial.Path);
		case AzureStorageAuthentication.AzureStorageAuthenticationEnum.ConnectionString:
			return CloudStorageAccount.Parse(databaseRow.Password).BlobEndpoint.ToString();
		default:
			return "New database";
		}
	}

	public static string GetBlobServiceUriString(string accountName)
	{
		return "https://" + accountName + ".blob.core.windows.net";
	}

	public abstract List<FileLikeNode> GetObjectsStructure(bool dynamicLoading);

	public abstract bool TestConnection();
}
