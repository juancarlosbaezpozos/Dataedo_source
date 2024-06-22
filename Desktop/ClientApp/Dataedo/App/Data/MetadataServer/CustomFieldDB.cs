using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Licences;
using Dataedo.App.Tools;
using Dataedo.Data.Base.Commands.Parameters.Delegates;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.Data.Commands;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.DataProcessing.Synchronize.Classes;
using Dataedo.Model.Data.CustomFields;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;

namespace Dataedo.App.Data.MetadataServer;

public class CustomFieldDB : BaseCustomFieldDB
{
	public CustomFieldDB(CommandsSetBase commands)
		: base(commands)
	{
	}

	public new static List<CustomField> GetConvertedCustomFieldWithOpenGeneralTypeChange(IEnumerable<CustomFieldRow> openTypeCustomFields)
	{
		List<CustomField> list = new List<CustomField>();
		foreach (CustomFieldRow openTypeCustomField in openTypeCustomFields)
		{
			CustomField customField = ConvertToCustomField(openTypeCustomField, toRemove: false);
			if (openTypeCustomField.Type == CustomFieldTypeEnum.CustomFieldType.TagsOpen)
			{
				customField.GeneralTypeChange = CustomField.GeneralTypeChangeEnum.NormalToOpenMultipleValuesDefinition;
			}
			else
			{
				customField.GeneralTypeChange = CustomField.GeneralTypeChangeEnum.NormalToOpenSingleValueDefinition;
			}
			list.Add(customField);
		}
		return list;
	}

	public void UpdateCustomFieldsAfterSynchronization(DocumentationCustomFieldRow[] documentationCustomFields)
	{
		List<CustomField> convertedCustomFieldWithOpenGeneralTypeChange = GetConvertedCustomFieldWithOpenGeneralTypeChange(from x in documentationCustomFields
			where x.CustomField?.IsOpenDefinitionType ?? false
			select x.CustomField);
		UpdateCustomFields(convertedCustomFieldWithOpenGeneralTypeChange.ToArray(), rebuildDictionaryIfNecessary: true, null);
	}

	public override int UpdateCustomFields(CustomField[] convertedCustomFields, bool rebuildDictionaryIfNecessary, ProgressSupport.UpdateProgress updateProgress, bool isServerRepository = true)
	{
		int? num = 0;
		string text = string.Empty;
		Exception ex = null;
		try
		{
			base.UpdateCustomFields(convertedCustomFields, rebuildDictionaryIfNecessary, updateProgress, isServerRepository);
		}
		catch (Exception ex2)
		{
			ex = ex2;
			text = ex2.Message;
			num = -1;
		}
		Messages.CheckAndShowErrorMessage(ex, "Error while updating custom fields: " + text, num);
		return Convert.ToInt32(num);
	}

	public new CustomField[] GetPreparedCustomFields(IEnumerable<CustomFieldRow> fields, IEnumerable<CustomFieldRow> fieldsToRemove, IEnumerable<CustomFieldRow> dbCustomFields)
	{
		CustomField[] array = ConvertToCustomFields(fields, toRemove: false);
		CustomField[] second = ConvertToCustomFields(fieldsToRemove, toRemove: true);
		foreach (CustomField convertedCustomField in array.Where((CustomField x) => x.CustomFieldId != 0))
		{
			CustomFieldRow customFieldRow = dbCustomFields.FirstOrDefault((CustomFieldRow x) => x.CustomFieldId == convertedCustomField.CustomFieldId);
			if (customFieldRow == null)
			{
				continue;
			}
			CustomFieldRow customFieldRow2 = fields.FirstOrDefault((CustomFieldRow x) => x.CustomFieldId == convertedCustomField.CustomFieldId);
			if (customFieldRow == null)
			{
				continue;
			}
			if (customFieldRow.IsOpenDefinitionType && !customFieldRow2.IsOpenDefinitionType)
			{
				convertedCustomField.GeneralTypeChange = CustomField.GeneralTypeChangeEnum.OpenDefinitonToNormal;
			}
			else if (!customFieldRow.IsOpenDefinitionType && customFieldRow2.IsOpenDefinitionType)
			{
				if (customFieldRow2.Type == CustomFieldTypeEnum.CustomFieldType.TagsOpen)
				{
					convertedCustomField.GeneralTypeChange = CustomField.GeneralTypeChangeEnum.NormalToOpenMultipleValuesDefinition;
				}
				else
				{
					convertedCustomField.GeneralTypeChange = CustomField.GeneralTypeChangeEnum.NormalToOpenSingleValueDefinition;
				}
			}
		}
		return array.Concat(second).ToArray();
	}

	public int InsertOrUpdateDocumentationCustomFields(IEnumerable<DocumentationCustomFieldRow> fields, int databaseid)
	{
		int? num = 0;
		string text = string.Empty;
		Exception ex = null;
		try
		{
			commands.Manipulation.CustomFields.InsertOrUpdateOrDeleteDocumentationCustomFields(ConvertToDocumentationCustomFields(fields, databaseid));
		}
		catch (Exception ex2)
		{
			ex = ex2;
			text = ex2.Message;
			num = -1;
		}
		Messages.CheckAndShowErrorMessage(ex, "Error while updating documentation custom fields: " + text, num);
		return Convert.ToInt32(num);
	}

	public override List<CustomFieldObject> GetCustomFields(int? customFieldsLimit = null, bool? forceJoin = false)
	{
		IEnumerable<int> classificatorCustomFields = DB.Classificator.CustomFieldsId(!Dataedo.App.StaticData.IsProjectFile);
		bool joinClasificatorCustomFields = forceJoin == true || (!Dataedo.App.StaticData.IsProjectFile && Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataClassification));
		return commands.Select.CustomFields.GetCustomFields(customFieldsLimit, classificatorCustomFields, joinClasificatorCustomFields);
	}

	public List<DocumentationCustomFieldRow> GetDocumentationCustomFields(int? databaseId)
	{
		List<DocumentationCustomFieldRow> list = (from x in commands.Select.CustomFields.GetDocumentationCustomFields(databaseId, Licence.GetCustomFieldsLimit())
			select new DocumentationCustomFieldRow(x, databaseId)).ToList();
		IEnumerable<CustomFieldRowExtended> source = from x in DB.CustomField.GetCustomFields(null, false)
			select new CustomFieldRowExtended(x);
		foreach (DocumentationCustomFieldRow documentationCustomField in list)
		{
			documentationCustomField.CustomField = source.FirstOrDefault((CustomFieldRowExtended x) => x.CustomFieldId == documentationCustomField.CustomFieldId);
		}
		return list;
	}

	public new List<string> GetCustomFieldDistinctValues(int customFieldId)
	{
		return commands.Select.CustomFields.GetCustomFieldDistinctValues(customFieldId);
	}

	public new string GenerateCustomFieldCode(string title, IEnumerable<CustomFieldRow> existingFields)
	{
		string text = title?.ToLower()?.Replace(' ', '_');
		if (IsFieldCodeUnique(text, existingFields))
		{
			return text;
		}
		return GetNextCustomFieldCode(text, existingFields);
	}

	private bool IsFieldCodeUnique(string fieldCode, IEnumerable<CustomFieldRow> existingFields)
	{
		return !existingFields.Select((CustomFieldRow x) => x.Code).Contains(fieldCode);
	}

	private string GetNextCustomFieldCode(string code, IEnumerable<CustomFieldRow> existingFields)
	{
		int i;
		for (i = 1; !IsFieldCodeUnique($"{code}_{i}", existingFields); i++)
		{
		}
		return $"{code}_{i}";
	}

	public new static CustomField ConvertToCustomField(CustomFieldRow field, bool toRemove)
	{
		return new CustomField
		{
			CustomFieldId = field.CustomFieldId,
			OrdinalPosition = field.OrdinalPosition,
			FieldName = field.FieldName,
			Title = field.Title,
			Code = field.Code,
			Description = field.Description,
			TableVisibility = field.TableVisibility,
			ProcedureVisibility = field.ProcedureVisibility,
			ColumnVisibility = field.ColumnVisibility,
			RelationVisibility = field.RelationVisibility,
			KeyVisibility = field.KeyVisibility,
			TriggerVisibility = field.TriggerVisibility,
			ParameterVisibility = field.ParameterVisibility,
			ModuleVisibility = field.ModuleVisibility,
			DocumentationVisibility = field.DocumentationVisibility,
			TermVisibility = field.TermVisibility,
			Type = CustomFieldTypeEnum.TypeToString(field.Type),
			Definition = field.Definition,
			CustomFieldClassId = field.CustomFieldClassId,
			CustomFieldClassName = field.CustomFieldClassName,
			IsToDelete = toRemove
		};
	}

	public new static CustomField[] ConvertToCustomFields(IEnumerable<CustomFieldRow> fields, bool toRemove)
	{
		return fields.Select((CustomFieldRow x) => ConvertToCustomField(x, toRemove)).ToArray();
	}

	private DocumentationCustomField[] ConvertToDocumentationCustomFields(IEnumerable<DocumentationCustomFieldRow> fields, int databaseId)
	{
		return fields.Select((DocumentationCustomFieldRow x) => new DocumentationCustomField
		{
			IsSelected = x.IsSelected,
			DocumentationCustomFieldId = x.DocumentationCustomFieldId,
			CustomFieldId = x.CustomFieldId,
			DatabaseId = databaseId,
			OrdinalPosition = x.OrdinalPosition,
			FieldName = x.FieldName,
			Title = x.Title,
			ExtendedProperty = x.ExtendedProperty
		}).ToArray();
	}
}
