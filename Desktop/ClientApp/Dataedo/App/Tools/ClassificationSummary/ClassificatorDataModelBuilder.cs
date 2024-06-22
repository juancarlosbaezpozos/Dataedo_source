using Dataedo.Model.Data.Classificator;
using Dataedo.Shared.Tools.ERD;

namespace Dataedo.App.Tools.ClassificationSummary;

public class ClassificatorDataModelBuilder
{
	private ClassificatorDataModel model;

	private string field1Name;

	private string field2Name;

	private string field3Name;

	private string field4Name;

	private string field5Name;

	public ClassificatorDataModelBuilder(string field1Name, string field2Name, string field3Name, string field4Name, string field5Name)
	{
		model = new ClassificatorDataModel();
		this.field1Name = field1Name;
		this.field2Name = field2Name;
		this.field3Name = field3Name;
		this.field4Name = field4Name;
		this.field5Name = field5Name;
	}

	public ClassificatorDataModel Build(Dataedo.Model.Data.Classificator.Classification row, int selectedCustomFieldsCount)
	{
		BuildStandardProperties(row);
		BuildCustomFieldProperties(row, selectedCustomFieldsCount);
		return model;
	}

	private void BuildStandardProperties(Dataedo.Model.Data.Classificator.Classification row)
	{
		model.Documentation = row.Title;
		model.Table = row.ObjectNameDisplay;
		model.Column = ColumnNames.GetFullName(row.ColumnPath, row.ColumnNameDisplay);
		model.ColumnFormatted = ColumnNames.GetFullNameFormatted(row.ColumnPath, row.ColumnNameDisplay);
		model.DataType = row.Datatype;
		model.ColumnId = row.ColumnId;
		model.Comment = row.Comments;
		model.Description = row.Description;
		model.DatabaseId = row.DatabaseId;
	}

	private void BuildCustomFieldProperties(Dataedo.Model.Data.Classificator.Classification row, int selectedCustomFieldsCount)
	{
		bool flag = false;
		if (selectedCustomFieldsCount > 0)
		{
			model.Field1Update = row.CustomField_1_Value;
			model.Field1Value = row.GetField(field1Name);
			model.Field1DBName = field1Name;
			flag |= string.IsNullOrEmpty(model.Field1Value) && !string.IsNullOrEmpty(model.Field1Update);
		}
		if (selectedCustomFieldsCount > 1)
		{
			model.Field2Update = row.CustomField_2_Value;
			model.Field2Value = row.GetField(field2Name);
			model.Field2DBName = field2Name;
			flag |= string.IsNullOrEmpty(model.Field2Value) && !string.IsNullOrEmpty(model.Field2Update);
		}
		if (selectedCustomFieldsCount > 2)
		{
			model.Field3Update = row.CustomField_3_Value;
			model.Field3Value = row.GetField(field3Name);
			model.Field3DBName = field3Name;
			flag |= string.IsNullOrEmpty(model.Field3Value) && !string.IsNullOrEmpty(model.Field3Update);
		}
		if (selectedCustomFieldsCount > 3)
		{
			model.Field4Update = row.CustomField_4_Value;
			model.Field4Value = row.GetField(field4Name);
			model.Field4DBName = field4Name;
			flag |= string.IsNullOrEmpty(model.Field4Value) && !string.IsNullOrEmpty(model.Field4Update);
		}
		if (selectedCustomFieldsCount > 4)
		{
			model.Field5Update = row.CustomField_5_Value;
			model.Field5Value = row.GetField(field5Name);
			model.Field5DBName = field5Name;
			flag |= string.IsNullOrEmpty(model.Field5Value) && !string.IsNullOrEmpty(model.Field5Update);
		}
		model.IsChecked = flag;
		model.IsProcessed = !flag;
	}
}
