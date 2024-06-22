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
using Dataedo.Model.Data.SSAS;
using Dataedo.Shared.Enums;
using Microsoft.AnalysisServices;
using Microsoft.AnalysisServices.Tabular;

namespace Dataedo.App.Data.SsasTabular;

internal class SynchronizeSsasTabular : SynchronizeDatabase
{
	public SynchronizeSsasTabular(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
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
		return new List<string>();
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		return new List<string>();
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		return new List<string>();
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		return new List<string>();
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		return new List<string>();
	}

	public override IEnumerable<string> TriggersQuery()
	{
		return new List<string>();
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		return new List<string>();
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		return new List<string>();
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		return new List<string>();
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		throw new NotImplementedException();
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		try
		{
			IEnumerable<string> tablesToSynchronize = from t in synchronizeParameters.DatabaseRow.tableRows
				where t.ToSynchronize
				select t.Name;
			IEnumerable<string> enumerable = tablesToSynchronize;
			if (enumerable != null && enumerable.Any() && !string.IsNullOrEmpty(synchronizeParameters.DatabaseRow.Perspective))
			{
				Microsoft.AnalysisServices.Database database = CommandsWithTimeout.SSASTabular(synchronizeParameters.DatabaseRow.Connection).Databases.FindByName(synchronizeParameters.DatabaseName);
				if (synchronizeParameters.DatabaseRow.Perspective == database?.Model.Name)
				{
					IEnumerable<Column> enumerable2 = database?.Model.Tables?.Where((Table t) => tablesToSynchronize.Contains(t.Name))?.SelectMany((Table s) => s.Columns);
					if (enumerable2 != null && enumerable2.Any())
					{
						foreach (Column item in enumerable2)
						{
							AddColumn(item, withCustomFields: true);
						}
					}
					IEnumerable<Microsoft.AnalysisServices.Tabular.Measure> enumerable3 = database?.Model.Tables?.Where((Table t) => tablesToSynchronize.Contains(t.Name))?.SelectMany((Table s) => s.Measures);
					if (enumerable3 != null && enumerable3.Any())
					{
						foreach (Microsoft.AnalysisServices.Tabular.Measure item2 in enumerable3)
						{
							AddColumn(item2, withCustomFields: true);
						}
					}
				}
				else
				{
					IEnumerable<PerspectiveColumn> enumerable4 = database?.Model.Perspectives.Find(synchronizeParameters.DatabaseRow.Perspective)?.PerspectiveTables?.Where((PerspectiveTable t) => tablesToSynchronize.Contains(t.Name))?.SelectMany((PerspectiveTable t) => t.PerspectiveColumns);
					if (enumerable4 != null)
					{
						foreach (PerspectiveColumn item3 in enumerable4)
						{
							AddColumn(item3, withCustomFields: true);
						}
					}
					IEnumerable<Microsoft.AnalysisServices.Tabular.PerspectiveMeasure> enumerable5 = database?.Model.Perspectives.Find(synchronizeParameters.DatabaseRow.Perspective)?.PerspectiveTables?.Where((PerspectiveTable t) => tablesToSynchronize.Contains(t.Name))?.SelectMany((PerspectiveTable s) => s.PerspectiveMeasures);
					if (enumerable5 != null && enumerable5.Any())
					{
						foreach (Microsoft.AnalysisServices.Tabular.PerspectiveMeasure item4 in enumerable5)
						{
							AddColumn(item4, withCustomFields: true);
						}
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

	public override bool GetDependencies(string query, Form owner = null)
	{
		return false;
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		try
		{
			if (string.IsNullOrEmpty(synchronizeParameters.DatabaseRow.Perspective))
			{
				return false;
			}
			backgroundWorkerManager.ReportProgress("Retrieving database's objects");
			List<NamedMetadataObject> list = new List<NamedMetadataObject>();
			Microsoft.AnalysisServices.Server server = CommandsWithTimeout.SSASTabular(synchronizeParameters.DatabaseRow.Connection);
			Microsoft.AnalysisServices.Database database = server.Databases?.FindByName(synchronizeParameters.DatabaseName);
			switch (query.ObjectType)
			{
			case FilterObjectTypeEnum.FilterObjectType.Any:
				if (synchronizeParameters.DatabaseRow.Perspective == database?.Model.Name)
				{
					TableCollection tableCollection2 = server.Databases?.FindByName(synchronizeParameters.DatabaseName)?.Model.Tables;
					if (tableCollection2 != null)
					{
						list.AddRange(tableCollection2.ToList());
					}
				}
				else
				{
					PerspectiveTableCollection perspectiveTableCollection2 = database?.Model.Perspectives.Find(synchronizeParameters.DatabaseRow.Perspective)?.PerspectiveTables;
					if (perspectiveTableCollection2 != null)
					{
						list.AddRange(perspectiveTableCollection2.ToList());
					}
				}
				break;
			case FilterObjectTypeEnum.FilterObjectType.Table:
				if (synchronizeParameters.DatabaseRow.Perspective == database?.Model.Name)
				{
					TableCollection tableCollection = database?.Model.Tables;
					if (tableCollection != null)
					{
						list.AddRange(tableCollection.ToList());
					}
				}
				else
				{
					PerspectiveTableCollection perspectiveTableCollection = database?.Model.Perspectives.Find(synchronizeParameters.DatabaseRow.Perspective)?.PerspectiveTables;
					if (perspectiveTableCollection != null)
					{
						list.AddRange(perspectiveTableCollection.ToList());
					}
				}
				break;
			}
			foreach (NamedMetadataObject item in list)
			{
				AddDBObject(item, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
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
		try
		{
			IEnumerable<string> tablesToSynchronize = from t in synchronizeParameters.DatabaseRow.tableRows
				where t.ToSynchronize
				select t.Name;
			IEnumerable<string> enumerable = tablesToSynchronize;
			if (enumerable != null && enumerable.Any())
			{
				IEnumerable<Microsoft.AnalysisServices.Tabular.Relationship> enumerable2 = CommandsWithTimeout.SSASTabular(synchronizeParameters.DatabaseRow.Connection).Databases.FindByName(synchronizeParameters.DatabaseName)?.Model.Relationships?.Where((Microsoft.AnalysisServices.Tabular.Relationship r) => tablesToSynchronize.Contains(r.FromTable?.Name) || tablesToSynchronize.Contains(r.ToTable?.Name));
				if (enumerable2 != null)
				{
					foreach (SingleColumnRelationship item in enumerable2)
					{
						AddRelation(new SsasRelation(item.FromTable?.Model?.Database?.Name, item.FromTable?.Name, string.Empty, item.FromColumn?.Name, item.ToTable?.Model?.Database?.Name, item.ToTable?.Name, string.Empty, item.ToColumn?.Name), SharedDatabaseTypeEnum.DatabaseType.SsasTabular, withCustomFields: true);
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

	public override bool GetTriggers(string query, Form owner = null)
	{
		return false;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		base.UniqueConstraintRows = new ObservableCollection<UniqueConstraintRow>();
		try
		{
			IEnumerable<string> tablesToSynchronize = from t in synchronizeParameters.DatabaseRow.tableRows
				where t.ToSynchronize
				select t.Name;
			IEnumerable<string> enumerable = tablesToSynchronize;
			if (enumerable != null && enumerable.Any() && !string.IsNullOrEmpty(synchronizeParameters.DatabaseRow.Perspective))
			{
				Microsoft.AnalysisServices.Database database = CommandsWithTimeout.SSASTabular(synchronizeParameters.DatabaseRow.Connection).Databases?.FindByName(synchronizeParameters.DatabaseName);
				if (synchronizeParameters.DatabaseRow.Perspective == database?.Model.Name)
				{
					IEnumerable<Column> enumerable2 = database?.Model.Tables.Where((Table t) => tablesToSynchronize.Contains(t.Name))?.SelectMany((Table s) => s.Columns)?.Where((Column c) => c.IsKey && c.IsUnique);
					if (enumerable2 != null)
					{
						foreach (Column item in enumerable2)
						{
							AddUniqueConstraint(new SsasConstraint(item.Name, item.Table?.Name, string.Empty, synchronizeParameters.DatabaseRow?.Name, item.Name, "P"), SharedDatabaseTypeEnum.DatabaseType.SsasTabular, withCustomFields: true);
						}
					}
				}
				else
				{
					IEnumerable<PerspectiveColumn> enumerable3 = database?.Model.Perspectives.Find(synchronizeParameters.DatabaseRow.Perspective).PerspectiveTables.Where((PerspectiveTable t) => tablesToSynchronize.Contains(t.Name))?.SelectMany((PerspectiveTable s) => s.PerspectiveColumns)?.Where((PerspectiveColumn c) => c.Column.IsKey && c.Column.IsUnique);
					if (enumerable3 != null)
					{
						foreach (PerspectiveColumn item2 in enumerable3)
						{
							AddUniqueConstraint(new SsasConstraint(item2.Name, item2.PerspectiveTable?.Name, string.Empty, synchronizeParameters.DatabaseRow?.Name, item2.Name, "P"), SharedDatabaseTypeEnum.DatabaseType.SsasTabular, withCustomFields: true);
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

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
		return false;
	}
}
