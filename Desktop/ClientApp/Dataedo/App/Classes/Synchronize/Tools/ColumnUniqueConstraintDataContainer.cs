using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Dataedo.App.Enums;
using Dataedo.App.Properties;

namespace Dataedo.App.Classes.Synchronize.Tools;

public class ColumnUniqueConstraintDataContainer
{
	public List<ColumnUniqueConstraintData> Data { get; set; }

	public bool IsKey => Data.Count > 0;

	public IEnumerable<ColumnUniqueConstraintData> SortedData => SortData(Data);

	public IEnumerable<ColumnUniqueConstraintData> SortedDataWithoutRelations => SortData(Data.Where((ColumnUniqueConstraintData x) => x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.NotSet));

	public ColumnUniqueConstraintData FirstItem => SortedData.FirstOrDefault();

	public ColumnUniqueConstraintData FirstItemWithoutRelations => SortedDataWithoutRelations.FirstOrDefault();

	public UniqueConstraintType.UniqueConstraintTypeEnum FirstItemConstraintType => FirstItem?.ConstraintType ?? UniqueConstraintType.UniqueConstraintTypeEnum.NotSet;

	public Image FirstItemIcon => FirstItem?.ObjectImage ?? Resources.blank_16;

	public Image FirstItemKeysIcon => FirstItemWithoutRelations?.KeysObjectImage ?? Resources.blank_16;

	public bool IsPk
	{
		get
		{
			ColumnUniqueConstraintData firstItem = FirstItem;
			if (firstItem == null)
			{
				return false;
			}
			return firstItem.ConstraintType == UniqueConstraintType.UniqueConstraintTypeEnum.PK;
		}
	}

	public bool IsUserDefinedPk => Data?.Any((ColumnUniqueConstraintData x) => x.ConstraintType == UniqueConstraintType.UniqueConstraintTypeEnum.PK_user) ?? false;

	public bool IsAnyActivePk
	{
		get
		{
			if (!IsPk)
			{
				return IsUserDefinedPk;
			}
			return true;
		}
	}

	public bool IsUk => Data?.Any((ColumnUniqueConstraintData x) => x.ConstraintType == UniqueConstraintType.UniqueConstraintTypeEnum.UK) ?? false;

	public bool IsUserDefinedUk => Data?.Any((ColumnUniqueConstraintData x) => x.ConstraintType == UniqueConstraintType.UniqueConstraintTypeEnum.UK_user) ?? false;

	public bool IsAnyActiveUk
	{
		get
		{
			if (!IsUk)
			{
				return IsUserDefinedUk;
			}
			return true;
		}
	}

	public ColumnUniqueConstraintDataContainer()
	{
		Data = new List<ColumnUniqueConstraintData>();
	}

	public ColumnUniqueConstraintDataContainer(ColumnUniqueConstraintData columnUniqueConstraintData)
		: this()
	{
		if (columnUniqueConstraintData.Id.HasValue)
		{
			Data.Add(columnUniqueConstraintData);
		}
	}

	public virtual IEnumerable<ColumnUniqueConstraintData> SortData(IEnumerable<ColumnUniqueConstraintData> enumerable)
	{
		return enumerable?.OrderBy((ColumnUniqueConstraintData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.PK) ? 1 : 0).ThenBy((ColumnUniqueConstraintData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.PK_user) ? 1 : 0).ThenBy((ColumnUniqueConstraintData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.UK) ? 1 : 0)
			.ThenBy((ColumnUniqueConstraintData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.UK_user) ? 1 : 0)
			.ThenBy((ColumnUniqueConstraintData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.PK_disabled) ? 1 : 0)
			.ThenBy((ColumnUniqueConstraintData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.UK_disabled) ? 1 : 0)
			.ThenBy((ColumnUniqueConstraintData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.PK_deleted) ? 1 : 0)
			.ThenBy((ColumnUniqueConstraintData x) => (x.ConstraintType != UniqueConstraintType.UniqueConstraintTypeEnum.UK_deleted) ? 1 : 0);
	}
}
