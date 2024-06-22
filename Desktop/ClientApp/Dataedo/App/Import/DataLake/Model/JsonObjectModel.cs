using System.Linq;
using Dataedo.Model.Data.MongoDB;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.DataLake.Model;

public class JsonObjectModel : ObjectModel
{
	public JsonObjectModel(string name, string path, SharedObjectTypeEnum.ObjectType objectType, DataLakeTypeEnum.DataLakeType dataLakeType, BsonTable bsonTable)
		: base(name, path, DataLakeTypeEnum.DataLakeType.JSON, objectType, DataLakeTypeEnum.GetObjectSubtype(objectType, dataLakeType))
	{
		foreach (BsonColumn field in bsonTable.Columns)
		{
			FieldModel parent = base.Fields.Where((FieldModel x) => x.FullName == field.Path).FirstOrDefault();
			JsonFieldModel item = new JsonFieldModel(field, parent);
			base.Fields.Add(item);
		}
	}

	public JsonObjectModel(SharedObjectTypeEnum.ObjectType objectType, DataLakeTypeEnum.DataLakeType dataLakeType, BsonTable bsonTable)
		: this("structure", "structure", objectType, dataLakeType, bsonTable)
	{
	}
}
