using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;

namespace Dataedo.App.ImportDescriptions.Tools.Fields;

public abstract class FieldDefinition
{
	public abstract string DisplayName { get; }

	public abstract string FieldName { get; }

	public bool IsSelected { get; set; }

	public FieldDefinition()
	{
	}

	public abstract GridColumn GetGridColumn(ref int visibleIndex);

	public abstract RepositoryItemTextEdit GetEdit();
}
