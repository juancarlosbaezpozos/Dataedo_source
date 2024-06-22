using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Tools;
using Dataedo.Shared.DatabasesSupport;
using Dataedo.Shared.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors;

namespace Dataedo.App.DatabasesSupport.DatabaseTypes.Base;

internal abstract class DatabaseSupportBase : IDatabaseSupportShared
{
	protected abstract SharedDatabaseTypeEnum.DatabaseType DatabaseType { get; }

	public IDatabaseSupportShared BasicData => DatabaseSupportFactoryShared.GetDatabaseSupport(DatabaseType);

	public string DefaultConnectionPort => BasicData.DefaultConnectionPort;

	public string FriendlyDisplayName => BasicData.FriendlyDisplayName;

	public SharedDatabaseTypeEnum.DatabaseType SupportedDatabaseType => BasicData.SupportedDatabaseType;

	public SharedDatabaseClassEnum.DatabaseClass SupportedDatabaseClass => BasicData.SupportedDatabaseClass;

	public string TypeValue => BasicData.TypeValue;

	public List<SharedImportFolderEnum.ImportFolder> ImportFolders => BasicData.ImportFolders;

	public string DocumentationLink => BasicData.DocumentationLink;

	protected ConnectorVersionInfo VersionInfo => ConnectorsVersion.GetVersionInfo(DatabaseType);

	public static void SetElementsButtonEdit(ButtonEdit schemasButtonEdit, IEnumerable<string> elements)
	{
		schemasButtonEdit.Text = string.Join(",", elements);
	}

	public static void SetElementsLabelControl(LabelControl schemasLabelControl, IEnumerable<string> elements, string elementsName = "schemas")
	{
		schemasLabelControl.Text = ((elements.Count() > 1) ? $"{elements.Count()} {elementsName}" : string.Empty);
	}

	protected string GetNotSupportedText(bool withQuestion = false)
	{
		string text = (string.IsNullOrWhiteSpace(DocumentationLink) ? Links.SupportedDatabases : DocumentationLink);
		string text2 = "The version of DBMS is not officially supported." + Environment.NewLine + $"Dataedo {ProgramVersion.Major}.{ProgramVersion.Minor}.{ProgramVersion.Build} " + "supports " + FriendlyDisplayName + " versions " + GetSupportedVersionsText() + "." + Environment.NewLine + Environment.NewLine + "Learn more about supported versions in the <href=" + text + ">documentation</href>.";
		if (withQuestion)
		{
			text2 = text2 + Environment.NewLine + Environment.NewLine + "Do you want to give it a try (sometimes unexpected issues might occur)?";
		}
		return text2;
	}

	protected virtual string GetSupportedVersionsText()
	{
		return $"from {VersionInfo.FirstSupportedVersion} " + $"and before {VersionInfo.FirstNotSupportedVersion}";
	}
}
