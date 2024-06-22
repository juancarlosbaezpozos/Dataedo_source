using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Dataedo.App.Properties;
using Dataedo.DataSources.Base.DataProfiling.Model;
using Dataedo.Model.Data.DataProfiling;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataProfiling.Models;

public class TableNavigationObject : INavigationObject
{
	public List<ColumnNavigationObject> Columns { get; }

	public SharedObjectTypeEnum.ObjectType ObjectType { get; private set; }

	public int TableId { get; private set; }

	public string TableName { get; private set; }

	public string TableSchema { get; private set; }

	public int Id { get; private set; }

	public string ParentTreeNodeId { get; set; }

	public Bitmap IconTreeList => ObjectType switch
	{
		SharedObjectTypeEnum.ObjectType.Table => Resources.table_16, 
		SharedObjectTypeEnum.ObjectType.View => Resources.view_16, 
		_ => Resources.question_16, 
	};

	public bool ErrorOccurred { get; set; }

	public string DisplayName
	{
		get
		{
			if (!string.IsNullOrEmpty(Title))
			{
				return Name + " (" + Title + ")";
			}
			return Name ?? "";
		}
	}

	public string Name { get; private set; }

	public string Title { get; private set; }

	public bool ProfileCheckbox { get; set; }

	public double Completion => Math.Round(Columns.Sum((ColumnNavigationObject x) => x.Completion) / (double)Columns.Count, 0);

	public string DataType => string.Empty;

	public string TextForSparkline => string.Empty;

	public long? RowsCount { get; set; }

	public SampleData ObjectSampleData { get; set; }

	public List<ColumnValuesDataObject> ListOfValues { get; set; }

	public bool ProfilingFailed => Columns.All((ColumnNavigationObject x) => x.ErrorOccurred && x.Completion == 0.0);

	public bool AllColumnProfiled => Columns.All((ColumnNavigationObject x) => x.Column.ProfilingDate.HasValue);

	public string IdForTreeList => $"{ObjectType}_{Id}";

	public TableNavigationObject(TableSimpleData tableSimpleData)
	{
		Id = tableSimpleData.TableId;
		Name = tableSimpleData.TableName;
		Title = tableSimpleData.Title;
		Columns = new List<ColumnNavigationObject>();
		ProfileCheckbox = true;
		TableId = tableSimpleData.TableId;
		TableName = tableSimpleData.TableName;
		TableSchema = tableSimpleData.Schema;
		ObjectType = tableSimpleData.ObjectType;
		ParentTreeNodeId = IdForTreeList;
	}

	public void ClearValueProfiling()
	{
		ListOfValues = null;
		ObjectSampleData = null;
	}

	public void ClearAllProfiling()
	{
		RowsCount = null;
		ClearValueProfiling();
	}
}
