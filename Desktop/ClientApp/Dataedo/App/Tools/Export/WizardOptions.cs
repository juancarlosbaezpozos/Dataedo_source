using System.Collections.Generic;
using Dataedo.App.Documentation;
using Dataedo.App.Forms.Support.DocWizardForm;
using Dataedo.App.Tools.CustomFields;

namespace Dataedo.App.Tools.Export;

public class WizardOptions
{
	public string Title { get; set; }

	public DocumentationModulesContainer Scope { get; set; }

	public List<ObjectTypeHierarchy> ExcludedObject { get; set; }

	public CustomFieldsSupport CustomFields { get; set; }

	public OtherFieldsSupport OtherFields { get; set; }
}
