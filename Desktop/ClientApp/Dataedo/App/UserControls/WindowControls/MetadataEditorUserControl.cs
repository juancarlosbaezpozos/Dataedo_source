using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.Data.StaticData;
using Dataedo.App.DataProfiling;
using Dataedo.App.DataProfiling.Models;
using Dataedo.App.DataProfiling.Tools;
using Dataedo.App.Documentation;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.ImportDescriptions;
using Dataedo.App.Licences;
using Dataedo.App.LoginFormTools.Tools.Enums;
using Dataedo.App.MenuTree;
using Dataedo.App.Onboarding;
using Dataedo.App.Onboarding.Home;
using Dataedo.App.Onboarding.Home.Model;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.ERD.Diagram;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Search;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.Columns;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.App.UserControls.PanelControls.Appearance;
using Dataedo.App.UserControls.SchemaImportsAndChanges.Model;
using Dataedo.App.UserControls.Search;
using Dataedo.App.UserControls.SummaryControls;
using Dataedo.App.UserControls.WindowControls.MetadataEditorUserControlFeatures;
using Dataedo.App.UserControls.WindowControls.MetadataEditorUserControlHelpers;
using Dataedo.CustomControls;
using Dataedo.CustomMessageBox;
using Dataedo.Model.Data.Modules;
using Dataedo.Model.Data.Progress;
using Dataedo.Model.Data.SchemaImportsAndChanges;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Extensions;
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
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.StyleFormatConditions;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraWaitForm;
using RecentProjectsLibrary;

namespace Dataedo.App.UserControls.WindowControls;

public class MetadataEditorUserControl : BaseUserControl
{
	private bool isSearchFinished = true;

	private bool disableClearingSearchSelection;

	private ProgressTypeModel progressType;

	private bool isSearchResultJustSelected;

	private HomePanelUserControl homePanelUserControl;

	private DatabaseUserControl databaseUserControl;

	private TableUserControl tableUserControl;

	private ModuleUserControl moduleUserControl;

	private ProcedureUserControl procedureUserControl;

	private TermUserControl termUserControl;

	private TableSummaryUserControl tableSummaryUserControl;

	private TableSummaryUserControl viewSummaryUserControl;

	private TableSummaryUserControl objectSummaryUserControl;

	private ProcedureSummaryUserControl procedureSummaryUserControl;

	private ProcedureSummaryUserControl functionSummaryUserControl;

	private ModuleSummaryUserControl moduleSummaryUserControl;

	private TermSummaryUserControl termSummaryUserControl;

	private UpgradeBusinessGlossaryControl upgradeBusinessGlossaryControl;

	private PopupMenu treePopupMenu;

	private BarManager treeBarManager;

	private BarButtonItem addModuleBarButtonItem;

	private BarButtonItem addTermBarButtonItem;

	private BarButtonItem addTermRelatedTermBarButtonItem;

	private BarButtonItem addDataLinkBarButtonItem;

	private BarButtonItem addNewTermBarButtonItem;

	private BarButtonItem refreshButtonItem;

	private BarButtonItem deleteBarButtonItem;

	private BarButtonItem designTableBarButtonItem;

	private BarButtonItem profileTableBarButtonItem;

	private BarButtonItem dataProfilingBarButtonItem;

	private BarButtonItem previewSampleDataBarButtonItem;

	private BarButtonItem synchronizeSchemaBarButtonItem;

	private BarButtonItem importDescriptionsBarButtonItem;

	private BarButtonItem generateDocumentationBarButtonItem;

	private BarButtonItem renameBarButtonItem;

	private BarButtonItem moveUpBarButtonItem;

	private BarButtonItem moveDownBarButtonItem;

	private BarSubItem addObjectBarButtonItem;

	private BarButtonItem addImportStructureBarButtonItem;

	private BarButtonItem addTableEntityBarButtonItem;

	private BarSubItem bulkAddObjectsBarButtonItem;

	private BarButtonItem bulkAddTableBarButtonItem;

	private BarButtonItem bulkAddStructureBarButtonItem;

	private BarButtonItem copyDescriptionsBarButtonItem;

	private BarButtonItem expandAllBarButtonItem;

	private BarButtonItem collapseAllBarButtonItem;

	private BarSubItem addBarButtonItem;

	private BarButtonItem addDatabaseConnectionBarButtonItem;

	private BarButtonItem addManualDatabaseBarButtonItem;

	private BarButtonItem addBusinessGlossaryBarButtonItem;

	private BarSubItem displaySchemaOptionsBarSubItem;

	private BarButtonItem alwaysDisplaySchemaBarButtonItem;

	private BarButtonItem neverDisplaySchemaBarButtonItem;

	private BarButtonItem defaultDisplaySchemaBarButtonItem;

	private BarButtonItem copyConnectionBarButtonItem;

	private BarButtonItem addViewBarButtonItem;

	private BarButtonItem addProcedureBarButtonItem;

	private BarButtonItem designProcedureBarButtonItem;

	private BarButtonItem duplicateModuleButtonItem;

	private BarButtonItem bulkAddViewBarButtonItem;

	private BarSubItem connectToExistingBarButtonItem;

	private BarButtonItem connectToBarButtonItem;

	private BarButtonItem createNewRepositoryBarButtonItem;

	private BarButtonItem addFunctionBarButtonItem;

	private BarButtonItem designFunctionBarButtonItem;

	private BarButtonItem clearTableProfilingBarButtonItem;

	private BarButtonItem sortModulesAlphabeticallyBarButtonItem;

	private BarButtonItem moveToTopBarButtonItem;

	private BarButtonItem moveToBottomBarButtonItem;

	private SharedObjectTypeEnum.ObjectType? selectedFormType;

	public EventHandler GetSuggestedDescriptions;

	public EventHandler<RecentProject> OpenRecentEvent;

	private DBTreeMenu dbTreeMenu;

	private SearchSupport searchSupport;

	private bool showProgress;

	private SemaphoreSlim searchMutex = new SemaphoreSlim(1, 1);

	private CancellationTokenSource searchCancellationTokenSource = new CancellationTokenSource();

	private bool lastOpenFromSearchResultsListGridView;

	private int lastOpenFromSearchResultsListGridViewRowHandle;

	private bool lastOpenFromSearchResultsTreeList;

	private TreeListNode lastOpenFromSearchResultsTreeListNode;

	private DBTreeNode lastFocusedNodeForModules;

	private BarManager treelistBarManager;

	private IContainer components;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ImageCollection treeMenuImageCollection;

	private ToolTipController dbTreeToolTipController;

	private BarButtonItem barButtonItem1;

	private BarButtonItem barButtonItem2;

	private BarButtonItem barButtonItem3;

	private BarButtonItem barButtonItem4;

	private ImageCollection deletedObjectsImageCollection;

	private ImageCollection treePopupImageCollection;

	private SaveFileDialog xlsxSaveFileDialog;

	private TreeList treeList1;

	private TreeList treeList2;

	private ToolTipController searchResultsListToolTipController;

	private SplitContainerControl metadataEditorSplitContainerControl;

	private RepositoryItemCheckedComboBoxEdit repositoryItemCheckedComboBoxEdit1;

	private RepositoryItemTextEdit repositoryItemTextEdit1;

	private CustomWithBordersXtraTabControl leftPanelXtraTabControl;

	private XtraTabPage repositoryXtraTabPage;

	private XtraTabPage searchXtraTabPage;

	private TreeList metadataTreeList;

	private TreeListColumn nameTreeListColumn;

	private RepositoryItemTextEdit titleRepositoryItemTextEdit;

	private TreeListColumn progressTreeListColumn;

	private RepositoryItemProgressBar progressDatabaseRepositoryItemProgressBar;

	private RepositoryItemProgressBar progressModuleRepositoryItemProgressBar;

	private RepositoryItemProgressBar progressFolderRepositoryItemProgressBar;

	private RepositoryItemProgressBar progressSimpleObjectRepositoryItemProgressBar;

	private XtraTabControl searchesXtraTabControl;

	private XtraTabPage searchResultsGridViewXtraTabPage;

	private GridControl searchResultsListGridControl;

	private GridColumn SearchResultsListIconGridColumn;

	private RepositoryItemPictureEdit repositoryItemPictureEdit1;

	private GridColumn SearchResultsListNameGridColumn;

	private RepositoryItemRichTextEdit SearchResultsListNameRepositoryItemRichTextEdit;

	private GridColumn SearchResultsListDocumentationNameGridColumn;

	private RepositoryItemRichTextEdit SearchResultsListNTitleRepositoryItemRichTextEdit;

	private GridColumn gridColumn1;

	private XtraTabPage searchResultsTreeListXtraTabPage;

	private TreeList searchResultsTreeList;

	private TreeListColumn NameSearchResultsTreeListColumn;

	private RepositoryItemRichTextEdit searchResultsTreeListNameRepositoryItemRichTextEdit;

	private CheckedComboBoxEdit TypesCheckedComboBoxEdit;

	private CheckedComboBoxEdit DocumentationsCheckedComboBoxEdit;

	private ProgressPanel searchProgressPanel;

	private Panel searchProgressPanelPanel;

	private SplashScreenManager splashScreenManager;

	private BannerControl bannerControl;

	private CustomGridUserControl searchResultsListGridView;

	private CustomGridUserControl InnerGrid;

	private NonCustomizableLayoutControl searchLayoutControl;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem layoutControlItem2;

	private AdvancedSearchPanel advancedSearchPanel;

	private SplitContainerControl advancedSearchSplitContainerControl;

	private LayoutControlItem layoutControlItem4;

	private TextEdit searchTextEdit;

	private LayoutControlItem layoutControlItem5;

	private RepositoryItemProgressBar loadingRepositoryItemProgressBar;

	public SplashScreenManager SplashScreenManager => splashScreenManager;

	public TreeListHelpers TreeListHelpers { get; private set; }

	public CustomFieldsSupport CustomFieldsSupport { get; private set; }

	public BusinessGlossarySupport BusinessGlossarySupport { get; private set; }

	public DataLinksSupport DataLinksSupport { get; private set; }

	public ProgressTypeModel ProgressType
	{
		get
		{
			if (progressType == null)
			{
				return new ProgressTypeModel(ProgressTypeEnum.AllDocumentations, "Descrtiptions")
				{
					FieldName = "Description"
				};
			}
			return progressType;
		}
		set
		{
			progressType = value;
			progressTreeListColumn.Caption = progressType.ColumnName;
			GetVisibleUserControl()?.VisibleGridView?.RefreshData();
		}
	}

	public bool GoingToNodeInProgress => moduleUserControl.GoingToNodeInProgress;

	public int ProgressSelectedCustomFieldId { get; set; }

	public SearchTreeNodeOperation SearchTreeNodeOperation { get; private set; }

	public bool ShowProgress
	{
		get
		{
			return showProgress;
		}
		set
		{
			progressTreeListColumn.Visible = (showProgress = value);
			BasePanelControl visibleUserControl = GetVisibleUserControl();
			if (showProgress)
			{
				visibleUserControl?.FillControlProgressHighlights();
			}
			else
			{
				visibleUserControl?.ClearTabsHighlights();
				visibleUserControl?.ClearControlProgressHighlights();
				databaseUserControl?.ClearControlProgressHighlights();
				moduleUserControl?.ClearControlProgressHighlights();
				tableUserControl?.ClearControlProgressHighlights();
				procedureUserControl?.ClearControlProgressHighlights();
				termUserControl?.ClearControlProgressHighlights();
			}
			visibleUserControl?.SetRichEditControlBackground();
			visibleUserControl?.SetTabsProgressHighlights();
			visibleUserControl?.VisibleGridView?.RefreshData();
			GetVisibleSummaryUserControl()?.RefreshData();
		}
	}

	public TreeList MetadataTreeList => metadataTreeList;

	public bool IsDocumentationCountLimitExceeded => metadataTreeList.Nodes.Select((TreeListNode x) => TreeListHelpers.GetNode(x)).Count((DBTreeNode y) => y.DatabaseId > 2) >= 1;

	public bool ShowSuggestions { get; internal set; } = true;


	public event EventHandler SelectedObjectChanged;

	public event EventHandler RefreshViewEvent;

	public event EventHandler ModuleERDTabSelectedEvent;

	public event EventHandler ModuleDescriptionTabSelectedEvent;

	public event EventHandler ModuleERDNodeSelectedEvent;

	public event EventHandler<int> ErdSavedEvent;

	public event EventHandler GoingToNodeEnded;

	public event EventHandler DependencyTreeListFocusedEvent;

	public event EventHandler AddRelationVisibilityEvent;

	public event EventHandler ProfileTableVisibilityEvent;

	public event EventHandler ProfileColumnVisibilityEvent;

	public event EventHandler EditRelationVisibilityEvent;

	public event EventHandler AddPrimaryKeyVisibilityEvent;

	public event EventHandler AddUniqueKeyVisibilityEvent;

	public event EventHandler EditConstraintVisibilityEvent;

	public event EventHandler RemoveConstraintVisibilityEvent;

	public event EventHandler SearchTabVisibilityEvent;

	public event EventHandler UserObjectsButtonsEnabledEvent;

	public event EventHandler RemoveDependencyVisibilityEvent;

	public event EventHandler RefreshBarButtonText;

	public event EventHandler AddRelatedTermVisibilityEvent;

	public event EventHandler AddDataLinkVisibilityEvent;

	public event EventHandler SaveButtonChangeEvent;

	public event EventHandler DataLineageTabVisibilityEvent;

	public event EventHandler DataLineageDiagramVisibilityEvent;

	public event EventHandler DataLineageColumnsVisibilityEvent;

	public event EventHandler<LoginFormPageEnum.LoginFormPage> ShowLoginFormEvent;

	public bool HasVisibleControlChanges()
	{
		if (metadataEditorSplitContainerControl.Panel2.Controls.Count == 0)
		{
			return false;
		}
		return GetSaveableUserControl()?.Edit.IsEdited ?? false;
	}

	public bool ContinueAfterPossibleChanges(Action ifChangedNoCancelAction = null, Action ifChangedNoAction = null)
	{
		if (!HasVisibleControlChanges())
		{
			return true;
		}
		ISaveable saveableUserControl = GetSaveableUserControl();
		DialogResult? dialogResult = null;
		dialogResult = ((!LastConnectionInfo.LOGIN_INFO.Autosave) ? GeneralMessageBoxesHandling.Show("There were changes. Do you want to save them?", "Save?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, parameters: new MessageBoxParameters
		{
			CheckBox = new CheckBoxParameters
			{
				Show = true,
				Title = "Save automatically from now on",
				OnResult = delegate(DialogResult result, bool value)
				{
					if (result == DialogResult.OK || result == DialogResult.Yes)
					{
						LastConnectionInfo.SetAutosave(value);
						this.SaveButtonChangeEvent?.Invoke(this, null);
					}
				}
			}
		}, owner: FindForm()).DialogResult : new DialogResult?(DialogResult.Yes));
		bool flag;
		switch (dialogResult)
		{
		case DialogResult.None:
		case DialogResult.Cancel:
			flag = false;
			break;
		case DialogResult.Yes:
		{
			GetVisibleUserControl()?.CloseEditors();
			bool num = Save();
			if (num)
			{
				ifChangedNoCancelAction?.Invoke();
			}
			flag = num;
			break;
		}
		case DialogResult.No:
			if (ifChangedNoCancelAction == null)
			{
				saveableUserControl.Edit.SetUnchanged();
				OpenPageControl();
			}
			else
			{
				ifChangedNoCancelAction?.Invoke();
			}
			ifChangedNoAction?.Invoke();
			flag = true;
			break;
		default:
			flag = true;
			break;
		}
		if (flag)
		{
			ClearSavedNodesTitle();
			if (dialogResult == DialogResult.No)
			{
				try
				{
					if (!(saveableUserControl is BasePanelControl))
					{
						return flag;
					}
					(saveableUserControl as BasePanelControl).RefreshModules();
					return flag;
				}
				catch (Exception exception)
				{
					GeneralExceptionHandling.Handle(exception, FindForm());
					return false;
				}
			}
		}
		return flag;
	}

	private void ClearSavedNodesTitle()
	{
		metadataTreeList.CloseEditor();
		SharedObjectTypeEnum.ObjectType? objectType = selectedFormType;
		if (objectType.HasValue)
		{
			switch (objectType.GetValueOrDefault())
			{
			case SharedObjectTypeEnum.ObjectType.Database:
				databaseUserControl.ClearSavedNodesTitle();
				break;
			case SharedObjectTypeEnum.ObjectType.Table:
			case SharedObjectTypeEnum.ObjectType.View:
			case SharedObjectTypeEnum.ObjectType.Structure:
				tableUserControl.ClearSavedNodesTitle();
				break;
			case SharedObjectTypeEnum.ObjectType.Module:
				moduleUserControl.ClearSavedNodesTitle();
				break;
			case SharedObjectTypeEnum.ObjectType.Function:
			case SharedObjectTypeEnum.ObjectType.Procedure:
				procedureUserControl.ClearSavedNodesTitle();
				break;
			case SharedObjectTypeEnum.ObjectType.Term:
				termUserControl.ClearSavedNodesTitle();
				break;
			}
		}
	}

	public MetadataEditorUserControl(bool loadData)
	{
		InitializeComponent();
		SkinsManager.SetPalette(searchesXtraTabControl.LookAndFeel);
		metadataTreeList.Appearance.FocusedCell.BackColor = SkinColors.FocusColorFromSystemColors;
		metadataTreeList.Appearance.FocusedCell.Options.UseBackColor = true;
		TreeListHelpers = new TreeListHelpers(this, metadataTreeList);
		dbTreeMenu = new DBTreeMenu(metadataTreeList, TreeListHelpers);
		BusinessGlossarySupport = new BusinessGlossarySupport(this, moduleSummaryUserControl, dbTreeMenu, metadataTreeList);
		DataLinksSupport = new DataLinksSupport(this, moduleSummaryUserControl, dbTreeMenu, metadataTreeList);
		ShowProgress = false;
		SearchTreeNodeOperation = new SearchTreeNodeOperation();
		Authentication.CreateAuthenticationDataSource();
		SnowflakeAuth.CreateAuthenticationDataSource();
		AuthenticationSSAS.CreateAuthenticationDataSource();
		AuthenticationTableau.CreateAuthenticationDataSource();
		Dataedo.App.Classes.TableauProduct.CreateTableauProductDataSource();
		Initialize(loadData);
		AddTreePopupMenu();
		LengthValidation.SetTitleOrNameLengthLimit(titleRepositoryItemTextEdit);
		UserViewData.LoadFromXML();
	}

	public void Initialize(bool loadData = true)
	{
		if (loadData)
		{
			CustomFieldsSupport = new CustomFieldsSupport();
			CustomFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: true);
			advancedSearchPanel.Initialize(CustomFieldsSupport);
			BusinessGlossarySupport.LoadTermRelationshipTypes();
			LoadOtherUserControls();
			searchSupport = new SearchSupport();
			RefreshTree(showWaitForm: false);
		}
	}

	public void LoadCustomFields(bool loadDefinitionValues)
	{
		bool isSplashFormVisible = splashScreenManager.IsSplashFormVisible;
		if (!isSplashFormVisible)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
		}
		CustomFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: true);
		if (!isSplashFormVisible)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private void MetadataEditorUserControl_Load(object sender, EventArgs e)
	{
		DBTreeNode focusedNode = GetFocusedNode();
		if (focusedNode != null)
		{
			this.SelectedObjectChanged?.Invoke(null, new ObjectTypeEventArgs(focusedNode));
		}
		else
		{
			this.SelectedObjectChanged?.Invoke(null, null);
		}
		LoadUserControls();
	}

	public void ShowSynchFormWhenDefaultRepository()
	{
		if (dbTreeMenu.OnlyDefaultRepository())
		{
			AddDatabase();
		}
	}

	public void ShowRelationAndConstraintButtons()
	{
		if (tableUserControl != null)
		{
			tableUserControl.ShowRelationButtons();
			tableUserControl.ShowConstraintButtons();
			tableUserControl.ShowDataLinkButtons();
		}
	}

	public void ShowDataLinksButtons()
	{
		if (tableUserControl != null)
		{
			tableUserControl.ShowDataLinkButtons();
		}
	}

	public void LoadUserControls()
	{
		SetBannerFunctionality();
		functionSummaryUserControl = new ProcedureSummaryUserControl(SharedObjectTypeEnum.ObjectType.Function);
		functionSummaryUserControl.TreeMenu = dbTreeMenu;
		functionSummaryUserControl.MainControl = this;
		functionSummaryUserControl.ShowProcedureControl += delegate(object sender2, EventArgs e2)
		{
			ShowObjectUserControl(SharedObjectTypeEnum.ObjectType.Function, CustomFieldsSupport, e2 as ObjectEventArgs);
		};
		procedureSummaryUserControl = new ProcedureSummaryUserControl(SharedObjectTypeEnum.ObjectType.Procedure);
		procedureSummaryUserControl.TreeMenu = dbTreeMenu;
		procedureSummaryUserControl.MainControl = this;
		procedureSummaryUserControl.ShowProcedureControl += delegate(object sender2, EventArgs e2)
		{
			ShowObjectUserControl(SharedObjectTypeEnum.ObjectType.Procedure, CustomFieldsSupport, e2 as ObjectEventArgs);
		};
		tableSummaryUserControl = new TableSummaryUserControl(SharedObjectTypeEnum.ObjectType.Table);
		tableSummaryUserControl.TreeMenu = dbTreeMenu;
		tableSummaryUserControl.MainControl = this;
		tableSummaryUserControl.ShowTableControl += delegate(object sender2, EventArgs e2)
		{
			ShowObjectUserControl(SharedObjectTypeEnum.ObjectType.Table, CustomFieldsSupport, e2 as ObjectEventArgs);
		};
		viewSummaryUserControl = new TableSummaryUserControl(SharedObjectTypeEnum.ObjectType.View);
		viewSummaryUserControl.TreeMenu = dbTreeMenu;
		viewSummaryUserControl.MainControl = this;
		viewSummaryUserControl.ShowTableControl += delegate(object sender2, EventArgs e2)
		{
			ShowObjectUserControl(SharedObjectTypeEnum.ObjectType.View, CustomFieldsSupport, e2 as ObjectEventArgs);
		};
		objectSummaryUserControl = new TableSummaryUserControl(SharedObjectTypeEnum.ObjectType.Structure);
		objectSummaryUserControl.TreeMenu = dbTreeMenu;
		objectSummaryUserControl.MainControl = this;
		objectSummaryUserControl.ShowTableControl += delegate(object sender2, EventArgs e2)
		{
			ShowObjectUserControl(SharedObjectTypeEnum.ObjectType.Structure, CustomFieldsSupport, e2 as ObjectEventArgs);
		};
		moduleSummaryUserControl = new ModuleSummaryUserControl(this);
		moduleSummaryUserControl.TreeMenu = dbTreeMenu;
		moduleSummaryUserControl.MainControl = this;
		moduleSummaryUserControl.ShowModuleControl += delegate(object sender2, EventArgs e2)
		{
			ShowObjectUserControl(SharedObjectTypeEnum.ObjectType.Module, CustomFieldsSupport, e2 as ObjectEventArgs);
		};
		termSummaryUserControl = new TermSummaryUserControl();
		termSummaryUserControl.TreeMenu = dbTreeMenu;
		termSummaryUserControl.MainControl = this;
		termSummaryUserControl.ShowTermControl += delegate(object sender2, EventArgs e2)
		{
			TreeListNode treeListNode = SearchTreeNodeOperation.FindNode(metadataTreeList.Nodes, (e2 as ObjectEventArgs).DatabaseId, (e2 as ObjectEventArgs).ObjectId, SharedObjectTypeEnum.ObjectType.Term);
			metadataTreeList.FocusedNode = treeListNode;
			OpenPageControl(showControl: true, TreeListHelpers.GetNode(treeListNode));
		};
		upgradeBusinessGlossaryControl = new UpgradeBusinessGlossaryControl();
		advancedSearchPanel.CustomFieldAdded += SetAdvancedSearchSplitterPositionAfterAdd;
		advancedSearchPanel.CustomFieldRemoved += delegate
		{
			SetAdvancedSearchSplitterPositionAfterRemove();
		};
		advancedSearchPanel.Search += CallSearch;
		TreeListHelpers.InitializeControls(tableUserControl, tableSummaryUserControl, viewSummaryUserControl, objectSummaryUserControl, procedureSummaryUserControl, functionSummaryUserControl, moduleSummaryUserControl, termSummaryUserControl);
	}

	private void SetAdvancedSearchSplitterPositionAfterAdd()
	{
		int actualHeight = advancedSearchPanel.GetActualHeight();
		if (advancedSearchSplitContainerControl.SplitterPosition < actualHeight)
		{
			advancedSearchSplitContainerControl.SplitterPosition = actualHeight;
		}
	}

	private void SetAdvancedSearchSplitterPositionAfterRemove()
	{
		int actualHeight = advancedSearchPanel.GetActualHeight();
		if (advancedSearchSplitContainerControl.SplitterPosition > actualHeight)
		{
			advancedSearchSplitContainerControl.SplitterPosition = actualHeight;
		}
	}

	public void SetBannerFunctionality()
	{
		bannerControl.SetFunctionality();
	}

	public void LoadOtherUserControls()
	{
		homePanelUserControl = new HomePanelUserControl(this);
		homePanelUserControl.AddDatabaseButtonClick += delegate
		{
			AddDatabase();
		};
		homePanelUserControl.ExportDocumentationsButtonClick += delegate
		{
			ShowDocCreator();
		};
		homePanelUserControl.ExportDocumentationButtonClick += delegate(object s, int e)
		{
			TreeListNode treeListNode2 = SearchTreeNodeOperation.FindNode(metadataTreeList.Nodes.FirstNode.Nodes, e, SharedObjectTypeEnum.ObjectType.Database, SharedObjectTypeEnum.ObjectType.BusinessGlossary);
			ShowDocCreator(TreeListHelpers.GetNode(treeListNode2));
		};
		homePanelUserControl.ImportChangesButtonClick += delegate(object s, int e)
		{
			if (ContinueAfterPossibleChanges())
			{
				TreeListNode treeListNode = SearchTreeNodeOperation.FindNode(metadataTreeList.Nodes.FirstNode.Nodes, e, SharedObjectTypeEnum.ObjectType.Database, SharedObjectTypeEnum.ObjectType.BusinessGlossary);
				DBTreeNode node = TreeListHelpers.GetNode(treeListNode);
				if (node != null && node.DatabaseType.HasValue)
				{
					Synchronize(fromCustomFocus: false, node);
				}
			}
		};
		homePanelUserControl.LinkClick += delegate(object s, string e)
		{
			Links.OpenLink(e);
		};
		homePanelUserControl.LinkToObjectClick += delegate(object s, LinkToObjectByIdModel e)
		{
			SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(e.ObjectType);
			SharedObjectTypeEnum.ObjectType? type = objectType;
			if (objectType.HasValue)
			{
				if (objectType == SharedObjectTypeEnum.ObjectType.Erd)
				{
					objectType = SharedObjectTypeEnum.ObjectType.Module;
				}
				TreeListHelpers.OpenNode(e.DatabaseId, e.ObjectId, objectType.Value);
				(GetVisibleUserControl() as ITabChangable)?.ChangeTab(type);
			}
		};
		databaseUserControl = new DatabaseUserControl(this);
		databaseUserControl.TreeMenu = dbTreeMenu;
		procedureUserControl = new ProcedureUserControl(this);
		procedureUserControl.TreeMenu = dbTreeMenu;
		procedureUserControl.AddModuleEvent += delegate(object sender, EventArgs e)
		{
			AddModuleFromComboControl((e as IdEventArgs).DatabaseId);
		};
		tableUserControl = new TableUserControl(this);
		tableUserControl.TreeMenu = dbTreeMenu;
		tableUserControl.AddModuleEvent += delegate(object sender, EventArgs e)
		{
			AddModuleFromComboControl((e as IdEventArgs).DatabaseId);
		};
		moduleUserControl = new ModuleUserControl(this);
		moduleUserControl.TreeMenu = dbTreeMenu;
		moduleUserControl.ERDTabPageSelectedEvent += delegate(object sender, EventArgs e)
		{
			SetERDGroupVisibility(e as BoolEventArgs);
		};
		moduleUserControl.DeleteSelectedERDNode += delegate(object sender, EventArgs e)
		{
			SetERDRemoveButtonVisibility(e as ErdButtonArgs);
		};
		moduleUserControl.ErdSavedEvent += delegate(object sender, int e)
		{
			this.ErdSavedEvent?.Invoke(sender, e);
		};
		moduleUserControl.GoingToNodeEnded += delegate(object sender, EventArgs e)
		{
			this.GoingToNodeEnded?.Invoke(sender, e);
		};
		termUserControl = new TermUserControl(this);
		termUserControl.TreeMenu = dbTreeMenu;
	}

	internal bool IsLinkToObjectValid(LinkToObjectByIdModel linkToObjectByIdModel)
	{
		SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(linkToObjectByIdModel.ObjectType);
		if (!objectType.HasValue)
		{
			return false;
		}
		if (objectType == SharedObjectTypeEnum.ObjectType.Erd)
		{
			objectType = SharedObjectTypeEnum.ObjectType.Module;
		}
		return SearchTreeNodeOperation.FindNode(metadataTreeList.Nodes.FirstNode.Nodes, linkToObjectByIdModel.DatabaseId, linkToObjectByIdModel.ObjectId, objectType.Value) != null;
	}

	internal void SetMoveDependencyButtonsVisibility(BoolEventArgs visibility)
	{
		this.DependencyTreeListFocusedEvent?.Invoke(null, visibility);
	}

	internal void SetRemoveDependencyButtonVisibility(BoolEventArgs visibility)
	{
		this.RemoveDependencyVisibilityEvent?.Invoke(null, visibility);
	}

	internal void SetAddRelationButtonVisibility(BoolEventArgs visibility)
	{
		BoolEventArgs e = PrepareArgsForRelationAndConstraint(visibility);
		this.AddRelationVisibilityEvent?.Invoke(null, e);
	}

	internal void SetProfileTableButtonVisibility(ProfilingVisibilityEventArgs visibility)
	{
		if (DB.DataProfiling.IsDataProfilingDisabled())
		{
			visibility = new ProfilingVisibilityEventArgs(singleObjectButtonsVisible: false, null, dataProfilingButtonVisible: false);
		}
		this.ProfileTableVisibilityEvent?.Invoke(null, visibility);
	}

	internal void SetProfileColumnButtonVisibility(BoolEventArgs visibility)
	{
		if (DB.DataProfiling.IsDataProfilingDisabled())
		{
			this.ProfileColumnVisibilityEvent?.Invoke(null, new BoolEventArgs(value: false));
		}
		else
		{
			this.ProfileColumnVisibilityEvent?.Invoke(null, visibility);
		}
	}

	internal void SetEditRelationButtonVisibility(BoolEventArgs visibility)
	{
		BoolEventArgs e = PrepareArgsForRelationAndConstraint(visibility);
		this.EditRelationVisibilityEvent?.Invoke(null, e);
	}

	internal void SetAddPrimaryKeyButtonVisibility(BoolEventArgs visibility)
	{
		this.AddPrimaryKeyVisibilityEvent?.Invoke(null, visibility);
	}

	internal void SetAddUniqueKeyButtonVisibility(BoolEventArgs visibility)
	{
		this.AddUniqueKeyVisibilityEvent?.Invoke(null, visibility);
	}

	internal void SetEditConstraintButtonVisibility(BoolEventArgs visibility)
	{
		BoolEventArgs e = PrepareArgsForRelationAndConstraint(visibility);
		this.EditConstraintVisibilityEvent?.Invoke(null, e);
	}

	internal void SetRemoveConstraintButtonVisibility(BoolEventArgs visibility)
	{
		BoolEventArgs e = PrepareArgsForRelationAndConstraint(visibility);
		this.RemoveConstraintVisibilityEvent?.Invoke(null, e);
	}

	internal void SetUserObjectsButtonsEnabled(BoolEventArgs enabled)
	{
		BoolEventArgs e = PrepareArgsForRelationAndConstraint(enabled);
		this.UserObjectsButtonsEnabledEvent?.Invoke(null, e);
	}

	internal void SetAddRelatedTermButtonVisibility(BoolEventArgs visibility)
	{
		BoolEventArgs e = PrepareArgsForRelationAndConstraint(visibility);
		this.AddRelatedTermVisibilityEvent?.Invoke(null, e);
	}

	internal void SetAddDataLinkButtonVisibility(BoolEventArgs visibility)
	{
		BoolEventArgs e = PrepareArgsForBusinessGlossary(visibility);
		this.AddDataLinkVisibilityEvent?.Invoke(null, e);
	}

	internal void RefreshBarButtonItemText()
	{
		this.RefreshBarButtonText?.Invoke(null, null);
	}

	private BoolEventArgs PrepareArgsForRelationAndConstraint(BoolEventArgs visibility)
	{
		bool flag = GetVisibleUserControl() == tableUserControl;
		return new BoolEventArgs(visibility.Value && flag);
	}

	private BoolEventArgs PrepareArgsForBusinessGlossary(BoolEventArgs visibility)
	{
		bool flag = GetVisibleUserControl() == tableUserControl;
		return new BoolEventArgs(visibility.Value && flag && Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary));
	}

	public BasePanelControl GetVisibleUserControl()
	{
		BasePanelControl result = null;
		if (metadataEditorSplitContainerControl.Panel2.Controls.Count > 0)
		{
			result = metadataEditorSplitContainerControl.Panel2.Controls[0] as BasePanelControl;
		}
		return result;
	}

	public BaseSummaryUserControl GetVisibleSummaryUserControl()
	{
		BaseSummaryUserControl result = null;
		if (metadataEditorSplitContainerControl.Panel2.Controls.Count > 0)
		{
			result = metadataEditorSplitContainerControl.Panel2.Controls[0] as BaseSummaryUserControl;
		}
		return result;
	}

	private ISaveable GetSaveableUserControl()
	{
		ISaveable result = null;
		if (metadataEditorSplitContainerControl.Panel2.Controls.Count > 0)
		{
			result = metadataEditorSplitContainerControl.Panel2.Controls[0] as ISaveable;
		}
		return result;
	}

	internal void SetERDGroupVisibility(BoolEventArgs visibility)
	{
		this.ModuleERDTabSelectedEvent?.Invoke(null, visibility);
	}

	internal void SetModulePositionButtonsVisibility(BoolEventArgs visibility)
	{
		this.ModuleDescriptionTabSelectedEvent?.Invoke(null, visibility);
	}

	public void SetERDRemoveButtonVisibility(ErdButtonArgs e)
	{
		this.ModuleERDNodeSelectedEvent?.Invoke(null, e);
	}

	private void AddTreePopupMenu()
	{
		treeBarManager = new BarManager();
		treeBarManager.ShowScreenTipsInMenus = true;
		treeBarManager.ToolTipController = dbTreeToolTipController;
		treeBarManager.Form = this;
		treeBarManager.Images = treePopupImageCollection;
		treePopupMenu = new PopupMenu(treeBarManager);
		treePopupMenu.CloseUp += delegate
		{
			TreeListNode customFocusedTreeListNode = TreeListHelpers.CustomFocusedTreeListNode;
			TreeListHelpers.CustomFocusedTreeListNode = null;
			metadataTreeList.RefreshNode(customFocusedTreeListNode);
		};
		treePopupMenu.ItemLinks.Clear();
		addModuleBarButtonItem = new BarButtonItem(treeBarManager, "Add Subject Area", 0);
		addModuleBarButtonItem.ItemClick += addModuleBarButtonItem_ItemClick;
		addTermBarButtonItem = new BarButtonItem(treeBarManager, "Add term", 11);
		addTermBarButtonItem.ItemClick += BusinessGlossarySupport.AddTermBarButtonItem_ItemClick;
		addTermRelatedTermBarButtonItem = new BarButtonItem(treeBarManager, "Edit term relationships", 12);
		addTermRelatedTermBarButtonItem.ItemClick += BusinessGlossarySupport.AddTermRelatedTermBarButtonItem_ItemClick;
		addDataLinkBarButtonItem = new BarButtonItem(treeBarManager, "Edit links to Business Glossary terms", 13);
		addDataLinkBarButtonItem.ItemClick += DataLinksSupport.AddDataLinkBarButtonItem_ItemClick;
		refreshButtonItem = new BarButtonItem(treeBarManager, "Refresh view", 1);
		refreshButtonItem.ItemClick += refreshButtonItem_ItemClick;
		deleteBarButtonItem = new BarButtonItem(treeBarManager, "Remove from repository", 2);
		deleteBarButtonItem.ItemClick += deleteBarButtonItem_ItemClick;
		synchronizeSchemaBarButtonItem = new BarButtonItem(treeBarManager, "Import changes", 3);
		synchronizeSchemaBarButtonItem.ItemClick += synchronizeSchemaBarButtonItem_ItemClick;
		importDescriptionsBarButtonItem = new BarButtonItem(treeBarManager, "Import descriptions", 34);
		importDescriptionsBarButtonItem.ItemClick += ImportDescriptionsBarButtonItem_ItemClick;
		generateDocumentationBarButtonItem = new BarButtonItem(treeBarManager, "Export", 23);
		generateDocumentationBarButtonItem.ItemClick += generateDocumentationBarButtonItem_ItemClick;
		renameBarButtonItem = new BarButtonItem(treeBarManager, "Rename", 5);
		renameBarButtonItem.ItemClick += renameBarButtonItem_ItemClick;
		moveDownBarButtonItem = new BarButtonItem(treeBarManager, "Move down", 6);
		moveDownBarButtonItem.ItemClick += moveDownBarButtonItem_ItemClick;
		moveUpBarButtonItem = new BarButtonItem(treeBarManager, "Move up", 7);
		moveUpBarButtonItem.ItemClick += moveUpBarButtonItem_ItemClick;
		designTableBarButtonItem = new BarButtonItem(treeBarManager, "Design table", 5);
		designTableBarButtonItem.ItemClick += editBarButtonItem_ItemClick;
		profileTableBarButtonItem = new BarButtonItem(treeBarManager, "Profile Table", 27);
		profileTableBarButtonItem.ItemClick += ProfileTableBarButtonItem_ItemClick;
		dataProfilingBarButtonItem = new BarButtonItem(treeBarManager, "Data Profiling", 39);
		dataProfilingBarButtonItem.ItemClick += DataProfilingBarButtonItem_ItemClick;
		previewSampleDataBarButtonItem = new BarButtonItem(treeBarManager, "Preview Sample Data", 35);
		previewSampleDataBarButtonItem.ItemClick += PreviewSampleDataBarButtonItem_ItemClick;
		addImportStructureBarButtonItem = new BarButtonItem(treeBarManager, "Add/Import Structure/File", 21);
		addImportStructureBarButtonItem.ItemClick += addImportStructureBarButtonItem_ItemClick;
		addTableEntityBarButtonItem = new BarButtonItem(treeBarManager, "Add Table/Entity", 8);
		addTableEntityBarButtonItem.ItemClick += addTableEntityBarButtonItem_ItemClick;
		bulkAddObjectsBarButtonItem = new BarSubItem(treeBarManager, "Bulk Add Objects", 20);
		bulkAddTableBarButtonItem = new BarButtonItem(treeBarManager, "Bulk Add Tables", 9);
		bulkAddTableBarButtonItem.ItemClick += bulkAddTableBarButtonItem_ItemClick;
		bulkAddStructureBarButtonItem = new BarButtonItem(treeBarManager, "Bulk Add Structures", 22);
		bulkAddStructureBarButtonItem.ItemClick += bulkAddStructureBarButtonItem_ItemClick;
		bulkAddViewBarButtonItem = new BarButtonItem(treeBarManager, "Bulk Add Views", 31);
		bulkAddViewBarButtonItem.ItemClick += BulkAddViewBarButtonItem_ItemClick;
		addNewTermBarButtonItem = new BarButtonItem(treeBarManager, "Add new linked Business Glossary term", 11);
		addObjectBarButtonItem = new BarSubItem(treeBarManager, "Add Object", 20);
		copyDescriptionsBarButtonItem = new BarButtonItem(treeBarManager, "Copy descriptions", 10);
		copyDescriptionsBarButtonItem.ItemClick += copyDescriptions_ItemClick;
		expandAllBarButtonItem = new BarButtonItem(treeBarManager, "Expand all", 14);
		expandAllBarButtonItem.ItemClick += ExpandAllBarButtonItem_ItemClick;
		collapseAllBarButtonItem = new BarButtonItem(treeBarManager, "Collapse all", 15);
		collapseAllBarButtonItem.ItemClick += CollapseAllBarButtonItem_ItemClick;
		addBarButtonItem = new BarSubItem(treeBarManager, "Add source", 16);
		addDatabaseConnectionBarButtonItem = new BarButtonItem(treeBarManager, "New connection", 17);
		addDatabaseConnectionBarButtonItem.ItemClick += AddDatabaseConnectionBarButtonItem_ItemClick;
		addManualDatabaseBarButtonItem = new BarButtonItem(treeBarManager, "Manual database", 18);
		addManualDatabaseBarButtonItem.ItemClick += AddManualDatabaseBarButtonItem_ItemClick;
		addBusinessGlossaryBarButtonItem = new BarButtonItem(treeBarManager, "Business Glossary", 19);
		addBusinessGlossaryBarButtonItem.AllowHtmlText = DefaultBoolean.True;
		addBusinessGlossaryBarButtonItem.ItemClick += AddBusinessGlossaryBarButtonItem_ItemClick;
		addBarButtonItem.AddItem(addDatabaseConnectionBarButtonItem);
		addBarButtonItem.AddItem(addManualDatabaseBarButtonItem);
		addBarButtonItem.AddItem(addBusinessGlossaryBarButtonItem);
		displaySchemaOptionsBarSubItem = new BarSubItem(treeBarManager, "Display schema");
		alwaysDisplaySchemaBarButtonItem = new BarButtonItem(treeBarManager, "Always");
		alwaysDisplaySchemaBarButtonItem.ItemClick += AlwaysDisplaySchemaBarButtonItem_ItemClick;
		neverDisplaySchemaBarButtonItem = new BarButtonItem(treeBarManager, "Never");
		neverDisplaySchemaBarButtonItem.ItemClick += NeverDisplaySchemaBarButtonItem_ItemClick;
		defaultDisplaySchemaBarButtonItem = new BarButtonItem(treeBarManager, "Default");
		defaultDisplaySchemaBarButtonItem.ItemClick += DefaultDisplaySchemaBarButtonItem_ItemClick;
		connectToExistingBarButtonItem = new BarSubItem(treeBarManager, "Recent repositories", 25);
		connectToBarButtonItem = new BarButtonItem(treeBarManager, "Change repository...", 25);
		connectToBarButtonItem.ItemClick += ConnectToBarButtonItem_ItemClick;
		createNewRepositoryBarButtonItem = new BarButtonItem(treeBarManager, "Create new repository", 26);
		createNewRepositoryBarButtonItem.ItemClick += CreateNewRepositoryBarButtonItem_ItemClick;
		copyConnectionBarButtonItem = new BarButtonItem(treeBarManager, "Copy connection");
		copyConnectionBarButtonItem.ItemClick += CopyConnectionBarButtonItem_ItemClick;
		addViewBarButtonItem = new BarButtonItem(treeBarManager, "Add view", 29);
		addViewBarButtonItem.ItemClick += AddViewBarButtonItem_ItemClick;
		duplicateModuleButtonItem = new BarButtonItem(treeBarManager, "Copy Subject Area", 30);
		duplicateModuleButtonItem.ItemClick += DuplicateModuleButtonItem_ItemClick;
		addProcedureBarButtonItem = new BarButtonItem(treeBarManager, "Add Procedure", 33);
		addProcedureBarButtonItem.ItemClick += AddProcedureBarButtonItem_ItemClick;
		designProcedureBarButtonItem = new BarButtonItem(treeBarManager, "Design Procedure", 5);
		designProcedureBarButtonItem.ItemClick += DesignProcedureBarButtonItem_ItemClick;
		addFunctionBarButtonItem = new BarButtonItem(treeBarManager, "Add Function", 32);
		addFunctionBarButtonItem.ItemClick += AddFunctionBarButtonItem_ItemClick;
		designFunctionBarButtonItem = new BarButtonItem(treeBarManager, "Design Function", 5);
		designFunctionBarButtonItem.ItemClick += DesignFunctionBarButtonItem_ItemClick;
		clearTableProfilingBarButtonItem = new BarButtonItem(treeBarManager, "Clear all Profiling Data", 28);
		clearTableProfilingBarButtonItem.ItemClick += ClearTableProfilingBarButtonItem_ItemClick;
		sortModulesAlphabeticallyBarButtonItem = new BarButtonItem(treeBarManager, "Sort alphabetically", 36);
		sortModulesAlphabeticallyBarButtonItem.ItemClick += SortModulesAlphabeticallyBarButtonItem_ItemClick;
		moveToTopBarButtonItem = new BarButtonItem(treeBarManager, "Move to top", 37);
		moveToTopBarButtonItem.ItemClick += MoveToTopBarButtonItem_ItemClick;
		moveToBottomBarButtonItem = new BarButtonItem(treeBarManager, "Move to bottom", 38);
		moveToBottomBarButtonItem.ItemClick += MoveToBottomBarButtonItem_ItemClick;
	}

	private void MoveToBottomBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		MoveModuleToTheBottom(fromCustomFocus: true);
	}

	private void MoveToTopBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		MoveModuleToTheTop(fromCustomFocus: true);
	}

	private void SortModulesAlphabeticallyBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		SortModulesAlphabetically(fromCustomFocus: true);
	}

	private void DesignFunctionBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		EditProcedure(TreeListHelpers.IsCustomFocus);
	}

	private void AddFunctionBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddFunction(fromCustomFocus: true);
	}

	private void BulkAddViewBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		BulkAddTables(SharedObjectTypeEnum.ObjectType.View, fromCustomFocus: true);
	}

	private void DesignProcedureBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		EditProcedure(TreeListHelpers.IsCustomFocus);
	}

	private void AddProcedureBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddProcedure(fromCustomFocus: true);
	}

	private void AddViewBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddView(fromCustomFocus: true);
	}

	private void CopyConnectionBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddDatabase(isCopyingConnection: true, fromCustomFocus: true);
	}

	private void CreateNewRepositoryBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		this.ShowLoginFormEvent?.Invoke(this, LoginFormPageEnum.LoginFormPage.CreateNewRepository);
	}

	private void ConnectToBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		this.ShowLoginFormEvent?.Invoke(this, LoginFormPageEnum.LoginFormPage.ConnectToRepository);
	}

	private void DuplicateModuleButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		DuplicateModule();
	}

	private void ImportDescriptionsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (ContinueAfterPossibleChanges())
		{
			ChooseImportTypeForm chooseImportTypeForm = new ChooseImportTypeForm();
			chooseImportTypeForm.Initialize(this);
			if (chooseImportTypeForm.ShowDialog() == DialogResult.OK)
			{
				RefreshSubnodes(sender, e);
				GetVisibleUserControl()?.SetParameters(GetFocusedNode(), CustomFieldsSupport);
			}
		}
	}

	private void DefaultDisplaySchemaBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		DBTreeNode node = TreeListHelpers.GetNode(TreeListHelpers.CustomFocusedTreeListNode);
		DB.Database.UpdateDocumentationShowSchemaOverride(node.DatabaseId, null);
		RefreshNames(node, null);
	}

	private void NeverDisplaySchemaBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		DBTreeNode node = TreeListHelpers.GetNode(TreeListHelpers.CustomFocusedTreeListNode);
		DB.Database.UpdateDocumentationShowSchemaOverride(node.DatabaseId, false);
		RefreshNames(node, false);
	}

	private void AlwaysDisplaySchemaBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		DBTreeNode node = TreeListHelpers.GetNode(TreeListHelpers.CustomFocusedTreeListNode);
		DB.Database.UpdateDocumentationShowSchemaOverride(node.DatabaseId, true);
		RefreshNames(node, true);
	}

	private void RefreshNames(DBTreeNode selectedNode, bool? showSchemaOverride)
	{
		selectedNode.DatabaseShowSchemaOverride = showSchemaOverride;
		selectedNode.SetName();
		foreach (DBTreeNode node in selectedNode.Nodes)
		{
			RefreshNames(node, showSchemaOverride);
		}
	}

	public void AddDatabaseConnectionBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (ContinueAfterPossibleChanges())
		{
			AddDatabase();
		}
	}

	public void AddManualDatabaseBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		StartAddingNewDatabase();
	}

	public void AddBusinessGlossaryBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (ContinueAfterPossibleChanges())
		{
			BusinessGlossarySupport.StartAddingNewBusinessGlossary(openNameEditor: true, FindForm());
		}
	}

	private void copyDescriptions_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (ContinueAfterPossibleChanges())
		{
			GeneralMessageBoxesHandling.Show("Copying documentations is irreversible." + Environment.NewLine + "We strongly recommend to <b>backup your repository</b> before continuing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
			DBTreeNode focusedNode = GetFocusedNode();
			DocWizardForm docWizardForm = new DocWizardForm(focusedNode.DatabaseId, focusedNode.ObjectType, focusedNode.DatabaseType, this);
			docWizardForm.SetCopyDescriptionsPage();
			docWizardForm.ShowDialog(this, setCustomMessageDefaultOwner: true);
			StaticData.ClearDatabaseInfoForCrashes();
			if (docWizardForm.HasCopiedInCurrentRepository())
			{
				RefreshTree(showWaitForm: true, progressType != null && ShowProgress);
			}
		}
	}

	private void moveUpBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		MoveUpModule(fromCustomFocus: true);
	}

	private void moveDownBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		MoveDownModule(fromCustomFocus: true);
	}

	public TreeListNode MoveUpNode(int moduleId)
	{
		TreeListNode treeListNode = metadataTreeList.FindNodes((TreeListNode x) => TreeListHelpers.GetDBTreeNode(x).ObjectType == SharedObjectTypeEnum.ObjectType.Module && TreeListHelpers.GetDBTreeNode(x).Id == moduleId).FirstOrDefault();
		if (treeListNode != null)
		{
			metadataTreeList.SetNodeIndex(treeListNode, metadataTreeList.GetNodeIndex(treeListNode) - 1);
		}
		return treeListNode;
	}

	public TreeListNode MoveDownNode(int moduleId)
	{
		TreeListNode treeListNode = metadataTreeList.FindNodes((TreeListNode x) => TreeListHelpers.GetDBTreeNode(x).ObjectType == SharedObjectTypeEnum.ObjectType.Module && TreeListHelpers.GetDBTreeNode(x).Id == moduleId).FirstOrDefault();
		if (treeListNode != null)
		{
			metadataTreeList.SetNodeIndex(treeListNode, metadataTreeList.GetNodeIndex(treeListNode) + 1);
		}
		return treeListNode;
	}

	public TreeListNode MoveUpNode(bool fromCustomFocus = false)
	{
		TreeListNode treeListNode = (fromCustomFocus ? TreeListHelpers.CustomFocusedTreeListNode : metadataTreeList.FocusedNode);
		if (treeListNode != null)
		{
			metadataTreeList.SetNodeIndex(treeListNode, metadataTreeList.GetNodeIndex(treeListNode) - 1);
		}
		return treeListNode;
	}

	public TreeListNode MoveDownNode(bool fromCustomFocus = false)
	{
		TreeListNode treeListNode = (fromCustomFocus ? TreeListHelpers.CustomFocusedTreeListNode : metadataTreeList.FocusedNode);
		if (treeListNode != null)
		{
			metadataTreeList.SetNodeIndex(treeListNode, metadataTreeList.GetNodeIndex(treeListNode) + 1);
		}
		return treeListNode;
	}

	internal void MoveUpModule(bool fromCustomFocus = false)
	{
		metadataTreeList.CloseEditor();
		GetVisibleSummaryUserControl()?.CloseEditor();
		TreeListNode treeListNode = MoveUpNode(fromCustomFocus);
		if (treeListNode != null)
		{
			ActualizeModulesOrder(treeListNode);
			BaseSummaryUserControl visibleSummaryUserControl = GetVisibleSummaryUserControl();
			if (visibleSummaryUserControl != null && visibleSummaryUserControl is ModuleSummaryUserControl)
			{
				(visibleSummaryUserControl as ModuleSummaryUserControl).ReloadRows();
			}
		}
	}

	internal void MoveDownModule(bool fromCustomFocus = false)
	{
		metadataTreeList.CloseEditor();
		GetVisibleSummaryUserControl()?.CloseEditor();
		TreeListNode treeListNode = MoveDownNode(fromCustomFocus);
		if (treeListNode != null)
		{
			ActualizeModulesOrder(treeListNode);
			BaseSummaryUserControl visibleSummaryUserControl = GetVisibleSummaryUserControl();
			if (visibleSummaryUserControl != null && visibleSummaryUserControl is ModuleSummaryUserControl)
			{
				(visibleSummaryUserControl as ModuleSummaryUserControl).ReloadRows();
			}
		}
	}

	internal void EditTable(bool fromCustomFocus = false)
	{
		if (!ContinueAfterPossibleChanges())
		{
			return;
		}
		DBTreeNode focusedNode = GetFocusedNode(fromCustomFocus);
		DBTreeNode focusedNode2 = GetFocusedNode();
		if (((focusedNode == null || focusedNode.ObjectType != SharedObjectTypeEnum.ObjectType.Table) && (focusedNode == null || focusedNode.ObjectType != SharedObjectTypeEnum.ObjectType.Structure) && (focusedNode == null || focusedNode.ObjectType != SharedObjectTypeEnum.ObjectType.View)) || focusedNode.IsFolder)
		{
			return;
		}
		try
		{
			if (!fromCustomFocus)
			{
				tableUserControl.ClearTabPageTitle();
				tableUserControl.SetDisableSettingAsEdited(value: true);
			}
			DesignTableForm designTableForm = new DesignTableForm(focusedNode, CustomFieldsSupport);
			designTableForm.ShowDialog();
			tableUserControl.isTableEdited = false;
			if ((focusedNode2 != null && focusedNode2.ObjectType == SharedObjectTypeEnum.ObjectType.Table) || (focusedNode2 != null && focusedNode2.ObjectType == SharedObjectTypeEnum.ObjectType.Structure) || (focusedNode2 != null && focusedNode2.ObjectType == SharedObjectTypeEnum.ObjectType.View))
			{
				if (designTableForm.DialogResult == DialogResult.OK && GetVisibleUserControl() == tableUserControl && focusedNode.ObjectType == focusedNode2.ObjectType && focusedNode.Id == focusedNode2.Id)
				{
					focusedNode.Source = designTableForm.TableDesigner.Source;
					tableUserControl.SetSource(designTableForm.TableDesigner.Source);
				}
				if (designTableForm.DialogResult == DialogResult.OK && GetVisibleUserControl() == tableUserControl)
				{
					tableUserControl.SetTitleTextEdit(designTableForm.TableDesigner.Title);
					tableUserControl.SetLocationTextEdit(designTableForm.TableDesigner.Location);
					tableUserControl.RefreshColumnsTable();
					tableUserControl.RefreshDataLinks();
					if (focusedNode2 != null && focusedNode2.ObjectType == SharedObjectTypeEnum.ObjectType.View)
					{
						tableUserControl.RefreshScriptText();
					}
					else
					{
						tableUserControl.RefreshSchemaText();
					}
				}
				if ((focusedNode2 != null && focusedNode2.ObjectType == SharedObjectTypeEnum.ObjectType.Table) || (focusedNode2 != null && focusedNode2.ObjectType == SharedObjectTypeEnum.ObjectType.Structure) || (focusedNode2 != null && focusedNode2.ObjectType == SharedObjectTypeEnum.ObjectType.View))
				{
					SetTableUserControlLabels(focusedNode2);
				}
			}
			UpdateERDNode(designTableForm);
			SetVisibleUserControlSubtype(focusedNode2);
			GetVisibleSummaryUserControl()?.SetParameters();
		}
		catch
		{
			throw;
		}
		finally
		{
			tableUserControl.SetDisableSettingAsEdited(value: false);
		}
	}

	internal void EditProcedure(bool fromCustomFocus = false)
	{
		if (!ContinueAfterPossibleChanges())
		{
			return;
		}
		DBTreeNode focusedNode = GetFocusedNode(fromCustomFocus);
		DBTreeNode focusedNode2 = GetFocusedNode();
		if (((focusedNode == null || focusedNode.ObjectType != SharedObjectTypeEnum.ObjectType.Procedure) && (focusedNode == null || focusedNode.ObjectType != SharedObjectTypeEnum.ObjectType.Function)) || focusedNode.IsFolder)
		{
			return;
		}
		try
		{
			if (!fromCustomFocus)
			{
				procedureUserControl.ClearTabPageTitle();
				procedureUserControl.SetDisableSettingAsEdited(value: true);
			}
			DesignProcedureForm designProcedureForm = new DesignProcedureForm(focusedNode, CustomFieldsSupport);
			designProcedureForm.ShowDialog();
			procedureUserControl.isProcedureEdited = false;
			if (((focusedNode2 != null && focusedNode2.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure) || (focusedNode2 != null && focusedNode2.ObjectType == SharedObjectTypeEnum.ObjectType.Function)) && designProcedureForm.DialogResult == DialogResult.OK && GetVisibleUserControl() == procedureUserControl)
			{
				if (focusedNode.ObjectType == focusedNode2.ObjectType && focusedNode.Id == focusedNode2.Id)
				{
					focusedNode.Source = designProcedureForm.ProcedureDesigner.Source;
					procedureUserControl.RefreshModules();
					procedureUserControl.Refresh();
				}
				procedureUserControl.SetTitleTextEdit(designProcedureForm.ProcedureDesigner.Title);
				SetProcedureUserControlLabels(focusedNode2);
				metadataTreeList.RefreshNode(metadataTreeList.FocusedNode);
				procedureUserControl.SetParameters(focusedNode2, CustomFieldsSupport);
				procedureUserControl.Refresh();
			}
			SetVisibleUserControlSubtype(focusedNode2);
			GetVisibleSummaryUserControl()?.SetParameters();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, base.ParentForm);
		}
		finally
		{
			procedureUserControl.SetDisableSettingAsEdited(value: false);
		}
	}

	private void SetVisibleUserControlSubtype(DBTreeNode node)
	{
		if (GetVisibleUserControl() != null)
		{
			GetVisibleUserControl().Subtype = node.Subtype;
		}
	}

	private void SetSchemaAndTitle(DBTreeNode node)
	{
		tableUserControl.SetSchemNameAndTitle(node.Schema, node.BaseName, node.Title);
	}

	private void SetTableUserControlLabels(DBTreeNode node)
	{
		tableUserControl.SetLabels(node.Schema, node.BaseName, node.Title, node.ObjectType, node.Subtype);
	}

	private void SetProcedureUserControlLabels(DBTreeNode node)
	{
		procedureUserControl.SetLabels(node.Schema, node.BaseName, node.Title, node.Subtype, node.Source ?? UserTypeEnum.UserType.DBMS);
	}

	private void UpdateERDNode(DesignTableForm designTable)
	{
		moduleUserControl.UpdateERDNode(designTable.TableDesigner.TableId, designTable.TableDesigner.Schema, designTable.TableDesigner.Name, designTable.TableDesigner.Title, designTable.TableDesigner.Type, designTable.TableDesigner.Source, designTable.TableDesigner.ColumnsToRemove);
	}

	internal bool ImportFromFile(Form parentForm, SharedObjectTypeEnum.ObjectType? objectType, bool fromCustomFocus = false)
	{
		DBTreeNode focusedNode = GetFocusedNode(fromCustomFocus);
		ImportFromFileForm importFromFileForm = new ImportFromFileForm(parentForm, focusedNode.DatabaseId, objectType ?? focusedNode.ContainedObjectsObjectType, CustomFieldsSupport);
		if (importFromFileForm.ShowDialog(this) != DialogResult.OK)
		{
			return false;
		}
		if (focusedNode.IsFolder && focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module)
		{
			foreach (ObjectModel objectModel in importFromFileForm.ObjectModels)
			{
				DB.Module.InsertManualTable(focusedNode.ParentNode.Id, objectModel.ObjectId.Value);
				DBTreeMenu.AddManualObjectToTree(importFromFileForm.DatabaseId, objectModel.ObjectId.Value, null, objectModel.Name, null, objectModel.ObjectType, objectModel.ObjectSubtype, objectModel.Source, focusedNode, SynchronizeStateEnum.SynchronizeState.New);
			}
		}
		FocusObjectNodeOnTree(fromCustomFocus, (importFromFileForm.ObjectModels.Count() > 1) ? null : importFromFileForm.ObjectModels.ElementAt(0).ObjectId, importFromFileForm.ObjectType);
		return true;
	}

	internal bool PasteDocument(Form parentForm, SharedObjectTypeEnum.ObjectType? objectType, bool fromCustomFocus = false)
	{
		DBTreeNode focusedNode = GetFocusedNode(fromCustomFocus);
		ImportPasteDocumentForm importPasteDocumentForm = new ImportPasteDocumentForm(parentForm, focusedNode.DatabaseId, objectType ?? focusedNode.ContainedObjectsObjectType, CustomFieldsSupport);
		if (importPasteDocumentForm.ShowDialog(this) != DialogResult.OK)
		{
			return false;
		}
		FocusObjectNodeOnTree(fromCustomFocus, (importPasteDocumentForm.ObjectModels.Count() > 1) ? null : importPasteDocumentForm.ObjectModels.ElementAt(0).ObjectId, importPasteDocumentForm.ObjectType);
		return true;
	}

	internal bool AddUserTable(Form parentForm, SharedObjectTypeEnum.ObjectType? objectType, bool fromCustomFocus = false)
	{
		if (!ContinueAfterPossibleChanges())
		{
			return false;
		}
		tableUserControl.ClearTabPageTitle();
		DBTreeNode focusedNode = GetFocusedNode(fromCustomFocus);
		int databaseId = ((focusedNode.ParentNode == null) ? focusedNode.DatabaseId : ((focusedNode.DatabaseId == focusedNode.ParentNode.DatabaseId) ? focusedNode.DatabaseId : focusedNode.ParentNode.DatabaseId));
		tableUserControl.CurrentNode = focusedNode;
		tableUserControl.SetTitleTextEdit(focusedNode.Title);
		tableUserControl.isTableEdited = false;
		if (!objectType.HasValue)
		{
			objectType = focusedNode.ContainedObjectsObjectType;
			if (objectType != SharedObjectTypeEnum.ObjectType.Table && objectType != SharedObjectTypeEnum.ObjectType.Structure)
			{
				if (focusedNode.DatabaseClass == SharedDatabaseClassEnum.DatabaseClass.Database)
				{
					objectType = SharedObjectTypeEnum.ObjectType.Table;
				}
				else if (focusedNode.DatabaseClass == SharedDatabaseClassEnum.DatabaseClass.DataLake)
				{
					objectType = SharedObjectTypeEnum.ObjectType.Structure;
				}
			}
		}
		if (!objectType.HasValue)
		{
			return false;
		}
		DesignTableForm designTableForm = new DesignTableForm(parentForm, databaseId, objectType.Value, CustomFieldsSupport);
		if (designTableForm.ShowDialog(this) == DialogResult.OK)
		{
			if (focusedNode.IsFolder && focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module)
			{
				DB.Module.InsertManualTable(focusedNode.ParentNode.Id, designTableForm.TableDesigner.TableId);
				DBTreeMenu.AddManualObjectToTree(designTableForm.TableDesigner.DatabaseId, designTableForm.TableDesigner.TableId, designTableForm.TableDesigner.Schema, designTableForm.TableDesigner.Name, designTableForm.TableDesigner.Title, designTableForm.TableDesigner.ObjectTypeValue, designTableForm.TableDesigner.Type, designTableForm.TableDesigner.Source, focusedNode);
			}
			else if ((focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.View || focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Function || focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure || focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure) && focusedNode.IsInModule)
			{
				DB.Module.InsertManualTable(focusedNode.ParentNode.ParentNode.Id, designTableForm.TableDesigner.TableId);
				DBTreeMenu.AddManualObjectToTree(designTableForm.TableDesigner.DatabaseId, designTableForm.TableDesigner.TableId, designTableForm.TableDesigner.Schema, designTableForm.TableDesigner.Name, designTableForm.TableDesigner.Title, designTableForm.TableDesigner.ObjectTypeValue, designTableForm.TableDesigner.Type, designTableForm.TableDesigner.Source, focusedNode.ParentNode);
			}
			FocusObjectNodeOnTree(fromCustomFocus, designTableForm.TableDesigner.TableId, designTableForm.TableDesigner.ObjectTypeValue);
			return true;
		}
		return false;
	}

	internal bool AddUserProcedure(Form parentForm, SharedObjectTypeEnum.ObjectType objectType, bool fromCustomFocus = false)
	{
		if (!ContinueAfterPossibleChanges())
		{
			return false;
		}
		procedureUserControl.ClearTabPageTitle();
		procedureUserControl.SetDisableSettingAsEdited(value: true);
		DBTreeNode focusedNode = GetFocusedNode(fromCustomFocus);
		int databaseId = ((focusedNode.ParentNode == null) ? focusedNode.DatabaseId : ((focusedNode.DatabaseId == focusedNode.ParentNode.DatabaseId) ? focusedNode.DatabaseId : focusedNode.ParentNode.DatabaseId));
		procedureUserControl.CurrentNode = focusedNode;
		procedureUserControl.SetTitleTextEdit(focusedNode.Title);
		using DesignProcedureForm designProcedureForm = new DesignProcedureForm(parentForm, databaseId, objectType, CustomFieldsSupport);
		DialogResult num = designProcedureForm.ShowDialog(this);
		procedureUserControl.SetDisableSettingAsEdited(value: false);
		if (num == DialogResult.OK)
		{
			if (focusedNode.IsFolder && focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module)
			{
				DB.Module.InsertManualProcedure(focusedNode.ParentNode.Id, designProcedureForm.ProcedureDesigner.ProcedureId);
				DBTreeMenu.AddManualObjectToTree(designProcedureForm.ProcedureDesigner.DatabaseId, designProcedureForm.ProcedureDesigner.ProcedureId, designProcedureForm.ProcedureDesigner.Schema, designProcedureForm.ProcedureDesigner.Name, designProcedureForm.ProcedureDesigner.Title, designProcedureForm.ProcedureDesigner.ObjectTypeValue, designProcedureForm.ProcedureDesigner.Type, designProcedureForm.ProcedureDesigner.Source, focusedNode);
			}
			else if ((focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.View || focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Function || focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure || focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure) && focusedNode.IsInModule)
			{
				DB.Module.InsertManualProcedure(focusedNode.ParentNode.ParentNode.Id, designProcedureForm.ProcedureDesigner.ProcedureId);
				DBTreeMenu.AddManualObjectToTree(designProcedureForm.ProcedureDesigner.DatabaseId, designProcedureForm.ProcedureDesigner.ProcedureId, designProcedureForm.ProcedureDesigner.Schema, designProcedureForm.ProcedureDesigner.Name, designProcedureForm.ProcedureDesigner.Title, designProcedureForm.ProcedureDesigner.ObjectTypeValue, designProcedureForm.ProcedureDesigner.Type, designProcedureForm.ProcedureDesigner.Source, focusedNode.ParentNode);
			}
			FocusObjectNodeOnTree(fromCustomFocus, designProcedureForm.ProcedureDesigner.ProcedureId, designProcedureForm.ProcedureDesigner.ObjectTypeValue);
			return true;
		}
		return false;
	}

	private void FocusObjectNodeOnTree(bool fromCustomFocus, int? objectId, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (fromCustomFocus)
		{
			SelectNodeOnTree(objectId, objectType, TreeListHelpers.CustomFocusedTreeListNode);
			RefreshModuleCustomFocused();
		}
		else
		{
			SelectNodeOnTree(objectId, objectType, metadataTreeList.FocusedNode);
			RefreshModuleFocusedNode();
		}
		if (objectId.HasValue)
		{
			RefreshObjectProgress(showWaitForm: false, objectId.Value, objectType);
		}
	}

	private void RefreshModuleFocusedNode()
	{
		if (metadataTreeList.FocusedNode.ParentNode != null)
		{
			metadataTreeList.RefreshNode(metadataTreeList.FocusedNode.ParentNode);
		}
	}

	private void RefreshModuleCustomFocused()
	{
		if (TreeListHelpers.CustomFocusedTreeListNode.ParentNode != null)
		{
			metadataTreeList.RefreshNode(TreeListHelpers.CustomFocusedTreeListNode.ParentNode);
		}
	}

	internal void BulkAddTables(SharedObjectTypeEnum.ObjectType? objectType, bool fromCustomFocus = false)
	{
		if (!ContinueAfterPossibleChanges())
		{
			return;
		}
		DBTreeNode focusedNode = GetFocusedNode(fromCustomFocus);
		objectType = objectType ?? focusedNode.ContainedObjectsObjectType;
		AddBulkTablesForm addBulkTablesForm = new AddBulkTablesForm(focusedNode.DatabaseId, objectType.Value);
		if (addBulkTablesForm.ShowDialog(this) != DialogResult.OK)
		{
			return;
		}
		CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
		addBulkTablesForm.Insert(focusedNode);
		CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		if (addBulkTablesForm.Columns.Count == 0)
		{
			return;
		}
		if (fromCustomFocus)
		{
			SelectNodeOnTree(addBulkTablesForm.Inserter.Tables.Select((TableModel x) => x.TableId.Value).FirstOrDefault(), objectType.Value, TreeListHelpers.CustomFocusedTreeListNode);
		}
		else
		{
			SelectNodeOnTree(addBulkTablesForm.Inserter.Tables.Select((TableModel x) => x.TableId.Value).FirstOrDefault(), objectType.Value, metadataTreeList.FocusedNode);
		}
		foreach (TableModel table in addBulkTablesForm.Inserter.Tables)
		{
			RefreshObjectProgress(showWaitForm: false, table.TableId ?? (-1), objectType.Value);
		}
	}

	private void ActualizeModulesOrder(TreeListNode focusedNode)
	{
		TreeListHelpers.ActualizeModulesOrder(focusedNode, base.ParentForm);
	}

	private void renameBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		TreeListNode focusedTreeListNode = TreeListHelpers.GetFocusedTreeListNode(fromCustomFocus: true);
		TreeListNode focusedTreeListNode2 = TreeListHelpers.GetFocusedTreeListNode();
		if (focusedTreeListNode != focusedTreeListNode2)
		{
			TreeListHelpers.DisableFocusEvents = true;
			TreeListHelpers.FocusNodeToRestoreAfterRename = focusedTreeListNode2;
		}
		metadataTreeList.FocusedNode = focusedTreeListNode;
		DBTreeNode focusedNode = GetFocusedNode();
		if ((focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure) && TreeListHelpers.CustomFocusedTreeListNode == metadataTreeList.FocusedNode)
		{
			TreeListHelpers.ChangeNodeNameAndSchema(focusedNode);
			metadataTreeList.ShowEditor();
		}
		else if (TreeListHelpers.StartEditTitle(fromCustomFocus: true) && TreeListHelpers.CustomFocusedTreeListNode == metadataTreeList.FocusedNode)
		{
			metadataTreeList.ShowEditor();
		}
	}

	private void generateDocumentationBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (ContinueAfterPossibleChanges())
		{
			ShowDocCreator(fromCustomFocus: true);
		}
	}

	private void deleteBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (ContinueAfterPossibleChanges())
		{
			BaseSummaryUserControl visibleSummaryUserControl = GetVisibleSummaryUserControl();
			visibleSummaryUserControl?.CloseEditor();
			DeleteDbElement(TreeListHelpers.GetNode(TreeListHelpers.CustomFocusedTreeListNode)?.IsInModule ?? false, fromCustomFocus: true);
			visibleSummaryUserControl?.SetParameters();
			RefreshProgressIfShown();
			GetVisibleUserControl()?.ReloadGridsData();
			RefreshDataLineageIfVisible();
			SetWaitformVisibility(visible: false);
		}
	}

	private void editBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		EditTable(TreeListHelpers.IsCustomFocus);
	}

	private void addImportStructureBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddImportStructure(fromCustomFocus: true);
	}

	private void ProfileTableBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ProfileSingleTable(fromCustomFocus: true);
	}

	private void DataProfilingBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ProfileMultipleTables(fromCustomFocus: true);
	}

	internal void ProfileSingleColumn()
	{
		if (!ContinueAfterPossibleChanges())
		{
			return;
		}
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
		{
			using (UpgradeDataProfilingForm upgradeDataProfilingForm = new UpgradeDataProfilingForm())
			{
				upgradeDataProfilingForm.ShowDialog();
				return;
			}
		}
		if (!DataProfilingUtils.CheckIfDataProfilingFormAlreadyOpened(FindForm()))
		{
			DBTreeNode focusedNode = GetFocusedNode();
			if (DataProfilingUtils.CanViewDataProfilingForms(focusedNode?.DatabaseType, FindForm()) && DataProfilingUtils.ObjectCanBeProfilled(focusedNode?.ObjectType) && GetVisibleUserControl() is TableUserControl tableUserControl)
			{
				new DataProfilingForm().SetParameters(columnId: tableUserControl?.GetSelectedColumnRows()?.FirstOrDefault()?.Id, tables: new TableSimpleData(focusedNode).Yield(), databaseId: focusedNode.DatabaseId, databaseTitle: focusedNode.DatabaseTitle, metadataEditorUserControl: this, databaseType: focusedNode.DatabaseType);
			}
		}
	}

	internal void ProfileMultipleTables(bool fromCustomFocus = false)
	{
		if (!ContinueAfterPossibleChanges())
		{
			return;
		}
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
		{
			using (UpgradeDataProfilingForm upgradeDataProfilingForm = new UpgradeDataProfilingForm())
			{
				upgradeDataProfilingForm.ShowDialog();
				return;
			}
		}
		if (DataProfilingUtils.CheckIfDataProfilingFormAlreadyOpened(FindForm()))
		{
			return;
		}
		if (fromCustomFocus)
		{
			ProfileMultipleTablesFromFocusedNode(fromCustomFocus);
		}
		else if (GetVisibleSummaryUserControl() is TableSummaryUserControl tableSummaryUserControl)
		{
			if (tableSummaryUserControl.NumberOfSelectedTablesInTheGrid > 1)
			{
				tableSummaryUserControl.ProfileSelectedTables();
			}
			else
			{
				ProfileMultipleTablesFromFocusedNode(fromCustomFocus: false);
			}
		}
		else if (GetVisibleUserControl() is DatabaseUserControl)
		{
			ProfileMultipleTablesFromFocusedNode(fromCustomFocus: false);
		}
	}

	internal void ProfileSingleTable(bool fromCustomFocus = false)
	{
		if (!ContinueAfterPossibleChanges())
		{
			return;
		}
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
		{
			using (UpgradeDataProfilingForm upgradeDataProfilingForm = new UpgradeDataProfilingForm())
			{
				upgradeDataProfilingForm.ShowDialog();
				return;
			}
		}
		if (!DataProfilingUtils.CheckIfDataProfilingFormAlreadyOpened(FindForm()))
		{
			if (!fromCustomFocus && GetVisibleSummaryUserControl() is TableSummaryUserControl tableSummaryUserControl && tableSummaryUserControl.NumberOfSelectedTablesInTheGrid == 1)
			{
				tableSummaryUserControl.ProfileSelectedTables();
			}
			else
			{
				ProfileTableFromFocusedNode(fromCustomFocus);
			}
		}
	}

	private void ProfileMultipleTablesFromFocusedNode(bool fromCustomFocus)
	{
		DBTreeNode focusedNode = GetFocusedNode(fromCustomFocus);
		if (focusedNode == null || !DataProfilingUtils.CanViewDataProfilingForms(focusedNode.DatabaseType, FindForm()))
		{
			return;
		}
		string tableFolderName = SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Table);
		string viewFolderName = SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.View);
		if (focusedNode.DatabaseClass == SharedDatabaseClassEnum.DatabaseClass.Database || (focusedNode.IsFolder && (!(focusedNode.Name != tableFolderName) || !(focusedNode.Name != viewFolderName))))
		{
			IEnumerable<TableSimpleData> tables = ((!focusedNode.IsFolder) ? (from x in focusedNode.Nodes.Where((DBTreeNode x) => x.IsFolder && (x.Name == tableFolderName || x.Name == viewFolderName)).SelectMany((DBTreeNode x) => x.Nodes)
				where DataProfilingUtils.NodeCanBeProfilled(x)
				select new TableSimpleData(x)) : (from x in focusedNode.Nodes
				where DataProfilingUtils.NodeCanBeProfilled(x)
				select new TableSimpleData(x)));
			new DataProfilingForm().SetParameters(tables, focusedNode.DatabaseId, focusedNode.DatabaseTitle, this, focusedNode.DatabaseType);
		}
	}

	private void ProfileTableFromFocusedNode(bool fromCustomFocus)
	{
		DBTreeNode focusedNode = GetFocusedNode(fromCustomFocus);
		if (DataProfilingUtils.CanViewDataProfilingForms(focusedNode?.DatabaseType, FindForm()) && DataProfilingUtils.ObjectCanBeProfilled(focusedNode?.ObjectType))
		{
			new DataProfilingForm().SetParameters(new TableSimpleData(focusedNode).Yield(), focusedNode.DatabaseId, focusedNode.DatabaseTitle, this, focusedNode.DatabaseType);
		}
	}

	private void PreviewSampleDataBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		PreviewSampleData(fromCustomFocus: true);
	}

	internal void PreviewSampleData(bool fromCustomFocus = false)
	{
		if (!ContinueAfterPossibleChanges())
		{
			return;
		}
		if (!Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
		{
			using (UpgradeDataProfilingForm upgradeDataProfilingForm = new UpgradeDataProfilingForm())
			{
				upgradeDataProfilingForm.ShowDialog(this);
				return;
			}
		}
		if (!fromCustomFocus && GetVisibleSummaryUserControl() is TableSummaryUserControl tableSummaryUserControl)
		{
			tableSummaryUserControl.PreviewSampleData();
			return;
		}
		DBTreeNode focusedNode = GetFocusedNode(fromCustomFocus);
		if (!DataProfilingUtils.ObjectCanBeProfilled(focusedNode?.ObjectType) || !DataProfilingUtils.CanViewDataProfilingForms(focusedNode.DatabaseType, FindForm()))
		{
			return;
		}
		using SampleDataForm sampleDataForm = new SampleDataForm();
		sampleDataForm.SetParameters(focusedNode.Id, focusedNode.BaseName, focusedNode.Schema, focusedNode.ObjectType, focusedNode.DatabaseId, focusedNode.DatabaseType, focusedNode.DBMSVersion);
		sampleDataForm.ShowDialog();
	}

	public void RefreshSparklinesForDataProfiling()
	{
		tableUserControl.RefreshSparklines();
		RefreshProfilingInVisibleTableSummaryControl();
	}

	private void ClearTableProfilingBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ClearTableProfiling(fromCustomFocus: true);
	}

	private void ClearTableProfiling(bool fromCustomFocus = false)
	{
		try
		{
			if (ContinueAfterPossibleChanges() && Functionalities.HasFunctionality(FunctionalityEnum.Functionality.DataProfiling))
			{
				DBTreeNode focusedNode = GetFocusedNode(fromCustomFocus);
				if (DataProfilingUtils.ObjectCanBeProfilled(focusedNode?.ObjectType) && GeneralMessageBoxesHandling.Show("Do you want to delete all profiling data from the <b>" + focusedNode.Name + "</b> " + focusedNode?.ObjectType.ToString().ToLower() + "?", "Clear all Profiling Data", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, null, 1, FindForm()).DialogResult == DialogResult.OK)
				{
					DateTime utcNow = DateTime.UtcNow;
					DB.DataProfiling.RemoveAllProfilingForSingleTable(focusedNode.Id, splashScreenManager, base.ParentForm);
					tableUserControl.RefreshSparklines();
					RefreshProfilingInVisibleTableSummaryControl(focusedNode.Id);
					string duration = ((int)(DateTime.UtcNow - utcNow).TotalSeconds).ToString();
					DataProfilingTrackingHelper.TrackDataProfilingCleared("ALL", duration);
				}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void addTableEntityBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddTableEntity(fromCustomFocus: true);
	}

	private void bulkAddTableBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		BulkAddTables(SharedObjectTypeEnum.ObjectType.Table, fromCustomFocus: true);
	}

	private void bulkAddStructureBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		BulkAddTables(SharedObjectTypeEnum.ObjectType.Structure, fromCustomFocus: true);
	}

	public void AddObject(bool fromCustomFocus = false)
	{
		DBTreeNode focusedNode = GetFocusedNode(fromCustomFocus);
		if (TreeListHelpers.IsObjectOrFolder(focusedNode, SharedObjectTypeEnum.ObjectType.Table))
		{
			AddTableEntity(fromCustomFocus);
		}
		else if (TreeListHelpers.IsObjectOrFolder(focusedNode, SharedObjectTypeEnum.ObjectType.Structure))
		{
			AddImportStructure(fromCustomFocus);
		}
		else if (TreeListHelpers.IsObjectOrFolder(focusedNode, SharedObjectTypeEnum.ObjectType.View))
		{
			AddView(fromCustomFocus);
		}
		else if (TreeListHelpers.IsObjectOrFolder(focusedNode, SharedObjectTypeEnum.ObjectType.Procedure))
		{
			AddProcedure(fromCustomFocus);
		}
		else if (TreeListHelpers.IsObjectOrFolder(focusedNode, SharedObjectTypeEnum.ObjectType.Function))
		{
			AddFunction(fromCustomFocus);
		}
	}

	public void AddView(bool fromCustomFocus = false)
	{
		AddObjectForm parentForm = new AddObjectForm();
		AddUserTable(parentForm, SharedObjectTypeEnum.ObjectType.View, fromCustomFocus);
	}

	public void AddProcedure(bool fromCustomFocus = false)
	{
		AddObjectForm parentForm = new AddObjectForm();
		AddUserProcedure(parentForm, SharedObjectTypeEnum.ObjectType.Procedure, fromCustomFocus);
	}

	public void AddFunction(bool fromCustomFocus = false)
	{
		AddObjectForm parentForm = new AddObjectForm();
		AddUserProcedure(parentForm, SharedObjectTypeEnum.ObjectType.Function, fromCustomFocus);
	}

	public void AddTableEntity(bool fromCustomFocus = false)
	{
		AddObjectForm form = new AddObjectForm();
		form.Initialize("Add Table/Entity", "Design Manually", delegate
		{
			ProcessAddObjectForm(form, () => AddUserTable(form, SharedObjectTypeEnum.ObjectType.Table, fromCustomFocus));
		}, Resources.design_table_32, "Paste JSON Document" + Environment.NewLine + "(NoSQL Collection)", delegate
		{
			ProcessAddObjectForm(form, () => PasteDocument(form, SharedObjectTypeEnum.ObjectType.Table, fromCustomFocus));
		}, Resources.paste_document_32, "Import JSON File" + Environment.NewLine + "(NoSQL Collection)", delegate
		{
			ProcessAddObjectForm(form, () => ImportFromFile(form, SharedObjectTypeEnum.ObjectType.Table, fromCustomFocus));
		}, Resources.import_from_file_32);
		form.ShowDialog();
	}

	public void AddImportStructure(bool fromCustomFocus = false)
	{
		AddObjectForm form = new AddObjectForm();
		form.Initialize("Add/Import Structure/File", "Design Manually", delegate
		{
			ProcessAddObjectForm(form, () => AddUserTable(form, SharedObjectTypeEnum.ObjectType.Structure, fromCustomFocus));
		}, Resources.design_structure_32, "Paste Document", delegate
		{
			ProcessAddObjectForm(form, () => PasteDocument(form, SharedObjectTypeEnum.ObjectType.Structure, fromCustomFocus));
		}, Resources.paste_document_32, "Import from File", delegate
		{
			ProcessAddObjectForm(form, () => ImportFromFile(form, SharedObjectTypeEnum.ObjectType.Structure, fromCustomFocus));
		}, Resources.import_from_file_32);
		form.ShowDialog();
	}

	private void ProcessAddObjectForm(AddObjectForm form, Func<bool> func)
	{
		try
		{
			if (func())
			{
				form.Close();
			}
			else
			{
				form.Visible = true;
			}
		}
		catch (Exception)
		{
			form.Close();
			throw;
		}
	}

	private void synchronizeSchemaBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (ContinueAfterPossibleChanges())
		{
			Synchronize(fromCustomFocus: true);
		}
	}

	public void Synchronize(bool fromCustomFocus = false, DBTreeNode dBTreeNode = null)
	{
		if (!Licenses.CheckRepositoryVersionAfterLogin())
		{
			return;
		}
		metadataTreeList.CloseEditor();
		DBTreeNode dBTreeNode2 = ((dBTreeNode != null) ? dBTreeNode : GetFocusedNode(fromCustomFocus)?.GetClosestNode(SharedObjectTypeEnum.ObjectType.Database));
		if (dBTreeNode2 == null)
		{
			return;
		}
		if (dBTreeNode2.DatabaseType.HasValue && !Connectors.HasDatabaseTypeConnector(dBTreeNode2.DatabaseType.Value))
		{
			GeneralMessageBoxesHandling.Show(SharedDatabaseTypeEnum.TypeToStringForDisplay(dBTreeNode2.DatabaseType) + " connector is not supported by your license. <href=" + Links.ManageAccounts + ">Upgrade</href> to connect.", "Upgrade account", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
			return;
		}
		ConnectAndSynchForm connectAndSynchForm = new ConnectAndSynchForm(this, dBTreeNode2.DatabaseId, metadataTreeList, false);
		connectAndSynchForm.SynchFinishedEvent += connectAndSynchForm_SynchFinishedEvent;
		connectAndSynchForm.ShowDialog(this, setCustomMessageDefaultOwner: true);
		if (connectAndSynchForm.HasFinished)
		{
			try
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
				RefreshDatabaseNodeAfterConnectToDatabase(dBTreeNode2, connectAndSynchForm);
				RefreshBarButtonItemText();
				CustomFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: true);
				BasePanelControl visibleUserControl = GetVisibleUserControl();
				visibleUserControl?.SetParameters(visibleUserControl.CurrentNode, CustomFieldsSupport);
			}
			finally
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			}
		}
		StaticData.ClearDatabaseInfoForCrashes();
	}

	private void RefreshDatabaseNodeAfterConnectToDatabase(DBTreeNode documentationNode, ConnectAndSynchForm connectAndSynchForm)
	{
		TreeListNode treeListNode = metadataTreeList.FindNodes((TreeListNode x) => TreeListHelpers.GetDBTreeNode(x).DatabaseId == documentationNode.DatabaseId && TreeListHelpers.GetDBTreeNode(x).ObjectType == SharedObjectTypeEnum.ObjectType.Database).FirstOrDefault();
		if (treeListNode == null)
		{
			return;
		}
		DBTreeNode dBTreeNode = TreeListHelpers.GetDBTreeNode(treeListNode);
		if (dBTreeNode == null)
		{
			return;
		}
		dBTreeNode.DatabaseType = connectAndSynchForm?.DatabaseType;
		IEnumerable<DBTreeNode> enumerable = dBTreeNode?.Nodes?.ToList();
		if (enumerable == null)
		{
			return;
		}
		enumerable.ForEach(delegate(DBTreeNode x)
		{
			x.DatabaseType = connectAndSynchForm?.DatabaseType;
		});
		enumerable.ForEach(delegate(DBTreeNode x)
		{
			x.Nodes?.ToList()?.ForEach(delegate(DBTreeNode y)
			{
				y.DatabaseType = connectAndSynchForm?.DatabaseType;
			});
		});
		metadataTreeList?.RefreshNode(treeListNode);
	}

	public void EditCustomFields()
	{
		if (Licenses.CheckRepositoryVersionAfterLogin())
		{
			metadataTreeList.CloseEditor();
			ClosePanelEditor();
			CustomFieldsSummaryForm customFieldsSummaryForm = new CustomFieldsSummaryForm();
			customFieldsSummaryForm.ShowDialog(this, setCustomMessageDefaultOwner: true);
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			if (customFieldsSummaryForm.DialogResult == DialogResult.OK)
			{
				CustomFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: true);
				RefreshSearchCustomFields();
			}
			OpenPageControl(showControl: false);
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	public void AddDatabase(bool isCopyingConnection = false, bool fromCustomFocus = false)
	{
		bool isDatabaseAdded = false;
		if (!Licenses.CheckRepositoryVersionAfterLogin())
		{
			return;
		}
		metadataTreeList.CloseEditor();
		ConnectAndSynchForm connectAndSynchForm = null;
		if (isCopyingConnection)
		{
			DBTreeNode dBTreeNode = GetFocusedNode(fromCustomFocus)?.GetClosestNode(SharedObjectTypeEnum.ObjectType.Database);
			if (dBTreeNode == null)
			{
				return;
			}
			if (dBTreeNode.DatabaseType.HasValue && !Connectors.HasDatabaseTypeConnector(dBTreeNode.DatabaseType.Value))
			{
				GeneralMessageBoxesHandling.Show(SharedDatabaseTypeEnum.TypeToStringForDisplay(dBTreeNode.DatabaseType) + " connector is not supported by your license. <href=" + Links.ManageAccounts + ">Upgrade</href> to connect.", "Upgrade account", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, base.ParentForm);
				return;
			}
			connectAndSynchForm = new ConnectAndSynchForm(this, dBTreeNode.DatabaseId, null, true);
		}
		else
		{
			connectAndSynchForm = new ConnectAndSynchForm(this, null, null, false);
		}
		connectAndSynchForm.AddDatabaseEvent += delegate(object sender, EventArgs e)
		{
			AddDatabaseNode((e as IdEventArgs).DatabaseId);
			isDatabaseAdded = true;
		};
		connectAndSynchForm.ShowDialog(this, setCustomMessageDefaultOwner: true);
		StaticData.ClearDatabaseInfoForCrashes();
		Rectangle? bounds = TreeListHelpers.GetNodeBounds(metadataTreeList.FocusedNode);
		if (bounds.HasValue && isDatabaseAdded)
		{
			OnboardingSupport.ShowPanel(OnboardingSupport.OnboardingMessages.Import, FindForm(), () => bounds.Value);
		}
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			if (connectAndSynchForm.HasFinished)
			{
				CustomFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: true);
				BasePanelControl visibleUserControl = GetVisibleUserControl();
				visibleUserControl?.SetParameters(visibleUserControl.CurrentNode, CustomFieldsSupport);
			}
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	public void AddDataLake()
	{
		new Dataedo.App.Forms.ConnectAndSynchDataLakeForm().ShowDialog();
	}

	private void connectAndSynchForm_SynchFinishedEvent(object sender, EventArgs e)
	{
		OpenPageControl(showControl: false);
		metadataTreeList.RefreshDataSource();
	}

	private void UpdateNodeDatabaseType(SharedDatabaseTypeEnum.DatabaseType? databaseType)
	{
		DBTreeNode focusedNode = GetFocusedNode();
		List<DBTreeNode> list = focusedNode.Nodes.ToList();
		focusedNode.DatabaseType = databaseType;
		list.ForEach(delegate(DBTreeNode x)
		{
			x.DatabaseType = databaseType;
		});
		list.ForEach(delegate(DBTreeNode x)
		{
			x.Nodes.ToList().ForEach(delegate(DBTreeNode y)
			{
				y.DatabaseType = databaseType;
			});
		});
		RefreshBarButtonItemText();
	}

	private void addModuleBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddModule(fromCustomFocus: true);
	}

	internal void AddRelationHandler()
	{
		metadataTreeList.CloseEditor();
		if (GetVisibleUserControl() == tableUserControl)
		{
			tableUserControl.AddRelation(tableUserControl.GetSelectedColumnRows());
		}
	}

	internal void EditRelationHandler()
	{
		metadataTreeList.CloseEditor();
		if (GetVisibleUserControl() == tableUserControl)
		{
			tableUserControl.EditRelation();
		}
	}

	internal void AddConstraintHandler()
	{
		metadataTreeList.CloseEditor();
		if (GetVisibleUserControl() == tableUserControl)
		{
			tableUserControl.AddConstraint(isPK: false);
		}
	}

	internal void EditConstraintHandler()
	{
		metadataTreeList.CloseEditor();
		if (GetVisibleUserControl() == tableUserControl)
		{
			tableUserControl.EditConstraint();
		}
	}

	internal void RemoveConstraintHandler()
	{
		metadataTreeList.CloseEditor();
		if (GetVisibleUserControl() == tableUserControl)
		{
			tableUserControl.ProcessConstraintDelete();
		}
	}

	internal void RemoveDependencyHandler()
	{
		metadataTreeList.CloseEditor();
		BasePanelControl visibleUserControl = GetVisibleUserControl();
		if (visibleUserControl == tableUserControl)
		{
			tableUserControl.ProcessDependencyDelete();
		}
		else if (visibleUserControl == procedureUserControl)
		{
			procedureUserControl.ProcessDependencyDelete();
		}
	}

	internal void SetControlsDuringAddingModuleEnabled(bool enabled)
	{
		moduleUserControl?.SetERDEnabled(enabled);
	}

	private void refreshButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		RefreshSubnodes(sender, e);
	}

	private void RefreshSubnodes(object sender, ItemClickEventArgs e)
	{
		TreeListNode focusedTreeListNode = TreeListHelpers.GetFocusedTreeListNode(fromCustomFocus: true);
		DBTreeNode node = TreeListHelpers.GetNode(focusedTreeListNode);
		if (node == null)
		{
			return;
		}
		if (node.ObjectType == SharedObjectTypeEnum.ObjectType.Repository)
		{
			this.RefreshViewEvent?.Invoke(sender, e);
			return;
		}
		DBTreeNode node2 = TreeListHelpers.GetNode(metadataTreeList.FocusedNode);
		bool flag = node.ContainsNode(node2);
		if (!flag || ContinueAfterPossibleChanges(null, delegate
		{
			OpenPageControl();
		}))
		{
			if (flag)
			{
				metadataTreeList.FocusedNode = focusedTreeListNode;
			}
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			metadataTreeList.BeginUpdate();
			dbTreeMenu.RefeshDBData(node, ShowProgress, progressType);
			metadataTreeList.EndUpdate();
			if (showProgress)
			{
				RefreshProgress(showWaitForm: false, node);
			}
			UpdateListView(node, CustomFieldsSupport, showControl: false);
			OpenNodes(focusedTreeListNode);
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	public void SetWaitformVisibility(bool visible)
	{
		CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, visible);
	}

	private void metadataTreeList_MouseDown(object sender, MouseEventArgs e)
	{
		TreeListHelpers.MouseDown(sender, e, this, Control.MousePosition, treePopupMenu, refreshButtonItem, addObjectBarButtonItem, addImportStructureBarButtonItem, addTableEntityBarButtonItem, bulkAddObjectsBarButtonItem, bulkAddTableBarButtonItem, bulkAddStructureBarButtonItem, addModuleBarButtonItem, moveUpBarButtonItem, moveDownBarButtonItem, renameBarButtonItem, synchronizeSchemaBarButtonItem, importDescriptionsBarButtonItem, generateDocumentationBarButtonItem, designTableBarButtonItem, profileTableBarButtonItem, dataProfilingBarButtonItem, previewSampleDataBarButtonItem, addTermBarButtonItem, deleteBarButtonItem, addTermRelatedTermBarButtonItem, addDataLinkBarButtonItem, addNewTermBarButtonItem, expandAllBarButtonItem, collapseAllBarButtonItem, copyDescriptionsBarButtonItem, addBarButtonItem, addBusinessGlossaryBarButtonItem, displaySchemaOptionsBarSubItem, alwaysDisplaySchemaBarButtonItem, neverDisplaySchemaBarButtonItem, defaultDisplaySchemaBarButtonItem, connectToExistingBarButtonItem, connectToBarButtonItem, createNewRepositoryBarButtonItem, copyConnectionBarButtonItem, addViewBarButtonItem, duplicateModuleButtonItem, bulkAddViewBarButtonItem, addProcedureBarButtonItem, designProcedureBarButtonItem, addFunctionBarButtonItem, designFunctionBarButtonItem, clearTableProfilingBarButtonItem, sortModulesAlphabeticallyBarButtonItem, moveToTopBarButtonItem, moveToBottomBarButtonItem);
	}

	internal void AddPrimaryKey()
	{
		tableUserControl.AddConstraint(isPK: true);
	}

	internal void AddUniqueKey()
	{
		tableUserControl.AddConstraint(isPK: false);
	}

	private void metadataTreeList_MouseUp(object sender, MouseEventArgs e)
	{
		TreeListHelpers.MouseUp(sender, e, this, progressTreeListColumn, delegate(TreeListNode node)
		{
			ProcessFocusNode(node);
		});
	}

	private void ExpandAllBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		TreeListHelpers.ExpandAll(fromCustomFocus: true);
	}

	private void CollapseAllBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		TreeListNode focusedTreeListNode = TreeListHelpers.GetFocusedTreeListNode(fromCustomFocus: true);
		TreeListNode focusedTreeListNode2 = TreeListHelpers.GetFocusedTreeListNode();
		if (TreeListHelpers.GetNode(focusedTreeListNode).ContainsNode(TreeListHelpers.GetNode(focusedTreeListNode2)))
		{
			if (!ContinueAfterPossibleChanges())
			{
				return;
			}
			metadataTreeList.FocusedNode = focusedTreeListNode2;
		}
		TreeListHelpers.CollaspeAll(focusedTreeListNode);
	}

	public void ForceRefreshCustomFieldsControlsOnNextLoading()
	{
		databaseUserControl.CustomFieldsPanelControl.ForceRefreshCustomFieldsControlsOnNextLoading();
		moduleUserControl.CustomFieldsPanelControl.ForceRefreshCustomFieldsControlsOnNextLoading();
		tableUserControl.CustomFieldsPanelControl.ForceRefreshCustomFieldsControlsOnNextLoading();
		procedureUserControl.CustomFieldsPanelControl.ForceRefreshCustomFieldsControlsOnNextLoading();
		termUserControl.CustomFieldsPanelControl.ForceRefreshCustomFieldsControlsOnNextLoading();
	}

	public void RefreshTree(bool showWaitForm, bool refreshProgress = false, bool rememberSelectedNode = true)
	{
		bool isSplashFormVisible = splashScreenManager.IsSplashFormVisible;
		TreeListHelpers.LockFocus = false;
		metadataTreeList.CloseEditor();
		metadataTreeList.BeginUpdate();
		if (showWaitForm && !isSplashFormVisible)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
		}
		List<int> nodesIndexes = null;
		if (rememberSelectedNode)
		{
			nodesIndexes = TreeListHelpers.GetSelectedNodeTreeIndexPath(FindForm());
		}
		CustomFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: true);
		BusinessGlossarySupport.LoadTermRelationshipTypes();
		DBTree dataSource = dbTreeMenu.LoadDBData(FindForm());
		metadataTreeList.BeginUpdate();
		metadataTreeList.BeginUnboundLoad();
		metadataTreeList.DataSource = dataSource;
		metadataTreeList.EndUnboundLoad();
		metadataTreeList.EndUpdate();
		metadataTreeList.ForceInitialize();
		metadataTreeList.Nodes.FirstNode.Expand();
		metadataTreeList.Nodes.FirstNode.Expanded = true;
		metadataTreeList.SetFocusedNode(metadataTreeList.Nodes.FirstNode);
		if (metadataTreeList.FocusedNode != null)
		{
			metadataTreeList.SetNodeCheckState(metadataTreeList.FocusedNode, CheckState.Checked);
		}
		ShowProgressColumn(showWaitForm, refreshProgress);
		nameTreeListColumn.Caption = "Repository Explorer";
		RefreshSearchDatabasesFromMetadataTreeList();
		RefreshSearchCustomFields();
		DocumentationsCheckedComboBoxEdit.Properties.DisplayMember = "Name";
		DocumentationsCheckedComboBoxEdit.Properties.ValueMember = "Id";
		ClearAllSearchHighlights();
		GetVisibleUserControl()?.SetTabsProgressHighlights();
		ResetSearchSupport();
		if (rememberSelectedNode)
		{
			TreeListHelpers.SelectNodeByIndexPath(nodesIndexes, FindForm());
		}
		if (showWaitForm && !isSplashFormVisible)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
		metadataTreeList.EndUpdate();
	}

	private void OpenNodes()
	{
		metadataTreeList.Nodes.FirstNode.Expand();
		metadataTreeList.Nodes.FirstNode.Expanded = true;
		TreeListNode[] nodes = metadataTreeList.Nodes.FirstNode.Nodes.ToArray();
		OpenNodes(nodes);
	}

	private void OpenNodes(params TreeListNode[] nodes)
	{
		nodes?.ToList().ForEach(delegate(TreeListNode x)
		{
			x.Expanded = true;
			TreeListNode treeListNode = SearchTreeNodeOperation.FindModuleDirectory(x.Nodes);
			if (treeListNode != null)
			{
				treeListNode.Expanded = true;
			}
			TreeListNode treeListNode2 = SearchTreeNodeOperation.FindTermsDirectory(x.Nodes);
			if (treeListNode2 != null)
			{
				treeListNode2.Expanded = true;
			}
			x.Expanded = false;
		});
	}

	private void ClearAllSearchHighlights(bool forceAll = false)
	{
		databaseUserControl.ClearHighlights(keepSearchActive: false);
		moduleUserControl.ClearHighlights(keepSearchActive: false);
		tableUserControl.ClearHighlights(keepSearchActive: false);
		moduleUserControl.ClearHighlights(keepSearchActive: false);
		procedureUserControl.ClearHighlights(keepSearchActive: false);
		termUserControl.ClearHighlights(keepSearchActive: false);
		if (forceAll)
		{
			moduleUserControl.ForceLayoutChange(forceAll: true);
			tableUserControl.ForceLayoutChange(forceAll: true);
			moduleUserControl.ForceLayoutChange(forceAll: true);
			procedureUserControl.ForceLayoutChange(forceAll: true);
			termUserControl.ForceLayoutChange(forceAll: true);
		}
	}

	public void AddDatabaseNode(int? databaseId)
	{
		if (databaseId.HasValue)
		{
			dbTreeMenu.AddDatabase(databaseId.Value, FindForm());
			metadataTreeList.FocusedNode = SearchTreeNodeOperation.FindNode(metadataTreeList.Nodes, databaseId.Value);
			RefreshSearchDatabasesFromMetadataTreeList();
			RebuildHomePage(forceReload: false);
		}
	}

	public void OpenPageControl(bool showControl = true, DBTreeNode dbTreeNode = null, DependencyRow.DependencyNodeCommonType? dependencyType = null, string filterColumn = null)
	{
		try
		{
			dbTreeNode = ((dbTreeNode == null) ? GetFocusedNode() : dbTreeNode);
			if (dbTreeNode != null && (dbTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Module || dbTreeNode.Id != -1) && (showControl || (!showControl && dbTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.UnresolvedEntity)))
			{
				switch (dbTreeNode.ObjectType)
				{
				case SharedObjectTypeEnum.ObjectType.Repository:
					if (!homePanelUserControl.IsLoaded)
					{
						homePanelUserControl.ClearData();
					}
					homePanelUserControl.RefreshDatabasesList();
					CommonFunctionsPanels.ShowUserControlInPanel(homePanelUserControl, metadataEditorSplitContainerControl.Panel2);
					homePanelUserControl.Invalidate();
					homePanelUserControl.SetParameters(IsLinkToObjectValid, forceReload: false);
					break;
				case SharedObjectTypeEnum.ObjectType.Database:
					databaseUserControl.CurrentNode = dbTreeNode;
					databaseUserControl.SetParameters(dbTreeNode, CustomFieldsSupport);
					if (showControl)
					{
						if (dbTreeNode.Available)
						{
							CommonFunctionsPanels.ShowUserControlInPanel(databaseUserControl, metadataEditorSplitContainerControl.Panel2);
						}
						selectedFormType = SharedObjectTypeEnum.ObjectType.Database;
					}
					break;
				case SharedObjectTypeEnum.ObjectType.BusinessGlossary:
					if (!Functionalities.HasAnyFunctionality(FunctionalityEnum.Functionality.BusinessGlossary))
					{
						upgradeBusinessGlossaryControl = new UpgradeBusinessGlossaryControl();
						CommonFunctionsPanels.ShowUserControlInPanel(upgradeBusinessGlossaryControl, metadataEditorSplitContainerControl.Panel2);
						break;
					}
					databaseUserControl.CurrentNode = dbTreeNode;
					databaseUserControl.SetParameters(dbTreeNode, CustomFieldsSupport);
					if (showControl)
					{
						if (dbTreeNode.Available)
						{
							CommonFunctionsPanels.ShowUserControlInPanel(databaseUserControl, metadataEditorSplitContainerControl.Panel2);
						}
						selectedFormType = SharedObjectTypeEnum.ObjectType.Database;
					}
					break;
				case SharedObjectTypeEnum.ObjectType.Function:
				case SharedObjectTypeEnum.ObjectType.Procedure:
				case SharedObjectTypeEnum.ObjectType.Table:
				case SharedObjectTypeEnum.ObjectType.View:
				case SharedObjectTypeEnum.ObjectType.Structure:
				case SharedObjectTypeEnum.ObjectType.Module:
				case SharedObjectTypeEnum.ObjectType.Term:
					ShowObjectUserControl(dbTreeNode.ObjectType, dbTreeNode, CustomFieldsSupport, showControl, dependencyType);
					break;
				}
				UpdateListView(dbTreeNode, CustomFieldsSupport, showControl);
			}
			if (metadataTreeList == null || metadataTreeList.Nodes.Count == 0)
			{
				ClearDataPanel();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
			ClearDataPanel();
		}
	}

	public void RebuildHomePage(bool forceReload)
	{
		homePanelUserControl.ForceRebuild(forceReload);
	}

	private void UpdateListView(DBTreeNode dbTreeNode, CustomFieldsSupport customFieldsSupport, bool showControl = true)
	{
		if ((dbTreeNode == null || !dbTreeNode.IsFolder) && (dbTreeNode == null || dbTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database))
		{
			return;
		}
		SharedObjectTypeEnum.ObjectType? objectType = ((dbTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Database) ? new SharedObjectTypeEnum.ObjectType?(SharedObjectTypeEnum.ObjectType.Module) : dbTreeNode.ParentNode?.ObjectType);
		BaseSummaryUserControl baseSummaryUserControl;
		switch (dbTreeNode.BaseName)
		{
		default:
			return;
		case "Tables":
			baseSummaryUserControl = tableSummaryUserControl;
			break;
		case "Views":
			baseSummaryUserControl = viewSummaryUserControl;
			break;
		case "Structures":
			baseSummaryUserControl = objectSummaryUserControl;
			break;
		case "Procedures":
			baseSummaryUserControl = procedureSummaryUserControl;
			break;
		case "Functions":
			baseSummaryUserControl = functionSummaryUserControl;
			break;
		case "Subject Areas":
			baseSummaryUserControl = moduleSummaryUserControl;
			break;
		case "Terms":
			baseSummaryUserControl = termSummaryUserControl;
			break;
		}
		try
		{
			SetWaitformVisibility(visible: true);
			baseSummaryUserControl.SetParameters(dbTreeNode, customFieldsSupport, objectType);
			if (showControl)
			{
				CommonFunctionsPanels.ShowUserControlInPanel(baseSummaryUserControl, metadataEditorSplitContainerControl.Panel2);
			}
		}
		finally
		{
			SetWaitformVisibility(visible: false);
		}
	}

	public void AddModule(bool fromCustomFocus = false)
	{
		if (!Licenses.CheckRepositoryVersionAfterLogin())
		{
			return;
		}
		metadataTreeList.CloseEditor();
		DBTreeNode focusedNode = GetFocusedNode(fromCustomFocus);
		TreeListNode focusedTreeListNode = TreeListHelpers.GetFocusedTreeListNode(fromCustomFocus);
		if (focusedNode != null)
		{
			if (focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Database)
			{
				moduleSummaryUserControl.SetEmptyModulesListPanelVisibility(visible: false);
				focusedTreeListNode.Expand();
				TreeListNode treeListNode = SearchTreeNodeOperation.FindModuleDirectory(focusedTreeListNode.Nodes);
				TreeListHelpers.ParentForAddModule = treeListNode;
				StartAddingModule(treeListNode);
			}
			else if (focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database)
			{
				moduleSummaryUserControl.SetEmptyModulesListPanelVisibility(visible: false);
				TreeListHelpers.ParentForAddModule = focusedTreeListNode;
				StartAddingModule(focusedTreeListNode);
			}
			else if (focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
			{
				moduleSummaryUserControl.SetEmptyModulesListPanelVisibility(visible: false);
				TreeListNode parentNode = focusedTreeListNode.ParentNode;
				TreeListHelpers.ParentForAddModule = parentNode;
				StartAddingModule(parentNode);
			}
		}
	}

	private void AddModuleFromComboControl(int? databaseId)
	{
		if (databaseId.HasValue)
		{
			lastFocusedNodeForModules = GetFocusedNode();
			moduleSummaryUserControl.ModuleAddingFromComboControl = true;
			TreeListHelpers.ParentForAddModule = SearchTreeNodeOperation.FindModuleDirectory(metadataTreeList.Nodes, databaseId.Value);
			StartAddingModule(TreeListHelpers.ParentForAddModule);
			moduleSummaryUserControl.ModuleAddingFromComboControl = false;
		}
	}

	private void StartAddingModule(TreeListNode parentNode)
	{
		if (parentNode != null && Licenses.CheckRepositoryVersionAfterLogin() && ContinueAfterPossibleChanges())
		{
			DBTreeNode node = TreeListHelpers.GetNode(parentNode);
			AddNewModule(parentNode, node);
		}
	}

	internal void RefreshProfilingInVisibleTableSummaryControl(int? tableId = null)
	{
		BaseSummaryUserControl visibleSummaryUserControl = GetVisibleSummaryUserControl();
		if (visibleSummaryUserControl == tableSummaryUserControl)
		{
			tableSummaryUserControl.RefreshTableProfiling(tableId);
		}
		else if (visibleSummaryUserControl == viewSummaryUserControl)
		{
			viewSummaryUserControl.RefreshTableProfiling(tableId);
		}
	}

	public void AddNewModuleFromObjectsList()
	{
		DBTreeNode dBTreeNode = GetFocusedNode()?.ParentNode;
		DBTreeNode dBTreeNode2 = null;
		if (dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database)
		{
			dBTreeNode2 = dBTreeNode;
		}
		else
		{
			dBTreeNode2 = dBTreeNode.Nodes.FirstOrDefault((DBTreeNode x) => x.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database);
			if (dBTreeNode2 == null)
			{
				dBTreeNode2 = dBTreeNode.ParentNode;
			}
		}
		TreeListNode treeListNode = metadataTreeList.FocusedNode.ParentNode.Nodes.FirstOrDefault();
		if (treeListNode == null)
		{
			treeListNode = metadataTreeList.FocusedNode.ParentNode.ParentNode.ParentNode.Nodes.FirstOrDefault();
		}
		CreateNewModuleFromObjectsList(treeListNode, dBTreeNode2);
	}

	private void AddNewModule(TreeListNode parentNode, DBTreeNode dbTreeNode)
	{
		metadataTreeList.CloseEditor();
		SetControlsDuringAddingModuleEnabled(enabled: false);
		if (dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database)
		{
			parentNode.ParentNode.Expand();
		}
		try
		{
			DBTreeNode dBTreeNode = CreateNewModuleNode(parentNode, dbTreeNode);
			metadataTreeList.OptionsBehavior.Editable = true;
			metadataTreeList.ShowEditor();
			DBTreeNode newModuleNode = dBTreeNode;
			RefreshRibbonModuleGroup(newModuleNode);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while adding subject area.", FindForm());
			SetControlsDuringAddingModuleEnabled(enabled: true);
		}
	}

	private DBTreeNode CreateNewModuleNode(TreeListNode parentNode, DBTreeNode dbTreeNode)
	{
		int? nodePositionIndex = null;
		DBTreeNode node = TreeListHelpers.GetNode(TreeListHelpers.CustomFocusedTreeListNode);
		if (node != null && node.ObjectType == SharedObjectTypeEnum.ObjectType.Module && parentNode.Nodes.Contains(TreeListHelpers.CustomFocusedTreeListNode))
		{
			nodePositionIndex = parentNode.Nodes.IndexOf(TreeListHelpers.CustomFocusedTreeListNode) + 1;
		}
		string newModuleName = GetNewModuleName(dbTreeNode);
		DBTreeNode dBTreeNode = dbTreeMenu.AddEmptyModule(dbTreeNode, -1, newModuleName, showProgress, nodePositionIndex);
		SetTreeFocusToNewNode(parentNode, dbTreeNode, dBTreeNode);
		InsertModuleIfNotAlreadyInserted();
		return dBTreeNode;
	}

	private void CreateNewModuleFromObjectsList(TreeListNode parentNode, DBTreeNode dbTreeNode)
	{
		metadataTreeList.CloseEditor();
		SetControlsDuringAddingModuleEnabled(enabled: false);
		try
		{
			CreateNewModuleNode(parentNode, dbTreeNode);
			DBTreeNode node = TreeListHelpers.GetNode(metadataTreeList.FocusedNode);
			DB.Module.Insert(node, node.Name, null, null, "STRAIGHT", erdShowTypes: false, "EXTERNAL_ENTITIES_ONLY", erdShowNullable: false, FindForm());
			OpenPageControl();
			RefreshRibbonModuleGroup(node);
			WorkWithDataedoTrackingHelper.TrackNewSubjectAreaAdd();
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while adding subject area.", FindForm());
		}
		finally
		{
			SetControlsDuringAddingModuleEnabled(enabled: true);
		}
	}

	private void DuplicateModule()
	{
		metadataTreeList.CloseEditor();
		GetVisibleSummaryUserControl()?.CloseEditor();
		TreeListNode customFocusedTreeListNode = TreeListHelpers.CustomFocusedTreeListNode;
		if (customFocusedTreeListNode == null)
		{
			return;
		}
		DBTreeNode node = TreeListHelpers.GetNode(customFocusedTreeListNode);
		if (node == null || node.ObjectType != SharedObjectTypeEnum.ObjectType.Module || GeneralMessageBoxesHandling.Show("Do you want to copy <b>" + node.Name + "</b> Subject Area?", "Duplicate?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, null, 1, FindForm()).DialogResult != DialogResult.OK)
		{
			return;
		}
		try
		{
			SetControlsDuringAddingModuleEnabled(enabled: false);
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			TreeListNode parentForAddModule = TreeListHelpers.ParentForAddModule;
			DBTreeNode node2 = TreeListHelpers.GetNode(parentForAddModule);
			CreateDuplicatedModuleNode(parentForAddModule, node2, node);
			metadataTreeList.OptionsBehavior.Editable = true;
			DBTreeNode node3 = TreeListHelpers.GetNode(metadataTreeList.FocusedNode);
			ModuleObject dataById = DB.Module.GetDataById(node.Id);
			if (dataById == null)
			{
				SetControlsDuringAddingModuleEnabled(enabled: true);
				CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
				return;
			}
			string descriptionSearch = null;
			using (RichEditDocumentServer richEditDocumentServer = new RichEditDocumentServer())
			{
				richEditDocumentServer.HtmlText = dataById.Description;
				descriptionSearch = richEditDocumentServer.Text;
			}
			int num = DB.Module.Insert(node3, node3.Name, dataById.Description, descriptionSearch, dataById.ErdLinkStyle, dataById.ErdShowTypes.GetValueOrDefault(), dataById.DisplayDocumentationNameMode);
			if (num >= 0)
			{
				Form owner = FindForm();
				DB.Module.CopyModuleObjects(node.Id, num, SharedObjectTypeEnum.ObjectType.Table, owner);
				DB.Module.CopyModuleObjects(node.Id, num, SharedObjectTypeEnum.ObjectType.Function, owner);
				DB.Module.CopyModuleObjects(node.Id, num, SharedObjectTypeEnum.ObjectType.Procedure, owner);
				DB.Module.CopyModuleObjects(node.Id, num, SharedObjectTypeEnum.ObjectType.View, owner);
				DB.Module.CopyModuleObjects(node.Id, num, SharedObjectTypeEnum.ObjectType.Structure, owner);
				DB.Module.CopyModuleDiagram(node.Id, num, owner);
				DB.Module.CopyModuleCustomFields(CustomFieldsSupport.Fields, dataById, node3.DatabaseId, num, node3.Name, descriptionSearch, owner);
				DB.Module.CopyModuleRelations(node.Id, num, owner);
			}
			DB.History.InsertHistoryRow(node3.DatabaseId, num, node3.Name, dataById.Description, dataById.DescriptionPlain, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType.Module), !string.IsNullOrEmpty(node3.Name), !string.IsNullOrEmpty(dataById.Description), SharedObjectTypeEnum.ObjectType.Module);
			ActualizeModulesOrder(TreeListHelpers.GetFocusedTreeListNode());
			SetControlsDuringAddingModuleEnabled(enabled: true);
			RefreshTree(showWaitForm: true, ShowProgress);
			RefreshRibbonModuleGroup(node3);
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.Module, num);
			metadataTreeList.ShowEditor();
			WorkWithDataedoTrackingHelper.TrackNewSubjectAreaAdd();
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
			GeneralExceptionHandling.Handle(exception, "Error while adding subject area.", FindForm());
			SetControlsDuringAddingModuleEnabled(enabled: true);
		}
	}

	private void CreateDuplicatedModuleNode(TreeListNode parentNode, DBTreeNode dbTreeNode, DBTreeNode moduleToDuplicate)
	{
		string duplicatedModuleName = GetDuplicatedModuleName(moduleToDuplicate);
		int num = moduleToDuplicate.ParentNode.Nodes.IndexOf(moduleToDuplicate);
		IEnumerable<DBTreeNode> enumerable = moduleToDuplicate.ParentNode.Nodes.Where((DBTreeNode x) => x.Name?.StartsWith(moduleToDuplicate.Name) ?? false);
		if (enumerable.Any())
		{
			foreach (DBTreeNode item in enumerable)
			{
				int num2 = moduleToDuplicate.ParentNode.Nodes.IndexOf(item);
				if (num2 > num)
				{
					num = num2;
				}
			}
		}
		DBTreeNode newNode = dbTreeMenu.AddEmptyModule(dbTreeNode, -1, duplicatedModuleName, showProgress, num + 1);
		SetTreeFocusToNewNode(parentNode, dbTreeNode, newNode);
	}

	private string GetDuplicatedModuleName(DBTreeNode dbTreeNode)
	{
		return FindingNewName.GetNewName(!DB.Module.DoNewModuleTitleExists(dbTreeNode.DatabaseId, dbTreeNode.Name ?? ""), dbTreeNode.Name ?? "", DB.Module.GetDataByDatabaseNewTitle(dbTreeNode.DatabaseId, dbTreeNode.Name ?? ""));
	}

	private void RefreshRibbonModuleGroup(DBTreeNode newModuleNode)
	{
		if (newModuleNode != null && this.SelectedObjectChanged != null)
		{
			this.SelectedObjectChanged(null, new ObjectTypeEventArgs(newModuleNode));
		}
	}

	private void SetTreeFocusToNewNode(TreeListNode parentNode, DBTreeNode dbTreeNode, DBTreeNode newNode)
	{
		metadataTreeList.FocusedColumn = metadataTreeList.Columns["name"];
		TreeListNode treeListNode = SearchTreeNodeOperation.FindModuleNode(parentNode, metadataTreeList.Nodes, dbTreeNode.DatabaseId, newNode);
		if (treeListNode != null)
		{
			metadataTreeList.FocusedNode = treeListNode;
		}
	}

	private string GetNewModuleName(DBTreeNode dbTreeNode)
	{
		return FindingNewName.GetNewName(!DB.Module.DoNewModuleTitleExists(dbTreeNode.DatabaseId), "New subject area", DB.Module.GetDataByDatabaseNewTitle(dbTreeNode.DatabaseId));
	}

	public void StartAddingNewDatabase()
	{
		string newName = FindingNewName.GetNewName(!DB.Database.GetDatabasesNewDatabaseTitle(), "New database", DB.Database.GetDatabaseNewTitle());
		int? num = DB.Database.InsertManualDatabase(newName, FindForm());
		if (num.HasValue)
		{
			DB.Database.UpdateManualDatabaseName(num, FindForm());
			DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.Database, num);
			DB.History.InsertHistoryRow(num, num, newName, null, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType.Database), saveTitle: true, saveDescription: false, SharedObjectTypeEnum.ObjectType.Database);
			AddDatabaseNode(num);
			WorkWithDataedoTrackingHelper.TrackNewManualDatabaseAdd();
			metadataTreeList.FocusedNode = SearchTreeNodeOperation.FindNode(metadataTreeList.Nodes, num.Value);
			metadataTreeList.FocusedColumn = metadataTreeList.Columns["name"];
			metadataTreeList.OptionsBehavior.Editable = true;
			if (metadataTreeList.CanShowEditor)
			{
				metadataTreeList.ShowEditor();
			}
		}
	}

	private void metadataTreeList_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		if (!TreeListHelpers.DisableFocusEvents)
		{
			TreeListHelpers.FocusedNodeChanged(sender, e, delegate(TreeListNode node)
			{
				ProcessFocusNode(node);
			});
		}
	}

	private void metadataTreeList_BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e)
	{
		if (!TreeListHelpers.DisableFocusEvents)
		{
			TreeListHelpers.BeforeFocusNode(sender, e, () => ContinueAfterPossibleChanges(), FindForm());
		}
	}

	private void metadataTreeList_BeforeCollapse(object sender, BeforeCollapseEventArgs e)
	{
		Point pt = metadataTreeList.PointToClient(Control.MousePosition);
		if (metadataTreeList.CalcHitInfo(pt).Column == progressTreeListColumn)
		{
			e.CanCollapse = false;
			return;
		}
		TreeListHelpers.BeforeCollapse(sender, e, () => ContinueAfterPossibleChanges());
	}

	public void EnableFocusNodeChangedEventActions()
	{
		TreeListHelpers.DisableFocusNodeChangedEventActions = false;
	}

	private void ProcessFocusNode(TreeListNode node)
	{
		TreeListHelpers.StartedToFocus = false;
		if (TreeListHelpers.DisableFocusNodeChangedEventActions)
		{
			TreeListHelpers.DisableFocusNodeChangedEventActions = false;
			if (metadataTreeList == null || metadataTreeList.Nodes.Count == 0)
			{
				ClearDataPanel();
			}
			return;
		}
		if (selectedFormType.HasValue && (searchResultsListGridControl.DataSource != null || searchResultsTreeList.DataSource != null))
		{
			ClearSearchSelection();
		}
		SwitchOffEditablitity();
		if (this.SelectedObjectChanged != null)
		{
			ChangeRibbonIcons(node);
		}
		OpenPageControl();
	}

	private DBTreeNode ChangeRibbonIcons(TreeListNode currentNode)
	{
		DBTreeNode node = TreeListHelpers.GetNode(currentNode);
		if (node != null)
		{
			this.SelectedObjectChanged(null, new ObjectTypeEventArgs(node));
		}
		else
		{
			this.SelectedObjectChanged(null, null);
		}
		return node;
	}

	public void ClearDataPanel()
	{
		metadataEditorSplitContainerControl.Panel2.Controls.Clear();
		selectedFormType = null;
		this.SelectedObjectChanged?.Invoke(null, null);
	}

	public DBTreeNode GetFocusedNode(bool fromCustomFocus = false)
	{
		return TreeListHelpers.GetFocusedNode(fromCustomFocus);
	}

	public void SelectNodeOnTree(int? objectId, SharedObjectTypeEnum.ObjectType type, TreeListNode node)
	{
		string name = SharedObjectTypeEnum.TypeToStringForMenu(type);
		if (!ContinueAfterPossibleChanges())
		{
			return;
		}
		node.Expanded = true;
		TreeListNode treeListNode = SearchTreeNodeOperation.FindNode(node.Nodes, name);
		if (treeListNode == null)
		{
			treeListNode = SearchTreeNodeOperation.FindNode(node.ParentNode?.Nodes, name);
			if (treeListNode == null)
			{
				treeListNode = SearchTreeNodeOperation.FindNode(node.ParentNode.ParentNode?.Nodes, name);
				if (treeListNode == null)
				{
					return;
				}
			}
		}
		bool expanded = treeListNode.Expanded;
		treeListNode.Expanded = true;
		if (objectId.HasValue)
		{
			TreeListNode treeListNode2 = SearchTreeNodeOperation.FindNode(treeListNode.Nodes, objectId.Value);
			if (treeListNode2 == null)
			{
				if (!expanded)
				{
					treeListNode.Expanded = false;
				}
			}
			else
			{
				SelectNodeInTree(treeListNode2);
				metadataTreeList.RefreshNode(treeListNode);
			}
		}
		else
		{
			SelectNodeInTree(treeListNode);
		}
	}

	public void SelectNodeFromCurrentModule(Node erdNode)
	{
		if (!ContinueAfterPossibleChanges())
		{
			return;
		}
		metadataTreeList.FocusedNode.Expanded = true;
		TreeListNode treeListNode = SearchTreeNodeOperation.FindNode(metadataTreeList.FocusedNode.Nodes, NodeTypeEnum.TypeToPluralString(erdNode.Type));
		if (treeListNode == null)
		{
			return;
		}
		bool expanded = treeListNode.Expanded;
		treeListNode.Expanded = true;
		TreeListNode treeListNode2 = SearchTreeNodeOperation.FindNode(treeListNode.Nodes, erdNode.TableId);
		if (treeListNode2 == null)
		{
			if (!expanded)
			{
				treeListNode.Expanded = false;
			}
		}
		else
		{
			SelectNodeInTree(treeListNode2);
		}
	}

	public void SelectNodeFromOtherModule(Node erdNode)
	{
		if (!ContinueAfterPossibleChanges())
		{
			return;
		}
		TreeListNode treeListNode = SearchTreeNodeOperation.FindNode(metadataTreeList.Nodes, erdNode.DatabaseId);
		if (treeListNode == null)
		{
			return;
		}
		bool expanded = treeListNode.Expanded;
		treeListNode.Expanded = true;
		TreeListNode treeListNode2 = SearchTreeNodeOperation.FindNode(treeListNode.Nodes, NodeTypeEnum.TypeToPluralString(erdNode.Type));
		if (treeListNode2 == null)
		{
			if (!expanded)
			{
				treeListNode.Expanded = false;
			}
			return;
		}
		bool expanded2 = treeListNode2.Expanded;
		treeListNode2.Expanded = true;
		TreeListNode treeListNode3 = SearchTreeNodeOperation.FindNode(treeListNode2.Nodes, erdNode.TableId);
		if (treeListNode3 == null)
		{
			if (!expanded)
			{
				treeListNode.Expanded = false;
			}
			if (!expanded2)
			{
				treeListNode2.Expanded = false;
			}
		}
		else
		{
			SelectNodeInTree(treeListNode3);
		}
	}

	public void SelectDependencyObject(DependencyRow dependency)
	{
		if (!ContinueAfterPossibleChanges())
		{
			return;
		}
		SetWaitformVisibility(visible: true);
		SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToType(dependency.Type);
		string empty = string.Empty;
		if (objectType != SharedObjectTypeEnum.ObjectType.Table && objectType != SharedObjectTypeEnum.ObjectType.View && objectType != SharedObjectTypeEnum.ObjectType.Procedure && objectType != SharedObjectTypeEnum.ObjectType.Function && objectType != SharedObjectTypeEnum.ObjectType.Trigger && objectType != SharedObjectTypeEnum.ObjectType.Structure)
		{
			return;
		}
		if (objectType == SharedObjectTypeEnum.ObjectType.Trigger)
		{
			objectType = SharedObjectTypeEnum.ObjectType.Table;
			empty = dependency.TableName;
		}
		else
		{
			empty = dependency.Name;
		}
		string schema = dependency.RowDatabase.Schema;
		DBTreeNode focusedNode = GetFocusedNode();
		if (focusedNode.IsInModule && focusedNode.DatabaseId == dependency.DestinationDatabaseId && SelectDependencyInModule(empty, objectType, schema, dependency.TypeToGoTo))
		{
			return;
		}
		TreeListNode treeListNode = SearchTreeNodeOperation.FindNode(metadataTreeList.Nodes, dependency.DestinationDatabaseId ?? dependency.RowDatabase.DocumentationId);
		if (treeListNode == null)
		{
			return;
		}
		bool expanded = treeListNode.Expanded;
		treeListNode.Expanded = true;
		TreeListNode treeListNode2 = SearchTreeNodeOperation.FindNode(treeListNode.Nodes, SharedObjectTypeEnum.TypeToStringForMenu(objectType));
		if (treeListNode2 == null)
		{
			if (!expanded)
			{
				treeListNode.Expanded = false;
			}
			return;
		}
		bool expanded2 = treeListNode2.Expanded;
		treeListNode2.Expanded = true;
		TreeListNode treeListNode3 = SearchTreeNodeOperation.FindDependencyNode(treeListNode2.Nodes, empty, schema);
		if (treeListNode3 == null)
		{
			if (!expanded)
			{
				treeListNode.Expanded = false;
			}
			if (!expanded2)
			{
				treeListNode2.Expanded = false;
			}
		}
		else
		{
			SelectNodeInTree(treeListNode3, dependency.TypeToGoTo);
		}
	}

	private bool SelectDependencyInModule(string name, SharedObjectTypeEnum.ObjectType? type, string schema, DependencyRow.DependencyNodeCommonType? dependencyType = null)
	{
		GetFocusedNode();
		TreeListNode treeListNode = metadataTreeList.FocusedNode?.ParentNode?.ParentNode;
		if (treeListNode == null)
		{
			return false;
		}
		bool expanded = treeListNode.Expanded;
		treeListNode.Expanded = true;
		TreeListNode treeListNode2 = SearchTreeNodeOperation.FindNode(treeListNode.Nodes, SharedObjectTypeEnum.TypeToStringForMenu(type));
		if (treeListNode2 == null)
		{
			if (!expanded)
			{
				treeListNode.Expanded = false;
			}
			return false;
		}
		bool expanded2 = treeListNode2.Expanded;
		treeListNode2.Expanded = true;
		TreeListNode treeListNode3 = SearchTreeNodeOperation.FindDependencyNode(treeListNode2.Nodes, name, schema);
		if (treeListNode3 == null)
		{
			if (!expanded)
			{
				treeListNode.Expanded = false;
			}
			if (!expanded2)
			{
				treeListNode2.Expanded = false;
			}
			return false;
		}
		SelectNodeInTree(treeListNode3, dependencyType);
		return true;
	}

	private void SelectNodeInTree(TreeListNode node, DependencyRow.DependencyNodeCommonType? dependencyType = null, string filterColumn = null)
	{
		SwitchOffEditablitity();
		if (this.SelectedObjectChanged != null)
		{
			DBTreeNode dbTreeNode = ChangeRibbonIcons(node);
			if (dependencyType.HasValue)
			{
				TreeListHelpers.DisableFocusNodeChangedEventActions = true;
			}
			if (metadataTreeList.FocusedNode != node)
			{
				metadataTreeList.SetFocusedNode(node);
				OpenPageControl(showControl: true, dbTreeNode, dependencyType, filterColumn);
			}
		}
	}

	public void SelectSchemaImportsAndChangesObject(int? moduleId, SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel)
	{
		TreeListNode treeListNode = metadataTreeList.Nodes.FirstOrDefault((TreeListNode x) => schemaImportsAndChangesObjectModel.Data.DatabaseId == ((DBTreeNode)x.TreeList.GetDataRecordByNode(x)).DatabaseId);
		if (treeListNode == null)
		{
			return;
		}
		SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel2 = schemaImportsAndChangesObjectModel;
		if (schemaImportsAndChangesObjectModel2 == null)
		{
			return;
		}
		SchemaImportsAndChangesObject data = schemaImportsAndChangesObjectModel2.Data;
		if (data == null || !data.ObjectIdCurrent.HasValue)
		{
			return;
		}
		SchemaImportsAndChangesObjectModel schemaImportsAndChangesObjectModel3 = schemaImportsAndChangesObjectModel;
		if (schemaImportsAndChangesObjectModel3 == null || !schemaImportsAndChangesObjectModel3.ObjectType.HasValue)
		{
			return;
		}
		bool flag = false;
		if (moduleId.HasValue)
		{
			TreeListNode treeListNode2 = treeListNode.Nodes.FirstOrDefault((TreeListNode x) => ((DBTreeNode)x.TreeList.GetDataRecordByNode(x)).ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database);
			bool expanded = treeListNode2.Expanded;
			treeListNode2.Expanded = true;
			TreeListNode treeListNode3 = treeListNode2.Nodes.FirstOrDefault(delegate(TreeListNode x)
			{
				DBTreeNode dBTreeNode = (DBTreeNode)x.TreeList.GetDataRecordByNode(x);
				return dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module && moduleId == dBTreeNode.Id;
			});
			bool expanded2 = treeListNode3.Expanded;
			treeListNode3.Expanded = true;
			flag = SelectObjectInDocumentationNode(treeListNode3, schemaImportsAndChangesObjectModel.Data.ObjectIdCurrent.Value, schemaImportsAndChangesObjectModel.ObjectType.Value);
			if (!flag)
			{
				treeListNode3.Expanded = expanded2;
				treeListNode2.Expanded = expanded;
			}
		}
		if (!flag)
		{
			SelectObjectInDocumentationNode(treeListNode, schemaImportsAndChangesObjectModel.Data.ObjectIdCurrent.Value, schemaImportsAndChangesObjectModel.ObjectType.Value);
		}
	}

	internal void AddNewTable(SharedObjectTypeEnum.ObjectType objectType)
	{
		ModuleUserControl obj = moduleUserControl;
		SharedObjectTypeEnum.ObjectType? objectType2 = objectType;
		obj.AddNode(null, objectType2);
	}

	internal void AddNewPostIt()
	{
		moduleUserControl.AddPostIt();
	}

	internal bool ShowSuggestedEntities()
	{
		return moduleUserControl.ShowSuggestedEntities();
	}

	internal void EditDisplayOptionsOnERD()
	{
		moduleUserControl.EditSelectedERDNode();
	}

	internal void RemoveSelectedObjects()
	{
		moduleUserControl.RemoveSelectedObjects();
	}

	internal void DesignSelectedObject()
	{
		moduleUserControl.DesignSelectedObject();
	}

	internal void EditSelectedERDNode()
	{
		moduleUserControl.EditSelectedERDNode();
	}

	internal void EditSelectedERDLink()
	{
		moduleUserControl.EditSelectedERDLink();
	}

	internal void SelectObject(int databaseId, int objectId, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (!ContinueAfterPossibleChanges())
		{
			return;
		}
		bool? obj = GetFocusedNode()?.IsInModule;
		DBTreeNode focusedNode = GetFocusedNode();
		TreeListNode treeListNode = ((GetFocusedNode().ObjectType != SharedObjectTypeEnum.ObjectType.Database) ? ((focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module) ? metadataTreeList.FocusedNode : metadataTreeList.FocusedNode?.ParentNode?.ParentNode) : metadataTreeList.FocusedNode);
		if (treeListNode == null)
		{
			treeListNode = metadataTreeList.FocusedNode?.ParentNode;
		}
		int? num = ((DBTreeNode)treeListNode.TreeList.GetDataRecordByNode(treeListNode))?.DatabaseId;
		if (obj == true || focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			if (databaseId == num && SelectObjectInDocumentationNode(treeListNode, objectId, objectType))
			{
				return;
			}
			treeListNode = metadataTreeList.FocusedNode?.ParentNode?.ParentNode?.ParentNode?.ParentNode;
		}
		if (databaseId != num)
		{
			treeListNode = treeListNode.TreeList.FindNode((TreeListNode x) => ((DBTreeNode)x.TreeList.GetDataRecordByNode(x)).DatabaseId == databaseId);
		}
		if (treeListNode != null)
		{
			SelectObjectInDocumentationNode(treeListNode, objectId, objectType);
		}
	}

	private bool SelectObjectInDocumentationNode(TreeListNode mainNode, int objectId, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (mainNode == null)
		{
			return false;
		}
		DBTreeNode dBTreeNode = (DBTreeNode)mainNode.TreeList.GetDataRecordByNode(mainNode);
		if (dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module || dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Database)
		{
			mainNode.Expanded = true;
		}
		TreeListNode treeListNode = SearchTreeNodeOperation.FindNode(mainNode.Nodes, SharedObjectTypeEnum.TypeToStringForMenu(objectType));
		if (treeListNode == null)
		{
			return false;
		}
		bool expanded = treeListNode.Expanded;
		treeListNode.Expanded = true;
		TreeListNode treeListNode2 = SearchTreeNodeOperation.FindNode(treeListNode.Nodes, objectId);
		if (treeListNode2 == null)
		{
			if (!expanded)
			{
				treeListNode.Expanded = false;
			}
			return false;
		}
		SelectNodeInTree(treeListNode2);
		return true;
	}

	private DBTreeNode FindNodeAndFocus(SharedObjectTypeEnum.ObjectType objectType, ObjectEventArgs databaseEventArgs)
	{
		metadataTreeList.FocusedNode = SearchTreeNodeOperation.FindNode(metadataTreeList.FocusedNode, metadataTreeList.Nodes, objectType, databaseEventArgs.DatabaseId, databaseEventArgs.ObjectId, databaseEventArgs.ModuleId);
		return (DBTreeNode)metadataTreeList.GetDataRecordByNode(metadataTreeList.FocusedNode);
	}

	private void ShowObjectUserControl(SharedObjectTypeEnum.ObjectType objectType, CustomFieldsSupport customFieldsSupport, ObjectEventArgs databaseEventArgs)
	{
		DBTreeNode dBTreeNode = FindNodeAndFocus(objectType, databaseEventArgs);
		if (dBTreeNode != null)
		{
			ShowObjectUserControl(objectType, dBTreeNode, customFieldsSupport);
		}
	}

	public void SelectObjectAndShowDataLineageTab(SharedObjectTypeEnum.ObjectType objectType, ObjectEventArgs databaseEventArgs, int? processId, bool selectDiagramTab = false, bool showColumns = false)
	{
		TreeListNode treeListNode = SearchTreeNodeOperation.FindNode(null, metadataTreeList.Nodes, objectType, databaseEventArgs.DatabaseId, databaseEventArgs.ObjectId, databaseEventArgs.ModuleId);
		if (treeListNode == null)
		{
			return;
		}
		if (metadataTreeList.FocusedNode != treeListNode)
		{
			if (!ContinueAfterPossibleChanges())
			{
				return;
			}
			metadataTreeList.FocusedNode = treeListNode;
		}
		if (GetVisibleUserControl() is IPanelWithDataLineage panelWithDataLineage)
		{
			panelWithDataLineage.FocusDataLineageTab(processId, selectDiagramTab, showColumns);
		}
	}

	private void ShowObjectUserControl(SharedObjectTypeEnum.ObjectType objectType, DBTreeNode dbTreeNode, CustomFieldsSupport customFieldsSupport, bool showControl = true, DependencyRow.DependencyNodeCommonType? dependencyType = null)
	{
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			BasePanelControl basePanelControl;
			switch (objectType)
			{
			case SharedObjectTypeEnum.ObjectType.Table:
			case SharedObjectTypeEnum.ObjectType.View:
			case SharedObjectTypeEnum.ObjectType.Structure:
				basePanelControl = tableUserControl;
				break;
			case SharedObjectTypeEnum.ObjectType.Function:
			case SharedObjectTypeEnum.ObjectType.Procedure:
				basePanelControl = procedureUserControl;
				break;
			case SharedObjectTypeEnum.ObjectType.Module:
				basePanelControl = moduleUserControl;
				break;
			case SharedObjectTypeEnum.ObjectType.Term:
				basePanelControl = termUserControl;
				break;
			default:
				return;
			}
			BasePanelControl visibleUserControl = GetVisibleUserControl();
			if (visibleUserControl != null)
			{
				visibleUserControl.CustomFieldsPanelControl.Visible = false;
				visibleUserControl.CancelSuggestedDescriptions();
			}
			if (showControl)
			{
				basePanelControl.CurrentNode = dbTreeNode;
				basePanelControl.HideCustomization();
			}
			basePanelControl.SetParameters(dbTreeNode, customFieldsSupport, dependencyType);
			SetERDGroupVisibility(new BoolEventArgs(objectType == SharedObjectTypeEnum.ObjectType.Module && moduleUserControl.IsERDTabPageActive));
			SetModulePositionButtonsVisibility(new BoolEventArgs(objectType == SharedObjectTypeEnum.ObjectType.Module && moduleUserControl.IsDescriptionTabPageActive));
			selectedFormType = objectType;
			if (showControl)
			{
				CommonFunctionsPanels.ShowUserControlInPanel(basePanelControl, metadataEditorSplitContainerControl.Panel2);
			}
		}
		catch (Exception)
		{
			throw;
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private void metadataTreeList_GetSelectImage(object sender, GetSelectImageEventArgs e)
	{
		TreeListHelpers.GetSelectImage(sender, e, treeMenuImageCollection);
	}

	public void DeleteDbElement(bool onlyFromModule = false, bool fromCustomFocus = false)
	{
		TreeListHelpers.DeleteDbElement(this, splashScreenManager, dbTreeMenu, ContinueAfterPossibleChanges, GetVisibleUserControl, ClearDataPanel, onlyFromModule, fromCustomFocus);
	}

	public void RemoveFromUncheckedModule(KeyValuePair<int, int> keyValuePair, DBTreeNode dbNode)
	{
		TreeListNode rootNode = metadataTreeList?.Nodes.SelectMany((TreeListNode t) => t.Nodes)?.FirstOrDefault((TreeListNode x) => x.HasChildren && TreeListHelpers.GetNode(x).DatabaseId == keyValuePair.Value && TreeListHelpers.GetNode(x).Nodes.FirstOrDefault((DBTreeNode modulesFolder) => modulesFolder.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database).Nodes.Any((DBTreeNode module) => module.Nodes.Any((DBTreeNode objectFolder) => objectFolder.Nodes.Any((DBTreeNode node) => node.Id == dbNode.Id))));
		RemoveFromModuleInTree(dbNode, rootNode, keyValuePair.Key);
		TreeListNode mainNode = metadataTreeList?.Nodes.FirstOrDefault((TreeListNode x) => x.HasChildren && TreeListHelpers.GetNode(x).DatabaseId == dbNode.DatabaseId);
		DBTreeNode focusedNode = GetFocusedNode();
		if (focusedNode == null || keyValuePair.Key == focusedNode.Id)
		{
			SelectObjectInDocumentationNode(mainNode, dbNode.Id, dbNode.ObjectType);
		}
	}

	public void RemoveFromModulesInTree(DBTreeNode dbTreeNode)
	{
		if (!dbTreeNode.IsNormalObject)
		{
			return;
		}
		IEnumerable<TreeListNode> enumerable = metadataTreeList?.Nodes.SelectMany((TreeListNode t) => t.Nodes)?.Where((TreeListNode x) => x.HasChildren && (TreeListHelpers.GetNode(x).Nodes.FirstOrDefault((DBTreeNode modulesFolder) => modulesFolder.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database)?.Nodes.Any((DBTreeNode module) => module.Nodes.Any((DBTreeNode objectFolder) => objectFolder.Nodes.Any((DBTreeNode node) => node.Id == dbTreeNode.Id))) ?? false));
		if (enumerable == null || enumerable.Count() == 0)
		{
			return;
		}
		foreach (TreeListNode item in enumerable)
		{
			RemoveFromModulesInTree(dbTreeNode, item);
		}
	}

	private void RemoveFromModulesInTree(DBTreeNode dbTreeNode, TreeListNode rootNode)
	{
		if (dbTreeNode == null || rootNode == null)
		{
			return;
		}
		Stack<TreeListNode> nodesToCollapse = new Stack<TreeListNode>();
		AddToListIfNotExpandedAndExpand(nodesToCollapse, rootNode);
		TreeListNode treeListNode = rootNode?.Nodes.SingleOrDefault((TreeListNode x) => TreeListHelpers.GetNode(x).ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database);
		AddToListIfNotExpandedAndExpand(nodesToCollapse, treeListNode);
		IEnumerable<TreeListNode> enumerable = ((from x in treeListNode?.Nodes
			where TreeListHelpers.GetNode(x).ObjectType == SharedObjectTypeEnum.ObjectType.Module
			select (x)))?.SelectMany(delegate(TreeListNode x)
		{
			AddToListIfNotExpandedAndExpand(nodesToCollapse, x);
			return x.Nodes.Where((TreeListNode y) => TreeListHelpers.GetNode(y).BaseName.Equals(dbTreeNode.ParentNode.BaseName));
		});
		TreeListNode[] array = enumerable?.SelectMany(delegate(TreeListNode x)
		{
			AddToListIfNotExpandedAndExpand(nodesToCollapse, x);
			return x.Nodes.Where((TreeListNode y) => TreeListHelpers.GetNode(y).Id == dbTreeNode.Id);
		}).ToArray();
		for (int i = 0; i < array?.Length; i++)
		{
			metadataTreeList.DeleteNode(array[i]);
		}
		foreach (TreeListNode item in enumerable)
		{
			if (item != null)
			{
				metadataTreeList.RefreshNode(item);
			}
		}
		foreach (TreeListNode item2 in nodesToCollapse)
		{
			item2.Expanded = false;
		}
	}

	private void RemoveFromModuleInTree(DBTreeNode dbTreeNode, TreeListNode rootNode, int moduleId)
	{
		if (dbTreeNode == null || rootNode == null)
		{
			return;
		}
		Stack<TreeListNode> stack = new Stack<TreeListNode>();
		AddToListIfNotExpandedAndExpand(stack, rootNode);
		TreeListNode treeListNode = rootNode?.Nodes.SingleOrDefault((TreeListNode x) => TreeListHelpers.GetNode(x).ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database);
		AddToListIfNotExpandedAndExpand(stack, treeListNode);
		TreeListNode treeListNode2 = treeListNode?.Nodes.FirstOrDefault((TreeListNode x) => TreeListHelpers.GetNode(x).Id == moduleId);
		AddToListIfNotExpandedAndExpand(stack, treeListNode2);
		foreach (TreeListNode node in treeListNode2.Nodes)
		{
			AddToListIfNotExpandedAndExpand(stack, node);
			TreeListNode treeListNode4 = node.Nodes.FirstOrDefault((TreeListNode x) => TreeListHelpers.GetNode(x).Id == dbTreeNode.Id);
			if (treeListNode4 != null)
			{
				if (!stack.Contains(treeListNode4))
				{
					AddToListIfNotExpandedAndExpand(stack, treeListNode2);
				}
				metadataTreeList.DeleteNode(treeListNode4);
				metadataTreeList.RefreshNode(node);
			}
		}
		foreach (TreeListNode item in stack)
		{
			item.Expanded = false;
		}
	}

	private void AddToListIfNotExpandedAndExpand(Stack<TreeListNode> list, TreeListNode element)
	{
		if (element != null && !element.Expanded)
		{
			list.Push(element);
			element.Expanded = true;
		}
	}

	private void metadataTreeList_KeyDown(object sender, KeyEventArgs e)
	{
		TreeListHelpers.KeyDown(sender, e, this, splashScreenManager, dbTreeMenu, ContinueAfterPossibleChanges, GetVisibleUserControl, ClearDataPanel);
	}

	private void RenameNode(Form owner = null)
	{
		DBTreeNode focusedNode = GetFocusedNode();
		if (focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table || focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Structure)
		{
			RenameTableNode(focusedNode, owner);
		}
		else if (focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Term)
		{
			RenameTermNode(focusedNode);
		}
		else if (focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
		{
			RenameModuleNode(focusedNode);
		}
	}

	private void RenameModuleNode(DBTreeNode node)
	{
		if (node != null)
		{
			node.BaseName = node.Name;
		}
	}

	private void RenameTableNode(DBTreeNode node, Form owner = null)
	{
		try
		{
			tableUserControl.SetDisableSettingAsEdited(value: true);
			TableValidator tableValidator = new TableValidator(node.DatabaseId, tableUserControl.TableId, node.ObjectType);
			node.ChangeManualTableSchemaAndName(node.ObjectType != SharedObjectTypeEnum.ObjectType.Structure);
			if (!tableValidator.Exists(node.Schema, node.BaseName))
			{
				foreach (DBTreeNode item in DBTreeMenu.FindNodesById(node.ObjectType, node.Id))
				{
					DBTreeMenu.RefreshNodeName(node.DatabaseId, node.ObjectType, node.BaseName, node.OldBaseName, node.Schema, node.OldSchema, node.Title, SharedDatabaseTypeEnum.DatabaseType.Manual, node.ShowSchema, node.Subtype);
					node.Name = DBTreeMenu.SetName(node.Schema, node.BaseName, node.Title, node.ObjectType, node.DatabaseType, node.ShowSchema);
					item.OldBaseName = node.BaseName;
					item.OldSchema = node.Schema;
					item.OldName = node.Name;
				}
				tableUserControl.SetTitleTextEdit(node.Title);
				tableUserControl.SetLabels(node.Schema, node.BaseName, node.Title, node.ObjectType, node.Subtype);
				DB.Table.UpdateManualTable(tableUserControl.TableId, node.Schema, node.BaseName, node.Title, null, SharedObjectSubtypeEnum.TypeToString(node.ObjectType, node.Subtype), null, updateDefinition: false, updateLocation: false, UserTypeEnum.TypeToString(node.Source.GetValueOrDefault()), FindForm());
				DB.Database.UpdateDocumentationShowSchemaFlag(node.DatabaseId, owner);
			}
			else
			{
				node.Schema = node.OldSchema;
				node.BaseName = node.OldBaseName;
				node.Name = node.OldName;
			}
		}
		catch
		{
			throw;
		}
		finally
		{
			tableUserControl.SetDisableSettingAsEdited(value: false);
		}
	}

	private void RenameTermNode(DBTreeNode node)
	{
		if (!DB.BusinessGlossary.UpdateTermTitle(node.Id, node.Name, FindForm()))
		{
			node.Name = node.OldName;
			return;
		}
		bool saveTitle = node.BaseName != node.Name;
		node.BaseName = node.Name;
		DB.History.InsertHistoryRow(node.DatabaseId, node.Id, node.Name, null, null, "glossary_terms", saveTitle, saveDescription: false, SharedObjectTypeEnum.ObjectType.Term);
		DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.Term, node.Id);
		try
		{
			termUserControl.SetDisableSettingAsEdited(value: true);
			termUserControl.SetTitleTextEdit(node.Name);
		}
		finally
		{
			termUserControl.SetDisableSettingAsEdited(value: false);
		}
	}

	private void SwitchOffEditablitity()
	{
		metadataTreeList.OptionsBehavior.Editable = false;
	}

	public void ShowDocXtraReportForm()
	{
		int databaseId = GetFocusedNode().DatabaseId;
		if (databaseId > 0)
		{
			new DocGeneratingForm(databaseId).Show();
		}
		else
		{
			GeneralMessageBoxesHandling.Show("You have to select database or database's object for which you would like to create documentation", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
		}
	}

	public void ShowDocCreator(bool fromCustomFocus = false)
	{
		try
		{
			metadataTreeList.CloseEditor();
			DBTreeNode dBTreeNode = GetFocusedNode(fromCustomFocus);
			if (dBTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Repository)
			{
				dBTreeNode = GetFocusedNode(fromCustomFocus)?.GetClosestNode(SharedObjectTypeEnum.ObjectType.Database);
				if (dBTreeNode == null)
				{
					dBTreeNode = GetFocusedNode(fromCustomFocus)?.GetClosestNode(SharedObjectTypeEnum.ObjectType.BusinessGlossary);
				}
			}
			ShowDocCreator(dBTreeNode);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	public void ShowDocCreator(DBTreeNode dbTreeNode)
	{
		if (dbTreeNode != null)
		{
			DocWizardForm docWizardForm = new DocWizardForm((dbTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Repository) ? new int?(dbTreeNode.DatabaseId) : null, dbTreeNode.ObjectType, dbTreeNode.DatabaseType, this);
			docWizardForm.ShowDialog(this, setCustomMessageDefaultOwner: true);
			StaticData.ClearDatabaseInfoForCrashes();
			if (docWizardForm.HasCopiedInCurrentRepository())
			{
				RefreshTree(showWaitForm: true, progressType != null && ShowProgress);
			}
		}
		else
		{
			GeneralMessageBoxesHandling.Show("You have to select database or database's object for which you would like to create documentation", string.Empty, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
		}
	}

	private void titleRepositoryItemTextEdit_Validating(object sender, CancelEventArgs e)
	{
		TextEdit textEdit = sender as TextEdit;
		if (textEdit.IsEditorActive)
		{
			string text = textEdit.Text;
			DBTreeNode focusedNode = GetFocusedNode();
			if (focusedNode != null)
			{
				if (string.IsNullOrWhiteSpace(text))
				{
					textEdit.Text = focusedNode.Name;
					return;
				}
				switch (focusedNode.ObjectType)
				{
				case SharedObjectTypeEnum.ObjectType.Database:
				case SharedObjectTypeEnum.ObjectType.BusinessGlossary:
					DB.Database.UpdateTitle(focusedNode, text, FindForm());
					DB.History.InsertHistoryRow(focusedNode.DatabaseId, focusedNode.DatabaseId, text, null, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(focusedNode.ObjectType), HistoryGeneralHelper.CheckAreValuesDiffrent(text, focusedNode.Title), saveDescription: false, focusedNode.ObjectType);
					focusedNode.Title = text;
					databaseUserControl.SetNewTitle(text);
					RefreshSearchDatabasesFromMetadataTreeList(focusedNode.Id, text);
					RebuildHomePage(forceReload: false);
					break;
				case SharedObjectTypeEnum.ObjectType.Module:
					if ((focusedNode.Id != -1 || focusedNode.InsertElement != 0) && DB.Module.Update(focusedNode.Id, text, null, null, null, "STRAIGHT", erdShowTypes: false, "EXTERNAL_ENTITIES_ONLY", erdShowNullable: false, FindForm()))
					{
						if (!TreeListHelpers.DisableFocusEvents)
						{
							moduleUserControl.SetNewTitle(text);
							UpdateListView(TreeListHelpers.GetNode(TreeListHelpers.ParentForAddModule), CustomFieldsSupport, showControl: false);
							moduleUserControl.ModuleId = focusedNode.Id;
							moduleUserControl.SetNewTitle(text);
						}
						DB.History.InsertHistoryRow(focusedNode.DatabaseId, focusedNode.Id, text, null, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(focusedNode.ObjectType), HistoryGeneralHelper.CheckAreValuesDiffrent(text, focusedNode.Title), saveDescription: false, focusedNode.ObjectType);
					}
					break;
				}
			}
		}
		SwitchOffEditablitity();
	}

	private void titleRepositoryItemTextEdit_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Escape)
		{
			DBTreeNode focusedNode = GetFocusedNode();
			if (metadataTreeList.FocusedNode != null && focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module && focusedNode.Id == -1)
			{
				TreeListHelpers.ParentForAddModule.Nodes.Remove(metadataTreeList.FocusedNode);
				SetControlsDuringAddingModuleEnabled(enabled: true);
			}
		}
		else if (e.KeyCode == Keys.Return)
		{
			DBTreeNode focusedNode2 = GetFocusedNode();
			if (metadataTreeList.FocusedNode != null && focusedNode2.ObjectType == SharedObjectTypeEnum.ObjectType.Module && focusedNode2.Id == -1 && focusedNode2.InsertElement != 0)
			{
				DB.Module.Insert(focusedNode2, focusedNode2.Name, null, null, "STRAIGHT", erdShowTypes: false, "EXTERNAL_ENTITIES_ONLY", erdShowNullable: false, FindForm());
				UpdateListView(TreeListHelpers.GetNode(TreeListHelpers.ParentForAddModule), CustomFieldsSupport, showControl: false);
				ProcessFocusNode(metadataTreeList.FocusedNode);
				SetControlsDuringAddingModuleEnabled(enabled: true);
				WorkWithDataedoTrackingHelper.TrackNewSubjectAreaAdd();
			}
			SwitchOffEditablitity();
		}
	}

	private void titleRepositoryItemTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		DBTreeNode focusedNode = GetFocusedNode();
		if (metadataTreeList.FocusedNode != null && focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module && focusedNode.Id == -1)
		{
			focusedNode.InsertElement = DBTreeNode.InsertElementEnum.OnValidating;
		}
	}

	private void titleRepositoryItemTextEdit_Leave(object sender, EventArgs e)
	{
		InsertModuleIfNotAlreadyInserted();
	}

	private void metadataTreeList_Leave(object sender, EventArgs e)
	{
		InsertModuleIfNotAlreadyInserted();
		SwitchOffEditablitity();
	}

	private void InsertModuleIfNotAlreadyInserted()
	{
		DBTreeNode focusedNode = GetFocusedNode();
		if (focusedNode != null && focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Module && focusedNode.Id == -1 && focusedNode.InsertElement != DBTreeNode.InsertElementEnum.AlreadyInserted)
		{
			focusedNode.Id = DB.Module.Insert(focusedNode, focusedNode.Name, null, null, "STRAIGHT", erdShowTypes: false, "EXTERNAL_ENTITIES_ONLY", erdShowNullable: false, FindForm());
			moduleUserControl.ModuleId = focusedNode.Id;
			ActualizeModulesOrder(metadataTreeList.FocusedNode);
			UpdateListView(TreeListHelpers.GetNode(TreeListHelpers.ParentForAddModule), CustomFieldsSupport, showControl: false);
			SwitchOffEditablitity();
			ProcessFocusNode(metadataTreeList.FocusedNode);
			SetControlsDuringAddingModuleEnabled(enabled: true);
			DB.Community.InsertFollowingToRepository(focusedNode.ObjectType, focusedNode.Id);
			DB.History.InsertHistoryRow(focusedNode.DatabaseId, focusedNode.Id, focusedNode.Name, null, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(focusedNode.ObjectType), saveTitle: true, saveDescription: false, focusedNode.ObjectType);
			WorkWithDataedoTrackingHelper.TrackNewSubjectAreaAdd();
		}
	}

	public bool Save()
	{
		try
		{
			metadataTreeList.CloseEditor();
			GetVisibleUserControl()?.CloseEditors();
			switch (selectedFormType)
			{
			case SharedObjectTypeEnum.ObjectType.Database:
				return databaseUserControl.Save();
			case SharedObjectTypeEnum.ObjectType.Table:
			case SharedObjectTypeEnum.ObjectType.View:
			case SharedObjectTypeEnum.ObjectType.Structure:
				return tableUserControl.Save();
			case SharedObjectTypeEnum.ObjectType.Module:
				return moduleUserControl.Save();
			case SharedObjectTypeEnum.ObjectType.Function:
			case SharedObjectTypeEnum.ObjectType.Procedure:
				return procedureUserControl.Save();
			case SharedObjectTypeEnum.ObjectType.Term:
				return termUserControl.Save();
			default:
				return false;
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
			return false;
		}
	}

	private void ClosePanelEditor()
	{
		GetVisibleSummaryUserControl()?.CloseEditor();
	}

	private void dbTreeToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		if (e.SelectedControl.Equals(metadataTreeList))
		{
			TreeListHitInfo treeListHitInfo = metadataTreeList.CalcHitInfo(e.ControlMousePosition);
			string value = string.Empty;
			DBTreeNode dBTreeNode = (DBTreeNode)metadataTreeList.GetDataRecordByNode(treeListHitInfo.Node);
			if ((treeListHitInfo != null && treeListHitInfo.HitInfoType == HitInfoType.SelectImage) || (dBTreeNode != null && dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database))
			{
				value = ((dBTreeNode.CustomSubtype != null) ? dBTreeNode.CustomSubtype : ToolTips.GetNodeDescription(dBTreeNode.ObjectType, dBTreeNode.Subtype, dBTreeNode.SynchronizeState, dBTreeNode.Source));
			}
			else if (treeListHitInfo != null && treeListHitInfo.HitInfoType == HitInfoType.Cell && treeListHitInfo.Column.Equals(progressTreeListColumn) && dBTreeNode.ProgressValue.HasValue)
			{
				value = $"{dBTreeNode.ProgressValue}% of {ProgressType.ColumnName} entered";
			}
			if (!string.IsNullOrWhiteSpace(value))
			{
				object @object = new TreeListCellToolTipInfo(treeListHitInfo.Node, treeListHitInfo.Column, null);
				e.Info = new ToolTipControlInfo(@object, value);
				e.Info.ToolTipType = ToolTipType.SuperTip;
			}
		}
	}

	private void metadataTreeList_DragEnter(object sender, DragEventArgs e)
	{
		TreeListHelpers.DragEnter(sender, e);
	}

	private void metadataTreeList_DragLeave(object sender, EventArgs e)
	{
		TreeListHelpers.DragLeave(sender, e);
	}

	private void metadataTreeList_DragOver(object sender, DragEventArgs e)
	{
		TreeListHelpers.DragOver(sender, e);
	}

	private void metadataTreeList_CalcNodeDragImageIndex(object sender, CalcNodeDragImageIndexEventArgs e)
	{
		if (!TreeListHelpers.CheckIfDraggingAllowed(e.DragArgs.Data, TreeListHelpers.GetDBTreeNode(e.Node)))
		{
			e.ImageIndex = -1;
		}
		else
		{
			e.ImageIndex = 1;
		}
	}

	private void metadataTreeList_DragDrop(object sender, DragEventArgs e)
	{
		TreeListHelpers.DragDrop(sender, e, FindForm());
		HomePanelUserControl obj = homePanelUserControl;
		if (obj != null && obj.Visible)
		{
			homePanelUserControl?.RefreshDatabasesList();
		}
	}

	private void metadataTreeList_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		TreeListHelpers.NodeCellStyle(sender, e);
	}

	private void metadataTreeList_PopupMenuShowing(object sender, DevExpress.XtraTreeList.PopupMenuShowingEventArgs e)
	{
		e.Allow = false;
	}

	public void ExportToExcelForm()
	{
	}

	private void searchTextEdit_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == '\r')
		{
			CallSearch();
		}
	}

	private void CallSearch()
	{
		if (!ContinueAfterPossibleChanges(delegate
		{
			OpenPageControl();
		}))
		{
			return;
		}
		WorkWithDataedoTrackingHelper.TrackFirstInSessionSearch();
		ClearControlHighlights();
		searchesXtraTabControl.Enabled = false;
		isSearchFinished = false;
		CenterSearchProgressPanel();
		searchProgressPanelPanel.Visible = true;
		ResultItem treeResults = null;
		new TaskFactory().StartNew(delegate
		{
			if (searchCancellationTokenSource != null)
			{
				searchCancellationTokenSource.Cancel();
			}
			searchMutex.Wait();
			searchCancellationTokenSource = new CancellationTokenSource();
			List<int> documentations = (from x in DocumentationsCheckedComboBoxEdit.Properties.Items.GetCheckedValues()
				select Convert.ToInt32(x)).Distinct().ToList();
			List<string> types = (from x in TypesCheckedComboBoxEdit.Properties.Items.GetCheckedValues()
				select x.ToString()).Distinct().ToList();
			string searchText = searchTextEdit.Text;
			List<CustomFieldSearchItem> customFieldsSearchItems = advancedSearchPanel.GetCustomFieldsSearchItems();
			if (!searchCancellationTokenSource.Token.IsCancellationRequested)
			{
				treeResults = searchSupport.PrepareTree(documentations, searchSupport.Search(documentations, types, searchText, customFieldsSearchItems, CustomFieldsSupport, FindForm()));
			}
		}).ContinueWith(delegate(Task x)
		{
			if (x.Exception != null)
			{
				HandleSearchingError(x.Exception);
				throw x.Exception;
			}
		}, TaskContinuationOptions.NotOnCanceled | TaskContinuationOptions.ExecuteSynchronously).ContinueWith(delegate
		{
			Invoke((Action)delegate
			{
				if (!searchCancellationTokenSource.Token.IsCancellationRequested)
				{
					searchResultsListGridView.OptionsSelection.MultiSelect = true;
					searchResultsListGridControl.BeginUpdate();
					searchResultsListGridControl.DataSource = (from r in treeResults.GetObjectNodes(withoutModuleOnly: false)
						orderby r.Rank descending, r.ObjectType descending
						select r).ToList();
					searchResultsListGridView.ClearSelection();
					SetColumnsWidths(searchResultsListGridView);
					searchResultsListGridControl.EndUpdate();
					searchResultsTreeList.BeginUpdate();
					searchResultsTreeList.DataSource = treeResults;
					searchResultsTreeList.BestFitColumns(applyAutoWidth: true);
					searchResultsTreeList.EndUpdate();
					CleanupSearchInterfaceResources();
				}
				CleanupSearchTaskResources();
			});
		}, TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.ExecuteSynchronously)
			.ContinueWith(delegate(Task x)
			{
				HandleSearchingError(x.Exception);
			}, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
	}

	private void SetColumnsWidths(GridView gridView)
	{
		gridView.OptionsView.ColumnAutoWidth = true;
		for (int i = 0; i < gridView.VisibleColumns.Count - 1; i++)
		{
			GridColumn gridColumn = searchResultsListGridView.VisibleColumns[i];
			gridColumn.BestFit();
			gridColumn.OptionsColumn.FixedWidth = true;
		}
		GridColumn gridColumn2 = searchResultsListGridView.VisibleColumns.Last();
		if (gridColumn2 != null)
		{
			gridColumn2.BestFit();
			gridColumn2.OptionsColumn.FixedWidth = false;
		}
	}

	private void ClearSearchSelection()
	{
		if (!disableClearingSearchSelection)
		{
			searchResultsListGridView.OptionsSelection.MultiSelect = true;
			searchResultsListGridView.ClearSelection();
			searchResultsTreeList.FocusedNode = null;
		}
	}

	private void ClearListGridViewSearchSelection()
	{
		if (!disableClearingSearchSelection)
		{
			searchResultsListGridView.OptionsSelection.MultiSelect = true;
			searchResultsListGridView.ClearSelection();
		}
	}

	private void ClearTreeListSearchSelection()
	{
		if (!disableClearingSearchSelection)
		{
			searchResultsTreeList.FocusedNode = null;
		}
	}

	private void CleanupSearchInterfaceResources()
	{
		Invoke((Action)delegate
		{
			searchProgressPanelPanel.Visible = false;
			searchesXtraTabControl.Enabled = true;
			isSearchFinished = true;
		});
	}

	private void CleanupSearchTaskResources()
	{
		searchCancellationTokenSource.Dispose();
		searchCancellationTokenSource = null;
		searchMutex.Release();
	}

	private void HandleSearchingError(Exception exception)
	{
		GeneralExceptionHandling.Handle(exception, "Error occurred while searching.", FindForm());
		CleanupSearchTaskResources();
		CleanupSearchInterfaceResources();
	}

	private void leftPanelXtraTabControl_Resize(object sender, EventArgs e)
	{
		CenterSearchProgressPanel();
	}

	private void CenterSearchProgressPanel()
	{
		if (!isSearchFinished)
		{
			searchProgressPanelPanel.Left = searchesXtraTabControl.Left + 1;
			searchProgressPanelPanel.Top = advancedSearchSplitContainerControl.Top + advancedSearchSplitContainerControl.SplitterPosition + (searchesXtraTabControl.Height - searchResultsListGridControl.Height) + searchResultsListGridControl.Height / 2;
			searchProgressPanelPanel.Width = advancedSearchSplitContainerControl.Width;
			searchProgressPanel.Left = searchProgressPanelPanel.Width / 2 - searchProgressPanel.Width / 2;
			searchProgressPanel.Top = searchProgressPanelPanel.Height / 2 - searchProgressPanel.Height / 2;
		}
	}

	private void searchResultsListToolTipController_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		if (e.SelectedControl.Equals(searchResultsListGridControl))
		{
			GridHitInfo gridHitInfo = searchResultsListGridView.CalcHitInfo(e.ControlMousePosition);
			GridColumn column = gridHitInfo.Column;
			if (column != null && column.FieldName == "SearchResultsListIcon")
			{
				ResultItem resultItem = (ResultItem)searchResultsListGridView.GetRow(gridHitInfo.RowHandle);
				if (resultItem != null)
				{
					object identifier = new StringBuilder().Append(gridHitInfo.HitTest).Append(gridHitInfo.RowHandle).ToString();
					e.Info = GetToolTipControlInfo(identifier, resultItem);
				}
			}
		}
		else
		{
			if (!e.SelectedControl.Equals(searchResultsTreeList))
			{
				return;
			}
			TreeListHitInfo treeListHitInfo = searchResultsTreeList.CalcHitInfo(e.ControlMousePosition);
			if (treeListHitInfo.HitInfoType == HitInfoType.SelectImage)
			{
				ResultItem resultItem2 = (ResultItem)searchResultsTreeList.GetDataRecordByNode(treeListHitInfo.Node);
				if (resultItem2 != null && resultItem2.Type != SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database && resultItem2.Type != SharedObjectTypeEnum.ObjectType.Folder_Database)
				{
					object identifier2 = new StringBuilder().Append(treeListHitInfo.Node.Id).ToString();
					e.Info = GetToolTipControlInfo(identifier2, resultItem2);
				}
			}
		}
	}

	private ToolTipControlInfo GetToolTipControlInfo(object identifier, ResultItem resultItem)
	{
		if (resultItem.ElementType.HasValue)
		{
			return new ToolTipControlInfo(identifier, SharedObjectSubtypeEnum.TypeToStringForSingle(resultItem.ElementType, resultItem.ElementSubtype));
		}
		if (resultItem.ObjectType != SharedObjectTypeEnum.ObjectType.Term)
		{
			return new ToolTipControlInfo(identifier, SharedObjectSubtypeEnum.TypeToStringForSingle(resultItem.ObjectType, resultItem.ObjectSubtype));
		}
		return new ToolTipControlInfo(identifier, resultItem.CustomObjectType);
	}

	private void searchResultsListGridView_CustomColumnSort(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnSortEventArgs e)
	{
		if (e.Column.FieldName == "SearchResultsListIcon")
		{
			ResultItem obj = (ResultItem)searchResultsListGridView.GetRow(e.ListSourceRowIndex1);
			ResultItem resultItem = (ResultItem)searchResultsListGridView.GetRow(e.ListSourceRowIndex2);
			int sortValue = SharedObjectTypeEnum.GetSortValue(obj.ObjectType);
			int sortValue2 = SharedObjectTypeEnum.GetSortValue(resultItem.ObjectType);
			if (sortValue > sortValue2)
			{
				e.Result = -1;
			}
			else if (sortValue < sortValue2)
			{
				e.Result = 1;
			}
			else
			{
				e.Result = 0;
			}
			e.Handled = true;
		}
	}

	public void RefreshSearchDatabasesFromMetadataTreeList(int? id = null, string title = null)
	{
		if (!(metadataTreeList.DataSource is DBTree source) || source.Count() <= 0)
		{
			return;
		}
		List<object> checkedValues = DocumentationsCheckedComboBoxEdit.Properties.Items.GetCheckedValues();
		DocumentationsCheckedComboBoxEdit.CheckAll();
		List<object> checkedValues2 = DocumentationsCheckedComboBoxEdit.Properties.Items.GetCheckedValues();
		DocumentationsCheckedComboBoxEdit.Properties.BeginUpdate();
		DocumentationsCheckedComboBoxEdit.Properties.DataSource = (metadataTreeList.DataSource as DBTree).Where((DBTreeNode x) => x.Available).ToList()[0].Nodes;
		DocumentationsCheckedComboBoxEdit.Properties.DropDownRows = 13;
		DocumentationsCheckedComboBoxEdit.Properties.EndUpdate();
		DocumentationsCheckedComboBoxEdit.CheckAll();
		foreach (CheckedListBoxItem item in DocumentationsCheckedComboBoxEdit.Properties.Items)
		{
			if (checkedValues.Contains(item.Value) || !checkedValues2.Contains(item.Value))
			{
				item.CheckState = CheckState.Checked;
			}
			else
			{
				item.CheckState = CheckState.Unchecked;
			}
			if (id.HasValue && title != null && item.Value is int && (int)item.Value == id)
			{
				item.Description = title;
			}
		}
	}

	public void RefreshSearchCustomFields()
	{
		advancedSearchPanel.Initialize(CustomFieldsSupport);
		advancedSearchPanel.RefreshDataSource();
	}

	private void DocumentationsCheckedComboBoxEdit_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		foreach (CheckedListBoxItem item in DocumentationsCheckedComboBoxEdit.Properties.Items)
		{
			if (item.CheckState != CheckState.Checked)
			{
				return;
			}
		}
		e.DisplayText = "All documentations";
	}

	private void TypesCheckedComboBoxEdit_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		foreach (CheckedListBoxItem item in TypesCheckedComboBoxEdit.Properties.Items)
		{
			if (item.CheckState != CheckState.Checked)
			{
				return;
			}
		}
		e.DisplayText = "All types";
	}

	public void OpenCurrentlySelectedSearchRow()
	{
		if (lastOpenFromSearchResultsListGridView)
		{
			OpenDataTab((ResultItem)searchResultsListGridView.GetRow(lastOpenFromSearchResultsListGridViewRowHandle));
		}
		else if (lastOpenFromSearchResultsTreeList)
		{
			OpenDataTab((ResultItem)searchResultsTreeList.GetDataRecordByNode(lastOpenFromSearchResultsTreeListNode));
		}
	}

	private void searchResultsListGridView_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
	{
		if (e.Button != MouseButtons.Left)
		{
			return;
		}
		if (!isSearchResultJustSelected && (!searchResultsListGridView.IsRowSelected(e.RowHandle) || searchResultsListGridView.OptionsSelection.MultiSelect))
		{
			if (ContinueAfterPossibleChanges())
			{
				ClearTreeListSearchSelection();
				searchResultsListGridView.OptionsSelection.MultiSelect = false;
				lastOpenFromSearchResultsListGridView = true;
				lastOpenFromSearchResultsTreeList = false;
				lastOpenFromSearchResultsListGridViewRowHandle = e.RowHandle;
				OpenDataTab((ResultItem)searchResultsListGridView.GetRow(e.RowHandle));
			}
		}
		else
		{
			isSearchResultJustSelected = false;
		}
	}

	private void SearchResultsListGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		if (ContinueAfterPossibleChanges())
		{
			if (isSearchFinished)
			{
				ClearTreeListSearchSelection();
				searchResultsListGridView.OptionsSelection.MultiSelect = false;
				lastOpenFromSearchResultsListGridView = true;
				lastOpenFromSearchResultsTreeList = false;
				lastOpenFromSearchResultsListGridViewRowHandle = e.FocusedRowHandle;
				OpenDataTab((ResultItem)searchResultsListGridView.GetRow(e.FocusedRowHandle));
				isSearchResultJustSelected = true;
			}
		}
		else
		{
			isSearchResultJustSelected = true;
		}
	}

	private void searchResultsListGridView_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		e.Allow = ContinueAfterPossibleChanges();
	}

	private void SearchResultsTreeList_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		ClearListGridViewSearchSelection();
		lastOpenFromSearchResultsListGridView = false;
		lastOpenFromSearchResultsTreeList = true;
		lastOpenFromSearchResultsTreeListNode = e.Node;
		OpenDataTab((ResultItem)searchResultsTreeList.GetDataRecordByNode(e.Node));
	}

	private void searchResultsTreeList_BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e)
	{
		try
		{
			if (e.Node != null)
			{
				e.CanFocus = ContinueAfterPossibleChanges();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error when loading data for selected object.", FindForm());
		}
	}

	private bool SelectElementInRepositoryTree(ResultItem row)
	{
		TreeListNode treeListNode = null;
		disableClearingSearchSelection = true;
		try
		{
			treeListNode = (from n in metadataTreeList.Nodes[0].Nodes
				where ((DBTreeNode)metadataTreeList.GetDataRecordByNode(n)).DatabaseId == row.DatabaseId
				select (n)).FirstOrDefault();
			if (treeListNode != null)
			{
				if ((row.ModuleId.HasValue && row.ModuleId != -1) || row.ObjectName == "Subject Areas")
				{
					treeListNode.Expanded = true;
					treeListNode = (from n in treeListNode.Nodes
						where ((DBTreeNode)metadataTreeList.GetDataRecordByNode(n)).ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database
						select (n)).FirstOrDefault();
				}
				if (treeListNode != null && row.ModuleId.HasValue && row.ModuleId != -1)
				{
					treeListNode.Expanded = true;
					treeListNode = (from n in treeListNode.Nodes
						where ((DBTreeNode)metadataTreeList.GetDataRecordByNode(n)).Id == row.ModuleId
						select (n)).FirstOrDefault();
				}
				if (treeListNode != null)
				{
					if (row.ObjectId.HasValue)
					{
						treeListNode.Expanded = true;
						string typeName = SharedObjectTypeEnum.TypeToStringForMenu(row.ObjectType);
						treeListNode = (from n in treeListNode.Nodes
							where ((DBTreeNode)metadataTreeList.GetDataRecordByNode(n)).Name == typeName
							select (n)).FirstOrDefault();
					}
					else if (row.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database && row.ObjectName != "Subject Areas")
					{
						treeListNode.Expanded = true;
						treeListNode = (from n in treeListNode.Nodes
							where ((DBTreeNode)metadataTreeList.GetDataRecordByNode(n)).Name == row.Name
							select (n)).FirstOrDefault();
					}
				}
				if (treeListNode != null && row.ObjectId.HasValue)
				{
					try
					{
						TreeListHelpers.DisableFocusEvents = true;
						metadataTreeList.CollapseAll();
					}
					finally
					{
						TreeListHelpers.DisableFocusEvents = false;
					}
					treeListNode.Expanded = true;
					treeListNode = SearchTreeNodeOperation.FindNode(treeListNode.Nodes, row.ObjectId.Value, row.ObjectType);
				}
				if (treeListNode != null)
				{
					TreeListHelpers.DisableFocusNodeChangedEventActions = true;
					metadataTreeList.SetFocusedNode(treeListNode);
					ChangeRibbonIcons(treeListNode);
				}
			}
		}
		catch
		{
		}
		disableClearingSearchSelection = false;
		return treeListNode != null;
	}

	private void OpenDataTab(ResultItem row)
	{
		if (row == null)
		{
			return;
		}
		DBTreeNode dBTreeNode = ((metadataTreeList.DataSource as DBTree)?.ToList()[0].Nodes).Where((DBTreeNode r) => r.DatabaseId == row.DatabaseId).FirstOrDefault();
		if (SelectElementInRepositoryTree(row))
		{
			if (row.ModuleId.HasValue && row.ModuleId != -1)
			{
				dBTreeNode = dBTreeNode.Nodes.Where((DBTreeNode r) => r.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database).FirstOrDefault().Nodes.Where((DBTreeNode r) => r.Id == row.ModuleId).FirstOrDefault();
			}
			string typeName = SharedObjectTypeEnum.TypeToStringForMenu(row.ObjectType);
			if (row.ObjectId.HasValue)
			{
				dBTreeNode = dBTreeNode.Nodes.Where((DBTreeNode r) => r.Name == typeName).FirstOrDefault();
				dBTreeNode = dBTreeNode.Nodes.Where((DBTreeNode r) => r.Id == row.ObjectId.Value).FirstOrDefault();
			}
			else if (row.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Database)
			{
				OpenPageControl();
				return;
			}
			OpenPageControl(showControl: true, dBTreeNode);
			ClearControlHighlights();
			FillHighlights(row);
		}
		else
		{
			ClearDataPanel();
			ClearControlHighlights();
			metadataTreeList.FocusedNode = null;
		}
	}

	public void FillHighlights(ResultItem row, bool allowChangingTab = true, SharedObjectTypeEnum.ObjectType? elementType = null)
	{
		ITabSettable tabSettable = GetVisibleUserControl() as ITabSettable;
		allowChangingTab = false;
		if (tabSettable == null)
		{
			return;
		}
		tabSettable.SetLastResult(row);
		if (row.ElementType.HasValue)
		{
			tabSettable.SetTab(row, row.ElementType, true, searchSupport.SearchWords, searchSupport.CustomFieldSearchItems, row.ElementId);
		}
		else
		{
			if (!IsDocumentationOrModuleAllowed(row))
			{
				return;
			}
			if ((row.Type == SharedObjectTypeEnum.ObjectType.Database && searchSupport.SearchInDocumentationObject) || (row.Type == SharedObjectTypeEnum.ObjectType.Module && searchSupport.SearchInModuleObject))
			{
				tabSettable.SetTab(row, (SharedObjectTypeEnum.ObjectType?)null, allowChangingTab, searchSupport.SearchWords, searchSupport.CustomFieldSearchItems, (int?[])null);
			}
			else if (row.Nodes.Count > 0)
			{
				var list = (from n in row.Nodes
					group n by n.ElementType into g
					select new
					{
						Key = g.Key,
						Rank = g.Sum((ResultItem n) => n.Rank),
						Nodes = g.Select((ResultItem n) => n.ElementId).ToArray()
					} into g
					orderby g.Rank
					select g).ToList();
				var anon = list[list.Count - 1];
				if (anon.Rank >= row.Rank)
				{
					tabSettable.SetTab(row, null, false, searchSupport.SearchWords, searchSupport.CustomFieldSearchItems);
					if (!elementType.HasValue || elementType == anon.Key)
					{
						tabSettable.SetTab(row, anon.Key, allowChangingTab, searchSupport.SearchWords, searchSupport.CustomFieldSearchItems, anon.Nodes);
					}
				}
				else
				{
					if (!elementType.HasValue || elementType == anon.Key)
					{
						tabSettable.SetTab(row, anon.Key, changeTab: false, searchSupport.SearchWords, searchSupport.CustomFieldSearchItems, anon.Nodes);
					}
					tabSettable.SetTab(row, null, allowChangingTab, searchSupport.SearchWords, searchSupport.CustomFieldSearchItems);
				}
				for (int i = 0; i < list.Count - 1; i++)
				{
					if (!elementType.HasValue || elementType == list[i].Key)
					{
						tabSettable.SetTab(row, list[i].Key, changeTab: false, searchSupport.SearchWords, searchSupport.CustomFieldSearchItems, list[i].Nodes);
					}
				}
			}
			else
			{
				tabSettable.SetTab(row, null, allowChangingTab, searchSupport.SearchWords, searchSupport.CustomFieldSearchItems);
			}
		}
	}

	private bool IsDocumentationOrModuleAllowed(ResultItem row)
	{
		if ((row.Type == SharedObjectTypeEnum.ObjectType.Database || row.Type == SharedObjectTypeEnum.ObjectType.Folder_Database || row.Type == SharedObjectTypeEnum.ObjectType.Module) && ((row.Type != SharedObjectTypeEnum.ObjectType.Database && row.Type != SharedObjectTypeEnum.ObjectType.Folder_Database) || !searchSupport.SearchInDocumentationObject))
		{
			if (row.Type == SharedObjectTypeEnum.ObjectType.Module)
			{
				return searchSupport.SearchInModuleObject;
			}
			return false;
		}
		return true;
	}

	private void ClearControlHighlights(bool clearControlHighlights = true, bool forceTabsAll = false)
	{
		if (metadataEditorSplitContainerControl.Panel2.Controls.Count > 0 && GetVisibleUserControl() is ITabSettable tabSettable)
		{
			if (clearControlHighlights)
			{
				tabSettable.ClearHighlights(keepSearchActive: false);
			}
			GetVisibleUserControl()?.SetTabsProgressHighlights();
			tabSettable.ForceLayoutChange(forceTabsAll);
		}
	}

	private void SearchResultsListGridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		e.Value = IconsSupport.GetNodeImage(treeMenuImageCollection, e.Row as ResultItem);
	}

	private void SearchResultsTreeList_GetSelectImage(object sender, GetSelectImageEventArgs e)
	{
		if (searchResultsTreeList == null)
		{
			return;
		}
		ResultItem resultItem = (ResultItem)searchResultsTreeList.GetDataRecordByNode(e.Node);
		if (resultItem != null)
		{
			if (resultItem.ObjectType != SharedObjectTypeEnum.ObjectType.Database)
			{
				IconsSupport.SetNodeImageIndex(treeMenuImageCollection, e, resultItem);
			}
			else
			{
				IconsSupport.SetTreeImage(resultItem.ObjectType, resultItem.DocumentationType, e, treeMenuImageCollection);
			}
		}
	}

	private void leftPanelXtraTabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			this.SearchTabVisibilityEvent?.Invoke(sender, new RibbonEventArgs(e.Page == searchXtraTabPage, GetFocusedNode()));
			if (e.Page == searchXtraTabPage)
			{
				if (!isSearchFinished)
				{
					searchProgressPanelPanel.Visible = true;
				}
				RefreshSearchCustomFields();
				searchTextEdit.Focus();
				ResultItem resultItem = null;
				if (searchesXtraTabControl.SelectedTabPage == searchResultsGridViewXtraTabPage)
				{
					resultItem = SelectRowFromGridView();
				}
				else if (searchesXtraTabControl.SelectedTabPage == searchResultsTreeListXtraTabPage)
				{
					resultItem = SelectRowFromTreeList();
				}
				if (resultItem == null)
				{
					resultItem = SelectRowFromGridView();
				}
				if (resultItem == null)
				{
					resultItem = SelectRowFromTreeList();
				}
				if (resultItem != null)
				{
					FillHighlights(resultItem, allowChangingTab: false);
					(GetVisibleUserControl() as ITabSettable).ForceLayoutChange();
				}
			}
			else if (e.Page == repositoryXtraTabPage)
			{
				if (!isSearchFinished)
				{
					searchProgressPanelPanel.Visible = false;
				}
				ClearAllSearchHighlights();
				ClearControlHighlights(clearControlHighlights: false, forceTabsAll: true);
			}
		}
		catch
		{
		}
	}

	private ResultItem SelectRowFromGridView()
	{
		if (searchResultsListGridView.SelectedRowsCount > 0)
		{
			int rowHandle = searchResultsListGridView.GetSelectedRows().FirstOrDefault();
			return searchResultsListGridView.GetRow(rowHandle) as ResultItem;
		}
		return null;
	}

	private ResultItem SelectRowFromTreeList()
	{
		return searchResultsTreeList.GetDataRecordByNode(searchResultsTreeList.FocusedNode) as ResultItem;
	}

	private void leftPanelXtraTabControl_CloseButtonClick(object sender, EventArgs e)
	{
		if ((e as ClosePageButtonEventArgs).Page == searchXtraTabPage)
		{
			HideSearchPanel();
		}
	}

	private void leftPanelXtraTabControl_KeyDown(object sender, KeyEventArgs e)
	{
		if (leftPanelXtraTabControl.SelectedTabPage == searchXtraTabPage)
		{
			HideSearchPanelOnEscapeKey(e.KeyCode);
		}
	}

	private void searchesXtraTabControl_KeyDown(object sender, KeyEventArgs e)
	{
		HideSearchPanelOnEscapeKey(e.KeyCode);
	}

	private void searchResultsListGridView_KeyDown(object sender, KeyEventArgs e)
	{
		HideSearchPanelOnEscapeKey(e.KeyCode);
	}

	private void searchResultsTreeList_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		HideSearchPanelOnEscapeKey(e.KeyCode);
	}

	private void searchControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		HideSearchPanelOnEscapeKey(e.KeyCode);
	}

	private void TypesCheckedComboBoxEdit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		HideSearchPanelOnEscapeKey(e.KeyCode);
	}

	private void DocumentationsCheckedComboBoxEdit_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		HideSearchPanelOnEscapeKey(e.KeyCode);
	}

	public void ShowSearchPanel()
	{
		metadataTreeList.CloseEditor();
		searchXtraTabPage.PageVisible = true;
		searchXtraTabPage.Show();
		leftPanelXtraTabControl.ShowTabHeader = DefaultBoolean.True;
		searchTextEdit.Focus();
	}

	private void HideSearchPanel()
	{
		repositoryXtraTabPage.Show();
		leftPanelXtraTabControl.ShowTabHeader = DefaultBoolean.False;
		OnSearchPanelClose();
	}

	private void HideSearchPanelOnEscapeKey(Keys key)
	{
		if (searchXtraTabPage.PageVisible && key == Keys.Escape)
		{
			HideSearchPanel();
		}
	}

	private void OnSearchPanelClose()
	{
		ResetSearchSupport();
	}

	private void ResetSearchSupport()
	{
		ClearControlHighlights();
		DocumentationsCheckedComboBoxEdit.CheckAll();
		TypesCheckedComboBoxEdit.CheckAll();
		searchTextEdit.Text = null;
		advancedSearchPanel.ClearSearchFields();
		isSearchResultJustSelected = false;
		TreeListHelpers.DisableFocusNodeChangedEventActions = false;
		disableClearingSearchSelection = false;
		ClearSearchSelection();
		searchResultsListGridControl.DataSource = null;
		searchResultsTreeList.DataSource = null;
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			switch (keyData)
			{
			case Keys.F | Keys.Control:
				ShowSearchPanel();
				return true;
			case Keys.S | Keys.Control:
			{
				BasePanelControl visibleUserControl = GetVisibleUserControl();
				if (visibleUserControl != null)
				{
					visibleUserControl.CloseEditors();
					visibleUserControl.Save();
				}
				return true;
			}
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	public void ShowProgressColumn(bool showWaitForm, bool show)
	{
		metadataTreeList.CloseEditor();
		if (!show)
		{
			ShowProgress = false;
		}
		else
		{
			RefreshProgress(showWaitForm);
		}
	}

	internal void RefreshProgressIfShown(bool showWaitForm = false, DBTreeNode mainNode = null)
	{
		if (ShowProgress)
		{
			RefreshProgress(showWaitForm, mainNode);
		}
	}

	internal void RefreshObjectProgress(bool showWaitForm, int objectId, SharedObjectTypeEnum.ObjectType objectType)
	{
		if (!ShowProgress)
		{
			return;
		}
		if (showWaitForm)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
		}
		IEnumerable<DBTreeNode> enumerable = DBTreeMenu.FindNodesById(objectType, objectId);
		List<ProgressObject> source;
		try
		{
			source = ((ProgressType.Type == ProgressTypeEnum.SelectedCustomField) ? DB.DocumentationProgress.GetObjectCustomFieldProgress(objectId, enumerable.FirstOrDefault().ObjectType, ProgressType.FieldName) : ((ProgressType.Type != ProgressTypeEnum.TablesAndColumns || (enumerable.FirstOrDefault().ObjectType != SharedObjectTypeEnum.ObjectType.Table && enumerable.FirstOrDefault().ObjectType != SharedObjectTypeEnum.ObjectType.View)) ? DB.DocumentationProgress.GetObjectProgress(objectId, enumerable.FirstOrDefault().ObjectType) : DB.DocumentationProgress.GetSingleTableViewAndColumnsProgress(objectId, enumerable.FirstOrDefault().ObjectType)));
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
			return;
		}
		metadataTreeList.BeginUpdate();
		ProgressObject progressObject = source.FirstOrDefault();
		if (progressObject == null || enumerable.FirstOrDefault().SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted)
		{
			foreach (DBTreeNode item in enumerable)
			{
				item.SetEmptyNodeProgress();
			}
		}
		else
		{
			foreach (DBTreeNode item2 in enumerable)
			{
				item2.Points = progressObject.Points.GetValueOrDefault();
				item2.TotalPoints = progressObject.Total.GetValueOrDefault();
				item2.ObjectPoints = progressObject.ObjectPoints.GetValueOrDefault();
				item2.TotalObjectPoints = progressObject.TotalObjectPoints.GetValueOrDefault();
				item2.ColumnsPoints = progressObject.ColumnsPoints.GetValueOrDefault();
				item2.TotalColumnsPoints = progressObject.TotalColumnsPoints.GetValueOrDefault();
				item2.FKRelationsPoints = progressObject.TotalTrFkRelationsPoints.GetValueOrDefault();
				item2.TotalFKRelationsPoints = progressObject.TotalTrFkRelationsPoints.GetValueOrDefault();
				item2.PKRelationsPoints = progressObject.TrPkRelationsPoints.GetValueOrDefault();
				item2.TotalPKRelationsPoints = progressObject.TotalTrPkRelationsPoints.GetValueOrDefault();
				item2.KeysPoints = progressObject.KeysPoints.GetValueOrDefault();
				item2.TotalKeysPoints = progressObject.TotalKeysPoints.GetValueOrDefault();
				item2.TriggersPoints = progressObject.TriggersPoints.GetValueOrDefault();
				item2.TotalTriggersPoints = progressObject.TotalTriggersPoints.GetValueOrDefault();
				item2.ParametersPoints = progressObject.ParametersPoints.GetValueOrDefault();
				item2.TotalParametersPoints = progressObject.TotalParametersPoints.GetValueOrDefault();
				item2.PointsSum = item2.Points;
				item2.TotalPointsSum = item2.TotalPoints;
				item2.ProgressValue = DBTreeNode.CalculateProgress(item2.Points, item2.TotalPoints);
				DBTreeNode.AggregateProgressUp(item2, ProgressType);
			}
		}
		metadataTreeList.EndUpdate();
		ShowProgress = true;
		if (showWaitForm)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	internal void RefreshProgress(bool showWaitForm, DBTreeNode mainNode = null)
	{
		if (showWaitForm)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
		}
		List<ProgressWithIdAndTypeObject> objectDocumentationProgress;
		try
		{
			objectDocumentationProgress = DB.DocumentationProgress.GetObjectDocumentationProgress(ProgressType);
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
			return;
		}
		IEnumerable<DBTreeNode> enumerable = DBTreeMenu.FindLowestLevelDBNodes(mainNode);
		metadataTreeList.BeginUpdate();
		foreach (DBTreeNode node in enumerable)
		{
			ProgressWithIdAndTypeObject progressWithIdAndTypeObject = objectDocumentationProgress.Where((ProgressWithIdAndTypeObject x) => x.Id == node.Id && (SharedObjectTypeEnum.ObjectType)Enum.Parse(typeof(SharedObjectTypeEnum.ObjectType), x.ObjectType) == node.ObjectType).SingleOrDefault();
			if (progressWithIdAndTypeObject == null || node.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted)
			{
				node.SetEmptyNodeProgress();
				continue;
			}
			node.Points = progressWithIdAndTypeObject.Points.GetValueOrDefault();
			node.TotalPoints = progressWithIdAndTypeObject.Total.GetValueOrDefault();
			node.ObjectPoints = progressWithIdAndTypeObject.ObjectPoints.GetValueOrDefault();
			node.TotalObjectPoints = progressWithIdAndTypeObject.TotalObjectPoints.GetValueOrDefault();
			node.ColumnsPoints = progressWithIdAndTypeObject.ColumnsPoints.GetValueOrDefault();
			node.TotalColumnsPoints = progressWithIdAndTypeObject.TotalColumnsPoints.GetValueOrDefault();
			node.FKRelationsPoints = progressWithIdAndTypeObject.TotalTrFkRelationsPoints.GetValueOrDefault();
			node.TotalFKRelationsPoints = progressWithIdAndTypeObject.TotalTrFkRelationsPoints.GetValueOrDefault();
			node.PKRelationsPoints = progressWithIdAndTypeObject.TrPkRelationsPoints.GetValueOrDefault();
			node.TotalPKRelationsPoints = progressWithIdAndTypeObject.TotalTrPkRelationsPoints.GetValueOrDefault();
			node.KeysPoints = progressWithIdAndTypeObject.KeysPoints.GetValueOrDefault();
			node.TotalKeysPoints = progressWithIdAndTypeObject.TotalKeysPoints.GetValueOrDefault();
			node.TriggersPoints = progressWithIdAndTypeObject.TriggersPoints.GetValueOrDefault();
			node.TotalTriggersPoints = progressWithIdAndTypeObject.TotalTriggersPoints.GetValueOrDefault();
			node.ParametersPoints = progressWithIdAndTypeObject.ParametersPoints.GetValueOrDefault();
			node.TotalParametersPoints = progressWithIdAndTypeObject.TotalParametersPoints.GetValueOrDefault();
			node.PointsSum = node.Points;
			node.TotalPointsSum = node.TotalPoints;
		}
		if (mainNode == null)
		{
			SearchTreeNodeOperation.FindNodesByType(metadataTreeList.Nodes.FirstOrDefault()?.Nodes, SharedObjectTypeEnum.ObjectType.Database, SharedObjectTypeEnum.ObjectType.BusinessGlossary).ForEach(delegate(DBTreeNode x)
			{
				DBTreeNode.AggregateProgressDown(x, progressType);
			});
		}
		else
		{
			DBTreeNode.AggregateProgressDown(mainNode, progressType);
		}
		metadataTreeList.EndUpdate();
		ShowProgress = true;
		if (showWaitForm)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private void metadataTreeList_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
	{
		if (!showProgress || e.Column != progressTreeListColumn)
		{
			return;
		}
		DBTreeNode dBTreeNode = (DBTreeNode)metadataTreeList.GetDataRecordByNode(e.Node);
		if (dBTreeNode != null)
		{
			if (e.CellValue != null && Convert.ToDouble(e.CellValue) > 0.0 && Convert.ToDouble(e.CellValue) < 100.0)
			{
				e.Appearance.BackColor = SkinsManager.CurrentSkin.ProgressLoadingColumnBackColor;
			}
			if (e.CellValue == null && (dBTreeNode == null || dBTreeNode.SynchronizeState == SynchronizeStateEnum.SynchronizeState.Deleted || dBTreeNode.IsWelcomeDocumentation))
			{
				e.Handled = true;
			}
			if ((ProgressType.Type == ProgressTypeEnum.TablesAndColumns && (dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure || dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Function || dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary || dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Term)) || (dBTreeNode.IsFolder && dBTreeNode.ParentNode.ObjectType == SharedObjectTypeEnum.ObjectType.BusinessGlossary))
			{
				e.Handled = true;
			}
			else if (ProgressType.Type == ProgressTypeEnum.SelectedCustomField && (!dBTreeNode.ProgressValue.HasValue || (dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table && !ProgressType.CustomField.ColumnVisibility && !ProgressType.CustomField.TableVisibility && !ProgressType.CustomField.TriggerVisibility && !ProgressType.CustomField.KeyVisibility && !ProgressType.CustomField.RelationVisibility)))
			{
				e.Handled = true;
			}
			else if (ProgressType.Type == ProgressTypeEnum.SelectedCustomField && (!dBTreeNode.ProgressValue.HasValue || (dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.View && !ProgressType.CustomField.ColumnVisibility && !ProgressType.CustomField.TableVisibility)))
			{
				e.Handled = true;
			}
			else if (ProgressType.Type == ProgressTypeEnum.SelectedCustomField && (!dBTreeNode.ProgressValue.HasValue || dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Procedure || dBTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Function) && !ProgressType.CustomField.ProcedureVisibility && !ProgressType.CustomField.ParameterVisibility)
			{
				e.Handled = true;
			}
		}
	}

	private void metadataTreeList_CustomNodeCellEdit(object sender, GetCustomNodeCellEditEventArgs e)
	{
		if (e.Column != progressTreeListColumn)
		{
			return;
		}
		DBTreeNode dBTreeNode = (DBTreeNode)metadataTreeList.GetDataRecordByNode(e.Node);
		if (dBTreeNode.ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Module)
		{
			_ = dBTreeNode.ObjectType;
			_ = 9;
		}
		bool flag = dBTreeNode.ProgressValue > 0 && dBTreeNode.ProgressValue < 100;
		switch (dBTreeNode.ObjectType)
		{
		case SharedObjectTypeEnum.ObjectType.BusinessGlossary:
			if (flag)
			{
				Color color2 = (loadingRepositoryItemProgressBar.StartColor = (loadingRepositoryItemProgressBar.EndColor = SkinsManager.CurrentSkin.ProgressDatabaseLoadingRepositoryItemProgressBarBackColor));
				e.RepositoryItem = loadingRepositoryItemProgressBar;
				e.RepositoryItem.Appearance.BackColor = SkinsManager.CurrentSkin.ProgressLoadingColumnBackColor;
			}
			else
			{
				Color color2 = (progressDatabaseRepositoryItemProgressBar.StartColor = (progressDatabaseRepositoryItemProgressBar.EndColor = SkinsManager.CurrentSkin.ProgressDatabaseRepositoryItemProgressBarBackColor));
				e.RepositoryItem = progressDatabaseRepositoryItemProgressBar;
			}
			break;
		case SharedObjectTypeEnum.ObjectType.Database:
			if (flag)
			{
				Color color2 = (loadingRepositoryItemProgressBar.StartColor = (loadingRepositoryItemProgressBar.EndColor = SkinsManager.CurrentSkin.ProgressDatabaseLoadingRepositoryItemProgressBarBackColor));
				e.RepositoryItem = loadingRepositoryItemProgressBar;
				e.RepositoryItem.Appearance.BackColor = SkinsManager.CurrentSkin.ProgressLoadingColumnBackColor;
			}
			else
			{
				Color color2 = (progressDatabaseRepositoryItemProgressBar.StartColor = (progressDatabaseRepositoryItemProgressBar.EndColor = SkinsManager.CurrentSkin.ProgressDatabaseRepositoryItemProgressBarBackColor));
				e.RepositoryItem = progressDatabaseRepositoryItemProgressBar;
			}
			break;
		case SharedObjectTypeEnum.ObjectType.Module:
			if (flag)
			{
				Color color2 = (loadingRepositoryItemProgressBar.StartColor = (loadingRepositoryItemProgressBar.EndColor = SkinsManager.CurrentSkin.ProgressModuleLoadingRepositoryItemProgressBarBackColor));
				e.RepositoryItem = loadingRepositoryItemProgressBar;
				e.RepositoryItem.Appearance.BackColor = SkinsManager.CurrentSkin.ProgressLoadingColumnBackColor;
			}
			else
			{
				Color color2 = (progressModuleRepositoryItemProgressBar.StartColor = (progressModuleRepositoryItemProgressBar.EndColor = SkinsManager.CurrentSkin.ProgressModuleRepositoryItemProgressBarBackColor));
				e.RepositoryItem = progressModuleRepositoryItemProgressBar;
			}
			break;
		case SharedObjectTypeEnum.ObjectType.Folder_Database:
		case SharedObjectTypeEnum.ObjectType.Folder_Module:
			if (flag)
			{
				Color color2 = (loadingRepositoryItemProgressBar.StartColor = (loadingRepositoryItemProgressBar.EndColor = SkinsManager.CurrentSkin.ProgressFolderLoadingRepositoryItemProgressBarBackColor));
				e.RepositoryItem = loadingRepositoryItemProgressBar;
				e.RepositoryItem.Appearance.BackColor = SkinsManager.CurrentSkin.ProgressLoadingColumnBackColor;
			}
			else
			{
				Color color2 = (progressFolderRepositoryItemProgressBar.StartColor = (progressFolderRepositoryItemProgressBar.EndColor = SkinsManager.CurrentSkin.ProgressFolderRepositoryItemProgressBarBackColor));
				e.RepositoryItem = progressFolderRepositoryItemProgressBar;
			}
			break;
		default:
			if (flag)
			{
				Color color2 = (loadingRepositoryItemProgressBar.StartColor = (loadingRepositoryItemProgressBar.EndColor = SkinsManager.CurrentSkin.ProgressObjectLoadingRepositoryItemProgressBarBackColor));
				e.RepositoryItem = loadingRepositoryItemProgressBar;
				e.RepositoryItem.Appearance.BackColor = SkinsManager.CurrentSkin.ProgressLoadingColumnBackColor;
			}
			else
			{
				Color color2 = (progressSimpleObjectRepositoryItemProgressBar.StartColor = (progressSimpleObjectRepositoryItemProgressBar.EndColor = SkinsManager.CurrentSkin.ProgressSimpleObjectRepositoryItemProgressBarBackColor));
				e.RepositoryItem = progressSimpleObjectRepositoryItemProgressBar;
			}
			break;
		}
	}

	private void metadataTreeList_ShownEditor(object sender, EventArgs e)
	{
		TextEdit editor = metadataTreeList.ActiveEditor as TextEdit;
		if (editor == null)
		{
			return;
		}
		editor.KeyUp += delegate(object s, KeyEventArgs innerE)
		{
			if (innerE.KeyCode == Keys.Left && innerE.Modifiers == Keys.None)
			{
				editor.SelectionLength = 0;
				innerE.Handled = true;
			}
			else if (innerE.KeyCode == Keys.Right && innerE.Modifiers == Keys.None && !string.IsNullOrEmpty(editor.SelectedText))
			{
				editor.SelectionStart += editor.SelectedText.Length;
				editor.SelectionLength = 0;
				innerE.Handled = true;
			}
		};
		editor.KeyDown += delegate(object s, KeyEventArgs innerE)
		{
			if (innerE.KeyCode == Keys.Left && innerE.Modifiers == Keys.None && !string.IsNullOrEmpty(editor.SelectedText))
			{
				editor.SelectionLength = 0;
				innerE.Handled = true;
			}
			else if (innerE.KeyCode == Keys.Right && innerE.Modifiers == Keys.None && !string.IsNullOrEmpty(editor.SelectedText))
			{
				editor.SelectionStart += editor.SelectedText.Length;
				editor.SelectionLength = 0;
				innerE.Handled = true;
			}
		};
	}

	private void metadataTreeList_HiddenEditor(object sender, EventArgs e)
	{
		InsertModuleIfNotAlreadyInserted();
		if (lastFocusedNodeForModules != null)
		{
			DB.Module.InsertModuleObject(moduleUserControl.ModuleId, lastFocusedNodeForModules.Id, lastFocusedNodeForModules.ObjectType, FindForm());
			DBTreeMenu.AddObjectToModule(lastFocusedNodeForModules.DatabaseId, lastFocusedNodeForModules.Id, moduleUserControl.ModuleId, lastFocusedNodeForModules.Schema, lastFocusedNodeForModules.BaseName, lastFocusedNodeForModules.Title, lastFocusedNodeForModules.Source ?? UserTypeEnum.UserType.DBMS, lastFocusedNodeForModules.ObjectType, lastFocusedNodeForModules.Subtype);
			lastFocusedNodeForModules = null;
		}
		SwitchOffEditablitity();
		DBTreeNode focusedNode = GetFocusedNode();
		if (metadataTreeList.FocusedNode != null && focusedNode != null && focusedNode.ObjectType == SharedObjectTypeEnum.ObjectType.Table && focusedNode.Source == UserTypeEnum.UserType.USER)
		{
			focusedNode.Name = DBTreeMenu.SetName(focusedNode.Schema, focusedNode.BaseName, focusedNode.Title, focusedNode.ObjectType, focusedNode.DatabaseType, focusedNode.ShowSchema);
		}
		metadataTreeList.EndCurrentEdit();
		if (TreeListHelpers.FocusNodeToRestoreAfterRename != null)
		{
			metadataTreeList.FocusedNode = TreeListHelpers.FocusNodeToRestoreAfterRename;
			TreeListHelpers.DisableFocusEvents = false;
			TreeListHelpers.FocusNodeToRestoreAfterRename = null;
		}
	}

	private void metadataTreeList_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		RenameNode(FindForm());
	}

	private void searchResultsListGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		e.Allow = false;
	}

	private void metadataTreeList_BeforeExpand(object sender, BeforeExpandEventArgs e)
	{
		Point pt = metadataTreeList.PointToClient(Control.MousePosition);
		if (metadataTreeList.CalcHitInfo(pt).Column == progressTreeListColumn)
		{
			e.CanExpand = false;
		}
	}

	private void metadataTreeList_AfterExpand(object sender, NodeEventArgs e)
	{
		ShowOnboardingsAfterExpanding(TreeListHelpers.GetDBTreeNode(e.Node), e.Node);
	}

	private void ShowOnboardingsAfterExpanding(DBTreeNode node, TreeListNode treeListNode)
	{
		if (node.ObjectType == SharedObjectTypeEnum.ObjectType.Database)
		{
			ShowDocumentationExpandOnboarding(treeListNode);
			ShowColumnsOpenMessageClosedOnboarding(treeListNode);
		}
	}

	private void ShowDocumentationExpandOnboarding(TreeListNode treeListNode)
	{
		if (!OnboardingSupport.CheckMessageCondition(OnboardingSupport.OnboardingMessages.DocumentationExpandTables) && !OnboardingSupport.CheckMessageCondition(OnboardingSupport.OnboardingMessages.DocumentationExpandViews) && !OnboardingSupport.CheckMessageCondition(OnboardingSupport.OnboardingMessages.DocumentationExpandProcedures) && !OnboardingSupport.CheckMessageCondition(OnboardingSupport.OnboardingMessages.DocumentationExpandFunctions) && !OnboardingSupport.CheckMessageCondition(OnboardingSupport.OnboardingMessages.DocumentationExpandStructures))
		{
			return;
		}
		TreeListNode folderNode = treeListNode.Nodes.FirstOrDefault(delegate(TreeListNode x)
		{
			DBTreeNode dBTreeNode2 = TreeListHelpers.GetDBTreeNode(x);
			if (dBTreeNode2.ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Database)
			{
				return false;
			}
			if (!dBTreeNode2.Nodes.Any())
			{
				return false;
			}
			SharedObjectTypeEnum.ObjectType? objectType2 = SharedObjectTypeEnum.StringToTypeForMenu(dBTreeNode2.Name);
			return (objectType2 == SharedObjectTypeEnum.ObjectType.Table || objectType2 == SharedObjectTypeEnum.ObjectType.View || objectType2 == SharedObjectTypeEnum.ObjectType.Function || objectType2 == SharedObjectTypeEnum.ObjectType.Procedure || objectType2 == SharedObjectTypeEnum.ObjectType.Structure) ? true : false;
		});
		DBTreeNode dBTreeNode = TreeListHelpers.GetDBTreeNode(folderNode);
		if (dBTreeNode == null)
		{
			return;
		}
		OnboardingSupport.OnboardingMessages? onboardingMessages = null;
		SharedObjectTypeEnum.ObjectType? objectType = SharedObjectTypeEnum.StringToTypeForMenu(dBTreeNode.Name);
		if (objectType == SharedObjectTypeEnum.ObjectType.Table)
		{
			onboardingMessages = OnboardingSupport.OnboardingMessages.DocumentationExpandTables;
		}
		else if (objectType == SharedObjectTypeEnum.ObjectType.View)
		{
			onboardingMessages = OnboardingSupport.OnboardingMessages.DocumentationExpandViews;
		}
		else if (objectType == SharedObjectTypeEnum.ObjectType.Procedure)
		{
			onboardingMessages = OnboardingSupport.OnboardingMessages.DocumentationExpandProcedures;
		}
		else if (objectType == SharedObjectTypeEnum.ObjectType.Function)
		{
			onboardingMessages = OnboardingSupport.OnboardingMessages.DocumentationExpandFunctions;
		}
		else if (objectType == SharedObjectTypeEnum.ObjectType.Structure)
		{
			onboardingMessages = OnboardingSupport.OnboardingMessages.DocumentationExpandStructures;
		}
		if (onboardingMessages.HasValue && TreeListHelpers.GetNodeBounds(folderNode).HasValue)
		{
			OnboardingSupport.ShowPanel(onboardingMessages.Value, FindForm(), () => TreeListHelpers.GetNodeBounds(folderNode).Value);
		}
	}

	private void ShowColumnsOpenMessageClosedOnboarding(TreeListNode treeListNode)
	{
		TreeListNode modulesFolder = new SearchTreeNodeOperation().FindNode(treeListNode.Nodes, SharedObjectTypeEnum.ObjectType.Module);
		if (TreeListHelpers.GetNodeBounds(modulesFolder).HasValue)
		{
			OnboardingSupport.ShowPanel(OnboardingSupport.OnboardingMessages.ColumnsOpenMessageClosed, FindForm(), () => TreeListHelpers.GetNodeBounds(modulesFolder).Value);
		}
	}

	public void SaveUserColumns()
	{
		tableUserControl.SaveColumns();
		procedureUserControl.SaveColumns();
		functionSummaryUserControl.SaveColumns();
		procedureSummaryUserControl.SaveColumns();
		tableSummaryUserControl.SaveColumns();
		objectSummaryUserControl.SaveColumns();
		viewSummaryUserControl.SaveColumns();
		moduleSummaryUserControl.SaveColumns();
		termSummaryUserControl.SaveColumns();
		UserViewData.SaveToXML();
	}

	internal void ReloadUserControlAfterMovingNode(DBTreeNode movedDBTreeNode)
	{
		if (movedDBTreeNode != null)
		{
			BasePanelControl visibleUserControl = GetVisibleUserControl();
			if (visibleUserControl?.ObjectType == movedDBTreeNode.ObjectType && visibleUserControl.ObjectId == movedDBTreeNode.Id)
			{
				visibleUserControl.SetParameters(movedDBTreeNode, CustomFieldsSupport);
			}
			else if (visibleUserControl != null && visibleUserControl is IPanelWithDataLineage panelWithDataLineage)
			{
				panelWithDataLineage.RefreshDataLineage(forceRefresh: true);
			}
			else
			{
				GetVisibleSummaryUserControl()?.SetParameters();
			}
		}
	}

	private void RefreshDataLineageIfVisible()
	{
		if (GetVisibleUserControl() is IPanelWithDataLineage panelWithDataLineage)
		{
			panelWithDataLineage.RefreshDataLineage(forceRefresh: true);
		}
	}

	internal void SelectDBTreeNode(TreeListNode focusedNode)
	{
		metadataTreeList.FocusedNode = focusedNode;
	}

	public void SortModulesAlphabetically(bool fromCustomFocus)
	{
		try
		{
			if (GeneralMessageBoxesHandling.Show("Are you sure you want to sort Subject Areas alphabetically?", "Sort alphabetically", MessageBoxButtons.YesNo, MessageBoxIcon.Question, null, 1, FindForm()).DialogResult != DialogResult.Yes)
			{
				return;
			}
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			TreeListNode treeListNode = (fromCustomFocus ? TreeListHelpers.CustomFocusedTreeListNode : metadataTreeList.FocusedNode);
			DBTreeNode node = TreeListHelpers.GetNode(treeListNode);
			if (node != null)
			{
				metadataTreeList.BeginUpdate();
				moduleSummaryUserControl.CloseEditor();
				DB.Module.RemoveDatabaseModulesOrdinalPosition(node.DatabaseId, base.ParentForm);
				if (node.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
				{
					metadataTreeList.FocusedNode = treeListNode.ParentNode;
					treeListNode = treeListNode.ParentNode;
					DBTreeMenu.ReloadModules(node.ParentNode);
				}
				else if (node.ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database)
				{
					metadataTreeList.FocusedNode = treeListNode;
					DBTreeMenu.ReloadModules(node);
				}
				TreeListNode treeListNode2 = SearchTreeNodeOperation.FindModuleDirectory(metadataTreeList.Nodes, node.DatabaseId);
				TreeListHelpers.ActualizeModulesOrder(treeListNode2, base.ParentForm);
				metadataTreeList.EndUpdate();
				if (GetVisibleSummaryUserControl() == moduleSummaryUserControl)
				{
					moduleSummaryUserControl.ReloadRows();
				}
				DBTreeNode node2 = TreeListHelpers.GetNode(treeListNode);
				RefreshProgressIfShown(showWaitForm: true, node2);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, base.ParentForm);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	public void MoveModuleToTheTop(bool fromCustomFocus)
	{
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			TreeListNode treeListNode = (fromCustomFocus ? TreeListHelpers.CustomFocusedTreeListNode : metadataTreeList.FocusedNode);
			DBTreeNode node = TreeListHelpers.GetNode(treeListNode);
			if (node != null && node.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
			{
				SetModuleIndex(treeListNode, 0);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, base.ParentForm);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	public void MoveModuleToTheBottom(bool fromCustomFocus)
	{
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			TreeListNode treeListNode = (fromCustomFocus ? TreeListHelpers.CustomFocusedTreeListNode : metadataTreeList.FocusedNode);
			DBTreeNode node = TreeListHelpers.GetNode(treeListNode);
			if (node != null && node.ObjectType == SharedObjectTypeEnum.ObjectType.Module)
			{
				SetModuleIndex(treeListNode, treeListNode.ParentNode.Nodes.IndexOf(treeListNode.ParentNode.Nodes.LastNode) + 1);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, base.ParentForm);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	public void MoveModuleToTheTop(int moduleId, bool reloadModulesSummaryRows = true, bool saveChangeToDatabase = true)
	{
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			if (TreeListHelpers.GetDBTreeNode(metadataTreeList.FocusedNode).ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database && !metadataTreeList.FocusedNode.Expanded)
			{
				metadataTreeList.FocusedNode.Expand();
			}
			TreeListNode treeListNode = metadataTreeList.FindNodes((TreeListNode x) => TreeListHelpers.GetDBTreeNode(x).ObjectType == SharedObjectTypeEnum.ObjectType.Module && TreeListHelpers.GetDBTreeNode(x).Id == moduleId).FirstOrDefault();
			if (treeListNode != null)
			{
				SetModuleIndex(treeListNode, 0, reloadModulesSummaryRows, saveChangeToDatabase);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, base.ParentForm);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	public void MoveModuleToTheBottom(int moduleId, bool reloadModulesSummaryRows = true, bool saveChangeToDatabase = true)
	{
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			if (TreeListHelpers.GetDBTreeNode(metadataTreeList.FocusedNode).ObjectType == SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database && !metadataTreeList.FocusedNode.Expanded)
			{
				metadataTreeList.FocusedNode.Expand();
			}
			TreeListNode treeListNode = metadataTreeList.FindNodes((TreeListNode x) => TreeListHelpers.GetDBTreeNode(x).ObjectType == SharedObjectTypeEnum.ObjectType.Module && TreeListHelpers.GetDBTreeNode(x).Id == moduleId).FirstOrDefault();
			if (treeListNode != null)
			{
				SetModuleIndex(treeListNode, metadataTreeList.FocusedNode.Nodes.IndexOf(metadataTreeList.FocusedNode.Nodes.LastNode) + 1, reloadModulesSummaryRows, saveChangeToDatabase);
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, base.ParentForm);
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private void SetModuleIndex(TreeListNode treeListNode, int nodeIndex, bool reloadModulesSummaryRows = true, bool saveChangeToDatabase = true)
	{
		metadataTreeList.CloseEditor();
		GetVisibleSummaryUserControl()?.CloseEditor();
		if (treeListNode != null)
		{
			metadataTreeList.SetNodeIndex(treeListNode, nodeIndex);
		}
		if (saveChangeToDatabase)
		{
			ActualizeModulesOrder(treeListNode);
		}
		if (reloadModulesSummaryRows)
		{
			ReloadModuleSummaryControlRows();
		}
	}

	public void ReloadModuleSummaryControlRows()
	{
		if (GetVisibleSummaryUserControl() is ModuleSummaryUserControl moduleSummaryUserControl)
		{
			moduleSummaryUserControl.ReloadRows();
		}
	}

	public void ChangeLineageDiagramColumnsVisibility(bool showColumns)
	{
		if (GetVisibleUserControl() is IPanelWithDataLineage panelWithDataLineage)
		{
			panelWithDataLineage.GetDataLineageControl()?.ChangeColumnsVisibility(showColumns);
		}
	}

	public void ChangeLineageDiagramZoom(bool zoomIn)
	{
		if (GetVisibleUserControl() is IPanelWithDataLineage panelWithDataLineage)
		{
			panelWithDataLineage.GetDataLineageControl()?.ChangeDiagramZoom(zoomIn);
		}
	}

	public void DataLineageModeChanged(bool isDiagramVisible)
	{
		this.DataLineageDiagramVisibilityEvent?.Invoke(null, new BoolEventArgs(isDiagramVisible));
	}

	public void DataLineageTabVisibilityChanged(bool isDataLineageTabVisible)
	{
		this.DataLineageTabVisibilityEvent?.Invoke(null, new BoolEventArgs(isDataLineageTabVisible));
	}

	public void DataLineageColumnsVisibilityChanged(bool areColumnsVisible)
	{
		this.DataLineageColumnsVisibilityEvent?.Invoke(null, new BoolEventArgs(areColumnsVisible));
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule treeListFormatRule = new DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleValue formatConditionRuleValue = new DevExpress.XtraEditors.FormatConditionRuleValue();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.WindowControls.MetadataEditorUserControl));
		this.treelistBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem3 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem4 = new DevExpress.XtraBars.BarButtonItem();
		this.dbTreeToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.leftPanelXtraTabControl = new Dataedo.App.UserControls.CustomWithBordersXtraTabControl();
		this.repositoryXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.metadataTreeList = new DevExpress.XtraTreeList.TreeList();
		this.nameTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.titleRepositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.progressTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.progressDatabaseRepositoryItemProgressBar = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.progressModuleRepositoryItemProgressBar = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.progressFolderRepositoryItemProgressBar = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.progressSimpleObjectRepositoryItemProgressBar = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.loadingRepositoryItemProgressBar = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.treeMenuImageCollection = new DevExpress.Utils.ImageCollection(this.components);
		this.bannerControl = new Dataedo.App.UserControls.BannerControl();
		this.searchXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.searchLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.searchTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.advancedSearchSplitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
		this.advancedSearchPanel = new Dataedo.App.UserControls.Search.AdvancedSearchPanel();
		this.searchesXtraTabControl = new DevExpress.XtraTab.XtraTabControl();
		this.searchResultsGridViewXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.searchResultsListGridControl = new DevExpress.XtraGrid.GridControl();
		this.searchResultsListGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.SearchResultsListIconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemPictureEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.SearchResultsListNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.SearchResultsListNameRepositoryItemRichTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemRichTextEdit();
		this.SearchResultsListDocumentationNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.SearchResultsListNTitleRepositoryItemRichTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemRichTextEdit();
		this.searchResultsListToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.InnerGrid = new Dataedo.App.UserControls.CustomGridUserControl();
		this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.searchResultsTreeListXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.searchResultsTreeList = new DevExpress.XtraTreeList.TreeList();
		this.NameSearchResultsTreeListColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.searchResultsTreeListNameRepositoryItemRichTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemRichTextEdit();
		this.DocumentationsCheckedComboBoxEdit = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.TypesCheckedComboBoxEdit = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.deletedObjectsImageCollection = new DevExpress.Utils.ImageCollection(this.components);
		this.treePopupImageCollection = new DevExpress.Utils.ImageCollection(this.components);
		this.xlsxSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
		this.treeList1 = new DevExpress.XtraTreeList.TreeList();
		this.treeList2 = new DevExpress.XtraTreeList.TreeList();
		this.metadataEditorSplitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
		this.repositoryItemCheckedComboBoxEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.searchProgressPanel = new DevExpress.XtraWaitForm.ProgressPanel();
		this.searchProgressPanelPanel = new System.Windows.Forms.Panel();
		this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true, typeof(System.Windows.Forms.UserControl));
		((System.ComponentModel.ISupportInitialize)this.treelistBarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.leftPanelXtraTabControl).BeginInit();
		this.leftPanelXtraTabControl.SuspendLayout();
		this.repositoryXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.metadataTreeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.progressDatabaseRepositoryItemProgressBar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.progressModuleRepositoryItemProgressBar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.progressFolderRepositoryItemProgressBar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.progressSimpleObjectRepositoryItemProgressBar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.loadingRepositoryItemProgressBar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.treeMenuImageCollection).BeginInit();
		this.searchXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.searchLayoutControl).BeginInit();
		this.searchLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.searchTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.advancedSearchSplitContainerControl).BeginInit();
		this.advancedSearchSplitContainerControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.searchesXtraTabControl).BeginInit();
		this.searchesXtraTabControl.SuspendLayout();
		this.searchResultsGridViewXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.searchResultsListGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.searchResultsListGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SearchResultsListNameRepositoryItemRichTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SearchResultsListNTitleRepositoryItemRichTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.InnerGrid).BeginInit();
		this.searchResultsTreeListXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.searchResultsTreeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.searchResultsTreeListNameRepositoryItemRichTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.DocumentationsCheckedComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.TypesCheckedComboBoxEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.deletedObjectsImageCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.treePopupImageCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.treeList1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.treeList2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.metadataEditorSplitContainerControl).BeginInit();
		this.metadataEditorSplitContainerControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckedComboBoxEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		this.searchProgressPanelPanel.SuspendLayout();
		base.SuspendLayout();
		this.treelistBarManager.DockControls.Add(this.barDockControlTop);
		this.treelistBarManager.DockControls.Add(this.barDockControlBottom);
		this.treelistBarManager.DockControls.Add(this.barDockControlLeft);
		this.treelistBarManager.DockControls.Add(this.barDockControlRight);
		this.treelistBarManager.Form = this;
		this.treelistBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[4] { this.barButtonItem1, this.barButtonItem2, this.barButtonItem3, this.barButtonItem4 });
		this.treelistBarManager.MaxItemId = 5;
		this.treelistBarManager.ShowScreenTipsInMenus = true;
		this.treelistBarManager.ToolTipController = this.dbTreeToolTipController;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.treelistBarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(1200, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 923);
		this.barDockControlBottom.Manager = this.treelistBarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(1200, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.treelistBarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 923);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1200, 0);
		this.barDockControlRight.Manager = this.treelistBarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 923);
		this.barButtonItem1.Caption = "barButtonItem1";
		this.barButtonItem1.Id = 1;
		this.barButtonItem1.Name = "barButtonItem1";
		this.barButtonItem2.Caption = "barButtonItem2";
		this.barButtonItem2.Id = 2;
		this.barButtonItem2.Name = "barButtonItem2";
		this.barButtonItem3.Caption = "barButtonItem3";
		this.barButtonItem3.Id = 3;
		this.barButtonItem3.Name = "barButtonItem3";
		this.barButtonItem4.Caption = "barButtonItem4";
		this.barButtonItem4.Id = 4;
		this.barButtonItem4.Name = "barButtonItem4";
		this.dbTreeToolTipController.AutoPopDelay = 15000;
		this.dbTreeToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.dbTreeToolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(dbTreeToolTipController_GetActiveObjectInfo);
		this.leftPanelXtraTabControl.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InAllTabPageHeaders;
		this.leftPanelXtraTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.leftPanelXtraTabControl.Location = new System.Drawing.Point(0, 0);
		this.leftPanelXtraTabControl.Margin = new System.Windows.Forms.Padding(0);
		this.leftPanelXtraTabControl.Name = "leftPanelXtraTabControl";
		this.leftPanelXtraTabControl.SelectedTabPage = this.repositoryXtraTabPage;
		this.leftPanelXtraTabControl.ShowTabHeader = DevExpress.Utils.DefaultBoolean.False;
		this.leftPanelXtraTabControl.Size = new System.Drawing.Size(397, 923);
		this.leftPanelXtraTabControl.TabIndex = 0;
		this.leftPanelXtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.repositoryXtraTabPage, this.searchXtraTabPage });
		this.leftPanelXtraTabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(leftPanelXtraTabControl_SelectedPageChanged);
		this.leftPanelXtraTabControl.CloseButtonClick += new System.EventHandler(leftPanelXtraTabControl_CloseButtonClick);
		this.leftPanelXtraTabControl.KeyDown += new System.Windows.Forms.KeyEventHandler(leftPanelXtraTabControl_KeyDown);
		this.leftPanelXtraTabControl.Resize += new System.EventHandler(leftPanelXtraTabControl_Resize);
		this.repositoryXtraTabPage.Controls.Add(this.metadataTreeList);
		this.repositoryXtraTabPage.Controls.Add(this.bannerControl);
		this.repositoryXtraTabPage.Name = "repositoryXtraTabPage";
		this.repositoryXtraTabPage.ShowCloseButton = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryXtraTabPage.Size = new System.Drawing.Size(395, 921);
		this.repositoryXtraTabPage.Text = "Repository";
		this.metadataTreeList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.metadataTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[2] { this.nameTreeListColumn, this.progressTreeListColumn });
		this.metadataTreeList.Dock = System.Windows.Forms.DockStyle.Fill;
		treeListFormatRule.Name = "Format0";
		formatConditionRuleValue.Condition = DevExpress.XtraEditors.FormatCondition.Equal;
		treeListFormatRule.Rule = formatConditionRuleValue;
		this.metadataTreeList.FormatRules.Add(treeListFormatRule);
		this.metadataTreeList.KeyFieldName = "id";
		this.metadataTreeList.Location = new System.Drawing.Point(0, 0);
		this.metadataTreeList.Name = "metadataTreeList";
		this.metadataTreeList.OptionsBehavior.AllowExpandAnimation = DevExpress.Utils.DefaultBoolean.True;
		this.metadataTreeList.OptionsBehavior.Editable = false;
		this.metadataTreeList.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.metadataTreeList.OptionsCustomization.AllowColumnMoving = false;
		this.metadataTreeList.OptionsCustomization.AllowQuickHideColumns = false;
		this.metadataTreeList.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Single;
		this.metadataTreeList.OptionsDragAndDrop.DropNodesMode = DevExpress.XtraTreeList.DropNodesMode.Advanced;
		this.metadataTreeList.OptionsView.ShowBandsMode = DevExpress.Utils.DefaultBoolean.False;
		this.metadataTreeList.OptionsView.ShowHorzLines = false;
		this.metadataTreeList.OptionsView.ShowIndicator = false;
		this.metadataTreeList.OptionsView.ShowVertLines = false;
		this.metadataTreeList.OptionsView.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.None;
		this.metadataTreeList.ParentFieldName = "parentId";
		this.metadataTreeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.titleRepositoryItemTextEdit, this.progressDatabaseRepositoryItemProgressBar, this.progressModuleRepositoryItemProgressBar, this.progressFolderRepositoryItemProgressBar, this.progressSimpleObjectRepositoryItemProgressBar, this.loadingRepositoryItemProgressBar });
		this.metadataTreeList.SelectImageList = this.treeMenuImageCollection;
		this.metadataTreeList.Size = new System.Drawing.Size(395, 786);
		this.metadataTreeList.TabIndex = 0;
		this.metadataTreeList.ToolTipController = this.dbTreeToolTipController;
		this.metadataTreeList.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(metadataTreeList_GetSelectImage);
		this.metadataTreeList.CustomNodeCellEdit += new DevExpress.XtraTreeList.GetCustomNodeCellEditEventHandler(metadataTreeList_CustomNodeCellEdit);
		this.metadataTreeList.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(metadataTreeList_NodeCellStyle);
		this.metadataTreeList.BeforeExpand += new DevExpress.XtraTreeList.BeforeExpandEventHandler(metadataTreeList_BeforeExpand);
		this.metadataTreeList.BeforeCollapse += new DevExpress.XtraTreeList.BeforeCollapseEventHandler(metadataTreeList_BeforeCollapse);
		this.metadataTreeList.BeforeFocusNode += new DevExpress.XtraTreeList.BeforeFocusNodeEventHandler(metadataTreeList_BeforeFocusNode);
		this.metadataTreeList.AfterExpand += new DevExpress.XtraTreeList.NodeEventHandler(metadataTreeList_AfterExpand);
		this.metadataTreeList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(metadataTreeList_FocusedNodeChanged);
		this.metadataTreeList.CalcNodeDragImageIndex += new DevExpress.XtraTreeList.CalcNodeDragImageIndexEventHandler(metadataTreeList_CalcNodeDragImageIndex);
		this.metadataTreeList.ShownEditor += new System.EventHandler(metadataTreeList_ShownEditor);
		this.metadataTreeList.HiddenEditor += new System.EventHandler(metadataTreeList_HiddenEditor);
		this.metadataTreeList.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(metadataTreeList_CustomDrawNodeCell);
		this.metadataTreeList.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(metadataTreeList_PopupMenuShowing);
		this.metadataTreeList.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(metadataTreeList_CellValueChanged);
		this.metadataTreeList.DragDrop += new System.Windows.Forms.DragEventHandler(metadataTreeList_DragDrop);
		this.metadataTreeList.DragEnter += new System.Windows.Forms.DragEventHandler(metadataTreeList_DragEnter);
		this.metadataTreeList.DragOver += new System.Windows.Forms.DragEventHandler(metadataTreeList_DragOver);
		this.metadataTreeList.DragLeave += new System.EventHandler(metadataTreeList_DragLeave);
		this.metadataTreeList.KeyDown += new System.Windows.Forms.KeyEventHandler(metadataTreeList_KeyDown);
		this.metadataTreeList.Leave += new System.EventHandler(metadataTreeList_Leave);
		this.metadataTreeList.MouseDown += new System.Windows.Forms.MouseEventHandler(metadataTreeList_MouseDown);
		this.metadataTreeList.MouseUp += new System.Windows.Forms.MouseEventHandler(metadataTreeList_MouseUp);
		this.nameTreeListColumn.Caption = "Documentation repository";
		this.nameTreeListColumn.ColumnEdit = this.titleRepositoryItemTextEdit;
		this.nameTreeListColumn.FieldName = "name";
		this.nameTreeListColumn.MinWidth = 33;
		this.nameTreeListColumn.Name = "nameTreeListColumn";
		this.nameTreeListColumn.OptionsColumn.AllowSort = false;
		this.nameTreeListColumn.OptionsFilter.AllowFilter = false;
		this.nameTreeListColumn.Visible = true;
		this.nameTreeListColumn.VisibleIndex = 0;
		this.nameTreeListColumn.Width = 296;
		this.titleRepositoryItemTextEdit.AutoHeight = false;
		this.titleRepositoryItemTextEdit.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.titleRepositoryItemTextEdit.MaxLength = 64;
		this.titleRepositoryItemTextEdit.Name = "titleRepositoryItemTextEdit";
		this.titleRepositoryItemTextEdit.ValidateOnEnterKey = true;
		this.titleRepositoryItemTextEdit.EditValueChanged += new System.EventHandler(titleRepositoryItemTextEdit_EditValueChanged);
		this.titleRepositoryItemTextEdit.KeyDown += new System.Windows.Forms.KeyEventHandler(titleRepositoryItemTextEdit_KeyDown);
		this.titleRepositoryItemTextEdit.Leave += new System.EventHandler(titleRepositoryItemTextEdit_Leave);
		this.titleRepositoryItemTextEdit.Validating += new System.ComponentModel.CancelEventHandler(titleRepositoryItemTextEdit_Validating);
		this.progressTreeListColumn.Caption = "Descriptions";
		this.progressTreeListColumn.FieldName = "progress";
		this.progressTreeListColumn.Name = "progressTreeListColumn";
		this.progressTreeListColumn.OptionsColumn.AllowSort = false;
		this.progressTreeListColumn.OptionsFilter.AllowFilter = false;
		this.progressTreeListColumn.Width = 61;
		this.progressDatabaseRepositoryItemProgressBar.EndColor = System.Drawing.Color.FromArgb(47, 92, 176);
		this.progressDatabaseRepositoryItemProgressBar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
		this.progressDatabaseRepositoryItemProgressBar.LookAndFeel.UseDefaultLookAndFeel = false;
		this.progressDatabaseRepositoryItemProgressBar.Name = "progressDatabaseRepositoryItemProgressBar";
		this.progressDatabaseRepositoryItemProgressBar.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.progressDatabaseRepositoryItemProgressBar.ShowTitle = true;
		this.progressDatabaseRepositoryItemProgressBar.StartColor = System.Drawing.Color.FromArgb(47, 92, 176);
		this.progressModuleRepositoryItemProgressBar.EndColor = System.Drawing.Color.FromArgb(88, 133, 202);
		this.progressModuleRepositoryItemProgressBar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
		this.progressModuleRepositoryItemProgressBar.LookAndFeel.UseDefaultLookAndFeel = false;
		this.progressModuleRepositoryItemProgressBar.Name = "progressModuleRepositoryItemProgressBar";
		this.progressModuleRepositoryItemProgressBar.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.progressModuleRepositoryItemProgressBar.ShowTitle = true;
		this.progressModuleRepositoryItemProgressBar.StartColor = System.Drawing.Color.FromArgb(88, 133, 202);
		this.progressFolderRepositoryItemProgressBar.EndColor = System.Drawing.Color.FromArgb(121, 162, 216);
		this.progressFolderRepositoryItemProgressBar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
		this.progressFolderRepositoryItemProgressBar.LookAndFeel.UseDefaultLookAndFeel = false;
		this.progressFolderRepositoryItemProgressBar.Name = "progressFolderRepositoryItemProgressBar";
		this.progressFolderRepositoryItemProgressBar.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.progressFolderRepositoryItemProgressBar.ShowTitle = true;
		this.progressFolderRepositoryItemProgressBar.StartColor = System.Drawing.Color.FromArgb(121, 162, 216);
		this.progressSimpleObjectRepositoryItemProgressBar.EndColor = System.Drawing.Color.FromArgb(151, 185, 227);
		this.progressSimpleObjectRepositoryItemProgressBar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
		this.progressSimpleObjectRepositoryItemProgressBar.LookAndFeel.UseDefaultLookAndFeel = false;
		this.progressSimpleObjectRepositoryItemProgressBar.Name = "progressSimpleObjectRepositoryItemProgressBar";
		this.progressSimpleObjectRepositoryItemProgressBar.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.progressSimpleObjectRepositoryItemProgressBar.ShowTitle = true;
		this.progressSimpleObjectRepositoryItemProgressBar.StartColor = System.Drawing.Color.FromArgb(151, 185, 227);
		this.loadingRepositoryItemProgressBar.EndColor = System.Drawing.Color.FromArgb(214, 132, 41);
		this.loadingRepositoryItemProgressBar.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Style3D;
		this.loadingRepositoryItemProgressBar.LookAndFeel.UseDefaultLookAndFeel = false;
		this.loadingRepositoryItemProgressBar.Name = "loadingRepositoryItemProgressBar";
		this.loadingRepositoryItemProgressBar.ProgressViewStyle = DevExpress.XtraEditors.Controls.ProgressViewStyle.Solid;
		this.loadingRepositoryItemProgressBar.ShowTitle = true;
		this.loadingRepositoryItemProgressBar.StartColor = System.Drawing.Color.FromArgb(214, 132, 41);
		this.treeMenuImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("treeMenuImageCollection.ImageStream");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.folder_16, "folder", typeof(Dataedo.App.Properties.Resources), 0, "folder_16");
		this.treeMenuImageCollection.Images.SetKeyName(0, "folder");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.folder_open_16, "folder_open", typeof(Dataedo.App.Properties.Resources), 1, "folder_open_16");
		this.treeMenuImageCollection.Images.SetKeyName(1, "folder_open");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.function_16, "function", typeof(Dataedo.App.Properties.Resources), 2, "function_16");
		this.treeMenuImageCollection.Images.SetKeyName(2, "function");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.function_deleted_16, "function_deleted", typeof(Dataedo.App.Properties.Resources), 3, "function_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(3, "function_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.function_new_16, "function_new", typeof(Dataedo.App.Properties.Resources), 4, "function_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(4, "function_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.function_updated_16, "function_updated", typeof(Dataedo.App.Properties.Resources), 5, "function_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(5, "function_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.function_user_16, "function_user", typeof(Dataedo.App.Properties.Resources), 6, "function_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(6, "function_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.module_16, "module", typeof(Dataedo.App.Properties.Resources), 7, "module_16");
		this.treeMenuImageCollection.Images.SetKeyName(7, "module");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.procedure_16, "procedure", typeof(Dataedo.App.Properties.Resources), 8, "procedure_16");
		this.treeMenuImageCollection.Images.SetKeyName(8, "procedure");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.procedure_deleted_16, "procedure_deleted", typeof(Dataedo.App.Properties.Resources), 9, "procedure_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(9, "procedure_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.procedure_new_16, "procedure_new", typeof(Dataedo.App.Properties.Resources), 10, "procedure_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(10, "procedure_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.procedure_updated_16, "procedure_updated", typeof(Dataedo.App.Properties.Resources), 11, "procedure_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(11, "procedure_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.procedure_user_16, "procedure_user", typeof(Dataedo.App.Properties.Resources), 12, "procedure_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(12, "procedure_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.table_16, "table", typeof(Dataedo.App.Properties.Resources), 13, "table_16");
		this.treeMenuImageCollection.Images.SetKeyName(13, "table");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.table_deleted_16, "table_deleted", typeof(Dataedo.App.Properties.Resources), 14, "table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(14, "table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.table_new_16, "table_new", typeof(Dataedo.App.Properties.Resources), 15, "table_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(15, "table_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.table_updated_16, "table_updated", typeof(Dataedo.App.Properties.Resources), 16, "table_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(16, "table_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.table_user_16, "table_user", typeof(Dataedo.App.Properties.Resources), 17, "table_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(17, "table_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.view_16, "view", typeof(Dataedo.App.Properties.Resources), 18, "view_16");
		this.treeMenuImageCollection.Images.SetKeyName(18, "view");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.view_deleted_16, "view_deleted", typeof(Dataedo.App.Properties.Resources), 19, "view_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(19, "view_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.view_new_16, "view_new", typeof(Dataedo.App.Properties.Resources), 20, "view_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(20, "view_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.view_updated_16, "view_updated", typeof(Dataedo.App.Properties.Resources), 21, "view_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(21, "view_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.view_user_16, "view_user", typeof(Dataedo.App.Properties.Resources), 22, "view_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(22, "view_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.documentation_16, "documentation", typeof(Dataedo.App.Properties.Resources), 23, "documentation_16");
		this.treeMenuImageCollection.Images.SetKeyName(23, "documentation");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.column_16, "column", typeof(Dataedo.App.Properties.Resources), 24, "column_16");
		this.treeMenuImageCollection.Images.SetKeyName(24, "column");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.column_deleted_16, "column_deleted", typeof(Dataedo.App.Properties.Resources), 25, "column_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(25, "column_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_16, "parameter", typeof(Dataedo.App.Properties.Resources), 26, "parameter_16");
		this.treeMenuImageCollection.Images.SetKeyName(26, "parameter");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_deleted_16, "parameter_deleted", typeof(Dataedo.App.Properties.Resources), 27, "parameter_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(27, "parameter_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_in_16, "parameter_in", typeof(Dataedo.App.Properties.Resources), 28, "parameter_in_16");
		this.treeMenuImageCollection.Images.SetKeyName(28, "parameter_in");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_in_deleted_16, "parameter_in_deleted", typeof(Dataedo.App.Properties.Resources), 29, "parameter_in_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(29, "parameter_in_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_inout_16, "parameter_inout", typeof(Dataedo.App.Properties.Resources), 30, "parameter_inout_16");
		this.treeMenuImageCollection.Images.SetKeyName(30, "parameter_inout");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_inout_deleted_16, "parameter_inout_deleted", typeof(Dataedo.App.Properties.Resources), 31, "parameter_inout_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(31, "parameter_inout_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_out_16, "parameter_out", typeof(Dataedo.App.Properties.Resources), 32, "parameter_out_16");
		this.treeMenuImageCollection.Images.SetKeyName(32, "parameter_out");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parameter_out_deleted_16, "parameter_out_deleted", typeof(Dataedo.App.Properties.Resources), 33, "parameter_out_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(33, "parameter_out_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_16, "primary_key", typeof(Dataedo.App.Properties.Resources), 34, "primary_key_16");
		this.treeMenuImageCollection.Images.SetKeyName(34, "primary_key");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_deleted_16, "primary_key_deleted", typeof(Dataedo.App.Properties.Resources), 35, "primary_key_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(35, "primary_key_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_disabled_16, "primary_key_disabled", typeof(Dataedo.App.Properties.Resources), 36, "primary_key_disabled_16");
		this.treeMenuImageCollection.Images.SetKeyName(36, "primary_key_disabled");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_user_16, "primary_key_user", typeof(Dataedo.App.Properties.Resources), 37, "primary_key_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(37, "primary_key_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_mx_1x_16, "relation_mx_1x_16", typeof(Dataedo.App.Properties.Resources), 38);
		this.treeMenuImageCollection.Images.SetKeyName(38, "relation_mx_1x_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_1x_1x_16, "relation_1x_1x_16", typeof(Dataedo.App.Properties.Resources), 39);
		this.treeMenuImageCollection.Images.SetKeyName(39, "relation_1x_1x_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.trigger_active_16, "trigger_active", typeof(Dataedo.App.Properties.Resources), 40, "trigger_active_16");
		this.treeMenuImageCollection.Images.SetKeyName(40, "trigger_active");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.trigger_deleted_16, "trigger_deleted", typeof(Dataedo.App.Properties.Resources), 41, "trigger_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(41, "trigger_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.trigger_disabled_16, "trigger_disabled", typeof(Dataedo.App.Properties.Resources), 42, "trigger_disabled_16");
		this.treeMenuImageCollection.Images.SetKeyName(42, "trigger_disabled");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_16, "unique_key", typeof(Dataedo.App.Properties.Resources), 43, "unique_key_16");
		this.treeMenuImageCollection.Images.SetKeyName(43, "unique_key");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_deleted_16, "unique_key_deleted", typeof(Dataedo.App.Properties.Resources), 44, "unique_key_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(44, "unique_key_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_disabled_16, "unique_key_disabled", typeof(Dataedo.App.Properties.Resources), 45, "unique_key_disabled_16");
		this.treeMenuImageCollection.Images.SetKeyName(45, "unique_key_disabled");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_user_16, "unique_key_user", typeof(Dataedo.App.Properties.Resources), 46, "unique_key_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(46, "unique_key_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.folder_user_16, "folder_module", typeof(Dataedo.App.Properties.Resources), 47, "folder_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(47, "folder_module");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.documentation_disabled_16, "documentation_disabled", typeof(Dataedo.App.Properties.Resources), 48, "documentation_disabled_16");
		this.treeMenuImageCollection.Images.SetKeyName(48, "documentation_disabled");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.collection_16, "collection", typeof(Dataedo.App.Properties.Resources), 49, "collection_16");
		this.treeMenuImageCollection.Images.SetKeyName(49, "collection");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.collection_deleted_16, "collection_deleted", typeof(Dataedo.App.Properties.Resources), 50, "collection_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(50, "collection_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.collection_new_16, "collection_new", typeof(Dataedo.App.Properties.Resources), 51, "collection_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(51, "collection_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.collection_updated_16, "collection_updated", typeof(Dataedo.App.Properties.Resources), 52, "collection_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(52, "collection_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.collection_user_16, "collection_user", typeof(Dataedo.App.Properties.Resources), 53, "collection_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(53, "collection_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cube_16, "cube", typeof(Dataedo.App.Properties.Resources), 54, "cube_16");
		this.treeMenuImageCollection.Images.SetKeyName(54, "cube");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cube_deleted_16, "cube_deleted", typeof(Dataedo.App.Properties.Resources), 55, "cube_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(55, "cube_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cube_new_16, "cube_new", typeof(Dataedo.App.Properties.Resources), 56, "cube_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(56, "cube_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cube_updated_16, "cube_updated", typeof(Dataedo.App.Properties.Resources), 57, "cube_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(57, "cube_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cube_user_16, "cube_user", typeof(Dataedo.App.Properties.Resources), 58, "cube_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(58, "cube_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.custom_object_16, "custom_object", typeof(Dataedo.App.Properties.Resources), 59, "custom_object_16");
		this.treeMenuImageCollection.Images.SetKeyName(59, "custom_object");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.custom_object_deleted_16, "custom_object_deleted", typeof(Dataedo.App.Properties.Resources), 60, "custom_object_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(60, "custom_object_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.custom_object_new_16, "custom_object_new", typeof(Dataedo.App.Properties.Resources), 61, "custom_object_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(61, "custom_object_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.custom_object_updated_16, "custom_object_updated", typeof(Dataedo.App.Properties.Resources), 62, "custom_object_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(62, "custom_object_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.custom_object_user_16, "custom_object_user", typeof(Dataedo.App.Properties.Resources), 63, "custom_object_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(63, "custom_object_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.dimension_16, "dimension", typeof(Dataedo.App.Properties.Resources), 64, "dimension_16");
		this.treeMenuImageCollection.Images.SetKeyName(64, "dimension");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.dimension_deleted_16, "dimension_deleted", typeof(Dataedo.App.Properties.Resources), 65, "dimension_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(65, "dimension_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.dimension_new_16, "dimension_new", typeof(Dataedo.App.Properties.Resources), 66, "dimension_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(66, "dimension_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.dimension_updated_16, "dimension_updated", typeof(Dataedo.App.Properties.Resources), 67, "dimension_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(67, "dimension_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.dimension_user_16, "dimension_user", typeof(Dataedo.App.Properties.Resources), 68, "dimension_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(68, "dimension_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.editioning_view_16, "editioning_view", typeof(Dataedo.App.Properties.Resources), 69, "editioning_view_16");
		this.treeMenuImageCollection.Images.SetKeyName(69, "editioning_view");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.editioning_view_deleted_16, "editioning_view_deleted", typeof(Dataedo.App.Properties.Resources), 70, "editioning_view_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(70, "editioning_view_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.editioning_view_new_16, "editioning_view_new", typeof(Dataedo.App.Properties.Resources), 71, "editioning_view_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(71, "editioning_view_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.editioning_view_updated_16, "editioning_view_updated", typeof(Dataedo.App.Properties.Resources), 72, "editioning_view_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(72, "editioning_view_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.editioning_view_user_16, "editioning_view_user", typeof(Dataedo.App.Properties.Resources), 73, "editioning_view_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(73, "editioning_view_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.entity_16, "entity", typeof(Dataedo.App.Properties.Resources), 74, "entity_16");
		this.treeMenuImageCollection.Images.SetKeyName(74, "entity");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.entity_deleted_16, "entity_deleted", typeof(Dataedo.App.Properties.Resources), 75, "entity_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(75, "entity_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.entity_new_16, "entity_new", typeof(Dataedo.App.Properties.Resources), 76, "entity_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(76, "entity_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.entity_updated_16, "entity_updated", typeof(Dataedo.App.Properties.Resources), 77, "entity_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(77, "entity_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.entity_user_16, "entity_user", typeof(Dataedo.App.Properties.Resources), 78, "entity_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(78, "entity_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_object_16, "external_object", typeof(Dataedo.App.Properties.Resources), 79, "external_object_16");
		this.treeMenuImageCollection.Images.SetKeyName(79, "external_object");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_object_deleted_16, "external_object_deleted", typeof(Dataedo.App.Properties.Resources), 80, "external_object_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(80, "external_object_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_object_new_16, "external_object_new", typeof(Dataedo.App.Properties.Resources), 81, "external_object_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(81, "external_object_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_object_updated_16, "external_object_updated", typeof(Dataedo.App.Properties.Resources), 82, "external_object_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(82, "external_object_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_object_user_16, "external_object_user", typeof(Dataedo.App.Properties.Resources), 83, "external_object_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(83, "external_object_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_table_16, "external_table", typeof(Dataedo.App.Properties.Resources), 84, "external_table_16");
		this.treeMenuImageCollection.Images.SetKeyName(84, "external_table");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_table_deleted_16, "external_table_deleted", typeof(Dataedo.App.Properties.Resources), 85, "external_table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(85, "external_table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_table_new_16, "external_table_new", typeof(Dataedo.App.Properties.Resources), 86, "external_table_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(86, "external_table_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_table_updated_16, "external_table_updated", typeof(Dataedo.App.Properties.Resources), 87, "external_table_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(87, "external_table_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_table_user_16, "external_table_user", typeof(Dataedo.App.Properties.Resources), 88, "external_table_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(88, "external_table_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.file_table_16, "file_table", typeof(Dataedo.App.Properties.Resources), 89, "file_table_16");
		this.treeMenuImageCollection.Images.SetKeyName(89, "file_table");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.file_table_deleted_16, "file_table_deleted", typeof(Dataedo.App.Properties.Resources), 90, "file_table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(90, "file_table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.file_table_new_16, "file_table_new", typeof(Dataedo.App.Properties.Resources), 91, "file_table_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(91, "file_table_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.file_table_updated_16, "file_table_updated", typeof(Dataedo.App.Properties.Resources), 92, "file_table_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(92, "file_table_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.file_table_user_16, "file_table_user", typeof(Dataedo.App.Properties.Resources), 93, "file_table_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(93, "file_table_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.flat_file_16, "flat_file", typeof(Dataedo.App.Properties.Resources), 94, "flat_file_16");
		this.treeMenuImageCollection.Images.SetKeyName(94, "flat_file");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.flat_file_deleted_16, "flat_file_deleted", typeof(Dataedo.App.Properties.Resources), 95, "flat_file_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(95, "flat_file_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.flat_file_new_16, "flat_file_new", typeof(Dataedo.App.Properties.Resources), 96, "flat_file_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(96, "flat_file_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.flat_file_updated_16, "flat_file_updated", typeof(Dataedo.App.Properties.Resources), 97, "flat_file_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(97, "flat_file_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.flat_file_user_16, "flat_file_user", typeof(Dataedo.App.Properties.Resources), 98, "flat_file_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(98, "flat_file_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.foreign_table_16, "foreign_table", typeof(Dataedo.App.Properties.Resources), 99, "foreign_table_16");
		this.treeMenuImageCollection.Images.SetKeyName(99, "foreign_table");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.foreign_table_deleted_16, "foreign_table_deleted", typeof(Dataedo.App.Properties.Resources), 100, "foreign_table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(100, "foreign_table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.foreign_table_new_16, "foreign_table_new", typeof(Dataedo.App.Properties.Resources), 101, "foreign_table_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(101, "foreign_table_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.foreign_table_updated_16, "foreign_table_updated", typeof(Dataedo.App.Properties.Resources), 102, "foreign_table_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(102, "foreign_table_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.foreign_table_user_16, "foreign_table_user", typeof(Dataedo.App.Properties.Resources), 103, "foreign_table_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(103, "foreign_table_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_edge_table_16, "graph_edge_table", typeof(Dataedo.App.Properties.Resources), 104, "graph_edge_table_16");
		this.treeMenuImageCollection.Images.SetKeyName(104, "graph_edge_table");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_edge_table_deleted_16, "graph_edge_table_deleted", typeof(Dataedo.App.Properties.Resources), 105, "graph_edge_table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(105, "graph_edge_table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_edge_table_new_16, "graph_edge_table_new", typeof(Dataedo.App.Properties.Resources), 106, "graph_edge_table_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(106, "graph_edge_table_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_edge_table_updated_16, "graph_edge_table_updated", typeof(Dataedo.App.Properties.Resources), 107, "graph_edge_table_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(107, "graph_edge_table_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_edge_table_user_16, "graph_edge_table_user", typeof(Dataedo.App.Properties.Resources), 108, "graph_edge_table_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(108, "graph_edge_table_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_node_table_16, "graph_node_table", typeof(Dataedo.App.Properties.Resources), 109, "graph_node_table_16");
		this.treeMenuImageCollection.Images.SetKeyName(109, "graph_node_table");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_node_table_deleted_16, "graph_node_table_deleted", typeof(Dataedo.App.Properties.Resources), 110, "graph_node_table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(110, "graph_node_table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_node_table_new_16, "graph_node_table_new", typeof(Dataedo.App.Properties.Resources), 111, "graph_node_table_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(111, "graph_node_table_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_node_table_updated_16, "graph_node_table_updated", typeof(Dataedo.App.Properties.Resources), 112, "graph_node_table_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(112, "graph_node_table_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_node_table_user_16, "graph_node_table_user", typeof(Dataedo.App.Properties.Resources), 113, "graph_node_table_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(113, "graph_node_table_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_table_16, "graph_table", typeof(Dataedo.App.Properties.Resources), 114, "graph_table_16");
		this.treeMenuImageCollection.Images.SetKeyName(114, "graph_table");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_table_deleted_16, "graph_table_deleted", typeof(Dataedo.App.Properties.Resources), 115, "graph_table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(115, "graph_table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_table_new_16, "graph_table_new", typeof(Dataedo.App.Properties.Resources), 116, "graph_table_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(116, "graph_table_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_table_updated_16, "graph_table_updated", typeof(Dataedo.App.Properties.Resources), 117, "graph_table_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(117, "graph_table_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_table_user_16, "graph_table_user", typeof(Dataedo.App.Properties.Resources), 118, "graph_table_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(118, "graph_table_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.history_table_16, "history_table", typeof(Dataedo.App.Properties.Resources), 119, "history_table_16");
		this.treeMenuImageCollection.Images.SetKeyName(119, "history_table");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.history_table_deleted_16, "history_table_deleted", typeof(Dataedo.App.Properties.Resources), 120, "history_table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(120, "history_table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.history_table_new_16, "history_table_new", typeof(Dataedo.App.Properties.Resources), 121, "history_table_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(121, "history_table_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.history_table_updated_16, "history_table_updated", typeof(Dataedo.App.Properties.Resources), 122, "history_table_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(122, "history_table_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.history_table_user_16, "history_table_user", typeof(Dataedo.App.Properties.Resources), 123, "history_table_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(123, "history_table_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.indexed_view_16, "indexed_view", typeof(Dataedo.App.Properties.Resources), 124, "indexed_view_16");
		this.treeMenuImageCollection.Images.SetKeyName(124, "indexed_view");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.indexed_view_deleted_16, "indexed_view_deleted", typeof(Dataedo.App.Properties.Resources), 125, "indexed_view_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(125, "indexed_view_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.indexed_view_new_16, "indexed_view_new", typeof(Dataedo.App.Properties.Resources), 126, "indexed_view_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(126, "indexed_view_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.indexed_view_updated_16, "indexed_view_updated", typeof(Dataedo.App.Properties.Resources), 127, "indexed_view_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(127, "indexed_view_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.indexed_view_user_16, "indexed_view_user", typeof(Dataedo.App.Properties.Resources), 128, "indexed_view_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(128, "indexed_view_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.materialized_view_16, "materialized_view", typeof(Dataedo.App.Properties.Resources), 129, "materialized_view_16");
		this.treeMenuImageCollection.Images.SetKeyName(129, "materialized_view");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.materialized_view_deleted_16, "materialized_view_deleted", typeof(Dataedo.App.Properties.Resources), 130, "materialized_view_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(130, "materialized_view_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.materialized_view_new_16, "materialized_view_new", typeof(Dataedo.App.Properties.Resources), 131, "materialized_view_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(131, "materialized_view_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.materialized_view_updated_16, "materialized_view_updated", typeof(Dataedo.App.Properties.Resources), 132, "materialized_view_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(132, "materialized_view_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.materialized_view_user_16, "materialized_view_user", typeof(Dataedo.App.Properties.Resources), 133, "materialized_view_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(133, "materialized_view_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.named_query_16, "named_query", typeof(Dataedo.App.Properties.Resources), 134, "named_query_16");
		this.treeMenuImageCollection.Images.SetKeyName(134, "named_query");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.named_query_deleted_16, "named_query_deleted", typeof(Dataedo.App.Properties.Resources), 135, "named_query_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(135, "named_query_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.named_query_new_16, "named_query_new", typeof(Dataedo.App.Properties.Resources), 136, "named_query_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(136, "named_query_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.named_query_updated_16, "named_query_updated", typeof(Dataedo.App.Properties.Resources), 137, "named_query_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(137, "named_query_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.named_query_user_16, "named_query_user", typeof(Dataedo.App.Properties.Resources), 138, "named_query_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(138, "named_query_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.object_16, "object", typeof(Dataedo.App.Properties.Resources), 139, "object_16");
		this.treeMenuImageCollection.Images.SetKeyName(139, "object");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.object_deleted_16, "object_deleted", typeof(Dataedo.App.Properties.Resources), 140, "object_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(140, "object_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.object_new_16, "object_new", typeof(Dataedo.App.Properties.Resources), 141, "object_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(141, "object_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.object_updated_16, "object_updated", typeof(Dataedo.App.Properties.Resources), 142, "object_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(142, "object_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.object_user_16, "object_user", typeof(Dataedo.App.Properties.Resources), 143, "object_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(143, "object_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.package_16, "package", typeof(Dataedo.App.Properties.Resources), 144, "package_16");
		this.treeMenuImageCollection.Images.SetKeyName(144, "package");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.package_deleted_16, "package_deleted", typeof(Dataedo.App.Properties.Resources), 145, "package_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(145, "package_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.package_new_16, "package_new", typeof(Dataedo.App.Properties.Resources), 146, "package_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(146, "package_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.package_updated_16, "package_updated", typeof(Dataedo.App.Properties.Resources), 147, "package_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(147, "package_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.package_user_16, "package_user", typeof(Dataedo.App.Properties.Resources), 148, "package_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(148, "package_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_active_16, "rule_active", typeof(Dataedo.App.Properties.Resources), 149, "rule_active_16");
		this.treeMenuImageCollection.Images.SetKeyName(149, "rule_active");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_active_deleted_16, "rule_active_deleted", typeof(Dataedo.App.Properties.Resources), 150, "rule_active_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(150, "rule_active_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_active_new_16, "rule_active_new", typeof(Dataedo.App.Properties.Resources), 151, "rule_active_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(151, "rule_active_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_active_updated_16, "rule_active_updated", typeof(Dataedo.App.Properties.Resources), 152, "rule_active_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(152, "rule_active_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_active_user_16, "rule_active_user", typeof(Dataedo.App.Properties.Resources), 153, "rule_active_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(153, "rule_active_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_disabled_16, "rule_disabled", typeof(Dataedo.App.Properties.Resources), 154, "rule_disabled_16");
		this.treeMenuImageCollection.Images.SetKeyName(154, "rule_disabled");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_disabled_deleted_16, "rule_disabled_deleted", typeof(Dataedo.App.Properties.Resources), 155, "rule_disabled_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(155, "rule_disabled_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_disabled_new_16, "rule_disabled_new", typeof(Dataedo.App.Properties.Resources), 156, "rule_disabled_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(156, "rule_disabled_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_disabled_updated_16, "rule_disabled_updated", typeof(Dataedo.App.Properties.Resources), 157, "rule_disabled_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(157, "rule_disabled_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_disabled_user_16, "rule_disabled_user", typeof(Dataedo.App.Properties.Resources), 158, "rule_disabled_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(158, "rule_disabled_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.search_index_16, "search_index", typeof(Dataedo.App.Properties.Resources), 159, "search_index_16");
		this.treeMenuImageCollection.Images.SetKeyName(159, "search_index");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.search_index_deleted_16, "search_index_deleted", typeof(Dataedo.App.Properties.Resources), 160, "search_index_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(160, "search_index_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.search_index_new_16, "search_index_new", typeof(Dataedo.App.Properties.Resources), 161, "search_index_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(161, "search_index_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.search_index_updated_16, "search_index_updated", typeof(Dataedo.App.Properties.Resources), 162, "search_index_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(162, "search_index_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.search_index_user_16, "search_index_user", typeof(Dataedo.App.Properties.Resources), 163, "search_index_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(163, "search_index_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.standard_object_16, "standard_object", typeof(Dataedo.App.Properties.Resources), 164, "standard_object_16");
		this.treeMenuImageCollection.Images.SetKeyName(164, "standard_object");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.standard_object_deleted_16, "standard_object_deleted", typeof(Dataedo.App.Properties.Resources), 165, "standard_object_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(165, "standard_object_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.standard_object_new_16, "standard_object_new", typeof(Dataedo.App.Properties.Resources), 166, "standard_object_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(166, "standard_object_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.standard_object_updated_16, "standard_object_updated", typeof(Dataedo.App.Properties.Resources), 167, "standard_object_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(167, "standard_object_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.standard_object_user_16, "standard_object_user", typeof(Dataedo.App.Properties.Resources), 168, "standard_object_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(168, "standard_object_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.system_versioned_table_16, "system_versioned_table", typeof(Dataedo.App.Properties.Resources), 169, "system_versioned_table_16");
		this.treeMenuImageCollection.Images.SetKeyName(169, "system_versioned_table");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.system_versioned_table_deleted_16, "system_versioned_table_deleted", typeof(Dataedo.App.Properties.Resources), 170, "system_versioned_table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(170, "system_versioned_table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.system_versioned_table_new_16, "system_versioned_table_new", typeof(Dataedo.App.Properties.Resources), 171, "system_versioned_table_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(171, "system_versioned_table_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.system_versioned_table_updated_16, "system_versioned_table_updated", typeof(Dataedo.App.Properties.Resources), 172, "system_versioned_table_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(172, "system_versioned_table_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.system_versioned_table_user_16, "system_versioned_table_user", typeof(Dataedo.App.Properties.Resources), 173, "system_versioned_table_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(173, "system_versioned_table_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.collection_deleted_16, "collection_deleted_16", typeof(Dataedo.App.Properties.Resources), 174);
		this.treeMenuImageCollection.Images.SetKeyName(174, "collection_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.column_deleted_16, "column_deleted_16", typeof(Dataedo.App.Properties.Resources), 175);
		this.treeMenuImageCollection.Images.SetKeyName(175, "column_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cube_deleted_16, "cube_deleted_16", typeof(Dataedo.App.Properties.Resources), 176);
		this.treeMenuImageCollection.Images.SetKeyName(176, "cube_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.dimension_deleted_16, "dimension_deleted_16", typeof(Dataedo.App.Properties.Resources), 177);
		this.treeMenuImageCollection.Images.SetKeyName(177, "dimension_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.editioning_view_deleted_16, "editioning_view_deleted_16", typeof(Dataedo.App.Properties.Resources), 178);
		this.treeMenuImageCollection.Images.SetKeyName(178, "editioning_view_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.entity_deleted_16, "entity_deleted_16", typeof(Dataedo.App.Properties.Resources), 179);
		this.treeMenuImageCollection.Images.SetKeyName(179, "entity_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_object_deleted_16, "external_object_deleted_16", typeof(Dataedo.App.Properties.Resources), 180);
		this.treeMenuImageCollection.Images.SetKeyName(180, "external_object_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_table_deleted_16, "external_table_deleted_16", typeof(Dataedo.App.Properties.Resources), 181);
		this.treeMenuImageCollection.Images.SetKeyName(181, "external_table_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.file_table_deleted_16, "file_table_deleted_16", typeof(Dataedo.App.Properties.Resources), 182);
		this.treeMenuImageCollection.Images.SetKeyName(182, "file_table_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.flat_file_deleted_16, "flat_file_deleted_16", typeof(Dataedo.App.Properties.Resources), 183);
		this.treeMenuImageCollection.Images.SetKeyName(183, "flat_file_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.foreign_table_deleted_16, "foreign_table_deleted_16", typeof(Dataedo.App.Properties.Resources), 184);
		this.treeMenuImageCollection.Images.SetKeyName(184, "foreign_table_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.function_deleted_16, "function_deleted_16", typeof(Dataedo.App.Properties.Resources), 185);
		this.treeMenuImageCollection.Images.SetKeyName(185, "function_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_edge_table_deleted_16, "graph_edge_table_deleted_16", typeof(Dataedo.App.Properties.Resources), 186);
		this.treeMenuImageCollection.Images.SetKeyName(186, "graph_edge_table_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_table_deleted_16, "graph_table_deleted_16", typeof(Dataedo.App.Properties.Resources), 187);
		this.treeMenuImageCollection.Images.SetKeyName(187, "graph_table_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.history_table_deleted_16, "history_table_deleted_16", typeof(Dataedo.App.Properties.Resources), 188);
		this.treeMenuImageCollection.Images.SetKeyName(188, "history_table_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.indexed_view_deleted_16, "indexed_view_deleted_16", typeof(Dataedo.App.Properties.Resources), 189);
		this.treeMenuImageCollection.Images.SetKeyName(189, "indexed_view_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.materialized_view_deleted_16, "materialized_view_deleted_16", typeof(Dataedo.App.Properties.Resources), 190);
		this.treeMenuImageCollection.Images.SetKeyName(190, "materialized_view_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.object_deleted_16, "object_deleted_16", typeof(Dataedo.App.Properties.Resources), 191);
		this.treeMenuImageCollection.Images.SetKeyName(191, "object_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.package_deleted_16, "package_deleted_16", typeof(Dataedo.App.Properties.Resources), 192);
		this.treeMenuImageCollection.Images.SetKeyName(192, "package_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_deleted_16, "primary_key_deleted_16", typeof(Dataedo.App.Properties.Resources), 193);
		this.treeMenuImageCollection.Images.SetKeyName(193, "primary_key_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.procedure_deleted_16, "procedure_deleted_16", typeof(Dataedo.App.Properties.Resources), 194);
		this.treeMenuImageCollection.Images.SetKeyName(194, "procedure_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_delete_16, "relation_delete_16", typeof(Dataedo.App.Properties.Resources), 195);
		this.treeMenuImageCollection.Images.SetKeyName(195, "relation_delete_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_deleted_16, "unique_key_deleted_16", typeof(Dataedo.App.Properties.Resources), 196);
		this.treeMenuImageCollection.Images.SetKeyName(196, "unique_key_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.column_user_16, "column_user_16", typeof(Dataedo.App.Properties.Resources), 197);
		this.treeMenuImageCollection.Images.SetKeyName(197, "column_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_1x_1x_16, "relation_1x_1x_16", typeof(Dataedo.App.Properties.Resources), 198);
		this.treeMenuImageCollection.Images.SetKeyName(198, "relation_1x_1x_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_1x_1x_deleted_16, "relation_1x_1x_deleted_16", typeof(Dataedo.App.Properties.Resources), 199);
		this.treeMenuImageCollection.Images.SetKeyName(199, "relation_1x_1x_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_1x_1x_user_16, "relation_1x_1x_user_16", typeof(Dataedo.App.Properties.Resources), 200);
		this.treeMenuImageCollection.Images.SetKeyName(200, "relation_1x_1x_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_deleted_16, "relation_mx_1x_deleted_16", typeof(Dataedo.App.Properties.Resources), 201, "relation_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(201, "relation_mx_1x_deleted_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_mx_1x_user_16, "relation_mx_1x_user_16", typeof(Dataedo.App.Properties.Resources), 202);
		this.treeMenuImageCollection.Images.SetKeyName(202, "relation_mx_1x_user_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.business_glossary_16, "business_glossary", typeof(Dataedo.App.Properties.Resources), 203, "business_glossary_16");
		this.treeMenuImageCollection.Images.SetKeyName(203, "business_glossary");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.term_16, "term", typeof(Dataedo.App.Properties.Resources), 204, "term_16");
		this.treeMenuImageCollection.Images.SetKeyName(204, "term");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.category_16, "category_16", typeof(Dataedo.App.Properties.Resources), 205);
		this.treeMenuImageCollection.Images.SetKeyName(205, "category_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.rule_16, "rule_16", typeof(Dataedo.App.Properties.Resources), 206);
		this.treeMenuImageCollection.Images.SetKeyName(206, "rule_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.policy_16, "policy_16", typeof(Dataedo.App.Properties.Resources), 207);
		this.treeMenuImageCollection.Images.SetKeyName(207, "policy_16");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.collection_deleted_16, "collection_deleted", typeof(Dataedo.App.Properties.Resources), 208, "collection_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(208, "collection_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.column_deleted_16, "column_deleted", typeof(Dataedo.App.Properties.Resources), 209, "column_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(209, "column_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cube_deleted_16, "cube_deleted", typeof(Dataedo.App.Properties.Resources), 210, "cube_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(210, "cube_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.dimension_deleted_16, "dimension_deleted", typeof(Dataedo.App.Properties.Resources), 211, "dimension_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(211, "dimension_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.editioning_view_deleted_16, "editioning_view_deleted", typeof(Dataedo.App.Properties.Resources), 212, "editioning_view_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(212, "editioning_view_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.entity_deleted_16, "entity_deleted", typeof(Dataedo.App.Properties.Resources), 213, "entity_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(213, "entity_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_object_deleted_16, "external_object_deleted", typeof(Dataedo.App.Properties.Resources), 214, "external_object_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(214, "external_object_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.external_table_deleted_16, "external_table_deleted", typeof(Dataedo.App.Properties.Resources), 215, "external_table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(215, "external_table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.file_table_deleted_16, "file_table_deleted", typeof(Dataedo.App.Properties.Resources), 216, "file_table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(216, "file_table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.flat_file_deleted_16, "flat_file_deleted", typeof(Dataedo.App.Properties.Resources), 217, "flat_file_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(217, "flat_file_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.foreign_table_deleted_16, "foreign_table_deleted", typeof(Dataedo.App.Properties.Resources), 218, "foreign_table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(218, "foreign_table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.function_deleted_16, "function_deleted", typeof(Dataedo.App.Properties.Resources), 219, "function_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(219, "function_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_edge_table_deleted_16, "graph_edge_table_deleted", typeof(Dataedo.App.Properties.Resources), 220, "graph_edge_table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(220, "graph_edge_table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.graph_table_deleted_16, "graph_table_deleted", typeof(Dataedo.App.Properties.Resources), 221, "graph_table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(221, "graph_table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.history_table_deleted_16, "history_table_deleted", typeof(Dataedo.App.Properties.Resources), 222, "history_table_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(222, "history_table_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.indexed_view_deleted_16, "indexed_view_deleted", typeof(Dataedo.App.Properties.Resources), 223, "indexed_view_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(223, "indexed_view_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.materialized_view_deleted_16, "materialized_view_deleted", typeof(Dataedo.App.Properties.Resources), 224, "materialized_view_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(224, "materialized_view_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.object_deleted_16, "object_deleted", typeof(Dataedo.App.Properties.Resources), 225, "object_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(225, "object_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.package_deleted_16, "package_deleted", typeof(Dataedo.App.Properties.Resources), 226, "package_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(226, "package_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_deleted_16, "primary_key_deleted", typeof(Dataedo.App.Properties.Resources), 227, "primary_key_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(227, "primary_key_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.procedure_deleted_16, "procedure_deleted", typeof(Dataedo.App.Properties.Resources), 228, "procedure_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(228, "procedure_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_delete_16, "relation_delete", typeof(Dataedo.App.Properties.Resources), 229, "relation_delete_16");
		this.treeMenuImageCollection.Images.SetKeyName(229, "relation_delete");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_deleted_16, "unique_key_deleted", typeof(Dataedo.App.Properties.Resources), 230, "unique_key_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(230, "unique_key_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.column_user_16, "column_user", typeof(Dataedo.App.Properties.Resources), 231, "column_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(231, "column_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_1x_1x_16, "relation_1x_1x", typeof(Dataedo.App.Properties.Resources), 232, "relation_1x_1x_16");
		this.treeMenuImageCollection.Images.SetKeyName(232, "relation_1x_1x");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_1x_1x_deleted_16, "relation_1x_1x_deleted", typeof(Dataedo.App.Properties.Resources), 233, "relation_1x_1x_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(233, "relation_1x_1x_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_1x_1x_user_16, "relation_1x_1x_user", typeof(Dataedo.App.Properties.Resources), 234, "relation_1x_1x_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(234, "relation_1x_1x_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_deleted_16, "relation_mx_1x_deleted", typeof(Dataedo.App.Properties.Resources), 235, "relation_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(235, "relation_mx_1x_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.relation_mx_1x_user_16, "relation_mx_1x_user", typeof(Dataedo.App.Properties.Resources), 236, "relation_mx_1x_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(236, "relation_mx_1x_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.clr_function_16, "clr_function", typeof(Dataedo.App.Properties.Resources), 237, "clr_function_16");
		this.treeMenuImageCollection.Images.SetKeyName(237, "clr_function");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.clr_procedure_16, "clr_procedure", typeof(Dataedo.App.Properties.Resources), 238, "clr_procedure_16");
		this.treeMenuImageCollection.Images.SetKeyName(238, "clr_procedure");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.extended_procedure_16, "extended_procedure", typeof(Dataedo.App.Properties.Resources), 239, "extended_procedure_16");
		this.treeMenuImageCollection.Images.SetKeyName(239, "extended_procedure");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.clr_function_deleted_16, "clr_function_deleted", typeof(Dataedo.App.Properties.Resources), 240, "clr_function_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(240, "clr_function_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.clr_function_new_16, "clr_function_new", typeof(Dataedo.App.Properties.Resources), 241, "clr_function_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(241, "clr_function_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.clr_function_updated_16, "clr_function_updated", typeof(Dataedo.App.Properties.Resources), 242, "clr_function_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(242, "clr_function_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.clr_function_user_16, "clr_function_user", typeof(Dataedo.App.Properties.Resources), 243, "clr_function_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(243, "clr_function_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.clr_procedure_deleted_16, "clr_procedure_deleted", typeof(Dataedo.App.Properties.Resources), 244, "clr_procedure_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(244, "clr_procedure_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.clr_procedure_new_16, "clr_procedure_new", typeof(Dataedo.App.Properties.Resources), 245, "clr_procedure_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(245, "clr_procedure_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.clr_procedure_updated_16, "clr_procedure_updated", typeof(Dataedo.App.Properties.Resources), 246, "clr_procedure_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(246, "clr_procedure_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.clr_procedure_user_16, "clr_procedure_user", typeof(Dataedo.App.Properties.Resources), 247, "clr_procedure_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(247, "clr_procedure_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.extended_procedure_deleted_16, "extended_procedure_deleted", typeof(Dataedo.App.Properties.Resources), 248, "extended_procedure_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(248, "extended_procedure_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.extended_procedure_new_16, "extended_procedure_new", typeof(Dataedo.App.Properties.Resources), 249, "extended_procedure_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(249, "extended_procedure_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.extended_procedure_updated_16, "extended_procedure_updated", typeof(Dataedo.App.Properties.Resources), 250, "extended_procedure_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(250, "extended_procedure_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.extended_procedure_user_16, "extended_procedure_user", typeof(Dataedo.App.Properties.Resources), 251, "extended_procedure_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(251, "extended_procedure_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.structure_16, "structure", typeof(Dataedo.App.Properties.Resources), 252, "structure_16");
		this.treeMenuImageCollection.Images.SetKeyName(252, "structure");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.structure_deleted_16, "structure_deleted", typeof(Dataedo.App.Properties.Resources), 253, "structure_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(253, "structure_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.structure_new_16, "structure_new", typeof(Dataedo.App.Properties.Resources), 254, "structure_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(254, "structure_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.structure_updated_16, "structure_updated", typeof(Dataedo.App.Properties.Resources), 255, "structure_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(255, "structure_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.structure_user_16, "structure_user", typeof(Dataedo.App.Properties.Resources), 256, "structure_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(256, "structure_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.avro_record_16, "avro_record", typeof(Dataedo.App.Properties.Resources), 257, "avro_record_16");
		this.treeMenuImageCollection.Images.SetKeyName(257, "avro_record");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.avro_record_deleted_16, "avro_record_deleted", typeof(Dataedo.App.Properties.Resources), 258, "avro_record_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(258, "avro_record_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.avro_record_new_16, "avro_record_new", typeof(Dataedo.App.Properties.Resources), 259, "avro_record_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(259, "avro_record_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.avro_record_updated_16, "avro_record_updated", typeof(Dataedo.App.Properties.Resources), 260, "avro_record_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(260, "avro_record_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.avro_record_user_16, "avro_record_user", typeof(Dataedo.App.Properties.Resources), 261, "avro_record_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(261, "avro_record_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cdm_16, "cdm", typeof(Dataedo.App.Properties.Resources), 262, "cdm_16");
		this.treeMenuImageCollection.Images.SetKeyName(262, "cdm");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cdm_deleted_16, "cdm_deleted", typeof(Dataedo.App.Properties.Resources), 263, "cdm_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(263, "cdm_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cdm_new_16, "cdm_new", typeof(Dataedo.App.Properties.Resources), 264, "cdm_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(264, "cdm_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cdm_updated_16, "cdm_updated", typeof(Dataedo.App.Properties.Resources), 265, "cdm_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(265, "cdm_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.cdm_user_16, "cdm_user", typeof(Dataedo.App.Properties.Resources), 266, "cdm_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(266, "cdm_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.csv_16, "csv", typeof(Dataedo.App.Properties.Resources), 267, "csv_16");
		this.treeMenuImageCollection.Images.SetKeyName(267, "csv");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.csv_deleted_16, "csv_deleted", typeof(Dataedo.App.Properties.Resources), 268, "csv_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(268, "csv_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.csv_new_16, "csv_new", typeof(Dataedo.App.Properties.Resources), 269, "csv_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(269, "csv_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.csv_updated_16, "csv_updated", typeof(Dataedo.App.Properties.Resources), 270, "csv_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(270, "csv_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.csv_user_16, "csv_user", typeof(Dataedo.App.Properties.Resources), 271, "csv_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(271, "csv_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.delimited_text_16, "delimited_text", typeof(Dataedo.App.Properties.Resources), 272, "delimited_text_16");
		this.treeMenuImageCollection.Images.SetKeyName(272, "delimited_text");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.delimited_text_deleted_16, "delimited_text_deleted", typeof(Dataedo.App.Properties.Resources), 273, "delimited_text_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(273, "delimited_text_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.delimited_text_new_16, "delimited_text_new", typeof(Dataedo.App.Properties.Resources), 274, "delimited_text_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(274, "delimited_text_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.delimited_text_updated_16, "delimited_text_updated", typeof(Dataedo.App.Properties.Resources), 275, "delimited_text_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(275, "delimited_text_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.delimited_text_user_16, "delimited_text_user", typeof(Dataedo.App.Properties.Resources), 276, "delimited_text_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(276, "delimited_text_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.directory_16, "directory", typeof(Dataedo.App.Properties.Resources), 277, "directory_16");
		this.treeMenuImageCollection.Images.SetKeyName(277, "directory");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.directory_deleted_16, "directory_deleted", typeof(Dataedo.App.Properties.Resources), 278, "directory_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(278, "directory_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.directory_new_16, "directory_new", typeof(Dataedo.App.Properties.Resources), 279, "directory_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(279, "directory_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.directory_updated_16, "directory_updated", typeof(Dataedo.App.Properties.Resources), 280, "directory_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(280, "directory_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.directory_user_16, "directory_user", typeof(Dataedo.App.Properties.Resources), 281, "directory_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(281, "directory_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.json_16, "json", typeof(Dataedo.App.Properties.Resources), 282, "json_16");
		this.treeMenuImageCollection.Images.SetKeyName(282, "json");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.json_deleted_16, "json_deleted", typeof(Dataedo.App.Properties.Resources), 283, "json_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(283, "json_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.json_new_16, "json_new", typeof(Dataedo.App.Properties.Resources), 284, "json_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(284, "json_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.json_updated_16, "json_updated", typeof(Dataedo.App.Properties.Resources), 285, "json_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(285, "json_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.json_user_16, "json_user", typeof(Dataedo.App.Properties.Resources), 286, "json_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(286, "json_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.orc_16, "orc", typeof(Dataedo.App.Properties.Resources), 287, "orc_16");
		this.treeMenuImageCollection.Images.SetKeyName(287, "orc");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.orc_deleted_16, "orc_deleted", typeof(Dataedo.App.Properties.Resources), 288, "orc_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(288, "orc_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.orc_new_16, "orc_new", typeof(Dataedo.App.Properties.Resources), 289, "orc_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(289, "orc_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.orc_updated_16, "orc_updated", typeof(Dataedo.App.Properties.Resources), 290, "orc_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(290, "orc_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.orc_user_16, "orc_user", typeof(Dataedo.App.Properties.Resources), 291, "orc_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(291, "orc_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parquet_16, "parquet", typeof(Dataedo.App.Properties.Resources), 292, "parquet_16");
		this.treeMenuImageCollection.Images.SetKeyName(292, "parquet");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parquet_deleted_16, "parquet_deleted", typeof(Dataedo.App.Properties.Resources), 293, "parquet_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(293, "parquet_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parquet_new_16, "parquet_new", typeof(Dataedo.App.Properties.Resources), 294, "parquet_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(294, "parquet_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parquet_updated_16, "parquet_updated", typeof(Dataedo.App.Properties.Resources), 295, "parquet_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(295, "parquet_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.parquet_user_16, "parquet_user", typeof(Dataedo.App.Properties.Resources), 296, "parquet_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(296, "parquet_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.xml_16, "xml", typeof(Dataedo.App.Properties.Resources), 297, "xml_16");
		this.treeMenuImageCollection.Images.SetKeyName(297, "xml");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.xml_deleted_16, "xml_deleted", typeof(Dataedo.App.Properties.Resources), 298, "xml_deleted_16");
		this.treeMenuImageCollection.Images.SetKeyName(298, "xml_deleted");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.xml_new_16, "xml_new", typeof(Dataedo.App.Properties.Resources), 299, "xml_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(299, "xml_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.xml_updated_16, "xml_updated", typeof(Dataedo.App.Properties.Resources), 300, "xml_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(300, "xml_updated");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.xml_user_16, "xml_user", typeof(Dataedo.App.Properties.Resources), 301, "xml_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(301, "xml_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.server_repository_16, "repository", typeof(Dataedo.App.Properties.Resources), 302, "server_repository_16");
		this.treeMenuImageCollection.Images.SetKeyName(302, "repository");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.file_repository_16, "file_repository", typeof(Dataedo.App.Properties.Resources), 303, "file_repository_16");
		this.treeMenuImageCollection.Images.SetKeyName(303, "file_repository");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.delta_lake_16, "delta_lake", typeof(Dataedo.App.Properties.Resources), 304, "delta_lake_16");
		this.treeMenuImageCollection.Images.SetKeyName(304, "delta_lake");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.delta_lake_user_16, "delta_lake_user", typeof(Dataedo.App.Properties.Resources), 305, "delta_lake_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(305, "delta_lake_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.delta_lake_new_16, "delta_lake_new", typeof(Dataedo.App.Properties.Resources), 306, "delta_lake_new_16");
		this.treeMenuImageCollection.Images.SetKeyName(306, "delta_lake_new");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.delta_lake_updated_16, "delta_lake_updated", typeof(Dataedo.App.Properties.Resources), 307, "delta_lake_updated_16");
		this.treeMenuImageCollection.Images.SetKeyName(307, "delta_lake_updated");
		this.treeMenuImageCollection.Images.SetKeyName(309, "excel_table");
		this.treeMenuImageCollection.Images.SetKeyName(310, "excel_table_deleted");
		this.treeMenuImageCollection.Images.SetKeyName(311, "excel_table_updated");
		this.treeMenuImageCollection.Images.SetKeyName(312, "excel_table_new");
		this.treeMenuImageCollection.Images.SetKeyName(313, "excel_table_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.program_16, "program", typeof(Dataedo.App.Properties.Resources), 314, "program_16");
		this.treeMenuImageCollection.Images.SetKeyName(314, "program");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.program_user_16, "program_user", typeof(Dataedo.App.Properties.Resources), 315, "program_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(315, "program_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.form_16, "form", typeof(Dataedo.App.Properties.Resources), 316, "form_16");
		this.treeMenuImageCollection.Images.SetKeyName(316, "form");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.form_user_16, "form_user", typeof(Dataedo.App.Properties.Resources), 317, "form_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(317, "form_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.report_16, "report", typeof(Dataedo.App.Properties.Resources), 318, "report_16");
		this.treeMenuImageCollection.Images.SetKeyName(318, "report");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.report_user_16, "report_user", typeof(Dataedo.App.Properties.Resources), 319, "report_user_16");
		this.treeMenuImageCollection.Images.SetKeyName(319, "report_user");
		this.treeMenuImageCollection.InsertImage(Dataedo.App.Properties.Resources.sort_asc_16, "sort_asc", typeof(Dataedo.App.Properties.Resources), 320, "sort_asc_16");
		this.treeMenuImageCollection.Images.SetKeyName(320, "sort_asc");
		this.bannerControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.bannerControl.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.bannerControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.bannerControl.Location = new System.Drawing.Point(0, 786);
		this.bannerControl.Name = "bannerControl";
		this.bannerControl.Size = new System.Drawing.Size(395, 135);
		this.bannerControl.TabIndex = 1;
		this.bannerControl.Visible = false;
		this.searchXtraTabPage.Controls.Add(this.searchLayoutControl);
		this.searchXtraTabPage.Name = "searchXtraTabPage";
		this.searchXtraTabPage.ShowCloseButton = DevExpress.Utils.DefaultBoolean.True;
		this.searchXtraTabPage.Size = new System.Drawing.Size(395, 921);
		this.searchXtraTabPage.Text = "Search";
		this.searchLayoutControl.AllowCustomization = false;
		this.searchLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.searchLayoutControl.Controls.Add(this.searchTextEdit);
		this.searchLayoutControl.Controls.Add(this.advancedSearchSplitContainerControl);
		this.searchLayoutControl.Controls.Add(this.DocumentationsCheckedComboBoxEdit);
		this.searchLayoutControl.Controls.Add(this.TypesCheckedComboBoxEdit);
		this.searchLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.searchLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.searchLayoutControl.Name = "searchLayoutControl";
		this.searchLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2397, 172, 250, 350);
		this.searchLayoutControl.Root = this.layoutControlGroup1;
		this.searchLayoutControl.Size = new System.Drawing.Size(395, 921);
		this.searchLayoutControl.TabIndex = 2;
		this.searchLayoutControl.Text = "layoutControl1";
		this.searchTextEdit.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.searchTextEdit.EditValue = "";
		this.searchTextEdit.Location = new System.Drawing.Point(107, 50);
		this.searchTextEdit.MenuManager = this.treelistBarManager;
		this.searchTextEdit.Name = "searchTextEdit";
		this.searchTextEdit.Properties.NullValuePrompt = "Enter text to search...";
		this.searchTextEdit.Size = new System.Drawing.Size(286, 20);
		this.searchTextEdit.StyleController = this.searchLayoutControl;
		this.searchTextEdit.TabIndex = 2;
		this.searchTextEdit.KeyPress += new System.Windows.Forms.KeyPressEventHandler(searchTextEdit_KeyPress);
		this.searchTextEdit.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(searchControl_PreviewKeyDown);
		this.advancedSearchSplitContainerControl.Horizontal = false;
		this.advancedSearchSplitContainerControl.Location = new System.Drawing.Point(2, 74);
		this.advancedSearchSplitContainerControl.Margin = new System.Windows.Forms.Padding(0);
		this.advancedSearchSplitContainerControl.Name = "advancedSearchSplitContainerControl";
		this.advancedSearchSplitContainerControl.Panel1.Controls.Add(this.advancedSearchPanel);
		this.advancedSearchSplitContainerControl.Panel1.MinSize = 28;
		this.advancedSearchSplitContainerControl.Panel1.Text = "Panel1";
		this.advancedSearchSplitContainerControl.Panel2.Controls.Add(this.searchesXtraTabControl);
		this.advancedSearchSplitContainerControl.Panel2.MinSize = 150;
		this.advancedSearchSplitContainerControl.Panel2.Text = "Panel2";
		this.advancedSearchSplitContainerControl.Size = new System.Drawing.Size(391, 845);
		this.advancedSearchSplitContainerControl.SplitterPosition = 28;
		this.advancedSearchSplitContainerControl.TabIndex = 0;
		this.advancedSearchSplitContainerControl.Text = "splitContainerControl1";
		this.advancedSearchPanel.BackColor = System.Drawing.Color.Transparent;
		this.advancedSearchPanel.Dock = System.Windows.Forms.DockStyle.Fill;
		this.advancedSearchPanel.Location = new System.Drawing.Point(0, 0);
		this.advancedSearchPanel.Name = "advancedSearchPanel";
		this.advancedSearchPanel.Size = new System.Drawing.Size(391, 28);
		this.advancedSearchPanel.TabIndex = 3;
		this.advancedSearchPanel.FieldPreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(searchControl_PreviewKeyDown);
		this.advancedSearchPanel.FieldKeyPress += new System.Windows.Forms.KeyPressEventHandler(searchTextEdit_KeyPress);
		this.searchesXtraTabControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.searchesXtraTabControl.BorderStylePage = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.searchesXtraTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.searchesXtraTabControl.Location = new System.Drawing.Point(0, 0);
		this.searchesXtraTabControl.LookAndFeel.SkinName = "The Bezier Common Customized";
		this.searchesXtraTabControl.LookAndFeel.UseDefaultLookAndFeel = false;
		this.searchesXtraTabControl.Margin = new System.Windows.Forms.Padding(0);
		this.searchesXtraTabControl.Name = "searchesXtraTabControl";
		this.searchesXtraTabControl.SelectedTabPage = this.searchResultsGridViewXtraTabPage;
		this.searchesXtraTabControl.Size = new System.Drawing.Size(391, 807);
		this.searchesXtraTabControl.TabIndex = 3;
		this.searchesXtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.searchResultsGridViewXtraTabPage, this.searchResultsTreeListXtraTabPage });
		this.searchesXtraTabControl.KeyDown += new System.Windows.Forms.KeyEventHandler(searchesXtraTabControl_KeyDown);
		this.searchResultsGridViewXtraTabPage.Controls.Add(this.searchResultsListGridControl);
		this.searchResultsGridViewXtraTabPage.ImageOptions.Image = Dataedo.App.Properties.Resources.search_list_16;
		this.searchResultsGridViewXtraTabPage.Margin = new System.Windows.Forms.Padding(0);
		this.searchResultsGridViewXtraTabPage.Name = "searchResultsGridViewXtraTabPage";
		this.searchResultsGridViewXtraTabPage.Size = new System.Drawing.Size(389, 775);
		this.searchResultsListGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.searchResultsListGridControl.Location = new System.Drawing.Point(0, 0);
		this.searchResultsListGridControl.MainView = this.searchResultsListGridView;
		this.searchResultsListGridControl.Margin = new System.Windows.Forms.Padding(0);
		this.searchResultsListGridControl.MenuManager = this.treelistBarManager;
		this.searchResultsListGridControl.Name = "searchResultsListGridControl";
		this.searchResultsListGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemPictureEdit1, this.SearchResultsListNameRepositoryItemRichTextEdit, this.SearchResultsListNTitleRepositoryItemRichTextEdit });
		this.searchResultsListGridControl.Size = new System.Drawing.Size(389, 775);
		this.searchResultsListGridControl.TabIndex = 0;
		this.searchResultsListGridControl.ToolTipController = this.searchResultsListToolTipController;
		this.searchResultsListGridControl.UseDirectXPaint = DevExpress.Utils.DefaultBoolean.False;
		this.searchResultsListGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.searchResultsListGridView, this.InnerGrid });
		this.searchResultsListGridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.searchResultsListGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.SearchResultsListIconGridColumn, this.SearchResultsListNameGridColumn, this.SearchResultsListDocumentationNameGridColumn });
		this.searchResultsListGridView.GridControl = this.searchResultsListGridControl;
		this.searchResultsListGridView.Name = "searchResultsListGridView";
		this.searchResultsListGridView.OptionsCustomization.AllowFilter = false;
		this.searchResultsListGridView.OptionsDetail.EnableMasterViewMode = false;
		this.searchResultsListGridView.OptionsDetail.ShowDetailTabs = false;
		this.searchResultsListGridView.OptionsView.ColumnAutoWidth = false;
		this.searchResultsListGridView.OptionsView.ShowGroupPanel = false;
		this.searchResultsListGridView.OptionsView.ShowIndicator = false;
		this.searchResultsListGridView.RowHighlightingIsEnabled = true;
		this.searchResultsListGridView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.LiveVertScroll;
		this.searchResultsListGridView.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(searchResultsListGridView_RowClick);
		this.searchResultsListGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(searchResultsListGridView_PopupMenuShowing);
		this.searchResultsListGridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(SearchResultsListGridView_FocusedRowChanged);
		this.searchResultsListGridView.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(searchResultsListGridView_BeforeLeaveRow);
		this.searchResultsListGridView.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(searchResultsListGridView_CustomColumnSort);
		this.searchResultsListGridView.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(SearchResultsListGridView_CustomUnboundColumnData);
		this.searchResultsListGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(searchResultsListGridView_KeyDown);
		this.SearchResultsListIconGridColumn.Caption = " ";
		this.SearchResultsListIconGridColumn.ColumnEdit = this.repositoryItemPictureEdit1;
		this.SearchResultsListIconGridColumn.FieldName = "SearchResultsListIcon";
		this.SearchResultsListIconGridColumn.MaxWidth = 26;
		this.SearchResultsListIconGridColumn.MinWidth = 26;
		this.SearchResultsListIconGridColumn.Name = "SearchResultsListIconGridColumn";
		this.SearchResultsListIconGridColumn.OptionsColumn.AllowEdit = false;
		this.SearchResultsListIconGridColumn.OptionsColumn.AllowShowHide = false;
		this.SearchResultsListIconGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.SearchResultsListIconGridColumn.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.SearchResultsListIconGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.SearchResultsListIconGridColumn.Visible = true;
		this.SearchResultsListIconGridColumn.VisibleIndex = 0;
		this.SearchResultsListIconGridColumn.Width = 26;
		this.repositoryItemPictureEdit1.AllowFocused = false;
		this.repositoryItemPictureEdit1.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryItemPictureEdit1.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryItemPictureEdit1.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
		this.repositoryItemPictureEdit1.ShowMenu = false;
		this.SearchResultsListNameGridColumn.Caption = "Name";
		this.SearchResultsListNameGridColumn.ColumnEdit = this.SearchResultsListNameRepositoryItemRichTextEdit;
		this.SearchResultsListNameGridColumn.FieldName = "NameWithTitle";
		this.SearchResultsListNameGridColumn.Name = "SearchResultsListNameGridColumn";
		this.SearchResultsListNameGridColumn.OptionsColumn.AllowEdit = false;
		this.SearchResultsListNameGridColumn.OptionsColumn.AllowShowHide = false;
		this.SearchResultsListNameGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.SearchResultsListNameGridColumn.Visible = true;
		this.SearchResultsListNameGridColumn.VisibleIndex = 1;
		this.SearchResultsListNameGridColumn.Width = 50;
		this.SearchResultsListNameRepositoryItemRichTextEdit.DocumentFormat = DevExpress.XtraRichEdit.DocumentFormat.Html;
		this.SearchResultsListNameRepositoryItemRichTextEdit.EncodingWebName = "utf-8";
		this.SearchResultsListNameRepositoryItemRichTextEdit.Name = "SearchResultsListNameRepositoryItemRichTextEdit";
		this.SearchResultsListNameRepositoryItemRichTextEdit.ShowCaretInReadOnly = false;
		this.SearchResultsListDocumentationNameGridColumn.Caption = "Documentation";
		this.SearchResultsListDocumentationNameGridColumn.FieldName = "DocumentationTitleNoHighlight";
		this.SearchResultsListDocumentationNameGridColumn.Name = "SearchResultsListDocumentationNameGridColumn";
		this.SearchResultsListDocumentationNameGridColumn.OptionsColumn.AllowEdit = false;
		this.SearchResultsListDocumentationNameGridColumn.OptionsColumn.AllowShowHide = false;
		this.SearchResultsListDocumentationNameGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.SearchResultsListDocumentationNameGridColumn.Visible = true;
		this.SearchResultsListDocumentationNameGridColumn.VisibleIndex = 2;
		this.SearchResultsListDocumentationNameGridColumn.Width = 100;
		this.SearchResultsListNTitleRepositoryItemRichTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.SearchResultsListNTitleRepositoryItemRichTextEdit.DocumentFormat = DevExpress.XtraRichEdit.DocumentFormat.Html;
		this.SearchResultsListNTitleRepositoryItemRichTextEdit.Name = "SearchResultsListNTitleRepositoryItemRichTextEdit";
		this.SearchResultsListNTitleRepositoryItemRichTextEdit.ShowCaretInReadOnly = false;
		this.searchResultsListToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.searchResultsListToolTipController.GetActiveObjectInfo += new DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventHandler(searchResultsListToolTipController_GetActiveObjectInfo);
		this.InnerGrid.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.gridColumn1 });
		this.InnerGrid.GridControl = this.searchResultsListGridControl;
		this.InnerGrid.Name = "InnerGrid";
		this.InnerGrid.OptionsView.ShowGroupPanel = false;
		this.InnerGrid.RowHighlightingIsEnabled = true;
		this.gridColumn1.Caption = "gridColumn1";
		this.gridColumn1.FieldName = "NameWithTitle";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.searchResultsTreeListXtraTabPage.Controls.Add(this.searchResultsTreeList);
		this.searchResultsTreeListXtraTabPage.ImageOptions.Image = Dataedo.App.Properties.Resources.search_tree_16;
		this.searchResultsTreeListXtraTabPage.Margin = new System.Windows.Forms.Padding(0);
		this.searchResultsTreeListXtraTabPage.Name = "searchResultsTreeListXtraTabPage";
		this.searchResultsTreeListXtraTabPage.Size = new System.Drawing.Size(389, 775);
		this.searchResultsTreeList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.searchResultsTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[1] { this.NameSearchResultsTreeListColumn });
		this.searchResultsTreeList.Dock = System.Windows.Forms.DockStyle.Fill;
		this.searchResultsTreeList.KeyFieldName = "Id";
		this.searchResultsTreeList.Location = new System.Drawing.Point(0, 0);
		this.searchResultsTreeList.Margin = new System.Windows.Forms.Padding(0);
		this.searchResultsTreeList.Name = "searchResultsTreeList";
		this.searchResultsTreeList.OptionsBehavior.AllowExpandAnimation = DevExpress.Utils.DefaultBoolean.True;
		this.searchResultsTreeList.OptionsBehavior.Editable = false;
		this.searchResultsTreeList.OptionsView.BestFitNodes = DevExpress.XtraTreeList.TreeListBestFitNodes.Visible;
		this.searchResultsTreeList.OptionsView.ShowColumns = false;
		this.searchResultsTreeList.OptionsView.ShowHorzLines = false;
		this.searchResultsTreeList.OptionsView.ShowIndicator = false;
		this.searchResultsTreeList.OptionsView.ShowVertLines = false;
		this.searchResultsTreeList.ParentFieldName = "ParentId";
		this.searchResultsTreeList.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.searchResultsTreeListNameRepositoryItemRichTextEdit });
		this.searchResultsTreeList.SelectImageList = this.treeMenuImageCollection;
		this.searchResultsTreeList.Size = new System.Drawing.Size(389, 775);
		this.searchResultsTreeList.TabIndex = 0;
		this.searchResultsTreeList.ToolTipController = this.searchResultsListToolTipController;
		this.searchResultsTreeList.UseDirectXPaint = DevExpress.Utils.DefaultBoolean.False;
		this.searchResultsTreeList.GetSelectImage += new DevExpress.XtraTreeList.GetSelectImageEventHandler(SearchResultsTreeList_GetSelectImage);
		this.searchResultsTreeList.BeforeFocusNode += new DevExpress.XtraTreeList.BeforeFocusNodeEventHandler(searchResultsTreeList_BeforeFocusNode);
		this.searchResultsTreeList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(SearchResultsTreeList_FocusedNodeChanged);
		this.searchResultsTreeList.PopupMenuShowing += new DevExpress.XtraTreeList.PopupMenuShowingEventHandler(metadataTreeList_PopupMenuShowing);
		this.searchResultsTreeList.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(searchResultsTreeList_PreviewKeyDown);
		this.NameSearchResultsTreeListColumn.Caption = " ";
		this.NameSearchResultsTreeListColumn.ColumnEdit = this.searchResultsTreeListNameRepositoryItemRichTextEdit;
		this.NameSearchResultsTreeListColumn.FieldName = "NameWithTitle";
		this.NameSearchResultsTreeListColumn.MinWidth = 33;
		this.NameSearchResultsTreeListColumn.Name = "NameSearchResultsTreeListColumn";
		this.NameSearchResultsTreeListColumn.OptionsColumn.AllowEdit = false;
		this.NameSearchResultsTreeListColumn.OptionsColumn.AllowSort = false;
		this.NameSearchResultsTreeListColumn.Visible = true;
		this.NameSearchResultsTreeListColumn.VisibleIndex = 0;
		this.NameSearchResultsTreeListColumn.Width = 33;
		this.searchResultsTreeListNameRepositoryItemRichTextEdit.Appearance.Options.UseTextOptions = true;
		this.searchResultsTreeListNameRepositoryItemRichTextEdit.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.NoWrap;
		this.searchResultsTreeListNameRepositoryItemRichTextEdit.DocumentFormat = DevExpress.XtraRichEdit.DocumentFormat.Html;
		this.searchResultsTreeListNameRepositoryItemRichTextEdit.EncodingWebName = "utf-8";
		this.searchResultsTreeListNameRepositoryItemRichTextEdit.Name = "searchResultsTreeListNameRepositoryItemRichTextEdit";
		this.searchResultsTreeListNameRepositoryItemRichTextEdit.OptionsVerticalScrollbar.Visibility = DevExpress.XtraRichEdit.RichEditScrollbarVisibility.Hidden;
		this.searchResultsTreeListNameRepositoryItemRichTextEdit.ReadOnly = true;
		this.searchResultsTreeListNameRepositoryItemRichTextEdit.ShowCaretInReadOnly = false;
		this.DocumentationsCheckedComboBoxEdit.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.DocumentationsCheckedComboBoxEdit.Location = new System.Drawing.Point(107, 2);
		this.DocumentationsCheckedComboBoxEdit.MenuManager = this.treelistBarManager;
		this.DocumentationsCheckedComboBoxEdit.Name = "DocumentationsCheckedComboBoxEdit";
		this.DocumentationsCheckedComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.DocumentationsCheckedComboBoxEdit.Properties.DropDownRows = 13;
		this.DocumentationsCheckedComboBoxEdit.Size = new System.Drawing.Size(286, 20);
		this.DocumentationsCheckedComboBoxEdit.StyleController = this.searchLayoutControl;
		this.DocumentationsCheckedComboBoxEdit.TabIndex = 0;
		this.DocumentationsCheckedComboBoxEdit.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(DocumentationsCheckedComboBoxEdit_CustomDisplayText);
		this.DocumentationsCheckedComboBoxEdit.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(DocumentationsCheckedComboBoxEdit_PreviewKeyDown);
		this.TypesCheckedComboBoxEdit.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.TypesCheckedComboBoxEdit.EditValue = "DOCUMENTATION, MODULE, TABLE, VIEW, PROCEDURE, FUNCTION, STRUCTURE, BUSINESS_GLOSSARY";
		this.TypesCheckedComboBoxEdit.Location = new System.Drawing.Point(107, 26);
		this.TypesCheckedComboBoxEdit.MenuManager = this.treelistBarManager;
		this.TypesCheckedComboBoxEdit.Name = "TypesCheckedComboBoxEdit";
		this.TypesCheckedComboBoxEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.TypesCheckedComboBoxEdit.Properties.DropDownRows = 9;
		this.TypesCheckedComboBoxEdit.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.CheckedListBoxItem[8]
		{
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem("DOCUMENTATION", "Documentations descriptions", System.Windows.Forms.CheckState.Checked),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem("MODULE", "Subject Areas descriptions", System.Windows.Forms.CheckState.Checked),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem("TABLE", "Tables", System.Windows.Forms.CheckState.Checked),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem("VIEW", "Views", System.Windows.Forms.CheckState.Checked),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem("PROCEDURE", "Procedures", System.Windows.Forms.CheckState.Checked),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem("FUNCTION", "Functions", System.Windows.Forms.CheckState.Checked),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem("STRUCTURE", "Structures", System.Windows.Forms.CheckState.Checked),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem("BUSINESS_GLOSSARY", "Business Glossary", System.Windows.Forms.CheckState.Checked)
		});
		this.TypesCheckedComboBoxEdit.Size = new System.Drawing.Size(286, 20);
		this.TypesCheckedComboBoxEdit.StyleController = this.searchLayoutControl;
		this.TypesCheckedComboBoxEdit.TabIndex = 1;
		this.TypesCheckedComboBoxEdit.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(TypesCheckedComboBoxEdit_CustomDisplayText);
		this.TypesCheckedComboBoxEdit.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(TypesCheckedComboBoxEdit_PreviewKeyDown);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[4] { this.layoutControlItem1, this.layoutControlItem2, this.layoutControlItem4, this.layoutControlItem5 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(395, 921);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.Control = this.DocumentationsCheckedComboBoxEdit;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 24);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(54, 24);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(395, 24);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.Text = "Documentation";
		this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(100, 0);
		this.layoutControlItem1.TextToControlDistance = 5;
		this.layoutControlItem2.Control = this.TypesCheckedComboBoxEdit;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(0, 24);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(54, 24);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(395, 24);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.Text = "Type";
		this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(100, 0);
		this.layoutControlItem2.TextToControlDistance = 5;
		this.layoutControlItem4.Control = this.advancedSearchSplitContainerControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 72);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(395, 849);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem5.Control = this.searchTextEdit;
		this.layoutControlItem5.Location = new System.Drawing.Point(0, 48);
		this.layoutControlItem5.MaxSize = new System.Drawing.Size(0, 24);
		this.layoutControlItem5.MinSize = new System.Drawing.Size(159, 24);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(395, 24);
		this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem5.Text = "Search";
		this.layoutControlItem5.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem5.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem5.TextToControlDistance = 5;
		this.deletedObjectsImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("deletedObjectsImageCollection.ImageStream");
		this.deletedObjectsImageCollection.Images.SetKeyName(0, "database_deleted.png");
		this.deletedObjectsImageCollection.Images.SetKeyName(1, "function_deleted.png");
		this.deletedObjectsImageCollection.Images.SetKeyName(2, "procedure_deleted.png");
		this.deletedObjectsImageCollection.Images.SetKeyName(3, "table_deleted.png");
		this.deletedObjectsImageCollection.Images.SetKeyName(4, "view_deleted.png");
		this.treePopupImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("treePopupImageCollection.ImageStream");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.module_add_16, "module_add_16", typeof(Dataedo.App.Properties.Resources), 0);
		this.treePopupImageCollection.Images.SetKeyName(0, "module_add_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.refresh_16, "refresh_16", typeof(Dataedo.App.Properties.Resources), 1);
		this.treePopupImageCollection.Images.SetKeyName(1, "refresh_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.delete_16, "delete_16", typeof(Dataedo.App.Properties.Resources), 2);
		this.treePopupImageCollection.Images.SetKeyName(2, "delete_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.import_changes_16, "import_changes_16", typeof(Dataedo.App.Properties.Resources), 3);
		this.treePopupImageCollection.Images.SetKeyName(3, "import_changes_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.generate_documentation_16, "generate_documentation_16", typeof(Dataedo.App.Properties.Resources), 4);
		this.treePopupImageCollection.Images.SetKeyName(4, "generate_documentation_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.edit_16, "edit_16", typeof(Dataedo.App.Properties.Resources), 5);
		this.treePopupImageCollection.Images.SetKeyName(5, "edit_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.arrow_down_16, "arrow_down_16", typeof(Dataedo.App.Properties.Resources), 6);
		this.treePopupImageCollection.Images.SetKeyName(6, "arrow_down_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.arrow_up_16, "arrow_up_16", typeof(Dataedo.App.Properties.Resources), 7);
		this.treePopupImageCollection.Images.SetKeyName(7, "arrow_up_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.table_add_16, "table_add_16", typeof(Dataedo.App.Properties.Resources), 8);
		this.treePopupImageCollection.Images.SetKeyName(8, "table_add_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.bulk_add_tables_16, "bulk_add_tables_16", typeof(Dataedo.App.Properties.Resources), 9);
		this.treePopupImageCollection.Images.SetKeyName(9, "bulk_add_tables_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.copy_documentation_16, "copy_documentation_16", typeof(Dataedo.App.Properties.Resources), 10);
		this.treePopupImageCollection.Images.SetKeyName(10, "copy_documentation_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.term_add_16, "term_add_16", typeof(Dataedo.App.Properties.Resources), 11);
		this.treePopupImageCollection.Images.SetKeyName(11, "term_add_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.term_add_related_term_16, "term_add_related_term_16", typeof(Dataedo.App.Properties.Resources), 12);
		this.treePopupImageCollection.Images.SetKeyName(12, "term_add_related_term_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.data_link_add_16, "data_link_add_16", typeof(Dataedo.App.Properties.Resources), 13);
		this.treePopupImageCollection.Images.SetKeyName(13, "data_link_add_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.expand_all_16, "expand_all_16", typeof(Dataedo.App.Properties.Resources), 14);
		this.treePopupImageCollection.Images.SetKeyName(14, "expand_all_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.collapse_all_16, "collapse_all_16", typeof(Dataedo.App.Properties.Resources), 15);
		this.treePopupImageCollection.Images.SetKeyName(15, "collapse_all_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.server_add_16, "server_add_16", typeof(Dataedo.App.Properties.Resources), 16);
		this.treePopupImageCollection.Images.SetKeyName(16, "server_add_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.server_connect_16, "server_connect_16", typeof(Dataedo.App.Properties.Resources), 17);
		this.treePopupImageCollection.Images.SetKeyName(17, "server_connect_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.server_updated_16, "server_updated_16", typeof(Dataedo.App.Properties.Resources), 18);
		this.treePopupImageCollection.Images.SetKeyName(18, "server_updated_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.business_glossary_16, "business_glossary_16", typeof(Dataedo.App.Properties.Resources), 19);
		this.treePopupImageCollection.Images.SetKeyName(19, "business_glossary_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.add_16, "add_16", typeof(Dataedo.App.Properties.Resources), 20);
		this.treePopupImageCollection.Images.SetKeyName(20, "add_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.structure_add_16, "structure_add_16", typeof(Dataedo.App.Properties.Resources), 21);
		this.treePopupImageCollection.Images.SetKeyName(21, "structure_add_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.bulk_add_structures_16, "bulk_add_structures_16", typeof(Dataedo.App.Properties.Resources), 22);
		this.treePopupImageCollection.Images.SetKeyName(22, "bulk_add_structures_16");
		this.treePopupImageCollection.Images.SetKeyName(23, "icon_16.png");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.folder_open_16, "folder_open_16", typeof(Dataedo.App.Properties.Resources), 24);
		this.treePopupImageCollection.Images.SetKeyName(24, "folder_open_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.server_repository_connect_16, "server_repository_connect_16", typeof(Dataedo.App.Properties.Resources), 25);
		this.treePopupImageCollection.Images.SetKeyName(25, "server_repository_connect_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.server_repository_add_16, "server_repository_add_16", typeof(Dataedo.App.Properties.Resources), 26);
		this.treePopupImageCollection.Images.SetKeyName(26, "server_repository_add_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.profile_table_16, "profile_table_16", typeof(Dataedo.App.Properties.Resources), 27);
		this.treePopupImageCollection.Images.SetKeyName(27, "profile_table_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.all_data_deleted_16, "all_data_deleted_16", typeof(Dataedo.App.Properties.Resources), 28);
		this.treePopupImageCollection.Images.SetKeyName(28, "all_data_deleted_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.view_new_16, "view_new_16", typeof(Dataedo.App.Properties.Resources), 29);
		this.treePopupImageCollection.Images.SetKeyName(29, "view_new_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.copy_16_alt, "copy_16_alt", typeof(Dataedo.App.Properties.Resources), 30);
		this.treePopupImageCollection.Images.SetKeyName(30, "copy_16_alt");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.bulk_add_views_16, "bulk_add_views_16", typeof(Dataedo.App.Properties.Resources), 31);
		this.treePopupImageCollection.Images.SetKeyName(31, "bulk_add_views_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.function_new_16, "function_new_16", typeof(Dataedo.App.Properties.Resources), 32);
		this.treePopupImageCollection.Images.SetKeyName(32, "function_new_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.procedure_new_16, "procedure_new_16", typeof(Dataedo.App.Properties.Resources), 33);
		this.treePopupImageCollection.Images.SetKeyName(33, "procedure_new_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.import_descriptions_16, "import_descriptions_16", typeof(Dataedo.App.Properties.Resources), 34);
		this.treePopupImageCollection.Images.SetKeyName(34, "import_descriptions_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.sample_data_16, "sample_data_16", typeof(Dataedo.App.Properties.Resources), 35);
		this.treePopupImageCollection.Images.SetKeyName(35, "sample_data_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.sort_asc_16, "sort_asc_16", typeof(Dataedo.App.Properties.Resources), 36);
		this.treePopupImageCollection.Images.SetKeyName(36, "sort_asc_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.arrow_top_16, "arrow_top_16", typeof(Dataedo.App.Properties.Resources), 37);
		this.treePopupImageCollection.Images.SetKeyName(37, "arrow_top_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.arrow_down_16, "arrow_down_16", typeof(Dataedo.App.Properties.Resources), 38);
		this.treePopupImageCollection.Images.SetKeyName(38, "arrow_down_16");
		this.treePopupImageCollection.InsertImage(Dataedo.App.Properties.Resources.profile_database_16, "profile_database_16", typeof(Dataedo.App.Properties.Resources), 39);
		this.treePopupImageCollection.Images.SetKeyName(39, "profile_database_16");
		this.xlsxSaveFileDialog.Filter = "Excel files (.xlsx)|*.xlsx|Excel files (.xls)|*.xls";
		this.treeList1.Location = new System.Drawing.Point(204, 94);
		this.treeList1.Name = "treeList1";
		this.treeList1.Size = new System.Drawing.Size(400, 200);
		this.treeList1.TabIndex = 0;
		this.treeList2.Location = new System.Drawing.Point(223, -30);
		this.treeList2.Name = "treeList2";
		this.treeList2.Size = new System.Drawing.Size(400, 200);
		this.treeList2.TabIndex = 0;
		this.metadataEditorSplitContainerControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.metadataEditorSplitContainerControl.Location = new System.Drawing.Point(0, 0);
		this.metadataEditorSplitContainerControl.Name = "metadataEditorSplitContainerControl";
		this.metadataEditorSplitContainerControl.Panel1.Controls.Add(this.leftPanelXtraTabControl);
		this.metadataEditorSplitContainerControl.Panel1.Text = "Panel1";
		this.metadataEditorSplitContainerControl.Panel2.Text = "Panel2";
		this.metadataEditorSplitContainerControl.Size = new System.Drawing.Size(1200, 923);
		this.metadataEditorSplitContainerControl.SplitterPosition = 397;
		this.metadataEditorSplitContainerControl.TabIndex = 1;
		this.metadataEditorSplitContainerControl.Text = "splitContainerControl1";
		this.repositoryItemCheckedComboBoxEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCheckedComboBoxEdit1.Items.AddRange(new DevExpress.XtraEditors.Controls.CheckedListBoxItem[6]
		{
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem("DOCUMENTATION", "Documentations descriptions", System.Windows.Forms.CheckState.Checked),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem("MODULE", "Subject Areas descriptions", System.Windows.Forms.CheckState.Checked),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem("TABLE", "Tables", System.Windows.Forms.CheckState.Checked),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem("VIEW", "Views", System.Windows.Forms.CheckState.Checked),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem("PROCEDURE", "Procedures", System.Windows.Forms.CheckState.Checked),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem("FUNCTION", "Functions", System.Windows.Forms.CheckState.Checked)
		});
		this.repositoryItemCheckedComboBoxEdit1.Name = "repositoryItemCheckedComboBoxEdit1";
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		this.searchProgressPanel.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.searchProgressPanel.Appearance.Options.UseBackColor = true;
		this.searchProgressPanel.AppearanceCaption.Font = new System.Drawing.Font("Microsoft Sans Serif", 12f);
		this.searchProgressPanel.AppearanceCaption.Options.UseFont = true;
		this.searchProgressPanel.AppearanceDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f);
		this.searchProgressPanel.AppearanceDescription.Options.UseFont = true;
		this.searchProgressPanel.Description = "Searching...";
		this.searchProgressPanel.Location = new System.Drawing.Point(4, 3);
		this.searchProgressPanel.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.searchProgressPanel.Name = "searchProgressPanel";
		this.searchProgressPanel.Size = new System.Drawing.Size(129, 66);
		this.searchProgressPanel.TabIndex = 2;
		this.searchProgressPanelPanel.Controls.Add(this.searchProgressPanel);
		this.searchProgressPanelPanel.Location = new System.Drawing.Point(0, 284);
		this.searchProgressPanelPanel.Name = "searchProgressPanelPanel";
		this.searchProgressPanelPanel.Size = new System.Drawing.Size(389, 67);
		this.searchProgressPanelPanel.TabIndex = 0;
		this.searchProgressPanelPanel.Visible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.AutoSize = true;
		base.Controls.Add(this.searchProgressPanelPanel);
		base.Controls.Add(this.metadataEditorSplitContainerControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "MetadataEditorUserControl";
		base.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
		base.Size = new System.Drawing.Size(1200, 929);
		base.Load += new System.EventHandler(MetadataEditorUserControl_Load);
		((System.ComponentModel.ISupportInitialize)this.treelistBarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.leftPanelXtraTabControl).EndInit();
		this.leftPanelXtraTabControl.ResumeLayout(false);
		this.repositoryXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.metadataTreeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.progressDatabaseRepositoryItemProgressBar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.progressModuleRepositoryItemProgressBar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.progressFolderRepositoryItemProgressBar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.progressSimpleObjectRepositoryItemProgressBar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.loadingRepositoryItemProgressBar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.treeMenuImageCollection).EndInit();
		this.searchXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.searchLayoutControl).EndInit();
		this.searchLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.searchTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.advancedSearchSplitContainerControl).EndInit();
		this.advancedSearchSplitContainerControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.searchesXtraTabControl).EndInit();
		this.searchesXtraTabControl.ResumeLayout(false);
		this.searchResultsGridViewXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.searchResultsListGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.searchResultsListGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SearchResultsListNameRepositoryItemRichTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SearchResultsListNTitleRepositoryItemRichTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.InnerGrid).EndInit();
		this.searchResultsTreeListXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.searchResultsTreeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.searchResultsTreeListNameRepositoryItemRichTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.DocumentationsCheckedComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.TypesCheckedComboBoxEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.deletedObjectsImageCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.treePopupImageCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.treeList1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.treeList2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.metadataEditorSplitContainerControl).EndInit();
		this.metadataEditorSplitContainerControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckedComboBoxEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		this.searchProgressPanelPanel.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
