using DevExpress.XtraEditors.DXErrorProvider;

namespace Dataedo.App.Classes.Synchronize;

public class ImportExtendedProperyRow : IDXDataErrorInfo
{
	private bool isSelected;

	private string extendedProperty;

	private DocumentationCustomFieldRow selectedCustomField;

	public bool AlreadyInUse { get; set; }

	public bool IsCustomFieldEmpty { get; set; }

	public bool IsSelected
	{
		get
		{
			return isSelected;
		}
		set
		{
			isSelected = value;
			if (SelectedCustomField != null)
			{
				SelectedCustomField.IsSelected = value;
			}
		}
	}

	public string ExtendedProperty
	{
		get
		{
			return extendedProperty;
		}
		set
		{
			extendedProperty = value;
			if (SelectedCustomField != null)
			{
				SelectedCustomField.ExtendedProperty = value;
			}
		}
	}

	public DocumentationCustomFieldRow SelectedCustomField
	{
		get
		{
			return selectedCustomField;
		}
		set
		{
			selectedCustomField = value;
			if (selectedCustomField != null)
			{
				selectedCustomField.ExtendedProperty = extendedProperty;
				selectedCustomField.IsSelected = isSelected;
			}
		}
	}

	public int? CustomFieldId { get; set; }

	public string Title => SelectedCustomField?.Title;

	public void GetPropertyError(string propertyName, ErrorInfo info)
	{
		if (!(propertyName != "CustomFieldId"))
		{
			if (AlreadyInUse)
			{
				info.ErrorText = "Custom field is already in use";
				info.ErrorType = ErrorType.Critical;
			}
			else if (IsSelected && IsCustomFieldEmpty)
			{
				info.ErrorText = "Custom field can not be empty";
				info.ErrorType = ErrorType.Critical;
			}
		}
	}

	public void GetError(ErrorInfo info)
	{
	}
}
