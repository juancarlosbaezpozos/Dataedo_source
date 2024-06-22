using System.Collections.Generic;
using System.Linq;
using Dataedo.App.Enums;
using Dataedo.Shared.Enums;

namespace Dataedo.App.Classes.Synchronize.Tools;

public class ColumnUniqueConstraintWithFkDataContainer : ColumnUniqueConstraintDataContainer
{
	public IEnumerable<ColumnUniqueConstraintWithFkData> CastedData => base.Data?.Cast<ColumnUniqueConstraintWithFkData>();

	public bool IsAnyFk => CastedData?.Any((ColumnUniqueConstraintWithFkData x) => x.IsFk) ?? false;

	public ColumnUniqueConstraintWithFkDataContainer()
	{
	}

	public ColumnUniqueConstraintWithFkDataContainer(ColumnUniqueConstraintWithFkData columnUniqueConstraintData)
		: this()
	{
		if (columnUniqueConstraintData.HasData)
		{
			base.Data.Add(columnUniqueConstraintData);
		}
	}

	public void AddUniqueConstraint(int? id, bool isPk, SynchronizeStateEnum.SynchronizeState status, bool disabled, UniqueConstraintType.UniqueConstraintTypeEnum constraintType, int? pkColumnId)
	{
		base.Data.Add(new ColumnUniqueConstraintWithFkData(id, UserTypeEnum.UserType.USER, isPk, status, disabled, constraintType, pkColumnId, UserTypeEnum.UserType.USER));
	}

	public void RemoveUniqueConstraint(int id)
	{
		base.Data.Remove(base.Data.FirstOrDefault((ColumnUniqueConstraintData x) => x.Id == id));
	}

	public override IEnumerable<ColumnUniqueConstraintData> SortData(IEnumerable<ColumnUniqueConstraintData> enumerable)
	{
		return enumerable.Cast<ColumnUniqueConstraintWithFkData>()?.OrderBy((ColumnUniqueConstraintWithFkData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.PK) ? 1 : 0).ThenBy((ColumnUniqueConstraintWithFkData x) => (!x.IsFk || x.RelationSource != UserTypeEnum.UserType.DBMS) ? 1 : 0).ThenBy((ColumnUniqueConstraintWithFkData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.PK_user) ? 1 : 0)
			.ThenBy((ColumnUniqueConstraintWithFkData x) => (!x.IsFk || x.RelationSource == UserTypeEnum.UserType.DBMS) ? 1 : 0)
			.ThenBy((ColumnUniqueConstraintWithFkData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.UK) ? 1 : 0)
			.ThenBy((ColumnUniqueConstraintWithFkData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.UK_user) ? 1 : 0)
			.ThenBy((ColumnUniqueConstraintWithFkData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.PK_disabled) ? 1 : 0)
			.ThenBy((ColumnUniqueConstraintWithFkData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.UK_disabled) ? 1 : 0)
			.ThenBy((ColumnUniqueConstraintWithFkData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.PK_deleted) ? 1 : 0)
			.ThenBy((ColumnUniqueConstraintWithFkData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.UK_deleted) ? 1 : 0)
			.ToArray();
	}
}
