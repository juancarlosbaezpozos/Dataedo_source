using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.Data.Commands.Enums;
using Dataedo.Data.Factories;
using Dataedo.DataProcessing.Classes;
using Dataedo.LicenseHelperLibrary.Repository;
using Dataedo.Model.Data.Repository;

namespace Dataedo.App.Forms.Tools;

public class RepositoriesDBHelper
{
	public static IEnumerable<string> GetRepositories()
	{
		try
		{
			List<string> list = new List<string>();
			foreach (string repository in CommandsFactory.GetCommands(StaticData.IsProjectFile ? DatabaseType.SqlServerCe : DatabaseType.SqlServer, StaticData.DataedoConnectionString).Select.Repository.GetRepositories())
			{
				list.Add(repository);
			}
			return list;
		}
		catch (Exception)
		{
			throw;
		}
	}

	public static RepositoryData GetRepositoryData(string database)
	{
		try
		{
			return CommandsFactory.GetCommands(StaticData.IsProjectFile ? DatabaseType.SqlServerCe : DatabaseType.SqlServer, StaticData.DataedoConnectionString).Select.Repository.GetRepositoryData(database);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static RepositoryData GetRepositoryData(string database, bool isProjectFile, string connectionString)
	{
		try
		{
			return CommandsFactory.GetCommands(isProjectFile ? DatabaseType.SqlServerCe : DatabaseType.SqlServer, connectionString).Select.Repository.GetRepositoryData(database);
		}
		catch (Exception)
		{
			return null;
		}
	}

	public static bool CheckRepository(string targetRepositoryName, Form owner = null)
	{
		if (targetRepositoryName.Equals(StaticData.Database))
		{
			return true;
		}
		try
		{
			RepositoryObject repositoryObject = CommandsFactory.GetCommands(StaticData.IsProjectFile ? DatabaseType.SqlServerCe : DatabaseType.SqlServer, StaticData.DataedoConnectionString).Select.Repository.CheckRepository(targetRepositoryName).FirstOrDefault();
			RepositoryVersion repositoryVersion = new RepositoryVersion(PrepareValue.ToInt(repositoryObject.Version).GetValueOrDefault(), PrepareValue.ToInt(repositoryObject.Update).GetValueOrDefault(), PrepareValue.ToInt(repositoryObject.Release).GetValueOrDefault(), stable: true);
			RepositoryVersion repositoryVersion2 = StaticData.LicenseHelper.GetRepositoryVersion(StaticData.DataedoConnectionString);
			if (repositoryVersion > repositoryVersion2)
			{
				GeneralMessageBoxesHandling.Show($"Selected repository has too high version to copy descriptions.{Environment.NewLine}Upgrade current repository to Dataedo {repositoryVersion.Version}.{repositoryVersion.Update}.{repositoryVersion.Build} before continuing. ", "Repository not compatible", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
				return false;
			}
			if (repositoryVersion < repositoryVersion2)
			{
				GeneralMessageBoxesHandling.Show("Selected repository has too low version to copy descriptions." + Environment.NewLine + "Connect to it to upgrade it before continuing.", "Repository not compatible", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
				return false;
			}
			if (CaseSensitiveCollationIsDifferentInThoseRepositories(targetRepositoryName))
			{
				GeneralMessageBoxesHandling.Show("We can’t copy descriptions between databases with different collations." + Environment.NewLine + "Source: <b>" + StaticData.Database + "</b> Destination: <b>" + targetRepositoryName + "</b>", "Repository not compatible", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
				return false;
			}
			return true;
		}
		catch (Exception)
		{
			GeneralMessageBoxesHandling.Show("Selected database is not a repository or you don't have access to it." + Environment.NewLine + "Choose a correct repository to copy descriptions to.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
			return false;
		}
	}

	public static IEnumerable<RepositoryDocumentationItem> GetRepositoryDocumentations(string repository)
	{
		try
		{
			List<RepositoryDocumentationItem> list = new List<RepositoryDocumentationItem>();
			foreach (RepositoryDocumentationObject repositoryDocumentation in CommandsFactory.GetCommands(StaticData.IsProjectFile ? DatabaseType.SqlServerCe : DatabaseType.SqlServer, StaticData.DataedoConnectionString).Select.Repository.GetRepositoryDocumentations(repository))
			{
				list.Add(new RepositoryDocumentationItem(repositoryDocumentation.Title, repositoryDocumentation.Type, PrepareValue.ToInt(repositoryDocumentation.DatabaseId) ?? (-1), repositoryDocumentation.DBMSVersion));
			}
			return list.Where((RepositoryDocumentationItem x) => x.CanBeUsedForCopying).ToList();
		}
		catch (Exception)
		{
			throw;
		}
	}

	public static bool GetIsWebPortalConnected()
	{
		try
		{
			new List<string>();
			return CommandsFactory.GetCommands(StaticData.IsProjectFile ? DatabaseType.SqlServerCe : DatabaseType.SqlServer, StaticData.DataedoConnectionString).Select.Repository.GetIsWebPortalConnected();
		}
		catch (Exception)
		{
			throw;
		}
	}

	private static bool CaseSensitiveCollationIsDifferentInThoseRepositories(string destinationRepo)
	{
		if (StaticData.Database != destinationRepo)
		{
			List<string> databasesCollation = DB.Database.GetDatabasesCollation(StaticData.Database, destinationRepo);
			if (databasesCollation.Count() == 2)
			{
				return databasesCollation[0] != databasesCollation[1];
			}
			return false;
		}
		return false;
	}
}
