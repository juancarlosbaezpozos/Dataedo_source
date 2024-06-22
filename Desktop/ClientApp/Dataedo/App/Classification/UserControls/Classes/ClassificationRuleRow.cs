using System.Linq;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.Classificator;
using Dataedo.Model.Enums;

namespace Dataedo.App.Classification.UserControls.Classes;

public class ClassificationRuleRow : IStatedObject, IClassificationRule
{
	public ManagingRowsEnum.ManagingRows RowState { get; set; }

	public int Id { get; set; }

	public ClassificatorModelRow Classificator { get; set; }

	public ClassificationMaskRow Mask { get; set; }

	public string CustomField_1_Value { get; set; }

	public string CustomField_2_Value { get; set; }

	public string CustomField_3_Value { get; set; }

	public string CustomField_4_Value { get; set; }

	public string CustomField_5_Value { get; set; }

	public string MaskPatterns => string.Join(",", Mask.Patterns.Select((ClassificationMaskPatternRow x) => x.Mask));

	public int ClassificatorId => Classificator.Id;

	public string MaskName => Mask.MaskName;

	public ClassificationRuleRow()
	{
	}

	public ClassificationRuleRow(ClassificationRule classificatorRule, ClassificatorModelRow classificatorModelRow, ClassificationMaskRow classificationMaskRow)
	{
		Id = classificatorRule.Id;
		CustomField_1_Value = classificatorRule.CustomField_1_Value;
		CustomField_2_Value = classificatorRule.CustomField_2_Value;
		CustomField_3_Value = classificatorRule.CustomField_3_Value;
		CustomField_4_Value = classificatorRule.CustomField_4_Value;
		CustomField_5_Value = classificatorRule.CustomField_5_Value;
		Mask = classificationMaskRow;
		Classificator = classificatorModelRow;
	}
}
