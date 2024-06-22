using System.Drawing;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.Model.Data.Erd;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Model.Data.Tables.Relations;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class ColumnUniqueConstraintWithFkData : ColumnUniqueConstraintData
{
	public override bool HasData
	{
		get
		{
			if (!base.HasData)
			{
				return PkColumnID.HasValue;
			}
			return true;
		}
	}

	public int? PkColumnID { get; set; }

	public UserTypeEnum.UserType? RelationSource { get; set; }

	public bool IsFk => PkColumnID.HasValue;

	public override Image ObjectImage
	{
		get
		{
			if (!base.IsPk && IsFk)
			{
				if (!IsFk)
				{
					return new Bitmap(16, 16);
				}
				if (RelationSource != UserTypeEnum.UserType.DBMS)
				{
					return Resources.relation_1x_mx_user_16;
				}
				return Resources.relation_1x_mx_16;
			}
			return base.ObjectImage;
		}
	}

	public ColumnUniqueConstraintWithFkData(int? id, UserTypeEnum.UserType? source, bool isPk, SynchronizeStateEnum.SynchronizeState status, bool disabled, UniqueConstraintType.UniqueConstraintTypeEnum constraintType, int? pkColumnId, UserTypeEnum.UserType? relationSource)
		: base(id, source, isPk, status, disabled, constraintType)
	{
		PkColumnID = pkColumnId;
		RelationSource = relationSource;
	}

	public ColumnUniqueConstraintWithFkData(int? pkColumnId)
		: base(null, null, isPk: false, SynchronizeStateEnum.SynchronizeState.Unknown, disabled: false, UniqueConstraintType.UniqueConstraintTypeEnum.NotSet)
	{
		PkColumnID = pkColumnId;
		RelationSource = UserTypeEnum.UserType.USER;
	}

	public ColumnUniqueConstraintWithFkData(ColumnDocObject row, string dataPropertyPrefix = "")
		: base(row, dataPropertyPrefix)
	{
		PkColumnID = row.PkColumnId;
		RelationSource = UserTypeEnum.ObjectToType(row.RelationSource);
	}

	public ColumnUniqueConstraintWithFkData(RelationWithUniqueConstraintsObject row, int? pkColumnID, UserTypeEnum.UserType? relationSource, bool isPk)
		: base(row, isPk)
	{
		PkColumnID = pkColumnID;
		RelationSource = relationSource;
	}

	public ColumnUniqueConstraintWithFkData(ErdNodeColumnObject row, string dataPropertyPrefix = "")
		: base(row, dataPropertyPrefix)
	{
		PkColumnID = row.PkColumnId;
		RelationSource = UserTypeEnum.ObjectToType(row.RelationSource);
	}
}
