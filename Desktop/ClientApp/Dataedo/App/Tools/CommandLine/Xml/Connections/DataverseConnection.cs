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

public class DataverseConnection : ConnectionBase
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
	public string Login { get; set; }

	[XmlElement]
	public XmlTextObject Password { get; set; }

	[XmlElement]
	public int? Port { get; set; }

	[XmlElement]
	public string ConnectionMode { get; set; }

	[XmlElement]
	public string AppId { get; set; }

	[XmlElement]
	public string RedirectUri { get; set; }

	[XmlElement]
	public string LanguageSetting { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.Dataverse;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		string password = string.Empty;
		if (Password != null && Password.Value != null)
		{
			password = ((!Password.IsEncrypted) ? Password.Value : new SimpleAES().DecryptString(Password.Value));
		}
		return Dataedo.App.DatabasesSupport.Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, Database, GetConnectionCommandType(), Host, Login, password, Dataedo.Shared.Enums.AuthenticationType.StringToType(AuthenticationType), Port, null, withPooling, withDatabase: true, isUnicode: false, isDirectConnect: false, encrypt, trustServerCertificate, null, null, null, null, useStandardConnectionString: false, isServiceName: true, null, isSrv: false, null, null, null, null, null, null, null, null, LanguageSetting, AppId, RedirectUri);
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
		return Host;
	}

	public override string GetLogin()
	{
		return Login;
	}

	public override string GetPassword()
	{
		return Password?.Value;
	}

	public override string GetConnectionMode()
	{
		return ConnectionMode;
	}

	public override bool? GetIsWindowsAuthentication()
	{
		return null;
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
		throw new NotSupportedException("Dataverse repository is not supported.");
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		Host = databaseRow.Host;
		Database = databaseRow.Name;
		Login = databaseRow.User;
		Password = new XmlTextObject(databaseRow.PasswordEncrypted, isEncrypted: true);
		LanguageSetting = databaseRow.Param2;
		AppId = databaseRow.Param3;
		RedirectUri = databaseRow.Param4;
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		databaseRow.Host = Host;
		databaseRow.Name = Database;
		databaseRow.User = Login;
		databaseRow.Type = GetConnectionCommandType().AsDatabaseType();
		databaseRow.Param2 = LanguageSetting;
		databaseRow.Param3 = AppId;
		databaseRow.Param4 = RedirectUri;
		return true;
	}
}
