using System;
using System.Collections.Generic;
using System.Linq;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.Classificator;
using Dataedo.Model.Enums;
using DevExpress.Utils;

namespace Dataedo.App.Classification.UserControls.Classes;

public class ClassificatorCustomFieldRow : ClassificatorCustomField, IStatedObject
{
	private ClassificatorModelRow classificatorModelRow;

	public ManagingRowsEnum.ManagingRows RowState { get; set; }

	private IEnumerable<string> ListOfLabelsFromMasks => classificatorModelRow.Rules?.Select((ClassificationRuleRow x) => x.GetCustomFieldValue(base.Number))?.OrderBy((string x) => x)?.Where((string x) => !string.IsNullOrWhiteSpace(x))?.Distinct() ?? new List<string>();

	public string LabelsFromMasks => string.Join(", ", ListOfLabelsFromMasks);

	public string AdditionalLabels { get; set; }

	public IEnumerable<string> ListOfAdditionalLabels => AdditionalLabels?.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries)?.Select((string x) => x.Trim()) ?? new List<string>();

	public string CustomFieldFieldType => "Drop-down list (open)";

	public IEnumerable<string> AllLabels => (from x in ListOfLabelsFromMasks.Union(ListOfAdditionalLabels)
		orderby x
		select x).Distinct();

	public ClassificatorCustomFieldRow(ClassificatorModelRow classificatorModelRow, ClassificatorCustomField classificatorCustomField)
	{
		this.classificatorModelRow = classificatorModelRow;
		base.Title = classificatorCustomField.Title;
		base.Id = classificatorCustomField.Id;
		base.Number = classificatorCustomField.Number;
		base.Definition = classificatorCustomField.Definition;
		base.IdFieldName = classificatorCustomField.IdFieldName;
		base.ValueFieldName = classificatorCustomField.ValueFieldName;
		base.CustomFieldClassId = classificatorCustomField.CustomFieldClassId;
		base.CustomFieldFieldName = classificatorCustomField.CustomFieldFieldName;
		AdditionalLabels = string.Join(", ", base.DefinitionValues.Except(ListOfLabelsFromMasks));
	}

	public ClassificatorCustomFieldRow(ClassificatorModelRow classificatorModelRow, string title, int number, int? customFieldClassId)
	{
		Guard.ArgumentIsInRange(1, 5, number, "New Classification Field number");
		this.classificatorModelRow = classificatorModelRow;
		base.CustomFieldClassId = customFieldClassId;
		base.Title = title;
		base.Number = number;
		base.IdFieldName = $"custom_field_{number}_id";
		base.ValueFieldName = $"custom_field_{number}_value";
	}

	public override string GetDefinition()
	{
		return string.Join(", ", AllLabels);
	}
}
