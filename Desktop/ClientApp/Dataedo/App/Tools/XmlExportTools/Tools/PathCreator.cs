using System;
using System.IO;
using System.Text;
using System.Xml;
using Dataedo.App.Tools.XmlExportTools.Model;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Tools.XmlExportTools.Tools;

internal class PathCreator
{
	public const string RootFolder = "doc";

	public string ForDocumentation(ModuleExportData databaseExportData)
	{
		return "doc\\" + databaseExportData.IdStringForPath;
	}

	public string CreateLink(ModuleExportData databaseExportData, SharedObjectTypeEnum.ObjectType? objectType, string name, int? moduleId, string moduleName, bool isForLink)
	{
		string empty = string.Empty;
		string text = null;
		name = Paths.RemoveInvalidFilePathCharacters(name, "_");
		moduleName = ((moduleName != null) ? Paths.RemoveInvalidFilePathCharacters(moduleName, "_", removeInvalidFileNameCharsOnly: true) : string.Empty);
		name = EscapeUri(name, isForLink);
		moduleName = EscapeUri(moduleName, isForLink);
		if (objectType == SharedObjectTypeEnum.ObjectType.Database)
		{
			empty += Path.Combine("doc", moduleName);
			text = SharedObjectTypeEnum.TypeToStringForSingleLower(objectType);
		}
		else if (objectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			empty += Path.Combine(SharedObjectTypeEnum.TypeToStringLower(objectType), moduleName);
			text = SharedObjectTypeEnum.TypeToStringForSingleLower(objectType);
		}
		else
		{
			string path = (moduleId.HasValue ? Path.Combine(SharedObjectTypeEnum.TypeToStringLower(SharedObjectTypeEnum.ObjectType.Module), moduleName) : string.Empty);
			empty += Path.Combine(path, SharedObjectTypeEnum.TypeToStringLower(objectType)).ToString();
			text = name;
		}
		return Path.Combine(ForDocumentation(databaseExportData), empty, text).ToString().Replace("\\", "/");
	}

	public string CreateLink(ModuleExportData databaseExportData, SharedObjectTypeEnum.ObjectType? objectType, string name, bool isForLink)
	{
		return CreateLink(databaseExportData, objectType, name, null, null, isForLink);
	}

	public static string EscapeUri(string value, bool isForLink)
	{
		StringBuilder stringBuilder = new StringBuilder();
		StringBuilder stringBuilder2 = new StringBuilder();
		if (value != null)
		{
			for (int i = 0; i < value?.Length; i++)
			{
				try
				{
					if (i + 1 < value.Length && XmlConvert.IsXmlSurrogatePair(value[i], value[i + 1]))
					{
						string text = new string(new char[2]
						{
							value[i],
							value[i + 1]
						});
						if (isForLink)
						{
							stringBuilder2.Append(Uri.EscapeDataString(text));
						}
						else if (!string.IsNullOrEmpty(Uri.EscapeDataString(text)))
						{
							stringBuilder.Append(text);
						}
						i++;
					}
					else if (XmlConvert.IsXmlChar(value[i]))
					{
						if (isForLink)
						{
							stringBuilder2.Append(Uri.EscapeDataString(value[i].ToString()));
						}
						else if (!string.IsNullOrEmpty(Uri.EscapeDataString(value[i].ToString())))
						{
							stringBuilder.Append(value[i].ToString());
						}
					}
				}
				catch (Exception)
				{
				}
			}
			if (isForLink)
			{
				return stringBuilder2.ToString();
			}
			return stringBuilder.ToString();
		}
		return null;
	}
}
