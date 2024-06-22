using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using Dataedo.CustomMessageBox;
using Dataedo.Data.Commands;
using Dataedo.Data.Commands.Enums;
using Dataedo.Data.Factories;
using Dataedo.LicenseHelperLibrary.License;
using Dataedo.Model.Data.Repository;
using Microsoft.Data.SqlClient;

namespace Dataedo.LicenseHelperLibrary.Repository;

public class LicenseHelper
{
    private CommandsSetBase commands;

    public void Initialize(DatabaseType databaseType, string connectionString)
    {
        commands = CommandsFactory.GetCommands(databaseType, connectionString);
    }

    public static string EscapeInvalidCharacters(string input)
    {
        return input?.Replace("'", "''");
    }

    public static string GetConnectionStringForAdministratorConsole(string applicationName, string database, DatabaseType? databaseType, string host, string login, string password, string authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true)
    {
        switch (databaseType)
        {
            case DatabaseType.SqlServer:
            case DatabaseType.AzureSQLDatabase:
            case DatabaseType.AzureSQLDataWarehouse:
                return GetSqlServerConnectionStringBuilder(applicationName, database, host, login, password, port, authenticationType, withPooling, encrypt, trustServerCertificate, withDatabase)?.ConnectionString;
            default:
                return string.Empty;
        }
    }

    public static SqlConnectionStringBuilder GetSqlServerConnectionStringBuilder(string applicationName, string database, string host, string login, string password, int? port, string authenticationType, bool withPooling, bool withDatabase = true)
    {
        return GetSqlServerConnectionStringBuilder(applicationName, database, host, login, password, port, authenticationType, withPooling, encrypt: false, trustServerCertificate: false, withDatabase = true);
    }

    public static SqlConnectionStringBuilder GetSqlServerConnectionStringBuilder(string applicationName, string database, string host, string login, string password, int? port, string authenticationType, bool withPooling, bool encrypt, bool trustServerCertificate, bool withDatabase = true)
    {
        if (withDatabase)
        {
            ThrowNotValidExceptionIfNull(database, "database");
        }
        ThrowNotValidExceptionIfNull(host, "host");
        var sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
        sqlConnectionStringBuilder.ApplicationName = applicationName;
        if (withDatabase)
        {
            sqlConnectionStringBuilder.InitialCatalog = database;
        }
        if (!port.HasValue || port == 1433)
        {
            sqlConnectionStringBuilder.DataSource = host;
        }
        else
        {
            sqlConnectionStringBuilder.DataSource = $"{host},{port}";
        }
        if (authenticationType.Equals("WINDOWS_AUTHENTICATION"))
        {
            sqlConnectionStringBuilder.IntegratedSecurity = true;
            login = WindowsIdentity.GetCurrent().Name;
        }
        else if (authenticationType.Equals("ACTIVE_DIRECTORY_PASSWORD"))
        {
            ThrowNotValidExceptionIfNull(login, "login");
            ThrowNotValidExceptionIfNull(password, "password");
            sqlConnectionStringBuilder.Authentication = SqlAuthenticationMethod.ActiveDirectoryPassword;
            sqlConnectionStringBuilder.UserID = login;
            sqlConnectionStringBuilder.Password = password;
        }
        else if (authenticationType.Equals("ACTIVE_DIRECTORY_INTERACTIVE"))
        {
            ThrowNotValidExceptionIfNull(login, "login");
            sqlConnectionStringBuilder.Authentication = SqlAuthenticationMethod.ActiveDirectoryInteractive;
            if (!string.IsNullOrEmpty(login))
            {
                sqlConnectionStringBuilder.UserID = login;
            }
        }
        else
        {
            ThrowNotValidExceptionIfNull(login, "login");
            ThrowNotValidExceptionIfNull(password, "password");
            sqlConnectionStringBuilder.UserID = login;
            sqlConnectionStringBuilder.Password = password;
        }
        sqlConnectionStringBuilder.FailoverPartner = "";
        sqlConnectionStringBuilder.Pooling = withPooling;
        sqlConnectionStringBuilder.Encrypt = encrypt;
        sqlConnectionStringBuilder.TrustServerCertificate = trustServerCertificate;
        return sqlConnectionStringBuilder;
    }

    private static void ThrowNotValidExceptionIfNull<T>(T value, string name)
    {
        if (value == null)
        {
            throw new ArgumentException("Specified " + name + " is not valid.");
        }
    }

    public static int GetLicensesCount(string connectionString, string databaseName, string key)
    {
        var packId = new LicenseKey(key).PackId;
        var num = 0;
        using var sqlConnection = new SqlConnection(connectionString);
        sqlConnection.Open();
        using (var sqlCommand = new SqlCommand("use [" + databaseName + "];select [key] from licenses", sqlConnection))
        {
            using var sqlDataReader = sqlCommand.ExecuteReader();
            while (sqlDataReader.Read())
            {
                try
                {
                    var licenseKey = new LicenseKey(sqlDataReader["key"].ToString());
                    var packId2 = licenseKey.PackId;
                    if (licenseKey.InitializationException == null && packId2 == packId)
                    {
                        num++;
                    }
                }
                catch (Exception)
                {
                }
            }
        }
        return num;
    }

    public static bool LicenseKeysIdEquals(string firstKey, string secondKey)
    {
        try
        {
            var packId = new LicenseKey(firstKey).PackId;
            var packId2 = new LicenseKey(secondKey).PackId;
            return packId == packId2;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public RepositoryVersion GetRepositoryVersion(CommandsSetBase commands)
    {
        try
        {
            var orderedEnumerable = from x in commands.Select.Repository.GetAllVersions()
                                    orderby x.Version descending, x.Update descending, x.Release descending
                                    select x;
            if (orderedEnumerable == null || orderedEnumerable.Count() == 0)
            {
                return null;
            }
            var repositoryDetailsObject = orderedEnumerable.FirstOrDefault();
            if (repositoryDetailsObject.Update.HasValue && repositoryDetailsObject.Stable.HasValue)
            {
                return new RepositoryVersion(repositoryDetailsObject.Version, repositoryDetailsObject.Update.Value, repositoryDetailsObject.Release.GetValueOrDefault(), repositoryDetailsObject.Stable.Value);
            }
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public RepositoryVersion GetRepositoryVersion(string connectionString)
    {
        try
        {
            var orderedEnumerable = from x in commands.Select.Repository.GetAllVersions()
                                    orderby x.Version descending, x.Update descending, x.Release descending
                                    select x;
            if (orderedEnumerable == null || orderedEnumerable.Count() == 0)
            {
                return null;
            }
            var repositoryDetailsObject = orderedEnumerable.FirstOrDefault();
            if (repositoryDetailsObject.Update.HasValue && repositoryDetailsObject.Stable.HasValue)
            {
                return new RepositoryVersion(repositoryDetailsObject.Version, repositoryDetailsObject.Update.Value, repositoryDetailsObject.Release.GetValueOrDefault(), repositoryDetailsObject.Stable.Value);
            }
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public RepositoryVersionEnum.RepositoryVersion CompareRepositoryVersion(string connectionString, int programVersion, int programUpdate, int programBuild)
    {
        var repositoryVersion = GetRepositoryVersion(connectionString);
        if (repositoryVersion == null)
        {
            return RepositoryVersionEnum.RepositoryVersion.ERROR;
        }
        var matchingRepositoryVersion = repositoryVersion.GetMatchingRepositoryVersion(programVersion, programUpdate, programBuild);
        var num = repositoryVersion.CompareTo(matchingRepositoryVersion.Major, matchingRepositoryVersion.Minor, matchingRepositoryVersion.Build);
        if (num == 0)
        {
            if (!repositoryVersion.Stable)
            {
                return RepositoryVersionEnum.RepositoryVersion.EQUALS_NOT_STABLE;
            }
            return RepositoryVersionEnum.RepositoryVersion.EQUALS;
        }
        if (num < 0)
        {
            return RepositoryVersionEnum.RepositoryVersion.OLDER;
        }
        return RepositoryVersionEnum.RepositoryVersion.NEWER;
    }

    public bool CheckRepositoryVersion(string connectionString, int programVersion, int programUpdate, int programBuild, bool isFile, bool showMessageBoxIfNeeded, out string message, out RepositoryVersionEnum.RepositoryVersion versionMatch)
    {
        message = null;
        versionMatch = CompareRepositoryVersion(connectionString, programVersion, programUpdate, programBuild);
        if (versionMatch == RepositoryVersionEnum.RepositoryVersion.EQUALS)
        {
            return true;
        }
        var repositoryVersion = GetRepositoryVersion(connectionString);
        if (repositoryVersion == null)
        {
            message = "Unable to check repository version. Database error occurred.\nPlease contact your administrator.";
            CustomMessageBoxForm.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            return false;
        }
        var text = "https://dataedo.com/versions/download/" + repositoryVersion.ApplicationVersionString;
        var text2 = $"<href={text}>download Dataedo {repositoryVersion.Version}.{repositoryVersion.Update}.{repositoryVersion.Build}</href>";
        if (versionMatch == RepositoryVersionEnum.RepositoryVersion.EQUALS_NOT_STABLE)
        {
            message = "Repository is unstable and may cause errors.\nPlease contact your administrator.";
            CustomMessageBoxForm.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
        else if (versionMatch == RepositoryVersionEnum.RepositoryVersion.OLDER)
        {
            message = (isFile ? $"You are trying to open file in version {repositoryVersion.ApplicationVersionString} with Dataedo {programVersion}.{programUpdate}.{programBuild}.{Environment.NewLine}Please {text2}." : $"You are trying to connect to repository in version {repositoryVersion.ApplicationVersionString} with Dataedo {programVersion}.{programUpdate}.{programBuild}.{Environment.NewLine}Upgrade repository or {text2}.");
            CustomMessageBoxForm.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
        else if (versionMatch == RepositoryVersionEnum.RepositoryVersion.NEWER)
        {
            message = (isFile ? $"You are trying to open file in version {repositoryVersion.ApplicationVersionString} with Dataedo {programVersion}.{programUpdate}.{programBuild}.{Environment.NewLine}Please {text2}." : $"You are trying to connect to repository in version {repositoryVersion.ApplicationVersionString} with Dataedo {programVersion}.{programUpdate}.{programBuild}.{Environment.NewLine}Please {text2}.");
            CustomMessageBoxForm.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
        return false;
    }

    public bool CheckRepositoryVersion(string connectionString, int programVersion, int programUpdate, int programBuild, bool isFile, out RepositoryVersionEnum.RepositoryVersion versionMatch)
    {
        string message;
        return CheckRepositoryVersion(connectionString, programVersion, programUpdate, programBuild, isFile, showMessageBoxIfNeeded: true, out message, out versionMatch);
    }

    public static bool CheckIfRepositoryIsValid(string connectionString, string database)
    {
        using var sqlConnection = new SqlConnection(connectionString);
        sqlConnection.Open();
        using (var sqlCommand = new SqlCommand("select count(*) where db_id('" + EscapeInvalidCharacters(database) + "') is not null", sqlConnection))
        {
            if ((int)sqlCommand.ExecuteScalar() <= 0)
            {
                return false;
            }
        }
        using var sqlCommand2 = new SqlCommand("use [" + database + "];select(count(COL_LENGTH('licenses','login')) +count(COL_LENGTH('licenses','host')) +count(COL_LENGTH('databases','title')) +count(COL_LENGTH('databases','type')) +count(COL_LENGTH('databases','name')))", sqlConnection);
        return (int)sqlCommand2.ExecuteScalar() == 5;
    }

    public static bool CheckIfRepositoryIsAzure(string connectionString)
    {
        try
        {
            var cmdText = "SELECT SERVERPROPERTY('Edition') AS edition;";
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            var sqlDataReader = new SqlCommand(cmdText, sqlConnection).ExecuteReader(CommandBehavior.CloseConnection);
            if (sqlDataReader.Read())
            {
                var value = sqlDataReader.GetValue(0)?.ToString();
                if ("SQL Azure".Equals(value))
                {
                    return true;
                }
            }
        }
        catch (Exception)
        {
        }
        return false;
    }

    public bool CheckIfLicenseUserIdValid(string key, string login, string connectionString)
    {
        var licenseKey = new LicenseKey(key);
        login = EscapeInvalidCharacters(login);
        return commands.Select.Licenses.GetLicenseId(login) == licenseKey.UserId;
    }

    public static int InsertUpdateLicenseRecord(string connectionString, string login, string databaseLogin, bool isOffline)
    {
        try
        {
            login = EscapeInvalidCharacters(login);
            databaseLogin = EscapeInvalidCharacters(databaseLogin);
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            using var sqlCommand = new SqlCommand("\r\n                            IF EXISTS (\r\n                            \t\tSELECT 1\r\n                            \t\tFROM [licenses]\r\n                            \t\tWHERE LOWER([login]) = LOWER(@login) AND [is_offline] = '1'\r\n                            \t\t)\r\n                            BEGIN\r\n                            \tUPDATE [licenses]\r\n                            \tSET [is_offline] = @is_offline\r\n                            \tWHERE LOWER([login]) = LOWER(@login)\r\n                            END;\r\n                            ELSE IF EXISTS\r\n                            (\r\n                                SELECT 1\r\n                                FROM [licenses]\r\n                                WHERE LOWER([login]) = LOWER(@login) AND [is_offline] is null\r\n                            )\r\n                            BEGIN\r\n                                UPDATE [licenses] \r\n                                SET [login] = @login, [is_offline] = @is_offline \r\n                                WHERE LOWER([login]) = LOWER(@login) AND [is_offline] is null\r\n                            END; \r\n                            ELSE IF EXISTS\r\n                            (\r\n                                SELECT 1\r\n                                FROM [licenses]\r\n                                WHERE LOWER([login]) = LOWER(@databaseLogin) AND [is_offline] is null\r\n                            )\r\n                            AND NOT EXISTS\r\n                            (\r\n                                SELECT 1 \r\n                                FROM [licenses] \r\n                                WHERE LOWER([login]) = LOWER(@login)\r\n                            )\r\n                            BEGIN \r\n                                UPDATE [licenses]\r\n                                SET [login] = @login, [is_offline] = @is_offline\r\n                                WHERE LOWER([login]) = LOWER(@databaseLogin) AND [is_offline] is null\r\n                            END; \r\n                           ELSE IF NOT EXISTS\r\n                            (\r\n                                SELECT 1 FROM [licenses] \r\n                                WHERE LOWER([login]) = LOWER(@login)\r\n                            ) \r\n                            BEGIN \r\n                                INSERT INTO [licenses] ( [login], [is_offline] ) \r\n                                VALUES ( @login, @is_offline ) \r\n                            END;", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@login", login);
            sqlCommand.Parameters.AddWithValue("@databaseLogin", databaseLogin);
            sqlCommand.Parameters.AddWithValue("@is_offline", isOffline);
            return ((int?)sqlCommand.ExecuteScalar()) ?? (-1);
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.ShowException(ex, "Unable to insert/update licenses record");
            return -1;
        }
    }

    public static bool UpdateKeyInLicense(string connectionString, string databaseName, string login, string key)
    {
        var licenseKey = new LicenseKey(key);
        return UpdateKeyInLicense(connectionString, databaseName, login, licenseKey.Code, licenseKey.Number);
    }

    public static bool UpdateKeyInLicense(string connectionString, string databaseName, string login, string key, int maxLicenseCount)
    {
        login = EscapeInvalidCharacters(login);
        using (var sqlConnection = new SqlConnection(connectionString))
        {
            sqlConnection.Open();
            var licenseKey = new LicenseKey(key);
            if (!CheckIfUserHasLicense(connectionString, login, databaseName))
            {
                return false;
            }
            string text = null;
            if (licenseKey.IsParsableLicense)
            {
                var licenseIdForLogin = GetLicenseIdForLogin(connectionString, login);
                if (licenseIdForLogin < 0)
                {
                    return false;
                }
                licenseKey.SetUserId(licenseIdForLogin);
                text = licenseKey.Code;
            }
            else
            {
                text = key;
            }
            using var sqlCommand = new SqlCommand("\r\n                        update licenses set\r\n                            [key] = @key\r\n                        where [login] = @login", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@key", text);
            sqlCommand.Parameters.AddWithValue("@login", login);
            if (sqlCommand.ExecuteNonQuery() < 1)
            {
                CustomMessageBoxForm.Show("Unable to update license", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return false;
            }
        }
        return true;
    }

    public static int GetLicenseIdForLogin(string connectionString, string login)
    {
        try
        {
            login = EscapeInvalidCharacters(login);
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            using var sqlCommand = new SqlCommand("select license_id from licenses where [login] = @login", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@login", login);
            return ((int?)sqlCommand.ExecuteScalar()) ?? (-1);
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.ShowException(ex, "Unable to get next license id");
            return -1;
        }
    }

    public static int GetNextLicenseId(string connectionString, string databaseName)
    {
        try
        {
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            using var sqlCommand = new SqlCommand("use [" + databaseName + "];select IDENT_CURRENT('licenses') + 1", sqlConnection);
            var num = int.Parse(sqlCommand.ExecuteScalar().ToString());
            if (num == 2 && GetExistingUsers(connectionString, databaseName).Count == 0)
            {
                num = 1;
            }
            return num;
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.ShowException(ex, "Unable to get next license_id");
            return -1;
        }
    }

    public static List<string> GetExistingUsers(string connectionString, string databaseName)
    {
        try
        {
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            using var sqlCommand = new SqlCommand("use [" + databaseName + "]; select login from licenses", sqlConnection);
            using var sqlDataReader = sqlCommand.ExecuteReader();
            var list = new List<string>();
            while (sqlDataReader.Read())
            {
                list.Add(sqlDataReader["login"].ToString());
            }
            return list;
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.ShowException(ex, "Unable to get licensed users list");
            return new List<string>();
        }
    }

    public static BindingList<RepositoryLicense> GetUserLicenses(string connectionString, string databaseName)
    {
        var bindingList = new BindingList<RepositoryLicense>();
        using var sqlConnection = new SqlConnection(connectionString);
        sqlConnection.Open();
        using var sqlCommand = new SqlCommand("use [" + databaseName + "];\r\n                                                SELECT p.name collate database_default as name\r\n                                                    ,[password]\r\n                                                    ,[key]\r\n                                                    ,1 AS is_login_exists\r\n                                                FROM sys.server_principals p \r\n                                                                 INNER JOIN master.sys.syslogins s on p.NAME collate database_default = s.NAME collate database_default\r\n                                                    LEFT JOIN licenses l ON s.name collate database_default = l.[login] collate database_default\r\n                                                WHERE lower(p.NAME) NOT LIKE 'nt service\\%'\r\n                                                    AND lower(p.NAME) NOT LIKE '%nt%\\system'\r\n                                                    AND type IN ('S', 'U')\r\n                                                UNION \r\n                                                SELECT [login]\r\n                                                    ,NULL\r\n                                                    ,[key]\r\n                                                    ,0 AS is_login_exists\r\n                                                FROM licenses\r\n                                                WHERE [login] NOT IN (\r\n                                                    SELECT p.NAME collate database_default\r\n                                                    FROM sys.server_principals p\r\n                                                       INNER JOIN master.sys.syslogins s on p.NAME collate database_default = s.NAME collate database_default\r\n                                                        LEFT JOIN licenses l ON s.NAME collate database_default = l.[login] collate database_default\r\n                                                    WHERE lower(p.NAME) NOT LIKE 'nt service\\%'\r\n                                                        AND lower(p.NAME) NOT LIKE '%nt%\\system'\r\n                                                        AND type IN ('S' ,'U')\r\n                                                    )", sqlConnection);
        var sqlDataReader = sqlCommand.ExecuteReader();
        while (sqlDataReader.Read())
        {
            bindingList.Add(new RepositoryLicense(sqlDataReader["name"].ToString(), sqlDataReader["password"].ToString(), CheckIfUserIsActive(connectionString, sqlDataReader["name"].ToString()), Convert.ToBoolean(sqlDataReader["is_login_exists"])));
        }
        return bindingList;
    }

    private static bool CheckIfUserIsActive(string connectionString, string user)
    {
        try
        {
            user = EscapeInvalidCharacters(user);
            var cmdText = "\r\n                    SELECT count(*)\r\n                    FROM sys.database_role_members rm \r\n                    JOIN sys.database_principals r \r\n                        ON rm.role_principal_id = r.principal_id\r\n                    JOIN sys.database_principals m \r\n                        ON rm.member_principal_id = m.principal_id\r\n                    WHERE m.[name] = @user AND r.[name] = 'users'";
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            using var sqlCommand = new SqlCommand(cmdText, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@user", user);
            return (int)sqlCommand.ExecuteScalar() > 0;
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.ShowException(ex, "Unable to check if user is active");
            return false;
        }
    }

    public string GetLicenseKey(string user)
    {
        return GetLicenseKey(user, commands);
    }

    public string GetLicenseKey(string user, CommandsSetBase commands)
    {
        user = EscapeInvalidCharacters(user);
        return commands.Select.Licenses.GetLicenseKey(user);
    }

    public static bool CheckIfUserHasLicense(string connectionString, string user, string databaseName)
    {
        try
        {
            user = EscapeInvalidCharacters(user);
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            using var sqlCommand = new SqlCommand("use [" + databaseName + "];select count(*) from licenses where [login] = @user", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@user", user);
            return (int)sqlCommand.ExecuteScalar() > 0;
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.ShowException(ex, "Unable to check user license");
            return false;
        }
    }

    public static string GetUserForLogin(string connectionString, string login, string databaseName)
    {
        try
        {
            login = EscapeInvalidCharacters(login);
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("use [{0}]; SELECT dp.name FROM sys.server_principals sp\r\n                    JOIN sys.database_principals dp ON (sp.sid = dp.sid)\r\n                    WHERE sp.name = '{1}'", databaseName, login);
            using var sqlCommand = new SqlCommand(stringBuilder.ToString(), sqlConnection);
            using var sqlDataReader = sqlCommand.ExecuteReader();
            if (sqlDataReader.Read())
            {
                return sqlDataReader[0].ToString();
            }
            return string.Empty;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Unable to get user for login " + login + "\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            return null;
        }
    }

    public static bool ActivateUser(string connectionString, string databaseName, string login, bool activate)
    {
        try
        {
            login = EscapeInvalidCharacters(login);
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            var userForLogin = GetUserForLogin(connectionString, login, databaseName);
            if (userForLogin == null)
            {
                return false;
            }
            if (!userForLogin.Equals("dbo") && !login.Equals("sa"))
            {
                var stringBuilder = new StringBuilder();
                if (activate)
                {
                    if (string.IsNullOrWhiteSpace(userForLogin))
                    {
                        stringBuilder.AppendFormat("use [{0}];IF NOT EXISTS(SELECT * FROM sys.database_principals WHERE [name] = '{1}')\r\n                                                            BEGIN\r\n                                                            create user [{1}] for login [{1}];\r\n                                                            END;", databaseName, login);
                    }
                    stringBuilder.AppendFormat("use [{0}];exec sp_addrolemember @rolename = 'users', @membername = '{1}';", databaseName, login);
                    stringBuilder.AppendFormat("use [{0}];IF NOT EXISTS(SELECT [license_id] FROM [licenses] WHERE [login] = '{1}')\r\n                                    BEGIN\r\n                                        INSERT INTO licenses([login], [is_offline]) VALUES('{1}', 1)\r\n                                    END;", databaseName, login);
                }
                else if (!string.IsNullOrWhiteSpace(userForLogin))
                {
                    stringBuilder.AppendFormat("use [{0}];exec sp_droprolemember @rolename = 'users', @membername = '{1}'", databaseName, userForLogin);
                }
                if (!string.IsNullOrWhiteSpace(stringBuilder.ToString()))
                {
                    using var sqlCommand = new SqlCommand(stringBuilder.ToString(), sqlConnection);
                    sqlCommand.ExecuteNonQuery();
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.Show("Unable to " + (activate ? "activate" : "deactivate") + " user \n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            return false;
        }
    }

    public static bool ChangeUserPassword(string connectionString, string user, string password)
    {
        try
        {
            user = EscapeInvalidCharacters(user);
            password = EscapeInvalidCharacters(password);
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("alter login [{0}] with password = '{1}'", user, password);
            using (var sqlCommand = new SqlCommand(stringBuilder.ToString(), sqlConnection))
            {
                sqlCommand.ExecuteNonQuery();
            }
            return true;
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.ShowException(ex, "Unable to change user password");
            return false;
        }
    }

    public static bool CheckIfUserExists(string connectionString, string user)
    {
        try
        {
            user = EscapeInvalidCharacters(user);
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            using var sqlCommand = new SqlCommand("select count(*) from sys.server_principals where name = @user and type in ('S', 'U')", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@user", user);
            return (int)sqlCommand.ExecuteScalar() > 0;
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.ShowException(ex, "Unable to check if user exists");
            return false;
        }
    }

    public static bool CheckIfLicenseExists(string connectionString, string user)
    {
        try
        {
            user = EscapeInvalidCharacters(user);
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            using var sqlCommand = new SqlCommand("select count(*) from licenses where login = @user", sqlConnection);
            sqlCommand.Parameters.AddWithValue("@user", user);
            return (int)sqlCommand.ExecuteScalar() > 0;
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.ShowException(ex, "Unable to check if user exists");
            return false;
        }
    }

    public static bool HasLoginPermissions(string connectionString, string permission)
    {
        try
        {
            permission = EscapeInvalidCharacters(permission);
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("SELECT COUNT(*) FROM fn_my_permissions(NULL, 'SERVER') where permission_name='" + permission + "'; ");
            using var sqlCommand = new SqlCommand(stringBuilder.ToString(), sqlConnection);
            using var sqlDataReader = sqlCommand.ExecuteReader();
            sqlDataReader.Read();
            var result = 0;
            return int.TryParse(sqlDataReader[0].ToString(), out result) && result == 1;
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.Show("Unable to check user role", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message);
            return false;
        }
    }

    public static bool IsUserAdmin(string connectionString, string login)
    {
        try
        {
            login = EscapeInvalidCharacters(login);
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("select is_srvrolemember('sysadmin', '{0}')", login);
            using var sqlCommand = new SqlCommand(stringBuilder.ToString(), sqlConnection);
            using var sqlDataReader = sqlCommand.ExecuteReader();
            sqlDataReader.Read();
            var result = 0;
            return int.TryParse(sqlDataReader[0].ToString(), out result) && result == 1;
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.Show("Unable to check user role", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message);
            return false;
        }
    }

    public static bool IsUserDbOwner(string connectionString)
    {
        try
        {
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            using var sqlCommand = new SqlCommand("SELECT IS_MEMBER ('db_owner')", sqlConnection);
            using var sqlDataReader = sqlCommand.ExecuteReader();
            sqlDataReader.Read();
            var result = 0;
            return int.TryParse(sqlDataReader[0].ToString(), out result) && result == 1;
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.Show("Unable to check user role", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message);
            return false;
        }
    }

    public static bool HasUserAdminsRole(string connectionString)
    {
        try
        {
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            using var sqlCommand = new SqlCommand("SELECT IS_ROLEMEMBER ('admins')", sqlConnection);
            using var sqlDataReader = sqlCommand.ExecuteReader();
            sqlDataReader.Read();
            var result = 0;
            return int.TryParse(sqlDataReader[0].ToString(), out result) && result == 1;
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.Show("Unable to check user role", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message);
            return false;
        }
    }

    public static DateTime? GetActualDate(string connectionString, bool getLocalDate)
    {
        var now = DateTime.Now;
        if (getLocalDate)
        {
            return now.Date;
        }
        try
        {
            using var sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            using var sqlCommand = new SqlCommand("select getdate()", sqlConnection);
            using var sqlDataReader = sqlCommand.ExecuteReader();
            sqlDataReader.Read();
            return DateTime.Parse(sqlDataReader[0].ToString()).Date;
        }
        catch (Exception ex)
        {
            CustomMessageBoxForm.ShowException(ex, "Unable to get server date");
            return null;
        }
    }

    public static DateTime GetActualMaxDate(string connectionString, bool getLocalDate)
    {
        var actualDate = GetActualDate(connectionString, getLocalDate: true);
        var dateTime = ((connectionString == null) ? null : GetActualDate(connectionString, getLocalDate));
        if (!dateTime.HasValue)
        {
            if (!actualDate.HasValue)
            {
                return DateTime.Now.Date;
            }
            return actualDate.Value;
        }
        return ((actualDate > dateTime) ? actualDate : dateTime).Value.Date;
    }

    public static bool ChangeRepositoryLicense(string connectionString, string databaseName, string login, LicenseKey actualLicenseKey, LicenseKey licenseKey, Action<int> noRemainingLicensesAction, bool doNotCheckRemainingLicenses)
    {
        if (!string.IsNullOrEmpty(connectionString) && (doNotCheckRemainingLicenses || CheckLicenseUsesCount(connectionString, databaseName, actualLicenseKey, licenseKey, noRemainingLicensesAction)))
        {
            UpdateKeyInLicense(connectionString, databaseName, login, licenseKey.Code, licenseKey.Number);
            return true;
        }
        return false;
    }

    public static bool CheckLicenseUsesCount(string connectionString, string database, LicenseKey actualLicenseKey, LicenseKey licenseKey, Action<int> noRemainingLicensesAction)
    {
        var num = (LicenseKeysIdEquals((actualLicenseKey == null) ? string.Empty : actualLicenseKey.Code, licenseKey.Code) ? 1 : 0);
        var licensesCount = GetLicensesCount(connectionString, database, licenseKey.Code);
        if (licensesCount < 0)
        {
            return false;
        }
        var number = licenseKey.Number;
        if (licensesCount >= number + num)
        {
            noRemainingLicensesAction?.Invoke(number);
            return false;
        }
        return true;
    }
}
