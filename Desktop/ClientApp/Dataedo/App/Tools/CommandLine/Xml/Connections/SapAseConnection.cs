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

public class SapAseConnection : ConnectionBase
{
	[XmlElement]
	public string Host { get; set; }

	[XmlElement]
	public string ServiceName { get; set; }

	[XmlElement]
	public int? Port { get; set; }

	[XmlElement]
	public string User { get; set; }

	[XmlElement]
	public XmlTextObject Password { get; set; }

	[XmlElement]
	public string Database { get; set; }

	[XmlElement]
	public string Charset { get; set; }

	[XmlElement]
	public override int? Timeout { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.SapAse;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		string password = string.Empty;
		if (Password != null && Password.Value != null)
		{
			password = ((!Password.IsEncrypted) ? Password.Value : new SimpleAES().DecryptString(Password.Value));
		}
		return Dataedo.App.DatabasesSupport.Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, Database, GetConnectionCommandType(), Host, User, password, AuthenticationType.AuthenticationTypeEnum.StandardAuthentication, Port, ServiceName, withPooling, withDatabase: true, isUnicode: false, isDirectConnect: false, encrypt: false, trustServerCertificate: false, null, null, null, null, useStandardConnectionString: false, isServiceName: true, null, isSrv: false, null, null, null, null, null, null, Charset);
	}

	public override string GetHost()
	{
		return Host;
	}

	public override string GetDatabase()
	{
		return Database;
	}

	public override string GetDatabaseFull()
	{
		return Database + "@" + Host;
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

	public string GetCharset()
	{
		return Charset;
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
		throw new NotSupportedException("SAP ASE repository is not supported.");
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		Host = databaseRow.Host;
		ServiceName = databaseRow.ServiceName;
		Port = databaseRow.Port;
		User = databaseRow.User;
		Password = new XmlTextObject(databaseRow.PasswordEncrypted, isEncrypted: true);
		Database = databaseRow.Name;
		Charset = databaseRow.Param1;
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		databaseRow.Host = Host;
		databaseRow.ServiceName = ServiceName;
		databaseRow.Port = Port;
		databaseRow.User = User;
		SetPassword(databaseRow, Password);
		databaseRow.Name = Database;
		databaseRow.Type = GetConnectionCommandType().AsDatabaseType();
		databaseRow.Param1 = Charset;
		return true;
	}
}
