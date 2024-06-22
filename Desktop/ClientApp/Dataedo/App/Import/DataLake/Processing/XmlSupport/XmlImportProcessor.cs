using System.Collections;
using System.Xml;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Import.DataLake.Processing.XmlSupport;

internal class XmlImportProcessor
{
	public XmlFieldImportModel Process(XmlNode xmlNode, XmlFieldImportModel correspondingField, FieldModel parentField)
	{
		if (correspondingField == null)
		{
			correspondingField = GetElementModel(xmlNode, parentField);
		}
		ProcessAttributes(xmlNode, correspondingField);
		ProcessNodes(xmlNode, correspondingField);
		if (correspondingField.ObjectSubtype != SharedObjectSubtypeEnum.ObjectSubtype.Document && (correspondingField.HasChildNodes() || correspondingField.HasAttributes()))
		{
			correspondingField.ObjectSubtype = SharedObjectSubtypeEnum.ObjectSubtype.DocumentArray;
		}
		return correspondingField;
	}

	private void ProcessAttributes(XmlNode xmlNode, XmlFieldImportModel correspondingField)
	{
		IEnumerator enumerator = xmlNode.Attributes.GetEnumerator();
		while (enumerator.MoveNext())
		{
			XmlAttribute xmlAttribute = enumerator.Current as XmlAttribute;
			if (!correspondingField.HasAttribute(xmlAttribute.Name))
			{
				XmlFieldImportModel attributeModel = GetAttributeModel(xmlAttribute, correspondingField);
				correspondingField.AddAttribute(attributeModel);
			}
		}
	}

	private void ProcessNodes(XmlNode xmlNode, XmlFieldImportModel correspondingField)
	{
		IEnumerator enumerator = xmlNode.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current is XmlNode xmlNode2 && (xmlNode2.NodeType == XmlNodeType.Element || xmlNode2.NodeType == XmlNodeType.Attribute))
			{
				XmlFieldImportModel childField = correspondingField.GetChildField(xmlNode2.Name);
				if (childField == null)
				{
					XmlFieldImportModel field = Process(xmlNode2, null, correspondingField);
					correspondingField.AddChildField(field);
				}
				else
				{
					Process(xmlNode2, childField, correspondingField);
				}
			}
		}
	}

	private XmlFieldImportModel GetElementModel(XmlNode xmlNode, FieldModel parentField)
	{
		XmlFieldImportModel xmlFieldImportModel = new XmlFieldImportModel(xmlNode.Name, parentField);
		if (parentField == null)
		{
			xmlFieldImportModel.ObjectSubtype = SharedObjectSubtypeEnum.ObjectSubtype.Document;
		}
		else
		{
			xmlFieldImportModel.ObjectSubtype = SharedObjectSubtypeEnum.ObjectSubtype.Field;
		}
		xmlFieldImportModel.DataType = SharedObjectSubtypeEnum.TypeToStringForSingle(xmlFieldImportModel.ObjectType, xmlFieldImportModel.ObjectSubtype);
		return xmlFieldImportModel;
	}

	private XmlFieldImportModel GetAttributeModel(XmlAttribute xmlAttribute, FieldModel parentField)
	{
		XmlFieldImportModel xmlFieldImportModel = new XmlFieldImportModel(xmlAttribute.Name, parentField);
		xmlFieldImportModel.ObjectSubtype = SharedObjectSubtypeEnum.ObjectSubtype.Attribute;
		xmlFieldImportModel.DataType = SharedObjectSubtypeEnum.TypeToStringForSingle(xmlFieldImportModel.ObjectType, xmlFieldImportModel.ObjectSubtype);
		return xmlFieldImportModel;
	}
}
