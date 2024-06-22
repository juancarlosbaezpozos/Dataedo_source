using System;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.Enums;
using Dataedo.App.Enums.Extensions;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ImportCommand.Connections;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class MongoDBConnectionConnectionString : ConnectionBase
{
	[XmlIgnore]
	public override int? Timeout { get; set; }

	[XmlElement]
	public string ConnectionString { get; set; }

	[XmlElement]
	public XmlTextObject Password { get; set; }

	[XmlElement]
	public DatabaseModel Database { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.MongoDB;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		_ = string.Empty;
		if (Password != null && Password.Value != null)
		{
			if (Password.IsEncrypted)
			{
				new SimpleAES().DecryptString(Password.Value);
			}
			else
			{
				_ = Password.Value;
			}
		}
		return ConnectionString;
	}

	public override string GetHost()
	{
		return MongoDBSupport.GetHostFromConnectionString(ConnectionString);
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
		return MongoDBSupport.GetLoginFromConnectionString(ConnectionString);
	}

	public override string GetPassword()
	{
		return Password?.Value;
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
		throw new NotSupportedException("MongoDB repository is not supported.");
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		ConnectionString = MongoDBSupport.GetConnectionStringWihoutPassword(databaseRow.UserProvidedConnectionString);
		if (databaseRow.UserProvidedConnectionString == databaseRow.Password)
		{
			string value = new SimpleAES().EncryptToString(MongoDBSupport.GetPasswordFromConnectionString(databaseRow.UserProvidedConnectionString));
			Password = new XmlTextObject(value, isEncrypted: true);
		}
		else
		{
			Password = new XmlTextObject(databaseRow.PasswordEncrypted, isEncrypted: true);
		}
		Database = ((databaseRow.UseDifferentSchema == true) ? null : new DatabaseModel(databaseRow.Name, databaseRow.HasMultipleSchemas == true));
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		databaseRow.Host = MongoDBSupport.GetHostFromConnectionString(ConnectionString);
		databaseRow.Port = MongoDBSupport.GetPortFromConnectionString(ConnectionString);
		databaseRow.User = MongoDBSupport.GetLoginFromConnectionString(ConnectionString);
		SetPassword(databaseRow, Password);
		DatabaseModel database = Database;
		databaseRow.HasMultipleSchemas = ((database != null) ? new bool?(database.IsMultiple) : databaseRow.UseDifferentSchema);
		databaseRow.UseDifferentSchema = databaseRow.HasMultipleSchemas;
		databaseRow.Name = Database?.Name;
		databaseRow.UseDifferentSchema = string.IsNullOrEmpty(Database?.Name);
		databaseRow.MongoDBIsSrv = MongoDBSupport.GetSrvFromConnectionString(ConnectionString);
		databaseRow.UserProvidedConnectionString = MongoDBSupport.GetConnectionStringWithPassword(ConnectionString, databaseRow.Password);
		databaseRow.GeneralConnectionType = GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString;
		Timeout = null;
		databaseRow.Type = GetConnectionCommandType().AsDatabaseType();
		return true;
	}
}
