using Dataedo.App.Enums;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.Common.Interfaces;
using Dataedo.Model.Data.Interfaces;
using Dataedo.Model.Enums;

namespace Dataedo.App.Classes.Synchronize;

public class BasicRow : BaseRow, IModifiableBasic, IBasicData, INameSource
{
	public SynchronizeStateEnum.SynchronizeState Status { get; set; }

	public BasicRow()
	{
		CommonInitialization();
	}

	public BasicRow(IStatus row)
	{
		CommonInitialization();
		SetStatus(row);
	}

	public void CommonInitialization()
	{
		SetUnchanged();
	}

	public void SetModified()
	{
		base.RowState = ManagingRowsEnum.ManagingRows.Updated;
	}

	public void SetUnchanged()
	{
		base.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
	}

	public void SetStatus(IStatus data)
	{
		Status = SynchronizeStateEnum.DBStringToState(data.Status);
	}

	public virtual bool CanBeDeleted()
	{
		return Status == SynchronizeStateEnum.SynchronizeState.Deleted;
	}

	public virtual bool IsDeletable()
	{
		return Status == SynchronizeStateEnum.SynchronizeState.Deleted;
	}
}
