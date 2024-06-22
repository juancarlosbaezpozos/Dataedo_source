using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Enums;
using Dataedo.App.LoginFormTools.Tools.Repository.RepositoryCreator.Data;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;
using Dataedo.Data.Factories;
using Dataedo.LicenseHelperLibrary.Repository;
using Dataedo.Model.Data.Repository;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.LoginFormTools.Tools.Repository.RepositoryCreator;

public static class DatabaseHelper
{
	public static string GetSQLServerProductVersion(string connectionString)
	{
		try
		{
			using SqlConnection sqlConnection = new SqlConnection(connectionString);
			sqlConnection.Open();
			using SqlCommand sqlCommand = CommandsWithTimeout.SqlServerForRepository("SELECT SERVERPROPERTY('ProductVersion') AS version", sqlConnection);
			SqlDataReader sqlDataReader = sqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
			return sqlDataReader.Read() ? sqlDataReader["version"].ToString() : null;
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to get SQL Server version", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message);
			return null;
		}
	}

	public static string GetServerEdition(string connectionString)
	{
		try
		{
			string cmdText = "SELECT SERVERPROPERTY('Edition') AS edition;";
			using SqlConnection sqlConnection = new SqlConnection(connectionString);
			sqlConnection.Open();
			SqlDataReader sqlDataReader = new SqlCommand(cmdText, sqlConnection).ExecuteReader(CommandBehavior.CloseConnection);
			if (sqlDataReader.Read())
			{
				return sqlDataReader.GetValue(0)?.ToString();
			}
			return null;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static bool InsertLiteLicense(string connectionString, string databaseName, string login)
	{
		return InsertLicense(ConfigurationFileHelper.LITE_KEY, connectionString, databaseName, login);
	}

	public static bool InsertLicense(string license, string connectionString, string databaseName, string login)
	{
		try
		{
			login = LicenseHelper.EscapeInvalidCharacters(login);
			using SqlConnection sqlConnection = new SqlConnection(connectionString);
			sqlConnection.Open();
			using SqlCommand sqlCommand = CommandsWithTimeout.SqlServerForRepository($"use [{databaseName}];insert into licenses([login], [key]) values('{login}', '{license}')", sqlConnection);
			if (sqlCommand.ExecuteNonQuery() < 1)
			{
				GeneralMessageBoxesHandling.Show("Unable to insert user", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to insert user", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message);
			return false;
		}
		return true;
	}

	public static Dataedo.App.LoginFormTools.Tools.Repository.RepositoryCreator.Data.RepositoryVersion GetRepositoryVersion(string connectionString, Form owner)
	{
		try
		{
			IOrderedEnumerable<RepositoryDetailsObject> orderedEnumerable = from x in CommandsFactory.GetCommands(DatabaseType.SqlServer, connectionString).Select.Repository.GetAllVersions()
				orderby x.Version descending, x.Update descending, x.Release descending
				select x;
			if (orderedEnumerable == null || orderedEnumerable.Count() == 0)
			{
				return null;
			}
			RepositoryDetailsObject repositoryDetailsObject = orderedEnumerable.FirstOrDefault();
			if (repositoryDetailsObject.Update.HasValue && repositoryDetailsObject.Stable.HasValue)
			{
				return new Dataedo.App.LoginFormTools.Tools.Repository.RepositoryCreator.Data.RepositoryVersion
				{
					Version = repositoryDetailsObject.Version,
					Update = repositoryDetailsObject.Update.Value,
					Release = repositoryDetailsObject.Release.GetValueOrDefault(),
					IsStable = repositoryDetailsObject.Stable.Value
				};
			}
			return null;
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to get Dataedo repository version", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, owner);
			return null;
		}
	}

	public static bool IsDatabaseEmpty(string connectionString, string database)
	{
		try
		{
			using SqlConnection sqlConnection = new SqlConnection(new SqlConnectionStringBuilder(connectionString)
			{
				InitialCatalog = database
			}.ConnectionString);
			sqlConnection.Open();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select\r\n                            (select count(1) from INFORMATION_SCHEMA.TABLES \r\n                                WHERE LOWER(TABLE_SCHEMA) != 'sys') +\r\n                            (select count(1) from INFORMATION_SCHEMA.VIEWS \r\n                                WHERE LOWER(TABLE_SCHEMA) != 'sys') +\r\n                            (select count(1) from INFORMATION_SCHEMA.ROUTINES)");
			using SqlCommand sqlCommand = CommandsWithTimeout.SqlServerForRepository(stringBuilder.ToString(), sqlConnection);
			return (int)sqlCommand.ExecuteScalar() == 0;
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to check if database is empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message);
			return false;
		}
	}

	public static bool TryConnection(string connectionString, bool showException, Form parentForm = null)
	{
		try
		{
			using (SqlConnection sqlConnection = new SqlConnection(connectionString))
			{
				sqlConnection.Open();
			}
			return true;
		}
		catch (Exception ex)
		{
			if (showException)
			{
				if (ex is SqlException ex2)
				{
					switch (ex2.Errors[0].Number)
					{
					case 53:
						GeneralMessageBoxesHandling.Show("Unable to connect to repository!\nCheck server name and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Errors[0].Message, 1, parentForm);
						break;
					case 4060:
						GeneralMessageBoxesHandling.Show("Unable to connect to repository!\nCheck repository name and try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Errors[0].Message, 1, parentForm);
						break;
					case 18452:
						GeneralMessageBoxesHandling.Show("Unable to connect to repository!\nCannot connect using windows credentials.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Errors[0].Message, 1, parentForm);
						break;
					case 18456:
						GeneralMessageBoxesHandling.Show("Unable to connect to repository!\nCheck login and password and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Errors[0].Message, 1, parentForm);
						break;
					default:
						GeneralMessageBoxesHandling.Show("Unable to connect to repository!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Errors[0].Message, 1, parentForm);
						break;
					}
				}
				else
				{
					GeneralMessageBoxesHandling.Show("Unable to connect to repository!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message);
				}
			}
			return false;
		}
	}

	public static RepositoryExistsStatusEnum CheckIfRepositoryExists(string connectionString, string database, ref Exception exception)
	{
		RepositoryExistsStatusEnum repositoryExistsStatusEnum = RepositoryExistsStatusEnum.Error;
		try
		{
			repositoryExistsStatusEnum = CheckIfDatabaseExists(connectionString, database);
		}
		catch (Exception ex)
		{
			Exception ex2 = (exception = ex);
			repositoryExistsStatusEnum = RepositoryExistsStatusEnum.Error;
		}
		try
		{
			if (repositoryExistsStatusEnum != RepositoryExistsStatusEnum.Exists)
			{
				repositoryExistsStatusEnum = CheckIfDatabaseExists(new SqlConnectionStringBuilder(connectionString)
				{
					InitialCatalog = string.Empty
				}.ConnectionString, database);
			}
			return repositoryExistsStatusEnum;
		}
		catch (Exception ex3)
		{
			Exception ex4 = (exception = ex3);
			return RepositoryExistsStatusEnum.Error;
		}
	}

	public static async Task<RepositoryExistsStatusEnum> CheckIfRepositoryExistsAsync(string connectionString, string database, CancellationToken cancellationToken)
	{
		RepositoryExistsStatusEnum repositoryExistsStatusEnum;
		try
		{
			repositoryExistsStatusEnum = await CheckIfDatabaseExistsAsync(connectionString, database, cancellationToken);
		}
		catch (Exception)
		{
			repositoryExistsStatusEnum = RepositoryExistsStatusEnum.Error;
		}
		try
		{
			if (repositoryExistsStatusEnum != RepositoryExistsStatusEnum.Exists)
			{
				repositoryExistsStatusEnum = await CheckIfDatabaseExistsAsync(new SqlConnectionStringBuilder(connectionString)
				{
					InitialCatalog = string.Empty
				}.ConnectionString, database, cancellationToken);
			}
			return repositoryExistsStatusEnum;
		}
		catch (Exception)
		{
			return RepositoryExistsStatusEnum.Error;
		}
	}

	public static RepositoryExistsStatusEnum CheckIfRepositoryExists(string connectionString, string database, bool checkIfDbOwner = true, string messageWhenNoDbOwnerRole = null, Form parentForm = null)
	{
		try
		{
			using SqlConnection sqlConnection = new SqlConnection(connectionString);
			sqlConnection.Open();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("SELECT count(*) FROM sys.databases WHERE [name] = '{0}'", LicenseHelper.EscapeInvalidCharacters(database));
			using (SqlCommand sqlCommand = CommandsWithTimeout.SqlServerForRepository(stringBuilder.ToString(), sqlConnection))
			{
				int num = (int)sqlCommand.ExecuteScalar();
				if (num == 0)
				{
					return RepositoryExistsStatusEnum.NotExists;
				}
				if (num < 0)
				{
					GeneralMessageBoxesHandling.Show("Unable to check if repository exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, parentForm);
					return RepositoryExistsStatusEnum.Error;
				}
			}
			SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectionString);
			sqlConnectionStringBuilder.InitialCatalog = database;
			if (checkIfDbOwner && !LicenseHelper.IsUserDbOwner(sqlConnectionStringBuilder.ConnectionString))
			{
				GeneralMessageBoxesHandling.Show(messageWhenNoDbOwnerRole, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return RepositoryExistsStatusEnum.Error;
			}
			return CheckIfRepositoryExists(sqlConnectionStringBuilder.ConnectionString, parentForm);
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to check if repository exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, parentForm);
			return RepositoryExistsStatusEnum.Error;
		}
	}

	private static RepositoryExistsStatusEnum CheckIfRepositoryExists(string connectionStringWithDatabase, Form owner)
	{
		try
		{
			using SqlConnection sqlConnection = new SqlConnection(connectionStringWithDatabase);
			sqlConnection.Open();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("select(\r\n                            count(COL_LENGTH('licenses','login')) +\r\n                            count(COL_LENGTH('licenses','host')) +\r\n                            count(COL_LENGTH('databases','title')) +\r\n                            count(COL_LENGTH('databases','type')) +\r\n                            count(COL_LENGTH('databases','name')))");
			using SqlCommand sqlCommand = CommandsWithTimeout.SqlServerForRepository(stringBuilder.ToString(), sqlConnection);
			int num = (int)sqlCommand.ExecuteScalar();
			if (num == 5)
			{
				return RepositoryExistsStatusEnum.Exists;
			}
			if (num < 0)
			{
				return RepositoryExistsStatusEnum.Error;
			}
			if (num > 0 && num < 5)
			{
				GeneralMessageBoxesHandling.Show("Can't create repository with this name, database already exists and is not empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
				return RepositoryExistsStatusEnum.Error;
			}
			return RepositoryExistsStatusEnum.ExistsEmpty;
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to check if repository exists", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, owner);
			return RepositoryExistsStatusEnum.Error;
		}
	}

	public static List<ConnectedUser> GetConnectedUsers(string connectionString, string databaseName, string host, string applicationName)
	{
		List<ConnectedUser> list = new List<ConnectedUser>();
		try
		{
			host = LicenseHelper.EscapeInvalidCharacters(host);
			applicationName = LicenseHelper.EscapeInvalidCharacters(applicationName);
			using SqlConnection sqlConnection = new SqlConnection(connectionString);
			sqlConnection.Open();
			using SqlCommand sqlCommand = CommandsWithTimeout.SqlServerForRepository(new StringBuilder().AppendFormat("\r\n                        DECLARE @Table TABLE\r\n                        (\r\n                            SPID INT,\r\n                            Status VARCHAR(1000) NULL,\r\n                            Login SYSNAME NULL,\r\n                            HostName SYSNAME NULL,\r\n                            BlkBy SYSNAME NULL,\r\n                            DBName SYSNAME NULL,\r\n                            Command VARCHAR(1000) NULL,\r\n                            CPUTime INT NULL,\r\n                            DiskIO INT NULL,\r\n                            LastBatch VARCHAR(1000) NULL,\r\n                            ProgramName VARCHAR(1000) NULL,\r\n                            SPID2 INT,\r\n                            REQUESTID int\r\n                        )\r\n\r\n                        INSERT INTO @Table\r\n                        EXEC sp_who2\r\n\r\n                        SELECT HostName as host, Login as login, ProgramName as application, DBName\r\n                        FROM @Table\r\n                        WHERE DBName = '{0}' and HostName != '{1}' and ProgramName != '{2}'", LicenseHelper.EscapeInvalidCharacters(databaseName), host, applicationName).ToString(), sqlConnection);
			using SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
			while (sqlDataReader.Read())
			{
				list.Add(new ConnectedUser(sqlDataReader["host"].ToString(), sqlDataReader["login"].ToString(), sqlDataReader["application"].ToString()));
			}
			return list;
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to get connected users list", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message);
			return null;
		}
	}

	public static RepositoryExistsStatusEnum CheckIfDatabaseExists(string connectionString, string database)
	{
		using SqlConnection sqlConnection = new SqlConnection(connectionString);
		sqlConnection.Open();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("SELECT count(*) FROM [sys].[databases] WHERE [name] = '{0}'", LicenseHelper.EscapeInvalidCharacters(database));
		using SqlCommand sqlCommand = CommandsWithTimeout.SqlServerForRepository(stringBuilder.ToString(), sqlConnection);
		int num = (int)sqlCommand.ExecuteScalar();
		if (num == 0)
		{
			return RepositoryExistsStatusEnum.NotExists;
		}
		if (num < 0)
		{
			return RepositoryExistsStatusEnum.Error;
		}
		return RepositoryExistsStatusEnum.Exists;
	}

	private static async Task<RepositoryExistsStatusEnum> CheckIfDatabaseExistsAsync(string connectionString, string database, CancellationToken cancellationToken)
	{
		using SqlConnection conn = new SqlConnection(connectionString);
		await conn.OpenAsync(cancellationToken);
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendFormat("SELECT count(*) FROM [sys].[databases] WHERE [name] = '{0}'", LicenseHelper.EscapeInvalidCharacters(database));
		using SqlCommand command = CommandsWithTimeout.SqlServerForRepository(stringBuilder.ToString(), conn);
		int num = (int)(await command.ExecuteScalarAsync(cancellationToken));
		if (num == 0)
		{
			return RepositoryExistsStatusEnum.NotExists;
		}
		if (num < 0)
		{
			return RepositoryExistsStatusEnum.Error;
		}
		return RepositoryExistsStatusEnum.Exists;
	}

	public static bool? IsDatabaseOnAzure(bool isProjectFile, string connectionString)
	{
		try
		{
			if (isProjectFile)
			{
				return false;
			}
			return CommandsFactory.GetCommands(DatabaseType.SqlServer, connectionString).Select.Repository.IsServerOnAzure();
		}
		catch
		{
			return null;
		}
	}
}
