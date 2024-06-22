using System.Collections.Generic;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.DataLake;

public interface IDataLakeImport
{
	DataLakeTypeEnum.DataLakeType DataLakeType { get; }

	SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype { get; }

	string DefaultExtension { get; }

	IEnumerable<string> Extensions { get; }

	bool DetermineByExtensionPriority { get; }

	string DocumentationLink { get; }

	bool IsValidData(string data);

	bool IsValidFile(string path);

	IEnumerable<ObjectModel> GetObjectsFromData(string data);

	IEnumerable<ObjectModel> GetObjectsFromFile(string path);
}
