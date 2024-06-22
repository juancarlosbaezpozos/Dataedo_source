using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.DataSources.Enums;
using Dataedo.Model.Data.DataProfiling;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.DataProfiling.DataProfilingUserControls;

public class ValuesSectionUserControl : BaseUserControl
{
	private readonly Stream layoutStream;

	private ColumnProfiledDataObject columnProfiledDataObject;

	private ProfilingDatabaseTypeEnum.ProfilingDatabaseType? databaseType;

	private IContainer components;

	private NonCustomizableLayoutControl valuesUserControlLayoutControl;

	private LayoutControlGroup Root;

	private LabelControl stringValuesLabelControl;

	private LayoutControlItem stringValuesLabelControlLayoutControlItem;

	private LabelControl stringValuesMinTextLabelControl;

	private LayoutControlItem stringValuesMinTextLabelControlLayoutControlItem;

	private LabelControl stringValuesMaxTextLabelControl;

	private LayoutControlItem stringValuesMaxTextLabelControlLayoutControlItem;

	private LabelControl minimumDateTextLabelControl;

	private LayoutControlItem minimumDateTextLabelControlLayoutControlItem;

	private LabelControl maximumDateTextLabelControl;

	private LayoutControlItem maximumDateTextLabelControlLayoutControlItem;

	private LabelControl dateSpanTextLabelControl;

	private LayoutControlItem dateSpanTextLabelControlLayoutControlItem;

	private LabelControl stringValuesDistinctTextLabelControl;

	private LayoutControlItem stringValuesDistinctTextLabelControlLayoutControlItem;

	private LabelControl valuesOrStringLengthLabelControl;

	private LayoutControlItem valuesOrStringLengthLabelControlLayoutControlItem;

	private LabelControl minimumTextLabelControl;

	private LayoutControlItem minimumTextLabelControlLayoutControlItem;

	private LabelControl maximumTextLabelControl;

	private LayoutControlItem maximumTextLabelControlLayoutControlItem;

	private LabelControl averageTextLabelControl;

	private LayoutControlItem averageTextLabelControlLayoutControlItem;

	private LabelControl numericSpanTextLabelControl;

	private LayoutControlItem numericSpanTextLabelControlLayoutControlItem;

	private LabelControl varianceTextLabelControl;

	private LayoutControlItem varianceTextLabelControlLayoutControlItem;

	private LabelControl stddevTextLabelControl;

	private LayoutControlItem stddevTextLabelControlLayoutControlItem;

	private LabelControl distinctValuesTextLabelControl;

	private LayoutControlItem distinctValuesTextLabelControlLayoutControlItem;

	private LabelControl distinctValuesValueLabelControl;

	private LayoutControlItem distinctValuesValueLabelControlLayoutControlItem;

	private LabelControl varianceValueLabelControl;

	private LayoutControlItem varianceValueLabelControlLayoutControlItem;

	private LabelControl averageValueLabelControl;

	private LayoutControlItem averageValueLabelControlLayoutControlItem;

	private LabelControl dateSpanValueLabelControl;

	private LayoutControlItem dateSpanValueLabelControlLayoutControlItem;

	private LabelControl minimumDateValueLabelControl;

	private LayoutControlItem minimumDateValueLabelControlLayoutControlItem;

	private LabelControl maximumDateValueLabelControl;

	private LayoutControlItem maximumDateValueLabelControlLayoutControlItem;

	private LabelControl stringValuesDistinctValueLabelControl;

	private LayoutControlItem stringValuesDistinctValueLabelControlLayoutControlItem;

	private LabelControl minimumValueLabelControl;

	private LayoutControlItem minimumValueLabelControlLayoutControlItem;

	private LabelControl numericSpanValueLabelControl;

	private LayoutControlItem numericSpanValueLabelControlLayoutControlItem;

	private LabelControl stringValuesMaxValueLabelControl;

	private LayoutControlItem stringValuesMaxValueLabelControlLayoutControlItem;

	private LabelControl stringValuesMinValueLabelControl;

	private LayoutControlItem stringValuesMinValueLabelControlLayoutControlItem;

	private LabelControl maximumValueLabelControl;

	private LayoutControlItem maximumValueLabelControlLayoutControlItem;

	private LabelControl stddevValueLabelControl;

	private LayoutControlItem stddevValueLabelControlLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	public ValuesSectionUserControl()
	{
		InitializeComponent();
		layoutStream = new MemoryStream();
		valuesUserControlLayoutControl.SaveLayoutToStream(layoutStream);
	}

	public void SetParameters(ColumnProfiledDataObject columnProfiledDataObject, ProfilingDatabaseTypeEnum.ProfilingDatabaseType databaseTypeEnum)
	{
		this.columnProfiledDataObject = columnProfiledDataObject;
		databaseType = databaseTypeEnum;
		LoadCenterValues();
	}

	private void LoadCenterValues()
	{
		string profilingDataType = this.columnProfiledDataObject.ProfilingDataType;
		ColumnProfiledDataObject columnProfiledDataObject = this.columnProfiledDataObject;
		if (columnProfiledDataObject.ProfilingDataType == null)
		{
			columnProfiledDataObject.ProfilingDataType = DataTypeChecker.GetProfilingDataType(profilingDataType, databaseType.Value);
		}
		if (columnProfiledDataObject.ProfilingDataType != "STRING" && columnProfiledDataObject.ProfilingDataType != "STRING_C" && columnProfiledDataObject.ProfilingDataType != "INT" && columnProfiledDataObject.ProfilingDataType != "REAL" && columnProfiledDataObject.ProfilingDataType != "DATETIME" && columnProfiledDataObject.ProfilingDataType != "DATE")
		{
			columnProfiledDataObject.ProfilingDataType = DataTypeChecker.GetProfilingDataType(profilingDataType, databaseType.Value);
		}
		if (columnProfiledDataObject.ProfilingDataType == "STRING" || columnProfiledDataObject.ProfilingDataType == "STRING_C")
		{
			stringValuesLabelControl.Text = "Values";
			valuesOrStringLengthLabelControl.Text = "String length";
			SetTextAndEnableDisableForValueMinString(columnProfiledDataObject.ValueMinString);
			SetTextAndEnableDisableForValueMaxString(columnProfiledDataObject.ValueMaxString);
			SetTextAndEnableDisableForValuesUniqueValues(columnProfiledDataObject.ValuesUniqueValues);
			SetTextAndEnableDisableForStringLengthMin(columnProfiledDataObject.StringLengthMin);
			SetTextAndEnableDisableForStringLengthMax(columnProfiledDataObject.StringLengthMax);
			SetTextAndEnableDisableForStringLengthAvg(columnProfiledDataObject.StringLengthAvg);
			SetTextAndEnableDisableForStringLengthStddev(columnProfiledDataObject.StringLengthStddev);
			SetTextAndEnableDisableForStringLengthVar(columnProfiledDataObject.StringLengthVar);
			HideAllInCenter(columnProfiledDataObject);
		}
		else if (columnProfiledDataObject.ProfilingDataType == "INT")
		{
			valuesOrStringLengthLabelControl.Text = "Values";
			SetTextAndEnableDisableForValueMin(columnProfiledDataObject.ValueMin);
			SetTextAndEnableDisableForValueMax(columnProfiledDataObject.ValueMax);
			SetTextAndEnableDisableForValueAvg(columnProfiledDataObject.ValueAvg);
			SetTextAndEnableDisableForalueStddev(columnProfiledDataObject.ValueStddev);
			SetTextAndEnableDisableForValueVar(columnProfiledDataObject.ValueVar);
			SetTextAndEnableDisableForValuesUniqueValues2(columnProfiledDataObject.ValuesUniqueValues);
			bool showNumericSpan = true;
			SetTextAndEnableDisableForNumericSpan(columnProfiledDataObject.ValueMin, columnProfiledDataObject.ValueMax);
			HideAllInCenter(columnProfiledDataObject, showNumericSpan);
		}
		else if (columnProfiledDataObject.ProfilingDataType == "REAL")
		{
			valuesOrStringLengthLabelControl.Text = "Values";
			SetTextAndEnableDisableForValueMin(columnProfiledDataObject.ValueMin);
			SetTextAndEnableDisableForValueMax(columnProfiledDataObject.ValueMax);
			SetTextAndEnableDisableForValueAvg(columnProfiledDataObject.ValueAvg);
			SetTextAndEnableDisableForalueStddev(columnProfiledDataObject.ValueStddev);
			SetTextAndEnableDisableForValueVar(columnProfiledDataObject.ValueVar);
			SetTextAndEnableDisableForValuesUniqueValues2(columnProfiledDataObject.ValuesUniqueValues);
			SetTextAndEnableDisableForNumericSpan(columnProfiledDataObject.ValueMin, columnProfiledDataObject.ValueMax);
			bool showNumericSpan2 = true;
			HideAllInCenter(columnProfiledDataObject, showNumericSpan2);
		}
		else if (columnProfiledDataObject.ProfilingDataType == "DATETIME" || columnProfiledDataObject.ProfilingDataType == "DATE")
		{
			valuesOrStringLengthLabelControl.Text = "Values";
			string minDate = string.Empty;
			string maxDate = string.Empty;
			SetFormatForDateOrDateTime(columnProfiledDataObject, out minDate, out maxDate);
			SetTextAndEnableDisableForMinDate(minDate);
			SetTextAndEnableDisableFoMaxDate(maxDate);
			SetTextAndEnableDisableForValuesUniqueValues(columnProfiledDataObject.ValuesUniqueValues);
			SetTextAndEnableDisableForDataSpan(columnProfiledDataObject, minDate, maxDate);
			bool showDateSpan = true;
			HideAllInCenter(columnProfiledDataObject, showNumericSpan: false, showDateSpan);
		}
		else
		{
			HideAllInCenter();
		}
	}

	private void SetTextAndEnableDisableForDataSpan(ColumnProfiledDataObject column, string minDate, string maxDate)
	{
		if (minDate != null && maxDate != null && minDate != maxDate)
		{
			TimeSpan? timeSpan = column.ValueMaxDate - column.ValueMinDate;
			DateTime dateTime = new DateTime(1, 1, 1);
			dateSpanTextLabelControl.Text = "Span";
			DateTime value = dateTime;
			TimeSpan? timeSpan2 = timeSpan;
			double num = (double)(value + timeSpan2).Value.Year - 1.0;
			value = dateTime;
			timeSpan2 = timeSpan;
			double num2 = (double)(value + timeSpan2).Value.Month - 1.0;
			value = dateTime;
			timeSpan2 = timeSpan;
			double num3 = (double)(value + timeSpan2).Value.Day - 1.0;
			if (num != 0.0)
			{
				if (num2 != 0.0 && num2 != 12.0)
				{
					num += 1.0 * num2 / 12.0;
				}
				dateSpanValueLabelControl.Text = DataProfilingStringFormatter.FormatFloat1Values(num) + " years";
			}
			else if (num2 != 0.0)
			{
				int num4 = DateTime.DaysInMonth(column.ValueMinDate.Value.Year, (int)num2);
				if (num3 != 0.0 && num3 != (double)num4)
				{
					num2 = (double)timeSpan.Value.Days / 30.5;
				}
				dateSpanValueLabelControl.Text = DataProfilingStringFormatter.FormatFloat1Values(Math.Truncate(num2 * 10.0) / 10.0) + " months";
			}
			else if (num3 != 0.0)
			{
				if (timeSpan.Value.Hours < 12)
				{
					LabelControl labelControl = dateSpanValueLabelControl;
					value = dateTime;
					timeSpan2 = timeSpan;
					labelControl.Text = $"{(value + timeSpan2).Value.Day} days";
				}
				else
				{
					LabelControl labelControl2 = dateSpanValueLabelControl;
					value = dateTime;
					timeSpan2 = timeSpan;
					labelControl2.Text = $"{(value + timeSpan2).Value.Day - 1} days";
				}
			}
			else if (timeSpan.Value.Hours != 0)
			{
				dateSpanValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(timeSpan.Value.Hours) + " hours";
			}
			else if (timeSpan.Value.Minutes != 0)
			{
				dateSpanValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(timeSpan.Value.Minutes) + " minutes";
			}
			else if (timeSpan.Value.Seconds != 0)
			{
				dateSpanValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(timeSpan.Value.Seconds) + " seconds";
			}
			else if (timeSpan.Value.Milliseconds != 0)
			{
				dateSpanValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(timeSpan.Value.Milliseconds) + " milliseconds";
			}
			dateSpanTextLabelControl.Text = "Span";
			dateSpanTextLabelControl.Enabled = true;
			dateSpanValueLabelControl.Enabled = true;
		}
		else
		{
			dateSpanTextLabelControl.Text = "Span";
			dateSpanTextLabelControl.Enabled = false;
			dateSpanValueLabelControl.Text = "-";
			dateSpanValueLabelControl.Enabled = false;
		}
	}

	private void SetFormatForDateOrDateTime(ColumnProfiledDataObject column, out string minDate, out string maxDate)
	{
		if (column.ProfilingDataType == "DATE")
		{
			minDate = DataProfilingStringFormatter.FormatDateToRepositoryFormat(column?.ValueMinDate, "DATE");
			maxDate = DataProfilingStringFormatter.FormatDateToRepositoryFormat(column?.ValueMaxDate, "DATE");
		}
		else
		{
			minDate = DataProfilingStringFormatter.FormatDateToRepositoryFormat(column?.ValueMinDate);
			maxDate = DataProfilingStringFormatter.FormatDateToRepositoryFormat(column?.ValueMaxDate);
		}
	}

	private void SetTextAndEnableDisableForMinDate(string minDate)
	{
		if (!string.IsNullOrEmpty(minDate))
		{
			minimumDateTextLabelControl.Text = "Min";
			minimumDateTextLabelControl.Enabled = true;
			minimumDateValueLabelControl.Text = minDate ?? "";
			minimumDateValueLabelControl.Enabled = true;
		}
		else
		{
			minimumDateTextLabelControl.Text = "Min";
			minimumDateTextLabelControl.Enabled = false;
			minimumDateValueLabelControl.Text = "-";
			minimumDateValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableFoMaxDate(string maxDate)
	{
		if (!string.IsNullOrEmpty(maxDate))
		{
			maximumDateTextLabelControl.Text = "Max";
			maximumDateTextLabelControl.Enabled = true;
			maximumDateValueLabelControl.Text = maxDate ?? "";
			maximumDateValueLabelControl.Enabled = true;
		}
		else
		{
			maximumDateTextLabelControl.Text = "Max";
			maximumDateTextLabelControl.Enabled = false;
			maximumDateValueLabelControl.Text = "-";
			maximumDateValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForNumericSpan(double? valueMin, double? valueMax)
	{
		if (valueMin.HasValue && valueMax.HasValue && valueMin != valueMax)
		{
			double? number = valueMax - valueMin;
			numericSpanTextLabelControl.Text = "Span";
			numericSpanTextLabelControl.Enabled = true;
			numericSpanValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(number);
			numericSpanValueLabelControl.Enabled = true;
		}
		else
		{
			numericSpanTextLabelControl.Text = "Span";
			numericSpanTextLabelControl.Enabled = false;
			numericSpanValueLabelControl.Text = "-";
			numericSpanValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForValuesUniqueValues2(long? valuesUniqueValues)
	{
		if (valuesUniqueValues.HasValue)
		{
			distinctValuesTextLabelControl.Text = "Distinct values";
			distinctValuesTextLabelControl.Enabled = true;
			distinctValuesValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(valuesUniqueValues) ?? "";
			distinctValuesValueLabelControl.Enabled = true;
		}
		else
		{
			distinctValuesTextLabelControl.Text = "Distinct values";
			distinctValuesTextLabelControl.Enabled = false;
			distinctValuesValueLabelControl.Text = "-";
			distinctValuesValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForValueVar(double? valueVar)
	{
		if (valueVar.HasValue)
		{
			varianceTextLabelControl.Text = "Variance";
			varianceTextLabelControl.Enabled = true;
			varianceValueLabelControl.Text = DataProfilingStringFormatter.FormatFloat2Values(valueVar);
			varianceValueLabelControl.Enabled = true;
		}
		else
		{
			varianceTextLabelControl.Text = "Variance";
			varianceTextLabelControl.Enabled = false;
			varianceValueLabelControl.Text = "-";
			varianceValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForalueStddev(double? valueStddev)
	{
		if (valueStddev.HasValue)
		{
			stddevTextLabelControl.Text = "Standard deviation";
			stddevTextLabelControl.Enabled = true;
			stddevValueLabelControl.Text = DataProfilingStringFormatter.FormatFloat2Values(valueStddev);
			stddevValueLabelControl.Enabled = true;
		}
		else
		{
			stddevTextLabelControl.Text = "Standard deviation";
			stddevTextLabelControl.Enabled = false;
			stddevValueLabelControl.Text = "-";
			stddevValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForValueAvg(double? valueAvg)
	{
		if (valueAvg.HasValue)
		{
			averageTextLabelControl.Text = "Avg";
			averageTextLabelControl.Enabled = true;
			averageValueLabelControl.Text = DataProfilingStringFormatter.FormatFloat2Values(valueAvg);
			averageValueLabelControl.Enabled = true;
		}
		else
		{
			averageTextLabelControl.Text = "Avg";
			averageTextLabelControl.Enabled = false;
			averageValueLabelControl.Text = "-";
			averageValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForValueMax(double? valueMax)
	{
		if (valueMax.HasValue)
		{
			maximumTextLabelControlLayoutControlItem.Text = "Max";
			maximumTextLabelControlLayoutControlItem.Enabled = true;
			maximumValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(valueMax);
			maximumValueLabelControl.Enabled = true;
		}
		else
		{
			maximumTextLabelControlLayoutControlItem.Text = "Max";
			maximumTextLabelControlLayoutControlItem.Enabled = false;
			maximumValueLabelControl.Text = "-";
			maximumValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForValueMin(double? valueMin)
	{
		if (valueMin.HasValue)
		{
			minimumTextLabelControl.Text = "Min";
			minimumTextLabelControl.Enabled = true;
			minimumValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(valueMin);
			minimumValueLabelControl.Enabled = true;
		}
		else
		{
			minimumTextLabelControl.Text = "Min";
			minimumTextLabelControl.Enabled = false;
			minimumValueLabelControl.Text = "-";
			minimumValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForValuesUniqueValues(long? valuesUniqueValues)
	{
		if (valuesUniqueValues.HasValue)
		{
			stringValuesDistinctTextLabelControl.Text = "Distinct values";
			stringValuesDistinctTextLabelControl.Enabled = true;
			stringValuesDistinctValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(valuesUniqueValues) ?? "";
			stringValuesDistinctValueLabelControl.Enabled = true;
		}
		else
		{
			stringValuesDistinctTextLabelControl.Text = "Distinct values";
			stringValuesDistinctTextLabelControl.Enabled = false;
			stringValuesDistinctValueLabelControl.Text = "-";
			stringValuesDistinctValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForValueMaxString(string valueMaxString)
	{
		if (valueMaxString != null)
		{
			stringValuesMaxTextLabelControl.Text = "Max";
			stringValuesMaxTextLabelControl.Enabled = true;
			stringValuesMaxValueLabelControl.Text = valueMaxString ?? "";
			stringValuesMaxValueLabelControl.Enabled = true;
		}
		else
		{
			stringValuesMaxTextLabelControl.Text = "Max";
			stringValuesMaxTextLabelControl.Enabled = false;
			stringValuesMaxValueLabelControl.Text = "-";
			stringValuesMaxValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForValueMinString(string valueMinString)
	{
		if (valueMinString != null)
		{
			stringValuesMinTextLabelControl.Text = "Min";
			stringValuesMinTextLabelControl.Enabled = true;
			stringValuesMinValueLabelControl.Text = valueMinString ?? "";
			stringValuesMinValueLabelControl.Enabled = true;
		}
		else
		{
			stringValuesMinTextLabelControl.Text = "Min";
			stringValuesMinTextLabelControl.Enabled = false;
			stringValuesMinValueLabelControl.Text = "-";
			stringValuesMinValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForStringLengthVar(double? stringLengthVar)
	{
		if (stringLengthVar.HasValue)
		{
			varianceTextLabelControl.Text = "Variance";
			varianceTextLabelControl.Enabled = true;
			varianceValueLabelControl.Text = DataProfilingStringFormatter.FormatFloat2Values(stringLengthVar);
			varianceValueLabelControl.Enabled = true;
		}
		else
		{
			varianceTextLabelControl.Enabled = false;
			varianceTextLabelControl.Text = "Variance";
			varianceValueLabelControl.Text = "-";
			varianceValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForStringLengthStddev(double? stringLengthStddev)
	{
		if (stringLengthStddev.HasValue)
		{
			stddevTextLabelControl.Text = "Standard deviation";
			stddevTextLabelControl.Enabled = true;
			stddevValueLabelControl.Text = DataProfilingStringFormatter.FormatFloat2Values(stringLengthStddev);
			stddevValueLabelControl.Enabled = true;
		}
		else
		{
			stddevTextLabelControl.Text = "Standard deviation";
			stddevTextLabelControl.Enabled = false;
			stddevValueLabelControl.Text = "-";
			stddevValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForStringLengthAvg(double? stringLengthAvg)
	{
		if (stringLengthAvg.HasValue)
		{
			averageTextLabelControl.Text = "Avg";
			averageTextLabelControl.Enabled = true;
			averageValueLabelControl.Text = DataProfilingStringFormatter.FormatFloat2Values(stringLengthAvg);
			averageValueLabelControl.Enabled = true;
		}
		else
		{
			averageTextLabelControl.Text = "Avg";
			averageTextLabelControl.Enabled = false;
			averageValueLabelControl.Text = "-";
			averageValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForStringLengthMax(double? stringLengthMax)
	{
		if (stringLengthMax.HasValue)
		{
			maximumTextLabelControlLayoutControlItem.Text = "Max";
			maximumTextLabelControlLayoutControlItem.Enabled = true;
			maximumValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(stringLengthMax);
			maximumValueLabelControl.Enabled = true;
		}
		else
		{
			maximumTextLabelControlLayoutControlItem.Text = "Max";
			maximumTextLabelControlLayoutControlItem.Enabled = false;
			maximumValueLabelControl.Text = "-";
			maximumValueLabelControl.Enabled = false;
		}
	}

	private void SetTextAndEnableDisableForStringLengthMin(int? stringLengthMin)
	{
		if (stringLengthMin.HasValue)
		{
			minimumTextLabelControl.Text = "Min";
			minimumTextLabelControl.Enabled = true;
			minimumValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(stringLengthMin);
			minimumValueLabelControl.Enabled = true;
		}
		else
		{
			minimumTextLabelControl.Text = "Min";
			minimumTextLabelControl.Enabled = false;
			minimumValueLabelControl.Text = "-";
			minimumValueLabelControl.Enabled = false;
		}
	}

	private void HideAllInCenter()
	{
		valuesUserControlLayoutControl.BeginUpdate();
		layoutStream.Position = 0L;
		valuesUserControlLayoutControl.RestoreLayoutFromStream(layoutStream);
		distinctValuesTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		distinctValuesTextLabelControlLayoutControlItem.HideToCustomization();
		distinctValuesValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		distinctValuesValueLabelControlLayoutControlItem.HideToCustomization();
		stddevTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		stddevTextLabelControlLayoutControlItem.HideToCustomization();
		stddevValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		stddevValueLabelControlLayoutControlItem.HideToCustomization();
		varianceTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		varianceTextLabelControlLayoutControlItem.HideToCustomization();
		varianceValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		varianceValueLabelControlLayoutControlItem.HideToCustomization();
		averageTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		averageTextLabelControlLayoutControlItem.HideToCustomization();
		averageValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		averageValueLabelControlLayoutControlItem.HideToCustomization();
		numericSpanTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		numericSpanTextLabelControlLayoutControlItem.HideToCustomization();
		numericSpanValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		numericSpanValueLabelControlLayoutControlItem.HideToCustomization();
		maximumTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		maximumTextLabelControlLayoutControlItem.HideToCustomization();
		maximumValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		maximumValueLabelControlLayoutControlItem.HideToCustomization();
		minimumTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		minimumTextLabelControlLayoutControlItem.HideToCustomization();
		minimumValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		minimumValueLabelControlLayoutControlItem.HideToCustomization();
		valuesOrStringLengthLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		valuesOrStringLengthLabelControlLayoutControlItem.HideToCustomization();
		stringValuesDistinctTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		stringValuesDistinctTextLabelControlLayoutControlItem.HideToCustomization();
		stringValuesDistinctValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		stringValuesDistinctValueLabelControlLayoutControlItem.HideToCustomization();
		dateSpanTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		dateSpanTextLabelControlLayoutControlItem.HideToCustomization();
		dateSpanValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		dateSpanValueLabelControlLayoutControlItem.HideToCustomization();
		maximumDateTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		maximumDateTextLabelControlLayoutControlItem.HideToCustomization();
		maximumDateValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		maximumDateValueLabelControlLayoutControlItem.HideToCustomization();
		minimumDateTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		minimumDateTextLabelControlLayoutControlItem.HideToCustomization();
		minimumDateValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		minimumDateValueLabelControlLayoutControlItem.HideToCustomization();
		stringValuesMaxTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		stringValuesMaxTextLabelControlLayoutControlItem.HideToCustomization();
		stringValuesMaxValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		stringValuesMaxValueLabelControlLayoutControlItem.HideToCustomization();
		stringValuesMinTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		stringValuesMinTextLabelControlLayoutControlItem.HideToCustomization();
		stringValuesMinValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		stringValuesMinValueLabelControlLayoutControlItem.HideToCustomization();
		stringValuesLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		stringValuesLabelControlLayoutControlItem.HideToCustomization();
		valuesUserControlLayoutControl.EndUpdate();
	}

	private void HideAllInCenter(ColumnProfiledDataObject column, bool showNumericSpan = false, bool showDateSpan = false)
	{
		valuesUserControlLayoutControl.BeginUpdate();
		layoutStream.Position = 0L;
		valuesUserControlLayoutControl.RestoreLayoutFromStream(layoutStream);
		bool flag = DataTypeChecker.IsRealType(column.ProfilingDataType) || DataTypeChecker.IsINTType(column.ProfilingDataType);
		bool flag2 = DataTypeChecker.IsStringType(column.ProfilingDataType) || DataTypeChecker.IsStringCoreType(column.ProfilingDataType);
		bool num = DataTypeChecker.IsDateTimeType(column.ProfilingDataType) || DataTypeChecker.IsDateType(column.ProfilingDataType);
		if (DataTypeChecker.IsINTType(column.ProfilingDataType) && DataTypeChecker.IsRealType(column.ProfilingDataType))
		{
			distinctValuesTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			distinctValuesTextLabelControlLayoutControlItem.HideToCustomization();
			distinctValuesValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			distinctValuesValueLabelControlLayoutControlItem.HideToCustomization();
		}
		if (num)
		{
			distinctValuesTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			distinctValuesTextLabelControlLayoutControlItem.HideToCustomization();
			distinctValuesValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			distinctValuesValueLabelControlLayoutControlItem.HideToCustomization();
		}
		if (!flag2 && !flag)
		{
			varianceTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			varianceTextLabelControlLayoutControlItem.HideToCustomization();
			varianceValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			varianceValueLabelControlLayoutControlItem.HideToCustomization();
		}
		if (!flag2 && !flag)
		{
			stddevTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			stddevTextLabelControlLayoutControlItem.HideToCustomization();
			stddevValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			stddevValueLabelControlLayoutControlItem.HideToCustomization();
		}
		if (!flag2 && !flag)
		{
			averageTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			averageTextLabelControlLayoutControlItem.HideToCustomization();
			averageValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			averageValueLabelControlLayoutControlItem.HideToCustomization();
		}
		if (!showNumericSpan)
		{
			numericSpanTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			numericSpanTextLabelControlLayoutControlItem.HideToCustomization();
			numericSpanValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			numericSpanValueLabelControlLayoutControlItem.HideToCustomization();
		}
		if (!flag2 && !flag)
		{
			maximumTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			maximumTextLabelControlLayoutControlItem.HideToCustomization();
			maximumValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			maximumValueLabelControlLayoutControlItem.HideToCustomization();
		}
		if (!flag2 && !flag)
		{
			minimumTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			minimumTextLabelControlLayoutControlItem.HideToCustomization();
			minimumValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			minimumValueLabelControlLayoutControlItem.HideToCustomization();
		}
		if (!flag)
		{
			if (flag2)
			{
				distinctValuesTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
				distinctValuesTextLabelControlLayoutControlItem.HideToCustomization();
				distinctValuesValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
				distinctValuesValueLabelControlLayoutControlItem.HideToCustomization();
			}
			else
			{
				valuesOrStringLengthLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
				valuesOrStringLengthLabelControlLayoutControlItem.HideToCustomization();
			}
		}
		if (flag)
		{
			stringValuesDistinctTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			stringValuesDistinctTextLabelControlLayoutControlItem.HideToCustomization();
			stringValuesDistinctValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			stringValuesDistinctValueLabelControlLayoutControlItem.HideToCustomization();
		}
		if (!showDateSpan)
		{
			dateSpanTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			dateSpanTextLabelControlLayoutControlItem.HideToCustomization();
			dateSpanValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			dateSpanValueLabelControlLayoutControlItem.HideToCustomization();
		}
		if (!num)
		{
			maximumDateTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			maximumDateTextLabelControlLayoutControlItem.HideToCustomization();
			maximumDateValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			maximumDateValueLabelControlLayoutControlItem.HideToCustomization();
		}
		if (!num)
		{
			minimumDateTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			minimumDateTextLabelControlLayoutControlItem.HideToCustomization();
			minimumDateValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			minimumDateValueLabelControlLayoutControlItem.HideToCustomization();
		}
		if (!flag2)
		{
			stringValuesMaxTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			stringValuesMaxTextLabelControlLayoutControlItem.HideToCustomization();
			stringValuesMaxValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			stringValuesMaxValueLabelControlLayoutControlItem.HideToCustomization();
		}
		if (!flag2)
		{
			stringValuesMinTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			stringValuesMinTextLabelControlLayoutControlItem.HideToCustomization();
			stringValuesMinValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			stringValuesMinValueLabelControlLayoutControlItem.HideToCustomization();
		}
		if (flag)
		{
			stringValuesLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			stringValuesLabelControlLayoutControlItem.HideToCustomization();
		}
		valuesUserControlLayoutControl.EndUpdate();
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.valuesUserControlLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.distinctValuesValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.numericSpanValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.maximumDateValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.minimumValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.dateSpanValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.stddevValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.stringValuesDistinctValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.varianceValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.averageValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.minimumDateValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.stringValuesLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.stringValuesMinTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.maximumValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.stringValuesMinValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.stringValuesMaxTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.minimumDateTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.maximumDateTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.numericSpanTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.dateSpanTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.stringValuesDistinctTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.valuesOrStringLengthLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.minimumTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.distinctValuesTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.maximumTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.averageTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.stddevTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.varianceTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.stringValuesMaxValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.stringValuesLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.stringValuesMinTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.stringValuesMinValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.minimumDateTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.maximumDateTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.stringValuesDistinctTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.valuesOrStringLengthLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.minimumTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.averageTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.varianceTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.distinctValuesTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.dateSpanValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.dateSpanTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.minimumDateValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.maximumDateValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.stringValuesDistinctValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.minimumValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.stringValuesMaxValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.stringValuesMaxTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.maximumTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.stddevTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.numericSpanTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.numericSpanValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.maximumValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.averageValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.stddevValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.varianceValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.distinctValuesValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.valuesUserControlLayoutControl).BeginInit();
		this.valuesUserControlLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.stringValuesLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.stringValuesMinTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.stringValuesMinValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.minimumDateTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.maximumDateTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.stringValuesDistinctTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valuesOrStringLengthLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.minimumTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.averageTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.varianceTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.distinctValuesTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateSpanValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateSpanTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.minimumDateValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.maximumDateValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.stringValuesDistinctValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.minimumValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.stringValuesMaxValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.stringValuesMaxTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.maximumTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.stddevTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.numericSpanTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.numericSpanValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.maximumValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.averageValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.stddevValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.varianceValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.distinctValuesValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		base.SuspendLayout();
		this.valuesUserControlLayoutControl.AutoSize = true;
		this.valuesUserControlLayoutControl.Controls.Add(this.distinctValuesValueLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.numericSpanValueLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.maximumDateValueLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.minimumValueLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.dateSpanValueLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.stddevValueLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.stringValuesDistinctValueLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.varianceValueLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.averageValueLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.minimumDateValueLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.stringValuesLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.stringValuesMinTextLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.maximumValueLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.stringValuesMinValueLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.stringValuesMaxTextLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.minimumDateTextLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.maximumDateTextLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.numericSpanTextLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.dateSpanTextLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.stringValuesDistinctTextLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.valuesOrStringLengthLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.minimumTextLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.distinctValuesTextLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.maximumTextLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.averageTextLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.stddevTextLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.varianceTextLabelControl);
		this.valuesUserControlLayoutControl.Controls.Add(this.stringValuesMaxValueLabelControl);
		this.valuesUserControlLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.valuesUserControlLayoutControl.Name = "valuesUserControlLayoutControl";
		this.valuesUserControlLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1002, 201, 650, 400);
		this.valuesUserControlLayoutControl.Root = this.Root;
		this.valuesUserControlLayoutControl.Size = new System.Drawing.Size(345, 266);
		this.valuesUserControlLayoutControl.TabIndex = 0;
		this.valuesUserControlLayoutControl.Text = "layoutControl1";
		this.distinctValuesValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.distinctValuesValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.distinctValuesValueLabelControl.AutoEllipsis = true;
		this.distinctValuesValueLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.distinctValuesValueLabelControl.Location = new System.Drawing.Point(152, 251);
		this.distinctValuesValueLabelControl.Name = "distinctValuesValueLabelControl";
		this.distinctValuesValueLabelControl.Size = new System.Drawing.Size(191, 13);
		this.distinctValuesValueLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.distinctValuesValueLabelControl.TabIndex = 53;
		this.numericSpanValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.numericSpanValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.numericSpanValueLabelControl.AutoEllipsis = true;
		this.numericSpanValueLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.numericSpanValueLabelControl.Location = new System.Drawing.Point(152, 183);
		this.numericSpanValueLabelControl.Name = "numericSpanValueLabelControl";
		this.numericSpanValueLabelControl.Size = new System.Drawing.Size(191, 13);
		this.numericSpanValueLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.numericSpanValueLabelControl.TabIndex = 47;
		this.maximumDateValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.maximumDateValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.maximumDateValueLabelControl.AutoEllipsis = true;
		this.maximumDateValueLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.maximumDateValueLabelControl.Location = new System.Drawing.Point(152, 73);
		this.maximumDateValueLabelControl.MaximumSize = new System.Drawing.Size(217, 0);
		this.maximumDateValueLabelControl.Name = "maximumDateValueLabelControl";
		this.maximumDateValueLabelControl.Size = new System.Drawing.Size(191, 13);
		this.maximumDateValueLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.maximumDateValueLabelControl.TabIndex = 52;
		this.minimumValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.minimumValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.minimumValueLabelControl.AutoEllipsis = true;
		this.minimumValueLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.minimumValueLabelControl.Location = new System.Drawing.Point(152, 149);
		this.minimumValueLabelControl.Name = "minimumValueLabelControl";
		this.minimumValueLabelControl.Size = new System.Drawing.Size(191, 13);
		this.minimumValueLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.minimumValueLabelControl.TabIndex = 46;
		this.dateSpanValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.dateSpanValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.dateSpanValueLabelControl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.dateSpanValueLabelControl.AutoEllipsis = true;
		this.dateSpanValueLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.dateSpanValueLabelControl.Location = new System.Drawing.Point(152, 90);
		this.dateSpanValueLabelControl.Name = "dateSpanValueLabelControl";
		this.dateSpanValueLabelControl.Size = new System.Drawing.Size(191, 13);
		this.dateSpanValueLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.dateSpanValueLabelControl.TabIndex = 57;
		this.dateSpanValueLabelControl.Text = "###.0 years \r\n##.0 months \r\n## days (zaokr. 1,2->1, 1,7->2) \r\n## hours \r\n## minutes ";
		this.stddevValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.stddevValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.stddevValueLabelControl.AutoEllipsis = true;
		this.stddevValueLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.stddevValueLabelControl.Location = new System.Drawing.Point(152, 217);
		this.stddevValueLabelControl.Name = "stddevValueLabelControl";
		this.stddevValueLabelControl.Size = new System.Drawing.Size(191, 13);
		this.stddevValueLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.stddevValueLabelControl.TabIndex = 49;
		this.stddevValueLabelControl.Text = "text";
		this.stringValuesDistinctValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.stringValuesDistinctValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.stringValuesDistinctValueLabelControl.AutoEllipsis = true;
		this.stringValuesDistinctValueLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.stringValuesDistinctValueLabelControl.Location = new System.Drawing.Point(152, 107);
		this.stringValuesDistinctValueLabelControl.Name = "stringValuesDistinctValueLabelControl";
		this.stringValuesDistinctValueLabelControl.Size = new System.Drawing.Size(191, 13);
		this.stringValuesDistinctValueLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.stringValuesDistinctValueLabelControl.TabIndex = 45;
		this.varianceValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.varianceValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.varianceValueLabelControl.AutoEllipsis = true;
		this.varianceValueLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.varianceValueLabelControl.Location = new System.Drawing.Point(152, 234);
		this.varianceValueLabelControl.Name = "varianceValueLabelControl";
		this.varianceValueLabelControl.Size = new System.Drawing.Size(191, 13);
		this.varianceValueLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.varianceValueLabelControl.TabIndex = 50;
		this.averageValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.averageValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.averageValueLabelControl.AutoEllipsis = true;
		this.averageValueLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.averageValueLabelControl.Location = new System.Drawing.Point(152, 200);
		this.averageValueLabelControl.Name = "averageValueLabelControl";
		this.averageValueLabelControl.Size = new System.Drawing.Size(191, 13);
		this.averageValueLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.averageValueLabelControl.TabIndex = 48;
		this.minimumDateValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.minimumDateValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.minimumDateValueLabelControl.AutoEllipsis = true;
		this.minimumDateValueLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.minimumDateValueLabelControl.Location = new System.Drawing.Point(152, 56);
		this.minimumDateValueLabelControl.Name = "minimumDateValueLabelControl";
		this.minimumDateValueLabelControl.Size = new System.Drawing.Size(191, 13);
		this.minimumDateValueLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.minimumDateValueLabelControl.TabIndex = 51;
		this.stringValuesLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold);
		this.stringValuesLabelControl.Appearance.Options.UseFont = true;
		this.stringValuesLabelControl.Location = new System.Drawing.Point(2, 2);
		this.stringValuesLabelControl.Name = "stringValuesLabelControl";
		this.stringValuesLabelControl.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
		this.stringValuesLabelControl.Size = new System.Drawing.Size(146, 16);
		this.stringValuesLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.stringValuesLabelControl.TabIndex = 27;
		this.stringValuesLabelControl.Text = "Values";
		this.stringValuesMinTextLabelControl.Location = new System.Drawing.Point(2, 22);
		this.stringValuesMinTextLabelControl.Name = "stringValuesMinTextLabelControl";
		this.stringValuesMinTextLabelControl.Padding = new System.Windows.Forms.Padding(23, 0, 0, 0);
		this.stringValuesMinTextLabelControl.Size = new System.Drawing.Size(146, 13);
		this.stringValuesMinTextLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.stringValuesMinTextLabelControl.TabIndex = 26;
		this.stringValuesMinTextLabelControl.Text = "String min";
		this.maximumValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.maximumValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.maximumValueLabelControl.AutoEllipsis = true;
		this.maximumValueLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.maximumValueLabelControl.Location = new System.Drawing.Point(152, 166);
		this.maximumValueLabelControl.Name = "maximumValueLabelControl";
		this.maximumValueLabelControl.Size = new System.Drawing.Size(191, 13);
		this.maximumValueLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.maximumValueLabelControl.TabIndex = 47;
		this.stringValuesMinValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.stringValuesMinValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.stringValuesMinValueLabelControl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.stringValuesMinValueLabelControl.AutoEllipsis = true;
		this.stringValuesMinValueLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.stringValuesMinValueLabelControl.Location = new System.Drawing.Point(152, 22);
		this.stringValuesMinValueLabelControl.Name = "stringValuesMinValueLabelControl";
		this.stringValuesMinValueLabelControl.Size = new System.Drawing.Size(191, 13);
		this.stringValuesMinValueLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.stringValuesMinValueLabelControl.TabIndex = 43;
		this.stringValuesMaxTextLabelControl.Location = new System.Drawing.Point(2, 39);
		this.stringValuesMaxTextLabelControl.Name = "stringValuesMaxTextLabelControl";
		this.stringValuesMaxTextLabelControl.Padding = new System.Windows.Forms.Padding(23, 2, 2, 2);
		this.stringValuesMaxTextLabelControl.Size = new System.Drawing.Size(146, 13);
		this.stringValuesMaxTextLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.stringValuesMaxTextLabelControl.TabIndex = 25;
		this.stringValuesMaxTextLabelControl.Text = "String max";
		this.minimumDateTextLabelControl.Location = new System.Drawing.Point(2, 56);
		this.minimumDateTextLabelControl.Name = "minimumDateTextLabelControl";
		this.minimumDateTextLabelControl.Padding = new System.Windows.Forms.Padding(23, 0, 0, 0);
		this.minimumDateTextLabelControl.Size = new System.Drawing.Size(146, 13);
		this.minimumDateTextLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.minimumDateTextLabelControl.TabIndex = 21;
		this.minimumDateTextLabelControl.Text = "Minimum date";
		this.maximumDateTextLabelControl.Location = new System.Drawing.Point(2, 73);
		this.maximumDateTextLabelControl.Name = "maximumDateTextLabelControl";
		this.maximumDateTextLabelControl.Padding = new System.Windows.Forms.Padding(23, 2, 2, 2);
		this.maximumDateTextLabelControl.Size = new System.Drawing.Size(146, 13);
		this.maximumDateTextLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.maximumDateTextLabelControl.TabIndex = 22;
		this.maximumDateTextLabelControl.Text = "Maximum date";
		this.numericSpanTextLabelControl.Location = new System.Drawing.Point(2, 183);
		this.numericSpanTextLabelControl.Name = "numericSpanTextLabelControl";
		this.numericSpanTextLabelControl.Padding = new System.Windows.Forms.Padding(23, 0, 0, 0);
		this.numericSpanTextLabelControl.Size = new System.Drawing.Size(146, 13);
		this.numericSpanTextLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.numericSpanTextLabelControl.TabIndex = 55;
		this.numericSpanTextLabelControl.Text = "numericSpan";
		this.dateSpanTextLabelControl.Location = new System.Drawing.Point(2, 90);
		this.dateSpanTextLabelControl.Name = "dateSpanTextLabelControl";
		this.dateSpanTextLabelControl.Padding = new System.Windows.Forms.Padding(23, 0, 0, 0);
		this.dateSpanTextLabelControl.Size = new System.Drawing.Size(146, 13);
		this.dateSpanTextLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.dateSpanTextLabelControl.TabIndex = 54;
		this.dateSpanTextLabelControl.Text = "dateSpanLabel";
		this.stringValuesDistinctTextLabelControl.Location = new System.Drawing.Point(2, 107);
		this.stringValuesDistinctTextLabelControl.Name = "stringValuesDistinctTextLabelControl";
		this.stringValuesDistinctTextLabelControl.Padding = new System.Windows.Forms.Padding(23, 0, 0, 0);
		this.stringValuesDistinctTextLabelControl.Size = new System.Drawing.Size(146, 13);
		this.stringValuesDistinctTextLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.stringValuesDistinctTextLabelControl.TabIndex = 28;
		this.stringValuesDistinctTextLabelControl.Text = "Distinct values";
		this.valuesOrStringLengthLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold);
		this.valuesOrStringLengthLabelControl.Appearance.Options.UseFont = true;
		this.valuesOrStringLengthLabelControl.Location = new System.Drawing.Point(2, 129);
		this.valuesOrStringLengthLabelControl.Name = "valuesOrStringLengthLabelControl";
		this.valuesOrStringLengthLabelControl.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
		this.valuesOrStringLengthLabelControl.Size = new System.Drawing.Size(146, 16);
		this.valuesOrStringLengthLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.valuesOrStringLengthLabelControl.TabIndex = 16;
		this.valuesOrStringLengthLabelControl.Text = "Values:";
		this.minimumTextLabelControl.Location = new System.Drawing.Point(2, 149);
		this.minimumTextLabelControl.Name = "minimumTextLabelControl";
		this.minimumTextLabelControl.Padding = new System.Windows.Forms.Padding(23, 0, 0, 0);
		this.minimumTextLabelControl.Size = new System.Drawing.Size(146, 13);
		this.minimumTextLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.minimumTextLabelControl.TabIndex = 24;
		this.minimumTextLabelControl.Text = "Min";
		this.distinctValuesTextLabelControl.Location = new System.Drawing.Point(2, 251);
		this.distinctValuesTextLabelControl.Name = "distinctValuesTextLabelControl";
		this.distinctValuesTextLabelControl.Padding = new System.Windows.Forms.Padding(23, 0, 0, 0);
		this.distinctValuesTextLabelControl.Size = new System.Drawing.Size(146, 13);
		this.distinctValuesTextLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.distinctValuesTextLabelControl.TabIndex = 23;
		this.distinctValuesTextLabelControl.Text = "Distinct Values";
		this.maximumTextLabelControl.Location = new System.Drawing.Point(2, 166);
		this.maximumTextLabelControl.Name = "maximumTextLabelControl";
		this.maximumTextLabelControl.Padding = new System.Windows.Forms.Padding(23, 0, 0, 0);
		this.maximumTextLabelControl.Size = new System.Drawing.Size(146, 13);
		this.maximumTextLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.maximumTextLabelControl.TabIndex = 17;
		this.maximumTextLabelControl.Text = "Max";
		this.averageTextLabelControl.Location = new System.Drawing.Point(2, 200);
		this.averageTextLabelControl.Name = "averageTextLabelControl";
		this.averageTextLabelControl.Padding = new System.Windows.Forms.Padding(23, 0, 0, 0);
		this.averageTextLabelControl.Size = new System.Drawing.Size(146, 13);
		this.averageTextLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.averageTextLabelControl.TabIndex = 18;
		this.averageTextLabelControl.Text = "Avg";
		this.stddevTextLabelControl.Location = new System.Drawing.Point(2, 217);
		this.stddevTextLabelControl.Name = "stddevTextLabelControl";
		this.stddevTextLabelControl.Padding = new System.Windows.Forms.Padding(23, 0, 0, 0);
		this.stddevTextLabelControl.Size = new System.Drawing.Size(146, 13);
		this.stddevTextLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.stddevTextLabelControl.TabIndex = 19;
		this.stddevTextLabelControl.Text = "Standard deviation";
		this.varianceTextLabelControl.Location = new System.Drawing.Point(2, 234);
		this.varianceTextLabelControl.Name = "varianceTextLabelControl";
		this.varianceTextLabelControl.Padding = new System.Windows.Forms.Padding(23, 0, 0, 0);
		this.varianceTextLabelControl.Size = new System.Drawing.Size(146, 13);
		this.varianceTextLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.varianceTextLabelControl.TabIndex = 20;
		this.varianceTextLabelControl.Text = "Variance";
		this.stringValuesMaxValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.stringValuesMaxValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.stringValuesMaxValueLabelControl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.stringValuesMaxValueLabelControl.AutoEllipsis = true;
		this.stringValuesMaxValueLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.stringValuesMaxValueLabelControl.Location = new System.Drawing.Point(152, 39);
		this.stringValuesMaxValueLabelControl.Name = "stringValuesMaxValueLabelControl";
		this.stringValuesMaxValueLabelControl.Size = new System.Drawing.Size(191, 13);
		this.stringValuesMaxValueLabelControl.StyleController = this.valuesUserControlLayoutControl;
		this.stringValuesMaxValueLabelControl.TabIndex = 44;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.False;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[29]
		{
			this.stringValuesLabelControlLayoutControlItem, this.stringValuesMinTextLabelControlLayoutControlItem, this.stringValuesMinValueLabelControlLayoutControlItem, this.minimumDateTextLabelControlLayoutControlItem, this.maximumDateTextLabelControlLayoutControlItem, this.stringValuesDistinctTextLabelControlLayoutControlItem, this.valuesOrStringLengthLabelControlLayoutControlItem, this.minimumTextLabelControlLayoutControlItem, this.averageTextLabelControlLayoutControlItem, this.varianceTextLabelControlLayoutControlItem,
			this.distinctValuesTextLabelControlLayoutControlItem, this.dateSpanValueLabelControlLayoutControlItem, this.dateSpanTextLabelControlLayoutControlItem, this.minimumDateValueLabelControlLayoutControlItem, this.maximumDateValueLabelControlLayoutControlItem, this.stringValuesDistinctValueLabelControlLayoutControlItem, this.minimumValueLabelControlLayoutControlItem, this.stringValuesMaxValueLabelControlLayoutControlItem, this.stringValuesMaxTextLabelControlLayoutControlItem, this.maximumTextLabelControlLayoutControlItem,
			this.stddevTextLabelControlLayoutControlItem, this.numericSpanTextLabelControlLayoutControlItem, this.numericSpanValueLabelControlLayoutControlItem, this.maximumValueLabelControlLayoutControlItem, this.averageValueLabelControlLayoutControlItem, this.stddevValueLabelControlLayoutControlItem, this.varianceValueLabelControlLayoutControlItem, this.distinctValuesValueLabelControlLayoutControlItem, this.emptySpaceItem1
		});
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
		this.Root.Size = new System.Drawing.Size(345, 266);
		this.Root.TextVisible = false;
		this.stringValuesLabelControlLayoutControlItem.Control = this.stringValuesLabelControl;
		this.stringValuesLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.stringValuesLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 20);
		this.stringValuesLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 20);
		this.stringValuesLabelControlLayoutControlItem.Name = "stringValuesLabelControlLayoutControlItem";
		this.stringValuesLabelControlLayoutControlItem.Size = new System.Drawing.Size(345, 20);
		this.stringValuesLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.stringValuesLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.stringValuesLabelControlLayoutControlItem.TextVisible = false;
		this.stringValuesMinTextLabelControlLayoutControlItem.Control = this.stringValuesMinTextLabelControl;
		this.stringValuesMinTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 20);
		this.stringValuesMinTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 17);
		this.stringValuesMinTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 17);
		this.stringValuesMinTextLabelControlLayoutControlItem.Name = "stringValuesMinTextLabelControlLayoutControlItem";
		this.stringValuesMinTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(150, 17);
		this.stringValuesMinTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.stringValuesMinTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.stringValuesMinTextLabelControlLayoutControlItem.TextVisible = false;
		this.stringValuesMinValueLabelControlLayoutControlItem.Control = this.stringValuesMinValueLabelControl;
		this.stringValuesMinValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(150, 20);
		this.stringValuesMinValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(195, 17);
		this.stringValuesMinValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 17);
		this.stringValuesMinValueLabelControlLayoutControlItem.Name = "stringValuesMinValueLabelControlLayoutControlItem";
		this.stringValuesMinValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(195, 17);
		this.stringValuesMinValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.stringValuesMinValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.stringValuesMinValueLabelControlLayoutControlItem.TextVisible = false;
		this.minimumDateTextLabelControlLayoutControlItem.Control = this.minimumDateTextLabelControl;
		this.minimumDateTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 54);
		this.minimumDateTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 17);
		this.minimumDateTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 17);
		this.minimumDateTextLabelControlLayoutControlItem.Name = "minimumDateTextLabelControlLayoutControlItem";
		this.minimumDateTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(150, 17);
		this.minimumDateTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.minimumDateTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.minimumDateTextLabelControlLayoutControlItem.TextVisible = false;
		this.maximumDateTextLabelControlLayoutControlItem.Control = this.maximumDateTextLabelControl;
		this.maximumDateTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 71);
		this.maximumDateTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 17);
		this.maximumDateTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 17);
		this.maximumDateTextLabelControlLayoutControlItem.Name = "maximumDateTextLabelControlLayoutControlItem";
		this.maximumDateTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(150, 17);
		this.maximumDateTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.maximumDateTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.maximumDateTextLabelControlLayoutControlItem.TextVisible = false;
		this.stringValuesDistinctTextLabelControlLayoutControlItem.Control = this.stringValuesDistinctTextLabelControl;
		this.stringValuesDistinctTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 105);
		this.stringValuesDistinctTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 17);
		this.stringValuesDistinctTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 17);
		this.stringValuesDistinctTextLabelControlLayoutControlItem.Name = "stringValuesDistinctTextLabelControlLayoutControlItem";
		this.stringValuesDistinctTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(150, 17);
		this.stringValuesDistinctTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.stringValuesDistinctTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.stringValuesDistinctTextLabelControlLayoutControlItem.TextVisible = false;
		this.valuesOrStringLengthLabelControlLayoutControlItem.Control = this.valuesOrStringLengthLabelControl;
		this.valuesOrStringLengthLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 127);
		this.valuesOrStringLengthLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 20);
		this.valuesOrStringLengthLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 20);
		this.valuesOrStringLengthLabelControlLayoutControlItem.Name = "valuesOrStringLengthLabelControlLayoutControlItem";
		this.valuesOrStringLengthLabelControlLayoutControlItem.Size = new System.Drawing.Size(345, 20);
		this.valuesOrStringLengthLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.valuesOrStringLengthLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.valuesOrStringLengthLabelControlLayoutControlItem.TextVisible = false;
		this.minimumTextLabelControlLayoutControlItem.Control = this.minimumTextLabelControl;
		this.minimumTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 147);
		this.minimumTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 17);
		this.minimumTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 17);
		this.minimumTextLabelControlLayoutControlItem.Name = "minimumTextLabelControlLayoutControlItem";
		this.minimumTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(150, 17);
		this.minimumTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.minimumTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.minimumTextLabelControlLayoutControlItem.TextVisible = false;
		this.averageTextLabelControlLayoutControlItem.Control = this.averageTextLabelControl;
		this.averageTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 198);
		this.averageTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 17);
		this.averageTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 17);
		this.averageTextLabelControlLayoutControlItem.Name = "averageTextLabelControlLayoutControlItem";
		this.averageTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(150, 17);
		this.averageTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.averageTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.averageTextLabelControlLayoutControlItem.TextVisible = false;
		this.varianceTextLabelControlLayoutControlItem.Control = this.varianceTextLabelControl;
		this.varianceTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 232);
		this.varianceTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 17);
		this.varianceTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 17);
		this.varianceTextLabelControlLayoutControlItem.Name = "varianceTextLabelControlLayoutControlItem";
		this.varianceTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(150, 17);
		this.varianceTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.varianceTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.varianceTextLabelControlLayoutControlItem.TextVisible = false;
		this.distinctValuesTextLabelControlLayoutControlItem.Control = this.distinctValuesTextLabelControl;
		this.distinctValuesTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 249);
		this.distinctValuesTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 17);
		this.distinctValuesTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 17);
		this.distinctValuesTextLabelControlLayoutControlItem.Name = "distinctValuesTextLabelControlLayoutControlItem";
		this.distinctValuesTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(150, 17);
		this.distinctValuesTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.distinctValuesTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.distinctValuesTextLabelControlLayoutControlItem.TextVisible = false;
		this.dateSpanValueLabelControlLayoutControlItem.Control = this.dateSpanValueLabelControl;
		this.dateSpanValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(150, 88);
		this.dateSpanValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(195, 17);
		this.dateSpanValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 17);
		this.dateSpanValueLabelControlLayoutControlItem.Name = "dateSpanValueLabelControlLayoutControlItem";
		this.dateSpanValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(195, 17);
		this.dateSpanValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.dateSpanValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.dateSpanValueLabelControlLayoutControlItem.TextVisible = false;
		this.dateSpanTextLabelControlLayoutControlItem.Control = this.dateSpanTextLabelControl;
		this.dateSpanTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 88);
		this.dateSpanTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 17);
		this.dateSpanTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 17);
		this.dateSpanTextLabelControlLayoutControlItem.Name = "dateSpanTextLabelControlLayoutControlItem";
		this.dateSpanTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(150, 17);
		this.dateSpanTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.dateSpanTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.dateSpanTextLabelControlLayoutControlItem.TextVisible = false;
		this.minimumDateValueLabelControlLayoutControlItem.Control = this.minimumDateValueLabelControl;
		this.minimumDateValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(150, 54);
		this.minimumDateValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(195, 17);
		this.minimumDateValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 17);
		this.minimumDateValueLabelControlLayoutControlItem.Name = "minimumDateValueLabelControlLayoutControlItem";
		this.minimumDateValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(195, 17);
		this.minimumDateValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.minimumDateValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.minimumDateValueLabelControlLayoutControlItem.TextVisible = false;
		this.maximumDateValueLabelControlLayoutControlItem.Control = this.maximumDateValueLabelControl;
		this.maximumDateValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(150, 71);
		this.maximumDateValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(195, 17);
		this.maximumDateValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 17);
		this.maximumDateValueLabelControlLayoutControlItem.Name = "maximumDateValueLabelControlLayoutControlItem";
		this.maximumDateValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(195, 17);
		this.maximumDateValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.maximumDateValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.maximumDateValueLabelControlLayoutControlItem.TextVisible = false;
		this.stringValuesDistinctValueLabelControlLayoutControlItem.Control = this.stringValuesDistinctValueLabelControl;
		this.stringValuesDistinctValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(150, 105);
		this.stringValuesDistinctValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(195, 17);
		this.stringValuesDistinctValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 17);
		this.stringValuesDistinctValueLabelControlLayoutControlItem.Name = "stringValuesDistinctValueLabelControlLayoutControlItem";
		this.stringValuesDistinctValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(195, 17);
		this.stringValuesDistinctValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.stringValuesDistinctValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.stringValuesDistinctValueLabelControlLayoutControlItem.TextVisible = false;
		this.minimumValueLabelControlLayoutControlItem.Control = this.minimumValueLabelControl;
		this.minimumValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(150, 147);
		this.minimumValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(195, 17);
		this.minimumValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 17);
		this.minimumValueLabelControlLayoutControlItem.Name = "minimumValueLabelControlLayoutControlItem";
		this.minimumValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(195, 17);
		this.minimumValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.minimumValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.minimumValueLabelControlLayoutControlItem.TextVisible = false;
		this.stringValuesMaxValueLabelControlLayoutControlItem.Control = this.stringValuesMaxValueLabelControl;
		this.stringValuesMaxValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(150, 37);
		this.stringValuesMaxValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(195, 17);
		this.stringValuesMaxValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 17);
		this.stringValuesMaxValueLabelControlLayoutControlItem.Name = "stringValuesMaxValueLabelControlLayoutControlItem";
		this.stringValuesMaxValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(195, 17);
		this.stringValuesMaxValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.stringValuesMaxValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.stringValuesMaxValueLabelControlLayoutControlItem.TextVisible = false;
		this.stringValuesMaxTextLabelControlLayoutControlItem.Control = this.stringValuesMaxTextLabelControl;
		this.stringValuesMaxTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 37);
		this.stringValuesMaxTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 17);
		this.stringValuesMaxTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 17);
		this.stringValuesMaxTextLabelControlLayoutControlItem.Name = "stringValuesMaxTextLabelControlLayoutControlItem";
		this.stringValuesMaxTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(150, 17);
		this.stringValuesMaxTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.stringValuesMaxTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.stringValuesMaxTextLabelControlLayoutControlItem.TextVisible = false;
		this.maximumTextLabelControlLayoutControlItem.Control = this.maximumTextLabelControl;
		this.maximumTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 164);
		this.maximumTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 17);
		this.maximumTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 17);
		this.maximumTextLabelControlLayoutControlItem.Name = "maximumTextLabelControlLayoutControlItem";
		this.maximumTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(150, 17);
		this.maximumTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.maximumTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.maximumTextLabelControlLayoutControlItem.TextVisible = false;
		this.stddevTextLabelControlLayoutControlItem.Control = this.stddevTextLabelControl;
		this.stddevTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 215);
		this.stddevTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 17);
		this.stddevTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 17);
		this.stddevTextLabelControlLayoutControlItem.Name = "stddevTextLabelControlLayoutControlItem";
		this.stddevTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(150, 17);
		this.stddevTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.stddevTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.stddevTextLabelControlLayoutControlItem.TextVisible = false;
		this.numericSpanTextLabelControlLayoutControlItem.Control = this.numericSpanTextLabelControl;
		this.numericSpanTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 181);
		this.numericSpanTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(150, 17);
		this.numericSpanTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 17);
		this.numericSpanTextLabelControlLayoutControlItem.Name = "numericSpanTextLabelControlLayoutControlItem";
		this.numericSpanTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(150, 17);
		this.numericSpanTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.numericSpanTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.numericSpanTextLabelControlLayoutControlItem.TextVisible = false;
		this.numericSpanValueLabelControlLayoutControlItem.Control = this.numericSpanValueLabelControl;
		this.numericSpanValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(150, 181);
		this.numericSpanValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(195, 17);
		this.numericSpanValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 17);
		this.numericSpanValueLabelControlLayoutControlItem.Name = "numericSpanValueLabelControlLayoutControlItem";
		this.numericSpanValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(195, 17);
		this.numericSpanValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.numericSpanValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.numericSpanValueLabelControlLayoutControlItem.TextVisible = false;
		this.maximumValueLabelControlLayoutControlItem.Control = this.maximumValueLabelControl;
		this.maximumValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(150, 164);
		this.maximumValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(195, 17);
		this.maximumValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 17);
		this.maximumValueLabelControlLayoutControlItem.Name = "maximumValueLabelControlLayoutControlItem";
		this.maximumValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(195, 17);
		this.maximumValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.maximumValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.maximumValueLabelControlLayoutControlItem.TextVisible = false;
		this.averageValueLabelControlLayoutControlItem.Control = this.averageValueLabelControl;
		this.averageValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(150, 198);
		this.averageValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(195, 17);
		this.averageValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 17);
		this.averageValueLabelControlLayoutControlItem.Name = "averageValueLabelControlLayoutControlItem";
		this.averageValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(195, 17);
		this.averageValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.averageValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.averageValueLabelControlLayoutControlItem.TextVisible = false;
		this.stddevValueLabelControlLayoutControlItem.Control = this.stddevValueLabelControl;
		this.stddevValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(150, 215);
		this.stddevValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(195, 17);
		this.stddevValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 17);
		this.stddevValueLabelControlLayoutControlItem.Name = "stddevValueLabelControlLayoutControlItem";
		this.stddevValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(195, 17);
		this.stddevValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.stddevValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.stddevValueLabelControlLayoutControlItem.TextVisible = false;
		this.varianceValueLabelControlLayoutControlItem.Control = this.varianceValueLabelControl;
		this.varianceValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(150, 232);
		this.varianceValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(195, 17);
		this.varianceValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 17);
		this.varianceValueLabelControlLayoutControlItem.Name = "varianceValueLabelControlLayoutControlItem";
		this.varianceValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(195, 17);
		this.varianceValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.varianceValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.varianceValueLabelControlLayoutControlItem.TextVisible = false;
		this.distinctValuesValueLabelControlLayoutControlItem.Control = this.distinctValuesValueLabelControl;
		this.distinctValuesValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(150, 249);
		this.distinctValuesValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(195, 17);
		this.distinctValuesValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(195, 17);
		this.distinctValuesValueLabelControlLayoutControlItem.Name = "distinctValuesValueLabelControlLayoutControlItem";
		this.distinctValuesValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(195, 17);
		this.distinctValuesValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.distinctValuesValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.distinctValuesValueLabelControlLayoutControlItem.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 122);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 5);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(1, 5);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(345, 5);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.AutoSize = true;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.valuesUserControlLayoutControl);
		base.Name = "ValuesSectionUserControl";
		base.Size = new System.Drawing.Size(348, 274);
		((System.ComponentModel.ISupportInitialize)this.valuesUserControlLayoutControl).EndInit();
		this.valuesUserControlLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.stringValuesLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.stringValuesMinTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.stringValuesMinValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.minimumDateTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.maximumDateTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.stringValuesDistinctTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valuesOrStringLengthLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.minimumTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.averageTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.varianceTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.distinctValuesTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateSpanValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateSpanTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.minimumDateValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.maximumDateValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.stringValuesDistinctValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.minimumValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.stringValuesMaxValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.stringValuesMaxTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.maximumTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.stddevTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.numericSpanTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.numericSpanValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.maximumValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.averageValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.stddevValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.varianceValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.distinctValuesValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
