using System;
using System.IO;
using Dataedo.App.Import.DataLake.Processing.ORCSupport.Protocol;
using ProtoBuf;

namespace Dataedo.App.Import.DataLake.Processing.ORCSupport;

internal class FileTail
{
	private readonly Stream inputStream;

	private PostScript postScript;

	private int postScriptLength;

	private Footer footer;

	public FileTail(Stream inputStream)
	{
		this.inputStream = inputStream;
		postScript = null;
		postScriptLength = -1;
		footer = null;
	}

	public PostScript GetPostScript()
	{
		if (postScript == null)
		{
			LoadPostScript();
		}
		return postScript;
	}

	public Footer GetFooter()
	{
		if (postScript == null)
		{
			LoadPostScript();
		}
		if (footer == null)
		{
			LoadFooter();
		}
		return footer;
	}

	private void LoadPostScript()
	{
		postScript = ReadPostScript(out var num);
		postScriptLength = num;
	}

	private void LoadFooter()
	{
		if (postScript == null)
		{
			LoadPostScript();
		}
		footer = ReadFooter();
	}

	private PostScript ReadPostScript(out int postScriptLength)
	{
		inputStream.Seek(-1L, SeekOrigin.End);
		postScriptLength = inputStream.ReadByte();
		if (postScriptLength < 0)
		{
			throw new InvalidOperationException("Read past end of stream");
		}
		postScriptLength = postScriptLength;
		inputStream.Seek(-1 - postScriptLength, SeekOrigin.End);
		byte[] buffer = new byte[postScriptLength];
		inputStream.Read(buffer, 0, postScriptLength);
		using MemoryStream source = new MemoryStream(buffer);
		PostScript obj = Serializer.Deserialize<PostScript>(source);
		if (obj.Magic != "ORC")
		{
			throw new InvalidDataException("Not an ORC file. PostScript doesn't contain magic bytes");
		}
		return obj;
	}

	private Footer ReadFooter()
	{
		byte[] buffer = new byte[(uint)postScript.FooterLength];
		inputStream.Seek(-1 - postScriptLength - (int)postScript.FooterLength, SeekOrigin.End);
		inputStream.Read(buffer, 0, (int)postScript.FooterLength);
		using Stream stream = new MemoryStream(buffer);
		if (postScript.Compression == CompressionKind.None)
		{
			return Serializer.Deserialize<Footer>(stream);
		}
		if (!ReadBlockHeader(stream, out var blockLength, out var isCompressed))
		{
			return null;
		}
		byte[] buffer2 = new byte[blockLength];
		stream.Read(buffer2, 0, blockLength);
		if (!isCompressed)
		{
			using (Stream source = new MemoryStream(buffer2))
			{
				return Serializer.Deserialize<Footer>(source);
			}
		}
		using Stream source2 = DecompressedStreamFactory.CreateDecompressedStream(postScript.Compression, new MemoryStream(buffer2));
		return Serializer.Deserialize<Footer>(source2);
	}

	private bool ReadBlockHeader(Stream inputStream, out int blockLength, out bool isCompressed)
	{
		int num = inputStream.ReadByte();
		if (num < 0)
		{
			blockLength = 0;
			isCompressed = false;
			return false;
		}
		int num2 = num | ((byte)inputStream.ReadByte() << 8) | ((byte)inputStream.ReadByte() << 16);
		blockLength = num2 >> 1;
		isCompressed = (num2 & 1) == 0;
		return true;
	}
}
