using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.ImportDescriptions.Tools.Fields;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools.Exceptions;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls;
using Dataedo.App.UserControls.WindowControls;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.CustomFields;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.ImportDescriptions;

public class ChooseImportFieldsForm : BaseXtraForm
{
	private Form parentForm;

	private MetadataEditorUserControl mainControl;

	private int databaseId;

	private SharedObjectTypeEnum.ObjectType objectType;

	private List<FieldDefinition> fieldDefinitions;

	private IContainer components;

	private NonCustomizableLayoutControl fieldsLayoutControl;

	private SimpleButton defineCustomFieldsSimpleButton;

	private GridControl fieldsGridControl;

	private GridColumn isSelectedGridColumn;

	private RepositoryItemCheckEdit isSelectedRepositoryItemCheckEdit;

	private GridColumn titleCustomFieldsGridColumn;

	private LayoutControlGroup layoutControlGroup;

	private LayoutControlItem fieldsGridControlLayoutControlItem;

	private EmptySpaceItem emptySpaceItem3;

	private LayoutControlItem defineCustomFieldLayoutControlItem;

	private EmptySpaceItem separatorEmptySpaceItem;

	private SimpleButton okSimpleButton;

	private LayoutControlItem okSimpleButtonLayoutControlItem;

	private SimpleButton cancelSimpleButton;

	private LayoutControlItem cancelSimpleButtonLayoutControlItem;

	private EmptySpaceItem beetwenButtonsEmptySpaceItem;

	private EmptySpaceItem bottomAboveButtonsEmptySpaceItem;

	private EmptySpaceItem leftButtonsEmptySpaceItem;

	private CustomGridUserControl fieldsGridView;

	private HyperLinkEdit allHyperLinkEdit;

	private LayoutControlItem allHyperLinkEditLayoutControlItem;

	private HyperLinkEdit noneHyperLinkEdit;

	private LayoutControlItem noneHyperLinkEditLayoutControlItem;

	private EmptySpaceItem allNoneEmptySpaceItem;

	public bool IsClosingSource { get; private set; } = true;


	public ChooseImportFieldsForm(Form parentForm, MetadataEditorUserControl mainControl, int databaseId, SharedObjectTypeEnum.ObjectType objectType)
	{
		this.parentForm = parentForm;
		this.mainControl = mainControl;
		this.databaseId = databaseId;
		this.objectType = objectType;
		InitializeComponent();
		RefreshFieldsGrid();
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (keyData == Keys.Escape)
		{
			Close();
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void ChooseImportFieldsForm_Shown(object sender, EventArgs e)
	{
		parentForm.Visible = false;
	}

	private void defineCustomFieldsSimpleButton_Click(object sender, EventArgs e)
	{
		try
		{
			if (mainControl.ContinueAfterPossibleChanges())
			{
				mainControl.EditCustomFields();
				RefreshFieldsGrid();
			}
		}
		catch (Exception exception)
		{
			GeneralExceptionHandling.Handle(exception);
		}
	}

	private void RefreshFieldsGrid()
	{
		mainControl.CustomFieldsSupport.LoadCustomFields(Licence.GetCustomFieldsLimit(), loadDefinitionValues: true);
		CustomFieldFieldDefinition[] collection = (from x in mainControl.CustomFieldsSupport.Fields
			where x.IsFieldVisible(objectType, visibleIfSelectedOnly: false)
			select new CustomFieldFieldDefinition(x)).ToArray();
		fieldDefinitions = new List<FieldDefinition>
		{
			new TitleFieldDefinition(),
			new DescriptionFieldDefinition()
		};
		fieldDefinitions.AddRange(collection);
		fieldsGridControl.BeginUpdate();
		fieldDefinitions.ForEach(delegate(FieldDefinition x)
		{
			x.IsSelected = true;
		});
		fieldsGridControl.DataSource = fieldDefinitions;
		fieldsGridControl.EndUpdate();
	}

	private void okSimpleButton_Click(object sender, EventArgs e)
	{
		if (!fieldDefinitions.Any((FieldDefinition x) => x.IsSelected))
		{
			GeneralMessageBoxesHandling.Show("Choose at least one field to continue.", "Fields to import", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return;
		}
		IsClosingSource = false;
		DialogResult dialogResult = new DataToImportForm(this, databaseId, objectType, fieldDefinitions, mainControl.CustomFieldsSupport).ShowDialog(this);
		if (dialogResult != DialogResult.Cancel)
		{
			base.DialogResult = dialogResult;
		}
		else
		{
			parentForm.Close();
		}
	}

	private void cancelSimpleButton_Click(object sender, EventArgs e)
	{
		parentForm.Visible = true;
		base.DialogResult = DialogResult.No;
	}

	private void allHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		fieldsGridControl.BeginUpdate();
		fieldDefinitions.ForEach(delegate(FieldDefinition x)
		{
			x.IsSelected = true;
		});
		fieldsGridControl.EndUpdate();
	}

	private void noneHyperLinkEdit_OpenLink(object sender, OpenLinkEventArgs e)
	{
		fieldsGridControl.BeginUpdate();
		fieldDefinitions.ForEach(delegate(FieldDefinition x)
		{
			x.IsSelected = false;
		});
		fieldsGridControl.EndUpdate();
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
		this.fieldsLayoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.noneHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.allHyperLinkEdit = new DevExpress.XtraEditors.HyperLinkEdit();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.okSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.defineCustomFieldsSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.fieldsGridControl = new DevExpress.XtraGrid.GridControl();
		this.fieldsGridView = new Dataedo.App.UserControls.CustomGridUserControl();
		this.isSelectedGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.isSelectedRepositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.titleCustomFieldsGridColumn = new DevExpress.XtraGrid.Columns.GridColumn();
		this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.fieldsGridControlLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.defineCustomFieldLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.separatorEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.okSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.cancelSimpleButtonLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.beetwenButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.bottomAboveButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.leftButtonsEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.allHyperLinkEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.noneHyperLinkEditLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.allNoneEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		((System.ComponentModel.ISupportInitialize)this.fieldsLayoutControl).BeginInit();
		this.fieldsLayoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.noneHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allHyperLinkEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsGridControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsGridView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.isSelectedRepositoryItemCheckEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsGridControlLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.defineCustomFieldLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.okSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cancelSimpleButtonLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.beetwenButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bottomAboveButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.leftButtonsEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allHyperLinkEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.noneHyperLinkEditLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.allNoneEmptySpaceItem).BeginInit();
		base.SuspendLayout();
		this.fieldsLayoutControl.AllowCustomization = false;
		this.fieldsLayoutControl.BackColor = System.Drawing.Color.Transparent;
		this.fieldsLayoutControl.Controls.Add(this.noneHyperLinkEdit);
		this.fieldsLayoutControl.Controls.Add(this.allHyperLinkEdit);
		this.fieldsLayoutControl.Controls.Add(this.cancelSimpleButton);
		this.fieldsLayoutControl.Controls.Add(this.okSimpleButton);
		this.fieldsLayoutControl.Controls.Add(this.defineCustomFieldsSimpleButton);
		this.fieldsLayoutControl.Controls.Add(this.fieldsGridControl);
		this.fieldsLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.fieldsLayoutControl.Location = new System.Drawing.Point(0, 0);
		this.fieldsLayoutControl.Name = "fieldsLayoutControl";
		this.fieldsLayoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(3288, 509, 712, 449);
		this.fieldsLayoutControl.OptionsView.UseDefaultDragAndDropRendering = false;
		this.fieldsLayoutControl.Root = this.layoutControlGroup;
		this.fieldsLayoutControl.Size = new System.Drawing.Size(545, 318);
		this.fieldsLayoutControl.TabIndex = 33;
		this.fieldsLayoutControl.Text = "layoutControl2";
		this.noneHyperLinkEdit.EditValue = "None";
		this.noneHyperLinkEdit.Location = new System.Drawing.Point(38, 49);
		this.noneHyperLinkEdit.MaximumSize = new System.Drawing.Size(40, 0);
		this.noneHyperLinkEdit.MinimumSize = new System.Drawing.Size(40, 0);
		this.noneHyperLinkEdit.Name = "noneHyperLinkEdit";
		this.noneHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.noneHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.noneHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.noneHyperLinkEdit.Size = new System.Drawing.Size(40, 18);
		this.noneHyperLinkEdit.StyleController = this.fieldsLayoutControl;
		this.noneHyperLinkEdit.TabIndex = 36;
		this.noneHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(noneHyperLinkEdit_OpenLink);
		this.allHyperLinkEdit.EditValue = "All";
		this.allHyperLinkEdit.Location = new System.Drawing.Point(13, 49);
		this.allHyperLinkEdit.MaximumSize = new System.Drawing.Size(25, 0);
		this.allHyperLinkEdit.MinimumSize = new System.Drawing.Size(25, 0);
		this.allHyperLinkEdit.Name = "allHyperLinkEdit";
		this.allHyperLinkEdit.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.allHyperLinkEdit.Properties.Appearance.Options.UseBackColor = true;
		this.allHyperLinkEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.allHyperLinkEdit.Size = new System.Drawing.Size(25, 18);
		this.allHyperLinkEdit.StyleController = this.fieldsLayoutControl;
		this.allHyperLinkEdit.TabIndex = 35;
		this.allHyperLinkEdit.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(allHyperLinkEdit_OpenLink);
		this.cancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelSimpleButton.Location = new System.Drawing.Point(452, 283);
		this.cancelSimpleButton.MaximumSize = new System.Drawing.Size(0, 22);
		this.cancelSimpleButton.MinimumSize = new System.Drawing.Size(0, 22);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.cancelSimpleButton.StyleController = this.fieldsLayoutControl;
		this.cancelSimpleButton.TabIndex = 34;
		this.cancelSimpleButton.Text = "Cancel";
		this.cancelSimpleButton.Click += new System.EventHandler(cancelSimpleButton_Click);
		this.okSimpleButton.Location = new System.Drawing.Point(356, 283);
		this.okSimpleButton.MaximumSize = new System.Drawing.Size(0, 22);
		this.okSimpleButton.MinimumSize = new System.Drawing.Size(0, 22);
		this.okSimpleButton.Name = "okSimpleButton";
		this.okSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.okSimpleButton.StyleController = this.fieldsLayoutControl;
		this.okSimpleButton.TabIndex = 33;
		this.okSimpleButton.Text = "OK";
		this.okSimpleButton.Click += new System.EventHandler(okSimpleButton_Click);
		this.defineCustomFieldsSimpleButton.Location = new System.Drawing.Point(13, 13);
		this.defineCustomFieldsSimpleButton.Name = "defineCustomFieldsSimpleButton";
		this.defineCustomFieldsSimpleButton.Size = new System.Drawing.Size(107, 22);
		this.defineCustomFieldsSimpleButton.StyleController = this.fieldsLayoutControl;
		this.defineCustomFieldsSimpleButton.TabIndex = 32;
		this.defineCustomFieldsSimpleButton.Text = "Define custom fields";
		this.defineCustomFieldsSimpleButton.Click += new System.EventHandler(defineCustomFieldsSimpleButton_Click);
		this.fieldsGridControl.Location = new System.Drawing.Point(13, 73);
		this.fieldsGridControl.MainView = this.fieldsGridView;
		this.fieldsGridControl.Name = "fieldsGridControl";
		this.fieldsGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.isSelectedRepositoryItemCheckEdit });
		this.fieldsGridControl.Size = new System.Drawing.Size(519, 196);
		this.fieldsGridControl.TabIndex = 4;
		this.fieldsGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.fieldsGridView });
		this.fieldsGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.isSelectedGridColumn, this.titleCustomFieldsGridColumn });
		this.fieldsGridView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
		this.fieldsGridView.GridControl = this.fieldsGridControl;
		this.fieldsGridView.Name = "fieldsGridView";
		this.fieldsGridView.OptionsFilter.AllowFilterEditor = false;
		this.fieldsGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.fieldsGridView.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.fieldsGridView.OptionsSelection.UseIndicatorForSelection = false;
		this.fieldsGridView.OptionsView.ShowColumnHeaders = false;
		this.fieldsGridView.OptionsView.ShowGroupPanel = false;
		this.fieldsGridView.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
		this.fieldsGridView.OptionsView.ShowIndicator = false;
		this.fieldsGridView.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.fieldsGridView.RowHighlightingIsEnabled = true;
		this.isSelectedGridColumn.Caption = " ";
		this.isSelectedGridColumn.ColumnEdit = this.isSelectedRepositoryItemCheckEdit;
		this.isSelectedGridColumn.FieldName = "IsSelected";
		this.isSelectedGridColumn.MaxWidth = 20;
		this.isSelectedGridColumn.Name = "isSelectedGridColumn";
		this.isSelectedGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.isSelectedGridColumn.OptionsFilter.AllowFilter = false;
		this.isSelectedGridColumn.Visible = true;
		this.isSelectedGridColumn.VisibleIndex = 0;
		this.isSelectedGridColumn.Width = 20;
		this.isSelectedRepositoryItemCheckEdit.AutoHeight = false;
		this.isSelectedRepositoryItemCheckEdit.Name = "isSelectedRepositoryItemCheckEdit";
		this.titleCustomFieldsGridColumn.Caption = "Custom field";
		this.titleCustomFieldsGridColumn.FieldName = "DisplayName";
		this.titleCustomFieldsGridColumn.Name = "titleCustomFieldsGridColumn";
		this.titleCustomFieldsGridColumn.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.titleCustomFieldsGridColumn.OptionsFilter.AllowFilter = false;
		this.titleCustomFieldsGridColumn.Visible = true;
		this.titleCustomFieldsGridColumn.VisibleIndex = 1;
		this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup.GroupBordersVisible = false;
		this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[12]
		{
			this.fieldsGridControlLayoutControlItem, this.emptySpaceItem3, this.defineCustomFieldLayoutControlItem, this.separatorEmptySpaceItem, this.okSimpleButtonLayoutControlItem, this.cancelSimpleButtonLayoutControlItem, this.beetwenButtonsEmptySpaceItem, this.bottomAboveButtonsEmptySpaceItem, this.leftButtonsEmptySpaceItem, this.allHyperLinkEditLayoutControlItem,
			this.noneHyperLinkEditLayoutControlItem, this.allNoneEmptySpaceItem
		});
		this.layoutControlGroup.Name = "Root";
		this.layoutControlGroup.Padding = new DevExpress.XtraLayout.Utils.Padding(11, 11, 11, 11);
		this.layoutControlGroup.Size = new System.Drawing.Size(545, 318);
		this.layoutControlGroup.TextVisible = false;
		this.fieldsGridControlLayoutControlItem.Control = this.fieldsGridControl;
		this.fieldsGridControlLayoutControlItem.Location = new System.Drawing.Point(0, 60);
		this.fieldsGridControlLayoutControlItem.Name = "fieldsGridControlLayoutControlItem";
		this.fieldsGridControlLayoutControlItem.Size = new System.Drawing.Size(523, 200);
		this.fieldsGridControlLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.fieldsGridControlLayoutControlItem.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(111, 0);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(1, 1);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(412, 26);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.defineCustomFieldLayoutControlItem.Control = this.defineCustomFieldsSimpleButton;
		this.defineCustomFieldLayoutControlItem.Location = new System.Drawing.Point(0, 0);
		this.defineCustomFieldLayoutControlItem.MaxSize = new System.Drawing.Size(111, 26);
		this.defineCustomFieldLayoutControlItem.MinSize = new System.Drawing.Size(111, 26);
		this.defineCustomFieldLayoutControlItem.Name = "defineCustomFieldLayoutControlItem";
		this.defineCustomFieldLayoutControlItem.Size = new System.Drawing.Size(111, 26);
		this.defineCustomFieldLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.defineCustomFieldLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.defineCustomFieldLayoutControlItem.TextVisible = false;
		this.separatorEmptySpaceItem.AllowHotTrack = false;
		this.separatorEmptySpaceItem.Location = new System.Drawing.Point(0, 26);
		this.separatorEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 10);
		this.separatorEmptySpaceItem.MinSize = new System.Drawing.Size(104, 10);
		this.separatorEmptySpaceItem.Name = "separatorEmptySpaceItem";
		this.separatorEmptySpaceItem.Size = new System.Drawing.Size(523, 10);
		this.separatorEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.separatorEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.okSimpleButtonLayoutControlItem.Control = this.okSimpleButton;
		this.okSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(343, 270);
		this.okSimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(84, 26);
		this.okSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(84, 26);
		this.okSimpleButtonLayoutControlItem.Name = "okSimpleButtonLayoutControlItem";
		this.okSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(84, 26);
		this.okSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.okSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.okSimpleButtonLayoutControlItem.TextVisible = false;
		this.cancelSimpleButtonLayoutControlItem.Control = this.cancelSimpleButton;
		this.cancelSimpleButtonLayoutControlItem.Location = new System.Drawing.Point(439, 270);
		this.cancelSimpleButtonLayoutControlItem.MaxSize = new System.Drawing.Size(84, 26);
		this.cancelSimpleButtonLayoutControlItem.MinSize = new System.Drawing.Size(84, 26);
		this.cancelSimpleButtonLayoutControlItem.Name = "cancelSimpleButtonLayoutControlItem";
		this.cancelSimpleButtonLayoutControlItem.Size = new System.Drawing.Size(84, 26);
		this.cancelSimpleButtonLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.cancelSimpleButtonLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.cancelSimpleButtonLayoutControlItem.TextVisible = false;
		this.beetwenButtonsEmptySpaceItem.AllowHotTrack = false;
		this.beetwenButtonsEmptySpaceItem.Location = new System.Drawing.Point(427, 270);
		this.beetwenButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(12, 26);
		this.beetwenButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(12, 26);
		this.beetwenButtonsEmptySpaceItem.Name = "beetwenButtonsEmptySpaceItem";
		this.beetwenButtonsEmptySpaceItem.Size = new System.Drawing.Size(12, 26);
		this.beetwenButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.beetwenButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.bottomAboveButtonsEmptySpaceItem.AllowHotTrack = false;
		this.bottomAboveButtonsEmptySpaceItem.Location = new System.Drawing.Point(0, 260);
		this.bottomAboveButtonsEmptySpaceItem.MaxSize = new System.Drawing.Size(104, 10);
		this.bottomAboveButtonsEmptySpaceItem.MinSize = new System.Drawing.Size(104, 10);
		this.bottomAboveButtonsEmptySpaceItem.Name = "bottomAboveButtonsEmptySpaceItem";
		this.bottomAboveButtonsEmptySpaceItem.Size = new System.Drawing.Size(523, 10);
		this.bottomAboveButtonsEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.bottomAboveButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.leftButtonsEmptySpaceItem.AllowHotTrack = false;
		this.leftButtonsEmptySpaceItem.Location = new System.Drawing.Point(0, 270);
		this.leftButtonsEmptySpaceItem.Name = "leftButtonsEmptySpaceItem";
		this.leftButtonsEmptySpaceItem.Size = new System.Drawing.Size(343, 26);
		this.leftButtonsEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.allHyperLinkEditLayoutControlItem.Control = this.allHyperLinkEdit;
		this.allHyperLinkEditLayoutControlItem.Location = new System.Drawing.Point(0, 36);
		this.allHyperLinkEditLayoutControlItem.MaxSize = new System.Drawing.Size(25, 24);
		this.allHyperLinkEditLayoutControlItem.MinSize = new System.Drawing.Size(25, 24);
		this.allHyperLinkEditLayoutControlItem.Name = "allHyperLinkEditLayoutControlItem";
		this.allHyperLinkEditLayoutControlItem.Size = new System.Drawing.Size(25, 24);
		this.allHyperLinkEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.allHyperLinkEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.allHyperLinkEditLayoutControlItem.TextVisible = false;
		this.noneHyperLinkEditLayoutControlItem.Control = this.noneHyperLinkEdit;
		this.noneHyperLinkEditLayoutControlItem.Location = new System.Drawing.Point(25, 36);
		this.noneHyperLinkEditLayoutControlItem.MaxSize = new System.Drawing.Size(54, 24);
		this.noneHyperLinkEditLayoutControlItem.MinSize = new System.Drawing.Size(45, 24);
		this.noneHyperLinkEditLayoutControlItem.Name = "noneHyperLinkEditLayoutControlItem";
		this.noneHyperLinkEditLayoutControlItem.Size = new System.Drawing.Size(54, 24);
		this.noneHyperLinkEditLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.noneHyperLinkEditLayoutControlItem.TextSize = new System.Drawing.Size(0, 0);
		this.noneHyperLinkEditLayoutControlItem.TextVisible = false;
		this.allNoneEmptySpaceItem.AllowHotTrack = false;
		this.allNoneEmptySpaceItem.Location = new System.Drawing.Point(79, 36);
		this.allNoneEmptySpaceItem.Name = "allNoneEmptySpaceItem";
		this.allNoneEmptySpaceItem.Size = new System.Drawing.Size(444, 24);
		this.allNoneEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(545, 318);
		base.Controls.Add(this.fieldsLayoutControl);
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		this.MinimumSize = new System.Drawing.Size(250, 270);
		base.Name = "ChooseImportFieldsForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Choose fields to import to";
		base.Shown += new System.EventHandler(ChooseImportFieldsForm_Shown);
		((System.ComponentModel.ISupportInitialize)this.fieldsLayoutControl).EndInit();
		this.fieldsLayoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.noneHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allHyperLinkEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsGridControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsGridView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.isSelectedRepositoryItemCheckEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fieldsGridControlLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.defineCustomFieldLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.separatorEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.okSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cancelSimpleButtonLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.beetwenButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bottomAboveButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.leftButtonsEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allHyperLinkEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.noneHyperLinkEditLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.allNoneEmptySpaceItem).EndInit();
		base.ResumeLayout(false);
	}
}
