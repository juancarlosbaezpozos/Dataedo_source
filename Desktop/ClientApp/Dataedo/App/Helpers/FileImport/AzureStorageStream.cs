using System.IO;
using Azure;
using Dataedo.App.Helpers.Extensions;
using Dataedo.App.Import.Exceptions;

namespace Dataedo.App.Helpers.FileImport;

public class AzureStorageStream : Stream
{
	private readonly Stream _stream;

	public override bool CanRead => _stream.CanRead;

	public override bool CanSeek => _stream.CanSeek;

	public override bool CanWrite => _stream.CanWrite;

	public override long Length => _stream.Length;

	public override long Position
	{
		get
		{
			return _stream.Position;
		}
		set
		{
			_stream.Position = value;
		}
	}

	public AzureStorageStream(Stream stream)
	{
		_stream = stream;
	}

	public override void Flush()
	{
		_stream.Flush();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		try
		{
			return _stream.Read(buffer, offset, count);
		}
		catch (RequestFailedException ex)
		{
			throw new InvalidDataProvidedException(ex.Message.GetFirstLine(), ex);
		}
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		return _stream.Seek(offset, origin);
	}

	public override void SetLength(long value)
	{
		_stream.SetLength(value);
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		_stream.Write(buffer, offset, count);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing)
		{
			_stream.Close();
		}
		base.Dispose(disposing);
	}
}
