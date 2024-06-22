using System;

namespace Dataedo.App.Data.Tableau;

public class TableauConnectionString
{
	public string Host { get; set; }

	public string Site { get; set; }

	public string SiteForConnection
	{
		get
		{
			if ("Default".Equals(Site))
			{
				return string.Empty;
			}
			return Site;
		}
	}

	public string Username { get; set; }

	public string Password { get; set; }

	public string Token { get; set; }

	public bool IsToken { get; set; }

	public string BaseUrl => Host + "/api/2.4";

	public override string ToString()
	{
		return $"{Host};{Site};{Username};{Password};{Token};{IsToken}";
	}

	public TableauConnectionString(string connectionString)
	{
		string[] array = connectionString.Split(';');
		if (array.Length < 6)
		{
			throw new ArgumentException("Invalid connection string.");
		}
		Host = array[0];
		Site = array[1];
		Username = array[2];
		Password = array[3];
		Token = array[4];
		IsToken = bool.Parse(array[5]);
	}

	public TableauConnectionString(string host, string site, string userName, string password, string token, bool isToken)
	{
		Host = host;
		Site = site;
		Username = userName;
		Password = password;
		Token = token;
		IsToken = isToken;
	}
}
