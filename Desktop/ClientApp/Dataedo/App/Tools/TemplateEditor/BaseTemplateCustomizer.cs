using System.IO;
using System.Windows.Forms;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Template;

namespace Dataedo.App.Tools.TemplateEditor;

public abstract class BaseTemplateCustomizer
{
	protected DocTemplateFile template;

	public BaseTemplateCustomizer(DocTemplateFile template)
	{
		this.template = template;
	}

	public abstract string Customize(Form owner = null);

	protected abstract void DuplicateDirectory(string sourcePath, string destinationPath);

	protected void CreateDirectory(string destinationPath)
	{
		Directory.CreateDirectory(destinationPath);
	}

	protected string GetNewTemplatePath()
	{
		string userTemplatesPath = Paths.GetUserTemplatesPath(TemplateTypeEnum.TypeToDocFormat(template.Template.Type));
		string nextName = Paths.GetNextName(userTemplatesPath, "CustomTemplate", null, forFile: false);
		return Path.Combine(userTemplatesPath, nextName);
	}
}
