namespace Dataedo.App.Classification.UserControls.Classes;

public class ClassificatorPresenterRow
{
	public string CustomFieldValue { get; set; }

	public int ClassifiedFieldNumber { get; set; }

	public ClassificatorPresenterRow(string customFieldValue, int classifiedFieldNumber)
	{
		CustomFieldValue = customFieldValue;
		ClassifiedFieldNumber = classifiedFieldNumber;
	}
}
