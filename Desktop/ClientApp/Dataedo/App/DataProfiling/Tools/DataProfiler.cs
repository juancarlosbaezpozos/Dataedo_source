using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DataProfiling.Enums;
using Dataedo.App.DataProfiling.Models;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.SqlCommands;
using Dataedo.DataSources.Base.DataProfiling.Enums;
using Dataedo.DataSources.Base.DataProfiling.Model;
using Dataedo.DataSources.Base.Exceptions;
using Dataedo.DataSources.Base.Models;
using Dataedo.DataSources.Commands;
using Dataedo.DataSources.Enums;
using Dataedo.DataSources.Factories;
using Dataedo.Shared.Enums;
using DevExpress.Utils.Extensions;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.DataProfiling.Tools;

internal static class DataProfiler
{
	public static DatabaseRow DatabaseRow { get; set; }

	public static async Task<long> ReturnColumnUniqueValuesCounterAsync(ColumnNavigationObject navColumn, DatabaseRow databaseRow, CancellationTokenSource cancellationTokenSource)
	{
		ProfilingDatabaseTypeEnum.ProfilingDatabaseType profilingDatabaseTypeEnum = EnumToEnumChanger.GetProfilingDatabaseTypeEnum(databaseRow.Type);
		CommandsSet commandsSet = CommandsFactory.GetDbConnectionCommands(profilingDatabaseTypeEnum, databaseRow.ConnectionString, CommandsWithTimeout.Timeout);
		long recordsWithUniqueValueCount = 0L;
		try
		{
			recordsWithUniqueValueCount = await Task.Run(() => commandsSet.DataProfiling.GetRecordsWithUniqueValueCount(navColumn.TableSchema, navColumn.TableName, navColumn.Name, cancellationTokenSource.Token));
			return recordsWithUniqueValueCount;
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
			return recordsWithUniqueValueCount;
		}
		catch (Exception ex3)
		{
			ProcessException(ex3, DatabaseRow);
			return recordsWithUniqueValueCount;
		}
	}

	public static async Task<IEnumerable<StringValueWithCount>> ReturnColumnMostCommonlyUsedValuesAsync(ColumnNavigationObject navColumn, DatabaseRow databaseRow, CancellationTokenSource cancellationTokenSource)
	{
		ProfilingDatabaseTypeEnum.ProfilingDatabaseType profilingDatabaseTypeEnum = EnumToEnumChanger.GetProfilingDatabaseTypeEnum(databaseRow.Type);
		CommandsSet commandsSet = CommandsFactory.GetDbConnectionCommands(profilingDatabaseTypeEnum, databaseRow.ConnectionString, CommandsWithTimeout.Timeout);
		int? maxResults = navColumn.ValuesListRowsCount;
		IEnumerable<StringValueWithCount> result = null;
		try
		{
			navColumn.IsEncrypted = await commandsSet.DataProfiling.CheckIfColumnIsEncrypted(navColumn.Name, navColumn.TableSchema, navColumn.TableName, cancellationTokenSource.Token);
			if (navColumn.IsEncrypted)
			{
				return result;
			}
			result = await ReturnColumnMostCommonlyUsedValuesAsync(cancellationTokenSource, commandsSet, navColumn, maxResults.GetValueOrDefault());
		}
		catch (OperationCanceledException)
		{
		}
		catch (DatasourceOperationNotSupportedException)
		{
		}
		catch (Exception ex3)
		{
			ProcessException(ex3, DatabaseRow);
		}
		return result;
	}

	private static async Task<IEnumerable<StringValueWithCount>> ReturnColumnMostCommonlyUsedValuesAsync(CancellationTokenSource cancellationTokenSource, CommandsSet commandsSet, ColumnNavigationObject navColumn, int maxResults)
	{
		if (navColumn.ValuesListMode != "T" && navColumn.ValuesListMode != "R")
		{
			return null;
		}
		if (navColumn.IsEncrypted)
		{
			return null;
		}
		return ((!DataTypeChecker.IsStringType(navColumn.DataType)) ? (await Task.Run(() => commandsSet.DataProfiling.GetAnyFieldMostCommonlyUsedValues(navColumn.TableSchema, navColumn.TableName, navColumn.Name, maxResults, cancellationTokenSource.Token))) : (await Task.Run(() => commandsSet.DataProfiling.GetStringFieldMostCommonlyUsedValues(navColumn.TableSchema, navColumn.TableName, navColumn.Name, maxResults, cancellationTokenSource.Token))))?.Where((StringValueWithCount v) => v?.Value != null);
	}

	public static async Task<SampleData> ReturnColumnRandomValuesAsync(ColumnNavigationObject navColumn, DatabaseRow databaseRow, CancellationTokenSource cancellationTokenSource, long rowsCount, bool returnOnlyNotNullValues = false)
	{
		ProfilingDatabaseTypeEnum.ProfilingDatabaseType databaseTypeEnum = EnumToEnumChanger.GetProfilingDatabaseTypeEnum(databaseRow.Type);
		CommandsSet commandsSet = CommandsFactory.GetDbConnectionCommands(databaseTypeEnum, databaseRow.ConnectionString, CommandsWithTimeout.Timeout);
		int maxResults = navColumn.ValuesListRowsCount.GetValueOrDefault();
		SampleData objectSampleData = null;
		try
		{
			navColumn.IsEncrypted = await commandsSet.DataProfiling.CheckIfColumnIsEncrypted(navColumn.Name, navColumn.TableSchema, navColumn.TableName, cancellationTokenSource.Token);
			if (navColumn.ValuesListMode == "S")
			{
				if (!navColumn.IsEncrypted)
				{
					if (!DataTypeChecker.TypeIsNotSupportedForProfiling(navColumn.DataType, databaseTypeEnum))
					{
						ProfilingField @this = new ProfilingField(navColumn.Name, DataTypeChecker.IsStringType(navColumn.DataType), isEncrypted: false);
						objectSampleData = await commandsSet.DataProfiling.GetObjectSampleData(navColumn.TableSchema, navColumn.TableName, @this.Yield().ToList(), maxResults, cancellationTokenSource.Token, rowsCount, null, navColumn.ParentObjectType == SharedObjectTypeEnum.ObjectType.View, returnOnlyNotNullValues);
						return objectSampleData;
					}
					return objectSampleData;
				}
				return objectSampleData;
			}
			return objectSampleData;
		}
		catch (OperationCanceledException)
		{
			return objectSampleData;
		}
		catch (DatasourceOperationNotSupportedException)
		{
			return objectSampleData;
		}
		catch (Exception ex3)
		{
			ProcessException(ex3, DatabaseRow);
			return objectSampleData;
		}
	}

	public static async Task ProfileTableAsync(TableNavigationObject navTable, CancellationTokenSource cancellationTokenSource, CommandsSet commandsSet)
	{
		navTable.ErrorOccurred = false;
		navTable.ObjectSampleData = await ProfileTableToGetSampleDataAsync(navTable, cancellationTokenSource, commandsSet);
	}

	private static async Task<SampleData> ProfileTableToGetSampleDataAsync(TableNavigationObject navTable, CancellationTokenSource cancellationTokenSource, CommandsSet commandsSet)
	{
		List<ProfilingField> namesOfColumns = (from c in navTable.Columns
			where !DataTypeChecker.TypeIsNotSupportedForProfiling(c.DataType, commandsSet.ProfilingDatabaseType)
			select c into x
			select new ProfilingField(x.Name, DataTypeChecker.IsStringType(x.DataType), isEncrypted: false)).ToList();
		SampleData objectSampleData = new SampleData();
		try
		{
			if (namesOfColumns.Any())
			{
				objectSampleData = await Task.Run(() => commandsSet.DataProfiling.GetObjectSampleData(navTable.TableSchema, navTable.Name, namesOfColumns, 20, cancellationTokenSource.Token, navTable.RowsCount.GetValueOrDefault(), null, navTable.ObjectType == SharedObjectTypeEnum.ObjectType.View));
				return objectSampleData;
			}
			return objectSampleData;
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
			return objectSampleData;
		}
		catch (Exception ex3)
		{
			ProcessException(ex3, DatabaseRow);
			navTable.ErrorOccurred = true;
			return objectSampleData;
		}
	}

	public static async Task<ProfiledDataModel> GetColumnProfiledDataModelAsync(ColumnNavigationObject navColumn, CommandsSet commandsSet, ProfilingDatabaseTypeEnum.ProfilingDatabaseType databaseTypeEnum, CancellationTokenSource cancellationTokenSource, ProfilingProgress mainProfilingProgress, ProfilingType profileMode)
	{
		bool isFullOrRowProfile = profileMode == ProfilingType.Full || profileMode == ProfilingType.Distribution;
		bool isFullOrValueProfile = profileMode == ProfilingType.Full || profileMode == ProfilingType.Values;
		_ = navColumn.TableSchema;
		_ = navColumn.TableName;
		navColumn.ErrorOccurred = false;
		navColumn.ExactCompletion = 0.0;
		string dataTypeProfiling = DataTypeChecker.GetProfilingDataType(navColumn.DataType, databaseTypeEnum);
		bool dataTypeProfilingIsUnknown = dataTypeProfiling == null;
		if (!dataTypeProfilingIsUnknown)
		{
			navColumn.ProfilingDataType = dataTypeProfiling;
			navColumn.Column.ProfilingDataType = dataTypeProfiling;
		}
		bool columnTypeIsNotSupported = DataTypeChecker.TypeIsNotSupportedForProfiling(dataTypeProfiling, databaseTypeEnum);
		if (navColumn.DataType?.ToUpper(CultureInfo.InvariantCulture) == "TIMESTAMP" && databaseTypeEnum == ProfilingDatabaseTypeEnum.ProfilingDatabaseType.SqlServer)
		{
			columnTypeIsNotSupported = true;
		}
		if (navColumn.DataType?.ToUpper(CultureInfo.InvariantCulture) == "TEXT" && databaseTypeEnum == ProfilingDatabaseTypeEnum.ProfilingDatabaseType.Snowflake)
		{
			columnTypeIsNotSupported = false;
		}
		if (await commandsSet.DataProfiling.CheckIfColumnIsEncrypted(navColumn.Name, navColumn.TableSchema, navColumn.TableName, cancellationTokenSource.Token))
		{
			navColumn.IsEncrypted = true;
			columnTypeIsNotSupported = true;
		}
		bool flag = dataTypeProfilingIsUnknown || dataTypeProfiling == "INT" || dataTypeProfiling == "REAL";
		bool flag2 = dataTypeProfilingIsUnknown || dataTypeProfiling == "STRING" || dataTypeProfiling == "STRING_C";
		bool flag3 = dataTypeProfilingIsUnknown || dataTypeProfiling == "DATETIME" || dataTypeProfiling == "DATE";
		bool flag4 = dataTypeProfilingIsUnknown || dataTypeProfiling == "DATETIME" || dataTypeProfiling == "TIME";
		Dictionary<int, bool> stepsConditions = new Dictionary<int, bool>
		{
			{ 1, isFullOrRowProfile },
			{ 2, true },
			{ 3, isFullOrValueProfile },
			{
				4,
				flag && isFullOrValueProfile
			},
			{
				5,
				flag2 && isFullOrValueProfile
			},
			{
				6,
				flag2 && isFullOrValueProfile
			},
			{ 7, isFullOrValueProfile },
			{ 8, isFullOrValueProfile },
			{ 9, isFullOrValueProfile },
			{
				10,
				flag2 && isFullOrValueProfile
			},
			{
				11,
				flag3 && isFullOrValueProfile
			},
			{
				12,
				flag4 && isFullOrValueProfile
			},
			{
				13,
				flag3 && isFullOrValueProfile
			},
			{
				14,
				flag3 && isFullOrValueProfile
			},
			{
				15,
				flag3 && isFullOrValueProfile
			}
		};
		int num = stepsConditions.Values.Count((bool x) => x);
		mainProfilingProgress.SetNumberOfStepsInColumnProfiling(num);
		double progressPerStep = 100.0 / (double)num;
		IEnumerable<ValueContentTypeStatistics> valueContentTypesCountsStatistics = null;
		if (stepsConditions[1])
		{
			valueContentTypesCountsStatistics = await GetStringFieldValueContentTypesCountsStatisticsAsync(navColumn, commandsSet, cancellationTokenSource, dataTypeProfilingIsUnknown, columnTypeIsNotSupported, dataTypeProfiling, progressPerStep);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		long objectRowsCount = 0L;
		if (stepsConditions[2])
		{
			objectRowsCount = await GetTableRowsCountAsync(navColumn, commandsSet, cancellationTokenSource, dataTypeProfilingIsUnknown, columnTypeIsNotSupported, progressPerStep);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		navColumn.RowsCount = objectRowsCount;
		navColumn.Column.RowCount = objectRowsCount;
		if (!navColumn.ValuesListRowsCount.HasValue && isFullOrValueProfile && navColumn.RowsCount != 0)
		{
			long? num2 = valueContentTypesCountsStatistics?.Where((ValueContentTypeStatistics x) => x.FieldValueContentType == FieldValueContentTypeEnum.FieldValueContentType.NonDistinct)?.FirstOrDefault()?.Count;
			if (num2.HasValue && profileMode == ProfilingType.Full)
			{
				if (navColumn.RowsCount < 10)
				{
					navColumn.ValuesListRowsCount = 10;
					if (navColumn.ValuesListMode == null)
					{
						navColumn.ValuesListMode = "T";
					}
				}
				else if (navColumn.RowsCount < 100)
				{
					navColumn.ValuesListRowsCount = 100;
					if (navColumn.ValuesListMode == null)
					{
						navColumn.ValuesListMode = "T";
					}
				}
				else if (navColumn.RowsCount >= 100)
				{
					navColumn.ValuesListRowsCount = 1000;
					if (navColumn.ValuesListMode == null)
					{
						navColumn.ValuesListMode = "T";
					}
				}
			}
			else if (!num2.HasValue)
			{
				if (navColumn.RowsCount < 100)
				{
					navColumn.ValuesListRowsCount = 1000;
					if (navColumn.ValuesListMode == null)
					{
						navColumn.ValuesListMode = "R";
					}
				}
				if (navColumn.RowsCount >= 100)
				{
					navColumn.ValuesListRowsCount = 10;
					if (navColumn.ValuesListMode == null)
					{
						navColumn.ValuesListMode = "S";
					}
				}
			}
			else
			{
				navColumn.ValuesListMode = (string.IsNullOrEmpty(navColumn.ValuesListMode) ? "T" : navColumn.ValuesListMode);
				navColumn.ValuesListRowsCount = navColumn.ValuesListRowsCount ?? 10;
			}
		}
		if ((string.IsNullOrEmpty(navColumn.ValuesListMode) || !navColumn.ValuesListRowsCount.HasValue) && profileMode == ProfilingType.Full)
		{
			navColumn.ValuesListMode = "T";
			navColumn.ValuesListRowsCount = 10;
		}
		int? maxResults = navColumn.ValuesListRowsCount;
		navColumn.Column.ValuesListMode = navColumn.ValuesListMode;
		navColumn.Column.ValuesListRowsCount = navColumn.ValuesListRowsCount;
		SampleData objectSampleData = null;
		if (stepsConditions[3])
		{
			objectSampleData = await ReturnColumnRandomValuesAsync(navColumn, maxResults, commandsSet, cancellationTokenSource, dataTypeProfilingIsUnknown, columnTypeIsNotSupported, progressPerStep, objectRowsCount);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		NumericFieldGeneralStatistics numericFieldValuesStatistics = null;
		if (stepsConditions[4])
		{
			numericFieldValuesStatistics = await GetNumericFieldValuesStatisticsAsync(navColumn, commandsSet, cancellationTokenSource, dataTypeProfilingIsUnknown, columnTypeIsNotSupported, progressPerStep);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		StringFieldGeneralStatistics stringFieldValuesStatistics = null;
		if (stepsConditions[5])
		{
			stringFieldValuesStatistics = await GetStringFieldValuesStatisticsAsync(navColumn, commandsSet, cancellationTokenSource, dataTypeProfiling, dataTypeProfilingIsUnknown, columnTypeIsNotSupported, progressPerStep);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		StringFieldLenghtGeneralStatistics stringFieldValuesLengthStatistics = null;
		if (stepsConditions[6])
		{
			stringFieldValuesLengthStatistics = await GetStringFieldValuesLengthStatisticsAsync(navColumn, commandsSet, cancellationTokenSource, dataTypeProfilingIsUnknown, columnTypeIsNotSupported, progressPerStep);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		long recordsWithDefaultValueCount = 0L;
		if (stepsConditions[7])
		{
			recordsWithDefaultValueCount = await GetRecordsWithDefaultValueCountAsync(navColumn, commandsSet, cancellationTokenSource, progressPerStep);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		long? recordsWithUniqueValueCount = null;
		if (stepsConditions[8])
		{
			recordsWithUniqueValueCount = await GetRecordsWithUniqueValueCountAsync(navColumn, dataTypeProfiling, commandsSet, cancellationTokenSource, dataTypeProfilingIsUnknown, columnTypeIsNotSupported, progressPerStep);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		IEnumerable<StringValueWithCount> fieldMostCommonlyUsedValues = null;
		if (stepsConditions[9])
		{
			fieldMostCommonlyUsedValues = await GetFieldMostCommonlyUsedValuesAsync(navColumn, maxResults, commandsSet, cancellationTokenSource, dataTypeProfilingIsUnknown, columnTypeIsNotSupported, dataTypeProfiling, progressPerStep);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		IEnumerable<IntValueWithCount> stringValuesLenghts = null;
		if (stepsConditions[10])
		{
			stringValuesLenghts = await GetStringValuesLenghtsAsync(navColumn, commandsSet, cancellationTokenSource, dataTypeProfilingIsUnknown, columnTypeIsNotSupported, progressPerStep);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		DateFieldGeneralStatistics dateFieldValuesStatistics = null;
		if (stepsConditions[11])
		{
			dateFieldValuesStatistics = await GetDateFieldValuesStatisticsAsync(navColumn, commandsSet, cancellationTokenSource, dataTypeProfilingIsUnknown, columnTypeIsNotSupported, progressPerStep);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		IEnumerable<NullableIntValueWithCount> dateValuesHours = null;
		if (stepsConditions[12])
		{
			dateValuesHours = await GetDateValuesHoursAsync(navColumn, commandsSet, cancellationTokenSource, dataTypeProfilingIsUnknown, columnTypeIsNotSupported, progressPerStep);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		IEnumerable<DayOfWeekValueWithCount> dateValuesDayOfWeeks = null;
		if (stepsConditions[13])
		{
			dateValuesDayOfWeeks = await GetDateValuesDaysOfWeeksAsync(navColumn, commandsSet, cancellationTokenSource, dataTypeProfilingIsUnknown, columnTypeIsNotSupported, progressPerStep);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		IEnumerable<NullableIntValueWithCount> dateValuesYears = null;
		if (stepsConditions[14])
		{
			dateValuesYears = await GetDateValuesYearsAsync(navColumn, commandsSet, cancellationTokenSource, dataTypeProfilingIsUnknown, columnTypeIsNotSupported, progressPerStep);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		IEnumerable<NullableIntValueWithCount> dateValuesMonths = null;
		if (stepsConditions[15])
		{
			dateValuesMonths = await GetDateValuesMonthsAsync(navColumn, commandsSet, cancellationTokenSource, dataTypeProfilingIsUnknown, columnTypeIsNotSupported, progressPerStep);
			mainProfilingProgress.ObjectProfilingStepEnded();
		}
		if (isFullOrValueProfile)
		{
			navColumn.FieldMostCommonlyUsedValues = fieldMostCommonlyUsedValues;
			navColumn.ObjectSampleData = objectSampleData;
			navColumn.RowsCount = objectRowsCount;
		}
		return new ProfiledDataModel(navColumn.TableId, navColumn.ColumnId, navColumn.ValuesListMode, objectSampleData, objectRowsCount, numericFieldValuesStatistics, stringFieldValuesStatistics, stringFieldValuesLengthStatistics, valueContentTypesCountsStatistics, dateFieldValuesStatistics, recordsWithDefaultValueCount, recordsWithUniqueValueCount, fieldMostCommonlyUsedValues, stringValuesLenghts, dateValuesHours, dateValuesDayOfWeeks, dateValuesYears, dateValuesMonths, profileMode);
	}

	private static async Task<IEnumerable<ValueContentTypeStatistics>> GetStringFieldValueContentTypesCountsStatisticsAsync(ColumnNavigationObject navColumn, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, bool dataTypeProfilingIsUnknown, bool columnTypeIsNotSupported, string dataTypeProfiling, double progressPerStep)
	{
		IEnumerable<ValueContentTypeStatistics> valueContentTypesCountsStatistics = null;
		bool stepFailed = false;
		if (dataTypeProfilingIsUnknown || dataTypeProfiling == "STRING")
		{
			try
			{
				valueContentTypesCountsStatistics = await Task.Run(() => commandsSet.DataProfiling.GetStringFieldValueContentTypesCountsStatistics(navColumn.TableSchema, navColumn.TableName, navColumn.Name, cancellationTokenSource.Token));
			}
			catch (OperationCanceledException)
			{
				throw new OperationCanceledException();
			}
			catch (DatasourceOperationNotSupportedException)
			{
			}
			catch (Exception ex3)
			{
				if (!dataTypeProfilingIsUnknown && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
				{
					ProcessException(ex3, DatabaseRow);
					stepFailed = true;
					navColumn.ErrorOccurred = true;
				}
			}
		}
		bool flag = valueContentTypesCountsStatistics == null;
		if (dataTypeProfilingIsUnknown || flag)
		{
			try
			{
				valueContentTypesCountsStatistics = await Task.Run(() => commandsSet.DataProfiling.GetAnyFieldValueContentTypesCountsStatistics(navColumn.TableSchema, navColumn.TableName, navColumn.Name, cancellationTokenSource.Token));
			}
			catch (OperationCanceledException)
			{
				throw new OperationCanceledException();
			}
			catch (DatasourceOperationNotSupportedException)
			{
			}
			catch (Exception ex6)
			{
				if (!dataTypeProfilingIsUnknown && dataTypeProfiling != "STRING_C" && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
				{
					ProcessException(ex6, DatabaseRow);
					stepFailed = true;
					navColumn.ErrorOccurred = true;
				}
			}
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return valueContentTypesCountsStatistics;
	}

	private static async Task<long> GetTableRowsCountAsync(ColumnNavigationObject navColumn, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, bool dataTypeProfilingIsUnknown, bool columnTypeIsNotSupported, double progressPerStep)
	{
		long objectRowsCount = 0L;
		bool stepFailed = false;
		try
		{
			objectRowsCount = await Task.Run(() => commandsSet.DataProfiling.GetObjectRowsCount(navColumn.TableSchema, navColumn.TableName, cancellationTokenSource.Token));
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
		}
		catch (Exception ex3)
		{
			if (!dataTypeProfilingIsUnknown && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
			{
				ProcessException(ex3, DatabaseRow);
				stepFailed = true;
				navColumn.ErrorOccurred = true;
			}
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return objectRowsCount;
	}

	private static async Task<SampleData> ReturnColumnRandomValuesAsync(ColumnNavigationObject navColumn, int? maxResults, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, bool dataTypeProfilingIsUnknown, bool columnTypeIsNotSupported, double progressPerStep, long rowsCount)
	{
		SampleData objectSampleData = null;
		bool stepFailed = false;
		try
		{
			if (navColumn.ValuesListMode == "S" && !navColumn.IsEncrypted && !DataTypeChecker.TypeIsNotSupportedForProfiling(navColumn.DataType, commandsSet.ProfilingDatabaseType))
			{
				ProfilingField @this = new ProfilingField(navColumn.Name, DataTypeChecker.IsStringType(navColumn.DataType), isEncrypted: false);
				objectSampleData = await commandsSet.DataProfiling.GetObjectSampleData(navColumn.TableSchema, navColumn.TableName, @this.Yield().ToList(), maxResults, cancellationTokenSource.Token, rowsCount, null, navColumn.ParentObjectType == SharedObjectTypeEnum.ObjectType.View);
			}
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
		}
		catch (Exception ex3)
		{
			if (!dataTypeProfilingIsUnknown && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
			{
				ProcessException(ex3, DatabaseRow);
				stepFailed = true;
				navColumn.ErrorOccurred = true;
			}
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return objectSampleData;
	}

	private static async Task<NumericFieldGeneralStatistics> GetNumericFieldValuesStatisticsAsync(ColumnNavigationObject navColumn, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, bool dataTypeProfilingIsUnknown, bool columnTypeIsNotSupported, double progressPerStep)
	{
		NumericFieldGeneralStatistics numericFieldValuesStatistics = null;
		bool stepFailed = false;
		try
		{
			numericFieldValuesStatistics = await Task.Run(() => commandsSet.DataProfiling.GetNumericFieldValuesStatistics(navColumn.TableSchema, navColumn.TableName, navColumn.Name, cancellationTokenSource.Token));
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
		}
		catch (Exception ex3)
		{
			if (!dataTypeProfilingIsUnknown && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
			{
				ProcessException(ex3, DatabaseRow);
				stepFailed = true;
				navColumn.ErrorOccurred = true;
			}
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return numericFieldValuesStatistics;
	}

	private static async Task<StringFieldGeneralStatistics> GetStringFieldValuesStatisticsAsync(ColumnNavigationObject navColumn, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, string dataTypeProfiling, bool dataTypeProfilingIsUnknown, bool columnTypeIsNotSupported, double progressPerStep)
	{
		StringFieldGeneralStatistics stringFieldValuesStatistics = null;
		bool stepFailed = false;
		try
		{
			stringFieldValuesStatistics = await Task.Run(() => commandsSet.DataProfiling.GetStringFieldValuesStatistics(navColumn.TableSchema, navColumn.TableName, navColumn.Name, cancellationTokenSource.Token));
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
		}
		catch (Exception ex3)
		{
			if (!dataTypeProfilingIsUnknown && dataTypeProfiling != "STRING_C" && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
			{
				ProcessException(ex3, DatabaseRow);
				stepFailed = true;
				navColumn.ErrorOccurred = true;
			}
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return stringFieldValuesStatistics;
	}

	private static async Task<StringFieldLenghtGeneralStatistics> GetStringFieldValuesLengthStatisticsAsync(ColumnNavigationObject navColumn, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, bool dataTypeProfilingIsUnknown, bool columnTypeIsNotSupported, double progressPerStep)
	{
		StringFieldLenghtGeneralStatistics stringFieldValuesLengthStatistics = null;
		bool stepFailed = false;
		try
		{
			stringFieldValuesLengthStatistics = await Task.Run(() => commandsSet.DataProfiling.GetStringFieldValuesLengthStatistics(navColumn.TableSchema, navColumn.TableName, navColumn.Name, cancellationTokenSource.Token));
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
		}
		catch (Exception ex3)
		{
			if (!dataTypeProfilingIsUnknown && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
			{
				ProcessException(ex3, DatabaseRow);
				stepFailed = true;
				navColumn.ErrorOccurred = true;
			}
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return stringFieldValuesLengthStatistics;
	}

	private static async Task<long> GetRecordsWithDefaultValueCountAsync(ColumnNavigationObject navColumn, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, double progressPerStep)
	{
		long recordsWithDefaultValueCount = 0L;
		bool stepFailed = false;
		try
		{
			recordsWithDefaultValueCount = await Task.Run(() => commandsSet.DataProfiling.GetRecordsWithDefaultValueCount(navColumn.TableSchema, navColumn.TableName, navColumn.Name, cancellationTokenSource.Token));
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
		}
		catch (Exception ex3)
		{
			ProcessException(ex3, DatabaseRow);
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return recordsWithDefaultValueCount;
	}

	private static async Task<long?> GetRecordsWithUniqueValueCountAsync(ColumnNavigationObject navColumn, string dataTypeProfiling, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, bool dataTypeProfilingIsUnknown, bool columnTypeIsNotSupported, double progressPerStep)
	{
		long? recordsWithUniqueValueCount = null;
		bool stepFailed = false;
		try
		{
			recordsWithUniqueValueCount = await Task.Run(() => commandsSet.DataProfiling.GetRecordsWithUniqueValueCount(navColumn.TableSchema, navColumn.TableName, navColumn.Name, cancellationTokenSource.Token));
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
		}
		catch (Exception ex3)
		{
			if (!dataTypeProfilingIsUnknown && dataTypeProfiling != "STRING_C" && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
			{
				ProcessException(ex3, DatabaseRow);
				stepFailed = true;
				navColumn.ErrorOccurred = true;
			}
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return recordsWithUniqueValueCount;
	}

	private static async Task<IEnumerable<StringValueWithCount>> GetFieldMostCommonlyUsedValuesAsync(ColumnNavigationObject navColumn, int? maxResults, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, bool dataTypeProfilingIsUnknown, bool columnTypeIsNotSupported, string dataTypeProfiling, double progressPerStep)
	{
		IEnumerable<StringValueWithCount> fieldMostCommonlyUsedValues = null;
		bool stepFailed = false;
		bool isTopOrAll = navColumn.ValuesListMode == "T" || navColumn.ValuesListMode == "R";
		if ((dataTypeProfilingIsUnknown || dataTypeProfiling == "STRING" || dataTypeProfiling == "STRING_C") && isTopOrAll && !navColumn.IsEncrypted)
		{
			try
			{
				fieldMostCommonlyUsedValues = await Task.Run(() => commandsSet.DataProfiling.GetStringFieldMostCommonlyUsedValues(navColumn.TableSchema, navColumn.TableName, navColumn.Name, maxResults, cancellationTokenSource.Token));
				fieldMostCommonlyUsedValues = fieldMostCommonlyUsedValues.Where((StringValueWithCount v) => v?.Value != null);
			}
			catch (OperationCanceledException)
			{
				throw new OperationCanceledException();
			}
			catch (DatasourceOperationNotSupportedException)
			{
			}
			catch (Exception ex3)
			{
				if (!dataTypeProfilingIsUnknown && dataTypeProfiling != "STRING_C" && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
				{
					ProcessException(ex3, DatabaseRow);
					stepFailed = true;
					navColumn.ErrorOccurred = true;
				}
			}
		}
		if (fieldMostCommonlyUsedValues == null && isTopOrAll && !navColumn.IsEncrypted)
		{
			try
			{
				fieldMostCommonlyUsedValues = await Task.Run(() => commandsSet.DataProfiling.GetAnyFieldMostCommonlyUsedValues(navColumn.TableSchema, navColumn.TableName, navColumn.Name, maxResults, cancellationTokenSource.Token));
				fieldMostCommonlyUsedValues = fieldMostCommonlyUsedValues?.Where((StringValueWithCount v) => v?.Value != null);
			}
			catch (OperationCanceledException)
			{
				throw new OperationCanceledException();
			}
			catch (DatasourceOperationNotSupportedException)
			{
			}
			catch (Exception ex6)
			{
				if (!dataTypeProfilingIsUnknown && dataTypeProfiling != "STRING_C" && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
				{
					ProcessException(ex6, DatabaseRow);
					stepFailed = true;
					navColumn.ErrorOccurred = true;
				}
			}
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return fieldMostCommonlyUsedValues;
	}

	private static async Task<IEnumerable<IntValueWithCount>> GetStringValuesLenghtsAsync(ColumnNavigationObject navColumn, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, bool dataTypeProfilingIsUnknown, bool columnTypeIsNotSupported, double progressPerStep)
	{
		IEnumerable<IntValueWithCount> stringValuesLenghts = null;
		bool stepFailed = false;
		try
		{
			stringValuesLenghts = await Task.Run(() => commandsSet.DataProfiling.GetStringValuesLenghts(navColumn.TableSchema, navColumn.TableName, navColumn.Name, cancellationTokenSource.Token));
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
		}
		catch (Exception ex3)
		{
			if (!dataTypeProfilingIsUnknown && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
			{
				ProcessException(ex3, DatabaseRow);
				stepFailed = true;
				navColumn.ErrorOccurred = true;
			}
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return stringValuesLenghts;
	}

	private static async Task<DateFieldGeneralStatistics> GetDateFieldValuesStatisticsAsync(ColumnNavigationObject navColumn, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, bool dataTypeProfilingIsUnknown, bool columnTypeIsNotSupported, double progressPerStep)
	{
		DateFieldGeneralStatistics dateFieldValuesStatistics = null;
		bool stepFailed = false;
		try
		{
			dateFieldValuesStatistics = await Task.Run(() => commandsSet.DataProfiling.GetDateFieldValuesStatistics(navColumn.TableSchema, navColumn.TableName, navColumn.Name, cancellationTokenSource.Token));
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
		}
		catch (Exception ex3)
		{
			if (!dataTypeProfilingIsUnknown && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
			{
				ProcessException(ex3, DatabaseRow);
				stepFailed = true;
				navColumn.ErrorOccurred = true;
			}
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return dateFieldValuesStatistics;
	}

	private static async Task<IEnumerable<NullableIntValueWithCount>> GetDateValuesHoursAsync(ColumnNavigationObject navColumn, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, bool dataTypeProfilingIsUnknown, bool columnTypeIsNotSupported, double progressPerStep)
	{
		IEnumerable<NullableIntValueWithCount> dateValuesHours = null;
		bool stepFailed = false;
		try
		{
			dateValuesHours = await Task.Run(() => commandsSet.DataProfiling.GetDateValuesHours(navColumn.TableSchema, navColumn.TableName, navColumn.Name, cancellationTokenSource.Token));
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
		}
		catch (Exception ex3)
		{
			if (!dataTypeProfilingIsUnknown && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
			{
				ProcessException(ex3, DatabaseRow);
				stepFailed = true;
				navColumn.ErrorOccurred = true;
			}
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return dateValuesHours;
	}

	private static async Task<IEnumerable<DayOfWeekValueWithCount>> GetDateValuesDaysOfWeeksAsync(ColumnNavigationObject navColumn, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, bool dataTypeProfilingIsUnknown, bool columnTypeIsNotSupported, double progressPerStep)
	{
		IEnumerable<DayOfWeekValueWithCount> dateValuesDayOfWeeks = null;
		bool stepFailed = false;
		try
		{
			dateValuesDayOfWeeks = await Task.Run(() => commandsSet.DataProfiling.GetDateValuesDayOfWeeks(navColumn.TableSchema, navColumn.TableName, navColumn.Name, cancellationTokenSource.Token));
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
		}
		catch (Exception ex3)
		{
			if (!dataTypeProfilingIsUnknown && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
			{
				ProcessException(ex3, DatabaseRow);
				stepFailed = true;
				navColumn.ErrorOccurred = true;
			}
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return dateValuesDayOfWeeks;
	}

	private static async Task<IEnumerable<NullableIntValueWithCount>> GetDateValuesYearsAsync(ColumnNavigationObject navColumn, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, bool dataTypeProfilingIsUnknown, bool columnTypeIsNotSupported, double progressPerStep)
	{
		IEnumerable<NullableIntValueWithCount> dateValuesYears = null;
		bool stepFailed = false;
		try
		{
			dateValuesYears = await Task.Run(() => commandsSet.DataProfiling.GetDateValuesYears(navColumn.TableSchema, navColumn.TableName, navColumn.Name, cancellationTokenSource.Token));
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
		}
		catch (Exception ex3)
		{
			if (!dataTypeProfilingIsUnknown && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
			{
				ProcessException(ex3, DatabaseRow);
				stepFailed = true;
				navColumn.ErrorOccurred = true;
			}
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return dateValuesYears;
	}

	private static async Task<IEnumerable<NullableIntValueWithCount>> GetDateValuesMonthsAsync(ColumnNavigationObject navColumn, CommandsSet commandsSet, CancellationTokenSource cancellationTokenSource, bool dataTypeProfilingIsUnknown, bool columnTypeIsNotSupported, double progressPerStep)
	{
		IEnumerable<NullableIntValueWithCount> dateValuesMonths = null;
		bool stepFailed = false;
		try
		{
			dateValuesMonths = await Task.Run(() => commandsSet.DataProfiling.GetDateValuesMonths(navColumn.TableSchema, navColumn.TableName, navColumn.Name, cancellationTokenSource.Token));
		}
		catch (OperationCanceledException)
		{
			throw new OperationCanceledException();
		}
		catch (DatasourceOperationNotSupportedException)
		{
		}
		catch (Exception ex3)
		{
			if (!dataTypeProfilingIsUnknown && !columnTypeIsNotSupported && !DataTypeChecker.IsTypeThatShouldNotFail(navColumn.Column.DataType))
			{
				ProcessException(ex3, DatabaseRow);
				stepFailed = true;
				navColumn.ErrorOccurred = true;
			}
		}
		if (!stepFailed)
		{
			navColumn.ExactCompletion += progressPerStep;
		}
		return dateValuesMonths;
	}

	public static async Task<long?> GetTableRowCounterAsync(string tableName, string schema, DatabaseRow databaseRow, CancellationToken cancellationToken)
	{
		ProfilingDatabaseTypeEnum.ProfilingDatabaseType profilingDatabaseTypeEnum = EnumToEnumChanger.GetProfilingDatabaseTypeEnum(databaseRow.Type);
		CommandsSet commandsSet = CommandsFactory.GetDbConnectionCommands(profilingDatabaseTypeEnum, databaseRow.ConnectionString, CommandsWithTimeout.Timeout);
		long? objectRowsCount = null;
		try
		{
			objectRowsCount = await Task.Run(() => commandsSet.DataProfiling.GetObjectRowsCount(schema, tableName, cancellationToken));
			return objectRowsCount;
		}
		catch (OperationCanceledException)
		{
			return objectRowsCount;
		}
		catch (DatasourceOperationNotSupportedException)
		{
			return objectRowsCount;
		}
		catch (Exception ex3)
		{
			ProcessException(ex3, DatabaseRow);
			return objectRowsCount;
		}
	}

	public static void GuessNavColumnProfilingDataType(ProfiledDataModel profiledDataModel, ColumnNavigationObject navColumn)
	{
		string profilingDataType = null;
		NumericFieldGeneralStatistics numericFieldValuesStatistics = profiledDataModel.NumericFieldValuesStatistics;
		if (numericFieldValuesStatistics == null || !numericFieldValuesStatistics.Minimum.HasValue)
		{
			NumericFieldGeneralStatistics numericFieldValuesStatistics2 = profiledDataModel.NumericFieldValuesStatistics;
			if (numericFieldValuesStatistics2 == null || !numericFieldValuesStatistics2.Maximum.HasValue)
			{
				NumericFieldGeneralStatistics numericFieldValuesStatistics3 = profiledDataModel.NumericFieldValuesStatistics;
				if (numericFieldValuesStatistics3 == null || !numericFieldValuesStatistics3.Average.HasValue)
				{
					NumericFieldGeneralStatistics numericFieldValuesStatistics4 = profiledDataModel.NumericFieldValuesStatistics;
					if (numericFieldValuesStatistics4 == null || !numericFieldValuesStatistics4.StandardDeviation.HasValue)
					{
						NumericFieldGeneralStatistics numericFieldValuesStatistics5 = profiledDataModel.NumericFieldValuesStatistics;
						if (numericFieldValuesStatistics5 == null || !numericFieldValuesStatistics5.Variance.HasValue)
						{
							DateFieldGeneralStatistics dateFieldGeneralStatistics = profiledDataModel.DateFieldGeneralStatistics;
							if (dateFieldGeneralStatistics != null && dateFieldGeneralStatistics.Minimum.HasValue)
							{
								DateFieldGeneralStatistics dateFieldGeneralStatistics2 = profiledDataModel.DateFieldGeneralStatistics;
								if (dateFieldGeneralStatistics2 != null && dateFieldGeneralStatistics2.Maximum.HasValue && profiledDataModel.DateValuesHours != null && profiledDataModel.DateValuesDayOfWeeks != null && profiledDataModel.DateValuesYears != null && profiledDataModel.DateValuesMonths != null)
								{
									navColumn.Column.ValueMinString = null;
									navColumn.Column.ValueMaxString = null;
									navColumn.Column.StringLengthMin = null;
									navColumn.Column.StringLengthMax = null;
									navColumn.Column.StringLengthAvg = null;
									navColumn.Column.StringLengthStddev = null;
									navColumn.Column.StringLengthVar = null;
									navColumn.Column.ValueMin = null;
									navColumn.Column.ValueMax = null;
									navColumn.Column.ValueAvg = null;
									navColumn.Column.ValueStddev = null;
									navColumn.Column.ValueVar = null;
									profilingDataType = "DATETIME";
									goto IL_06ca;
								}
							}
							if (profiledDataModel.StringFieldValuesStatistics?.Minimum == null && profiledDataModel.StringFieldValuesStatistics?.Maximum == null)
							{
								goto IL_0379;
							}
							StringFieldLenghtGeneralStatistics stringFieldValuesLengthStatistics = profiledDataModel.StringFieldValuesLengthStatistics;
							if (stringFieldValuesLengthStatistics == null || !stringFieldValuesLengthStatistics.Maximum.HasValue)
							{
								StringFieldLenghtGeneralStatistics stringFieldValuesLengthStatistics2 = profiledDataModel.StringFieldValuesLengthStatistics;
								if (stringFieldValuesLengthStatistics2 == null || !stringFieldValuesLengthStatistics2.Average.HasValue)
								{
									StringFieldLenghtGeneralStatistics stringFieldValuesLengthStatistics3 = profiledDataModel.StringFieldValuesLengthStatistics;
									if (stringFieldValuesLengthStatistics3 == null || !stringFieldValuesLengthStatistics3.StandardDeviation.HasValue)
									{
										StringFieldLenghtGeneralStatistics stringFieldValuesLengthStatistics4 = profiledDataModel.StringFieldValuesLengthStatistics;
										if (stringFieldValuesLengthStatistics4 == null || !stringFieldValuesLengthStatistics4.Variance.HasValue)
										{
											goto IL_0379;
										}
									}
								}
							}
							navColumn.Column.ValueMin = null;
							navColumn.Column.ValueMax = null;
							navColumn.Column.ValueAvg = null;
							navColumn.Column.ValueStddev = null;
							navColumn.Column.ValueVar = null;
							profilingDataType = "STRING";
							goto IL_06ca;
						}
					}
				}
			}
		}
		navColumn.Column.ValueMinString = null;
		navColumn.Column.ValueMaxString = null;
		navColumn.Column.StringLengthMin = null;
		navColumn.Column.StringLengthMax = null;
		navColumn.Column.StringLengthAvg = null;
		navColumn.Column.StringLengthStddev = null;
		navColumn.Column.StringLengthVar = null;
		profilingDataType = "REAL";
		goto IL_06ca;
		IL_0379:
		StringFieldLenghtGeneralStatistics stringFieldValuesLengthStatistics5 = profiledDataModel.StringFieldValuesLengthStatistics;
		if (stringFieldValuesLengthStatistics5 == null || !stringFieldValuesLengthStatistics5.Maximum.HasValue)
		{
			StringFieldLenghtGeneralStatistics stringFieldValuesLengthStatistics6 = profiledDataModel.StringFieldValuesLengthStatistics;
			if (stringFieldValuesLengthStatistics6 == null || !stringFieldValuesLengthStatistics6.Average.HasValue)
			{
				StringFieldLenghtGeneralStatistics stringFieldValuesLengthStatistics7 = profiledDataModel.StringFieldValuesLengthStatistics;
				if (stringFieldValuesLengthStatistics7 == null || !stringFieldValuesLengthStatistics7.StandardDeviation.HasValue)
				{
					StringFieldLenghtGeneralStatistics stringFieldValuesLengthStatistics8 = profiledDataModel.StringFieldValuesLengthStatistics;
					if (stringFieldValuesLengthStatistics8 == null || !stringFieldValuesLengthStatistics8.Variance.HasValue)
					{
						DateFieldGeneralStatistics dateFieldGeneralStatistics3 = profiledDataModel.DateFieldGeneralStatistics;
						if (dateFieldGeneralStatistics3 != null && dateFieldGeneralStatistics3.Minimum.HasValue)
						{
							DateFieldGeneralStatistics dateFieldGeneralStatistics4 = profiledDataModel.DateFieldGeneralStatistics;
							if (dateFieldGeneralStatistics4 != null && dateFieldGeneralStatistics4.Maximum.HasValue && profiledDataModel.DateValuesYears != null && profiledDataModel.DateValuesMonths != null && profiledDataModel.DateValuesDayOfWeeks != null)
							{
								navColumn.Column.ValueMinString = null;
								navColumn.Column.ValueMaxString = null;
								navColumn.Column.StringLengthMin = null;
								navColumn.Column.StringLengthMax = null;
								navColumn.Column.StringLengthAvg = null;
								navColumn.Column.StringLengthStddev = null;
								navColumn.Column.StringLengthVar = null;
								navColumn.Column.ValueMin = null;
								navColumn.Column.ValueMax = null;
								navColumn.Column.ValueAvg = null;
								navColumn.Column.ValueStddev = null;
								navColumn.Column.ValueVar = null;
								profilingDataType = "DATE";
								goto IL_06ca;
							}
						}
						DateFieldGeneralStatistics dateFieldGeneralStatistics5 = profiledDataModel.DateFieldGeneralStatistics;
						if (dateFieldGeneralStatistics5 != null && dateFieldGeneralStatistics5.Minimum.HasValue)
						{
							DateFieldGeneralStatistics dateFieldGeneralStatistics6 = profiledDataModel.DateFieldGeneralStatistics;
							if (dateFieldGeneralStatistics6 != null && dateFieldGeneralStatistics6.Maximum.HasValue)
							{
								navColumn.Column.ValueMinString = null;
								navColumn.Column.ValueMaxString = null;
								navColumn.Column.StringLengthMin = null;
								navColumn.Column.StringLengthMax = null;
								navColumn.Column.StringLengthAvg = null;
								navColumn.Column.StringLengthStddev = null;
								navColumn.Column.StringLengthVar = null;
								navColumn.Column.ValueMin = null;
								navColumn.Column.ValueMax = null;
								navColumn.Column.ValueAvg = null;
								navColumn.Column.ValueStddev = null;
								navColumn.Column.ValueVar = null;
								profilingDataType = "TIME";
							}
						}
						goto IL_06ca;
					}
				}
			}
		}
		navColumn.Column.ValueMin = null;
		navColumn.Column.ValueMax = null;
		navColumn.Column.ValueAvg = null;
		navColumn.Column.ValueStddev = null;
		navColumn.Column.ValueVar = null;
		profilingDataType = "STRING_C";
		goto IL_06ca;
		IL_06ca:
		navColumn.Column.ProfilingDataType = profilingDataType;
		navColumn.ProfilingDataType = profilingDataType;
	}

	private static void ProcessException(Exception ex, DatabaseRow databaseRow)
	{
		StaticData.CrashedDatabaseType = databaseRow?.Type;
		StaticData.CrashedDBMSVersion = databaseRow?.DbmsVersion;
		GeneralExceptionHandling.Handle(ex, HandlingMethodEnumeration.HandlingMethod.LogInErrorLog);
		StaticData.ClearDatabaseInfoForCrashes();
	}

	public static DatabaseRow ReturnConnectedDatabaseRow(DatabaseRow databaseRow, int databaseId, SharedDatabaseTypeEnum.DatabaseType? databaseType, Form owner, SplashScreenManager splashScreenManager = null)
	{
		bool isSplashFormVisible = splashScreenManager.IsSplashFormVisible;
		CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true, "Connecting to database");
		try
		{
			if (databaseRow != null)
			{
				ConnectionResult connectionResult = databaseRow.TryConnection();
				if (connectionResult.IsSuccess && string.IsNullOrEmpty(connectionResult.Message))
				{
					if (!isSplashFormVisible)
					{
						CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
					}
					return databaseRow;
				}
			}
		}
		catch (Exception ex)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			ShowErrorMessageBox(ex, owner);
		}
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true, "Connecting to database");
			databaseRow = new DatabaseRow(DB.Database.GetDataById(databaseId));
		}
		catch (Exception ex2)
		{
			databaseRow = null;
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			ShowErrorMessageBox(ex2, owner);
		}
		if (databaseRow != null)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			if (DataProfilingUtils.IsConnectorInLicense(databaseType, owner))
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true, "Connecting to database");
				if (!string.IsNullOrEmpty(databaseRow.TryConnection().Message))
				{
					CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
					return ShowConnectionWindow(databaseId, databaseType);
				}
			}
			else
			{
				databaseRow = null;
			}
		}
		if (!isSplashFormVisible)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
		return databaseRow;
	}

	public static DatabaseRow ShowConnectionWindow(int databaseId, SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		using (ConnectToDatabaseWindowForm connectToDatabaseWindowForm = new ConnectToDatabaseWindowForm(databaseId, databaseType))
		{
			if (connectToDatabaseWindowForm.ShowDialog() == DialogResult.OK && connectToDatabaseWindowForm.CurrentDatabaseRow != null)
			{
				return connectToDatabaseWindowForm.CurrentDatabaseRow;
			}
		}
		return null;
	}

	public static void ShowErrorMessageBox(Exception ex, Form owner)
	{
		if (!(ex is SqlException ex2))
		{
			GeneralMessageBoxesHandling.Show("Unable to connect to repository!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, owner);
			return;
		}
		switch (ex2.Errors[0].Number)
		{
		case 53:
			GeneralMessageBoxesHandling.Show("Unable to connect to repository!\nCheck server name and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Errors[0].Message, 1, owner);
			break;
		case 4060:
			GeneralMessageBoxesHandling.Show("Unable to connect to repository!\nCheck repository name and try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Errors[0].Message, 1, owner);
			break;
		case 18452:
			GeneralMessageBoxesHandling.Show("Unable to connect to repository!\nCannot connect using windows credentials.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Errors[0].Message, 1, owner);
			break;
		case 18456:
			GeneralMessageBoxesHandling.Show("Unable to connect to repository!\nCheck login and password and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Errors[0].Message, 1, owner);
			break;
		default:
			GeneralMessageBoxesHandling.Show("Unable to connect to repository!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex2.Errors[0].Message, 1, owner);
			break;
		}
	}
}
