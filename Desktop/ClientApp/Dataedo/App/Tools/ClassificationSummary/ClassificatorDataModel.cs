using DevExpress.Utils;

namespace Dataedo.App.Tools.ClassificationSummary;

public class ClassificatorDataModel
{
	public string Documentation { get; set; }

	public string Table { get; set; }

	public string Column { get; set; }

	public string ColumnFormatted { get; set; }

	public string ColumnName => Column?.ToLower();

	public string DataType { get; set; }

	public int ColumnId { get; set; }

	public string Description { get; set; }

	public string ShortDescription => GetShortDescription();

	public string Field1Value { get; set; }

	public string Field1Update { get; set; }

	public string Field1DBName { get; set; }

	public string Field2Value { get; set; }

	public string Field2Update { get; set; }

	public string Field2DBName { get; set; }

	public string Field3Value { get; set; }

	public string Field3Update { get; set; }

	public string Field3DBName { get; set; }

	public string Field4Value { get; set; }

	public string Field4Update { get; set; }

	public string Field4DBName { get; set; }

	public string Field5Value { get; set; }

	public string Field5Update { get; set; }

	public string Field5DBName { get; set; }

	public string Comment { get; set; }

	public bool IsChecked { get; set; }

	public bool IsProcessed { get; set; }

	public int DatabaseId { get; set; }

	public bool IsUnprocessed
	{
		get
		{
			if ((!string.IsNullOrEmpty(Field1Value) || string.IsNullOrEmpty(Field1Update)) && (!string.IsNullOrEmpty(Field2Value) || string.IsNullOrEmpty(Field2Update)) && (!string.IsNullOrEmpty(Field3Value) || string.IsNullOrEmpty(Field3Update)) && (!string.IsNullOrEmpty(Field4Value) || string.IsNullOrEmpty(Field4Update)))
			{
				if (string.IsNullOrEmpty(Field5Value))
				{
					return !string.IsNullOrEmpty(Field5Update);
				}
				return false;
			}
			return true;
		}
	}

	public bool AnythingToSave
	{
		get
		{
			if ((string.IsNullOrEmpty(Field1Update) || Field1Update.Equals(Field1Value)) && (string.IsNullOrEmpty(Field2Update) || Field2Update.Equals(Field2Value)) && (string.IsNullOrEmpty(Field3Update) || Field3Update.Equals(Field3Value)) && (string.IsNullOrEmpty(Field4Update) || Field4Update.Equals(Field4Value)))
			{
				if (!string.IsNullOrEmpty(Field5Update))
				{
					return !Field5Update.Equals(Field5Value);
				}
				return false;
			}
			return true;
		}
	}

	public ClassificatorDataModel GetCopy()
	{
		return (ClassificatorDataModel)MemberwiseClone();
	}

	public bool IsFieldForSaving(int i)
	{
		Guard.ArgumentIsInRange(1, 5, i, "Classification Custom Field number");
		if (!string.IsNullOrEmpty(GetFieldDBName(i)) && !string.IsNullOrEmpty(GetFieldUpdateValue(i)))
		{
			return !GetFieldUpdateValue(i).Equals(GetFieldValue(i));
		}
		return false;
	}

	public string GetFieldDBName(int i)
	{
		Guard.ArgumentIsInRange(1, 5, i, "Classification Custom Field number");
		return i switch
		{
			1 => Field1DBName, 
			2 => Field2DBName, 
			3 => Field3DBName, 
			4 => Field4DBName, 
			5 => Field5DBName, 
			_ => null, 
		};
	}

	public string GetFieldUpdateValue(int i)
	{
		Guard.ArgumentIsInRange(1, 5, i, "Classification Custom Field number");
		return i switch
		{
			1 => Field1Update, 
			2 => Field2Update, 
			3 => Field3Update, 
			4 => Field4Update, 
			5 => Field5Update, 
			_ => null, 
		};
	}

	public string GetFieldValue(int i)
	{
		Guard.ArgumentIsInRange(1, 5, i, "Classification Custom Field number");
		return i switch
		{
			1 => Field1Value, 
			2 => Field2Value, 
			3 => Field3Value, 
			4 => Field4Value, 
			5 => Field5Value, 
			_ => null, 
		};
	}

	private string GetShortDescription()
	{
		string description = Description;
		string text = ((description == null || description.Length < 50) ? Description : Description?.Substring(0, 49));
		if (string.IsNullOrEmpty(text))
		{
			return text;
		}
		return text += "...";
	}

	public void SavingCompleted()
	{
		IsChecked = false;
		Field1Value = Field1Update;
		Field2Value = Field2Update;
		Field3Value = Field3Update;
		Field4Value = Field4Update;
		Field5Value = Field5Update;
	}
}
