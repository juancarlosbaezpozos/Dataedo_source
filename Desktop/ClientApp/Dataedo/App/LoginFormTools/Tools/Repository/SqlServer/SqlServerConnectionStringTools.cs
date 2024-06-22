using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Forms.Tools;
using Dataedo.App.LoginFormTools.Tools.Repository.RepositoryCreator;
using Dataedo.App.LoginFormTools.UserControls.Subcontrols.Interfaces;
using Dataedo.Data.Commands.Enums;
using Dataedo.LicenseHelperLibrary.Repository;
using Dataedo.Shared.Enums;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.LoginFormTools.Tools.Repository.SqlServer;

internal static class SqlServerConnectionStringTools
{
	internal static string GetConnectionStringForConnection(string database, DatabaseType repositoryType, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, SqlServerConnectionModeEnum.SqlServerConnectionMode? sqlServerConnectionMode, out RepositoryExistsStatusEnum? connectionResult, out Exception exception, bool showMessages)
	{
		string identifierName = null;
		sqlServerConnectionMode = sqlServerConnectionMode ?? SqlServerConnectionModeEnum.DefaultMode;
		connectionResult = null;
		exception = null;
		string text = null;
		switch (sqlServerConnectionMode)
		{
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.ForceEncryptionRequireTrustedCertificate:
			text = Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, database, repositoryType, host, login, password, authenticationType, port, identifierName, withPooling: true, withDatabase: true, isUnicode: false, isDirectConnect: false, encrypt: true);
			break;
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.ForceEncryptionTrustServerCertificate:
			text = Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, database, repositoryType, host, login, password, authenticationType, port, identifierName, withPooling: true, withDatabase: true, isUnicode: false, isDirectConnect: false, encrypt: true, trustServerCertificate: true);
			break;
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.EncryptConnectionIfPossible:
			text = Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, database, repositoryType, host, login, password, authenticationType, port, identifierName, withPooling: true, withDatabase: true, isUnicode: false, isDirectConnect: false, encrypt: true, trustServerCertificate: true);
			connectionResult = LoginHelper.CheckIfRepositoryExists(text, database, ref exception, showMessages);
			if (connectionResult == RepositoryExistsStatusEnum.Error)
			{
				text = Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, database, repositoryType, host, login, password, authenticationType, port, identifierName, withPooling: true, withDatabase: true, isUnicode: false, isDirectConnect: false, encrypt: false, trustServerCertificate: true);
				connectionResult = LoginHelper.CheckIfRepositoryExists(text, database, ref exception, showMessages);
			}
			break;
		}
		return text;
	}

	internal static async Task<string> GetConnectionStringForConnectionAsync(string database, DatabaseType repositoryType, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, SqlServerConnectionModeEnum.SqlServerConnectionMode? sqlServerConnectionMode, bool showMessages, CancellationToken cancellationToken)
	{
		string serviceName = null;
		sqlServerConnectionMode = sqlServerConnectionMode ?? SqlServerConnectionModeEnum.DefaultMode;
		string newConnectionString = null;
		switch (sqlServerConnectionMode)
		{
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.ForceEncryptionRequireTrustedCertificate:
			newConnectionString = Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, database, repositoryType, host, login, password, authenticationType, port, serviceName, withPooling: true, withDatabase: true, isUnicode: false, isDirectConnect: false, encrypt: true);
			break;
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.ForceEncryptionTrustServerCertificate:
			newConnectionString = Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, database, repositoryType, host, login, password, authenticationType, port, serviceName, withPooling: true, withDatabase: true, isUnicode: false, isDirectConnect: false, encrypt: true, trustServerCertificate: true);
			break;
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.EncryptConnectionIfPossible:
			newConnectionString = Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, database, repositoryType, host, login, password, authenticationType, port, serviceName, withPooling: true, withDatabase: true, isUnicode: false, isDirectConnect: false, encrypt: true, trustServerCertificate: true);
			if (new RepositoryExistsStatusEnum?(await LoginHelper.CheckIfRepositoryExistsAsync(newConnectionString, database, showMessages, cancellationToken)) == RepositoryExistsStatusEnum.Error)
			{
				newConnectionString = Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, database, repositoryType, host, login, password, authenticationType, port, serviceName, withPooling: true, withDatabase: true, isUnicode: false, isDirectConnect: false, encrypt: false, trustServerCertificate: true);
				await LoginHelper.CheckIfRepositoryExistsAsync(newConnectionString, database, showMessages, cancellationToken);
			}
			break;
		}
		return newConnectionString;
	}

	internal static SqlConnectionStringBuilder GetConnectionStringBuilder(IConnectionData data, Form owner, bool withDatabase = true, bool? databaseExists = false)
	{
		SqlServerConnectionModeEnum.SqlServerConnectionMode? sqlServerConnectionMode = data.SqlServerConnectionMode;
		if (data.AuthenticationTypeString == "ACTIVE_DIRECTORY_INTERACTIVE")
		{
			withDatabase = true;
		}
		SqlConnectionStringBuilder sqlServerConnectionStringBuilder = LicenseHelper.GetSqlServerConnectionStringBuilder(Assembly.GetExecutingAssembly().GetName().Name, data.Database, data.ServerName, data.Login, data.Password, data.Port, data.AuthenticationTypeString, withPooling: true, withDatabase);
		switch (sqlServerConnectionMode)
		{
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.ForceEncryptionRequireTrustedCertificate:
			sqlServerConnectionStringBuilder = LicenseHelper.GetSqlServerConnectionStringBuilder(Assembly.GetExecutingAssembly().GetName().Name, data.Database, data.ServerName, data.Login, data.Password, data.Port, data.AuthenticationTypeString, withPooling: true, encrypt: true, trustServerCertificate: false, withDatabase);
			break;
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.ForceEncryptionTrustServerCertificate:
			sqlServerConnectionStringBuilder = LicenseHelper.GetSqlServerConnectionStringBuilder(Assembly.GetExecutingAssembly().GetName().Name, data.Database, data.ServerName, data.Login, data.Password, data.Port, data.AuthenticationTypeString, withPooling: true, encrypt: true, trustServerCertificate: true, withDatabase);
			break;
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.EncryptConnectionIfPossible:
			sqlServerConnectionStringBuilder = LicenseHelper.GetSqlServerConnectionStringBuilder(Assembly.GetExecutingAssembly().GetName().Name, data.Database, data.ServerName, data.Login, data.Password, data.Port, data.AuthenticationTypeString, withPooling: true, encrypt: true, trustServerCertificate: true, withDatabase);
			if (databaseExists != false && !DatabaseHelper.TryConnection(sqlServerConnectionStringBuilder.ConnectionString, showException: false, owner))
			{
				sqlServerConnectionStringBuilder = LicenseHelper.GetSqlServerConnectionStringBuilder(Assembly.GetExecutingAssembly().GetName().Name, data.Database, data.ServerName, data.Login, data.Password, data.Port, data.AuthenticationTypeString, withPooling: true, encrypt: false, trustServerCertificate: true, withDatabase);
			}
			break;
		}
		return sqlServerConnectionStringBuilder;
	}
}
