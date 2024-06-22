using ProtoBuf;

namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

[ProtoContract]
public class DecimalStatistics : IDecimalStatistics
{
	[ProtoMember(1)]
	public string Minimum { get; set; }

	decimal? IDecimalStatistics.Minimum
	{
		get
		{
			if (!string.IsNullOrEmpty(Minimum))
			{
				return decimal.Parse(Minimum);
			}
			return null;
		}
	}

	[ProtoMember(2)]
	public string Maximum { get; set; }

	decimal? IDecimalStatistics.Maximum
	{
		get
		{
			if (!string.IsNullOrEmpty(Maximum))
			{
				return decimal.Parse(Maximum);
			}
			return null;
		}
	}

	[ProtoMember(3)]
	public string Sum { get; set; }

	decimal? IDecimalStatistics.Sum
	{
		get
		{
			if (!string.IsNullOrEmpty(Sum))
			{
				return decimal.Parse(Sum);
			}
			return null;
		}
	}
}
