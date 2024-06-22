using System.Reflection;
using System.Xml.Serialization;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Enums.Extensions;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class HiveMetastoreSQLServerConnection : ConnectionBase
{
	[XmlElement]
	public override int? Timeout { get; set; }

	[XmlElement]
	public string Host { get; set; }

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
	public string Param1 { get; set; }

	[XmlElement]
	public string Param2 { get; set; }

	[XmlElement]
	public string Param3 { get; set; }

	[XmlElement]
	public string Param4 { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.HiveMetastoreSQLServer;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		string password = string.Empty;
		if (Password != null && Password.Value != null)
		{
			password = ((!Password.IsEncrypted) ? Password.Value : new SimpleAES().DecryptString(Password.Value));
		}
		return Dataedo.App.DatabasesSupport.Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, GetDatabase(), GetConnectionCommandType(), Host, Login, password, Dataedo.Shared.Enums.AuthenticationType.StringToType(AuthenticationType), Port, null, withPooling, withDatabase: true, isUnicode: false, isDirectConnect: false, encrypt, trustServerCertificate, null, null, null, null, useStandardConnectionString: false, isServiceName: true, null, isSrv: false, null, null, null, null, null, null, null, Param1);
	}

	public override string GetHost()
	{
		return Host;
	}

	public override string GetDatabase()
	{
		return Param2;
	}

	public override string GetDatabaseFull()
	{
		return Param3 + "." + Param4 + "@HiveMetastore";
	}

	public override bool? GetIsWindowsAuthentication()
	{
		return Authentication.IsWindowsAuthentication(AuthenticationType);
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
		Param2 = loginInfo.DataedoDatabase;
		AuthenticationType = loginInfo.AuthenticationType;
		Port = PrepareValue.ToInt(loginInfo.Port);
		if (!Authentication.IsWindowsAuthentication(AuthenticationType))
		{
			Login = loginInfo.DataedoLogin;
			Password = new XmlTextObject(loginInfo.DataedoPasswordEncrypted, isEncrypted: true);
		}
		ConnectionMode = loginInfo.DataedoConnectionMode;
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		Host = databaseRow.Host;
		Param2 = databaseRow.Param2;
		Param3 = databaseRow.Param3;
		Param4 = databaseRow.Param4;
		AuthenticationType = Dataedo.Shared.Enums.AuthenticationType.TypeToString(databaseRow.SelectedAuthenticationType);
		Port = databaseRow.Port;
		if (!Authentication.IsWindowsAuthentication(AuthenticationType))
		{
			Login = databaseRow.User;
			Password = new XmlTextObject(databaseRow.PasswordEncrypted, isEncrypted: true);
		}
		Param1 = databaseRow.Param1;
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		databaseRow.Host = Host;
		Param2 = databaseRow.Param2;
		Param3 = databaseRow.Param3;
		Param4 = databaseRow.Param4;
		databaseRow.SelectedAuthenticationType = Dataedo.Shared.Enums.AuthenticationType.StringToType(AuthenticationType);
		databaseRow.WindowsAutentication = databaseRow.SelectedAuthenticationType == Dataedo.Shared.Enums.AuthenticationType.AuthenticationTypeEnum.WindowsAuthentication;
		databaseRow.User = Login;
		if (!databaseRow.WindowsAutentication)
		{
			SetPassword(databaseRow, Password);
		}
		databaseRow.Param1 = Param1 ?? SqlServerConnectionModeEnum.DefaultModeString;
		databaseRow.Type = GetConnectionCommandType().AsDatabaseType();
		return true;
	}
}
