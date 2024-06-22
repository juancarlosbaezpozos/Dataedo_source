using System.Reflection;
using System.Xml.Serialization;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums.Extensions;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class SsasTabularConnection : ConnectionBase
{
	[XmlElement]
	public override int? Timeout { get; set; }

	[XmlElement]
	public string Host { get; set; }

	[XmlElement]
	public string Database { get; set; }

	[XmlElement]
	public string Login { get; set; }

	[XmlElement]
	public XmlTextObject Password { get; set; }

	[XmlElement]
	public string AuthenticationType { get; set; }

	[XmlElement]
	public string ConnectionMode { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.SsasTabular;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		string password = string.Empty;
		if (Password != null && Password.Value != null)
		{
			password = ((!Password.IsEncrypted) ? Password.Value : new SimpleAES().DecryptString(Password.Value));
		}
		return Dataedo.App.DatabasesSupport.Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, Database, GetConnectionCommandType(), Host, Login, password, Dataedo.Shared.Enums.AuthenticationType.StringToType(AuthenticationType), null, null, withPooling, withDatabase: true, isUnicode: false, isDirectConnect: false, encrypt, trustServerCertificate);
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
		return AuthenticationSSAS.IsWindowsAuthentication(AuthenticationType);
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

	public override void SetConnection(LoginInfo loginInfo)
	{
		Host = loginInfo.DataedoHost;
		Database = loginInfo.DataedoDatabase;
		if (!AuthenticationSSAS.IsWindowsAuthentication(AuthenticationType))
		{
			Login = loginInfo.DataedoLogin;
			Password = new XmlTextObject(loginInfo.DataedoPasswordEncrypted, isEncrypted: true);
		}
		ConnectionMode = loginInfo.DataedoConnectionMode;
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		Host = databaseRow.Host;
		Database = databaseRow.Name;
		AuthenticationType = Dataedo.Shared.Enums.AuthenticationType.TypeToString(databaseRow.SelectedAuthenticationType);
		if (!AuthenticationSSAS.IsWindowsAuthentication(AuthenticationType))
		{
			Login = databaseRow.User;
			Password = new XmlTextObject(databaseRow.PasswordEncrypted, isEncrypted: true);
		}
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		databaseRow.Host = Host;
		databaseRow.Name = Database;
		databaseRow.User = Login;
		databaseRow.SelectedAuthenticationType = Dataedo.Shared.Enums.AuthenticationType.StringToType(AuthenticationType);
		databaseRow.WindowsAutentication = databaseRow.SelectedAuthenticationType == Dataedo.Shared.Enums.AuthenticationType.AuthenticationTypeEnum.WindowsAuthentication;
		if (!databaseRow.WindowsAutentication)
		{
			SetPassword(databaseRow, Password);
		}
		databaseRow.Type = GetConnectionCommandType().AsDatabaseType();
		return true;
	}
}
