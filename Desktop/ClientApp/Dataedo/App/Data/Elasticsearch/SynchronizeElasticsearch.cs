using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Data.Tables.Columns.Tools;
using Dataedo.Shared.Enums;
using Nest;

namespace Dataedo.App.Data.Elasticsearch;

internal class SynchronizeElasticsearch : SynchronizeDatabase
{
	public SynchronizeElasticsearch(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		try
		{
			IEnumerable<IndexName> keys = CommandsWithTimeout.Elasticsearch(synchronizeParameters.DatabaseRow.Connection).Indices.Get(new GetIndexRequest(Indices.All)).Indices.Keys;
			ReadCount(SharedObjectTypeEnum.ObjectType.Table, keys.Count());
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
		if (base.ObjectsCounter != null)
		{
			try
			{
				ElasticClient elasticClient = CommandsWithTimeout.Elasticsearch(synchronizeParameters.DatabaseRow.Connection);
				elasticClient.Indices.Get(new GetIndexRequest(Indices.All));
				foreach (KeyValuePair<IndexName, IndexMappings> index in elasticClient.Indices.GetMapping(new GetMappingRequest(Indices.All)).Indices)
				{
					if (!FilterUserObjectsFilter(index.Key.Name, FilterObjectTypeEnum.FilterObjectType.Table))
					{
						AddDBObject(index.Key.Name, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager);
					}
				}
			}
			catch (Exception exception)
			{
				GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption);
				return false;
			}
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
			ElasticClient elasticClient = CommandsWithTimeout.Elasticsearch(synchronizeParameters.DatabaseRow.Connection);
			elasticClient.Indices.Get(new GetIndexRequest(Indices.All));
			foreach (KeyValuePair<IndexName, IndexMappings> index in elasticClient.Indices.GetMapping(new GetMappingRequest(Indices.All)).Indices)
			{
				AddDefaultColumns(index.Key.Name);
				if (index.Value.Mappings.Properties != null)
				{
					AddColumnFromMapping(index.Value.Mappings.Properties, index.Key.Name);
				}
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

	private void AddColumnFromMapping(IProperties properties, string tableName, Form owner = null)
	{
		int num = 2;
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		List<ElasticsearchColumnData> list2 = new List<ElasticsearchColumnData>();
		foreach (KeyValuePair<PropertyName, IProperty> property in properties)
		{
			list2.AddRange(GetColumnsFromMapping(property.Value, tableName, num, 1, list));
			num++;
		}
		foreach (ElasticsearchColumnData item in list2)
		{
			if (list.Count() > 0)
			{
				foreach (KeyValuePair<string, string> item2 in list)
				{
					if (item.Name == item2.Key)
					{
						item.Computed = true;
						if (!string.IsNullOrEmpty(item.ComputedFormula))
						{
							item.ComputedFormula += " ";
						}
						item.ComputedFormula += item2.Value;
					}
				}
			}
			AddColumn(item);
		}
	}

	private List<ElasticsearchColumnData> GetColumnsFromMapping(IProperty typeMapping, string tableName, int index, int level, List<KeyValuePair<string, string>> copyToFields, string parentPath = null, Form owner = null)
	{
		List<ElasticsearchColumnData> list = new List<ElasticsearchColumnData>();
		string name = typeMapping.Name.Name;
		string columnDataType = GetColumnDataType(typeMapping);
		string columnType = GetColumnType(typeMapping);
		copyToFields.AddRange(GetCopyToFields(typeMapping, name));
		string path = GetPath(name, parentPath);
		list.Add(new ElasticsearchColumnData(name, tableName, columnDataType, columnType, index, parentPath, level));
		int level2 = level + 1;
		if (typeMapping is ObjectProperty objectProperty && objectProperty.Properties != null)
		{
			int num = 1;
			{
				foreach (KeyValuePair<PropertyName, IProperty> property in objectProperty.Properties)
				{
					list.AddRange(GetColumnsFromMapping(property.Value, tableName, num, level2, copyToFields, path));
					num++;
				}
				return list;
			}
		}
		return list;
	}

	private string GetColumnDataType(IProperty typeMapping)
	{
		return typeMapping.Type;
	}

	private string GetColumnType(IProperty typeMapping)
	{
		string type = typeMapping.Type;
		if (type == "object")
		{
			return "OBJECT";
		}
		if (type == "nested")
		{
			return "OBJECT";
		}
		return "FIELD";
	}

	private List<KeyValuePair<string, string>> GetCopyToFields(IProperty typeMapping, string tableName)
	{
		List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
		if (typeMapping is TextProperty textProperty && textProperty.CopyTo != null && textProperty.CopyTo.Count() > 0)
		{
			foreach (Field item in textProperty.CopyTo)
			{
				list.Add(new KeyValuePair<string, string>(item.Name, tableName));
			}
			return list;
		}
		return list;
	}

	private string GetPath(string name, string parentPath)
	{
		if (parentPath != null)
		{
			return parentPath + "." + name;
		}
		return name;
	}

	private bool FilterUserObjectsFilter(string name, FilterObjectTypeEnum.FilterObjectType objectType)
	{
		IEnumerable<FilterRule> source = synchronizeParameters.DatabaseRow.Filter.Rules.Where((FilterRule x) => (x.ObjectType == objectType || x.ObjectType == FilterObjectTypeEnum.FilterObjectType.Any) && x.RuleType == FilterRuleType.Include);
		if (synchronizeParameters.DatabaseRow.Filter.Rules.Where((FilterRule x) => (x.ObjectType == objectType || x.ObjectType == FilterObjectTypeEnum.FilterObjectType.Any) && x.RuleType == FilterRuleType.Exclude).Any((FilterRule x) => Like(name, x.PreparedName)))
		{
			return true;
		}
		if (source.Count() > 0)
		{
			if (!source.Any((FilterRule x) => Like(name, x.PreparedName)))
			{
				return true;
			}
			return false;
		}
		return false;
	}

	private bool Like(string toSearch, string toFind)
	{
		toSearch = toSearch.ToUpper();
		toFind = toFind.ToUpper();
		return new Regex("\\A" + new Regex("\\.|\\$|\\^|\\{|\\[|\\(|\\||\\)|\\*|\\+|\\?|\\\\").Replace(toFind, (Match ch) => "\\" + ch).Replace('_', '.').Replace("%", ".*") + "\\z", RegexOptions.Singleline).IsMatch(toSearch);
	}

	private void AddDefaultColumns(string index)
	{
		AddColumn(new ElasticsearchColumnData("_id", index, "ID", "ID", 1, null, 1));
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountFunctionsQuery()
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

	public override IEnumerable<string> GetProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> RelationsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
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

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		yield return string.Empty;
	}
}
