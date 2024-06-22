namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

public interface IColumnStatistics
{
	ulong NumberOfValues { get; }

	IIntegerStatistics IntStatistics { get; }

	IDoubleStatistics DoubleStatistics { get; }

	IStringStatistics StringStatistics { get; }

	IBooleanStatistics BooleanStatistics { get; }

	IDecimalStatistics DecimalStatistics { get; }

	IDateTimeStatistics DateStatistics { get; }

	IBinaryStatistics BinaryStatistics { get; }

	IDateTimeStatistics TimestampStatistics { get; }

	bool HasNull { get; }
}
