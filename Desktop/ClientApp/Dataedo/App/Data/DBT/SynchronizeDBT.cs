using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.DBTConnector;
using Dataedo.DBTConnector.Models;
using Dataedo.Model.Data.Object;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.DBT;

public class SynchronizeDBT : SynchronizeDatabase
{
	private const string MATERIALIZATION_EPHEMERAL = "ephemeral";

	public SynchronizeDBT(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		try
		{
			DBTPackage dBTPackage = CommandsWithTimeout.DBT(synchronizeParameters.DatabaseRow.Connection);
			if (dBTPackage == null)
			{
				return false;
			}
			int count = dBTPackage.GetModelNodes().Count;
			ReadCount(FilterObjectTypeEnum.FilterObjectType.View, count);
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (base.ObjectsCounter != null)
		{
			try
			{
				backgroundWorkerManager.ReportProgress("Retrieving database's objects");
				DBTPackage dBTPackage = CommandsWithTimeout.DBT(synchronizeParameters.DatabaseRow.Connection);
				if (dBTPackage == null)
				{
					return false;
				}
				IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.View);
				foreach (Node modelNode in dBTPackage.GetModelNodes())
				{
					if (!IsObjectFiltered(modelNode.Name, rulesByObjectType))
					{
						AddDBObject(modelNode, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager);
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

	public override void GetObjectsDetails(BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
	}

	public override bool GetRelations(string query, Form owner = null)
	{
		DBTPackage dBTPackage = CommandsWithTimeout.DBT(synchronizeParameters.DatabaseRow.Connection);
		if (dBTPackage == null)
		{
			return false;
		}
		foreach (Relationship relationship in dBTPackage.GetRelationships())
		{
			AddRelation(relationship, SharedDatabaseTypeEnum.DatabaseType.DBT);
		}
		return true;
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		DBTPackage dBTPackage = CommandsWithTimeout.DBT(synchronizeParameters.DatabaseRow.Connection);
		if (dBTPackage == null)
		{
			return false;
		}
		List<Node> modelNodes = dBTPackage.GetModelNodes();
		List<ColumnRow> list = new List<ColumnRow>();
		foreach (Node item2 in modelNodes)
		{
			int num = 1;
			foreach (Column column in item2.Columns)
			{
				ColumnRow item = new ColumnRow
				{
					Name = column.Name,
					TableName = item2.Name,
					TableSchema = item2.Schema,
					DatabaseName = synchronizeParameters.DatabaseRow.Name,
					DataType = column.DataType,
					Nullable = column.Nullable,
					Position = num++,
					Description = column.Description
				};
				list.Add(item);
			}
		}
		foreach (ColumnRow item3 in list)
		{
			AddColumn(item3);
		}
		return true;
	}

	public override bool GetTriggers(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		DBTPackage dBTPackage = CommandsWithTimeout.DBT(synchronizeParameters.DatabaseRow.Connection);
		if (dBTPackage == null)
		{
			return false;
		}
		List<UniqueColumnDbt> uniqueColumns = dBTPackage.GetUniqueColumns();
		uniqueColumns.ForEach(delegate(UniqueColumnDbt uniqueColumn)
		{
			uniqueColumn.DatabaseName = synchronizeParameters.DatabaseName;
		});
		foreach (UniqueColumnDbt item in uniqueColumns)
		{
			AddUniqueConstraint(item, SharedDatabaseTypeEnum.DatabaseType.DBT);
		}
		return true;
	}

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
		throw new NotImplementedException();
	}

	public override bool GetDependencies(string query, Form owner = null)
	{
		throw new NotImplementedException();
	}

	public override List<DataProcessRow> GetDataLineage()
	{
		DBTPackage dbt = CommandsWithTimeout.DBT(synchronizeParameters.DatabaseRow.Connection);
		if (dbt == null)
		{
			return new List<DataProcessRow>();
		}
		List<TableWithSchemaByDatabaseObject> source;
		if (!int.TryParse(synchronizeParameters.DatabaseRow.Param1, out var result))
		{
			result = -1;
			source = new List<TableWithSchemaByDatabaseObject>();
		}
		else
		{
			source = DB.Table.GetTablesAndViewsWithSchemaByDatabase(result, null);
		}
		List<Node> modelNodes = dbt.GetModelNodes();
		List<DataProcessRow> list = new List<DataProcessRow>();
		foreach (Node node in modelNodes)
		{
			List<string> nodeParentsIds = dbt.GetNodeParentsIds(node);
			List<Node> list2 = (from x in nodeParentsIds
				select dbt.GetModelNode(x) into x
				where x != null
				select x).ToList();
			List<Source> list3 = ((result > 0) ? (from x in nodeParentsIds
				select dbt.GetSourceNode(x) into x
				where x != null
				select x).ToList() : new List<Source>());
			DataProcessRow dataProcessRow = null;
			if (list2.Count > 0 || list3.Count > 0)
			{
				dataProcessRow = CreateDefaultProcess(node);
				if (list2.Count > 0)
				{
					foreach (Node item in list2)
					{
						dataProcessRow.InflowRows.Add(new DataFlowRow
						{
							RowState = ManagingRowsEnum.ManagingRows.ForAdding,
							Direction = FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.IN),
							ObjectType = GetNodeObjectType(item),
							Name = item.Name,
							Schema = item.Schema,
							Source = UserTypeEnum.UserType.DBMS,
							Process = dataProcessRow
						});
					}
				}
				if (list3.Count > 0)
				{
					foreach (Source parent in list3)
					{
						TableWithSchemaByDatabaseObject tableWithSchemaByDatabaseObject = source.Where((TableWithSchemaByDatabaseObject x) => string.Equals(x.Name, parent.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Schema, parent.Schema, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
						if (tableWithSchemaByDatabaseObject != null)
						{
							SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(tableWithSchemaByDatabaseObject.ObjectType);
							dataProcessRow.InflowRows.Add(new DataFlowRow
							{
								RowState = ManagingRowsEnum.ManagingRows.ForAdding,
								Direction = FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.IN),
								ObjectType = objectType.Value,
								Name = parent.Name,
								Schema = parent.Schema,
								Source = UserTypeEnum.UserType.DBMS,
								Process = dataProcessRow,
								ObjectId = tableWithSchemaByDatabaseObject.Id,
								DatabaseId = result
							});
						}
					}
				}
			}
			if (!string.Equals(node.Materialized, "ephemeral", StringComparison.OrdinalIgnoreCase))
			{
				TableWithSchemaByDatabaseObject tableWithSchemaByDatabaseObject2 = source.Where((TableWithSchemaByDatabaseObject x) => string.Equals(x.Name, node.Name, StringComparison.OrdinalIgnoreCase) && string.Equals(x.Schema, node.Schema, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
				if (tableWithSchemaByDatabaseObject2 != null)
				{
					if (dataProcessRow == null)
					{
						dataProcessRow = CreateDefaultProcess(node);
					}
					SharedObjectTypeEnum.ObjectType? objectType2 = SharedObjectTypeEnum.StringToType(tableWithSchemaByDatabaseObject2.ObjectType);
					dataProcessRow.OutflowRows.Add(new DataFlowRow
					{
						RowState = ManagingRowsEnum.ManagingRows.ForAdding,
						Direction = FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.OUT),
						ObjectType = objectType2.Value,
						Name = node.Name,
						Schema = node.Schema,
						Source = UserTypeEnum.UserType.DBMS,
						Process = dataProcessRow,
						ObjectId = tableWithSchemaByDatabaseObject2.Id,
						DatabaseId = result
					});
				}
			}
			if (dataProcessRow != null && (dataProcessRow.InflowRows.Any() || dataProcessRow.OutflowRows.Any()))
			{
				dataProcessRow.OutflowRows.Add(new DataFlowRow
				{
					RowState = ManagingRowsEnum.ManagingRows.ForAdding,
					Direction = FlowDirectionEnum.TypeToString(FlowDirectionEnum.Direction.OUT),
					ObjectType = GetNodeObjectType(node),
					Name = node.Name,
					Schema = node.Schema,
					Source = UserTypeEnum.UserType.DBMS,
					Process = dataProcessRow
				});
				list.Add(dataProcessRow);
			}
		}
		return list;
	}

	private static DataProcessRow CreateDefaultProcess(Node node)
	{
		return new DataProcessRow
		{
			Name = "dbt lineage",
			ParentName = node.Name,
			ParentSchema = node.Schema,
			ParentObjectType = GetNodeObjectType(node),
			Source = UserTypeEnum.UserType.DBMS
		};
	}

	private static SharedObjectTypeEnum.ObjectType GetNodeObjectType(Node node)
	{
		return SharedObjectTypeEnum.StringToType(ObjectSynchronizationForDBTObject.GetViewType(node)).Value;
	}

	public void ReadCount(FilterObjectTypeEnum.FilterObjectType type, int count)
	{
		switch (type)
		{
		case FilterObjectTypeEnum.FilterObjectType.Table:
			base.ObjectsCounter.TablesCount += count;
			break;
		case FilterObjectTypeEnum.FilterObjectType.View:
			base.ObjectsCounter.ViewsCount += count;
			break;
		case FilterObjectTypeEnum.FilterObjectType.Procedure:
			base.ObjectsCounter.ProceduresCount += count;
			break;
		case FilterObjectTypeEnum.FilterObjectType.Function:
			base.ObjectsCounter.FunctionsCount += count;
			break;
		}
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		return new List<string>();
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		return new List<string> { string.Empty };
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		return new List<string>();
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		return new List<string>();
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		return new List<string>();
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		return new List<string> { string.Empty };
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		return new List<string>();
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		return new List<string>();
	}

	public override IEnumerable<string> RelationsQuery()
	{
		return new List<string> { string.Empty };
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		return new List<string> { string.Empty };
	}

	public override IEnumerable<string> TriggersQuery()
	{
		return new List<string>();
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		return new List<string> { string.Empty };
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		return new List<string>();
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		return new List<string>();
	}
}
