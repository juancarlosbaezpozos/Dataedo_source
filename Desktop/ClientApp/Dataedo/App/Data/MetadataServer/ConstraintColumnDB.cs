using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Parameters.Types;

namespace Dataedo.App.Data.MetadataServer;

internal class ConstraintColumnDB : CommonDBSupport
{
	public ConstraintColumnDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public int Synchronize(IEnumerable<UniqueConstraintRow> uniqueConstraintColumns, string tableName, string schema, int databaseId, bool isDbAdded, int updateId, Form owner = null)
	{
		try
		{
			UniqueConstraintColumnForSynchronization[] items = uniqueConstraintColumns.SelectMany((UniqueConstraintRow r) => r.Columns.Select((UniqueConstraintColumnRow c) => new UniqueConstraintColumnForSynchronization
			{
				BaseName = tableName,
				BaseSchema = schema,
				BaseId = databaseId,
				ConstraintName = r.Name,
				ColumnName = c.ColumnName,
				OrdinalPosition = c.OrdinalPosition,
				UpdateId = updateId,
				ColumnPath = c.ColumnPath
			})).ToArray();
			return commands.Synchronization.ConstraintsColumns.SynchronizeUniqueConstraintColumns(items, tableName, schema, databaseId, isDbAdded, updateId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while synchronizing the unique constraints' columns:", owner);
			return -1;
		}
	}
}
