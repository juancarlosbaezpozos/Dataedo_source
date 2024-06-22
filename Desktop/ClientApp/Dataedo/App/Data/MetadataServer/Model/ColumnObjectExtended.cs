using System.Collections.Generic;
using Dataedo.Model.Data.Tables.Columns;

namespace Dataedo.App.Data.MetadataServer.Model;

internal class ColumnObjectExtended<T> where T : ColumnObject
{
	public ColumnObject ColumnObject { get; set; }

	public ColumnObjectExtended<T> ParentColumn { get; set; }

	public IEnumerable<ColumnObjectExtended<T>> Subcolumns { get; set; }

	public ColumnObjectExtended(ColumnObject columnObject)
	{
		ColumnObject = columnObject;
	}
}
