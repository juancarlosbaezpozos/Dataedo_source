using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;

namespace Dataedo.App.Data.Azure;

internal class SynchronizeAzureSynapseServerless : SynchronizeAzureSQLDataWarehouse
{
	public SynchronizeAzureSynapseServerless(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override bool GetTriggers(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		return true;
	}
}
