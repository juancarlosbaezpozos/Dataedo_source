namespace Dataedo.App.Tools;

public class BulkObjectColumnsHelper : ColumnBulkHelper
{
	public BulkObjectColumnsHelper()
	{
		base.Template = "Name\tData type\tSize\tNullable\tTitle\tDescription";
	}
}
