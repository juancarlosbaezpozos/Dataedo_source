using Dataedo.Model.Data.MongoDB;

namespace Dataedo.App.Import.DataLake.Model;

internal class JsonFieldModel : FieldModel
{
	public JsonFieldModel(BsonColumn bsonColumn, FieldModel parent)
	{
		base.Name = bsonColumn.Name;
		base.ObjectSubtype = bsonColumn.Type;
		base.DataType = bsonColumn.DataType;
		base.DataTypeSize = null;
		base.Nullable = !bsonColumn.Required;
		base.Position = bsonColumn.Position;
		base.ParentId = null;
		base.ParentField = parent;
		base.Path = bsonColumn.Path;
		base.Level = bsonColumn.Level;
		base.ObjectSubtype = bsonColumn.Type;
	}
}
