using System;

namespace Dataedo.App.Tools;

public class KeyTooltips
{
	public const string Name = "Name of the key/constraint in the source database.";

	public const string Columns = "Column(s) that define this primary/unique key.";

	public static readonly string Description = "Description of key that can be edited with Dataedo. Description is imported from the source at first import and then it’s maintained in Dataedo." + Environment.NewLine + "You can add multiple descriptive fields using “Custom fields” option. ";
}
