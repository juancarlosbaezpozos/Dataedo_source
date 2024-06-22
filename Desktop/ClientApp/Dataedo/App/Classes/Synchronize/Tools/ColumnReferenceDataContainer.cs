using System;
using System.Collections.Generic;
using System.Linq;

namespace Dataedo.App.Classes.Synchronize.Tools;

public class ColumnReferenceDataContainer
{
	public List<ColumnReferenceData> Data { get; set; }

	public IEnumerable<ColumnReferenceData> SortedData => SortData(Data);

	public IEnumerable<ColumnReferenceData> DistinctData => (from x in Data?.GroupBy((ColumnReferenceData x) => x.PkObjectId)
		select x.First());

	public IEnumerable<ColumnReferenceData> DistinctDataSorted => SortData(DistinctData);

	public string ReferencesString => string.Join(Environment.NewLine, SortedData?.GroupBy((ColumnReferenceData x) => x.PkObjectId)?.Select((IGrouping<int?, ColumnReferenceData> x) => x.Where((ColumnReferenceData y) => !string.IsNullOrEmpty(y.ReferenceString)).FirstOrDefault().ReferenceString) ?? new string[0]);

	public string ReferencesStringCommaNewLineDelimited => string.Join(", " + Environment.NewLine, SortedData?.GroupBy((ColumnReferenceData x) => x.PkObjectId)?.Select((IGrouping<int?, ColumnReferenceData> x) => x.Where((ColumnReferenceData y) => !string.IsNullOrEmpty(y.ReferenceString)).FirstOrDefault().ReferenceString) ?? new string[0]);

	public string ReferencesStringCommaDelimited => string.Join(", ", SortedData?.GroupBy((ColumnReferenceData x) => x.PkObjectId)?.Select((IGrouping<int?, ColumnReferenceData> x) => x.Where((ColumnReferenceData y) => !string.IsNullOrEmpty(y.ReferenceString)).FirstOrDefault().ReferenceString) ?? new string[0]);

	public string ReferencesStringWithColumnName => string.Join(Environment.NewLine, from x in SortedData
		where !string.IsNullOrEmpty(x.ReferenceStringWithColumnName)
		select x.ReferenceStringWithColumnName);

	public ColumnReferenceDataContainer()
	{
		Data = new List<ColumnReferenceData>();
	}

	public ColumnReferenceDataContainer(ColumnReferenceData columnReferenceData)
		: this()
	{
		if (columnReferenceData.PkColumnId.HasValue)
		{
			Data.Add(columnReferenceData);
		}
	}

	public void AddReference(RelationRow relation, string pkColumnName, int pkColumnId)
	{
		ColumnReferenceData item = new ColumnReferenceData(relation, pkColumnName, pkColumnId);
		Data.Add(item);
	}

	public void RemoveReference(int relationId)
	{
		Data.Remove(Data.FirstOrDefault((ColumnReferenceData x) => x.RelationId == relationId));
	}

	public IEnumerable<ColumnReferenceData> SortData(IEnumerable<ColumnReferenceData> enumerable)
	{
		return enumerable?.OrderBy((ColumnReferenceData x) => (!x.IsSameDatabase) ? 1 : 0).ThenBy((ColumnReferenceData x) => x.PkDatabaseName).ThenBy((ColumnReferenceData x) => x.ReferenceStringWithColumnName);
	}
}
