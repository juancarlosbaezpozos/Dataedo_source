using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.Properties;
using Dataedo.DataSources.Base.DataProfiling.Enums;
using Dataedo.DataSources.Base.DataProfiling.Model;
using Dataedo.Model.Data.DataProfiling;
using Dataedo.Model.Data.Interfaces;
using Dataedo.Shared.Enums;

namespace Dataedo.App.DataProfiling.Models;

public class ColumnNavigationObject : INavigationObject, IColumnBase, IColumnInfoForProfiling
{
	public TableNavigationObject Table { get; set; }

	public ColumnProfiledDataObject Column { get; set; }

	public int TableId => Table.TableId;

	public string TableName => Table.TableName;

	public string TableSchema => Table.TableSchema;

	public SharedObjectTypeEnum.ObjectType ParentObjectType => Table.ObjectType;

	public int Id { get; private set; }

	public string ParentTreeNodeId { get; set; }

	public Bitmap IconTreeList => Resources.column_16;

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

	public string DataType { get; set; }

	public string ProfilingDataType { get; set; }

	public bool ProfileCheckbox { get; set; }

	public double ExactCompletion { get; set; }

	public double Completion => Math.Round(ExactCompletion, 0);

	public string TextForSparkline { get; set; }

	public int ColumnId { get; private set; }

	public string ValuesListMode { get; set; }

	public int? ValuesListRowsCount { get; set; }

	public long? RowsCount { get; set; }

	public bool IsEncrypted { get; set; }

	public IEnumerable<StringValueWithCount> FieldMostCommonlyUsedValues { get; set; }

	public SampleData ObjectSampleData { get; set; }

	public List<ColumnValuesDataObject> ListOfValues { get; set; }

	public bool ProfilingFailed
	{
		get
		{
			if (ErrorOccurred)
			{
				return Completion == 0.0;
			}
			return false;
		}
	}

	public bool ProfilingCompletedWithErrors
	{
		get
		{
			if (ErrorOccurred && Completion > 0.0)
			{
				return Completion < 100.0;
			}
			return false;
		}
	}

	public string IdForTreeList => $"Column_{Id}";

	public ColumnNavigationObject(TableNavigationObject table, ColumnProfiledDataObject column, int id, int navigationId, List<ColumnValuesDataObject> listOfValues)
	{
		Name = column.Name;
		Title = column.Title;
		DataType = column.DataType;
		ProfilingDataType = column.ProfilingDataType;
		ProfileCheckbox = true;
		Table = table;
		ColumnId = column.ColumnId;
		ExactCompletion = (column.ProfilingDate.HasValue ? 100 : 0);
		ValuesListMode = column.ValuesListMode;
		ValuesListRowsCount = column.ValuesListRowsCount;
		Column = column;
		Id = id;
		ParentTreeNodeId = Table.IdForTreeList;
		ListOfValues = listOfValues;
		ErrorOccurred = false;
		column.SetTextForSparkLine();
		TextForSparkline = column.TextForSparkLine;
	}

	public void ClearAllProfiling()
	{
		ExactCompletion = 0.0;
		ErrorOccurred = false;
		ClearValueProfiling();
		ClearDistiributionProfiling();
	}

	public void ClearDistiributionProfiling()
	{
		Column.SetTextForSparkLine();
		TextForSparkline = Column.TextForSparkLine;
		if (Column.ThereIsNoProfiling())
		{
			ExactCompletion = 0.0;
			ErrorOccurred = false;
		}
	}

	public void ClearValueProfiling()
	{
		ObjectSampleData = null;
		ListOfValues = null;
		FieldMostCommonlyUsedValues = null;
		ValuesListMode = null;
		ValuesListRowsCount = null;
		if (Column.ThereIsNoProfiling())
		{
			ExactCompletion = 0.0;
			ErrorOccurred = false;
		}
	}

	public void SetValuesListRowsCount()
	{
		if (Column.ValuesListRowsCount.HasValue)
		{
			if (ValuesListMode == "S")
			{
				ValuesListRowsCount = 10;
			}
			else if (ValuesListMode == "R")
			{
				ValuesListRowsCount = 1000;
			}
			else if (ValuesListMode == "T" && !Column.ValuesListRowsCount.HasValue)
			{
				ValuesListRowsCount = 10;
			}
		}
	}

	public void GetValuesFromRepository(Form owner)
	{
		try
		{
			List<ColumnValuesDataObject> list2 = (ListOfValues = DB.DataProfiling.SelectColumnValuesDataObjectForColumn(TableId, ColumnId));
		}
		catch (Exception ex)
		{
			ListOfValues = null;
			DataProfiler.ShowErrorMessageBox(ex, owner);
		}
	}

	public void UpdateProfilingData(ProfiledDataModel newProfiledDataModel)
	{
		Column.ProfilingDate = DateTime.Now;
		Column.ProfilingDataType = ProfilingDataType;
		Column.RowCount = newProfiledDataModel.ObjectRowsCount;
		if (newProfiledDataModel.IsRowDistributionProfiling)
		{
			Column.ValuesNullRowCount = (newProfiledDataModel.ValueContentTypesCountsStatistics?.Where((ValueContentTypeStatistics x) => x.FieldValueContentType == FieldValueContentTypeEnum.FieldValueContentType.Null)?.FirstOrDefault()?.Count).GetValueOrDefault();
			Column.ValuesEmptyRowCount = (newProfiledDataModel.ValueContentTypesCountsStatistics?.Where((ValueContentTypeStatistics x) => x.FieldValueContentType == FieldValueContentTypeEnum.FieldValueContentType.Empty)?.FirstOrDefault()?.Count).GetValueOrDefault();
			Column.ValuesDistinctRowCount = (newProfiledDataModel.ValueContentTypesCountsStatistics?.Where((ValueContentTypeStatistics x) => x.FieldValueContentType == FieldValueContentTypeEnum.FieldValueContentType.Distinct)?.FirstOrDefault()?.Count).GetValueOrDefault();
			Column.ValuesNondistinctRowCount = (newProfiledDataModel.ValueContentTypesCountsStatistics?.Where((ValueContentTypeStatistics x) => x.FieldValueContentType == FieldValueContentTypeEnum.FieldValueContentType.NonDistinct)?.FirstOrDefault()?.Count).GetValueOrDefault();
			Column.ValuesDefaultRowCount = (newProfiledDataModel.ValueContentTypesCountsStatistics?.Where((ValueContentTypeStatistics x) => x == null || !x.FieldValueContentType.HasValue)?.FirstOrDefault()?.Count).GetValueOrDefault();
		}
		if (newProfiledDataModel.IsValuesProfiling)
		{
			Column.ValuesDefaultRowCount = newProfiledDataModel.RecordsWithDefaultValueCount;
			Column.ValuesUniqueValues = newProfiledDataModel.RecordsWithUniqueValueCount;
			Column.ValueMin = ReturnNullIfInfinity(newProfiledDataModel.NumericFieldValuesStatistics?.Minimum);
			Column.ValueMax = ReturnNullIfInfinity(newProfiledDataModel.NumericFieldValuesStatistics?.Maximum);
			Column.ValueAvg = ReturnNullIfInfinity(newProfiledDataModel.NumericFieldValuesStatistics?.Average);
			Column.ValueStddev = ReturnNullIfInfinity(newProfiledDataModel.NumericFieldValuesStatistics?.StandardDeviation);
			Column.ValueVar = ReturnNullIfInfinity(newProfiledDataModel.NumericFieldValuesStatistics?.Variance);
			Column.ValueMinString = newProfiledDataModel.StringFieldValuesStatistics?.Minimum;
			Column.ValueMaxString = newProfiledDataModel.StringFieldValuesStatistics?.Maximum;
			Column.ValueMinDate = newProfiledDataModel.DateFieldGeneralStatistics?.Minimum;
			Column.ValueMaxDate = newProfiledDataModel.DateFieldGeneralStatistics?.Maximum;
			Column.StringLengthMin = newProfiledDataModel.StringFieldValuesLengthStatistics?.Minimum;
			Column.StringLengthMax = ReturnNullIfInfinity(newProfiledDataModel.StringFieldValuesLengthStatistics?.Maximum);
			Column.StringLengthAvg = ReturnNullIfInfinity(newProfiledDataModel.StringFieldValuesLengthStatistics?.Average);
			Column.StringLengthStddev = ReturnNullIfInfinity(newProfiledDataModel.StringFieldValuesLengthStatistics?.StandardDeviation);
			Column.StringLengthVar = ReturnNullIfInfinity(newProfiledDataModel.StringFieldValuesLengthStatistics?.Variance);
		}
	}

	private static double? ReturnNullIfInfinity(double? number)
	{
		if (double.IsInfinity(number.GetValueOrDefault()) || double.IsNaN(number.GetValueOrDefault()))
		{
			number = null;
		}
		return number;
	}
}
