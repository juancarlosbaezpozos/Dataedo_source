using System.Drawing;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.DataProcessing.Classes;
using Dataedo.Model.Data.Erd;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Model.Data.Tables.Relations;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class ColumnUniqueConstraintData
{
	public virtual bool HasData => Id.HasValue;

	public int? Id { get; set; }

	public UserTypeEnum.UserType? Source { get; set; }

	public bool IsPk { get; set; }

	public SynchronizeStateEnum.SynchronizeState Status { get; set; }

	public bool Disabled { get; set; }

	public UniqueConstraintType.UniqueConstraintTypeEnum ConstraintType { get; set; }

	public Image ReferenceImage
	{
		get
		{
			if (Source != UserTypeEnum.UserType.DBMS)
			{
				return Resources.relation_mx_1x_user_16;
			}
			return Resources.relation_mx_1x_16;
		}
	}

	public virtual Image ObjectImage => UniqueConstraintType.GetSmallIcon(ConstraintType);

	public virtual Image KeysObjectImage => UniqueConstraintType.GetSmallIcon(ConstraintType);

	public ColumnUniqueConstraintData(int? id, UserTypeEnum.UserType? source, bool isPk, SynchronizeStateEnum.SynchronizeState status, bool disabled, UniqueConstraintType.UniqueConstraintTypeEnum constraintType)
	{
		Id = id;
		Source = source;
		IsPk = isPk;
		Status = status;
		Disabled = disabled;
		ConstraintType = constraintType;
	}

	public ColumnUniqueConstraintData(ColumnDocObject row, string dataPropertyPrefix = "")
	{
		Id = row.UniqueConstraintId;
		Source = UserTypeEnum.ObjectToType(row.UniqueConstraintSource);
		IsPk = row.UniqueConstraintPrimaryKey.GetValueOrDefault();
		Status = SynchronizeStateEnum.DBStringToState(row.UniqueConstraintStatus);
		Disabled = row.UniqueConstraintDisabled.GetValueOrDefault();
		if (Id.HasValue)
		{
			ConstraintType = UniqueConstraintType.SetType(Source, Status, IsPk, Disabled);
		}
	}

	public ColumnUniqueConstraintData(RelationWithUniqueConstraintsObject row, bool isPk)
	{
		if (isPk)
		{
			Id = row.PkUniqueConstraintId;
			Source = UserTypeEnum.ObjectToType(row.PkUniqueConstraintSource);
			IsPk = row.PkUniqueConstraintPrimaryKey.GetValueOrDefault();
			Status = SynchronizeStateEnum.DBStringToState(row.PkUniqueConstraintStatus);
			Disabled = row.PkUniqueConstraintDisabled.GetValueOrDefault();
		}
		else
		{
			Id = row.FkUniqueConstraintId;
			Source = UserTypeEnum.ObjectToType(row.FkUniqueConstraintSource);
			IsPk = row.FkUniqueConstraintPrimaryKey.GetValueOrDefault();
			Status = SynchronizeStateEnum.DBStringToState(row.FkUniqueConstraintStatus);
			Disabled = row.FkUniqueConstraintDisabled.GetValueOrDefault();
		}
		if (Id.HasValue)
		{
			ConstraintType = UniqueConstraintType.SetType(Source, Status, IsPk, Disabled);
		}
	}

	public ColumnUniqueConstraintData(ErdNodeColumnObject row, string dataPropertyPrefix = "")
	{
		Id = row.UniqueConstraintId;
		Source = UserTypeEnum.ObjectToType(row.UniqueConstraintSource);
		IsPk = PrepareValue.ToBool(row.UniqueConstraintPrimaryKey);
		Status = SynchronizeStateEnum.DBStringToState(row.UniqueConstraintStatus);
		Disabled = PrepareValue.ToBool(row.UniqueConstraintDisabled);
		if (Id.HasValue)
		{
			ConstraintType = UniqueConstraintType.SetType(Source, Status, IsPk, Disabled);
		}
	}
}
