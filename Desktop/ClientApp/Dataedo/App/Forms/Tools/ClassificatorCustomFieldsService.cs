using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Licences;
using Dataedo.App.Tools.CustomFields;
using Dataedo.CustomMessageBox;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.DataProcessing.Synchronize.Classes;
using Dataedo.Model.Data.Classificator;
using Dataedo.Model.Data.CustomFields;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Forms.Tools;

public class ClassificatorCustomFieldsService
{
	private readonly int customFieldClassOtherId = 9;

	private CustomFieldsSupport support;

	public IEnumerable<CustomFieldRow> AllCustomFields { get; private set; }

	public ClassificatorCustomFieldsService(CustomFieldsSupport support)
	{
		this.support = support;
		AllCustomFields = new List<CustomFieldRow>();
	}

	private IEnumerable<string> GetNotUsedFields(IEnumerable<CustomFieldRow> existingFields, IEnumerable<CustomFieldRow> newFields)
	{
		List<string> usedNames = existingFields.Select((CustomFieldRow x) => x.FieldName).ToList();
		usedNames.AddRange(newFields.Select((CustomFieldRow x) => x.FieldName));
		return from x in Enumerable.Range(1, 100)
			select $"field{x}" into x
			where !usedNames.Contains(x)
			select x;
	}

	public void LoadAllCustomFields()
	{
		AllCustomFields = (from x in DB.CustomField.GetCustomFields(null, false)
			select new CustomFieldRowExtended(x)).ToList();
	}

	public CustomFieldRow CreateCustomField(ClassificatorCustomField classificatorCustomField, IEnumerable<CustomFieldRow> newFields, string classificatorTitle)
	{
		IEnumerable<string> notUsedFields = GetNotUsedFields(AllCustomFields, newFields);
		if (notUsedFields.Count() == 0)
		{
			CustomMessageBoxForm.Show("There are no remaining custom fields to use. Please remove any existing field to add new one.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
		}
		CustomFieldRow obj = new CustomFieldRow
		{
			Title = GetTitle(classificatorCustomField.Title, support).Trim(),
			FieldName = notUsedFields.FirstOrDefault(),
			Type = CustomFieldTypeEnum.CustomFieldType.ListOpen
		};
		bool columnVisibility = (obj.TermVisibility = true);
		obj.ColumnVisibility = columnVisibility;
		bool flag3 = (obj.DocumentationVisibility = false);
		bool flag5 = (obj.TriggerVisibility = flag3);
		bool flag7 = (obj.TableVisibility = flag5);
		bool flag9 = (obj.RelationVisibility = flag7);
		bool flag11 = (obj.ProcedureVisibility = flag9);
		bool flag13 = (obj.ParameterVisibility = flag11);
		columnVisibility = (obj.ModuleVisibility = flag13);
		obj.KeyVisibility = columnVisibility;
		obj.Definition = classificatorCustomField.GetDefinition();
		obj.Description = "Reserved for " + classificatorTitle + ".";
		obj.CustomFieldClassId = classificatorCustomField.CustomFieldClassId ?? customFieldClassOtherId;
		return obj;
	}

	public void UpdateCustomField(CustomField customFieldRow, ClassificatorCustomField classificatorCustomField, string classificatorTitle)
	{
		customFieldRow.Title = classificatorCustomField.Title.Trim();
		customFieldRow.Definition = classificatorCustomField.GetDefinition();
		customFieldRow.Description = "Reserved for " + classificatorTitle + ".";
		customFieldRow.CustomFieldClassId = classificatorCustomField.CustomFieldClassId ?? customFieldClassOtherId;
	}

	public static string GetTitle(string title, CustomFieldsSupport support)
	{
		string result = title;
		if (support.Fields.Any((CustomFieldRowExtended x) => x.Title.Equals(title)))
		{
			string text = Regex.Replace(title, "[^\\d]", "");
			if (string.IsNullOrEmpty(text))
			{
				result = title + " (1)";
			}
			else
			{
				int num = int.Parse(text);
				result = $"{title.Substring(0, title.Length - 3 - text.Length)} ({++num})";
			}
			result = GetTitle(result, support);
		}
		return result;
	}

	public int AddCustomField(CustomFieldRow field)
	{
		int num2 = (field.OrdinalPosition = ((AllCustomFields.Count() == 0) ? 1 : (support.Fields.Max((CustomFieldRowExtended x) => x.OrdinalPosition) + 1)));
		field.Code = DB.CustomField.GenerateCustomFieldCode(field.Title, AllCustomFields);
		field.CustomFieldId = DB.CustomField.InsertCustomField(field) ?? (-1);
		support.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: true);
		return field.CustomFieldId;
	}
}
