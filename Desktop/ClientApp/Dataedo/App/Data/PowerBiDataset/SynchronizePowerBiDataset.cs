using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.SsasTabular;

namespace Dataedo.App.Data.PowerBiDataset;

internal class SynchronizePowerBiDataset : SynchronizeSsasTabular
{
	public SynchronizePowerBiDataset(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}
}
