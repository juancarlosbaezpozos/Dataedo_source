using System;
using System.Reflection;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Enums.Extensions;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ImportCommand.Connections;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class MongoDBConnectionValues : ConnectionBase
{
	[XmlElement]
	public override int? Timeout { get; set; }

	[XmlElement]
	public string Host { get; set; }

	[XmlElement]
	public string ServiceName { get; set; }

	[XmlElement]
	public string User { get; set; }

	[XmlElement]
	public XmlTextObject Password { get; set; }

	[XmlElement]
	public DatabaseModel Database { get; set; }

	[XmlElement]
	public bool IsSrv { get; set; }

	[XmlElement]
	public string AuthenticationDatabase { get; set; }

	[XmlElement]
	public string ReplicaSet { get; set; }

	[XmlElement]
	public string MultiHost { get; set; }

	[XmlElement]
	public string SSLType { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.MongoDB;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		string text = string.Empty;
		if (Password != null && Password.Value != null)
		{
			text = ((!Password.IsEncrypted) ? Password.Value : new SimpleAES().DecryptString(Password.Value));
		}
		return Dataedo.App.DatabasesSupport.Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, Database?.Name, GetConnectionCommandType(), Host, User, text, AuthenticationType.AuthenticationTypeEnum.StandardAuthentication, null, null, withPooling, withDatabase: true, isUnicode: false, isDirectConnect: false, encrypt: false, trustServerCertificate: false, null, null, null, null, useStandardConnectionString: false, isServiceName: true, null, isSrv: false, null, multiHost: MultiHost, replicaSet: ReplicaSet, SSLType: SSLType);
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
		throw new NotSupportedException("MongoDB repository is not supported.");
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		Host = databaseRow.Host;
		User = databaseRow.User;
		Password = new XmlTextObject(databaseRow.PasswordEncrypted, isEncrypted: true);
		Database = ((databaseRow.UseDifferentSchema == true) ? null : new DatabaseModel(databaseRow.Name, databaseRow.HasMultipleSchemas == true));
		IsSrv = databaseRow.MongoDBIsSrv;
		AuthenticationDatabase = databaseRow.MongoDBAuthenticationDatabase;
		ReplicaSet = databaseRow.MongoDBReplicaSet;
		MultiHost = databaseRow.MultiHost;
		SSLType = databaseRow.SSLType;
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		databaseRow.Host = Host;
		databaseRow.User = User;
		SetPassword(databaseRow, Password);
		DatabaseModel database = Database;
		databaseRow.HasMultipleSchemas = ((database != null) ? new bool?(database.IsMultiple) : databaseRow.UseDifferentSchema);
		databaseRow.UseDifferentSchema = databaseRow.HasMultipleSchemas;
		databaseRow.Name = Database?.Name;
		databaseRow.UseDifferentSchema = string.IsNullOrEmpty(Database?.Name);
		databaseRow.MongoDBIsSrv = IsSrv;
		databaseRow.MongoDBAuthenticationDatabase = AuthenticationDatabase;
		databaseRow.MongoDBReplicaSet = ReplicaSet;
		databaseRow.MultiHost = MultiHost;
		databaseRow.SSLType = SSLType;
		databaseRow.GeneralConnectionType = GeneralConnectionTypeEnum.GeneralConnectionType.Values;
		databaseRow.Type = GetConnectionCommandType().AsDatabaseType();
		return true;
	}
}
