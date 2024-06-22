using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classes;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize.DatabaseRows;

internal class PostgresDatabaseRow : DatabaseRow
{
	public PostgresDatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string name, string title, string host, string user, string password, object port, int? id, string filter, string dbmsVersion, string sslType, SSLSettings sslSettings)
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
		base.SSLType = sslType;
		base.SSLSettings = sslSettings ?? new SSLSettings();
		base.Param1 = base.SSLSettings.CertPassword;
	}
}
