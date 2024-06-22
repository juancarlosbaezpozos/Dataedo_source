using System.Collections.Generic;
using Dataedo.App.LoginFormTools.Tools.Recent;
using Dataedo.App.LoginFormTools.Tools.ScriptsSupport;
using Dataedo.LicenseHelperLibrary.Repository;

namespace Dataedo.App.LoginFormTools.Tools.Repository;

public class RepositoryOperation
{
	public List<Instruction> CreateDatabaseInstructions { get; }

	public List<Instruction> Instructions { get; private set; }

	public string ConnectionString { get; private set; }

	public string Database { get; private set; }

	public RecentItemModel RecentItemModel { get; private set; }

	public RepositoryVersion RepositoryVersion { get; private set; }

	public string DatabaseConnectionString { get; private set; }

	public RepositoryOperation(List<Instruction> instructions, string connectionString, string database, RecentItemModel recentItemModel)
	{
		Instructions = instructions;
		ConnectionString = connectionString;
		Database = database;
		RecentItemModel = recentItemModel;
	}

	public RepositoryOperation(List<Instruction> createDatabaseInstructions, List<Instruction> instructions, string connectionString, string database, RecentItemModel recentItemModel, string databaseConnectionString)
		: this(instructions, connectionString, database, recentItemModel)
	{
		CreateDatabaseInstructions = createDatabaseInstructions;
		Instructions = instructions;
		ConnectionString = connectionString;
		Database = database;
		RecentItemModel = recentItemModel;
		DatabaseConnectionString = databaseConnectionString;
	}

	public RepositoryOperation(List<Instruction> instructions, string connectionString, string database, RecentItemModel recentItemModel, RepositoryVersion repositoryVersion)
		: this(instructions, connectionString, database, recentItemModel)
	{
		RepositoryVersion = repositoryVersion;
	}
}
