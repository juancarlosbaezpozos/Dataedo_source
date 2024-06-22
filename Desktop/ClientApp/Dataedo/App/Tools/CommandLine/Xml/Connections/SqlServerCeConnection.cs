using System;
using System.Reflection;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class SqlServerCeConnection : ConnectionBase
{
	[XmlElement]
	public override int? Timeout { get; set; }

	[XmlElement]
	public string Path { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.SqlServerCe;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		return Dataedo.App.DatabasesSupport.Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, Path, GetConnectionCommandType(), null, null, null, AuthenticationType.AuthenticationTypeEnum.StandardAuthentication, null, null, withPooling);
	}

	public override string GetHost()
	{
		return Path;
	}

	public override string GetDatabase()
	{
		return Path;
	}

	public override string GetDatabaseFull()
	{
		return Path;
	}

	public override bool? GetIsWindowsAuthentication()
	{
		return null;
	}

	public override string GetLogin()
	{
		return null;
	}

	public override string GetPassword()
	{
		return null;
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
		Path = loginInfo.ProjectFilePath;
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		throw new NotSupportedException("SQL Server Compact as source database is not supported.");
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		throw new NotSupportedException("SQL Server Compact as source database is not supported.");
	}
}
