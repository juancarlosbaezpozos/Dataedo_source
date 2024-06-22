using System.Collections.Generic;
using ProtoBuf;

namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

[ProtoContract]
internal class ColumnType
{
	[ProtoMember(1)]
	public ColumnTypeKind Kind { get; set; }

	[ProtoMember(2, IsPacked = true)]
	public List<uint> SubTypes { get; } = new List<uint>();


	[ProtoMember(3)]
	public List<string> FieldNames { get; } = new List<string>();


	[ProtoMember(4)]
	public uint MaximumLength { get; set; }

	[ProtoMember(5)]
	public uint Precision { get; set; }

	[ProtoMember(6)]
	public uint Scale { get; set; }
}
