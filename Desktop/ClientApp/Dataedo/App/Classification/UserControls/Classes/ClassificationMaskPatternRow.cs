using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.Classificator;
using Dataedo.Model.Enums;

namespace Dataedo.App.Classification.UserControls.Classes;

public class ClassificationMaskPatternRow : IClassificationMaskPattern, IStatedObject
{
	public ManagingRowsEnum.ManagingRows RowState { get; set; }

	public int Id { get; set; }

	public string Mask { get; set; }

	public string DataTypes { get; set; }

	public bool IsColumn { get; set; }

	public bool IsTitle { get; set; }

	public bool IsDescription { get; set; }

	public ClassificationMaskRow ClassificationMaskRow { get; private set; }

	public string MaskName => ClassificationMaskRow.MaskName;

	public ClassificationMaskPatternRow(ClassificationMaskRow classificationMaskRow)
	{
		ClassificationMaskRow = classificationMaskRow;
	}

	public ClassificationMaskPatternRow(ClassificationMaskRow classificationMaskRow, ClassificationMaskPattern classificatorCustomField)
	{
		Id = classificatorCustomField.Id;
		Mask = classificatorCustomField.Mask;
		DataTypes = classificatorCustomField.DataTypes;
		IsColumn = classificatorCustomField.IsColumn;
		IsTitle = classificatorCustomField.IsTitle;
		IsDescription = classificatorCustomField.IsDescription;
		ClassificationMaskRow = classificationMaskRow;
	}
}
