using System;
using ProtoBuf;

namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

[ProtoContract]
public class TimestampStatistics : IDateTimeStatistics
{
	private DateTime Epoch { get; set; } = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);


	[ProtoMember(1, DataFormat = DataFormat.ZigZag)]
	public long? Minimum { get; set; }

	[ProtoMember(2, DataFormat = DataFormat.ZigZag)]
	public long? Maximum { get; set; }

	DateTime? IDateTimeStatistics.Minimum
	{
		get
		{
			if (!Minimum.HasValue)
			{
				return null;
			}
			return Epoch.AddTicks(Minimum.Value * 10000);
		}
	}

	DateTime? IDateTimeStatistics.Maximum
	{
		get
		{
			if (!Maximum.HasValue)
			{
				return null;
			}
			return Epoch.AddTicks(Maximum.Value * 10000);
		}
	}
}
