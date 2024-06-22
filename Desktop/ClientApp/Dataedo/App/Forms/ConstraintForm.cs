using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dataedo.App.Classes;
using Dataedo.App.Classes.Synchronize;
using Dataedo.App.Data.MetadataServer;
using Dataedo.App.Enums;
using Dataedo.App.Properties;
using Dataedo.App.Tools;
using Dataedo.App.Tools.MessageBoxes;
using Dataedo.App.UserControls.RadioGroupWithImages;
using Dataedo.CustomControls;
using Dataedo.DataProcessing.Synchronize;
using Dataedo.Model.Enums;
using Dataedo.Shared.Enums;
using DevExpress.Utils;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;

namespace Dataedo.App.Forms;

public class ConstraintForm : BaseXtraForm
{
	private UniqueConstraintRow constraint;

	private List<UniqueConstraintColumnRow> existingKeyColumns;

	private bool constraintChanged;

	private string autoName = string.Empty;

	private bool isEditMode;

	private int tableId;

	private UniqueConstraintType.UniqueConstraintTypeEnum type;

	private int typeInt;

	private IEnumerable<UniqueConstraintColumnRow> datasourceColumns;

	private DXErrorProvider errorProvider;

	private IContainer components;

	private NonCustomizableLayoutControl layoutControl;

	private LayoutControlGroup layoutControlGroup;

	private TextEdit nameTextEdit;

	private LayoutControlItem layoutControlItem1;

	private SimpleButton okSimpleButton;

	private SimpleButton closeSimpleButton;

	private LayoutControlItem layoutControlItem2;

	private LayoutControlItem layoutControlItem3;

	private EmptySpaceItem emptySpaceItem1;

	private EmptySpaceItem emptySpaceItem2;

	private ImageCollection contraintObjectImageCollection;

	private MemoEdit descriptionMemoEdit;

	private LayoutControlItem layoutControlItem6;

	private RadioGroupWithImages typeRadioGroup;

	private LayoutControlItem layoutControlItem7;

	private EmptySpaceItem emptySpaceItem3;

	private LabelControl columnsLabelControl;

	private CheckedListBoxControl checkedListBoxControl;

	private LayoutControlItem layoutControlItem4;

	private LayoutControlItem layoutControlItem5;

	private BarManager barManager;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	public ConstraintForm(UniqueConstraintRow constraint, IEnumerable<ColumnRow> tableColumns, int databaseId, int tableId, string tableName, bool editMode = true)
		: this(databaseId, editMode)
	{
		errorProvider = new DXErrorProvider();
		this.constraint = constraint;
		if (string.IsNullOrEmpty(this.constraint.TableName))
		{
			this.constraint.TableName = tableName;
		}
		existingKeyColumns = new List<UniqueConstraintColumnRow>();
		this.tableId = tableId;
		datasourceColumns = ConvertColumnRowToUniqueConstraintColumnRow(tableColumns);
		if (!editMode)
		{
			typeRadioGroup.SelectedIndex = ((!this.constraint.IsPK) ? 1 : 0);
			return;
		}
		nameTextEdit.Text = (autoName = constraint.Name);
		descriptionMemoEdit.Text = constraint.Description;
		typeRadioGroup.EditValue = constraint.UniqueConstraintTypeInt;
		foreach (UniqueConstraintColumnRow column in constraint.Columns)
		{
			existingKeyColumns.Add(new UniqueConstraintColumnRow(column));
			if (datasourceColumns != null && datasourceColumns.Count() > 0)
			{
				UniqueConstraintColumnRow uniqueConstraintColumnRow = datasourceColumns.FirstOrDefault((UniqueConstraintColumnRow x) => x.ColumnId == column.ColumnId);
				if (uniqueConstraintColumnRow != null)
				{
					uniqueConstraintColumnRow.IsChecked = true;
				}
			}
		}
	}

	public void SetCheckedColumns(IEnumerable<BaseRow> columns)
	{
		foreach (UniqueConstraintColumnRow item in datasourceColumns.Where((UniqueConstraintColumnRow x) => columns.Any((BaseRow y) => y.Id == x.ColumnId)))
		{
			item.IsChecked = true;
		}
		nameTextEdit.Text = (autoName = GetConstraintName());
	}

	public ConstraintForm(int databaseId, bool editMode = true)
	{
		InitializeComponent();
		isEditMode = editMode;
		Text = (editMode ? "User-defined unique key" : "New user-defined unique key");
		LengthValidation.SetTitleOrNameLengthLimit(nameTextEdit);
	}

	private IEnumerable<UniqueConstraintColumnRow> ConvertColumnRowToUniqueConstraintColumnRow(IEnumerable<ColumnRow> tableColumns)
	{
		List<UniqueConstraintColumnRow> list = new List<UniqueConstraintColumnRow>();
		int num = 0;
		foreach (ColumnRow tableColumn in tableColumns)
		{
			list.Add(new UniqueConstraintColumnRow
			{
				ConstraintId = constraint.Id,
				ColumnId = tableColumn.Id,
				OrdinalPosition = num++,
				ColumnName = tableColumn.Name,
				ColumnPath = tableColumn.Path,
				ColumnTitle = tableColumn.Title,
				ColumnParentId = tableColumn.ParentId,
				RowState = ManagingRowsEnum.ManagingRows.Unchanged
			});
		}
		return list;
	}

	private void ConstraintForm_Load(object sender, EventArgs e)
	{
		checkedListBoxControl.DataSource = datasourceColumns;
	}

	private string GetConstraintName()
	{
		StringBuilder stringBuilder = new StringBuilder();
		IEnumerable<string> enumerable = from x in GetCheckedColumns()
			select x.ColumnName;
		string value = ((type == UniqueConstraintType.UniqueConstraintTypeEnum.PK_user) ? "pk" : "uk");
		stringBuilder.Append(value);
		stringBuilder.Append("_" + constraint.TableName);
		foreach (string item in enumerable)
		{
			stringBuilder.Append("_" + item);
		}
		string text = stringBuilder.ToString();
		if (text.Length <= 100)
		{
			return text;
		}
		return text.Substring(0, 99);
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
		if (type == UniqueConstraintType.UniqueConstraintTypeEnum.NotSet)
		{
			errorProvider.SetError(typeRadioGroup, "Type is required");
			base.DialogResult = DialogResult.None;
			return;
		}
		BindingList<UniqueConstraintColumnRow> checkedColumns = GetCheckedColumns();
		constraint.Name = nameTextEdit.Text;
		constraint.Description = descriptionMemoEdit.Text;
		constraint.Columns = checkedColumns;
		constraint.TableId = tableId;
		constraint.Source = UserTypeEnum.UserType.USER;
		constraint.Type = UniqueConstraintType.ToString(type);
		constraint.UniqueConstraintTypeInt = typeInt;
		if (constraint.Id == 0)
		{
			DB.Constraint.Insert(constraint, this);
		}
		else
		{
			DB.Constraint.Update(constraint, this);
		}
		base.DialogResult = DialogResult.OK;
	}

	private BindingList<UniqueConstraintColumnRow> GetCheckedColumns()
	{
		BindingList<UniqueConstraintColumnRow> bindingList = new BindingList<UniqueConstraintColumnRow>();
		foreach (UniqueConstraintColumnRow item in datasourceColumns.Where((UniqueConstraintColumnRow x) => x.IsChecked))
		{
			bindingList.Add(item);
		}
		return bindingList;
	}

	private void okSimpleButton_Click(object sender, EventArgs e)
	{
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void closeSimpleButton_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void constraintColumnsGridView_PopupMenuShowing(object sender, DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventArgs e)
	{
		CommonFunctionsPanels.ManageOptionsInHeaderPopup(e);
	}

	private void descriptionMemoEdit_EditValueChanged(object sender, EventArgs e)
	{
		constraintChanged = (sender as BaseEdit).IsModified;
	}

	private void nameTextEdit_EditValueChanged(object sender, EventArgs e)
	{
		constraintChanged = (sender as BaseEdit).IsModified;
	}

	private void typeRadioGroup_EditValueChanged(object sender, EventArgs e)
	{
		RadioGroup radioGroup = sender as RadioGroup;
		int num = ((radioGroup != null && radioGroup.EditValue != null) ? ((int)radioGroup.EditValue) : (-1));
		type = UniqueConstraintType.ToType(num);
		typeInt = num;
		if (!isEditMode && autoName.Equals(nameTextEdit.Text))
		{
			nameTextEdit.Text = (autoName = GetConstraintName());
		}
		constraintChanged = radioGroup.IsModified;
	}

	private void typeRadioGroup_CustomItemImage(object sender, CustomItemImageEventArgs e)
	{
		int? num = e.Item.Value as int?;
		if (num.HasValue)
		{
			e.Image = contraintObjectImageCollection.Images[num.Value];
		}
	}

	private void ConstraintForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (base.DialogResult == DialogResult.OK)
		{
			Save();
		}
		else if (constraintChanged)
		{
			DialogResult? dialogResult = GeneralMessageBoxesHandling.Show("Constraint has been changed, would you like to save these changes?", "Relationship has been changed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation, null, 1, this).DialogResult;
			if (dialogResult == DialogResult.Yes)
			{
				Save();
			}
			else if (dialogResult != DialogResult.No)
			{
				base.DialogResult = DialogResult.Cancel;
				e.Cancel = true;
			}
		}
	}

	private void checkedListBoxControl_ItemCheck(object sender, DevExpress.XtraEditors.Controls.ItemCheckEventArgs e)
	{
		if (!isEditMode && autoName.Equals(nameTextEdit.Text))
		{
			nameTextEdit.Text = (autoName = GetConstraintName());
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Dataedo.App.Forms.ConstraintForm));
		this.columnsLabelControl = new DevExpress.XtraEditors.LabelControl();
		this.layoutControl = new Dataedo.CustomControls.NonCustomizableLayoutControl();
		this.checkedListBoxControl = new DevExpress.XtraEditors.CheckedListBoxControl();
		this.typeRadioGroup = new Dataedo.App.UserControls.RadioGroupWithImages.RadioGroupWithImages();
		this.descriptionMemoEdit = new DevExpress.XtraEditors.MemoEdit();
		this.barManager = new DevExpress.XtraBars.BarManager(this.components);
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.okSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.closeSimpleButton = new DevExpress.XtraEditors.SimpleButton();
		this.nameTextEdit = new DevExpress.XtraEditors.TextEdit();
		this.layoutControlGroup = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
		this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
		this.contraintObjectImageCollection = new DevExpress.Utils.ImageCollection(this.components);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).BeginInit();
		this.layoutControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.checkedListBoxControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.typeRadioGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionMemoEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.nameTextEdit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.contraintObjectImageCollection).BeginInit();
		base.SuspendLayout();
		this.columnsLabelControl.Location = new System.Drawing.Point(12, 115);
		this.columnsLabelControl.Name = "columnsLabelControl";
		this.columnsLabelControl.Size = new System.Drawing.Size(44, 13);
		this.columnsLabelControl.StyleController = this.layoutControl;
		this.columnsLabelControl.TabIndex = 7;
		this.columnsLabelControl.Text = "Columns:";
		this.layoutControl.AllowCustomization = false;
		this.layoutControl.Controls.Add(this.columnsLabelControl);
		this.layoutControl.Controls.Add(this.checkedListBoxControl);
		this.layoutControl.Controls.Add(this.typeRadioGroup);
		this.layoutControl.Controls.Add(this.descriptionMemoEdit);
		this.layoutControl.Controls.Add(this.okSimpleButton);
		this.layoutControl.Controls.Add(this.closeSimpleButton);
		this.layoutControl.Controls.Add(this.nameTextEdit);
		this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl.Location = new System.Drawing.Point(0, 0);
		this.layoutControl.Name = "layoutControl";
		this.layoutControl.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(872, 95, 250, 350);
		this.layoutControl.Root = this.layoutControlGroup;
		this.layoutControl.Size = new System.Drawing.Size(568, 503);
		this.layoutControl.TabIndex = 0;
		this.layoutControl.Text = "layoutControl1";
		this.checkedListBoxControl.AllowHtmlDraw = DevExpress.Utils.DefaultBoolean.True;
		this.checkedListBoxControl.CheckMember = "IsChecked";
		this.checkedListBoxControl.DisplayMember = "ColumnFullNameFormattedWithTitle";
		this.checkedListBoxControl.Location = new System.Drawing.Point(12, 132);
		this.checkedListBoxControl.Name = "checkedListBoxControl";
		this.checkedListBoxControl.Size = new System.Drawing.Size(544, 178);
		this.checkedListBoxControl.StyleController = this.layoutControl;
		this.checkedListBoxControl.TabIndex = 6;
		this.checkedListBoxControl.ItemCheck += new DevExpress.XtraEditors.Controls.ItemCheckEventHandler(checkedListBoxControl_ItemCheck);
		this.typeRadioGroup.Location = new System.Drawing.Point(12, 77);
		this.typeRadioGroup.Name = "typeRadioGroup";
		this.typeRadioGroup.Properties.AllowFocused = false;
		this.typeRadioGroup.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.typeRadioGroup.Properties.Appearance.Options.UseBackColor = true;
		this.typeRadioGroup.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.typeRadioGroup.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(0, "Primary key", true, "pk"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(4, "Unique key", true, "uk")
		});
		this.typeRadioGroup.Size = new System.Drawing.Size(227, 25);
		this.typeRadioGroup.StyleController = this.layoutControl;
		this.typeRadioGroup.TabIndex = 1;
		this.typeRadioGroup.CustomItemImage += new Dataedo.App.UserControls.RadioGroupWithImages.RadioGroupWithImages.CustomItemImageHandler(typeRadioGroup_CustomItemImage);
		this.typeRadioGroup.EditValueChanged += new System.EventHandler(typeRadioGroup_EditValueChanged);
		this.descriptionMemoEdit.Location = new System.Drawing.Point(12, 330);
		this.descriptionMemoEdit.MenuManager = this.barManager;
		this.descriptionMemoEdit.Name = "descriptionMemoEdit";
		this.descriptionMemoEdit.Size = new System.Drawing.Size(544, 126);
		this.descriptionMemoEdit.StyleController = this.layoutControl;
		this.descriptionMemoEdit.TabIndex = 3;
		this.descriptionMemoEdit.EditValueChanged += new System.EventHandler(descriptionMemoEdit_EditValueChanged);
		this.barManager.DockControls.Add(this.barDockControlTop);
		this.barManager.DockControls.Add(this.barDockControlBottom);
		this.barManager.DockControls.Add(this.barDockControlLeft);
		this.barManager.DockControls.Add(this.barDockControlRight);
		this.barManager.Form = this;
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager;
		this.barDockControlTop.Size = new System.Drawing.Size(568, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 503);
		this.barDockControlBottom.Manager = this.barManager;
		this.barDockControlBottom.Size = new System.Drawing.Size(568, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 503);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(568, 0);
		this.barDockControlRight.Manager = this.barManager;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 503);
		this.okSimpleButton.Location = new System.Drawing.Point(380, 469);
		this.okSimpleButton.Name = "okSimpleButton";
		this.okSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.okSimpleButton.StyleController = this.layoutControl;
		this.okSimpleButton.TabIndex = 4;
		this.okSimpleButton.Text = "OK";
		this.okSimpleButton.Click += new System.EventHandler(okSimpleButton_Click);
		this.closeSimpleButton.Location = new System.Drawing.Point(476, 469);
		this.closeSimpleButton.Name = "closeSimpleButton";
		this.closeSimpleButton.Size = new System.Drawing.Size(80, 22);
		this.closeSimpleButton.StyleController = this.layoutControl;
		this.closeSimpleButton.TabIndex = 5;
		this.closeSimpleButton.Text = "Cancel";
		this.closeSimpleButton.Click += new System.EventHandler(closeSimpleButton_Click);
		this.nameTextEdit.Location = new System.Drawing.Point(12, 28);
		this.nameTextEdit.MenuManager = this.barManager;
		this.nameTextEdit.Name = "nameTextEdit";
		this.nameTextEdit.Size = new System.Drawing.Size(544, 20);
		this.nameTextEdit.StyleController = this.layoutControl;
		this.nameTextEdit.TabIndex = 0;
		this.nameTextEdit.EditValueChanged += new System.EventHandler(nameTextEdit_EditValueChanged);
		this.layoutControlGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup.GroupBordersVisible = false;
		this.layoutControlGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[10] { this.layoutControlItem1, this.layoutControlItem2, this.layoutControlItem3, this.emptySpaceItem1, this.emptySpaceItem2, this.layoutControlItem6, this.layoutControlItem7, this.emptySpaceItem3, this.layoutControlItem4, this.layoutControlItem5 });
		this.layoutControlGroup.Name = "Root";
		this.layoutControlGroup.Size = new System.Drawing.Size(568, 503);
		this.layoutControlGroup.TextVisible = false;
		this.layoutControlItem1.Control = this.nameTextEdit;
		this.layoutControlItem1.CustomizationFormText = "Name:";
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 49);
		this.layoutControlItem1.MinSize = new System.Drawing.Size(57, 49);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 11);
		this.layoutControlItem1.Size = new System.Drawing.Size(548, 49);
		this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem1.Text = "Name:";
		this.layoutControlItem1.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem1.TextSize = new System.Drawing.Size(57, 13);
		this.layoutControlItem2.Control = this.closeSimpleButton;
		this.layoutControlItem2.Location = new System.Drawing.Point(464, 457);
		this.layoutControlItem2.MaxSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem2.MinSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(84, 26);
		this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem2.TextVisible = false;
		this.layoutControlItem3.Control = this.okSimpleButton;
		this.layoutControlItem3.Location = new System.Drawing.Point(368, 457);
		this.layoutControlItem3.MaxSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.MinSize = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(84, 26);
		this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem3.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.Location = new System.Drawing.Point(452, 457);
		this.emptySpaceItem1.MaxSize = new System.Drawing.Size(12, 26);
		this.emptySpaceItem1.MinSize = new System.Drawing.Size(12, 26);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(12, 26);
		this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 457);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(0, 26);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(1, 26);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(368, 26);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem6.Control = this.descriptionMemoEdit;
		this.layoutControlItem6.Location = new System.Drawing.Point(0, 302);
		this.layoutControlItem6.MaxSize = new System.Drawing.Size(0, 155);
		this.layoutControlItem6.MinSize = new System.Drawing.Size(57, 155);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 11);
		this.layoutControlItem6.Size = new System.Drawing.Size(548, 155);
		this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem6.Text = "Description:";
		this.layoutControlItem6.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem6.TextSize = new System.Drawing.Size(57, 13);
		this.layoutControlItem7.Control = this.typeRadioGroup;
		this.layoutControlItem7.Location = new System.Drawing.Point(0, 49);
		this.layoutControlItem7.MaxSize = new System.Drawing.Size(231, 54);
		this.layoutControlItem7.MinSize = new System.Drawing.Size(231, 54);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 11);
		this.layoutControlItem7.Size = new System.Drawing.Size(231, 54);
		this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem7.Text = "Type:";
		this.layoutControlItem7.TextLocation = DevExpress.Utils.Locations.Top;
		this.layoutControlItem7.TextSize = new System.Drawing.Size(57, 13);
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.Location = new System.Drawing.Point(231, 49);
		this.emptySpaceItem3.MaxSize = new System.Drawing.Size(188, 54);
		this.emptySpaceItem3.MinSize = new System.Drawing.Size(188, 54);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(317, 54);
		this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.Control = this.checkedListBoxControl;
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 120);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(548, 182);
		this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem4.TextVisible = false;
		this.layoutControlItem5.Control = this.columnsLabelControl;
		this.layoutControlItem5.Location = new System.Drawing.Point(0, 103);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(548, 17);
		this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem5.TextVisible = false;
		this.contraintObjectImageCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("contraintObjectImageCollection.ImageStream");
		this.contraintObjectImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_user_16, "primary_key_user_16", typeof(Dataedo.App.Properties.Resources), 0);
		this.contraintObjectImageCollection.Images.SetKeyName(0, "primary_key_user_16");
		this.contraintObjectImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_deleted_16, "primary_key_deleted_16", typeof(Dataedo.App.Properties.Resources), 1);
		this.contraintObjectImageCollection.Images.SetKeyName(1, "primary_key_deleted_16");
		this.contraintObjectImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_16, "primary_key_16", typeof(Dataedo.App.Properties.Resources), 2);
		this.contraintObjectImageCollection.Images.SetKeyName(2, "primary_key_16");
		this.contraintObjectImageCollection.InsertImage(Dataedo.App.Properties.Resources.primary_key_disabled_16, "primary_key_disabled_16", typeof(Dataedo.App.Properties.Resources), 3);
		this.contraintObjectImageCollection.Images.SetKeyName(3, "primary_key_disabled_16");
		this.contraintObjectImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_user_16, "unique_key_user_16", typeof(Dataedo.App.Properties.Resources), 4);
		this.contraintObjectImageCollection.Images.SetKeyName(4, "unique_key_user_16");
		this.contraintObjectImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_deleted_16, "unique_key_deleted_16", typeof(Dataedo.App.Properties.Resources), 5);
		this.contraintObjectImageCollection.Images.SetKeyName(5, "unique_key_deleted_16");
		this.contraintObjectImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_16, "unique_key_16", typeof(Dataedo.App.Properties.Resources), 6);
		this.contraintObjectImageCollection.Images.SetKeyName(6, "unique_key_16");
		this.contraintObjectImageCollection.InsertImage(Dataedo.App.Properties.Resources.unique_key_disabled_16, "unique_key_disabled_16", typeof(Dataedo.App.Properties.Resources), 7);
		this.contraintObjectImageCollection.Images.SetKeyName(7, "unique_key_disabled_16");
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(568, 503);
		base.Controls.Add(this.layoutControl);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("ConstraintForm.IconOptions.Icon");
		base.IconOptions.Image = Dataedo.App.Properties.Resources.icon;
		base.Name = "ConstraintForm";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Unique key";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(ConstraintForm_FormClosing);
		base.Load += new System.EventHandler(ConstraintForm_Load);
		((System.ComponentModel.ISupportInitialize)this.layoutControl).EndInit();
		this.layoutControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.checkedListBoxControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.typeRadioGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.descriptionMemoEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.nameTextEdit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.contraintObjectImageCollection).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
