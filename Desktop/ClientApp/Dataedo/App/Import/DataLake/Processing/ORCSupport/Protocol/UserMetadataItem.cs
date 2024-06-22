using ProtoBuf;

namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

[ProtoContract]
internal class UserMetadataItem
{
	[ProtoMember(1)]
	public string Name { get; set; }

	[ProtoMember(2)]
	public byte[] Value { get; set; }
}
