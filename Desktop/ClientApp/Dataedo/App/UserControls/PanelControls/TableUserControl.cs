using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.CommonFunctionsForPanels;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.DataProfiling;
using Dataedo.App.DataProfiling.DataProfilingUserControls;
using Dataedo.App.DataProfiling.Models;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.Documentation;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.History;
using Dataedo.App.Licences;
using Dataedo.App.MenuTree;
using Dataedo.App.Onboarding;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.Search;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.Tools.UI;
using Dataedo.App.Tools.UI.Skins.Base;
using Dataedo.App.UserControls.Columns;
using Dataedo.App.UserControls.DataLineage;
using Dataedo.App.UserControls.Dependencies;
using Dataedo.App.UserControls.PanelControls.Appearance;
using Dataedo.App.UserControls.PanelControls.CommonHelpers;
using Dataedo.App.UserControls.PanelControls.TableUserControlHelpers;
using Dataedo.App.UserControls.PanelControls.TableUserControlHelpers.Interfaces;
using Dataedo.App.UserControls.SchemaImportsAndChanges;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.DataProfiling;
using Dataedo.Model.Data.History;
using Dataedo.Model.Data.Modules;
using Dataedo.Model.Data.Tables.Columns;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraTab;

namespace Dataedo.App.UserControls.PanelControls;

public class TableUserControl : BasePanelControl, ITabSettable, ITabChangable, IBusinessGlossaryObject, IPanelWithDataLineage
{
	private const int relationGridPanelColumnPickerIndex = 4;

	private const int keyGridPanelColumnPickerIndex = 4;

	public ObjectEventArgs ObjectEventArgs;

	private DataLinksManager dataLinksManager;

	private TableObject currentTableRow;

	private bool isColumnsDataLoaded;

	private bool isRelationsDataLoaded;

	private bool isConstraintsDataLoaded;

	private bool isTriggersDataLoaded;

	private bool hasLoadedColumnSuggestedDescriptions;

	private bool hasLoadedRelationSuggestedDescriptions;

	private bool hasLoadedKeySuggestedDescriptions;

	private bool hasLoadedTriggerSuggestedDescriptions;

	private bool columnsLoadRequired;

	private string name;

	private string title;

	private string schema;

	private int? databaseId;

	private string databaseName;

	private string databaseTitle;

	private string databaseServer;

	private bool showDatabaseTitle;

	private int tableId;

	private int? moduleId;

	private bool isModuleEdited;

	private UserTypeEnum.UserType source;

	private SharedDatabaseTypeEnum.DatabaseType? databaseType;

	private IdEventArgs idEventArgs;

	private GridHitInfo clickHitInfo;

	private BindingList<int> deletedColumnsRows = new BindingList<int>();

	private BindingList<int> deletedUniqueConstraintRows = new BindingList<int>();

	private BindingList<int> deletedRelationsRows = new BindingList<int>();

	private BindingList<int> deletedTriggersRows = new BindingList<int>();

	private List<ColumnProfiledDataObject> profiledColumns = new List<ColumnProfiledDataObject>();

	private List<ColumnValuesDataObject> ListOfDataProfilingTopValues = new List<ColumnValuesDataObject>();

	private bool _isTableEdited;

	private Dictionary<SharedObjectTypeEnum.ObjectType, GridView> customFieldsSettings = new Dictionary<SharedObjectTypeEnum.ObjectType, GridView>();

	private CustomFieldsCellsTypesSupport customFieldsCellsTypesSupportForGrids;

	private ColumnsDragDropManager columnsDragDropManager;

	private DataLinksDragDropManager dataLinksDragDropManager;

	private UpgradeBusinessGlossaryControl upgradeBusinessGlossaryControl;

	private bool isModuleDropdownLoaded;

	private Dictionary<string, string> customFieldsTableViewStructureForHistory = new Dictionary<string, string>();

	private ObjectWithTitleAndHTMLDescriptionHistory tableTitleAndDescriptionHistory = new ObjectWithTitleAndHTMLDescriptionHistory();

	private Dictionary<int, Dictionary<string, string>> customFieldsColumnForHistory = new Dictionary<int, Dictionary<string, string>>();

	private Dictionary<int, ObjectWithTitleAndDescription> titleAndDescriptionColumnsForHistory = new Dictionary<int, ObjectWithTitleAndDescription>();

	private Dictionary<int, Dictionary<string, string>> customFieldsTriggersForHistory = new Dictionary<int, Dictionary<string, string>>();

	private Dictionary<int, ObjectWithTitleAndDescription> titleAndDescriptionTriggersForHistory = new Dictionary<int, ObjectWithTitleAndDescription>();

	private IContainer components;

	private XtraTabControl tableXtraTabControl;

	private XtraTabPage tableDescriptionXtraTabPage;

	private NonCustomizableLayoutControl tableLayoutControl;

	private TextEdit tableTitleTextEdit;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem titleLayoutControlItem;

	private XtraTabPage tableColumnListXtraTabPage;

	private GridControl tableColumnsGrid;

	private GridColumn nameTableColumnsGridColumn;

	private GridColumn titleTableColumnsGridColumn;

	private GridColumn dataTypeTableColumnsGridColumn;

	private XtraTabPage tableRelationsXtraTabPage;

	private GridColumn descriptionTableColumnsGridColumn;

	private GridColumn iconTableColumnsGridColumn;

	private RepositoryItemPictureEdit iconTableColumnRepositoryItemPictureEdit;

	private GridColumn keyTableColumnsGridColumn;

	private RepositoryItemPictureEdit keyTableColumnRepositoryItemPictureEdit;

	private DXErrorProvider relationNameErrorProvider;

	private XtraTabPage tableTriggersXtraTabPage;

	private GridColumn nullableTableColumnsGridColumn;

	private RepositoryItemCheckEdit checkRepositoryItemCheckEdit;

	private XtraTabPage tableConstraintsXtraTabPage;

	private GridControl tableConstraintsGrid;

	private GridColumn nameTableConstraintsGridColumn;

	private HtmlUserControl tabHtmlUserControl;

	private GridColumn iconTableConstraintsGridColumn;

	private ToolTipController tableToolTipController;

	private GridColumn descriptionTableConstraintsGridColumn;

	private EmptySpaceItem emptySpaceItem2;

	private GridColumn ordinalPositionTableColumnsGridColumn;

	private EmptySpaceItem emptySpaceItem1;

	private InfoUserControl tableStatusUserControl;

	private RepositoryItemAutoHeightMemoEdit descriptionColumnRepositoryItemMemoEdit;

	private RepositoryItemAutoHeightMemoEdit descriptionConstraintRepositoryItemMemoEdit;

	private XtraTabPage metadataXtraTabPage;

	private NonCustomizableLayoutControl metadataLayoutControl;

	private LabelControl dbmsLastUpdatedLabel;

	private LayoutControlGroup layoutControlGroup2;

	private LayoutControlItem dbmsLastUpdatedLayoutControlItem;

	private EmptySpaceItem emptySpaceItem3;

	private LabelControl lastSynchronizedLabel;

	private LayoutControlItem lastImportedLayoutControlItem;

	private LabelControl lastUpdatedLabelControl;

	private LayoutControlItem lastUpdatedLayoutControlItem;

	private RepositoryItemCustomTextEdit titleColumnRepositoryItemCustomTextEdit;

	private RepositoryItemCustomTextEdit nameRepositoryItemCustomTextEdit;

	private ToolTipController toolTipController;

	private XtraTabPage dependenciesXtraTabPage;

	private ImageCollection treeMenuImageCollection;

	private DependenciesUserControl dependenciesUserControl;

	private GridControl tableRelationsGrid;

	private GridColumn nameTableRelationsGridColumn;

	private RepositoryItemCustomTextEdit nameRalationRepositoryItemCustomTextEdit;

	private GridColumn fkTableTableRelationsGridColumn;

	private GridColumn iconTableRelationsGridColumn;

	private RepositoryItemPictureEdit tableRelationRepositoryItemPictureEdit;

	private GridColumn pkTableTableRelationsGridColumn;

	private GridColumn descriptionTableRelationsGridColumn;

	private RepositoryItemAutoHeightMemoEdit descriptionRelationRepositoryItemMemoEdit;

	private GridColumn joinGridColumn;

	private RepositoryItemAutoHeightMemoEdit joinRepositoryItemAutoHeightMemoEdit;

	private RepositoryItemPictureEdit typeRepositoryItemPictureEdit;

	private GridColumn columnsTableConstraintsGridColumn;

	private GridColumn titleTableRelationsGridColumn;

	private RepositoryItemTextEdit titleRelationRepositoryItemTextEdit;

	private GridColumn identityTableColumnsGridColumn;

	private GridColumn defaultComputedTableColumnsGridColumn;

	private RepositoryItemTextEdit relationTitleRepositoryItemTextEdit;

	private RepositoryItemCustomTextEdit FKTableObjectNameCustomTextEdit;

	private RepositoryItemCustomTextEdit PKTableObjectNameCustomTextEdit;

	private GridColumn referencesTableColumnsGridColumn;

	private RepositoryItemAutoHeightMemoEdit referencesRepositoryItemAutoHeightMemoEdit;

	private LayoutControlItem layoutControlItem8;

	private CustomFieldsPanelControl customFieldsPanelControl;

	private LayoutControlItem customFieldsLayoutControlItem;

	private BulkCopyGridUserControl tableColumnsGridView;

	private BulkCopyGridUserControl tableConstraintsGridView;

	private BulkCopyGridUserControl tableRelationsGridView;

	private TextEdit nameTextEdit;

	private LayoutControlItem nameLayoutControlItem;

	private TextEdit tableTextEdit;

	private GridPanelUserControl columnsGridPanelUserControl;

	private GridPanelUserControl relationsGridPanelUserControl;

	private GridPanelUserControl constraintsGridPanelUserControl;

	private GridPanelUserControl triggersGridPanelUserControl;

	private TextEdit documentationTextEdit;

	private TextEdit schemaTextEdit;

	private LayoutControlItem schemaLayoutControlItem;

	private LayoutControlItem documentationLayoutControlItem;

	private LabelControl dbmsCreatedLabelControl;

	private LayoutControlItem dbmsCreatedLayoutControlItem;

	private LabelControl createdLabelControl;

	private LayoutControlItem firstImportedLayoutControlItem;

	private GridColumn createdGridColumn;

	private GridColumn createdByGridColumn;

	private GridColumn lastUpdatedGridColumn;

	private GridColumn lastUpdatedByGridColumn;

	private DocumentationModulesUserControl documentationModulesUserControl;

	private LayoutControlItem modulesLayoutControlItem;

	private XtraTabPage schemaImportsAndChangesXtraTabPage;

	private SchemaImportsAndChangesUserControl schemaImportsAndChangesUserControl;

	private GridColumn dataLinksGridColumn;

	private RepositoryItemAutoHeightMemoEdit dataLinksRepositoryItemAutoHeightMemoEdit;

	private XtraTabPage dataLinksXtraTabPage;

	private GridPanelUserControl dataLinksGridPanelUserControl;

	private GridControl dataLinksGrid;

	private BulkCopyGridUserControl dataLinksGridView;

	private GridColumn objectIconDataLinksGridColumn;

	private RepositoryItemPictureEdit iconRepositoryItemPictureEdit;

	private GridColumn tableColumnDataLinksGridColumn;

	private RepositoryItemAutoHeightMemoEdit repositoryItemAutoHeightMemoEdit1;

	private GridColumn objectDataLinksGridColumn;

	private GridColumn typeDataLinksGridColumn;

	private GridColumn createdDataLinksGridColumn;

	private GridColumn createdByDataLinksGridColumn;

	private GridColumn lastUpdatedDataLinksGridColumn;

	private GridColumn lastUpdatedByDataLinksGridColumn;

	private GridColumn termIconDataLinksGridColumn;

	private LinkUserControl termsLinkUserControl;

	private LabelControl searchCountLabelControl;

	private LabelControl descriptionLabelControl;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private SplitContainerControl triggersSplitContainerControl;

	private GridControl tableTriggerGrid;

	private BulkCopyGridUserControl tableTriggerGridView;

	private GridColumn iconTableTriggersGridColumn;

	private RepositoryItemPictureEdit iconTriggerRepositoryItemPictureEdit;

	private GridColumn nameTableTriggersGridColumn;

	private GridColumn whenTableTriggersGridColumn;

	private GridColumn descriptionTableTriggersGridColumn;

	private RepositoryItemAutoHeightMemoEdit descriptionTriggerRepositoryItemMemoEdit;

	private RepositoryItemCheckEdit onInsertRepositoryItemCheckEdit;

	private RepositoryItemCheckEdit onUpdateRepositoryItemCheckEdit;

	private RepositoryItemCheckEdit onDeleteRepositoryItemCheckEdit;

	private RichEditUserControl triggerScriptRichEditControl;

	private LayoutControlGroup layoutControlGroup3;

	private LayoutControlItem layoutControlItem4;

	private LayoutControlGroup layoutControlGroup4;

	private LayoutControlItem layoutControlItem7;

	private LabelControl scriptLabelControl;

	private LayoutControlItem layoutControlItem5;

	private NonCustomizableLayoutControl layoutControl1;

	private NonCustomizableLayoutControl layoutControl2;

	private XtraTabPage tableScriptXtraTabPage;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl1;

	private LabelControl scriptTextSearchCountLabelControl;

	private LabelControl labelControl2;

	private RichEditUserControl scriptRichEditControl;

	private LayoutControlGroup layoutControlGroup5;

	private LayoutControlItem layoutControlItem6;

	private LayoutControlItem layoutControlItem9;

	private LayoutControlItem layoutControlItem10;

	private BarDockControl barDockControlLeft;

	private BarManager linkedTermsBarManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlRight;

	private BarButtonItem goToObjectLinkedTermsBarButtonItem;

	private BarButtonItem editLinksLinkedTermsBarButtonItem;

	private BarButtonItem removeLinkLinkedTermsBarButtonItem;

	private PopupMenu linkedTermsPopupMenu;

	private BarDockControl barDockControl3;

	private BarManager uniqueKeysBarManager;

	private BarDockControl barDockControl1;

	private BarDockControl barDockControl2;

	private BarDockControl barDockControl4;

	private PopupMenu uniqueKeysPopupMenu;

	private BarButtonItem addPrimaryKeyUniqueKeysBarButtonItem;

	private BarButtonItem addUniqueKeyUniqueKeysBarButtonItem;

	private BarButtonItem editKeyUniqueKeysBarButtonItem;

	private BarButtonItem removeKeyUniqueKeysBarButtonItem;

	private BarDockControl barDockControl7;

	private BarManager triggersBarManager;

	private BarDockControl barDockControl5;

	private BarDockControl barDockControl6;

	private BarDockControl barDockControl8;

	private BarButtonItem removeTriggerTriggersBarButtonItem;

	private PopupMenu triggersPopupMenu;

	private BarDockControl barDockControl11;

	private BarManager relationsBarManager;

	private BarDockControl barDockControl9;

	private BarDockControl barDockControl10;

	private BarDockControl barDockControl12;

	private PopupMenu relationsPopupMenu;

	private BarButtonItem addRelationRelationsBarButtonItem;

	private BarButtonItem editRelationRelationsBarButtonItem;

	private BarButtonItem removeRelationRelationsBarButtonItem;

	private BarButtonItem goToObjectRelationsBarButtonItem;

	private BarDockControl barDockControl15;

	private BarManager columnsBarManager;

	private BarDockControl barDockControl13;

	private BarDockControl barDockControl14;

	private BarDockControl barDockControl16;

	private PopupMenu columnsPopupMenu;

	private BarButtonItem addRelationColumnsBarButtonItem;

	private BarButtonItem designTableColumnsBarButtonItem;

	private BarButtonItem editLinksColumnsBarButtonItem;

	private BarButtonItem addNewLinkedTermColumnsBarButtonItem;

	private BarButtonItem removeColumnsColumnsBarButtonItem;

	private BarSubItem barSubItem1;

	private ToolTipController metadataToolTipController;

	private RepositoryItemCustomTextEdit columnFullNameRepositoryItemCustomTextEdit;

	private RepositoryItemCustomTextEdit columnFullNameFormattedRepositoryItemCustomTextEdit;

	private RepositoryItemCustomTextEdit fullNameRepositoryItemCustomTextEdit;

	private BarButtonItem profileColumnBarButtonItem;

	private XtraTabPage tableSchemaXtraTabPage;

	private TextEdit tableLocationTextEdit;

	private LayoutControlItem tableLocationLayoutControlItem;

	private NonCustomizableLayoutControl schemaTextLayoutControl;

	private RichEditUserControl schemaTextRichEditUserControl;

	private LayoutControlGroup layoutControlGroup6;

	private LayoutControlItem layoutControlItem11;

	private LabelControl schemaTextSearchCountLabelControl;

	private LabelControl schemaTextLabelControl;

	private LayoutControlItem layoutControlItem12;

	private LayoutControlItem layoutControlItem13;

	private GridColumn sparklineRowDistributionColumn;

	private GridColumn sparklineTopValuesColumn;

	private ToolTipController rowDistributionSparkLineToolTipController;

	private BarButtonItem clearColumnProfilingDataBarButtonItem;

	private XtraTabPage dataLineageXtraTabPage;

	private DataLineageUserControl dataLineageUserControl;

	private BarButtonItem viewHistoryBarButtonItem;

	private RepositoryItemAutoHeightMemoEdit comutedRepositoryItemAutoHeightMemoEdit;

	private BarButtonItem viewHistoryTriggersBarButtonItem;

	private BarButtonItem viewHistoryUniqueKeysBarButtonItem;

	private BarButtonItem viewHistoryRelationsBarButtonItem;

	public override int DatabaseId => databaseId ?? (-1);

	public override int ObjectModuleId => moduleId ?? (-1);

	public override int ObjectId => tableId;

	public override string ObjectSchema => schema;

	public override string ObjectName => name;

	public override HtmlUserControl DescriptionHtmlUserControl => tabHtmlUserControl;

	public override XtraTabPage SchemaImportsAndChangesXtraTabPage => schemaImportsAndChangesXtraTabPage;

	public override SchemaImportsAndChangesUserControl SchemaImportsAndChangesUserControl => schemaImportsAndChangesUserControl;

	public override CustomFieldsPanelControl CustomFieldsPanelControl => customFieldsPanelControl;

	public SuggestedDescriptionManager ColumnSuggestedDescriptionManager { get; protected set; }

	public SuggestedDescriptionManager RelationSuggestedDescriptionManager { get; protected set; }

	public SuggestedDescriptionManager KeySuggestedDescriptionManager { get; protected set; }

	public SuggestedDescriptionManager TriggerSuggestedDescriptionManager { get; protected set; }

	private IEnumerable<int> allDeletedRelations => deletedRelationsRows.Union(dependenciesUserControl.DeletedRelations);

	public int TableId => tableId;

	public bool isTableEdited
	{
		get
		{
			return _isTableEdited;
		}
		set
		{
			if (!base.DisableSettingAsEdited && _isTableEdited != value)
			{
				_isTableEdited = value;
				SetTabPageTitle(_isTableEdited, tableDescriptionXtraTabPage, base.Edit);
			}
		}
	}

	public BindingList<ColumnRow> TableColumns => tableColumnsGrid.DataSource as BindingList<ColumnRow>;

	public BindingList<RelationRow> RelationRows => tableRelationsGrid.DataSource as BindingList<RelationRow>;

	public BindingList<TriggerRow> TriggerRows => tableTriggerGrid.DataSource as BindingList<TriggerRow>;

	public BindingList<UniqueConstraintRow> ConstraintsRows => tableConstraintsGrid.DataSource as BindingList<UniqueConstraintRow>;

	public override BaseView VisibleGridView => GetVisibleGridControl()?.MainView;

	public IDragRowsBase ColumnsDragRows { get; set; }

	public event EventHandler AddModuleEvent;

	public TableUserControl(MetadataEditorUserControl control)
		: base(control)
	{
		InitializeComponent();
		Initialize();
		MetadataToolTip.SetLayoutControlItemToolTip(dbmsCreatedLayoutControlItem, "dbms_created");
		MetadataToolTip.SetLayoutControlItemToolTip(dbmsLastUpdatedLayoutControlItem, "dbms_last_updated");
		MetadataToolTip.SetLayoutControlItemToolTip(firstImportedLayoutControlItem, "first_imported");
		MetadataToolTip.SetLayoutControlItemToolTip(lastImportedLayoutControlItem, "last_imported");
		MetadataToolTip.SetLayoutControlItemToolTip(lastUpdatedLayoutControlItem, "last_updated");
		suggestedValuesContextMenu = new PopupMenu
		{
			Manager = new BarManager(),
			DrawMenuSideStrip = DefaultBoolean.False
		};
		base.Edit = new Edit(tableTextEdit);
		idEventArgs = new IdEventArgs();
		documentationModulesUserControl.LoadData(idEventArgs);
		ObjectEventArgs = new ObjectEventArgs();
		dependenciesUserControl.RelationsGridControl = tableRelationsGrid;
		dependenciesUserControl.SetRelationsPageAsModified = SetRelationPageAsModified;
		LengthValidation.SetTitleOrNameLengthLimit(tableTitleTextEdit);
		LengthValidation.SetTitleOrNameLengthLimit(titleColumnRepositoryItemCustomTextEdit);
		LengthValidation.SetTitleOrNameLengthLimit(relationTitleRepositoryItemTextEdit);
		base.UserControlHelpers = new UserControlHelpers(4);
		dependenciesUserControl.DependeciesChanged += delegate
		{
			RefreshDependencies(forceRefresh: true, refreshImmediatelyIfLoaded: true);
		};
		dataLineageUserControl.DataLineageEdited += DataLineageUserControl_DataLineageEdited;
		tableColumnsGridView.SetRowCellValue(-2147483646, "iconTableColumnsGridColumn", Resources.blank_16);
		tableColumnsGridView.SetRowCellValue(-2147483646, "UniqueConstraintIcon", Resources.blank_16);
		tableRelationsGridView.SetRowCellValue(-2147483646, "iconTableRelationsGridColumn", Resources.blank_16);
		tableConstraintsGridView.SetRowCellValue(-2147483646, "ImageFile", Resources.blank_16);
		tableTriggerGridView.SetRowCellValue(-2147483646, "iconTableColumnsGridColumn", Resources.blank_16);
		customFieldsCellsTypesSupportForGrids = new CustomFieldsCellsTypesSupport(isForSummaryTable: false);
		SchemaImportsAndChangesSupport = new SchemaImportsAndChangesSupport(this);
		columnsDragDropManager = new ColumnsDragDropManager();
		columnsDragDropManager.AddEvents(tableColumnsGrid);
		dataLinksDragDropManager = new DataLinksDragDropManager();
		dataLinksDragDropManager.AddEvents(dataLinksGrid);
		ColumnsDragRows = new DragRowsBase<ColumnRow>(tableColumnsGridView);
		ColumnsDragRows.AddEvents();
		dataLinksManager = new DataLinksManager(this, tableXtraTabControl, dataLinksXtraTabPage, dataLinksGridPanelUserControl, columnsGridPanelUserControl, dataLinksGrid, dataLinksGridView, tableColumnsGrid, tableColumnsGridView, dataLinksGridColumn, termIconDataLinksGridColumn, objectIconDataLinksGridColumn, linkedTermsPopupMenu, goToObjectLinkedTermsBarButtonItem, editLinksLinkedTermsBarButtonItem, removeLinkLinkedTermsBarButtonItem);
		dataLinksManager.SetEvents(dataLinksGridView, dataLinksGridPanelUserControl, columnsGridPanelUserControl, removeLinkLinkedTermsBarButtonItem, editLinksLinkedTermsBarButtonItem, goToObjectLinkedTermsBarButtonItem, linkedTermsPopupMenu, editLinksColumnsBarButtonItem);
		dataLinksManager.PrepareDataLinksGridPanelUserControl();
		dataLinksManager.PrepareColumnsGridPanelUserControl();
		PrepareKeyGridPanelUserControl();
		PrepareRelationGridPanelUserControl();
		customFieldsSettings = new Dictionary<SharedObjectTypeEnum.ObjectType, GridView>
		{
			[SharedObjectTypeEnum.ObjectType.Column] = tableColumnsGridView,
			[SharedObjectTypeEnum.ObjectType.Relation] = tableRelationsGridView,
			[SharedObjectTypeEnum.ObjectType.Key] = tableConstraintsGridView,
			[SharedObjectTypeEnum.ObjectType.Trigger] = tableTriggerGridView
		};
		tableColumnsGridView.AddColumnToFilterMapping(nameTableColumnsGridColumn, "FullName");
		tableConstraintsGridView.AddColumnToFilterMapping(columnsTableConstraintsGridColumn, "ColumnsString");
		tableRelationsGridView.AddColumnToFilterMapping(joinGridColumn, "JoinColumns");
		dataLinksGridView.AddColumnToFilterMapping(tableColumnDataLinksGridColumn, "FullNameForObject");
		nameTableColumnsGridColumn.SortMode = ColumnSortMode.Custom;
		columnsTableConstraintsGridColumn.SortMode = ColumnSortMode.Custom;
		joinGridColumn.SortMode = ColumnSortMode.Custom;
		tableColumnDataLinksGridColumn.SortMode = ColumnSortMode.Custom;
		tableStatusUserControl.SetShouldLoadColorsAfterLoad(shouldLoadColorsAfterLoad: false);
	}

	private void TableUserControl_Load(object sender, EventArgs e)
	{
		suggestedValuesContextMenu.Manager.Form = base.ParentForm;
		AddEvents();
		tableTriggerGrid.CausesValidation = true;
		tabHtmlUserControl.ContentChangedEvent += tabHtmlUserControl_PreviewKeyDown;
		tabHtmlUserControl.ProgressValueChanged += SetRichEditControlBackground;
		tabHtmlUserControl.IsEditorFocused += SetRichEditControlBackgroundWhileFocused;
		(tableConstraintsGrid.MainView as GridView).CustomDrawCell += TableViewHelpers.TableUserControl_CustomDrawCell;
		schemaTextRichEditUserControl.ContentChangedEvent += schemaTextRichEditUserControl_ContentChangedEvent;
		Sorting.ApplyCustom(tableRelationsGridView, tableConstraintsGridView);
		WorkWithDataedoTrackingHelper.TrackFirstInSessionObjectView();
	}

	public void RefreshDataLinks()
	{
		dataLinksManager.RefreshDataLinks(forceRefresh: true);
		dataLinksManager.RefreshColumnsLinks();
	}

	private void highlightGridView_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
	}

	public void ClearHighlights(bool keepSearchActive)
	{
		base.UserControlHelpers.ClearHighlights(keepSearchActive, tableXtraTabControl, schemaTextEdit, nameTextEdit, tableTitleTextEdit, customFieldsPanelControl.FieldControls);
		tabHtmlUserControl.ClearHighlights();
		triggerScriptRichEditControl.ClearHighlights();
		scriptRichEditControl.ClearHighlights();
		searchCountLabelControl.Text = string.Empty;
		searchCountLabelControl.BackColor = SkinColors.ControlColorFromSystemColors;
		scriptLabelControl.Text = "Script";
		scriptLabelControl.BackColor = SkinColors.ControlColorFromSystemColors;
		scriptTextSearchCountLabelControl.Text = string.Empty;
		scriptTextSearchCountLabelControl.BackColor = SkinColors.ControlColorFromSystemColors;
		schemaTextSearchCountLabelControl.Text = string.Empty;
		schemaTextSearchCountLabelControl.BackColor = SkinColors.ControlColorFromSystemColors;
	}

	public override void SetTabsProgressHighlights()
	{
		base.UserControlHelpers.ClearTabHighlights(tableXtraTabControl);
		if (base.MainControl.ShowProgress)
		{
			CreateKeyValuePairs();
			base.UserControlHelpers.SetTabsProgressHighlights(tableXtraTabControl, base.KeyValuePairs);
		}
	}

	private void CreateKeyValuePairs()
	{
		base.KeyValuePairs.Clear();
		if (base.MainControl.ProgressType.Type == ProgressTypeEnum.TablesAndColumns)
		{
			GetTableAndColumnsProgress();
		}
		else
		{
			GetAllTabsProgress();
		}
	}

	private void GetTableAndColumnsProgress()
	{
		foreach (XtraTabPage tabPage in tableXtraTabControl.TabPages)
		{
			int key = tableXtraTabControl.TabPages.IndexOf(tabPage);
			if (tabPage.Equals(tableDescriptionXtraTabPage))
			{
				KeyValuePair<int, int> value = new KeyValuePair<int, int>(base.CurrentNode.ObjectPoints, base.CurrentNode.TotalObjectPoints);
				base.KeyValuePairs.Add(key, value);
			}
			else if (tabPage.Equals(tableColumnListXtraTabPage))
			{
				KeyValuePair<int, int> value = new KeyValuePair<int, int>(base.CurrentNode.ColumnsPoints, base.CurrentNode.TotalColumnsPoints);
				base.KeyValuePairs.Add(key, value);
			}
		}
	}

	private void GetAllTabsProgress()
	{
		foreach (XtraTabPage tabPage in tableXtraTabControl.TabPages)
		{
			int key = tableXtraTabControl.TabPages.IndexOf(tabPage);
			if (tabPage.Equals(tableDescriptionXtraTabPage))
			{
				KeyValuePair<int, int> value = new KeyValuePair<int, int>(base.CurrentNode.ObjectPoints, base.CurrentNode.TotalObjectPoints);
				base.KeyValuePairs.Add(key, value);
			}
			else if (tabPage.Equals(tableColumnListXtraTabPage))
			{
				KeyValuePair<int, int> value = new KeyValuePair<int, int>(base.CurrentNode.ColumnsPoints, base.CurrentNode.TotalColumnsPoints);
				base.KeyValuePairs.Add(key, value);
			}
			else if (tabPage.Equals(tableConstraintsXtraTabPage))
			{
				KeyValuePair<int, int> value = new KeyValuePair<int, int>(base.CurrentNode.KeysPoints, base.CurrentNode.TotalKeysPoints);
				base.KeyValuePairs.Add(key, value);
			}
			else if (tabPage.Equals(tableRelationsXtraTabPage))
			{
				KeyValuePair<int, int> value = new KeyValuePair<int, int>(base.CurrentNode.FKRelationsPoints + base.CurrentNode.PKRelationsPoints, base.CurrentNode.TotalFKRelationsPoints + base.CurrentNode.TotalPKRelationsPoints);
				base.KeyValuePairs.Add(key, value);
			}
			else if (tabPage.Equals(tableTriggersXtraTabPage))
			{
				KeyValuePair<int, int> value = new KeyValuePair<int, int>(base.CurrentNode.TriggersPoints, base.CurrentNode.TotalTriggersPoints);
				base.KeyValuePairs.Add(key, value);
			}
		}
	}

	public override void FillControlProgressHighlights()
	{
		if (base.MainControl.ShowProgress)
		{
			base.UserControlHelpers.SetProgressHighlights(customFieldsPanelControl.FieldControls, base.progressType.FieldName, tableDescriptionXtraTabPage);
		}
	}

	public void ForceLayoutChange(bool forceAll = false)
	{
		if (!forceAll)
		{
			if (tableXtraTabControl.SelectedTabPageIndex == 1)
			{
				tableColumnsGridView.LayoutChanged();
			}
			else if (tableXtraTabControl.SelectedTabPageIndex == 2)
			{
				tableRelationsGridView.LayoutChanged();
			}
			else if (tableXtraTabControl.SelectedTabPageIndex == 3)
			{
				tableConstraintsGridView.LayoutChanged();
			}
			else if (tableXtraTabControl.SelectedTabPageIndex == 4)
			{
				tableTriggerGridView.LayoutChanged();
			}
		}
		else
		{
			tableColumnsGridView.LayoutChanged();
			tableRelationsGridView.LayoutChanged();
			tableConstraintsGridView.LayoutChanged();
			tableTriggerGridView.LayoutChanged();
		}
	}

	public void SetDisableSettingAsEdited(bool value)
	{
		base.DisableSettingAsEdited = value;
	}

	public void ClearTabPageTitle()
	{
		tableDescriptionXtraTabPage.Text = CommonFunctionsPanels.SetTitle(isEdited: false, tableDescriptionXtraTabPage.Text);
	}

	public void SetTab(ResultItem row, SharedObjectTypeEnum.ObjectType? type, bool changeTab, string[] searchWords, List<CustomFieldSearchItem> customFieldSearchItems, params int?[] elementId)
	{
		int num = 0;
		if (!type.HasValue)
		{
			num = 0;
			base.UserControlHelpers.SetHighlight(row, searchWords, customFieldSearchItems, null, num, tableXtraTabControl.TabPages.IndexOf(tableScriptXtraTabPage), tableXtraTabControl.TabPages.IndexOf(tableSchemaXtraTabPage), tableXtraTabControl, schemaTextEdit, nameTextEdit, tableTitleTextEdit, customFieldsPanelControl.FieldControls, tabHtmlUserControl, scriptRichEditControl, schemaTextRichEditUserControl, null, null);
			searchCountLabelControl.Text = tabHtmlUserControl.Occurrences;
			BaseSkin.SetSearchHighlightOrDefault(searchCountLabelControl, tabHtmlUserControl.OccurrencesCount > 0);
			scriptTextSearchCountLabelControl.Text = scriptRichEditControl.Occurrences;
			BaseSkin.SetSearchHighlightOrDefault(scriptTextSearchCountLabelControl, scriptRichEditControl.OccurrencesCount > 0);
			schemaTextSearchCountLabelControl.Text = schemaTextRichEditUserControl.Occurrences;
			BaseSkin.SetSearchHighlightOrDefault(schemaTextSearchCountLabelControl, schemaTextRichEditUserControl.OccurrencesCount > 0);
		}
		else if (type.Value == SharedObjectTypeEnum.ObjectType.Column)
		{
			num = tableXtraTabControl.TabPages.IndexOf(tableColumnListXtraTabPage);
			base.UserControlHelpers.SetHighlight(searchWords, customFieldSearchItems, 0, tableColumnsGridView, tableXtraTabControl, num, "column_id", elementId);
		}
		else if (type.Value == SharedObjectTypeEnum.ObjectType.Relation)
		{
			num = tableXtraTabControl.TabPages.IndexOf(tableRelationsXtraTabPage);
			base.UserControlHelpers.SetHighlight(searchWords, customFieldSearchItems, 1, tableRelationsGridView, tableXtraTabControl, num, "table_relation_id", elementId);
		}
		else if (type.Value == SharedObjectTypeEnum.ObjectType.Key)
		{
			num = tableXtraTabControl.TabPages.IndexOf(tableConstraintsXtraTabPage);
			base.UserControlHelpers.SetHighlight(searchWords, customFieldSearchItems, 2, tableConstraintsGridView, tableXtraTabControl, num, "unique_constraint_id", elementId);
		}
		else if (type.Value == SharedObjectTypeEnum.ObjectType.Trigger)
		{
			num = tableXtraTabControl.TabPages.IndexOf(tableTriggersXtraTabPage);
			base.UserControlHelpers.SetHighlight(searchWords, customFieldSearchItems, 3, tableTriggerGridView, tableXtraTabControl, num, "trigger_id", elementId);
			RefreshTriggerDetailPanel();
		}
		if (changeTab)
		{
			tableXtraTabControl.SelectedTabPageIndex = num;
		}
	}

	public override void SetRichEditControlBackground()
	{
		DescriptionHtmlUserControl.SetEmptyProgressBackgroundColor(base.MainControl.ShowProgress && (base.MainControl.ProgressType.Type == ProgressTypeEnum.AllDocumentations || base.MainControl.ProgressType.Type == ProgressTypeEnum.TablesAndColumns));
	}

	public override void SetRichEditControlBackgroundWhileFocused()
	{
		DescriptionHtmlUserControl.SetFocusedColor(base.MainControl.ShowProgress && (base.MainControl.ProgressType.Type == ProgressTypeEnum.AllDocumentations || base.MainControl.ProgressType.Type == ProgressTypeEnum.TablesAndColumns));
	}

	public void ChangeTab(SharedObjectTypeEnum.ObjectType? type)
	{
		if (type.HasValue)
		{
			switch (type.GetValueOrDefault())
			{
			case SharedObjectTypeEnum.ObjectType.Table:
			case SharedObjectTypeEnum.ObjectType.View:
				tableXtraTabControl.SelectedTabPage = tableDescriptionXtraTabPage;
				break;
			case SharedObjectTypeEnum.ObjectType.Column:
				tableXtraTabControl.SelectedTabPage = tableColumnListXtraTabPage;
				break;
			case SharedObjectTypeEnum.ObjectType.Trigger:
				tableXtraTabControl.SelectedTabPage = tableTriggersXtraTabPage;
				break;
			case SharedObjectTypeEnum.ObjectType.Relation:
				tableXtraTabControl.SelectedTabPage = tableRelationsXtraTabPage;
				break;
			case SharedObjectTypeEnum.ObjectType.Key:
				tableXtraTabControl.SelectedTabPage = tableConstraintsXtraTabPage;
				break;
			case SharedObjectTypeEnum.ObjectType.Dependency:
				tableXtraTabControl.SelectedTabPage = dependenciesXtraTabPage;
				break;
			case SharedObjectTypeEnum.ObjectType.DataLink:
				tableXtraTabControl.SelectedTabPage = dataLinksXtraTabPage;
				break;
			case SharedObjectTypeEnum.ObjectType.Script:
				tableXtraTabControl.SelectedTabPage = tableScriptXtraTabPage;
				break;
			}
		}
	}

	private void SetFunctionality()
	{
		SchemaImportsAndChangesUserControl.SetFunctionality();
		SetBusinessGlossaryFunctionality();
		SetClearAllProfilingDataButtonVisibility();
	}

	private void SetClearAllProfilingDataButtonVisibility()
	{
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
		{
			clearColumnProfilingDataBarButtonItem.Visibility = BarItemVisibility.Never;
		}
		else
		{
			clearColumnProfilingDataBarButtonItem.Visibility = BarItemVisibility.Always;
		}
	}

	private void SetBusinessGlossaryFunctionality()
	{
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary))
		{
			GridPanelUserControl gridPanelUserControl = dataLinksGridPanelUserControl;
			bool visible = (dataLinksGrid.Visible = false);
			gridPanelUserControl.Visible = visible;
			if (upgradeBusinessGlossaryControl == null)
			{
				upgradeBusinessGlossaryControl = new UpgradeBusinessGlossaryControl
				{
					Dock = DockStyle.Fill,
					Visible = true
				};
				dataLinksXtraTabPage.Controls.Add(upgradeBusinessGlossaryControl);
			}
			termsLinkUserControl.Visible = false;
		}
		else
		{
			if (upgradeBusinessGlossaryControl != null)
			{
				upgradeBusinessGlossaryControl.Visible = false;
			}
			termsLinkUserControl.Visible = false;
			GridPanelUserControl gridPanelUserControl2 = dataLinksGridPanelUserControl;
			bool visible = (dataLinksGrid.Visible = true);
			gridPanelUserControl2.Visible = visible;
		}
	}

	private void AddEvents()
	{
		AddEventsForDeleting(SharedObjectTypeEnum.ObjectType.Column, tableColumnsGridView, tableColumnListXtraTabPage, base.Edit, columnsPopupMenu, removeColumnsColumnsBarButtonItem, isObject: true, deletedColumnsRows);
		AddEventsForDeleting(SharedObjectTypeEnum.ObjectType.Relation, tableRelationsGridView, tableRelationsXtraTabPage, base.Edit, relationsPopupMenu, removeRelationRelationsBarButtonItem, isObject: true, deletedRelationsRows, ommitDeletingEvents: true);
		AddEventsForDeleting(SharedObjectTypeEnum.ObjectType.Key, tableConstraintsGridView, tableConstraintsXtraTabPage, base.Edit, uniqueKeysPopupMenu, removeKeyUniqueKeysBarButtonItem, isObject: true, deletedUniqueConstraintRows);
		AddEventsForDeleting(SharedObjectTypeEnum.ObjectType.Trigger, tableTriggerGridView, tableTriggersXtraTabPage, base.Edit, triggersPopupMenu, removeTriggerTriggersBarButtonItem, isObject: true, deletedTriggersRows);
		CommonFunctionsPanels.AddEventForColoringDeletedRows(tableConstraintsGridView, isObject: true);
		CommonFunctionsPanels.AddEventForColoringDeletedRows(tableRelationsGridView, isObject: true);
		List<ToolTipData> list = new List<ToolTipData>();
		list.Add(new ToolTipData(tableConstraintsGrid, SharedObjectTypeEnum.ObjectType.Key, iconTableConstraintsGridColumn.VisibleIndex));
		list.Add(new ToolTipData(tableRelationsGrid, SharedObjectTypeEnum.ObjectType.Relation, iconTableRelationsGridColumn.VisibleIndex));
		list.Add(new ToolTipData(tableColumnsGrid, SharedObjectTypeEnum.ObjectType.Column, iconTableColumnsGridColumn.VisibleIndex));
		list.Add(new ToolTipData(tableColumnsGrid, SharedObjectTypeEnum.ObjectType.ColumnPK, keyTableColumnsGridColumn.VisibleIndex));
		list.Add(new ToolTipData(tableTriggerGrid, SharedObjectTypeEnum.ObjectType.Trigger, iconTableTriggersGridColumn.VisibleIndex));
		dataLinksManager.AddToolTipDataForColumnsGrid(list, tableColumnsGrid, nameTableColumnsGridColumn, dataLinksGridColumn);
		dataLinksManager.AddToolTipDataForDataLinkGrid(list, dataLinksGrid, termIconDataLinksGridColumn, objectDataLinksGridColumn, typeDataLinksGridColumn, objectIconDataLinksGridColumn);
		dataLinksManager.AddEventForAutoFilterRow();
		CommonFunctionsPanels.AddEventsForToolTips(tableToolTipController, list);
		documentationModulesUserControl.AddModule = this.AddModuleEvent;
		MetadataEditorUserControl mainControl = base.MainControl;
		mainControl.GetSuggestedDescriptions = (EventHandler)Delegate.Combine(mainControl.GetSuggestedDescriptions, new EventHandler(LoadSuggestedDescriptions));
		dependenciesUserControl.AddEvents(base.MainControl);
		tableXtraTabControl.SelectedPageChanged += delegate
		{
			ColumnsButtonsVisibleChanged(null, null);
		};
		tableXtraTabControl.SelectedPageChanged += delegate
		{
			RelationButtonsVisibleChanged(null, null);
		};
		tableXtraTabControl.SelectedPageChanged += delegate
		{
			ConstraintButtonsVisibleChanged(null, null);
		};
		tableXtraTabControl.SelectedPageChanged += delegate
		{
			DependencyButtonsVisibleChanged(null, null);
		};
		tableXtraTabControl.SelectedPageChanged += delegate
		{
			DataLinksButtonsVisibleChanged(null, null);
		};
		tableXtraTabControl.SelectedPageChanged += delegate
		{
			DataLineageButtonsVisibleChanged(null, null);
		};
		tableXtraTabControl.SelectedPageChanged += delegate
		{
			HideCustomization();
		};
		tableRelationsGridView.FocusedRowChanged += RelationButtonsVisibleChanged;
		tableConstraintsGridView.FocusedRowChanged += ConstraintButtonsVisibleChanged;
		tableColumnsGridView.FocusedRowChanged += ColumnsButtonsVisibleChanged;
		tableTriggerGridView.FocusedRowChanged += TriggersButtonsVisibleChanged;
		CommonFunctionsPanels.AddEventForAutoFilterRow(tableColumnsGridView);
		CommonFunctionsPanels.AddEventForAutoFilterRow(tableRelationsGridView);
		CommonFunctionsPanels.AddEventForAutoFilterRow(tableConstraintsGridView);
		CommonFunctionsPanels.AddEventForAutoFilterRow(tableTriggerGridView);
	}

	public void EditRelation()
	{
		int? relationId = GetFocusedRelationRow()?.Id;
		if (!base.MainControl.ContinueAfterPossibleChanges())
		{
			return;
		}
		RelationRow relationRow = RelationRows.FirstOrDefault((RelationRow x) => x.Id == relationId);
		BindingList<RelationColumnRow> columns = relationRow.Columns;
		if (relationRow == null)
		{
			return;
		}
		RefreshColumns(currentTableRow, forceRefresh: false, refreshImmediatelyIfNotLoaded: true);
		DialogResult num = new RelationForm(TableColumns, relationRow, databaseId.Value, databaseType, tableId, base.ContextShowSchema).ShowDialog(this, setCustomMessageDefaultOwner: true);
		CommonFunctionsPanels.RefreshDataAndWidths(tableColumnsGridView);
		if (num == DialogResult.OK)
		{
			CommonFunctionsPanels.SetBestFitForColumns(tableRelationsGridView);
			if (relationRow.RowState != 0)
			{
				SetRelationPageAsModified();
			}
			if (relationRow.FKTableId != tableId && relationRow.PKTableId != tableId)
			{
				int rowHandle = tableRelationsGridView.LocateByValue("Id", relationRow.Id);
				tableRelationsGridView.DeleteRow(rowHandle);
			}
			ReferenceManager.RemoveReferences(relationRow, TableColumns, columns);
			ReferenceManager.AddReferences(TableColumns, relationRow);
			tableColumnsGridView.RefreshData();
		}
		if (RelationRows.All((RelationRow x) => x.RowState != ManagingRowsEnum.ManagingRows.Updated))
		{
			SetRelationPageAsUnchanged();
		}
	}

	public void EditTable()
	{
		if (base.MainControl.ContinueAfterPossibleChanges())
		{
			DesignTableForm designTableForm = new DesignTableForm(databaseId.Value, tableId, schema, name, title, source, ObjectType, base.Subtype, databaseName, base.CustomFieldsSupport);
			if (designTableForm.ShowDialog() == DialogResult.OK)
			{
				RefreshColumnsTable();
				RefreshDataLinks();
				schema = designTableForm.TableDesigner.Schema;
				title = designTableForm.TableDesigner.Title;
				name = designTableForm.TableDesigner.Name;
				nameTextEdit.Text = name;
				schemaTextEdit.Text = schema;
				source = designTableForm.TableDesigner.Source;
				SetTitleTextEdit(title);
				base.Subtype = designTableForm.TableDesigner.Type;
			}
		}
	}

	public void AddRelation(IEnumerable<ColumnRow> columns = null)
	{
		if (base.MainControl.ContinueAfterPossibleChanges())
		{
			RelationRow relationRow = new RelationRow(tableId, base.CustomFieldsSupport);
			if (databaseId.HasValue)
			{
				relationRow.FKTableDatabaseId = databaseId.Value;
				relationRow.PKTableDatabaseId = databaseId.Value;
			}
			RefreshColumns(currentTableRow, forceRefresh: false, refreshImmediatelyIfNotLoaded: true);
			RelationForm relationForm = new RelationForm(TableColumns, relationRow, columns?.Where((ColumnRow x) => x.Status != SynchronizeStateEnum.SynchronizeState.Deleted), databaseId.Value, databaseType, tableId, base.ContextShowSchema, editMode: false);
			DialogResult num = relationForm.ShowDialog(this, setCustomMessageDefaultOwner: true);
			CommonFunctionsPanels.RefreshDataAndWidths(tableColumnsGridView);
			if (num == DialogResult.OK)
			{
				tableRelationsGrid.BeginUpdate();
				RefeshRelationsTable(forceRefresh: false, refreshImmediatelyIfLoaded: false, refreshImmediatelyIfNotLoaded: true);
				(tableRelationsGrid.DataSource as BindingList<RelationRow>).Add(relationRow);
				CommonFunctionsPanels.SetBestFitForColumns(tableRelationsGridView);
				relationRow.RowState = ManagingRowsEnum.ManagingRows.Added;
				relationRow.Source = UserTypeEnum.UserType.USER;
				relationRow.FKSubtype = relationForm.FkSubtype;
				relationRow.PKSubtype = relationForm.PkSubtype;
				relationRow.PKSource = relationForm.PkSource;
				relationRow.FKSource = relationForm.FkSource;
				relationRow.PKStatus = relationForm.PKStatus;
				relationRow.FKStatus = relationForm.FKStatus;
				ReferenceManager.AddReferences(TableColumns, relationRow);
				tableColumnsGridView.RefreshData();
				RefeshRelationsTable(forceRefresh: true);
				RefreshColumns(currentTableRow, forceRefresh: true);
				tableRelationsGrid.EndUpdate();
			}
		}
	}

	public void EditConstraint()
	{
		int? constraintId = GetFocusedUniqueConstraintRow()?.Id;
		if (!base.MainControl.ContinueAfterPossibleChanges())
		{
			return;
		}
		UniqueConstraintRow uniqueConstraintRow = ConstraintsRows.FirstOrDefault((UniqueConstraintRow x) => x.Id == constraintId);
		if (uniqueConstraintRow == null || !uniqueConstraintRow.IsEditable)
		{
			return;
		}
		BindingList<UniqueConstraintColumnRow> columns = uniqueConstraintRow.Columns;
		RefreshColumns(currentTableRow, forceRefresh: false, refreshImmediatelyIfNotLoaded: true);
		if (new ConstraintForm(uniqueConstraintRow, TableColumns, databaseId.Value, tableId, name).ShowDialog(this, setCustomMessageDefaultOwner: true) == DialogResult.OK)
		{
			CommonFunctionsPanels.SetBestFitForColumns(tableConstraintsGridView);
			if (uniqueConstraintRow.RowState != 0)
			{
				SetConstraintPageAsModified();
			}
			UniqueConstraintManager.RemoveUnnecessaryIcons(constraintId ?? (-1), TableColumns, columns);
			UniqueConstraintManager.AddIcons(uniqueConstraintRow, TableColumns);
			tableColumnsGridView.RefreshData();
		}
		if (ConstraintsRows.All((UniqueConstraintRow x) => x.RowState != ManagingRowsEnum.ManagingRows.Updated))
		{
			SetConstraintPageAsUnchanged();
		}
	}

	public void AddConstraint(bool isPK)
	{
		IEnumerable<BaseRow> selectedRows = GetSelectedRows(tableColumnsGridView);
		if (base.MainControl.ContinueAfterPossibleChanges())
		{
			UniqueConstraintRow uniqueConstraintRow = new UniqueConstraintRow(tableId, base.CustomFieldsSupport);
			uniqueConstraintRow.IsPK = isPK;
			RefreshColumns(currentTableRow, forceRefresh: false, refreshImmediatelyIfNotLoaded: true);
			ConstraintForm constraintForm = new ConstraintForm(uniqueConstraintRow, TableColumns, databaseId.Value, tableId, name, editMode: false);
			if (tableXtraTabControl.SelectedTabPage == tableColumnListXtraTabPage)
			{
				constraintForm.SetCheckedColumns(selectedRows);
			}
			if (constraintForm.ShowDialog(this, setCustomMessageDefaultOwner: true) == DialogResult.OK)
			{
				CommonFunctionsPanels.SetBestFitForColumns(tableConstraintsGridView);
				(tableConstraintsGrid?.DataSource as BindingList<UniqueConstraintRow>)?.Add(uniqueConstraintRow);
				UniqueConstraintManager.AddIcons(uniqueConstraintRow, TableColumns);
				tableColumnsGridView.RefreshData();
				uniqueConstraintRow.RowState = ManagingRowsEnum.ManagingRows.Unchanged;
			}
		}
	}

	public void SetSchemNameAndTitle(string schema, string name, string title)
	{
		this.schema = schema;
		this.name = name;
		this.title = title;
	}

	public void SetLabels(string schema, string name, string title, SharedObjectTypeEnum.ObjectType type, SharedObjectSubtypeEnum.ObjectSubtype subtype)
	{
		schemaTextEdit.Text = (this.schema = schema);
		nameTextEdit.Text = (this.name = name);
		tableTitleTextEdit.Text = title;
		RefreshHistorySchemaNameTitle(schema, name, title, SharedObjectTypeEnum.TypeToString(type), SharedObjectSubtypeEnum.TypeToString(type, subtype));
		CommonFunctionsPanels.SetName(tableTextEdit, tableDescriptionXtraTabPage, ObjectType, subtype, this.schema, this.name, this.title, databaseType, databaseTitle, showDatabaseTitle, base.ShowSchema, source);
	}

	private void RefreshHistorySchemaNameTitle(string schema, string name, string title, string objectType, string subType)
	{
		if (tabHtmlUserControl?.TableRow != null)
		{
			tabHtmlUserControl.TableRow.Title = title;
			tabHtmlUserControl.TableRow.Schema = schema;
			tabHtmlUserControl.TableRow.Name = name;
			tabHtmlUserControl.TableRow.ObjectType = objectType;
			tabHtmlUserControl.TableRow.Subtype = subType;
		}
	}

	public void SetTitleTextEdit(string title)
	{
		tableTitleTextEdit.Text = title;
		if (tableTitleAndDescriptionHistory != null)
		{
			tableTitleAndDescriptionHistory.Title = title;
		}
		if (tabHtmlUserControl != null && tabHtmlUserControl.TableRow != null)
		{
			tabHtmlUserControl.TableRow.Title = title;
		}
	}

	public void SetSource(UserTypeEnum.UserType source)
	{
		this.source = source;
	}

	public void SetLocationTextEdit(string location)
	{
		tableLocationTextEdit.Text = location;
	}

	public void SaveColumns()
	{
		if (ObjectType != SharedObjectTypeEnum.ObjectType.UnresolvedEntity)
		{
			UserViewData.SaveColumns(UserViewData.GetViewName(this, tableColumnsGridView, ObjectType), tableColumnsGridView);
		}
	}

	public override void SetParameters(DBTreeNode selectedNode, CustomFieldsSupport customFieldsSupport, Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType? dependencyType = null)
	{
		base.SetParameters(selectedNode, customFieldsSupport, dependencyType);
		try
		{
			base.KeyValuePairs.Clear();
			columnsLoadRequired = false;
			if (ObjectType != selectedNode.ObjectType)
			{
				SaveColumns();
				columnsLoadRequired = true;
			}
			ObjectType = selectedNode.ObjectType;
			schemaLayoutControlItem.Visibility = ((ObjectType == SharedObjectTypeEnum.ObjectType.Structure) ? LayoutVisibility.Never : LayoutVisibility.Always);
			tableLocationLayoutControlItem.Visibility = ((ObjectType != SharedObjectTypeEnum.ObjectType.Structure) ? LayoutVisibility.Never : LayoutVisibility.Always);
			tableTriggersXtraTabPage.PageVisible = ObjectType != SharedObjectTypeEnum.ObjectType.Structure;
			tableScriptXtraTabPage.PageVisible = ObjectType == SharedObjectTypeEnum.ObjectType.View;
			tableSchemaXtraTabPage.PageVisible = ObjectType == SharedObjectTypeEnum.ObjectType.Structure;
			GridColumn gridColumn = defaultComputedTableColumnsGridColumn;
			OptionsColumn optionsColumn = defaultComputedTableColumnsGridColumn.OptionsColumn;
			GridColumn gridColumn2 = identityTableColumnsGridColumn;
			bool flag2 = (identityTableColumnsGridColumn.OptionsColumn.ShowInCustomizationForm = ObjectType != SharedObjectTypeEnum.ObjectType.Structure);
			bool flag4 = (gridColumn2.Visible = flag2);
			bool visible = (optionsColumn.ShowInCustomizationForm = flag4);
			gridColumn.Visible = visible;
			dataLinksManager.SetParameters(base.ShowSchema, selectedNode.Id, selectedNode.BaseName);
			showDatabaseTitle = selectedNode?.ParentNode?.ParentNode != null && selectedNode.ParentNode.ParentNode.HasMultipleDatabases;
			columnsDragDropManager.SetObject((selectedNode.ParentNode.ParentNode.ObjectType == SharedObjectTypeEnum.ObjectType.Database) ? selectedNode.ParentNode.ParentNode : selectedNode.ParentNode.ParentNode.ParentNode, selectedNode, dataLinksManager);
			dataLinksDragDropManager.SetObject((selectedNode.ParentNode.ParentNode.ObjectType == SharedObjectTypeEnum.ObjectType.Database) ? selectedNode.ParentNode.ParentNode : selectedNode.ParentNode.ParentNode.ParentNode, selectedNode, dataLinksManager);
			isTableEdited = false;
			base.DisableSettingAsEdited = true;
			tabHtmlUserControl.CanListen = false;
			base.TabPageChangedProgrammatically = true;
			CommonFunctionsPanels.SetSelectedTabPage(tableXtraTabControl, dependencyType);
			base.TabPageChangedProgrammatically = false;
			ShowRelationButtons();
			ShowPrimaryUniqueButtons();
			ShowConstraintButtons();
			ShowDependencyButtons();
			ShowDataLinkButtons();
			tableId = selectedNode.Id;
			moduleId = ((selectedNode == null || selectedNode.ParentNode?.ParentNode.ObjectType != SharedObjectTypeEnum.ObjectType.Module) ? null : selectedNode?.ParentNode?.ParentNode.Id);
			TableObject dataById = DB.Table.GetDataById(tableId);
			ClearData(selectedNode);
			base.DisableSettingAsEdited = true;
			currentTableRow = dataById;
			if (dataById != null)
			{
				SetTableBasicData(dataById);
				if (databaseId.HasValue)
				{
					databaseType = DatabaseTypeEnum.StringToType(dataById.DatabaseType);
					base.HasMultipleSchemas = dataById.DatabaseMultipleSchemas;
					base.DatabaseShowSchema = dataById.DatabaseDatabaseShowSchema;
					base.DatabaseShowSchemaOverride = dataById.DatabaseShowSchemaOverride;
					databaseName = dataById.DatabaseName;
					databaseTitle = dataById.DatabaseTitle;
					databaseServer = dataById.Host;
				}
				documentationTextEdit.Text = databaseTitle;
				CommonFunctionsPanels.SetName(tableTextEdit, tableDescriptionXtraTabPage, ObjectType, base.Subtype, schema, name, tableTitleTextEdit.Text, databaseType, databaseTitle, showDatabaseTitle, base.ShowSchema, source);
				base.ModulesId = DB.Table.GetTableModules(tableId);
				isModuleDropdownLoaded = false;
				documentationModulesUserControl.ClearData();
				List<ModuleTitleWithDatabaseTitle> objectsTop5ModulesNamesWithDatabase = DB.Table.GetObjectsTop5ModulesNamesWithDatabase(tableId);
				documentationModulesUserControl.SetPopupBarText(string.Join(", ", objectsTop5ModulesNamesWithDatabase.Select((ModuleTitleWithDatabaseTitle m) => m.ModuleTitleWithDatabase)));
				documentationModulesUserControl.Refresh();
				base.CustomFieldsSupport = customFieldsSupport;
				customFields = new CustomFieldContainer(ObjectType, ObjectId, customFieldsSupport);
				customFields.RetrieveCustomFields(dataById);
				customFields.ClearAddedDefinitionValues(null);
				SetCustomFieldsDataSource();
				columnsGridPanelUserControl.Initialize(tableColumnListXtraTabPage.Text);
				constraintsGridPanelUserControl.Initialize(tableConstraintsXtraTabPage.Text);
				relationsGridPanelUserControl.Initialize(tableRelationsXtraTabPage.Text);
				triggersGridPanelUserControl.Initialize(tableTriggersXtraTabPage.Text);
				dataLinksGridPanelUserControl.Initialize(dataLinksXtraTabPage.Text);
				hasLoadedColumnSuggestedDescriptions = false;
				hasLoadedRelationSuggestedDescriptions = false;
				hasLoadedTriggerSuggestedDescriptions = false;
				hasLoadedKeySuggestedDescriptions = false;
				constraintsGridPanelUserControl.Delete += ProcessConstraintDelete;
				relationsGridPanelUserControl.Delete += ProcessRelationDelete;
				columnsGridPanelUserControl.CustomFields += base.EditCustomFieldsFromGridPanel;
				relationsGridPanelUserControl.CustomFields += base.EditCustomFieldsFromGridPanel;
				constraintsGridPanelUserControl.CustomFields += base.EditCustomFieldsFromGridPanel;
				triggersGridPanelUserControl.CustomFields += base.EditCustomFieldsFromGridPanel;
				if (ObjectType == SharedObjectTypeEnum.ObjectType.View)
				{
					RefreshDefinition(dataById);
				}
				if (ObjectType == SharedObjectTypeEnum.ObjectType.Structure)
				{
					RefreshSchemaText(dataById);
				}
				SetProfilingColumnsDataForRowDistributionSparkLine();
				SetListOfDataProfilingTopValues();
				RefreshColumns(dataById, forceRefresh: true);
				RefeshRelationsTable(forceRefresh: true);
				RefreshContraintsTable(forceRefresh: true);
				RefreshTriggersTable(forceRefresh: true);
				RefreshDependencies(forceRefresh: true);
				RefreshDataLineage(forceRefresh: true);
				dataLinksManager.RefreshDataLinks(forceRefresh: true);
				schemaImportsAndChangesUserControl.ClearData();
				RefreshSchemaImportsAndChanges(forceRefresh: true);
				isTableEdited = false;
				CommonFunctionsPanels.ClearTabPagesTitle(tableXtraTabControl, base.Edit);
				tableStatusUserControl.CheckAndSetDeletedObjectProperties(dataById);
			}
			else
			{
				tableStatusUserControl.SetDeletedObjectProperties();
			}
			token = new CancellationTokenSource();
			FillControlProgressHighlights();
			SetRichEditControlBackground();
			SetFunctionality();
			SetColumnGridViewTooltips();
			SetKeysGridViewTooltips();
			documentationLayoutControlItem.OptionsToolTip.ToolTip = "Database/data source in the Dataedo metadata repository ";
			schemaLayoutControlItem.OptionsToolTip.ToolTip = CommonTooltips.GetSchema(ObjectType);
			nameLayoutControlItem.OptionsToolTip.ToolTip = CommonTooltips.GetName(ObjectType);
			titleLayoutControlItem.OptionsToolTip.ToolTip = CommonTooltips.GetTitle(ObjectType);
			descriptionLabelControl.ToolTip = CommonTooltips.GetDescription(ObjectType);
			SetTabsProgressHighlights();
			tableStatusUserControl.Description = "This " + SharedObjectTypeEnum.TypeToString(ObjectType).ToLower() + " has been removed from the database. You can remove it from the repository.";
			if (tableXtraTabControl.SelectedTabPage == tableColumnListXtraTabPage)
			{
				ShowOnboardingAfterOpeningColumnsTab();
			}
			customFieldsTableViewStructureForHistory = HistoryCustomFieldsHelper.GetOldCustomFieldsInObjectUserControl(customFields);
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			base.DisableSettingAsEdited = false;
		}
	}

	private void LoadUserColumns()
	{
		if (columnsLoadRequired && isColumnsDataLoaded)
		{
			if (!UserViewData.LoadColumns(UserViewData.GetViewName(this, tableColumnsGridView, ObjectType), tableColumnsGridView))
			{
				CustomFieldsCellsTypesSupport.SortCustomColumns(tableColumnsGridView);
				CommonFunctionsPanels.SetBestFitForColumns(tableColumnsGridView);
			}
			columnsLoadRequired = false;
		}
	}

	private void SetListOfDataProfilingTopValues()
	{
		try
		{
			ListOfDataProfilingTopValues = DB.DataProfiling.SelectColumnValuesDataObjectForTable(tableId);
		}
		catch (Exception)
		{
			ListOfDataProfilingTopValues = new List<ColumnValuesDataObject>();
		}
	}

	private void SetProfilingColumnsDataForRowDistributionSparkLine()
	{
		try
		{
			profiledColumns = DB.DataProfiling.SelectColumnsProfilingData(tableId);
		}
		catch (Exception)
		{
		}
	}

	private void ShowPrimaryUniqueButtons()
	{
		if (tableXtraTabControl.SelectedTabPage == tableColumnListXtraTabPage)
		{
			base.MainControl.SetAddPrimaryKeyButtonVisibility(new BoolEventArgs(value: true));
			base.MainControl.SetAddUniqueKeyButtonVisibility(new BoolEventArgs(value: true));
		}
		else if (tableXtraTabControl.SelectedTabPage == tableConstraintsXtraTabPage)
		{
			base.MainControl.SetAddPrimaryKeyButtonVisibility(new BoolEventArgs(value: true));
			base.MainControl.SetAddUniqueKeyButtonVisibility(new BoolEventArgs(value: true));
		}
		else
		{
			base.MainControl.SetAddPrimaryKeyButtonVisibility(new BoolEventArgs(value: false));
			base.MainControl.SetAddUniqueKeyButtonVisibility(new BoolEventArgs(value: false));
		}
	}

	private void SetTableBasicData(TableObject tableRow)
	{
		base.Subtype = SharedObjectSubtypeEnum.StringToType(SharedObjectTypeEnum.ObjectType.Table, tableRow.Subtype);
		schemaTextEdit.Text = tableRow.Schema;
		string text2 = (title = (tableTitleTextEdit.Text = tableRow.Title));
		tableLocationTextEdit.Text = tableRow.Location;
		text2 = (name = (nameTextEdit.Text = tableRow.Name));
		schema = tableRow.Schema;
		databaseId = tableRow.DatabaseId;
		source = UserTypeEnum.ObjectToType(tableRow.Source).Value;
		idEventArgs.DatabaseId = databaseId.Value;
		tabHtmlUserControl.HtmlText = tableRow.Description;
		tabHtmlUserControl.ClearHistoryObjects();
		tabHtmlUserControl.TableRow = tableRow;
		tabHtmlUserControl.SplashScreenManager = GetSplashScreenManager();
		tableTitleAndDescriptionHistory = new ObjectWithTitleAndHTMLDescriptionHistory
		{
			ObjectId = tableId,
			Title = tableTitleTextEdit.Text,
			Description = PrepareValue.GetHtmlText(tabHtmlUserControl?.PlainText, tabHtmlUserControl?.HtmlText),
			DescriptionPlain = tabHtmlUserControl?.PlainText
		};
		dbmsCreatedLabelControl.Text = PrepareValue.SetDateTimeWithFormatting(tableRow.DbmsCreationDate);
		createdLabelControl.Text = PrepareValue.SetDateTimeWithFormatting(tableRow.CreationDate) + " " + tableRow.CreatedBy;
		dbmsLastUpdatedLabel.Text = PrepareValue.SetDateTimeWithFormatting(tableRow.DbmsLastModificationDate);
		lastSynchronizedLabel.Text = PrepareValue.SetDateTimeWithFormatting(tableRow.SynchronizationDate) + " " + tableRow.SynchronizedBy;
		lastUpdatedLabelControl.Text = PrepareValue.SetDateTimeWithFormatting(tableRow.LastModificationDate) + " " + tableRow.ModifiedBy;
	}

	private void RefreshColumns(TableObject tableRow, bool forceRefresh = false, bool refreshImmediatelyIfNotLoaded = false)
	{
		if ((IsRefreshRequired(tableColumnListXtraTabPage, !isColumnsDataLoaded, forceRefresh) || (refreshImmediatelyIfNotLoaded && !isColumnsDataLoaded)) && tableRow != null)
		{
			base.MainControl.SetWaitformVisibility(visible: true);
			BindingList<ColumnRow> dataObjectByTableId = DB.Column.GetDataObjectByTableId(base.ContextShowSchema, new ObjectRow(tableRow, base.CustomFieldsSupport), tableId, notDeletedOnly: false, base.CustomFieldsSupport);
			tableColumnsGridView.LeftCoord = 0;
			if (!SetProfilingColumnsAvailability())
			{
				sparklineRowDistributionColumn.Visible = false;
				sparklineTopValuesColumn.Visible = false;
			}
			else
			{
				bool flag = false;
				bool flag2 = false;
				foreach (ColumnRow d in dataObjectByTableId)
				{
					ColumnProfiledDataObject columnProfiledDataObject = profiledColumns.Where(delegate(ColumnProfiledDataObject x)
					{
						if (x.ColumnId == d.Id && x != null)
						{
							_ = x.ColumnId;
							ColumnRow columnRow = d;
							if (columnRow == null)
							{
								return false;
							}
							_ = columnRow.Id;
							return true;
						}
						return false;
					}).FirstOrDefault();
					if (columnProfiledDataObject != null)
					{
						d.SparklineRowDistributionText = columnProfiledDataObject.TextForSparkLine;
						if (!flag)
						{
							flag = columnProfiledDataObject != null && columnProfiledDataObject.RowCount > 0 && ((columnProfiledDataObject != null && columnProfiledDataObject.ValuesDistinctRowCount > 0) || (columnProfiledDataObject != null && columnProfiledDataObject.ValuesNondistinctRowCount > 0) || (columnProfiledDataObject != null && columnProfiledDataObject.ValuesNullRowCount > 0) || (columnProfiledDataObject != null && columnProfiledDataObject.ValuesEmptyRowCount > 0));
						}
						if (!flag2)
						{
							List<ColumnValuesDataObject> listOfDataProfilingTopValues = ListOfDataProfilingTopValues;
							flag2 = listOfDataProfilingTopValues != null && listOfDataProfilingTopValues.Count() > 0;
						}
					}
				}
				if (flag)
				{
					sparklineRowDistributionColumn.VisibleIndex = descriptionTableColumnsGridColumn.VisibleIndex + 1;
				}
				else
				{
					sparklineRowDistributionColumn.Visible = flag;
				}
				if (flag2)
				{
					sparklineTopValuesColumn.VisibleIndex = descriptionTableColumnsGridColumn.VisibleIndex + 2;
				}
				else
				{
					sparklineTopValuesColumn.Visible = flag2;
				}
			}
			tableColumnsGrid.DataSource = dataObjectByTableId;
			BasicRow[] elements = TableColumns?.ToArray();
			HistoryColumnsHelper.SaveColumnsOrParametrsOfOldCustomFields(elements, customFieldsColumnForHistory, titleAndDescriptionColumnsForHistory);
			dataLinksGridColumn.Visible = Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary) && dataObjectByTableId.Any((ColumnRow x) => x.DataLinksDataContainer.HasData);
			ColumnSuggestedDescriptionManager = new SuggestedDescriptionManager(base.ContextShowSchema, tableId, GetFieldNames((CustomFieldRowExtended x) => x.ColumnVisibility && (x.Type == CustomFieldTypeEnum.CustomFieldType.Text || x.Type == CustomFieldTypeEnum.CustomFieldType.ListOpen)), "columns", "table_id", TableColumns?.ToList().Cast<BaseRow>().ToList(), tableColumnsGridView, ObjectType, token);
			SuggestedDescriptionManager columnSuggestedDescriptionManager = ColumnSuggestedDescriptionManager;
			columnSuggestedDescriptionManager.Redraw = (Action)Delegate.Combine(columnSuggestedDescriptionManager.Redraw, new Action(Redraw));
			ColumnSuggestedDescriptionManager.Rows = TableColumns?.ToList().Cast<BaseRow>().ToList();
			hasLoadedColumnSuggestedDescriptions = base.ShowHints;
			foreach (ColumnRow tableColumn in TableColumns)
			{
				tableColumn.SuggestedDescriptions.Clear();
			}
			if (base.ShowHints)
			{
				ColumnSuggestedDescriptionManager.GetSuggestedDescriptions();
			}
			FillHighlights(SharedObjectTypeEnum.ObjectType.Column);
			isColumnsDataLoaded = true;
			LoadUserColumns();
			base.MainControl.SetWaitformVisibility(visible: false);
			columnsGridPanelUserControl.SetDefaultLockButtonVisibility(value: true);
		}
		else if (forceRefresh)
		{
			isColumnsDataLoaded = false;
		}
	}

	private void RefeshRelationsTable(bool forceRefresh = false, bool refreshImmediatelyIfLoaded = false, bool refreshImmediatelyIfNotLoaded = false)
	{
		if (IsRefreshRequired(tableRelationsXtraTabPage, !isRelationsDataLoaded, forceRefresh) || (refreshImmediatelyIfNotLoaded && !isRelationsDataLoaded) || (refreshImmediatelyIfLoaded && isRelationsDataLoaded))
		{
			base.MainControl.SetWaitformVisibility(visible: true);
			BindingList<RelationRow> dataObjectByTableId = DB.Relation.GetDataObjectByTableId(tableId, base.ContextShowSchema, base.CustomFieldsSupport);
			deletedRelationsRows.Clear();
			tableRelationsGridView.LeftCoord = 0;
			tableRelationsGrid.DataSource = dataObjectByTableId;
			RelationSuggestedDescriptionManager = new SuggestedDescriptionManager(base.ContextShowSchema, tableId, GetFieldNames((CustomFieldRowExtended x) => x.RelationVisibility && (x.Type == CustomFieldTypeEnum.CustomFieldType.Text || x.Type == CustomFieldTypeEnum.CustomFieldType.ListOpen)), "tables_relations", "fk_table_id", RelationRows?.ToList().Cast<BaseRow>().ToList(), tableRelationsGridView, ObjectType, token);
			SuggestedDescriptionManager relationSuggestedDescriptionManager = RelationSuggestedDescriptionManager;
			relationSuggestedDescriptionManager.Redraw = (Action)Delegate.Combine(relationSuggestedDescriptionManager.Redraw, new Action(Redraw));
			RelationSuggestedDescriptionManager.Rows = RelationRows?.ToList().Cast<BaseRow>().ToList();
			foreach (RelationRow relationRow in RelationRows)
			{
				relationRow.SuggestedDescriptions.Clear();
			}
			if (base.ShowHints)
			{
				RelationSuggestedDescriptionManager.GetSuggestedDescriptions();
				hasLoadedRelationSuggestedDescriptions = base.ShowHints;
			}
			CommonFunctionsPanels.SetBestFitForColumns(tableRelationsGridView);
			FillHighlights(SharedObjectTypeEnum.ObjectType.Relation);
			isRelationsDataLoaded = true;
			base.MainControl.SetWaitformVisibility(visible: false);
		}
		else if (forceRefresh)
		{
			isRelationsDataLoaded = false;
		}
	}

	private void RefreshDefinition(TableObject tableRow)
	{
		if (databaseType == SharedDatabaseTypeEnum.DatabaseType.Cassandra || databaseType == SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra)
		{
			string cassandraDefinition = GetCassandraDefinition(tableRow);
			ColorizeSyntax(scriptRichEditControl, cassandraDefinition, "SQL");
		}
		else
		{
			string definition = tableRow.Definition;
			if (string.IsNullOrEmpty(definition))
			{
				scriptRichEditControl.Text = GetEmptyScriptMessage(databaseType, source);
			}
			else
			{
				ColorizeSyntax(scriptRichEditControl, definition, tableRow.Language);
			}
		}
		scriptRichEditControl.RefreshSkin();
	}

	private string GetCassandraDefinition(TableObject tableRow)
	{
		string text = "CREATE MATERIALIZED VIEW " + schema + "." + name + Environment.NewLine;
		List<ColumnObject> dataByTable = DB.Column.GetDataByTable(tableId);
		string text2 = null;
		foreach (ColumnObject item in dataByTable)
		{
			text2 = text2 + item.Name + "," + Environment.NewLine;
		}
		if (string.IsNullOrWhiteSpace(text2))
		{
			return string.Empty;
		}
		text2 = text2.Remove(text2.LastIndexOf(","), 1);
		string text3 = "AS SELECT " + Environment.NewLine + text2;
		List<Dataedo.App.Data.MetadataServer.Model.DependencyRow> list = DB.Dependency.GetUses(databaseServer, databaseName, databaseTitle, base.HasMultipleSchemas, base.ShowSchema, base.DatabaseShowSchemaOverride, base.ShowSchema, schema, name, title, name, ObjectType, base.Subtype, "DBMS", new DatabaseInfo(databaseId.Value, databaseType, databaseServer, databaseName, databaseTitle, base.HasMultipleSchemas, base.ShowSchema, base.DatabaseShowSchemaOverride, schema), ObjectId, notDeletedOnly: false, addTriggers: true, maxLevel: 2, currentLevel: 1).ToList();
		string text4 = "FROM " + list.First().Name + Environment.NewLine;
		string text5 = "WHERE " + tableRow.Definition + Environment.NewLine;
		return text + text3 + text4 + text5;
	}

	private void RefreshSchemaText(TableObject tableRow)
	{
		tableSchemaXtraTabPage.PageVisible = !string.IsNullOrEmpty(tableRow.Definition);
		if (!string.IsNullOrEmpty(tableRow.Definition))
		{
			schemaTextRichEditUserControl.Text = tableRow.Definition;
			_ = schemaTextRichEditUserControl.Document;
			schemaTextRichEditUserControl.RefreshSkin();
		}
	}

	public void RefreshSchemaText()
	{
		TableObject dataById = DB.Table.GetDataById(tableId);
		RefreshSchemaText(dataById);
	}

	public void RefreshScriptText()
	{
		TableObject dataById = DB.Table.GetDataById(tableId);
		RefreshScriptText(dataById);
	}

	private void RefreshScriptText(TableObject tableRow)
	{
		tableScriptXtraTabPage.PageVisible = !string.IsNullOrEmpty(tableRow.Definition);
		if (!string.IsNullOrEmpty(tableRow.Definition))
		{
			scriptRichEditControl.Text = tableRow.Definition;
			ColorizeSyntax(scriptRichEditControl, scriptRichEditControl.Text, "SQL");
			scriptRichEditControl.RefreshSkin();
		}
	}

	public void RefreshColumnsTable()
	{
		base.MainControl.SetWaitformVisibility(visible: true);
		BindingList<ColumnRow> dataObjectByTableId = DB.Column.GetDataObjectByTableId(base.ContextShowSchema, new ObjectRow(currentTableRow, base.CustomFieldsSupport), tableId);
		deletedRelationsRows.Clear();
		if (DB.DataProfiling.IsDataProfilingDisabled())
		{
			sparklineRowDistributionColumn.Visible = false;
			sparklineTopValuesColumn.Visible = false;
		}
		else
		{
			bool flag = false;
			bool flag2 = false;
			foreach (ColumnRow d in dataObjectByTableId)
			{
				ColumnProfiledDataObject columnProfiledDataObject = profiledColumns.Where(delegate(ColumnProfiledDataObject x)
				{
					if (x.ColumnId == d.Id && x != null)
					{
						_ = x.ColumnId;
						ColumnRow columnRow = d;
						if (columnRow == null)
						{
							return false;
						}
						_ = columnRow.Id;
						return true;
					}
					return false;
				}).FirstOrDefault();
				if (columnProfiledDataObject != null)
				{
					d.SparklineRowDistributionText = columnProfiledDataObject.TextForSparkLine;
					if (!flag)
					{
						flag = columnProfiledDataObject != null && columnProfiledDataObject.RowCount > 0 && ((columnProfiledDataObject != null && columnProfiledDataObject.ValuesDistinctRowCount > 0) || (columnProfiledDataObject != null && columnProfiledDataObject.ValuesNondistinctRowCount > 0) || (columnProfiledDataObject != null && columnProfiledDataObject.ValuesNullRowCount > 0) || (columnProfiledDataObject != null && columnProfiledDataObject.ValuesEmptyRowCount > 0));
					}
					if (!flag2)
					{
						List<ColumnValuesDataObject> listOfDataProfilingTopValues = ListOfDataProfilingTopValues;
						flag2 = listOfDataProfilingTopValues != null && listOfDataProfilingTopValues.Count() > 0;
					}
				}
			}
			if (flag)
			{
				sparklineRowDistributionColumn.VisibleIndex = descriptionTableColumnsGridColumn.VisibleIndex + 1;
			}
			else
			{
				sparklineRowDistributionColumn.Visible = flag;
			}
			if (flag2)
			{
				sparklineTopValuesColumn.VisibleIndex = descriptionTableColumnsGridColumn.VisibleIndex + 2;
			}
			else
			{
				sparklineTopValuesColumn.Visible = flag2;
			}
		}
		tableColumnsGrid.DataSource = dataObjectByTableId;
		BasicRow[] elements = TableColumns?.ToArray();
		HistoryColumnsHelper.SaveColumnsOrParametrsOfOldCustomFields(elements, customFieldsColumnForHistory, titleAndDescriptionColumnsForHistory);
		CommonFunctionsPanels.SetBestFitForColumns(tableRelationsGridView);
		FillHighlights(SharedObjectTypeEnum.ObjectType.Relation);
		isColumnsDataLoaded = true;
		base.MainControl.SetWaitformVisibility(visible: false);
		isColumnsDataLoaded = false;
		isRelationsDataLoaded = false;
		isConstraintsDataLoaded = false;
		RefreshColumns(currentTableRow);
		RefeshRelationsTable(forceRefresh: true);
		RefreshContraintsTable();
		RefreshDependencies(forceRefresh: false, refreshImmediatelyIfLoaded: true);
	}

	public void RefreshSparklines()
	{
		base.MainControl.SetWaitformVisibility(visible: true);
		SetProfilingColumnsDataForRowDistributionSparkLine();
		SetListOfDataProfilingTopValues();
		bool flag = false;
		bool flag2 = false;
		if (DB.DataProfiling.IsDataProfilingDisabled())
		{
			sparklineRowDistributionColumn.Visible = false;
			sparklineTopValuesColumn.Visible = false;
		}
		else
		{
			if (TableColumns != null)
			{
				foreach (ColumnRow d in TableColumns)
				{
					ColumnProfiledDataObject columnProfiledDataObject = profiledColumns.Where(delegate(ColumnProfiledDataObject x)
					{
						if (x.ColumnId == d.Id && x != null)
						{
							_ = x.ColumnId;
							ColumnRow columnRow = d;
							if (columnRow == null)
							{
								return false;
							}
							_ = columnRow.Id;
							return true;
						}
						return false;
					}).FirstOrDefault();
					if (columnProfiledDataObject != null)
					{
						d.SparklineRowDistributionText = columnProfiledDataObject.TextForSparkLine;
						if (!flag)
						{
							flag = columnProfiledDataObject != null && columnProfiledDataObject.RowCount > 0 && ((columnProfiledDataObject != null && columnProfiledDataObject.ValuesDistinctRowCount > 0) || (columnProfiledDataObject != null && columnProfiledDataObject.ValuesNondistinctRowCount > 0) || (columnProfiledDataObject != null && columnProfiledDataObject.ValuesNullRowCount > 0) || (columnProfiledDataObject != null && columnProfiledDataObject.ValuesEmptyRowCount > 0));
						}
						if (!flag2)
						{
							List<ColumnValuesDataObject> listOfDataProfilingTopValues = ListOfDataProfilingTopValues;
							flag2 = listOfDataProfilingTopValues != null && listOfDataProfilingTopValues.Count() > 0;
						}
					}
				}
			}
			if (flag)
			{
				sparklineRowDistributionColumn.VisibleIndex = descriptionTableColumnsGridColumn.VisibleIndex + 1;
			}
			else
			{
				sparklineRowDistributionColumn.Visible = flag;
			}
			if (flag2)
			{
				sparklineTopValuesColumn.VisibleIndex = descriptionTableColumnsGridColumn.VisibleIndex + 2;
			}
			else
			{
				sparklineTopValuesColumn.Visible = flag2;
			}
		}
		tableColumnsGrid.Refresh();
		base.MainControl.SetWaitformVisibility(visible: false);
	}

	private void RefreshContraintsTable(bool forceRefresh = false, bool refreshImmediatelyIfLoaded = false, bool refreshImmediatelyIfNotLoaded = false)
	{
		if (IsRefreshRequired(tableConstraintsXtraTabPage, !isConstraintsDataLoaded, forceRefresh) || (refreshImmediatelyIfNotLoaded && !isConstraintsDataLoaded) || (refreshImmediatelyIfLoaded && isConstraintsDataLoaded))
		{
			base.MainControl.SetWaitformVisibility(visible: true);
			BindingList<UniqueConstraintRow> dataObjectByTableId = DB.Constraint.GetDataObjectByTableId(tableId, notDeletedOnly: true, base.CustomFieldsSupport);
			deletedUniqueConstraintRows.Clear();
			tableConstraintsGridView.LeftCoord = 0;
			tableConstraintsGrid.DataSource = dataObjectByTableId;
			KeySuggestedDescriptionManager = new SuggestedDescriptionManager(base.ContextShowSchema, tableId, GetConstraintAndTriggerFieldNames((CustomFieldRowExtended x) => x.KeyVisibility && (x.Type == CustomFieldTypeEnum.CustomFieldType.Text || x.Type == CustomFieldTypeEnum.CustomFieldType.ListOpen)), "unique_constraints", "table_id", ConstraintsRows?.ToList().Cast<BaseRow>().ToList(), tableConstraintsGridView, ObjectType, token);
			SuggestedDescriptionManager keySuggestedDescriptionManager = KeySuggestedDescriptionManager;
			keySuggestedDescriptionManager.Redraw = (Action)Delegate.Combine(keySuggestedDescriptionManager.Redraw, new Action(Redraw));
			KeySuggestedDescriptionManager.Rows = ConstraintsRows?.ToList().Cast<BaseRow>().ToList();
			foreach (UniqueConstraintRow constraintsRow in ConstraintsRows)
			{
				constraintsRow.SuggestedDescriptions.Clear();
			}
			if (base.ShowHints)
			{
				KeySuggestedDescriptionManager.GetSuggestedDescriptions();
				hasLoadedKeySuggestedDescriptions = base.ShowHints;
			}
			CommonFunctionsPanels.SetBestFitForColumns(tableConstraintsGridView);
			FillHighlights(SharedObjectTypeEnum.ObjectType.Key);
			isConstraintsDataLoaded = true;
			base.MainControl.SetWaitformVisibility(visible: false);
		}
		else if (forceRefresh)
		{
			isConstraintsDataLoaded = false;
		}
	}

	private void RefreshTriggersTable(bool forceRefresh = false)
	{
		if (IsRefreshRequired(tableTriggersXtraTabPage, !isTriggersDataLoaded, forceRefresh))
		{
			base.MainControl.SetWaitformVisibility(visible: true);
			BindingList<TriggerRow> dataObjectByTableId = DB.Trigger.GetDataObjectByTableId(tableId, base.CustomFieldsSupport);
			deletedTriggersRows.Clear();
			tableTriggerGridView.LeftCoord = 0;
			tableTriggerGrid.BeginUpdate();
			tableTriggerGrid.DataSource = dataObjectByTableId;
			tableTriggerGrid.EndUpdate();
			BasicRow[] elements = TriggerRows?.ToArray();
			HistoryColumnsHelper.SaveColumnsOrParametrsOfOldCustomFields(elements, customFieldsTriggersForHistory, titleAndDescriptionTriggersForHistory);
			TriggerSuggestedDescriptionManager = new SuggestedDescriptionManager(base.ContextShowSchema, tableId, GetConstraintAndTriggerFieldNames((CustomFieldRowExtended x) => x.TriggerVisibility && (x.Type == CustomFieldTypeEnum.CustomFieldType.Text || x.Type == CustomFieldTypeEnum.CustomFieldType.ListOpen)), "triggers", "table_id", TriggerRows?.ToList().Cast<BaseRow>().ToList(), tableTriggerGridView, ObjectType, token);
			SuggestedDescriptionManager triggerSuggestedDescriptionManager = TriggerSuggestedDescriptionManager;
			triggerSuggestedDescriptionManager.Redraw = (Action)Delegate.Combine(triggerSuggestedDescriptionManager.Redraw, new Action(Redraw));
			TriggerSuggestedDescriptionManager.Rows = TriggerRows?.ToList().Cast<BaseRow>().ToList();
			foreach (TriggerRow triggerRow in TriggerRows)
			{
				triggerRow.SuggestedDescriptions.Clear();
			}
			if (base.ShowHints)
			{
				TriggerSuggestedDescriptionManager.GetSuggestedDescriptions();
				hasLoadedTriggerSuggestedDescriptions = base.ShowHints;
			}
			RefreshTriggerDetailPanel();
			CommonFunctionsPanels.SetBestFitForColumns(tableTriggerGridView);
			FillHighlights(SharedObjectTypeEnum.ObjectType.Trigger);
			isTriggersDataLoaded = true;
			base.MainControl.SetWaitformVisibility(visible: false);
		}
		else if (forceRefresh)
		{
			isTriggersDataLoaded = false;
		}
	}

	private void RefreshDependencies(bool forceRefresh = false, bool refreshImmediatelyIfLoaded = false, bool refreshImmediatelyIfNotLoaded = false)
	{
		bool flag = dependenciesUserControl.DependencyObjectEquals(tableId, ObjectType);
		if ((IsRefreshRequired(dependenciesXtraTabPage, !flag, forceRefresh) || (refreshImmediatelyIfNotLoaded && !flag) || (refreshImmediatelyIfLoaded && flag)) && databaseId.HasValue)
		{
			base.MainControl.SetWaitformVisibility(visible: true);
			dependenciesUserControl.SetParameters(databaseId.Value, databaseServer, databaseName, databaseTitle, base.HasMultipleSchemas, base.DatabaseShowSchema, base.DatabaseShowSchemaOverride, base.ContextShowSchema, schema, name, title, UserTypeEnum.TypeToString(source), ObjectType, base.Subtype, databaseType, tableId, base.MainControl);
			base.MainControl.SetWaitformVisibility(visible: false);
		}
	}

	public void RefreshDataLineage(bool forceRefresh = false, bool refreshImmediatelyIfLoaded = false, bool refreshImmediatelyIfNotLoaded = false)
	{
		bool flag = dataLineageUserControl.CheckIfObjectEquals(tableId, ObjectType);
		if ((IsRefreshRequired(dataLineageXtraTabPage, !flag, forceRefresh) || (refreshImmediatelyIfNotLoaded && !flag) || (refreshImmediatelyIfLoaded && flag)) && databaseId.HasValue)
		{
			base.MainControl.SetWaitformVisibility(visible: true);
			dataLineageUserControl.SetParameters(base.MainControl, base.CurrentNode);
			base.MainControl.SetWaitformVisibility(visible: false);
		}
	}

	private bool IsRefreshRequired(XtraTabPage tabPage, bool additionalCondition = true, bool forceRefresh = false)
	{
		if (!forceRefresh || tableXtraTabControl.SelectedTabPage != tabPage)
		{
			if (tableXtraTabControl.SelectedTabPage == tabPage && additionalCondition)
			{
				return !base.TabPageChangedProgrammatically;
			}
			return false;
		}
		return true;
	}

	private void ClearData(DBTreeNode selectedNode, bool setDeletedObjectProperties = false)
	{
		base.DisableSettingAsEdited = true;
		try
		{
			currentTableRow = null;
			name = null;
			title = null;
			schema = null;
			databaseId = null;
			databaseName = null;
			databaseTitle = null;
			databaseServer = null;
			base.HasMultipleSchemas = null;
			isModuleEdited = false;
			databaseType = null;
			TextEdit textEdit = schemaTextEdit;
			TextEdit textEdit2 = tableTitleTextEdit;
			TextEdit textEdit3 = nameTextEdit;
			HtmlUserControl htmlUserControl = tabHtmlUserControl;
			LabelControl labelControl = dbmsLastUpdatedLabel;
			LabelControl labelControl2 = lastSynchronizedLabel;
			string text2 = (scriptRichEditControl.Text = null);
			string text4 = (labelControl2.Text = text2);
			string text6 = (labelControl.Text = text4);
			string text8 = (htmlUserControl.HtmlText = text6);
			string text10 = (textEdit3.Text = text8);
			string text13 = (textEdit.Text = (textEdit2.Text = text10));
			idEventArgs.DatabaseId = null;
			CommonFunctionsPanels.SetName(tableTextEdit, tableDescriptionXtraTabPage, ObjectType, selectedNode.Subtype, selectedNode.Schema, selectedNode.Name, selectedNode.Title, databaseType, databaseTitle, showDatabaseTitle, base.ShowSchema, source);
			ClearColumnsTable();
			ClearRelationsTable();
			ClearContraintsTable();
			ClearTriggersTable();
			isTableEdited = false;
			CommonFunctionsPanels.ClearTabPagesTitle(tableXtraTabControl, base.Edit);
			if (setDeletedObjectProperties)
			{
				tableStatusUserControl.SetDeletedObjectProperties();
			}
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			base.DisableSettingAsEdited = false;
		}
	}

	private void ClearColumnsTable()
	{
		isColumnsDataLoaded = false;
		deletedColumnsRows.Clear();
		tableColumnsGrid.DataSource = null;
	}

	private void ClearContraintsTable()
	{
		isConstraintsDataLoaded = false;
		deletedUniqueConstraintRows.Clear();
		tableConstraintsGrid.DataSource = null;
	}

	private void ClearRelationsTable()
	{
		isRelationsDataLoaded = false;
		deletedRelationsRows.Clear();
		tableRelationsGrid.DataSource = null;
	}

	private void ClearTriggersTable()
	{
		isTriggersDataLoaded = false;
		deletedTriggersRows.Clear();
		tableTriggerGrid.BeginUpdate();
		tableTriggerGrid.DataSource = null;
		tableTriggerGrid.EndUpdate();
		RefreshTriggerDetailPanel();
	}

	private void SetCustomFieldsDataSource()
	{
		customFieldsPanelControl.EditValueChanging += delegate
		{
			SetCurrentTabPageTitle(isEdited: true, tableDescriptionXtraTabPage);
		};
		customFieldsPanelControl.ShowHistoryClick -= CustomFieldsPanelControl_ShowHistoryClick;
		customFieldsPanelControl.ShowHistoryClick += CustomFieldsPanelControl_ShowHistoryClick;
		IEnumerable<CustomFieldDefinition> customFieldRows = customFields.CustomFieldsData.Where((CustomFieldDefinition x) => x.CustomField?.TableVisibility ?? false);
		customFieldsPanelControl.LoadFields(customFieldRows, delegate
		{
			isTableEdited = true;
		}, customFieldsLayoutControlItem);
		customFieldsCellsTypesSupportForGrids.SetCustomFields(base.CustomFieldsSupport, customFieldsSettings);
	}

	private void OpenColumnsForm(string columnName)
	{
		if (base.MainControl.ContinueAfterPossibleChanges())
		{
			ColumnRow columnRow = tableColumnsGridView.GetFocusedRow() as ColumnRow;
			InitializeColumnsForm(columnName, columnRow).ShowDialog();
			if (columnRow.RowState != 0)
			{
				RefreshColumns(currentTableRow, forceRefresh: true);
				columnRow.SetUnchanged();
			}
		}
	}

	private ColumnsForm InitializeColumnsForm(string columnName, ColumnRow row)
	{
		ColumnsForm columnsForm = new ColumnsForm();
		columnsForm.SetParameters(base.CustomFieldsSupport, base.MainControl.EditCustomFields, row, columnName);
		columnsForm.SetColumns();
		return columnsForm;
	}

	protected override void Redraw()
	{
		if (GetVisibleGridControl() != null)
		{
			GetVisibleGridControl().Refresh();
		}
	}

	public override void CancelSuggestedDescriptions()
	{
		ColumnSuggestedDescriptionManager?.Cancel();
		RelationSuggestedDescriptionManager?.Cancel();
		TriggerSuggestedDescriptionManager?.Cancel();
		KeySuggestedDescriptionManager?.Cancel();
	}

	protected override void LoadSuggestedDescriptions(object sender, EventArgs e)
	{
		SuggestedDescriptionManager currentSuggestedDescriptionManager = GetCurrentSuggestedDescriptionManager();
		if (currentSuggestedDescriptionManager != null)
		{
			if (!GetCurrentTabHasLoadedSuggestedDescription() && base.ShowHints)
			{
				currentSuggestedDescriptionManager.GetSuggestedDescriptions();
				SetCurrentTabHasLoadedSuggestedDescription(value: true);
			}
			Redraw();
		}
	}

	private bool GetCurrentTabHasLoadedSuggestedDescription()
	{
		if (tableXtraTabControl.SelectedTabPage == tableColumnListXtraTabPage)
		{
			return hasLoadedColumnSuggestedDescriptions;
		}
		if (tableXtraTabControl.SelectedTabPage == tableConstraintsXtraTabPage)
		{
			return hasLoadedKeySuggestedDescriptions;
		}
		if (tableXtraTabControl.SelectedTabPage == tableRelationsXtraTabPage)
		{
			return hasLoadedRelationSuggestedDescriptions;
		}
		if (tableXtraTabControl.SelectedTabPage == tableTriggersXtraTabPage)
		{
			return hasLoadedTriggerSuggestedDescriptions;
		}
		return false;
	}

	private void SetCurrentTabHasLoadedSuggestedDescription(bool value)
	{
		if (tableXtraTabControl.SelectedTabPage == tableColumnListXtraTabPage)
		{
			hasLoadedColumnSuggestedDescriptions = value;
		}
		if (tableXtraTabControl.SelectedTabPage == tableConstraintsXtraTabPage)
		{
			hasLoadedKeySuggestedDescriptions = true;
		}
		if (tableXtraTabControl.SelectedTabPage == tableRelationsXtraTabPage)
		{
			hasLoadedRelationSuggestedDescriptions = true;
		}
		if (tableXtraTabControl.SelectedTabPage == tableTriggersXtraTabPage)
		{
			hasLoadedTriggerSuggestedDescriptions = true;
		}
	}

	private SuggestedDescriptionManager GetCurrentSuggestedDescriptionManager()
	{
		if (tableXtraTabControl.SelectedTabPage == tableColumnListXtraTabPage)
		{
			return ColumnSuggestedDescriptionManager;
		}
		if (tableXtraTabControl.SelectedTabPage == tableConstraintsXtraTabPage)
		{
			return KeySuggestedDescriptionManager;
		}
		if (tableXtraTabControl.SelectedTabPage == tableRelationsXtraTabPage)
		{
			return RelationSuggestedDescriptionManager;
		}
		if (tableXtraTabControl.SelectedTabPage == tableTriggersXtraTabPage)
		{
			return TriggerSuggestedDescriptionManager;
		}
		return null;
	}

	private GridControl GetVisibleGridControl()
	{
		if (tableXtraTabControl.SelectedTabPage == tableColumnListXtraTabPage)
		{
			return tableColumnsGrid;
		}
		if (tableXtraTabControl.SelectedTabPage == tableConstraintsXtraTabPage)
		{
			return tableConstraintsGrid;
		}
		if (tableXtraTabControl.SelectedTabPage == tableRelationsXtraTabPage)
		{
			return tableRelationsGrid;
		}
		if (tableXtraTabControl.SelectedTabPage == tableTriggersXtraTabPage)
		{
			return tableTriggerGrid;
		}
		if (tableXtraTabControl.SelectedTabPage == dataLinksXtraTabPage)
		{
			return dataLinksGrid;
		}
		return null;
	}

	private void tableXtraTabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		if (tableXtraTabControl.SelectedTabPageIndex == 0)
		{
			SelectedTab.selectedTabCaption = "info";
		}
		else
		{
			SelectedTab.selectedTabCaption = tableXtraTabControl.SelectedTabPage.Text;
		}
		RefreshColumns(currentTableRow);
		RefeshRelationsTable();
		RefreshContraintsTable();
		RefreshTriggersTable();
		RefreshDependencies();
		RefreshDataLineage();
		dataLinksManager.RefreshDataLinks();
		ShowPrimaryUniqueButtons();
		if (tableXtraTabControl.SelectedTabPage == tableColumnListXtraTabPage)
		{
			ShowOnboardingAfterOpeningColumnsTab();
		}
		RefreshSchemaImportsAndChanges();
	}

	private void RefreshSchemaImportsAndChanges(bool forceRefresh = false, bool refreshImmediatelyIfLoaded = false, bool refreshImmediatelyIfNotLoaded = false)
	{
		if (IsRefreshRequired(schemaImportsAndChangesXtraTabPage, additionalCondition: true, forceRefresh) || refreshImmediatelyIfNotLoaded || refreshImmediatelyIfLoaded)
		{
			schemaImportsAndChangesUserControl.RefreshImports();
		}
		base.MainControl.SetProfileColumnButtonVisibility(new BoolEventArgs(value: false));
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			switch (keyData)
			{
			case Keys.F | Keys.Control:
				if (tabHtmlUserControl.PerformFindActionIfFocused())
				{
					return true;
				}
				if (triggerScriptRichEditControl.Focused)
				{
					return false;
				}
				if (scriptRichEditControl.Focused)
				{
					return false;
				}
				if (schemaTextRichEditUserControl.Focused)
				{
					return false;
				}
				break;
			case Keys.F2 | Keys.Shift:
			{
				GridControl visibleGridControl = GetVisibleGridControl();
				if (visibleGridControl == null)
				{
					return false;
				}
				if ((visibleGridControl.MainView as GridView).FocusedRowHandle >= 0)
				{
					ShowSuggestedDescriptionsContextMenu(GetVisibleGridControl(), GetCurrentSuggestedDescriptionManager());
					return true;
				}
				break;
			}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	public override void HideCustomization()
	{
		tableColumnsGridView.HideCustomization();
		tableConstraintsGridView.HideCustomization();
		tableRelationsGridView.HideCustomization();
		tableTriggerGridView.HideCustomization();
		dataLinksGridView.HideCustomization();
	}

	private void columnsPopupMenu_Popup(object sender, EventArgs e)
	{
		Point pt = tableColumnsGridView.GridControl.PointToClient(Control.MousePosition);
		GridHitInfo gridHitInfo = tableColumnsGridView.CalcHitInfo(pt);
		if (gridHitInfo.InRowCell)
		{
			_ = 1;
		}
		else
			_ = gridHitInfo.InRow;
	}

	private void columnsPopupMenu_BeforePopup(object sender, CancelEventArgs e)
	{
		bool isGroupAdded = false;
		if ((TableColumns.Count > 0 && tableColumnsGridView.FocusedRowHandle < 0) || Grids.GetBeforePopupShouldCancel(sender))
		{
			e.Cancel = true;
			return;
		}
		bool flag = clickHitInfo.InRow || clickHitInfo.InRowCell;
		ColumnRow column = tableColumnsGridView.GetFocusedRow() as ColumnRow;
		if (column == null)
		{
			e.Cancel = true;
			return;
		}
		columnsPopupMenu.ItemLinks.Clear();
		string text = SharedObjectTypeEnum.TypeToStringForSingle(ObjectType);
		designTableColumnsBarButtonItem.Caption = "Design " + text;
		designTableColumnsBarButtonItem.Enabled = true;
		designTableColumnsBarButtonItem.Hint = string.Empty;
		if (!flag && (ObjectType == SharedObjectTypeEnum.ObjectType.Table || ObjectType == SharedObjectTypeEnum.ObjectType.Structure))
		{
			columnsPopupMenu.ItemLinks.Add(designTableColumnsBarButtonItem);
		}
		if (flag)
		{
			if (base.ShowHints)
			{
				ColumnSuggestedDescriptionManager.CreateSuggestedDescriptionContextMenuItems(columnsPopupMenu, isSeparatorDrawn: true, base.ShowHints);
			}
			if (CommonFunctionsDatabase.AreRowsToDelete(tableColumnsGridView, isObject: true))
			{
				columnsPopupMenu.ItemLinks.Add(removeColumnsColumnsBarButtonItem);
				columnsPopupMenu.StartGroupBeforeLastItem(ref isGroupAdded);
			}
			if (ObjectType == SharedObjectTypeEnum.ObjectType.Table || ObjectType == SharedObjectTypeEnum.ObjectType.Structure)
			{
				columnsPopupMenu.ItemLinks.Add(designTableColumnsBarButtonItem);
				columnsPopupMenu.StartGroupBeforeLastItem(ref isGroupAdded);
			}
			if (column.Source == UserTypeEnum.UserType.DBMS && (column.Status == SynchronizeStateEnum.SynchronizeState.New || column.Status == SynchronizeStateEnum.SynchronizeState.Synchronized))
			{
				columnsPopupMenu.ItemLinks.Add(profileColumnBarButtonItem);
				if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
				{
					columnsPopupMenu.ItemLinks.Add(clearColumnProfilingDataBarButtonItem);
				}
			}
			columnsPopupMenu.ItemLinks.Add(addRelationColumnsBarButtonItem);
			columnsPopupMenu.ItemLinks.Add(addPrimaryKeyUniqueKeysBarButtonItem);
			columnsPopupMenu.ItemLinks.Add(addUniqueKeyUniqueKeysBarButtonItem);
			columnsPopupMenu.StartGroupBeforeLastItem(ref isGroupAdded);
			if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary))
			{
				columnsPopupMenu.ItemLinks.Add(editLinksColumnsBarButtonItem);
				columnsPopupMenu.ItemLinks.Add(addNewLinkedTermColumnsBarButtonItem);
				dataLinksManager.SetAddNewBusinessGlossaryTermMenu(columnsPopupMenu.ItemLinks.FirstOrDefault((BarItemLink x) => x.Item.Name?.Equals("addNewLinkedTermColumnsBarButtonItem") ?? false), columnsPopupMenu, FindForm());
				columnsPopupMenu.StartGroupBeforeLastItem(ref isGroupAdded);
			}
			BarItemLinkCollection itemLinks = columnsPopupMenu.ItemLinks;
			BarItem[] items = column.ReferencesDataContainer.DistinctDataSorted.SelectMany(delegate(ColumnReferenceData x)
			{
				List<BarButtonItem> list = new List<BarButtonItem>();
				BarButtonItem barButtonItem2 = new BarButtonItem
				{
					Caption = "Go to " + Escaping.EscapeTextForUI(x.ReferenceString),
					Glyph = x.ObjectImage,
					Tag = x
				};
				barButtonItem2.ItemClick += delegate
				{
					ColumnReferenceData columnReferenceData = x;
					if (columnReferenceData != null && columnReferenceData.PkDatabaseId.HasValue && columnReferenceData.PkObjectId.HasValue && columnReferenceData.PkObjectType.HasValue && columnReferenceData.PkObjectId != tableId)
					{
						base.MainControl.SelectObject(columnReferenceData.PkDatabaseId.Value, columnReferenceData.PkObjectId.Value, columnReferenceData.PkObjectType.Value);
					}
				};
				list.Add(barButtonItem2);
				columnsPopupMenu.StartGroupBeforeLastItem(ref isGroupAdded);
				return list;
			}).ToArray();
			itemLinks.AddRange(items);
			BarButtonItem barButtonItem = new BarButtonItem
			{
				Caption = "Find " + Escaping.EscapeTextForUI(column.Name) + " columns",
				Glyph = Icons.GetImageForContextMenu(column, SharedObjectTypeEnum.ObjectType.Column)
			};
			barButtonItem.ItemClick += delegate
			{
				OpenColumnsForm(column.Name);
			};
			columnsPopupMenu.ItemLinks.Add(barButtonItem);
			if (clickHitInfo == null || clickHitInfo.Column == null)
			{
				return;
			}
			if (clickHitInfo.Column == titleTableColumnsGridColumn)
			{
				viewHistoryBarButtonItem.Tag = "title";
				columnsPopupMenu.ItemLinks.Add(viewHistoryBarButtonItem);
			}
			if (clickHitInfo.Column == descriptionTableColumnsGridColumn)
			{
				viewHistoryBarButtonItem.Tag = "description";
				columnsPopupMenu.ItemLinks.Add(viewHistoryBarButtonItem);
			}
			if (clickHitInfo.Column.FieldName.ToLower().StartsWith("Field".ToLower()))
			{
				viewHistoryBarButtonItem.Tag = clickHitInfo.Column.FieldName;
				columnsPopupMenu.ItemLinks.Add(viewHistoryBarButtonItem);
			}
			if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary))
			{
				dataLinksManager.AddGoToMenuItemsForColumns(columnsPopupMenu, column);
			}
		}
		e.Cancel = false;
	}

	private void addRelationColumnsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddRelation(GetSelectedColumnRows());
	}

	private void designTableColumnsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		EditTable();
	}

	private RelationRow GetFocusedRelationRow()
	{
		return tableRelationsGridView.GetFocusedRow() as RelationRow;
	}

	private void SetRelationPageAsModified()
	{
		SetTabPageTitle(isEdited: true, tableRelationsXtraTabPage, base.Edit);
	}

	private void SetRelationPageAsUnchanged()
	{
		SetTabPageTitle(isEdited: false, tableXtraTabControl, tableRelationsXtraTabPage, base.Edit);
	}

	private RelationRow SetWholePageAndFocusedRelationAsModified()
	{
		RelationRow focusedRelationRow = GetFocusedRelationRow();
		if (focusedRelationRow == null)
		{
			return null;
		}
		focusedRelationRow.SetModified();
		SetRelationPageAsModified();
		return focusedRelationRow;
	}

	private RelationRow SetWholePageAndGivenRelationAsModified(RelationRow editedRelation)
	{
		if (editedRelation.Source == UserTypeEnum.UserType.NotSet)
		{
			editedRelation.Source = UserTypeEnum.UserType.USER;
		}
		editedRelation.SetModified();
		SetRelationPageAsModified();
		return editedRelation;
	}

	private void tableRelationsGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		CheckIfRelationsUpdated();
	}

	private void tableRelationsGridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		SetWholePageAndFocusedRelationAsModified();
	}

	private void tableRelationsGrid_ProcessGridKey(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return && tableRelationsGridView.FocusedColumn != descriptionTableRelationsGridColumn && tableRelationsGridView.FocusedColumn != titleTableRelationsGridColumn && tableRelationsGridView.FocusedRowHandle >= 0)
		{
			EditRelation();
		}
	}

	private void deleteRelationsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		ProcessRelationDelete();
	}

	private void ProcessRelationDelete()
	{
		try
		{
			int[] selectedRows = tableRelationsGridView.GetSelectedRows();
			List<int> list = new List<int>();
			if (!selectedRows.Select((int r) => tableRelationsGridView.GetRow(r) as RelationRow).Any((RelationRow r) => r.CanBeDeleted()) || !CommonFunctionsDatabase.AskIfDeleting(selectedRows, tableRelationsGridView, SharedObjectTypeEnum.ObjectType.Relation))
			{
				return;
			}
			int[] array = selectedRows;
			foreach (int num in array)
			{
				if (!(tableRelationsGridView.GetRow(num) is RelationRow relationRow) || !relationRow.CanBeDeleted())
				{
					continue;
				}
				if (relationRow.RowState != ManagingRowsEnum.ManagingRows.ForAdding)
				{
					SetWholePageAndGivenRelationAsModified(relationRow);
					relationRow.Columns.Clear();
					if (relationRow.RowState != ManagingRowsEnum.ManagingRows.Added)
					{
						deletedRelationsRows.Add(relationRow.Id);
					}
					list.Add(num);
					RefreshColumns(currentTableRow, forceRefresh: false, refreshImmediatelyIfNotLoaded: true);
					foreach (ColumnRow tableColumn in TableColumns)
					{
						int? num2 = tableColumn?.ReferencesDataContainer.Data?.Count;
						if (!num2.HasValue || num2 == 0)
						{
							continue;
						}
						for (int num3 = num2.Value - 1; num3 >= 0; num3--)
						{
							if (tableColumn?.ReferencesDataContainer.Data[num3]?.RelationId == relationRow.Id)
							{
								tableColumn?.ReferencesDataContainer.Data?.RemoveAt(num3);
							}
						}
					}
					CommonFunctionsPanels.RefreshDataAndWidths(tableColumnsGridView);
				}
				else
				{
					relationRow.Clear();
				}
			}
			for (int num4 = list.Count - 1; num4 >= 0; num4--)
			{
				tableRelationsGridView.DeleteRow(list[num4]);
			}
			CheckIfRelationsUpdated();
			RefreshRelationGrids();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void CheckIfRelationsUpdated()
	{
		if ((tableRelationsGridView.DataSource as BindingList<RelationRow>).Any((RelationRow r) => ((r.RowState == ManagingRowsEnum.ManagingRows.ForAdding || r.RowState == ManagingRowsEnum.ManagingRows.Added) && !r.IsEmpty) || r.RowState == ManagingRowsEnum.ManagingRows.Updated) || deletedRelationsRows.Count > 0)
		{
			SetRelationPageAsModified();
		}
		else
		{
			SetRelationPageAsUnchanged();
		}
	}

	private void CheckIfTriggersUpdated()
	{
		if ((tableTriggerGridView.DataSource as BindingList<TriggerRow>).Any((TriggerRow r) => r.RowState == ManagingRowsEnum.ManagingRows.Updated))
		{
			SetTriggerPageAsModified();
		}
		else
		{
			SetTriggerPageAsModified();
		}
	}

	private void tableRelationsListGridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		Icons.SetIcon(e, iconTableRelationsGridColumn, SharedObjectTypeEnum.ObjectType.Relation);
	}

	private void tableRelationsGridView_ShowingEditor(object sender, CancelEventArgs e)
	{
		bool flag = GetFocusedRelationRow()?.IsEditable ?? false;
		e.Cancel = tableRelationsGridView.FocusedColumn.ReadOnly && !flag;
	}

	private void tableRelationsListGridView_RowCellStyle(object sender, RowCellCustomDrawEventArgs e)
	{
		if (isParentTable(e.Column, e.RowHandle))
		{
			e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
		}
	}

	private bool isParentTable(GridColumn column, int rowHandle)
	{
		if (column != null && rowHandle >= 0 && (column.Equals(fkTableTableRelationsGridColumn) || column.Equals(pkTableTableRelationsGridColumn)))
		{
			RelationRow relationRow = tableRelationsGridView.GetRow(rowHandle) as RelationRow;
			if (relationRow.PKTableId != tableId || !column.Equals(pkTableTableRelationsGridColumn))
			{
				if (relationRow.FKTableId == tableId)
				{
					return column.Equals(fkTableTableRelationsGridColumn);
				}
				return false;
			}
			return true;
		}
		return false;
	}

	private void tableRelationsGridView_DoubleClick(object sender, EventArgs e)
	{
		DXMouseEventArgs dXMouseEventArgs = (DXMouseEventArgs)e;
		GridHitInfo gridHitInfo = tableRelationsGridView.CalcHitInfo(dXMouseEventArgs.Location);
		if (tableRelationsGridView.FocusedRowHandle >= 0 && (gridHitInfo.InRow || gridHitInfo.InRowCell) && gridHitInfo.Column.ReadOnly)
		{
			EditRelation();
		}
	}

	public override void ReloadGridsData()
	{
		RefeshRelationsTable(forceRefresh: true);
		tableRelationsGridView.RefreshData();
		RefreshDataLinks();
	}

	private void RefreshRelationGrids()
	{
		RelationRow focusedRelationRow = GetFocusedRelationRow();
		if (focusedRelationRow != null)
		{
			if (focusedRelationRow.RowState == ManagingRowsEnum.ManagingRows.ForAdding && !focusedRelationRow.IsEmpty)
			{
				SetWholePageAndFocusedRelationAsModified();
			}
			tableRelationsGridView.RefreshRow(tableRelationsGridView.FocusedRowHandle);
		}
	}

	private void SetRelationButton()
	{
	}

	private void ClearRelationButtonsTooltips()
	{
		relationsBarManager.ShowScreenTipsInMenus = false;
		string text3 = (addRelationRelationsBarButtonItem.Hint = (editRelationRelationsBarButtonItem.Hint = null));
	}

	private void relationsPopupMenu_BeforePopup(object sender, CancelEventArgs e)
	{
		if ((RelationRows.Count > 0 && tableRelationsGridView.FocusedRowHandle < 0) || Grids.GetBeforePopupShouldCancel(sender))
		{
			e.Cancel = true;
			return;
		}
		relationsPopupMenu.ItemLinks.Clear();
		Grids.GetBeforePopupHitInfo(sender);
		bool beforePopupIsRowClicked = Grids.GetBeforePopupIsRowClicked(sender);
		RelationRow focusedRelationRow = GetFocusedRelationRow();
		bool flag = beforePopupIsRowClicked && tableRelationsGridView.FocusedRowHandle >= 0;
		bool flag2 = beforePopupIsRowClicked && ((focusedRelationRow != null && focusedRelationRow.ShowEditRemoveButton) || (focusedRelationRow != null && focusedRelationRow.Status == SynchronizeStateEnum.SynchronizeState.Deleted));
		editRelationRelationsBarButtonItem.Visibility = ((!flag) ? BarItemVisibility.Never : BarItemVisibility.Always);
		removeRelationRelationsBarButtonItem.Visibility = ((!flag2) ? BarItemVisibility.Never : BarItemVisibility.Always);
		goToObjectRelationsBarButtonItem.Visibility = ((!beforePopupIsRowClicked || tableRelationsGridView.FocusedRowHandle < 0) ? BarItemVisibility.Never : BarItemVisibility.Always);
		relationsBarManager.ShowScreenTipsInMenus = false;
		BarButtonItem barButtonItem = addRelationRelationsBarButtonItem;
		BarButtonItem barButtonItem2 = editRelationRelationsBarButtonItem;
		string text2 = (removeRelationRelationsBarButtonItem.Hint = null);
		string text5 = (barButtonItem.Hint = (barButtonItem2.Hint = text2));
		if (beforePopupIsRowClicked && focusedRelationRow != null)
		{
			if (base.ShowHints)
			{
				RelationSuggestedDescriptionManager.CreateSuggestedDescriptionContextMenuItems(relationsPopupMenu, isSeparatorDrawn: true, base.ShowHints);
			}
			string value = string.Empty;
			SharedObjectTypeEnum.ObjectType? mainType = null;
			SharedObjectSubtypeEnum.ObjectSubtype? objectSubtype = null;
			UserTypeEnum.UserType? userType = null;
			ObjectStatusEnum.ObjectStatus? objectStatus = null;
			if (focusedRelationRow.PKTableId == tableId)
			{
				value = focusedRelationRow.FKTableDisplayName;
				mainType = focusedRelationRow.FKObjectType;
				objectSubtype = focusedRelationRow.FKSubtype;
				userType = focusedRelationRow.FKSource;
				objectStatus = focusedRelationRow.FKStatus;
			}
			else if (focusedRelationRow.FKTableId == tableId)
			{
				value = focusedRelationRow.PKTableDisplayName;
				mainType = focusedRelationRow.PKObjectType;
				objectSubtype = focusedRelationRow.PKSubtype;
				userType = focusedRelationRow.PKSource;
				objectStatus = focusedRelationRow.PKStatus;
			}
			if (!mainType.HasValue)
			{
				return;
			}
			goToObjectRelationsBarButtonItem.Glyph = Icons.SetObjectIcon(mainType.ToString().ToLower(), SharedObjectSubtypeEnum.TypeToString(mainType, objectSubtype).ToLower(), userType == UserTypeEnum.UserType.USER, objectStatus == ObjectStatusEnum.ObjectStatus.Deleted);
			goToObjectRelationsBarButtonItem.Caption = "Go to " + Escaping.EscapeTextForUI(value);
		}
		relationsPopupMenu.ItemLinks.Add(addRelationRelationsBarButtonItem);
		relationsPopupMenu.StartGroupBeforeLastItem();
		relationsPopupMenu.ItemLinks.Add(editRelationRelationsBarButtonItem);
		if (CommonFunctionsDatabase.AreDeletableRows(tableRelationsGridView, isObject: true))
		{
			relationsPopupMenu.ItemLinks.Add(removeRelationRelationsBarButtonItem);
		}
		relationsPopupMenu.ItemLinks.Add(goToObjectRelationsBarButtonItem);
	}

	private void SetRelationButtonsTooltips()
	{
		relationsBarManager.ShowScreenTipsInMenus = true;
		string text3 = (addRelationRelationsBarButtonItem.Hint = (editRelationRelationsBarButtonItem.Hint = "Upgrade to Pro to enable it"));
	}

	private void addRelationRelationsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddRelation();
	}

	private void editRelationRelationsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		EditRelation();
	}

	private void removeRelationRelationsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ProcessRelationDelete();
	}

	private void goToObjectRelationsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		RelationRow focusedRelationRow = GetFocusedRelationRow();
		int? num = null;
		int? num2 = null;
		SharedObjectTypeEnum.ObjectType? objectType = null;
		if (focusedRelationRow.PKTableId == tableId)
		{
			num = focusedRelationRow.FKTableDatabaseId;
			num2 = focusedRelationRow.FKTableId;
			objectType = focusedRelationRow.FKObjectType;
		}
		else if (focusedRelationRow.FKTableId == tableId)
		{
			num = focusedRelationRow.PKTableDatabaseId;
			num2 = focusedRelationRow.PKTableId;
			objectType = focusedRelationRow.PKObjectType;
		}
		if (num.HasValue && num2.HasValue)
		{
			base.MainControl.SelectObject(num.Value, num2.Value, objectType.Value);
		}
	}

	public override bool Save()
	{
		try
		{
			bool isError = false;
			if (base.Edit.IsEdited)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
				HashSet<XtraTabPage> hashSet = new HashSet<XtraTabPage>();
				if (!Licenses.CheckRepositoryVersionAfterLogin())
				{
					isError = true;
				}
				else
				{
					if (base.UserControlHelpers.IsSearchActive)
					{
						ClearHighlights(keepSearchActive: true);
					}
					DocumentationModules[] checkedValues = null;
					(SharedObjectTypeEnum.ObjectType, int) loadedFor = documentationModulesUserControl.LoadedFor;
					SharedObjectTypeEnum.ObjectType objectType = ObjectType;
					int num = tableId;
					ObjectIdName[] checkedObjects;
					if (loadedFor.Item1 != objectType || loadedFor.Item2 != num)
					{
						checkedObjects = (from x in DB.Table.GetTableModules(tableId)
							select new ObjectIdName
							{
								BaseId = x
							}).ToArray();
					}
					else
					{
						checkedValues = documentationModulesUserControl.GetSelectedModules();
						checkedObjects = (from x in checkedValues.SelectMany((DocumentationModules x) => x.Modules.Where((ModuleRow y) => y.IsShown))
							select new ObjectIdName
							{
								BaseId = (x.Id ?? (-1)),
								Name = x.Name
							}).ToArray();
					}
					CommonFunctionsDatabase.CheckModulesBeforeSaving(databaseId, ObjectType, tableId, ref checkedObjects, documentationModulesUserControl, GetSplashScreenManager(), FindForm());
					TableRow tableRow = new TableRow(tableId, tableTitleTextEdit.Text, tableLocationTextEdit.Text, checkedObjects, PrepareValue.GetHtmlText(tabHtmlUserControl?.PlainText, tabHtmlUserControl?.HtmlText), tabHtmlUserControl.PlainText);
					tableRow.ObjectType = ObjectType;
					tableRow.CustomFields = customFields;
					customFieldsPanelControl.SetCustomFieldsValuesInRow(tableRow);
					if (DB.Table.Update(tableRow, FindForm()))
					{
						tabHtmlUserControl.SetHtmlTextAsOriginal();
						isTableEdited = false;
						DBTreeMenu.RefeshNodeTitle(databaseId.Value, tableTitleTextEdit.Text, ObjectType, name, schema, databaseType, base.ShowSchema);
						CommonFunctionsPanels.SetName(tableTextEdit, tableDescriptionXtraTabPage, ObjectType, base.Subtype, schema, name, tableTitleTextEdit.Text, databaseType, databaseTitle, showDatabaseTitle, base.ShowSchema, source);
						HistoryCustomFieldsHelper.InsertHistoryCustomFieldsOnTablePanel(tableRow?.Id, customFieldsTableViewStructureForHistory, customFields, databaseId, ObjectType);
						customFieldsPanelControl.UpdateDefinitionValues();
						tableRow.CustomFields.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
						customFieldsTableViewStructureForHistory = new Dictionary<string, string>();
						CustomFieldDefinition[] customFieldsData = customFields.CustomFieldsData;
						foreach (CustomFieldDefinition customFieldDefinition in customFieldsData)
						{
							customFieldsTableViewStructureForHistory.Add(customFieldDefinition.CustomField.FieldName, customFieldDefinition.FieldValue);
						}
						DB.Community.InsertFollowingToRepository(ObjectType, tableId);
						bool flag = HistoryGeneralHelper.CheckAreValuesDiffrent(tableRow.Title, tableTitleAndDescriptionHistory?.Title);
						bool saveDescription = HistoryGeneralHelper.CheckAreHtmlValuesAreDiffrent(tableRow?.DescriptionPlain, tableRow?.Description, tableTitleAndDescriptionHistory?.DescriptionPlain, tableTitleAndDescriptionHistory?.Description);
						tableTitleAndDescriptionHistory = new ObjectWithTitleAndHTMLDescriptionHistory
						{
							ObjectId = tableId,
							Title = tableRow?.Title,
							Description = tableRow?.Description,
							DescriptionPlain = tableRow?.DescriptionPlain
						};
						if (flag && tabHtmlUserControl?.TableRow != null)
						{
							tabHtmlUserControl.TableRow.Title = tableRow.Title;
						}
						DB.History.InsertHistoryRow(databaseId, tableRow.Id, tableRow.Title, tableRow.Description, tableRow.DescriptionPlain, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(ObjectType), flag, saveDescription, ObjectType);
						WorkWithDataedoTrackingHelper.TrackFirstInSessionObjectEdit();
					}
					else
					{
						isError = true;
					}
					if (UpdateColumns())
					{
						SetTabPageTitle(isEdited: false, tableColumnListXtraTabPage);
					}
					else
					{
						hashSet.Add(tableColumnListXtraTabPage);
						isError = true;
					}
					if (UpdateTriggers())
					{
						SetTabPageTitle(isEdited: false, tableTriggersXtraTabPage);
					}
					else
					{
						hashSet.Add(tableTriggersXtraTabPage);
						isError = true;
					}
					bool flag2 = false;
					if (flag2 = UpdateUniqueConstraints())
					{
						SetTabPageTitle(isEdited: false, tableConstraintsXtraTabPage);
					}
					else
					{
						hashSet.Add(tableConstraintsXtraTabPage);
						isError = true;
					}
					if (UpdateDependencies())
					{
						SetTabPageTitle(isEdited: false, dependenciesXtraTabPage);
					}
					else
					{
						hashSet.Add(dependenciesXtraTabPage);
						isError = true;
					}
					bool flag3 = false;
					if (flag3 = UpdateRelations())
					{
						SetTabPageTitle(isEdited: false, tableRelationsXtraTabPage);
					}
					else
					{
						hashSet.Add(tableRelationsXtraTabPage);
						isError = true;
					}
					if (UpdateDataLineage())
					{
						SetTabPageTitle(isEdited: false, dataLineageXtraTabPage);
					}
					else
					{
						hashSet.Add(dataLineageXtraTabPage);
						isError = true;
					}
					dataLinksManager.ProcessUpdating(ref isError);
					RefreshDataLinks();
					if (SchemaImportsAndChangesSupport.UpdateSchemaImportsAndChangesComments(FindForm()))
					{
						SetTabPageTitle(isEdited: false, schemaImportsAndChangesXtraTabPage);
					}
					else
					{
						hashSet.Add(schemaImportsAndChangesXtraTabPage);
						isError = true;
					}
					if (!flag2 && !flag3)
					{
						GeneralMessageBoxesHandling.Show("Please provide all required data for user-defined relationships and unique keys.", "Saving failed", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
					}
					else if (!flag2)
					{
						GeneralMessageBoxesHandling.Show("Please provide all required data for user-defined unique keys.", "Saving failed", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
					}
					else if (!flag3)
					{
						GeneralMessageBoxesHandling.Show("Please provide all required data for user-defined relationships.", "Saving failed", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
					}
					else
					{
						base.MainControl.RefreshObjectProgress(showWaitForm: false, ObjectId, ObjectType);
					}
					if (!isError)
					{
						base.Edit.SetUnchanged();
						RefreshModules();
						ManageObjectsInMenu(databaseId ?? (-1), checkedValues, tableId, schema, name, title, source, checkedObjects);
						if (base.UserControlHelpers.IsSearchActive)
						{
							base.MainControl.OpenCurrentlySelectedSearchRow();
							tabHtmlUserControl.SetNotChanged();
							if (tabHtmlUserControl.Highlight())
							{
								base.UserControlHelpers.SetHighlight();
								searchCountLabelControl.Text = tabHtmlUserControl.Occurrences;
							}
							ForceLostFocus();
						}
					}
					else
					{
						base.Edit.SetEdited();
						foreach (XtraTabPage item in hashSet)
						{
							SetTabPageTitle(isEdited: true, item);
						}
					}
					FillControlProgressHighlights();
				}
				RefreshColumns(currentTableRow, forceRefresh: true);
				SetTabsProgressHighlights();
			}
			TableObject dataById = DB.Table.GetDataById(tableId);
			dbmsCreatedLabelControl.Text = PrepareValue.SetDateTimeWithFormatting(dataById.DbmsCreationDate);
			createdLabelControl.Text = PrepareValue.SetDateTimeWithFormatting(dataById.CreationDate) + " " + dataById.CreatedBy;
			dbmsLastUpdatedLabel.Text = PrepareValue.SetDateTimeWithFormatting(dataById.DbmsLastModificationDate);
			lastSynchronizedLabel.Text = PrepareValue.SetDateTimeWithFormatting(dataById.SynchronizationDate) + " " + dataById.SynchronizedBy;
			lastUpdatedLabelControl.Text = PrepareValue.SetDateTimeWithFormatting(dataById.LastModificationDate) + " " + dataById.ModifiedBy;
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			return !isError;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
			return false;
		}
	}

	public override void RefreshModules()
	{
		if (isModuleEdited)
		{
			base.TreeMenu.RefreshAllModulesAndSelect(databaseId.Value, ObjectType, moduleId, tableId);
			isModuleEdited = false;
		}
	}

	private void PrepareRelationGridPanelUserControl()
	{
		SetRelationButton();
		relationsGridPanelUserControl.InsertAdditionalButton(addRelationColumnsBarButtonItem, 4);
	}

	private void PrepareKeyGridPanelUserControl()
	{
		SetKeyButtons();
		constraintsGridPanelUserControl.InsertAdditionalButton(addPrimaryKeyUniqueKeysBarButtonItem, 4);
		constraintsGridPanelUserControl.InsertAdditionalButton(addUniqueKeyUniqueKeysBarButtonItem, 5);
	}

	private void AddPKBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddConstraint(isPK: true);
	}

	private void AddFKBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddConstraint(isPK: false);
	}

	private void AddRelationBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddRelation();
	}

	private bool UpdateColumns()
	{
		try
		{
			tableColumnsGridView.CloseEditor();
			if (TableColumns != null)
			{
				DB.Column.Delete(deletedColumnsRows, FindForm());
				ColumnRow[] array = TableColumns.Where((ColumnRow x) => x.RowState != ManagingRowsEnum.ManagingRows.Unchanged).ToArray();
				if (DB.Column.Update(FindForm(), array))
				{
					HistoryColumnsHelper.SaveTitleDescriptionCustomFieldsHistoryInUpdateColumns(customFieldsColumnForHistory, titleAndDescriptionColumnsForHistory, array, SharedObjectTypeEnum.ObjectType.Column, databaseId);
					ColumnRow[] array2 = array;
					for (int i = 0; i < array2.Length; i++)
					{
						array2[i].SetUnchanged();
					}
				}
				CustomFieldContainer.UpdateDefinitionValues(array.SelectMany((ColumnRow x) => x.CustomFields.CustomFieldsData));
				deletedColumnsRows.Clear();
			}
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating columns", FindForm());
			return false;
		}
	}

	private bool UpdateUniqueConstraints()
	{
		try
		{
			tableConstraintsGridView.CloseEditor();
			if (tableConstraintsGrid.DataSource != null)
			{
				BindingList<UniqueConstraintRow> bindingList = tableConstraintsGrid.DataSource as BindingList<UniqueConstraintRow>;
				if (bindingList.Any((UniqueConstraintRow x) => !x.IsReady))
				{
					return false;
				}
				DB.Constraint.Delete(deletedUniqueConstraintRows, FindForm());
				List<UniqueConstraintRow> list = bindingList.Where((UniqueConstraintRow x) => x.RowState != 0 && x.IsReadyNotEmpty).ToList();
				list.ForEach(delegate(UniqueConstraintRow x)
				{
					DB.Constraint.Update(x, FindForm());
				});
				CustomFieldContainer.UpdateDefinitionValues(list.SelectMany((UniqueConstraintRow x) => x.CustomFields.CustomFieldsData));
				RefreshContraintsTable(forceRefresh: false, refreshImmediatelyIfLoaded: true);
			}
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating constraints", FindForm());
			return false;
		}
	}

	private bool UpdateRelations()
	{
		try
		{
			tableRelationsGridView.CloseEditor();
			if (tableRelationsGrid.DataSource != null)
			{
				BindingList<RelationRow> bindingList = tableRelationsGrid.DataSource as BindingList<RelationRow>;
				if (bindingList.Any((RelationRow x) => !x.IsReady))
				{
					return false;
				}
				DB.Relation.Delete(allDeletedRelations, FindForm());
				List<RelationRow> list = bindingList.Where((RelationRow x) => x.RowState != ManagingRowsEnum.ManagingRows.Unchanged).ToList();
				list.ForEach(delegate(RelationRow x)
				{
					DB.Relation.Update(x, FindForm());
				});
				CustomFieldContainer.UpdateDefinitionValues(list.SelectMany((RelationRow x) => x.CustomFields.CustomFieldsData));
				RefeshRelationsTable(forceRefresh: false, refreshImmediatelyIfLoaded: true);
				if (allDeletedRelations.Any() || list.Any())
				{
					RefreshDependencies(forceRefresh: false, refreshImmediatelyIfLoaded: true);
				}
			}
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating relationships", FindForm());
			return false;
		}
	}

	private bool UpdateDataLineage()
	{
		if (dataLineageUserControl.IsInitialized)
		{
			return dataLineageUserControl.SaveChanges();
		}
		return true;
	}

	private bool UpdateTriggers()
	{
		try
		{
			tableTriggerGridView.CloseEditor();
			if (tableTriggerGrid.DataSource != null)
			{
				DB.Trigger.Delete(deletedTriggersRows, FindForm());
				List<TriggerRow> list = (tableTriggerGrid.DataSource as BindingList<TriggerRow>).Where((TriggerRow x) => x.RowState != ManagingRowsEnum.ManagingRows.Unchanged).ToList();
				if (DB.Trigger.Update(list, FindForm()))
				{
					HistoryColumnsHelper.SaveTitleDescriptionCustomFieldsHistoryInUpdateColumns(customFieldsTriggersForHistory, titleAndDescriptionTriggersForHistory, list, SharedObjectTypeEnum.ObjectType.Trigger, databaseId);
					list.ForEach(delegate(TriggerRow x)
					{
						x.SetUnchanged();
					});
				}
				CustomFieldContainer.UpdateDefinitionValues(list.SelectMany((TriggerRow x) => x.CustomFields.CustomFieldsData));
				deletedTriggersRows.Clear();
			}
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating triggers", FindForm());
			return false;
		}
	}

	private bool UpdateDependencies()
	{
		return UpdateDependencies(databaseId.Value, dependenciesUserControl);
	}

	public void ProcessDependencyDelete()
	{
		dependenciesUserControl.DeleteDependency();
	}

	private void tabHtmlUserControl_PreviewKeyDown(object sender, EventArgs e)
	{
		isTableEdited = true;
	}

	private void tableTitleTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		isTableEdited = true;
		title = tableTitleTextEdit.Text;
	}

	private void tableLocationTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		isTableEdited = true;
	}

	private void tableTypeLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		isTableEdited = true;
	}

	private void tableModuleLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		isTableEdited = true;
		isModuleEdited = true;
	}

	private void tableTriggerGridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		if (tableTriggerGridView.FocusedRowHandle >= 0)
		{
			SetTabPageTitle(isEdited: true, tableTriggersXtraTabPage, base.Edit);
			(tableTriggerGridView.GetFocusedRow() as TriggerRow).SetModified();
		}
	}

	private void dependenciesUserControl_DependencyChanging()
	{
		SetTabPageTitle(isEdited: true, dependenciesXtraTabPage, base.Edit);
	}

	private void DataLineageUserControl_DataLineageEdited()
	{
		SetTabPageTitle(isEdited: true, dataLineageXtraTabPage, base.Edit);
	}

	private void schemaTextRichEditUserControl_ContentChangedEvent(object sender, EventArgs e)
	{
		SetTabPageTitle(isEdited: true, tableSchemaXtraTabPage, base.Edit);
	}

	private void tableColumnsGridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		Icons.SetIcon(e, iconTableColumnsGridColumn, SharedObjectTypeEnum.ObjectType.Column);
		Icons.SetColumnPKIcon(e, keyTableColumnsGridColumn);
	}

	private UniqueConstraintRow GetFocusedUniqueConstraintRow()
	{
		return tableConstraintsGridView.GetFocusedRow() as UniqueConstraintRow;
	}

	private void tableConstraintsGridView_RowCellStyle(object sender, RowCellCustomDrawEventArgs e)
	{
	}

	private void SetConstraintPageAsModified()
	{
		SetTabPageTitle(isEdited: true, tableConstraintsXtraTabPage, base.Edit);
	}

	private void SetTriggerPageAsModified()
	{
		SetTabPageTitle(isEdited: true, tableTriggersXtraTabPage, base.Edit);
	}

	private void SetConstraintPageAsUnchanged()
	{
		SetTabPageTitle(isEdited: false, tableXtraTabControl, tableConstraintsXtraTabPage, base.Edit);
	}

	private UniqueConstraintRow SetWholePageAndFocusedConstraintAsModified()
	{
		if (GetFocusedUniqueConstraintRow() != null)
		{
			return SetWholePageAndGivenConstraintAsModified(GetFocusedUniqueConstraintRow());
		}
		return null;
	}

	private UniqueConstraintRow SetWholePageAndGivenConstraintAsModified(UniqueConstraintRow editedConstraint)
	{
		if (editedConstraint.Source == UserTypeEnum.UserType.NotSet)
		{
			editedConstraint.Source = UserTypeEnum.UserType.USER;
		}
		if (editedConstraint.RowState != ManagingRowsEnum.ManagingRows.ForAdding && editedConstraint.RowState != ManagingRowsEnum.ManagingRows.Added)
		{
			editedConstraint.SetModified();
		}
		SetConstraintPageAsModified();
		return editedConstraint;
	}

	private void tableConstraintsGrid_ProcessGridKey(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return && tableConstraintsGridView.FocusedColumn != descriptionTableConstraintsGridColumn)
		{
			EditConstraint();
		}
	}

	public void ProcessConstraintDelete()
	{
		try
		{
			int[] selectedRows = tableConstraintsGridView.GetSelectedRows();
			List<int> list = new List<int>();
			if (!selectedRows.Select((int r) => tableConstraintsGridView.GetRow(r) as UniqueConstraintRow).Any((UniqueConstraintRow r) => r.CanBeDeleted()) || !CommonFunctionsDatabase.AskIfDeleting(selectedRows, tableConstraintsGridView, SharedObjectTypeEnum.ObjectType.Key))
			{
				return;
			}
			int[] array = selectedRows;
			foreach (int num in array)
			{
				if (!(tableConstraintsGridView.GetRow(num) is UniqueConstraintRow uniqueConstraintRow) || !uniqueConstraintRow.CanBeDeleted())
				{
					continue;
				}
				if (uniqueConstraintRow.RowState != ManagingRowsEnum.ManagingRows.ForAdding)
				{
					SetWholePageAndGivenConstraintAsModified(uniqueConstraintRow);
					uniqueConstraintRow.Columns.Clear();
					if (uniqueConstraintRow.RowState != ManagingRowsEnum.ManagingRows.Added)
					{
						deletedUniqueConstraintRows.Add(uniqueConstraintRow.Id);
					}
					list.Add(num);
				}
				else
				{
					uniqueConstraintRow.Clear();
				}
				RefreshConstraintGrids();
			}
			for (int num2 = list.Count - 1; num2 >= 0; num2--)
			{
				tableConstraintsGridView.DeleteRow(list[num2]);
			}
			CheckIfConstraintsUpdated();
			RefreshConstraintGrids();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void CheckIfConstraintsUpdated()
	{
		if ((tableConstraintsGridView.DataSource as BindingList<UniqueConstraintRow>).Any((UniqueConstraintRow r) => ((r.RowState == ManagingRowsEnum.ManagingRows.ForAdding || r.RowState == ManagingRowsEnum.ManagingRows.Added) && !r.IsEmpty) || r.RowState == ManagingRowsEnum.ManagingRows.Updated) || deletedUniqueConstraintRows.Count > 0)
		{
			SetConstraintPageAsModified();
		}
		else
		{
			SetConstraintPageAsUnchanged();
		}
	}

	private void RefreshConstraintGrids()
	{
		UniqueConstraintRow focusedUniqueConstraintRow = GetFocusedUniqueConstraintRow();
		if (focusedUniqueConstraintRow != null && focusedUniqueConstraintRow.RowState == ManagingRowsEnum.ManagingRows.ForAdding && !focusedUniqueConstraintRow.IsEmpty)
		{
			SetWholePageAndFocusedConstraintAsModified();
		}
		tableConstraintsGridView.RefreshRow(tableConstraintsGridView.FocusedRowHandle);
	}

	private void SetKeyButtons()
	{
	}

	private void uniqueKeysPopupMenu_BeforePopup(object sender, CancelEventArgs e)
	{
		if ((ConstraintsRows.Count > 0 && tableConstraintsGridView.FocusedRowHandle < 0) || Grids.GetBeforePopupShouldCancel(sender))
		{
			e.Cancel = true;
			return;
		}
		uniqueKeysPopupMenu.ItemLinks.Clear();
		Grids.GetBeforePopupHitInfo(sender);
		bool beforePopupIsRowClicked = Grids.GetBeforePopupIsRowClicked(sender);
		UniqueConstraintRow focusedUniqueConstraintRow = GetFocusedUniqueConstraintRow();
		bool flag = beforePopupIsRowClicked && (focusedUniqueConstraintRow?.ShowEditRemoveButton ?? false);
		bool flag2 = focusedUniqueConstraintRow != null && (flag || focusedUniqueConstraintRow.Source != UserTypeEnum.UserType.USER);
		editKeyUniqueKeysBarButtonItem.Visibility = ((!flag) ? BarItemVisibility.Never : BarItemVisibility.Always);
		removeKeyUniqueKeysBarButtonItem.Visibility = ((!flag2) ? BarItemVisibility.Never : BarItemVisibility.Always);
		if (beforePopupIsRowClicked && base.ShowHints)
		{
			KeySuggestedDescriptionManager.CreateSuggestedDescriptionContextMenuItems(uniqueKeysPopupMenu, isSeparatorDrawn: true, base.ShowHints);
		}
		uniqueKeysBarManager.ShowScreenTipsInMenus = false;
		BarButtonItem barButtonItem = addPrimaryKeyUniqueKeysBarButtonItem;
		BarButtonItem barButtonItem2 = addUniqueKeyUniqueKeysBarButtonItem;
		BarButtonItem barButtonItem3 = editKeyUniqueKeysBarButtonItem;
		string text2 = (removeKeyUniqueKeysBarButtonItem.Hint = null);
		string text4 = (barButtonItem3.Hint = text2);
		string text7 = (barButtonItem.Hint = (barButtonItem2.Hint = text4));
		uniqueKeysPopupMenu.ItemLinks.Add(addPrimaryKeyUniqueKeysBarButtonItem);
		uniqueKeysPopupMenu.StartGroupBeforeLastItem();
		uniqueKeysPopupMenu.ItemLinks.Add(addUniqueKeyUniqueKeysBarButtonItem);
		uniqueKeysPopupMenu.ItemLinks.Add(editKeyUniqueKeysBarButtonItem);
		if (CommonFunctionsDatabase.AreDeletableRows(tableConstraintsGridView, isObject: true))
		{
			uniqueKeysPopupMenu.ItemLinks.Add(removeKeyUniqueKeysBarButtonItem);
		}
	}

	private void ClearKeyButtonsTooltips()
	{
		uniqueKeysBarManager.ShowScreenTipsInMenus = false;
		string text3 = (addPrimaryKeyUniqueKeysBarButtonItem.Hint = (addUniqueKeyUniqueKeysBarButtonItem.Hint = null));
	}

	private void addPrimaryKeyUniqueKeysBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddConstraint(isPK: true);
	}

	private void addUniqueKeyUniqueKeysBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddConstraint(isPK: false);
	}

	private void editKeyUniqueKeysBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		EditConstraint();
	}

	private void tableTriggerGridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		Icons.SetIcon(e, iconTableTriggersGridColumn, SharedObjectTypeEnum.ObjectType.Trigger);
	}

	private void tableTriggerGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		RefreshTriggerDetailPanel();
	}

	private void RefreshTriggerDetailPanel()
	{
		TriggerRow triggerRow = tableTriggerGridView.GetFocusedRow() as TriggerRow;
		if (triggerRow != null && triggerRow.Definition != null)
		{
			if (databaseType == SharedDatabaseTypeEnum.DatabaseType.Cassandra || databaseType == SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra)
			{
				string cassandraTriggerScript = GetCassandraTriggerScript(triggerRow);
				ColorizeSyntax(triggerScriptRichEditControl, cassandraTriggerScript, "SQL");
			}
			else
			{
				ColorizeSyntax(triggerScriptRichEditControl, triggerRow?.Definition, "SQL");
			}
		}
		else if (triggerRow != null && triggerRow.Subtype == SharedObjectSubtypeEnum.ObjectSubtype.CLRTrigger)
		{
			triggerScriptRichEditControl.SetCLRTriggerText();
		}
		else
		{
			triggerScriptRichEditControl.Text = null;
		}
		triggerScriptRichEditControl.SetOriginalHtmlText();
		if (base.UserControlHelpers.IsSearchActive)
		{
			triggerScriptRichEditControl.Highlight(base.UserControlHelpers.GetLastSearchWords());
			BaseSkin.SetSearchHighlightOrDefault(scriptLabelControl, triggerScriptRichEditControl.OccurrencesCount > 0);
			scriptLabelControl.Text = "Script " + triggerScriptRichEditControl.Occurrences;
		}
		triggerScriptRichEditControl.RefreshSkin();
	}

	private string GetCassandraTriggerScript(TriggerRow triggerRow)
	{
		return "CREATE TRIGGER " + triggerRow.Name + Environment.NewLine + "ON " + schema + "." + name + Environment.NewLine + "USING '" + triggerRow.Definition + "'";
	}

	private void triggersPopupMenu_BeforePopup(object sender, CancelEventArgs e)
	{
		if (tableTriggerGrid == null || tableTriggerGridView == null || (TriggerRows.Count > 0 && tableTriggerGridView.FocusedRowHandle < 0) || Grids.GetBeforePopupShouldCancel(sender))
		{
			e.Cancel = true;
		}
	}

	private TriggerRow GetFocusedTriggerRow()
	{
		return tableTriggerGridView.GetFocusedRow() as TriggerRow;
	}

	private void tableColumnsGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
		clickHitInfo = e.HitInfo;
		columnsPopupMenu.ShowPopupMenu(sender, e, inRowCellOnly: false);
	}

	private void tableRelationsListGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
		relationsPopupMenu.ShowPopupMenu(sender, e, inRowCellOnly: false);
	}

	private void tableConstraintsGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
		uniqueKeysPopupMenu.ShowPopupMenu(sender, e, inRowCellOnly: false);
	}

	private void tableTriggerGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
		triggersPopupMenu.ItemLinks.Clear();
		bool flag = e.HitInfo.InRowCell || e.HitInfo.InRow;
		bool flag2 = flag && (GetFocusedTriggerRow()?.ShowEditRemoveButton ?? false);
		removeTriggerTriggersBarButtonItem.Visibility = ((!flag2) ? BarItemVisibility.Never : BarItemVisibility.Always);
		if (e.HitInfo.Column != null)
		{
			if (flag && base.ShowHints && !e.HitInfo.Column.FieldName.Equals("Name") && !e.HitInfo.Column.FieldName.Equals("WhenRun"))
			{
				TriggerSuggestedDescriptionManager.CreateSuggestedDescriptionContextMenuItems(triggersPopupMenu, isSeparatorDrawn: true, showHints: true);
			}
			triggersBarManager.ShowScreenTipsInMenus = false;
			removeTriggerTriggersBarButtonItem.Hint = null;
			if (CommonFunctionsDatabase.AreRowsToDelete(tableTriggerGridView, isObject: true))
			{
				triggersPopupMenu.ItemLinks.Add(removeTriggerTriggersBarButtonItem);
				triggersPopupMenu.StartGroupBeforeLastItem();
			}
			if (e.HitInfo.Column == descriptionTableTriggersGridColumn || e.HitInfo.Column.FieldName.ToLower().StartsWith("field"))
			{
				viewHistoryTriggersBarButtonItem.Tag = e.HitInfo.Column.FieldName.ToLower();
				triggersPopupMenu.ItemLinks.Add(viewHistoryTriggersBarButtonItem);
			}
			triggersPopupMenu.ShowPopupMenu(sender, e, inRowCellOnly: false);
		}
	}

	private void dataLinksGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
	}

	private void ColumnsButtonsVisibleChanged(object sender, EventArgs e)
	{
		bool value = tableXtraTabControl.SelectedTabPage == tableColumnListXtraTabPage || tableXtraTabControl.SelectedTabPage == tableRelationsXtraTabPage;
		base.MainControl.SetAddRelationButtonVisibility(new BoolEventArgs(value));
	}

	private void TriggersButtonsVisibleChanged(object sender, EventArgs e)
	{
		if (tableTriggerGridView.GetFocusedRow() is TriggerRow triggerRow)
		{
			triggersGridPanelUserControl.SetRemoveButtonVisibility(triggerRow.Source == UserTypeEnum.UserType.USER);
		}
	}

	private void RelationButtonsVisibleChanged(object sender, EventArgs e)
	{
		ShowRelationButtons();
	}

	private void ConstraintButtonsVisibleChanged(object sender, EventArgs e)
	{
		ShowConstraintButtons();
	}

	private void DependencyButtonsVisibleChanged(object sender, EventArgs e)
	{
		ShowDependencyButtons();
	}

	private void DataLinksButtonsVisibleChanged(object sender, EventArgs e)
	{
		ShowDataLinkButtons();
	}

	public void ShowRelationButtons()
	{
		bool value = tableXtraTabControl.SelectedTabPage == tableRelationsXtraTabPage || tableXtraTabControl.SelectedTabPage == tableColumnListXtraTabPage;
		_ = GetFocusedRelationRow()?.IsEditable;
		bool value2 = tableRelationsGrid.Focused && GetFocusedRelationRow() != null;
		base.MainControl.SetAddRelationButtonVisibility(new BoolEventArgs(value));
		base.MainControl.SetEditRelationButtonVisibility(new BoolEventArgs(value2));
	}

	public void ShowConstraintButtons()
	{
		UniqueConstraintRow focusedUniqueConstraintRow = GetFocusedUniqueConstraintRow();
		bool flag = tableConstraintsGrid.Focused && (focusedUniqueConstraintRow?.ShowEditRemoveButton ?? false);
		bool value = flag || (tableConstraintsGrid.Focused && focusedUniqueConstraintRow != null && focusedUniqueConstraintRow.Status == SynchronizeStateEnum.SynchronizeState.Deleted);
		base.MainControl.SetEditConstraintButtonVisibility(new BoolEventArgs(flag));
		base.MainControl.SetRemoveConstraintButtonVisibility(new BoolEventArgs(value));
	}

	private void ShowDependencyButtons()
	{
		bool value = tableXtraTabControl.SelectedTabPage == dependenciesXtraTabPage && dependenciesUserControl.CanFocusedNodeBeDeleted();
		base.MainControl.SetRemoveDependencyButtonVisibility(new BoolEventArgs(value));
	}

	public void ShowDataLinkButtons()
	{
		bool value = tableXtraTabControl.SelectedTabPage == dataLinksXtraTabPage;
		base.MainControl.SetAddDataLinkButtonVisibility(new BoolEventArgs(value));
	}

	public void FocusColumn(int columnId)
	{
		CommonActions.FocusColumn(tableColumnsGridView, TableColumns, columnId);
	}

	public IEnumerable<ColumnRow> GetSelectedColumnRows()
	{
		return from x in tableColumnsGridView.GetSelectedRows()
			select tableColumnsGridView.GetRow(x) as ColumnRow into x
			where x != null
			select x;
	}

	private void tableConstraintsGridView_DoubleClick(object sender, EventArgs e)
	{
		DXMouseEventArgs dXMouseEventArgs = (DXMouseEventArgs)e;
		GridHitInfo gridHitInfo = tableConstraintsGridView.CalcHitInfo(dXMouseEventArgs.Location);
		if ((gridHitInfo.InRow || gridHitInfo.InRowCell) && gridHitInfo.Column.ReadOnly)
		{
			EditConstraint();
		}
	}

	private void tableConstraintsGridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		SetWholePageAndFocusedConstraintAsModified();
	}

	private void tableConstraintsGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		CheckIfConstraintsUpdated();
	}

	private void tableConstraintsGridView_ShowingEditor(object sender, CancelEventArgs e)
	{
		UniqueConstraintRow focusedUniqueConstraintRow = GetFocusedUniqueConstraintRow();
		if (focusedUniqueConstraintRow != null)
		{
			bool isEditable = focusedUniqueConstraintRow.IsEditable;
			e.Cancel = tableConstraintsGridView.FocusedColumn.ReadOnly && !isEditable;
		}
		else
		{
			e.Cancel = true;
		}
	}

	private void dependenciesUserControl_BeforeChangingRelations()
	{
		RefeshRelationsTable(forceRefresh: false, refreshImmediatelyIfLoaded: false, refreshImmediatelyIfNotLoaded: true);
	}

	private void tableColumnsGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		SetTabPageTitle(isEdited: true, tableColumnListXtraTabPage, base.Edit);
		(tableColumnsGridView.GetFocusedRow() as ColumnRow).SetModified();
	}

	private void documentationModulesUserControl_EditValueChanged(object sender, EventArgs e)
	{
		isTableEdited = isTableEdited || documentationModulesUserControl.IsChanged;
		isModuleEdited = isModuleEdited || documentationModulesUserControl.IsChanged;
		documentationModulesUserControl.IsChanged = false;
	}

	private void tableRelationsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		RelationRow focusedRelationRow = GetFocusedRelationRow();
		if (focusedRelationRow != null)
		{
			relationsGridPanelUserControl.SetRemoveButtonVisibility(focusedRelationRow.Source == UserTypeEnum.UserType.USER);
		}
		else
		{
			relationsGridPanelUserControl.SetRemoveButtonVisibility(value: false);
		}
	}

	private void tableConstraintsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		UniqueConstraintRow focusedUniqueConstraintRow = GetFocusedUniqueConstraintRow();
		if (focusedUniqueConstraintRow != null)
		{
			constraintsGridPanelUserControl.SetRemoveButtonVisibility(focusedUniqueConstraintRow.Source == UserTypeEnum.UserType.USER);
		}
		else
		{
			constraintsGridPanelUserControl.SetRemoveButtonVisibility(value: false);
		}
	}

	private void tableRelationsGridView_DataSourceChanged(object sender, EventArgs e)
	{
		relationsGridPanelUserControl.SetRemoveButtonVisibility(value: false);
	}

	private void tableConstraintsGridView_DataSourceChanged(object sender, EventArgs e)
	{
		constraintsGridPanelUserControl.SetRemoveButtonVisibility(value: false);
	}

	private void tableTriggerGridView_DataSourceChanged(object sender, EventArgs e)
	{
		triggersGridPanelUserControl.SetRemoveButtonVisibility(value: false);
	}

	private void tableToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		SetSuggestedDescriptionTooltips(GetVisibleGridControl().MainView as GridView, e);
	}

	private void GridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		int? num = null;
		ProgressPainter.SetEmptyProgressCellsBackground(e, base.progressType, base.MainControl.ShowProgress);
		BulkCopyGridUserControl bulkCopyGridUserControl = sender as BulkCopyGridUserControl;
		if (sender.Equals(tableColumnsGridView))
		{
			num = 0;
			if ((e.Column == ordinalPositionTableColumnsGridColumn || e.Column == nameTableColumnsGridColumn || e.Column == dataTypeTableColumnsGridColumn || e.Column == keyTableColumnsGridColumn || e.Column == referencesTableColumnsGridColumn || e.Column == nullableTableColumnsGridColumn || e.Column == identityTableColumnsGridColumn || e.Column == defaultComputedTableColumnsGridColumn || e.Column == dataLinksGridColumn || e.Column == createdByGridColumn || e.Column == createdGridColumn || e.Column == lastUpdatedByGridColumn || e.Column == lastUpdatedGridColumn) && ((bulkCopyGridUserControl != null && !bulkCopyGridUserControl.IsFocusedView) || (bulkCopyGridUserControl != null && !bulkCopyGridUserControl.GetSelectedRows().Contains(e.RowHandle))))
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.GridNonEditableColumnsBackColor;
			}
			ProgressPainter.SetEmptyDescriptionCellsBackground(e, base.progressType, base.MainControl.ShowProgress);
		}
		else if (sender.Equals(tableRelationsGridView))
		{
			num = 1;
			if ((e.Column == fkTableTableRelationsGridColumn || e.Column == pkTableTableRelationsGridColumn || e.Column == pkTableTableRelationsGridColumn || e.Column == nameTableRelationsGridColumn || e.Column == joinGridColumn) && ((bulkCopyGridUserControl != null && !bulkCopyGridUserControl.IsFocusedView) || (bulkCopyGridUserControl != null && !bulkCopyGridUserControl.GetSelectedRows().Contains(e.RowHandle))))
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.GridNonEditableColumnsBackColor;
			}
			tableRelationsListGridView_RowCellStyle(sender, e);
		}
		else if (sender.Equals(tableConstraintsGridView))
		{
			num = 2;
			if ((e.Column == nameTableConstraintsGridColumn || e.Column == columnsTableConstraintsGridColumn) && ((bulkCopyGridUserControl != null && !bulkCopyGridUserControl.IsFocusedView) || (bulkCopyGridUserControl != null && !bulkCopyGridUserControl.GetSelectedRows().Contains(e.RowHandle))))
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.GridNonEditableColumnsBackColor;
			}
			tableConstraintsGridView_RowCellStyle(sender, e);
		}
		else if (sender.Equals(tableTriggerGridView))
		{
			num = 3;
			if ((e.Column == nameTableTriggersGridColumn || e.Column == whenTableTriggersGridColumn) && ((bulkCopyGridUserControl != null && !bulkCopyGridUserControl.IsFocusedView) || (bulkCopyGridUserControl != null && !bulkCopyGridUserControl.GetSelectedRows().Contains(e.RowHandle))))
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.GridNonEditableColumnsBackColor;
			}
		}
		else if (sender.Equals(dataLinksGridView))
		{
			num = 3;
			if ((e.Column == termIconDataLinksGridColumn || e.Column == objectDataLinksGridColumn || e.Column == typeDataLinksGridColumn || e.Column == objectIconDataLinksGridColumn) && ((bulkCopyGridUserControl != null && !bulkCopyGridUserControl.IsFocusedView) || (bulkCopyGridUserControl != null && !bulkCopyGridUserControl.GetSelectedRows().Contains(e.RowHandle))))
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.GridNonEditableColumnsBackColor;
			}
		}
		base.UserControlHelpers.HighlightRowCellStyle(num, sender, e);
		if (base.ShowHints)
		{
			GetVisibleGridControl()?.Invoke((Action)delegate
			{
				CustomDrawGridCell(sender, e);
			});
		}
		int rowHandle = e.RowHandle;
		ColumnRow columnRow = tableColumnsGridView.GetRow(rowHandle) as ColumnRow;
		string text = "sparklineRowDistributionColumn";
		string text2 = "sparklineTopValuesColumn";
		if (e.Column.Name != text && e.Column.Name != text2)
		{
			return;
		}
		ColumnProfiledDataObject profiledColumn = profiledColumns.Where(delegate(ColumnProfiledDataObject c)
		{
			if (columnRow?.Id == c?.ColumnId && c != null)
			{
				_ = c.ColumnId;
				ColumnRow columnRow2 = columnRow;
				if (columnRow2 == null)
				{
					return false;
				}
				_ = columnRow2.Id;
				return true;
			}
			return false;
		}).FirstOrDefault();
		if (profiledColumn == null)
		{
			return;
		}
		if (profiledColumn.TextForSparkLine == "100% NULL")
		{
			e.Appearance.ForeColor = Color.Black;
		}
		else
		{
			e.Appearance.ForeColor = Color.White;
		}
		if (!profiledColumn.RowCount.HasValue || profiledColumn.RowCount == 0)
		{
			return;
		}
		if (e.Column.Name == text)
		{
			DataProfilingUtils.ShowRowDistributionSparklines(e.Cache, e.Bounds, profiledColumn);
		}
		else if (e.Column.Name == text2 && (profiledColumn.ValuesListMode == "T" || profiledColumn.ValuesListMode == "R"))
		{
			int count = 20;
			List<long?> list = ListOfDataProfilingTopValues.Where((ColumnValuesDataObject c) => c.ColumnId == profiledColumn.ColumnId)?.Where((ColumnValuesDataObject x) => x?.RowCount.HasValue ?? false)?.Take(count)?.Select((ColumnValuesDataObject x) => x?.RowCount)?.ToList();
			if (list != null && list.Any())
			{
				DataProfilingUtils.DrawValuesSparklines(list, e.Cache, e.Bounds, 0);
			}
		}
	}

	private void TableUserControl_Leave(object sender, EventArgs e)
	{
		(GetVisibleGridControl()?.MainView as GridView)?.HideCustomization();
	}

	private void tableTriggerGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		CheckIfTriggersUpdated();
	}

	private void TableColumnsGridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		if (!tableColumnsGridView.OptionsView.ShowAutoFilterRow || tableColumnsGridView.FocusedRowHandle >= 0)
		{
			SetTabPageTitle(isEdited: true, tableColumnListXtraTabPage, base.Edit);
			(tableColumnsGridView.GetFocusedRow() as ColumnRow).SetModified();
		}
	}

	private void columnsBarManager_HighlightedLinkChanged(object sender, HighlightedLinkChangedEventArgs e)
	{
		if (e.Link is BarCustomContainerItemLink barCustomContainerItemLink && barCustomContainerItemLink.ScreenBounds.Contains(Control.MousePosition))
		{
			barCustomContainerItemLink.OpenMenu();
		}
	}

	private void tableColumnsGridView_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		if (e.Column == nameTableColumnsGridColumn)
		{
			string fullName = (e.RowObject1 as ColumnRow).FullName;
			string fullName2 = (e.RowObject2 as ColumnRow).FullName;
			e.Result = fullName.CompareTo(fullName2);
			e.Handled = true;
		}
		else if (e.Column == ordinalPositionTableColumnsGridColumn)
		{
			SortOrdinalPosition(e);
		}
	}

	private void SortOrdinalPosition(CustomColumnSortEventArgs e)
	{
		e.Result = new OrdinalPositionComparer().Compare(e.Value1?.ToString(), e.Value2?.ToString());
		e.Handled = true;
	}

	private void tableRelationsGridView_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		if (e.Column == joinGridColumn)
		{
			string joinColumns = (e.RowObject1 as RelationRow).JoinColumns;
			string joinColumns2 = (e.RowObject2 as RelationRow).JoinColumns;
			e.Result = joinColumns.CompareTo(joinColumns2);
			e.Handled = true;
		}
	}

	private void tableConstraintsGridView_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		if (e.Column == columnsTableConstraintsGridColumn)
		{
			string columnsString = (e.RowObject1 as UniqueConstraintRow).ColumnsString;
			string columnsString2 = (e.RowObject2 as UniqueConstraintRow).ColumnsString;
			e.Result = columnsString.CompareTo(columnsString2);
			e.Handled = true;
		}
	}

	private void dataLinksGridView_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		if (e.Column == tableColumnDataLinksGridColumn)
		{
			string fullNameForObject = (e.RowObject1 as DataLinkObject).FullNameForObject;
			string fullNameForObject2 = (e.RowObject2 as DataLinkObject).FullNameForObject;
			e.Result = fullNameForObject.CompareTo(fullNameForObject2);
			e.Handled = true;
		}
	}

	private void ShowOnboardingAfterOpeningColumnsTab()
	{
		Rectangle? rectangle = ((GridViewInfo)tableColumnsGridView.GetViewInfo())?.ViewRects?.ColumnPanelActual;
		if (rectangle.HasValue)
		{
			Rectangle bounds = new Rectangle(tableColumnsGrid.PointToScreen(rectangle.Value.Location), new Size(200, rectangle.Value.Height));
			OnboardingSupport.ShowPanel(OnboardingSupport.OnboardingMessages.ColumnsOpen, FindForm(), () => bounds);
		}
	}

	private void SetColumnGridViewTooltips()
	{
		keyTableColumnsGridColumn.ToolTip = ColumnTooltips.Key;
		nameTableColumnsGridColumn.ToolTip = "Name of the column/ field in source database. You can provide user/ business friendly alias using Title field. Title will be displayed next to column names in Dataedo shared documentation.";
		ordinalPositionTableColumnsGridColumn.ToolTip = "Ordinal position of columns in source database. You can reorder columns in Dataedo using Designer.Reordering will not impact table in your source database.";
		titleTableColumnsGridColumn.ToolTip = CommonTooltips.GetTitle(SharedObjectTypeEnum.ObjectType.Column);
		dataTypeTableColumnsGridColumn.ToolTip = "Data type of column/field as defined in the source database/dataset.";
		referencesTableColumnsGridColumn.ToolTip = ColumnTooltips.References;
		nullableTableColumnsGridColumn.ToolTip = "Read - only indicator whether column is nullable in the source database.You can use custom fields to provide additional nullability information.";
		descriptionTableColumnsGridColumn.ToolTip = CommonTooltips.GetDescription(SharedObjectTypeEnum.ObjectType.Column);
		dataLinksGridColumn.ToolTip = ColumnTooltips.DataLink;
		identityTableColumnsGridColumn.ToolTip = "Read-only indicator whether column is identity/autoincrement/serial in the source database. You can use custom fields to provide additional information.";
		defaultComputedTableColumnsGridColumn.ToolTip = "Read-only definition of computed columns and default values imported from the source database. You can use custom fields to provide additional information about calculations, rules, etc.";
		createdGridColumn.ToolTip = "Data and time when column was first imported or manually created in Dataedo metadata repository. ";
		createdByGridColumn.ToolTip = "Dataedo user name who first imported or manually created in Dataedo metadata repository.";
		lastUpdatedGridColumn.ToolTip = "Data and time when any column/field data was last updated in Dataedo metadata repository.";
		lastUpdatedByGridColumn.ToolTip = "Dataedo user name who last updated in Dataedo metadata repository.";
		foreach (GridColumn item in tableColumnsGridView.Columns.Where((GridColumn x) => x.Tag is CustomFieldRowExtended))
		{
			item.ToolTip = (item.Tag as CustomFieldRowExtended).Description;
		}
	}

	private void SetKeysGridViewTooltips()
	{
		nameTableConstraintsGridColumn.ToolTip = "Name of the key/constraint in the source database.";
		columnsTableConstraintsGridColumn.ToolTip = "Column(s) that define this primary/unique key.";
		descriptionTableConstraintsGridColumn.ToolTip = KeyTooltips.Description;
	}

	private void ProfileColumnBarButtonItemAsync_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
		{
			using (UpgradeDataProfilingForm upgradeDataProfilingForm = new UpgradeDataProfilingForm())
			{
				upgradeDataProfilingForm.ShowDialog();
				return;
			}
		}
		if (DataProfilingUtils.CanViewDataProfilingForms(databaseType, FindForm()) && !DataProfilingUtils.CheckIfDataProfilingFormAlreadyOpened(FindForm()))
		{
			new DataProfilingForm().SetParameters(columnId: GetSelectedColumnRows().FirstOrDefault().Id, tables: new TableSimpleData(tableId, name, title, schema, ObjectType).Yield(), databaseId: DatabaseId, databaseTitle: databaseTitle, metadataEditorUserControl: base.MainControl, databaseType: databaseType);
		}
	}

	private void TableColumnsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		if (!(sender is GridView gridView))
		{
			return;
		}
		int[] selectedRows = gridView.GetSelectedRows();
		if (selectedRows.Length != 1 || !DataProfilingUtils.ObjectCanBeProfilled(currentTableRow))
		{
			base.MainControl.SetProfileColumnButtonVisibility(new BoolEventArgs(value: false));
			clearColumnProfilingDataBarButtonItem.Visibility = BarItemVisibility.Never;
			profileColumnBarButtonItem.Visibility = BarItemVisibility.Never;
		}
		else if (DataProfilingUtils.ObjectCanBeProfilled(gridView.GetRow(selectedRows.FirstOrDefault()) as ColumnRow))
		{
			if (DB.DataProfiling.IsDataProfilingDisabled())
			{
				base.MainControl.SetProfileColumnButtonVisibility(new BoolEventArgs(value: false));
				profileColumnBarButtonItem.Visibility = BarItemVisibility.Never;
				clearColumnProfilingDataBarButtonItem.Visibility = BarItemVisibility.Never;
			}
			else
			{
				base.MainControl.SetProfileColumnButtonVisibility(new BoolEventArgs(value: true));
				profileColumnBarButtonItem.Visibility = BarItemVisibility.Always;
				SetClearAllProfilingDataButtonVisibility();
			}
		}
	}

	private void RowDistributionSparkLineToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		SetSuggestedDescriptionTooltips(GetVisibleGridControl().MainView as GridView, e);
		SetRowDisctributionDescriptiomTooltip(GetVisibleGridControl().MainView as GridView, e);
	}

	private void SetRowDisctributionDescriptiomTooltip(GridView grid, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		ToolTipControlInfo toolTipControlInfo = null;
		GridHitInfo gridHitInfo = grid.CalcHitInfo(e.ControlMousePosition);
		if (gridHitInfo.RowHandle < 0 || gridHitInfo.Column == null)
		{
			return;
		}
		ColumnRow columnRow = grid.GetRow(gridHitInfo.RowHandle) as ColumnRow;
		if (columnRow == null)
		{
			return;
		}
		string obj = gridHitInfo.Column.Name;
		string text = "sparklineRowDistributionColumn";
		if (obj != text)
		{
			return;
		}
		ColumnProfiledDataObject columnProfiledDataObject = profiledColumns.Where(delegate(ColumnProfiledDataObject c)
		{
			if (columnRow?.Id == c?.ColumnId)
			{
				ColumnRow columnRow2 = columnRow;
				if (columnRow2 == null)
				{
					return false;
				}
				_ = columnRow2.Id;
				return true;
			}
			return false;
		}).FirstOrDefault();
		if (columnProfiledDataObject == null)
		{
			return;
		}
		RowStatsUserControlcs rowStatsUserControlcs = new RowStatsUserControlcs
		{
			Width = 302,
			Height = 125
		};
		rowStatsUserControlcs.ChangeSizeOfControl(150, 18, 150, 18);
		rowStatsUserControlcs.DoLeftPaddingManually();
		bool isToolTip = true;
		rowStatsUserControlcs.SetParameters(columnProfiledDataObject, isToolTip);
		rowStatsUserControlcs.BorderStyle = BorderStyle.FixedSingle;
		if (rowStatsUserControlcs.ShowThisUserControl)
		{
			e.Info = new ToolTipControlInfo();
			object obj4 = (e.Info.Object = (e.Info.Object = gridHitInfo.HitTest.ToString() + gridHitInfo.RowHandle));
			e.Info.FlyoutControl = rowStatsUserControlcs;
			if (toolTipControlInfo != null)
			{
				e.Info = toolTipControlInfo;
			}
		}
	}

	private void ClearColumnProfilingDataBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
			{
				return;
			}
			DateTime utcNow = DateTime.UtcNow;
			tableColumnsGridView.BeginDataUpdate();
			foreach (ColumnRow item in GetSelectedColumnRows()?.Where((ColumnRow c) => c.TableId.HasValue))
			{
				DB.DataProfiling.RemoveAllProfilingForSingleColumn(item.TableId.Value, item.Id, GetSplashScreenManager(), FindForm());
			}
			RefreshSparklines();
			string duration = ((int)(DateTime.UtcNow - utcNow).TotalSeconds).ToString();
			DataProfilingTrackingHelper.TrackDataProfilingCleared("ALL", duration);
		}
		finally
		{
			tableColumnsGridView.EndDataUpdate();
		}
	}

	private bool SetProfilingColumnsAvailability()
	{
		bool showInCustomizationForm;
		if (DB.DataProfiling.IsDataProfilingDisabled() || !Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
		{
			OptionsColumn optionsColumn = sparklineRowDistributionColumn.OptionsColumn;
			showInCustomizationForm = (sparklineTopValuesColumn.OptionsColumn.ShowInCustomizationForm = false);
			optionsColumn.ShowInCustomizationForm = showInCustomizationForm;
			return false;
		}
		OptionsColumn optionsColumn2 = sparklineRowDistributionColumn.OptionsColumn;
		showInCustomizationForm = (sparklineTopValuesColumn.OptionsColumn.ShowInCustomizationForm = true);
		optionsColumn2.ShowInCustomizationForm = showInCustomizationForm;
		return true;
	}

	public void FocusDataLineageTab(int? processId, bool selectDiagramTab = false, bool? showColumns = null)
	{
		tableXtraTabControl.SelectedTabPage = dataLineageXtraTabPage;
		dataLineageUserControl.SelectDataProcess(processId);
		dataLineageUserControl.ChangeColumnsVisibility(showColumns);
		if (selectDiagramTab)
		{
			dataLineageUserControl.SelectDiagramTab();
		}
	}

	private void ViewHistoryBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
		string fieldName = viewHistoryBarButtonItem?.Tag?.ToString();
		if (HistoryGeneralHelper.IsNotFieldForHistory(fieldName) || !(tableColumnsGridView.GetFocusedRow() is ColumnRow columnRow))
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			return;
		}
		titleAndDescriptionColumnsForHistory.TryGetValue(columnRow.Id, out var value);
		if (value == null)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			return;
		}
		try
		{
			using HistoryForm historyForm = new HistoryForm();
			historyForm.CustomFieldCaption = customFields?.CustomFieldsData?.Where((CustomFieldDefinition x) => x.CustomField.FieldName.ToLower() == fieldName)?.FirstOrDefault()?.CustomField?.Title;
			historyForm.SetParameters(columnRow.Id, fieldName, columnRow.Name, columnRow.TableSchema, base.DatabaseShowSchema, base.DatabaseShowSchemaOverride, value.Title, "columns", SharedObjectTypeEnum.TypeToString(columnRow.ObjectType), SharedObjectSubtypeEnum.TypeToString(columnRow.ObjectType, columnRow.ObjectSubtype), UserTypeEnum.TypeToString(columnRow.Source), null);
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			historyForm.ShowDialog(FindForm());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
		}
	}

	private void TableTitleTextEdit_Properties_BeforeShowMenu(object sender, BeforeShowMenuEventArgs e)
	{
		string viewHistoryMenuItemCaption = "View History";
		if (!e.Menu.Items.Any((DXMenuItem x) => x.Caption == viewHistoryMenuItemCaption))
		{
			DXMenuItem item = new DXMenuItem(viewHistoryMenuItemCaption, ViewHistoryClicked_DXMenuItem, Resources.search_16);
			e.Menu.Items.Add(item);
		}
	}

	public void ViewHistoryClicked_DXMenuItem(object sender, EventArgs e)
	{
		ViewHistoryForField("title");
	}

	public void ViewHistoryForField(string fieldName)
	{
		if (tableTitleAndDescriptionHistory == null)
		{
			return;
		}
		try
		{
			using HistoryForm historyForm = new HistoryForm();
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
			historyForm.CustomFieldCaption = customFields?.CustomFieldsData?.Where((CustomFieldDefinition x) => x.CustomField.FieldName.ToLower() == fieldName)?.FirstOrDefault()?.CustomField?.Title;
			historyForm.SetParameters(tableId, fieldName, name, schema, base.DatabaseShowSchema, base.DatabaseShowSchemaOverride, tableTitleAndDescriptionHistory.Title, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(ObjectType), SharedObjectTypeEnum.TypeToString(ObjectType), SharedObjectSubtypeEnum.TypeToString(ObjectType, base.Subtype), UserTypeEnum.TypeToString(source), null);
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			historyForm.ShowDialog(FindForm());
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void DocumentationModulesUserControl_Enter(object sender, EventArgs e)
	{
		if (!isModuleDropdownLoaded)
		{
			base.MainControl?.SetWaitformVisibility(visible: true);
			base.ModulesId = documentationModulesUserControl.GetCurrentRowModulesId(ObjectType, TableId);
			base.MainControl?.SetWaitformVisibility(visible: false);
			documentationModulesUserControl.Refresh();
			isModuleDropdownLoaded = true;
		}
	}

	private void CustomFieldsPanelControl_ShowHistoryClick(object sender, EventArgs e)
	{
		if (e is TextEventArgs textEventArgs && textEventArgs.Text.StartsWith("field"))
		{
			ViewHistoryForField(textEventArgs.Text);
		}
	}

	public void DataLineageButtonsVisibleChanged(object sender, EventArgs e)
	{
		base.MainControl.DataLineageTabVisibilityChanged(tableXtraTabControl.SelectedTabPage == dataLineageXtraTabPage && ObjectType != SharedObjectTypeEnum.ObjectType.Table && ObjectType != SharedObjectTypeEnum.ObjectType.Structure);
	}

	public DataLineageUserControl GetDataLineageControl()
	{
		return dataLineageUserControl;
	}

	private void ViewHistoryTriggersVarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
		string fieldName = viewHistoryTriggersBarButtonItem?.Tag?.ToString();
		if (HistoryGeneralHelper.IsNotFieldForHistory(fieldName) || !(tableTriggerGridView.GetFocusedRow() is TriggerRow triggerRow))
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			return;
		}
		try
		{
			using HistoryForm historyForm = new HistoryForm();
			string text = SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Trigger);
			Bitmap objectIcon = IconsSupport.GetObjectIcon(SharedObjectTypeEnum.ObjectType.Trigger, SharedObjectSubtypeEnum.ObjectSubtype.Trigger, triggerRow.Source, CommonFunctionsDatabase.IsDeletedFromDB(triggerRow) ? new SynchronizeStateEnum.SynchronizeState?(SynchronizeStateEnum.SynchronizeState.Deleted) : null, !triggerRow.Disabled);
			historyForm.CustomFieldCaption = customFields?.CustomFieldsData?.Where((CustomFieldDefinition x) => x.CustomField.FieldName.ToLower() == fieldName)?.FirstOrDefault()?.CustomField?.Title;
			historyForm.SetParameters(triggerRow.Id, fieldName, triggerRow.Name, triggerRow.TableSchema, base.DatabaseShowSchema, base.DatabaseShowSchemaOverride, triggerRow.Title, "triggers", text, text, UserTypeEnum.TypeToString(triggerRow.Source), objectIcon);
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			historyForm.ShowDialog(FindForm());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.PanelControls.TableUserControl));
		Dataedo.App.Tools.DefaultBulkCopy defaultBulkCopy = new Dataedo.App.Tools.DefaultBulkCopy();
		Dataedo.App.Tools.DefaultBulkCopy defaultBulkCopy2 = new Dataedo.App.Tools.DefaultBulkCopy();
		Dataedo.App.Tools.DefaultBulkCopy defaultBulkCopy3 = new Dataedo.App.Tools.DefaultBulkCopy();
		Dataedo.App.Tools.DefaultBulkCopy defaultBulkCopy4 = new Dataedo.App.Tools.DefaultBulkCopy();
		this.tableXtraTabControl = new DevExpress.XtraTab.XtraTabControl();
		this.tableDescriptionXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.tableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.searchCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.descriptionLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.documentationModulesUserControl = new Dataedo.App.UserControls.DocumentationModulesUserControl();
		this.documentationTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.schemaTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.customFieldsPanelControl = new Dataedo.App.UserControls.CustomFieldsPanelControl();
		this.tabHtmlUserControl = new Dataedo.App.UserControls.HtmlUserControl();
		this.tableTitleTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.nameTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.tableLocationTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.titleLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
		this.customFieldsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.nameLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.schemaLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.documentationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.modulesLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.tableLocationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.tableColumnListXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.tableColumnsGrid = new DevExpress.XtraGrid.GridControl();
		this.tableColumnsGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.iconTableColumnsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconTableColumnRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.keyTableColumnsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.keyTableColumnRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.ordinalPositionTableColumnsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameTableColumnsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.columnFullNameRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.titleTableColumnsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.titleColumnRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.dataTypeTableColumnsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.referencesTableColumnsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.referencesRepositoryItemAutoHeightMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.descriptionTableColumnsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionColumnRepositoryItemMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.sparklineRowDistributionColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.sparklineTopValuesColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dataLinksRepositoryItemAutoHeightMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.nullableTableColumnsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.checkRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.identityTableColumnsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.defaultComputedTableColumnsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.comutedRepositoryItemAutoHeightMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.createdGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.rowDistributionSparkLineToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.columnsGridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.tableRelationsXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.tableRelationsGrid = new DevExpress.XtraGrid.GridControl();
		this.tableRelationsGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.nameTableRelationsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameRalationRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.titleTableRelationsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.relationTitleRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.fkTableTableRelationsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.FKTableObjectNameCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.iconTableRelationsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.tableRelationRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.pkTableTableRelationsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.PKTableObjectNameCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.joinGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.joinRepositoryItemAutoHeightMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.descriptionTableRelationsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionRelationRepositoryItemMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.titleRelationRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.tableToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.relationsGridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.tableConstraintsXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.tableConstraintsGrid = new DevExpress.XtraGrid.GridControl();
		this.tableConstraintsGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.iconTableConstraintsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.typeRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.nameTableConstraintsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.columnsTableConstraintsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.columnFullNameFormattedRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.descriptionTableConstraintsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionConstraintRepositoryItemMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.constraintsGridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.tableTriggersXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.triggersSplitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
		this.tableTriggerGrid = new DevExpress.XtraGrid.GridControl();
		this.tableTriggerGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.iconTableTriggersGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconTriggerRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.nameTableTriggersGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.whenTableTriggersGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionTableTriggersGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionTriggerRepositoryItemMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.onInsertRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.onUpdateRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.onDeleteRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.layoutControl2 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.scriptLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.triggerScriptRichEditControl = new Dataedo.App.UserControls.RichEditUserControl();
		this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.triggersGridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.dataLineageXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.dataLineageUserControl = new Dataedo.App.UserControls.DataLineage.DataLineageUserControl();
		this.dependenciesXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.dependenciesUserControl = new Dataedo.App.UserControls.Dependencies.DependenciesUserControl();
		this.tableScriptXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.nonCustomizableLayoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.scriptTextSearchCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.scriptRichEditControl = new Dataedo.App.UserControls.RichEditUserControl();
		this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
		this.tableSchemaXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.schemaTextLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.schemaTextSearchCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.schemaTextLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.schemaTextRichEditUserControl = new Dataedo.App.UserControls.RichEditUserControl();
		this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem13 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
		this.dataLinksXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.dataLinksGrid = new DevExpress.XtraGrid.GridControl();
		this.dataLinksGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.termIconDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.objectDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.typeDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.objectIconDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.tableColumnDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.fullNameRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.createdDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdByDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedByDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemAutoHeightMemoEdit1 = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.dataLinksGridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.termsLinkUserControl = new Dataedo.App.UserControls.LinkUserControl();
		this.schemaImportsAndChangesXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.schemaImportsAndChangesUserControl = new Dataedo.App.UserControls.SchemaImportsAndChanges.SchemaImportsAndChangesUserControl();
		this.metadataXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.metadataLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.dbmsLastUpdatedLabel = new DevExpress.XtraEditors.LabelControl();
		this.lastSynchronizedLabel = new DevExpress.XtraEditors.LabelControl();
		this.lastUpdatedLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.dbmsCreatedLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.createdLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.dbmsLastUpdatedLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.lastImportedLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.lastUpdatedLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.dbmsCreatedLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.firstImportedLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.metadataToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.treeMenuImageCollection = new DevExpress.Utils.ImageCollection(this.components);
		this.relationNameErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
		this.tableStatusUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.tableTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.linkedTermsPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.goToObjectLinkedTermsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.editLinksLinkedTermsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeLinkLinkedTermsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.linkedTermsBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.uniqueKeysPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.addPrimaryKeyUniqueKeysBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.addUniqueKeyUniqueKeysBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.editKeyUniqueKeysBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeKeyUniqueKeysBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.viewHistoryUniqueKeysBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.uniqueKeysBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
		this.triggersPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.removeTriggerTriggersBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.viewHistoryTriggersBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.triggersBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControl5 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl6 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl7 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl8 = new DevExpress.XtraBars.BarDockControl();
		this.relationsPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.addRelationRelationsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.editRelationRelationsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeRelationRelationsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.goToObjectRelationsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.viewHistoryRelationsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.relationsBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControl9 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl10 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl11 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl12 = new DevExpress.XtraBars.BarDockControl();
		this.columnsPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.addRelationColumnsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.profileColumnBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.clearColumnProfilingDataBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.designTableColumnsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.editLinksColumnsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.addNewLinkedTermColumnsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeColumnsColumnsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.viewHistoryBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.columnsBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControl13 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl14 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl15 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl16 = new DevExpress.XtraBars.BarDockControl();
		this.barSubItem1 = new DevExpress.XtraBars.BarSubItem();
		((System.ComponentModel.ISupportInitialize)this.tableXtraTabControl).BeginInit();
		this.tableXtraTabControl.SuspendLayout();
		this.tableDescriptionXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tableLayoutControl).BeginInit();
		this.tableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.documentationTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.schemaTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableTitleTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableLocationTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.schemaLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.documentationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.modulesLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableLocationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		this.tableColumnListXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tableColumnsGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableColumnsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconTableColumnRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.keyTableColumnRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnFullNameRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleColumnRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.referencesRepositoryItemAutoHeightMemoEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionColumnRepositoryItemMemoEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataLinksRepositoryItemAutoHeightMemoEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.comutedRepositoryItemAutoHeightMemoEdit).BeginInit();
		this.tableRelationsXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tableRelationsGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableRelationsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameRalationRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.relationTitleRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.FKTableObjectNameCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableRelationRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PKTableObjectNameCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.joinRepositoryItemAutoHeightMemoEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionRelationRepositoryItemMemoEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleRelationRepositoryItemTextEdit).BeginInit();
		this.tableConstraintsXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tableConstraintsGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableConstraintsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnFullNameFormattedRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionConstraintRepositoryItemMemoEdit).BeginInit();
		this.tableTriggersXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.triggersSplitContainerControl).BeginInit();
		this.triggersSplitContainerControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tableTriggerGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableTriggerGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconTriggerRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionTriggerRepositoryItemMemoEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.onInsertRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.onUpdateRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.onDeleteRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).BeginInit();
		this.layoutControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		this.dataLineageXtraTabPage.SuspendLayout();
		this.dependenciesXtraTabPage.SuspendLayout();
		this.tableScriptXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).BeginInit();
		this.nonCustomizableLayoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).BeginInit();
		this.tableSchemaXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.schemaTextLayoutControl).BeginInit();
		this.schemaTextLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem11).BeginInit();
		this.dataLinksXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataLinksGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataLinksGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fullNameRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemAutoHeightMemoEdit1).BeginInit();
		this.schemaImportsAndChangesXtraTabPage.SuspendLayout();
		this.metadataXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.metadataLayoutControl).BeginInit();
		this.metadataLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsLastUpdatedLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lastImportedLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lastUpdatedLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsCreatedLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.firstImportedLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.treeMenuImageCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.relationNameErrorProvider).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tableTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.linkedTermsPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.linkedTermsBarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.uniqueKeysPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.uniqueKeysBarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.triggersPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.triggersBarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.relationsPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.relationsBarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnsPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnsBarManager).BeginInit();
		base.SuspendLayout();
		this.tableXtraTabControl.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.tableXtraTabControl.Appearance.Options.UseBackColor = true;
		this.tableXtraTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableXtraTabControl.Location = new System.Drawing.Point(0, 60);
		this.tableXtraTabControl.Name = "tableXtraTabControl";
		this.tableXtraTabControl.SelectedTabPage = this.tableDescriptionXtraTabPage;
		this.tableXtraTabControl.Size = new System.Drawing.Size(881, 580);
		this.tableXtraTabControl.TabIndex = 1;
		this.tableXtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[12]
		{
			this.tableDescriptionXtraTabPage, this.tableColumnListXtraTabPage, this.tableRelationsXtraTabPage, this.tableConstraintsXtraTabPage, this.tableTriggersXtraTabPage, this.dataLineageXtraTabPage, this.dependenciesXtraTabPage, this.tableScriptXtraTabPage, this.tableSchemaXtraTabPage, this.dataLinksXtraTabPage,
			this.schemaImportsAndChangesXtraTabPage, this.metadataXtraTabPage
		});
		this.tableXtraTabControl.ToolTipController = this.toolTipController;
		this.tableXtraTabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(tableXtraTabControl_SelectedPageChanged);
		this.tableDescriptionXtraTabPage.Controls.Add(this.tableLayoutControl);
		this.tableDescriptionXtraTabPage.Name = "tableDescriptionXtraTabPage";
		this.tableDescriptionXtraTabPage.Size = new System.Drawing.Size(879, 555);
		this.tableDescriptionXtraTabPage.Text = "Table";
		this.tableLayoutControl.AllowCustomization = false;
		this.tableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.tableLayoutControl.Controls.Add(this.searchCountLabelControl);
		this.tableLayoutControl.Controls.Add(this.descriptionLabelControl);
		this.tableLayoutControl.Controls.Add(this.documentationModulesUserControl);
		this.tableLayoutControl.Controls.Add(this.documentationTextEdit);
		this.tableLayoutControl.Controls.Add(this.schemaTextEdit);
		this.tableLayoutControl.Controls.Add(this.customFieldsPanelControl);
		this.tableLayoutControl.Controls.Add(this.tabHtmlUserControl);
		this.tableLayoutControl.Controls.Add(this.tableTitleTextEdit);
		this.tableLayoutControl.Controls.Add(this.nameTextEdit);
		this.tableLayoutControl.Controls.Add(this.tableLocationTextEdit);
		this.tableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.tableLayoutControl.Name = "tableLayoutControl";
		this.tableLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3250, 272, 914, 544);
		this.tableLayoutControl.Root = this.layoutControlGroup1;
		this.tableLayoutControl.Size = new System.Drawing.Size(879, 555);
		this.tableLayoutControl.TabIndex = 0;
		this.tableLayoutControl.Text = "layoutControl1";
		this.tableLayoutControl.ToolTipController = this.toolTipController;
		this.searchCountLabelControl.Location = new System.Drawing.Point(69, 204);
		this.searchCountLabelControl.Name = "searchCountLabelControl";
		this.searchCountLabelControl.Size = new System.Drawing.Size(798, 13);
		this.searchCountLabelControl.StyleController = this.tableLayoutControl;
		this.searchCountLabelControl.TabIndex = 10;
		this.descriptionLabelControl.Location = new System.Drawing.Point(12, 204);
		this.descriptionLabelControl.Name = "descriptionLabelControl";
		this.descriptionLabelControl.Size = new System.Drawing.Size(53, 13);
		this.descriptionLabelControl.StyleController = this.tableLayoutControl;
		this.descriptionLabelControl.TabIndex = 9;
		this.descriptionLabelControl.Text = "Description";
		this.descriptionLabelControl.ToolTipController = this.toolTipController;
		this.toolTipController.AutoPopDelay = 100000;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.documentationModulesUserControl.BackColor = System.Drawing.Color.Transparent;
		this.documentationModulesUserControl.IsChanged = false;
		this.documentationModulesUserControl.Location = new System.Drawing.Point(97, 136);
		this.documentationModulesUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.documentationModulesUserControl.MaximumSize = new System.Drawing.Size(500, 24);
		this.documentationModulesUserControl.MinimumSize = new System.Drawing.Size(300, 24);
		this.documentationModulesUserControl.Modules = null;
		this.documentationModulesUserControl.Name = "documentationModulesUserControl";
		this.documentationModulesUserControl.Size = new System.Drawing.Size(411, 24);
		this.documentationModulesUserControl.TabIndex = 6;
		this.documentationModulesUserControl.EditValueChanged += new System.EventHandler(documentationModulesUserControl_EditValueChanged);
		this.documentationModulesUserControl.Enter += new System.EventHandler(DocumentationModulesUserControl_Enter);
		this.documentationTextEdit.Location = new System.Drawing.Point(94, 12);
		this.documentationTextEdit.Name = "documentationTextEdit";
		this.documentationTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.documentationTextEdit.Properties.ReadOnly = true;
		this.documentationTextEdit.Size = new System.Drawing.Size(773, 18);
		this.documentationTextEdit.StyleController = this.tableLayoutControl;
		this.documentationTextEdit.TabIndex = 2;
		this.documentationTextEdit.TabStop = false;
		this.schemaTextEdit.Location = new System.Drawing.Point(94, 36);
		this.schemaTextEdit.Name = "schemaTextEdit";
		this.schemaTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.schemaTextEdit.Properties.ReadOnly = true;
		this.schemaTextEdit.Size = new System.Drawing.Size(773, 18);
		this.schemaTextEdit.StyleController = this.tableLayoutControl;
		this.schemaTextEdit.TabIndex = 3;
		this.schemaTextEdit.TabStop = false;
		this.customFieldsPanelControl.BackColor = System.Drawing.Color.Transparent;
		this.customFieldsPanelControl.Location = new System.Drawing.Point(10, 162);
		this.customFieldsPanelControl.Margin = new System.Windows.Forms.Padding(0);
		this.customFieldsPanelControl.Name = "customFieldsPanelControl";
		this.customFieldsPanelControl.Size = new System.Drawing.Size(857, 38);
		this.customFieldsPanelControl.TabIndex = 7;
		this.tabHtmlUserControl.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.tabHtmlUserControl.CanListen = false;
		this.tabHtmlUserControl.DatabaseRow = null;
		this.tabHtmlUserControl.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.tabHtmlUserControl.HtmlText = resources.GetString("tabHtmlUserControl.HtmlText");
		this.tabHtmlUserControl.IsHighlighted = false;
		this.tabHtmlUserControl.Location = new System.Drawing.Point(12, 221);
		this.tabHtmlUserControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.tabHtmlUserControl.MinimumSize = new System.Drawing.Size(0, 117);
		this.tabHtmlUserControl.ModuleRow = null;
		this.tabHtmlUserControl.Name = "tabHtmlUserControl";
		this.tabHtmlUserControl.OccurrencesCount = 0;
		this.tabHtmlUserControl.OriginalHtmlText = resources.GetString("tabHtmlUserControl.OriginalHtmlText");
		this.tabHtmlUserControl.PlainText = "\u00a0";
		this.tabHtmlUserControl.ProcedureObject = null;
		this.tabHtmlUserControl.Size = new System.Drawing.Size(855, 332);
		this.tabHtmlUserControl.SplashScreenManager = null;
		this.tabHtmlUserControl.TabIndex = 8;
		this.tabHtmlUserControl.TableRow = null;
		this.tabHtmlUserControl.TermObject = null;
		this.tableTitleTextEdit.Location = new System.Drawing.Point(97, 84);
		this.tableTitleTextEdit.MaximumSize = new System.Drawing.Size(0, 22);
		this.tableTitleTextEdit.MinimumSize = new System.Drawing.Size(0, 22);
		this.tableTitleTextEdit.Name = "tableTitleTextEdit";
		this.tableTitleTextEdit.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(TableTitleTextEdit_Properties_BeforeShowMenu);
		this.tableTitleTextEdit.Size = new System.Drawing.Size(411, 22);
		this.tableTitleTextEdit.StyleController = this.tableLayoutControl;
		this.tableTitleTextEdit.TabIndex = 5;
		this.tableTitleTextEdit.EditValueChanged += new System.EventHandler(tableTitleTextEdit_EditValueChanged);
		this.nameTextEdit.Location = new System.Drawing.Point(94, 60);
		this.nameTextEdit.Name = "nameTextEdit";
		this.nameTextEdit.Properties.AllowFocused = false;
		this.nameTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.nameTextEdit.Properties.ReadOnly = true;
		this.nameTextEdit.Size = new System.Drawing.Size(773, 18);
		this.nameTextEdit.StyleController = this.tableLayoutControl;
		this.nameTextEdit.TabIndex = 4;
		this.nameTextEdit.TabStop = false;
		this.tableLocationTextEdit.Location = new System.Drawing.Point(97, 110);
		this.tableLocationTextEdit.MaximumSize = new System.Drawing.Size(0, 22);
		this.tableLocationTextEdit.MinimumSize = new System.Drawing.Size(0, 22);
		this.tableLocationTextEdit.Name = "tableLocationTextEdit";
		this.tableLocationTextEdit.Size = new System.Drawing.Size(411, 22);
		this.tableLocationTextEdit.StyleController = this.tableLayoutControl;
		this.tableLocationTextEdit.TabIndex = 5;
		this.tableLocationTextEdit.EditValueChanged += new System.EventHandler(tableLocationTextEdit_EditValueChanged);
		this.layoutControlGroup1.CustomizationFormText = "Root";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[12]
		{
			this.titleLayoutControlItem, this.emptySpaceItem2, this.emptySpaceItem1, this.layoutControlItem8, this.customFieldsLayoutControlItem, this.nameLayoutControlItem, this.schemaLayoutControlItem, this.documentationLayoutControlItem, this.modulesLayoutControlItem, this.tableLocationLayoutControlItem,
			this.layoutControlItem2, this.layoutControlItem3
		});
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(879, 555);
		this.layoutControlGroup1.TextVisible = false;
		this.titleLayoutControlItem.Control = this.tableTitleTextEdit;
		this.titleLayoutControlItem.CustomizationFormText = "Title:";
		this.titleLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.titleLayoutControlItem.MaxSize = new System.Drawing.Size(500, 26);
		this.titleLayoutControlItem.MinSize = new System.Drawing.Size(500, 26);
		this.titleLayoutControlItem.Name = "titleLayoutControlItem";
		this.titleLayoutControlItem.Size = new System.Drawing.Size(500, 26);
		this.titleLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.titleLayoutControlItem.Text = "Title";
		this.titleLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.titleLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.titleLayoutControlItem.TextToControlDistance = 3;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem2.Location = new System.Drawing.Point(500, 124);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(104, 24);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(359, 26);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
		this.emptySpaceItem1.Location = new System.Drawing.Point(500, 72);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(359, 26);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.Control = this.tabHtmlUserControl;
		this.layoutControlItem8.Location = new System.Drawing.Point(0, 209);
		this.layoutControlItem8.MinSize = new System.Drawing.Size(57, 300);
		this.layoutControlItem8.Name = "layoutControlItem8";
		this.layoutControlItem8.Size = new System.Drawing.Size(859, 336);
		this.layoutControlItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem8.Text = "Description";
		this.layoutControlItem8.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.TextVisible = false;
		this.customFieldsLayoutControlItem.Control = this.customFieldsPanelControl;
		this.customFieldsLayoutControlItem.Location = new System.Drawing.Point(0, 150);
		this.customFieldsLayoutControlItem.Name = "customFieldsLayoutControlItem";
		this.customFieldsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
		this.customFieldsLayoutControlItem.Size = new System.Drawing.Size(859, 42);
		this.customFieldsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.customFieldsLayoutControlItem.TextVisible = false;
		this.nameLayoutControlItem.Control = this.nameTextEdit;
		this.nameLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.nameLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.nameLayoutControlItem.MinSize = new System.Drawing.Size(150, 24);
		this.nameLayoutControlItem.Name = "nameLayoutControlItem";
		this.nameLayoutControlItem.Size = new System.Drawing.Size(859, 24);
		this.nameLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.nameLayoutControlItem.Text = "Name";
		this.nameLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.nameLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.nameLayoutControlItem.TextToControlDistance = 0;
		this.schemaLayoutControlItem.Control = this.schemaTextEdit;
		this.schemaLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.schemaLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.schemaLayoutControlItem.MinSize = new System.Drawing.Size(150, 24);
		this.schemaLayoutControlItem.Name = "schemaLayoutControlItem";
		this.schemaLayoutControlItem.Size = new System.Drawing.Size(859, 24);
		this.schemaLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.schemaLayoutControlItem.Text = "Schema";
		this.schemaLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.schemaLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.schemaLayoutControlItem.TextToControlDistance = 0;
		this.documentationLayoutControlItem.Control = this.documentationTextEdit;
		this.documentationLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.documentationLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.documentationLayoutControlItem.MinSize = new System.Drawing.Size(150, 24);
		this.documentationLayoutControlItem.Name = "documentationLayoutControlItem";
		this.documentationLayoutControlItem.Size = new System.Drawing.Size(859, 24);
		this.documentationLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.documentationLayoutControlItem.Text = "Database";
		this.documentationLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.documentationLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.documentationLayoutControlItem.TextToControlDistance = 0;
		this.modulesLayoutControlItem.Control = this.documentationModulesUserControl;
		this.modulesLayoutControlItem.Location = new System.Drawing.Point(0, 124);
		this.modulesLayoutControlItem.MaxSize = new System.Drawing.Size(500, 26);
		this.modulesLayoutControlItem.MinSize = new System.Drawing.Size(500, 26);
		this.modulesLayoutControlItem.Name = "modulesLayoutControlItem";
		this.modulesLayoutControlItem.Size = new System.Drawing.Size(500, 26);
		this.modulesLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.modulesLayoutControlItem.Text = "Subject Area";
		this.modulesLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.modulesLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.modulesLayoutControlItem.TextToControlDistance = 3;
		this.tableLocationLayoutControlItem.Control = this.tableLocationTextEdit;
		this.tableLocationLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.tableLocationLayoutControlItem.CustomizationFormText = "Title:";
		this.tableLocationLayoutControlItem.Location = new System.Drawing.Point(0, 98);
		this.tableLocationLayoutControlItem.MaxSize = new System.Drawing.Size(500, 26);
		this.tableLocationLayoutControlItem.MinSize = new System.Drawing.Size(500, 26);
		this.tableLocationLayoutControlItem.Name = "tableLocationLayoutControlItem";
		this.tableLocationLayoutControlItem.Size = new System.Drawing.Size(859, 26);
		this.tableLocationLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.tableLocationLayoutControlItem.Text = "Location";
		this.tableLocationLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.tableLocationLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.tableLocationLayoutControlItem.TextToControlDistance = 3;
		this.layoutControlItem2.Control = this.descriptionLabelControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 192);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(57, 17);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.Control = this.searchCountLabelControl;
		this.layoutControlItem3.Location = new System.Drawing.Point(57, 192);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(802, 17);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.tableColumnListXtraTabPage.Controls.Add(this.tableColumnsGrid);
		this.tableColumnListXtraTabPage.Controls.Add(this.columnsGridPanelUserControl);
		this.tableColumnListXtraTabPage.Name = "tableColumnListXtraTabPage";
		this.tableColumnListXtraTabPage.Size = new System.Drawing.Size(879, 555);
		this.tableColumnListXtraTabPage.Text = "Columns";
		this.tableColumnsGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.tableColumnsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableColumnsGrid.Location = new System.Drawing.Point(0, 30);
		this.tableColumnsGrid.MainView = this.tableColumnsGridView;
		this.tableColumnsGrid.Name = "tableColumnsGrid";
		this.tableColumnsGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.iconTableColumnRepositoryItemPictureEdit, this.keyTableColumnRepositoryItemPictureEdit, this.checkRepositoryItemCheckEdit, this.descriptionColumnRepositoryItemMemoEdit, this.titleColumnRepositoryItemCustomTextEdit, this.referencesRepositoryItemAutoHeightMemoEdit, this.dataLinksRepositoryItemAutoHeightMemoEdit, this.columnFullNameRepositoryItemCustomTextEdit, this.comutedRepositoryItemAutoHeightMemoEdit });
		this.tableColumnsGrid.ShowOnlyPredefinedDetails = true;
		this.tableColumnsGrid.Size = new System.Drawing.Size(879, 525);
		this.tableColumnsGrid.TabIndex = 0;
		this.tableColumnsGrid.ToolTipController = this.rowDistributionSparkLineToolTipController;
		this.tableColumnsGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tableColumnsGridView });
		this.tableColumnsGridView.Appearance.Row.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.tableColumnsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[18]
		{
			this.iconTableColumnsGridColumn, this.keyTableColumnsGridColumn, this.ordinalPositionTableColumnsGridColumn, this.nameTableColumnsGridColumn, this.titleTableColumnsGridColumn, this.dataTypeTableColumnsGridColumn, this.referencesTableColumnsGridColumn, this.descriptionTableColumnsGridColumn, this.sparklineRowDistributionColumn, this.sparklineTopValuesColumn,
			this.dataLinksGridColumn, this.nullableTableColumnsGridColumn, this.identityTableColumnsGridColumn, this.defaultComputedTableColumnsGridColumn, this.createdGridColumn, this.createdByGridColumn, this.lastUpdatedGridColumn, this.lastUpdatedByGridColumn
		});
		defaultBulkCopy.IsCopying = false;
		this.tableColumnsGridView.Copy = defaultBulkCopy;
		this.tableColumnsGridView.GridControl = this.tableColumnsGrid;
		this.tableColumnsGridView.Name = "tableColumnsGridView";
		this.tableColumnsGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.tableColumnsGridView.OptionsFilter.ShowAllTableValuesInFilterPopup = true;
		this.tableColumnsGridView.OptionsFilter.UseNewCustomFilterDialog = true;
		this.tableColumnsGridView.OptionsSelection.MultiSelect = true;
		this.tableColumnsGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.tableColumnsGridView.OptionsView.ColumnAutoWidth = false;
		this.tableColumnsGridView.OptionsView.RowAutoHeight = true;
		this.tableColumnsGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.tableColumnsGridView.OptionsView.ShowGroupPanel = false;
		this.tableColumnsGridView.OptionsView.ShowIndicator = false;
		this.tableColumnsGridView.RowHeight = 20;
		this.tableColumnsGridView.RowHighlightingIsEnabled = true;
		this.tableColumnsGridView.SplashScreenManager = null;
		this.tableColumnsGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(GridView_CustomDrawCell);
		this.tableColumnsGridView.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(highlightGridView_RowCellStyle);
		this.tableColumnsGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(tableColumnsGridView_PopupMenuShowing);
		this.tableColumnsGridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(TableColumnsGridView_SelectionChanged);
		this.tableColumnsGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(tableColumnsGridView_CellValueChanged);
		this.tableColumnsGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(TableColumnsGridView_CellValueChanging);
		this.tableColumnsGridView.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(tableColumnsGridView_CustomColumnSort);
		this.tableColumnsGridView.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(tableColumnsGridView_CustomUnboundColumnData);
		this.iconTableColumnsGridColumn.Caption = " ";
		this.iconTableColumnsGridColumn.ColumnEdit = this.iconTableColumnRepositoryItemPictureEdit;
		this.iconTableColumnsGridColumn.FieldName = "iconTableColumnsGridColumn";
		this.iconTableColumnsGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.iconTableColumnsGridColumn.MaxWidth = 21;
		this.iconTableColumnsGridColumn.MinWidth = 21;
		this.iconTableColumnsGridColumn.Name = "iconTableColumnsGridColumn";
		this.iconTableColumnsGridColumn.OptionsColumn.AllowEdit = false;
		this.iconTableColumnsGridColumn.OptionsColumn.ReadOnly = true;
		this.iconTableColumnsGridColumn.OptionsFilter.AllowAutoFilter = false;
		this.iconTableColumnsGridColumn.OptionsFilter.AllowFilter = false;
		this.iconTableColumnsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.iconTableColumnsGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.iconTableColumnsGridColumn.Visible = true;
		this.iconTableColumnsGridColumn.VisibleIndex = 0;
		this.iconTableColumnsGridColumn.Width = 21;
		this.iconTableColumnRepositoryItemPictureEdit.AllowFocused = false;
		this.iconTableColumnRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.iconTableColumnRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconTableColumnRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconTableColumnRepositoryItemPictureEdit.Name = "iconTableColumnRepositoryItemPictureEdit";
		this.iconTableColumnRepositoryItemPictureEdit.ShowMenu = false;
		this.keyTableColumnsGridColumn.Caption = "Key";
		this.keyTableColumnsGridColumn.ColumnEdit = this.keyTableColumnRepositoryItemPictureEdit;
		this.keyTableColumnsGridColumn.FieldName = "UniqueConstraintIcon";
		this.keyTableColumnsGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.keyTableColumnsGridColumn.MaxWidth = 30;
		this.keyTableColumnsGridColumn.MinWidth = 30;
		this.keyTableColumnsGridColumn.Name = "keyTableColumnsGridColumn";
		this.keyTableColumnsGridColumn.OptionsColumn.AllowEdit = false;
		this.keyTableColumnsGridColumn.OptionsColumn.ReadOnly = true;
		this.keyTableColumnsGridColumn.OptionsFilter.AllowAutoFilter = false;
		this.keyTableColumnsGridColumn.OptionsFilter.AllowFilter = false;
		this.keyTableColumnsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.keyTableColumnsGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.keyTableColumnsGridColumn.Visible = true;
		this.keyTableColumnsGridColumn.VisibleIndex = 1;
		this.keyTableColumnsGridColumn.Width = 30;
		this.keyTableColumnRepositoryItemPictureEdit.AllowFocused = false;
		this.keyTableColumnRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.keyTableColumnRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.keyTableColumnRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.keyTableColumnRepositoryItemPictureEdit.Name = "keyTableColumnRepositoryItemPictureEdit";
		this.keyTableColumnRepositoryItemPictureEdit.ShowMenu = false;
		this.ordinalPositionTableColumnsGridColumn.Caption = "#";
		this.ordinalPositionTableColumnsGridColumn.FieldName = "DisplayPosition";
		this.ordinalPositionTableColumnsGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.ordinalPositionTableColumnsGridColumn.MaxWidth = 100;
		this.ordinalPositionTableColumnsGridColumn.MinWidth = 30;
		this.ordinalPositionTableColumnsGridColumn.Name = "ordinalPositionTableColumnsGridColumn";
		this.ordinalPositionTableColumnsGridColumn.OptionsColumn.AllowEdit = false;
		this.ordinalPositionTableColumnsGridColumn.OptionsColumn.ReadOnly = true;
		this.ordinalPositionTableColumnsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.ordinalPositionTableColumnsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.ordinalPositionTableColumnsGridColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.ordinalPositionTableColumnsGridColumn.Tag = "FIT_WIDTH";
		this.ordinalPositionTableColumnsGridColumn.Visible = true;
		this.ordinalPositionTableColumnsGridColumn.VisibleIndex = 2;
		this.ordinalPositionTableColumnsGridColumn.Width = 30;
		this.nameTableColumnsGridColumn.Caption = "Name";
		this.nameTableColumnsGridColumn.ColumnEdit = this.columnFullNameRepositoryItemCustomTextEdit;
		this.nameTableColumnsGridColumn.FieldName = "FullNameFormatted";
		this.nameTableColumnsGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.nameTableColumnsGridColumn.Name = "nameTableColumnsGridColumn";
		this.nameTableColumnsGridColumn.OptionsColumn.AllowEdit = false;
		this.nameTableColumnsGridColumn.OptionsColumn.ReadOnly = true;
		this.nameTableColumnsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.nameTableColumnsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.nameTableColumnsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.nameTableColumnsGridColumn.Tag = "FIT_WIDTH";
		this.nameTableColumnsGridColumn.Visible = true;
		this.nameTableColumnsGridColumn.VisibleIndex = 3;
		this.nameTableColumnsGridColumn.Width = 140;
		this.columnFullNameRepositoryItemCustomTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.columnFullNameRepositoryItemCustomTextEdit.AutoHeight = false;
		this.columnFullNameRepositoryItemCustomTextEdit.Name = "columnFullNameRepositoryItemCustomTextEdit";
		this.titleTableColumnsGridColumn.Caption = "Title";
		this.titleTableColumnsGridColumn.ColumnEdit = this.titleColumnRepositoryItemCustomTextEdit;
		this.titleTableColumnsGridColumn.FieldName = "Title";
		this.titleTableColumnsGridColumn.Name = "titleTableColumnsGridColumn";
		this.titleTableColumnsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.titleTableColumnsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.titleTableColumnsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.titleTableColumnsGridColumn.Tag = "FIT_WIDTH";
		this.titleTableColumnsGridColumn.Visible = true;
		this.titleTableColumnsGridColumn.VisibleIndex = 4;
		this.titleTableColumnsGridColumn.Width = 140;
		this.titleColumnRepositoryItemCustomTextEdit.AutoHeight = false;
		this.titleColumnRepositoryItemCustomTextEdit.Name = "titleColumnRepositoryItemCustomTextEdit";
		this.dataTypeTableColumnsGridColumn.Caption = "Data type";
		this.dataTypeTableColumnsGridColumn.FieldName = "DataType";
		this.dataTypeTableColumnsGridColumn.Name = "dataTypeTableColumnsGridColumn";
		this.dataTypeTableColumnsGridColumn.OptionsColumn.AllowEdit = false;
		this.dataTypeTableColumnsGridColumn.OptionsColumn.ReadOnly = true;
		this.dataTypeTableColumnsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.dataTypeTableColumnsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.dataTypeTableColumnsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.dataTypeTableColumnsGridColumn.Tag = "FIT_WIDTH";
		this.dataTypeTableColumnsGridColumn.Visible = true;
		this.dataTypeTableColumnsGridColumn.VisibleIndex = 5;
		this.dataTypeTableColumnsGridColumn.Width = 100;
		this.referencesTableColumnsGridColumn.Caption = "References";
		this.referencesTableColumnsGridColumn.ColumnEdit = this.referencesRepositoryItemAutoHeightMemoEdit;
		this.referencesTableColumnsGridColumn.FieldName = "ReferencesString";
		this.referencesTableColumnsGridColumn.Name = "referencesTableColumnsGridColumn";
		this.referencesTableColumnsGridColumn.OptionsColumn.AllowEdit = false;
		this.referencesTableColumnsGridColumn.OptionsColumn.ReadOnly = true;
		this.referencesTableColumnsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.referencesTableColumnsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.referencesTableColumnsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.referencesTableColumnsGridColumn.Tag = "FIT_WIDTH";
		this.referencesTableColumnsGridColumn.Visible = true;
		this.referencesTableColumnsGridColumn.VisibleIndex = 6;
		this.referencesTableColumnsGridColumn.Width = 150;
		this.referencesRepositoryItemAutoHeightMemoEdit.Name = "referencesRepositoryItemAutoHeightMemoEdit";
		this.referencesRepositoryItemAutoHeightMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.descriptionTableColumnsGridColumn.AppearanceCell.Options.UseTextOptions = true;
		this.descriptionTableColumnsGridColumn.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.descriptionTableColumnsGridColumn.Caption = "Description";
		this.descriptionTableColumnsGridColumn.ColumnEdit = this.descriptionColumnRepositoryItemMemoEdit;
		this.descriptionTableColumnsGridColumn.FieldName = "Description";
		this.descriptionTableColumnsGridColumn.MaxWidth = 1000;
		this.descriptionTableColumnsGridColumn.MinWidth = 200;
		this.descriptionTableColumnsGridColumn.Name = "descriptionTableColumnsGridColumn";
		this.descriptionTableColumnsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.descriptionTableColumnsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.descriptionTableColumnsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.descriptionTableColumnsGridColumn.Tag = "FIT_WIDTH";
		this.descriptionTableColumnsGridColumn.Visible = true;
		this.descriptionTableColumnsGridColumn.VisibleIndex = 8;
		this.descriptionTableColumnsGridColumn.Width = 400;
		this.descriptionColumnRepositoryItemMemoEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.descriptionColumnRepositoryItemMemoEdit.Name = "descriptionColumnRepositoryItemMemoEdit";
		this.descriptionColumnRepositoryItemMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.sparklineRowDistributionColumn.AppearanceCell.BackColor = System.Drawing.Color.Transparent;
		this.sparklineRowDistributionColumn.AppearanceCell.ForeColor = System.Drawing.Color.White;
		this.sparklineRowDistributionColumn.AppearanceCell.Options.UseBackColor = true;
		this.sparklineRowDistributionColumn.AppearanceCell.Options.UseForeColor = true;
		this.sparklineRowDistributionColumn.AppearanceCell.Options.UseTextOptions = true;
		this.sparklineRowDistributionColumn.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.sparklineRowDistributionColumn.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.sparklineRowDistributionColumn.Caption = "Row distribution";
		this.sparklineRowDistributionColumn.FieldName = "SparklineRowDistributionText";
		this.sparklineRowDistributionColumn.MaxWidth = 110;
		this.sparklineRowDistributionColumn.MinWidth = 110;
		this.sparklineRowDistributionColumn.Name = "sparklineRowDistributionColumn";
		this.sparklineRowDistributionColumn.OptionsColumn.AllowEdit = false;
		this.sparklineRowDistributionColumn.OptionsColumn.ReadOnly = true;
		this.sparklineRowDistributionColumn.Visible = true;
		this.sparklineRowDistributionColumn.VisibleIndex = 9;
		this.sparklineRowDistributionColumn.Width = 110;
		this.sparklineTopValuesColumn.AppearanceCell.BackColor = System.Drawing.Color.Transparent;
		this.sparklineTopValuesColumn.AppearanceCell.Options.UseBackColor = true;
		this.sparklineTopValuesColumn.Caption = "Top values";
		this.sparklineTopValuesColumn.FieldName = "sparklineTopValues";
		this.sparklineTopValuesColumn.MaxWidth = 107;
		this.sparklineTopValuesColumn.MinWidth = 107;
		this.sparklineTopValuesColumn.Name = "sparklineTopValuesColumn";
		this.sparklineTopValuesColumn.OptionsColumn.AllowEdit = false;
		this.sparklineTopValuesColumn.OptionsColumn.ReadOnly = true;
		this.sparklineTopValuesColumn.Visible = true;
		this.sparklineTopValuesColumn.VisibleIndex = 10;
		this.sparklineTopValuesColumn.Width = 107;
		this.dataLinksGridColumn.Caption = "Linked terms";
		this.dataLinksGridColumn.ColumnEdit = this.dataLinksRepositoryItemAutoHeightMemoEdit;
		this.dataLinksGridColumn.FieldName = "DataLinksString";
		this.dataLinksGridColumn.Name = "dataLinksGridColumn";
		this.dataLinksGridColumn.OptionsColumn.AllowEdit = false;
		this.dataLinksGridColumn.OptionsColumn.ReadOnly = true;
		this.dataLinksGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.dataLinksGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.dataLinksGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.dataLinksGridColumn.Tag = "FIT_WIDTH";
		this.dataLinksGridColumn.ToolTip = "Linked Business Glossary terms";
		this.dataLinksGridColumn.Visible = true;
		this.dataLinksGridColumn.VisibleIndex = 11;
		this.dataLinksGridColumn.Width = 150;
		this.dataLinksRepositoryItemAutoHeightMemoEdit.Name = "dataLinksRepositoryItemAutoHeightMemoEdit";
		this.dataLinksRepositoryItemAutoHeightMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.nullableTableColumnsGridColumn.Caption = "Nullable";
		this.nullableTableColumnsGridColumn.ColumnEdit = this.checkRepositoryItemCheckEdit;
		this.nullableTableColumnsGridColumn.FieldName = "Nullable";
		this.nullableTableColumnsGridColumn.Name = "nullableTableColumnsGridColumn";
		this.nullableTableColumnsGridColumn.OptionsColumn.AllowEdit = false;
		this.nullableTableColumnsGridColumn.OptionsColumn.ReadOnly = true;
		this.nullableTableColumnsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.nullableTableColumnsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.nullableTableColumnsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.nullableTableColumnsGridColumn.Tag = "FIT_WIDTH";
		this.nullableTableColumnsGridColumn.Visible = true;
		this.nullableTableColumnsGridColumn.VisibleIndex = 7;
		this.nullableTableColumnsGridColumn.Width = 51;
		this.checkRepositoryItemCheckEdit.AutoHeight = false;
		this.checkRepositoryItemCheckEdit.Caption = "Check";
		this.checkRepositoryItemCheckEdit.HotTrackWhenReadOnly = false;
		this.checkRepositoryItemCheckEdit.Name = "checkRepositoryItemCheckEdit";
		this.checkRepositoryItemCheckEdit.ReadOnly = true;
		this.identityTableColumnsGridColumn.Caption = "Identity";
		this.identityTableColumnsGridColumn.ColumnEdit = this.checkRepositoryItemCheckEdit;
		this.identityTableColumnsGridColumn.FieldName = "IsIdentity";
		this.identityTableColumnsGridColumn.Name = "identityTableColumnsGridColumn";
		this.identityTableColumnsGridColumn.OptionsColumn.AllowEdit = false;
		this.identityTableColumnsGridColumn.OptionsColumn.ReadOnly = true;
		this.identityTableColumnsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.identityTableColumnsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.identityTableColumnsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.identityTableColumnsGridColumn.Tag = "FIT_WIDTH";
		this.identityTableColumnsGridColumn.ToolTip = "Identity / Auto increment";
		this.identityTableColumnsGridColumn.Visible = true;
		this.identityTableColumnsGridColumn.VisibleIndex = 12;
		this.identityTableColumnsGridColumn.Width = 51;
		this.defaultComputedTableColumnsGridColumn.Caption = "Default / Computed";
		this.defaultComputedTableColumnsGridColumn.ColumnEdit = this.comutedRepositoryItemAutoHeightMemoEdit;
		this.defaultComputedTableColumnsGridColumn.FieldName = "defaultComputedTableColumnsGridColumn";
		this.defaultComputedTableColumnsGridColumn.Name = "defaultComputedTableColumnsGridColumn";
		this.defaultComputedTableColumnsGridColumn.OptionsColumn.AllowEdit = false;
		this.defaultComputedTableColumnsGridColumn.OptionsColumn.ReadOnly = true;
		this.defaultComputedTableColumnsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.defaultComputedTableColumnsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.defaultComputedTableColumnsGridColumn.Tag = "FIT_WIDTH";
		this.defaultComputedTableColumnsGridColumn.UnboundExpression = resources.GetString("defaultComputedTableColumnsGridColumn.UnboundExpression");
		this.defaultComputedTableColumnsGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.defaultComputedTableColumnsGridColumn.Visible = true;
		this.defaultComputedTableColumnsGridColumn.VisibleIndex = 13;
		this.defaultComputedTableColumnsGridColumn.Width = 110;
		this.comutedRepositoryItemAutoHeightMemoEdit.Name = "comutedRepositoryItemAutoHeightMemoEdit";
		this.comutedRepositoryItemAutoHeightMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.createdGridColumn.Caption = "Created/first imported";
		this.createdGridColumn.FieldName = "CreationDateString";
		this.createdGridColumn.Name = "createdGridColumn";
		this.createdGridColumn.OptionsColumn.AllowEdit = false;
		this.createdGridColumn.OptionsFilter.AllowFilter = false;
		this.createdGridColumn.Width = 150;
		this.createdByGridColumn.Caption = "Created/first imported by";
		this.createdByGridColumn.FieldName = "CreatedBy";
		this.createdByGridColumn.Name = "createdByGridColumn";
		this.createdByGridColumn.OptionsColumn.AllowEdit = false;
		this.createdByGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.createdByGridColumn.Width = 150;
		this.lastUpdatedGridColumn.Caption = "Last updated";
		this.lastUpdatedGridColumn.FieldName = "LastModificationDateString";
		this.lastUpdatedGridColumn.Name = "lastUpdatedGridColumn";
		this.lastUpdatedGridColumn.OptionsColumn.AllowEdit = false;
		this.lastUpdatedGridColumn.OptionsFilter.AllowFilter = false;
		this.lastUpdatedGridColumn.Width = 100;
		this.lastUpdatedByGridColumn.Caption = "Last updated by";
		this.lastUpdatedByGridColumn.FieldName = "ModifiedBy";
		this.lastUpdatedByGridColumn.Name = "lastUpdatedByGridColumn";
		this.lastUpdatedByGridColumn.OptionsColumn.AllowEdit = false;
		this.lastUpdatedByGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.lastUpdatedByGridColumn.Width = 100;
		this.rowDistributionSparkLineToolTipController.AutoPopDelay = 10000;
		this.rowDistributionSparkLineToolTipController.InitialDelay = 1;
		this.rowDistributionSparkLineToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.rowDistributionSparkLineToolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(RowDistributionSparkLineToolTipController_GetActiveObjectInfo);
		this.columnsGridPanelUserControl.BackColor = System.Drawing.Color.Transparent;
		this.columnsGridPanelUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.columnsGridPanelUserControl.GridView = this.tableColumnsGridView;
		this.columnsGridPanelUserControl.Location = new System.Drawing.Point(0, 0);
		this.columnsGridPanelUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.columnsGridPanelUserControl.Name = "columnsGridPanelUserControl";
		this.columnsGridPanelUserControl.Size = new System.Drawing.Size(879, 30);
		this.columnsGridPanelUserControl.TabIndex = 7;
		this.tableRelationsXtraTabPage.Controls.Add(this.tableRelationsGrid);
		this.tableRelationsXtraTabPage.Controls.Add(this.relationsGridPanelUserControl);
		this.tableRelationsXtraTabPage.Name = "tableRelationsXtraTabPage";
		this.tableRelationsXtraTabPage.Size = new System.Drawing.Size(879, 555);
		this.tableRelationsXtraTabPage.Text = "Relationships";
		this.tableRelationsGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.tableRelationsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableRelationsGrid.Location = new System.Drawing.Point(0, 30);
		this.tableRelationsGrid.MainView = this.tableRelationsGridView;
		this.tableRelationsGrid.Name = "tableRelationsGrid";
		this.tableRelationsGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.tableRelationRepositoryItemPictureEdit, this.descriptionRelationRepositoryItemMemoEdit, this.nameRalationRepositoryItemCustomTextEdit, this.joinRepositoryItemAutoHeightMemoEdit, this.titleRelationRepositoryItemTextEdit, this.relationTitleRepositoryItemTextEdit, this.FKTableObjectNameCustomTextEdit, this.PKTableObjectNameCustomTextEdit });
		this.tableRelationsGrid.Size = new System.Drawing.Size(879, 525);
		this.tableRelationsGrid.TabIndex = 0;
		this.tableRelationsGrid.ToolTipController = this.tableToolTipController;
		this.tableRelationsGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tableRelationsGridView });
		this.tableRelationsGrid.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(tableRelationsGrid_ProcessGridKey);
		this.tableRelationsGrid.Enter += new System.EventHandler(RelationButtonsVisibleChanged);
		this.tableRelationsGrid.Leave += new System.EventHandler(RelationButtonsVisibleChanged);
		this.tableRelationsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.nameTableRelationsGridColumn, this.titleTableRelationsGridColumn, this.fkTableTableRelationsGridColumn, this.iconTableRelationsGridColumn, this.pkTableTableRelationsGridColumn, this.joinGridColumn, this.descriptionTableRelationsGridColumn });
		defaultBulkCopy2.IsCopying = false;
		this.tableRelationsGridView.Copy = defaultBulkCopy2;
		this.tableRelationsGridView.GridControl = this.tableRelationsGrid;
		this.tableRelationsGridView.Name = "tableRelationsGridView";
		this.tableRelationsGridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
		this.tableRelationsGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.tableRelationsGridView.OptionsDetail.EnableMasterViewMode = false;
		this.tableRelationsGridView.OptionsFilter.ShowAllTableValuesInFilterPopup = true;
		this.tableRelationsGridView.OptionsFilter.UseNewCustomFilterDialog = true;
		this.tableRelationsGridView.OptionsNavigation.AutoFocusNewRow = true;
		this.tableRelationsGridView.OptionsSelection.MultiSelect = true;
		this.tableRelationsGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.tableRelationsGridView.OptionsView.ColumnAutoWidth = false;
		this.tableRelationsGridView.OptionsView.RowAutoHeight = true;
		this.tableRelationsGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.tableRelationsGridView.OptionsView.ShowGroupPanel = false;
		this.tableRelationsGridView.OptionsView.ShowIndicator = false;
		this.tableRelationsGridView.RowHighlightingIsEnabled = true;
		this.tableRelationsGridView.SplashScreenManager = null;
		this.tableRelationsGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(GridView_CustomDrawCell);
		this.tableRelationsGridView.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(highlightGridView_RowCellStyle);
		this.tableRelationsGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(tableRelationsListGridView_PopupMenuShowing);
		this.tableRelationsGridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(tableRelationsGridView_SelectionChanged);
		this.tableRelationsGridView.ShowingEditor += new System.ComponentModel.CancelEventHandler(tableRelationsGridView_ShowingEditor);
		this.tableRelationsGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(tableRelationsGridView_CellValueChanged);
		this.tableRelationsGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(tableRelationsGridView_CellValueChanging);
		this.tableRelationsGridView.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(tableRelationsGridView_CustomColumnSort);
		this.tableRelationsGridView.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(tableRelationsListGridView_CustomUnboundColumnData);
		this.tableRelationsGridView.DoubleClick += new System.EventHandler(tableRelationsGridView_DoubleClick);
		this.tableRelationsGridView.DataSourceChanged += new System.EventHandler(tableRelationsGridView_DataSourceChanged);
		this.nameTableRelationsGridColumn.Caption = "Relationship name";
		this.nameTableRelationsGridColumn.ColumnEdit = this.nameRalationRepositoryItemCustomTextEdit;
		this.nameTableRelationsGridColumn.FieldName = "Name";
		this.nameTableRelationsGridColumn.Name = "nameTableRelationsGridColumn";
		this.nameTableRelationsGridColumn.OptionsColumn.AllowEdit = false;
		this.nameTableRelationsGridColumn.OptionsColumn.ReadOnly = true;
		this.nameTableRelationsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.nameTableRelationsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.nameTableRelationsGridColumn.Tag = "FIT_WIDTH";
		this.nameTableRelationsGridColumn.Visible = true;
		this.nameTableRelationsGridColumn.VisibleIndex = 4;
		this.nameTableRelationsGridColumn.Width = 200;
		this.nameRalationRepositoryItemCustomTextEdit.AutoHeight = false;
		this.nameRalationRepositoryItemCustomTextEdit.Name = "nameRalationRepositoryItemCustomTextEdit";
		this.titleTableRelationsGridColumn.Caption = "Title";
		this.titleTableRelationsGridColumn.ColumnEdit = this.relationTitleRepositoryItemTextEdit;
		this.titleTableRelationsGridColumn.FieldName = "Title";
		this.titleTableRelationsGridColumn.Name = "titleTableRelationsGridColumn";
		this.titleTableRelationsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.titleTableRelationsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.titleTableRelationsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.titleTableRelationsGridColumn.Tag = "FIT_WIDTH";
		this.titleTableRelationsGridColumn.Visible = true;
		this.titleTableRelationsGridColumn.VisibleIndex = 3;
		this.titleTableRelationsGridColumn.Width = 140;
		this.relationTitleRepositoryItemTextEdit.AutoHeight = false;
		this.relationTitleRepositoryItemTextEdit.Name = "relationTitleRepositoryItemTextEdit";
		this.fkTableTableRelationsGridColumn.Caption = "FK Table";
		this.fkTableTableRelationsGridColumn.ColumnEdit = this.FKTableObjectNameCustomTextEdit;
		this.fkTableTableRelationsGridColumn.FieldName = "FKTableObjectNameWithTitle";
		this.fkTableTableRelationsGridColumn.Name = "fkTableTableRelationsGridColumn";
		this.fkTableTableRelationsGridColumn.OptionsColumn.AllowEdit = false;
		this.fkTableTableRelationsGridColumn.OptionsColumn.ReadOnly = true;
		this.fkTableTableRelationsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.fkTableTableRelationsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.fkTableTableRelationsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.fkTableTableRelationsGridColumn.Tag = "FIT_WIDTH";
		this.fkTableTableRelationsGridColumn.Visible = true;
		this.fkTableTableRelationsGridColumn.VisibleIndex = 0;
		this.fkTableTableRelationsGridColumn.Width = 120;
		this.FKTableObjectNameCustomTextEdit.AutoHeight = false;
		this.FKTableObjectNameCustomTextEdit.Name = "FKTableObjectNameCustomTextEdit";
		this.iconTableRelationsGridColumn.Caption = " ";
		this.iconTableRelationsGridColumn.ColumnEdit = this.tableRelationRepositoryItemPictureEdit;
		this.iconTableRelationsGridColumn.FieldName = "iconTableRelationsGridColumn";
		this.iconTableRelationsGridColumn.MaxWidth = 29;
		this.iconTableRelationsGridColumn.MinWidth = 29;
		this.iconTableRelationsGridColumn.Name = "iconTableRelationsGridColumn";
		this.iconTableRelationsGridColumn.OptionsColumn.AllowEdit = false;
		this.iconTableRelationsGridColumn.OptionsColumn.ReadOnly = true;
		this.iconTableRelationsGridColumn.OptionsFilter.AllowAutoFilter = false;
		this.iconTableRelationsGridColumn.OptionsFilter.AllowFilter = false;
		this.iconTableRelationsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.iconTableRelationsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.iconTableRelationsGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.iconTableRelationsGridColumn.Visible = true;
		this.iconTableRelationsGridColumn.VisibleIndex = 1;
		this.iconTableRelationsGridColumn.Width = 29;
		this.tableRelationRepositoryItemPictureEdit.AllowFocused = false;
		this.tableRelationRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.tableRelationRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.tableRelationRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.tableRelationRepositoryItemPictureEdit.Name = "tableRelationRepositoryItemPictureEdit";
		this.tableRelationRepositoryItemPictureEdit.ShowMenu = false;
		this.pkTableTableRelationsGridColumn.Caption = "PK Table";
		this.pkTableTableRelationsGridColumn.ColumnEdit = this.PKTableObjectNameCustomTextEdit;
		this.pkTableTableRelationsGridColumn.FieldName = "PKTableObjectNameWithTitle";
		this.pkTableTableRelationsGridColumn.Name = "pkTableTableRelationsGridColumn";
		this.pkTableTableRelationsGridColumn.OptionsColumn.AllowEdit = false;
		this.pkTableTableRelationsGridColumn.OptionsColumn.ReadOnly = true;
		this.pkTableTableRelationsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.pkTableTableRelationsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.pkTableTableRelationsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.pkTableTableRelationsGridColumn.Tag = "FIT_WIDTH";
		this.pkTableTableRelationsGridColumn.Visible = true;
		this.pkTableTableRelationsGridColumn.VisibleIndex = 2;
		this.pkTableTableRelationsGridColumn.Width = 120;
		this.PKTableObjectNameCustomTextEdit.AutoHeight = false;
		this.PKTableObjectNameCustomTextEdit.Name = "PKTableObjectNameCustomTextEdit";
		this.joinGridColumn.Caption = "Join";
		this.joinGridColumn.ColumnEdit = this.joinRepositoryItemAutoHeightMemoEdit;
		this.joinGridColumn.FieldName = "JoinColumnsFormattedWithTitles";
		this.joinGridColumn.Name = "joinGridColumn";
		this.joinGridColumn.OptionsColumn.AllowEdit = false;
		this.joinGridColumn.OptionsColumn.ReadOnly = true;
		this.joinGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.joinGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.joinGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.joinGridColumn.Tag = "FIT_WIDTH";
		this.joinGridColumn.Visible = true;
		this.joinGridColumn.VisibleIndex = 5;
		this.joinGridColumn.Width = 200;
		this.joinRepositoryItemAutoHeightMemoEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.joinRepositoryItemAutoHeightMemoEdit.Name = "joinRepositoryItemAutoHeightMemoEdit";
		this.joinRepositoryItemAutoHeightMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.descriptionTableRelationsGridColumn.AppearanceCell.Options.UseTextOptions = true;
		this.descriptionTableRelationsGridColumn.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.descriptionTableRelationsGridColumn.Caption = "Description";
		this.descriptionTableRelationsGridColumn.ColumnEdit = this.descriptionRelationRepositoryItemMemoEdit;
		this.descriptionTableRelationsGridColumn.FieldName = "Description";
		this.descriptionTableRelationsGridColumn.MaxWidth = 1000;
		this.descriptionTableRelationsGridColumn.MinWidth = 200;
		this.descriptionTableRelationsGridColumn.Name = "descriptionTableRelationsGridColumn";
		this.descriptionTableRelationsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.descriptionTableRelationsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.descriptionTableRelationsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.descriptionTableRelationsGridColumn.Tag = "FIT_WIDTH";
		this.descriptionTableRelationsGridColumn.Visible = true;
		this.descriptionTableRelationsGridColumn.VisibleIndex = 6;
		this.descriptionTableRelationsGridColumn.Width = 400;
		this.descriptionRelationRepositoryItemMemoEdit.Name = "descriptionRelationRepositoryItemMemoEdit";
		this.descriptionRelationRepositoryItemMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.titleRelationRepositoryItemTextEdit.AutoHeight = false;
		this.titleRelationRepositoryItemTextEdit.Name = "titleRelationRepositoryItemTextEdit";
		this.tableToolTipController.AllowHtmlText = true;
		this.tableToolTipController.AutoPopDelay = 10000;
		this.tableToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.tableToolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(tableToolTipController_GetActiveObjectInfo);
		this.relationsGridPanelUserControl.BackColor = System.Drawing.Color.Transparent;
		this.relationsGridPanelUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.relationsGridPanelUserControl.GridView = this.tableRelationsGridView;
		this.relationsGridPanelUserControl.Location = new System.Drawing.Point(0, 0);
		this.relationsGridPanelUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.relationsGridPanelUserControl.Name = "relationsGridPanelUserControl";
		this.relationsGridPanelUserControl.Size = new System.Drawing.Size(879, 30);
		this.relationsGridPanelUserControl.TabIndex = 2;
		this.tableConstraintsXtraTabPage.Controls.Add(this.tableConstraintsGrid);
		this.tableConstraintsXtraTabPage.Controls.Add(this.constraintsGridPanelUserControl);
		this.tableConstraintsXtraTabPage.Name = "tableConstraintsXtraTabPage";
		this.tableConstraintsXtraTabPage.Size = new System.Drawing.Size(879, 555);
		this.tableConstraintsXtraTabPage.Text = "Unique keys";
		this.tableConstraintsGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.tableConstraintsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableConstraintsGrid.Location = new System.Drawing.Point(0, 30);
		this.tableConstraintsGrid.MainView = this.tableConstraintsGridView;
		this.tableConstraintsGrid.Name = "tableConstraintsGrid";
		this.tableConstraintsGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.descriptionConstraintRepositoryItemMemoEdit, this.nameRepositoryItemCustomTextEdit, this.typeRepositoryItemPictureEdit, this.columnFullNameFormattedRepositoryItemCustomTextEdit });
		this.tableConstraintsGrid.Size = new System.Drawing.Size(879, 525);
		this.tableConstraintsGrid.TabIndex = 0;
		this.tableConstraintsGrid.ToolTipController = this.tableToolTipController;
		this.tableConstraintsGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tableConstraintsGridView });
		this.tableConstraintsGrid.ProcessGridKey += new System.Windows.Forms.KeyEventHandler(tableConstraintsGrid_ProcessGridKey);
		this.tableConstraintsGrid.Enter += new System.EventHandler(ConstraintButtonsVisibleChanged);
		this.tableConstraintsGrid.Leave += new System.EventHandler(ConstraintButtonsVisibleChanged);
		this.tableConstraintsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.iconTableConstraintsGridColumn, this.nameTableConstraintsGridColumn, this.columnsTableConstraintsGridColumn, this.descriptionTableConstraintsGridColumn });
		this.tableConstraintsGridView.Copy = defaultBulkCopy2;
		this.tableConstraintsGridView.GridControl = this.tableConstraintsGrid;
		this.tableConstraintsGridView.Name = "tableConstraintsGridView";
		this.tableConstraintsGridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
		this.tableConstraintsGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.tableConstraintsGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.tableConstraintsGridView.OptionsDetail.EnableMasterViewMode = false;
		this.tableConstraintsGridView.OptionsFilter.ShowAllTableValuesInFilterPopup = true;
		this.tableConstraintsGridView.OptionsNavigation.AutoFocusNewRow = true;
		this.tableConstraintsGridView.OptionsSelection.MultiSelect = true;
		this.tableConstraintsGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.tableConstraintsGridView.OptionsView.ColumnAutoWidth = false;
		this.tableConstraintsGridView.OptionsView.RowAutoHeight = true;
		this.tableConstraintsGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.tableConstraintsGridView.OptionsView.ShowGroupPanel = false;
		this.tableConstraintsGridView.OptionsView.ShowIndicator = false;
		this.tableConstraintsGridView.RowHighlightingIsEnabled = true;
		this.tableConstraintsGridView.SplashScreenManager = null;
		this.tableConstraintsGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(GridView_CustomDrawCell);
		this.tableConstraintsGridView.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(highlightGridView_RowCellStyle);
		this.tableConstraintsGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(tableConstraintsGridView_PopupMenuShowing);
		this.tableConstraintsGridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(tableConstraintsGridView_SelectionChanged);
		this.tableConstraintsGridView.ShowingEditor += new System.ComponentModel.CancelEventHandler(tableConstraintsGridView_ShowingEditor);
		this.tableConstraintsGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(tableConstraintsGridView_CellValueChanged);
		this.tableConstraintsGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(tableConstraintsGridView_CellValueChanging);
		this.tableConstraintsGridView.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(tableConstraintsGridView_CustomColumnSort);
		this.tableConstraintsGridView.DoubleClick += new System.EventHandler(tableConstraintsGridView_DoubleClick);
		this.tableConstraintsGridView.DataSourceChanged += new System.EventHandler(tableConstraintsGridView_DataSourceChanged);
		this.iconTableConstraintsGridColumn.Caption = " ";
		this.iconTableConstraintsGridColumn.ColumnEdit = this.typeRepositoryItemPictureEdit;
		this.iconTableConstraintsGridColumn.FieldName = "ImageFile";
		this.iconTableConstraintsGridColumn.MaxWidth = 21;
		this.iconTableConstraintsGridColumn.MinWidth = 21;
		this.iconTableConstraintsGridColumn.Name = "iconTableConstraintsGridColumn";
		this.iconTableConstraintsGridColumn.OptionsColumn.AllowEdit = false;
		this.iconTableConstraintsGridColumn.OptionsColumn.ReadOnly = true;
		this.iconTableConstraintsGridColumn.OptionsFilter.AllowAutoFilter = false;
		this.iconTableConstraintsGridColumn.OptionsFilter.AllowFilter = false;
		this.iconTableConstraintsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.iconTableConstraintsGridColumn.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.iconTableConstraintsGridColumn.Visible = true;
		this.iconTableConstraintsGridColumn.VisibleIndex = 0;
		this.iconTableConstraintsGridColumn.Width = 21;
		this.typeRepositoryItemPictureEdit.AllowFocused = false;
		this.typeRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.typeRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.typeRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.typeRepositoryItemPictureEdit.Name = "typeRepositoryItemPictureEdit";
		this.typeRepositoryItemPictureEdit.ShowMenu = false;
		this.nameTableConstraintsGridColumn.Caption = "Key name";
		this.nameTableConstraintsGridColumn.ColumnEdit = this.nameRepositoryItemCustomTextEdit;
		this.nameTableConstraintsGridColumn.FieldName = "Name";
		this.nameTableConstraintsGridColumn.Name = "nameTableConstraintsGridColumn";
		this.nameTableConstraintsGridColumn.OptionsColumn.AllowEdit = false;
		this.nameTableConstraintsGridColumn.OptionsColumn.ReadOnly = true;
		this.nameTableConstraintsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.nameTableConstraintsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.nameTableConstraintsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.nameTableConstraintsGridColumn.Tag = "FIT_WIDTH";
		this.nameTableConstraintsGridColumn.Visible = true;
		this.nameTableConstraintsGridColumn.VisibleIndex = 1;
		this.nameTableConstraintsGridColumn.Width = 130;
		this.nameRepositoryItemCustomTextEdit.AutoHeight = false;
		this.nameRepositoryItemCustomTextEdit.Name = "nameRepositoryItemCustomTextEdit";
		this.columnsTableConstraintsGridColumn.Caption = "Columns";
		this.columnsTableConstraintsGridColumn.ColumnEdit = this.columnFullNameFormattedRepositoryItemCustomTextEdit;
		this.columnsTableConstraintsGridColumn.FieldName = "ColumnsStringFormattedWithTitles";
		this.columnsTableConstraintsGridColumn.Name = "columnsTableConstraintsGridColumn";
		this.columnsTableConstraintsGridColumn.OptionsColumn.AllowEdit = false;
		this.columnsTableConstraintsGridColumn.OptionsColumn.ReadOnly = true;
		this.columnsTableConstraintsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.columnsTableConstraintsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.columnsTableConstraintsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.columnsTableConstraintsGridColumn.Tag = "FIT_WIDTH";
		this.columnsTableConstraintsGridColumn.Visible = true;
		this.columnsTableConstraintsGridColumn.VisibleIndex = 2;
		this.columnsTableConstraintsGridColumn.Width = 150;
		this.columnFullNameFormattedRepositoryItemCustomTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.columnFullNameFormattedRepositoryItemCustomTextEdit.AutoHeight = false;
		this.columnFullNameFormattedRepositoryItemCustomTextEdit.Name = "columnFullNameFormattedRepositoryItemCustomTextEdit";
		this.descriptionTableConstraintsGridColumn.AppearanceCell.Options.UseTextOptions = true;
		this.descriptionTableConstraintsGridColumn.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.descriptionTableConstraintsGridColumn.Caption = "Description";
		this.descriptionTableConstraintsGridColumn.ColumnEdit = this.descriptionConstraintRepositoryItemMemoEdit;
		this.descriptionTableConstraintsGridColumn.FieldName = "Description";
		this.descriptionTableConstraintsGridColumn.MaxWidth = 1000;
		this.descriptionTableConstraintsGridColumn.MinWidth = 200;
		this.descriptionTableConstraintsGridColumn.Name = "descriptionTableConstraintsGridColumn";
		this.descriptionTableConstraintsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.descriptionTableConstraintsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.descriptionTableConstraintsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.descriptionTableConstraintsGridColumn.Tag = "FIT_WIDTH";
		this.descriptionTableConstraintsGridColumn.Visible = true;
		this.descriptionTableConstraintsGridColumn.VisibleIndex = 3;
		this.descriptionTableConstraintsGridColumn.Width = 400;
		this.descriptionConstraintRepositoryItemMemoEdit.Name = "descriptionConstraintRepositoryItemMemoEdit";
		this.descriptionConstraintRepositoryItemMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.constraintsGridPanelUserControl.BackColor = System.Drawing.Color.Transparent;
		this.constraintsGridPanelUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.constraintsGridPanelUserControl.GridView = this.tableConstraintsGridView;
		this.constraintsGridPanelUserControl.Location = new System.Drawing.Point(0, 0);
		this.constraintsGridPanelUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.constraintsGridPanelUserControl.Name = "constraintsGridPanelUserControl";
		this.constraintsGridPanelUserControl.Size = new System.Drawing.Size(879, 30);
		this.constraintsGridPanelUserControl.TabIndex = 3;
		this.tableTriggersXtraTabPage.Controls.Add(this.layoutControl1);
		this.tableTriggersXtraTabPage.Controls.Add(this.triggersGridPanelUserControl);
		this.tableTriggersXtraTabPage.Name = "tableTriggersXtraTabPage";
		this.tableTriggersXtraTabPage.Size = new System.Drawing.Size(879, 555);
		this.tableTriggersXtraTabPage.Text = "Triggers";
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl1.Controls.Add(this.triggersSplitContainerControl);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 30);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.Root = this.layoutControlGroup3;
		this.layoutControl1.Size = new System.Drawing.Size(879, 525);
		this.layoutControl1.TabIndex = 8;
		this.layoutControl1.Text = "layoutControl1";
		this.triggersSplitContainerControl.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
		this.triggersSplitContainerControl.Location = new System.Drawing.Point(0, 0);
		this.triggersSplitContainerControl.Name = "triggersSplitContainerControl";
		this.triggersSplitContainerControl.Panel1.Controls.Add(this.tableTriggerGrid);
		this.triggersSplitContainerControl.Panel1.Text = "Panel1";
		this.triggersSplitContainerControl.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.triggersSplitContainerControl.Panel2.Controls.Add(this.layoutControl2);
		this.triggersSplitContainerControl.Panel2.Text = "Panel2";
		this.triggersSplitContainerControl.Size = new System.Drawing.Size(879, 525);
		this.triggersSplitContainerControl.SplitterPosition = 361;
		this.triggersSplitContainerControl.TabIndex = 5;
		this.triggersSplitContainerControl.Text = "splitContainerControl1";
		this.tableTriggerGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.tableTriggerGrid.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tableTriggerGrid.Location = new System.Drawing.Point(0, 0);
		this.tableTriggerGrid.MainView = this.tableTriggerGridView;
		this.tableTriggerGrid.Margin = new System.Windows.Forms.Padding(0);
		this.tableTriggerGrid.Name = "tableTriggerGrid";
		this.tableTriggerGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.iconTriggerRepositoryItemPictureEdit, this.onInsertRepositoryItemCheckEdit, this.onUpdateRepositoryItemCheckEdit, this.onDeleteRepositoryItemCheckEdit, this.descriptionTriggerRepositoryItemMemoEdit });
		this.tableTriggerGrid.Size = new System.Drawing.Size(508, 525);
		this.tableTriggerGrid.TabIndex = 1;
		this.tableTriggerGrid.ToolTipController = this.tableToolTipController;
		this.tableTriggerGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.tableTriggerGridView });
		this.tableTriggerGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.iconTableTriggersGridColumn, this.nameTableTriggersGridColumn, this.whenTableTriggersGridColumn, this.descriptionTableTriggersGridColumn });
		defaultBulkCopy3.IsCopying = false;
		this.tableTriggerGridView.Copy = defaultBulkCopy3;
		this.tableTriggerGridView.GridControl = this.tableTriggerGrid;
		this.tableTriggerGridView.Name = "tableTriggerGridView";
		this.tableTriggerGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.tableTriggerGridView.OptionsFilter.ShowAllTableValuesInFilterPopup = true;
		this.tableTriggerGridView.OptionsSelection.MultiSelect = true;
		this.tableTriggerGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.tableTriggerGridView.OptionsView.ColumnAutoWidth = false;
		this.tableTriggerGridView.OptionsView.RowAutoHeight = true;
		this.tableTriggerGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.tableTriggerGridView.OptionsView.ShowGroupPanel = false;
		this.tableTriggerGridView.OptionsView.ShowIndicator = false;
		this.tableTriggerGridView.RowHighlightingIsEnabled = true;
		this.tableTriggerGridView.SplashScreenManager = null;
		this.tableTriggerGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(GridView_CustomDrawCell);
		this.tableTriggerGridView.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(highlightGridView_RowCellStyle);
		this.tableTriggerGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(tableTriggerGridView_PopupMenuShowing);
		this.tableTriggerGridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(tableTriggerGridView_FocusedRowChanged);
		this.tableTriggerGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(tableTriggerGridView_CellValueChanged);
		this.tableTriggerGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(tableTriggerGridView_CellValueChanging);
		this.tableTriggerGridView.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(tableTriggerGridView_CustomUnboundColumnData);
		this.tableTriggerGridView.DataSourceChanged += new System.EventHandler(tableTriggerGridView_DataSourceChanged);
		this.iconTableTriggersGridColumn.Caption = " ";
		this.iconTableTriggersGridColumn.ColumnEdit = this.iconTriggerRepositoryItemPictureEdit;
		this.iconTableTriggersGridColumn.FieldName = "iconTableColumnsGridColumn";
		this.iconTableTriggersGridColumn.MaxWidth = 21;
		this.iconTableTriggersGridColumn.MinWidth = 21;
		this.iconTableTriggersGridColumn.Name = "iconTableTriggersGridColumn";
		this.iconTableTriggersGridColumn.OptionsColumn.AllowEdit = false;
		this.iconTableTriggersGridColumn.OptionsFilter.AllowAutoFilter = false;
		this.iconTableTriggersGridColumn.OptionsFilter.AllowFilter = false;
		this.iconTableTriggersGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.iconTableTriggersGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.iconTableTriggersGridColumn.Visible = true;
		this.iconTableTriggersGridColumn.VisibleIndex = 0;
		this.iconTableTriggersGridColumn.Width = 21;
		this.iconTriggerRepositoryItemPictureEdit.AllowFocused = false;
		this.iconTriggerRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.iconTriggerRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconTriggerRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconTriggerRepositoryItemPictureEdit.Name = "iconTriggerRepositoryItemPictureEdit";
		this.iconTriggerRepositoryItemPictureEdit.ShowMenu = false;
		this.nameTableTriggersGridColumn.Caption = "Name";
		this.nameTableTriggersGridColumn.FieldName = "Name";
		this.nameTableTriggersGridColumn.Name = "nameTableTriggersGridColumn";
		this.nameTableTriggersGridColumn.OptionsColumn.AllowEdit = false;
		this.nameTableTriggersGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.nameTableTriggersGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.nameTableTriggersGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.nameTableTriggersGridColumn.Tag = "FIT_WIDTH";
		this.nameTableTriggersGridColumn.Visible = true;
		this.nameTableTriggersGridColumn.VisibleIndex = 1;
		this.nameTableTriggersGridColumn.Width = 140;
		this.whenTableTriggersGridColumn.Caption = "When";
		this.whenTableTriggersGridColumn.FieldName = "WhenRun";
		this.whenTableTriggersGridColumn.Name = "whenTableTriggersGridColumn";
		this.whenTableTriggersGridColumn.OptionsColumn.AllowEdit = false;
		this.whenTableTriggersGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.whenTableTriggersGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.whenTableTriggersGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.whenTableTriggersGridColumn.Tag = "FIT_WIDTH";
		this.whenTableTriggersGridColumn.Visible = true;
		this.whenTableTriggersGridColumn.VisibleIndex = 2;
		this.whenTableTriggersGridColumn.Width = 100;
		this.descriptionTableTriggersGridColumn.AppearanceCell.Options.UseTextOptions = true;
		this.descriptionTableTriggersGridColumn.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.descriptionTableTriggersGridColumn.Caption = "Description";
		this.descriptionTableTriggersGridColumn.ColumnEdit = this.descriptionTriggerRepositoryItemMemoEdit;
		this.descriptionTableTriggersGridColumn.FieldName = "Description";
		this.descriptionTableTriggersGridColumn.MaxWidth = 1000;
		this.descriptionTableTriggersGridColumn.MinWidth = 200;
		this.descriptionTableTriggersGridColumn.Name = "descriptionTableTriggersGridColumn";
		this.descriptionTableTriggersGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.descriptionTableTriggersGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.descriptionTableTriggersGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.descriptionTableTriggersGridColumn.Tag = "FIT_WIDTH";
		this.descriptionTableTriggersGridColumn.Visible = true;
		this.descriptionTableTriggersGridColumn.VisibleIndex = 3;
		this.descriptionTableTriggersGridColumn.Width = 400;
		this.descriptionTriggerRepositoryItemMemoEdit.Name = "descriptionTriggerRepositoryItemMemoEdit";
		this.descriptionTriggerRepositoryItemMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.onInsertRepositoryItemCheckEdit.AutoHeight = false;
		this.onInsertRepositoryItemCheckEdit.Caption = "Check";
		this.onInsertRepositoryItemCheckEdit.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
		this.onInsertRepositoryItemCheckEdit.ImageOptions.ImageChecked = Dataedo.App.Properties.Resources.tick_16;
		this.onInsertRepositoryItemCheckEdit.Name = "onInsertRepositoryItemCheckEdit";
		this.onUpdateRepositoryItemCheckEdit.AutoHeight = false;
		this.onUpdateRepositoryItemCheckEdit.Caption = "Check";
		this.onUpdateRepositoryItemCheckEdit.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
		this.onUpdateRepositoryItemCheckEdit.ImageOptions.ImageChecked = Dataedo.App.Properties.Resources.tick_16;
		this.onUpdateRepositoryItemCheckEdit.Name = "onUpdateRepositoryItemCheckEdit";
		this.onDeleteRepositoryItemCheckEdit.AutoHeight = false;
		this.onDeleteRepositoryItemCheckEdit.Caption = "Check";
		this.onDeleteRepositoryItemCheckEdit.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.UserDefined;
		this.onDeleteRepositoryItemCheckEdit.ImageOptions.ImageChecked = Dataedo.App.Properties.Resources.tick_16;
		this.onDeleteRepositoryItemCheckEdit.Name = "onDeleteRepositoryItemCheckEdit";
		this.layoutControl2.AllowCustomization = false;
		this.layoutControl2.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl2.Controls.Add(this.scriptLabelControl);
		this.layoutControl2.Controls.Add(this.triggerScriptRichEditControl);
		this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl2.Location = new System.Drawing.Point(0, 0);
		this.layoutControl2.Name = "layoutControl2";
		this.layoutControl2.OptionsView.ShareLookAndFeelWithChildren = false;
		this.layoutControl2.Root = this.layoutControlGroup4;
		this.layoutControl2.Size = new System.Drawing.Size(357, 521);
		this.layoutControl2.TabIndex = 4;
		this.layoutControl2.Text = "layoutControl2";
		this.scriptLabelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.scriptLabelControl.Location = new System.Drawing.Point(0, 0);
		this.scriptLabelControl.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
		this.scriptLabelControl.Name = "scriptLabelControl";
		this.scriptLabelControl.Size = new System.Drawing.Size(357, 20);
		this.scriptLabelControl.TabIndex = 4;
		this.scriptLabelControl.Text = "Script";
		this.triggerScriptRichEditControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
		this.triggerScriptRichEditControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.triggerScriptRichEditControl.IsHighlighted = false;
		this.triggerScriptRichEditControl.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
		this.triggerScriptRichEditControl.Location = new System.Drawing.Point(0, 20);
		this.triggerScriptRichEditControl.Margin = new System.Windows.Forms.Padding(0);
		this.triggerScriptRichEditControl.Name = "triggerScriptRichEditControl";
		this.triggerScriptRichEditControl.OccurrencesCount = 0;
		this.triggerScriptRichEditControl.Options.HorizontalScrollbar.Visibility = DevExpress.XtraRichEdit.RichEditScrollbarVisibility.Hidden;
		this.triggerScriptRichEditControl.OriginalHtmlText = null;
		this.triggerScriptRichEditControl.ReadOnly = true;
		this.triggerScriptRichEditControl.Size = new System.Drawing.Size(357, 501);
		this.triggerScriptRichEditControl.TabIndex = 3;
		this.triggerScriptRichEditControl.Views.SimpleView.Padding = new System.Windows.Forms.Padding(4, 4, 4, 0);
		this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup4.GroupBordersVisible = false;
		this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.layoutControlItem7, this.layoutControlItem5 });
		this.layoutControlGroup4.Name = "layoutControlGroup4";
		this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup4.Size = new System.Drawing.Size(357, 521);
		this.layoutControlGroup4.TextVisible = false;
		this.layoutControlItem7.Control = this.triggerScriptRichEditControl;
		this.layoutControlItem7.Location = new System.Drawing.Point(0, 20);
		this.layoutControlItem7.MinSize = new System.Drawing.Size(100, 20);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem7.Size = new System.Drawing.Size(357, 501);
		this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.TextVisible = false;
		this.layoutControlItem5.Control = this.scriptLabelControl;
		this.layoutControlItem5.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem5.MaxSize = new System.Drawing.Size(0, 20);
		this.layoutControlItem5.MinSize = new System.Drawing.Size(33, 20);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem5.Size = new System.Drawing.Size(357, 20);
		this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup3.GroupBordersVisible = false;
		this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem4 });
		this.layoutControlGroup3.Name = "layoutControlGroup3";
		this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup3.Size = new System.Drawing.Size(879, 525);
		this.layoutControlGroup3.TextVisible = false;
		this.layoutControlItem4.Control = this.triggersSplitContainerControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem4.Size = new System.Drawing.Size(879, 525);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.triggersGridPanelUserControl.BackColor = System.Drawing.Color.Transparent;
		this.triggersGridPanelUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.triggersGridPanelUserControl.GridView = this.tableTriggerGridView;
		this.triggersGridPanelUserControl.Location = new System.Drawing.Point(0, 0);
		this.triggersGridPanelUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.triggersGridPanelUserControl.Name = "triggersGridPanelUserControl";
		this.triggersGridPanelUserControl.Size = new System.Drawing.Size(879, 30);
		this.triggersGridPanelUserControl.TabIndex = 5;
		this.dataLineageXtraTabPage.Controls.Add(this.dataLineageUserControl);
		this.dataLineageXtraTabPage.Name = "dataLineageXtraTabPage";
		this.dataLineageXtraTabPage.Size = new System.Drawing.Size(879, 555);
		this.dataLineageXtraTabPage.Text = "Data Lineage";
		this.dataLineageUserControl.BackColor = System.Drawing.Color.Transparent;
		this.dataLineageUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dataLineageUserControl.IsInitialized = false;
		this.dataLineageUserControl.Location = new System.Drawing.Point(0, 0);
		this.dataLineageUserControl.Margin = new System.Windows.Forms.Padding(2);
		this.dataLineageUserControl.Name = "dataLineageUserControl";
		this.dataLineageUserControl.Size = new System.Drawing.Size(879, 555);
		this.dataLineageUserControl.TabIndex = 0;
		this.dependenciesXtraTabPage.Controls.Add(this.dependenciesUserControl);
		this.dependenciesXtraTabPage.Name = "dependenciesXtraTabPage";
		this.dependenciesXtraTabPage.Size = new System.Drawing.Size(879, 555);
		this.dependenciesXtraTabPage.Text = "Dependencies";
		this.dependenciesUserControl.BackColor = System.Drawing.Color.Transparent;
		this.dependenciesUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dependenciesUserControl.Location = new System.Drawing.Point(0, 0);
		this.dependenciesUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.dependenciesUserControl.Name = "dependenciesUserControl";
		this.dependenciesUserControl.RelationsGridControl = null;
		this.dependenciesUserControl.SetRelationsPageAsModified = null;
		this.dependenciesUserControl.Size = new System.Drawing.Size(879, 555);
		this.dependenciesUserControl.TabIndex = 4;
		this.dependenciesUserControl.DependencyChanging += new Dataedo.App.UserControls.Dependencies.DependenciesUserControl.DependencyChangingHandler(dependenciesUserControl_DependencyChanging);
		this.dependenciesUserControl.BeforeChangingRelations += new Dataedo.App.UserControls.Dependencies.DependenciesUserControl.DependencyChangingHandler(dependenciesUserControl_BeforeChangingRelations);
		this.tableScriptXtraTabPage.Controls.Add(this.nonCustomizableLayoutControl1);
		this.tableScriptXtraTabPage.Name = "tableScriptXtraTabPage";
		this.tableScriptXtraTabPage.Size = new System.Drawing.Size(879, 555);
		this.tableScriptXtraTabPage.Text = "Script";
		this.nonCustomizableLayoutControl1.AllowCustomization = false;
		this.nonCustomizableLayoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl1.Controls.Add(this.scriptTextSearchCountLabelControl);
		this.nonCustomizableLayoutControl1.Controls.Add(this.labelControl2);
		this.nonCustomizableLayoutControl1.Controls.Add(this.scriptRichEditControl);
		this.nonCustomizableLayoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl1.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl1.Name = "nonCustomizableLayoutControl1";
		this.nonCustomizableLayoutControl1.Root = this.layoutControlGroup5;
		this.nonCustomizableLayoutControl1.Size = new System.Drawing.Size(879, 555);
		this.nonCustomizableLayoutControl1.TabIndex = 3;
		this.nonCustomizableLayoutControl1.Text = "nonCustomizableLayoutControl1";
		this.scriptTextSearchCountLabelControl.Location = new System.Drawing.Point(33, 2);
		this.scriptTextSearchCountLabelControl.Name = "scriptTextSearchCountLabelControl";
		this.scriptTextSearchCountLabelControl.Size = new System.Drawing.Size(844, 13);
		this.scriptTextSearchCountLabelControl.StyleController = this.nonCustomizableLayoutControl1;
		this.scriptTextSearchCountLabelControl.TabIndex = 5;
		this.labelControl2.Location = new System.Drawing.Point(2, 2);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(27, 13);
		this.labelControl2.StyleController = this.nonCustomizableLayoutControl1;
		this.labelControl2.TabIndex = 4;
		this.labelControl2.Text = "Script";
		this.scriptRichEditControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
		this.scriptRichEditControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.scriptRichEditControl.IsHighlighted = false;
		this.scriptRichEditControl.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
		this.scriptRichEditControl.Location = new System.Drawing.Point(0, 17);
		this.scriptRichEditControl.Margin = new System.Windows.Forms.Padding(0);
		this.scriptRichEditControl.Name = "scriptRichEditControl";
		this.scriptRichEditControl.OccurrencesCount = 0;
		this.scriptRichEditControl.Options.HorizontalScrollbar.Visibility = DevExpress.XtraRichEdit.RichEditScrollbarVisibility.Hidden;
		this.scriptRichEditControl.OriginalHtmlText = null;
		this.scriptRichEditControl.ReadOnly = true;
		this.scriptRichEditControl.Size = new System.Drawing.Size(879, 538);
		this.scriptRichEditControl.TabIndex = 2;
		this.scriptRichEditControl.Views.SimpleView.Padding = new System.Windows.Forms.Padding(4, 4, 4, 0);
		this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup5.GroupBordersVisible = false;
		this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.layoutControlItem6, this.layoutControlItem9, this.layoutControlItem10 });
		this.layoutControlGroup5.Name = "layoutControlGroup4";
		this.layoutControlGroup5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup5.Size = new System.Drawing.Size(879, 555);
		this.layoutControlGroup5.TextVisible = false;
		this.layoutControlItem6.Control = this.scriptRichEditControl;
		this.layoutControlItem6.Location = new System.Drawing.Point(0, 17);
		this.layoutControlItem6.Name = "layoutControlItem8";
		this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem6.Size = new System.Drawing.Size(879, 538);
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.layoutControlItem9.Control = this.labelControl2;
		this.layoutControlItem9.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem9.Name = "layoutControlItem9";
		this.layoutControlItem9.Size = new System.Drawing.Size(31, 17);
		this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem9.TextVisible = false;
		this.layoutControlItem10.Control = this.scriptTextSearchCountLabelControl;
		this.layoutControlItem10.Location = new System.Drawing.Point(31, 0);
		this.layoutControlItem10.Name = "layoutControlItem10";
		this.layoutControlItem10.Size = new System.Drawing.Size(848, 17);
		this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem10.TextVisible = false;
		this.tableSchemaXtraTabPage.Controls.Add(this.schemaTextLayoutControl);
		this.tableSchemaXtraTabPage.Name = "tableSchemaXtraTabPage";
		this.tableSchemaXtraTabPage.Size = new System.Drawing.Size(879, 555);
		this.tableSchemaXtraTabPage.Text = "Schema";
		this.schemaTextLayoutControl.AllowCustomization = false;
		this.schemaTextLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.schemaTextLayoutControl.Controls.Add(this.schemaTextSearchCountLabelControl);
		this.schemaTextLayoutControl.Controls.Add(this.schemaTextLabelControl);
		this.schemaTextLayoutControl.Controls.Add(this.schemaTextRichEditUserControl);
		this.schemaTextLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.schemaTextLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.schemaTextLayoutControl.Name = "schemaTextLayoutControl";
		this.schemaTextLayoutControl.Root = this.layoutControlGroup6;
		this.schemaTextLayoutControl.Size = new System.Drawing.Size(879, 555);
		this.schemaTextLayoutControl.TabIndex = 4;
		this.schemaTextLayoutControl.Text = "nonCustomizableLayoutControl2";
		this.schemaTextSearchCountLabelControl.Location = new System.Drawing.Point(45, 2);
		this.schemaTextSearchCountLabelControl.Name = "schemaTextSearchCountLabelControl";
		this.schemaTextSearchCountLabelControl.Size = new System.Drawing.Size(832, 13);
		this.schemaTextSearchCountLabelControl.StyleController = this.schemaTextLayoutControl;
		this.schemaTextSearchCountLabelControl.TabIndex = 7;
		this.schemaTextLabelControl.Location = new System.Drawing.Point(2, 2);
		this.schemaTextLabelControl.Name = "schemaTextLabelControl";
		this.schemaTextLabelControl.Size = new System.Drawing.Size(39, 13);
		this.schemaTextLabelControl.StyleController = this.schemaTextLayoutControl;
		this.schemaTextLabelControl.TabIndex = 6;
		this.schemaTextLabelControl.Text = "Schema";
		this.schemaTextRichEditUserControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
		this.schemaTextRichEditUserControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.schemaTextRichEditUserControl.IsHighlighted = false;
		this.schemaTextRichEditUserControl.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
		this.schemaTextRichEditUserControl.Location = new System.Drawing.Point(0, 17);
		this.schemaTextRichEditUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.schemaTextRichEditUserControl.Name = "schemaTextRichEditUserControl";
		this.schemaTextRichEditUserControl.OccurrencesCount = 0;
		this.schemaTextRichEditUserControl.Options.HorizontalScrollbar.Visibility = DevExpress.XtraRichEdit.RichEditScrollbarVisibility.Hidden;
		this.schemaTextRichEditUserControl.OriginalHtmlText = null;
		this.schemaTextRichEditUserControl.ReadOnly = true;
		this.schemaTextRichEditUserControl.Size = new System.Drawing.Size(879, 538);
		this.schemaTextRichEditUserControl.TabIndex = 2;
		this.schemaTextRichEditUserControl.Views.SimpleView.Padding = new System.Windows.Forms.Padding(4, 4, 4, 0);
		this.layoutControlGroup6.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup6.GroupBordersVisible = false;
		this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.layoutControlItem12, this.layoutControlItem13, this.layoutControlItem11 });
		this.layoutControlGroup6.Name = "layoutControlGroup4";
		this.layoutControlGroup6.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup6.Size = new System.Drawing.Size(879, 555);
		this.layoutControlGroup6.TextVisible = false;
		this.layoutControlItem12.Control = this.schemaTextSearchCountLabelControl;
		this.layoutControlItem12.Location = new System.Drawing.Point(43, 0);
		this.layoutControlItem12.Name = "layoutControlItem12";
		this.layoutControlItem12.Size = new System.Drawing.Size(836, 17);
		this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem12.TextVisible = false;
		this.layoutControlItem13.Control = this.schemaTextLabelControl;
		this.layoutControlItem13.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem13.Name = "layoutControlItem13";
		this.layoutControlItem13.Size = new System.Drawing.Size(43, 17);
		this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem13.TextVisible = false;
		this.layoutControlItem11.Control = this.schemaTextRichEditUserControl;
		this.layoutControlItem11.Location = new System.Drawing.Point(0, 17);
		this.layoutControlItem11.Name = "layoutControlItem8";
		this.layoutControlItem11.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem11.Size = new System.Drawing.Size(879, 538);
		this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem11.TextVisible = false;
		this.dataLinksXtraTabPage.Controls.Add(this.dataLinksGrid);
		this.dataLinksXtraTabPage.Controls.Add(this.dataLinksGridPanelUserControl);
		this.dataLinksXtraTabPage.Controls.Add(this.termsLinkUserControl);
		this.dataLinksXtraTabPage.Name = "dataLinksXtraTabPage";
		this.dataLinksXtraTabPage.Size = new System.Drawing.Size(879, 555);
		this.dataLinksXtraTabPage.Text = "Linked terms";
		this.dataLinksXtraTabPage.Tooltip = "Linked Business Glossary terms";
		this.dataLinksGrid.AllowDrop = true;
		this.dataLinksGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.dataLinksGrid.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dataLinksGrid.Location = new System.Drawing.Point(0, 64);
		this.dataLinksGrid.MainView = this.dataLinksGridView;
		this.dataLinksGrid.Name = "dataLinksGrid";
		this.dataLinksGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemAutoHeightMemoEdit1, this.iconRepositoryItemPictureEdit, this.fullNameRepositoryItemCustomTextEdit });
		this.dataLinksGrid.Size = new System.Drawing.Size(879, 491);
		this.dataLinksGrid.TabIndex = 18;
		this.dataLinksGrid.ToolTipController = this.tableToolTipController;
		this.dataLinksGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.dataLinksGridView });
		this.dataLinksGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.termIconDataLinksGridColumn, this.objectDataLinksGridColumn, this.typeDataLinksGridColumn, this.objectIconDataLinksGridColumn, this.tableColumnDataLinksGridColumn, this.createdDataLinksGridColumn, this.createdByDataLinksGridColumn, this.lastUpdatedDataLinksGridColumn, this.lastUpdatedByDataLinksGridColumn });
		defaultBulkCopy4.IsCopying = false;
		this.dataLinksGridView.Copy = defaultBulkCopy4;
		this.dataLinksGridView.GridControl = this.dataLinksGrid;
		this.dataLinksGridView.Name = "dataLinksGridView";
		this.dataLinksGridView.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
		this.dataLinksGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.dataLinksGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.dataLinksGridView.OptionsDetail.EnableMasterViewMode = false;
		this.dataLinksGridView.OptionsFilter.ShowAllTableValuesInFilterPopup = true;
		this.dataLinksGridView.OptionsSelection.MultiSelect = true;
		this.dataLinksGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.dataLinksGridView.OptionsView.ColumnAutoWidth = false;
		this.dataLinksGridView.OptionsView.RowAutoHeight = true;
		this.dataLinksGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.dataLinksGridView.OptionsView.ShowGroupPanel = false;
		this.dataLinksGridView.OptionsView.ShowIndicator = false;
		this.dataLinksGridView.RowHighlightingIsEnabled = true;
		this.dataLinksGridView.SplashScreenManager = null;
		this.dataLinksGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(dataLinksGridView_PopupMenuShowing);
		this.dataLinksGridView.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(dataLinksGridView_CustomColumnSort);
		this.termIconDataLinksGridColumn.Caption = " ";
		this.termIconDataLinksGridColumn.ColumnEdit = this.iconRepositoryItemPictureEdit;
		this.termIconDataLinksGridColumn.FieldName = "TermIcon";
		this.termIconDataLinksGridColumn.MaxWidth = 21;
		this.termIconDataLinksGridColumn.MinWidth = 21;
		this.termIconDataLinksGridColumn.Name = "termIconDataLinksGridColumn";
		this.termIconDataLinksGridColumn.OptionsColumn.AllowEdit = false;
		this.termIconDataLinksGridColumn.OptionsColumn.ReadOnly = true;
		this.termIconDataLinksGridColumn.OptionsFilter.AllowFilter = false;
		this.termIconDataLinksGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.termIconDataLinksGridColumn.Visible = true;
		this.termIconDataLinksGridColumn.VisibleIndex = 0;
		this.termIconDataLinksGridColumn.Width = 21;
		this.iconRepositoryItemPictureEdit.Name = "iconRepositoryItemPictureEdit";
		this.objectDataLinksGridColumn.Caption = "Term";
		this.objectDataLinksGridColumn.FieldName = "TermTitle";
		this.objectDataLinksGridColumn.Name = "objectDataLinksGridColumn";
		this.objectDataLinksGridColumn.OptionsColumn.AllowEdit = false;
		this.objectDataLinksGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.objectDataLinksGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.objectDataLinksGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.objectDataLinksGridColumn.Tag = "FIT_WIDTH";
		this.objectDataLinksGridColumn.Visible = true;
		this.objectDataLinksGridColumn.VisibleIndex = 1;
		this.typeDataLinksGridColumn.Caption = "Type";
		this.typeDataLinksGridColumn.FieldName = "TermType";
		this.typeDataLinksGridColumn.Name = "typeDataLinksGridColumn";
		this.typeDataLinksGridColumn.OptionsColumn.AllowEdit = false;
		this.typeDataLinksGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.typeDataLinksGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.typeDataLinksGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.typeDataLinksGridColumn.Tag = "FIT_WIDTH";
		this.typeDataLinksGridColumn.Visible = true;
		this.typeDataLinksGridColumn.VisibleIndex = 2;
		this.objectIconDataLinksGridColumn.Caption = " ";
		this.objectIconDataLinksGridColumn.ColumnEdit = this.iconRepositoryItemPictureEdit;
		this.objectIconDataLinksGridColumn.FieldName = "ObjectIcon";
		this.objectIconDataLinksGridColumn.MaxWidth = 21;
		this.objectIconDataLinksGridColumn.MinWidth = 21;
		this.objectIconDataLinksGridColumn.Name = "objectIconDataLinksGridColumn";
		this.objectIconDataLinksGridColumn.OptionsColumn.AllowEdit = false;
		this.objectIconDataLinksGridColumn.OptionsColumn.ReadOnly = true;
		this.objectIconDataLinksGridColumn.OptionsFilter.AllowFilter = false;
		this.objectIconDataLinksGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.objectIconDataLinksGridColumn.Visible = true;
		this.objectIconDataLinksGridColumn.VisibleIndex = 3;
		this.objectIconDataLinksGridColumn.Width = 21;
		this.tableColumnDataLinksGridColumn.Caption = "Table/Column";
		this.tableColumnDataLinksGridColumn.ColumnEdit = this.fullNameRepositoryItemCustomTextEdit;
		this.tableColumnDataLinksGridColumn.FieldName = "FullNameForObjectFormatted";
		this.tableColumnDataLinksGridColumn.Name = "tableColumnDataLinksGridColumn";
		this.tableColumnDataLinksGridColumn.OptionsColumn.AllowEdit = false;
		this.tableColumnDataLinksGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.tableColumnDataLinksGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.tableColumnDataLinksGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.tableColumnDataLinksGridColumn.Tag = "FIT_WIDTH";
		this.tableColumnDataLinksGridColumn.Visible = true;
		this.tableColumnDataLinksGridColumn.VisibleIndex = 4;
		this.tableColumnDataLinksGridColumn.Width = 140;
		this.fullNameRepositoryItemCustomTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.fullNameRepositoryItemCustomTextEdit.AutoHeight = false;
		this.fullNameRepositoryItemCustomTextEdit.Name = "fullNameRepositoryItemCustomTextEdit";
		this.createdDataLinksGridColumn.Caption = "Created";
		this.createdDataLinksGridColumn.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
		this.createdDataLinksGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.createdDataLinksGridColumn.FieldName = "CreationDate";
		this.createdDataLinksGridColumn.Name = "createdDataLinksGridColumn";
		this.createdDataLinksGridColumn.OptionsColumn.AllowEdit = false;
		this.createdDataLinksGridColumn.OptionsFilter.AllowFilter = false;
		this.createdDataLinksGridColumn.Width = 120;
		this.createdByDataLinksGridColumn.Caption = "Created by";
		this.createdByDataLinksGridColumn.FieldName = "CreatedBy";
		this.createdByDataLinksGridColumn.Name = "createdByDataLinksGridColumn";
		this.createdByDataLinksGridColumn.OptionsColumn.AllowEdit = false;
		this.createdByDataLinksGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.createdByDataLinksGridColumn.Width = 150;
		this.lastUpdatedDataLinksGridColumn.Caption = "Last updated";
		this.lastUpdatedDataLinksGridColumn.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm";
		this.lastUpdatedDataLinksGridColumn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.lastUpdatedDataLinksGridColumn.FieldName = "LastModificationDate";
		this.lastUpdatedDataLinksGridColumn.Name = "lastUpdatedDataLinksGridColumn";
		this.lastUpdatedDataLinksGridColumn.OptionsColumn.AllowEdit = false;
		this.lastUpdatedDataLinksGridColumn.OptionsFilter.AllowFilter = false;
		this.lastUpdatedDataLinksGridColumn.Width = 100;
		this.lastUpdatedByDataLinksGridColumn.Caption = "Last updated by";
		this.lastUpdatedByDataLinksGridColumn.FieldName = "ModifiedBy";
		this.lastUpdatedByDataLinksGridColumn.Name = "lastUpdatedByDataLinksGridColumn";
		this.lastUpdatedByDataLinksGridColumn.OptionsColumn.AllowEdit = false;
		this.lastUpdatedByDataLinksGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.lastUpdatedByDataLinksGridColumn.Width = 100;
		this.repositoryItemAutoHeightMemoEdit1.Name = "repositoryItemAutoHeightMemoEdit1";
		this.repositoryItemAutoHeightMemoEdit1.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.dataLinksGridPanelUserControl.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.dataLinksGridPanelUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.dataLinksGridPanelUserControl.GridView = this.dataLinksGridView;
		this.dataLinksGridPanelUserControl.Location = new System.Drawing.Point(0, 36);
		this.dataLinksGridPanelUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.dataLinksGridPanelUserControl.Name = "dataLinksGridPanelUserControl";
		this.dataLinksGridPanelUserControl.Size = new System.Drawing.Size(879, 28);
		this.dataLinksGridPanelUserControl.TabIndex = 17;
		this.termsLinkUserControl.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.termsLinkUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.termsLinkUserControl.Location = new System.Drawing.Point(0, 0);
		this.termsLinkUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.termsLinkUserControl.Name = "termsLinkUserControl";
		this.termsLinkUserControl.Size = new System.Drawing.Size(879, 36);
		this.termsLinkUserControl.TabIndex = 19;
		this.schemaImportsAndChangesXtraTabPage.Controls.Add(this.schemaImportsAndChangesUserControl);
		this.schemaImportsAndChangesXtraTabPage.Margin = new System.Windows.Forms.Padding(0);
		this.schemaImportsAndChangesXtraTabPage.Name = "schemaImportsAndChangesXtraTabPage";
		this.schemaImportsAndChangesXtraTabPage.Size = new System.Drawing.Size(879, 555);
		this.schemaImportsAndChangesXtraTabPage.Text = "Schema Changes";
		this.schemaImportsAndChangesUserControl.BackColor = System.Drawing.Color.Transparent;
		this.schemaImportsAndChangesUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.schemaImportsAndChangesUserControl.ErrorOccurred = false;
		this.schemaImportsAndChangesUserControl.ExpandAllRequested = false;
		this.schemaImportsAndChangesUserControl.IsChanged = false;
		this.schemaImportsAndChangesUserControl.Location = new System.Drawing.Point(0, 0);
		this.schemaImportsAndChangesUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.schemaImportsAndChangesUserControl.Name = "schemaImportsAndChangesUserControl";
		this.schemaImportsAndChangesUserControl.ShowAllImports = true;
		this.schemaImportsAndChangesUserControl.Size = new System.Drawing.Size(879, 555);
		this.schemaImportsAndChangesUserControl.TabIndex = 1;
		this.metadataXtraTabPage.Controls.Add(this.metadataLayoutControl);
		this.metadataXtraTabPage.Name = "metadataXtraTabPage";
		this.metadataXtraTabPage.Size = new System.Drawing.Size(879, 555);
		this.metadataXtraTabPage.Text = "Metadata";
		this.metadataLayoutControl.AllowCustomization = false;
		this.metadataLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.metadataLayoutControl.Controls.Add(this.dbmsLastUpdatedLabel);
		this.metadataLayoutControl.Controls.Add(this.lastSynchronizedLabel);
		this.metadataLayoutControl.Controls.Add(this.lastUpdatedLabelControl);
		this.metadataLayoutControl.Controls.Add(this.dbmsCreatedLabelControl);
		this.metadataLayoutControl.Controls.Add(this.createdLabelControl);
		this.metadataLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.metadataLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.metadataLayoutControl.Name = "metadataLayoutControl";
		this.metadataLayoutControl.Root = this.layoutControlGroup2;
		this.metadataLayoutControl.Size = new System.Drawing.Size(879, 555);
		this.metadataLayoutControl.TabIndex = 0;
		this.metadataLayoutControl.Text = "layoutControl2";
		this.metadataLayoutControl.ToolTipController = this.metadataToolTipController;
		this.dbmsLastUpdatedLabel.Location = new System.Drawing.Point(147, 36);
		this.dbmsLastUpdatedLabel.Name = "dbmsLastUpdatedLabel";
		this.dbmsLastUpdatedLabel.Size = new System.Drawing.Size(720, 20);
		this.dbmsLastUpdatedLabel.StyleController = this.metadataLayoutControl;
		this.dbmsLastUpdatedLabel.TabIndex = 13;
		this.lastSynchronizedLabel.Location = new System.Drawing.Point(147, 84);
		this.lastSynchronizedLabel.Name = "lastSynchronizedLabel";
		this.lastSynchronizedLabel.Size = new System.Drawing.Size(720, 20);
		this.lastSynchronizedLabel.StyleController = this.metadataLayoutControl;
		this.lastSynchronizedLabel.TabIndex = 14;
		this.lastUpdatedLabelControl.Location = new System.Drawing.Point(147, 108);
		this.lastUpdatedLabelControl.Name = "lastUpdatedLabelControl";
		this.lastUpdatedLabelControl.Size = new System.Drawing.Size(720, 20);
		this.lastUpdatedLabelControl.StyleController = this.metadataLayoutControl;
		this.lastUpdatedLabelControl.TabIndex = 14;
		this.dbmsCreatedLabelControl.Location = new System.Drawing.Point(147, 12);
		this.dbmsCreatedLabelControl.Name = "dbmsCreatedLabelControl";
		this.dbmsCreatedLabelControl.Size = new System.Drawing.Size(720, 20);
		this.dbmsCreatedLabelControl.StyleController = this.metadataLayoutControl;
		this.dbmsCreatedLabelControl.TabIndex = 14;
		this.createdLabelControl.Location = new System.Drawing.Point(147, 60);
		this.createdLabelControl.Name = "createdLabelControl";
		this.createdLabelControl.Size = new System.Drawing.Size(720, 20);
		this.createdLabelControl.StyleController = this.metadataLayoutControl;
		this.createdLabelControl.TabIndex = 14;
		this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup2.GroupBordersVisible = false;
		this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.dbmsLastUpdatedLayoutControlItem, this.emptySpaceItem3, this.lastImportedLayoutControlItem, this.lastUpdatedLayoutControlItem, this.dbmsCreatedLayoutControlItem, this.firstImportedLayoutControlItem });
		this.layoutControlGroup2.Name = "Root";
		this.layoutControlGroup2.Size = new System.Drawing.Size(879, 555);
		this.layoutControlGroup2.TextVisible = false;
		this.dbmsLastUpdatedLayoutControlItem.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.dbmsLastUpdatedLayoutControlItem.AppearanceItemCaption.Options.UseFont = true;
		this.dbmsLastUpdatedLayoutControlItem.Control = this.dbmsLastUpdatedLabel;
		this.dbmsLastUpdatedLayoutControlItem.CustomizationFormText = "DBMS last updated:";
		this.dbmsLastUpdatedLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.dbmsLastUpdatedLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.dbmsLastUpdatedLayoutControlItem.MinSize = new System.Drawing.Size(116, 24);
		this.dbmsLastUpdatedLayoutControlItem.Name = "dbmsLastUpdatedLayoutControlItem";
		this.dbmsLastUpdatedLayoutControlItem.Size = new System.Drawing.Size(859, 24);
		this.dbmsLastUpdatedLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.dbmsLastUpdatedLayoutControlItem.Text = "Source last updated";
		this.dbmsLastUpdatedLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.dbmsLastUpdatedLayoutControlItem.TextSize = new System.Drawing.Size(130, 13);
		this.dbmsLastUpdatedLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 120);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(859, 415);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.lastImportedLayoutControlItem.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lastImportedLayoutControlItem.AppearanceItemCaption.Options.UseFont = true;
		this.lastImportedLayoutControlItem.Control = this.lastSynchronizedLabel;
		this.lastImportedLayoutControlItem.CustomizationFormText = "Last imported:";
		this.lastImportedLayoutControlItem.Location = new System.Drawing.Point(0, 72);
		this.lastImportedLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.lastImportedLayoutControlItem.MinSize = new System.Drawing.Size(116, 24);
		this.lastImportedLayoutControlItem.Name = "lastImportedLayoutControlItem";
		this.lastImportedLayoutControlItem.Size = new System.Drawing.Size(859, 24);
		this.lastImportedLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.lastImportedLayoutControlItem.Text = "Last imported";
		this.lastImportedLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.lastImportedLayoutControlItem.TextSize = new System.Drawing.Size(130, 13);
		this.lastImportedLayoutControlItem.TextToControlDistance = 5;
		this.lastUpdatedLayoutControlItem.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lastUpdatedLayoutControlItem.AppearanceItemCaption.Options.UseFont = true;
		this.lastUpdatedLayoutControlItem.Control = this.lastUpdatedLabelControl;
		this.lastUpdatedLayoutControlItem.CustomizationFormText = "Last imported:";
		this.lastUpdatedLayoutControlItem.Location = new System.Drawing.Point(0, 96);
		this.lastUpdatedLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.lastUpdatedLayoutControlItem.MinSize = new System.Drawing.Size(116, 24);
		this.lastUpdatedLayoutControlItem.Name = "lastUpdatedLayoutControlItem";
		this.lastUpdatedLayoutControlItem.Size = new System.Drawing.Size(859, 24);
		this.lastUpdatedLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.lastUpdatedLayoutControlItem.Text = "Last updated";
		this.lastUpdatedLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.lastUpdatedLayoutControlItem.TextSize = new System.Drawing.Size(130, 13);
		this.lastUpdatedLayoutControlItem.TextToControlDistance = 5;
		this.dbmsCreatedLayoutControlItem.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.dbmsCreatedLayoutControlItem.AppearanceItemCaption.Options.UseFont = true;
		this.dbmsCreatedLayoutControlItem.Control = this.dbmsCreatedLabelControl;
		this.dbmsCreatedLayoutControlItem.CustomizationFormText = "DBMS created:";
		this.dbmsCreatedLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.dbmsCreatedLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.dbmsCreatedLayoutControlItem.MinSize = new System.Drawing.Size(116, 24);
		this.dbmsCreatedLayoutControlItem.Name = "dbmsCreatedLayoutControlItem";
		this.dbmsCreatedLayoutControlItem.Size = new System.Drawing.Size(859, 24);
		this.dbmsCreatedLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.dbmsCreatedLayoutControlItem.Text = "Source created";
		this.dbmsCreatedLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.dbmsCreatedLayoutControlItem.TextSize = new System.Drawing.Size(130, 13);
		this.dbmsCreatedLayoutControlItem.TextToControlDistance = 5;
		this.firstImportedLayoutControlItem.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.firstImportedLayoutControlItem.AppearanceItemCaption.Options.UseFont = true;
		this.firstImportedLayoutControlItem.Control = this.createdLabelControl;
		this.firstImportedLayoutControlItem.CustomizationFormText = "Created/first imported:";
		this.firstImportedLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.firstImportedLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.firstImportedLayoutControlItem.MinSize = new System.Drawing.Size(116, 24);
		this.firstImportedLayoutControlItem.Name = "firstImportedLayoutControlItem";
		this.firstImportedLayoutControlItem.Size = new System.Drawing.Size(859, 24);
		this.firstImportedLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.firstImportedLayoutControlItem.Text = "Created/first imported";
		this.firstImportedLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.firstImportedLayoutControlItem.TextSize = new System.Drawing.Size(130, 13);
		this.firstImportedLayoutControlItem.TextToControlDistance = 5;
		this.metadataToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.treeMenuImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("treeMenuImageCollection.ImageStream");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.function_16, "FUNCTION", typeof(Dataedo.App.Properties.Resources), 0, "function_16");
		this.treeMenuImageCollection.Images.SetKeyName(0, "FUNCTION");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.procedure_16, "PROCEDURE", typeof(Dataedo.App.Properties.Resources), 1, "procedure_16");
		this.treeMenuImageCollection.Images.SetKeyName(1, "PROCEDURE");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.table_16, "TABLE", typeof(Dataedo.App.Properties.Resources), 2, "table_16");
		this.treeMenuImageCollection.Images.SetKeyName(2, "TABLE");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.view_16, "VIEW", typeof(Dataedo.App.Properties.Resources), 3, "view_16");
		this.treeMenuImageCollection.Images.SetKeyName(3, "VIEW");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.column_16, "COLUMN", typeof(Dataedo.App.Properties.Resources), 4, "column_16");
		this.treeMenuImageCollection.Images.SetKeyName(4, "COLUMN");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.trigger_active_16, "TRIGGER", typeof(Dataedo.App.Properties.Resources), 5, "trigger_active_16");
		this.treeMenuImageCollection.Images.SetKeyName(5, "TRIGGER");
		this.relationNameErrorProvider.ContainerControl = this;
		this.tableStatusUserControl.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.tableStatusUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.tableStatusUserControl.Description = "This table has been removed from the database. You can remove it from the repository.";
		this.tableStatusUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.tableStatusUserControl.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
		this.tableStatusUserControl.Image = Dataedo.App.Properties.Resources.warning_16;
		this.tableStatusUserControl.Location = new System.Drawing.Point(0, 20);
		this.tableStatusUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.tableStatusUserControl.Name = "tableStatusUserControl";
		this.tableStatusUserControl.Size = new System.Drawing.Size(881, 40);
		this.tableStatusUserControl.TabIndex = 9;
		this.tableStatusUserControl.Visible = false;
		this.tableTextEdit.Dock = System.Windows.Forms.DockStyle.Top;
		this.tableTextEdit.EditValue = "";
		this.tableTextEdit.Location = new System.Drawing.Point(0, 0);
		this.tableTextEdit.Name = "tableTextEdit";
		this.tableTextEdit.Properties.AllowFocused = false;
		this.tableTextEdit.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold);
		this.tableTextEdit.Properties.Appearance.Options.UseFont = true;
		this.tableTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.tableTextEdit.Properties.ContextImageOptions.Image = Dataedo.App.Properties.Resources.add_16;
		this.tableTextEdit.Properties.ReadOnly = true;
		this.tableTextEdit.Size = new System.Drawing.Size(881, 20);
		this.tableTextEdit.TabIndex = 10;
		this.tableTextEdit.TabStop = false;
		this.tableTextEdit.ToolTipController = this.toolTipController;
		this.linkedTermsPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.goToObjectLinkedTermsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.editLinksLinkedTermsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeLinkLinkedTermsBarButtonItem)
		});
		this.linkedTermsPopupMenu.Manager = this.linkedTermsBarManager;
		this.linkedTermsPopupMenu.Name = "linkedTermsPopupMenu";
		this.goToObjectLinkedTermsBarButtonItem.Caption = "Go to object";
		this.goToObjectLinkedTermsBarButtonItem.Id = 0;
		this.goToObjectLinkedTermsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.term_16;
		this.goToObjectLinkedTermsBarButtonItem.Name = "goToObjectLinkedTermsBarButtonItem";
		this.editLinksLinkedTermsBarButtonItem.Caption = "Edit links to Business Glossary terms";
		this.editLinksLinkedTermsBarButtonItem.Id = 1;
		this.editLinksLinkedTermsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.data_link_add_16;
		this.editLinksLinkedTermsBarButtonItem.Name = "editLinksLinkedTermsBarButtonItem";
		this.removeLinkLinkedTermsBarButtonItem.Caption = "Remove from repository";
		this.removeLinkLinkedTermsBarButtonItem.Id = 2;
		this.removeLinkLinkedTermsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeLinkLinkedTermsBarButtonItem.Name = "removeLinkLinkedTermsBarButtonItem";
		this.linkedTermsBarManager.DockControls.Add(this.barDockControlTop);
		this.linkedTermsBarManager.DockControls.Add(this.barDockControlBottom);
		this.linkedTermsBarManager.DockControls.Add(this.barDockControlLeft);
		this.linkedTermsBarManager.DockControls.Add(this.barDockControlRight);
		this.linkedTermsBarManager.Form = this;
		this.linkedTermsBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[3] { this.goToObjectLinkedTermsBarButtonItem, this.editLinksLinkedTermsBarButtonItem, this.removeLinkLinkedTermsBarButtonItem });
		this.linkedTermsBarManager.MaxItemId = 3;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.linkedTermsBarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(881, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 640);
		this.barDockControlBottom.Manager = this.linkedTermsBarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(881, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.linkedTermsBarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 640);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(881, 0);
		this.barDockControlRight.Manager = this.linkedTermsBarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 640);
		this.uniqueKeysPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.addPrimaryKeyUniqueKeysBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.addUniqueKeyUniqueKeysBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.editKeyUniqueKeysBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeKeyUniqueKeysBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.viewHistoryUniqueKeysBarButtonItem)
		});
		this.uniqueKeysPopupMenu.Manager = this.uniqueKeysBarManager;
		this.uniqueKeysPopupMenu.Name = "uniqueKeysPopupMenu";
		this.uniqueKeysPopupMenu.BeforePopup += new System.ComponentModel.CancelEventHandler(uniqueKeysPopupMenu_BeforePopup);
		this.addPrimaryKeyUniqueKeysBarButtonItem.Caption = "Add primary key";
		this.addPrimaryKeyUniqueKeysBarButtonItem.Id = 0;
		this.addPrimaryKeyUniqueKeysBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.primary_key_add_16;
		this.addPrimaryKeyUniqueKeysBarButtonItem.Name = "addPrimaryKeyUniqueKeysBarButtonItem";
		this.addPrimaryKeyUniqueKeysBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addPrimaryKeyUniqueKeysBarButtonItem_ItemClick);
		this.addUniqueKeyUniqueKeysBarButtonItem.Caption = "Add unique key";
		this.addUniqueKeyUniqueKeysBarButtonItem.Id = 1;
		this.addUniqueKeyUniqueKeysBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.unique_key_add_16;
		this.addUniqueKeyUniqueKeysBarButtonItem.Name = "addUniqueKeyUniqueKeysBarButtonItem";
		this.addUniqueKeyUniqueKeysBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addUniqueKeyUniqueKeysBarButtonItem_ItemClick);
		this.editKeyUniqueKeysBarButtonItem.Caption = "Edit key";
		this.editKeyUniqueKeysBarButtonItem.Id = 2;
		this.editKeyUniqueKeysBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
		this.editKeyUniqueKeysBarButtonItem.Name = "editKeyUniqueKeysBarButtonItem";
		this.editKeyUniqueKeysBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(editKeyUniqueKeysBarButtonItem_ItemClick);
		this.removeKeyUniqueKeysBarButtonItem.Caption = "Remove from repository";
		this.removeKeyUniqueKeysBarButtonItem.Id = 3;
		this.removeKeyUniqueKeysBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.key_delete_16;
		this.removeKeyUniqueKeysBarButtonItem.Name = "removeKeyUniqueKeysBarButtonItem";
		this.viewHistoryUniqueKeysBarButtonItem.Caption = "View History";
		this.viewHistoryUniqueKeysBarButtonItem.Id = 4;
		this.viewHistoryUniqueKeysBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.search_16;
		this.viewHistoryUniqueKeysBarButtonItem.Name = "viewHistoryUniqueKeysBarButtonItem";
		this.viewHistoryUniqueKeysBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
		this.uniqueKeysBarManager.DockControls.Add(this.barDockControl1);
		this.uniqueKeysBarManager.DockControls.Add(this.barDockControl2);
		this.uniqueKeysBarManager.DockControls.Add(this.barDockControl3);
		this.uniqueKeysBarManager.DockControls.Add(this.barDockControl4);
		this.uniqueKeysBarManager.Form = this;
		this.uniqueKeysBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[5] { this.addPrimaryKeyUniqueKeysBarButtonItem, this.addUniqueKeyUniqueKeysBarButtonItem, this.editKeyUniqueKeysBarButtonItem, this.removeKeyUniqueKeysBarButtonItem, this.viewHistoryUniqueKeysBarButtonItem });
		this.uniqueKeysBarManager.MaxItemId = 5;
		this.uniqueKeysBarManager.ShowScreenTipsInMenus = true;
		this.barDockControl1.CausesValidation = false;
		this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControl1.Location = new System.Drawing.Point(0, 0);
		this.barDockControl1.Manager = this.uniqueKeysBarManager;
		this.barDockControl1.Size = new System.Drawing.Size(881, 0);
		this.barDockControl2.CausesValidation = false;
		this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControl2.Location = new System.Drawing.Point(0, 640);
		this.barDockControl2.Manager = this.uniqueKeysBarManager;
		this.barDockControl2.Size = new System.Drawing.Size(881, 0);
		this.barDockControl3.CausesValidation = false;
		this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControl3.Location = new System.Drawing.Point(0, 0);
		this.barDockControl3.Manager = this.uniqueKeysBarManager;
		this.barDockControl3.Size = new System.Drawing.Size(0, 640);
		this.barDockControl4.CausesValidation = false;
		this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControl4.Location = new System.Drawing.Point(881, 0);
		this.barDockControl4.Manager = this.uniqueKeysBarManager;
		this.barDockControl4.Size = new System.Drawing.Size(0, 640);
		this.triggersPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.removeTriggerTriggersBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.viewHistoryTriggersBarButtonItem)
		});
		this.triggersPopupMenu.Manager = this.triggersBarManager;
		this.triggersPopupMenu.Name = "triggersPopupMenu";
		this.triggersPopupMenu.BeforePopup += new System.ComponentModel.CancelEventHandler(triggersPopupMenu_BeforePopup);
		this.removeTriggerTriggersBarButtonItem.Caption = "Remove from repository";
		this.removeTriggerTriggersBarButtonItem.Id = 0;
		this.removeTriggerTriggersBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeTriggerTriggersBarButtonItem.Name = "removeTriggerTriggersBarButtonItem";
		this.viewHistoryTriggersBarButtonItem.Caption = "View History";
		this.viewHistoryTriggersBarButtonItem.Id = 1;
		this.viewHistoryTriggersBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.search_16;
		this.viewHistoryTriggersBarButtonItem.Name = "viewHistoryTriggersBarButtonItem";
		this.viewHistoryTriggersBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ViewHistoryTriggersVarButtonItem_ItemClick);
		this.triggersBarManager.DockControls.Add(this.barDockControl5);
		this.triggersBarManager.DockControls.Add(this.barDockControl6);
		this.triggersBarManager.DockControls.Add(this.barDockControl7);
		this.triggersBarManager.DockControls.Add(this.barDockControl8);
		this.triggersBarManager.Form = this;
		this.triggersBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[2] { this.removeTriggerTriggersBarButtonItem, this.viewHistoryTriggersBarButtonItem });
		this.triggersBarManager.MaxItemId = 2;
		this.barDockControl5.CausesValidation = false;
		this.barDockControl5.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControl5.Location = new System.Drawing.Point(0, 0);
		this.barDockControl5.Manager = this.triggersBarManager;
		this.barDockControl5.Size = new System.Drawing.Size(881, 0);
		this.barDockControl6.CausesValidation = false;
		this.barDockControl6.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControl6.Location = new System.Drawing.Point(0, 640);
		this.barDockControl6.Manager = this.triggersBarManager;
		this.barDockControl6.Size = new System.Drawing.Size(881, 0);
		this.barDockControl7.CausesValidation = false;
		this.barDockControl7.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControl7.Location = new System.Drawing.Point(0, 0);
		this.barDockControl7.Manager = this.triggersBarManager;
		this.barDockControl7.Size = new System.Drawing.Size(0, 640);
		this.barDockControl8.CausesValidation = false;
		this.barDockControl8.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControl8.Location = new System.Drawing.Point(881, 0);
		this.barDockControl8.Manager = this.triggersBarManager;
		this.barDockControl8.Size = new System.Drawing.Size(0, 640);
		this.relationsPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[5]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.addRelationRelationsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.editRelationRelationsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeRelationRelationsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.goToObjectRelationsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.viewHistoryRelationsBarButtonItem)
		});
		this.relationsPopupMenu.Manager = this.relationsBarManager;
		this.relationsPopupMenu.Name = "relationsPopupMenu";
		this.relationsPopupMenu.BeforePopup += new System.ComponentModel.CancelEventHandler(relationsPopupMenu_BeforePopup);
		this.addRelationRelationsBarButtonItem.Caption = "Add relationship";
		this.addRelationRelationsBarButtonItem.Id = 0;
		this.addRelationRelationsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.relation_add_16;
		this.addRelationRelationsBarButtonItem.Name = "addRelationRelationsBarButtonItem";
		this.addRelationRelationsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addRelationRelationsBarButtonItem_ItemClick);
		this.editRelationRelationsBarButtonItem.Caption = "Edit relationship";
		this.editRelationRelationsBarButtonItem.Id = 1;
		this.editRelationRelationsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
		this.editRelationRelationsBarButtonItem.Name = "editRelationRelationsBarButtonItem";
		this.editRelationRelationsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(editRelationRelationsBarButtonItem_ItemClick);
		this.removeRelationRelationsBarButtonItem.Caption = "Remove from repository";
		this.removeRelationRelationsBarButtonItem.Id = 2;
		this.removeRelationRelationsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeRelationRelationsBarButtonItem.Name = "removeRelationRelationsBarButtonItem";
		this.removeRelationRelationsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(removeRelationRelationsBarButtonItem_ItemClick);
		this.goToObjectRelationsBarButtonItem.Caption = "Go to object";
		this.goToObjectRelationsBarButtonItem.Id = 3;
		this.goToObjectRelationsBarButtonItem.Name = "goToObjectRelationsBarButtonItem";
		this.goToObjectRelationsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(goToObjectRelationsBarButtonItem_ItemClick);
		this.viewHistoryRelationsBarButtonItem.Caption = "View History";
		this.viewHistoryRelationsBarButtonItem.Id = 4;
		this.viewHistoryRelationsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.search_16;
		this.viewHistoryRelationsBarButtonItem.Name = "viewHistoryRelationsBarButtonItem";
		this.viewHistoryRelationsBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
		this.relationsBarManager.DockControls.Add(this.barDockControl9);
		this.relationsBarManager.DockControls.Add(this.barDockControl10);
		this.relationsBarManager.DockControls.Add(this.barDockControl11);
		this.relationsBarManager.DockControls.Add(this.barDockControl12);
		this.relationsBarManager.Form = this;
		this.relationsBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[5] { this.addRelationRelationsBarButtonItem, this.editRelationRelationsBarButtonItem, this.removeRelationRelationsBarButtonItem, this.goToObjectRelationsBarButtonItem, this.viewHistoryRelationsBarButtonItem });
		this.relationsBarManager.MaxItemId = 5;
		this.barDockControl9.CausesValidation = false;
		this.barDockControl9.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControl9.Location = new System.Drawing.Point(0, 0);
		this.barDockControl9.Manager = this.relationsBarManager;
		this.barDockControl9.Size = new System.Drawing.Size(881, 0);
		this.barDockControl10.CausesValidation = false;
		this.barDockControl10.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControl10.Location = new System.Drawing.Point(0, 640);
		this.barDockControl10.Manager = this.relationsBarManager;
		this.barDockControl10.Size = new System.Drawing.Size(881, 0);
		this.barDockControl11.CausesValidation = false;
		this.barDockControl11.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControl11.Location = new System.Drawing.Point(0, 0);
		this.barDockControl11.Manager = this.relationsBarManager;
		this.barDockControl11.Size = new System.Drawing.Size(0, 640);
		this.barDockControl12.CausesValidation = false;
		this.barDockControl12.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControl12.Location = new System.Drawing.Point(881, 0);
		this.barDockControl12.Manager = this.relationsBarManager;
		this.barDockControl12.Size = new System.Drawing.Size(0, 640);
		this.columnsPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[8]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.addRelationColumnsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.profileColumnBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.clearColumnProfilingDataBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.designTableColumnsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.editLinksColumnsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.addNewLinkedTermColumnsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeColumnsColumnsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.viewHistoryBarButtonItem)
		});
		this.columnsPopupMenu.Manager = this.columnsBarManager;
		this.columnsPopupMenu.Name = "columnsPopupMenu";
		this.columnsPopupMenu.Popup += new System.EventHandler(columnsPopupMenu_Popup);
		this.columnsPopupMenu.BeforePopup += new System.ComponentModel.CancelEventHandler(columnsPopupMenu_BeforePopup);
		this.addRelationColumnsBarButtonItem.Caption = "Add relationship";
		this.addRelationColumnsBarButtonItem.Id = 0;
		this.addRelationColumnsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.relation_add_16;
		this.addRelationColumnsBarButtonItem.Name = "addRelationColumnsBarButtonItem";
		this.addRelationColumnsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addRelationColumnsBarButtonItem_ItemClick);
		this.profileColumnBarButtonItem.Caption = "Profile Column";
		this.profileColumnBarButtonItem.Id = 6;
		this.profileColumnBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.profile_column_16;
		this.profileColumnBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.profile_column_32;
		this.profileColumnBarButtonItem.Name = "profileColumnBarButtonItem";
		this.profileColumnBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ProfileColumnBarButtonItemAsync_ItemClick);
		this.clearColumnProfilingDataBarButtonItem.Caption = "Clear all Profiling Data";
		this.clearColumnProfilingDataBarButtonItem.Id = 7;
		this.clearColumnProfilingDataBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.all_data_deleted_16;
		this.clearColumnProfilingDataBarButtonItem.Name = "clearColumnProfilingDataBarButtonItem";
		this.clearColumnProfilingDataBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ClearColumnProfilingDataBarButtonItem_ItemClick);
		this.designTableColumnsBarButtonItem.Caption = "Design table";
		this.designTableColumnsBarButtonItem.Id = 1;
		this.designTableColumnsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
		this.designTableColumnsBarButtonItem.Name = "designTableColumnsBarButtonItem";
		this.designTableColumnsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(designTableColumnsBarButtonItem_ItemClick);
		this.editLinksColumnsBarButtonItem.Caption = "Edit links to Business Glossary terms";
		this.editLinksColumnsBarButtonItem.Id = 2;
		this.editLinksColumnsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.data_link_add_16;
		this.editLinksColumnsBarButtonItem.Name = "editLinksColumnsBarButtonItem";
		this.addNewLinkedTermColumnsBarButtonItem.Caption = "Add new linked Business Glossary term";
		this.addNewLinkedTermColumnsBarButtonItem.Id = 3;
		this.addNewLinkedTermColumnsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.term_add_16;
		this.addNewLinkedTermColumnsBarButtonItem.Name = "addNewLinkedTermColumnsBarButtonItem";
		this.removeColumnsColumnsBarButtonItem.Caption = "Remove from repository";
		this.removeColumnsColumnsBarButtonItem.Id = 4;
		this.removeColumnsColumnsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeColumnsColumnsBarButtonItem.Name = "removeColumnsColumnsBarButtonItem";
		this.viewHistoryBarButtonItem.Caption = "View History";
		this.viewHistoryBarButtonItem.Id = 8;
		this.viewHistoryBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.search_16;
		this.viewHistoryBarButtonItem.Name = "viewHistoryBarButtonItem";
		this.viewHistoryBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ViewHistoryBarButtonItem_ItemClick);
		this.columnsBarManager.DockControls.Add(this.barDockControl13);
		this.columnsBarManager.DockControls.Add(this.barDockControl14);
		this.columnsBarManager.DockControls.Add(this.barDockControl15);
		this.columnsBarManager.DockControls.Add(this.barDockControl16);
		this.columnsBarManager.Form = this;
		this.columnsBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[8] { this.addRelationColumnsBarButtonItem, this.designTableColumnsBarButtonItem, this.editLinksColumnsBarButtonItem, this.addNewLinkedTermColumnsBarButtonItem, this.removeColumnsColumnsBarButtonItem, this.profileColumnBarButtonItem, this.clearColumnProfilingDataBarButtonItem, this.viewHistoryBarButtonItem });
		this.columnsBarManager.MaxItemId = 9;
		this.columnsBarManager.HighlightedLinkChanged += new DevExpress.XtraBars.HighlightedLinkChangedEventHandler(columnsBarManager_HighlightedLinkChanged);
		this.barDockControl13.CausesValidation = false;
		this.barDockControl13.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControl13.Location = new System.Drawing.Point(0, 0);
		this.barDockControl13.Manager = this.columnsBarManager;
		this.barDockControl13.Size = new System.Drawing.Size(881, 0);
		this.barDockControl14.CausesValidation = false;
		this.barDockControl14.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControl14.Location = new System.Drawing.Point(0, 640);
		this.barDockControl14.Manager = this.columnsBarManager;
		this.barDockControl14.Size = new System.Drawing.Size(881, 0);
		this.barDockControl15.CausesValidation = false;
		this.barDockControl15.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControl15.Location = new System.Drawing.Point(0, 0);
		this.barDockControl15.Manager = this.columnsBarManager;
		this.barDockControl15.Size = new System.Drawing.Size(0, 640);
		this.barDockControl16.CausesValidation = false;
		this.barDockControl16.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControl16.Location = new System.Drawing.Point(881, 0);
		this.barDockControl16.Manager = this.columnsBarManager;
		this.barDockControl16.Size = new System.Drawing.Size(0, 640);
		this.barSubItem1.Caption = "barSubItem1";
		this.barSubItem1.Id = 5;
		this.barSubItem1.Name = "barSubItem1";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.AutoSize = true;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.tableXtraTabControl);
		base.Controls.Add(this.tableStatusUserControl);
		base.Controls.Add(this.tableTextEdit);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Controls.Add(this.barDockControl3);
		base.Controls.Add(this.barDockControl4);
		base.Controls.Add(this.barDockControl2);
		base.Controls.Add(this.barDockControl1);
		base.Controls.Add(this.barDockControl7);
		base.Controls.Add(this.barDockControl8);
		base.Controls.Add(this.barDockControl6);
		base.Controls.Add(this.barDockControl5);
		base.Controls.Add(this.barDockControl11);
		base.Controls.Add(this.barDockControl12);
		base.Controls.Add(this.barDockControl10);
		base.Controls.Add(this.barDockControl9);
		base.Controls.Add(this.barDockControl15);
		base.Controls.Add(this.barDockControl16);
		base.Controls.Add(this.barDockControl14);
		base.Controls.Add(this.barDockControl13);
		base.Name = "TableUserControl";
		base.Size = new System.Drawing.Size(881, 640);
		base.Load += new System.EventHandler(TableUserControl_Load);
		base.Leave += new System.EventHandler(TableUserControl_Leave);
		((System.ComponentModel.ISupportInitialize)this.tableXtraTabControl).EndInit();
		this.tableXtraTabControl.ResumeLayout(false);
		this.tableDescriptionXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tableLayoutControl).EndInit();
		this.tableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.documentationTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.schemaTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableTitleTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableLocationTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.schemaLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.documentationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.modulesLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableLocationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		this.tableColumnListXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tableColumnsGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableColumnsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconTableColumnRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.keyTableColumnRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnFullNameRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleColumnRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.referencesRepositoryItemAutoHeightMemoEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionColumnRepositoryItemMemoEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataLinksRepositoryItemAutoHeightMemoEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.comutedRepositoryItemAutoHeightMemoEdit).EndInit();
		this.tableRelationsXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tableRelationsGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableRelationsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameRalationRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.relationTitleRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.FKTableObjectNameCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableRelationRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PKTableObjectNameCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.joinRepositoryItemAutoHeightMemoEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionRelationRepositoryItemMemoEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleRelationRepositoryItemTextEdit).EndInit();
		this.tableConstraintsXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tableConstraintsGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableConstraintsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnFullNameFormattedRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionConstraintRepositoryItemMemoEdit).EndInit();
		this.tableTriggersXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.triggersSplitContainerControl).EndInit();
		this.triggersSplitContainerControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tableTriggerGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableTriggerGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconTriggerRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionTriggerRepositoryItemMemoEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.onInsertRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.onUpdateRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.onDeleteRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).EndInit();
		this.layoutControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		this.dataLineageXtraTabPage.ResumeLayout(false);
		this.dependenciesXtraTabPage.ResumeLayout(false);
		this.tableScriptXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl1).EndInit();
		this.nonCustomizableLayoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).EndInit();
		this.tableSchemaXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.schemaTextLayoutControl).EndInit();
		this.schemaTextLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem11).EndInit();
		this.dataLinksXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dataLinksGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataLinksGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fullNameRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemAutoHeightMemoEdit1).EndInit();
		this.schemaImportsAndChangesXtraTabPage.ResumeLayout(false);
		this.metadataXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.metadataLayoutControl).EndInit();
		this.metadataLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsLastUpdatedLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lastImportedLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lastUpdatedLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsCreatedLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.firstImportedLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.treeMenuImageCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.relationNameErrorProvider).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tableTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.linkedTermsPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.linkedTermsBarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.uniqueKeysPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.uniqueKeysBarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.triggersPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.triggersBarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.relationsPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.relationsBarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnsPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnsBarManager).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
