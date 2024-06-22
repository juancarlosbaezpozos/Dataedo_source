using System.Collections.Generic;
using Dataedo.App.Import.DataLake.Model;

namespace Dataedo.App.Import.DataLake.Interfaces;

public interface IDataModel
{
	IEnumerable<ObjectModel> ObjectModels { get; }
}
