using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;

namespace Dataedo.App.UserControls;

[UserRepositoryItem("RegisterAutoHeightMemoEdit")]
public class RepositoryItemAutoHeightMemoEdit : RepositoryItemMemoEdit
{
	public const string CustomEditName = "AutoHeightMemoEdit";

	public override string EditorTypeName => "AutoHeightMemoEdit";

	static RepositoryItemAutoHeightMemoEdit()
	{
		RegisterAutoHeightMemoEdit();
	}

	public RepositoryItemAutoHeightMemoEdit()
	{
		base.EditValueChanged += RepositoryItemAutoHeightMemoEdit_EditValueChanged;
		base.KeyDown += RepositoryItemAutoHeightMemoEdit_KeyDown;
		ScrollBars = ScrollBars.None;
		base.MaskBoxPadding = new Padding(0, 2, 0, 2);
	}

	private void RepositoryItemAutoHeightMemoEdit_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return && !e.Shift)
		{
			GetGridView(sender)?.CloseEditor();
			e.Handled = true;
		}
		else if (e.KeyCode == Keys.Left)
		{
			MemoEdit ownerEdit = GetOwnerEdit(sender);
			if (!string.IsNullOrEmpty(ownerEdit.Text) && ownerEdit.SelectionLength == ownerEdit.Text.Length)
			{
				ownerEdit.SelectionStart = 0;
				ownerEdit.SelectionLength = 0;
				e.Handled = true;
			}
		}
		else if (e.KeyCode == Keys.Right)
		{
			MemoEdit ownerEdit2 = GetOwnerEdit(sender);
			if (!string.IsNullOrEmpty(ownerEdit2.Text) && ownerEdit2.SelectionLength == ownerEdit2.Text.Length)
			{
				ownerEdit2.SelectionStart = ownerEdit2.Text.Length;
				ownerEdit2.SelectionLength = 0;
				e.Handled = true;
			}
		}
		else if (e.KeyCode == Keys.Up)
		{
			MemoEdit ownerEdit3 = GetOwnerEdit(sender);
			if (!string.IsNullOrEmpty(ownerEdit3.Text) && ownerEdit3.SelectionLength == ownerEdit3.Text.Length)
			{
				ownerEdit3.SelectionStart = 0;
				ownerEdit3.SelectionLength = 0;
				e.Handled = true;
			}
			else if (string.IsNullOrEmpty(ownerEdit3.Text) || GetCursorLine(sender) == 0)
			{
				GridView gridView = GetGridView(sender);
				if (gridView != null)
				{
					gridView.CloseEditor();
					gridView.UnselectRow(gridView.FocusedRowHandle);
					gridView.FocusedRowHandle--;
					gridView.SelectRow(gridView.FocusedRowHandle);
				}
				e.Handled = true;
			}
		}
		else
		{
			if (e.KeyCode != Keys.Down)
			{
				return;
			}
			MemoEdit ownerEdit4 = GetOwnerEdit(sender);
			if (!string.IsNullOrEmpty(ownerEdit4.Text) && ownerEdit4.SelectionLength == ownerEdit4.Text.Length)
			{
				ownerEdit4.SelectionStart = ownerEdit4.Text.Length;
				ownerEdit4.SelectionLength = 0;
				e.Handled = true;
			}
			else if (string.IsNullOrEmpty(ownerEdit4.Text) || GetCursorLine(sender) == GetOwnerEdit(sender).Lines.Count() - 1)
			{
				GridView gridView2 = GetGridView(sender);
				if (gridView2 != null)
				{
					gridView2.CloseEditor();
					gridView2.UnselectRow(gridView2.FocusedRowHandle);
					gridView2.FocusedRowHandle++;
					gridView2.SelectRow(gridView2.FocusedRowHandle);
				}
				e.Handled = true;
			}
		}
	}

	private TextBox FindTextBoxMaskBox(MemoEdit edit)
	{
		for (int i = 0; i < edit.Controls.Count; i++)
		{
			if (edit.Controls[i] is TextBox result)
			{
				return result;
			}
		}
		return null;
	}

	private int GetCursorLine(object sender)
	{
		MemoEdit ownerEdit = GetOwnerEdit(sender);
		return FindTextBoxMaskBox(ownerEdit).GetLineFromCharIndex(ownerEdit.SelectionStart);
	}

	private MemoEdit GetOwnerEdit(object sender)
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
		return baseEdit as MemoEdit;
	}

	private GridView GetGridView(object sender)
	{
		return (GetOwnerEdit(sender).Parent as GridControl)?.MainView as GridView;
	}

	public static void RegisterAutoHeightMemoEdit()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("AutoHeightMemoEdit", typeof(CustomMemoEdit), typeof(RepositoryItemAutoHeightMemoEdit), typeof(MemoEditViewInfo), new MemoEditPainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemAutoHeightMemoEdit;
		}
		finally
		{
			EndUpdate();
		}
	}

	private void RepositoryItemAutoHeightMemoEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (!(sender is CustomMemoEdit customMemoEdit))
		{
			return;
		}
		if (!(customMemoEdit.Parent is TreeList))
		{
			GridControl gridControl = GetGridView(sender)?.GridControl;
			if (gridControl == null || gridControl.Parent?.Parent is CustomFieldControl)
			{
				return;
			}
		}
		int num = customMemoEdit.CalcAutoHeight();
		if (num >= customMemoEdit.Bounds.Height)
		{
			customMemoEdit.Height = num + base.MaskBoxPadding.Top + base.MaskBoxPadding.Bottom;
		}
	}
}
