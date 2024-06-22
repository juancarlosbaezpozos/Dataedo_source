using Dataedo.App.Tools;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class TreeDependencyRow : BaseRow
{
	public int? DependencyId { get; set; }

	public int? Level { get; set; }

	public string Type { get; set; }

	public new int? Id { get; set; }

	public new string Name { get; set; }

	public string Schema { get; set; }

	public string Database { get; set; }

	public int? DatabaseId { get; set; }

	public string Server { get; set; }

	public int? ParentId { get; set; }

	public new SharedObjectTypeEnum.ObjectType? ObjectType { get; set; }

	public virtual string ObjectName => Database + "." + Schema + "." + Name;

	public virtual string ReferencingObjectIdString => Paths.EncodeInvalidPathCharacters(Database + "_" + Schema + "_" + Name);

	public string ReferencingDisplayName => ObjectName;
}
