using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Model.Data.DataProfiling;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.DataProfiling.DataProfilingUserControls;

public class RowStatsUserControlcs : BaseUserControl
{
	private readonly Stream layoutStream;

	private IContainer components;

	private NonCustomizableLayoutControl rowStatsUserControlLayoutControl;

	private LayoutControlGroup Root;

	private LabelControl nonDistinctRowTextLabelControl;

	private LayoutControlItem valuesNonDistinctTextLabelControlLayoutControlItem;

	private LabelControl nonDistinctRowValuesLabelControl;

	private LayoutControlItem nonDistinctValuesLabelControlLayoutControlItem;

	private LabelControl nonDistinctRowPercentLabelControl;

	private LayoutControlItem nonDistinctRowPercentLabelControlLayoutControlItem;

	private ProgressBarControl nonDistinctRowProgressBarControl;

	private LayoutControlItem layoutControlItem7;

	private LabelControl emptyRowTextLabelControl;

	private LayoutControlItem emptyRowsTextLabelControlLayoutControlItem;

	private LabelControl emptyRowCountValueLabelControl;

	private LayoutControlItem emptyRowCountValueLabelControlLayoutControlItem;

	private LabelControl emptyRowPercentCountLabelControl;

	private LayoutControlItem emptyRowPercentCountLabelControlLayoutControlItem;

	private ProgressBarControl emptyRowsProgressBarControl;

	private LayoutControlItem layoutControlItem18;

	private LabelControl nullRowsLabelControl;

	private LayoutControlItem nullRowsTextLabelControlLayoutControlItem;

	private LabelControl nullRowsValueLabelControl;

	private LayoutControlItem nullRowsValueLabelControlLayoutControlItem;

	private ProgressBarControl nullRowsProgressBarControl;

	private LayoutControlItem nullRowsProgressControlLayoutControlItem;

	private LabelControl rowCountValueLabelControl;

	private LayoutControlItem rowCountValueLabelControlLayoutControlItem;

	private LabelControl distinctRowTextLabelControl;

	private LayoutControlItem valuesDistinctRowTextLabelControlLayoutControlItem;

	private LabelControl distinctRowValueLabelControl;

	private LayoutControlItem valuesDistinctRowValuesLabelControlLayoutControlItem;

	private LabelControl distinctRowPercentLabelControl;

	private LayoutControlItem valuesDistinctRowPercentLabelControlLayoutControlItem;

	private ProgressBarControl distinctRowProgressBarControl;

	private LayoutControlItem valuesDistinctRowProgressBarControlLayoutControlItem;

	private PictureEdit distinctRowsPictureEdit;

	private LayoutControlItem distinctRowsPictureEditlayoutControlItem;

	private PictureEdit nonDistinctRowsPictureEdit1;

	private LayoutControlItem nonDistinctRowsPictureEditLayoutControlItem;

	private PictureEdit emptyRowsPictureEdit;

	private LayoutControlItem emptyRowsPictureEditLayoutControlItem;

	private PictureEdit nullRowsPictureEdit;

	private LayoutControlItem nullRowsPictureEditLayoutControlItem;

	private LabelControl rowsTitleLabelControl;

	private LayoutControlItem rowsTitleLabelControlLayoutControlItem;

	private LabelControl rowCountTextLabelControl;

	private LayoutControlItem rowCountLabelControlLayoutControlItem;

	private LabelControl nullRowsPercentLabelControl;

	private LayoutControlItem nullRowsPercentsLabelControlLayoutControlItem;

	public bool ShowThisUserControl { get; set; } = true;


	public RowStatsUserControlcs()
	{
		InitializeComponent();
		SetDefaultProgressBarControlStyle(nullRowsProgressBarControl);
		SetDefaultProgressBarControlStyle(emptyRowsProgressBarControl);
		SetDefaultProgressBarControlStyle(distinctRowProgressBarControl);
		SetDefaultProgressBarControlStyle(nonDistinctRowProgressBarControl);
		layoutStream = new MemoryStream();
		rowStatsUserControlLayoutControl.SaveLayoutToStream(layoutStream);
	}

	private void SetDefaultProgressBarControlStyle(ProgressBarControl progressBar)
	{
		progressBar.Properties.LookAndFeel.SetStyle(LookAndFeelStyle.Style3D, useWindowsXPTheme: false, useDefaultLookAndFeel: false);
	}

	public void SetParameters(ColumnProfiledDataObject column, bool isToolTip)
	{
		PresentValuesInRowsSection(column, isToolTip);
	}

	public void DoLeftPaddingManually()
	{
		rowsTitleLabelControl.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
		distinctRowsPictureEditlayoutControlItem.MinSize = new Size(25, 20);
		nonDistinctRowsPictureEditLayoutControlItem.MinSize = new Size(25, 20);
		emptyRowsPictureEditLayoutControlItem.MinSize = new Size(25, 20);
		nullRowsPictureEditLayoutControlItem.MinSize = new Size(25, 20);
		distinctRowsPictureEditlayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 0, 4, 4);
		nonDistinctRowsPictureEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 0, 4, 4);
		emptyRowsPictureEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 0, 4, 4);
		nullRowsPictureEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 0, 4, 4);
		rowCountTextLabelControl.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
	}

	public void ChangeSizeOfControl(int minWidth, int minHeight, int maxWidth, int maxHeight)
	{
		minHeight = 18;
		rowStatsUserControlLayoutControl.BeginUpdate();
		int num = 130;
		int num2 = 45;
		Size minSize = new Size(num, minHeight);
		Size maxSize = new Size(num, minHeight);
		Size size = new Size(num2, minHeight);
		Size maxSize2 = new Size(num2, minHeight);
		rowsTitleLabelControlLayoutControlItem.MinSize = minSize;
		rowsTitleLabelControlLayoutControlItem.MaxSize = maxSize;
		rowCountLabelControlLayoutControlItem.MinSize = minSize;
		rowCountLabelControlLayoutControlItem.MaxSize = maxSize;
		rowCountValueLabelControlLayoutControlItem.MinSize = size;
		rowCountValueLabelControlLayoutControlItem.MaxSize = size;
		valuesDistinctRowTextLabelControlLayoutControlItem.MinSize = minSize;
		valuesDistinctRowValuesLabelControlLayoutControlItem.MinSize = size;
		valuesDistinctRowPercentLabelControlLayoutControlItem.MinSize = size;
		valuesDistinctRowProgressBarControlLayoutControlItem.MinSize = size;
		valuesDistinctRowTextLabelControlLayoutControlItem.MaxSize = maxSize;
		valuesDistinctRowValuesLabelControlLayoutControlItem.MaxSize = maxSize2;
		valuesDistinctRowPercentLabelControlLayoutControlItem.MaxSize = maxSize2;
		valuesDistinctRowProgressBarControlLayoutControlItem.MaxSize = maxSize2;
		valuesNonDistinctTextLabelControlLayoutControlItem.MinSize = minSize;
		nonDistinctValuesLabelControlLayoutControlItem.MinSize = size;
		nonDistinctRowPercentLabelControlLayoutControlItem.MinSize = size;
		layoutControlItem7.MinSize = size;
		valuesNonDistinctTextLabelControlLayoutControlItem.MaxSize = maxSize;
		nonDistinctValuesLabelControlLayoutControlItem.MaxSize = maxSize2;
		nonDistinctRowPercentLabelControlLayoutControlItem.MaxSize = maxSize2;
		layoutControlItem7.MaxSize = maxSize2;
		emptyRowsTextLabelControlLayoutControlItem.MinSize = minSize;
		emptyRowCountValueLabelControlLayoutControlItem.MinSize = size;
		emptyRowPercentCountLabelControlLayoutControlItem.MinSize = size;
		layoutControlItem18.MinSize = size;
		emptyRowsTextLabelControlLayoutControlItem.MaxSize = maxSize;
		emptyRowCountValueLabelControlLayoutControlItem.MaxSize = maxSize2;
		emptyRowPercentCountLabelControlLayoutControlItem.MaxSize = maxSize2;
		layoutControlItem18.MaxSize = maxSize2;
		nullRowsTextLabelControlLayoutControlItem.MinSize = minSize;
		nullRowsValueLabelControlLayoutControlItem.MinSize = size;
		nullRowsPercentsLabelControlLayoutControlItem.MinSize = size;
		nullRowsProgressControlLayoutControlItem.MinSize = size;
		nullRowsTextLabelControlLayoutControlItem.MaxSize = maxSize;
		nullRowsValueLabelControlLayoutControlItem.MaxSize = maxSize2;
		nullRowsPercentsLabelControlLayoutControlItem.MaxSize = maxSize2;
		nullRowsProgressControlLayoutControlItem.MaxSize = maxSize2;
		rowStatsUserControlLayoutControl.EndUpdate();
	}

	private void PresentValuesInRowsSection(ColumnProfiledDataObject column, bool isToolTip)
	{
		rowsTitleLabelControl.Text = "Rows";
		double? num = 0.0;
		double? num2 = 0.0;
		double? num3 = 0.0;
		double? num4 = 0.0;
		if (column.RowCount.HasValue && column.RowCount != 0)
		{
			num = 1.0 * (double)column.ValuesNullRowCount.GetValueOrDefault() / (double?)column.RowCount * 100.0;
			num2 = 1.0 * (double)column.ValuesEmptyRowCount.GetValueOrDefault() / (double?)column.RowCount * 100.0;
			num3 = 1.0 * (double)column.ValuesDistinctRowCount.GetValueOrDefault() / (double?)column.RowCount * 100.0;
			num4 = 1.0 * (double)column.ValuesNondistinctRowCount.GetValueOrDefault() / (double?)column.RowCount * 100.0;
		}
		bool flag = DataTypeChecker.IsStringCoreType(column.ProfilingDataType) || (num == 0.0 && num2 == 0.0 && num3 == 0.0 && num4 == 0.0);
		if (isToolTip && flag)
		{
			ShowThisUserControl = false;
			return;
		}
		if (!isToolTip && flag)
		{
			HideAllInCenter(column);
			return;
		}
		if (!isToolTip && !flag)
		{
			layoutStream.Position = 0L;
			rowStatsUserControlLayoutControl.RestoreLayoutFromStream(layoutStream);
		}
		nullRowsLabelControl.Text = "NULL rows";
		nullRowsValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(column.ValuesNullRowCount) ?? "";
		if (num.HasValue)
		{
			nullRowsPercentLabelControl.Text = DataProfilingStringFormatter.FormatFloat1Values(num) + "%";
		}
		else
		{
			nullRowsPercentLabelControl.Text = string.Empty;
		}
		nullRowsProgressBarControl.EditValue = num;
		emptyRowTextLabelControl.Text = "Empty rows";
		emptyRowCountValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(column.ValuesEmptyRowCount) ?? "";
		if (num2.HasValue)
		{
			emptyRowPercentCountLabelControl.Text = DataProfilingStringFormatter.FormatFloat1Values(num2) + "%";
		}
		else
		{
			emptyRowPercentCountLabelControl.Text = string.Empty;
		}
		emptyRowsProgressBarControl.EditValue = num2;
		distinctRowTextLabelControl.Text = "Distinct values rows";
		distinctRowValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(column.ValuesDistinctRowCount) ?? "";
		distinctRowPercentLabelControl.Text = $"{num3}";
		if (num3.HasValue)
		{
			distinctRowPercentLabelControl.Text = DataProfilingStringFormatter.FormatFloat1Values(num3) + "%";
		}
		else
		{
			distinctRowPercentLabelControl.Text = string.Empty;
		}
		distinctRowProgressBarControl.EditValue = num3;
		nonDistinctRowTextLabelControl.Text = "Non distinct values rows";
		nonDistinctRowValuesLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(column.ValuesNondistinctRowCount) ?? "";
		if (num4.HasValue)
		{
			nonDistinctRowPercentLabelControl.Text = DataProfilingStringFormatter.FormatFloat1Values(num4) + "%";
		}
		else
		{
			nonDistinctRowPercentLabelControl.Text = string.Empty;
		}
		nonDistinctRowProgressBarControl.EditValue = num4;
		rowCountTextLabelControl.Text = "Total rows";
		rowCountValueLabelControl.Text = DataProfilingStringFormatter.Format2FloatInteligentValues(column.RowCount) ?? "";
		DisableOrEnabableRowsLabels(column.ValuesNullRowCount, column.ValuesEmptyRowCount, column.ValuesDistinctRowCount, column.ValuesNondistinctRowCount, column.RowCount);
	}

	private void DisableOrEnabableRowsLabels(long? nullRowCount, long? emptyRowCount, long? distinctRowCount, long? nonDistinct, long? totalRowCount)
	{
		if (nullRowCount == 0 || !nullRowCount.HasValue)
		{
			nullRowsTextLabelControlLayoutControlItem.Enabled = false;
			nullRowsValueLabelControlLayoutControlItem.Enabled = false;
			nullRowsPercentsLabelControlLayoutControlItem.Enabled = false;
		}
		else
		{
			nullRowsTextLabelControlLayoutControlItem.Enabled = true;
			nullRowsValueLabelControlLayoutControlItem.Enabled = true;
			nullRowsPercentsLabelControlLayoutControlItem.Enabled = true;
		}
		if (emptyRowCount == 0 || !emptyRowCount.HasValue)
		{
			emptyRowsTextLabelControlLayoutControlItem.Enabled = false;
			emptyRowCountValueLabelControlLayoutControlItem.Enabled = false;
			emptyRowPercentCountLabelControlLayoutControlItem.Enabled = false;
		}
		else
		{
			emptyRowsTextLabelControlLayoutControlItem.Enabled = true;
			emptyRowCountValueLabelControlLayoutControlItem.Enabled = true;
			emptyRowPercentCountLabelControlLayoutControlItem.Enabled = true;
		}
		if (distinctRowCount == 0 || !distinctRowCount.HasValue)
		{
			valuesDistinctRowTextLabelControlLayoutControlItem.Enabled = false;
			valuesDistinctRowValuesLabelControlLayoutControlItem.Enabled = false;
			valuesDistinctRowPercentLabelControlLayoutControlItem.Enabled = false;
		}
		else
		{
			valuesDistinctRowTextLabelControlLayoutControlItem.Enabled = true;
			valuesDistinctRowValuesLabelControlLayoutControlItem.Enabled = true;
			valuesDistinctRowPercentLabelControlLayoutControlItem.Enabled = true;
		}
		if (nonDistinct == 0 || !nonDistinct.HasValue)
		{
			valuesNonDistinctTextLabelControlLayoutControlItem.Enabled = false;
			nonDistinctValuesLabelControlLayoutControlItem.Enabled = false;
			nonDistinctRowPercentLabelControlLayoutControlItem.Enabled = false;
		}
		else
		{
			valuesNonDistinctTextLabelControlLayoutControlItem.Enabled = true;
			nonDistinctValuesLabelControlLayoutControlItem.Enabled = true;
			nonDistinctRowPercentLabelControlLayoutControlItem.Enabled = true;
		}
		if (totalRowCount == 0 || !totalRowCount.HasValue)
		{
			rowCountLabelControlLayoutControlItem.Enabled = false;
			rowCountValueLabelControlLayoutControlItem.Enabled = false;
		}
		else
		{
			rowCountLabelControlLayoutControlItem.Enabled = true;
			rowCountValueLabelControlLayoutControlItem.Enabled = true;
		}
	}

	private void HideAllInCenter(ColumnProfiledDataObject column)
	{
		rowStatsUserControlLayoutControl.BeginUpdate();
		layoutStream.Position = 0L;
		rowStatsUserControlLayoutControl.RestoreLayoutFromStream(layoutStream);
		if (!column.RowCount.HasValue || column.RowCount == 0)
		{
			rowsTitleLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			rowsTitleLabelControlLayoutControlItem.HideToCustomization();
		}
		distinctRowsPictureEditlayoutControlItem.Visibility = LayoutVisibility.Never;
		distinctRowsPictureEditlayoutControlItem.HideToCustomization();
		valuesDistinctRowTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		valuesDistinctRowTextLabelControlLayoutControlItem.HideToCustomization();
		valuesDistinctRowValuesLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		valuesDistinctRowValuesLabelControlLayoutControlItem.HideToCustomization();
		valuesDistinctRowPercentLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		valuesDistinctRowPercentLabelControlLayoutControlItem.HideToCustomization();
		valuesDistinctRowProgressBarControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		valuesDistinctRowProgressBarControlLayoutControlItem.HideToCustomization();
		nonDistinctRowsPictureEditLayoutControlItem.Visibility = LayoutVisibility.Never;
		nonDistinctRowsPictureEditLayoutControlItem.HideToCustomization();
		valuesNonDistinctTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		valuesNonDistinctTextLabelControlLayoutControlItem.HideToCustomization();
		nonDistinctValuesLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		nonDistinctValuesLabelControlLayoutControlItem.HideToCustomization();
		nonDistinctRowPercentLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		nonDistinctRowPercentLabelControlLayoutControlItem.HideToCustomization();
		layoutControlItem7.Visibility = LayoutVisibility.Never;
		layoutControlItem7.HideToCustomization();
		emptyRowsPictureEditLayoutControlItem.Visibility = LayoutVisibility.Never;
		emptyRowsPictureEditLayoutControlItem.HideToCustomization();
		emptyRowCountValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		emptyRowCountValueLabelControlLayoutControlItem.HideToCustomization();
		emptyRowsTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		emptyRowsTextLabelControlLayoutControlItem.HideToCustomization();
		emptyRowPercentCountLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		emptyRowPercentCountLabelControlLayoutControlItem.HideToCustomization();
		layoutControlItem18.Visibility = LayoutVisibility.Never;
		layoutControlItem18.HideToCustomization();
		nullRowsPictureEditLayoutControlItem.Visibility = LayoutVisibility.Never;
		nullRowsPictureEditLayoutControlItem.HideToCustomization();
		nullRowsTextLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		nullRowsTextLabelControlLayoutControlItem.HideToCustomization();
		nullRowsValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		nullRowsValueLabelControlLayoutControlItem.HideToCustomization();
		nullRowsPercentsLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		nullRowsPercentsLabelControlLayoutControlItem.HideToCustomization();
		nullRowsProgressControlLayoutControlItem.Visibility = LayoutVisibility.Never;
		nullRowsProgressControlLayoutControlItem.HideToCustomization();
		if (!column.RowCount.HasValue || column.RowCount == 0)
		{
			rowCountLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			rowCountLabelControlLayoutControlItem.HideToCustomization();
			rowCountValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Never;
			rowCountValueLabelControlLayoutControlItem.HideToCustomization();
		}
		else
		{
			rowCountLabelControlLayoutControlItem.Visibility = LayoutVisibility.Always;
			rowCountValueLabelControlLayoutControlItem.Visibility = LayoutVisibility.Always;
			rowCountValueLabelControl.Text = $"{column.RowCount}";
		}
		rowStatsUserControlLayoutControl.EndUpdate();
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
		this.rowStatsUserControlLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.nullRowsPercentLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.distinctRowProgressBarControl = new DevExpress.XtraEditors.ProgressBarControl();
		this.distinctRowPercentLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.distinctRowValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.distinctRowTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.rowCountValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.nullRowsProgressBarControl = new DevExpress.XtraEditors.ProgressBarControl();
		this.nullRowsValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.nullRowsLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.emptyRowsProgressBarControl = new DevExpress.XtraEditors.ProgressBarControl();
		this.emptyRowPercentCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.emptyRowCountValueLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.emptyRowTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.nonDistinctRowProgressBarControl = new DevExpress.XtraEditors.ProgressBarControl();
		this.nonDistinctRowPercentLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.nonDistinctRowValuesLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.nonDistinctRowTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.distinctRowsPictureEdit = new DevExpress.XtraEditors.PictureEdit();
		this.nonDistinctRowsPictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
		this.emptyRowsPictureEdit = new DevExpress.XtraEditors.PictureEdit();
		this.nullRowsPictureEdit = new DevExpress.XtraEditors.PictureEdit();
		this.rowsTitleLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.rowCountTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.valuesNonDistinctTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.nonDistinctValuesLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.nonDistinctRowPercentLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptyRowsTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptyRowCountValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptyRowPercentCountLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem18 = new DevExpress.XtraLayout.LayoutControlItem();
		this.nullRowsTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.nullRowsValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.nullRowsProgressControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.rowCountValueLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.valuesDistinctRowTextLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.valuesDistinctRowValuesLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.valuesDistinctRowPercentLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.valuesDistinctRowProgressBarControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.distinctRowsPictureEditlayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.nonDistinctRowsPictureEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptyRowsPictureEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.nullRowsPictureEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.rowsTitleLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.rowCountLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.nullRowsPercentsLabelControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.rowStatsUserControlLayoutControl).BeginInit();
		this.rowStatsUserControlLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.distinctRowProgressBarControl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nullRowsProgressBarControl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptyRowsProgressBarControl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nonDistinctRowProgressBarControl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.distinctRowsPictureEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nonDistinctRowsPictureEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptyRowsPictureEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nullRowsPictureEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valuesNonDistinctTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nonDistinctValuesLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nonDistinctRowPercentLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptyRowsTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptyRowCountValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptyRowPercentCountLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nullRowsTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nullRowsValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nullRowsProgressControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowCountValueLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valuesDistinctRowTextLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valuesDistinctRowValuesLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valuesDistinctRowPercentLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valuesDistinctRowProgressBarControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.distinctRowsPictureEditlayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nonDistinctRowsPictureEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptyRowsPictureEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nullRowsPictureEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowsTitleLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rowCountLabelControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nullRowsPercentsLabelControlLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.rowStatsUserControlLayoutControl.AutoSize = true;
		this.rowStatsUserControlLayoutControl.Controls.Add(this.nullRowsPercentLabelControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.distinctRowProgressBarControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.distinctRowPercentLabelControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.distinctRowValueLabelControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.distinctRowTextLabelControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.rowCountValueLabelControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.nullRowsProgressBarControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.nullRowsValueLabelControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.nullRowsLabelControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.emptyRowsProgressBarControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.emptyRowPercentCountLabelControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.emptyRowCountValueLabelControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.emptyRowTextLabelControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.nonDistinctRowProgressBarControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.nonDistinctRowPercentLabelControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.nonDistinctRowValuesLabelControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.nonDistinctRowTextLabelControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.distinctRowsPictureEdit);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.nonDistinctRowsPictureEdit1);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.emptyRowsPictureEdit);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.nullRowsPictureEdit);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.rowsTitleLabelControl);
		this.rowStatsUserControlLayoutControl.Controls.Add(this.rowCountTextLabelControl);
		this.rowStatsUserControlLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.rowStatsUserControlLayoutControl.Name = "rowStatsUserControlLayoutControl";
		this.rowStatsUserControlLayoutControl.Root = this.Root;
		this.rowStatsUserControlLayoutControl.Size = new System.Drawing.Size(465, 118);
		this.rowStatsUserControlLayoutControl.TabIndex = 0;
		this.rowStatsUserControlLayoutControl.Text = "layoutControl1";
		this.nullRowsPercentLabelControl.Appearance.Options.UseTextOptions = true;
		this.nullRowsPercentLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.nullRowsPercentLabelControl.Location = new System.Drawing.Point(350, 79);
		this.nullRowsPercentLabelControl.Name = "nullRowsPercentLabelControl";
		this.nullRowsPercentLabelControl.Size = new System.Drawing.Size(56, 14);
		this.nullRowsPercentLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.nullRowsPercentLabelControl.TabIndex = 59;
		this.distinctRowProgressBarControl.Location = new System.Drawing.Point(410, 25);
		this.distinctRowProgressBarControl.Name = "distinctRowProgressBarControl";
		this.distinctRowProgressBarControl.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.distinctRowProgressBarControl.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.distinctRowProgressBarControl.Properties.EndColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.distinctRowProgressBarControl.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.distinctRowProgressBarControl.Properties.StartColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.distinctRowProgressBarControl.Size = new System.Drawing.Size(50, 14);
		this.distinctRowProgressBarControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.distinctRowProgressBarControl.TabIndex = 58;
		this.distinctRowPercentLabelControl.Appearance.Options.UseTextOptions = true;
		this.distinctRowPercentLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.distinctRowPercentLabelControl.Location = new System.Drawing.Point(350, 25);
		this.distinctRowPercentLabelControl.Name = "distinctRowPercentLabelControl";
		this.distinctRowPercentLabelControl.Size = new System.Drawing.Size(56, 14);
		this.distinctRowPercentLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.distinctRowPercentLabelControl.TabIndex = 57;
		this.distinctRowValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.distinctRowValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.distinctRowValueLabelControl.Location = new System.Drawing.Point(260, 25);
		this.distinctRowValueLabelControl.Name = "distinctRowValueLabelControl";
		this.distinctRowValueLabelControl.Size = new System.Drawing.Size(86, 14);
		this.distinctRowValueLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.distinctRowValueLabelControl.TabIndex = 56;
		this.distinctRowTextLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftTop;
		this.distinctRowTextLabelControl.Location = new System.Drawing.Point(40, 25);
		this.distinctRowTextLabelControl.Name = "distinctRowTextLabelControl";
		this.distinctRowTextLabelControl.Size = new System.Drawing.Size(216, 14);
		this.distinctRowTextLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.distinctRowTextLabelControl.TabIndex = 55;
		this.distinctRowTextLabelControl.Text = "Distinct rows";
		this.rowCountValueLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.rowCountValueLabelControl.Appearance.Options.UseFont = true;
		this.rowCountValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.rowCountValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.rowCountValueLabelControl.Location = new System.Drawing.Point(260, 99);
		this.rowCountValueLabelControl.Name = "rowCountValueLabelControl";
		this.rowCountValueLabelControl.Size = new System.Drawing.Size(86, 14);
		this.rowCountValueLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.rowCountValueLabelControl.TabIndex = 54;
		this.nullRowsProgressBarControl.Location = new System.Drawing.Point(410, 79);
		this.nullRowsProgressBarControl.MaximumSize = new System.Drawing.Size(200, 0);
		this.nullRowsProgressBarControl.Name = "nullRowsProgressBarControl";
		this.nullRowsProgressBarControl.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.nullRowsProgressBarControl.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.nullRowsProgressBarControl.Properties.EndColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.nullRowsProgressBarControl.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.nullRowsProgressBarControl.Properties.StartColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.nullRowsProgressBarControl.Size = new System.Drawing.Size(47, 14);
		this.nullRowsProgressBarControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.nullRowsProgressBarControl.TabIndex = 52;
		this.nullRowsValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.nullRowsValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.nullRowsValueLabelControl.Location = new System.Drawing.Point(260, 79);
		this.nullRowsValueLabelControl.Name = "nullRowsValueLabelControl";
		this.nullRowsValueLabelControl.Size = new System.Drawing.Size(86, 14);
		this.nullRowsValueLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.nullRowsValueLabelControl.TabIndex = 51;
		this.nullRowsLabelControl.ImageOptions.Alignment = System.Drawing.ContentAlignment.MiddleLeft;
		this.nullRowsLabelControl.Location = new System.Drawing.Point(40, 79);
		this.nullRowsLabelControl.Margin = new System.Windows.Forms.Padding(0);
		this.nullRowsLabelControl.Name = "nullRowsLabelControl";
		this.nullRowsLabelControl.Size = new System.Drawing.Size(216, 14);
		this.nullRowsLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.nullRowsLabelControl.TabIndex = 50;
		this.nullRowsLabelControl.Text = "Null rows";
		this.emptyRowsProgressBarControl.Location = new System.Drawing.Point(410, 61);
		this.emptyRowsProgressBarControl.Name = "emptyRowsProgressBarControl";
		this.emptyRowsProgressBarControl.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.emptyRowsProgressBarControl.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.emptyRowsProgressBarControl.Properties.EndColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.emptyRowsProgressBarControl.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.emptyRowsProgressBarControl.Properties.StartColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.emptyRowsProgressBarControl.Size = new System.Drawing.Size(50, 14);
		this.emptyRowsProgressBarControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.emptyRowsProgressBarControl.TabIndex = 48;
		this.emptyRowPercentCountLabelControl.Appearance.Options.UseTextOptions = true;
		this.emptyRowPercentCountLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.emptyRowPercentCountLabelControl.Location = new System.Drawing.Point(350, 61);
		this.emptyRowPercentCountLabelControl.MaximumSize = new System.Drawing.Size(60, 0);
		this.emptyRowPercentCountLabelControl.Name = "emptyRowPercentCountLabelControl";
		this.emptyRowPercentCountLabelControl.Size = new System.Drawing.Size(56, 14);
		this.emptyRowPercentCountLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.emptyRowPercentCountLabelControl.TabIndex = 47;
		this.emptyRowCountValueLabelControl.Appearance.Options.UseTextOptions = true;
		this.emptyRowCountValueLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.emptyRowCountValueLabelControl.Location = new System.Drawing.Point(260, 61);
		this.emptyRowCountValueLabelControl.Name = "emptyRowCountValueLabelControl";
		this.emptyRowCountValueLabelControl.Size = new System.Drawing.Size(86, 14);
		this.emptyRowCountValueLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.emptyRowCountValueLabelControl.TabIndex = 46;
		this.emptyRowTextLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftTop;
		this.emptyRowTextLabelControl.Location = new System.Drawing.Point(40, 61);
		this.emptyRowTextLabelControl.Name = "emptyRowTextLabelControl";
		this.emptyRowTextLabelControl.Size = new System.Drawing.Size(216, 14);
		this.emptyRowTextLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.emptyRowTextLabelControl.TabIndex = 45;
		this.emptyRowTextLabelControl.Text = "Empty Rows";
		this.nonDistinctRowProgressBarControl.Location = new System.Drawing.Point(410, 43);
		this.nonDistinctRowProgressBarControl.Name = "nonDistinctRowProgressBarControl";
		this.nonDistinctRowProgressBarControl.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.nonDistinctRowProgressBarControl.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.nonDistinctRowProgressBarControl.Properties.EndColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.nonDistinctRowProgressBarControl.Properties.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.nonDistinctRowProgressBarControl.Properties.StartColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.nonDistinctRowProgressBarControl.Size = new System.Drawing.Size(50, 14);
		this.nonDistinctRowProgressBarControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.nonDistinctRowProgressBarControl.TabIndex = 43;
		this.nonDistinctRowPercentLabelControl.Appearance.Options.UseTextOptions = true;
		this.nonDistinctRowPercentLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.nonDistinctRowPercentLabelControl.Location = new System.Drawing.Point(350, 43);
		this.nonDistinctRowPercentLabelControl.Name = "nonDistinctRowPercentLabelControl";
		this.nonDistinctRowPercentLabelControl.Size = new System.Drawing.Size(56, 14);
		this.nonDistinctRowPercentLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.nonDistinctRowPercentLabelControl.TabIndex = 42;
		this.nonDistinctRowValuesLabelControl.Appearance.Options.UseTextOptions = true;
		this.nonDistinctRowValuesLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.nonDistinctRowValuesLabelControl.Location = new System.Drawing.Point(260, 43);
		this.nonDistinctRowValuesLabelControl.Name = "nonDistinctRowValuesLabelControl";
		this.nonDistinctRowValuesLabelControl.Size = new System.Drawing.Size(86, 14);
		this.nonDistinctRowValuesLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.nonDistinctRowValuesLabelControl.TabIndex = 41;
		this.nonDistinctRowTextLabelControl.ImageAlignToText = DevExpress.XtraEditors.ImageAlignToText.LeftTop;
		this.nonDistinctRowTextLabelControl.Location = new System.Drawing.Point(40, 43);
		this.nonDistinctRowTextLabelControl.Name = "nonDistinctRowTextLabelControl";
		this.nonDistinctRowTextLabelControl.Size = new System.Drawing.Size(216, 14);
		this.nonDistinctRowTextLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.nonDistinctRowTextLabelControl.TabIndex = 40;
		this.nonDistinctRowTextLabelControl.Text = "Non distinct rows";
		this.distinctRowsPictureEdit.Location = new System.Drawing.Point(26, 29);
		this.distinctRowsPictureEdit.Margin = new System.Windows.Forms.Padding(0);
		this.distinctRowsPictureEdit.MaximumSize = new System.Drawing.Size(9, 9);
		this.distinctRowsPictureEdit.MinimumSize = new System.Drawing.Size(9, 9);
		this.distinctRowsPictureEdit.Name = "distinctRowsPictureEdit";
		this.distinctRowsPictureEdit.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(214, 132, 41);
		this.distinctRowsPictureEdit.Properties.Appearance.Options.UseBackColor = true;
		this.distinctRowsPictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.distinctRowsPictureEdit.Properties.NullText = " ";
		this.distinctRowsPictureEdit.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
		this.distinctRowsPictureEdit.Size = new System.Drawing.Size(9, 9);
		this.distinctRowsPictureEdit.StyleController = this.rowStatsUserControlLayoutControl;
		this.distinctRowsPictureEdit.TabIndex = 38;
		this.nonDistinctRowsPictureEdit1.Location = new System.Drawing.Point(26, 47);
		this.nonDistinctRowsPictureEdit1.Margin = new System.Windows.Forms.Padding(0);
		this.nonDistinctRowsPictureEdit1.MaximumSize = new System.Drawing.Size(9, 9);
		this.nonDistinctRowsPictureEdit1.MinimumSize = new System.Drawing.Size(9, 9);
		this.nonDistinctRowsPictureEdit1.Name = "nonDistinctRowsPictureEdit1";
		this.nonDistinctRowsPictureEdit1.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(68, 114, 196);
		this.nonDistinctRowsPictureEdit1.Properties.Appearance.Options.UseBackColor = true;
		this.nonDistinctRowsPictureEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.nonDistinctRowsPictureEdit1.Properties.NullText = " ";
		this.nonDistinctRowsPictureEdit1.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
		this.nonDistinctRowsPictureEdit1.Size = new System.Drawing.Size(9, 9);
		this.nonDistinctRowsPictureEdit1.StyleController = this.rowStatsUserControlLayoutControl;
		this.nonDistinctRowsPictureEdit1.TabIndex = 38;
		this.emptyRowsPictureEdit.Location = new System.Drawing.Point(26, 65);
		this.emptyRowsPictureEdit.Margin = new System.Windows.Forms.Padding(0);
		this.emptyRowsPictureEdit.MaximumSize = new System.Drawing.Size(9, 9);
		this.emptyRowsPictureEdit.MinimumSize = new System.Drawing.Size(9, 9);
		this.emptyRowsPictureEdit.Name = "emptyRowsPictureEdit";
		this.emptyRowsPictureEdit.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(163, 164, 164);
		this.emptyRowsPictureEdit.Properties.Appearance.Options.UseBackColor = true;
		this.emptyRowsPictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.emptyRowsPictureEdit.Properties.NullText = " ";
		this.emptyRowsPictureEdit.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
		this.emptyRowsPictureEdit.Size = new System.Drawing.Size(9, 9);
		this.emptyRowsPictureEdit.StyleController = this.rowStatsUserControlLayoutControl;
		this.emptyRowsPictureEdit.TabIndex = 38;
		this.nullRowsPictureEdit.Location = new System.Drawing.Point(26, 83);
		this.nullRowsPictureEdit.Margin = new System.Windows.Forms.Padding(0);
		this.nullRowsPictureEdit.MaximumSize = new System.Drawing.Size(9, 9);
		this.nullRowsPictureEdit.MinimumSize = new System.Drawing.Size(9, 9);
		this.nullRowsPictureEdit.Name = "nullRowsPictureEdit";
		this.nullRowsPictureEdit.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(226, 226, 226);
		this.nullRowsPictureEdit.Properties.Appearance.Options.UseBackColor = true;
		this.nullRowsPictureEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.nullRowsPictureEdit.Properties.NullText = " ";
		this.nullRowsPictureEdit.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Auto;
		this.nullRowsPictureEdit.Size = new System.Drawing.Size(9, 9);
		this.nullRowsPictureEdit.StyleController = this.rowStatsUserControlLayoutControl;
		this.nullRowsPictureEdit.TabIndex = 38;
		this.rowsTitleLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold);
		this.rowsTitleLabelControl.Appearance.Options.UseFont = true;
		this.rowsTitleLabelControl.Location = new System.Drawing.Point(3, 3);
		this.rowsTitleLabelControl.Name = "rowsTitleLabelControl";
		this.rowsTitleLabelControl.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
		this.rowsTitleLabelControl.Size = new System.Drawing.Size(55, 20);
		this.rowsTitleLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.rowsTitleLabelControl.TabIndex = 10;
		this.rowsTitleLabelControl.Text = "Rows";
		this.rowCountTextLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.rowCountTextLabelControl.Appearance.Options.UseFont = true;
		this.rowCountTextLabelControl.Location = new System.Drawing.Point(5, 99);
		this.rowCountTextLabelControl.Name = "rowCountTextLabelControl";
		this.rowCountTextLabelControl.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
		this.rowCountTextLabelControl.Size = new System.Drawing.Size(251, 14);
		this.rowCountTextLabelControl.StyleController = this.rowStatsUserControlLayoutControl;
		this.rowCountTextLabelControl.TabIndex = 15;
		this.rowCountTextLabelControl.Text = "Row Count";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[23]
		{
			this.valuesNonDistinctTextLabelControlLayoutControlItem, this.nonDistinctValuesLabelControlLayoutControlItem, this.nonDistinctRowPercentLabelControlLayoutControlItem, this.layoutControlItem7, this.emptyRowsTextLabelControlLayoutControlItem, this.emptyRowCountValueLabelControlLayoutControlItem, this.emptyRowPercentCountLabelControlLayoutControlItem, this.layoutControlItem18, this.nullRowsTextLabelControlLayoutControlItem, this.nullRowsValueLabelControlLayoutControlItem,
			this.nullRowsProgressControlLayoutControlItem, this.rowCountValueLabelControlLayoutControlItem, this.valuesDistinctRowTextLabelControlLayoutControlItem, this.valuesDistinctRowValuesLabelControlLayoutControlItem, this.valuesDistinctRowPercentLabelControlLayoutControlItem, this.valuesDistinctRowProgressBarControlLayoutControlItem, this.distinctRowsPictureEditlayoutControlItem, this.nonDistinctRowsPictureEditLayoutControlItem, this.emptyRowsPictureEditLayoutControlItem, this.nullRowsPictureEditLayoutControlItem,
			this.rowsTitleLabelControlLayoutControlItem, this.rowCountLabelControlLayoutControlItem, this.nullRowsPercentsLabelControlLayoutControlItem
		});
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
		this.Root.Size = new System.Drawing.Size(465, 118);
		this.Root.TextVisible = false;
		this.valuesNonDistinctTextLabelControlLayoutControlItem.Control = this.nonDistinctRowTextLabelControl;
		this.valuesNonDistinctTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(35, 38);
		this.valuesNonDistinctTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(220, 18);
		this.valuesNonDistinctTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(220, 18);
		this.valuesNonDistinctTextLabelControlLayoutControlItem.Name = "valuesNonDistinctTextLabelControlLayoutControlItem";
		this.valuesNonDistinctTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(220, 18);
		this.valuesNonDistinctTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.valuesNonDistinctTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.valuesNonDistinctTextLabelControlLayoutControlItem.TextVisible = false;
		this.nonDistinctValuesLabelControlLayoutControlItem.Control = this.nonDistinctRowValuesLabelControl;
		this.nonDistinctValuesLabelControlLayoutControlItem.Location = new System.Drawing.Point(255, 38);
		this.nonDistinctValuesLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(90, 18);
		this.nonDistinctValuesLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(90, 18);
		this.nonDistinctValuesLabelControlLayoutControlItem.Name = "nonDistinctValuesLabelControlLayoutControlItem";
		this.nonDistinctValuesLabelControlLayoutControlItem.Size = new System.Drawing.Size(90, 18);
		this.nonDistinctValuesLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.nonDistinctValuesLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.nonDistinctValuesLabelControlLayoutControlItem.TextVisible = false;
		this.nonDistinctRowPercentLabelControlLayoutControlItem.Control = this.nonDistinctRowPercentLabelControl;
		this.nonDistinctRowPercentLabelControlLayoutControlItem.Location = new System.Drawing.Point(345, 38);
		this.nonDistinctRowPercentLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(60, 18);
		this.nonDistinctRowPercentLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(60, 18);
		this.nonDistinctRowPercentLabelControlLayoutControlItem.Name = "nonDistinctRowPercentLabelControlLayoutControlItem";
		this.nonDistinctRowPercentLabelControlLayoutControlItem.Size = new System.Drawing.Size(60, 18);
		this.nonDistinctRowPercentLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.nonDistinctRowPercentLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.nonDistinctRowPercentLabelControlLayoutControlItem.TextVisible = false;
		this.layoutControlItem7.Control = this.nonDistinctRowProgressBarControl;
		this.layoutControlItem7.Location = new System.Drawing.Point(405, 38);
		this.layoutControlItem7.MaxSize = new System.Drawing.Size(54, 18);
		this.layoutControlItem7.MinSize = new System.Drawing.Size(54, 18);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Size = new System.Drawing.Size(54, 18);
		this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.TextVisible = false;
		this.emptyRowsTextLabelControlLayoutControlItem.Control = this.emptyRowTextLabelControl;
		this.emptyRowsTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(35, 56);
		this.emptyRowsTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(220, 18);
		this.emptyRowsTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(220, 18);
		this.emptyRowsTextLabelControlLayoutControlItem.Name = "emptyRowsTextLabelControlLayoutControlItem";
		this.emptyRowsTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(220, 18);
		this.emptyRowsTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptyRowsTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.emptyRowsTextLabelControlLayoutControlItem.TextVisible = false;
		this.emptyRowCountValueLabelControlLayoutControlItem.Control = this.emptyRowCountValueLabelControl;
		this.emptyRowCountValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(255, 56);
		this.emptyRowCountValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(90, 18);
		this.emptyRowCountValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(90, 18);
		this.emptyRowCountValueLabelControlLayoutControlItem.Name = "emptyRowCountValueLabelControlLayoutControlItem";
		this.emptyRowCountValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(90, 18);
		this.emptyRowCountValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptyRowCountValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.emptyRowCountValueLabelControlLayoutControlItem.TextVisible = false;
		this.emptyRowPercentCountLabelControlLayoutControlItem.Control = this.emptyRowPercentCountLabelControl;
		this.emptyRowPercentCountLabelControlLayoutControlItem.Location = new System.Drawing.Point(345, 56);
		this.emptyRowPercentCountLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(60, 18);
		this.emptyRowPercentCountLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(60, 18);
		this.emptyRowPercentCountLabelControlLayoutControlItem.Name = "emptyRowPercentCountLabelControlLayoutControlItem";
		this.emptyRowPercentCountLabelControlLayoutControlItem.Size = new System.Drawing.Size(60, 18);
		this.emptyRowPercentCountLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptyRowPercentCountLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.emptyRowPercentCountLabelControlLayoutControlItem.TextVisible = false;
		this.layoutControlItem18.Control = this.emptyRowsProgressBarControl;
		this.layoutControlItem18.Location = new System.Drawing.Point(405, 56);
		this.layoutControlItem18.MaxSize = new System.Drawing.Size(54, 18);
		this.layoutControlItem18.MinSize = new System.Drawing.Size(54, 18);
		this.layoutControlItem18.Name = "layoutControlItem18";
		this.layoutControlItem18.Size = new System.Drawing.Size(54, 18);
		this.layoutControlItem18.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem18.TextVisible = false;
		this.nullRowsTextLabelControlLayoutControlItem.Control = this.nullRowsLabelControl;
		this.nullRowsTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(35, 74);
		this.nullRowsTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(220, 18);
		this.nullRowsTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(220, 18);
		this.nullRowsTextLabelControlLayoutControlItem.Name = "nullRowsTextLabelControlLayoutControlItem";
		this.nullRowsTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(220, 20);
		this.nullRowsTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.nullRowsTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.nullRowsTextLabelControlLayoutControlItem.TextVisible = false;
		this.nullRowsValueLabelControlLayoutControlItem.Control = this.nullRowsValueLabelControl;
		this.nullRowsValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(255, 74);
		this.nullRowsValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(90, 18);
		this.nullRowsValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(90, 18);
		this.nullRowsValueLabelControlLayoutControlItem.Name = "nullRowsValueLabelControlLayoutControlItem";
		this.nullRowsValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(90, 20);
		this.nullRowsValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.nullRowsValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.nullRowsValueLabelControlLayoutControlItem.TextVisible = false;
		this.nullRowsProgressControlLayoutControlItem.Control = this.nullRowsProgressBarControl;
		this.nullRowsProgressControlLayoutControlItem.Location = new System.Drawing.Point(405, 74);
		this.nullRowsProgressControlLayoutControlItem.MaxSize = new System.Drawing.Size(51, 18);
		this.nullRowsProgressControlLayoutControlItem.MinSize = new System.Drawing.Size(51, 18);
		this.nullRowsProgressControlLayoutControlItem.Name = "nullRowsProgressControlLayoutControlItem";
		this.nullRowsProgressControlLayoutControlItem.Size = new System.Drawing.Size(54, 20);
		this.nullRowsProgressControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.nullRowsProgressControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.nullRowsProgressControlLayoutControlItem.TextVisible = false;
		this.rowCountValueLabelControlLayoutControlItem.Control = this.rowCountValueLabelControl;
		this.rowCountValueLabelControlLayoutControlItem.Location = new System.Drawing.Point(255, 94);
		this.rowCountValueLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(90, 18);
		this.rowCountValueLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(90, 18);
		this.rowCountValueLabelControlLayoutControlItem.Name = "rowCountValueLabelControlLayoutControlItem";
		this.rowCountValueLabelControlLayoutControlItem.Size = new System.Drawing.Size(204, 18);
		this.rowCountValueLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.rowCountValueLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.rowCountValueLabelControlLayoutControlItem.TextVisible = false;
		this.valuesDistinctRowTextLabelControlLayoutControlItem.Control = this.distinctRowTextLabelControl;
		this.valuesDistinctRowTextLabelControlLayoutControlItem.Location = new System.Drawing.Point(35, 20);
		this.valuesDistinctRowTextLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(220, 18);
		this.valuesDistinctRowTextLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(220, 18);
		this.valuesDistinctRowTextLabelControlLayoutControlItem.Name = "valuesDistinctRowTextLabelControlLayoutControlItem";
		this.valuesDistinctRowTextLabelControlLayoutControlItem.Size = new System.Drawing.Size(220, 18);
		this.valuesDistinctRowTextLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.valuesDistinctRowTextLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.valuesDistinctRowTextLabelControlLayoutControlItem.TextVisible = false;
		this.valuesDistinctRowValuesLabelControlLayoutControlItem.Control = this.distinctRowValueLabelControl;
		this.valuesDistinctRowValuesLabelControlLayoutControlItem.Location = new System.Drawing.Point(255, 20);
		this.valuesDistinctRowValuesLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(90, 18);
		this.valuesDistinctRowValuesLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(90, 18);
		this.valuesDistinctRowValuesLabelControlLayoutControlItem.Name = "valuesDistinctRowValuesLabelControlLayoutControlItem";
		this.valuesDistinctRowValuesLabelControlLayoutControlItem.Size = new System.Drawing.Size(90, 18);
		this.valuesDistinctRowValuesLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.valuesDistinctRowValuesLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.valuesDistinctRowValuesLabelControlLayoutControlItem.TextVisible = false;
		this.valuesDistinctRowPercentLabelControlLayoutControlItem.Control = this.distinctRowPercentLabelControl;
		this.valuesDistinctRowPercentLabelControlLayoutControlItem.Location = new System.Drawing.Point(345, 20);
		this.valuesDistinctRowPercentLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(60, 18);
		this.valuesDistinctRowPercentLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(60, 18);
		this.valuesDistinctRowPercentLabelControlLayoutControlItem.Name = "valuesDistinctRowPercentLabelControlLayoutControlItem";
		this.valuesDistinctRowPercentLabelControlLayoutControlItem.Size = new System.Drawing.Size(60, 18);
		this.valuesDistinctRowPercentLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.valuesDistinctRowPercentLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.valuesDistinctRowPercentLabelControlLayoutControlItem.TextVisible = false;
		this.valuesDistinctRowProgressBarControlLayoutControlItem.Control = this.distinctRowProgressBarControl;
		this.valuesDistinctRowProgressBarControlLayoutControlItem.Location = new System.Drawing.Point(405, 20);
		this.valuesDistinctRowProgressBarControlLayoutControlItem.MaxSize = new System.Drawing.Size(54, 18);
		this.valuesDistinctRowProgressBarControlLayoutControlItem.MinSize = new System.Drawing.Size(54, 18);
		this.valuesDistinctRowProgressBarControlLayoutControlItem.Name = "aluesDistinctRowProgressBarControlLayoutControlItem";
		this.valuesDistinctRowProgressBarControlLayoutControlItem.Size = new System.Drawing.Size(54, 18);
		this.valuesDistinctRowProgressBarControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.valuesDistinctRowProgressBarControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.valuesDistinctRowProgressBarControlLayoutControlItem.TextVisible = false;
		this.distinctRowsPictureEditlayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.distinctRowsPictureEditlayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.distinctRowsPictureEditlayoutControlItem.Control = this.distinctRowsPictureEdit;
		this.distinctRowsPictureEditlayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.distinctRowsPictureEditlayoutControlItem.CustomizationFormText = "layoutControlItem1";
		this.distinctRowsPictureEditlayoutControlItem.Location = new System.Drawing.Point(0, 20);
		this.distinctRowsPictureEditlayoutControlItem.MaxSize = new System.Drawing.Size(35, 18);
		this.distinctRowsPictureEditlayoutControlItem.MinSize = new System.Drawing.Size(35, 18);
		this.distinctRowsPictureEditlayoutControlItem.Name = "distinctRowsPictureEditlayoutControlItem";
		this.distinctRowsPictureEditlayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(23, 0, 6, 4);
		this.distinctRowsPictureEditlayoutControlItem.Size = new System.Drawing.Size(35, 18);
		this.distinctRowsPictureEditlayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.distinctRowsPictureEditlayoutControlItem.Text = "layoutControlItem1";
		this.distinctRowsPictureEditlayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.distinctRowsPictureEditlayoutControlItem.TextVisible = false;
		this.nonDistinctRowsPictureEditLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.nonDistinctRowsPictureEditLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.nonDistinctRowsPictureEditLayoutControlItem.Control = this.nonDistinctRowsPictureEdit1;
		this.nonDistinctRowsPictureEditLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.nonDistinctRowsPictureEditLayoutControlItem.CustomizationFormText = "layoutControlItem1";
		this.nonDistinctRowsPictureEditLayoutControlItem.Location = new System.Drawing.Point(0, 38);
		this.nonDistinctRowsPictureEditLayoutControlItem.MaxSize = new System.Drawing.Size(35, 18);
		this.nonDistinctRowsPictureEditLayoutControlItem.MinSize = new System.Drawing.Size(35, 18);
		this.nonDistinctRowsPictureEditLayoutControlItem.Name = "nonDistinctRowsPictureEditLayoutControlItem";
		this.nonDistinctRowsPictureEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(23, 0, 6, 4);
		this.nonDistinctRowsPictureEditLayoutControlItem.Size = new System.Drawing.Size(35, 18);
		this.nonDistinctRowsPictureEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.nonDistinctRowsPictureEditLayoutControlItem.Text = "layoutControlItem1";
		this.nonDistinctRowsPictureEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.nonDistinctRowsPictureEditLayoutControlItem.TextVisible = false;
		this.emptyRowsPictureEditLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.emptyRowsPictureEditLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.emptyRowsPictureEditLayoutControlItem.Control = this.emptyRowsPictureEdit;
		this.emptyRowsPictureEditLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.emptyRowsPictureEditLayoutControlItem.CustomizationFormText = "layoutControlItem1";
		this.emptyRowsPictureEditLayoutControlItem.Location = new System.Drawing.Point(0, 56);
		this.emptyRowsPictureEditLayoutControlItem.MaxSize = new System.Drawing.Size(35, 18);
		this.emptyRowsPictureEditLayoutControlItem.MinSize = new System.Drawing.Size(35, 18);
		this.emptyRowsPictureEditLayoutControlItem.Name = "emptyRowsPictureEditLayoutControlItem";
		this.emptyRowsPictureEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(23, 0, 6, 4);
		this.emptyRowsPictureEditLayoutControlItem.Size = new System.Drawing.Size(35, 18);
		this.emptyRowsPictureEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptyRowsPictureEditLayoutControlItem.Text = "layoutControlItem1";
		this.emptyRowsPictureEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.emptyRowsPictureEditLayoutControlItem.TextVisible = false;
		this.nullRowsPictureEditLayoutControlItem.ContentHorzAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.nullRowsPictureEditLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Center;
		this.nullRowsPictureEditLayoutControlItem.Control = this.nullRowsPictureEdit;
		this.nullRowsPictureEditLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.nullRowsPictureEditLayoutControlItem.CustomizationFormText = "nullRowsPictureEditLayoutControlItem";
		this.nullRowsPictureEditLayoutControlItem.Location = new System.Drawing.Point(0, 74);
		this.nullRowsPictureEditLayoutControlItem.MaxSize = new System.Drawing.Size(35, 20);
		this.nullRowsPictureEditLayoutControlItem.MinSize = new System.Drawing.Size(35, 20);
		this.nullRowsPictureEditLayoutControlItem.Name = "nullRowsPictureEditLayoutControlItem";
		this.nullRowsPictureEditLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(23, 0, 6, 4);
		this.nullRowsPictureEditLayoutControlItem.Size = new System.Drawing.Size(35, 20);
		this.nullRowsPictureEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.nullRowsPictureEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.nullRowsPictureEditLayoutControlItem.TextVisible = false;
		this.rowsTitleLabelControlLayoutControlItem.Control = this.rowsTitleLabelControl;
		this.rowsTitleLabelControlLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.rowsTitleLabelControlLayoutControlItem.CustomizationFormText = "rowslabelControlLayoutControlItem";
		this.rowsTitleLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.rowsTitleLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(55, 20);
		this.rowsTitleLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(55, 20);
		this.rowsTitleLabelControlLayoutControlItem.Name = "rowsTitleLabelControlLayoutControlItem";
		this.rowsTitleLabelControlLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.rowsTitleLabelControlLayoutControlItem.Size = new System.Drawing.Size(459, 20);
		this.rowsTitleLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.rowsTitleLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.rowsTitleLabelControlLayoutControlItem.TextVisible = false;
		this.rowCountLabelControlLayoutControlItem.Control = this.rowCountTextLabelControl;
		this.rowCountLabelControlLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.rowCountLabelControlLayoutControlItem.CustomizationFormText = "rowCountlabelControlLayoutControlItem";
		this.rowCountLabelControlLayoutControlItem.Location = new System.Drawing.Point(0, 94);
		this.rowCountLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(255, 18);
		this.rowCountLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(255, 18);
		this.rowCountLabelControlLayoutControlItem.Name = "rowCountLabelControlLayoutControlItem";
		this.rowCountLabelControlLayoutControlItem.Size = new System.Drawing.Size(255, 18);
		this.rowCountLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.rowCountLabelControlLayoutControlItem.Text = "rowCountlabelControlLayoutControlItem";
		this.rowCountLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.rowCountLabelControlLayoutControlItem.TextVisible = false;
		this.nullRowsPercentsLabelControlLayoutControlItem.Control = this.nullRowsPercentLabelControl;
		this.nullRowsPercentsLabelControlLayoutControlItem.Location = new System.Drawing.Point(345, 74);
		this.nullRowsPercentsLabelControlLayoutControlItem.MaxSize = new System.Drawing.Size(60, 18);
		this.nullRowsPercentsLabelControlLayoutControlItem.MinSize = new System.Drawing.Size(60, 18);
		this.nullRowsPercentsLabelControlLayoutControlItem.Name = "nullRowsPercentsLabelControlLayoutControlItem";
		this.nullRowsPercentsLabelControlLayoutControlItem.Size = new System.Drawing.Size(60, 20);
		this.nullRowsPercentsLabelControlLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.nullRowsPercentsLabelControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.nullRowsPercentsLabelControlLayoutControlItem.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.AutoSize = true;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.rowStatsUserControlLayoutControl);
		base.Name = "RowStatsUserControlcs";
		base.Size = new System.Drawing.Size(468, 121);
		((System.ComponentModel.ISupportInitialize)this.rowStatsUserControlLayoutControl).EndInit();
		this.rowStatsUserControlLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.distinctRowProgressBarControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nullRowsProgressBarControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptyRowsProgressBarControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nonDistinctRowProgressBarControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.distinctRowsPictureEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nonDistinctRowsPictureEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptyRowsPictureEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nullRowsPictureEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valuesNonDistinctTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nonDistinctValuesLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nonDistinctRowPercentLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptyRowsTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptyRowCountValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptyRowPercentCountLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nullRowsTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nullRowsValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nullRowsProgressControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowCountValueLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valuesDistinctRowTextLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valuesDistinctRowValuesLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valuesDistinctRowPercentLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valuesDistinctRowProgressBarControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.distinctRowsPictureEditlayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nonDistinctRowsPictureEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptyRowsPictureEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nullRowsPictureEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowsTitleLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rowCountLabelControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nullRowsPercentsLabelControlLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
