using System;
using System.Collections.Generic;
using System.Linq;

namespace Dataedo.App.Helpers.CloudStorage;

public class FileLikeNodeDynamicRootNode : FileLikeNode
{
	private readonly List<FileLikeNode> _children;

	public override long? Size => null;

	public override string Name => "root";

	public override DateTime? LastModified => null;

	public override IReadOnlyList<FileLikeNode> Children => _children;

	public override bool IsDirectoryLike => true;

	public FileLikeNodeDynamicRootNode(IReadOnlyList<FileLikeNode> objects)
	{
		_children = objects.ToList();
	}
}
