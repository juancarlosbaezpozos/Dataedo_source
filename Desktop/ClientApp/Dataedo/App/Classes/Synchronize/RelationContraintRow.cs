using Dataedo.Model.Data.Common.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class RelationContraintRow : BasicRow
{
	public string SourceString
	{
		get
		{
			if (base.Source != UserTypeEnum.UserType.USER)
			{
				return string.Empty;
			}
			return "user";
		}
	}

	public bool IsEditingUserObjectsEnabled { get; set; }

	public bool IsEditable
	{
		get
		{
			if (base.Source != UserTypeEnum.UserType.DBMS)
			{
				return IsEditingUserObjectsEnabled;
			}
			return false;
		}
	}

	public bool ShowEditRemoveButton => base.Source != UserTypeEnum.UserType.DBMS;

	public RelationContraintRow()
	{
	}

	public RelationContraintRow(IStatus row)
		: base(row)
	{
	}

	public RelationContraintRow(IStatus row, bool isEditingUserObjectsEnabled)
		: this(row)
	{
		IsEditingUserObjectsEnabled = isEditingUserObjectsEnabled;
	}

	public RelationContraintRow(bool isEditingUserObjectsEnabled)
	{
		IsEditingUserObjectsEnabled = isEditingUserObjectsEnabled;
	}
}
