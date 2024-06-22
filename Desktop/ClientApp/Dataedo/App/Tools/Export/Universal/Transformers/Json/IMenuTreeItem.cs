using System.Collections.Generic;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json;

internal interface IMenuTreeItem
{
	IMenuTreeItem Parent { get; }

	string Id { get; }

	string ObjectId { get; }

	string MenuType { get; }

	string MenuSubtype { get; }

	bool IsUserDefined { get; }

	string MenuName { get; }

	IEnumerable<string> ColumnNames { get; }

	IEnumerable<IMenuTreeItem> MenuChildren { get; }
}
