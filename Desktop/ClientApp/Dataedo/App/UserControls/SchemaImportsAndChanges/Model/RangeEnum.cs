using System.Collections.Generic;
using System.Linq;

namespace Dataedo.App.UserControls.SchemaImportsAndChanges.Model;

public class RangeEnum
{
	public enum Range
	{
		LastChange = 0,
		Last7Days = 1,
		LastMonth = 2,
		Last12Months = 3,
		All = 4,
		CustomDateRange = 5
	}

	public class RangeModel
	{
		public Range Value { get; set; }

		public string Title { get; set; }

		public RangeModel(Range value, string title)
		{
			Value = value;
			Title = title;
		}
	}

	private static readonly Dictionary<Range, string> rangeDisplayString = new Dictionary<Range, string>
	{
		{
			Range.LastChange,
			"Last change"
		},
		{
			Range.Last7Days,
			"Last 7 days"
		},
		{
			Range.LastMonth,
			"Last month"
		},
		{
			Range.Last12Months,
			"Last 12 months"
		},
		{
			Range.All,
			"All"
		},
		{
			Range.CustomDateRange,
			"Custom date range"
		}
	};

	public static List<RangeModel> GetRanges()
	{
		return rangeDisplayString.Select((KeyValuePair<Range, string> x) => new RangeModel(x.Key, x.Value)).ToList();
	}
}
