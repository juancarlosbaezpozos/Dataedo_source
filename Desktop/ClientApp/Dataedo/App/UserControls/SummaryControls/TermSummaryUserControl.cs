using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.CommonFunctionsForPanels;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.History;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Helpers;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Columns;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.App.UserControls.PanelControls.TableUserControlHelpers;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
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
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.UserControls.SummaryControls;

public class TermSummaryUserControl : BaseSummaryUserControl
{
	private readonly TermsDragDropManager termsDragDropManager;

	private int databaseId;

	private DBTreeNode folderNode;

	private IContainer components;

	private LabelControl allDatabaseTermsLabel;

	private GridControl termsGrid;

	private GridColumn titleTableTermsGridColumn;

	private RepositoryItemLookUpEdit typeRepositoryItemLookUpEdit;

	private RepositoryItemLookUpEdit moduleRepositoryItemLookUpEdit;

	private RepositoryItemPictureEdit iconTableRepositoryItemPictureEdit;

	private RepositoryItemCheckedComboBoxEdit moduleRepositoryItemCheckedComboBoxEdit;

	private ToolTipController termsToolTipController;

	private RepositoryItemCustomTextEdit titleRepositoryItemCustomTextEdit;

	private BulkCopyGridUserControl termsGridView;

	private GridColumn typeTableGridColumn;

	private GridPanelUserControl gridPanelUserControl;

	private GridColumn createdGridColumn;

	private GridColumn createdByGridColumn;

	private GridColumn lastUpdatedGridColumn;

	private GridColumn lastUpdatedByGridColumn;

	private GridColumn iconGridColumn;

	private PopupMenu allTermsPopupMenu;

	private BarButtonItem newBarButtonItem;

	private BarButtonItem deleteBarButtonItem;

	private BarManager allTermsBarManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private SplashScreenManager splashScreenManager;

	private BarButtonItem addTermBarButtonItem;

	private BarButtonItem viewHistoryBarButtonItem;

	public event EventHandler ShowTermControl;

	public TermSummaryUserControl()
	{
		InitializeComponent();
		base.BulkCopy = new SummaryBulkCopy(SharedObjectTypeEnum.ObjectType.Term, this);
		termsGridView.Copy = base.BulkCopy;
		termsGridView.SplashScreenManager = splashScreenManager;
		MetadataToolTip.SetColumnToolTip(createdGridColumn, "first_imported");
		MetadataToolTip.SetColumnToolTip(createdByGridColumn, "first_imported_by");
		MetadataToolTip.SetColumnToolTip(lastUpdatedGridColumn, "last_updated");
		MetadataToolTip.SetColumnToolTip(lastUpdatedByGridColumn, "last_updated_by");
		ObjectEventArgs = new ObjectEventArgs();
		LengthValidation.SetTitleOrNameLengthLimit(titleRepositoryItemCustomTextEdit);
		termsDragDropManager = new TermsDragDropManager();
		termsDragDropManager.AddEvents(termsGrid);
	}

	private void TermSummaryUserControl_Load(object sender, EventArgs e)
	{
		AddEvents();
	}

	public override void RefreshData()
	{
		termsGridView.RefreshData();
	}

	protected override void AddEvents()
	{
		CommonFunctionsPanels.AddEventsForSummaryTable(termsGridView, SharedObjectTypeEnum.ObjectType.Term, allTermsPopupMenu, allTermsBarManager, this.ShowTermControl, iconGridColumn, deleteBarButtonItem, ObjectEventArgs, base.TreeMenu, moduleRepositoryItemCheckedComboBoxEdit, this, gridPanelUserControl, null, null, null, null, null, FindForm());
		base.DragRows = new DragRowsBase<TermObject>(termsGridView);
		base.DragRows.AddEvents();
		List<ToolTipData> toolTipDataList = new List<ToolTipData>
		{
			new ToolTipData(termsGrid, SharedObjectTypeEnum.ObjectType.Term, iconGridColumn.VisibleIndex)
		};
		addTermBarButtonItem.ItemClick += delegate
		{
			CommonFunctionsDatabase.AddNewTerm(termsGridView, databaseId, folderNode, base.ParentForm);
		};
		CommonFunctionsPanels.AddEventsForToolTips(termsToolTipController, toolTipDataList);
		CommonFunctionsPanels.AddEventForAutoFilterRow(termsGridView);
	}

	public void SaveColumns()
	{
		if (base.ObjectType.HasValue && base.ObjectType != SharedObjectTypeEnum.ObjectType.UnresolvedEntity)
		{
			UserViewData.SaveColumns(UserViewData.GetViewName(this, termsGridView, base.ObjectType.Value), termsGridView);
		}
	}

	public override void SetParameters(DBTreeNode node, CustomFieldsSupport customFieldsSupport, SharedObjectTypeEnum.ObjectType? objectType)
	{
		gridPanelUserControl.SetRemoveButtonVisibility(value: false);
		gridPanelUserControl.SetDefaultLockButtonVisibility(value: true);
		gridPanelUserControl.CustomFields += base.MainControl.EditCustomFields;
		bool flag = false;
		if (!base.ObjectType.HasValue)
		{
			flag = true;
		}
		base.SetParameters(node, customFieldsSupport, objectType);
		databaseId = node.DatabaseId;
		folderNode = node;
		termsGridView.SetRowCellValue(-2147483646, "iconGridColumn", Resources.blank_16);
		termsGrid.DataSource = null;
		new CustomFieldsCellsTypesSupport(isForSummaryTable: true).SetCustomColumns(termsGridView, customFieldsSupport, SharedObjectTypeEnum.ObjectType.Term, customFieldsAsArray: true);
		List<TermObject> allTerms = DB.BusinessGlossary.GetAllTerms(node.Id);
		termsGrid.DataSource = allTerms;
		CommonFunctionsPanels.FillObjectEventArgs(ObjectEventArgs, node);
		ObjectEventArgs.DatabaseId = node.DatabaseId;
		ObjectEventArgs.ModuleId = null;
		deleteBarButtonItem.Caption = "Remove from repository";
		CommonFunctionsPanels.SetSummaryObjectTitle(allDatabaseTermsLabel, objectType, "Terms", node.ParentNode.Title);
		gridPanelUserControl.Initialize(SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Term), objectType == SharedObjectTypeEnum.ObjectType.Module);
		if (flag && !UserViewData.LoadColumns(UserViewData.GetViewName(this, termsGridView, base.ObjectType.Value), termsGridView))
		{
			CustomFieldsCellsTypesSupport.SortCustomColumns(termsGridView);
			CommonFunctionsPanels.SetBestFitForColumns(termsGridView);
		}
		gridPanelUserControl.InsertAdditionalButton(addTermBarButtonItem, 4);
		SaveTermSummaryCustomFields(allTerms);
	}

	public override void ClearData()
	{
		termsGrid.BeginUpdate();
		termsGrid.DataSource = null;
		termsGrid.EndUpdate();
	}

	private void TermsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		gridPanelUserControl.SetRemoveButtonVisibility(termsGridView.SelectedRowsCount > 0);
	}

	private void TermSummaryUserControl_Leave(object sender, EventArgs e)
	{
		termsGridView.HideCustomization();
	}

	public override void CloseEditor()
	{
		termsGridView.CloseEditor();
	}

	private void TermsGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (e.Column != iconGridColumn && GridColumnsHelper.ShouldColumnBeGrayOut(e.Column) && (!termsGridView.IsFocusedView || !termsGridView.GetSelectedRows().Contains(e.RowHandle)))
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridNonEditableColumnsBackColor;
		}
	}

	private void ViewHistoryBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		TermObject termObject = termsGridView?.GetFocusedRow() as TermObject;
		string field = termsGridView?.FocusedColumn?.FieldName?.ToLower();
		if (termObject == null || (field != "title" && base.CustomFieldsSupport.GetField(field) == null) || termObject == null)
		{
			return;
		}
		try
		{
			using HistoryForm historyForm = new HistoryForm();
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			historyForm.CustomFieldCaption = termsGridView?.Columns?.Where((GridColumn x) => x.FieldName.ToLower() == field)?.FirstOrDefault()?.Caption;
			historyForm.SetParameters(termObject.TermId, field, termObject.Title, null, termObject.DatabaseShowSchema, termObject.DatabaseShowSchemaOverride, null, "glossary_terms", termObject.TypeTitle, null, null, BusinessGlossarySupport.GetTermIcon(termObject.TypeIconId));
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			historyForm.ShowDialog(FindForm());
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void SaveTermSummaryCustomFields(List<TermObject> data)
	{
		CommonFunctionsPanels.customFieldsForHistory = new Dictionary<int, Dictionary<string, BaseWithCustomFields.CustomFieldWithValue>>();
		IEnumerable<GridColumn> enumerable = termsGridView?.Columns?.Where((GridColumn x) => x.FieldName.Contains("Field"));
		if (enumerable == null)
		{
			return;
		}
		foreach (TermObject objectWithModules in data)
		{
			TermObject termObject = objectWithModules;
			if (termObject != null && termObject.TermId.HasValue)
			{
				CommonFunctionsPanels.customFieldsForHistory.Add(objectWithModules.TermId.Value, enumerable.ToDictionary((GridColumn x) => x.FieldName, (GridColumn y) => BaseCustomFieldDB.GetCustomFieldWithValue(base.CustomFieldsSupport.GetField(y.FieldName), objectWithModules.GetField(y.FieldName)?.ToString())));
			}
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
		Dataedo.App.Tools.DefaultBulkCopy defaultBulkCopy = new Dataedo.App.Tools.DefaultBulkCopy();
		this.allDatabaseTermsLabel = new DevExpress.XtraEditors.LabelControl();
		this.termsGrid = new DevExpress.XtraGrid.GridControl();
		this.termsGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.iconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconTableRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.titleTableTermsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.titleRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.typeTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.moduleRepositoryItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.typeRepositoryItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.moduleRepositoryItemCheckedComboBoxEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
		this.termsToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.gridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.allTermsPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.newBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.deleteBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.allTermsBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.addTermBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true, typeof(System.Windows.Forms.UserControl));
		this.viewHistoryBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		((System.ComponentModel.ISupportInitialize)this.termsGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.termsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconTableRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.moduleRepositoryItemLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeRepositoryItemLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.moduleRepositoryItemCheckedComboBoxEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allTermsPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allTermsBarManager).BeginInit();
		base.SuspendLayout();
		this.allDatabaseTermsLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.allDatabaseTermsLabel.Appearance.Options.UseFont = true;
		this.allDatabaseTermsLabel.Appearance.Options.UseTextOptions = true;
		this.allDatabaseTermsLabel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.allDatabaseTermsLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.allDatabaseTermsLabel.Dock = System.Windows.Forms.DockStyle.Top;
		this.allDatabaseTermsLabel.Location = new System.Drawing.Point(0, 0);
		this.allDatabaseTermsLabel.Name = "allDatabaseTermsLabel";
		this.allDatabaseTermsLabel.Padding = new System.Windows.Forms.Padding(0, 4, 4, 4);
		this.allDatabaseTermsLabel.Size = new System.Drawing.Size(1040, 19);
		this.allDatabaseTermsLabel.TabIndex = 10;
		this.allDatabaseTermsLabel.Text = "Terms in the database:";
		this.allDatabaseTermsLabel.UseMnemonic = false;
		this.termsGrid.AllowDrop = true;
		this.termsGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.termsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
		this.termsGrid.Location = new System.Drawing.Point(0, 47);
		this.termsGrid.MainView = this.termsGridView;
		this.termsGrid.Name = "termsGrid";
		this.termsGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.moduleRepositoryItemLookUpEdit, this.typeRepositoryItemLookUpEdit, this.iconTableRepositoryItemPictureEdit, this.moduleRepositoryItemCheckedComboBoxEdit, this.titleRepositoryItemCustomTextEdit });
		this.termsGrid.Size = new System.Drawing.Size(1040, 332);
		this.termsGrid.TabIndex = 11;
		this.termsGrid.ToolTipController = this.termsToolTipController;
		this.termsGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.termsGridView });
		this.termsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.iconGridColumn, this.titleTableTermsGridColumn, this.typeTableGridColumn, this.createdGridColumn, this.createdByGridColumn, this.lastUpdatedGridColumn, this.lastUpdatedByGridColumn });
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
		this.termsGridView.RowHighlightingIsEnabled = true;
		this.termsGridView.SplashScreenManager = null;
		this.termsGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(TermsGridView_CustomDrawCell);
		this.termsGridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(TermsGridView_SelectionChanged);
		this.iconGridColumn.Caption = " ";
		this.iconGridColumn.ColumnEdit = this.iconTableRepositoryItemPictureEdit;
		this.iconGridColumn.FieldName = "iconTableGridColumn";
		this.iconGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.iconGridColumn.MaxWidth = 21;
		this.iconGridColumn.MinWidth = 21;
		this.iconGridColumn.Name = "iconGridColumn";
		this.iconGridColumn.OptionsColumn.AllowEdit = false;
		this.iconGridColumn.OptionsColumn.ReadOnly = true;
		this.iconGridColumn.OptionsFilter.AllowFilter = false;
		this.iconGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.iconGridColumn.Visible = true;
		this.iconGridColumn.VisibleIndex = 0;
		this.iconGridColumn.Width = 21;
		this.iconTableRepositoryItemPictureEdit.AllowFocused = false;
		this.iconTableRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.iconTableRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconTableRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconTableRepositoryItemPictureEdit.Name = "iconTableRepositoryItemPictureEdit";
		this.iconTableRepositoryItemPictureEdit.ShowMenu = false;
		this.titleTableTermsGridColumn.Caption = "Title";
		this.titleTableTermsGridColumn.ColumnEdit = this.titleRepositoryItemCustomTextEdit;
		this.titleTableTermsGridColumn.FieldName = "Title";
		this.titleTableTermsGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.titleTableTermsGridColumn.Name = "titleTableTermsGridColumn";
		this.titleTableTermsGridColumn.OptionsColumn.AllowEdit = false;
		this.titleTableTermsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.titleTableTermsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.titleTableTermsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.titleTableTermsGridColumn.Tag = "FIT_WIDTH";
		this.titleTableTermsGridColumn.Visible = true;
		this.titleTableTermsGridColumn.VisibleIndex = 1;
		this.titleTableTermsGridColumn.Width = 140;
		this.titleRepositoryItemCustomTextEdit.AutoHeight = false;
		this.titleRepositoryItemCustomTextEdit.Name = "titleRepositoryItemCustomTextEdit";
		this.typeTableGridColumn.Caption = "Type";
		this.typeTableGridColumn.FieldName = "TypeTitle";
		this.typeTableGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.typeTableGridColumn.Name = "typeTableGridColumn";
		this.typeTableGridColumn.OptionsColumn.AllowEdit = false;
		this.typeTableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.typeTableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.typeTableGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.typeTableGridColumn.Tag = "FIT_WIDTH";
		this.typeTableGridColumn.Visible = true;
		this.typeTableGridColumn.VisibleIndex = 2;
		this.createdGridColumn.Caption = "Created";
		this.createdGridColumn.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
		this.createdGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.createdGridColumn.FieldName = "CreationDate";
		this.createdGridColumn.Name = "createdGridColumn";
		this.createdGridColumn.OptionsColumn.AllowEdit = false;
		this.createdGridColumn.Width = 120;
		this.createdByGridColumn.Caption = "Created by";
		this.createdByGridColumn.FieldName = "CreatedBy";
		this.createdByGridColumn.Name = "createdByGridColumn";
		this.createdByGridColumn.OptionsColumn.AllowEdit = false;
		this.createdByGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.createdByGridColumn.Width = 150;
		this.lastUpdatedGridColumn.Caption = "Last updated";
		this.lastUpdatedGridColumn.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
		this.lastUpdatedGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.lastUpdatedGridColumn.FieldName = "LastModificationDate";
		this.lastUpdatedGridColumn.Name = "lastUpdatedGridColumn";
		this.lastUpdatedGridColumn.OptionsColumn.AllowEdit = false;
		this.lastUpdatedGridColumn.Width = 100;
		this.lastUpdatedByGridColumn.Caption = "Last updated by";
		this.lastUpdatedByGridColumn.FieldName = "ModifiedBy";
		this.lastUpdatedByGridColumn.Name = "lastUpdatedByGridColumn";
		this.lastUpdatedByGridColumn.OptionsColumn.AllowEdit = false;
		this.lastUpdatedByGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.lastUpdatedByGridColumn.Width = 100;
		this.moduleRepositoryItemLookUpEdit.AutoHeight = false;
		this.moduleRepositoryItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.moduleRepositoryItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("title", "Title")
		});
		this.moduleRepositoryItemLookUpEdit.DisplayMember = "title";
		this.moduleRepositoryItemLookUpEdit.Name = "moduleRepositoryItemLookUpEdit";
		this.moduleRepositoryItemLookUpEdit.NullText = "";
		this.moduleRepositoryItemLookUpEdit.ShowFooter = false;
		this.moduleRepositoryItemLookUpEdit.ShowHeader = false;
		this.moduleRepositoryItemLookUpEdit.ShowLines = false;
		this.moduleRepositoryItemLookUpEdit.ValueMember = "module_id";
		this.typeRepositoryItemLookUpEdit.AutoHeight = false;
		this.typeRepositoryItemLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.typeRepositoryItemLookUpEdit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("title", "Title")
		});
		this.typeRepositoryItemLookUpEdit.DisplayMember = "title";
		this.typeRepositoryItemLookUpEdit.Name = "typeRepositoryItemLookUpEdit";
		this.typeRepositoryItemLookUpEdit.NullText = "";
		this.typeRepositoryItemLookUpEdit.ShowFooter = false;
		this.typeRepositoryItemLookUpEdit.ShowHeader = false;
		this.typeRepositoryItemLookUpEdit.ShowLines = false;
		this.typeRepositoryItemLookUpEdit.ValueMember = "name";
		this.moduleRepositoryItemCheckedComboBoxEdit.AutoHeight = false;
		this.moduleRepositoryItemCheckedComboBoxEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.moduleRepositoryItemCheckedComboBoxEdit.DisplayMember = "DisplayName";
		this.moduleRepositoryItemCheckedComboBoxEdit.Name = "moduleRepositoryItemCheckedComboBoxEdit";
		this.moduleRepositoryItemCheckedComboBoxEdit.PopupSizeable = false;
		this.moduleRepositoryItemCheckedComboBoxEdit.SelectAllItemVisible = false;
		this.moduleRepositoryItemCheckedComboBoxEdit.ShowButtons = false;
		this.moduleRepositoryItemCheckedComboBoxEdit.ShowPopupCloseButton = false;
		this.moduleRepositoryItemCheckedComboBoxEdit.ValueMember = "ModuleId";
		this.termsToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.gridPanelUserControl.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.gridPanelUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.gridPanelUserControl.GridView = this.termsGridView;
		this.gridPanelUserControl.Location = new System.Drawing.Point(0, 19);
		this.gridPanelUserControl.Name = "gridPanelUserControl";
		this.gridPanelUserControl.Size = new System.Drawing.Size(1040, 28);
		this.gridPanelUserControl.TabIndex = 12;
		this.allTermsPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.newBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.deleteBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.viewHistoryBarButtonItem)
		});
		this.allTermsPopupMenu.Manager = this.allTermsBarManager;
		this.allTermsPopupMenu.Name = "allTermsPopupMenu";
		this.newBarButtonItem.Caption = "New";
		this.newBarButtonItem.Id = 0;
		this.newBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.add_16;
		this.newBarButtonItem.Name = "newBarButtonItem";
		this.deleteBarButtonItem.Caption = "Delete";
		this.deleteBarButtonItem.Id = 1;
		this.deleteBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.deleteBarButtonItem.Name = "deleteBarButtonItem";
		this.allTermsBarManager.DockControls.Add(this.barDockControlTop);
		this.allTermsBarManager.DockControls.Add(this.barDockControlBottom);
		this.allTermsBarManager.DockControls.Add(this.barDockControlLeft);
		this.allTermsBarManager.DockControls.Add(this.barDockControlRight);
		this.allTermsBarManager.Form = this;
		this.allTermsBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[4] { this.newBarButtonItem, this.deleteBarButtonItem, this.addTermBarButtonItem, this.viewHistoryBarButtonItem });
		this.allTermsBarManager.MaxItemId = 4;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.allTermsBarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(1040, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 379);
		this.barDockControlBottom.Manager = this.allTermsBarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(1040, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.allTermsBarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 379);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1040, 0);
		this.barDockControlRight.Manager = this.allTermsBarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 379);
		this.addTermBarButtonItem.Hint = "Add Term";
		this.addTermBarButtonItem.Id = 2;
		this.addTermBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.term_add_16;
		this.addTermBarButtonItem.Name = "addTermBarButtonItem";
		this.splashScreenManager.ClosingDelay = 500;
		this.viewHistoryBarButtonItem.Caption = "View History";
		this.viewHistoryBarButtonItem.Id = 3;
		this.viewHistoryBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.search_16;
		this.viewHistoryBarButtonItem.Name = "viewHistoryBarButtonItem";
		this.viewHistoryBarButtonItem.Tag = "viewHistory";
		this.viewHistoryBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ViewHistoryBarButtonItem_ItemClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.termsGrid);
		base.Controls.Add(this.gridPanelUserControl);
		base.Controls.Add(this.allDatabaseTermsLabel);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "TermSummaryUserControl";
		base.Size = new System.Drawing.Size(1040, 379);
		base.Load += new System.EventHandler(TermSummaryUserControl_Load);
		base.Leave += new System.EventHandler(TermSummaryUserControl_Leave);
		((System.ComponentModel.ISupportInitialize)this.termsGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.termsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconTableRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.moduleRepositoryItemLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeRepositoryItemLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.moduleRepositoryItemCheckedComboBoxEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allTermsPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allTermsBarManager).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
