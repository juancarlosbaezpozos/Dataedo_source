using System;

namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

public interface IDateTimeStatistics
{
	DateTime? Minimum { get; }

	DateTime? Maximum { get; }
}
