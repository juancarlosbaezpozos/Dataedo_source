using System.Collections.Generic;

namespace Dataedo.App.Tools;

public class DocumentationModel
{
	public int Id { get; set; }

	public string DisplayName { get; set; }

	public List<ModuleModel> Modules { get; set; }

	public DocumentationModel()
	{
		Modules = new List<ModuleModel>();
	}
}
