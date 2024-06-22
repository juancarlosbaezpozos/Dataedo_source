using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Data.Salesforce;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.SalesforceConnector.SalesforceObjects;
using Dataedo.Shared.Enums;
using Salesforce.Common.Models.Json;
using Salesforce.Force;

namespace Dataedo.App.Data.Salesforce;

public class SynchronizeSalesforce : SynchronizeDatabase
{
	private static List<SObject> sObjects;

	private static List<SObjectDescribe> sObjectsDescribe;

	public SynchronizeSalesforce(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		try
		{
			backgroundWorkerManager.ReportProgress("Counting objects to import");
			ForceClient forceClient = CommandsWithTimeout.Salesforce(synchronizeParameters.DatabaseRow.Connection);
			if (forceClient == null)
			{
				return false;
			}
			DescribeGlobalResult<SObject> result = forceClient.GetObjectsAsync<SObject>().Result;
			if (result == null)
			{
				return false;
			}
			int num = 0;
			IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
			foreach (SObject sObject in result.SObjects)
			{
				if (!IsObjectFiltered(sObject.Name, rulesByObjectType))
				{
					num++;
				}
			}
			ReadCount(query.ObjectType, num);
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
				IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
				backgroundWorkerManager.ReportProgress("Retrieving database's objects");
				sObjectsDescribe = new List<SObjectDescribe>();
				sObjects = new List<SObject>();
				ForceClient forceClient = CommandsWithTimeout.Salesforce(synchronizeParameters.DatabaseRow.Connection);
				if (forceClient == null)
				{
					return false;
				}
				DescribeGlobalResult<SObject> result = forceClient.GetObjectsAsync<SObject>().Result;
				sObjects.AddRange(result?.SObjects);
				foreach (SObject sObject in sObjects)
				{
					if (!IsObjectFiltered(sObject.Name, rulesByObjectType))
					{
						AddDBObject(sObject, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
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
		try
		{
			ForceClient forceClient = CommandsWithTimeout.Salesforce(synchronizeParameters.DatabaseRow.Connection);
			if (forceClient == null)
			{
				return;
			}
			if (backgroundWorkerManager != null)
			{
				backgroundWorkerManager.SetMaxProgress(backgroundWorkerManager.GetMaxProgress() + 1);
				backgroundWorkerManager.ReportProgress("Retrieving objects details. This may take a while...");
			}
			IEnumerable<string> tablesToSynchronize = from t in synchronizeParameters.DatabaseRow.tableRows
				where t.ToSynchronize
				select t.Name;
			foreach (SObject item in sObjects.Where((SObject o) => tablesToSynchronize.Contains(o.Name)))
			{
				SObjectDescribe result = forceClient.DescribeAsync<SObjectDescribe>(item.Name).Result;
				sObjectsDescribe.Add(result);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
		}
	}

	public override bool GetRelations(string query, Form owner = null)
	{
		try
		{
			if (sObjectsDescribe == null)
			{
				return false;
			}
			IEnumerable<string> tablesToSynchronize = from t in synchronizeParameters.DatabaseRow.tableRows
				where t.ToSynchronize
				select t.Name;
			foreach (SObjectDescribe item in sObjectsDescribe.Where((SObjectDescribe o) => tablesToSynchronize.Contains(o.Name)))
			{
				SObjectChildRelationship[] childRelationships = item.ChildRelationships;
				foreach (SObjectChildRelationship sObjectChildRelationship in childRelationships)
				{
					SObjectField sObjectField = item.Fields.FirstOrDefault((SObjectField f) => f.Type == "id" && f.Length == 18);
					if (sObjectField != null)
					{
						AddRelation(new SalesforceRelation(synchronizeParameters.DatabaseRow.Name, sObjectChildRelationship.ChildSObject, string.Empty, sObjectChildRelationship.Field, synchronizeParameters.DatabaseRow.Name, item.Name, string.Empty, sObjectField?.Name, sObjectChildRelationship.RelationshipName ?? (sObjectChildRelationship.ChildSObject + "s")), SharedDatabaseTypeEnum.DatabaseType.Salesforce, withCustomFields: true);
					}
				}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		try
		{
			IEnumerable<string> tablesToSynchronize = from t in synchronizeParameters.DatabaseRow.tableRows
				where t.ToSynchronize
				select t.Name;
			if (sObjectsDescribe == null)
			{
				return false;
			}
			foreach (SObjectDescribe item in from o in sObjectsDescribe
				where tablesToSynchronize.Contains(o.Name)
				orderby o.Label
				select o)
			{
				SObjectField[] fields = item.Fields;
				foreach (SObjectField field in fields)
				{
					AddColumn(new SObjectFieldWrapper(field, item), withCustomFields: true);
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

	public override bool GetTriggers(string query, Form owner = null)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(query))
			{
				return false;
			}
			ForceClient forceClient = CommandsWithTimeout.Salesforce(synchronizeParameters.DatabaseRow.Connection);
			if (forceClient == null)
			{
				return false;
			}
			IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
			try
			{
				QueryResult<STriggerObject> result = forceClient.QueryAsync<STriggerObject>(query).Result;
				if (result != null)
				{
					foreach (STriggerObject record in result.Records)
					{
						if (!IsObjectFiltered(record.TableEnumOrId, rulesByObjectType))
						{
							AddTrigger(record, withCustomFields: true);
						}
					}
				}
			}
			catch
			{
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		try
		{
			IEnumerable<string> tablesToSynchronize = from t in synchronizeParameters.DatabaseRow.tableRows
				where t.ToSynchronize
				select t.Name;
			if (sObjectsDescribe == null)
			{
				return false;
			}
			foreach (SObjectDescribe item in from o in sObjectsDescribe
				where tablesToSynchronize.Contains(o.Name)
				orderby o.Label
				select o)
			{
				foreach (SObjectField item2 in item.Fields.Where((SObjectField f) => f.Type == "id" && f.Length == 18))
				{
					AddUniqueConstraint(new SalesforceConstraint(item2.Name, item.Name, string.Empty, synchronizeParameters.DatabaseRow?.Name, item2.Name, "P"), SharedDatabaseTypeEnum.DatabaseType.Salesforce, withCustomFields: true);
				}
				foreach (SObjectField item3 in item.Fields.Where((SObjectField f) => f.Unique))
				{
					AddUniqueConstraint(new SalesforceConstraint(item3.Name, item.Name, string.Empty, synchronizeParameters.DatabaseRow?.Name, item3.Name, "U"), SharedDatabaseTypeEnum.DatabaseType.Salesforce, withCustomFields: true);
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

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
		throw new NotImplementedException();
	}

	public override bool GetDependencies(string query, Form owner = null)
	{
		throw new NotImplementedException();
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
		return new List<string> { "" };
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		return new List<string>();
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
		return new List<string> { "" };
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		return new List<string>();
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
		return new List<string> { "" };
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return "SELECT Id, \r\n                Name,\r\n                TableEnumOrId,\r\n                Body, \r\n                ApiVersion,\r\n                Status,\r\n                UsageAfterDelete,\r\n                UsageAfterInsert,\r\n                UsageAfterUndelete,\r\n                UsageAfterUpdate,\r\n                UsageBeforeDelete,\r\n                UsageBeforeInsert,\r\n                UsageBeforeUpdate,\r\n                UsageIsBulk \r\n                FROM \r\n                ApexTrigger";
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		return new List<string> { "" };
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
