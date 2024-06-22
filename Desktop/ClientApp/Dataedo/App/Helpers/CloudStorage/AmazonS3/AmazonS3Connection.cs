using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace Dataedo.App.Helpers.CloudStorage.AmazonS3;

public class AmazonS3Connection
{
	public const char DefaultDelimeter = '/';

	private readonly AWSCredentials _credentials;

	private readonly RegionEndpoint _regionEndpoint;

	private readonly AmazonS3Client _client;

	public AmazonS3Connection(AWSCredentials credentials, RegionEndpoint regionEndpoint)
	{
		_credentials = credentials;
		_regionEndpoint = regionEndpoint;
		_client = new AmazonS3Client(credentials, regionEndpoint);
	}

	public HttpStatusCode TestArn(Arn arn)
	{
		string bucketName = GetBucketName(arn);
		string objectName = GetObjectName(arn);
		ListObjectsV2Request request = new ListObjectsV2Request
		{
			BucketName = bucketName,
			Prefix = objectName,
			MaxKeys = 1
		};
		return _client.ListObjectsV2(request).HttpStatusCode;
	}

	public List<S3Node> GetObjectsStructure(Arn arn)
	{
		return GetBucketStructure(GetObjectsList(arn));
	}

	public List<S3Object> GetObjectsList(Arn arn)
	{
		string bucketName = GetBucketName(arn);
		string objectName = GetObjectName(arn);
		List<S3Object> list = new List<S3Object>();
		ListObjectsV2Request listObjectsV2Request = new ListObjectsV2Request
		{
			BucketName = bucketName,
			Prefix = objectName
		};
		ListObjectsV2Response listObjectsV2Response;
		do
		{
			listObjectsV2Response = _client.ListObjectsV2(listObjectsV2Request);
			list.AddRange(listObjectsV2Response.S3Objects.ToList());
			listObjectsV2Request.ContinuationToken = listObjectsV2Response.NextContinuationToken;
		}
		while (listObjectsV2Response.IsTruncated);
		return list;
	}

	public GetObjectResponse GetObject(S3Object s3Object)
	{
		return _client.GetObject(s3Object.BucketName, s3Object.Key);
	}

	public SeekableS3Stream GetObjectStream(S3Object s3Object, long pageSize)
	{
		return new SeekableS3Stream(_client, s3Object.BucketName, s3Object.Key, pageSize);
	}

	public GetBucketPolicyResponse GetBucketPolicy(Arn arn)
	{
		string bucketName = GetBucketName(arn);
		return _client.GetBucketPolicy(bucketName);
	}

	public static string GetBucketName(string arnString, char delimiter = '/')
	{
		return GetBucketName(Arn.Parse(arnString), delimiter);
	}

	public static string GetBucketName(Arn arn, char delimiter = '/')
	{
		if (!string.Equals(arn.Service, "s3", StringComparison.OrdinalIgnoreCase))
		{
			throw new ArgumentException("ARN number not for s3 service");
		}
		return arn.Resource.Split(delimiter)[0];
	}

	public static string GetObjectName(Arn arn, char delimiter = '/')
	{
		if (arn.Service != "s3")
		{
			throw new ArgumentException("ARN number not for s3 service");
		}
		List<string> list = arn.Resource.Split(delimiter).ToList();
		list.RemoveAt(0);
		return string.Join(delimiter.ToString(), list);
	}

	public static List<S3Node> GetBucketStructure(List<S3Object> s3Objects)
	{
		List<S3Node> list = new List<S3Node>();
		foreach (S3Object s3Object in s3Objects)
		{
			S3Node s3Node = list.Where((S3Node x) => x.Name == s3Object.BucketName).FirstOrDefault();
			string text = s3Object.BucketName;
			if (s3Node == null)
			{
				s3Node = new S3Node(s3Object.BucketName);
				s3Node.FullPath = text;
				list.Add(s3Node);
			}
			string[] array = s3Object.Key.Split(new char[1] { '/' }, StringSplitOptions.RemoveEmptyEntries);
			S3Node s3Node2 = s3Node;
			string[] array2 = array;
			foreach (string item in array2)
			{
				text = text + "/" + item;
				S3Node s3Node3 = s3Node2.Nodes.Where((S3Node x) => x.Name == item).FirstOrDefault();
				if (s3Node3 == null)
				{
					s3Node3 = new S3Node(item);
					s3Node3.FullPath = text;
					s3Node2.Nodes.Add(s3Node3);
				}
				s3Node2 = s3Node3;
			}
			s3Node2.S3Object = s3Object;
		}
		return list;
	}
}
