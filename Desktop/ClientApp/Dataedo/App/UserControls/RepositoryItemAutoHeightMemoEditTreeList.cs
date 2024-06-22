using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTreeList;

namespace Dataedo.App.UserControls;

[UserRepositoryItem("RepositoryItemAutoHeightMemoEditTreeList")]
public class RepositoryItemAutoHeightMemoEditTreeList : RepositoryItemMemoEdit
{
	public const string CustomEditName = "AutoHeightMemoEditTreeList";

	public override string EditorTypeName => "AutoHeightMemoEditTreeList";

	static RepositoryItemAutoHeightMemoEditTreeList()
	{
		RegisterAutoHeightMemoEdit();
	}

	public RepositoryItemAutoHeightMemoEditTreeList()
	{
		base.EditValueChanged += RepositoryItemAutoHeightMemoEdit_EditValueChanged;
		base.KeyDown += RepositoryItemAutoHeightMemoEdit_KeyDown;
		ScrollBars = ScrollBars.None;
	}

	private void RepositoryItemAutoHeightMemoEdit_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return && !e.Shift)
		{
			GetTreeList(sender).CloseEditor();
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
				TreeList treeList = GetTreeList(sender);
				treeList.CloseEditor();
				treeList.MovePrevVisible();
				e.Handled = true;
			}
		}
		else if (e.KeyCode == Keys.Down)
		{
			MemoEdit ownerEdit4 = GetOwnerEdit(sender);
			if (!string.IsNullOrEmpty(ownerEdit4.Text) && ownerEdit4.SelectionLength == ownerEdit4.Text.Length)
			{
				ownerEdit4.SelectionStart = ownerEdit4.Text.Length;
				ownerEdit4.SelectionLength = 0;
				e.Handled = true;
			}
			else if (string.IsNullOrEmpty(ownerEdit4.Text) || GetCursorLine(sender) == GetOwnerEdit(sender).Lines.Count() - 1)
			{
				TreeList treeList2 = GetTreeList(sender);
				treeList2.CloseEditor();
				treeList2.MoveNextVisible();
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

	private TreeList GetTreeList(object sender)
	{
		return GetOwnerEdit(sender).Parent as TreeList;
	}

	public static void RegisterAutoHeightMemoEdit()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("AutoHeightMemoEditTreeList", typeof(CustomMemoEditTreelist), typeof(RepositoryItemAutoHeightMemoEditTreeList), typeof(MemoEditViewInfo), new MemoEditPainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemAutoHeightMemoEditTreeList;
		}
		finally
		{
			EndUpdate();
		}
	}

	private void RepositoryItemAutoHeightMemoEdit_EditValueChanged(object sender, EventArgs e)
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
		if (baseEdit != null)
		{
			TreeList treeList = baseEdit.Parent as TreeList;
			MemoEdit obj = baseEdit as MemoEdit;
			int selectionStart = obj.SelectionStart;
			int selectionLength = obj.SelectionLength;
			treeList.PostEditor();
			treeList.ShowEditor();
			obj.SelectionStart = selectionStart;
			obj.SelectionLength = selectionLength;
		}
	}
}
