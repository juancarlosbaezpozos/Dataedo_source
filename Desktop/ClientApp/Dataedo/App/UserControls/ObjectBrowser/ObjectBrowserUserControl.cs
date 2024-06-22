using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Model.Data.Common.Objects;
using Dataedo.Model.Data.Documentations;
using Dataedo.Model.Data.Tables.Relations;
using Dataedo.Shared.Enums;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.UserControls.ObjectBrowser;

public class ObjectBrowserUserControl : BaseUserControl
{
	private DBTreeNode currentObjectTreeNode;

	private ObjectBrowserManager objectBrowserManager;

	private GridHitInfo erdAvailableObjectsGridHitInfo;

	private bool shouldCurrentDatabaseBeSelected;

	private IContainer components;

	private ToolTipControllerUserControl toolTipControllerUserControl;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private TextEdit objectsFilterTextEdit;

	private CheckedComboBoxEdit objectTypesChooserComboBoxEdit;

	private GridControl proposedObjectsGridControl;

	private GridView proposedObjectsGridView;

	private LayoutControlGroup Root;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem filterByEntityTypeLayoutControlItem;

	private LayoutControlItem layoutControlItem5;

	private GridColumn iconGridColumn;

	private GridColumn displayNameGridColumn;

	private GridColumn isSuggestedGridColumn;

	private LabelControl labelControl1;

	private LayoutControlItem layoutControlItem6;

	private CheckedComboBoxEdit databaseChooserComboBoxEdit;

	private LayoutControlItem filterByDatabseLayoutControlItem;

	private GridColumn databaseIconGridColumn;

	private GridColumn databaseTitleGridColumn;

	private SplashScreenManager splashScreenManager;

	private static int? PreviousDatabaseId { get; set; }

	private static List<int> SelectedDatabaseIDs { get; set; }

	private static IEnumerable<SharedObjectTypeEnum.ObjectType> SelectedEntityTypes { get; set; }

	public GridControl GridControl => proposedObjectsGridControl;

	public bool IsSetParametersInProgress { get; private set; }

	public List<RelationTablesIdPair> TableRelations { get; private set; }

	public List<ObjectIdWithType> RelatedObjects { get; private set; }

	public ObjectBrowserUserControl()
	{
		InitializeComponent();
		objectBrowserManager = new ObjectBrowserManager();
		InitObjectTypesChooser();
	}

	public void SetManager(ObjectBrowserManager objectBrowserManager)
	{
		this.objectBrowserManager = objectBrowserManager;
	}

	private void InitObjectTypesChooser()
	{
		((ISupportInitialize)objectTypesChooserComboBoxEdit.Properties).BeginInit();
		objectTypesChooserComboBoxEdit.Properties.Items.Add(SharedObjectTypeEnum.ObjectType.Table, isChecked: true);
		objectTypesChooserComboBoxEdit.Properties.Items.Add(SharedObjectTypeEnum.ObjectType.View, isChecked: true);
		objectTypesChooserComboBoxEdit.Properties.Items.Add(SharedObjectTypeEnum.ObjectType.Procedure, isChecked: true);
		objectTypesChooserComboBoxEdit.Properties.Items.Add(SharedObjectTypeEnum.ObjectType.Function, isChecked: true);
		objectTypesChooserComboBoxEdit.Properties.Items.Add(SharedObjectTypeEnum.ObjectType.Structure, isChecked: true);
		((ISupportInitialize)objectTypesChooserComboBoxEdit.Properties).EndInit();
	}

	public void SetParameters(DBTreeNode dbTreeNode)
	{
		IsSetParametersInProgress = true;
		try
		{
			currentObjectTreeNode = dbTreeNode;
			if (PreviousDatabaseId.HasValue && PreviousDatabaseId != currentObjectTreeNode.DatabaseId)
			{
				shouldCurrentDatabaseBeSelected = true;
			}
			else
			{
				shouldCurrentDatabaseBeSelected = false;
			}
			PreviousDatabaseId = currentObjectTreeNode.DatabaseId;
			SetRememberedEntityTypeFilter();
			SetDatabaseChooserDataSource();
			GetRelations();
			GetRelatedTableIDsBasedOnScript();
			Task.Run(async () => ReloadManagerObjectsAsync());
		}
		finally
		{
			IsSetParametersInProgress = false;
		}
	}

	private void SetRememberedEntityTypeFilter()
	{
		foreach (CheckedListBoxItem item in objectTypesChooserComboBoxEdit.Properties.Items.Cast<CheckedListBoxItem>())
		{
			if (SelectedEntityTypes == null)
			{
				item.CheckState = CheckState.Checked;
			}
			else if (item.Value is SharedObjectTypeEnum.ObjectType value && SelectedEntityTypes.Contains(value))
			{
				item.CheckState = CheckState.Checked;
			}
			else
			{
				item.CheckState = CheckState.Unchecked;
			}
		}
	}

	private void GetRelatedTableIDsBasedOnScript()
	{
		DBTreeNode dBTreeNode = currentObjectTreeNode;
		if (dBTreeNode == null || dBTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Procedure)
		{
			DBTreeNode dBTreeNode2 = currentObjectTreeNode;
			if (dBTreeNode2 == null || dBTreeNode2.ObjectType != SharedObjectTypeEnum.ObjectType.Function)
			{
				DBTreeNode dBTreeNode3 = currentObjectTreeNode;
				if (dBTreeNode3 == null || dBTreeNode3.ObjectType != SharedObjectTypeEnum.ObjectType.View)
				{
					RelatedObjects = null;
					return;
				}
			}
		}
		RelatedObjects = DB.ObjectBrowser.GetObjectRelatedTableIDs(currentObjectTreeNode.Id, currentObjectTreeNode.ObjectType, base.ParentForm);
	}

	private void GetRelations()
	{
		DBTreeNode dBTreeNode = currentObjectTreeNode;
		if (dBTreeNode == null || dBTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Table)
		{
			DBTreeNode dBTreeNode2 = currentObjectTreeNode;
			if (dBTreeNode2 == null || dBTreeNode2.ObjectType != SharedObjectTypeEnum.ObjectType.View)
			{
				DBTreeNode dBTreeNode3 = currentObjectTreeNode;
				if (dBTreeNode3 == null || dBTreeNode3.ObjectType != SharedObjectTypeEnum.ObjectType.Structure)
				{
					TableRelations = null;
					return;
				}
			}
		}
		TableRelations = DB.Relation.GetRelationTablesIDsByObject(currentObjectTreeNode.Id);
	}

	private void ProposedObjectsGrid_MouseDown(object sender, MouseEventArgs e)
	{
		GridView gridView = (sender as GridControl).MainView as GridView;
		erdAvailableObjectsGridHitInfo = gridView.CalcHitInfo(new Point(e.X, e.Y));
	}

	private void ProposedObjectsGrid_MouseMove(object sender, MouseEventArgs e)
	{
		if (erdAvailableObjectsGridHitInfo == null || e.Button != MouseButtons.Left)
		{
			return;
		}
		GridControl gridControl = sender as GridControl;
		GridView gridView = gridControl.MainView as GridView;
		if (erdAvailableObjectsGridHitInfo.InGroupRow || erdAvailableObjectsGridHitInfo.RowHandle < 0)
		{
			DXMouseEventArgs.GetMouseArgs(e).Handled = true;
			return;
		}
		Rectangle rectangle = new Rectangle(new Point(erdAvailableObjectsGridHitInfo.HitPoint.X - SystemInformation.DragSize.Width / 2, erdAvailableObjectsGridHitInfo.HitPoint.Y - SystemInformation.DragSize.Height / 2), SystemInformation.DragSize);
		if (!rectangle.Contains(new Point(e.X, e.Y)) && gridView.GetRow(erdAvailableObjectsGridHitInfo.RowHandle) is ObjectBrowserItem data)
		{
			int topRowIndex = gridView.TopRowIndex;
			int focusedRowHandle = gridView.FocusedRowHandle;
			gridControl.DoDragDrop(data, DragDropEffects.Copy | DragDropEffects.Link);
			gridView.TopRowIndex = topRowIndex;
			gridView.FocusedRowHandle = focusedRowHandle;
		}
	}

	private void ProposedObjectsGridView_CustomDrawGroupRow(object sender, RowObjectCustomDrawEventArgs e)
	{
		e.Appearance.BackColor = SkinColors.GridViewBandBackColor;
		e.Appearance.ForeColor = SkinColors.GridViewBandForeColor;
	}

	private void ProposedObjectsGridView_RowStyle(object sender, RowStyleEventArgs e)
	{
		if (proposedObjectsGridView.IsGroupRow(e.RowHandle))
		{
			e.Appearance.BackColor = proposedObjectsGridView.PaintAppearance.GroupRow.BackColor;
			e.HighPriority = true;
		}
	}

	private void ProposedObjectsGridView_GroupRowCollapsing(object sender, RowAllowEventArgs e)
	{
		e.Allow = false;
	}

	private void ProposedObjectsGridView_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		if (e.Column == isSuggestedGridColumn && e.Value is bool && (bool)e.Value)
		{
			e.DisplayText = "Suggested entities";
		}
		if (e.Column == isSuggestedGridColumn && e.Value is bool && !(bool)e.Value)
		{
			e.DisplayText = "Other entities";
		}
	}

	private void ProposedObjectsGridView_MouseDown(object sender, MouseEventArgs e)
	{
		if (erdAvailableObjectsGridHitInfo.InGroupRow || erdAvailableObjectsGridHitInfo.RowHandle < 0)
		{
			DXMouseEventArgs.GetMouseArgs(e).Handled = true;
		}
	}

	private void erdObjectsFilterTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		SetDataSource();
	}

	private void objectTypesChooserComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (!IsSetParametersInProgress && !(sender as CheckedComboBoxEdit).IsLoading)
		{
			objectBrowserManager.ReloadObjects(GetSelectedDatabaseIDs());
			SelectedEntityTypes = (from CheckedListBoxItem x in objectTypesChooserComboBoxEdit.Properties.Items
				where x.CheckState == CheckState.Checked
				select x.Value).Cast<SharedObjectTypeEnum.ObjectType>();
			SetDataSource();
		}
	}

	private async Task ReloadManagerObjectsAsync()
	{
		Invoke((Action)delegate
		{
			if (base.Visible)
			{
				SetSplashScreenPosition();
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			}
			proposedObjectsGridControl.BeginUpdate();
			proposedObjectsGridControl.DataSource = new List<ObjectBrowserItem>
			{
				CreateDummyObject(iSuggested: true, showCaption: false),
				CreateDummyObject(iSuggested: false)
			};
			proposedObjectsGridControl.EndUpdate();
			objectBrowserManager.SetParameters(currentObjectTreeNode, GetSelectedDatabaseIDs());
			if (base.Visible)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
			SetDataSource();
		});
	}

	public void SetDataSource()
	{
		try
		{
			string filter = objectsFilterTextEdit.Text.ToLower()?.Trim();
			if (filter != null && filter == objectsFilterTextEdit?.Properties?.NullText?.ToLower())
			{
				filter = null;
			}
			proposedObjectsGridControl.BeginUpdate();
			if (objectBrowserManager.ProposedObjects == null)
			{
				proposedObjectsGridControl.EndUpdate();
				return;
			}
			List<ObjectBrowserItem> list = (from x in objectBrowserManager.ProposedObjects
				where !x.Deleted
				orderby x.DisplayName
				select x).ToList();
			if (list == null)
			{
				proposedObjectsGridControl.EndUpdate();
				return;
			}
			IEnumerable<SharedObjectTypeEnum.ObjectType> typesToShow = SelectedEntityTypes ?? (from CheckedListBoxItem x in objectTypesChooserComboBoxEdit.Properties.Items
				where x.CheckState == CheckState.Checked
				select x.Value).Cast<SharedObjectTypeEnum.ObjectType>();
			List<ObjectBrowserItem> source = list.Where((ObjectBrowserItem x) => typesToShow.Contains(x.ObjectType)).ToList();
			List<int> checkedDatabases = GetSelectedDatabaseIDs();
			source = source.Where((ObjectBrowserItem x) => checkedDatabases.Any((int d) => d == x.DatabaseId)).ToList();
			if (!string.IsNullOrWhiteSpace(filter))
			{
				source = source.Where((ObjectBrowserItem x) => x.DisplayName.ToLower().Contains(filter)).ToList();
			}
			SetSuggestedRecords(source);
			if (!source.Any((ObjectBrowserItem x) => x.IsSuggested))
			{
				source.Add(CreateDummyObject(iSuggested: true));
			}
			if (!source.Any((ObjectBrowserItem x) => !x.IsSuggested))
			{
				source.Add(CreateDummyObject(iSuggested: false));
			}
			proposedObjectsGridControl.DataSource = source.OrderByDescending((ObjectBrowserItem x) => x.DisplayName).ToList();
		}
		finally
		{
			proposedObjectsGridControl.EndUpdate();
			proposedObjectsGridControl.Refresh();
		}
	}

	private static ObjectBrowserItem CreateDummyObject(bool iSuggested, bool showCaption = true)
	{
		return new ObjectBrowserItem
		{
			IsDummyObject = true,
			IsSuggested = iSuggested,
			BaseName = (showCaption ? "No results found" : string.Empty)
		};
	}

	private void SetSuggestedRecords(List<ObjectBrowserItem> filteredNodes)
	{
		bool wasSuggestionFound = false;
		List<RelationTablesIdPair> tableRelations = TableRelations;
		if (tableRelations != null && tableRelations.Any())
		{
			foreach (RelationTablesIdPair table in TableRelations)
			{
				foreach (ObjectBrowserItem item in filteredNodes.Where((ObjectBrowserItem x) => (x.Id == table.FkTableId || x.Id == table.PkTableId) && (x.ObjectType == SharedObjectTypeEnum.ObjectType.Table || x.ObjectType == SharedObjectTypeEnum.ObjectType.View || x.ObjectType == SharedObjectTypeEnum.ObjectType.Structure)))
				{
					item.IsSuggested = true;
					wasSuggestionFound = true;
				}
			}
		}
		List<ObjectIdWithType> relatedObjects = RelatedObjects;
		if (relatedObjects != null && relatedObjects.Any())
		{
			foreach (ObjectBrowserItem item2 in filteredNodes.Where((ObjectBrowserItem x) => RelatedObjects.Any((ObjectIdWithType r) => r.ObjectId == x.Id && r.ObjectType == SharedObjectTypeEnum.TypeToString(x.ObjectType))))
			{
				item2.IsSuggested = true;
				wasSuggestionFound = true;
			}
		}
		SetAdditionalSuggestions(filteredNodes, ref wasSuggestionFound);
		if (wasSuggestionFound)
		{
			foreach (ObjectBrowserItem item3 in filteredNodes.Where((ObjectBrowserItem x) => x.IsDummyObject).ToList())
			{
				filteredNodes.Remove(item3);
			}
			return;
		}
		filteredNodes.Where((ObjectBrowserItem x) => x.IsSuggested && !x.IsDummyObject).ToList().ForEach(delegate(ObjectBrowserItem x)
		{
			x.IsSuggested = false;
		});
		foreach (ObjectBrowserItem item4 in filteredNodes.Where((ObjectBrowserItem x) => x.BaseName == currentObjectTreeNode.BaseName))
		{
			item4.IsSuggested = true;
			wasSuggestionFound = true;
		}
	}

	protected virtual void SetAdditionalSuggestions(List<ObjectBrowserItem> nodes, ref bool wasSuggestionFound)
	{
	}

	private List<int> GetSelectedDatabaseIDs()
	{
		return (from x in databaseChooserComboBoxEdit.Properties.Items
			where x.CheckState == CheckState.Checked
			select x into d
			where d.Value is int
			select d into x
			select Convert.ToInt32(x.Value)).ToList();
	}

	private void SetDatabaseChooserDataSource()
	{
		List<DocumentationForMenuObject> dataForMenu = DB.Database.GetDataForMenu();
		((ISupportInitialize)databaseChooserComboBoxEdit.Properties).BeginInit();
		databaseChooserComboBoxEdit.Properties.DataSource = from x in dataForMenu
			where SharedDatabaseClassEnum.StringToType(x.Class) == SharedDatabaseClassEnum.DatabaseClass.Database
			orderby x.Title
			select x;
		databaseChooserComboBoxEdit.Properties.DisplayMember = "Title";
		databaseChooserComboBoxEdit.Properties.ValueMember = "DatabaseId";
		databaseChooserComboBoxEdit.Refresh();
		((ISupportInitialize)databaseChooserComboBoxEdit.Properties).EndInit();
		_ = databaseChooserComboBoxEdit.Properties.GetItems().Count;
		if (SelectedDatabaseIDs != null)
		{
			if (shouldCurrentDatabaseBeSelected)
			{
				CheckedListBoxItem checkedListBoxItem = databaseChooserComboBoxEdit.Properties.Items.FirstOrDefault((CheckedListBoxItem x) => x.Value is int num2 && num2 == currentObjectTreeNode.DatabaseId);
				if (checkedListBoxItem != null)
				{
					checkedListBoxItem.CheckState = CheckState.Checked;
				}
			}
			foreach (int selectedID in SelectedDatabaseIDs)
			{
				CheckedListBoxItem checkedListBoxItem2 = databaseChooserComboBoxEdit.Properties.Items.FirstOrDefault((CheckedListBoxItem x) => x.Value is int num && num == selectedID);
				if (checkedListBoxItem2 != null)
				{
					checkedListBoxItem2.CheckState = CheckState.Checked;
				}
			}
			SelectedDatabaseIDs = GetSelectedDatabaseIDs();
		}
		else
		{
			databaseChooserComboBoxEdit.Properties.Items.ToList().ForEach(delegate(CheckedListBoxItem x)
			{
				x.CheckState = CheckState.Checked;
			});
		}
	}

	private void DatabaseChooserComboBoxEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (!IsSetParametersInProgress && !(sender as CheckedComboBoxEdit).IsLoading && currentObjectTreeNode != null)
		{
			if (databaseChooserComboBoxEdit.Properties.Items.All((CheckedListBoxItem x) => x.CheckState == CheckState.Checked))
			{
				objectBrowserManager.ShowObjectsFromAllDbs = true;
			}
			else
			{
				objectBrowserManager.ShowObjectsFromAllDbs = false;
			}
			List<int> databaseIDs = (SelectedDatabaseIDs = GetSelectedDatabaseIDs());
			objectBrowserManager.ReloadObjects(databaseIDs);
			SetDataSource();
		}
	}

	private void DatabaseChooserComboBoxEdit_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		if (string.IsNullOrEmpty(e.DisplayText))
		{
			e.DisplayText = "No database selected";
		}
		else if (!databaseChooserComboBoxEdit.Properties.Items.Any((CheckedListBoxItem x) => x.CheckState == CheckState.Unchecked))
		{
			e.DisplayText = "All databases";
		}
	}

	private void ObjectTypesChooserComboBoxEdit_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		if (string.IsNullOrEmpty(e.DisplayText))
		{
			e.DisplayText = "No type selected";
		}
		else if (!objectTypesChooserComboBoxEdit.Properties.Items.Any((CheckedListBoxItem x) => x.CheckState == CheckState.Unchecked))
		{
			e.DisplayText = "All objects";
		}
	}

	private void SetSplashScreenPosition()
	{
		Point point = proposedObjectsGridControl.PointToScreen(Point.Empty);
		splashScreenManager.SplashFormLocation = new Point(point.X + base.Width / 2 - 85, point.Y + 32);
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
		this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.UserControls.ObjectBrowser.ObjectBrowserWaitForm), true, true, DevExpress.XtraSplashScreen.SplashFormStartPosition.Manual, new System.Drawing.Point(0, 0), typeof(System.Windows.Forms.UserControl));
		this.toolTipControllerUserControl = new Dataedo.App.UserControls.ToolTipControllerUserControl(this.components);
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.objectsFilterTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.objectTypesChooserComboBoxEdit = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.proposedObjectsGridControl = new DevExpress.XtraGrid.GridControl();
		this.proposedObjectsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.displayNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.isSuggestedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.databaseIconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.databaseTitleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.databaseChooserComboBoxEdit = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.filterByEntityTypeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.filterByDatabseLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.objectsFilterTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectTypesChooserComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.proposedObjectsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.proposedObjectsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseChooserComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.filterByEntityTypeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.filterByDatabseLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.splashScreenManager.ClosingDelay = 500;
		this.toolTipControllerUserControl.AllowHtmlText = true;
		this.toolTipControllerUserControl.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl1.Controls.Add(this.labelControl1);
		this.nonCustomizableLayoutControl1.Controls.Add(this.objectsFilterTextEdit);
		this.nonCustomizableLayoutControl1.Controls.Add(this.objectTypesChooserComboBoxEdit);
		this.nonCustomizableLayoutControl1.Controls.Add(this.proposedObjectsGridControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.databaseChooserComboBoxEdit);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Margin = new System.Windows.Forms.Padding(2);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(883, 230, 812, 500);
		this.nonCustomizableLayoutControl1.Root = this.Root;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(329, 483);
		this.nonCustomizableLayoutControl1.TabIndex = 0;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.labelControl1.Location = new System.Drawing.Point(2, 2);
		this.labelControl1.Margin = new System.Windows.Forms.Padding(2);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(64, 13);
		this.labelControl1.StyleController = this.nonCustomizableLayoutControl1;
		this.labelControl1.TabIndex = 9;
		this.labelControl1.Text = "Display list of:";
		this.objectsFilterTextEdit.Location = new System.Drawing.Point(2, 67);
		this.objectsFilterTextEdit.Margin = new System.Windows.Forms.Padding(2);
		this.objectsFilterTextEdit.Name = "objectsFilterTextEdit";
		this.objectsFilterTextEdit.Properties.NullText = "Type here to filter by entity name ...";
		this.objectsFilterTextEdit.Properties.NullValuePrompt = "Type here to filter...";
		this.objectsFilterTextEdit.Size = new System.Drawing.Size(325, 20);
		this.objectsFilterTextEdit.StyleController = this.nonCustomizableLayoutControl1;
		this.objectsFilterTextEdit.TabIndex = 8;
		this.objectsFilterTextEdit.EditValueChanged += new System.EventHandler(erdObjectsFilterTextEdit_EditValueChanged);
		this.objectTypesChooserComboBoxEdit.EditValue = "";
		this.objectTypesChooserComboBoxEdit.Location = new System.Drawing.Point(95, 19);
		this.objectTypesChooserComboBoxEdit.Margin = new System.Windows.Forms.Padding(2);
		this.objectTypesChooserComboBoxEdit.Name = "objectTypesChooserComboBoxEdit";
		this.objectTypesChooserComboBoxEdit.Properties.AllowMultiSelect = true;
		this.objectTypesChooserComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.objectTypesChooserComboBoxEdit.Size = new System.Drawing.Size(232, 20);
		this.objectTypesChooserComboBoxEdit.StyleController = this.nonCustomizableLayoutControl1;
		this.objectTypesChooserComboBoxEdit.TabIndex = 5;
		this.objectTypesChooserComboBoxEdit.EditValueChanged += new System.EventHandler(objectTypesChooserComboBoxEdit_EditValueChanged);
		this.objectTypesChooserComboBoxEdit.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(ObjectTypesChooserComboBoxEdit_CustomDisplayText);
		this.proposedObjectsGridControl.AllowDrop = true;
		this.proposedObjectsGridControl.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(2);
		this.proposedObjectsGridControl.Location = new System.Drawing.Point(2, 91);
		this.proposedObjectsGridControl.MainView = this.proposedObjectsGridView;
		this.proposedObjectsGridControl.Margin = new System.Windows.Forms.Padding(2);
		this.proposedObjectsGridControl.Name = "proposedObjectsGridControl";
		this.proposedObjectsGridControl.Size = new System.Drawing.Size(325, 390);
		this.proposedObjectsGridControl.TabIndex = 4;
		this.proposedObjectsGridControl.ToolTipController = this.toolTipControllerUserControl;
		this.proposedObjectsGridControl.UseDirectXPaint = DevExpress.Utils.DefaultBoolean.False;
		this.proposedObjectsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.proposedObjectsGridView });
		this.proposedObjectsGridControl.MouseDown += new System.Windows.Forms.MouseEventHandler(ProposedObjectsGrid_MouseDown);
		this.proposedObjectsGridControl.MouseMove += new System.Windows.Forms.MouseEventHandler(ProposedObjectsGrid_MouseMove);
		this.proposedObjectsGridView.AutoFillColumn = this.displayNameGridColumn;
		this.proposedObjectsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.iconGridColumn, this.displayNameGridColumn, this.isSuggestedGridColumn, this.databaseIconGridColumn, this.databaseTitleGridColumn });
		this.proposedObjectsGridView.DetailHeight = 284;
		this.proposedObjectsGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
		this.proposedObjectsGridView.GridControl = this.proposedObjectsGridControl;
		this.proposedObjectsGridView.GroupCount = 1;
		this.proposedObjectsGridView.GroupFormat = "[#image]{1} {2}";
		this.proposedObjectsGridView.Name = "proposedObjectsGridView";
		this.proposedObjectsGridView.OptionsBehavior.AutoExpandAllGroups = true;
		this.proposedObjectsGridView.OptionsBehavior.Editable = false;
		this.proposedObjectsGridView.OptionsBehavior.KeepFocusedRowOnUpdate = false;
		this.proposedObjectsGridView.OptionsBehavior.ReadOnly = true;
		this.proposedObjectsGridView.OptionsView.ShowColumnHeaders = false;
		this.proposedObjectsGridView.OptionsView.ShowDetailButtons = false;
		this.proposedObjectsGridView.OptionsView.ShowGroupExpandCollapseButtons = false;
		this.proposedObjectsGridView.OptionsView.ShowGroupPanel = false;
		this.proposedObjectsGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.proposedObjectsGridView.OptionsView.ShowIndicator = false;
		this.proposedObjectsGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.proposedObjectsGridView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.isSuggestedGridColumn, DevExpress.Data.ColumnSortOrder.Descending)
		});
		this.proposedObjectsGridView.CustomDrawGroupRow += new DevExpress.XtraGrid.Views.Base.RowObjectCustomDrawEventHandler(ProposedObjectsGridView_CustomDrawGroupRow);
		this.proposedObjectsGridView.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(ProposedObjectsGridView_RowStyle);
		this.proposedObjectsGridView.GroupRowCollapsing += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(ProposedObjectsGridView_GroupRowCollapsing);
		this.proposedObjectsGridView.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(ProposedObjectsGridView_CustomColumnDisplayText);
		this.proposedObjectsGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(ProposedObjectsGridView_MouseDown);
		this.displayNameGridColumn.FieldName = "DisplayName";
		this.displayNameGridColumn.MinWidth = 15;
		this.displayNameGridColumn.Name = "displayNameGridColumn";
		this.displayNameGridColumn.OptionsColumn.AllowEdit = false;
		this.displayNameGridColumn.OptionsColumn.AllowFocus = false;
		this.displayNameGridColumn.OptionsColumn.AllowMove = false;
		this.displayNameGridColumn.OptionsColumn.ReadOnly = true;
		this.displayNameGridColumn.Visible = true;
		this.displayNameGridColumn.VisibleIndex = 1;
		this.displayNameGridColumn.Width = 124;
		this.iconGridColumn.FieldName = "NodeIcon";
		this.iconGridColumn.MaxWidth = 19;
		this.iconGridColumn.MinWidth = 19;
		this.iconGridColumn.Name = "iconGridColumn";
		this.iconGridColumn.OptionsColumn.AllowEdit = false;
		this.iconGridColumn.OptionsColumn.AllowFocus = false;
		this.iconGridColumn.OptionsColumn.ReadOnly = true;
		this.iconGridColumn.Visible = true;
		this.iconGridColumn.VisibleIndex = 0;
		this.iconGridColumn.Width = 19;
		this.isSuggestedGridColumn.Caption = " ";
		this.isSuggestedGridColumn.FieldName = "IsSuggested";
		this.isSuggestedGridColumn.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.Value;
		this.isSuggestedGridColumn.MinWidth = 15;
		this.isSuggestedGridColumn.Name = "isSuggestedGridColumn";
		this.isSuggestedGridColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.isSuggestedGridColumn.Visible = true;
		this.isSuggestedGridColumn.VisibleIndex = 2;
		this.isSuggestedGridColumn.Width = 70;
		this.databaseIconGridColumn.Caption = "Database Icon";
		this.databaseIconGridColumn.FieldName = "DatabaseIcon";
		this.databaseIconGridColumn.MaxWidth = 19;
		this.databaseIconGridColumn.MinWidth = 19;
		this.databaseIconGridColumn.Name = "databaseIconGridColumn";
		this.databaseIconGridColumn.OptionsColumn.AllowEdit = false;
		this.databaseIconGridColumn.OptionsColumn.AllowFocus = false;
		this.databaseIconGridColumn.OptionsColumn.ReadOnly = true;
		this.databaseIconGridColumn.OptionsColumn.ShowCaption = false;
		this.databaseIconGridColumn.Visible = true;
		this.databaseIconGridColumn.VisibleIndex = 2;
		this.databaseIconGridColumn.Width = 19;
		this.databaseTitleGridColumn.Caption = "Database Title";
		this.databaseTitleGridColumn.FieldName = "DatabaseTitle";
		this.databaseTitleGridColumn.Name = "databaseTitleGridColumn";
		this.databaseTitleGridColumn.OptionsColumn.AllowEdit = false;
		this.databaseTitleGridColumn.OptionsColumn.AllowFocus = false;
		this.databaseTitleGridColumn.OptionsColumn.AllowMove = false;
		this.databaseTitleGridColumn.OptionsColumn.ReadOnly = true;
		this.databaseTitleGridColumn.OptionsColumn.ShowCaption = false;
		this.databaseTitleGridColumn.Visible = true;
		this.databaseTitleGridColumn.VisibleIndex = 3;
		this.databaseChooserComboBoxEdit.EditValue = "";
		this.databaseChooserComboBoxEdit.Location = new System.Drawing.Point(95, 43);
		this.databaseChooserComboBoxEdit.Margin = new System.Windows.Forms.Padding(2);
		this.databaseChooserComboBoxEdit.Name = "databaseChooserComboBoxEdit";
		this.databaseChooserComboBoxEdit.Properties.AllowMultiSelect = true;
		this.databaseChooserComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.databaseChooserComboBoxEdit.Size = new System.Drawing.Size(232, 20);
		this.databaseChooserComboBoxEdit.StyleController = this.nonCustomizableLayoutControl1;
		this.databaseChooserComboBoxEdit.TabIndex = 5;
		this.databaseChooserComboBoxEdit.EditValueChanged += new System.EventHandler(DatabaseChooserComboBoxEdit_EditValueChanged);
		this.databaseChooserComboBoxEdit.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(DatabaseChooserComboBoxEdit_CustomDisplayText);
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.layoutControlItem1, this.layoutControlItem5, this.layoutControlItem6, this.filterByEntityTypeLayoutControlItem, this.filterByDatabseLayoutControlItem });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(329, 483);
		this.Root.TextVisible = false;
		this.layoutControlItem1.Control = this.proposedObjectsGridControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 89);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(329, 394);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem5.Control = this.objectsFilterTextEdit;
		this.layoutControlItem5.Location = new System.Drawing.Point(0, 65);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(329, 24);
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.layoutControlItem6.Control = this.labelControl1;
		this.layoutControlItem6.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.Size = new System.Drawing.Size(329, 17);
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.filterByEntityTypeLayoutControlItem.Control = this.objectTypesChooserComboBoxEdit;
		this.filterByEntityTypeLayoutControlItem.Location = new System.Drawing.Point(0, 17);
		this.filterByEntityTypeLayoutControlItem.Name = "filterByEntityTypeLayoutControlItem";
		this.filterByEntityTypeLayoutControlItem.Size = new System.Drawing.Size(329, 24);
		this.filterByEntityTypeLayoutControlItem.Text = "Filter by entity type:";
		this.filterByEntityTypeLayoutControlItem.TextSize = new System.Drawing.Size(90, 13);
		this.filterByDatabseLayoutControlItem.Control = this.databaseChooserComboBoxEdit;
		this.filterByDatabseLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.filterByDatabseLayoutControlItem.CustomizationFormText = "Filter by database:";
		this.filterByDatabseLayoutControlItem.Location = new System.Drawing.Point(0, 41);
		this.filterByDatabseLayoutControlItem.Name = "filterByDatabseLayoutControlItem";
		this.filterByDatabseLayoutControlItem.OptionsPrint.AppearanceItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.filterByDatabseLayoutControlItem.OptionsPrint.AppearanceItem.Options.UseFont = true;
		this.filterByDatabseLayoutControlItem.OptionsPrint.AppearanceItemControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.filterByDatabseLayoutControlItem.OptionsPrint.AppearanceItemControl.Options.UseFont = true;
		this.filterByDatabseLayoutControlItem.OptionsPrint.AppearanceItemText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.filterByDatabseLayoutControlItem.OptionsPrint.AppearanceItemText.Options.UseFont = true;
		this.filterByDatabseLayoutControlItem.Size = new System.Drawing.Size(329, 24);
		this.filterByDatabseLayoutControlItem.Text = "Filter by database:";
		this.filterByDatabseLayoutControlItem.TextSize = new System.Drawing.Size(90, 13);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.nonCustomizableLayoutControl1);
		base.Margin = new System.Windows.Forms.Padding(2);
		base.Name = "ObjectBrowserUserControl";
		base.Size = new System.Drawing.Size(329, 483);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.objectsFilterTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectTypesChooserComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.proposedObjectsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.proposedObjectsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseChooserComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.filterByEntityTypeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.filterByDatabseLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
