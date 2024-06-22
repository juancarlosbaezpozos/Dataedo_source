using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.Documentation;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.History;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
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
using Dataedo.App.UserControls.SchemaImportsAndChanges;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Data.History;
using Dataedo.Model.Data.Modules;
using Dataedo.Model.Data.Procedures.Procedures;
using Dataedo.Shared.Enums;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
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
using DevExpress.XtraRichEdit;
using DevExpress.XtraTab;

namespace Dataedo.App.UserControls.PanelControls;

public class ProcedureUserControl : BasePanelControl, ITabSettable, IPanelWithDataLineage
{
	private bool isParametersDataLoaded;

	private bool columnsLoadRequired;

	private string name;

	private string title;

	private string schema;

	private int procedureId;

	private int? moduleId;

	private bool isModuleEdited;

	private int? databaseId;

	private SharedDatabaseTypeEnum.DatabaseType? databaseType;

	private IdEventArgs idEventArgs;

	private string databaseName;

	private string databaseTitle;

	private string databaseServer;

	private bool showDatabaseTitle;

	private UserTypeEnum.UserType? source;

	private bool _isProcedureEdited;

	private BindingList<int> deletedParametersRows = new BindingList<int>();

	private Dictionary<SharedObjectTypeEnum.ObjectType, GridView> customFieldsSettings = new Dictionary<SharedObjectTypeEnum.ObjectType, GridView>();

	private bool hasLoadedSuggestedDescriptions;

	private CustomFieldsCellsTypesSupport customFieldsCellsTypesSupportForGrids;

	private bool isModuleDropdownLoaded;

	private Dictionary<string, string> objectCustomFieldsForHistory = new Dictionary<string, string>();

	private ObjectWithTitleAndHTMLDescriptionHistory objectTitleAndDescriptionHistory = new ObjectWithTitleAndHTMLDescriptionHistory();

	private Dictionary<int, Dictionary<string, string>> customFieldsParametersForHistory = new Dictionary<int, Dictionary<string, string>>();

	private Dictionary<int, ObjectWithTitleAndDescription> titleAndDescriptionParametersForHistory = new Dictionary<int, ObjectWithTitleAndDescription>();

	private IContainer components;

	private XtraTabControl procedureXtraTabControl;

	private XtraTabPage procedureDescriptionXtraTabPage;

	private HtmlUserControl procedureHtmlUserControl;

	private NonCustomizableLayoutControl procedureLayoutControl;

	private TextEdit databaseHostTextEdit;

	private TextEdit procedureTitleTextEdit;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem titleLayoutControlItem;

	private LayoutControlItem layoutControlItem2;

	private XtraTabPage procedureScriptXtraTabPage;

	private TextEdit textEdit1;

	private LayoutControlItem layoutControlItem3;

	private XtraTabPage procedureParametersXtraTabPage;

	private GridControl procedureParametersGrid;

	private GridColumn nameProcedureParametersGridColumn;

	private GridColumn descriptionProcedureParametersGridColumn;

	private GridColumn parameterModeProcedureParametersGridColumn;

	private GridColumn datatypeProcedureParametersGridColumn;

	private EmptySpaceItem emptySpaceItem2;

	private GridColumn ordinalPositionProcedureParametersGridColumn;

	private RepositoryItemPictureEdit iconFunctionParamterRepositoryItemPictureEdit;

	private GridColumn iconProcedureParametersGridColumn;

	private ToolTipController procedureToolTipController;

	private EmptySpaceItem emptySpaceItem1;

	private InfoUserControl procedureStatusUserControl;

	private RepositoryItemAutoHeightMemoEdit descriptionInputOutputRepositoryItemMemoEdit;

	private XtraTabPage procedureMetadataXtraTabPage;

	private NonCustomizableLayoutControl metadataLayoutControl;

	private LayoutControlGroup layoutControlGroup2;

	private LabelControl dbmsLastUpdatedLabel;

	private LayoutControlItem dbmsLastUpdatedLayoutControlItem;

	private LabelControl lastSynchronizedLabel;

	private LayoutControlItem lastImportedLayoutControlItem;

	private EmptySpaceItem emptySpaceItem3;

	private LabelControl lastUpdatedLabel;

	private LayoutControlItem lastUpdatedLayoutControlItem;

	private XtraTabPage dependenciesXtraTabPage;

	private DependenciesUserControl dependenciesUserControl;

	private RichEditUserControl scriptRichEditControl;

	private CustomFieldsPanelControl customFieldsPanelControl;

	private LayoutControlItem customFieldsLayoutControlItem;

	private LayoutControlItem layoutControlItem12;

	private BulkCopyGridUserControl procedureParametersGridView;

	private TextEdit nameTextEdit;

	private LayoutControlItem nameLayoutControlItem;

	private TextEdit procedureTextEdit;

	private GridPanelUserControl parametersGridPanelUserControl;

	private TextEdit schemaTextEdit;

	private TextEdit documentationTextEdit;

	private LayoutControlItem documentationLayoutControlItem;

	private LayoutControlItem schemaLayoutControlItem;

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

	private LabelControl searchCountLabelControl;

	private LabelControl descriptionLabelControl;

	private LayoutControlItem layoutControlItem4;

	private LayoutControlItem layoutControlItem5;

	private LabelControl scriptSearchCountLabelControl;

	private LabelControl labelControl2;

	private LayoutControlGroup layoutControlGroup3;

	private LayoutControlItem layoutControlItem6;

	private LayoutControlItem layoutControlItem7;

	private LayoutControlItem layoutControlItem8;

	private NonCustomizableLayoutControl layoutControl1;

	private PopupMenu parametersPopupMenu;

	private BarButtonItem removeParametersParemetersBarButtonItem;

	private BarManager parametersBarManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ToolTipController metadataToolTipController;

	private ToolTipController toolTipController;

	private BarButtonItem designProcedureBarButtonItem;

	private BarButtonItem designFunctionBarButtonItem;

	private XtraTabPage dataLineageXtraTabPage;

	private DataLineageUserControl dataLineageUserControl;

	private BarButtonItem viewHistoryBarButtonItem;

	public override int DatabaseId => databaseId ?? (-1);

	public override int ObjectModuleId => moduleId ?? (-1);

	public override int ObjectId => procedureId;

	public override string ObjectSchema => schema;

	public override string ObjectName => name;

	public override HtmlUserControl DescriptionHtmlUserControl => procedureHtmlUserControl;

	public override XtraTabPage SchemaImportsAndChangesXtraTabPage => schemaImportsAndChangesXtraTabPage;

	public override SchemaImportsAndChangesUserControl SchemaImportsAndChangesUserControl => schemaImportsAndChangesUserControl;

	public override CustomFieldsPanelControl CustomFieldsPanelControl => customFieldsPanelControl;

	public bool isProcedureEdited
	{
		get
		{
			return _isProcedureEdited;
		}
		set
		{
			if (!base.DisableSettingAsEdited && _isProcedureEdited != value)
			{
				_isProcedureEdited = value;
				SetTabPageTitle(_isProcedureEdited, procedureDescriptionXtraTabPage, base.Edit);
			}
		}
	}

	public BindingList<ParameterRow> ParameterRows => procedureParametersGrid.DataSource as BindingList<ParameterRow>;

	public SuggestedDescriptionManager SuggestedDescriptionManager { get; private set; }

	public override BaseView VisibleGridView => procedureParametersGridView;

	public event EventHandler AddModuleEvent;

	public ProcedureUserControl(MetadataEditorUserControl control)
		: base(control)
	{
		InitializeComponent();
		Initialize();
		MetadataToolTip.SetLayoutControlItemToolTip(dbmsCreatedLayoutControlItem, "dbms_created");
		MetadataToolTip.SetLayoutControlItemToolTip(dbmsLastUpdatedLayoutControlItem, "dbms_last_updated");
		MetadataToolTip.SetLayoutControlItemToolTip(firstImportedLayoutControlItem, "first_imported");
		MetadataToolTip.SetLayoutControlItemToolTip(lastImportedLayoutControlItem, "last_imported");
		MetadataToolTip.SetLayoutControlItemToolTip(lastUpdatedLayoutControlItem, "last_updated");
		MetadataToolTip.SetColumnToolTip(createdGridColumn, "first_imported");
		MetadataToolTip.SetColumnToolTip(createdByGridColumn, "first_imported_by");
		MetadataToolTip.SetColumnToolTip(lastUpdatedGridColumn, "last_updated");
		MetadataToolTip.SetColumnToolTip(lastUpdatedByGridColumn, "last_updated_by");
		suggestedValuesContextMenu = new PopupMenu
		{
			Manager = new BarManager(),
			DrawMenuSideStrip = DefaultBoolean.False
		};
		procedureHtmlUserControl.ContentChangedEvent += procedureHtmlUserControl_PreviewKeyDown;
		procedureHtmlUserControl.ProgressValueChanged += SetRichEditControlBackground;
		procedureHtmlUserControl.IsEditorFocused += SetRichEditControlBackgroundWhileFocused;
		base.Edit = new Edit(procedureTextEdit);
		idEventArgs = new IdEventArgs();
		documentationModulesUserControl.LoadData(idEventArgs);
		LengthValidation.SetTitleOrNameLengthLimit(procedureTitleTextEdit);
		base.UserControlHelpers = new UserControlHelpers(1);
		dependenciesUserControl.DependeciesChanged += delegate
		{
			RefreshDependencies(forceRefresh: true, refreshImmediatelyIfLoaded: true);
		};
		dataLineageUserControl.DataLineageEdited += DataLineageUserControl_DataLineageEdited;
		procedureParametersGridView.SetRowCellValue(-2147483646, "iconProcedureParametersGridColumn", Resources.blank_16);
		customFieldsCellsTypesSupportForGrids = new CustomFieldsCellsTypesSupport(isForSummaryTable: false);
		SchemaImportsAndChangesSupport = new SchemaImportsAndChangesSupport(this);
		customFieldsSettings = new Dictionary<SharedObjectTypeEnum.ObjectType, GridView> { [SharedObjectTypeEnum.ObjectType.Parameter] = procedureParametersGridView };
		procedureStatusUserControl.SetShouldLoadColorsAfterLoad(shouldLoadColorsAfterLoad: false);
	}

	private void ProcedureUserControl_Load(object sender, EventArgs e)
	{
		suggestedValuesContextMenu.Manager.Form = base.ParentForm;
		AddEvents();
		WorkWithDataedoTrackingHelper.TrackFirstInSessionObjectView();
	}

	private void highlightGridView_RowCellStyle(object sender, RowCellCustomDrawEventArgs e)
	{
	}

	public override void SetTabsProgressHighlights()
	{
		base.UserControlHelpers.ClearTabHighlights(procedureXtraTabControl);
		if (base.MainControl.ShowProgress && base.MainControl.ProgressType.Type != ProgressTypeEnum.TablesAndColumns)
		{
			CreateKeyValuePairs();
			base.UserControlHelpers.SetTabsProgressHighlights(procedureXtraTabControl, base.KeyValuePairs);
		}
	}

	private void CreateKeyValuePairs()
	{
		base.KeyValuePairs.Clear();
		foreach (XtraTabPage tabPage in procedureXtraTabControl.TabPages)
		{
			int key = procedureXtraTabControl.TabPages.IndexOf(tabPage);
			if (tabPage.Equals(procedureDescriptionXtraTabPage))
			{
				KeyValuePair<int, int> value = new KeyValuePair<int, int>(base.CurrentNode.ObjectPoints, base.CurrentNode.TotalObjectPoints);
				base.KeyValuePairs.Add(key, value);
			}
			else if (tabPage.Equals(procedureParametersXtraTabPage))
			{
				KeyValuePair<int, int> value = new KeyValuePair<int, int>(base.CurrentNode.ParametersPoints, base.CurrentNode.TotalParametersPoints);
				base.KeyValuePairs.Add(key, value);
			}
		}
	}

	public void ClearHighlights(bool keepSearchActive)
	{
		base.UserControlHelpers.ClearHighlights(keepSearchActive, procedureXtraTabControl, schemaTextEdit, nameTextEdit, procedureTitleTextEdit, customFieldsPanelControl.FieldControls);
		procedureHtmlUserControl.ClearHighlights();
		scriptRichEditControl.ClearHighlights();
		searchCountLabelControl.Text = string.Empty;
		searchCountLabelControl.Appearance.BackColor = SkinColors.ControlColorFromSystemColors;
		scriptSearchCountLabelControl.Text = string.Empty;
		scriptSearchCountLabelControl.Appearance.BackColor = SkinColors.ControlColorFromSystemColors;
	}

	public void ForceLayoutChange(bool forceAll = false)
	{
		if (!forceAll)
		{
			if (procedureXtraTabControl.SelectedTabPageIndex == 1)
			{
				procedureParametersGridView.LayoutChanged();
			}
		}
		else
		{
			procedureParametersGridView.LayoutChanged();
		}
	}

	public void SetTab(ResultItem row, SharedObjectTypeEnum.ObjectType? type, bool changeTab, string[] searchWords, List<CustomFieldSearchItem> customFieldSearchItems, params int?[] elementId)
	{
		int num = 0;
		if (!type.HasValue)
		{
			num = 0;
			base.UserControlHelpers.SetHighlight(row, searchWords, customFieldSearchItems, null, num, procedureXtraTabControl.TabPages.IndexOf(procedureScriptXtraTabPage), null, procedureXtraTabControl, schemaTextEdit, nameTextEdit, procedureTitleTextEdit, customFieldsPanelControl.FieldControls, procedureHtmlUserControl, scriptRichEditControl, null, null, null);
			searchCountLabelControl.Text = procedureHtmlUserControl.Occurrences;
			BaseSkin.SetSearchHighlightOrDefault(searchCountLabelControl, procedureHtmlUserControl.OccurrencesCount > 0);
			scriptSearchCountLabelControl.Text = scriptRichEditControl.Occurrences;
			BaseSkin.SetSearchHighlightOrDefault(scriptSearchCountLabelControl, scriptRichEditControl.OccurrencesCount > 0);
		}
		else if (type.Value == SharedObjectTypeEnum.ObjectType.Parameter)
		{
			num = procedureXtraTabControl.TabPages.IndexOf(procedureParametersXtraTabPage);
			base.UserControlHelpers.SetHighlight(searchWords, customFieldSearchItems, 0, procedureParametersGridView, procedureXtraTabControl, num, "parameter_id", elementId);
		}
		if (changeTab)
		{
			procedureXtraTabControl.SelectedTabPageIndex = num;
		}
	}

	private void SetFunctionality()
	{
		SchemaImportsAndChangesUserControl.SetFunctionality();
	}

	private void AddEvents()
	{
		AddEventsForDeleting(SharedObjectTypeEnum.ObjectType.Parameter, procedureParametersGridView, procedureParametersXtraTabPage, base.Edit, parametersPopupMenu, removeParametersParemetersBarButtonItem, isObject: true, deletedParametersRows);
		List<ToolTipData> list = new List<ToolTipData>();
		list.Add(new ToolTipData(procedureParametersGrid, SharedObjectTypeEnum.ObjectType.Parameter, iconProcedureParametersGridColumn.VisibleIndex));
		CommonFunctionsPanels.AddEventsForToolTips(procedureToolTipController, list);
		documentationModulesUserControl.AddModule = this.AddModuleEvent;
		MetadataEditorUserControl mainControl = base.MainControl;
		mainControl.GetSuggestedDescriptions = (EventHandler)Delegate.Combine(mainControl.GetSuggestedDescriptions, new EventHandler(LoadSuggestedDescriptions));
		procedureXtraTabControl.SelectedPageChanged += delegate
		{
			DependencyButtonsVisibleChanged(null, null);
		};
		procedureXtraTabControl.SelectedPageChanged += delegate
		{
			DataLineageButtonsVisibleChanged(null, null);
		};
		dependenciesUserControl.AddEvents(base.MainControl);
		CommonFunctionsPanels.AddEventForAutoFilterRow(procedureParametersGridView);
	}

	private void procedureHtmlUserControl_PreviewKeyDown(object sender, EventArgs e)
	{
		isProcedureEdited = true;
	}

	private void procedureTitleTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		isProcedureEdited = true;
	}

	private void procedureTypeLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		isProcedureEdited = true;
	}

	private void procedureModuleLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		isProcedureEdited = true;
		isModuleEdited = true;
	}

	private void dependenciesUserControl_DependencyChanging()
	{
		SetTabPageTitle(isEdited: true, dependenciesXtraTabPage, base.Edit);
	}

	private void DataLineageUserControl_DataLineageEdited()
	{
		SetTabPageTitle(isEdited: true, dataLineageXtraTabPage, base.Edit);
	}

	public void SaveColumns()
	{
		if (ObjectType != SharedObjectTypeEnum.ObjectType.UnresolvedEntity)
		{
			UserViewData.SaveColumns(UserViewData.GetViewName(this, procedureParametersGridView, ObjectType), procedureParametersGridView);
		}
	}

	public override void SetParameters(DBTreeNode selectedNode, CustomFieldsSupport customFieldsSupport, Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType? dependencyType = null)
	{
		base.SetParameters(selectedNode, customFieldsSupport, dependencyType);
		try
		{
			procedureParametersGridView.BeginUpdate();
			columnsLoadRequired = false;
			if (ObjectType != selectedNode.ObjectType)
			{
				SaveColumns();
				columnsLoadRequired = true;
			}
			ObjectType = selectedNode.ObjectType;
			isProcedureEdited = false;
			base.DisableSettingAsEdited = true;
			if (ObjectType == SharedObjectTypeEnum.ObjectType.Procedure)
			{
				designProcedureBarButtonItem.Visibility = BarItemVisibility.Always;
				designFunctionBarButtonItem.Visibility = BarItemVisibility.Never;
			}
			else if (ObjectType == SharedObjectTypeEnum.ObjectType.Function)
			{
				designProcedureBarButtonItem.Visibility = BarItemVisibility.Never;
				designFunctionBarButtonItem.Visibility = BarItemVisibility.Always;
			}
			else
			{
				designProcedureBarButtonItem.Visibility = BarItemVisibility.Never;
				designFunctionBarButtonItem.Visibility = BarItemVisibility.Never;
			}
			showDatabaseTitle = selectedNode?.ParentNode?.ParentNode != null && selectedNode.ParentNode.ParentNode.HasMultipleDatabases;
			procedureHtmlUserControl.CanListen = false;
			base.TabPageChangedProgrammatically = true;
			CommonFunctionsPanels.SetSelectedTabPage(procedureXtraTabControl, dependencyType);
			base.TabPageChangedProgrammatically = false;
			ShowDependencyButtons();
			procedureId = selectedNode.Id;
			moduleId = ((selectedNode == null || selectedNode.ParentNode?.ParentNode.ObjectType != SharedObjectTypeEnum.ObjectType.Module) ? null : selectedNode?.ParentNode?.ParentNode.Id);
			ProcedureObject dataById = DB.Procedure.GetDataById(procedureId);
			ClearData(selectedNode);
			base.DisableSettingAsEdited = true;
			if (dataById != null)
			{
				SetProcedureBasicData(dataById);
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
				CommonFunctionsPanels.SetName(procedureTextEdit, procedureDescriptionXtraTabPage, ObjectType, base.Subtype, schema, name, procedureTitleTextEdit.Text, databaseType, databaseTitle, showDatabaseTitle, base.ShowSchema, (!source.HasValue) ? UserTypeEnum.UserType.DBMS : source.Value);
				base.ModulesId = DB.Procedure.GetProcedureModules(procedureId);
				isModuleDropdownLoaded = false;
				documentationModulesUserControl.ClearData();
				List<ModuleTitleWithDatabaseTitle> objectsTop5ModulesNamesWithDatabase = DB.Procedure.GetObjectsTop5ModulesNamesWithDatabase(procedureId);
				documentationModulesUserControl.SetPopupBarText(string.Join(", ", objectsTop5ModulesNamesWithDatabase.Select((ModuleTitleWithDatabaseTitle m) => m.ModuleTitleWithDatabase)));
				documentationModulesUserControl.Refresh();
				isModuleEdited = false;
				base.CustomFieldsSupport = customFieldsSupport;
				customFields = new CustomFieldContainer(ObjectType, ObjectId, customFieldsSupport);
				customFields.RetrieveCustomFields(dataById);
				customFields.ClearAddedDefinitionValues(null);
				SetCustomFieldsDataSource();
				procedureHtmlUserControl.HtmlText = PrepareValue.ToString(dataById.Description);
				dbmsCreatedLabelControl.Text = PrepareValue.SetDateTimeWithFormatting(dataById.DbmsCreationDate);
				createdLabelControl.Text = PrepareValue.SetDateTimeWithFormatting(dataById.CreationDate) + " " + PrepareValue.ToString(dataById.CreatedBy);
				dbmsLastUpdatedLabel.Text = PrepareValue.SetDateTimeWithFormatting(dataById.DbmsLastModificationDate);
				lastSynchronizedLabel.Text = PrepareValue.SetDateTimeWithFormatting(dataById.SynchronizationDate) + " " + PrepareValue.ToString(dataById.SynchronizedBy);
				lastUpdatedLabel.Text = PrepareValue.SetDateTimeWithFormatting(dataById.LastModificationDate) + " " + PrepareValue.ToString(dataById.ModifiedBy);
				parametersGridPanelUserControl.Initialize(procedureParametersXtraTabPage.Text);
				parametersGridPanelUserControl.CustomFields += base.EditCustomFieldsFromGridPanel;
				parametersGridPanelUserControl.SetDefaultLockButtonVisibility(value: true);
				hasLoadedSuggestedDescriptions = false;
				RefreshDefinition(dataById);
				RefreshParameters(forceRefresh: true);
				RefreshDependencies(forceRefresh: true);
				RefreshDataLineage(forceRefresh: true);
				schemaImportsAndChangesUserControl.ClearData();
				RefreshSchemaImportsAndChanges(forceRefresh: true);
				isProcedureEdited = false;
				CommonFunctionsPanels.ClearTabPagesTitle(procedureXtraTabControl, base.Edit);
				procedureStatusUserControl.CheckAndSetDeletedObjectProperties(dataById);
				objectCustomFieldsForHistory = HistoryCustomFieldsHelper.GetOldCustomFieldsInObjectUserControl(customFields);
				SaveProcedureTitleAndDescription(dataById);
			}
			else
			{
				procedureStatusUserControl.SetDeletedObjectProperties();
			}
			token = new CancellationTokenSource();
			SetFunctionality();
			FillControlProgressHighlights();
			SetRichEditControlBackground();
			procedureStatusUserControl.Description = "This " + SharedObjectTypeEnum.TypeToString(ObjectType).ToLower() + " has been removed from the database. You can remove it from the repository.";
			documentationLayoutControlItem.OptionsToolTip.ToolTip = "Database/data source in the Dataedo metadata repository ";
			schemaLayoutControlItem.OptionsToolTip.ToolTip = CommonTooltips.GetSchema(ObjectType);
			nameLayoutControlItem.OptionsToolTip.ToolTip = CommonTooltips.GetName(ObjectType);
			titleLayoutControlItem.OptionsToolTip.ToolTip = CommonTooltips.GetTitle(ObjectType);
			descriptionLabelControl.ToolTip = CommonTooltips.GetDescription(ObjectType);
			SetTabsProgressHighlights();
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			procedureParametersGridView.EndUpdate();
			base.DisableSettingAsEdited = false;
		}
	}

	private void LoadUserColumns()
	{
		if (columnsLoadRequired && isParametersDataLoaded)
		{
			if (!UserViewData.LoadColumns(UserViewData.GetViewName(this, procedureParametersGridView, ObjectType), procedureParametersGridView))
			{
				CustomFieldsCellsTypesSupport.SortCustomColumns(procedureParametersGridView);
				CommonFunctionsPanels.SetBestFitForColumns(procedureParametersGridView);
			}
			columnsLoadRequired = false;
		}
	}

	private void SetProcedureBasicData(ProcedureObject procedureRow)
	{
		base.Subtype = SharedObjectSubtypeEnum.StringToType(SharedObjectTypeEnum.ObjectType.Procedure, procedureRow.Subtype);
		databaseId = procedureRow.DatabaseId;
		idEventArgs.DatabaseId = databaseId.Value;
		schemaTextEdit.Text = procedureRow.Schema;
		string text2 = (title = (procedureTitleTextEdit.Text = procedureRow.Title));
		text2 = (name = (nameTextEdit.Text = procedureRow.Name));
		schema = procedureRow.Schema;
		source = UserTypeEnum.ObjectToType(procedureRow.Source);
	}

	protected override void Redraw()
	{
		if (procedureParametersGrid != null)
		{
			procedureParametersGrid.Refresh();
		}
	}

	private void RefreshParameters(bool forceRefresh = false)
	{
		if (IsRefreshRequired(procedureParametersXtraTabPage, !isParametersDataLoaded, forceRefresh))
		{
			base.MainControl.SetWaitformVisibility(visible: true);
			BindingList<ParameterRow> dataObjectByProcedureId = DB.Parameter.GetDataObjectByProcedureId(procedureId, notDeletedOnly: false, base.CustomFieldsSupport);
			procedureParametersGridView.LeftCoord = 0;
			procedureParametersGrid.DataSource = dataObjectByProcedureId;
			BasicRow[] elements = ParameterRows?.ToArray();
			HistoryColumnsHelper.SaveColumnsOrParametrsOfOldCustomFields(elements, customFieldsParametersForHistory, titleAndDescriptionParametersForHistory);
			if (procedureXtraTabControl.SelectedTabPage == procedureParametersXtraTabPage)
			{
				foreach (ParameterRow parameterRow in ParameterRows)
				{
					parameterRow.SuggestedDescriptions.Clear();
				}
				SuggestedDescriptionManager = new SuggestedDescriptionManager(base.ContextShowSchema, procedureId, GetParameterFieldNames(), "parameters", "procedure_id", ParameterRows?.ToList().Cast<BaseRow>().ToList(), procedureParametersGridView, SharedObjectTypeEnum.ObjectType.Function, token);
				SuggestedDescriptionManager suggestedDescriptionManager = SuggestedDescriptionManager;
				suggestedDescriptionManager.Redraw = (Action)Delegate.Combine(suggestedDescriptionManager.Redraw, new Action(Redraw));
				SuggestedDescriptionManager.Rows = ParameterRows?.ToList().Cast<BaseRow>().ToList();
				if (base.ShowHints)
				{
					SuggestedDescriptionManager.GetSuggestedDescriptions();
					hasLoadedSuggestedDescriptions = base.ShowHints;
				}
			}
			FillHighlights(SharedObjectTypeEnum.ObjectType.Parameter);
			isParametersDataLoaded = true;
			LoadUserColumns();
			base.MainControl.SetWaitformVisibility(visible: false);
		}
		else if (forceRefresh)
		{
			isParametersDataLoaded = false;
		}
	}

	private void RefreshDependencies(bool forceRefresh = false, bool refreshImmediatelyIfLoaded = false, bool refreshImmediatelyIfNotLoaded = false)
	{
		bool flag = dependenciesUserControl.DependencyObjectEquals(procedureId, ObjectType);
		if ((IsRefreshRequired(dependenciesXtraTabPage, !flag, forceRefresh) || (refreshImmediatelyIfNotLoaded && !flag) || (refreshImmediatelyIfLoaded && flag)) && databaseId.HasValue)
		{
			base.MainControl.SetWaitformVisibility(visible: true);
			dependenciesUserControl.SetParameters(databaseId.Value, databaseServer, databaseName, databaseTitle, base.HasMultipleSchemas, base.DatabaseShowSchema, base.DatabaseShowSchemaOverride, base.ContextShowSchema, schema, name, title, UserTypeEnum.TypeToString(UserTypeEnum.UserType.DBMS), ObjectType, base.Subtype, databaseType, procedureId, base.MainControl);
			base.MainControl.SetWaitformVisibility(visible: false);
		}
	}

	public void RefreshDataLineage(bool forceRefresh = false, bool refreshImmediatelyIfLoaded = false, bool refreshImmediatelyIfNotLoaded = false)
	{
		bool flag = dataLineageUserControl.CheckIfObjectEquals(procedureId, ObjectType);
		if ((IsRefreshRequired(dataLineageXtraTabPage, !flag, forceRefresh) || (refreshImmediatelyIfNotLoaded && !flag) || (refreshImmediatelyIfLoaded && flag)) && databaseId.HasValue)
		{
			base.MainControl.SetWaitformVisibility(visible: true);
			dataLineageUserControl.SetParameters(base.MainControl, base.CurrentNode);
			base.MainControl.SetWaitformVisibility(visible: false);
		}
	}

	private void RefreshDefinition(ProcedureObject procedureRow)
	{
		string text = PrepareValue.ToString(procedureRow.Definition);
		if (string.IsNullOrEmpty(text))
		{
			scriptRichEditControl.Text = GetEmptyScriptMessage(databaseType, source);
			scriptRichEditControl.SetOriginalHtmlText();
		}
		else if (databaseType == SharedDatabaseTypeEnum.DatabaseType.Cassandra || databaseType == SharedDatabaseTypeEnum.DatabaseType.CosmosDbCassandra)
		{
			string cassandraDefinition = GetCassandraDefinition(procedureRow, text);
			ColorizeSyntax(scriptRichEditControl, cassandraDefinition, "SQL");
		}
		else
		{
			ColorizeSyntax(scriptRichEditControl, text, "SQL");
		}
		scriptRichEditControl.RefreshSkin();
	}

	private string GetCassandraDefinition(ProcedureObject procedureRow, string definition)
	{
		string text = "CREATE FUNCTION " + schema + "." + name;
		BindingList<ParameterRow> dataObjectByProcedureId = DB.Parameter.GetDataObjectByProcedureId(procedureId, notDeletedOnly: false, base.CustomFieldsSupport);
		string text2 = " (";
		foreach (ParameterRow item in dataObjectByProcedureId.Where((ParameterRow x) => x.Mode == ParameterRow.ModeEnum.In))
		{
			text2 = text2 + item.DataType + " " + item.Name + ", ";
		}
		text2 = text2.Substring(0, text2.Length - 2);
		text2 = text2 + ")" + Environment.NewLine;
		string text3 = null;
		foreach (ParameterRow item2 in dataObjectByProcedureId.Where((ParameterRow x) => x.Mode == ParameterRow.ModeEnum.Out))
		{
			text3 = text3 + item2.Name.ToUpper() + " " + item2.DataType;
		}
		text3 += Environment.NewLine;
		string text4 = "LANGUAGE " + procedureRow.Language + " AS" + Environment.NewLine;
		string text5 = "'" + definition + "'";
		return text + text2 + text3 + text4 + text5;
	}

	private bool IsRefreshRequired(XtraTabPage tabPage, bool additionalCondition = true, bool forceRefresh = false)
	{
		if (!forceRefresh || procedureXtraTabControl.SelectedTabPage != tabPage)
		{
			if (procedureXtraTabControl.SelectedTabPage == tabPage && additionalCondition)
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
			source = null;
			TextEdit textEdit = schemaTextEdit;
			TextEdit textEdit2 = procedureTitleTextEdit;
			TextEdit textEdit3 = nameTextEdit;
			HtmlUserControl htmlUserControl = procedureHtmlUserControl;
			LabelControl labelControl = lastSynchronizedLabel;
			RichEditUserControl richEditUserControl = scriptRichEditControl;
			string text2 = (dbmsLastUpdatedLabel.Text = null);
			string text4 = (richEditUserControl.Text = text2);
			string text6 = (labelControl.Text = text4);
			string text8 = (htmlUserControl.HtmlText = text6);
			string text10 = (textEdit3.Text = text8);
			string text13 = (textEdit.Text = (textEdit2.Text = text10));
			idEventArgs.DatabaseId = null;
			CommonFunctionsPanels.SetName(procedureTextEdit, procedureDescriptionXtraTabPage, ObjectType, selectedNode.Subtype, selectedNode.Schema, selectedNode.Name, selectedNode.Title, databaseType, databaseTitle, showDatabaseTitle, base.ShowSchema, (!selectedNode.Source.HasValue) ? UserTypeEnum.UserType.DBMS : selectedNode.Source.Value);
			ClearParametersTable();
			isProcedureEdited = false;
			CommonFunctionsPanels.ClearTabPagesTitle(procedureXtraTabControl, base.Edit);
			if (setDeletedObjectProperties)
			{
				procedureStatusUserControl.SetDeletedObjectProperties();
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

	private void ClearParametersTable()
	{
		isParametersDataLoaded = false;
		deletedParametersRows.Clear();
		procedureParametersGrid.DataSource = null;
	}

	private void SetCustomFieldsDataSource()
	{
		CustomFieldsPanelControl.EditValueChanging += delegate
		{
			SetCurrentTabPageTitle(isEdited: true, procedureDescriptionXtraTabPage);
		};
		customFieldsPanelControl.ShowHistoryClick -= CustomFieldsPanelControl_ShowHistoryClick;
		customFieldsPanelControl.ShowHistoryClick += CustomFieldsPanelControl_ShowHistoryClick;
		IEnumerable<CustomFieldDefinition> customFieldRows = customFields.CustomFieldsData.Where((CustomFieldDefinition x) => x.CustomField?.ProcedureVisibility ?? false);
		customFieldsPanelControl.LoadFields(customFieldRows, delegate
		{
			isProcedureEdited = true;
		}, customFieldsLayoutControlItem);
		customFieldsCellsTypesSupportForGrids.SetCustomFields(base.CustomFieldsSupport, customFieldsSettings);
	}

	protected override void LoadSuggestedDescriptions(object sender, EventArgs e)
	{
		if (SuggestedDescriptionManager != null)
		{
			if (!hasLoadedSuggestedDescriptions && base.ShowHints)
			{
				SuggestedDescriptionManager.GetSuggestedDescriptions();
				hasLoadedSuggestedDescriptions = true;
			}
			Redraw();
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			switch (keyData)
			{
			case Keys.F | Keys.Control:
				if (procedureHtmlUserControl.PerformFindActionIfFocused())
				{
					return true;
				}
				if (scriptRichEditControl.Focused)
				{
					return false;
				}
				break;
			case Keys.F2 | Keys.Shift:
			{
				GridView visibleGridControl = GetVisibleGridControl();
				if (visibleGridControl == null)
				{
					return false;
				}
				if (visibleGridControl.FocusedRowHandle >= 0)
				{
					ShowSuggestedDescriptionsContextMenu(procedureParametersGrid, SuggestedDescriptionManager);
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
		procedureParametersGridView.HideCustomization();
	}

	public override bool Save()
	{
		try
		{
			bool flag = false;
			if (base.Edit.IsEdited)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
				List<XtraTabPage> list = new List<XtraTabPage>();
				if (!Licenses.CheckRepositoryVersionAfterLogin())
				{
					flag = true;
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
					int num = procedureId;
					ObjectIdName[] checkedObjects;
					if (loadedFor.Item1 != objectType || loadedFor.Item2 != num)
					{
						checkedObjects = (from x in DB.Procedure.GetProcedureModules(procedureId)
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
					CommonFunctionsDatabase.CheckModulesBeforeSaving(databaseId, ObjectType, procedureId, ref checkedObjects, documentationModulesUserControl, GetSplashScreenManager(), FindForm());
					ProcedureRow procedureRow = new ProcedureRow(procedureId, procedureTitleTextEdit.Text, checkedObjects, PrepareValue.GetHtmlText(procedureHtmlUserControl.PlainText, procedureHtmlUserControl.HtmlText), procedureHtmlUserControl.PlainTextForSearch);
					procedureRow.ObjectType = ObjectType;
					procedureRow.CustomFields = customFields;
					customFieldsPanelControl.SetCustomFieldsValuesInRow(procedureRow);
					procedureRow.CustomFields.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
					if (DB.Procedure.Update(procedureRow, FindForm()))
					{
						procedureHtmlUserControl.SetHtmlTextAsOriginal();
						SaveHistory(procedureRow);
						isProcedureEdited = false;
						DBTreeMenu.RefeshNodeTitle(databaseId.Value, procedureTitleTextEdit.Text, ObjectType, name, schema, databaseType, base.HasMultipleSchemas ?? base.ShowSchema);
						CommonFunctionsPanels.SetName(procedureTextEdit, procedureDescriptionXtraTabPage, ObjectType, base.Subtype, schema, name, procedureTitleTextEdit.Text, databaseType, databaseTitle, showDatabaseTitle, base.ShowSchema, UserTypeEnum.UserType.DBMS);
						customFieldsPanelControl.UpdateDefinitionValues();
						DB.Community.InsertFollowingToRepository(ObjectType, ObjectId);
						WorkWithDataedoTrackingHelper.TrackFirstInSessionObjectEdit();
					}
					else
					{
						flag = true;
					}
					procedureParametersGridView.CloseEditor();
					if (CommonFunctionsDatabase.UpdateParameters(procedureParametersGrid, deletedParametersRows, databaseId, customFieldsParametersForHistory, titleAndDescriptionParametersForHistory))
					{
						SetTabPageTitle(isEdited: false, procedureParametersXtraTabPage);
					}
					else
					{
						list.Add(procedureParametersXtraTabPage);
						flag = true;
					}
					if (UpdateDependencies())
					{
						SetTabPageTitle(isEdited: false, dependenciesXtraTabPage);
					}
					else
					{
						list.Add(dependenciesXtraTabPage);
						flag = true;
					}
					if (UpdateDataLineage())
					{
						SetTabPageTitle(isEdited: false, dataLineageXtraTabPage);
					}
					else
					{
						list.Add(dataLineageXtraTabPage);
						flag = true;
					}
					if (SchemaImportsAndChangesSupport.UpdateSchemaImportsAndChangesComments(FindForm()))
					{
						SetTabPageTitle(isEdited: false, schemaImportsAndChangesXtraTabPage);
					}
					else
					{
						list.Add(schemaImportsAndChangesXtraTabPage);
						flag = true;
					}
					base.MainControl.RefreshObjectProgress(showWaitForm: false, ObjectId, ObjectType);
					if (!flag)
					{
						base.Edit.SetUnchanged();
						RefreshModules();
						ManageObjectsInMenu(databaseId ?? (-1), checkedValues, procedureId, schema, name, title, source ?? UserTypeEnum.UserType.DBMS, checkedObjects);
						if (base.UserControlHelpers.IsSearchActive)
						{
							base.MainControl.OpenCurrentlySelectedSearchRow();
							procedureHtmlUserControl.SetNotChanged();
							if (procedureHtmlUserControl.Highlight())
							{
								base.UserControlHelpers.SetHighlight();
								searchCountLabelControl.Text = procedureHtmlUserControl.Occurrences;
							}
							ForceLostFocus();
						}
					}
					else
					{
						base.Edit.SetEdited();
						foreach (XtraTabPage item in list)
						{
							SetTabPageTitle(isEdited: true, item);
						}
					}
					FillControlProgressHighlights();
				}
			}
			ProcedureObject dataById = DB.Procedure.GetDataById(procedureId);
			dbmsCreatedLabelControl.Text = PrepareValue.SetDateTimeWithFormatting(dataById.DbmsCreationDate);
			createdLabelControl.Text = PrepareValue.SetDateTimeWithFormatting(dataById.CreationDate) + " " + PrepareValue.ToString(dataById.CreatedBy);
			dbmsLastUpdatedLabel.Text = PrepareValue.SetDateTimeWithFormatting(dataById.DbmsLastModificationDate);
			lastSynchronizedLabel.Text = PrepareValue.SetDateTimeWithFormatting(dataById.SynchronizationDate) + " " + PrepareValue.ToString(dataById.SynchronizedBy);
			lastUpdatedLabel.Text = PrepareValue.SetDateTimeWithFormatting(dataById.LastModificationDate) + " " + PrepareValue.ToString(dataById.ModifiedBy);
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			return !flag;
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
			base.TreeMenu.RefreshAllModulesAndSelect(databaseId.Value, ObjectType, moduleId, procedureId);
			isModuleEdited = false;
		}
	}

	private bool UpdateDependencies()
	{
		return UpdateDependencies(databaseId.Value, dependenciesUserControl);
	}

	private bool UpdateDataLineage()
	{
		if (dataLineageUserControl.IsInitialized)
		{
			return dataLineageUserControl.SaveChanges();
		}
		return true;
	}

	public void ProcessDependencyDelete()
	{
		dependenciesUserControl.DeleteDependency();
	}

	private void procedureXtraTabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		if (procedureXtraTabControl.SelectedTabPageIndex == 0)
		{
			SelectedTab.selectedTabCaption = "info";
		}
		else
		{
			SelectedTab.selectedTabCaption = procedureXtraTabControl.SelectedTabPage.Text;
		}
		RefreshParameters();
		RefreshDependencies();
		RefreshDataLineage();
		RefreshSchemaImportsAndChanges();
		HideCustomization();
	}

	private void RefreshSchemaImportsAndChanges(bool forceRefresh = false, bool refreshImmediatelyIfLoaded = false, bool refreshImmediatelyIfNotLoaded = false)
	{
		if (IsRefreshRequired(schemaImportsAndChangesXtraTabPage, additionalCondition: true, forceRefresh) || refreshImmediatelyIfNotLoaded || refreshImmediatelyIfLoaded)
		{
			schemaImportsAndChangesUserControl.RefreshImports();
		}
	}

	private void DependencyButtonsVisibleChanged(object sender, EventArgs e)
	{
		ShowDependencyButtons();
	}

	private void procedureParametersGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
		parametersPopupMenu.ShowPopupMenu(sender, e, inRowCellOnly: false);
	}

	private void procedureTablesGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
	}

	private void ShowDependencyButtons()
	{
		bool value = procedureXtraTabControl.SelectedTabPage == dependenciesXtraTabPage && dependenciesUserControl.CanFocusedNodeBeDeleted();
		base.MainControl.SetRemoveDependencyButtonVisibility(new BoolEventArgs(value));
	}

	private void procedureParametersGridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		Icons.SetIcon(e, iconProcedureParametersGridColumn, SharedObjectTypeEnum.ObjectType.Parameter);
	}

	private void documentationModulesUserControl1_EditValueChanged(object sender, EventArgs e)
	{
		isProcedureEdited = isProcedureEdited || documentationModulesUserControl.IsChanged;
		isModuleEdited = isModuleEdited || documentationModulesUserControl.IsChanged;
		documentationModulesUserControl.IsChanged = false;
	}

	private void procedureToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		SetSuggestedDescriptionTooltips(procedureParametersGridView, e);
	}

	private GridView GetVisibleGridControl()
	{
		if (procedureXtraTabControl.SelectedTabPage == procedureParametersXtraTabPage)
		{
			return procedureParametersGridView;
		}
		return null;
	}

	private void procedureParametersGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		int? num = null;
		ProgressPainter.SetEmptyProgressCellsBackground(e, base.progressType, base.MainControl.ShowProgress);
		if (sender.Equals(procedureParametersGridView))
		{
			num = 0;
		}
		base.UserControlHelpers.HighlightRowCellStyle(num, sender, e);
		if (base.ShowHints)
		{
			procedureParametersGrid.Invoke((Action)delegate
			{
				CustomDrawGridCell(sender, e);
			});
		}
		if ((e.Column == parameterModeProcedureParametersGridColumn || e.Column == nameProcedureParametersGridColumn || e.Column == datatypeProcedureParametersGridColumn || e.Column == createdGridColumn || e.Column == createdByGridColumn || e.Column == lastUpdatedGridColumn || e.Column == lastUpdatedByGridColumn || e.Column == ordinalPositionProcedureParametersGridColumn) && (!procedureParametersGridView.IsFocusedView || !procedureParametersGridView.GetSelectedRows().Contains(e.RowHandle)))
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridNonEditableColumnsBackColor;
		}
	}

	private void procedureParametersGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (!procedureParametersGridView.OptionsView.ShowAutoFilterRow || procedureParametersGridView.FocusedRowHandle >= 0)
		{
			SetTabPageTitle(isEdited: true, procedureParametersXtraTabPage, base.Edit);
			(procedureParametersGridView.GetFocusedRow() as ParameterRow).SetModified();
		}
	}

	private void ProcedureUserControl_Leave(object sender, EventArgs e)
	{
		procedureParametersGridView.HideCustomization();
	}

	private void ProcedureParametersGridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		if (!procedureParametersGridView.OptionsView.ShowAutoFilterRow || procedureParametersGridView.FocusedRowHandle >= 0)
		{
			SetTabPageTitle(isEdited: true, procedureParametersXtraTabPage, base.Edit);
			(procedureParametersGridView.GetFocusedRow() as ParameterRow).SetModified();
		}
	}

	private void parametersPopupMenu_BeforePopup(object sender, CancelEventArgs e)
	{
		if (procedureParametersGridView.FocusedRowHandle < 0 || Grids.GetBeforePopupShouldCancel(sender))
		{
			e.Cancel = true;
			return;
		}
		parametersPopupMenu.ItemLinks.Clear();
		GridHitInfo beforePopupHitInfo = Grids.GetBeforePopupHitInfo(sender);
		bool beforePopupIsRowClicked = Grids.GetBeforePopupIsRowClicked(sender);
		procedureParametersGridView.GetFocusedRow();
		suggestedValuesContextMenu.ItemLinks.Clear();
		if (beforePopupIsRowClicked)
		{
			if (base.ShowHints && beforePopupHitInfo?.Column != null)
			{
				SuggestedDescriptionManager.CreateSuggestedDescriptionContextMenuItems(parametersPopupMenu, isSeparatorDrawn: true, base.ShowHints, beforePopupHitInfo.Column.AbsoluteIndex >= 5);
			}
			if (CommonFunctionsDatabase.AreRowsToDelete(procedureParametersGridView, isObject: true))
			{
				parametersPopupMenu.ItemLinks.Add(removeParametersParemetersBarButtonItem);
				parametersPopupMenu.StartGroupBeforeLastItem();
			}
			if (ObjectType == SharedObjectTypeEnum.ObjectType.Procedure)
			{
				parametersPopupMenu.ItemLinks.Add(designProcedureBarButtonItem);
			}
			else
			{
				parametersPopupMenu.ItemLinks.Add(designFunctionBarButtonItem);
			}
			if (beforePopupHitInfo.Column == descriptionProcedureParametersGridColumn)
			{
				viewHistoryBarButtonItem.Tag = "description";
				parametersPopupMenu.ItemLinks.Add(viewHistoryBarButtonItem);
			}
			if (beforePopupHitInfo.Column.FieldName.ToLower().StartsWith("Field".ToLower()))
			{
				viewHistoryBarButtonItem.Tag = beforePopupHitInfo.Column.FieldName;
				parametersPopupMenu.ItemLinks.Add(viewHistoryBarButtonItem);
			}
		}
		e.Cancel = parametersPopupMenu.ItemLinks.Count == 0;
	}

	private void procedureParametersGridView_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		if (e.Column == ordinalPositionProcedureParametersGridColumn)
		{
			e.Result = new OrdinalPositionComparer().Compare(e.Value1?.ToString(), e.Value2?.ToString());
			e.Handled = true;
		}
	}

	public void ClearTabPageTitle()
	{
		procedureDescriptionXtraTabPage.Text = CommonFunctionsPanels.SetTitle(isEdited: false, procedureDescriptionXtraTabPage.Text);
	}

	public void SetTitleTextEdit(string title)
	{
		procedureTitleTextEdit.Text = title;
		objectTitleAndDescriptionHistory.Title = title;
	}

	public void SetLabels(string schema, string name, string title, SharedObjectSubtypeEnum.ObjectSubtype subtype, UserTypeEnum.UserType source)
	{
		schemaTextEdit.Text = (this.schema = schema);
		nameTextEdit.Text = (this.name = name);
		procedureTitleTextEdit.Text = title;
		CommonFunctionsPanels.SetName(procedureTextEdit, procedureDescriptionXtraTabPage, ObjectType, subtype, this.schema, this.name, this.title, databaseType, databaseTitle, showDatabaseTitle, base.ShowSchema, source);
	}

	public void SetDisableSettingAsEdited(bool value)
	{
		base.DisableSettingAsEdited = value;
	}

	private void DesignProcedureBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		EditProcedureOrFunction();
	}

	private void DesignFunctionBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		EditProcedureOrFunction();
	}

	private void EditProcedureOrFunction()
	{
		if (!base.MainControl.ContinueAfterPossibleChanges())
		{
			return;
		}
		using DesignProcedureForm designProcedureForm = new DesignProcedureForm(databaseId.Value, procedureId, schema, name, title, source ?? UserTypeEnum.UserType.DBMS, ObjectType, base.Subtype, databaseName, base.CustomFieldsSupport);
		if (designProcedureForm.ShowDialog() == DialogResult.OK)
		{
			RefreshParameters(forceRefresh: true);
			schema = designProcedureForm.ProcedureDesigner.Schema;
			title = designProcedureForm.ProcedureDesigner.Title;
			name = designProcedureForm.ProcedureDesigner.Name;
			nameTextEdit.Text = name;
			schemaTextEdit.Text = schema;
			source = designProcedureForm.ProcedureDesigner.Source;
			SetTitleTextEdit(title);
			base.Subtype = designProcedureForm.ProcedureDesigner.Type;
		}
	}

	public void FocusDataLineageTab(int? processId, bool selectDiagramTab = false, bool? showColumns = null)
	{
		procedureXtraTabControl.SelectedTabPage = dataLineageXtraTabPage;
		dataLineageUserControl.SelectDataProcess(processId);
		dataLineageUserControl.ChangeColumnsVisibility(showColumns);
		if (selectDiagramTab)
		{
			dataLineageUserControl.SelectDiagramTab();
		}
	}

	private void DocumentationModulesUserControl_Enter(object sender, EventArgs e)
	{
		if (!isModuleDropdownLoaded)
		{
			base.MainControl?.SetWaitformVisibility(visible: true);
			base.ModulesId = documentationModulesUserControl.GetCurrentRowModulesId(ObjectType, procedureId);
			base.MainControl?.SetWaitformVisibility(visible: false);
			documentationModulesUserControl.Refresh();
			isModuleDropdownLoaded = true;
		}
	}

	public void DataLineageButtonsVisibleChanged(object sender, EventArgs e)
	{
		base.MainControl.DataLineageTabVisibilityChanged(procedureXtraTabControl.SelectedTabPage == dataLineageXtraTabPage);
	}

	public DataLineageUserControl GetDataLineageControl()
	{
		return dataLineageUserControl;
	}

	private void CustomFieldsPanelControl_ShowHistoryClick(object sender, EventArgs e)
	{
		if (e is TextEventArgs textEventArgs && textEventArgs.Text.StartsWith("field"))
		{
			ViewHistoryForField(textEventArgs.Text);
		}
	}

	public void ViewHistoryForField(string fieldName)
	{
		try
		{
			using HistoryForm historyForm = new HistoryForm();
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
			historyForm.CustomFieldCaption = customFields?.CustomFieldsData?.Where((CustomFieldDefinition x) => x.CustomField.FieldName.ToLower() == fieldName)?.FirstOrDefault()?.CustomField?.Title;
			historyForm.SetParameters(procedureId, fieldName, name, schema, base.DatabaseShowSchema, base.DatabaseShowSchemaOverride, objectTitleAndDescriptionHistory?.Title, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(ObjectType), SharedObjectTypeEnum.TypeToString(ObjectType), SharedObjectSubtypeEnum.TypeToString(ObjectType, base.Subtype), (!source.HasValue) ? null : UserTypeEnum.TypeToString(source.Value), null);
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			historyForm.ShowDialog(FindForm());
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void ProcedureTitleTextEdit_Properties_BeforeShowMenu(object sender, BeforeShowMenuEventArgs e)
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

	private void SaveHistory(ProcedureRow procedureRow)
	{
		bool saveTitle = HistoryGeneralHelper.CheckAreValuesDiffrent(objectTitleAndDescriptionHistory.Title, procedureTitleTextEdit.Text);
		bool saveDescription = HistoryGeneralHelper.CheckAreHtmlValuesAreDiffrent(procedureRow?.DescriptionPlain, procedureRow?.Description, objectTitleAndDescriptionHistory?.DescriptionPlain, objectTitleAndDescriptionHistory?.Description);
		objectTitleAndDescriptionHistory = new ObjectWithTitleAndHTMLDescriptionHistory
		{
			ObjectId = procedureId,
			Title = procedureTitleTextEdit.Text,
			Description = PrepareValue.GetHtmlText(DescriptionHtmlUserControl?.PlainText, DescriptionHtmlUserControl?.HtmlText),
			DescriptionPlain = DescriptionHtmlUserControl?.PlainText
		};
		HistoryCustomFieldsHelper.InsertHistoryCustomFieldsOnTablePanel(procedureId, objectCustomFieldsForHistory, customFields, databaseId, ObjectType);
		objectCustomFieldsForHistory = new Dictionary<string, string>();
		CustomFieldDefinition[] customFieldsData = customFields.CustomFieldsData;
		foreach (CustomFieldDefinition customFieldDefinition in customFieldsData)
		{
			objectCustomFieldsForHistory.Add(customFieldDefinition.CustomField.FieldName, customFieldDefinition.FieldValue);
		}
		DB.History.InsertHistoryRow(databaseId, procedureId, objectTitleAndDescriptionHistory?.Title, objectTitleAndDescriptionHistory?.Description, objectTitleAndDescriptionHistory?.DescriptionPlain, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(ObjectType), saveTitle, saveDescription, ObjectType);
		if (DescriptionHtmlUserControl?.ProcedureObject != null)
		{
			DescriptionHtmlUserControl.ProcedureObject.Title = objectTitleAndDescriptionHistory?.Title;
			DescriptionHtmlUserControl.ProcedureObject.ObjectType = SharedObjectTypeEnum.TypeToString(ObjectType);
		}
	}

	private void SaveProcedureTitleAndDescription(ProcedureObject procedureRow)
	{
		if (DescriptionHtmlUserControl != null)
		{
			objectTitleAndDescriptionHistory = new ObjectWithTitleAndHTMLDescriptionHistory
			{
				ObjectId = ObjectId,
				Title = procedureTitleTextEdit.Text,
				Description = PrepareValue.GetHtmlText(DescriptionHtmlUserControl.PlainText, DescriptionHtmlUserControl.HtmlText),
				DescriptionPlain = DescriptionHtmlUserControl.PlainText
			};
			DescriptionHtmlUserControl.ClearHistoryObjects();
			DescriptionHtmlUserControl.ProcedureObject = procedureRow;
		}
	}

	private void ViewHistoryBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
		string field = viewHistoryBarButtonItem?.Tag?.ToString();
		if (!(procedureParametersGridView.GetFocusedRow() is ParameterRow parameterRow) || (!(field == "title") && !(field == "description") && !field.ToLower().StartsWith("field")))
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			return;
		}
		try
		{
			using HistoryForm historyForm = new HistoryForm();
			bool isDeleted = CommonFunctionsDatabase.IsDeletedFromDB(parameterRow);
			Bitmap parameterIcon = Icons.GetParameterIcon(ParameterRow.GetMode(parameterRow.ParameterMode), parameterRow.Source, isDeleted);
			historyForm.CustomFieldCaption = customFields?.CustomFieldsData?.Where((CustomFieldDefinition x) => x.CustomField.FieldName.ToLower() == field)?.FirstOrDefault()?.CustomField?.Title;
			historyForm.SetParameters(parameterRow.Id, field, parameterRow.Name, parameterRow.ProcedureSchema, base.DatabaseShowSchema, base.DatabaseShowSchemaOverride, parameterRow.Title, "parameters", SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Parameter), null, UserTypeEnum.TypeToString(parameterRow.Source), parameterIcon);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.PanelControls.ProcedureUserControl));
		Dataedo.App.Tools.DefaultBulkCopy defaultBulkCopy = new Dataedo.App.Tools.DefaultBulkCopy();
		this.procedureXtraTabControl = new DevExpress.XtraTab.XtraTabControl();
		this.procedureDescriptionXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.procedureLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.searchCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.descriptionLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.schemaTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.documentationTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.nameTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.procedureHtmlUserControl = new Dataedo.App.UserControls.HtmlUserControl();
		this.customFieldsPanelControl = new Dataedo.App.UserControls.CustomFieldsPanelControl();
		this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
		this.databaseHostTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.procedureTitleTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.documentationModulesUserControl = new Dataedo.App.UserControls.DocumentationModulesUserControl();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.titleLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.customFieldsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem12 = new DevExpress.XtraLayout.LayoutControlItem();
		this.nameLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.documentationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.schemaLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.modulesLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.procedureParametersXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.procedureParametersGrid = new DevExpress.XtraGrid.GridControl();
		this.procedureParametersGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.iconProcedureParametersGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconFunctionParamterRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.ordinalPositionProcedureParametersGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameProcedureParametersGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.parameterModeProcedureParametersGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.datatypeProcedureParametersGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionProcedureParametersGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.descriptionInputOutputRepositoryItemMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.createdGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.procedureToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.parametersGridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.dataLineageXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.dataLineageUserControl = new Dataedo.App.UserControls.DataLineage.DataLineageUserControl();
		this.dependenciesXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.dependenciesUserControl = new Dataedo.App.UserControls.Dependencies.DependenciesUserControl();
		this.procedureScriptXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.scriptSearchCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
		this.scriptRichEditControl = new Dataedo.App.UserControls.RichEditUserControl();
		this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
		this.schemaImportsAndChangesXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.schemaImportsAndChangesUserControl = new Dataedo.App.UserControls.SchemaImportsAndChanges.SchemaImportsAndChangesUserControl();
		this.procedureMetadataXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.metadataLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.dbmsLastUpdatedLabel = new DevExpress.XtraEditors.LabelControl();
		this.lastSynchronizedLabel = new DevExpress.XtraEditors.LabelControl();
		this.lastUpdatedLabel = new DevExpress.XtraEditors.LabelControl();
		this.dbmsCreatedLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.createdLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.dbmsLastUpdatedLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.lastImportedLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.lastUpdatedLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.dbmsCreatedLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.firstImportedLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.metadataToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.procedureStatusUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.procedureTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.parametersPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.designProcedureBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.designFunctionBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeParametersParemetersBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.parametersBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.viewHistoryBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		((System.ComponentModel.ISupportInitialize)this.procedureXtraTabControl).BeginInit();
		this.procedureXtraTabControl.SuspendLayout();
		this.procedureDescriptionXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.procedureLayoutControl).BeginInit();
		this.procedureLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.schemaTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.documentationTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseHostTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.procedureTitleTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.documentationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.schemaLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.modulesLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		this.procedureParametersXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.procedureParametersGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.procedureParametersGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconFunctionParamterRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionInputOutputRepositoryItemMemoEdit).BeginInit();
		this.dataLineageXtraTabPage.SuspendLayout();
		this.dependenciesXtraTabPage.SuspendLayout();
		this.procedureScriptXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).BeginInit();
		this.schemaImportsAndChangesXtraTabPage.SuspendLayout();
		this.procedureMetadataXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.metadataLayoutControl).BeginInit();
		this.metadataLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsLastUpdatedLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lastImportedLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lastUpdatedLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsCreatedLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.firstImportedLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.procedureTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.parametersPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.parametersBarManager).BeginInit();
		base.SuspendLayout();
		this.procedureXtraTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.procedureXtraTabControl.Location = new System.Drawing.Point(0, 60);
		this.procedureXtraTabControl.Name = "procedureXtraTabControl";
		this.procedureXtraTabControl.SelectedTabPage = this.procedureDescriptionXtraTabPage;
		this.procedureXtraTabControl.Size = new System.Drawing.Size(881, 509);
		this.procedureXtraTabControl.TabIndex = 1;
		this.procedureXtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[7] { this.procedureDescriptionXtraTabPage, this.procedureParametersXtraTabPage, this.dataLineageXtraTabPage, this.dependenciesXtraTabPage, this.procedureScriptXtraTabPage, this.schemaImportsAndChangesXtraTabPage, this.procedureMetadataXtraTabPage });
		this.procedureXtraTabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(procedureXtraTabControl_SelectedPageChanged);
		this.procedureDescriptionXtraTabPage.Controls.Add(this.procedureLayoutControl);
		this.procedureDescriptionXtraTabPage.Name = "procedureDescriptionXtraTabPage";
		this.procedureDescriptionXtraTabPage.Size = new System.Drawing.Size(879, 484);
		this.procedureDescriptionXtraTabPage.Text = "Procedure";
		this.procedureLayoutControl.AllowCustomization = false;
		this.procedureLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.procedureLayoutControl.Controls.Add(this.searchCountLabelControl);
		this.procedureLayoutControl.Controls.Add(this.descriptionLabelControl);
		this.procedureLayoutControl.Controls.Add(this.schemaTextEdit);
		this.procedureLayoutControl.Controls.Add(this.documentationTextEdit);
		this.procedureLayoutControl.Controls.Add(this.nameTextEdit);
		this.procedureLayoutControl.Controls.Add(this.procedureHtmlUserControl);
		this.procedureLayoutControl.Controls.Add(this.customFieldsPanelControl);
		this.procedureLayoutControl.Controls.Add(this.textEdit1);
		this.procedureLayoutControl.Controls.Add(this.databaseHostTextEdit);
		this.procedureLayoutControl.Controls.Add(this.procedureTitleTextEdit);
		this.procedureLayoutControl.Controls.Add(this.documentationModulesUserControl);
		this.procedureLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.procedureLayoutControl.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.layoutControlItem2, this.layoutControlItem3 });
		this.procedureLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.procedureLayoutControl.Name = "procedureLayoutControl";
		this.procedureLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(-668, -62, 250, 350);
		this.procedureLayoutControl.Root = this.layoutControlGroup1;
		this.procedureLayoutControl.Size = new System.Drawing.Size(879, 484);
		this.procedureLayoutControl.TabIndex = 0;
		this.procedureLayoutControl.Text = "layoutControl1";
		this.procedureLayoutControl.ToolTipController = this.toolTipController;
		this.searchCountLabelControl.Location = new System.Drawing.Point(69, 160);
		this.searchCountLabelControl.Name = "searchCountLabelControl";
		this.searchCountLabelControl.Size = new System.Drawing.Size(798, 13);
		this.searchCountLabelControl.StyleController = this.procedureLayoutControl;
		this.searchCountLabelControl.TabIndex = 11;
		this.descriptionLabelControl.Location = new System.Drawing.Point(12, 160);
		this.descriptionLabelControl.Name = "descriptionLabelControl";
		this.descriptionLabelControl.Size = new System.Drawing.Size(53, 13);
		this.descriptionLabelControl.StyleController = this.procedureLayoutControl;
		this.descriptionLabelControl.TabIndex = 10;
		this.descriptionLabelControl.Text = "Description";
		this.descriptionLabelControl.ToolTipController = this.toolTipController;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.schemaTextEdit.Location = new System.Drawing.Point(94, 36);
		this.schemaTextEdit.Name = "schemaTextEdit";
		this.schemaTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.schemaTextEdit.Properties.ReadOnly = true;
		this.schemaTextEdit.Size = new System.Drawing.Size(773, 18);
		this.schemaTextEdit.StyleController = this.procedureLayoutControl;
		this.schemaTextEdit.TabIndex = 3;
		this.schemaTextEdit.TabStop = false;
		this.documentationTextEdit.Location = new System.Drawing.Point(94, 12);
		this.documentationTextEdit.Name = "documentationTextEdit";
		this.documentationTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.documentationTextEdit.Properties.ReadOnly = true;
		this.documentationTextEdit.Size = new System.Drawing.Size(773, 18);
		this.documentationTextEdit.StyleController = this.procedureLayoutControl;
		this.documentationTextEdit.TabIndex = 2;
		this.documentationTextEdit.TabStop = false;
		this.nameTextEdit.Location = new System.Drawing.Point(94, 60);
		this.nameTextEdit.Name = "nameTextEdit";
		this.nameTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.nameTextEdit.Properties.ReadOnly = true;
		this.nameTextEdit.Size = new System.Drawing.Size(773, 18);
		this.nameTextEdit.StyleController = this.procedureLayoutControl;
		this.nameTextEdit.TabIndex = 4;
		this.nameTextEdit.TabStop = false;
		this.procedureHtmlUserControl.BackColor = System.Drawing.Color.Transparent;
		this.procedureHtmlUserControl.CanListen = false;
		this.procedureHtmlUserControl.DatabaseRow = null;
		this.procedureHtmlUserControl.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.procedureHtmlUserControl.HtmlText = resources.GetString("procedureHtmlUserControl.HtmlText");
		this.procedureHtmlUserControl.IsHighlighted = false;
		this.procedureHtmlUserControl.Location = new System.Drawing.Point(12, 177);
		this.procedureHtmlUserControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.procedureHtmlUserControl.ModuleRow = null;
		this.procedureHtmlUserControl.Name = "procedureHtmlUserControl";
		this.procedureHtmlUserControl.OccurrencesCount = 0;
		this.procedureHtmlUserControl.OriginalHtmlText = resources.GetString("procedureHtmlUserControl.OriginalHtmlText");
		this.procedureHtmlUserControl.PlainText = "\u00a0";
		this.procedureHtmlUserControl.ProcedureObject = null;
		this.procedureHtmlUserControl.Size = new System.Drawing.Size(855, 305);
		this.procedureHtmlUserControl.SplashScreenManager = null;
		this.procedureHtmlUserControl.TabIndex = 8;
		this.procedureHtmlUserControl.TableRow = null;
		this.procedureHtmlUserControl.TermObject = null;
		this.customFieldsPanelControl.BackColor = System.Drawing.Color.Transparent;
		this.customFieldsPanelControl.Location = new System.Drawing.Point(10, 136);
		this.customFieldsPanelControl.Margin = new System.Windows.Forms.Padding(0);
		this.customFieldsPanelControl.Name = "customFieldsPanelControl";
		this.customFieldsPanelControl.Size = new System.Drawing.Size(857, 20);
		this.customFieldsPanelControl.TabIndex = 7;
		this.textEdit1.Location = new System.Drawing.Point(108, 36);
		this.textEdit1.Name = "textEdit1";
		this.textEdit1.Size = new System.Drawing.Size(1126, 20);
		this.textEdit1.StyleController = this.procedureLayoutControl;
		this.textEdit1.TabIndex = 9;
		this.databaseHostTextEdit.Location = new System.Drawing.Point(44, 36);
		this.databaseHostTextEdit.Name = "databaseHostTextEdit";
		this.databaseHostTextEdit.Size = new System.Drawing.Size(1190, 20);
		this.databaseHostTextEdit.StyleController = this.procedureLayoutControl;
		this.databaseHostTextEdit.TabIndex = 5;
		this.procedureTitleTextEdit.Location = new System.Drawing.Point(97, 84);
		this.procedureTitleTextEdit.MaximumSize = new System.Drawing.Size(0, 22);
		this.procedureTitleTextEdit.MinimumSize = new System.Drawing.Size(0, 22);
		this.procedureTitleTextEdit.Name = "procedureTitleTextEdit";
		this.procedureTitleTextEdit.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(ProcedureTitleTextEdit_Properties_BeforeShowMenu);
		this.procedureTitleTextEdit.Size = new System.Drawing.Size(411, 22);
		this.procedureTitleTextEdit.StyleController = this.procedureLayoutControl;
		this.procedureTitleTextEdit.TabIndex = 5;
		this.procedureTitleTextEdit.EditValueChanged += new System.EventHandler(procedureTitleTextEdit_EditValueChanged);
		this.documentationModulesUserControl.BackColor = System.Drawing.Color.Transparent;
		this.documentationModulesUserControl.IsChanged = false;
		this.documentationModulesUserControl.Location = new System.Drawing.Point(97, 110);
		this.documentationModulesUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.documentationModulesUserControl.MaximumSize = new System.Drawing.Size(500, 24);
		this.documentationModulesUserControl.MinimumSize = new System.Drawing.Size(300, 24);
		this.documentationModulesUserControl.Modules = null;
		this.documentationModulesUserControl.Name = "documentationModulesUserControl";
		this.documentationModulesUserControl.Size = new System.Drawing.Size(411, 24);
		this.documentationModulesUserControl.TabIndex = 6;
		this.documentationModulesUserControl.EditValueChanged += new System.EventHandler(documentationModulesUserControl1_EditValueChanged);
		this.documentationModulesUserControl.Enter += new System.EventHandler(DocumentationModulesUserControl_Enter);
		this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem2.Control = this.databaseHostTextEdit;
		this.layoutControlItem2.CustomizationFormText = "Host:";
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(1226, 24);
		this.layoutControlItem2.Text = "Host:";
		this.layoutControlItem2.TextSize = new System.Drawing.Size(31, 13);
		this.layoutControlItem3.Control = this.textEdit1;
		this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(1226, 24);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(50, 20);
		this.layoutControlGroup1.CustomizationFormText = "Root";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[11]
		{
			this.titleLayoutControlItem, this.emptySpaceItem2, this.emptySpaceItem1, this.customFieldsLayoutControlItem, this.layoutControlItem12, this.nameLayoutControlItem, this.documentationLayoutControlItem, this.schemaLayoutControlItem, this.modulesLayoutControlItem, this.layoutControlItem4,
			this.layoutControlItem5
		});
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(879, 484);
		this.layoutControlGroup1.TextVisible = false;
		this.titleLayoutControlItem.Control = this.procedureTitleTextEdit;
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
		this.emptySpaceItem2.Location = new System.Drawing.Point(500, 98);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(359, 26);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
		this.emptySpaceItem1.Location = new System.Drawing.Point(500, 72);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(359, 26);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.customFieldsLayoutControlItem.Control = this.customFieldsPanelControl;
		this.customFieldsLayoutControlItem.Location = new System.Drawing.Point(0, 124);
		this.customFieldsLayoutControlItem.Name = "customFieldsLayoutControlItem";
		this.customFieldsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
		this.customFieldsLayoutControlItem.Size = new System.Drawing.Size(859, 24);
		this.customFieldsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.customFieldsLayoutControlItem.TextVisible = false;
		this.layoutControlItem12.Control = this.procedureHtmlUserControl;
		this.layoutControlItem12.Location = new System.Drawing.Point(0, 165);
		this.layoutControlItem12.MinSize = new System.Drawing.Size(104, 300);
		this.layoutControlItem12.Name = "layoutControlItem12";
		this.layoutControlItem12.Size = new System.Drawing.Size(859, 309);
		this.layoutControlItem12.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem12.Text = "Description";
		this.layoutControlItem12.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem12.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem12.TextVisible = false;
		this.nameLayoutControlItem.Control = this.nameTextEdit;
		this.nameLayoutControlItem.Location = new System.Drawing.Point(0, 48);
		this.nameLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.nameLayoutControlItem.MinSize = new System.Drawing.Size(110, 24);
		this.nameLayoutControlItem.Name = "nameLayoutControlItem";
		this.nameLayoutControlItem.Size = new System.Drawing.Size(859, 24);
		this.nameLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.nameLayoutControlItem.Text = "Name";
		this.nameLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.nameLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.nameLayoutControlItem.TextToControlDistance = 0;
		this.documentationLayoutControlItem.Control = this.documentationTextEdit;
		this.documentationLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.documentationLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.documentationLayoutControlItem.MinSize = new System.Drawing.Size(110, 24);
		this.documentationLayoutControlItem.Name = "documentationLayoutControlItem";
		this.documentationLayoutControlItem.Size = new System.Drawing.Size(859, 24);
		this.documentationLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.documentationLayoutControlItem.Text = "Database";
		this.documentationLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.documentationLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.documentationLayoutControlItem.TextToControlDistance = 0;
		this.schemaLayoutControlItem.Control = this.schemaTextEdit;
		this.schemaLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.schemaLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.schemaLayoutControlItem.MinSize = new System.Drawing.Size(110, 24);
		this.schemaLayoutControlItem.Name = "schemaLayoutControlItem";
		this.schemaLayoutControlItem.Size = new System.Drawing.Size(859, 24);
		this.schemaLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.schemaLayoutControlItem.Text = "Schema";
		this.schemaLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.schemaLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.schemaLayoutControlItem.TextToControlDistance = 0;
		this.modulesLayoutControlItem.Control = this.documentationModulesUserControl;
		this.modulesLayoutControlItem.CustomizationFormText = "Subject Area";
		this.modulesLayoutControlItem.Location = new System.Drawing.Point(0, 98);
		this.modulesLayoutControlItem.MaxSize = new System.Drawing.Size(500, 26);
		this.modulesLayoutControlItem.MinSize = new System.Drawing.Size(500, 26);
		this.modulesLayoutControlItem.Name = "modulesLayoutControlItem";
		this.modulesLayoutControlItem.Size = new System.Drawing.Size(500, 26);
		this.modulesLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.modulesLayoutControlItem.Text = "Subject Area";
		this.modulesLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.modulesLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.modulesLayoutControlItem.TextToControlDistance = 3;
		this.layoutControlItem4.Control = this.descriptionLabelControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 148);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(57, 17);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem5.Control = this.searchCountLabelControl;
		this.layoutControlItem5.Location = new System.Drawing.Point(57, 148);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(802, 17);
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.procedureParametersXtraTabPage.Controls.Add(this.procedureParametersGrid);
		this.procedureParametersXtraTabPage.Controls.Add(this.parametersGridPanelUserControl);
		this.procedureParametersXtraTabPage.Name = "procedureParametersXtraTabPage";
		this.procedureParametersXtraTabPage.Size = new System.Drawing.Size(879, 484);
		this.procedureParametersXtraTabPage.Text = "Input/Output";
		this.procedureParametersGrid.Dock = System.Windows.Forms.DockStyle.Fill;
		this.procedureParametersGrid.Location = new System.Drawing.Point(0, 30);
		this.procedureParametersGrid.MainView = this.procedureParametersGridView;
		this.procedureParametersGrid.Name = "procedureParametersGrid";
		this.procedureParametersGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.iconFunctionParamterRepositoryItemPictureEdit, this.descriptionInputOutputRepositoryItemMemoEdit });
		this.procedureParametersGrid.Size = new System.Drawing.Size(879, 454);
		this.procedureParametersGrid.TabIndex = 1;
		this.procedureParametersGrid.ToolTipController = this.procedureToolTipController;
		this.procedureParametersGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.procedureParametersGridView });
		this.procedureParametersGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.iconProcedureParametersGridColumn, this.ordinalPositionProcedureParametersGridColumn, this.nameProcedureParametersGridColumn, this.parameterModeProcedureParametersGridColumn, this.datatypeProcedureParametersGridColumn, this.descriptionProcedureParametersGridColumn, this.createdGridColumn, this.createdByGridColumn, this.lastUpdatedGridColumn, this.lastUpdatedByGridColumn });
		defaultBulkCopy.IsCopying = false;
		this.procedureParametersGridView.Copy = defaultBulkCopy;
		this.procedureParametersGridView.GridControl = this.procedureParametersGrid;
		this.procedureParametersGridView.Name = "procedureParametersGridView";
		this.procedureParametersGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.procedureParametersGridView.OptionsFilter.ShowAllTableValuesInFilterPopup = true;
		this.procedureParametersGridView.OptionsFilter.UseNewCustomFilterDialog = true;
		this.procedureParametersGridView.OptionsSelection.MultiSelect = true;
		this.procedureParametersGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.procedureParametersGridView.OptionsView.ColumnAutoWidth = false;
		this.procedureParametersGridView.OptionsView.RowAutoHeight = true;
		this.procedureParametersGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.procedureParametersGridView.OptionsView.ShowGroupPanel = false;
		this.procedureParametersGridView.OptionsView.ShowIndicator = false;
		this.procedureParametersGridView.RowHighlightingIsEnabled = true;
		this.procedureParametersGridView.SplashScreenManager = null;
		this.procedureParametersGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(procedureParametersGridView_CustomDrawCell);
		this.procedureParametersGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(procedureParametersGridView_PopupMenuShowing);
		this.procedureParametersGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(procedureParametersGridView_CellValueChanged);
		this.procedureParametersGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(ProcedureParametersGridView_CellValueChanging);
		this.procedureParametersGridView.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(procedureParametersGridView_CustomColumnSort);
		this.procedureParametersGridView.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(procedureParametersGridView_CustomUnboundColumnData);
		this.iconProcedureParametersGridColumn.Caption = " ";
		this.iconProcedureParametersGridColumn.ColumnEdit = this.iconFunctionParamterRepositoryItemPictureEdit;
		this.iconProcedureParametersGridColumn.FieldName = "iconProcedureParametersGridColumn";
		this.iconProcedureParametersGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.iconProcedureParametersGridColumn.MaxWidth = 21;
		this.iconProcedureParametersGridColumn.MinWidth = 21;
		this.iconProcedureParametersGridColumn.Name = "iconProcedureParametersGridColumn";
		this.iconProcedureParametersGridColumn.OptionsColumn.AllowEdit = false;
		this.iconProcedureParametersGridColumn.OptionsColumn.ReadOnly = true;
		this.iconProcedureParametersGridColumn.OptionsFilter.AllowAutoFilter = false;
		this.iconProcedureParametersGridColumn.OptionsFilter.AllowFilter = false;
		this.iconProcedureParametersGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.iconProcedureParametersGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.iconProcedureParametersGridColumn.Visible = true;
		this.iconProcedureParametersGridColumn.VisibleIndex = 0;
		this.iconProcedureParametersGridColumn.Width = 21;
		this.iconFunctionParamterRepositoryItemPictureEdit.AllowFocused = false;
		this.iconFunctionParamterRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.iconFunctionParamterRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconFunctionParamterRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconFunctionParamterRepositoryItemPictureEdit.Name = "iconFunctionParamterRepositoryItemPictureEdit";
		this.iconFunctionParamterRepositoryItemPictureEdit.ShowMenu = false;
		this.ordinalPositionProcedureParametersGridColumn.Caption = "#";
		this.ordinalPositionProcedureParametersGridColumn.FieldName = "Position";
		this.ordinalPositionProcedureParametersGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.ordinalPositionProcedureParametersGridColumn.MaxWidth = 100;
		this.ordinalPositionProcedureParametersGridColumn.MinWidth = 30;
		this.ordinalPositionProcedureParametersGridColumn.Name = "ordinalPositionProcedureParametersGridColumn";
		this.ordinalPositionProcedureParametersGridColumn.OptionsColumn.AllowEdit = false;
		this.ordinalPositionProcedureParametersGridColumn.OptionsColumn.ReadOnly = true;
		this.ordinalPositionProcedureParametersGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.ordinalPositionProcedureParametersGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.ordinalPositionProcedureParametersGridColumn.Tag = "FIT_WIDTH";
		this.ordinalPositionProcedureParametersGridColumn.Visible = true;
		this.ordinalPositionProcedureParametersGridColumn.VisibleIndex = 1;
		this.ordinalPositionProcedureParametersGridColumn.Width = 30;
		this.nameProcedureParametersGridColumn.Caption = "Parameter";
		this.nameProcedureParametersGridColumn.FieldName = "Name";
		this.nameProcedureParametersGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.nameProcedureParametersGridColumn.Name = "nameProcedureParametersGridColumn";
		this.nameProcedureParametersGridColumn.OptionsColumn.AllowEdit = false;
		this.nameProcedureParametersGridColumn.OptionsColumn.ReadOnly = true;
		this.nameProcedureParametersGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.nameProcedureParametersGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.nameProcedureParametersGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.nameProcedureParametersGridColumn.Tag = "FIT_WIDTH";
		this.nameProcedureParametersGridColumn.Visible = true;
		this.nameProcedureParametersGridColumn.VisibleIndex = 2;
		this.nameProcedureParametersGridColumn.Width = 140;
		this.parameterModeProcedureParametersGridColumn.Caption = "Mode";
		this.parameterModeProcedureParametersGridColumn.FieldName = "ParameterMode";
		this.parameterModeProcedureParametersGridColumn.Name = "parameterModeProcedureParametersGridColumn";
		this.parameterModeProcedureParametersGridColumn.OptionsColumn.AllowEdit = false;
		this.parameterModeProcedureParametersGridColumn.OptionsColumn.ReadOnly = true;
		this.parameterModeProcedureParametersGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.parameterModeProcedureParametersGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.parameterModeProcedureParametersGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.parameterModeProcedureParametersGridColumn.Tag = "FIT_WIDTH";
		this.parameterModeProcedureParametersGridColumn.Visible = true;
		this.parameterModeProcedureParametersGridColumn.VisibleIndex = 3;
		this.parameterModeProcedureParametersGridColumn.Width = 100;
		this.datatypeProcedureParametersGridColumn.Caption = "Data type";
		this.datatypeProcedureParametersGridColumn.FieldName = "DataType";
		this.datatypeProcedureParametersGridColumn.Name = "datatypeProcedureParametersGridColumn";
		this.datatypeProcedureParametersGridColumn.OptionsColumn.AllowEdit = false;
		this.datatypeProcedureParametersGridColumn.OptionsColumn.ReadOnly = true;
		this.datatypeProcedureParametersGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.datatypeProcedureParametersGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.datatypeProcedureParametersGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.datatypeProcedureParametersGridColumn.Tag = "FIT_WIDTH";
		this.datatypeProcedureParametersGridColumn.Visible = true;
		this.datatypeProcedureParametersGridColumn.VisibleIndex = 4;
		this.datatypeProcedureParametersGridColumn.Width = 100;
		this.descriptionProcedureParametersGridColumn.AppearanceCell.Options.UseTextOptions = true;
		this.descriptionProcedureParametersGridColumn.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.descriptionProcedureParametersGridColumn.Caption = "Description";
		this.descriptionProcedureParametersGridColumn.ColumnEdit = this.descriptionInputOutputRepositoryItemMemoEdit;
		this.descriptionProcedureParametersGridColumn.FieldName = "Description";
		this.descriptionProcedureParametersGridColumn.MaxWidth = 1000;
		this.descriptionProcedureParametersGridColumn.MinWidth = 200;
		this.descriptionProcedureParametersGridColumn.Name = "descriptionProcedureParametersGridColumn";
		this.descriptionProcedureParametersGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.descriptionProcedureParametersGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.descriptionProcedureParametersGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.descriptionProcedureParametersGridColumn.Tag = "FIT_WIDTH";
		this.descriptionProcedureParametersGridColumn.Visible = true;
		this.descriptionProcedureParametersGridColumn.VisibleIndex = 5;
		this.descriptionProcedureParametersGridColumn.Width = 400;
		this.descriptionInputOutputRepositoryItemMemoEdit.Name = "descriptionInputOutputRepositoryItemMemoEdit";
		this.descriptionInputOutputRepositoryItemMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
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
		this.procedureToolTipController.AutoPopDelay = 10000;
		this.procedureToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.procedureToolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(procedureToolTipController_GetActiveObjectInfo);
		this.parametersGridPanelUserControl.BackColor = System.Drawing.Color.Transparent;
		this.parametersGridPanelUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.parametersGridPanelUserControl.GridView = this.procedureParametersGridView;
		this.parametersGridPanelUserControl.Location = new System.Drawing.Point(0, 0);
		this.parametersGridPanelUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.parametersGridPanelUserControl.Name = "parametersGridPanelUserControl";
		this.parametersGridPanelUserControl.Size = new System.Drawing.Size(879, 30);
		this.parametersGridPanelUserControl.TabIndex = 5;
		this.dataLineageXtraTabPage.Controls.Add(this.dataLineageUserControl);
		this.dataLineageXtraTabPage.Name = "dataLineageXtraTabPage";
		this.dataLineageXtraTabPage.Size = new System.Drawing.Size(879, 484);
		this.dataLineageXtraTabPage.Text = "Data Lineage";
		this.dataLineageUserControl.BackColor = System.Drawing.Color.Transparent;
		this.dataLineageUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dataLineageUserControl.Location = new System.Drawing.Point(0, 0);
		this.dataLineageUserControl.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
		this.dataLineageUserControl.Name = "dataLineageUserControl1";
		this.dataLineageUserControl.Size = new System.Drawing.Size(879, 484);
		this.dataLineageUserControl.TabIndex = 0;
		this.dependenciesXtraTabPage.Controls.Add(this.dependenciesUserControl);
		this.dependenciesXtraTabPage.Name = "dependenciesXtraTabPage";
		this.dependenciesXtraTabPage.Size = new System.Drawing.Size(879, 484);
		this.dependenciesXtraTabPage.Text = "Dependencies";
		this.dependenciesUserControl.BackColor = System.Drawing.Color.Transparent;
		this.dependenciesUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dependenciesUserControl.Location = new System.Drawing.Point(0, 0);
		this.dependenciesUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.dependenciesUserControl.Name = "dependenciesUserControl";
		this.dependenciesUserControl.RelationsGridControl = null;
		this.dependenciesUserControl.SetRelationsPageAsModified = null;
		this.dependenciesUserControl.Size = new System.Drawing.Size(879, 484);
		this.dependenciesUserControl.TabIndex = 0;
		this.dependenciesUserControl.DependencyChanging += new Dataedo.App.UserControls.Dependencies.DependenciesUserControl.DependencyChangingHandler(dependenciesUserControl_DependencyChanging);
		this.procedureScriptXtraTabPage.Controls.Add(this.layoutControl1);
		this.procedureScriptXtraTabPage.Name = "procedureScriptXtraTabPage";
		this.procedureScriptXtraTabPage.Size = new System.Drawing.Size(879, 484);
		this.procedureScriptXtraTabPage.Text = "Script";
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl1.Controls.Add(this.scriptSearchCountLabelControl);
		this.layoutControl1.Controls.Add(this.labelControl2);
		this.layoutControl1.Controls.Add(this.scriptRichEditControl);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.Root = this.layoutControlGroup3;
		this.layoutControl1.Size = new System.Drawing.Size(879, 484);
		this.layoutControl1.TabIndex = 3;
		this.layoutControl1.Text = "layoutControl1";
		this.scriptSearchCountLabelControl.Location = new System.Drawing.Point(33, 2);
		this.scriptSearchCountLabelControl.Name = "scriptSearchCountLabelControl";
		this.scriptSearchCountLabelControl.Size = new System.Drawing.Size(844, 13);
		this.scriptSearchCountLabelControl.StyleController = this.layoutControl1;
		this.scriptSearchCountLabelControl.TabIndex = 5;
		this.labelControl2.Location = new System.Drawing.Point(2, 2);
		this.labelControl2.Name = "labelControl2";
		this.labelControl2.Size = new System.Drawing.Size(27, 13);
		this.labelControl2.StyleController = this.layoutControl1;
		this.labelControl2.TabIndex = 4;
		this.labelControl2.Text = "Script";
		this.scriptRichEditControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
		this.scriptRichEditControl.IsHighlighted = false;
		this.scriptRichEditControl.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
		this.scriptRichEditControl.Location = new System.Drawing.Point(2, 19);
		this.scriptRichEditControl.Modified = true;
		this.scriptRichEditControl.Name = "scriptRichEditControl";
		this.scriptRichEditControl.OccurrencesCount = 0;
		this.scriptRichEditControl.Options.HorizontalScrollbar.Visibility = DevExpress.XtraRichEdit.RichEditScrollbarVisibility.Hidden;
		this.scriptRichEditControl.OriginalHtmlText = null;
		this.scriptRichEditControl.ReadOnly = true;
		this.scriptRichEditControl.Size = new System.Drawing.Size(875, 463);
		this.scriptRichEditControl.TabIndex = 2;
		this.scriptRichEditControl.Views.SimpleView.Padding = new System.Windows.Forms.Padding(4, 4, 4, 0);
		this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup3.GroupBordersVisible = false;
		this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.layoutControlItem6, this.layoutControlItem7, this.layoutControlItem8 });
		this.layoutControlGroup3.Name = "layoutControlGroup3";
		this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup3.Size = new System.Drawing.Size(879, 484);
		this.layoutControlGroup3.TextVisible = false;
		this.layoutControlItem6.Control = this.scriptRichEditControl;
		this.layoutControlItem6.Location = new System.Drawing.Point(0, 17);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.Size = new System.Drawing.Size(879, 467);
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.layoutControlItem7.Control = this.labelControl2;
		this.layoutControlItem7.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Size = new System.Drawing.Size(31, 17);
		this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.TextVisible = false;
		this.layoutControlItem8.Control = this.scriptSearchCountLabelControl;
		this.layoutControlItem8.Location = new System.Drawing.Point(31, 0);
		this.layoutControlItem8.Name = "layoutControlItem8";
		this.layoutControlItem8.Size = new System.Drawing.Size(848, 17);
		this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.TextVisible = false;
		this.schemaImportsAndChangesXtraTabPage.Controls.Add(this.schemaImportsAndChangesUserControl);
		this.schemaImportsAndChangesXtraTabPage.Name = "schemaImportsAndChangesXtraTabPage";
		this.schemaImportsAndChangesXtraTabPage.Size = new System.Drawing.Size(879, 484);
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
		this.schemaImportsAndChangesUserControl.Size = new System.Drawing.Size(879, 484);
		this.schemaImportsAndChangesUserControl.TabIndex = 1;
		this.procedureMetadataXtraTabPage.Controls.Add(this.metadataLayoutControl);
		this.procedureMetadataXtraTabPage.Name = "procedureMetadataXtraTabPage";
		this.procedureMetadataXtraTabPage.Size = new System.Drawing.Size(879, 484);
		this.procedureMetadataXtraTabPage.Text = "Metadata";
		this.metadataLayoutControl.AllowCustomization = false;
		this.metadataLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.metadataLayoutControl.Controls.Add(this.dbmsLastUpdatedLabel);
		this.metadataLayoutControl.Controls.Add(this.lastSynchronizedLabel);
		this.metadataLayoutControl.Controls.Add(this.lastUpdatedLabel);
		this.metadataLayoutControl.Controls.Add(this.dbmsCreatedLabelControl);
		this.metadataLayoutControl.Controls.Add(this.createdLabelControl);
		this.metadataLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.metadataLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.metadataLayoutControl.Name = "metadataLayoutControl";
		this.metadataLayoutControl.Root = this.layoutControlGroup2;
		this.metadataLayoutControl.Size = new System.Drawing.Size(879, 484);
		this.metadataLayoutControl.TabIndex = 0;
		this.metadataLayoutControl.Text = "layoutControl1";
		this.metadataLayoutControl.ToolTipController = this.metadataToolTipController;
		this.dbmsLastUpdatedLabel.Location = new System.Drawing.Point(147, 36);
		this.dbmsLastUpdatedLabel.Name = "dbmsLastUpdatedLabel";
		this.dbmsLastUpdatedLabel.Size = new System.Drawing.Size(720, 20);
		this.dbmsLastUpdatedLabel.StyleController = this.metadataLayoutControl;
		this.dbmsLastUpdatedLabel.TabIndex = 14;
		this.lastSynchronizedLabel.Location = new System.Drawing.Point(147, 84);
		this.lastSynchronizedLabel.Name = "lastSynchronizedLabel";
		this.lastSynchronizedLabel.Size = new System.Drawing.Size(720, 20);
		this.lastSynchronizedLabel.StyleController = this.metadataLayoutControl;
		this.lastSynchronizedLabel.TabIndex = 15;
		this.lastUpdatedLabel.Location = new System.Drawing.Point(147, 108);
		this.lastUpdatedLabel.Name = "lastUpdatedLabel";
		this.lastUpdatedLabel.Size = new System.Drawing.Size(720, 20);
		this.lastUpdatedLabel.StyleController = this.metadataLayoutControl;
		this.lastUpdatedLabel.TabIndex = 15;
		this.dbmsCreatedLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.dbmsCreatedLabelControl.Appearance.Options.UseFont = true;
		this.dbmsCreatedLabelControl.Location = new System.Drawing.Point(147, 12);
		this.dbmsCreatedLabelControl.Name = "dbmsCreatedLabelControl";
		this.dbmsCreatedLabelControl.Size = new System.Drawing.Size(720, 20);
		this.dbmsCreatedLabelControl.StyleController = this.metadataLayoutControl;
		this.dbmsCreatedLabelControl.TabIndex = 14;
		this.createdLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.createdLabelControl.Appearance.Options.UseFont = true;
		this.createdLabelControl.Location = new System.Drawing.Point(147, 60);
		this.createdLabelControl.Name = "createdLabelControl";
		this.createdLabelControl.Size = new System.Drawing.Size(720, 20);
		this.createdLabelControl.StyleController = this.metadataLayoutControl;
		this.createdLabelControl.TabIndex = 14;
		this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup2.GroupBordersVisible = false;
		this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.dbmsLastUpdatedLayoutControlItem, this.lastImportedLayoutControlItem, this.emptySpaceItem3, this.lastUpdatedLayoutControlItem, this.dbmsCreatedLayoutControlItem, this.firstImportedLayoutControlItem });
		this.layoutControlGroup2.Name = "Root";
		this.layoutControlGroup2.Size = new System.Drawing.Size(879, 484);
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
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 120);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(859, 344);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.lastUpdatedLayoutControlItem.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lastUpdatedLayoutControlItem.AppearanceItemCaption.Options.UseFont = true;
		this.lastUpdatedLayoutControlItem.Control = this.lastUpdatedLabel;
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
		this.procedureStatusUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.procedureStatusUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.procedureStatusUserControl.Description = "This procedure has been removed from the database. You can remove it from the repository.";
		this.procedureStatusUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.procedureStatusUserControl.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
		this.procedureStatusUserControl.Image = Dataedo.App.Properties.Resources.warning_16;
		this.procedureStatusUserControl.Location = new System.Drawing.Point(0, 20);
		this.procedureStatusUserControl.Margin = new System.Windows.Forms.Padding(4);
		this.procedureStatusUserControl.Name = "procedureStatusUserControl";
		this.procedureStatusUserControl.Size = new System.Drawing.Size(881, 40);
		this.procedureStatusUserControl.TabIndex = 10;
		this.procedureStatusUserControl.Visible = false;
		this.procedureTextEdit.Dock = System.Windows.Forms.DockStyle.Top;
		this.procedureTextEdit.EditValue = "";
		this.procedureTextEdit.Location = new System.Drawing.Point(0, 0);
		this.procedureTextEdit.Name = "procedureTextEdit";
		this.procedureTextEdit.Properties.AllowFocused = false;
		this.procedureTextEdit.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold);
		this.procedureTextEdit.Properties.Appearance.Options.UseFont = true;
		this.procedureTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.procedureTextEdit.Properties.ReadOnly = true;
		this.procedureTextEdit.Size = new System.Drawing.Size(881, 20);
		this.procedureTextEdit.TabIndex = 11;
		this.procedureTextEdit.TabStop = false;
		this.procedureTextEdit.ToolTipController = this.toolTipController;
		this.parametersPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[4]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.designProcedureBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.designFunctionBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeParametersParemetersBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.viewHistoryBarButtonItem)
		});
		this.parametersPopupMenu.Manager = this.parametersBarManager;
		this.parametersPopupMenu.Name = "parametersPopupMenu";
		this.parametersPopupMenu.BeforePopup += new System.ComponentModel.CancelEventHandler(parametersPopupMenu_BeforePopup);
		this.designProcedureBarButtonItem.Caption = "Design Procedure";
		this.designProcedureBarButtonItem.Id = 1;
		this.designProcedureBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
		this.designProcedureBarButtonItem.Name = "designProcedureBarButtonItem";
		this.designProcedureBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
		this.designProcedureBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(DesignProcedureBarButtonItem_ItemClick);
		this.designFunctionBarButtonItem.Caption = "Design Function";
		this.designFunctionBarButtonItem.Id = 2;
		this.designFunctionBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.edit_16;
		this.designFunctionBarButtonItem.Name = "designFunctionBarButtonItem";
		this.designFunctionBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
		this.designFunctionBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(DesignFunctionBarButtonItem_ItemClick);
		this.removeParametersParemetersBarButtonItem.Caption = "Remove from repository";
		this.removeParametersParemetersBarButtonItem.Id = 0;
		this.removeParametersParemetersBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeParametersParemetersBarButtonItem.Name = "removeParametersParemetersBarButtonItem";
		this.parametersBarManager.DockControls.Add(this.barDockControlTop);
		this.parametersBarManager.DockControls.Add(this.barDockControlBottom);
		this.parametersBarManager.DockControls.Add(this.barDockControlLeft);
		this.parametersBarManager.DockControls.Add(this.barDockControlRight);
		this.parametersBarManager.Form = this;
		this.parametersBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[4] { this.removeParametersParemetersBarButtonItem, this.designProcedureBarButtonItem, this.designFunctionBarButtonItem, this.viewHistoryBarButtonItem });
		this.parametersBarManager.MaxItemId = 4;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.parametersBarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(881, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 569);
		this.barDockControlBottom.Manager = this.parametersBarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(881, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.parametersBarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 569);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(881, 0);
		this.barDockControlRight.Manager = this.parametersBarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 569);
		this.viewHistoryBarButtonItem.Caption = "View history";
		this.viewHistoryBarButtonItem.Id = 3;
		this.viewHistoryBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.search_16;
		this.viewHistoryBarButtonItem.Name = "viewHistoryBarButtonItem";
		this.viewHistoryBarButtonItem.Tag = "";
		this.viewHistoryBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ViewHistoryBarButtonItem_ItemClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.procedureXtraTabControl);
		base.Controls.Add(this.procedureStatusUserControl);
		base.Controls.Add(this.procedureTextEdit);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "ProcedureUserControl";
		base.Size = new System.Drawing.Size(881, 569);
		base.Load += new System.EventHandler(ProcedureUserControl_Load);
		base.Leave += new System.EventHandler(ProcedureUserControl_Leave);
		((System.ComponentModel.ISupportInitialize)this.procedureXtraTabControl).EndInit();
		this.procedureXtraTabControl.ResumeLayout(false);
		this.procedureDescriptionXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.procedureLayoutControl).EndInit();
		this.procedureLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.schemaTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.documentationTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseHostTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.procedureTitleTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.documentationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.schemaLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.modulesLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		this.procedureParametersXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.procedureParametersGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.procedureParametersGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconFunctionParamterRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionInputOutputRepositoryItemMemoEdit).EndInit();
		this.dataLineageXtraTabPage.ResumeLayout(false);
		this.dependenciesXtraTabPage.ResumeLayout(false);
		this.procedureScriptXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).EndInit();
		this.schemaImportsAndChangesXtraTabPage.ResumeLayout(false);
		this.procedureMetadataXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.metadataLayoutControl).EndInit();
		this.metadataLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsLastUpdatedLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lastImportedLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lastUpdatedLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsCreatedLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.firstImportedLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.procedureTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.parametersPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.parametersBarManager).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
