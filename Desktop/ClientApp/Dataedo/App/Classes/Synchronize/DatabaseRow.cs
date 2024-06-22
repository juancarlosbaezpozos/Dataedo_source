using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Dataedo.App.Data.General;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.DatabasesSupport.DatabaseTypes;
using Dataedo.App.DatabasesSupport.DatabaseTypes.Interfaces;
using Dataedo.App.Enums;
using Dataedo.App.Interfaces.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Data.Commands.Enums;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Documentations;
using Dataedo.Model.Data.PersonalSettings;
using Dataedo.Shared.Enums;
using Devart.Data.Oracle;
using DevExpress.XtraSplashScreen;
using Microsoft.Data.SqlClient;
using Snowflake.Data.Client;

namespace Dataedo.App.Classes.Synchronize;

public class DatabaseRow : DatabaseRowBase, IDatabaseRow
{
	public ObservableCollection<ObjectRow> tableRows;

	private string name;

	private string _Password;

	public bool IsValidDatabase { get; protected set; }

	public bool IsWelcomeDocumentation => base.Id == 0;

	public object Tag { get; set; }

	public override string Name
	{
		get
		{
			return name;
		}
		set
		{
			name = value;
			if (UseDifferentSchema == true && HasMultipleSchemas == true)
			{
				Schemas = GetSchemasNames(value).ToList();
				return;
			}
			Schemas = new List<string> { name };
		}
	}

	public string UserProvidedConnectionString { get; set; }

	public string ConnectionRole { get; set; }

	public SSLSettings SSLSettings { get; set; }

	public string SSLType { get; set; }

	public string NameForDisplaying
	{
		get
		{
			if (UseDifferentSchema != true || HasMultipleSchemas != true)
			{
				return name;
			}
			return $"{Schemas.Count} schemas";
		}
	}

	public List<string> Names
	{
		get
		{
			if (UseDifferentSchema != true || HasMultipleSchemas != true)
			{
				return new List<string> { Name };
			}
			return Schemas;
		}
	}

	public string UserControlName { get; set; }

	public ConnectionTypeEnum.ConnectionType ConnectionType { get; set; }

	public bool IsDirectConnect => ConnectionType == ConnectionTypeEnum.ConnectionType.Direct;

	public InstanceIdentifierEnum.InstanceIdentifier InstanceIdentifier { get; set; }

	public string InstanceIdentifierString
	{
		get
		{
			if (Type != SharedDatabaseTypeEnum.DatabaseType.Vertica && Type != SharedDatabaseTypeEnum.DatabaseType.Teradata)
			{
				return InstanceIdentifierEnum.TypeToString(InstanceIdentifier);
			}
			return null;
		}
	}

	public string Host { get; set; }

	public string User { get; set; }

	public bool? UseDifferentSchema { get; set; }

	public bool? HasMultipleSchemas { get; set; }

	public bool ImportAllSchemas { get; set; }

	public bool? ShowSchema { get; set; }

	public bool? ShowSchemaOverride { get; set; }

	public List<string> Schemas { get; set; }

	public string DbmsVersion { get; set; }

	public bool MongoDBIsSrv { get; set; }

	public string MongoDBAuthenticationDatabase { get; set; }

	public string MongoDBReplicaSet { get; set; }

	public string MultiHost { get; set; }

	public GeneralConnectionTypeEnum.GeneralConnectionType GeneralConnectionType { get; set; }

	public bool UseSchema
	{
		get
		{
			List<string> schemas = Schemas;
			if (schemas == null)
			{
				return false;
			}
			return schemas.Count > 0;
		}
	}

	public string SchemasListForCondition => ((Schemas != null) ? ("'" + string.Join("', '", Schemas) + "'") : ("'" + Name.ToUpper() + "'")).ToUpper();

	public string SchemasList
	{
		get
		{
			if (Schemas == null)
			{
				return "\"" + Name.ToUpper() + "\"";
			}
			return PrepareSchemasList(Schemas);
		}
	}

	public string HasMultipleSchemasForCondition
	{
		get
		{
			if (HasMultipleSchemas != true)
			{
				return "0";
			}
			return "1";
		}
	}

	public string Password
	{
		get
		{
			if (string.IsNullOrEmpty(_Password))
			{
				return _Password;
			}
			try
			{
				return new SimpleAES().DecryptString(_Password);
			}
			catch
			{
				return string.Empty;
			}
		}
		set
		{
			if (string.IsNullOrEmpty(value))
			{
				_Password = value;
			}
			try
			{
				SimpleAES simpleAES = new SimpleAES();
				_Password = simpleAES.EncryptToString(value);
			}
			catch
			{
				_Password = string.Empty;
			}
		}
	}

	public string PasswordEncrypted
	{
		get
		{
			return _Password;
		}
		set
		{
			_Password = value;
		}
	}

	public FilterRulesCollection Filter { get; set; }

	public int SelectedObjectsCount { get; set; }

	public SharedDatabaseTypeEnum.DatabaseType? Type { get; set; }

	public bool SynchronizeColumnsComputedFormula => DatabaseSupportFactory.GetDatabaseSupport(Type).ShouldSynchronizeComputedFormula(Connection);

	public bool SynchronizeParameters => DatabaseSupportFactory.GetDatabaseSupport(Type).ShouldSynchronizeParameters(Connection);

	public int? Port { get; set; }

	public string ServiceName { get; set; }

	public string OracleSid { get; set; }

	public AuthenticationType.AuthenticationTypeEnum SelectedAuthenticationType { get; set; }

	public bool WindowsAutentication { get; set; }

	public string Perspective { get; set; }

	public string Param1 { get; set; }

	public string Param2 { get; set; }

	public string Param3 { get; set; }

	public string Param4 { get; set; }

	public string Param5 { get; set; }

	public string Param6 { get; set; }

	public string Param7 { get; set; }

	public string Param8 { get; set; }

	public string Param9 { get; set; }

	public string Param10 { get; set; }

	public string ConnectionStringForGettingDatabasesList => GetConnectionStringForGettingDatabasesList();

	public string ConnectionString => GetConnectionString();

	public object Connection { get; set; }

	public SynchConnectStateEnum.SynchConnectStateType ConnectAndSynchronizeState { get; set; }

	public string ErrorMessage { get; set; }

	public DatabaseRow()
	{
		Filter = new FilterRulesCollection();
		InstanceIdentifier = InstanceIdentifierEnum.InstanceIdentifier.ServiceName;
	}

	public DatabaseRow(DocumentationObject row, CustomFieldsSupport customFieldsSupport = null)
		: this()
	{
		if (row != null)
		{
			base.Id = row.DatabaseId;
			Type = DatabaseTypeEnum.StringToType(row.Type);
			HasMultipleSchemas = row.MultipleSchemas;
			ShowSchema = row.ShowSchema;
			ShowSchemaOverride = row.ShowSchemaOverride;
			PasswordEncrypted = row.Password;
			Host = row.Host;
			if (Type == SharedDatabaseTypeEnum.DatabaseType.MongoDB || Type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB)
			{
				MongoDBIsSrv = SrvEnum.StringToType(row.InstanceIdentifier) == SrvEnum.Srv.SRV;
				UserProvidedConnectionString = Password;
				ImportAllSchemas = row.DifferentSchema == true;
				UseDifferentSchema = row.MultipleSchemas;
				Host = row.MultiHost;
			}
			else
			{
				Host = row.Host;
				UseDifferentSchema = row.DifferentSchema;
			}
			Name = row.Name;
			base.Title = row.Title;
			ConnectionType = ConnectionTypeEnum.StringToType(row.ConnectionType);
			GeneralConnectionType = GeneralConnectionTypeEnum.StringToType(row.ConnectionType);
			User = row.User;
			ConnectAndSynchronizeState = SynchConnectStateEnum.SynchConnectStateType.Unknown;
			Port = row.Port;
			ServiceName = row.ServiceName;
			OracleSid = row.OracleSid;
			InstanceIdentifier = InstanceIdentifierEnum.StringToType(row.InstanceIdentifier);
			base.Description = row.Description;
			SelectedAuthenticationType = AuthenticationType.StringToType(row.ConnectionType);
			DbmsVersion = row.DbmsVersion;
			Type = DatabaseTypeEnum.StringToType(row.Type);
			IsValidDatabase = row.Type != "BUSINESS_GLOSSARY";
			tableRows = null;
			UserControlName = "Database: " + Name;
			MongoDBAuthenticationDatabase = row.AuthenticationDatabase;
			MongoDBReplicaSet = row.ReplicaSet;
			MultiHost = row.MultiHost;
			SSLType = row.SSLType;
			SSLSettings = new SSLSettings();
			SSLSettings.KeyPath = row.SSLKeyPath;
			SSLSettings.CertPath = row.SSLCertPath;
			SSLSettings.CAPath = row.SSLCaPath;
			ConnectionRole = row.ConnectionRole;
			Perspective = row.Perspective;
			Param1 = row.Param1;
			Param2 = row.Param2;
			Param3 = row.Param3;
			Param4 = row.Param4;
			Param5 = row.Param5;
			Param6 = row.Param6;
			Param7 = row.Param7;
			Param8 = row.Param8;
			Param9 = row.Param9;
			Param10 = row.Param10;
			Filter.SetRulesFromXml(PrepareValue.ToString(row.Filter));
			if (SharedDatabaseClassEnum.StringToType(row.DatabaseClass) == SharedDatabaseClassEnum.DatabaseClass.Glossary)
			{
				base.ObjectTypeValue = SharedObjectTypeEnum.ObjectType.BusinessGlossary;
			}
		}
		base.CustomFields = new CustomFieldContainer(SharedObjectTypeEnum.ObjectType.Database, base.Id, customFieldsSupport);
		base.CustomFields.RetrieveCustomFields(row);
	}

	public DatabaseRow(PersonalSettingsObject row, bool getSSLSettings)
		: this()
	{
		if (row == null)
		{
			return;
		}
		base.Id = row.DatabaseId;
		UseDifferentSchema = row.DifferentSchema;
		HasMultipleSchemas = row.MultipleSchemas;
		Name = row.Name;
		base.Title = row.Title;
		Type = DatabaseTypeEnum.StringToType(PrepareValue.ToString(row.Type));
		ConnectionType = ConnectionTypeEnum.StringToType(row.ConnectionType);
		Host = row.Host;
		MultiHost = row.Host;
		User = row.User;
		PasswordEncrypted = row.Password;
		if (Type == SharedDatabaseTypeEnum.DatabaseType.MongoDB || Type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB)
		{
			GeneralConnectionType = GeneralConnectionTypeEnum.StringToType(row.ConnectionType);
			MongoDBIsSrv = SrvEnum.StringToType(row.InstanceIdentifier) == SrvEnum.Srv.SRV;
			UserProvidedConnectionString = Password;
			ImportAllSchemas = row.DifferentSchema == true;
			UseDifferentSchema = row.MultipleSchemas;
			if (ImportAllSchemas)
			{
				Name = null;
			}
		}
		if (!ImportAllSchemas)
		{
			Name = row.Name;
		}
		ConnectAndSynchronizeState = SynchConnectStateEnum.SynchConnectStateType.Unknown;
		Port = row.Port;
		ServiceName = row.ServiceName;
		OracleSid = row.OracleSid;
		InstanceIdentifier = InstanceIdentifierEnum.StringToType(row.InstanceIdentifier);
		base.Description = row.Description;
		DbmsVersion = row.DbmsVersion;
		IsValidDatabase = row.Type != "BUSINESS_GLOSSARY";
		tableRows = null;
		UserControlName = "Database: " + Name;
		SelectedAuthenticationType = AuthenticationType.StringToType(row.ConnectionType);
		ConnectionRole = row.ConnectionRole;
		Perspective = row.Perspective;
		Param1 = row.Param1;
		Param2 = row.Param2;
		Param3 = row.Param3;
		Param4 = row.Param4;
		Param5 = row.Param5;
		Param6 = row.Param6;
		Param7 = row.Param7;
		Param8 = row.Param8;
		Param9 = row.Param9;
		Param10 = row.Param10;
		Filter.SetRulesFromXml(row.Filter);
		if (getSSLSettings || StaticData.IsProjectFile)
		{
			SSLSettings = new SSLSettings
			{
				CAPath = row.SslCaPath,
				CertPath = row.SslCertPath,
				KeyPath = row.SslKeyPath,
				Cipher = row.SslCipher
			};
		}
		SSLType = row.SSLType;
	}

	public SharedDatabaseTypeEnum.DatabaseType? GetVersion(Form owner = null)
	{
		Type = GeneralQueries.GetDatabaseVersion(Type, Connection, owner);
		return Type;
	}

	public string GetDbmsVersion(Form owner = null)
	{
		try
		{
			return DatabaseSupportFactory.GetDatabaseSupport(Type).GetDbmsVersion(Connection);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while retrieving database's version", owner);
			return null;
		}
	}

	public DatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string name, string title, string host, string user, string password, bool windows_authentication, int? id, string filter, string dbmsVersion, string port, AuthenticationType.AuthenticationTypeEnum? sqlAuthentication)
		: this()
	{
		Type = databaseType;
		IsValidDatabase = true;
		UseDifferentSchema = null;
		HasMultipleSchemas = null;
		Name = name;
		WindowsAutentication = windows_authentication;
		base.Title = title;
		Host = host;
		User = user;
		Password = password;
		base.Id = id;
		Filter.SetRulesFromXml(filter);
		DbmsVersion = dbmsVersion;
		Port = PrepareValue.ToInt(port);
		SelectedAuthenticationType = sqlAuthentication ?? AuthenticationType.AuthenticationTypeEnum.StandardAuthentication;
	}

	public DatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string name, string title, string host, string user, string password, object port, int? id, string filter, string dbmsVersion, string serviceName = null, string connectionRole = null, SSLSettings sslSettings = null, List<string> schemas = null, string sslType = null)
		: this()
	{
		Type = databaseType;
		IsValidDatabase = true;
		UseDifferentSchema = null;
		HasMultipleSchemas = null;
		Name = name;
		ServiceName = serviceName;
		base.Title = title;
		Host = host;
		User = user;
		Password = password;
		Port = PrepareValue.ToInt(port);
		base.Id = id;
		Filter.SetRulesFromXml(filter);
		DbmsVersion = dbmsVersion;
		SSLSettings = sslSettings ?? new SSLSettings();
		SSLType = sslType;
		Schemas = schemas;
		ConnectionRole = connectionRole;
	}

	public DatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string name, string title, string userConnectionString, int? id, string filter, string dbmsVersion, bool getAllDatabases, bool hasMultipleSchemas, string serviceName = null, SSLSettings sslSettings = null, List<string> schemas = null)
		: this()
	{
		GeneralConnectionType = GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString;
		Type = databaseType;
		IsValidDatabase = true;
		ServiceName = serviceName;
		new MongoDBSupport();
		Host = MongoDBSupport.GetHostFromConnectionString(userConnectionString);
		User = MongoDBSupport.GetLoginFromConnectionString(userConnectionString);
		Password = MongoDBSupport.GetPasswordFromConnectionString(userConnectionString);
		Port = MongoDBSupport.GetPortFromConnectionString(userConnectionString);
		MongoDBIsSrv = MongoDBSupport.GetSrvFromConnectionString(userConnectionString);
		MultiHost = MongoDBSupport.GetMultiHostFromConnectionString(userConnectionString);
		UseDifferentSchema = getAllDatabases;
		HasMultipleSchemas = hasMultipleSchemas;
		Name = name;
		base.Title = title;
		UserProvidedConnectionString = userConnectionString;
		base.Id = id;
		Filter.SetRulesFromXml(filter);
		DbmsVersion = dbmsVersion;
		SSLSettings = sslSettings;
		Schemas = schemas;
		HasMultipleSchemas = Schemas.Count > 1;
	}

	public DatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string name, string title, string host, string user, string password, object port, int? id, string filter, string dbmsVersion, bool isSrv, bool getAllDatabases, bool hasMultipleSchemas, List<string> schemas = null, string authDb = null, string replicaSet = null, string sslType = null, string sslKeyPath = null)
		: this()
	{
		GeneralConnectionType = GeneralConnectionTypeEnum.GeneralConnectionType.Values;
		Type = databaseType;
		IsValidDatabase = true;
		UseDifferentSchema = getAllDatabases;
		HasMultipleSchemas = hasMultipleSchemas;
		Name = name;
		MongoDBIsSrv = isSrv;
		base.Title = title;
		Host = host.Split(',').FirstOrDefault();
		User = user;
		Password = password;
		Port = PrepareValue.ToInt(port);
		base.Id = id;
		Filter.SetRulesFromXml(filter);
		DbmsVersion = dbmsVersion;
		Schemas = schemas.ToList();
		MongoDBAuthenticationDatabase = authDb;
		MongoDBReplicaSet = replicaSet;
		MultiHost = host;
		SSLType = sslType;
		SSLSettings = new SSLSettings();
		SSLSettings.KeyPath = sslKeyPath;
	}

	public DatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string name, string title, string host, string user, string password, int? id, string filter, string dbmsVersion, string serviceName = null, SSLSettings sslSettings = null, bool multipleSchemas = false, List<string> schemas = null)
		: this()
	{
		Type = databaseType;
		IsValidDatabase = true;
		UseDifferentSchema = null;
		HasMultipleSchemas = multipleSchemas;
		Name = name;
		ServiceName = serviceName;
		base.Title = title;
		Host = host;
		User = user;
		Password = password;
		base.Id = id;
		Filter.SetRulesFromXml(filter);
		DbmsVersion = dbmsVersion;
		SSLSettings = sslSettings;
		Schemas = schemas;
	}

	public DatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string name, string title, string host, string user, string password, int? id, string filter, string dbmsVersion, AuthenticationType.AuthenticationTypeEnum? authentication, Dataedo.Shared.Enums.TableauProduct.TableauProductEnum? connectionMode)
		: this()
	{
		Type = databaseType;
		IsValidDatabase = true;
		UseDifferentSchema = null;
		Name = name;
		base.Title = title;
		Host = host;
		if (authentication == AuthenticationType.AuthenticationTypeEnum.Token)
		{
			ServiceName = user;
		}
		else
		{
			User = user;
		}
		Password = password;
		base.Id = id;
		Filter.SetRulesFromXml(filter);
		DbmsVersion = dbmsVersion;
		SelectedAuthenticationType = authentication ?? AuthenticationType.AuthenticationTypeEnum.StandardAuthentication;
		Param1 = Dataedo.Shared.Enums.TableauProduct.TypeToString(connectionMode);
	}

	public DatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string name, string title, ConnectionTypeEnum.ConnectionType connectionType, string host, string user, string password, object port, string serviceName, string sid, InstanceIdentifierEnum.InstanceIdentifier instanceIdentifier, int? id, bool useDifferentSchema, bool hasMultipleSchemas, string filter, string dbmsVersion, List<string> schemas = null)
		: this()
	{
		Type = databaseType;
		IsValidDatabase = true;
		UseDifferentSchema = useDifferentSchema;
		HasMultipleSchemas = hasMultipleSchemas;
		Name = name;
		base.Title = title;
		ConnectionType = connectionType;
		Host = host;
		User = user;
		Password = password;
		Port = PrepareValue.ToInt(port);
		ServiceName = serviceName;
		OracleSid = sid;
		InstanceIdentifier = instanceIdentifier;
		base.Id = id;
		Filter.SetRulesFromXml(filter);
		Schemas = schemas;
		DbmsVersion = dbmsVersion;
	}

	public DatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string name, string title, string host, string user, string password, object port, int? id, string filter, string dbmsVersion, string sslType)
		: this()
	{
		Type = databaseType;
		IsValidDatabase = true;
		UseDifferentSchema = null;
		HasMultipleSchemas = null;
		Name = name;
		base.Title = title;
		Host = host;
		User = user;
		Password = password;
		Port = PrepareValue.ToInt(port);
		base.Id = id;
		Filter.SetRulesFromXml(filter);
		DbmsVersion = dbmsVersion;
		SSLType = sslType;
	}

	public DatabaseRow(SharedDatabaseTypeEnum.DatabaseType? databaseType, string name, string title, string host, string user, string password, bool windows_authentication, int? id, string filter, string dbmsVersion, string port, AuthenticationType.AuthenticationTypeEnum? sqlAuthentication, string perspective)
		: this()
	{
		Type = databaseType;
		IsValidDatabase = true;
		UseDifferentSchema = null;
		HasMultipleSchemas = null;
		Name = name;
		WindowsAutentication = windows_authentication;
		base.Title = title;
		Host = host;
		User = user;
		Password = password;
		base.Id = id;
		Filter.SetRulesFromXml(filter);
		DbmsVersion = dbmsVersion;
		Port = PrepareValue.ToInt(port);
		SelectedAuthenticationType = sqlAuthentication ?? AuthenticationType.AuthenticationTypeEnum.StandardAuthentication;
		Perspective = perspective;
	}

	public string GetFilter(bool saveInsertFilter)
	{
		string rulesXml = Filter.GetRulesXml();
		if (!saveInsertFilter)
		{
			return Filter.GetDefaultFilter();
		}
		if (!string.IsNullOrEmpty(rulesXml))
		{
			return rulesXml;
		}
		return Filter.GetDefaultFilter();
	}

	public string GetSchemasListForCondition(Func<string, string> escapingFunction)
	{
		string schemasListForCondition = SchemasListForCondition;
		if (escapingFunction == null || string.IsNullOrEmpty(schemasListForCondition))
		{
			return schemasListForCondition;
		}
		return ((Schemas != null) ? ("'" + string.Join("', '", Schemas.Select((string x) => escapingFunction(x))) + "'") : ("'" + Name.ToUpper() + "'")).ToUpper();
	}

	public static string PrepareSchemasList(IEnumerable<string> schemas)
	{
		if (schemas != null && schemas.Count() > 0)
		{
			if (schemas.Count() == 1)
			{
				return schemas.First();
			}
			if (schemas == null)
			{
				return string.Empty;
			}
			return "\"" + string.Join("\", \"", schemas) + "\"";
		}
		return string.Empty;
	}

	public string GetConnectionString(bool withUnicode = false, bool useStandardConnectionString = false)
	{
		bool hasSslSettings = DatabaseSupportFactory.GetDatabaseSupport(Type).HasSslSettings;
		string database = (SharedDatabaseTypeEnum.IsHiveMetastore(Type) ? Param2 : Name);
		string text = null;
		SharedDatabaseTypeEnum.DatabaseType? type = Type;
		text = ((!type.HasValue || type.GetValueOrDefault() != SharedDatabaseTypeEnum.DatabaseType.SapAse) ? null : Param1);
		string applicationName = Assembly.GetExecutingAssembly().GetName().Name;
		DatabaseType? databaseType = DatabaseTypeEnum.ToTypeForDataCommands(Type, isFile: false);
		string host = Host;
		string user = User;
		string password = Password;
		AuthenticationType.AuthenticationTypeEnum selectedAuthenticationType = SelectedAuthenticationType;
		int? port = Port;
		string identifierName = ((InstanceIdentifier == InstanceIdentifierEnum.InstanceIdentifier.ServiceName) ? ServiceName : OracleSid);
		bool isDirectConnect = IsDirectConnect;
		string keyPath = (hasSslSettings ? SSLSettings.KeyPath : null);
		string certPath = (hasSslSettings ? SSLSettings.CertPath : null);
		string caPath = (hasSslSettings ? SSLSettings.CAPath : null);
		string cipher = (hasSslSettings ? SSLSettings.Cipher : null);
		bool isServiceName = InstanceIdentifier == InstanceIdentifierEnum.InstanceIdentifier.ServiceName;
		string userConnectionString = ((GeneralConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString) ? UserProvidedConnectionString : null);
		bool mongoDBIsSrv = MongoDBIsSrv;
		string mongoDBAuthenticationDatabase = MongoDBAuthenticationDatabase;
		string mongoDBReplicaSet = MongoDBReplicaSet;
		string multiHost = MultiHost;
		string sSLType = SSLType;
		string sSLKeyPath = SSLSettings?.KeyPath;
		string charset = text;
		return Connections.GetConnectionString(applicationName, database, databaseType, host, user, password, selectedAuthenticationType, port, identifierName, withPooling: false, withDatabase: true, withUnicode, isDirectConnect, encrypt: false, trustServerCertificate: false, keyPath, certPath, caPath, cipher, useStandardConnectionString, isServiceName, userConnectionString, mongoDBIsSrv, mongoDBAuthenticationDatabase, mongoDBReplicaSet, multiHost, sSLType, sSLKeyPath, ConnectionRole, charset, Param1, Param2, Param3, Param4, Param5, Param6, Param7, Param8, Param9, Param10);
	}

	public string GetConnectionString()
	{
		if (DatabaseTypeEnum.ToTypeForDataCommands(Type, isFile: false) != DatabaseType.Oracle)
		{
			return GetConnectionString(withUnicode: false, useStandardConnectionString: false);
		}
		return GetConnectionString(withUnicode: true);
	}

	public string GetConnectionStringForGettingDatabasesList()
	{
		if (Type == SharedDatabaseTypeEnum.DatabaseType.Cassandra || Type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra)
		{
			return Connections.GetConnectionString(Assembly.GetExecutingAssembly().GetName().Name, null, DatabaseTypeEnum.ToTypeForDataCommands(Type, isFile: false), Host, User, Password, SelectedAuthenticationType, Port, (InstanceIdentifier == InstanceIdentifierEnum.InstanceIdentifier.ServiceName) ? ServiceName : OracleSid, withPooling: false, withDatabase: false, isUnicode: false, IsDirectConnect, encrypt: false, trustServerCertificate: false, null, null, null, null, useStandardConnectionString: false, InstanceIdentifier == InstanceIdentifierEnum.InstanceIdentifier.ServiceName, (GeneralConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString) ? UserProvidedConnectionString : null, MongoDBIsSrv, MongoDBAuthenticationDatabase, MongoDBReplicaSet, MultiHost, SSLType);
		}
		string database = (SharedDatabaseTypeEnum.IsHiveMetastore(Type) ? Param2 : Name);
		bool hasSslSettings = DatabaseSupportFactory.GetDatabaseSupport(Type).HasSslSettings;
		string text = null;
		SharedDatabaseTypeEnum.DatabaseType? type = Type;
		return Connections.GetConnectionString(charset: (!type.HasValue || type.GetValueOrDefault() != SharedDatabaseTypeEnum.DatabaseType.SapAse) ? null : Param1, applicationName: Assembly.GetExecutingAssembly().GetName().Name, database: database, databaseType: DatabaseTypeEnum.ToTypeForDataCommands(Type, isFile: false), host: Host, login: User, password: Password, authenticationType: SelectedAuthenticationType, port: Port, identifierName: (InstanceIdentifier == InstanceIdentifierEnum.InstanceIdentifier.ServiceName) ? ServiceName : OracleSid, withPooling: false, withDatabase: false, isUnicode: false, isDirectConnect: IsDirectConnect, encrypt: false, trustServerCertificate: false, keyPath: hasSslSettings ? SSLSettings.KeyPath : null, certPath: hasSslSettings ? SSLSettings.CertPath : null, caPath: hasSslSettings ? SSLSettings.CAPath : null, cipher: null, useStandardConnectionString: false, isServiceName: InstanceIdentifier == InstanceIdentifierEnum.InstanceIdentifier.ServiceName, userConnectionString: (GeneralConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString) ? UserProvidedConnectionString : null, isSrv: MongoDBIsSrv, authenticationDatabase: MongoDBAuthenticationDatabase, replicaSet: MongoDBReplicaSet, multiHost: MultiHost, SSLType: SSLType, SSLKeyPath: SSLSettings?.KeyPath, connectionRole: ConnectionRole, param1: Param1, param2: Param2);
	}

	public string GetOracleExceptionMessage(Exception ex)
	{
		OracleException ex2 = (ex as OracleException) ?? (ex?.InnerException as OracleException);
		if (ex2 != null)
		{
			return ex2.Code switch
			{
				12545 => "The target host does not exist!", 
				1017 => "Invalid username/password!", 
				1005 => "Password cannot be empty.", 
				28009 => "Dataedo does not support connection as SYS user." + Environment.NewLine + "Please connect as a regular user.", 
				12170 => "Connection timeout. Check your network settings or connection data.", 
				_ => "Unable to connect with server.", 
			};
		}
		if (ex.Message.Contains("CanNotObtainOracleClientFromRegistry") || ex.InnerException is BadImageFormatException)
		{
			string text;
			string text2;
			if (Environment.Is64BitProcess)
			{
				text = "64";
				text2 = "32";
			}
			else
			{
				text = "32";
				text2 = "64";
			}
			return "You are running Dataedo (" + text + " bit)." + Environment.NewLine + "Installed Oracle client is invalid for Dataedo (" + text + " bit)." + Environment.NewLine + Environment.NewLine + "Use direct connection (Connection type/Direct) or if you have " + text2 + " bit Oracle client run Dataedo (" + text2 + " bit)." + Environment.NewLine + Environment.NewLine + "Visit <href=" + Links.OracleConnectionRequirementsUrl + ">dataedo.com</href> for more information.";
		}
		if (ex is OracleException && ex?.InnerException is NullReferenceException)
		{
			return "Unable to connect with server. Check your network settings or connection data.";
		}
		return "Unable to connect to server." + Environment.NewLine + Environment.NewLine + ex.Message;
	}

	public string GetSqlServerExceptionMessage(SqlException ex, bool isLoadingDatabases = false)
	{
		int? num = ex.Errors[0].Number;
		string message = ex.Errors[0].Message;
		switch (num)
		{
		case 53:
			return "Unable to connect to database.\nCheck server name and try again.";
		case 4060:
			return "Unable to connect to database.\nCheck database name and try again.";
		case 18452:
			return "Unable to connect to database.\nCannot connect using windows credentials.";
		case 229:
		case 18456:
			return "Unable to connect to database.\nCheck login and password and try again.";
		default:
			return message;
		}
	}

	public ConnectionResult TryConnection(bool useOnlyRequiredFields = false)
	{
		if (SharedDatabaseTypeEnum.IsHiveMetastore(Type))
		{
			IHiveMetastoreSupport hiveMetastoreSupport = DatabaseSupportFactory.GetDatabaseSupport(Type) as IHiveMetastoreSupport;
			ConnectionResult connectionResult = hiveMetastoreSupport.TryConnection(() => ConnectionString, Param2, Param3, Param4, User, (InstanceIdentifier == InstanceIdentifierEnum.InstanceIdentifier.ServiceName) ? ServiceName : OracleSid, useOnlyRequiredFields);
			if (useOnlyRequiredFields || !connectionResult.IsSuccess)
			{
				hiveMetastoreSupport.CloseConnection(connectionResult.Connection);
			}
			return connectionResult;
		}
		if (Type == SharedDatabaseTypeEnum.DatabaseType.AmazonS3)
		{
			AmazonS3Support amazonS3Support = DatabaseSupportFactory.GetDatabaseSupport(Type) as AmazonS3Support;
			ConnectionResult connectionResult2 = amazonS3Support.TryConnection(Host, User, Password);
			if (!connectionResult2.IsSuccess)
			{
				amazonS3Support.CloseConnection(connectionResult2.Connection);
			}
			return connectionResult2;
		}
		if (Type == SharedDatabaseTypeEnum.DatabaseType.AzureBlobStorage || Type == SharedDatabaseTypeEnum.DatabaseType.AzureDataLakeStorage)
		{
			AzureStorageSupport azureStorageSupport = DatabaseSupportFactory.GetDatabaseSupport(Type) as AzureStorageSupport;
			ConnectionResult connectionResult3 = azureStorageSupport.TryConnection(this);
			if (!connectionResult3.IsSuccess)
			{
				azureStorageSupport.CloseConnection(connectionResult3.Connection);
			}
			return connectionResult3;
		}
		IDatabaseSupport databaseSupport = DatabaseSupportFactory.GetDatabaseSupport(Type);
		string text = ((Type == SharedDatabaseTypeEnum.DatabaseType.GoogleBigQuery) ? Host : Name);
		ConnectionResult connectionResult4 = databaseSupport.TryConnection(() => ConnectionString, text, User, (InstanceIdentifier == InstanceIdentifierEnum.InstanceIdentifier.ServiceName) ? ServiceName : OracleSid, useOnlyRequiredFields);
		if (useOnlyRequiredFields || !connectionResult4.IsSuccess)
		{
			databaseSupport.CloseConnection(connectionResult4.Connection);
		}
		return connectionResult4;
	}

	public string GetSnowflakeExceptionMessage(OdbcException ex, OdbcError error)
	{
		switch (error.SQLState)
		{
		case "22000":
			return "Incorrect database was specified. Database '" + Name + "' not exists or is not reachable.";
		case "57P03":
			return "Incorrect warehouse was specified. Warehouse '" + ServiceName + "' not exists or is not reachable.";
		case "28000":
			return error.NativeError switch
			{
				390102 => "User temporarily locked. Try again later, or contact your local system administrator.", 
				390101 => "User access disabled. Contact your local system administrator.", 
				_ => "Incorrect username or password was specified.", 
			};
		case "IM002":
		{
			string text = (Environment.Is64BitProcess ? "32" : "64");
			return "Data source name not found and no default driver specified.\nInstall Snowflake ODBC driver, see <href=https://docs.snowflake.net/manuals/user-guide/odbc-download.html>official Snowflake guide</href> for more information.\nIf you have installed the driver, try running Dataedo in " + text + "-bit mode.";
		}
		case "HY000":
			return "Cannot connect to server." + Environment.NewLine + error.Message;
		default:
			return error.Message;
		}
	}

	public List<string> GetDatabases(SplashScreenManager splashScreenManager = null, bool forceStandardConnection = false, Form owner = null)
	{
		if (string.IsNullOrEmpty(Host) && string.IsNullOrEmpty(UserProvidedConnectionString))
		{
			return null;
		}
		try
		{
			if (Type == SharedDatabaseTypeEnum.DatabaseType.GoogleBigQuery)
			{
				return DatabaseSupportFactory.GetDatabaseSupport(Type).GetDatabases(ConnectionStringForGettingDatabasesList, splashScreenManager, Host, ServiceName, owner);
			}
			return DatabaseSupportFactory.GetDatabaseSupport(Type).GetDatabases(ConnectionStringForGettingDatabasesList, splashScreenManager, Name, ServiceName, owner);
		}
		catch (Exception ex)
		{
			if (splashScreenManager != null)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			GeneralMessageBoxesHandling.Show("Unable to connect to server." + Environment.NewLine + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}

	public List<string> GetWarehouses(SplashScreenManager splashScreenManager = null, Form owner = null)
	{
		if (string.IsNullOrEmpty(Host))
		{
			return null;
		}
		List<string> list = new List<string>();
		try
		{
			SharedDatabaseTypeEnum.DatabaseType? type = Type;
			if (type.HasValue && type.GetValueOrDefault() == SharedDatabaseTypeEnum.DatabaseType.Snowflake)
			{
				using SnowflakeDbConnection snowflakeDbConnection = new SnowflakeDbConnection();
				SnowflakeDbConnectionStringBuilder snowflakeDbConnectionStringBuilder = new SnowflakeDbConnectionStringBuilder();
				snowflakeDbConnectionStringBuilder.ConnectionString = ConnectionString;
				if (snowflakeDbConnectionStringBuilder.ContainsKey("warehouse"))
				{
					snowflakeDbConnectionStringBuilder.Remove("warehouse");
				}
				if (snowflakeDbConnectionStringBuilder.ContainsKey("db"))
				{
					snowflakeDbConnectionStringBuilder.Remove("db");
				}
				snowflakeDbConnection.ConnectionString = snowflakeDbConnectionStringBuilder.ConnectionString;
				snowflakeDbConnection.Open();
				using IDbCommand dbCommand = CommandsWithTimeout.Snowflake("SHOW WAREHOUSES;", snowflakeDbConnection);
				using IDataReader dataReader = dbCommand?.ExecuteReader();
				while (dataReader != null && dataReader.Read())
				{
					int ordinal = dataReader.GetOrdinal("name");
					if (dataReader[ordinal] != null && !(dataReader[ordinal] is DBNull))
					{
						list.Add(dataReader[ordinal]?.ToString());
					}
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show(GetOracleExceptionMessage(ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, owner);
		}
		return null;
	}

	public List<string> GetRoles(SplashScreenManager splashScreenManager = null)
	{
		if (string.IsNullOrEmpty(Host))
		{
			return null;
		}
		List<string> list = new List<string>();
		try
		{
			SharedDatabaseTypeEnum.DatabaseType? type = Type;
			if (type.HasValue && type.GetValueOrDefault() == SharedDatabaseTypeEnum.DatabaseType.Snowflake)
			{
				using SnowflakeDbConnection snowflakeDbConnection = new SnowflakeDbConnection();
				SnowflakeDbConnectionStringBuilder snowflakeDbConnectionStringBuilder = new SnowflakeDbConnectionStringBuilder();
				snowflakeDbConnectionStringBuilder.ConnectionString = ConnectionString;
				if (snowflakeDbConnectionStringBuilder.ContainsKey("role"))
				{
					snowflakeDbConnectionStringBuilder.Remove("role");
				}
				if (snowflakeDbConnectionStringBuilder.ContainsKey("db"))
				{
					snowflakeDbConnectionStringBuilder.Remove("db");
				}
				snowflakeDbConnection.ConnectionString = snowflakeDbConnectionStringBuilder.ConnectionString;
				snowflakeDbConnection.Open();
				using IDbCommand dbCommand = CommandsWithTimeout.Snowflake("SHOW ROLES;", snowflakeDbConnection);
				using IDataReader dataReader = dbCommand?.ExecuteReader();
				while (dataReader != null && dataReader.Read())
				{
					int ordinal = dataReader.GetOrdinal("name");
					if (dataReader[ordinal] != null && !(dataReader[ordinal] is DBNull))
					{
						list.Add(dataReader[ordinal]?.ToString());
					}
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			GeneralMessageBoxesHandling.Show(GetOracleExceptionMessage(ex), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		return null;
	}

	public bool IsCurrentDocumentation(string name)
	{
		if (name != null)
		{
			string nameUpper = name.ToUpper();
			if (UseDifferentSchema != true || HasMultipleSchemas != true)
			{
				return Name.ToUpper() == nameUpper;
			}
			return Schemas.Any((string x) => x.ToUpper() == nameUpper);
		}
		return false;
	}

	public DatabaseVersionUpdate GetVersionUpdate()
	{
		return DatabaseSupportFactory.GetDatabaseSupport(Type).GetVersion(Connection);
	}

	public static IEnumerable<string> GetSchemasNames(string namesList)
	{
		if (namesList == null)
		{
			return new List<string>();
		}
		string[] array = namesList.Split(new string[1] { "\", \"" }, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length > 1)
		{
			array[0] = array[0].TrimStart('"');
			array[array.Length - 1] = array[array.Length - 1].TrimEnd('"');
		}
		return array;
	}

	public static bool GetShowSchema(bool? databaseShowSchema, bool? databaseShowSchemaOverride)
	{
		if (databaseShowSchemaOverride != true)
		{
			if (!databaseShowSchemaOverride.HasValue)
			{
				return databaseShowSchema == true;
			}
			return false;
		}
		return true;
	}

	public List<string> GetExtendedProperties()
	{
		return DatabaseSupportFactory.GetDatabaseSupport(Type).GetExtendedProperties(Connection, Host, Schemas);
	}

	public static bool GetShowSchema(bool? databaseShowSchema, bool? databaseShowSchemaOverride, bool contextShowSchema)
	{
		return databaseShowSchemaOverride == true || (!databaseShowSchemaOverride.HasValue && databaseShowSchema == true) || contextShowSchema;
	}

	public void CloseConnection()
	{
		if (Type.HasValue)
		{
			DatabaseSupportFactory.GetDatabaseSupport(Type).CloseConnection(Connection);
		}
	}
}
