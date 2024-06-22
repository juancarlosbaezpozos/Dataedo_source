using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Helpers.Files;
using Dataedo.App.Import.DataLake;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.Import.DataLake.Processing;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.Tools.UI;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Shared.Enums;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraSplashScreen;

namespace Dataedo.App.Forms;

public class ImportChooseObjects : BaseXtraForm
{
	private Form parentForm;

	private int databaseId;

	private DataLakeTypeEnum.DataLakeType? dataLakeType;

	private List<TableByDatabaseIdObject> existingObjects;

	private string lastPath;

	public SharedObjectTypeEnum.ObjectType ObjectType;

	private readonly CustomFieldsSupport customFieldsSupport;

	private IContainer components;

	private NonCustomizableLayoutControl mainLayoutControl;

	private LayoutControlGroup mainLayoutControlGroup;

	private SimpleButton importSimpleButton;

	private LayoutControlItem importSimpleButtonLayoutControlItem;

	private SimpleButton cancelSimpleButton;

	private LayoutControlItem cancelSimpleButtonLayoutControlItem;

	private EmptySpaceItem buttonsSeparator2EmptySpaceItem;

	private EmptySpaceItem buttonsSeparator1EmptySpaceItem;

	private EmptySpaceItem buttonsSeparator3EmptySpaceItem;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private ToolTipController toolTipController;

	private NonCustomizableLayoutControl objectsLayoutControl;

	private LayoutControlGroup objectsLayoutControlGroup;

	private NonCustomizableLayoutControl fieldsMainLayoutControl;

	private GridControl fieldsGridControl;

	private GridView fieldsGridView;

	private GridColumn fieldIconGridColumn;

	private RepositoryItemPictureEdit fieldIconRepositoryItemPictureEdit;

	private GridColumn fieldTypeGridColumn;

	private GridColumn fieldNameGridColumn;

	private LayoutControlGroup fieldsLayoutControlGroup;

	private LayoutControlItem layoutControlItem2;

	private GridControl availableObjectsGridControl;

	private GridView availableObjectsGridView;

	private GridColumn isSelectedGridColumn;

	private RepositoryItemCheckEdit isSelectRepositoryItemCheckEdit;

	private GridColumn iconGridColumn;

	private RepositoryItemPictureEdit iconRepositoryItemPictureEdit;

	private GridColumn typeGridColumn;

	private GridColumn nameGridColumn;

	private GridColumn locationGridColumn;

	private LayoutControlItem layoutControlItem1;

	private SimpleLabelItem availableSimpleLabelItem;

	private SimpleLabelItem fieldsSimpleLabelItem;

	private SplitContainerControl splitContainerControl;

	private LayoutControlItem splitContainerControlLayoutControlItem;

	private GridColumn fieldDataTypeGridColumn;

	private GridColumn fieldSizeGridColumn;

	private GridColumn fieldNullableGridColumn;

	private RepositoryItemCustomTextEdit columnFullNameRepositoryItemCustomTextEdit;

	private GridColumn fieldOrdinalPositonGridColumn;

	private LayoutControlItem addFilesSimpleButtonLayoutControlItem;

	private OpenFileDialog openFileDialog;

	private SplashScreenManager splashScreenManager;

	private SimpleButton addFilesSimpleButton;

	private RepositoryItemGridLookUpEdit typeRepositoryItemGridLookUpEdit;

	private GridView repositoryItemGridLookUpEditView;

	private GridColumn displayNameGridColumn;

	public List<ObjectModel> ObjectModels { get; private set; }

	public ImportChooseObjects(Form parentForm, int databaseId, DataLakeTypeEnum.DataLakeType? dataLakeType, SharedObjectTypeEnum.ObjectType objectType, IEnumerable<ObjectModel> objectModels, CustomFieldsSupport customFieldsSupport)
	{
		this.parentForm = parentForm;
		InitializeComponent();
		this.databaseId = databaseId;
		this.dataLakeType = dataLakeType;
		ObjectType = objectType;
		this.customFieldsSupport = customFieldsSupport;
		Initialize();
		fieldTypeGridColumn.Visible = (fieldTypeGridColumn.OptionsColumn.ShowInCustomizationForm = false);
		ObjectModels = objectModels.ToList();
		ValidateModels(ObjectModels, checkCurrentList: true);
		ObjectModels = ObjectModels.OrderBy((ObjectModel x) => x.Name).ToList();
		availableObjectsGridControl.BeginUpdate();
		availableObjectsGridControl.DataSource = ObjectModels;
		availableObjectsGridControl.EndUpdate();
		availableObjectsGridControl.ForceInitialize();
		availableObjectsGridView.BestFitColumns();
		nameGridColumn.Width += 60;
		CheckSelectedRows();
		lastPath = ObjectModels.FirstOrDefault()?.FilePath;
		typeRepositoryItemGridLookUpEdit.DataSource = DataLakeTypeEnum.GetDataLakeTypeObjects();
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.S | Keys.Control:
			if (CheckSelectedRows())
			{
				base.DialogResult = DialogResult.OK;
				Close();
			}
			break;
		case Keys.Escape:
			Close();
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void Initialize()
	{
		List<ToolTipData> toolTipDataList = new List<ToolTipData>
		{
			new ToolTipData(availableObjectsGridControl, ObjectType, iconGridColumn.VisibleIndex),
			new ToolTipData(fieldsGridControl, ObjectType, fieldIconGridColumn.VisibleIndex)
		};
		CommonFunctionsPanels.AddEventsForToolTips(toolTipController, toolTipDataList);
	}

	private void LoadExistingObjects()
	{
		existingObjects = DB.Table.GetTablesByDatabase(databaseId, ObjectType);
	}

	private void ValidateModels(IEnumerable<ObjectModel> objects, bool checkCurrentList)
	{
		LoadExistingObjects();
		for (int i = 0; i < objects.Count(); i++)
		{
			ObjectModel objectModel = objects.ElementAt(i);
			bool flag2 = (objectModel.OriginalObjectExists = CheckObjectExists(existingObjects, objectModel.Name, checkCurrentList));
			objectModel.ObjectExists = flag2;
			objectModel.IsSelected = !flag2;
			if (!flag2)
			{
				continue;
			}
			int num = 1;
			while (CheckObjectExists(existingObjects, objectModel.Name, checkCurrentList) || CheckObjectExists(objects.Except(new ObjectModel[1] { objectModel }), objectModel.Name, checkCurrentList: false))
			{
				objectModel.Name = $"{objectModel.OriginalName} ({++num})";
				if (num == 0)
				{
					objectModel.Name = objectModel.OriginalName;
					return;
				}
			}
			objectModel.ObjectExists = false;
			objectModel.IsSelected = true;
			objectModel.SetNameAsCorrectedName();
		}
	}

	private new bool Validate()
	{
		bool result = true;
		for (int i = 0; i < availableObjectsGridView.DataRowCount; i++)
		{
			ObjectModel objectModel = availableObjectsGridView.GetRow(i) as ObjectModel;
			if (objectModel.IsSelected)
			{
				objectModel.ObjectExists = CheckObjectExists(existingObjects, objectModel.Name, checkCurrentList: true);
				if (objectModel.ObjectExists)
				{
					result = false;
				}
			}
		}
		return result;
	}

	private bool CheckObjectExists(IEnumerable<TableByDatabaseIdObject> objects, string name, bool checkCurrentList)
	{
		if (!objects.Any((TableByDatabaseIdObject x) => ColumnsHelper.GetFullTableName(x.Schema, x.Name).Equals(ColumnsHelper.GetFullTableName(null, name))) && (!checkCurrentList || ObjectModels.Where((ObjectModel x) => x.IsSelected && x.Name == name).Count() <= 1))
		{
			if (!checkCurrentList)
			{
				return ObjectModels.Any((ObjectModel x) => x.Name == name);
			}
			return false;
		}
		return true;
	}

	private bool CheckObjectExists(IEnumerable<ObjectModel> objects, string name, bool checkCurrentList)
	{
		checkCurrentList = true;
		if (!objects.Any((ObjectModel x) => ColumnsHelper.GetFullTableName(null, x.Name).Equals(ColumnsHelper.GetFullTableName(null, name))) && (!checkCurrentList || ObjectModels.Where((ObjectModel x) => x.IsSelected && x.Name == name).Count() <= 1))
		{
			if (!checkCurrentList)
			{
				return ObjectModels.Any((ObjectModel x) => x.IsSelected && x.Name == name);
			}
			return false;
		}
		return true;
	}

	private void Save()
	{
		LoadExistingObjects();
		if (!Validate())
		{
			importSimpleButton.Enabled = false;
		}
		else
		{
			base.DialogResult = DialogResult.OK;
		}
	}

	private bool CheckSelectedRows()
	{
		bool flag = ObjectModels.Any((ObjectModel x) => x.IsSelected) && ObjectModels.Where((ObjectModel x) => x.IsSelected).All((ObjectModel x) => x.IsValidToAdd);
		importSimpleButton.Enabled = flag;
		return flag;
	}

	private void AvailableObjectsGridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		if (e.Column.Equals(iconGridColumn))
		{
			ObjectModel objectModel = e.Row as ObjectModel;
			e.Value = IconsSupport.GetObjectIcon(objectModel.ObjectType, objectModel.ObjectSubtype, objectModel.Source);
		}
	}

	private void AvailableObjectsGridView_MouseDown(object sender, MouseEventArgs e)
	{
		Grids.ShowEditiorOnClick(sender, e, "IsSelected");
	}

	private void CancelSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void AvailableObjectsGridView_CustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
	{
		if (!(sender is GridView gridView))
		{
			return;
		}
		ObjectModel objectModel = gridView.GetRow(e.RowHandle) as ObjectModel;
		if (e.Column == nameGridColumn)
		{
			if (objectModel != null && !objectModel.IsInitializedSuccessfully)
			{
				BaseEditViewInfo viewInfo = ((GridCellInfo)e.Cell).ViewInfo;
				viewInfo.ErrorIcon = DXErrorProvider.GetErrorIconInternal(ErrorType.Critical);
				if (!objectModel.DataLakeType.HasValue)
				{
					viewInfo.ErrorIconText = "Object \"" + objectModel.OriginalName + "\" format is not set.";
				}
				else
				{
					viewInfo.ErrorIconText = "Object \"" + objectModel.OriginalName + "\" is not valid " + DataLakeTypeEnum.GetDisplayName(objectModel.DataLakeType.Value) + ".";
				}
				if (!string.IsNullOrWhiteSpace(objectModel.InitializationDetails))
				{
					viewInfo.ErrorIconText = viewInfo.ErrorIconText + Environment.NewLine + Environment.NewLine + objectModel.InitializationDetails;
				}
				viewInfo.ShowErrorIcon = true;
				viewInfo.CalcViewInfo(e.Cache.Graphics);
				e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
				e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridDisabledGridRowForeColor;
			}
			else if (objectModel != null && objectModel.ObjectExists && objectModel.IsSelected)
			{
				BaseEditViewInfo viewInfo2 = ((GridCellInfo)e.Cell).ViewInfo;
				viewInfo2.ErrorIcon = DXErrorProvider.GetErrorIconInternal(ErrorType.Critical);
				viewInfo2.ErrorIconText = "Object with this name already exists.";
				viewInfo2.ShowErrorIcon = true;
				viewInfo2.CalcViewInfo(e.Cache.Graphics);
			}
			else if (string.IsNullOrEmpty(objectModel.Name) && objectModel.IsSelected)
			{
				BaseEditViewInfo viewInfo3 = ((GridCellInfo)e.Cell).ViewInfo;
				viewInfo3.ErrorIcon = DXErrorProvider.GetErrorIconInternal(ErrorType.Critical);
				viewInfo3.ErrorIconText = "Name can not be empty.";
				viewInfo3.ShowErrorIcon = true;
				viewInfo3.CalcViewInfo(e.Cache.Graphics);
			}
			else if (objectModel != null && objectModel.OriginalObjectExists && objectModel != null && objectModel.IsCorrectedNameSet)
			{
				BaseEditViewInfo viewInfo4 = ((GridCellInfo)e.Cell).ViewInfo;
				viewInfo4.ErrorIcon = DXErrorProvider.GetErrorIconInternal(ErrorType.Warning);
				viewInfo4.ErrorIconText = "Object \"" + objectModel.OriginalName + "\" already exists." + Environment.NewLine + "The name has been changed to valid name.";
				viewInfo4.ShowErrorIcon = true;
				viewInfo4.CalcViewInfo(e.Cache.Graphics);
			}
		}
		else if (objectModel != null && !objectModel.IsInitializedSuccessfully && e.Column.FieldName != "DataLakeType")
		{
			e.Appearance.BackColor = SkinsManager.CurrentSkin.GridDisabledGridRowBackColor;
			e.Appearance.ForeColor = SkinsManager.CurrentSkin.GridDisabledGridRowForeColor;
		}
	}

	private void IsSelectRepositoryItemCheckEdit_CheckedChanged(object sender, EventArgs e)
	{
		availableObjectsGridView.PostEditor();
	}

	private void AvailableObjectsGridView_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		Validate();
		CheckSelectedRows();
	}

	private void AvailableObjectsGridView_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		if (sender is GridView gridView && gridView.FocusedRowHandle != -2147483646 && e.Column == nameGridColumn)
		{
			ObjectModel objectModel = availableObjectsGridView.GetRow(e.RowHandle) as ObjectModel;
			objectModel.Name = e.Value.ToString();
			Validate();
			CheckSelectedRows();
			SetFieldsLabel(objectModel);
		}
	}

	private void TypeRepositoryItemGridLookUpEdit_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			GridView gridView = ((sender as GridLookUpEdit).Parent as GridControl).DefaultView as GridView;
			_ = e.OldValue;
			DataLakeTypeEnum.DataLakeType? dataLakeType = e.NewValue as DataLakeTypeEnum.DataLakeType?;
			ObjectModel objectModel = gridView.GetFocusedRow() as ObjectModel;
			List<ObjectModel> list = new List<ObjectModel>();
			try
			{
				IDataLakeImport dataLakeImport = DataLakeImportFactory.GetDataLakeImport(ObjectType, dataLakeType.Value);
				list = ((objectModel.ImportItem.IsStream && dataLakeImport is AvroImport avroImport) ? avroImport.GetObjectsFromAvroOrAvscStream(objectModel.ImportItem.CreateStream(), objectModel.ImportItem.Name).ToList() : ((!objectModel.ImportItem.IsStream || !(dataLakeImport is IStreamableDataLakeImport streamableDataLakeImport)) ? dataLakeImport.GetObjectsFromFile(objectModel.ImportItem.GetFile().FullName).ToList() : streamableDataLakeImport.GetObjectsFromStream(objectModel.ImportItem.CreateStream()).ToList()));
				foreach (ObjectModel item in list)
				{
					objectModel.ImportItem?.CorrectObjectModelAfterImport(item);
				}
			}
			catch (Exception exception)
			{
				ObjectModel invalidObjectModel = FilesImporter.GetInvalidObjectModel(objectModel.ImportItem, dataLakeType, ObjectType, exception);
				objectModel.ImportItem?.CorrectObjectModelAfterImport(invalidObjectModel);
				list.Add(invalidObjectModel);
			}
			gridView.BeginDataUpdate();
			int num = ObjectModels.IndexOf(objectModel);
			ObjectModel objectModel2 = list?.FirstOrDefault();
			if (num >= 0)
			{
				ObjectModel objectModel3 = ObjectModels[num];
				objectModel3.Fields = objectModel2.Fields;
				if (objectModel2 != null)
				{
					objectModel3.ApplyData(objectModel2);
					objectModel3.ImportItem.CorrectObjectModelAfterImport(objectModel3);
				}
				SetObjectFields(ObjectModels[num]);
			}
			ValidateModels(ObjectModels, checkCurrentList: true);
			gridView.PostEditor();
			gridView.EndDataUpdate();
			CheckSelectedRows();
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
	}

	private void AvailableObjectsGridView_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		ObjectModel objectFields = availableObjectsGridView.GetRow(e.FocusedRowHandle) as ObjectModel;
		SetObjectFields(objectFields);
	}

	private void SetObjectFields(ObjectModel data)
	{
		SetFieldsLabel(data);
		fieldsGridControl.DataSource = data.Fields;
		fieldsGridControl.ForceInitialize();
		fieldsGridView.BestFitColumns();
	}

	private void SetFieldsLabel(ObjectModel data = null)
	{
		data = data ?? (availableObjectsGridView.GetRow(availableObjectsGridView.FocusedRowHandle) as ObjectModel);
		fieldsSimpleLabelItem.Text = ((data == null) ? "Fields" : (string.IsNullOrEmpty(data.Name) ? (data.SubtypeDisplayText + " fields") : (data.SubtypeDisplayText + " " + data.Name + " fields")));
	}

	private void FieldsGridView_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		if (e.Column.Equals(fieldIconGridColumn))
		{
			FieldModel fieldModel = e.Row as FieldModel;
			e.Value = IconsSupport.GetObjectIcon(fieldModel.ObjectType, fieldModel.ObjectSubtype, fieldModel.Source);
		}
	}

	private void AvailableObjectsGridView_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (sender is GridView gridView)
		{
			ObjectModel obj = gridView.GetRow(gridView.FocusedRowHandle) as ObjectModel;
			if (obj != null && !obj.IsInitializedSuccessfully && gridView.FocusedColumn.FieldName != "DataLakeType")
			{
				e.Cancel = true;
			}
		}
	}

	private void AddFilesSimpleButton_Click(object sender, EventArgs e)
	{
		ImportFromFileForm importFromFileForm = new ImportFromFileForm(null, databaseId, getDataOnly: true, ObjectType, customFieldsSupport);
		importFromFileForm.ShowDialog();
		if (importFromFileForm.DialogResult == DialogResult.OK)
		{
			dataLakeType = importFromFileForm.DataLakeType;
			ValidateModels(importFromFileForm.ObjectModels, checkCurrentList: false);
			availableObjectsGridControl.BeginUpdate();
			ObjectModels.AddRange(importFromFileForm.ObjectModels);
			availableObjectsGridControl.EndUpdate();
			availableObjectsGridControl.ForceInitialize();
			CheckSelectedRows();
		}
	}

	private void AvailableObjectsGridControl_DragOver(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			e.Effect = DragDropEffects.Copy;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void AvailableObjectsGridControl_DragEnter(object sender, DragEventArgs e)
	{
		if (e.Data.GetDataPresent(DataFormats.FileDrop))
		{
			e.Effect = DragDropEffects.Copy;
		}
		else
		{
			e.Effect = DragDropEffects.None;
		}
	}

	private void AvailableObjectsGridControl_DragDrop(object sender, DragEventArgs e)
	{
		string[] array = null;
		try
		{
			array = (string[])e.Data.GetData(DataFormats.FileDrop);
		}
		catch
		{
		}
		if (array != null)
		{
			AddFiles(array);
		}
	}

	private bool AddFiles(string[] paths)
	{
		List<ObjectModel> list = null;
		try
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: true);
			list = new List<ObjectModel>();
			string[] array = paths;
			foreach (string text in array)
			{
				if (!File.Exists(text))
				{
					CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
					GeneralMessageBoxesHandling.Show("File \"" + text + "\" not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
					return false;
				}
			}
			array = paths;
			foreach (string path in array)
			{
				DataLakeTypeEnum.DataLakeType? dataLakeType = this.dataLakeType;
				try
				{
					dataLakeType = DataLakeTypeDeterminer.DetermineTypeByFileExtension(path, null, out var _);
					if (!dataLakeType.HasValue)
					{
						list.Add(FilesImporter.GetInvalidObjectModel(path, dataLakeType, ObjectType));
					}
					else
					{
						list.AddRange(DataLakeImportFactory.GetDataLakeImport(ObjectType, dataLakeType.Value).GetObjectsFromFile(path));
					}
				}
				catch (Exception)
				{
					list.Add(FilesImporter.GetInvalidObjectModel(path, dataLakeType, ObjectType));
				}
			}
			ValidateModels(list, checkCurrentList: false);
			availableObjectsGridControl.BeginUpdate();
			ObjectModels.AddRange(list);
			availableObjectsGridControl.EndUpdate();
			availableObjectsGridControl.ForceInitialize();
			CheckSelectedRows();
		}
		finally
		{
			CommonFunctionsDatabase.SetWaitFormVisibility(splashScreenManager, show: false);
		}
		return true;
	}

	private void AvailableObjectsGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
	}

	private void FieldsGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
	}

	private void ImportChooseObjects_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (base.DialogResult == DialogResult.OK)
		{
			Save();
		}
		else if (ObjectModels.Any((ObjectModel x) => x.IsValid))
		{
			DialogResult? dialogResult = GeneralMessageBoxesHandling.Show("Data has been changed, would you like to save these changes?", "Data has been changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
			if (dialogResult == DialogResult.Yes)
			{
				base.DialogResult = DialogResult.OK;
				Save();
			}
			else if (dialogResult != DialogResult.No)
			{
				base.DialogResult = DialogResult.Cancel;
				e.Cancel = true;
			}
		}
	}

	private void ImportChooseObjects_Shown(object sender, EventArgs e)
	{
		if (parentForm != null)
		{
			parentForm.Visible = false;
		}
	}

	private void TypeRepositoryItemGridLookUpEdit_BeforePopup(object sender, EventArgs e)
	{
		Grids.SetPopupHeight(sender as GridLookUpEdit, 10);
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
		new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.ImportChooseObjects));
		this.mainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.addFilesSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
		this.objectsLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.availableObjectsGridControl = new DevExpress.XtraGrid.GridControl();
		this.availableObjectsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.isSelectedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.isSelectRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.iconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.typeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.typeRepositoryItemGridLookUpEdit = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
		this.repositoryItemGridLookUpEditView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.displayNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.nameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.locationGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.objectsLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.availableSimpleLabelItem = new DevExpress.XtraLayout.SimpleLabelItem();
		this.fieldsMainLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.fieldsGridControl = new DevExpress.XtraGrid.GridControl();
		this.fieldsGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.fieldIconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.fieldIconRepositoryItemPictureEdit = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.fieldTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.fieldOrdinalPositonGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.fieldNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.columnFullNameRepositoryItemCustomTextEdit = new Dataedo.App.UserControls.RepositoryItemCustomTextEdit();
		this.fieldDataTypeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.fieldSizeGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.fieldNullableGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.fieldsLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.fieldsSimpleLabelItem = new DevExpress.XtraLayout.SimpleLabelItem();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.importSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.mainLayoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.buttonsSeparator3EmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.cancelSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.buttonsSeparator2EmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.importSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.buttonsSeparator1EmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.splitContainerControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.addFilesSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
		this.splashScreenManager = new DevExpress.XtraSplashScreen.SplashScreenManager(this, typeof(Dataedo.App.Forms.DefaultWaitForm), true, true);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).BeginInit();
		this.mainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl).BeginInit();
		this.splitContainerControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.objectsLayoutControl).BeginInit();
		this.objectsLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.availableObjectsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.availableObjectsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.isSelectRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeRepositoryItemGridLookUpEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEditView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.objectsLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.availableSimpleLabelItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsMainLayoutControl).BeginInit();
		this.fieldsMainLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.fieldsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fieldIconRepositoryItemPictureEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.columnFullNameRepositoryItemCustomTextEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsSimpleLabelItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsSeparator3EmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cancelSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsSeparator2EmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.importSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsSeparator1EmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.addFilesSimpleButtonLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.mainLayoutControl.AllowCustomization = false;
		this.mainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.mainLayoutControl.Controls.Add(this.addFilesSimpleButton);
		this.mainLayoutControl.Controls.Add(this.splitContainerControl);
		this.mainLayoutControl.Controls.Add(this.cancelSimpleButton);
		this.mainLayoutControl.Controls.Add(this.importSimpleButton);
		this.mainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.mainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.mainLayoutControl.MenuManager = this.barManager;
		this.mainLayoutControl.Name = "mainLayoutControl";
		this.mainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2928, 517, 832, 483);
		this.mainLayoutControl.Root = this.mainLayoutControlGroup;
		this.mainLayoutControl.Size = new System.Drawing.Size(1099, 546);
		this.mainLayoutControl.TabIndex = 0;
		this.mainLayoutControl.Text = "mainLayoutControl";
		this.mainLayoutControl.ToolTipController = this.toolTipController;
		this.addFilesSimpleButton.Location = new System.Drawing.Point(12, 486);
		this.addFilesSimpleButton.Name = "addFilesSimpleButton";
		this.addFilesSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.addFilesSimpleButton.StyleController = this.mainLayoutControl;
		this.addFilesSimpleButton.TabIndex = 2;
		this.addFilesSimpleButton.Text = "Add";
		this.addFilesSimpleButton.Click += new System.EventHandler(AddFilesSimpleButton_Click);
		this.splitContainerControl.Location = new System.Drawing.Point(12, 12);
		this.splitContainerControl.Name = "splitContainerControl";
		this.splitContainerControl.Panel1.Controls.Add(this.objectsLayoutControl);
		this.splitContainerControl.Panel1.Text = "Panel1";
		this.splitContainerControl.Panel2.Controls.Add(this.fieldsMainLayoutControl);
		this.splitContainerControl.Panel2.Text = "Panel2";
		this.splitContainerControl.ShowSplitGlyph = DevExpress.Utils.DefaultBoolean.True;
		this.splitContainerControl.Size = new System.Drawing.Size(1075, 470);
		this.splitContainerControl.SplitterPosition = 512;
		this.splitContainerControl.TabIndex = 18;
		this.objectsLayoutControl.AllowCustomization = false;
		this.objectsLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.objectsLayoutControl.Controls.Add(this.availableObjectsGridControl);
		this.objectsLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.objectsLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.objectsLayoutControl.Name = "objectsLayoutControl";
		this.objectsLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(106, 382, 650, 400);
		this.objectsLayoutControl.Root = this.objectsLayoutControlGroup;
		this.objectsLayoutControl.Size = new System.Drawing.Size(512, 470);
		this.objectsLayoutControl.TabIndex = 17;
		this.objectsLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.availableObjectsGridControl.AllowDrop = true;
		this.availableObjectsGridControl.Location = new System.Drawing.Point(2, 29);
		this.availableObjectsGridControl.MainView = this.availableObjectsGridView;
		this.availableObjectsGridControl.Name = "availableObjectsGridControl";
		this.availableObjectsGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.iconRepositoryItemPictureEdit, this.isSelectRepositoryItemCheckEdit, this.typeRepositoryItemGridLookUpEdit });
		this.availableObjectsGridControl.ShowOnlyPredefinedDetails = true;
		this.availableObjectsGridControl.Size = new System.Drawing.Size(498, 429);
		this.availableObjectsGridControl.TabIndex = 3;
		this.availableObjectsGridControl.ToolTipController = this.toolTipController;
		this.availableObjectsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.availableObjectsGridView });
		this.availableObjectsGridControl.DragDrop += new System.Windows.Forms.DragEventHandler(AvailableObjectsGridControl_DragDrop);
		this.availableObjectsGridControl.DragEnter += new System.Windows.Forms.DragEventHandler(AvailableObjectsGridControl_DragEnter);
		this.availableObjectsGridControl.DragOver += new System.Windows.Forms.DragEventHandler(AvailableObjectsGridControl_DragOver);
		this.availableObjectsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.isSelectedGridColumn, this.iconGridColumn, this.typeGridColumn, this.nameGridColumn, this.locationGridColumn });
		this.availableObjectsGridView.GridControl = this.availableObjectsGridControl;
		this.availableObjectsGridView.Name = "availableObjectsGridView";
		this.availableObjectsGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.availableObjectsGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.availableObjectsGridView.OptionsNavigation.AutoMoveRowFocus = false;
		this.availableObjectsGridView.OptionsSelection.MultiSelect = true;
		this.availableObjectsGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.availableObjectsGridView.OptionsView.ColumnAutoWidth = false;
		this.availableObjectsGridView.OptionsView.RowAutoHeight = true;
		this.availableObjectsGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.availableObjectsGridView.OptionsView.ShowGroupPanel = false;
		this.availableObjectsGridView.OptionsView.ShowIndicator = false;
		this.availableObjectsGridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(AvailableObjectsGridView_CustomDrawCell);
		this.availableObjectsGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(AvailableObjectsGridView_PopupMenuShowing);
		this.availableObjectsGridView.ShowingEditor += new System.ComponentModel.CancelEventHandler(AvailableObjectsGridView_ShowingEditor);
		this.availableObjectsGridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(AvailableObjectsGridView_FocusedRowChanged);
		this.availableObjectsGridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(AvailableObjectsGridView_CellValueChanged);
		this.availableObjectsGridView.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(AvailableObjectsGridView_CellValueChanging);
		this.availableObjectsGridView.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(AvailableObjectsGridView_CustomUnboundColumnData);
		this.availableObjectsGridView.MouseDown += new System.Windows.Forms.MouseEventHandler(AvailableObjectsGridView_MouseDown);
		this.isSelectedGridColumn.Caption = " ";
		this.isSelectedGridColumn.ColumnEdit = this.isSelectRepositoryItemCheckEdit;
		this.isSelectedGridColumn.FieldName = "IsSelected";
		this.isSelectedGridColumn.MaxWidth = 29;
		this.isSelectedGridColumn.MinWidth = 29;
		this.isSelectedGridColumn.Name = "isSelectedGridColumn";
		this.isSelectedGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.isSelectedGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.isSelectedGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.isSelectedGridColumn.Visible = true;
		this.isSelectedGridColumn.VisibleIndex = 0;
		this.isSelectedGridColumn.Width = 29;
		this.isSelectRepositoryItemCheckEdit.AutoHeight = false;
		this.isSelectRepositoryItemCheckEdit.Name = "isSelectRepositoryItemCheckEdit";
		this.isSelectRepositoryItemCheckEdit.CheckedChanged += new System.EventHandler(IsSelectRepositoryItemCheckEdit_CheckedChanged);
		this.iconGridColumn.Caption = " ";
		this.iconGridColumn.ColumnEdit = this.iconRepositoryItemPictureEdit;
		this.iconGridColumn.FieldName = "iconGridColumn";
		this.iconGridColumn.MaxWidth = 21;
		this.iconGridColumn.MinWidth = 21;
		this.iconGridColumn.Name = "iconGridColumn";
		this.iconGridColumn.OptionsColumn.AllowEdit = false;
		this.iconGridColumn.OptionsFilter.AllowFilter = false;
		this.iconGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.iconGridColumn.Visible = true;
		this.iconGridColumn.VisibleIndex = 1;
		this.iconGridColumn.Width = 21;
		this.iconRepositoryItemPictureEdit.AllowFocused = false;
		this.iconRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.iconRepositoryItemPictureEdit.Name = "iconRepositoryItemPictureEdit";
		this.iconRepositoryItemPictureEdit.ShowMenu = false;
		this.typeGridColumn.Caption = "Type";
		this.typeGridColumn.ColumnEdit = this.typeRepositoryItemGridLookUpEdit;
		this.typeGridColumn.FieldName = "DataLakeType";
		this.typeGridColumn.Name = "typeGridColumn";
		this.typeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.typeGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.typeGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.typeGridColumn.Visible = true;
		this.typeGridColumn.VisibleIndex = 2;
		this.typeRepositoryItemGridLookUpEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.typeRepositoryItemGridLookUpEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.typeRepositoryItemGridLookUpEdit.DisplayMember = "DisplayName";
		this.typeRepositoryItemGridLookUpEdit.Name = "typeRepositoryItemGridLookUpEdit";
		this.typeRepositoryItemGridLookUpEdit.NullText = "";
		this.typeRepositoryItemGridLookUpEdit.PopupView = this.repositoryItemGridLookUpEditView;
		this.typeRepositoryItemGridLookUpEdit.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.None;
		this.typeRepositoryItemGridLookUpEdit.ShowFooter = false;
		this.typeRepositoryItemGridLookUpEdit.ValueMember = "Value";
		this.typeRepositoryItemGridLookUpEdit.BeforePopup += new System.EventHandler(TypeRepositoryItemGridLookUpEdit_BeforePopup);
		this.typeRepositoryItemGridLookUpEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(TypeRepositoryItemGridLookUpEdit_EditValueChanging);
		this.repositoryItemGridLookUpEditView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.displayNameGridColumn });
		this.repositoryItemGridLookUpEditView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.repositoryItemGridLookUpEditView.Name = "repositoryItemGridLookUpEditView";
		this.repositoryItemGridLookUpEditView.OptionsBehavior.Editable = false;
		this.repositoryItemGridLookUpEditView.OptionsBehavior.ReadOnly = true;
		this.repositoryItemGridLookUpEditView.OptionsCustomization.AllowColumnMoving = false;
		this.repositoryItemGridLookUpEditView.OptionsCustomization.AllowColumnResizing = false;
		this.repositoryItemGridLookUpEditView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.repositoryItemGridLookUpEditView.OptionsView.ShowColumnHeaders = false;
		this.repositoryItemGridLookUpEditView.OptionsView.ShowGroupPanel = false;
		this.repositoryItemGridLookUpEditView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryItemGridLookUpEditView.OptionsView.ShowIndicator = false;
		this.repositoryItemGridLookUpEditView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.displayNameGridColumn.Caption = "Name";
		this.displayNameGridColumn.FieldName = "DisplayName";
		this.displayNameGridColumn.Name = "displayNameGridColumn";
		this.displayNameGridColumn.Visible = true;
		this.displayNameGridColumn.VisibleIndex = 0;
		this.nameGridColumn.Caption = "Name";
		this.nameGridColumn.FieldName = "Name";
		this.nameGridColumn.Name = "nameGridColumn";
		this.nameGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.nameGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.nameGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.nameGridColumn.Visible = true;
		this.nameGridColumn.VisibleIndex = 3;
		this.locationGridColumn.Caption = "Location";
		this.locationGridColumn.FieldName = "Location";
		this.locationGridColumn.Name = "locationGridColumn";
		this.locationGridColumn.OptionsColumn.AllowEdit = false;
		this.locationGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.locationGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.locationGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.locationGridColumn.Visible = true;
		this.locationGridColumn.VisibleIndex = 4;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.objectsLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.objectsLayoutControlGroup.GroupBordersVisible = false;
		this.objectsLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.layoutControlItem1, this.availableSimpleLabelItem });
		this.objectsLayoutControlGroup.Name = "Root";
		this.objectsLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 10, 10, 10);
		this.objectsLayoutControlGroup.Size = new System.Drawing.Size(512, 470);
		this.objectsLayoutControlGroup.TextVisible = false;
		this.layoutControlItem1.Control = this.availableObjectsGridControl;
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 17);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(502, 433);
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.availableSimpleLabelItem.AllowHotTrack = false;
		this.availableSimpleLabelItem.Location = new System.Drawing.Point(0, 0);
		this.availableSimpleLabelItem.Name = "simpleLabelItem1";
		this.availableSimpleLabelItem.Size = new System.Drawing.Size(502, 17);
		this.availableSimpleLabelItem.Text = "Available objects";
		this.availableSimpleLabelItem.TextSize = new System.Drawing.Size(81, 13);
		this.fieldsMainLayoutControl.AllowCustomization = false;
		this.fieldsMainLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.fieldsMainLayoutControl.Controls.Add(this.fieldsGridControl);
		this.fieldsMainLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.fieldsMainLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.fieldsMainLayoutControl.Name = "fieldsMainLayoutControl";
		this.fieldsMainLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(915, 538, 650, 400);
		this.fieldsMainLayoutControl.Root = this.fieldsLayoutControlGroup;
		this.fieldsMainLayoutControl.Size = new System.Drawing.Size(553, 470);
		this.fieldsMainLayoutControl.TabIndex = 16;
		this.fieldsMainLayoutControl.Text = "nonCustomizableLayoutControl1";
		this.fieldsGridControl.Location = new System.Drawing.Point(12, 29);
		this.fieldsGridControl.MainView = this.fieldsGridView;
		this.fieldsGridControl.Name = "fieldsGridControl";
		this.fieldsGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.fieldIconRepositoryItemPictureEdit, this.columnFullNameRepositoryItemCustomTextEdit });
		this.fieldsGridControl.Size = new System.Drawing.Size(539, 429);
		this.fieldsGridControl.TabIndex = 4;
		this.fieldsGridControl.ToolTipController = this.toolTipController;
		this.fieldsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.fieldsGridView });
		this.fieldsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.fieldIconGridColumn, this.fieldTypeGridColumn, this.fieldOrdinalPositonGridColumn, this.fieldNameGridColumn, this.fieldDataTypeGridColumn, this.fieldSizeGridColumn, this.fieldNullableGridColumn });
		this.fieldsGridView.GridControl = this.fieldsGridControl;
		this.fieldsGridView.Name = "fieldsGridView";
		this.fieldsGridView.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDownFocused;
		this.fieldsGridView.OptionsClipboard.CopyColumnHeaders = DevExpress.Utils.DefaultBoolean.False;
		this.fieldsGridView.OptionsNavigation.AutoMoveRowFocus = false;
		this.fieldsGridView.OptionsSelection.MultiSelect = true;
		this.fieldsGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.fieldsGridView.OptionsView.ColumnAutoWidth = false;
		this.fieldsGridView.OptionsView.RowAutoHeight = true;
		this.fieldsGridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.fieldsGridView.OptionsView.ShowGroupPanel = false;
		this.fieldsGridView.OptionsView.ShowIndicator = false;
		this.fieldsGridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(FieldsGridView_PopupMenuShowing);
		this.fieldsGridView.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(FieldsGridView_CustomUnboundColumnData);
		this.fieldIconGridColumn.Caption = " ";
		this.fieldIconGridColumn.ColumnEdit = this.fieldIconRepositoryItemPictureEdit;
		this.fieldIconGridColumn.FieldName = "fieldIconGridColumn";
		this.fieldIconGridColumn.MaxWidth = 21;
		this.fieldIconGridColumn.MinWidth = 21;
		this.fieldIconGridColumn.Name = "fieldIconGridColumn";
		this.fieldIconGridColumn.OptionsColumn.AllowEdit = false;
		this.fieldIconGridColumn.OptionsFilter.AllowFilter = false;
		this.fieldIconGridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Object;
		this.fieldIconGridColumn.Visible = true;
		this.fieldIconGridColumn.VisibleIndex = 0;
		this.fieldIconGridColumn.Width = 21;
		this.fieldIconRepositoryItemPictureEdit.AllowFocused = false;
		this.fieldIconRepositoryItemPictureEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.False;
		this.fieldIconRepositoryItemPictureEdit.AllowScrollOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.fieldIconRepositoryItemPictureEdit.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.False;
		this.fieldIconRepositoryItemPictureEdit.Name = "fieldIconRepositoryItemPictureEdit";
		this.fieldIconRepositoryItemPictureEdit.ShowMenu = false;
		this.fieldTypeGridColumn.Caption = "Type";
		this.fieldTypeGridColumn.FieldName = "SubtypeDisplayText";
		this.fieldTypeGridColumn.Name = "fieldTypeGridColumn";
		this.fieldTypeGridColumn.OptionsColumn.AllowEdit = false;
		this.fieldTypeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.fieldTypeGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.fieldTypeGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.fieldTypeGridColumn.Visible = true;
		this.fieldTypeGridColumn.VisibleIndex = 1;
		this.fieldOrdinalPositonGridColumn.Caption = "#";
		this.fieldOrdinalPositonGridColumn.FieldName = "DisplayPosition";
		this.fieldOrdinalPositonGridColumn.MinWidth = 30;
		this.fieldOrdinalPositonGridColumn.Name = "fieldOrdinalPositonGridColumn";
		this.fieldOrdinalPositonGridColumn.OptionsColumn.AllowEdit = false;
		this.fieldOrdinalPositonGridColumn.OptionsColumn.ReadOnly = true;
		this.fieldOrdinalPositonGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.fieldOrdinalPositonGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.fieldOrdinalPositonGridColumn.Visible = true;
		this.fieldOrdinalPositonGridColumn.VisibleIndex = 2;
		this.fieldOrdinalPositonGridColumn.Width = 30;
		this.fieldNameGridColumn.Caption = "Name";
		this.fieldNameGridColumn.ColumnEdit = this.columnFullNameRepositoryItemCustomTextEdit;
		this.fieldNameGridColumn.FieldName = "FullNameFormatted";
		this.fieldNameGridColumn.Name = "fieldNameGridColumn";
		this.fieldNameGridColumn.OptionsColumn.AllowEdit = false;
		this.fieldNameGridColumn.OptionsColumn.ReadOnly = true;
		this.fieldNameGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.fieldNameGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.fieldNameGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.fieldNameGridColumn.Visible = true;
		this.fieldNameGridColumn.VisibleIndex = 3;
		this.fieldNameGridColumn.Width = 140;
		this.columnFullNameRepositoryItemCustomTextEdit.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.columnFullNameRepositoryItemCustomTextEdit.AutoHeight = false;
		this.columnFullNameRepositoryItemCustomTextEdit.Name = "columnFullNameRepositoryItemCustomTextEdit";
		this.fieldDataTypeGridColumn.Caption = "Data type";
		this.fieldDataTypeGridColumn.FieldName = "DataType";
		this.fieldDataTypeGridColumn.MinWidth = 70;
		this.fieldDataTypeGridColumn.Name = "fieldDataTypeGridColumn";
		this.fieldDataTypeGridColumn.OptionsColumn.AllowEdit = false;
		this.fieldDataTypeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.fieldDataTypeGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.fieldDataTypeGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.fieldDataTypeGridColumn.Visible = true;
		this.fieldDataTypeGridColumn.VisibleIndex = 4;
		this.fieldDataTypeGridColumn.Width = 70;
		this.fieldSizeGridColumn.Caption = "Size";
		this.fieldSizeGridColumn.FieldName = "DataTypeSizeString";
		this.fieldSizeGridColumn.MinWidth = 55;
		this.fieldSizeGridColumn.Name = "fieldSizeGridColumn";
		this.fieldSizeGridColumn.OptionsColumn.AllowEdit = false;
		this.fieldSizeGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.fieldSizeGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.fieldSizeGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.fieldSizeGridColumn.Visible = true;
		this.fieldSizeGridColumn.VisibleIndex = 5;
		this.fieldSizeGridColumn.Width = 55;
		this.fieldNullableGridColumn.Caption = "Nullable";
		this.fieldNullableGridColumn.FieldName = "Nullable";
		this.fieldNullableGridColumn.MinWidth = 46;
		this.fieldNullableGridColumn.Name = "fieldNullableGridColumn";
		this.fieldNullableGridColumn.OptionsColumn.AllowEdit = false;
		this.fieldNullableGridColumn.OptionsFilter.AutoFilterCondition = DevExpress.XtraGrid.Columns.AutoFilterCondition.Contains;
		this.fieldNullableGridColumn.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.fieldNullableGridColumn.OptionsFilter.ShowBlanksFilterItems = DevExpress.Utils.DefaultBoolean.True;
		this.fieldNullableGridColumn.Visible = true;
		this.fieldNullableGridColumn.VisibleIndex = 6;
		this.fieldNullableGridColumn.Width = 46;
		this.fieldsLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.fieldsLayoutControlGroup.GroupBordersVisible = false;
		this.fieldsLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.layoutControlItem2, this.fieldsSimpleLabelItem });
		this.fieldsLayoutControlGroup.Name = "Root";
		this.fieldsLayoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 0, 10, 10);
		this.fieldsLayoutControlGroup.Size = new System.Drawing.Size(553, 470);
		this.fieldsLayoutControlGroup.TextVisible = false;
		this.layoutControlItem2.Control = this.fieldsGridControl;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 17);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(543, 433);
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.fieldsSimpleLabelItem.AllowHotTrack = false;
		this.fieldsSimpleLabelItem.Location = new System.Drawing.Point(0, 0);
		this.fieldsSimpleLabelItem.Name = "fieldsSimpleLabelItem";
		this.fieldsSimpleLabelItem.Size = new System.Drawing.Size(543, 17);
		this.fieldsSimpleLabelItem.Text = "Fields:";
		this.fieldsSimpleLabelItem.TextSize = new System.Drawing.Size(31, 13);
		this.cancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelSimpleButton.Location = new System.Drawing.Point(992, 512);
		this.cancelSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.StyleController = this.mainLayoutControl;
		this.cancelSimpleButton.TabIndex = 6;
		this.cancelSimpleButton.Text = "Cancel";
		this.cancelSimpleButton.Click += new System.EventHandler(CancelSimpleButton_Click);
		this.importSimpleButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.importSimpleButton.Location = new System.Drawing.Point(890, 512);
		this.importSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.importSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.importSimpleButton.Name = "importSimpleButton";
		this.importSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.importSimpleButton.StyleController = this.mainLayoutControl;
		this.importSimpleButton.TabIndex = 5;
		this.importSimpleButton.TabStop = false;
		this.importSimpleButton.Text = "Import";
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(1099, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 546);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(1099, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 546);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(1099, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 546);
		this.mainLayoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.mainLayoutControlGroup.GroupBordersVisible = false;
		this.mainLayoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[7] { this.buttonsSeparator3EmptySpaceItem, this.cancelSimpleButtonLayoutControlItem, this.buttonsSeparator2EmptySpaceItem, this.importSimpleButtonLayoutControlItem, this.buttonsSeparator1EmptySpaceItem, this.splitContainerControlLayoutControlItem, this.addFilesSimpleButtonLayoutControlItem });
		this.mainLayoutControlGroup.Name = "Root";
		this.mainLayoutControlGroup.Size = new System.Drawing.Size(1099, 546);
		this.mainLayoutControlGroup.TextVisible = false;
		this.buttonsSeparator3EmptySpaceItem.AllowHotTrack = false;
		this.buttonsSeparator3EmptySpaceItem.Location = new System.Drawing.Point(1069, 500);
		this.buttonsSeparator3EmptySpaceItem.MaxSize = new System.Drawing.Size(10, 0);
		this.buttonsSeparator3EmptySpaceItem.MinSize = new System.Drawing.Size(10, 24);
		this.buttonsSeparator3EmptySpaceItem.Name = "buttonsSeparator3EmptySpaceItem";
		this.buttonsSeparator3EmptySpaceItem.Size = new System.Drawing.Size(10, 26);
		this.buttonsSeparator3EmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.buttonsSeparator3EmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.cancelSimpleButtonLayoutControlItem.Control = this.cancelSimpleButton;
		this.cancelSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(980, 500);
		this.cancelSimpleButtonLayoutControlItem.Name = "cancelSimpleButtonLayoutControlItem";
		this.cancelSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(89, 26);
		this.cancelSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.cancelSimpleButtonLayoutControlItem.TextVisible = false;
		this.buttonsSeparator2EmptySpaceItem.AllowHotTrack = false;
		this.buttonsSeparator2EmptySpaceItem.Location = new System.Drawing.Point(967, 500);
		this.buttonsSeparator2EmptySpaceItem.MaxSize = new System.Drawing.Size(13, 0);
		this.buttonsSeparator2EmptySpaceItem.MinSize = new System.Drawing.Size(13, 24);
		this.buttonsSeparator2EmptySpaceItem.Name = "buttonsSeparator2EmptySpaceItem";
		this.buttonsSeparator2EmptySpaceItem.Size = new System.Drawing.Size(13, 26);
		this.buttonsSeparator2EmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.buttonsSeparator2EmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.importSimpleButtonLayoutControlItem.Control = this.importSimpleButton;
		this.importSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(878, 500);
		this.importSimpleButtonLayoutControlItem.Name = "importSimpleButtonLayoutControlItem";
		this.importSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(89, 26);
		this.importSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.importSimpleButtonLayoutControlItem.TextVisible = false;
		this.buttonsSeparator1EmptySpaceItem.AllowHotTrack = false;
		this.buttonsSeparator1EmptySpaceItem.Location = new System.Drawing.Point(0, 500);
		this.buttonsSeparator1EmptySpaceItem.Name = "buttonsSeparator1EmptySpaceItem";
		this.buttonsSeparator1EmptySpaceItem.Size = new System.Drawing.Size(878, 26);
		this.buttonsSeparator1EmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.splitContainerControlLayoutControlItem.Control = this.splitContainerControl;
		this.splitContainerControlLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControlLayoutControlItem.Name = "splitContainerControlLayoutControlItem";
		this.splitContainerControlLayoutControlItem.Size = new System.Drawing.Size(1079, 474);
		this.splitContainerControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.splitContainerControlLayoutControlItem.TextVisible = false;
		this.addFilesSimpleButtonLayoutControlItem.ContentVertAlignment = DevExpress.Utils.VertAlignment.Bottom;
		this.addFilesSimpleButtonLayoutControlItem.Control = this.addFilesSimpleButton;
		this.addFilesSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(0, 474);
		this.addFilesSimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(89, 26);
		this.addFilesSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(89, 26);
		this.addFilesSimpleButtonLayoutControlItem.Name = "addFilesSimpleButtonLayoutControlItem";
		this.addFilesSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(1079, 26);
		this.addFilesSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.addFilesSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.addFilesSimpleButtonLayoutControlItem.TextVisible = false;
		this.openFileDialog.Multiselect = true;
		this.splashScreenManager.ClosingDelay = 500;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1099, 546);
		base.Controls.Add(this.mainLayoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		this.MinimumSize = new System.Drawing.Size(420, 300);
		base.Name = "ImportChooseObjects";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Choose Objects";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ImportChooseObjects_FormClosing);
		base.Shown += new System.EventHandler(ImportChooseObjects_Shown);
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControl).EndInit();
		this.mainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl).EndInit();
		this.splitContainerControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.objectsLayoutControl).EndInit();
		this.objectsLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.availableObjectsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.availableObjectsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.isSelectRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.iconRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeRepositoryItemGridLookUpEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEditView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.objectsLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.availableSimpleLabelItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsMainLayoutControl).EndInit();
		this.fieldsMainLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.fieldsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fieldIconRepositoryItemPictureEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.columnFullNameRepositoryItemCustomTextEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsSimpleLabelItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.mainLayoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsSeparator3EmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cancelSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsSeparator2EmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.importSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.buttonsSeparator1EmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.addFilesSimpleButtonLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
