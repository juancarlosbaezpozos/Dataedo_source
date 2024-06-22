using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Dataedo.App.DataProfiling.Models;
using Dataedo.DataSources.Base.DataProfiling.Model;
using Dataedo.Model.Data.DataProfiling;

namespace Dataedo.App.DataProfiling.Tools;

public class ValuesTreeListPresenter
{
	public static BindingList<ColumnTopValues> ShowSavedInRepositoryValuesWhenListOfValuesIsNull(ColumnNavigationObject navigationColumn)
	{
		ColumnProfiledDataObject column = navigationColumn.Column;
		BindingList<ColumnTopValues> bindingList = new BindingList<ColumnTopValues>();
		long? num = 0L;
		bool flag = true;
		foreach (ColumnValuesDataObject listOfValue in navigationColumn.ListOfValues)
		{
			num += listOfValue.RowCount;
			if (listOfValue.Value == null || (listOfValue.Value.ToLower() == "NULL".ToLower() && flag))
			{
				flag = false;
			}
		}
		int? valuesListRowsCount = navigationColumn.ValuesListRowsCount;
		bool flag2 = column.ValuesListMode == "T";
		int num2 = navigationColumn.ListOfValues.Count();
		int num3 = 0;
		bool flag3 = true;
		if (!navigationColumn.ValuesListRowsCount.HasValue)
		{
			num3 = num2;
			flag3 = false;
		}
		else if (num2 < navigationColumn.ValuesListRowsCount)
		{
			num3 = num2;
			flag3 = false;
		}
		else
		{
			num3 = column.ValuesListRowsCount.GetValueOrDefault();
		}
		if (flag2)
		{
			if (flag3)
			{
				string text = DataProfilingStringFormatter.FormatTopIntValues(num3);
				string text2 = DataProfilingStringFormatter.FormatTopIntValues(column.ValuesUniqueValues.GetValueOrDefault());
				if (!column.ValuesUniqueValues.HasValue)
				{
					bindingList.Add(new ColumnTopValues("Top " + text + " values", num.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -1, -11));
				}
				else
				{
					bindingList.Add(new ColumnTopValues("Top " + text + " values (of " + text2 + ")", num.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -1, -11));
				}
			}
			else
			{
				string text3 = DataProfilingStringFormatter.FormatTopIntValues(num3);
				bindingList.Add(new ColumnTopValues("All " + text3 + " values", num.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -1, -11));
			}
		}
		else if (flag3)
		{
			string text4 = DataProfilingStringFormatter.FormatTopIntValues(valuesListRowsCount.GetValueOrDefault());
			bindingList.Add(new ColumnTopValues("All " + text4 + " values", num.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -1, -11));
		}
		else
		{
			string text5 = DataProfilingStringFormatter.FormatTopIntValues(num3);
			bindingList.Add(new ColumnTopValues("All " + text5 + " values", num.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -1, -11));
		}
		int num4 = 1;
		int num5 = 0;
		string value = "(empty)";
		foreach (ColumnValuesDataObject listOfValue2 in navigationColumn.ListOfValues)
		{
			if (listOfValue2 != null)
			{
				if (string.IsNullOrEmpty(listOfValue2.Value))
				{
					bindingList.Add(new ColumnTopValues(value, listOfValue2.RowCount.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), num4, -1));
				}
				else
				{
					bindingList.Add(new ColumnTopValues(listOfValue2.Value, listOfValue2.RowCount.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), num4, -1));
				}
				num4++;
			}
			else
			{
				num5++;
			}
		}
		if (flag2)
		{
			long valueOrDefault = (column.ValuesUniqueValues - navigationColumn.ListOfValues.Count() - num5).GetValueOrDefault();
			if (valueOrDefault > 0)
			{
				string text6 = DataProfilingStringFormatter.FormatTopIntValues(valueOrDefault);
				bindingList.Add(new ColumnTopValues("Bottom " + text6 + " values", (column.RowCount - num - column.ValuesNullRowCount).GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -2, -22));
			}
		}
		if (column.ValuesNullRowCount > 0)
		{
			bindingList.Add(new ColumnTopValues("NULL rows", column.ValuesNullRowCount.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -3, -33));
		}
		if (column.ValuesUniqueValues.HasValue && column.ValuesUniqueValues > 0)
		{
			string text7 = DataProfilingStringFormatter.FormatTopIntValues(column.ValuesUniqueValues.GetValueOrDefault());
			bindingList.Add(new ColumnTopValues("Total (" + text7 + " values)", column.RowCount.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -4, -44));
		}
		return bindingList;
	}

	public static BindingList<ColumnTopValues> LoadValuesFromSampleData(ColumnNavigationObject navColumn)
	{
		BindingList<ColumnTopValues> bindingList = new BindingList<ColumnTopValues>();
		SampleData objectSampleData = navColumn.ObjectSampleData;
		ColumnProfiledDataObject column = navColumn.Column;
		if (objectSampleData == null)
		{
			return bindingList;
		}
		int num = objectSampleData.Values.Count();
		int num2 = 0;
		num2 = ((!(num < navColumn.ValuesListRowsCount)) ? navColumn.ValuesListRowsCount.GetValueOrDefault() : num);
		string text = DataProfilingStringFormatter.FormatTopIntValues(num2);
		string text2 = string.Empty;
		if (column.ValuesUniqueValues.HasValue && column.ValuesUniqueValues != 0)
		{
			string text3 = DataProfilingStringFormatter.FormatTopIntValues(column.ValuesUniqueValues.GetValueOrDefault());
			text2 = " of (" + text3 + ")";
		}
		bindingList.Add(new ColumnTopValues(text + " random values" + text2, null, navColumn.RowsCount.GetValueOrDefault(), -1, -11));
		int num3 = 1;
		int num4 = 1;
		string value = "(empty)";
		List<ColumnTopValues> list = new List<ColumnTopValues>();
		object[][] values = objectSampleData.Values;
		foreach (object[] array in values)
		{
			if ((array == null || array.Count() != 0) && array[0] != null)
			{
				if (string.IsNullOrEmpty(Convert.ToString(array[0], CultureInfo.InvariantCulture)))
				{
					list.Add(new ColumnTopValues(value, null, column.RowCount.GetValueOrDefault(), num3, -1));
				}
				else
				{
					object obj = array[0];
					string empty = string.Empty;
					empty = ((!(obj is DateTime dateTime)) ? Convert.ToString(array[0], CultureInfo.InvariantCulture) : dateTime.ToString("yyyy-MM-dd hh:mm:ss tt"));
					list.Add(new ColumnTopValues(empty, null, column.RowCount.GetValueOrDefault(), num3, -1));
				}
				num3++;
			}
			else if (array == null || array.Count() != 0)
			{
				list.Add(new ColumnTopValues("null", null, column.RowCount.GetValueOrDefault(), num3, -1));
				num3++;
				num4++;
			}
		}
		list = list.OrderBy((ColumnTopValues x) => x.Value).ToList();
		foreach (ColumnTopValues item in list)
		{
			bindingList.Add(item);
		}
		num4--;
		return bindingList;
	}

	public static BindingList<ColumnTopValues> PopulateTopAllValuesList(ColumnNavigationObject navigationColumn)
	{
		IEnumerable<StringValueWithCount> fieldMostCommonlyUsedValues = navigationColumn.FieldMostCommonlyUsedValues;
		ColumnProfiledDataObject column = navigationColumn.Column;
		BindingList<ColumnTopValues> bindingList = new BindingList<ColumnTopValues>();
		int? num = 0;
		bool flag = true;
		foreach (StringValueWithCount item in fieldMostCommonlyUsedValues)
		{
			num += item.Count;
			if (item.Value == null || (item.Value.ToLower() == "NULL".ToLower() && flag))
			{
				flag = false;
			}
		}
		int? valuesListRowsCount = navigationColumn.ValuesListRowsCount;
		bool flag2 = column.ValuesListMode == "T";
		int num2 = fieldMostCommonlyUsedValues?.Count() ?? 0;
		int num3 = 0;
		bool flag3 = true;
		if (!navigationColumn.ValuesListRowsCount.HasValue)
		{
			num3 = num2;
			flag3 = false;
		}
		else if (num2 < navigationColumn.ValuesListRowsCount)
		{
			num3 = num2;
			flag3 = false;
		}
		else
		{
			num3 = valuesListRowsCount.GetValueOrDefault();
		}
		if (flag2)
		{
			if (flag3)
			{
				string text = DataProfilingStringFormatter.FormatTopIntValues(num3);
				string text2 = DataProfilingStringFormatter.FormatTopIntValues(column.ValuesUniqueValues.GetValueOrDefault());
				bindingList.Add(new ColumnTopValues("Top " + text + " values of (" + text2 + ")", num.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -1, -11));
			}
			else
			{
				string text3 = DataProfilingStringFormatter.FormatTopIntValues(num3);
				bindingList.Add(new ColumnTopValues("All " + text3 + " values", num.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -1, -11));
			}
		}
		else if (flag3)
		{
			string text4 = DataProfilingStringFormatter.FormatTopIntValues(valuesListRowsCount.GetValueOrDefault());
			bindingList.Add(new ColumnTopValues("All " + text4 + " values", num.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -1, -11));
		}
		else
		{
			string text5 = DataProfilingStringFormatter.FormatTopIntValues(num3);
			bindingList.Add(new ColumnTopValues("All " + text5 + " values", num.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -1, -11));
		}
		int num4 = 1;
		int num5 = 0;
		string value = "(empty)";
		foreach (StringValueWithCount item2 in fieldMostCommonlyUsedValues)
		{
			if (item2 != null)
			{
				if (string.IsNullOrEmpty(item2.Value))
				{
					bindingList.Add(new ColumnTopValues(value, item2.Count, column.RowCount.GetValueOrDefault(), num4, -1));
				}
				else
				{
					bindingList.Add(new ColumnTopValues(Convert.ToString(item2.Value, CultureInfo.InvariantCulture), item2.Count, column.RowCount.GetValueOrDefault(), num4, -1));
				}
				num4++;
			}
			else
			{
				num5++;
			}
		}
		if (flag2)
		{
			long valueOrDefault = (column.ValuesUniqueValues - fieldMostCommonlyUsedValues.Count() - num5).GetValueOrDefault();
			if (valueOrDefault > 0)
			{
				string text6 = DataProfilingStringFormatter.FormatTopIntValues(valueOrDefault);
				bindingList.Add(new ColumnTopValues("Bottom " + text6 + " values", (column.RowCount - num - column.ValuesNullRowCount).GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -2, -22));
			}
		}
		if (column.ValuesNullRowCount > 0)
		{
			bindingList.Add(new ColumnTopValues("NULL rows", column.ValuesNullRowCount.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -3, -33));
		}
		if (column.ValuesUniqueValues.HasValue && column.ValuesUniqueValues > 0)
		{
			string text7 = DataProfilingStringFormatter.FormatTopIntValues(column.ValuesUniqueValues.GetValueOrDefault());
			bindingList.Add(new ColumnTopValues("Total (" + text7 + " values)", column.RowCount.GetValueOrDefault(), column.RowCount.GetValueOrDefault(), -4, -44));
		}
		return bindingList;
	}

	public static BindingList<ColumnTopValues> ShowSampleSavedValues(ColumnNavigationObject navColumn)
	{
		ColumnProfiledDataObject column = navColumn.Column;
		BindingList<ColumnTopValues> bindingList = new BindingList<ColumnTopValues>();
		int num = navColumn.ListOfValues.Count();
		int num2 = 0;
		num2 = ((!(num <= navColumn.ValuesListRowsCount)) ? navColumn.ValuesListRowsCount.GetValueOrDefault() : num);
		string text = DataProfilingStringFormatter.FormatTopIntValues(num2);
		string text2 = string.Empty;
		if (column.ValuesUniqueValues.HasValue && column.ValuesUniqueValues != 0)
		{
			string text3 = DataProfilingStringFormatter.FormatTopIntValues(column.ValuesUniqueValues.GetValueOrDefault());
			text2 = " of (" + text3 + ")";
		}
		bindingList.Add(new ColumnTopValues(text + " random values" + text2, null, navColumn.RowsCount.GetValueOrDefault(), -1, -11));
		int num3 = 1;
		int num4 = 1;
		string value = "empty";
		List<ColumnTopValues> list = new List<ColumnTopValues>();
		foreach (ColumnValuesDataObject listOfValue in navColumn.ListOfValues)
		{
			if (listOfValue != null && listOfValue.Value != null && listOfValue.Value != "NULL")
			{
				if (string.IsNullOrEmpty(Convert.ToString(listOfValue.Value, CultureInfo.InvariantCulture)))
				{
					list.Add(new ColumnTopValues(value, null, column.RowCount.GetValueOrDefault(), num3, -1));
				}
				else
				{
					list.Add(new ColumnTopValues(Convert.ToString(listOfValue.Value, CultureInfo.InvariantCulture), null, column.RowCount.GetValueOrDefault(), num3, -1));
				}
				num3++;
			}
			else
			{
				list.Add(new ColumnTopValues("null", null, column.RowCount.GetValueOrDefault(), num3, -1));
				num3++;
				num4++;
			}
		}
		list = list.OrderBy((ColumnTopValues x) => x.Value).ToList();
		foreach (ColumnTopValues item in list)
		{
			bindingList.Add(item);
		}
		num4--;
		return bindingList;
	}
}
