using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public abstract class ConnectionBase
{
	[XmlIgnore]
	public abstract int? Timeout { get; set; }

	public abstract DatabaseType GetConnectionCommandType();

	public abstract string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate);

	public abstract string GetHost();

	public abstract string GetDatabase();

	public abstract string GetDatabaseFull();

	public abstract bool? GetIsWindowsAuthentication();

	public abstract string GetLogin();

	public abstract string GetPassword();

	public virtual string GetConnectionMode()
	{
		return string.Empty;
	}

	public abstract void SetConnection(LoginInfo loginInfo);

	public abstract void SetConnection(DatabaseRow databaseRow);

	public abstract bool SetDataInDatabaseRow(DatabaseRow databaseRow);

	protected void SetPassword(DatabaseRow databaseRow, XmlTextObject password)
	{
		if (password != null)
		{
			if (password.IsEncrypted)
			{
				databaseRow.PasswordEncrypted = password.Value;
			}
			else
			{
				databaseRow.Password = password.Value;
			}
		}
		else
		{
			databaseRow.Password = null;
		}
	}
}
