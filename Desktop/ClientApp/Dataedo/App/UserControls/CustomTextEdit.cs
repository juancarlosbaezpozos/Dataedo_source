using System.ComponentModel;
using DevExpress.XtraEditors;

namespace Dataedo.App.UserControls;

[ToolboxItem(true)]
public class CustomTextEdit : TextEdit
{
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	public new RepositoryItemCustomTextEdit Properties => base.Properties as RepositoryItemCustomTextEdit;

	public override string EditorTypeName => "CustomTextEdit";

	static CustomTextEdit()
	{
		RepositoryItemCustomTextEdit.RegisterCustomTextEdit();
	}
}
