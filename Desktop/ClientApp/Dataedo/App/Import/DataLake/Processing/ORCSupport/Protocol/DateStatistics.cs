using System;
using ProtoBuf;

namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

[ProtoContract]
public class DateStatistics : IDateTimeStatistics
{
	private DateTime Epoch { get; set; } = new DateTime(2015, 1, 1, 0, 0, 0, DateTimeKind.Utc);


	[ProtoMember(1, DataFormat = DataFormat.ZigZag)]
	public int? Minimum { get; set; }

	[ProtoMember(2, DataFormat = DataFormat.ZigZag)]
	public int? Maximum { get; set; }

	DateTime? IDateTimeStatistics.Minimum
	{
		get
		{
			if (!Minimum.HasValue)
			{
				return null;
			}
			return Epoch.AddTicks(Minimum.Value * 864000000000L);
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
			return Epoch.AddTicks(Maximum.Value * 864000000000L);
		}
	}
}
