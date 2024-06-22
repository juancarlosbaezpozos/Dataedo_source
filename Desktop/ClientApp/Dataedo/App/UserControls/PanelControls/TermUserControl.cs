using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.History;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.DragDrop;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Helpers;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.Search;
using Dataedo.App.Tools.Tracking.Helpers;
using Dataedo.App.Tools.UI;
using Dataedo.App.Tools.UI.Skins.Base;
using Dataedo.App.UserControls.MetadataEditorUserControlFeatures;
using Dataedo.App.UserControls.PanelControls.Appearance;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.CustomMessageBox;
using Dataedo.DataProcessing.Classes;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.BusinessGlossary;
using Dataedo.Model.Data.Interfaces;
using Dataedo.Shared.Enums;
using DevExpress.Data;
using DevExpress.Utils;
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
using DevExpress.XtraTab;

namespace Dataedo.App.UserControls.PanelControls;

public class TermUserControl : BasePanelControl, ITabSettable
{
	private List<TermRelationshipTypeObject> termRelationshipTypes;

	private BindingList<TermRelationshipObjectExtended> termRelationshipObjects;

	private BindingList<DataLinkObjectExtended> objectsWithDataLink;

	private bool isRelatedTermDataLoaded;

	private bool isDataLinksDataLoaded;

	private DBTreeNode selectedDBTreeNode;

	private string title;

	private int termId;

	private int? databaseId;

	private SharedDatabaseTypeEnum.DatabaseType? databaseType;

	private IdEventArgs idEventArgs;

	private string databaseTitle;

	private bool showDatabaseTitle;

	private bool isTermEdited;

	private Dictionary<string, string> customFieldsTermForHistory = new Dictionary<string, string>();

	private IContainer components;

	private XtraTabControl termXtraTabControl;

	private XtraTabPage termDescriptionXtraTabPage;

	private LayoutControlGroup layoutControlGroup1;

	private LayoutControlItem layoutControlItem1;

	private LayoutControlItem layoutControlItem2;

	private TextEdit termTitleTextEdit;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem2;

	private ToolTipController termToolTipController;

	private EmptySpaceItem emptySpaceItem1;

	private InfoUserControl termStatusUserControl;

	private XtraTabPage termMetadataXtraTabPage;

	private LayoutControlGroup layoutControlGroup2;

	private EmptySpaceItem emptySpaceItem3;

	private LabelControl lastUpdatedLabel;

	private LayoutControlItem lastUpdatedLayoutControlItem;

	private HtmlUserControl termHtmlUserControl;

	private LayoutControlItem layoutControlItem10;

	private CustomFieldsPanelControl customFieldsPanelControl;

	private LayoutControlItem customFieldsLayoutControlItem;

	private TextEdit termTextEdit;

	private TextEdit documentationTextEdit;

	private LayoutControlItem documentationLayoutControlItem;

	private LabelControl createdLabelControl;

	private LayoutControlItem firstImportedLayoutControlItem;

	private LookUpEdit termTypeLookUpEdit;

	private LayoutControlItem typeLayoutControlItem;

	private XtraTabPage relatedTermsXtraTabPage;

	private GridControl relatedTermsGrid;

	private BulkCopyGridUserControl relatedTermsGridView;

	private RepositoryItemPictureEdit iconTableRepositoryItemPictureEdit;

	private GridColumn relationshipTypeGridColumn;

	private GridColumn relatedTermTypeTableGridColumn;

	private GridColumn createdGridColumn;

	private GridColumn createdByGridColumn;

	private GridColumn lastUpdatedGridColumn;

	private GridColumn lastUpdatedByGridColumn;

	private GridPanelUserControl relatedTermsGridPanelUserControl;

	private GridColumn relatedTermTitleGridColumn;

	private GridColumn commentsGridColumn;

	private RepositoryItemAutoHeightMemoEdit commentsRepositoryItemAutoHeightMemoEdit;

	private XtraTabPage dataLinksXtraTabPage;

	private GridControl dataLinksGrid;

	private BulkCopyGridUserControl dataLinksGridView;

	private GridColumn documentationDataLinksGridColumn;

	private GridColumn tableDataLinksGridColumn;

	private GridColumn columnDataLinksGridColumn;

	private RepositoryItemAutoHeightMemoEdit repositoryItemAutoHeightMemoEdit1;

	private GridColumn createdDataLinksGridColumn;

	private GridColumn createdByDataLinksGridColumn;

	private GridColumn lastUpdatedDataLinksGridColumn;

	private GridColumn lastUpdatedByDataLinksGridColumn;

	private GridPanelUserControl dataLinksGridPanelUserControl;

	private GridColumn iconDataLinksGridColumn;

	private RepositoryItemPictureEdit iconRepositoryItemPictureEdit;

	private NonCustomizableLayoutControl termLayoutControl;

	private NonCustomizableLayoutControl metadataLayoutControl;

	private GridColumn relatedTermIconGridColumn;

	private DXErrorProvider termTitleErrorProvider;

	private RepositoryItemLookUpEdit relationshipTypeRepositoryItemLookUpEdit;

	private LabelControl labelControl1;

	private LayoutControlItem layoutControlItem4;

	private LabelControl searchCountLabelControl;

	private LayoutControlItem layoutControlItem5;

	private BarDockControl barDockControlLeft;

	private BarManager relatedTermsBarManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlRight;

	private BarButtonItem editTermRelationshipsRelatedTermsBarButtonItem;

	private BarButtonItem goToObjectRelatedTermsBarButtonItem;

	private BarButtonItem removeRelatedTermsBarButtonItem;

	private PopupMenu relatedTermsPopupMenu;

	private BarDockControl barDockControl3;

	private BarManager linkedDataBarManager;

	private BarDockControl barDockControl1;

	private BarDockControl barDockControl2;

	private BarDockControl barDockControl4;

	private BarButtonItem editLinksLinkedDataBarButtonItem;

	private BarButtonItem goToObjectLinkedDataBarButtonItem;

	private BarButtonItem removeLinkedDataBarButtonItem;

	private PopupMenu linkedDataPopupMenu;

	private ToolTipController metadataToolTipController;

	private ToolTipController toolTipController;

	public override int DatabaseId => databaseId ?? (-1);

	public override int ObjectModuleId => -1;

	public override int ObjectId => termId;

	public override SharedObjectTypeEnum.ObjectType ObjectType => SharedObjectTypeEnum.ObjectType.Term;

	public override string ObjectSchema => null;

	public override string ObjectName => null;

	public override HtmlUserControl DescriptionHtmlUserControl => termHtmlUserControl;

	public override CustomFieldsPanelControl CustomFieldsPanelControl => customFieldsPanelControl;

	public bool IsTermEdited
	{
		get
		{
			return isTermEdited;
		}
		set
		{
			if (!base.DisableSettingAsEdited)
			{
				isTermEdited = value;
				SetTabPageTitle(isTermEdited, termDescriptionXtraTabPage, base.Edit);
			}
		}
	}

	private TermObject TermObjectCopyHistory { get; set; }

	public TermUserControl(MetadataEditorUserControl control)
		: base(control)
	{
		InitializeComponent();
		Initialize();
		MetadataToolTip.SetLayoutControlItemToolTip(firstImportedLayoutControlItem, "first_imported");
		MetadataToolTip.SetLayoutControlItemToolTip(lastUpdatedLayoutControlItem, "last_updated");
		termHtmlUserControl.ContentChangedEvent += TermHtmlUserControl_PreviewKeyDown;
		termHtmlUserControl.ProgressValueChanged += SetRichEditControlBackground;
		termHtmlUserControl.IsEditorFocused += SetRichEditControlBackgroundWhileFocused;
		base.Edit = new Edit(termTextEdit);
		idEventArgs = new IdEventArgs();
		LengthValidation.SetTitleOrNameLengthLimit(termTitleTextEdit);
		base.UserControlHelpers = new UserControlHelpers(3);
		relatedTermsGridPanelUserControl.RemoveCustomFieldsButton();
		termRelationshipObjects = new BindingList<TermRelationshipObjectExtended>();
		BarButtonItem barButtonItem = new BarButtonItem();
		barButtonItem.Glyph = Resources.term_add_related_term_16;
		barButtonItem.Hint = "Edit term relationships";
		barButtonItem.Name = "addRelatedTermBarButtonItem";
		barButtonItem.ItemClick += AddRelatedTermBarButtonItem_ItemClick;
		relatedTermsGridPanelUserControl.InsertAdditionalButtonBeforeRemoveButton(barButtonItem);
		relatedTermsGridPanelUserControl.Delete += ProcessDeletingRelatedTerms;
		SetTermRelationshipTypes();
		relationshipTypeRepositoryItemLookUpEdit.DataSource = termRelationshipTypes;
		dataLinksGridPanelUserControl.RemoveCustomFieldsButton();
		objectsWithDataLink = new BindingList<DataLinkObjectExtended>();
		BarButtonItem barButtonItem2 = new BarButtonItem();
		barButtonItem2.Glyph = Resources.data_link_add_16;
		barButtonItem2.Hint = "Edit links to Data Dictionary elements";
		barButtonItem2.Name = "addLinkBarButtonItem";
		barButtonItem2.ItemClick += AddLinkBarButtonItem_ItemClick;
		dataLinksGridPanelUserControl.InsertAdditionalButtonBeforeRemoveButton(barButtonItem2);
		dataLinksGridPanelUserControl.Delete += ProcessDeletingLink;
		dataLinksGridView.AddColumnToFilterMapping(columnDataLinksGridColumn, "ElementFullName");
		termStatusUserControl.SetShouldLoadColorsAfterLoad(shouldLoadColorsAfterLoad: false);
	}

	public void SetTermRelationshipTypes()
	{
		termRelationshipTypes = base.MainControl.BusinessGlossarySupport.TermRelationshipTypes;
	}

	private void HighlightGridView_RowCellStyle(object sender, RowCellCustomDrawEventArgs e)
	{
	}

	public void ClearHighlights(bool keepSearchActive)
	{
		base.UserControlHelpers.ClearHighlights(keepSearchActive, termXtraTabControl, null, null, termTitleTextEdit, customFieldsPanelControl.FieldControls);
		termHtmlUserControl.ClearHighlights();
		searchCountLabelControl.Text = string.Empty;
		searchCountLabelControl.BackColor = SkinColors.ControlColorFromSystemColors;
	}

	public void ForceLayoutChange(bool forceAll = false)
	{
	}

	public void SetDisableSettingAsEdited(bool value)
	{
		base.DisableSettingAsEdited = value;
	}

	public void SetTab(ResultItem row, SharedObjectTypeEnum.ObjectType? type, bool changeTab, string[] searchWords, List<CustomFieldSearchItem> customFieldSearchItems, params int?[] elementId)
	{
		int num = 0;
		if (!type.HasValue)
		{
			num = 0;
			base.UserControlHelpers.SetHighlight(row, searchWords, customFieldSearchItems, null, num, termXtraTabControl, null, null, termTitleTextEdit, customFieldsPanelControl.FieldControls, termHtmlUserControl, null, null);
			searchCountLabelControl.Text = termHtmlUserControl.Occurrences;
			BaseSkin.SetSearchHighlightOrDefault(searchCountLabelControl, termHtmlUserControl.OccurrencesCount > 0);
		}
		if (changeTab)
		{
			termXtraTabControl.SelectedTabPageIndex = num;
		}
	}

	private void TermUserControl_Load(object sender, EventArgs e)
	{
		AddEvents();
		WorkWithDataedoTrackingHelper.TrackFirstInSessionTermView();
	}

	private void UpdateTitle(string title)
	{
		string text3 = (termTitleTextEdit.Text = (termTextEdit.Text = title));
	}

	public void SetNewTitle(string title)
	{
		editedTitleFromTreeList = true;
		try
		{
			base.DisableSettingAsEdited = true;
			UpdateTitle(title);
		}
		finally
		{
			base.DisableSettingAsEdited = false;
		}
	}

	public void SetTitleTextEdit(string title)
	{
		string text3 = (termTitleTextEdit.Text = (termTextEdit.Text = title));
		TermObjectCopyHistory.Title = title;
	}

	public override void SetParameters(DBTreeNode selectedNode, CustomFieldsSupport customFieldsSupport, DependencyRow.DependencyNodeCommonType? dependencyType = null)
	{
		base.SetParameters(selectedNode, customFieldsSupport, dependencyType);
		try
		{
			IsTermEdited = false;
			base.DisableSettingAsEdited = true;
			selectedDBTreeNode = selectedNode;
			showDatabaseTitle = selectedNode?.ParentNode?.ParentNode != null && selectedNode.ParentNode.ParentNode.HasMultipleDatabases;
			termHtmlUserControl.CanListen = false;
			base.TabPageChangedProgrammatically = true;
			CommonFunctionsPanels.SetSelectedTabPage(termXtraTabControl, dependencyType);
			base.TabPageChangedProgrammatically = false;
			termId = selectedNode.Id;
			TermObject term = DB.BusinessGlossary.GetTerm(termId);
			if (term == null)
			{
				base.MainControl.ClearDataPanel();
				base.MainControl.SetWaitformVisibility(visible: false);
				GeneralMessageBoxesHandling.Show("Term does not exist in repository.", "Term does not exist", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
				return;
			}
			SetTermTypes(term.TypeId);
			ClearData(selectedNode);
			base.DisableSettingAsEdited = true;
			if (term != null)
			{
				SetTermBasicData(term);
				termStatusUserControl.Hide();
				termHtmlUserControl.ClearHistoryObjects();
				termHtmlUserControl.TermObject = term;
				termHtmlUserControl.SplashScreenManager = GetSplashScreenManager();
				base.CustomFieldsSupport = customFieldsSupport;
				customFields = new CustomFieldContainer(ObjectType, ObjectId, customFieldsSupport);
				customFields.RetrieveCustomFields(term);
				customFields.ClearAddedDefinitionValues(null);
				SetCustomFieldsDataSource();
				databaseType = DatabaseTypeEnum.StringToType(term.DatabaseType);
				databaseTitle = term.DatabaseTitle;
				documentationTextEdit.Text = databaseTitle;
				CommonFunctionsPanels.SetName(termTextEdit, termDescriptionXtraTabPage, SharedObjectTypeEnum.ObjectType.Term, base.Subtype, null, termTitleTextEdit.Text, null, databaseType, databaseTitle, showDatabaseTitle, withSchema: false, UserTypeEnum.UserType.DBMS, selectedDBTreeNode.CustomInfo);
				termRelationshipTypes = base.MainControl.BusinessGlossarySupport.TermRelationshipTypes;
				termHtmlUserControl.HtmlText = term.Description;
				TermObjectCopyHistory = new TermObject
				{
					TermId = term.TermId,
					Title = term.Title,
					Description = PrepareValue.GetHtmlText(termHtmlUserControl?.PlainText, termHtmlUserControl?.HtmlText),
					DescriptionPlain = termHtmlUserControl?.PlainText
				};
				SetMetadata(term);
				RefreshRelatedTerms(forceRefresh: true);
				RefreshDataLinks(forceRefresh: true);
				relatedTermsGridPanelUserControl.Initialize(relatedTermsXtraTabPage.Text);
				relatedTermsGridPanelUserControl.CustomFields += base.EditCustomFieldsFromGridPanel;
				relatedTermsGridPanelUserControl.SetRemoveButton("Remove term relationship", Resources.delete_16);
				dataLinksGridPanelUserControl.Initialize(dataLinksXtraTabPage.Text);
				dataLinksGridPanelUserControl.CustomFields += base.EditCustomFieldsFromGridPanel;
				dataLinksGridPanelUserControl.SetRemoveButton("Remove data link", Resources.delete_16);
				IsTermEdited = false;
				CommonFunctionsPanels.ClearTabPagesTitle(termXtraTabControl, base.Edit);
				customFieldsTermForHistory = HistoryCustomFieldsHelper.GetOldCustomFieldsInObjectUserControl(customFields);
			}
			else
			{
				termStatusUserControl.SetDeletedObjectProperties();
			}
			token = new CancellationTokenSource();
			FillControlProgressHighlights();
			SetRichEditControlBackground();
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

	private void SetTermTypes(int? typeId)
	{
		List<TermTypeObject> termTypes = DB.BusinessGlossary.GetTermTypes();
		termTypeLookUpEdit.Properties.DataSource = termTypes;
		termTypeLookUpEdit.Properties.DropDownRows = ((termTypes.Count > 10) ? 10 : termTypes.Count);
		termTypeLookUpEdit.EditValue = typeId;
	}

	private void SetTermBasicData(TermObject termObject)
	{
		base.Subtype = SharedObjectSubtypeEnum.ObjectSubtype.None;
		databaseId = termObject.DatabaseId;
		idEventArgs.DatabaseId = databaseId.Value;
		string text2 = (title = (termTitleTextEdit.Text = termObject.Title));
	}

	private bool IsRefreshRequired(XtraTabPage tabPage, bool additionalCondition = true, bool forceRefresh = false)
	{
		if (!forceRefresh || termXtraTabControl.SelectedTabPage != tabPage)
		{
			if (termXtraTabControl.SelectedTabPage == tabPage && additionalCondition)
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
			title = null;
			databaseId = null;
			databaseTitle = null;
			databaseType = null;
			string text3 = (termTitleTextEdit.Text = (termHtmlUserControl.HtmlText = null));
			idEventArgs.DatabaseId = null;
			CommonFunctionsPanels.SetName(termTextEdit, termDescriptionXtraTabPage, SharedObjectTypeEnum.ObjectType.Term, selectedNode.Subtype, selectedNode.Schema, selectedNode.Name, selectedNode.Title, databaseType, databaseTitle, showDatabaseTitle, withSchema: false, UserTypeEnum.UserType.DBMS, selectedNode.CustomInfo);
			IsTermEdited = false;
			CommonFunctionsPanels.ClearTabPagesTitle(termXtraTabControl, base.Edit);
			if (setDeletedObjectProperties)
			{
				termStatusUserControl.SetDeletedObjectProperties();
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

	private void SetCustomFieldsDataSource()
	{
		CustomFieldsPanelControl.EditValueChanging += delegate
		{
			SetCurrentTabPageTitle(isEdited: true, termDescriptionXtraTabPage);
		};
		customFieldsPanelControl.ShowHistoryClick -= CustomFieldsPanelControl_ShowHistoryClick;
		customFieldsPanelControl.ShowHistoryClick += CustomFieldsPanelControl_ShowHistoryClick;
		IEnumerable<CustomFieldDefinition> customFieldRows = customFields.CustomFieldsData.Where((CustomFieldDefinition x) => x.CustomField?.TermVisibility ?? false);
		customFieldsPanelControl.LoadFields(customFieldRows, delegate
		{
			IsTermEdited = true;
		}, customFieldsLayoutControlItem);
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if (keyData == (Keys.F | Keys.Control) && termHtmlUserControl.PerformFindActionIfFocused())
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
				List<XtraTabPage> list = new List<XtraTabPage>();
				if (!Licenses.CheckRepositoryVersionAfterLogin())
				{
					flag = true;
				}
				else
				{
					if (termRelationshipObjects != null)
					{
						foreach (TermRelationshipObjectExtended termRelationshipObject in termRelationshipObjects)
						{
							termRelationshipObject.IsValidated = true;
						}
						if (termRelationshipObjects.Any((TermRelationshipObjectExtended x) => x.IsInvalid))
						{
							CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
							relatedTermsGridView.Invalidate();
							GeneralMessageBoxesHandling.Show("Some of required parameters are not specified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, null, 1, FindForm());
							return false;
						}
					}
					if (IsTitleValid())
					{
						if (base.UserControlHelpers.IsSearchActive)
						{
							ClearHighlights(keepSearchActive: true);
						}
						TermObject termObject = new TermObject();
						termObject.Initialize();
						termObject.TermId = termId;
						termObject.TypeId = termTypeLookUpEdit.EditValue as int?;
						termObject.Title = termTitleTextEdit.Text;
						termObject.Description = PrepareValue.GetHtmlText(termHtmlUserControl?.PlainText, termHtmlUserControl?.HtmlText);
						termObject.DescriptionPlain = termHtmlUserControl?.PlainText;
						customFieldsPanelControl.SetCustomFieldsValues(termObject);
						customFieldsPanelControl.SetCustomFieldsValues(customFields);
						if (DB.BusinessGlossary.UpdateTerm(termObject, FindForm()))
						{
							TermObject term = DB.BusinessGlossary.GetTerm(termId);
							term.Description = PrepareValue.GetHtmlText(term?.DescriptionPlain, term?.Description);
							termHtmlUserControl.SetHtmlTextAsOriginal();
							bool saveTitle = HistoryGeneralHelper.CheckAreValuesDiffrent(TermObjectCopyHistory?.Title, termTitleTextEdit.Text);
							bool saveDescription = HistoryGeneralHelper.CheckAreHtmlValuesAreDiffrent(TermObjectCopyHistory?.DescriptionPlain, TermObjectCopyHistory?.Description, termObject?.DescriptionPlain, termObject?.Description);
							DB.History.InsertHistoryRow(term.DatabaseId, term.TermId, term.Title, termHtmlUserControl.HtmlText, termHtmlUserControl.PlainText, "glossary_terms", saveTitle, saveDescription, SharedObjectTypeEnum.ObjectType.Term);
							IsTermEdited = false;
							DBTreeMenu.RefeshNodeTitle(ObjectId, termTitleTextEdit.Text, SharedObjectTypeEnum.ObjectType.Term, term.TypeIconId);
							CommonFunctionsPanels.SetName(termTextEdit, termDescriptionXtraTabPage, SharedObjectTypeEnum.ObjectType.Term, base.Subtype, null, termTitleTextEdit.Text, termTitleTextEdit.Text, databaseType, databaseTitle, showDatabaseTitle, withSchema: false, UserTypeEnum.UserType.DBMS, selectedDBTreeNode.CustomInfo);
							customFieldsPanelControl.UpdateDefinitionValues();
							customFields.UpdateCustomFieldDefinitionValues(DB.CustomField.UpdateCustomFieldValues);
							HistoryCustomFieldsHelper.InsertHistoryCustomFieldsOnTablePanel(ObjectId, customFieldsTermForHistory, customFields, databaseId, SharedObjectTypeEnum.ObjectType.Term);
							customFieldsTermForHistory = new Dictionary<string, string>();
							CustomFieldDefinition[] customFieldsData = customFields.CustomFieldsData;
							foreach (CustomFieldDefinition customFieldDefinition in customFieldsData)
							{
								customFieldsTermForHistory.Add(customFieldDefinition.CustomField.FieldName, customFieldDefinition.FieldValue);
							}
							DB.Community.InsertFollowingToRepository(SharedObjectTypeEnum.ObjectType.Term, termObject.TermId);
							WorkWithDataedoTrackingHelper.TrackFirstInSessionTermEdit();
							TermObjectCopyHistory = new TermObject
							{
								TermId = termObject.TermId,
								Title = termObject.Title,
								Description = PrepareValue.GetHtmlText(termHtmlUserControl?.PlainText, termHtmlUserControl?.HtmlText),
								DescriptionPlain = termObject?.DescriptionPlain
							};
							termHtmlUserControl.TermObject = term;
						}
						else
						{
							flag = true;
						}
					}
					else
					{
						CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
						GeneralMessageBoxesHandling.Show("Title of the term can't be empty", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, FindForm());
						flag = true;
					}
					if (!flag && UpdateTermRelationships())
					{
						SetTabPageTitle(isEdited: false, relatedTermsXtraTabPage);
					}
					else if (!flag)
					{
						flag = true;
					}
					if (!flag && UpdateDataLinks())
					{
						SetTabPageTitle(isEdited: false, dataLinksXtraTabPage);
					}
					else if (!flag)
					{
						flag = true;
					}
					if (base.MainControl.ProgressType.Type != ProgressTypeEnum.TablesAndColumns)
					{
						base.MainControl.RefreshObjectProgress(showWaitForm: false, ObjectId, SharedObjectTypeEnum.ObjectType.Term);
					}
					if (!flag)
					{
						base.Edit.SetUnchanged();
						RefreshModules();
						if (base.UserControlHelpers.IsSearchActive)
						{
							base.MainControl.OpenCurrentlySelectedSearchRow();
							termHtmlUserControl.SetNotChanged();
							if (termHtmlUserControl.Highlight())
							{
								base.UserControlHelpers.SetHighlight();
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
			SetMetadata(DB.BusinessGlossary.GetTerm(termId));
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

	private bool IsTitleValid()
	{
		return ValidateFields.IsEditNotEmptyRaiseError(termTitleTextEdit, termTitleErrorProvider);
	}

	public override void ReloadGridsData()
	{
		RefreshRelatedTerms(forceRefresh: true);
		RefreshDataLinks(forceRefresh: true);
	}

	private void SetMetadata(TermObject termObject)
	{
		createdLabelControl.Text = PrepareValue.SetDateTimeWithFormatting(termObject.CreationDate) + " " + termObject.CreatedBy;
		lastUpdatedLabel.Text = PrepareValue.SetDateTimeWithFormatting(termObject.LastModificationDate) + " " + PrepareValue.ToString(termObject.ModifiedBy);
		if (termXtraTabControl.SelectedTabPage == termMetadataXtraTabPage)
		{
			termMetadataXtraTabPage.Invalidate();
		}
	}

	private void TermXtraTabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		if (termXtraTabControl.SelectedTabPageIndex == 0)
		{
			SelectedTab.selectedTabCaption = "info";
		}
		else
		{
			SelectedTab.selectedTabCaption = termXtraTabControl.SelectedTabPage.Text;
		}
		RefreshRelatedTerms();
		RefreshDataLinks();
		HideCustomization();
	}

	private void AddEvents()
	{
		List<ToolTipData> list = new List<ToolTipData>();
		list.Add(new ToolTipData(relatedTermsGrid, SharedObjectTypeEnum.ObjectType.Term, relatedTermTitleGridColumn.VisibleIndex));
		list.Add(new ToolTipData(relatedTermsGrid, SharedObjectTypeEnum.ObjectType.Term, relatedTermIconGridColumn.VisibleIndex));
		CommonFunctionsPanels.AddEventForAutoFilterRow(relatedTermsGridView);
		list.Add(new ToolTipData(dataLinksGrid, SharedObjectTypeEnum.ObjectType.Table, iconDataLinksGridColumn.VisibleIndex));
		CommonFunctionsPanels.AddEventsForToolTips(termToolTipController, list);
		CommonFunctionsPanels.AddEventForAutoFilterRow(dataLinksGridView);
	}

	private void TermHtmlUserControl_PreviewKeyDown(object sender, EventArgs e)
	{
		IsTermEdited = true;
	}

	private void TermTitleTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValidateFields.IsEditNotEmptyRaiseError(termTitleTextEdit, termTitleErrorProvider);
		IsTermEdited = true;
	}

	private void TermTypeLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		IsTermEdited = true;
	}

	private void TermModuleLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		IsTermEdited = true;
	}

	public void RefreshRelatedTerms(bool forceRefresh = false, bool refreshImmediatelyIfNotLoaded = false)
	{
		if (IsRefreshRequired(relatedTermsXtraTabPage, !isRelatedTermDataLoaded, forceRefresh) || (refreshImmediatelyIfNotLoaded && !isRelatedTermDataLoaded))
		{
			base.MainControl.SetWaitformVisibility(visible: true);
			termRelationshipObjects = new BindingList<TermRelationshipObjectExtended>(DB.BusinessGlossary.GetTermRelationships(termId));
			int num = relationshipTypeGridColumn.Width;
			relatedTermsGridView.LeftCoord = 0;
			relatedTermsGrid.DataSource = termRelationshipObjects;
			CommonFunctionsPanels.SetBestFitForColumns(relatedTermsGridView);
			relationshipTypeGridColumn.Width = ((relationshipTypeGridColumn.Width < num) ? num : relationshipTypeGridColumn.Width);
			FillHighlights(SharedObjectTypeEnum.ObjectType.TermRelationship);
			isRelatedTermDataLoaded = true;
			base.MainControl.SetWaitformVisibility(visible: false);
			relatedTermsGridPanelUserControl.SetRemoveButtonVisibility(relatedTermsGridView.SelectedRowsCount > 0);
		}
		else if (forceRefresh)
		{
			isRelatedTermDataLoaded = false;
		}
	}

	public bool UpdateTermRelationships()
	{
		try
		{
			DB.BusinessGlossary.ProcessSavingTermsRelationships(termRelationshipObjects);
			return true;
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			GeneralExceptionHandling.Handle(exception, "Error while updating relationships", FindForm());
			return false;
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
		}
	}

	private void AddRelatedTermBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		base.MainControl.BusinessGlossarySupport.StartAddingTermRelatedTerm(base.MainControl.TreeListHelpers.GetFocusedTreeListNode(), fromCustomFocus: false, FindForm());
	}

	private void RelatedTermsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		relatedTermsGridPanelUserControl.SetRemoveButtonVisibility(relatedTermsGridView.SelectedRowsCount > 0);
	}

	private void RelatedTermsGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
		relatedTermsPopupMenu.ShowPopupMenu(sender, e);
	}

	private void relatedTermsPopupMenu_BeforePopup(object sender, CancelEventArgs e)
	{
		if (relatedTermsGridView.IsFilterRow(relatedTermsGridView.FocusedRowHandle) || Grids.GetBeforePopupShouldCancel(sender))
		{
			e.Cancel = true;
			return;
		}
		bool beforePopupIsRowClicked = Grids.GetBeforePopupIsRowClicked(sender);
		removeRelatedTermsBarButtonItem.Visibility = ((!beforePopupIsRowClicked) ? BarItemVisibility.Never : BarItemVisibility.Always);
		goToObjectRelatedTermsBarButtonItem.Visibility = ((!beforePopupIsRowClicked) ? BarItemVisibility.Never : BarItemVisibility.Always);
		if (beforePopupIsRowClicked)
		{
			if (relatedTermsGridView.GetFocusedRow() is IGoTo goTo)
			{
				goToObjectRelatedTermsBarButtonItem.Caption = "Go to " + Escaping.EscapeTextForUI(goTo.FullName);
				goToObjectRelatedTermsBarButtonItem.ImageOptions.Image = BusinessGlossarySupport.GetTermIcon((goTo as TermRelationshipObjectExtended)?.RelatedTermTypeIconId);
			}
			else
			{
				goToObjectRelatedTermsBarButtonItem.Visibility = BarItemVisibility.Never;
			}
		}
	}

	private void editTermRelationshipsRelatedTermsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ProcessAddingRelatedTerms(null);
	}

	private void goToObjectRelatedTermsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (base.MainControl.ContinueAfterPossibleChanges())
		{
			base.MainControl.TreeListHelpers.OpenNode(relatedTermsGridView.GetFocusedRow() as IGoTo);
		}
	}

	private void removeRelatedTermsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ProcessDeletingRelatedTerms();
	}

	private void RelatedTermsGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		SetTabPageTitle(isEdited: true, relatedTermsXtraTabPage, base.Edit);
		(relatedTermsGridView.GetFocusedRow() as TermRelationshipObjectExtended).IsModified = true;
	}

	private void RelationshipTypeRepositoryItemLookUpEdit_Closed(object sender, ClosedEventArgs e)
	{
		relatedTermsGridView.CloseEditor();
	}

	private void RelatedTermsGridView_CustomRowFilter(object sender, RowFilterEventArgs e)
	{
		if (termRelationshipObjects.Count > e.ListSourceRow)
		{
			TermRelationshipObjectExtended termRelationshipObjectExtended = termRelationshipObjects[e.ListSourceRow];
			if (termRelationshipObjectExtended != null && termRelationshipObjectExtended.IsDeleted)
			{
				e.Visible = false;
				e.Handled = true;
			}
		}
	}

	private void RelatedTermsGridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		if (e.Column.Equals(relatedTermIconGridColumn))
		{
			TermRelationshipObject termRelationshipObject = e.Row as TermRelationshipObject;
			e.Value = BusinessGlossarySupport.GetTermIcon(termRelationshipObject.RelatedTermTypeIconId);
		}
	}

	private void ProcessDeletingRelatedTerms()
	{
		if (CommonFunctionsDatabase.AskIfDeleting(relatedTermsGridView.GetSelectedRows(), relatedTermsGridView, SharedObjectTypeEnum.ObjectType.TermRelationship))
		{
			int[] selectedRows = relatedTermsGridView.GetSelectedRows();
			foreach (int rowHandle in selectedRows)
			{
				(relatedTermsGridView.GetRow(rowHandle) as TermRelationshipObjectExtended).IsDeleted = true;
			}
			relatedTermsGridView.RefreshData();
			SetTabPageTitle(isEdited: true, relatedTermsXtraTabPage, base.Edit);
		}
	}

	private void ProcessAddingRelatedTerms(int? draggedId)
	{
		base.MainControl.BusinessGlossarySupport.StartAddingTermRelatedTerm(base.MainControl.TreeListHelpers.GetFocusedTreeListNode(), draggedId, fromCustomFocus: false, FindForm());
	}

	private void RelatedTermsGridView_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		if (e.RowHandle == -2147483646)
		{
			RelatedTermsGridView_CustomRowCellEditForAutoFilterRow(sender, e);
		}
		else
		{
			if (termRelationshipObjects == null || termRelationshipTypes == null)
			{
				return;
			}
			TermRelationshipObjectExtended termObject = relatedTermsGridView.GetRow(e.RowHandle) as TermRelationshipObjectExtended;
			if (termObject == null)
			{
				return;
			}
			BindingList<TermRelationshipObjectExtended> source = termRelationshipObjects;
			IEnumerable<int> currentlyAddedRelationshipTypes = from x in source
				where x.RelatedTermId == termObject.RelatedTermId && x.RelationshipTypeId.HasValue && !x.IsDeleted
				select x.RelationshipTypeId.Value;
			if (!(e.Column.FieldName == "RelationshipType") || termRelationshipTypes == null)
			{
				return;
			}
			IEnumerable<TermRelationshipTypeObject> datasource = termRelationshipTypes.Where((TermRelationshipTypeObject x) => x.TypeId == termObject.RelationshipTypeId || !currentlyAddedRelationshipTypes.Any((int y) => y == x.TypeId));
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = AddTermRelationshipsForm.CreateRelationshipTypeRepositoryItemLookUpEdit(dataLinksGridView, datasource);
			repositoryItemLookUpEdit.Closed += RelationshipTypeRepositoryItemLookUpEdit_Closed;
			e.RepositoryItem = repositoryItemLookUpEdit;
		}
	}

	private void RelatedTermsGridView_CustomRowCellEditForAutoFilterRow(object sender, CustomRowCellEditEventArgs e)
	{
		if (!(e.Column.FieldName == "RelationshipType") || termRelationshipTypes == null)
		{
			return;
		}
		List<TermRelationshipTypeObject> datasource = termRelationshipTypes.Where((TermRelationshipTypeObject x) => termRelationshipObjects.Where((TermRelationshipObjectExtended y) => !y.IsDeleted).Any((TermRelationshipObjectExtended y) => x.TypeId == y.RelationshipType.TypeId && x.IsReverse == y.RelationshipType.IsReverse)).ToList();
		e.RepositoryItem = AddTermRelationshipsForm.CreateRelationshipTypeRepositoryItemLookUpEdit(dataLinksGridView, datasource);
	}

	private void RelatedTermsGrid_DragEnter(object sender, DragEventArgs e)
	{
		if (CheckWhetherIsValidRelationDragDrop(e))
		{
			e.Effect = DragDropEffects.Link;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void RelatedTermsGrid_DragOver(object sender, DragEventArgs e)
	{
		if (CheckWhetherIsValidRelationDragDrop(e))
		{
			e.Effect = DragDropEffects.Link;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void RelatedTermsGrid_DragDrop(object sender, DragEventArgs e)
	{
		DBTreeNode dBTreeNode = base.MainControl.TreeListHelpers.GetDBTreeNode(e.Data);
		if (CheckWhetherIsValidRelationDragDrop(dBTreeNode))
		{
			ProcessAddingRelatedTerms(dBTreeNode.Id);
		}
	}

	private bool CheckWhetherIsValidRelationDragDrop(DragEventArgs e)
	{
		DBTreeNode dBTreeNode = base.MainControl.TreeListHelpers.GetDBTreeNode(e.Data);
		return CheckWhetherIsValidRelationDragDrop(dBTreeNode);
	}

	private bool CheckWhetherIsValidRelationDragDrop(DBTreeNode dbTreeNode)
	{
		if (dbTreeNode == null)
		{
			return false;
		}
		if (dbTreeNode.ObjectType == SharedObjectTypeEnum.ObjectType.Term && dbTreeNode.Id != ObjectId)
		{
			return true;
		}
		return false;
	}

	public void RefreshDataLinks(bool forceRefresh = false)
	{
		if (IsRefreshRequired(dataLinksXtraTabPage, !isDataLinksDataLoaded, forceRefresh))
		{
			base.MainControl.SetWaitformVisibility(visible: true);
			objectsWithDataLink = new BindingList<DataLinkObjectExtended>(DB.BusinessGlossary.GetDataLinks<DataLinkObjectExtended>(termId, applyWithShowSchemaOrdering: true));
			dataLinksGrid.BeginUpdate();
			dataLinksGridView.LeftCoord = 0;
			dataLinksGrid.DataSource = objectsWithDataLink;
			dataLinksGrid.EndUpdate();
			CommonFunctionsPanels.SetBestFitForColumns(dataLinksGridView);
			FillHighlights(SharedObjectTypeEnum.ObjectType.TermRelationship);
			isDataLinksDataLoaded = true;
			base.MainControl.SetWaitformVisibility(visible: false);
			dataLinksGridPanelUserControl.SetRemoveButtonVisibility(dataLinksGridView.SelectedRowsCount > 0);
		}
		else if (forceRefresh)
		{
			isDataLinksDataLoaded = false;
		}
	}

	public void SetLinksDataIfNotModified()
	{
		if (isDataLinksDataLoaded && !objectsWithDataLink.Any((DataLinkObjectExtended x) => x.IsModified))
		{
			if (termXtraTabControl.SelectedTabPage == dataLinksXtraTabPage)
			{
				RefreshDataLinks(forceRefresh: true);
			}
			else
			{
				isDataLinksDataLoaded = false;
			}
		}
	}

	public bool UpdateDataLinks()
	{
		try
		{
			DB.BusinessGlossary.ProcessSavingDataLinks(objectsWithDataLink);
			List<DataLinkObjectExtended> list = objectsWithDataLink.Where((DataLinkObjectExtended x) => !x.IsDeleted).ToList();
			list.ForEach(delegate(DataLinkObjectExtended x)
			{
				x.SetAsCurrentlyAdded();
			});
			objectsWithDataLink = new BindingList<DataLinkObjectExtended>(list);
			return true;
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			GeneralExceptionHandling.Handle(exception, "Error while updating data links", FindForm());
			return false;
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
		}
	}

	private void AddLinkBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (base.MainControl.DataLinksSupport.StartAddingDataLink(base.MainControl.TreeListHelpers.GetFocusedTreeListNode(), fromCustomFocus: false, FindForm()) == DialogResult.OK)
		{
			RefreshDataLinks(forceRefresh: true);
		}
	}

	private void DataLinksGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		dataLinksGridPanelUserControl.SetRemoveButtonVisibility(dataLinksGridView.SelectedRowsCount > 0);
	}

	private void DataLinksGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
		linkedDataPopupMenu.ShowPopupMenu(sender, e);
	}

	private void linkedDataPopupMenu_BeforePopup(object sender, CancelEventArgs e)
	{
		if (dataLinksGridView.IsFilterRow(dataLinksGridView.FocusedRowHandle) || Grids.GetBeforePopupShouldCancel(sender))
		{
			e.Cancel = true;
			return;
		}
		bool beforePopupIsRowClicked = Grids.GetBeforePopupIsRowClicked(sender);
		removeLinkedDataBarButtonItem.Visibility = ((!beforePopupIsRowClicked) ? BarItemVisibility.Never : BarItemVisibility.Always);
		goToObjectLinkedDataBarButtonItem.Visibility = ((!beforePopupIsRowClicked) ? BarItemVisibility.Never : BarItemVisibility.Always);
		if (beforePopupIsRowClicked)
		{
			if (dataLinksGridView.GetFocusedRow() is IGoTo goTo)
			{
				goToObjectLinkedDataBarButtonItem.Caption = "Go to " + Escaping.EscapeTextForUI(goTo.FullName);
				DataLinkObjectExtended dataLinkObjectExtended = goTo as DataLinkObjectExtended;
				goToObjectLinkedDataBarButtonItem.ImageOptions.Image = IconsSupport.GetObjectIcon(dataLinkObjectExtended.TypeForIcon, dataLinkObjectExtended.SubtypeForIcon, dataLinkObjectExtended.SourceForIcon);
			}
			else
			{
				goToObjectLinkedDataBarButtonItem.Visibility = BarItemVisibility.Never;
			}
		}
	}

	private void editLinksLinkedDataBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ProcessAddingDataLink(null);
	}

	private void goToObjectLinkedDataBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		if (!base.MainControl.ContinueAfterPossibleChanges())
		{
			return;
		}
		base.MainControl.TreeListHelpers.OpenNode(dataLinksGridView.GetFocusedRow() as IGoTo);
		if (dataLinksGridView.GetFocusedRow() is DataLinkObject dataLinkObject && dataLinkObject.ElementId.HasValue)
		{
			BasePanelControl visibleUserControl = base.MainControl.GetVisibleUserControl();
			(visibleUserControl as ITabChangable)?.ChangeTab(SharedObjectTypeEnum.ObjectType.Column);
			if (visibleUserControl is TableUserControl)
			{
				(visibleUserControl as TableUserControl).FocusColumn(dataLinkObject.ElementId.Value);
			}
		}
	}

	private void removeLinkedDataBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ProcessDeletingLink();
	}

	private void DataLinksGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
	}

	private void DataLinksGridView_CustomRowFilter(object sender, RowFilterEventArgs e)
	{
		DataLinkObjectExtended dataLinkObjectExtended = objectsWithDataLink[e.ListSourceRow];
		if (dataLinkObjectExtended != null && dataLinkObjectExtended.IsDeleted)
		{
			e.Visible = false;
			e.Handled = true;
		}
	}

	private void ProcessDeletingLink()
	{
		if (CommonFunctionsDatabase.AskIfDeleting(dataLinksGridView.GetSelectedRows(), dataLinksGridView, SharedObjectTypeEnum.ObjectType.DataLink))
		{
			int[] selectedRows = dataLinksGridView.GetSelectedRows();
			foreach (int rowHandle in selectedRows)
			{
				DataLinkObjectExtended obj = dataLinksGridView.GetRow(rowHandle) as DataLinkObjectExtended;
				obj.IsModified = true;
				obj.IsSelected = false;
			}
			dataLinksGridView.RefreshData();
			SetTabPageTitle(isEdited: true, dataLinksXtraTabPage, base.Edit);
		}
	}

	private void ProcessAddingDataLink(int? draggedId)
	{
		if (base.MainControl.DataLinksSupport.StartAddingDataLink(base.MainControl.TreeListHelpers.GetFocusedTreeListNode(), fromCustomFocus: false, FindForm()) == DialogResult.OK)
		{
			RefreshRelatedTerms(forceRefresh: true);
		}
	}

	private void ProcessAddingDataLink(DBTreeNode targetDBNode, DBTreeNode draggedDBNode)
	{
		_ = targetDBNode.Id;
		_ = string.Empty;
		_ = draggedDBNode.DatabaseId;
		_ = targetDBNode.DatabaseId;
		string text = draggedDBNode.DatabaseTitle + "." + draggedDBNode.Name;
		_ = targetDBNode.BaseName;
		if (CustomMessageBoxForm.Show("Do you want to add link to <b>" + text + "</b> to <b>" + targetDBNode.DisplayNameWithShema + "</b>?", "Add link", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes && DB.BusinessGlossary.InsertDataLink(new DataLinkObjectBase(targetDBNode.Id, SharedObjectTypeEnum.TypeToString(draggedDBNode.ObjectType), draggedDBNode.Id, null)))
		{
			RefreshDataLinks(forceRefresh: true);
		}
	}

	private void DataLinksGridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		if (e.Column.Equals(iconDataLinksGridColumn))
		{
			DataLinkObjectExtended dataLinkObjectExtended = e.Row as DataLinkObjectExtended;
			e.Value = IconsSupport.GetObjectIcon(dataLinkObjectExtended.TypeForIcon, dataLinkObjectExtended.SubtypeForIcon, dataLinkObjectExtended.SourceForIcon, dataLinkObjectExtended.ObjectStatus);
		}
	}

	private void DataLinksGrid_DragEnter(object sender, DragEventArgs e)
	{
		if (CheckWhetherIsValidDataLinkDragDrop(e))
		{
			e.Effect = DragDropEffects.Link;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void DataLinksGrid_DragOver(object sender, DragEventArgs e)
	{
		if (CheckWhetherIsValidDataLinkDragDrop(e))
		{
			e.Effect = DragDropEffects.Link;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void DataLinksGrid_DragDrop(object sender, DragEventArgs e)
	{
		DBTreeNode dBTreeNode = base.MainControl.TreeListHelpers.GetDBTreeNode(e.Data);
		if (CheckWhetherIsValidDataLinkDragDrop(dBTreeNode))
		{
			ProcessAddingDataLink(selectedDBTreeNode, dBTreeNode);
		}
	}

	private bool CheckWhetherIsValidDataLinkDragDrop(DragEventArgs e)
	{
		DBTreeNode dBTreeNode = base.MainControl.TreeListHelpers.GetDBTreeNode(e.Data);
		return CheckWhetherIsValidDataLinkDragDrop(dBTreeNode);
	}

	private bool CheckWhetherIsValidDataLinkDragDrop(DBTreeNode dbTreeNode)
	{
		if (dbTreeNode == null)
		{
			return false;
		}
		return MatchingDragDropTypes.IsValidObjectToTermDragDrop(dbTreeNode.ObjectType);
	}

	private void RelatedTermsGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (relatedTermsGridView.GetRow(e.RowHandle) is TermRelationshipObjectExtended termRelationshipObjectExtended && termRelationshipObjectExtended.IsValidated && termRelationshipObjectExtended.IsInvalid && termRelationshipObjectExtended.IsModified && e.Column.FieldName == "RelationshipType")
		{
			GridCellInfo obj = e.Cell as GridCellInfo;
			obj.ViewInfo.ErrorIconText = "Relationship type is required";
			obj.ViewInfo.ShowErrorIcon = true;
			obj.ViewInfo.CalcViewInfo(e.Graphics);
		}
		if (e.Column != relatedTermIconGridColumn && GridColumnsHelper.ShouldColumnBeGrayOut(e.Column) && (!relatedTermsGridView.IsFocusedView || !relatedTermsGridView.GetSelectedRows().Contains(e.RowHandle)))
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridNonEditableColumnsBackColor;
		}
	}

	private void DataLinksGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (e.Column != iconDataLinksGridColumn && GridColumnsHelper.ShouldColumnBeGrayOut(e.Column) && (!dataLinksGridView.IsFocusedView || !dataLinksGridView.GetSelectedRows().Contains(e.RowHandle)))
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridNonEditableColumnsBackColor;
		}
	}

	private void dataLinksGridView_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		if (e.Column == columnDataLinksGridColumn)
		{
			string elementFullName = (e.RowObject1 as DataLinkObject).ElementFullName;
			string elementFullName2 = (e.RowObject2 as DataLinkObject).ElementFullName;
			e.Result = elementFullName.CompareTo(elementFullName2);
			e.Handled = true;
		}
	}

	private void TermTitleTextEdit_Properties_BeforeShowMenu(object sender, BeforeShowMenuEventArgs e)
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
		try
		{
			using HistoryForm historyForm = new HistoryForm();
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
			historyForm.SetParameters(termId, "title", title, ObjectSchema, base.DatabaseShowSchema, base.DatabaseShowSchemaOverride, null, "glossary_terms", SharedObjectTypeEnum.TypeToString(ObjectType), TermObjectCopyHistory.TypeTitle, null, BusinessGlossarySupport.GetTermIcon(TermObjectCopyHistory.TypeIconId));
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			historyForm.ShowDialog();
		}
		catch (Exception exception)
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: false);
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	public void ViewHistoryForField(string fieldName)
	{
		try
		{
			using HistoryForm historyForm = new HistoryForm();
			CommonFunctionsDatabase.SetWaitFormVisibility(GetSplashScreenManager(), show: true);
			historyForm.CustomFieldCaption = customFields?.CustomFieldsData?.Where((CustomFieldDefinition x) => x.CustomField.FieldName.ToLower() == fieldName)?.FirstOrDefault()?.CustomField?.Title;
			historyForm.SetParameters(ObjectId, fieldName, ObjectName, ObjectSchema, base.DatabaseShowSchema, base.DatabaseShowSchemaOverride, title, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(ObjectType), SharedObjectTypeEnum.TypeToString(ObjectType), SharedObjectSubtypeEnum.TypeToString(ObjectType, base.Subtype), null, null);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.PanelControls.TermUserControl));
		Dataedo.App.Tools.DefaultBulkCopy defaultBulkCopy = new Dataedo.App.Tools.DefaultBulkCopy();
		Dataedo.App.Tools.DefaultBulkCopy defaultBulkCopy2 = new Dataedo.App.Tools.DefaultBulkCopy();
		this.termXtraTabControl = new DevExpress.XtraTab.XtraTabControl();
		this.termDescriptionXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.termLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.termTypeLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.documentationTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.customFieldsPanelControl = new Dataedo.App.UserControls.CustomFieldsPanelControl();
		this.termHtmlUserControl = new Dataedo.App.UserControls.HtmlUserControl();
		this.termTitleTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.searchCountLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
		this.customFieldsLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.documentationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.typeLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.relatedTermsXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.relatedTermsGrid = new DevExpress.XtraGrid.GridControl();
		this.relatedTermsGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.relationshipTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.relationshipTypeRepositoryItemLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.relatedTermIconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconTableRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.relatedTermTitleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.relatedTermTypeTableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.commentsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.commentsRepositoryItemAutoHeightMemoEdit = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.createdGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedByGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.termToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.relatedTermsGridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.dataLinksXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.dataLinksGrid = new DevExpress.XtraGrid.GridControl();
		this.dataLinksGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.iconDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.tableDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.columnDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemAutoHeightMemoEdit1 = new Dataedo.App.UserControls.RepositoryItemAutoHeightMemoEdit();
		this.documentationDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.createdByDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.lastUpdatedByDataLinksGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dataLinksGridPanelUserControl = new Dataedo.App.UserControls.PanelControls.GridPanelUserControl();
		this.termMetadataXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.metadataLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.lastUpdatedLabel = new DevExpress.XtraEditors.LabelControl();
		this.createdLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.lastUpdatedLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.firstImportedLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.metadataToolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.termStatusUserControl = new Dataedo.App.UserControls.InfoUserControl();
		this.termTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.termTitleErrorProvider = new DevExpress.XtraEditors.DXErrorProvider.DXErrorProvider(this.components);
		this.relatedTermsPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.editTermRelationshipsRelatedTermsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.goToObjectRelatedTermsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeRelatedTermsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.relatedTermsBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.linkedDataPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.editLinksLinkedDataBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.goToObjectLinkedDataBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeLinkedDataBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.linkedDataBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControl1 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl2 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl3 = new DevExpress.XtraBars.BarDockControl();
		this.barDockControl4 = new DevExpress.XtraBars.BarDockControl();
		((System.ComponentModel.ISupportInitialize)this.termXtraTabControl).BeginInit();
		this.termXtraTabControl.SuspendLayout();
		this.termDescriptionXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.termLayoutControl).BeginInit();
		this.termLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.termTypeLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.documentationTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.termTitleTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.documentationLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		this.relatedTermsXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.relatedTermsGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.relatedTermsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.relationshipTypeRepositoryItemLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconTableRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.commentsRepositoryItemAutoHeightMemoEdit).BeginInit();
		this.dataLinksXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dataLinksGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataLinksGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemAutoHeightMemoEdit1).BeginInit();
		this.termMetadataXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.metadataLayoutControl).BeginInit();
		this.metadataLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lastUpdatedLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.firstImportedLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.termTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.termTitleErrorProvider).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.relatedTermsPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.relatedTermsBarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.linkedDataPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.linkedDataBarManager).BeginInit();
		base.SuspendLayout();
		this.termXtraTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.termXtraTabControl.Location = new System.Drawing.Point(0, 60);
		this.termXtraTabControl.Name = "termXtraTabControl";
		this.termXtraTabControl.SelectedTabPage = this.termDescriptionXtraTabPage;
		this.termXtraTabControl.Size = new System.Drawing.Size(814, 494);
		this.termXtraTabControl.TabIndex = 1;
		this.termXtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[4] { this.termDescriptionXtraTabPage, this.relatedTermsXtraTabPage, this.dataLinksXtraTabPage, this.termMetadataXtraTabPage });
		this.termXtraTabControl.ToolTipController = this.toolTipController;
		this.termXtraTabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(TermXtraTabControl_SelectedPageChanged);
		this.termDescriptionXtraTabPage.Controls.Add(this.termLayoutControl);
		this.termDescriptionXtraTabPage.Name = "termDescriptionXtraTabPage";
		this.termDescriptionXtraTabPage.Size = new System.Drawing.Size(812, 465);
		this.termDescriptionXtraTabPage.Text = "Term";
		this.termLayoutControl.AllowCustomization = false;
		this.termLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.termLayoutControl.Controls.Add(this.labelControl1);
		this.termLayoutControl.Controls.Add(this.termTypeLookUpEdit);
		this.termLayoutControl.Controls.Add(this.documentationTextEdit);
		this.termLayoutControl.Controls.Add(this.customFieldsPanelControl);
		this.termLayoutControl.Controls.Add(this.termHtmlUserControl);
		this.termLayoutControl.Controls.Add(this.termTitleTextEdit);
		this.termLayoutControl.Controls.Add(this.searchCountLabelControl);
		this.termLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.termLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.termLayoutControl.Name = "termLayoutControl";
		this.termLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1201, 247, 637, 770);
		this.termLayoutControl.Root = this.layoutControlGroup1;
		this.termLayoutControl.Size = new System.Drawing.Size(812, 465);
		this.termLayoutControl.TabIndex = 4;
		this.termLayoutControl.Text = "layoutControl1";
		this.labelControl1.Location = new System.Drawing.Point(12, 120);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(53, 13);
		this.labelControl1.StyleController = this.termLayoutControl;
		this.labelControl1.TabIndex = 11;
		this.labelControl1.Text = "Description";
		this.termTypeLookUpEdit.Location = new System.Drawing.Point(97, 62);
		this.termTypeLookUpEdit.MaximumSize = new System.Drawing.Size(0, 22);
		this.termTypeLookUpEdit.MinimumSize = new System.Drawing.Size(0, 22);
		this.termTypeLookUpEdit.Name = "termTypeLookUpEdit";
		this.termTypeLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.termTypeLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Title", "Title")
		});
		this.termTypeLookUpEdit.Properties.DisplayMember = "Title";
		this.termTypeLookUpEdit.Properties.DropDownRows = 4;
		this.termTypeLookUpEdit.Properties.NullText = "";
		this.termTypeLookUpEdit.Properties.ShowFooter = false;
		this.termTypeLookUpEdit.Properties.ShowHeader = false;
		this.termTypeLookUpEdit.Properties.ShowLines = false;
		this.termTypeLookUpEdit.Properties.ValueMember = "TermTypeId";
		this.termTypeLookUpEdit.Size = new System.Drawing.Size(411, 22);
		this.termTypeLookUpEdit.StyleController = this.termLayoutControl;
		this.termTypeLookUpEdit.TabIndex = 9;
		this.termTypeLookUpEdit.EditValueChanged += new System.EventHandler(TermTypeLookUpEdit_EditValueChanged);
		this.documentationTextEdit.Location = new System.Drawing.Point(94, 12);
		this.documentationTextEdit.Name = "documentationTextEdit";
		this.documentationTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.documentationTextEdit.Properties.ReadOnly = true;
		this.documentationTextEdit.Size = new System.Drawing.Size(706, 18);
		this.documentationTextEdit.StyleController = this.termLayoutControl;
		this.documentationTextEdit.TabIndex = 2;
		this.documentationTextEdit.TabStop = false;
		this.customFieldsPanelControl.BackColor = System.Drawing.Color.Transparent;
		this.customFieldsPanelControl.Location = new System.Drawing.Point(10, 88);
		this.customFieldsPanelControl.Margin = new System.Windows.Forms.Padding(0);
		this.customFieldsPanelControl.Name = "customFieldsPanelControl";
		this.customFieldsPanelControl.Size = new System.Drawing.Size(790, 28);
		this.customFieldsPanelControl.TabIndex = 7;
		this.termHtmlUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.termHtmlUserControl.CanListen = false;
		this.termHtmlUserControl.Font = new System.Drawing.Font("Segoe UI", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.termHtmlUserControl.HtmlText = resources.GetString("termHtmlUserControl.HtmlText");
		this.termHtmlUserControl.IsHighlighted = false;
		this.termHtmlUserControl.Location = new System.Drawing.Point(12, 137);
		this.termHtmlUserControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.termHtmlUserControl.Name = "termHtmlUserControl";
		this.termHtmlUserControl.OccurrencesCount = 0;
		this.termHtmlUserControl.OriginalHtmlText = null;
		this.termHtmlUserControl.PlainText = "\u00a0";
		this.termHtmlUserControl.Size = new System.Drawing.Size(788, 326);
		this.termHtmlUserControl.SplashScreenManager = null;
		this.termHtmlUserControl.TabIndex = 8;
		this.termHtmlUserControl.TableRow = null;
		this.termHtmlUserControl.TermObject = null;
		this.termTitleTextEdit.AllowHtmlTextInToolTip = DevExpress.Utils.DefaultBoolean.False;
		this.termTitleTextEdit.Location = new System.Drawing.Point(97, 36);
		this.termTitleTextEdit.MaximumSize = new System.Drawing.Size(0, 22);
		this.termTitleTextEdit.MinimumSize = new System.Drawing.Size(0, 22);
		this.termTitleTextEdit.Name = "termTitleTextEdit";
		this.termTitleTextEdit.Properties.BeforeShowMenu += new DevExpress.XtraEditors.Controls.BeforeShowMenuEventHandler(TermTitleTextEdit_Properties_BeforeShowMenu);
		this.termTitleTextEdit.Size = new System.Drawing.Size(411, 22);
		this.termTitleTextEdit.StyleController = this.termLayoutControl;
		this.termTitleTextEdit.TabIndex = 5;
		this.termTitleTextEdit.EditValueChanged += new System.EventHandler(TermTitleTextEdit_EditValueChanged);
		this.searchCountLabelControl.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.searchCountLabelControl.Appearance.Options.UseBackColor = true;
		this.searchCountLabelControl.Location = new System.Drawing.Point(69, 120);
		this.searchCountLabelControl.Name = "searchCountLabelControl";
		this.searchCountLabelControl.Size = new System.Drawing.Size(731, 13);
		this.searchCountLabelControl.StyleController = this.termLayoutControl;
		this.searchCountLabelControl.TabIndex = 11;
		this.layoutControlGroup1.CustomizationFormText = "Root";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[9] { this.layoutControlItem3, this.emptySpaceItem2, this.emptySpaceItem1, this.layoutControlItem10, this.customFieldsLayoutControlItem, this.documentationLayoutControlItem, this.typeLayoutControlItem, this.layoutControlItem4, this.layoutControlItem5 });
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 10, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(812, 465);
		this.layoutControlGroup1.TextVisible = false;
		this.layoutControlItem3.Control = this.termTitleTextEdit;
		this.layoutControlItem3.CustomizationFormText = "Title:";
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(500, 26);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(500, 26);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(500, 26);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.Text = "Title";
		this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(82, 13);
		this.layoutControlItem3.TextToControlDistance = 3;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem2.Location = new System.Drawing.Point(500, 50);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(292, 26);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
		this.emptySpaceItem1.Location = new System.Drawing.Point(500, 24);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(292, 26);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem10.Control = this.termHtmlUserControl;
		this.layoutControlItem10.Location = new System.Drawing.Point(0, 125);
		this.layoutControlItem10.MinSize = new System.Drawing.Size(104, 300);
		this.layoutControlItem10.Name = "layoutControlItem10";
		this.layoutControlItem10.Size = new System.Drawing.Size(792, 330);
		this.layoutControlItem10.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem10.Text = "Description";
		this.layoutControlItem10.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem10.TextVisible = false;
		this.customFieldsLayoutControlItem.Control = this.customFieldsPanelControl;
		this.customFieldsLayoutControlItem.Location = new System.Drawing.Point(0, 76);
		this.customFieldsLayoutControlItem.Name = "customFieldsLayoutControlItem";
		this.customFieldsLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 2, 2, 2);
		this.customFieldsLayoutControlItem.Size = new System.Drawing.Size(792, 32);
		this.customFieldsLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.customFieldsLayoutControlItem.TextVisible = false;
		this.documentationLayoutControlItem.Control = this.documentationTextEdit;
		this.documentationLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.documentationLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.documentationLayoutControlItem.MinSize = new System.Drawing.Size(150, 24);
		this.documentationLayoutControlItem.Name = "documentationLayoutControlItem";
		this.documentationLayoutControlItem.Size = new System.Drawing.Size(792, 24);
		this.documentationLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.documentationLayoutControlItem.Text = "Glossary";
		this.documentationLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.documentationLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.documentationLayoutControlItem.TextToControlDistance = 0;
		this.typeLayoutControlItem.Control = this.termTypeLookUpEdit;
		this.typeLayoutControlItem.Location = new System.Drawing.Point(0, 50);
		this.typeLayoutControlItem.MaxSize = new System.Drawing.Size(500, 26);
		this.typeLayoutControlItem.MinSize = new System.Drawing.Size(500, 26);
		this.typeLayoutControlItem.Name = "typeLayoutControlItem";
		this.typeLayoutControlItem.Size = new System.Drawing.Size(500, 26);
		this.typeLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.typeLayoutControlItem.Text = "Type";
		this.typeLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.typeLayoutControlItem.TextSize = new System.Drawing.Size(82, 13);
		this.typeLayoutControlItem.TextToControlDistance = 3;
		this.layoutControlItem4.Control = this.labelControl1;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 108);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(57, 17);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem5.Control = this.searchCountLabelControl;
		this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
		this.layoutControlItem5.Location = new System.Drawing.Point(57, 108);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(735, 17);
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.relatedTermsXtraTabPage.Controls.Add(this.relatedTermsGrid);
		this.relatedTermsXtraTabPage.Controls.Add(this.relatedTermsGridPanelUserControl);
		this.relatedTermsXtraTabPage.Name = "relatedTermsXtraTabPage";
		this.relatedTermsXtraTabPage.Size = new System.Drawing.Size(812, 465);
		this.relatedTermsXtraTabPage.Text = "Related terms";
		this.relatedTermsGrid.AllowDrop = true;
		this.relatedTermsGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.relatedTermsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
		this.relatedTermsGrid.Location = new System.Drawing.Point(0, 28);
		this.relatedTermsGrid.MainView = this.relatedTermsGridView;
		this.relatedTermsGrid.Name = "relatedTermsGrid";
		this.relatedTermsGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.iconTableRepositoryItemPictureEdit, this.commentsRepositoryItemAutoHeightMemoEdit, this.relationshipTypeRepositoryItemLookUpEdit });
		this.relatedTermsGrid.Size = new System.Drawing.Size(812, 437);
		this.relatedTermsGrid.TabIndex = 13;
		this.relatedTermsGrid.ToolTipController = this.termToolTipController;
		this.relatedTermsGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.relatedTermsGridView });
		this.relatedTermsGrid.DragDrop += new System.Windows.Forms.DragEventHandler(RelatedTermsGrid_DragDrop);
		this.relatedTermsGrid.DragEnter += new System.Windows.Forms.DragEventHandler(RelatedTermsGrid_DragEnter);
		this.relatedTermsGrid.DragOver += new System.Windows.Forms.DragEventHandler(RelatedTermsGrid_DragOver);
		this.relatedTermsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.relationshipTypeGridColumn, this.relatedTermIconGridColumn, this.relatedTermTitleGridColumn, this.relatedTermTypeTableGridColumn, this.commentsGridColumn, this.createdGridColumn, this.createdByGridColumn, this.lastUpdatedGridColumn, this.lastUpdatedByGridColumn });
		defaultBulkCopy.IsCopying = false;
		this.relatedTermsGridView.Copy = defaultBulkCopy;
		this.relatedTermsGridView.GridControl = this.relatedTermsGrid;
		this.relatedTermsGridView.Name = "relatedTermsGridView";
		this.relatedTermsGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.relatedTermsGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.relatedTermsGridView.OptionsSelection.MultiSelect = true;
		this.relatedTermsGridView.OptionsView.ColumnAutoWidth = false;
		this.relatedTermsGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.relatedTermsGridView.OptionsView.ShowGroupPanel = false;
		this.relatedTermsGridView.OptionsView.ShowIndicator = false;
		this.relatedTermsGridView.RowHighlightingIsEnabled = true;
		this.relatedTermsGridView.SplashScreenManager = null;
		this.relatedTermsGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(RelatedTermsGridView_CustomDrawCell);
		this.relatedTermsGridView.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(RelatedTermsGridView_CustomRowCellEdit);
		this.relatedTermsGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(RelatedTermsGridView_PopupMenuShowing);
		this.relatedTermsGridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(RelatedTermsGridView_SelectionChanged);
		this.relatedTermsGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(RelatedTermsGridView_CellValueChanged);
		this.relatedTermsGridView.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(RelatedTermsGridView_CustomUnboundColumnData);
		this.relatedTermsGridView.CustomRowFilter += new DevExpress.XtraGrid.Views.Base.RowFilterEventHandler(RelatedTermsGridView_CustomRowFilter);
		this.relationshipTypeGridColumn.Caption = "Relationship";
		this.relationshipTypeGridColumn.ColumnEdit = this.relationshipTypeRepositoryItemLookUpEdit;
		this.relationshipTypeGridColumn.FieldName = "RelationshipType";
		this.relationshipTypeGridColumn.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.relationshipTypeGridColumn.Name = "relationshipTypeGridColumn";
		this.relationshipTypeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.relationshipTypeGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.relationshipTypeGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.relationshipTypeGridColumn.Tag = "FIT_WIDTH";
		this.relationshipTypeGridColumn.Visible = true;
		this.relationshipTypeGridColumn.VisibleIndex = 0;
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
		this.relatedTermIconGridColumn.Caption = " ";
		this.relatedTermIconGridColumn.ColumnEdit = this.iconTableRepositoryItemPictureEdit;
		this.relatedTermIconGridColumn.FieldName = "RelatedTermIcon";
		this.relatedTermIconGridColumn.MaxWidth = 21;
		this.relatedTermIconGridColumn.MinWidth = 21;
		this.relatedTermIconGridColumn.Name = "relatedTermIconGridColumn";
		this.relatedTermIconGridColumn.OptionsColumn.AllowEdit = false;
		this.relatedTermIconGridColumn.OptionsColumn.ReadOnly = true;
		this.relatedTermIconGridColumn.OptionsFilter.AllowFilter = false;
		this.relatedTermIconGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.relatedTermIconGridColumn.Visible = true;
		this.relatedTermIconGridColumn.VisibleIndex = 1;
		this.relatedTermIconGridColumn.Width = 21;
		this.iconTableRepositoryItemPictureEdit.AllowFocused = false;
		this.iconTableRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.iconTableRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconTableRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconTableRepositoryItemPictureEdit.Name = "iconTableRepositoryItemPictureEdit";
		this.iconTableRepositoryItemPictureEdit.ShowMenu = false;
		this.relatedTermTitleGridColumn.Caption = "Related term";
		this.relatedTermTitleGridColumn.FieldName = "RelatedTermTitle";
		this.relatedTermTitleGridColumn.Name = "relatedTermTitleGridColumn";
		this.relatedTermTitleGridColumn.OptionsColumn.AllowEdit = false;
		this.relatedTermTitleGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.relatedTermTitleGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.relatedTermTitleGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.relatedTermTitleGridColumn.Tag = "FIT_WIDTH";
		this.relatedTermTitleGridColumn.Visible = true;
		this.relatedTermTitleGridColumn.VisibleIndex = 2;
		this.relatedTermTitleGridColumn.Width = 140;
		this.relatedTermTypeTableGridColumn.Caption = "Related term type";
		this.relatedTermTypeTableGridColumn.FieldName = "RelatedTermType";
		this.relatedTermTypeTableGridColumn.Name = "relatedTermTypeTableGridColumn";
		this.relatedTermTypeTableGridColumn.OptionsColumn.AllowEdit = false;
		this.relatedTermTypeTableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.relatedTermTypeTableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.relatedTermTypeTableGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.relatedTermTypeTableGridColumn.Tag = "FIT_WIDTH";
		this.relatedTermTypeTableGridColumn.Visible = true;
		this.relatedTermTypeTableGridColumn.VisibleIndex = 3;
		this.relatedTermTypeTableGridColumn.Width = 110;
		this.commentsGridColumn.Caption = "Comments";
		this.commentsGridColumn.ColumnEdit = this.commentsRepositoryItemAutoHeightMemoEdit;
		this.commentsGridColumn.FieldName = "Description";
		this.commentsGridColumn.MaxWidth = 1000;
		this.commentsGridColumn.MinWidth = 200;
		this.commentsGridColumn.Name = "commentsGridColumn";
		this.commentsGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.commentsGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.commentsGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.commentsGridColumn.Tag = "FIT_WIDTH";
		this.commentsGridColumn.Visible = true;
		this.commentsGridColumn.VisibleIndex = 4;
		this.commentsGridColumn.Width = 400;
		this.commentsRepositoryItemAutoHeightMemoEdit.Name = "commentsRepositoryItemAutoHeightMemoEdit";
		this.commentsRepositoryItemAutoHeightMemoEdit.ScrollBars = System.Windows.Forms.ScrollBars.None;
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
		this.termToolTipController.AllowHtmlText = true;
		this.termToolTipController.AutoPopDelay = 10000;
		this.termToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.relatedTermsGridPanelUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.relatedTermsGridPanelUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.relatedTermsGridPanelUserControl.GridView = this.relatedTermsGridView;
		this.relatedTermsGridPanelUserControl.Location = new System.Drawing.Point(0, 0);
		this.relatedTermsGridPanelUserControl.Name = "relatedTermsGridPanelUserControl";
		this.relatedTermsGridPanelUserControl.Size = new System.Drawing.Size(812, 28);
		this.relatedTermsGridPanelUserControl.TabIndex = 14;
		this.dataLinksXtraTabPage.Controls.Add(this.dataLinksGrid);
		this.dataLinksXtraTabPage.Controls.Add(this.dataLinksGridPanelUserControl);
		this.dataLinksXtraTabPage.Name = "dataLinksXtraTabPage";
		this.dataLinksXtraTabPage.Size = new System.Drawing.Size(812, 465);
		this.dataLinksXtraTabPage.Text = "Linked data elements";
		this.dataLinksXtraTabPage.Tooltip = "Linked Data Dictionary elements";
		this.dataLinksGrid.AllowDrop = true;
		this.dataLinksGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.dataLinksGrid.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dataLinksGrid.Location = new System.Drawing.Point(0, 28);
		this.dataLinksGrid.MainView = this.dataLinksGridView;
		this.dataLinksGrid.Name = "dataLinksGrid";
		this.dataLinksGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemAutoHeightMemoEdit1, this.iconRepositoryItemPictureEdit });
		this.dataLinksGrid.Size = new System.Drawing.Size(812, 437);
		this.dataLinksGrid.TabIndex = 15;
		this.dataLinksGrid.ToolTipController = this.termToolTipController;
		this.dataLinksGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.dataLinksGridView });
		this.dataLinksGrid.DragDrop += new System.Windows.Forms.DragEventHandler(DataLinksGrid_DragDrop);
		this.dataLinksGrid.DragEnter += new System.Windows.Forms.DragEventHandler(DataLinksGrid_DragEnter);
		this.dataLinksGrid.DragOver += new System.Windows.Forms.DragEventHandler(DataLinksGrid_DragOver);
		this.dataLinksGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.iconDataLinksGridColumn, this.tableDataLinksGridColumn, this.columnDataLinksGridColumn, this.documentationDataLinksGridColumn, this.createdDataLinksGridColumn, this.createdByDataLinksGridColumn, this.lastUpdatedDataLinksGridColumn, this.lastUpdatedByDataLinksGridColumn });
		defaultBulkCopy2.IsCopying = false;
		this.dataLinksGridView.Copy = defaultBulkCopy2;
		this.dataLinksGridView.GridControl = this.dataLinksGrid;
		this.dataLinksGridView.Name = "dataLinksGridView";
		this.dataLinksGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.dataLinksGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.dataLinksGridView.OptionsSelection.MultiSelect = true;
		this.dataLinksGridView.OptionsView.ColumnAutoWidth = false;
		this.dataLinksGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.dataLinksGridView.OptionsView.ShowGroupPanel = false;
		this.dataLinksGridView.OptionsView.ShowIndicator = false;
		this.dataLinksGridView.RowHighlightingIsEnabled = true;
		this.dataLinksGridView.SplashScreenManager = null;
		this.dataLinksGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(DataLinksGridView_PopupMenuShowing);
		this.dataLinksGridView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(DataLinksGridView_SelectionChanged);
		this.dataLinksGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(DataLinksGridView_CellValueChanged);
		this.dataLinksGridView.CustomColumnSort += new DevExpress.XtraGrid.Views.Base.CustomColumnSortEventHandler(dataLinksGridView_CustomColumnSort);
		this.dataLinksGridView.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(DataLinksGridView_CustomUnboundColumnData);
		this.dataLinksGridView.CustomRowFilter += new DevExpress.XtraGrid.Views.Base.RowFilterEventHandler(DataLinksGridView_CustomRowFilter);
		this.dataLinksGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(DataLinksGridView_CustomDrawCell);
		this.iconDataLinksGridColumn.Caption = " ";
		this.iconDataLinksGridColumn.ColumnEdit = this.iconRepositoryItemPictureEdit;
		this.iconDataLinksGridColumn.FieldName = "Icon";
		this.iconDataLinksGridColumn.MaxWidth = 21;
		this.iconDataLinksGridColumn.MinWidth = 21;
		this.iconDataLinksGridColumn.Name = "iconDataLinksGridColumn";
		this.iconDataLinksGridColumn.OptionsColumn.AllowEdit = false;
		this.iconDataLinksGridColumn.OptionsColumn.ReadOnly = true;
		this.iconDataLinksGridColumn.OptionsFilter.AllowFilter = false;
		this.iconDataLinksGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.iconDataLinksGridColumn.Visible = true;
		this.iconDataLinksGridColumn.VisibleIndex = 0;
		this.iconDataLinksGridColumn.Width = 21;
		this.iconRepositoryItemPictureEdit.Name = "iconRepositoryItemPictureEdit";
		this.tableDataLinksGridColumn.Caption = "Table";
		this.tableDataLinksGridColumn.FieldName = "ObjectNameWithSchemaAndTitle";
		this.tableDataLinksGridColumn.Name = "tableDataLinksGridColumn";
		this.tableDataLinksGridColumn.OptionsColumn.AllowEdit = false;
		this.tableDataLinksGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.tableDataLinksGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.tableDataLinksGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.tableDataLinksGridColumn.Tag = "FIT_WIDTH";
		this.tableDataLinksGridColumn.Visible = true;
		this.tableDataLinksGridColumn.VisibleIndex = 1;
		this.tableDataLinksGridColumn.Width = 140;
		this.columnDataLinksGridColumn.Caption = "Column";
		this.columnDataLinksGridColumn.ColumnEdit = this.repositoryItemAutoHeightMemoEdit1;
		this.columnDataLinksGridColumn.FieldName = "ElementFullNameWithTitleFormatted";
		this.columnDataLinksGridColumn.Name = "columnDataLinksGridColumn";
		this.columnDataLinksGridColumn.OptionsColumn.AllowEdit = false;
		this.columnDataLinksGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.columnDataLinksGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.columnDataLinksGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.columnDataLinksGridColumn.Tag = "FIT_WIDTH";
		this.columnDataLinksGridColumn.Visible = true;
		this.columnDataLinksGridColumn.VisibleIndex = 2;
		this.columnDataLinksGridColumn.Width = 300;
		this.repositoryItemAutoHeightMemoEdit1.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.repositoryItemAutoHeightMemoEdit1.Name = "repositoryItemAutoHeightMemoEdit1";
		this.repositoryItemAutoHeightMemoEdit1.ScrollBars = System.Windows.Forms.ScrollBars.None;
		this.documentationDataLinksGridColumn.Caption = "Database";
		this.documentationDataLinksGridColumn.FieldName = "ObjectDocumentationTitle";
		this.documentationDataLinksGridColumn.Name = "documentationDataLinksGridColumn";
		this.documentationDataLinksGridColumn.OptionsColumn.AllowEdit = false;
		this.documentationDataLinksGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.documentationDataLinksGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.documentationDataLinksGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.documentationDataLinksGridColumn.Tag = "FIT_WIDTH";
		this.documentationDataLinksGridColumn.Visible = true;
		this.documentationDataLinksGridColumn.VisibleIndex = 3;
		this.documentationDataLinksGridColumn.Width = 140;
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
		this.createdByDataLinksGridColumn.OptionsFilter.AllowFilter = false;
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
		this.lastUpdatedByDataLinksGridColumn.OptionsFilter.AllowFilter = false;
		this.lastUpdatedByDataLinksGridColumn.Width = 100;
		this.dataLinksGridPanelUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.dataLinksGridPanelUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.dataLinksGridPanelUserControl.GridView = this.dataLinksGridView;
		this.dataLinksGridPanelUserControl.Location = new System.Drawing.Point(0, 0);
		this.dataLinksGridPanelUserControl.Name = "dataLinksGridPanelUserControl";
		this.dataLinksGridPanelUserControl.Size = new System.Drawing.Size(812, 28);
		this.dataLinksGridPanelUserControl.TabIndex = 16;
		this.termMetadataXtraTabPage.Controls.Add(this.metadataLayoutControl);
		this.termMetadataXtraTabPage.Name = "termMetadataXtraTabPage";
		this.termMetadataXtraTabPage.Size = new System.Drawing.Size(812, 465);
		this.termMetadataXtraTabPage.Text = "Metadata";
		this.metadataLayoutControl.AllowCustomization = false;
		this.metadataLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.metadataLayoutControl.Controls.Add(this.lastUpdatedLabel);
		this.metadataLayoutControl.Controls.Add(this.createdLabelControl);
		this.metadataLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.metadataLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.metadataLayoutControl.Name = "metadataLayoutControl";
		this.metadataLayoutControl.Root = this.layoutControlGroup2;
		this.metadataLayoutControl.Size = new System.Drawing.Size(812, 465);
		this.metadataLayoutControl.TabIndex = 0;
		this.metadataLayoutControl.Text = "layoutControl1";
		this.metadataLayoutControl.ToolTipController = this.metadataToolTipController;
		this.lastUpdatedLabel.Location = new System.Drawing.Point(147, 36);
		this.lastUpdatedLabel.Name = "lastUpdatedLabel";
		this.lastUpdatedLabel.Size = new System.Drawing.Size(653, 20);
		this.lastUpdatedLabel.StyleController = this.metadataLayoutControl;
		this.lastUpdatedLabel.TabIndex = 11;
		this.createdLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.createdLabelControl.Appearance.Options.UseFont = true;
		this.createdLabelControl.Location = new System.Drawing.Point(147, 12);
		this.createdLabelControl.Name = "createdLabelControl";
		this.createdLabelControl.Size = new System.Drawing.Size(653, 20);
		this.createdLabelControl.StyleController = this.metadataLayoutControl;
		this.createdLabelControl.TabIndex = 14;
		this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup2.GroupBordersVisible = false;
		this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[3] { this.emptySpaceItem3, this.lastUpdatedLayoutControlItem, this.firstImportedLayoutControlItem });
		this.layoutControlGroup2.Name = "Root";
		this.layoutControlGroup2.Size = new System.Drawing.Size(812, 465);
		this.layoutControlGroup2.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.emptySpaceItem3.AppearanceItemCaption.Options.UseFont = true;
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 48);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(792, 397);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.lastUpdatedLayoutControlItem.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.lastUpdatedLayoutControlItem.AppearanceItemCaption.Options.UseFont = true;
		this.lastUpdatedLayoutControlItem.Control = this.lastUpdatedLabel;
		this.lastUpdatedLayoutControlItem.CustomizationFormText = "Last imported:";
		this.lastUpdatedLayoutControlItem.Location = new System.Drawing.Point(0, 24);
		this.lastUpdatedLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.lastUpdatedLayoutControlItem.MinSize = new System.Drawing.Size(116, 24);
		this.lastUpdatedLayoutControlItem.Name = "lastUpdatedLayoutControlItem";
		this.lastUpdatedLayoutControlItem.Size = new System.Drawing.Size(792, 24);
		this.lastUpdatedLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.lastUpdatedLayoutControlItem.Text = "Last updated";
		this.lastUpdatedLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.lastUpdatedLayoutControlItem.TextSize = new System.Drawing.Size(130, 13);
		this.lastUpdatedLayoutControlItem.TextToControlDistance = 5;
		this.firstImportedLayoutControlItem.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.firstImportedLayoutControlItem.AppearanceItemCaption.Options.UseFont = true;
		this.firstImportedLayoutControlItem.Control = this.createdLabelControl;
		this.firstImportedLayoutControlItem.CustomizationFormText = "Created/first imported:";
		this.firstImportedLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.firstImportedLayoutControlItem.MaxSize = new System.Drawing.Size(0, 24);
		this.firstImportedLayoutControlItem.MinSize = new System.Drawing.Size(116, 24);
		this.firstImportedLayoutControlItem.Name = "firstImportedLayoutControlItem";
		this.firstImportedLayoutControlItem.Size = new System.Drawing.Size(792, 24);
		this.firstImportedLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.firstImportedLayoutControlItem.Text = "Created";
		this.firstImportedLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.firstImportedLayoutControlItem.TextSize = new System.Drawing.Size(130, 13);
		this.firstImportedLayoutControlItem.TextToControlDistance = 5;
		this.metadataToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem1.CustomizationFormText = "Title:";
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(1089, 24);
		this.layoutControlItem1.Text = "Title:";
		this.layoutControlItem1.TextSize = new System.Drawing.Size(31, 13);
		this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem2.CustomizationFormText = "Title:";
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem2.Name = "layoutControlItem1";
		this.layoutControlItem2.Size = new System.Drawing.Size(1089, 24);
		this.layoutControlItem2.Text = "Title:";
		this.layoutControlItem2.TextSize = new System.Drawing.Size(31, 13);
		this.termStatusUserControl.BackColor = System.Drawing.Color.FromArgb(255, 255, 255);
		this.termStatusUserControl.BackgroundColor = System.Drawing.Color.FromArgb(224, 234, 248);
		this.termStatusUserControl.Description = "This term has been removed from the database. You can remove it from the repository.";
		this.termStatusUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.termStatusUserControl.ForeColor = System.Drawing.Color.FromArgb(38, 38, 38);
		this.termStatusUserControl.Image = Dataedo.App.Properties.Resources.warning_16;
		this.termStatusUserControl.Location = new System.Drawing.Point(0, 20);
		this.termStatusUserControl.Name = "termStatusUserControl";
		this.termStatusUserControl.Size = new System.Drawing.Size(814, 40);
		this.termStatusUserControl.TabIndex = 11;
		this.termStatusUserControl.Visible = false;
		this.termTextEdit.Dock = System.Windows.Forms.DockStyle.Top;
		this.termTextEdit.EditValue = "";
		this.termTextEdit.Location = new System.Drawing.Point(0, 0);
		this.termTextEdit.Name = "termTextEdit";
		this.termTextEdit.Properties.AllowFocused = false;
		this.termTextEdit.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold);
		this.termTextEdit.Properties.Appearance.Options.UseFont = true;
		this.termTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.termTextEdit.Properties.ReadOnly = true;
		this.termTextEdit.Size = new System.Drawing.Size(814, 20);
		this.termTextEdit.TabIndex = 12;
		this.termTextEdit.TabStop = false;
		this.termTextEdit.ToolTipController = this.toolTipController;
		this.termTitleErrorProvider.ContainerControl = this;
		this.relatedTermsPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.editTermRelationshipsRelatedTermsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.goToObjectRelatedTermsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeRelatedTermsBarButtonItem)
		});
		this.relatedTermsPopupMenu.Manager = this.relatedTermsBarManager;
		this.relatedTermsPopupMenu.Name = "relatedTermsPopupMenu";
		this.relatedTermsPopupMenu.BeforePopup += new System.ComponentModel.CancelEventHandler(relatedTermsPopupMenu_BeforePopup);
		this.editTermRelationshipsRelatedTermsBarButtonItem.Caption = "Edit term relationships";
		this.editTermRelationshipsRelatedTermsBarButtonItem.Id = 0;
		this.editTermRelationshipsRelatedTermsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.term_add_related_term_16;
		this.editTermRelationshipsRelatedTermsBarButtonItem.Name = "editTermRelationshipsRelatedTermsBarButtonItem";
		this.editTermRelationshipsRelatedTermsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(editTermRelationshipsRelatedTermsBarButtonItem_ItemClick);
		this.goToObjectRelatedTermsBarButtonItem.Caption = "Go to object";
		this.goToObjectRelatedTermsBarButtonItem.Id = 1;
		this.goToObjectRelatedTermsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.table_16;
		this.goToObjectRelatedTermsBarButtonItem.Name = "goToObjectRelatedTermsBarButtonItem";
		this.goToObjectRelatedTermsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(goToObjectRelatedTermsBarButtonItem_ItemClick);
		this.removeRelatedTermsBarButtonItem.Caption = "Remove from repository";
		this.removeRelatedTermsBarButtonItem.Id = 2;
		this.removeRelatedTermsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeRelatedTermsBarButtonItem.Name = "removeRelatedTermsBarButtonItem";
		this.removeRelatedTermsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(removeRelatedTermsBarButtonItem_ItemClick);
		this.relatedTermsBarManager.DockControls.Add(this.barDockControlTop);
		this.relatedTermsBarManager.DockControls.Add(this.barDockControlBottom);
		this.relatedTermsBarManager.DockControls.Add(this.barDockControlLeft);
		this.relatedTermsBarManager.DockControls.Add(this.barDockControlRight);
		this.relatedTermsBarManager.Form = this;
		this.relatedTermsBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[3] { this.editTermRelationshipsRelatedTermsBarButtonItem, this.goToObjectRelatedTermsBarButtonItem, this.removeRelatedTermsBarButtonItem });
		this.relatedTermsBarManager.MaxItemId = 3;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.relatedTermsBarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(814, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 554);
		this.barDockControlBottom.Manager = this.relatedTermsBarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(814, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.relatedTermsBarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 554);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(814, 0);
		this.barDockControlRight.Manager = this.relatedTermsBarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 554);
		this.linkedDataPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[3]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.editLinksLinkedDataBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.goToObjectLinkedDataBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeLinkedDataBarButtonItem)
		});
		this.linkedDataPopupMenu.Manager = this.linkedDataBarManager;
		this.linkedDataPopupMenu.Name = "linkedDataPopupMenu";
		this.linkedDataPopupMenu.BeforePopup += new System.ComponentModel.CancelEventHandler(linkedDataPopupMenu_BeforePopup);
		this.editLinksLinkedDataBarButtonItem.Caption = "Edit links to Data Dictionary elements";
		this.editLinksLinkedDataBarButtonItem.Id = 0;
		this.editLinksLinkedDataBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.data_link_add_16;
		this.editLinksLinkedDataBarButtonItem.Name = "editLinksLinkedDataBarButtonItem";
		this.editLinksLinkedDataBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(editLinksLinkedDataBarButtonItem_ItemClick);
		this.goToObjectLinkedDataBarButtonItem.Caption = "Go to object";
		this.goToObjectLinkedDataBarButtonItem.Id = 1;
		this.goToObjectLinkedDataBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.table_16;
		this.goToObjectLinkedDataBarButtonItem.Name = "goToObjectLinkedDataBarButtonItem";
		this.goToObjectLinkedDataBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(goToObjectLinkedDataBarButtonItem_ItemClick);
		this.removeLinkedDataBarButtonItem.Caption = "Remove link";
		this.removeLinkedDataBarButtonItem.Id = 2;
		this.removeLinkedDataBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeLinkedDataBarButtonItem.Name = "removeLinkedDataBarButtonItem";
		this.removeLinkedDataBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(removeLinkedDataBarButtonItem_ItemClick);
		this.linkedDataBarManager.DockControls.Add(this.barDockControl1);
		this.linkedDataBarManager.DockControls.Add(this.barDockControl2);
		this.linkedDataBarManager.DockControls.Add(this.barDockControl3);
		this.linkedDataBarManager.DockControls.Add(this.barDockControl4);
		this.linkedDataBarManager.Form = this;
		this.linkedDataBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[3] { this.editLinksLinkedDataBarButtonItem, this.goToObjectLinkedDataBarButtonItem, this.removeLinkedDataBarButtonItem });
		this.linkedDataBarManager.MaxItemId = 3;
		this.barDockControl1.CausesValidation = false;
		this.barDockControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControl1.Location = new System.Drawing.Point(0, 0);
		this.barDockControl1.Manager = this.linkedDataBarManager;
		this.barDockControl1.Size = new System.Drawing.Size(814, 0);
		this.barDockControl2.CausesValidation = false;
		this.barDockControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControl2.Location = new System.Drawing.Point(0, 554);
		this.barDockControl2.Manager = this.linkedDataBarManager;
		this.barDockControl2.Size = new System.Drawing.Size(814, 0);
		this.barDockControl3.CausesValidation = false;
		this.barDockControl3.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControl3.Location = new System.Drawing.Point(0, 0);
		this.barDockControl3.Manager = this.linkedDataBarManager;
		this.barDockControl3.Size = new System.Drawing.Size(0, 554);
		this.barDockControl4.CausesValidation = false;
		this.barDockControl4.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControl4.Location = new System.Drawing.Point(814, 0);
		this.barDockControl4.Manager = this.linkedDataBarManager;
		this.barDockControl4.Size = new System.Drawing.Size(0, 554);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.termXtraTabControl);
		base.Controls.Add(this.termStatusUserControl);
		base.Controls.Add(this.termTextEdit);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Controls.Add(this.barDockControl3);
		base.Controls.Add(this.barDockControl4);
		base.Controls.Add(this.barDockControl2);
		base.Controls.Add(this.barDockControl1);
		base.Name = "TermUserControl";
		base.Size = new System.Drawing.Size(814, 554);
		base.Load += new System.EventHandler(TermUserControl_Load);
		((System.ComponentModel.ISupportInitialize)this.termXtraTabControl).EndInit();
		this.termXtraTabControl.ResumeLayout(false);
		this.termDescriptionXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.termLayoutControl).EndInit();
		this.termLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.termTypeLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.documentationTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.termTitleTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.customFieldsLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.documentationLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		this.relatedTermsXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.relatedTermsGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.relatedTermsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.relationshipTypeRepositoryItemLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconTableRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.commentsRepositoryItemAutoHeightMemoEdit).EndInit();
		this.dataLinksXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dataLinksGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataLinksGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemAutoHeightMemoEdit1).EndInit();
		this.termMetadataXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.metadataLayoutControl).EndInit();
		this.metadataLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lastUpdatedLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.firstImportedLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.termTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.termTitleErrorProvider).EndInit();
		((System.ComponentModel.ISupportInitialize)this.relatedTermsPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.relatedTermsBarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.linkedDataPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.linkedDataBarManager).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
