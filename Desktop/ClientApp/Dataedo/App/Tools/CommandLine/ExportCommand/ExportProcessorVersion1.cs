using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Tools.CommandLine.Tools;
using Dataedo.App.Tools.CommandLine.Xml;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand;
using Dataedo.Model.Data.Modules;

namespace Dataedo.App.Tools.CommandLine.ExportCommand;

internal class ExportProcessorVersion1 : ExportProcessor
{
	public ExportProcessorVersion1(Dataedo.App.Tools.CommandLine.Tools.Log log, string commandsFilePath)
		: base(log, commandsFilePath)
	{
	}

	protected override void InitializeExport(ExportBase exportCommand, ref int handlingResultsCount)
	{
		if (exportCommand is ExportVersion1 exportVersion && exportVersion.RepositoryDocumentationId.HasValue)
		{
			exportVersion.RepositoryDatabaseRow = new DatabaseRow(DB.Database.GetDataById(exportVersion.RepositoryDocumentationId.Value));
			DatabaseRow repositoryDatabaseRow = exportVersion.RepositoryDatabaseRow;
			if (repositoryDatabaseRow != null && repositoryDatabaseRow.Id.HasValue)
			{
				base.Log.Write("Export " + exportVersion.RepositoryDatabaseRow.Title + " documentation from " + exportVersion.RepositoryConnection.GetDatabaseFull());
				return;
			}
			base.Log.Write("The documentation to export (ID: " + exportVersion?.RepositoryDocumentationId + ") not found in " + exportVersion.RepositoryConnection.GetDatabaseFull() + ".");
			handlingResultsCount++;
		}
		else
		{
			base.Log.Write("The documentation to export ('RepositoryDocumentationId' element in command file) is not set.");
			handlingResultsCount++;
		}
	}

	protected override void SetModules(ExportBase exportCommand)
	{
		ExportVersion1 exportVersion = exportCommand as ExportVersion1;
		if (exportVersion.Modules == null)
		{
			base.Log.Write("List of modules to export is not specified in command file.", "All modules will be exported.");
			exportVersion.Modules = new ModulesModel
			{
				ExportNotSpecified = true
			};
		}
		if (!exportVersion.Modules.ExportNotSpecified)
		{
			return;
		}
		foreach (ModuleObject item in DB.Module.GetDataByDatabase(exportVersion.RepositoryDocumentationId.Value))
		{
			Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ModuleModel module = new Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ModuleModel(new ModuleRow(item).IdValue, export: true);
			if (!exportVersion.Modules.Modules.Any((ModuleModelBase x) => x.IdValue == module.Id))
			{
				exportVersion.Modules.Modules.Add(module);
			}
		}
		ModuleOtherModel otherModule = new ModuleOtherModel(export: true);
		if (!exportVersion.Modules.Modules.Any((ModuleModelBase x) => x.IdValue == otherModule.IdValue))
		{
			exportVersion.Modules.Modules.Add(otherModule);
		}
	}
}
