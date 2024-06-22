using System;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.LicenseHelperLibrary.License;

public class ConnectionParemeters
{
    public DatabaseType RepositoryType { get; set; }

    public string ConnectionString { get; set; }

    public ConnectionParemeters(DatabaseType repositoryType, string connectionString)
    {
        RepositoryType = repositoryType;
        ConnectionString = connectionString ?? throw new ArgumentNullException("connectionString");
    }
}
