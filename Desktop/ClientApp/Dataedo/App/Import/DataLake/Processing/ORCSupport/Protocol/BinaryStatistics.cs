using ProtoBuf;

namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

[ProtoContract]
public class BinaryStatistics : IBinaryStatistics
{
	[ProtoMember(1, DataFormat = DataFormat.ZigZag)]
	public long? Sum { get; set; }
}
