using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using ColorCode;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.Model;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Documentation;
using Dataedo.App.Enums;
using Dataedo.App.Forms;
using Dataedo.App.Forms.Tools;
using Dataedo.App.MenuTree;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Search;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.Dependencies;
using Dataedo.App.UserControls.PanelControls.CommonHelpers;
using Dataedo.App.UserControls.SchemaImportsAndChanges;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraSplashScreen;
using DevExpress.XtraTab;

namespace Dataedo.App.UserControls.PanelControls;

public class BasePanelControl : BaseUserControl, ISaveable
{
	protected SchemaImportsAndChangesSupport SchemaImportsAndChangesSupport;

	private List<int> modulesId;

	protected PopupMenu suggestedValuesContextMenu;

	protected CancellationTokenSource token = new CancellationTokenSource();

	protected bool editedTitleFromTreeList;

	private SplashScreenManager splashScreenManager;

	private CodeColorizer codeColorizer;

	protected CustomFieldContainer customFields;

	public virtual int DatabaseId { get; } = -1;


	public virtual int ObjectModuleId { get; } = -1;


	public virtual int ObjectId { get; } = -1;


	public virtual SharedObjectTypeEnum.ObjectType ObjectType { get; protected set; } = SharedObjectTypeEnum.ObjectType.UnresolvedEntity;


	public virtual string ObjectSchema { get; } = string.Empty;


	public virtual string ObjectName { get; } = string.Empty;


	public bool? HasMultipleSchemas { get; set; }

	public bool? DatabaseShowSchema { get; set; }

	public bool? DatabaseShowSchemaOverride { get; set; }

	public bool ShowSchema
	{
		get
		{
			if (!DatabaseRow.GetShowSchema(DatabaseShowSchema, DatabaseShowSchemaOverride))
			{
				return ContextShowSchema;
			}
			return true;
		}
	}

	public bool ContextShowSchema { get; set; }

	public Dictionary<int, KeyValuePair<int, int>> KeyValuePairs { get; set; }

	public virtual HtmlUserControl DescriptionHtmlUserControl { get; }

	public virtual XtraTabPage SchemaImportsAndChangesXtraTabPage { get; }

	public virtual SchemaImportsAndChangesUserControl SchemaImportsAndChangesUserControl { get; }

	public virtual CustomFieldsPanelControl CustomFieldsPanelControl { get; }

	protected bool DisableSettingAsEdited { get; set; }

	protected List<int> ModulesId
	{
		get
		{
			if (modulesId == null)
			{
				modulesId = new List<int>();
			}
			return modulesId;
		}
		set
		{
			modulesId = value;
		}
	}

	public virtual BaseView VisibleGridView { get; set; }

	public SharedObjectSubtypeEnum.ObjectSubtype Subtype { get; set; }

	public bool ShowHints => MainControl.ShowSuggestions;

	protected ProgressTypeModel progressType => MainControl.ProgressType;

	public IEdit Edit { get; set; }

	public bool TabPageChangedProgrammatically { get; protected set; }

	public DBTreeMenu TreeMenu { get; set; }

	public DBTreeNode CurrentNode { get; set; }

	public MetadataEditorUserControl MainControl { get; protected set; }

	public CustomFieldsSupport CustomFieldsSupport { get; protected set; }

	public ResultItem LastSearchResult { get; private set; }

	public UserControlHelpers UserControlHelpers { get; protected set; }

	public SplashScreenManager GetSplashScreenManager()
	{
		return splashScreenManager;
	}

	public void SetLastResult(ResultItem result)
	{
		LastSearchResult = result;
	}

	protected void FillHighlights(SharedObjectTypeEnum.ObjectType? elementType)
	{
		if (UserControlHelpers.IsSearchActive && UserControlHelpers != null && LastSearchResult != null)
		{
			MainControl.FillHighlights(LastSearchResult, allowChangingTab: false, elementType);
		}
	}

	public virtual void SetRichEditControlBackground()
	{
		DescriptionHtmlUserControl.SetEmptyProgressBackgroundColor(MainControl.ShowProgress && MainControl.ProgressType.Type == ProgressTypeEnum.AllDocumentations);
	}

	public virtual void SetRichEditControlBackgroundWhileFocused()
	{
		DescriptionHtmlUserControl.SetFocusedColor(MainControl.ShowProgress && MainControl.ProgressType.Type == ProgressTypeEnum.AllDocumentations);
	}

	public virtual void ClearTabsHighlights()
	{
	}

	public virtual void FillControlProgressHighlights()
	{
		if (MainControl.ShowProgress)
		{
			UserControlHelpers.SetProgressHighlights(CustomFieldsPanelControl.FieldControls, progressType.FieldName);
		}
	}

	public BasePanelControl()
	{
		codeColorizer = new CodeColorizer();
		KeyValuePairs = new Dictionary<int, KeyValuePair<int, int>>();
		InitializeComponent();
	}

	public void Initialize()
	{
		CustomFieldsPanelControl.SizeChanged += delegate
		{
			SetCustomFieldsPanelControlHeight();
		};
	}

	public virtual void SetTabsProgressHighlights()
	{
	}

	public virtual void HideCustomization()
	{
	}

	public BasePanelControl(MetadataEditorUserControl control)
		: this()
	{
		MainControl = control;
	}

	public virtual void CloseEditors()
	{
		SchemaImportsAndChangesSupport?.CloseEditors();
		CustomFieldsPanelControl?.CloseEditors();
	}

	public virtual bool Save()
	{
		return false;
	}

	public void EditCustomFieldsFromGridPanel()
	{
		try
		{
			if (MainControl.ContinueAfterPossibleChanges())
			{
				MainControl.EditCustomFields();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, null, FindForm());
		}
	}

	public virtual void ReloadGridsData()
	{
	}

	protected IEnumerable<BaseRow> GetSelectedRows(GridView gridView)
	{
		List<BaseRow> list = new List<BaseRow>();
		int[] selectedRows = gridView.GetSelectedRows();
		foreach (int rowHandle in selectedRows)
		{
			list.Add(gridView.GetRow(rowHandle) as BaseRow);
		}
		return list;
	}

	protected virtual void Redraw()
	{
	}

	public virtual void CancelSuggestedDescriptions()
	{
	}

	protected virtual void LoadSuggestedDescriptions(object sender, EventArgs e)
	{
	}

	internal virtual void ClearControlProgressHighlights()
	{
		foreach (CustomFieldControl fieldControl in CustomFieldsPanelControl.FieldControls)
		{
			fieldControl.IsProgressPainterActive = false;
			if (fieldControl.ValueEditBackColor.Equals(ProgressPainter.Color))
			{
				fieldControl.ValueEditBackColor = SkinColors.ControlColorFromSystemColors;
			}
		}
	}

	protected void ColorizeSyntax(RichEditUserControl destination, string sourceCode, string language)
	{
		RichEditUserControlHelper.ColorizeSyntax(destination, codeColorizer, sourceCode, "SQL");
	}

	public static void TrimEmptyParagraphs(Document document, bool trimStart = true, bool trimEnd = true)
	{
		DocumentRange[] array = document.FindAll("\r\n", SearchOptions.None);
		if (array.Length != 0)
		{
			if (trimStart && array[0] != null && array[0].Start.ToInt() == 0)
			{
				document.Replace(array[0], string.Empty);
			}
			if (trimEnd && array.Length - 1 != 0)
			{
				document.Replace(array[array.Length - 1], string.Empty);
			}
		}
	}

	public virtual void SetParameters(DBTreeNode selectedNode, CustomFieldsSupport customFieldsSupport, Dataedo.App.Data.MetadataServer.Model.DependencyRow.DependencyNodeCommonType? dependencyType = null)
	{
		ContextShowSchema = selectedNode.ParentNode?.ShowSchema ?? selectedNode.ShowSchema;
		DescriptionHtmlUserControl.Initialize();
	}

	public virtual List<string> GetFieldNames(Func<CustomFieldRowExtended, bool> predicate)
	{
		List<string> list = new List<string>();
		list.AddRange((from x in customFields.CustomFields.Fields.Where(predicate)
			select x.FieldName).ToList());
		list.Add("description");
		list.Add("title");
		return list;
	}

	public virtual List<string> GetConstraintAndTriggerFieldNames(Func<CustomFieldRowExtended, bool> predicate)
	{
		List<string> list = new List<string>();
		list.AddRange((from x in customFields.CustomFields.Fields.Where(predicate)
			select x.FieldName).ToList());
		list.Add("description");
		return list;
	}

	public virtual List<string> GetParameterFieldNames()
	{
		List<string> list = new List<string>();
		list.AddRange((from x in customFields.CustomFields.Fields
			where x.ParameterVisibility && (x.Type == CustomFieldTypeEnum.CustomFieldType.Text || x.Type == CustomFieldTypeEnum.CustomFieldType.ListOpen)
			select x.FieldName).ToList());
		list.Add("description");
		return list;
	}

	protected void ManageObjectsInMenu(int databaseId, DocumentationModules[] checkedValues, int objectId, string schema, string name, string title, UserTypeEnum.UserType source, ObjectIdName[] checkedObjects)
	{
		List<int> list = new List<int>();
		if (checkedValues != null)
		{
			foreach (DocumentationModules documentationModules in checkedValues)
			{
				list.AddRange(documentationModules.ModulesId);
			}
		}
		else if (checkedObjects != null)
		{
			list.AddRange(checkedObjects.Select((ObjectIdName x) => x.BaseId));
		}
		CommonFunctionsDatabase.ManageObjectInTree(databaseId, list, ModulesId, objectId, schema, name, title, source, ObjectType, Subtype, MainControl);
		RemoveFromModules(list);
		AddToModules(list);
	}

	private void AddToModules(List<int> selectedModules)
	{
		foreach (int item in selectedModules.Where((int x) => !ModulesId.Contains(x)).ToList())
		{
			ModulesId.Add(item);
		}
	}

	private void RemoveFromModules(List<int> selectedModules)
	{
		foreach (int item in ModulesId.Where((int x) => !selectedModules.Contains(x)).ToList())
		{
			ModulesId.Remove(item);
		}
	}

	public virtual void RefreshModules()
	{
	}

	protected void SetSuggestedDescriptionTooltips(GridView grid, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		if (!ShowHints)
		{
			return;
		}
		GridHitInfo gridHitInfo = grid.CalcHitInfo(e.ControlMousePosition);
		if (gridHitInfo.RowHandle < 0 || gridHitInfo.Column == null)
		{
			return;
		}
		string fieldName = gridHitInfo.Column.FieldName;
		if (!(grid.GetRow(gridHitInfo.RowHandle) is BaseRow baseRow))
		{
			return;
		}
		object rowCellValue = grid.GetRowCellValue(gridHitInfo.RowHandle, fieldName);
		SuperToolTipSetupArgs superToolTipSetupArgs = new SuperToolTipSetupArgs();
		if (!baseRow.SuggestedDescriptions.ContainsKey(fieldName.ToLower()) || baseRow.SuggestedDescriptions[fieldName.ToLower()].Count() != 1 || rowCellValue == null || !rowCellValue.Equals(baseRow.SuggestedDescriptions[fieldName.ToLower()].FirstOrDefault().Description))
		{
			if (!string.IsNullOrEmpty(fieldName) && baseRow.SuggestedDescriptions.ContainsKey(fieldName.ToLower()) && baseRow.SuggestedDescriptions[fieldName.ToLower()].Any())
			{
				superToolTipSetupArgs.Contents.Text = "Press shift+F2 or right click on the cell to show suggested descriptions";
			}
			else if (!string.IsNullOrEmpty(fieldName) && string.IsNullOrEmpty(rowCellValue?.ToString()) && gridHitInfo.Column.Name == "referencesTableColumnsGridColumn")
			{
				superToolTipSetupArgs.Contents.Text = "Define your own relationships(foreign keys) to tables, views or structures across entire repisitory. Adding new relationships will not impact the source database. To add a relationship, select column(s) and choose “Add relationship” from context menu or the ribbon.";
			}
			else if (!string.IsNullOrEmpty(fieldName) && gridHitInfo.Column.Name == "keyTableColumnsGridColumn")
			{
				superToolTipSetupArgs.Contents.Text = "Define your own relationships primary or unique keys. This information will not impact your source database. To add key, select column(s) and choose “Add primary key” or “Add unique key” options from context menu or the ribbon.";
			}
			e.Info = new ToolTipControlInfo();
			e.Info.Object = gridHitInfo.HitTest.ToString() + gridHitInfo.RowHandle;
			e.Info.ToolTipType = ToolTipType.SuperTip;
			e.Info.SuperTip = new SuperToolTip();
			e.Info.SuperTip.Setup(superToolTipSetupArgs);
		}
	}

	protected void ShowSuggestedDescriptionsContextMenu(GridControl grid, SuggestedDescriptionManager manager)
	{
		GridView gridView = grid.MainView as GridView;
		if (!gridView.FocusedColumn.ReadOnly && gridView.FocusedColumn.OptionsColumn.AllowEdit)
		{
			gridView.ShowEditor();
			if (gridView.ActiveEditor != null)
			{
				Point location = new Point(gridView.ActiveEditor.Location.X, gridView.ActiveEditor.Location.Y + gridView.ActiveEditor.Size.Height);
				suggestedValuesContextMenu.ItemLinks.Clear();
				manager.CreateSuggestedDescriptionContextMenuItems(suggestedValuesContextMenu, isSeparatorDrawn: false, ShowHints, forceJoin: true);
				suggestedValuesContextMenu.ShowPopupMenu(grid.FocusedView, location, inRowCellOnly: false);
			}
		}
	}

	protected void CustomDrawGridCell(object sender, RowCellCustomDrawEventArgs e)
	{
		int num = 6;
		BaseRow baseRow = (sender as GridView).GetRow(e.RowHandle) as BaseRow;
		string key = e.Column.FieldName.ToLower();
		object obj = ((e.CellValue == null) ? string.Empty : e.CellValue);
		if (baseRow != null)
		{
			if (e.RowHandle >= 0 && baseRow.SuggestedDescriptions.ContainsKey(key) && baseRow.SuggestedDescriptions[key].Count() == 1 && obj.Equals(baseRow.SuggestedDescriptions[key].FirstOrDefault().Description))
			{
				e.DefaultDraw();
				e.Handled = true;
			}
			else if (e.RowHandle >= 0 && baseRow.SuggestedDescriptions.ContainsKey(key) && baseRow.SuggestedDescriptions[key].Any())
			{
				int num2 = e.Bounds.Location.X + e.Bounds.Width - num;
				int num3 = e.Bounds.Location.Y;
				Point[] points = new Point[3]
				{
					new Point(num2 + num, num3 + num),
					new Point(num2, num3),
					new Point(num2 + num, num3)
				};
				Color suggestedDescriptionsMarkerColor = SkinsManager.CurrentSkin.SuggestedDescriptionsMarkerColor;
				e.DefaultDraw();
				e.Cache.FillPolygon(points, suggestedDescriptionsMarkerColor);
				e.Handled = true;
			}
		}
	}

	public void SetCurrentTabPageTitle(bool isEdited, XtraTabPage xtraTabPage)
	{
		CommonFunctionsPanels.SetTabPageAndNodesTitle(isEdited, xtraTabPage, CurrentNode?.GetRootNode(), CurrentNode, Edit);
	}

	public void SetTabPageTitle(bool isEdited, XtraTabPage xtraTabPage, IEdit edit = null)
	{
		CommonFunctionsPanels.SetTabPageAndNodesTitle(isEdited, xtraTabPage, CurrentNode?.GetRootNode(), CurrentNode, edit);
	}

	protected void SetTabPageTitle(bool isEdited, XtraTabControl tabControl, XtraTabPage xtraTabPage, IEdit edit = null)
	{
		CommonFunctionsPanels.SetTabPageAndNodesTitle(isEdited, tabControl, xtraTabPage, CurrentNode?.GetRootNode(), CurrentNode, edit);
	}

	protected void AddEventsForDeleting(SharedObjectTypeEnum.ObjectType objectType, GridView gridView, XtraTabPage xtraTabPage, IEdit edit, PopupMenu popupMenu, BarButtonItem deleteObjectToolStripMenuItem, bool isObject, BindingList<int> deletedRelationsConstraintsRows = null, bool ommitDeletingEvents = false)
	{
		CommonFunctionsPanels.AddEventsForDeleting(objectType, gridView, xtraTabPage, MainControl, edit, popupMenu, deleteObjectToolStripMenuItem, isObject, deletedRelationsConstraintsRows, ommitDeletingEvents);
	}

	protected bool UpdateDependencies(int? databaseId, DependenciesUserControl dependenciesUserControl)
	{
		try
		{
			dependenciesUserControl.CloseAllEditors();
			DB.Dependency.DeleteUserDependencies(dependenciesUserControl.DeletedDependencies);
			dependenciesUserControl.DeletedDependencies.Clear();
			DB.Dependency.ActualizeDependencies(dependenciesUserControl.ChangedDependencies, databaseId.Value, FindForm());
			dependenciesUserControl.ChangedDependencies.Clear();
			DB.Dependency.InsertDependencies(dependenciesUserControl.AddedDependencies.Where((Dataedo.App.Data.MetadataServer.Model.DependencyRow x) => !x.DependencyId.HasValue));
			if (dependenciesUserControl.AddedDependencies.Any((Dataedo.App.Data.MetadataServer.Model.DependencyRow x) => !x.IsSaved))
			{
				return false;
			}
			dependenciesUserControl.AddedDependencies.Clear();
			return true;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while updating dependencies", FindForm());
			return false;
		}
	}

	public void ClearSavedNodesTitle()
	{
		CommonFunctionsPanels.SetNodesTitle(isEdited: false, CurrentNode?.GetRootNode(), CurrentNode, Edit);
	}

	public void SetCustomFieldsPanelControlHeight()
	{
		CustomFieldsPanelControl.SetPanelControlHeight();
	}

	private void InitializeComponent()
	{
		this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true, typeof(System.Windows.Forms.UserControl));
		base.SuspendLayout();
		base.Name = "BasePanelControl";
		base.ResumeLayout(false);
	}

	protected void ForceLostFocus()
	{
		Control control = new Control();
		base.Controls.Add(control);
		control.Focus();
		base.Controls.Remove(control);
	}

	protected string GetEmptyScriptMessage(SharedDatabaseTypeEnum.DatabaseType? databaseType, UserTypeEnum.UserType? source)
	{
		if (source == UserTypeEnum.UserType.USER)
		{
			return "No script provided. To add the script use the Design " + SharedObjectTypeEnum.TypeToStringForSingle(ObjectType) + " button.";
		}
		if (Subtype == SharedObjectSubtypeEnum.ObjectSubtype.CLRFunction || Subtype == SharedObjectSubtypeEnum.ObjectSubtype.CLRProcedure || Subtype == SharedObjectSubtypeEnum.ObjectSubtype.ExtendedProcedure)
		{
			return "The script is unavailable for assembly (CLR) functions/procedures.";
		}
		return DatabaseSupportFactory.GetDatabaseSupport(databaseType).EmptyScriptMessage;
	}
}
