using System;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class DdlConnection : ConnectionBase
{
	[XmlElement]
	public override int? Timeout { get; set; }

	[XmlElement]
	public string FileToImport { get; set; }

	[XmlElement]
	public string ScriptLanguage { get; set; }

	[XmlElement]
	public string Database { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.DdlScript;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		return string.Empty;
	}

	public override string GetHost()
	{
		return string.Empty;
	}

	public override string GetDatabase()
	{
		return string.Empty;
	}

	public override string GetDatabaseFull()
	{
		return Database;
	}

	public override bool? GetIsWindowsAuthentication()
	{
		return null;
	}

	public override string GetLogin()
	{
		return string.Empty;
	}

	public override string GetPassword()
	{
		return string.Empty;
	}

	public override string GetConnectionMode()
	{
		return string.Empty;
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
		throw new NotSupportedException("Ddl script repository is not supported.");
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		Database = databaseRow.Name;
		FileToImport = databaseRow.Param1;
		ScriptLanguage = databaseRow.Param2;
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		databaseRow.Name = Database;
		databaseRow.Param1 = FileToImport;
		databaseRow.Param2 = ScriptLanguage;
		return true;
	}
}
