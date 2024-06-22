using System;
using System.Collections.Generic;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Model.Data.InterfaceTables;

namespace Dataedo.App.Data.MetadataServer;

internal class InterfaceTablesDB : CommonDBSupport
{
	public InterfaceTablesDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public List<string> GetDatabaseNamesForImport()
	{
		try
		{
			return commands.Select.InterfaceTables.GetDatabaseNamesForImport();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting database names for import.");
		}
		return null;
	}

	public bool ClearInterfaceTablesImportErrors(string databaseName)
	{
		try
		{
			commands.Manipulation.InterfaceTables.ClearInterfaceTablesImportErrors(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while cleaning import errors.");
			return false;
		}
		return true;
	}

	public bool ValidateImportTablesTable(string databaseName)
	{
		try
		{
			commands.Manipulation.InterfaceTables.ValidateImportTablesTable(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while validating the import_tables table.");
			return false;
		}
		return true;
	}

	public bool ValidateImportColumnsTable(string databaseName)
	{
		try
		{
			commands.Manipulation.InterfaceTables.ValidateImportColumnsTable(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while validating the import_columns table.");
			return false;
		}
		return true;
	}

	public bool ValidateImportTablesKeysColumnsTable(string databaseName)
	{
		try
		{
			commands.Manipulation.InterfaceTables.ValidateImportTablesKeysColumnsTable(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while validating the import_tables_keys_columns table.");
			return false;
		}
		return true;
	}

	public bool ValidateImportTablesForeignKeysColumnsTable(string databaseName)
	{
		try
		{
			commands.Manipulation.InterfaceTables.ValidateImportTablesForeignKeysColumnsTable(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while validating the import_tables_foreign_keys_columns table.");
			return false;
		}
		return true;
	}

	public bool ValidateImportProceduresTable(string databaseName)
	{
		try
		{
			commands.Manipulation.InterfaceTables.ValidateImportProceduresTable(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while validating the import_procedures table.");
			return false;
		}
		return true;
	}

	public bool ValidateImportProceduresParametersTable(string databaseName)
	{
		try
		{
			commands.Manipulation.InterfaceTables.ValidateImportProceduresParametersTable(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while validating the import_procedures_parameters table.");
			return false;
		}
		return true;
	}

	public bool ValidateImportTriggersTable(string databaseName)
	{
		try
		{
			commands.Manipulation.InterfaceTables.ValidateImportTriggersTable(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while validating the import_triggers table.");
			return false;
		}
		return true;
	}

	public bool InsertErrorsIntoImportErrorsTable(string databaseName)
	{
		try
		{
			commands.Manipulation.InterfaceTables.InsertErrorsIntoImportErrorsTable(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting errors into import_errors table.");
			return false;
		}
		return true;
	}

	public bool ValidateAllImportTables(string databaseName)
	{
		return true & ClearInterfaceTablesImportErrors(databaseName) & ValidateImportTablesTable(databaseName) & ValidateImportColumnsTable(databaseName) & ValidateImportTablesKeysColumnsTable(databaseName) & ValidateImportTablesForeignKeysColumnsTable(databaseName) & ValidateImportProceduresTable(databaseName) & ValidateImportProceduresParametersTable(databaseName) & ValidateImportTriggersTable(databaseName) & InsertErrorsIntoImportErrorsTable(databaseName);
	}

	public List<InterfaceTableErrorObject> GetImportTablesTableErrors(string databaseName)
	{
		try
		{
			return commands.Select.InterfaceTables.GetImportTablesTableErrors(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting errors from the import_tables table.");
		}
		return null;
	}

	public List<InterfaceTableErrorObject> GetImportColumnsTableErrors(string databaseName)
	{
		try
		{
			return commands.Select.InterfaceTables.GetImportColumnsTableErrors(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting errors from the import_columns table.");
		}
		return null;
	}

	public List<InterfaceTableErrorObject> GetImportTablesKeysColumnsTableErrors(string databaseName)
	{
		try
		{
			return commands.Select.InterfaceTables.GetImportTablesKeysColumnsTableErrors(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting errors from the import_tables_keys_columns table.");
		}
		return null;
	}

	public List<InterfaceTableErrorObject> GetImportTablesForeignKeysColumnsTableErrors(string databaseName)
	{
		try
		{
			return commands.Select.InterfaceTables.GetImportTablesForeignKeysColumnsTableErrors(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting errors from the import_tables_foreign_keys_columns table.");
		}
		return null;
	}

	public List<InterfaceTableErrorObject> GetImportProceduresTableErrors(string databaseName)
	{
		try
		{
			return commands.Select.InterfaceTables.GetImportProceduresTableErrors(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting errors from the import_procedures table.");
		}
		return null;
	}

	public List<InterfaceTableErrorObject> GetImportProceduresParametersTableErrors(string databaseName)
	{
		try
		{
			return commands.Select.InterfaceTables.GetImportProceduresParametersTableErrors(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting errors from the import_procedures_parameters table.");
		}
		return null;
	}

	public List<InterfaceTableErrorObject> GetImportTriggersTableErrors(string databaseName)
	{
		try
		{
			return commands.Select.InterfaceTables.GetImportTriggersTableErrors(databaseName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while getting errors from the import_triggers table.");
		}
		return null;
	}

	public List<InterfaceTableErrorObject> GetAllImportErrors(string databaseName)
	{
		List<InterfaceTableErrorObject> list = new List<InterfaceTableErrorObject>();
		list.AddRange(GetImportTablesTableErrors(databaseName) ?? new List<InterfaceTableErrorObject>());
		list.AddRange(GetImportColumnsTableErrors(databaseName) ?? new List<InterfaceTableErrorObject>());
		list.AddRange(GetImportTablesKeysColumnsTableErrors(databaseName) ?? new List<InterfaceTableErrorObject>());
		list.AddRange(GetImportTablesForeignKeysColumnsTableErrors(databaseName) ?? new List<InterfaceTableErrorObject>());
		list.AddRange(GetImportProceduresTableErrors(databaseName) ?? new List<InterfaceTableErrorObject>());
		list.AddRange(GetImportProceduresParametersTableErrors(databaseName) ?? new List<InterfaceTableErrorObject>());
		list.AddRange(GetImportTriggersTableErrors(databaseName) ?? new List<InterfaceTableErrorObject>());
		return list;
	}
}
