namespace Dataedo.App.Forms.Support.DocWizardForm;

public class CustomExcludedTypeEnum
{
	public enum CustomExcludedType
	{
		Documentation = 0,
		DatabaseName = 1,
		HostName = 2
	}

	public static string TypeToString(CustomExcludedType? type)
	{
		return type switch
		{
			CustomExcludedType.Documentation => "Documentation", 
			CustomExcludedType.DatabaseName => "Database name", 
			CustomExcludedType.HostName => "Host name", 
			_ => string.Empty, 
		};
	}
}
