using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Shared.Enums;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class AddTermRelationshipsForm : BaseXtraForm
{
	private MetadataEditorUserControl mainControl;

	private BindingList<TermObjectWithRelationshipExtended> termObjectsWithRelationship;

	private RepositoryItemHyperLinkEdit addNewRelationshipRepositoryItemHyperLinkEdit;

	private RepositoryItemButtonEdit emptyRepositoryItemButtonEdit = new RepositoryItemButtonEdit();

	private bool isModified;

	private IContainer components;

	private LayoutControlGroup rootLayoutControlGroup;

	private GridControl termsGrid;

	private BulkCopyGridUserControl termsGridView;

	private RepositoryItemPictureEdit iconRepositoryItemPictureEdit;

	private GridColumn titleGridColumn;

	private RepositoryItemCustomTextEdit titleRepositoryItemCustomTextEdit;

	private GridColumn typeTableGridColumn;

	private GridColumn createdGridColumn;

	private GridColumn createdByGridColumn;

	private GridColumn lastUpdatedGridColumn;

	private GridColumn lastUpdatedByGridColumn;

	private GridPanelUserControl gridPanelUserControl;

	private LayoutControlItem termsLayoutControlItem;

	private LayoutControlItem gridPanelLayoutControlItem;

	private EmptySpaceItem leftButtonsEmptySpaceItem;

	private SimpleButton saveSimpleButton;

	private LayoutControlItem saveLayoutControlItem;

	private SimpleButton cancelSimpleButton;

	private LayoutControlItem cancelLayoutControlItem;

	private EmptySpaceItem beetwenButtonsEmptySpaceItem;

	private EmptySpaceItem bottomAboveButtonsEmptySpaceItem;

	private ToolTipController termsToolTipController;

	private GridColumn isSelectedGridColumn;

	private GridColumn iconGridColumn;

	private GridColumn commentsGridColumn;

	private GridColumn relationshipTypeGridColumn;

	private RepositoryItemCheckEdit isSelectedRepositoryItemCheckEdit;

	private GridColumn addNewRelationshipGridColumn;

	private GridColumn glossaryTitleGridColumn;

	private NonCustomizableLayoutControl rootLayoutControl;

	private RepositoryItemLookUpEdit relationshipTypeRepositoryItemLookUpEdit;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	public List<TermRelationshipTypeObject> TermRelationshipTypes { get; private set; }

	public AddTermRelationshipsForm()
	{
		InitializeComponent();
		addNewRelationshipRepositoryItemHyperLinkEdit = new RepositoryItemHyperLinkEdit
		{
			Caption = "Add copy"
		};
		addNewRelationshipRepositoryItemHyperLinkEdit.Click += AddNewRelationshipRepositoryItemHyperLinkEdit_Click;
		emptyRepositoryItemButtonEdit = new RepositoryItemButtonEdit();
		emptyRepositoryItemButtonEdit.Buttons.Clear();
		emptyRepositoryItemButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
		AddEvents();
		termsGridView.Copy = new BulkCopyWithBlockedIsSelectedCells<TermObjectWithRelationshipExtended>();
	}

	public void SetParameters(int termIdToAddRelationTo, int? termIdFilter, MetadataEditorUserControl mainControl, CustomFieldsSupport customFieldsSupport)
	{
		this.mainControl = mainControl;
		TermObject term = DB.BusinessGlossary.GetTerm(termIdToAddRelationTo);
		if (term == null)
		{
			GeneralMessageBoxesHandling.Show("Term does not exist in repository.", "Term does not exist", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this);
			Close();
			return;
		}
		Text = "'" + term.Title + "' relationships";
		termsGridView.SetRowCellValue(-2147483646, "iconGridColumn", Resources.blank_16);
		new CustomFieldsCellsTypesSupport(isForSummaryTable: true).SetCustomColumns(termsGridView, customFieldsSupport, SharedObjectTypeEnum.ObjectType.Term, customFieldsAsArray: true);
		foreach (GridColumn item in termsGridView.Columns.Where((GridColumn x) => x.FieldName != "IsSelected" && x.FieldName != "RelationshipType" && x.FieldName != "RelationshipDescription" && x.FieldName != "AddNewRelationshipText"))
		{
			item.OptionsColumn.AllowEdit = false;
		}
		foreach (GridColumn item2 in termsGridView.Columns.Where((GridColumn x) => x.FieldName.Contains("Field") || x.FieldName.Contains("field")))
		{
			item2.Visible = false;
		}
		termObjectsWithRelationship = new BindingList<TermObjectWithRelationshipExtended>(DB.BusinessGlossary.GetTermsAndTermRelationships(termIdToAddRelationTo, termIdFilter));
		termsGrid.DataSource = termObjectsWithRelationship;
		int baseRelationshipTypeGridColumnWidth = relationshipTypeGridColumn.Width;
		int baseAddNewRelationshipGridColumnWidth = addNewRelationshipGridColumn.Width;
		CommonFunctionsPanels.SetBestFitForColumns(termsGridView);
		relationshipTypeGridColumn.Width = ((relationshipTypeGridColumn.Width < baseRelationshipTypeGridColumnWidth) ? baseRelationshipTypeGridColumnWidth : relationshipTypeGridColumn.Width);
		addNewRelationshipGridColumn.Width = ((addNewRelationshipGridColumn.Width < baseAddNewRelationshipGridColumnWidth) ? baseAddNewRelationshipGridColumnWidth : addNewRelationshipGridColumn.Width);
		TermRelationshipTypes = DB.BusinessGlossary.GetTermRelationshipTypes();
		isModified = false;
		relationshipTypeRepositoryItemLookUpEdit.DataSource = TermRelationshipTypes;
		gridPanelUserControl.SetRemoveButtonVisibility(value: false);
		gridPanelUserControl.SetDefineCustomFieldsButtonVisibility(value: false);
		gridPanelUserControl.Initialize(SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Term), isSummaryControlOnModule: false, isColumnsForm: false, new GridColumn[2] { relationshipTypeGridColumn, addNewRelationshipGridColumn });
		gridPanelUserControl.CustomFields += mainControl.EditCustomFields;
		gridPanelUserControl.BeforeEndUpdateSettingDefualtView += delegate
		{
			relationshipTypeGridColumn.Width = baseRelationshipTypeGridColumnWidth;
			addNewRelationshipGridColumn.Width = baseAddNewRelationshipGridColumnWidth;
		};
	}

	protected void AddEvents()
	{
		List<ToolTipData> toolTipDataList = new List<ToolTipData>
		{
			new ToolTipData(termsGrid, SharedObjectTypeEnum.ObjectType.Term, iconGridColumn.VisibleIndex),
			new ToolTipData(termsGrid, SharedObjectTypeEnum.ObjectType.Term, titleGridColumn.VisibleIndex)
		};
		CommonFunctionsPanels.AddEventsForToolTips(termsToolTipController, toolTipDataList);
		CommonFunctionsPanels.AddEventForAutoFilterRow(termsGridView);
		termsGridView.CustomUnboundColumnData += delegate(object sender, CustomColumnDataEventArgs e)
		{
			Icons.SetIcon(e, iconGridColumn, SharedObjectTypeEnum.ObjectType.Term);
		};
		termsGridView.PopupMenuShowing += delegate(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
		{
			CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
		};
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.S | Keys.Control:
			base.DialogResult = DialogResult.OK;
			Close();
			break;
		case Keys.Escape:
			Close();
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void TermsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
	}

	private void TermsGridView_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (termsGridView.FocusedRowHandle != -2147483646)
		{
			TermObjectWithRelationshipExtended termObjectWithRelationshipExtended = termsGridView.GetRow(termsGridView.FocusedRowHandle) as TermObjectWithRelationshipExtended;
			bool flag = IsSelectedColumn(termsGridView.FocusedColumn);
			if ((!flag && (termObjectWithRelationshipExtended == null || !termObjectWithRelationshipExtended.IsSelected)) || ((termObjectWithRelationshipExtended?.IsCurrentlyAdded ?? false) && flag))
			{
				e.Cancel = true;
			}
		}
	}

	private void TermsGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (e.RowHandle != -2147483646)
		{
			TermObjectWithRelationshipExtended termObjectWithRelationshipExtended = termsGridView.GetRow(e.RowHandle) as TermObjectWithRelationshipExtended;
			bool flag = IsSelectedColumn(e.Column);
			if (((termObjectWithRelationshipExtended?.IsSelected ?? false) || flag) && (termObjectWithRelationshipExtended == null || !termObjectWithRelationshipExtended.IsCurrentlyAdded || !flag))
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.GridGridRowBackColor;
				e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridGridRowForeColor;
				Grids.SetYellowRowColor(sender, e, termObjectWithRelationshipExtended.IsModified && (!termObjectWithRelationshipExtended.IsNew || termObjectWithRelationshipExtended.IsSelected));
			}
			else
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
				e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridDisabledGridRowForeColor;
			}
			if (termObjectWithRelationshipExtended.IsValidated && termObjectWithRelationshipExtended.IsInvalid && (termObjectWithRelationshipExtended.IsModified || termObjectWithRelationshipExtended.IsNew) && e.Column.FieldName == "RelationshipType")
			{
				GridCellInfo obj = e.Cell as GridCellInfo;
				obj.ViewInfo.ErrorIconText = "Relationship type is required";
				obj.ViewInfo.ShowErrorIcon = true;
				obj.ViewInfo.CalcViewInfo(e.Graphics);
			}
		}
	}

	private void TermsGridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		if (e.RowHandle == -2147483646)
		{
			TermsGridView_CustomRowCellEditForAutoFilterRow(sender, e);
		}
		else
		{
			if (termObjectsWithRelationship == null || TermRelationshipTypes == null)
			{
				return;
			}
			TermObjectWithRelationshipExtended termObject = termsGridView.GetRow(e.RowHandle) as TermObjectWithRelationshipExtended;
			if (termObject == null)
			{
				return;
			}
			IEnumerable<TermObjectWithRelationshipExtended> source = termObjectsWithRelationship.Where((TermObjectWithRelationshipExtended x) => x.TermId == termObject.TermId);
			IEnumerable<int> currentlyAddedRelationshipTypes = from x in source
				where x.RelationshipTypeId.HasValue
				select x.RelationshipTypeId.Value;
			IEnumerable<IGrouping<int?, TermRelationshipTypeObject>> source2 = from x in TermRelationshipTypes
				group x by x.TypeId;
			bool flag = source.Count() < source2.Count() && currentlyAddedRelationshipTypes.Count() < source2.Count();
			if (e.Column.FieldName == "RelationshipType" && TermRelationshipTypes != null)
			{
				IEnumerable<TermRelationshipTypeObject> datasource = TermRelationshipTypes.Where((TermRelationshipTypeObject x) => x.TypeId == termObject.RelationshipTypeId || !currentlyAddedRelationshipTypes.Any((int y) => y == x.TypeId));
				RepositoryItemLookUpEdit repositoryItemLookUpEdit = CreateRelationshipTypeRepositoryItemLookUpEdit(termsGridView, datasource);
				repositoryItemLookUpEdit.Validating += Edit_RelationshipTypeRepositoryItemLookUpEditValidating;
				repositoryItemLookUpEdit.Closed += RelationshipTypeRepositoryItemLookUpEdit_Closed;
				e.RepositoryItem = repositoryItemLookUpEdit;
			}
			else if (e.Column.Name == "addNewRelationshipGridColumn")
			{
				if (termObject.IsSelected && flag)
				{
					e.RepositoryItem = addNewRelationshipRepositoryItemHyperLinkEdit;
				}
				else
				{
					e.RepositoryItem = emptyRepositoryItemButtonEdit;
				}
			}
		}
	}

	private void TermsGridView_CustomRowCellEditForAutoFilterRow(object sender, CustomRowCellEditEventArgs e)
	{
		if (e.Column.FieldName == "TypeTitle")
		{
			IEnumerable<TermObjectWithRelationshipExtended> datasource = from x in termObjectsWithRelationship
				group x by x.TypeId into x
				select x.FirstOrDefault();
			e.RepositoryItem = CreateTermTypeRepositoryItemLookUpEdit(datasource);
		}
		else if (e.Column.FieldName == "RelationshipType" && TermRelationshipTypes != null)
		{
			List<TermRelationshipTypeObject> datasource2 = TermRelationshipTypes.Where((TermRelationshipTypeObject x) => termObjectsWithRelationship.Any((TermObjectWithRelationshipExtended y) => x.TypeId == y.RelationshipTypeId && x.IsReverse == y.RelationshipType.IsReverse)).ToList();
			e.RepositoryItem = CreateRelationshipTypeRepositoryItemLookUpEdit(termsGridView, datasource2);
		}
		else if (e.Column.Name == "addNewRelationshipGridColumn")
		{
			e.RepositoryItem = emptyRepositoryItemButtonEdit;
		}
	}

	private void TermsGridView_MouseDown(object sender, MouseEventArgs e)
	{
		GridView gridView = sender as GridView;
		gridView.CloseEditor();
		GridHitInfo gridHitInfo = gridView?.CalcHitInfo(e.Location);
		if (gridHitInfo != null && gridHitInfo.InRowCell && gridHitInfo.Column.FieldName == "AddNewRelationshipText")
		{
			gridView.FocusedColumn = gridHitInfo.Column;
			gridView.FocusedRowHandle = gridHitInfo.RowHandle;
			gridView.ShowEditor();
		}
		else if (gridHitInfo != null && gridHitInfo.InRowCell && gridHitInfo.Column.FieldName == "IsSelected")
		{
			gridView.FocusedColumn = gridHitInfo.Column;
			gridView.FocusedRowHandle = gridHitInfo.RowHandle;
			gridView.ShowEditor();
		}
	}

	private void TermsGridView_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return && sender is GridView gridView && GetColumnInfo(gridView, gridView.FocusedRowHandle, gridView.FocusedColumn)?.Editor is RepositoryItemHyperLinkEdit)
		{
			if (!gridView.IsEditing)
			{
				gridView.ShowEditorByKey(e);
			}
			if (gridView.ActiveEditor is HyperLinkEdit)
			{
				AddNewRelationshipRepositoryItemHyperLinkEdit_Click(sender, new EventArgs());
				e.SuppressKeyPress = true;
				e.Handled = true;
			}
		}
	}

	private void AddNewRelationshipRepositoryItemHyperLinkEdit_Click(object sender, EventArgs e)
	{
		TermObjectWithRelationshipExtended termObjectWithRelationshipExtended = termsGridView.GetFocusedRow() as TermObjectWithRelationshipExtended;
		TermObjectWithRelationshipExtended termObjectWithRelationshipExtended2 = termObjectWithRelationshipExtended.DeepCopy();
		termObjectWithRelationshipExtended2.SetAsNewRelation();
		termObjectWithRelationshipExtended2.IsModified = true;
		termObjectsWithRelationship.Insert(termObjectsWithRelationship.IndexOf(termObjectWithRelationshipExtended) + 1, termObjectWithRelationshipExtended2);
		isModified = true;
		OpenRelationshipTypeLookUp(termObjectWithRelationshipExtended2);
	}

	private void OpenRelationshipTypeLookUp(TermObjectWithRelationshipExtended rowObject)
	{
		termsGridView.CloseEditor();
		termsGridView.FocusedRowHandle = termsGridView.GetRowHandle(termObjectsWithRelationship.IndexOf(rowObject));
		termsGridView.FocusedColumn = relationshipTypeGridColumn;
		termsGridView.ShowEditor();
		(termsGridView.ActiveEditor as LookUpEdit)?.ShowPopup();
	}

	private void IsSelectedRepositoryItemCheckEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (termsGridView.FocusedRowHandle != -2147483646)
		{
			termsGridView.CloseEditor();
			OpenRelationshipTypeLookUp(termsGridView.GetFocusedRow() as TermObjectWithRelationshipExtended);
			relationshipTypeRepositoryItemLookUpEdit.Tag = "isSelectedRepositoryItemCheckEdit";
		}
	}

	private void Edit_RelationshipTypeRepositoryItemLookUpEditValidating(object sender, CancelEventArgs e)
	{
		if (relationshipTypeRepositoryItemLookUpEdit.Tag as string== "isSelectedRepositoryItemCheckEdit")
		{
			termsGridView.FocusedColumn = isSelectedGridColumn;
		}
		relationshipTypeRepositoryItemLookUpEdit.Tag = null;
	}

	private void TermsGridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		if (e.RowHandle != -2147483646)
		{
			(termsGridView.GetFocusedRow() as TermObjectWithRelationshipExtended).IsModified = true;
			isModified = true;
		}
	}

	private void RelationshipTypeRepositoryItemLookUpEdit_Closed(object sender, ClosedEventArgs e)
	{
		termsGridView.CloseEditor();
	}

	private bool IsSelectedColumn(GridColumn column)
	{
		return column.FieldName == "IsSelected";
	}

	private void SaveSimpleButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void CancelSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void AddTermRelationshipsForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (base.DialogResult == DialogResult.OK)
		{
			Save(e);
		}
		else if (isModified)
		{
			DialogResult? dialogResult = GeneralMessageBoxesHandling.Show("Relationships have been changed, would you like to save these changes?", "Relationships have been changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
			if (dialogResult == DialogResult.Yes)
			{
				base.DialogResult = DialogResult.OK;
				Save(e);
			}
			else if (dialogResult != DialogResult.No)
			{
				base.DialogResult = DialogResult.Cancel;
				e.Cancel = true;
			}
		}
	}

	private void Save(FormClosingEventArgs e)
	{
		try
		{
			foreach (TermObjectWithRelationshipExtended item in termObjectsWithRelationship.Where((TermObjectWithRelationshipExtended x) => x.IsSelected))
			{
				item.IsValidated = true;
			}
			if (termObjectsWithRelationship.Any((TermObjectWithRelationshipExtended x) => x.IsInvalid))
			{
				e.Cancel = true;
				termsGridView.Invalidate();
				GeneralMessageBoxesHandling.Show("Some of required parameters are not specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, this);
				return;
			}
			try
			{
				if (DB.BusinessGlossary.ProcessSavingTermsRelationships(termObjectsWithRelationship))
				{
					RefreshRelatedTermsAndProgress();
				}
			}
			catch (Exception exception)
			{
				RefreshRelatedTermsAndProgress();
				GeneralExceptionHandling.Handle(exception, "Error while updating term relationships:", this);
			}
		}
		catch (Exception exception2)
		{
			GeneralExceptionHandling.Handle(exception2, "Error while updating term relationships:", this);
		}
	}

	private void RefreshRelatedTermsAndProgress()
	{
		(mainControl.GetVisibleUserControl() as TermUserControl)?.RefreshRelatedTerms(forceRefresh: true);
		mainControl.RefreshObjectProgress(showWaitForm: false, mainControl.GetVisibleUserControl().ObjectId, SharedObjectTypeEnum.ObjectType.Term);
	}

	private void TermsGridView_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
	}

	private GridCellInfo GetColumnInfo(GridView view, int rowHandle, GridColumn gridColumn)
	{
		return (view.GetViewInfo() as GridViewInfo)?.GetGridCellInfo(rowHandle, gridColumn);
	}

	private RepositoryItemLookUpEdit CreateTermTypeRepositoryItemLookUpEdit(IEnumerable<TermObjectWithRelationshipExtended> datasource)
	{
		List<TermObjectWithRelationshipExtended> list = datasource.ToList();
		list.Insert(0, new TermObjectWithRelationshipExtended());
		RepositoryItemLookUpEdit obj = new RepositoryItemLookUpEdit
		{
			ValueMember = "TypeTitle",
			DisplayMember = "TypeTitle",
			DataSource = list,
			NullText = string.Empty,
			ShowFooter = false,
			ShowHeader = false,
			ShowLines = false,
			DropDownRows = ((list.Count() < 20) ? list.Count() : 20)
		};
		LookUpColumnInfo column = new LookUpColumnInfo("TypeTitle");
		obj.Columns.Add(column);
		return obj;
	}

	public static RepositoryItemLookUpEdit CreateRelationshipTypeRepositoryItemLookUpEdit(BulkCopyGridUserControl gridView, IEnumerable<TermRelationshipTypeObject> datasource)
	{
		List<TermRelationshipTypeObject> list = datasource.ToList();
		list.Insert(0, new TermRelationshipTypeObject());
		RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
		repositoryItemLookUpEdit.ValueMember = "RelationshipType";
		repositoryItemLookUpEdit.DisplayMember = "Title";
		repositoryItemLookUpEdit.DataSource = list;
		repositoryItemLookUpEdit.NullText = string.Empty;
		repositoryItemLookUpEdit.ShowFooter = false;
		repositoryItemLookUpEdit.ShowHeader = false;
		repositoryItemLookUpEdit.ShowLines = false;
		repositoryItemLookUpEdit.DropDownRows = ((list.Count() < 20) ? list.Count() : 20);
		LookUpColumnInfo column = new LookUpColumnInfo("Title");
		repositoryItemLookUpEdit.Columns.Add(column);
		repositoryItemLookUpEdit.Closed += delegate
		{
			gridView.RefreshData();
		};
		return repositoryItemLookUpEdit;
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
		Dataedo.App.Tools.DefaultBulkCopy defaultBulkCopy = new Dataedo.App.Tools.DefaultBulkCopy();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.AddTermRelationshipsForm));
		this.rootLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.termsGrid = new DevExpress.XtraGrid.GridControl();
		this.termsGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.isSelectedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.isSelectedRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.relationshipTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.relationshipTypeRepositoryItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.iconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.titleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.titleRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.typeTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.glossaryTitleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.commentsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.addNewRelationshipGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.termsToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.gridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.rootLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.termsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.gridPanelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.leftButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.saveLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cancelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.beetwenButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.bottomAboveButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.rootLayoutControl).BeginInit();
		this.rootLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.termsGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.termsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.isSelectedRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.relationshipTypeRepositoryItemLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rootLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.termsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridPanelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.leftButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.saveLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cancelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.beetwenButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomAboveButtonsEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.rootLayoutControl.AllowCustomization = false;
		this.rootLayoutControl.Controls.Add(this.cancelSimpleButton);
		this.rootLayoutControl.Controls.Add(this.saveSimpleButton);
		this.rootLayoutControl.Controls.Add(this.termsGrid);
		this.rootLayoutControl.Controls.Add(this.gridPanelUserControl);
		this.rootLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.rootLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.rootLayoutControl.Name = "rootLayoutControl";
		this.rootLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3044, 122, 518, 486);
		this.rootLayoutControl.Root = this.rootLayoutControlGroup;
		this.rootLayoutControl.Size = new System.Drawing.Size(959, 554);
		this.rootLayoutControl.TabIndex = 0;
		this.rootLayoutControl.Text = "layoutControl1";
		this.cancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelSimpleButton.Location = new System.Drawing.Point(866, 519);
		this.cancelSimpleButton.MaximumSize = new System.Drawing.Size(0, 22);
		this.cancelSimpleButton.MinimumSize = new System.Drawing.Size(0, 22);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.cancelSimpleButton.StyleController = this.rootLayoutControl;
		this.cancelSimpleButton.TabIndex = 16;
		this.cancelSimpleButton.Text = "Cancel";
		this.cancelSimpleButton.Click += new System.EventHandler(CancelSimpleButton_Click);
		this.saveSimpleButton.Location = new System.Drawing.Point(770, 519);
		this.saveSimpleButton.MaximumSize = new System.Drawing.Size(0, 22);
		this.saveSimpleButton.MinimumSize = new System.Drawing.Size(0, 22);
		this.saveSimpleButton.Name = "saveSimpleButton";
		this.saveSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.saveSimpleButton.StyleController = this.rootLayoutControl;
		this.saveSimpleButton.TabIndex = 15;
		this.saveSimpleButton.Text = "Save";
		this.saveSimpleButton.Click += new System.EventHandler(SaveSimpleButton_Click);
		this.termsGrid.AllowDrop = true;
		this.termsGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.termsGrid.Location = new System.Drawing.Point(11, 30);
		this.termsGrid.MainView = this.termsGridView;
		this.termsGrid.Margin = new System.Windows.Forms.Padding(0);
		this.termsGrid.MenuManager = this.barManager;
		this.termsGrid.Name = "termsGrid";
		this.termsGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.iconRepositoryItemPictureEdit, this.titleRepositoryItemCustomTextEdit, this.isSelectedRepositoryItemCheckEdit, this.relationshipTypeRepositoryItemLookUpEdit });
		this.termsGrid.Size = new System.Drawing.Size(937, 477);
		this.termsGrid.TabIndex = 13;
		this.termsGrid.ToolTipController = this.termsToolTipController;
		this.termsGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.termsGridView });
		this.termsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.isSelectedGridColumn, this.relationshipTypeGridColumn, this.iconGridColumn, this.titleGridColumn, this.typeTableGridColumn, this.glossaryTitleGridColumn, this.commentsGridColumn, this.createdGridColumn, this.createdByGridColumn, this.lastUpdatedGridColumn,
			this.lastUpdatedByGridColumn, this.addNewRelationshipGridColumn
		});
		defaultBulkCopy.IsCopying = false;
		this.termsGridView.Copy = defaultBulkCopy;
		this.termsGridView.GridControl = this.termsGrid;
		this.termsGridView.Name = "termsGridView";
		this.termsGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.termsGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.termsGridView.OptionsSelection.MultiSelect = true;
		this.termsGridView.OptionsView.ColumnAutoWidth = false;
		this.termsGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.termsGridView.OptionsView.ShowGroupPanel = false;
		this.termsGridView.OptionsView.ShowIndicator = false;
		this.termsGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(TermsGridView_CustomDrawCell);
		this.termsGridView.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(TermsGridView_CustomRowCellEdit);
		this.termsGridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(TermsGridView_SelectionChanged);
		this.termsGridView.ShowingEditor += new System.ComponentModel.CancelEventHandler(TermsGridView_ShowingEditor);
		this.termsGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(TermsGridView_CellValueChanging);
		this.termsGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(TermsGridView_KeyDown);
		this.termsGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(TermsGridView_MouseDown);
		this.termsGridView.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(TermsGridView_ValidatingEditor);
		this.isSelectedGridColumn.Caption = " ";
		this.isSelectedGridColumn.ColumnEdit = this.isSelectedRepositoryItemCheckEdit;
		this.isSelectedGridColumn.FieldName = "IsSelected";
		this.isSelectedGridColumn.MaxWidth = 29;
		this.isSelectedGridColumn.MinWidth = 29;
		this.isSelectedGridColumn.Name = "isSelectedGridColumn";
		this.isSelectedGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.isSelectedGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.isSelectedGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.isSelectedGridColumn.Visible = true;
		this.isSelectedGridColumn.VisibleIndex = 0;
		this.isSelectedGridColumn.Width = 29;
		this.isSelectedRepositoryItemCheckEdit.AutoHeight = false;
		this.isSelectedRepositoryItemCheckEdit.Name = "isSelectedRepositoryItemCheckEdit";
		this.isSelectedRepositoryItemCheckEdit.EditValueChanged += new System.EventHandler(IsSelectedRepositoryItemCheckEdit_EditValueChanged);
		this.relationshipTypeGridColumn.Caption = "Relationship type";
		this.relationshipTypeGridColumn.ColumnEdit = this.relationshipTypeRepositoryItemLookUpEdit;
		this.relationshipTypeGridColumn.FieldName = "RelationshipType";
		this.relationshipTypeGridColumn.Name = "relationshipTypeGridColumn";
		this.relationshipTypeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.relationshipTypeGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.relationshipTypeGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.relationshipTypeGridColumn.Tag = "FIT_WIDTH";
		this.relationshipTypeGridColumn.Visible = true;
		this.relationshipTypeGridColumn.VisibleIndex = 1;
		this.relationshipTypeGridColumn.Width = 160;
		this.relationshipTypeRepositoryItemLookUpEdit.AutoHeight = false;
		this.relationshipTypeRepositoryItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.relationshipTypeRepositoryItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Title", "Title")
		});
		this.relationshipTypeRepositoryItemLookUpEdit.DisplayMember = "Title";
		this.relationshipTypeRepositoryItemLookUpEdit.Name = "relationshipTypeRepositoryItemLookUpEdit";
		this.relationshipTypeRepositoryItemLookUpEdit.NullText = "";
		this.relationshipTypeRepositoryItemLookUpEdit.ValueMember = "RelationshipType";
		this.relationshipTypeRepositoryItemLookUpEdit.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(RelationshipTypeRepositoryItemLookUpEdit_Closed);
		this.iconGridColumn.Caption = " ";
		this.iconGridColumn.ColumnEdit = this.iconRepositoryItemPictureEdit;
		this.iconGridColumn.FieldName = "iconGridColumn";
		this.iconGridColumn.MaxWidth = 29;
		this.iconGridColumn.MinWidth = 29;
		this.iconGridColumn.Name = "iconGridColumn";
		this.iconGridColumn.OptionsColumn.AllowEdit = false;
		this.iconGridColumn.OptionsColumn.ReadOnly = true;
		this.iconGridColumn.OptionsFilter.AllowFilter = false;
		this.iconGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.iconGridColumn.Visible = true;
		this.iconGridColumn.VisibleIndex = 2;
		this.iconGridColumn.Width = 29;
		this.iconRepositoryItemPictureEdit.AllowFocused = false;
		this.iconRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.Name = "iconRepositoryItemPictureEdit";
		this.iconRepositoryItemPictureEdit.ShowMenu = false;
		this.titleGridColumn.Caption = "Title";
		this.titleGridColumn.ColumnEdit = this.titleRepositoryItemCustomTextEdit;
		this.titleGridColumn.FieldName = "Title";
		this.titleGridColumn.Name = "titleGridColumn";
		this.titleGridColumn.OptionsColumn.AllowEdit = false;
		this.titleGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.titleGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.titleGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.titleGridColumn.Tag = "FIT_WIDTH";
		this.titleGridColumn.Visible = true;
		this.titleGridColumn.VisibleIndex = 3;
		this.titleGridColumn.Width = 140;
		this.titleRepositoryItemCustomTextEdit.AutoHeight = false;
		this.titleRepositoryItemCustomTextEdit.Name = "titleRepositoryItemCustomTextEdit";
		this.typeTableGridColumn.Caption = "Type";
		this.typeTableGridColumn.FieldName = "TypeTitle";
		this.typeTableGridColumn.Name = "typeTableGridColumn";
		this.typeTableGridColumn.OptionsColumn.AllowEdit = false;
		this.typeTableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.typeTableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.typeTableGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.typeTableGridColumn.Tag = "FIT_WIDTH";
		this.typeTableGridColumn.Visible = true;
		this.typeTableGridColumn.VisibleIndex = 4;
		this.glossaryTitleGridColumn.Caption = "Glossary";
		this.glossaryTitleGridColumn.FieldName = "DatabaseTitle";
		this.glossaryTitleGridColumn.Name = "glossaryTitleGridColumn";
		this.glossaryTitleGridColumn.OptionsColumn.AllowEdit = false;
		this.glossaryTitleGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.glossaryTitleGridColumn.Tag = "FIT_WIDTH";
		this.glossaryTitleGridColumn.Visible = true;
		this.glossaryTitleGridColumn.VisibleIndex = 5;
		this.glossaryTitleGridColumn.Width = 200;
		this.commentsGridColumn.Caption = "Relationship comments";
		this.commentsGridColumn.FieldName = "RelationshipDescription";
		this.commentsGridColumn.MinWidth = 200;
		this.commentsGridColumn.Name = "commentsGridColumn";
		this.commentsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.commentsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.commentsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.commentsGridColumn.Tag = "FIT_WIDTH";
		this.commentsGridColumn.Visible = true;
		this.commentsGridColumn.VisibleIndex = 6;
		this.commentsGridColumn.Width = 400;
		this.createdGridColumn.Caption = "Created";
		this.createdGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.createdGridColumn.FieldName = "CreationDate";
		this.createdGridColumn.Name = "createdGridColumn";
		this.createdGridColumn.OptionsColumn.AllowFocus = false;
		this.createdGridColumn.Width = 120;
		this.createdByGridColumn.Caption = "Created by";
		this.createdByGridColumn.FieldName = "CreatedBy";
		this.createdByGridColumn.Name = "createdByGridColumn";
		this.createdByGridColumn.OptionsColumn.AllowFocus = false;
		this.createdByGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.createdByGridColumn.Width = 150;
		this.lastUpdatedGridColumn.Caption = "Last updated";
		this.lastUpdatedGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.lastUpdatedGridColumn.FieldName = "LastModificationDate";
		this.lastUpdatedGridColumn.Name = "lastUpdatedGridColumn";
		this.lastUpdatedGridColumn.OptionsColumn.AllowFocus = false;
		this.lastUpdatedGridColumn.Width = 100;
		this.lastUpdatedByGridColumn.Caption = "Last updated by";
		this.lastUpdatedByGridColumn.FieldName = "ModifiedBy";
		this.lastUpdatedByGridColumn.Name = "lastUpdatedByGridColumn";
		this.lastUpdatedByGridColumn.OptionsColumn.AllowFocus = false;
		this.lastUpdatedByGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.lastUpdatedByGridColumn.Width = 100;
		this.addNewRelationshipGridColumn.Caption = " ";
		this.addNewRelationshipGridColumn.FieldName = "AddNewRelationshipText";
		this.addNewRelationshipGridColumn.Name = "addNewRelationshipGridColumn";
		this.addNewRelationshipGridColumn.OptionsFilter.AllowFilter = false;
		this.addNewRelationshipGridColumn.Tag = "";
		this.addNewRelationshipGridColumn.Visible = true;
		this.addNewRelationshipGridColumn.VisibleIndex = 7;
		this.addNewRelationshipGridColumn.Width = 60;
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(959, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 554);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(959, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 554);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(959, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 554);
		this.termsToolTipController.AllowHtmlText = true;
		this.termsToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.gridPanelUserControl.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.gridPanelUserControl.GridView = this.termsGridView;
		this.gridPanelUserControl.Location = new System.Drawing.Point(11, 0);
		this.gridPanelUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.gridPanelUserControl.Name = "gridPanelUserControl";
		this.gridPanelUserControl.Size = new System.Drawing.Size(937, 30);
		this.gridPanelUserControl.TabIndex = 14;
		this.rootLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.rootLayoutControlGroup.GroupBordersVisible = false;
		this.rootLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[7] { this.termsLayoutControlItem, this.gridPanelLayoutControlItem, this.leftButtonsEmptySpaceItem, this.saveLayoutControlItem, this.cancelLayoutControlItem, this.beetwenButtonsEmptySpaceItem, this.bottomAboveButtonsEmptySpaceItem });
		this.rootLayoutControlGroup.Name = "Root";
		this.rootLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(11, 11, 0, 11);
		this.rootLayoutControlGroup.Size = new System.Drawing.Size(959, 554);
		this.rootLayoutControlGroup.TextVisible = false;
		this.termsLayoutControlItem.Control = this.termsGrid;
		this.termsLayoutControlItem.Location = new System.Drawing.Point(0, 30);
		this.termsLayoutControlItem.MinSize = new System.Drawing.Size(104, 24);
		this.termsLayoutControlItem.Name = "termsLayoutControlItem";
		this.termsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.termsLayoutControlItem.Size = new System.Drawing.Size(937, 477);
		this.termsLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.termsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.termsLayoutControlItem.TextVisible = false;
		this.gridPanelLayoutControlItem.Control = this.gridPanelUserControl;
		this.gridPanelLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.gridPanelLayoutControlItem.MaxSize = new System.Drawing.Size(0, 30);
		this.gridPanelLayoutControlItem.MinSize = new System.Drawing.Size(100, 30);
		this.gridPanelLayoutControlItem.Name = "gridPanelLayoutControlItem";
		this.gridPanelLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.gridPanelLayoutControlItem.Size = new System.Drawing.Size(937, 30);
		this.gridPanelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.gridPanelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.gridPanelLayoutControlItem.TextVisible = false;
		this.leftButtonsEmptySpaceItem.AllowHotTrack = false;
		this.leftButtonsEmptySpaceItem.CustomizationFormText = "emptySpaceItem1";
		this.leftButtonsEmptySpaceItem.Location = new System.Drawing.Point(0, 517);
		this.leftButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 26);
		this.leftButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(1, 26);
		this.leftButtonsEmptySpaceItem.Name = "leftButtonsEmptySpaceItem";
		this.leftButtonsEmptySpaceItem.Size = new System.Drawing.Size(757, 26);
		this.leftButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.leftButtonsEmptySpaceItem.Text = "emptySpaceItem1";
		this.leftButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveLayoutControlItem.Control = this.saveSimpleButton;
		this.saveLayoutControlItem.Location = new System.Drawing.Point(757, 517);
		this.saveLayoutControlItem.MaxSize = new System.Drawing.Size(84, 26);
		this.saveLayoutControlItem.MinSize = new System.Drawing.Size(84, 26);
		this.saveLayoutControlItem.Name = "saveLayoutControlItem";
		this.saveLayoutControlItem.Size = new System.Drawing.Size(84, 26);
		this.saveLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.saveLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.saveLayoutControlItem.TextVisible = false;
		this.cancelLayoutControlItem.Control = this.cancelSimpleButton;
		this.cancelLayoutControlItem.Location = new System.Drawing.Point(853, 517);
		this.cancelLayoutControlItem.MaxSize = new System.Drawing.Size(84, 26);
		this.cancelLayoutControlItem.MinSize = new System.Drawing.Size(84, 26);
		this.cancelLayoutControlItem.Name = "cancelLayoutControlItem2";
		this.cancelLayoutControlItem.Size = new System.Drawing.Size(84, 26);
		this.cancelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cancelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.cancelLayoutControlItem.TextVisible = false;
		this.beetwenButtonsEmptySpaceItem.AllowHotTrack = false;
		this.beetwenButtonsEmptySpaceItem.Location = new System.Drawing.Point(841, 517);
		this.beetwenButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(12, 26);
		this.beetwenButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(12, 26);
		this.beetwenButtonsEmptySpaceItem.Name = "beetwenButtonsEmptySpaceItem";
		this.beetwenButtonsEmptySpaceItem.Size = new System.Drawing.Size(12, 26);
		this.beetwenButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.beetwenButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.bottomAboveButtonsEmptySpaceItem.AllowHotTrack = false;
		this.bottomAboveButtonsEmptySpaceItem.Location = new System.Drawing.Point(0, 507);
		this.bottomAboveButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 10);
		this.bottomAboveButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(104, 10);
		this.bottomAboveButtonsEmptySpaceItem.Name = "bottomAboveButtonsEmptySpaceItem";
		this.bottomAboveButtonsEmptySpaceItem.Size = new System.Drawing.Size(937, 10);
		this.bottomAboveButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.bottomAboveButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(959, 554);
		base.Controls.Add(this.rootLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("AddTermRelationshipsForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "AddTermRelationshipsForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Add relationships";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(AddTermRelationshipsForm_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.rootLayoutControl).EndInit();
		this.rootLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.termsGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.termsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.isSelectedRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.relationshipTypeRepositoryItemLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rootLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.termsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridPanelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.leftButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.saveLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cancelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.beetwenButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomAboveButtonsEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
