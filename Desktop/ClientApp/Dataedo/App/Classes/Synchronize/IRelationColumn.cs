using Dataedo.App.Classes.Synchronize.Tools;

namespace Dataedo.App.Classes.Synchronize;

public interface IRelationColumn
{
	int ColumnId { get; }

	string Name { get; }

	string Title { get; }

	string Path { get; }

	int? OrdinalPosition { get; }

	ColumnUniqueConstraintWithFkDataContainer UniqueConstraintsDataContainer { get; }
}
