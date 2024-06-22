using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Dataedo.App.Documentation;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.App.Tools.MessageBoxes;

namespace Dataedo.App.Tools.TemplateEditor;

public class HtmlTemplateCustomizer : BaseTemplateCustomizer
{
	public static void CopyCustomizedTemplateFiles(string sourceDirectory, string destinationDirectory)
	{
		FileSourceAndDestination[] array = new FileSourceAndDestination[4]
		{
			new FileSourceAndDestination(Path.Combine(sourceDirectory, "user.js"), Path.Combine(destinationDirectory, "js", "user.js")),
			new FileSourceAndDestination(Path.Combine(sourceDirectory, "user.css"), Path.Combine(destinationDirectory, "css", "user.css")),
			new FileSourceAndDestination(Path.Combine(sourceDirectory, "logo.png"), Path.Combine(destinationDirectory, "img", "logo.png")),
			new FileSourceAndDestination(Path.Combine(sourceDirectory, "favicon.ico"), Path.Combine(destinationDirectory, "favicon.ico"))
		};
		foreach (FileSourceAndDestination fileSourceAndDestination in array)
		{
			try
			{
				File.Copy(fileSourceAndDestination.Source, fileSourceAndDestination.Destination, overwrite: true);
			}
			catch (Exception)
			{
			}
		}
	}

	public HtmlTemplateCustomizer(DocTemplateFile template)
		: base(template)
	{
	}

	public override string Customize(Form owner = null)
	{
		string fileName = template.FileName;
		if (template.IsUserTemplate)
		{
			new Process();
			OpenTemplateFolder(fileName, owner);
			return Path.GetFileName(fileName);
		}
		string newTemplatePath = GetNewTemplatePath();
		string fileName2 = Path.GetFileName(fileName);
		DuplicateDirectory(fileName, newTemplatePath);
		if (CustomizeTemplateFiles(newTemplatePath, fileName2, owner))
		{
			OpenTemplateFolder(newTemplatePath);
		}
		return Path.GetFileName(newTemplatePath);
	}

	private void OpenTemplateFolder(string path, Form owner = null)
	{
		try
		{
			Process.Start(path);
		}
		catch (Win32Exception ex)
		{
			if (ex.NativeErrorCode == 2)
			{
				GeneralMessageBoxesHandling.Show("Unable to find template folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, owner);
				return;
			}
			throw;
		}
	}

	private bool CustomizeTemplateFiles(string templatePath, string baseTemplateName, Form owner = null)
	{
		try
		{
			string xmlPath = Path.Combine(templatePath, DocTemplateFile.MetaFileName);
			string fileName = Path.GetFileName(templatePath);
			ElementsCollection elementsCollection = new ElementsCollection(xmlPath, DocFormatEnum.DocFormat.HTML, owner);
			if (elementsCollection.IsElementsDataLoaded)
			{
				elementsCollection.InitializeModel(withoutCustomizationModel: false, owner);
				elementsCollection.Name = fileName;
				elementsCollection.Description = DocTemplateFile.GetCustomizedTemplateDescription(this?.template?.Template?.Name);
				elementsCollection.SaveXml(DocFormatEnum.DocFormat.HTML, owner);
				return true;
			}
		}
		catch (DirectoryNotFoundException ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to find template folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, owner);
		}
		return false;
	}

	protected override void DuplicateDirectory(string sourcePath, string destinationPath)
	{
		string[] obj = new string[5]
		{
			Path.Combine(sourcePath, "resources/js/user.js"),
			Path.Combine(sourcePath, "resources/css/user.css"),
			Path.Combine(sourcePath, "resources/img/logo.png"),
			Path.Combine(sourcePath, "resources/favicon.ico"),
			Path.Combine(sourcePath, "meta.xml")
		};
		Directory.CreateDirectory(destinationPath);
		string[] array = obj;
		foreach (string obj2 in array)
		{
			string fileName = Path.GetFileName(obj2);
			string newValue = Path.Combine(destinationPath, fileName);
			File.Copy(obj2, obj2.Replace(obj2, newValue), overwrite: true);
		}
		Resources.thumbnail_custom_html.Save(Path.Combine(destinationPath, DocTemplateFile.ThumbnailFileName + ".png"));
		CreateGuideUrlShortcut(destinationPath);
	}

	private void CreateGuideUrlShortcut(string destinationPath)
	{
		string contents = "\r\n[InternetShortcut]\r\nURL=" + Links.HowToCustomizeHTML;
		File.WriteAllText(Path.Combine(destinationPath, "How to customize HTML template.url"), contents);
	}
}
