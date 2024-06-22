using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.ImportDescriptions.Processing.DataToImport;
using Dataedo.App.ImportDescriptions.Tools;
using Dataedo.App.ImportDescriptions.Tools.Fields;
using Dataedo.App.Properties;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.ImportDescriptions.UserControls;

public class DataToImportUserControl : BaseUserControl
{
	private List<FieldDefinition> fieldDefinitions;

	private IImportProcessorBase importProcessor;

	private CustomFieldsCellsTypesSupport customFieldsCellsTypesSupportForGrids;

	private bool isDataValid;

	private IContainer components;

	private GridControl dataGridControl;

	private CustomGridUserControl dataGridView;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private LayoutControlItem layoutControlItem1;

	private NonCustomizableLayoutControl errorTextLayoutControl;

	private LabelControl errorTextLabelControl;

	private PictureBox errorIconPictureBox;

	private LayoutControlGroup errorTextLayoutControlGroup;

	private LayoutControlItem errorIconLayoutControlItem;

	private LayoutControlItem errorTextLayoutControlItem;

	private LayoutControlItem errorInformationLayoutControlItem;

	private NonCustomizableLayoutControl warningTextLayoutControl;

	private LabelControl warningTextLabelControl;

	private PictureBox warningIconPictureBox;

	private LayoutControlGroup warningTextLayoutControlGroup;

	private LayoutControlItem warningIconLayoutControlItem;

	private LayoutControlItem warningTextLayoutControlItem;

	private LayoutControlItem warningInformationLayoutControlItem;

	private ToolTipController toolTipController;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private NonCustomizableLayoutControl valuesTextLayoutControl1;

	private LabelControl valuesTextLabelControl1;

	private PictureBox valuesIconPictureBox1;

	private LayoutControlGroup valuesTextLayoutControlGroup1;

	private LayoutControlItem valuesIconLayoutControlItem1;

	private LayoutControlItem valuesTextLayoutControlItem1;

	private LayoutControlItem valuesInformationlayoutControlItem1;

	public bool IsDataValid
	{
		get
		{
			return isDataValid;
		}
		set
		{
			if (value != isDataValid)
			{
				isDataValid = value;
				this.ValidityChangedEvent?.Invoke(this, new BoolEventArgs(value));
			}
		}
	}

	public List<ImportDataModel> ModelsGeneral => importProcessor.ModelsGeneral;

	public bool HasData => importProcessor.ModelsGeneral.Any();

	public bool IsChanged { get; set; }

	[Browsable(true)]
	public event EventHandler<BoolEventArgs> ValidityChangedEvent;

	public DataToImportUserControl()
	{
		InitializeComponent();
		errorInformationLayoutControlItem.Visibility = LayoutVisibility.Never;
		warningInformationLayoutControlItem.Visibility = LayoutVisibility.Never;
		valuesInformationlayoutControlItem1.Visibility = LayoutVisibility.Never;
	}

	public void Initialize(int databaseId, SharedObjectTypeEnum.ObjectType objectType, List<FieldDefinition> fieldDefinitions)
	{
		this.fieldDefinitions = fieldDefinitions;
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Table:
			importProcessor = new ImportTablesProcessor(databaseId, this.fieldDefinitions, dataGridView);
			break;
		case SharedObjectTypeEnum.ObjectType.Column:
			importProcessor = new ImportColumnsProcessor(databaseId, this.fieldDefinitions, dataGridView);
			break;
		default:
			throw new ArgumentException($"Provided type ({objectType}) is not valid argument.");
		}
		importProcessor.SetLabels(errorTextLabelControl, errorInformationLayoutControlItem, warningTextLabelControl, warningInformationLayoutControlItem, valuesTextLabelControl1, valuesInformationlayoutControlItem1);
		importProcessor?.PrepareColumns();
		dataGridControl.BeginUpdate();
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Table:
			dataGridControl.DataSource = new BindingList<TableImportDataModel>((importProcessor as ImportTablesProcessor).Models);
			break;
		case SharedObjectTypeEnum.ObjectType.Column:
			dataGridControl.DataSource = new BindingList<ColumnImportDataModel>((importProcessor as ImportColumnsProcessor).Models);
			break;
		}
		dataGridControl.EndUpdate();
		customFieldsCellsTypesSupportForGrids = new CustomFieldsCellsTypesSupport(isForSummaryTable: false);
		SetCustomFieldsDataSource();
		dataGridView.BestFitColumns();
		IsDataValid = false;
	}

	public void SetData(string data, BackgroundWorker backgroundWorker, DoWorkEventArgs e)
	{
		dataGridControl.BeginUpdate();
		importProcessor.AddRows(data, backgroundWorker, e);
	}

	public void ClearExisting()
	{
		importProcessor.RemoveAllRows();
	}

	public void FinishSettingData()
	{
		dataGridControl.EndUpdate();
	}

	public void AfterAddingRows()
	{
		importProcessor.CheckRows();
		IsChanged = true;
		dataGridControl.BeginUpdate();
		dataGridView.OptionsView.BestFitMaxRowCount = 100;
		dataGridView.BestFitColumns();
		dataGridView.RefreshData();
		dataGridControl.EndUpdate();
		SetIfDataValid();
	}

	public void RemoveSelectedRow()
	{
		importProcessor.RemoveSelectedRows();
		SetIfDataValid();
	}

	public void RemoveAllRows()
	{
		importProcessor.RemoveAllRows();
		SetIfDataValid();
	}

	public string GetTemplate()
	{
		IEnumerable<string> values = dataGridView.Columns.Select((GridColumn x) => x.Caption);
		return string.Join("\t", values).Trim() + "\t";
	}

	private void dataGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (e.RowHandle == -2147483646 || !(dataGridView.GetRow(e.RowHandle) is ImportDataModel importDataModel))
		{
			return;
		}
		if (e.Column != null && e.Column.Tag != null)
		{
			CustomFieldRowExtended customFieldRowExtended = e.Column.Tag as CustomFieldRowExtended;
			if (customFieldRowExtended.IsDomainValueType && !customFieldRowExtended.IsValueProperForDomainValuesType(e.CellValue?.ToString()))
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.RedBackColor;
			}
		}
		if (importDataModel.IsDuplicated)
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.DoubledRowBackColor;
		}
	}

	private void SetCustomFieldsDataSource()
	{
		customFieldsCellsTypesSupportForGrids.SetCustomColumnsForExistingColumns(dataGridView);
	}

	private void SetIfDataValid()
	{
		IsDataValid = importProcessor.CheckIfDataValid();
	}

	private void dataGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e, withLockUnlockOption: false);
		e.Menu?.Hide(GridStringId.MenuColumnColumnCustomization);
		e.Menu?.Hide(GridStringId.MenuColumnRemoveColumn);
	}

	private void dataGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (sender is GridView gridView)
		{
			object row = gridView.GetRow(e.RowHandle);
			if (row != null)
			{
				importProcessor.CheckValueChanged(row, e.Column.FieldName);
				SetIfDataValid();
			}
		}
	}

	private void toolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		GridHitInfo gridHitInfo = dataGridView.CalcHitInfo(e.ControlMousePosition);
		if (gridHitInfo.RowHandle < 0)
		{
			return;
		}
		ImportDataModel importDataModel = dataGridView.GetRow(gridHitInfo.RowHandle) as ImportDataModel;
		SuperToolTipSetupArgs superToolTipSetupArgs = new SuperToolTipSetupArgs();
		if (gridHitInfo.Column.FieldName.Equals("Image"))
		{
			superToolTipSetupArgs.Contents.Text = importProcessor.GetTooltipString(importDataModel, gridHitInfo.Column.FieldName);
			if (!string.IsNullOrEmpty(superToolTipSetupArgs.Contents.Text))
			{
				e.Info = new ToolTipControlInfo
				{
					Object = gridHitInfo.HitTest.ToString() + gridHitInfo.RowHandle,
					ToolTipType = ToolTipType.SuperTip,
					SuperTip = new SuperToolTip()
				};
				e.Info.SuperTip.Setup(superToolTipSetupArgs);
			}
		}
		else if (gridHitInfo.Column.FieldName.Equals("TableTypeImage"))
		{
			superToolTipSetupArgs.Contents.Text = importDataModel?.TableObjectType.ToString();
			if (!string.IsNullOrEmpty(superToolTipSetupArgs.Contents.Text))
			{
				e.Info = new ToolTipControlInfo
				{
					Object = gridHitInfo.HitTest.ToString() + gridHitInfo.RowHandle,
					ToolTipType = ToolTipType.SuperTip,
					SuperTip = new SuperToolTip()
				};
				e.Info.SuperTip.Setup(superToolTipSetupArgs);
			}
		}
	}

	public void ShowWarnings()
	{
		foreach (KeyValuePair<string, string> item in importProcessor.WarningsDuringImport)
		{
			GeneralMessageBoxesHandling.Show(item.Value, item.Key, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
		}
		importProcessor.WarningsDuringImport.Clear();
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
		this.dataGridControl = new DevExpress.XtraGrid.GridControl();
		this.dataGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.warningTextLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.warningTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.warningIconPictureBox = new System.Windows.Forms.PictureBox();
		this.warningTextLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.warningIconLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.warningTextLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.errorTextLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.errorTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.errorIconPictureBox = new System.Windows.Forms.PictureBox();
		this.errorTextLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.errorIconLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.errorTextLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.valuesTextLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.valuesTextLabelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.valuesIconPictureBox1 = new System.Windows.Forms.PictureBox();
		this.valuesTextLayoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.valuesIconLayoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.valuesTextLayoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.errorInformationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.warningInformationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.valuesInformationlayoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.dataGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.warningTextLayoutControl).BeginInit();
		this.warningTextLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.warningIconPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.warningTextLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.warningIconLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.warningTextLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.errorTextLayoutControl).BeginInit();
		this.errorTextLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.errorIconPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.errorTextLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.errorIconLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.errorTextLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valuesTextLayoutControl1).BeginInit();
		this.valuesTextLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.valuesIconPictureBox1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valuesTextLayoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valuesIconLayoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valuesTextLayoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.errorInformationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.warningInformationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.valuesInformationlayoutControlItem1).BeginInit();
		base.SuspendLayout();
		this.dataGridControl.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4);
		this.dataGridControl.Location = new System.Drawing.Point(2, 2);
		this.dataGridControl.MainView = this.dataGridView;
		this.dataGridControl.Margin = new System.Windows.Forms.Padding(4);
		this.dataGridControl.MenuManager = this.barManager;
		this.dataGridControl.Name = "dataGridControl";
		this.dataGridControl.ShowOnlyPredefinedDetails = true;
		this.dataGridControl.Size = new System.Drawing.Size(1216, 609);
		this.dataGridControl.TabIndex = 2;
		this.dataGridControl.ToolTipController = this.toolTipController;
		this.dataGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.dataGridView });
		this.dataGridView.DetailHeight = 431;
		this.dataGridView.GridControl = this.dataGridControl;
		this.dataGridView.Name = "dataGridView";
		this.dataGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.dataGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.dataGridView.OptionsFilter.AllowFilterEditor = false;
		this.dataGridView.OptionsSelection.MultiSelect = true;
		this.dataGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.dataGridView.OptionsView.ColumnAutoWidth = false;
		this.dataGridView.OptionsView.RowAutoHeight = true;
		this.dataGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.dataGridView.OptionsView.ShowGroupPanel = false;
		this.dataGridView.OptionsView.ShowIndicator = false;
		this.dataGridView.RowHighlightingIsEnabled = true;
		this.dataGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(dataGridView_CustomDrawCell);
		this.dataGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(dataGridView_PopupMenuShowing);
		this.dataGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(dataGridView_CellValueChanged);
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Margin = new System.Windows.Forms.Padding(4);
		this.barDockControlTop.Size = new System.Drawing.Size(1220, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 702);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(4);
		this.barDockControlBottom.Size = new System.Drawing.Size(1220, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(4);
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 702);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1220, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Margin = new System.Windows.Forms.Padding(4);
		this.barDockControlRight.Size = new System.Drawing.Size(0, 702);
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(toolTipController_GetActiveObjectInfo);
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.warningTextLayoutControl);
		this.mainLayoutControl.Controls.Add(this.errorTextLayoutControl);
		this.mainLayoutControl.Controls.Add(this.dataGridControl);
		this.mainLayoutControl.Controls.Add(this.valuesTextLayoutControl1);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.Margin = new System.Windows.Forms.Padding(4);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(1220, 702);
		this.mainLayoutControl.TabIndex = 3;
		this.mainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.warningTextLayoutControl.AllowCustomization = false;
		this.warningTextLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.warningTextLayoutControl.Controls.Add(this.warningTextLabelControl);
		this.warningTextLayoutControl.Controls.Add(this.warningIconPictureBox);
		this.warningTextLayoutControl.Location = new System.Drawing.Point(12, 675);
		this.warningTextLayoutControl.Margin = new System.Windows.Forms.Padding(4);
		this.warningTextLayoutControl.Name = "warningTextLayoutControl";
		this.warningTextLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(446, 815, 327, 427);
		this.warningTextLayoutControl.Root = this.warningTextLayoutControlGroup;
		this.warningTextLayoutControl.Size = new System.Drawing.Size(1205, 25);
		this.warningTextLayoutControl.TabIndex = 25;
		this.warningTextLayoutControl.Text = "layoutControl1";
		this.warningTextLabelControl.AutoEllipsis = true;
		this.warningTextLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.warningTextLabelControl.Location = new System.Drawing.Point(29, 2);
		this.warningTextLabelControl.Margin = new System.Windows.Forms.Padding(4);
		this.warningTextLabelControl.MaximumSize = new System.Drawing.Size(0, 20);
		this.warningTextLabelControl.MinimumSize = new System.Drawing.Size(0, 20);
		this.warningTextLabelControl.Name = "warningTextLabelControl";
		this.warningTextLabelControl.Size = new System.Drawing.Size(1174, 20);
		this.warningTextLabelControl.StyleController = this.warningTextLayoutControl;
		this.warningTextLabelControl.TabIndex = 23;
		this.warningTextLabelControl.Text = "Error text";
		this.warningIconPictureBox.Image = Dataedo.App.Properties.Resources.warning_16;
		this.warningIconPictureBox.Location = new System.Drawing.Point(2, 2);
		this.warningIconPictureBox.Margin = new System.Windows.Forms.Padding(0);
		this.warningIconPictureBox.MaximumSize = new System.Drawing.Size(21, 20);
		this.warningIconPictureBox.MinimumSize = new System.Drawing.Size(21, 20);
		this.warningIconPictureBox.Name = "warningIconPictureBox";
		this.warningIconPictureBox.Size = new System.Drawing.Size(21, 20);
		this.warningIconPictureBox.TabIndex = 22;
		this.warningIconPictureBox.TabStop = false;
		this.warningTextLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.warningTextLayoutControlGroup.GroupBordersVisible = false;
		this.warningTextLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.warningIconLayoutControlItem, this.warningTextLayoutControlItem });
		this.warningTextLayoutControlGroup.Name = "warningTextLayoutControlGroup";
		this.warningTextLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.warningTextLayoutControlGroup.Size = new System.Drawing.Size(1205, 25);
		this.warningTextLayoutControlGroup.TextVisible = false;
		this.warningIconLayoutControlItem.Control = this.warningIconPictureBox;
		this.warningIconLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.warningIconLayoutControlItem.MaxSize = new System.Drawing.Size(27, 25);
		this.warningIconLayoutControlItem.MinSize = new System.Drawing.Size(27, 25);
		this.warningIconLayoutControlItem.Name = "warningIconLayoutControlItem";
		this.warningIconLayoutControlItem.Size = new System.Drawing.Size(27, 25);
		this.warningIconLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.warningIconLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.warningIconLayoutControlItem.TextVisible = false;
		this.warningTextLayoutControlItem.Control = this.warningTextLabelControl;
		this.warningTextLayoutControlItem.Location = new System.Drawing.Point(27, 0);
		this.warningTextLayoutControlItem.MinSize = new System.Drawing.Size(1, 25);
		this.warningTextLayoutControlItem.Name = "warningTextLayoutControlItem";
		this.warningTextLayoutControlItem.Size = new System.Drawing.Size(1178, 25);
		this.warningTextLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.warningTextLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.warningTextLayoutControlItem.TextVisible = false;
		this.errorTextLayoutControl.AllowCustomization = false;
		this.errorTextLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.errorTextLayoutControl.Controls.Add(this.errorTextLabelControl);
		this.errorTextLayoutControl.Controls.Add(this.errorIconPictureBox);
		this.errorTextLayoutControl.Location = new System.Drawing.Point(12, 615);
		this.errorTextLayoutControl.Margin = new System.Windows.Forms.Padding(4);
		this.errorTextLayoutControl.Name = "errorTextLayoutControl";
		this.errorTextLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(446, 815, 327, 427);
		this.errorTextLayoutControl.Root = this.errorTextLayoutControlGroup;
		this.errorTextLayoutControl.Size = new System.Drawing.Size(1205, 27);
		this.errorTextLayoutControl.TabIndex = 24;
		this.errorTextLayoutControl.Text = "layoutControl1";
		this.errorTextLabelControl.AutoEllipsis = true;
		this.errorTextLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.errorTextLabelControl.Location = new System.Drawing.Point(29, 2);
		this.errorTextLabelControl.Margin = new System.Windows.Forms.Padding(4);
		this.errorTextLabelControl.MaximumSize = new System.Drawing.Size(0, 20);
		this.errorTextLabelControl.MinimumSize = new System.Drawing.Size(0, 20);
		this.errorTextLabelControl.Name = "errorTextLabelControl";
		this.errorTextLabelControl.Size = new System.Drawing.Size(1174, 20);
		this.errorTextLabelControl.StyleController = this.errorTextLayoutControl;
		this.errorTextLabelControl.TabIndex = 23;
		this.errorTextLabelControl.Text = "Error text";
		this.errorIconPictureBox.Image = Dataedo.App.Properties.Resources.error_16;
		this.errorIconPictureBox.Location = new System.Drawing.Point(2, 2);
		this.errorIconPictureBox.Margin = new System.Windows.Forms.Padding(0);
		this.errorIconPictureBox.MaximumSize = new System.Drawing.Size(21, 20);
		this.errorIconPictureBox.MinimumSize = new System.Drawing.Size(21, 20);
		this.errorIconPictureBox.Name = "errorIconPictureBox";
		this.errorIconPictureBox.Size = new System.Drawing.Size(21, 20);
		this.errorIconPictureBox.TabIndex = 22;
		this.errorIconPictureBox.TabStop = false;
		this.errorTextLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.errorTextLayoutControlGroup.GroupBordersVisible = false;
		this.errorTextLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.errorIconLayoutControlItem, this.errorTextLayoutControlItem });
		this.errorTextLayoutControlGroup.Name = "errorTextLayoutControlGroup";
		this.errorTextLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.errorTextLayoutControlGroup.Size = new System.Drawing.Size(1205, 27);
		this.errorTextLayoutControlGroup.TextVisible = false;
		this.errorIconLayoutControlItem.Control = this.errorIconPictureBox;
		this.errorIconLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.errorIconLayoutControlItem.MaxSize = new System.Drawing.Size(27, 25);
		this.errorIconLayoutControlItem.MinSize = new System.Drawing.Size(27, 25);
		this.errorIconLayoutControlItem.Name = "errorIconLayoutControlItem";
		this.errorIconLayoutControlItem.Size = new System.Drawing.Size(27, 27);
		this.errorIconLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.errorIconLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.errorIconLayoutControlItem.TextVisible = false;
		this.errorTextLayoutControlItem.Control = this.errorTextLabelControl;
		this.errorTextLayoutControlItem.Location = new System.Drawing.Point(27, 0);
		this.errorTextLayoutControlItem.MinSize = new System.Drawing.Size(1, 25);
		this.errorTextLayoutControlItem.Name = "errorTextLayoutControlItem";
		this.errorTextLayoutControlItem.Size = new System.Drawing.Size(1178, 27);
		this.errorTextLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.errorTextLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.errorTextLayoutControlItem.TextVisible = false;
		this.valuesTextLayoutControl1.AllowCustomization = false;
		this.valuesTextLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.valuesTextLayoutControl1.Controls.Add(this.valuesTextLabelControl1);
		this.valuesTextLayoutControl1.Controls.Add(this.valuesIconPictureBox1);
		this.valuesTextLayoutControl1.Location = new System.Drawing.Point(12, 646);
		this.valuesTextLayoutControl1.Margin = new System.Windows.Forms.Padding(4);
		this.valuesTextLayoutControl1.Name = "valuesTextLayoutControl1";
		this.valuesTextLayoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(446, 815, 327, 427);
		this.valuesTextLayoutControl1.Root = this.valuesTextLayoutControlGroup1;
		this.valuesTextLayoutControl1.Size = new System.Drawing.Size(1205, 25);
		this.valuesTextLayoutControl1.TabIndex = 24;
		this.valuesTextLayoutControl1.Text = "layoutControl1";
		this.valuesTextLabelControl1.AutoEllipsis = true;
		this.valuesTextLabelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.valuesTextLabelControl1.Location = new System.Drawing.Point(29, 2);
		this.valuesTextLabelControl1.Margin = new System.Windows.Forms.Padding(4);
		this.valuesTextLabelControl1.MaximumSize = new System.Drawing.Size(0, 20);
		this.valuesTextLabelControl1.MinimumSize = new System.Drawing.Size(0, 20);
		this.valuesTextLabelControl1.Name = "valuesTextLabelControl1";
		this.valuesTextLabelControl1.Size = new System.Drawing.Size(1174, 20);
		this.valuesTextLabelControl1.StyleController = this.valuesTextLayoutControl1;
		this.valuesTextLabelControl1.TabIndex = 23;
		this.valuesTextLabelControl1.Text = "Error text";
		this.valuesIconPictureBox1.Image = Dataedo.App.Properties.Resources.about_16;
		this.valuesIconPictureBox1.Location = new System.Drawing.Point(2, 2);
		this.valuesIconPictureBox1.Margin = new System.Windows.Forms.Padding(0);
		this.valuesIconPictureBox1.MaximumSize = new System.Drawing.Size(21, 20);
		this.valuesIconPictureBox1.MinimumSize = new System.Drawing.Size(21, 20);
		this.valuesIconPictureBox1.Name = "valuesIconPictureBox1";
		this.valuesIconPictureBox1.Size = new System.Drawing.Size(21, 20);
		this.valuesIconPictureBox1.TabIndex = 22;
		this.valuesIconPictureBox1.TabStop = false;
		this.valuesTextLayoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.valuesTextLayoutControlGroup1.GroupBordersVisible = false;
		this.valuesTextLayoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.valuesIconLayoutControlItem1, this.valuesTextLayoutControlItem1 });
		this.valuesTextLayoutControlGroup1.Name = "valuesTextLayoutControlGroup1";
		this.valuesTextLayoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.valuesTextLayoutControlGroup1.Size = new System.Drawing.Size(1205, 25);
		this.valuesTextLayoutControlGroup1.TextVisible = false;
		this.valuesIconLayoutControlItem1.Control = this.valuesIconPictureBox1;
		this.valuesIconLayoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.valuesIconLayoutControlItem1.MaxSize = new System.Drawing.Size(27, 25);
		this.valuesIconLayoutControlItem1.MinSize = new System.Drawing.Size(27, 25);
		this.valuesIconLayoutControlItem1.Name = "valuesIconLayoutControlItem1";
		this.valuesIconLayoutControlItem1.Size = new System.Drawing.Size(27, 25);
		this.valuesIconLayoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.valuesIconLayoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.valuesIconLayoutControlItem1.TextVisible = false;
		this.valuesTextLayoutControlItem1.Control = this.valuesTextLabelControl1;
		this.valuesTextLayoutControlItem1.Location = new System.Drawing.Point(27, 0);
		this.valuesTextLayoutControlItem1.MinSize = new System.Drawing.Size(1, 25);
		this.valuesTextLayoutControlItem1.Name = "valuesTextLayoutControlItem1";
		this.valuesTextLayoutControlItem1.Size = new System.Drawing.Size(1178, 25);
		this.valuesTextLayoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.valuesTextLayoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.valuesTextLayoutControlItem1.TextVisible = false;
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.layoutControlItem1, this.errorInformationLayoutControlItem, this.warningInformationLayoutControlItem, this.valuesInformationlayoutControlItem1 });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(1220, 702);
		this.mainLayoutControlGroup.TextVisible = false;
		this.layoutControlItem1.Control = this.dataGridControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(1220, 613);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.errorInformationLayoutControlItem.Control = this.errorTextLayoutControl;
		this.errorInformationLayoutControlItem.Location = new System.Drawing.Point(0, 613);
		this.errorInformationLayoutControlItem.Name = "errorInformationlayoutControlItem";
		this.errorInformationLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 3, 2, 2);
		this.errorInformationLayoutControlItem.Size = new System.Drawing.Size(1220, 31);
		this.errorInformationLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.errorInformationLayoutControlItem.TextVisible = false;
		this.warningInformationLayoutControlItem.Control = this.warningTextLayoutControl;
		this.warningInformationLayoutControlItem.Location = new System.Drawing.Point(0, 673);
		this.warningInformationLayoutControlItem.Name = "warningInformationLayoutControlItem";
		this.warningInformationLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 3, 2, 2);
		this.warningInformationLayoutControlItem.Size = new System.Drawing.Size(1220, 29);
		this.warningInformationLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.warningInformationLayoutControlItem.TextVisible = false;
		this.valuesInformationlayoutControlItem1.Control = this.valuesTextLayoutControl1;
		this.valuesInformationlayoutControlItem1.Location = new System.Drawing.Point(0, 644);
		this.valuesInformationlayoutControlItem1.Name = "valuesInformationlayoutControlItem1";
		this.valuesInformationlayoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 3, 2, 2);
		this.valuesInformationlayoutControlItem1.Size = new System.Drawing.Size(1220, 29);
		this.valuesInformationlayoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.valuesInformationlayoutControlItem1.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(8f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Margin = new System.Windows.Forms.Padding(4);
		base.Name = "DataToImportUserControl";
		base.Size = new System.Drawing.Size(1220, 702);
		((System.ComponentModel.ISupportInitialize)this.dataGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.warningTextLayoutControl).EndInit();
		this.warningTextLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.warningIconPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.warningTextLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.warningIconLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.warningTextLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.errorTextLayoutControl).EndInit();
		this.errorTextLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.errorIconPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.errorTextLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.errorIconLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.errorTextLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valuesTextLayoutControl1).EndInit();
		this.valuesTextLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.valuesIconPictureBox1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valuesTextLayoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valuesIconLayoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valuesTextLayoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.errorInformationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.warningInformationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.valuesInformationlayoutControlItem1).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
