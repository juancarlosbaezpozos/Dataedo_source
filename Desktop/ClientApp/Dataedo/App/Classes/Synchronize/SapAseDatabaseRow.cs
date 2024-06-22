using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class SapAseDatabaseRow : DatabaseRow
{
	public SapAseDatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string name, string title, string host, string user, string password, object port, int? id, string filter, string dbmsVersion, string charset)
	{
		base.Type = databaseType;
		base.IsValidDatabase = true;
		base.UseDifferentSchema = null;
		base.HasMultipleSchemas = null;
		Name = name;
		base.Title = title;
		base.Host = host;
		base.User = user;
		base.Password = password;
		base.Port = PrepareValue.ToInt(port);
		base.Id = id;
		base.Filter.SetRulesFromXml(filter);
		base.DbmsVersion = dbmsVersion;
		base.Param1 = charset;
	}
}
