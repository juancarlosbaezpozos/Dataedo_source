using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools;

public class TableModel
{
	public int DatabaseId { get; set; }

	public bool AlreadyExists { get; set; }

	public string Name { get; set; }

	public string Schema { get; set; }

	public string Title { get; set; }

	public string Location { get; set; }

	public SharedObjectTypeEnum.ObjectType ObjectType { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype? Subtype { get; set; }

	public string Definition { get; set; }

	public string SubtypeString => SharedObjectSubtypeEnum.TypeToString(ObjectType, Subtype);

	public int? TableId { get; set; }

	public UserTypeEnum.UserType? Source { get; set; }

	public int NextColumnSortValue { get; set; }

	public string FullName
	{
		get
		{
			if (!string.IsNullOrEmpty(Schema))
			{
				return Schema + "." + Name;
			}
			return Name;
		}
	}

	public TableModel(int databaseId, string name, string schema, string title, string location, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype? subtype, string definition, UserTypeEnum.UserType source)
	{
		DatabaseId = databaseId;
		Name = name;
		Schema = schema;
		Title = title;
		Location = location;
		ObjectType = objectType;
		Subtype = subtype;
		Definition = definition;
		Source = source;
	}

	public TableModel()
	{
	}
}
