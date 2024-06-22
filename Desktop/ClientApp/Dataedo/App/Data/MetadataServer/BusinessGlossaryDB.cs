using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.Data.Base.Commands.Executing;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.FeedbackWidgetData;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Data.MetadataServer;

internal class BusinessGlossaryDB : CommonDBSupport
{
	public BusinessGlossaryDB()
	{
		commands = Dataedo.App.StaticData.Commands;
	}

	public List<BusinessGlossaryObject> GetBusinessGlossaries(int? documentationId)
	{
		return commands.Select.BusinessGlossaries.GetBusinessGlossaries(documentationId);
	}

	public int? InsertBusinessGlossary(string title, Form owner = null)
	{
		try
		{
			return commands.Manipulation.BusinessGlossaries.InsertBusinessGlossary(title);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while inserting Business Glossary:", owner);
			return null;
		}
	}

	public void UpdateBusinessGlossaryName(int? databaseId, Form owner = null)
	{
		try
		{
			commands.Manipulation.BusinessGlossaries.UpdateBusinessGlossaryName(databaseId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating Business Glossary name", owner);
		}
	}

	public List<string> GetTitleExistingNumbers(string baseName)
	{
		return commands.Select.BusinessGlossaries.GetTitleExistingNumbers(baseName);
	}

	public bool CheckIfNewTermTitleExists(string baseName)
	{
		return commands.Select.BusinessGlossaries.CheckIfTitleExists(baseName);
	}

	public int? InsertTerm(int documentationId, int? parentId, string title, int? termTypeId)
	{
		if (!termTypeId.HasValue)
		{
			termTypeId = GetDefaultTermTypeId();
		}
		return commands.Manipulation.BusinessGlossaries.InsertTerm(documentationId, termTypeId, parentId, title);
	}

	public int? InsertTerm(int documentationId, int? parentId, string title, int? termTypeId, string description, string descriptionPlain)
	{
		if (!termTypeId.HasValue)
		{
			termTypeId = GetDefaultTermTypeId();
		}
		return commands.Manipulation.BusinessGlossaries.InsertTermWithDescription(documentationId, termTypeId, parentId, title, description, descriptionPlain);
	}

	public int? GetDefaultTermTypeId()
	{
		int? result = commands.Select.BusinessGlossaries.GetDefaultTermType()?.TermTypeId;
		if (!result.HasValue)
		{
			result = InsertDefaultTermType();
		}
		return result;
	}

	public BaseFeedbackWidgetDataObject GetMovedTermsFeedbackData(params int[] termIDs)
	{
		if (termIDs == null)
		{
			return null;
		}
		return commands.Select.BusinessGlossaries.GetMovedTermsFeedbackData(null, termIDs);
	}

	public void IncreaseBGFeedbackData(int id, BaseFeedbackWidgetDataObject data)
	{
		commands.Manipulation.BusinessGlossaries.IncreaseBusinessGlossaryFeedbackData(id, data);
	}

	public void DecreaseBGFeedbackData(int id, BaseFeedbackWidgetDataObject data)
	{
		commands.Manipulation.BusinessGlossaries.DecreaseBusinessGlossaryFeedbackData(id, data);
	}

	private int? InsertDefaultTermType()
	{
		return commands.Manipulation.BusinessGlossaries.InsertTermType("Term", 1);
	}

	public List<TermObject> GetAllTerms(int? databaseId)
	{
		return GetAllTerms<TermObject>(databaseId);
	}

	public List<T> GetAllTerms<T>(int? databaseId) where T : TermObject, new()
	{
		return new List<T>(from x in commands.Select.BusinessGlossaries.GetTerms<T>(databaseId, null, anyParentId: true, null, null)
			orderby x.Title
			select x);
	}

	public List<TermObject> GetRootTerms(int? databaseId)
	{
		return commands.Select.BusinessGlossaries.GetTerms<TermObject>(databaseId, null, anyParentId: false, null, null);
	}

	public List<TermObject> GetTerms(int? parentId)
	{
		return commands.Select.BusinessGlossaries.GetTerms<TermObject>(null, parentId, anyParentId: false, null, null);
	}

	public TermObject GetTerm(int termId)
	{
		return commands.Select.BusinessGlossaries.GetTerms<TermObject>(null, null, anyParentId: true, termId, null).FirstOrDefault();
	}

	public List<TermDocObject> GetGlossaryEntriesDoc(int databaseId, SharedTermTypeEnum.TermType[] excludedTypes, bool withOther)
	{
		return commands.Select.BusinessGlossaries.GetTermsDoc(databaseId, (from x in SharedTermTypeEnum.GetTermTypes()
			select SharedTermTypeEnum.TypeToString(x)).ToArray(), excludedTypes.Select((SharedTermTypeEnum.TermType x) => SharedTermTypeEnum.TypeToString(x)).ToArray(), withOther);
	}

	public List<TermTypeObject> GetTermTypes()
	{
		return commands.Select.BusinessGlossaries.GetTermTypes();
	}

	public bool UpdateTerm(TermObject termObject, Form owner = null)
	{
		try
		{
			commands.Manipulation.BusinessGlossaries.UpdateTerm(termObject);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the term:", owner);
			return false;
		}
		return true;
	}

	public bool UpdateTerm(TermObject termObject, CustomFieldsSupport customFieldsSupport, Form owner = null)
	{
		try
		{
			customFieldsSupport.SetCustomFieldsAsActive(SharedObjectTypeEnum.ObjectType.Term, termObject);
			commands.Manipulation.BusinessGlossaries.UpdateTerm(termObject);
			DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.Term, termObject.TermId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the term:", owner);
			return false;
		}
		return true;
	}

	public void BulkCopyTermsUpdate(List<int> ids, string value, string fieldName, Form owner = null)
	{
		try
		{
			commands.Manipulation.BusinessGlossaries.BulkCopyTermsUpdate(ids, value, fieldName);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating terms", owner);
		}
	}

	public void BulkCopyTermsUpdateSingleTerm(int id, Dictionary<string, object> keyValuePairs, DbTransaction transaction = null, Form owner = null)
	{
		try
		{
			commands.Manipulation.BusinessGlossaries.BulkCopyTermsUpdateSingleTerm(id, keyValuePairs);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating terms", owner);
		}
	}

	public bool UpdateTermTitle(int termId, string title, Form owner = null)
	{
		try
		{
			commands.Manipulation.BusinessGlossaries.UpdateTermTitle(termId, title);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the term:", owner);
			return false;
		}
		return true;
	}

	public bool Delete(Term[] terms, Form owner = null)
	{
		try
		{
			if (Dataedo.App.StaticData.IsProjectFile)
			{
				commands.Manipulation.BusinessGlossaries.DeleteTermsCE(terms);
			}
			else
			{
				commands.Manipulation.BusinessGlossaries.DeleteTerms(terms);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the term:", owner);
			return false;
		}
		return true;
	}

	public bool ChangeTermParent(int termId, int documenationId, int? parentId, Form owner = null)
	{
		try
		{
			commands.Manipulation.BusinessGlossaries.ChangeTermParent(termId, documenationId, parentId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the term:", owner);
			return false;
		}
		return true;
	}

	public bool ChangeTermsParent(int? currentParentId, int? newDocumentationId, int? newParentId, Form owner = null)
	{
		try
		{
			commands.Manipulation.BusinessGlossaries.ChangeTermsParent(currentParentId, newDocumentationId, newParentId);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating the term:", owner);
			return false;
		}
		return true;
	}

	public List<TermRelationshipObjectExtended> GetTermRelationships(int termId)
	{
		return new List<TermRelationshipObjectExtended>(from x in commands.Select.BusinessGlossaries.GetTermRelationships<TermRelationshipObjectExtended>(termId)
			orderby x.RelatedTermTitle, x.RelatedTermId, x.RelationshipTitle
			select x);
	}

	public List<TermRelationshipObjectSimple> GetTermParentChildRelationships(int termId)
	{
		return new List<TermRelationshipObjectSimple>(from x in commands.Select.BusinessGlossaries.GetTermParentChildRelationships<TermRelationshipObjectSimple>(termId)
			orderby x.RelatedTermTitle, x.RelatedTermId, x.RelationshipTitle
			select x);
	}

	public List<TermRelationshipTypeObject> GetTermRelationshipTypes()
	{
		return commands.Select.BusinessGlossaries.GetTermRelationshipTypes();
	}

	public List<TermObjectWithRelationshipExtended> GetTermsAndTermRelationships(int? termId, int? termIdFilter)
	{
		return new List<TermObjectWithRelationshipExtended>(from x in commands.Select.BusinessGlossaries.GetTermsAndTermRelationships<TermObjectWithRelationshipExtended>(termId, termIdFilter)
			orderby x.RelationshipRelatedTermTitle, x.TermId, x.RelationshipTitle
			select x);
	}

	public bool ProcessSavingTermsRelationships(IEnumerable<TermObjectWithRelationshipExtended> items)
	{
		return commands.Manipulation.BusinessGlossaries.ProcessSavingTermsRelationships(items);
	}

	public bool ProcessSavingTermsRelationships(IEnumerable<TermRelationshipObjectExtended> items)
	{
		if (items == null)
		{
			return false;
		}
		return commands.Manipulation.BusinessGlossaries.ProcessSavingTermsRelationships(items);
	}

	public List<T> GetTermsAndTermLinks<T>(int? termId, int? objectId, int? elementId, string objectType) where T : TermObjectWithLink, new()
	{
		return commands.Select.BusinessGlossaries.GetTermsAndTermLinks<T>(termId, objectId, elementId, objectType);
	}

	public List<T> GetDataLinks<T>(int? termId, int? objectId, bool applyWithShowSchemaOrdering = false) where T : DataLinkObjectExtended, new()
	{
		List<T> source = new List<T>(commands.Select.BusinessGlossaries.GetDataLinks<T>(termId, objectId));
		source = ((!applyWithShowSchemaOrdering) ? (from x in source
			orderby x.ObjectDocumentationTitle, x.ObjectSchema, x.ObjectNameWithTitle, x.ElementName
			select x).ToList() : (from x in source
			orderby x.ObjectDocumentationTitle, x.ObjectSchemaShowSchema, x.ObjectNameWithTitle, x.ElementName
			select x).ToList());
		source.ForEach(delegate(T x)
		{
			x.LinkedObjectTypeForDisplay = SharedObjectTypeEnum.TypeToStringForSingle(SharedObjectTypeEnum.StringToType(x.LinkedObjectType));
		});
		return source;
	}

	public List<T> GetDataLinks<T>(int termId, bool applyWithShowSchemaOrdering = false) where T : DataLinkObjectExtended, new()
	{
		return GetDataLinks<T>(termId, null, applyWithShowSchemaOrdering);
	}

	public Task<List<ObjectWithDataLinkExtended>> GetObjectsWithDataLinks(int? termId, string name, bool mappedOnly, IEnumerable<string> types, int? maxResultsCount, out ICommandExtended command)
	{
		return commands.Select.BusinessGlossaries.GetObjectsWithDataLinks<ObjectWithDataLinkExtended>(termId, name, mappedOnly, null, types, maxResultsCount, out command);
	}

	public void ProcessSavingDataLinks(IEnumerable<ObjectWithDataLinkExtended> items)
	{
		commands.Manipulation.BusinessGlossaries.ProcessSavingDataLinks(items);
	}

	public void ProcessSavingDataLinks(IEnumerable<DataLinkObjectExtended> items)
	{
		commands.Manipulation.BusinessGlossaries.ProcessSavingDataLinks(items);
	}

	public void ProcessSavingDataLinks(IEnumerable<TermObjectWithLinkExtended> items)
	{
		commands.Manipulation.BusinessGlossaries.ProcessSavingDataLinks(items);
	}

	public bool InsertDataLink(DataLinkObjectBase item)
	{
		return commands.Manipulation.BusinessGlossaries.InsertDataLinkIfNotExist(item);
	}

	public List<T> GetLinkableObjects<T>(int? objectId, DbTransaction transaction = null) where T : LinkableObject, new()
	{
		return commands.Select.BusinessGlossaries.GetLinkableObjects<T>(objectId, transaction);
	}
}
