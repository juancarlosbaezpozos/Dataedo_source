using System.ComponentModel;
using DevExpress.XtraEditors;

namespace Dataedo.App.UserControls;

[ToolboxItem(true)]
public class BaseCustomMemoEdit : MemoEdit
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemBaseAutoHeightMemoEdit Properties => base.Properties as RepositoryItemBaseAutoHeightMemoEdit;

	public override string EditorTypeName => "BaseAutoHeightMemoEdit";

	static BaseCustomMemoEdit()
	{
		RepositoryItemBaseAutoHeightMemoEdit.RegisterBaseAutoHeightMemoEdit();
	}
}
