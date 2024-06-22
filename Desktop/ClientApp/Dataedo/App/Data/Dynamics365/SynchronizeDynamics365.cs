using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.Dataverse;

namespace Dataedo.App.Data.Dynamics365;

internal class SynchronizeDynamics365 : SynchronizeDataverse
{
	public SynchronizeDynamics365(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}
}
