using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Data.Tools;
using Dataedo.Shared.Enums;
using Newtonsoft.Json.Linq;

namespace Dataedo.App.Data.Tableau;

internal class SynchronizeTableau : SynchronizeDatabase
{
	public SynchronizeTableau(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		if (query.ObjectType != FilterObjectTypeEnum.FilterObjectType.Table)
		{
			return true;
		}
		try
		{
			JObject jObject = RequestHelper.SendGraphQLRequest(CommandsWithTimeout.Tableau(synchronizeParameters.DatabaseRow.Connection), "\r\n                query count_tables\r\n                {\r\n                    tablesConnection\r\n                    {\r\n                        totalCount\r\n                    }\r\n                }\r\n                ");
			ReadCount(SharedObjectTypeEnum.ObjectType.Table, jObject["tablesConnection"]!["totalCount"].Value<int>());
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, synchronizeParameters.Owner);
			return false;
		}
		return true;
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (base.ObjectsCounter == null)
		{
			return true;
		}
		try
		{
			backgroundWorkerManager.ReportProgress("Retrieving database's objects");
			if (query.ObjectType != FilterObjectTypeEnum.FilterObjectType.Table)
			{
				return true;
			}
			DocumentationCustomFieldRow documentationCustomFieldRow = synchronizeParameters.CustomFields.FirstOrDefault((DocumentationCustomFieldRow x) => "contact".Equals(x.ExtendedProperty));
			string text = ((documentationCustomFieldRow == null) ? "contact\r\n                        {\r\n                            name\r\n                        }" : (documentationCustomFieldRow.FieldName + ": contact\r\n                        {\r\n                            name\r\n                        }"));
			DocumentationCustomFieldRow documentationCustomFieldRow2 = synchronizeParameters.CustomFields.FirstOrDefault((DocumentationCustomFieldRow x) => "tags".Equals(x.ExtendedProperty));
			string text2 = ((documentationCustomFieldRow2 == null) ? "tags\r\n                        {\r\n                            name\r\n                        }" : (documentationCustomFieldRow2.FieldName + ": tags\r\n                        {\r\n                            name\r\n                        }"));
			DocumentationCustomFieldRow documentationCustomFieldRow3 = synchronizeParameters.CustomFields.FirstOrDefault((DocumentationCustomFieldRow x) => "dataQualityWarnings".Equals(x.ExtendedProperty));
			string text3 = ((documentationCustomFieldRow3 == null) ? "dataQualityWarnings\r\n                        {\r\n                            warningType\r\n                            message\r\n                        }" : (documentationCustomFieldRow3.FieldName + ": dataQualityWarnings\r\n                        {\r\n                            warningType\r\n                            message\r\n                        }"));
			DocumentationCustomFieldRow documentationCustomFieldRow4 = synchronizeParameters.CustomFields.FirstOrDefault((DocumentationCustomFieldRow x) => "connectionType".Equals(x.ExtendedProperty));
			string text4 = ((documentationCustomFieldRow4 == null) ? "connectionType" : (documentationCustomFieldRow4.FieldName + ": connectionType"));
			TableauConnection conn = CommandsWithTimeout.Tableau(synchronizeParameters.DatabaseRow.Connection);
			string query2 = "\r\n                query tables {\r\n                  databaseTables {\r\n                    name\r\n                    id\r\n                    description\r\n                    schema\r\n                    connectionType\r\n                    database {\r\n                      name\r\n                    }\r\n\t                " + text + "\r\n\t                " + text2 + "\r\n\t                " + text3 + "\r\n                    " + text4 + "\r\n                  }\r\n                }\r\n                ";
			JObject jObject = RequestHelper.SendGraphQLRequest(conn, query2);
			List<JToken> list = new List<JToken>();
			foreach (JToken item in (IEnumerable<JToken>)(jObject["databaseTables"]!))
			{
				if (synchronizeParameters.DbSynchLocker.IsCanceled)
				{
					return false;
				}
				item["databaseName"] = item["database"]!["name"];
				string text5 = item["schema"].Value<string>();
				string text6 = item["name"].Value<string>();
				item["name"] = (string.IsNullOrEmpty(text5) ? text6 : (text5 + "." + text6));
				if (documentationCustomFieldRow4 != null)
				{
					item[documentationCustomFieldRow4.FieldName] = TableauDataExtractor.FormatConnectionTypes(item[documentationCustomFieldRow4.FieldName].Value<string>());
				}
				list.Add(item);
			}
			list = (from x in list
				group x by new
				{
					Id = x["id"]
				} into x
				select x.FirstOrDefault()).ToList();
			List<JToken> duplicatedTables = (from x in list
				group x by new
				{
					Schema = x["databaseName"],
					Name = x["name"]
				} into x
				where x.Count() > 1
				select x).SelectMany(x => x).ToList();
			IEnumerable<JToken> enumerable = list.Where((JToken x) => !duplicatedTables.Any((JToken y) => y["id"]!.Equals(x["id"])));
			foreach (JToken item2 in duplicatedTables)
			{
				item2["name"] = RequestHelper.GetNameWithHash(item2.Value<string>("id"), item2.Value<string>("name"));
				ObservableCollection<ObjectRow> tableRows = synchronizeParameters.DatabaseRow.tableRows;
				Form owner2 = owner;
				AddDBObject(item2, tableRows, backgroundWorkerManager, null, null, owner2);
			}
			foreach (JToken item3 in enumerable)
			{
				ObservableCollection<ObjectRow> tableRows2 = synchronizeParameters.DatabaseRow.tableRows;
				Form owner2 = owner;
				AddDBObject(item3, tableRows2, backgroundWorkerManager, null, null, owner2);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetRelations(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		try
		{
			TableauConnection conn = CommandsWithTimeout.Tableau(synchronizeParameters.DatabaseRow.Connection);
			DocumentationCustomFieldRow documentationCustomFieldRow = synchronizeParameters.CustomFields.FirstOrDefault((DocumentationCustomFieldRow x) => "tags".Equals(x.ExtendedProperty));
			string text = ((documentationCustomFieldRow == null) ? "tags\r\n                        {\r\n                            name\r\n                        }" : (documentationCustomFieldRow.FieldName + ": tags\r\n                        {\r\n                            name\r\n                        }"));
			JObject jObject = RequestHelper.SendGraphQLRequest(conn, "\r\n                query columns {\r\n                  databaseTables {\r\n                    name\r\n                    schema\r\n                    id\r\n                    database {\r\n                      name\r\n                    }\r\n                    columns{\r\n                      name\r\n                      description\r\n                      nullable: isNullable\r\n                      datatype: remoteType\r\n                      " + text + "\r\n                    }\r\n                  }\r\n                }\r\n                ");
			List<JToken> list = new List<JToken>();
			foreach (JToken item in (IEnumerable<JToken>)(jObject["databaseTables"]!))
			{
				if (synchronizeParameters.DbSynchLocker.IsCanceled)
				{
					return false;
				}
				string text2 = item["name"].Value<string>();
				string text3 = item["schema"].Value<string>();
				string text4 = (string.IsNullOrEmpty(text3) ? text2 : (text3 + "." + text2));
				JToken jToken = item["database"];
				foreach (JToken item2 in (IEnumerable<JToken>)(item["columns"]!))
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					item2["tableName"] = text4;
					item2["tableId"] = item["id"];
					item2["databaseName"] = jToken["name"];
					list.Add(item2);
				}
			}
			var source = (from x in list
				select new
				{
					Id = x["tableId"].Value<string>(),
					Name = x["tableName"].Value<string>(),
					Database = x["databaseName"].Value<string>()
				} into x
				group x by new { x.Id, x.Name, x.Database } into x
				select x.First() into x
				group x by new { x.Database, x.Name } into x
				where x.Count() > 1
				select x).SelectMany(x => x).ToArray();
			foreach (JToken column in list)
			{
				if (source.Any(x => column["tableId"].Value<string>()!.Equals(x.Id)))
				{
					column["tableName"] = RequestHelper.GetNameWithHash(column["tableId"].Value<string>(), column["tableName"].Value<string>());
				}
				AddColumn(column, withCustomFields: true);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetTriggers(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
		return true;
	}

	public override bool GetDependencies(string query, Form owner = null)
	{
		return true;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> RelationsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		yield return string.Empty;
	}
}
