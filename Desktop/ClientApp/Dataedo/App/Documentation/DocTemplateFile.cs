using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Dataedo.App.Documentation.Template;
using Dataedo.App.Documentation.Template.PdfTemplate.Model;
using Dataedo.App.Documentation.Template.PdfTemplate.Model.Common;
using Dataedo.App.Enums;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using DevExpress.Compression;

namespace Dataedo.App.Documentation;

public class DocTemplateFile
{
	public static readonly string templatesFolderName = "Templates";

	public static readonly string MetaFileName = "meta.xml";

	public static readonly string ThumbnailFileName = "thumbnail";

	public static readonly string DefaultProgramTemplate = "Detailed documentation";

	public bool IsFile { get; private set; }

	public bool IsUserTemplate { get; set; }

	public object CustomObject { get; set; }

	public string FileName { get; set; }

	public string CoreTemplateFilePath => GetCoreTemplateFilePath(Template);

	public string Description { get; set; }

	public TemplateTypeEnum.TemplateType Type { get; set; }

	public Image Image { get; set; }

	public IBaseTemplateModel Template { get; set; }

	public bool IsCustomizable
	{
		get
		{
			if (Type != TemplateTypeEnum.TemplateType.HTML)
			{
				if (Type == TemplateTypeEnum.TemplateType.PDF)
				{
					IBaseTemplateModel template = Template;
					if (template == null)
					{
						return false;
					}
					return template.IsCustomizable == true;
				}
				return false;
			}
			return true;
		}
	}

	public static string GetTemplatesPath(DocFormatEnum.DocFormat format, Form owner = null)
	{
		string path = string.Empty;
		string empty = string.Empty;
		switch (format)
		{
		case DocFormatEnum.DocFormat.PDF:
			path = "PDF";
			break;
		case DocFormatEnum.DocFormat.HTML:
			path = "HTML";
			break;
		case DocFormatEnum.DocFormat.Excel:
			path = "EXCEL";
			break;
		}
		empty = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Templates", path);
		if (Directory.Exists(empty.ToString()))
		{
			return empty.ToString();
		}
		GeneralMessageBoxesHandling.Show("Directory with the documentation's templates files not found: ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		return string.Empty;
	}

	public static string GetCustomizedTemplateDescription(string name)
	{
		return "Custom template based on " + name;
	}

	public static string GetCoreTemplateFilePath(IBaseTemplateModel template, Form owner = null)
	{
		switch (template.Type)
		{
		case TemplateTypeEnum.TemplateType.PDF:
		{
			PdfTemplateModel obj = template as PdfTemplateModel;
			string templatesPath = GetTemplatesPath(DocFormatEnum.DocFormat.PDF, owner);
			return obj.Subtype switch
			{
				TemplatePDFSubtypeEnum.TemplatePDFSubtype.DetailedPrinterFriendly => Path.Combine(templatesPath, "Detailed documentation (printer friendly).zip"), 
				TemplatePDFSubtypeEnum.TemplatePDFSubtype.Detailed => Path.Combine(templatesPath, "Detailed documentation.zip"), 
				TemplatePDFSubtypeEnum.TemplatePDFSubtype.Overview => Path.Combine(templatesPath, "Overview documentation.zip"), 
				_ => string.Empty, 
			};
		}
		case TemplateTypeEnum.TemplateType.HTML:
			return Path.Combine(GetTemplatesPath(DocFormatEnum.DocFormat.HTML, owner), "vue");
		default:
			return string.Empty;
		}
	}

	public DocTemplateFile()
	{
	}

	public DocTemplateFile(DocFormatEnum.DocFormat format, string fileName, bool userTemplate = false, Form owner = null)
	{
		try
		{
			FileName = fileName;
			IsUserTemplate = userTemplate;
			Path.GetExtension(fileName);
			XmlSerializer[] serializers = ((format == DocFormatEnum.DocFormat.PDF) ? new XmlSerializer[1]
			{
				new XmlSerializer(typeof(PdfTemplateModel))
			} : new XmlSerializer[2]
			{
				new XmlSerializer(typeof(BaseTemplateModel)),
				null
			});
			IBaseTemplateModel template = null;
			if (!File.GetAttributes(fileName).HasFlag(FileAttributes.Directory) && Path.GetExtension(fileName).Equals(".zip"))
			{
				try
				{
					using ZipArchive archive = ZipArchive.Read(fileName);
					ProcessZipTemplate(archive, serializers, ref template, fileName);
				}
				catch (Exception ex) when (ex is ArgumentException || ex is NotSupportedException)
				{
					template = null;
					Image = null;
					using ZipArchive archive2 = ZipArchive.Read(fileName, Encoding.GetEncoding(1250));
					ProcessZipTemplate(archive2, serializers, ref template, fileName);
				}
				if (template == null)
				{
					using ZipArchive archive3 = ZipArchive.Read(fileName, Encoding.GetEncoding(1250));
					ProcessZipTemplate(archive3, serializers, ref template, fileName);
				}
			}
			else
			{
				string filePath = Path.Combine(FileName, MetaFileName);
				if (File.Exists(filePath))
				{
					HandleTemplateModel(FileName, MetaFileName, () => File.OpenRead(filePath), serializers, ref template);
					if (template is PdfTemplateModel)
					{
						PdfTemplateModel pdfTemplateModel = template as PdfTemplateModel;
						new DirectoryInfo(FileName);
						LoadRootedOrRelativeCustomizationItems(pdfTemplateModel);
					}
				}
				string text = Path.Combine(FileName, ThumbnailFileName);
				if (File.Exists(text + ".jpg"))
				{
					using StreamReader streamReader = new StreamReader(text + ".jpg");
					Image = Image.FromStream(streamReader.BaseStream);
				}
				else if (File.Exists(text + ".png"))
				{
					using StreamReader streamReader2 = new StreamReader(text + ".png");
					Image = Image.FromStream(streamReader2.BaseStream);
				}
			}
			if (template != null)
			{
				Type = template.Type;
				if (template.ExceptionValue == null)
				{
					Description = new StringBuilder().Append("<b>").Append(template.Name).Append("</b><br><br><br>")
						.Append(template.Description)
						.ToString();
				}
				else
				{
					Description = new StringBuilder().Append("<b>").Append(template.Name).Append("</b><br><br>")
						.Append(template.Description)
						.ToString();
				}
			}
			Template = template;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, owner);
		}
	}

	private void ProcessZipTemplate(ZipArchive archive, XmlSerializer[] serializers, ref IBaseTemplateModel template, string fileName)
	{
		IBaseTemplateModel template2 = null;
		foreach (ZipItem item2 in archive.ToList())
		{
			Path.GetFileName(item2.Name);
			HandleTemplateModel(fileName, item2.Name, () => item2.Open(), serializers, ref template2);
		}
		if (template2 is PdfTemplateModel)
		{
			PdfTemplateModel pdfTemplateModel = template2 as PdfTemplateModel;
			archive.ToList().ForEach(delegate(ZipItem item)
			{
				LoadCustomizationItems(item.Name, () => item.Open(), pdfTemplateModel);
			});
		}
		template = template2;
	}

	private void LoadCustomizationItems(string filename, Func<Stream> getStream, PdfTemplateModel pdfTemplateModel)
	{
		if (filename == pdfTemplateModel?.Customization?.TitlePage?.Image?.Name)
		{
			using StreamReader streamReader = new StreamReader(getStream());
			pdfTemplateModel.Customization.TitlePage.Image.ImageValue = Image.FromStream(streamReader.BaseStream);
		}
		if (filename == pdfTemplateModel?.Customization?.Footer?.Left?.Image?.Name)
		{
			using StreamReader streamReader2 = new StreamReader(getStream());
			pdfTemplateModel.Customization.Footer.Left.Image.ImageValue = Image.FromStream(streamReader2.BaseStream);
		}
		if (filename == pdfTemplateModel?.Customization?.Footer?.Center?.Image?.Name)
		{
			using StreamReader streamReader3 = new StreamReader(getStream());
			pdfTemplateModel.Customization.Footer.Center.Image.ImageValue = Image.FromStream(streamReader3.BaseStream);
		}
		if (filename == pdfTemplateModel?.Customization?.Footer?.Right?.Image?.Name)
		{
			using (StreamReader streamReader4 = new StreamReader(getStream()))
			{
				pdfTemplateModel.Customization.Footer.Right.Image.ImageValue = Image.FromStream(streamReader4.BaseStream);
			}
		}
	}

	private void LoadRootedOrRelativeCustomizationItems(PdfTemplateModel pdfTemplateModel)
	{
		LoadImageWithRootedOrRelativePath(pdfTemplateModel.Customization.TitlePage.Image);
		LoadImageWithRootedOrRelativePath(pdfTemplateModel.Customization.Footer.Left.Image);
		LoadImageWithRootedOrRelativePath(pdfTemplateModel.Customization.Footer.Center.Image);
		LoadImageWithRootedOrRelativePath(pdfTemplateModel.Customization.Footer.Right.Image);
	}

	private void LoadImageWithRootedOrRelativePath(ImageModel imageModel)
	{
		if (imageModel?.ImageValue == null)
		{
			if (Path.IsPathRooted(imageModel?.Name) && File.Exists(imageModel.Name))
			{
				imageModel.ImageValue = Image.FromFile(imageModel.Name);
			}
			else if (FileName != null && imageModel != null && imageModel.Name != null && File.Exists(Path.Combine(FileName, imageModel.Name)))
			{
				imageModel.ImageValue = Image.FromFile(Path.Combine(FileName, imageModel.Name));
			}
		}
	}

	private void HandleTemplateModel(string fileName, string itemFileName, Func<Stream> getStream, XmlSerializer[] serializers, ref IBaseTemplateModel template)
	{
		IBaseTemplateModel baseTemplateModel = null;
		if (itemFileName == MetaFileName)
		{
			Exception exceptionValue = null;
			for (int i = 0; i < serializers.Length; i++)
			{
				try
				{
					using Stream stream = getStream();
					baseTemplateModel = serializers[i].Deserialize(stream) as IBaseTemplateModel;
				}
				catch (Exception ex)
				{
					if (i == 0)
					{
						exceptionValue = ex;
					}
					continue;
				}
				break;
			}
			if (baseTemplateModel == null)
			{
				baseTemplateModel = new BaseTemplateModel
				{
					Name = "<color=darkred>Invalid template: " + Path.GetFileName(fileName) + "</color>",
					Description = "<b>Double click for more information</b>",
					ExceptionValue = exceptionValue
				};
			}
			template = baseTemplateModel;
		}
		else if (itemFileName == ThumbnailFileName + ".png" || itemFileName == ThumbnailFileName + ".jpg")
		{
			using (StreamReader streamReader = new StreamReader(getStream()))
			{
				Image = Image.FromStream(streamReader.BaseStream);
			}
		}
	}
}
