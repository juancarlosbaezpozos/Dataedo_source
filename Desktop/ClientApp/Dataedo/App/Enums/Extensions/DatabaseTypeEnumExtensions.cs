using System;
using System.Collections.Generic;
using Dataedo.Data.Commands.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Enums.Extensions;

public static class DatabaseTypeEnumExtensions
{
	public static readonly Dictionary<DatabaseType, SharedDatabaseTypeEnum.DatabaseType> CommandsDatabaseTypeToDatabaseType = new Dictionary<DatabaseType, SharedDatabaseTypeEnum.DatabaseType>
	{
		{
			DatabaseType.Aurora,
			SharedDatabaseTypeEnum.DatabaseType.Aurora
		},
		{
			DatabaseType.AuroraPostgreSQL,
			SharedDatabaseTypeEnum.DatabaseType.AuroraPostgreSQL
		},
		{
			DatabaseType.AzureSQLDatabase,
			SharedDatabaseTypeEnum.DatabaseType.AzureSQLDatabase
		},
		{
			DatabaseType.AzureSQLDataWarehouse,
			SharedDatabaseTypeEnum.DatabaseType.AzureSQLDataWarehouse
		},
		{
			DatabaseType.Db2LUW,
			SharedDatabaseTypeEnum.DatabaseType.Db2LUW
		},
		{
			DatabaseType.Db2Cloud,
			SharedDatabaseTypeEnum.DatabaseType.Db2Cloud
		},
		{
			DatabaseType.IBMDB2BigQuery,
			SharedDatabaseTypeEnum.DatabaseType.IBMDb2BigQuery
		},
		{
			DatabaseType.MariaDB,
			SharedDatabaseTypeEnum.DatabaseType.MariaDB
		},
		{
			DatabaseType.MySql,
			SharedDatabaseTypeEnum.DatabaseType.MySQL
		},
		{
			DatabaseType.MySQL8,
			SharedDatabaseTypeEnum.DatabaseType.MySQL
		},
		{
			DatabaseType.Odbc,
			SharedDatabaseTypeEnum.DatabaseType.Odbc
		},
		{
			DatabaseType.Oracle,
			SharedDatabaseTypeEnum.DatabaseType.Oracle
		},
		{
			DatabaseType.PerconaMySQL,
			SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL
		},
		{
			DatabaseType.PerconaMySQL8,
			SharedDatabaseTypeEnum.DatabaseType.PerconaMySQL
		},
		{
			DatabaseType.PostgreSQL,
			SharedDatabaseTypeEnum.DatabaseType.PostgreSQL
		},
		{
			DatabaseType.Redshift,
			SharedDatabaseTypeEnum.DatabaseType.Redshift
		},
		{
			DatabaseType.Snowflake,
			SharedDatabaseTypeEnum.DatabaseType.Snowflake
		},
		{
			DatabaseType.SqlServer,
			SharedDatabaseTypeEnum.DatabaseType.SqlServer
		},
		{
			DatabaseType.Vertica,
			SharedDatabaseTypeEnum.DatabaseType.Vertica
		},
		{
			DatabaseType.Teradata,
			SharedDatabaseTypeEnum.DatabaseType.Teradata
		},
		{
			DatabaseType.MongoDB,
			SharedDatabaseTypeEnum.DatabaseType.MongoDB
		},
		{
			DatabaseType.Cassandra,
			SharedDatabaseTypeEnum.DatabaseType.Cassandra
		},
		{
			DatabaseType.GoogleBigQuery,
			SharedDatabaseTypeEnum.DatabaseType.GoogleBigQuery
		},
		{
			DatabaseType.Elasticsearch,
			SharedDatabaseTypeEnum.DatabaseType.Elasticsearch
		},
		{
			DatabaseType.Neo4j,
			SharedDatabaseTypeEnum.DatabaseType.Neo4j
		},
		{
			DatabaseType.SsasTabular,
			SharedDatabaseTypeEnum.DatabaseType.SsasTabular
		},
		{
			DatabaseType.PowerBiDataset,
			SharedDatabaseTypeEnum.DatabaseType.PowerBiDataset
		},
		{
			DatabaseType.CosmosDBSql,
			SharedDatabaseTypeEnum.DatabaseType.CosmosDbSQL
		},
		{
			DatabaseType.CosmosDbMongoDB,
			SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB
		},
		{
			DatabaseType.CosmosDbCassandra,
			SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra
		},
		{
			DatabaseType.CosmosDbTable,
			SharedDatabaseTypeEnum.DatabaseType.CosmosDbTable
		},
		{
			DatabaseType.CosmosDbGremlin,
			SharedDatabaseTypeEnum.DatabaseType.CosmosDbGremlin
		},
		{
			DatabaseType.HiveMetastoreMariaDB,
			SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMariaDB
		},
		{
			DatabaseType.HiveMetastoreSQLServer,
			SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreSQLServer
		},
		{
			DatabaseType.HiveMetastoreMySQL,
			SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreMySQL
		},
		{
			DatabaseType.HiveMetastoreOracle,
			SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreOracle
		},
		{
			DatabaseType.HiveMetastorePostgreSQL,
			SharedDatabaseTypeEnum.DatabaseType.HiveMetastorePostgreSQL
		},
		{
			DatabaseType.HiveMetastoreAzureSQL,
			SharedDatabaseTypeEnum.DatabaseType.HiveMetastoreAzureSQL
		},
		{
			DatabaseType.AmazonKeyspaces,
			SharedDatabaseTypeEnum.DatabaseType.AmazonKeyspaces
		},
		{
			DatabaseType.Salesforce,
			SharedDatabaseTypeEnum.DatabaseType.Salesforce
		},
		{
			DatabaseType.SapAse,
			SharedDatabaseTypeEnum.DatabaseType.SapAse
		},
		{
			DatabaseType.Athena,
			SharedDatabaseTypeEnum.DatabaseType.Athena
		},
		{
			DatabaseType.Tableau,
			SharedDatabaseTypeEnum.DatabaseType.Tableau
		},
		{
			DatabaseType.AmazonS3,
			SharedDatabaseTypeEnum.DatabaseType.AmazonS3
		},
		{
			DatabaseType.InterfaceTables,
			SharedDatabaseTypeEnum.DatabaseType.InterfaceTables
		},
		{
			DatabaseType.DdlScript,
			SharedDatabaseTypeEnum.DatabaseType.DdlScript
		},
		{
			DatabaseType.Dataverse,
			SharedDatabaseTypeEnum.DatabaseType.Dataverse
		},
		{
			DatabaseType.Dynamics365,
			SharedDatabaseTypeEnum.DatabaseType.Dynamics365
		},
		{
			DatabaseType.DynamoDB,
			SharedDatabaseTypeEnum.DatabaseType.DynamoDB
		},
		{
			DatabaseType.SapHana,
			SharedDatabaseTypeEnum.DatabaseType.SapHana
		},
		{
			DatabaseType.Astra,
			SharedDatabaseTypeEnum.DatabaseType.Astra
		},
		{
			DatabaseType.NetSuite,
			SharedDatabaseTypeEnum.DatabaseType.NetSuite
		},
		{
			DatabaseType.DBT,
			SharedDatabaseTypeEnum.DatabaseType.DBT
		},
		{
			DatabaseType.SSIS,
			SharedDatabaseTypeEnum.DatabaseType.SSIS
		},
		{
			DatabaseType.AzureBlobStorage,
			SharedDatabaseTypeEnum.DatabaseType.AzureBlobStorage
		},
		{
			DatabaseType.AzureDataLakeStorage,
			SharedDatabaseTypeEnum.DatabaseType.AzureDataLakeStorage
		}
	};

	public static SharedDatabaseTypeEnum.DatabaseType AsDatabaseType(this DatabaseType? commandsDatabaseType)
	{
		if (!commandsDatabaseType.HasValue)
		{
			throw new NotSupportedException();
		}
		return commandsDatabaseType.Value.AsDatabaseType();
	}

	public static SharedDatabaseTypeEnum.DatabaseType AsDatabaseType(this DatabaseType commandsDatabaseType)
	{
		if (CommandsDatabaseTypeToDatabaseType.TryGetValue(commandsDatabaseType, out var value))
		{
			return value;
		}
		throw new NotSupportedException();
	}
}
