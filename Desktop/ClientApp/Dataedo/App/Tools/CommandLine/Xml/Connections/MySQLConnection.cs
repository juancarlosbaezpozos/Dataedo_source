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
using Devart.Data.Universal;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class MySQLConnection : ConnectionBase
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
	public string KeyPath { get; set; }

	[XmlElement]
	public string CertPath { get; set; }

	[XmlElement]
	public string CAPath { get; set; }

	[XmlElement]
	public string Cipher { get; set; }

	[XmlElement]
	public string TlsVersion { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.MySql;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		try
		{
			string mySQLConnectionString = GetMySQLConnectionString(useSSLConnectionString: false);
			using UniConnection uniConnection = new UniConnection(mySQLConnectionString);
			uniConnection.Open();
			return mySQLConnectionString;
		}
		catch (Exception)
		{
			try
			{
				string mySQLConnectionString = GetMySQLConnectionString(useSSLConnectionString: true);
				using UniConnection uniConnection2 = new UniConnection(mySQLConnectionString);
				uniConnection2.Open();
				return mySQLConnectionString;
			}
			catch
			{
				return GetMySQLConnectionString(useSSLConnectionString: true, useStandardConnection: true);
			}
		}
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
		throw new NotSupportedException("MySQL repository is not supported.");
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		Host = databaseRow.Host;
		Port = databaseRow.Port;
		User = databaseRow.User;
		Password = new XmlTextObject(databaseRow.PasswordEncrypted, isEncrypted: true);
		Database = databaseRow.Name;
		KeyPath = databaseRow?.SSLSettings?.KeyPath;
		CertPath = databaseRow?.SSLSettings?.CertPath;
		CAPath = databaseRow?.SSLSettings?.CAPath;
		Cipher = databaseRow?.SSLSettings?.Cipher;
		TlsVersion = databaseRow?.Param1;
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		databaseRow.Host = Host;
		databaseRow.Port = Port;
		databaseRow.User = User;
		SetPassword(databaseRow, Password);
		databaseRow.Name = Database;
		databaseRow.SSLSettings = new SSLSettings
		{
			KeyPath = KeyPath,
			CertPath = CertPath,
			CAPath = CAPath,
			Cipher = Cipher
		};
		databaseRow.Type = GetConnectionCommandType().AsDatabaseType();
		return true;
	}

	private string GetMySQLConnectionString(bool useSSLConnectionString, bool useStandardConnection = false, bool useDatabase = true)
	{
		string password = string.Empty;
		if (Password != null && Password.Value != null)
		{
			password = ((!Password.IsEncrypted) ? Password.Value : new SimpleAES().DecryptString(Password.Value));
		}
		string keyPath = (useSSLConnectionString ? null : KeyPath);
		string certPath = (useSSLConnectionString ? null : CertPath);
		string caPath = (useSSLConnectionString ? null : CAPath);
		string cipher = (useSSLConnectionString ? null : Cipher);
		return Dataedo.App.DatabasesSupport.Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, Database, GetConnectionCommandType(), Host, User, password, AuthenticationType.AuthenticationTypeEnum.StandardAuthentication, Port, null, withPooling: false, useDatabase, isUnicode: false, isDirectConnect: false, encrypt: false, trustServerCertificate: false, keyPath, certPath, caPath, cipher, useStandardConnection, isServiceName: true, null, isSrv: false, null, null, null, null, null, null, null, TlsVersion);
	}
}
