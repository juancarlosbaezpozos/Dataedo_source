using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize.DataLineage;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Licences;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.OverriddenControls;
using Dataedo.CustomControls;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Dataedo.App.UserControls.DataLineage;

public class DataLineageProcessesUserControl : BaseUserControl
{
	private DBTreeNode currentObjectTreeNode;

	private EventHandler processTreeListNamechangedEventHandler;

	private DataProcessesCollection DataProcessesCollection;

	public EventHandler FocusedProcessChanged;

	private bool isDatasourceRefreshing;

	private IContainer components;

	private LayoutControlGroup Root;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private PopupMenu processesPopupMenu;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarButtonItem addProcessBarButtonItem;

	private BarButtonItem renameProcessBarButtonItem;

	private BarButtonItem removeProcessBarButtonItem;

	private TreeListWithInfoText processesTreeList;

	private TreeListColumn nameTreeListColumn;

	private ImageCollection nodesImageCollection;

	private LayoutControlItem processesTreeListLayoutControlItem;

	private ToolTipController toolTipController;

	[Browsable(true)]
	public event DataLineageUserControl.DataLineageEditedHandler DataLineageEdited;

	public DataLineageProcessesUserControl()
	{
		InitializeComponent();
		processesTreeList.Appearance.SelectedRow.BackColor = SkinColors.FocusColorFromSystemColors;
		processesTreeList.Appearance.SelectedRow.Options.UseBackColor = true;
		processesTreeList.Appearance.HideSelectionRow.BackColor = SkinColors.FocusColorFromSystemColors;
		processesTreeList.Appearance.HideSelectionRow.Options.UseBackColor = true;
	}

	public void SetParameters(DBTreeNode dbTreeNode)
	{
		currentObjectTreeNode = dbTreeNode;
		if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataLineage))
		{
			DataProcessesCollection = new DataProcessesCollection();
			DataProcessesCollection.Init(currentObjectTreeNode.ObjectType);
			processesTreeList.DataSource = DataProcessesCollection;
			ReloadData();
			SetInfoText();
		}
	}

	private void SetInfoText()
	{
		string infoText = string.Empty;
		switch (currentObjectTreeNode.ObjectType)
		{
		case SharedObjectTypeEnum.ObjectType.Table:
			infoText = "Processes are unavailable for tables";
			break;
		case SharedObjectTypeEnum.ObjectType.Structure:
			infoText = "Processes are unavailable for structures";
			break;
		case SharedObjectTypeEnum.ObjectType.View:
			infoText = "Views can only have one process";
			break;
		case SharedObjectTypeEnum.ObjectType.Function:
		case SharedObjectTypeEnum.ObjectType.Procedure:
			infoText = "Right click to add or edit process";
			break;
		}
		processesTreeList.SetInfoText(infoText);
	}

	public void ReloadData()
	{
		DataProcessesCollection.ClearData();
		DataProcessesCollection.LoadData(currentObjectTreeNode, FindForm());
		RefreshDataSource();
		processesTreeList.ExpandAll();
		processesTreeList.MoveFirst();
	}

	public bool CheckIfObjectEquals(int? objectId, SharedObjectTypeEnum.ObjectType? objectType)
	{
		DBTreeNode dBTreeNode = currentObjectTreeNode;
		if ((dBTreeNode != null) ? (dBTreeNode.Id == objectId) : (!objectId.HasValue))
		{
			DBTreeNode dBTreeNode2 = currentObjectTreeNode;
			if (dBTreeNode2 == null)
			{
				return !objectType.HasValue;
			}
			return dBTreeNode2.ObjectType == objectType;
		}
		return false;
	}

	public DataProcessesCollection GetDataProcessesCollection()
	{
		return DataProcessesCollection;
	}

	private void RemoveProcessBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ProcessDataProcessDeletion();
	}

	private void AddProcessBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ProcessDataProcessAdding();
	}

	private void RenameProcessBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ProcessDataProcessRename();
	}

	private void ProcessDataProcessRename()
	{
		DataProcessRow dataProcessRow = processesTreeList.GetFocusedRow() as DataProcessRow;
		if (dataProcessRow == null || dataProcessRow.Source != UserTypeEnum.UserType.USER)
		{
			return;
		}
		string processNameBeforeChange = dataProcessRow.Name;
		processesTreeList.HiddenEditor -= processTreeListNamechangedEventHandler;
		processTreeListNamechangedEventHandler = delegate
		{
			try
			{
				processesTreeList.HiddenEditor -= processTreeListNamechangedEventHandler;
				string focusedDisplayText = processesTreeList.GetFocusedDisplayText();
				if (!string.IsNullOrWhiteSpace(focusedDisplayText) && !processNameBeforeChange.Equals(focusedDisplayText))
				{
					if (DataProcessesCollection.ProcessAlreadyExistsCaseSensitive(focusedDisplayText))
					{
						ShowProcessAlreadyExistingWarning(focusedDisplayText);
						dataProcessRow.Name = processNameBeforeChange;
						RefreshDataSource();
					}
					else if (DataProcessesCollection.IsProcessPendingDeletion(focusedDisplayText))
					{
						ShowProcessPendingDeletionWarning(focusedDisplayText);
						dataProcessRow.Name = processNameBeforeChange;
						RefreshDataSource();
					}
					else
					{
						dataProcessRow.Name = focusedDisplayText;
						if (dataProcessRow.RowState != ManagingRowsEnum.ManagingRows.ForAdding)
						{
							dataProcessRow.RowState = ManagingRowsEnum.ManagingRows.Updated;
						}
					}
				}
			}
			finally
			{
				SetEditModeOnProcessesTreeList(allowEdit: false);
				this.DataLineageEdited?.Invoke();
				DataProcessesCollection.SortProcesses();
				RefreshDataSource();
				processesTreeList.Refresh();
			}
		};
		processesTreeList.CloseEditor();
		processesTreeList.HiddenEditor += processTreeListNamechangedEventHandler;
		SetEditModeOnProcessesTreeList(allowEdit: true);
		processesTreeList.FocusedColumn = nameTreeListColumn;
		processesTreeList.ShowEditor();
	}

	private void SetEditModeOnProcessesTreeList(bool allowEdit)
	{
		nameTreeListColumn.OptionsColumn.AllowEdit = allowEdit;
		processesTreeList.OptionsBehavior.Editable = allowEdit;
		processesTreeList.OptionsBehavior.ReadOnly = !allowEdit;
	}

	private void ProcessDataProcessAdding()
	{
		string text = TextInputForm.ShowForm("Add process", "Name:", null, 250, "OK", "Cancel", FindForm(), allowEmptyValue: false);
		if (string.IsNullOrWhiteSpace(text))
		{
			return;
		}
		if (DataProcessesCollection.ProcessAlreadyExistsIgnoreCase(text))
		{
			ShowProcessAlreadyExistingWarning(text);
			return;
		}
		if (DataProcessesCollection.IsProcessPendingDeletion(text))
		{
			ShowProcessPendingDeletionWarning(text);
			return;
		}
		DataProcessRow addedProcessRow = new DataProcessRow
		{
			Name = text,
			ParentId = currentObjectTreeNode.Id,
			ParentObjectType = currentObjectTreeNode.ObjectType,
			Source = UserTypeEnum.UserType.USER,
			Status = SynchronizeStateEnum.SynchronizeState.New,
			RowState = ManagingRowsEnum.ManagingRows.ForAdding
		};
		DataProcessesCollection.Add(addedProcessRow);
		RefreshDataSource();
		TreeListNode treeListNode = (from x in processesTreeList.GetNodeList()
			where processesTreeList.GetDataRecordByNode(x) == addedProcessRow
			select x).FirstOrDefault();
		if (treeListNode != null)
		{
			processesTreeList.SetFocusedNode(treeListNode);
		}
		this.DataLineageEdited?.Invoke();
		processesTreeList.ExpandAll();
	}

	private void ShowProcessAlreadyExistingWarning(string newProcessName)
	{
		GeneralMessageBoxesHandling.Show("There is already a process with the provided name <i>" + newProcessName + "</i>.<br>Please choose a different name.", "Process already exists", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
	}

	private void ShowProcessPendingDeletionWarning(string newProcessName)
	{
		GeneralMessageBoxesHandling.Show("There is a process with the provided name <i>" + newProcessName + "</i> pending deletion.<br>Please save changes and try again.", "Process pending deletion", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
	}

	private void ProcessDataProcessDeletion()
	{
		try
		{
			if (!(processesTreeList.GetFocusedRow() is DataProcessRow dataProcessRow) || (currentObjectTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.View && dataProcessRow.Source == UserTypeEnum.UserType.USER && DataProcessesCollection.AllDataProcesses.Where((DataProcessRow x) => x.Source == UserTypeEnum.UserType.USER).Count() < 2))
			{
				return;
			}
			if (DataProcessesCollection.AllDataProcesses.Count() < 2)
			{
				GeneralMessageBoxesHandling.Show(SharedObjectTypeEnum.TypeToStringForSingle(currentObjectTreeNode.ObjectType) + " must have at least one process", "Cannot delete process", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
				return;
			}
			int valueOrDefault = DB.DataFlows.GetProcessFlowsNumber(dataProcessRow.Id, "IN").GetValueOrDefault();
			int valueOrDefault2 = DB.DataFlows.GetProcessFlowsNumber(dataProcessRow.Id, "OUT").GetValueOrDefault();
			string empty = string.Empty;
			empty = ((valueOrDefault != 0 || valueOrDefault2 != 0) ? ("You are about to delete the process <b>" + dataProcessRow.Name + "</b> with:" + Environment.NewLine + $"   - {valueOrDefault} inflow" + ((valueOrDefault > 1) ? "s" : string.Empty) + Environment.NewLine + $"   - {valueOrDefault2} outflow" + ((valueOrDefault2 > 1) ? "s" : string.Empty) + Environment.NewLine + "Are you sure you want to proceed?") : ("Are you sure you want to delete the process <b>" + dataProcessRow.Name + "</b>?"));
			GeneralMessageBoxesHandling.HandlingDialogResult handlingDialogResult = GeneralMessageBoxesHandling.Show(empty, "Delete process", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, null, 2, FindForm());
			if (handlingDialogResult != null && handlingDialogResult.DialogResult == DialogResult.Yes && dataProcessRow.CanBeDeleted())
			{
				DataProcessesCollection.Remove(dataProcessRow);
				RefreshDataSource();
				this.DataLineageEdited?.Invoke();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void RefreshDataSource()
	{
		isDatasourceRefreshing = true;
		try
		{
			processesTreeList.RefreshDataSource();
		}
		finally
		{
			isDatasourceRefreshing = false;
		}
		object focusedRow = processesTreeList.GetFocusedRow();
		FocusedProcessChanged?.Invoke(focusedRow, null);
	}

	private void ProcessesTreeList_GetSelectImage(object sender, GetSelectImageEventArgs e)
	{
		object dataRecordByNode = processesTreeList.GetDataRecordByNode(e.Node);
		DataProcessRow dataProcessRow = dataRecordByNode as DataProcessRow;
		if (dataRecordByNode is ProcessesContainer)
		{
			e.NodeImageIndex = nodesImageCollection.Images.IndexOf(nodesImageCollection.Images["folder"]);
		}
		else if (dataProcessRow == null)
		{
			e.NodeImageIndex = -1;
		}
		else if (dataProcessRow.RowState == ManagingRowsEnum.ManagingRows.ForAdding || dataProcessRow.RowState == ManagingRowsEnum.ManagingRows.Added)
		{
			e.NodeImageIndex = nodesImageCollection.Images.IndexOf(nodesImageCollection.Images["process_new"]);
		}
		else
		{
			e.NodeImageIndex = nodesImageCollection.Images.IndexOf(nodesImageCollection.Images["process"]);
		}
	}

	private void ProcessesTreeList_MouseDown(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Right)
		{
			return;
		}
		TreeList treeList = sender as TreeList;
		treeList.Focus();
		TreeListHitInfo treeListHitInfo = treeList.CalcHitInfo(new Point(e.X, e.Y));
		processesPopupMenu.ItemLinks.Clear();
		SharedObjectTypeEnum.ObjectType objectType = currentObjectTreeNode.ObjectType;
		if ((uint)(objectType - 2) <= 1u)
		{
			processesPopupMenu.ItemLinks.Add(addProcessBarButtonItem);
		}
		if (treeListHitInfo != null && treeListHitInfo.InRow)
		{
			treeList.FocusedNode = treeListHitInfo.Node;
			if (treeList.GetDataRecordByNode(treeListHitInfo.Node) is DataProcessRow dataProcessRow)
			{
				if (dataProcessRow.Source == UserTypeEnum.UserType.USER)
				{
					processesPopupMenu.ItemLinks.Add(renameProcessBarButtonItem);
				}
				switch (currentObjectTreeNode.ObjectType)
				{
				case SharedObjectTypeEnum.ObjectType.Function:
				case SharedObjectTypeEnum.ObjectType.Procedure:
					processesPopupMenu.ItemLinks.Add(removeProcessBarButtonItem);
					break;
				case SharedObjectTypeEnum.ObjectType.View:
					if (DataProcessesCollection.AllDataProcesses.Count() > 1)
					{
						processesPopupMenu.ItemLinks.Add(removeProcessBarButtonItem);
					}
					break;
				}
			}
		}
		if (processesPopupMenu.ItemLinks.Any())
		{
			processesPopupMenu.ShowPopup(Control.MousePosition);
		}
	}

	public DataProcessRow GetFocusedRow()
	{
		return processesTreeList?.GetFocusedRow() as DataProcessRow;
	}

	public AllDataFlowsContainer GetAllDataFlowsContainer()
	{
		return DataProcessesCollection?.GetAllDataFlowsContainer();
	}

	private void ProcessesTreeList_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		if (!isDatasourceRefreshing)
		{
			object dataRecordByNode = processesTreeList.GetDataRecordByNode(e.Node);
			FocusedProcessChanged?.Invoke(dataRecordByNode, null);
		}
	}

	public void SelectDataProcess(int processId)
	{
		DataProcessRow dataProcessRow = DataProcessesCollection.AllDataProcesses.Where((DataProcessRow x) => x.Id == processId).FirstOrDefault();
		if (dataProcessRow != null)
		{
			int index = DataProcessesCollection.AllDataProcesses.IndexOf(dataProcessRow);
			TreeListNodes treeListNodes = processesTreeList.Nodes[1]?.Nodes;
			if (treeListNodes != null)
			{
				processesTreeList.FocusedNode = treeListNodes[index];
			}
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			switch (keyData)
			{
			case Keys.F2:
				if (processesTreeList.HasFocus)
				{
					ProcessDataProcessRename();
					return true;
				}
				break;
			case Keys.Delete:
				ProcessDataProcessDeletion();
				break;
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void ToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.DataLineage.DataLineageProcessesUserControl));
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.addProcessBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.renameProcessBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeProcessBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.processesTreeListLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.processesTreeList = new Dataedo.App.UserControls.OverriddenControls.TreeListWithInfoText();
		this.nameTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.nodesImageCollection = new DevExpress.Utils.ImageCollection(this.components);
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.processesPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.processesTreeListLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.processesTreeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nodesImageCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.processesPopupMenu).BeginInit();
		base.SuspendLayout();
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[3] { this.addProcessBarButtonItem, this.renameProcessBarButtonItem, this.removeProcessBarButtonItem });
		this.barManager.MaxItemId = 3;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Margin = new System.Windows.Forms.Padding(2);
		this.barDockControlTop.Size = new System.Drawing.Size(210, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 585);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Margin = new System.Windows.Forms.Padding(2);
		this.barDockControlBottom.Size = new System.Drawing.Size(210, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Margin = new System.Windows.Forms.Padding(2);
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 585);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(210, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Margin = new System.Windows.Forms.Padding(2);
		this.barDockControlRight.Size = new System.Drawing.Size(0, 585);
		this.addProcessBarButtonItem.Caption = "Add Process";
		this.addProcessBarButtonItem.Id = 0;
		this.addProcessBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.process_new_16;
		this.addProcessBarButtonItem.Name = "addProcessBarButtonItem";
		this.addProcessBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(AddProcessBarButtonItem_ItemClick);
		this.renameProcessBarButtonItem.Caption = "Rename Process";
		this.renameProcessBarButtonItem.Id = 1;
		this.renameProcessBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
		this.renameProcessBarButtonItem.Name = "renameProcessBarButtonItem";
		this.renameProcessBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(RenameProcessBarButtonItem_ItemClick);
		this.removeProcessBarButtonItem.Caption = "Remove Process";
		this.removeProcessBarButtonItem.Id = 2;
		this.removeProcessBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeProcessBarButtonItem.Name = "removeProcessBarButtonItem";
		this.removeProcessBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(RemoveProcessBarButtonItem_ItemClick);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.processesTreeListLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(210, 585);
		this.Root.TextVisible = false;
		this.processesTreeListLayoutControlItem.Control = this.processesTreeList;
		this.processesTreeListLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.processesTreeListLayoutControlItem.Name = "processesTreeListLayoutControlItem";
		this.processesTreeListLayoutControlItem.Size = new System.Drawing.Size(210, 585);
		this.processesTreeListLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.processesTreeListLayoutControlItem.TextVisible = false;
		this.processesTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[1] { this.nameTreeListColumn });
		this.processesTreeList.FixedLineWidth = 1;
		this.processesTreeList.HorzScrollStep = 2;
		this.processesTreeList.KeyFieldName = "Id";
		this.processesTreeList.Location = new System.Drawing.Point(2, 2);
		this.processesTreeList.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
		this.processesTreeList.MenuManager = this.barManager;
		this.processesTreeList.MinWidth = 16;
		this.processesTreeList.Name = "processesTreeList";
		this.processesTreeList.OptionsBehavior.AllowExpandOnDblClick = false;
		this.processesTreeList.OptionsBehavior.Editable = false;
		this.processesTreeList.OptionsBehavior.ReadOnly = true;
		this.processesTreeList.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.processesTreeList.OptionsCustomization.AllowColumnMoving = false;
		this.processesTreeList.OptionsCustomization.AllowQuickHideColumns = false;
		this.processesTreeList.OptionsMenu.ShowExpandCollapseItems = false;
		this.processesTreeList.OptionsPrint.AutoRowHeight = false;
		this.processesTreeList.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.processesTreeList.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.None;
		this.processesTreeList.OptionsView.RowImagesShowMode = DevExpress.XtraTreeList.RowImagesShowMode.InCell;
		this.processesTreeList.OptionsView.ShowBandsMode = DevExpress.Utils.DefaultBoolean.False;
		this.processesTreeList.OptionsView.ShowButtons = false;
		this.processesTreeList.OptionsView.ShowColumns = false;
		this.processesTreeList.OptionsView.ShowHorzLines = false;
		this.processesTreeList.OptionsView.ShowIndentAsRowStyle = true;
		this.processesTreeList.OptionsView.ShowIndicator = false;
		this.processesTreeList.OptionsView.ShowRoot = false;
		this.processesTreeList.OptionsView.ShowVertLines = false;
		this.processesTreeList.OptionsView.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.None;
		this.processesTreeList.ParentFieldName = "ParentId";
		this.processesTreeList.PreviewFieldName = "Name";
		this.processesTreeList.RowHeight = 28;
		this.processesTreeList.SelectImageList = this.nodesImageCollection;
		this.processesTreeList.Size = new System.Drawing.Size(206, 581);
		this.processesTreeList.TabIndex = 5;
		this.processesTreeList.ToolTipController = this.toolTipController;
		this.processesTreeList.TreeLevelWidth = 22;
		this.processesTreeList.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(ProcessesTreeList_GetSelectImage);
		this.processesTreeList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(ProcessesTreeList_FocusedNodeChanged);
		this.processesTreeList.MouseDown += new System.Windows.Forms.MouseEventHandler(ProcessesTreeList_MouseDown);
		this.nameTreeListColumn.Caption = "Name";
		this.nameTreeListColumn.FieldName = "Name";
		this.nameTreeListColumn.MinWidth = 16;
		this.nameTreeListColumn.Name = "nameTreeListColumn";
		this.nameTreeListColumn.OptionsColumn.AllowEdit = false;
		this.nameTreeListColumn.OptionsColumn.AllowSort = false;
		this.nameTreeListColumn.OptionsFilter.AllowFilter = false;
		this.nameTreeListColumn.Visible = true;
		this.nameTreeListColumn.VisibleIndex = 0;
		this.nameTreeListColumn.Width = 138;
		this.nodesImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("nodesImageCollection.ImageStream");
		this.nodesImageCollection.InsertImage(Dataedo.App.Properties.Resources.process_32, "process", typeof(Dataedo.App.Properties.Resources), 0, "process_32");
		this.nodesImageCollection.Images.SetKeyName(0, "process");
		this.nodesImageCollection.InsertImage(Dataedo.App.Properties.Resources.process_new_32, "process_new", typeof(Dataedo.App.Properties.Resources), 1, "process_new_32");
		this.nodesImageCollection.Images.SetKeyName(1, "process_new");
		this.nodesImageCollection.InsertImage(Dataedo.App.Properties.Resources.process_deleted_32, "process_deleted", typeof(Dataedo.App.Properties.Resources), 2, "process_deleted_32");
		this.nodesImageCollection.Images.SetKeyName(2, "process_deleted");
		this.nodesImageCollection.InsertImage(Dataedo.App.Properties.Resources.folder_16, "folder", typeof(Dataedo.App.Properties.Resources), 3, "folder_16");
		this.nodesImageCollection.Images.SetKeyName(3, "folder");
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.processesTreeList);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1886, 255, 812, 500);
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(210, 585);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.processesPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.addProcessBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.renameProcessBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeProcessBarButtonItem)
		});
		this.processesPopupMenu.Manager = this.barManager;
		this.processesPopupMenu.Name = "processesPopupMenu";
		this.toolTipController.AllowHtmlText = true;
		this.toolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(ToolTipController_GetActiveObjectInfo);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
		base.Name = "DataLineageProcessesUserControl";
		base.Size = new System.Drawing.Size(210, 585);
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.processesTreeListLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.processesTreeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nodesImageCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.processesPopupMenu).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
