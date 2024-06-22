using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Data.Documentations;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using Dataedo.SSISConnector;
using Dataedo.SSISConnector.DtsxParser;

namespace Dataedo.App.Data.SSIS;

internal class SynchronizeSSIS : SynchronizeDatabase
{
	private List<Package> packages;

	private List<Package> Packages
	{
		get
		{
			if (packages == null)
			{
				packages = GetPackages();
			}
			return packages;
		}
		set
		{
			packages = value;
		}
	}

	public SynchronizeSSIS(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	private List<Package> GetPackages()
	{
		List<Package> result = new List<Package>();
		object connection = synchronizeParameters.DatabaseRow.Connection;
		if (connection is Project project)
		{
			result = project.Packages;
		}
		else if (connection is Package item)
		{
			result = new List<Package> { item };
		}
		return result;
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		try
		{
			ReadCount(SharedObjectTypeEnum.ObjectType.Procedure, Packages.Count);
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		try
		{
			backgroundWorkerManager.ReportProgress("Retrieving database's objects");
			IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Procedure);
			foreach (Package package in Packages)
			{
				if (!IsObjectFiltered(package.Name, rulesByObjectType))
				{
					AddDBObject(package, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
				}
			}
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
	}

	public override List<DataProcessRow> GetDataLineage()
	{
		List<DocumentationObject> data = DB.Database.GetData();
		List<DataProcessRow> list = new List<DataProcessRow>();
		Dictionary<int, List<TableWithSchemaByDatabaseObject>> dictionary = new Dictionary<int, List<TableWithSchemaByDatabaseObject>>();
		foreach (Package package in Packages)
		{
			foreach (Task task in package.Tasks)
			{
				DataProcessRow dataProcessRow = new DataProcessRow
				{
					Name = (task.GetDisplayName() ?? ""),
					ParentName = package.Name,
					ParentSchema = string.Empty,
					ParentObjectType = SharedObjectTypeEnum.ObjectType.Procedure,
					Source = UserTypeEnum.UserType.DBMS,
					Script = task.Command
				};
				foreach (Connection connection in task.Connections)
				{
					foreach (int item in (from doc in data.Where((DocumentationObject db) => db.Host != null && db.Host.Equals(connection.Host) && db.Name != null && db.Name.Equals(connection.Database)).ToList()
						select doc.DatabaseId).ToList())
					{
						if (!dictionary.ContainsKey(item))
						{
							dictionary.Add(item, DB.Table.GetTablesAndViewsWithSchemaByDatabase(item, null));
						}
						TableWithSchemaByDatabaseObject dbObject = dictionary[item].Where(ConnectionPredicate(connection)).FirstOrDefault();
						string directionString = connection.GetDirectionString();
						if (connection.ConnectionType == ConnectionTypeEnum.SOURCE)
						{
							dataProcessRow.InflowRows.Add(CreateDataFlowRowForSSIS(dbObject, directionString, dataProcessRow));
						}
						else if (connection.ConnectionType == ConnectionTypeEnum.DESTINATION)
						{
							dataProcessRow.OutflowRows.Add(CreateDataFlowRowForSSIS(dbObject, directionString, dataProcessRow));
						}
					}
				}
				list.Add(dataProcessRow);
			}
		}
		return list;
	}

	private static Func<TableWithSchemaByDatabaseObject, bool> ConnectionPredicate(Connection connection)
	{
		List<string> list = connection.Table.Split('.').ToList();
		if (list.Count == 2)
		{
			string schema = list[0].Trim('[', ']');
			string name2 = list[1].Trim('[', ']');
			return (TableWithSchemaByDatabaseObject t) => t.Schema != null && t.Schema.Equals(schema) && t.Name != null && t.Name.Equals(name2);
		}
		if (list.Count == 1)
		{
			string name = list[0].Trim('[', ']');
			return (TableWithSchemaByDatabaseObject t) => string.IsNullOrEmpty(t.Schema) && t.Name != null && t.Name.Equals(name);
		}
		return (TableWithSchemaByDatabaseObject t) => false;
	}

	private static Func<TableWithSchemaByDatabaseObject, bool> SourcePredicate(Connection connection)
	{
		if (connection.ConnectionType != 0)
		{
			return (TableWithSchemaByDatabaseObject t) => false;
		}
		return ConnectionPredicate(connection);
	}

	private static Func<TableWithSchemaByDatabaseObject, bool> DestinationPredicate(Connection connection)
	{
		if (connection.ConnectionType != ConnectionTypeEnum.DESTINATION)
		{
			return (TableWithSchemaByDatabaseObject t) => false;
		}
		return ConnectionPredicate(connection);
	}

	private DataFlowRow CreateDataFlowRowForSSIS(TableWithSchemaByDatabaseObject dbObject, string direction, DataProcessRow dataProcess)
	{
		SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(dbObject.ObjectType);
		return new DataFlowRow
		{
			RowState = ManagingRowsEnum.ManagingRows.ForAdding,
			Direction = direction,
			ObjectType = objectType.Value,
			Name = dbObject.Name,
			Schema = dbObject.Schema,
			Source = UserTypeEnum.UserType.DBMS,
			Process = dataProcess,
			ObjectId = dbObject.Id,
			DatabaseId = dbObject.DatabaseId
		};
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetDependencies(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
		return true;
	}

	public override bool GetRelations(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetTriggers(string query, Form owner = null)
	{
		return true;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> RelationsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> TriggersQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		return new List<string> { "" };
	}
}
