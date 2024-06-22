using System.Collections.Generic;
using System.Linq;
using Dataedo.App.DataRepository.Models;
using Dataedo.App.Tools.Export.Universal.Transformers.Json.Objects.Parts;

namespace Dataedo.App.Tools.Export.Universal.Transformers.Json;

internal static class ObjectTools
{
	public static IEnumerable<Column> FlattenColumns(IEnumerable<Column> items)
	{
		if (items == null)
		{
			return new List<Column>();
		}
		List<Column> list = new List<Column>(items);
		foreach (IEnumerable<Column> item in items.Select((Column x) => x.Children))
		{
			list.AddRange(FlattenColumns(item));
		}
		return list;
	}

	public static IEnumerable<IColumn> FlattenColumns(IEnumerable<IColumn> items)
	{
		if (items == null)
		{
			return new List<IColumn>();
		}
		List<IColumn> list = new List<IColumn>(items);
		foreach (IEnumerable<IColumn> item in items.Select((IColumn x) => x.Children))
		{
			list.AddRange(FlattenColumns(item));
		}
		return list;
	}
}
