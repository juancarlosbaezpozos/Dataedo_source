using System.ComponentModel;
using DevExpress.XtraEditors;

namespace Dataedo.App.UserControls;

[ToolboxItem(true)]
public class CustomMemoEditTreelist : MemoEdit
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemAutoHeightMemoEditTreeList Properties => base.Properties as RepositoryItemAutoHeightMemoEditTreeList;

	public override string EditorTypeName => "AutoHeightMemoEditTreeList";

	static CustomMemoEditTreelist()
	{
		RepositoryItemAutoHeightMemoEditTreeList.RegisterAutoHeightMemoEdit();
	}
}
