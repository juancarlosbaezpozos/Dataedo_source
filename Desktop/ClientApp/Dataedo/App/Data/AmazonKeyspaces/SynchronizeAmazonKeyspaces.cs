using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Cassandra;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.Cassandra;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.Model.Data.UniqueConstraints;

namespace Dataedo.App.Data.AmazonKeyspaces;

internal class SynchronizeAmazonKeyspaces : SynchronizeCassandra
{
	public SynchronizeAmazonKeyspaces(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		return true;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		try
		{
			IEnumerable<string> enumerable = from t in synchronizeParameters.DatabaseRow.tableRows
				where t.ToSynchronize
				select t.Name;
			if (enumerable != null && enumerable.Any())
			{
				ISession session = CommandsWithTimeout.Cassandra(synchronizeParameters.DatabaseRow.Connection);
				foreach (string item in enumerable)
				{
					TableMetadata table = session.Cluster.Metadata.GetTable(synchronizeParameters.DatabaseName, item);
					if (table != null)
					{
						TableColumn[] partitionKeys = table.PartitionKeys;
						foreach (TableColumn tableColumn in partitionKeys)
						{
							UniqueConstraintBaseObject dr = new UniqueConstraintBaseObject("primary_key", synchronizeParameters.DatabaseRow.Name, tableColumn.Keyspace, tableColumn.Table, tableColumn.Name, tableColumn.Index, null);
							AddUniqueConstraint(dr, synchronizeParameters.DatabaseRow.Type.Value);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	protected override string GetKeyspaceCondition()
	{
		return "keyspace_name = '" + synchronizeParameters.DatabaseName + "'";
	}
}
