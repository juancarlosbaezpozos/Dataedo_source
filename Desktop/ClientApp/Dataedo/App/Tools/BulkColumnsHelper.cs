namespace Dataedo.App.Tools;

public class BulkColumnsHelper : ColumnBulkHelper
{
	public BulkColumnsHelper()
	{
		base.Template = "Name\tData type\tSize\tNullable\tDefault value\tComputedFormula\tIdentity\tTitle\tDescription";
	}
}
