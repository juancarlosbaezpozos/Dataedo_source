using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.DDLGenerating;

public class DDLObject
{
	public int Id { get; set; }

	public int ParentId { get; set; }

	public string DisplayName { get; set; }

	public string Name { get; set; }

	public string Schema { get; set; }

	public UserTypeEnum.UserType? Source { get; set; }

	public bool Checked { get; set; }

	public string DatatypeLen { get; set; }

	public bool Nullable { get; set; }

	public string DefaultValue { get; set; }

	public SharedObjectTypeEnum.ObjectType? Type { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype Subtype { get; set; }
}
