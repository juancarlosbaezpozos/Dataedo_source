using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlServerCe;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Tools.GeneralHandling;
using Devart.Data.Oracle;
using Microsoft.Data.SqlClient;

namespace Dataedo.App.Tools.Exceptions;

internal static class GeneralExceptionHandling
{
	private static readonly HashSet<int> sqlPasswordPolicyErrorCodes = new HashSet<int>
	{
		15113, 15116, 15117, 15118, 18463, 18464, 18465, 18466, 18467, 18468,
		18487, 18488
	};

	private static HashSet<int> sqlCeInternalErrorCodes = new HashSet<int>
	{
		25051, 25053, 25058, 25067, 25087, 25105, 25107, 25111, 25116, 25118,
		25122, 25125, 25133
	};

	public static HandlingResult HandleOracle(Exception exception, OracleException oracleException, string caption = null, string prefixMessage = null, string suffixMessage = null)
	{
		HandlingResult handlingResult;
		switch (oracleException.Code)
		{
		case 1005:
			handlingResult = new HandlingResult(caption, "Password cannot be empty.", exception.Message);
			break;
		case 3113:
			handlingResult = new HandlingResult(caption, "Connection failed due to ORA-03113 end-of-file on communication channel." + Environment.NewLine + Environment.NewLine + "Try increasing timeout for the operation.", exception.Message);
			break;
		case 1013:
			handlingResult = new HandlingResult(caption, "Operation timed out. Try again after increasing the timeout value." + Environment.NewLine + Environment.NewLine + "If the issue persists, <href=" + Links.SupportContact + ">contact us</href>.", string.Empty);
			handlingResult.MessageSimple = "Operation timed out. Try again after increasing the timeout value." + Environment.NewLine + Environment.NewLine + "If the issue persists, contact us at: " + Links.SupportContactMailAddress + ".";
			break;
		default:
			handlingResult = OtherExceptionHandler(exception, caption, prefixMessage, suffixMessage);
			break;
		}
		return handlingResult;
	}

	public static HandlingResult HandleSqlServer(Exception exception, SqlException sqlException, string caption = null, string prefixMessage = null, string suffixMessage = null)
	{
		HandlingResult handlingResult;
		if (sqlPasswordPolicyErrorCodes.Contains(sqlException.Number))
		{
			handlingResult = new HandlingResult(caption, prefixMessage + "An error with password policy occurred." + suffixMessage, sqlException.Message);
		}
		else if (sqlException.Number == -1 || sqlException.Number == 53)
		{
			handlingResult = new HandlingResult(caption, prefixMessage + "Unable to connect to repository.\nCheck server name and try again." + suffixMessage, sqlException.Message);
		}
		else if (sqlException.Number == 229)
		{
			handlingResult = new HandlingResult(caption, prefixMessage + "Permission denied." + suffixMessage, sqlException.Message);
		}
		else if (sqlException.Number == 4060)
		{
			if (sqlException.Message.ToLower().Contains("requested by the login"))
			{
				handlingResult = new HandlingResult(caption, prefixMessage + "Unable to connect to repository." + Environment.NewLine + "Specified database doesn't exist or you don't have access to it." + Environment.NewLine + "To grant access to existing repository please use <href=" + Paths.GetAdminConsolePath() + "> Administration Console </href>." + suffixMessage, sqlException.Message);
				handlingResult.MessageSimple = prefixMessage + "Unable to connect to repository." + Environment.NewLine + "Specified database doesn't exist or you don't have access to it." + Environment.NewLine + "To grant access to existing repository please use Administration Console: " + Paths.GetAdminConsolePath() + "." + suffixMessage;
			}
			else
			{
				handlingResult = new HandlingResult(caption, prefixMessage + "Unable to connect to repository.\nCheck repository name and try again." + suffixMessage, sqlException.Message);
			}
		}
		else if (sqlException.Number == 18452)
		{
			handlingResult = new HandlingResult(caption, prefixMessage + "Unable to connect to repository.\nCannot connect using windows credentials." + suffixMessage, sqlException.Message);
		}
		else if (sqlException.Number == 18470)
		{
			handlingResult = new HandlingResult(caption, prefixMessage + "Unable to connect to repository.\nSQL Server login is disabled.\nUse SQL Server administration tools to enable user or ask your database administrator." + suffixMessage, sqlException.Message);
		}
		else if (sqlException.Number == 18456 || sqlException.Number == 229)
		{
			handlingResult = new HandlingResult(caption, prefixMessage + "Unable to connect to repository.\nCheck login and password and try again." + suffixMessage, sqlException.Message);
		}
		else if (sqlException.Number == 208)
		{
			handlingResult = new HandlingResult(caption, prefixMessage + "Unable to connect to repository." + suffixMessage, "This is not valid Dataedo repository." + Environment.NewLine + Environment.NewLine + sqlException.Message);
		}
		else if (sqlException.Number == 17142)
		{
			handlingResult = new HandlingResult(caption, prefixMessage + "Unable to connect to repository." + suffixMessage, sqlException.Message);
		}
		else if (sqlException.Number == 1205)
		{
			handlingResult = new HandlingResult(caption, prefixMessage + "Unable to complete current operation." + Environment.NewLine + "Please try again." + suffixMessage, sqlException.Message);
		}
		else if (sqlException.Number == 40607)
		{
			handlingResult = new HandlingResult(caption, prefixMessage + "Unable to connect to repository." + suffixMessage, sqlException.Message);
		}
		else if (!(sqlException?.InnerException is Win32Exception))
		{
			handlingResult = ((!sqlException.Message.ToLower().Contains("cannot generate SSPI context")) ? OtherExceptionHandler(exception, caption, prefixMessage, suffixMessage) : new HandlingResult(caption, prefixMessage + "Unable to connect." + suffixMessage, sqlException.Message));
		}
		else
		{
			Win32Exception ex = sqlException?.InnerException as Win32Exception;
			handlingResult = (((ex == null || ex.NativeErrorCode != 121) && (ex == null || ex.NativeErrorCode != 258)) ? ((ex != null && ex.NativeErrorCode == 1225) ? new HandlingResult(caption, prefixMessage + "Unable to connect to repository." + Environment.NewLine + "The remote computer refused the network connection." + suffixMessage, sqlException.Message) : ((ex != null && ex.NativeErrorCode == 87) ? new HandlingResult(caption, prefixMessage + "Unable to connect to repository." + Environment.NewLine + "The connection parameters are invalid." + suffixMessage, sqlException.Message) : ((ex != null && ex.NativeErrorCode == 64) ? new HandlingResult(caption, prefixMessage + "Unable to connect to repository." + Environment.NewLine + "The specified network name is no longer available." + suffixMessage, sqlException.Message) : ((ex != null && ex.NativeErrorCode == 2) ? new HandlingResult(caption, prefixMessage + "Unable to connect to repository." + Environment.NewLine + "The server was not found or was not accessible." + suffixMessage, sqlException.Message) : ((ex != null && ex.NativeErrorCode == 1236) ? new HandlingResult(caption, prefixMessage + "Unable to connect to repository." + Environment.NewLine + "The network connection was aborted by the local system." + suffixMessage, sqlException.Message) : ((ex != null && ex.NativeErrorCode == 1326) ? new HandlingResult(caption, prefixMessage + "Unable to connect to repository." + Environment.NewLine + "Check server name, login, password and try again." + suffixMessage, sqlException.Message) : ((ex == null || ex.NativeErrorCode != 10054) ? OtherExceptionHandler(exception, caption, prefixMessage, suffixMessage) : new HandlingResult(caption, prefixMessage + "Unable to connect to repository." + Environment.NewLine + "An existing connection was forcibly closed by the remote host." + suffixMessage, sqlException.Message)))))))) : new HandlingResult(caption, prefixMessage + "Unable to connect to repository." + Environment.NewLine + "The operation timed out." + suffixMessage, sqlException.Message));
		}
		return handlingResult;
	}

	private static HandlingResult HandleSqlServerCe(Exception exception, SqlCeException sqlCeException, string caption = null, string prefixMessage = null, string suffixMessage = null)
	{
		if (sqlCeInternalErrorCodes.Contains(sqlCeException.NativeError))
		{
			return OtherExceptionHandler(sqlCeException, caption, prefixMessage + "Internal error of SQL Server Compact Edition occurred." + Environment.NewLine + suffixMessage);
		}
		if (sqlCeException.NativeError == 25046)
		{
			return new HandlingResult(caption, prefixMessage + "Unable to open file." + suffixMessage, "Unable to open file. File does not exist." + Environment.NewLine + Environment.NewLine + sqlCeException?.Message);
		}
		if (sqlCeException.NativeError == 25009)
		{
			return new HandlingResult(caption, prefixMessage + "Unable to open file." + suffixMessage, "Unable to open file. The path is not valid." + Environment.NewLine + Environment.NewLine + sqlCeException?.Message);
		}
		if (sqlCeException.NativeError == 25035)
		{
			return new HandlingResult(caption, prefixMessage + "Unable to open file." + suffixMessage, sqlCeException?.Message);
		}
		if (sqlCeException.NativeError == 25039)
		{
			return new HandlingResult(caption, prefixMessage + "Unable to open file." + suffixMessage, sqlCeException?.Message);
		}
		if (sqlCeException.NativeError == 25017 || sqlCeException.NativeError == 25011 || sqlCeException.Message.Contains("[ version ]"))
		{
			return new HandlingResult(caption, prefixMessage + "Unable to open file." + suffixMessage, "This is not valid Dataedo file." + Environment.NewLine + Environment.NewLine + sqlCeException.Message);
		}
		if (sqlCeException.NativeError == 25104)
		{
			return new HandlingResult(caption, prefixMessage + "Unable to open file." + Environment.NewLine + "The database file is larger than the configured maximum database size (4091 MB)." + suffixMessage, sqlCeException?.Message);
		}
		if (sqlCeException.StackTrace.Contains("LoadNativeBinaries()") || sqlCeException.StackTrace.Contains("ThrowIfNativeLibraryNotLoaded()"))
		{
			return LoadNativeBinariesExceptionHandling(caption, prefixMessage, suffixMessage, sqlCeException?.Message);
		}
		return OtherExceptionHandler(exception, caption, prefixMessage, suffixMessage);
	}

	private static HandlingResult LoadNativeBinariesExceptionHandling(string caption, string prefixMessage, string suffixMessage, string details)
	{
		HandlingResult handlingResult = new HandlingResult(caption, prefixMessage + "Unable to load the native components of SQL Server Compact." + Environment.NewLine + "Please try reinstalling Dataedo or contact <href=" + Links.GetSupportMailtoLink() + ">support@dataedo.com</href>." + suffixMessage, details);
		handlingResult.MessageSimple = prefixMessage + "Unable to load the native components of SQL Server Compact." + Environment.NewLine + "Please try reinstalling Dataedo or contact support@dataedo.com." + suffixMessage;
		return handlingResult;
	}

	public static HandlingResult Handle(Exception exception, Form handlingWindowOwner = null)
	{
		return Handle(exception, null, null, null, HandlingMethodEnumeration.HandlingMethod.ShowMessageBox, handlingWindowOwner);
	}

	public static HandlingResult Handle(Exception exception, HandlingMethodEnumeration.HandlingMethod handlingMethod, Form handlingWindowOwner = null)
	{
		return Handle(exception, null, null, null, handlingMethod, handlingWindowOwner);
	}

	public static HandlingResult Handle(Exception exception, string prefixMessage, Form handlingWindowOwner = null)
	{
		return Handle(exception, prefixMessage, null, null, HandlingMethodEnumeration.HandlingMethod.ShowMessageBox, handlingWindowOwner);
	}

	public static HandlingResult Handle(Exception exception, string prefixMessage, string caption, Form handlingWindowOwner = null)
	{
		return Handle(exception, prefixMessage, caption, null, HandlingMethodEnumeration.HandlingMethod.ShowMessageBox, handlingWindowOwner);
	}

	public static HandlingResult Handle(Exception exception, string prefixMessage, string caption, string suffixMessage, Form handlingWindowOwner = null)
	{
		return Handle(exception, prefixMessage, caption, suffixMessage, HandlingMethodEnumeration.HandlingMethod.ShowMessageBox, handlingWindowOwner);
	}

	public static HandlingResult Handle(Exception exception, string prefixMessage, string caption, string suffixMessage, HandlingMethodEnumeration.HandlingMethod handlingMethod, Form handlingWindowOwner = null)
	{
		if (handlingMethod != HandlingMethodEnumeration.HandlingMethod.LogInErrorLog)
		{
			handlingMethod = GeneralHandlingSupport.OverrideHandlingMethod ?? handlingMethod;
		}
		prefixMessage = (string.IsNullOrEmpty(prefixMessage) ? prefixMessage : (prefixMessage + Environment.NewLine));
		suffixMessage = (string.IsNullOrEmpty(suffixMessage) ? suffixMessage : (Environment.NewLine + suffixMessage));
		if (string.IsNullOrEmpty(caption))
		{
			caption = "Error";
		}
		Exception ex = null;
		SqlCeException ex2 = null;
		SqlException ex3 = null;
		OracleException ex4 = null;
		ex = exception;
		ex2 = ex as SqlCeException;
		while (ex2 == null && ex?.InnerException != null)
		{
			ex = ex?.InnerException;
			ex2 = ex as SqlCeException;
		}
		if (ex2 == null)
		{
			ex = exception;
			ex3 = ex as SqlException;
			while (ex3 == null && ex?.InnerException != null)
			{
				ex = ex?.InnerException;
				ex3 = ex as SqlException;
			}
		}
		if (ex3 == null)
		{
			ex = exception;
			ex4 = ex as OracleException;
			while (ex4 == null && ex?.InnerException != null)
			{
				ex = ex?.InnerException;
				ex4 = ex as OracleException;
			}
		}
		HandlingResult handlingResult = ((ex2 != null) ? HandleSqlServerCe(exception, ex2, caption, prefixMessage, suffixMessage) : ((ex3 != null) ? HandleSqlServer(exception, ex3, caption, prefixMessage, suffixMessage) : ((ex4 != null) ? HandleOracle(exception, ex4, caption, prefixMessage, suffixMessage) : ((exception is AggregateException) ? OtherExceptionHandler(exception, caption, prefixMessage, suffixMessage) : ((exception is OutOfMemoryException || ex is OutOfMemoryException) ? new HandlingResult(caption, "Out of memory.", exception.Message) : ((exception.Source == "MySql.Data" && exception.TargetSite?.Name == "HandleTimeoutOrThreadAbort") ? new HandlingResult(caption, prefixMessage + "Timeout expired. The timeout period elapsed prior to completion of the operation or the server is not responding." + suffixMessage, null) : ((exception == null || exception.InnerException?.ToString()?.Contains("SqlServerCe.NativeMethods.LoadNativeBinaries()") != true) ? OtherExceptionHandler(exception, caption, prefixMessage, suffixMessage) : LoadNativeBinariesExceptionHandling(caption, prefixMessage, suffixMessage, exception?.Message + ((exception != null && exception.InnerException?.Message != null) ? (Environment.NewLine + exception.InnerException.Message) : string.Empty)))))))));
		handlingResult.Process(handlingMethod, handlingWindowOwner);
		if (handlingMethod == HandlingMethodEnumeration.HandlingMethod.NoActionStoreExceptions)
		{
			GeneralHandlingSupport.StoreResult(handlingResult);
		}
		return handlingResult;
	}

	private static HandlingResult OtherExceptionHandler(Exception exception, string caption = null, string prefixMessage = null, string suffixMessage = null)
	{
		HandlingResult handlingResult;
		if (string.IsNullOrEmpty(prefixMessage) && string.IsNullOrEmpty(suffixMessage))
		{
			handlingResult = new HandlingResult(caption, "An unexpected error occurred.", null);
		}
		else
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!string.IsNullOrEmpty(prefixMessage) && (exception == null || exception.Message?.StartsWith(prefixMessage?.Trim()) != true))
			{
				stringBuilder.Append(prefixMessage);
			}
			stringBuilder.Append(exception?.Message);
			stringBuilder.Append(suffixMessage);
			handlingResult = new HandlingResult(caption, stringBuilder.ToString(), null);
		}
		handlingResult.Exception = exception;
		handlingResult.IsException = true;
		handlingResult.IsUnhandled = true;
		return handlingResult;
	}

	public static bool IsSockedAccessDenied(Exception exception, ref string message)
	{
		bool result = false;
		SocketException ex = exception as SocketException;
		while (ex == null && exception?.InnerException != null)
		{
			exception = exception?.InnerException;
			if (exception != null)
			{
				ex = exception as SocketException;
				if (ex != null && ex.SocketErrorCode == SocketError.AccessDenied)
				{
					result = true;
					message = ex.Message;
					break;
				}
			}
		}
		return result;
	}
}
