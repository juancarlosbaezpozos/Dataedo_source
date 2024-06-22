using System;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Enums.Extensions;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class DynamoDBConnection : ConnectionBase
{
	[XmlElement]
	public override int? Timeout { get; set; }

	[XmlElement]
	public string AccessKey { get; set; }

	[XmlElement]
	public XmlTextObject SecretKey { get; set; }

	[XmlElement]
	public string SampleSize { get; set; }

	[XmlElement]
	public string Database { get; set; }

	[XmlElement]
	public string Host { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.DynamoDB;
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
		return Database + "@DynamoDB";
	}

	public override string GetHost()
	{
		return Host;
	}

	public override bool? GetIsWindowsAuthentication()
	{
		throw new NotImplementedException();
	}

	public override string GetLogin()
	{
		return AccessKey;
	}

	public override string GetPassword()
	{
		return SecretKey.Value;
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
		throw new NotSupportedException();
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		Host = databaseRow.Host;
		Database = databaseRow.Name;
		AccessKey = databaseRow.User;
		SecretKey = new XmlTextObject(databaseRow.PasswordEncrypted, isEncrypted: true);
		SampleSize = databaseRow.Param1;
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		databaseRow.Host = Host;
		databaseRow.Name = Database;
		databaseRow.User = AccessKey;
		SetPassword(databaseRow, SecretKey);
		databaseRow.Type = GetConnectionCommandType().AsDatabaseType();
		databaseRow.Param1 = SampleSize;
		return true;
	}
}
