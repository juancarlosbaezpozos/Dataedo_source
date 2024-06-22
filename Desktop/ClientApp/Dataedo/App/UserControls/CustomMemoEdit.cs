using System.ComponentModel;
using DevExpress.XtraEditors;

namespace Dataedo.App.UserControls;

[ToolboxItem(true)]
public class CustomMemoEdit : MemoEdit
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemAutoHeightMemoEdit Properties => base.Properties as RepositoryItemAutoHeightMemoEdit;

	public override string EditorTypeName => "AutoHeightMemoEdit";

	static CustomMemoEdit()
	{
		RepositoryItemAutoHeightMemoEdit.RegisterAutoHeightMemoEdit();
	}
}
