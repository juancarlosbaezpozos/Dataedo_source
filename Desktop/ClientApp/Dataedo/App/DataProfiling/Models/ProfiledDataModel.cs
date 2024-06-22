using System.Collections.Generic;
using Dataedo.App.DataProfiling.Enums;
using Dataedo.DataSources.Base.DataProfiling.Model;

namespace Dataedo.App.DataProfiling.Models;

public class ProfiledDataModel
{
	public int TableId { get; set; }

	public int ColumnId { get; set; }

	public string ValuesListMode { get; set; }

	public bool IsRowDistributionProfiling { get; set; }

	public bool IsValuesProfiling { get; set; }

	public long ObjectRowsCount { get; set; }

	public SampleData ObjectSampleData { get; set; }

	public NumericFieldGeneralStatistics NumericFieldValuesStatistics { get; set; }

	public StringFieldGeneralStatistics StringFieldValuesStatistics { get; set; }

	public StringFieldLenghtGeneralStatistics StringFieldValuesLengthStatistics { get; set; }

	public IEnumerable<ValueContentTypeStatistics> ValueContentTypesCountsStatistics { get; set; }

	public DateFieldGeneralStatistics DateFieldGeneralStatistics { get; set; }

	public long RecordsWithDefaultValueCount { get; set; }

	public long? RecordsWithUniqueValueCount { get; set; }

	public IEnumerable<StringValueWithCount> StringFieldMostCommonlyUsedValues { get; set; }

	public IEnumerable<IntValueWithCount> StringValuesLenghts { get; set; }

	public IEnumerable<NullableIntValueWithCount> DateValuesHours { get; set; }

	public IEnumerable<DayOfWeekValueWithCount> DateValuesDayOfWeeks { get; set; }

	public IEnumerable<NullableIntValueWithCount> DateValuesYears { get; set; }

	public IEnumerable<NullableIntValueWithCount> DateValuesMonths { get; set; }

	public ProfiledDataModel(int tableId, int columnId, string valuesListMode, SampleData objectSampleData, long objectRowsCount, NumericFieldGeneralStatistics numericFieldValuesStatistics, StringFieldGeneralStatistics stringFieldValuesStatistics, StringFieldLenghtGeneralStatistics stringFieldValuesLengthStatistics, IEnumerable<ValueContentTypeStatistics> valueContentTypesCountsStatistics, DateFieldGeneralStatistics dateFieldGeneralStatistics, long recordsWithDefaultValueCount, long? recordsWithUniqueValueCount, IEnumerable<StringValueWithCount> stringFieldMostCommonlyUsedValues, IEnumerable<IntValueWithCount> stringValuesLenghts, IEnumerable<NullableIntValueWithCount> dateValuesHours, IEnumerable<DayOfWeekValueWithCount> dateValuesDayOfWeeks, IEnumerable<NullableIntValueWithCount> dateValuesYears, IEnumerable<NullableIntValueWithCount> dateValuesMonths, ProfilingType profileMode)
	{
		TableId = tableId;
		ColumnId = columnId;
		ValuesListMode = valuesListMode;
		ObjectSampleData = objectSampleData;
		ObjectRowsCount = objectRowsCount;
		NumericFieldValuesStatistics = numericFieldValuesStatistics;
		StringFieldValuesStatistics = stringFieldValuesStatistics;
		StringFieldValuesLengthStatistics = stringFieldValuesLengthStatistics;
		ValueContentTypesCountsStatistics = valueContentTypesCountsStatistics;
		DateFieldGeneralStatistics = dateFieldGeneralStatistics;
		RecordsWithDefaultValueCount = recordsWithDefaultValueCount;
		RecordsWithUniqueValueCount = recordsWithUniqueValueCount;
		StringFieldMostCommonlyUsedValues = stringFieldMostCommonlyUsedValues;
		StringValuesLenghts = stringValuesLenghts;
		DateValuesHours = dateValuesHours;
		DateValuesDayOfWeeks = dateValuesDayOfWeeks;
		DateValuesYears = dateValuesYears;
		DateValuesMonths = dateValuesMonths;
		SetProfilingTypeByProfilingModel(profileMode);
	}

	public void UpdateProfilingModel(ProfiledDataModel model)
	{
		if (model.ValuesListMode != null)
		{
			ValuesListMode = model.ValuesListMode;
		}
		if (model.ObjectSampleData != null)
		{
			ObjectSampleData = model.ObjectSampleData;
		}
		if (model.ObjectRowsCount != 0L)
		{
			ObjectRowsCount = model.ObjectRowsCount;
		}
		if (model.NumericFieldValuesStatistics != null)
		{
			NumericFieldValuesStatistics = model.NumericFieldValuesStatistics;
		}
		if (model.StringFieldValuesStatistics != null)
		{
			StringFieldValuesStatistics = model.StringFieldValuesStatistics;
		}
		if (model.StringFieldValuesLengthStatistics != null)
		{
			StringFieldValuesLengthStatistics = model.StringFieldValuesLengthStatistics;
		}
		if (model.ValueContentTypesCountsStatistics != null)
		{
			ValueContentTypesCountsStatistics = model.ValueContentTypesCountsStatistics;
		}
		if (model.DateFieldGeneralStatistics != null)
		{
			DateFieldGeneralStatistics = model.DateFieldGeneralStatistics;
		}
		if (model.RecordsWithDefaultValueCount != 0L)
		{
			RecordsWithDefaultValueCount = model.RecordsWithDefaultValueCount;
		}
		if (model.RecordsWithUniqueValueCount.HasValue && model.RecordsWithUniqueValueCount != 0)
		{
			RecordsWithUniqueValueCount = model.RecordsWithUniqueValueCount;
		}
		if (model.StringFieldMostCommonlyUsedValues != null)
		{
			StringFieldMostCommonlyUsedValues = model.StringFieldMostCommonlyUsedValues;
		}
		if (model.StringValuesLenghts != null)
		{
			StringValuesLenghts = model.StringValuesLenghts;
		}
		if (model.DateValuesHours != null)
		{
			DateValuesHours = model.DateValuesHours;
		}
		if (model.DateValuesDayOfWeeks != null)
		{
			DateValuesDayOfWeeks = model.DateValuesDayOfWeeks;
		}
		if (model.DateValuesYears != null)
		{
			DateValuesYears = model.DateValuesYears;
		}
		if (model.DateValuesMonths != null)
		{
			DateValuesMonths = model.DateValuesMonths;
		}
		if (model.IsRowDistributionProfiling)
		{
			IsRowDistributionProfiling = model.IsRowDistributionProfiling;
		}
		if (model.IsValuesProfiling)
		{
			IsValuesProfiling = model.IsValuesProfiling;
		}
	}

	private void SetProfilingTypeByProfilingModel(ProfilingType profileMode)
	{
		switch (profileMode)
		{
		case ProfilingType.Full:
			IsRowDistributionProfiling = true;
			IsValuesProfiling = true;
			break;
		case ProfilingType.Distribution:
			IsRowDistributionProfiling = true;
			break;
		case ProfilingType.Values:
			IsValuesProfiling = true;
			break;
		}
	}
}
