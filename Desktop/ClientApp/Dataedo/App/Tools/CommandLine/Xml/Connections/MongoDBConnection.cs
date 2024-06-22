using System;
using System.Xml.Serialization;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.Enums;
using Dataedo.ConfigurationFileHelperLibrary;
using Dataedo.Data.Commands.Enums;

namespace Dataedo.App.Tools.CommandLine.Xml.Connections;

public class MongoDBConnection : ConnectionBase
{
	[XmlIgnore]
	public override int? Timeout
	{
		get
		{
			return GetCurrentModel().Timeout;
		}
		set
		{
			GetCurrentModel().Timeout = value;
		}
	}

	[XmlElement]
	public GeneralConnectionTypeEnum.GeneralConnectionType ConnectionType { get; set; }

	[XmlElement]
	public MongoDBConnectionValues Values { get; set; }

	[XmlElement]
	public MongoDBConnectionConnectionString ConnectionString { get; set; }

	public override DatabaseType GetConnectionCommandType()
	{
		return DatabaseType.MongoDB;
	}

	public override string GetConnectionString(bool withPooling, bool encrypt, bool trustServerCertificate)
	{
		return GetCurrentModel().GetConnectionString(withPooling, encrypt, trustServerCertificate);
	}

	public override string GetHost()
	{
		return GetCurrentModel().GetHost();
	}

	public override string GetDatabase()
	{
		return GetCurrentModel().GetDatabase();
	}

	public override string GetDatabaseFull()
	{
		return GetCurrentModel().GetDatabaseFull();
	}

	public override bool? GetIsWindowsAuthentication()
	{
		return null;
	}

	public override string GetLogin()
	{
		return GetCurrentModel().GetDatabaseFull();
	}

	public override string GetPassword()
	{
		return GetCurrentModel().GetPassword();
	}

	public override void SetConnection(LoginInfo loginInfo)
	{
		throw new NotSupportedException("MongoDB repository is not supported.");
	}

	public override void SetConnection(DatabaseRow databaseRow)
	{
		ConnectionType = databaseRow.GeneralConnectionType;
		if (ConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.Values)
		{
			Values = new MongoDBConnectionValues();
		}
		else if (ConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString)
		{
			ConnectionString = new MongoDBConnectionConnectionString();
		}
		GetCurrentModel().SetConnection(databaseRow);
	}

	public override bool SetDataInDatabaseRow(DatabaseRow databaseRow)
	{
		GetCurrentModel().SetDataInDatabaseRow(databaseRow);
		if (string.IsNullOrEmpty(databaseRow.Name))
		{
			return MongoDBSupport.PrepareAllMongoDbSchemas(databaseRow, databaseRow.GetDatabases());
		}
		return true;
	}

	private ConnectionBase GetCurrentModel()
	{
		if (ConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.Values)
		{
			return Values;
		}
		if (ConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString)
		{
			return ConnectionString;
		}
		throw new ArgumentException("Provided connection type is not valid.");
	}
}
