using System;
using System.Collections.Generic;
using Dataedo.App.Helpers.Files;

namespace Dataedo.App.Helpers.CloudStorage;

public abstract class FileLikeNode
{
	public abstract long? Size { get; }

	public abstract string Name { get; }

	public string ReadableSize
	{
		get
		{
			if (Size.HasValue)
			{
				return FilesHelper.ToReadableSize(Size.Value);
			}
			return string.Empty;
		}
	}

	public abstract DateTime? LastModified { get; }

	public abstract IReadOnlyList<FileLikeNode> Children { get; }

	public abstract bool IsDirectoryLike { get; }
}
