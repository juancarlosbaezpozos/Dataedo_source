using System.Collections.Generic;
using System.Drawing;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Properties;

namespace Dataedo.App.Documentation;

public class DocumentationWithObjectTypes
{
	public DatabaseRow Documentation { get; set; }

	public List<ObjectTypeSelection> ObjectTypes { get; set; }

	public bool IsSelected { get; set; }

	public Image DocumentationImage { get; set; } = Resources.documentation_16;


	public bool IsForAddingNewDatabase { get; set; }
}
