namespace Dataedo.App.Tools;

public class BulkObjectsHelper : BulkHelper
{
	public BulkObjectsHelper()
	{
		base.Template = "Name\tColumn name\tData type\tSize\tNullable\tTitle\tDescription";
	}
}
