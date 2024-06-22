using Dataedo.App.Tools;
using Dataedo.App.UserControls;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Shared.Enums;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;

namespace Dataedo.App.ImportDescriptions.Tools.Fields;

public class CustomFieldFieldDefinition : FieldDefinition
{
	public CustomFieldRowExtended CustomField { get; set; }

	public override string FieldName => CustomField.FieldPropertyName;

	public override string DisplayName => CustomField.Title;

	public CustomFieldFieldDefinition(CustomFieldRowExtended customField)
	{
		CustomField = customField;
	}

	public override RepositoryItemTextEdit GetEdit()
	{
		RepositoryItemTextEdit repositoryItemTextEdit = ((CustomField.Type != CustomFieldTypeEnum.CustomFieldType.Text) ? ((RepositoryItemTextEdit)new RepositoryItemCustomTextEdit
		{
			AutoHeight = false
		}) : ((RepositoryItemTextEdit)new RepositoryItemAutoHeightMemoEdit
		{
			AutoHeight = true
		}));
		LengthValidation.SetCustomFieldLength(repositoryItemTextEdit);
		return repositoryItemTextEdit;
	}

	public override GridColumn GetGridColumn(ref int visibleIndex)
	{
		GridColumn gridColumn = new GridColumn();
		gridColumn.Caption = DisplayName;
		gridColumn.FieldName = FieldName + ".OverwriteValue";
		gridColumn.Visible = true;
		gridColumn.VisibleIndex = ++visibleIndex;
		gridColumn.Tag = CustomField;
		gridColumn.ColumnEdit = GetEdit();
		gridColumn.OptionsColumn.AllowShowHide = false;
		return gridColumn;
	}
}
