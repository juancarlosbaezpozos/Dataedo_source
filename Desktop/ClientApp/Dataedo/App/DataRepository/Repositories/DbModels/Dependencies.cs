using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.DataRepository.Models;

namespace Dataedo.App.DataRepository.Repositories.DbModels;

internal class Dependencies : IDependencies
{
	private DbRepository repository;

	public IEnumerable<IDependency> Uses { get; private set; }

	public IEnumerable<IDependency> UsedBy { get; private set; }

	public Dependencies(DbRepository repository, IEnumerable<DependencyRow> uses, IEnumerable<DependencyRow> usedBy)
	{
		this.repository = repository;
		Uses = uses?.Select((DependencyRow x) => new Dependency(repository, x));
		UsedBy = usedBy?.Select((DependencyRow x) => new Dependency(repository, x));
	}

	public object Clone()
	{
		return MemberwiseClone();
	}
}
