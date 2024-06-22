using System.Collections.Generic;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json.MenuTree;

internal abstract class Folder : IMenuTreeItem
{
	public IMenuTreeItem Parent { get; private set; }

	public string Id { get; set; }

	public string ObjectId { get; set; }

	public string MenuType { get; set; }

	public string MenuSubtype => null;

	public bool IsUserDefined => false;

	public string MenuName { get; set; }

	public IEnumerable<string> ColumnNames { get; set; }

	public IEnumerable<IMenuTreeItem> MenuChildren { get; set; }

	public Folder(IMenuTreeItem parent)
	{
		Parent = parent;
	}
}
