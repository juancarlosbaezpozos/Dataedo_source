using System;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using Cassandra;
using Dataedo.App.Data.Neo4j;
using Dataedo.App.Data.Tableau;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.Data.Tools;
using Dataedo.DBTConnector;
using Devart.Data.Universal;
using Google.Cloud.BigQuery.V2;
using Microsoft.AnalysisServices;
using Microsoft.Azure.Cosmos;
using Microsoft.Data.SqlClient;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;
using MongoDB.Driver;
using MySqlConnector;
using Neo4j.Driver;
using Nest;
using NetSuite.SuiteAnalyticsConnect;
using Npgsql;
using Salesforce.Force;
using Sap.Data.Hana;
using Snowflake.Data.Client;
using Sybase.Data.AseClient;
using Teradata.Client.Provider;
using Vertica.Data.VerticaClient;

namespace Dataedo.App.Tools.SqlCommands;

public static class CommandsWithTimeout
{
	private static bool useTimeout = true;

	private static int timeout = defaultTimeout;

	private static int defaultTimeout => LastConnectionInfo.LOGIN_INFO?.ConnectionTimeout ?? 120;

	public static int Timeout
	{
		get
		{
			return timeout;
		}
		set
		{
			if (value < 1)
			{
				timeout = 1;
				return;
			}
			timeout = value;
			useTimeout = true;
			if (LastConnectionInfo.LOGIN_INFO != null)
			{
				LastConnectionInfo.SetConnectionTimeout(timeout);
			}
		}
	}

	public static IDbCommand Snowflake(string commandText, object conn)
	{
		if (!(conn is SnowflakeDbConnection snowflakeDbConnection))
		{
			return null;
		}
		if (snowflakeDbConnection.State != ConnectionState.Open)
		{
			snowflakeDbConnection.Open();
		}
		DbCommand dbCommand = snowflakeDbConnection.CreateCommand();
		((IDbCommand)dbCommand).CommandText = commandText;
		((IDbCommand)dbCommand).CommandTimeout = Timeout;
		return dbCommand;
	}

	public static void SetDefaultTimeout()
	{
		Timeout = defaultTimeout;
		useTimeout = true;
	}

	public static void SetNotToUseTimeout()
	{
		useTimeout = false;
	}

	private static UniCommand UniConnectionDatabase(string commandText, object conn)
	{
		if (!(conn is UniConnection uniConnection))
		{
			return null;
		}
		if (uniConnection.State != ConnectionState.Open)
		{
			uniConnection.Open();
		}
		return new UniCommand(commandText, uniConnection)
		{
			CommandTimeout = Timeout
		};
	}

	public static MySqlCommand MySQL(string commandText, object conn)
	{
		if (!(conn is MySqlConnection mySqlConnection))
		{
			return null;
		}
		if (mySqlConnection.State != ConnectionState.Open)
		{
			mySqlConnection.Open();
		}
		return new MySqlCommand(commandText, mySqlConnection)
		{
			CommandTimeout = Timeout
		};
	}

	public static SqlCommand SqlServer(string commandText, object conn)
	{
		if (!(conn is SqlConnection sqlConnection))
		{
			return null;
		}
		if (sqlConnection.State != ConnectionState.Open)
		{
			sqlConnection.Open();
		}
		return new SqlCommand(commandText, sqlConnection)
		{
			CommandTimeout = Timeout
		};
	}

	public static SqlCommand SqlServerForRepository(string commandText, object conn)
	{
		if (!(conn is SqlConnection sqlConnection))
		{
			return null;
		}
		if (sqlConnection.State != ConnectionState.Open)
		{
			sqlConnection.Open();
		}
		return new SqlCommand(commandText, sqlConnection)
		{
			CommandTimeout = CommandsTimeout.Timeout
		};
	}

	public static UniCommand Oracle(string commandText, object conn)
	{
		return UniConnectionDatabase(commandText, conn);
	}

	public static NpgsqlCommand PostgreSql(string commandText, object conn)
	{
		if (!(conn is NpgsqlConnection npgsqlConnection))
		{
			return null;
		}
		if (npgsqlConnection.State != ConnectionState.Open)
		{
			npgsqlConnection.Open();
		}
		return new NpgsqlCommand(commandText, npgsqlConnection)
		{
			CommandTimeout = Timeout
		};
	}

	public static OdbcCommand Odbc(string commandText, object conn)
	{
		if (!(conn is OdbcConnection odbcConnection))
		{
			return null;
		}
		if (odbcConnection.State != ConnectionState.Open)
		{
			odbcConnection.Open();
		}
		return new OdbcCommand(commandText, odbcConnection)
		{
			CommandTimeout = Timeout
		};
	}

	public static NpgsqlCommand Redshift(string commandText, object conn)
	{
		if (!(conn is NpgsqlConnection npgsqlConnection))
		{
			return null;
		}
		if (npgsqlConnection.State != ConnectionState.Open)
		{
			npgsqlConnection.Open();
		}
		return new NpgsqlCommand(commandText, npgsqlConnection)
		{
			CommandTimeout = Timeout
		};
	}

	public static VerticaCommand Vertica(string commandText, object conn)
	{
		if (!(conn is VerticaConnection verticaConnection))
		{
			return null;
		}
		if (verticaConnection.State != ConnectionState.Open)
		{
			verticaConnection.Open();
		}
		return new VerticaCommand(commandText, verticaConnection)
		{
			CommandTimeout = Timeout
		};
	}

	public static TdCommand Teradata(string commandText, object conn)
	{
		if (!(conn is TdConnection tdConnection))
		{
			return null;
		}
		if (tdConnection.State != ConnectionState.Open)
		{
			tdConnection.Open();
		}
		return new TdCommand(commandText, tdConnection)
		{
			CommandTimeout = Timeout
		};
	}

	public static MongoClient MongoDb(object conn)
	{
		return conn as MongoClient;
	}

	public static MongoClient MongoDb(string connectionString)
	{
		if (useTimeout)
		{
			MongoUrlBuilder mongoUrlBuilder = new MongoUrlBuilder(connectionString);
			TimeSpan timeSpan5 = (mongoUrlBuilder.WaitQueueTimeout = (mongoUrlBuilder.SocketTimeout = (mongoUrlBuilder.ServerSelectionTimeout = (mongoUrlBuilder.ConnectTimeout = new TimeSpan(0, 0, Timeout)))));
			return new MongoClient(mongoUrlBuilder.ToString());
		}
		return new MongoClient(connectionString);
	}

	public static global::Cassandra.ISession Cassandra(object conn)
	{
		return conn as global::Cassandra.ISession;
	}

	public static ElasticClient Elasticsearch(object conn)
	{
		return conn as ElasticClient;
	}

	public static Neo4jSession Neo4j(object conn)
	{
		return conn as Neo4jSession;
	}

	public static IResult Neo4j(object conn, string command)
	{
		return Neo4j(conn).Run(command);
	}

	public static BigQueryClient GoogleBigQuery(object conn)
	{
		return conn as BigQueryClient;
	}

	public static BigQueryResults GoogleBigQuery(object conn, string command)
	{
		GetQueryResultsOptions getQueryResultsOptions = new GetQueryResultsOptions();
		getQueryResultsOptions.Timeout = new TimeSpan(0, 0, Timeout);
		return GoogleBigQuery(conn).ExecuteQuery(command, null, null, getQueryResultsOptions);
	}

	public static Server SSASTabular(object conn)
	{
		if (!(conn is Server server))
		{
			return null;
		}
		if (!server.Connected)
		{
			server.Reconnect();
		}
		return server;
	}

	public static CosmosClient CosmosDB(object conn)
	{
		return conn as CosmosClient;
	}

	public static CosmosClient CosmosDB(string connectionString)
	{
		return new CosmosClient(connectionString, new CosmosClientOptions
		{
			ApplicationName = "Dataedo"
		});
	}

	public static ForceClient Salesforce(object conn)
	{
		return conn as ForceClient;
	}

	public static AseCommand SapAse(string commandText, object conn)
	{
		if (!(conn is AseConnection aseConnection))
		{
			return null;
		}
		if (aseConnection.State != ConnectionState.Open)
		{
			aseConnection.Open();
		}
		return new AseCommand(commandText, aseConnection)
		{
			CommandTimeout = Timeout
		};
	}

	public static AthenaConnectionWrapper Athena(object conn)
	{
		if (!(conn is AthenaConnectionWrapper athenaConnectionWrapper) || athenaConnectionWrapper.Client == null)
		{
			return null;
		}
		return athenaConnectionWrapper;
	}

	public static TableauConnection Tableau(object conn)
	{
		return conn as TableauConnection;
	}

	public static IOrganizationService Dataverse(object conn)
	{
		if (!(conn is CrmServiceClient crmServiceClient))
		{
			return null;
		}
		IOrganizationService organizationWebProxyClient = crmServiceClient.OrganizationWebProxyClient;
		return organizationWebProxyClient ?? crmServiceClient.OrganizationServiceProxy;
	}

	public static HanaCommand SapHana(string commandText, object conn)
	{
		if (!(conn is HanaConnection hanaConnection))
		{
			return null;
		}
		if (hanaConnection.State != ConnectionState.Open)
		{
			hanaConnection.Open();
		}
		return new HanaCommand(commandText, hanaConnection)
		{
			CommandTimeout = Timeout
		};
	}

	public static OpenAccessCommand NetSuiteCommand(string commandText, object conn)
	{
		if (!(conn is OpenAccessConnection openAccessConnection))
		{
			return null;
		}
		if (openAccessConnection.State != ConnectionState.Open)
		{
			openAccessConnection.Open();
		}
		return new OpenAccessCommand(commandText, openAccessConnection)
		{
			CommandTimeout = Timeout
		};
	}

	public static DBTPackage DBT(object conn)
	{
		return conn as DBTPackage;
	}
}
