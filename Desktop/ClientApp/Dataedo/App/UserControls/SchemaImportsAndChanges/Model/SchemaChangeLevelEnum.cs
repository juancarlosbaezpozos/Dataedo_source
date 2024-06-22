namespace Dataedo.App.UserControls.SchemaImportsAndChanges.Model;

public class SchemaChangeLevelEnum
{
	public enum SchemaChangeLevel
	{
		NotSet = 0,
		NoResults = 1,
		LicenseWitoutSCT = 2,
		Root = 3,
		Date = 4,
		Operation = 5,
		Object = 6,
		Element = 7
	}
}
