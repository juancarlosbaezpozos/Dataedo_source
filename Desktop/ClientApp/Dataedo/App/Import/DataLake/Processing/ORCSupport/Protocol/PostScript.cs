using System.Collections.Generic;
using ProtoBuf;

namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

[ProtoContract]
internal class PostScript
{
	[ProtoMember(1)]
	public ulong FooterLength { get; set; }

	[ProtoMember(2)]
	public CompressionKind Compression { get; set; }

	[ProtoMember(3)]
	public ulong CompressionBlockSize { get; set; }

	[ProtoMember(4, IsPacked = true)]
	public List<uint> Version { get; set; } = new List<uint>();


	public uint? VersionMajor
	{
		get
		{
			if (Version.Count <= 0)
			{
				return null;
			}
			return Version[0];
		}
	}

	public uint? VersionMinor
	{
		get
		{
			if (Version.Count <= 1)
			{
				return null;
			}
			return Version[1];
		}
	}

	[ProtoMember(5)]
	public ulong MetadataLength { get; set; }

	[ProtoMember(6)]
	public uint WriterVersion { get; set; }

	[ProtoMember(8000)]
	public string Magic { get; set; }
}
