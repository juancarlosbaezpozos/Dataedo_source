using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Export;
using Dataedo.Model.Data.Tables.Constraints;
using Dataedo.Model.Data.Tables.Relations;

namespace Dataedo.App.Tools.DDLGenerating;

public class DDLGenerator
{
	private DDLGeneratorSettings settings = new DDLGeneratorSettings();

	public DDLGenerator(DDLGeneratorSettings settings)
	{
		this.settings = settings;
	}

	public void ProcessDatabase(int id, IEnumerable<DDLObject> ddlObjects, BackgroundProcessingWorker exportWorker, string outputFileName, Form owner = null)
	{
		IEnumerable<DDLObject> source = ddlObjects.Where((DDLObject x) => x.ParentId == 0);
		IEnumerable<DDLObject> enumerable = source.Where((DDLObject x) => x.Checked);
		IEnumerable<DDLObject> selectedColumns = ddlObjects.Where((DDLObject x) => x.ParentId != 0 && x.Checked);
		IEnumerable<DDLObject> enumerable2 = source.Where((DDLObject x) => !x.Checked && selectedColumns.Any((DDLObject y) => y.ParentId == x.Id));
		StringBuilder stringBuilder = new StringBuilder();
		int num = 1;
		int num2 = enumerable.Count() + enumerable2.Count();
		if (settings.CreateFKScript)
		{
			num2 += enumerable.Count();
		}
		try
		{
			exportWorker.ReportProgress("Preparing...", 0);
			exportWorker.SetTotalProgressStep(100f);
			exportWorker.DivideProgressStep(num2);
			foreach (DDLObject table2 in enumerable)
			{
				if (exportWorker.IsCancelled)
				{
					return;
				}
				IEnumerable<DDLObject> enumerable3 = selectedColumns.Where((DDLObject x) => x.ParentId == table2.Id);
				if (enumerable3.Any())
				{
					string createTableScript = GetCreateTableScript(table2.Id, table2.Schema, table2.Name, enumerable3);
					stringBuilder.Append(createTableScript);
					exportWorker.ReportProgress($"Processing {num++} of {num2}");
					exportWorker.IncreaseProgress();
				}
			}
			foreach (DDLObject table in enumerable2)
			{
				if (exportWorker.IsCancelled)
				{
					return;
				}
				List<DDLObject> columns = selectedColumns.Where((DDLObject x) => x.ParentId == table.Id).ToList();
				string alterTableScript = GetAlterTableScript(table.Id, table.Schema, table.Name, columns);
				stringBuilder.Append(alterTableScript);
				exportWorker.ReportProgress($"Processing {num++} of {num2}");
				exportWorker.IncreaseProgress();
			}
			if (settings.CreateFKScript)
			{
				foreach (DDLObject item in enumerable)
				{
					if (exportWorker.IsCancelled)
					{
						return;
					}
					AppendForeignKeysScript(stringBuilder, item.Id, item.Schema, item.Name, selectedColumns);
					exportWorker.ReportProgress($"Processing {num++} of {num2}");
					exportWorker.IncreaseProgress();
				}
			}
			File.WriteAllText(outputFileName, stringBuilder.ToString());
		}
		catch (OperationCanceledException)
		{
			exportWorker.HasResult = false;
			return;
		}
		catch (Exception exception)
		{
			exportWorker.HasError = true;
			GeneralExceptionHandling.Handle(exception, "Error while exporting descriptions.", owner);
		}
		exportWorker.HasResult = true;
	}

	private string GetCreateTableScript(int tableId, string schema, string name, IEnumerable<DDLObject> columns)
	{
		StringBuilder stringBuilder = new StringBuilder("CREATE TABLE ");
		AppendSchema(stringBuilder, schema);
		AppendName(stringBuilder, name);
		stringBuilder.AppendLine(" (");
		AppendColumnsScript(stringBuilder, columns.ToList());
		AppendConstraintsScript(stringBuilder, tableId, columns);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine(");");
		stringBuilder.AppendLine();
		return stringBuilder.ToString();
	}

	private string GetAlterTableScript(int tableId, string schema, string name, List<DDLObject> columns)
	{
		if (!columns.Any())
		{
			return string.Empty;
		}
		StringBuilder stringBuilder = new StringBuilder("ALTER TABLE ");
		AppendSchema(stringBuilder, schema);
		AppendName(stringBuilder, name);
		stringBuilder.AppendLine();
		stringBuilder.AppendLine("ADD");
		AppendColumnsScript(stringBuilder, columns);
		AppendConstraintsScript(stringBuilder, tableId, columns);
		stringBuilder.AppendLine(";");
		stringBuilder.AppendLine();
		return stringBuilder.ToString();
	}

	private void AppendSchema(StringBuilder builder, string schema)
	{
		if (settings.ShowSchema && !string.IsNullOrEmpty(schema))
		{
			AppendName(builder, schema);
			builder.Append('.');
		}
	}

	private void AppendName(StringBuilder builder, string name)
	{
		if (!string.IsNullOrEmpty(name))
		{
			builder.Append(settings.OpenEscapeCharacter);
			builder.Append(name);
			builder.Append(settings.CloseEscapeCharacter);
		}
	}

	private void AppendColumnsScript(StringBuilder builder, List<DDLObject> columns)
	{
		for (int i = 0; i < columns.Count; i++)
		{
			DDLObject dDLObject = columns[i];
			builder.Append('\t');
			AppendName(builder, dDLObject.Name);
			if (!string.IsNullOrWhiteSpace(dDLObject.DatatypeLen))
			{
				builder.Append(' ');
				builder.Append(SplitDataType(dDLObject.DatatypeLen));
			}
			if (settings.ShowNullability)
			{
				builder.Append(' ');
				builder.Append(dDLObject.Nullable ? "NULL" : "NOT NULL");
			}
			if (settings.ShowDefault && !string.IsNullOrWhiteSpace(dDLObject.DefaultValue))
			{
				builder.Append(" DEFAULT ");
				builder.Append(dDLObject.DefaultValue);
			}
			if (i != columns.Count - 1)
			{
				builder.AppendLine(",");
			}
		}
	}

	private static string SplitDataType(string dataType)
	{
		return dataType.Split(':')[0];
	}

	private void AppendConstraintsScript(StringBuilder builder, int tableId, IEnumerable<DDLObject> columns)
	{
		if (!settings.CreatePKScript && !settings.CreateUKScript)
		{
			return;
		}
		List<UniqueConstraintWithColumnObject> dataByTableWithColumns = DB.Constraint.GetDataByTableWithColumns(tableId);
		if (settings.CreatePKScript)
		{
			IEnumerable<UniqueConstraintWithColumnObject> constraints = dataByTableWithColumns.Where((UniqueConstraintWithColumnObject x) => x.PrimaryKey);
			AppendConstraintsScript(builder, constraints, isPK: true, columns);
		}
		if (settings.CreateUKScript)
		{
			IEnumerable<UniqueConstraintWithColumnObject> constraints2 = dataByTableWithColumns.Where((UniqueConstraintWithColumnObject x) => !x.PrimaryKey);
			AppendConstraintsScript(builder, constraints2, isPK: false, columns);
		}
	}

	private void AppendConstraintsScript(StringBuilder builder, IEnumerable<UniqueConstraintWithColumnObject> constraints, bool isPK, IEnumerable<DDLObject> selectedColumns)
	{
		if (!constraints.Any())
		{
			return;
		}
		string text = (isPK ? "PRIMARY KEY" : "UNIQUE");
		foreach (IGrouping<int, UniqueConstraintWithColumnObject> item in from x in constraints
			group x by x.UniqueConstraintId)
		{
			IEnumerable<UniqueConstraintWithColumnObject> source = item.Where((UniqueConstraintWithColumnObject x) => !string.IsNullOrWhiteSpace(x.ColumnName));
			if (source.Any() && !source.Any((UniqueConstraintWithColumnObject x) => !selectedColumns.Any((DDLObject y) => y.Id == x.ColumnId)))
			{
				builder.AppendLine(",");
				builder.Append("\tCONSTRAINT ");
				AppendName(builder, source.First().Name);
				builder.Append(" " + text + " (");
				builder.Append(string.Join(", ", source.Select((UniqueConstraintWithColumnObject x) => settings.OpenEscapeCharacter + x.ColumnName + settings.CloseEscapeCharacter)));
				builder.Append(")");
			}
		}
	}

	private void AppendForeignKeysScript(StringBuilder builder, int tableId, string schema, string tableName, IEnumerable<DDLObject> columns)
	{
		IEnumerable<IGrouping<int, RelationWithColumnAndUniqueConstraint>> enumerable = from x in DB.Relation.GetDataByTableWithColumnsAndUniqueConstraints(tableId)
			where x.IsFk == 0
			group x by x.RelationWithUniqueConstraint.TableRelationId;
		if (!enumerable.Any())
		{
			return;
		}
		foreach (IGrouping<int, RelationWithColumnAndUniqueConstraint> item in enumerable)
		{
			var source = item.Select((RelationWithColumnAndUniqueConstraint x) => new
			{
				x.RelationWithUniqueConstraint.ColumnFkId,
				x.RelationWithUniqueConstraint.ColumnFkName
			});
			var source2 = item.Select((RelationWithColumnAndUniqueConstraint x) => new
			{
				x.RelationWithUniqueConstraint.ColumnPkId,
				x.RelationWithUniqueConstraint.ColumnPkName
			});
			if (source.Any() && source2.Any() && !source.Any(x => string.IsNullOrEmpty(x.ColumnFkName)) && !source2.Any(x => string.IsNullOrEmpty(x.ColumnPkName)) && !source.Any(x => !columns.Any((DDLObject y) => y.Id == x.ColumnFkId)) && !source2.Any(x => !columns.Any((DDLObject y) => y.Id == x.ColumnPkId)))
			{
				IEnumerable<string> values = source.Select(x => settings.OpenEscapeCharacter + x.ColumnFkName + settings.CloseEscapeCharacter);
				IEnumerable<string> values2 = source2.Select(x => settings.OpenEscapeCharacter + x.ColumnPkName + settings.CloseEscapeCharacter);
				builder.Append("ALTER TABLE ");
				AppendSchema(builder, schema);
				AppendName(builder, tableName);
				builder.Append(" ADD CONSTRAINT ");
				AppendName(builder, item.First().Name);
				builder.Append(" FOREIGN KEY (");
				builder.Append(string.Join(", ", values));
				builder.AppendLine(")");
				builder.Append("REFERENCES ");
				if (settings.ShowSchema && !string.IsNullOrEmpty(schema))
				{
					AppendName(builder, item.First().PkTableObject.Schema);
					builder.Append('.');
				}
				AppendName(builder, item.First().PkTableObject.Name);
				builder.Append(" (");
				builder.Append(string.Join(", ", values2));
				builder.AppendLine(");");
				builder.AppendLine();
			}
		}
	}
}
