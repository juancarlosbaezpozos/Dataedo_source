using System.Collections.Generic;

namespace Dataedo.App.Data.MetadataServer.Model;

internal class DependencyDescriptionEqualityComparer : IEqualityComparer<DependencyRow>
{
	public bool Equals(DependencyRow x, DependencyRow y)
	{
		if (x.DependencyDescriptionsData.Id.HasValue && y.DependencyDescriptionsData.Id.HasValue)
		{
			return x.DependencyDescriptionsData.Id == y.DependencyDescriptionsData.Id;
		}
		return false;
	}

	public int GetHashCode(DependencyRow obj)
	{
		return obj.DependencyDescriptionsData.Id ?? obj.Id;
	}
}
