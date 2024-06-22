using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.DB2;

namespace Dataedo.App.Data.IBMDb2BigQuery;

internal class SynchronizeIBMDb2BigQuery : SynchronizeDb2
{
	public SynchronizeIBMDb2BigQuery(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}
}
