using System;
using System.Windows.Forms;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.General;

public class GeneralQueries
{
	public static DatabaseVersionUpdate GetVersionUpdate(SharedDatabaseTypeEnum.DatabaseType? databaseType, object connection)
	{
		return DatabaseSupportFactory.GetDatabaseSupport(databaseType).GetVersion(connection);
	}

	public static SharedDatabaseTypeEnum.DatabaseType? GetDatabaseVersion(SharedDatabaseTypeEnum.DatabaseType? databaseType, object connection, Form owner = null)
	{
		DatabaseVersionUpdate versionUpdate = GetVersionUpdate(databaseType, connection);
		if (versionUpdate == null)
		{
			GeneralMessageBoxesHandling.Show("Error while retrieving database's version.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return null;
		}
		return DatabaseSupportFactory.GetDatabaseSupport(databaseType).CheckVersion(connection, versionUpdate, owner);
	}

	public static SharedDatabaseTypeEnum.DatabaseType? AskUserWhichConnectorUse(SharedDatabaseTypeEnum.DatabaseType? realType, SharedDatabaseTypeEnum.DatabaseType expectedType, object connection, Form owner = null)
	{
		if (!realType.HasValue)
		{
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show("The version of the DBMS downloaded from the source is unrecognized or not supported." + Environment.NewLine + "Connecting might cause some issues." + Environment.NewLine + "Try to connect anyway?", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, null, 1, owner);
			if (handlingDialogResult != null && handlingDialogResult.DialogResult == DialogResult.OK)
			{
				return expectedType;
			}
			return null;
		}
		string text = DatabaseTypeEnum.TypeToStringForDisplay(realType);
		string text2 = DatabaseTypeEnum.TypeToStringForDisplay(expectedType);
		GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult2 = GeneralMessageBoxesHandling.Show("You're connecting to " + text + ", but specified " + text2 + " as DBMS." + Environment.NewLine + "This may cause issues." + Environment.NewLine + "Change DBMS to " + text + "?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 1, owner);
		if (handlingDialogResult2 != null && handlingDialogResult2.DialogResult == DialogResult.Yes)
		{
			return GetDatabaseVersion(realType, connection, owner);
		}
		return expectedType;
	}

	public static DateTime? GetServerTime(object connection, SharedDatabaseTypeEnum.DatabaseType? type, Form owner = null)
	{
		try
		{
			return DatabaseSupportFactory.GetDatabaseSupport(type).GetServerTime(connection);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while retrieving database's time", owner);
			return null;
		}
	}
}
