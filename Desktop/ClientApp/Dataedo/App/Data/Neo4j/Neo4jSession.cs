using System;
using System.Collections.Generic;
using System.Linq;
using Neo4j.Driver;

namespace Dataedo.App.Data.Neo4j;

public class Neo4jSession : IDisposable
{
	private Action<SessionConfigBuilder> action;

	private readonly string connectionString;

	private bool connectionInitialized;

	private IDriver driver;

	private ISession session;

	private ITransaction transaction;

	public Neo4jSession(string connectionString)
	{
		this.connectionString = connectionString;
	}

	public void InitConnection(Action<SessionConfigBuilder> action)
	{
		Dictionary<string, string> connectionDict = SplitConnectionString(connectionString);
		this.action = action;
		IAuthToken authToken = CreateAuthToken(connectionDict);
		Uri uri = GetUri(connectionDict);
		driver = GraphDatabase.Driver(uri, authToken);
		session = driver.Session(this.action);
		transaction = session.BeginTransaction();
		connectionInitialized = true;
	}

	public void InitConnection()
	{
		Action<SessionConfigBuilder> action = CreateaDatabaseAction(SplitConnectionString(connectionString));
		InitConnection(action);
	}

	public IResult Run(string query)
	{
		if (!connectionInitialized)
		{
			InitConnection();
		}
		return transaction.Run(query);
	}

	public void Commit()
	{
		if (connectionInitialized)
		{
			transaction.Commit();
		}
	}

	public void Dispose()
	{
		TransactionSafeDispose();
		SessionSafeDispose();
		DriverSafeDispose();
	}

	private void TransactionSafeDispose()
	{
		if (transaction != null)
		{
			transaction.Dispose();
		}
	}

	private void SessionSafeDispose()
	{
		if (session != null)
		{
			session.Dispose();
		}
	}

	private void DriverSafeDispose()
	{
		if (driver != null)
		{
			driver.Dispose();
		}
	}

	private static Dictionary<string, string> SplitConnectionString(string connectionString)
	{
		return (from part in connectionString.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries)
			select part.Split('=')).ToDictionary((string[] parts) => parts[0], (string[] parts) => parts[1]);
	}

	private static Uri GetUri(Dictionary<string, string> connectionDict)
	{
		return new Uri(connectionDict["host"] ?? "");
	}

	private static Action<SessionConfigBuilder> CreateaDatabaseAction(Dictionary<string, string> connectionDict)
	{
		if (connectionDict.TryGetValue("database", out var value))
		{
			if (string.Equals(value, "system", StringComparison.OrdinalIgnoreCase))
			{
				throw new ArgumentException("Cannot document a system database");
			}
			return SessionConfigBuilder.ForDatabase(value);
		}
		return null;
	}

	private static IAuthToken CreateAuthToken(Dictionary<string, string> connectionDict)
	{
		string text = connectionDict["login"];
		if (!string.IsNullOrEmpty(text))
		{
			string password = connectionDict["password"];
			return AuthTokens.Basic(text, password);
		}
		return AuthTokens.None;
	}
}
