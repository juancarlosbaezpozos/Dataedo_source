using System;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Documentation;
using Dataedo.App.Documentation.Template.PdfTemplate.Model.ChildModels;
using Dataedo.App.Tools;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Common.CustomFields;
using Dataedo.Model.Data.Common.Interfaces;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.Documentations;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Documentation;

public class ObjectDoc : IdStringBase, ISupportsCustomFields
{
	private SharedObjectSubtypeEnum.ObjectSubtype? objectSubtype;

	public static readonly string PlainTextNewLineSeparator = Environment.NewLine;

	public static readonly string HtmlNewLineSeparator = "<br/>";

	public virtual string NewLineSeparator { get; set; } = PlainTextNewLineSeparator;


	public int? Id { get; set; }

	public string Schema { get; set; }

	public string ObjectName { get; set; }

	public string ObjectTitle { get; set; }

	public string Name => NameBase;

	public string NameHtml { get; set; }

	protected string NameBase { get; set; }

	public string NameHeader { get; set; }

	public string Description { get; set; }

	public DocGeneratingOptions GeneratingOptions { get; set; }

	public CustomFieldContainer CustomFields { get; set; }

	public SharedObjectTypeEnum.ObjectType? ObjectTypeValue => ObjectType;

	public SharedObjectTypeEnum.ObjectType? ObjectType { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype ObjectSubtype
	{
		get
		{
			return objectSubtype ?? SharedObjectSubtypeEnum.GetDefaultByMainType(ObjectType);
		}
		set
		{
			objectSubtype = value;
		}
	}

	public string CustomFieldsString => CustomFields.GetCustomFieldsString(ObjectType);

	public string CustomFieldsStringHtml => PrepareValue.FixDescription(CustomFieldsString);

	public bool HasDescription => !string.IsNullOrEmpty(Description);

	public bool HasDescriptionOrCustomFields
	{
		get
		{
			if (string.IsNullOrEmpty(Description))
			{
				return !string.IsNullOrEmpty(CustomFieldsString);
			}
			return true;
		}
	}

	public string CustomFieldsStringWithNewLinesHtml
	{
		get
		{
			string customFieldsString = CustomFieldsString;
			if (!string.IsNullOrEmpty(customFieldsString))
			{
				return PrepareValue.FixDescription(customFieldsString + HtmlNewLineSeparator + HtmlNewLineSeparator);
			}
			return customFieldsString;
		}
	}

	public virtual string CustomFieldsStringValuesSeparator => Environment.NewLine;

	public string NameDescription => string.Join(PlainTextNewLineSeparator, new string[2] { NameBase, Description }.Where((string x) => !string.IsNullOrEmpty(x)));

	public string NameDescriptionHtml => string.Join(HtmlNewLineSeparator, new string[2] { NameBase, Description }.Where((string x) => !string.IsNullOrEmpty(x)));

	public int ObjectOrder { get; set; }

	public int? ObjectsOnSameLevelCount { get; set; }

	public SharedDatabaseTypeEnum.DatabaseType? DatabaseType { get; set; }

	public int? DocumentationId { get; set; }

	public string DocumentationTitle { get; set; }

	public bool? HasDatabaseMultipleSchemas { get; set; }

	public bool? ShowSchema { get; set; }

	public bool? ShowSchemaOverride { get; set; }

	public bool ShowSchemaEffective => DatabaseRow.GetShowSchema(ShowSchema, ShowSchemaOverride);

	public bool? DatabaseShowSchema { get; set; }

	public bool? DatabaseShowSchemaOverride { get; set; }

	public bool DatabaseShowSchemaEffective => DatabaseRow.GetShowSchema(DatabaseShowSchema, DatabaseShowSchemaOverride);

	public SharedDatabaseTypeEnum.DatabaseType? CurrentObjectDocumentationType { get; set; }

	public bool? IsFromAnotherDocumentation { get; set; }

	public bool? IsInMultipleDatabaseModule { get; set; }

	public DocHeaders DocHeaders { get; set; }

	public int Order { get; set; }

	public bool UseSchema { get; set; }

	public HeadingsNamesModel HeadingsNamesModel { get; set; }

	public bool UseModuleString { get; set; }

	public ObjectDoc()
	{
	}

	public ObjectDoc(DocGeneratingOptions docGeneratingOptions, IBasicIdentification row, SharedObjectTypeEnum.ObjectType objectType, bool withName = true, bool retrieveDescription = true, bool retrieveCustomFields = true, int objectOrder = 1)
	{
		ObjectType = objectType;
		if (row != null && objectType != SharedObjectTypeEnum.ObjectType.Module)
		{
			ObjectName = PrepareValue.ToString(row.Name);
		}
		GeneratingOptions = docGeneratingOptions;
		CustomFields = new CustomFieldContainer(GeneratingOptions.CustomFields, FormatCustomField, CustomFieldsStringValuesSeparator);
		if (withName)
		{
			NameBase = ObjectName;
		}
		if (retrieveDescription)
		{
			RetrieveDescription(row as CustomFieldsData, retrieveCustomFields);
		}
		else if (retrieveCustomFields)
		{
			RetrieveCustomFields(row as CustomFieldsData);
		}
		ObjectOrder = objectOrder;
	}

	public ObjectDoc(DocGeneratingOptions docGeneratingOptions, DocumentationObject row, SharedObjectTypeEnum.ObjectType objectType, int objectOrder = 1, int? objectsOnSameLevelCount = null)
	{
		GeneratingOptions = docGeneratingOptions;
		CustomFields = new CustomFieldContainer(GeneratingOptions.CustomFields, FormatCustomField, CustomFieldsStringValuesSeparator);
		ObjectType = objectType;
		ObjectName = PrepareValue.ToString(row.Name);
		ObjectOrder = objectOrder;
		ObjectsOnSameLevelCount = objectsOnSameLevelCount;
		if (objectType == SharedObjectTypeEnum.ObjectType.Database)
		{
			ObjectTitle = PrepareValue.ToString(row.Title);
			NameBase = ObjectName;
			DocHeaders docHeaders = new DocHeaders(objectOrder, null);
			NameHeader = docHeaders.CreateModuleNameWithOrder(ObjectTitle, ObjectsOnSameLevelCount > 1);
			RetrieveDescription(row);
		}
	}

	public ObjectDoc(DocGeneratingOptions docGeneratingOptions, DatabaseDoc database, ObjectDocObject row, DocHeaders docHeaders, int order, SharedDatabaseTypeEnum.DatabaseType? databaseType, bool useSchema, SharedObjectTypeEnum.ObjectType objectType, SharedObjectSubtypeEnum.ObjectSubtype objectSubtype, HeadingsNamesModel headingsNamesModel, bool useModuleString, string objectTypeString = null)
	{
		DocHeaders = docHeaders;
		Order = order;
		UseSchema = useSchema;
		HeadingsNamesModel = headingsNamesModel;
		UseModuleString = useModuleString;
		DatabaseShowSchema = database.DatabaseShowSchema;
		DatabaseShowSchemaOverride = database.DatabaseShowSchemaOverride;
		DatabaseType = databaseType;
		LoadStandardData(database, row);
		GeneratingOptions = docGeneratingOptions;
		CustomFields = new CustomFieldContainer(GeneratingOptions.CustomFields, FormatCustomField, CustomFieldsStringValuesSeparator);
		ObjectType = objectType;
		ObjectName = row.Name;
		ObjectTitle = row.Title;
		Schema = row.Schema;
		NameBase = PrepareValue.CreateNameDisplayed(ObjectName, ObjectTitle);
		ObjectSubtype = objectSubtype;
		if (ShowSchemaEffective || DatabaseShowSchemaEffective)
		{
			if (objectTypeString == null)
			{
				NameHeader = docHeaders.CreateNameWithOrder(string.IsNullOrEmpty(row.Schema) ? Name : (row.Schema + "." + Name), order, objectType, objectSubtype, headingsNamesModel, useModuleString);
			}
			else
			{
				NameHeader = docHeaders.CreateNameWithOrder(string.IsNullOrEmpty(row.Schema) ? Name : (row.Schema + "." + Name), order, objectTypeString, headingsNamesModel, useModuleString);
			}
		}
		else if (objectTypeString == null)
		{
			NameHeader = DocHeaders.CreateNameWithOrder(Name, order, objectType, objectSubtype, headingsNamesModel, useModuleString);
		}
		else
		{
			NameHeader = DocHeaders.CreateNameWithOrder(Name, order, objectTypeString, headingsNamesModel, useModuleString);
		}
		RetrieveDescription(row);
	}

	public void FixHeader()
	{
		string text = string.Empty;
		if (IsFromAnotherDocumentation == true)
		{
			text = DocumentationTitle + ".";
		}
		if (ShowSchemaEffective || DatabaseShowSchemaEffective)
		{
			NameHeader = DocHeaders.CreateNameWithOrder(string.IsNullOrEmpty(Schema) ? (text + Name) : (text + Schema + "." + Name), Order, ObjectType.Value, ObjectSubtype, HeadingsNamesModel, UseModuleString);
		}
		else
		{
			NameHeader = DocHeaders.CreateNameWithOrder(text + Name, Order, ObjectType.Value, ObjectSubtype, HeadingsNamesModel, UseModuleString);
		}
	}

	protected virtual void RetrieveDescription(CustomFieldsData row, bool retrieveCustomFields = true)
	{
		if (retrieveCustomFields)
		{
			RetrieveCustomFields(row);
		}
		if (row is IDescription)
		{
			string descriptionWithHtmlNewLines = GetDescriptionWithHtmlNewLines((row as IDescription).Description);
			Description = (string.IsNullOrEmpty(descriptionWithHtmlNewLines) ? CustomFieldsString : (descriptionWithHtmlNewLines + NewLineSeparator + CustomFieldsString));
		}
	}

	protected string GetDescriptionWithHtmlNewLines(string description)
	{
		return description?.Replace(Environment.NewLine, HtmlNewLineSeparator);
	}

	protected virtual string FormatCustomField(string title, string fieldValue)
	{
		if (!string.IsNullOrEmpty(fieldValue))
		{
			return "<b>" + title + "</b>: " + fieldValue.Replace(Environment.NewLine, HtmlNewLineSeparator);
		}
		return string.Empty;
	}

	protected void RetrieveDescriptionAsHtml(CustomFieldsData row, bool retrieveCustomFields = true)
	{
		if (retrieveCustomFields)
		{
			RetrieveCustomFields(row);
		}
		string text = (row as IDescription).Description;
		if (text != null && !text.StartsWith("<!DOCTYPE html"))
		{
			text = PrepareValue.FixDescription(text);
		}
		Description = text;
	}

	protected void RetrieveCustomFieldsAsHtmlWithDescription(CustomFieldsData row, bool retrieveCustomFields = true)
	{
		if (retrieveCustomFields)
		{
			RetrieveCustomFields(row);
		}
		string text = (row as IDescription).Description;
		if (text != null && !text.StartsWith("<!DOCTYPE html"))
		{
			text = PrepareValue.FixDescription(text);
		}
		Description = CustomFieldsStringWithNewLinesHtml + text;
	}

	protected void RetrieveDescriptionWithCustomFieldsAsHtml(CustomFieldsData row, bool retrieveCustomFields = true)
	{
		if (retrieveCustomFields)
		{
			RetrieveCustomFields(row);
		}
		string text = (row as IDescription).Description;
		if (text != null && !text.StartsWith("<!DOCTYPE html"))
		{
			text = PrepareValue.FixDescription(text);
		}
		Description = (string.IsNullOrEmpty(text) ? CustomFieldsStringHtml : (text + Environment.NewLine + Environment.NewLine + CustomFieldsStringHtml));
	}

	protected void RetrieveCustomFields(ICustomFieldsData customFieldsData)
	{
		CustomFields.RetrieveCustomFields(customFieldsData);
	}

	protected virtual void LoadStandardData(DatabaseDoc database, ObjectDocObject row)
	{
	}
}
