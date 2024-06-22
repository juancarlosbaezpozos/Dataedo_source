using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class SSISConnection : ConnectionBase
{
	[XmlElement]
	public string Host { get; set; }

	[XmlElement]
	public string Database { get; set; }

	[XmlElement]
	public string ConnectionType { get; set; }

	[XmlElement]
	public string Login { get; set; }

	[XmlElement]
	public XmlTextObject Password { get; set; }

	[XmlElement]
	public int? Port { get; set; }

	[XmlElement]
	public string PackagePath { get; set; }

	public override int? Timeout { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.SSIS;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		return string.Empty;
	}

	public override string GetDatabase()
	{
		return Database;
	}

	public override string GetDatabaseFull()
	{
		return Database + "@" + Host;
	}

	public override string GetHost()
	{
		return Host;
	}

	public override bool? GetIsWindowsAuthentication()
	{
		return false;
	}

	public override string GetLogin()
	{
		return Login;
	}

	public override string GetPassword()
	{
		return string.Empty;
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		ConnectionType = databaseRow.Param1;
		PackagePath = databaseRow.Param2;
		Host = databaseRow.Host;
		Database = databaseRow.Name;
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		databaseRow.Param1 = ConnectionType;
		databaseRow.Param2 = PackagePath;
		databaseRow.Host = Host;
		databaseRow.Name = Database;
		return true;
	}
}
