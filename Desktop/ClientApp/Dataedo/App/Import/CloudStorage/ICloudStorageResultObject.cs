using System.Collections.Generic;
using Dataedo.App.Helpers.FileImport;

namespace Dataedo.App.Import.CloudStorage;

public interface ICloudStorageResultObject
{
	string WarningMessage { get; }

	List<ImportItem> GetStreamableImportItems();

	List<string> CloudStorageItems();
}
