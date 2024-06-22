using System;
using System.Reflection;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums.Extensions;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class CosmosDBSqlConnection : ConnectionBase
{
	[XmlIgnore]
	public override int? Timeout { get; set; }

	[XmlElement]
	public string Database { get; set; }

	[XmlElement]
	public string Host { get; set; }

	[XmlElement]
	public XmlTextObject Password { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.CosmosDBSql;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		string password = string.Empty;
		if (Password != null && Password.Value != null)
		{
			password = ((!Password.IsEncrypted) ? Password.Value : new SimpleAES().DecryptString(Password.Value));
		}
		return Dataedo.App.DatabasesSupport.Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, Database, GetConnectionCommandType(), Host, null, password, AuthenticationType.AuthenticationTypeEnum.StandardAuthentication, null, null, withPooling);
	}

	public override string GetHost()
	{
		return Host;
	}

	public override string GetDatabase()
	{
		return Database;
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
		return Password?.Value;
	}

	public override string GetDatabaseFull()
	{
		return Database + "@" + Host;
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
		throw new NotSupportedException("CosmosDB repository is not supported.");
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		Database = databaseRow.Name;
		Host = databaseRow.Host;
		Password = new XmlTextObject(databaseRow.PasswordEncrypted, isEncrypted: true);
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		databaseRow.Name = Database;
		databaseRow.Host = Host;
		SetPassword(databaseRow, Password);
		databaseRow.Type = GetConnectionCommandType().AsDatabaseType();
		return true;
	}
}
