using System;
using Dataedo.App.Tools.SqlCommands;
using MySqlConnector;

namespace Dataedo.App.Data.MySQL;

internal class LowerCaseTableNamesHelper
{
	private const string Binary = "binary";

	private readonly object connection;

	private string binaryKeyword;

	public LowerCaseTableNamesHelper(object connection)
	{
		this.connection = connection;
	}

	public string GetBinaryKeyword()
	{
		if (binaryKeyword == null)
		{
			SetBinaryKeyword();
		}
		return binaryKeyword;
	}

	private void SetBinaryKeyword()
	{
		try
		{
			using MySqlCommand mySqlCommand = CommandsWithTimeout.MySQL("select @@lower_case_table_names", connection);
			using MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
			if (mySqlDataReader.Read())
			{
				int @int = mySqlDataReader.GetInt32(0);
				binaryKeyword = ((@int == 0) ? "binary" : string.Empty);
			}
			else
			{
				binaryKeyword = "binary";
			}
		}
		catch (Exception)
		{
			binaryKeyword = "binary";
		}
	}
}
