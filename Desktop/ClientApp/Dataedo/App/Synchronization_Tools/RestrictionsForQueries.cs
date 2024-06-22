using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dataedo.App.Classes.Synchronize;

namespace Dataedo.App.Synchronization_Tools;

public class RestrictionsForQueries
{
	public static int maxObjects = 100;

	public static string JoinObjectsNames(IEnumerable<ObjectRow> rows)
	{
		int num = rows.Count();
		if (num > 0 && num < maxObjects)
		{
			StringBuilder tableNamesSb = new StringBuilder();
			rows.ToList().ForEach(delegate(ObjectRow x)
			{
				if (string.IsNullOrEmpty(x.Schema))
				{
					tableNamesSb.Append("'").Append(RepairParameterNames(x.Name)).Append("', ");
				}
				else
				{
					tableNamesSb.Append("'").Append(RepairParameterNames(x.Schema)).Append(".")
						.Append(RepairParameterNames(x.Name))
						.Append("', ");
				}
			});
			if (num > 0)
			{
				tableNamesSb.Remove(tableNamesSb.Length - 2, 2);
			}
			return tableNamesSb.ToString();
		}
		return null;
	}

	public static string JoinObjectsNamesForMySQL(IEnumerable<ObjectRow> rows)
	{
		int num = rows.Count();
		if (num > 0 && num < maxObjects)
		{
			StringBuilder tableNamesSb = new StringBuilder();
			rows.ToList().ForEach(delegate(ObjectRow x)
			{
				tableNamesSb.Append("'").Append(RepairParameterNames(x.DatabaseName)).Append(".")
					.Append(RepairParameterNames(x.Name))
					.Append("', ");
			});
			if (num > 0)
			{
				tableNamesSb.Remove(tableNamesSb.Length - 2, 2);
			}
			return tableNamesSb.ToString();
		}
		return null;
	}

	private static string RepairParameterNames(string parameter)
	{
		return parameter?.Replace("'", "''");
	}
}
