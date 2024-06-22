using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes.Documentation;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Documentation;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Documentations;
using Dataedo.Model.Data.Modules;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
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
using DevExpress.XtraWizard;

namespace Dataedo.App.UserControls;

public class ChooseModulesUserControl : BaseUserControl
{
	public DevExpress.XtraWizard.WizardButton Button;

	private Action<bool, Button> editButton;

	private int? currentDatabaseId;

	private MetadataEditorUserControl metadataEditorUserControl;

	private DocumentationModulesContainer documentationsModulesData;

	private IContainer components;

	private NonCustomizableLayoutControl modulesLayoutControl;

	private HyperLinkEdit checkNoneHyperLinkEdit;

	private HyperLinkEdit checkAllHyperLinkEdit;

	private GridControl modulesGrid;

	private GridColumn moduleSelectionGridColumn;

	private RepositoryItemCheckEdit moduleRepositoryItemCheckEdit;

	private GridColumn moduleTitleGridColumn;

	private GridColumn documentationSelectionGridColumn;

	private GridColumn documentationTitleGridColumn;

	private LayoutControlGroup layoutControlGroup3;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private LayoutControlItem layoutControlItem6;

	private EmptySpaceItem emptySpaceItem4;

	private CustomGridUserControl modulesGridView;

	private CustomGridUserControl documentationsGridView;

	public DocumentationModulesContainer DocumentationsModulesData => documentationsModulesData;

	public bool AreManyDocumentationsSelected
	{
		get
		{
			DocumentationModulesContainer documentationModulesContainer = DocumentationsModulesData;
			if (documentationModulesContainer == null)
			{
				return false;
			}
			return documentationModulesContainer.SelectedDocumentationsCount > 1;
		}
	}

	public int? DocumentationId => DocumentationsModulesData?.SelectedDocumentations?.FirstOrDefault()?.Documentation?.Id ?? currentDatabaseId;

	public bool? HasSelectedAnySubItem => DocumentationsModulesData.SelectedDocumentations?.Any((DocumentationModules x) => x.Modules.Any((ModuleRow y) => y.IsShown));

	[Browsable(true)]
	public event EventHandler ModulesCellValueChanged;

	public event Action<bool, Button> EditButton
	{
		add
		{
			if (editButton == null || !editButton.GetInvocationList().Contains(value))
			{
				editButton = (Action<bool, Button>)Delegate.Combine(editButton, value);
			}
		}
		remove
		{
			editButton = (Action<bool, Button>)Delegate.Remove(editButton, value);
		}
	}

	public ChooseModulesUserControl()
	{
		InitializeComponent();
	}

	public void GetModules(int? databaseId, MetadataEditorUserControl metadataEditor, bool useAllDocumentations, bool useOtherModule, bool includeBusinessGlossary, List<int> selectedModules)
	{
		currentDatabaseId = databaseId;
		metadataEditorUserControl = metadataEditor;
		LoadModules(currentDatabaseId, ref documentationsModulesData, modulesGrid, documentationsGridView, useAllDocumentations, useOtherModule, includeBusinessGlossary, selectedModules);
	}

	public void ClearModules()
	{
		documentationsModulesData = new DocumentationModulesContainer();
	}

	public void TurnOffHyperlinksVisibility()
	{
		layoutControlItem3.Visibility = LayoutVisibility.Never;
		layoutControlItem6.Visibility = LayoutVisibility.Never;
		emptySpaceItem4.Visibility = LayoutVisibility.Never;
	}

	public int GetVisibleRowsHeight()
	{
		int num = 0;
		foreach (DocumentationModules item in modulesGrid.DataSource as List<DocumentationModules>)
		{
			num += (item.IsSelected ? ((item.Modules.Count + 1) * 18) : 18);
		}
		return num;
	}

	private void LoadModules(int? currentDatabaseId, ref DocumentationModulesContainer documentationsModulesData, GridControl modulesGrid, GridView documentationsGridView, bool useAllDocumentations, bool isExport, bool includeBusinessGlossary, List<int> selectedModules)
	{
		documentationsModulesData = new DocumentationModulesContainer();
		IEnumerable<DatabaseRow> enumerable = new List<DatabaseRow>();
		IEnumerable<ModuleRow> source = new List<ModuleRow>();
		if (useAllDocumentations)
		{
			enumerable = from x in DB.Database.GetData()
				select new DatabaseRow(x) into x
				where !x.IsWelcomeDocumentation && x.IsValidDatabase
				orderby x.Title
				select x;
			source = from x in DB.Module.GetDataByDatabaseWithoutDescription()
				select new ModuleRow(x);
		}
		else if (currentDatabaseId.HasValue)
		{
			DocumentationObject dataById = DB.Database.GetDataById(currentDatabaseId.Value);
			if (dataById != null)
			{
				DatabaseRow databaseRow = new DatabaseRow(dataById);
				enumerable = new List<DatabaseRow>(new DatabaseRow[1] { databaseRow });
				source = from x in DB.Module.GetDataByDatabaseWithoutDescription(databaseRow.Id)
					select new ModuleRow(x);
			}
		}
		foreach (DatabaseRow documentation in enumerable)
		{
			bool flag = !currentDatabaseId.HasValue || documentation.IdValue == currentDatabaseId;
			List<ModuleRow> list = source.Where((ModuleRow x) => x.DatabaseId == documentation.Id).ToList();
			foreach (ModuleRow item3 in list)
			{
				item3.IsShown = (isExport ? flag : selectedModules.Contains(item3.Id ?? (-1)));
			}
			if (isExport && list.Count > 0 && ModuleDoc.AreObjectsWithoutModule(documentation.IdValue))
			{
				ModuleRow generalModule = ModuleRow.GetGeneralModule();
				generalModule.IsShown = flag;
				list.Add(generalModule);
			}
			DocumentationModules item = new DocumentationModules
			{
				Documentation = documentation,
				Modules = list,
				IsSelected = (flag || list.Any((ModuleRow x) => x.IsShown))
			};
			if (!isExport)
			{
				documentationsModulesData.Data.Add(item);
			}
			else if (isExport)
			{
				documentationsModulesData.Data.Add(item);
			}
		}
		documentationsModulesData.Data = documentationsModulesData.Data.OrderBy((DocumentationModules x) => x.Documentation.Title).ToList();
		if (includeBusinessGlossary)
		{
			List<BusinessGlossaryObject> businessGlossaries = DB.BusinessGlossary.GetBusinessGlossaries(null);
			List<DocumentationModules> list2 = new List<DocumentationModules>();
			foreach (BusinessGlossaryObject item4 in businessGlossaries)
			{
				DatabaseRow databaseRow2 = new DatabaseRow
				{
					Id = item4.DocumentationId,
					Title = item4.Title,
					ObjectTypeValue = SharedObjectTypeEnum.ObjectType.BusinessGlossary
				};
				DocumentationModules item2 = new DocumentationModules
				{
					Documentation = databaseRow2,
					Modules = new List<ModuleRow>(),
					IsSelected = (!currentDatabaseId.HasValue || databaseRow2.IdValue == currentDatabaseId)
				};
				list2.Add(item2);
			}
			documentationsModulesData.Data.InsertRange(0, list2.OrderBy((DocumentationModules x) => x.Documentation.Title));
		}
		modulesGrid.DataSource = documentationsModulesData.Data;
		foreach (DocumentationModules selectedDocumentation in documentationsModulesData.SelectedDocumentations)
		{
			documentationsGridView.ExpandMasterRow(documentationsGridView.GetRowHandle(documentationsModulesData.Data.IndexOf(selectedDocumentation)));
		}
	}

	public List<int> GetChosenModules()
	{
		List<int> result = documentationsModulesData.GetSelectedModulesIds(currentDatabaseId).ToList();
		modulesGridView.RefreshData();
		documentationsGridView.RefreshData();
		return result;
	}

	public DocumentationModules[] GetSelectedModules()
	{
		if (documentationsModulesData != null)
		{
			return documentationsModulesData.GetSelectedModules();
		}
		return new DocumentationModules[0];
	}

	private void checkNoneHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		documentationsGridView.BeginUpdate();
		modulesGridView.BeginUpdate();
		foreach (DocumentationModules datum in DocumentationsModulesData.Data)
		{
			datum.IsSelected = false;
			foreach (ModuleRow module in datum.Modules)
			{
				module.IsShown = false;
			}
		}
		modulesGridView.RefreshData();
		modulesGridView.EndUpdate();
		if (Button != null)
		{
			Button.Enabled = false;
		}
		documentationsGridView.EndUpdate();
	}

	private void checkAllHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		documentationsGridView.BeginUpdate();
		modulesGridView.BeginUpdate();
		foreach (DocumentationModules datum in DocumentationsModulesData.Data)
		{
			datum.IsSelected = true;
			foreach (ModuleRow module in datum.Modules)
			{
				module.IsShown = true;
			}
		}
		modulesGridView.RefreshData();
		modulesGridView.EndUpdate();
		if (Button != null)
		{
			Button.Enabled = true;
		}
		documentationsGridView.EndUpdate();
	}

	private void documentationsGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		GridView gridView = sender as GridView;
		List<DocumentationModules> obj = gridView.DataSource as List<DocumentationModules>;
		if (obj != null && obj.Count <= 1)
		{
			(e.Cell as GridCellInfo).CellButtonRect = Rectangle.Empty;
		}
		for (int i = 0; i < gridView.GetRelationCount(e.RowHandle); i++)
		{
			if (gridView.IsMasterRowEmptyEx(e.RowHandle, i))
			{
				(e.Cell as GridCellInfo).CellButtonRect = Rectangle.Empty;
			}
		}
	}

	private void documentationsGridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		BeginInvoke((Action)delegate
		{
			((GridView)sender).CloseEditor();
		});
	}

	private void documentationsGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (e.Column.FieldName == "IsSelected")
		{
			DocumentationModules currentRow = documentationsGridView.GetRow(e.RowHandle) as DocumentationModules;
			modulesGridView.BeginUpdate();
			currentRow.Modules.ForEach(delegate(ModuleRow x)
			{
				x.IsShown = currentRow?.IsSelected ?? false;
			});
			modulesGridView.EndUpdate();
		}
		if (Button != null)
		{
			Button.Enabled = DocumentationsModulesData.Data.Any((DocumentationModules x) => x.IsSelected);
		}
		this.ModulesCellValueChanged?.Invoke(this, e);
	}

	private void modulesGridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		BeginInvoke((Action)delegate
		{
			((GridView)sender).CloseEditor();
		});
	}

	private void modulesGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (e.Column.FieldName == "IsShown" && documentationsGridView.GetRow(modulesGrid.FocusedView.SourceRowHandle) is DocumentationModules documentationModules && e.Value as bool? == true)
		{
			documentationsGridView.BeginUpdate();
			documentationModules.IsSelected = true;
			documentationsGridView.EndUpdate();
		}
		if (Button != null)
		{
			Button.Enabled = HasSelectedAnySubItem == true;
		}
		this.ModulesCellValueChanged?.Invoke(this, e);
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
		this.modulesGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.moduleSelectionGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.moduleRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.moduleTitleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.modulesGrid = new DevExpress.XtraGrid.GridControl();
		this.documentationsGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.documentationSelectionGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.documentationTitleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.modulesLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.checkNoneHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.checkAllHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.modulesGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.moduleRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.modulesGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.documentationsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.modulesLayoutControl).BeginInit();
		this.modulesLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.checkNoneHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkAllHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		base.SuspendLayout();
		this.modulesGridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.modulesGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.moduleSelectionGridColumn, this.moduleTitleGridColumn });
		this.modulesGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
		this.modulesGridView.GridControl = this.modulesGrid;
		this.modulesGridView.Name = "modulesGridView";
		this.modulesGridView.OptionsCustomization.AllowColumnMoving = false;
		this.modulesGridView.OptionsCustomization.AllowColumnResizing = false;
		this.modulesGridView.OptionsDetail.ShowDetailTabs = false;
		this.modulesGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.modulesGridView.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.modulesGridView.OptionsView.ShowColumnHeaders = false;
		this.modulesGridView.OptionsView.ShowGroupPanel = false;
		this.modulesGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.modulesGridView.OptionsView.ShowIndicator = false;
		this.modulesGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.modulesGridView.RowHighlightingIsEnabled = true;
		this.modulesGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(modulesGridView_CellValueChanged);
		this.modulesGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(modulesGridView_CellValueChanging);
		this.moduleSelectionGridColumn.ColumnEdit = this.moduleRepositoryItemCheckEdit;
		this.moduleSelectionGridColumn.FieldName = "IsShown";
		this.moduleSelectionGridColumn.MaxWidth = 22;
		this.moduleSelectionGridColumn.MinWidth = 22;
		this.moduleSelectionGridColumn.Name = "moduleSelectionGridColumn";
		this.moduleSelectionGridColumn.OptionsColumn.AllowSize = false;
		this.moduleSelectionGridColumn.OptionsColumn.ShowCaption = false;
		this.moduleSelectionGridColumn.Visible = true;
		this.moduleSelectionGridColumn.VisibleIndex = 0;
		this.moduleSelectionGridColumn.Width = 22;
		this.moduleRepositoryItemCheckEdit.AutoHeight = false;
		this.moduleRepositoryItemCheckEdit.Caption = "Check";
		this.moduleRepositoryItemCheckEdit.Name = "moduleRepositoryItemCheckEdit";
		this.moduleTitleGridColumn.Caption = "Subject Area";
		this.moduleTitleGridColumn.FieldName = "Title";
		this.moduleTitleGridColumn.FieldNameSortGroup = "Title";
		this.moduleTitleGridColumn.Name = "moduleTitleGridColumn";
		this.moduleTitleGridColumn.OptionsColumn.AllowEdit = false;
		this.moduleTitleGridColumn.OptionsColumn.AllowFocus = false;
		this.moduleTitleGridColumn.OptionsColumn.ReadOnly = true;
		this.moduleTitleGridColumn.OptionsColumn.ShowCaption = false;
		this.moduleTitleGridColumn.Visible = true;
		this.moduleTitleGridColumn.VisibleIndex = 1;
		this.modulesGrid.Cursor = System.Windows.Forms.Cursors.Default;
		gridLevelNode.LevelTemplate = this.modulesGridView;
		gridLevelNode.RelationName = "Modules";
		this.modulesGrid.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[1] { gridLevelNode });
		this.modulesGrid.Location = new System.Drawing.Point(0, 24);
		this.modulesGrid.MainView = this.documentationsGridView;
		this.modulesGrid.Name = "modulesGrid";
		this.modulesGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.moduleRepositoryItemCheckEdit });
		this.modulesGrid.Size = new System.Drawing.Size(606, 398);
		this.modulesGrid.TabIndex = 0;
		this.modulesGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.documentationsGridView, this.modulesGridView });
		this.documentationsGridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.documentationsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.documentationSelectionGridColumn, this.documentationTitleGridColumn });
		this.documentationsGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFullFocus;
		this.documentationsGridView.GridControl = this.modulesGrid;
		this.documentationsGridView.LevelIndent = 36;
		this.documentationsGridView.Name = "documentationsGridView";
		this.documentationsGridView.OptionsCustomization.AllowColumnMoving = false;
		this.documentationsGridView.OptionsCustomization.AllowColumnResizing = false;
		this.documentationsGridView.OptionsDetail.DetailMode = DevExpress.XtraGrid.Views.Grid.DetailMode.Embedded;
		this.documentationsGridView.OptionsDetail.ShowDetailTabs = false;
		this.documentationsGridView.OptionsDetail.ShowEmbeddedDetailIndent = DevExpress.Utils.DefaultBoolean.False;
		this.documentationsGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.documentationsGridView.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.documentationsGridView.OptionsView.ShowColumnHeaders = false;
		this.documentationsGridView.OptionsView.ShowGroupPanel = false;
		this.documentationsGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.documentationsGridView.OptionsView.ShowIndicator = false;
		this.documentationsGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.documentationsGridView.RowHighlightingIsEnabled = true;
		this.documentationsGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(documentationsGridView_CustomDrawCell);
		this.documentationsGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(documentationsGridView_CellValueChanged);
		this.documentationsGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(documentationsGridView_CellValueChanging);
		this.documentationSelectionGridColumn.ColumnEdit = this.moduleRepositoryItemCheckEdit;
		this.documentationSelectionGridColumn.FieldName = "IsSelected";
		this.documentationSelectionGridColumn.MaxWidth = 18;
		this.documentationSelectionGridColumn.MinWidth = 18;
		this.documentationSelectionGridColumn.Name = "documentationSelectionGridColumn";
		this.documentationSelectionGridColumn.OptionsColumn.AllowSize = false;
		this.documentationSelectionGridColumn.OptionsColumn.ShowCaption = false;
		this.documentationSelectionGridColumn.Visible = true;
		this.documentationSelectionGridColumn.VisibleIndex = 0;
		this.documentationSelectionGridColumn.Width = 18;
		this.documentationTitleGridColumn.Caption = "Documentation";
		this.documentationTitleGridColumn.FieldName = "Documentation.Title";
		this.documentationTitleGridColumn.Name = "documentationTitleGridColumn";
		this.documentationTitleGridColumn.OptionsColumn.AllowEdit = false;
		this.documentationTitleGridColumn.OptionsColumn.AllowFocus = false;
		this.documentationTitleGridColumn.OptionsColumn.ReadOnly = true;
		this.documentationTitleGridColumn.Visible = true;
		this.documentationTitleGridColumn.VisibleIndex = 1;
		this.documentationTitleGridColumn.Width = 596;
		this.modulesLayoutControl.AllowCustomization = false;
		this.modulesLayoutControl.Controls.Add(this.checkNoneHyperLinkEdit);
		this.modulesLayoutControl.Controls.Add(this.checkAllHyperLinkEdit);
		this.modulesLayoutControl.Controls.Add(this.modulesGrid);
		this.modulesLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.modulesLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.modulesLayoutControl.Name = "modulesLayoutControl";
		this.modulesLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(748, 241, 808, 578);
		this.modulesLayoutControl.OptionsView.UseDefaultDragAndDropRendering = false;
		this.modulesLayoutControl.Root = this.layoutControlGroup3;
		this.modulesLayoutControl.Size = new System.Drawing.Size(606, 422);
		this.modulesLayoutControl.TabIndex = 1;
		this.modulesLayoutControl.Text = "layoutControl2";
		this.checkNoneHyperLinkEdit.EditValue = "None";
		this.checkNoneHyperLinkEdit.Location = new System.Drawing.Point(27, 2);
		this.checkNoneHyperLinkEdit.MaximumSize = new System.Drawing.Size(40, 0);
		this.checkNoneHyperLinkEdit.MinimumSize = new System.Drawing.Size(40, 0);
		this.checkNoneHyperLinkEdit.Name = "checkNoneHyperLinkEdit";
		this.checkNoneHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.checkNoneHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.checkNoneHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.checkNoneHyperLinkEdit.Size = new System.Drawing.Size(40, 18);
		this.checkNoneHyperLinkEdit.StyleController = this.modulesLayoutControl;
		this.checkNoneHyperLinkEdit.TabIndex = 8;
		this.checkNoneHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(checkNoneHyperLinkEdit_OpenLink);
		this.checkAllHyperLinkEdit.EditValue = "All";
		this.checkAllHyperLinkEdit.Location = new System.Drawing.Point(2, 2);
		this.checkAllHyperLinkEdit.MaximumSize = new System.Drawing.Size(25, 0);
		this.checkAllHyperLinkEdit.MinimumSize = new System.Drawing.Size(25, 0);
		this.checkAllHyperLinkEdit.Name = "checkAllHyperLinkEdit";
		this.checkAllHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.checkAllHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.checkAllHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.checkAllHyperLinkEdit.Size = new System.Drawing.Size(25, 18);
		this.checkAllHyperLinkEdit.StyleController = this.modulesLayoutControl;
		this.checkAllHyperLinkEdit.TabIndex = 7;
		this.checkAllHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(checkAllHyperLinkEdit_OpenLink);
		this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup3.GroupBordersVisible = false;
		this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.layoutControlItem2, this.layoutControlItem3, this.layoutControlItem6, this.emptySpaceItem4 });
		this.layoutControlGroup3.Name = "Root";
		this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup3.Size = new System.Drawing.Size(606, 422);
		this.layoutControlGroup3.TextVisible = false;
		this.layoutControlItem2.Control = this.modulesGrid;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem2.Size = new System.Drawing.Size(606, 398);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.Control = this.checkAllHyperLinkEdit;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(25, 24);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(25, 24);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(25, 24);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.layoutControlItem6.Control = this.checkNoneHyperLinkEdit;
		this.layoutControlItem6.Location = new System.Drawing.Point(25, 0);
		this.layoutControlItem6.MaxSize = new System.Drawing.Size(45, 24);
		this.layoutControlItem6.MinSize = new System.Drawing.Size(45, 24);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.Size = new System.Drawing.Size(45, 24);
		this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(70, 0);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(536, 24);
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.modulesLayoutControl);
		base.Name = "ChooseModulesUserControl";
		base.Size = new System.Drawing.Size(606, 422);
		((System.ComponentModel.ISupportInitialize)this.modulesGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.moduleRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.modulesGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.documentationsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.modulesLayoutControl).EndInit();
		this.modulesLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.checkNoneHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkAllHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		base.ResumeLayout(false);
	}
}
