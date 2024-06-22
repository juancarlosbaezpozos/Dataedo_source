namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

public interface IDoubleStatistics
{
	double? Minimum { get; }

	double? Maximum { get; }

	double? Sum { get; }
}
