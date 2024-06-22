using Dataedo.App.Import.DataLake.Processing.CsvSupport;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.DataLake.Model;

internal class CsvObjectModel : ObjectModel
{
	public CsvObjectModel(string name, string path, SharedObjectTypeEnum.ObjectType objectType, DataLakeTypeEnum.DataLakeType dataLakeType, CsvTable bsonTable)
		: base(name, path, DataLakeTypeEnum.DataLakeType.CSV, objectType, DataLakeTypeEnum.GetObjectSubtype(objectType, dataLakeType))
	{
		foreach (CsvColumn column in bsonTable.Columns)
		{
			CsvFieldModel item = new CsvFieldModel(column);
			base.Fields.Add(item);
		}
	}
}
