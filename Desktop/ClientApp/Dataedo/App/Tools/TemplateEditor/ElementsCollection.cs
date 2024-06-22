using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using Dataedo.App.Documentation.Template;
using Dataedo.App.Documentation.Template.PdfTemplate.Model;
using Dataedo.App.Enums;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.CustomMessageBox;

namespace Dataedo.App.Tools.TemplateEditor;

public class ElementsCollection
{
	private string sourcePath;

	private BaseTemplateModel model;

	public string Name
	{
		get
		{
			return this?.model?.Name;
		}
		set
		{
			if (this?.model != null)
			{
				model.Name = value;
			}
		}
	}

	public string Description
	{
		get
		{
			return this?.model?.Description;
		}
		set
		{
			if (this?.model != null)
			{
				model.Description = value;
			}
		}
	}

	public bool IsElementsDataLoaded => this?.model != null;

	public ElementsCollection(string xmlPath, DocFormatEnum.DocFormat templateType, Form owner = null)
	{
		sourcePath = xmlPath;
		LoadXml(templateType, owner);
	}

	private void LoadXml(DocFormatEnum.DocFormat templateType, Form owner = null)
	{
		try
		{
			switch (templateType)
			{
			case DocFormatEnum.DocFormat.PDF:
			{
				XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(PdfTemplateModel));
				using FileStream input2 = new FileStream(sourcePath, FileMode.Open);
				XmlReader xmlReader2 = XmlReader.Create(input2);
				model = (PdfTemplateModel)xmlSerializer2.Deserialize(xmlReader2);
				break;
			}
			case DocFormatEnum.DocFormat.HTML:
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(BaseTemplateModel));
				using FileStream input = new FileStream(sourcePath, FileMode.Open);
				XmlReader xmlReader = XmlReader.Create(input);
				model = (BaseTemplateModel)xmlSerializer.Deserialize(xmlReader);
				break;
			}
			}
		}
		catch (UnauthorizedAccessException ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to load template." + Environment.NewLine + "Access is denied.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, owner);
		}
		catch (DirectoryNotFoundException ex2)
		{
			GeneralMessageBoxesHandling.Show("Unable to load template." + Environment.NewLine + "Unable to find template folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Message, 1, owner);
		}
		catch (Exception ex3)
		{
			CustomMessageBoxForm.ShowException(ex3, "Unable to load template", null, null, null, null, null, null, null, "Error", null, owner);
		}
	}

	public List<Element> GetCollection(Form owner = null)
	{
		try
		{
			return model?.GetAllSubElements();
		}
		catch (UnauthorizedAccessException ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to load template." + Environment.NewLine + "Access is denied.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, owner);
			return null;
		}
		catch (DirectoryNotFoundException ex2)
		{
			GeneralMessageBoxesHandling.Show("Unable to load template." + Environment.NewLine + "Unable to find template folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Message, 1, owner);
			return null;
		}
		catch (Exception ex3)
		{
			CustomMessageBoxForm.ShowException(ex3, "Unable to load template", null, null, null, null, null, null, null, "Error", null, owner);
			return null;
		}
	}

	public IEnumerable<Element> GetImages()
	{
		return (from x in model?.GetAllSubElements()
			where x.ElementType == ElementTypeEnum.ElementType.Path
			select x).Distinct();
	}

	internal void InitializeModel(bool withoutCustomizationModel = false, Form owner = null)
	{
		try
		{
			int id = 1;
			model?.Initialize(ref id, null);
		}
		catch (UnauthorizedAccessException ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to load template." + Environment.NewLine + "Access is denied.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, owner);
		}
		catch (DirectoryNotFoundException ex2)
		{
			GeneralMessageBoxesHandling.Show("Unable to load template." + Environment.NewLine + "Unable to find template folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Message, 1, owner);
		}
		catch (Exception ex3)
		{
			CustomMessageBoxForm.ShowException(ex3, "Unable to load template");
		}
	}

	public void SaveXml(DocFormatEnum.DocFormat templateType, Form owner = null)
	{
		try
		{
			switch (templateType)
			{
			case DocFormatEnum.DocFormat.PDF:
			{
				XmlSerializer xmlSerializer2 = new XmlSerializer(typeof(PdfTemplateModel));
				using FileStream fileStream2 = new FileStream(sourcePath, FileMode.Create);
				XmlWriter.Create(fileStream2);
				xmlSerializer2.Serialize(fileStream2, model);
				break;
			}
			case DocFormatEnum.DocFormat.HTML:
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(BaseTemplateModel));
				using FileStream fileStream = new FileStream(sourcePath, FileMode.Create);
				XmlWriter.Create(fileStream);
				xmlSerializer.Serialize(fileStream, model);
				break;
			}
			}
		}
		catch (UnauthorizedAccessException ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to load template." + Environment.NewLine + "Access is denied.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, owner);
		}
		catch (DirectoryNotFoundException ex2)
		{
			GeneralMessageBoxesHandling.Show("Unable to load template." + Environment.NewLine + "Unable to find template folder.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Message, 1, owner);
		}
		catch (Exception ex3)
		{
			CustomMessageBoxForm.ShowException(ex3, "Unable to load template");
		}
	}
}
