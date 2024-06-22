using DevExpress.XtraEditors.DXErrorProvider;

namespace Dataedo.App.Tools;

public class CopyDocumentationModel : IDXDataErrorInfo
{
	public string Title { get; set; }

	public string Type { get; set; }

	public string DBMSVersion { get; set; }

	public bool IsChecked { get; set; }

	public int DatabaseId { get; set; }

	public int DestinationDocumentationId { get; set; }

	public bool IsValid { get; set; }

	public CopyDocumentationModel(string title, int databaseId, int destinationDatabaseId, string type, string dbmsVersion)
	{
		Title = title;
		DatabaseId = databaseId;
		DestinationDocumentationId = destinationDatabaseId;
		IsValid = true;
		Type = type;
		DBMSVersion = dbmsVersion;
	}

	public void GetPropertyError(string propertyName, ErrorInfo info)
	{
		if (!(propertyName != "DestinationDocumentationId") && !IsValid)
		{
			info.ErrorText = "Destination documentation can not be empty";
			info.ErrorType = ErrorType.Critical;
		}
	}

	public void GetError(ErrorInfo info)
	{
	}
}
