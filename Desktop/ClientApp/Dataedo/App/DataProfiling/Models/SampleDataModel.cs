namespace Dataedo.App.DataProfiling.Models;

public class SampleDataModel
{
	public string ColumnName { get; set; }

	public string Value1 { get; set; }

	public string Value2 { get; set; }

	public string Value3 { get; set; }

	public string Value4 { get; set; }

	public string Value5 { get; set; }

	public string Value6 { get; set; }

	public string Value7 { get; set; }

	public string Value8 { get; set; }

	public string Value9 { get; set; }

	public string Value10 { get; set; }

	public SampleDataModel(string columnName, string value1 = null, string value2 = null, string value3 = null, string value4 = null, string value5 = null, string value6 = null, string value7 = null, string value8 = null, string value9 = null, string value10 = null)
	{
		ColumnName = columnName;
		Value1 = value1;
		Value2 = value2;
		Value3 = value3;
		Value4 = value4;
		Value5 = value5;
		Value6 = value6;
		Value7 = value7;
		Value8 = value8;
		Value9 = value9;
		Value10 = value10;
	}
}
