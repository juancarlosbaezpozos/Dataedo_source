using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MySQL;

namespace Dataedo.App.Data.PerconaMySQL;

internal class SynchronizePerconaMySQL : SynchronizeMySQL
{
	public SynchronizePerconaMySQL(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}
}
