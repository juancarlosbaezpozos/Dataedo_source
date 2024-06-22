using System;
using System.IO;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Template;
using Dataedo.App.Forms;
using DevExpress.Compression;

namespace Dataedo.App.Tools.TemplateEditor;

public class TemplateCustomizer
{
	private PDFTemplateEditorForm editor;

	public DocTemplateFile Template { get; set; }

	public TemplateCustomizer(DocTemplateFile template)
	{
		Template = template;
	}

	public string Customize()
	{
		try
		{
			string fileName = Template.FileName;
			FileAttributes attributes = File.GetAttributes(fileName);
			if (attributes.HasFlag(FileAttributes.Directory))
			{
				return CustomizeDirectory(fileName);
			}
			if (attributes.HasFlag(FileAttributes.Archive))
			{
				return CustomizeZip(fileName);
			}
			return null;
		}
		catch (Exception)
		{
			return null;
		}
	}

	private string CustomizeDirectory(string templatePath)
	{
		if (Template.IsUserTemplate)
		{
			editor = new PDFTemplateEditorForm(templatePath);
		}
		else
		{
			string newTemplatePath = GetNewTemplatePath();
			string fileName = Path.GetFileName(templatePath);
			DuplicateDirectory(templatePath, newTemplatePath);
			Path.Combine(newTemplatePath, DocTemplateFile.MetaFileName);
			editor = new PDFTemplateEditorForm(newTemplatePath, newTemplateCreated: true, fileName);
		}
		editor?.ShowDialog();
		return editor?.TemplateName;
	}

	private string GetNewTemplatePath()
	{
		string userTemplatesPath = Paths.GetUserTemplatesPath(TemplateTypeEnum.TypeToDocFormat(Template.Template.Type));
		string nextName = Paths.GetNextName(userTemplatesPath, "CustomTemplate", null, forFile: false);
		return Path.Combine(userTemplatesPath, nextName);
	}

	private string CustomizeZip(string templatePath)
	{
		string newTemplatePath = GetNewTemplatePath();
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(templatePath);
		CreateDirectory(newTemplatePath);
		using (ZipArchive zipArchive = ZipArchive.Read(templatePath))
		{
			zipArchive.Extract(newTemplatePath, AllowFileOverwriteMode.Allow);
		}
		editor = new PDFTemplateEditorForm(newTemplatePath, newTemplateCreated: true, fileNameWithoutExtension);
		editor.ShowDialog();
		return editor.TemplateName;
	}

	private void DuplicateDirectory(string sourcePath, string destinationPath)
	{
		Directory.CreateDirectory(destinationPath);
		string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
		foreach (string obj in files)
		{
			File.Copy(obj, obj.Replace(sourcePath, destinationPath), overwrite: true);
		}
	}

	private void CreateDirectory(string destinationPath)
	{
		Directory.CreateDirectory(destinationPath);
	}
}
