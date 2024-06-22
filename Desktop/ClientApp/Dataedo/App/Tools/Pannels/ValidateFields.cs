using System;
using System.Text;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.Tools.Pannels;

internal static class ValidateFields
{
	public static bool FocusIfIsStringNullOrEmpty(BaseEdit edit)
	{
		if (edit.Enabled && IsStringNullOrEmpty(edit))
		{
			edit.Focus();
			return true;
		}
		return false;
	}

	public static bool IsStringNullOrEmpty(BaseEdit edit)
	{
		if (edit == null)
		{
			return false;
		}
		string text = edit.EditValue as string;
		if (text != null || edit is TextEdit)
		{
			return string.IsNullOrEmpty(text);
		}
		return false;
	}

	public static bool IsFieldNotEmpty(object textObj)
	{
		string text = Convert.ToString(textObj);
		if (text != null && !string.IsNullOrEmpty(text.Trim()))
		{
			return true;
		}
		return false;
	}

	public static bool IsGridFieldNotEmptyRaiseError(GridView grid, int rowHandle, string fieldName, string communicate)
	{
		GridColumn column = grid.Columns[fieldName];
		if (!IsFieldNotEmpty(grid.GetRowCellValue(rowHandle, column)))
		{
			grid.SetColumnError(column, new StringBuilder().Append("The ").Append(communicate).Append(" is required!")
				.ToString());
			return false;
		}
		grid.ClearColumnErrors();
		return true;
	}

	public static bool IsEditNotEmptyRaiseError(TextEdit textEdit, DXErrorProvider errorProvider, string communicate = "title")
	{
		if (!IsFieldNotEmpty(textEdit.Text))
		{
			errorProvider.SetError(textEdit, "The " + communicate + " is required!");
			return false;
		}
		errorProvider.SetError(textEdit, string.Empty);
		return true;
	}

	public static bool IsEditNotEmptyRaiseError(LookUpEdit lookUpEdit, DXErrorProvider errorProvider, string communicate = "title")
	{
		if (!IsFieldNotEmpty(lookUpEdit.EditValue))
		{
			errorProvider.SetError(lookUpEdit, "The " + communicate + " is required!");
			return false;
		}
		errorProvider.SetError(lookUpEdit, string.Empty);
		return true;
	}

	public static bool ValidateEdit(TextEdit textEdit, DXErrorProvider errorProvider, string communicate, bool acceptEmptyValue = false)
	{
		if (acceptEmptyValue && !IsFieldNotEmpty(textEdit.Text))
		{
			return true;
		}
		return IsEditNotEmptyRaiseError(textEdit, errorProvider, communicate);
	}

	public static bool ValidateEdit(LookUpEdit lookUpEdit, DXErrorProvider errorProvider, string communicate, bool acceptEmptyValue = false)
	{
		if (acceptEmptyValue && !IsFieldNotEmpty(lookUpEdit.EditValue))
		{
			return true;
		}
		return IsEditNotEmptyRaiseError(lookUpEdit, errorProvider, communicate);
	}
}
