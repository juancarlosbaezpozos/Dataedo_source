using System;
using Dataedo.App.DataProfiling.Tools;

namespace Dataedo.App.DataProfiling.Models;

public class ColumnTopValues
{
	public int Id { get; set; }

	public int NavigationId { get; set; }

	public string Value { get; set; }

	public string Rows { get; set; }

	public long? RowsL { get; set; }

	public decimal? Percents { get; set; }

	public double? NotFormattedPercents { get; set; }

	public ColumnTopValues(string value, long? rows, long allrows, int id, int navigationId)
	{
		Value = value;
		RowsL = rows;
		if (!rows.HasValue)
		{
			NotFormattedPercents = null;
			Percents = null;
		}
		else
		{
			NotFormattedPercents = ((allrows == 0L) ? 0.0 : ((double)rows.Value / (double)allrows * 100.0));
			Percents = Truncate((decimal)NotFormattedPercents.Value, 1);
		}
		Id = id;
		NavigationId = navigationId;
		if (!RowsL.HasValue)
		{
			Rows = string.Empty;
		}
		else
		{
			Rows = DataProfilingStringFormatter.FormatTopIntValues(RowsL.GetValueOrDefault());
		}
	}

	private static decimal Truncate(decimal d, byte decimals)
	{
		decimal num = Math.Round(d, decimals);
		if (d > 0m && num > d)
		{
			return num - new decimal(1, 0, 0, isNegative: false, decimals);
		}
		if (d < 0m && num < d)
		{
			return num + new decimal(1, 0, 0, isNegative: false, decimals);
		}
		return num;
	}
}
