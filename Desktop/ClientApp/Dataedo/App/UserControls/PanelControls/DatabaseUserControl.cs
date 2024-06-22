using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.History;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.Search;
using Dataedo.App.Tools.UI;
using Dataedo.App.Tools.UI.Skins.Base;
using Dataedo.App.UserControls.PanelControls.Appearance;
using Dataedo.App.UserControls.PanelControls.CommonHelpers;
using Dataedo.App.UserControls.SchemaImportsAndChanges;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Documentations;
using Dataedo.Model.Data.History;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTab;

namespace Dataedo.App.UserControls.PanelControls;

public class DatabaseUserControl : BasePanelControl, ITabSettable, ISaveable
{
	private int databaseId;

	private bool _isDatabaseEdited;

	private DBTreeNode selectedNode;

	private ObjectWithTitleAndHTMLDescriptionHistory databaseTitleAndDescriptionHistory = new ObjectWithTitleAndHTMLDescriptionHistory();

	private Dictionary<string, string> customFieldsDatabaseForHistory = new Dictionary<string, string>();

	private IContainer components;

	private XtraTabControl databaseXtraTabControl;

	private XtraTabPage databaseDescriptionXtraTabPage;

	private NonCustomizableLayoutControl databaseLayoutControl;

	private TextEdit databaseTitleTextEdit;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem1;

	private HtmlUserControl databaseHtmlUserControl;

	private BackgroundWorker synchronizeBackgroundWorker;

	private ToolStripMenuItem addObjSynchToolStripMenuItem;

	private ToolStripMenuItem ignoreObjSynchToolStripMenuItem;

	private ImageCollection databaseConnectAndSynchronizeImageCollection;

	private ImageCollection objectSynchronizeImageCollection;

	private BackgroundWorker databaseConnectBackgroundWorker;

	private PopupMenu objSynchPopupMenu;

	private BarButtonItem barButtonItem1;

	private BarButtonItem barButtonItem2;

	private BarManager objSynchBarManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarButtonItem addObjSynchBarButtonItem;

	private BarButtonItem ignoreObjSynchBarButtonItem;

	private EmptySpaceItem emptySpaceItem1;

	private InfoUserControl databaseStatusUserControl;

	private DXErrorProvider databaseTitleErrorProvider;

	private LayoutControlItem layoutControlItem2;

	private CustomFieldsPanelControl customFieldsPanelControl;

	private LayoutControlItem customFieldsLayoutControlItem;

	private TextEdit databaseTextEdit;

	private TextEdit hostTextEdit;

	private LayoutControlItem hostLayoutControlItem;

	private XtraTabPage schemaImportsAndChangesXtraTabPage;

	private SchemaImportsAndChangesUserControl schemaImportsAndChangesUserControl;

	private LabelControl searchCountLabelControl;

	private LabelControl labelControl1;

	private LayoutControlItem layoutControlItem3;

	private LayoutControlItem layoutControlItem4;

	private TextEdit perspectiveTextEdit;

	private LayoutControlItem perspectiveLayoutControlItem;

	private LabelControl nameLabelControl;

	private LayoutControlItem nameLayoutControlItem;

	public override int DatabaseId => databaseId;

	public override int ObjectModuleId => -1;

	public override int ObjectId => databaseId;

	public override SharedObjectTypeEnum.ObjectType ObjectType => SharedObjectTypeEnum.ObjectType.Database;

	public override string ObjectSchema => string.Empty;

	public override string ObjectName => string.Empty;

	public override HtmlUserControl DescriptionHtmlUserControl => databaseHtmlUserControl;

	public override XtraTabPage SchemaImportsAndChangesXtraTabPage => schemaImportsAndChangesXtraTabPage;

	public override SchemaImportsAndChangesUserControl SchemaImportsAndChangesUserControl => schemaImportsAndChangesUserControl;

	public override CustomFieldsPanelControl CustomFieldsPanelControl => customFieldsPanelControl;

	public bool isDatabaseEdited
	{
		get
		{
			return _isDatabaseEdited;
		}
		set
		{
			_isDatabaseEdited = value;
			SetTabPageTitle(_isDatabaseEdited, databaseDescriptionXtraTabPage);
		}
	}

	private DatabaseRow database { get; set; }

	public DatabaseUserControl(MetadataEditorUserControl control)
		: base(control)
	{
		InitializeComponent();
		Initialize();
		databaseHtmlUserControl.ContentChangedEvent += databaseHtmlUserControl_PreviewKeyDown;
		base.Edit = new Edit(databaseTextEdit);
		LengthValidation.SetTitleOrNameLengthLimit(databaseTitleTextEdit);
		base.UserControlHelpers = new UserControlHelpers(0);
		SchemaImportsAndChangesSupport = new SchemaImportsAndChangesSupport(this);
		databaseStatusUserControl.SetShouldLoadColorsAfterLoad(shouldLoadColorsAfterLoad: false);
	}

	public override void SetRichEditControlBackground()
	{
	}

	public override void FillControlProgressHighlights()
	{
	}

	public void ClearHighlights(bool keepSearchActive)
	{
		base.UserControlHelpers.ClearHighlights(keepSearchActive, databaseXtraTabControl, null, nameLabelControl, databaseTitleTextEdit, customFieldsPanelControl.FieldControls);
		databaseHtmlUserControl.ClearHighlights();
		searchCountLabelControl.Text = string.Empty;
		searchCountLabelControl.BackColor = SkinColors.ControlColorFromSystemColors;
	}

	public void ForceLayoutChange(bool forceAll = false)
	{
	}

	public void SetTab(ResultItem row, SharedObjectTypeEnum.ObjectType? type, bool changeTab, string[] searchWords, List<CustomFieldSearchItem> customFieldSearchItems, params int?[] elementId)
	{
		if (!type.HasValue)
		{
			base.UserControlHelpers.SetHighlight(row, searchWords, customFieldSearchItems, null, 0, databaseXtraTabControl, null, nameLabelControl, databaseTitleTextEdit, customFieldsPanelControl.FieldControls, databaseHtmlUserControl, null, null);
			searchCountLabelControl.Text = databaseHtmlUserControl.Occurrences;
			BaseSkin.SetSearchHighlightOrDefault(searchCountLabelControl, databaseHtmlUserControl.OccurrencesCount > 0);
		}
	}

	public override void SetParameters(DBTreeNode selectedNode, CustomFieldsSupport customFieldsSupport, Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType? dependencyType = null)
	{
		base.SetParameters(selectedNode, customFieldsSupport, dependencyType);
		databaseHtmlUserControl.CanListen = false;
		this.selectedNode = selectedNode;
		int num = (databaseId = selectedNode.Id);
		hostTextEdit.Text = string.Empty;
		nameLabelControl.Text = string.Empty;
		perspectiveTextEdit.Text = string.Empty;
		perspectiveLayoutControlItem.Visibility = LayoutVisibility.Never;
		databaseTextEdit.Properties.ContextImage = IconsSupport.GetDatabaseIconByName16(selectedNode.DatabaseType, selectedNode.ObjectType);
		DocumentationObject dataById = DB.Database.GetDataById(num);
		if (dataById != null)
		{
			database = new DatabaseRow(dataById, customFieldsSupport);
			database.ObjectTypeValue = selectedNode.ObjectType;
			if (!database.IsWelcomeDocumentation && selectedNode.ObjectType != SharedObjectTypeEnum.ObjectType.BusinessGlossary)
			{
				hostLayoutControlItem.Visibility = LayoutVisibility.Always;
				nameLayoutControlItem.Visibility = LayoutVisibility.Always;
				schemaImportsAndChangesXtraTabPage.PageVisible = true;
			}
			else
			{
				hostLayoutControlItem.Visibility = LayoutVisibility.Never;
				nameLayoutControlItem.Visibility = LayoutVisibility.Never;
				schemaImportsAndChangesXtraTabPage.PageVisible = false;
			}
			hostTextEdit.Text = database.Host;
			if (database.Type == SharedDatabaseTypeEnum.DatabaseType.SsasTabular)
			{
				perspectiveTextEdit.Text = database.Perspective;
				perspectiveLayoutControlItem.Visibility = LayoutVisibility.Always;
			}
			else
			{
				perspectiveLayoutControlItem.Visibility = LayoutVisibility.Never;
			}
			if (database.HasMultipleSchemas == false)
			{
				nameLabelControl.Text = database.Name;
			}
			else if (database.ImportAllSchemas)
			{
				nameLabelControl.Text = string.Empty;
			}
			else
			{
				nameLabelControl.Text = string.Join(", ", database.Schemas);
			}
			CommonFunctionsPanels.SetSelectedTabPage(databaseXtraTabControl, dependencyType);
			databaseHtmlUserControl.HtmlText = database.Description;
			UpdateTitle(database.Title);
			base.CustomFieldsSupport = customFieldsSupport;
			customFields = new CustomFieldContainer(ObjectType, DatabaseId, customFieldsSupport);
			customFields.RetrieveCustomFields(dataById);
			customFields.ClearAddedDefinitionValues(null);
			SetCustomFieldsDataSource();
			isDatabaseEdited = false;
			CommonFunctionsPanels.ClearTabPagesTitle(databaseXtraTabControl, base.Edit);
			databaseStatusUserControl.Hide();
			schemaImportsAndChangesUserControl.ClearData();
			RefreshSchemaImportsAndChanges(forceRefresh: true);
		}
		else
		{
			CommonFunctionsPanels.SetSelectedTabPage(databaseXtraTabControl, dependencyType);
			databaseHtmlUserControl.HtmlText = null;
			UpdateTitle(selectedNode.Name);
			isDatabaseEdited = false;
			CommonFunctionsPanels.ClearTabPagesTitle(databaseXtraTabControl, base.Edit);
			databaseStatusUserControl.SetDeletedObjectProperties();
		}
		SchemaImportsAndChangesUserControl.SetFunctionality();
		customFieldsDatabaseForHistory = HistoryCustomFieldsHelper.GetOldCustomFieldsInObjectUserControl(customFields);
		databaseTitleAndDescriptionHistory = new ObjectWithTitleAndHTMLDescriptionHistory
		{
			ObjectId = databaseId,
			Title = databaseTitleTextEdit.Text,
			Description = PrepareValue.GetHtmlText(databaseHtmlUserControl?.PlainText, databaseHtmlUserControl?.HtmlText),
			DescriptionPlain = databaseHtmlUserControl?.PlainText
		};
		databaseHtmlUserControl.ClearHistoryObjects();
		databaseHtmlUserControl.DatabaseRow = database;
	}

	private bool IsRefreshRequired(XtraTabPage tabPage, bool additionalCondition = true, bool forceRefresh = false)
	{
		if (!forceRefresh || databaseXtraTabControl.SelectedTabPage != tabPage)
		{
			if (databaseXtraTabControl.SelectedTabPage == tabPage && additionalCondition)
			{
				return !base.TabPageChangedProgrammatically;
			}
			return false;
		}
		return true;
	}

	private void SetCustomFieldsDataSource()
	{
		CustomFieldsPanelControl.EditValueChanging += delegate
		{
			SetCurrentTabPageTitle(isEdited: true, databaseDescriptionXtraTabPage);
		};
		customFieldsPanelControl.ShowHistoryClick -= CustomFieldsPanelControl_ShowHistoryClick;
		customFieldsPanelControl.ShowHistoryClick += CustomFieldsPanelControl_ShowHistoryClick;
		IEnumerable<CustomFieldDefinition> customFieldRows = customFields.CustomFieldsData.Where((CustomFieldDefinition x) => x.CustomField?.DocumentationVisibility ?? false);
		customFieldsPanelControl.LoadFields(customFieldRows, delegate
		{
			isDatabaseEdited = true;
			base.Edit.SetEdited();
		}, customFieldsLayoutControlItem);
	}

	private void UpdateTitle(string title)
	{
		string text3 = (databaseTitleTextEdit.Text = (databaseTextEdit.Text = title));
		if (databaseTitleAndDescriptionHistory != null)
		{
			databaseTitleAndDescriptionHistory.Title = title;
		}
		if (databaseHtmlUserControl != null && databaseHtmlUserControl.DatabaseRow != null)
		{
			databaseHtmlUserControl.DatabaseRow.Title = title;
		}
	}

	public void SetNewTitle(string title)
	{
		editedTitleFromTreeList = true;
		UpdateTitle(title);
	}

	private void databaseXtraTabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		if (databaseXtraTabControl.SelectedTabPageIndex == 0)
		{
			SelectedTab.selectedTabCaption = "info";
		}
		else
		{
			SelectedTab.selectedTabCaption = databaseXtraTabControl.SelectedTabPage.Text;
		}
		RefreshSchemaImportsAndChanges();
	}

	private void RefreshSchemaImportsAndChanges(bool forceRefresh = false, bool refreshImmediatelyIfLoaded = false, bool refreshImmediatelyIfNotLoaded = false)
	{
		if (IsRefreshRequired(schemaImportsAndChangesXtraTabPage, additionalCondition: true, forceRefresh) || refreshImmediatelyIfNotLoaded || refreshImmediatelyIfLoaded)
		{
			schemaImportsAndChangesUserControl.RefreshImports();
		}
	}

	private void databaseTitleTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (editedTitleFromTreeList)
		{
			editedTitleFromTreeList = false;
			return;
		}
		IsTitleValid();
		isDatabaseEdited = true;
		base.Edit.SetEdited();
	}

	private void databaseHtmlUserControl_PreviewKeyDown(object sender, EventArgs e)
	{
		isDatabaseEdited = true;
		base.Edit.SetEdited();
	}

	private bool IsTitleValid()
	{
		return ValidateFields.IsEditNotEmptyRaiseError(databaseTitleTextEdit, databaseTitleErrorProvider);
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if (keyData == (Keys.F | Keys.Control) && databaseHtmlUserControl.PerformFindActionIfFocused())
			{
				return true;
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	public override bool Save()
	{
		try
		{
			bool flag = false;
			if (base.Edit.IsEdited)
			{
				CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
				if (!Licenses.CheckRepositoryVersionAfterLogin())
				{
					flag = true;
				}
				else if (IsTitleValid())
				{
					if (base.UserControlHelpers.IsSearchActive)
					{
						ClearHighlights(keepSearchActive: true);
					}
					EditTitleNode();
					DatabaseSyncRow databaseSyncRow = new DatabaseSyncRow(database.Id.Value, database.Name, databaseTitleTextEdit.Text, PrepareValue.GetHtmlText(databaseHtmlUserControl.PlainText, databaseHtmlUserControl.HtmlText), databaseHtmlUserControl.PlainTextForSearch);
					databaseSyncRow.CustomFields = customFields;
					customFieldsPanelControl.SetCustomFieldsValuesInRow(databaseSyncRow);
					if (DB.Database.Update(databaseSyncRow, FindForm()))
					{
						databaseHtmlUserControl.SetHtmlTextAsOriginal();
						isDatabaseEdited = false;
						customFieldsPanelControl.UpdateDefinitionValues();
						databaseSyncRow.CustomFields.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
					}
					else
					{
						flag = true;
					}
					if (SchemaImportsAndChangesSupport.UpdateSchemaImportsAndChangesComments(FindForm()))
					{
						SetTabPageTitle(isEdited: false, schemaImportsAndChangesXtraTabPage);
					}
					else
					{
						flag = true;
					}
					if (!flag)
					{
						SaveHistory(databaseSyncRow);
						UpdateTitle(databaseTitleTextEdit.Text);
						base.Edit.SetUnchanged();
						base.MainControl.RefreshSearchDatabasesFromMetadataTreeList(database.IdValue, database.Title);
						base.MainControl.RebuildHomePage(forceReload: false);
						if (base.UserControlHelpers.IsSearchActive)
						{
							base.MainControl.OpenCurrentlySelectedSearchRow();
							databaseHtmlUserControl.SetNotChanged();
							if (databaseHtmlUserControl.Highlight())
							{
								base.UserControlHelpers.SetHighlight();
								base.UserControlHelpers.SetHighlight();
								searchCountLabelControl.Text = databaseHtmlUserControl.Occurrences;
							}
							ForceLostFocus();
						}
						DB.Community.InsertFollowingToRepository(database.ObjectTypeValue, databaseSyncRow.Id);
					}
				}
				else
				{
					CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
					GeneralMessageBoxesHandling.Show("Title of the documentation can't be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
					flag = true;
				}
			}
			return !flag;
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			GeneralExceptionHandling.Handle(exception, FindForm());
			return false;
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
		}
	}

	private void SaveHistory(DatabaseSyncRow databaseRow)
	{
		bool saveTitle = HistoryGeneralHelper.CheckAreValuesDiffrent(databaseTitleAndDescriptionHistory?.Title, databaseTitleTextEdit.Text);
		bool saveDescription = HistoryGeneralHelper.CheckAreHtmlValuesAreDiffrent(databaseRow?.DescriptionPlain, databaseRow?.Description, databaseTitleAndDescriptionHistory?.DescriptionPlain, databaseTitleAndDescriptionHistory?.Description);
		databaseTitleAndDescriptionHistory = new ObjectWithTitleAndHTMLDescriptionHistory
		{
			ObjectId = databaseId,
			Title = databaseTitleTextEdit.Text,
			Description = PrepareValue.GetHtmlText(databaseHtmlUserControl?.PlainText, databaseHtmlUserControl?.HtmlText),
			DescriptionPlain = databaseHtmlUserControl?.PlainText
		};
		HistoryCustomFieldsHelper.InsertHistoryCustomFieldsOnTablePanel(databaseId, customFieldsDatabaseForHistory, customFields, databaseId, ObjectType);
		customFieldsDatabaseForHistory = new Dictionary<string, string>();
		CustomFieldDefinition[] customFieldsData = customFields.CustomFieldsData;
		foreach (CustomFieldDefinition customFieldDefinition in customFieldsData)
		{
			customFieldsDatabaseForHistory.Add(customFieldDefinition.CustomField.FieldName, customFieldDefinition.FieldValue);
		}
		DB.History.InsertHistoryRow(databaseId, databaseId, databaseTitleAndDescriptionHistory?.Title, databaseTitleAndDescriptionHistory?.Description, databaseTitleAndDescriptionHistory?.DescriptionPlain, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(ObjectType), saveTitle, saveDescription, ObjectType);
		if (databaseHtmlUserControl?.DatabaseRow != null)
		{
			databaseHtmlUserControl.DatabaseRow.Title = databaseTitleAndDescriptionHistory?.Title;
			databaseHtmlUserControl.DatabaseRow.ObjectTypeValue = database.ObjectTypeValue;
			databaseHtmlUserControl.DatabaseRow.Type = database?.Type;
		}
	}

	public void EditTitleNode()
	{
		string text3 = (selectedNode.Name = (database.Title = databaseTitleTextEdit.Text));
		selectedNode.Title = databaseTitleTextEdit.Text;
	}

	private void DatabaseTitleTextEdit_Properties_BeforeShowMenu(object sender, BeforeShowMenuEventArgs e)
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
		if (databaseTitleAndDescriptionHistory == null)
		{
			return;
		}
		try
		{
			using HistoryForm historyForm = new HistoryForm();
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
			historyForm.CustomFieldCaption = customFields?.CustomFieldsData?.Where((CustomFieldDefinition x) => x.CustomField.FieldName.ToLower() == fieldName)?.FirstOrDefault()?.CustomField?.Title;
			historyForm.DatabaseType = database?.Type;
			historyForm.SetParameters(databaseId, fieldName, null, ObjectSchema, base.DatabaseShowSchema, base.DatabaseShowSchemaOverride, databaseTitleAndDescriptionHistory.Title, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(database.ObjectTypeValue), SharedObjectTypeEnum.TypeToString(database.ObjectTypeValue), null, null, null);
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			historyForm.ShowDialog();
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void CustomFieldsPanelControl_ShowHistoryClick(object sender, EventArgs e)
	{
		if (e is TextEventArgs textEventArgs && textEventArgs.Text.StartsWith("field"))
		{
			ViewHistoryForField(textEventArgs.Text);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.PanelControls.DatabaseUserControl));
		this.databaseXtraTabControl = new DevExpress.XtraTab.XtraTabControl();
		this.databaseDescriptionXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.databaseLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.nameLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.searchCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.hostTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.objSynchBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.addObjSynchBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.ignoreObjSynchBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
		this.barButtonItem2 = new DevExpress.XtraBars.BarButtonItem();
		this.databaseTitleTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.customFieldsPanelControl = new Dataedo.App.UserControls.CustomFieldsPanelControl();
		this.databaseHtmlUserControl = new Dataedo.App.UserControls.HtmlUserControl();
		this.perspectiveTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.customFieldsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.hostLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.perspectiveLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.nameLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.schemaImportsAndChangesXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.schemaImportsAndChangesUserControl = new Dataedo.App.UserControls.SchemaImportsAndChanges.SchemaImportsAndChangesUserControl();
		this.addObjSynchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.ignoreObjSynchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.synchronizeBackgroundWorker = new System.ComponentModel.BackgroundWorker();
		this.databaseConnectAndSynchronizeImageCollection = new DevExpress.Utils.ImageCollection(this.components);
		this.objectSynchronizeImageCollection = new DevExpress.Utils.ImageCollection(this.components);
		this.databaseConnectBackgroundWorker = new System.ComponentModel.BackgroundWorker();
		this.objSynchPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.databaseStatusUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.databaseTitleErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
		this.databaseTextEdit = new DevExpress.XtraEditors.TextEdit();
		((System.ComponentModel.ISupportInitialize)this.databaseXtraTabControl).BeginInit();
		this.databaseXtraTabControl.SuspendLayout();
		this.databaseDescriptionXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.databaseLayoutControl).BeginInit();
		this.databaseLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.hostTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objSynchBarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseTitleTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.perspectiveTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hostLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.perspectiveLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameLayoutControlItem).BeginInit();
		this.schemaImportsAndChangesXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.databaseConnectAndSynchronizeImageCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectSynchronizeImageCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objSynchPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseTitleErrorProvider).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.databaseTextEdit.Properties).BeginInit();
		base.SuspendLayout();
		this.databaseXtraTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.databaseXtraTabControl.Location = new System.Drawing.Point(0, 69);
		this.databaseXtraTabControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.databaseXtraTabControl.Name = "databaseXtraTabControl";
		this.databaseXtraTabControl.SelectedTabPage = this.databaseDescriptionXtraTabPage;
		this.databaseXtraTabControl.Size = new System.Drawing.Size(942, 508);
		this.databaseXtraTabControl.TabIndex = 1;
		this.databaseXtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.databaseDescriptionXtraTabPage, this.schemaImportsAndChangesXtraTabPage });
		this.databaseXtraTabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(databaseXtraTabControl_SelectedPageChanged);
		this.databaseDescriptionXtraTabPage.Controls.Add(this.databaseLayoutControl);
		this.databaseDescriptionXtraTabPage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.databaseDescriptionXtraTabPage.Name = "databaseDescriptionXtraTabPage";
		this.databaseDescriptionXtraTabPage.Size = new System.Drawing.Size(940, 483);
		this.databaseDescriptionXtraTabPage.Text = "Documentation";
		this.databaseLayoutControl.AllowCustomization = false;
		this.databaseLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.databaseLayoutControl.Controls.Add(this.nameLabelControl);
		this.databaseLayoutControl.Controls.Add(this.searchCountLabelControl);
		this.databaseLayoutControl.Controls.Add(this.labelControl1);
		this.databaseLayoutControl.Controls.Add(this.hostTextEdit);
		this.databaseLayoutControl.Controls.Add(this.databaseTitleTextEdit);
		this.databaseLayoutControl.Controls.Add(this.customFieldsPanelControl);
		this.databaseLayoutControl.Controls.Add(this.databaseHtmlUserControl);
		this.databaseLayoutControl.Controls.Add(this.perspectiveTextEdit);
		this.databaseLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.databaseLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.databaseLayoutControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.databaseLayoutControl.Name = "databaseLayoutControl";
		this.databaseLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1359, 262, 250, 350);
		this.databaseLayoutControl.Root = this.layoutControlGroup1;
		this.databaseLayoutControl.Size = new System.Drawing.Size(940, 483);
		this.databaseLayoutControl.TabIndex = 0;
		this.databaseLayoutControl.Text = "layoutControl1";
		this.nameLabelControl.Appearance.Options.UseTextOptions = true;
		this.nameLabelControl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.nameLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
		this.nameLabelControl.Location = new System.Drawing.Point(99, 40);
		this.nameLabelControl.Name = "nameLabelControl";
		this.nameLabelControl.Size = new System.Drawing.Size(829, 13);
		this.nameLabelControl.StyleController = this.databaseLayoutControl;
		this.nameLabelControl.TabIndex = 9;
		this.nameLabelControl.Text = "nameLabelControl";
		this.searchCountLabelControl.Location = new System.Drawing.Point(69, 168);
		this.searchCountLabelControl.Name = "searchCountLabelControl";
		this.searchCountLabelControl.Size = new System.Drawing.Size(859, 13);
		this.searchCountLabelControl.StyleController = this.databaseLayoutControl;
		this.searchCountLabelControl.TabIndex = 8;
		this.labelControl1.Location = new System.Drawing.Point(12, 168);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(53, 13);
		this.labelControl1.StyleController = this.databaseLayoutControl;
		this.labelControl1.TabIndex = 7;
		this.labelControl1.Text = "Description";
		this.hostTextEdit.Location = new System.Drawing.Point(94, 12);
		this.hostTextEdit.MenuManager = this.objSynchBarManager;
		this.hostTextEdit.Name = "hostTextEdit";
		this.hostTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.hostTextEdit.Properties.ReadOnly = true;
		this.hostTextEdit.Size = new System.Drawing.Size(834, 18);
		this.hostTextEdit.StyleController = this.databaseLayoutControl;
		this.hostTextEdit.TabIndex = 2;
		this.hostTextEdit.TabStop = false;
		this.objSynchBarManager.Categories.AddRange(new DevExpress.XtraBars.BarManagerCategory[1]
		{
			new DevExpress.XtraBars.BarManagerCategory("Main", new System.Guid("3ad5fb8c-c802-4421-b49c-a1cca1c86118"))
		});
		this.objSynchBarManager.DockControls.Add(this.barDockControlTop);
		this.objSynchBarManager.DockControls.Add(this.barDockControlBottom);
		this.objSynchBarManager.DockControls.Add(this.barDockControlLeft);
		this.objSynchBarManager.DockControls.Add(this.barDockControlRight);
		this.objSynchBarManager.Form = this;
		this.objSynchBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[4] { this.addObjSynchBarButtonItem, this.ignoreObjSynchBarButtonItem, this.barButtonItem1, this.barButtonItem2 });
		this.objSynchBarManager.MaxItemId = 4;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.objSynchBarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(942, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 577);
		this.barDockControlBottom.Manager = this.objSynchBarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(942, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.objSynchBarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 577);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(942, 0);
		this.barDockControlRight.Manager = this.objSynchBarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 577);
		this.addObjSynchBarButtonItem.Caption = "Add";
		this.addObjSynchBarButtonItem.CategoryGuid = new System.Guid("3ad5fb8c-c802-4421-b49c-a1cca1c86118");
		this.addObjSynchBarButtonItem.Id = 0;
		this.addObjSynchBarButtonItem.Name = "addObjSynchBarButtonItem";
		this.ignoreObjSynchBarButtonItem.Caption = "Ignore";
		this.ignoreObjSynchBarButtonItem.CategoryGuid = new System.Guid("3ad5fb8c-c802-4421-b49c-a1cca1c86118");
		this.ignoreObjSynchBarButtonItem.Id = 1;
		this.ignoreObjSynchBarButtonItem.Name = "ignoreObjSynchBarButtonItem";
		this.barButtonItem1.Caption = "Add";
		this.barButtonItem1.Id = 2;
		this.barButtonItem1.Name = "barButtonItem1";
		this.barButtonItem2.Caption = "Ignore";
		this.barButtonItem2.Id = 3;
		this.barButtonItem2.Name = "barButtonItem2";
		this.databaseTitleTextEdit.Location = new System.Drawing.Point(97, 85);
		this.databaseTitleTextEdit.MaximumSize = new System.Drawing.Size(0, 22);
		this.databaseTitleTextEdit.MinimumSize = new System.Drawing.Size(0, 22);
		this.databaseTitleTextEdit.Name = "databaseTitleTextEdit";
		this.databaseTitleTextEdit.Properties.MaxLength = 64;
		this.databaseTitleTextEdit.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(DatabaseTitleTextEdit_Properties_BeforeShowMenu);
		this.databaseTitleTextEdit.Size = new System.Drawing.Size(411, 22);
		this.databaseTitleTextEdit.StyleController = this.databaseLayoutControl;
		this.databaseTitleTextEdit.TabIndex = 4;
		this.databaseTitleTextEdit.EditValueChanged += new System.EventHandler(databaseTitleTextEdit_EditValueChanged);
		this.customFieldsPanelControl.BackColor = System.Drawing.Color.Transparent;
		this.customFieldsPanelControl.Location = new System.Drawing.Point(10, 111);
		this.customFieldsPanelControl.Margin = new System.Windows.Forms.Padding(0);
		this.customFieldsPanelControl.Name = "customFieldsPanelControl";
		this.customFieldsPanelControl.Size = new System.Drawing.Size(918, 53);
		this.customFieldsPanelControl.TabIndex = 5;
		this.databaseHtmlUserControl.BackColor = System.Drawing.Color.Transparent;
		this.databaseHtmlUserControl.CanListen = false;
		this.databaseHtmlUserControl.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.databaseHtmlUserControl.HtmlText = resources.GetString("databaseHtmlUserControl.HtmlText");
		this.databaseHtmlUserControl.IsHighlighted = false;
		this.databaseHtmlUserControl.Location = new System.Drawing.Point(12, 185);
		this.databaseHtmlUserControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.databaseHtmlUserControl.Name = "databaseHtmlUserControl";
		this.databaseHtmlUserControl.OccurrencesCount = 0;
		this.databaseHtmlUserControl.OriginalHtmlText = resources.GetString("databaseHtmlUserControl.OriginalHtmlText");
		this.databaseHtmlUserControl.PlainText = "\u00a0";
		this.databaseHtmlUserControl.Size = new System.Drawing.Size(916, 296);
		this.databaseHtmlUserControl.SplashScreenManager = null;
		this.databaseHtmlUserControl.TabIndex = 6;
		this.databaseHtmlUserControl.TableRow = null;
		this.databaseHtmlUserControl.TermObject = null;
		this.perspectiveTextEdit.Location = new System.Drawing.Point(94, 61);
		this.perspectiveTextEdit.Name = "perspectiveTextEdit";
		this.perspectiveTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.perspectiveTextEdit.Properties.ReadOnly = true;
		this.perspectiveTextEdit.Size = new System.Drawing.Size(834, 18);
		this.perspectiveTextEdit.StyleController = this.databaseLayoutControl;
		this.perspectiveTextEdit.TabIndex = 3;
		this.perspectiveTextEdit.TabStop = false;
		this.layoutControlGroup1.CustomizationFormText = "Root";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[9] { this.layoutControlItem1, this.emptySpaceItem1, this.layoutControlItem2, this.customFieldsLayoutControlItem, this.hostLayoutControlItem, this.layoutControlItem3, this.layoutControlItem4, this.perspectiveLayoutControlItem, this.nameLayoutControlItem });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(940, 483);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem1.Control = this.databaseTitleTextEdit;
		this.layoutControlItem1.CustomizationFormText = "Title:";
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 73);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(500, 26);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(500, 26);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(500, 26);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.Text = "Title";
		this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(82, 13);
		this.layoutControlItem1.TextToControlDistance = 3;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
		this.emptySpaceItem1.Location = new System.Drawing.Point(500, 73);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(420, 26);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.Control = this.databaseHtmlUserControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 173);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(104, 300);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(920, 300);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.Text = "Description";
		this.layoutControlItem2.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.customFieldsLayoutControlItem.Control = this.customFieldsPanelControl;
		this.customFieldsLayoutControlItem.Location = new System.Drawing.Point(0, 99);
		this.customFieldsLayoutControlItem.Name = "customFieldsLayoutControlItem";
		this.customFieldsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
		this.customFieldsLayoutControlItem.Size = new System.Drawing.Size(920, 57);
		this.customFieldsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.customFieldsLayoutControlItem.TextVisible = false;
		this.hostLayoutControlItem.Control = this.hostTextEdit;
		this.hostLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.hostLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.hostLayoutControlItem.MinSize = new System.Drawing.Size(172, 24);
		this.hostLayoutControlItem.Name = "hostLayoutControlItem";
		this.hostLayoutControlItem.Size = new System.Drawing.Size(920, 24);
		this.hostLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.hostLayoutControlItem.Text = "Host";
		this.hostLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.hostLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.hostLayoutControlItem.TextToControlDistance = 0;
		this.layoutControlItem3.Control = this.labelControl1;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 156);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(57, 17);
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.layoutControlItem4.Control = this.searchCountLabelControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(57, 156);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(863, 17);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.perspectiveLayoutControlItem.Control = this.perspectiveTextEdit;
		this.perspectiveLayoutControlItem.ControlAlignment = System.Drawing.ContentAlignment.TopLeft;
		this.perspectiveLayoutControlItem.CustomizationFormText = "Perspective";
		this.perspectiveLayoutControlItem.Location = new System.Drawing.Point(0, 49);
		this.perspectiveLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.perspectiveLayoutControlItem.MinSize = new System.Drawing.Size(141, 24);
		this.perspectiveLayoutControlItem.Name = "perspectiveLayoutControlItem";
		this.perspectiveLayoutControlItem.Size = new System.Drawing.Size(920, 24);
		this.perspectiveLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.perspectiveLayoutControlItem.Text = "Perspective";
		this.perspectiveLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.perspectiveLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.perspectiveLayoutControlItem.TextToControlDistance = 0;
		this.perspectiveLayoutControlItem.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.nameLayoutControlItem.Control = this.nameLabelControl;
		this.nameLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.nameLayoutControlItem.Name = "nameLayoutControlItem";
		this.nameLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 6, 6);
		this.nameLayoutControlItem.Size = new System.Drawing.Size(920, 25);
		this.nameLayoutControlItem.Text = "Database";
		this.nameLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.nameLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.nameLayoutControlItem.TextToControlDistance = 5;
		this.schemaImportsAndChangesXtraTabPage.Controls.Add(this.schemaImportsAndChangesUserControl);
		this.schemaImportsAndChangesXtraTabPage.Name = "schemaImportsAndChangesXtraTabPage";
		this.schemaImportsAndChangesXtraTabPage.Size = new System.Drawing.Size(940, 483);
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
		this.schemaImportsAndChangesUserControl.Size = new System.Drawing.Size(940, 483);
		this.schemaImportsAndChangesUserControl.TabIndex = 0;
		this.addObjSynchToolStripMenuItem.Name = "addObjSynchToolStripMenuItem";
		this.addObjSynchToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
		this.ignoreObjSynchToolStripMenuItem.Name = "ignoreObjSynchToolStripMenuItem";
		this.ignoreObjSynchToolStripMenuItem.Size = new System.Drawing.Size(32, 19);
		this.synchronizeBackgroundWorker.WorkerReportsProgress = true;
		this.synchronizeBackgroundWorker.WorkerSupportsCancellation = true;
		this.databaseConnectAndSynchronizeImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("databaseConnectAndSynchronizeImageCollection.ImageStream");
		this.databaseConnectAndSynchronizeImageCollection.Images.SetKeyName(0, "database_connecting.png");
		this.databaseConnectAndSynchronizeImageCollection.Images.SetKeyName(1, "database.png");
		this.databaseConnectAndSynchronizeImageCollection.Images.SetKeyName(2, "database_deleted.png");
		this.databaseConnectAndSynchronizeImageCollection.Images.SetKeyName(3, "database_new.png");
		this.databaseConnectAndSynchronizeImageCollection.Images.SetKeyName(4, "database_synch.png");
		this.databaseConnectAndSynchronizeImageCollection.Images.SetKeyName(5, "database_unsynch.png");
		this.databaseConnectAndSynchronizeImageCollection.Images.SetKeyName(6, "database_error.png");
		this.databaseConnectAndSynchronizeImageCollection.Images.SetKeyName(7, "database_updating.png");
		this.databaseConnectAndSynchronizeImageCollection.Images.SetKeyName(8, "database_synchronize.png");
		this.objectSynchronizeImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("objectSynchronizeImageCollection.ImageStream");
		this.objectSynchronizeImageCollection.Images.SetKeyName(0, "function_deleted.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(1, "function_ignored.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(2, "function_new.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(3, "function_synch.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(4, "function_unsynch.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(5, "procedure_deleted.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(6, "procedure_ignored.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(7, "procedure_new.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(8, "procedure_synch.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(9, "procedure_unsynch.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(10, "table_deleted.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(11, "table_ignored.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(12, "table_new.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(13, "table_synch.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(14, "table_unsynch.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(15, "view_deleted.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(16, "view_ignored.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(17, "view_new.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(18, "view_unsynch.png");
		this.objectSynchronizeImageCollection.Images.SetKeyName(19, "view_synch.png");
		this.databaseConnectBackgroundWorker.WorkerReportsProgress = true;
		this.databaseConnectBackgroundWorker.WorkerSupportsCancellation = true;
		this.objSynchPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem1),
			new DevExpress.XtraBars.LinkPersistInfo(this.barButtonItem2)
		});
		this.objSynchPopupMenu.Manager = this.objSynchBarManager;
		this.objSynchPopupMenu.Name = "objSynchPopupMenu";
		this.databaseStatusUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.databaseStatusUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.databaseStatusUserControl.Description = "This database has been removed from the repository.";
		this.databaseStatusUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.databaseStatusUserControl.ForeColor = System.Drawing.Color.FromArgb(40, 40, 40);
		this.databaseStatusUserControl.Image = Dataedo.App.Properties.Resources.warning_16;
		this.databaseStatusUserControl.Location = new System.Drawing.Point(0, 20);
		this.databaseStatusUserControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.databaseStatusUserControl.Name = "databaseStatusUserControl";
		this.databaseStatusUserControl.Size = new System.Drawing.Size(942, 49);
		this.databaseStatusUserControl.TabIndex = 10;
		this.databaseStatusUserControl.Visible = false;
		this.databaseTitleErrorProvider.ContainerControl = this;
		this.databaseTextEdit.Dock = System.Windows.Forms.DockStyle.Top;
		this.databaseTextEdit.EditValue = "";
		this.databaseTextEdit.Location = new System.Drawing.Point(0, 0);
		this.databaseTextEdit.MenuManager = this.objSynchBarManager;
		this.databaseTextEdit.Name = "databaseTextEdit";
		this.databaseTextEdit.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold);
		this.databaseTextEdit.Properties.Appearance.Options.UseFont = true;
		this.databaseTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.databaseTextEdit.Properties.ContextImageOptions.Image = Dataedo.App.Properties.Resources.server_16;
		this.databaseTextEdit.Properties.ReadOnly = true;
		this.databaseTextEdit.Size = new System.Drawing.Size(942, 20);
		this.databaseTextEdit.TabIndex = 15;
		this.databaseTextEdit.TabStop = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.databaseXtraTabControl);
		base.Controls.Add(this.databaseStatusUserControl);
		base.Controls.Add(this.databaseTextEdit);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		this.Font = new System.Drawing.Font("Arial", 10f);
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		base.Name = "DatabaseUserControl";
		base.Size = new System.Drawing.Size(942, 577);
		((System.ComponentModel.ISupportInitialize)this.databaseXtraTabControl).EndInit();
		this.databaseXtraTabControl.ResumeLayout(false);
		this.databaseDescriptionXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.databaseLayoutControl).EndInit();
		this.databaseLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.hostTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objSynchBarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseTitleTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.perspectiveTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hostLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.perspectiveLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameLayoutControlItem).EndInit();
		this.schemaImportsAndChangesXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.databaseConnectAndSynchronizeImageCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectSynchronizeImageCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objSynchPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseTitleErrorProvider).EndInit();
		((System.ComponentModel.ISupportInitialize)this.databaseTextEdit.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
