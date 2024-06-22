namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

public interface IStringStatistics
{
	string Minimum { get; }

	string Maximum { get; }

	long? Sum { get; }
}
