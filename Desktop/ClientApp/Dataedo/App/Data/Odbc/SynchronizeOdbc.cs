using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Odbc;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Drivers.ODBC;
using Dataedo.App.Drivers.ODBC.Repositories;
using Dataedo.App.Drivers.ODBC.ValueObjects;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Data.Odbc.DataCatalog;
using Dataedo.Data.Odbc.DataCatalog.Objects;
using Dataedo.Shared.Enums;
using RazorEngine;
using RazorEngine.Templating;

namespace Dataedo.App.Data.Odbc;

internal class SynchronizeOdbc : SynchronizeDatabase
{
	protected Driver driver;

	private bool wasDataCatalogCountTableObjectsRun;

	private bool wasDataCatalogCountViewObjectsRun;

	private bool wasDataCatalogGetTableObjectsRun;

	private bool wasDataCatalogGetViewObjectsRun;

	private string GetMd5Hash(string input)
	{
		MD5 mD = MD5.Create();
		byte[] bytes = System.Text.Encoding.ASCII.GetBytes(input);
		byte[] array = mD.ComputeHash(bytes);
		StringBuilder stringBuilder = new StringBuilder();
		byte[] array2 = array;
		foreach (byte b in array2)
		{
			stringBuilder.Append(b.ToString("X2"));
		}
		return stringBuilder.ToString();
	}

	private string Template(string query)
	{
		string text = query ?? string.Empty;
		return Engine.Razor.RunCompile(text, GetMd5Hash(text), null, synchronizeParameters);
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		yield return Template(driver.Queries.CountTablesQuery);
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		yield return Template(driver.Queries.CountViewsQuery);
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		yield return Template(driver.Queries.CountProceduresQuery);
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		yield return Template(driver.Queries.CountFunctionsQuery);
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		yield return Template(driver.Queries.GetTablesQuery);
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		yield return Template(driver.Queries.GetViewsQuery);
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		yield return Template(driver.Queries.GetProceduresQuery);
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		yield return Template(driver.Queries.GetFunctionsQuery);
	}

	public override IEnumerable<string> RelationsQuery()
	{
		yield return Template(driver.Queries.RelationsQuery);
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		yield return Template(driver.Queries.ColumnsQuery);
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return Template(driver.Queries.TriggersQuery);
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		yield return Template(driver.Queries.UniqueConstraintsQuery);
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		yield return Template(driver.Queries.ParametersQuery);
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		yield return Template(driver.Queries.DependenciesQuery);
	}

	public SynchronizeOdbc(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
		driver = LoadDriver();
	}

	private Driver LoadDriver()
	{
		string serviceName = synchronizeParameters.DatabaseRow.ServiceName;
		IRepository localRepository = Dataedo.App.Drivers.ODBC.Factory.GetLocalRepository();
		if (localRepository.Has(serviceName))
		{
			return localRepository.Load(serviceName);
		}
		return Dataedo.App.Drivers.ODBC.Factory.MakeEmptyDriver();
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (string.IsNullOrEmpty(query.Query))
		{
			if (query.ObjectType == FilterObjectTypeEnum.FilterObjectType.Table)
			{
				return DataCatalogCountTableObjects(backgroundWorkerManager, owner);
			}
			if (query.ObjectType == FilterObjectTypeEnum.FilterObjectType.View)
			{
				return DataCatalogCountViewObjects(backgroundWorkerManager, owner);
			}
			return true;
		}
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		new ObservableCollection<ObjectRow>();
		try
		{
			using OdbcCommand odbcCommand = CommandsWithTimeout.Odbc(query.Query, synchronizeParameters.DatabaseRow.Connection);
			using OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
			while (odbcDataReader.Read())
			{
				if (synchronizeParameters.DbSynchLocker.IsCanceled)
				{
					return false;
				}
				ReadCount(odbcDataReader);
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	private bool DataCatalogCountTableObjects(BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (!wasDataCatalogCountTableObjectsRun)
		{
			wasDataCatalogCountTableObjectsRun = true;
			backgroundWorkerManager.ReportProgress("Counting objects to import");
			new ObservableCollection<ObjectRow>();
			try
			{
				IReader reader = Dataedo.Data.Odbc.DataCatalog.Factory.MakeReader(synchronizeParameters.DatabaseRow.Connection as OdbcConnection);
				ReadCount(SharedObjectTypeEnum.ObjectType.Table, reader.CountTables());
			}
			catch (Exception ex)
			{
				GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
				return false;
			}
		}
		return true;
	}

	private bool DataCatalogCountViewObjects(BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (!wasDataCatalogCountViewObjectsRun)
		{
			wasDataCatalogCountViewObjectsRun = true;
			backgroundWorkerManager.ReportProgress("Counting objects to import");
			new ObservableCollection<ObjectRow>();
			try
			{
				IReader reader = Dataedo.Data.Odbc.DataCatalog.Factory.MakeReader(synchronizeParameters.DatabaseRow.Connection as OdbcConnection);
				ReadCount(SharedObjectTypeEnum.ObjectType.Table, reader.CountTables());
			}
			catch (Exception ex)
			{
				GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
				return false;
			}
		}
		return true;
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (string.IsNullOrEmpty(query.Query))
		{
			if (query.ObjectType == FilterObjectTypeEnum.FilterObjectType.Table)
			{
				return DataCatalogGetTableObjects(backgroundWorkerManager, owner);
			}
			if (query.ObjectType == FilterObjectTypeEnum.FilterObjectType.View)
			{
				return DataCatalogGetViewObjects(backgroundWorkerManager, owner);
			}
			return true;
		}
		if (base.ObjectsCounter != null)
		{
			try
			{
				backgroundWorkerManager.ReportProgress("Retrieving database's objects");
				using OdbcCommand odbcCommand = CommandsWithTimeout.Odbc(query.Query, synchronizeParameters.DatabaseRow.Connection);
				using OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
				while (odbcDataReader.Read())
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					AddDBObject(odbcDataReader, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, null, null, owner);
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

	private bool DataCatalogGetTableObjects(BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (!wasDataCatalogGetTableObjectsRun)
		{
			wasDataCatalogGetTableObjectsRun = true;
			if (base.ObjectsCounter != null)
			{
				try
				{
					backgroundWorkerManager.ReportProgress("Retrieving database's objects");
					IReader reader = Dataedo.Data.Odbc.DataCatalog.Factory.MakeReader(synchronizeParameters.DatabaseRow.Connection as OdbcConnection);
					DataCatalogGetTableObjects(reader, backgroundWorkerManager, owner);
				}
				catch (Exception exception)
				{
					GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
					return false;
				}
			}
		}
		return true;
	}

	private bool DataCatalogGetViewObjects(BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		if (!wasDataCatalogGetViewObjectsRun)
		{
			wasDataCatalogGetViewObjectsRun = true;
			if (base.ObjectsCounter != null)
			{
				try
				{
					backgroundWorkerManager.ReportProgress("Retrieving database's objects");
					IReader reader = Dataedo.Data.Odbc.DataCatalog.Factory.MakeReader(synchronizeParameters.DatabaseRow.Connection as OdbcConnection);
					DataCatalogGetViewObjects(reader, backgroundWorkerManager, owner);
				}
				catch (Exception exception)
				{
					GeneralExceptionHandling.Handle(exception, null, ErrorMessageCaption, owner);
					return false;
				}
			}
		}
		return true;
	}

	private void DataCatalogGetTableObjects(IReader reader, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		foreach (Table item in reader.FindTables())
		{
			if (item.IsUser)
			{
				ObjectRow row = new ObjectRow(item.Name, (item.Schema == null) ? string.Empty : item.Schema, synchronizeParameters.DatabaseRow.Name, "table", "1970-01-01 00:00:00", "1970-01-01 00:00:00", item.Description, null, null, null, synchronizeParameters.DatabaseRow.Id, synchronizeParameters.DatabaseRow.HasMultipleSchemas);
				AddDBObject(row, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, owner);
			}
		}
	}

	private void DataCatalogGetViewObjects(IReader reader, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		foreach (Dataedo.Data.Odbc.DataCatalog.Objects.View item in reader.FindViews())
		{
			if (item.IsUser)
			{
				ObjectRow row = new ObjectRow(item.Name, (item.Schema == null) ? string.Empty : item.Schema, synchronizeParameters.DatabaseRow.Name, "view", "1970-01-01 00:00:00", "1970-01-01 00:00:00", item.Description, null, null, null, synchronizeParameters.DatabaseRow.Id, synchronizeParameters.DatabaseRow.HasMultipleSchemas);
				AddDBObject(row, synchronizeParameters.DatabaseRow.tableRows, backgroundWorkerManager, owner);
			}
		}
	}

	public override bool GetRelations(string query, Form owner = null)
	{
		if (string.IsNullOrEmpty(query))
		{
			return true;
		}
		try
		{
			using OdbcCommand odbcCommand = CommandsWithTimeout.Odbc(query, synchronizeParameters.DatabaseRow.Connection);
			using OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
			while (odbcDataReader.Read())
			{
				AddRelation(odbcDataReader, SharedDatabaseTypeEnum.DatabaseType.Odbc);
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		if (string.IsNullOrEmpty(query))
		{
			return DataCatalogGetColumns(owner);
		}
		try
		{
			using OdbcCommand odbcCommand = CommandsWithTimeout.Odbc(query, synchronizeParameters.DatabaseRow.Connection);
			using OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
			while (odbcDataReader.Read())
			{
				AddColumn(odbcDataReader);
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	private bool DataCatalogGetColumns(Form owner = null)
	{
		try
		{
			foreach (Column item in Dataedo.Data.Odbc.DataCatalog.Factory.MakeReader(synchronizeParameters.DatabaseRow.Connection as OdbcConnection).FindColumns())
			{
				ColumnRow row = new ColumnRow
				{
					Name = item.Name,
					DatabaseName = synchronizeParameters.DatabaseName,
					TableName = item.Object.Name,
					TableSchema = ((item.Object.Schema == null) ? string.Empty : item.Object.Schema),
					DataType = item.Type,
					Position = item.OridinalPosition,
					Description = item.Description,
					ConstraintType = null,
					Nullable = item.IsNullable,
					DefaultValue = item.DefaultValue,
					IsIdentity = false,
					IsComputed = false,
					ComputedFormula = null,
					DataLength = item.TypeSize.ToString()
				};
				AddColumn(row);
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
		if (string.IsNullOrEmpty(query))
		{
			return true;
		}
		try
		{
			using OdbcCommand odbcCommand = CommandsWithTimeout.Odbc(query, synchronizeParameters.DatabaseRow.Connection);
			using OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
			while (odbcDataReader.Read())
			{
				AddTrigger(odbcDataReader);
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
		if (string.IsNullOrEmpty(query))
		{
			return true;
		}
		try
		{
			using OdbcCommand odbcCommand = CommandsWithTimeout.Odbc(query, synchronizeParameters.DatabaseRow.Connection);
			using OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
			while (odbcDataReader.Read())
			{
				AddUniqueConstraint(odbcDataReader, SharedDatabaseTypeEnum.DatabaseType.Odbc);
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
		if (string.IsNullOrEmpty(query))
		{
			return true;
		}
		try
		{
			using OdbcCommand odbcCommand = CommandsWithTimeout.Odbc(query, synchronizeParameters.DatabaseRow.Connection);
			using OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
			while (odbcDataReader.Read())
			{
				AddParameter(odbcDataReader);
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
		if (string.IsNullOrEmpty(query))
		{
			return true;
		}
		try
		{
			using OdbcCommand odbcCommand = CommandsWithTimeout.Odbc(query, synchronizeParameters.DatabaseRow.Connection);
			using OdbcDataReader odbcDataReader = odbcCommand.ExecuteReader();
			while (odbcDataReader.Read())
			{
				AddDependency(odbcDataReader);
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
