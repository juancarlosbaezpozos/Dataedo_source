using System;
using System.Collections.Generic;
using System.Linq;

namespace Dataedo.App.Classes.Synchronize.Tools;

public class DataLinkDataContainer
{
	public List<DataLinkData> Data { get; set; }

	public IEnumerable<DataLinkData> SortedData => Data;

	public IEnumerable<DataLinkData> DistinctData => (from x in Data?.GroupBy((DataLinkData x) => x.TermId)
		select x.First());

	public IEnumerable<DataLinkData> DistinctDataSorted => SortData(DistinctData);

	public bool HasData
	{
		get
		{
			List<DataLinkData> data = Data;
			if (data == null)
			{
				return false;
			}
			return data.Count > 0;
		}
	}

	public string DataLinksString => string.Join(", " + Environment.NewLine, DistinctDataSorted?.GroupBy((DataLinkData x) => x.TermId)?.Select((IGrouping<int?, DataLinkData> x) => x.Where((DataLinkData y) => y.TermId.HasValue).FirstOrDefault()?.TermTitle ?? "") ?? new string[0]);

	public string DataLinksDescription => string.Join(Environment.NewLine ?? "", DistinctDataSorted?.GroupBy((DataLinkData x) => x.TermId)?.Select((IGrouping<int?, DataLinkData> x) => x.Where((DataLinkData y) => y.TermId.HasValue).FirstOrDefault()?.ShortDescriptionFormatted ?? "") ?? new string[0]);

	public DataLinkDataContainer()
	{
		Data = new List<DataLinkData>();
	}

	public DataLinkDataContainer(DataLinkData dataLinkData)
		: this()
	{
		if (dataLinkData.HasData)
		{
			Data.Add(dataLinkData);
		}
	}

	public IEnumerable<DataLinkData> SortData(IEnumerable<DataLinkData> enumerable)
	{
		return enumerable?.OrderBy((DataLinkData x) => x.TermTitle);
	}
}
