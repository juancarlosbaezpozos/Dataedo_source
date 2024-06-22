using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Cassandra;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.Cassandra;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.Model.Data.UniqueConstraints;
using Dataedo.Model.Extensions;

namespace Dataedo.App.Data.CosmosDbCassandraAPI;

internal class SynchronizeCosmosDbCassandraAPI : SynchronizeCassandra
{
	private const string FieldName = "name";

	private const string FieldSchema = "schema";

	protected override string GetKeyspaceCondition()
	{
		return "keyspace_name = '" + synchronizeParameters.DatabaseName + "'";
	}

	public SynchronizeCosmosDbCassandraAPI(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		return true;
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (base.ObjectsCounter != null)
		{
			backgroundWorkerManager.ReportProgress("Retrieving database's objects");
			try
			{
				using RowSet rowSet = CommandsWithTimeout.Cassandra(synchronizeParameters.DatabaseRow.Connection).Execute(new SimpleStatement(query.Query));
				foreach (Row item in rowSet)
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					if (!FilterUserObjectsFilter(item.Field<string>("name"), query.ObjectType) && !CosmosDbCassandraApiSupport.SystemDatabases.Contains(item.Field<string>("schema")))
					{
						AddDBObject(item, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
					}
				}
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
				return false;
			}
		}
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
}
