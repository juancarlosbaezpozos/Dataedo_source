using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools;

public class SheetNameData
{
	public int? DocumentationId { get; set; }

	public int? ObjectId { get; set; }

	public string Schema { get; set; }

	public string Name { get; set; }

	public string Title { get; set; }

	public SharedObjectTypeEnum.ObjectType? ObjectType { get; set; }

	public string SheetName { get; set; }

	public SheetNameData(int? documentationId, int? objectId, string schema, string name, SharedObjectTypeEnum.ObjectType? objectType, string sheetName)
	{
		DocumentationId = documentationId;
		ObjectId = objectId;
		Schema = schema;
		Name = name;
		ObjectType = objectType;
		SheetName = sheetName;
	}

	public SheetNameData(int? documentationId, int? objectId, string sheetName, string title, SharedObjectTypeEnum.ObjectType? objectType)
	{
		DocumentationId = documentationId;
		ObjectId = objectId;
		Title = title;
		ObjectType = objectType;
		SheetName = sheetName;
	}

	public SheetNameData(int? documentationId, int? objectId, string sheetName)
	{
		DocumentationId = documentationId;
		ObjectId = objectId;
		SheetName = sheetName;
	}
}
