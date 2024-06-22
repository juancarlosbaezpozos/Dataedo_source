using System;
using System.Linq;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Enums.Extensions;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class GoogleBigQueryConnection : ConnectionBase
{
	[XmlElement]
	public string Login { get; set; }

	[XmlElement]
	public string ServiceName { get; set; }

	[XmlElement]
	public string Host { get; set; }

	[XmlElement]
	public string Database { get; set; }

	[XmlElement]
	public override int? Timeout { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.GoogleBigQuery;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		return Login;
	}

	public override string GetDatabase()
	{
		return Database;
	}

	public override string GetDatabaseFull()
	{
		return Database;
	}

	public override string GetHost()
	{
		return null;
	}

	public override bool? GetIsWindowsAuthentication()
	{
		return null;
	}

	public override string GetLogin()
	{
		return Login;
	}

	public override string GetPassword()
	{
		throw new NotSupportedException();
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
		throw new NotSupportedException("Google BigQuery repository is not supported.");
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		Login = databaseRow.User;
		Host = databaseRow.Host;
		Database = databaseRow.Name;
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		string text2 = (databaseRow.User = (Login = Login));
		databaseRow.Host = Host;
		databaseRow.Name = Database;
		databaseRow.HasMultipleSchemas = Database.Split(new string[1] { "," }, StringSplitOptions.None).ToList().Count() > 1;
		databaseRow.Schemas = Database.Split(new string[1] { "," }, StringSplitOptions.None).ToList();
		databaseRow.Type = GetConnectionCommandType().AsDatabaseType();
		return true;
	}
}
