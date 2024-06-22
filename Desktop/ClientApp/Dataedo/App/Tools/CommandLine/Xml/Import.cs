using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.Xml.Commands.Base;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ImportCommand.CustomFields;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.UserControls.ImportFilter;

namespace Dataedo.App.Tools.CommandLine.Xml;

public class Import : RepositoryDocumentationCommandBase
{
	[XmlElement]
	public bool FullReimport { get; set; }

	[XmlAnyElement(Name = "UpdateEntireDocumentationComment")]
	public XmlCommentObject UpdateEntireDocumentationComment { get; set; } = new XmlCommentObject("This is a legacy field, do not alter this value.");


	public bool UpdateEntireDocumentation { get; set; }

	[XmlAnyElement(Name = "SourceDatabaseConnectionComment")]
	public XmlCommentObject SourceDatabaseConnectionComment { get; set; } = new XmlCommentObject("Database connection");


	[XmlElement(Type = typeof(SqlServerConnection), ElementName = "SqlServerSourceDatabase")]
	[XmlElement(Type = typeof(OracleConnection), ElementName = "OracleSourceDatabase")]
	[XmlElement(Type = typeof(MySQLConnection), ElementName = "MySQLSourceDatabase")]
	[XmlElement(Type = typeof(MySQL8Connection), ElementName = "MySQL8SourceDatabase")]
	[XmlElement(Type = typeof(MariaDBConnection), ElementName = "MariaDBSourceDatabase")]
	[XmlElement(Type = typeof(RedshiftConnection), ElementName = "RedshiftSourceDatabase")]
	[XmlElement(Type = typeof(SnowflakeConnection), ElementName = "SnowflakeSourceDatabase")]
	[XmlElement(Type = typeof(Db2Connection), ElementName = "Db2SourceDatabase")]
	[XmlElement(Type = typeof(OdbcConnection), ElementName = "OdbcSourceDatabase")]
	[XmlElement(Type = typeof(AzureSQLDatabaseConnection), ElementName = "AzureSQLDatabaseConnection")]
	[XmlElement(Type = typeof(AzureSQLDataWarehouseConnection), ElementName = "AzureSQLDataWarehouseConnection")]
	[XmlElement(Type = typeof(PostgreSQLConnection), ElementName = "PostgreSQLSourceDatabase")]
	[XmlElement(Type = typeof(VerticaConnection), ElementName = "VerticaSourceDatabase")]
	[XmlElement(Type = typeof(TeradataConnection), ElementName = "TeradataSourceDatabase")]
	[XmlElement(Type = typeof(MongoDBConnection), ElementName = "MongoDBSourceDatabase")]
	[XmlElement(Type = typeof(CassandraConnection), ElementName = "CassandraSourceDatabase")]
	[XmlElement(Type = typeof(GoogleBigQueryConnection), ElementName = "GoogleBigQuerySourceDatabase")]
	[XmlElement(Type = typeof(ElasticsearchConnection), ElementName = "ElasticsearchSourceDatabase")]
	[XmlElement(Type = typeof(SsasTabularConnection), ElementName = "SsasTabularSourceDatabase")]
	[XmlElement(Type = typeof(PowerBiDatasetConnection), ElementName = "PowerBiDatasetSourceDatabase")]
	[XmlElement(Type = typeof(CosmosDBSqlConnection), ElementName = "CosmosDBSourceDatabase")]
	[XmlElement(Type = typeof(CosmosDbCassandraApiConnection), ElementName = "CosmosDbCassandraApiSourceDatabase")]
	[XmlElement(Type = typeof(AmazonKeyspacesConnection), ElementName = "AmazonKeyspaceSourceDatabase")]
	[XmlElement(Type = typeof(HiveMetastoreMySQLConnection), ElementName = "HiveMetastoreMySQLSourceDatabase")]
	[XmlElement(Type = typeof(HiveMetastoreSQLServerConnection), ElementName = "HiveMetastoreSQLServerSourceDatabase")]
	[XmlElement(Type = typeof(SalesforceConnection), ElementName = "SalesforceSourceDatabase")]
	[XmlElement(Type = typeof(CosmosDbMongoApiConnection), ElementName = "CosmosDbMongoApiSourceDatabase")]
	[XmlElement(Type = typeof(HiveMetastoreMariaDBConnection), ElementName = "HiveMetastoreMariaDBSourceDatabase")]
	[XmlElement(Type = typeof(HiveMetastorePostgreSQLConnection), ElementName = "HiveMetastorePostgreSQLSourceDatabase")]
	[XmlElement(Type = typeof(HiveMetastoreAzureSQLConnection), ElementName = "HiveMetastoreAzureSQLSourceDatabase")]
	[XmlElement(Type = typeof(AthenaConnection), ElementName = "AthenaSourceDatabase")]
	[XmlElement(Type = typeof(SapAseConnection), ElementName = "SapAseSourceDatabase")]
	[XmlElement(Type = typeof(TableauConnection), ElementName = "TableauSourceDatabase")]
	[XmlElement(Type = typeof(DdlConnection), ElementName = "DdlSourceDatabase")]
	[XmlElement(Type = typeof(InterfaceTablesConnection), ElementName = "InterfaceTablesSourceDatabase")]
	[XmlElement(Type = typeof(DataverseConnection), ElementName = "DataverseSourceDatabase")]
	[XmlElement(Type = typeof(Dynamics365Connection), ElementName = "Dynamics365SourceDatabase")]
	[XmlElement(Type = typeof(DynamoDBConnection), ElementName = "DynamoDBSourceDatabase")]
	[XmlElement(Type = typeof(SapHanaConnection), ElementName = "SapHanaSourceDatabase")]
	[XmlElement(Type = typeof(AstraConnection), ElementName = "AstraSourceDatabase")]
	[XmlElement(Type = typeof(NetSuiteConnection), ElementName = "NetSuiteSourceDatabase")]
	[XmlElement(Type = typeof(DBTConnection), ElementName = "DBTSourceDatabase")]
	[XmlElement(Type = typeof(SSISConnection), ElementName = "SSISSourceDatabase")]
	public ConnectionBase SourceDatabaseConnection { get; set; }

	[XmlElement]
	public FilterRulesCollection FilterRules { get; set; }

	[XmlAnyElement(Name = "ExtendedPropertiesImportComment")]
	public XmlCommentObject ExtendedPropertiesImportComment { get; set; } = new XmlCommentObject("Applies only to SQL Server, Azure and Tableau.");


	[XmlElement]
	public ExtendedPropertiesImportBase ExtendedPropertiesImport { get; set; }
}
