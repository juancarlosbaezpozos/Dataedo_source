using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Drivers.ODBC;
using Dataedo.App.Drivers.ODBC.Repositories;
using Dataedo.App.Properties;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DatabasesSupport;

public static class DatabaseSupportFactory
{
    public static IDatabaseSupport GetDatabaseSupport(SharedDatabaseTypeEnum.DatabaseType? databaseType)
    {
        switch (databaseType)
        {
            case SharedDatabaseTypeEnum.DatabaseType.SqlServer:
                return new SqlServerSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Oracle:
                return new OracleSupport();
            case SharedDatabaseTypeEnum.DatabaseType.MySQL:
            case SharedDatabaseTypeEnum.DatabaseType.MySQL8:
                return new MySqlSupport();
            case SharedDatabaseTypeEnum.DatabaseType.MariaDB:
                return new MariaDbSupport();
            case SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase:
                return new AzureSqlDatabaseSupport();
            case SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse:
                return new AzureSqlDataWarehouseSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Aurora:
                return new AuroraSupport();
            case SharedDatabaseTypeEnum.DatabaseType.PostgreSQL:
                return new PostgreSqlSupport();
            case SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL:
            case SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL8:
                return new PerconaMySqlSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Snowflake:
                return new SnowflakeSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Redshift:
                return new RedshiftSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Manual:
                return new ManualDatabaseSupport();
            case SharedDatabaseTypeEnum.DatabaseType.AuroraPostgreSQL:
                return new AuroraPostgreSqlSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Db2LUW:
                return new Db2Support();
            case SharedDatabaseTypeEnum.DatabaseType.Db2Cloud:
                return new Db2CloudSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Odbc:
                return new OdbcSupport();
            case SharedDatabaseTypeEnum.DatabaseType.IBMDb2BigQuery:
                return new Db2BigQuerySupport();
            case SharedDatabaseTypeEnum.DatabaseType.Vertica:
                return new VerticaSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Teradata:
                return new TeradataSupport();
            case SharedDatabaseTypeEnum.DatabaseType.MongoDB:
                return new MongoDBSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Cassandra:
                return new CassandraSupport();
            case SharedDatabaseTypeEnum.DatabaseType.GoogleBigQuery:
                return new GoogleBigQuerySupport();
            case SharedDatabaseTypeEnum.DatabaseType.Elasticsearch:
                return new ElasticsearchSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Neo4j:
                return new Neo4jSupport();
            case SharedDatabaseTypeEnum.DatabaseType.SsasTabular:
                return new SsasTabularSupport();
            case SharedDatabaseTypeEnum.DatabaseType.PowerBiDataset:
                return new PowerBiDatasetSupport();
            case SharedDatabaseTypeEnum.DatabaseType.CosmosDB:
                return new CosmosDbSupport();
            case SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL:
                return new CosmosDbSqlSupport();
            case SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra:
                return new CosmosDbCassandraApiSupport();
            case SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB:
                return new CosmosDbMongoDbApiSupport();
            case SharedDatabaseTypeEnum.DatabaseType.CosmosDbTable:
                return new CosmosDbTableApiSupport();
            case SharedDatabaseTypeEnum.DatabaseType.CosmosDbGremlin:
                return new CosmosDbGremlinAPISupport();
            case SharedDatabaseTypeEnum.DatabaseType.ApacheHiveMetastore:
                return new ApacheHiveMetastoreSupport();
            case SharedDatabaseTypeEnum.DatabaseType.ApacheImpalaMetastore:
                return new ApacheImpalaMetastoreSupport();
            case SharedDatabaseTypeEnum.DatabaseType.ApacheSparkMetastore:
                return new ApacheSparkMetastoreSupport();
            case SharedDatabaseTypeEnum.DatabaseType.HiveMetastore:
                return new HiveMetastoreSupport();
            case SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMariaDB:
                return new HiveMetastoreMariaDBSupport();
            case SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreSQLServer:
                return new HiveMetastoreSQLServerSupport();
            case SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMySQL:
                return new HiveMetastoreMySQLSupport();
            case SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreOracle:
                return new HiveMetastoreOracleSupport();
            case SharedDatabaseTypeEnum.DatabaseType.HiveMetastorePostgreSQL:
                return new HiveMetastorePostgreSQLSupport();
            case SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreAzureSQL:
                return new HiveMetastoreAzureSQLSupport();
            case SharedDatabaseTypeEnum.DatabaseType.AmazonKeyspaces:
                return new AmazonKeyspacesSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Salesforce:
                return new SalesforceSupport();
            case SharedDatabaseTypeEnum.DatabaseType.SapAse:
                return new SapAseSupport();
            case SharedDatabaseTypeEnum.DatabaseType.DdlScript:
                return new DdlScriptSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Athena:
                return new AthenaSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Tableau:
                return new TableauSupport();
            case SharedDatabaseTypeEnum.DatabaseType.DatabricksMetastore:
                return new DatabricksMetastoreSupport();
            case SharedDatabaseTypeEnum.DatabaseType.AmazonS3:
                return new AmazonS3Support();
            case SharedDatabaseTypeEnum.DatabaseType.InterfaceTables:
                return new InterfaceTablesSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Dataverse:
                return new DataverseSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Dynamics365:
                return new Dynamics365Support();
            case SharedDatabaseTypeEnum.DatabaseType.DynamoDB:
                return new DynamoDBSupport();
            case SharedDatabaseTypeEnum.DatabaseType.SapHana:
                return new SapHanaSupport();
            case SharedDatabaseTypeEnum.DatabaseType.Astra:
                return new AstraSupport();
            case SharedDatabaseTypeEnum.DatabaseType.DBT:
                return new DBTSupport();
            case SharedDatabaseTypeEnum.DatabaseType.NetSuite:
                return new NetSuiteSupport();
            case SharedDatabaseTypeEnum.DatabaseType.SSIS:
                return new SSISSupport();
            case SharedDatabaseTypeEnum.DatabaseType.AzureBlobStorage:
                return new AzureBlobStorageSupport();
            case SharedDatabaseTypeEnum.DatabaseType.AzureDataLakeStorage:
                return new AzureDataLakeStorageSupport();
            default:
                throw new NotSupportedException("The selected database type is not supported.");
        }
    }

    public static IEnumerable<IDatabaseSupport> GetOrderedDatabaseSupportObjectsForImport()
    {
        SharedDatabaseTypeEnum.DatabaseType[] second = new SharedDatabaseTypeEnum.DatabaseType[16]
        {
            SharedDatabaseTypeEnum.DatabaseType.Manual,
            SharedDatabaseTypeEnum.DatabaseType.Odbc,
            SharedDatabaseTypeEnum.DatabaseType.MySQL8,
            SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL8,
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL,
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra,
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB,
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbTable,
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbGremlin,
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreAzureSQL,
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMariaDB,
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMySQL,
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreOracle,
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastorePostgreSQL,
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreSQLServer,
            SharedDatabaseTypeEnum.DatabaseType.InterfaceTables
        };
        List<IDatabaseSupport> list = (from x in Enum.GetValues(typeof(SharedDatabaseTypeEnum.DatabaseType)).Cast<SharedDatabaseTypeEnum.DatabaseType>().Except(second)
                                       select GetDatabaseSupport(x) into x
                                       orderby x.FriendlyDisplayName
                                       select x).ToList();
        if (!StaticData.IsProjectFile)
        {
            list.Add(GetDatabaseSupport(SharedDatabaseTypeEnum.DatabaseType.InterfaceTables));
        }
        list.Add(GetDatabaseSupport(SharedDatabaseTypeEnum.DatabaseType.Odbc));
        list.Add(GetDatabaseSupport(SharedDatabaseTypeEnum.DatabaseType.Manual));
        return list;
    }

    public static IEnumerable<IDatabaseSupport> GetDatabaseSubtypesSupportObjectsForImport(SharedDatabaseTypeEnum.DatabaseType? databaseType)
    {
        IEnumerable<SharedDatabaseTypeEnum.DatabaseType> enumerable = null;
        switch (databaseType)
        {
            case SharedDatabaseTypeEnum.DatabaseType.CosmosDB:
                enumerable = new SharedDatabaseTypeEnum.DatabaseType[5]
                {
                SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL,
                SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra,
                SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB,
                SharedDatabaseTypeEnum.DatabaseType.CosmosDbGremlin,
                SharedDatabaseTypeEnum.DatabaseType.CosmosDbTable
                };
                break;
            case SharedDatabaseTypeEnum.DatabaseType.HiveMetastore:
            case SharedDatabaseTypeEnum.DatabaseType.ApacheHiveMetastore:
            case SharedDatabaseTypeEnum.DatabaseType.ApacheImpalaMetastore:
            case SharedDatabaseTypeEnum.DatabaseType.ApacheSparkMetastore:
            case SharedDatabaseTypeEnum.DatabaseType.DatabricksMetastore:
                enumerable = new SharedDatabaseTypeEnum.DatabaseType[6]
                {
                SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreAzureSQL,
                SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMariaDB,
                SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMySQL,
                SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreOracle,
                SharedDatabaseTypeEnum.DatabaseType.HiveMetastorePostgreSQL,
                SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreSQLServer
                };
                break;
        }
        return enumerable?.Select((SharedDatabaseTypeEnum.DatabaseType x) => GetDatabaseSupport(x))?.ToList();
    }

    public static bool IsAdditionalSelectionType(SharedDatabaseTypeEnum.DatabaseType? databaseType)
    {
        if (!databaseType.HasValue)
        {
            return false;
        }
        if (new SharedDatabaseTypeEnum.DatabaseType[11]
        {
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL,
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra,
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB,
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbGremlin,
            SharedDatabaseTypeEnum.DatabaseType.CosmosDbTable,
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMySQL,
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMariaDB,
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastorePostgreSQL,
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreOracle,
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreSQLServer,
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreAzureSQL
        }.Contains(databaseType.Value))
        {
            return true;
        }
        return false;
    }

    public static SharedDatabaseTypeEnum.DatabaseType? GetParentDatabaseType(SharedDatabaseTypeEnum.DatabaseType? databaseType)
    {
        if (!databaseType.HasValue)
        {
            return null;
        }
        switch (databaseType)
        {
            case SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL:
            case SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra:
            case SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB:
            case SharedDatabaseTypeEnum.DatabaseType.CosmosDbTable:
            case SharedDatabaseTypeEnum.DatabaseType.CosmosDbGremlin:
                return SharedDatabaseTypeEnum.DatabaseType.CosmosDB;
            case SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMySQL:
            case SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMariaDB:
            case SharedDatabaseTypeEnum.DatabaseType.HiveMetastorePostgreSQL:
            case SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreOracle:
            case SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreSQLServer:
            case SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreAzureSQL:
                return SharedDatabaseTypeEnum.DatabaseType.HiveMetastore;
            default:
                return databaseType;
        }
    }

    public static bool IsTypeWithSubtypes(SharedDatabaseTypeEnum.DatabaseType? databaseType)
    {
        if (!databaseType.HasValue || !databaseType.HasValue)
        {
            return false;
        }
        return new SharedDatabaseTypeEnum.DatabaseType[6]
        {
            SharedDatabaseTypeEnum.DatabaseType.CosmosDB,
            SharedDatabaseTypeEnum.DatabaseType.HiveMetastore,
            SharedDatabaseTypeEnum.DatabaseType.ApacheHiveMetastore,
            SharedDatabaseTypeEnum.DatabaseType.ApacheImpalaMetastore,
            SharedDatabaseTypeEnum.DatabaseType.ApacheSparkMetastore,
            SharedDatabaseTypeEnum.DatabaseType.DatabricksMetastore
        }.Contains(databaseType.Value);
    }

    public static bool SupportsFilters(SharedDatabaseTypeEnum.DatabaseType? databaseType, DatabaseRow databaseRow)
    {
        switch (databaseType)
        {
            case SharedDatabaseTypeEnum.DatabaseType.Odbc:
                {
                    IRepository localRepository = Factory.GetLocalRepository();
                    string text = databaseRow?.ServiceName;
                    if (text != null)
                    {
                        return localRepository.Has(text);
                    }
                    return false;
                }
            case SharedDatabaseTypeEnum.DatabaseType.SsasTabular:
            case SharedDatabaseTypeEnum.DatabaseType.DdlScript:
            case SharedDatabaseTypeEnum.DatabaseType.AmazonS3:
            case SharedDatabaseTypeEnum.DatabaseType.AzureBlobStorage:
            case SharedDatabaseTypeEnum.DatabaseType.AzureDataLakeStorage:
                return false;
            default:
                return true;
        }
    }

    public static Image GetDatabaseSupportImage(SharedDatabaseTypeEnum.DatabaseType? databaseType)
    {
        return GetDatabaseSupport(databaseType)?.TypeImage ?? Resources.server_16;
    }
}
