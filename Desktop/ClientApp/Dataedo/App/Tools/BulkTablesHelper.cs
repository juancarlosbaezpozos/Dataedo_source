namespace Dataedo.App.Tools;

public class BulkTablesHelper : BulkHelper
{
	public BulkTablesHelper()
	{
		base.Template = "Schema\tTable name\tColumn name\tData type\tSize\tNullable\tDefault value\tComputedFormula\tIdentity\tTitle\tDescription";
	}
}
