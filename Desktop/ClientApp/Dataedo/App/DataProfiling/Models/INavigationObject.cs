using System.Collections.Generic;
using System.Drawing;
using Dataedo.DataSources.Base.DataProfiling.Model;
using Dataedo.Model.Data.DataProfiling;

namespace Dataedo.App.DataProfiling.Models;

public interface INavigationObject
{
	int TableId { get; }

	string TableName { get; }

	string TableSchema { get; }

	int Id { get; }

	string ParentTreeNodeId { get; set; }

	Bitmap IconTreeList { get; }

	bool ErrorOccurred { get; set; }

	string DisplayName { get; }

	string Name { get; }

	string Title { get; }

	string DataType { get; }

	string TextForSparkline { get; }

	bool ProfileCheckbox { get; set; }

	double Completion { get; }

	long? RowsCount { get; set; }

	SampleData ObjectSampleData { get; set; }

	List<ColumnValuesDataObject> ListOfValues { get; set; }

	bool ProfilingFailed { get; }

	string IdForTreeList { get; }
}
