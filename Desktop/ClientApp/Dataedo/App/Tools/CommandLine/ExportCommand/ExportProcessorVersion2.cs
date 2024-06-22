using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Tools.CommandLine.Tools;
using Dataedo.App.Tools.CommandLine.Xml;
using Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand;
using Dataedo.Model.Data.Modules;

namespace Dataedo.App.Tools.CommandLine.ExportCommand;

internal class ExportProcessorVersion2 : ExportProcessor
{
	public ExportProcessorVersion2(Dataedo.App.Tools.CommandLine.Tools.Log log, string commandsFilePath)
		: base(log, commandsFilePath)
	{
	}

	protected override void InitializeExport(ExportBase exportCommand, ref int handlingResultsCount)
	{
		ExportVersion2 exportVersion = exportCommand as ExportVersion2;
		if (exportVersion?.Documentations != null)
		{
			if (exportVersion != null && exportVersion.DocumentationsToExport?.Count() > 0)
			{
				List<string> list = new List<string>();
				foreach (DocumentationModelBase item in exportVersion.DocumentationsToExport)
				{
					DatabaseRow databaseRow = new DatabaseRow(DB.Database.GetDataById(item.DocumentationId));
					if (databaseRow != null && databaseRow.Id.HasValue)
					{
						item.RepositoryDatabaseRow = databaseRow;
						list.Add(databaseRow.Title);
						continue;
					}
					base.Log.Write("The documentation to export (ID: " + item.DocumentationId + ") not found in " + exportVersion.RepositoryConnection.GetDatabaseFull() + ".");
				}
				if (list.Count == 1)
				{
					base.Log.Write("Export " + list[0] + " documentation from " + exportVersion.RepositoryConnection.GetDatabaseFull());
				}
				else if (list.Count > 1)
				{
					base.Log.Write("Export " + string.Join(", ", list) + " documentations from " + exportVersion.RepositoryConnection.GetDatabaseFull());
				}
				else
				{
					handlingResultsCount++;
				}
			}
			else
			{
				base.Log.Write("No documentations ('Documentation' elements in command file) to export.");
				handlingResultsCount++;
			}
		}
		else
		{
			base.Log.Write("The documentations to export ('Documentations' element in command file) is not set.");
			handlingResultsCount++;
		}
	}

	protected override void SetModules(ExportBase exportCommand)
	{
		foreach (DocumentationModelBase item in (exportCommand as ExportVersion2).DocumentationsToExport)
		{
			if (item?.RepositoryDatabaseRow == null)
			{
				continue;
			}
			if (item.Modules == null)
			{
				base.Log.Write("List of modules to export from " + item.RepositoryDatabaseRow.Title + " is not specified in command file.", "All modules will be exported.");
				item.Modules = new ModulesModel
				{
					ExportNotSpecified = true
				};
			}
			if (!item.Modules.ExportNotSpecified)
			{
				continue;
			}
			foreach (ModuleObject item2 in DB.Module.GetDataByDatabase(item.DocumentationId))
			{
				Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ModuleModel module = new Dataedo.App.Tools.CommandLine.Xml.Commands.ExportCommand.ModuleModel(new ModuleRow(item2).IdValue, export: true);
				if (!item.Modules.Modules.Any((ModuleModelBase x) => x.IdValue == module.Id))
				{
					item.Modules.Modules.Add(module);
				}
			}
			ModuleOtherModel otherModule = new ModuleOtherModel(export: true);
			if (!item.Modules.Modules.Any((ModuleModelBase x) => x.IdValue == otherModule.IdValue))
			{
				item.Modules.Modules.Add(otherModule);
			}
		}
	}
}
