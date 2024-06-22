using System;
using System.Collections.Generic;
using Amazon.S3.Model;

namespace Dataedo.App.Helpers.CloudStorage.AmazonS3;

public class S3Node : FileLikeNode
{
	public override string Name { get; }

	public List<S3Node> Nodes { get; }

	public override IReadOnlyList<FileLikeNode> Children => Nodes;

	public S3Object S3Object { get; set; }

	public override long? Size => S3Object?.Size;

	public override DateTime? LastModified => S3Object?.LastModified;

	public override bool IsDirectoryLike
	{
		get
		{
			if (Nodes.Count <= 0 && Size.HasValue)
			{
				if (S3Object?.Key != null)
				{
					return S3Object.Key.EndsWith('/'.ToString());
				}
				return false;
			}
			return true;
		}
	}

	public string FullPath { get; set; }

	public S3Node(string name)
	{
		Name = name;
		Nodes = new List<S3Node>();
	}
}
