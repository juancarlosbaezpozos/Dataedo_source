using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.DataProfiling.Models;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.Properties;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.DataSources.Enums;
using Dataedo.Model.Data.DataProfiling;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Printing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.DataProfiling.DataProfilingUserControls;

public class CenterSectionUserControl : BaseUserControl
{
	private CurrentToolTipValuesForToolTip currentToolTipValuesForToolTip;

	private ProfilingDatabaseTypeEnum.ProfilingDatabaseType? profilingDatabaseType;

	private INavigationObject FocusedNavigationObject;

	private IContainer components;

	private NonCustomizableLayoutControl centerSectionLayoutControl;

	private LabelControl previevOnlyLabelControl;

	private LabelControl dateAtLabelControl;

	private LabelControl sampleDataTextLabelControl;

	private SimpleButton centerSectionTextSimpleButton;

	private RowStatsUserControlcs rowStatsUserControlcs;

	private ValuesSectionUserControl valuesSectionUserControl;

	private GridControl sampleDataGridControl;

	private GridView sampleDataGridControlGridView;

	private ChartControl columnDetailsChartControl;

	private LabelControl columnDetailsTitleLabelControl;

	private ChartControl rowStatsChartControl;

	private LayoutControlGroup centerSectionLayoutControlGroup;

	private ToolTipController currentValueChartControlToolTipController;

	private LayoutControlItem rowStatsChartLayoutControlItem;

	private LayoutControlItem columnNameLayoutControlItem;

	private LayoutControlItem sampleDataGridControlLayoutControlItem;

	private LayoutControlItem columnDetailsChartControlLayoutControlItem;

	private LayoutControlItem valuesSectionUserControlLayoutControlItem;

	private LayoutControlItem rowStatsUserControlcsLayoutControlItem;

	private LayoutControlItem sampleDataTextLabelControlLayoutControlItem;

	private LayoutControlItem centerSectionTextSimpleButtonLayoutControlItem;

	private LayoutControlItem dateAtLabelControlLayoutControlItem;

	private EmptySpaceItem emptySpaceItem9;

	private LayoutControlItem previevOnlyLabelControlLayoutControlItem;

	private ColumnNavigationObject FocusedColumnNavigationObject => FocusedNavigationObject as ColumnNavigationObject;

	private TableNavigationObject FocusedTableNavigationObject => FocusedNavigationObject as TableNavigationObject;

	private DataProfilingForm DataProfilingForm => FindForm() as DataProfilingForm;

	public CenterSectionUserControl()
	{
		InitializeComponent();
		currentToolTipValuesForToolTip = new CurrentToolTipValuesForToolTip();
		columnDetailsChartControl.BackColor = SkinColors.ControlColorFromSystemColors;
		rowStatsChartControl.BackColor = SkinColors.ControlColorFromSystemColors;
	}

	public void SetFocusedNavObject(INavigationObject navObj)
	{
		FocusedNavigationObject = navObj;
	}

	public void SetProfilingDatabaseType(SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		profilingDatabaseType = EnumToEnumChanger.GetProfilingDatabaseTypeEnum(databaseType);
	}

	private void ColumnDetailsTitleLabelControl_TextChanged(object sender, EventArgs e)
	{
		Size size = TextRenderer.MeasureText(columnDetailsTitleLabelControl.Text, columnDetailsTitleLabelControl.Font);
		columnNameLayoutControlItem.MinSize = new Size(70, columnNameLayoutControlItem.Height);
		int num = size.Width + columnNameLayoutControlItem.ImageOptions.Image.Width + columnNameLayoutControlItem.Padding.Left + columnNameLayoutControlItem.Padding.Right + 3;
		columnNameLayoutControlItem.MaxSize = new Size(num, columnNameLayoutControlItem.Height);
		columnNameLayoutControlItem.Size = new Size(num, columnNameLayoutControlItem.Height);
	}

	private async void CenterSectionTextSimpleButton_Click(object sender, EventArgs e)
	{
		if (centerSectionTextSimpleButton.Text == "Profile Column" && FocusedColumnNavigationObject != null)
		{
			await DataProfilingForm.FullProfileSingleNavColumnAsync(FocusedColumnNavigationObject.ColumnId);
		}
		else if (centerSectionTextSimpleButton.Text == "Preview Sample Rows" && FocusedTableNavigationObject != null)
		{
			await DataProfilingForm.PreviewSampleDataForSingleNavTableAsync(FocusedTableNavigationObject);
		}
	}

	private void CurrentValueChartControlToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		if (!(e.SelectedControl is ChartControl chartControl))
		{
			return;
		}
		DiagramCoordinates diagramCoordinates = ((XYDiagram)chartControl.Diagram).PointToDiagram(e.ControlMousePosition);
		ChartHitInfo chartHitInfo = chartControl.CalcHitInfo(e.ControlMousePosition);
		if (!chartHitInfo.InSeries || diagramCoordinates.AxisX == null || diagramCoordinates.AxisY == null)
		{
			return;
		}
		SuperToolTipSetupArgs superToolTipSetupArgs = new SuperToolTipSetupArgs();
		if (chartControl.Name == "rowStatsChartControl")
		{
			if (currentToolTipValuesForToolTip.RowDistributionValues.TryGetValue(chartHitInfo.Series.Tag.ToString(), out var value))
			{
				superToolTipSetupArgs.Contents.Text = value;
			}
		}
		else
		{
			if (!(chartControl.Name == "columnDetailsChartControl"))
			{
				return;
			}
			superToolTipSetupArgs = GetToolTipForColumnDetailsChart(diagramCoordinates);
		}
		e.Info = new ToolTipControlInfo
		{
			Object = $"object e.Info {e.ControlMousePosition}",
			ToolTipType = ToolTipType.SuperTip,
			SuperTip = new SuperToolTip()
		};
		e.Info.SuperTip.Setup(superToolTipSetupArgs);
	}

	private SuperToolTipSetupArgs GetToolTipForColumnDetailsChart(DiagramCoordinates coords)
	{
		SuperToolTipSetupArgs superToolTipSetupArgs = new SuperToolTipSetupArgs();
		CurrentToolTipValuesForToolTip obj = currentToolTipValuesForToolTip;
		if (obj != null && obj.Min > 0.0)
		{
			if (coords.NumericalValue <= currentToolTipValuesForToolTip?.Min)
			{
				superToolTipSetupArgs.Contents.Text = $"{currentToolTipValuesForToolTip.Min}";
			}
			else if (coords.NumericalValue <= currentToolTipValuesForToolTip?.Avg)
			{
				superToolTipSetupArgs.Contents.Text = $"{currentToolTipValuesForToolTip.Avg - currentToolTipValuesForToolTip.Min}";
			}
			else if (coords.NumericalValue <= currentToolTipValuesForToolTip?.Max)
			{
				superToolTipSetupArgs.Contents.Text = $"{currentToolTipValuesForToolTip.Max - currentToolTipValuesForToolTip.Avg}";
			}
		}
		else
		{
			CurrentToolTipValuesForToolTip obj2 = currentToolTipValuesForToolTip;
			if (obj2 != null && obj2.Min < 0.0)
			{
				CurrentToolTipValuesForToolTip obj3 = currentToolTipValuesForToolTip;
				if (obj3 != null && obj3.Avg < 0.0)
				{
					CurrentToolTipValuesForToolTip obj4 = currentToolTipValuesForToolTip;
					if (obj4 != null && obj4.Max < 0.0)
					{
						if (coords.NumericalValue >= currentToolTipValuesForToolTip?.Min)
						{
							superToolTipSetupArgs.Contents.Text = $"{0.0 - currentToolTipValuesForToolTip.Min}";
						}
						else if (coords.NumericalValue >= currentToolTipValuesForToolTip?.Avg)
						{
							superToolTipSetupArgs.Contents.Text = $"{currentToolTipValuesForToolTip.Min + currentToolTipValuesForToolTip.Avg}";
						}
						else if (coords.NumericalValue >= currentToolTipValuesForToolTip?.Max)
						{
							superToolTipSetupArgs.Contents.Text = $"{currentToolTipValuesForToolTip.Avg + currentToolTipValuesForToolTip.Max}";
						}
						goto IL_08d5;
					}
				}
			}
			CurrentToolTipValuesForToolTip obj5 = currentToolTipValuesForToolTip;
			if (obj5 != null && obj5.Min < 0.0)
			{
				CurrentToolTipValuesForToolTip obj6 = currentToolTipValuesForToolTip;
				if (obj6 != null && obj6.Avg > 0.0)
				{
					CurrentToolTipValuesForToolTip obj7 = currentToolTipValuesForToolTip;
					if (obj7 != null && obj7.Max > 0.0)
					{
						if (coords.NumericalValue < currentToolTipValuesForToolTip?.Avg)
						{
							superToolTipSetupArgs.Contents.Text = $"{currentToolTipValuesForToolTip?.Avg + currentToolTipValuesForToolTip.Min * -1.0}";
						}
						else if (coords.NumericalValue >= currentToolTipValuesForToolTip?.Avg && coords.NumericalValue <= currentToolTipValuesForToolTip?.Max)
						{
							superToolTipSetupArgs.Contents.Text = $"{currentToolTipValuesForToolTip.Max - currentToolTipValuesForToolTip.Avg}";
						}
						goto IL_08d5;
					}
				}
			}
			CurrentToolTipValuesForToolTip obj8 = currentToolTipValuesForToolTip;
			if (obj8 != null && obj8.Min <= 0.0)
			{
				CurrentToolTipValuesForToolTip obj9 = currentToolTipValuesForToolTip;
				if (obj9 != null && obj9.Avg <= 0.0)
				{
					CurrentToolTipValuesForToolTip obj10 = currentToolTipValuesForToolTip;
					if (obj10 != null && obj10.Max >= 0.0)
					{
						if (coords.NumericalValue < currentToolTipValuesForToolTip?.Avg)
						{
							superToolTipSetupArgs.Contents.Text = $"{(currentToolTipValuesForToolTip?.Avg - currentToolTipValuesForToolTip.Min) * -1.0}";
						}
						else if (coords.NumericalValue >= currentToolTipValuesForToolTip?.Avg)
						{
							superToolTipSetupArgs.Contents.Text = $"{-1.0 * currentToolTipValuesForToolTip.Avg + currentToolTipValuesForToolTip.Max}";
						}
					}
				}
			}
		}
		goto IL_08d5;
		IL_08d5:
		return superToolTipSetupArgs;
	}

	public void SetPresentationFocusOnSpecificColumn()
	{
		try
		{
			INavigationObject focusedNavigationObject = FocusedNavigationObject;
			if (focusedNavigationObject == null)
			{
				return;
			}
			if (focusedNavigationObject is ColumnNavigationObject columnNavigationObject && columnNavigationObject.Column != null)
			{
				ColumnProfiledDataObject column = columnNavigationObject.Column;
				bool hasValue = column.ProfilingDate.HasValue;
				HideSampleDataElements();
				SetUpperLabelForColumn(column);
				HideColumnProfilingDisplayElements();
				if (!hasValue)
				{
					if (columnNavigationObject.RowsCount != 0)
					{
						ShowCenterButtonForNotProfiledColumn();
						return;
					}
					HideCenterButton();
					sampleDataTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Always;
					sampleDataTextLabelControl.Text = "The column does not contain any data";
					return;
				}
				HideCenterButton();
				rowStatsUserControlcs.SetParameters(column, isToolTip: false);
				if (column.ValuesNullRowCount.HasValue || column.ValuesEmptyRowCount.HasValue || column.ValuesDistinctRowCount.HasValue || column.ValuesNondistinctRowCount.HasValue || (column.RowCount.HasValue && column.RowCount != 0))
				{
					rowStatsUserControlcsLayoutControlItem.Visibility = LayoutVisibility.Always;
				}
				PopulateDiagramRow();
				valuesSectionUserControl.SetParameters(column, profilingDatabaseType.Value);
				valuesSectionUserControlLayoutControlItem.Visibility = LayoutVisibility.Always;
				PopulateDiagramValueBasicStatistics();
			}
			else
			{
				if (!(focusedNavigationObject is TableNavigationObject tableNavigationObject))
				{
					return;
				}
				sampleDataGridControl.DataSource = null;
				HideColumnProfilingDisplayElements();
				SetUpperLabelForTable(tableNavigationObject);
				if (tableNavigationObject.ObjectSampleData == null && !tableNavigationObject.RowsCount.HasValue)
				{
					HideSampleDataElements();
					ShowCenterButtonForTableWithNoSampleData();
					return;
				}
				HideCenterButton();
				DataProfilingUtils.PopulateSampleDataGridFromDownloadedSampleData(tableNavigationObject.Columns, sampleDataGridControl, sampleDataGridControlGridView, tableNavigationObject.ObjectSampleData, profilingDatabaseType.Value);
				if (tableNavigationObject.RowsCount == 0)
				{
					sampleDataTextLabelControl.Text = "The " + tableNavigationObject.ObjectType.ToString().ToLower() + " does not contain any data";
				}
				else
				{
					sampleDataTextLabelControl.Text = "Sample, randomly picked rows: " + $"{sampleDataGridControlGridView.DataRowCount}";
				}
				ShowSampleDataElements();
			}
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public void EnableCentralProfilingButton()
	{
		centerSectionTextSimpleButton.Enabled = true;
	}

	public void DisableCentralProfilingButton()
	{
		centerSectionTextSimpleButton.Enabled = false;
	}

	private void HideColumnProfilingDisplayElements()
	{
		columnDetailsChartControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		valuesSectionUserControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		rowStatsChartLayoutControlItem.Visibility = LayoutVisibility.Never;
		rowStatsUserControlcsLayoutControlItem.Visibility = LayoutVisibility.Never;
	}

	private void SetUpperLabelForColumn(ColumnProfiledDataObject column)
	{
		columnDetailsTitleLabelControl.ImageOptions.Image = Resources.column_16;
		columnDetailsTitleLabelControl.Text = column.DisplayName;
		dateAtLabelControl.Text = (column.ProfilingDate.HasValue ? (" at " + DataProfilingStringFormatter.FormatDateToRepositoryFormat(column.ProfilingDate)) : string.Empty);
		SuperToolTipSetupArgs superToolTipSetupArgs = new SuperToolTipSetupArgs();
		superToolTipSetupArgs.Contents.Text = "Column: " + column.DisplayName;
		columnDetailsTitleLabelControl.SuperTip.Setup(superToolTipSetupArgs);
	}

	private void SetUpperLabelForTable(TableNavigationObject table)
	{
		columnDetailsTitleLabelControl.ImageOptions.Image = table.IconTreeList;
		columnDetailsTitleLabelControl.Text = table.DisplayName;
		dateAtLabelControl.Text = string.Empty;
		SuperToolTipSetupArgs superToolTipSetupArgs = new SuperToolTipSetupArgs();
		superToolTipSetupArgs.Contents.Text = $"{table.ObjectType}: {table.DisplayName}";
		columnDetailsTitleLabelControl.SuperTip.Setup(superToolTipSetupArgs);
	}

	private void HideCenterButton()
	{
		centerSectionTextSimpleButtonLayoutControlItem.Visibility = LayoutVisibility.Never;
	}

	private void ShowCenterButtonForNotProfiledColumn()
	{
		centerSectionTextSimpleButton.MaximumSize = new Size(100, 100);
		centerSectionTextSimpleButton.Size = new Size(100, 100);
		centerSectionTextSimpleButton.ImageOptions.Image = Resources.profile_column_32;
		centerSectionTextSimpleButton.Text = "Profile Column";
		centerSectionTextSimpleButtonLayoutControlItem.Visibility = LayoutVisibility.Always;
	}

	private void ShowCenterButtonForTableWithNoSampleData()
	{
		centerSectionTextSimpleButton.MaximumSize = new Size(150, 100);
		centerSectionTextSimpleButton.Size = new Size(150, 100);
		centerSectionTextSimpleButton.ImageOptions.Image = Resources.sample_data_32;
		centerSectionTextSimpleButton.Text = "Preview Sample Rows";
		centerSectionTextSimpleButtonLayoutControlItem.Visibility = LayoutVisibility.Always;
	}

	private void HideSampleDataElements()
	{
		sampleDataGridControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		sampleDataTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		previevOnlyLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
	}

	private void ShowSampleDataElements()
	{
		sampleDataGridControlLayoutControlItem.Visibility = LayoutVisibility.Always;
		sampleDataTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Always;
		previevOnlyLabelControlLayoutControlItem.Visibility = LayoutVisibility.Always;
	}

	private void PopulateDiagramValueBasicStatistics()
	{
		double? num = null;
		double? num2 = null;
		double? num3 = null;
		ColumnProfiledDataObject column = FocusedColumnNavigationObject.Column;
		if (DataTypeChecker.IsINTType(column.ProfilingDataType) || DataTypeChecker.IsRealType(column.ProfilingDataType))
		{
			num = column.ValueMin;
			num2 = column.ValueAvg;
			num3 = column.ValueMax;
		}
		else if (DataTypeChecker.IsStringType(column.ProfilingDataType))
		{
			num = column.StringLengthMin;
			num2 = column.StringLengthAvg;
			num3 = column.StringLengthMax;
		}
		else if (DataTypeChecker.IsDateTimeType(column.ProfilingDataType))
		{
			return;
		}
		if (!num3.HasValue)
		{
			return;
		}
		currentToolTipValuesForToolTip.Min = null;
		currentToolTipValuesForToolTip.Avg = null;
		currentToolTipValuesForToolTip.Max = null;
		currentToolTipValuesForToolTip.Min = num;
		currentToolTipValuesForToolTip.Avg = num2;
		currentToolTipValuesForToolTip.Max = num3;
		DataTable dataTable = new DataTable("Table2");
		dataTable.Columns.Add("Parameter", typeof(string));
		dataTable.Columns.Add("Section", typeof(string));
		dataTable.Columns.Add("Value", typeof(double));
		if (num3 == num && num >= 0.0 && num3 >= 0.0)
		{
			dataTable.Rows.Add("min", "details", num);
		}
		else if (num < 0.0 && num3 < 0.0)
		{
			dataTable.Rows.Add("trans", "details", num3);
			dataTable.Rows.Add("max", "details", num - num3);
		}
		else if (num < 0.0)
		{
			dataTable.Rows.Add("max", "details", num);
			dataTable.Rows.Add("max2", "details", num3);
		}
		else
		{
			dataTable.Rows.Add("min", "details", num);
			dataTable.Rows.Add("max", "details", num3 - num);
		}
		columnDetailsChartControl.DataSource = dataTable;
		columnDetailsChartControl.SeriesDataMember = "Parameter";
		columnDetailsChartControl.SeriesTemplate.ArgumentDataMember = "Section";
		columnDetailsChartControl.SeriesTemplate.ValueDataMembers.AddRange("Value");
		columnDetailsChartControl.Dock = DockStyle.Fill;
		XYDiagram xYDiagram = (XYDiagram)columnDetailsChartControl.Diagram;
		if (xYDiagram == null)
		{
			return;
		}
		double? num4 = 1.5 * num3 / 100.0 * 3.5;
		if (num4 > 0.0)
		{
			if (num < 0.0)
			{
				xYDiagram.AxisY.WholeRange.MinValue = num - num4;
			}
			else
			{
				xYDiagram.AxisY.WholeRange.MinValue = 0.0 - num4;
			}
			if (num3 > 0.0)
			{
				xYDiagram.AxisY.WholeRange.MaxValue = num3 + num4;
			}
			else
			{
				xYDiagram.AxisY.WholeRange.MaxValue = num3 - num4;
			}
		}
		else
		{
			if (num < 0.0)
			{
				xYDiagram.AxisY.WholeRange.MinValue = num + num4;
			}
			else
			{
				xYDiagram.AxisY.WholeRange.MinValue = num - num4;
			}
			if (num3 > 0.0)
			{
				xYDiagram.AxisY.WholeRange.MaxValue = num3 + num4;
			}
			else
			{
				xYDiagram.AxisY.WholeRange.MaxValue = num3 - num4;
			}
		}
		StackedBarSeriesView view = new StackedBarSeriesView
		{
			BarWidth = 0.3
		};
		Color transparent = Color.Transparent;
		Color color = Color.FromArgb(255, 68, 114, 196);
		Color color2 = Color.FromArgb(255, 226, 226, 226);
		columnDetailsChartControl.SeriesTemplate.View = view;
		if (columnDetailsChartControl?.Series["min"] != null)
		{
			columnDetailsChartControl.Series["min"].View.Color = color2;
		}
		if (columnDetailsChartControl?.Series["max"] != null)
		{
			columnDetailsChartControl.Series["max"].View.Color = color;
		}
		if (columnDetailsChartControl?.Series["max2"] != null)
		{
			columnDetailsChartControl.Series["max2"].View.Color = color;
		}
		if (columnDetailsChartControl?.Series["trans"] != null)
		{
			columnDetailsChartControl.Series["trans"].View.Color = transparent;
		}
		AxisY axisY = xYDiagram.AxisY;
		axisY.CustomLabels.Clear();
		ConstantLine constantLine = new ConstantLine("Constant Line 1");
		ConstantLine constantLine2 = new ConstantLine("Constant Line 2");
		ConstantLine constantLine3 = new ConstantLine("Constant Line 3");
		xYDiagram.AxisY.ConstantLines.Clear();
		if (num3 == num && num2 == num)
		{
			axisY.CustomLabels.Add(new CustomAxisLabel(DataProfilingStringFormatter.FormatFloat2Values(num3) + Environment.NewLine + "Min Avg Max", num3));
			xYDiagram.AxisY.ConstantLines.Add(constantLine);
			xYDiagram.AxisY.ConstantLines.Add(constantLine2);
			xYDiagram.AxisY.ConstantLines.Add(constantLine3);
		}
		else if (num == num2)
		{
			axisY.CustomLabels.Add(new CustomAxisLabel(DataProfilingStringFormatter.FormatFloat2Values(num) + Environment.NewLine + "Min Avg", num));
			axisY.CustomLabels.Add(new CustomAxisLabel(DataProfilingStringFormatter.FormatFloat2Values(num3) + Environment.NewLine + "Max", num3));
			xYDiagram.AxisY.ConstantLines.Add(constantLine);
			xYDiagram.AxisY.ConstantLines.Add(constantLine2);
			xYDiagram.AxisY.ConstantLines.Add(constantLine3);
		}
		else if (num3 == num)
		{
			axisY.CustomLabels.Add(new CustomAxisLabel(DataProfilingStringFormatter.FormatFloat2Values(num) + Environment.NewLine + "Min Max", num3));
			axisY.CustomLabels.Add(new CustomAxisLabel(DataProfilingStringFormatter.FormatFloat2Values(num2) + Environment.NewLine + "Avg", num2));
			xYDiagram.AxisY.ConstantLines.Add(constantLine);
			xYDiagram.AxisY.ConstantLines.Add(constantLine2);
			xYDiagram.AxisY.ConstantLines.Add(constantLine3);
		}
		else
		{
			axisY.CustomLabels.Add(new CustomAxisLabel(DataProfilingStringFormatter.FormatFloat2Values(num) + Environment.NewLine + "Min", num));
			axisY.CustomLabels.Add(new CustomAxisLabel(DataProfilingStringFormatter.FormatFloat2Values(num2) + Environment.NewLine + "Avg", num2));
			axisY.CustomLabels.Add(new CustomAxisLabel(DataProfilingStringFormatter.FormatFloat2Values(num3) + Environment.NewLine + "Max", num3));
			xYDiagram.AxisY.ConstantLines.Add(constantLine);
			xYDiagram.AxisY.ConstantLines.Add(constantLine2);
			xYDiagram.AxisY.ConstantLines.Add(constantLine3);
		}
		constantLine.AxisValue = num;
		constantLine.Color = Color.Black;
		constantLine.LineStyle.Thickness = 4;
		constantLine.Visible = true;
		constantLine.ShowInLegend = true;
		constantLine.LegendText = "Min";
		constantLine.ShowBehind = false;
		constantLine.Title.Visible = false;
		constantLine2.AxisValue = num2;
		constantLine2.Color = Color.Black;
		constantLine2.LineStyle.Thickness = 4;
		constantLine2.Visible = true;
		constantLine2.ShowInLegend = true;
		constantLine2.LegendText = "Avg";
		constantLine2.ShowBehind = false;
		constantLine2.Title.Visible = false;
		constantLine3.AxisValue = num3;
		constantLine3.Color = Color.Black;
		constantLine3.LineStyle.Thickness = 4;
		constantLine3.Visible = true;
		constantLine3.ShowInLegend = true;
		constantLine3.LegendText = "max";
		constantLine3.ShowBehind = false;
		constantLine3.Title.Visible = false;
		columnDetailsChartControlLayoutControlItem.Visibility = LayoutVisibility.Always;
	}

	private void PopulateDiagramRow()
	{
		DataTable dataTable = new DataTable("Table1");
		dataTable.Columns.Add("Parameter", typeof(string));
		dataTable.Columns.Add("Section", typeof(string));
		dataTable.Columns.Add("Value", typeof(long));
		ColumnProfiledDataObject column = FocusedColumnNavigationObject.Column;
		if ((column.ValuesDistinctRowCount.HasValue || column.ValuesNondistinctRowCount.HasValue || column.ValuesDistinctRowCount.HasValue || column.ValuesNullRowCount.HasValue) && (column.ValuesDistinctRowCount != 0 || column.ValuesNondistinctRowCount != 0 || column.ValuesDistinctRowCount != 0 || column.ValuesNullRowCount != 0))
		{
			double? number = 0.0;
			double? number2 = 0.0;
			double? number3 = 0.0;
			double? number4 = 0.0;
			if (column.RowCount.HasValue && column.RowCount != 0)
			{
				number = 1.0 * (double?)column.ValuesNullRowCount / (double?)column.RowCount * 100.0;
				number2 = 1.0 * (double?)column.ValuesEmptyRowCount / (double?)column.RowCount * 100.0;
				number3 = 1.0 * (double?)column.ValuesDistinctRowCount / (double?)column.RowCount * 100.0;
				number4 = 1.0 * (double?)column.ValuesNondistinctRowCount / (double?)column.RowCount * 100.0;
			}
			string empty = string.Empty;
			string empty2 = string.Empty;
			string empty3 = string.Empty;
			string empty4 = string.Empty;
			empty = ((!number.HasValue) ? string.Empty : (DataProfilingStringFormatter.FormatFloat1Values(number) + "%"));
			empty2 = ((!number2.HasValue) ? string.Empty : (DataProfilingStringFormatter.FormatFloat1Values(number2) + "%"));
			empty3 = ((!number3.HasValue) ? string.Empty : (DataProfilingStringFormatter.FormatFloat1Values(number3) + "%"));
			empty4 = ((!number4.HasValue) ? string.Empty : (DataProfilingStringFormatter.FormatFloat1Values(number4) + "%"));
			string text = "Distinct values (" + empty3 + ")";
			string text2 = "Non distinct values (" + empty4 + ")";
			string text3 = "Empty (" + empty2 + ")";
			string text4 = "NULL (" + empty + ")";
			currentToolTipValuesForToolTip.RowDistributionValues.Clear();
			if (column.ValuesDistinctRowCount > 0)
			{
				dataTable.Rows.Add(text, "rows", column.ValuesDistinctRowCount);
				currentToolTipValuesForToolTip.RowDistributionValues.Add(text, empty3);
			}
			if (column.ValuesNondistinctRowCount > 0)
			{
				dataTable.Rows.Add(text2, "rows", column.ValuesNondistinctRowCount);
				currentToolTipValuesForToolTip.RowDistributionValues.Add(text2, empty4);
			}
			if (column.ValuesEmptyRowCount > 0)
			{
				dataTable.Rows.Add(text3, "rows", column.ValuesEmptyRowCount);
				currentToolTipValuesForToolTip.RowDistributionValues.Add(text3, empty2);
			}
			if (column.ValuesNullRowCount > 0)
			{
				dataTable.Rows.Add(text4, "rows", column.ValuesNullRowCount);
				currentToolTipValuesForToolTip.RowDistributionValues.Add(text4, empty);
			}
			rowStatsChartControl.DataSource = dataTable;
			rowStatsChartControl.SeriesDataMember = "Parameter";
			rowStatsChartControl.SeriesTemplate.ArgumentDataMember = "Section";
			rowStatsChartControl.SeriesTemplate.ValueDataMembers.AddRange("Value");
			rowStatsChartControl.SeriesTemplate.View = new FullStackedBarSeriesView();
			rowStatsChartControl.Dock = DockStyle.Fill;
			FullStackedBarSeriesView fullStackedBarSeriesView = new FullStackedBarSeriesView();
			fullStackedBarSeriesView.BarWidth = 0.3;
			rowStatsChartControl.SeriesTemplate.View = fullStackedBarSeriesView;
			if (column.ValuesNullRowCount > 0)
			{
				rowStatsChartControl.Series[text4].View.Color = Color.FromArgb(255, 226, 226, 226);
			}
			if (column.ValuesEmptyRowCount > 0)
			{
				rowStatsChartControl.Series[text3].View.Color = Color.FromArgb(255, 164, 164, 164);
			}
			if (column.ValuesDistinctRowCount > 0)
			{
				rowStatsChartControl.Series[text].View.Color = Color.FromArgb(255, 214, 132, 41);
			}
			if (column.ValuesNondistinctRowCount > 0)
			{
				rowStatsChartControl.Series[text2].View.Color = Color.FromArgb(255, 68, 114, 196);
			}
			rowStatsChartLayoutControlItem.Visibility = LayoutVisibility.Always;
		}
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
		this.components = new System.ComponentModel.Container();
		DevExpress.XtraCharts.XYDiagram xYDiagram = new DevExpress.XtraCharts.XYDiagram();
		DevExpress.XtraCharts.Series series = new DevExpress.XtraCharts.Series();
		DevExpress.XtraCharts.SideBySideBarSeriesView sideBySideBarSeriesView = new DevExpress.XtraCharts.SideBySideBarSeriesView();
		DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
		DevExpress.Utils.SuperToolTip superToolTip = new DevExpress.Utils.SuperToolTip();
		DevExpress.Utils.ToolTipItem toolTipItem = new DevExpress.Utils.ToolTipItem();
		DevExpress.XtraCharts.XYDiagram xYDiagram2 = new DevExpress.XtraCharts.XYDiagram();
		DevExpress.XtraCharts.Series series2 = new DevExpress.XtraCharts.Series();
		DevExpress.XtraCharts.FullStackedBarSeriesView fullStackedBarSeriesView = new DevExpress.XtraCharts.FullStackedBarSeriesView();
		DevExpress.XtraCharts.SideBySideBarSeriesLabel sideBySideBarSeriesLabel2 = new DevExpress.XtraCharts.SideBySideBarSeriesLabel();
		this.centerSectionLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.previevOnlyLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.dateAtLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.sampleDataTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.centerSectionTextSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.rowStatsUserControlcs = new Dataedo.App.DataProfiling.DataProfilingUserControls.RowStatsUserControlcs();
		this.valuesSectionUserControl = new Dataedo.App.DataProfiling.DataProfilingUserControls.ValuesSectionUserControl();
		this.sampleDataGridControl = new DevExpress.XtraGrid.GridControl();
		this.sampleDataGridControlGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.columnDetailsChartControl = new DevExpress.XtraCharts.ChartControl();
		this.currentValueChartControlToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.columnDetailsTitleLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.rowStatsChartControl = new DevExpress.XtraCharts.ChartControl();
		this.centerSectionLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.rowStatsChartLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.columnNameLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sampleDataGridControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.columnDetailsChartControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.valuesSectionUserControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.rowStatsUserControlcsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.sampleDataTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.centerSectionTextSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.dateAtLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem9 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.previevOnlyLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.centerSectionLayoutControl).BeginInit();
		this.centerSectionLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.sampleDataGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sampleDataGridControlGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDetailsChartControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)xYDiagram).BeginInit();
		((System.ComponentModel.ISupportInitialize)series).BeginInit();
		((System.ComponentModel.ISupportInitialize)sideBySideBarSeriesView).BeginInit();
		((System.ComponentModel.ISupportInitialize)sideBySideBarSeriesLabel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowStatsChartControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)xYDiagram2).BeginInit();
		((System.ComponentModel.ISupportInitialize)series2).BeginInit();
		((System.ComponentModel.ISupportInitialize)fullStackedBarSeriesView).BeginInit();
		((System.ComponentModel.ISupportInitialize)sideBySideBarSeriesLabel2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.centerSectionLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowStatsChartLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnNameLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sampleDataGridControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnDetailsChartControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valuesSectionUserControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowStatsUserControlcsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sampleDataTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.centerSectionTextSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateAtLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.previevOnlyLabelControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.centerSectionLayoutControl.Controls.Add(this.previevOnlyLabelControl);
		this.centerSectionLayoutControl.Controls.Add(this.dateAtLabelControl);
		this.centerSectionLayoutControl.Controls.Add(this.sampleDataTextLabelControl);
		this.centerSectionLayoutControl.Controls.Add(this.centerSectionTextSimpleButton);
		this.centerSectionLayoutControl.Controls.Add(this.rowStatsUserControlcs);
		this.centerSectionLayoutControl.Controls.Add(this.valuesSectionUserControl);
		this.centerSectionLayoutControl.Controls.Add(this.sampleDataGridControl);
		this.centerSectionLayoutControl.Controls.Add(this.columnDetailsChartControl);
		this.centerSectionLayoutControl.Controls.Add(this.columnDetailsTitleLabelControl);
		this.centerSectionLayoutControl.Controls.Add(this.rowStatsChartControl);
		this.centerSectionLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.centerSectionLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.centerSectionLayoutControl.MinimumSize = new System.Drawing.Size(200, 0);
		this.centerSectionLayoutControl.Name = "centerSectionLayoutControl";
		this.centerSectionLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2893, 578, 650, 400);
		this.centerSectionLayoutControl.Root = this.centerSectionLayoutControlGroup;
		this.centerSectionLayoutControl.Size = new System.Drawing.Size(641, 797);
		this.centerSectionLayoutControl.TabIndex = 13;
		this.centerSectionLayoutControl.Text = "layoutControl4";
		this.previevOnlyLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.previevOnlyLabelControl.Appearance.Options.UseFont = true;
		this.previevOnlyLabelControl.Enabled = false;
		this.previevOnlyLabelControl.Location = new System.Drawing.Point(2, 56);
		this.previevOnlyLabelControl.Name = "previevOnlyLabelControl";
		this.previevOnlyLabelControl.Size = new System.Drawing.Size(310, 16);
		this.previevOnlyLabelControl.StyleController = this.centerSectionLayoutControl;
		this.previevOnlyLabelControl.TabIndex = 66;
		this.previevOnlyLabelControl.Text = "This data is preview-only, it is not saved to repository!";
		this.dateAtLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold);
		this.dateAtLabelControl.Appearance.Options.UseFont = true;
		this.dateAtLabelControl.Location = new System.Drawing.Point(117, 19);
		this.dateAtLabelControl.Name = "dateAtLabelControl";
		this.dateAtLabelControl.Size = new System.Drawing.Size(193, 16);
		this.dateAtLabelControl.StyleController = this.centerSectionLayoutControl;
		this.dateAtLabelControl.TabIndex = 65;
		this.sampleDataTextLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.sampleDataTextLabelControl.Appearance.Options.UseFont = true;
		this.sampleDataTextLabelControl.Location = new System.Drawing.Point(2, 76);
		this.sampleDataTextLabelControl.Name = "sampleDataTextLabelControl";
		this.sampleDataTextLabelControl.Size = new System.Drawing.Size(637, 21);
		this.sampleDataTextLabelControl.StyleController = this.centerSectionLayoutControl;
		this.sampleDataTextLabelControl.TabIndex = 64;
		this.sampleDataTextLabelControl.Text = "Sample, randomly picked rows:  X";
		this.centerSectionTextSimpleButton.Appearance.Options.UseTextOptions = true;
		this.centerSectionTextSimpleButton.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.centerSectionTextSimpleButton.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.centerSectionTextSimpleButton.ImageOptions.Image = Dataedo.App.Properties.Resources.profile_column_32;
		this.centerSectionTextSimpleButton.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.centerSectionTextSimpleButton.Location = new System.Drawing.Point(266, 157);
		this.centerSectionTextSimpleButton.MaximumSize = new System.Drawing.Size(100, 100);
		this.centerSectionTextSimpleButton.Name = "centerSectionTextSimpleButton";
		this.centerSectionTextSimpleButton.Size = new System.Drawing.Size(100, 68);
		this.centerSectionTextSimpleButton.StyleController = this.centerSectionLayoutControl;
		this.centerSectionTextSimpleButton.TabIndex = 63;
		this.centerSectionTextSimpleButton.Text = "Profile Column";
		this.centerSectionTextSimpleButton.Click += new System.EventHandler(CenterSectionTextSimpleButton_Click);
		this.rowStatsUserControlcs.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.rowStatsUserControlcs.BackColor = System.Drawing.Color.Transparent;
		this.rowStatsUserControlcs.Location = new System.Drawing.Point(2, 229);
		this.rowStatsUserControlcs.Name = "rowStatsUserControlcs";
		this.rowStatsUserControlcs.ShowThisUserControl = true;
		this.rowStatsUserControlcs.Size = new System.Drawing.Size(637, 122);
		this.rowStatsUserControlcs.TabIndex = 61;
		this.valuesSectionUserControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
		this.valuesSectionUserControl.BackColor = System.Drawing.Color.Transparent;
		this.valuesSectionUserControl.Location = new System.Drawing.Point(2, 499);
		this.valuesSectionUserControl.Name = "valuesSectionUserControl";
		this.valuesSectionUserControl.Size = new System.Drawing.Size(637, 182);
		this.valuesSectionUserControl.TabIndex = 60;
		this.sampleDataGridControl.EmbeddedNavigator.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.sampleDataGridControl.EmbeddedNavigator.Appearance.Options.UseBackColor = true;
		this.sampleDataGridControl.Location = new System.Drawing.Point(2, 101);
		this.sampleDataGridControl.MainView = this.sampleDataGridControlGridView;
		this.sampleDataGridControl.MinimumSize = new System.Drawing.Size(400, 0);
		this.sampleDataGridControl.Name = "sampleDataGridControl";
		this.sampleDataGridControl.Size = new System.Drawing.Size(637, 52);
		this.sampleDataGridControl.TabIndex = 59;
		this.sampleDataGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.sampleDataGridControlGridView });
		this.sampleDataGridControlGridView.GridControl = this.sampleDataGridControl;
		this.sampleDataGridControlGridView.Name = "sampleDataGridControlGridView";
		this.sampleDataGridControlGridView.OptionsView.ShowGroupPanel = false;
		this.sampleDataGridControlGridView.OptionsView.ShowIndicator = false;
		this.columnDetailsChartControl.AutoLayout = false;
		this.columnDetailsChartControl.BackColor = System.Drawing.Color.Transparent;
		this.columnDetailsChartControl.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False;
		this.columnDetailsChartControl.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
		this.columnDetailsChartControl.CrosshairOptions.CrosshairLabelTextOptions.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
		this.columnDetailsChartControl.CrosshairOptions.GroupHeaderTextOptions.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
		xYDiagram.AxisX.Visibility = DevExpress.Utils.DefaultBoolean.False;
		xYDiagram.AxisX.VisibleInPanesSerializable = "-1";
		xYDiagram.AxisY.GridLines.Visible = false;
		xYDiagram.AxisY.Visibility = DevExpress.Utils.DefaultBoolean.True;
		xYDiagram.AxisY.VisibleInPanesSerializable = "-1";
		xYDiagram.AxisY.WholeRange.AlwaysShowZeroLevel = false;
		xYDiagram.AxisY.WholeRange.Auto = false;
		xYDiagram.AxisY.WholeRange.AutoSideMargins = false;
		xYDiagram.AxisY.WholeRange.MaxValueSerializable = "9.9";
		xYDiagram.AxisY.WholeRange.MinValueSerializable = "1.3";
		xYDiagram.AxisY.WholeRange.SideMarginsValue = 0.0;
		xYDiagram.Rotated = true;
		xYDiagram.RuntimePaneResize = true;
		this.columnDetailsChartControl.Diagram = xYDiagram;
		this.columnDetailsChartControl.EmptyChartText.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
		this.columnDetailsChartControl.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Center;
		this.columnDetailsChartControl.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.BottomOutside;
		this.columnDetailsChartControl.Legend.BackColor = System.Drawing.Color.Transparent;
		this.columnDetailsChartControl.Legend.Border.Visibility = DevExpress.Utils.DefaultBoolean.False;
		this.columnDetailsChartControl.Legend.Direction = DevExpress.XtraCharts.LegendDirection.LeftToRight;
		this.columnDetailsChartControl.Legend.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
		this.columnDetailsChartControl.Legend.Name = "Default Legend";
		this.columnDetailsChartControl.Legend.Visibility = DevExpress.Utils.DefaultBoolean.False;
		this.columnDetailsChartControl.Location = new System.Drawing.Point(50, 685);
		this.columnDetailsChartControl.Name = "columnDetailsChartControl";
		this.columnDetailsChartControl.OptionsPrint.SizeMode = DevExpress.XtraCharts.Printing.PrintSizeMode.Zoom;
		this.columnDetailsChartControl.RuntimeHitTesting = true;
		series.LabelsVisibility = DevExpress.Utils.DefaultBoolean.True;
		series.Name = "Series 1";
		series.ShowInLegend = false;
		sideBySideBarSeriesView.Border.Visibility = DevExpress.Utils.DefaultBoolean.True;
		sideBySideBarSeriesView.Color = System.Drawing.Color.FromArgb(79, 129, 189);
		series.View = sideBySideBarSeriesView;
		this.columnDetailsChartControl.SeriesSerializable = new DevExpress.XtraCharts.Series[1] { series };
		this.columnDetailsChartControl.SeriesTemplate.CrosshairTextOptions.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
		sideBySideBarSeriesLabel.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
		this.columnDetailsChartControl.SeriesTemplate.Label = sideBySideBarSeriesLabel;
		this.columnDetailsChartControl.Size = new System.Drawing.Size(541, 110);
		this.columnDetailsChartControl.SmallChartText.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
		this.columnDetailsChartControl.TabIndex = 5;
		this.columnDetailsChartControl.ToolTipController = this.currentValueChartControlToolTipController;
		this.columnDetailsChartControl.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.False;
		this.currentValueChartControlToolTipController.AutoPopDelay = 10000;
		this.currentValueChartControlToolTipController.InitialDelay = 1;
		this.currentValueChartControlToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.currentValueChartControlToolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(CurrentValueChartControlToolTipController_GetActiveObjectInfo);
		this.columnDetailsTitleLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold);
		this.columnDetailsTitleLabelControl.Appearance.Options.UseFont = true;
		this.columnDetailsTitleLabelControl.AutoEllipsis = true;
		this.columnDetailsTitleLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.columnDetailsTitleLabelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.columnDetailsTitleLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftBottom;
		this.columnDetailsTitleLabelControl.ImageOptions.Image = Dataedo.App.Properties.Resources.column_16;
		this.columnDetailsTitleLabelControl.Location = new System.Drawing.Point(2, 2);
		this.columnDetailsTitleLabelControl.Name = "columnDetailsTitleLabelControl";
		this.columnDetailsTitleLabelControl.Size = new System.Drawing.Size(111, 50);
		this.columnDetailsTitleLabelControl.StyleController = this.centerSectionLayoutControl;
		toolTipItem.Text = "Object name";
		superToolTip.Items.Add(toolTipItem);
		this.columnDetailsTitleLabelControl.SuperTip = superToolTip;
		this.columnDetailsTitleLabelControl.TabIndex = 9;
		this.columnDetailsTitleLabelControl.Text = "Object name";
		this.columnDetailsTitleLabelControl.TextChanged += new System.EventHandler(ColumnDetailsTitleLabelControl_TextChanged);
		this.rowStatsChartControl.AutoLayout = false;
		this.rowStatsChartControl.BackColor = System.Drawing.Color.Transparent;
		this.rowStatsChartControl.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False;
		this.rowStatsChartControl.CrosshairEnabled = DevExpress.Utils.DefaultBoolean.False;
		this.rowStatsChartControl.CrosshairOptions.CrosshairLabelTextOptions.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
		this.rowStatsChartControl.CrosshairOptions.GroupHeaderTextOptions.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
		xYDiagram2.AxisX.Visibility = DevExpress.Utils.DefaultBoolean.False;
		xYDiagram2.AxisX.VisibleInPanesSerializable = "-1";
		xYDiagram2.AxisY.Label.TextPattern = "{V:P0}";
		xYDiagram2.AxisY.VisibleInPanesSerializable = "-1";
		xYDiagram2.AxisY.WholeRange.Auto = false;
		xYDiagram2.AxisY.WholeRange.AutoSideMargins = false;
		xYDiagram2.AxisY.WholeRange.MaxValueSerializable = "1";
		xYDiagram2.AxisY.WholeRange.MinValueSerializable = "0";
		xYDiagram2.AxisY.WholeRange.SideMarginsValue = 0.0;
		xYDiagram2.Rotated = true;
		xYDiagram2.RuntimePaneResize = true;
		this.rowStatsChartControl.Diagram = xYDiagram2;
		this.rowStatsChartControl.EmptyChartText.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
		this.rowStatsChartControl.Legend.AlignmentHorizontal = DevExpress.XtraCharts.LegendAlignmentHorizontal.Center;
		this.rowStatsChartControl.Legend.AlignmentVertical = DevExpress.XtraCharts.LegendAlignmentVertical.BottomOutside;
		this.rowStatsChartControl.Legend.BackColor = System.Drawing.Color.Transparent;
		this.rowStatsChartControl.Legend.Border.Visibility = DevExpress.Utils.DefaultBoolean.False;
		this.rowStatsChartControl.Legend.Direction = DevExpress.XtraCharts.LegendDirection.LeftToRight;
		this.rowStatsChartControl.Legend.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
		this.rowStatsChartControl.Legend.Name = "Default Legend";
		this.rowStatsChartControl.Location = new System.Drawing.Point(50, 355);
		this.rowStatsChartControl.Name = "rowStatsChartControl";
		this.rowStatsChartControl.OptionsPrint.SizeMode = DevExpress.XtraCharts.Printing.PrintSizeMode.Zoom;
		this.rowStatsChartControl.RuntimeHitTesting = true;
		series2.LegendName = "Default Legend";
		series2.Name = "Series 1";
		series2.ShowInLegend = false;
		fullStackedBarSeriesView.BarWidth = 0.1;
		fullStackedBarSeriesView.Border.Visibility = DevExpress.Utils.DefaultBoolean.True;
		series2.View = fullStackedBarSeriesView;
		this.rowStatsChartControl.SeriesSerializable = new DevExpress.XtraCharts.Series[1] { series2 };
		this.rowStatsChartControl.SeriesTemplate.CrosshairTextOptions.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
		sideBySideBarSeriesLabel2.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
		this.rowStatsChartControl.SeriesTemplate.Label = sideBySideBarSeriesLabel2;
		this.rowStatsChartControl.Size = new System.Drawing.Size(541, 140);
		this.rowStatsChartControl.SmallChartText.EnableAntialiasing = DevExpress.Utils.DefaultBoolean.False;
		this.rowStatsChartControl.TabIndex = 4;
		this.rowStatsChartControl.ToolTipController = this.currentValueChartControlToolTipController;
		this.rowStatsChartControl.ToolTipEnabled = DevExpress.Utils.DefaultBoolean.False;
		this.centerSectionLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.centerSectionLayoutControlGroup.GroupBordersVisible = false;
		this.centerSectionLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[11]
		{
			this.rowStatsChartLayoutControlItem, this.columnNameLayoutControlItem, this.sampleDataGridControlLayoutControlItem, this.columnDetailsChartControlLayoutControlItem, this.valuesSectionUserControlLayoutControlItem, this.rowStatsUserControlcsLayoutControlItem, this.sampleDataTextLabelControlLayoutControlItem, this.centerSectionTextSimpleButtonLayoutControlItem, this.dateAtLabelControlLayoutControlItem, this.emptySpaceItem9,
			this.previevOnlyLabelControlLayoutControlItem
		});
		this.centerSectionLayoutControlGroup.Name = "Root";
		this.centerSectionLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.centerSectionLayoutControlGroup.Size = new System.Drawing.Size(641, 797);
		this.centerSectionLayoutControlGroup.TextVisible = false;
		this.rowStatsChartLayoutControlItem.Control = this.rowStatsChartControl;
		this.rowStatsChartLayoutControlItem.Location = new System.Drawing.Point(0, 353);
		this.rowStatsChartLayoutControlItem.MaxSize = new System.Drawing.Size(800, 144);
		this.rowStatsChartLayoutControlItem.MinSize = new System.Drawing.Size(150, 144);
		this.rowStatsChartLayoutControlItem.Name = "rowStatsChartLayoutControlItem";
		this.rowStatsChartLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(50, 50, 2, 2);
		this.rowStatsChartLayoutControlItem.Size = new System.Drawing.Size(641, 144);
		this.rowStatsChartLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.rowStatsChartLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.rowStatsChartLayoutControlItem.TextVisible = false;
		this.columnNameLayoutControlItem.Control = this.columnDetailsTitleLabelControl;
		this.columnNameLayoutControlItem.ImageOptions.Image = Dataedo.App.Properties.Resources.add_16;
		this.columnNameLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.columnNameLayoutControlItem.MaxSize = new System.Drawing.Size(600, 54);
		this.columnNameLayoutControlItem.MinSize = new System.Drawing.Size(38, 54);
		this.columnNameLayoutControlItem.Name = "columnNameLayoutControlItem";
		this.columnNameLayoutControlItem.Size = new System.Drawing.Size(115, 54);
		this.columnNameLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.columnNameLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.columnNameLayoutControlItem.TextVisible = false;
		this.sampleDataGridControlLayoutControlItem.Control = this.sampleDataGridControl;
		this.sampleDataGridControlLayoutControlItem.Location = new System.Drawing.Point(0, 99);
		this.sampleDataGridControlLayoutControlItem.Name = "sampleDataGridControlLayoutControlItem";
		this.sampleDataGridControlLayoutControlItem.Size = new System.Drawing.Size(641, 56);
		this.sampleDataGridControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.sampleDataGridControlLayoutControlItem.TextVisible = false;
		this.sampleDataGridControlLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.columnDetailsChartControlLayoutControlItem.Control = this.columnDetailsChartControl;
		this.columnDetailsChartControlLayoutControlItem.Location = new System.Drawing.Point(0, 683);
		this.columnDetailsChartControlLayoutControlItem.MaxSize = new System.Drawing.Size(800, 114);
		this.columnDetailsChartControlLayoutControlItem.MinSize = new System.Drawing.Size(150, 114);
		this.columnDetailsChartControlLayoutControlItem.Name = "columnDetailsChartControlLayoutControlItem";
		this.columnDetailsChartControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(50, 50, 2, 2);
		this.columnDetailsChartControlLayoutControlItem.Size = new System.Drawing.Size(641, 114);
		this.columnDetailsChartControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.columnDetailsChartControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.columnDetailsChartControlLayoutControlItem.TextVisible = false;
		this.columnDetailsChartControlLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.valuesSectionUserControlLayoutControlItem.Control = this.valuesSectionUserControl;
		this.valuesSectionUserControlLayoutControlItem.Location = new System.Drawing.Point(0, 497);
		this.valuesSectionUserControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 186);
		this.valuesSectionUserControlLayoutControlItem.MinSize = new System.Drawing.Size(100, 186);
		this.valuesSectionUserControlLayoutControlItem.Name = "valuesSectionUserControlLayoutControlItem";
		this.valuesSectionUserControlLayoutControlItem.Size = new System.Drawing.Size(641, 186);
		this.valuesSectionUserControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.valuesSectionUserControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.valuesSectionUserControlLayoutControlItem.TextVisible = false;
		this.rowStatsUserControlcsLayoutControlItem.Control = this.rowStatsUserControlcs;
		this.rowStatsUserControlcsLayoutControlItem.Location = new System.Drawing.Point(0, 227);
		this.rowStatsUserControlcsLayoutControlItem.MaxSize = new System.Drawing.Size(0, 126);
		this.rowStatsUserControlcsLayoutControlItem.MinSize = new System.Drawing.Size(150, 126);
		this.rowStatsUserControlcsLayoutControlItem.Name = "rowStatsUserControlcsLayoutControlItem";
		this.rowStatsUserControlcsLayoutControlItem.Size = new System.Drawing.Size(641, 126);
		this.rowStatsUserControlcsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.rowStatsUserControlcsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.rowStatsUserControlcsLayoutControlItem.TextVisible = false;
		this.sampleDataTextLabelControlLayoutControlItem.Control = this.sampleDataTextLabelControl;
		this.sampleDataTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 74);
		this.sampleDataTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 25);
		this.sampleDataTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(67, 25);
		this.sampleDataTextLabelControlLayoutControlItem.Name = "sampleDataTextLabelControlLayoutControlItem";
		this.sampleDataTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(641, 25);
		this.sampleDataTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.sampleDataTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.sampleDataTextLabelControlLayoutControlItem.TextVisible = false;
		this.centerSectionTextSimpleButtonLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.centerSectionTextSimpleButtonLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.centerSectionTextSimpleButtonLayoutControlItem.Control = this.centerSectionTextSimpleButton;
		this.centerSectionTextSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(0, 155);
		this.centerSectionTextSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(112, 57);
		this.centerSectionTextSimpleButtonLayoutControlItem.Name = "centerSectionTextSimpleButtonLayoutControlItem";
		this.centerSectionTextSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(641, 72);
		this.centerSectionTextSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.centerSectionTextSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.centerSectionTextSimpleButtonLayoutControlItem.TextVisible = false;
		this.dateAtLabelControlLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.dateAtLabelControlLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.dateAtLabelControlLayoutControlItem.Control = this.dateAtLabelControl;
		this.dateAtLabelControlLayoutControlItem.Location = new System.Drawing.Point(115, 0);
		this.dateAtLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(0, 54);
		this.dateAtLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(177, 54);
		this.dateAtLabelControlLayoutControlItem.Name = "dateAtLabelControlLayoutControlItem";
		this.dateAtLabelControlLayoutControlItem.Size = new System.Drawing.Size(197, 54);
		this.dateAtLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.dateAtLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.dateAtLabelControlLayoutControlItem.TextVisible = false;
		this.emptySpaceItem9.AllowHotTrack = false;
		this.emptySpaceItem9.Location = new System.Drawing.Point(312, 0);
		this.emptySpaceItem9.Name = "emptySpaceItem9";
		this.emptySpaceItem9.Size = new System.Drawing.Size(329, 54);
		this.emptySpaceItem9.TextSize = new System.Drawing.Size(0, 0);
		this.previevOnlyLabelControlLayoutControlItem.Control = this.previevOnlyLabelControl;
		this.previevOnlyLabelControlLayoutControlItem.Enabled = false;
		this.previevOnlyLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 54);
		this.previevOnlyLabelControlLayoutControlItem.Name = "previevOnlyLabelControlLayoutControlItem";
		this.previevOnlyLabelControlLayoutControlItem.Size = new System.Drawing.Size(641, 20);
		this.previevOnlyLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.previevOnlyLabelControlLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.centerSectionLayoutControl);
		base.Name = "CenterSectionUserControl";
		base.Size = new System.Drawing.Size(641, 797);
		((System.ComponentModel.ISupportInitialize)this.centerSectionLayoutControl).EndInit();
		this.centerSectionLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.sampleDataGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sampleDataGridControlGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)xYDiagram).EndInit();
		((System.ComponentModel.ISupportInitialize)sideBySideBarSeriesView).EndInit();
		((System.ComponentModel.ISupportInitialize)series).EndInit();
		((System.ComponentModel.ISupportInitialize)sideBySideBarSeriesLabel).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDetailsChartControl).EndInit();
		((System.ComponentModel.ISupportInitialize)xYDiagram2).EndInit();
		((System.ComponentModel.ISupportInitialize)fullStackedBarSeriesView).EndInit();
		((System.ComponentModel.ISupportInitialize)series2).EndInit();
		((System.ComponentModel.ISupportInitialize)sideBySideBarSeriesLabel2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowStatsChartControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.centerSectionLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowStatsChartLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnNameLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sampleDataGridControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnDetailsChartControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valuesSectionUserControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowStatsUserControlcsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sampleDataTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.centerSectionTextSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateAtLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.previevOnlyLabelControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
