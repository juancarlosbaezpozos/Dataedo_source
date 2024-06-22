using System;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class InterfaceTablesConnection : SqlServerConnection
{
	[XmlElement]
	public string DatabaseNameToImport { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.InterfaceTables;
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
		throw new NotSupportedException("Interface Tables repository is not supported.");
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		base.SetConnection(databaseRow);
		DatabaseNameToImport = databaseRow.Param1;
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		base.SetDataInDatabaseRow(databaseRow);
		databaseRow.Param1 = DatabaseNameToImport;
		return true;
	}
}
