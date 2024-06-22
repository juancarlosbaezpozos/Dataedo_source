using System;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Enums.Extensions;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class MariaDBConnection : MySQLConnection
{
	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.MariaDB;
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
		throw new NotSupportedException("MariaDB repository is not supported.");
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		base.SetDataInDatabaseRow(databaseRow);
		databaseRow.Type = GetConnectionCommandType().AsDatabaseType();
		return true;
	}
}
