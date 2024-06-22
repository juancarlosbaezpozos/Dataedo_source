using Dataedo.App.Classes.Synchronize.Tools;

namespace Dataedo.App.Classes.Synchronize;

public class RelationColumn : IRelationColumn
{
	private ColumnRow row;

	public int ColumnId => row.Id;

	public string Name => row.Name;

	public string Title => row.Title;

	public string Path => row.Path;

	public int? OrdinalPosition => row.Position;

	public ColumnUniqueConstraintWithFkDataContainer UniqueConstraintsDataContainer => row.UniqueConstraintsDataContainer;

	public RelationColumn(ColumnRow row)
	{
		this.row = row;
	}
}
