namespace Dataedo.App.Tools;

public class BulkViewsHelper : BulkHelper
{
	public BulkViewsHelper()
	{
		base.Template = "Schema\tView name\tColumn name\tData type\tSize\tNullable\tDefault value\tComputedFormula\tIdentity\tTitle\tDescription";
	}
}
