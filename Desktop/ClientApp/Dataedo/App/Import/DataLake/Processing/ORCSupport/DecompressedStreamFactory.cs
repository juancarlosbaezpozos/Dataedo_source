using System;
using System.IO;
using System.IO.Compression;
using Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;
using IronSnappy;

namespace Dataedo.App.Import.DataLake.Processing.ORCSupport;

internal class DecompressedStreamFactory
{
	public static Stream CreateDecompressedStream(CompressionKind compressionKind, Stream compressedStream)
	{
		switch (compressionKind)
		{
		case CompressionKind.None:
			return compressedStream;
		case CompressionKind.Snappy:
		{
			using MemoryStream memoryStream = new MemoryStream();
			compressedStream.CopyTo(memoryStream);
			return new MemoryStream(IronSnappy.Snappy.Decode(memoryStream.ToArray()));
		}
		case CompressionKind.Zlib:
			return new DeflateStream(compressedStream, CompressionMode.Decompress, leaveOpen: true);
		default:
			throw new NotImplementedException($"Unimplemented {compressionKind} decompression");
		}
	}
}
