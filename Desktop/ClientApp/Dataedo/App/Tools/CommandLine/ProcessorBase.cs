using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Tools.CommandLine.Tools;
using Dataedo.App.Tools.CommandLine.Xml.Commands.Base;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.GeneralHandling;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;
using Dataedo.Data.Factories;

namespace Dataedo.App.Tools.CommandLine;

public abstract class ProcessorBase
{
	protected Dataedo.App.Tools.CommandLine.Tools.Log Log { get; set; }

	protected string CommandsFilePath { get; set; }

	public ProcessorBase(Dataedo.App.Tools.CommandLine.Tools.Log log, string commandsFilePath)
	{
		Log = log;
		CommandsFilePath = commandsFilePath;
	}

	public int Process(CommandBase command, Form owner = null)
	{
		GeneralHandlingSupport.OverrideHandlingMethod = HandlingMethodEnumeration.HandlingMethod.NoActionStoreExceptions;
		int result = ProcessInternal(command, owner);
		GeneralHandlingSupport.ResetOverrideHandlingMethod();
		GeneralHandlingSupport.ClearStoredHandlingResults();
		return result;
	}

	protected abstract int ProcessInternal(CommandBase command, Form owner = null);

	protected bool InitializeRepository(RepositoryCommandBase command, Form owner = null)
	{
		bool hasAnyErrors = false;
		List<string> list = new List<string>();
		if (command.RepositoryConnection == null)
		{
			list.Add("The repository connection is not provided.");
			hasAnyErrors = true;
		}
		if (HandleErrors(hasAnyErrors, list))
		{
			return false;
		}
		bool flag = command.RepositoryConnection.GetConnectionCommandType() == DatabaseType.SqlServerCe;
		if (!File.Exists(ConfigurationFileHelper.GetConfPath()))
		{
			ConfigurationFileHelper.CreateConfFile();
		}
		LastConnectionInfo.LOGIN_INFO = new LoginInfo(emptyValues: false);
		ConnectorsVersion.LoadFromXML();
		if (!flag)
		{
			list.Add(CheckIfIsNull(command.RepositoryConnection.GetHost(), "host", ref hasAnyErrors));
		}
		else
		{
			list.Add(CheckIfIsNull(command.RepositoryConnection.GetHost(), "path", ref hasAnyErrors));
		}
		list.Add(CheckIfIsNull(command.RepositoryConnection.GetDatabase(), "database", ref hasAnyErrors));
		if (command.RepositoryConnection.GetIsWindowsAuthentication() == false)
		{
			list.Add(CheckIfIsNull(command.RepositoryConnection.GetLogin(), "login", ref hasAnyErrors));
			list.Add(CheckIfIsNull(command.RepositoryConnection.GetPassword(), "password", ref hasAnyErrors));
		}
		if (HandleErrors(hasAnyErrors, list))
		{
			return false;
		}
		SqlServerConnectionModeEnum.SqlServerConnectionMode sqlServerConnectionMode = SqlServerConnectionModeEnum.StringToTypeOrDefault(command.RepositoryConnection.GetConnectionMode());
		string empty = string.Empty;
		switch (sqlServerConnectionMode)
		{
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.ForceEncryptionRequireTrustedCertificate:
			empty = command.RepositoryConnection.GetConnectionString(withPooling: true, encrypt: true, trustServerCertificate: false);
			break;
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.ForceEncryptionTrustServerCertificate:
			empty = command.RepositoryConnection.GetConnectionString(withPooling: true, encrypt: true, trustServerCertificate: true);
			break;
		case SqlServerConnectionModeEnum.SqlServerConnectionMode.EncryptConnectionIfPossible:
			empty = command.RepositoryConnection.GetConnectionString(withPooling: true, encrypt: true, trustServerCertificate: true);
			if (LoginHelper.CheckIfRepositoryExists(empty, command.RepositoryConnection.GetDatabase(), owner) == RepositoryExistsStatusEnum.Error)
			{
				empty = command.RepositoryConnection.GetConnectionString(withPooling: true, encrypt: false, trustServerCertificate: true);
			}
			break;
		default:
			return false;
		}
		if (!flag)
		{
			Exception exception = null;
			RepositoryExistsStatusEnum repositoryExistsStatusEnum = LoginHelper.CheckIfRepositoryExists(empty, command.RepositoryConnection.GetDatabase(), ref exception, showMessage: true, owner);
			if (repositoryExistsStatusEnum == RepositoryExistsStatusEnum.NotExists || repositoryExistsStatusEnum == RepositoryExistsStatusEnum.Error)
			{
				if (exception != null)
				{
					string text = Environment.NewLine + ((exception != null && exception.InnerException?.Message != null) ? (exception.Message + Environment.NewLine + exception.InnerException.Message) : exception.Message);
					Log.Write("Unable to connect to repository." + text, "Details:", exception.ToString());
				}
				return false;
			}
		}
		CommandsFactory.GetCommands(command.RepositoryConnection.GetConnectionCommandType(), empty).Select.Repository.GetAllVersions();
		StaticData.Initialize(command.RepositoryConnection.GetConnectionCommandType(), empty);
		DB.ReloadClasses();
		StaticData.DataedoConnectionString = empty;
		StaticData.RepositoryType = command.RepositoryConnection.GetConnectionCommandType();
		Licenses.Initialize(StaticData.Commands);
		if (!StaticData.LicenseHelper.CheckRepositoryVersion(StaticData.DataedoConnectionString, ProgramVersion.Major, ProgramVersion.Minor, ProgramVersion.Build, flag, out var _))
		{
			return false;
		}
		return true;
	}

	protected int LogHandlingResults(List<HandlingResult> list)
	{
		int count = list.Count;
		if (count > 0)
		{
			foreach (HandlingResult item in list)
			{
				Log.Write(item);
			}
			return count;
		}
		return count;
	}

	private string CheckIfIsNull<T>(T value, string name, ref bool hasAnyErrors)
	{
		if (value == null)
		{
			hasAnyErrors = true;
			return "The " + name + " for the repository connection is not provided.";
		}
		return null;
	}

	private bool HandleErrors(bool hasAnyErrors, List<string> errorMessages)
	{
		if (hasAnyErrors)
		{
			Log.Write("Errors:");
			Log.Write(errorMessages.Where((string x) => !string.IsNullOrEmpty(x)).ToArray());
			Log.WriteSimple();
			return true;
		}
		return false;
	}
}
