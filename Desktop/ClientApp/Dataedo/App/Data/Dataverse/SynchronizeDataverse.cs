using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.App.UserControls.ImportFilter;
using Dataedo.Model.Data.Dependencies;
using Dataedo.Model.Data.Object;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Model.Data.Tables.Constraints;
using Dataedo.Model.Data.Tables.Relations;
using Dataedo.Shared.Enums;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Query;

namespace Dataedo.App.Data.Dataverse;

internal class SynchronizeDataverse : SynchronizeDatabase
{
	private RetrieveAllEntitiesResponse _allEntitesResponseCache;

	private HashSet<string> _objectNamesToSynchronizeCache;

	private EntityCollection _viewsWithExtendedColumnsCache;

	protected SharedDatabaseTypeEnum.DatabaseType DatabaseType => SharedDatabaseTypeEnum.DatabaseType.Dataverse;

	public string SavedQuery => "savedquery";

	public string Name => "name";

	public string IsUserDefined => "isuserdefined";

	public string FetchXml => "fetchxml";

	public string OfflineSqlQuery => "offlinesqlquery";

	public string Description => "description";

	public string CreatedOn => "createdon";

	public string ModifiedOn => "modifiedon";

	public string[] BaseColumnsFields => new string[4] { Name, IsUserDefined, FetchXml, OfflineSqlQuery };

	public string[] ExtendedColumnsFields => new string[7] { Name, IsUserDefined, FetchXml, OfflineSqlQuery, Description, CreatedOn, ModifiedOn };

	public SynchronizeDataverse(SynchronizeParameters synchronizeParameters)
		: base(synchronizeParameters)
	{
	}

	public override bool CountObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Counting objects to import");
		if (query.ObjectType != FilterObjectTypeEnum.FilterObjectType.Table)
		{
			return true;
		}
		try
		{
			IOrganizationService organizationService = CommandsWithTimeout.Dataverse(synchronizeParameters.DatabaseRow.Connection);
			if (organizationService == null)
			{
				return false;
			}
			int num = 0;
			foreach (Entity entity in GetViewsWithExtendedColumns(organizationService).Entities)
			{
				if (IsVerifiedView(entity))
				{
					num++;
				}
			}
			ReadCount(SharedObjectTypeEnum.ObjectType.View, num);
			RetrieveAllEntitiesResponse fullEntities = GetFullEntities(organizationService);
			ReadCount(SharedObjectTypeEnum.ObjectType.Table, Convert.ToInt32(fullEntities.EntityMetadata.Length));
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	private bool IsVerifiedView(Entity view)
	{
		if (BaseColumnsFields.All((string k) => view.Attributes.ContainsKey(k)) && !string.IsNullOrEmpty((string)view.Attributes[FetchXml]) && (bool)view.Attributes[IsUserDefined] && !string.IsNullOrEmpty((string)view.Attributes[Name]) && !string.IsNullOrEmpty((string)view.Attributes[OfflineSqlQuery]))
		{
			return true;
		}
		return false;
	}

	public override bool GetObjects(ImportQuery query, BackgroundWorkerManager backgroundWorkerManager, Form owner = null)
	{
		backgroundWorkerManager.ReportProgress("Retrieving database's objects");
		try
		{
			IOrganizationService organizationService = CommandsWithTimeout.Dataverse(synchronizeParameters.DatabaseRow.Connection);
			if (organizationService == null)
			{
				return false;
			}
			IEnumerable<FilterRule> rulesByObjectType = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.Table);
			IEnumerable<FilterRule> rulesByObjectType2 = synchronizeParameters.DatabaseRow.Filter.GetRulesByObjectType(FilterObjectTypeEnum.FilterObjectType.View);
			if (query.ObjectType == FilterObjectTypeEnum.FilterObjectType.Table)
			{
				EntityMetadata[] entityMetadata = GetFullEntities(organizationService).EntityMetadata;
				foreach (EntityMetadata entityMetadata2 in entityMetadata)
				{
					if (synchronizeParameters.DbSynchLocker.IsCanceled)
					{
						return false;
					}
					new List<ObjectRow>(synchronizeParameters.DatabaseRow.tableRows);
					if (!IsObjectFiltered(entityMetadata2.LogicalName, rulesByObjectType))
					{
						DataverseObjectWrapper dr = new DataverseObjectWrapper
						{
							Table = entityMetadata2,
							IsView = false,
							LanguageSetting = LanguageType.StringToType(synchronizeParameters.DatabaseRow.Param2)
						};
						ObservableCollection<ObjectRow> tableRows = synchronizeParameters.DatabaseRow.tableRows;
						Form owner2 = owner;
						AddDBObject(dr, tableRows, backgroundWorkerManager, null, null, owner2);
					}
				}
			}
			else if (query.ObjectType == FilterObjectTypeEnum.FilterObjectType.View)
			{
				foreach (Entity entity in GetViewsWithExtendedColumns(organizationService).Entities)
				{
					new List<ObjectRow>(synchronizeParameters.DatabaseRow.tableRows);
					if (IsVerifiedView(entity))
					{
						string entityNameFromXml = GetEntityNameFromXml(entity.Attributes[FetchXml] as string);
						string viewFullName = $"{entityNameFromXml}.{entity.Attributes[Name]}";
						if (!IsObjectFiltered(entityNameFromXml, rulesByObjectType) && !IsObjectFiltered(entity.Attributes[Name] as string, rulesByObjectType2) && !synchronizeParameters.DatabaseRow.tableRows.Any((ObjectRow x) => string.Equals(x.Name, viewFullName, StringComparison.OrdinalIgnoreCase)))
						{
							DataverseObjectWrapper dr2 = new DataverseObjectWrapper
							{
								View = entity,
								IsView = true,
								LanguageSetting = LanguageType.StringToType(synchronizeParameters.DatabaseRow.Param2)
							};
							ObservableCollection<ObjectRow> tableRows2 = synchronizeParameters.DatabaseRow.tableRows;
							Form owner2 = owner;
							AddDBObject(dr2, tableRows2, backgroundWorkerManager, null, null, owner2);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	private List<string> GetViewAttributesFromXml(string xml, Form owner = null)
	{
		List<string> list = new List<string>();
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			foreach (XmlNode item in xmlDocument.SelectNodes("fetch/entity/attribute"))
			{
				list.Add(item.Attributes[Name].Value);
			}
			return list;
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return list;
		}
	}

	private string GetEntityNameFromXml(string xml, Form owner = null)
	{
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(xml);
			return xmlDocument.SelectSingleNode("fetch/entity").Attributes[Name].Value;
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
		}
		return string.Empty;
	}

	public override bool GetColumns(string query, Form owner = null)
	{
		try
		{
			List<AttributeMetadata> list = new List<AttributeMetadata>();
			IOrganizationService organizationService = CommandsWithTimeout.Dataverse(synchronizeParameters.DatabaseRow.Connection);
			if (organizationService == null)
			{
				return false;
			}
			RetrieveAllEntitiesResponse fullEntities = GetFullEntities(organizationService);
			HashSet<string> objectNamesToSynchronize = GetObjectNamesToSynchronize();
			EntityMetadata[] entityMetadata = fullEntities.EntityMetadata;
			foreach (EntityMetadata entityMetadata2 in entityMetadata)
			{
				AttributeMetadata[] attributes = entityMetadata2.Attributes;
				foreach (AttributeMetadata item in attributes)
				{
					list.Add(item);
				}
				if (objectNamesToSynchronize.Contains(entityMetadata2.LogicalName))
				{
					attributes = entityMetadata2.Attributes;
					foreach (AttributeMetadata column in attributes)
					{
						DataverseColumnWrapper dr = new DataverseColumnWrapper
						{
							Column = column,
							Database = synchronizeParameters.DatabaseRow.Name,
							isViewColumn = false,
							LanguageSetting = LanguageType.StringToType(synchronizeParameters.DatabaseRow.Param2)
						};
						AddColumn(dr);
					}
				}
			}
			int num = 1;
			foreach (Entity entity in GetViewsWithExtendedColumns(organizationService).Entities)
			{
				if (!IsVerifiedView(entity))
				{
					continue;
				}
				string entityName = GetEntityNameFromXml(entity.Attributes[FetchXml] as string, owner);
				string viewFullName = $"{entityName}.{entity.Attributes[Name]}";
				if (!objectNamesToSynchronize.Contains(viewFullName))
				{
					continue;
				}
				List<string> viewAttributesFromXml = GetViewAttributesFromXml((string)entity.Attributes[FetchXml], owner);
				num = 1;
				foreach (string attribute in viewAttributesFromXml)
				{
					AttributeMetadata foundColumn = list.FirstOrDefault((AttributeMetadata el) => el.LogicalName == attribute && el.EntityLogicalName == entityName);
					if (foundColumn != null && !base.ColumnRows.Any((ColumnRow x) => string.Equals(x.TableName, viewFullName, StringComparison.OrdinalIgnoreCase) && x.Name == foundColumn.LogicalName))
					{
						DataverseColumnWrapper dr2 = new DataverseColumnWrapper
						{
							Column = foundColumn,
							ViewName = viewFullName,
							ColumnPosition = num,
							Database = synchronizeParameters.DatabaseRow.Name,
							isViewColumn = true,
							LanguageSetting = LanguageType.StringToType(synchronizeParameters.DatabaseRow.Param2)
						};
						AddColumn(dr2);
						num++;
					}
				}
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetTriggers(string query, Form owner = null)
	{
		return true;
	}

	public override bool GetRelations(string query, Form owner = null)
	{
		try
		{
			IOrganizationService organizationService = CommandsWithTimeout.Dataverse(synchronizeParameters.DatabaseRow.Connection);
			if (organizationService == null)
			{
				return false;
			}
			RetrieveAllEntitiesResponse fullEntities = GetFullEntities(organizationService);
			HashSet<string> objectNamesToSynchronize = GetObjectNamesToSynchronize();
			EntityMetadata[] entityMetadata = fullEntities.EntityMetadata;
			foreach (EntityMetadata entityMetadata2 in entityMetadata)
			{
				OneToManyRelationshipMetadata[] oneToManyRelationships = entityMetadata2.OneToManyRelationships;
				foreach (OneToManyRelationshipMetadata oneToManyRelationshipMetadata in oneToManyRelationships)
				{
					if (objectNamesToSynchronize.Contains(oneToManyRelationshipMetadata.ReferencedEntity) || objectNamesToSynchronize.Contains(oneToManyRelationshipMetadata.ReferencingEntity))
					{
						DataverseRelationWrapper dr = new DataverseRelationWrapper
						{
							OneToManyRelation = oneToManyRelationshipMetadata,
							Database = synchronizeParameters.DatabaseRow.Name,
							IsManyToMany = false
						};
						AddRelation(dr, DatabaseType);
					}
				}
				ManyToManyRelationshipMetadata[] manyToManyRelationships = entityMetadata2.ManyToManyRelationships;
				foreach (ManyToManyRelationshipMetadata manyToManyRelationshipMetadata in manyToManyRelationships)
				{
					if (objectNamesToSynchronize.Contains(manyToManyRelationshipMetadata.Entity1LogicalName) || objectNamesToSynchronize.Contains(manyToManyRelationshipMetadata.Entity2LogicalName))
					{
						DataverseRelationWrapper dr2 = new DataverseRelationWrapper
						{
							ManyToManyRelation = manyToManyRelationshipMetadata,
							Database = synchronizeParameters.DatabaseRow.Name,
							IsManyToMany = true
						};
						AddRelation(dr2, DatabaseType);
					}
				}
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetUniqueConstraints(string query, Form owner = null)
	{
		try
		{
			IOrganizationService organizationService = CommandsWithTimeout.Dataverse(synchronizeParameters.DatabaseRow.Connection);
			if (organizationService == null)
			{
				return false;
			}
			RetrieveAllEntitiesResponse fullEntities = GetFullEntities(organizationService);
			HashSet<string> objectNamesToSynchronize = GetObjectNamesToSynchronize();
			EntityMetadata[] entityMetadata = fullEntities.EntityMetadata;
			foreach (EntityMetadata entityMetadata2 in entityMetadata)
			{
				if (!objectNamesToSynchronize.Contains(entityMetadata2.LogicalName))
				{
					continue;
				}
				EntityKeyMetadata[] keys = entityMetadata2.Keys;
				foreach (EntityKeyMetadata entityKeyMetadata in keys)
				{
					string[] keyAttributes = entityKeyMetadata.KeyAttributes;
					foreach (string columnName in keyAttributes)
					{
						int value = (entityMetadata2.Attributes.First((AttributeMetadata el) => el.LogicalName == columnName)?.ColumnNumber).Value;
						UniqueConstraintDataverseWrapper dr = new UniqueConstraintDataverseWrapper
						{
							ColumnName = columnName,
							EntityKey = entityKeyMetadata,
							ColumnPosition = value,
							isAltKey = true,
							Database = synchronizeParameters.DatabaseRow.Name
						};
						AddUniqueConstraint(dr, SharedDatabaseTypeEnum.DatabaseType.Dataverse);
					}
				}
				AttributeMetadata[] attributes = entityMetadata2.Attributes;
				foreach (AttributeMetadata attributeMetadata in attributes)
				{
					if (attributeMetadata.AttributeType == AttributeTypeCode.Uniqueidentifier)
					{
						UniqueConstraintDataverseWrapper dr2 = new UniqueConstraintDataverseWrapper
						{
							Column = attributeMetadata,
							isAltKey = false,
							Database = synchronizeParameters.DatabaseRow.Name
						};
						AddUniqueConstraint(dr2, SharedDatabaseTypeEnum.DatabaseType.Dataverse);
					}
				}
			}
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
		return true;
	}

	public override bool GetDependencies(string query, Form owner = null)
	{
		try
		{
			IOrganizationService organizationService = CommandsWithTimeout.Dataverse(synchronizeParameters.DatabaseRow.Connection);
			if (organizationService == null)
			{
				return false;
			}
			EntityCollection viewsWithExtendedColumns = GetViewsWithExtendedColumns(organizationService);
			HashSet<string> objectNamesToSynchronize = GetObjectNamesToSynchronize();
			foreach (Entity entity in viewsWithExtendedColumns.Entities)
			{
				if (IsVerifiedView(entity))
				{
					string entityNameFromXml = GetEntityNameFromXml((string)entity.Attributes[FetchXml], owner);
					string text = entityNameFromXml + "." + (string)entity.Attributes[Name];
					if (objectNamesToSynchronize.Contains(text) && objectNamesToSynchronize.Contains(entityNameFromXml))
					{
						DataverseDependencyWrapper dr = new DataverseDependencyWrapper
						{
							ReferencingEntityName = text,
							ReferencingType = "VIEW",
							ReferencingServer = synchronizeParameters.DatabaseRow.Host,
							ReferencingDatabase = synchronizeParameters.DatabaseRow.Name,
							ReferencedEntityName = entityNameFromXml,
							ReferencedType = "TABLE",
							ReferencedServer = synchronizeParameters.DatabaseRow.Host,
							ReferencedDatabase = synchronizeParameters.DatabaseRow.Name
						};
						AddDependency(dr);
					}
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			GeneralExceptionHandling.Handle(ex, ex.Message, ErrorMessageCaption, owner);
			return false;
		}
	}

	public override bool GetParameters(string query, bool canReadParameters = true, Form owner = null)
	{
		return true;
	}

	private HashSet<string> GetObjectNamesToSynchronize()
	{
		if (_objectNamesToSynchronizeCache != null)
		{
			return _objectNamesToSynchronizeCache;
		}
		return _objectNamesToSynchronizeCache = (from t in synchronizeParameters.DatabaseRow.tableRows
			where t.ToSynchronize
			select t into x
			select x.Name).ToHashSet();
	}

	private EntityCollection GetViewsWithExtendedColumns(IOrganizationService service)
	{
		if (_viewsWithExtendedColumnsCache != null)
		{
			return _viewsWithExtendedColumnsCache;
		}
		QueryExpression query = new QueryExpression(SavedQuery)
		{
			ColumnSet = new ColumnSet(ExtendedColumnsFields)
		};
		return _viewsWithExtendedColumnsCache = service.RetrieveMultiple(query);
	}

	private RetrieveAllEntitiesResponse GetFullEntities(IOrganizationService service)
	{
		if (_allEntitesResponseCache != null)
		{
			return _allEntitesResponseCache;
		}
		return RefreshFullEntitiesCache(service);
	}

	private RetrieveAllEntitiesResponse RefreshFullEntitiesCache(IOrganizationService service)
	{
		RetrieveAllEntitiesRequest request = new RetrieveAllEntitiesRequest
		{
			EntityFilters = (EntityFilters.Entity | EntityFilters.Attributes | EntityFilters.Relationships)
		};
		return _allEntitesResponseCache = (RetrieveAllEntitiesResponse)(service?.Execute(request));
	}

	public override IEnumerable<string> CountTablesQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountViewsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountFunctionsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> CountProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetTablesQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetViewsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetProceduresQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> ColumnsQuery(bool getComputedFormula = true)
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> UniqueConstraintsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> RelationsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> TriggersQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> GetFunctionsQuery()
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> ParametersQuery(bool canReadParameters = true)
	{
		yield return string.Empty;
	}

	public override IEnumerable<string> DependenciesQuery(bool canReadExternalDependencies = true)
	{
		yield return string.Empty;
	}
}
