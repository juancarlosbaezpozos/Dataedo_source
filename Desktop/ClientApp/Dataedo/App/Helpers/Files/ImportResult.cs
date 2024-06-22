using System.Collections.Generic;
using Dataedo.App.Import.DataLake.Model;

namespace Dataedo.App.Helpers.Files;

public class ImportResult
{
	public bool Success { get; set; } = true;


	public bool CancelEvent { get; set; }

	public List<ObjectModel> ObjectModels { get; set; } = new List<ObjectModel>();

}
