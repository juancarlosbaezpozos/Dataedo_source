using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MySQL;

namespace Dataedo.App.Data.Aurora;

internal class SynchronizeAurora : SynchronizeMySQL
{
	public SynchronizeAurora(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}
}
