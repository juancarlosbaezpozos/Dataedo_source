using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls.Base;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;

namespace Dataedo.App.UserControls.PanelControls;

public class GridPanelUserControl : BaseUserControl
{
	public delegate void DeleteItem();

	private readonly Dictionary<int, ColumnDefaultValues> defaultValues;

	private bool isColumnsForm;

	private IEnumerable<GridColumn> gridColumnsToOmitForBestFit;

	private readonly string[] columnNamesLockedByDefault = new string[14]
	{
		"iconTableColumnsGridColumn", "keyTableColumnsGridColumn", "ordinalPositionTableColumnsGridColumn", "nameTableColumnsGridColumn", "iconTableGridColumn", "typeTableGridColumn", "schemaTableGridColumn", "nameTableGridColumn", "iconProcedureParametersGridColumn", "ordinalPositionProcedureParametersGridColumn",
		"nameProcedureParametersGridColumn", "iconGridColumn", "titleTableTermsGridColumn", "typeTableGridColumn"
	};

	private DeleteItem delete;

	private Action customFields;

	private Action beforeEndUpdateSettingDefualtView;

	private string gridNameToExport;

	private IContainer components;

	protected BarManager gridPanelBarManager;

	protected Bar gridPanelBar;

	protected BarButtonItem columnChooserBarButtonItem;

	protected BarButtonItem defaultViewBarButtonItem;

	protected BarButtonItem customFieldsBarButtonItem;

	protected BarButtonItem clearBarButtonItem;

	protected BarDockControl barDockControlTop;

	protected BarDockControl barDockControlBottom;

	protected BarDockControl barDockControlLeft;

	protected BarDockControl barDockControlRight;

	private BarCheckItem filterBarCheckItem;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private RepositoryItemCheckEdit repositoryItemCheckEdit2;

	private RepositoryItemCheckEdit repositoryItemCheckEdit3;

	private RepositoryItemButtonEdit repositoryItemButtonEdit1;

	private RepositoryItemButtonEdit repositoryItemButtonEdit2;

	protected BarButtonItem removeBarButtonItem;

	private ButtonEdit filterButtonEdit;

	private BarButtonItem defaultLockBarButtonItem;

	private BarButtonItem unlockAllBarButtonItem;

	protected BarButtonItem exportToExcelBarButtonItem;

	private SaveFileDialog excelExportSaveFileDialog;

	[Browsable(true)]
	public GridView GridView { get; set; }

	public event DeleteItem Delete
	{
		add
		{
			if (delete == null || !delete.GetInvocationList().Contains(value))
			{
				delete = (DeleteItem)Delegate.Combine(delete, value);
			}
		}
		remove
		{
			delete = (DeleteItem)Delegate.Remove(delete, value);
		}
	}

	public event Action CustomFields
	{
		add
		{
			if (customFields == null || !customFields.GetInvocationList().Contains(value))
			{
				customFields = (Action)Delegate.Combine(customFields, value);
			}
		}
		remove
		{
			customFields = (Action)Delegate.Remove(customFields, value);
		}
	}

	public event Action BeforeEndUpdateSettingDefualtView
	{
		add
		{
			if (beforeEndUpdateSettingDefualtView == null || !beforeEndUpdateSettingDefualtView.GetInvocationList().Contains(value))
			{
				beforeEndUpdateSettingDefualtView = (Action)Delegate.Combine(beforeEndUpdateSettingDefualtView, value);
			}
		}
		remove
		{
			beforeEndUpdateSettingDefualtView = (Action)Delegate.Remove(beforeEndUpdateSettingDefualtView, value);
		}
	}

	private void OnDelete()
	{
		delete?.Invoke();
	}

	private void OnCustomFields()
	{
		customFields?.Invoke();
	}

	private void OnBeforeEndUpdateSettingDefualtView()
	{
		beforeEndUpdateSettingDefualtView?.Invoke();
	}

	public GridPanelUserControl()
	{
		InitializeComponent();
		defaultValues = new Dictionary<int, ColumnDefaultValues>();
		excelExportSaveFileDialog.InitialDirectory = LastConnectionInfo.LOGIN_INFO?.FileRepositoryPath;
	}

	public void SetButtonVisibility(BarButtonItem button, bool value)
	{
		int num = gridPanelBarManager.Items.IndexOf(button);
		if (num != -1)
		{
			gridPanelBarManager.Items[num].Visibility = ((!value) ? BarItemVisibility.Never : BarItemVisibility.Always);
		}
	}

	public void SetRemoveButtonVisibility(bool value)
	{
		removeBarButtonItem.Visibility = ((!value) ? BarItemVisibility.Never : BarItemVisibility.Always);
	}

	public void SetDefaultLockButtonVisibility(bool value)
	{
		defaultLockBarButtonItem.Visibility = ((!value) ? BarItemVisibility.Never : BarItemVisibility.Always);
	}

	public void SetRemoveButton(string hint, Image glyph)
	{
		removeBarButtonItem.Hint = hint;
		removeBarButtonItem.Glyph = glyph;
	}

	public void SetDefineCustomFieldsButtonVisibility(bool value)
	{
		customFieldsBarButtonItem.Visibility = ((!value) ? BarItemVisibility.Never : BarItemVisibility.Always);
	}

	public void HideButtons()
	{
		BarItemVisibility barItemVisibility3 = (removeBarButtonItem.Visibility = (customFieldsBarButtonItem.Visibility = BarItemVisibility.Never));
	}

	public void Initialize(string gridNameToExport, bool isSummaryControlOnModule = false, bool isColumnsForm = false, IEnumerable<GridColumn> gridColumnsToOmitForBestFit = null)
	{
		this.gridNameToExport = gridNameToExport;
		defaultValues.Clear();
		this.isColumnsForm = isColumnsForm;
		this.gridColumnsToOmitForBestFit = gridColumnsToOmitForBestFit;
		foreach (GridColumn column in GridView.Columns)
		{
			if (!defaultValues.ContainsKey(column.AbsoluteIndex))
			{
				ColumnDefaultValues columnDefaultValues = new ColumnDefaultValues(column.VisibleIndex, column.Width);
				if (column is BandedGridColumn)
				{
					columnDefaultValues.Band = (column as BandedGridColumn).OwnerBand;
				}
				defaultValues.Add(column.AbsoluteIndex, columnDefaultValues);
			}
			column.ClearFilter();
		}
		GridView.ClearSelection();
		GridOptionsView optionsView = GridView.OptionsView;
		bool showAutoFilterRow = (filterBarCheckItem.Checked = false);
		optionsView.ShowAutoFilterRow = showAutoFilterRow;
		string text2 = (GridView.FindFilterText = (filterButtonEdit.Text = string.Empty));
		if (Functionalities.HasFunctionality(FunctionalityEnum.Functionality.CustomFields))
		{
			customFieldsBarButtonItem.Enabled = true;
			customFieldsBarButtonItem.Hint = "Define custom fields";
		}
		else
		{
			customFieldsBarButtonItem.Enabled = false;
			customFieldsBarButtonItem.SuperTip = Functionalities.GetUnavailableActionToolTip(FunctionalityEnum.Functionality.CustomFields);
		}
		removeBarButtonItem.Hint = (isSummaryControlOnModule ? "Remove from Subject Area" : "Remove from repository");
	}

	public void RemoveCustomFieldsButton()
	{
		customFieldsBarButtonItem.Visibility = BarItemVisibility.Never;
	}

	public void InsertAdditionalButtonBeforeRemoveButton(BarButtonItem barButtonItem)
	{
		int position = gridPanelBarManager.Items.IndexOf(removeBarButtonItem);
		InsertAdditionalButton(barButtonItem, position);
	}

	public void InsertAdditionalButton(BarButtonItem barButtonItem, int position)
	{
		if (position >= 0)
		{
			int index = ((position > gridPanelBarManager.Items.Count()) ? (position + 1) : position);
			gridPanelBarManager.Items.Insert(index, barButtonItem);
			gridPanelBar.LinksPersistInfo.Insert(index, new LinkPersistInfo(barButtonItem));
		}
		else
		{
			gridPanelBarManager.Items.Add(barButtonItem);
			gridPanelBar.LinksPersistInfo.Add(new LinkPersistInfo(barButtonItem));
		}
	}

	public void AddAdditionalButton(BarButtonItem barButtonItem)
	{
		gridPanelBarManager.Items.Add(barButtonItem);
		gridPanelBar.LinksPersistInfo.Add(new LinkPersistInfo(barButtonItem));
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Return)
		{
			GridView.FindFilterText = filterButtonEdit.Text;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void FilterButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		GridView.CloseEditor();
		string text2 = (GridView.FindFilterText = (filterButtonEdit.Text = string.Empty));
	}

	private void ColumnChooserBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		GridView.ColumnsCustomization();
	}

	private void FilterBarCheckItem_CheckedChanged(object sender, ItemClickEventArgs e)
	{
		GridView.CloseEditor();
		GridView.OptionsView.ShowAutoFilterRow = !GridView.OptionsView.ShowAutoFilterRow;
	}

	private void DefaultViewBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		GridView.BeginUpdate();
		GridOptionsView optionsView = GridView.OptionsView;
		bool showAutoFilterRow = (filterBarCheckItem.Checked = false);
		optionsView.ShowAutoFilterRow = showAutoFilterRow;
		string text2 = (GridView.FindFilterText = (filterButtonEdit.Text = string.Empty));
		foreach (GridColumn column in GridView.Columns)
		{
			column.ClearFilter();
			column.Fixed = (columnNamesLockedByDefault.Contains(column.Name) ? FixedStyle.Left : FixedStyle.None);
			column.VisibleIndex = defaultValues[column.AbsoluteIndex].Position;
			column.Width = defaultValues[column.AbsoluteIndex].Width;
			column.SortOrder = ColumnSortOrder.None;
			if (column is BandedGridColumn)
			{
				(column as BandedGridColumn).OwnerBand = defaultValues[column.AbsoluteIndex].Band;
			}
		}
		if (!isColumnsForm)
		{
			CommonFunctionsPanels.SetBestFitForColumns(GridView, gridColumnsToOmitForBestFit);
		}
		OnBeforeEndUpdateSettingDefualtView();
		GridView.EndUpdate();
	}

	private void CustomFieldsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		OnCustomFields();
	}

	private void ClearBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		GridView.CloseEditor();
		GridView.BeginUpdate();
		GridView.ClearColumnsFilter();
		string text2 = (GridView.FindFilterText = (filterButtonEdit.Text = string.Empty));
		filterBarCheckItem.Checked = false;
		GridView.OptionsView.ShowAutoFilterRow = false;
		GridView.EndUpdate();
	}

	private void RemoveBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		OnDelete();
	}

	private void FilterButtonEdit_TextChanged(object sender, EventArgs e)
	{
		filterButtonEdit.Properties.Buttons[0].Image = (string.IsNullOrEmpty(filterButtonEdit.Text) ? Resources.search_16 : Resources.delete_gray_16);
		GridView.FindFilterText = filterButtonEdit.Text;
	}

	private void DefaultLockButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		foreach (GridColumn column in GridView.Columns)
		{
			if (columnNamesLockedByDefault.Contains(column.Name))
			{
				column.Fixed = FixedStyle.Left;
			}
		}
	}

	private void UnlockAllButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		List<GridColumn> list = new List<GridColumn>();
		foreach (GridColumn column in GridView.Columns)
		{
			if (column.Fixed == FixedStyle.Left)
			{
				list.Add(column);
			}
		}
		list = list.OrderByDescending((GridColumn c) => c.VisibleIndex).ToList();
		foreach (GridColumn item in list)
		{
			item.Fixed = FixedStyle.None;
		}
	}

	private void ExportToExcelBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		string fileName = Paths.RemoveInvalidFilePathCharacters($"Exported {gridNameToExport} {DateTime.Now:yyyy-MM-dd HH_mm_ss}", "_");
		excelExportSaveFileDialog.FileName = fileName;
		if (excelExportSaveFileDialog.ShowDialog(this) == DialogResult.OK)
		{
			string fileName2 = excelExportSaveFileDialog.FileName;
			string extension = Path.GetExtension(fileName2);
			if (ExportToExcel(fileName2, extension))
			{
				OpenSavedFile(fileName2);
			}
		}
	}

	private bool ExportToExcel(string path, string ext)
	{
		try
		{
			if (!(ext == ".xls"))
			{
				if (!(ext == ".xlsx"))
				{
					return false;
				}
				GridView.ExportToXlsx(path);
			}
			else
			{
				GridView.ExportToXls(path);
			}
			return true;
		}
		catch (IOException ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to save file " + path, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, FindForm());
			return false;
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Error while exporting grid", FindForm());
			return false;
		}
	}

	private void OpenSavedFile(string path)
	{
		try
		{
			Paths.OpenSavedFile(path, FindForm());
		}
		catch (IOException ex)
		{
			GeneralMessageBoxesHandling.Show("Unable to open file " + path, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand, ex.Message, 1, FindForm());
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception, "Unable to open file " + path, FindForm());
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.UserControls.PanelControls.GridPanelUserControl));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.gridPanelBarManager = new DevExpress.XtraBars.BarManager(this.components);
		this.gridPanelBar = new DevExpress.XtraBars.Bar();
		this.defaultViewBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.defaultLockBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.unlockAllBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.columnChooserBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.customFieldsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.filterBarCheckItem = new DevExpress.XtraBars.BarCheckItem();
		this.clearBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.exportToExcelBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.repositoryItemCheckEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.repositoryItemCheckEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemButtonEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.filterButtonEdit = new DevExpress.XtraEditors.ButtonEdit();
		this.excelExportSaveFileDialog = new System.Windows.Forms.SaveFileDialog();
		((System.ComponentModel.ISupportInitialize)this.gridPanelBarManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.filterButtonEdit.Properties).BeginInit();
		base.SuspendLayout();
		this.gridPanelBarManager.AllowCustomization = false;
		this.gridPanelBarManager.AllowQuickCustomization = false;
		this.gridPanelBarManager.AllowShowToolbarsPopup = false;
		this.gridPanelBarManager.Bars.AddRange(new DevExpress.XtraBars.Bar[1] { this.gridPanelBar });
		this.gridPanelBarManager.DockControls.Add(this.barDockControlTop);
		this.gridPanelBarManager.DockControls.Add(this.barDockControlBottom);
		this.gridPanelBarManager.DockControls.Add(this.barDockControlLeft);
		this.gridPanelBarManager.DockControls.Add(this.barDockControlRight);
		this.gridPanelBarManager.Form = this;
		this.gridPanelBarManager.Items.AddRange(new DevExpress.XtraBars.BarItem[9] { this.columnChooserBarButtonItem, this.defaultViewBarButtonItem, this.customFieldsBarButtonItem, this.clearBarButtonItem, this.filterBarCheckItem, this.removeBarButtonItem, this.defaultLockBarButtonItem, this.unlockAllBarButtonItem, this.exportToExcelBarButtonItem });
		this.gridPanelBarManager.MaxItemId = 22;
		this.gridPanelBarManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.repositoryItemCheckEdit1, this.repositoryItemCheckEdit2, this.repositoryItemCheckEdit3, this.repositoryItemButtonEdit1, this.repositoryItemButtonEdit2 });
		this.gridPanelBar.BarName = "Tools";
		this.gridPanelBar.DockCol = 0;
		this.gridPanelBar.DockRow = 0;
		this.gridPanelBar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
		this.gridPanelBar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[9]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.defaultViewBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.defaultLockBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.unlockAllBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.columnChooserBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.customFieldsBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.filterBarCheckItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.clearBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.removeBarButtonItem),
			new DevExpress.XtraBars.LinkPersistInfo(this.exportToExcelBarButtonItem)
		});
		this.gridPanelBar.OptionsBar.AllowQuickCustomization = false;
		this.gridPanelBar.OptionsBar.DisableCustomization = true;
		this.gridPanelBar.OptionsBar.DrawBorder = false;
		this.gridPanelBar.OptionsBar.DrawDragBorder = false;
		this.gridPanelBar.Text = "Tools";
		this.defaultViewBarButtonItem.Hint = "Default view";
		this.defaultViewBarButtonItem.Id = 2;
		this.defaultViewBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.filter_default_view_16;
		this.defaultViewBarButtonItem.Name = "defaultViewBarButtonItem";
		this.defaultViewBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(DefaultViewBarButtonItem_ItemClick);
		this.defaultLockBarButtonItem.Hint = "Lock default columns";
		this.defaultLockBarButtonItem.Id = 19;
		this.defaultLockBarButtonItem.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("defaultLockBarButtonItem.ImageOptions.Image");
		this.defaultLockBarButtonItem.Name = "defaultLockBarButtonItem";
		this.defaultLockBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
		this.defaultLockBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(DefaultLockButtonItem_ItemClick);
		this.unlockAllBarButtonItem.Hint = "Unlock all columns";
		this.unlockAllBarButtonItem.Id = 20;
		this.unlockAllBarButtonItem.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("unlockAllBarButtonItem.ImageOptions.Image");
		this.unlockAllBarButtonItem.Name = "unlockAllBarButtonItem";
		this.unlockAllBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(UnlockAllButtonItem_ItemClick);
		this.columnChooserBarButtonItem.Hint = "Show columns";
		this.columnChooserBarButtonItem.Id = 0;
		this.columnChooserBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.column_chooser_16;
		this.columnChooserBarButtonItem.Name = "columnChooserBarButtonItem";
		this.columnChooserBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ColumnChooserBarButtonItem_ItemClick);
		this.customFieldsBarButtonItem.Hint = "Define custom fields";
		this.customFieldsBarButtonItem.Id = 3;
		this.customFieldsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.custom_fields_16;
		this.customFieldsBarButtonItem.Name = "customFieldsBarButtonItem";
		this.customFieldsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(CustomFieldsBarButtonItem_ItemClick);
		this.filterBarCheckItem.Hint = "Filter row";
		this.filterBarCheckItem.Id = 11;
		this.filterBarCheckItem.ImageOptions.Image = Dataedo.App.Properties.Resources.filter_auto_16;
		this.filterBarCheckItem.Name = "filterBarCheckItem";
		this.filterBarCheckItem.CheckedChanged += new DevExpress.XtraBars.ItemClickEventHandler(FilterBarCheckItem_CheckedChanged);
		this.clearBarButtonItem.Hint = "Clear filters";
		this.clearBarButtonItem.Id = 4;
		this.clearBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.filter_clear_16;
		this.clearBarButtonItem.Name = "clearBarButtonItem";
		this.clearBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ClearBarButtonItem_ItemClick);
		this.removeBarButtonItem.Hint = "Remove from repository";
		this.removeBarButtonItem.Id = 15;
		this.removeBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.delete_16;
		this.removeBarButtonItem.Name = "removeBarButtonItem";
		this.removeBarButtonItem.Visibility = DevExpress.XtraBars.BarItemVisibility.Never;
		this.removeBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(RemoveBarButtonItem_ItemClick);
		this.exportToExcelBarButtonItem.Hint = "Export to Excel";
		this.exportToExcelBarButtonItem.Id = 21;
		this.exportToExcelBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.excel_color_16;
		this.exportToExcelBarButtonItem.Name = "exportToExcelBarButtonItem";
		this.exportToExcelBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(ExportToExcelBarButtonItem_ItemClick);
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.gridPanelBarManager;
		this.barDockControlTop.Size = new System.Drawing.Size(962, 28);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 28);
		this.barDockControlBottom.Manager = this.gridPanelBarManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(962, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 28);
		this.barDockControlLeft.Manager = this.gridPanelBarManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 0);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(962, 28);
		this.barDockControlRight.Manager = this.gridPanelBarManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 0);
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit2.AutoHeight = false;
		this.repositoryItemCheckEdit2.Name = "repositoryItemCheckEdit2";
		this.repositoryItemCheckEdit3.AutoHeight = false;
		this.repositoryItemCheckEdit3.Name = "repositoryItemCheckEdit3";
		this.repositoryItemButtonEdit1.AutoHeight = false;
		this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
		this.repositoryItemButtonEdit2.AutoHeight = false;
		this.repositoryItemButtonEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.repositoryItemButtonEdit2.Name = "repositoryItemButtonEdit2";
		this.filterButtonEdit.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.filterButtonEdit.Location = new System.Drawing.Point(763, 5);
		this.filterButtonEdit.MaximumSize = new System.Drawing.Size(196, 20);
		this.filterButtonEdit.MinimumSize = new System.Drawing.Size(196, 20);
		this.filterButtonEdit.Name = "filterButtonEdit";
		this.filterButtonEdit.Properties.AllowFocused = false;
		editorButtonImageOptions.Image = Dataedo.App.Properties.Resources.search_16;
		this.filterButtonEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.filterButtonEdit.Properties.UseReadOnlyAppearance = false;
		this.filterButtonEdit.Size = new System.Drawing.Size(196, 20);
		this.filterButtonEdit.TabIndex = 13;
		this.filterButtonEdit.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(FilterButtonEdit_ButtonClick);
		this.filterButtonEdit.TextChanged += new System.EventHandler(FilterButtonEdit_TextChanged);
		this.excelExportSaveFileDialog.DefaultExt = "xlsx";
		this.excelExportSaveFileDialog.Filter = "Excel Workbook (*.xlsx)|*.xlsx|Excel 97-2003 Workbook (*.xls)|*.xls";
		this.excelExportSaveFileDialog.Title = "Export as";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.filterButtonEdit);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.Name = "GridPanelUserControl";
		base.Size = new System.Drawing.Size(962, 28);
		((System.ComponentModel.ISupportInitialize)this.gridPanelBarManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.filterButtonEdit.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
