using Dataedo.Model.Data.Classificator;

namespace Dataedo.App.Classification.UserControls.Classes;

public class ClassificationMaskPresence
{
	public IClassificatorModel Classificator { get; set; }

	public bool IsPresent { get; set; }

	public bool IsChanged { get; set; }

	public string ClassificatorTitle => Classificator.Title;

	public int ClassificatorId => Classificator.Id;

	public ClassificationMaskRow ClassificationMaskRow { get; private set; }

	public string MaskName => ClassificationMaskRow.MaskName;

	public ClassificationMaskPresence(ClassificationMaskRow classificationMaskRow, IClassificatorModel classificator)
	{
		ClassificationMaskRow = classificationMaskRow;
		Classificator = classificator;
	}

	public ClassificationRule ToClassificationRule()
	{
		return new ClassificationRule
		{
			ClassificatorId = ClassificatorId,
			MaskName = MaskName
		};
	}
}
