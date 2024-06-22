using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class TableauDatabaseRow : DatabaseRow
{
	public TableauDatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string name, string title, string host, string user, string password, string token, int? id, string filter, string dbmsVersion, AuthenticationType.AuthenticationTypeEnum? authentication, Dataedo.Shared.Enums.TableauProduct.TableauProductEnum? connectionMode)
	{
		base.Type = databaseType;
		base.IsValidDatabase = true;
		base.UseDifferentSchema = null;
		Name = name;
		base.Title = title;
		base.Host = host;
		base.User = user;
		base.Password = password;
		base.ServiceName = token;
		base.Id = id;
		base.Filter.SetRulesFromXml(filter);
		base.DbmsVersion = dbmsVersion;
		base.SelectedAuthenticationType = authentication ?? AuthenticationType.AuthenticationTypeEnum.StandardAuthentication;
		base.Param1 = Dataedo.Shared.Enums.TableauProduct.TypeToString(connectionMode);
	}
}
