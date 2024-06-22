using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Enums;
using Dataedo.App.Licences;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Data.Base.Tools;
using Dataedo.DataProcessing.Classes;
using Dataedo.LicenseHelperLibrary.Repository;
using Dataedo.Model.Data.Documentations;
using Dataedo.Model.Data.ExtendedProperties;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;

namespace Dataedo.App.Data.MetadataServer;

internal class DatabaseDB : CommonDBSupport
{
	public const string DefaultNewDatabaseName = "New database";

	public DatabaseDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public List<DocumentationForMenuObject> GetDataForMenu()
	{
		if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary))
		{
			return commands.Select.Documentations.GetDocumentationsForMenuForEnterprise();
		}
		return commands.Select.Documentations.GetDocumentationsForMenu(null);
	}

	public List<DocumentationForMenuObject> GetDataWithoutBusinessGlossaryForMenu()
	{
		return commands.Select.Documentations.GetDocumentationsWithoutBusinessGlossaryForMenu(null);
	}

	public List<DocumentationObject> GetData()
	{
		return commands.Select.Documentations.GetDocumentations(null);
	}

	public List<DocumentationWithObjectsCountObject> GetDocumentationsWithObjectsCount()
	{
		List<DocumentationWithObjectsCountObject> source = commands.Select.Documentations.GetDocumentationsWithObjectsCount();
		if (!Dataedo.App.StaticData.License.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary))
		{
			source = source.Where((DocumentationWithObjectsCountObject x) => x.Class != "GLOSSARY").ToList();
		}
		return (from x in source
			orderby (!(x.Class == "GLOSSARY")) ? 1 : 0, x.Title
			select x).ToList();
	}

	public void CopyDocumentation(int sourceId, int destinationId, string sourceRepository, string destintationRepository, int synchDescriptions, int synchUserObjects)
	{
		int? clientId = (Dataedo.App.StaticData.IsProjectFile ? null : new int?(LicenseHelper.GetLicenseIdForLogin(Dataedo.App.StaticData.DataedoConnectionString, LoginStrategy.GetUserName())));
		int savingIsEnabled = (DB.History.SavingEnabled ? 1 : 0);
		commands.Manipulation.Documentations.CopyDocumentation(sourceId, destinationId, sourceRepository, destintationRepository, synchDescriptions, synchUserObjects, clientId, Dataedo.App.StaticData.ProductVersion, savingIsEnabled);
	}

	public DocumentationObject GetDataById(int databaseId)
	{
		List<DocumentationObject> documentations = commands.Select.Documentations.GetDocumentations(databaseId);
		if (documentations.Count <= 0)
		{
			return null;
		}
		return documentations[0];
	}

	public bool CheckIfIsUserImportedDatabase(int? databaseId)
	{
		return commands.Select.Documentations.CheckIfIsUserImportedDatabase(databaseId);
	}

	public DocumentationObject GetDataByIdWithCounter(int databaseId, bool notDeletedOnly = false)
	{
		return commands.Select.Documentations.GetDocumentations(databaseId).FirstOrDefault();
	}

	public DocumentationForMenuObject GetDataByIdForMenu(int databaseId)
	{
		return commands.Select.Documentations.GetDocumentationsForMenu(databaseId)[0];
	}

	public string GetDocumentationTitle(int databaseId)
	{
		return commands.Select.Documentations.GetDocumentationTitle(databaseId);
	}

	public string GetTitle(int databaseId)
	{
		return PrepareValue.ToString(GetDocumentationTitle(databaseId));
	}

	public List<DbDescriptionObject> UpdateExtendedPropertiesCommand(int databaseId, List<int> modulesIds, string types)
	{
		return commands.Manipulation.Documentations.UpdateExtendedPropertiesCommand(databaseId, modulesIds, types);
	}

	public List<DbDescriptionObject> UpdateCustomFieldsExtendedPropertiesCommand(int databaseId, List<int> modulesIds, string types, string customFieldName, int customFieldId)
	{
		return commands.Manipulation.Documentations.UpdateCustomFieldsExtendedPropertiesCommand(databaseId, modulesIds, types, customFieldName, customFieldId);
	}

	public void UpdateDocumentationShowSchemaOverride(int databaseId, bool? value)
	{
		commands.Manipulation.Documentations.UpdateDocumentationShowSchemaOverride(databaseId, value);
	}

	public List<DbDescriptionObject> UpdateCommentsCommand(int databaseId, List<int> modulesIds, string typeNames, string viewType)
	{
		return commands.Manipulation.Documentations.UpdateCommentsCommand(databaseId, modulesIds, typeNames, viewType);
	}

	public string GetDatabaseTypeById(int databaseId)
	{
		return commands.Select.Documentations.GetDocumentationType(databaseId);
	}

	public string GetDatabaseType(DocumentationObject database)
	{
		if (database == null)
		{
			return null;
		}
		return PrepareValue.ToString(database.Type);
	}

	public bool Insert(DatabaseRow database, bool saveInsertFilter, Form owner = null)
	{
		string connectionType = ConnectionTypeService.GetConnectionType(database);
		try
		{
			if ((database.Type == SharedDatabaseTypeEnum.DatabaseType.MongoDB || database.Type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) && database.GeneralConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString)
			{
				database.Password = database.ConnectionString;
				database.User = null;
				database.MongoDBIsSrv = false;
				database.Port = null;
			}
			if (!Dataedo.App.StaticData.IsProjectFile)
			{
				string filter = database.GetFilter(saveInsertFilter);
				int? num2 = (database.Id = commands.Manipulation.Documentations.InsertDocumentation(database.Name, database.Title, (database.Type == SharedDatabaseTypeEnum.DatabaseType.MongoDB || database.Type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) ? GeneralConnectionTypeEnum.TypeToString(database.GeneralConnectionType) : connectionType, database.Host, database.User, database.PasswordEncrypted, DatabaseTypeEnum.TypeToString(database.Type), database.Port, database.ServiceName, database.OracleSid, (database.Type != SharedDatabaseTypeEnum.DatabaseType.MongoDB && database.Type != SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) ? InstanceIdentifierEnum.TypeToString(database.InstanceIdentifier) : (database.MongoDBIsSrv ? SrvEnum.TypeToString(SrvEnum.Srv.SRV) : null), database.Description, database.DescriptionSearch, database.WindowsAutentication, database.UseDifferentSchema, database.HasMultipleSchemas, filter, database.DbmsVersion, null, database.MongoDBAuthenticationDatabase, database.MongoDBReplicaSet, database.MultiHost, database.SSLType, database.SSLSettings?.KeyPath, database.SSLSettings?.CertPath, database.SSLSettings?.CAPath, database.ConnectionRole, database.Perspective, database.Param1, database.Param2, database.Param3, database.Param4, database.Param5, database.Param6, database.Param7, database.Param8, database.Param9, database.Param10));
				return true;
			}
			string filter2 = database.GetFilter(saveInsertFilter);
			int? num4 = (database.Id = commands.Manipulation.Documentations.InsertDocumentationCE(database.Name, database.Title, (database.Type == SharedDatabaseTypeEnum.DatabaseType.MongoDB || database.Type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) ? GeneralConnectionTypeEnum.TypeToString(database.GeneralConnectionType) : connectionType, database.Host, database.User, database.PasswordEncrypted, DatabaseTypeEnum.TypeToString(database.Type), database.Port, database.ServiceName, database.OracleSid, (database.Type != SharedDatabaseTypeEnum.DatabaseType.MongoDB && database.Type != SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) ? InstanceIdentifierEnum.TypeToString(database.InstanceIdentifier) : (database.MongoDBIsSrv ? SrvEnum.TypeToString(SrvEnum.Srv.SRV) : null), database.Description, database.DescriptionSearch, database.WindowsAutentication, database.UseDifferentSchema, database.HasMultipleSchemas, filter2, database.DbmsVersion, database?.SSLSettings?.KeyPath, database?.SSLSettings?.CertPath, database?.SSLSettings?.CAPath, database?.SSLSettings?.Cipher, null, database.MongoDBAuthenticationDatabase, database.MongoDBReplicaSet, database.MultiHost, database.SSLType, database.ConnectionRole, database.Perspective, database.Param1, database.Param2, database.Param3, database.Param4, database.Param5, database.Param6, database.Param7, database.Param8, database.Param9, database.Param10));
			return true;
		}
		catch (Exception)
		{
		}
		try
		{
			if (!Dataedo.App.StaticData.IsProjectFile)
			{
				int? num6 = (database.Id = commands.Manipulation.Documentations.InsertDocumentation(database.Name, database.Title, connectionType, database.Host, database.User, null, DatabaseTypeEnum.TypeToString(database.Type), database.Port, database.ServiceName, database.OracleSid, (database.Type != SharedDatabaseTypeEnum.DatabaseType.MongoDB && database.Type != SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) ? InstanceIdentifierEnum.TypeToString(database.InstanceIdentifier) : (database.MongoDBIsSrv ? SrvEnum.TypeToString(SrvEnum.Srv.SRV) : null), database.Description, database.DescriptionSearch, database.WindowsAutentication, database.UseDifferentSchema, database.HasMultipleSchemas, database.Filter.GetRulesXml(), database.DbmsVersion, null, database.MongoDBAuthenticationDatabase, database.MongoDBReplicaSet, database.MongoDBReplicaSet, database.SSLType, database.SSLSettings?.KeyPath, database.SSLSettings?.CertPath, database.SSLSettings?.CAPath, database.ConnectionRole, database.Perspective, database.Param1, database.Param2, database.Param3, database.Param4, database.Param5, database.Param6, database.Param7, database.Param8, database.Param9, database.Param10));
				GeneralMessageBoxesHandling.Show("Due to the security reasons some of your connection parameters won't be stored. You'll have to retype them at each documentation update.", "Skipped storing connection parameters", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, owner);
				return true;
			}
			int? num8 = (database.Id = commands.Manipulation.Documentations.InsertDocumentationCE(database.Name, database.Title, connectionType, database.Host, database.User, null, DatabaseTypeEnum.TypeToString(database.Type), database.Port, database.ServiceName, database.OracleSid, (database.Type != SharedDatabaseTypeEnum.DatabaseType.MongoDB && database.Type != SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) ? InstanceIdentifierEnum.TypeToString(database.InstanceIdentifier) : (database.MongoDBIsSrv ? SrvEnum.TypeToString(SrvEnum.Srv.SRV) : null), database.Description, database.DescriptionSearch, database.WindowsAutentication, database.UseDifferentSchema, database.HasMultipleSchemas, database.Filter.GetRulesXml(), database.DbmsVersion, database?.SSLSettings?.KeyPath, database?.SSLSettings?.CertPath, database?.SSLSettings?.CAPath, database?.SSLSettings?.Cipher, null, database.MongoDBAuthenticationDatabase, database.MongoDBReplicaSet, database.MultiHost, database.SSLType, database.ConnectionRole, database.Perspective, database.Param1, database.Param2, database.Param3, database.Param4, database.Param5, database.Param6, database.Param7, database.Param8, database.Param9, database.Param10));
			GeneralMessageBoxesHandling.Show("Due to the security reasons some of your connection parameters won't be stored. You'll have to retype them at each documentation update.", "Skipped storing connection parameters", MessageBoxButtons.OK, MessageBoxIcon.Asterisk, null, 1, owner);
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while adding database:", owner);
			return false;
		}
	}

	public bool UpdateManualDocumentationBaseInfo(DatabaseRow database, Form owner = null)
	{
		try
		{
			commands.Manipulation.Documentations.UpdateManualDocumentationBaseInfo(database.Id, database.Name, database.Title, database.Host, DatabaseTypeEnum.TypeToString(database.Type), database.Description, database.DescriptionSearch, database.DbmsVersion, database.ServiceName, database.OracleSid, (database.Type != SharedDatabaseTypeEnum.DatabaseType.MongoDB && database.Type != SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) ? InstanceIdentifierEnum.TypeToString(database.InstanceIdentifier) : (database.MongoDBIsSrv ? SrvEnum.TypeToString(SrvEnum.Srv.SRV) : null), null, database.Perspective, database.Param1, database.Param2, database.Param3, database.Param4, database.Param5, database.Param6, database.Param7, database.Param8, database.Param9, database.Param10);
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating manual database", owner);
			return false;
		}
	}

	public bool InsertBaseInformation(DatabaseRow database, Form owner = null)
	{
		try
		{
			int? num2 = (database.Id = commands.Manipulation.Documentations.InsertDocumentationBaseInformation(database.Name, database.Title, database.Host, DatabaseTypeEnum.TypeToString(database.Type), database.Description, database.DescriptionSearch, database.DbmsVersion, database.ServiceName, database.OracleSid, (database.Type != SharedDatabaseTypeEnum.DatabaseType.MongoDB && database.Type != SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) ? InstanceIdentifierEnum.TypeToString(database.InstanceIdentifier) : ((database.MongoDBIsSrv && database.GeneralConnectionType != GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString) ? SrvEnum.TypeToString(SrvEnum.Srv.SRV) : null), database.UseDifferentSchema, database.HasMultipleSchemas, null, database.Perspective, database.Param1, database.Param2, database.Param3, database.Param4, database.Param5, database.Param6, database.Param7, database.Param8, database.Param9, database.Param10));
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while adding database:", owner);
			return false;
		}
	}

	public int? InsertManualDatabase(string title, Form owner = null)
	{
		try
		{
			return commands.Manipulation.Documentations.InsertManualDatabase(title);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting manual database:", owner);
			return null;
		}
	}

	public void UpdateManualDatabaseName(int? databaseId, Form owner = null)
	{
		try
		{
			commands.Manipulation.Documentations.UpdateManualDatabaseName(databaseId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating manual database name", owner);
		}
	}

	public bool Update(DatabaseRow database, Form owner = null)
	{
		if ((database.Type == SharedDatabaseTypeEnum.DatabaseType.MongoDB || database.Type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) && database.GeneralConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString)
		{
			database.Password = database.ConnectionString;
		}
		try
		{
			string connectionType = ConnectionTypeService.GetConnectionType(database);
			commands.Manipulation.Documentations.UpdateDocumentation(database.Id, database.Name, database.Title, (database.Type == SharedDatabaseTypeEnum.DatabaseType.MongoDB || database.Type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) ? GeneralConnectionTypeEnum.TypeToString(database.GeneralConnectionType) : connectionType, database.Host, database.User, database.PasswordEncrypted, DatabaseTypeEnum.TypeToString(database.Type), database.Port, database.ServiceName, database.OracleSid, (database.Type != SharedDatabaseTypeEnum.DatabaseType.MongoDB && database.Type != SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) ? InstanceIdentifierEnum.TypeToString(database.InstanceIdentifier) : (database.MongoDBIsSrv ? SrvEnum.TypeToString(SrvEnum.Srv.SRV) : null), database.Description, database.DescriptionSearch, database.WindowsAutentication, database.UseDifferentSchema, database.HasMultipleSchemas, database.Filter.GetRulesXml(), database.DbmsVersion, null, database.MongoDBAuthenticationDatabase, database.MongoDBReplicaSet, database.MultiHost, database.SSLType, database.SSLSettings?.KeyPath, database.SSLSettings?.CertPath, database.SSLSettings?.CAPath, database.ConnectionRole, database.Perspective, database.Param1, database.Param2, database.Param3, database.Param4, database.Param5, database.Param6, database.Param7, database.Param8, database.Param9, database.Param10);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating database:", owner);
			return false;
		}
		return true;
	}

	public bool UpdateCE(DatabaseRow database, Form owner = null)
	{
		if ((database.Type == SharedDatabaseTypeEnum.DatabaseType.MongoDB || database.Type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) && database.GeneralConnectionType == GeneralConnectionTypeEnum.GeneralConnectionType.ConnectionString)
		{
			database.Password = database.ConnectionString;
		}
		try
		{
			string connectionType = ConnectionTypeService.GetConnectionType(database);
			commands.Manipulation.Documentations.UpdateDocumentationCE(database.Id, database.Name, database.Title, (database.Type == SharedDatabaseTypeEnum.DatabaseType.MongoDB || database.Type == SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) ? GeneralConnectionTypeEnum.TypeToString(database.GeneralConnectionType) : connectionType, database.Host, database.User, database.PasswordEncrypted, DatabaseTypeEnum.TypeToString(database.Type), database.Port, database.ServiceName, database.OracleSid, (database.Type != SharedDatabaseTypeEnum.DatabaseType.MongoDB && database.Type != SharedDatabaseTypeEnum.DatabaseType.CosmosDbMongoDB) ? InstanceIdentifierEnum.TypeToString(database.InstanceIdentifier) : (database.MongoDBIsSrv ? SrvEnum.TypeToString(SrvEnum.Srv.SRV) : null), database.Description, database.DescriptionSearch, database.WindowsAutentication, database.UseDifferentSchema, database.HasMultipleSchemas, database.Filter.GetRulesXml(), database.DbmsVersion, database.SSLSettings?.KeyPath, database.SSLSettings?.CertPath, database.SSLSettings?.CAPath, database.SSLSettings?.Cipher, null, database.MongoDBAuthenticationDatabase, database.MongoDBReplicaSet, database.MultiHost, database.SSLType, database.ConnectionRole, database.Perspective, database.Param1, database.Param2, database.Param3, database.Param4, database.Param5, database.Param6, database.Param7, database.Param8, database.Param9, database.Param10);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating database:", owner);
			return false;
		}
		return true;
	}

	public bool Update(DatabaseSyncRow row, Form owner = null)
	{
		try
		{
			Database database = ConvertRowToTable(row);
			commands.Manipulation.Documentations.UpdateDocumentationWithCustomFields(database);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating database:", owner);
			return false;
		}
		return true;
	}

	private Database ConvertRowToTable(DatabaseSyncRow row)
	{
		Database database = new Database
		{
			Id = row.Id,
			Title = row.Title,
			Name = row.Name,
			Description = row.Description,
			DescriptionPlain = row.DescriptionPlain
		};
		SetCustomFields(database, row);
		return database;
	}

	public bool UpdateTitle(DBTreeNode dbTreeNode, string newTitle, Form owner = null)
	{
		try
		{
			commands.Manipulation.Documentations.UpdateDocumentationTitle(dbTreeNode.Id, newTitle);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating database:", owner);
			return false;
		}
		return true;
	}

	public bool UpdateType(int? databaseId, string type, Form owner = null)
	{
		try
		{
			commands.Manipulation.Documentations.UpdateDocumentationType(databaseId, type);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating database:", owner);
			return false;
		}
		return true;
	}

	public List<string> GetDatabaseNewTitle(DbTransaction transaction = null)
	{
		return GetDatabaseNewTitle("New database", transaction);
	}

	public List<string> GetDatabaseNewTitle(string title, DbTransaction transaction = null)
	{
		return commands.Select.Documentations.GetTitleExistingNumbers(title);
	}

	public bool GetDatabasesNewDatabaseTitle()
	{
		return GetDatabasesNewDatabaseTitle("New database");
	}

	public bool GetDatabasesNewDatabaseTitle(string title)
	{
		return commands.Select.Documentations.CheckIfTitleExists(title);
	}

	public List<string> GetBusinessGlossaryNewTitle(DbTransaction transaction = null)
	{
		return commands.Select.Documentations.GetTitleExistingNumbers("New Business Glossary");
	}

	public bool GetBusinessGlossaryNewBusinessGlossaryTitle()
	{
		return commands.Select.Documentations.CheckIfTitleExists("New Business Glossary");
	}

	public bool UpdateFilter(int? databaseId, string filter, Form owner = null)
	{
		try
		{
			commands.Manipulation.Documentations.UpdateDocumentationFilter(databaseId, filter);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating database:", owner);
			return false;
		}
		return true;
	}

	public bool Delete(int database_id, Form owner = null)
	{
		try
		{
			if (Dataedo.App.StaticData.IsProjectFile)
			{
				commands.Manipulation.Documentations.DeleteDocumentationCE(database_id);
			}
			else
			{
				commands.Manipulation.Documentations.DeleteDocumentation(database_id);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while removing database:", owner);
			return false;
		}
		return true;
	}

	public bool Delete(int object_id, string object_type, DateTime server_time, int update_id, Form owner = null)
	{
		try
		{
			commands.Synchronization.Objects.SynchronizeObjectDelete(object_id, server_time, object_type, update_id);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while deleting the object: ", owner);
			return false;
		}
		return true;
	}

	public string CheckSynchronization(string name, string schema, string object_type, int database_id, DateTime? dbmsLastModificationDate, Form owner = null)
	{
		string result = null;
		string message = string.Empty;
		try
		{
			commands.Synchronization.Checking.CheckSynchronization(name, schema, object_type, database_id, dbmsLastModificationDate).PutExecutionInfo(ref result, ref message);
			return result;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while checking synchronization:", owner);
			return result;
		}
	}

	public bool SetSynchronized(List<ObjectIdType> objects, DateTime server_time, Form owner = null)
	{
		try
		{
			foreach (ObjectIdType @object in objects)
			{
				@object.ServerTime = server_time;
				@object.SynchronizedBy = LastConnectionInfo.LOGIN_INFO.DataedoRealLogin;
			}
			commands.Manipulation.Objects.SetObjectsSynchronized(objects.ToArray());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while setting synchronization time:", owner);
			return false;
		}
		return true;
	}

	public bool ClearExistsFlag(int databaseId, Form owner = null)
	{
		try
		{
			commands.Manipulation.Objects.ClearExistsFlag(databaseId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while clearing exists flag:", owner);
			return false;
		}
		return true;
	}

	public bool UpdateDocumentationShowSchemaFlag(int databaseId, Form owner = null)
	{
		try
		{
			commands.Manipulation.Documentations.UpdateDocumentationShowSchemaFlag(databaseId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating value indicating whether to show schemas:", owner);
			return false;
		}
		return true;
	}

	public List<string> GetSavedHostsForDatabaseType(string databaseType, Form owner = null)
	{
		try
		{
			return commands.Select.Documentations.GetSavedHostsForDatabaseType(databaseType);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating value indicating whether to show schemas:", owner);
			return null;
		}
	}

	public bool CheckIfDatabaseTitleExists(string databaseTitle)
	{
		return commands.Select.Documentations.CheckIfTitleExists(databaseTitle);
	}

	public string GetDocumentationType(int databaseId)
	{
		return commands.Select.Documentations.GetDocumentationType(databaseId);
	}

	public List<string> GetDatabasesCollation(string sourceDatabaseName, string destinationDatabaseName, DbTransaction transaction = null)
	{
		return commands.Select.Documentations.GetDatabasesCollationByDatabasesIds(sourceDatabaseName, destinationDatabaseName, transaction);
	}
}
