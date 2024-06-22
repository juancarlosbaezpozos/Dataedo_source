using System;
using System.Reflection;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Enums.Extensions;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class SalesforceConnection : ConnectionBase
{
	[XmlElement]
	public override int? Timeout { get; set; }

	[XmlElement]
	public string Host { get; set; }

	[XmlElement]
	public string Database { get; set; }

	[XmlElement]
	public string AuthenticationType { get; set; }

	[XmlElement]
	public string User { get; set; }

	[XmlElement]
	public XmlTextObject Password { get; set; }

	[XmlElement]
	public XmlTextObject ConsumerKey { get; set; }

	[XmlElement]
	public XmlTextObject ConsumerSecret { get; set; }

	[XmlElement]
	public int? Port { get; set; }

	[XmlElement]
	public string ConnectionType { get; set; }

	[XmlElement]
	public string IsSandboxInstance { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.Salesforce;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		string password = string.Empty;
		if (Password != null && Password.Value != null)
		{
			password = ((!Password.IsEncrypted) ? Password.Value : new SimpleAES().DecryptString(Password.Value));
		}
		return Dataedo.App.DatabasesSupport.Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, Database, GetConnectionCommandType(), Host, User, password, Dataedo.Shared.Enums.AuthenticationType.AuthenticationTypeEnum.StandardAuthentication, Port, null, withPooling);
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
		throw new NotSupportedException("Salesforce repository is not supported.");
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		Host = databaseRow.Host;
		Database = databaseRow.Name;
		if (databaseRow.Param3 != SalesforceConnectionTypeEnum.TypeToString(SalesforceConnectionTypeEnum.SalesforceConnectionType.Interactive))
		{
			User = databaseRow.User;
			Password = new XmlTextObject(databaseRow.PasswordEncrypted, isEncrypted: true);
			ConsumerKey = new XmlTextObject(databaseRow.Param1, isEncrypted: true);
			ConsumerSecret = new XmlTextObject(databaseRow.Param2, isEncrypted: true);
		}
		ConnectionType = databaseRow.Param3;
		IsSandboxInstance = databaseRow.Param4;
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		databaseRow.Host = Host;
		databaseRow.Name = Database;
		if (ConnectionType != SalesforceConnectionTypeEnum.TypeToString(SalesforceConnectionTypeEnum.SalesforceConnectionType.Interactive))
		{
			databaseRow.User = User;
			SetPassword(databaseRow, Password);
			databaseRow.Param1 = ConsumerKey.Value;
			databaseRow.Param2 = ConsumerSecret.Value;
		}
		databaseRow.Type = GetConnectionCommandType().AsDatabaseType();
		databaseRow.Param3 = ConnectionType;
		databaseRow.Param4 = IsSandboxInstance;
		return true;
	}
}
