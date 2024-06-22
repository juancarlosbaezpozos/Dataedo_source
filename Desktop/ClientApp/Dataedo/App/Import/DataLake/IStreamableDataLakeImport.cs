using System.Collections.Generic;
using System.IO;
using Dataedo.App.Import.DataLake.Model;

namespace Dataedo.App.Import.DataLake;

public interface IStreamableDataLakeImport : IDataLakeImport
{
	IEnumerable<ObjectModel> GetObjectsFromStream(Stream stream);
}
