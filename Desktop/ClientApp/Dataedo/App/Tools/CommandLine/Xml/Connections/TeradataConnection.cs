using System;
using System.Reflection;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums.Extensions;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ImportCommand.Connections;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class TeradataConnection : ConnectionBase
{
	[XmlElement]
	public override int? Timeout { get; set; }

	[XmlElement]
	public string Host { get; set; }

	[XmlElement]
	public int? Port { get; set; }

	[XmlElement]
	public string User { get; set; }

	[XmlElement]
	public XmlTextObject Password { get; set; }

	[XmlElement]
	public DatabaseModel Database { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.Teradata;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		string password = string.Empty;
		if (Password != null && Password.Value != null)
		{
			password = ((!Password.IsEncrypted) ? Password.Value : new SimpleAES().DecryptString(Password.Value));
		}
		return Dataedo.App.DatabasesSupport.Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, Database?.Name, GetConnectionCommandType(), Host, User, password, AuthenticationType.AuthenticationTypeEnum.StandardAuthentication, Port, null, withPooling);
	}

	public override string GetHost()
	{
		return Host;
	}

	public override string GetDatabase()
	{
		return Database?.Name;
	}

	public override string GetDatabaseFull()
	{
		if (!string.IsNullOrEmpty(Database?.Name))
		{
			return Database?.Name + "@" + GetHost();
		}
		if (!string.IsNullOrEmpty(GetLogin()))
		{
			return GetLogin() + "@" + GetHost();
		}
		return GetHost();
	}

	public override bool? GetIsWindowsAuthentication()
	{
		return null;
	}

	public override string GetLogin()
	{
		return User;
	}

	public override string GetPassword()
	{
		return Password?.Value;
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
		throw new NotSupportedException("Teradata repository is not supported.");
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		Host = databaseRow.Host;
		Port = databaseRow.Port;
		User = databaseRow.User;
		Password = new XmlTextObject(databaseRow.PasswordEncrypted, isEncrypted: true);
		Database = new DatabaseModel(databaseRow.Name, databaseRow.HasMultipleSchemas == true);
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		string text2 = (databaseRow.Host = (Host = Host));
		databaseRow.Port = Port;
		databaseRow.User = User;
		SetPassword(databaseRow, Password);
		DatabaseModel database = Database;
		databaseRow.HasMultipleSchemas = ((database != null) ? new bool?(database.IsMultiple) : databaseRow.UseDifferentSchema);
		databaseRow.UseDifferentSchema = databaseRow.HasMultipleSchemas;
		databaseRow.Name = Database?.Name;
		databaseRow.Type = GetConnectionCommandType().AsDatabaseType();
		return true;
	}
}
