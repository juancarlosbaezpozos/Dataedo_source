using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Licences;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.CustomControls;
using Dataedo.CustomMessageBox;
using Dataedo.DataProcessing.Synchronize.Classes;
using Dataedo.Model.Data.CustomFields;
using Dataedo.Shared.Enums;
using Dataedo.Shared.Licenses.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class CustomFieldForm : BaseXtraForm
{
	private CustomFieldRow customField;

	private List<CustomFieldRow> otherExistingFields;

	private bool isEditMode;

	private IContainer components;

	private NonCustomizableLayoutControl layoutControl;

	private LayoutControlGroup layoutControlGroup1;

	private SimpleButton okSimpleButton;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem2;

	private MemoEdit descriptionMemoEdit;

	private TextEdit titleTextEdit;

	private LayoutControlItem layoutControlItem4;

	private LayoutControlItem layoutControlItem5;

	private SimpleButton cancelSimpleButton;

	private LayoutControlItem layoutControlItem1;

	private EmptySpaceItem emptySpaceItem3;

	private LookUpEdit typeLookUpEdit;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem definitionLayoutControlItem;

	private MemoEdit definitionMemoEdit;

	private TextEdit fieldTextEdit;

	private LayoutControlItem fieldLayoutControlItem;

	private EmptySpaceItem emptySpaceItem4;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem definitionEmptySpaceItem;

	private LookUpEdit classLookUpEdit;

	private LayoutControlItem layoutControlItem7;

	private ToolTipController toolTipController;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private CheckedComboBoxEdit visibilityCheckedComboBox;

	private LayoutControlItem visibilityCheckedComboBoxLayoutControlItem;

	public bool ValuesChanged { get; private set; }

	public CustomFieldForm(CustomFieldRow customField, IEnumerable<CustomFieldRow> otherExistingFields, bool editMode = true)
	{
		InitializeComponent();
		this.customField = customField;
		this.otherExistingFields = otherExistingFields.ToList();
		isEditMode = editMode;
		Text = (editMode ? ("Custom field: " + customField.Title) : "New custom field");
		LengthValidation.SetTitleOrNameLengthLimit(titleTextEdit);
		LengthValidation.SetCustomFieldLength(descriptionMemoEdit);
		titleTextEdit.Text = customField.Title;
		descriptionMemoEdit.Text = customField.Description;
		List<CustomFieldTypeEnum.CustomFieldTypeModel> customFieldsTypes = CustomFieldTypeEnum.GetCustomFieldsTypes();
		typeLookUpEdit.Properties.DataSource = customFieldsTypes;
		typeLookUpEdit.Properties.DropDownRows = customFieldsTypes.Count;
		typeLookUpEdit.EditValue = this.customField.Type;
		IEnumerable<CustomFieldClassRow> enumerable = from x in DB.CustomField.GetCustomFieldClasses()
			select new CustomFieldClassRow(x);
		classLookUpEdit.Properties.DataSource = enumerable;
		classLookUpEdit.Properties.DropDownRows = enumerable.Count();
		classLookUpEdit.EditValue = this.customField.CustomFieldClassId;
		definitionMemoEdit.EditValue = this.customField.Definition;
		fieldTextEdit.EditValue = customField.FieldName;
		SetDefinitionVisibility(typeLookUpEdit);
		SetCheckboxVisibilities();
		SetObjectActivityState("Term", Functionalities.HasFunctionality(FunctionalityEnum.Functionality.BusinessGlossary));
		ValuesChanged = false;
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		switch (keyData)
		{
		case Keys.S | Keys.Control:
			Save();
			break;
		case Keys.Escape:
			Close();
			break;
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void Save()
	{
		SetCustomFieldData();
	}

	private bool SetCustomFieldData()
	{
		string text = titleTextEdit.Text.Trim();
		if (string.IsNullOrWhiteSpace(text))
		{
			CustomMessageBoxForm.Show("Field title must not be empty.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
		}
		if (!IsFieldTitleUnique(text))
		{
			CustomMessageBoxForm.Show("Field title must be unique. Please enter another title.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
		}
		CustomFieldTypeEnum.CustomFieldType? type = typeLookUpEdit.EditValue as CustomFieldTypeEnum.CustomFieldType?;
		if (!type.HasValue)
		{
			CustomMessageBoxForm.Show("Custom field type must be set.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
		}
		customField.Title = text;
		customField.Description = descriptionMemoEdit.Text;
		if (!isEditMode)
		{
			customField.Code = DB.CustomField.GenerateCustomFieldCode(text, otherExistingFields);
		}
		SetCustomFieldVisibility();
		if (!(classLookUpEdit.GetSelectedDataRow() is CustomFieldClassRow customFieldClassRow))
		{
			CustomMessageBoxForm.Show("Custom field class must be set.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			return false;
		}
		customField.CustomFieldClassId = customFieldClassRow.CustomFieldClassId;
		customField.CustomFieldClassName = customFieldClassRow.Name;
		customField.Type = type;
		customField.Definition = ((!customField.IsDefinitionType) ? null : definitionMemoEdit.EditValue?.ToString());
		base.DialogResult = DialogResult.OK;
		return true;
	}

	private bool IsFieldTitleUnique(string fieldName)
	{
		string value = fieldName.ToLower();
		return !otherExistingFields.Select((CustomFieldRow x) => x.Title.ToLower()).Contains(value);
	}

	private void SetCustomFieldVisibility()
	{
		customField.TableVisibility = GetObjectVisibility("Structure");
		customField.ProcedureVisibility = GetObjectVisibility("Procedure");
		customField.ColumnVisibility = GetObjectVisibility("Column");
		customField.RelationVisibility = GetObjectVisibility("Relationship");
		customField.KeyVisibility = GetObjectVisibility("Key");
		customField.TriggerVisibility = GetObjectVisibility("Trigger");
		customField.ParameterVisibility = GetObjectVisibility("Parameter");
		customField.ModuleVisibility = GetObjectVisibility("SubjectArea");
		customField.DocumentationVisibility = GetObjectVisibility("Documentation");
		customField.TermVisibility = GetObjectVisibility("Term");
	}

	private bool GetObjectVisibility(string tag)
	{
		CheckedListBoxItem checkedListBoxItem = visibilityCheckedComboBox.Properties.Items.Cast<CheckedListBoxItem>().FirstOrDefault((CheckedListBoxItem x) => tag.Equals(x.Tag));
		if (checkedListBoxItem == null)
		{
			return true;
		}
		return checkedListBoxItem.CheckState == CheckState.Checked;
	}

	private void SetObjectVisibility(string tag, bool value)
	{
		CheckedListBoxItem checkedListBoxItem = visibilityCheckedComboBox.Properties.Items.Cast<CheckedListBoxItem>().FirstOrDefault((CheckedListBoxItem x) => tag.Equals(x.Tag));
		if (checkedListBoxItem != null)
		{
			checkedListBoxItem.CheckState = (value ? CheckState.Checked : CheckState.Unchecked);
		}
	}

	private void SetObjectActivityState(string tag, bool value)
	{
		CheckedListBoxItem checkedListBoxItem = visibilityCheckedComboBox.Properties.Items.Cast<CheckedListBoxItem>().FirstOrDefault((CheckedListBoxItem x) => tag.Equals(x.Tag));
		if (checkedListBoxItem != null)
		{
			checkedListBoxItem.Enabled = value;
		}
	}

	private void okSimpleButton_Click(object sender, EventArgs e)
	{
		Save();
	}

	private void cancelSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void textEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValuesChanged = (sender as BaseEdit).IsModified;
	}

	private void typeLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		SetDefinitionVisibility(sender as LookUpEdit);
		ValuesChanged = true;
	}

	private void definitionMemoEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValuesChanged = true;
	}

	private void CustomFieldForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (base.DialogResult == DialogResult.OK)
		{
			Save();
		}
		else
		{
			if (!ValuesChanged)
			{
				return;
			}
			DialogResult? dialogResult = GeneralMessageBoxesHandling.Show("Custom field has been changed, would you like to save these changes?", "Custom field has been changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
			if (dialogResult == DialogResult.Yes && !string.IsNullOrWhiteSpace(titleTextEdit.Text) && IsFieldTitleUnique(titleTextEdit.Text))
			{
				Save();
			}
			else if (dialogResult == DialogResult.Yes)
			{
				if (string.IsNullOrWhiteSpace(titleTextEdit.Text))
				{
					CustomMessageBoxForm.Show("Field title must not be empty.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
				}
				if (!IsFieldTitleUnique(titleTextEdit.Text))
				{
					CustomMessageBoxForm.Show("Field title must be unique. Please enter another title.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, null, 1, this);
				}
				base.DialogResult = DialogResult.Cancel;
				e.Cancel = true;
			}
			else if (dialogResult != DialogResult.No)
			{
				base.DialogResult = DialogResult.Cancel;
				e.Cancel = true;
			}
		}
	}

	private void SetDefinitionVisibility(LookUpEdit lookUpEdit)
	{
		CustomFieldTypeEnum.CustomFieldType? customFieldType = lookUpEdit?.EditValue as CustomFieldTypeEnum.CustomFieldType?;
		LayoutVisibility layoutVisibility3 = (definitionLayoutControlItem.Visibility = (definitionEmptySpaceItem.Visibility = ((!CustomFieldTypeEnum.IsDefinitionType(customFieldType)) ? LayoutVisibility.Never : LayoutVisibility.Always)));
	}

	private void classLookUpEdit_EditValueChanged(object sender, EventArgs e)
	{
		ValuesChanged = true;
	}

	private void visibilityCheckedComboBox_EditValueChanged(object sender, EventArgs e)
	{
		ValuesChanged = true;
	}

	private void SetCheckboxVisibilities()
	{
		if (isEditMode)
		{
			SetObjectVisibility("Structure", customField.TableVisibility);
			SetObjectVisibility("Procedure", customField.ProcedureVisibility);
			SetObjectVisibility("Column", customField.ColumnVisibility);
			SetObjectVisibility("Relationship", customField.RelationVisibility);
			SetObjectVisibility("Key", customField.KeyVisibility);
			SetObjectVisibility("Trigger", customField.TriggerVisibility);
			SetObjectVisibility("Parameter", customField.ParameterVisibility);
			SetObjectVisibility("SubjectArea", customField.ModuleVisibility);
			SetObjectVisibility("Documentation", customField.DocumentationVisibility);
			SetObjectVisibility("Term", customField.TermVisibility);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.CustomFieldForm));
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.visibilityCheckedComboBox = new DevExpress.XtraEditors.CheckedComboBoxEdit();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.classLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.toolTipController = new DevExpress.Utils.ToolTipController(this.components);
		this.definitionMemoEdit = new DevExpress.XtraEditors.MemoEdit();
		this.typeLookUpEdit = new DevExpress.XtraEditors.LookUpEdit();
		this.cancelSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.descriptionMemoEdit = new DevExpress.XtraEditors.MemoEdit();
		this.titleTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.okSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.fieldTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.definitionLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.fieldLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.definitionEmptySpaceItem = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.visibilityCheckedComboBoxLayoutControlItem = new DevExpress.XtraLayout.LayoutControlItem();
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.visibilityCheckedComboBox.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.classLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.definitionMemoEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeLookUpEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionMemoEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.titleTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fieldTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.definitionLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.fieldLayoutControlItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.definitionEmptySpaceItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.visibilityCheckedComboBoxLayoutControlItem).BeginInit();
		base.SuspendLayout();
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.BackColor = System.Drawing.Color.Transparent;
		this.layoutControl.Controls.Add(this.visibilityCheckedComboBox);
		this.layoutControl.Controls.Add(this.classLookUpEdit);
		this.layoutControl.Controls.Add(this.definitionMemoEdit);
		this.layoutControl.Controls.Add(this.typeLookUpEdit);
		this.layoutControl.Controls.Add(this.cancelSimpleButton);
		this.layoutControl.Controls.Add(this.descriptionMemoEdit);
		this.layoutControl.Controls.Add(this.titleTextEdit);
		this.layoutControl.Controls.Add(this.okSimpleButton);
		this.layoutControl.Controls.Add(this.fieldTextEdit);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(767, 164, 886, 614);
		this.layoutControl.OptionsView.UseDefaultDragAndDropRendering = false;
		this.layoutControl.Root = this.layoutControlGroup1;
		this.layoutControl.Size = new System.Drawing.Size(650, 471);
		this.layoutControl.TabIndex = 1;
		this.layoutControl.Text = "layoutControl1";
		this.visibilityCheckedComboBox.EditValue = ", , , , , , , , , ";
		this.visibilityCheckedComboBox.Location = new System.Drawing.Point(12, 247);
		this.visibilityCheckedComboBox.MenuManager = this.barManager;
		this.visibilityCheckedComboBox.Name = "visibilityCheckedComboBox";
		this.visibilityCheckedComboBox.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.visibilityCheckedComboBox.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.CheckedListBoxItem[10]
		{
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(null, "Documentation", System.Windows.Forms.CheckState.Checked, "Documentation"),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(null, "Key", System.Windows.Forms.CheckState.Checked, "Key"),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(null, "Subject Area", System.Windows.Forms.CheckState.Checked, "SubjectArea"),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(null, "Trigger", System.Windows.Forms.CheckState.Checked, "Trigger"),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(null, "Table/View/Structure", System.Windows.Forms.CheckState.Checked, "Structure"),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(null, "Procedure/Function", System.Windows.Forms.CheckState.Checked, "Procedure"),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(null, "Column", System.Windows.Forms.CheckState.Checked, "Column"),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(null, "Parameter", System.Windows.Forms.CheckState.Checked, "Parameter"),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(null, "Relationship", System.Windows.Forms.CheckState.Checked, "Relationship"),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(null, "Term", System.Windows.Forms.CheckState.Checked, "Term")
		});
		this.visibilityCheckedComboBox.Properties.PopupFormMinSize = new System.Drawing.Size(200, 270);
		this.visibilityCheckedComboBox.Size = new System.Drawing.Size(626, 20);
		this.visibilityCheckedComboBox.StyleController = this.layoutControl;
		this.visibilityCheckedComboBox.TabIndex = 11;
		this.visibilityCheckedComboBox.EditValueChanged += new System.EventHandler(visibilityCheckedComboBox_EditValueChanged);
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(650, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 471);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(650, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 471);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(650, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 471);
		this.classLookUpEdit.Location = new System.Drawing.Point(12, 126);
		this.classLookUpEdit.MenuManager = this.barManager;
		this.classLookUpEdit.Name = "classLookUpEdit";
		this.classLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.classLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.classLookUpEdit.Properties.DisplayMember = "Name";
		this.classLookUpEdit.Properties.DropDownRows = 9;
		this.classLookUpEdit.Properties.NullText = "";
		this.classLookUpEdit.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.OnlyInPopup;
		this.classLookUpEdit.Properties.ShowFooter = false;
		this.classLookUpEdit.Properties.ShowHeader = false;
		this.classLookUpEdit.Properties.ShowLines = false;
		this.classLookUpEdit.Properties.ValueMember = "CustomFieldClassId";
		this.classLookUpEdit.Size = new System.Drawing.Size(626, 20);
		this.classLookUpEdit.StyleController = this.layoutControl;
		this.classLookUpEdit.TabIndex = 10;
		this.classLookUpEdit.ToolTipController = this.toolTipController;
		this.classLookUpEdit.EditValueChanged += new System.EventHandler(classLookUpEdit_EditValueChanged);
		this.toolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
		this.definitionMemoEdit.EditValue = "";
		this.definitionMemoEdit.Location = new System.Drawing.Point(12, 175);
		this.definitionMemoEdit.MenuManager = this.barManager;
		this.definitionMemoEdit.MinimumSize = new System.Drawing.Size(523, 43);
		this.definitionMemoEdit.Name = "definitionMemoEdit";
		this.definitionMemoEdit.Size = new System.Drawing.Size(626, 43);
		this.definitionMemoEdit.StyleController = this.layoutControl;
		this.definitionMemoEdit.TabIndex = 9;
		this.definitionMemoEdit.ToolTipController = this.toolTipController;
		this.definitionMemoEdit.EditValueChanged += new System.EventHandler(definitionMemoEdit_EditValueChanged);
		this.typeLookUpEdit.Location = new System.Drawing.Point(12, 77);
		this.typeLookUpEdit.MenuManager = this.barManager;
		this.typeLookUpEdit.Name = "typeLookUpEdit";
		this.typeLookUpEdit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.typeLookUpEdit.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Title", "Title")
		});
		this.typeLookUpEdit.Properties.DisplayMember = "Title";
		this.typeLookUpEdit.Properties.DropDownRows = 10;
		this.typeLookUpEdit.Properties.NullText = "";
		this.typeLookUpEdit.Properties.SearchMode = DevExpress.XtraEditors.Controls.SearchMode.OnlyInPopup;
		this.typeLookUpEdit.Properties.ShowFooter = false;
		this.typeLookUpEdit.Properties.ShowHeader = false;
		this.typeLookUpEdit.Properties.ShowLines = false;
		this.typeLookUpEdit.Properties.ValueMember = "Value";
		this.typeLookUpEdit.Size = new System.Drawing.Size(626, 20);
		this.typeLookUpEdit.StyleController = this.layoutControl;
		this.typeLookUpEdit.TabIndex = 7;
		this.typeLookUpEdit.ToolTipController = this.toolTipController;
		this.typeLookUpEdit.EditValueChanged += new System.EventHandler(typeLookUpEdit_EditValueChanged);
		this.cancelSimpleButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.cancelSimpleButton.Location = new System.Drawing.Point(558, 437);
		this.cancelSimpleButton.Name = "cancelSimpleButton";
		this.cancelSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.cancelSimpleButton.StyleController = this.layoutControl;
		this.cancelSimpleButton.TabIndex = 5;
		this.cancelSimpleButton.Text = "Cancel";
		this.cancelSimpleButton.Click += new System.EventHandler(cancelSimpleButton_Click);
		this.descriptionMemoEdit.EditValue = "";
		this.descriptionMemoEdit.Location = new System.Drawing.Point(12, 296);
		this.descriptionMemoEdit.MenuManager = this.barManager;
		this.descriptionMemoEdit.Name = "descriptionMemoEdit";
		this.descriptionMemoEdit.Size = new System.Drawing.Size(626, 97);
		this.descriptionMemoEdit.StyleController = this.layoutControl;
		this.descriptionMemoEdit.TabIndex = 3;
		this.descriptionMemoEdit.ToolTipController = this.toolTipController;
		this.descriptionMemoEdit.EditValueChanged += new System.EventHandler(textEdit_EditValueChanged);
		this.titleTextEdit.Location = new System.Drawing.Point(12, 28);
		this.titleTextEdit.MenuManager = this.barManager;
		this.titleTextEdit.Name = "titleTextEdit";
		this.titleTextEdit.Size = new System.Drawing.Size(626, 20);
		this.titleTextEdit.StyleController = this.layoutControl;
		this.titleTextEdit.TabIndex = 1;
		this.titleTextEdit.ToolTipController = this.toolTipController;
		this.titleTextEdit.EditValueChanged += new System.EventHandler(textEdit_EditValueChanged);
		this.okSimpleButton.Location = new System.Drawing.Point(464, 437);
		this.okSimpleButton.Name = "okSimpleButton";
		this.okSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.okSimpleButton.StyleController = this.layoutControl;
		this.okSimpleButton.TabIndex = 4;
		this.okSimpleButton.Text = "OK";
		this.okSimpleButton.Click += new System.EventHandler(okSimpleButton_Click);
		this.fieldTextEdit.Location = new System.Drawing.Point(42, 406);
		this.fieldTextEdit.Name = "fieldTextEdit";
		this.fieldTextEdit.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.fieldTextEdit.Properties.ReadOnly = true;
		this.fieldTextEdit.Size = new System.Drawing.Size(596, 18);
		this.fieldTextEdit.StyleController = this.layoutControl;
		this.fieldTextEdit.TabIndex = 1;
		this.fieldTextEdit.TabStop = false;
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[14]
		{
			this.emptySpaceItem2, this.layoutControlItem4, this.layoutControlItem1, this.layoutControlItem3, this.emptySpaceItem3, this.layoutControlItem5, this.layoutControlItem2, this.definitionLayoutControlItem, this.fieldLayoutControlItem, this.emptySpaceItem4,
			this.emptySpaceItem1, this.definitionEmptySpaceItem, this.layoutControlItem7, this.visibilityCheckedComboBoxLayoutControlItem
		});
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Size = new System.Drawing.Size(650, 471);
		this.layoutControlGroup1.TextVisible = false;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 425);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(452, 26);
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.Control = this.titleTextEdit;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem4.MaxSize = new System.Drawing.Size(0, 49);
		this.layoutControlItem4.MinSize = new System.Drawing.Size(527, 49);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(630, 49);
		this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem4.Text = "Title";
		this.layoutControlItem4.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem4.TextSize = new System.Drawing.Size(211, 13);
		this.layoutControlItem1.Control = this.cancelSimpleButton;
		this.layoutControlItem1.Location = new System.Drawing.Point(546, 425);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(84, 26);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.TextVisible = false;
		this.layoutControlItem3.Control = this.okSimpleButton;
		this.layoutControlItem3.Location = new System.Drawing.Point(452, 425);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(536, 425);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(10, 26);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(10, 26);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.Control = this.descriptionMemoEdit;
		this.layoutControlItem5.Location = new System.Drawing.Point(0, 268);
		this.layoutControlItem5.MinSize = new System.Drawing.Size(225, 36);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(630, 117);
		this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem5.Text = "Description";
		this.layoutControlItem5.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem5.TextSize = new System.Drawing.Size(211, 13);
		this.layoutControlItem2.Control = this.typeLookUpEdit;
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 49);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(0, 49);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(527, 49);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(630, 49);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.Text = "Type";
		this.layoutControlItem2.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(211, 13);
		this.definitionLayoutControlItem.Control = this.definitionMemoEdit;
		this.definitionLayoutControlItem.Location = new System.Drawing.Point(0, 147);
		this.definitionLayoutControlItem.MaxSize = new System.Drawing.Size(0, 63);
		this.definitionLayoutControlItem.MinSize = new System.Drawing.Size(548, 63);
		this.definitionLayoutControlItem.Name = "definitionLayoutControlItem";
		this.definitionLayoutControlItem.Size = new System.Drawing.Size(630, 63);
		this.definitionLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.definitionLayoutControlItem.Text = "Definition (type in elements separated with ',')";
		this.definitionLayoutControlItem.TextLocation = DevExpress.Utils.Locations.Top;
		this.definitionLayoutControlItem.TextSize = new System.Drawing.Size(211, 13);
		this.fieldLayoutControlItem.Control = this.fieldTextEdit;
		this.fieldLayoutControlItem.CustomizationFormText = "Title";
		this.fieldLayoutControlItem.Location = new System.Drawing.Point(0, 394);
		this.fieldLayoutControlItem.MaxSize = new System.Drawing.Size(0, 22);
		this.fieldLayoutControlItem.MinSize = new System.Drawing.Size(278, 22);
		this.fieldLayoutControlItem.Name = "fieldLayoutControlItem";
		this.fieldLayoutControlItem.Size = new System.Drawing.Size(630, 22);
		this.fieldLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.fieldLayoutControlItem.Text = "Field:";
		this.fieldLayoutControlItem.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
		this.fieldLayoutControlItem.TextSize = new System.Drawing.Size(25, 13);
		this.fieldLayoutControlItem.TextToControlDistance = 5;
		this.emptySpaceItem4.AllowHotTrack = false;
		this.emptySpaceItem4.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem4.Location = new System.Drawing.Point(0, 385);
		this.emptySpaceItem4.MaxSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem4.MinSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem4.Name = "emptySpaceItem4";
		this.emptySpaceItem4.Size = new System.Drawing.Size(630, 9);
		this.emptySpaceItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem4.Text = "emptySpaceItem1";
		this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem1.Location = new System.Drawing.Point(0, 416);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(527, 9);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(630, 9);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.definitionEmptySpaceItem.AllowHotTrack = false;
		this.definitionEmptySpaceItem.Location = new System.Drawing.Point(0, 210);
		this.definitionEmptySpaceItem.MaxSize = new System.Drawing.Size(0, 9);
		this.definitionEmptySpaceItem.MinSize = new System.Drawing.Size(104, 9);
		this.definitionEmptySpaceItem.Name = "definitionEmptySpaceItem";
		this.definitionEmptySpaceItem.Size = new System.Drawing.Size(630, 9);
		this.definitionEmptySpaceItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.definitionEmptySpaceItem.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem7.Control = this.classLookUpEdit;
		this.layoutControlItem7.Location = new System.Drawing.Point(0, 98);
		this.layoutControlItem7.MaxSize = new System.Drawing.Size(0, 49);
		this.layoutControlItem7.MinSize = new System.Drawing.Size(527, 49);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Size = new System.Drawing.Size(630, 49);
		this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem7.Text = "Class";
		this.layoutControlItem7.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem7.TextSize = new System.Drawing.Size(211, 13);
		this.visibilityCheckedComboBoxLayoutControlItem.Control = this.visibilityCheckedComboBox;
		this.visibilityCheckedComboBoxLayoutControlItem.Location = new System.Drawing.Point(0, 219);
		this.visibilityCheckedComboBoxLayoutControlItem.MaxSize = new System.Drawing.Size(0, 49);
		this.visibilityCheckedComboBoxLayoutControlItem.MinSize = new System.Drawing.Size(400, 49);
		this.visibilityCheckedComboBoxLayoutControlItem.Name = "visibilityCheckedComboBoxLayoutControlItem";
		this.visibilityCheckedComboBoxLayoutControlItem.Size = new System.Drawing.Size(630, 49);
		this.visibilityCheckedComboBoxLayoutControlItem.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.visibilityCheckedComboBoxLayoutControlItem.Text = "Visibility";
		this.visibilityCheckedComboBoxLayoutControlItem.TextLocation = DevExpress.Utils.Locations.Top;
		this.visibilityCheckedComboBoxLayoutControlItem.TextSize = new System.Drawing.Size(211, 13);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(650, 471);
		base.Controls.Add(this.layoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("CustomFieldForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		this.MinimumSize = new System.Drawing.Size(650, 501);
		base.Name = "CustomFieldForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Custom field";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(CustomFieldForm_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.visibilityCheckedComboBox.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.classLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.definitionMemoEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeLookUpEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionMemoEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.titleTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fieldTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.definitionLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.fieldLayoutControlItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.definitionEmptySpaceItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.visibilityCheckedComboBoxLayoutControlItem).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
