using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Dataedo.Repository.Services.Services;

namespace Dataedo.Api.Enums.Data;

/// <summary>
/// The class providing enumeration of database types.
/// </summary>
[DataContract]
public class DatabaseTypeEnum : BaseEnumConversions<DatabaseTypeEnum.DatabaseType>
{
	/// <summary>
	/// Specifies applicable database types.
	/// <para>
	/// When using <see cref="T:Dataedo.Api.Enums.Data.DatabaseTypeEnum.DatabaseType" /> for JSON serialization use <code>[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]</code> attribute to make results contain <see cref="T:System.Runtime.Serialization.EnumMemberAttribute" /> EnumMember value instead of number value.
	/// </para>
	/// </summary>
	public enum DatabaseType
	{
		/// <summary>
		/// An Aurora database.
		/// </summary>
		[EnumMember(Value = "Aurora")]
		Aurora = 0,
		/// <summary>
		/// An Aurora PostgreSQL database.
		/// </summary>
		[EnumMember(Value = "AuroraPostgreSQL")]
		AuroraPostgreSQL = 1,
		/// <summary>
		/// A Azure SQL Database database.
		/// </summary>
		[EnumMember(Value = "AzureSQLDatabase")]
		AzureSQLDatabase = 2,
		/// <summary>
		/// A Azure SQL DataWarehouse database.
		/// </summary>
		[EnumMember(Value = "AzureSQLDataWarehouse")]
		AzureSQLDataWarehouse = 3,
		/// <summary>
		/// A DB2 database.
		/// </summary>
		[EnumMember(Value = "DB2")]
		DB2 = 4,
		/// <summary>
		/// A IBM DB2 Big Query database.
		/// </summary>
		[EnumMember(Value = "IBMDb2BigQuery")]
		IBMDb2BigQuery = 5,
		/// <summary>
		/// A Manual (user-defined) database.
		/// </summary>
		[EnumMember(Value = "Manual")]
		Manual = 6,
		/// <summary>
		/// A MariaDB database.
		/// </summary>
		[EnumMember(Value = "MariaDB")]
		MariaDB = 7,
		/// <summary>
		/// A MySQL database.
		/// </summary>
		[EnumMember(Value = "MySQL")]
		MySQL = 8,
		/// <summary>
		/// A MySQL 8 database.
		/// </summary>
		[EnumMember(Value = "MySQL8")]
		MySQL8 = 9,
		/// <summary>
		/// A database imported using ODBC.
		/// </summary>
		[EnumMember(Value = "ODBC")]
		ODBC = 10,
		/// <summary>
		/// A Oracle database.
		/// </summary>
		[EnumMember(Value = "Oracle")]
		Oracle = 11,
		/// <summary>
		/// A Percona MySQL database.
		/// </summary>
		[EnumMember(Value = "PerconaMySQL")]
		PerconaMySQL = 12,
		/// <summary>
		/// A Percona MySQL 8 database.
		/// </summary>
		[EnumMember(Value = "PerconaMySQL8")]
		PerconaMySQL8 = 13,
		/// <summary>
		/// A PostgreSQL database.
		/// </summary>
		[EnumMember(Value = "PostgreSQL")]
		PostgreSQL = 14,
		/// <summary>
		/// A Redshift database.
		/// </summary>
		[EnumMember(Value = "Redshift")]
		Redshift = 15,
		/// <summary>
		/// A Snowflake database.
		/// </summary>
		[EnumMember(Value = "Snowflake")]
		Snowflake = 16,
		/// <summary>
		/// A SQL Server database.
		/// </summary>
		[EnumMember(Value = "SqlServer")]
		SqlServer = 17
	}

	/// <summary>
	/// The dictionary used for conversion from repository representation of database type to enumeration value.
	/// </summary>
	private static readonly Dictionary<string, DatabaseType> RepositoryValueToEnumDictionary = new Dictionary<string, DatabaseType>
	{
		{
			"AURORA",
			DatabaseType.Aurora
		},
		{
			"AURORA_POSTGRESQL",
			DatabaseType.AuroraPostgreSQL
		},
		{
			"AZURE_SQL_DATABASE",
			DatabaseType.AzureSQLDatabase
		},
		{
			"AZURE_SQL_DATA_WAREHOUSE",
			DatabaseType.AzureSQLDataWarehouse
		},
		{
			"DB2",
			DatabaseType.DB2
		},
		{
			"IBM_DB2_Big_Query",
			DatabaseType.IBMDb2BigQuery
		},
		{
			"MANUAL",
			DatabaseType.Manual
		},
		{
			"MARIADB",
			DatabaseType.MariaDB
		},
		{
			"MYSQL",
			DatabaseType.MySQL
		},
		{
			"MYSQL8",
			DatabaseType.MySQL8
		},
		{
			"ODBC",
			DatabaseType.ODBC
		},
		{
			"ORACLE",
			DatabaseType.Oracle
		},
		{
			"PERCONA_MYSQL",
			DatabaseType.PerconaMySQL
		},
		{
			"PERCONA_MYSQL8",
			DatabaseType.PerconaMySQL8
		},
		{
			"POSTGRESQL",
			DatabaseType.PostgreSQL
		},
		{
			"REDSHIFT",
			DatabaseType.Redshift
		},
		{
			"SNOWFLAKE",
			DatabaseType.Snowflake
		},
		{
			"SQLSERVER",
			DatabaseType.SqlServer
		},
		{
			"SQLSERVER_2000",
			DatabaseType.SqlServer
		}
	};

	/// <summary>
	/// Converts text representation from repository value to enumeration value.
	/// </summary>
	/// <param name="value">The text representation from repository value.</param>
	/// <returns>The enumeration value.</returns>;
	/// <exception cref="T:System.ArgumentException">Thrown when value does not match enumeration value.</exception>
	public static DatabaseType ToEnumFromRepositoryValue(string value)
	{
		if (RepositoryValueToEnumDictionary.TryGetValue(value, out var result))
		{
			return result;
		}
		throw new ArgumentException("The value is invalid. Provided value: \"" + value + "\".");
	}
}
