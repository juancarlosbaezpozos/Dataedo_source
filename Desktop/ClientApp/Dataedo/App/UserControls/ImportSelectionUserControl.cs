using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.DatabasesSupport;
using Dataedo.App.Enums;
using Dataedo.App.Licences;
using Dataedo.App.Tools;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls.Base;
using Dataedo.App.UserControls.Interfaces;
using Dataedo.CustomControls;
using Dataedo.Shared.DatabasesSupport;
using Dataedo.Shared.Enums;
using DevExpress.Data.Filtering;
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
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Dataedo.App.UserControls;

public class ImportSelectionUserControl : BaseUserControl, ISelectImportControl
{
	private int? databaseId;

	private bool disableSearchTextEditEvents;

	private IContainer components;

	private NonCustomizableLayoutControl nonCustomizableLayoutControl;

	private LayoutControlGroup Root;

	private TreeList folderTreeList;

	private LayoutControlItem layoutControlItem1;

	private EmptySpaceItem emptySpaceItem3;

	private EmptySpaceItem emptySpaceItem2;

	private TreeListColumn folderNameColumn;

	private GridControl dbmsGridControl;

	private GridView dbmsGridView;

	private GridColumn dbmsIconGridColumn;

	private GridColumn dbmsNameGridColumn;

	private GridColumn typeGridColumn;

	private GridColumn upgradeUrlColumn;

	private RepositoryItemHypertextLabel LabelURL;

	private LayoutControlItem layoutControlItem2;

	private ButtonEdit searchTextEdit;

	private LayoutControlItem searchEditLayoutControlItem;

	private LabelControl noResultsLabelControl;

	private LayoutControlGroup dbmsGridLayoutControlGroup;

	private LayoutControlGroup noItemsLayoutControlGroup;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem4;

	private EmptySpaceItem emptySpaceItem5;

	private LabelControl suggestDataSourceLabelControl;

	private LayoutControlItem suggestDataSourceLayoutControlItem;

	private EmptySpaceItem emptySpaceItem6;

	public DBMSGridModel DBMSGridModel { get; private set; }

	public SharedDatabaseTypeEnum.DatabaseType? SelectedDatabaseType { get; set; }

	public bool IsDBAdded { get; private set; }

	private List<FoldersGridObject> FoldersGridObjects { get; set; }

	public event EventHandler FocusedDatabaseTypeChanged;

	public event EventHandler DbmsGridViewDoubleClick;

	public ImportSelectionUserControl()
	{
		InitializeComponent();
	}

	public void SetParameters(int? databaseId = null, SharedDatabaseTypeEnum.DatabaseType? databaseType = null, bool showOnlyDatabaseSubtypesItems = false)
	{
		CreateFolders();
		folderTreeList.DataSource = FoldersGridObjects;
		this.databaseId = databaseId;
		if (!showOnlyDatabaseSubtypesItems)
		{
			DatabaseTypes.CreateDatabaseTypesDataSource();
			dbmsGridControl.DataSource = DatabaseTypes.databaseTypesDataSource;
		}
		else
		{
			DatabaseTypes.CreateDatabaseSubtypesDataSource(DatabaseSupportFactory.GetParentDatabaseType(databaseType));
			dbmsGridControl.DataSource = DatabaseTypes.databaseSubtypesDataSource;
		}
		if (!this.databaseId.HasValue)
		{
			IsDBAdded = true;
		}
		else
		{
			IsDBAdded = false;
			SelectedDatabaseType = databaseType;
			if (DatabaseSupportFactoryShared.CheckIfTypeIsSupported(databaseType))
			{
				List<DBMSGridModel> list = dbmsGridView.DataSource as List<DBMSGridModel>;
				DBMSGridModel item = list.FirstOrDefault((DBMSGridModel x) => x.Type.Equals(DatabaseTypeEnum.TypeToString(databaseType)));
				dbmsGridView.FocusedRowHandle = list.IndexOf(item);
				dbmsGridView.Focus();
			}
		}
		InvokeDbmsGridView_FocusedRowChanged();
	}

	private void FolderTreeList_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		if (e.Node == null)
		{
			return;
		}
		object row = folderTreeList.GetRow(e.Node.Id);
		FoldersGridObject foldersGridObject = row as FoldersGridObject;
		if (foldersGridObject == null)
		{
			return;
		}
		RemoveSearchFilter();
		SetSearchTextEditWithoutEvents(null);
		if (foldersGridObject.IsAllSources)
		{
			dbmsGridControl.DataSource = DatabaseTypes.databaseTypesDataSource;
		}
		else
		{
			dbmsGridControl.DataSource = DatabaseTypes.databaseTypesDataSource.Where((DBMSGridModel x) => x.ImportFolders.Contains(foldersGridObject.ImportFolder.Value));
		}
		InvokeDbmsGridView_FocusedRowChanged();
	}

	private void SetSearchTextEditWithoutEvents(string text)
	{
		try
		{
			disableSearchTextEditEvents = true;
			searchTextEdit.Text = text;
		}
		finally
		{
			disableSearchTextEditEvents = false;
		}
	}

	private void CreateFolders()
	{
		FoldersGridObjects = new List<FoldersGridObject>();
		FoldersGridObjects.Add(new FoldersGridObject
		{
			FolderName = "All sources",
			IsAllSources = true
		});
		foreach (SharedImportFolderEnum.ImportFolder item in SharedImportFolderEnum.GetOrderedImportFoldersForImport())
		{
			FoldersGridObjects.Add(new FoldersGridObject
			{
				FolderName = SharedImportFolderEnum.GetFriendlyDisplayNameForImport(item),
				ImportFolder = item
			});
		}
	}

	private void SearchTextEdit_EditValueChanging(object sender, ChangingEventArgs e)
	{
		if (folderTreeList.Nodes != null && folderTreeList.Nodes.Count != 0 && !disableSearchTextEditEvents)
		{
			folderTreeList.FocusedNode = folderTreeList.Nodes[0];
			if (!string.IsNullOrEmpty(e.NewValue as string))
			{
				ApplySearchFilter(e.NewValue.ToString());
			}
			else
			{
				RemoveSearchFilter();
			}
			InvokeDbmsGridView_FocusedRowChanged();
		}
	}

	private void SearchTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		if (searchTextEdit.Properties.Buttons.Any())
		{
			if (string.IsNullOrEmpty(searchTextEdit.EditValue as string))
			{
				searchTextEdit.Properties.Buttons[0].Visible = false;
			}
			else
			{
				searchTextEdit.Properties.Buttons[0].Visible = true;
			}
			SetDbmsGridVisibility();
		}
	}

	private void SetDbmsGridVisibility()
	{
		if (dbmsGridView.DataRowCount == 0)
		{
			dbmsGridLayoutControlGroup.Visibility = LayoutVisibility.Never;
			noItemsLayoutControlGroup.Visibility = LayoutVisibility.Always;
		}
		else
		{
			dbmsGridLayoutControlGroup.Visibility = LayoutVisibility.Always;
			noItemsLayoutControlGroup.Visibility = LayoutVisibility.Never;
		}
	}

	private void InvokeDbmsGridView_FocusedRowChanged()
	{
		DbmsGridView_FocusedRowChanged(dbmsGridControl, new FocusedRowChangedEventArgs(-1, dbmsGridView.FocusedRowHandle));
	}

	private void ApplySearchFilter(string searchValue)
	{
		dbmsGridView.ActiveFilterCriteria = new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty("Name"), searchValue);
	}

	private void RemoveSearchFilter()
	{
		dbmsGridView.ActiveFilterCriteria = null;
	}

	private void DbmsGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		DBMSGridModel = dbmsGridView.GetRow(e.FocusedRowHandle) as DBMSGridModel;
		if (DBMSGridModel == null)
		{
			this.FocusedDatabaseTypeChanged?.Invoke(sender, e);
			return;
		}
		SelectedDatabaseType = (string.IsNullOrWhiteSpace(DBMSGridModel.Type) ? null : DatabaseTypeEnum.StringToType(DBMSGridModel.Type));
		this.FocusedDatabaseTypeChanged?.Invoke(sender, e);
	}

	private void DbmsGridView_MouseDown(object sender, MouseEventArgs e)
	{
		if (!Connectors.HasAllConnectors() && sender is GridView gridView)
		{
			GridHitInfo gridHitInfo = gridView.CalcHitInfo(e.Location);
			if (gridHitInfo != null && gridHitInfo.RowHandle >= 0 && gridView.GetRow(gridHitInfo.RowHandle) is DBMSGridModel dBMSGridModel && gridHitInfo.Column.FieldName == "URL" && !dBMSGridModel.IsConnectorInLicense)
			{
				Links.OpenLink(Links.ManageAccounts);
			}
		}
	}

	private void DbmsGridView_RowStyle(object sender, RowStyleEventArgs e)
	{
		DontShowManualDataSource();
		if (!Connectors.HasAllConnectors() && e.RowHandle >= 0 && sender is GridView gridView && gridView.GetRow(e.RowHandle) is DBMSGridModel dBMSGridModel && !dBMSGridModel.IsConnectorInLicense)
		{
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridDisabledGridRowForeColor;
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
		}
	}

	private void DontShowManualDataSource()
	{
		List<DBMSGridModel> list = dbmsGridView?.DataSource as List<DBMSGridModel>;
		DBMSGridModel dBMSGridModel = list?.FirstOrDefault((DBMSGridModel x) => x.Type.Equals(DatabaseTypeEnum.TypeToString(SharedDatabaseTypeEnum.DatabaseType.Manual)));
		if (dBMSGridModel != null && list != null && list.Count > 0)
		{
			int num = dbmsGridView.LocateByValue("Type", dBMSGridModel.Type);
			if (num >= 0)
			{
				dbmsGridView.DeleteRow(num);
				dbmsGridView.FocusedRowHandle = 0;
			}
		}
	}

	public DBMSGridModel GetFocusedDBMSGridModel()
	{
		if (!(dbmsGridView.GetFocusedRow() is DBMSGridModel dBMSGridModel))
		{
			return null;
		}
		SelectedDatabaseType = (string.IsNullOrWhiteSpace(dBMSGridModel.Type) ? null : DatabaseTypeEnum.StringToType(dBMSGridModel.Type));
		return dBMSGridModel;
	}

	private void DbmsGridView_DoubleClick(object sender, EventArgs e)
	{
		if (e is DXMouseEventArgs dXMouseEventArgs)
		{
			GridHitInfo gridHitInfo = dbmsGridView.CalcHitInfo(dXMouseEventArgs.Location);
			if (gridHitInfo != null && gridHitInfo.InRow)
			{
				this.DbmsGridViewDoubleClick?.Invoke(sender, e);
			}
		}
	}

	private void SearchTextEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		searchTextEdit.EditValue = null;
	}

	private void SuggestDataSourceLabelControl_HyperlinkClick(object sender, HyperlinkClickEventArgs e)
	{
		Links.OpenLink(Links.SuggestDataSource, FindForm());
	}

	private void DbmsGridView_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Tab)
		{
			if (e.Modifiers == Keys.Shift)
			{
				dbmsGridView.MovePrev();
			}
			else
			{
				dbmsGridView.MoveNext();
			}
			e.Handled = true;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.nonCustomizableLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.suggestDataSourceLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.noResultsLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.searchTextEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.dbmsGridControl = new DevExpress.XtraGrid.GridControl();
		this.dbmsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.dbmsIconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.dbmsNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.typeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.upgradeUrlColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.LabelURL = new DevExpress.XtraEditors.Repository.RepositoryItemHypertextLabel();
		this.folderTreeList = new DevExpress.XtraTreeList.TreeList();
		this.folderNameColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.searchEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.dbmsGridLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.noItemsLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.suggestDataSourceLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).BeginInit();
		this.nonCustomizableLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.searchTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.LabelURL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.folderTreeList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.searchEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsGridLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.noItemsLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.suggestDataSourceLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		base.SuspendLayout();
		this.nonCustomizableLayoutControl.AllowCustomization = false;
		this.nonCustomizableLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.nonCustomizableLayoutControl.Controls.Add(this.suggestDataSourceLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.noResultsLabelControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.searchTextEdit);
		this.nonCustomizableLayoutControl.Controls.Add(this.dbmsGridControl);
		this.nonCustomizableLayoutControl.Controls.Add(this.folderTreeList);
		this.nonCustomizableLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.nonCustomizableLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.nonCustomizableLayoutControl.Name = "nonCustomizableLayoutControl";
		this.nonCustomizableLayoutControl.Root = this.Root;
		this.nonCustomizableLayoutControl.Size = new System.Drawing.Size(647, 536);
		this.nonCustomizableLayoutControl.TabIndex = 0;
		this.nonCustomizableLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.suggestDataSourceLabelControl.AllowHtmlString = true;
		this.suggestDataSourceLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.suggestDataSourceLabelControl.Appearance.Options.UseFont = true;
		this.suggestDataSourceLabelControl.Appearance.Options.UseTextOptions = true;
		this.suggestDataSourceLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.suggestDataSourceLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.suggestDataSourceLabelControl.Location = new System.Drawing.Point(223, 454);
		this.suggestDataSourceLabelControl.Name = "suggestDataSourceLabelControl";
		this.suggestDataSourceLabelControl.Size = new System.Drawing.Size(418, 16);
		this.suggestDataSourceLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.suggestDataSourceLabelControl.TabIndex = 31;
		this.suggestDataSourceLabelControl.Text = "<href>Suggest a new data source.</href>";
		this.suggestDataSourceLabelControl.HyperlinkClick += new DevExpress.Utils.HyperlinkClickEventHandler(SuggestDataSourceLabelControl_HyperlinkClick);
		this.noResultsLabelControl.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 238);
		this.noResultsLabelControl.Appearance.ForeColor = System.Drawing.Color.Gray;
		this.noResultsLabelControl.Appearance.Options.UseFont = true;
		this.noResultsLabelControl.Appearance.Options.UseForeColor = true;
		this.noResultsLabelControl.Appearance.Options.UseTextOptions = true;
		this.noResultsLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.noResultsLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.noResultsLabelControl.Location = new System.Drawing.Point(223, 408);
		this.noResultsLabelControl.Name = "noResultsLabelControl";
		this.noResultsLabelControl.Size = new System.Drawing.Size(418, 32);
		this.noResultsLabelControl.StyleController = this.nonCustomizableLayoutControl;
		this.noResultsLabelControl.TabIndex = 30;
		this.noResultsLabelControl.Text = "No results were found.\r\nClear the search filter, to display all data sources.";
		this.searchTextEdit.Location = new System.Drawing.Point(2, 2);
		this.searchTextEdit.Name = "searchTextEdit";
		this.searchTextEdit.Properties.AutoHeight = false;
		this.searchTextEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.searchTextEdit.Properties.NullValuePrompt = "Type here to Search...";
		this.searchTextEdit.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(SearchTextEdit_ButtonClick);
		this.searchTextEdit.Size = new System.Drawing.Size(201, 41);
		this.searchTextEdit.StyleController = this.nonCustomizableLayoutControl;
		this.searchTextEdit.TabIndex = 29;
		this.searchTextEdit.EditValueChanged += new System.EventHandler(SearchTextEdit_EditValueChanged);
		this.searchTextEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(SearchTextEdit_EditValueChanging);
		this.dbmsGridControl.Location = new System.Drawing.Point(221, 4);
		this.dbmsGridControl.MainView = this.dbmsGridView;
		this.dbmsGridControl.Name = "dbmsGridControl";
		this.dbmsGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.LabelURL });
		this.dbmsGridControl.Size = new System.Drawing.Size(422, 376);
		this.dbmsGridControl.TabIndex = 28;
		this.dbmsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.dbmsGridView });
		this.dbmsGridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.dbmsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.dbmsIconGridColumn, this.dbmsNameGridColumn, this.typeGridColumn, this.upgradeUrlColumn });
		this.dbmsGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.dbmsGridView.GridControl = this.dbmsGridControl;
		this.dbmsGridView.Name = "dbmsGridView";
		this.dbmsGridView.OptionsBehavior.AutoPopulateColumns = false;
		this.dbmsGridView.OptionsBehavior.Editable = false;
		this.dbmsGridView.OptionsBehavior.ReadOnly = true;
		this.dbmsGridView.OptionsCustomization.AllowFilter = false;
		this.dbmsGridView.OptionsDetail.EnableMasterViewMode = false;
		this.dbmsGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.dbmsGridView.OptionsView.ShowColumnHeaders = false;
		this.dbmsGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.dbmsGridView.OptionsView.ShowGroupPanel = false;
		this.dbmsGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.dbmsGridView.OptionsView.ShowIndicator = false;
		this.dbmsGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.dbmsGridView.RowHeight = 25;
		this.dbmsGridView.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(DbmsGridView_RowStyle);
		this.dbmsGridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(DbmsGridView_FocusedRowChanged);
		this.dbmsGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(DbmsGridView_KeyDown);
		this.dbmsGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(DbmsGridView_MouseDown);
		this.dbmsGridView.DoubleClick += new System.EventHandler(DbmsGridView_DoubleClick);
		this.dbmsIconGridColumn.FieldName = "Image";
		this.dbmsIconGridColumn.MaxWidth = 20;
		this.dbmsIconGridColumn.Name = "dbmsIconGridColumn";
		this.dbmsIconGridColumn.Visible = true;
		this.dbmsIconGridColumn.VisibleIndex = 0;
		this.dbmsIconGridColumn.Width = 20;
		this.dbmsNameGridColumn.FieldName = "Name";
		this.dbmsNameGridColumn.Name = "dbmsNameGridColumn";
		this.dbmsNameGridColumn.Visible = true;
		this.dbmsNameGridColumn.VisibleIndex = 1;
		this.dbmsNameGridColumn.Width = 348;
		this.typeGridColumn.FieldName = "Type";
		this.typeGridColumn.Name = "typeGridColumn";
		this.upgradeUrlColumn.Caption = "gridColumn1";
		this.upgradeUrlColumn.ColumnEdit = this.LabelURL;
		this.upgradeUrlColumn.FieldName = "URL";
		this.upgradeUrlColumn.MinWidth = 120;
		this.upgradeUrlColumn.Name = "upgradeUrlColumn";
		this.upgradeUrlColumn.Visible = true;
		this.upgradeUrlColumn.VisibleIndex = 2;
		this.upgradeUrlColumn.Width = 120;
		this.LabelURL.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.LabelURL.Name = "LabelURL";
		this.folderTreeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[1] { this.folderNameColumn });
		this.folderTreeList.CustomizationFormBounds = new System.Drawing.Rectangle(1772, 406, 250, 280);
		this.folderTreeList.Location = new System.Drawing.Point(2, 57);
		this.folderTreeList.Margin = new System.Windows.Forms.Padding(0, 2, 0, 2);
		this.folderTreeList.Name = "folderTreeList";
		this.folderTreeList.OptionsBehavior.AllowExpandOnDblClick = false;
		this.folderTreeList.OptionsBehavior.AutoPopulateColumns = false;
		this.folderTreeList.OptionsBehavior.Editable = false;
		this.folderTreeList.OptionsBehavior.ReadOnly = true;
		this.folderTreeList.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.folderTreeList.OptionsCustomization.AllowColumnMoving = false;
		this.folderTreeList.OptionsCustomization.AllowQuickHideColumns = false;
		this.folderTreeList.OptionsMenu.ShowExpandCollapseItems = false;
		this.folderTreeList.OptionsPrint.AutoRowHeight = false;
		this.folderTreeList.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.folderTreeList.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFocus;
		this.folderTreeList.OptionsView.ShowBandsMode = DevExpress.Utils.DefaultBoolean.False;
		this.folderTreeList.OptionsView.ShowButtons = false;
		this.folderTreeList.OptionsView.ShowColumns = false;
		this.folderTreeList.OptionsView.ShowHorzLines = false;
		this.folderTreeList.OptionsView.ShowIndicator = false;
		this.folderTreeList.OptionsView.ShowRoot = false;
		this.folderTreeList.OptionsView.ShowVertLines = false;
		this.folderTreeList.OptionsView.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.None;
		this.folderTreeList.RowHeight = 28;
		this.folderTreeList.Size = new System.Drawing.Size(201, 477);
		this.folderTreeList.TabIndex = 5;
		this.folderTreeList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(FolderTreeList_FocusedNodeChanged);
		this.folderNameColumn.Caption = "folderNameColumn";
		this.folderNameColumn.FieldName = "FolderName";
		this.folderNameColumn.Name = "folderNameColumn";
		this.folderNameColumn.Visible = true;
		this.folderNameColumn.VisibleIndex = 0;
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.layoutControlItem1, this.emptySpaceItem3, this.emptySpaceItem2, this.searchEditLayoutControlItem, this.dbmsGridLayoutControlGroup, this.noItemsLayoutControlGroup });
		this.Root.Name = "Root";
		this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.Root.Size = new System.Drawing.Size(647, 536);
		this.Root.TextVisible = false;
		this.layoutControlItem1.Control = this.folderTreeList;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 55);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(205, 0);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(205, 24);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(205, 481);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 45);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(205, 10);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(205, 0);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(12, 0);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(12, 10);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(12, 536);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.searchEditLayoutControlItem.Control = this.searchTextEdit;
		this.searchEditLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.searchEditLayoutControlItem.MaxSize = new System.Drawing.Size(0, 45);
		this.searchEditLayoutControlItem.MinSize = new System.Drawing.Size(54, 45);
		this.searchEditLayoutControlItem.Name = "searchEditLayoutControlItem";
		this.searchEditLayoutControlItem.Size = new System.Drawing.Size(205, 45);
		this.searchEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.searchEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.searchEditLayoutControlItem.TextVisible = false;
		this.dbmsGridLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem2 });
		this.dbmsGridLayoutControlGroup.Location = new System.Drawing.Point(217, 0);
		this.dbmsGridLayoutControlGroup.Name = "dbmsGridLayoutControlGroup";
		this.dbmsGridLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 1, 1, 1);
		this.dbmsGridLayoutControlGroup.Size = new System.Drawing.Size(430, 384);
		this.dbmsGridLayoutControlGroup.TextVisible = false;
		this.layoutControlItem2.Control = this.dbmsGridControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlItem2.Size = new System.Drawing.Size(422, 376);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.noItemsLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[6] { this.layoutControlItem3, this.emptySpaceItem4, this.emptySpaceItem5, this.suggestDataSourceLayoutControlItem, this.emptySpaceItem6, this.emptySpaceItem1 });
		this.noItemsLayoutControlGroup.Location = new System.Drawing.Point(217, 384);
		this.noItemsLayoutControlGroup.Name = "noItemsLayoutControlGroup";
		this.noItemsLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(1, 1, 1, 1);
		this.noItemsLayoutControlGroup.Size = new System.Drawing.Size(430, 152);
		this.noItemsLayoutControlGroup.TextVisible = false;
		this.noItemsLayoutControlGroup.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layoutControlItem3.Control = this.noResultsLabelControl;
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 18);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(0, 36);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(16, 36);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(422, 36);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.Location = new System.Drawing.Point(0, 54);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(0, 10);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(10, 10);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(422, 10);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.Location = new System.Drawing.Point(0, 84);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(422, 13);
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.suggestDataSourceLayoutControlItem.Control = this.suggestDataSourceLabelControl;
		this.suggestDataSourceLayoutControlItem.Location = new System.Drawing.Point(0, 64);
		this.suggestDataSourceLayoutControlItem.MaxSize = new System.Drawing.Size(0, 20);
		this.suggestDataSourceLayoutControlItem.MinSize = new System.Drawing.Size(16, 20);
		this.suggestDataSourceLayoutControlItem.Name = "suggestDataSourceLayoutControlItem";
		this.suggestDataSourceLayoutControlItem.Size = new System.Drawing.Size(422, 20);
		this.suggestDataSourceLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.suggestDataSourceLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.suggestDataSourceLayoutControlItem.TextVisible = false;
		this.emptySpaceItem6.AllowHotTrack = false;
		this.emptySpaceItem6.Location = new System.Drawing.Point(0, 97);
		this.emptySpaceItem6.Name = "emptySpaceItem6";
		this.emptySpaceItem6.Size = new System.Drawing.Size(422, 47);
		this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 0);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 18);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 18);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(422, 18);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.Transparent;
		base.Controls.Add(this.nonCustomizableLayoutControl);
		base.Margin = new System.Windows.Forms.Padding(1);
		base.Name = "ImportSelectionUserControl";
		base.Size = new System.Drawing.Size(647, 536);
		((System.ComponentModel.ISupportInitialize)this.nonCustomizableLayoutControl).EndInit();
		this.nonCustomizableLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.searchTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.LabelURL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.folderTreeList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.searchEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dbmsGridLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.noItemsLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.suggestDataSourceLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		base.ResumeLayout(false);
	}
}
