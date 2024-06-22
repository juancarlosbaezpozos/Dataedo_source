using Dataedo.App.Tools.ExtendedPropertiesExport;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.LicenseHelperLibrary.Repository;
using Dataedo.Model.Data.CustomFields;
using Dataedo.Model.Data.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors.DXErrorProvider;

namespace Dataedo.App.Classes.Synchronize;

public class DocumentationCustomFieldRow : IExtendedProperty, ISelectableWithTitle, ISelectable, IDXDataErrorInfo
{
	private string extendedProperty;

	public bool IsSelected { get; set; }

	public int? DocumentationCustomFieldId { get; set; }

	public int CustomFieldId { get; set; }

	public int? DatabaseId { get; set; }

	public int OrdinalPosition { get; set; }

	public string FieldName { get; set; }

	public string Title { get; set; }

	public CustomFieldTypeEnum.CustomFieldType? Type { get; set; }

	public bool HasEmptyExtendedPropertyField { get; set; }

	public string ExtendedProperty
	{
		get
		{
			return extendedProperty;
		}
		set
		{
			extendedProperty = value;
			ExtendedPropertyForQueries = LicenseHelper.EscapeInvalidCharacters(value);
		}
	}

	public string ExtendedPropertyForQueries { get; private set; }

	public CustomFieldRowExtended CustomField { get; set; }

	public DocumentationCustomFieldRow()
	{
	}

	public DocumentationCustomFieldRow(DocumentationCustomFieldObject row, int? databaseId)
	{
		DocumentationCustomFieldId = row.DocumentationCustomFieldId;
		CustomFieldId = row.CustomFieldId;
		DatabaseId = databaseId;
		OrdinalPosition = row.OrdinalPosition;
		FieldName = row.FieldName;
		Title = row.Title;
		ExtendedProperty = row.ExtendedProperty;
		IsSelected = !string.IsNullOrEmpty(ExtendedProperty);
		Type = CustomFieldTypeEnum.StringToType(row.Type);
	}

	public DocumentationCustomFieldRow GetCopy()
	{
		return (DocumentationCustomFieldRow)MemberwiseClone();
	}

	public void GetPropertyError(string propertyName, ErrorInfo info)
	{
		if (propertyName == "ExtendedProperty" && HasEmptyExtendedPropertyField)
		{
			info.ErrorText = "Value can not be empty.";
			info.ErrorType = ErrorType.Critical;
		}
	}

	public void GetError(ErrorInfo info)
	{
	}
}
