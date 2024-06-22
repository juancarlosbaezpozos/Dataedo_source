namespace Dataedo.App.Data.Tableau;

public class TableauConnection
{
	public string SiteId { get; set; }

	public string Token { get; set; }

	public string ApiVersion { get; set; }

	public string Url => ConnectionString.Host + "/api/" + ApiVersion;

	public string SiteUrl => Url + "/sites/" + SiteId;

	public string GraphQLUrl => ConnectionString.Host + "/api/metadata/graphql";

	public TableauConnectionString ConnectionString { get; set; }

	public TableauConnection(TableauConnectionString connectionString, string site, string token, string apiVersion)
	{
		ConnectionString = connectionString;
		SiteId = site;
		Token = token;
		ApiVersion = apiVersion;
	}
}
