using System;
using System.Drawing;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Properties;
using DevExpress.XtraEditors.DXErrorProvider;

namespace Dataedo.App.Tools;

[Serializable]
public class BulkTableModel : IDXDataErrorInfo
{
	public Image Image
	{
		get
		{
			if (!IsNotValid)
			{
				return Resources.ok_16;
			}
			return DXErrorProvider.GetErrorIconInternal(ErrorType.Critical);
		}
	}

	public bool ColumnEmptyName
	{
		get
		{
			return Column.IsNameEmpty;
		}
		set
		{
			Column.IsNameEmpty = value;
		}
	}

	public bool HasEmptyName { get; set; }

	public bool IsColumnDuplicated { get; set; }

	public string FullName
	{
		get
		{
			if (!string.IsNullOrEmpty(Schema))
			{
				return Schema + "." + TableName;
			}
			return TableName;
		}
	}

	public string TableName { get; set; }

	public string Schema { get; set; }

	public string ColumnName
	{
		get
		{
			return Column.Name;
		}
		set
		{
			Column.Name = value;
		}
	}

	public string DataType
	{
		get
		{
			return Column.DataType;
		}
		set
		{
			Column.DataType = value;
			Column.DataTypeWithoutLength = value;
		}
	}

	public string Size
	{
		get
		{
			return Column.DataLength;
		}
		set
		{
			Column.DataLength = value;
		}
	}

	public bool Nullable
	{
		get
		{
			return Column.Nullable;
		}
		set
		{
			Column.Nullable = value;
		}
	}

	public string DefaultValue
	{
		get
		{
			return Column.DefaultValue;
		}
		set
		{
			Column.DefaultValue = value;
		}
	}

	public string ComputedFormula
	{
		get
		{
			return Column.ComputedFormula;
		}
		set
		{
			Column.ComputedFormula = value;
		}
	}

	public bool Identity
	{
		get
		{
			return Column.IsIdentity;
		}
		set
		{
			Column.IsIdentity = value;
		}
	}

	public string ColumnTitle
	{
		get
		{
			return Column.Title;
		}
		set
		{
			Column.Title = value;
		}
	}

	public string Description
	{
		get
		{
			return Column.Description;
		}
		set
		{
			Column.Description = value;
		}
	}

	public bool IsNotValid
	{
		get
		{
			if (!HasEmptyName && !ColumnEmptyName)
			{
				return IsColumnDuplicated;
			}
			return true;
		}
	}

	public ColumnRow Column { get; set; }

	public BulkTableModel()
	{
		Column = new ColumnRow();
	}

	public void CheckIfNameIsEmpty()
	{
		HasEmptyName = string.IsNullOrEmpty(TableName);
		ColumnEmptyName = string.IsNullOrEmpty(ColumnName);
	}

	public void GetPropertyError(string propertyName, ErrorInfo info)
	{
		if (propertyName == "TableName" && HasEmptyName)
		{
			info.ErrorText = "Table name can not be empty";
			info.ErrorType = ErrorType.Critical;
		}
		if (propertyName == "ColumnName" && ColumnEmptyName)
		{
			info.ErrorText = "Column name can not be empty";
			info.ErrorType = ErrorType.Critical;
		}
	}

	public void GetError(ErrorInfo info)
	{
	}
}
