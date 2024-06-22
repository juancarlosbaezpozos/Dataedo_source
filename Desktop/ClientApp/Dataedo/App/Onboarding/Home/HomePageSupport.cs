using Dataedo.App.Data.MetadataServer;

namespace Dataedo.App.Onboarding.Home;

public class HomePageSupport
{
	public static bool CheckIsAnyDatabaseIsImportedByUser()
	{
		return DB.Database.CheckIfIsUserImportedDatabase(null);
	}
}
