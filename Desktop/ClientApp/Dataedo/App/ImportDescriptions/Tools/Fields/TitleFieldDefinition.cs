using Dataedo.App.Tools;
using Dataedo.App.UserControls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;

namespace Dataedo.App.ImportDescriptions.Tools.Fields;

public class TitleFieldDefinition : FieldDefinition
{
	public override string FieldName => "Title";

	public override string DisplayName => "Title";

	public override RepositoryItemTextEdit GetEdit()
	{
		RepositoryItemCustomTextEdit obj = new RepositoryItemCustomTextEdit
		{
			AutoHeight = false
		};
		LengthValidation.SetTitleOrNameLengthLimit(obj);
		return obj;
	}

	public override GridColumn GetGridColumn(ref int visibleIndex)
	{
		GridColumn gridColumn = new GridColumn();
		gridColumn.Caption = DisplayName;
		gridColumn.FieldName = FieldName + ".OverwriteValue";
		gridColumn.Visible = true;
		gridColumn.VisibleIndex = ++visibleIndex;
		gridColumn.ColumnEdit = GetEdit();
		gridColumn.OptionsColumn.AllowShowHide = false;
		return gridColumn;
	}
}
