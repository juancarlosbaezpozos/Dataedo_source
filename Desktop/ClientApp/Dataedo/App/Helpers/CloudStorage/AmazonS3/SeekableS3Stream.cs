using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace Dataedo.App.Helpers.CloudStorage.AmazonS3;

public class SeekableS3Stream : Stream
{
	internal class MetaData
	{
		public string Bucket;

		public string Key;

		public string S3eTag;

		public long Length;

		public long Position;

		public long PageSize = 26214400L;

		public long MaxPages = 20L;

		public Dictionary<long, byte[]> Pages = new Dictionary<long, byte[]>(20);

		public Dictionary<long, long> HotList = new Dictionary<long, long>(20);
	}

	private const long DEFAULT_PAGE_LENGTH = 26214400L;

	private const int DEFAULT_MAX_PAGE_COUNT = 20;

	private MetaData _metadata;

	private IAmazonS3 _s3;

	public long TotalRead { get; private set; }

	public long TotalLoaded { get; private set; }

	public override bool CanRead => true;

	public override bool CanSeek => true;

	public override bool CanWrite => false;

	public override long Length => _metadata.Length;

	public override long Position
	{
		get
		{
			return _metadata.Position;
		}
		set
		{
			Seek(value, (value < 0) ? SeekOrigin.End : SeekOrigin.Begin);
		}
	}

	public SeekableS3Stream(IAmazonS3 s3, string bucket, string key, long page = 26214400L, int maxpages = 20)
	{
		_s3 = s3;
		_metadata = new MetaData
		{
			Bucket = bucket,
			Key = key,
			PageSize = page,
			MaxPages = maxpages
		};
		GetObjectMetadataResponse result = _s3.GetObjectMetadataAsync(_metadata.Bucket, _metadata.Key).GetAwaiter().GetResult();
		_metadata.Length = result.ContentLength;
		_metadata.S3eTag = result.ETag;
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
	}

	public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
	{
		return base.ReadAsync(buffer, offset, count, cancellationToken);
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		if (_metadata.Position < 0 || _metadata.Position >= Length)
		{
			return 0;
		}
		long num = _metadata.Position;
		do
		{
			long num2 = num / _metadata.PageSize;
			long num3 = num % _metadata.PageSize;
			byte[] array = null;
			if (_metadata.Pages.ContainsKey(num2))
			{
				array = _metadata.Pages[num2];
			}
			if (array == null)
			{
				while (_metadata.Pages.Count >= _metadata.MaxPages)
				{
					long key = _metadata.Pages.OrderBy((KeyValuePair<long, byte[]> kv) => _metadata.HotList[kv.Key]).First().Key;
					_metadata.Pages.Remove(key);
				}
				long num4 = num2 * _metadata.PageSize;
				long num5 = num4 + Math.Min(_metadata.PageSize, _metadata.Length - num4);
				GetObjectRequest request = new GetObjectRequest
				{
					BucketName = _metadata.Bucket,
					Key = _metadata.Key,
					EtagToMatch = _metadata.S3eTag,
					ByteRange = new ByteRange(num4, num5)
				};
				array = (_metadata.Pages[num2] = new byte[num5 - num4]);
				if (!_metadata.HotList.ContainsKey(num2))
				{
					_metadata.HotList[num2] = 1L;
				}
				int num6 = 0;
				using (GetObjectResponse getObjectResponse = _s3.GetObjectAsync(request).GetAwaiter().GetResult())
				{
					do
					{
						num6 += getObjectResponse.ResponseStream.Read(array, num6, array.Length - num6);
					}
					while (num6 < array.Length);
				}
				TotalLoaded += num6;
			}
			else
			{
				_metadata.HotList[num2]++;
			}
			long num7 = Math.Min(array.Length - num3, count);
			Array.Copy(array, (int)num3, buffer, offset, (int)num7);
			offset += (int)num7;
			count -= (int)num7;
			num += (int)num7;
		}
		while (count > 0 && num < _metadata.Length);
		long num8 = num - _metadata.Position;
		TotalRead += num8;
		_metadata.Position = num;
		return (int)num8;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		long num = _metadata.Position;
		switch (origin)
		{
		case SeekOrigin.Begin:
			num = offset;
			break;
		case SeekOrigin.Current:
			num += offset;
			break;
		case SeekOrigin.End:
			num = _metadata.Length - Math.Abs(offset);
			break;
		}
		if (num < 0 || num > _metadata.Length)
		{
			throw new InvalidOperationException("Stream position is invalid.");
		}
		return _metadata.Position = num;
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}

	public override void Flush()
	{
		throw new NotImplementedException();
	}
}
