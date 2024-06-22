using System;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class HiveMetastoreMariaDBConnection : HiveMetastoreMySQLConnection
{
	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.HiveMetastoreMariaDB;
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
		throw new NotSupportedException("Hive Metastore MariaDB repository is not supported.");
	}
}
