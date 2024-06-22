using System;

namespace Dataedo.App.Classes.Synchronize;

public class ConnectionResult
{
	public Exception Exception { get; set; }

	public bool FailedConnection { get; set; }

	public bool IsSuccess
	{
		get
		{
			if (Exception == null)
			{
				return !FailedConnection;
			}
			return false;
		}
	}

	public bool IsWarning { get; set; }

	public string Message { get; set; }

	public string NewConnectionString { get; set; }

	public string MessageSimple { get; set; }

	public string Details { get; set; }

	public object Connection { get; set; }

	public ConnectionResult(Exception exception, string message)
	{
		Exception = exception;
		Message = message;
	}

	public ConnectionResult(Exception exception, string message, string details)
		: this(exception, message)
	{
		Details = details;
	}

	public ConnectionResult(Exception exception, string message, bool failedConnection)
		: this(exception, message)
	{
		FailedConnection = failedConnection;
	}

	public ConnectionResult(Exception exception, string message, object connection)
	{
		Exception = exception;
		Message = message;
		Connection = connection;
	}
}
