using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Documentation;
using Dataedo.App.Forms;
using Dataedo.App.Tools.MessageBoxes;
using DevExpress.Compression;

namespace Dataedo.App.Tools.TemplateEditor;

public class PdfTemplateCustomizer : BaseTemplateCustomizer
{
	private PDFTemplateEditorForm editor;

	private IWin32Window owner;

	public PdfTemplateCustomizer(DocTemplateFile template, IWin32Window parent)
		: base(template)
	{
		owner = parent;
	}

	public override string Customize(Form owner = null)
	{
		try
		{
			string fileName = template.FileName;
			FileAttributes attributes = File.GetAttributes(fileName);
			if (attributes.HasFlag(FileAttributes.Directory))
			{
				return CustomizeDirectory(fileName);
			}
			if (attributes.HasFlag(FileAttributes.Archive))
			{
				return CustomizeZip(fileName);
			}
		}
		catch (FileNotFoundException ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to load template.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, owner);
		}
		return null;
	}

	protected override void DuplicateDirectory(string sourcePath, string destinationPath)
	{
		Directory.CreateDirectory(destinationPath);
		string[] files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
		foreach (string obj in files)
		{
			File.Copy(obj, obj.Replace(sourcePath, destinationPath), overwrite: true);
		}
	}

	private string CustomizeDirectory(string templatePath)
	{
		if (template.IsUserTemplate)
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
		if (editor.IsXmlDataLoaded)
		{
			editor?.ShowDialog(owner);
		}
		return editor?.TemplateName;
	}

	private string CustomizeZip(string templatePath)
	{
		string newTemplatePath = GetNewTemplatePath();
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(templatePath);
		CreateDirectory(newTemplatePath);
		try
		{
			using ZipArchive zipArchive = ZipArchive.Read(templatePath);
			zipArchive.Extract(newTemplatePath, AllowFileOverwriteMode.Allow);
		}
		catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException)
		{
			using ZipArchive zipArchive2 = ZipArchive.Read(templatePath, Encoding.GetEncoding(1250));
			zipArchive2.Extract(newTemplatePath, AllowFileOverwriteMode.Allow);
		}
		DeleteRepxFilesFromDirectory(newTemplatePath);
		editor = new PDFTemplateEditorForm(newTemplatePath, newTemplateCreated: true, fileNameWithoutExtension);
		editor.ShowDialog(owner);
		return editor.TemplateName;
	}

	private void DeleteRepxFilesFromDirectory(string path)
	{
		foreach (FileInfo item in from p in new DirectoryInfo(path).GetFiles("*.repx")
			where p.Extension == ".repx"
			select p)
		{
			try
			{
				item.Attributes = FileAttributes.Normal;
				File.Delete(item.FullName);
			}
			catch
			{
			}
		}
	}
}
