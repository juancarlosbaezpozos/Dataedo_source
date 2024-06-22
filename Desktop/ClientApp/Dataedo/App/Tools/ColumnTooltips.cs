using System;

namespace Dataedo.App.Tools;

public class ColumnTooltips
{
	public static readonly string Key = "Icons indicate whether column is part of primary or unique key. You can define your own keys in Dataedo metadata repository. This information will not impact your source database." + Environment.NewLine + "To add key, select column(s) and choose “Add primary key” or “Add unique key” options from context menu or the ribbon";

	public const string OrdinalPosition = "Ordinal position of columns in source database. You can reorder columns in Dataedo using Designer.Reordering will not impact table in your source database.";

	public const string Name = "Name of the column/ field in source database. You can provide user/ business friendly alias using Title field. Title will be displayed next to column names in Dataedo shared documentation.";

	public const string DataType = "Data type of column/field as defined in the source database/dataset.";

	public static readonly string References = "Foreign keys, tables/ objects that are referenced by values in this column. This information is extracted automatically from the source database from foreign key constraints. You can define your own relationships to document how tables can be joined, even across different databases or platforms.Adding new relationships will not impact the source database." + Environment.NewLine + "To add a relationship, select column(s) and choose “Add relationship” from context menu or the ribbon." + Environment.NewLine + "Hover on the table name to see relationship details." + Environment.NewLine + "Right click and choose “Go to…” option to go to related table form.";

	public const string Nullable = "Read - only indicator whether column is nullable in the source database.You can use custom fields to provide additional nullability information.";

	public static readonly string DataLink = "Dataedo allows you to build a glossary of business terms with their definitions that will help you establish common language and facilitate understanding of key concepts and metrics across all departments, applications and bridge gap between IT and business. You can map glossary terms to specific columns/fields and tables/objects." + Environment.NewLine + "Hover on term name to see the definition." + Environment.NewLine + "Right click and choose “Go to…” option to go to linked term detailed form." + Environment.NewLine + "Right click and choose “Add term…” to create new linked term." + Environment.NewLine + "Right click and choose “Edit links…” to link column to existing term." + Environment.NewLine + "Go to “Linked terms” tabs to see all terms linked with current table / object.";

	public const string Identity = "Read-only indicator whether column is identity/autoincrement/serial in the source database. You can use custom fields to provide additional information.";

	public const string Default = "Read-only definition of computed columns and default values imported from the source database. You can use custom fields to provide additional information about calculations, rules, etc.";

	public const string Created = "Data and time when column was first imported or manually created in Dataedo metadata repository. ";

	public const string CreatedBy = "Dataedo user name who first imported or manually created in Dataedo metadata repository.";

	public const string LastUpdated = "Data and time when any column/field data was last updated in Dataedo metadata repository.";

	public const string LastUpdatedBy = "Dataedo user name who last updated in Dataedo metadata repository.";
}
