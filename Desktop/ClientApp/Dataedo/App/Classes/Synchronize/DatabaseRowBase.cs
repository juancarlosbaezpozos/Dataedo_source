using Dataedo.DataProcessing.CustomFields;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public abstract class DatabaseRowBase : ISupportsCustomFields
{
	public int? Id { get; set; }

	public string Title { get; set; }

	public string Description { get; set; }

	public string DescriptionSearch { get; set; }

	public abstract string Name { get; set; }

	public CustomFieldContainer CustomFields { get; set; }

	public SharedObjectTypeEnum.ObjectType? ObjectTypeValue { get; set; } = SharedObjectTypeEnum.ObjectType.Database;


	public int IdValue => Id ?? (-1);

	public virtual string ObjectId => Id.ToString();
}
