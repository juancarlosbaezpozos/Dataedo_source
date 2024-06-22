using System.Collections.Generic;
using System.Text;
using Dataedo.App.Documentation.Template.PdfTemplate.Model.ChildModels;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools;

public class DocHeaders
{
	private string parentPrefix = string.Empty;

	private int moduleOrder;

	private int objectOrder;

	private string moduleString => $"{moduleOrder}.";

	public Dictionary<string, string> ObjectTypeOrderDictionary { get; set; }

	public DocHeaders(int moduleOrder, string parentPrefix)
	{
		this.moduleOrder = moduleOrder;
		ObjectTypeOrderDictionary = new Dictionary<string, string>();
		objectOrder = 1;
		this.parentPrefix = parentPrefix;
	}

	private string AddToDictionary(string objectType, bool useModuleString = true)
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (useModuleString)
		{
			stringBuilder.Append(moduleString);
		}
		string text = stringBuilder.Append(objectOrder++).Append(".").ToString();
		ObjectTypeOrderDictionary.Add(objectType, text);
		return text;
	}

	public string CreateModuleNameWithOrder(string name, bool useModuleString = true)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(parentPrefix);
		if (useModuleString)
		{
			stringBuilder.Append(moduleString).Append("   ");
		}
		else
		{
			stringBuilder.Append(" ");
		}
		return stringBuilder.Append(name).ToString();
	}

	public string CreateListNameWithOrder(SharedObjectTypeEnum.ObjectType objectType, string title)
	{
		return new StringBuilder().Append(parentPrefix).Append(ObjectTypeOrderDictionary[SharedObjectTypeEnum.TypeToString(objectType)]).Append("  ")
			.Append(title)
			.ToString();
	}

	public string CreateListNameWithOrder(string objectType, string title)
	{
		string value = null;
		if (!ObjectTypeOrderDictionary.TryGetValue(objectType, out value))
		{
			value = AddToDictionary(objectType, useModuleString: false);
		}
		return new StringBuilder().Append(parentPrefix).Append(ObjectTypeOrderDictionary[objectType]).Append("  ")
			.Append(title)
			.ToString();
	}

	public string CreateNameWithOrder(string name, int objectOrder, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype? objectSubtype, HeadingsNamesModel headingsNamesModel, bool useModuleString = true)
	{
		string value = null;
		if (!ObjectTypeOrderDictionary.TryGetValue(SharedObjectTypeEnum.TypeToString(objectType), out value))
		{
			value = AddToDictionary(SharedObjectTypeEnum.TypeToString(objectType), useModuleString);
		}
		if (headingsNamesModel == null)
		{
			headingsNamesModel = new HeadingsNamesModel();
		}
		string objectSubtypeTypeName = headingsNamesModel.GetObjectSubtypeTypeName(objectType, objectSubtype);
		return new StringBuilder().Append(parentPrefix).Append(value).Append(objectOrder)
			.Append(".   ")
			.Append((!string.IsNullOrEmpty(objectSubtypeTypeName)) ? (objectSubtypeTypeName + ": ") : null)
			.Append(name)
			.ToString();
	}

	public string CreateNameWithOrder(string name, int objectOrder, string objectType, HeadingsNamesModel headingsNamesModel, bool useModuleString = true)
	{
		string value = null;
		if (!ObjectTypeOrderDictionary.TryGetValue(objectType, out value))
		{
			value = AddToDictionary(objectType, useModuleString);
		}
		if (headingsNamesModel == null)
		{
			headingsNamesModel = new HeadingsNamesModel();
		}
		return new StringBuilder().Append(parentPrefix).Append(value).Append(objectOrder)
			.Append(".   ")
			.Append((!string.IsNullOrEmpty(objectType)) ? (objectType + ": ") : null)
			.Append(name)
			.ToString();
	}
}
