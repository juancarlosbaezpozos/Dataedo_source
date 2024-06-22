namespace Dataedo.App.Tools;

public class RepositoryDocumentationItem
{
	public string Title { get; set; }

	public string Type { get; set; }

	public string DBMSVersion { get; set; }

	public bool IsChecked { get; set; }

	public int DatabaseId { get; }

	public int DestinationDocumentationId { get; set; }

	public bool CanBeUsedForCopying => Type != "BUSINESS_GLOSSARY";

	public RepositoryDocumentationItem(string title, string type, int databaseId, string dbmsVersion)
	{
		Title = title;
		DatabaseId = databaseId;
		Type = type;
		DBMSVersion = dbmsVersion;
	}
}
