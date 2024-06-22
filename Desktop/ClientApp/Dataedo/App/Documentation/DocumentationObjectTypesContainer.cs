using System.ComponentModel;

namespace Dataedo.App.Documentation;

public class DocumentationObjectTypesContainer
{
	public BindingList<DocumentationWithObjectTypes> Data { get; set; }

	public DocumentationObjectTypesContainer()
	{
		Data = new BindingList<DocumentationWithObjectTypes>();
	}
}
