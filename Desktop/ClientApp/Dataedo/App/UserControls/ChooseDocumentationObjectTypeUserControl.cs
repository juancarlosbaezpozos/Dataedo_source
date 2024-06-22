using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.Import.CloudStorage;
using Dataedo.App.Import.DataLake;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.UserControls.Base;
using Dataedo.CustomControls;
using Dataedo.Model.Data.Documentations;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.Utils.Extensions;
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

namespace Dataedo.App.UserControls;

public class ChooseDocumentationObjectTypeUserControl : BaseUserControl
{
	private DocumentationObjectTypesContainer documentationObjectTypesContainer;

	public EventHandler SelectionChanged;

	public new EventHandler DoubleClick;

	private IContainer components;

	private NonCustomizableLayoutControl objectTypesLayoutControl;

	private GridControl objectTypeGrid;

	private GridColumn objectTypeSelectionGridColumn;

	private RepositoryItemCheckEdit objectTypeRepositoryItemCheckEdit;

	private GridColumn objectTypeGridColumn;

	private GridColumn documentationTitleGridColumn;

	private LayoutControlGroup layoutControlGroup3;

	private LayoutControlItem layoutControlItem2;

	private CustomGridUserControl objectTypesGridView;

	private CustomGridUserControl documentationsGridView;

	private GridColumn documentationImageGridColumn;

	public SharedObjectTypeEnum.ObjectType SelectedObjectType { get; set; }

	public int? SelectedDatabaseId { get; set; }

	public bool IsNewDatabase { get; private set; }

	public DataLakeTypeEnum.DataLakeType? SelectedDataLakeType { get; private set; }

	public CloudStorageTypeEnum.CloudStorageType SelectedCloudStorageType { get; private set; }

	public ChooseDocumentationObjectTypeUserControl()
	{
		InitializeComponent();
	}

	public void SetParameters(DataLakeTypeEnum.DataLakeType dataLakeType, int? databaseId = null, SharedObjectTypeEnum.ObjectType? objectType = null)
	{
		SelectedDataLakeType = dataLakeType;
		if (documentationObjectTypesContainer?.Data != null)
		{
			DocumentationObjectTypesContainer obj = documentationObjectTypesContainer;
			if (obj == null || obj.Data?.Count != 0)
			{
				goto IL_0081;
			}
		}
		LoadDocumentations(objectTypeGrid, dataLakeType);
		goto IL_0081;
		IL_0328:
		int num;
		if (dataLakeType == DataLakeTypeEnum.DataLakeType.JSON)
		{
			documentationsGridView.SetMasterRowExpanded(num, expand: true);
		}
		objectTypesGridView.EndUpdate();
		documentationsGridView.EndUpdate();
		goto IL_034f;
		IL_034f:
		if (dataLakeType != 0)
		{
			documentationsGridView.CollapseAllDetails();
		}
		return;
		IL_0081:
		if (dataLakeType == DataLakeTypeEnum.DataLakeType.JSON)
		{
			documentationsGridView.OptionsView.ShowDetailButtons = true;
			documentationsGridView.OptionsSelection.EnableAppearanceFocusedRow = false;
			SelectionChanged?.Invoke(null, new BoolEventArgs(value: false));
			objectTypesGridView.DoubleClick -= ObjectTypesGridView_DoubleClick;
			objectTypesGridView.DoubleClick += ObjectTypesGridView_DoubleClick;
			objectTypesGridView.BeginUpdate();
			documentationObjectTypesContainer.Data.Where((DocumentationWithObjectTypes x) => x.ObjectTypes != null).ForEach(delegate(DocumentationWithObjectTypes x)
			{
				x.ObjectTypes.ForEach(delegate(ObjectTypeSelection t)
				{
					t.IsSelected = false;
				});
			});
			objectTypesGridView.EndUpdate();
		}
		else
		{
			documentationsGridView.OptionsView.ShowDetailButtons = false;
			documentationsGridView.OptionsSelection.EnableAppearanceFocusedRow = true;
			if (documentationsGridView.DataRowCount > 0)
			{
				documentationsGridView.MoveFirst();
				SelectionChanged?.Invoke(null, new BoolEventArgs(value: true));
			}
			else
			{
				SelectionChanged?.Invoke(null, new BoolEventArgs(value: false));
			}
		}
		if (databaseId.HasValue)
		{
			DocumentationWithObjectTypes documentationWithObjectTypes = documentationObjectTypesContainer.Data.FirstOrDefault((DocumentationWithObjectTypes x) => x.Documentation.Id == databaseId);
			if (documentationWithObjectTypes == null)
			{
				return;
			}
			objectTypesGridView.BeginUpdate();
			documentationsGridView.BeginUpdate();
			num = documentationObjectTypesContainer.Data.IndexOf(documentationWithObjectTypes);
			documentationsGridView.FocusedRowHandle = num;
			if (objectType.HasValue && documentationWithObjectTypes.ObjectTypes.Any())
			{
				if (objectType == SharedObjectTypeEnum.ObjectType.Table)
				{
					ObjectTypeSelection objectTypeSelection = documentationWithObjectTypes.ObjectTypes.FirstOrDefault((ObjectTypeSelection x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Table);
					if (objectTypeSelection != null)
					{
						objectTypeSelection.IsSelected = true;
						SelectionChanged?.Invoke(null, new BoolEventArgs(value: true));
						goto IL_0328;
					}
				}
				if (objectType == SharedObjectTypeEnum.ObjectType.Structure)
				{
					ObjectTypeSelection objectTypeSelection2 = documentationWithObjectTypes.ObjectTypes.FirstOrDefault((ObjectTypeSelection x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Structure);
					if (objectTypeSelection2 != null)
					{
						objectTypeSelection2.IsSelected = true;
						SelectionChanged?.Invoke(null, new BoolEventArgs(value: true));
					}
				}
			}
			goto IL_0328;
		}
		goto IL_034f;
	}

	public void SetParameters(CloudStorageTypeEnum.CloudStorageType cloudStorageType, int? databaseId = null, SharedObjectTypeEnum.ObjectType? objectType = null)
	{
		SelectedDataLakeType = null;
		SelectedCloudStorageType = cloudStorageType;
		if (documentationObjectTypesContainer?.Data != null)
		{
			DocumentationObjectTypesContainer obj = documentationObjectTypesContainer;
			if (obj == null || obj.Data?.Count != 0)
			{
				goto IL_008f;
			}
		}
		LoadDocumentations(objectTypeGrid);
		goto IL_008f;
		IL_025d:
		objectTypesGridView.EndUpdate();
		documentationsGridView.EndUpdate();
		return;
		IL_008f:
		documentationsGridView.OptionsView.ShowDetailButtons = false;
		documentationsGridView.OptionsSelection.EnableAppearanceFocusedRow = true;
		if (documentationsGridView.DataRowCount > 0)
		{
			documentationsGridView.MoveFirst();
			SelectionChanged?.Invoke(null, new BoolEventArgs(value: true));
		}
		else
		{
			SelectionChanged?.Invoke(null, new BoolEventArgs(value: false));
		}
		if (!databaseId.HasValue)
		{
			return;
		}
		DocumentationWithObjectTypes documentationWithObjectTypes = documentationObjectTypesContainer.Data.FirstOrDefault((DocumentationWithObjectTypes x) => x.Documentation.Id == databaseId);
		if (documentationWithObjectTypes == null)
		{
			return;
		}
		objectTypesGridView.BeginUpdate();
		documentationsGridView.BeginUpdate();
		int focusedRowHandle = documentationObjectTypesContainer.Data.IndexOf(documentationWithObjectTypes);
		documentationsGridView.FocusedRowHandle = focusedRowHandle;
		if (objectType.HasValue && documentationWithObjectTypes.ObjectTypes.Any())
		{
			if (objectType == SharedObjectTypeEnum.ObjectType.Table)
			{
				ObjectTypeSelection objectTypeSelection = documentationWithObjectTypes.ObjectTypes.FirstOrDefault((ObjectTypeSelection x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Table);
				if (objectTypeSelection != null)
				{
					objectTypeSelection.IsSelected = true;
					SelectionChanged?.Invoke(null, new BoolEventArgs(value: true));
					goto IL_025d;
				}
			}
			if (objectType == SharedObjectTypeEnum.ObjectType.Structure)
			{
				ObjectTypeSelection objectTypeSelection2 = documentationWithObjectTypes.ObjectTypes.FirstOrDefault((ObjectTypeSelection x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Structure);
				if (objectTypeSelection2 != null)
				{
					objectTypeSelection2.IsSelected = true;
					SelectionChanged?.Invoke(null, new BoolEventArgs(value: true));
				}
			}
		}
		goto IL_025d;
	}

	private void LoadDocumentations(GridControl objectTypesGrid, DataLakeTypeEnum.DataLakeType? dataLakeType = null)
	{
		documentationObjectTypesContainer = new DocumentationObjectTypesContainer();
		List<DatabaseRow> list = (from x in DB.Database.GetData()
			select new DatabaseRow(x) into x
			where !x.IsWelcomeDocumentation && x.IsValidDatabase
			orderby x.Title
			select x).ToList();
		DocumentationWithObjectTypes item = new DocumentationWithObjectTypes
		{
			Documentation = new DatabaseRow
			{
				Title = "< Add new database ...>"
			},
			DocumentationImage = new Bitmap(16, 16),
			IsForAddingNewDatabase = true,
			ObjectTypes = new List<ObjectTypeSelection>
			{
				new ObjectTypeSelection
				{
					ObjectType = SharedObjectTypeEnum.ObjectType.Structure
				},
				new ObjectTypeSelection
				{
					ObjectType = SharedObjectTypeEnum.ObjectType.Table
				}
			}
		};
		documentationObjectTypesContainer.Data.Add(item);
		foreach (DatabaseRow item3 in list)
		{
			DocumentationWithObjectTypes item2 = new DocumentationWithObjectTypes
			{
				Documentation = item3,
				DocumentationImage = IconsSupport.GetDatabaseIconByName16(item3.Type, item3.ObjectTypeValue),
				ObjectTypes = new List<ObjectTypeSelection>
				{
					new ObjectTypeSelection
					{
						ObjectType = SharedObjectTypeEnum.ObjectType.Structure
					},
					new ObjectTypeSelection
					{
						ObjectType = SharedObjectTypeEnum.ObjectType.Table
					}
				}
			};
			documentationObjectTypesContainer.Data.Add(item2);
		}
		objectTypesGrid.DataSource = documentationObjectTypesContainer.Data;
	}

	private void objectTypesGridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		BeginInvoke((Action)delegate
		{
			((GridView)sender).CloseEditor();
		});
	}

	private void objectTypesGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (sender is GridView gridView && gridView.GetRow(e.RowHandle) is ObjectTypeSelection objectType)
		{
			SelectObjectTypeRow(objectType, (bool)e.Value);
		}
	}

	private void SelectObjectTypeRow(ObjectTypeSelection objectType, bool isSelected)
	{
		objectTypesGridView.BeginUpdate();
		documentationsGridView.BeginUpdate();
		documentationObjectTypesContainer.Data.Where((DocumentationWithObjectTypes x) => x.ObjectTypes != null).ForEach(delegate(DocumentationWithObjectTypes x)
		{
			x.ObjectTypes.ForEach(delegate(ObjectTypeSelection t)
			{
				t.IsSelected = false;
			});
		});
		if (documentationsGridView.GetFocusedRow() is DocumentationWithObjectTypes documentationWithObjectTypes)
		{
			SelectedObjectType = objectType.ObjectType;
			SelectedDatabaseId = documentationWithObjectTypes?.Documentation.Id;
			IsNewDatabase = documentationWithObjectTypes.IsForAddingNewDatabase;
			objectType.IsSelected = isSelected;
		}
		documentationsGridView.EndUpdate();
		objectTypesGridView.EndUpdate();
		SelectionChanged?.Invoke(null, new BoolEventArgs(isSelected));
	}

	private void objectTypeRepositoryItemCheckEdit_Click(object sender, EventArgs e)
	{
		ChangeCheckEditState(sender);
	}

	private void ChangeCheckEditState(object sender)
	{
		if (sender is CheckEdit checkEdit)
		{
			if (checkEdit.CheckState == CheckState.Unchecked)
			{
				checkEdit.CheckState = CheckState.Checked;
			}
			else
			{
				checkEdit.CheckState = CheckState.Unchecked;
			}
		}
	}

	private void DocumentationsGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		if (SelectedDataLakeType == DataLakeTypeEnum.DataLakeType.JSON)
		{
			return;
		}
		if (!(documentationsGridView.GetRow(e.FocusedRowHandle) is DocumentationWithObjectTypes documentationWithObjectTypes))
		{
			SelectedDatabaseId = null;
			SelectionChanged?.Invoke(null, new BoolEventArgs(value: false));
			return;
		}
		SelectedObjectType = SharedObjectTypeEnum.ObjectType.Structure;
		if (documentationWithObjectTypes.IsForAddingNewDatabase)
		{
			IsNewDatabase = true;
			SelectedDatabaseId = null;
		}
		else
		{
			IsNewDatabase = false;
			SelectedDatabaseId = documentationWithObjectTypes?.Documentation.Id;
		}
		SelectionChanged?.Invoke(null, new BoolEventArgs(value: true));
	}

	private void DocumentationsGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (SelectedDataLakeType != DataLakeTypeEnum.DataLakeType.JSON && e.RowHandle == 0 && e.Cell is GridCellInfo gridCellInfo)
		{
			gridCellInfo.CellButtonRect = Rectangle.Empty;
		}
	}

	private void ObjectTypesGridView_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (sender is GridView gridView && e is DXMouseEventArgs dXMouseEventArgs)
			{
				GridHitInfo gridHitInfo = gridView.CalcHitInfo(dXMouseEventArgs.Location);
				if (gridHitInfo != null && gridHitInfo.InRow && gridView.GetRow(gridHitInfo.RowHandle) is ObjectTypeSelection objectType)
				{
					SelectObjectTypeRow(objectType, isSelected: true);
					DoubleClick?.Invoke(null, null);
				}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, base.ParentForm);
		}
	}

	private void DocumentationsGridView_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (!(e is DXMouseEventArgs dXMouseEventArgs))
			{
				return;
			}
			GridHitInfo gridHitInfo = documentationsGridView.CalcHitInfo(dXMouseEventArgs.Location);
			if (gridHitInfo != null && gridHitInfo.InRow)
			{
				if (SelectedDataLakeType != DataLakeTypeEnum.DataLakeType.JSON)
				{
					DoubleClick?.Invoke(null, null);
				}
				else if (documentationsGridView.GetMasterRowExpanded(gridHitInfo.RowHandle))
				{
					documentationsGridView.CollapseMasterRow(gridHitInfo.RowHandle);
				}
				else
				{
					documentationsGridView.ExpandMasterRow(gridHitInfo.RowHandle);
				}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, base.ParentForm);
		}
	}

	private void ObjectTypesGridView_KeyDown(object sender, KeyEventArgs e)
	{
		if (!(sender is GridView gridView) || gridView.Name != objectTypesGridView.Name)
		{
			return;
		}
		switch (e.KeyCode)
		{
		case Keys.Tab:
		case Keys.Up:
		case Keys.Down:
			ChangeObjectTypesGridViewFocusedRow(gridView, e.KeyCode == Keys.Up || e.Modifiers == Keys.Shift, gridView.IsLastRow, gridView.IsFirstRow);
			e.Handled = true;
			break;
		case Keys.Space:
			if (gridView.GetRow(gridView.FocusedRowHandle) is ObjectTypeSelection objectType)
			{
				SelectObjectTypeRow(objectType, isSelected: true);
				e.Handled = true;
			}
			break;
		}
	}

	private void ChangeObjectTypesGridViewFocusedRow(GridView gridView, bool moveUp, bool isLast = false, bool isFirst = false)
	{
		if (isLast && !moveUp)
		{
			if (gridView.ParentView is GridView gridView2)
			{
				gridView2.BeginUpdate();
				int focusedRowHandle = gridView2.FocusedRowHandle;
				gridView2.CollapseMasterRow(focusedRowHandle);
				gridView2.MoveNext();
				gridView2.ExpandMasterRow(focusedRowHandle);
				gridView2.EndUpdate();
			}
		}
		else if (isFirst && moveUp)
		{
			if (gridView.ParentView is GridView gridView3)
			{
				gridView3.BeginUpdate();
				gridView3.CollapseMasterRow(gridView3.FocusedRowHandle);
				gridView3.ExpandMasterRow(gridView3.FocusedRowHandle);
				gridView3.EndUpdate();
			}
		}
		else if (moveUp)
		{
			gridView.MovePrev();
		}
		else
		{
			gridView.MoveNext();
		}
	}

	private void DocumentationsGridView_KeyDown(object sender, KeyEventArgs e)
	{
		switch (e.KeyCode)
		{
		case Keys.Tab:
			if (e.Modifiers == Keys.Shift)
			{
				documentationsGridView.MovePrev();
			}
			else
			{
				documentationsGridView.MoveNext();
			}
			e.Handled = true;
			break;
		case Keys.Right:
			if (SelectedDataLakeType == DataLakeTypeEnum.DataLakeType.JSON)
			{
				documentationsGridView.ExpandMasterRow(documentationsGridView.FocusedRowHandle);
			}
			break;
		case Keys.Left:
			if (SelectedDataLakeType == DataLakeTypeEnum.DataLakeType.JSON)
			{
				documentationsGridView.CollapseMasterRow(documentationsGridView.FocusedRowHandle);
			}
			break;
		}
	}

	public bool IsObjectTypeSelected()
	{
		return documentationObjectTypesContainer.Data.Any((DocumentationWithObjectTypes x) => x.ObjectTypes.Any((ObjectTypeSelection o) => o.IsSelected));
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
		DevExpress.XtraGrid.GridLevelNode gridLevelNode = new DevExpress.XtraGrid.GridLevelNode();
		this.objectTypesGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.objectTypeSelectionGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.objectTypeRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.objectTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.objectTypeGrid = new DevExpress.XtraGrid.GridControl();
		this.documentationsGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.documentationImageGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.documentationTitleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.objectTypesLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.objectTypesGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectTypeRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectTypeGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.documentationsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectTypesLayoutControl).BeginInit();
		this.objectTypesLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		base.SuspendLayout();
		this.objectTypesGridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.objectTypesGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.objectTypeSelectionGridColumn, this.objectTypeGridColumn });
		this.objectTypesGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
		this.objectTypesGridView.GridControl = this.objectTypeGrid;
		this.objectTypesGridView.Name = "objectTypesGridView";
		this.objectTypesGridView.OptionsCustomization.AllowColumnMoving = false;
		this.objectTypesGridView.OptionsCustomization.AllowColumnResizing = false;
		this.objectTypesGridView.OptionsDetail.ShowDetailTabs = false;
		this.objectTypesGridView.OptionsScrollAnnotations.ShowSelectedRows = DevExpress.Utils.DefaultBoolean.False;
		this.objectTypesGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.objectTypesGridView.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.objectTypesGridView.OptionsView.ShowColumnHeaders = false;
		this.objectTypesGridView.OptionsView.ShowGroupPanel = false;
		this.objectTypesGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.objectTypesGridView.OptionsView.ShowIndicator = false;
		this.objectTypesGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.objectTypesGridView.RowHighlightingIsEnabled = false;
		this.objectTypesGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(objectTypesGridView_CellValueChanged);
		this.objectTypesGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(objectTypesGridView_CellValueChanging);
		this.objectTypesGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(ObjectTypesGridView_KeyDown);
		this.objectTypeSelectionGridColumn.ColumnEdit = this.objectTypeRepositoryItemCheckEdit;
		this.objectTypeSelectionGridColumn.FieldName = "IsSelected";
		this.objectTypeSelectionGridColumn.MaxWidth = 22;
		this.objectTypeSelectionGridColumn.MinWidth = 22;
		this.objectTypeSelectionGridColumn.Name = "objectTypeSelectionGridColumn";
		this.objectTypeSelectionGridColumn.OptionsColumn.AllowSize = false;
		this.objectTypeSelectionGridColumn.OptionsColumn.ReadOnly = true;
		this.objectTypeSelectionGridColumn.OptionsColumn.ShowCaption = false;
		this.objectTypeSelectionGridColumn.Visible = true;
		this.objectTypeSelectionGridColumn.VisibleIndex = 0;
		this.objectTypeSelectionGridColumn.Width = 22;
		this.objectTypeRepositoryItemCheckEdit.AutoHeight = false;
		this.objectTypeRepositoryItemCheckEdit.Caption = "Check";
		this.objectTypeRepositoryItemCheckEdit.CheckBoxOptions.Style = DevExpress.XtraEditors.Controls.CheckBoxStyle.Radio;
		this.objectTypeRepositoryItemCheckEdit.Name = "objectTypeRepositoryItemCheckEdit";
		this.objectTypeRepositoryItemCheckEdit.Click += new System.EventHandler(objectTypeRepositoryItemCheckEdit_Click);
		this.objectTypeGridColumn.Caption = "ObjectType";
		this.objectTypeGridColumn.FieldName = "ObjectType";
		this.objectTypeGridColumn.FieldNameSortGroup = "ObjectType";
		this.objectTypeGridColumn.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.objectTypeGridColumn.Name = "objectTypeGridColumn";
		this.objectTypeGridColumn.OptionsColumn.AllowEdit = false;
		this.objectTypeGridColumn.OptionsColumn.AllowFocus = false;
		this.objectTypeGridColumn.OptionsColumn.ReadOnly = true;
		this.objectTypeGridColumn.OptionsColumn.ShowCaption = false;
		this.objectTypeGridColumn.Visible = true;
		this.objectTypeGridColumn.VisibleIndex = 1;
		this.objectTypeGrid.Cursor = System.Windows.Forms.Cursors.Default;
		gridLevelNode.LevelTemplate = this.objectTypesGridView;
		gridLevelNode.RelationName = "ObjectTypes";
		this.objectTypeGrid.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[1] { gridLevelNode });
		this.objectTypeGrid.Location = new System.Drawing.Point(0, 0);
		this.objectTypeGrid.MainView = this.documentationsGridView;
		this.objectTypeGrid.Name = "objectTypeGrid";
		this.objectTypeGrid.Padding = new System.Windows.Forms.Padding(1);
		this.objectTypeGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.objectTypeRepositoryItemCheckEdit });
		this.objectTypeGrid.Size = new System.Drawing.Size(606, 422);
		this.objectTypeGrid.TabIndex = 0;
		this.objectTypeGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.documentationsGridView, this.objectTypesGridView });
		this.documentationsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.documentationImageGridColumn, this.documentationTitleGridColumn });
		this.documentationsGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
		this.documentationsGridView.GridControl = this.objectTypeGrid;
		this.documentationsGridView.LevelIndent = 36;
		this.documentationsGridView.Name = "documentationsGridView";
		this.documentationsGridView.OptionsCustomization.AllowColumnMoving = false;
		this.documentationsGridView.OptionsCustomization.AllowColumnResizing = false;
		this.documentationsGridView.OptionsDetail.DetailMode = DevExpress.XtraGrid.Views.Grid.DetailMode.Embedded;
		this.documentationsGridView.OptionsDetail.ShowDetailTabs = false;
		this.documentationsGridView.OptionsDetail.ShowEmbeddedDetailIndent = DevExpress.Utils.DefaultBoolean.False;
		this.documentationsGridView.OptionsScrollAnnotations.ShowSelectedRows = DevExpress.Utils.DefaultBoolean.False;
		this.documentationsGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.documentationsGridView.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.documentationsGridView.OptionsView.ShowColumnHeaders = false;
		this.documentationsGridView.OptionsView.ShowGroupPanel = false;
		this.documentationsGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.documentationsGridView.OptionsView.ShowIndicator = false;
		this.documentationsGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.documentationsGridView.RowHighlightingIsEnabled = false;
		this.documentationsGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(DocumentationsGridView_CustomDrawCell);
		this.documentationsGridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(DocumentationsGridView_FocusedRowChanged);
		this.documentationsGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(DocumentationsGridView_KeyDown);
		this.documentationsGridView.DoubleClick += new System.EventHandler(DocumentationsGridView_DoubleClick);
		this.documentationImageGridColumn.FieldName = "DocumentationImage";
		this.documentationImageGridColumn.MaxWidth = 20;
		this.documentationImageGridColumn.Name = "documentationImageGridColumn";
		this.documentationImageGridColumn.OptionsColumn.AllowEdit = false;
		this.documentationImageGridColumn.OptionsColumn.AllowFocus = false;
		this.documentationImageGridColumn.OptionsColumn.ReadOnly = true;
		this.documentationImageGridColumn.Visible = true;
		this.documentationImageGridColumn.VisibleIndex = 0;
		this.documentationImageGridColumn.Width = 20;
		this.documentationTitleGridColumn.Caption = "Documentation";
		this.documentationTitleGridColumn.FieldName = "Documentation.Title";
		this.documentationTitleGridColumn.Name = "documentationTitleGridColumn";
		this.documentationTitleGridColumn.OptionsColumn.AllowEdit = false;
		this.documentationTitleGridColumn.OptionsColumn.AllowFocus = false;
		this.documentationTitleGridColumn.OptionsColumn.ReadOnly = true;
		this.documentationTitleGridColumn.Visible = true;
		this.documentationTitleGridColumn.VisibleIndex = 1;
		this.documentationTitleGridColumn.Width = 584;
		this.objectTypesLayoutControl.AllowCustomization = false;
		this.objectTypesLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.objectTypesLayoutControl.Controls.Add(this.objectTypeGrid);
		this.objectTypesLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.objectTypesLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.objectTypesLayoutControl.Name = "objectTypesLayoutControl";
		this.objectTypesLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(748, 241, 808, 578);
		this.objectTypesLayoutControl.OptionsView.UseDefaultDragAndDropRendering = false;
		this.objectTypesLayoutControl.Padding = new System.Windows.Forms.Padding(1);
		this.objectTypesLayoutControl.Root = this.layoutControlGroup3;
		this.objectTypesLayoutControl.Size = new System.Drawing.Size(606, 422);
		this.objectTypesLayoutControl.TabIndex = 1;
		this.objectTypesLayoutControl.Text = "layoutControl2";
		this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup3.GroupBordersVisible = false;
		this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem2 });
		this.layoutControlGroup3.Name = "Root";
		this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup3.Size = new System.Drawing.Size(606, 422);
		this.layoutControlGroup3.TextVisible = false;
		this.layoutControlItem2.Control = this.objectTypeGrid;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem2.Size = new System.Drawing.Size(606, 422);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.objectTypesLayoutControl);
		base.Name = "ChooseDocumentationObjectTypeUserControl";
		base.Size = new System.Drawing.Size(606, 422);
		((System.ComponentModel.ISupportInitialize)this.objectTypesGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectTypeRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectTypeGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.documentationsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectTypesLayoutControl).EndInit();
		this.objectTypesLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		base.ResumeLayout(false);
	}
}
