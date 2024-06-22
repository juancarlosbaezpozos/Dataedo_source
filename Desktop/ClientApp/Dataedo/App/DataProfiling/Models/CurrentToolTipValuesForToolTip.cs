using System.Collections.Generic;

namespace Dataedo.App.DataProfiling.Models;

public class CurrentToolTipValuesForToolTip
{
	public Dictionary<string, string> RowDistributionValues { get; }

	public double? Min { get; set; }

	public double? Avg { get; set; }

	public double? Max { get; set; }

	public CurrentToolTipValuesForToolTip()
	{
		RowDistributionValues = new Dictionary<string, string>();
	}
}
