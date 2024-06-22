using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.SchemaImportsAndChanges.Model;
using Dataedo.CustomControls;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.UserControls.SchemaImportsAndChanges;

public class SchemaImportsAndChangesUserControl : BaseUserControl
{
	private readonly Color changedBackColor = SkinsManager.CurrentSkin.SchemaImportsAndChangesPanelUpdatedBackColor;

	private readonly Color removedBackColor = SkinsManager.CurrentSkin.SchemaImportsAndChangesPanelDeletedBackColor;

	private readonly Color addedBackColor = SkinsManager.CurrentSkin.SchemaImportsAndChangesPanelAddedBackColor;

	private readonly Color changedForeColor = SkinsManager.CurrentSkin.SchemaImportsAndChangesPanelUpdatedForeColor;

	private readonly Color removedForeColor = SkinsManager.CurrentSkin.SchemaImportsAndChangesPanelDeletedForeColor;

	private readonly Color addedForeColor = SkinsManager.CurrentSkin.SchemaImportsAndChangesPanelAddedForeColor;

	private UpgradeSchemaChangeTrackingControl upgradeSchemaChangeTrackingControl;

	private float splitterPositionRatio;

	private int? objectId;

	private SharedObjectTypeEnum.ObjectType? objectType;

	private int? documentationId;

	private int? moduleId;

	private bool isSchemaChangeTrackingBlocked;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private SplitContainerControl mainSplitContainerControl;

	private NonCustomizableLayoutControl reportLayoutControl;

	private SchemaImportsAndChangesTreeList reportTreeList;

	private TreeListColumn dateOperationObjectTreeListColumn;

	private TreeListColumn operationTreeListColumn;

	private TreeListColumn typeTreeListColumn;

	private TreeListColumn commentsTreeListColumn;

	private HyperLinkEdit expandAllHyperLinkEdit;

	private HyperLinkEdit collapseAllHyperLinkEdit;

	private LayoutControlGroup reportLayoutControlGroup;

	private LayoutControlItem reportTreeListLayoutControlItem;

	private LayoutControlItem expandAllLayoutControlItem;

	private LayoutControlItem collapseAllLayoutControlItem;

	private EmptySpaceItem topReportMenuEmptySpaceItem;

	private LayoutControlItem layoutControlItem2;

	private EmptySpaceItem topMenuEmptySpaceItem;

	private NonCustomizableLayoutControl differencesDataLayoutControl;

	private LabelControl differencesHeaderLabelControl;

	private GridControl differencesGridControl;

	private PixelScrollingGridView differencesGridView;

	private GridColumn attributeGridColumn;

	private GridColumn beforeGridColumn;

	private RepositoryItemAutoHeightMemoEdit repositoryItemAutoHeightMemoEdit;

	private GridColumn afterGridColumn;

	private LayoutControlGroup differencesDataLayoutControlGroup;

	private LayoutControlItem differencesGridLayoutControlItem;

	private LayoutControlItem differencesHeaderLayoutControlItem;

	private TreeListColumn commentedByTreeListColumn;

	private TreeListColumn commentDateTreeListColumn;

	private PictureBox iconPictureBox;

	private LayoutControlItem iconLayoutControlItem;

	private EmptySpaceItem emptySpaceItem1;

	private LabelControl labelControl1;

	private NonCustomizableLayoutControl disabledChangeTrackingLayoutControl;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem1;

	private EmptySpaceItem emptySpaceItem6;

	private RepositoryItemCustomTextEdit dateOperationObjectRepositoryItemCustomTextEdit;

	private TreeListColumn ConnectionDetailTreeListColumn;

	private TreeListColumn UserTreeListColumn;

	private TreeListColumn DBMSVersionTreeListColumn;

	private ToggleSwitch importsToggleSwitch;

	private LayoutControlItem importsToggleSwitchLayoutControlItem;

	private LinkUserControl linkUserControl;

	public bool IsChanged { get; set; }

	public bool ErrorOccurred { get; set; }

	public bool ExpandAllRequested { get; set; }

	public bool ShowAllImports { get; set; } = true;


	public IEnumerable<SchemaImportsAndChangesObjectModel> FlattenData => (reportTreeList.DataSource as SchemaImportsAndChangesBindingList)?.Flatten();

	[Browsable(true)]
	public event SchemaImportsAndChangesTreeList.SchemaImportsAndChangesObjectModelEventHandler GoToObjectClick;

	[Browsable(true)]
	public event EventHandler RefreshImportsCalled;

	[Browsable(true)]
	public event EventHandler ExpandAllCalled;

	[Browsable(true)]
	public event EventHandler ValueChanged;

	[Browsable(true)]
	public event BeforeExpandEventHandler TreeBeforeExpand
	{
		add
		{
			reportTreeList.BeforeExpand += value;
		}
		remove
		{
			reportTreeList.BeforeExpand -= value;
		}
	}

	public SchemaImportsAndChangesUserControl()
	{
		InitializeComponent();
		SkinsManager.SetToggleSwitchTheme(importsToggleSwitch);
	}

	public void RefreshImports()
	{
		importsToggleSwitch.IsOn = true;
		this.RefreshImportsCalled?.Invoke(null, null);
	}

	public void SetFunctionality()
	{
		if (StaticData.IsProjectFile)
		{
			mainLayoutControl.Visible = false;
			disabledChangeTrackingLayoutControl.Visible = false;
			reportTreeListLayoutControlItem.Enabled = false;
			reportTreeListLayoutControlItem.Visibility = LayoutVisibility.Never;
			linkUserControl.Visible = true;
			linkUserControl.SetParameters(enabled: false, "Schema change tracking requires a server repository to work. <href=" + Links.CreatingServerRepository + ">Learn more</href>", Resources.upgrade_24);
		}
		else
		{
			reportTreeListLayoutControlItem.Enabled = true;
			reportTreeListLayoutControlItem.Visibility = LayoutVisibility.Always;
			mainLayoutControl.Visible = true;
			disabledChangeTrackingLayoutControl.Visible = true;
			linkUserControl.Visible = false;
			if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.SchemaChangeTracking))
			{
				if (!isSchemaChangeTrackingBlocked)
				{
					isSchemaChangeTrackingBlocked = true;
					SetDefaultLayout();
				}
				if (upgradeSchemaChangeTrackingControl == null)
				{
					upgradeSchemaChangeTrackingControl = new UpgradeSchemaChangeTrackingControl
					{
						Dock = DockStyle.Fill
					};
					mainSplitContainerControl.Panel2.Controls.Add(upgradeSchemaChangeTrackingControl);
				}
				upgradeSchemaChangeTrackingControl.Visible = true;
				differencesDataLayoutControl.Visible = false;
			}
			else
			{
				if (isSchemaChangeTrackingBlocked)
				{
					isSchemaChangeTrackingBlocked = false;
					SetDefaultLayout();
				}
				isSchemaChangeTrackingBlocked = false;
				differencesGridControl.Visible = true;
				differencesDataLayoutControl.Visible = true;
				if (upgradeSchemaChangeTrackingControl != null)
				{
					upgradeSchemaChangeTrackingControl.Visible = false;
				}
			}
		}
		reportTreeList.CustomDrawNodeCell += ReportTreeList_CustomDrawNodeCell;
	}

	private void ReportTreeList_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
	{
		if (sender is TreeList treeList && treeList.GetDataRecordByNode(e.Node) is SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel && schemaImportsAndChangesObjectModel.Level == SchemaChangeLevelEnum.SchemaChangeLevel.LicenseWitoutSCT)
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridDisabledGridRowForeColor;
		}
	}

	public void CloseEditors()
	{
		reportTreeList.CloseEditor();
	}

	public void ClearData()
	{
		SetData(null, null, null, null, null);
	}

	public void SetData(int? objectId, SharedObjectTypeEnum.ObjectType? objectType, int? documentationId, int? moduleId, List<SchemaImportsAndChangesObjectModel> data)
	{
		this.objectId = objectId;
		this.objectType = objectType;
		this.documentationId = documentationId;
		this.moduleId = moduleId;
		reportTreeList.BeginUpdate();
		reportTreeList.DataSource = ((data != null) ? new SchemaImportsAndChangesBindingList(data) : null);
		reportTreeList.MakeColumnVisible(dateOperationObjectTreeListColumn);
		AllowNodesToExpandByShowingExpandButton(data);
		reportTreeList.EndUpdate();
		reportTreeList.ForceInitialize();
		iconPictureBox.Image = null;
		differencesHeaderLabelControl.Text = string.Empty;
		differencesGridControl.DataSource = null;
		IsChanged = false;
	}

	private void SchemaImportsAndChangesUserControl_Load(object sender, EventArgs e)
	{
		SetDefaultLayout();
		WorkWithDataedoTrackingHelper.TrackFirstInSessionSCTView();
	}

	private void SetDefaultLayout()
	{
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.SchemaChangeTracking))
		{
			splitterPositionRatio = 0.35f;
			mainSplitContainerControl.SplitterPosition = (int)((float)base.Height * splitterPositionRatio);
		}
		else
		{
			splitterPositionRatio = 0.75f;
			mainSplitContainerControl.SplitterPosition = (int)((float)base.Height * splitterPositionRatio);
		}
	}

	private void SchemaImportsAndChangesUserControl_Resize(object sender, EventArgs e)
	{
		mainSplitContainerControl.SplitterPosition = (int)((float)base.Height * splitterPositionRatio);
	}

	private void MainSplitContainerControl_SplitterMoved(object sender, EventArgs e)
	{
		if (base.Size != Size.Empty)
		{
			splitterPositionRatio = (float)mainSplitContainerControl.SplitterPosition / (float)base.Height;
		}
	}

	private void AllowNodesToExpandByShowingExpandButton(List<SchemaImportsAndChangesObjectModel> data)
	{
		if (data == null)
		{
			return;
		}
		int num = 0;
		foreach (object node in reportTreeList.Nodes)
		{
			TreeListNode treeListNode = node as TreeListNode;
			bool num2 = data[num].Level != SchemaChangeLevelEnum.SchemaChangeLevel.NoResults && (data[num].Data.AddedCount != 0 || data[num].Data.UpdatedCount != 0 || data[num].Data.DeletedCount != 0);
			SharedObjectTypeEnum.StringToType(data[num]?.Data?.ObjectType);
			bool flag = true;
			if (num2 && flag)
			{
				treeListNode.HasChildren = true;
			}
			num++;
		}
	}

	private void ExpandAllHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		ErrorOccurred = false;
		ExpandAllRequested = true;
		this.ExpandAllCalled(reportTreeList, null);
		reportTreeList.ExpandAll();
	}

	private void CollapseAllHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		reportTreeList.CollapseAll();
	}

	private void ReportTreeList_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel = reportTreeList.GetDataRecordByNode(e.Node) as SchemaImportsAndChangesObjectModel;
		differencesHeaderLabelControl.Text = schemaImportsAndChangesObjectModel?.FullChangeText;
		if (schemaImportsAndChangesObjectModel != null && schemaImportsAndChangesObjectModel.MayHaveChanges)
		{
			iconLayoutControlItem.Visibility = LayoutVisibility.Always;
			iconPictureBox.Image = reportTreeList.GetNodeSelectImage(e.Node);
			differencesHeaderLabelControl.Text = schemaImportsAndChangesObjectModel.FullChangeText;
			differencesGridControl.DataSource = schemaImportsAndChangesObjectModel.Differences;
		}
		else
		{
			iconLayoutControlItem.Visibility = LayoutVisibility.Never;
			iconPictureBox.Image = null;
			differencesHeaderLabelControl.Text = string.Empty;
			differencesGridControl.DataSource = null;
		}
	}

	private void ReportTreeList_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		IsChanged = true;
		this.ValueChanged?.Invoke(this, new EventArgs());
	}

	private void DifferencesGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (!((sender as GridView).GetRow(e.RowHandle) is DifferenceModel differenceModel))
		{
			return;
		}
		if (differenceModel.IsAdded)
		{
			if (e.Column.FieldName == "After")
			{
				e.Appearance.BackColor = addedBackColor;
				e.Appearance.ForeColor = addedForeColor;
			}
		}
		else if (differenceModel.IsRemoved)
		{
			if (e.Column.FieldName == "Before")
			{
				e.Appearance.BackColor = removedBackColor;
				e.Appearance.ForeColor = removedForeColor;
			}
		}
		else if (differenceModel.IsChanged && (e.Column.FieldName == "Before" || e.Column.FieldName == "After"))
		{
			e.Appearance.BackColor = changedBackColor;
			e.Appearance.ForeColor = changedForeColor;
		}
	}

	private void ReportTreeList_GoToObjectClick(object sender, SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel)
	{
		this.GoToObjectClick?.Invoke(this, schemaImportsAndChangesObjectModel);
	}

	private void ReportTreeList_PopupShowing(object sender, CancelEventArgs e, SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel)
	{
		e.Cancel = objectType == schemaImportsAndChangesObjectModel.ObjectType && objectId == schemaImportsAndChangesObjectModel.Data?.ObjectIdCurrent;
	}

	private void EnableChangeTrackingSimpleButton_Click(object sender, EventArgs e)
	{
		DB.SchemaImportsAndChanges.SetEnabled(enabled: true);
		SetFunctionality();
		SetDefaultLayout();
	}

	private void DisabledChangeTrackingLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		if (e.MouseArgs.Button == MouseButtons.Left)
		{
			Links.OpenCTALink(e.Link, this, afterLogin: true, null, FindForm());
		}
	}

	private void SchemaImportsAndChangesUserControl_Leave(object sender, EventArgs e)
	{
		reportTreeList.DestroyCustomization();
		differencesGridView.HideCustomization();
	}

	private void DifferencesGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInSchemaImportsAndChanges(e);
	}

	private void ReportTreeList_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
	{
		TreeListCustomizationForm customizationForm = (sender as SchemaImportsAndChangesTreeList)?.CustomizationForm;
		CommonFunctionsPanels.ManageOptionsInHeaderPopupWithoutSorting(e, customizationForm);
	}

	private void ImportsToggleSwitch_Toggled(object sender, EventArgs e)
	{
		if (importsToggleSwitch.IsOn)
		{
			ShowAllImports = true;
			this.RefreshImportsCalled?.Invoke(null, null);
		}
		else if (!importsToggleSwitch.IsOn)
		{
			ShowAllImports = false;
			this.RefreshImportsCalled?.Invoke(null, null);
		}
	}

	private void ReportTreeList_AfterExpand(object sender, NodeEventArgs e)
	{
		if (ErrorOccurred)
		{
			e.Node.Expanded = false;
		}
		SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel = reportTreeList.GetDataRecordByNode(e.Node) as SchemaImportsAndChangesObjectModel;
		if (schemaImportsAndChangesObjectModel.Level == SchemaChangeLevelEnum.SchemaChangeLevel.Date && schemaImportsAndChangesObjectModel.Level != SchemaChangeLevelEnum.SchemaChangeLevel.NoResults && schemaImportsAndChangesObjectModel.Children.Count == 0)
		{
			e.Node.HasChildren = false;
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
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.mainSplitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
		this.reportLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.importsToggleSwitch = new DevExpress.XtraEditors.ToggleSwitch();
		this.reportTreeList = new Dataedo.App.UserControls.SchemaImportsAndChanges.SchemaImportsAndChangesTreeList();
		this.dateOperationObjectTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.dateOperationObjectRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.operationTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.typeTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.commentsTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.commentedByTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.ConnectionDetailTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.commentDateTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.UserTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.DBMSVersionTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.expandAllHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.collapseAllHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.reportLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.reportTreeListLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.collapseAllLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.topReportMenuEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.expandAllLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.topMenuEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.importsToggleSwitchLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.differencesDataLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.iconPictureBox = new System.Windows.Forms.PictureBox();
		this.differencesHeaderLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.differencesGridControl = new DevExpress.XtraGrid.GridControl();
		this.differencesGridView = new Dataedo.App.Tools.PixelScrollingGridView();
		this.attributeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.beforeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemAutoHeightMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.afterGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.differencesDataLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.differencesGridLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.differencesHeaderLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.iconLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.disabledChangeTrackingLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.linkUserControl = new Dataedo.App.UserControls.LinkUserControl();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.mainSplitContainerControl).BeginInit();
		this.mainSplitContainerControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.reportLayoutControl).BeginInit();
		this.reportLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.importsToggleSwitch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.reportTreeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateOperationObjectRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.expandAllHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.collapseAllHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.reportLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.reportTreeListLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.collapseAllLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.topReportMenuEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.expandAllLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.topMenuEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.importsToggleSwitchLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.differencesDataLayoutControl).BeginInit();
		this.differencesDataLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.iconPictureBox).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.differencesGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.differencesGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemAutoHeightMemoEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.differencesDataLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.differencesGridLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.differencesHeaderLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.disabledChangeTrackingLayoutControl).BeginInit();
		this.disabledChangeTrackingLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.mainSplitContainerControl);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 75);
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(1184, 746);
		this.mainLayoutControl.TabIndex = 1;
		this.mainLayoutControl.Text = "layoutControl1";
		this.mainSplitContainerControl.Horizontal = false;
		this.mainSplitContainerControl.Location = new System.Drawing.Point(0, 5);
		this.mainSplitContainerControl.Margin = new System.Windows.Forms.Padding(0);
		this.mainSplitContainerControl.Name = "mainSplitContainerControl";
		this.mainSplitContainerControl.Panel1.Controls.Add(this.reportLayoutControl);
		this.mainSplitContainerControl.Panel1.Text = "Panel1";
		this.mainSplitContainerControl.Panel2.Controls.Add(this.differencesDataLayoutControl);
		this.mainSplitContainerControl.Panel2.Text = "Panel2";
		this.mainSplitContainerControl.Size = new System.Drawing.Size(1184, 741);
		this.mainSplitContainerControl.SplitterPosition = 474;
		this.mainSplitContainerControl.TabIndex = 12;
		this.mainSplitContainerControl.SplitterMoved += new System.EventHandler(MainSplitContainerControl_SplitterMoved);
		this.reportLayoutControl.AllowCustomization = false;
		this.reportLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.reportLayoutControl.Controls.Add(this.importsToggleSwitch);
		this.reportLayoutControl.Controls.Add(this.reportTreeList);
		this.reportLayoutControl.Controls.Add(this.expandAllHyperLinkEdit);
		this.reportLayoutControl.Controls.Add(this.collapseAllHyperLinkEdit);
		this.reportLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.reportLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.reportLayoutControl.Margin = new System.Windows.Forms.Padding(0);
		this.reportLayoutControl.Name = "reportLayoutControl";
		this.reportLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1312, 264, 564, 525);
		this.reportLayoutControl.Root = this.reportLayoutControlGroup;
		this.reportLayoutControl.Size = new System.Drawing.Size(1184, 474);
		this.reportLayoutControl.TabIndex = 0;
		this.reportLayoutControl.Text = "layoutControl1";
		this.importsToggleSwitch.EditValue = true;
		this.importsToggleSwitch.Location = new System.Drawing.Point(7, 2);
		this.importsToggleSwitch.Name = "importsToggleSwitch";
		this.importsToggleSwitch.Properties.OffText = "Changes only";
		this.importsToggleSwitch.Properties.OnText = "All imports";
		this.importsToggleSwitch.Size = new System.Drawing.Size(125, 20);
		this.importsToggleSwitch.StyleController = this.reportLayoutControl;
		this.importsToggleSwitch.TabIndex = 11;
		this.importsToggleSwitch.Toggled += new System.EventHandler(ImportsToggleSwitch_Toggled);
		this.reportTreeList.AllowDrop = true;
		this.reportTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[9] { this.dateOperationObjectTreeListColumn, this.operationTreeListColumn, this.typeTreeListColumn, this.commentsTreeListColumn, this.commentedByTreeListColumn, this.ConnectionDetailTreeListColumn, this.commentDateTreeListColumn, this.UserTreeListColumn, this.DBMSVersionTreeListColumn });
		this.reportTreeList.Font = new System.Drawing.Font("Tahoma", 8.25f);
		this.reportTreeList.KeyFieldName = "Id";
		this.reportTreeList.Location = new System.Drawing.Point(0, 50);
		this.reportTreeList.Margin = new System.Windows.Forms.Padding(0);
		this.reportTreeList.Name = "reportTreeList";
		this.reportTreeList.OptionsView.AutoWidth = false;
		this.reportTreeList.OptionsView.ShowHorzLines = false;
		this.reportTreeList.OptionsView.ShowIndicator = false;
		this.reportTreeList.OptionsView.ShowVertLines = false;
		this.reportTreeList.ParentFieldName = "ParentId";
		this.reportTreeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.dateOperationObjectRepositoryItemCustomTextEdit });
		this.reportTreeList.Size = new System.Drawing.Size(1184, 424);
		this.reportTreeList.TabIndex = 3;
		this.reportTreeList.UseDirectXPaint = DevExpress.Utils.DefaultBoolean.False;
		this.reportTreeList.GoToObjectClick += new Dataedo.App.UserControls.SchemaImportsAndChanges.SchemaImportsAndChangesTreeList.SchemaImportsAndChangesObjectModelEventHandler(ReportTreeList_GoToObjectClick);
		this.reportTreeList.PopupShowing += new Dataedo.App.UserControls.SchemaImportsAndChanges.SchemaImportsAndChangesTreeList.SchemaImportsAndChangesObjectModelCancelEventHandler(ReportTreeList_PopupShowing);
		this.reportTreeList.AfterExpand += new DevExpress.XtraTreeList.NodeEventHandler(ReportTreeList_AfterExpand);
		this.reportTreeList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(ReportTreeList_FocusedNodeChanged);
		this.reportTreeList.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(ReportTreeList_PopupMenuShowing);
		this.reportTreeList.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(ReportTreeList_CellValueChanged);
		this.dateOperationObjectTreeListColumn.Caption = "Date / Operation / Object";
		this.dateOperationObjectTreeListColumn.ColumnEdit = this.dateOperationObjectRepositoryItemCustomTextEdit;
		this.dateOperationObjectTreeListColumn.FieldName = "DateOperationObject";
		this.dateOperationObjectTreeListColumn.MinWidth = 33;
		this.dateOperationObjectTreeListColumn.Name = "dateOperationObjectTreeListColumn";
		this.dateOperationObjectTreeListColumn.OptionsColumn.AllowEdit = false;
		this.dateOperationObjectTreeListColumn.OptionsColumn.AllowFocus = false;
		this.dateOperationObjectTreeListColumn.OptionsColumn.AllowSort = false;
		this.dateOperationObjectTreeListColumn.OptionsColumn.ReadOnly = true;
		this.dateOperationObjectTreeListColumn.Visible = true;
		this.dateOperationObjectTreeListColumn.VisibleIndex = 0;
		this.dateOperationObjectTreeListColumn.Width = 350;
		this.dateOperationObjectRepositoryItemCustomTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.dateOperationObjectRepositoryItemCustomTextEdit.AutoHeight = false;
		this.dateOperationObjectRepositoryItemCustomTextEdit.Name = "dateOperationObjectRepositoryItemCustomTextEdit";
		this.operationTreeListColumn.Caption = "Operation";
		this.operationTreeListColumn.FieldName = "Operation";
		this.operationTreeListColumn.Name = "operationTreeListColumn";
		this.operationTreeListColumn.OptionsColumn.AllowEdit = false;
		this.operationTreeListColumn.OptionsColumn.AllowFocus = false;
		this.operationTreeListColumn.OptionsColumn.AllowSort = false;
		this.operationTreeListColumn.OptionsColumn.ReadOnly = true;
		this.operationTreeListColumn.Visible = true;
		this.operationTreeListColumn.VisibleIndex = 1;
		this.operationTreeListColumn.Width = 100;
		this.typeTreeListColumn.Caption = "Type";
		this.typeTreeListColumn.FieldName = "Type";
		this.typeTreeListColumn.Name = "typeTreeListColumn";
		this.typeTreeListColumn.OptionsColumn.AllowEdit = false;
		this.typeTreeListColumn.OptionsColumn.AllowFocus = false;
		this.typeTreeListColumn.OptionsColumn.AllowSort = false;
		this.typeTreeListColumn.OptionsColumn.ReadOnly = true;
		this.typeTreeListColumn.Visible = true;
		this.typeTreeListColumn.VisibleIndex = 2;
		this.typeTreeListColumn.Width = 100;
		this.commentsTreeListColumn.Caption = "Comments";
		this.commentsTreeListColumn.FieldName = "Comments";
		this.commentsTreeListColumn.Name = "commentsTreeListColumn";
		this.commentsTreeListColumn.OptionsColumn.AllowSort = false;
		this.commentsTreeListColumn.Visible = true;
		this.commentsTreeListColumn.VisibleIndex = 4;
		this.commentsTreeListColumn.Width = 250;
		this.commentedByTreeListColumn.Caption = "Commented by";
		this.commentedByTreeListColumn.FieldName = "CommentedBy";
		this.commentedByTreeListColumn.Name = "commentedByTreeListColumn";
		this.commentedByTreeListColumn.OptionsColumn.AllowEdit = false;
		this.commentedByTreeListColumn.OptionsColumn.AllowFocus = false;
		this.commentedByTreeListColumn.OptionsColumn.AllowSort = false;
		this.commentedByTreeListColumn.OptionsColumn.ReadOnly = true;
		this.commentedByTreeListColumn.Visible = true;
		this.commentedByTreeListColumn.VisibleIndex = 5;
		this.commentedByTreeListColumn.Width = 100;
		this.ConnectionDetailTreeListColumn.Caption = "Connection details";
		this.ConnectionDetailTreeListColumn.FieldName = "ConnectionDetails";
		this.ConnectionDetailTreeListColumn.Name = "ConnectionDetailTreeListColumn";
		this.ConnectionDetailTreeListColumn.OptionsColumn.AllowEdit = false;
		this.ConnectionDetailTreeListColumn.OptionsColumn.AllowFocus = false;
		this.ConnectionDetailTreeListColumn.OptionsColumn.AllowSort = false;
		this.ConnectionDetailTreeListColumn.OptionsColumn.ReadOnly = true;
		this.ConnectionDetailTreeListColumn.Visible = true;
		this.ConnectionDetailTreeListColumn.VisibleIndex = 3;
		this.ConnectionDetailTreeListColumn.Width = 250;
		this.commentDateTreeListColumn.Caption = "Comment date";
		this.commentDateTreeListColumn.FieldName = "CommentDate";
		this.commentDateTreeListColumn.Format.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.commentDateTreeListColumn.Name = "commentDateTreeListColumn";
		this.commentDateTreeListColumn.OptionsColumn.AllowEdit = false;
		this.commentDateTreeListColumn.OptionsColumn.AllowFocus = false;
		this.commentDateTreeListColumn.OptionsColumn.AllowSort = false;
		this.commentDateTreeListColumn.OptionsColumn.ReadOnly = true;
		this.commentDateTreeListColumn.Visible = true;
		this.commentDateTreeListColumn.VisibleIndex = 6;
		this.commentDateTreeListColumn.Width = 100;
		this.UserTreeListColumn.Caption = "User";
		this.UserTreeListColumn.FieldName = "User";
		this.UserTreeListColumn.Name = "UserTreeListColumn";
		this.UserTreeListColumn.OptionsColumn.AllowEdit = false;
		this.UserTreeListColumn.OptionsColumn.AllowFocus = false;
		this.UserTreeListColumn.OptionsColumn.AllowSort = false;
		this.UserTreeListColumn.OptionsColumn.ReadOnly = true;
		this.UserTreeListColumn.Visible = true;
		this.UserTreeListColumn.VisibleIndex = 7;
		this.DBMSVersionTreeListColumn.Caption = "DBMSVersion";
		this.DBMSVersionTreeListColumn.FieldName = "DBMSVersion";
		this.DBMSVersionTreeListColumn.Name = "DBMSVersionTreeListColumn";
		this.DBMSVersionTreeListColumn.OptionsColumn.AllowEdit = false;
		this.DBMSVersionTreeListColumn.OptionsColumn.AllowFocus = false;
		this.DBMSVersionTreeListColumn.OptionsColumn.AllowSort = false;
		this.DBMSVersionTreeListColumn.OptionsColumn.ReadOnly = true;
		this.DBMSVersionTreeListColumn.Visible = true;
		this.DBMSVersionTreeListColumn.VisibleIndex = 8;
		this.DBMSVersionTreeListColumn.Width = 100;
		this.expandAllHyperLinkEdit.EditValue = "Expand all";
		this.expandAllHyperLinkEdit.Location = new System.Drawing.Point(7, 28);
		this.expandAllHyperLinkEdit.Name = "expandAllHyperLinkEdit";
		this.expandAllHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.expandAllHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.expandAllHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.expandAllHyperLinkEdit.Size = new System.Drawing.Size(61, 18);
		this.expandAllHyperLinkEdit.StyleController = this.reportLayoutControl;
		this.expandAllHyperLinkEdit.TabIndex = 2;
		this.expandAllHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(ExpandAllHyperLinkEdit_OpenLink);
		this.collapseAllHyperLinkEdit.EditValue = "Collapse all";
		this.collapseAllHyperLinkEdit.Location = new System.Drawing.Point(72, 28);
		this.collapseAllHyperLinkEdit.MinimumSize = new System.Drawing.Size(65, 0);
		this.collapseAllHyperLinkEdit.Name = "collapseAllHyperLinkEdit";
		this.collapseAllHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.collapseAllHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.collapseAllHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.collapseAllHyperLinkEdit.Size = new System.Drawing.Size(65, 18);
		this.collapseAllHyperLinkEdit.StyleController = this.reportLayoutControl;
		this.collapseAllHyperLinkEdit.TabIndex = 0;
		this.collapseAllHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(CollapseAllHyperLinkEdit_OpenLink);
		this.reportLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.reportLayoutControlGroup.GroupBordersVisible = false;
		this.reportLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[7] { this.reportTreeListLayoutControlItem, this.collapseAllLayoutControlItem, this.topReportMenuEmptySpaceItem, this.expandAllLayoutControlItem, this.topMenuEmptySpaceItem, this.emptySpaceItem6, this.importsToggleSwitchLayoutControlItem });
		this.reportLayoutControlGroup.Name = "Root";
		this.reportLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.reportLayoutControlGroup.Size = new System.Drawing.Size(1184, 474);
		this.reportLayoutControlGroup.TextVisible = false;
		this.reportTreeListLayoutControlItem.Control = this.reportTreeList;
		this.reportTreeListLayoutControlItem.Location = new System.Drawing.Point(0, 50);
		this.reportTreeListLayoutControlItem.MinSize = new System.Drawing.Size(100, 20);
		this.reportTreeListLayoutControlItem.Name = "reportTreeListLayoutControlItem";
		this.reportTreeListLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.reportTreeListLayoutControlItem.Size = new System.Drawing.Size(1184, 424);
		this.reportTreeListLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.reportTreeListLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.reportTreeListLayoutControlItem.TextVisible = false;
		this.collapseAllLayoutControlItem.Control = this.collapseAllHyperLinkEdit;
		this.collapseAllLayoutControlItem.Location = new System.Drawing.Point(70, 26);
		this.collapseAllLayoutControlItem.MaxSize = new System.Drawing.Size(65, 24);
		this.collapseAllLayoutControlItem.MinSize = new System.Drawing.Size(65, 24);
		this.collapseAllLayoutControlItem.Name = "collapseAllLayoutControlItem";
		this.collapseAllLayoutControlItem.Size = new System.Drawing.Size(65, 24);
		this.collapseAllLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.collapseAllLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.collapseAllLayoutControlItem.TextVisible = false;
		this.topReportMenuEmptySpaceItem.AllowHotTrack = false;
		this.topReportMenuEmptySpaceItem.Location = new System.Drawing.Point(135, 26);
		this.topReportMenuEmptySpaceItem.Name = "topReportMenuEmptySpaceItem";
		this.topReportMenuEmptySpaceItem.Size = new System.Drawing.Size(1049, 24);
		this.topReportMenuEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.expandAllLayoutControlItem.Control = this.expandAllHyperLinkEdit;
		this.expandAllLayoutControlItem.Location = new System.Drawing.Point(5, 26);
		this.expandAllLayoutControlItem.MaxSize = new System.Drawing.Size(65, 24);
		this.expandAllLayoutControlItem.MinSize = new System.Drawing.Size(65, 24);
		this.expandAllLayoutControlItem.Name = "expandAllLayoutControlItem";
		this.expandAllLayoutControlItem.Size = new System.Drawing.Size(65, 24);
		this.expandAllLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.expandAllLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.expandAllLayoutControlItem.TextVisible = false;
		this.topMenuEmptySpaceItem.AllowHotTrack = false;
		this.topMenuEmptySpaceItem.Location = new System.Drawing.Point(134, 0);
		this.topMenuEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 26);
		this.topMenuEmptySpaceItem.MinSize = new System.Drawing.Size(104, 26);
		this.topMenuEmptySpaceItem.Name = "topMenuEmptySpaceItem";
		this.topMenuEmptySpaceItem.Size = new System.Drawing.Size(1050, 26);
		this.topMenuEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.topMenuEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem6.AllowHotTrack = false;
		this.emptySpaceItem6.Location = new System.Drawing.Point(0, 0);
		this.emptySpaceItem6.MaxSize = new System.Drawing.Size(5, 0);
		this.emptySpaceItem6.MinSize = new System.Drawing.Size(5, 10);
		this.emptySpaceItem6.Name = "emptySpaceItem6";
		this.emptySpaceItem6.Size = new System.Drawing.Size(5, 50);
		this.emptySpaceItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		this.importsToggleSwitchLayoutControlItem.Control = this.importsToggleSwitch;
		this.importsToggleSwitchLayoutControlItem.Location = new System.Drawing.Point(5, 0);
		this.importsToggleSwitchLayoutControlItem.Name = "importsToggleSwitchLayoutControlItem";
		this.importsToggleSwitchLayoutControlItem.Size = new System.Drawing.Size(129, 26);
		this.importsToggleSwitchLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.importsToggleSwitchLayoutControlItem.TextVisible = false;
		this.differencesDataLayoutControl.AllowCustomization = false;
		this.differencesDataLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.differencesDataLayoutControl.Controls.Add(this.iconPictureBox);
		this.differencesDataLayoutControl.Controls.Add(this.differencesHeaderLabelControl);
		this.differencesDataLayoutControl.Controls.Add(this.differencesGridControl);
		this.differencesDataLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.differencesDataLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.differencesDataLayoutControl.Name = "differencesDataLayoutControl";
		this.differencesDataLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1169, 271, 692, 610);
		this.differencesDataLayoutControl.Root = this.differencesDataLayoutControlGroup;
		this.differencesDataLayoutControl.Size = new System.Drawing.Size(1184, 257);
		this.differencesDataLayoutControl.TabIndex = 11;
		this.iconPictureBox.Location = new System.Drawing.Point(2, 4);
		this.iconPictureBox.Margin = new System.Windows.Forms.Padding(0);
		this.iconPictureBox.MaximumSize = new System.Drawing.Size(0, 16);
		this.iconPictureBox.MinimumSize = new System.Drawing.Size(0, 16);
		this.iconPictureBox.Name = "iconPictureBox";
		this.iconPictureBox.Size = new System.Drawing.Size(16, 16);
		this.iconPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
		this.iconPictureBox.TabIndex = 13;
		this.iconPictureBox.TabStop = false;
		this.differencesHeaderLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.differencesHeaderLabelControl.Appearance.Options.UseFont = true;
		this.differencesHeaderLabelControl.AutoEllipsis = true;
		this.differencesHeaderLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.differencesHeaderLabelControl.Location = new System.Drawing.Point(22, 2);
		this.differencesHeaderLabelControl.Margin = new System.Windows.Forms.Padding(0);
		this.differencesHeaderLabelControl.MaximumSize = new System.Drawing.Size(0, 20);
		this.differencesHeaderLabelControl.MinimumSize = new System.Drawing.Size(0, 20);
		this.differencesHeaderLabelControl.Name = "differencesHeaderLabelControl";
		this.differencesHeaderLabelControl.Padding = new System.Windows.Forms.Padding(2);
		this.differencesHeaderLabelControl.Size = new System.Drawing.Size(1160, 20);
		this.differencesHeaderLabelControl.StyleController = this.differencesDataLayoutControl;
		this.differencesHeaderLabelControl.TabIndex = 12;
		this.differencesGridControl.Location = new System.Drawing.Point(0, 30);
		this.differencesGridControl.MainView = this.differencesGridView;
		this.differencesGridControl.Margin = new System.Windows.Forms.Padding(0);
		this.differencesGridControl.Name = "differencesGridControl";
		this.differencesGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemAutoHeightMemoEdit });
		this.differencesGridControl.Size = new System.Drawing.Size(1184, 227);
		this.differencesGridControl.TabIndex = 11;
		this.differencesGridControl.UseDirectXPaint = DevExpress.Utils.DefaultBoolean.False;
		this.differencesGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.differencesGridView });
		this.differencesGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.attributeGridColumn, this.beforeGridColumn, this.afterGridColumn });
		this.differencesGridView.GridControl = this.differencesGridControl;
		this.differencesGridView.Name = "differencesGridView";
		this.differencesGridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
		this.differencesGridView.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
		this.differencesGridView.OptionsBehavior.Editable = false;
		this.differencesGridView.OptionsBehavior.ReadOnly = true;
		this.differencesGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.differencesGridView.OptionsCustomization.AllowSort = false;
		this.differencesGridView.OptionsDetail.EnableMasterViewMode = false;
		this.differencesGridView.OptionsFilter.AllowFilterEditor = false;
		this.differencesGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.differencesGridView.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.differencesGridView.OptionsSelection.MultiSelect = true;
		this.differencesGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.differencesGridView.OptionsView.ColumnAutoWidth = false;
		this.differencesGridView.OptionsView.RowAutoHeight = true;
		this.differencesGridView.OptionsView.ShowGroupPanel = false;
		this.differencesGridView.OptionsView.ShowIndicator = false;
		this.differencesGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(DifferencesGridView_CustomDrawCell);
		this.differencesGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(DifferencesGridView_PopupMenuShowing);
		this.attributeGridColumn.Caption = "Attribute";
		this.attributeGridColumn.FieldName = "Name";
		this.attributeGridColumn.Name = "attributeGridColumn";
		this.attributeGridColumn.OptionsFilter.AllowFilter = false;
		this.attributeGridColumn.Visible = true;
		this.attributeGridColumn.VisibleIndex = 0;
		this.attributeGridColumn.Width = 200;
		this.beforeGridColumn.Caption = "Before";
		this.beforeGridColumn.ColumnEdit = this.repositoryItemAutoHeightMemoEdit;
		this.beforeGridColumn.FieldName = "Before";
		this.beforeGridColumn.Name = "beforeGridColumn";
		this.beforeGridColumn.OptionsFilter.AllowFilter = false;
		this.beforeGridColumn.Visible = true;
		this.beforeGridColumn.VisibleIndex = 1;
		this.beforeGridColumn.Width = 400;
		this.repositoryItemAutoHeightMemoEdit.Name = "repositoryItemAutoHeightMemoEdit";
		this.repositoryItemAutoHeightMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.afterGridColumn.Caption = "After";
		this.afterGridColumn.ColumnEdit = this.repositoryItemAutoHeightMemoEdit;
		this.afterGridColumn.FieldName = "After";
		this.afterGridColumn.Name = "afterGridColumn";
		this.afterGridColumn.OptionsFilter.AllowFilter = false;
		this.afterGridColumn.Visible = true;
		this.afterGridColumn.VisibleIndex = 2;
		this.afterGridColumn.Width = 400;
		this.differencesDataLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.differencesDataLayoutControlGroup.GroupBordersVisible = false;
		this.differencesDataLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.differencesGridLayoutControlItem, this.differencesHeaderLayoutControlItem, this.iconLayoutControlItem, this.emptySpaceItem1 });
		this.differencesDataLayoutControlGroup.Name = "Root";
		this.differencesDataLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.differencesDataLayoutControlGroup.Size = new System.Drawing.Size(1184, 257);
		this.differencesDataLayoutControlGroup.TextVisible = false;
		this.differencesGridLayoutControlItem.Control = this.differencesGridControl;
		this.differencesGridLayoutControlItem.Location = new System.Drawing.Point(0, 30);
		this.differencesGridLayoutControlItem.MinSize = new System.Drawing.Size(104, 24);
		this.differencesGridLayoutControlItem.Name = "differencesGridLayoutControlItem";
		this.differencesGridLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.differencesGridLayoutControlItem.Size = new System.Drawing.Size(1184, 227);
		this.differencesGridLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.differencesGridLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.differencesGridLayoutControlItem.TextVisible = false;
		this.differencesHeaderLayoutControlItem.Control = this.differencesHeaderLabelControl;
		this.differencesHeaderLayoutControlItem.Location = new System.Drawing.Point(20, 0);
		this.differencesHeaderLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.differencesHeaderLayoutControlItem.MinSize = new System.Drawing.Size(13, 24);
		this.differencesHeaderLayoutControlItem.Name = "differencesHeaderLayoutControlItem";
		this.differencesHeaderLayoutControlItem.Size = new System.Drawing.Size(1164, 24);
		this.differencesHeaderLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.differencesHeaderLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.differencesHeaderLayoutControlItem.TextVisible = false;
		this.iconLayoutControlItem.Control = this.iconPictureBox;
		this.iconLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.iconLayoutControlItem.MaxSize = new System.Drawing.Size(20, 20);
		this.iconLayoutControlItem.MinSize = new System.Drawing.Size(20, 20);
		this.iconLayoutControlItem.Name = "iconLayoutControlItem";
		this.iconLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 4, 2);
		this.iconLayoutControlItem.Size = new System.Drawing.Size(20, 24);
		this.iconLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.iconLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.iconLayoutControlItem.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 24);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 6);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 6);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(1184, 6);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem2 });
		this.mainLayoutControlGroup.Name = "mainLayoutControlGroup";
		this.mainLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 5, 0);
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(1184, 746);
		this.mainLayoutControlGroup.TextVisible = false;
		this.layoutControlItem2.Control = this.mainSplitContainerControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(104, 24);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem2.Size = new System.Drawing.Size(1184, 741);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.disabledChangeTrackingLayoutControl.AllowCustomization = false;
		this.disabledChangeTrackingLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.disabledChangeTrackingLayoutControl.Controls.Add(this.labelControl1);
		this.disabledChangeTrackingLayoutControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.disabledChangeTrackingLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.disabledChangeTrackingLayoutControl.Name = "disabledChangeTrackingLayoutControl";
		this.disabledChangeTrackingLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2298, 239, 250, 350);
		this.disabledChangeTrackingLayoutControl.Root = this.layoutControlGroup1;
		this.disabledChangeTrackingLayoutControl.Size = new System.Drawing.Size(1184, 39);
		this.disabledChangeTrackingLayoutControl.TabIndex = 16;
		this.disabledChangeTrackingLayoutControl.Text = "layoutControl1";
		this.labelControl1.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold);
		this.labelControl1.Appearance.Options.UseFont = true;
		this.labelControl1.Appearance.Options.UseTextOptions = true;
		this.labelControl1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.labelControl1.Location = new System.Drawing.Point(7, 10);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(1175, 16);
		this.labelControl1.StyleController = this.disabledChangeTrackingLayoutControl;
		this.labelControl1.TabIndex = 14;
		this.labelControl1.Text = "Schema Imports and Changes";
		this.labelControl1.UseMnemonic = false;
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem1 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(5, 0, 5, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(1184, 39);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.Control = this.labelControl1;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 23);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(17, 23);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(1179, 34);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 3, 0);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.linkUserControl.BackColor = System.Drawing.Color.FromArgb(83, 83, 83);
		this.linkUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.linkUserControl.Location = new System.Drawing.Point(0, 39);
		this.linkUserControl.Name = "linkUserControl";
		this.linkUserControl.Size = new System.Drawing.Size(1184, 36);
		this.linkUserControl.TabIndex = 13;
		this.linkUserControl.Visible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.mainLayoutControl);
		base.Controls.Add(this.linkUserControl);
		base.Controls.Add(this.disabledChangeTrackingLayoutControl);
		base.Margin = new System.Windows.Forms.Padding(0);
		base.Name = "SchemaImportsAndChangesUserControl";
		base.Size = new System.Drawing.Size(1184, 821);
		base.Load += new System.EventHandler(SchemaImportsAndChangesUserControl_Load);
		base.Leave += new System.EventHandler(SchemaImportsAndChangesUserControl_Leave);
		base.Resize += new System.EventHandler(SchemaImportsAndChangesUserControl_Resize);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.mainSplitContainerControl).EndInit();
		this.mainSplitContainerControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.reportLayoutControl).EndInit();
		this.reportLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.importsToggleSwitch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.reportTreeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateOperationObjectRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.expandAllHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.collapseAllHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.reportLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.reportTreeListLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.collapseAllLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.topReportMenuEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.expandAllLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.topMenuEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.importsToggleSwitchLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.differencesDataLayoutControl).EndInit();
		this.differencesDataLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.iconPictureBox).EndInit();
		((System.ComponentModel.ISupportInitialize)this.differencesGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.differencesGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemAutoHeightMemoEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.differencesDataLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.differencesGridLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.differencesHeaderLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.disabledChangeTrackingLayoutControl).EndInit();
		this.disabledChangeTrackingLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		base.ResumeLayout(false);
	}
}
