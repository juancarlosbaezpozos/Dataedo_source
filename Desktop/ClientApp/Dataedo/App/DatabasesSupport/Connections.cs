using System.Data.SqlServerCe;
using Dataedo.App.Enums.Extensions;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport;

internal static class Connections
{
    public static string GetConnectionString(string applicationName, string database, DatabaseType? databaseType, string host, string login, string password, AuthenticationType.AuthenticationTypeEnum authenticationType, int? port, string identifierName, bool withPooling, bool withDatabase = true, bool isUnicode = false, bool isDirectConnect = false, bool encrypt = false, bool trustServerCertificate = false, string keyPath = null, string certPath = null, string caPath = null, string cipher = null, bool useStandardConnectionString = false, bool isServiceName = true, string userConnectionString = null, bool isSrv = false, string authenticationDatabase = null, string replicaSet = null, string multiHost = null, string SSLType = null, string SSLKeyPath = null, string connectionRole = null, string charset = null, string param1 = null, string param2 = null, string param3 = null, string param4 = null, string param5 = null, string param6 = null, string param7 = null, string param8 = null, string param9 = null, string param10 = null)
    {
        if (databaseType.HasValue && databaseType.GetValueOrDefault() == DatabaseType.SqlServerCe)
        {
            return GetSqlServerCeConnectionStringBuilder(database)?.ConnectionString;
        }
        return DatabaseSupportFactory.GetDatabaseSupport(databaseType.AsDatabaseType()).GetConnectionString(applicationName, database, host, login, password, authenticationType, port, identifierName, withPooling, withDatabase, isUnicode, isDirectConnect, encrypt, trustServerCertificate, keyPath, certPath, caPath, cipher, useStandardConnectionString, isServiceName, userConnectionString, isSrv, authenticationDatabase, replicaSet, multiHost, SSLType, SSLKeyPath, connectionRole, charset, param1, param2, param3, param4, param5, param6, param7, param8, param9, param10);
    }

    private static SqlCeConnectionStringBuilder GetSqlServerCeConnectionStringBuilder(string database)
    {
        return new SqlCeConnectionStringBuilder
        {
            DataSource = database,
            MaxDatabaseSize = 4091
        };
    }
}
