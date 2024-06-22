using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.API.Models;
using Dataedo.App.API.Services;
using Dataedo.App.LoginFormTools.Tools.Licenses;
using Dataedo.App.Tools.CommandLine.ExportCommand;
using Dataedo.App.Tools.CommandLine.ImportCommand;
using Dataedo.App.Tools.CommandLine.Tools;
using Dataedo.App.Tools.CommandLine.Xml;
using Dataedo.App.Tools.CommandLine.Xml.Commands.Base;
using Dataedo.App.Tools.CommandLine.Xml.Connections;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.GeneralHandling;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine;

public class CommandsProcessor
{
	public class CommandsProcessorResult
	{
		public bool KeepOpen { get; set; }

		public ConnectionBase RepositoryConnection { get; set; }

		public int ErrorsCount { get; set; }

		public int FailedCommandsCount { get; set; }

		public int ValidCommandsCount { get; set; }
	}

	private string commandsFilePath;

	public Dataedo.App.Tools.CommandLine.Tools.Log log { get; set; }

	public void PrepareLog(DataedoCommandsBase dataedoCommands)
	{
		log = new Dataedo.App.Tools.CommandLine.Tools.Log();
		string logFilePath = string.Empty;
		try
		{
			PrepareLogFile(dataedoCommands, ref logFilePath, writeFileLogPath: true);
		}
		catch (Exception ex)
		{
			log.WriteToConsoleSimple("Log file path (\"" + dataedoCommands.Settings.LogFilePathParsed + "\") is invalid.", "Details:", ex.ToString());
			dataedoCommands.Settings.LogFile.Path = "{MyDocuments}\\Dataedo\\Logs\\{DateTime:yyyy-MM-dd}.log";
			logFilePath = PathHelpers.GetRootedOrRelative(dataedoCommands.CommandsFilePath, dataedoCommands.Settings.LogFilePathParsed);
			log.WriteToConsoleSimple(string.Empty, "File log path changed to: \"" + logFilePath + "\".");
			try
			{
				PrepareLogFile(dataedoCommands, ref logFilePath, writeFileLogPath: false);
			}
			catch (Exception ex2)
			{
				log.WriteToConsoleSimple("Log file path (\"" + dataedoCommands.Settings.LogFilePathParsed + "\") is invalid.", "Details:", ex2.ToString());
			}
		}
		log.WriteToConsoleSimple();
	}

	private void PrepareLogFile(DataedoCommandsBase dataedoCommands, ref string logFilePath, bool writeFileLogPath)
	{
		logFilePath = PathHelpers.GetRootedOrRelative(dataedoCommands.CommandsFilePath, dataedoCommands.Settings.LogFilePathParsed);
		if (log == null)
		{
			log = new Dataedo.App.Tools.CommandLine.Tools.Log(logFilePath);
		}
		else
		{
			log.FilePath = logFilePath;
		}
		if (writeFileLogPath)
		{
			log.WriteToConsole("File log: " + logFilePath + ".");
		}
		if (!string.IsNullOrEmpty(Path.GetDirectoryName(logFilePath)) && !Directory.Exists(Path.GetDirectoryName(logFilePath)))
		{
			Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
		}
		using (File.Open(logFilePath, FileMode.OpenOrCreate, FileAccess.Read))
		{
		}
	}

	public async Task<CommandsProcessorResult> ProcessCommands(DataedoCommandsBase dataedoCommands, Form owner = null)
	{
		commandsFilePath = dataedoCommands.CommandsFilePath;
		CommandsProcessorResult result = new CommandsProcessorResult();
		if (!string.IsNullOrEmpty(dataedoCommands.CommandsFilePath))
		{
			log.Write("Processing file: " + Path.GetFileName(dataedoCommands.CommandsFilePath) + " (" + dataedoCommands.CommandsFilePath + ").");
		}
		else
		{
			log.Write("Processing file.");
		}
		if (!(await SignIn()))
		{
			result.ErrorsCount++;
			log.Write(GeneralHandlingSupport.StoredHandlingResults);
		}
		else
		{
			foreach (CommandBase command in dataedoCommands.Commands)
			{
				log.WriteSimple();
				GeneralHandlingSupport.ClearStoredHandlingResults();
				if (command is Dataedo.App.Tools.CommandLine.Xml.Import)
				{
					HandleImportCommand(command as Dataedo.App.Tools.CommandLine.Xml.Import, result, log, owner);
				}
				else if (command is ExportBase)
				{
					HandleExportCommand(command as ExportBase, result, log, owner);
				}
				log.Write(GeneralHandlingSupport.StoredHandlingResults);
			}
		}
		log.WriteSimple(string.Empty, "Processing finished. Status: " + ((result.ErrorsCount <= 0) ? "[OK]" : "[FAIL]"), string.Empty, string.Empty, string.Empty);
		log.FlushBuffer();
		return result;
	}

	private async Task<bool> SignIn()
	{
		if (!SessionDataHelper.Load())
		{
			LicenseFileDataContainer licenseFileData = LicenseFileDataHelper.GetLicenseFileData();
			if (licenseFileData != null)
			{
				LicenseFileDataHelper.Save(licenseFileData.LicenseFile, licenseFileData.LastSelectedLicense);
				return true;
			}
			log.Write("No web license set. Sign in using Dataedo Desktop to set license.");
			return false;
		}
		if (StaticData.LastLicenseId == null)
		{
			log.Write("No license set. Sign in using Dataedo Desktop to set license.");
			return false;
		}
		LoginService loginService = new LoginService();
		ResultWithData<LicensesResult> resultWithData;
		try
		{
			resultWithData = await loginService.GetLicenses(StaticData.Token);
		}
		catch (HttpRequestException)
		{
			GeneralMessageBoxesHandling.Show("Error while loading licenses." + Environment.NewLine + Environment.NewLine + "Unable to connect to Dataedo server.", "Licenses", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while loading licenses:");
			return false;
		}
		List<LicenseDataResult> licenses = resultWithData.Data.Licenses;
		if (!resultWithData.IsOK)
		{
			licenses = StaticData.Licenses;
		}
		LicenseDataResult selectedLicense = licenses.FirstOrDefault((LicenseDataResult x) => x.Id == StaticData.LastLicenseId);
		if (selectedLicense != null && !selectedLicense.IsValid)
		{
			log.Write("Last used license is not valid. Sign in using Dataedo Desktop to check license.");
			return false;
		}
		Result result = new Result(HttpStatusCode.OK, isValid: true);
		try
		{
			_ = 1;
			try
			{
				result = await loginService.UseLicense(StaticData.Token, selectedLicense);
			}
			catch
			{
			}
			if (result.IsOK)
			{
				StaticData.License = new AppLicense(selectedLicense);
				StaticData.LicenseEnum = LicenseEnum.DataedoAccount;
				SessionDataHelper.Save();
				return true;
			}
			if (result.HasErrors)
			{
				string line = result.Errors.Message + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, result.Errors.Errors.SelectMany((KeyValuePair<string, string[]> x) => x.Value));
				log.Write(line);
			}
			else if (result.ShouldProposeTryAgain)
			{
				string line2 = "Unable to use license in at this time. Please try again in a few minutes.";
				log.Write(line2);
			}
			else
			{
				string line3 = "Unable to use license.";
				log.Write(line3);
			}
		}
		catch (Exception exception2)
		{
			GeneralHandlingSupport.StoreResult(GeneralExceptionHandling.Handle(exception2, HandlingMethodEnumeration.HandlingMethod.NoAction));
		}
		return false;
	}

	private void HandleImportCommand(Dataedo.App.Tools.CommandLine.Xml.Import command, CommandsProcessorResult result, Dataedo.App.Tools.CommandLine.Tools.Log log, Form owner = null)
	{
		result.ValidCommandsCount++;
		int num = new ImportProcessor(log, commandsFilePath).Process(command, owner);
		result.ErrorsCount += num;
		result.FailedCommandsCount += ((num > 0) ? 1 : 0);
	}

	private void HandleExportCommand(ExportBase command, CommandsProcessorResult result, Dataedo.App.Tools.CommandLine.Tools.Log log, Form owner = null)
	{
		result.ValidCommandsCount++;
		ExportProcessor exportProcessor = null;
		if (command is ExportVersion1)
		{
			exportProcessor = new ExportProcessorVersion1(log, commandsFilePath);
		}
		else if (command is ExportVersion2)
		{
			exportProcessor = new ExportProcessorVersion2(log, commandsFilePath);
		}
		int num = exportProcessor.Process(command, owner);
		result.ErrorsCount += num;
		result.FailedCommandsCount += ((num > 0) ? 1 : 0);
	}
}
