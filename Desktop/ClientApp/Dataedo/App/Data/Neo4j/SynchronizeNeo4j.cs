using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Data.Neo4j;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using Neo4j.Driver;

namespace Dataedo.App.Data.Neo4j;

internal class SynchronizeNeo4j : SynchronizeDatabase
{
	private readonly string relationNode1LabelKey = "node1Label";

	private readonly string relationEdgeLabelKey = "edgeLabel";

	private readonly string relationNode2Label = "node2Label";

	private Neo4jFilter neo4jFilter;

	private string unauthorizedColectionMessage = "Dataedo couldn't access this collection's data. To see more details, reimport this collection with a user that has access to it granted.";

	private string schema = string.Empty;

	public override IEnumerable<string> CountTablesQuery()
	{
		string filterStringForObjectType = neo4jFilter.GetFilterStringForObjectType("nodeLabel", FilterObjectTypeEnum.FilterObjectType.Table, "AND");
		string filterStringForObjectType2 = neo4jFilter.GetFilterStringForObjectType("relLabel", FilterObjectTypeEnum.FilterObjectType.Table, "AND");
		yield return $"CALL db.schema.nodeTypeProperties()\r\n                            YIELD nodeType\r\n                            WITH split(replace(nodeType, '`', ''), ':') as nodeLabels\r\n                            UNWIND nodeLabels AS nodeLabel\r\n                            WITH nodeLabel\r\n                            WHERE nodeLabel <> \"\"\r\n                            {filterStringForObjectType}\r\n\r\n                            CALL db.schema.relTypeProperties()\r\n                            YIELD relType\r\n                            WITH split(replace(relType, '`', ''), ':') as relLabels, nodeLabel\r\n                            UNWIND relLabels as relLabel\r\n                            WITH relLabel, nodeLabel\r\n                            WHERE relLabel <> \"\"\r\n                            {filterStringForObjectType2}\r\n\r\n                            RETURN '{SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Table)}' as {Neo4jEnums.RecordColumnsKeys.type},\r\n                                    COUNT(DISTINCT nodeLabel) + COUNT(DISTINCT relLabel) as {Neo4jEnums.RecordColumnsKeys.count}\r\n                            UNION\r\n                            RETURN '{SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Table)}' as {Neo4jEnums.RecordColumnsKeys.type},\r\n                                    0 as {Neo4jEnums.RecordColumnsKeys.count}";
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		string filterStringForObjectType = neo4jFilter.GetFilterStringForObjectType("name", FilterObjectTypeEnum.FilterObjectType.Procedure, "AND");
		yield return $"CALL dbms.procedures()\r\n                                YIELD name\r\n                                WHERE name <> \"\"\r\n                                {filterStringForObjectType}\r\n                                RETURN '{SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Procedure)}' as {Neo4jEnums.RecordColumnsKeys.type},\r\n                                        count(DISTINCT name) as {Neo4jEnums.RecordColumnsKeys.count}\r\n                                UNION\r\n                                RETURN '{SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Procedure)}' as {Neo4jEnums.RecordColumnsKeys.type},\r\n                                0 as {Neo4jEnums.RecordColumnsKeys.count}";
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		string filterStringForObjectType = neo4jFilter.GetFilterStringForObjectType("name", FilterObjectTypeEnum.FilterObjectType.Function, "AND");
		yield return $"CALL dbms.functions()\r\n                            YIELD name\r\n                            WHERE name <> \"\"\r\n                            {filterStringForObjectType}\r\n                            RETURN '{SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Function)}' as {Neo4jEnums.RecordColumnsKeys.type}, \r\n                                    count(DISTINCT name) as {Neo4jEnums.RecordColumnsKeys.count}\r\n                            UNION\r\n                            RETURN '{SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Function)}' as {Neo4jEnums.RecordColumnsKeys.type}, \r\n                                    0 as {Neo4jEnums.RecordColumnsKeys.count}";
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		string filterStringForObjectType = neo4jFilter.GetFilterStringForObjectType("nodeLabel", FilterObjectTypeEnum.FilterObjectType.Table, "AND");
		string relFilterString = neo4jFilter.GetFilterStringForObjectType("relLabel", FilterObjectTypeEnum.FilterObjectType.Table, "AND");
		yield return $"CALL db.schema.nodeTypeProperties()\r\n                            YIELD nodeType\r\n                            WITH split(replace(nodeType, '`', ''), ':') as nodeLabels\r\n                            UNWIND nodeLabels AS nodeLabel\r\n                            WITH nodeLabel\r\n                            WHERE nodeLabel <> \"\"\r\n                            {filterStringForObjectType}\r\n                            RETURN DISTINCT '{Neo4jEnums.TypeToString(Neo4jEnums.TableTypeEnum.nodeTable)}' \r\n                                                            as {Neo4jEnums.RecordColumnsKeys.type}, \r\n                                            nodeLabel as {Neo4jEnums.RecordColumnsKeys.name}";
		yield return $"CALL db.schema.relTypeProperties()\r\n                            YIELD relType\r\n                            WITH split(replace(relType, '`', ''), ':') as relLabels\r\n                            UNWIND relLabels AS relLabel\r\n                            WITH relLabel\r\n                            WHERE relLabel <> \"\"\r\n                            {relFilterString}\r\n                            RETURN DISTINCT '{Neo4jEnums.TypeToString(Neo4jEnums.TableTypeEnum.edgeTable)}' \r\n                                                            as {Neo4jEnums.RecordColumnsKeys.type}, \r\n                                             relLabel as {Neo4jEnums.RecordColumnsKeys.name}";
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		string filterStringForObjectType = neo4jFilter.GetFilterStringForObjectType("name", FilterObjectTypeEnum.FilterObjectType.Procedure, "AND");
		yield return $"CALL dbms.procedures()\r\n                            YIELD name, description \r\n                            WHERE name <> \"\"\r\n                            {filterStringForObjectType}\r\n                            RETURN DISTINCT '{Neo4jEnums.TypeToString(Neo4jEnums.TableTypeEnum.procedure)}' \r\n                                                        as {Neo4jEnums.RecordColumnsKeys.type}, \r\n                                             name as {Neo4jEnums.RecordColumnsKeys.name}, \r\n                                             description as {Neo4jEnums.RecordColumnsKeys.description}";
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		string filterStringForObjectType = neo4jFilter.GetFilterStringForObjectType("name", FilterObjectTypeEnum.FilterObjectType.Function, "AND");
		yield return $"CALL dbms.functions()\r\n                            YIELD name, description\r\n                            WHERE name <> \"\"\r\n                            {filterStringForObjectType}\r\n                            RETURN DISTINCT '{Neo4jEnums.TypeToString(Neo4jEnums.TableTypeEnum.function)}' \r\n                                                            as {Neo4jEnums.RecordColumnsKeys.type}, \r\n                                             name as {Neo4jEnums.RecordColumnsKeys.name}, \r\n                                             description as {Neo4jEnums.RecordColumnsKeys.description}";
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		string filterStringForObjectType = neo4jFilter.GetFilterStringForObjectType("nodeLabel", FilterObjectTypeEnum.FilterObjectType.Table, "AND");
		string relFilterString = neo4jFilter.GetFilterStringForObjectType("relLabel", FilterObjectTypeEnum.FilterObjectType.Table, "AND");
		yield return $"CALL db.schema.nodeTypeProperties()\r\n                            YIELD nodeType, propertyName, propertyTypes, mandatory\r\n                            WITH split(replace(nodeType, '`', ''), ':') as nodeLabels, propertyName, propertyTypes, mandatory\r\n                            UNWIND nodeLabels AS nodeLabel\r\n                            WITH nodeLabel, propertyName, propertyTypes, mandatory\r\n                            WHERE nodeLabel <> \"\"\r\n                            {filterStringForObjectType}\r\n                            RETURN DISTINCT nodeLabel as {Neo4jEnums.RecordColumnsKeys.name}, \r\n                                            propertyName as {Neo4jEnums.RecordColumnsKeys.propertyName},\r\n                                            propertyTypes as {Neo4jEnums.RecordColumnsKeys.propertyTypes},\r\n                                            mandatory as {Neo4jEnums.RecordColumnsKeys.propertyMandatory}\r\n                            ORDER BY {Neo4jEnums.RecordColumnsKeys.name}";
		yield return $"CALL db.schema.relTypeProperties()\r\n                            YIELD relType, propertyName, propertyTypes, mandatory\r\n                            WITH split(replace(relType, '`', ''), ':') as relLabels, propertyName, propertyTypes, mandatory\r\n                            UNWIND relLabels AS relLabel\r\n                            WITH relLabel, propertyName, propertyTypes, mandatory\r\n                            WHERE relLabel <> \"\"\r\n                            {relFilterString}\r\n                            RETURN DISTINCT relLabel as {Neo4jEnums.RecordColumnsKeys.name}, \r\n                                            propertyName as {Neo4jEnums.RecordColumnsKeys.propertyName},\r\n                                            propertyTypes as {Neo4jEnums.RecordColumnsKeys.propertyTypes},\r\n                                            mandatory as {Neo4jEnums.RecordColumnsKeys.propertyMandatory}\r\n                            ORDER BY {Neo4jEnums.RecordColumnsKeys.name}";
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		string filterStringForObjectType = neo4jFilter.GetFilterStringForObjectType("name", FilterObjectTypeEnum.FilterObjectType.Procedure, "AND");
		string functionsFilter = neo4jFilter.GetFilterStringForObjectType("name", FilterObjectTypeEnum.FilterObjectType.Function, "AND");
		yield return $"CALL dbms.procedures() \r\n                            YIELD name, signature \r\n                            WHERE name <> \"\"\r\n                            {filterStringForObjectType}\r\n                            RETURN DISTINCT name as {Neo4jEnums.RecordColumnsKeys.name}, \r\n                                            signature as {Neo4jEnums.RecordColumnsKeys.objectSignature}";
		yield return $"CALL dbms.functions() \r\n                            YIELD name, signature \r\n                            WHERE name <> \"\" \r\n                            {functionsFilter}\r\n                            RETURN DISTINCT name as {Neo4jEnums.RecordColumnsKeys.name}, \r\n                                            signature as {Neo4jEnums.RecordColumnsKeys.objectSignature}";
	}

	public override IEnumerable<string> RelationsQuery()
	{
		string filterStringForObjectType = neo4jFilter.GetFilterStringForObjectType(relationNode1LabelKey, FilterObjectTypeEnum.FilterObjectType.Table, "AND");
		string filterStringForObjectType2 = neo4jFilter.GetFilterStringForObjectType(relationNode2Label, FilterObjectTypeEnum.FilterObjectType.Table, "AND");
		string filterStringForObjectType3 = neo4jFilter.GetFilterStringForObjectType(relationEdgeLabelKey, FilterObjectTypeEnum.FilterObjectType.Table, "AND");
		yield return "MATCH (n)-[r]->(m) \r\n                            WITH labels(n) as node1Labels, type(r) as " + relationEdgeLabelKey + ", labels(m) as node2Labels\r\n                            UNWIND node1Labels as " + relationNode1LabelKey + "\r\n                            UNWIND node2Labels as " + relationNode2Label + "\r\n                            WITH " + relationNode1LabelKey + ", \r\n                                 " + relationEdgeLabelKey + ", \r\n                                 " + relationNode2Label + "\r\n                            WHERE " + relationNode1LabelKey + " <> \"\" \r\n                                AND " + relationNode2Label + " <> \"\"\r\n                            " + filterStringForObjectType + "\r\n                            " + filterStringForObjectType2 + "\r\n                            " + filterStringForObjectType3 + "\r\n                            RETURN DISTINCT " + relationNode1LabelKey + ", \r\n                                            " + relationEdgeLabelKey + ", \r\n                                            " + relationNode2Label;
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		yield return $"CALL db.constraints()\r\n                            YIELD description\r\n                            return description as {Neo4jEnums.RecordColumnsKeys.description};";
	}

	public SynchronizeNeo4j(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
		neo4jFilter = new Neo4jFilter(base.synchronizeParameters);
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		try
		{
			IResult result = CommandsWithTimeout.Neo4j(synchronizeParameters.DatabaseRow.Connection, query.Query);
			string objectTypeString = null;
			long num = -1L;
			foreach (IRecord item in result)
			{
				if (synchronizeParameters.DbSynchLocker.IsCanceled)
				{
					return false;
				}
				objectTypeString = item[Neo4jEnums.RecordColumnsKeys.type.ToString()].ToString();
				num = Math.Max((long)item[Neo4jEnums.RecordColumnsKeys.count.ToString()], num);
			}
			if (num != -1)
			{
				SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(objectTypeString);
				if (!objectType.HasValue)
				{
					throw new InvalidOperationException("Returned ObjectType has wrong format");
				}
				ReadCount(objectType.Value, (int)num);
				return true;
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Retrieving database's objects");
		try
		{
			if (string.IsNullOrWhiteSpace(query.Query))
			{
				return false;
			}
			foreach (IRecord item in CommandsWithTimeout.Neo4j(synchronizeParameters.DatabaseRow.Connection, query.Query))
			{
				if (synchronizeParameters.DbSynchLocker.IsCanceled)
				{
					return false;
				}
				string type = item[Neo4jEnums.RecordColumnsKeys.type.ToString()].ToString();
				string name = item[Neo4jEnums.RecordColumnsKeys.name.ToString()].ToString().Trim();
				object value = null;
				if (item.Values.TryGetValue(Neo4jEnums.RecordColumnsKeys.description.ToString(), out value))
				{
					AddDBObject(new Neo4jObject(name, schema, type, value.ToString()), synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager);
				}
				else
				{
					AddDBObject(new Neo4jObject(name, schema, type), synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager);
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
			IResult result = CommandsWithTimeout.Neo4j(synchronizeParameters.DatabaseRow.Connection, query);
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			int value = 1;
			HashSet<string> hashSet = new HashSet<string>();
			string text = string.Empty;
			foreach (IRecord item in result)
			{
				string tableName = item[Neo4jEnums.RecordColumnsKeys.name.ToString()].ToString().Trim();
				if (string.IsNullOrWhiteSpace(tableName) || !synchronizeParameters.DatabaseRow.tableRows.Any((ObjectRow x) => x.Name == tableName) || item[Neo4jEnums.RecordColumnsKeys.propertyName.ToString()] == null)
				{
					continue;
				}
				if (text != tableName)
				{
					dictionary[text] = value;
					text = tableName;
					if (!dictionary.TryGetValue(tableName, out value))
					{
						value = 1;
					}
				}
				string text2 = item[Neo4jEnums.RecordColumnsKeys.propertyName.ToString()].ToString();
				bool flag = bool.Parse(item[Neo4jEnums.RecordColumnsKeys.propertyMandatory.ToString()].ToString());
				List<string> list = item[Neo4jEnums.RecordColumnsKeys.propertyTypes.ToString()].As<List<string>>();
				string text3 = null;
				text3 = ((list.Count <= 5) ? string.Join(",", list) : "Mixed");
				if (hashSet.Add($"{text2}:{tableName}:{text3}:{flag}"))
				{
					Neo4jColumn dr = new Neo4jColumn(text2, text3, flag, tableName, schema, value);
					AddColumn(dr);
					value++;
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

	public override bool GetDependencies(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
		try
		{
			foreach (IRecord item in CommandsWithTimeout.Neo4j(synchronizeParameters.DatabaseRow.Connection, query))
			{
				string text = item[Neo4jEnums.RecordColumnsKeys.name.ToString()].ToString();
				string text2 = item[Neo4jEnums.RecordColumnsKeys.objectSignature.ToString()].ToString();
				int length = text.Length;
				string[] array = text2.Substring(length).Split(new string[1] { ") :: " }, StringSplitOptions.None);
				string[] args = array[0].Trim('(', ')').Split(',');
				int position = 1;
				position = CreateParametersFromArray(text, args, "IN", position);
				string[] args2 = array[1].Trim('(', ')').Split(',');
				position = CreateParametersFromArray(text, args2, "OUT", position);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	private int CreateParametersFromArray(string funcName, string[] args, string mode, int position = 1)
	{
		for (int i = 0; i < args.Length; i++)
		{
			string[] array = args[i].Split(new string[1] { "::" }, StringSplitOptions.None);
			int num = array.Length;
			string parameterName = string.Empty;
			string text = string.Empty;
			switch (num)
			{
			case 1:
				text = array[0].Trim();
				break;
			case 2:
				parameterName = array[0].Trim();
				text = array[1].Trim();
				break;
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				Neo4jParameter dr = new Neo4jParameter(funcName, parameterName, schema, text, mode, position);
				AddParameter(dr, withCustomFields: true);
				position++;
			}
		}
		return position;
	}

	public override bool GetRelations(string query, Form owner = null)
	{
		_ = synchronizeParameters.DatabaseRow.ConnectionString;
		try
		{
			foreach (IRecord item in CommandsWithTimeout.Neo4j(synchronizeParameters.DatabaseRow.Connection, query))
			{
				string fktable = item[relationNode1LabelKey].ToString();
				string pktable = item[relationNode2Label].ToString();
				string text = item[relationEdgeLabelKey].ToString();
				AddRelation(new Neo4jRelation(fktable, text, schema, schema), SharedDatabaseTypeEnum.DatabaseType.Neo4j);
				AddRelation(new Neo4jRelation(text, pktable, schema, schema), SharedDatabaseTypeEnum.DatabaseType.Neo4j);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption);
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
		_ = synchronizeParameters.DatabaseRow.ConnectionString;
		try
		{
			IResult result = CommandsWithTimeout.Neo4j(synchronizeParameters.DatabaseRow.Connection, query);
			Regex regex = new Regex("CONSTRAINT ON(.*?)ASSERT");
			Regex regex2 = new Regex("ASSERT(.*?)IS");
			foreach (IRecord item in result)
			{
				string text = item[Neo4jEnums.RecordColumnsKeys.description.ToString()].ToString();
				string text2 = null;
				if (text.Contains("IS NODE KEY"))
				{
					text2 = "P";
				}
				else if (text.Contains("IS UNIQUE"))
				{
					text2 = "U";
				}
				if (text2 == null)
				{
					continue;
				}
				string text3 = ((text2 == "P") ? "NODE KEY" : "UNIQUE");
				string tableName = regex.Match(text).Groups[1].ToString();
				tableName = tableName.Split(':')[1];
				tableName = tableName.Split(']')[0];
				tableName = tableName.Split(')')[0];
				tableName = tableName.Trim();
				ObjectRow objectRow = synchronizeParameters.DatabaseRow.tableRows.Where((ObjectRow x) => x.Name == tableName).FirstOrDefault();
				if (objectRow != null)
				{
					string databaseName = objectRow.DatabaseName;
					List<string> list = new List<string>();
					StringBuilder stringBuilder = new StringBuilder(tableName + "(");
					string[] array = regex2.Match(text).Groups[1].ToString().Split(',');
					foreach (string text4 in array)
					{
						list.Add(text4.Split('.')[1].Trim('(', ')', ' '));
					}
					string text5 = string.Join(",", list);
					stringBuilder.Append(text5 + ") " + text3);
					AddUniqueConstraint(new Neo4jConstraint(stringBuilder.ToString(), tableName, schema, databaseName, text5, text2), SharedDatabaseTypeEnum.DatabaseType.Neo4j);
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
}
