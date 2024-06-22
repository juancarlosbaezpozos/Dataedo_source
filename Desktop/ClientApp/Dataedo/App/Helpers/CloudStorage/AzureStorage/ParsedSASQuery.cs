using System;
using System.Collections.Specialized;
using System.Web;

namespace Dataedo.App.Helpers.CloudStorage.AzureStorage;

public class ParsedSASQuery
{
	public string SignedServices { get; }

	public string SignedResourceTypes { get; }

	public string SignedResource { get; }

	public bool IsSASForContainer => SignedResource == "c";

	public bool IsSASForDirectory => SignedResource == "d";

	public bool IsSASForService => SignedResourceTypes.Contains("s");

	public DateTime? ExpirationDate { get; }

	public ParsedSASQuery(string sasQuery)
	{
		NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(sasQuery);
		SignedServices = nameValueCollection["ss"];
		SignedResourceTypes = nameValueCollection["srt"];
		SignedResource = nameValueCollection["sr"];
		string text = nameValueCollection["se"];
		if (!string.IsNullOrEmpty(text) && DateTime.TryParse(text, out var result))
		{
			ExpirationDate = result;
		}
	}

	public ParsedSASQuery(Uri uri)
		: this(uri?.Query)
	{
	}
}
