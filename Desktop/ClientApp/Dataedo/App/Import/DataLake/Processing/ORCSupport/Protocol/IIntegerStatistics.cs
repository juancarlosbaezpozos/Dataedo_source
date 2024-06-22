namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

public interface IIntegerStatistics
{
	long? Minimum { get; }

	long? Maximum { get; }

	long? Sum { get; }
}
