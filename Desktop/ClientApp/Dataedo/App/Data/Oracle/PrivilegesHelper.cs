using System;
using System.Collections.Generic;
using System.Data;
using Dataedo.App.Tools.SqlCommands;
using Devart.Data.Universal;

namespace Dataedo.App.Data.Oracle;

public class PrivilegesHelper
{
	private const string DbaPrefix = "DBA";

	private const string AllPrefix = "ALL";

	private readonly Dictionary<string, bool> objectsPrefixes = new Dictionary<string, bool>();

	private readonly object connection;

	public PrivilegesHelper(object connection)
	{
		this.connection = connection;
	}

	public string GetName(string objectNameBase)
	{
		if (!objectsPrefixes.ContainsKey(objectNameBase))
		{
			objectsPrefixes.Add(objectNameBase, HasDbaPrivileges(objectNameBase));
		}
		return CreateName(objectNameBase, objectsPrefixes[objectNameBase]);
	}

	private bool HasDbaPrivileges(string objectNameBase)
	{
		try
		{
			string text = CreateName(objectNameBase, hasDbaPrivileges: true);
			using UniCommand uniCommand = CommandsWithTimeout.Oracle("SELECT 1 FROM " + text + " WHERE 0 = 1", connection);
			using UniDataReader uniDataReader = uniCommand.ExecuteReader(CommandBehavior.CloseConnection);
			uniDataReader.Read();
			return true;
		}
		catch (Exception)
		{
			return false;
		}
	}

	private string CreateName(string objectNameBase, bool hasDbaPrivileges)
	{
		return (hasDbaPrivileges ? "DBA" : "ALL") + "_" + objectNameBase;
	}
}
