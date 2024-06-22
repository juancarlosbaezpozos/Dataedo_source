using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Common.WaitFormCanceling;
using Dataedo.App.Enums;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools;
using Dataedo.Model.Data.Common.Objects;

namespace Dataedo.App.Classes;

public static class CommonFunctionsSynchronize
{
	public static void AddDeletedObjects(ObservableCollection<ObjectRow> rows, IEnumerable<ObjectDeletedFromDatabaseObject> source, SynchronizeParameters synchronizeParameters, BackgroundWorkerManager backgroundWorkerManager)
	{
		if (source != null && source.Count() > 0)
		{
			backgroundWorkerManager.ReportProgress("Marking missing objects");
			(from ObjectDeletedFromDatabaseObject row in source
				select new ObjectRow(row, synchronizeParameters.DatabaseRow.Name)).ToList().ForEach(delegate(ObjectRow x)
			{
				rows.Add(x);
			});
		}
	}

	public static void SetToSynchronizeCheck(ObservableCollection<ObjectRow> rows, Locker dbConnectionLocker)
	{
		if (!dbConnectionLocker.IsCanceled)
		{
			rows.Where((ObjectRow x) => x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.New || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted || x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Unsynchronized).ToList().ForEach(delegate(ObjectRow x)
			{
				x.ToSynchronize = true;
			});
		}
		if (!dbConnectionLocker.IsCanceled)
		{
			rows.Where((ObjectRow x) => x.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Ignored).ToList().ForEach(delegate(ObjectRow x)
			{
				x.ToSynchronize = false;
			});
		}
	}

	public static List<string> ReportProgressForObject(ObjectRow row, double rowCount, double countObjects)
	{
		string text = row.TypeAsString.ToLower();
		return new List<string>
		{
			new StringBuilder().Append("Importing object ").Append(rowCount + " of ").Append(countObjects)
				.ToString(),
			new StringBuilder().Append(char.ToUpper(text[0])).Append(text.Substring(1)).Append(": ")
				.Append(DBTreeMenu.SetNameOnlySchema(row, row.MultipleSchemasDatabase == true))
				.ToString()
		};
	}
}
