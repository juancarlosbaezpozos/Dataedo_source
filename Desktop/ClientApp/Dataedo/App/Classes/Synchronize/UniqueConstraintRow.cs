using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.App.Tools.CustomFields;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Tables.Constraints;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class UniqueConstraintRow : RelationContraintRow
{
	private string _tableSchema;

	public bool IsReady
	{
		get
		{
			if (base.RowState == ManagingRowsEnum.ManagingRows.Unchanged || base.RowState == ManagingRowsEnum.ManagingRows.Deleted || (ConstraintType == UniqueConstraintType.UniqueConstraintTypeEnum.NotSet && string.IsNullOrEmpty(base.Name) && string.IsNullOrEmpty(base.Description) && (Columns.Count == 0 || Columns.All((UniqueConstraintColumnRow c) => c.ColumnId == -1))) || ConstraintType != 0)
			{
				return true;
			}
			return false;
		}
	}

	public bool IsReadyNotEmpty
	{
		get
		{
			if (IsReady)
			{
				return !IsEmpty;
			}
			return false;
		}
	}

	public bool IsEmpty
	{
		get
		{
			if (ConstraintType == UniqueConstraintType.UniqueConstraintTypeEnum.NotSet && string.IsNullOrEmpty(base.Name) && string.IsNullOrEmpty(base.Description))
			{
				if (Columns.Count != 0)
				{
					return Columns.All((UniqueConstraintColumnRow c) => c.ColumnId == -1);
				}
				return true;
			}
			return false;
		}
	}

	public Color RowColoring
	{
		get
		{
			if (!IsReady)
			{
				return Color.FromArgb(150, 255, 255, 224);
			}
			return Color.White;
		}
	}

	public string NotEmptyName
	{
		get
		{
			if (!string.IsNullOrEmpty(base.Name))
			{
				return base.Name;
			}
			if (ConstraintType == UniqueConstraintType.UniqueConstraintTypeEnum.PK_user)
			{
				return "User-defined primary key";
			}
			if (ConstraintType == UniqueConstraintType.UniqueConstraintTypeEnum.UK_user)
			{
				return "User-defined unique key";
			}
			return string.Empty;
		}
	}

	public int TableId { get; set; }

	public bool IsPK { get; set; }

	public string DatabaseName { get; set; }

	public string TableName { get; set; }

	public string TableSchema
	{
		get
		{
			return _tableSchema;
		}
		set
		{
			_tableSchema = ((value == null) ? string.Empty : value);
		}
	}

	public string Type { get; set; }

	public BindingList<UniqueConstraintColumnRow> Columns { get; set; }

	public string ColumnsStringFormatted
	{
		get
		{
			IEnumerable<string> values = from x in Columns
				where !string.IsNullOrWhiteSpace(x.ColumnName)
				select x.ColumnFullNameFormatted;
			return string.Join(", ", values);
		}
	}

	public string ColumnsStringFormattedWithTitles
	{
		get
		{
			IEnumerable<string> values = from x in Columns
				where !string.IsNullOrWhiteSpace(x.ColumnName)
				select x.ColumnFullNameFormattedWithTitle;
			return string.Join(", ", values);
		}
	}

	public string ColumnsString
	{
		get
		{
			IEnumerable<string> values = from x in Columns
				where !string.IsNullOrWhiteSpace(x.ColumnName)
				select x.ColumnFullName;
			return string.Join(", ", values);
		}
	}

	public bool Disabled { get; set; }

	private int uniqueConstraintTypeInt { get; set; }

	public int UniqueConstraintTypeInt
	{
		get
		{
			return uniqueConstraintTypeInt;
		}
		set
		{
			uniqueConstraintTypeInt = value;
			ConstraintType = UniqueConstraintType.ToType(uniqueConstraintTypeInt);
			IsPK = ConstraintType == UniqueConstraintType.UniqueConstraintTypeEnum.PK || ConstraintType == UniqueConstraintType.UniqueConstraintTypeEnum.PK_deleted || ConstraintType == UniqueConstraintType.UniqueConstraintTypeEnum.PK_user;
			Disabled = ConstraintType == UniqueConstraintType.UniqueConstraintTypeEnum.PK_disabled || ConstraintType == UniqueConstraintType.UniqueConstraintTypeEnum.UK_disabled;
			base.Source = ((ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.PK_user && ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.UK_user && ConstraintType != 0) ? UserTypeEnum.UserType.DBMS : UserTypeEnum.UserType.USER);
		}
	}

	public Image ImageFile => UniqueConstraintTypeInt switch
	{
		0 => Resources.primary_key_user_16, 
		1 => Resources.primary_key_deleted_16, 
		2 => Resources.primary_key_16, 
		3 => Resources.primary_key_disabled_16, 
		4 => Resources.unique_key_user_16, 
		5 => Resources.unique_key_deleted_16, 
		6 => Resources.unique_key_16, 
		7 => Resources.unique_key_disabled_16, 
		_ => Resources.blank_16, 
	};

	public UniqueConstraintType.UniqueConstraintTypeEnum ConstraintType { get; set; }

	public UniqueConstraintRow()
	{
		Columns = new BindingList<UniqueConstraintColumnRow>();
		base.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
	}

	public UniqueConstraintRow(UniqueConstraintSynchronizationObject data, CustomFieldsSupport customFieldsSupport)
	{
		base.Name = data.Name;
		DatabaseName = data.DatabaseName;
		TableName = data.TableName;
		TableSchema = data.TableSchema;
		Type = data.Type;
		IsPK = data.IsPK;
		base.Description = data.Description;
		Columns = new BindingList<UniqueConstraintColumnRow>();
		base.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
		Disabled = data.Disabled;
		base.CustomFields = new CustomFieldContainer(customFieldsSupport);
		base.CustomFields.RetrieveCustomFields(data);
	}

	public UniqueConstraintRow(UniqueConstraintObject row, CustomFieldsSupport customFieldsSupport)
		: base(row, isEditingUserObjectsEnabled: true)
	{
		base.Id = row.UniqueConstraintId;
		TableId = row.TableId;
		base.Name = row.Name;
		base.Description = row.Description;
		IsPK = row.PrimaryKey;
		Disabled = row.Disabled.GetValueOrDefault();
		ConstraintType = UniqueConstraintType.SetType(base.Source, base.Status, IsPK, Disabled);
		UniqueConstraintTypeInt = UniqueConstraintType.ToInt(ConstraintType);
		base.Source = UserTypeEnum.ObjectToType(row.Source) ?? UserTypeEnum.UserType.DBMS;
		base.CustomFields = new CustomFieldContainer(SharedObjectTypeEnum.ObjectType.Key, base.Id, customFieldsSupport);
		base.CustomFields.RetrieveCustomFields(row);
	}

	public UniqueConstraintRow(UniqueConstraintWithColumnObject row, CustomFieldsSupport customFieldsSupport)
		: base(row, isEditingUserObjectsEnabled: true)
	{
		base.Id = row.UniqueConstraintId;
		TableId = row.TableId;
		base.Name = row.Name;
		base.Description = row.Description;
		IsPK = row.PrimaryKey;
		Disabled = row.Disabled.GetValueOrDefault();
		base.Source = UserTypeEnum.ObjectToType(row.Source) ?? UserTypeEnum.UserType.DBMS;
		ConstraintType = UniqueConstraintType.SetType(base.Source, base.Status, IsPK, Disabled);
		UniqueConstraintTypeInt = UniqueConstraintType.ToInt(ConstraintType);
		base.CustomFields = new CustomFieldContainer(SharedObjectTypeEnum.ObjectType.Key, base.Id, customFieldsSupport);
		base.CustomFields.RetrieveCustomFields(row);
		Columns = new BindingList<UniqueConstraintColumnRow>
		{
			new UniqueConstraintColumnRow(row)
		};
	}

	public UniqueConstraintRow(int tableId, CustomFieldsSupport customFieldsSupport)
		: base(isEditingUserObjectsEnabled: true)
	{
		base.RowState = ManagingRowsEnum.ManagingRows.ForAdding;
		TableId = tableId;
		UniqueConstraintTypeInt = UniqueConstraintType.ToInt(UniqueConstraintType.UniqueConstraintTypeEnum.NotSet);
		Columns = new BindingList<UniqueConstraintColumnRow>();
		base.CustomFields = new CustomFieldContainer(customFieldsSupport);
		base.CustomFields.RetrieveCustomFields();
	}

	public void Clear()
	{
		Columns.Clear();
		base.Name = string.Empty;
		base.Title = string.Empty;
		base.Description = string.Empty;
		UniqueConstraintTypeInt = UniqueConstraintType.ToInt(UniqueConstraintType.UniqueConstraintTypeEnum.NotSet);
	}

	public override bool CanBeDeleted()
	{
		if (!base.IsEditable || base.Source != UserTypeEnum.UserType.USER || IsEmpty)
		{
			if (!base.IsEditable)
			{
				return base.Status == SynchronizeStateEnum.SynchronizeState.Deleted;
			}
			return false;
		}
		return true;
	}

	public override bool IsDeletable()
	{
		if (base.Status != SynchronizeStateEnum.SynchronizeState.Deleted)
		{
			return base.Source == UserTypeEnum.UserType.USER;
		}
		return true;
	}
}
