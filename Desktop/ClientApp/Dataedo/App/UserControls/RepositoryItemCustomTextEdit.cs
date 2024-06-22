using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.UserControls;

[UserRepositoryItem("RegisterCustomTextEdit")]
public class RepositoryItemCustomTextEdit : RepositoryItemTextEdit
{
	public const string CustomEditName = "CustomTextEdit";

	public override string EditorTypeName => "CustomTextEdit";

	static RepositoryItemCustomTextEdit()
	{
		RegisterCustomTextEdit();
	}

	public RepositoryItemCustomTextEdit()
	{
		base.KeyDown += RepositoryItemCustomTextEdit_KeyDown;
	}

	public static void RegisterCustomTextEdit()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("CustomTextEdit", typeof(CustomTextEdit), typeof(RepositoryItemCustomTextEdit), typeof(CustomTextEditViewInfo), new CustomTextEditPainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemCustomTextEdit;
		}
		finally
		{
			EndUpdate();
		}
	}

	private TextEdit GetOwnerEdit(object sender)
	{
		BaseEdit baseEdit = null;
		if (sender is RepositoryItem)
		{
			baseEdit = ((RepositoryItem)sender).OwnerEdit;
		}
		if (sender is BaseEdit)
		{
			baseEdit = (BaseEdit)sender;
		}
		return baseEdit as TextEdit;
	}

	private GridView GetGridView(object sender)
	{
		return (GetOwnerEdit(sender).Parent as GridControl).MainView as GridView;
	}

	private void RepositoryItemCustomTextEdit_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return && !e.Shift)
		{
			GetGridView(sender).CloseEditor();
			e.Handled = true;
		}
		else if (e.KeyCode == Keys.Up)
		{
			TextEdit ownerEdit = GetOwnerEdit(sender);
			if (!string.IsNullOrEmpty(ownerEdit.Text) && ownerEdit.SelectionLength == ownerEdit.Text.Length)
			{
				ownerEdit.SelectionStart = 0;
				ownerEdit.SelectionLength = 0;
				e.Handled = true;
				return;
			}
			GridView gridView = GetGridView(sender);
			gridView.CloseEditor();
			gridView.UnselectRow(gridView.FocusedRowHandle);
			gridView.FocusedRowHandle--;
			gridView.SelectRow(gridView.FocusedRowHandle);
			e.Handled = true;
		}
		else if (e.KeyCode == Keys.Left)
		{
			TextEdit ownerEdit2 = GetOwnerEdit(sender);
			if (!string.IsNullOrEmpty(ownerEdit2.Text) && ownerEdit2.SelectionLength == ownerEdit2.Text.Length)
			{
				ownerEdit2.SelectionStart = 0;
				ownerEdit2.SelectionLength = 0;
				e.Handled = true;
			}
		}
		else if (e.KeyCode == Keys.Down)
		{
			TextEdit ownerEdit3 = GetOwnerEdit(sender);
			if (!string.IsNullOrEmpty(ownerEdit3.Text) && ownerEdit3.SelectionLength == ownerEdit3.Text.Length)
			{
				ownerEdit3.SelectionStart = ownerEdit3.Text.Length;
				ownerEdit3.SelectionLength = 0;
				e.Handled = true;
				return;
			}
			GridView gridView2 = GetGridView(sender);
			gridView2.CloseEditor();
			gridView2.UnselectRow(gridView2.FocusedRowHandle);
			gridView2.FocusedRowHandle++;
			gridView2.SelectRow(gridView2.FocusedRowHandle);
			e.Handled = true;
		}
		else if (e.KeyCode == Keys.Right)
		{
			TextEdit ownerEdit4 = GetOwnerEdit(sender);
			if (!string.IsNullOrEmpty(ownerEdit4.Text) && ownerEdit4.SelectionLength == ownerEdit4.Text.Length)
			{
				ownerEdit4.SelectionStart = ownerEdit4.Text.Length;
				ownerEdit4.SelectionLength = 0;
				e.Handled = true;
			}
		}
	}
}
