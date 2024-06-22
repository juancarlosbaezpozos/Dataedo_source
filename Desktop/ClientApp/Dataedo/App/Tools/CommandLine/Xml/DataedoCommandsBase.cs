using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.Xml.Commands.Base;

namespace Dataedo.App.Tools.CommandLine.Xml;

[XmlRoot(ElementName = "DataedoCommands")]
public abstract class DataedoCommandsBase
{
	[XmlAttribute]
	public abstract string Version { get; set; }

	[XmlIgnore]
	public abstract List<CommandBase> Commands { get; set; }

	[XmlAnyElement(Name = "GeneralComment")]
	public XmlCommentObject GeneralComment { get; set; } = new XmlCommentObject("Visit " + Links.CommandLineSupportUrl + " for more information about Dataedo command line scripts.");


	[XmlElement]
	public Settings Settings { get; set; }

	public bool IsLogEnabled => !string.IsNullOrEmpty(Settings?.LogFile?.Path);

	[XmlIgnore]
	public string CommandsFilePath { get; set; }

	public DataedoCommandsBase()
	{
		Settings = new Settings();
		Commands = new List<CommandBase>();
		Settings.LogFile.Path = "Dataedo {DateTime:yyyy-MM-dd}.log";
	}

	public static DataedoCommandsBase GetCommands(string path, ref Exception exception)
	{
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(path);
			string attribute = xmlDocument.DocumentElement.GetAttribute("Version");
			if (File.Exists(path))
			{
				using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read))
				{
					DataedoCommandsBase dataedoCommandsBase = null;
					dataedoCommandsBase = ((attribute == DataedoCommandsVersion2.VersionValue) ? DataedoCommandsVersion2.DeserializeXml(stream) : ((!(attribute == DataedoCommandsVersion1.VersionValue)) ? ((DataedoCommandsBase)DataedoCommandsVersion2.DeserializeXml(stream)) : ((DataedoCommandsBase)DataedoCommandsVersion1.DeserializeXml(stream))));
					dataedoCommandsBase.CommandsFilePath = path;
					return dataedoCommandsBase;
				}
			}
		}
		catch (InvalidOperationException ex)
		{
			InvalidOperationException ex2 = (InvalidOperationException)(exception = ex);
			return null;
		}
		catch (XmlException ex3)
		{
			XmlException ex4 = (XmlException)(exception = ex3);
			return null;
		}
		return null;
	}

	public abstract XmlSerializer GetCurrentSerializer();

	public void SaveCommandsXml(string path, FileMode fileMode)
	{
		XmlSerializer currentSerializer = GetCurrentSerializer();
		using FileStream stream = File.Open(path, fileMode, FileAccess.Write);
		currentSerializer.Serialize(stream, this);
	}
}
