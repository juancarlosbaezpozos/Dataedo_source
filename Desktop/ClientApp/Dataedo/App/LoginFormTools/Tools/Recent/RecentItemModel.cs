using System;
using System.Drawing;
using System.IO;
using Dataedo.App.Properties;
using RecentProjectsLibrary;

namespace Dataedo.App.LoginFormTools.Tools.Recent;

public class RecentItemModel
{
	public RecentProject Data { get; set; }

	public RepositoryType Type => Data.Type;

	public string Server => Data.Server;

	public string Database => Data.Database;

	public Image Legacy
	{
		get
		{
			if (Type != 0)
			{
				return null;
			}
			return Resources.Legacy;
		}
	}

	public string DisplayName
	{
		get
		{
			if (Type == RepositoryType.Repository)
			{
				return Database + "@" + Server;
			}
			if (Type == RepositoryType.File)
			{
				try
				{
					return Path.GetFileName(Database);
				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
			return string.Empty;
		}
	}

	public Bitmap Icon
	{
		get
		{
			if (Type == RepositoryType.Repository)
			{
				return Resources.server_repository_32;
			}
			if (Type == RepositoryType.File)
			{
				return Resources.file_repository_32;
			}
			return Resources.unresolved_32;
		}
	}

	public string Descrption
	{
		get
		{
			if (Type == RepositoryType.Repository)
			{
				return Database + "@" + Server;
			}
			if (Type == RepositoryType.File)
			{
				try
				{
					return Path.GetDirectoryName(Database) + Path.GetFileName(Database);
				}
				catch (Exception)
				{
					return string.Empty;
				}
			}
			return string.Empty;
		}
	}

	public RecentItemModel(RecentProject recentProject)
	{
		Data = recentProject;
	}
}
