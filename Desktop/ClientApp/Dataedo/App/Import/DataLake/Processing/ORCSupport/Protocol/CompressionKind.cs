namespace Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;

internal enum CompressionKind
{
	None = 0,
	Zlib = 1,
	Snappy = 2,
	Lzo = 3,
	Lz4 = 4,
	Zstd = 5
}
