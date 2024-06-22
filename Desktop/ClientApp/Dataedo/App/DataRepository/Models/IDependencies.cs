using System.Collections.Generic;

namespace Dataedo.App.DataRepository.Models;

public interface IDependencies
{
	IEnumerable<IDependency> Uses { get; }

	IEnumerable<IDependency> UsedBy { get; }
}
