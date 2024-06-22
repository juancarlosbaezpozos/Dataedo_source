using Dataedo.App.UserControls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;

namespace Dataedo.App.ImportDescriptions.Tools.Fields;

public class DescriptionFieldDefinition : FieldDefinition
{
	public override string FieldName => "Description";

	public override string DisplayName => "Description";

	public override RepositoryItemTextEdit GetEdit()
	{
		return new RepositoryItemAutoHeightMemoEdit
		{
			AutoHeight = true
		};
	}

	public override GridColumn GetGridColumn(ref int visibleIndex)
	{
		GridColumn gridColumn = new GridColumn();
		gridColumn.Caption = DisplayName;
		gridColumn.FieldName = FieldName + ".OverwriteValue";
		gridColumn.Visible = true;
		gridColumn.VisibleIndex = ++visibleIndex;
		gridColumn.MinWidth = 100;
		gridColumn.MaxWidth = 1000;
		gridColumn.ColumnEdit = GetEdit();
		gridColumn.OptionsColumn.AllowShowHide = false;
		return gridColumn;
	}
}
