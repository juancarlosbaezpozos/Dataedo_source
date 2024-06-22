using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Tools.CommandLine.Common;
using Dataedo.App.Tools.CommandLine.Xml;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ImportCommand.CustomFields;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.ImportCommand;

internal static class ImportCommandBuilder
{
	public static Dataedo.App.Tools.CommandLine.Xml.Import CreateImportCommandObject(DatabaseType repositoryType, LoginInfo loginInfo, DatabaseRow sourceDatabase, int timeout, bool fullReimport, bool updateEntireDocumentation, FilterRulesCollection filterRulesCollection, List<DocumentationCustomFieldRow> customFields)
	{
		Dataedo.App.Tools.CommandLine.Xml.Import import = new Dataedo.App.Tools.CommandLine.Xml.Import
		{
			IsEnabled = true,
			RepositoryDocumentationId = sourceDatabase.Id,
			RepositoryDocumentationIdComment = new XmlCommentObject("Updated documentation: " + sourceDatabase.Title),
			FullReimport = fullReimport,
			UpdateEntireDocumentation = updateEntireDocumentation
		};
		CommandBuilderBase.SetRepositoryConnection(import, repositoryType, loginInfo);
		SetSourceDatabaseConnection(import, sourceDatabase, timeout);
		import.FilterRules = filterRulesCollection;
		if (DatabaseSupportFactory.GetDatabaseSupport(sourceDatabase.Type).CanImportToCustomFields)
		{
			import.ExtendedPropertiesImport = new ExtendedPropertiesImportBase
			{
				ExtendedProperties = customFields.Select((DocumentationCustomFieldRow x) => new ExtendedPropertyModel(x.CustomFieldId, x.IsSelected, x.Title, x.ExtendedProperty)).ToList()
			};
		}
		return import;
	}

	private static void SetSourceDatabaseConnection(Dataedo.App.Tools.CommandLine.Xml.Import importCommand, DatabaseRow sourceDatabase, int? timeout)
	{
		importCommand.SourceDatabaseConnection = DatabaseSupportFactory.GetDatabaseSupport(sourceDatabase.Type).GetXmlConnectionModel();
		importCommand.SourceDatabaseConnection.SetConnection(sourceDatabase);
		importCommand.SourceDatabaseConnection.Timeout = timeout;
	}
}
