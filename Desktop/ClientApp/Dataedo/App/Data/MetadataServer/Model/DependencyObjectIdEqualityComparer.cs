using System.Collections.Generic;

namespace Dataedo.App.Data.MetadataServer.Model;

internal class DependencyObjectIdEqualityComparer : IEqualityComparer<DependencyRow>
{
	public bool Equals(DependencyRow x, DependencyRow y)
	{
		if (x.ObjectId.HasValue && y.ObjectId.HasValue)
		{
			return x.ObjectId == y.ObjectId;
		}
		return false;
	}

	public int GetHashCode(DependencyRow obj)
	{
		return obj.ObjectId ?? obj.Id;
	}
}
