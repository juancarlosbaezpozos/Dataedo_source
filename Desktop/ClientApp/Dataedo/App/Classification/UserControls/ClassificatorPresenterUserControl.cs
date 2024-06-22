using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classification.UserControls.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Helpers.Controls;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Synchronize.Classes;
using Dataedo.Model.Data.Classificator;
using Dataedo.Model.Data.CustomFields;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.BandedGrid.ViewInfo;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;

namespace Dataedo.App.Classification.UserControls;

public class ClassificatorPresenterUserControl : BaseUserControl
{
	private ClassificatorModel choosenClassificator;

	private List<ClassificationStats> classificatorStats;

	private List<CustomFieldClassRow> customFieldsClasses;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LabelControl classificatorTitleLabelControl;

	private LayoutControlGroup Root;

	private LayoutControlItem classificatorTitleLayoutControlItem;

	private GridControl gridControl;

	private BandedGridView bandedGridView;

	private LayoutControlItem gridControlLayoutControlItem;

	private GridBand classificatorCustomFieldValuesBand;

	private BandedGridColumn customFieldValueGridColumn;

	private GridBand classifiedFieldsBand;

	private BandedGridColumn classifiedFieldsNumberColumn;

	private BandedGridColumn classifiedFieldsProgressColumn;

	private RepositoryItemProgressBar repositoryItemProgressBar;

	private EmptySpaceItem emptySpaceItem1;

	private ToolTipController toolTipController;

	public ClassificatorPresenterUserControl()
	{
		InitializeComponent();
		repositoryItemProgressBar.LookAndFeel.UseDefaultLookAndFeel = false;
		repositoryItemProgressBar.LookAndFeel.Style = LookAndFeelStyle.Flat;
		bandedGridView.CustomDrawCell += BandedGridView_CustomDrawCell;
	}

	private void BandedGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (e.Column == classifiedFieldsProgressColumn)
		{
			GridViewHelpers.DrawBackgroundForProgressBar(e, Color.FromArgb(224, 234, 248));
		}
	}

	public void SetParameters(ClassificatorModel choosenClassificator, List<ClassificationStats> classificatorStats)
	{
		if (customFieldsClasses == null)
		{
			customFieldsClasses = (from x in DB.CustomField.GetCustomFieldClasses()
				where new string[2] { "DOMAIN", "CLASSIFICATION" }.Contains(x.Code)
				select new CustomFieldClassRow(x)).ToList();
		}
		this.choosenClassificator = choosenClassificator;
		this.classificatorStats = classificatorStats;
		classificatorTitleLabelControl.Text = this.choosenClassificator.Title;
		ClassificatorCustomField classificatorCustomField = this.choosenClassificator.Fields.FirstOrDefault((ClassificatorCustomField x) => x.CustomFieldClassId == customFieldsClasses.Where((CustomFieldClassRow y) => y.Code == "CLASSIFICATION").FirstOrDefault()?.CustomFieldClassId && !string.IsNullOrWhiteSpace(x.Definition));
		repositoryItemProgressBar.Maximum = this.classificatorStats.Where((ClassificationStats x) => !string.IsNullOrEmpty(x.CustomFieldValue)).Sum((ClassificationStats x) => x.CustomFieldValueCounter);
		if (repositoryItemProgressBar.Maximum == 0)
		{
			repositoryItemProgressBar.Maximum = 1;
		}
		if (classificatorCustomField != null)
		{
			classificatorCustomFieldValuesBand.Caption = classificatorCustomField?.Title + ":";
			IEnumerable<ClassificatorPresenterRow> dataSource = classificatorCustomField.DefinitionValues.Select((string x) => new ClassificatorPresenterRow(x, this.classificatorStats.Where((ClassificationStats y) => y.CustomFieldValue == x).Sum((ClassificationStats y) => y.CustomFieldValueCounter)));
			gridControl.DataSource = dataSource;
		}
		else
		{
			classificatorCustomFieldValuesBand.Caption = string.Empty;
			gridControl.DataSource = null;
		}
		GridViewHelpers.SetColumnBestWidth(classifiedFieldsNumberColumn);
	}

	private void BandedGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageBandedGridViewPopup(e);
		_ = e.Menu;
	}

	private void GridControl_PaintEx(object sender, PaintExEventArgs e)
	{
		BandedGridViewHelpers.DrawVerticalLinesBetweenBands(bandedGridView, e);
	}

	private void ToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		if (!(e?.SelectedControl is GridControl gridControl) || !(gridControl.MainView is BandedGridView bandedGridView))
		{
			return;
		}
		BandedGridHitInfo bandedGridHitInfo = bandedGridView.CalcHitInfo(e.ControlMousePosition);
		if (bandedGridHitInfo.InRowCell && bandedGridHitInfo.Column == classifiedFieldsProgressColumn && bandedGridView.GetRow(bandedGridHitInfo.RowHandle) is ClassificatorPresenterRow classificatorPresenterRow)
		{
			int num = classificatorStats.Where((ClassificationStats x) => !string.IsNullOrEmpty(x.CustomFieldValue)).Sum((ClassificationStats x) => x.CustomFieldValueCounter);
			ToolTipControlInfo toolTipControlInfo = new ToolTipControlInfo
			{
				Object = classificatorPresenterRow,
				SuperTip = new SuperToolTip()
			};
			toolTipControlInfo.SuperTip.Items.Add($"{classificatorPresenterRow.ClassifiedFieldNumber} out of {num}" + " classified columns are marked as " + classificatorPresenterRow.CustomFieldValue);
			e.Info = toolTipControlInfo;
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
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.gridControl = new DevExpress.XtraGrid.GridControl();
		this.bandedGridView = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridView();
		this.classificatorCustomFieldValuesBand = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.customFieldValueGridColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.classifiedFieldsBand = new DevExpress.XtraGrid.Views.BandedGrid.GridBand();
		this.classifiedFieldsNumberColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.classifiedFieldsProgressColumn = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.repositoryItemProgressBar = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.classificatorTitleLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.classificatorTitleLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.gridControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bandedGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemProgressBar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.classificatorTitleLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.gridControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.classificatorTitleLabelControl);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(916, 11, 650, 400);
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(355, 303);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl";
		this.gridControl.Location = new System.Drawing.Point(12, 29);
		this.gridControl.MainView = this.bandedGridView;
		this.gridControl.Name = "gridControl";
		this.gridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemProgressBar });
		this.gridControl.Size = new System.Drawing.Size(331, 262);
		this.gridControl.TabIndex = 5;
		this.gridControl.ToolTipController = this.toolTipController;
		this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.bandedGridView });
		this.gridControl.PaintEx += new DevExpress.XtraGrid.PaintExEventHandler(GridControl_PaintEx);
		this.bandedGridView.Appearance.FooterPanel.Options.UseFont = true;
		this.bandedGridView.Bands.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.GridBand[2] { this.classificatorCustomFieldValuesBand, this.classifiedFieldsBand });
		this.bandedGridView.Columns.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn[3] { this.customFieldValueGridColumn, this.classifiedFieldsNumberColumn, this.classifiedFieldsProgressColumn });
		this.bandedGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
		this.bandedGridView.GridControl = this.gridControl;
		this.bandedGridView.Name = "bandedGridView";
		this.bandedGridView.OptionsBehavior.Editable = false;
		this.bandedGridView.OptionsCustomization.AllowBandMoving = false;
		this.bandedGridView.OptionsCustomization.AllowBandResizing = false;
		this.bandedGridView.OptionsCustomization.AllowColumnMoving = false;
		this.bandedGridView.OptionsCustomization.AllowFilter = false;
		this.bandedGridView.OptionsCustomization.AllowGroup = false;
		this.bandedGridView.OptionsCustomization.AllowQuickHideColumns = false;
		this.bandedGridView.OptionsFind.AllowFindPanel = false;
		this.bandedGridView.OptionsMenu.EnableFooterMenu = false;
		this.bandedGridView.OptionsMenu.EnableGroupPanelMenu = false;
		this.bandedGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.bandedGridView.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.VisibleAlways;
		this.bandedGridView.OptionsView.ShowColumnHeaders = false;
		this.bandedGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.bandedGridView.OptionsView.ShowFooter = true;
		this.bandedGridView.OptionsView.ShowGroupPanel = false;
		this.bandedGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.bandedGridView.OptionsView.ShowIndicator = false;
		this.bandedGridView.OptionsView.ShowPreviewRowLines = DevExpress.Utils.DefaultBoolean.False;
		this.bandedGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.bandedGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(BandedGridView_PopupMenuShowing);
		this.classificatorCustomFieldValuesBand.Caption = "classificatorCustomFieldValuesBand";
		this.classificatorCustomFieldValuesBand.Columns.Add(this.customFieldValueGridColumn);
		this.classificatorCustomFieldValuesBand.Name = "classificatorCustomFieldValuesBand";
		this.classificatorCustomFieldValuesBand.VisibleIndex = 0;
		this.classificatorCustomFieldValuesBand.Width = 381;
		this.customFieldValueGridColumn.Caption = "customFieldValueGridColumn";
		this.customFieldValueGridColumn.FieldName = "CustomFieldValue";
		this.customFieldValueGridColumn.Name = "customFieldValueGridColumn";
		this.customFieldValueGridColumn.Visible = true;
		this.customFieldValueGridColumn.Width = 381;
		this.classifiedFieldsBand.AppearanceHeader.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.classifiedFieldsBand.AppearanceHeader.Options.UseFont = true;
		this.classifiedFieldsBand.AppearanceHeader.Options.UseTextOptions = true;
		this.classifiedFieldsBand.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.classifiedFieldsBand.Caption = "Classified fields";
		this.classifiedFieldsBand.Columns.Add(this.classifiedFieldsNumberColumn);
		this.classifiedFieldsBand.Columns.Add(this.classifiedFieldsProgressColumn);
		this.classifiedFieldsBand.Name = "classifiedFieldsBand";
		this.classifiedFieldsBand.OptionsBand.FixedWidth = true;
		this.classifiedFieldsBand.VisibleIndex = 1;
		this.classifiedFieldsBand.Width = 135;
		this.classifiedFieldsNumberColumn.Caption = "classifiedFieldsNumberGridColumn";
		this.classifiedFieldsNumberColumn.DisplayFormat.FormatString = "{0:N0}";
		this.classifiedFieldsNumberColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.classifiedFieldsNumberColumn.FieldName = "ClassifiedFieldNumber";
		this.classifiedFieldsNumberColumn.Name = "classifiedFieldsNumberColumn";
		this.classifiedFieldsNumberColumn.OptionsColumn.AllowEdit = false;
		this.classifiedFieldsNumberColumn.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "ClassifiedFieldNumber", "{0:N0}")
		});
		this.classifiedFieldsNumberColumn.Visible = true;
		this.classifiedFieldsNumberColumn.Width = 65;
		this.classifiedFieldsProgressColumn.Caption = "classifiedFieldsProgressGridColumn";
		this.classifiedFieldsProgressColumn.ColumnEdit = this.repositoryItemProgressBar;
		this.classifiedFieldsProgressColumn.FieldName = "ClassifiedFieldNumber";
		this.classifiedFieldsProgressColumn.MinWidth = 70;
		this.classifiedFieldsProgressColumn.Name = "classifiedFieldsProgressColumn";
		this.classifiedFieldsProgressColumn.OptionsColumn.FixedWidth = true;
		this.classifiedFieldsProgressColumn.Visible = true;
		this.classifiedFieldsProgressColumn.Width = 70;
		this.repositoryItemProgressBar.EndColor = System.Drawing.Color.FromArgb(77, 130, 184);
		this.repositoryItemProgressBar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.repositoryItemProgressBar.LookAndFeel.UseDefaultLookAndFeel = false;
		this.repositoryItemProgressBar.Name = "repositoryItemProgressBar";
		this.repositoryItemProgressBar.ProgressPadding = new System.Windows.Forms.Padding(1, 5, 4, 5);
		this.repositoryItemProgressBar.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.repositoryItemProgressBar.StartColor = System.Drawing.Color.FromArgb(77, 130, 184);
		this.classificatorTitleLabelControl.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Bold);
		this.classificatorTitleLabelControl.Appearance.Options.UseFont = true;
		this.classificatorTitleLabelControl.AutoEllipsis = true;
		this.classificatorTitleLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.classificatorTitleLabelControl.Location = new System.Drawing.Point(12, 12);
		this.classificatorTitleLabelControl.Name = "classificatorTitleLabelControl";
		this.classificatorTitleLabelControl.Size = new System.Drawing.Size(306, 13);
		this.classificatorTitleLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.classificatorTitleLabelControl.TabIndex = 4;
		this.classificatorTitleLabelControl.Text = "Very very very long name of very important classificator";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.classificatorTitleLayoutControlItem, this.gridControlLayoutControlItem, this.emptySpaceItem1 });
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(355, 303);
		this.Root.TextVisible = false;
		this.classificatorTitleLayoutControlItem.AppearanceItemCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.classificatorTitleLayoutControlItem.AppearanceItemCaption.Options.UseFont = true;
		this.classificatorTitleLayoutControlItem.Control = this.classificatorTitleLabelControl;
		this.classificatorTitleLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.classificatorTitleLayoutControlItem.MaxSize = new System.Drawing.Size(310, 17);
		this.classificatorTitleLayoutControlItem.MinSize = new System.Drawing.Size(310, 17);
		this.classificatorTitleLayoutControlItem.Name = "classificatorTitleLayoutControlItem";
		this.classificatorTitleLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.classificatorTitleLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.classificatorTitleLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.classificatorTitleLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.classificatorTitleLayoutControlItem.Size = new System.Drawing.Size(310, 17);
		this.classificatorTitleLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.classificatorTitleLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.classificatorTitleLayoutControlItem.TextVisible = false;
		this.gridControlLayoutControlItem.Control = this.gridControl;
		this.gridControlLayoutControlItem.Location = new System.Drawing.Point(0, 17);
		this.gridControlLayoutControlItem.Name = "gridControlLayoutControlItem";
		this.gridControlLayoutControlItem.Size = new System.Drawing.Size(335, 266);
		this.gridControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.gridControlLayoutControlItem.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(310, 0);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(25, 17);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(ToolTipController_GetActiveObjectInfo);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.Name = "ClassificatorPresenterUserControl";
		base.Size = new System.Drawing.Size(355, 303);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bandedGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemProgressBar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.classificatorTitleLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		base.ResumeLayout(false);
	}
}
