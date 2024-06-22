using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Data.DdlScript;
using Dataedo.Model.Data.Tables.Triggers;
using Dataedo.Shared.Enums;
using Dataedo.SqlParser;
using Dataedo.SqlParser.Domain;

namespace Dataedo.App.Data.Script;

internal class SynchronizeDdlScript : SynchronizeDatabase
{
	private List<ScriptData> _scriptData;

	private BackgroundWorkerManager _backgroundWorkerManager;

	private SqlInterpreter _interpreter;

	private SqlLanguage _sqlLanguage;

	private List<ColumnRow> _columns = new List<ColumnRow>();

	public SynchronizeDdlScript(SynchronizeParameters parameters)
		: base(parameters)
	{
	}

	public override void SetBackgroundWorkerManager(BackgroundWorkerManager backgroundWorkerManager)
	{
		_backgroundWorkerManager = backgroundWorkerManager;
		RunInterpreter();
	}

	private void RunInterpreter()
	{
		List<ScriptData> list = synchronizeParameters.DatabaseRow.Tag as List<ScriptData>;
		if (list == null || list.FirstOrDefault().HasBeenRead)
		{
			_scriptData = list;
			return;
		}
		_sqlLanguage = SqlLanguageEnum.StringToType(synchronizeParameters.DatabaseRow.Param2);
		string param = synchronizeParameters.DatabaseRow.Param3;
		synchronizeParameters.DatabaseRow.Name = "DDLScriptDatabase";
		_interpreter = new SqlInterpreter(list.FirstOrDefault()?.DdlScript, _sqlLanguage, param)
		{
			InterpretingFunction = LogFunction,
			InterpretingProcedure = LogProcedure,
			InterpretingTable = LogTable,
			InterpretingTrigger = LogTrigger,
			InterpretingView = LogView,
			InterpretingForeignKey = LogForeignKey,
			InterpretingPrimaryKey = LogPrimaryKey,
			InterpretingUniqueKey = LogUniqueKey
		};
		_backgroundWorkerManager?.SetMessage("Parsing DLL, it may take a while with bigger scripts...");
		_scriptData = _interpreter.GetData();
		if (_scriptData != null && _scriptData.Count != 0)
		{
			_scriptData.FirstOrDefault().HasBeenRead = true;
			synchronizeParameters.DatabaseRow.Tag = _scriptData;
		}
	}

	private void LogTable(Table table)
	{
		_backgroundWorkerManager?.SetMessage($"Interpreting table {table}");
	}

	private void LogView(Dataedo.SqlParser.Domain.View view)
	{
		_backgroundWorkerManager?.SetMessage($"Interpreting view {view}");
	}

	private void LogProcedure(Procedure procedure)
	{
		_backgroundWorkerManager?.SetMessage($"Interpreting procedure {procedure}");
	}

	private void LogFunction(Function function)
	{
		_backgroundWorkerManager?.SetMessage($"Interpreting function {function}");
	}

	private void LogPrimaryKey(PrimaryKey primaryKey)
	{
		_backgroundWorkerManager?.SetMessage($"Interpreting primary key {primaryKey}");
	}

	private void LogForeignKey(ForeignKey foreignKey)
	{
		_backgroundWorkerManager?.SetMessage($"Interpreting foreign key {foreignKey}");
	}

	private void LogUniqueKey(UniqueKey uniqueKey)
	{
		_backgroundWorkerManager?.SetMessage($"Interpreting unique key {uniqueKey}");
	}

	private void LogTrigger(Trigger trigger)
	{
		_backgroundWorkerManager?.SetMessage($"Interpreting trigger {trigger}");
	}

	public override bool PreCmdImportOperations(Form owner)
	{
		try
		{
			string text = File.ReadAllText(synchronizeParameters.DatabaseRow.Param1);
			synchronizeParameters.DatabaseRow.Tag = new List<ScriptData>
			{
				new ScriptData
				{
					DdlScript = text.Trim()
				}
			};
			RunInterpreter();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (_scriptData == null)
		{
			return true;
		}
		if (_scriptData.Count == 0)
		{
			return false;
		}
		int count = _scriptData.Where((ScriptData d) => d.Type == ScriptType.CREATE_TABLE || d.Type == ScriptType.ALTER_TABLE || d.Type == ScriptType.CREATE_OR_ALTER_TABLE).ToList().Count;
		int count2 = _scriptData.Where((ScriptData d) => d.Type == ScriptType.CREATE_VIEW || d.Type == ScriptType.ALTER_VIEW || d.Type == ScriptType.CREATE_OR_ALTER_VIEW).ToList().Count;
		int count3 = _scriptData.Where((ScriptData d) => d.Type == ScriptType.CREATE_PROCEDURE || d.Type == ScriptType.ALTER_PROCEDURE || d.Type == ScriptType.CREATE_OR_ALTER_PROCEDURE).ToList().Count;
		int count4 = _scriptData.Where((ScriptData d) => d.Type == ScriptType.CREATE_FUNCTION || d.Type == ScriptType.ALTER_FUNCTION || d.Type == ScriptType.CREATE_OR_ALTER_FUNCTION).ToList().Count;
		ReadCount(SharedObjectTypeEnum.ObjectType.Table, count);
		ReadCount(SharedObjectTypeEnum.ObjectType.View, count2);
		ReadCount(SharedObjectTypeEnum.ObjectType.Procedure, count3);
		ReadCount(SharedObjectTypeEnum.ObjectType.Function, count4);
		return true;
	}

	public override bool GetProcedureDefinitions(BackgroundWorkerManager backgroundWorkerManager)
	{
		return true;
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		if (!(synchronizeParameters.DatabaseRow.Tag is List<ScriptData> list))
		{
			return false;
		}
		foreach (ScriptData item in list)
		{
			foreach (Table table in item.Tables)
			{
				AddColumnsFromTable(table);
			}
			foreach (Dataedo.SqlParser.Domain.View view in item.Views)
			{
				AddColumnsFromView(view);
			}
		}
		foreach (ColumnRow column in _columns)
		{
			AddColumn(column);
		}
		return true;
	}

	public override bool GetTriggers(string query, Form owner = null)
	{
		if (_scriptData == null)
		{
			_scriptData = synchronizeParameters.DatabaseRow.Tag as List<ScriptData>;
		}
		if (_scriptData == null)
		{
			return true;
		}
		foreach (ScriptData item in _scriptData.Where((ScriptData s) => s.Type == ScriptType.CREATE_TRIGGER).ToList())
		{
			if (item.Trigger != null)
			{
				Trigger trigger = item.Trigger;
				TriggerSynchronizationObject dr = new TriggerSynchronizationObject
				{
					TriggerName = (trigger.Name ?? string.Empty),
					Definition = trigger.Definition,
					Disabled = trigger.IsDisabled,
					TableName = trigger.TableName,
					TableSchema = trigger.TableSchema,
					Isbefore = trigger.IsBefore,
					Isafter = trigger.IsAfter,
					Isinsteadof = trigger.IsInsteadOf,
					Isupdate = trigger.IsUpdate,
					Isdelete = trigger.IsDelete,
					Isinsert = trigger.IsInsert,
					Type = "Trigger"
				};
				AddTrigger(dr);
			}
		}
		return true;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		if (_scriptData == null)
		{
			_scriptData = synchronizeParameters.DatabaseRow.Tag as List<ScriptData>;
		}
		if (_scriptData == null)
		{
			return true;
		}
		List<ScriptData> list = _scriptData.Where((ScriptData d) => d.Type == ScriptType.CREATE_TABLE || d.Type == ScriptType.ALTER_TABLE || d.Type == ScriptType.CREATE_OR_ALTER_TABLE).ToList();
		List<Table> list2 = new List<Table>();
		foreach (ScriptData item in list)
		{
			list2.AddRange(item.Tables);
		}
		foreach (Table item2 in list2)
		{
			foreach (PrimaryKey primaryKey in item2.PrimaryKeys)
			{
				DdlScriptConstraint dr = new DdlScriptConstraint(primaryKey.Name, item2.Name, item2.Schema, synchronizeParameters.DatabaseName ?? string.Empty, primaryKey.Columns.FirstOrDefault() ?? string.Empty, "PRIMARY", isPk: true);
				AddUniqueConstraint(dr, SharedDatabaseTypeEnum.DatabaseType.DdlScript);
			}
			foreach (UniqueKey uniqueKey in item2.UniqueKeys)
			{
				foreach (string column in uniqueKey.Columns)
				{
					DdlScriptConstraint dr2 = new DdlScriptConstraint(uniqueKey.Name, item2.Name, item2.Schema, synchronizeParameters.DatabaseName ?? string.Empty, column ?? string.Empty, "UNIQUE", isPk: false);
					AddUniqueConstraint(dr2, SharedDatabaseTypeEnum.DatabaseType.DdlScript);
				}
			}
		}
		return true;
	}

	public override bool GetDependencies(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		switch (query.ObjectType)
		{
		case FilterObjectTypeEnum.FilterObjectType.Table:
			AddTables(backgroundWorkerManager, owner);
			break;
		case FilterObjectTypeEnum.FilterObjectType.View:
			AddViews(backgroundWorkerManager, owner);
			break;
		case FilterObjectTypeEnum.FilterObjectType.Procedure:
			AddProcedures(backgroundWorkerManager, owner);
			break;
		case FilterObjectTypeEnum.FilterObjectType.Function:
			AddFunctions(backgroundWorkerManager, owner);
			break;
		}
		synchronizeParameters.DatabaseRow.Tag = _scriptData;
		return true;
	}

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
		if (_scriptData == null)
		{
			_scriptData = synchronizeParameters.DatabaseRow.Tag as List<ScriptData>;
		}
		if (_scriptData == null)
		{
			return true;
		}
		List<ScriptData> list = _scriptData.Where((ScriptData d) => d.Type == ScriptType.CREATE_PROCEDURE || d.Type == ScriptType.ALTER_PROCEDURE || d.Type == ScriptType.CREATE_OR_ALTER_PROCEDURE).ToList();
		List<ScriptData> list2 = _scriptData.Where((ScriptData d) => d.Type == ScriptType.CREATE_FUNCTION || d.Type == ScriptType.ALTER_FUNCTION || d.Type == ScriptType.CREATE_OR_ALTER_FUNCTION).ToList();
		foreach (ScriptData item in list)
		{
			int num = 1;
			foreach (Parameter parameter in item.Procedure.Parameters)
			{
				ParameterRow paramObj2 = new ParameterRow
				{
					Name = parameter.Name,
					ProcedureName = item.Procedure.Name,
					DataType = parameter.DataType,
					Mode = (ParameterRow.ModeEnum)Enum.Parse(typeof(ParameterRow.ModeEnum), parameter.Mode, ignoreCase: true),
					ParameterMode = parameter.Mode,
					Position = num++,
					ProcedureSchema = item.Procedure.Schema,
					Source = UserTypeEnum.UserType.NotSet
				};
				if (base.ParameterRows.FirstOrDefault((ParameterRow p) => p.Name.Equals(paramObj2.Name) && p.ProcedureName.Equals(paramObj2) && p.ProcedureSchema.Equals(paramObj2) && p.DataType.Equals(paramObj2.DataType)) == null)
				{
					AddParameter(paramObj2);
				}
			}
		}
		foreach (ScriptData item2 in list2)
		{
			int num2 = 1;
			foreach (Parameter parameter2 in item2.Function.Parameters)
			{
				ParameterRow paramObj = new ParameterRow
				{
					Name = parameter2.Name,
					ProcedureName = item2.Function.Name,
					DataType = parameter2.DataType,
					Mode = (ParameterRow.ModeEnum)Enum.Parse(typeof(ParameterRow.ModeEnum), parameter2.Mode, ignoreCase: true),
					ParameterMode = parameter2.Mode,
					Position = num2++,
					ProcedureSchema = item2.Function.Schema,
					Source = UserTypeEnum.UserType.NotSet
				};
				if (base.ParameterRows.FirstOrDefault((ParameterRow p) => p.Name.Equals(paramObj.Name) && p.ProcedureName.Equals(paramObj) && p.ProcedureSchema.Equals(paramObj) && p.DataType.Equals(paramObj.DataType)) == null)
				{
					AddParameter(paramObj);
				}
			}
		}
		return true;
	}

	public override bool GetRelations(string query, Form owner = null)
	{
		if (_scriptData == null)
		{
			_scriptData = synchronizeParameters.DatabaseRow.Tag as List<ScriptData>;
		}
		if (_scriptData == null)
		{
			return true;
		}
		List<ScriptData> list = _scriptData.Where((ScriptData d) => d.Type == ScriptType.CREATE_TABLE || d.Type == ScriptType.ALTER_TABLE || d.Type == ScriptType.CREATE_OR_ALTER_TABLE || d.Type == ScriptType.CREATE_VIEW).ToList();
		List<Table> list2 = new List<Table>();
		List<Dataedo.SqlParser.Domain.View> list3 = new List<Dataedo.SqlParser.Domain.View>();
		foreach (ScriptData item in list)
		{
			list2.AddRange(item.Tables);
			list3.AddRange(item.Views);
		}
		foreach (Table item2 in list2)
		{
			foreach (ForeignKey foreignKey in item2.ForeignKeys)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				int num = 0;
				foreach (string column in foreignKey.Columns)
				{
					dictionary.Add(column, foreignKey.ForeignColumns[num]);
					num++;
				}
				foreach (KeyValuePair<string, string> item3 in dictionary)
				{
					DdlScriptRelation ddlScriptRelation = new DdlScriptRelation();
					ddlScriptRelation.Name = (string.IsNullOrEmpty(foreignKey?.Name) ? ("FK_" + item2.Name + "_" + item3.Key + "_" + item3.Value + "_" + foreignKey.ForeignTable) : foreignKey?.Name);
					ddlScriptRelation.FkColumn = item3.Key;
					ddlScriptRelation.RefColumn = item3.Value;
					ddlScriptRelation.FkTableName = item2.Name ?? string.Empty;
					ddlScriptRelation.FkTableSchema = item2.Schema ?? string.Empty;
					ddlScriptRelation.RefTableName = foreignKey.ForeignTable ?? string.Empty;
					ddlScriptRelation.RefTableSchema = foreignKey.ForeignTableSchema ?? string.Empty;
					DdlScriptRelation dr = ddlScriptRelation;
					AddRelation(dr, SharedDatabaseTypeEnum.DatabaseType.DdlScript);
				}
			}
		}
		return true;
	}

	private void AddTables(BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (_scriptData == null)
		{
			return;
		}
		foreach (ScriptData item in _scriptData.Where((ScriptData d) => d.Type == ScriptType.CREATE_TABLE || d.Type == ScriptType.ALTER_TABLE || d.Type == ScriptType.CREATE_OR_ALTER_TABLE).ToList())
		{
			foreach (Table table in item.Tables)
			{
				ObjectRow obj = new ObjectRow(table.Name, table.Schema ?? string.Empty, synchronizeParameters.DatabaseRow.Name, "table", "1970-01-01 00:00:00", "1970-01-01 00:00:00", null, null, null, null, synchronizeParameters.DatabaseRow.Id, null);
				obj.Subtype = table.Subtype;
				if (synchronizeParameters.DatabaseRow.tableRows.FirstOrDefault((ObjectRow t) => t.Name.Equals(obj.Name) && t.Type == SharedObjectTypeEnum.ObjectType.Table) == null)
				{
					AddDBObject(obj, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, owner);
				}
			}
		}
	}

	private void AddColumnsFromTable(Table table)
	{
		int num = 1;
		foreach (Column column in table.Columns)
		{
			ColumnRow item = new ColumnRow
			{
				Name = column.Name,
				TableName = table.Name,
				TableSchema = table.Schema,
				DatabaseName = synchronizeParameters.DatabaseRow.Name,
				DataType = column.DataType,
				DataLength = column.Length,
				Nullable = column.Nullable,
				IsIdentity = column.IsIdentity,
				Position = num++,
				DefaultValue = (column.DefaultValue ?? string.Empty),
				ComputedFormula = column.ComputedValue,
				IsComputed = !string.IsNullOrEmpty(column.ComputedValue)
			};
			_columns.Add(item);
		}
	}

	private void AddColumnsFromView(Dataedo.SqlParser.Domain.View view)
	{
		int num = 1;
		foreach (Column column in view.Columns)
		{
			ColumnRow item = new ColumnRow
			{
				Name = column.Name,
				TableName = view.Name,
				TableSchema = view.Schema,
				DatabaseName = synchronizeParameters.DatabaseRow.Name,
				DataType = column.DataType,
				DataLength = column.Length,
				Nullable = column.Nullable,
				Position = num++
			};
			_columns.Add(item);
		}
	}

	private void AddViews(BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (_scriptData == null)
		{
			return;
		}
		List<ScriptData> list = _scriptData?.Where((ScriptData d) => d.Type == ScriptType.CREATE_VIEW || d.Type == ScriptType.ALTER_VIEW || d.Type == ScriptType.CREATE_OR_ALTER_VIEW).ToList();
		List<Dataedo.SqlParser.Domain.View> list2 = new List<Dataedo.SqlParser.Domain.View>();
		if (list != null)
		{
			foreach (ScriptData item in list)
			{
				list2.AddRange(item.Views);
			}
		}
		foreach (Dataedo.SqlParser.Domain.View view in list2)
		{
			if (synchronizeParameters.DatabaseRow.tableRows.FirstOrDefault((ObjectRow t) => t.Name.Equals(view.Name) && t.Schema.Equals(view.Schema)) == null)
			{
				ObjectRow objectRow = new ObjectRow(view.Name, view.Schema ?? string.Empty, synchronizeParameters.DatabaseRow.Name, "view", "1970-01-01 00:00:00", "1970-01-01 00:00:00", null, view.Body, null, null, synchronizeParameters.DatabaseRow.Id, null);
				objectRow.Subtype = view.Subtype;
				AddDBObject(objectRow, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, owner);
			}
		}
	}

	private void AddProcedures(BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (_scriptData == null)
		{
			return;
		}
		foreach (ScriptData batch in _scriptData.Where((ScriptData s) => s.Type == ScriptType.CREATE_OR_ALTER_PROCEDURE).ToList())
		{
			if (synchronizeParameters.DatabaseRow.tableRows.FirstOrDefault((ObjectRow t) => t.Name.Equals(batch.Procedure.Name) && t.Schema.Equals(batch.Procedure.Schema)) == null)
			{
				ObjectRow row = new ObjectRow(batch.Procedure.Name, batch.Procedure.Schema ?? string.Empty, synchronizeParameters.DatabaseRow.Name, "procedure", "1970-01-01 00:00:00", "1970-01-01 00:00:00", null, batch.Procedure.Body, null, null, synchronizeParameters.DatabaseRow.Id, null);
				AddDBObject(row, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, owner);
			}
		}
	}

	private void AddFunctions(BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (_scriptData == null)
		{
			return;
		}
		foreach (ScriptData item in _scriptData.Where((ScriptData d) => d.Type == ScriptType.CREATE_OR_ALTER_FUNCTION).ToList())
		{
			ObjectRow row = new ObjectRow(item.Function.Name, item.Function.Schema ?? string.Empty, synchronizeParameters.DatabaseRow.Name, "function", "1970-01-01 00:00:00", "1970-01-01 00:00:00", null, item.Function.Body, null, null, synchronizeParameters.DatabaseRow.Id, null);
			AddDBObject(row, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, owner);
		}
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> RelationsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		return new List<string> { "" };
	}

	public override IEnumerable<string> GetProceduresQuery()
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

	public override IEnumerable<string> TriggersQuery()
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

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		return new List<string> { "" };
	}
}
