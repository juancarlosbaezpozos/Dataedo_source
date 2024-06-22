namespace Dataedo.App.Tools;

public interface ISelectableColumn
{
	int ColumnId { get; }

	int? ParentId { get; }

	bool Selected { get; set; }
}
