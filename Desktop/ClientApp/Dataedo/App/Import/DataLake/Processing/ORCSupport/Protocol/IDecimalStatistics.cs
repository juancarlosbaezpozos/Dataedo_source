namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

public interface IDecimalStatistics
{
	decimal? Minimum { get; }

	decimal? Maximum { get; }

	decimal? Sum { get; }
}
