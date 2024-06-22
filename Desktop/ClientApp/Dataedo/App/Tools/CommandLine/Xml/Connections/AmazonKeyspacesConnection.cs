using System;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class AmazonKeyspacesConnection : CassandraConnection
{
	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.AmazonKeyspaces;
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
		throw new NotSupportedException("Amazon Keyspaces repository is not supported.");
	}
}
