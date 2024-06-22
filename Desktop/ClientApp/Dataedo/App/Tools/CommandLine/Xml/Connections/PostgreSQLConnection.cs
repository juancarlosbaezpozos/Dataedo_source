using System;
using System.Reflection;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums.Extensions;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class PostgreSQLConnection : ConnectionBase
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
	public string Database { get; set; }

	[XmlElement]
	public string SSLType { get; set; }

	[XmlElement]
	public string CertPath { get; set; }

	[XmlElement]
	public string CertPassword { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.PostgreSQL;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		string password = string.Empty;
		if (Password != null && Password.Value != null)
		{
			password = ((!Password.IsEncrypted) ? Password.Value : new SimpleAES().DecryptString(Password.Value));
		}
		return Dataedo.App.DatabasesSupport.Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, Database, GetConnectionCommandType(), Host, User, password, AuthenticationType.AuthenticationTypeEnum.StandardAuthentication, Port, null, withPooling, withDatabase: true, isUnicode: false, isDirectConnect: false, encrypt: false, trustServerCertificate: false, null, CertPath, null, null, useStandardConnectionString: false, isServiceName: true, null, isSrv: false, null, null, null, SSLType, null, null, null, CertPassword);
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

	public override void SetConnection(LoginInfo loginInfo)
	{
		throw new NotSupportedException("PostgreSQL repository is not supported.");
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		Host = databaseRow.Host;
		Port = databaseRow.Port;
		User = databaseRow.User;
		Password = new XmlTextObject(databaseRow.PasswordEncrypted, isEncrypted: true);
		Database = databaseRow.Name;
		SSLType = databaseRow.SSLType;
		CertPath = ((databaseRow.SSLSettings == null) ? null : databaseRow.SSLSettings.CertPath);
		CertPassword = ((databaseRow.SSLSettings == null) ? null : databaseRow.SSLSettings.CertPassword);
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		SSLSettings sSLSettings = new SSLSettings
		{
			CertPath = CertPath,
			CertPassword = CertPassword
		};
		string text2 = (databaseRow.Host = (Host = Host));
		databaseRow.Port = Port;
		databaseRow.User = User;
		SetPassword(databaseRow, Password);
		databaseRow.Name = Database;
		databaseRow.SSLType = SSLType;
		databaseRow.Type = GetConnectionCommandType().AsDatabaseType();
		databaseRow.SSLSettings = sSLSettings;
		return true;
	}
}
