using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ColorCode;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Forms.Tools;
using Dataedo.App.Import.DataLake.Model;
using Dataedo.App.MenuTree;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.CustomFields;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Model.Data.Tables.Tables;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using DevExpress.DataProcessing;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraTab;

namespace Dataedo.App.Forms;

public class DesignTableForm : BaseXtraForm
{
	private Form parentForm;

	private bool hasMultipleLevelColumns;

	private bool forceExistanceValidation;

	private DXErrorProvider errorProvider;

	private readonly CodeColorizer codeColorizer;

	private IContainer components;

	private NonCustomizableLayoutControl layoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	private SimpleButton cancelSimpleButton;

	private SimpleButton saveSimpleButton;

	private EmptySpaceItem emptySpaceItem5;

	private LayoutControlItem layoutControlItem6;

	private LayoutControlItem layoutControlItem7;

	private EmptySpaceItem emptySpaceItem6;

	private LabelControl schemaLabelControl;

	private LayoutControlItem schemaLabelLayoutControlItem;

	private LabelControl nameLabelControl;

	private LayoutControlItem layoutControlItem9;

	private EmptySpaceItem emptySpaceItem8;

	private TextEdit nameTextEdit;

	private TextEdit schemaTextEdit;

	private LayoutControlItem schemaLayoutControlItem;

	private LayoutControlItem layoutControlItem11;

	private LabelControl documentationLabelControl;

	private LayoutControlItem layoutControlItem14;

	private RibbonControl ribbonControl;

	private RibbonPage ribbonPage1;

	private RibbonPageGroup ribbonPageGroup1;

	private LayoutControlItem ribbonLayoutControlItem;

	private BarButtonItem barButtonItem1;

	private BarButtonItem addBarButtonItem;

	private RibbonPage ribbonPage;

	private RibbonPageGroup manipulateRibbonPageGroup;

	private RibbonPageGroup sortRibbonPageGroup;

	private BarButtonItem removeBarButtonItem;

	private BarButtonItem moveUpBarButtonItem;

	private BarButtonItem defaultSortBarButtonItem;

	private BarButtonItem addBulkColumnsBarButtonItem;

	private RibbonPageGroup bulkColumnsRibbonPageGroup;

	private BarButtonItem moveDownBarButtonItem;

	private EmptySpaceItem emptySpaceItem1;

	private LabelControl documentationTitleLabelControl;

	private LayoutControlItem documentationTitleLayoutContorlItem;

	private EmptySpaceItem emptySpaceItem2;

	private LabelControl titleLabelControl;

	private TextEdit titleTextEdit;

	private LayoutControlItem layoutControlItem15;

	private LayoutControlItem layoutControlItem16;

	private LabelControl labelControl1;

	private GridLookUpEdit typeGridLookUpEdit;

	private GridView typeGridLookUpEditView;

	private LayoutControlItem typeGridLayoutControlItem;

	private LayoutControlItem layoutControlItem2;

	private BindingSource subtypeBindingSource;

	private GridColumn displayNameGridColumn;

	private GridColumn iconGridColumn;

	private BarButtonItem moveToBottomBarButtonItem;

	private BarButtonItem moveToTopBarButtonItem;

	private EmptySpaceItem emptySpaceItem4;

	private EmptySpaceItem emptySpaceItem7;

	private EmptySpaceItem emptySpaceItem3;

	private EmptySpaceItem emptySpaceItem9;

	private XtraTabControl xtraTabControl;

	private XtraTabPage columnsXtraTabPage;

	private XtraTabPage schemaXtraTabPage;

	private LayoutControlItem xtraTabControlLayoutControlItem;

	private DesignTableUserControl designTableUserControl;

	private TextEdit locationTextEdit;

	private LabelControl locationLabelControl;

	private LayoutControlItem locationLabelLayoutControlItem;

	private LayoutControlItem locationLayoutControlItem;

	private ToolTipController toolTipController;

	private DefaultToolTipController defaultToolTipController;

	private MemoEdit schemaMemoEdit;

	private XtraTabPage scriptXtraTabPage;

	private RichEditUserControl scriptRichEditControl;

	public TableDesigner TableDesigner { get; }

	public bool CanSave { get; private set; }

	public DesignTableForm(SharedObjectTypeEnum.ObjectType objectType, CustomFieldsSupport customFieldsSupport)
	{
		ColumnRow.UniqueIdSource = 1;
		InitializeComponent();
		errorProvider = new DXErrorProvider();
		codeColorizer = new CodeColorizer();
		LengthValidation.SetTitleOrNameLengthLimit(nameTextEdit);
		LengthValidation.SetTitleOrNameLengthLimit(schemaTextEdit);
		LengthValidation.SetTitleOrNameLengthLimit(titleTextEdit);
		designTableUserControl.SetRemoveButtonAvailability += SetRemoveButtonAvailability;
		SharedObjectTypeEnum.TypeToStringForSingle(objectType);
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Structure:
		{
			LayoutVisibility layoutVisibility3 = (schemaLabelLayoutControlItem.Visibility = (schemaLayoutControlItem.Visibility = LayoutVisibility.Never));
			layoutVisibility3 = (locationLabelLayoutControlItem.Visibility = (locationLayoutControlItem.Visibility = LayoutVisibility.Always));
			scriptXtraTabPage.PageVisible = false;
			break;
		}
		case SharedObjectTypeEnum.ObjectType.View:
		{
			LayoutVisibility layoutVisibility3 = (schemaLabelLayoutControlItem.Visibility = (schemaLayoutControlItem.Visibility = LayoutVisibility.Always));
			layoutVisibility3 = (locationLabelLayoutControlItem.Visibility = (locationLayoutControlItem.Visibility = LayoutVisibility.Never));
			scriptRichEditControl.KeyDown += ScriptRichEditControl_KeyDown;
			scriptXtraTabPage.PageVisible = true;
			scriptRichEditControl.RefreshSkin();
			break;
		}
		default:
		{
			LayoutVisibility layoutVisibility3 = (schemaLabelLayoutControlItem.Visibility = (schemaLayoutControlItem.Visibility = LayoutVisibility.Always));
			layoutVisibility3 = (locationLabelLayoutControlItem.Visibility = (locationLayoutControlItem.Visibility = LayoutVisibility.Never));
			scriptXtraTabPage.PageVisible = false;
			break;
		}
		}
		xtraTabControl.SelectedPageChanged += XtraTabControl_SelectedPageChanged;
		SetParameters(objectType);
		xtraTabControl.SelectedTabPage = columnsXtraTabPage;
		switch (objectType)
		{
		case SharedObjectTypeEnum.ObjectType.Structure:
			xtraTabControl.ShowTabHeader = DefaultBoolean.True;
			schemaXtraTabPage.PageVisible = true;
			scriptXtraTabPage.PageVisible = false;
			break;
		case SharedObjectTypeEnum.ObjectType.View:
			xtraTabControl.ShowTabHeader = DefaultBoolean.True;
			schemaXtraTabPage.PageVisible = false;
			scriptXtraTabPage.PageVisible = true;
			break;
		default:
			xtraTabControl.ShowTabHeader = DefaultBoolean.False;
			break;
		}
	}

	public DesignTableForm(Form parentForm, int databaseId, SharedObjectTypeEnum.ObjectType objectType, CustomFieldsSupport customFieldsSupport)
		: this(objectType, customFieldsSupport)
	{
		this.parentForm = parentForm;
		TableDesigner = new TableDesigner(objectType);
		TableDesigner.DatabaseId = databaseId;
		TableDesigner.CustomFieldsSupport = customFieldsSupport;
		designTableUserControl.Init(TableDesigner);
		typeGridLookUpEdit.EditValue = SharedObjectSubtypeEnum.GetDefaultByMainType(objectType);
		Text = "Add " + SharedObjectTypeEnum.TypeToStringForSingle(objectType);
		TableDesigner.DocumentationTitle = DBTreeMenu.FindDBNodeById(SharedObjectTypeEnum.ObjectType.Database, databaseId).Name;
		documentationTitleLabelControl.Text = CommonFunctionsPanels.SetTitle(isEdited: false, TableDesigner.DocumentationTitle);
	}

	public DesignTableForm(Form parentForm, int databaseId, ObjectModel objectModel, UserTypeEnum.UserType source, SynchronizeStateEnum.SynchronizeState synchronizeState, bool forceExistanceValidation, bool validateOnStart, CustomFieldsSupport customFieldsSupport)
		: this(parentForm, databaseId, objectModel.ObjectType, customFieldsSupport)
	{
		DesignTableForm designTableForm = this;
		this.forceExistanceValidation = forceExistanceValidation;
		hasMultipleLevelColumns = objectModel.HasHierarchicalFiels;
		SetFunctionality();
		nameTextEdit.Text = objectModel.Name;
		locationTextEdit.Text = objectModel.Location;
		typeGridLookUpEdit.EditValue = ((typeGridLookUpEdit.Properties.DataSource as List<Subtype>).Any((Subtype x) => x.SubType == objectModel.ObjectSubtype) ? objectModel.ObjectSubtype : SharedObjectSubtypeEnum.GetDefaultByMainType(objectModel.ObjectType));
		TableDesigner.Source = source;
		TableDesigner.SynchronizeState = synchronizeState;
		List<ColumnRowFromObjectModel> sourceColumns = objectModel.Fields.Select((FieldModel x) => designTableForm.CreateColumnRowFromObjectModel(x, source)).ToList();
		sourceColumns.Where((ColumnRowFromObjectModel x) => x.Level > 1).ForEach(delegate(ColumnRowFromObjectModel x)
		{
			ColumnRowFromObjectModel parentColumn = sourceColumns.First((ColumnRowFromObjectModel y) => x.SourceFieldModel.ParentField == y.SourceFieldModel);
			x.ParentColumn = parentColumn;
		});
		TableDesigner.DataSourceColumns = sourceColumns.Cast<ColumnRow>().ToList();
		designTableUserControl.RefreshDataSource();
		if (validateOnStart)
		{
			Validate();
		}
	}

	private ColumnRowFromObjectModel CreateColumnRowFromObjectModel(FieldModel fieldModel, UserTypeEnum.UserType source)
	{
		ColumnRowFromObjectModel columnRowFromObjectModel = new ColumnRowFromObjectModel();
		columnRowFromObjectModel.UniqueId = ColumnRow.UniqueIdSource++;
		columnRowFromObjectModel.Type = SharedObjectSubtypeEnum.TypeToString(fieldModel.ObjectType, fieldModel.ObjectSubtype);
		columnRowFromObjectModel.Name = fieldModel.Name;
		columnRowFromObjectModel.DataTypeWithoutLength = fieldModel.DataType;
		columnRowFromObjectModel.DataLength = ((!fieldModel.DataTypeSize.HasValue) ? null : fieldModel.DataTypeSize.ToString());
		columnRowFromObjectModel.Nullable = fieldModel.Nullable;
		columnRowFromObjectModel.Path = fieldModel.Path;
		columnRowFromObjectModel.Level = fieldModel.Level;
		columnRowFromObjectModel.Position = fieldModel.Position;
		columnRowFromObjectModel.RowState = ManagingRowsEnum.ManagingRows.ForAdding;
		columnRowFromObjectModel.Source = source;
		columnRowFromObjectModel.SourceFieldModel = fieldModel;
		columnRowFromObjectModel.Title = fieldModel.Title;
		columnRowFromObjectModel.Description = fieldModel.Description;
		columnRowFromObjectModel.CustomFields = new CustomFieldContainer(TableDesigner.CustomFieldsSupport);
		columnRowFromObjectModel.CustomFields.RetrieveCustomFields();
		return columnRowFromObjectModel;
	}

	public DesignTableForm(DBTreeNode selectedNode, CustomFieldsSupport customFieldsSupport)
		: this(selectedNode.DatabaseId, selectedNode.Id, selectedNode.Schema, selectedNode.BaseName, selectedNode.Title, selectedNode.Source.Value, selectedNode.ObjectType, selectedNode.Subtype, selectedNode.DatabaseTitle, customFieldsSupport)
	{
	}

	public DesignTableForm(int databaseId, int tableId, string schema, string name, string title, UserTypeEnum.UserType source, SharedObjectTypeEnum.ObjectType type, SharedObjectSubtypeEnum.ObjectSubtype subtype, string documentationTitle, CustomFieldsSupport customFieldsSupport)
		: this(type, customFieldsSupport)
	{
		schemaTextEdit.Text = schema;
		nameTextEdit.Text = name;
		titleTextEdit.Text = title;
		string definition = null;
		if (type == SharedObjectTypeEnum.ObjectType.Structure)
		{
			TableObject dataById = DB.Table.GetDataById(tableId);
			hasMultipleLevelColumns = DB.Table.HasMultipleLevelColumns(tableId);
			SetFunctionality();
			definition = dataById.Definition;
			TableDesigner = new TableDesigner(tableId, schema, name, source, type, definition, customFieldsSupport);
			Text = "Design " + SharedObjectTypeEnum.TypeToStringForSingle(type) + ": " + dataById.Name;
			schemaMemoEdit.Text = definition;
			locationTextEdit.Text = dataById.Location;
		}
		else if (type == SharedObjectTypeEnum.ObjectType.View)
		{
			TableObject dataById2 = DB.Table.GetDataById(tableId);
			hasMultipleLevelColumns = DB.Table.HasMultipleLevelColumns(tableId);
			SetFunctionality();
			definition = dataById2.Definition;
			TableDesigner = new TableDesigner(tableId, schema, name, source, type, definition, customFieldsSupport);
			Text = "Design " + SharedObjectTypeEnum.TypeToStringForSingle(type) + ": " + dataById2.Name;
			scriptRichEditControl.Text = definition;
		}
		else
		{
			TableDesigner = new TableDesigner(tableId, schema, name, source, type, definition, customFieldsSupport);
			Text = "Design " + SharedObjectTypeEnum.TypeToStringForSingle(type) + ": " + (string.IsNullOrEmpty(schema) ? (name ?? "") : (schema + "." + name));
		}
		designTableUserControl.Init(TableDesigner);
		TableDesigner.OldName = name;
		TableDesigner.OldSchema = schema;
		TableDesigner.OldTitle = title;
		TableDesigner.DatabaseId = databaseId;
		if (!(typeGridLookUpEdit.Properties.DataSource as List<Subtype>).Any((Subtype x) => x.Type == type && x.SubType == subtype))
		{
			subtype = SharedObjectSubtypeEnum.GetDefaultByMainType(type);
		}
		typeGridLookUpEdit.EditValue = (TableDesigner.Type = (TableDesigner.OldType = subtype));
		TableDesigner.DocumentationTitle = CommonFunctionsPanels.SetTitle(isEdited: false, documentationTitle);
		documentationTitleLabelControl.Text = TableDesigner.DocumentationTitle;
		defaultSortBarButtonItem.Visibility = ((TableDesigner.Source != UserTypeEnum.UserType.DBMS) ? BarItemVisibility.Never : BarItemVisibility.Always);
	}

	private void SetFunctionality()
	{
		_ = hasMultipleLevelColumns;
	}

	private void moveUpSimpleButton_Click(object sender, EventArgs e)
	{
		designTableUserControl.MoveUp();
	}

	private void moveDownSimpleButton_Click(object sender, EventArgs e)
	{
		designTableUserControl.MoveDown();
	}

	private void cancelSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void saveSimpleButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	public void SetParameters(SharedObjectTypeEnum.ObjectType objectType)
	{
		typeGridLookUpEdit.Properties.DataSource = ObjectSubtypeEnum.GetSubtypes(objectType);
		int count = (typeGridLookUpEdit.Properties.DataSource as List<Subtype>).Count;
		int num = ((count > 15) ? 15 : count);
		typeGridLookUpEdit.Properties.PopupFormMinSize = new Size(typeGridLookUpEdit.Width, (typeGridLookUpEdit.Height + 2) * num);
		typeGridLookUpEdit.Properties.PopupFormSize = new Size(typeGridLookUpEdit.Width, (typeGridLookUpEdit.Height + 2) * num);
	}

	private void Save()
	{
		designTableUserControl.CloseEditor();
		CanSave = true;
		TableDesigner.ModifySource();
		errorProvider.ClearErrors();
		Validate();
		if (designTableUserControl.IsNotValid() || !CanSave)
		{
			base.DialogResult = DialogResult.None;
			return;
		}
		if (!TableDesigner.IsChanged)
		{
			base.DialogResult = DialogResult.OK;
			return;
		}
		TableDesigner.RemoveFromIgnoredObjects(this);
		if (!TableDesigner.IsEditMode)
		{
			TableDesigner.InsertTable(this);
			base.DialogResult = DialogResult.OK;
		}
		else
		{
			TableDesigner.UpdateTable(this);
		}
		TableDesigner.SaveColumns(this);
	}

	private new void Validate()
	{
		TableDesigner.Schema = schemaTextEdit.Text;
		TableDesigner.Name = nameTextEdit.Text;
		TableDesigner.Title = titleTextEdit.Text;
		TableDesigner.Location = locationTextEdit.Text;
		if (TableDesigner.ObjectTypeValue == SharedObjectTypeEnum.ObjectType.View)
		{
			TableDesigner.Definition = scriptRichEditControl.Text;
		}
		else
		{
			TableDesigner.Definition = schemaMemoEdit.Text;
		}
		errorProvider.ClearErrors();
		if (TableDesigner.Source == UserTypeEnum.UserType.USER || forceExistanceValidation)
		{
			string text = SharedObjectTypeEnum.TypeToStringForSingle(TableDesigner.ObjectTypeValue);
			SetError(TableDesigner.Exists("Schema", "Name"), text + " with this name already exists");
			SetError(string.IsNullOrEmpty(TableDesigner.Name), "Name can not be empty");
		}
	}

	private void SetError(bool condition, string message, Control control = null)
	{
		if (condition)
		{
			errorProvider.SetError(control ?? nameTextEdit, message, ErrorType.Critical);
			CanSave = false;
			base.DialogResult = DialogResult.None;
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.S | Keys.Control:
			base.DialogResult = DialogResult.OK;
			Close();
			break;
		case Keys.Escape:
			Close();
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	public void BlockAddColumnsButton()
	{
		if (addBarButtonItem.Enabled && TableDesigner.DataSourceColumns.Count >= ColumnsHelper.MaxColumnsCount)
		{
			BarButtonItem barButtonItem = addBarButtonItem;
			bool enabled = (addBulkColumnsBarButtonItem.Enabled = false);
			barButtonItem.Enabled = enabled;
		}
	}

	public void UnblockAddColumnsButton()
	{
		if (!addBarButtonItem.Enabled && TableDesigner.DataSourceColumns.Count < ColumnsHelper.MaxColumnsCount)
		{
			BarButtonItem barButtonItem = addBarButtonItem;
			bool enabled = (addBulkColumnsBarButtonItem.Enabled = true);
			barButtonItem.Enabled = enabled;
		}
	}

	private void SetRemoveButtonAvailability()
	{
		ColumnRow columnRow = designTableUserControl?.FocusedRow;
		removeBarButtonItem.Enabled = columnRow != null && columnRow != null && columnRow.Source == UserTypeEnum.UserType.USER;
	}

	private void TextEdit_TextChanged(object sender, EventArgs e)
	{
		if (TableDesigner != null)
		{
			TableDesigner tableDesigner = TableDesigner;
			bool isChanged = (TableDesigner.IsTableChanged = true);
			tableDesigner.IsChanged = isChanged;
		}
	}

	private void ManualTableForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (base.DialogResult == DialogResult.OK)
		{
			Save();
		}
		else
		{
			if (!TableDesigner.IsChanged)
			{
				return;
			}
			string text = SharedObjectTypeEnum.TypeToStringForSingle(TableDesigner.ObjectTypeValue);
			DialogResult? dialogResult = GeneralMessageBoxesHandling.Show(text + " has been changed, would you like to save these changes?", text + " has been changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
			if (dialogResult == DialogResult.Yes)
			{
				base.DialogResult = DialogResult.OK;
				Save();
				return;
			}
			if (dialogResult == DialogResult.No)
			{
				base.DialogResult = DialogResult.Cancel;
				{
					foreach (CustomFieldRowExtended visibleField in TableDesigner.CustomFieldsSupport.GetVisibleFields(TableDesigner.ObjectTypeValue))
					{
						visibleField.ClearAddedDefinitionValues();
					}
					return;
				}
			}
			if (dialogResult != DialogResult.No)
			{
				base.DialogResult = DialogResult.Cancel;
				e.Cancel = true;
			}
		}
	}

	private void resetSortSimpleButton_Click(object sender, EventArgs e)
	{
		designTableUserControl.ResetSort();
		TableDesigner.IsChanged = true;
	}

	private void removeBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		designTableUserControl.CloseEditor();
		designTableUserControl.RemoveColumn();
		designTableUserControl.Refresh();
		UnblockAddColumnsButton();
		SetRemoveButtonAvailability();
	}

	private void addBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		designTableUserControl.AddColumn();
		designTableUserControl.Refresh();
		BlockAddColumnsButton();
	}

	private void defaultSortBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		designTableUserControl.ResetSort();
		TableDesigner.IsChanged = true;
	}

	private void moveUpBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		designTableUserControl.MoveUp();
	}

	private void moveDownBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		designTableUserControl.MoveDown();
	}

	private void addBulkColumnsBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		AddBulkColumnsForm addBulkColumnsForm = new AddBulkColumnsForm(TableDesigner.ObjectTypeValue, TableDesigner.TableId, TableDesigner.DataSourceColumns.Count, TableDesigner.CustomFieldsSupport);
		addBulkColumnsForm.ShowDialog();
		if (addBulkColumnsForm.DataSourceColumns.Count > 0 && addBulkColumnsForm.DialogResult == DialogResult.OK)
		{
			List<ColumnRow> dataSourceColumns = addBulkColumnsForm.DataSourceColumns;
			dataSourceColumns.ForEach(delegate(ColumnRow x)
			{
				x.UniqueId = ++ColumnRow.UniqueIdSource;
			});
			TableDesigner.DataSourceColumns.AddRange(dataSourceColumns);
			foreach (ColumnRow dataSourceColumn in TableDesigner.DataSourceColumns)
			{
				_ = dataSourceColumn;
				designTableUserControl.UpdateCustomFields();
			}
			designTableUserControl.RefreshDataSource();
			TableDesigner.IsChanged = true;
			BlockAddColumnsButton();
			return;
		}
		foreach (CustomFieldRowExtended visibleField in TableDesigner.CustomFieldsSupport.GetVisibleFields(TableDesigner.ObjectTypeValue))
		{
			visibleField.ClearAddedDefinitionValues();
		}
	}

	private void DesignTableForm_Load(object sender, EventArgs e)
	{
		ribbonControl.Manager.UseAltKeyForMenu = false;
		BarButtonItem barButtonItem = addBarButtonItem;
		bool enabled = (addBulkColumnsBarButtonItem.Enabled = TableDesigner.DataSourceColumns.Count < ColumnsHelper.MaxColumnsCount);
		barButtonItem.Enabled = enabled;
		defaultSortBarButtonItem.Visibility = ((TableDesigner.Source != UserTypeEnum.UserType.DBMS) ? BarItemVisibility.Never : BarItemVisibility.Always);
		BarButtonItem barButtonItem2 = removeBarButtonItem;
		int enabled2;
		if (designTableUserControl?.FocusedRow != null)
		{
			DesignTableUserControl obj = designTableUserControl;
			enabled2 = ((obj != null && obj.FocusedRow?.Source == UserTypeEnum.UserType.USER) ? 1 : 0);
		}
		else
		{
			enabled2 = 0;
		}
		barButtonItem2.Enabled = (byte)enabled2 != 0;
	}

	private void typeGridLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		TableDesigner.Type = (typeGridLookUpEdit.GetSelectedDataRow() as Subtype).SubType;
		ModifySource();
		TableDesigner.IsChanged = (sender as BaseEdit).IsModified;
	}

	private void moveToBottomBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		designTableUserControl.MoveToBottom();
	}

	private void moveToTopBarButtonItem_ItemClick(object sender, ItemClickEventArgs e)
	{
		designTableUserControl.MoveToTop();
	}

	private void TextEdit_EditValueChanging(object sender, ChangingEventArgs e)
	{
		ModifySource();
		errorProvider.ClearErrors();
	}

	private void ModifySource()
	{
		if (TableDesigner != null && TableDesigner.Source == UserTypeEnum.UserType.DBMS)
		{
			TableDesigner.Schema = schemaTextEdit.Text;
			TableDesigner.Name = nameTextEdit.Text;
			TableDesigner.HasModifiedSourceProperties();
			designTableUserControl.RefreshDataSource();
		}
	}

	private void schemaMemoEdit_TextChanged(object sender, EventArgs e)
	{
		TableDesigner.IsChanged = true;
	}

	private void locationTextEdit_TextChanged(object sender, EventArgs e)
	{
		TableDesigner.IsChanged = true;
	}

	private void DesignTableForm_Shown(object sender, EventArgs e)
	{
		if (parentForm != null)
		{
			parentForm.Visible = false;
		}
	}

	private void ScriptRichEditControl_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Space || (e.Control && e.KeyCode == Keys.V))
		{
			FormatScriptRichEditControlText();
		}
	}

	private void XtraTabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		if (e.Page == scriptXtraTabPage)
		{
			FormatScriptRichEditControlText();
		}
	}

	private void FormatScriptRichEditControlText()
	{
		DocumentPosition documentPosition = scriptRichEditControl.Document?.CaretPosition;
		RichEditUserControlHelper.ColorizeSyntax(scriptRichEditControl, codeColorizer, scriptRichEditControl.Text, "SQL");
		if (documentPosition != null)
		{
			scriptRichEditControl.Document.CaretPosition = documentPosition;
		}
	}

	private void ScriptRichEditControl_TextChanged(object sender, EventArgs e)
	{
		if (sender is RichEditUserControl richEditUserControl && richEditUserControl.ContainsFocus)
		{
			TableDesigner.IsChanged = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.DesignTableForm));
		this.layoutControl1 = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.locationTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.locationLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
		this.columnsXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.designTableUserControl = new Dataedo.App.UserControls.DesignTableUserControl();
		this.schemaXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.schemaMemoEdit = new DevExpress.XtraEditors.MemoEdit();
		this.scriptXtraTabPage = new DevExpress.XtraTab.XtraTabPage();
		this.scriptRichEditControl = new Dataedo.App.UserControls.RichEditUserControl();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.typeGridLookUpEdit = new DevExpress.XtraEditors.GridLookUpEdit();
		this.ribbonControl = new DevExpress.XtraBars.Ribbon.RibbonControl();
		this.barButtonItem1 = new DevExpress.XtraBars.BarButtonItem();
		this.addBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.removeBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveUpBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.defaultSortBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.addBulkColumnsBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveDownBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveToBottomBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.moveToTopBarButtonItem = new DevExpress.XtraBars.BarButtonItem();
		this.ribbonPage = new DevExpress.XtraBars.Ribbon.RibbonPage();
		this.manipulateRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.sortRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.bulkColumnsRibbonPageGroup = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.subtypeBindingSource = new System.Windows.Forms.BindingSource(this.components);
		this.typeGridLookUpEditView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.displayNameGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.iconGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.titleLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.titleTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.documentationTitleLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.documentationLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.nameTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.schemaTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.nameLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.schemaLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.saveSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem6 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.schemaLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem8 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.schemaLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem11 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem14 = new DevExpress.XtraLayout.LayoutControlItem();
		this.documentationTitleLayoutContorlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.ribbonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem15 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem16 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.typeGridLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem7 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem9 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.xtraTabControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.locationLabelLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.locationLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
		this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
		this.defaultToolTipController = new DevExpress.Utils.DefaultToolTipController(this.components);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.locationTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl).BeginInit();
		this.xtraTabControl.SuspendLayout();
		this.columnsXtraTabPage.SuspendLayout();
		this.schemaXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.schemaMemoEdit.Properties).BeginInit();
		this.scriptXtraTabPage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.typeGridLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.subtypeBindingSource).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeGridLookUpEditView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.schemaTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.schemaLabelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.schemaLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem14).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.documentationTitleLayoutContorlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ribbonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem15).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeGridLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.locationLabelLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.locationLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.AllowCustomization = false;
		this.layoutControl1.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl1.Controls.Add(this.locationTextEdit);
		this.layoutControl1.Controls.Add(this.locationLabelControl);
		this.layoutControl1.Controls.Add(this.xtraTabControl);
		this.layoutControl1.Controls.Add(this.labelControl1);
		this.layoutControl1.Controls.Add(this.typeGridLookUpEdit);
		this.layoutControl1.Controls.Add(this.ribbonControl);
		this.layoutControl1.Controls.Add(this.titleLabelControl);
		this.layoutControl1.Controls.Add(this.titleTextEdit);
		this.layoutControl1.Controls.Add(this.documentationTitleLabelControl);
		this.layoutControl1.Controls.Add(this.documentationLabelControl);
		this.layoutControl1.Controls.Add(this.nameTextEdit);
		this.layoutControl1.Controls.Add(this.schemaTextEdit);
		this.layoutControl1.Controls.Add(this.nameLabelControl);
		this.layoutControl1.Controls.Add(this.schemaLabelControl);
		this.layoutControl1.Controls.Add(this.cancelSimpleButton);
		this.layoutControl1.Controls.Add(this.saveSimpleButton);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(2784, 285, 598, 666);
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(957, 649);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.layoutControl1.ToolTipController = this.toolTipController;
		this.locationTextEdit.Location = new System.Drawing.Point(95, 233);
		this.locationTextEdit.MaximumSize = new System.Drawing.Size(0, 20);
		this.locationTextEdit.MinimumSize = new System.Drawing.Size(251, 20);
		this.locationTextEdit.Name = "locationTextEdit";
		this.locationTextEdit.Size = new System.Drawing.Size(852, 20);
		this.locationTextEdit.StyleController = this.layoutControl1;
		this.locationTextEdit.TabIndex = 26;
		this.locationTextEdit.TextChanged += new System.EventHandler(locationTextEdit_TextChanged);
		this.locationLabelControl.Location = new System.Drawing.Point(12, 233);
		this.locationLabelControl.MaximumSize = new System.Drawing.Size(79, 20);
		this.locationLabelControl.MinimumSize = new System.Drawing.Size(79, 20);
		this.locationLabelControl.Name = "locationLabelControl";
		this.locationLabelControl.Size = new System.Drawing.Size(79, 20);
		this.locationLabelControl.StyleController = this.layoutControl1;
		this.locationLabelControl.TabIndex = 25;
		this.locationLabelControl.Text = "Location:";
		this.xtraTabControl.Location = new System.Drawing.Point(2, 266);
		this.xtraTabControl.Name = "xtraTabControl";
		this.xtraTabControl.SelectedTabPage = this.columnsXtraTabPage;
		this.xtraTabControl.Size = new System.Drawing.Size(953, 337);
		this.xtraTabControl.TabIndex = 23;
		this.xtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.columnsXtraTabPage, this.schemaXtraTabPage, this.scriptXtraTabPage });
		this.columnsXtraTabPage.Controls.Add(this.designTableUserControl);
		this.columnsXtraTabPage.Name = "columnsXtraTabPage";
		this.columnsXtraTabPage.Size = new System.Drawing.Size(951, 308);
		this.columnsXtraTabPage.Text = "Columns";
		this.defaultToolTipController.SetAllowHtmlText(this.designTableUserControl, DevExpress.Utils.DefaultBoolean.Default);
		this.designTableUserControl.BackColor = System.Drawing.Color.Transparent;
		this.designTableUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.designTableUserControl.Location = new System.Drawing.Point(0, 0);
		this.designTableUserControl.Margin = new System.Windows.Forms.Padding(0);
		this.designTableUserControl.Name = "designTableUserControl";
		this.designTableUserControl.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
		this.designTableUserControl.Size = new System.Drawing.Size(951, 308);
		this.designTableUserControl.TabIndex = 4;
		this.schemaXtraTabPage.Controls.Add(this.schemaMemoEdit);
		this.schemaXtraTabPage.Name = "schemaXtraTabPage";
		this.schemaXtraTabPage.Size = new System.Drawing.Size(951, 308);
		this.schemaXtraTabPage.Text = "Schema";
		this.schemaMemoEdit.Dock = System.Windows.Forms.DockStyle.Fill;
		this.schemaMemoEdit.Location = new System.Drawing.Point(0, 0);
		this.schemaMemoEdit.Name = "schemaMemoEdit";
		this.schemaMemoEdit.Size = new System.Drawing.Size(951, 308);
		this.schemaMemoEdit.TabIndex = 13;
		this.schemaMemoEdit.TextChanged += new System.EventHandler(schemaMemoEdit_TextChanged);
		this.scriptXtraTabPage.Controls.Add(this.scriptRichEditControl);
		this.scriptXtraTabPage.Name = "scriptXtraTabPage";
		this.scriptXtraTabPage.Size = new System.Drawing.Size(951, 308);
		this.scriptXtraTabPage.Text = "Script";
		this.scriptRichEditControl.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
		this.scriptRichEditControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.scriptRichEditControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.scriptRichEditControl.IsHighlighted = false;
		this.scriptRichEditControl.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
		this.scriptRichEditControl.Location = new System.Drawing.Point(0, 0);
		this.scriptRichEditControl.Margin = new System.Windows.Forms.Padding(0);
		this.scriptRichEditControl.Name = "scriptRichEditControl";
		this.scriptRichEditControl.OccurrencesCount = 0;
		this.scriptRichEditControl.Options.HorizontalScrollbar.Visibility = DevExpress.XtraRichEdit.RichEditScrollbarVisibility.Hidden;
		this.scriptRichEditControl.OriginalHtmlText = null;
		this.scriptRichEditControl.Size = new System.Drawing.Size(951, 308);
		this.scriptRichEditControl.TabIndex = 3;
		this.scriptRichEditControl.Views.SimpleView.Padding = new System.Windows.Forms.Padding(4, 4, 4, 0);
		this.scriptRichEditControl.TextChanged += new System.EventHandler(ScriptRichEditControl_TextChanged);
		this.labelControl1.Location = new System.Drawing.Point(12, 137);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(79, 20);
		this.labelControl1.StyleController = this.layoutControl1;
		this.labelControl1.TabIndex = 21;
		this.labelControl1.Text = "Type:";
		this.typeGridLookUpEdit.Location = new System.Drawing.Point(95, 137);
		this.typeGridLookUpEdit.MaximumSize = new System.Drawing.Size(248, 20);
		this.typeGridLookUpEdit.MenuManager = this.ribbonControl;
		this.typeGridLookUpEdit.MinimumSize = new System.Drawing.Size(251, 20);
		this.typeGridLookUpEdit.Name = "typeGridLookUpEdit";
		this.typeGridLookUpEdit.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.typeGridLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.typeGridLookUpEdit.Properties.DataSource = this.subtypeBindingSource;
		this.typeGridLookUpEdit.Properties.DisplayMember = "DisplayName";
		this.typeGridLookUpEdit.Properties.ImmediatePopup = true;
		this.typeGridLookUpEdit.Properties.NullText = "";
		this.typeGridLookUpEdit.Properties.PopupFilterMode = DevExpress.XtraEditors.PopupFilterMode.Contains;
		this.typeGridLookUpEdit.Properties.PopupView = this.typeGridLookUpEditView;
		this.typeGridLookUpEdit.Properties.ShowFooter = false;
		this.typeGridLookUpEdit.Properties.ValueMember = "SubType";
		this.typeGridLookUpEdit.Size = new System.Drawing.Size(251, 20);
		this.typeGridLookUpEdit.StyleController = this.layoutControl1;
		this.typeGridLookUpEdit.TabIndex = 20;
		this.typeGridLookUpEdit.EditValueChanged += new System.EventHandler(typeGridLookUpEdit_EditValueChanged);
		this.ribbonControl.AllowMinimizeRibbon = false;
		this.ribbonControl.Dock = System.Windows.Forms.DockStyle.None;
		this.ribbonControl.ExpandCollapseItem.Id = 0;
		this.ribbonControl.Items.AddRange(new DevExpress.XtraBars.BarItem[11]
		{
			this.ribbonControl.ExpandCollapseItem,
			this.ribbonControl.SearchEditItem,
			this.barButtonItem1,
			this.addBarButtonItem,
			this.removeBarButtonItem,
			this.moveUpBarButtonItem,
			this.defaultSortBarButtonItem,
			this.addBulkColumnsBarButtonItem,
			this.moveDownBarButtonItem,
			this.moveToBottomBarButtonItem,
			this.moveToTopBarButtonItem
		});
		this.ribbonControl.Location = new System.Drawing.Point(0, 0);
		this.ribbonControl.Margin = new System.Windows.Forms.Padding(0);
		this.ribbonControl.MaxItemId = 12;
		this.ribbonControl.Name = "ribbonControl";
		this.ribbonControl.OptionsPageCategories.ShowCaptions = false;
		this.ribbonControl.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[1] { this.ribbonPage });
		this.ribbonControl.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowDisplayOptionsMenuButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowExpandCollapseButton = DevExpress.Utils.DefaultBoolean.False;
		this.ribbonControl.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
		this.ribbonControl.ShowToolbarCustomizeItem = false;
		this.ribbonControl.Size = new System.Drawing.Size(957, 102);
		this.ribbonControl.Toolbar.ShowCustomizeItem = false;
		this.ribbonControl.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
		this.barButtonItem1.Caption = "barButtonItem1";
		this.barButtonItem1.CategoryGuid = new System.Guid("6ffddb2b-9015-4d97-a4c1-91613e0ef537");
		this.barButtonItem1.Id = 1;
		this.barButtonItem1.Name = "barButtonItem1";
		this.addBarButtonItem.Caption = "Add";
		this.addBarButtonItem.Id = 2;
		this.addBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.about_16;
		this.addBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.add_32;
		this.addBarButtonItem.Name = "addBarButtonItem";
		this.addBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addBarButtonItem_ItemClick);
		this.removeBarButtonItem.Caption = "Remove";
		this.removeBarButtonItem.Id = 3;
		this.removeBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.delete_32;
		this.removeBarButtonItem.Name = "removeBarButtonItem";
		this.removeBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(removeBarButtonItem_ItemClick);
		this.moveUpBarButtonItem.Caption = "Move up";
		this.moveUpBarButtonItem.Id = 4;
		this.moveUpBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_up_16;
		this.moveUpBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.arrow_up_32;
		this.moveUpBarButtonItem.Name = "moveUpBarButtonItem";
		this.moveUpBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(moveUpBarButtonItem_ItemClick);
		this.defaultSortBarButtonItem.Caption = "Default sort";
		this.defaultSortBarButtonItem.Id = 7;
		this.defaultSortBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.sort_num_asc_16;
		this.defaultSortBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.sort_num_asc_32;
		this.defaultSortBarButtonItem.Name = "defaultSortBarButtonItem";
		this.defaultSortBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(defaultSortBarButtonItem_ItemClick);
		this.addBulkColumnsBarButtonItem.Caption = "Bulk add columns";
		this.addBulkColumnsBarButtonItem.Id = 8;
		this.addBulkColumnsBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.bulk_add_columns_16;
		this.addBulkColumnsBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.bulk_add_columns_32;
		this.addBulkColumnsBarButtonItem.Name = "addBulkColumnsBarButtonItem";
		this.addBulkColumnsBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(addBulkColumnsBarButtonItem_ItemClick);
		this.moveDownBarButtonItem.Caption = "Move down";
		this.moveDownBarButtonItem.Id = 9;
		this.moveDownBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_down_16;
		this.moveDownBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.arrow_down_32;
		this.moveDownBarButtonItem.Name = "moveDownBarButtonItem";
		this.moveDownBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(moveDownBarButtonItem_ItemClick);
		this.moveToBottomBarButtonItem.Caption = "Move to bottom";
		this.moveToBottomBarButtonItem.Id = 10;
		this.moveToBottomBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_bottom_16;
		this.moveToBottomBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.arrow_bottom_32;
		this.moveToBottomBarButtonItem.Name = "moveToBottomBarButtonItem";
		this.moveToBottomBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(moveToBottomBarButtonItem_ItemClick);
		this.moveToTopBarButtonItem.Caption = "Move to top";
		this.moveToTopBarButtonItem.Id = 11;
		this.moveToTopBarButtonItem.ImageOptions.Image = Dataedo.App.Properties.Resources.arrow_top_16;
		this.moveToTopBarButtonItem.ImageOptions.LargeImage = Dataedo.App.Properties.Resources.arrow_top_32;
		this.moveToTopBarButtonItem.Name = "moveToTopBarButtonItem";
		this.moveToTopBarButtonItem.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(moveToTopBarButtonItem_ItemClick);
		this.ribbonPage.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[3] { this.manipulateRibbonPageGroup, this.sortRibbonPageGroup, this.bulkColumnsRibbonPageGroup });
		this.ribbonPage.Name = "ribbonPage";
		this.ribbonPage.Text = "ribbonPage";
		this.manipulateRibbonPageGroup.AllowTextClipping = false;
		this.manipulateRibbonPageGroup.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.manipulateRibbonPageGroup.ItemLinks.Add(this.addBarButtonItem);
		this.manipulateRibbonPageGroup.ItemLinks.Add(this.removeBarButtonItem);
		this.manipulateRibbonPageGroup.Name = "manipulateRibbonPageGroup";
		this.manipulateRibbonPageGroup.Text = "Columns";
		this.sortRibbonPageGroup.AllowTextClipping = false;
		this.sortRibbonPageGroup.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.sortRibbonPageGroup.ItemLinks.Add(this.moveUpBarButtonItem);
		this.sortRibbonPageGroup.ItemLinks.Add(this.moveDownBarButtonItem);
		this.sortRibbonPageGroup.ItemLinks.Add(this.moveToTopBarButtonItem);
		this.sortRibbonPageGroup.ItemLinks.Add(this.moveToBottomBarButtonItem);
		this.sortRibbonPageGroup.ItemLinks.Add(this.defaultSortBarButtonItem);
		this.sortRibbonPageGroup.Name = "sortRibbonPageGroup";
		this.sortRibbonPageGroup.Text = "Order";
		this.bulkColumnsRibbonPageGroup.AllowTextClipping = false;
		this.bulkColumnsRibbonPageGroup.CaptionButtonVisible = DevExpress.Utils.DefaultBoolean.False;
		this.bulkColumnsRibbonPageGroup.ItemLinks.Add(this.addBulkColumnsBarButtonItem);
		this.bulkColumnsRibbonPageGroup.Name = "bulkColumnsRibbonPageGroup";
		this.bulkColumnsRibbonPageGroup.Text = "Bulk";
		this.subtypeBindingSource.DataSource = typeof(Dataedo.App.Tools.Subtype);
		this.typeGridLookUpEditView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.displayNameGridColumn, this.iconGridColumn });
		this.typeGridLookUpEditView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.typeGridLookUpEditView.Name = "typeGridLookUpEditView";
		this.typeGridLookUpEditView.OptionsBehavior.AutoPopulateColumns = false;
		this.typeGridLookUpEditView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.typeGridLookUpEditView.OptionsView.ShowColumnHeaders = false;
		this.typeGridLookUpEditView.OptionsView.ShowGroupPanel = false;
		this.typeGridLookUpEditView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.typeGridLookUpEditView.OptionsView.ShowIndicator = false;
		this.typeGridLookUpEditView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.displayNameGridColumn.FieldName = "DisplayName";
		this.displayNameGridColumn.Name = "displayNameGridColumn";
		this.displayNameGridColumn.Visible = true;
		this.displayNameGridColumn.VisibleIndex = 1;
		this.displayNameGridColumn.Width = 380;
		this.iconGridColumn.FieldName = "Image";
		this.iconGridColumn.MaxWidth = 20;
		this.iconGridColumn.Name = "iconGridColumn";
		this.iconGridColumn.Visible = true;
		this.iconGridColumn.VisibleIndex = 0;
		this.iconGridColumn.Width = 20;
		this.titleLabelControl.Location = new System.Drawing.Point(12, 209);
		this.titleLabelControl.MaximumSize = new System.Drawing.Size(79, 20);
		this.titleLabelControl.MinimumSize = new System.Drawing.Size(79, 20);
		this.titleLabelControl.Name = "titleLabelControl";
		this.titleLabelControl.Size = new System.Drawing.Size(79, 20);
		this.titleLabelControl.StyleController = this.layoutControl1;
		this.titleLabelControl.TabIndex = 5;
		this.titleLabelControl.Text = "Title:";
		this.titleTextEdit.Location = new System.Drawing.Point(95, 209);
		this.titleTextEdit.MaximumSize = new System.Drawing.Size(251, 20);
		this.titleTextEdit.MinimumSize = new System.Drawing.Size(251, 20);
		this.titleTextEdit.Name = "titleTextEdit";
		this.titleTextEdit.Size = new System.Drawing.Size(251, 20);
		this.titleTextEdit.StyleController = this.layoutControl1;
		this.titleTextEdit.TabIndex = 19;
		this.titleTextEdit.TextChanged += new System.EventHandler(TextEdit_TextChanged);
		this.documentationTitleLabelControl.Appearance.Options.UseTextOptions = true;
		this.documentationTitleLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.documentationTitleLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.documentationTitleLabelControl.Location = new System.Drawing.Point(95, 113);
		this.documentationTitleLabelControl.MaximumSize = new System.Drawing.Size(248, 20);
		this.documentationTitleLabelControl.MinimumSize = new System.Drawing.Size(248, 20);
		this.documentationTitleLabelControl.Name = "documentationTitleLabelControl";
		this.documentationTitleLabelControl.Size = new System.Drawing.Size(248, 20);
		this.documentationTitleLabelControl.StyleController = this.layoutControl1;
		this.documentationTitleLabelControl.TabIndex = 18;
		this.documentationLabelControl.Appearance.Options.UseTextOptions = true;
		this.documentationLabelControl.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.documentationLabelControl.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.documentationLabelControl.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
		this.documentationLabelControl.Location = new System.Drawing.Point(12, 113);
		this.documentationLabelControl.MaximumSize = new System.Drawing.Size(79, 20);
		this.documentationLabelControl.MinimumSize = new System.Drawing.Size(79, 20);
		this.documentationLabelControl.Name = "documentationLabelControl";
		this.documentationLabelControl.Size = new System.Drawing.Size(79, 20);
		this.documentationLabelControl.StyleController = this.layoutControl1;
		this.documentationLabelControl.TabIndex = 17;
		this.documentationLabelControl.Text = "Documentation: ";
		this.nameTextEdit.Location = new System.Drawing.Point(95, 185);
		this.nameTextEdit.Name = "nameTextEdit";
		this.nameTextEdit.Size = new System.Drawing.Size(251, 20);
		this.nameTextEdit.StyleController = this.layoutControl1;
		this.nameTextEdit.TabIndex = 14;
		this.nameTextEdit.EditValueChanged += new System.EventHandler(TextEdit_TextChanged);
		this.nameTextEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(TextEdit_EditValueChanging);
		this.schemaTextEdit.Location = new System.Drawing.Point(95, 161);
		this.schemaTextEdit.MinimumSize = new System.Drawing.Size(248, 20);
		this.schemaTextEdit.Name = "schemaTextEdit";
		this.schemaTextEdit.Size = new System.Drawing.Size(251, 20);
		this.schemaTextEdit.StyleController = this.layoutControl1;
		this.schemaTextEdit.TabIndex = 13;
		this.schemaTextEdit.EditValueChanged += new System.EventHandler(TextEdit_TextChanged);
		this.schemaTextEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(TextEdit_EditValueChanging);
		this.schemaTextEdit.TextChanged += new System.EventHandler(TextEdit_TextChanged);
		this.nameLabelControl.Location = new System.Drawing.Point(12, 185);
		this.nameLabelControl.Name = "nameLabelControl";
		this.nameLabelControl.Size = new System.Drawing.Size(79, 20);
		this.nameLabelControl.StyleController = this.layoutControl1;
		this.nameLabelControl.TabIndex = 12;
		this.nameLabelControl.Text = "Name:";
		this.schemaLabelControl.Location = new System.Drawing.Point(12, 161);
		this.schemaLabelControl.Name = "schemaLabelControl";
		this.schemaLabelControl.Size = new System.Drawing.Size(79, 20);
		this.schemaLabelControl.StyleController = this.layoutControl1;
		this.schemaLabelControl.TabIndex = 11;
		this.schemaLabelControl.Text = "Schema:";
		this.cancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelSimpleButton.Location = new System.Drawing.Point(860, 616);
		this.cancelSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.cancelSimpleButton.StyleController = this.layoutControl1;
		this.cancelSimpleButton.TabIndex = 10;
		this.cancelSimpleButton.Text = "Cancel";
		this.cancelSimpleButton.Click += new System.EventHandler(cancelSimpleButton_Click);
		this.saveSimpleButton.DialogResult = System.Windows.Forms.DialogResult.OK;
		this.saveSimpleButton.Location = new System.Drawing.Point(758, 616);
		this.saveSimpleButton.MaximumSize = new System.Drawing.Size(85, 22);
		this.saveSimpleButton.MinimumSize = new System.Drawing.Size(85, 22);
		this.saveSimpleButton.Name = "saveSimpleButton";
		this.saveSimpleButton.Size = new System.Drawing.Size(85, 22);
		this.saveSimpleButton.StyleController = this.layoutControl1;
		this.saveSimpleButton.TabIndex = 9;
		this.saveSimpleButton.Text = "Save";
		this.saveSimpleButton.Click += new System.EventHandler(saveSimpleButton_Click);
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[25]
		{
			this.emptySpaceItem5, this.layoutControlItem6, this.layoutControlItem7, this.emptySpaceItem6, this.schemaLabelLayoutControlItem, this.layoutControlItem9, this.emptySpaceItem8, this.schemaLayoutControlItem, this.layoutControlItem11, this.layoutControlItem14,
			this.documentationTitleLayoutContorlItem, this.ribbonLayoutControlItem, this.emptySpaceItem1, this.emptySpaceItem2, this.layoutControlItem15, this.layoutControlItem16, this.layoutControlItem2, this.typeGridLayoutControlItem, this.emptySpaceItem4, this.emptySpaceItem7,
			this.emptySpaceItem3, this.emptySpaceItem9, this.xtraTabControlLayoutControlItem, this.locationLabelLayoutControlItem, this.locationLayoutControlItem
		});
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.layoutControlGroup1.Size = new System.Drawing.Size(957, 649);
		this.layoutControlGroup1.TextVisible = false;
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.Location = new System.Drawing.Point(0, 614);
		this.emptySpaceItem5.MinSize = new System.Drawing.Size(50, 25);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(756, 26);
		this.emptySpaceItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.Control = this.saveSimpleButton;
		this.layoutControlItem6.Location = new System.Drawing.Point(756, 614);
		this.layoutControlItem6.MaxSize = new System.Drawing.Size(89, 26);
		this.layoutControlItem6.MinSize = new System.Drawing.Size(89, 26);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.Size = new System.Drawing.Size(89, 26);
		this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.TextVisible = false;
		this.layoutControlItem7.Control = this.cancelSimpleButton;
		this.layoutControlItem7.Location = new System.Drawing.Point(858, 614);
		this.layoutControlItem7.MaxSize = new System.Drawing.Size(89, 26);
		this.layoutControlItem7.MinSize = new System.Drawing.Size(89, 26);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Size = new System.Drawing.Size(89, 26);
		this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.TextVisible = false;
		this.emptySpaceItem6.AllowHotTrack = false;
		this.emptySpaceItem6.Location = new System.Drawing.Point(845, 614);
		this.emptySpaceItem6.MaxSize = new System.Drawing.Size(13, 26);
		this.emptySpaceItem6.MinSize = new System.Drawing.Size(13, 26);
		this.emptySpaceItem6.Name = "emptySpaceItem6";
		this.emptySpaceItem6.Size = new System.Drawing.Size(13, 26);
		this.emptySpaceItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem6.TextSize = new System.Drawing.Size(0, 0);
		this.schemaLabelLayoutControlItem.Control = this.schemaLabelControl;
		this.schemaLabelLayoutControlItem.Location = new System.Drawing.Point(10, 159);
		this.schemaLabelLayoutControlItem.MaxSize = new System.Drawing.Size(83, 24);
		this.schemaLabelLayoutControlItem.MinSize = new System.Drawing.Size(83, 24);
		this.schemaLabelLayoutControlItem.Name = "schemaLabelLayoutControlItem";
		this.schemaLabelLayoutControlItem.Size = new System.Drawing.Size(83, 24);
		this.schemaLabelLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.schemaLabelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.schemaLabelLayoutControlItem.TextVisible = false;
		this.layoutControlItem9.Control = this.nameLabelControl;
		this.layoutControlItem9.Location = new System.Drawing.Point(10, 183);
		this.layoutControlItem9.MaxSize = new System.Drawing.Size(83, 24);
		this.layoutControlItem9.MinSize = new System.Drawing.Size(83, 24);
		this.layoutControlItem9.Name = "layoutControlItem9";
		this.layoutControlItem9.Size = new System.Drawing.Size(83, 24);
		this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem9.TextVisible = false;
		this.emptySpaceItem8.AllowHotTrack = false;
		this.emptySpaceItem8.Location = new System.Drawing.Point(348, 111);
		this.emptySpaceItem8.Name = "emptySpaceItem8";
		this.emptySpaceItem8.Size = new System.Drawing.Size(609, 120);
		this.emptySpaceItem8.TextSize = new System.Drawing.Size(0, 0);
		this.schemaLayoutControlItem.Control = this.schemaTextEdit;
		this.schemaLayoutControlItem.Location = new System.Drawing.Point(93, 159);
		this.schemaLayoutControlItem.MaxSize = new System.Drawing.Size(255, 24);
		this.schemaLayoutControlItem.MinSize = new System.Drawing.Size(255, 24);
		this.schemaLayoutControlItem.Name = "schemaLayoutControlItem";
		this.schemaLayoutControlItem.Size = new System.Drawing.Size(255, 24);
		this.schemaLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.schemaLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.schemaLayoutControlItem.TextVisible = false;
		this.layoutControlItem11.Control = this.nameTextEdit;
		this.layoutControlItem11.Location = new System.Drawing.Point(93, 183);
		this.layoutControlItem11.MaxSize = new System.Drawing.Size(255, 34);
		this.layoutControlItem11.MinSize = new System.Drawing.Size(252, 24);
		this.layoutControlItem11.Name = "layoutControlItem11";
		this.layoutControlItem11.Size = new System.Drawing.Size(255, 24);
		this.layoutControlItem11.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem11.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem11.TextVisible = false;
		this.layoutControlItem14.Control = this.documentationLabelControl;
		this.layoutControlItem14.Location = new System.Drawing.Point(10, 111);
		this.layoutControlItem14.Name = "layoutControlItem14";
		this.layoutControlItem14.Size = new System.Drawing.Size(83, 24);
		this.layoutControlItem14.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem14.TextVisible = false;
		this.documentationTitleLayoutContorlItem.Control = this.documentationTitleLabelControl;
		this.documentationTitleLayoutContorlItem.Location = new System.Drawing.Point(93, 111);
		this.documentationTitleLayoutContorlItem.MaxSize = new System.Drawing.Size(255, 24);
		this.documentationTitleLayoutContorlItem.MinSize = new System.Drawing.Size(255, 24);
		this.documentationTitleLayoutContorlItem.Name = "documentationTitleLayoutContorlItem";
		this.documentationTitleLayoutContorlItem.Size = new System.Drawing.Size(255, 24);
		this.documentationTitleLayoutContorlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.documentationTitleLayoutContorlItem.TextSize = new System.Drawing.Size(0, 0);
		this.documentationTitleLayoutContorlItem.TextVisible = false;
		this.ribbonLayoutControlItem.Control = this.ribbonControl;
		this.ribbonLayoutControlItem.CustomizationFormText = "ribbonLayoutControlItem";
		this.ribbonLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.ribbonLayoutControlItem.MaxSize = new System.Drawing.Size(0, 102);
		this.ribbonLayoutControlItem.MinSize = new System.Drawing.Size(1, 102);
		this.ribbonLayoutControlItem.Name = "ribbonLayoutControlItem";
		this.ribbonLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
		this.ribbonLayoutControlItem.Size = new System.Drawing.Size(957, 102);
		this.ribbonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.ribbonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.ribbonLayoutControlItem.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 111);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(10, 82);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 82);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(10, 144);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(947, 614);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(10, 26);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem15.Control = this.titleTextEdit;
		this.layoutControlItem15.Location = new System.Drawing.Point(93, 207);
		this.layoutControlItem15.MaxSize = new System.Drawing.Size(255, 24);
		this.layoutControlItem15.MinSize = new System.Drawing.Size(255, 24);
		this.layoutControlItem15.Name = "layoutControlItem15";
		this.layoutControlItem15.Size = new System.Drawing.Size(255, 24);
		this.layoutControlItem15.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem15.TextVisible = false;
		this.layoutControlItem16.Control = this.titleLabelControl;
		this.layoutControlItem16.Location = new System.Drawing.Point(10, 207);
		this.layoutControlItem16.MaxSize = new System.Drawing.Size(83, 24);
		this.layoutControlItem16.MinSize = new System.Drawing.Size(83, 24);
		this.layoutControlItem16.Name = "layoutControlItem16";
		this.layoutControlItem16.Size = new System.Drawing.Size(83, 24);
		this.layoutControlItem16.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem16.TextVisible = false;
		this.layoutControlItem2.Control = this.labelControl1;
		this.layoutControlItem2.Location = new System.Drawing.Point(10, 135);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(83, 24);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(83, 24);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(83, 24);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.typeGridLayoutControlItem.Control = this.typeGridLookUpEdit;
		this.typeGridLayoutControlItem.Location = new System.Drawing.Point(93, 135);
		this.typeGridLayoutControlItem.MaxSize = new System.Drawing.Size(255, 24);
		this.typeGridLayoutControlItem.MinSize = new System.Drawing.Size(255, 24);
		this.typeGridLayoutControlItem.Name = "typeGridLayoutControlItem";
		this.typeGridLayoutControlItem.Size = new System.Drawing.Size(255, 24);
		this.typeGridLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.typeGridLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.typeGridLayoutControlItem.TextVisible = false;
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem4.Location = new System.Drawing.Point(0, 640);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(957, 9);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.Text = "emptySpaceItem1";
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem7.AllowHotTrack = false;
		this.emptySpaceItem7.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem7.Location = new System.Drawing.Point(0, 255);
		this.emptySpaceItem7.MaxSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem7.MinSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem7.Name = "emptySpaceItem7";
		this.emptySpaceItem7.Size = new System.Drawing.Size(957, 9);
		this.emptySpaceItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem7.Text = "emptySpaceItem1";
		this.emptySpaceItem7.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem3.Location = new System.Drawing.Point(0, 605);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(957, 9);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.Text = "emptySpaceItem1";
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem9.AllowHotTrack = false;
		this.emptySpaceItem9.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem9.Location = new System.Drawing.Point(0, 102);
		this.emptySpaceItem9.MaxSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem9.MinSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem9.Name = "emptySpaceItem9";
		this.emptySpaceItem9.Size = new System.Drawing.Size(957, 9);
		this.emptySpaceItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem9.Text = "emptySpaceItem1";
		this.emptySpaceItem9.TextSize = new System.Drawing.Size(0, 0);
		this.xtraTabControlLayoutControlItem.Control = this.xtraTabControl;
		this.xtraTabControlLayoutControlItem.Location = new System.Drawing.Point(0, 264);
		this.xtraTabControlLayoutControlItem.Name = "xtraTabControlLayoutControlItem";
		this.xtraTabControlLayoutControlItem.Size = new System.Drawing.Size(957, 341);
		this.xtraTabControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.xtraTabControlLayoutControlItem.TextVisible = false;
		this.locationLabelLayoutControlItem.Control = this.locationLabelControl;
		this.locationLabelLayoutControlItem.Location = new System.Drawing.Point(10, 231);
		this.locationLabelLayoutControlItem.Name = "locationLabelLayoutControlItem";
		this.locationLabelLayoutControlItem.Size = new System.Drawing.Size(83, 24);
		this.locationLabelLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.locationLabelLayoutControlItem.TextVisible = false;
		this.locationLayoutControlItem.Control = this.locationTextEdit;
		this.locationLayoutControlItem.Location = new System.Drawing.Point(93, 231);
		this.locationLayoutControlItem.Name = "locationLayoutControlItem";
		this.locationLayoutControlItem.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 10, 2, 2);
		this.locationLayoutControlItem.Size = new System.Drawing.Size(864, 24);
		this.locationLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.locationLayoutControlItem.TextVisible = false;
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[1] { this.ribbonPageGroup1 });
		this.ribbonPage1.Name = "ribbonPage1";
		this.ribbonPage1.Text = "ribbonPage1";
		this.ribbonPageGroup1.Name = "ribbonPageGroup1";
		this.defaultToolTipController.DefaultController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.defaultToolTipController.SetAllowHtmlText(this, DevExpress.Utils.DefaultBoolean.Default);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(957, 649);
		base.Controls.Add(this.layoutControl1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("DesignTableForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		this.MinimumSize = new System.Drawing.Size(600, 450);
		base.Name = "DesignTableForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Add table";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ManualTableForm_FormClosing);
		base.Load += new System.EventHandler(DesignTableForm_Load);
		base.Shown += new System.EventHandler(DesignTableForm_Shown);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		this.layoutControl1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.locationTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl).EndInit();
		this.xtraTabControl.ResumeLayout(false);
		this.columnsXtraTabPage.ResumeLayout(false);
		this.schemaXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.schemaMemoEdit.Properties).EndInit();
		this.scriptXtraTabPage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.typeGridLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ribbonControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.subtypeBindingSource).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeGridLookUpEditView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.schemaTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.schemaLabelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.schemaLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem14).EndInit();
		((System.ComponentModel.ISupportInitialize)this.documentationTitleLayoutContorlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ribbonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem15).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeGridLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.locationLabelLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.locationLayoutControlItem).EndInit();
		base.ResumeLayout(false);
	}
}
