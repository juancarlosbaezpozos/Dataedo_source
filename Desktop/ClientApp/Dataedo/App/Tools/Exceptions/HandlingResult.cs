using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.CustomMessageBox;
using Dataedo.Log.Error;
using Dataedo.Log.Execution;

namespace Dataedo.App.Tools.Exceptions;

public class HandlingResult
{
	public string Title { get; set; }

	public string Message { get; set; }

	public string MessageSimple { get; set; }

	public string Details { get; set; }

	public Exception Exception { get; set; }

	public bool IsException { get; set; }

	public bool IsUnhandled { get; set; }

	public bool IsStored { get; set; }

	public DateTime TimeStamp { get; private set; }

	public HandlingResult(string title, string message, string details)
	{
		Title = title;
		Message = message;
		Details = details;
		TimeStamp = DateTime.Now;
	}

	public HandlingResult(bool isException = true, bool isUnhandled = true)
	{
		IsException = isException;
		IsUnhandled = IsUnhandled;
		TimeStamp = DateTime.Now;
	}

	public void Process(HandlingMethodEnumeration.HandlingMethod handlingMethod, Form handlingWindowOwner = null)
	{
		HandleExecutionLog();
		if (handlingMethod != HandlingMethodEnumeration.HandlingMethod.NoActionAndThrowIfOtherException || !IsException)
		{
			switch (handlingMethod)
			{
			case HandlingMethodEnumeration.HandlingMethod.ThrowAlways:
				break;
			case HandlingMethodEnumeration.HandlingMethod.ShowMessageBox:
			case HandlingMethodEnumeration.HandlingMethod.ShowMessageBoxAndThrowIfOtherException:
				if (!IsException)
				{
					if (ExecutionLog.IsLogEnabled)
					{
						Stopwatch stopwatch = new Stopwatch();
						string[] additionalData = new string[3] { Title, Message, Details };
						ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP MESSAGE", null, null, 2, 1, additionalData);
					}
					GeneralMessageBoxesHandling.Show(Message, Title, MessageBoxButtons.OK, MessageBoxIcon.Hand, Details, 1, handlingWindowOwner);
					return;
				}
				if (handlingMethod == HandlingMethodEnumeration.HandlingMethod.ShowMessageBox)
				{
					if (ExecutionLog.IsLogEnabled)
					{
						Stopwatch stopwatch2 = new Stopwatch();
						string[] additionalData = new string[5]
						{
							Title,
							Message,
							StaticData.RepositoryTypeString,
							StaticData.CrashedDatabaseTypeString,
							Exception.ToString()
						};
						ExecutionLog.WriteExecutionLog(stopwatch2, "DATAEDO.APP EXCEPTION", null, null, 2, 1, additionalData);
					}
					CustomMessageBoxForm.ShowException(Exception, Message, StaticData.RepositoryTypeString, StaticData.RepositoryCollation, StaticData.ServerCollation, StaticData.CrashedDatabaseTypeString, StaticData.CrashedDBMSVersion, StaticData.License?.PackageCode, StaticData.License?.AccountId, Title, null, handlingWindowOwner);
					return;
				}
				throw Exception;
			case HandlingMethodEnumeration.HandlingMethod.LogInErrorLog:
				HandleErrorLog();
				return;
			default:
				return;
			}
		}
		throw Exception;
	}

	public DialogResult? Process(HandlingMethodEnumeration.HandlingMethod handlingMethod, MessageBoxButtons messageBoxButtons, MessageBoxIcon messageBoxIcon, int numberOfSelectedButton, string serviceForErrorReporting, Form owner, MessageBoxParameters parameters = null)
	{
		HandleExecutionLog();
		if (ExecutionLog.IsLogEnabled)
		{
			if (!IsException)
			{
				Stopwatch stopwatch = new Stopwatch();
				string[] additionalData = new string[3] { Title, Message, Details };
				ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP MESSAGE", null, null, 2, 1, additionalData);
			}
			else
			{
				Stopwatch stopwatch2 = new Stopwatch();
				string[] additionalData = new string[5]
				{
					Title,
					Message,
					StaticData.RepositoryTypeString,
					StaticData.CrashedDatabaseTypeString,
					Exception.ToString()
				};
				ExecutionLog.WriteExecutionLog(stopwatch2, "DATAEDO.APP EXCEPTION", null, null, 2, 1, additionalData);
			}
		}
		if (handlingMethod != HandlingMethodEnumeration.HandlingMethod.NoActionAndThrowIfOtherException || !IsException)
		{
			switch (handlingMethod)
			{
			case HandlingMethodEnumeration.HandlingMethod.ThrowAlways:
				break;
			case HandlingMethodEnumeration.HandlingMethod.ShowMessageBox:
			case HandlingMethodEnumeration.HandlingMethod.ShowMessageBoxAndThrowIfOtherException:
				if (!IsException)
				{
					return CustomMessageBoxForm.Show(Message, Title, messageBoxButtons, messageBoxIcon, Details, numberOfSelectedButton, owner, parameters);
				}
				if (handlingMethod == HandlingMethodEnumeration.HandlingMethod.ShowMessageBox)
				{
					return CustomMessageBoxForm.ShowException(Exception, Message, StaticData.RepositoryTypeString, StaticData.RepositoryCollation, StaticData.ServerCollation, StaticData.CrashedDatabaseTypeString, StaticData.CrashedDBMSVersion, StaticData.License?.PackageCode, StaticData.License?.AccountId, Title, serviceForErrorReporting, owner);
				}
				throw Exception;
			default:
				return null;
			}
		}
		throw Exception;
	}

	public override string ToString()
	{
		return ToString(showExceptionAlways: false);
	}

	public string ToString(bool showExceptionAlways)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string text = TimeStamp.ToString("[yyyy-MM-dd HH:mm:ss]") + " ";
		if (!string.IsNullOrEmpty(Title))
		{
			stringBuilder.AppendLine(text + Title + ":");
			text = string.Empty;
		}
		string text2 = null;
		if (!string.IsNullOrEmpty(MessageSimple))
		{
			text2 = MessageSimple;
		}
		else if (!string.IsNullOrEmpty(Message))
		{
			text2 = Message;
		}
		if (!string.IsNullOrEmpty(text2))
		{
			stringBuilder.AppendLine(text + text2);
			text = string.Empty;
		}
		if (!string.IsNullOrEmpty(Details) && !text2.StartsWith(Details.Trim()))
		{
			if (!string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(text2))
			{
				stringBuilder.AppendLine("Details:");
			}
			stringBuilder.AppendLine(text + Details);
			text = string.Empty;
		}
		if (IsException && (IsUnhandled || showExceptionAlways))
		{
			stringBuilder.AppendLine(text + "Exception:");
			stringBuilder.AppendLine(Exception.ToString());
		}
		return stringBuilder.ToString();
	}

	private void HandleExecutionLog()
	{
		if (!IsException)
		{
			Stopwatch stopwatch = new Stopwatch();
			string[] additionalData = new string[3] { Title, Message, Details };
			ExecutionLog.WriteExecutionLog(stopwatch, "DATAEDO.APP MESSAGE", null, null, 2, 1, additionalData);
		}
		else
		{
			Stopwatch stopwatch2 = new Stopwatch();
			string[] additionalData = new string[5]
			{
				Title,
				Message,
				StaticData.RepositoryTypeString,
				StaticData.CrashedDatabaseTypeString,
				Exception.ToString()
			};
			ExecutionLog.WriteExecutionLog(stopwatch2, "DATAEDO.APP EXCEPTION", null, null, 2, 1, additionalData);
		}
	}

	private void HandleErrorLog()
	{
		if (!IsException)
		{
			string[] additionalData = new string[3] { Title, Message, Details };
			ErrorLog.WriteExecutionLog("DATAEDO.APP MESSAGE", null, null, 2, 1, additionalData);
		}
		else
		{
			string[] additionalData = new string[6]
			{
				Title,
				Message,
				StaticData.RepositoryTypeString,
				StaticData.CrashedDatabaseTypeString,
				StaticData.CrashedDBMSVersion,
				Exception.ToString()
			};
			ErrorLog.WriteExecutionLog("DATAEDO.APP EXCEPTION", null, null, 2, 1, additionalData);
		}
	}
}
