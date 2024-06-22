using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.EventArgsDef;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Data.MetadataServer.History;
using Dataedo.App.History;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.Pannels;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Columns;
using Dataedo.App.UserControls.PanelControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.Data.Base.Commands.Parameters.Types;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Modules;
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

namespace Dataedo.App.UserControls.SummaryControls;

public class ModuleSummaryUserControl : BaseSummaryUserControl
{
	private const int columnPickerBarButtonIndex = 3;

	private MetadataEditorUserControl mainControl;

	public bool ModuleAddingFromComboControl;

	private int databaseId;

	private IContainer components;

	private LabelControl allDatabaseModulesLabel;

	private GridControl allDatabaseModulesGrid;

	private GridColumn iconModuleGridColumn;

	private RepositoryItemPictureEdit iconModuleRepositoryItemPictureEdit;

	private GridColumn titleModuleGridColumn;

	private RepositoryItemCustomTextEdit titleRepositoryItemCustomTextEdit;

	private PanelControl emptyModulesListPanelControl;

	private LabelControl emptyModulesListMessageLabelControl;

	private SimpleButton addFirstModuleSimpleButton;

	private BulkCopyGridUserControl allDatabaseModulesGridView;

	private PopupMenu allModulesPopupMenu;

	private BarButtonItem deleteBarButtonItem;

	private BarManager allModulesBarManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ModulesGridPanelUserControl modulesGridPanelUserControl;

	private BarButtonItem moveUpBarButtonItem;

	private BarButtonItem moveDownBarButtonItem;

	private BarButtonItem moveToTopBarButtonItem;

	private BarButtonItem moveToBottomBarButtonItem;

	private BarButtonItem sortAlphabeticallyBarButtonItem;

	private BarButtonItem addModuleBarButtonItem;

	private BarButtonItem viewHistory;

	public event EventHandler ShowModuleControl;

	public ModuleSummaryUserControl(MetadataEditorUserControl mainControl)
	{
		InitializeComponent();
		base.BulkCopy = new SummaryBulkCopy(SharedObjectTypeEnum.ObjectType.Module, this);
		allDatabaseModulesGridView.Copy = base.BulkCopy;
		this.mainControl = mainControl;
		ObjectEventArgs = new ObjectEventArgs();
		LengthValidation.SetTitleOrNameLengthLimit(titleRepositoryItemCustomTextEdit);
		BarButtonItem barButtonItem = new BarButtonItem();
		barButtonItem.ItemClick += addModuleSimpleButton_Click;
		barButtonItem.Glyph = Resources.module_add_16;
		barButtonItem.Hint = "Add Subject Area";
		modulesGridPanelUserControl.InsertAdditionalButton(barButtonItem, 3);
	}

	private void ModuleSummaryUserControl_Load(object sender, EventArgs e)
	{
		AddEvents();
	}

	public override void RefreshData()
	{
		allDatabaseModulesGridView.RefreshData();
	}

	protected override void AddEvents()
	{
		CommonFunctionsPanels.AddEventsForSummaryTable(allDatabaseModulesGridView, SharedObjectTypeEnum.ObjectType.Module, allModulesPopupMenu, allModulesBarManager, this.ShowModuleControl, iconModuleGridColumn, deleteBarButtonItem, ObjectEventArgs, base.TreeMenu, null, this, modulesGridPanelUserControl, moveUpBarButtonItem, moveDownBarButtonItem, moveToTopBarButtonItem, moveToBottomBarButtonItem, sortAlphabeticallyBarButtonItem, FindForm());
		CommonFunctionsPanels.AddEventForAutoFilterRow(allDatabaseModulesGridView);
	}

	public override void CloseEditor()
	{
		allDatabaseModulesGridView.CloseEditor();
	}

	public void SaveColumns()
	{
		if (base.ObjectType.HasValue && base.ObjectType != SharedObjectTypeEnum.ObjectType.UnresolvedEntity)
		{
			UserViewData.SaveColumns(UserViewData.GetViewName(this, allDatabaseModulesGridView, base.ObjectType.Value), allDatabaseModulesGridView);
		}
	}

	public override void SetParameters(DBTreeNode node, CustomFieldsSupport customFieldsSupport, SharedObjectTypeEnum.ObjectType? objectType = null)
	{
		bool flag = false;
		if (!base.ObjectType.HasValue)
		{
			flag = true;
		}
		base.SetParameters(node, customFieldsSupport, objectType);
		mainControl.SetWaitformVisibility(visible: true);
		new CustomFieldsCellsTypesSupport(isForSummaryTable: true).SetCustomColumns(allDatabaseModulesGridView, customFieldsSupport, SharedObjectTypeEnum.ObjectType.Module, customFieldsAsArray: true);
		databaseId = node.DatabaseId;
		CommonFunctionsPanels.SetSummaryObjectTitle(allDatabaseModulesLabel, SharedObjectTypeEnum.ObjectType.Database, "Subject Areas", node.ParentNode.Title);
		List<ModuleWithoutDescriptionObject> dataByDatabaseWithoutDescription = DB.Module.GetDataByDatabaseWithoutDescription(node.Id);
		allDatabaseModulesGrid.DataSource = dataByDatabaseWithoutDescription;
		SetEmptyModulesListPanelVisibility(node != null && node.Nodes?.Count == 0 && !ModuleAddingFromComboControl);
		modulesGridPanelUserControl.CustomFields += mainControl.EditCustomFields;
		modulesGridPanelUserControl.SetRemoveButtonVisibility(value: true);
		modulesGridPanelUserControl.Initialize(SharedObjectTypeEnum.TypeToStringForMenu(SharedObjectTypeEnum.ObjectType.Module));
		modulesGridPanelUserControl.SetMainControl(mainControl);
		if (flag && !UserViewData.LoadColumns(UserViewData.GetViewName(this, allDatabaseModulesGridView, base.ObjectType.Value), allDatabaseModulesGridView))
		{
			CustomFieldsCellsTypesSupport.SortCustomColumns(allDatabaseModulesGridView);
			CommonFunctionsPanels.SetBestFitForColumns(allDatabaseModulesGridView);
		}
		SetTermSummaryOldCustomFieldsAndTitle(dataByDatabaseWithoutDescription);
		mainControl.SetWaitformVisibility(visible: false);
	}

	public void ReloadRows()
	{
		allDatabaseModulesGridView.BeginDataUpdate();
		ModuleWithoutDescriptionObject moduleWithoutDescriptionObject = allDatabaseModulesGridView.GetFocusedRow() as ModuleWithoutDescriptionObject;
		if (allDatabaseModulesGridView.ActiveEditor != null && !allDatabaseModulesGridView.ActiveEditor.OldEditValue.Equals(allDatabaseModulesGridView.ActiveEditor?.EditValue))
		{
			moduleWithoutDescriptionObject.Title = allDatabaseModulesGridView.ActiveEditor.EditValue as string;
			CommonFunctionsDatabase.UpdateObjectFromRow(allDatabaseModulesGridView, moduleWithoutDescriptionObject, SharedObjectTypeEnum.ObjectType.Module, null, null, null, FindForm());
		}
		List<ModuleWithoutDescriptionObject> dataByDatabaseWithoutDescription = DB.Module.GetDataByDatabaseWithoutDescription(base.Node.Id);
		allDatabaseModulesGrid.DataSource = dataByDatabaseWithoutDescription;
		allDatabaseModulesGridView.EndDataUpdate();
	}

	public override void ClearData()
	{
		allDatabaseModulesGrid.BeginUpdate();
		allDatabaseModulesGrid.DataSource = null;
		allDatabaseModulesGrid.EndUpdate();
	}

	public void SetEmptyModulesListPanelVisibility(bool visible)
	{
		if (visible)
		{
			emptyModulesListPanelControl.Visible = true;
			GridControl gridControl = allDatabaseModulesGrid;
			bool visible2 = (modulesGridPanelUserControl.Visible = false);
			gridControl.Visible = visible2;
			BackColor = SkinColors.ControlColorFromSystemColors;
		}
		else
		{
			emptyModulesListPanelControl.Visible = false;
			GridControl gridControl2 = allDatabaseModulesGrid;
			bool visible2 = (modulesGridPanelUserControl.Visible = true);
			gridControl2.Visible = visible2;
		}
	}

	private void allDatabaseModulesGridView_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		if (allDatabaseModulesGridView.FocusedRowHandle > 0 && (sender as GridView).FocusedColumn == titleModuleGridColumn)
		{
			if (string.IsNullOrEmpty(e.Value as string))
			{
				e.Valid = false;
				allDatabaseModulesGridView.SetColumnError(titleModuleGridColumn, "Title of the Subject Area can't be empty");
			}
			else
			{
				e.Valid = true;
				allDatabaseModulesGridView.ClearColumnErrors();
			}
		}
	}

	private void allDatabaseModulesGridView_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		e.ExceptionMode = ExceptionMode.NoAction;
	}

	private void addModuleSimpleButton_Click(object sender, EventArgs e)
	{
		DBTreeNode focusedNode = mainControl.GetFocusedNode();
		if (focusedNode == null || focusedNode.ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database || !Licenses.CheckRepositoryVersionAfterLogin() || !mainControl.ContinueAfterPossibleChanges())
		{
			return;
		}
		try
		{
			mainControl.MetadataTreeList.BeginUpdate();
			CommonFunctionsDatabase.AddNewModule(allDatabaseModulesGridView, databaseId, focusedNode, null, base.CustomFieldsSupport, base.ParentForm);
		}
		finally
		{
			mainControl.MetadataTreeList.EndUpdate();
		}
	}

	private void addFirstModuleSimpleButton_Click(object sender, EventArgs e)
	{
		mainControl.AddModule();
	}

	private void ModuleSummaryUserControl_Leave(object sender, EventArgs e)
	{
		allDatabaseModulesGridView.HideCustomization();
	}

	private void addModuleBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		DBTreeNode focusedNode = mainControl.GetFocusedNode();
		if (focusedNode == null || focusedNode.ObjectType != SharedObjectTypeEnum.ObjectType.Folder_Module_In_Database || !Licenses.CheckRepositoryVersionAfterLogin() || !mainControl.ContinueAfterPossibleChanges())
		{
			return;
		}
		int? num = allDatabaseModulesGridView.GetSelectedRows()?.LastOrDefault();
		if (!num.HasValue)
		{
			return;
		}
		try
		{
			mainControl.MetadataTreeList.BeginUpdate();
			CommonFunctionsDatabase.AddNewModule(allDatabaseModulesGridView, databaseId, focusedNode, num + 1, base.CustomFieldsSupport, base.ParentForm);
		}
		finally
		{
			mainControl.MetadataTreeList.EndUpdate();
		}
	}

	private void SetTermSummaryOldCustomFieldsAndTitle(List<ModuleWithoutDescriptionObject> data)
	{
		CommonFunctionsPanels.customFieldsForHistory = new Dictionary<int, Dictionary<string, BaseWithCustomFields.CustomFieldWithValue>>();
		CommonFunctionsPanels.summaryObjectTitleHistory = new Dictionary<int, string>();
		IEnumerable<GridColumn> enumerable = allDatabaseModulesGridView?.Columns?.Where((GridColumn x) => x.FieldName.Contains("Field"));
		if (enumerable == null)
		{
			return;
		}
		foreach (ModuleWithoutDescriptionObject objectWithModules in data)
		{
			ModuleWithoutDescriptionObject moduleWithoutDescriptionObject = objectWithModules;
			if (moduleWithoutDescriptionObject == null)
			{
				continue;
			}
			_ = moduleWithoutDescriptionObject.Id;
			if (0 == 0)
			{
				CommonFunctionsPanels.summaryObjectTitleHistory.Add(objectWithModules.Id, objectWithModules.Title);
				CommonFunctionsPanels.customFieldsForHistory.Add(objectWithModules.Id, enumerable.ToDictionary((GridColumn x) => x.FieldName, (GridColumn y) => BaseCustomFieldDB.GetCustomFieldWithValue(base.CustomFieldsSupport.GetField(y.FieldName), objectWithModules.GetField(y.FieldName)?.ToString())));
			}
		}
	}

	private void ViewHistoryBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		ModuleWithoutDescriptionObject moduleWithoutDescriptionObject = allDatabaseModulesGridView?.GetFocusedRow() as ModuleWithoutDescriptionObject;
		string field = allDatabaseModulesGridView?.FocusedColumn?.FieldName?.ToLower();
		if (moduleWithoutDescriptionObject == null || (field != "title" && base.CustomFieldsSupport.GetField(field) == null) || moduleWithoutDescriptionObject == null)
		{
			return;
		}
		try
		{
			using HistoryForm historyForm = new HistoryForm();
			historyForm.CustomFieldCaption = allDatabaseModulesGridView?.Columns?.Where((GridColumn x) => x.FieldName.ToLower() == field)?.FirstOrDefault()?.Caption;
			historyForm.SetParameters(moduleWithoutDescriptionObject.ModuleId, field, moduleWithoutDescriptionObject.Title, null, null, null, null, HistoryGeneralHelper.GetObjectTableInRepositoryByObjectType(SharedObjectTypeEnum.ObjectType.Module), SharedObjectTypeEnum.TypeToString(SharedObjectTypeEnum.ObjectType.Module), null, null, null);
			historyForm.ShowDialog(FindForm());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, FindForm());
		}
	}

	private void allModulesPopupMenu_BeforePopup(object sender, CancelEventArgs e)
	{
		if (viewHistory == null || allDatabaseModulesGridView == null || titleModuleGridColumn == null)
		{
			return;
		}
		BarButtonItem barButtonItem = viewHistory;
		if (allDatabaseModulesGridView.GetSelectedRows().Count() != 1)
		{
			goto IL_00a7;
		}
		if (allDatabaseModulesGridView?.FocusedColumn != titleModuleGridColumn)
		{
			BulkCopyGridUserControl bulkCopyGridUserControl = allDatabaseModulesGridView;
			if (bulkCopyGridUserControl == null || bulkCopyGridUserControl.FocusedColumn?.FieldName?.StartsWith("Field") != true)
			{
				goto IL_00a7;
			}
		}
		int visibility = 0;
		goto IL_00ab;
		IL_00ab:
		barButtonItem.Visibility = (BarItemVisibility)visibility;
		return;
		IL_00a7:
		visibility = 1;
		goto IL_00ab;
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
		this.allDatabaseModulesLabel = new DevExpress.XtraEditors.LabelControl();
		this.allDatabaseModulesGrid = new DevExpress.XtraGrid.GridControl();
		this.allDatabaseModulesGridView = new Dataedo.App.UserControls.BulkCopyGridUserControl();
		this.iconModuleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconModuleRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.titleModuleGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.titleRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.emptyModulesListPanelControl = new DevExpress.XtraEditors.PanelControl();
		this.addFirstModuleSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.emptyModulesListMessageLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.allModulesPopupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.addModuleBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveUpBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveDownBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveToTopBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveToBottomBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.sortAlphabeticallyBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.deleteBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.viewHistory = new DevExpress.XtraBars.BarButtonItem();
		this.allModulesBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.modulesGridPanelUserControl = new Dataedo.App.UserControls.PanelControls.ModulesGridPanelUserControl();
		((System.ComponentModel.ISupportInitialize)this.allDatabaseModulesGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allDatabaseModulesGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconModuleRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptyModulesListPanelControl).BeginInit();
		this.emptyModulesListPanelControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.allModulesPopupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allModulesBarManager).BeginInit();
		base.SuspendLayout();
		this.allDatabaseModulesLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 238);
		this.allDatabaseModulesLabel.Appearance.Options.UseFont = true;
		this.allDatabaseModulesLabel.Appearance.Options.UseTextOptions = true;
		this.allDatabaseModulesLabel.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.allDatabaseModulesLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.allDatabaseModulesLabel.Dock = System.Windows.Forms.DockStyle.Top;
		this.allDatabaseModulesLabel.Location = new System.Drawing.Point(0, 0);
		this.allDatabaseModulesLabel.Name = "allDatabaseModulesLabel";
		this.allDatabaseModulesLabel.Padding = new System.Windows.Forms.Padding(0, 4, 4, 4);
		this.allDatabaseModulesLabel.Size = new System.Drawing.Size(1070, 19);
		this.allDatabaseModulesLabel.TabIndex = 11;
		this.allDatabaseModulesLabel.Text = "Subject Areas in the database";
		this.allDatabaseModulesLabel.UseMnemonic = false;
		this.allDatabaseModulesGrid.Cursor = System.Windows.Forms.Cursors.Default;
		this.allDatabaseModulesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
		this.allDatabaseModulesGrid.Location = new System.Drawing.Point(0, 117);
		this.allDatabaseModulesGrid.MainView = this.allDatabaseModulesGridView;
		this.allDatabaseModulesGrid.Name = "allDatabaseModulesGrid";
		this.allDatabaseModulesGrid.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.iconModuleRepositoryItemPictureEdit, this.titleRepositoryItemCustomTextEdit });
		this.allDatabaseModulesGrid.Size = new System.Drawing.Size(1070, 321);
		this.allDatabaseModulesGrid.TabIndex = 12;
		this.allDatabaseModulesGrid.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.allDatabaseModulesGridView });
		this.allDatabaseModulesGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.iconModuleGridColumn, this.titleModuleGridColumn });
		defaultBulkCopy.IsCopying = false;
		this.allDatabaseModulesGridView.Copy = defaultBulkCopy;
		this.allDatabaseModulesGridView.GridControl = this.allDatabaseModulesGrid;
		this.allDatabaseModulesGridView.Name = "allDatabaseModulesGridView";
		this.allDatabaseModulesGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.allDatabaseModulesGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.allDatabaseModulesGridView.OptionsSelection.MultiSelect = true;
		this.allDatabaseModulesGridView.OptionsView.ColumnAutoWidth = false;
		this.allDatabaseModulesGridView.OptionsView.RowAutoHeight = true;
		this.allDatabaseModulesGridView.OptionsView.ShowGroupPanel = false;
		this.allDatabaseModulesGridView.OptionsView.ShowIndicator = false;
		this.allDatabaseModulesGridView.RowHighlightingIsEnabled = true;
		this.allDatabaseModulesGridView.SplashScreenManager = null;
		this.allDatabaseModulesGridView.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(allDatabaseModulesGridView_ValidatingEditor);
		this.allDatabaseModulesGridView.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(allDatabaseModulesGridView_InvalidValueException);
		this.iconModuleGridColumn.Caption = " ";
		this.iconModuleGridColumn.ColumnEdit = this.iconModuleRepositoryItemPictureEdit;
		this.iconModuleGridColumn.FieldName = "iconTableGridColumn";
		this.iconModuleGridColumn.MaxWidth = 21;
		this.iconModuleGridColumn.MinWidth = 21;
		this.iconModuleGridColumn.Name = "iconModuleGridColumn";
		this.iconModuleGridColumn.OptionsColumn.AllowEdit = false;
		this.iconModuleGridColumn.OptionsFilter.AllowFilter = false;
		this.iconModuleGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.iconModuleGridColumn.Visible = true;
		this.iconModuleGridColumn.VisibleIndex = 0;
		this.iconModuleGridColumn.Width = 21;
		this.iconModuleRepositoryItemPictureEdit.AllowFocused = false;
		this.iconModuleRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.iconModuleRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconModuleRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconModuleRepositoryItemPictureEdit.Name = "iconModuleRepositoryItemPictureEdit";
		this.iconModuleRepositoryItemPictureEdit.ShowMenu = false;
		this.titleModuleGridColumn.Caption = "Title";
		this.titleModuleGridColumn.ColumnEdit = this.titleRepositoryItemCustomTextEdit;
		this.titleModuleGridColumn.FieldName = "Title";
		this.titleModuleGridColumn.Name = "titleModuleGridColumn";
		this.titleModuleGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.titleModuleGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.titleModuleGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.titleModuleGridColumn.Tag = "FIT_WIDTH";
		this.titleModuleGridColumn.Visible = true;
		this.titleModuleGridColumn.VisibleIndex = 1;
		this.titleModuleGridColumn.Width = 250;
		this.titleRepositoryItemCustomTextEdit.AutoHeight = false;
		this.titleRepositoryItemCustomTextEdit.Name = "titleRepositoryItemCustomTextEdit";
		this.emptyModulesListPanelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.emptyModulesListPanelControl.Controls.Add(this.addFirstModuleSimpleButton);
		this.emptyModulesListPanelControl.Controls.Add(this.emptyModulesListMessageLabelControl);
		this.emptyModulesListPanelControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.emptyModulesListPanelControl.Location = new System.Drawing.Point(0, 19);
		this.emptyModulesListPanelControl.MaximumSize = new System.Drawing.Size(0, 76);
		this.emptyModulesListPanelControl.MinimumSize = new System.Drawing.Size(0, 70);
		this.emptyModulesListPanelControl.Name = "emptyModulesListPanelControl";
		this.emptyModulesListPanelControl.Padding = new System.Windows.Forms.Padding(1, 0, 0, 0);
		this.emptyModulesListPanelControl.Size = new System.Drawing.Size(1070, 70);
		this.emptyModulesListPanelControl.TabIndex = 14;
		this.emptyModulesListPanelControl.Visible = false;
		this.addFirstModuleSimpleButton.ImageOptions.Image = Dataedo.App.Properties.Resources.module_add_16;
		this.addFirstModuleSimpleButton.Location = new System.Drawing.Point(1, 40);
		this.addFirstModuleSimpleButton.Name = "addFirstModuleSimpleButton";
		this.addFirstModuleSimpleButton.Size = new System.Drawing.Size(143, 23);
		this.addFirstModuleSimpleButton.TabIndex = 13;
		this.addFirstModuleSimpleButton.Text = "Add first Subject Area";
		this.addFirstModuleSimpleButton.Click += new System.EventHandler(addFirstModuleSimpleButton_Click);
		this.emptyModulesListMessageLabelControl.AllowHtmlString = true;
		this.emptyModulesListMessageLabelControl.Appearance.Options.UseTextOptions = true;
		this.emptyModulesListMessageLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.emptyModulesListMessageLabelControl.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.emptyModulesListMessageLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.emptyModulesListMessageLabelControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.emptyModulesListMessageLabelControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.emptyModulesListMessageLabelControl.Location = new System.Drawing.Point(1, 0);
		this.emptyModulesListMessageLabelControl.Name = "emptyModulesListMessageLabelControl";
		this.emptyModulesListMessageLabelControl.Size = new System.Drawing.Size(1069, 32);
		this.emptyModulesListMessageLabelControl.TabIndex = 12;
		this.emptyModulesListMessageLabelControl.Text = "Subject Areas are your custom \"folders\" to group database objects and describe topics related to your database.\r\nAssign tables, views etc., write a narrative and visualize schema with <b>ERD</b>.";
		this.allModulesPopupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[8]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.addModuleBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.moveUpBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.moveDownBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.moveToTopBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.moveToBottomBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.sortAlphabeticallyBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.deleteBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.viewHistory)
		});
		this.allModulesPopupMenu.Manager = this.allModulesBarManager;
		this.allModulesPopupMenu.Name = "allModulesPopupMenu";
		this.allModulesPopupMenu.BeforePopup += new System.ComponentModel.CancelEventHandler(allModulesPopupMenu_BeforePopup);
		this.addModuleBarButtonItem.Caption = "Add Subject Area";
		this.addModuleBarButtonItem.Id = 8;
		this.addModuleBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.module_add_16;
		this.addModuleBarButtonItem.Name = "addModuleBarButtonItem";
		this.addModuleBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addModuleBarButtonItem_ItemClick);
		this.moveUpBarButtonItem.Caption = "Move up";
		this.moveUpBarButtonItem.Id = 2;
		this.moveUpBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_up_16;
		this.moveUpBarButtonItem.Name = "moveUpBarButtonItem";
		this.moveDownBarButtonItem.Caption = "Move down";
		this.moveDownBarButtonItem.Id = 3;
		this.moveDownBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_down_16;
		this.moveDownBarButtonItem.Name = "moveDownBarButtonItem";
		this.moveToTopBarButtonItem.Caption = "Move to top";
		this.moveToTopBarButtonItem.Id = 4;
		this.moveToTopBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_top_16;
		this.moveToTopBarButtonItem.Name = "moveToTopBarButtonItem";
		this.moveToBottomBarButtonItem.Caption = "Move to bottom";
		this.moveToBottomBarButtonItem.Id = 5;
		this.moveToBottomBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_bottom_16;
		this.moveToBottomBarButtonItem.Name = "moveToBottomBarButtonItem";
		this.sortAlphabeticallyBarButtonItem.Caption = "Sort alphabetically";
		this.sortAlphabeticallyBarButtonItem.Id = 6;
		this.sortAlphabeticallyBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.sort_asc_16;
		this.sortAlphabeticallyBarButtonItem.Name = "sortAlphabeticallyBarButtonItem";
		this.deleteBarButtonItem.Caption = "Remove from repository";
		this.deleteBarButtonItem.Id = 0;
		this.deleteBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.deleteBarButtonItem.Name = "deleteBarButtonItem";
		this.viewHistory.Caption = "View history";
		this.viewHistory.Id = 9;
		this.viewHistory.ImageOptions.Image = Dataedo.App.Properties.Resources.search_16;
		this.viewHistory.Name = "viewHistory";
		this.viewHistory.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
		this.viewHistory.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ViewHistoryBarButtonItem_ItemClick);
		this.allModulesBarManager.DockControls.Add(this.barDockControlTop);
		this.allModulesBarManager.DockControls.Add(this.barDockControlBottom);
		this.allModulesBarManager.DockControls.Add(this.barDockControlLeft);
		this.allModulesBarManager.DockControls.Add(this.barDockControlRight);
		this.allModulesBarManager.Form = this;
		this.allModulesBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[8] { this.deleteBarButtonItem, this.moveUpBarButtonItem, this.moveDownBarButtonItem, this.moveToTopBarButtonItem, this.moveToBottomBarButtonItem, this.sortAlphabeticallyBarButtonItem, this.addModuleBarButtonItem, this.viewHistory });
		this.allModulesBarManager.MaxItemId = 10;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.allModulesBarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(1070, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 438);
		this.barDockControlBottom.Manager = this.allModulesBarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(1070, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.allModulesBarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 438);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1070, 0);
		this.barDockControlRight.Manager = this.allModulesBarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 438);
		this.modulesGridPanelUserControl.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
		this.modulesGridPanelUserControl.Dock = System.Windows.Forms.DockStyle.Top;
		this.modulesGridPanelUserControl.GridView = this.allDatabaseModulesGridView;
		this.modulesGridPanelUserControl.Location = new System.Drawing.Point(0, 89);
		this.modulesGridPanelUserControl.Name = "modulesGridPanelUserControl";
		this.modulesGridPanelUserControl.Size = new System.Drawing.Size(1070, 28);
		this.modulesGridPanelUserControl.TabIndex = 19;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.allDatabaseModulesGrid);
		base.Controls.Add(this.modulesGridPanelUserControl);
		base.Controls.Add(this.emptyModulesListPanelControl);
		base.Controls.Add(this.allDatabaseModulesLabel);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "ModuleSummaryUserControl";
		base.Size = new System.Drawing.Size(1070, 438);
		base.Load += new System.EventHandler(ModuleSummaryUserControl_Load);
		base.Leave += new System.EventHandler(ModuleSummaryUserControl_Leave);
		((System.ComponentModel.ISupportInitialize)this.allDatabaseModulesGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allDatabaseModulesGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconModuleRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptyModulesListPanelControl).EndInit();
		this.emptyModulesListPanelControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.allModulesPopupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allModulesBarManager).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
