using System.Drawing;
using Dataedo.App.Properties;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Enums;

public class UniqueConstraintType
{
	public enum UniqueConstraintTypeEnum
	{
		NotSet = 0,
		PK_user = 1,
		PK_deleted = 2,
		PK = 3,
		PK_disabled = 4,
		UK_user = 5,
		UK_deleted = 6,
		UK = 7,
		UK_disabled = 8
	}

	public static bool IsUserDefined(UniqueConstraintTypeEnum type)
	{
		if (type == UniqueConstraintTypeEnum.PK_user || type == UniqueConstraintTypeEnum.UK_user)
		{
			return true;
		}
		return false;
	}

	public static int ToInt(UniqueConstraintTypeEnum type)
	{
		return type switch
		{
			UniqueConstraintTypeEnum.PK_user => 0, 
			UniqueConstraintTypeEnum.PK_deleted => 1, 
			UniqueConstraintTypeEnum.PK => 2, 
			UniqueConstraintTypeEnum.PK_disabled => 3, 
			UniqueConstraintTypeEnum.UK_user => 4, 
			UniqueConstraintTypeEnum.UK_deleted => 5, 
			UniqueConstraintTypeEnum.UK => 6, 
			UniqueConstraintTypeEnum.NotSet => -1, 
			_ => 7, 
		};
	}

	public static Bitmap GetSmallIcon(UniqueConstraintTypeEnum type)
	{
		return type switch
		{
			UniqueConstraintTypeEnum.PK_user => Resources.primary_key_user_16, 
			UniqueConstraintTypeEnum.PK_deleted => Resources.primary_key_deleted_16, 
			UniqueConstraintTypeEnum.PK => Resources.primary_key_16, 
			UniqueConstraintTypeEnum.PK_disabled => Resources.primary_key_disabled_16, 
			UniqueConstraintTypeEnum.UK_user => Resources.unique_key_user_16, 
			UniqueConstraintTypeEnum.UK_deleted => Resources.unique_key_deleted_16, 
			UniqueConstraintTypeEnum.UK => Resources.unique_key_16, 
			UniqueConstraintTypeEnum.UK_disabled => Resources.unique_key_disabled_16, 
			UniqueConstraintTypeEnum.NotSet => new Bitmap(16, 16), 
			_ => null, 
		};
	}

	public static string ToString(UniqueConstraintTypeEnum type)
	{
		return type switch
		{
			UniqueConstraintTypeEnum.PK => "PK", 
			UniqueConstraintTypeEnum.PK_disabled => "DisabledPK", 
			UniqueConstraintTypeEnum.PK_user => "UserDefinedPK", 
			UniqueConstraintTypeEnum.PK_deleted => "DeletedPK", 
			UniqueConstraintTypeEnum.UK => "UK", 
			UniqueConstraintTypeEnum.UK_disabled => "DisabledUK", 
			UniqueConstraintTypeEnum.UK_user => "UserDefinedUK", 
			UniqueConstraintTypeEnum.UK_deleted => "DeletedUK", 
			_ => "Unknown", 
		};
	}

	public static string TypeForXmlExport(UniqueConstraintTypeEnum type)
	{
		return type switch
		{
			UniqueConstraintTypeEnum.PK => "PK", 
			UniqueConstraintTypeEnum.PK_disabled => "DisabledPK", 
			UniqueConstraintTypeEnum.PK_user => "UserDefinedPK", 
			UniqueConstraintTypeEnum.PK_deleted => "DeletedPK", 
			UniqueConstraintTypeEnum.UK => "UK", 
			UniqueConstraintTypeEnum.UK_disabled => "DisabledUK", 
			UniqueConstraintTypeEnum.UK_user => "UserDefinedUK", 
			UniqueConstraintTypeEnum.UK_deleted => "DeletedUK", 
			_ => "", 
		};
	}

	public static UniqueConstraintTypeEnum ToType(int type)
	{
		return type switch
		{
			0 => UniqueConstraintTypeEnum.PK_user, 
			1 => UniqueConstraintTypeEnum.PK_deleted, 
			2 => UniqueConstraintTypeEnum.PK, 
			3 => UniqueConstraintTypeEnum.PK_disabled, 
			4 => UniqueConstraintTypeEnum.UK_user, 
			5 => UniqueConstraintTypeEnum.UK_deleted, 
			6 => UniqueConstraintTypeEnum.UK, 
			-1 => UniqueConstraintTypeEnum.NotSet, 
			_ => UniqueConstraintTypeEnum.UK_disabled, 
		};
	}

	public static UniqueConstraintTypeEnum SetType(UserTypeEnum.UserType? source, SynchronizeStateEnum.SynchronizeState? status, bool isPK, bool disabled)
	{
		if (source == UserTypeEnum.UserType.DBMS)
		{
			if (status == SynchronizeStateEnum.SynchronizeState.Deleted)
			{
				if (!isPK)
				{
					return UniqueConstraintTypeEnum.UK_deleted;
				}
				return UniqueConstraintTypeEnum.PK_deleted;
			}
			if (isPK)
			{
				if (!disabled)
				{
					return UniqueConstraintTypeEnum.PK;
				}
				return UniqueConstraintTypeEnum.PK_disabled;
			}
			if (!disabled)
			{
				return UniqueConstraintTypeEnum.UK;
			}
			return UniqueConstraintTypeEnum.UK_disabled;
		}
		if (!isPK)
		{
			return UniqueConstraintTypeEnum.UK_user;
		}
		return UniqueConstraintTypeEnum.PK_user;
	}
}
