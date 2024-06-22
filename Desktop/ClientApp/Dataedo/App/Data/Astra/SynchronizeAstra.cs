using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.Cassandra;

namespace Dataedo.App.Data.Astra;

internal class SynchronizeAstra : SynchronizeCassandra
{
	public SynchronizeAstra(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}
}
