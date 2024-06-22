using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;

namespace Dataedo.App.Documentation;

public class DocumentationModules
{
	public DatabaseRow Documentation { get; set; }

	public List<ModuleRow> Modules { get; set; }

	public IEnumerable<int> ModulesId => Modules.Select((ModuleRow x) => x.Id.Value);

	public bool IsSelected { get; set; }
}
